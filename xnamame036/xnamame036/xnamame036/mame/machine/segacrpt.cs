using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class segacrpt
    {
        static void sega_decode(byte[][] xortable)
        {
            int A;
            _BytePtr rom = Mame.memory_region(Mame.REGION_CPU1);
            int diff = Mame.memory_region_length(Mame.REGION_CPU1) / 2;


            Mame.memory_set_opcode_base(0, new _BytePtr(rom, diff));

            for (A = 0x0000; A < 0x8000; A++)
            {
                int row, col;
                byte src;


                src = rom[A];

                /* pick the translation table from bits 0, 4, 8 and 12 of the address */
                row = (A & 1) + (((A >> 4) & 1) << 1) + (((A >> 8) & 1) << 2) + (((A >> 12) & 1) << 3);

                /* pick the offset in the table from bits 3 and 5 of the source data */
                col = ((src >> 3) & 1) + (((src >> 5) & 1) << 1);
                /* the bottom half of the translation table is the mirror image of the top */
                if ((src & 0x80) != 0) col = 3 - col;

                /* decode the opcodes */
                rom[A + diff] = (byte)(src ^ xortable[2 * row][col]);

                /* decode the data */
                rom[A] = (byte)(src ^ xortable[2 * row + 1][col]);

                if (xortable[2 * row][col] == 0xff)	/* table incomplete! (for development) */
                    rom[A + diff] = 0x00;
                if (xortable[2 * row + 1][col] == 0xff)	/* table incomplete! (for development) */
                    rom[A] = 0xee;
            }

            /* copy the opcodes from the not encrypted part of the ROMs */
            for (A = 0x8000; A < diff; A++)
                rom[A + diff] = rom[A];
        }

        public static void szaxxon_decode()
        {
            byte[][] xortable =
	{
		/*       opcode                   data                     address      */
		/*  A    B    C    D         A    B    C    D                           */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 },	/* ...0...0...0...0 */
		new byte[]{ 0x08,0x20,0xa8,0x80 }, new byte []{ 0x88,0x88,0x28,0x28 },	/* ...0...0...0...1 */
		new byte[]{ 0xa8,0x20,0x80,0x08 }, new byte []{ 0x20,0xa8,0x20,0xa8 },	/* ...0...0...1...0 */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 },	/* ...0...0...1...1 */
		new byte[]{ 0x08,0x20,0xa8,0x80 }, new byte []{ 0x88,0x88,0x28,0x28 },	/* ...0...1...0...0 */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 },	/* ...0...1...0...1 */
		new byte[]{ 0xa8,0x20,0x80,0x08 }, new byte []{ 0x20,0xa8,0x20,0xa8 },	/* ...0...1...1...0 */
		new byte[]{ 0x08,0x20,0xa8,0x80 }, new byte []{ 0x88,0x88,0x28,0x28 },	/* ...0...1...1...1 */
		new byte[]{ 0x08,0x20,0xa8,0x80 }, new byte []{ 0x88,0x88,0x28,0x28 },	/* ...1...0...0...0 */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 },	/* ...1...0...0...1 */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 },	/* ...1...0...1...0 */
		new byte[]{ 0xa8,0x20,0x80,0x08 }, new byte []{ 0x20,0xa8,0x20,0xa8 },	/* ...1...0...1...1 */
		new byte[]{ 0xa8,0x20,0x80,0x08 }, new byte []{ 0x20,0xa8,0x20,0xa8 },	/* ...1...1...0...0 */
		new byte[]{ 0xa8,0x20,0x80,0x08 }, new byte []{ 0x20,0xa8,0x20,0xa8 },	/* ...1...1...0...1 */
		new byte[]{ 0x08,0x20,0xa8,0x80 }, new byte []{ 0x88,0x88,0x28,0x28 },	/* ...1...1...1...0 */
		new byte[]{ 0x88,0xa0,0xa0,0x88 }, new byte []{ 0x28,0x28,0x88,0x88 }	/* ...1...1...1...1 */
	};


            sega_decode(xortable);
        }
    }
}
