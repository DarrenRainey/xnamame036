using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_speedfrk : Mame.GameDriver
    {
        static Mame.MemoryReadAddress[] readmem = 
        {
            new Mame.MemoryReadAddress(0x0000,0x7fff,Mame.MRA_ROM),
            new Mame.MemoryReadAddress(-1)
        };
        static Mame.MemoryWriteAddress[] writemem =
        {
            new Mame.MemoryWriteAddress(0x0000,0x7fff,Mame.MWA_ROM),
            new Mame.MemoryWriteAddress(-1),
        };
        delegate void cinemat_soundhandler(byte a, byte b);
        static cinemat_soundhandler cinemat_sound_handler;

        static int cinemat_outputs = 0xff;
        static void cinemat_writeport(int offset, int data)
        {
            switch (offset)
            {
                case Mame.cpu_ccpu.CCPU_PORT_IOOUTPUTS:
                    if (((cinemat_outputs ^ data) & 0x9f) != 0)
                    {
                        if (cinemat_sound_handler != null)
                            cinemat_sound_handler((byte)(data & 0x9f), (byte)((cinemat_outputs ^ data) & 0x9f));

                    }
                    cinemat_outputs = data;
                    break;
            }
        }

        static Mame.IOReadPort[] speedfrk_readport =
{
	new Mame.IOReadPort( 0, Mame.cpu_ccpu.CCPU_PORT_MAX, speedfrk_readports ),
	new Mame.IOReadPort( -1 )  /* end of table */
};
        static Mame.IOWritePort[] writeport =
{
	new Mame.IOWritePort( 0, Mame.cpu_ccpu.CCPU_PORT_MAX, cinemat_writeport ),
	new Mame.IOWritePort( -1 )  /* end of table */
};

        static byte[] speedfrk_steer = { 0xe, 0x6, 0x2, 0x0, 0x3, 0x7, 0xf };

        static int last_wheel = 0, delta_wheel, last_frame = 0, gear = 0xe0;
        static int speedfrk_in2_r(int offset)
        {
            int val, current_frame;

            /* check the fake gear input port and determine the bit settings for the gear */
            if ((Mame.input_port_4_r(0) & 0xf0) != 0xf0)
                gear = Mame.input_port_4_r(0) & 0xf0;

            val = gear;

            /* add the start key into the mix */
            if ((Mame.input_port_2_r(0) & 0x80) != 0)
                val |= 0x80;
            else
                val &= ~0x80;

            /* and for the cherry on top, we add the scrambled analog steering */
            current_frame = Mame.cpu_getcurrentframe();
            if (current_frame > last_frame)
            {
                /* the shift register is cleared once per 'frame' */
                delta_wheel = Mame.input_port_3_r(0) - last_wheel;
                last_wheel += delta_wheel;
                if (delta_wheel > 3)
                    delta_wheel = 3;
                else if (delta_wheel < -3)
                    delta_wheel = -3;
            }
            last_frame = current_frame;

            val |= speedfrk_steer[delta_wheel + 3];

            return val;
        }

        static int speedfrk_readports(int offset)
        {
            switch (offset)
            {
                case Mame.cpu_ccpu.CCPU_PORT_IOSWITCHES:
                    return Mame.readinputport(0);

                case Mame.cpu_ccpu.CCPU_PORT_IOINPUTS:
                    return (Mame.readinputport(1) << 8) + speedfrk_in2_r(0);

                case Mame.cpu_ccpu.CCPU_PORT_IOOUTPUTS:
                    return cinemat_outputs;
            }

            return 0;
        }

        static int cinemat_clear_list()
        {
            if (Mame.osd_skip_this_frame() != 0)
                Vector.vector_clear_list();
            return Mame.ignore_interrupt();
        }


        static string[] speedfrk_sample_names =
{
    null	/* end of array */
};

        static Mame.Samplesinterface speedfrk_samples_interface =
        new Mame.Samplesinterface(
            8,	/* 8 channels */
            25,	/* volume */
            speedfrk_sample_names
        );

        class machine_driver_speedfrk : Mame.MachineDriver
        {
            public machine_driver_speedfrk()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_CCPU, 5000000, readmem, writemem, speedfrk_readport, writeport, cinemat_clear_list, 1));
                frames_per_second = 38;
                vblank_duration = 0;
                cpu_slices_per_frame = 1;
                screen_width = 400;
                screen_height = 300;
                visible_area = new Mame.rectangle(0, 1024, 0,768);
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, speedfrk_samples_interface));
            }
            public override void init_machine()
            {
                Mame.cpu_ccpu.ccpu_Config(0, Mame.cpu_ccpu.CCPU_MEMSIZE_8K, Mame.cpu_ccpu.CCPU_MONITOR_BILEV);
                cinemat_sound_handler = null;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                cinemat.cinemat_init_colors(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return cinemat.cinemat_vh_start();
            }
            public override void vh_stop()
            {
                cinemat.cinemat_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                cinemat.cinemat_vh_screenrefresh(bitmap,full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }

        const byte SW7 = 0x40;
        const byte SW6 = 0x02;
        const byte SW5 = 0x04;
        const byte SW4 = 0x08;
        const byte SW3 = 0x01;
        const byte SW2 = 0x20;
        const byte SW1 = 0x10;

        const byte SW7OFF = SW7;
        const byte SW6OFF = SW6;
        const byte SW5OFF = SW5;
        const byte SW4OFF = SW4;
        const byte SW3OFF = SW3;
        const byte SW2OFF = SW2;
        const byte SW1OFF = SW1;

        const byte SW7ON = 0;
        const byte SW6ON = 0;
        const byte SW5ON = 0;
        const byte SW4ON = 0;
        const byte SW3ON = 0;
        const byte SW2ON = 0;
        const byte SW1ON = 0;

        public override void driver_init()
        {
            cinemat.cinemat_select_artwork(Mame.cpu_ccpu.CCPU_MONITOR_BILEV, 0, 0, null);
        }
        Mame.RomModule[] rom_speedfrk()
        {
            ROM_START("speedfrk");
            ROM_REGION(0x2000, Mame.REGION_CPU1);	/* 8k for code */
            ROM_LOAD_GFX_EVEN("speedfrk.t7", 0x0000, 0x0800, 0x3552c03f);
            ROM_LOAD_GFX_ODD("speedfrk.p7", 0x0000, 0x0800, 0x4b90cdec);
            ROM_LOAD_GFX_EVEN("speedfrk.u7", 0x1000, 0x0800, 0x616c7cf9);
            ROM_LOAD_GFX_ODD("speedfrk.r7", 0x1000, 0x0800, 0xfbe90d63);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_speedfrk()
        {

            INPUT_PORTS_START("speedfrk");
            PORT_START(); /* switches */
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 1);
            PORT_DIPNAME(SW7, SW7OFF, DEF_STR("Unknown"));
            PORT_DIPSETTING(SW7OFF, DEF_STR("Off"));
            PORT_DIPSETTING(SW7ON, DEF_STR("On"));
            PORT_DIPNAME(SW6, SW6OFF, DEF_STR("Unknown"));
            PORT_DIPSETTING(SW6OFF, DEF_STR("Off"));
            PORT_DIPSETTING(SW6ON, DEF_STR("On"));
            PORT_DIPNAME(SW5, SW5OFF, DEF_STR("Unknown"));
            PORT_DIPSETTING(SW5OFF, DEF_STR("Off"));
            PORT_DIPSETTING(SW5ON, DEF_STR("On"));
            PORT_DIPNAME(SW4, SW4OFF, DEF_STR("Unknown"));
            PORT_DIPSETTING(SW4OFF, DEF_STR("Off"));
            PORT_DIPSETTING(SW4ON, DEF_STR("On"));
            PORT_DIPNAME(SW3, SW3OFF, DEF_STR("Unknown"));
            PORT_DIPSETTING(SW3OFF, DEF_STR("Off"));
            PORT_DIPSETTING(SW3ON, DEF_STR("On"));
            PORT_DIPNAME(SW2 | SW1, SW2OFF | SW1ON, "Extra Time");
            PORT_DIPSETTING(SW2ON | SW1ON, "69");
            PORT_DIPSETTING(SW2ON | SW1OFF, "99");
            PORT_DIPSETTING(SW2OFF | SW1ON, "129");
            PORT_DIPSETTING(SW2OFF | SW1OFF, "159");

            PORT_START(); /* inputs high */
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1); /* gas */

            PORT_START(); /* inputs low */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x70, IP_ACTIVE_LOW, IPT_UNUSED); /* actually the gear shift, see fake below */
            PORT_ANALOG(0x0f, 0x04, (uint)inptports.IPT_AD_STICK_X | IPF_CENTER, 25, 1, 0x00, 0x08);

            PORT_START(); /* steering wheel */
            PORT_ANALOG(0xff, 0x00, (uint)inptports.IPT_DIAL, 100, 1, 0x00, 0xff);

            PORT_START(); /* in4 - fake for gear shift */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BITX(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2, "1st gear", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2, "2nd gear", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2, "3rd gear", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2, "4th gear", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            return INPUT_PORTS_END;
        }
        public driver_speedfrk()
        {

            drv = new machine_driver_speedfrk();
            year = "19??";
            name = "speedfrk";
            description = "Speed Freak";
            manufacturer = "Vectorbeam";
            flags = Mame.ROT0;
            input_ports = input_ports_speedfrk();
            rom = rom_speedfrk();
            drv.HasNVRAMhandler = false;
        }
    }
}
