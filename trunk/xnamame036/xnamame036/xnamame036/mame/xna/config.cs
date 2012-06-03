using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace xnamame036.mame
{
    partial class Mame
    {
        const byte JOY_TYPE_NONE = 0;
        const byte JOY_TYPE_AUTODETECT = 1;


        DataSet config_ds = new DataSet();
        string rompath, samplepath;

        int get_bool(string section, string option, string shortcut, int def)
        {
            if (config_ds.Tables[section].Columns.Contains(option))
                return Convert.ToInt32(config_ds.Tables[section].Rows[0][option]);
            else
                return def;
        }
        string get_string(string section, string option, string shortcut, string def)
        {
            if (config_ds.Tables[section].Columns.Contains(option))
                return Convert.ToString(config_ds.Tables[section].Rows[0][option]);
            else
                return def;
        }
        void get_rom_sample_path()
        {
            rompath = get_string("directory", "rompath", null, ".;ROMS");
            samplepath = get_string("directory", "samplepath", null, ".;SAMPLES");

            decompose_rom_sample_path(rompath, samplepath);
        }
        void config()
        {
            config_ds.ReadXml("config.xml");

            /* read graphic configuration */
            scanlines = get_bool("config", "scanlines", null, 1);
            stretch = get_bool("config", "stretch", null, 1);
            options.use_artwork = get_bool("config", "artwork", null, 1)!=0;
            options.use_samples = get_bool("config", "samples", null, 1)!=0;
            video_sync = get_bool("config", "vsync", null, 0);
            wait_vsync = get_bool("config", "waitvsync", null, 0);
            use_triplebuf = get_bool("config", "triplebuffer", null, 0);
            use_tweaked = get_bool("config", "tweak", null, 0);
            //vesamode = get_string("config", "vesamode", null, "vesa3");
            use_mmx = get_bool("config", "mmx", null, -1);
            use_dirty = get_bool("config", "dirty", null, -1);
            options.antialias = get_bool("config", "antialias", null, 0);
            options.translucency = get_bool("config", "translucency", null, 1);

            options.beam = get_bool("config", "beam", null, 1);
            options.use_artwork = true;

            /* set default subdirectories */
            nvdir = get_string("directory", "nvram", null, "NVRAM");
            hidir = get_string("directory", "hi", null, "HI");
            cfgdir = get_string("directory", "cfg", null, "CFG");
            screenshotdir = get_string("directory", "snap", null, "SNAP");
            memcarddir = get_string("directory", "memcard", null, "MEMCARD");
            stadir = get_string("directory", "sta", null, "STA");
            artworkdir = get_string("directory", "artwork", null, "ARTWORK");

            frameskip = 0;
            autoframeskip = false;

            get_rom_sample_path();
        }
    }
}
