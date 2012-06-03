using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_galaxian
    {
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
        const int NOISE_AMPLITUDE = 70 * 256;
        const int TOOTHSAW_AMPLITUDE = 64;

        const int MINFREQ = 139 - 139 / 3;
        const int MAXFREQ = 139 + 139 / 3;

        static Mame.Timer.timer_entry lfotimer = null;
        static int noisevolume;
        static _ShortPtr noisewave;
        static _ShortPtr shootwave;
        static int shoot_length;
        static int shoot_rate;
        static int shootsampleloaded = 0;
        static int deathsampleloaded = 0;
        static int last_port1 = 0;
        static int last_port2 = 0;

        static sbyte[][] tonewave = new sbyte[4][];

        static short[] backgroundwave = {
   0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000,
   0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000, 0x4000,
   0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,
  -0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,-0x4000,
};
        static int channelnoise, channelshoot, channellfo;
        static int tone_stream;

        static void galaxian_background_enable_w(int offset, int data)
        {
            Mame.mixer_set_volume(channellfo + offset, (data & 1) != 0 ? 100 : 0);
        }
        static void galaxian_lfo_freq_w(int offset, int data)
        {
            throw new Exception();
        }
        static void galaxian_noise_enable_w(int offset, int data)
        {
            throw new Exception();
        }
        static void galaxian_shoot_enable_w(int offset, int data)
        {
            throw new Exception();
        }
        static void galaxian_vol_w(int offset, int data)
        {
            throw new Exception();
        }

        static void galaxian_pitch_w(int offset, int data)
        {
            throw new Exception();
        }

    }
}
