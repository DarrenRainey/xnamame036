using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_nemesis : Mame.GameDriver
    {
        static _BytePtr ram = new _BytePtr(1);
        static _BytePtr ram2 = new _BytePtr(1);
        static _BytePtr nemesis_videoram1 = new _BytePtr(1);
        static _BytePtr nemesis_videoram2 = new _BytePtr(1);
        static _BytePtr nemesis_characterram = new _BytePtr(1);
        static _BytePtr nemesis_characterram_gfx = new _BytePtr(1);
        static _BytePtr nemesis_xscroll1 = new _BytePtr(1), nemesis_xscroll2 = new _BytePtr(1), nemesis_yscroll = new _BytePtr(1);

        static int[] nemesis_characterram_size = new int[1];

        static Mame.osd_bitmap tmpbitmap2, tmpbitmap3, tmpbitmap4;
        static bool[] video1_dirty;
        static bool[] video2_dirty;
        static byte[] char_dirty;
        static byte[] sprite_dirty;
        static bool[] sprite3216_dirty;
        static bool[] sprite816_dirty;
        static bool[] sprite1632_dirty;
        static bool[] sprite3232_dirty;
        static bool[] sprite168_dirty;
        static bool[] sprite6464_dirty;

        static int irq_on = 0;
        static int irq1_on = 0;
        static int irq2_on = 0;
        static int irq4_on = 0;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x000000, 0x03ffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x040000, 0x04ffff, nemesis_characterram_r ),
	new Mame.MemoryReadAddress( 0x050000, 0x0503ff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x050400, 0x0507ff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0x050800, 0x050bff, Mame.MRA_BANK3 ),
	new Mame.MemoryReadAddress( 0x050c00, 0x050fff, Mame.MRA_BANK4 ),
	new Mame.MemoryReadAddress( 0x052000, 0x053fff, nemesis_videoram1_r ),
	new Mame.MemoryReadAddress( 0x054000, 0x055fff, nemesis_videoram2_r ),
	new Mame.MemoryReadAddress( 0x056000, 0x056fff,  Mame.MRA_BANK5 ),
	new Mame.MemoryReadAddress( 0x05a000, 0x05afff, Mame.paletteram_word_r ),
	new Mame.MemoryReadAddress( 0x05c400, 0x05c401, Mame.input_port_4_r ),	/* DSW0 */
	new Mame.MemoryReadAddress( 0x05c402, 0x05c403, Mame.input_port_5_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0x05cc00, 0x05cc01, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x05cc02, 0x05cc03, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x05cc04, 0x05cc05, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( 0x05cc06, 0x05cc07, Mame.input_port_3_r ),	/* TEST */
	new Mame.MemoryReadAddress( 0x060000, 0x067fff, Mame.MRA_BANK6 ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x03ffff,  Mame.MWA_ROM ),	/* ROM */
	new Mame.MemoryWriteAddress( 0x040000, 0x04ffff, nemesis_characterram_w, nemesis_characterram, nemesis_characterram_size ),
	new Mame.MemoryWriteAddress( 0x050000, 0x0503ff,  Mame.MWA_BANK1, nemesis_xscroll1 ),
	new Mame.MemoryWriteAddress( 0x050400, 0x0507ff,  Mame.MWA_BANK2, nemesis_xscroll2 ),
	new Mame.MemoryWriteAddress( 0x050800, 0x050bff,  Mame.MWA_BANK3 ),
	new Mame.MemoryWriteAddress( 0x050c00, 0x050fff,  Mame.MWA_BANK4, nemesis_yscroll ),
	new Mame.MemoryWriteAddress( 0x051000, 0x051fff,  Mame.MWA_NOP ),		/* used, but written to with 0's */
	new Mame.MemoryWriteAddress( 0x052000, 0x053fff, nemesis_videoram1_w, nemesis_videoram1 ),	/* VRAM 1 */
	new Mame.MemoryWriteAddress( 0x054000, 0x055fff, nemesis_videoram2_w, nemesis_videoram2 ),	/* VRAM 2 */
	new Mame.MemoryWriteAddress( 0x056000, 0x056fff,  Mame.MWA_BANK5, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x05a000, 0x05afff, nemesis_palette_w,  Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x05c000, 0x05c001, nemesis_soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x05c800, 0x05c801,  Mame.watchdog_reset_w ),	/* probably */
	new Mame.MemoryWriteAddress( 0x05e000, 0x05e001, nemesis_irq_enable_w ),	/* Nemesis */
	new Mame.MemoryWriteAddress( 0x05e002, 0x05e003, nemesis_irq_enable_w ),	/* Konami GT */
	new Mame.MemoryWriteAddress( 0x05e004, 0x05e005,  Mame.MWA_NOP),	/* bit 8 of the word probably triggers IRQ on sound board */
	new Mame.MemoryWriteAddress( 0x060000, 0x067fff,  Mame.MWA_BANK6, ram ),	/* WORK RAM */
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM),
	new Mame.MemoryReadAddress( 0xe001, 0xe001, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0xe086, 0xe086, ay8910.AY8910_read_port_0_r ),
	new Mame.MemoryReadAddress( 0xe205, 0xe205, ay8910.AY8910_read_port_1_r ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xafff, k005289.k005289_pitch_A_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, k005289.k005289_pitch_B_w ),
	new Mame.MemoryWriteAddress( 0xe003, 0xe003, k005289.k005289_keylatch_A_w ),
	new Mame.MemoryWriteAddress( 0xe004, 0xe004, k005289.k005289_keylatch_B_w ),
	new Mame.MemoryWriteAddress( 0xe005, 0xe005, ay8910.AY8910_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xe006, 0xe006, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe106, 0xe106, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe405, 0xe405, ay8910.AY8910_write_port_1_w ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            2048,	/* 2048 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8     /* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            512,	/* 512 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4 },
            new uint[]{ 0*64, 1*64, 2*64, 3*64, 4*64, 5*64, 6*64, 7*64,
			8*64, 9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64 },
            128 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout3216 =
        new Mame.GfxLayout(
            32, 16,	/* 32*16 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4,
		   16*4,17*4, 18*4, 19*4, 20*4, 21*4, 22*4, 23*4,
		   24*4,25*4, 26*4, 27*4, 28*4, 29*4, 30*4, 31*4},
            new uint[]{ 0*128, 1*128, 2*128, 3*128, 4*128, 5*128, 6*128, 7*128,
			8*128, 9*128, 10*128, 11*128, 12*128, 13*128, 14*128, 15*128 },
            256 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout1632 =
        new Mame.GfxLayout(
            16, 32,	/* 16*32 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4},
            new uint[]{ 0*64,  1*64,  2*64,  3*64,  4*64,  5*64,  6*64,  7*64,
	  8*64,  9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64,
	 16*64, 17*64, 18*64, 19*64, 20*64, 21*64, 22*64, 23*64,
	 24*64, 25*64, 26*64, 27*64, 28*64, 29*64, 30*64, 31*64},
            256 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout3232 =
        new Mame.GfxLayout(
            32, 32,	/* 32*32 sprites */
            128,	/* 128 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4,
		   16*4,17*4, 18*4, 19*4, 20*4, 21*4, 22*4, 23*4,
		   24*4,25*4, 26*4, 27*4, 28*4, 29*4, 30*4, 31*4},
            new uint[]{ 0*128, 1*128, 2*128, 3*128, 4*128, 5*128, 6*128, 7*128,
			8*128,  9*128, 10*128, 11*128, 12*128, 13*128, 14*128, 15*128,
		   16*128, 17*128, 18*128, 19*128, 20*128, 21*128, 22*128, 23*128,
		   24*128, 25*128, 26*128, 27*128, 28*128, 29*128, 30*128, 31*128},
            512 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout816 =
        new Mame.GfxLayout(
            8, 16,	/* 16*16 sprites */
            1024,	/* 1024 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			8*32, 9*32, 10*32, 11*32, 12*32, 13*32, 14*32, 15*32 },
            64 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout168 =
        new Mame.GfxLayout(
            16, 8,	/* 16*8 sprites */
            1024,	/* 1024 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4},
            new uint[] { 0 * 64, 1 * 64, 2 * 64, 3 * 64, 4 * 64, 5 * 64, 6 * 64, 7 * 64 },
            64 * 8     /* every sprite takes 128 consecutive bytes */

        );

        static Mame.GfxLayout spritelayout6464 =
        new Mame.GfxLayout(
            64, 64,	/* 32*32 sprites */
            32,	/* 128 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* the two bitplanes are merged in the same nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4,
		   16*4,17*4, 18*4, 19*4, 20*4, 21*4, 22*4, 23*4,
		   24*4,25*4, 26*4, 27*4, 28*4, 29*4, 30*4, 31*4,
		   32*4,33*4, 34*4, 35*4, 36*4, 37*4, 38*4, 39*4,
		   40*4,41*4, 42*4, 43*4, 44*4, 45*4, 46*4, 47*4,
		   48*4,49*4, 50*4, 51*4, 52*4, 53*4, 54*4, 55*4,
		   56*4,57*4, 58*4, 59*4, 60*4, 61*4, 62*4, 63*4},

            new uint[]{ 0*256, 1*256, 2*256, 3*256, 4*256, 5*256, 6*256, 7*256,
			8*256,  9*256, 10*256, 11*256, 12*256, 13*256, 14*256, 15*256,
		   16*256, 17*256, 18*256, 19*256, 20*256, 21*256, 22*256, 23*256,
		   24*256, 25*256, 26*256, 27*256, 28*256, 29*256, 30*256, 31*256,
		   32*256, 33*256, 34*256, 35*256, 36*256, 37*256, 38*256, 39*256,
		   40*256, 41*256, 42*256, 43*256, 44*256, 45*256, 46*256, 47*256,
		   48*256, 49*256, 50*256, 51*256, 52*256, 53*256, 54*256, 55*256,
		   56*256, 57*256, 58*256, 59*256, 60*256, 61*256, 62*256, 63*256},
            2048 * 8     /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
    new Mame.GfxDecodeInfo(0, 0x0, charlayout,   0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout3216, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout816, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout3232, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout1632, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout168, 0, 0x80 ),	/* the game dynamically modifies this */
    new Mame.GfxDecodeInfo(0, 0x0, spritelayout6464, 0, 0x80 ),	/* the game dynamically modifies this */
};


        static int nemesis_videoram1_r(int offset)
        {
            return nemesis_videoram1.READ_WORD(offset);
        }
        static int nemesis_videoram2_r(int offset)
        {
            return nemesis_videoram2.READ_WORD(offset);
        }
        static int nemesis_characterram_r(int offset)
        {
            int res;

            res = nemesis_characterram_gfx.READ_WORD(offset);

#if WINDOWS
            res = ((res & 0x00ff) << 8) | ((res & 0xff00) >> 8);
#endif

            return res;
        }
        static void nemesis_characterram_w(int offset, int data)
        {
            int oldword = nemesis_characterram_gfx.READ_WORD(offset);
            int newword;

            Mame.COMBINE_WORD_MEM(nemesis_characterram, offset, data);	/* this is need so that twinbee can run code in the
																character RAM */

#if WINDOWS
            data = (int)(((data & 0x00ff00ff) << 8) | ((data & 0xff00ff00) >> 8));
#endif

            newword = Mame.COMBINE_WORD(oldword, data);
            if (oldword != newword)
            {
                nemesis_characterram_gfx.WRITE_WORD(offset, (ushort)newword);

                char_dirty[offset / 32] = 1;
                sprite_dirty[offset / 128] = 1;
                sprite3216_dirty[offset / 256] = true;
                sprite1632_dirty[offset / 256] = true;
                sprite3232_dirty[offset / 512] = true;
                sprite168_dirty[offset / 64] = true;
                sprite816_dirty[offset / 64] = true;
                sprite6464_dirty[offset / 2048] = true;
            }
        }
        static void nemesis_videoram1_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(nemesis_videoram1, offset, data);
            if (offset < 0x1000)
                video1_dirty[offset / 2] = true;
            else
                video2_dirty[(offset - 0x1000) / 2] = true;
        }
        static void nemesis_videoram2_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(nemesis_videoram2, offset, data);
            if (offset < 0x1000)
                video1_dirty[offset / 2] = true;
            else
                video2_dirty[(offset - 0x1000) / 2] = true;
        }
        static void nemesis_palette_w(int offset, int data)
        {
            int r, g, b, bit1, bit2, bit3, bit4, bit5;

            Mame.COMBINE_WORD_MEM(Mame.paletteram, offset, data);
            data = Mame.paletteram.READ_WORD(offset);

            /* Mish, 30/11/99 - Schematics show the resistor values are:
                300 Ohms
                620 Ohms
                1200 Ohms
                2400 Ohms
                4700 Ohms

                So the correct weights per bit are 8, 17, 33, 67, 130
            */

            // MULTIPLIER 8 * bit1 + 17 * bit2 + 33 * bit3 + 67 * bit4 + 130 * bit5

            bit1 = (data >> 0) & 1;
            bit2 = (data >> 1) & 1;
            bit3 = (data >> 2) & 1;
            bit4 = (data >> 3) & 1;
            bit5 = (data >> 4) & 1;
            r = 8 * bit1 + 17 * bit2 + 33 * bit3 + 67 * bit4 + 130 * bit5;
            r = (int)Math.Pow(r / 255.0, 2) * 255;
            bit1 = (data >> 5) & 1;
            bit2 = (data >> 6) & 1;
            bit3 = (data >> 7) & 1;
            bit4 = (data >> 8) & 1;
            bit5 = (data >> 9) & 1;
            g = 8 * bit1 + 17 * bit2 + 33 * bit3 + 67 * bit4 + 130 * bit5;
            g = (int)Math.Pow(g / 255.0, 2) * 255;
            bit1 = (data >> 10) & 1;
            bit2 = (data >> 11) & 1;
            bit3 = (data >> 12) & 1;
            bit4 = (data >> 13) & 1;
            bit5 = (data >> 14) & 1;
            b = 8 * bit1 + 17 * bit2 + 33 * bit3 + 67 * bit4 + 130 * bit5;
            b = (int)Math.Pow(b / 255.0, 2) * 255;

            Mame.palette_change_color(offset / 2, (byte)r, (byte)g, (byte)b);
        }
        static void nemesis_irq_enable_w(int offset, int data)
        {
            irq_on = data & 0xff;
        }
        static int nemesis_interrupt()
        {
            if (irq_on != 0) return 1;

            return 0;
        }
        static void nemesis_soundlatch_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data & 0xff);

            /* the IRQ should probably be generated by 5e004, but we'll handle it here for now */
            Mame.cpu_cause_interrupt(1, 0xff);
        }







        public static void nemesis_vh_stop()
        {
            Mame.osd_free_bitmap(Generic.tmpbitmap);
            Mame.osd_free_bitmap(tmpbitmap2);
            Mame.osd_free_bitmap(tmpbitmap3);
            Mame.osd_free_bitmap(tmpbitmap4);
            Generic.tmpbitmap = null;
            char_dirty = null;
            sprite_dirty = null;
            sprite3216_dirty = null;
            sprite1632_dirty = null;
            sprite3232_dirty = null;
            sprite168_dirty = null;
            sprite816_dirty = null;
            sprite6464_dirty = null;
            char_dirty = null;
            video1_dirty = null;
            video2_dirty = null;
            nemesis_characterram_gfx = null;
        }
        public static int nemesis_vh_start()
        {
            Generic.tmpbitmap = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

            tmpbitmap2 = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

            tmpbitmap3 = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

            tmpbitmap4 = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

            char_dirty = new byte[2048];
            for (int i = 0; i < 2048; i++) char_dirty[i] = 1;

            sprite_dirty = new byte[512];
            for (int i = 0; i < 512; i++) sprite_dirty[i] = 1;

            sprite3216_dirty = new bool[256];
            for (int i = 0; i < 256; i++) sprite3216_dirty[i] = true;

            sprite1632_dirty = new bool[256];
            for (int i = 0; i < 256; i++) sprite1632_dirty[i] = true;

            sprite3232_dirty = new bool[128];
            for (int i = 0; i < 128; i++) sprite3232_dirty[i] = true;

            sprite168_dirty = new bool[1024];
            for (int i = 0; i < 1024; i++) sprite168_dirty[i] = true;

            sprite816_dirty = new bool[1024];
            for (int i = 0; i < 1024; i++) sprite816_dirty[i] = true;

            sprite6464_dirty = new bool[32];
            for (int i = 0; i < 32; i++) sprite6464_dirty[i] = true;

            video1_dirty = new bool[0x800];
            video2_dirty = new bool[0x800];
            for (int i = 0; i < 0x800; i++)
            {
                video1_dirty[i] = true;
                video2_dirty[i] = true;
            }

            nemesis_characterram_gfx = new _BytePtr(nemesis_characterram_size[0]);
            Array.Clear(nemesis_characterram_gfx.buffer, nemesis_characterram_gfx.offset, nemesis_characterram_size[0]);
            //memset(nemesis_characterram_gfx, 0, nemesis_characterram_size);

            return 0;
        }
        static void setup_palette()
        {
            int color, code, i;
            int[] colmask = new int[0x80];
            int pal_base, offs;

            Mame.palette_init_used_colors();

            pal_base = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;
            for (color = 0; color < 0x80; color++) colmask[color] = 0;
            for (offs = 0x1000 - 2; offs >= 0; offs -= 2)
            {
                code = nemesis_videoram1.READ_WORD(offs + 0x1000) & 0x7ff;
                if (char_dirty[code] == 1)
                {
                    Mame.decodechar(Mame.Machine.gfx[0], code, nemesis_characterram_gfx,
                            Mame.Machine.drv.gfxdecodeinfo[0].gfxlayout);
                    char_dirty[code] = 2;
                }
                color = nemesis_videoram2.READ_WORD(offs + 0x1000) & 0x7f;
                colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
            }

            for (color = 0; color < 0x80; color++)
            {
                if ((colmask[color] & (1 << 0)) != 0)
                    Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                for (i = 1; i < 16; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                }
            }


            pal_base = Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start;
            for (color = 0; color < 0x80; color++) colmask[color] = 0;
            for (offs = 0; offs < Generic.spriteram_size[0]; offs += 16)
            {
                int char_type;
                int zoom = Generic.spriteram.READ_WORD(offs + 4);
                code = Generic.spriteram.READ_WORD(offs + 6) + ((Generic.spriteram.READ_WORD(offs + 8) & 0xc0) << 2);
                if (zoom != 0xFF || code != 0)
                {
                    int size = Generic.spriteram.READ_WORD(offs + 2);
                    switch (size & 0x38)
                    {
                        case 0x00:
                            /* sprite 32x32*/
                            char_type = 4;
                            code /= 8;
                            if (sprite3232_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite3232_dirty[code] = false;
                            }
                            break;
                        case 0x08:
                            /* sprite 16x32 */
                            char_type = 5;
                            code /= 4;
                            if (sprite1632_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite1632_dirty[code] = false;

                            }
                            break;
                        case 0x10:
                            /* sprite 32x16 */
                            char_type = 2;
                            code /= 4;
                            if (sprite3216_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite3216_dirty[code] = false;
                            }
                            break;
                        case 0x18:
                            /* sprite 64x64 */
                            char_type = 7;
                            code /= 32;
                            if (sprite6464_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite6464_dirty[code] = false;
                            }
                            break;
                        case 0x20:
                            /* char 8x8 */
                            char_type = 0;
                            code *= 2;
                            if (char_dirty[code] == 1)
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                char_dirty[code] = 0;
                            }
                            break;
                        case 0x28:
                            /* sprite 16x8 */
                            char_type = 6;
                            if (sprite168_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite168_dirty[code] = false;
                            }
                            break;
                        case 0x30:
                            /* sprite 8x16 */
                            char_type = 3;
                            if (sprite816_dirty[code])
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite816_dirty[code] = false;
                            }
                            break;
                        default:
                            Mame.printf("UN-SUPPORTED SPRITE SIZE %-4x\n", size & 0x38);
                            goto case 0x38;
                        case 0x38:
                            /* sprite 16x16 */
                            char_type = 1;
                            code /= 2;
                            if (sprite_dirty[code] == 1)
                            {
                                Mame.decodechar(Mame.Machine.gfx[char_type], code, nemesis_characterram_gfx,
                                        Mame.Machine.drv.gfxdecodeinfo[char_type].gfxlayout);
                                sprite_dirty[code] = 2;

                            }
                            break;
                    }

                    color = (Generic.spriteram.READ_WORD(offs + 8) & 0x1e) >> 1;
                    colmask[color] |= (int)Mame.Machine.gfx[char_type].pen_usage[code];
                }
            }


            for (color = 0; color < 0x80; color++)
            {
                if ((colmask[color] & (1 << 0)) != 0)
                    Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                for (i = 1; i < 16; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                }
            }


            pal_base = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;
            for (color = 0; color < 0x80; color++) colmask[color] = 0;
            for (offs = 0x1000 - 2; offs >= 0; offs -= 2)
            {
                code = nemesis_videoram1.READ_WORD(offs) & 0x7ff;
                if (char_dirty[code] == 1)
                {
                    Mame.decodechar(Mame.Machine.gfx[0], code, nemesis_characterram_gfx,
                            Mame.Machine.drv.gfxdecodeinfo[0].gfxlayout);
                    char_dirty[code] = 2;
                }
                color = nemesis_videoram2.READ_WORD(offs) & 0x7f;
                colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
            }

            for (color = 0; color < 0x80; color++)
            {
                if ((colmask[color] & (1 << 0)) != 0)
                    Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                for (i = 1; i < 16; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                }
            }

            if (Mame.palette_recalc() != null)
            {
                for (i = 0; i < 0x800; i++)
                {
                    video1_dirty[i] = true;
                    video2_dirty[i] = true;
                }
            }
        }

        static void setup_backgrounds()
        {
            int offs;

            /* Do the foreground first */
            for (offs = 0x1000 - 2; offs >= 0; offs -= 2)
            {
                int code, color;


                code = nemesis_videoram1.READ_WORD(offs + 0x1000) & 0x7ff;

                if (video2_dirty[offs / 2] || char_dirty[code] != 0)
                {
                    int sx, sy, flipx, flipy;

                    color = nemesis_videoram2.READ_WORD(offs + 0x1000) & 0x7f;

                    video2_dirty[offs / 2] = false;

                    sx = (offs / 2) % 64;
                    sy = (offs / 2) / 64;
                    flipx = nemesis_videoram2.READ_WORD(offs + 0x1000) & 0x80;
                    flipy = nemesis_videoram1.READ_WORD(offs + 0x1000) & 0x800;

                    if (nemesis_videoram1.READ_WORD(offs + 0x1000) != 0 || nemesis_videoram2.READ_WORD(offs + 0x1000) != 0)
                    {
                        if ((nemesis_videoram1.READ_WORD(offs + 0x1000) & 0x1000) != 0)		//screen priority
                        {
                            Mame.rectangle clip = new Mame.rectangle();

                            Mame.drawgfx(tmpbitmap3, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);

                            clip.min_x = 8 * sx;
                            clip.max_x = 8 * sx + 7;
                            clip.min_y = 8 * sy;
                            clip.max_y = 8 * sy + 7;
                            Mame.fillbitmap(Generic.tmpbitmap, Mame.palette_transparent_pen, clip);
                        }
                        else
                        {
                            Mame.rectangle clip = new Mame.rectangle();

                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);

                            clip.min_x = 8 * sx;
                            clip.max_x = 8 * sx + 7;
                            clip.min_y = 8 * sy;
                            clip.max_y = 8 * sy + 7;
                            Mame.fillbitmap(tmpbitmap3, Mame.palette_transparent_pen, clip);
                        }
                    }
                    else
                    {
                        Mame.rectangle clip = new Mame.rectangle();
                        clip.min_x = 8 * sx;
                        clip.max_x = 8 * sx + 7;
                        clip.min_y = 8 * sy;
                        clip.max_y = 8 * sy + 7;
                        Mame.fillbitmap(Generic.tmpbitmap, Mame.palette_transparent_pen, clip);
                        Mame.fillbitmap(tmpbitmap3, Mame.palette_transparent_pen, clip);
                    }
                }
            }

            /* Background */
            for (offs = 0x1000 - 2; offs >= 0; offs -= 2)
            {
                int code, color;


                code = nemesis_videoram1.READ_WORD(offs) & 0x7ff;

                if (video1_dirty[offs / 2] || char_dirty[code] != 0)
                {
                    int sx, sy, flipx, flipy;

                    video1_dirty[offs / 2] = false;

                    color = nemesis_videoram2.READ_WORD(offs) & 0x7f;

                    sx = (offs / 2) % 64;
                    sy = (offs / 2) / 64;
                    flipx = nemesis_videoram2.READ_WORD(offs) & 0x80;
                    flipy = nemesis_videoram1.READ_WORD(offs) & 0x800;

                    if (nemesis_videoram1.READ_WORD(offs) != 0 || nemesis_videoram2.READ_WORD(offs) != 0)
                    {
                        if ((nemesis_videoram1.READ_WORD(offs) & 0x1000) != 0)		//screen priority
                        {
                            Mame.rectangle clip = new Mame.rectangle();

                            Mame.drawgfx(tmpbitmap4, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);

                            clip.min_x = 8 * sx;
                            clip.max_x = 8 * sx + 7;
                            clip.min_y = 8 * sy;
                            clip.max_y = 8 * sy + 7;
                            Mame.fillbitmap(tmpbitmap2, Mame.palette_transparent_pen, clip);
                        }
                        else
                        {
                            Mame.rectangle clip = new Mame.rectangle();

                            Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);

                            clip.min_x = 8 * sx;
                            clip.max_x = 8 * sx + 7;
                            clip.min_y = 8 * sy;
                            clip.max_y = 8 * sy + 7;
                            Mame.fillbitmap(tmpbitmap4, Mame.palette_transparent_pen, clip);
                        }
                    }
                    else
                    {
                        Mame.rectangle clip = new Mame.rectangle();
                        clip.min_x = 8 * sx;
                        clip.max_x = 8 * sx + 7;
                        clip.min_y = 8 * sy;
                        clip.max_y = 8 * sy + 7;
                        Mame.fillbitmap(tmpbitmap2, Mame.palette_transparent_pen, clip);
                        Mame.fillbitmap(tmpbitmap4, Mame.palette_transparent_pen, clip);
                    }
                }
            }
        }
        static void draw_sprites(Mame.osd_bitmap bitmap)
        {
            /*
             *	16 bytes per sprite, in memory from 56000-56fff
             *
             *	byte	0 :	relative priority.
             *	byte	2 :	size (?) value #E0 means not used., bit 0x01 is flipx
                            0xc0 is upper 2 bits of zoom.
                            0x38 is size.
             * 	byte	4 :	zoom = 0xff
             *	byte	6 :	low bits sprite code.
             *	byte	8 :	color + hi bits sprite code., bit 0x20 is flipy bit. bit 0x01 is high bit of X pos.
             *	byte	A :	X position.
             *	byte	C :	Y position.
             * 	byte	E :	not used.
             */

            int adress;	/* start of sprite in spriteram */
            int sx;	/* sprite X-pos */
            int sy;	/* sprite Y-pos */
            int code;	/* start of sprite in obj RAM */
            int color;	/* color of the sprite */
            int flipx, flipy;
            int zoom;
            int char_type;
            int priority;
            int size;

            for (priority = 0; priority < 256; priority++)
            {
                for (adress = 0; adress < Generic.spriteram_size[0]; adress += 16)
                {
                    if (Generic.spriteram.READ_WORD(adress) != priority) continue;

                    code = Generic.spriteram.READ_WORD(adress + 6) + ((Generic.spriteram.READ_WORD(adress + 8) & 0xc0) << 2);
                    zoom = Generic.spriteram.READ_WORD(adress + 4) & 0xff;
                    if (zoom != 0xFF || code != 0)
                    {
                        size = Generic.spriteram.READ_WORD(adress + 2);
                        zoom += (size & 0xc0) << 2;

                        sx = Generic.spriteram.READ_WORD(adress + 10) & 0xff;
                        sy = Generic.spriteram.READ_WORD(adress + 12) & 0xff;
                        if ((Generic.spriteram.READ_WORD(adress + 8) & 1) != 0) sx -= 0x100;	/* fixes left side clip */
                        color = (Generic.spriteram.READ_WORD(adress + 8) & 0x1e) >> 1;
                        flipx = Generic.spriteram.READ_WORD(adress + 2) & 0x01;
                        flipy = Generic.spriteram.READ_WORD(adress + 8) & 0x20;

                        switch (size & 0x38)
                        {
                            case 0x00:	/* sprite 32x32*/
                                char_type = 4;
                                code /= 8;
                                break;
                            case 0x08:	/* sprite 16x32 */
                                char_type = 5;
                                code /= 4;
                                break;
                            case 0x10:	/* sprite 32x16 */
                                char_type = 2;
                                code /= 4;
                                break;
                            case 0x18:		/* sprite 64x64 */
                                char_type = 7;
                                code /= 32;
                                break;
                            case 0x20:	/* char 8x8 */
                                char_type = 0;
                                code *= 2;
                                break;
                            case 0x28:		/* sprite 16x8 */
                                char_type = 6;
                                break;
                            case 0x30:	/* sprite 8x16 */
                                char_type = 3;
                                break;
                            case 0x38:
                            default:	/* sprite 16x16 */
                                char_type = 1;
                                code /= 2;
                                break;
                        }

                        /*  0x80 == no zoom */
                        if (zoom == 0x80)
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[char_type],
                                    (uint)code,
                                    (uint)color,
                                    flipx!=0, flipy!=0,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        else if (zoom >= 0x80)
                        {
                            nemesis_drawgfx_zoomdown(bitmap, Mame.Machine.gfx[char_type],
                                    (uint)code,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0, zoom);
                        }
                        else if (zoom >= 0x10)
                        {
                            nemesis_drawgfx_zoomup(bitmap, Mame.Machine.gfx[char_type],
                                    (uint)code,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0, zoom);
                        }
                    } /* if sprite */
                } /* for loop */
            } /* priority */
        }
        /* This is a bit slow, but it works. I'll speed it up later */
        static void nemesis_drawgfx_zoomup(Mame.osd_bitmap dest, Mame.GfxElement gfx,
                uint code, uint color, int flipx, int flipy, int sx, int sy,
                Mame.rectangle clip, int transparency, int transparent_color, int scale)
        {
            int ex, ey, y, start, dy;
            _BytePtr sd;
            _BytePtr bm;
            int col;
            Mame.rectangle myclip = new Mame.rectangle(); ;

            int dda_x = 0;
            int dda_y = 0;
            int ex_count;
            int ey_count;
            int real_x;
            int ysize;
            int xsize;
            UShortSubArray paldata;	/* ASG 980209 */
            int transmask;

            if (gfx == null) return;

            code %= gfx.total_elements;
            color %= (uint)gfx.total_colors;

            transmask = 1 << transparent_color;

            if ((gfx.pen_usage[code] & ~transmask) == 0)
                /* character is totally transparent, no need to draw */
                return;


            if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
            {
                int temp;

                temp = sx;
                sx = sy;
                sy = temp;

                temp = flipx;
                flipx = flipy;
                flipy = temp;

                if (clip != null)
                {
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_x;
                    myclip.min_x = clip.min_y;
                    myclip.min_y = temp;
                    temp = clip.max_x;
                    myclip.max_x = clip.max_y;
                    myclip.max_y = temp;
                    clip = myclip;
                }
            }
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
            {
                sx = dest.width - gfx.width - sx;

                if (clip != null)
                {
                    int temp;

                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_x;
                    myclip.min_x = dest.width - 1 - clip.max_x;
                    myclip.max_x = dest.width - 1 - temp;
                    myclip.min_y = clip.min_y;
                    myclip.max_y = clip.max_y;
                    clip = myclip;
                }
            }
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
            {
                sy = dest.height - gfx.height - sy;

                if (clip != null)
                {
                    int temp;

                    myclip.min_x = clip.min_x;
                    myclip.max_x = clip.max_x;
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_y;
                    myclip.min_y = dest.height - 1 - clip.max_y;
                    myclip.max_y = dest.height - 1 - temp;
                    clip = myclip;
                }
            }


            /* check bounds */
            xsize = gfx.width;
            ysize = gfx.height;
            /* Clipping currently done in code loop */
            ex = sx + xsize - 1;
            ey = sy + ysize - 1;
            /*	if (ex >= dest.width) ex = dest.width-1;
                if (clip && ex > clip.max_x) ex = clip.max_x;
                if (sx > ex) return;
                if (ey >= dest.height) tey = dest.height-1;
                if (clip && ey > clip.max_y) ey = clip.max_y;
                if (sy > ey) return;
            */
            /* start = code * gfx.height; */
            if (flipy != 0)	/* Y flip */
            {
                start = (int)(code * gfx.height + gfx.height - 1);
                dy = -1;
            }
            else		/* normal */
            {
                start = (int)(code * gfx.height);
                dy = 1;
            }



            paldata = new UShortSubArray(gfx.colortable, (int)(gfx.color_granularity * color));

            if (flipx != 0)	/* X flip */
            {
                if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                    y = sy + ysize - 1;
                else
                    y = sy;
                dda_y = 0x80;
                ey_count = sy;
                do
                {
                    if (y >= clip.min_y && y <= clip.max_y)
                    {
                        if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
                        {
                            bm = new _BytePtr(dest.line[y], sx + xsize - 1);
                            real_x = sx + xsize - 1;
                        }
                        else
                        {
                            bm = new _BytePtr(dest.line[y], sx);
                            real_x = sx;
                        }
                        sd = new _BytePtr(gfx.gfxdata, start * gfx.line_modulo + xsize - 1);
                        dda_x = 0x80;
                        ex_count = sx;
                        col = sd[0];
                        do
                        {
                            if ((real_x <= clip.max_x) && (real_x >= clip.min_x))
                                if (col != transparent_color) bm[0] = (byte)paldata[col];
                            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
                            {
                                bm.offset--;
                                real_x--;
                            }
                            else
                            {
                                bm.offset++;
                                real_x++;
                            }
                            dda_x -= scale;
                            if (dda_x <= 0)
                            {
                                dda_x += 0x80;
                                sd.offset--;
                                ex_count++;
                                col = sd[0];
                            }
                        } while (ex_count <= ex);
                    }
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                        y--;
                    else
                        y++;
                    dda_y -= scale;
                    if (dda_y <= 0)
                    {
                        dda_y += 0x80;
                        start += dy;
                        ey_count++;
                    }

                } while (ey_count <= ey);
            }
            else		/* normal */
            {
                if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                    y = sy + ysize - 1;
                else
                    y = sy;
                dda_y = 0x80;
                ey_count = sy;
                do
                {
                    if (y >= clip.min_y && y <= clip.max_y)
                    {
                        if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
                        {
                            bm = new _BytePtr(dest.line[y], sx + xsize - 1);
                            real_x = sx + xsize - 1;
                        }
                        else
                        {
                            bm = new _BytePtr(dest.line[y], sx);
                            real_x = sx;
                        }
                        sd = new _BytePtr(gfx.gfxdata, start * gfx.line_modulo);
                        dda_x = 0x80;
                        ex_count = sx;
                        col = sd[0];
                        do
                        {
                            if ((real_x <= clip.max_x) && (real_x >= clip.min_x))
                                if (col != transparent_color) bm[0] = (byte)paldata[col];
                            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
                            {
                                bm.offset--;
                                real_x--;
                            }
                            else
                            {
                                bm.offset++;
                                real_x++;
                            }
                            dda_x -= scale;
                            if (dda_x <= 0)
                            {
                                dda_x += 0x80;
                                sd.offset++;
                                ex_count++;
                                col = sd[0];
                            }
                        } while (ex_count <= ex);
                    }
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                        y--;
                    else
                        y++;
                    dda_y -= scale;
                    if (dda_y <= 0)
                    {
                        dda_y += 0x80;
                        start += dy;
                        ey_count++;
                    }

                } while (ey_count <= ey);
            }
        }

        /* This is a bit slow, but it works. I'll speed it up later */
        static void nemesis_drawgfx_zoomdown(Mame.osd_bitmap dest, Mame.GfxElement gfx,
                uint code, uint color, int flipx, int flipy, int sx, int sy,
                Mame.rectangle clip, int transparency, int transparent_color, int scale)
{
	int ex,ey,y,start,dy;
	_BytePtr sd;
	_BytePtr bm;
	int col;
    Mame.rectangle myclip = new Mame.rectangle();

	int dda_x=0;
	int dda_y=0;
	int ex_count;
	int ey_count;
	int real_x;
	int ysize;
	int xsize;
	int transmask;
	UShortSubArray paldata;	/* ASG 980209 */

	if (gfx==null) return;

	code %= gfx.total_elements;
	color %= (uint)gfx.total_colors;

	transmask = 1 << transparent_color;
	if ((gfx.pen_usage[code] & ~transmask) == 0)
		/* character is totally transparent, no need to draw */
		return;


	if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY)!=0)
	{
		int temp;

		temp = sx;
		sx = sy;
		sy = temp;

		temp = flipx;
		flipx = flipy;
		flipy = temp;

		if (clip!=null)
		{
			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_x;
			myclip.min_x = clip.min_y;
			myclip.min_y = temp;
			temp = clip.max_x;
			myclip.max_x = clip.max_y;
			myclip.max_y = temp;
			clip = myclip;
		}
	}
	if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
	{
		sx = dest.width - gfx.width - sx;

		if (clip!=null)
		{
			int temp;


			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_x;
			myclip.min_x = dest.width-1 - clip.max_x;
			myclip.max_x = dest.width-1 - temp;
			myclip.min_y = clip.min_y;
			myclip.max_y = clip.max_y;
			clip = myclip;
		}
	}
	if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
	{
		sy = dest.height - gfx.height - sy;

		if (clip!=null)
		{
			int temp;

			myclip.min_x = clip.min_x;
			myclip.max_x = clip.max_x;
			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_y;
			myclip.min_y = dest.height-1 - clip.max_y;
			myclip.max_y = dest.height-1 - temp;
			clip = myclip;
		}
	}


	/* check bounds */
	xsize=gfx.width;
	ysize=gfx.height;
	ex = sx + xsize -1;
	if (ex >= dest.width) ex = dest.width-1;
	if (clip !=null&& ex > clip.max_x) ex = clip.max_x;
	if (sx > ex) return;
	ey = sy + ysize -1;
	if (ey >= dest.height) ey = dest.height-1;
	if (clip !=null&& ey > clip.max_y) ey = clip.max_y;
	if (sy > ey) return;

	/* start = code * gfx.height; */
	if (flipy!=0)	/* Y flip */
	{
		start = (int)(code * gfx.height + gfx.height-1);
		dy = -1;
	}
	else		/* normal */
	{
		start = (int)(code * gfx.height);
		dy = 1;
	}



	paldata = new UShortSubArray(gfx.colortable, (int)(gfx.color_granularity * color));

	if (flipx!=0)	/* X flip */
	{
		if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
			y=sy + ysize -1;
		else
			y=sy;
		dda_y=0-scale/2;
		for(ey_count=0;ey_count<ysize;ey_count++)
		{
			if(dda_y<0) dda_y+=0x80;
			if(dda_y>=0)
			{
				dda_y-=scale;
				if(y>=clip.min_y && y<=clip.max_y)
				{
					if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
					{
						bm  = new _BytePtr(dest.line[y], sx + xsize -1);
						real_x=sx + xsize -1;
					} else {
						bm  = new _BytePtr(dest.line[y], sx);
						real_x=sx;
					}
					sd = new _BytePtr(gfx.gfxdata,  start * gfx.line_modulo + xsize -1);
					dda_x=0-scale/2;
					for(ex_count=0;ex_count<xsize;ex_count++)
					{
						if(dda_x<0) dda_x+=0x80;
						if(dda_x>=0)
						{
							dda_x-=scale;
							if ((real_x<=clip.max_x) && (real_x>=clip.min_x))
							{
								col = sd[0];
								if (col != transparent_color) bm[0] = (byte)paldata[col];
							}
							if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
							{
								bm.offset--;
								real_x--;
							} else {
								bm.offset++;
								real_x++;
							}
						}
						sd.offset--;
					}
				}
				if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
					y--;
				else
					y++;
			}
			start+=dy;
		}

	}
	else		/* normal */
	{
		if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
			y=sy + ysize -1;
		else
			y=sy;
		dda_y=0-scale/2;
		for(ey_count=0;ey_count<ysize;ey_count++)
		{
			if(dda_y<0) dda_y+=0x80;
			if(dda_y>=0)
			{
				dda_y-=scale;
				if(y>=clip.min_y && y<=clip.max_y)
				{
					if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
					{
						bm  = new _BytePtr(dest.line[y], sx + xsize -1);
						real_x=sx + xsize -1;
					} else {
						bm  = new _BytePtr(dest.line[y], sx);
						real_x=sx;
					}
					sd = new _BytePtr(gfx.gfxdata,  start * gfx.line_modulo);
					dda_x=0-scale/2;
					for(ex_count=0;ex_count<xsize;ex_count++)
					{
						if(dda_x<0) dda_x+=0x80;
						if(dda_x>=0)
						{
							dda_x-=scale;
							if ((real_x<=clip.max_x) && (real_x>=clip.min_x))
							{
								col = sd[0];
								if (col != transparent_color) bm[0] = (byte)paldata[col];
							}
							if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
							{
								bm.offset--;
								real_x--;
							} else {
								bm.offset++;
								real_x++;
							}
						}
						sd.offset++;
					}
				}
				if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
					y--;
				else
					y++;
			}
			start+=dy;
		}

	}
}


        public static void nemesis_vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int offs;
            int[] xscroll = new int[256], xscroll2 = new int[256];
            int yscroll;

            setup_palette();

            /* Render backgrounds */
            setup_backgrounds();

            /* screen flash */
            Mame.fillbitmap(bitmap, Mame.Machine.pens[Mame.paletteram.READ_WORD(0x00) & 0x7ff], Mame.Machine.drv.visible_area);

            /* Copy the background bitmap */
            yscroll = -(nemesis_yscroll.READ_WORD(0x300) & 0xff);	/* used on nemesis level 2 */
            for (offs = 0; offs < 256; offs++)
            {
                xscroll2[offs] = -((nemesis_xscroll2.READ_WORD(2 * offs) & 0xff) +
                        ((nemesis_xscroll2.READ_WORD(0x200 + 2 * offs) & 1) << 8));
            }
            Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 256, xscroll2, 1, new int[] { yscroll }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

            /* Do the foreground */
            for (offs = 0; offs < 256; offs++)
            {
                xscroll[offs] = -((nemesis_xscroll1.READ_WORD(2 * offs) & 0xff) +
                        ((nemesis_xscroll1.READ_WORD(0x200 + 2 * offs) & 1) << 8));
            }
            Mame.copyscrollbitmap(bitmap, tmpbitmap2, 256, xscroll, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

            draw_sprites(bitmap);

            Mame.copyscrollbitmap(bitmap, tmpbitmap3, 256, xscroll2, 1, new int[] { yscroll }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);
            Mame.copyscrollbitmap(bitmap, tmpbitmap4, 256, xscroll, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

            for (offs = 0; offs < 2048; offs++)
            {
                if (char_dirty[offs] == 2)
                    char_dirty[offs] = 0;
            }
        }

        class machine_driver_nemesis : Mame.MachineDriver
        {
            public machine_driver_nemesis()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 14318180 / 2, readmem, writemem, null, null, nemesis_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 4, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_nemesis.gfxdecodeinfo;
                total_colors = 2048;
                color_table_len = 2048;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                //sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
                //throw new Exception();
            }
            public override void init_machine()
            {
                irq_on = 0;
                irq1_on = 0;
                irq2_on = 0;
                irq4_on = 0;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                return nemesis_vh_start();
            }
            public override void vh_stop()
            {
                nemesis_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                nemesis_vh_update(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //null
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_nemesis()
        {
            ROM_START("nemesis");
            ROM_REGION(0x40000, Mame.REGION_CPU1);    /* 4 * 64k for code and rom */
            ROM_LOAD_EVEN("12a_01.bin", 0x00000, 0x8000, 0x35ff1aaa);
            ROM_LOAD_ODD("12c_05.bin", 0x00000, 0x8000, 0x23155faa);
            ROM_LOAD_EVEN("13a_02.bin", 0x10000, 0x8000, 0xac0cf163);
            ROM_LOAD_ODD("13c_06.bin", 0x10000, 0x8000, 0x023f22a9);
            ROM_LOAD_EVEN("14a_03.bin", 0x20000, 0x8000, 0x8cefb25f);
            ROM_LOAD_ODD("14c_07.bin", 0x20000, 0x8000, 0xd50b82cb);
            ROM_LOAD_EVEN("15a_04.bin", 0x30000, 0x8000, 0x9ca75592);
            ROM_LOAD_ODD("15c_08.bin", 0x30000, 0x8000, 0x03c0b7f5);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for sound */
            ROM_LOAD("09c_snd.bin", 0x00000, 0x4000, 0x26bf9636);

            ROM_REGION(0x0200, Mame.REGION_SOUND1);    /* 2x 256 byte for 0005289 wavetable data */
            ROM_LOAD("400-a01.fse", 0x00000, 0x0100, 0x5827b1e8);
            ROM_LOAD("400-a02.fse", 0x00100, 0x0100, 0x2f44f970);
            return ROM_END;
        }
        static void GX400_COINAGE_DIP()
        {
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x02, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x01, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0x0f, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x07, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0x0d, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0b, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x0a, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x09, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x20, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x50, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x10, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x70, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0xd0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xb0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0xa0, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, "Disabled");
        }
        Mame.InputPortTiny[] input_ports_nemesis()
        {
            INPUT_PORTS_START("nemesis");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* TEST */
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x02, "Version");
            PORT_DIPSETTING(0x02, "Normal");
            PORT_DIPSETTING(0x00, "Vs");
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN); ;

            PORT_START();	/* DSW0 */
            GX400_COINAGE_DIP();

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x04, DEF_STR(Cocktail));
            PORT_DIPNAME(0x18, 0x18, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x18, "50k and every 100k");
            PORT_DIPSETTING(0x10, "30k");
            PORT_DIPSETTING(0x08, "50k");
            PORT_DIPSETTING(0x00, "100k");
            PORT_DIPNAME(0x60, 0x60, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x60, "Easy");
            PORT_DIPSETTING(0x40, "Normal");
            PORT_DIPSETTING(0x20, "Difficult");
            PORT_DIPSETTING(0x00, "Very Difficult");
            PORT_DIPNAME(0x80, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            return INPUT_PORTS_END;
        }
        public driver_nemesis()
        {
            drv = new machine_driver_nemesis();
            year = "1985";
            name = "nemesis";
            description = "Nemesis (hacked?)";
            manufacturer = "Konami";
            flags = Mame.ROT0;
            input_ports = input_ports_nemesis();
            rom = rom_nemesis();
            drv.HasNVRAMhandler = false;
        }
    }
}
