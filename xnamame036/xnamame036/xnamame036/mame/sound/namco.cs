using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class Namco_interface
    {
        public Namco_interface(int samplerate, int voices, int volume, int region, bool stereo = false)
        {
            this.samplerate = samplerate; this.voices = voices; this.volume = volume; this.region = region;
            this.stereo = stereo;
        }
        public int samplerate, voices, volume, region;
        public bool stereo;
    }
    public class Namco : Mame.snd_interface
    {
        public Namco()
        {
            this.name = "Namco";
            this.sound_num = Mame.SOUND_NAMCO;
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((Namco_interface)msound.sound_interface).samplerate;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return 1;
        }

        public override void stop()
        {
            mixer_buffer = null;
        }
        public override void reset()
        {
            //none
        }
        public override void update()
        {
            //none
        }
        /* 8 voices max */
        const int MAX_VOICES = 8;

        /* this structure defines the parameters for a channel */
        class sound_channel
        {
            public int frequency;
            public int counter;
            public int[] volume = new int[2];
            public int noise_sw;
            public int noise_state;
            public int noise_seed;
            public int noise_counter;
            public _BytePtr wave;
        }

        /* globals available to everyone */
        public static _BytePtr namco_soundregs = new _BytePtr(1);
        public static _BytePtr namco_wavedata=new _BytePtr(1);

        /* data about the sound system */
        static sound_channel[] channel_list = new sound_channel[MAX_VOICES];
        static int last_channel;

        /* global sound parameters */
        static _BytePtr sound_prom;
        static int samples_per_byte;
        static int num_voices;
        static bool sound_enable;
        static int stream;
        static int namco_clock;
        static int sample_rate;

        /* mixer tables and internal buffers */
        
        static short[] mixer_lookup;
        static _ShortPtr mixer_buffer;
        static _ShortPtr mixer_buffer_2;

        public static void pengo_sound_enable_w(int offset, int data)
        {
            sound_enable = data!=0;
        }
        public static void pengo_sound_w(int offset, int data)
        {
            int voice;
            int _base;

            /* update the streams */
            Mame.stream_update(stream, 0);

            /* set the register */
            namco_soundregs[offset] = (byte)(data & 0x0f);

            /* recompute all the voice parameters */
            for (_base = 0, voice = 0; voice < last_channel; voice++, _base += 5)
            {
                channel_list[voice].frequency = namco_soundregs[0x14 + _base];	/* always 0 */
                channel_list[voice].frequency = channel_list[voice].frequency * 16 + namco_soundregs[0x13 + _base];
                channel_list[voice].frequency = channel_list[voice].frequency * 16 + namco_soundregs[0x12 + _base];
                channel_list[voice].frequency = channel_list[voice].frequency * 16 + namco_soundregs[0x11 + _base];
                if (_base == 0)	/* the first voice has extra frequency bits */
                    channel_list[voice].frequency = channel_list[voice].frequency * 16 + namco_soundregs[0x10 + _base];
                else
                    channel_list[voice].frequency = channel_list[voice].frequency * 16;

                channel_list[voice].volume[0] = namco_soundregs[0x15 + _base] & 0x0f;
                channel_list[voice].wave = new _BytePtr(sound_prom, 32 * (namco_soundregs[0x05 + _base] & 7));
            }
        }        
        static int mixer_lookup_middle;
        void make_mixer_table(int voices)
        {
            int count = voices * 128;
            int gain = 16;

            /* allocate memory */
            //mixer_table = new _ShortPtr(256 * voices * sizeof(short));

            /* find the middle of the table */
            mixer_lookup = new short[256 * voices];
            mixer_lookup_middle = voices * 128;
            /* fill in the table - 16 bit case */
            for (int i = 0; i < count; i++)
            {
                short val = (short)(i * gain * 16 / voices);
                if (val > 32767) val = 32767;
                mixer_lookup[mixer_lookup_middle + i] = val;
                mixer_lookup[mixer_lookup_middle -i] = (short)-val;
            }
        }

        public override int start(xnamame036.mame.Mame.MachineSound msound)
        {
            string mono_name = "NAMCO sound";
            string[] stereo_names = { "NAMCO sound left", "NAMCO sound right" };

            Namco_interface intf = (Namco_interface)msound.sound_interface;

            namco_clock = intf.samplerate;
            sample_rate = Mame.Machine.sample_rate;

            /* get stream channels */
            if (intf.stereo)
            {
                int[] vol = new int[2];

                vol[1] = Mame.MIXER(intf.volume, Mame.MIXER_PAN_RIGHT);
                vol[0] = Mame.MIXER(intf.volume, Mame.MIXER_PAN_LEFT);
                stream = Mame.stream_init_multi(2, stereo_names, vol, intf.samplerate, 0, namco_update_stereo);
            }
            else
            {
                stream = Mame.stream_init(mono_name, intf.volume, intf.samplerate, 0, namco_update_mono);
            }

            /* allocate a pair of buffers to mix into - 1 second's worth should be more than enough */
            mixer_buffer = new _ShortPtr(2 * intf.samplerate * sizeof(short) );
            mixer_buffer_2 = new _ShortPtr(mixer_buffer, intf.samplerate * sizeof(short));

            /* build the mixer table */
            make_mixer_table(intf.voices);

            /* extract globals from the interface */
            num_voices = intf.voices;
            last_channel = num_voices;

            if (intf.region == -1)
            {
                sound_prom = namco_wavedata;
                samples_per_byte = 2;	/* first 4 high bits, then low 4 bits */
            }
            else
            {
                sound_prom = Mame.memory_region(intf.region);
                samples_per_byte = 1;	/* use only low 4 bits */
            }

            /* start with sound enabled, many games don't have a sound enable register */
            sound_enable = true;

            /* reset all the voices */
            for (int i = 0; i < last_channel; i++)
            //for (voice = channel_list; voice < last_channel; voice++)
            {
                channel_list[i] = new sound_channel();
                channel_list[i].frequency = 0;
                channel_list[i].volume[0] = channel_list[i].volume[1] = 0;
                channel_list[i].wave = sound_prom;
                channel_list[i].counter = 0;
                channel_list[i].noise_sw = 0;
                channel_list[i].noise_state = 0;
                channel_list[i].noise_seed = 1;
                channel_list[i].noise_counter = 0;
            }

            return 0;
        }
        void namco_update_mono(int ch, _ShortPtr buffer, int length)
        {
            _ShortPtr mix;

            /* if no sound, we're done */
            if (!sound_enable)
            {
                Array.Clear(buffer.buffer, buffer.offset, length*2);
                return;
            }

            /* zap the contents of the mixer buffer */
            Array.Clear(mixer_buffer.buffer, mixer_buffer.offset, length * 2);

            /* loop over each voice and add its contribution */
            
            for (int voice=0;voice<last_channel;voice++)//; voice < last_channel; voice++)
            {
                int f = channel_list[voice].frequency;
                int v = channel_list[voice].volume[0];

                mix = new _ShortPtr(mixer_buffer);

                if (channel_list[voice].noise_sw != 0)
                {
                    /* only update if we have non-zero volume and frequency */
                    if (v != 0 && (f & 0xff) != 0)
                    {
                        float fbase = (float)sample_rate / (float)namco_clock;
                        int delta = (int)((float)((f & 0xff) << 4) * fbase);
                        int c = channel_list[voice].noise_counter;

                        /* add our contribution */
                        for (int i = 0; i < length; i++)
                        {
                            int noise_data;
                            int cnt;

                            if (channel_list[voice].noise_state != 0)
                                noise_data = 0x07;
                            else
                                noise_data = -0x07;
                            mix.write16(0, (ushort)((short)mix.read16(0) + noise_data * (v >> 1)));
                            mix.offset += 2;

                            c += delta;
                            cnt = (c >> 12);
                            c &= (1 << 12) - 1;
                            for (; cnt > 0; cnt--)
                            {
                                if (((channel_list[voice].noise_seed + 1) & 2) != 0) channel_list[voice].noise_state ^= 1;
                                if ((channel_list[voice].noise_seed & 1) != 0) channel_list[voice].noise_seed ^= 0x28000;
                                channel_list[voice].noise_seed >>= 1;
                            }
                        }

                        /* update the counter for this voice */
                        channel_list[voice].noise_counter = c;
                    }
                }
                else
                {
                    /* only update if we have non-zero volume and frequency */
                    if (v != 0 && f != 0)
                    {                        
                        int c = channel_list[voice].counter;

                        /* add our contribution */
                        for (int i = 0; i < length; i++)
                        {
                            c += f;
                            int offs = (c >> 15) & 0x1f;
                            //ushort currentmix = mix.read16(0);
                            if (samples_per_byte == 1)	/* use only low 4 bits */
                            {
                                mix.write16(0, (ushort)((short)mix.read16(0) + ((channel_list[voice].wave[offs] & 0x0f) - 8) * v));
                                mix.offset += 2;
                            }
                            else	/* use full byte, first the high 4 bits, then the low 4 bits */
                            {
                                if ((offs & 1) != 0)
                                {
                                    mix.write16(0, (ushort)((short)mix.read16(0) + ((channel_list[voice].wave[offs >> 1] & 0x0f) - 8) * v));
                                    mix.offset += 2;
                                }
                                else
                                {
                                    mix.write16(0, (ushort)((short)mix.read16(0) + (((channel_list[voice].wave[offs >> 1] >> 4) & 0x0f) - 8) * v));
                                    mix.offset += 2;
                                }
                            }
                        }

                        /* update the counter for this voice */
                        channel_list[voice].counter = c;
                    }
                }
            }

            /* mix it down */
            mix = new _ShortPtr(mixer_buffer);
            for (int i = 0; i < length; i++)
            {
                buffer.write16(0, (ushort)mixer_lookup[mixer_lookup_middle+(short)mix.read16(0)]);
                buffer.offset += 2;
                mix.offset += 2;
            }
        }
        void namco_update_stereo(int ch, _ShortPtr[] buffer, int length)
        {
            throw new Exception();
        }
        public static void mappy_sound_w(int offset, int data)
        {
            int voice;
            int _base;

            /* update the streams */
            Mame.stream_update(stream, 0);

            /* set the register */
            namco_soundregs[offset] = (byte)data;

            /* recompute all the voice parameters */
            for (_base = 0, voice = 0; voice < last_channel; voice++, _base += 8)
            {
                channel_list[voice].frequency = namco_soundregs[0x06 + _base] & 15;	/* high bits are from here */
                channel_list[voice].frequency = channel_list[voice].frequency * 256 + namco_soundregs[0x05 + _base];
                channel_list[voice].frequency = channel_list[voice].frequency * 256 + namco_soundregs[0x04 + _base];

                channel_list[voice].volume[0] = namco_soundregs[0x03 + _base] & 0x0f;
                channel_list[voice].wave = new _BytePtr(sound_prom, 32 * ((namco_soundregs[0x06 + _base] >> 4) & 7));
            }
        }
        public static void mappy_sound_enable_w(int offset, int data)
        {
            sound_enable = offset!=0;
        }

        static int nssw;
        public static void namcos1_sound_w(int offset, int data)
        {
            sound_channel voice;
            int _base;

            /* verify the offset */
            if (offset > 63)
            {
                Mame.printf("NAMCOS1 sound: Attempting to write past the 64 registers segment\n");
                return;
            }

            /* update the streams */
            Mame.stream_update(stream, 0);

            /* set the register */
            namco_soundregs[offset] = (byte)data;

            /* recompute all the voice parameters */
            int vi = 0;
            for (_base = 0; vi < last_channel; vi++, _base += 8)
            {
                voice = channel_list[vi];
                voice.frequency = namco_soundregs[0x01 + _base] & 15;	/* high bits are from here */
                voice.frequency = voice.frequency * 256 + namco_soundregs[0x02 + _base];
                voice.frequency = voice.frequency * 256 + namco_soundregs[0x03 + _base];

                voice.volume[0] = namco_soundregs[0x00 + _base] & 0x0f;
                voice.volume[1] = namco_soundregs[0x04 + _base] & 0x0f;
                voice.wave = new _BytePtr(sound_prom, 32 / samples_per_byte * ((namco_soundregs[0x01 + _base] >> 4) & 15));

                nssw = ((namco_soundregs[0x04 + _base] & 0x80) >> 7);
                if ((vi + 1) < last_channel) channel_list[vi + 1].noise_sw = nssw;
            }
            //voice = 0;
            channel_list[0].noise_sw = nssw;
        }
        public static void namcos1_wavedata_w(int offset, int data)
        {
            /* update the streams */
            Mame.stream_update(stream, 0);

            namco_wavedata[offset] = (byte)data;
        }
        public static int namcos1_wavedata_r(int offset)
        {
            return namco_wavedata[offset];
        }

        public static int namcos1_sound_r(int offset)
        {
            return namco_soundregs[offset];
        }




    }
}

