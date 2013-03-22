using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_xevious : Mame.GameDriver
    {
        static _BytePtr xevious_fg_videoram = new _BytePtr(1);
        static _BytePtr xevious_bg_videoram = new _BytePtr(1);
        static _BytePtr xevious_fg_colorram = new _BytePtr(1);
        static _BytePtr xevious_bg_colorram = new _BytePtr(1);
        static Mame.tilemap fg_tilemap, bg_tilemap;
        static int flipscreen;

        static _BytePtr xevious_sharedram = new _BytePtr(1);
        static byte interrupt_enable_1, interrupt_enable_2, interrupt_enable_3;
        static _BytePtr rom2a, rom2b, rom2c;
        static int[] xevious_bs = new int[2];
        static object nmi_timer;
        static byte[] namco_key = { 5, 5, 5, 5, 7, 6, 7, 6, 3, 3, 4, 4, 1, 2, 0, 8 };

        static Mame.MemoryReadAddress[] readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x6800, 0x6807, xevious_dsw_r ),
	new Mame.MemoryReadAddress( 0x7000, 0x700f, xevious_customio_data_r ),
	new Mame.MemoryReadAddress( 0x7100, 0x7100, xevious_customio_r ),
	new Mame.MemoryReadAddress( 0x7800, 0xcfff, xevious_sharedram_r ),
	new Mame.MemoryReadAddress( 0xf000, 0xffff, xevious_bb_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x6800, 0x6807, xevious_dsw_r ),
	new Mame.MemoryReadAddress( 0x7800, 0xcfff, xevious_sharedram_r ),
	new Mame.MemoryReadAddress( 0xf000, 0xffff, xevious_bb_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu3 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x7800, 0xcfff, xevious_sharedram_r),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6820, 0x6820, xevious_interrupt_enable_1_w ),
	new Mame.MemoryWriteAddress( 0x6821, 0x6821, xevious_interrupt_enable_2_w ),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, xevious_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x6823, 0x6823, xevious_halt_w ),			/* reset controll */
	new Mame.MemoryWriteAddress( 0x6830, 0x683f, Mame.MWA_NOP ),				/* watch dock reset */
	new Mame.MemoryWriteAddress( 0x7000, 0x700f, xevious_customio_data_w ),
	new Mame.MemoryWriteAddress( 0x7100, 0x7100, xevious_customio_w ),
	new Mame.MemoryWriteAddress( 0x7800, 0xafff, xevious_sharedram_w, xevious_sharedram ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb7ff, xevious_fg_colorram_w, xevious_fg_colorram ),
	new Mame.MemoryWriteAddress( 0xb800, 0xbfff, xevious_bg_colorram_w, xevious_bg_colorram ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, xevious_fg_videoram_w, xevious_fg_videoram ),
	new Mame.MemoryWriteAddress( 0xc800, 0xcfff, xevious_bg_videoram_w, xevious_bg_videoram ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd07f, xevious_vh_latch_w ), /* ?? */
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, xevious_bs_w ),
	new Mame.MemoryWriteAddress( 0x8780, 0x87ff, Mame.MWA_RAM, Generic.spriteram_2 ),	/* here only */
	new Mame.MemoryWriteAddress( 0x9780, 0x97ff, Mame.MWA_RAM, Generic.spriteram_3 ),	/* to initialize */
	new Mame.MemoryWriteAddress( 0xa780, 0xa7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),	/* the pointers */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress(0x0000, 0x1fff,Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0x6830, 0x683f,Mame.MWA_NOP ),				/* watch dog reset */
	new Mame.MemoryWriteAddress(0x7800, 0xafff, xevious_sharedram_w ),
	new Mame.MemoryWriteAddress(0xb000, 0xb7ff, xevious_fg_colorram_w ),
	new Mame.MemoryWriteAddress(0xb800, 0xbfff, xevious_bg_colorram_w ),
	new Mame.MemoryWriteAddress(0xc000, 0xc7ff, xevious_fg_videoram_w ),
	new Mame.MemoryWriteAddress(0xc800, 0xcfff, xevious_bg_videoram_w ),
	new Mame.MemoryWriteAddress(0xd000, 0xd07f, xevious_vh_latch_w ), /* ?? */
	new Mame.MemoryWriteAddress(0xf000, 0xffff, xevious_bs_w ),
	new Mame.MemoryWriteAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu3 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6800, 0x681f, Namco.pengo_sound_w, Namco.namco_soundregs),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, xevious_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x7800, 0xcfff, xevious_sharedram_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        /* foreground characters */
        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            1,	/* 1 bit per pixel */
            new uint[] { 0 },
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        /* background tiles */
        static Mame.GfxLayout bgcharlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 0, 512 * 8 * 8 },
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        /* sprite set #1 */
        static Mame.GfxLayout spritelayout1 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites */
            3,	/* 3 bits per pixel */
            new uint[] { 128 * 64 * 8 + 4, 0, 4 },
            new uint[]{ 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 128 consecutive bytes */
        );
        /* sprite set #2 */
        static Mame.GfxLayout spritelayout2 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 128 * 64 * 8, 128 * 64 * 8 + 4 },
            new uint[]{ 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 128 consecutive bytes */
        );
        /* sprite set #3 */
        static Mame.GfxLayout spritelayout3 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            64,	/* 64 sprites */
            3,	/* 3 bits per pixel (one is always 0) */
            new uint[] { 64 * 64 * 8, 0, 4 },
            new uint[]{ 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, charlayout, 128*4+64*8,  64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, bgcharlayout,        0, 128 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x0000, spritelayout1,   128*4,  64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x2000, spritelayout2,   128*4,  64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x6000, spritelayout3,   128*4,  64 ),
};



        static Namco_interface namco_interface =
        new Namco_interface(
            3072000 / 32,	/* sample rate */
            3,			/* number of voices */
            100,		/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );

        static string[] xevious_sample_names =
{
	"*xevious",
	"explo1.wav",	/* ground target explosion */
	"explo2.wav",	/* Solvalou explosion */
	null	/* end of array */
};

        static Mame.Samplesinterface samples_interface =
        new Mame.Samplesinterface(
            1,	/* one channel */
            80,	/* volume */
            xevious_sample_names
        );


        static void xevious_fg_videoram_w(int offset, int data)
        {
            if (xevious_fg_videoram[offset] != data)
            {
                xevious_fg_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(fg_tilemap, offset % 64, offset / 64);
            }
        }
        static void xevious_fg_colorram_w(int offset, int data)
        {
            if (xevious_fg_colorram[offset] != data)
            {
                xevious_fg_colorram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(fg_tilemap, offset % 64, offset / 64);
            }
        }
        static void xevious_bg_videoram_w(int offset, int data)
        {
            if (xevious_bg_videoram[offset] != data)
            {
                xevious_bg_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, offset % 64, offset / 64);
            }
        }
        static void xevious_bg_colorram_w(int offset, int data)
        {
            if (xevious_bg_colorram[offset] != data)
            {
                xevious_bg_colorram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, offset % 64, offset / 64);
            }
        }
        static void xevious_vh_latch_w(int offset, int data)
        {
            int reg;

            data = data + ((offset & 0x01) << 8);   /* A0 . D8 */
            reg = (offset & 0xf0) >> 4;

            switch (reg)
            {
                case 0:
                    if (flipscreen != 0)
                        Mame.tilemap_set_scrollx(bg_tilemap, 0, data - 312);
                    else
                        Mame.tilemap_set_scrollx(bg_tilemap, 0, data + 20);
                    break;
                case 1:
                    Mame.tilemap_set_scrollx(fg_tilemap, 0, data + 32);
                    break;
                case 2:
                    Mame.tilemap_set_scrolly(bg_tilemap, 0, data + 16);
                    break;
                case 3:
                    Mame.tilemap_set_scrolly(fg_tilemap, 0, data + 18);
                    break;
                case 7:     /* DISPLAY XY FLIP ?? */
                    flipscreen = data & 1;
                    Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
                    break;
                default:
                    Mame.printf("CRTC WRITE REG: %x  Data: %03x\n", reg, data);
                    break;
            }
        }





        static int leftcoininserted;
        static int rightcoininserted;
        static int auxcoininserted;
        static int customio_command;
        static int mode, credits;
        static int auxcoinpercred, auxcredpercoin;
        static int leftcoinpercred, leftcredpercoin;
        static int rightcoinpercred, rightcredpercoin;
        static byte[] customio = new byte[16];

        /* emulation for schematic 9B */
        static void xevious_bs_w(int offset, int data)
        {
            xevious_bs[offset & 0x01] = data;
        }
        static int xevious_bb_r(int offset)
        {
            int adr_2b, adr_2c;
            int dat1, dat2;


            /* get BS to 12 bit data from 2A,2B */
            adr_2b = ((xevious_bs[1] & 0x7e) << 6) | ((xevious_bs[0] & 0xfe) >> 1);
            if ((adr_2b & 1) != 0)
            {
                /* high bits select */
                dat1 = ((rom2a[adr_2b >> 1] & 0xf0) << 4) | rom2b[adr_2b];
            }
            else
            {
                /* low bits select */
                dat1 = ((rom2a[adr_2b >> 1] & 0x0f) << 8) | rom2b[adr_2b];
            }
            adr_2c = (dat1 & 0x1ff) << 2;
            if ((offset & 0x01) != 0)
                adr_2c += (1 << 11);	/* signal 4H to A11 */
            if (((xevious_bs[0] & 1) ^ ((dat1 >> 10) & 1)) != 0)
                adr_2c |= 1;
            if (((xevious_bs[1] & 1) ^ ((dat1 >> 9) & 1)) != 0)
                adr_2c |= 2;
            if ((offset & 0x01) != 0)
            {
                /* return BB1 */
                dat2 = rom2c[adr_2c];
            }
            else
            {
                /* return BB0 */
                dat2 = rom2c[adr_2c];
                /* swap bit 6 & 7 */
                dat2 = (dat2 & 0x3f) | ((dat2 & 0x80) >> 1) | ((dat2 & 0x40) << 1);
                /* flip x & y */
                dat2 ^= (dat1 >> 4) & 0x40;
                dat2 ^= (dat1 >> 2) & 0x80;
            }
            return dat2;
        }
        static int xevious_sharedram_r(int offset)
        {
            return xevious_sharedram[offset];
        }
        static void xevious_sharedram_w(int offset, int data)
        {
            xevious_sharedram[offset] = (byte)data;
        }
        static int xevious_dsw_r(int offset)
        {
            int bit0, bit1;

            bit0 = (Mame.input_port_0_r(0) >> offset) & 1;
            bit1 = (Mame.input_port_1_r(0) >> offset) & 1;

            return bit0 | (bit1 << 1);
        }
        static void xevious_customio_data_w(int offset, int data)
        {
            customio[offset] = (byte)data;

            //if (errorlog) fprintf(errorlog,"%04x: custom IO offset %02x data %02x\n",cpu_get_pc(),offset,data);

            switch (customio_command)
            {
                case 0xa1:
                    if (offset == 0)
                    {
                        if (data == 0x05)
                            mode = 1;	/* go into switch mode */
                        else	/* go into credit mode */
                        {
                            credits = 0;	/* this is a good time to reset the credits counter */
                            mode = 0;
                        }
                    }
                    else if (offset == 7)
                    {
                        auxcoinpercred = customio[1];
                        auxcredpercoin = customio[2];
                        leftcoinpercred = customio[3];
                        leftcredpercoin = customio[4];
                        rightcoinpercred = customio[5];
                        rightcredpercoin = customio[6];
                    }
                    break;

                case 0x68:
                    if (offset == 6)
                    {
                        /* it is not known how the parameters control the explosion. */
                        /* We just use samples. */
                        if (memcmp(customio, new byte[] { 0x40, 0x40, 0x40, 0x01, 0xff, 0x00, 0x20 }, 7) == 0)
                            Mame.sample_start(0, 0, false);
                        else if (memcmp(customio, new byte[] { 0x30, 0x40, 0x00, 0x02, 0xdf, 0x00, 0x10 }, 7) == 0)
                            Mame.sample_start(0, 1, false);
                    }
                    break;
            }
        }
        static int memcmp(byte[] b1, byte[] b2, int l)
        {
            for (int i = 0; i < l; i++)
            {
                if (b1[i] < b2[i]) return -1;
                if (b1[i] > b2[i]) return 1;
            }
            return 0;
        }
        static int xevious_customio_data_r(int offset)
        {
            //if (errorlog && customio_command != 0x71) fprintf(errorlog,"%04x: custom IO read offset %02x\n",cpu_get_pc(),offset);

            switch (customio_command)
            {
                case 0x71:	/* read input */
                case 0xb1:	/* only issued after 0xe1 (go into credit mode) */
                    if (offset == 0)
                    {
                        if (mode != 0)	/* switch mode */
                        {
                            /* bit 7 is the service switch */
                            return Mame.readinputport(4);
                        }
                        else	/* credits mode: return number of credits in BCD format */
                        {
                            int _in;


                            _in = Mame.readinputport(4);

                            /* check if the user inserted a coin */
                            if (leftcoinpercred > 0)
                            {
                                if ((_in & 0x10) == 0 && credits < 99)
                                {
                                    leftcoininserted++;
                                    if (leftcoininserted >= leftcoinpercred)
                                    {
                                        credits += leftcredpercoin;
                                        leftcoininserted = 0;
                                    }
                                }
                                if ((_in & 0x20) == 0 && credits < 99)
                                {
                                    rightcoininserted++;
                                    if (rightcoininserted >= rightcoinpercred)
                                    {
                                        credits += rightcredpercoin;
                                        rightcoininserted = 0;
                                    }
                                }
                                if ((_in & 0x40) == 0 && credits < 99)
                                {
                                    auxcoininserted++;
                                    if (auxcoininserted >= auxcoinpercred)
                                    {
                                        credits += auxcredpercoin;
                                        auxcoininserted = 0;
                                    }
                                }
                            }
                            else credits = 2;


                            /* check for 1 player start button */
                            if ((_in & 0x04) == 0)
                                if (credits >= 1) credits--;

                            /* check for 2 players start button */
                            if ((_in & 0x08) == 0)
                                if (credits >= 2) credits -= 2;

                            return (credits / 10) * 16 + credits % 10;
                        }
                    }
                    else if (offset == 1)
                    {
                        int _in;


                        _in = Mame.readinputport(2);	/* player 1 input */
                        if (mode == 0)	/* convert joystick input only when in credits mode */
                            _in = namco_key[_in & 0x0f] | (_in & 0xf0);
                        return _in;
                    }
                    else if (offset == 2)
                    {
                        int _in;


                        _in = Mame.readinputport(3);	/* player 2 input */
                        if (mode == 0)	/* convert joystick input only when in credits mode */
                            _in = namco_key[_in & 0x0f] | (_in & 0xf0);
                        return _in;
                    }

                    break;

                case 0x74:		/* protect data read ? */
                    if (offset == 3)
                    {
                        if (customio[0] == 0x80 || customio[0] == 0x10)
                            return 0x05;	/* 1st check */
                        else
                            return 0x95;  /* 2nd check */
                    }
                    else return 0x00;
                    break;
            }

            return -1;
        }
        static int xevious_customio_r(int offset)
        {
            return customio_command;
        }
        static void xevious_nmi_generate(int param)
        {
            Mame.cpu_cause_interrupt(0, Mame.cpu_Z80.Z80_NMI_INT);
        }
        static void xevious_customio_w(int offset, int data)
        {
            //if (errorlog && data != 0x10 && data != 0x71) fprintf(errorlog,"%04x: custom IO command %02x\n",cpu_get_pc(),data);

            customio_command = data;

            switch (data)
            {
                case 0x10:
                    if (nmi_timer != null) Mame.Timer.timer_remove(nmi_timer);
                    nmi_timer = null;
                    return; /* nop */
            }

            nmi_timer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_USEC(50), 0, xevious_nmi_generate);
        }
        static void xevious_halt_w(int offset, int data)
        {
            if ((data & 1) != 0)
            {
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
                Mame.cpu_set_reset_line(2, Mame.CLEAR_LINE);
            }
            else
            {
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
            }
        }
        static void xevious_interrupt_enable_1_w(int offset, int data)
        {
            interrupt_enable_1 = (byte)(data & 1);
        }
        static int xevious_interrupt_1()
        {
            if (interrupt_enable_1 != 0) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void xevious_interrupt_enable_2_w(int offset, int data)
        {
            interrupt_enable_2 = (byte)(data & 1);
        }
        static int xevious_interrupt_2()
        {
            if (interrupt_enable_2 != 0) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void xevious_interrupt_enable_3_w(int offset, int data)
        {
            interrupt_enable_3 = (data & 1) == 0 ? (byte)1 : (byte)0;
        }
        static int xevious_interrupt_3()
        {
            if (interrupt_enable_3 != 0) return Mame.nmi_interrupt();
            else return Mame.ignore_interrupt();
        }

        class machine_driver_xevious : Mame.MachineDriver
        {
            public machine_driver_xevious()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu1, writemem_cpu1, null, null, xevious_interrupt_1, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu2, writemem_cpu2, null, null, xevious_interrupt_2, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu3, writemem_cpu3, null, null, xevious_interrupt_3, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_xevious.gfxdecodeinfo;
                total_colors = 12801;
                color_table_len = 128 * 4 + 64 * 8 + 64 * 2;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
                //xxxsound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
            }
            public override void init_machine()
            {
                rom2a = Mame.memory_region(Mame.REGION_GFX4);
                rom2b = new _BytePtr(Mame.memory_region(Mame.REGION_GFX4), 0x1000);
                rom2c = new _BytePtr(Mame.memory_region(Mame.REGION_GFX4), 0x3000);

                nmi_timer = null;

                xevious_halt_w(0, 0);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < 128; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    /* green component */
                    bit0 = (color_prom[cpi + 256] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 256] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 256] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 256] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    /* blue component */
                    bit0 = (color_prom[cpi + 2 * 256] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 2 * 256] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 2 * 256] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 2 * 256] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                /* color 0x80 is used by sprites to mark transparency */
                palette[pi++] = 0;
                palette[pi++] = 0;
                palette[pi++] = 0;

                cpi += 128;  /* the bottom part of the PROM is unused */
                cpi += 2 * 256;
                /* color_prom now points to the beginning of the lookup table */

                /* background tiles */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    COLOR(colortable, 1, i, (color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(1)] & 0x0f) << 4));

                    cpi++;
                }
                cpi += (uint)TOTAL_COLORS(1);

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                {
                    int c = (color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(2)] & 0x0f) << 4);

                    if ((c & 0x80) != 0) COLOR(colortable, 2, i, c & 0x7f);
                    else COLOR(colortable, 2, i, 0x80); /* transparent */

                    cpi++;
                }
                cpi += (uint)TOTAL_COLORS(2);

                /* foreground characters */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                {
                    if (i % 2 == 0) COLOR(colortable, 0, i, 0x80);  /* transparent */
                    else COLOR(colortable, 0, i, i / 2);
                }
            }
            static void get_fg_tile_info(int col, int row)
            {
                int tile_index = 64 * row + col;
                byte attr = xevious_fg_colorram[tile_index];
                Mame.SET_TILE_INFO(0, xevious_fg_videoram[tile_index],
                        ((attr & 0x03) << 4) | ((attr & 0x3c) >> 2));
                Mame.tile_info.flags = (byte)Mame.TILE_FLIPYX((attr & 0xc0) >> 6);
            }

            static void get_bg_tile_info(int col, int row)
            {
                int tile_index = 64 * row + col;
                byte code = xevious_bg_videoram[tile_index];
                byte attr = xevious_bg_colorram[tile_index];
                Mame.SET_TILE_INFO(1, code + ((attr & 0x01) << 8),
                        ((attr & 0x3c) >> 2) | ((code & 0x80) >> 3) | ((attr & 0x03) << 5));
                Mame.tile_info.flags = (byte)Mame.TILE_FLIPYX((attr & 0xc0) >> 6);
            }

            public override int vh_start()
            {
                fg_tilemap = Mame.tilemap_create(
         get_fg_tile_info,
         Mame.TILEMAP_TRANSPARENT,
         8, 8,
         64, 32
     );
                bg_tilemap = Mame.tilemap_create(
                    get_bg_tile_info,
                    0,
                    8, 8,
                    64, 32
                );

                if (fg_tilemap != null && bg_tilemap != null)
                {
                    fg_tilemap.transparent_pen = 0;

                    return 0;
                }

                return 1;
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            static void draw_sprites(Mame.osd_bitmap bitmap)
            {
                int offs, sx, sy;

                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    if ((Generic.spriteram[offs + 1] & 0x40) == 0)  /* I'm not sure about this one */
                    {
                        int bank, code, color;
                        bool flipx, flipy;

                        if ((Generic.spriteram_3[offs] & 0x80) != 0)
                        {
                            bank = 4;
                            code = Generic.spriteram[offs] & 0x3f;
                        }
                        else
                        {
                            bank = 2 + ((Generic.spriteram[offs] & 0x80) >> 7);
                            code = Generic.spriteram[offs] & 0x7f;
                        }

                        color = Generic.spriteram[offs + 1] & 0x7f;
                        flipx = (Generic.spriteram_3[offs] & 4) != 0;
                        flipy = (Generic.spriteram_3[offs] & 8) != 0;
                        if (flipscreen != 0)
                        {
                            flipx = !flipx;
                            flipy = !flipy;
                        }
                        sx = Generic.spriteram_2[offs + 1] - 40 + 0x100 * (Generic.spriteram_3[offs + 1] & 1);
                        sy = 28 * 8 - Generic.spriteram_2[offs] - 1;
                        if ((Generic.spriteram_3[offs] & 2) != 0)  /* double height (?) */
                        {
                            if ((Generic.spriteram_3[offs] & 1) != 0)  /* double width, double height */
                            {
                                code &= 0x7c;
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                        (uint)code + 3, (uint)color, flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy - 16 : sy,
                                       Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                        (uint)code + 1, (uint)color, flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy : sy - 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                            }
                            code &= 0x7d;
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                    (uint)code + 2, (uint)color, flipx, flipy,
                                    flipx ? sx + 16 : sx, flipy ? sy - 16 : sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                    (uint)code, (uint)color, flipx, flipy,
                                    flipx ? sx + 16 : sx, flipy ? sy : sy - 16,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                        }
                        else if ((Generic.spriteram_3[offs] & 1) != 0) /* double width */
                        {
                            code &= 0x7e;
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                    (uint)code, (uint)color, flipx, flipy,
                                    flipx ? sx + 16 : sx, flipy ? sy - 16 : sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                    (uint)code + 1, (uint)color, flipx, flipy,
                                    flipx ? sx : sx + 16, flipy ? sy - 16 : sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                        }
                        else    /* normal */
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                                    (uint)code, (uint)color, flipx, flipy, sx, sy,
                                   Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0x80);
                        }
                    }
                }
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.tilemap_update(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);

                Mame.tilemap_draw(bitmap, bg_tilemap, 0);
                draw_sprites(bitmap);
                Mame.tilemap_draw(bitmap, fg_tilemap, 0);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_xevious()
        {
            ROM_START("xevious");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for the first CPU */
            ROM_LOAD("xvi_1.3p", 0x0000, 0x1000, 0x09964dda);
            ROM_LOAD("xvi_2.3m", 0x1000, 0x1000, 0x60ecce84);
            ROM_LOAD("xvi_3.2m", 0x2000, 0x1000, 0x79754b7d);
            ROM_LOAD("xvi_4.2l", 0x3000, 0x1000, 0xc7d4bbf0);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("xvi_5.3f", 0x0000, 0x1000, 0xc85b703f);
            ROM_LOAD("xvi_6.3j", 0x1000, 0x1000, 0xe18cdaad);

            ROM_REGION(0x10000, Mame.REGION_CPU3);	/* 64k for the audio CPU */
            ROM_LOAD("xvi_7.2c", 0x0000, 0x1000, 0xdd35cf1c);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("xvi_12.3b", 0x0000, 0x1000, 0x088c8b26);/* foreground characters */

            ROM_REGION(0x2000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("xvi_13.3c", 0x0000, 0x1000, 0xde60ba25);/* bg pattern B0 */
            ROM_LOAD("xvi_14.3d", 0x1000, 0x1000, 0x535cdbbc);/* bg pattern B1 */

            ROM_REGION(0x8000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("xvi_15.4m", 0x0000, 0x2000, 0xdc2c0ecb);/* sprite set #1, planes 0/1 */
            ROM_LOAD("xvi_18.4r", 0x2000, 0x2000, 0x02417d19);/* sprite set #1, plane 2, set #2, plane 0 */
            ROM_LOAD("xvi_17.4p", 0x4000, 0x2000, 0xdfb587ce);/* sprite set #2, planes 1/2 */
            ROM_LOAD("xvi_16.4n", 0x6000, 0x1000, 0x605ca889);/* sprite set #3, planes 0/1 */
            /* 0xa000-0xafff empty space to decode sprite set #3 as 3 bits per pixel */

            ROM_REGION(0x4000, Mame.REGION_GFX4);	/* background tilemaps */
            ROM_LOAD("xvi_9.2a", 0x0000, 0x1000, 0x57ed9879);
            ROM_LOAD("xvi_10.2b", 0x1000, 0x2000, 0xae3ba9e5);
            ROM_LOAD("xvi_11.2c", 0x3000, 0x1000, 0x31e244dd);

            ROM_REGION(0x0b00, Mame.REGION_PROMS);
            ROM_LOAD("xvi_8bpr.6a", 0x0000, 0x0100, 0x5cc2727f); /* palette red component */
            ROM_LOAD("xvi_9bpr.6d", 0x0100, 0x0100, 0x5c8796cc); /* palette green component */
            ROM_LOAD("xvi10bpr.6e", 0x0200, 0x0100, 0x3cb60975); /* palette blue component */
            ROM_LOAD("xvi_7bpr.4h", 0x0300, 0x0200, 0x22d98032); /* bg tiles lookup table low bits */
            ROM_LOAD("xvi_6bpr.4f", 0x0500, 0x0200, 0x3a7599f0); /* bg tiles lookup table high bits */
            ROM_LOAD("xvi_4bpr.3l", 0x0700, 0x0200, 0xfd8b9d91); /* sprite lookup table low bits */
            ROM_LOAD("xvi_5bpr.3m", 0x0900, 0x0200, 0xbf906d82); /* sprite lookup table high bits */

            ROM_REGION(0x0200, Mame.REGION_SOUND1);	/* sound PROMs */
            ROM_LOAD("xvi_2bpr.7n", 0x0000, 0x0100, 0x550f06bc);
            ROM_LOAD("xvi_1bpr.5n", 0x0100, 0x0100, 0x77245b66);	/* timing - not used */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_xevious()
        {

            INPUT_PORTS_START("xevious");
            PORT_START();	/* DSW0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_DIPNAME(0x02, 0x02, "Flags Award Bonus Life");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x02, DEF_STR("Yes"));
            PORT_DIPNAME(0x0c, 0x0c, "Right Coin");
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_6C"));
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_DIPNAME(0x60, 0x60, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x40, "Easy");
            PORT_DIPSETTING(0x60, "Normal");
            PORT_DIPSETTING(0x20, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x80, 0x80, "Freeze?");
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, "Left Coin");
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            /* TODO: bonus scores are different for 5 lives */
            PORT_DIPNAME(0x1c, 0x1c, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x18, "10K 40K 40K");
            PORT_DIPSETTING(0x14, "10K 50K 50K");
            PORT_DIPSETTING(0x10, "20K 50K 50K");
            PORT_DIPSETTING(0x0c, "20K 70K 70K");
            PORT_DIPSETTING(0x08, "20K 80K 80K");
            PORT_DIPSETTING(0x1c, "20K 60K 60K");
            PORT_DIPSETTING(0x04, "20K 60K");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x60, 0x60, DEF_STR("Lives"));
            PORT_DIPSETTING(0x40, "1");
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x60, "3");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));

            PORT_START();	/* FAKE */
            /* The player inputs are not memory mapped, they are handled by an I/O chip. */
            /* These fake input ports are read by galaga_customio_data_r() */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* FAKE */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* FAKE */
            PORT_BIT(0x03, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START2, 1);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 1);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 1);
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        public driver_xevious()
        {
            drv = new machine_driver_xevious();
            year = "1982";
            name = "xevious";
            description = "Xevious (Namco)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_xevious();
            rom = rom_xevious();
            drv.HasNVRAMhandler = false;
        }
    }
}
