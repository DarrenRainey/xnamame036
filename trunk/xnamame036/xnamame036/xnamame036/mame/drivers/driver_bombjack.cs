using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_bombjack : Mame.GameDriver
    {

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x97ff, Mame.MRA_RAM ),	/* including video and color RAM */
	new Mame.MemoryReadAddress( 0xb000, 0xb000, Mame.input_port_0_r ),	/* player 1 input */
	new Mame.MemoryReadAddress( 0xb001, 0xb001, Mame.input_port_1_r ),	/* player 2 input */
	new Mame.MemoryReadAddress( 0xb002, 0xb002, Mame.input_port_2_r ),	/* coin */
	new Mame.MemoryReadAddress( 0xb003, 0xb003, Mame.MRA_NOP ),	/* watchdog reset? */
	new Mame.MemoryReadAddress( 0xb004, 0xb004, Mame.input_port_3_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0xb005, 0xb005, Mame.input_port_4_r ),	/* DSW2 */
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x9000, 0x93ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x9400, 0x97ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x9820, 0x987f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x9a00, 0x9a00, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0x9c00, 0x9cff, Mame.paletteram_xxxxBBBBGGGGRRRR_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x9e00, 0x9e00, bombjack_background_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb000, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0xb004, 0xb004, bombjack_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0xb800, 0xb800, bombjack_soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdfff, Mame.MWA_ROM),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] bombjack_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x43ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, bombjack_soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] bombjack_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff,Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff,Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};


        static Mame.IOWritePort[] bombjack_sound_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, ay8910.AY8910_control_port_0_w ),
	new Mame.IOWritePort( 0x01, 0x01, ay8910.AY8910_write_port_0_w ),
	new Mame.IOWritePort( 0x10, 0x10, ay8910.AY8910_control_port_1_w ),
	new Mame.IOWritePort( 0x11, 0x11, ay8910.AY8910_write_port_1_w ),
	new Mame.IOWritePort( 0x80, 0x80, ay8910.AY8910_control_port_2_w ),
	new Mame.IOWritePort( 0x81, 0x81, ay8910.AY8910_write_port_2_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout1 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 512 * 8 * 8, 2 * 512 * 8 * 8 },	/* the bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout charlayout2 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 characters */
            256,	/* 256 characters */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 1024 * 8 * 8, 2 * 1024 * 8 * 8 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,	/* pretty straightforward layout */
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8	/* every character takes 32 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout1 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 1024 * 8 * 8, 2 * 1024 * 8 * 8 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout2 =
        new Mame.GfxLayout(
            32, 32,	/* 32*32 sprites */
            32,	/* 32 sprites */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 1024 * 8 * 8, 2 * 1024 * 8 * 8 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7,
			32*8+0, 32*8+1, 32*8+2, 32*8+3, 32*8+4, 32*8+5, 32*8+6, 32*8+7,
			40*8+0, 40*8+1, 40*8+2, 40*8+3, 40*8+4, 40*8+5, 40*8+6, 40*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8,
			64*8, 65*8, 66*8, 67*8, 68*8, 69*8, 70*8, 71*8,
			80*8, 81*8, 82*8, 83*8, 84*8, 85*8, 86*8, 87*8 },
            128 * 8	/* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, charlayout1,      0, 16 ),	/* characters */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000, charlayout2,      0, 16 ),	/* background tiles */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x0000, spritelayout1,    0, 16 ),	/* normal sprites */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x1000, spritelayout2,    0, 16 ),	/* large sprites */
};



        static AY8910interface ay8910_interface =
        new AY8910interface(
            3,	/* 3 chips */
            1500000,	/* 1.5 MHz?????? */
            new int[] { 13, 13, 13 },
            new AY8910portRead[] { null, null, null },
            new AY8910portRead[] { null, null, null },
            new AY8910portWrite[] { null, null, null },
            new AY8910portWrite[] { null, null, null },
            null
        );

        static int latch;

        static void soundlatch_callback(int param)
        {
            latch = param;
        }
        static void bombjack_soundlatch_w(int offset, int data)
        {
            /* make all the CPUs synchronize, and only AFTER that write the new command to the latch */
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, data, soundlatch_callback);
        }
        static int bombjack_soundlatch_r(int offset)
        {
            int res = latch;
            latch = 0;
            return res;
        }

        static int background_image;
        static int flipscreen;

        static void bombjack_background_w(int offset, int data)
        {
            if (background_image != data)
            {
                Generic.SetDirtyBuffer(true);
                background_image = data;
            }
        }
        static void bombjack_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        class machine_driver_bombjack : Mame.MachineDriver
        {
            public machine_driver_bombjack()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3072000, bombjack_sound_readmem, bombjack_sound_writemem, null, bombjack_sound_writeport, Mame.nmi_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_bombjack.gfxdecodeinfo;
                total_colors = 128;
                color_table_len = 128;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                //none
            }
            public override int vh_start()
            {
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int offs, _base;

                if (Mame.palette_recalc() != null) Generic.SetDirtyBuffer(true);

                _base = 0x200 * (background_image & 0x07);

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int tilecode = 0, tileattribute = 0xff;

                    int sx = offs % 32;
                    int sy = offs / 32;

                    if ((background_image & 0x10) != 0)
                    {
                        int bgoffs;


                        bgoffs = _base + 16 * (sy / 2) + sx / 2;

                        tilecode = Mame.memory_region(Mame.REGION_GFX4)[bgoffs];
                        tileattribute = Mame.memory_region(Mame.REGION_GFX4)[bgoffs + 0x100];
                    }

                    if (Generic.dirtybuffer[offs])
                    {
                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        /* draw the background (this can be handled better) */
                        if (tilecode != 0xff)
                        {
                            Mame.rectangle clip = new Mame.rectangle();

                            clip.min_x = 8 * sx;
                            clip.max_x = 8 * sx + 7;
                            clip.min_y = 8 * sy;
                            clip.max_y = 8 * sy + 7;

                            bool flipy = (tileattribute & 0x80) != 0;
                            if (flipscreen != 0) flipy = !flipy;

                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[1],
                                    (uint)tilecode,
                                    (uint)(tileattribute & 0x0f),
                                    flipscreen != 0, flipy,
                                    16 * (sx / 2), 16 * (sy / 2),
                                    clip, Mame.TRANSPARENCY_NONE, 0);

                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    (uint)(Generic.videoram[offs] + 16 * (Generic.colorram[offs] & 0x10)),
                                    (uint)(Generic.colorram[offs] & 0x0f),
                                    flipscreen != 0, flipscreen != 0,
                                    8 * sx, 8 * sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        else
                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    (uint)(Generic.videoram[offs] + 16 * (Generic.colorram[offs] & 0x10)),
                                    (uint)(Generic.colorram[offs] & 0x0f),
                                    flipscreen != 0, flipscreen != 0,
                                    8 * sx, 8 * sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                        Generic.dirtybuffer[offs] = false;
                    }
                }


                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. */
                for (offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {

                    /*
                     abbbbbbb cdefgggg hhhhhhhh iiiiiiii

                     a        use big sprites (32x32 instead of 16x16)
                     bbbbbbb  sprite code
                     c        x flip
                     d        y flip (used only in death sequence?)
                     e        ? (set when big sprites are selected)
                     f        ? (set only when the bonus (B) materializes?)
                     gggg     color
                     hhhhhhhh x position
                     iiiiiiii y position
                    */
                    int sx, sy;
                    bool flipx, flipy;


                    sx = Generic.spriteram[offs + 3];
                    if ((Generic.spriteram[offs] & 0x80) != 0)
                        sy = 225 - Generic.spriteram[offs + 2];
                    else
                        sy = 241 - Generic.spriteram[offs + 2];
                    flipx = (Generic.spriteram[offs + 1] & 0x40) != 0;
                    flipy = (Generic.spriteram[offs + 1] & 0x80) != 0;
                    if (flipscreen != 0)
                    {
                        if ((Generic.spriteram[offs + 1] & 0x20) != 0)
                        {
                            sx = 224 - sx;
                            sy = 224 - sy;
                        }
                        else
                        {
                            sx = 240 - sx;
                            sy = 240 - sy;
                        }
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[(Generic.spriteram[offs] & 0x80) != 0 ? 3 : 2],
                            (uint)(Generic.spriteram[offs] & 0x7f),
                            (uint)(Generic.spriteram[offs + 1] & 0x0f),
                            flipx, flipy,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
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
        Mame.RomModule[] rom_bombjack()
        {
            ROM_START("bombjack");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("09_j01b.bin", 0x0000, 0x2000, 0xc668dc30);
            ROM_LOAD("10_l01b.bin", 0x2000, 0x2000, 0x52a1e5fb);
            ROM_LOAD("11_m01b.bin", 0x4000, 0x2000, 0xb68a062a);
            ROM_LOAD("12_n01b.bin", 0x6000, 0x2000, 0x1d3ecee5);
            ROM_LOAD("13.1r", 0xc000, 0x2000, 0x70e0244d);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for sound board */
            ROM_LOAD("01_h03t.bin", 0x0000, 0x2000, 0x8407917d);

            ROM_REGION(0x3000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("03_e08t.bin", 0x0000, 0x1000, 0x9f0470d5);/* chars */
            ROM_LOAD("04_h08t.bin", 0x1000, 0x1000, 0x81ec12e6);
            ROM_LOAD("05_k08t.bin", 0x2000, 0x1000, 0xe87ec8b1);

            ROM_REGION(0x6000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("06_l08t.bin", 0x0000, 0x2000, 0x51eebd89);/* background tiles */
            ROM_LOAD("07_n08t.bin", 0x2000, 0x2000, 0x9dd98e9d);
            ROM_LOAD("08_r08t.bin", 0x4000, 0x2000, 0x3155ee7d);

            ROM_REGION(0x6000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("16_m07b.bin", 0x0000, 0x2000, 0x94694097);/* sprites */
            ROM_LOAD("15_l07b.bin", 0x2000, 0x2000, 0x013f58f2);
            ROM_LOAD("14_j07b.bin", 0x4000, 0x2000, 0x101c858d);

            ROM_REGION(0x1000, Mame.REGION_GFX4);	/* background tilemaps */
            ROM_LOAD("02_p04t.bin", 0x0000, 0x1000, 0x398d4a02);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_bombjack()
        {
            INPUT_PORTS_START("bombjack");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0xf0, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x30, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x30, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x20, "5");
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x40, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x00, "Initial High Score?");
            PORT_DIPSETTING(0x00, "10000");
            PORT_DIPSETTING(0x01, "100000");
            PORT_DIPSETTING(0x02, "30000");
            PORT_DIPSETTING(0x03, "50000");
            PORT_DIPSETTING(0x04, "100000");
            PORT_DIPSETTING(0x05, "50000");
            PORT_DIPSETTING(0x06, "100000");
            PORT_DIPSETTING(0x07, "50000");
            PORT_DIPNAME(0x18, 0x00, "Bird Speed");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Medium");
            PORT_DIPSETTING(0x10, "Hard");
            PORT_DIPSETTING(0x18, "Hardest");
            PORT_DIPNAME(0x60, 0x00, "Enemies Number & Speed");
            PORT_DIPSETTING(0x20, "Easy");
            PORT_DIPSETTING(0x00, "Medium");
            PORT_DIPSETTING(0x40, "Hard");
            PORT_DIPSETTING(0x60, "Hardest");
            PORT_DIPNAME(0x80, 0x00, "Special Coin");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x80, "Hard");
            return INPUT_PORTS_END;
        }
        public driver_bombjack()
        {
            drv = new machine_driver_bombjack();
            year = "1984";
            name = "bombjack";
            description = "Bomb Jack (set 1)";
            manufacturer = "Tehkan";
            flags = Mame.ROT90;
            input_ports = input_ports_bombjack();
            rom = rom_bombjack();
            drv.HasNVRAMhandler = false;
        }
    }
}
