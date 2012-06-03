using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int BUFFER_LEN = 16384;
        static int[] stream_joined_channels = new int[MIXER_MAX_CHANNELS];
        static _ShortPtr[] stream_buffer = new _ShortPtr[MIXER_MAX_CHANNELS];
        static int[] stream_sample_rate = new int[MIXER_MAX_CHANNELS];
        static int[] stream_buffer_pos = new int[MIXER_MAX_CHANNELS];
        static int[] stream_sample_length = new int[MIXER_MAX_CHANNELS];
        static int[] stream_param = new int[MIXER_MAX_CHANNELS];
        int[] memory = new int[MIXER_MAX_CHANNELS];
        static int[] r1 = new int[MIXER_MAX_CHANNELS], r2 = new int[MIXER_MAX_CHANNELS], r3 = new int[MIXER_MAX_CHANNELS], c = new int[MIXER_MAX_CHANNELS];

        public delegate void _stream_callback(int param, _ShortPtr buffer, int length);
        public delegate void _stream_callback_multi(int param, _ShortPtr[] buffer, int length);

        static _stream_callback[] stream_callback = new _stream_callback[MIXER_MAX_CHANNELS];
        static _stream_callback_multi[] stream_callback_multi = new _stream_callback_multi[MIXER_MAX_CHANNELS];


        int streams_sh_start()
        {
            for (int i = 0; i < MIXER_MAX_CHANNELS; i++)
            {
                stream_joined_channels[i] = 1;
                stream_buffer[i] = null;
            }

            return 0;
        }
        void streams_sh_stop()
        {
            for (int i = 0; i < MIXER_MAX_CHANNELS; i++)
            {            
                stream_buffer[i] = null;
            }
        }

        public static int stream_init(string name, int default_mixing_level, int sample_rate, int param, _stream_callback callback)
        {
            int channel = mixer_allocate_channel(default_mixing_level);

            stream_joined_channels[channel] = 1;

            mixer_set_name(channel, name);

            stream_buffer[channel] = new _ShortPtr(sizeof(short) * BUFFER_LEN);

            stream_sample_rate[channel] = sample_rate;
            stream_buffer_pos[channel] = 0;
            if (sample_rate != 0)
                stream_sample_length[channel] = 1000000 / sample_rate;
            else
                stream_sample_length[channel] = 0;
            stream_param[channel] = param;
            stream_callback[channel] = callback;
            set_RC_filter(channel, 0, 0, 0, 0);

            return channel;
        }
        public static int stream_init_multi(int channels, string[] names, int[] default_mixing_levels, int sample_rate, int param, _stream_callback_multi callback)
        {
            int channel = mixer_allocate_channels(channels, default_mixing_levels);

            stream_joined_channels[channel] = channels;

            for (int i = 0; i < channels; i++)
            {
                mixer_set_name(channel + i, names[i]);

                stream_buffer[channel + i] = new _ShortPtr(sizeof(short) * BUFFER_LEN);

                stream_sample_rate[channel + i] = sample_rate;
                stream_buffer_pos[channel + i] = 0;
                if (sample_rate!=0)
                    stream_sample_length[channel + i] = 1000000 / sample_rate;
                else
                    stream_sample_length[channel + i] = 0;
            }

            stream_param[channel] = param;
            stream_callback_multi[channel] = callback;
            set_RC_filter(channel, 0, 0, 0, 0);

            return channel;
        }
        const int EXTRA_SAMPLES = 1;
        static int mixer_need_samples_this_frame(int channel, int freq)
        {
            return (int)(samples_this_frame - mixer_channel[channel].samples_available + EXTRA_SAMPLES) * freq / Machine.sample_rate;
        }

        public void streams_sh_update()
        {
            if (Machine.sample_rate == 0) return;

            /* update all the output buffers */
            for (int channel = 0; channel < MIXER_MAX_CHANNELS; channel += stream_joined_channels[channel])
            {
                if (stream_buffer[channel] != null)
                {
                    int newpos = mixer_need_samples_this_frame(channel, stream_sample_rate[(channel)]);

                    int buflen = newpos - stream_buffer_pos[channel];

                    if (stream_joined_channels[channel] > 1)
                    {
                        _ShortPtr[] buf = new _ShortPtr[MIXER_MAX_CHANNELS];

                        if (buflen > 0)
                        {
                            for (int i = 0; i < stream_joined_channels[channel]; i++)
                                buf[i] = new _ShortPtr(stream_buffer[channel + i], stream_buffer_pos[channel + i]*sizeof(short));

                            stream_callback_multi[channel](stream_param[channel], buf, buflen);
                        }

                        for (int i = 0; i < stream_joined_channels[channel]; i++)
                            stream_buffer_pos[channel + i] = 0;

                        for (int i = 0; i < stream_joined_channels[channel]; i++)
                            apply_RC_filter(channel + i, stream_buffer[channel + i], buflen, stream_sample_rate[channel + i]);
                    }
                    else
                    {
                        if (buflen > 0)
                        {
                            _ShortPtr buf= new _ShortPtr(stream_buffer[channel], stream_buffer_pos[channel]*sizeof(short));

                            stream_callback[channel](stream_param[channel], buf, buflen);
                        }

                        stream_buffer_pos[channel] = 0;

                        apply_RC_filter(channel, stream_buffer[channel], buflen, stream_sample_rate[channel]);
                    }
                }
            }

            for (int channel = 0; channel < MIXER_MAX_CHANNELS; channel += stream_joined_channels[channel])
            {
                if (stream_buffer[channel] != null)
                {
                    for (int i = 0; i < stream_joined_channels[channel]; i++)
                        mixer_play_streamed_sample_16(channel + i,
                                stream_buffer[channel + i], sizeof(short) *
                                mixer_need_samples_this_frame((channel + i), stream_sample_rate[(channel + i)]),
                                stream_sample_rate[channel]);
                }
            }
        }
        /*
signal >--R1--+--R2--+
              |      |
              C      R3---> amp
              |      |
             GND    GND
*/

        /* R1, R2, R3 in Ohm; C in pF */
        /* set C = 0 to disable the filter */
        public static void set_RC_filter(int channel, int R1, int R2, int R3, int C)
        {
            r1[channel] = R1;
            r2[channel] = R2;
            r3[channel] = R3;
            c[channel] = C;
        }
        void apply_RC_filter(int channel, _ShortPtr buf, int len, int sample_rate)
        {
            if (c[channel] == 0) return;	/* filter disabled */

            float R1 = r1[channel];
            float R2 = r2[channel];
            float R3 = r3[channel];
            float C = (float)(c[channel] * 1E-12);	/* convert pF to F */

            /* Cut Frequency = 1/(2*Pi*Req*C) */

            float Req = (R1 * (R2 + R3)) / (R1 + R2 + R3);

            int K = (int)(0x10000 * Math.Exp(-1 / (Req * C) / sample_rate));

            buf.write16(0, (ushort)(buf.read16(0) + (memory[channel] - buf.read16(0)) * K / 0x10000));

            for (int i = 1; i < len; i++)
                buf.write16(i, (ushort)(buf.read16(i) + (buf.read16(i - 1) - buf.read16(i)) * K / 0x10000));

            memory[channel] = buf.read16(len - 1);
        }

        public static void stream_update(int channel, int min_interval)
        {
            if (Machine.sample_rate == 0 || stream_buffer[channel] == null)
                return;

            /* get current position based on the timer */
            int newpos = sound_scalebufferpos(mixer_need_samples_this_frame(channel, stream_sample_rate[channel]));//SAMPLES_THIS_FRAME(channel));

            int buflen = newpos - stream_buffer_pos[channel];

            if (buflen * stream_sample_length[channel] > min_interval)
            {
                if (stream_joined_channels[channel] > 1)
                {
                    _ShortPtr[] buf = new _ShortPtr[MIXER_MAX_CHANNELS];

                    for (int i = 0; i < stream_joined_channels[channel]; i++)
                        buf[i] = new _ShortPtr(stream_buffer[channel + i], stream_buffer_pos[channel + i] * sizeof(short));

                    stream_callback_multi[channel](stream_param[channel], buf, buflen);

                    for (int i = 0; i < stream_joined_channels[channel]; i++)
                        stream_buffer_pos[channel + i] += buflen;
                }
                else
                {
                    _ShortPtr buf = new _ShortPtr(stream_buffer[channel], stream_buffer_pos[channel] * sizeof(short));

                    stream_callback[channel](stream_param[channel], buf, buflen);

                    stream_buffer_pos[channel] += buflen;
                }
            }
        }
    }
}
