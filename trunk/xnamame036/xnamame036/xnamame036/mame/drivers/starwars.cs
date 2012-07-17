using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_starwars : Mame.GameDriver
    {
        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];
        static byte[] slapstic_base, slapstic_area;
        static int last_bank;

        const byte kPitch = 0, kYaw = 1, kThrust = 2;
        static byte control_num = kPitch;

        static int port_A = 0, port_B = 0;
        static int irq_flag = 0;
        static int port_A_ddr = 0, port_B_ddr = 0;
        static int PA7_irq = 0;

        static int sound_data;	/* data for the sound cpu */
        static int main_data;   /* data for the main  cpu */

        /* Star Wars READ memory map */
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x2fff, Mame.MRA_RAM ),   /* vector_ram */
	new Mame.MemoryReadAddress( 0x3000, 0x3fff, Mame.MRA_ROM ),		/* vector_rom */
	new Mame.MemoryReadAddress( 0x4300, 0x431f, Mame.input_port_0_r ), /* Memory mapped input port 0 */
	new Mame.MemoryReadAddress( 0x4320, 0x433f, starwars_input_bank_1_r ), /* Memory mapped input port 1 */
	new Mame.MemoryReadAddress( 0x4340, 0x435f, Mame.input_port_2_r ),	/* DIP switches bank 0 */
	new Mame.MemoryReadAddress( 0x4360, 0x437f, Mame.input_port_3_r ),	/* DIP switches bank 1 */
	new Mame.MemoryReadAddress( 0x4380, 0x439f, starwars_control_r ), /* a-d control result */
	new Mame.MemoryReadAddress( 0x4400, 0x4400, starwars_main_read_r ),
	new Mame.MemoryReadAddress( 0x4401, 0x4401, starwars_main_ready_flag_r ),
	new Mame.MemoryReadAddress( 0x4500, 0x45ff, Mame.MRA_RAM ),		/* nov_ram */
	new Mame.MemoryReadAddress( 0x4700, 0x4700, SWMathBox.reh ),
	new Mame.MemoryReadAddress( 0x4701, 0x4701, SWMathBox.rel ),
	new Mame.MemoryReadAddress( 0x4703, 0x4703, SWMathBox.prng ),			/* pseudo random number generator */
/*	new Mame.MemoryReadAddress( 0x4800, 0x4fff, MRA_RAM }, */		/* cpu_ram */
/*	new Mame.MemoryReadAddress( 0x5000, 0x5fff, MRA_RAM }, */		/* (math_ram_r) math_ram */
	new Mame.MemoryReadAddress( 0x4800, 0x5fff, Mame.MRA_RAM ),		/* CPU and Math RAM */
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, Mame.MRA_BANK1 ),	    /* banked ROM */
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),		/* rest of main_rom */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        /* Star Wars Sound READ memory map */
        static Mame.MemoryReadAddress[] readmem2 =
{
	new Mame.MemoryReadAddress( 0x0800, 0x0fff, starwars_sin_r ),		/* SIN Read */
	new Mame.MemoryReadAddress( 0x1000, 0x107f, Mame.MRA_RAM ),	/* 6532 RAM */
	new Mame.MemoryReadAddress( 0x1080, 0x109f, starwars_m6532_r ),
	new Mame.MemoryReadAddress (0x2000, 0x27ff, Mame.MRA_RAM ),	/* program RAM */
	new Mame.MemoryReadAddress (0x4000, 0xbfff, Mame.MRA_ROM ),	/* sound roms */
	new Mame.MemoryReadAddress (0xc000, 0xffff, Mame.MRA_ROM ),	/* load last rom twice */
	                        								/* for proper int vec operation */
	new Mame.MemoryReadAddress (-1 )  /* end of table */
};

        /* Star Wars WRITE memory map */
        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x2fff, Mame.MWA_RAM,  Vector.vectorram, Vector.vectorram_size ), /* vector_ram */
	new Mame.MemoryWriteAddress( 0x3000, 0x3fff, Mame.MWA_ROM ),		/* vector_rom */
	new Mame.MemoryWriteAddress( 0x4400, 0x4400, starwars_main_wr_w ),
	new Mame.MemoryWriteAddress( 0x4500, 0x45ff, Mame.MWA_RAM, nvram, nvram_size ),		/* nov_ram */
	new Mame.MemoryWriteAddress( 0x4600, 0x461f, AvgDvg.avgdvg_go ),
	new Mame.MemoryWriteAddress( 0x4620, 0x463f, AvgDvg.avgdvg_reset ),
	new Mame.MemoryWriteAddress( 0x4640, 0x465f, Mame.MWA_NOP ),		/* (wdclr) Watchdog clear */
	new Mame.MemoryWriteAddress( 0x4660, 0x467f, Mame.MWA_NOP ),        /* irqclr: clear periodic interrupt */
	new Mame.MemoryWriteAddress( 0x4680, 0x4687, starwars_out_w ),
	new Mame.MemoryWriteAddress( 0x46a0, 0x46bf, Mame.MWA_NOP ),		/* nstore */
	new Mame.MemoryWriteAddress( 0x46c0, 0x46c2, starwars_control_w ),	/* Selects which a-d control port (0-3) will be read */
	new Mame.MemoryWriteAddress( 0x46e0, 0x46e0, starwars_soundrst ),
	new Mame.MemoryWriteAddress( 0x4700, 0x4707, SWMathBox.swmathbx ),
/*	new Mame.MemoryWriteAddress( 0x4800, 0x4fff, MWA_RAM }, */		/* cpu_ram */
/*	new Mame.MemoryWriteAddress( 0x5000, 0x5fff, MWA_RAM }, */		/* (math_ram_w) math_ram */
	new Mame.MemoryWriteAddress( 0x4800, 0x5fff, Mame.MWA_RAM ),		/* CPU and Math RAM */
	new Mame.MemoryWriteAddress( 0x6000, 0xffff, Mame.MWA_ROM ),		/* main_rom */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        /* Star Wars sound WRITE memory map */
        static Mame.MemoryWriteAddress[] writemem2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x07ff, starwars_sout_w ),
	new Mame.MemoryWriteAddress( 0x1000, 0x107f, Mame.MWA_RAM ), /* 6532 ram */
	new Mame.MemoryWriteAddress( 0x1080, 0x109f, starwars_m6532_w ),
	new Mame.MemoryWriteAddress( 0x1800, 0x183f, Pokey.quad_pokey_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x27ff, Mame.MWA_RAM ), /* program RAM */
	new Mame.MemoryWriteAddress( 0x4000, 0xbfff, Mame.MWA_ROM ), /* sound rom */
	new Mame.MemoryWriteAddress( 0xc000, 0xffff, Mame.MWA_ROM ), /* sound rom again, for intvecs */
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static void starwars_out_w (int offset, int data)
{
	_BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

	switch (offset)
	{
		case 0:		/* Coin counter 1 */
			Mame.coin_counter_w (0, data);
			break;
		case 1:		/* Coin counter 2 */
			Mame.coin_counter_w (1, data);
			break;
		case 2:		/* LED 3 */
			Mame.osd_led_w (2, data >> 7);
			break;
		case 3:		/* LED 2 */
			Mame.osd_led_w (1, data >> 7);
			break;
		case 4:
//			if (errorlog) fprintf (errorlog, "bank_switch_w, %02x\n", data);
			if ((data & 0x80)!=0)
			{
				Mame.cpu_setbank(1,new _BytePtr(RAM, 0x10000));
                Mame.cpu_setbank(2, new _BytePtr(RAM,0x1c000));
			}
			else
			{
				Mame.cpu_setbank(1,new _BytePtr(RAM,0x06000));
				Mame.cpu_setbank(2,new _BytePtr(RAM,0x0a000));
			}
			break;
		case 5:
			SWMathBox.prngclr (offset, data);
			break;
		case 6:
			Mame.osd_led_w (0, data >> 7);
			break;	/* LED 1 */
		case 7:
			Mame.printf( "recall\n"); /* what's that? */
			break;
	}
}
        static int starwars_input_bank_1_r(int offset)
        {
            int x;
            x = Mame.input_port_1_r(0); /* Read memory mapped port 2 */

#if false
	x=x&0x34; /* Clear out bit 3 (SPARE 2), and 0 and 1 (UNUSED) */
	/* MATH_RUN (bit 7) set to 0 */
	x=x|(0x40);  /* Set bit 6 to 1 (VGHALT) */
#endif

            /* Kludge to enable Starwars Mathbox Self-test                  */
            /* The mathbox looks like it's running, from this address... :) */
            if (Mame.cpu_get_pc() == 0xf978)
                x |= 0x80;

            /* Kludge to enable Empire Mathbox Self-test                  */
            /* The mathbox looks like it's running, from this address... :) */
            if (Mame.cpu_get_pc() == 0xf655)
                x |= 0x80;

            if (AvgDvg.avgdvg_done())
                x |= 0x40;
            else
                x &= ~0x40;

            return x;
        }
        /*********************************************************/
        /********************************************************/
        static int starwars_control_r(int offset)
        {

            if (control_num == kPitch)
                return Mame.readinputport(4);
            else if (control_num == kYaw)
                return Mame.readinputport(5);
            /* default to unused thrust */
            else return 0;
        }

        static void starwars_control_w(int offset, int data)
        {
            control_num = (byte)offset;
        }
        static int starwars_main_read_r(int offset)
        {
            int res;

            //if (errorlog)fprintf(errorlog, "main_read_r\n");

            port_A &= 0xbf;  /* ready to receive new commands from sound cpu */
            res = main_data;
            main_data = 0;
            return res;
        }
        static int starwars_main_ready_flag_r(int offset)
        {
#if false
            /* correct, but doesn't work */
	return (port_A & 0xc0); /* only upper two flag bits mapped */
#else
            return (port_A & 0x40); /* sound cpu always ready */
#endif
        }
        static int starwars_sin_r(int offset)
        {
            int res;

            port_A &= 0x7f; /* ready to receive new commands from main */
            res = sound_data;
            sound_data = 0;
            return res;
        }
        static int temp;
        static int starwars_m6532_r(int offset)
        {

            switch (offset)
            {
                case 0: /* 0x80 - Read Port A */

                    /* Note: bit 4 is always set to avoid sound self test */

                    return port_A | 0x10 | ((!tms5220.tms5220_ready_r()?1:0) << 2);

                case 1: /* 0x81 - Read Port A DDR */
                    return port_A_ddr;

                case 2: /* 0x82 - Read Port B */
                    return port_B;  /* speech data read? */

                case 3: /* 0x83 - Read Port B DDR */
                    return port_B_ddr;

                case 5: /* 0x85 - Read Interrupt Flag Register */
                    temp = irq_flag;
                    irq_flag = 0;   /* Clear int flags */
                    return temp;

                default:
                    return 0;
            }

            return 0; /* will never execute this */
        }
        static void starwars_main_wr_w(int offset, int data)
        {
            port_A |= 0x80;  /* command from main cpu pending */
            sound_data = data;
            if (PA7_irq != 0)
                Mame.cpu_cause_interrupt(1, Mame.cpu_m6809.M6809_INT_IRQ);
        }
        static void starwars_soundrst(int offset, int data)
        {
            port_A &= 0x3f;

            /* reset sound CPU here  */
            Mame.cpu_set_reset_line(1, Mame.PULSE_LINE);
        }
        static void starwars_sout_w(int offset, int data)
        {
            port_A |= 0x40; /* result from sound cpu pending */
            main_data = data;
            return;
        }
        static void starwars_m6532_w(int offset, int data)
        {
            switch (offset)
            {
                case 0: /* 0x80 - Port A Write */

                    /* Write to speech chip on PA0 falling edge */

                    if ((port_A & 0x01) == 1)
                    {
                        port_A = (port_A & (~port_A_ddr)) | (data & port_A_ddr);
                        if ((port_A & 0x01) == 0)
                            tms5220.tms5220_data_w(0, port_B);
                    }
                    else
                        port_A = (port_A & (~port_A_ddr)) | (data & port_A_ddr);

                    return;

                case 1: /* 0x81 - Port A DDR Write */
                    port_A_ddr = data;
                    return;

                case 2: /* 0x82 - Port B Write */
                    /* TMS5220 Speech Data on port B */

                    /* ignore DDR for now */
                    port_B = data;

                    return;

                case 3: /* 0x83 - Port B DDR Write */
                    port_B_ddr = data;
                    return;

                case 7: /* 0x87 - Enable Interrupt on PA7 Transitions */

                    /* This feature is emulated now.  When the Main CPU  */
                    /* writes to mainwrite, it may send an IRQ to the    */
                    /* sound CPU, depending on the state of this flag.   */

                    PA7_irq = data;
                    return;


                case 0x1f: /* 0x9f - Set Timer to decrement every n*1024 clocks, */
                    /*        With IRQ enabled on countdown               */

                    /* Should be decrementing every data*1024 6532 clock cycles */
                    /* 6532 runs at 1.5 Mhz, so there a 3 cylces in 2 usec */

                    Mame.Timer.timer_set(Mame.Timer.TIME_IN_USEC((1024 * 2 / 3) * data), 0, snd_interrupt);
                    return;

                default:
                    return;
            }

            return; /* will never execute this */

        }
        static void snd_interrupt(int foo)
        {
            irq_flag |= 0x80; /* set timer interrupt flag */
            Mame.cpu_cause_interrupt(1, Mame.cpu_m6809.M6809_INT_IRQ);
        }
        static TMS5220interface tms5220_interface = new TMS5220interface(640000, 50, null);
        static POKEYinterface pokey_interface = new POKEYinterface(
            4,1500000,new int[]{20,20,20,20},
             new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            new pot_delegate[] { null, null, null, null },
            /* The allpot handler */
            new pot_delegate[] { null, null, null, null },
             new pot_delegate[] { null, null, null, null },
             new pokey_serout[] { null, null, null, null },
             new Mame.irqcallback[] { null, null, null, null }
            );
        class machine_driver_starwars : Mame.MachineDriver
        {
            public machine_driver_starwars()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1500000, readmem, writemem, null, null, Mame.interrupt, 6));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809 | Mame.CPU_AUDIO_CPU, 1500000, readmem2, writemem2, null, null, null, 0, null, 0));
                frames_per_second = 30;
                vblank_duration = 0;
                cpu_slices_per_frame = 1;
                screen_width = 400;
                screen_height = 300;
                visible_area = new Mame.rectangle(0, 250, 0, 280);
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_POKEY, pokey_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_TMS5220, tms5220_interface));
            }
            public override void init_machine()
            {
                SWMathBox.init_swmathbox();
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
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                AvgDvg.avg_init_palette(AvgDvg.VEC_PAL_SWARS, palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return AvgDvg.avg_start_starwars();
            }
            public override void vh_stop()
            {
                AvgDvg.avg_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                AvgDvg.avg_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            SWMathBox.init_starwars();
        }
        Mame.RomModule[] rom_starwars()
        {
            ROM_START("starwars");
            ROM_REGION(0x12000, Mame.REGION_CPU1);    /* 2 64k ROM spaces */
            ROM_LOAD("136021.105", 0x3000, 0x1000, 0x538e7d2f);/* 3000-3fff is 4k vector rom */
            ROM_LOAD("136021.214", 0x6000, 0x2000, 0x04f1876e);   /* ROM 0 bank pages 0 and 1 */
            ROM_CONTINUE(0x10000, 0x2000);
            ROM_LOAD("136021.102", 0x8000, 0x2000, 0xf725e344);/*  8k ROM 1 bank */
            ROM_LOAD("136021.203", 0xa000, 0x2000, 0xf6da0a00);/*  8k ROM 2 bank */
            ROM_LOAD("136021.104", 0xc000, 0x2000, 0x7e406703);/*  8k ROM 3 bank */
            ROM_LOAD("136021.206", 0xe000, 0x2000, 0xc7e51237);/*  8k ROM 4 bank */

            /* Load the Mathbox PROM's temporarily into the Vector RAM area */
            /* During initialisation they will be converted into useable form */
            /* and stored elsewhere. */
            ROM_LOAD("136021.110", 0x0000, 0x0400, 0x01061762); /* PROM 0 */
            ROM_LOAD("136021.111", 0x0400, 0x0400, 0x2e619b70); /* PROM 1 */
            ROM_LOAD("136021.112", 0x0800, 0x0400, 0x6cfa3544); /* PROM 2 */
            ROM_LOAD("136021.113", 0x0c00, 0x0400, 0x03f6acb2); /* PROM 3 */

            /* Sound ROMS */
            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* Really only 32k, but it looks like 64K */
            ROM_LOAD("136021.107", 0x4000, 0x2000, 0xdbf3aea2);/* Sound ROM 0 */
            ROM_RELOAD(0xc000, 0x2000); /* Copied again for */
            ROM_LOAD("136021.208", 0x6000, 0x2000, 0xe38070a8); /* Sound ROM 0 */
            ROM_RELOAD(0xe000, 0x2000); /* proper int vecs */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_starwars()
        {
            INPUT_PORTS_START("starwars");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_SERVICE(0x10, IP_ACTIVE_LOW);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BITX(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Diagnostic Step", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            /* Bit 6 is MATH_RUN - see machine/starwars.c */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            /* Bit 7 is VG_HALT - see machine/starwars.c */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* DSW0 */
            PORT_DIPNAME(0x03, 0x00, "Starting Shields");
            PORT_DIPSETTING(0x00, "6");
            PORT_DIPSETTING(0x01, "7");
            PORT_DIPSETTING(0x02, "8");
            PORT_DIPSETTING(0x03, "9");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x04, "Moderate");
            PORT_DIPSETTING(0x08, "Hard");
            PORT_DIPSETTING(0x0c, "Hardest");
            PORT_DIPNAME(0x30, 0x00, "Bonus Shields");
            PORT_DIPSETTING(0x00, "0");
            PORT_DIPSETTING(0x10, "1");
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPNAME(0x40, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, "Freeze");
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x02, DEF_STR(Coinage));
            PORT_DIPSETTING(0x03, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            PORT_DIPNAME(0x0c, 0x00, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x00, "*1");
            PORT_DIPSETTING(0x04, "*4");
            PORT_DIPSETTING(0x08, "*5");
            PORT_DIPSETTING(0x0c, "*6");
            PORT_DIPNAME(0x10, 0x00, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x00, "*1");
            PORT_DIPSETTING(0x10, "*2");
            PORT_DIPNAME(0xe0, 0x00, "Bonus Coinage");
            PORT_DIPSETTING(0x20, "2 gives 1");
            PORT_DIPSETTING(0x60, "4 gives 2");
            PORT_DIPSETTING(0xa0, "3 gives 1");
            PORT_DIPSETTING(0x40, "4 gives 1");
            PORT_DIPSETTING(0x80, "5 gives 1");
            PORT_DIPSETTING(0x00, "None");
            /* 0xc0 and 0xe0 None */

            PORT_START();	/* IN4 */
            PORT_ANALOG(0xff, 0x80, (uint)inptports.IPT_AD_STICK_Y, 70, 30, 0, 255);

            PORT_START();	/* IN5 */
            PORT_ANALOG(0xff, 0x80, (uint)inptports.IPT_AD_STICK_X, 50, 30, 0, 255);
            return INPUT_PORTS_END;
        }
        public driver_starwars()
        {
            drv = new machine_driver_starwars();
            year = "1983";
            name = "starwars";
            description = "Star Wars (rev2)";
            manufacturer = "Atari";
            flags = Mame.ROT0;
            input_ports = input_ports_starwars();
            rom = rom_starwars();
            drv.HasNVRAMhandler = true;
        }
    }
}
