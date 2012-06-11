using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_gaplus : Mame.GameDriver
    {
        static _BytePtr gaplus_snd_sharedram = new _BytePtr(1);
        static _BytePtr gaplus_sharedram = new _BytePtr(1);
        public static _BytePtr gaplus_customio_1 = new _BytePtr(1);
        public static _BytePtr gaplus_customio_2 = new _BytePtr(1);
        public static _BytePtr gaplus_customio_3 = new _BytePtr(1);
        static int int_enable_2, int_enable_3;
        static int credits, coincounter1, coincounter2;

        /* CPU 1 (MAIN CPU) read addresses */
        static Mame.MemoryReadAddress[] readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Generic.videoram_r ), /* video RAM */
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Generic.colorram_r ), /* color RAM */
	new Mame.MemoryReadAddress( 0x0800, 0x1fff, gaplus_sharedram_r ),  /* shared RAM with CPU #2 & spriteram */
	new Mame.MemoryReadAddress( 0x6040, 0x63ff, gaplus_snd_sharedram_r ),  /* shared RAM with CPU #3 */
	new Mame.MemoryReadAddress( 0x6800, 0x680f, gaplus_customio_r_1 ),	/* custom I/O chip #1 interface */
	new Mame.MemoryReadAddress( 0x6810, 0x681f, gaplus_customio_r_2 ),	/* custom I/O chip #2 interface */
	new Mame.MemoryReadAddress( 0x6820, 0x682f, gaplus_customio_r_3 ),	/* custom I/O chip #3 interface */
	new Mame.MemoryReadAddress( 0x7820, 0x782f, Mame.MRA_RAM ),	/* ??? */
	new Mame.MemoryReadAddress( 0x7c00, 0x7c01, Mame.MRA_NOP ),	/* ??? */
	new Mame.MemoryReadAddress( 0xa000, 0xffff, Mame.MRA_ROM ),	/* gp2-4.64 at a000, gp2-3.64 at c000, gp2-2.64 at e000 */
	new Mame.MemoryReadAddress( -1 )						  /* end of table */
};

        /* CPU 1 (MAIN CPU) write addresses */
        public static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x03ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size),  /* video RAM */
	new Mame.MemoryWriteAddress( 0x0400, 0x07ff, Generic.colorram_w, Generic.colorram ),  /* color RAM */
	new Mame.MemoryWriteAddress( 0x0800, 0x1fff, gaplus_sharedram_w, gaplus_sharedram ), /* shared RAM with CPU #2 */
	new Mame.MemoryWriteAddress( 0x6040, 0x63ff, gaplus_snd_sharedram_w, gaplus_snd_sharedram ), /* shared RAM with CPU #3 */
	new Mame.MemoryWriteAddress( 0x6800, 0x680f, gaplus_customio_w_1, gaplus_customio_1 ),	/* custom I/O chip #1 interface */
	new Mame.MemoryWriteAddress( 0x6810, 0x681f, gaplus_customio_w_2, gaplus_customio_2 ),	/* custom I/O chip #2 interface */
	new Mame.MemoryWriteAddress( 0x6820, 0x682f, gaplus_customio_w_3, gaplus_customio_3 ),	/* custom I/O chip #3 interface */
	new Mame.MemoryWriteAddress( 0x7820, 0x782f, Mame.MWA_RAM ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x7c00, 0x7c00, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x8400, 0x8400, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x8c00, 0x8c00, gaplus_reset_2_3_w ),	 	/* reset CPU #2 y #3? */
	new Mame.MemoryWriteAddress( 0x9400, 0x9400, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x9c00, 0x9c00, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0xa000, 0xa003, gaplus_starfield_control_w ), /* starfield control */
	new Mame.MemoryWriteAddress( 0xa000, 0xffff, Mame.MWA_ROM ),				/* ROM */
	new Mame.MemoryWriteAddress( -1 )									  /* end of table */
};

        /* CPU 2 (SUB CPU) read addresses */
        public static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Generic.videoram_r ), /* video RAM */
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Generic.colorram_r ), /* color RAM */
	new Mame.MemoryReadAddress( 0x0800, 0x1fff, gaplus_sharedram_r),  /* shared RAM with CPU #1 & spriteram */
	new Mame.MemoryReadAddress( 0xa000, 0xffff, Mame.MRA_ROM ),	/* gp2-8.64 at a000, gp2-7.64 at c000, gp2-6.64 at e000 */
	new Mame.MemoryReadAddress( -1 )						  /* end of table */
};

        /* CPU 2 (SUB CPU) write addresses */
        public static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x03ff, Generic.videoram_w ),  /* video RAM */
	new Mame.MemoryWriteAddress( 0x0400, 0x07ff, Generic.colorram_w ),  /* color RAM */
	new Mame.MemoryWriteAddress( 0x0800, 0x1fff, gaplus_sharedram_w ), /* shared RAM with CPU #1 */
	new Mame.MemoryWriteAddress( 0x500f, 0x500f, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x6001, 0x6001, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x6080, 0x6081, gaplus_interrupt_ctrl_2_w ),   /* IRQ 2 enable */
	new Mame.MemoryWriteAddress( 0xa000, 0xffff, Mame.MWA_ROM ),	/* ROM */
	new Mame.MemoryWriteAddress( -1 )						  /* end of table */
};

        /* CPU 3 (SOUND CPU) read addresses */
        public static Mame.MemoryReadAddress[] readmem_cpu3 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x003f, Mame.MRA_RAM ),	/* sound registers? */
	new Mame.MemoryReadAddress( 0x0040, 0x03ff, gaplus_snd_sharedram_r ), /* shared RAM with CPU #1 */
	new Mame.MemoryReadAddress( 0x3000, 0x3001,Mame.MRA_NOP ),	/* ???*/
	new Mame.MemoryReadAddress( 0xe000, 0xffff,Mame.MRA_ROM ),	/* ROM gp2-1.64 */
	new Mame.MemoryReadAddress( -1 )						  /* end of table */
};

        /* CPU 3 (SOUND CPU) write addresses */
        public static Mame.MemoryWriteAddress[] writemem_cpu3 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x003f, Namco.mappy_sound_w, Namco.namco_soundregs),	/* sound registers */
	new Mame.MemoryWriteAddress( 0x0040, 0x03ff, gaplus_snd_sharedram_w ), /* shared RAM with the main CPU */
	new Mame.MemoryWriteAddress( 0x2007, 0x2007, Mame.MWA_NOP),	/* ???*/
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, Mame.watchdog_reset_w ),	/* watchdog */
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, gaplus_interrupt_ctrl_3a_w ),	/* interrupt enable */
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, gaplus_interrupt_ctrl_3b_w ),	/* interrupt disable */
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),	/* ROM */
	new Mame.MemoryWriteAddress( -1 )						  /* end of table */
};

        static Mame.GfxLayout charlayout1 =
        new Mame.GfxLayout(
            8, 8,											/* 8*8 characters */
            256,											/* 256 characters */
            2,											  	/* 2 bits per pixel */
            new uint[] { 4, 6 },				 						/* the 2 bitplanes are packed into one nibble */
            new uint[] { 16 * 8, 16 * 8 + 1, 24 * 8, 24 * 8 + 1, 0, 1, 8 * 8, 8 * 8 + 1 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            32 * 8
        );

        static Mame.GfxLayout charlayout2 =
        new Mame.GfxLayout(
            8, 8,											/* 8*8 characters */
            256,											/* 256 characters */
            2,												/* 2 bits per pixel */
            new uint[] { 0, 2 },										/* the 2 bitplanes are packed into one nibble */
            new uint[] { 16 * 8, 16 * 8 + 1, 24 * 8, 24 * 8 + 1, 0, 1, 8 * 8, 8 * 8 + 1 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            32 * 8
        );

        static Mame.GfxLayout spritelayout1 =
        new Mame.GfxLayout(
            16, 16,			/* 16*16 sprites */
            128,		   /* 128 sprites */
            3,			 /* 3 bits per pixel */
            new uint[] { 0, 8192 * 8 + 0, 8192 * 8 + 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
	  16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
	  32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8		   /* every sprite takes 64 bytes */
        );


        static Mame.GfxLayout spritelayout2 =
        new Mame.GfxLayout(
            16, 16,			/* 16*16 sprites */
            128,		   /* 128 sprites */
            3,			 /* 3 bits per pixel */
            new uint[] { 4, 8192 * 8 * 2 + 0, 8192 * 8 * 2 + 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
	  16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
	  32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8		   /* every sprite takes 64 bytes */
        );

        static Mame.GfxLayout spritelayout3 = new Mame.GfxLayout(
            16, 16,										   /* 16*16 sprites */
            128,										   /* 128 sprites */
            3,											   /* 3 bits per pixel (one is always 0) */
            new uint[] { 8192 * 8 + 0, 0, 4 },							   /* the two bitplanes are packed into one byte */
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
		16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
	  32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8											/* every sprite takes 64 bytes */
        );

        public static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, charlayout1,      0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, charlayout2,      0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, spritelayout1, 64*4, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, spritelayout2, 64*4, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x6000, spritelayout3, 64*4, 64 ),
};

        public static Namco_interface namco_interface =
        new Namco_interface(
            23920,	/* sample rate (approximate value) */
            8,	  /* number of voices */
            100,	/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );

        static string[] gaplus_sample_names =
{
	"*galaga",
	"bang.wav",
	null       /* end of array */
};

        public static Mame.Samplesinterface samples_interface =
        new Mame.Samplesinterface(
            1,	/* one channel */
            80,	/* volume */
            gaplus_sample_names
        );

        const byte MAX_STARS = 250;
        const float SPEED_1 = 0.5f, SPEED_2 = 1.0f, SPEED_3 = 2.0f;
        struct star { public float x, y; public int col, set;}
        static star[] stars = new star[MAX_STARS];


        static byte[] gaplus_starfield_control = new byte[4];
        static int total_stars;
        static int flipscreen = 0;

        static void starfield_init()
        {
            int generator = 0;
            int x, y;
            int set = 0;
            int width, height;

            width = Mame.Machine.drv.screen_width;
            height = Mame.Machine.drv.screen_height;

            total_stars = 0;

            /* precalculate the star background */
            /* this comes from the Galaxian hardware, Gaplus is probably different */

            for (y = 0; y < height; y++)
            {
                for (x = width * 2 - 1; x >= 0; x--)
                {
                    int bit1, bit2;

                    generator <<= 1;
                    bit1 = (~generator >> 17) & 1;
                    bit2 = (generator >> 5) & 1;

                    if ((bit1 ^ bit2) != 0) generator |= 1;

                    if (((~generator >> 16) & 1) != 0 && (generator & 0xff) == 0xff)
                    {
                        int color;

                        color = (~(generator >> 8)) & 0x3f;
                        if (color != 0 && total_stars < MAX_STARS)
                        {
                            stars[total_stars].x = x;
                            stars[total_stars].y = y;
                            stars[total_stars].col = Mame.Machine.pens[color];
                            stars[total_stars].set = set++;

                            if (set == 3)
                                set = 0;

                            total_stars++;
                        }
                    }
                }
            }
        }
        static void gaplus_starfield_update()
        {
            int i;
            int width, height;

            width = Mame.Machine.drv.screen_width;
            height = Mame.Machine.drv.screen_height;

            /* check if we're running */
            if ((gaplus_starfield_control[0] & 1) == 0)
                return;

            /* update the starfields */
            for (i = 0; i < total_stars; i++)
            {
                switch (gaplus_starfield_control[stars[i].set + 1])
                {
                    case 0x87:
                        /* stand still */
                        break;

                    case 0x86:
                        /* scroll down (speed 1) */
                        stars[i].x += SPEED_1;
                        break;

                    case 0x85:
                        /* scroll down (speed 2) */
                        stars[i].x += SPEED_2;
                        break;

                    case 0x06:
                        /* scroll down (speed 3) */
                        stars[i].x += SPEED_3;
                        break;

                    case 0x80:
                        /* scroll up (speed 1) */
                        stars[i].x -= SPEED_1;
                        break;

                    case 0x82:
                        /* scroll up (speed 2) */
                        stars[i].x -= SPEED_2;
                        break;

                    case 0x81:
                        /* scroll up (speed 3) */
                        stars[i].x -= SPEED_3;
                        break;

                    case 0x9f:
                        /* scroll left (speed 2) */
                        stars[i].y += SPEED_2;
                        break;

                    case 0xaf:
                        /* scroll left (speed 1) */
                        stars[i].y += SPEED_1;
                        break;
                }

                /* wrap */
                if (stars[i].x < 0)
                    stars[i].x = (float)(width * 2) + stars[i].x;

                if (stars[i].x >= (float)(width * 2))
                    stars[i].x -= (float)(width * 2);

                if (stars[i].y < 0)
                    stars[i].y = (float)(height) + stars[i].y;

                if (stars[i].y >= (float)(height))
                    stars[i].y -= (float)(height);
            }
        }
        static void starfield_render(Mame.osd_bitmap bitmap)
        {
            int width, height;

            width = Mame.Machine.drv.screen_width;
            height = Mame.Machine.drv.screen_height;

            /* check if we're running */
            if ((gaplus_starfield_control[0] & 1) == 0)
                return;

            /* draw the starfields */
            for (int i = 0; i < total_stars; i++)
            {
                int x, y;

                x = (int)stars[i].x;
                y = (int)stars[i].y;

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    Mame.plot_pixel(bitmap, x, y, stars[i].col);
                }
            }
        }
        static void gaplus_starfield_control_w(int offset, int data)
        {
            gaplus_starfield_control[offset] = (byte)data;
        }
        static void gaplus_flipscreen_w(int data)
        {
            flipscreen = data;
            Generic.SetDirtyBuffer(true);
        }


        public static int gaplus_sharedram_r(int offset)
        {
            return gaplus_sharedram[offset];
        }
        static void gaplus_sharedram_w(int offset, int data)
        {
            if (offset == 0x082c)	/* 0x102c */
                gaplus_flipscreen_w(data);
            gaplus_sharedram[offset] = (byte)data;
        }
        public static int gaplus_snd_sharedram_r(int offset)
        {
            return gaplus_snd_sharedram[offset];
        }
        static void gaplus_snd_sharedram_w(int offset, int data)
        {
            gaplus_snd_sharedram[offset] = (byte)data;
        }
        static void gaplus_interrupt_ctrl_2_w(int offset, int data)
        {
            int_enable_2 = offset;
        }
        static void gaplus_interrupt_ctrl_3a_w(int offset, int data)
        {
            int_enable_3 = 1;
        }
        static void gaplus_interrupt_ctrl_3b_w(int offset, int data)
        {
            int_enable_3 = 0;
        }
        public static int gaplus_interrupt_1()
        {
            gaplus_starfield_update(); /* update starfields */

            return Mame.interrupt();
        }
        public static int gaplus_interrupt_2()
        {
            if (int_enable_2 != 0)
                return Mame.interrupt();
            else
                return Mame.ignore_interrupt();
        }
        public static int gaplus_interrupt_3()
        {
            if (int_enable_3 != 0)
                return Mame.interrupt();
            else
                return Mame.ignore_interrupt();
        }
        static void gaplus_reset_2_3_w(int offset, int data)
        {
            int_enable_2 = int_enable_3 = 1;
            Mame.cpu_set_reset_line(1, Mame.PULSE_LINE);
            Mame.cpu_set_reset_line(2, Mame.PULSE_LINE);
            credits = coincounter1 = coincounter2 = 0;
        }
        static void gaplus_customio_w_1(int offset, int data)
        {
            gaplus_customio_1[offset] = (byte)data;
        }
        static void gaplus_customio_w_2(int offset, int data)
        {
            gaplus_customio_2[offset] = (byte)data;
        }
        static void gaplus_customio_w_3(int offset, int data)
        {
            if ((offset == 0x09) && (data >= 0x0f))
                Mame.sample_start(0, 0, 0);
            gaplus_customio_3[offset] = (byte)data;
        }

        static int[] credmoned = { 1, 1, 2, 3 };
        static int[] monedcred = { 1, 2, 1, 1 };

        static int lastval1, lastval2, lastval3, lastval4;
        static int gaplus_customio_r_1(int offset)
        {
            int mode, val, temp1, temp2;

            mode = gaplus_customio_1[8];
            if (mode == 3)	/* normal mode */
            {
                switch (offset)
                {
                    case 0:     /* Coin slots, high nibble of port 2 */
                        {
                            val = Mame.readinputport(2) >> 4;
                            temp1 = Mame.readinputport(0) & 0x03;
                            temp2 = (Mame.readinputport(0) >> 6) & 0x03;

                            /* bit 0 is a trigger for the coin slot 1 */
                            if ((val & 1) != 0 && ((val ^ lastval1) & 1) != 0)
                            {
                                coincounter1++;
                                if (coincounter1 >= credmoned[temp1])
                                {
                                    credits += monedcred[temp1];
                                    coincounter1 -= credmoned[temp1];
                                }
                            }
                            /* bit 1 is a trigger for the coin slot 2 */
                            if ((val & 2) != 0 && ((val ^ lastval1) & 2) != 0)
                            {
                                coincounter2++;
                                if (coincounter2 >= credmoned[temp2])
                                {
                                    credits += monedcred[temp2];
                                    coincounter2 -= credmoned[temp2];
                                }
                            }

                            if (credits > 99)
                                credits = 99;

                            return lastval1 = val;
                        }
                    case 1:
                        {
                            val = Mame.readinputport(2) & 0x03;
                            temp1 = Mame.readinputport(0) & 0x03;
                            temp2 = (Mame.readinputport(0) >> 6) & 0x03;

                            /* bit 0 is a trigger for the 1 player start */
                            if ((val & 1) != 0 && ((val ^ lastval2) & 1) != 0)
                            {
                                if (credits > 0)
                                    credits--;
                                else
                                    val &= ~1;   /* otherwise you can start with no credits! */
                            }
                            /* bit 1 is a trigger for the 2 player start */
                            if ((val & 2) != 0 && ((val ^ lastval2) & 2) != 0)
                            {
                                if (credits >= 2)
                                    credits -= 2;
                                else
                                    val &= ~2;   /* otherwise you can start with no credits! */
                            }
                            return lastval2 = val;
                        }
                    case 2:
                        return (credits / 10);      /* high BCD of credits */
                    case 3:
                        return (credits % 10);      /* low BCD of credits */
                    case 4:
                        return (Mame.readinputport(3) & 0x0f);   /* 1P controls */
                    case 5:
                        return (Mame.readinputport(4) & 0x03);   /* 1P button 1 */
                    case 6:
                        return (Mame.readinputport(3) >> 4);     /* 2P controls */
                    case 7:
                        return ((Mame.readinputport(4) >> 2) & 0x03);    /* 2P button 1 */
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            else if (mode == 5)  /* IO tests chip 1 */
            {
                switch (offset)
                {
                    case 0:
                    case 1:
                        return 0x0f;
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            else if (mode == 1)	/* test mode controls */
            {
                switch (offset)
                {
                    case 4:
                        return (Mame.readinputport(2) & 0x03);	/* start 1 & 2 */
                    case 5:
                        return (Mame.readinputport(3) & 0x0f);	/* 1P controls */
                    case 6:
                        return (Mame.readinputport(3) >> 4);	/* 2P controls */
                    case 7:
                        return (Mame.readinputport(4) & 0x0f);	/* button 1 & 2 */
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            return gaplus_customio_1[offset];
        }
        static int gaplus_customio_r_2(int offset)
        {
            int val, mode;

            mode = gaplus_customio_2[8];
            if (mode == 8)  /* IO tests chip 2 */
            {
                switch (offset)
                {
                    case 0:
                        return 0x06;
                    case 1:
                        return 0x09;
                    default:
                        return gaplus_customio_2[offset];
                }
            }
            else if (mode == 1)	/* this values are read only by the game on power up */
            {
                switch (offset)
                {
                    case 0:
                        val = Mame.readinputport(0) & 0x0f; /* credits/coin 1P & fighters */
                        break;
                    case 1:
                        val = Mame.readinputport(1) >> 5;   /* bonus life */
                        break;
                    case 2:
                        val = Mame.readinputport(1) & 0x0f; /* rank & test mode */
                        break;
                    case 3:
                        val = Mame.readinputport(0) >> 6;   /* credits/coin 2P */
                        break;
                    default:
                        val = gaplus_customio_2[offset];
                        break;
                }
                return val;
            }
            else
                return gaplus_customio_2[offset];
        }
        static int gaplus_customio_r_3(int offset)
        {
            int mode;

            mode = gaplus_customio_3[8];
            if (mode == 2)
            {
                switch (offset)
                {
                    case 2:
                        return 0x0f;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
            else
            {
                switch (offset)
                {
                    case 0:
                        return ((Mame.readinputport(0) & 0x20) >> 3);   /* cabinet */
                    case 1:
                        return 0x0f;
                    case 2:
                        return 0x0e;
                    case 3:
                        return 0x01;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
        }
        static int gaplusa_customio_r_1(int offset)
        {
            int mode, val, temp1, temp2;

            mode = gaplus_customio_1[8];
            if (mode == 4)	/* normal mode */
            {
                switch (offset)
                {
                    case 0:
                        return (credits / 10);      /* high BCD of credits */
                    case 1:
                        return (credits % 10);      /* low BCD of credits */
                    case 2:     /* Coin slots, high nibble of port 2 */
                        {
                            val = Mame.readinputport(2) >> 4;
                            temp1 = Mame.readinputport(0) & 0x03;
                            temp2 = (Mame.readinputport(0) >> 6) & 0x03;

                            /* bit 0 is a trigger for the coin slot 1 */
                            if ((val & 1) != 0 && ((val ^ lastval3) & 1) != 0)
                            {
                                coincounter1++;
                                if (coincounter1 >= credmoned[temp1])
                                {
                                    credits += monedcred[temp1];
                                    coincounter1 -= credmoned[temp1];
                                }
                            }
                            /* bit 1 is a trigger for the coin slot 2 */
                            if ((val & 2) != 0 && ((val ^ lastval3) & 2) != 0)
                            {
                                coincounter2++;
                                if (coincounter2 >= credmoned[temp2])
                                {
                                    credits += monedcred[temp2];
                                    coincounter2 -= credmoned[temp2];
                                }
                            }

                            if (credits > 99)
                                credits = 99;

                            return lastval3 = val;
                        }
                    case 3:
                        {
                            val = Mame.readinputport(2) & 0x03;
                            temp1 = Mame.readinputport(0) & 0x03;
                            temp2 = (Mame.readinputport(0) >> 6) & 0x03;

                            /* bit 0 is a trigger for the 1 player start */
                            if ((val & 1) != 0 && ((val ^ lastval4) & 1) != 0)
                            {
                                if (credits > 0)
                                    credits--;
                                else
                                    val &= ~1;   /* otherwise you can start with no credits! */
                            }
                            /* bit 1 is a trigger for the 2 player start */
                            if ((val & 2) != 0 && ((val ^ lastval4) & 2) != 0)
                            {
                                if (credits >= 2)
                                    credits -= 2;
                                else
                                    val &= ~2;   /* otherwise you can start with no credits! */
                            }
                            return lastval4 = val;
                        }
                    case 4:
                        return (Mame.readinputport(3) & 0x0f);   /* 1P controls */
                    case 5:
                        return (Mame.readinputport(4) & 0x03);   /* 1P button 1 */
                    case 6:
                        return (Mame.readinputport(3) >> 4);     /* 2P controls */
                    case 7:
                        return ((Mame.readinputport(4) >> 2) & 0x03);    /* 2P button 1 */
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            else if (mode == 8)  /* IO tests chip 1 */
            {
                switch (offset)
                {
                    case 0:
                        return 0x06;
                    case 1:
                        return 0x09;
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            else if (mode == 1)	/* test mode */
            {
                switch (offset)
                {
                    case 0:
                        return (Mame.readinputport(2) & 0x03);	/* start 1 & 2 */
                    case 1:
                        return (Mame.readinputport(3) & 0x0f);	/* 1P controls */
                    case 2:
                        return (Mame.readinputport(3) >> 4);	/* 2P controls */
                    case 3:
                        return (Mame.readinputport(4) & 0x0f);	/* button 1 & 2 */
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            return gaplus_customio_1[offset];
        }
        static int gaplusa_customio_r_2(int offset)
        {
            int val, mode;

            mode = gaplus_customio_2[8];
            if (mode == 5)  /* IO tests chip 2 */
            {
                switch (offset)
                {
                    case 0:
                    case 1:
                        return 0x0f;
                    default:
                        return gaplus_customio_2[offset];
                }
            }
            else if (mode == 4)	/* this values are read only by the game on power up */
            {
                switch (offset)
                {
                    case 1:
                        val = Mame.readinputport(0) & 0x0f; /* credits/coin 1P & fighters */
                        break;
                    case 2:
                        val = Mame.readinputport(1) >> 5;   /* bonus life */
                        break;
                    case 4:
                        val = Mame.readinputport(1) & 0x0f; /* rank & test mode */
                        break;
                    case 7:
                        val = Mame.readinputport(0) >> 6;   /* credits/coin 2P */
                        break;
                    default:
                        val = gaplus_customio_2[offset];
                        break;
                }
                return val;
            }
            else
                return gaplus_customio_2[offset];
        }
        static int gaplusa_customio_r_3(int offset)
        {
            int mode;

            mode = gaplus_customio_3[8];
            if (mode == 2)
            {
                switch (offset)
                {
                    case 2:
                        return 0x0f;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
            else
            {
                switch (offset)
                {
                    case 0:
                        return ((Mame.readinputport(0) & 0x20) >> 3);   /* cabinet */
                    case 1:
                        return 0x0f;
                    case 2:
                        return 0x0e;
                    case 3:
                        return 0x01;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
        }
        static int galaga3_customio_r_1(int offset)
        {
            int mode;

            mode = gaplus_customio_1[8];
            if (mode == 1)  /* normal mode & test mode */
            {
                switch (offset)
                {
                    case 0:
                        return (Mame.readinputport(2) >> 4);	/* coin 1 & 2 */
                    case 1:
                        return (Mame.readinputport(3) & 0x0f);	/* 1P controls */
                    case 2:
                        return (Mame.readinputport(3) >> 4);	/* 2P controls */
                    case 3:
                        return (Mame.readinputport(2) & 0x0f);	/* start 1 & 2 and button 1 & 2 */
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            else if (mode == 8)  /* IO tests chip 1 */
            {
                switch (offset)
                {
                    case 0:
                        return 0x06;
                    case 1:
                        return 0x09;
                    default:
                        return gaplus_customio_1[offset];
                }
            }
            return gaplus_customio_1[offset];
        }
        static int galaga3_customio_r_2(int offset)
        {
            int val, mode;

            mode = gaplus_customio_2[8];
            if (mode == 5)  /* IO tests chip 2 */
            {
                switch (offset)
                {
                    case 0:
                    case 1:
                        return 0x0f;
                    default:
                        return gaplus_customio_2[offset];
                }
            }
            else if (mode == 4)     /* this values are read only by the game on power up */
            {
                switch (offset)
                {
                    case 1:
                        val = Mame.readinputport(0) & 0x0f;	/* credits/coin 1P & fighters */
                        break;
                    case 2:
                        val = Mame.readinputport(1) >> 5;		/* bonus life */
                        break;
                    case 4:
                        val = Mame.readinputport(1) & 0x07;	/* rank */
                        break;
                    case 7:
                        val = Mame.readinputport(0) >> 6;		/* credits/coin 2P */
                        break;
                    default:
                        val = gaplus_customio_2[offset];
                        break;
                }
                return val;
            }
            else
                return gaplus_customio_2[offset];
        }
        static int galaga3_customio_r_3(int offset)
        {
            int mode;

            mode = gaplus_customio_3[8];
            if (mode == 2)
            {
                switch (offset)
                {
                    case 0:
                        return ((Mame.readinputport(0) & 0x20) >> 3) ^ ~(Mame.readinputport(1) & 0x08); /* cabinet & test mode */;
                    case 2:
                        return 0x0f;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
            else
            {
                switch (offset)
                {
                    case 0:
                        return ((Mame.readinputport(0) & 0x20) >> 3) ^ ~(Mame.readinputport(1) & 0x08); /* cabinet & test mode */;
                    case 1:
                        return 0x0f;
                    case 2:
                        return 0x0e;
                    case 3:
                        return 0x01;
                    default:
                        return gaplus_customio_3[offset];
                }
            }
        }

        public static void  gaplus_vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    /* green component */
                    bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    /* blue component */
                    bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                cpi += 2 * Mame.Machine.drv.total_colors;
                /* color_prom now points to the beginning of the lookup table */


                /* characters use colors 240-255 */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable, 0, i, 240 + (color_prom[cpi++] & 0x0f));

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                {
                    COLOR(colortable, 2, i, (color_prom[cpi] & 0x0f) + ((color_prom[cpi + (uint)TOTAL_COLORS(2)] & 0x0f) << 4));
                    cpi++;
                }
            }

        public static void gaplus_init_machine()
        {
            int_enable_2 = int_enable_3 = 1;
            credits = coincounter1 = coincounter2 = 0;
        }
        public static int gaplus_vhstart()
        {
            /* set up spriteram area */
            Generic.spriteram_size[0] = 0x80;
            Generic.spriteram = new _BytePtr(gaplus_sharedram, 0x780);
            Generic.spriteram_2 = new _BytePtr(gaplus_sharedram, 0x780 + 0x800);
            Generic.spriteram_3 = new _BytePtr(gaplus_sharedram, 0x780 + 0x800 + 0x800);

            starfield_init();

            return Generic.generic_vh_start();
        }
        static void gaplus_draw_sprite(Mame.osd_bitmap dest, uint code, uint color, bool flipx, bool flipy, int sx, int sy)
        {
            if (code < 128)
                Mame.drawgfx(dest, Mame.Machine.gfx[2], code, color, flipx, flipy, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 255);
            else if (code < 256)
                Mame.drawgfx(dest, Mame.Machine.gfx[3], code, color, flipx, flipy, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 255);
            else
                Mame.drawgfx(dest, Mame.Machine.gfx[4], code, color, flipx, flipy, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 255);
        }
        public static void gaplus_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            Mame.fillbitmap(bitmap, Mame.Machine.pens[0], Mame.Machine.drv.visible_area);

            starfield_render(bitmap);

            /* for every character in the Video RAM, check if it has been modified */
            /* since last time and update it accordingly. */
            for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
            {
                int sx, sy, mx, my, bank;

                /* Even if Gaplus screen is 28x36, the memory layout is 32x32. We therefore
                have to convert the memory coordinates into screen coordinates.
                Note that 32*32 = 1024, while 28*36 = 1008: therefore 16 bytes of Video RAM
                don't map to a screen position. We don't check that here, however: range
                checking is performed by drawgfx(). */

                mx = offs / 32;
                my = offs % 32;

                if (mx <= 1)        /* bottom screen characters */
                {
                    sx = 29 - my;
                    sy = mx + 34;
                }
                else if (mx >= 30)  /* top screen characters */
                {
                    sx = 29 - my;
                    sy = mx - 30;
                }
                else                /* middle screen characters */
                {
                    sx = 29 - mx;
                    sy = my + 2;
                }

                if (flipscreen != 0)
                {
                    sx = 27 - sx;
                    sy = 35 - sy;
                }
                /* colorram layout: */
                /* bit 7 = bank */
                /* bit 6 = chars that go on top of sprites (unimplemented yet) */
                /* bit 5-0 = color */

                sx = ((Mame.Machine.drv.screen_height - 1) / 8) - sx;

                bank = (Generic.colorram[offs] & 0x80) != 0 ? 1 : 0;

                Mame.drawgfx(bitmap, Mame.Machine.gfx[bank],
                        Generic.videoram[offs],
                        (uint)(Generic.colorram[offs] & 0x3f),
                        flipscreen != 0, flipscreen != 0, 8 * sy, 8 * sx,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }

            /* Draw the sprites. */
            for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
            {
                /* is it on? */
                if ((Generic.spriteram_3[offs + 1] & 2) == 0)
                {
                    uint sprite = (uint)(Generic.spriteram[offs] + 4 * (Generic.spriteram_3[offs] & 0x40));
                    uint color = (uint)(Generic.spriteram[offs + 1] & 0x3f);
                    int y = (Mame.Machine.drv.screen_height) - Generic.spriteram_2[offs] - 8;
                    int x = (Generic.spriteram_2[offs + 1] - 71) + 0x100 * (Generic.spriteram_3[offs + 1] & 1);
                    bool flipy = (Generic.spriteram_3[offs] & 2) != 0;
                    bool flipx = (Generic.spriteram_3[offs] & 1) != 0;

                    if (flipscreen != 0)
                    {
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    switch (Generic.spriteram_3[offs] & 0xa8)
                    {
                        case 0:		/* normal size */
                            gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                            break;

                        case 0x20:     /* 2x horizontal */
                            sprite &= unchecked((uint)~2);
                            if (!flipy)
                            {
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y - 16);
                            }
                            else
                            {
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y - 16);
                            }
                            break;

                        case 0x08:     /* 2x vertical */
                            sprite &= unchecked((uint)~1);
                            if (!flipx)
                            {
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y);
                            }
                            else
                            {
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y);
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y);
                            }
                            break;

                        case 0x28:        /* 2x both ways */
                            sprite &= unchecked((uint)~3);
                            if (!flipx && !flipy)
                            {
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x + 16, y);
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y - 16);
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y - 16);
                            }
                            else if (flipx && flipy)
                            {
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y);
                                gaplus_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x, y - 16);
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x + 16, y - 16);
                            }
                            else if (flipy)
                            {
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y);
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y - 16);
                                gaplus_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x + 16, y - 16);
                            }
                            else /* flipx */
                            {
                                gaplus_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x, y);
                                gaplus_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x + 16, y);
                                gaplus_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y - 16);
                                gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y - 16);
                            }
                            break;

                        case 0xa0:  /*  draw the sprite twice in a row */
                            gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                            gaplus_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y - 16);
                            break;

                    }
                }
            }
        }
        public class machine_driver_gaplus : Mame.MachineDriver
        {
            public machine_driver_gaplus()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu1, writemem_cpu1, null, null, gaplus_interrupt_1, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu2, writemem_cpu2, null, null, gaplus_interrupt_2, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu3, writemem_cpu3, null, null, gaplus_interrupt_3, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_gaplus.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 64 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
            }
            public override void init_machine()
            {
                gaplus_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                gaplus_vh_init_palette(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return gaplus_vhstart();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                gaplus_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_gaplus()
        {

            ROM_START("gaplus");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 64k for the MAIN CPU */
            ROM_LOAD("gp2-4.64", 0xa000, 0x2000, 0x484f11e0);
            ROM_LOAD("gp2-3.64", 0xc000, 0x2000, 0xa74b0266);
            ROM_LOAD("gp2-2.64", 0xe000, 0x2000, 0x69fdfdb7);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64k for the SUB CPU */
            ROM_LOAD("gp2-8.64", 0xa000, 0x2000, 0xbff601a6);
            ROM_LOAD("gp2-7.64", 0xc000, 0x2000, 0x0621f7df);
            ROM_LOAD("gp2-6.64", 0xe000, 0x2000, 0x14cd61ea);

            ROM_REGION(0x10000, Mame.REGION_CPU3);/* 64k for the SOUND CPU */
            ROM_LOAD("gp2-1.64", 0xe000, 0x2000, 0xed8aa206);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gp2-5.64", 0x0000, 0x2000, 0xf3d19987);	/* characters */

            ROM_REGION(0xa000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gp2-9.64", 0x0000, 0x2000, 0xe6a9ae67);	/* objects */
            ROM_LOAD("gp2-11.64", 0x2000, 0x2000, 0x57740ff9);	/* objects */
            ROM_LOAD("gp2-10.64", 0x4000, 0x2000, 0x6cd8ce11);	/* objects */
            ROM_LOAD("gp2-12.64", 0x6000, 0x2000, 0x7316a1f1);	/* objects */
            /* 0xa000-0xbfff empty space to decode sprite set #3 as 3 bits per pixel */

            ROM_REGION(0x0800, Mame.REGION_PROMS);
            ROM_LOAD("gp2-1p.bin", 0x0000, 0x0100, 0xa5091352); /* red palette ROM (4 bits) */
            ROM_LOAD("gp2-1n.bin", 0x0100, 0x0100, 0x8bc8022a);/* green palette ROM (4 bits) */
            ROM_LOAD("gp2-2n.bin", 0x0200, 0x0100, 0x8dabc20b);  /* blue palette ROM (4 bits) */
            ROM_LOAD("gp2-6s.bin", 0x0300, 0x0100, 0x2faa3e09);  /* char color ROM */
            ROM_LOAD("gp2-6p.bin", 0x0400, 0x0200, 0x6f99c2da);  /* sprite color ROM (lower 4 bits) */
            ROM_LOAD("gp2-6n.bin", 0x0600, 0x0200, 0xc7d31657);  /* sprite color ROM (upper 4 bits) */

            ROM_REGION(0x0100, Mame.REGION_SOUND1); /* sound prom */
            ROM_LOAD("gp2-3f.bin", 0x0000, 0x0100, 0x2d9fbdd8);
            return ROM_END;
        }
        public static Mame.InputPortTiny[] input_ports_gaplus()
        {
            INPUT_PORTS_START("gaplus");
            PORT_START();  /* DSW0 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x03, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x04, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x08, "4");
            PORT_DIPSETTING(0x0c, "5");
            PORT_DIPNAME(0x10, 0x10, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x20, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_2C"));

            PORT_START();  /* DSW1 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "0");
            PORT_DIPSETTING(0x01, "1");
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x05, "5");
            PORT_DIPSETTING(0x06, "6");
            PORT_DIPSETTING(0x07, "7");
            PORT_SERVICE(0x08, IP_ACTIVE_HIGH);
            PORT_BITX(0x10, 0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0xe0, 0xe0, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0xe0, "30k 70k and every 70k");
            PORT_DIPSETTING(0xc0, "30k 100k and every 100k");
            PORT_DIPSETTING(0xa0, "30k 100k and every 200k");
            PORT_DIPSETTING(0x80, "50k 100k and every 100k");
            PORT_DIPSETTING(0x60, "50k 100k and every 200k");
            PORT_DIPSETTING(0x00, "50k 150k and every 150k");
            PORT_DIPSETTING(0x40, "50k 150k and every 300k");
            PORT_DIPSETTING(0x20, "50k 150k");

            PORT_START();  /* IN0 */
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2, 1);
            /* 0x08 service switch (not implemented yet) */
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2, 1);

            PORT_START();  /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);

            PORT_START();  /* IN2 */
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2, 1);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            return INPUT_PORTS_END;
        }
        public driver_gaplus()
        {
            drv = new machine_driver_gaplus();
            year = "1984";
            name = "gaplus";
            description = "Gaplus (set 1)";
            manufacturer = "Namco";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_gaplus();
            rom = rom_gaplus();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_galaga3 : Mame.GameDriver
    {
        static Mame.MemoryReadAddress[] galaga3_readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Generic.videoram_r ), /* video RAM */
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Generic.colorram_r ), /* color RAM */
	new Mame.MemoryReadAddress( 0x0800, 0x1fff, driver_gaplus.gaplus_sharedram_r ),  /* shared RAM with CPU #2 & spriteram */
	new Mame.MemoryReadAddress( 0x6040, 0x63ff, driver_gaplus.gaplus_snd_sharedram_r ),  /* shared RAM with CPU #3 */
	new Mame.MemoryReadAddress( 0x6800, 0x680f, galaga3_customio_r_1 ),	/* custom I/O chip #1 interface */
	new Mame.MemoryReadAddress( 0x6810, 0x681f, galaga3_customio_r_2 ),	/* custom I/O chip #2 interface */
	new Mame.MemoryReadAddress( 0x6820, 0x682f, galaga3_customio_r_3 ),	/* custom I/O chip #3 interface */
	new Mame.MemoryReadAddress( 0x7820, 0x782f, Mame.MRA_RAM ),	/* ??? */
	new Mame.MemoryReadAddress( 0x7c00, 0x7c01, Mame.MRA_NOP ),	/* ??? */
	new Mame.MemoryReadAddress( 0xa000, 0xffff, Mame.MRA_ROM ),	/* gp2-4.64 at a000, gp2-3.64 at c000, gp2-2.64 at e000 */
	new Mame.MemoryReadAddress( -1 )						  /* end of table */
};
        static int galaga3_customio_r_1(int offset)
        {
            int mode;

            mode = driver_gaplus.gaplus_customio_1[8];
            if (mode == 1)  /* normal mode & test mode */
            {
                switch (offset)
                {
                    case 0:return (Mame.readinputport(2) >> 4);	/* coin 1 & 2 */
                    case 1:return (Mame.readinputport(3) & 0x0f);	/* 1P controls */
                    case 2:return (Mame.readinputport(3) >> 4);	/* 2P controls */
                    case 3:return (Mame.readinputport(2) & 0x0f);	/* start 1 & 2 and button 1 & 2 */
                    default:return driver_gaplus.gaplus_customio_1[offset];
                }
            }
            else if (mode == 8)  /* IO tests chip 1 */
            {
                switch (offset)
                {
                    case 0:return 0x06;
                    case 1:return 0x09;
                    default:return driver_gaplus.gaplus_customio_1[offset];
                }
            }
            return driver_gaplus.gaplus_customio_1[offset];
        }
        static int galaga3_customio_r_2(int offset)
        {
            int val, mode;

            mode = driver_gaplus.gaplus_customio_2[8];
            if (mode == 5)  /* IO tests chip 2 */
            {
                switch (offset)
                {
                    case 0:
                    case 1:return 0x0f;
                    default:return driver_gaplus.gaplus_customio_2[offset];
                }
            }
            else if (mode == 4)     /* this values are read only by the game on power up */
            {
                switch (offset)
                {
                    case 1:
                        val = Mame.readinputport(0) & 0x0f;	/* credits/coin 1P & fighters */
                        break;
                    case 2:
                        val =Mame.readinputport(1) >> 5;		/* bonus life */
                        break;
                    case 4:
                        val = Mame.readinputport(1) & 0x07;	/* rank */
                        break;
                    case 7:
                        val = Mame.readinputport(0) >> 6;		/* credits/coin 2P */
                        break;
                    default:
                        val = driver_gaplus.gaplus_customio_2[offset];
                        break;
                }
                return val;
            }
            else
                return driver_gaplus.gaplus_customio_2[offset];
        }

        static int galaga3_customio_r_3(int offset)
        {
            int mode;

            mode = driver_gaplus.gaplus_customio_3[8];
            if (mode == 2)
            {
                switch (offset)
                {
                    case 0:return ((Mame.readinputport(0) & 0x20) >> 3) ^ ~(Mame.readinputport(1) & 0x08); /* cabinet & test mode */;
                    case 2:return 0x0f;
                    default:return driver_gaplus.gaplus_customio_3[offset];
                }
            }
            else
            {
                switch (offset)
                {
                    case 0:return ((Mame.readinputport(0) & 0x20) >> 3) ^ ~(Mame.readinputport(1) & 0x08); /* cabinet & test mode */;
                    case 1:return 0x0f;
                    case 2:return 0x0e;
                    case 3:return 0x01;
                    default:return driver_gaplus.gaplus_customio_3[offset];
                }
            }
        }
        class machine_driver_galaga3 : Mame.MachineDriver
        {
            public machine_driver_galaga3()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, galaga3_readmem_cpu1, driver_gaplus.writemem_cpu1, null, null, driver_gaplus.gaplus_interrupt_1, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, driver_gaplus.readmem_cpu2, driver_gaplus.writemem_cpu2, null, null, driver_gaplus.gaplus_interrupt_2, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, driver_gaplus.readmem_cpu3, driver_gaplus.writemem_cpu3, null, null, driver_gaplus.gaplus_interrupt_3, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_gaplus.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 64 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, driver_gaplus.namco_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, driver_gaplus.samples_interface));
            }
            public override void init_machine()
            {
                driver_gaplus.gaplus_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                driver_gaplus.gaplus_vh_init_palette(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return driver_gaplus.gaplus_vhstart();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_gaplus.gaplus_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_galaga3()
        {
            ROM_START("galaga3");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 64k for the MAIN CPU */
            ROM_LOAD("gal3_9e.bin", 0xa000, 0x2000, 0xf4845e7f);
            ROM_LOAD("gal3_9d.bin", 0xc000, 0x2000, 0x86fac687);
            ROM_LOAD("gal3_9c.bin", 0xe000, 0x2000, 0xf1b00073);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64k for the SUB CPU */
            ROM_LOAD("gal3_6l.bin", 0xa000, 0x2000, 0x9ec3dce5);
            ROM_LOAD("gp2-7.64", 0xc000, 0x2000, 0x0621f7df);
            ROM_LOAD("gal3_6n.bin", 0xe000, 0x2000, 0x6a2942c5);

            ROM_REGION(0x10000, Mame.REGION_CPU3);/* 64k for the SOUND CPU */
            ROM_LOAD("gp2-1.64", 0xe000, 0x2000, 0xed8aa206);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gal3_9l.bin", 0x0000, 0x2000, 0x8d4dcebf);	/* characters */

            ROM_REGION(0xa000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gp2-9.64", 0x0000, 0x2000, 0xe6a9ae67);	/* objects */
            ROM_LOAD("gp2-11.64", 0x2000, 0x2000, 0x57740ff9);	/* objects */
            ROM_LOAD("gp2-10.64", 0x4000, 0x2000, 0x6cd8ce11);	/* objects */
            ROM_LOAD("gp2-12.64", 0x6000, 0x2000, 0x7316a1f1);	/* objects */
            /* 0xa000-0xbfff empty space to decode sprite set #3 as 3 bits per pixel */

            ROM_REGION(0x0800, Mame.REGION_PROMS);
            ROM_LOAD("gp2-1p.bin", 0x0000, 0x0100, 0xa5091352);  /* red palette ROM (4 bits) */
            ROM_LOAD("gp2-1n.bin", 0x0100, 0x0100, 0x8bc8022a);  /* green palette ROM (4 bits) */
            ROM_LOAD("gp2-2n.bin", 0x0200, 0x0100, 0x8dabc20b);  /* blue palette ROM (4 bits) */
            ROM_LOAD("gp2-6s.bin", 0x0300, 0x0100, 0x2faa3e09);  /* char color ROM */
            ROM_LOAD("g3_3f.bin", 0x0400, 0x0200, 0xd48c0eef);  /* sprite color ROM (lower 4 bits) */
            ROM_LOAD("g3_3e.bin", 0x0600, 0x0200, 0x417ba0dc);  /* sprite color ROM (upper 4 bits) */

            ROM_REGION(0x0100, Mame.REGION_SOUND1); /* sound prom */
            ROM_LOAD("gp2-3f.bin", 0x0000, 0x0100, 0x2d9fbdd8);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_galaga3()
        {
        
INPUT_PORTS_START( "galaga3" );
	PORT_START (); /* DSW0 */
	PORT_DIPNAME( 0x03, 0x00, DEF_STR( "Coin A" ) );
	PORT_DIPSETTING(    0x03, DEF_STR( "3C_1C" ) );
	PORT_DIPSETTING(    0x02, DEF_STR( "2C_1C" ) );
	PORT_DIPSETTING(    0x00, DEF_STR( "1C_1C" ) );
	PORT_DIPSETTING(    0x01, DEF_STR( "1C_2C" ) );
	PORT_DIPNAME( 0x0c, 0x00, DEF_STR( "Lives" ) );
	PORT_DIPSETTING(    0x04, "2" );
	PORT_DIPSETTING(    0x00, "3" );
	PORT_DIPSETTING(    0x08, "4" );
	PORT_DIPSETTING(    0x0c, "5" );
	PORT_DIPNAME( 0x10, 0x10, DEF_STR( "Unknown" ) );
	PORT_DIPSETTING(    0x10, DEF_STR( "Off" ) );
	PORT_DIPSETTING(    0x00, DEF_STR( "On" ) );
	PORT_DIPNAME( 0x20, 0x00, DEF_STR( "Cabinet" ) );
	PORT_DIPSETTING(    0x00, DEF_STR( "Upright" ) );
	PORT_DIPSETTING(    0x20, DEF_STR( "Cocktail" ) );
	PORT_DIPNAME( 0xc0, 0x00, DEF_STR( "Coin B" ) );
	PORT_DIPSETTING(    0xc0, DEF_STR( "3C_1C" ) );
	PORT_DIPSETTING(    0x80, DEF_STR( "2C_1C" ) );
	PORT_DIPSETTING(    0x00, DEF_STR( "1C_1C" ) );
	PORT_DIPSETTING(    0x40, DEF_STR( "1C_2C" ) );

	PORT_START (); /* DSW1 */
	PORT_DIPNAME( 0x07, 0x00, DEF_STR( "Difficulty" ) );
	PORT_DIPSETTING(    0x00, "0" );
	PORT_DIPSETTING(    0x01, "1" );
	PORT_DIPSETTING(    0x02, "2" );
	PORT_DIPSETTING(    0x03, "3" );
	PORT_DIPSETTING(    0x04, "4" );
	PORT_DIPSETTING(    0x05, "5" );
	PORT_DIPSETTING(    0x06, "6" );
	PORT_DIPSETTING(    0x07, "7" );
	PORT_SERVICE( 0x08, IP_ACTIVE_HIGH );
	PORT_BITX( 0x10,    0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test",(ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE );
	PORT_DIPSETTING(    0x00, DEF_STR( "Off" ) );
	PORT_DIPSETTING(    0x10, DEF_STR( "On" ) );
	PORT_DIPNAME( 0xe0, 0xe0, DEF_STR( "Bonus Life" ) );
	PORT_DIPSETTING(    0xa0, "30k 80k and every 100k" );
	PORT_DIPSETTING(    0x80, "30k 100k and every 100k" );
	PORT_DIPSETTING(    0x60, "30k 100k and every 150k" );
	PORT_DIPSETTING(    0x00, "30k 100k and every 200k" );
	PORT_DIPSETTING(    0x40, "30k 100k and every 300k" );
	PORT_DIPSETTING(    0xe0, "50k 150k and every 150k" );
	PORT_DIPSETTING(    0xc0, "50k 150k and every 200k" );
	PORT_DIPSETTING(    0x20, "30k 150k" );

	PORT_START();  /* IN0 */
	PORT_BIT( 0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 );
	PORT_BIT( 0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
	PORT_BIT( 0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1 );
	PORT_BIT( 0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2 );
	PORT_BIT( 0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1 );
	PORT_BIT( 0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2 );
		/* 0x40 service switch (not implemented yet) */

	PORT_START (); /* IN1 */
    PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
    PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
    PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
    PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
    PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
    PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
    PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
    PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);

return INPUT_PORTS_END;
        }
        public driver_galaga3()
        {
            drv = new machine_driver_galaga3();
            year = "1984";
            name = "galaga3";
            description = "Galaga 3 (set 1)";
            manufacturer = "Namco";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_galaga3();
            rom = rom_galaga3();
            drv.HasNVRAMhandler = false;
        }
    }
}
