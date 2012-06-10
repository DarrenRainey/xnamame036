using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_locomotn : Mame.GameDriver
    {

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters (256 in Jungler) */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },
            new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites (64 in Jungler) */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },
            new uint[]{ 8*8, 8*8+1, 8*8+2, 8*8+3, 0, 1, 2, 3,
			24*8+0, 24*8+1, 24*8+2, 24*8+3, 16*8+0, 16*8+1, 16*8+2, 16*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxLayout dotlayout =
        new Mame.GfxLayout(
            4, 4,	/* 4*4 characters */
            8,	/* 8 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 6, 7 },
            new uint[] { 3 * 8, 2 * 8, 1 * 8, 0 * 8 },
            new uint[] { 3 * 32, 2 * 32, 1 * 32, 0 * 32 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,      0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, spritelayout,    0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, dotlayout,    64*4,  1 ),
};

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9800, 0x9fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xa000, 0xa000, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0xa080, 0xa080, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0xa100, 0xa100, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( 0xa180, 0xa180, Mame.input_port_3_r ),	/* DSW */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, rallyx.rallyx_videoram2_w, rallyx.rallyx_videoram2 ),
	new Mame.MemoryWriteAddress( 0x8800, 0x8bff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x8c00, 0x8fff, rallyx.rallyx_colorram2_w, rallyx.rallyx_colorram2 ),
	new Mame.MemoryWriteAddress( 0x9800, 0x9fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa00f, Mame.MWA_RAM,rallyx.rallyx_radarattr ),
	new Mame.MemoryWriteAddress( 0xa080, 0xa080, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0xa100, 0xa100, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xa130, 0xa130, Mame.MWA_RAM, rallyx.rallyx_scrollx ),
	new Mame.MemoryWriteAddress( 0xa140, 0xa140, Mame.MWA_RAM, rallyx.rallyx_scrolly ),
	new Mame.MemoryWriteAddress( 0xa170, 0xa170, Mame.MWA_NOP ),	/* ????? */
	new Mame.MemoryWriteAddress( 0xa180, 0xa180, timeplt.timeplt_sh_irqtrigger_w ),
	new Mame.MemoryWriteAddress( 0xa181, 0xa181, Mame.interrupt_enable_w ),
//	new Mame.MemoryWriteAddress( 0xa182, 0xa182, MWA_NOP },	sound mute
	new Mame.MemoryWriteAddress( 0xa183, 0xa183, rallyx.rallyx_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0xa184, 0xa184, coin_1_w ),
	new Mame.MemoryWriteAddress( 0xa186, 0xa186, coin_2_w ),
//	new Mame.MemoryWriteAddress( 0xa187, 0xa187, MWA_NOP },	stars enable
	new Mame.MemoryWriteAddress( 0x8000, 0x801f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),	/* these are here just to initialize */
	new Mame.MemoryWriteAddress( 0x8800, 0x881f, Mame.MWA_RAM, Generic.spriteram_2 ),	/* the pointers. */
	new Mame.MemoryWriteAddress( 0x8020, 0x803f, Mame.MWA_RAM, rallyx.rallyx_radarx, rallyx.rallyx_radarram_size ),	/* ditto */
	new Mame.MemoryWriteAddress( 0x8820, 0x883f, Mame.MWA_RAM, rallyx.rallyx_radary ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static void coin_1_w(int offset, int data)
        {
            Mame.coin_counter_w(0, data & 1);
        }
        static void coin_2_w(int offset, int data)
        {
            Mame.coin_counter_w(1, data & 1);
        }
        class machine_driver_locomotn : Mame.MachineDriver
        {
            public machine_driver_locomotn()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6, readmem, writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 8, timeplt.timeplt_sound_readmem, timeplt.timeplt_sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_locomotn.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 64 * 4 + 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, timeplt.timeplt_ay8910_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint cpi = 0, pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0, bit1, bit2;


                    /* red component */
                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = (color_prom[cpi] >> 6) & 0x01;
                    bit1 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x50 * bit0 + 0xab * bit1);

                    cpi++;
                }

                /* color_prom now points to the beginning of the lookup table */

                /* character lookup table */
                /* sprites use the same color lookup table as characters */
                /* characters use colors 0-15 */
                for (int i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
                    colortable[Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i] = (ushort)(color_prom[cpi++] & 0x0f);

                /* radar dots lookup table */
                /* they use colors 16-19 */
                for (int i = 0; i < 4; i++) colortable[Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start + i] = (ushort)(16 + i);

            }
            public override int vh_start()
            {
                return rallyx.rallyx_vh_start();
            }
            public override void vh_stop()
            {
                rallyx.rallyx_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (rallyx.dirtybuffer2[offs])
                    {
                        bool flipx, flipy;

                        rallyx.dirtybuffer2[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;
                        /* not a mistake, one bit selects both  flips */
                        flipx = (rallyx.rallyx_colorram2[offs] & 0x80) != 0;
                        flipy = (rallyx.rallyx_colorram2[offs] & 0x80) != 0;
                        if (rallyx.flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(rallyx.tmpbitmap1, Mame.Machine.gfx[0],
                                (uint)((rallyx.rallyx_videoram2[offs] & 0x7f) + 2 * (rallyx.rallyx_colorram2[offs] & 0x40) + 2 * (rallyx.rallyx_videoram2[offs] & 0x80)),
                                (uint)(rallyx.rallyx_colorram2[offs] & 0x3f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                /* update radar */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        bool flipx, flipy;

                        Generic.dirtybuffer[offs] = false;

                        int sx = (offs % 32) ^ 4;
                        int sy = offs / 32 - 2;
                        /* not a mistake, one bit selects both  flips */
                        flipx = (Generic.colorram[offs] & 0x80) != 0;
                        flipy = (Generic.colorram[offs] & 0x80) != 0;
                        if (rallyx.flipscreen != 0)
                        {
                            sx = 7 - sx;
                            sy = 27 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)((Generic.videoram[offs] & 0x7f) + 2 * (Generic.colorram[offs] & 0x40) + 2 * (Generic.videoram[offs] & 0x80)),
                                (uint)(Generic.colorram[offs] & 0x3f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                rallyx.radarvisibleareaflip, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int scrollx, scrolly;


                    if (rallyx.flipscreen != 0)
                    {
                        scrollx = (rallyx.rallyx_scrollx[0]) + 32;
                        scrolly = (rallyx.rallyx_scrolly[0] + 16) - 32;
                    }
                    else
                    {
                        scrollx = -(rallyx.rallyx_scrollx[0]);
                        scrolly = -(rallyx.rallyx_scrolly[0] + 16);
                    }

                    Mame.copyscrollbitmap(bitmap, rallyx.tmpbitmap1, 1, new int[] { scrollx }, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* radar */
                if (rallyx.flipscreen != 0)
                    Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, rallyx.radarvisibleareaflip, Mame.TRANSPARENCY_NONE, 0);
                else
                    Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 28 * 8, 0, rallyx.radarvisiblearea, Mame.TRANSPARENCY_NONE, 0);


                /* draw the sprites */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    int sx = Generic.spriteram[offs + 1] - 1;
                    int sy = 224 - Generic.spriteram_2[offs];
                    if (rallyx.flipscreen != 0) sx += 32;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)(((Generic.spriteram[offs] & 0x7c) >> 2) + 0x20 * (Generic.spriteram[offs] & 0x01) + ((Generic.spriteram[offs] & 0x80) >> 1)),
                            (uint)(Generic.spriteram_2[offs + 1] & 0x3f),
                            !(rallyx.flipscreen != 0), !(rallyx.flipscreen != 0),
                            sx, sy,
                            rallyx.flipscreen != 0 ? rallyx.spritevisibleareaflip : rallyx.spritevisiblearea, Mame.TRANSPARENCY_COLOR, 0);
                }


                /* draw the cars on the radar */
                for (int offs = 0; offs < rallyx.rallyx_radarram_size[0]; offs++)
                {
                    int x, y;

                    /* it looks like the addresses used are
                       a000-a003  a004-a00f
                       8020-8023  8034-803f
                       8820-8823  8834-883f
                       so 8024-8033 and 8824-8833 are not used
                    */

                    x = rallyx.rallyx_radarx[offs] + ((~rallyx.rallyx_radarattr[offs & 0x0f] & 0x08) << 5);
                    if (rallyx.flipscreen != 0) x += 32;
                    y = 237 - rallyx.rallyx_radary[offs];

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                            (uint)((rallyx.rallyx_radarattr[offs & 0x0f] & 0x07) ^ 0x07),
                            0,
                            rallyx.flipscreen != 0, rallyx.flipscreen != 0,
                            x, y,
                        //				&Machine.drv.visible_area,TRANSPARENCY_PEN,3);
                            rallyx.flipscreen != 0 ? rallyx.spritevisibleareaflip : rallyx.spritevisiblearea, Mame.TRANSPARENCY_PEN, 3);
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            //nonethrow new NotImplementedException();
        }
        Mame.InputPortTiny[] input_ports_locomotn()
        {
            INPUT_PORTS_START("locomotn");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_BUTTON1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_START1);

            PORT_START("DSW0");
            PORT_DIPNAME(0x01, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x02, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x04, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x08, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Lives"));
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x20, "4");
            PORT_DIPSETTING(0x10, "5");
            PORT_BITX(0, 0x00, (uint)ports.inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "255", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)ports.inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_START("DSW1");
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x04, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x0a, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x08, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0x0f, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x0e, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0x0b, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0d, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x09, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x40, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x80, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0xb0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xd0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, "Disabled");
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_locomotn()
        {
            ROM_START("locomotn");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("1a.cpu", 0x0000, 0x1000, 0xb43e689a);
            ROM_LOAD("2a.cpu", 0x1000, 0x1000, 0x529c823d);
            ROM_LOAD("3.cpu", 0x2000, 0x1000, 0xc9dbfbd1);
            ROM_LOAD("4.cpu", 0x3000, 0x1000, 0xcaf6431c);
            ROM_LOAD("5.cpu", 0x4000, 0x1000, 0x64cf8dd6);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("1b_s1.bin", 0x0000, 0x1000, 0xa1105714);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("5l_c1.bin", 0x0000, 0x1000, 0x5732eda9);
            ROM_LOAD("c2.cpu", 0x1000, 0x1000, 0xc3035300);

            ROM_REGION(0x0100, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("10g.bpr", 0x0000, 0x0100, 0x2ef89356); /* dots */

            ROM_REGION(0x0160, Mame.REGION_PROMS);
            ROM_LOAD("8b.bpr", 0x0000, 0x0020, 0x75b05da0); /* palette */
            ROM_LOAD("9d.bpr", 0x0020, 0x0100, 0xaa6cf063); /* loookup table */
            ROM_LOAD("7a.bpr", 0x0120, 0x0020, 0x48c8f094); /* video layout (not used) */
            ROM_LOAD("10a.bpr", 0x0140, 0x0020, 0xb8861096); /* video timing (not used) */
            return ROM_END;
        }
        public driver_locomotn()
        {
            drv = new machine_driver_locomotn();
            year = "1982";
            name = "locomotn";
            description = "Loco-Motion";
            manufacturer = "Konami (Centuri license)";
            flags = Mame.ROT90;
            input_ports = input_ports_locomotn();
            rom = rom_locomotn();
            drv.HasNVRAMhandler = false;
        }
    }
}
