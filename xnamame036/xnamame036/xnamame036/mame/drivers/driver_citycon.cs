using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_citycon : Mame.GameDriver
    {
        static _BytePtr citycon_charlookup = new _BytePtr(1);
        static _BytePtr citycon_scroll = new _BytePtr(1);
        static Mame.osd_bitmap tmpbitmap2;
        static int bg_image, dirty_background;
        static bool[] dirtylookup = new bool[32];
        static int flipscreen;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x3000, 0x3000, citycon_in_r ),	/* player 1 & 2 inputs multiplexed */
	new Mame.MemoryReadAddress( 0x3001, 0x3001, Mame.input_port_2_r),
	new Mame.MemoryReadAddress( 0x3002, 0x3002, Mame.input_port_3_r),
	new Mame.MemoryReadAddress( 0x3007, 0x3007, Mame.watchdog_reset_r ),	/* ? */
	new Mame.MemoryReadAddress( 0x4000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1000, 0x1fff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x2000, 0x20ff, citycon_charlookup_w, citycon_charlookup ),
	new Mame.MemoryWriteAddress( 0x2800, 0x28ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, citycon_background_w ),
	new Mame.MemoryWriteAddress( 0x3001, 0x3001, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x3002, 0x3002, Mame.soundlatch2_w ),
	new Mame.MemoryWriteAddress( 0x3004, 0x3005, Mame.MWA_RAM, citycon_scroll ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3cff, Mame.paletteram_RRRRGGGGBBBBxxxx_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] readmem_sound =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_RAM ),
//	new Mame.MemoryReadAddress( 0x4002, 0x4002, YM2203_read_port_1_r },	/* ?? */
	new Mame.MemoryReadAddress( 0x6001, 0x6001, ym2203.YM2203_read_port_0_r),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM),
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, ym2203.YM2203_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0x4001, 0x4001, ym2203.YM2203_write_port_1_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x6001, 0x6001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 ) /* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },
            new uint[] { 0, 1, 2, 3, 256 * 8 * 8 + 0, 256 * 8 * 8 + 1, 256 * 8 * 8 + 2, 256 * 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 4, 0, 0xc000 * 8 + 4, 0xc000 * 8 + 0 },
            new uint[] { 0, 1, 2, 3, 256 * 8 * 8 + 0, 256 * 8 * 8 + 1, 256 * 8 * 8 + 2, 256 * 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            8, 16,	/* 8*16 sprites */
            128,	/* 128 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 4, 0, 0x2000 * 8 + 4, 0x2000 * 8 + 0 },
            new uint[] { 0, 1, 2, 3, 128 * 16 * 8 + 0, 128 * 16 * 8 + 1, 128 * 16 * 8 + 2, 128 * 16 * 8 + 3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
            8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x00000, charlayout, 512, 32 ),	/* colors 512-639 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x00000, spritelayout, 0, 16 ),	/* colors 0-255 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x01000, spritelayout, 0, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x00000, tilelayout, 256, 16 ),	/* colors 256-511 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x01000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x02000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x03000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x04000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x05000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x06000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x07000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x08000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x09000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x0a000, tilelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x0b000, tilelayout, 256, 16 ),
};



        /* actually there is one AY8910 and one YM2203, but the sound core doesn't */
        /* support that so we use 2 YM2203 */
        static YM2203interface ym2203_interface =
        new YM2203interface(
            2,			/* 2 chips */
            1250000,	/* 1.25 MHz */
            new int[] { ym2203.YM2203_VOL(20, Mame.MIXERG(20, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER)), ym2203.YM2203_VOL(20, Mame.MIXERG(20, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER)) },
            new AY8910portRead[] { Mame.soundlatch_r, null },
            new AY8910portRead[] { Mame.soundlatch2_r, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null },
            null
        );

        static int citycon_in_r(int offset)
        {
            return Mame.readinputport(flipscreen);
        }
        static void citycon_charlookup_w(int offset, int data)
        {
            if (citycon_charlookup[offset] != data)
            {
                citycon_charlookup[offset] = (byte)data;

                dirtylookup[offset / 8] = true;
            }
        }
        static void citycon_background_w(int offset, int data)
        {
            /* bits 4-7 control the background image */
            if (bg_image != (data >> 4))
            {
                bg_image = data >> 4;
                dirty_background = 1;
            }

            /* bit 0 flips screen */
            /* it is also used to multiplex player 1 and player 2 controls */
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);// memset(dirtybuffer, 1, videoram_size);
                dirty_background = 1;
            }

            /* bits 1-3 are unknown */
            //if (errorlog && (data & 0x0e) != 0) fprintf(errorlog, "background register = %02x\n", data);
        }

        class machine_driver_citycon : Mame.MachineDriver
        {
            public machine_driver_citycon()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2048000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809 | Mame.CPU_AUDIO_CPU, 640000, readmem_sound, writemem_sound, null, null, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_citycon.gfxdecodeinfo;
                total_colors = 640;
                color_table_len = 640;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                //None
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //none
            }
            public override int vh_start()
            {
                Generic.dirtybuffer = new bool[Generic.videoram_size[0]];

                Generic.SetDirtyBuffer(true);

                dirty_background = 1;

                /* CityConnection has a virtual screen 4 times as large as the visible screen */
                Generic.tmpbitmap = Mame.osd_new_bitmap(4 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

                /* And another one for background */
                tmpbitmap2 = Mame.osd_new_bitmap(4 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

                return 0;
            }
            public override void vh_stop()
            {
                Generic.dirtybuffer = null;
                Mame.osd_free_bitmap(Generic.tmpbitmap);
                Mame.osd_free_bitmap(tmpbitmap2);
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {

                Mame.palette_init_used_colors();

                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int code, color;

                    code = Mame.memory_region(Mame.REGION_GFX4)[0x1000 * bg_image + offs];
                    color = Mame.memory_region(Mame.REGION_GFX4)[0xc000 + 0x100 * bg_image + code];
                    for (int i = 0; i < 16; i++) Mame.palette_used_colors[i + 256 + 16 * color] = Mame.PALETTE_COLOR_USED;
                }
                for (int offs = 0; offs < 256; offs++)
                {
                    int color;

                    color = citycon_charlookup[offs];
                    Mame.palette_used_colors[512 + 4 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                    for (int i = 0; i < 3; i++) Mame.palette_used_colors[i + 512 + 4 * color + 1] = Mame.PALETTE_COLOR_USED; ;
                }
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int color;

                    color = Generic.spriteram[offs + 2] & 0x0f;
                    for (int i = 0; i < 15; i++) Mame.palette_used_colors[i + 16 * color + 1] = Mame.PALETTE_COLOR_USED;
                }

                if (Mame.palette_recalc() != null)
                {
                    Generic.SetDirtyBuffer(true);
                    dirty_background = 1;
                }

                /* Create the background */
                if (dirty_background != 0)
                {
                    dirty_background = 0;

                    for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                    {
                        int sx, sy, code;


                        sy = offs / 32;
                        sx = (offs % 32) + (sy & 0x60);
                        sy = sy & 31;
                        if (flipscreen != 0)
                        {
                            sx = 127 - sx;
                            sy = 31 - sy;
                        }

                        code = Mame.memory_region(Mame.REGION_GFX4)[0x1000 * bg_image + offs];

                        Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[3 + bg_image],
                                (uint)code,
                                Mame.memory_region(Mame.REGION_GFX4)[0xc000 + 0x100 * bg_image + code],
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                /* copy the temporary bitmap to the screen */
                {
                    int scroll;

                    if (flipscreen != 0)
                        scroll = 256 + ((citycon_scroll[0] * 256 + citycon_scroll[1]) >> 1);
                    else
                        scroll = -((citycon_scroll[0] * 256 + citycon_scroll[1]) >> 1);

                    Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, new int[] { scroll }, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx, sy;


                    sy = offs / 32;
                    sx = (offs % 32) + (sy & 0x60);
                    sy = sy & 0x1f;

                    if (Generic.dirtybuffer[offs] || dirtylookup[sy])
                    {
                        int i;
                        Mame.rectangle clip = new Mame.rectangle();


                        Generic.dirtybuffer[offs] = false;

                        if (flipscreen != 0)
                        {
                            sx = 127 - sx;
                            sy = 31 - sy;
                        }
                        clip.min_x = 8 * sx;
                        clip.max_x = 8 * sx + 7;

                        /* City Connection controls the color code for each _scanline_, not */
                        /* for each character as happens in most games. Therefore, we have to draw */
                        /* the character eight times, each time clipped to one line and using */
                        /* the color code for that scanline */
                        for (i = 0; i < 8; i++)
                        {
                            clip.min_y = 8 * sy + i;
                            clip.max_y = 8 * sy + i;

                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    Generic.videoram[offs],
                                    citycon_charlookup[flipscreen != 0 ? (255 - 8 * sy - i) : 8 * sy + i],
                                    flipscreen!=0, flipscreen!=0,
                                    8 * sx, 8 * sy,
                                    clip, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int i;
                    int[] scroll = new int[32];

                    if (flipscreen != 0)
                    {
                        for (i = 0; i < 6; i++)
                            scroll[31 - i] = 256;
                        for (i = 6; i < 32; i++)
                            scroll[31 - i] = 256 + (citycon_scroll[0] * 256 + citycon_scroll[1]);
                    }
                    else
                    {
                        for (i = 0; i < 6; i++)
                            scroll[i] = 0;
                        for (i = 6; i < 32; i++)
                            scroll[i] = -(citycon_scroll[0] * 256 + citycon_scroll[1]);
                    }
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 32, scroll, 0, null,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);
                }


                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int sx, sy;
                    bool flipx;


                    sx = Generic.spriteram[offs + 3];
                    sy = 239 - Generic.spriteram[offs];
                    flipx = (~Generic.spriteram[offs + 2] & 0x10) != 0;
                    if (flipscreen != 0)
                    {
                        sx = 240 - sx;
                        sy = 238 - sy;
                        flipx = !flipx;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[(Generic.spriteram[offs + 1] & 0x80) != 0 ? 2 : 1],
                            (uint)(Generic.spriteram[offs + 1] & 0x7f),
                            (uint)(Generic.spriteram[offs + 2] & 0x0f),
                            flipx, flipscreen != 0,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }


                for (int offs = 0; offs < 32; offs++)
                    dirtylookup[offs] = false;
            }

            public override void vh_eof_callback()
            {
                //nonethrow new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_citycon()
        {
            ROM_START("citycon");
            ROM_REGION(0x10000, Mame.REGION_CPU1);    /* 64k for code */
            ROM_LOAD("c10", 0x4000, 0x4000, 0xae88b53c);
            ROM_LOAD("c11", 0x8000, 0x8000, 0x139eb1aa);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("c1", 0x8000, 0x8000, 0x1fad7589);

            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("c4", 0x00000, 0x2000, 0xa6b32fc6);	/* Characters */

            ROM_REGION(0x04000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("c12", 0x00000, 0x2000, 0x08eaaccd);	/* Sprites    */
            ROM_LOAD("c13", 0x02000, 0x2000, 0x1819aafb);

            ROM_REGION(0x18000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("c9", 0x00000, 0x8000, 0x8aeb47e6);/* Background tiles */
            ROM_LOAD("c8", 0x08000, 0x4000, 0x0d7a1eeb);
            ROM_LOAD("c6", 0x0c000, 0x8000, 0x2246fe9d);
            ROM_LOAD("c7", 0x14000, 0x4000, 0xe8b97de9);

            ROM_REGION(0xe000, Mame.REGION_GFX4);	/* background tilemaps */
            ROM_LOAD("c2", 0x0000, 0x8000, 0xf2da4f23);	/* background maps */
            ROM_LOAD("c3", 0x8000, 0x4000, 0x7ef3ac1b);
            ROM_LOAD("c5", 0xc000, 0x2000, 0xc03d8b1b);/* color codes for the background */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_citycon()
        {
            INPUT_PORTS_START("citycon");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_BITX(0, 0x03, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "Infinite", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x04, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x08, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x40, DEF_STR("Cocktail"));
            /* the coin input must stay low for exactly 2 frames to be consistently recognized. */
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 2);

            PORT_START();
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x07, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x08, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x40, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, "Flip Screen?");
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        public driver_citycon()
        {
            drv = new machine_driver_citycon();
            year = "1985";
            name = "citycon";
            description = "City Connection (set 1)";
            manufacturer = "Jaleco";
            flags = Mame.ROT0;
            input_ports = input_ports_citycon();
            rom = rom_citycon();
            drv.HasNVRAMhandler = false;
        }
    }
}
