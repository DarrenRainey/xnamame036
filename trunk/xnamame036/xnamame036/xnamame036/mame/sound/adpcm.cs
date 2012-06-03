using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class OKIM6295interface
    {
        public OKIM6295interface(int num, int[] frequency, int[] region, int[] mixing_level)
        {
            this.num = num; this.frequency = frequency;
            this.region = region; this.mixing_level = mixing_level;
        }
        public int num;
        public int[] frequency = new int[okim6295.MAX_OKIM6295];
        public int[] region = new int[okim6295.MAX_OKIM6295];
        public int[] mixing_level = new int[okim6295.MAX_OKIM6295];
    }


    class okim6295 : Mame.snd_interface
    {
        public okim6295()
        {
            this.name = "OKI6295";
            this.sound_num = Mame.SOUND_OKIM6295;
        }

        public const int MAX_OKIM6295 = 2;
        public const int MAX_OKIM6295_VOICES = 4;
        public const int ALL_VOICES = -1;

        const int MAX_ADPCM = 8;

        const int MAX_SAMPLE_CHUNK = 10000;
        const int FRAC_BITS = 14;
        const int FRAC_ONE = 1 << FRAC_BITS;
        const int FRAC_MASK = FRAC_ONE - 1;
        class ADPCMsample
        {
            public int num, offset, length;
        }

        struct ADPCMVoice
        {
            public int stream;				/* which stream are we playing on? */
            public byte playing;			/* 1 if we are actively playing */

            public _BytePtr region_base;		/* pointer to the base of the region */
            public _BytePtr _base;			/* pointer to the base memory location */
            public uint sample;			/* current sample number */
            public uint count;			/* total samples to play */

            public uint signal;			/* current ADPCM signal */
            public uint step;			/* current ADPCM step */
            public uint volume;			/* output volume */

            public short last_sample;		/* last sample output */
            public short curr_sample;		/* current sample target */
            public uint source_step;		/* step value for frequency conversion */
            public uint source_pos;		/* current fractional position */
        };
        static byte num_voices;

        static ADPCMVoice[] _adpcm = new ADPCMVoice[MAX_ADPCM];
        static ADPCMsample sample_list;
        static int[] index_shift = { -1, -1, -1, -1, 2, 4, 6, 8 };
        static int[] diff_lookup = new int[49 * 16];
        static uint[] volume_table = new uint[16];

        static int[] okim6295_command = new int[MAX_OKIM6295];
        static int[][] okim6295_base = new int[MAX_OKIM6295][];


        static okim6295()
        {
            for (int i = 0; i < MAX_OKIM6295; i++)
                okim6295_base[i] = new int[MAX_OKIM6295_VOICES];
        }


        static void OKIM6295_data_w(int num, int data)
        {
            /* range check the numbers */
            if (num >= num_voices / MAX_OKIM6295_VOICES)
            {
                Mame.printf("error: OKIM6295_data_w() called with chip = %d, but only %d chips allocated\n", num, num_voices / MAX_OKIM6295_VOICES);
                return;
            }

            /* if a command is pending, process the second half */
            if (okim6295_command[num] != -1)
            {
                int temp = data >> 4, i, start, stop;
                _BytePtr _base;

                /* determine which voice(s) (voice is set by a 1 bit in the upper 4 bits of the second byte) */
                for (i = 0; i < MAX_OKIM6295_VOICES; i++, temp >>= 1)
                    if ((temp & 1) != 0)
                    {
                        ADPCMVoice voice = _adpcm[num * MAX_OKIM6295_VOICES + i];

                        /* update the stream */
                        Mame.stream_update(voice.stream, 0);

                        /* determine the start/stop positions */
                        _base = new _BytePtr(voice.region_base, okim6295_base[num][i] + okim6295_command[num] * 8);
                        start = (_base[0] << 16) + (_base[1] << 8) + _base[2];
                        stop = (_base[3] << 16) + (_base[4] << 8) + _base[5];

                        /* set up the voice to play this sample */
                        if (start < 0x40000 && stop < 0x40000)
                        {
                            voice.playing = 1;
                            voice._base = new _BytePtr(voice.region_base, okim6295_base[num][i] + start);
                            voice.sample = 0;
                            voice.count = (uint)(2 * (stop - start + 1));

                            /* also reset the ADPCM parameters */
                            voice.signal = unchecked((uint)-2);
                            voice.step = 0;
                            voice.volume = volume_table[data & 0x0f];
                        }

                        /* invalid samples go here */
                        else
                        {
                            Mame.printf("OKIM6295: requested to play invalid sample %02x\n", okim6295_command[num]);
                            voice.playing = 0;
                        }
                    }

                /* reset the command */
                okim6295_command[num] = -1;
            }

            /* if this is the start of a command, remember the sample number for next time */
            else if ((data & 0x80) != 0)
            {
                okim6295_command[num] = data & 0x7f;
            }

            /* otherwise, see if this is a silence command */
            else
            {
                int temp = data >> 3, i;

                /* determine which voice(s) (voice is set by a 1 bit in bits 3-6 of the command */
                for (i = 0; i < 4; i++, temp >>= 1)
                    if ((temp & 1) != 0)
                    {
                        ADPCMVoice voice = _adpcm[num * MAX_OKIM6295_VOICES + i];

                        /* update the stream, then turn it off */
                        Mame.stream_update(voice.stream, 0);
                        voice.playing = 0;
                    }
            }
        }
        public static void OKIM6295_set_bank_base(int which, int channel, int _base)
        {

            /* handle the all voice case */
            if (channel == ALL_VOICES)
            {
                int i;
                for (i = 0; i < MAX_OKIM6295_VOICES; i++)
                    OKIM6295_set_bank_base(which, i, _base);
                return;
            }
            ADPCMVoice voice = _adpcm[which * MAX_OKIM6295_VOICES + channel];

            /* update the stream and set the new base */
            Mame.stream_update(voice.stream, 0);
            okim6295_base[which][channel] = _base;
        }

        static void compute_tables()
        {
            /* nibble to bit map */
            int[][] nbl2bit =
	{
		new int[]{ 1, 0, 0, 0}, new int[]{ 1, 0, 0, 1},new int[]{ 1, 0, 1, 0}, new int[]{ 1, 0, 1, 1},
		new int[]{ 1, 1, 0, 0}, new int[]{ 1, 1, 0, 1},new int[]{ 1, 1, 1, 0}, new int[]{ 1, 1, 1, 1},
		new int[]{-1, 0, 0, 0}, new int[]{-1, 0, 0, 1},new int[]{-1, 0, 1, 0}, new int[]{-1, 0, 1, 1},
		new int[]{-1, 1, 0, 0}, new int[]{-1, 1, 0, 1},new int[]{-1, 1, 1, 0}, new int[]{-1, 1, 1, 1}
	};

            int step, nib;

            /* loop over all possible steps */
            for (step = 0; step <= 48; step++)
            {
                /* compute the step value */
                int stepval = (int)Math.Floor(16.0 * Math.Pow(11.0 / 10.0, (double)step));

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

            /* generate the volume table (currently just a guess) */
            for (step = 0; step < 16; step++)
            {
                double _out = 256.0;
                int vol = step;

                /* assume 2dB per step (most likely wrong!) */
                while (vol-- > 0)
                    _out /= 1.258925412;	/* = 10 ^ (2/20) = 2dB */
                volume_table[step] = (uint)_out;
            }
        }
        static void adpcm_update(int num, _ShortPtr buffer, int length)
        {
            ADPCMVoice voice = _adpcm[num];
            _ShortPtr sample_data = new _ShortPtr(MAX_SAMPLE_CHUNK * 2), curr_data = new _ShortPtr(sample_data);
            short prev = voice.last_sample, curr = voice.curr_sample;
            uint final_pos;
            uint new_samples;

            /* finish off the current sample */
            if (voice.source_pos > 0)
            {
                /* interpolate */
                while (length > 0 && voice.source_pos < FRAC_ONE)
                {
                    buffer.write16(0, (ushort)((((int)prev * (FRAC_ONE - voice.source_pos)) + ((int)curr * voice.source_pos)) >> FRAC_BITS));
                    buffer.offset += 2;
                    voice.source_pos += voice.source_step;
                    length--;
                }

                /* if we're over, continue; otherwise, we're done */
                if (voice.source_pos >= FRAC_ONE)
                    voice.source_pos -= FRAC_ONE;
                else
                    return;
            }

            /* compute how many new samples we need */
            final_pos = (uint)(voice.source_pos + length * voice.source_step);
            new_samples = (final_pos + FRAC_ONE - 1) >> FRAC_BITS;
            if (new_samples > MAX_SAMPLE_CHUNK)
                new_samples = MAX_SAMPLE_CHUNK;

            /* generate them into our buffer */
            generate_adpcm(voice, sample_data, (int)new_samples);
            prev = curr;
            curr = (short)curr_data.read16(0); curr_data.offset += 2;

            /* then sample-rate convert with linear interpolation */
            while (length > 0)
            {
                /* interpolate */
                while (length > 0 && voice.source_pos < FRAC_ONE)
                {
                    buffer.write16(0, (ushort)((((int)prev * (FRAC_ONE - voice.source_pos)) + ((int)curr * voice.source_pos)) >> FRAC_BITS));
                    buffer.offset += 2;
                    voice.source_pos += voice.source_step;
                    length--;
                }

                /* if we're over, grab the next samples */
                if (voice.source_pos >= FRAC_ONE)
                {
                    voice.source_pos -= FRAC_ONE;
                    prev = curr;
                    curr = (short)curr_data.read16(0); curr_data.offset += 2;
                }
            }

            /* remember the last samples */
            voice.last_sample = prev;
            voice.curr_sample = curr;
        }
        static void generate_adpcm(ADPCMVoice voice, _ShortPtr buffer, int samples)
        {
            /* if this voice is active */
            if (voice.playing != 0)
            {
                _BytePtr _base = voice._base;
                int sample = (int)voice.sample;
                int signal = (int)voice.signal;
                int count = (int)voice.count;
                int step = (int)voice.step;
                int val;

                /* loop while we still have samples to generate */
                while (samples != 0)
                {
                    /* compute the new amplitude and update the current step */
                    val = _base[sample / 2] >> (((sample & 1) << 2) ^ 4);
                    signal += diff_lookup[step * 16 + (val & 15)];

                    /* clamp to the maximum */
                    if (signal > 2047)
                        signal = 2047;
                    else if (signal < -2048)
                        signal = -2048;

                    /* adjust the step size and clamp */
                    step += index_shift[val & 7];
                    if (step > 48)
                        step = 48;
                    else if (step < 0)
                        step = 0;

                    /* output to the buffer, scaling by the volume */
                    buffer.write16(0, (ushort)(signal * voice.volume / 16));
                    buffer.offset += 2;
                    samples--;

                    /* next! */
                    if (++sample > count)
                    {
                        voice.playing = 0;
                        break;
                    }
                }

                /* update the parameters */
                voice.sample = (uint)sample;
                voice.signal = (uint)signal;
                voice.step = (uint)step;
            }

            /* fill the rest with silence */
            while (samples-- != 0)
            {
                buffer.write16(0, 0);
                buffer.offset += 2;
            }
        }

        static int OKIM6295_status_r(int num)
        {
            int i, result;

            /* range check the numbers */
            if (num >= num_voices / MAX_OKIM6295_VOICES)
            {
                Mame.printf("error: OKIM6295_status_r() called with chip = %d, but only %d chips allocated\n", num, num_voices / MAX_OKIM6295_VOICES);
                return 0x0f;
            }

            /* set the bit to 1 if something is playing on a given channel */
            result = 0;
            for (i = 0; i < MAX_OKIM6295_VOICES; i++)
            {
                ADPCMVoice voice = _adpcm[num * MAX_OKIM6295_VOICES + i];

                /* update the stream */
                Mame.stream_update(voice.stream, 0);

                /* set the bit if it's playing */
                if (voice.playing != 0)
                    result |= 1 << i;
            }

            return result;
        }

        public static void OKIM6295_data_0_w(int offset, int data)
        {
            OKIM6295_data_w(0, data);
        }
        public static void OKIM6295_data_1_w(int offset, int data)
        {
            OKIM6295_data_w(1, data);
        }
        public static int OKIM6295_status_0_r(int offset)
        {
            return OKIM6295_status_r(0);
        }
        public static int OKIM6295_status_1_r(int offset)
        {
            return OKIM6295_status_r(1);
        }






        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((OKIM6295interface)msound.sound_interface).num;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((OKIM6295interface)msound.sound_interface).num;
        }
        public override int start(Mame.MachineSound msound)
        {
            OKIM6295interface intf = (OKIM6295interface)msound.sound_interface;
            string stream_name;
            int i;

            /* reset the ADPCM system */
            num_voices = (byte)(intf.num * MAX_OKIM6295_VOICES);
            compute_tables();
            sample_list = null;

            /* initialize the voices */

            for (i = 0; i < num_voices; i++)
            {
                int chip = i / MAX_OKIM6295_VOICES;
                int voice = i % MAX_OKIM6295_VOICES;

                /* reset the OKI-specific parameters */
                okim6295_command[chip] = -1;

                okim6295_base[chip][voice] = 0;

                /* generate the name and create the stream */
                stream_name = Mame.sprintf("%s #%d (voice %d)", Mame.sound_name(msound), chip, voice);
                _adpcm[i].stream = Mame.stream_init(stream_name, intf.mixing_level[chip], Mame.Machine.sample_rate, i, adpcm_update);
                if (_adpcm[i].stream == -1)
                    return 1;

                /* initialize the rest of the structure */
                _adpcm[i].region_base = Mame.memory_region(intf.region[chip]);
                _adpcm[i].volume = 255;
                _adpcm[i].signal = unchecked((uint)-2);
                if (Mame.Machine.sample_rate != 0)
                    _adpcm[i].source_step = (uint)((double)intf.frequency[chip] * (double)FRAC_ONE / (double)Mame.Machine.sample_rate);
            }

            /* success */
            return 0;
        }
        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void reset()
        {
            //none
        }
        public override void update()
        {
            //none
        }
    }
}
