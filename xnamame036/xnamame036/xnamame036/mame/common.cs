using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int COIN_COUNTERS = 4;
        static uint dispensed_tickets;
        static uint[] coins = new uint[COIN_COUNTERS];
        static uint[] lastcoin = new uint[COIN_COUNTERS];
        static uint[] coinlockedout = new uint[COIN_COUNTERS];


        static Random _rnd = new Random();
        public static int rand()
        {
            return _rnd.Next();
        }
        public class RomModule
        {
            public RomModule(string name, uint offset, uint length, uint crc)
            {
                this.name = name;
                this.offset = offset;
                this.length = length;
                this.crc = crc;
            }
            public string name;
            public uint offset, length, crc;
        }
        public class GameSample
        {
            public GameSample(int len) { data = new byte[len]; }
            public int length, smpfreq, resolution;
            public byte[] data = new byte[1];
        }
        public class GameSamples
        {
            public int total;
            public GameSample[] sample = new GameSample[1];

            public GameSamples(int size)
            {
                sample = new GameSample[size];
                for (int i = 0; i < size; i++)
                    sample[i] = new GameSample(1);
            }
            public GameSamples()
            {
                sample[0] = new GameSample(1);
            }
        }
        public const int
    REGION_INVALID = 0x80,
    REGION_CPU1 = 0x81,
    REGION_CPU2 = 0x82,
    REGION_CPU3 = 0x83,
    REGION_CPU4 = 0x84,
    REGION_CPU5 = 0x85,
    REGION_CPU6 = 0x86,
    REGION_CPU7 = 0x87,
    REGION_CPU8 = 0x88,
    REGION_GFX1 = 0x89,
    REGION_GFX2 = 0x8a,
    REGION_GFX3 = 0x8b,
    REGION_GFX4 = 0x8c,
    REGION_GFX5 = 0x8d,
    REGION_GFX6 = 0x8e,
    REGION_GFX7 = 0x8f,
    REGION_GFX8 = 0x90,
    REGION_PROMS = 0x91,
    REGION_SOUND1 = 0x92,
    REGION_SOUND2 = 0x93,
    REGION_SOUND3 = 0x94,
    REGION_SOUND4 = 0x95,
    REGION_SOUND5 = 0x96,
    REGION_SOUND6 = 0x97,
    REGION_SOUND7 = 0x98,
    REGION_SOUND8 = 0x99,
    REGION_USER1 = 0x9a,
    REGION_USER2 = 0x9b,
    REGION_USER3 = 0x9c,
    REGION_USER4 = 0x9d,
    REGION_USER5 = 0x9e,
    REGION_USER6 = 0x9f,
    REGION_USER7 = 0xa0,
    REGION_USER8 = 0xa1,
    REGION_MAX = 0xa2;

        public const uint REGIONFLAG_MASK = 0xf8000000;
        public const uint REGIONFLAG_DISPOSE = 0x80000000;
        public const uint REGIONFLAG_SOUNDONLY = 0x40000000;

        public const uint ROMFLAG_MASK = 0xf8000000;
        public const uint ROMFLAG_ALTERNATE = 0x80000000;
        public const uint ROMFLAG_WIDE = 0x40000000;
        public const uint ROMFLAG_SWAP = 0x20000000;
        public const uint ROMFLAG_NIBBLE = 0x10000000;
        public const uint ROMFLAG_QUAD = 0x08000000;

        public uint BADCRC(uint crc) { return ~crc; }
        int readroms()
        {
            int region;
            RomModule[] romp;
            int warning = 0;
            int fatalerror = 0;
            int total_roms, current_rom;
            string buf = "";
            int ridx = 0;

            total_roms = current_rom = 0;
            romp = Machine.gamedrv.rom;

            if (romp == null) return 0;

            while (romp[ridx].name != null || romp[ridx].offset != 0 || romp[ridx].length != 0)
            {
                if (romp[ridx].name != null && romp[ridx].name != "-1")
                    total_roms++;

                ridx++;
            }

            ridx = 0;

            for (region = 0; region < MAX_MEMORY_REGIONS; region++)
                Machine.memory_region[region] = null;

            region = 0;

            while (romp[ridx].name != null || romp[ridx].offset != 0 || romp[ridx].length != 0)
            {
                string name;

                /* Mish:  An 'optional' rom region, only loaded if sound emulation is turned on */
                if (Machine.sample_rate == 0 && (romp[ridx].crc & REGIONFLAG_SOUNDONLY) != 0)
                {
                    System.Console.WriteLine(sprintf("readroms():  Ignoring rom region %d\n", region));
                    Machine.memory_region_type[region] = (int)romp[ridx].crc;
                    region++;

                    ridx++;
                    while (romp[ridx].name != null || romp[ridx].length != 0)
                        ridx++;

                    continue;
                }

                if (romp[ridx].name != null || romp[ridx].length != 0)
                {
                    printf("Error in RomModule definition: expecting ROM_REGION\n");
                    goto getout;
                }

                uint region_size = romp[ridx].offset;
                if ((Machine.memory_region[region] = new _BytePtr((int)region_size)) == null)
                {
                    printf("readroms():  Unable to allocate %d bytes of RAM\n", region_size);
                    goto getout;
                }
                Machine.memory_region_length[region] = region_size;
                Machine.memory_region_type[region] = (int)romp[ridx].crc;

                /* some games (i.e. Pleiades) want the memory clear on startup */
                if (region_size <= 0x400000)	/* don't clear large regions which will be filled anyway */
                    memset(Machine.memory_region[region], 0, (int)region_size);

                ridx++;

                while (romp[ridx].length != 0)
                {
                    object f;
                    uint expchecksum = romp[ridx].crc;
                    int explength = 0;

                    if (romp[ridx].name == null)
                    {
                        printf("Error in RomModule definition: ROM_CONTINUE not preceded by ROM_LOAD\n");
                        goto getout;
                    }
                    else if (romp[ridx].name == "-1")
                    {
                        printf("Error in RomModule definition: ROM_RELOAD not preceded by ROM_LOAD\n");
                        goto getout;
                    }

                    name = romp[ridx].name;

                    /* update status display */
                    if (osd_display_loading_rom_message(name, ++current_rom, total_roms) != 0)
                        goto getout;

                    {
                        GameDriver drv;

                        drv = Machine.gamedrv;
                        do
                        {
                            f = osd_fopen(drv.name, name, OSD_FILETYPE_ROM, 0);
                            drv = drv.clone_of;
                        } while (f == null && drv != null);

                        if (f == null)
                        {
                            /* NS981003: support for "load by CRC" */
                            string crc = sprintf("%08x", romp[ridx].crc);
                            drv = Machine.gamedrv;
                            do
                            {
                                f = osd_fopen(drv.name, crc, OSD_FILETYPE_ROM, 0);
                                drv = drv.clone_of;
                            } while (f == null && drv != null);
                        }
                    }

                    if (f != null)
                    {
                        do
                        {
                            _BytePtr c;
                            uint i;
                            int length = (int)(romp[ridx].length & ~ROMFLAG_MASK);

                            if (romp[ridx].name == "-1")
                                osd_fseek(f, 0, SEEK_SET);	/* ROM_RELOAD */
                            else
                                explength += length;

                            if (romp[ridx].offset + length > region_size ||
                                ((romp[ridx].length & ROMFLAG_NIBBLE) == 0 && (romp[ridx].length & ROMFLAG_ALTERNATE) != 0
                                        && (romp[ridx].offset & ~1) + 2 * length > region_size))
                            {
                                printf("Error in RomModule definition: %s out of memory region space\n", name);
                                osd_fclose(f);
                                goto getout;
                            }

                            if ((romp[ridx].length & ROMFLAG_NIBBLE) != 0)
                            {
                                byte[] temp = new byte[length];

                                if (temp == null)
                                {
                                    printf("Out of memory reading ROM %s\n", name);
                                    osd_fclose(f);
                                    goto getout;
                                }

                                if (osd_fread(f, temp, length) != length)
                                {
                                    printf("Unable to read ROM %s\n", name);
                                }

                                /* ROM_LOAD_NIB_LOW and ROM_LOAD_NIB_HIGH */
                                c = new _BytePtr(Machine.memory_region[region], (int)romp[ridx].offset);
                                if ((romp[ridx].length & ROMFLAG_ALTERNATE) != 0)
                                {
                                    /* Load into the high nibble */
                                    for (i = 0; i < length; i++)
                                    {
                                        c[i] = (byte)((c[i] & 0x0f) | ((temp[i] & 0x0f) << 4));
                                    }
                                }
                                else
                                {
                                    /* Load into the low nibble */
                                    for (i = 0; i < length; i++)
                                    {
                                        c[i] = (byte)((c[i] & 0xf0) | (temp[i] & 0x0f));
                                    }
                                }

                                temp = null;
                            }
                            else if ((romp[ridx].length & ROMFLAG_ALTERNATE) != 0)
                            {
                                /* ROM_LOAD_EVEN and ROM_LOAD_ODD */
                                /* copy the ROM data */
#if WINDOWS
                                c = new _BytePtr(Machine.memory_region[region], (int)(romp[ridx].offset ^ 1));
#else
						c = Machine.memory_region[region] + romp[ridx].offset;
#endif

                                if (osd_fread_scatter(f, c, length, 2) != length)
                                {
                                    printf("Unable to read ROM %s\n", name);
                                }
                            }
                            else if ((romp[ridx].length & ROMFLAG_QUAD) != 0)
                            {
                                int which_quad = 0; /* This is multi session friendly, as we only care about the modulus */
                                byte[] temp;
                                int _base = 0;

                                temp = new byte[length];	/* Need to load rom to temporary space */
                                osd_fread(f, temp, length);

                                /* Copy quad to region */
                                c = new _BytePtr(Machine.memory_region[region], (int)romp[ridx].offset);

#if WINDOWS
                                switch (which_quad % 4)
                                {
                                    case 0: _base = 1; break;
                                    case 1: _base = 0; break;
                                    case 2: _base = 3; break;
                                    case 3: _base = 2; break;
                                }
#else
						switch (which_quad%4) {
							case 0: _base=0; break;
							case 1: _base=1; break;
							case 2: _base=2; break;
							case 3: _base=3; break;
						}
#endif

                                for (i = (uint)_base; i < length * 4; i += 4)
                                    c[i] = temp[i / 4];

                                which_quad++;
                                temp = null;
                            }
                            else
                            {
                                int wide = (int)(romp[ridx].length & ROMFLAG_WIDE);
#if WINDOWS
                                int swap = (int)((romp[ridx].length & ROMFLAG_SWAP) ^ ROMFLAG_SWAP);
#else
						int swap = romp[ridx].length & ROMFLAG_SWAP;
#endif

                                osd_fread(f, new _BytePtr(Machine.memory_region[region], (int)romp[ridx].offset), length);

                                /* apply swappage */
                                c = new _BytePtr(Machine.memory_region[region], (int)romp[ridx].offset);
                                if (wide != 0 && swap != 0)
                                {
                                    for (i = 0; i < length; i += 2)
                                    {
                                        byte temp = c[i];
                                        c[i] = c[i + 1];
                                        c[i + 1] = temp;
                                    }
                                }
                            }

                            ridx++;
                        } while (romp[ridx].length != 0 && (romp[ridx].name == null || romp[ridx].name == "-1"));

                        if (explength != osd_fsize(f))
                        {
                            buf += sprintf("%-12s WRONG LENGTH (expected: %08x found: %08x)\n", name, explength, osd_fsize(f));
                            warning = 1;
                        }

                        if (expchecksum != osd_fcrc(f))
                        {
                            warning = 1;
                            if (expchecksum == 0)
                                buf += sprintf("%-12s NO GOOD DUMP KNOWN\n", name);
                            else if (expchecksum == BADCRC(osd_fcrc(f)))
                                buf += sprintf("%-12s ROM NEEDS REDUMP\n", name);
                            else
                                buf += sprintf("%-12s WRONG CRC (expected: %08x found: %08x)\n", name, expchecksum, osd_fcrc(f));
                        }

                        osd_fclose(f);
                    }
                    else
                    {
                        /* allow for a NO GOOD DUMP KNOWN rom to be missing */
                        if (expchecksum == 0)
                        {
                            buf += sprintf("%-12s NOT FOUND (NO GOOD DUMP KNOWN)\n", name);
                            warning = 1;
                        }
                        else
                        {
                            buf += sprintf("%-12s NOT FOUND\n", name);
                            fatalerror = 1;
                        }

                        do
                        {
                            if (fatalerror == 0)
                            {
                                int i;

                                /* fill space with random data */
                                if ((romp[ridx].length & ROMFLAG_ALTERNATE) != 0)
                                {
                                    _BytePtr c;

                                    /* ROM_LOAD_EVEN and ROM_LOAD_ODD */
#if WINDOWS
                                    c = new _BytePtr(Machine.memory_region[region], (int)(romp[ridx].offset ^ 1));
#else
							c = Machine.memory_region[region] + romp[ridx].offset;
#endif

                                    for (i = 0; i < (romp[ridx].length & ~ROMFLAG_MASK); i++)
                                        c[2 * i] = (byte)rand();
                                }
                                else
                                {
                                    for (i = 0; i < (romp[ridx].length & ~ROMFLAG_MASK); i++)
                                        Machine.memory_region[region][(uint)(romp[ridx].offset + i)] = (byte)rand();
                                }
                            }
                            ridx++;
                        } while (romp[ridx].length != 0 && (romp[ridx].name == null || romp[ridx].name == "-1"));
                    }
                }

                region++;
            }

            /* final status display */
            osd_display_loading_rom_message(null, current_rom, total_roms);

            if (warning != 0 || fatalerror != 0)
            {
                if (fatalerror != 0)
                {
                    buf += "ERROR: required files are missing, the game cannot be run.\n";
                    bailing = 1;
                }
                else
                    buf += "WARNING: the game might not run correctly.\n";
                printf("%s", buf);

                //if (options.gui_host == 0 && bailing == 0)
                //{
                //    printf("Press any key to continue\n");
                //    keyboard_read_sync();
                //    if (keyboard_pressed((int)InputCodes.KEYCODE_LCONTROL) && keyboard_pressed((int)InputCodes.KEYCODE_C))
                //        return 1;
                //}
            }

            if (fatalerror != 0) return 1;
            else return 0;


        getout:
            /* final status display */
            osd_display_loading_rom_message(null, current_rom, total_roms);

            for (region = 0; region < MAX_MEMORY_REGIONS; region++)
            {
                Machine.memory_region[region] = null;
            }

            return 1;
        }
        public static void coin_counter_w(int offset, int data)
        {
            if (offset >= COIN_COUNTERS) return;
            /* Count it only if the data has changed from 0 to non-zero */
            if (data != 0 && (lastcoin[offset] == 0))
            {
                coins[offset]++;
            }
            lastcoin[offset] = (uint)data;
        }

        public static void coin_lockout_w(int offset, int data)
        {
            if (offset >= COIN_COUNTERS) return;

            coinlockedout[offset] = (uint)data;
        }

        /* Locks out all the coin inputs */
        public static void coin_lockout_global_w(int offset, int data)
        {
            for (int i = 0; i < COIN_COUNTERS; i++)
            {
                coin_lockout_w(i, data);
            }
        }
        public static _BytePtr memory_region(int num)
        {
            if (num < MAX_MEMORY_REGIONS)
                return Machine.memory_region[num];
            else
            {
                for (int i = 0; i < MAX_MEMORY_REGIONS; i++)
                {
                    if ((Machine.memory_region_type[i] & ~REGIONFLAG_MASK) == num)
                        return Machine.memory_region[i];
                }
            }

            return null;
        }


        public static int memory_region_length(int num)
        {
            if (num < MAX_MEMORY_REGIONS)
                return (int)Machine.memory_region_length[num];
            else
            {
                for (int i = 0; i < MAX_MEMORY_REGIONS; i++)
                {
                    if ((Machine.memory_region_type[i] & ~REGIONFLAG_MASK) == num)
                        return (int)Machine.memory_region_length[i];
                }
            }

            return 0;
        }
        void freesamples(ref GameSamples samples)
        {
            if (samples == null) return;
            for (int i = 0; i < samples.total; i++)
                samples.sample[i] = null;
            samples = null;
        }
        public static GameSamples readsamples(string[] samplenames, string basename)
        {
            int i;
            GameSamples samples;
            int skipfirst = 0;

            /* if the user doesn't want to use samples, bail */
            if (!options.use_samples) return null;

            if (samplenames == null || samplenames[0] == null) return null;

            if (samplenames[0][0] == '*')
                skipfirst = 1;

            i = 0;
            while (i < samplenames.Length && samplenames[i + skipfirst] != null) i++;

            if (i == 0) return null;

            samples = new GameSamples(i);

            samples.total = i;
            for (i = 0; i < samples.total; i++)
                samples.sample[i] = null;

            for (i = 0; i < samples.total; i++)
            {
                object f;

                if (samplenames[i + skipfirst][0] != '\0')
                {
                    if ((f = osd_fopen(basename, samplenames[i + skipfirst], OSD_FILETYPE_SAMPLE, 0)) == null)
                        if (skipfirst != 0)
                            f = osd_fopen(samplenames[0] + 1, samplenames[i + skipfirst], OSD_FILETYPE_SAMPLE, 0);
                    if (f != null)
                    {
                        samples.sample[i] = read_wav_sample(f);
                        osd_fclose(f);
                    }
                }
            }

            return samples;
        }
        uint intelLong(uint x)
        {
#if !XBOX
            return x;
#else
#error blkbkb
#endif
        }
        static GameSample read_wav_sample(object f)
        {
            uint offset = 0;
            uint length=0, rate=0, filesize, temp32;
            ushort bits=0, temp16=0;
            byte[] buf = new byte[32];
            GameSample result;

            /* read the core header and make sure it's a WAVE file */
            offset += (uint)osd_fread(f, buf, 4);
            if (offset < 4)
                return null;
            if (!Encoding.Default.GetString(buf).StartsWith("RIFF"))
                return null;
            /* get the total size */
            offset += (uint)osd_fread(f, buf, 4);
            if (offset < 8)
                return null;
            filesize = (uint)BitConverter.ToInt32(buf, 0);

            /* read the RIFF file type and make sure it's a WAVE file */
            offset += (uint)osd_fread(f, buf, 4);
            if (offset < 12)
                return null;
            if (!Encoding.Default.GetString(buf).StartsWith("WAVE"))
                return null;

            /* seek until we find a format tag */
            while (true)
            {
                offset += (uint)osd_fread(f, buf, 4);
                offset += (uint)osd_fread(f, ref length, 4);
                //length = intelLong(length);
                if (Encoding.Default.GetString(buf).StartsWith("fmt "))
                    break;

                /* seek to the next block */
                osd_fseek(f, (int)length, SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                    return null;
            }

            /* read the format -- make sure it is PCM */
            offset +=(uint) osd_fread_lsbfirst(f, ref temp16, 2);
            if (temp16 != 1)
                return null;

            /* number of channels -- only mono is supported */
            offset += (uint)osd_fread_lsbfirst(f, ref temp16, 2);
            if (temp16 != 1)
                return null;

            /* sample rate */
            offset += (uint)osd_fread(f, ref rate, 4);
            //rate = intelLong(rate);

            /* bytes/second and block alignment are ignored */
            offset += (uint)osd_fread(f, buf, 6);

            /* bits/sample */
            offset += (uint)osd_fread_lsbfirst(f, ref bits, 2);
            if (bits != 8 && bits != 16)
                return null;

            /* seek past any extra data */
            osd_fseek(f,(int) length - 16, SEEK_CUR);
            offset += length - 16;

            /* seek until we find a data tag */
            while (true)
            {
                offset += (uint)osd_fread(f, buf, 4);
                offset += (uint)osd_fread(f, ref length, 4);
                //length = intelLong(length);
                if (Encoding.Default.GetString(buf).StartsWith("data"))
                    break;

                /* seek to the next block */
                osd_fseek(f, (int)length, SEEK_CUR);
                offset += length;
                if (offset >= filesize)
                    return null;
            }

            /* allocate the game sample */
            result = new GameSample((int)length);

            /* fill in the sample data */
            result.length = (int)length;
            result.smpfreq = (int)rate;
            result.resolution = bits;

            /* read the data in */
            if (bits == 8)
            {
                osd_fread(f, result.data, (int)length);

                /* convert 8-bit data to signed samples */
                for (temp32 = 0; temp32 < length; temp32++)
                {
                    sbyte b = (sbyte)result.data[temp32];
                    b ^= unchecked((sbyte)0x80);
                    result.data[temp32] = (byte)b;
                }
                    
            }
            else
            {
                /* 16-bit data is fine as-is */
                osd_fread_lsbfirst(f, result.data, (int)length);
            }

            return result;
        }
    }
}
