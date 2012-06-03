using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int kPlainFile = 0, kRAMFile = 1, kZippedFile = 2;
        const int SEEK_SET = 0;
        const int SEEK_CUR = 1;
        const int SEEK_END = 2;
        class FakeFileHandle
        {
            public Stream file;
            public byte[] data = new byte[1];
            public uint offset;
            public uint length;
            public int type;
            public uint crc;
        }
        class osd_file
        {
            public Stream handle;
            public ulong filepos, end, offset, bufferbase;
            public uint bufferbytes;
            public byte[] buffer = new byte[256];
        }
        static int samplepathc = 0;
        static int rompathc = 0;
        string roms, samples;

        static string cfgdir = "cfg", nvdir = "nvram", hidir = "hi", inpdir = "", stadir = "sta", memcarddir = "memcard", artworkdir = "artwork", screenshotdir = "snap";
        static List<string> samplepathv = new List<string>(), rompathv = new List<string>();

        static int checksum_zipentry(Stream f, int length,ref byte[] p, ref uint size, ref uint crc)
        {
            byte[] data;

            /* allocate space for entire file */
            data = new byte[length];

            /* read entire file into memory */
            f.Read(data, 0, length);

            size = (uint)length;
            //crc = crc32(0L, data, length);
            Crc32 crc32 = new Crc32();
            string hash = "";
            foreach (byte b in crc32.ComputeHash(data)) hash += b.ToString("x2").ToLower();
            crc = Convert.ToUInt32(hash, 16);
            if (p != null)
                p = data;
            else
                data = null;

            fclose(f);

            return 0;
        }
        static int checksum_file(string file, ref byte[] p, ref uint size, ref uint crc)
        {
            int length;
            byte[] data;
            Stream f;

            f = fopen(file, "rb");
            if (f == null)
                return -1;

            length = (int)f.Length;

            /* allocate space for entire file */
            data = new byte[length];

            /* read entire file into memory */
            f.Read(data, 0, length);

            size = (uint)length;
            //crc = crc32(0L, data, length);
            Crc32 crc32 = new Crc32();
            string hash = "";
            foreach (byte b in crc32.ComputeHash(data)) hash += b.ToString("x2").ToLower();
            crc = Convert.ToUInt32(hash,16);
            if (p != null)
                p = data;
            else
                data = null;

            fclose(f);

            return 0;
        }
        int osd_display_loading_rom_message(string name, int current, int total)
        {
            if (name != null)
                printf("loading %-12s\r", name);
            else
                printf("                    \r");

            if (keyboard_pressed((int)InputCodes.KEYCODE_LCONTROL) && keyboard_pressed((int)InputCodes.KEYCODE_C))
                return 1;

            return 0;
        }
        static void osd_fclose(object file)
        {
            FakeFileHandle f = (FakeFileHandle)file;

            switch (f.type)
            {
                case kPlainFile:
                    f.file.Close();
                    break;
                case kZippedFile:
                case kRAMFile:
                    if (f.data != null)
                        f.data = null;
                    break;
            }
            f = null;
        }
        uint osd_fcrc(object file)
        {
            FakeFileHandle f = (FakeFileHandle)file;

            return f.crc;
        }
        object osd_fopen(int pathtype, int pathindex, string filename, string mode)
        {
            return fopen(filename, mode);
        }
        static object osd_fopen(string game, string filename, int filetype, int _write)
        {
            string name, gamename;
            bool found = false;
            int indx;

            FakeFileHandle f;
            int pathc;
            string[] pathv;


            f = new FakeFileHandle();

            gamename = game;

            ///* Support "-romdir" yuck. */
            //if( alternate_name )
            //{
            //    LOG((errorlog, "osd_fopen: -romdir overrides '%s' by '%s'\n", gamename, alternate_name));
            //    gamename = alternate_name;
            //}

            switch (filetype)
            {
                case OSD_FILETYPE_ROM:
                case OSD_FILETYPE_SAMPLE:

                    /* only for reading */
                    if (_write != 0)
                    {
                        System.Console.WriteLine("osd_fopen: OSD_FILETYPE_ROM/SAMPLE/ROM_CART write not supported\n");
                        break;
                    }

                    if (filetype == OSD_FILETYPE_SAMPLE)
                    {
                        System.Console.WriteLine("osd_fopen: using samplepath\n");
                        pathc = samplepathc;
                        pathv = samplepathv.ToArray();
                    }
                    else
                    {
                        System.Console.WriteLine("osd_fopen: using rompath\n");
                        pathc = rompathc;
                        pathv = rompathv.ToArray();
                    }

                    for (indx = 0; indx < pathc && !found; ++indx)
                    {
                        string dir_name = "c:\\xnamame\\"+pathv[indx];

                        if (!found)
                        {
                            name = sprintf("%s/%s", dir_name, gamename);
                            //printf("Trying %s\n", name);
                            if (Directory.Exists(name))
                            {
                                printf("loading from %s\n", name);
                                name = sprintf("%s/%s/%s", dir_name, gamename, filename);
                                if (filetype == OSD_FILETYPE_ROM)
                                {
                                    if (checksum_file(name, ref f.data, ref f.length, ref f.crc) == 0)
                                    {
                                        f.type = kRAMFile;
                                        f.offset = 0;
                                        found = true;
                                    }
                                }
                                else
                                {
                                    f.type = kPlainFile;
                                    f.file = fopen(name, "rb");
                                    found = f.file != null;
                                }
                            }
                        }

                        if (!found)
                        {
                            /* try with a .zip extension */
                            name = sprintf("%s/%s.zip", dir_name, gamename);
                            //printf("Trying %s file\n", name);
                            if (File.Exists(name))
                            {
                                printf("loading from %s\n",name);
                                ZipFile zipFile = new ZipFile(name);
                                Dictionary<string, ZipEntry> entries;
                                entries = zipFile.getEntries();
                                if (entries.ContainsKey(filename))
                                {
                                    if (checksum_zipentry(zipFile.getInputStream(entries[filename]),(int)entries[filename].getSize(), ref f.data, ref f.length, ref f.crc) == 0)
                                    {
                                        f.type = kRAMFile;
                                        f.offset = 0;
                                        found = true;
                                    }
                                }
                                //else
                                //throw new Exception(sprintf("zip file does not contains entry for %s ",filename));
                           
                            }
                        }

                        //if (!found)
                        //{
                        //    /* try with a .zip directory (if ZipMagic is installed) */
                        //    name = sprintf("%s/%s.zip", dir_name, gamename);
                        //    printf("Trying %s directory\n", name);
                        //    if (File.Exists(name))
                        //    {
                        //        throw new Exception("zip file not supported yet");
                        //    //if (cache_stat(name, &stat_buffer) == 0 && (stat_buffer.st_mode & S_IFDIR))
                        //    //{
                        //    //    name = sprintf("%s/%s.zip/%s", dir_name, gamename, filename);
                        //    //    if (filetype == OSD_FILETYPE_ROM)
                        //    //    {
                        //    //        if (checksum_file(name, ref f.data, ref f.length, ref f.crc) == 0)
                        //    //        {
                        //    //            f.type = kRAMFile;
                        //    //            f.offset = 0;
                        //    //            found = true;
                        //    //        }
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        f.type = kPlainFile;
                        //    //        f.file = fopen(name, "rb");
                        //    //        found = f.file != null;
                        //    //    }
                        //    }
                        //}
                    }

                    break;

                case OSD_FILETYPE_NVRAM:
                    if (!found)
                    {
                        name = sprintf("%s/%s.nv", nvdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (!found)
                    {
                        /* try with a .zip directory (if ZipMagic is installed) */
                        name = sprintf("%s.zip/%s.nv", nvdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (!found)
                    {
                        /* try with a .zif directory (if ZipFolders is installed) */
                        name = sprintf("%s.zif/%s.nv", nvdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }
                    break;

                case OSD_FILETYPE_HIGHSCORE:
                    if (mame_highscore_enabled() )
                    {
                        if (!found)
                        {
                            name = sprintf("%s/%s.hi", hidir, gamename);
                            f.type = kPlainFile;
                            f.file = fopen(name, _write != 0 ? "wb" : "rb");
                            found = f.file != null;
                        }

                        if (!found)
                        {
                            /* try with a .zip directory (if ZipMagic is installed) */
                            name = sprintf("%s.zip/%s.hi", hidir, gamename);
                            f.type = kPlainFile;
                            f.file = fopen(name, _write != 0 ? "wb" : "rb");
                            found = f.file != null;
                        }

                        if (!found)
                        {
                            /* try with a .zif directory (if ZipFolders is installed) */
                            name = sprintf("%s.zif/%s.hi", hidir, gamename);
                            f.type = kPlainFile;
                            f.file = fopen(name, _write != 0 ? "wb" : "rb");
                            found = f.file != null;
                        }
                    }
                    break;

                case OSD_FILETYPE_CONFIG:
                    name = sprintf("%s/%s.cfg", cfgdir, gamename);
                    f.type = kPlainFile;
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = f.file != null;

                    if (!found)
                    {
                        /* try with a .zip directory (if ZipMagic is installed) */
                        name = sprintf("%s.zip/%s.cfg", cfgdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (!found)
                    {
                        /* try with a .zif directory (if ZipFolders is installed) */
                        name = sprintf("%s.zif/%s.cfg", cfgdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }
                    break;

                case OSD_FILETYPE_INPUTLOG:
                    name = sprintf("%s/%s.inp", inpdir, gamename);
                    f.type = kPlainFile;
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = f.file != null;

                    if (!found)
                    {
                        /* try with a .zip directory (if ZipMagic is installed) */
                        name = sprintf("%s.zip/%s.cfg", inpdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (!found)
                    {
                        /* try with a .zif directory (if ZipFolders is installed) */
                        name = sprintf("%s.zif/%s.cfg", inpdir, gamename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (_write == 0)
                    {
                        string file = sprintf("%s.inp", gamename);
                        name = sprintf("%s/%s.zip", inpdir, gamename);
                        printf("Trying %s in %s\n", file, name);
                        throw new Exception("zipfile not supported yet");
                        //if (cache_stat(name, &stat_buffer) == 0)
                        //{
                        //    if (load_zipped_file(name, file, &f.data, &f.length) == 0)
                        //    {
                        //        printf("Using (osd_fopen) zip file %s for %s\n", name, file);
                        //        f.type = kZippedFile;
                        //        f.offset = 0;
                        //        found = true;
                        //    }
                        //}
                    }

                    break;

                case OSD_FILETYPE_STATE:
                    name = sprintf("%s/%s.sta", stadir, gamename);
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = !(f.file == null);
                    if (!found)
                    {
                        /* try with a .zip directory (if ZipMagic is installed) */
                        name = sprintf("%s.zip/%s.sta", stadir, gamename);
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = !(f.file == null);
                    }
                    if (!found)
                    {
                        /* try with a .zif directory (if ZipFolders is installed) */
                        name = sprintf("%s.zif/%s.sta", stadir, gamename);
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = !(f.file == null);
                    }
                    break;

                case OSD_FILETYPE_ARTWORK:
                    /* only for reading */
                    if (_write != 0)
                    {
                        printf("osd_fopen: OSD_FILETYPE_ARTWORK write not supported\n");
                        break;
                    }
                    name = sprintf("%s/%s", artworkdir, filename);
                    f.type = kPlainFile;
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = f.file != null;
                    if (!found)
                    {
                        /* try with a .zip directory (if ZipMagic is installed) */
                        name = sprintf("%s.zip/%s.png", artworkdir, filename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }

                    if (!found)
                    {
                        /* try with a .zif directory (if ZipFolders is installed) */
                        name = sprintf("%s.zif/%s.png", artworkdir, filename);
                        f.type = kPlainFile;
                        f.file = fopen(name, _write != 0 ? "wb" : "rb");
                        found = f.file != null;
                    }
                    //throw new Exception("zif files not supported");
                    if (!found)
                    {
                        string file, extension;
                        file = sprintf("%s", filename);
                        name = sprintf("%s/%s", artworkdir, filename);
                        extension = Path.GetExtension(name); ;
                        if (extension != null)
                            extension = ".zip";
                        else
                            name += ".zip";
                        //printf("Trying %s in %s\n", file, name);
                        if (File.Exists(name))
                        {
                            printf("loading from %s\n", name);
                            ZipFile zipFile = new ZipFile(name);
                            Dictionary<string, ZipEntry> entries;
                            entries = zipFile.getEntries();
                            if (entries.ContainsKey(filename))
                            {
                                zipFile.getInputStream(entries[filename]).Read(f.data, 0, (int)f.length);
                                f.type = kZippedFile;
                                f.offset = 0;
                                found = true;
                            }
                        }


                        if (!found)
                        {
                            //  throw new Exception();
                            name = sprintf("%s/%s.zip", artworkdir, game);
                            printf("Trying %s in %s\n", file, name);
                            if (File.Exists(name))
                            {
                                ZipFile zipFile = new ZipFile(name);
                                Dictionary<string, ZipEntry> entries;
                                entries = zipFile.getEntries();
                                if (entries.ContainsKey(filename))
                                {
                                    zipFile.getInputStream(entries[filename]).Read(f.data, 0, (int)f.length);
                                    f.type = kZippedFile;
                                    f.offset = 0;
                                    found = true;
                                }
                            }
                        
                        }
                    }
                    break;

                case OSD_FILETYPE_MEMCARD:
                    name = sprintf("%s/%s", memcarddir, filename);
                    f.type = kPlainFile;
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = f.file != null;
                    break;

                case OSD_FILETYPE_SCREENSHOT:
                    /* only for writing */
                    if (_write == 0)
                    {
                        printf("osd_fopen: OSD_FILETYPE_SCREENSHOT read not supported\n");
                        break;
                    }

                    name = sprintf("%s/%s.png", screenshotdir, filename);
                    f.type = kPlainFile;
                    f.file = fopen(name, _write != 0 ? "wb" : "rb");
                    found = f.file != null;
                    break;
            }

            if (!found)
            {
                f = null;
                return null;
            }

            return f;
        }
        public static int osd_fread(object file, byte[] buffer, int offset, int length)
        {
            FakeFileHandle f = (FakeFileHandle)file;

            switch (f.type)
            {
                case kPlainFile:
                    return f.file.Read(buffer,offset, length);//fread(buffer, 1, length, f.file);
                    
                case kZippedFile:
                case kRAMFile:
                    /* reading from the RAM image of a file */
                    if (f.data!=null)
                    {
                        if (length + f.offset > f.length)
                            length = (int)(f.length - f.offset);
                        memcpy(buffer, offset,f.data,(int)f.offset , length);
                        f.offset += (uint)length;
                        return length;
                    }
                    break;
            }

            return 0;
        }
        public static int osd_fread(object file, ref uint v, int length)
        {
            byte[] b = new byte[length];
            var r = osd_fread(file, b, length);
            v = BitConverter.ToUInt32(b, 0);
            return r;
        }
        public static int osd_fread(object file, byte[] buffer, int length)
        {
            return osd_fread(file, buffer, 0, length);
        }
        public static int osd_fread(object file, sbyte[] buffer, int length)
        {
            byte[] b = new byte[length];
            var r = osd_fread(file, b, 0, length);
            Buffer.BlockCopy(b, 0, buffer, 0, length);
            return r;
        }
        public static int osd_fread(object file, _BytePtr buffer, int length)
        {
            return osd_fread(file, buffer.buffer, (int)buffer.offset, length);
        }
        int osd_fread_scatter(object file, byte[] buffer, int length, int increment)
        {
            throw new Exception();
        }
        int osd_fread_scatter(object file, _BytePtr buffer, int length, int increment)
        {
            int buf = 0;
	FakeFileHandle f = (FakeFileHandle ) file;
	byte[] tempbuf=new byte[4096];
	int totread, r, i;

	switch( f.type )
	{
	case kPlainFile:
		totread = 0;
		while (length!=0)
		{
			r = length;
			if( r > 4096 )
				r = 4096;
            r = f.file.Read(tempbuf, 0, r);
			//r = fread (tempbuf, 1, r, f.file);
			if( r == 0 )
				return totread;		   /* error */
			for( i = 0; i < r; i++ )
			{
				buffer[buf] = tempbuf[i];
				buf += increment;
			}
			totread += r;
			length -= r;
		}
		return totread;
		break;
	case kZippedFile:
	case kRAMFile:
		/* reading from the RAM image of a file */
		if( f.data !=null)
		{
			if( length + f.offset > f.length )
				length = (int)(f.length - f.offset);
			for( i = 0; i < length; i++ )
			{
				buffer[buf] = f.data[f.offset + i];
				buf += increment;
			}
			f.offset +=(uint) length;
			return length;
		}
		break;
	}

	return 0;
        }
        static int osd_fread_lsbfirst(object file, ref ushort v, int length)
        {
            byte[] b = new byte[length];
            var r = osd_fread(file, b, length);
            v = BitConverter.ToUInt16(b, 0);
            return r;
        }
        static int osd_fread_lsbfirst(object file, byte[] buffer, int length)
        {
            return osd_fread(file, buffer, 0, length);
        }
        static int osd_fread_lsbfirst(object file, sbyte[] buffer, int length)
        {
            byte[] b = new byte[length];
            var r = osd_fread(file, b, 0, length);
            Buffer.BlockCopy(b, 0, buffer, 0, length);
            return r;
        }
        static int osd_fseek(object file, int offset, int whence)
        {
            FakeFileHandle f = (FakeFileHandle)file;
            int err = 0;

            switch (f.type)
            {
                case kPlainFile:
                    //return fseek(f.file, offset, whence);
                    switch (whence)
                    {
                        case SEEK_SET: ((FakeFileHandle)file).file.Seek(offset, SeekOrigin.Begin); break;
                        case SEEK_CUR: ((FakeFileHandle)file).file.Seek(offset, SeekOrigin.Current); break;
                        default: throw new Exception();
                    }
                    break;
                case kZippedFile:
                case kRAMFile:
                    /* seeking within the RAM image of a file */
                    switch (whence)
                    {
                        case SEEK_SET:
                            f.offset = (uint)offset;
                            break;
                        case SEEK_CUR:
                            f.offset += (uint)offset;
                            break;
                        case SEEK_END:
                            f.offset = (uint)(f.length + offset);
                            break;
                    }
                    break;
            }

            return err;

            return (int)((FakeFileHandle)file).file.Position;
        }
        int osd_fsize(object file)
        {
            FakeFileHandle f = (FakeFileHandle)file;

            if (f.type == kRAMFile || f.type == kZippedFile)
                return (int)f.length;

            if (f.file!=null)
            {
                return (int)f.file.Length;
            }

            return 0;
        }
        void decompose_rom_sample_path(string rompath, string samplepath)
        {
            string token = "";

            /* start with zero path components */
            rompathc = samplepathc = 0;

            roms = rompath;
            samples = samplepath;

            if (roms.Contains(";"))
                token = roms.Substring(0, roms.IndexOf(";"));
            while (token != "")
            {
                rompathv.Add(token);
                rompathc++;
                roms = roms.Substring(roms.IndexOf(";") + 1);
                if (roms.Contains(";"))
                    token = roms.Substring(0, roms.IndexOf(";"));
                else
                {
                    rompathv.Add(roms);
                    rompathc++;
                    token = "";
                }
            }

            if (samples.Contains(";"))
                token = samples.Substring(0, samples.IndexOf(";"));
            while (token != "")
            {
                samplepathv.Add(token);
                samplepathc++;
                samples = samples.Substring(samples.IndexOf(";") + 1);
                if (samples.Contains(";"))
                    token = samples.Substring(0, samples.IndexOf(";"));
                else
                {
                    samplepathv.Add(samples);
                    samplepathc++;
                    token = "";
                }
            }
        }
        int osd_get_path_info(int pathtype, int pathindex, string filename)
        {
            throw new Exception();
        }
        public static int osd_fwrite(object file, _BytePtr buffer, int length)
        {
            throw new Exception();
        }
        public static int osd_fwrite(object file, byte[] buffer, int length)
        {
            FakeFileHandle f = (FakeFileHandle)file;
            switch (f.type)
            {
                case kPlainFile:
                    {
                        f.file.Write(buffer, 0, length);
                        return length;
                    }
            }
            return 0;

        }
    }
}
