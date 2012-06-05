using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class Samplesinterface
        {
            public int channels, volume;
            public string[] samplenames;
            public Samplesinterface(int channels, int volume, string[] samplenames)
            {
                this.channels = channels; this.volume = volume;
                this.samplenames = samplenames;
            }
        }
        public static void sample_start(int channel, int samplenum, int loop)
        {
            if (Machine.sample_rate == 0) return;
            if (Machine.samples == null) return;
            if (Machine.samples.sample[samplenum] == null) return;
            if (channel >= numchannels)
            {
                printf("error: sample_start() called with channel = %d, but only %d channels allocated\n", channel, numchannels);
                return;
            }
            if (samplenum >= Machine.samples.total)
            {
                printf("error: sample_start() called with samplenum = %d, but only %d samples available\n", samplenum, Machine.samples.total);
                return;
            }

            if (Machine.samples.sample[samplenum].resolution == 8)
            {
                mixer_play_sample(firstchannel + channel,
                       Machine.samples.sample[samplenum].data,
                        Machine.samples.sample[samplenum].length,
                        Machine.samples.sample[samplenum].smpfreq,
                        loop != 0);
            }
            else
            {
                mixer_play_sample_16(firstchannel + channel,
                        new _ShortPtr(Machine.samples.sample[samplenum].data),
                        Machine.samples.sample[samplenum].length,
                        Machine.samples.sample[samplenum].smpfreq,
                        loop != 0);
            }
        }
        public static void sample_stop(int channel)
        {
            if (Machine.sample_rate == 0) return;
            if (channel >= numchannels)
            {
                printf("error: sample_stop() called with channel = %d, but only %d channels allocated\n", channel, numchannels);
                return;
            }

            mixer_stop_sample(channel + firstchannel);
        }
        static int firstchannel, numchannels;
        public class Samples : Mame.snd_interface
        {
            public Samples()
            {
                this.name = "Samples";
                this.sound_num = Mame.SOUND_SAMPLES;
            }
            public override int chips_clock(MachineSound msound)
            {
                return 0;
            }
            public override int chips_num(MachineSound msound)
            {
                return 0;
            }
            public override void reset()
            {
                //none
            }
            public override void stop()
            {
                //none
            }
            public override void update()
            {
                //none
            }
            public override int start(MachineSound msound)
            {
                int i;
                int[] vol = new int[MIXER_MAX_CHANNELS];
                Samplesinterface intf = (Samplesinterface)msound.sound_interface;

                /* read audio samples if available */
                Machine.samples = readsamples(intf.samplenames, Machine.gamedrv.name);

                numchannels = intf.channels;
                for (i = 0; i < numchannels; i++)
                    vol[i] = intf.volume;
                firstchannel = mixer_allocate_channels(numchannels, vol);
                for (i = 0; i < numchannels; i++)
                {
                    string buf = sprintf("Sample #%d", i);
                    mixer_set_name(firstchannel + i, buf);
                }
                return 0;
            }
        }

        public static void sample_set_freq(int channel, int freq)
        {
            throw new Exception();
        }
        public static void sample_set_volume(int channel, int volume)
        {

            if (Machine.sample_rate == 0) return;
            if (Machine.samples == null) return;
            if (channel >= numchannels)
            {
                printf("error: sample_adjust() called with channel = %d, but only %d channels allocated\n", channel, numchannels);
                return;
            }

            mixer_set_volume(channel + firstchannel, volume * 100 / 255);
        }
        public static bool sample_playing(int channel)
        {
            if (Machine.sample_rate == 0) return false;
            if (channel >= numchannels)
            {
                printf("error: sample_playing() called with channel = %d, but only %d channels allocated\n", channel, numchannels);
                return false;
            }

            return mixer_is_sample_playing(channel + firstchannel);
        }
    }
}
