using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class osd_bitmap
        {
            public int width,height,depth;
            public object _private;
            public _BytePtr[] line;
        }
        public const int OSD_FILETYPE_ROM = 1;
        public const int OSD_FILETYPE_SAMPLE = 2;
        public const int OSD_FILETYPE_NVRAM = 3;
        public const int OSD_FILETYPE_HIGHSCORE = 4;
        public const int OSD_FILETYPE_CONFIG = 5;
        public const int OSD_FILETYPE_INPUTLOG = 6;
        public const int OSD_FILETYPE_STATE = 7;
        public const int OSD_FILETYPE_ARTWORK = 8;
        public const int OSD_FILETYPE_MEMCARD = 9;
        public const int OSD_FILETYPE_SCREENSHOT = 10;
        public const uint OSD_KEY_NONE = 0xffffffff;

        const byte PATH_NOT_FOUND = 0, PATH_IS_FILE = 1, PATH_IS_DIRECTORY = 2;

        const byte X_AXIS = 1, Y_AXIS = 2;

        public static osd_bitmap osd_create_bitmap(int w, int h) { return osd_new_bitmap(w, h, Machine.scrbitmap.depth); }
    }
}
