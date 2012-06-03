using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class williams
    {
        public static _BytePtr williams_bank_base = new _BytePtr(1);
        public static _BytePtr defender_bank_base = new _BytePtr(1);

        /* RAM globals */
        public static _BytePtr williams_videoram = new _BytePtr(1);
        public static _BytePtr williams2_paletteram = new _BytePtr(1);

        /* blitter variables */
        public static _BytePtr williams_blitterram = new _BytePtr(1);
        public static byte williams_blitter_xor;
        public static byte williams_blitter_remap;
        public static byte williams_blitter_clip;
        public static ushort sinistar_clip;
        public static byte williams_cocktail;

        /* Blaster extra variables */
        public static _BytePtr blaster_video_bits = new _BytePtr(1);
        public static _BytePtr blaster_color_zero_table = new _BytePtr(1);
        public static _BytePtr blaster_color_zero_flags = new _BytePtr(1);
        public static _BytePtr blaster_remap = new _BytePtr(1);
        public static _BytePtr blaster_remap_lookup = new _BytePtr(1);
        public static byte blaster_erase_screen;
        public static ushort blaster_back_color;

        /* tilemap variables */
        public byte williams2_tilemap_mask;
        public static _BytePtr williams2_row_to_palette = new _BytePtr(1); /* take care of IC79 and J1/J2 */
        public byte williams2_M7_flip;
        public sbyte williams2_videoshift;
        public byte williams2_special_bg_color;
        public static byte williams2_fg_color; /* IC90 */
        public static byte williams2_bg_color; /* IC89 */

        /* later-Williams video control variables */
        public static _BytePtr williams2_blit_inhibit = new _BytePtr(1);
        public static _BytePtr williams2_xscroll_low = new _BytePtr(1);
        public static _BytePtr williams2_xscroll_high = new _BytePtr(1);

       public static uint[] defender_bank_list;

        public static _6821pia.pia6821_interface williams_pia_1_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ Mame.input_port_2_r, null, null, null, null, null,
            /*outputs: A/B,CA/B2       */ null, williams_snd_cmd_w, null, null,
            /*irqs   : A/B             */ williams_main_irq, williams_main_irq
);

        /* Generic PIA 2, maps to DAC data in and sound IRQs */
        public static _6821pia.pia6821_interface williams_snd_pia_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ null, null, null, null, null, null,
            /*outputs: A/B,CA/B2       */ DAC.DAC_data_w, null, null, null,
            /*irqs   : A/B             */ williams_snd_irq, williams_snd_irq
);
        static byte williams2_bank;


    }
}
namespace xnamame036.mame.drivers
{
    //Setup global static variables to be used across the williams drivers here
    class driver_defender : Mame.GameDriver
    {        static Mame.MemoryReadAddress[] defender_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x97ff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x9800, 0xbfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0xd000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};


        static Mame.MemoryWriteAddress[] defender_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x97ff, williams.williams_videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x9800, 0xbfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_BANK2, williams.defender_bank_base ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc00f, Mame.MWA_RAM, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, williams.defender_bank_select_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x007f, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0400, 0x0403, _6821pia.pia_2_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x8403, _6821pia.pia_2_r ),	/* used by Colony 7, perhaps others? */
	new Mame.MemoryReadAddress( 0xb000, 0xffff, Mame.MRA_ROM ),	/* most games start at $F000; Sinistar starts at $B000 */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};


static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x007f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0400, 0x0403,_6821pia.pia_2_w ),
	new Mame.MemoryWriteAddress( 0x8400, 0x8403,_6821pia.pia_2_w ),	/* used by Colony 7, perhaps others? */
	new Mame.MemoryWriteAddress( 0xb000, 0xffff, Mame.MWA_ROM ),	/* most games start at $F000; Sinistar starts at $B000 */
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static ushort cmos_base;
        static ushort cmos_length;

        /* banking addresses set by the drivers */
        
        _BytePtr mayday_protection;

        /* internal bank switching tracking */
        static byte blaster_bank;
        static byte vram_bank;
        byte williams2_bank;

        /* switches controlled by $c900 */
        ushort sinistar_clip;
        byte williams_cocktail;

        static Mame.DACinterface dac_interface = new Mame.DACinterface(1, new int[] { 50 });
        /* other stuff */
        static ushort joust2_current_sound_data;
        class machine_driver_defender : Mame.MachineDriver
        {
            public machine_driver_defender()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1000000, defender_readmem, defender_writemem, null, null, Mame.ignore_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6808 | Mame.CPU_AUDIO_CPU, 3579000 / 4, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 304;
                screen_height = 256;
                visible_area = new Mame.rectangle(6, 298 - 1, 7, 247 - 1);
                total_colors = 16;
                color_table_len = 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                /* standard init */
                williams.williams_init_machine();

                /* make sure the banking is reset to 0 */
                williams.defender_bank_select_w(0, 0);
                Mame.cpu_setbank(1, williams.williams_videoram);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
               _BytePtr ram = Mame.memory_region(Mame.REGION_CPU1);

                if (read_or_write!=0)
                    Mame.osd_fwrite(file, new _BytePtr(ram, cmos_base), cmos_length);
                else
                {
                    if (file!=null)
                        Mame.osd_fread(file, new _BytePtr(ram, cmos_base), cmos_length);
                    else
                        for (int i = 0; i < cmos_length; i++)
                        {
                            ram.buffer[ram.offset + i + cmos_base] = 0;
                        }
                        //memset(&ram[cmos_base], 0, cmos_length);
                }
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
               //nothing
            }
            public override int vh_start()
            {
                return williams.williams_vh_start();
            }
            public override void vh_stop()
            {
                williams.williams_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                williams.williams_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        void CONFIGURE_CMOS(ushort a, ushort l)
        {
            cmos_base = a; cmos_length = l;
        }
        static _6821pia.pia6821_interface defender_pia_0_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ williams.defender_input_port_0_r, Mame.input_port_1_r, null, null, null, null,
            /*outputs: A/B,CA/B2       */ null, null, null, null,
            /*irqs   : A/B             */ null, null
);

        void CONFIGURE_PIAS(_6821pia.pia6821_interface a, _6821pia.pia6821_interface b, _6821pia.pia6821_interface c)
        {
            _6821pia.pia_unconfig();
            _6821pia.pia_config(0, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), a);
            _6821pia.pia_config(1, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), b);
            _6821pia.pia_config(2, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), c);
        }
        public override void driver_init()
        {
            uint[] bank = { 0x0c000, 0x10000, 0x11000, 0x12000, 0x0c000, 0x0c000, 0x0c000, 0x13000 };
            williams.defender_bank_list = bank;

            /* CMOS configuration */
            CONFIGURE_CMOS(0xc400, 0x100);

            /* PIA configuration */
            CONFIGURE_PIAS(defender_pia_0_intf, williams.williams_pia_1_intf, williams.williams_snd_pia_intf);
        }
        Mame.InputPortTiny[] input_ports_defender()
        {
            INPUT_PORTS_START("defender");
            PORT_START("IN0");
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, "Fire", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2, "Thrust", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3, "Smart Bomb", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON4, "Hyperspace", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BITX(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON6, "Reverse", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0xfe, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BITX(0x01, IP_ACTIVE_HIGH, 0, "Auto Up", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, 0, "Advance", (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, 0, "High Score Reset", (ushort)Mame.InputCodes.KEYCODE_7, IP_JOY_NONE);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_TILT);

            PORT_START("IN3");      /* IN3 - fake port for better joystick control */
            /* This fake port is handled via defender_input_port_1 */
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_CHEAT);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_CHEAT);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_defender()
        {
            ROM_START("defender");
            ROM_REGION(0x14000, Mame.REGION_CPU1);
            ROM_LOAD("defend.1", 0x0d000, 0x0800, 0xc3e52d7e);
            ROM_LOAD("defend.4", 0x0d800, 0x0800, 0x9a72348b);
            ROM_LOAD("defend.2", 0x0e000, 0x1000, 0x89b75984);
            ROM_LOAD("defend.3", 0x0f000, 0x1000, 0x94f51e9b);
            /* bank 0 is the place for CMOS ram */
            ROM_LOAD("defend.9", 0x10000, 0x0800, 0x6870e8a5);
            ROM_LOAD("defend.12", 0x10800, 0x0800, 0xf1f88938);
            ROM_LOAD("defend.8", 0x11000, 0x0800, 0xb649e306);
            ROM_LOAD("defend.11", 0x11800, 0x0800, 0x9deaf6d9);
            ROM_LOAD("defend.7", 0x12000, 0x0800, 0x339e092e);
            ROM_LOAD("defend.10", 0x12800, 0x0800, 0xa543b167);
            ROM_RELOAD(0x13800, 0x0800);
            ROM_LOAD("defend.6", 0x13000, 0x0800, 0x65f4efd1);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for the sound CPU */
            ROM_LOAD("defend.snd", 0xf800, 0x0800, 0xfefd5b48);
            return ROM_END;
        }
        public driver_defender()
        {
            drv = new machine_driver_defender();
            year = "1980";
            name = "defender";
            description = "Defender (Red label)";
            manufacturer = "Williams";
            flags = Mame.ROT0;
            input_ports = input_ports_defender();
            rom = rom_defender();
            drv.HasNVRAMhandler = true;
        }
    }
}
