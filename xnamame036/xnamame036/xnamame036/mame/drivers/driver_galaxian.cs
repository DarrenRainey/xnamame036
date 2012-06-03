﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_galaxian : Mame.GameDriver
    {
        
static Mame.MemoryReadAddress[] galaxian_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),	/* not all games use all the space */
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x5000, 0x53ff, Mame.MRA_RAM ),	/* video RAM */
	new Mame.MemoryReadAddress( 0x5400, 0x57ff, Generic.videoram_r ),	/* video RAM mirror */
	new Mame.MemoryReadAddress( 0x5800, 0x5fff, Mame.MRA_RAM ),	/* screen attributes, sprites, bullets */
	new Mame.MemoryReadAddress( 0x6000, 0x6000, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x6800, 0x6800, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x7000, 0x7000, Mame.input_port_2_r ),	/* DSW */
	new Mame.MemoryReadAddress( 0x7800, 0x7800, Mame.watchdog_reset_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress []galaxian_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),	/* not all games use all the space */
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x5000, 0x53ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x5800, 0x583f, galaxian_attributes_w, galaxian_attributesram ),
	new Mame.MemoryWriteAddress( 0x5840, 0x585f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x5860, 0x587f, Mame.MWA_RAM, galaxian_bulletsram, galaxian_bulletsram_size ),
	new Mame.MemoryWriteAddress( 0x5880, 0x58ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x6000, 0x6001, Mame.osd_led_w ),
	new Mame.MemoryWriteAddress( 0x6004, 0x6007, galaxian_lfo_freq_w ),
	new Mame.MemoryWriteAddress( 0x6800, 0x6802, galaxian_background_enable_w ),
	new Mame.MemoryWriteAddress( 0x6803, 0x6803, galaxian_noise_enable_w ),
	new Mame.MemoryWriteAddress( 0x6805, 0x6805, galaxian_shoot_enable_w ),
	new Mame.MemoryWriteAddress( 0x6806, 0x6807, galaxian_vol_w ),
	new Mame.MemoryWriteAddress( 0x7001, 0x7001, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x7004, 0x7004, galaxian_stars_w ),
	new Mame.MemoryWriteAddress( 0x7006, 0x7006, galaxian_flipx_w ),
	new Mame.MemoryWriteAddress( 0x7007, 0x7007, galaxian_flipy_w ),
	new Mame.MemoryWriteAddress( 0x7800, 0x7800, galaxian_pitch_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
static void galaxian_coin_lockout_w(int offset, int data)
{
    Mame.coin_lockout_global_w(offset, data ^ 1);
}
static void machine_init_galaxian()
{
    Mame.install_mem_write_handler(0, 0x6002, 0x6002, galaxian_coin_lockout_w);
}

static Mame.GfxLayout bulletlayout =new Mame.GfxLayout
(
	/* there is no gfx ROM for this one, it is generated by the hardware */
	3,1,
	1,	/* just one */
	1,	/* 1 bit per pixel */
	new uint[]{ 0 },
	new uint[]{ 2,2,2 },	/* I "know" that this bit of the */
    new uint[] { 0 },						/* graphics ROMs is 1 */
	0	/* no use */
);
        static Mame.GfxLayout galaxian_charlayout =new Mame.GfxLayout
(
	8,8,	/* 8*8 characters */
	Mame.RGN_FRAC(1,2),
	2,	/* 2 bits per pixel */
	new uint[]{ Mame.RGN_FRAC(0,2), Mame.RGN_FRAC(1,2) },	/* the two bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	8*8	/* every char takes 8 consecutive bytes */
);
static Mame.GfxLayout galaxian_spritelayout = new Mame.GfxLayout
(
	16,16,	/* 16*16 sprites */
	Mame.RGN_FRAC(1,2),	/* 64 sprites */
	2,	/* 2 bits per pixel */
    new uint[] { Mame.RGN_FRAC(0, 2), Mame.RGN_FRAC(1, 2) },	/* the two bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
	32*8	/* every sprite takes 32 consecutive bytes */
);
static Mame.GfxLayout backgroundlayout = new Mame.GfxLayout
(
	/* there is no gfx ROM for this one, it is generated by the hardware */
	8,8,
	32,	/* one for each column */
	7,	/* 128 colors max */
	new uint[]{ 1, 2, 3, 4, 5, 6, 7 },
	new uint[]{ 0*8*8, 1*8*8, 2*8*8, 3*8*8, 4*8*8, 5*8*8, 6*8*8, 7*8*8 },
    new uint[] { 0, 8, 16, 24, 32, 40, 48, 56 },
	8*8*8	/* each character takes 64 bytes */
);
static Mame.GfxDecodeInfo[] galaxian_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, galaxian_charlayout,    0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, galaxian_spritelayout,  0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, bulletlayout,         8*4, 2 ),
	new Mame.GfxDecodeInfo( 0,           0x0000, backgroundlayout, 8*4+2*2, 1 ),	/* this will be dynamically created */
};


class galaxian_soundinterface : Mame.CustomSoundInterface
{
    public override int start(Mame.MachineSound msound)
    {
        throw new NotImplementedException();
    }
    public override void stop()
    {
        throw new NotImplementedException();
    }
    public override void update()
    {
        throw new NotImplementedException();
    }
}
static galaxian_soundinterface custom_interface = new galaxian_soundinterface();

        class machine_driver_galaxian : Mame.MachineDriver
        {
            public machine_driver_galaxian()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6,galaxian_readmem,galaxian_writemem,null,null,galaxian_vh_interrupt,1));
                frames_per_second = 16000.0f / 132 / 2;
                vblank_duration = 2500;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = galaxian_gfxdecodeinfo;
                total_colors = 32 + 64 + 1;
                color_table_len = 8 * 4 + 2 * 2 + 128 * 1;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_CUSTOM, custom_interface));
            }
            public override void init_machine()
            {
                machine_init_galaxian();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                galaxian_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return galaxian_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                galaxian_vh_screenrefresh(bitmap,full_refresh);
            }
            public override void vh_eof_callback()
            {
                throw new NotImplementedException();
            }
        }
        Mame.InputPortTiny[] input_ports_galaxian()
        {

            INPUT_PORTS_START("galaxian");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1);
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x20, DEF_STR("Cocktail"));
            PORT_SERVICE(0x40, IP_ACTIVE_HIGH);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN3);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0xc0, DEF_STR("Free Play"));

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "7000");
            PORT_DIPSETTING(0x01, "10000");
            PORT_DIPSETTING(0x02, "12000");
            PORT_DIPSETTING(0x03, "20000");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x04, "3");
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x08, DEF_STR("On"));
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, (uint)IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_galaxian()
        {
            ROM_START("galaxian");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("galmidw.u", 0x0000, 0x0800, 0x745e2d61);
            ROM_LOAD("galmidw.v", 0x0800, 0x0800, 0x9c999a40);
            ROM_LOAD("galmidw.w", 0x1000, 0x0800, 0xb5894925);
            ROM_LOAD("galmidw.y", 0x1800, 0x0800, 0x6b3ca10b);
            ROM_LOAD("7l", 0x2000, 0x0800, 0x1b933207);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1h", 0x0000, 0x0800, 0x39fb43a4);
            ROM_LOAD("1k", 0x0800, 0x0800, 0x7e3f56a2);

            ROM_REGION(0x0020, Mame.REGION_PROMS);
            ROM_LOAD("galaxian.clr", 0x0000, 0x0020, 0xc3ac9467);
            return ROM_END;
        }
        public override void driver_init()
        {
            //nonethrow new NotImplementedException();
        }
        public driver_galaxian()
        {
            drv = new machine_driver_galaxian();
            year = "1979";
            name = "galaxian";
            description = "Galaxian (Namco)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_galaxian();
            rom = rom_galaxian();
            drv.HasNVRAMhandler = false;
        }
    }
}
