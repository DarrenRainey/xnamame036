using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_digdug : Mame.GameDriver
    {

        static Mame.MemoryReadAddress[] readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x7000, 0x700f, digdug_customio_data_r ),
	new Mame.MemoryReadAddress( 0x7100, 0x7100, digdug_customio_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, digdug_sharedram_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress (0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress (0x8000, 0x9fff, digdug_sharedram_r ),
	new Mame.MemoryReadAddress (-1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu3 =
{
	new Mame.MemoryReadAddress (0x0000, 0x0fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress (0x8000, 0x9fff, digdug_sharedram_r ),
	new Mame.MemoryReadAddress (-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6820, 0x6820, digdug_interrupt_enable_1_w ),
	new Mame.MemoryWriteAddress( 0x6821, 0x6821, digdug_interrupt_enable_2_w ),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, digdug_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x6823, 0x6823, digdug_halt_w ),
    new Mame.MemoryWriteAddress( 0xa007, 0xa007, digdug_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x6825, 0x6827, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0x6830, 0x6830, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x7000, 0x700f, digdug_customio_data_w ),
	new Mame.MemoryWriteAddress( 0x7100, 0x7100, digdug_customio_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, digdug_sharedram_w, digdug_sharedram ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Mame.MWA_RAM, Generic.videoram, Generic.videoram_size ),   /* dirtybuffer[] handling is not needed because */
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, Mame.MWA_RAM ),	                          /* characters are redrawn every frame */
	new Mame.MemoryWriteAddress( 0x8b80, 0x8bff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ), /* these three are here just to initialize */
	new Mame.MemoryWriteAddress( 0x9380, 0x93ff, Mame.MWA_RAM, Generic.spriteram_2 ),	          /* the pointers. The actual writes are */
	new Mame.MemoryWriteAddress( 0x9b80, 0x9bff, Mame.MWA_RAM, Generic.spriteram_3 ),                /* handled by digdug_sharedram_w() */
	new Mame.MemoryWriteAddress( 0xa000, 0xa00f, digdug_vh_latch_w, digdug_vlatches ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6821, 0x6821, digdug_interrupt_enable_2_w ),
	new Mame.MemoryWriteAddress( 0x6830, 0x6830, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, digdug_sharedram_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa00f, digdug_vh_latch_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu3 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6800, 0x681f, Namco.pengo_sound_w, Namco.namco_soundregs),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, digdug_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, digdug_sharedram_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.GfxLayout charlayout1 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            128,	/* 128 characters */
            1,		/* 1 bit per pixel */
            new uint[] { 0 },	/* one bitplane */
            new uint[] { 7, 6, 5, 4, 3, 2, 1, 0 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout charlayout2 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 0, 4 },      /* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },   /* bits are packed in groups of four */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },   /* characters are rotated 90 degrees */
            16 * 8	       /* every char takes 16 bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	        /* 16*16 sprites */
            256,	        /* 256 sprites */
            2,	        /* 2 bits per pixel */
            new uint[] { 0, 4 },	/* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 0, 1, 2, 3, 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 16 * 8 + 0, 16 * 8 + 1, 16 * 8 + 2, 16 * 8 + 3, 24 * 8 + 0, 24 * 8 + 1, 24 * 8 + 2, 24 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 32 * 8, 33 * 8, 34 * 8, 35 * 8, 36 * 8, 37 * 8, 38 * 8, 39 * 8 },
            64 * 8	/* every sprite takes 64 bytes */
        );


        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout1,            0,  8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout,         8*2, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, charlayout2,   64*4 + 8*2, 64 ),
};


        static Namco_interface namco_interface =
        new Namco_interface(
            3072000 / 32,	/* sample rate */
            3,			/* number of voices */
            100,		/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );

        class machine_driver_digdug : Mame.MachineDriver
        {
            public machine_driver_digdug()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80,3125000,readmem_cpu1,writemem_cpu1,null,null,digdug_interrupt_1,1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80,3125000,readmem_cpu2,writemem_cpu2,null,null,digdug_interrupt_2,1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu3, writemem_cpu3, null, null, digdug_interrupt_3, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_digdug.gfxdecodeinfo;
                total_colors=32;
                color_table_len = 8*2+64*4+64*4;
                video_attributes = Mame.VIDEO_TYPE_RASTER|Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO,namco_interface));
            }
            public override void init_machine()
            {
                digdig_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                digdug_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return digdug_vh_start();
            }
            public override void vh_stop()
            {
                digdug_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                digdug_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        Mame.InputPortTiny[] input_ports_digdug()
        {
            INPUT_PORTS_START("digdug");
            PORT_START("DSW0");
            PORT_DIPNAME(0x07, 0x01, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x07, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_7C"));
            /* TODO: bonus scores are different for 5 lives */
            PORT_DIPNAME(0x38, 0x18, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x20, "10k 40k 40k");
            PORT_DIPSETTING(0x10, "10k 50k 50k");
            PORT_DIPSETTING(0x30, "20k 60k 60k");
            PORT_DIPSETTING(0x08, "20k 70k 70k");
            PORT_DIPSETTING(0x28, "10k 40k");
            PORT_DIPSETTING(0x18, "20k 60k");
            PORT_DIPSETTING(0x38, "10k");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0xc0, 0x80, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "1");
            PORT_DIPSETTING(0x40, "2");
            PORT_DIPSETTING(0x80, "3");
            PORT_DIPSETTING(0xc0, "5");

            PORT_START("DSW1");
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x20, 0x20, "Freeze");
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x00, "Allow Continue");
            PORT_DIPSETTING(0x08, DEF_STR("No"));
            PORT_DIPSETTING(0x00, DEF_STR("Yes"));
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x04, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x02, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x03, "Hardest");

            PORT_START("FAKE");
            /* The player inputs are not memory mapped, they are handled by an I/O chip. */
            /* These fake input ports are read by digdug_customio_data_r() */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, null, (ushort)(Mame.InputCodes.CODE_PREVIOUS), (ushort)Mame.InputCodes.CODE_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("FAKE");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, null, (ushort)Mame.InputCodes.CODE_PREVIOUS, (ushort)Mame.InputCodes.CODE_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("FAKE");
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 1);
            PORT_BIT(0x0c, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_START2, 1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_digdug()
        {

            ROM_START("digdug");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code for the first CPU  */
            ROM_LOAD("136007.101", 0x0000, 0x1000, 0xb9198079);
            ROM_LOAD("136007.102", 0x1000, 0x1000, 0xb2acbe49);
            ROM_LOAD("136007.103", 0x2000, 0x1000, 0xd6407b49);
            ROM_LOAD("dd1.4b", 0x3000, 0x1000, 0xf4cebc16);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("dd1.5b", 0x0000, 0x1000, 0x370ef9b4);
            ROM_LOAD("dd1.6b", 0x1000, 0x1000, 0x361eeb71);

            ROM_REGION(0x10000, Mame.REGION_CPU3);	/* 64k for the third CPU  */
            ROM_LOAD("136007.107", 0x0000, 0x1000, 0xa41bce72);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("dd1.9", 0x0000, 0x0800, 0xf14a6fe1);

            ROM_REGION(0x4000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("136007.116", 0x0000, 0x1000, 0xe22957c8);
            ROM_LOAD("dd1.14", 0x1000, 0x1000, 0x2829ec99);
            ROM_LOAD("136007.118", 0x2000, 0x1000, 0x458499e9);
            ROM_LOAD("136007.119", 0x3000, 0x1000, 0xc58252a0);

            ROM_REGION(0x1000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("dd1.11", 0x0000, 0x1000, 0x7b383983);

            ROM_REGION(0x1000, Mame.REGION_GFX4); /* 4k for the playfield graphics */
            ROM_LOAD("dd1.10b", 0x0000, 0x1000, 0x2cf399c2);

            ROM_REGION(0x0220, Mame.REGION_PROMS);
            ROM_LOAD("digdug.5n", 0x0000, 0x0020, 0x4cb9da99);
            ROM_LOAD("digdug.1c", 0x0020, 0x0100, 0x00c7c419);
            ROM_LOAD("digdug.2n", 0x0120, 0x0100, 0xe9b3e08e);

            ROM_REGION(0x0100, Mame.REGION_SOUND1);	/* sound prom */
            ROM_LOAD("digdug.spr", 0x0000, 0x0100, 0x7a2815b4);
            return ROM_END;
        }
        public override void driver_init()
        {
            //none
        }
        public driver_digdug()
        {
            drv = new machine_driver_digdug();
            year = "1982";
            name = "digdug";
            description = "Dig Dug (set 1)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_digdug();
            rom = rom_digdug();
            drv.HasNVRAMhandler = false;
        }
    }
}
