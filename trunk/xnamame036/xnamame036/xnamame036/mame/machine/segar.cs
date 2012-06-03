using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class segar
    {
        public delegate void segar_decrypt(int a, ref uint b);

        public static segar_decrypt sega_decrypt;
        public static _BytePtr segar_mem = new _BytePtr(1);

        static void sega_decrypt76(int pc, ref uint lo)
        {
            uint i = 0;
            uint b = lo;

            switch (pc & 0x09)
            {
                case 0x00:
                    /* A */
                    i = b;
                    break;
                case 0x01:
                    /* B */
                    i = b & 0x03;
                    i += ((b & 0x80) >> 1);
                    i += ((b & 0x60) >> 3);
                    i += ((~b) & 0x10);
                    i += ((b & 0x08) << 2);
                    i += ((b & 0x04) << 5);
                    i &= 0xFF;
                    break;
                case 0x08:
                    /* C */
                    i = b & 0x03;
                    i += ((b & 0x80) >> 4);
                    i += (((~b) & 0x40) >> 1);
                    i += ((b & 0x20) >> 1);
                    i += ((b & 0x10) >> 2);
                    i += ((b & 0x08) << 3);
                    i += ((b & 0x04) << 5);
                    i &= 0xFF;
                    break;
                case 0x09:
                    /* D */
                    i = b & 0x23;
                    i += ((b & 0xC0) >> 4);
                    i += ((b & 0x10) << 2);
                    i += ((b & 0x08) << 1);
                    i += (((~b) & 0x04) << 5);
                    i &= 0xFF;
                    break;
            }

            lo = i;
        }
        static void sega_decrypt82(int pc, ref uint lo)
        {
            uint i = 0;
            uint b = lo;

            switch (pc & 0x11)
            {
                case 0x00:
                    /* A */
                    i = b;
                    break;
                case 0x01:
                    /* B */
                    i = b & 0x03;
                    i += ((b & 0x80) >> 1);
                    i += ((b & 0x60) >> 3);
                    i += ((~b) & 0x10);
                    i += ((b & 0x08) << 2);
                    i += ((b & 0x04) << 5);
                    i &= 0xFF;
                    break;
                case 0x10:
                    /* C */
                    i = b & 0x03;
                    i += ((b & 0x80) >> 4);
                    i += (((~b) & 0x40) >> 1);
                    i += ((b & 0x20) >> 1);
                    i += ((b & 0x10) >> 2);
                    i += ((b & 0x08) << 3);
                    i += ((b & 0x04) << 5);
                    i &= 0xFF;
                    break;
                case 0x11:
                    /* D */
                    i = b & 0x23;
                    i += ((b & 0xC0) >> 4);
                    i += ((b & 0x10) << 2);
                    i += ((b & 0x08) << 1);
                    i += (((~b) & 0x04) << 5);
                    i &= 0xFF;
                    break;
            }

            lo = i;
        }
        public static void sega_security(int chip)
        {
            switch (chip)
            {
                default: throw new Exception();
                case 76: sega_decrypt = sega_decrypt76; break;
                case 82: sega_decrypt = sega_decrypt82; break;
            }
        }
    }
}
