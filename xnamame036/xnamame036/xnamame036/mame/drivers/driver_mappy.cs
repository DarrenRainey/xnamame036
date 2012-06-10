using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_mappy : Mame.GameDriver
    {
        static _BytePtr mappy_sharedram = new _BytePtr(1);
        static _BytePtr mappy_customio_1 = new _BytePtr(1), mappy_customio_2 = new _BytePtr(1);
        static byte interrupt_enable_1, interrupt_enable_2;
        static int credits, coin, start1, start2, io_chip_1_enabled, io_chip_2_enabled;
        static byte mappy_scroll;
        static int special_display, flipscreen;


        /* CPU 1 read addresses */
        static Mame.MemoryReadAddress[] mappy_readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x4040, 0x43ff, Mame.MRA_RAM ),			/* shared RAM with the sound CPU */
	new Mame.MemoryReadAddress( 0x4800, 0x480f, mappy_customio_r_1 ),	/* custom I/O chip #1 interface */
	new Mame.MemoryReadAddress( 0x4810, 0x481f, mappy_customio_r_2 ),	/* custom I/O chip #2 interface */
	new Mame.MemoryReadAddress( 0x0000, 0x9fff, Mame.MRA_RAM),			/* RAM everywhere else */
	new Mame.MemoryReadAddress( 0xa000, 0xffff,Mame.MRA_ROM ),			/* ROM code */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};
        /* CPU 2 read addresses */
        static Mame.MemoryReadAddress[] mappy_readmem_cpu2 =
{
	new Mame.MemoryReadAddress (0xe000, 0xffff, Mame.MRA_ROM ),                                 /* ROM code */
	new Mame.MemoryReadAddress (0x0040, 0x03ff, mappy_sharedram_r2 ),                      /* shared RAM with the main CPU */
	new Mame.MemoryReadAddress (-1 )  /* end of table */
};

        /* CPU 1 write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress( 0x1000, 0x177f,Mame.MWA_RAM ),                                 /* general RAM, area 1 */
	new Mame.MemoryWriteAddress( 0x1800, 0x1f7f,Mame.MWA_RAM ),                                 /* general RAM, area 2 */
	new Mame.MemoryWriteAddress( 0x2000, 0x277f,Mame.MWA_RAM ),                                 /* general RAM, area 3 */
	new Mame.MemoryWriteAddress( 0x4040, 0x43ff,Mame.MWA_RAM, mappy_sharedram ),               /* shared RAM with the sound CPU */
	new Mame.MemoryWriteAddress( 0x4820, 0x4bff,Mame.MWA_RAM ),                                 /* extra RAM for Dig Dug 2 */
	new Mame.MemoryWriteAddress( 0x0000, 0x07ff, mappy_videoram_w, Generic.videoram, Generic.videoram_size ),/* video RAM */
	new Mame.MemoryWriteAddress( 0x0800, 0x0fff, mappy_colorram_w, Generic.colorram ),             /* color RAM */
	new Mame.MemoryWriteAddress( 0x1780, 0x17ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),    /* sprite RAM, area 1 */
	new Mame.MemoryWriteAddress( 0x1f80, 0x1fff, Mame.MWA_RAM, Generic.spriteram_2 ),                   /* sprite RAM, area 2 */
	new Mame.MemoryWriteAddress( 0x2780, 0x27ff, Mame.MWA_RAM, Generic.spriteram_3 ),                   /* sprite RAM, area 3 */
	new Mame.MemoryWriteAddress( 0x3800, 0x3fff, mappy_scroll_w ),                          /* scroll registers */
	new Mame.MemoryWriteAddress( 0x4800, 0x480f, mappy_customio_w_1, mappy_customio_1 ),   /* custom I/O chip #1 interface */
	new Mame.MemoryWriteAddress( 0x4810, 0x481f, mappy_customio_w_2, mappy_customio_2 ),   /* custom I/O chip #2 interface */
	new Mame.MemoryWriteAddress( 0x5002, 0x5003, mappy_interrupt_enable_1_w ),              /* interrupt enable */
	new Mame.MemoryWriteAddress( 0x5004, 0x5005, mappy_flipscreen_w ),				 /* cocktail flipscreen */
	new Mame.MemoryWriteAddress( 0x5008, 0x5008, mappy_reset_2_w ),			       /* reset CPU #2 & disable I/O chips */
	new Mame.MemoryWriteAddress( 0x5009, 0x5009, mappy_io_chips_enable_w ),		       /* enable I/O chips */
	new Mame.MemoryWriteAddress( 0x500a, 0x500b, mappy_cpu_enable_w ),                      /* sound CPU enable */
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, Mame.MWA_NOP ),                                 /* watchdog timer */
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),                                 /* ROM code */

	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        /* CPU 2 write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	 new Mame.MemoryWriteAddress( 0x0040, 0x03ff, mappy_sharedram_w ),                       /* shared RAM with the main CPU */
	 new Mame.MemoryWriteAddress( 0x0000, 0x003f, Namco.mappy_sound_w, Namco.namco_soundregs ),         /* sound control registers */
	 new Mame.MemoryWriteAddress( 0x2000, 0x2001, mappy_interrupt_enable_2_w ),              /* interrupt enable */
	 new Mame.MemoryWriteAddress( 0x2006, 0x2007, Namco.mappy_sound_enable_w ),                    /* sound enable */
	 new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),                                 /* ROM code */
    
	 new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
    8, 8,             /* 8*8 characters */
    256,             /* 256 characters */
    2,             /* 2 bits per pixel */
    new uint[] { 0, 4 },      /* the two bitplanes for 4 pixels are packed into one byte */
    new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },   /* bits are packed in groups of four */
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },   /* characters are rotated 90 degrees */
    16 * 8           /* every char takes 16 bytes */
);

        /* layout of the 16x16x4 sprite data */
        static Mame.GfxLayout mappy_spritelayout =
        new Mame.GfxLayout(
            16, 16,       /* 16*16 sprites */
            128,            /* 128 sprites */
            4,                 /* 4 bits per pixel */
            new uint[] { 0, 4, 8192 * 8, 8192 * 8 + 4 },     /* the two bitplanes for 4 pixels are packed into one byte */
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3, 16*8+0, 16*8+1, 16*8+2, 16*8+3,
			24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8    /* every sprite takes 64 bytes */
        );
        static Mame.GfxDecodeInfo[] mappy_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,            0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, mappy_spritelayout, 64*4, 16 ),
};
        static Namco_interface namco_interface =
        new Namco_interface(
            23920,	/* sample rate (approximate value) */
            8,		/* number of voices */
            100,	/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );



        static int mappy_sharedram_r(int offset)
        {
            return mappy_sharedram[offset];
        }
        static int mappy_sharedram_r2(int offset)
        {
            /* to speed up emulation, we check for the loop the sound CPU sits in most of the time
               and end the current iteration (things will start going again with the next IRQ) */
            if (offset == 0x010a - 0x40 && mappy_sharedram[offset] == 0)
                Mame.cpu_spinuntil_int();
            return mappy_sharedram[offset];
        }
        static void mappy_sharedram_w(int offset, int data)
        {
            mappy_sharedram[offset] = (byte)data;
        }
        static void mappy_customio_w_1(int offset, int data)
        {
            mappy_customio_1[offset] = (byte)data;
        }
        static void mappy_customio_w_2(int offset, int data)
        {
            mappy_customio_2[offset] = (byte)data;
        }
        static void mappy_reset_2_w(int offset, int data)
        {
            io_chip_1_enabled = io_chip_2_enabled = 0;
            Mame.cpu_set_reset_line(1, Mame.PULSE_LINE);
        }

        static void mappy_io_chips_enable_w(int offset, int data)
        {
            io_chip_1_enabled = io_chip_2_enabled = 1;
        }
        static int lastval1;
        static int lastval2;
        static int mappy_customio_r_1(int offset)
        {
            int[] crednum = { 1, 2, 3, 6, 1, 3, 1, 2 };
            int[] credden = { 1, 1, 1, 1, 2, 2, 3, 3 };
            int val, temp, mode = mappy_customio_1[8];

            //if (errorlog)fprintf (errorlog, "I/O read 1: mode %d offset %d\n", mode, offset);

            /* mode 3 is the standard, and returns actual important values */
            if (mode == 1 || mode == 3)
            {
                switch (offset)
                {
                    case 0:		/* Coin slots, low nibble of port 4 */
                        {

                            val = Mame.readinputport(4) & 0x0f;

                            /* bit 0 is a trigger for the coin slot */
                            if ((val & 1) != 0 && ((val ^ lastval1) & 1) != 0) ++credits;

                            return lastval1 = val;
                        }

                    case 1:		/* Start buttons, high nibble of port 4 */
                        {

                            temp = Mame.readinputport(1) & 7;
                            val = Mame.readinputport(4) >> 4;

                            /* bit 0 is a trigger for the 1 player start */
                            if ((val & 1) != 0 && ((val ^ lastval2) & 1) != 0)
                            {
                                if (credits >= credden[temp]) credits -= credden[temp];
                                else val &= ~1;	/* otherwise you can start with no credits! */
                            }
                            /* bit 1 is a trigger for the 2 player start */
                            if ((val & 2) != 0 && ((val ^ lastval2) & 2) != 0)
                            {
                                if (credits >= 2 * credden[temp]) credits -= 2 * credden[temp];
                                else val &= ~2;	/* otherwise you can start with no credits! */
                            }

                            return lastval2 = val;
                        }

                    case 2:		/* High BCD of credits */
                        temp = Mame.readinputport(1) & 7;
                        return (credits * crednum[temp] / credden[temp]) / 10;

                    case 3:		/* Low BCD of credits */
                        temp = Mame.readinputport(1) & 7;
                        return (credits * crednum[temp] / credden[temp]) % 10;

                    case 4:		/* Player 1 joystick */
                        return Mame.readinputport(3) & 0x0f;

                    case 5:		/* Player 1 buttons */
                        return Mame.readinputport(3) >> 4;

                    case 6:		/* Player 2 joystick */
                        return Mame.readinputport(5) & 0x0f;

                    case 7:		/* Player 2 joystick */
                        return Mame.readinputport(5) >> 4;
                }
            }

            /* mode 5 values are actually checked against these numbers during power up */
            else if (mode == 5)
            {
                int[] testvals = { 8, 4, 6, 14, 13, 9, 13 };
                if (offset >= 1 && offset <= 7)
                    return testvals[offset - 1];
            }

            /* by default, return what was stored there */
            return mappy_customio_1[offset];
        }


        static int mappy_customio_r_2(int offset)
        {
            int mode = mappy_customio_2[8];

            //if (errorlog)fprintf (errorlog, "I/O read 2: mode %d, offset %d\n", mappy_customio_2[8], offset);

            /* mode 4 is the standard, and returns actual important values */
            if (mode == 4)
            {
                switch (offset)
                {
                    case 0:		/* DSW1, low nibble */
                        return Mame.readinputport(1) & 0x0f;

                    case 1:		/* DSW1, high nibble */
                        return Mame.readinputport(1) >> 4;

                    case 2:		/* DSW0, low nibble */
                        return Mame.readinputport(0) & 0x0f;

                    case 4:		/* DSW0, high nibble */
                        return Mame.readinputport(0) >> 4;

                    case 6:		/* DSW2 - service switch */
                        return Mame.readinputport(2) & 0x0f;

                    case 3:		/* read, but unknown */
                    case 5:		/* read, but unknown */
                    case 7:		/* read, but unknown */
                        return 0;
                }
            }

            /* mode 5 values are actually checked against these numbers during power up */
            else if (mode == 5)
            {
                int[] testvals = { 8, 4, 6, 14, 13, 9, 13 };
                if (offset >= 1 && offset <= 7)
                    return testvals[offset - 1];
            }

            /* by default, return what was stored there */
            return mappy_customio_2[offset];
        }

        static void mappy_interrupt_enable_1_w(int offset, int data)
        {
            interrupt_enable_1 = (byte)offset;
        }
        static int mappy_interrupt_1()
        {
            if (interrupt_enable_1 != 0) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void mappy_interrupt_enable_2_w(int offset, int data)
        {
            interrupt_enable_2 = (byte)offset;
        }
        static int mappy_interrupt_2()
        {
            if (interrupt_enable_2 != 0) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void mappy_cpu_enable_w(int offset, int data)
        {
            Mame.cpu_set_halt_line(1, offset != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }

        static void mappy_videoram_w(int offset, int data)
        {
            if (Generic.videoram[offset] != data)
            {
                Generic.dirtybuffer[offset] = true;
                Generic.videoram[offset] = (byte)data;
            }
        }
        static void mappy_colorram_w(int offset, int data)
        {
            if (Generic.colorram[offset] != data)
            {
                Generic.dirtybuffer[offset] = true;
                Generic.colorram[offset] = (byte)data;
            }
        }
        static void mappy_scroll_w(int offset, int data)
        {
            mappy_scroll = (byte)(offset >> 3);
        }


        static void mappy_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

        class machine_driver_mappy : Mame.MachineDriver
        {
            public machine_driver_mappy()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1100000, mappy_readmem_cpu1, writemem_cpu1, null, null, mappy_interrupt_1, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 110000, mappy_readmem_cpu2, writemem_cpu2, null, null, mappy_interrupt_2, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = mappy_gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 64 * 4 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
            }
            public override void init_machine()
            {
                /* Reset all flags */
                credits = coin = start1 = start2 = 0;
                interrupt_enable_1 = interrupt_enable_2 = 0;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0, bit1, bit2;

                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                /* characters */
                for (int i = 0 * 4; i < 64 * 4; i++)
                    colortable[i]= (ushort)((color_prom[cpi + (uint)(i ^ 3)] & 0x0f) + 0x10);

                /* sprites */
                for (int i = 64 * 4; i < Mame.Machine.drv.color_table_len; i++)
                    colortable[i]= (ushort)(color_prom[cpi + (uint)i] & 0x0f);
            }
            static int common_vh_start()
            {
                Generic.dirtybuffer = new bool[Generic.videoram_size[0]];
                Generic.SetDirtyBuffer(true);

                Generic.tmpbitmap = Mame.osd_create_bitmap(36 * 8, 60 * 8);

                return 0;
            }
            public override int vh_start()
            {
                special_display = 0;
                return common_vh_start();
            }
            public override void vh_stop()
            {
                Generic.dirtybuffer = null;
                Mame.osd_free_bitmap(Generic.tmpbitmap);
            }
            static void mappy_draw_sprite(Mame.osd_bitmap dest, uint code, uint color, bool flipx, bool flipy, int sx, int sy)
            {
                if (special_display == 1) sy++;	/* Motos */

                Mame.drawgfx(dest, Mame.Machine.gfx[1], code, color, flipx, flipy, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 15);
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy, mx, my;

                        Generic.dirtybuffer[offs] = false;

                        if (offs >= Generic.videoram_size[0] - 64)
                        {
                            int off = offs;

                            if (special_display == 1)
                            {
                                /* Motos */
                                if (off == 0x07d1 || off == 0x07d0 || off == 0x07f1 || off == 0x07f0)
                                    off -= 0x10;
                                if (off == 0x07c1 || off == 0x07c0 || off == 0x07e1 || off == 0x07e0)
                                    off += 0x10;
                            }

                            /* Draw the top 2 lines. */
                            mx = (off - (Generic.videoram_size[0] - 64)) / 32;
                            my = off % 32;

                            sx = mx;
                            sy = my - 2;
                        }
                        else if (offs >= Generic.videoram_size[0] - 128)
                        {
                            int off = offs;

                            if (special_display == 2)
                            {
                                /* Tower of Druaga */
                                if (off == 0x0791 || off == 0x0790 || off == 0x07b1 || off == 0x07b0)
                                    off -= 0x10;
                                if (off == 0x0781 || off == 0x0780 || off == 0x07a1 || off == 0x07a0)
                                    off += 0x10;
                            }

                            /* Draw the bottom 2 lines. */
                            mx = (off - (Generic.videoram_size[0] - 128)) / 32;
                            my = off % 32;

                            sx = mx + 34;
                            sy = my - 2;
                        }
                        else
                        {
                            /* draw the rest of the screen */
                            mx = offs % 32;
                            my = offs / 32;

                            sx = mx + 2;
                            sy = my;
                        }

                        if (flipscreen != 0)
                        {
                            sx = 35 - sx;
                            sy = 59 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                Generic.videoram[offs],
                                (uint)(Generic.colorram[offs] & 0x3f),
                                flipscreen != 0, flipscreen != 0, 8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                /* copy the temporary bitmap to the screen */
                {
                    int[] scroll = new int[36];

                    for (int offs = 0; offs < 2; offs++)
                        scroll[offs] = 0;
                    for (int offs = 2; offs < 34; offs++)
                        scroll[offs] = -mappy_scroll;
                    for (int offs = 34; offs < 36; offs++)
                        scroll[offs] = 0;

                    if (flipscreen != 0)
                    {
                        for (int offs = 0; offs < 36; offs++)
                            scroll[offs] = 224 - scroll[offs];
                    }

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 36, scroll, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                /* Draw the sprites. */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    /* is it on? */
                    if ((Generic.spriteram_3[offs + 1] & 2) == 0)
                    {
                        uint sprite = Generic.spriteram[offs];
                        uint color = Generic.spriteram[offs + 1];
                        int x = (Generic.spriteram_2[offs + 1] - 40) + 0x100 * (Generic.spriteram_3[offs + 1] & 1);
                        int y = 28 * 8 - Generic.spriteram_2[offs];
                        bool flipx = (Generic.spriteram_3[offs] & 1) != 0;
                        bool flipy = (Generic.spriteram_3[offs] & 2) != 0;

                        if (flipscreen != 0)
                        {
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        switch (Generic.spriteram_3[offs] & 0x0c)
                        {
                            case 0:		/* normal size */
                                mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                break;

                            case 4:		/* 2x horizontal */
                                sprite &= unchecked((uint)~1);
                                if (!flipx)
                                {
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y);
                                }
                                else
                                {
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y);
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y);
                                }
                                break;

                            case 8:		/* 2x vertical */
                                sprite &= unchecked((uint)~2);
                                if (!flipy)
                                {
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y - 16);
                                }
                                else
                                {
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y - 16);
                                }
                                break;

                            case 12:		/* 2x both ways */
                                sprite &= unchecked((uint)~3);
                                if (!flipx && !flipy)
                                {
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x + 16, y);
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y - 16);
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y - 16);
                                }
                                else if (flipx && flipy)
                                {
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y);
                                    mappy_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x, y - 16);
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x + 16, y - 16);
                                }
                                else if (flipy)
                                {
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x + 16, y);
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x, y - 16);
                                    mappy_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x + 16, y - 16);
                                }
                                else /* flipx */
                                {
                                    mappy_draw_sprite(bitmap, 3 + sprite, color, flipx, flipy, x, y);
                                    mappy_draw_sprite(bitmap, 2 + sprite, color, flipx, flipy, x + 16, y);
                                    mappy_draw_sprite(bitmap, 1 + sprite, color, flipx, flipy, x, y - 16);
                                    mappy_draw_sprite(bitmap, sprite, color, flipx, flipy, x + 16, y - 16);
                                }
                                break;
                        }
                    }
                }

                /* Draw the high priority characters */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if ((Generic.colorram[offs] & 0x40) != 0)
                    {
                        int sx, sy, mx, my;

                        if (offs >= Generic.videoram_size[0] - 64)
                        {
                            /* Draw the top 2 lines. */
                            mx = (offs - (Generic.videoram_size[0] - 64)) / 32;
                            my = offs % 32;

                            sx = mx;
                            sy = my - 2;

                            sy *= 8;
                        }
                        else if (offs >= Generic.videoram_size[0] - 128)
                        {
                            /* Draw the bottom 2 lines. */
                            mx = (offs - (Generic.videoram_size[0] - 128)) / 32;
                            my = offs % 32;

                            sx = mx + 34;
                            sy = my - 2;

                            sy *= 8;
                        }
                        else
                        {
                            /* draw the rest of the screen */
                            mx = offs % 32;
                            my = offs / 32;

                            sx = mx + 2;
                            sy = my;

                            sy = (8 * sy - mappy_scroll);
                        }

                        if (flipscreen != 0)
                        {
                            sx = 35 - sx;
                            sy = 216 - sy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                Generic.videoram[offs],
                                (uint)(Generic.colorram[offs] & 0x3f),
                                flipscreen != 0, flipscreen != 0, 8 * sx, sy,
                                null, Mame.TRANSPARENCY_COLOR, 31);
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //nonethrow new NotImplementedException();
            }
        }
        Mame.RomModule[] rom_mappy()
        {

            ROM_START("mappy");
            ROM_REGION(0x10000, Mame.REGION_CPU1);   /* 64k for code for the first CPU  */
            ROM_LOAD("mappy1d.64", 0xa000, 0x2000, 0x52e6c708);
            ROM_LOAD("mappy1c.64", 0xc000, 0x2000, 0xa958a61c);
            ROM_LOAD("mappy1b.64", 0xe000, 0x2000, 0x203766d4);

            ROM_REGION(0x10000, Mame.REGION_CPU2);  /* 64k for the second CPU */
            ROM_LOAD("mappy1k.64", 0xe000, 0x2000, 0x8182dd5b);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mappy3b.32", 0x0000, 0x1000, 0x16498b9f);

            ROM_REGION(0x4000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mappy3m.64", 0x0000, 0x2000, 0xf2d9647a);
            ROM_LOAD("mappy3n.64", 0x2000, 0x2000, 0x757cf2b6);

            ROM_REGION(0x0220, Mame.REGION_PROMS);
            ROM_LOAD("mappy.pr1", 0x0000, 0x0020, 0x56531268);/* palette */
            ROM_LOAD("mappy.pr2", 0x0020, 0x0100, 0x50765082); /* characters */
            ROM_LOAD("mappy.pr3", 0x0120, 0x0100, 0x5396bd78); /* sprites */

            ROM_REGION(0x0100, Mame.REGION_SOUND1);	/* sound prom */
            ROM_LOAD("mappy.spr", 0x0000, 0x0100, 0x16a9166a);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_mappy()
        {
            INPUT_PORTS_START("mappy");
            PORT_START("DSW0");
            /* According to the manual, 0x04, 0x08 and 0x10 should always be off,
            but... */
            PORT_DIPNAME(0x07, 0x00, "Rank");
            PORT_DIPSETTING(0x00, "A");
            PORT_DIPSETTING(0x01, "B");
            PORT_DIPSETTING(0x02, "C");
            PORT_DIPSETTING(0x03, "D");
            PORT_DIPSETTING(0x04, "E");
            PORT_DIPSETTING(0x05, "F");
            PORT_DIPSETTING(0x06, "G");
            PORT_DIPSETTING(0x07, "H");
            PORT_DIPNAME(0x18, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x18, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_7C"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x40, 0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x40, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, "Freeze");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x06, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            /* TODO: bonus scores are different for 5 lives */
            PORT_DIPNAME(0x38, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x28, "20k 70k and every 70k");
            PORT_DIPSETTING(0x30, "20k 80k and every 80k");
            PORT_DIPSETTING(0x08, "20k 60k");
            PORT_DIPSETTING(0x00, "20k 70k");
            PORT_DIPSETTING(0x10, "20k 80k");
            PORT_DIPSETTING(0x18, "30k 100k");
            PORT_DIPSETTING(0x20, "20k");
            PORT_DIPSETTING(0x38, "None");
            /* those are the bonus with 5 lives
                PORT_DIPNAME( 0x38, 0x00, DEF_STR( Bonus_Life ) )
                PORT_DIPSETTING(    0x28, "30k 100k and every 100k" )
                PORT_DIPSETTING(    0x30, "40k 120k and every 120k" )
                PORT_DIPSETTING(    0x00, "30k 80k" )
                PORT_DIPSETTING(    0x08, "30k 100k" )
                PORT_DIPSETTING(    0x10, "30k 120k" )
                PORT_DIPSETTING(    0x18, "30k" )
                PORT_DIPSETTING(    0x20, "40k" )
                PORT_DIPSETTING(    0x38, "None" ) */
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x80, "1");
            PORT_DIPSETTING(0xc0, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x40, "5");

            PORT_START("DSW2");
            PORT_BIT(0x03, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_SERVICE(0x08, IP_ACTIVE_HIGH);
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("FAKE");
            /* The player inputs are not memory mapped, they are handled by an I/O chip. */
            /* These fake input ports are read by mappy_customio_data_r() */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("FAKE");
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1, 1);
            /* Coin 2 is not working */
            PORT_BIT_IMPULSE(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2, 1);
            PORT_BIT(0x0c, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2, 1);
            PORT_BIT(0xc0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("FAKE");
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, 1);
            PORT_BITX(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);

            return INPUT_PORTS_END;
        }
        public override void driver_init()
        {
            //None
        }
        public driver_mappy()
        {
            drv = new machine_driver_mappy();
            year = "1983";
            name = "mappy";
            description = "Mappy (US)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_mappy();
            rom = rom_mappy();
            drv.HasNVRAMhandler = false;
        }
    }
}
