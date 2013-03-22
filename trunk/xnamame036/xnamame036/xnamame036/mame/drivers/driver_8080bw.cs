using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_8080bw : Mame.GameDriver
    {
        public static int shift_data1, shift_data2, shift_amount;

        public static Mame.MemoryReadAddress[] readmem = 
        {
    new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, Mame.MRA_ROM ),
    new Mame.MemoryReadAddress(-1)
        };
        public static Mame.MemoryWriteAddress[] writemem = 
        {
        new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x23ff,Mame. MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2400, 0x3fff, invaders_videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x4000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */};

        public static int use_tmpbitmap;
        public static int flipscreen;
        public static int screen_red;
        public static int screen_red_enabled;		/* 1 for games that can turn the screen red */
        public static int redraw_screen;
        public static int color_map_select;

        public delegate void _videoram_w_p(int offset, int data);
        public delegate void _vh_screenrefresh_p(Mame.osd_bitmap bitmap, bool full_refresh);
        public delegate void _plot_pixel_p(int x, int y, int col);

        public static _videoram_w_p videoram_w_p;
        public static _vh_screenrefresh_p vh_screenrefresh_p;
        public static _plot_pixel_p plot_pixel_p;

        public static Mame.artwork_element[] init_overlay;
        public static Mame.artwork overlay;

        static byte[] BLACK = { 0x00, 0x00, 0x00 };
        static byte[] RED = { 0xff, 0x20, 0x20 };
        static byte[] GREEN = { 0x20, 0xff, 0x20 };
        static byte[] YELLOW = { 0xff, 0xff, 0x20 };
        static byte[] WHITE = { 0xff, 0xff, 0xff };
        static byte[] CYAN = { 0x20, 0xff, 0xff };
        static byte[] PURPLE = { 0xff, 0x20, 0xff };

        static byte[] ORANGE = { 0xff, 0x90, 0x20 };
        static byte[] YELLOW_GREEN = { 0x90, 0xff, 0x20 };
        static byte[] GREEN_CYAN = { 0x20, 0xff, 0x90 };
        static Mame.artwork_element[] invaders_overlay =
{
	new Mame.artwork_element(new Mame.rectangle(	 0, 255,   0, 255), WHITE,  0xff),
	new Mame.artwork_element(new Mame.rectangle(  16,  71,   0, 255), GREEN,  0xff),
	new Mame.artwork_element(new Mame.rectangle(   0,  15,  16, 133), GREEN,  0xff),
	new Mame.artwork_element(new Mame.rectangle( 192, 223,   0, 255), RED,    0xff),
};

        static byte[] invaders_palette =
{
	0x00,0x00,0x00, /* BLACK */
	0xff,0xff,0xff, /* WHITE */
};

        public static void init_palette(byte[] game_palette, ushort[] game_colortable, _BytePtr color_prom)
        {
            Buffer.BlockCopy(invaders_palette, 0, game_palette, 0, invaders_palette.Length);
            //memcpy(game_palette,invaders_palette,sizeof(invaders_palette));
        }

        public static void init_8080bw()
        {
            videoram_w_p = bw_videoram_w;
            vh_screenrefresh_p = vh_screenrefresh;
            use_tmpbitmap = 0;
            screen_red_enabled = 0;
            init_overlay = null;
            color_map_select = 0;
            flipscreen = 0;
        }
        public static void init_invaders()
        {
            init_8080bw();
            init_overlay = invaders_overlay;
        }
        public static void invaders_screen_red_w(int data)
        {
            if (screen_red_enabled != 0 && (data != screen_red))
            {
                screen_red = data;
                redraw_screen = 1;
            }
        }
        public static void invaders_flipscreen_w(int data)
        {
            if (data != color_map_select)
            {
                color_map_select = data;
                redraw_screen = 1;
            }

            if ((Mame.input_port_3_r(0) & 0x01) != 0)
            {
                if (data != flipscreen)
                {
                    flipscreen = data;
                    redraw_screen = 1;
                }
            }
        }
        public static void invaders_videoram_w(int offset, int data)
        {
            videoram_w_p(offset, data);
        }
        public static void invaders_vh_screenrefresh(Mame.osd_bitmap bitmap, bool full_refresh)
        {
            vh_screenrefresh_p(bitmap, full_refresh);
        }
        public static int invaders_vh_start()
        {
            /* create overlay if one of was specified in init_X */
            if (init_overlay != null)
            {
                if ((overlay = Mame.artwork_create(init_overlay, 2, (int)Mame.Machine.drv.total_colors - 2)) == null)
                    return 1;

                use_tmpbitmap = 1;
            }

            if (use_tmpbitmap != 0 && (Generic.generic_bitmapped_vh_start() != 0))
                return 1;

            if (use_tmpbitmap != 0)
                plot_pixel_p = plot_pixel_8080_tmpbitmap;
            else
                plot_pixel_p = plot_pixel_8080;

            return 0;
        }
        public static void plot_pixel_8080(int x, int y, int col)
        {
            if (flipscreen != 0)
            {
                x = 255 - x;
                y = 223 - y;
            }

            Mame.plot_pixel(Mame.Machine.scrbitmap, x, y, Mame.Machine.pens[col]);
        }

        public static void plot_pixel_8080_tmpbitmap(int x, int y, int col)
        {
            if (flipscreen != 0)
            {
                x = 255 - x;
                y = 223 - y;
            }

            Mame.plot_pixel2(Mame.Machine.scrbitmap, Generic.tmpbitmap, x, y, Mame.Machine.pens[col]);
        }
        public static void invaders_vh_stop()
        {
            if (overlay != null)
            {
                Mame.artwork_free(ref overlay);
                overlay = null;
            }

            if (use_tmpbitmap != 0) Generic.generic_bitmapped_vh_stop();
        }

        public static void bw_videoram_w(int offset, int data)
        {
            int i, x, y;

            Generic.videoram[offset] = (byte)data;

            y = offset / 32;
            x = 8 * (offset % 32);

            for (i = 0; i < 8; i++)
            {
                plot_pixel_p(x, y, data & 0x01);

                x++;
                data >>= 1;
            }
        }
        public static void vh_screenrefresh(Mame.osd_bitmap bitmap, bool full_refresh)
        {
            if (Mame.palette_recalc() != null || redraw_screen != 0 || (full_refresh && use_tmpbitmap == 0))
            {
                int offs;

                for (offs = 0; offs < Generic.videoram_size[0]; offs++)
                    videoram_w_p(offs, Generic.videoram[offs]);

                redraw_screen = 0;

                if (overlay != null)
                    Mame.overlay_remap(overlay);
            }


            if (full_refresh && use_tmpbitmap != 0)
                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

            if (overlay != null)
                Mame.overlay_draw(bitmap, overlay);
        }

        public override void driver_init()
        {
            //none. Init is called explicit from specific game driver init
        }
    }
    class driver_boothill : driver_8080bw
    {
        public static Mame.IOReadPort[] boothill_readport =
{
	new Mame.IOReadPort( 0x00, 0x00, boothill_port_0_r ),
	new Mame.IOReadPort( 0x01, 0x01, boothill_port_1_r ),
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r ),
	new Mame.IOReadPort( 0x03, 0x03, boothill_shift_data_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        static Mame.IOWritePort[] boothill_writeport =
{
	new Mame.IOWritePort( 0x01, 0x01, driver_invaders.invaders_shift_amount_w ),
	new Mame.IOWritePort( 0x02, 0x02, driver_invaders.invaders_shift_data_w ),
	new Mame.IOWritePort( 0x03, 0x03, boothill_sh_port3_w ),
	new Mame.IOWritePort( 0x05, 0x05, boothill_sh_port5_w ),
	new Mame.IOWritePort( -1 )  /* end of table */
};
        static string[] boothill_sample_names =
{
	"*boothill", /* in case we ever find any bootlegs hehehe */
	"addcoin.wav",
	"endgame.wav",
	"gunshot.wav",
	"killed.wav",
	null      /* end of array */
}; 

        static Mame.Samplesinterface boothill_samples_interface =
new Mame.Samplesinterface(
    9,	/* 9 channels */
    25,	/* volume */
    boothill_sample_names
);

        static void boothill_sh_port3_w(int offset, int data)
        {
            switch (data)
            {
                case 0x0c:
                    Mame.sample_start(0, 0, false);
                    break;

                case 0x18:
                case 0x28:
                    Mame.sample_start(1, 2, false);
                    break;

                case 0x48:
                case 0x88:
                    Mame.sample_start(2, 3, false);
                    break;
            }
        }

        /* HC 4/14/98 */
        static void boothill_sh_port5_w(int offset, int data)
        {
            switch (data)
            {
                case 0x3b:
                    Mame.sample_start(2, 1, false);
                    break;
            }
        }

        static int boothill_shift_data_r(int offset)
        {
            if (driver_8080bw.shift_amount < 0x10)
                return driver_invaders.invaders_shift_data_r(0);
            else
                return driver_invaders.invaders_shift_data_rev_r(0);
        }

        /* Grays Binary again! */

        static int[] BootHillTable = {
	0x00, 0x40, 0x60, 0x70, 0x30, 0x10, 0x50, 0x50
};

        static int boothill_port_0_r(int offset)
        {
            return (Mame.input_port_0_r(0) & 0x8F) | BootHillTable[Mame.input_port_3_r(0) >> 5];
        }

        static int boothill_port_1_r(int offset)
        {
            return (Mame.input_port_1_r(0) & 0x8F) | BootHillTable[Mame.input_port_4_r(0) >> 5];
        }
        class machine_driver_boothill : Mame.MachineDriver
        {
            public machine_driver_boothill()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_8080, 2000000, driver_8080bw.readmem, driver_8080bw.writemem, driver_boothill.boothill_readport, driver_boothill.boothill_writeport, driver_invaders.invaders_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = null;
                total_colors = 256;
                color_table_len = 0;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, driver_boothill.boothill_samples_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                driver_8080bw.init_palette(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return driver_8080bw.invaders_vh_start();
            }
            public override void vh_stop()
            {
                driver_8080bw.invaders_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_8080bw.invaders_vh_screenrefresh(bitmap, full_refresh != 0);
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            init_8080bw();
        }
        public driver_boothill()
        {
            drv = new machine_driver_boothill();
            year = "1977";
            name = "boothill";
            description = "Boot Hill";
            manufacturer = "Midway";
            flags = Mame.ROT0;
            input_ports = input_ports_boothill();
            rom = rom_boothill();
            drv.HasNVRAMhandler = false;
        }
        Mame.InputPortTiny[] input_ports_boothill()
        {
            INPUT_PORTS_START("boothill");
            /* Gun position uses bits 4-6, handled using fake paddles */
            PORT_START("IN0 - Player 2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);       /* Move Man */
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2); /* Fire */

            PORT_START("IN1 - Player 1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);/* Move Man */
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);/* Fire */

            PORT_START("IN2 Dips & Coins");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            //	PORT_DIPSETTING(    0x03, DEF_STR( "1C_2C" ) );
            PORT_DIPNAME(0x0c, 0x00, "Time");
            PORT_DIPSETTING(0x00, "64");
            PORT_DIPSETTING(0x04, "74");
            PORT_DIPSETTING(0x08, "84");
            PORT_DIPSETTING(0x0C, "94");
            PORT_SERVICE(0x10, IP_ACTIVE_HIGH);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();                                                                                       /* Player 2 Gun */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_PADDLE | IPF_PLAYER2, 50, 10, 1, 255, IP_KEY_NONE, IP_KEY_NONE, IP_JOY_NONE, IP_JOY_NONE);

            PORT_START();                                                                                          /* Player 1 Gun */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_PADDLE, 50, 10, 1, 255, (ushort)Mame.InputCodes.KEYCODE_Z, (ushort)Mame.InputCodes.KEYCODE_A, IP_JOY_NONE, IP_JOY_NONE);
            return INPUT_PORTS_END;

        }
        Mame.RomModule[] rom_boothill()
        {
            ROM_START("boothill");
            ROM_REGION(0x10000, Mame.REGION_CPU1);     /* 64k for code */
            ROM_LOAD("romh.cpu", 0x0000, 0x0800, 0x1615d077);
            ROM_LOAD("romg.cpu", 0x0800, 0x0800, 0x65a90420);
            ROM_LOAD("romf.cpu", 0x1000, 0x0800, 0x3fdafd79);
            ROM_LOAD("rome.cpu", 0x1800, 0x0800, 0x374529f4);
            return ROM_END;
        }
    }
    class driver_invaders : driver_8080bw
    {


        static Mame.IOReadPort[] readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r ),
	new Mame.IOReadPort( 0x01, 0x01, Mame.input_port_1_r ),
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r ),
	new Mame.IOReadPort( 0x03, 0x03, invaders_shift_data_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        static Mame.IOWritePort[] writeport =
{
	new Mame.IOWritePort( 0x02, 0x02, invaders_shift_amount_w ),
	new Mame.IOWritePort( 0x03, 0x03, invaders_sh_port3_w ),
	new Mame.IOWritePort( 0x04, 0x04, invaders_shift_data_w ),
	new Mame.IOWritePort( 0x05, 0x05, invaders_sh_port5_w ),
	new Mame.IOWritePort( -1 )  /* end of table */
};

        //static int SHIFT() { return (((((shift_data1 << 8) | shift_data2) << (shift_amount & 0x07)) >> 8) & 0xff); }
        public static void invaders_shift_amount_w(int offset, int data)
        {
            shift_amount = data;
        }
        public static int invaders_shift_data_rev_r(int offset)
        {
            int ret = (((((shift_data1 << 8) | shift_data2) << (shift_amount & 0x07)) >> 8) & 0xff);

            ret = ((ret & 0x01) << 7)
                | ((ret & 0x02) << 5)
                | ((ret & 0x04) << 3)
                | ((ret & 0x08) << 1)
                | ((ret & 0x10) >> 1)
                | ((ret & 0x20) >> 3)
                | ((ret & 0x40) >> 5)
                | ((ret & 0x80) >> 7);

            return ret;
        }
        public static void invaders_shift_data_w(int offset, int data)
        {
            shift_data2 = shift_data1;
            shift_data1 = data;
        }
        static byte Sound3 = 0;
        static void invaders_sh_port3_w(int offset, int data)
        {

            if ((data & 0x01) != 0 && (~Sound3 & 0x01) != 0)
                Mame.sample_start(0, 0, true);

            if ((~data & 0x01) != 0 && (Sound3 & 0x01) != 0)
                Mame.sample_stop(0);

            if ((data & 0x02) != 0 && (~Sound3 & 0x02) != 0)
                Mame.sample_start(1, 1, false);

            if ((data & 0x04) != 0 && (~Sound3 & 0x04) != 0)
                Mame.sample_start(2, 2, false);

            if ((~data & 0x04) != 0 && (Sound3 & 0x04) != 0)
                Mame.sample_stop(2);

            if ((data & 0x08) != 0 && (~Sound3 & 0x08) != 0)
                Mame.sample_start(3, 3, false);

            if ((data & 0x10) != 0 && (~Sound3 & 0x10) != 0)
                Mame.sample_start(4, 9, false);

            invaders_screen_red_w(data & 0x04);

            Sound3 = (byte)data;
        }
        static byte Sound5 = 0;
        static void invaders_sh_port5_w(int offset, int data)
        {

            if ((data & 0x01) != 0 && (~Sound5 & 0x01) != 0)
                Mame.sample_start(5, 4, false);			/* Fleet 1 */

            if ((data & 0x02) != 0 && (~Sound5 & 0x02) != 0)
                Mame.sample_start(5, 5, false);			/* Fleet 2 */

            if ((data & 0x04) != 0 && (~Sound5 & 0x04) != 0)
                Mame.sample_start(5, 6, false);			/* Fleet 3 */

            if ((data & 0x08) != 0 && (~Sound5 & 0x08) != 0)
                Mame.sample_start(5, 7, false);			/* Fleet 4 */

            if ((data & 0x10) != 0 && (~Sound5 & 0x10) != 0)
                Mame.sample_start(6, 8, false);			/* Saucer Hit */

            invaders_flipscreen_w(data & 0x20);

            Sound5 = (byte)data;
        }
        public static int invaders_shift_data_r(int offset)
        {
            return (((((shift_data1 << 8) | shift_data2) << (shift_amount & 0x07)) >> 8) & 0xff);
        }
        public static string[] invaders_sample_names = {
                                                "*invaders",

/* these are used in invaders, invadpt2, invdpt2m, and others */
	"0.wav",	/* UFO/Saucer */
	"1.wav",	/* Shot/Missle */
	"2.wav",	/* Base Hit/Explosion */
	"3.wav",	/* Invader Hit */
	"4.wav",	/* Fleet move 1 */
	"5.wav",	/* Fleet move 2 */
	"6.wav",	/* Fleet move 3 */
	"7.wav",	/* Fleet move 4 */
	"8.wav",	/* UFO/Saucer Hit */
	"9.wav",	/* Bonus Base */

/* these are only use by invad2ct */
	"0.wav",	/* UFO/Saucer - Player 2 */
	"11.wav",	/* Shot/Missle - Player 2 */
	"12.wav",	/* Base Hit/Explosion - Player 2 */
	"13.wav",	/* Invader Hit - Player 2 */
	"14.wav",	/* Fleet move 1 - Player 2 */
	"15.wav",	/* Fleet move 2 - Player 2 */
	"16.wav",	/* Fleet move 3 - Player 2 */
	"17.wav",	/* Fleet move 4 - Player 2 */
	"18.wav",	/* UFO/Saucer Hit - Player 2 */
    null,
                                                };
        static Mame.Samplesinterface samples_interface = new Mame.Samplesinterface(13, 25, invaders_sample_names);

        static int count;
        public static int invaders_interrupt()
        {
            count++;

            if ((count & 1) != 0)
                return 0x00cf;  /* RST 08h */
            else
                return 0x00d7;  /* RST 10h */
        }
        class machine_driver_invaders : Mame.MachineDriver
        {
            public machine_driver_invaders()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_8080, 2000000, readmem, writemem, readport, writeport, invaders_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = null; // no gfxdecodeinfo - bitmapped display
                total_colors = 256;
                color_table_len = 0;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
            }
            public override void init_machine()
            {
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //none
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                init_palette(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return invaders_vh_start();
            }
            public override void vh_stop()
            {
                invaders_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                invaders_vh_screenrefresh(bitmap, full_refresh != 0);
            }
        }
        Mame.InputPortTiny[] input_ports_invaders()
        {
            INPUT_PORTS_START("invaders");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_START1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPSETTING(0x03, "6");
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_TILT);
            PORT_DIPNAME(0x08, 0x00, ipdn_defaultstrings["Bonus Life"]);
            PORT_DIPSETTING(0x08, "1000");
            PORT_DIPSETTING(0x00, "1500");
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_PLAYER2);
            PORT_DIPNAME(0x80, 0x00, "Coin Info");
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);

            PORT_START();		/* Dummy port for cocktail mode */
            PORT_DIPNAME(0x01, 0x00, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["Cocktail"]);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_invaders()
        {
            ROM_START("invaders");
            ROM_REGION(0x10000, Mame.REGION_CPU1);     /* 64k for code */
            ROM_LOAD("invaders.h", 0x0000, 0x0800, 0x734f5ad8);
            ROM_LOAD("invaders.g", 0x0800, 0x0800, 0x6bfaca4a);
            ROM_LOAD("invaders.f", 0x1000, 0x0800, 0x0ccead96);
            ROM_LOAD("invaders.e", 0x1800, 0x0800, 0x14e538b0);
            return ROM_END;
        }
        public driver_invaders()
        {
            drv = new machine_driver_invaders();
            year = "1978";
            name = "invaders";
            description = "Space Invaders";
            manufacturer = "Midway";
            flags = Mame.ROT270;
            input_ports = input_ports_invaders();
            rom = rom_invaders();
        }
        public override void driver_init()
        {
            init_invaders();
        }
    }

    class driver_gunfight : driver_8080bw
    {
       
        static Mame.IOWritePort[] gunfight_writeport =
{
	new Mame.IOWritePort(0x02, 0x02, driver_invaders.invaders_shift_amount_w ),
	new Mame.IOWritePort(0x04, 0x04, driver_invaders.invaders_shift_data_w ),
	new Mame.IOWritePort(-1 )  /* end of table */
};

        static Mame.Samplesinterface samples_interface = new Mame.Samplesinterface(13, 25, driver_invaders.invaders_sample_names);
        
        class machine_driver_gunfight : Mame.MachineDriver
        {
            public machine_driver_gunfight()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_8080, 2000000, readmem, writemem, driver_boothill.boothill_readport, gunfight_writeport, driver_invaders.invaders_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = null;
                total_colors = 256;
                color_table_len = 0;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES,samples_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                driver_invaders.init_palette(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return invaders_vh_start();
            }
            public override void vh_stop()
            {
                invaders_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                invaders_vh_screenrefresh(bitmap, full_refresh != 0);
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }


        Mame.InputPortTiny[] input_ports_gunfight()
        {
            INPUT_PORTS_START("gunfight");
            /* Gun position uses bits 4-6, handled using fake paddles */
            PORT_START("IN0 - Player 2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);       /* Move Man */
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);            /* Fire */

            PORT_START("IN1 - Player 1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);              /* Move Man */
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);                    /* Fire */

#if NOTDEF
	PORT_START      /* IN2 Dips & Coins */
	PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 )
	PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START1 )
	PORT_DIPNAME( 0x0C, 0x00, "Plays" )
	PORT_DIPSETTING(    0x00, "1" )
	PORT_DIPSETTING(    0x04, "2" )
	PORT_DIPSETTING(    0x08, "3" )
	PORT_DIPSETTING(    0x0C, "4" )
	PORT_DIPNAME( 0x30, 0x00, "Time" ) /* These are correct */
	PORT_DIPSETTING(    0x00, "60" )
	PORT_DIPSETTING(    0x10, "70" )
	PORT_DIPSETTING(    0x20, "80" )
	PORT_DIPSETTING(    0x30, "90" )
	PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Coinage ) )
	PORT_DIPSETTING(    0x00, "1 Coin - 1 Player" )
	PORT_DIPSETTING(    0x40, "1 Coin - 2 Players" )
	PORT_DIPSETTING(    0x80, "1 Coin - 3 Players" )
	PORT_DIPSETTING(    0xc0, "1 Coin - 4 Players" )
#endif

            PORT_START("IN2 Dips & Coins");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x00, "1 Coin");
            PORT_DIPSETTING(0x01, "2 Coins");
            PORT_DIPSETTING(0x02, "3 Coins");
            PORT_DIPSETTING(0x03, "4 Coins");
            PORT_DIPNAME(0x0C, 0x00, "Plays");
            PORT_DIPSETTING(0x00, "1");
            PORT_DIPSETTING(0x04, "2");
            PORT_DIPSETTING(0x08, "3");
            PORT_DIPSETTING(0x0C, "4");
            PORT_DIPNAME(0x30, 0x00, "Time"); /* These are correct */
            PORT_DIPSETTING(0x00, "60");
            PORT_DIPSETTING(0x10, "70");
            PORT_DIPSETTING(0x20, "80");
            PORT_DIPSETTING(0x30, "90");
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);

            PORT_START();                                                                                          /* Player 2 Gun */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_PADDLE | IPF_PLAYER2, 50, 10, 1, 255, IP_KEY_NONE, IP_KEY_NONE, IP_JOY_NONE, IP_JOY_NONE);

            PORT_START();                                                                                          /* Player 1 Gun */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_PADDLE, 50, 10, 1, 255, (ushort)Mame.InputCodes.KEYCODE_Z, (ushort)Mame.InputCodes.KEYCODE_A, IP_JOY_NONE, IP_JOY_NONE);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_gunfight()
        {
            ROM_START("gunfight");
            ROM_REGION(0x10000, Mame.REGION_CPU1);   /* 64k for code */
            ROM_LOAD("7609h.bin", 0x0000, 0x0400, 0x0b117d73);
            ROM_LOAD("7609g.bin", 0x0400, 0x0400, 0x57bc3159);
            ROM_LOAD("7609f.bin", 0x0800, 0x0400, 0x8049a6bd);
            ROM_LOAD("7609e.bin", 0x0c00, 0x0400, 0x773264e2);
            return ROM_END;
        }
        public override void driver_init()
        {
            init_8080bw();
        }
        public driver_gunfight()
        {
            drv = new machine_driver_gunfight();
            year = "1975";
            name = "gunfight";
            description = "Gun Fight";
            manufacturer = "Midway";
            flags = Mame.ROT0;
            input_ports = input_ports_gunfight();
            rom = rom_gunfight();
            drv.HasNVRAMhandler = false;
        }
    }
}
