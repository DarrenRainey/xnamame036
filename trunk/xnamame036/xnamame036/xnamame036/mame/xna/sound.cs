//#define UseCallbackEventHandler
//#define EventSubmitTwoBuffers

//#define SubmitAudioAsNeeded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace xnamame036.mame
{
    partial class Mame
    {
        static int master_volume = 256;

        static int stream_playing;
        static short[] stream_cache_data;
        static int stream_cache_len;
        static bool stream_cache_stereo;

        const int NUM_BUFFERS = 3;	/* raising this number should improve performance with frameskip, */
        /* but also increases the latency. */

        static int voice_pos;
        static int audio_buffer_length;

        /* global sample tracking */
        static double samples_per_frame;
        static double samples_left_over;
        static uint snd_samples_this_frame;

        private DynamicSoundEffectInstance soundInstance;
        const int MAX_BUFFER_SIZE = 128 * 1024;
        byte[] waveBuffer;
        static uint stream_buffer_in, stream_buffer_size;
        int wBitsPerSample ;
        int nChannels ;
        int nSamplesPerSec;
        int nBlockAlign;
        int nAvgBytesPerSec;
        private short ReadSample(byte[] buffer, int index)
        {
            // Ensure we're doing aligned reads.
            if (index % sizeof(short) != 0)
            {
                throw new ArgumentException("index");
            }

            if (index >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            short sample = 0;
            if (!BitConverter.IsLittleEndian)
            {
                sample = (short)(buffer[index] << 8 | buffer[index + 1] & 0xff);
            }
            else
            {
                sample = (short)(buffer[index] & 0xff | buffer[index + 1] << 8);
            }

            return sample;
        }
        int osd_start_audio_stream(bool stereo)
        {
            stream_cache_stereo = stereo;
             wBitsPerSample = 16;
             nChannels = stereo ? 2 : 1;
             nSamplesPerSec = Machine.sample_rate;
             nBlockAlign = wBitsPerSample * nChannels / 8;
             nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;

            /* determine the number of samples per frame */
            samples_per_frame = (double)Machine.sample_rate / (double)Machine.drv.frames_per_second;
            stream_buffer_size = (uint)(((ulong)MAX_BUFFER_SIZE * (ulong)nSamplesPerSec) / 44100);
            stream_buffer_size = (uint)(stream_buffer_size * nBlockAlign) / 4;
            stream_buffer_size =(uint)( (stream_buffer_size * 30) / Machine.drv.frames_per_second);
            stream_buffer_size = (stream_buffer_size / 1024) * 1024;
            System.Console.WriteLine("stream_buffer_size {0}", stream_buffer_size);
            /* compute how many samples to generate this frame */
            samples_left_over = samples_per_frame;
            snd_samples_this_frame = (uint)samples_left_over;
            samples_left_over -= (double)snd_samples_this_frame;
            audio_buffer_length = (int)(NUM_BUFFERS * samples_per_frame + 20);

            if (Machine.sample_rate == 0) return 0;
            stream_playing = 1;
            voice_pos = 0;
            audio_buffer_length = (int)(NUM_BUFFERS * snd_samples_this_frame )-1;

            soundInstance = new DynamicSoundEffectInstance(Machine.sample_rate, stereo ? AudioChannels.Stereo : AudioChannels.Mono);
            System.Console.WriteLine("{0}/{1}", audio_buffer_length, soundInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(31)));
#if UseCallbackEventHandler
            soundInstance.BufferNeeded += new EventHandler<EventArgs>(DynamicSoundInstance_BufferNeeded);
#endif
            audio_buffer_length =soundInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(22));
            waveBuffer = new byte[stream_buffer_size];//soundInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(25))];

            soundInstance.Play();
            return (int)samples_this_frame;
        }
        void DynamicSoundInstance_BufferNeeded(object sender, EventArgs e)
        {
            // Submitting the larger buffers as two smaller buffers ensures that
            // the event is raised before the buffers run out, avoiding glitches
#if EventSubmitTwoBuffers
            soundInstance.SubmitBuffer(waveBuffer, 0, waveBuffer.Length / 2);
            soundInstance.SubmitBuffer(waveBuffer, waveBuffer.Length / 2, waveBuffer.Length / 2);
#else
            soundInstance.SubmitBuffer(waveBuffer, 0, waveBuffer.Length); 
#endif
            }
        private void WriteSample(byte[] buffer, int index, short sample)
        {
            // Ensure we're doing aligned writes.
            if (index % sizeof(short) != 0)
            {
                throw new ArgumentException("index");
            }

            if (index >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (!BitConverter.IsLittleEndian)
            {
                buffer[index] = (byte)(sample >> 8);
                buffer[index + 1] = (byte)sample;
            }
            else
            {
                buffer[index] = (byte)sample;
                buffer[index + 1] = (byte)(sample >> 8);
            }
        }

        bool xna_update_audio()
        {
            if (Machine.sample_rate == 0 || stream_cache_data == null) return false;
           
#if SubmitAudioAsNeeded
            if (soundInstance.PendingBufferCount < 3)
            soundInstance.SubmitBuffer(waveBuffer, 0, waveBuffer.Length);
#endif
            //updateaudiostream();

            return true;
        }
        void osd_sound_enable(bool enable_it)
        {
            if (enable_it)
                soundInstance.Play();
            else
                soundInstance.Stop();
        }
        void updateaudiostream()
        {
            short[] data = stream_cache_data;
            bool stereo = stream_cache_stereo;
            int len = stream_cache_len;
            int buflen;
            int start, end;

            if (stream_playing == 0) return;	/* error */

            buflen = audio_buffer_length;
            start = voice_pos;
            end = voice_pos + len;
            if (end > buflen) end -= buflen;


            if (stereo)
            {
                int p = start;

                int di = 0;
                while (p != end)
                {
                    if (p >= buflen) p -= buflen;
                    WriteSample(waveBuffer, p++, (short)((data[di++]*master_volume/256)^0x6000));
                    WriteSample(waveBuffer, p++, (short)((data[di++]*master_volume/256)^0x6000));
                }
            }
            else
            {
                int p = start;
                int bi = 0;
                while (p != end)
                {
                    if (p >= buflen) p -= buflen;
                    WriteSample(waveBuffer,  bi, (short)((data[p++]*master_volume/256)^0x6000));
                    bi += 2;
                }
            }

            voice_pos = end;
            if (voice_pos == buflen) voice_pos = 0;
        }
        int osd_update_audio_stream1(short[] buffer)
        {
            stream_cache_data = buffer;
            stream_cache_len = (int)samples_this_frame;

            /* compute how many samples to generate next frame */
            samples_left_over += samples_per_frame;
            samples_this_frame = (uint)samples_left_over;
            samples_left_over -= (double)samples_this_frame;

            return (int)samples_this_frame;
        }
        int osd_update_audio_stream(short[] buffer)
        {
            int length1 = waveBuffer.Length;
            // adjust the input pointer
            stream_buffer_in = (stream_buffer_in + samples_this_frame) % stream_buffer_size;
            int bytes_to_copy =(int)( samples_this_frame * nBlockAlign);
            if (bytes_to_copy > length1)System.Console.WriteLine("1 {0}={1}", bytes_to_copy, length1);
            // copy the first chunk
            int cur_bytes = (bytes_to_copy > length1) ? length1 : bytes_to_copy;
            //memcpy(buffer1, data, cur_bytes);
            for (int i = 0; i < cur_bytes / 2; i++)
                WriteSample(waveBuffer, i * 2, (short)((buffer[i] * master_volume / 256) ));

            if (cur_bytes > 0)
            soundInstance.SubmitBuffer(waveBuffer, 0, cur_bytes);
            // adjust for the number of bytes
            bytes_to_copy -= cur_bytes;
           // data = (UINT16*)((UINT8*)data + cur_bytes);

            // copy the second chunk
            if (bytes_to_copy != 0)
            {
                //if (bytes_to_copy > length1) System.Console.WriteLine("2 {0}={1}", bytes_to_copy, length1);
                cur_bytes = (bytes_to_copy > length1) ? length1 : bytes_to_copy;
                for (int i = 0; i < cur_bytes / 2; i++)
                    WriteSample(waveBuffer, i * 2, (short)((buffer[i] * master_volume / 256) ));

                if (cur_bytes > 0)    
                soundInstance.SubmitBuffer(waveBuffer, 0, cur_bytes);
            }

            stream_cache_data = buffer;
            stream_cache_len = (int)samples_this_frame;

            /* compute how many samples to generate next frame */
            samples_left_over += samples_per_frame;
            samples_this_frame = (uint)samples_left_over;
            samples_left_over -= (double)samples_this_frame;

            return (int)samples_this_frame;
        }
         
        void osd_stop_audio_stream()
        {
            if (Machine.sample_rate == 0) return;

            soundInstance.Stop();
            stream_playing = 0;
        }
    }
}
