using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public class driver_pacman : Mame.GameDriver
    {
        static Mame.GfxLayout tilelayout = new Mame.GfxLayout(
    8, 8,	/* 8*8 characters */
    256,    /* 256 characters */
    2,  /* 2 bits per pixel */
    new uint[] { 0, 4 },   /* the two bitplanes for 4 pixels are packed into one byte */
    new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 }, /* bits are packed in groups of four */
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    16 * 8    /* every char takes 16 bytes */
);


        static Mame.GfxLayout spritelayout = new Mame.GfxLayout(
    16, 16,	/* 16*16 sprites */
    64,	/* 64 sprites */
    2,	/* 2 bits per pixel */
    new uint[] { 0, 4 },	/* the two bitplanes for 4 pixels are packed into one byte */
    new uint[] { 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 16 * 8 + 0, 16 * 8 + 1, 16 * 8 + 2, 16 * 8 + 3, 24 * 8 + 0, 24 * 8 + 1, 24 * 8 + 2, 24 * 8 + 3, 0, 1, 2, 3 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 32 * 8, 33 * 8, 34 * 8, 35 * 8, 36 * 8, 37 * 8, 38 * 8, 39 * 8 },
    64 * 8	/* every sprite takes 64 bytes */
);

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, tilelayout,   0, 32 ),
    new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout, 0, 32 ),
};

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),	/* video and color RAM */
	new Mame.MemoryReadAddress( 0x4c00, 0x4fff, Mame.MRA_RAM ),	/* including sprite codes at 4ff0-4fff */
	new Mame.MemoryReadAddress( 0x5000, 0x503f, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x5040, 0x507f, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x5080, 0x50bf, Mame.input_port_2_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0x50c0, 0x50ff, Mame.input_port_3_r ),	/* DSW2 */
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_ROM ),	/* Ms. Pac-Man / Ponpoko only */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x4400, 0x47ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x4c00, 0x4fef, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4ff0, 0x4fff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x5000, 0x5000, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x5001, 0x5001, Namco.pengo_sound_enable_w ),
	new Mame.MemoryWriteAddress( 0x5002, 0x5002, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0x5003, 0x5003, Pengo.pengo_flipscreen_w ),
 	new Mame.MemoryWriteAddress( 0x5004, 0x5005, Mame.osd_led_w ),
// 	new Mame.MemoryWriteAddress( 0x5006, 0x5006, Mame.pacman_coin_lockout_global_w },	this breaks many games
 	new Mame.MemoryWriteAddress( 0x5007, 0x5007, Mame.coin_counter_w ),
	new Mame.MemoryWriteAddress( 0x5040, 0x505f, Namco.pengo_sound_w, Namco.namco_soundregs ),
	new Mame.MemoryWriteAddress( 0x5060, 0x506f, Mame.MWA_RAM, Generic.spriteram_2 ),
	new Mame.MemoryWriteAddress( 0x50c0, 0x50c0, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xbfff, Mame.MWA_ROM ),	/* Ms. Pac-Man / Ponpoko only */
	new Mame.MemoryWriteAddress( 0xc000, 0xc3ff, Generic.videoram_w ), /* mirror address for video ram, */
	new Mame.MemoryWriteAddress( 0xc400, 0xc7ef, Generic.colorram_w ), /* used to display HIGH SCORE and CREDITS */
	new Mame.MemoryWriteAddress( 0xffff, 0xffff, Mame.MWA_NOP ),	/* Eyes writes to this location to simplify code */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        Mame.RomModule[] rom_pacman()
        {
            ROM_START("pacman");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("namcopac.6e", 0x0000, 0x1000, 0xfee263b3);
            ROM_LOAD("namcopac.6f", 0x1000, 0x1000, 0x39d1fc83);
            ROM_LOAD("namcopac.6h", 0x2000, 0x1000, 0x02083b03);
            ROM_LOAD("namcopac.6j", 0x3000, 0x1000, 0x7a36fe55);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("pacman.5e", 0x0000, 0x1000, 0x0c944964);

            ROM_REGION(0x1000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("pacman.5f", 0x0000, 0x1000, 0x958fedf9);

            ROM_REGION(0x0120, Mame.REGION_PROMS);
            ROM_LOAD("82s123.7f", 0x0000, 0x0020, 0x2fc650bd);
            ROM_LOAD("82s126.4a", 0x0020, 0x0100, 0x3eb3a8e4);

            ROM_REGION(0x0200, Mame.REGION_SOUND1);	/* sound PROMs */
            ROM_LOAD("82s126.1m", 0x0000, 0x0100, 0xa9cc86bf);
            ROM_LOAD("82s126.3m", 0x0100, 0x0100, 0x77245b66);	/* timing - not used */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_pacman()
        {
            INPUT_PORTS_START("pacman");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BITX(0x10, 0x10, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_SERVICE(0x10, IP_ACTIVE_LOW);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            PORT_DIPNAME(0x80, 0x80, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Cocktail"]);

            PORT_START("DSW 1");
            PORT_DIPNAME(0x03, 0x01, ipdn_defaultstrings["Coinage"]);
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Free Play"]);
            PORT_DIPNAME(0x0c, 0x08, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x00, "1");
            PORT_DIPSETTING(0x04, "2");
            PORT_DIPSETTING(0x08, "3");
            PORT_DIPSETTING(0x0c, "5");
            PORT_DIPNAME(0x30, 0x00, ipdn_defaultstrings["Bonus Life"]);
            PORT_DIPSETTING(0x00, "10000");
            PORT_DIPSETTING(0x10, "15000");
            PORT_DIPSETTING(0x20, "20000");
            PORT_DIPSETTING(0x30, "None");
            PORT_DIPNAME(0x40, 0x40, ipdn_defaultstrings["Difficulty"]);
            PORT_DIPSETTING(0x40, "Normal");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x80, 0x80, "Ghost Names");
            PORT_DIPSETTING(0x80, "Normal");
            PORT_DIPSETTING(0x00, "Alternate");

            PORT_START("DSW 2");
            PORT_BIT(0xff, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("FAKE");
            /* This fake input port is used to get the status of the fire button */
            /* and activate the speedup cheat if it is. */
            PORT_BITX(0x01, 0x00, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Speedup Cheat", (ushort)Mame.InputCodes.KEYCODE_LCONTROL, (ushort)Mame.InputCodes.JOYCODE_1_BUTTON1);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["On"]);
            return INPUT_PORTS_END;
        }
        static int speedcheat = 0;
        static void pacman_init_machine()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            /* check if the loaded set of ROMs allows the Pac Man speed hack */
            if ((RAM[0x180b] == 0xbe && RAM[0x1ffd] == 0x00) ||
                    (RAM[0x180b] == 0x01 && RAM[0x1ffd] == 0xbd))
                speedcheat = 1;
            else
                speedcheat = 0;
        }
        static Mame.IOWritePort[] writeport = {
                                                  new Mame.IOWritePort(0x00,0x00,Mame.interrupt_vector_w),
                                                  new Mame.IOWritePort(-1)
                                              };
        static int pacman_interrupt()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            /* speed up cheat */
            if (speedcheat != 0)
            {
                if ((Mame.readinputport(4) & 1) != 0)	/* check status of the fake dip switch */
                {
                    /* activate the cheat */
                    RAM[0x180b] = 0x01;
                    RAM[0x1ffd] = 0xbd;
                }
                else
                {
                    /* remove the cheat */
                    RAM[0x180b] = 0xbe;
                    RAM[0x1ffd] = 0x00;
                }
            }

            return Mame.interrupt();
        }
        static Namco_interface namco_interface = new Namco_interface(3072000 / 32, 3, 100, Mame.REGION_SOUND1);
        class machine_driver_pacman : Mame.MachineDriver
        {
            public machine_driver_pacman()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6,	/* 3.072 Mhz */
                        readmem, writemem, null, writeport, pacman_interrupt, 1)
                    );
                frames_per_second = 60;
                vblank_duration = 2500;
                cpu_slices_per_frame = 1;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_pacman.gfxdecodeinfo;
                total_colors = 16;
                color_table_len = 4 * 32;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
            }
            public override void init_machine()
            {
                pacman_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                //none
            }
            public override void vh_eof_callback()
            {
                //none
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                Pengo.pacman_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return Pengo.pacman_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Pengo.pengo_vh_screenrefresh(bitmap, full_refresh);
            }
        }

        public driver_pacman()
        {
            drv = new machine_driver_pacman();
            year = "1980";
            name = "pacman";
            description = "PuckMan (Japan set 1)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_pacman();
            rom = rom_pacman();
            drv.HasNVRAMhandler = false;
        }
        public override void driver_init()
        {
            //none
        }
    }
}
