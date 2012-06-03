using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{
    partial class Mame
    {
        class tDatafileIndex
        {
            public int offset;
            public GameDriver driver;
        }
        string history_filename = null, mameinfo_filename = null;
        const int bufsize = 256 * 1024;
        static tDatafileIndex[] hist_idx = null;
        static tDatafileIndex[] mame_idx = null;
        static string DATAFILE_TAG_KEY = "$info";
        static string DATAFILE_TAG_BIO = "$bio";
        static string DATAFILE_TAG_MAME = "$mame";
        const uint TOKEN_COMMA = 0, TOKEN_EQUALS = 1, TOKEN_SYMBOL = 2, TOKEN_LINEBREAK = 3, TOKEN_INVALID = unchecked((uint)-1);
        const byte CR = 0x0de, LF = 0x0a;

        Stream fp;
        uint dwFilePos;
        byte[] bToken = new byte[256];
        struct driver_data_type
        {
            public string name;
            public int index;
        }
        List<driver_data_type> sorted_drivers = new List<driver_data_type>();
        int num_games;
        const int MAX_DATAFILE_ENTRIES = 256;

        bool ParseOpen(string pszFilename)
        {
            fp = fopen(pszFilename, "rb");
            if (fp == null) return false;
            dwFilePos = 0;
            return true;
        }
        void ParseClose()
        {
            fp.Close();
            fp = null;
        }
        bool ParseSeek(int offset, int whence)
        {
            if (whence == SEEK_SET)
                fp.Seek(offset, SeekOrigin.Begin);
            else
                throw new Exception();
            dwFilePos = (uint)fp.Position;
            return false;
        }
        uint GetNextToken(ref string ppszTokenText, ref uint pdwPosition)
        {
            uint dwLength;                                                /* Length of symbol */
            int dwPos;                                                             /* Temporary position */
            int pbTokenPtr = 0;                             /* Point to the beginning */
            byte bData;                                                    /* Temporary data byte */

            while (true)
            {
                bData = (byte)fp.ReadByte();                                  /* Get next character */

                /* If we're at the end of the file, bail out */

                if (fp.Position == fp.Length)
                    return (TOKEN_INVALID);

                /* If it's not whitespace, then let's start eating characters */

                if (' ' != bData && '\t' != bData)
                {
                    /* Store away our file position (if given on input) */

                    if (pdwPosition != 0)
                        pdwPosition = dwFilePos;

                    /* If it's a separator, special case it */

                    if (',' == bData || '=' == bData)
                    {
                        bToken[pbTokenPtr++] = bData;
                        bToken[pbTokenPtr++] = (byte)'\0';
                        ++dwFilePos;

                        if (',' == bData)
                            return (TOKEN_COMMA);
                        else
                            return (TOKEN_EQUALS);
                    }

                    /* Otherwise, let's try for a symbol */

                    if (bData > ' ')
                    {
                        dwLength = 0;                   /* Assume we're 0 length to start with */

                        /* Loop until we've hit something we don't understand */

                        while (bData != ',' &&
                                         bData != '=' &&
                                         bData != ' ' &&
                                         bData != '\t' &&
                                         bData != '\n' &&
                                         bData != '\r' &&
                                         fp.Position < fp.Length)
                        {
                            ++dwFilePos;
                            bToken[pbTokenPtr++] = bData;  /* Store our byte */
                            ++dwLength;
                            //assert(dwLength < MAX_TOKEN_LENGTH);
                            bData = (byte)fp.ReadByte();
                        }

                        /* If it's not the end of the file, put the last received byte */
                        /* back. We don't want to touch the file position, though if */
                        /* we're past the end of the file. Otherwise, adjust it. */

                        if (fp.Position < fp.Length)
                        {
                            fp.Position--;// mame_ungetc(bData, fp);
                        }

                        /* Null terminate the token */

                        bToken[pbTokenPtr] = (byte)'\0';

                        /* Connect up the */

                        if (ppszTokenText != null)
                            ppszTokenText = Encoding.Default.GetString(bToken);

                        return (TOKEN_SYMBOL);
                    }

                    /* Not a symbol. Let's see if it's a cr/cr, lf/lf, or cr/lf/cr/lf */
                    /* sequence */

                    if (LF == bData)
                    {
                        /* Unix style perhaps? */

                        bData = (byte)fp.ReadByte();          /* Peek ahead */
                        fp.Position--;// mame_ungetc(bData, fp);          /* Force a retrigger if subsequent LF's */

                        if (LF == bData)                /* Two LF's in a row - it's a UNIX hard CR */
                        {
                            ++dwFilePos;
                            bToken[pbTokenPtr++] = bData;  /* A real linefeed */
                            bToken[pbTokenPtr] = (byte)'\0';
                            return (TOKEN_LINEBREAK);
                        }

                        /* Otherwise, fall through and keep parsing. */

                    }
                    else
                        if (CR == bData)                /* Carriage return? */
                        {
                            /* Figure out if it's Mac or MSDOS format */

                            ++dwFilePos;
                            bData = (byte)fp.ReadByte();          /* Peek ahead */

                            /* We don't need to bother with EOF checking. It will be 0xff if */
                            /* it's the end of the file and will be caught by the outer loop. */

                            if (CR == bData)                /* Mac style hard return! */
                            {
                                /* Do not advance the file pointer in case there are successive */
                                /* CR/CR sequences */

                                /* Stuff our character back upstream for successive CR's */

                                fp.Position--;// mame_ungetc(bData, fp);

                                bToken[pbTokenPtr++] = bData;  /* A real carriage return (hard) */
                                bToken[pbTokenPtr] = (byte)'\0';
                                return (TOKEN_LINEBREAK);
                            }
                            else
                                if (LF == bData)        /* MSDOS format! */
                                {
                                    ++dwFilePos;                    /* Our file position to reset to */
                                    dwPos = (int)dwFilePos;              /* Here so we can reposition things */

                                    /* Look for a followup CR/LF */

                                    bData = (byte)fp.ReadByte();  /* Get the next byte */

                                    if (CR == bData)        /* CR! Good! */
                                    {
                                        bData = (byte)fp.ReadByte();  /* Get the next byte */

                                        /* We need to do this to pick up subsequent CR/LF sequences */
                                        fp.Seek(dwPos, SeekOrigin.Begin);
                                        //fseek(fp, dwPos, SEEK_SET);

                                        if (pdwPosition!=0)
                                            pdwPosition = (uint)dwPos;

                                        if (LF == bData)        /* LF? Good! */
                                        {
                                            bToken[pbTokenPtr++] = (byte)'\r';
                                            bToken[pbTokenPtr++] = (byte)'\n';
                                            bToken[pbTokenPtr] = (byte)'\0';

                                            return (TOKEN_LINEBREAK);
                                        }
                                    }
                                    else
                                    {
                                        --dwFilePos;
                                        fp.Position--;  /* Put the character back. No good */
                                    }
                                }
                                else
                                {
                                    --dwFilePos;
                                    fp.Position--;  /* Put the character back. No good */
                                }

                            /* Otherwise, fall through and keep parsing */
                        }
                }

                ++dwFilePos;
            }
        }
        static int ci_strncmp(string s1, string s2, int n)
        {
            int c1, c2;
            int si1 = 0, si2 = 0;
            while (n != 0)
            {
                if ((c1 = char.ToUpper(s1[si1])) != (c2 = char.ToUpper(s2[si2])))
                    return (c1 - c2);
                else if (c1 == 0)
                    break;
                --n;

                si1++;
                si2++;
            }
            return 0;
        }
        int index_datafile(ref tDatafileIndex[] _index)
        {
            tDatafileIndex idx;
            int ix = 0;
            int count = 0;
            uint token = TOKEN_SYMBOL;

            /* rewind file */
            if (ParseSeek(0, SEEK_SET)) return 0;

            /* allocate index */
            _index = new tDatafileIndex[MAX_DATAFILE_ENTRIES];
            idx = _index[ix];

            /* loop through datafile */
            while ((count < (MAX_DATAFILE_ENTRIES - 1)) && TOKEN_INVALID != token)
            {
                uint tell = 0;
                string s = "";

                token = GetNextToken(ref s, ref tell);
                if (TOKEN_SYMBOL != token) continue;

                /* DATAFILE_TAG_KEY identifies the driver */
                if (ci_strncmp(DATAFILE_TAG_KEY, s, DATAFILE_TAG_KEY.Length) == 0)
                {
                    token = GetNextToken(ref s, ref tell);
                    if (TOKEN_EQUALS == token)
                    {
                        bool done = false;

                        token = GetNextToken(ref s, ref tell);
                        while (!done && TOKEN_SYMBOL == token)
                        {
                            /* search for matching driver name */
                            foreach (string gn in drivers)
                            {
                                if (s.ToLower().CompareTo(gn.ToLower()) == 0)
                                {
                                    /* found correct driver -- fill in index entry */
                                    idx.driver = GetDriver(gn);
                                    idx.offset = (int)tell;
                                    idx = _index[++ix];
                                    count++;
                                    done = true;
                                    break;
                                }
                            }

                            if (!done)
                            {
                                token = GetNextToken(ref s, ref tell);
                                if (TOKEN_COMMA == token)
                                    token = GetNextToken(ref s, ref tell);
                                else
                                    done = true; /* end of key field */
                            }
                        }
                    }
                }
            }

            /* mark end of index */
            idx.offset = 0;
            idx.driver = null;
            return count;
        }
        int load_datafile_text(GameDriver drv, ref string buffer, int bufsize, tDatafileIndex[] idx, string tag)
        {
            throw new Exception();
        }
        bool load_driver_history(GameDriver drv, ref string buffer)
        {
            bool history = false, mameinfo = false;
            int err;

            buffer = "";

            if (history_filename == null)
                history_filename = "history.dat";

            /* try to open history datafile */
            if (ParseOpen(history_filename))
            {
                /* create index if necessary */
                if (hist_idx != null)
                    history = true;
                else
                    history = (index_datafile(ref hist_idx) != 0);

                /* load history text */
                if (hist_idx != null)
                {
                    GameDriver gdrv = drv;

                    do
                    {
                        err = load_datafile_text(gdrv, ref  buffer, bufsize, hist_idx, DATAFILE_TAG_BIO);
                        gdrv = gdrv.clone_of;
                    } while (err != 0 && gdrv != null);

                    if (err != 0) history = false;
                }
                ParseClose();
            }

            if (mameinfo_filename == null)
                mameinfo_filename = "mameinfo.dat";

            /* try to open mameinfo datafile */
            if (ParseOpen(mameinfo_filename))
            {
                /* create index if necessary */
                if (mame_idx != null)
                    mameinfo = true;
                else
                    mameinfo = (index_datafile(ref mame_idx) != 0);

                /* load informational text (append) */
                if (mame_idx != null)
                {
                    int len = buffer.Length;
                    GameDriver gdrv = drv;

                    gdrv = drv; do
                    {
                        err = load_datafile_text(gdrv, ref buffer, bufsize - len, mame_idx, DATAFILE_TAG_MAME);
                        gdrv = gdrv.clone_of;
                    } while (err != 0 && gdrv != null);

                    if (err != 0) mameinfo = false;
                }
                ParseClose();
            }

            return (history == false && mameinfo == false);
        }
    }
}
