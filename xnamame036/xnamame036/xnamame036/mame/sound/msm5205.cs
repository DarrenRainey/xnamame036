using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAX_MSM5205 = 4;
    }
    public class MSM5205_interface
    {
        public int num, baseclock;
        public MSM5205.irqcallback[] vclk_interrupt;
        public int[] select;
        public int[] mixing_level;
        public MSM5205_interface(int num, int baseclock, MSM5205.irqcallback[] vclk_interrupt, int[] select, int[] mixing_level)
        {
            this.num = num; this.baseclock = baseclock;
            this.vclk_interrupt = vclk_interrupt;
            this.select = select; this.mixing_level = mixing_level;
        }
    }
    public class MSM5205 : Mame.snd_interface
    {
        public MSM5205()
        {
            this.sound_num = Mame.SOUND_MSM5205;
            this.name = "MSM5205";
        }
        public const byte MSM5205_S96_3B = 0;     /* prsicaler 1/96(4KHz) , data 3bit */
        public const byte MSM5205_S48_3B = 1;/* prsicaler 1/48(8KHz) , data 3bit */
        public const byte MSM5205_S64_3B = 2;/* prsicaler 1/64(6KHz) , data 3bit */
        public const byte MSM5205_SEX_3B = 3; /* VCLK slave mode      , data 3bit */
        public const byte MSM5205_S96_4B = 4;  /* prsicaler 1/96(4KHz) , data 4bit */
        public const byte MSM5205_S48_4B = 5;   /* prsicaler 1/48(8KHz) , data 4bit */
        public const byte MSM5205_S64_4B = 6;    /* prsicaler 1/64(6KHz) , data 4bit */
        public const byte MSM5205_SEX_4B = 7;     /* VCLK slave mode      , data 4bit */

        public delegate void irqcallback(int i);
        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((MSM5205_interface)msound.sound_interface).baseclock;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((MSM5205_interface)msound.sound_interface).num;
        }
        public override int start(Mame.MachineSound msound)
        {
            /* save a global pointer to our interface */
            msm5205_intf = (MSM5205_interface)msound.sound_interface;

            /* compute the difference tables */
            ComputeTables();

            /* initialize the voices */
            //memset (msm5205, 0, sizeof (msm5205));

            /* stream system initialize */
            for (int i = 0; i < msm5205_intf.num; i++)
            {
                MSM5205Voice voice = msm5205[i];
                string name = Mame.sprintf("MSM5205 #%d", i);
                voice.stream = Mame.stream_init(name, msm5205_intf.mixing_level[i],
                                        Mame.Machine.sample_rate, i,
                                        MSM5205_update);
            }
            /* initialize */
            MSM5205_sh_reset();
            /* success */
            return 0;
        }
        public override void stop()
        {

        }
        public override void reset()
        {
            MSM5205_sh_reset();
        }
        public override void update()
        {

        }
        static int[] diff_lookup = new int[49 * 16];
        static void ComputeTables()
        {
            /* nibble to bit map */
            int[][] nbl2bit =
	{
		new int[]{ 1, 0, 0, 0}, new int[]{ 1, 0, 0, 1}, new int[]{ 1, 0, 1, 0}, new int[]{ 1, 0, 1, 1},
		new int[]{ 1, 1, 0, 0}, new int[]{ 1, 1, 0, 1}, new int[]{ 1, 1, 1, 0}, new int[]{ 1, 1, 1, 1},
		new int[]{-1, 0, 0, 0}, new int[]{-1, 0, 0, 1}, new int[]{-1, 0, 1, 0}, new int[]{-1, 0, 1, 1},
		new int[]{-1, 1, 0, 0}, new int[]{-1, 1, 0, 1}, new int[]{-1, 1, 1, 0}, new int[]{-1, 1, 1, 1}
	};

            int step, nib;

            /* loop over all possible steps */
            for (step = 0; step <= 48; step++)
            {
                /* compute the step value */
                int stepval = (int)(Math.Floor(16.0 * Math.Pow(11.0 / 10.0, (double)step)));

                /* loop over all nibbles and compute the difference */
                for (nib = 0; nib < 16; nib++)
                {
                    diff_lookup[step * 16 + nib] = nbl2bit[nib][0] *
                        (stepval * nbl2bit[nib][1] +
                         stepval / 2 * nbl2bit[nib][2] +
                         stepval / 4 * nbl2bit[nib][3] +
                         stepval / 8);
                }
            }
        }
        struct MSM5205Voice
        {
            public int stream;
            public object timer;
            public int data;
            public int vclk;
            public int reset;
            public int prescaler;
            public int bitwidth;
            public int signal;
            public int step;
        }
        static MSM5205_interface msm5205_intf;

        static int[] index_shift = { -1, -1, -1, -1, 2, 4, 6, 8 };
        static MSM5205Voice[] msm5205 = new MSM5205Voice[Mame.MAX_MSM5205];
        public static void MSM5205_data_w(int num, int data)
        {
            if (msm5205[num].bitwidth == 4)
                msm5205[num].data = data & 0x0f;
            else
                msm5205[num].data = (data & 0x07) << 1; /* unknown */
        }
        public static void MSM5205_reset_w(int num, int reset)
        {
            /* range check the numbers */
            if (num >= msm5205_intf.num)
            {
                //if (errorlog) fprintf(errorlog,"error: MSM5205_reset_w() called with chip = %d, but only %d chips allocated\n", num, msm5205_intf.num);
                return;
            }
            msm5205[num].reset = reset;
        }
        static void MSM5205_set_timer(int num, int select)
        {
            MSM5205Voice voice = msm5205[num];
            int[] prescaler_table = { 96, 48, 64, 0 };
            int prescaler = prescaler_table[select & 0x03];

            if (voice.prescaler != prescaler)
            {
                /* remove VCLK timer */
                if (voice.timer != null)
                {
                    Mame.Timer.timer_remove(voice.timer);
                    voice.timer = null;
                }
                voice.prescaler = prescaler;
                /* timer set */
                if (prescaler != 0)
                {
                    voice.timer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_HZ(msm5205_intf.baseclock / prescaler), num, MSM5205_vclk_callback);
                }
            }
        }

        static void MSM5205_sh_reset()
        {
            /* bail if we're not emulating sound */
            if (Mame.Machine.sample_rate == 0)
                return;

            for (int i = 0; i < msm5205_intf.num; i++)
            {
                MSM5205Voice voice = msm5205[i];
                /* initialize work */
                voice.data = 0;
                voice.vclk = 0;
                voice.reset = 0;
                voice.signal = 0;
                voice.step = 0;
                /* timer set */
                MSM5205_set_timer(i, msm5205_intf.select[i] & 0x03);
                /* bitwidth reset */
                msm5205[i].bitwidth = (msm5205_intf.select[i] & 0x04) != 0 ? 4 : 3;
            }
        }

        static void MSM5205_update(int chip, _ShortPtr buffer, int length)
        {
            MSM5205Voice voice = msm5205[chip];

            /* if this voice is active */
            if (voice.signal != 0)
            {
                short val = (short)(voice.signal * 16);
                int i = 0;
                while (length != 0)
                {
                    buffer.write16(i++, (ushort)val);
                    length--;
                }
            }
            else
                Array.Clear(buffer.buffer, buffer.offset, length);
        }

        /* timer callback at VCLK low eddge */
        static void MSM5205_vclk_callback(int num)
        {
            MSM5205Voice voice = msm5205[num];
            int val;
            int new_signal;
            /* callback user handler and latch next data */
            if (msm5205_intf.vclk_interrupt[num] != null) msm5205_intf.vclk_interrupt[num](num);

            /* reset check at last hieddge of VCLK */
            if (voice.reset != 0)
            {
                new_signal = 0;
                voice.step = 0;
            }
            else
            {
                /* update signal */
                /* !! MSM5205 has internal 12bit decoding, signal width is 0 to 8191 !! */
                val = voice.data;
                new_signal = voice.signal + diff_lookup[voice.step * 16 + (val & 15)];
                if (new_signal > 2047) new_signal = 2047;
                else if (new_signal < -2048) new_signal = -2048;
                voice.step += index_shift[val & 7];
                if (voice.step > 48) voice.step = 48;
                else if (voice.step < 0) voice.step = 0;
            }
            /* update when signal changed */
            if (voice.signal != new_signal)
            {
                Mame.stream_update(voice.stream, 0);
                voice.signal = new_signal;
            }
        }
    }
}
