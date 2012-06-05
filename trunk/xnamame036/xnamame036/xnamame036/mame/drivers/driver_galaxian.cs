//#define NEW_LFO
#define NEW_SHOOT
#define SAMPLES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_galaxian : Mame.GameDriver
    {
        static Mame.rectangle _spritevisiblearea = new Mame.rectangle(2 * 8 + 1, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
        static Mame.rectangle _spritevisibleareaflipx = new Mame.rectangle(0 * 8, 30 * 8 - 2, 2 * 8, 30 * 8 - 1);

        static Mame.rectangle spritevisiblearea;
        static Mame.rectangle spritevisibleareaflipx;

        delegate void _modify_charcode(ref int a, int b);
        delegate void _modify_spritecode(ref int a, ref int b, ref int c, int d);

        static _modify_charcode modify_charcode;
        static _modify_spritecode modify_spritecode;

        const int MAX_STARS = 250;
        const int STARS_COLOR_BASE = 32;

        static _BytePtr galaxian_attributesram = new _BytePtr(1);
        static _BytePtr galaxian_bulletsram = new _BytePtr(1);

        static int[] galaxian_bulletsram_size = new int[1];
        static int stars_on, stars_blink;
        static int stars_type;		/* -1 = no stars */
        /*  0 = Galaxian stars */
        /*  1 = Scramble stars */
        /*  2 = Rescue stars (same as Scramble, but only half screen) */
        /*  3 = Mariner stars (same as Galaxian, but some parts are blanked */
        static uint stars_scroll;
        static int color_mask;
        static int mooncrst_gfxextend;
        static int pisces_gfxbank;
        static int[] jumpbug_gfxbank = new int[5];
        static int[] flipscreen = new int[2];

        static int background_on;
        static byte[] backcolor = new byte[256];
        struct star
        {
            public int x, y, code;
        };
        static star[] stars = new star[MAX_STARS];
        static int total_stars;
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

        static Mame.MemoryWriteAddress[] galaxian_writemem =
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

        static void galaxian_attributes_w(int offset, int data)
        {
            if ((offset & 1) != 0 && galaxian_attributesram[offset] != data)
            {
                int i;


                for (i = offset / 2; i < Generic.videoram_size[0]; i += 32)
                    Generic.dirtybuffer[i] = true;
            }

            galaxian_attributesram[offset] = (byte)data;
        }

        static int galaxian_vh_interrupt()
        {
            stars_scroll++;

            return Mame.nmi_interrupt();
        }
        static void galaxian_coin_lockout_w(int offset, int data)
        {
            Mame.coin_lockout_global_w(offset, data ^ 1);
        }
        static void machine_init_galaxian()
        {
            Mame.install_mem_write_handler(0, 0x6002, 0x6002, galaxian_coin_lockout_w);
        }

        static Mame.GfxLayout bulletlayout = new Mame.GfxLayout
        (
            /* there is no gfx ROM for this one, it is generated by the hardware */
            3, 1,
            1,	/* just one */
            1,	/* 1 bit per pixel */
            new uint[] { 0 },
            new uint[] { 2, 2, 2 },	/* I "know" that this bit of the */
            new uint[] { 0 },						/* graphics ROMs is 1 */
            0	/* no use */
        );
        static Mame.GfxLayout galaxian_charlayout = new Mame.GfxLayout
(
    8, 8,	/* 8*8 characters */
    Mame.RGN_FRAC(1, 2),
    2,	/* 2 bits per pixel */
    new uint[] { Mame.RGN_FRAC(0, 2), Mame.RGN_FRAC(1, 2) },	/* the two bitplanes are separated */
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every char takes 8 consecutive bytes */
);
        static Mame.GfxLayout galaxian_spritelayout = new Mame.GfxLayout
        (
            16, 16,	/* 16*16 sprites */
            Mame.RGN_FRAC(1, 2),	/* 64 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { Mame.RGN_FRAC(0, 2), Mame.RGN_FRAC(1, 2) },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 8 * 8 + 4, 8 * 8 + 5, 8 * 8 + 6, 8 * 8 + 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 16 * 8, 17 * 8, 18 * 8, 19 * 8, 20 * 8, 21 * 8, 22 * 8, 23 * 8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );
        static Mame.GfxLayout backgroundlayout = new Mame.GfxLayout
        (
            /* there is no gfx ROM for this one, it is generated by the hardware */
            8, 8,
            32,	/* one for each column */
            7,	/* 128 colors max */
            new uint[] { 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8 * 8, 1 * 8 * 8, 2 * 8 * 8, 3 * 8 * 8, 4 * 8 * 8, 5 * 8 * 8, 6 * 8 * 8, 7 * 8 * 8 },
            new uint[] { 0, 8, 16, 24, 32, 40, 48, 56 },
            8 * 8 * 8	/* each character takes 64 bytes */
        );
        static Mame.GfxDecodeInfo[] galaxian_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, galaxian_charlayout,    0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, galaxian_spritelayout,  0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, bulletlayout,         8*4, 2 ),
	new Mame.GfxDecodeInfo( 0,           0x0000, backgroundlayout, 8*4+2*2, 1 ),	/* this will be dynamically created */
};

        const int NEW_LFO = 0;
        const int NEW_SHOOT = 1;

        const int XTAL = 18432000;
        const double SOUND_CLOCK = XTAL / 6 / 2;

        const int SAMPLES = 1;

        const double RNG_RATE = XTAL / 3;
        const double NOISE_RATE = XTAL / 3 / 192 / 2 / 2;
        const double NOISE_LENGTH = NOISE_RATE * 4;

        const int SHOOT_RATE = 2672;
        const int SHOOT_LENGTH = 13000;

        const int TOOTHSAW_LENGTH = 16;
        const int TOOTHSAW_VOLUME = 36;
        const int STEPS = 16;
        const int LFO_VOLUME = 6;
        const int SHOOT_VOLUME = 50;
        const int NOISE_VOLUME = 50;
        const int NOISE_AMPLITUDE = 70 * 256;
        const int TOOTHSAW_AMPLITUDE = 64;

        const int MINFREQ = 139 - 139 / 3;
        const int MAXFREQ = 139 + 139 / 3;
        static int freq = MAXFREQ;

        static Mame.Timer.timer_entry lfotimer = null;
        static int noisevolume;
        static object noisetimer = null;
        static _ShortPtr noisewave;
        static _ShortPtr shootwave;
#if NEW_SHOOT
        static int shoot_length;
        static int shoot_rate;
#endif

#if SAMPLES
        static int shootsampleloaded = 0;
        static int deathsampleloaded = 0;
        static int last_port1 = 0;
#endif
        static int last_port2 = 0;
        static int pitch, vol;
        static _BytePtr[] tonewave = new _BytePtr[4];

        static _ShortPtr backgroundwave = new _ShortPtr(32 * 2);
        static driver_galaxian()
        {
            short[] t = {
   0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000,
   0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000,
   0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,
  -0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,
};
            for (int i = 0; i < t.Length; i++)
                backgroundwave.write16(i, (ushort)t[i]);
        }
        static int channelnoise, channelshoot, channellfo;
        static int tone_stream;

        static void galaxian_background_enable_w(int offset, int data)
        {
            Mame.mixer_set_volume(channellfo + offset, (data & 1) != 0 ? 100 : 0);
        }
        static int[] lfobit = new int[4];
        static void galaxian_lfo_freq_w(int offset, int data)
        {
#if NEW_LFO
	static int lfobit[4];

	/* R18 1M,R17 470K,R16 220K,R15 100K */
	const int rv[4] = { 1000000,470000,220000,100000};
	double r1,r2,Re,td;
	int i;

	if( (data & 1) == lfobit[offset] )
		return;

	/*
	 * NE555 9R is setup as astable multivibrator
	 * - this circuit looks LINEAR RAMP V-F converter
	   I  = 1/Re * ( R1/(R1+R2)-Vbe)
	   td = (2/3VCC*Re*(R1+R2)*C) / (R1*VCC-Vbe*(R1+R2))
	  parts assign
	   R1  : (R15* L1)|(R16* L2)|(R17* L3)|(R18* L1)
	   R2  : (R15*~L1)|(R16*~L2)|(R17*~L3)|(R18*~L4)|R??(330K)
	   Re  : R21(100K)
	   Vbe : Q2(2SA1015)-Vbe
	 * - R20(15K) and Q1 is unknown,maybe current booster.
	*/

	lfobit[offset] = data & 1;

	/* R20 15K */
	r1 = 1e12;
	/* R19? 330k to gnd */
	r2 = 330000;
	//r1 = 15000;
	/* R21 100K */
	Re = 100000;
	/* register calculation */
	for(i=0;i<4;i++)
	{
		if(lfobit[i])
			r1 = (r1*rv[i])/(r1+rv[i]); /* Hi  */
		else
			r2 = (r2*rv[i])/(r2+rv[i]); /* Low */
	}

	if( lfotimer )
	{
		timer_remove( lfotimer );
		lfotimer = 0;
	}

double Vcc 5.0;
double Vbe =0.65;		/* 2SA1015 */
double Cap =0.000001;	/* C15 1uF */
	td = (Vcc*2/3*Re*(r1+r2)*Cap) / (r1*Vcc - Vbe*(r1+r2) );

	if( errorlog ) fprintf(errorlog, "lfo timer bits:%d%d%d%d r1:%d, r2:%d, re: %d, td: %9.2fsec\n", lfobit[0], lfobit[1], lfobit[2], lfobit[3], (int)r1, (int)r2, (int)Re, td);
	lfotimer = timer_pulse( TIME_IN_SEC(td / (MAXFREQ-MINFREQ)), 0, lfo_timer_cb);
#else

            double r0, r1, rx = 100000.0;

            if ((data & 1) == lfobit[offset])
                return;

            /*
             * NE555 9R is setup as astable multivibrator
             * - Ra is between 100k and ??? (open?)
             * - Rb is zero here (bridge between pins 6 and 7)
             * - C is 1uF
             * charge time t1 = 0.693 * (Ra + Rb) * C
             * discharge time t2 = 0.693 * (Rb) *  C
             * period T = t1 + t2 = 0.693 * (Ra + 2 * Rb) * C
             * . min period: 0.693 * 100 kOhm * 1uF . 69300 us = 14.4Hz
             * . max period: no idea, since I don't know the max. value for Ra :(
             */

            lfobit[offset] = data & 1;

            /* R?? 330k to gnd */
            r0 = 1.0 / 330000;
            /* open is a very high value really ;-) */
            r1 = 1.0 / 1e12;

            /* R18 1M */
            if (lfobit[0] != 0)
                r1 += 1.0 / 1000000;
            else
                r0 += 1.0 / 1000000;

            /* R17 470k */
            if (lfobit[1] != 0)
                r1 += 1.0 / 470000;
            else
                r0 += 1.0 / 470000;

            /* R16 220k */
            if (lfobit[2] != 0)
                r1 += 1.0 / 220000;
            else
                r0 += 1.0 / 220000;

            /* R15 100k */
            if (lfobit[3] != 0)
                r1 += 1.0 / 100000;
            else
                r0 += 1.0 / 100000;

            if (lfotimer != null)
            {
                Mame.Timer.timer_remove(lfotimer);
                lfotimer = null;
            }

            r0 = 1.0 / r0;
            r1 = 1.0 / r1;

            /* I used an arbitrary value for max. Ra of 2M */
            rx = rx + 2000000.0 * r0 / (r0 + r1);

            //LOG((errorlog, "lfotimer bits:%d%d%d%d r0:%d, r1:%d, rx: %d, time: %9.2fus\n", lfobit[3], lfobit[2], lfobit[1], lfobit[0], (int)r0, (int)r1, (int)rx, 0.639 * rx));
            lfotimer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_USEC(0.639 * rx / (MAXFREQ - MINFREQ)), 0, lfo_timer_cb);
#endif
        }
        static void lfo_timer_cb(int param)
        {
            if (freq > MINFREQ)
                freq--;
            else
                freq = MAXFREQ;
        }
        static void galaxian_noise_enable_w(int offset, int data)
        {
#if SAMPLES
            if (deathsampleloaded != 0)
            {
                if ((data & 1) != 0 && (last_port1 & 1) == 0)
                    Mame.mixer_play_sample(channelnoise, Mame.Machine.samples.sample[1].data,
                            Mame.Machine.samples.sample[1].length,
                            Mame.Machine.samples.sample[1].smpfreq,
                            false);
                last_port1 = data;
            }
            else
#endif
            {
                if ((data & 1) != 0)
                {
                    if (noisetimer != null)
                    {
                        Mame.Timer.timer_remove(noisetimer);
                        noisetimer = null;
                    }
                    noisevolume = 100;
                    Mame.mixer_set_volume(channelnoise, noisevolume);
                }
                else
                {
                    /* discharge C21, 22uF via 150k+22k R35/R36 */
                    if (noisevolume == 100)
                    {
                        noisetimer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_USEC(0.693 * (155000 + 22000) * 22 / 100), 0, noise_timer_cb);
                    }
                }
            }
        }
        static void noise_timer_cb(int param)
        {
            if (noisevolume > 0)
            {
                noisevolume -= (noisevolume / 10) + 1;
                Mame.mixer_set_volume(channelnoise, noisevolume);
            }
        }
        static void galaxian_shoot_enable_w(int offset, int data)
        {
            if ((data & 1) != 0 && (last_port2 & 1) == 0)
            {
#if SAMPLES
                if (shootsampleloaded != 0)
                {
                    Mame.mixer_play_sample(channelshoot, Mame.Machine.samples.sample[0].data,
                            Mame.Machine.samples.sample[0].length,
                            Mame.Machine.samples.sample[0].smpfreq,
                            false);
                }
                else
#endif
                {
#if NEW_SHOOT
                    Mame.mixer_play_sample_16(channelshoot, new _ShortPtr(shootwave), shoot_length, shoot_rate, false);
#else
			mixer_play_sample_16(channelshoot, shootwave, SHOOT_LENGTH, 10*SHOOT_RATE, 0);
#endif
                    Mame.mixer_set_volume(channelshoot, SHOOT_VOLUME);
                }
            }
            last_port2 = data;
        }
        static void galaxian_vol_w(int offset, int data)
        {
            Mame.stream_update(tone_stream, 0);

            /* offset 0 = bit 0, offset 1 = bit 1 */
            vol = (vol & ~(1 << offset)) | ((data & 1) << offset);
        }

        static void galaxian_pitch_w(int offset, int data)
        {
            Mame.stream_update(tone_stream, 0);

            pitch = data;
        }

        class galaxian_soundinterface : Mame.CustomSoundInterface
        {
            static string[] galaxian_sample_names = 
            {
            "*galaxian",
	"shot.wav",
	"death.wav",
	null	/* end of array */
            };
            static int AMP(int n) { return (n) * 0x8000 / 100 - 0x8000; }
            static int[] charge_discharge = {
			AMP( 0), AMP(25), AMP(45), AMP(60), AMP(70), AMP(85),
			AMP(70), AMP(50), AMP(25), AMP( 0)
		};
            static int counter, countdown;
            static void tone_update(int ch, _ShortPtr buffer, int length)
            {
                int i, j;
                _BytePtr w = new _BytePtr(tonewave[vol]);

                /* only update if we have non-zero volume and frequency */
                if (pitch != 0xff)
                {
                    for (i = 0; i < length; i++)
                    {
                        int mix = 0;

                        for (j = 0; j < STEPS; j++)
                        {
                            if (countdown >= 256)
                            {
                                counter = (counter + 1) % TOOTHSAW_LENGTH;
                                countdown = pitch;
                            }
                            countdown++;

                            mix += (sbyte)w[counter];
                        }
                        buffer.write16(0, (ushort)((mix << 8) / STEPS));
                        buffer.offset += 2;
                    }
                }
                else
                {
                    for (i = 0; i < length; i++)
                    {
                        buffer.write16(0, 0);
                        buffer.offset += 2;
                    }
                }
            }

            static int V(double r0, double r1)
            {
                return (int)(2 * TOOTHSAW_AMPLITUDE * (r0) / (r0 + r1) - TOOTHSAW_AMPLITUDE);
            }
            public override int start(Mame.MachineSound msound)
            {
                int i, j, sweep, charge, countdown, generator, bit1, bit2;
                int[] lfovol = { LFO_VOLUME, LFO_VOLUME, LFO_VOLUME };

#if SAMPLES
                Mame.Machine.samples = Mame.readsamples(galaxian_sample_names, Mame.Machine.gamedrv.name);
#endif

                channelnoise = Mame.mixer_allocate_channel(NOISE_VOLUME);
                Mame.mixer_set_name(channelnoise, "Noise");
                channelshoot = Mame.mixer_allocate_channel(SHOOT_VOLUME);
                Mame.mixer_set_name(channelshoot, "Shoot");
                channellfo = Mame.mixer_allocate_channels(3, lfovol);
                Mame.mixer_set_name(channellfo + 0, "Background #0");
                Mame.mixer_set_name(channellfo + 1, "Background #1");
                Mame.mixer_set_name(channellfo + 2, "Background #2");

#if SAMPLES
                if (Mame.Machine.samples != null && Mame.Machine.samples.sample[0] != null)	/* We should check also that Samplename[0] = 0 */
                    shootsampleloaded = 1;
                else
                    shootsampleloaded = 0;

                if (Mame.Machine.samples != null && Mame.Machine.samples.sample[1] != null)	/* We should check also that Samplename[0] = 0 */
                    deathsampleloaded = 1;
                else
                    deathsampleloaded = 0;
#endif

                if ((noisewave = new _ShortPtr((int)(NOISE_LENGTH * sizeof(short)))) == null)
                {
                    return 1;
                }

#if NEW_SHOOT
                byte SHOOT_SEC = 2;
                shoot_rate = Mame.Machine.sample_rate;
                shoot_length = SHOOT_SEC * shoot_rate;
                if ((shootwave = new _ShortPtr(shoot_length * sizeof(short))) == null)
#else
	if( (shootwave = new _ShortPtr(SHOOT_LENGTH * sizeof(short))) == null )
#endif
                {
                    noisewave = null;
                    return 1;
                }

                /*
                 * The RNG shifter is clocked with RNG_RATE, bit 17 is
                 * latched every 2V cycles (every 2nd scanline).
                 * This signal is used as a noise source.
                 */
                generator = 0;
                countdown = (int)(NOISE_RATE / 2);
                for (i = 0; i < NOISE_LENGTH; i++)
                {
                    countdown -= (int)RNG_RATE;
                    while (countdown < 0)
                    {
                        generator <<= 1;
                        bit1 = (~generator >> 17) & 1;
                        bit2 = (generator >> 5) & 1;
                        if ((bit1 ^ bit2) != 0) generator |= 1;
                        countdown += (int)NOISE_RATE;
                    }
                    noisewave.write16(i, (ushort)(((generator >> 17) & 1) != 0 ? (ushort)NOISE_AMPLITUDE : -(ushort)NOISE_AMPLITUDE));
                }

#if NEW_SHOOT

                /* dummy */
                sweep = 100;
                charge = +2;
                j = 0;
                {
                    int R41__ = 100000;
                    int R44__ = 10000;
                    int R45__ = 22000;
                    int R46__ = 10000;
                    int R47__ = 2200;
                    int R48__ = 2200;
                    double C25__ = 0.000001;
                    double C27__ = 0.00000001;
                    double C28__ = 0.000047;
                    double C29__ = 0.00000001;
                    double IC8L3_L = 0.2;  /* 7400 L level */
                    double IC8L3_H = 4.5; /* 7400 H level */
                    double NOISE_L = 0.2;  /* 7474 L level */
                    double NOISE_H = 4.5;   /* 7474 H level */
                    /*
                        key on/off time is programmable
                        Therefore,  it is necessary to make separate sample with key on/off.
                        And,  calculate the playback point according to the voltage of c28.
                    */
                    double SHOOT_KEYON_TIME = 0.1;  /* second */
                    /*
                        NE555-FM input calculation is wrong.
                        The frequency is not proportional to the voltage of FM input.
                        And,  duty will be changed,too.
                    */
                    double NE555_FM_ADJUST_RATE = 0.80;
                    /* discharge : 100K * 1uF */
                    double v = 5.0;
                    double vK = (shoot_rate) != 0 ? Math.Exp(-1 / (R41__ * C25__) / shoot_rate) : 0;
                    /* -- SHOOT KEY port -- */
                    double IC8L3 = IC8L3_L; /* key on */
                    int IC8Lcnt = (int)(SHOOT_KEYON_TIME * shoot_rate); /* count for key off */
                    /* C28 : KEY port capacity */
                    /*       connection : 8L-3 - R47(2.2K) - C28(47u) - R48(2.2K) - C29 */
                    double c28v = IC8L3_H - (IC8L3_H - (NOISE_H + NOISE_L) / 2) / (R46__ + R47__ + R48__) * R47__;
                    double c28K = (shoot_rate) != 0 ? Math.Exp(-1 / (22000 * 0.000047) / shoot_rate) : 0;
                    /* C29 : NOISE capacity */
                    /*       connection : NOISE - R46(10K) - C29(0.1u) - R48(2.2K) - C28 */
                    double c29v = IC8L3_H - (IC8L3_H - (NOISE_H + NOISE_L) / 2) / (R46__ + R47__ + R48__) * (R47__ + R48__);
                    double c29K1 = (shoot_rate) != 0 ? Math.Exp(-1 / (22000 * 0.00000001) / shoot_rate) : 0; /* form C28   */
                    double c29K2 = (shoot_rate) != 0 ? Math.Exp(-1 / (100000 * 0.00000001) / shoot_rate) : 0; /* from noise */
                    /* NE555 timer */
                    /* RA = 10K , RB = 22K , C=.01u ,FM = C29 */
                    double ne555cnt = 0;
                    double ne555step = (shoot_rate) != 0 ? ((1.44 / ((R44__ + R45__ * 2) * C27__)) / shoot_rate) : 0;
                    double ne555duty = (double)(R44__ + R45__) / (R44__ + R45__ * 2); /* t1 duty */
                    double ne555sr;		/* threshold (FM) rate */
                    /* NOISE source */
                    double ncnt = 0.0;
                    double nstep = (shoot_rate) != 0 ? ((double)NOISE_RATE / shoot_rate) : 0;
                    double noise_sh2; /* voltage level */

                    for (i = 0; i < shoot_length; i++)
                    {
                        /* noise port */
                        noise_sh2 = noisewave.read16((int)(ncnt % NOISE_LENGTH)) == NOISE_AMPLITUDE ? NOISE_H : NOISE_L;
                        ncnt += nstep;
                        /* calculate NE555 threshold level by FM input */
                        ne555sr = c29v * NE555_FM_ADJUST_RATE / (5.0 * 2 / 3);
                        /* calc output */
                        ne555cnt += ne555step;
                        if (ne555cnt >= ne555sr) ne555cnt -= ne555sr;
                        if (ne555cnt < ne555sr * ne555duty)
                        {
                            /* t1 time */
                            shootwave.write16(i, (ushort)(v / 5 * 0x7fff));
                            /* discharge output level */
                            if (IC8L3 == IC8L3_H)
                                v *= vK;
                        }
                        else
                            shootwave.write16(i, 0);
                        /* C28 charge/discharge */
                        c28v += (IC8L3 - c28v) - (IC8L3 - c28v) * c28K;	/* from R47 */
                        c28v += (c29v - c28v) - (c29v - c28v) * c28K;		/* from R48 */
                        /* C29 charge/discharge */
                        c29v += (c28v - c29v) - (c28v - c29v) * c29K1;	/* from R48 */
                        c29v += (noise_sh2 - c29v) - (noise_sh2 - c29v) * c29K2;	/* from R46 */
                        /* key off */
                        if (IC8L3 == IC8L3_L && --IC8Lcnt == 0)
                            IC8L3 = IC8L3_H;
                    }
                }
#else
	/*
	 * Ra is 10k, Rb is 22k, C is 0.01uF
	 * charge time t1 = 0.693 * (Ra + Rb) * C . 221.76us
	 * discharge time t2 = 0.693 * (Rb) *  C . 152.46us
	 * average period 374.22us . 2672Hz
	 * I use an array of 10 values to define some points
	 * of the charge/discharge curve. The wave is modulated
	 * using the charge/discharge timing of C28, a 47uF capacitor,
	 * over a 2k2 resistor. This will change the frequency from
	 * approx. Favg-Favg/3 up to Favg+Favg/3 down to Favg-Favg/3 again.
	 */
	sweep = 100;
	charge = +2;
	countdown = sweep / 2;
	for( i = 0, j = 0; i < SHOOT_LENGTH; i++ )
	{
		
		shootwave[i] = charge_discharge[j];
		LOG((errorlog, "shoot[%5d] $%04x (sweep: %3d, j:%d)\n", i, shootwave[i] & 0xffff, sweep, j));
		/*
		 * The current sweep and a 2200/10000 fraction (R45 and R48)
		 * of the noise are frequency modulating the NE555 chip.
		 */
		countdown -= sweep + noisewave[i % NOISE_LENGTH] / (2200*NOISE_AMPLITUDE/10000);
		while( countdown < 0 )
		{
			countdown += 100;
			j = ++j % 10;
		}
		/* sweep from 100 to 133 and down to 66 over the time of SHOOT_LENGTH */
		if( i % (SHOOT_LENGTH / 33 / 3 ) == 0 )
		{
			sweep += charge;
			if( sweep >= 133 )
				charge = -1;
		}
	}
#endif

                //memset(tonewave, 0, sizeof(tonewave));

                for (i = 0; i < TOOTHSAW_LENGTH; i++)
                {
                    //#define V(r0,r1) 2*TOOTHSAW_AMPLITUDE*(r0)/(r0+r1)-TOOTHSAW_AMPLITUDE
                    double r0a = 1.0 / 1e12, r1a = 1.0 / 1e12;
                    double r0b = 1.0 / 1e12, r1b = 1.0 / 1e12;

                    /* #0: VOL1=0 and VOL2=0
                     * only the 33k and the 22k resistors R51 and R50
                     */
                    if ((i & 1) != 0)
                    {
                        r1a += 1.0 / 33000;
                        r1b += 1.0 / 33000;
                    }
                    else
                    {
                        r0a += 1.0 / 33000;
                        r0b += 1.0 / 33000;
                    }
                    if ((i & 4) != 0)
                    {
                        r1a += 1.0 / 22000;
                        r1b += 1.0 / 22000;
                    }
                    else
                    {
                        r0a += 1.0 / 22000;
                        r0b += 1.0 / 22000;
                    }
                    for (int k = 0; k < 4; k++) tonewave[k] = new _BytePtr(TOOTHSAW_LENGTH);
                    tonewave[0][i] = (byte)V(1.0 / r0a, 1.0 / r1a);
                    //#define V(r0,r1) 2*TOOTHSAW_AMPLITUDE*(r0)/(r0+r1)-TOOTHSAW_AMPLITUDE

                    /* #1: VOL1=1 and VOL2=0
                     * add the 10k resistor R49 for bit QC
                     */
                    if ((i & 4) != 0)
                        r1a += 1.0 / 10000;
                    else
                        r0a += 1.0 / 10000;
                    tonewave[1][i] = (byte)V(1.0 / r0a, 1.0 / r1a);

                    /* #2: VOL1=0 and VOL2=1
                     * add the 15k resistor R52 for bit QD
                     */
                    if ((i & 8) != 0)
                        r1b += 1.0 / 15000;
                    else
                        r0b += 1.0 / 15000;
                    tonewave[2][i] = (byte)V(1.0 / r0b, 1.0 / r1b);

                    /* #3: VOL1=1 and VOL2=1
                     * add the 10k resistor R49 for QC
                     */
                    if ((i & 4) != 0)
                        r0b += 1.0 / 10000;
                    else
                        r1b += 1.0 / 10000;
                    tonewave[3][i] = (byte)V(1.0 / r0b, 1.0 / r1b);
                    //LOG((errorlog, "tone[%2d]: $%02x $%02x $%02x $%02x\n", i, tonewave[0][i], tonewave[1][i], tonewave[2][i], tonewave[3][i]));
                }

                pitch = 0;
                vol = 0;

                tone_stream = Mame.stream_init("Tone", TOOTHSAW_VOLUME, (int)(SOUND_CLOCK / STEPS), 0, tone_update);

#if SAMPLES
                if (deathsampleloaded == 0)
#endif
                {
                    Mame.mixer_set_volume(channelnoise, 0);
                    Mame.mixer_play_sample_16(channelnoise, noisewave, (int)NOISE_LENGTH, (int)NOISE_RATE, true);
                }
#if SAMPLES
                if (shootsampleloaded == 0)
#endif
                {
                    Mame.mixer_set_volume(channelshoot, 0);
                    Mame.mixer_play_sample_16(channelshoot, shootwave, SHOOT_LENGTH, SHOOT_RATE, false);
                }

                Mame.mixer_set_volume(channellfo + 0, 0);
                Mame.mixer_play_sample_16(channellfo + 0, backgroundwave, backgroundwave.buffer.Length / 2, 1000, true);
                Mame.mixer_set_volume(channellfo + 1, 0);
                Mame.mixer_play_sample_16(channellfo + 1, backgroundwave, backgroundwave.buffer.Length / 2, 1000, true);
                Mame.mixer_set_volume(channellfo + 2, 0);
                Mame.mixer_play_sample_16(channellfo + 2, backgroundwave, backgroundwave.buffer.Length / 2, 1000, true);

                return 0;
            }
            public override void stop()
            {
                if (lfotimer != null)
                {
                    Mame.Timer.timer_remove(lfotimer);
                    lfotimer = null;
                }
                if (noisetimer != null)
                {
                    Mame.Timer.timer_remove(noisetimer);
                    noisetimer = null;
                }
                Mame.mixer_stop_sample(channelnoise);
                Mame.mixer_stop_sample(channelshoot);
                Mame.mixer_stop_sample(channellfo + 0);
                Mame.mixer_stop_sample(channellfo + 1);
                Mame.mixer_stop_sample(channellfo + 2);
                noisewave = null;
                shootwave = null;
            }
            public override void update()
            {
                /*
      * NE555 8R, 8S and 8T are used as pulse position modulators
      * FS1 Ra=100k, Rb=470k and C=0.01uF
      *	. 0.693 * 1040k * 0.01uF . 7207.2us = 139Hz
      * FS2 Ra=100k, Rb=330k and C=0.01uF
      *	. 0.693 * 760k * 0.01uF . 5266.8us = 190Hz
      * FS2 Ra=100k, Rb=220k and C=0.01uF
      *	. 0.693 * 540k * 0.01uF . 3742.2us = 267Hz
      */

                Mame.mixer_set_sample_frequency(channellfo + 0, (backgroundwave.buffer.Length / 2) * freq * (100 + 2 * 470) / (100 + 2 * 470));
                Mame.mixer_set_sample_frequency(channellfo + 1, (backgroundwave.buffer.Length / 2) * freq * (100 + 2 * 300) / (100 + 2 * 470));
                Mame.mixer_set_sample_frequency(channellfo + 2, (backgroundwave.buffer.Length / 2) * freq * (100 + 2 * 220) / (100 + 2 * 470));

            }
        }
        static galaxian_soundinterface custom_interface = new galaxian_soundinterface();
        static void decode_background()
        {
            int i, j, k;
            byte[] tile = new byte[32 * 8 * 8];


            for (i = 0; i < 32; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    for (k = 0; k < 8; k++)
                    {
                        tile[i * 64 + j * 8 + k] = backcolor[i * 8 + j];
                    }
                }

                Mame.decodechar(Mame.Machine.gfx[3], i, new _BytePtr(tile), Mame.Machine.drv.gfxdecodeinfo[3].gfxlayout);
            }
        }
        static int common_vh_start()
        {
            int generator;
            int x, y;


            modify_charcode = null;
            modify_spritecode = null;

            mooncrst_gfxextend = 0;
            stars_on = 0;
            flipscreen[0] = 0;
            flipscreen[1] = 0;

            if (Generic.generic_vh_start() != 0)
                return 1;

            /* Default alternate background - Solid Blue */

            for (x = 0; x < 256; x++)
            {
                backcolor[x] = 0;
            }
            background_on = 0;

            decode_background();


            /* precalculate the star background */

            total_stars = 0;
            generator = 0;

            for (y = 255; y >= 0; y--)
            {
                for (x = 511; x >= 0; x--)
                {
                    int bit1, bit2;


                    generator <<= 1;
                    bit1 = (~generator >> 17) & 1;
                    bit2 = (generator >> 5) & 1;

                    if ((bit1 ^ bit2) != 0) generator |= 1;

                    if (((~generator >> 16) & 1) != 0 && (generator & 0xff) == 0xff)
                    {
                        int color;

                        color = (~(generator >> 8)) & 0x3f;
                        if (color != 0 && total_stars < MAX_STARS)
                        {
                            stars[total_stars].x = x;
                            stars[total_stars].y = y;
                            stars[total_stars].code = color;

                            total_stars++;
                        }
                    }
                }
            }


            /* all the games except New Sinbad 7 clip the sprites at the top of the screen,
               New Sinbad 7 does it at the bottom */
            //if (Mame.Machine.gamedrv == &driver_newsin7)
            //{
            //    spritevisiblearea      = &_spritevisibleareaflipx;
            //    spritevisibleareaflipx = &_spritevisiblearea;
            //}
            //else
            {
                spritevisiblearea = _spritevisiblearea;
                spritevisibleareaflipx = _spritevisibleareaflipx;
            }


            return 0;
        }
        static int galaxian_vh_start()
        {

            stars_type = 0;
            return common_vh_start();
        }
        static void galaxian_stars_w(int offset, int data)
        {
            stars_on = (data & 1);
            stars_scroll = 0;
        }
        static void galaxian_flipx_w(int offset, int data)
        {
            if (flipscreen[0] != (data & 1))
            {
                flipscreen[0] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        static void galaxian_flipy_w(int offset, int data)
        {
            if (flipscreen[1] != (data & 1))
            {
                flipscreen[1] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        public static void galaxian_vh_convert_color_prom(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
        {
            color_mask = (Mame.Machine.gfx[0].color_granularity == 4) ? 7 : 3;
            int cpi = 0, pi = 0;
            /* first, the character/sprite palette */
            for (int i = 0; i < 32; i++)
            {
                int bit0, bit1, bit2;

                /* red component */
                bit0 = (color_prom[cpi] >> 0) & 0x01;
                bit1 = (color_prom[cpi] >> 1) & 0x01;
                bit2 = (color_prom[cpi] >> 2) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                /* green component */
                bit0 = (color_prom[cpi] >> 3) & 0x01;
                bit1 = (color_prom[cpi] >> 4) & 0x01;
                bit2 = (color_prom[cpi] >> 5) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                /* blue component */
                bit0 = (color_prom[cpi] >> 6) & 0x01;
                bit1 = (color_prom[cpi] >> 7) & 0x01;
                palette[pi++] = (byte)(0x4f * bit0 + 0xa8 * bit1);

                cpi++;
            }

            /* now the stars */
            for (int i = 0; i < 64; i++)
            {
                int bits;
                byte[] map = { 0x00, 0x88, 0xcc, 0xff };

                bits = (i >> 0) & 0x03;
                palette[pi++] = map[bits];
                bits = (i >> 2) & 0x03;
                palette[pi++] = map[bits];
                bits = (i >> 4) & 0x03;
                palette[pi++] = map[bits];
            }

            /* characters and sprites use the same palette */
            for (int i = 0; i < TOTAL_COLORS(0); i++)
            {
                /* 00 is always mapped to pen 0 */
                if ((i & (Mame.Machine.gfx[0].color_granularity - 1)) == 0)
                    COLOR(colortable, 0, i, 0);
            }

            /* bullets can be either white or yellow */

            COLOR(colortable, 2, 0, 0);
            COLOR(colortable, 2, 1, 0x0f + STARS_COLOR_BASE);	/* yellow */
            COLOR(colortable, 2, 2, 0);
            COLOR(colortable, 2, 3, 0x3f + STARS_COLOR_BASE);	/* white */

            /* default blue background */
            palette[pi++] = 0;
            palette[pi++] = 0;
            palette[pi++] = 0x55;

            for (int i = 0; i < TOTAL_COLORS(3); i++)
            {
                COLOR(colortable, 3, i, (ushort)(96 + (i % (Mame.Machine.drv.total_colors - 96))));
            }
        }
        public static void galaxian_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int i, offs;

            /* for every character in the Video RAM, check if it has been modified */
            /* since last time and update it accordingly. */
            for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
            {
                if (Generic.dirtybuffer[offs])
                {
                    int sx, sy, charcode, background_charcode;


                    Generic.dirtybuffer[offs] = false;

                    sx = offs % 32;
                    sy = offs / 32;

                    background_charcode = sx;

                    charcode = Generic.videoram[offs];

                    if (flipscreen[0] != 0) sx = 31 - sx;
                    if (flipscreen[1] != 0) sy = 31 - sy;

                    if (modify_charcode != null)
                    {
                        modify_charcode(ref charcode, offs);
                    }

                    if (background_on != 0)
                    {
                        /* Draw background */

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[3],
                                (uint)background_charcode,
                                0,
                                flipscreen[0] != 0, flipscreen[1] != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }

                    Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                            (uint)charcode,
                            (uint)(galaxian_attributesram[2 * (offs % 32) + 1] & color_mask),
                            flipscreen[0] != 0, flipscreen[1] != 0,
                            8 * sx, 8 * sy,
                            null, background_on != 0 ? Mame.TRANSPARENCY_COLOR : Mame.TRANSPARENCY_NONE, 0);
                }
            }


            /* copy the temporary bitmap to the screen */
            {
                int[] scroll = new int[32];


                if (flipscreen[0] != 0)
                {
                    for (i = 0; i < 32; i++)
                    {
                        scroll[31 - i] = -galaxian_attributesram[2 * i];
                        if (flipscreen[1] != 0) scroll[31 - i] = -scroll[31 - i];
                    }
                }
                else
                {
                    for (i = 0; i < 32; i++)
                    {
                        scroll[i] = -galaxian_attributesram[2 * i];
                        if (flipscreen[1] != 0) scroll[i] = -scroll[i];
                    }
                }

                Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 32, scroll, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
            }


            /* draw the bullets */
            for (offs = 0; offs < galaxian_bulletsram_size[0]; offs += 4)
            {
                int x, y;
                int color;


                if (offs == 7 * 4) color = 0;	/* yellow */
                else color = 1;	/* white */

                x = 255 - galaxian_bulletsram[offs + 3] - Mame.Machine.drv.gfxdecodeinfo[2].gfxlayout.width;
                y = 255 - galaxian_bulletsram[offs + 1];
                if (flipscreen[1] != 0) y = 255 - y;

                Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                        0,	/* this is just a line, generated by the hardware */
                        (uint)color,
                        false, false,
                        x, y,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }


            /* Draw the sprites */
            for (offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
            {
                int flipx, flipy, sx, sy, spritecode;


                sx = (Generic.spriteram[offs + 3] + 1) & 0xff; /* This is definately correct in Mariner. Look at
												  the 'gate' moving up/down. It stops at the
  												  right spots */
                sy = 240 - Generic.spriteram[offs];
                flipx = Generic.spriteram[offs + 1] & 0x40;
                flipy = Generic.spriteram[offs + 1] & 0x80;
                spritecode = Generic.spriteram[offs + 1] & 0x3f;

                if (modify_spritecode != null)
                {
                    modify_spritecode(ref spritecode, ref flipx, ref flipy, offs);
                }

                if (flipscreen[0] != 0)
                {
                    sx = 240 - sx;	/* I checked a bunch of games including Scramble
							   (# of pixels the ship is from the top of the mountain),
			                   Mariner and Checkman. This is correct for them */
                    flipx = flipx == 0 ? 1 : 0;
                }
                if (flipscreen[1] != 0)
                {
                    sy = 240 - sy;
                    flipy = flipy == 0 ? 1 : 0;
                }

                /* In Amidar, */
                /* Sprites #0, #1 and #2 need to be offset one pixel to be correctly */
                /* centered on the ladders in Turtles (we move them down, but since this */
                /* is a rotated game, we actually move them left). */
                /* Note that the adjustment must be done AFTER handling flipscreen, thus */
                /* proving that this is a hardware related "feature" */
                /* This is not Amidar, it is Galaxian/Scramble/hundreds of clones, and I'm */
                /* not sure it should be the same. A good game to test alignment is Armored Car */
                /*		if (offs <= 2*4) sy++;*/

                Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                        (uint)spritecode,
                        (uint)(Generic.spriteram[offs + 2] & color_mask),
                        flipx != 0, flipy != 0,
                        sx, sy,
                        flipscreen[0] != 0 ? spritevisibleareaflipx : spritevisiblearea, Mame.TRANSPARENCY_PEN, 0);
            }


            /* draw the stars */
            if (stars_on != 0)
            {
                switch (stars_type)
                {
                    case -1: /* no stars */
                        break;

                    case 0:	/* Galaxian stars */
                    case 3:	/* Mariner stars */
                        for (offs = 0; offs < total_stars; offs++)
                        {
                            int x, y;


                            x = (int)((stars[offs].x + stars_scroll) % 512) / 2;
                            y = (int)(stars[offs].y + (stars_scroll + stars[offs].x) / 512) % 256;

                            if (y >= Mame.Machine.drv.visible_area.min_y &&
                                y <= Mame.Machine.drv.visible_area.max_y)
                            {
                                /* No stars below row (column) 64, between rows 176 and 215 or
                                   between 224 and 247 */
                                if ((stars_type == 3) &&
                                    ((x < 64) ||
                                    ((x >= 176) && (x < 216)) ||
                                    ((x >= 224) && (x < 248)))) continue;

                                if (((y & 1) ^ ((x >> 4) & 1)) != 0)
                                {
                                    plot_star(bitmap, x, y, stars[offs].code);
                                }
                            }
                        }
                        break;

                    case 1:	/* Scramble stars */
                    case 2:	/* Rescue stars */
                        for (offs = 0; offs < total_stars; offs++)
                        {
                            int x, y;


                            x = stars[offs].x / 2;
                            y = stars[offs].y;

                            if (y >= Mame.Machine.drv.visible_area.min_y &&
                                y <= Mame.Machine.drv.visible_area.max_y)
                            {
                                if ((stars_type != 2 || x < 128) &&	/* draw only half screen in Rescue */
                                   ((y & 1) ^ ((x >> 4) & 1)) != 0)
                                {
                                    /* Determine when to skip plotting */
                                    switch (stars_blink)
                                    {
                                        case 0:
                                            if ((stars[offs].code & 1) == 0) continue;
                                            break;
                                        case 1:
                                            if ((stars[offs].code & 4) == 0) continue;
                                            break;
                                        case 2:
                                            if ((stars[offs].x & 4) == 0) continue;
                                            break;
                                        case 3:
                                            /* Always plot */
                                            break;
                                    }
                                    plot_star(bitmap, x, y, stars[offs].code);
                                }
                            }
                        }
                        break;
                }
            }
        }
        static void plot_star(Mame.osd_bitmap bitmap, int x, int y, int code)
        {
            int backcol, pixel;

            backcol = backcolor[x];

            if (flipscreen[0] != 0)
            {
                x = 255 - x;
            }
            if (flipscreen[1] != 0)
            {
                y = 255 - y;
            }

            pixel = Mame.read_pixel(bitmap, x, y);

            if ((pixel == Mame.Machine.pens.read16(0)) ||
                (pixel == Mame.Machine.pens.read16(96 + backcol)))
            {
                Mame.plot_pixel(bitmap, x, y, Mame.Machine.pens.read16(STARS_COLOR_BASE + code));
            }
        }

        class machine_driver_galaxian : Mame.MachineDriver
        {
            public machine_driver_galaxian()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6, galaxian_readmem, galaxian_writemem, null, null, galaxian_vh_interrupt, 1));
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
                galaxian_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //none
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
