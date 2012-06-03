using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_qix : Mame.GameDriver
    {
        static int suspended;

        static int sdungeon_coinctrl;
        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];

        static _BytePtr qix_palettebank = new _BytePtr(1);
        static _BytePtr qix_videoaddress = new _BytePtr(1);
        static _BytePtr qix_sharedram = new _BytePtr(1);

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, qix_sharedram_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8800, 0x8800, Mame.MRA_RAM ),   /* ACIA */
	new Mame.MemoryReadAddress( 0x9000, 0x9003, _6821pia.pia_3_r ),
	new Mame.MemoryReadAddress( 0x9400, 0x9403, _6821pia.pia_0_r ),
	new Mame.MemoryReadAddress( 0x9900, 0x9903, _6821pia.pia_1_r ),
	new Mame.MemoryReadAddress( 0x9c00, 0x9FFF, _6821pia.pia_2_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 ) /* end of table */
};
        static Mame.MemoryReadAddress[] readmem_video =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, qix_videoram_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, qix_sharedram_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9400, 0x9400, qix_addresslatch_r ),
	new Mame.MemoryReadAddress( 0x9800, 0x9800, qix_scanline_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 ) /* end of table */
};
        static Mame.MemoryReadAddress[] readmem_sound =
{
	new  Mame.MemoryReadAddress( 0x0000, 0x007f, Mame.MRA_RAM ),
	new  Mame.MemoryReadAddress( 0x2000, 0x2003, _6821pia.pia_5_r ),
	new  Mame.MemoryReadAddress( 0x4000, 0x4003, _6821pia.pia_4_r ),
	new  Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),
	new  Mame.MemoryReadAddress( -1 ) /* end of table */
};
        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, qix_sharedram_w, qix_sharedram ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8c00, 0x8c00, qix_video_firq_w ),
	new Mame.MemoryWriteAddress( 0x9000, 0x9003, _6821pia.pia_3_w ),
	new Mame.MemoryWriteAddress( 0x9400, 0x9403, sdungeon_pia_0_w ),
	new Mame.MemoryWriteAddress( 0x9900, 0x9903, _6821pia.pia_1_w ),
	new Mame.MemoryWriteAddress( 0x9c00, 0x9fff, _6821pia.pia_2_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 ) /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_video =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x7fff, qix_videoram_w ),
	new  Mame.MemoryWriteAddress( 0x8000, 0x83ff, qix_sharedram_w ),
	new  Mame.MemoryWriteAddress( 0x8400, 0x87ff, Mame.MWA_RAM, nvram, nvram_size ),
	new  Mame.MemoryWriteAddress( 0x8800, 0x8800, qix_palettebank_w, qix_palettebank ),
	new  Mame.MemoryWriteAddress( 0x8c00, 0x8c00, qix_data_firq_w ),
	new  Mame.MemoryWriteAddress( 0x9000, 0x93ff, qix_paletteram_w, Mame.paletteram ),
	new  Mame.MemoryWriteAddress( 0x9400, 0x9400, qix_addresslatch_w ),
	new  Mame.MemoryWriteAddress( 0x9402, 0x9403, Mame.MWA_RAM, qix_videoaddress ),
	new  Mame.MemoryWriteAddress( 0x9c00, 0x9FFF, Mame.MWA_RAM ), /* Video controller */
	new  Mame.MemoryWriteAddress( 0xa000, 0xffff, Mame.MWA_ROM ),
	new  Mame.MemoryWriteAddress( -1 ) /* end of table */
};
        static Mame.MemoryWriteAddress[] writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x007f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2003, _6821pia.pia_5_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x4003, _6821pia.pia_4_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 ) /* end of table */
};

        static void qix_sharedram_w(int offset, int data)
        {
            qix_sharedram[offset] = (byte)data;
        }
        static int qix_sharedram_r(int offset)
        {
            return qix_sharedram[offset];
        }
        static int qix_videoram_r(int offset)
        {
            offset += (qix_videoaddress[0] & 0x80) * 0x100;
            return Generic.videoram[offset];
        }
        static void qix_videoram_w(int offset, int data)
        {
            int x, y;

            offset += (qix_videoaddress[0] & 0x80) * 0x100;

            x = offset & 0xff;
            y = offset >> 8;

            Mame.plot_pixel(Mame.Machine.scrbitmap, x, y, Mame.Machine.pens.read16(data));

            Generic.videoram[offset] = (byte)data;
        }
        static void update_pen(int pen, int val)
        {
            /* this conversion table should be about right. It gives a reasonable */
            /* gray scale in the test screen, and the red, green and blue squares */
            /* in the same screen are barely visible, as the manual requires. */
            byte[] table =
	{
		0x00,	/* value = 0, intensity = 0 */
		0x12,	/* value = 0, intensity = 1 */
		0x24,	/* value = 0, intensity = 2 */
		0x49,	/* value = 0, intensity = 3 */
		0x12,	/* value = 1, intensity = 0 */
		0x24,	/* value = 1, intensity = 1 */
		0x49,	/* value = 1, intensity = 2 */
		0x92,	/* value = 1, intensity = 3 */
		0x5b,	/* value = 2, intensity = 0 */
		0x6d,	/* value = 2, intensity = 1 */
		0x92,	/* value = 2, intensity = 2 */
		0xdb,	/* value = 2, intensity = 3 */
		0x7f,	/* value = 3, intensity = 0 */
		0x91,	/* value = 3, intensity = 1 */
		0xb6,	/* value = 3, intensity = 2 */
		0xff	/* value = 3, intensity = 3 */
	};

            int bits, intensity, red, green, blue;

            intensity = (val >> 0) & 0x03;
            bits = (val >> 6) & 0x03;
            red = table[(bits << 2) | intensity];
            bits = (val >> 4) & 0x03;
            green = table[(bits << 2) | intensity];
            bits = (val >> 2) & 0x03;
            blue = table[(bits << 2) | intensity];

            Mame.palette_change_color(pen, (byte)red, (byte)green, (byte)blue);
        }


        static void qix_palettebank_w(int offset, int data)
        {
            if ((qix_palettebank[0] & 0x03) != (data & 0x03))
            {
                _BytePtr pram = new _BytePtr(Mame.paletteram, 256 * (data & 0x03));
                int i;

                for (i = 0; i < 256; i++)
                {
                    update_pen(i, pram[0]);
                    pram.offset++;
                }
            }

            qix_palettebank[0] = (byte)data;

#if DEBUG_LEDS
	data = ~(data) & 0xfc;
	if (led_log)
	{
		fprintf (led_log, "LEDS: %d %d %d %d %d %d\n", (data & 0x80)>>7, (data & 0x40)>>6,
			(data & 0x20)>>5, (data & 0x10)>>4, (data & 0x08)>>3, (data & 0x04)>>2 );
	}
#endif
        }
        static void qix_addresslatch_w(int offset, int data)
        {
            int x, y;

            offset = qix_videoaddress[0] * 0x100 + qix_videoaddress[1];

            x = offset & 0xff;
            y = offset >> 8;

            Mame.plot_pixel(Mame.Machine.scrbitmap, x, y, Mame.Machine.pens.read16(data));

            Generic.videoram[offset] = (byte)data;
        }
        static void qix_paletteram_w(int offset, int data)
        {
            Mame.paletteram[offset] = (byte)data;

            if ((qix_palettebank[0] & 0x03) == (offset / 256))
                update_pen(offset % 256, data);
        }



        static int qix_addresslatch_r(int offset)
        {
            offset = qix_videoaddress[0] * 0x100 + qix_videoaddress[1];
            return Generic.videoram[offset];
        }
        static int qix_scanline_r(int offset)
        {
            /* The +80&0xff thing is a hack to avoid flicker in Electric Yo-Yo */
            return (Mame.cpu_scalebyfcount(256) + 80) & 0xff;
        }
        static void qix_data_firq_w(int offset, int data)
        {
            /* generate firq for data cpu */
            Mame.cpu_cause_interrupt(0, Mame.cpu_m6809.M6809_INT_FIRQ);
        }
        static void qix_video_firq_w(int offset, int data)
        {
            /* generate firq for video cpu */
            Mame.cpu_cause_interrupt(1, Mame.cpu_m6809.M6809_INT_FIRQ);
        }
        static void sdungeon_pia_0_w(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"%04x: PIA 1 write offset %02x data %02x\n",cpu_get_pc(),offset,data);

            /* Hack: Kram and Zoo Keeper for some reason (protection?) leave the port A */
            /* DDR set to 0xff, so they cannot read the player 1 controls. Here I force */
            /* the DDR to 0, so the controls work correctly. */
            if (offset == 0) data = 0;

            /* make all the CPUs synchronize, and only AFTER that write the command to the PIA */
            /* otherwise the 68705 will miss commands */
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, data | (offset << 8), pia_0_w_callback);
        }
        static void pia_0_w_callback(int param)
        {
            _6821pia.pia_0_w(param >> 8, param & 0xff);
        }
        static Mame.DACinterface dac_interface = new Mame.DACinterface(1, new int[] { 100 });
        static void qix_dac_w(int offset, int data)
        {
            DAC.DAC_data_w(0, data);
        }

        static int qix_sound_r(int offset)
        {
            /* if we've suspended the main CPU for this, trigger it and give up some of our timeslice */
            if (suspended!=0)
            {
                Mame.Timer.timer_trigger(500);
                Mame.cpu_yielduntil_time(Mame.Timer.TIME_IN_USEC(100));
                suspended = 0;
            }
            return _6821pia.pia_4_porta_r(offset);
        }

        static void qix_pia_dint(int state)
        {
            /* not used by Qix, but others might use it; depends on a jumper on the PCB */
        }

        static void qix_pia_sint(int state)
        {
            /* generate a sound interrupt */
            /*	cpu_set_irq_line (2, M6809_IRQ_LINE, state ? ASSERT_LINE : CLEAR_LINE);*/

            if (state!=0)
            {
                /* ideally we should use the cpu_set_irq_line call above, but it breaks */
                /* sound in Qix */
                Mame.cpu_cause_interrupt(2, Mame.cpu_m6809.M6809_INT_IRQ);

                /* wait for the sound CPU to read the command */
                Mame.cpu_yielduntil_trigger(500);
                suspended = 1;

                /* but add a watchdog so that we're not hosed if interrupts are disabled */
                Mame.cpu_triggertime(Mame.Timer.TIME_IN_USEC(100), 500);
            }
        }
        static _6821pia.pia6821_interface qix_pia_0_intf = new _6821pia.pia6821_interface(Mame.input_port_0_r, Mame.input_port_1_r, null, null, null, null, null, null, null, null, null, null);
        static _6821pia.pia6821_interface qix_pia_1_intf = new _6821pia.pia6821_interface(Mame.input_port_2_r,Mame.input_port_3_r, null, null, null, null, null, null, null, null, null, null);
        static _6821pia.pia6821_interface qix_pia_2_intf = new _6821pia.pia6821_interface(Mame.input_port_4_r, null, null, null, null, null, null, null, null, null, null, null);
        static _6821pia.pia6821_interface qix_pia_3_intf = new _6821pia.pia6821_interface(null, null, null, null, null, null, _6821pia.pia_4_porta_w, null, _6821pia.pia_4_ca1_w, null, null, null);
        static _6821pia.pia6821_interface qix_pia_4_intf = new _6821pia.pia6821_interface(qix_sound_r, null, null, null, null, null, _6821pia.pia_3_porta_w, qix_dac_w, _6821pia.pia_3_ca1_w, null, qix_pia_sint, qix_pia_sint);
        static _6821pia.pia6821_interface qix_pia_5_intf = new _6821pia.pia6821_interface(null, null, null, null, null, null, _6821pia.pia_3_porta_w, qix_dac_w, _6821pia.pia_3_ca1_w, null, qix_pia_sint, qix_pia_sint);

        class machine_driver_qix : Mame.MachineDriver
        {
            public machine_driver_qix()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1250000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1250000, readmem_video, writemem_video, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6802, 3680000 / 4, readmem_sound, writemem_sound, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 60;
                screen_width = 256;
                screen_height = 256;
                visible_area = new Mame.rectangle(0, 255, 8, 247);
                gfxdecodeinfo = null;
                total_colors = 256;
                color_table_len = 0;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                suspended = 0;

                _6821pia.pia_unconfig();
                _6821pia.pia_config(0, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_0_intf);
                _6821pia.pia_config(1, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_1_intf);
                _6821pia.pia_config(2, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_2_intf);
                _6821pia.pia_config(3, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_3_intf);
                _6821pia.pia_config(4, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_4_intf);
                _6821pia.pia_config(5, _6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT, qix_pia_5_intf);
                _6821pia.pia_reset();

                sdungeon_coinctrl = 0x00;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write != 0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file != null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                    else
                        for (int i = 0; i < nvram_size[0]; i++) nvram[i] = 0;
                }
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                Generic.videoram = new _BytePtr(256 * 256);
                return 0;
            }
            public override void vh_stop()
            {
                Generic.videoram = null;
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* recalc the palette if necessary */
                if (Mame.palette_recalc() != null || full_refresh != 0)
                {
                    for (int offs = 0; offs < 256 * 256; offs++)
                    {
                        int x = offs & 0xff;
                        int y = offs >> 8;

                        Mame.plot_pixel(bitmap, x, y, Mame.Machine.pens.read16(Generic.videoram[offs]));
                    }
                }
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_qix()
        {
            ROM_START("qix");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code for the first CPU (Data) */
            ROM_LOAD("u12", 0xC000, 0x0800, 0xaad35508);
            ROM_LOAD("u13", 0xC800, 0x0800, 0x46c13504);
            ROM_LOAD("u14", 0xD000, 0x0800, 0x5115e896);
            ROM_LOAD("u15", 0xD800, 0x0800, 0xccd52a1b);
            ROM_LOAD("u16", 0xE000, 0x0800, 0xcd1c36ee);
            ROM_LOAD("u17", 0xE800, 0x0800, 0x1acb682d);
            ROM_LOAD("u18", 0xF000, 0x0800, 0xde77728b);
            ROM_LOAD("u19", 0xF800, 0x0800, 0xc0994776);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for code for the second CPU (Video) */
            ROM_LOAD("u4", 0xC800, 0x0800, 0x5b906a09);
            ROM_LOAD("u5", 0xD000, 0x0800, 0x254a3587);
            ROM_LOAD("u6", 0xD800, 0x0800, 0xace30389);
            ROM_LOAD("u7", 0xE000, 0x0800, 0x8ebcfa7c);
            ROM_LOAD("u8", 0xE800, 0x0800, 0xb8a3c8f9);
            ROM_LOAD("u9", 0xF000, 0x0800, 0x26cbcd55);
            ROM_LOAD("u10", 0xF800, 0x0800, 0x568be942);

            ROM_REGION(0x10000, Mame.REGION_CPU3); 	/* 64k for code for the third CPU (sound) */
            ROM_LOAD("u27", 0xF800, 0x0800, 0xf3782bd0);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_qix()
        {
            INPUT_PORTS_START("qix");
            PORT_START();	/* PIA 0 Port A (PLAYER 1) */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);

            PORT_START();	/* PIA 0 Port B (COIN) */
            PORT_BITX(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Test Advance", (ushort)Mame.InputCodes.KEYCODE_F1, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Test Next line", (ushort)Mame.InputCodes.KEYCODE_F2, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Test Slew Up", (ushort)Mame.InputCodes.KEYCODE_F5, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Test Slew Down", (ushort)Mame.InputCodes.KEYCODE_F6, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);

            PORT_START();	/* PIA 1 Port A (SPARE) */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START(); /* PIA 1 Port B (PLAYER 1/2) */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* PIA 2 Port A (PLAYER 2) */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            return INPUT_PORTS_END;
        }
        public driver_qix()
        {
            drv = new machine_driver_qix();
            year = "1981";
            name = "qix";
            description = "Qix (set 1)";
            manufacturer = "Taito America Corporation";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_qix();
            rom = rom_qix();
            drv.HasNVRAMhandler = true;
        }
    }
}
