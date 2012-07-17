using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_troangel : Mame.GameDriver
    {
        static _BytePtr troangel_scroll = new _BytePtr(1);
        static int flipscreen;

        static Mame.MemoryReadAddress[] troangel_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9000, 0x90ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xd000, 0xd000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xd001, 0xd001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xd002, 0xd002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0xd003, 0xd003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xd004, 0xd004, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xe7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] troangel_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
//	new Mame.MemoryWriteAddress( 0x8800, 0x8fff, MWA_RAM },
	new Mame.MemoryWriteAddress( 0x9000, 0x91ff, Mame.MWA_RAM, troangel_scroll ),
	new Mame.MemoryWriteAddress( 0xc820, 0xc8ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, irem.irem_sound_cmd_w ),
	new Mame.MemoryWriteAddress( 0xd001, 0xd001, troangel_flipscreen_w ),	/* + coin counters */
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8, /* character size */
            1024, /* number of characters */
            3, /* bits per pixel */
            new uint[] { 0, 1024 * 8 * 8, 2 * 1024 * 8 * 8 },
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* character offset */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 32, /* sprite size */
            64, /* number of sprites */
            3, /* bits per pixel */
            new uint[] { 0, 0x4000 * 8, 2 * 0x4000 * 8 },
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8,
			256*64+0*8, 256*64+1*8, 256*64+2*8, 256*64+3*8, 256*64+4*8, 256*64+5*8, 256*64+6*8, 256*64+7*8,
			256*64+8*8, 256*64+9*8, 256*64+10*8, 256*64+11*8, 256*64+12*8, 256*64+13*8, 256*64+14*8, 256*64+15*8 },
            32 * 8	/* character offset */
        );

        static Mame.GfxDecodeInfo[] troangel_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, charlayout,      0, 32 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, spritelayout, 32*8, 32 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x1000, spritelayout, 32*8, 32 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x2000, spritelayout, 32*8, 32 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x3000, spritelayout, 32*8, 32 ),
};
        static void troangel_flipscreen_w(int offset, int data)
        {
            /* screen flip is handled both by software and hardware */
            data ^= ~Mame.readinputport(4) & 1;

            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                //memset(dirtybuffer, 1, videoram_size);
                Generic.SetDirtyBuffer(true);
            }

            Mame.coin_counter_w(0, data & 0x02);
            Mame.coin_counter_w(1, data & 0x20);
        }
        class machine_driver_troangel : Mame.MachineDriver
        {
            public machine_driver_troangel()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3000000, troangel_readmem, troangel_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 57;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 1 * 8, 31 * 8 - 1);
                total_colors = 32 * 8 + 16;
                color_table_len = 32 * 8 + 32 * 8;
                gfxdecodeinfo = troangel_gfxdecodeinfo;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < 256; i++)
                {
                    /* red component */
                    int bit0 = 0;
                    int bit1 = (color_prom[cpi + 256] >> 2) & 0x01;
                    int bit2 = (color_prom[cpi + 256] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi + 256] >> 0) & 0x01;
                    bit2 = (color_prom[cpi + 256] >> 1) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    COLOR(colortable, 0, i, i);
                    cpi++;
                }

                cpi += 256;
                /* color_prom now points to the beginning of the sprite palette */

                /* sprite palette */
                for (int i = 0; i < 16; i++)
                {
                    /* red component */
                    int bit0 = 0;
                    int bit1 = (color_prom[cpi] >> 6) & 0x01;
                    int bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                cpi += 16;
                /* color_prom now points to the beginning of the sprite lookup table */


                /* sprite lookup table */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    COLOR(colortable, 1, i, 256 + (~color_prom[cpi] & 0x0f));
                    cpi++;
                }
            }
            public override int vh_start()
            {
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            static void draw_background(Mame.osd_bitmap bitmap)
            {
                Mame.GfxElement gfx = Mame.Machine.gfx[0];

                for (int offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    if (Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1])
                    {
                        Generic.dirtybuffer[offs] = Generic.dirtybuffer[offs + 1] = false;

                        int sx = (offs / 2) % 32;
                        int sy = (offs / 2) / 32;

                        int attr = Generic.videoram[offs];
                        int code = Generic.videoram[offs + 1] + ((attr & 0xc0) << 2);
                        bool flipx = (attr & 0x20) != 0;

                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            flipx = !flipx;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, gfx,
                            (uint)code,
                            (uint)attr & 0x1f,
                            flipx, flipscreen != 0,
                            8 * sx, 8 * sy,
                            null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                {
                    int[] xscroll = new int[256];

                    if (flipscreen != 0)
                    {
                        /* fixed */
                        for (int offs = 0; offs < 64; offs++) xscroll[255 - offs] = 0;

                        /* scroll (wraps around) */
                        for (int offs = 64; offs < 128; offs++) xscroll[255 - offs] = troangel_scroll[64];

                        /* linescroll (no wrap) */
                        for (int offs = 128; offs < 256; offs++) xscroll[255 - offs] = troangel_scroll[offs];
                    }
                    else
                    {
                        /* fixed */
                        for (int offs = 0; offs < 64; offs++) xscroll[offs] = 0;

                        /* scroll (wraps around) */
                        for (int offs = 64; offs < 128; offs++) xscroll[offs] = -troangel_scroll[64];

                        /* linescroll (no wrap) */
                        for (int offs = 128; offs < 256; offs++) xscroll[offs] = -troangel_scroll[offs];
                    }

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 256, xscroll, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }

            static void draw_sprites(Mame.osd_bitmap bitmap)
            {
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    byte attributes = Generic.spriteram[offs + 1];
                    int sx = Generic.spriteram[offs + 3];
                    int sy = ((224 - Generic.spriteram[offs + 0] - 32) & 0xff) + 32;
                    int code = Generic.spriteram[offs + 2];
                    int color = attributes & 0x1f;
                    bool flipy = (attributes & 0x80) != 0;
                    bool flipx = (attributes & 0x40) != 0;

                    int tile_number = code & 0x3f;

                    int bank = 0;
                    if ((code & 0x80) != 0) bank += 1;
                    if ((attributes & 0x20) != 0) bank += 2;

                    if (flipscreen != 0)
                    {
                        sx = 240 - sx;
                        sy = 224 - sy;
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1 + bank],
                        (uint)tile_number,
                        (uint)color,
                        flipx, flipy,
                        sx, sy,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                draw_background(bitmap);
                draw_sprites(bitmap);
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
        Mame.RomModule[] rom_troangel()
        {
            ROM_START("troangel");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* main CPU */
            ROM_LOAD("ta-a-3k", 0x0000, 0x2000, 0xf21f8196);
            ROM_LOAD("ta-a-3m", 0x2000, 0x2000, 0x58801e55);
            ROM_LOAD("ta-a-3n", 0x4000, 0x2000, 0xde3dea44);
            ROM_LOAD("ta-a-3q", 0x6000, 0x2000, 0xfff0fc2a);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* sound CPU */
            ROM_LOAD("ta-s-1a", 0xe000, 0x2000, 0x15a83210);

            ROM_REGION(0x06000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ta-a-3c", 0x00000, 0x2000, 0x7ff5482f);	/* characters */
            ROM_LOAD("ta-a-3d", 0x02000, 0x2000, 0x06eef241);
            ROM_LOAD("ta-a-3e", 0x04000, 0x2000, 0xe49f7ad8);

            ROM_REGION(0x0c000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ta-b-5j", 0x00000, 0x2000, 0x86895c0c);	/* sprites */
            ROM_LOAD("ta-b-5h", 0x02000, 0x2000, 0xf8cff29d);
            ROM_LOAD("ta-b-5e", 0x04000, 0x2000, 0x8b21ee9a);
            ROM_LOAD("ta-b-5d", 0x06000, 0x2000, 0xcd473d47);
            ROM_LOAD("ta-b-5c", 0x08000, 0x2000, 0xc19134c9);
            ROM_LOAD("ta-b-5a", 0x0a000, 0x2000, 0x0012792a);

            ROM_REGION(0x0320, Mame.REGION_PROMS);
            ROM_LOAD("ta-a-5a", 0x0000, 0x0100, 0x01de1167); /* chars palette low 4 bits */
            ROM_LOAD("ta-a-5b", 0x0100, 0x0100, 0xefd11d4b); /* chars palette high 4 bits */
            ROM_LOAD("ta-b-1b", 0x0200, 0x0020, 0xf94911ea); /* sprites palette */
            ROM_LOAD("ta-b-3d", 0x0220, 0x0100, 0xed3e2aa4); /* sprites lookup table */
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_troangel()
        {
            INPUT_PORTS_START("troangel");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            /* coin input must be active for 19 frames to be consistently recognized */
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3, 19);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2 | IPF_COCKTAIL); ;
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, "Time");
            PORT_DIPSETTING(0x03, "180 160 140");
            PORT_DIPSETTING(0x02, "160 140 120");
            PORT_DIPSETTING(0x01, "140 120 100");
            PORT_DIPSETTING(0x00, "120 100 100");
            PORT_DIPNAME(0x04, 0x04, "Crash Loss Time");
            PORT_DIPSETTING(0x04, "5");
            PORT_DIPSETTING(0x00, "10");
            PORT_DIPNAME(0x08, 0x08, "Background Sound");
            PORT_DIPSETTING(0x08, "Boat Motor");
            PORT_DIPSETTING(0x00, "Music");
            /* TODO: support the different settings which happen in Coin Mode 2 */
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR(Coinage)); /* mapped on coin mode 1 */
            PORT_DIPSETTING(0xa0, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0xb0, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xd0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            /* settings 0x10, 0x20, 0x80, 0x90 all give 1 Coin/1 Credit */

            PORT_START();	/* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x02, DEF_STR(Cocktail));
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            /* TODO: the following enables an analog accelerator input read from 0xd003 */
            /* however that is the DSW1 input so it must be multiplexed some way */
            PORT_DIPNAME(0x08, 0x08, "Analog Accelarator");
            PORT_DIPSETTING(0x08, DEF_STR(No));
            PORT_DIPSETTING(0x00, DEF_STR(Yes));
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x10, 0x10, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Stop Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Unknown));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BITX(0x40, 0x40, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        public driver_troangel()
        {
            drv = new machine_driver_troangel();
            year = "1983";
            name = "troangel";
            description = "Tropical Angel";
            manufacturer = "Irem";
            flags = Mame.ROT0;
            input_ports = input_ports_troangel();
            rom = rom_troangel();
            drv.HasNVRAMhandler = false;
        }
    }
}
