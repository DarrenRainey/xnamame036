using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class atari_vg
    {
        const byte EAROM_SIZE = 0x40;
        static int earom_offset;
        static int earom_data;
        static byte[] earom = new byte[EAROM_SIZE];


        public static void atari_vg_earom_w(int offset, int data)
        {
            //if (errorlog)fprintf(errorlog, "write earom: %02x:%02x\n", offset, data);
            earom_offset = offset;
            earom_data = data;
        }

        /* 0,8 and 14 get written to this location, too.
         * Don't know what they do exactly
         */
        public static void atari_vg_earom_ctrl(int offset, int data)
        {
            //if (errorlog)fprintf(errorlog, "earom ctrl: %02x:%02x\n", offset, data);
            /*
                0x01 = clock
                0x02 = set data latch? - writes only (not always)
                0x04 = write mode? - writes only
                0x08 = set addr latch?
            */
            if ((data & 0x01)!=0)
                earom_data = earom[earom_offset];
            if ((data & 0x0c) == 0x0c)
            {
                earom[earom_offset] = (byte)earom_data;
                //if (errorlog)fprintf(errorlog, "    written %02x:%02x\n", earom_offset, earom_data);
            }
        }
        public static int atari_vg_earom_r(int offset)
        {
            //if (errorlog)fprintf(errorlog, "read earom: %02x(%02x):%02x\n", earom_offset, offset, earom_data);
            return (earom_data);
        }
        public static void atari_vg_earom_handler(object file, int read_or_write)
        {
            if (read_or_write!=0)
                Mame.osd_fwrite(file, earom, EAROM_SIZE);
            else
            {
                if (file!=null)
                    Mame.osd_fread(file, earom, EAROM_SIZE);
                else
                    Array.Clear(earom, 0, EAROM_SIZE);
            }
        }
    }
}
