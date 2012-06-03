using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int FILETYPE_RAW = 0,
     FILETYPE_ROM = 1,
     FILETYPE_IMAGE = 2,
     FILETYPE_IMAGE_DIFF = 3,
     FILETYPE_SAMPLE = 4,
     FILETYPE_ARTWORK = 5,
     FILETYPE_NVRAM = 6,
     FILETYPE_HIGHSCORE = 7,
     FILETYPE_HIGHSCORE_DB = 8,
     FILETYPE_CONFIG = 9,
     FILETYPE_INPUTLOG = 10,
     FILETYPE_STATE = 11,
     FILETYPE_MEMCARD = 12,
     FILETYPE_SCREENSHOT = 13,
     FILETYPE_HISTORY = 14,
     FILETYPE_CHEAT = 15,
     FILETYPE_LANGUAGE = 16,
     FILETYPE_CTRLR = 17,
     FILETYPE_INI = 18,
#if MESS
	FILETYPE_CRC,
#endif
 FILETYPE_end = 19; /* dummy last entry */

        const int PLAIN_FILE = 0, RAM_FILE = 1, ZIPPED_FILE = 2, UNLOADED_ZIPPED_FILE = 3;


        const byte FILEFLAG_OPENREAD = 0x01;
        const byte FILEFLAG_OPENWRITE = 0x02;
        const byte FILEFLAG_HASH = 0x04;
        const byte FILEFLAG_REVERSE_SEARCH = 0x08;
        const byte FILEFLAG_VERIFY_ONLY = 0x10;
        const byte FILEFLAG_NOZIP = 0x20;

        class mame_file
        {
            public osd_file file;
            public byte[] data;
            public ulong offset, length;
            public bool eof;
            public byte type;
            public string hash;
        }

    }
}
