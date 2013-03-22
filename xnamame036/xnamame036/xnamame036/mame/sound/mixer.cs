using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        static bool mixer_sound_enabled;

        public const int MIXER_PAN_CENTER = 0, MIXER_PAN_LEFT = 1, MIXER_PAN_RIGHT = 2;
        public const int MIXER_MAX_CHANNELS = 16;
        public const bool DISABLE_CLIPPING = false;
        public const int ACCUMULATOR_SAMPLES = 8192;
        public const int ACCUMULATOR_MASK = ACCUMULATOR_SAMPLES - 1;
        public const int FRACTION_BITS = 16;
        public const int FRACTION_MASK = ((1 << FRACTION_BITS) - 1);

        public const int MIXER_GAIN_1x = 0, MIXER_GAIN_2x = 1, MIXER_GAIN_4x = 2, MIXER_GAIN_8x = 3;
        public static int MIXERG(int level, int gain, int pan) { return ((level & 0xff) | ((gain & 0x03) << 10) | ((pan & 0x03) << 8)); }
        public static int MIXER(int level, int pan)
        {
            return ((level & 0xff) | ((pan & 0x03) << 8));
        }
        struct mixer_channel_data
        {
            public string name;
            public int volume;
            public int gain;
            public int pan;

            /* mixing levels */
            public byte mixing_level;
            public byte default_mixing_level;
            public byte config_mixing_level;
            public byte config_default_mixing_level;

            /* current playback positions */
            public uint input_frac;
            public uint samples_available;
            public uint frequency;
            public uint step_size;

            /* state of non-streamed playback */
            public bool is_stream;
            public bool is_playing;
            public bool is_looping;
            public bool is_16bit;
            public _BytePtr data_start;
            public int data_end;
            public int data_current;
        }

        /* channel data */
        static mixer_channel_data[] mixer_channel = new mixer_channel_data[MIXER_MAX_CHANNELS];
        static byte[] config_mixing_level = new byte[MIXER_MAX_CHANNELS];
        static byte[] config_default_mixing_level = new byte[MIXER_MAX_CHANNELS];
        static byte first_free_channel = 0;
        static byte config_invalid;
        static bool is_stereo;

        /* 32-bit accumulators */
        static uint accum_base;
        static int[] left_accum = new int[ACCUMULATOR_SAMPLES];
        static int[] right_accum = new int[ACCUMULATOR_SAMPLES];

        /* 16-bit mix buffers */
        static short[] mix_buffer = new short[ACCUMULATOR_SAMPLES * 2];	/* *2 for stereo */

        /* global sample tracking */
        static uint samples_this_frame;

        static int MIXER_GET_GAIN(int mixing_level) { return (((mixing_level) >> 10) & 0x03); }
        static int MIXER_GET_PAN(int mixing_level) { return (((mixing_level) >> 8) & 0x03); }
        static int MIXER_GET_LEVEL(int mixing_level) { return ((mixing_level) & 0xff); }

        int mixer_sh_start()
        {
            /* reset all channels to their defaults */
            for (int i = 0; i < mixer_channel.Length; i++)
            {
                mixer_channel[i] = new mixer_channel_data();
                mixer_channel[i].mixing_level = 0xff;
                mixer_channel[i].default_mixing_level = 0xff;
                mixer_channel[i].config_mixing_level = config_mixing_level[i];
                mixer_channel[i].config_default_mixing_level = config_default_mixing_level[i];
            }

            /* determine if we're playing in stereo or not */
            first_free_channel = 0;
            is_stereo = ((Machine.drv.sound_attributes & SOUND_SUPPORTS_STEREO) != 0);

            /* clear the accumulators */
            accum_base = 0;
            for (int i = 0; i < ACCUMULATOR_SAMPLES; i++)
            {
                left_accum[i] = 0;
                right_accum[i] = 0;
            }

            samples_this_frame = (uint)osd_start_audio_stream(is_stereo);

            mixer_sound_enabled = true;

            return 0;
        }
        void mixer_sh_stop()
        {
            osd_stop_audio_stream();
        }
        string mixer_get_name(int ch)
        {
            return mixer_channel[ch].name;
        }
        static void mixer_update_channel(mixer_channel_data channel, int total_sample_count)
        {
            int samples_to_generate = (int)(total_sample_count - channel.samples_available);

            /* don't do anything for streaming channels */
            if (channel.is_stream)
                return;

            /* if we're all caught up, just return */
            if (samples_to_generate <= 0)
                return;

            /* if we're playing, mix in the data */
            if (channel.is_playing)
            {
                if (channel.is_16bit)
                    mix_sample_16(channel, samples_to_generate);
                else
                    mix_sample_8(channel, samples_to_generate);
            }

            /* just eat the rest */
            channel.samples_available += (uint)samples_to_generate;
        }
        static void mix_sample_8(mixer_channel_data channel, int samples_to_generate)
        {
            uint step_size, input_frac, output_pos;
            _BytePtr source;
            int source_end;
            int mixing_volume;

            /* compute the overall mixing volume */
            if (mixer_sound_enabled)
                mixing_volume = ((channel.volume * channel.mixing_level * 256) << channel.gain) / (100 * 100);
            else
                mixing_volume = 0;

            /* get the initial state */
            step_size = channel.step_size;
            source = new _BytePtr(channel.data_start,channel.data_current);
            source_end = channel.data_end;
            input_frac = channel.input_frac;
            output_pos = (accum_base + channel.samples_available) & ACCUMULATOR_MASK;

            /* an outer loop to handle looping samples */
            while (samples_to_generate > 0)
            {
                /* if we're mono or left panning, just mix to the left channel */
                if (!is_stereo || channel.pan == MIXER_PAN_LEFT)
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        left_accum[output_pos] += (sbyte)source[0] * mixing_volume;
                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS);
                        input_frac &= FRACTION_MASK;
                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* if we're right panning, just mix to the right channel */
                else if (channel.pan == MIXER_PAN_RIGHT)
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        right_accum[output_pos] += (sbyte)source[0] * mixing_volume;
                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS);
                        input_frac &= FRACTION_MASK;
                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* if we're stereo center, mix to both channels */
                else
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        int mixing_value = (sbyte)source[0] * mixing_volume;
                        left_accum[output_pos] += mixing_value;
                        right_accum[output_pos] += mixing_value;
                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS);
                        input_frac &= FRACTION_MASK;
                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* handle the end case */
                if (source.offset >= source_end)
                {
                    /* if we're done, stop playing */
                    if (!channel.is_looping)
                    {
                        channel.is_playing = false;
                        break;
                    }

                    /* if we're looping, wrap to the beginning */
                    else
                        source.offset -= source_end;// -(INT8*)channel.data_start;
                }
            }

            /* update the final positions */
            channel.input_frac = input_frac;
            channel.data_current = source.offset;
        }
        static void mix_sample_16(mixer_channel_data channel, int samples_to_generate)
        {
            uint step_size, input_frac, output_pos;
            _ShortPtr source;
            int source_end;
            int mixing_volume;

            /* compute the overall mixing volume */
            if (mixer_sound_enabled)
                mixing_volume = ((channel.volume * channel.mixing_level * 256) << channel.gain) / (100 * 100);
            else
                mixing_volume = 0;

            /* get the initial state */
            step_size = channel.step_size;
            source = new _ShortPtr(channel.data_current);
            source_end = channel.data_end;
            input_frac = channel.input_frac;
            output_pos = (accum_base + channel.samples_available) & ACCUMULATOR_MASK;

            /* an outer loop to handle looping samples */
            while (samples_to_generate > 0)
            {
                /* if we're mono or left panning, just mix to the left channel */
                if (!is_stereo || channel.pan == MIXER_PAN_LEFT)
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        left_accum[output_pos] += (source.read16(0) * mixing_volume) >> 8;

                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS) * 2;
                        input_frac &= FRACTION_MASK;

                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* if we're right panning, just mix to the right channel */
                else if (channel.pan == MIXER_PAN_RIGHT)
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        right_accum[output_pos] += (source.read16(0) * mixing_volume) >> 8;

                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS) * 2;
                        input_frac &= FRACTION_MASK;

                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* if we're stereo center, mix to both channels */
                else
                {
                    while (source.offset < source_end && samples_to_generate > 0)
                    {
                        int mixing_value = (source.read16(0) * mixing_volume) >> 8;
                        left_accum[output_pos] += mixing_value;
                        right_accum[output_pos] += mixing_value;

                        input_frac += step_size;
                        source.offset += (int)(input_frac >> FRACTION_BITS) * 2;
                        input_frac &= FRACTION_MASK;

                        output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                        samples_to_generate--;
                    }
                }

                /* handle the end case */
                if (source.offset >= source_end)
                {
                    /* if we're done, stop playing */
                    if (!channel.is_looping)
                    {
                        channel.is_playing = false;
                        break;
                    }

                    /* if we're looping, wrap to the beginning */
                    else
                        source.offset-=source_end;// source.offset -= (INT16*)source_end - (INT16*)channel.data_start;
                }
            }

            /* update the final positions */
            channel.input_frac = input_frac;
            channel.data_current = source.offset;
        }
        void mixer_sh_update()
        {
            uint accum_pos = accum_base;

            int sample;

            /* update all channels (for streams this is a no-op) */
            for (int i = 0; i < first_free_channel; i++)
            {
                mixer_update_channel(mixer_channel[i], (int)samples_this_frame);

                /* if we needed more than they could give, adjust their pointers */
                if (samples_this_frame > mixer_channel[i].samples_available)
                    mixer_channel[i].samples_available = 0;
                else
                    mixer_channel[i].samples_available -= samples_this_frame;
            }

            /* copy the mono 32-bit data to a 16-bit buffer, clipping along the way */
            if (!is_stereo)
            {
                int mix = 0;
                for (int i = 0; i < samples_this_frame; i++)
                {
                    /* fetch and clip the sample */
                    sample = left_accum[accum_pos];
#if !DISABLE_CLIPPING
                    if (sample < -32768)
                        sample = -32768;
                    else if (sample > 32767)
                        sample = 32767;
#endif

                    /* store and zero out behind us */
                    mix_buffer[mix++] = (short)sample;
                    left_accum[accum_pos] = 0;

                    /* advance to the next sample */
                    accum_pos = (accum_pos + 1) & ACCUMULATOR_MASK;
                }
            }

            /* copy the stereo 32-bit data to a 16-bit buffer, clipping along the way */
            else
            {
                int mix = 0;// mix_buffer;
                for (int i = 0; i < samples_this_frame; i++)
                {
                    /* fetch and clip the left sample */
                    sample = left_accum[accum_pos];
                    if (!DISABLE_CLIPPING)
                    {
                        if (sample < -32768)
                            sample = -32768;
                        else if (sample > 32767)
                            sample = 32767;
                    }

                    /* store and zero out behind us */
                    mix_buffer[mix++] = (short)sample;
                    left_accum[accum_pos] = 0;

                    /* fetch and clip the right sample */
                    sample = right_accum[accum_pos];
                    if (!DISABLE_CLIPPING)
                    {
                        if (sample < -32768)
                            sample = -32768;
                        else if (sample > 32767)
                            sample = 32767;
                    }

                    /* store and zero out behind us */
                    mix_buffer[mix++] = (short)sample;
                    right_accum[accum_pos] = 0;

                    /* advance to the next sample */
                    accum_pos = (accum_pos + 1) & ACCUMULATOR_MASK;
                }
            }

            /* play the result */
            samples_this_frame = (uint)osd_update_audio_stream(mix_buffer);

            accum_base = accum_pos;
        }
        public static void mixer_play_streamed_sample_16(int ch, _ShortPtr data, int len, int freq)
        {

            uint step_size, input_pos, output_pos, samples_mixed;
            int mixing_volume;

            /* skip if sound is off */
            if (Machine.sample_rate == 0)
                return;
            mixer_channel[ch].is_stream = true;



            /* compute the overall mixing volume */
            if (mixer_sound_enabled)
                mixing_volume = ((mixer_channel[ch].volume * mixer_channel[ch].mixing_level * 256) << mixer_channel[ch].gain) / (100 * 100);
            else
                mixing_volume = 0;

            /* compute the step size for sample rate conversion */
            if (freq != mixer_channel[ch].frequency)
            {
                mixer_channel[ch].frequency = (uint)freq;
                mixer_channel[ch].step_size = (uint)((double)freq * (double)(1 << FRACTION_BITS) / (double)Machine.sample_rate);
            }
            step_size = mixer_channel[ch].step_size;

            /* now determine where to mix it */
            input_pos = mixer_channel[ch].input_frac;
            output_pos = (accum_base + mixer_channel[ch].samples_available) & ACCUMULATOR_MASK;

            /* compute the length in fractional form */
            len = (len / 2) << FRACTION_BITS;
            samples_mixed = 0;

            /* if we're mono or left panning, just mix to the left channel */
            if (!is_stereo || mixer_channel[ch].pan == MIXER_PAN_LEFT)
            {
                while (input_pos < len)
                {
                    left_accum[output_pos] += ((short)data.read16((int)(input_pos >> FRACTION_BITS)) * mixing_volume) >> 8;
                    input_pos += step_size;
                    output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                    samples_mixed++;
                }
            }

            /* if we're right panning, just mix to the right channel */
            else if (mixer_channel[ch].pan == MIXER_PAN_RIGHT)
            {
                while (input_pos < len)
                {
                    right_accum[output_pos] += ((short)data.read16((int)(input_pos >> FRACTION_BITS)) * mixing_volume) >> 8;
                    input_pos += step_size;
                    output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                    samples_mixed++;
                }
            }

            /* if we're stereo center, mix to both channels */
            else
            {
                while (input_pos < len)
                {
                    int mixing_value = ((short)data.read16((int)(input_pos >> FRACTION_BITS)) * mixing_volume) >> 8;
                    left_accum[output_pos] += mixing_value;
                    right_accum[output_pos] += mixing_value;
                    input_pos += step_size;
                    output_pos = (output_pos + 1) & ACCUMULATOR_MASK;
                    samples_mixed++;
                }
            }

            /* update the final positions */
            mixer_channel[ch].input_frac = input_pos & FRACTION_MASK;
            mixer_channel[ch].samples_available += samples_mixed;
        }
        public static int mixer_allocate_channel(int default_mixing_level) { return mixer_allocate_channels(1, new int[] { default_mixing_level }); }
        public static int mixer_allocate_channels(int channels, int[] default_mixing_levels)
        {
            /* make sure we didn't overrun the number of available channels */
            if (first_free_channel + channels > MIXER_MAX_CHANNELS)
            {
                throw new Exception(sprintf("Too many mixer channels (requested %d, available %d)\n", first_free_channel + channels, MIXER_MAX_CHANNELS));
            }

            /* loop over channels requested */
            for (int i = 0; i < channels; i++)
            {
                /* extract the basic data */
                mixer_channel[first_free_channel + i].default_mixing_level = (byte)MIXER_GET_LEVEL(default_mixing_levels[i]);
                mixer_channel[first_free_channel + i].pan = MIXER_GET_PAN(default_mixing_levels[i]);
                mixer_channel[first_free_channel + i].gain = MIXER_GET_GAIN(default_mixing_levels[i]);
                mixer_channel[first_free_channel + i].volume = 100;

                /* backwards compatibility with old 0-255 volume range */
                if (mixer_channel[first_free_channel + i].default_mixing_level > 100)
                    mixer_channel[first_free_channel + i].default_mixing_level = (byte)(mixer_channel[first_free_channel + i].default_mixing_level * 25 / 255);

                /* attempt to load in the configuration data for this channel */
                mixer_channel[first_free_channel + i].mixing_level = mixer_channel[first_free_channel + i].default_mixing_level;
                if (config_invalid == 0)
                {
                    /* if the defaults match, set the mixing level from the config */
                    if (mixer_channel[first_free_channel + i].default_mixing_level == mixer_channel[first_free_channel + i].config_default_mixing_level)
                        mixer_channel[first_free_channel + i].mixing_level = mixer_channel[first_free_channel + i].config_mixing_level;

                    /* otherwise, invalidate all channels that have been created so far */
                    else
                    {
                        config_invalid = 1;
                        for (int j = 0; j < first_free_channel + i; j++)
                            mixer_set_mixing_level(j, mixer_channel[j].default_mixing_level);
                    }
                }

                /* set the default name */
                mixer_set_name(first_free_channel + i, null);
            }

            /* increment the counter and return the first one */
            first_free_channel += (byte)channels;
            return first_free_channel - channels;
        }
        public static void mixer_set_name(int ch, string name)
        {
            /* either copy the name or create a default one */
            if (name != null)
                mixer_channel[ch].name = name;
            else
                mixer_channel[ch].name = sprintf("<channel #%d>", ch);

            /* append left/right onto the channel as appropriate */
            if (mixer_channel[ch].pan == MIXER_PAN_LEFT)
                mixer_channel[ch].name += " (Lt)";
            else if (mixer_channel[ch].pan == MIXER_PAN_RIGHT)
                mixer_channel[ch].name += " (Rt)";
        }
        public static void mixer_set_mixing_level(int ch, int level)
        {
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));
            mixer_channel[ch].mixing_level = (byte)level;
        }

        public static void mixer_set_volume(int ch, int volume)
        {
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));
            mixer_channel[ch].volume = volume;
        }
        public static void mixer_play_sample(int ch, byte[] data, int len, int freq, bool loop)
        {

            /* skip if sound is off, or if this channel is a stream */
            if (Machine.sample_rate == 0 || mixer_channel[ch].is_stream)
                return;

            /* update the state of this channel */
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));

            /* compute the step size for sample rate conversion */
            if (freq != mixer_channel[ch].frequency)
            {
                mixer_channel[ch].frequency = (uint)freq;
                mixer_channel[ch].step_size = (uint)((double)freq * (double)(1 << FRACTION_BITS) / (double)Machine.sample_rate);
            }

            /* now determine where to mix it */
            mixer_channel[ch].input_frac = 0;
            mixer_channel[ch].data_start =new _BytePtr ( data);
            mixer_channel[ch].data_current = 0;
            //Buffer.BlockCopy(data, 0, mixer_channel[ch].data_current.buffer, 0, data.Length);
            mixer_channel[ch].data_end = len;
            mixer_channel[ch].is_playing = true;
            mixer_channel[ch].is_looping = loop;
            mixer_channel[ch].is_16bit = false;
        }
        public static void mixer_play_sample_16(int ch,_ShortPtr data, int len, int freq, bool loop)
        {
            /* skip if sound is off, or if this channel is a stream */
            if (Machine.sample_rate == 0 || mixer_channel[ch].is_stream)
                return;

            /* update the state of this channel */
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));

            /* compute the step size for sample rate conversion */
            if (freq != mixer_channel[ch].frequency)
            {
                mixer_channel[ch].frequency = (uint)freq;
                mixer_channel[ch].step_size = (uint)((double)freq * (double)(1 << FRACTION_BITS) / (double)Machine.sample_rate);
            }

            /* now determine where to mix it */
            mixer_channel[ch].input_frac = 0;
            mixer_channel[ch].data_start = data;
            mixer_channel[ch].data_current = 0;// new _BytePtr(data.Length * 2);
            //for (int i = 0; i < data.Length; i++)mixer_channel[ch].data_current.write16(i,(ushort) data[i]);
            //Buffer.BlockCopy(data, 0, mixer_channel[ch].data_current.buffer, 0, data.Length * 2);
            mixer_channel[ch].data_end = len;
            mixer_channel[ch].is_playing = true;
            mixer_channel[ch].is_looping = loop;
            mixer_channel[ch].is_16bit = true;
        }
        static public bool mixer_is_sample_playing(int ch)
        {
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));
            return mixer_channel[ch].is_playing;
        }

        public static void mixer_stop_sample(int ch)
        {
            mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));
            mixer_channel[ch].is_playing = false;
        }
        void mixer_write_config(object f)
        {
            byte[] default_levels = new byte[MIXER_MAX_CHANNELS];
            byte[] mixing_levels = new byte[MIXER_MAX_CHANNELS];

            for (int i = 0; i < MIXER_MAX_CHANNELS; i++)
            {
                default_levels[i] = mixer_channel[i].default_mixing_level;
                mixing_levels[i] = mixer_channel[i].mixing_level;
            }
            osd_fwrite(f, default_levels, MIXER_MAX_CHANNELS);
            osd_fwrite(f, mixing_levels, MIXER_MAX_CHANNELS);
        }
        void mixer_read_config(object f)
        {
            byte[] default_levels = new byte[MIXER_MAX_CHANNELS];
            byte[] mixing_levels = new byte[MIXER_MAX_CHANNELS];

            for (int i = 0; i < default_levels.Length; i++) default_levels[i] = 0xff;
            for (int i = 0; i < mixing_levels.Length; i++) mixing_levels[i] = 0xff;

            osd_fread(f, default_levels, MIXER_MAX_CHANNELS);
            osd_fread(f, mixing_levels, MIXER_MAX_CHANNELS);
            for (int i = 0; i < MIXER_MAX_CHANNELS; i++)
            {
                config_default_mixing_level[i] = default_levels[i];
                config_mixing_level[i] = mixing_levels[i];
            }
            config_invalid = 0;
        }

        public static void mixer_set_sample_frequency(int ch, int freq)
{

	mixer_update_channel(mixer_channel[ch], sound_scalebufferpos((int)samples_this_frame));

	/* compute the step size for sample rate conversion */
    if (freq != mixer_channel[ch].frequency)
	{
		mixer_channel[ch].frequency =(uint) freq;
		mixer_channel[ch].step_size = (uint)((double)freq * (double)(1 << FRACTION_BITS) / (double)Machine.sample_rate);
	}
}
    }
}
