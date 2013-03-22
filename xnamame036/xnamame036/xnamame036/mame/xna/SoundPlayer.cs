using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace xnamame036.mame
{
    class SoundPlayer
    {
        const int MAX_BUFFER_SIZE = 128 * 1024;
        private DynamicSoundEffectInstance soundInstance;
        private byte[] waveBuffer;
        uint stream_buffer_size;

        public SoundPlayer(int sampleRate, bool stereo, int framesPerSecond)
        {
            soundInstance = new DynamicSoundEffectInstance(sampleRate, stereo ? AudioChannels.Stereo : AudioChannels.Mono);
             stream_buffer_size = (uint)(((ulong)MAX_BUFFER_SIZE * (ulong)sampleRate) / 44100);
            var wBitsPerSample = 16;
            var nChannels = stereo ? 2 : 1;
            var nBlockAlign = wBitsPerSample * nChannels / 8;
            stream_buffer_size = (uint)(stream_buffer_size * nBlockAlign) / 4;
            stream_buffer_size = (uint)((stream_buffer_size * 30) / framesPerSecond);
            stream_buffer_size = (stream_buffer_size / 1024) * 1024;

            waveBuffer = new byte[stream_buffer_size];//soundInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(25))];

            soundInstance.Play();            
        }
        public uint GetStreamBufferSize() { return stream_buffer_size; }
        public int GetSampleSizeInBytes(TimeSpan duration)
        {
            return soundInstance.GetSampleSizeInBytes(duration);
        }
        public void Play()
        {
            soundInstance.Play();
        }
        public void Stop()
        {
            soundInstance.Stop();
        }       
        public void WriteSample(int index, short sample)
        {
            if (!BitConverter.IsLittleEndian)
            {
                waveBuffer[index] = (byte)(sample >> 8);
                waveBuffer[index + 1] = (byte)sample;
            }
            else
            {
                waveBuffer[index] = (byte)sample;
                waveBuffer[index + 1] = (byte)(sample >> 8);
            }
            //waveBuffer,  bi, (short)((data[p++]*master_volume/256)));
        }
        public void SubmitBuffer(int offset, int length)
        {
            soundInstance.SubmitBuffer(waveBuffer,offset, length); // remove if using new stuff
        }
    }
}
