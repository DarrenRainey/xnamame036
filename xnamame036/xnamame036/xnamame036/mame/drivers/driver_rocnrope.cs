using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_rocnrope : Mame.GameDriver
    {
        static int flipscreen;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x3080, 0x3080, Mame.input_port_0_r ), /* IO Coin */
	new Mame.MemoryReadAddress( 0x3081, 0x3081, Mame.input_port_1_r ), /* P1 IO */
	new Mame.MemoryReadAddress( 0x3082, 0x3082, Mame.input_port_2_r ), /* P2 IO */
	new Mame.MemoryReadAddress( 0x3083, 0x3083, Mame.input_port_3_r ), /* DSW 0 */
	new Mame.MemoryReadAddress( 0x3000, 0x3000, Mame.input_port_4_r ), /* DSW 1 */
	new Mame.MemoryReadAddress( 0x3100, 0x3100, Mame.input_port_5_r ), /* DSW 2 */
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x4000, 0x403f, Mame.MWA_RAM, Generic.spriteram_2 ),
	new Mame.MemoryWriteAddress( 0x4040, 0x43ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4400, 0x443f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x4440, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4800, 0x4bff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x4c00, 0x4fff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x5000, 0x5fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x8080, 0x8080, rocnrope_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x8081, 0x8081, timeplt.timeplt_sh_irqtrigger_w ),  /* cause interrupt on audio CPU */
	new Mame.MemoryWriteAddress( 0x8082, 0x8082, Mame.MWA_NOP ),	/* interrupt acknowledge??? */
	new Mame.MemoryWriteAddress( 0x8083, 0x8083, Mame.MWA_NOP ),	/* Coin counter 1 */
	new Mame.MemoryWriteAddress( 0x8084, 0x8084, Mame.MWA_NOP ),	/* Coin counter 2 */
	new Mame.MemoryWriteAddress( 0x8087, 0x8087, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x8100, 0x8100, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x8182, 0x818d, rocnrope_interrupt_vector_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static void rocnrope_interrupt_vector_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            RAM[0xFFF2 + offset] = (byte)data;
        }
        static void rocnrope_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (~data & 1))
            {
                flipscreen = ~data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 sprites */
            512,	/* 512 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0x2000 * 8 + 4, 0x2000 * 8 + 0, 4, 0 },
            new uint[] { 0, 1, 2, 3, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 256 * 64 * 8 + 4, 256 * 64 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,       0, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, spritelayout, 16*16, 16 ),
};


        class machine_driver_rocnrope : Mame.MachineDriver
        {
            public machine_driver_rocnrope()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2048000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 7, timeplt.timeplt_sound_readmem, timeplt.timeplt_sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_rocnrope.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 16 * 16 + 16 * 16;
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
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[i++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                /* color_prom now points to the beginning of the lookup table */

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable, 1, i, color_prom[cpi++] & 0x0f);

                /* characters */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable, 0, i, color_prom[cpi++] & 0x0f);
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
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;
                        bool flipx = (Generic.colorram[offs] & 0x40) != 0;
                        bool flipy = (Generic.colorram[offs] & 0x20) != 0;
                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 2 * (Generic.colorram[offs] & 0x80)),
                                (uint)(Generic.colorram[offs] & 0x0f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                /* copy the temporary bitmap to the screen */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                /* Draw the sprites. */
                for (int offs = Generic.spriteram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            Generic.spriteram[offs + 1],
                            (uint)(Generic.spriteram_2[offs] & 0x0f),
                            (Generic.spriteram_2[offs] & 0x40) != 0, (~Generic.spriteram_2[offs] & 0x80) != 0,
                            240 - Generic.spriteram[offs], Generic.spriteram_2[offs + 1],
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0);
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            _BytePtr rom = Mame.memory_region(Mame.REGION_CPU1);
            int diff = Mame.memory_region_length(Mame.REGION_CPU1) / 2;

            Konami.konami1_decode();
            rom[0x703d + diff] = 0x98;	/* fix one instruction */
        }
        Mame.RomModule[] rom_rocnrope()
        {
            ROM_START("rocnrope");
            ROM_REGION(2 * 0x10000, Mame.REGION_CPU1);     /* 64k for code + 64k for decrypted opcodes */
            ROM_LOAD("rr1.1h", 0x6000, 0x2000, 0x83093134);
            ROM_LOAD("rr2.2h", 0x8000, 0x2000, 0x75af8697);
            ROM_LOAD("rr3.3h", 0xa000, 0x2000, 0xb21372b1);
            ROM_LOAD("rr4.4h", 0xc000, 0x2000, 0x7acb2a05);
            ROM_LOAD("rnr_h5.vid", 0xe000, 0x2000, 0x150a6264);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("rnr_7a.snd", 0x0000, 0x1000, 0x75d2c4e2);
            ROM_LOAD("rnr_8a.snd", 0x1000, 0x1000, 0xca4325ae);

            ROM_REGION(0x4000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("rnr_h12.vid", 0x0000, 0x2000, 0xe2114539);
            ROM_LOAD("rnr_h11.vid", 0x2000, 0x2000, 0x169a8f3f);

            ROM_REGION(0x8000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("rnr_a11.vid", 0x0000, 0x2000, 0xafdaba5e);
            ROM_LOAD("rnr_a12.vid", 0x2000, 0x2000, 0x054cafeb);
            ROM_LOAD("rnr_a9.vid", 0x4000, 0x2000, 0x9d2166b2);
            ROM_LOAD("rnr_a10.vid", 0x6000, 0x2000, 0xaff6e22f);

            ROM_REGION(0x0220, Mame.REGION_PROMS);
            ROM_LOAD("a17_prom.bin", 0x0000, 0x0020, 0x22ad2c3e);
            ROM_LOAD("b16_prom.bin", 0x0020, 0x0100, 0x750a9677);
            ROM_LOAD("rocnrope.pr3", 0x0120, 0x0100, 0xb5c75a27);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_rocnrope()
        {
            INPUT_PORTS_START("rocnrope");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("DSW0");
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR("Coin A"));
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
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coin B"));
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
            /* 0x00 disables Coin 2. It still accepts coins and makes the sound, but
               it doesn't give you any credit */

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "255", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x78, 0x78, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x78, "Easy 1");
            PORT_DIPSETTING(0x70, "Easy 2");
            PORT_DIPSETTING(0x68, "Easy 3");
            PORT_DIPSETTING(0x60, "Easy 4");
            PORT_DIPSETTING(0x58, "Normal 1");
            PORT_DIPSETTING(0x50, "Normal 2");
            PORT_DIPSETTING(0x48, "Normal 3");
            PORT_DIPSETTING(0x40, "Normal 4");
            PORT_DIPSETTING(0x38, "Normal 5");
            PORT_DIPSETTING(0x30, "Normal 6");
            PORT_DIPSETTING(0x28, "Normal 7");
            PORT_DIPSETTING(0x20, "Normal 8");
            PORT_DIPSETTING(0x18, "Difficult 1");
            PORT_DIPSETTING(0x10, "Difficult 2");
            PORT_DIPSETTING(0x08, "Difficult 3");
            PORT_DIPSETTING(0x00, "Difficult 4");
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("DSW2");
            PORT_DIPNAME(0x07, 0x07, "First Bonus");
            PORT_DIPSETTING(0x07, "20000");
            PORT_DIPSETTING(0x05, "30000");
            PORT_DIPSETTING(0x04, "40000");
            PORT_DIPSETTING(0x03, "50000");
            PORT_DIPSETTING(0x02, "60000");
            PORT_DIPSETTING(0x01, "70000");
            PORT_DIPSETTING(0x00, "80000");
            /* 0x06 gives 20000 */
            PORT_DIPNAME(0x38, 0x38, "Repeated Bonus");
            PORT_DIPSETTING(0x38, "40000");
            PORT_DIPSETTING(0x18, "50000");
            PORT_DIPSETTING(0x10, "60000");
            PORT_DIPSETTING(0x08, "70000");
            PORT_DIPSETTING(0x00, "80000");
            /* 0x20, 0x28 and 0x30 all gives 40000 */
            PORT_DIPNAME(0x40, 0x00, "Grant Repeated Bonus");
            PORT_DIPSETTING(0x40, DEF_STR("No"));
            PORT_DIPSETTING(0x00, DEF_STR("Yes"));
            PORT_DIPNAME(0x80, 0x00, "Unknown DSW 8");
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        public driver_rocnrope()
        {
            drv = new machine_driver_rocnrope();
            year = "1983";
            name = "rocnrope";
            description = "Roc'n Rope";
            manufacturer = "Konami";
            flags = Mame.ROT270;
            input_ports = input_ports_rocnrope();
            rom = rom_rocnrope();
            drv.HasNVRAMhandler = false;
        }
    }
}
