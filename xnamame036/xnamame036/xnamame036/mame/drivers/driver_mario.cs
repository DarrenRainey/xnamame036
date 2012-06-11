using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_mario : Mame.GameDriver
    {
        static int[] p = { 0, 0xf0, 0, 0, 0, 0, 0, 0 };
        static int[] t = { 0, 0 };

        static int gfx_bank, palette_bank;
        static _BytePtr mario_scrolly = new _BytePtr(1);

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x5fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(0x6000, 0x6fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(0x7400, 0x77ff, Mame.MRA_RAM),	/* video RAM */
	new Mame.MemoryReadAddress(0x7c00, 0x7c00, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress(0x7c80, 0x7c80, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress(0x7f80, 0x7f80, Mame.input_port_2_r ),	/* DSW */
	new Mame.MemoryReadAddress(0xf000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0x6000, 0x68ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress(0x6a80, 0x6fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress(0x6900, 0x6a7f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress(0x7400, 0x77ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress(0x7c00, 0x7c00, mario_sh1_w ), /* Mario run sample */
	new Mame.MemoryWriteAddress(0x7c80, 0x7c80, mario_sh2_w ), /* Luigi run sample */
	new Mame.MemoryWriteAddress(0x7d00, 0x7d00, Mame.MWA_RAM, mario_scrolly ),
	new Mame.MemoryWriteAddress(0x7e80, 0x7e80, mario_gfxbank_w ),
	new Mame.MemoryWriteAddress(0x7e83, 0x7e83, mario_palettebank_w ),
	new Mame.MemoryWriteAddress(0x7e84, 0x7e84, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress(0x7f00, 0x7f00, mario_sh_w ),	/* death */
	new Mame.MemoryWriteAddress(0x7f01, 0x7f01, mario_sh_getcoin ),
	new Mame.MemoryWriteAddress(0x7f03, 0x7f03, mario_sh_crab ),
	new Mame.MemoryWriteAddress(0x7f04, 0x7f04, mario_sh_turtle ),
	new Mame.MemoryWriteAddress(0x7f05, 0x7f05, mario_sh_fly ),
	new Mame.MemoryWriteAddress(0x7f00, 0x7f07, mario_sh3_w ), /* Misc discrete samples */
	new Mame.MemoryWriteAddress(0x7e00, 0x7e00, mario_sh_tuneselect ),
	new Mame.MemoryWriteAddress(0x7000, 0x73ff, Mame.MWA_NOP ),	/* ??? */
//	new Mame.MemoryWriteAddress(0x7e85, 0x7e85, MWA_RAM },	/* Sets alternative 1 and 0 */
	new Mame.MemoryWriteAddress(0xf000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(-1 )	/* end of table */
};
        static Mame.IOWritePort[] mario_writeport =
{
	new Mame.IOWritePort( 0x00,   0x00,   null ),  /* unknown... is this a trigger? */
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_sound =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};
        static Mame.MemoryWriteAddress[] writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOReadPort[] readport_sound =
{
	new Mame.IOReadPort( 0x00,     0xff,     mario_sh_gettune ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_p1, Mame.cpu_i8039.I8039_p1, mario_sh_getp1 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_p2,Mame.cpu_i8039.I8039_p2, mario_sh_getp2 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_t0, Mame.cpu_i8039.I8039_t0, mario_sh_gett0 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_t1, Mame.cpu_i8039.I8039_t1, mario_sh_gett1 ),
	new Mame.IOReadPort( -1 )	/* end of table */
};
        static Mame.IOWritePort[] writeport_sound =
{
	new Mame.IOWritePort( 0x00,     0xff,     mario_sh_putsound ),
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p1, Mame.cpu_i8039.I8039_p1, mario_sh_putp1 ),
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p2, Mame.cpu_i8039.I8039_p2, mario_sh_putp2 ),
	new Mame.IOWritePort( -1 )	/* end of table */
};


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 512 * 8 * 8, 0 },	/* the bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );


        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            3,	/* 3 bits per pixel */
            new uint[] { 2 * 256 * 16 * 16, 256 * 16 * 16, 0 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,		/* the two halves of the sprite are separated */
			256*16*8+0, 256*16*8+1, 256*16*8+2, 256*16*8+3, 256*16*8+4, 256*16*8+5, 256*16*8+6, 256*16*8+7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 8 * 8, 9 * 8, 10 * 8, 11 * 8, 12 * 8, 13 * 8, 14 * 8, 15 * 8 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,      0, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, spritelayout, 16*4, 32 ),
};



        static Mame.DACinterface dac_interface =
        new Mame.DACinterface(
            1,
            new int[] { 100 }
        );

        static string[] mario_sample_names =
{
	"*mario",

	/* 7f01 - 7f07 sounds */
	"ice.wav",    /* 0x02 ice appears (formerly effect0.wav) */
	"coin.wav",   /* 0x06 coin appears (formerly effect1.wav) */
	"skid.wav",   /* 0x07 skid */

	/* 7c00 */
	"run.wav",        /* 03, 02, 01 - 0x1b */

	/* 7c80 */
	"luigirun.wav",   /* 03, 02, 01 - 0x1c */

    null	/* end of array */
};

        static Mame.Samplesinterface samples_interface =
        new Mame.Samplesinterface(
            3,	/* 3 channels */
            25,	/* volume */
            mario_sample_names
        );

        static AY8910interface ay8910_interface =
        new AY8910interface(
            1,      /* 1 chip */
            14318000 / 6,	/* ? */
            new int[] { 50 },
            new AY8910portRead[] { Mame.soundlatch_r },
            new AY8910portRead[] { null },
            new AY8910portWrite[] { null },
            new AY8910portWrite[] { null }, null
        );
        static void mario_sh_w(int offset, int data)
        {
            if (data != 0)
                Mame.cpu_set_irq_line(1, 0, Mame.ASSERT_LINE);
            else
                Mame.cpu_set_irq_line(1, 0, Mame.CLEAR_LINE);
        }

        /* Mario running sample */
        static int last1;
        static void mario_sh1_w(int offset, int data)
        {
            if (last1 != data)
            {
                last1 = data;
                if (data != 0 && !Mame.sample_playing(0)) Mame.sample_start(0, 3, 0);
            }
        }

        /* Luigi running sample */
        static int last2;
        static void mario_sh2_w(int offset, int data)
        {
            if (last2 != data)
            {
                last2 = data;
                if (data != 0 && !Mame.sample_playing(1)) Mame.sample_start(1, 4, 0);
            }
        }

        /* Misc samples */
        static int[] state = new int[8];
        static void mario_sh3_w(int offset, int data)
        {
            /* Don't trigger the sample if it's still playing */
            if (state[offset] == data) return;

            state[offset] = data;
            if (data != 0)
            {
                switch (offset)
                {
                    case 2: /* ice */
                        Mame.sample_start(2, 0, 0);
                        break;
                    case 6: /* coin */
                        Mame.sample_start(2, 1, 0);
                        break;
                    case 7: /* skid */
                        Mame.sample_start(2, 2, 0);
                        break;
                }
            }
        }

        static void mario_gfxbank_w(int offset, int data)
        {
            if (gfx_bank != (data & 1))
            {
                Generic.SetDirtyBuffer(true);
                gfx_bank = data & 1;
            }
        }
        static void mario_palettebank_w(int offset, int data)
        {
            if (palette_bank != (data & 1))
            {
                Generic.SetDirtyBuffer(true);
                palette_bank = data & 1;
            }
        }

        static void mario_sh_growing(int offset, int data) { t[1] = data; }
        static void mario_sh_getcoin(int offset, int data) { t[0] = data; }
        static void mario_sh_crab(int offset, int data) { p[1] = ((p[1] & (~(1 << 0))) | (data << 0)); }
        static void mario_sh_turtle(int offset, int data) { p[1] = ((p[1] & (~(1 << 0))) | (data << 0)); }
        static void mario_sh_fly(int offset, int data) { p[1] = ((p[1] & (~(1 << 0))) | (data << 0)); }
        static void mario_sh_tuneselect(int offset, int data) { Mame.soundlatch_w(offset, data); }

        static int mario_sh_getp1(int offset) { return p[1]; }
        static int mario_sh_getp2(int offset) { return p[2]; }
        static int mario_sh_gett0(int offset) { return t[0]; }
        static int mario_sh_gett1(int offset) { return t[1]; }
        static int mario_sh_gettune(int offset) { return Mame.soundlatch_r(offset); }

        static void mario_sh_putsound(int offset, int data)
        {
            DAC.DAC_data_w(0, data);
        }
        static void mario_sh_putp1(int offset, int data)
        {
            p[1] = data;
        }
        static void mario_sh_putp2(int offset, int data)
        {
            p[2] = data;
        }
        class machine_driver_mario : Mame.MachineDriver
        {
            public machine_driver_mario()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, writemem, null, mario_writeport, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_I8039 | Mame.CPU_AUDIO_CPU, 730000, readmem_sound, writemem_sound, readport_sound, writeport_sound, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_mario.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 16 * 4 + 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
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
                int pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 5) & 1;
                    int bit1 = (color_prom[cpi] >> 6) & 1;
                    int bit2 = (color_prom[cpi] >> 7) & 1;
                    palette[pi++] = (byte)(255 - (0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2));
                    /* green component */
                    bit0 = (color_prom[cpi] >> 2) & 1;
                    bit1 = (color_prom[cpi] >> 3) & 1;
                    bit2 = (color_prom[cpi] >> 4) & 1;
                    palette[pi++] = (byte)(255 - (0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2));
                    /* blue component */
                    bit0 = (color_prom[cpi] >> 0) & 1;
                    bit1 = (color_prom[cpi] >> 1) & 1;
                    palette[pi++] = (byte)(255 - (0x55 * bit0 + 0xaa * bit1));

                    cpi++;
                }

                /* characters use the same palette as sprites, however characters */
                /* use only colors 64-127 and 192-255. */
                for (int i = 0; i < 8; i++)
                {
                    COLOR(colortable, 0, 4 * i, (ushort)(8 * i + 64));
                    COLOR(colortable, 0, 4 * i + 1, (ushort)(8 * i + 1 + 64));
                    COLOR(colortable, 0, 4 * i + 2, (ushort)(8 * i + 2 + 64));
                    COLOR(colortable, 0, 4 * i + 3, (ushort)(8 * i + 3 + 64));
                }
                for (int i = 0; i < 8; i++)
                {
                    COLOR(colortable, 0, 4 * i + 8 * 4, (ushort)(8 * i + 192));
                    COLOR(colortable, 0, 4 * i + 8 * 4 + 1, (ushort)(8 * i + 1 + 192));
                    COLOR(colortable, 0, 4 * i + 8 * 4 + 2, (ushort)(8 * i + 2 + 192));
                    COLOR(colortable, 0, 4 * i + 8 * 4 + 3, (ushort)(8 * i + 3 + 192));
                }

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable, 1, i, i);
            }
            public override int vh_start()
            {
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_eof_callback()
            {
                //none
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 256 * gfx_bank),
                                (uint)((Generic.videoram[offs] >> 5) + 8 * palette_bank),
                               false, false,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int scrolly;

                    /* I'm not positive the scroll direction is right */
                    scrolly = -mario_scrolly[0] - 17;
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                /* Draw the sprites. */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
                {
                    if (Generic.spriteram[offs] != 0)
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                Generic.spriteram[offs + 2],
                                (uint)((Generic.spriteram[offs + 1] & 0x0f) + 16 * palette_bank),
                                (Generic.spriteram[offs + 1] & 0x80) != 0, (Generic.spriteram[offs + 1] & 0x40) != 0,
                                Generic.spriteram[offs + 3] - 8, 240 - Generic.spriteram[offs] + 8,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_mario()
        {
            ROM_START("mario");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("mario.7f", 0x0000, 0x2000, 0xc0c6e014);
            ROM_LOAD("mario.7e", 0x2000, 0x2000, 0x116b3856);
            ROM_LOAD("mario.7d", 0x4000, 0x2000, 0xdcceb6c1);
            ROM_LOAD("mario.7c", 0xf000, 0x1000, 0x4a63d96b);

            ROM_REGION(0x1000, Mame.REGION_CPU2);	/* sound */
            ROM_LOAD("tma1c-a.6k", 0x0000, 0x1000, 0x06b9ff85);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mario.3f", 0x0000, 0x1000, 0x28b0c42c);
            ROM_LOAD("mario.3j", 0x1000, 0x1000, 0x0c8cc04d);

            ROM_REGION(0x6000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mario.7m", 0x0000, 0x1000, 0x22b7372e);
            ROM_LOAD("mario.7n", 0x1000, 0x1000, 0x4f3a1f47);
            ROM_LOAD("mario.7p", 0x2000, 0x1000, 0x56be6ccd);
            ROM_LOAD("mario.7s", 0x3000, 0x1000, 0x56f1d613);
            ROM_LOAD("mario.7t", 0x4000, 0x1000, 0x641f0008);
            ROM_LOAD("mario.7u", 0x5000, 0x1000, 0x7baf5309);

            ROM_REGION(0x0200, Mame.REGION_PROMS);
            ROM_LOAD("mario.4p", 0x0000, 0x0200, 0xafc9bd41);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_mario()
        {
            INPUT_PORTS_START("mario");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BITX(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, DEF_STR("Service Mode"), (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPSETTING(0x03, "6");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x30, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "20000");
            PORT_DIPSETTING(0x10, "30000");
            PORT_DIPSETTING(0x20, "40000");
            PORT_DIPSETTING(0x30, "None");
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x40, "Medium");
            PORT_DIPSETTING(0x80, "Hard");
            PORT_DIPSETTING(0xc0, "Hardest");
            return INPUT_PORTS_END;
        }
        public driver_mario()
        {
            drv = new machine_driver_mario();
            year = "1983";
            name = "mario";
            description = "Mario Bros. (US)";
            manufacturer = "Nintendo of America";
            flags = Mame.ROT180;
            input_ports = input_ports_mario();
            rom = rom_mario();
        }
    }
}
