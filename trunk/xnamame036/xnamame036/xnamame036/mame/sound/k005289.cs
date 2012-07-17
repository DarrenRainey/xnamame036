using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class k005289 : Mame.snd_interface
    {
        public k005289()
        {
            this.name = "005289";
            this.sound_num = Mame.SOUND_K005289;
        }
        const int FREQBASEBITS = 16;
        struct k005289_sound_channel
        {
            public int frequency, counter, volume;
            public _BytePtr wave;
        }
        static k005289_sound_channel[] channel_list = new k005289_sound_channel[2];
        static _BytePtr sound_prom;
        static int stream, mclock, rate;
        static _ShortPtr mixer_table;
        static _ShortPtr mixer_lookup;
        static _ShortPtr mixer_buffer;

        static int k005289_A_frequency, k005289_B_frequency;
        static int k005289_A_volume, k005289_B_volume;
        static int k005289_A_waveform, k005289_B_waveform;
        static int k005289_A_latch, k005289_B_latch;

        public override int chips_clock(Mame.MachineSound msound)
        {
            return 0;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return 0;
        }
        static int make_mixer_table(int voices)
        {
            int count = voices * 128;
            int i;
            int gain = 16;

            /* allocate memory */
            mixer_table = new _ShortPtr(256 * voices * sizeof(short));

            /* find the middle of the table */
            mixer_lookup = new _ShortPtr(mixer_table, (128 * voices) * 2);

            /* fill in the table - 16 bit case */
            for (i = 0; i < count; i++)
            {
                int val = i * gain * 16 / voices;
                if (val > 32767) val = 32767;
                mixer_lookup.write16(i, (ushort)val);
                mixer_lookup.write16(-i, (ushort)-val);
            }

            return 0;
        }


        public override int start(Mame.MachineSound msound)
        {
            string snd_name = "K005289";
            //k005289_sound_channel voice=channel_list;
            k005289_interface intf = (k005289_interface)msound.sound_interface;

            /* get stream channels */
            stream = Mame.stream_init(snd_name, intf.volume, Mame.Machine.sample_rate, 0, K005289_update);
            mclock = intf.master_clock;
            rate = Mame.Machine.sample_rate;

            /* allocate a pair of buffers to mix into - 1 second's worth should be more than enough */
            mixer_buffer = new _ShortPtr(2 * sizeof(short) * Mame.Machine.sample_rate);

            /* build the mixer table */
            if (make_mixer_table(2) != 0)
            {
                mixer_buffer = null;
                return 1;
            }

            sound_prom = Mame.memory_region(intf.region);

            /* reset all the voices */
            channel_list[0].frequency = 0;
            channel_list[0].volume = 0;
            channel_list[0].wave = new _BytePtr(sound_prom);
            channel_list[0].counter = 0;
            channel_list[1].frequency = 0;
            channel_list[1].volume = 0;
            channel_list[1].wave = new _BytePtr(sound_prom, 0x100);
            channel_list[1].counter = 0;

            return 0;
        }
        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void reset()
        {
            //nothing
        }
        public override void update()
        {
            //nothingthrow new NotImplementedException();
        }
        static void K005289_update(int ch, _ShortPtr buffer, int length)
        {
            _ShortPtr mix;

            /* zap the contents of the mixer buffer */
            Array.Clear(mixer_buffer.buffer, mixer_buffer.offset, length * sizeof(short));
            //memset(mixer_buffer, 0, length * sizeof(INT16));

            int v = channel_list[0].volume;
            int f = channel_list[0].frequency;
            if (v != 0 && f != 0)
            {
                _BytePtr w = channel_list[0].wave;
                int c = channel_list[0].counter;

                mix = new _ShortPtr(mixer_buffer);

                /* add our contribution */
                for (int i = 0; i < length; i++)
                {
                    int offs;

                    c += (int)((((float)mclock / (float)(f * 16)) * (float)(1 << FREQBASEBITS)) / (float)(rate / 32));
                    offs = (c >> 16) & 0x1f;
                    ushort _w = mix.read16(0);
                    mix.write16(0, (ushort)(_w + (short)(((w[offs] & 0x0f) - 8) * v)));
                }

                /* update the counter for this voice */
                channel_list[0].counter = c;
            }

            v = channel_list[1].volume;
            f = channel_list[1].frequency;
            if (v != 0 && f != 0)
            {
                _BytePtr w = channel_list[1].wave;
                int c = channel_list[1].counter;

                mix = mixer_buffer;

                /* add our contribution */
                for (int i = 0; i < length; i++)
                {
                    int offs;

                    c += (int)((((float)mclock / (float)(f * 16)) * (float)(1 << FREQBASEBITS)) / (float)(rate / 32));
                    offs = (c >> 16) & 0x1f;
                    ushort _w = mix.read16(0);
                    mix.write16(0, (ushort)(_w + (short)(((w[offs] & 0x0f) - 8) * v)));
                }

                /* update the counter for this voice */
                channel_list[1].counter = c;
            }

            /* mix it down */
            mix = new _ShortPtr(mixer_buffer);
            for (int i = 0; i < length; i++)
            {
                buffer.write16(0, (ushort)mixer_lookup.read16((short)mix.read16(0)));
                buffer.offset += 2;
                mix.offset += 2;
            }
        }

        public static void k005289_control_A_w(int offset, int data)
        {
            k005289_A_volume = data & 0xf;
            k005289_A_waveform = data >> 5;
            k005289_recompute();
        }
        public static void k005289_control_B_w(int offset, int data)
        {
            k005289_B_volume = data & 0xf;
            k005289_B_waveform = data >> 5;
            k005289_recompute();
        }


        static void k005289_recompute()
        {
            k005289_sound_channel[] voice = channel_list;

            Mame.stream_update(stream, 0); 	/* update the streams */

            voice[0].frequency = k005289_A_frequency;
            voice[1].frequency = k005289_B_frequency;
            voice[0].volume = k005289_A_volume;
            voice[1].volume = k005289_B_volume;
            voice[0].wave = new _BytePtr(sound_prom, 32 * k005289_A_waveform);
            voice[1].wave = new _BytePtr(sound_prom, 32 * k005289_B_waveform + 0x100);
        }

        public static void k005289_keylatch_A_w(int offset, int data)
        {
            k005289_A_frequency = k005289_A_latch;
            k005289_recompute();
        }

        public static void k005289_keylatch_B_w(int offset, int data)
        {
            k005289_B_frequency = k005289_B_latch;
            k005289_recompute();
        }
        public static void k005289_pitch_A_w(int offset, int data)
        {
            k005289_A_latch = 0x1000 - offset;
        }

        public static void k005289_pitch_B_w(int offset, int data)
        {
            k005289_B_latch = 0x1000 - offset;
        }

    }
    class k005289_interface
    {
        public int master_clock, volume, region;
        public k005289_interface(int master_clock, int volume, int region)
        {
            this.master_clock = master_clock; this.volume = volume; this.region = region;
        }
    }
}
