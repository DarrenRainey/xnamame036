using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class k005289
    {
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



        static void k005289_recompute()
{
	k005289_sound_channel[] voice = channel_list;

	Mame.stream_update(stream,0); 	/* update the streams */

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
}
