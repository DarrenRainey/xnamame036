using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_asteroid : Mame.GameDriver
    {
        public static int asteroid_IN0_r(int offset)
        {
            int res;
            int bitmask;

            res = Mame.readinputport(0);

            bitmask = (1 << offset);

            if ((Mame.cpu_gettotalcycles() & 0x100) != 0)
                res |= 0x02;
            if (!AvgDvg.avgdvg_done())
                res |= 0x04;

            if ((res & bitmask) != 0)
                res = 0x80;
            else
                res = ~0x80;

            return res;
        }
        public static int asteroib_IN0_r(int offset)
        {
            int res;

            res = Mame.readinputport(0);

            //	if (cpu_gettotalcycles() & 0x100)
            //		res |= 0x02;
            if (!AvgDvg.avgdvg_done())
                res |= 0x80;

            return res;
        }

        /*
         * These 7 memory locations are used to read the player's controls.
         * Typically, only the high bit is used. This is handled by one input port.
         */
        public static int asteroid_IN1_r(int offset)
        {
            int res;
            int bitmask;

            res = Mame.readinputport(1);
            bitmask = (1 << offset);

            if ((res & bitmask) != 0)
                res = 0x80;
            else
                res = ~0x80;
            return (res);
        }
        public static int asteroid_DSW1_r(int offset)
        {
            int res;
            int res1;

            res1 = Mame.readinputport(2);

            res = 0xfc | ((res1 >> (2 * (3 - (offset & 0x3)))) & 0x3);
            return res;
        }
        static int asteroid_bank = 0;
        static void asteroid_bank_switch_w(int offset, int data)
        {
            int asteroid_newbank;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            asteroid_newbank = (data >> 2) & 1;
            if (asteroid_bank != asteroid_newbank)
            {
                /* Perform bankswitching on page 2 and page 3 */
                int temp;
                int i;

                asteroid_bank = asteroid_newbank;
                for (i = 0; i < 0x100; i++)
                {
                    temp = RAM[0x200 + i];
                    RAM[0x200 + i] = RAM[0x300 + i];
                    RAM[0x300 + i] = (byte)temp;
                }
            }
            Mame.osd_led_w(0, ~(data >> 1));
            Mame.osd_led_w(1, ~data);
        }
        static int asteroid_interrupt()
        {
            /* Turn off interrupts if self-test is enabled */
            if ((Mame.readinputport(0) & 0x80) != 0)
                return Mame.ignore_interrupt();
            else
                return Mame.nmi_interrupt();
        }

        static int channel, explosion_latch, thump_latch;
        static int[] sound_latch = new int[8];
        static int polynome, thump_frequency;

        static void asteroid_explode_w(int offset, int data)
        {
            if (data == explosion_latch)
                return;

            Mame.stream_update(channel, 0);
            explosion_latch = data;
        }
        static void asteroid_thump_w(int offset, int data)
        {
            double r0 = 1 / 47000, r1 = 1 / 1e12;

            if (data == thump_latch)
                return;

            Mame.stream_update(channel, 0);
            thump_latch = data;

            if ((thump_latch & 1) != 0)
                r1 += 1.0 / 220000;
            else
                r0 += 1.0 / 220000;
            if ((thump_latch & 2) != 0)
                r1 += 1.0 / 100000;
            else
                r0 += 1.0 / 100000;
            if ((thump_latch & 4) != 0)
                r1 += 1.0 / 47000;
            else
                r0 += 1.0 / 47000;
            if ((thump_latch & 8) != 0)
                r1 += 1.0 / 22000;
            else
                r0 += 1.0 / 22000;

            /* NE555 setup as voltage controlled astable multivibrator
             * C = 0.22u, Ra = 22k...???, Rb = 18k
             * frequency = 1.44 / ((22k + 2*18k) * 0.22n) = 56Hz .. huh?
             */
            thump_frequency = (int)(56 + 56 * r0 / (r0 + r1));
        }
        static void asteroid_sounds_w(int offset, int data)
        {
            data &= 0x80;
            if (data == sound_latch[offset])
                return;

            Mame.stream_update(channel, 0);
            sound_latch[offset] = data;
        }


        static Mame.MemoryReadAddress[] asteroid_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x2000, 0x2007, asteroid_IN0_r ), /* IN0 */
	new Mame.MemoryReadAddress( 0x2400, 0x2407, asteroid_IN1_r ), /* IN1 */
	new Mame.MemoryReadAddress( 0x2800, 0x2803, asteroid_DSW1_r ), /* DSW1 */
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x5000, 0x57ff, Mame.MRA_ROM ), /* vector rom */
	new Mame.MemoryReadAddress( 0x6800, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xf800, 0xffff, Mame.MRA_ROM ), /* for the reset / interrupt vectors */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] asteroid_writemem =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x03ff, Mame.MWA_RAM ),
	new  Mame.MemoryWriteAddress( 0x3000, 0x3000, AvgDvg.avgdvg_go ),
	new  Mame.MemoryWriteAddress( 0x3200, 0x3200, asteroid_bank_switch_w ),
	new  Mame.MemoryWriteAddress( 0x3400, 0x3400, Mame.watchdog_reset_w ),
	new  Mame.MemoryWriteAddress( 0x3600, 0x3600, asteroid_explode_w ),
	new  Mame.MemoryWriteAddress( 0x3a00, 0x3a00, asteroid_thump_w ),
	new  Mame.MemoryWriteAddress( 0x3c00, 0x3c05, asteroid_sounds_w ),
	new  Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM, Vector.vectorram, Vector.vectorram_size ),
	new  Mame.MemoryWriteAddress( 0x5000, 0x57ff, Mame.MWA_ROM ), /* vector rom */
	new  Mame.MemoryWriteAddress( 0x6800, 0x7fff, Mame.MWA_ROM ),
	new  Mame.MemoryWriteAddress( -1)	/* end of table */
};
        class machine_driver_asteroid : Mame.MachineDriver
        {
            public machine_driver_asteroid()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502, 1500000, asteroid_readmem, asteroid_writemem, null, null, asteroid_interrupt, 4));
                frames_per_second = 60;
                vblank_duration = 0;
                cpu_slices_per_frame = 1;
                screen_width = 400;
                screen_height = 300;
                visible_area = new Mame.rectangle(0, 1040, 70, 950);
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;
                //sound.Add(new Mame.MachineSound(Mame.SOUND_CUSTOM, asteroid_custom_interface));
            }
            public override void init_machine()
            {
                asteroid_bank_switch_w(0, 0);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                AvgDvg.avg_init_palette_white(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return AvgDvg.dvg_start();
            }
            public override void vh_stop()
            {
                AvgDvg.dvg_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                AvgDvg.dvg_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //noe
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_asteroid()
        {
            ROM_START("asteroid");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("035145.02", 0x6800, 0x0800, 0x0cc75459);
            ROM_LOAD("035144.02", 0x7000, 0x0800, 0x096ed35c);
            ROM_LOAD("035143.02", 0x7800, 0x0800, 0x312caa02);
            ROM_RELOAD(0xf800, 0x0800);	/* for reset/interrupt vectors */
            /* Vector ROM */
            ROM_LOAD("035127.02", 0x5000, 0x0800, 0x8b71fd9e);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_asteroid()
        {
            INPUT_PORTS_START("asteroid");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            /* Bit 2 and 3 are handled in the machine dependent part. */
            /* Bit 2 is the 3 KHz source and Bit 3 the VG_HALT bit    */
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BITX(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, "Diagnostic Step", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_TILT);
            PORT_SERVICE(0x80, IP_ACTIVE_HIGH);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x00, "Language");
            PORT_DIPSETTING(0x00, "English");
            PORT_DIPSETTING(0x01, "German");
            PORT_DIPSETTING(0x02, "French");
            PORT_DIPSETTING(0x03, "Spanish");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Lives"));
            PORT_DIPSETTING(0x04, "3");
            PORT_DIPSETTING(0x00, "4");
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x08, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0xc0, 0x80, DEF_STR("Coinage"));
            PORT_DIPSETTING(0xc0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            return INPUT_PORTS_END;
        }
        public driver_asteroid()
        {
            drv = new machine_driver_asteroid();
            year = "1979";
            name = "asteroid";
            description = "Asteroids (rev 2)";
            manufacturer = "Atari";
            flags = Mame.ROT0;
            input_ports = input_ports_asteroid();
            rom = rom_asteroid();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_llander : Mame.GameDriver
    {
        public override void driver_init()
        {
           //none
        }
        public Mame.RomModule[] rom_llander()
        {
           ROM_START( "llander" );
           ROM_REGION(0x10000, Mame.REGION_CPU1);/* 64k for code */
	ROM_LOAD( "034572.02",    0x6000, 0x0800, 0xb8763eea );
	ROM_LOAD( "034571.02",    0x6800, 0x0800, 0x77da4b2f );
	ROM_LOAD( "034570.01",    0x7000, 0x0800, 0x2724e591 );
	ROM_LOAD( "034569.02",    0x7800, 0x0800, 0x72837a4e );
	ROM_RELOAD(            0xf800, 0x0800 );	/* for reset/interrupt vectors */
	/* Vector ROM */
	ROM_LOAD( "034599.01",    0x4800, 0x0800, 0x355a9371 );
	ROM_LOAD( "034598.01",    0x5000, 0x0800, 0x9c4ffa68 );
	/* This _should_ be the rom for international versions. */
	/* Unfortunately, is it not currently available. */
	ROM_LOAD( "034597.01",    0x5800, 0x0800, 0x00000000 );
return ROM_END;

        }
        public Mame.InputPortTiny[] input_ports_llander()
        {
            INPUT_PORTS_START("llander");
            PORT_START("IN0");
            /* Bit 0 is VG_HALT, handled in the machine dependant part */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_SERVICE(0x02, IP_ACTIVE_LOW);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            /* Of the rest, Bit 6 is the 3KHz source. 3,4 and 5 are unknown */
            PORT_BIT(0x78, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BITX(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Diagnostic Step", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BITX(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2, "Select Game", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, "Abort", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x01, "Right Coin");
            PORT_DIPSETTING(0x00, "*1");
            PORT_DIPSETTING(0x01, "*4");
            PORT_DIPSETTING(0x02, "*5");
            PORT_DIPSETTING(0x03, "*6");
            PORT_DIPNAME(0x0c, 0x00, "Language");
            PORT_DIPSETTING(0x00, "English");
            PORT_DIPSETTING(0x04, "French");
            PORT_DIPSETTING(0x08, "Spanish");
            PORT_DIPSETTING(0x0c, "German");
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x00, "Normal");
            PORT_DIPSETTING(0x20, DEF_STR("Free Play"));
            PORT_DIPNAME(0xd0, 0x80, "Fuel units");
            PORT_DIPSETTING(0x00, "450");
            PORT_DIPSETTING(0x40, "600");
            PORT_DIPSETTING(0x80, "750");
            PORT_DIPSETTING(0xc0, "900");
            PORT_DIPSETTING(0x10, "1100");
            PORT_DIPSETTING(0x50, "1300");
            PORT_DIPSETTING(0x90, "1550");
            PORT_DIPSETTING(0xd0, "1800");

            /* The next one is a potentiometer */
            PORT_START("IN3");
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_PADDLE | IPF_REVERSE, 100, 10, 0, 255, (ushort)Mame.InputCodes.KEYCODE_UP, (ushort)Mame.InputCodes.KEYCODE_DOWN, (ushort)Mame.InputCodes.JOYCODE_1_UP, (ushort)Mame.InputCodes.JOYCODE_1_DOWN);
            return INPUT_PORTS_END;
        }
        
//static Mame.CustomSoundInterface llander_custom_interface =
//new 
//    llander_sh_start,
//    llander_sh_stop,
//    llander_sh_update
//};
        static _BytePtr llander_zeropage = new _BytePtr(1);

static int llander_zeropage_r(int offset)
{
	return llander_zeropage[offset & 0xff];
}

static void llander_zeropage_w(int offset,int data)
{
	llander_zeropage[offset & 0xff] =(byte) data;
}
        static int llander_interrupt ()
{
	/* Turn off interrupts if self-test is enabled */
	if ((Mame.readinputport(0) & 0x02)!=0)
		return Mame.nmi_interrupt();
	else
		return Mame.ignore_interrupt();
}
        static int llander_IN0_r(int offset)
        {
            int res;

            res = Mame.readinputport(0);

            if (AvgDvg.avgdvg_done())
                res |= 0x01;
            if ((Mame.cpu_gettotalcycles() & 0x100)!=0)
                res |= 0x40;

            return res;
        }

static Mame.MemoryReadAddress[] llander_readmem=
{
	new  Mame.MemoryReadAddress( 0x0000, 0x01ff, llander_zeropage_r ),
	new  Mame.MemoryReadAddress( 0x2000, 0x2000, llander_IN0_r ), /* IN0 */
	new  Mame.MemoryReadAddress( 0x2400, 0x2407, driver_asteroid.asteroid_IN1_r ), /* IN1 */
	new  Mame.MemoryReadAddress( 0x2800, 0x2803, driver_asteroid.asteroid_DSW1_r ), /* DSW1 */
	new  Mame.MemoryReadAddress( 0x2c00, 0x2c00, Mame.input_port_3_r ), /* IN3 */
	new  Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new  Mame.MemoryReadAddress( 0x4800, 0x5fff, Mame.MRA_ROM ), /* vector rom */
	new  Mame.MemoryReadAddress( 0x6000, 0x7fff, Mame.MRA_ROM ),
	new  Mame.MemoryReadAddress( 0xf800, 0xffff, Mame.MRA_ROM ), /* for the reset / interrupt vectors */
	new  Mame.MemoryReadAddress( -1 )  /* end of table */
};

static Mame.MemoryWriteAddress[] llander_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x01ff, llander_zeropage_w, llander_zeropage ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, AvgDvg.avgdvg_go ),
	new Mame.MemoryWriteAddress( 0x3200, 0x3200, llander_led_w ),
	new Mame.MemoryWriteAddress( 0x3400, 0x3400, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3c00, llander_sounds_w ),
	new Mame.MemoryWriteAddress( 0x3e00, 0x3e00, llander_snd_reset_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM, Vector.vectorram, Vector.vectorram_size ),
	new Mame.MemoryWriteAddress( 0x4800, 0x5fff, Mame.MWA_ROM ), /* vector rom */
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

const byte MIN_SLICE = 10;
const int LANDER_OVERSAMPLE_RATE = 768000;
static int volume;
static int tone_6khz;
static int tone_3khz;
static int llander_explosion; 
        static int buffer_len;
static int emulation_rate;
static long multiplier;
static int sample_pos;
static int channel;
static int lfsr_index;
static _ShortPtr sample_buffer;
static _ShortPtr lfsr_buffer;

static void llander_snd_reset_w(int offset, int data)
{
    lfsr_index = 0;
}
static int[] sinetable ={
	128,140,153,165,177,188,199,209,218,226,234,240,245,250,253,254,
	255,254,253,250,245,240,234,226,218,209,199,188,177,165,153,140,
	128,116,103, 91, 79, 68, 57, 47, 38, 30, 22, 16, 11,  6,  3,  2,
	1  ,2  ,3  ,  6, 11, 16, 22, 30, 38, 47, 57, 68, 79, 91,103,116
};

static int[] llander_volume = { 0x00, 0x20, 0x40, 0x60, 0x80, 0xa0, 0xc0, 0xff };
static int sampnum = 0;
static int noisetarg = 0, noisecurrent = 0;
static int lastoversampnum = 0;
static void llander_process(_ShortPtr buffer, int start, int n)
{
	int loop,sample;
	int oversampnum,loop2;

	for(loop=0;loop<n;loop++)
	{
		oversampnum=(int)(sampnum*multiplier)>>16;

//		if (errorlog) fprintf (errorlog, "LANDER: sampnum=%x oversampnum=%lx\n",sampnum, oversampnum);

		/* Pick up new noise target value whenever 12khz changes */

		if(lastoversampnum>>6!=oversampnum>>6)
		{
			lfsr_index=lfsr_buffer.read16(lfsr_index);
			noisetarg=(lfsr_buffer.read16(lfsr_index)&0x4000)!=0?llander_volume[volume]:0x00;
			noisetarg<<=16;
		}

		/* Do tracking of noisetarg to noise current done in fixed point 16:16    */
		/* each step takes us 1/256 of the difference between desired and current */

		for(loop2=lastoversampnum;loop2<oversampnum;loop2++)
		{
			noisecurrent+=(noisetarg-noisecurrent)>>7;	/* Equiv of multiply by 1/256 */
		}

		sample=(int)(noisecurrent>>16);
		sample<<=1;	/* Gain = 2 */

		if(tone_3khz!=0)
		{
			sample+=sinetable[(oversampnum>>2)&0x3f];
		}
		if(tone_6khz!=0)
		{
			sample+=sinetable[(oversampnum>>1)&0x3f];
		}
		if(llander_explosion!=0)
		{
			sample+=(int)(noisecurrent>>(16-2));	/* Gain of 4 */
		}

		/* Scale ouput down to buffer */

        buffer.write16(start + loop,(ushort)( (sample << 5) - 0x8000));

		sampnum++;
		lastoversampnum=oversampnum;
	}
}


        static void llander_sh_update_partial()
{
	int newpos;

	if (Mame.Machine.sample_rate == 0) return;

    newpos = Mame.sound_scalebufferpos(buffer_len); /* get current position based on the timer */

	if(newpos-sample_pos<MIN_SLICE) return;

	/* Process count samples into the buffer */

	llander_process (sample_buffer, sample_pos, newpos - sample_pos);

	/* Update sample position */

	sample_pos = newpos;
}

static void llander_sounds_w(int offset, int data)
{
    /* Update sound to present */
    llander_sh_update_partial();

    /* Lunar Lander sound breakdown */

    volume = data & 0x07;
    tone_3khz = data & 0x10;
    tone_6khz = data & 0x20;
    llander_explosion = data & 0x08;
}
const byte NUM_LIGHTS = 5;
        static Mame.artwork llander_panel,llander_lit_panel;
        static Mame.rectangle[] light_areas = {
    new Mame.rectangle(  0, 205, 0, 127 ),
	new Mame.rectangle(206, 343, 0, 127 ),
	new Mame.rectangle(344, 481, 0, 127 ),
	new Mame.rectangle(482, 616, 0, 127 ),
	new Mame.rectangle(617, 799, 0, 127 )};

        static bool[] lights = new bool[NUM_LIGHTS];
        static bool[] lights_changed = new bool[NUM_LIGHTS];
static void llander_led_w(int offset, int data)
{
    for (int i = 0; i < 5; i++)
    {
        bool new_light = (data & (1 << (4 - i))) != 0;
        if (lights[i] != new_light)
        {
            lights[i] = new_light;
            lights_changed[i] = true;
        }
    }



}


        class machine_driver_llander : Mame.MachineDriver
        {
            public machine_driver_llander()
        {
            cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502, 1500000, llander_readmem, llander_writemem, null, null, llander_interrupt, 6));
            frames_per_second = 40;
            vblank_duration = 0;
            cpu_slices_per_frame = 1;
            screen_width = 400;
            screen_height = 300;
                visible_area = new Mame.rectangle(0,1050,0,900);
                total_colors=256;
                color_table_len=256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;
                //sound.Add(new Mame.MachineSound(Mame.SOUND_CUSTOM,llander_custom_interface));
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
                int width, height, i;

                AvgDvg.avg_init_palette_white(palette, colortable, color_prom);

                llander_lit_panel = null;
                width = Mame.Machine.scrbitmap.width;
                height = (int)(0.16 * width);

                if ((llander_panel = Mame.artwork_load_size("llander.png", 24, 230, width, height)) != null)
                {
                    if ((llander_lit_panel = Mame.artwork_load_size("llander1.png", 24 + llander_panel.num_pens_used, 230 - llander_panel.num_pens_used, width, height)) == null)
                    {
                        Mame.artwork_free(ref llander_panel);
                        llander_panel = null;
                        return;
                    }
                }
                else
                    return;

                for (i = 0; i < 16; i++)
                    palette[3 * (i + 8)] = palette[3 * (i + 8) + 1] = palette[3 * (i + 8) + 2] = (byte)((255 * i) / 15);

                for (i = 0; i < 3 * llander_panel.num_pens_used; i++)                
                    palette[3 * llander_panel.start_pen + i] = llander_panel.orig_palette[i];
                //memcpy(palette + 3 * llander_panel.start_pen, llander_panel.orig_palette,3 * llander_panel.num_pens_used);
                for (i = 0; i < 3 * llander_lit_panel.num_pens_used; i++)                
                    palette[3 * llander_lit_panel.start_pen + i] = llander_lit_panel.orig_palette[i];
                
                //memcpy(palette + 3 * llander_lit_panel.start_pen, llander_lit_panel.orig_palette,3 * llander_lit_panel.num_pens_used);
            }
            public override int vh_start()
            {
                if (AvgDvg.dvg_start()!=0)
                    return 1;

                if (llander_panel == null)
                    return 0;

                for (int i = 0; i < NUM_LIGHTS; i++)
                {
                    lights[i] = false;
                    lights_changed[i] = true;
                }
                if (llander_panel!=null) Mame.backdrop_refresh(llander_panel);
                if (llander_lit_panel!=null) Mame.backdrop_refresh(llander_lit_panel);
                return 0;
            }
            public override void vh_stop()
            {
                AvgDvg.dvg_stop();

                if (llander_panel != null)
                    Mame.artwork_free(ref llander_panel);
                llander_panel = null;

                if (llander_lit_panel != null)
                    Mame.artwork_free(ref llander_lit_panel);
                llander_lit_panel = null;
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
               int i, pwidth, pheight;
	float scale;
	Mame.osd_bitmap vector_bitmap=new Mame.osd_bitmap();
	Mame.rectangle rect=new Mame.rectangle();

	if (llander_panel == null)
	{
		AvgDvg.dvg_screenrefresh(bitmap,full_refresh);
		return;
	}

	pwidth = llander_panel._artwork.width;
	pheight = llander_panel._artwork.height;

	vector_bitmap.width = bitmap.width;
	vector_bitmap.height = bitmap.height - pheight;
	vector_bitmap._private = bitmap._private;
	vector_bitmap.line = bitmap.line;

	AvgDvg.dvg_screenrefresh(vector_bitmap,full_refresh);

	if (full_refresh!=0)
	{
		rect.min_x = 0;
		rect.max_x = pwidth-1;
		rect.min_y = bitmap.height - pheight;
		rect.max_y = bitmap.height - 1;

		Mame.copybitmap(bitmap,llander_panel._artwork,false,false,0, bitmap.height - pheight, rect, Mame.TRANSPARENCY_NONE, 0);
		Mame.osd_mark_dirty (rect.min_x,rect.min_y,rect.max_x,rect.max_y,0);
	}

	scale = pwidth/800.0f;

	for (i=0;i<NUM_LIGHTS;i++)
	{
		if (lights_changed[i] || full_refresh!=0)
		{
			rect.min_x = (int)(scale * light_areas[i].min_x);
			rect.max_x = (int)(scale * light_areas[i].max_x);
			rect.min_y = (int)(bitmap.height - pheight + scale * light_areas[i].min_y);
			rect.max_y = (int)(bitmap.height - pheight + scale * light_areas[i].max_y);

			if (lights[i])
                Mame.copybitmap(bitmap, llander_lit_panel._artwork, false,false,0, bitmap.height - pheight, rect, Mame.TRANSPARENCY_NONE, 0);
			else
                Mame.copybitmap(bitmap, llander_panel._artwork, false,false,0, bitmap.height - pheight, rect, Mame.TRANSPARENCY_NONE, 0);

            Mame.osd_mark_dirty(rect.min_x, rect.min_y, rect.max_x, rect.max_y, 0);

			lights_changed[i] = false;
		}
	}
            }
            public override void vh_eof_callback()
            {
               //none
            }
        }
        public driver_llander()
        {
            drv = new machine_driver_llander();
            year = "1979";
            name = "llander";
            description = "Lunar Lander (rev 2)";
            manufacturer = "Atari";
            flags = Mame.ROT0;
            input_ports = input_ports_llander();
            rom = rom_llander();
            drv.HasNVRAMhandler = false;
        }
    }
}
