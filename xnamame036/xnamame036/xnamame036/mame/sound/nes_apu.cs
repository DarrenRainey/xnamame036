using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAX_NESPSG = 2;
    }
    public class NESinterface
    {
        public int num;
        public int[] region = new int[Mame.MAX_NESPSG];
        public int[] volume = new int[Mame.MAX_NESPSG];
        public NESinterface(int num, int[] region, int[] volume) { this.num = num; this.region = region; this.volume = volume; }
    }
    class nes_apu : Mame.snd_interface
    {
        const byte SYNCS_MAX1 = 0x20, SYNCS_MAX2 = 0x80;
        const byte APU_WRA0 = 0x00;
        const byte APU_WRA1 = 0x01;
        const byte APU_WRA2 = 0x02;
        const byte APU_WRA3 = 0x03;
        const byte APU_WRB0 = 0x04;
        const byte APU_WRB1 = 0x05;
        const byte APU_WRB2 = 0x06;
        const byte APU_WRB3 = 0x07;
        const byte APU_WRC0 = 0x08;
        const byte APU_WRC2 = 0x0A;
        const byte APU_WRC3 = 0x0B;
        const byte APU_WRD0 = 0x0C;
        const byte APU_WRD2 = 0x0E;
        const byte APU_WRD3 = 0x0F;
        const byte APU_WRE0 = 0x10;
        const byte APU_WRE1 = 0x11;
        const byte APU_WRE2 = 0x12;
        const byte APU_WRE3 = 0x13;

        const byte APU_SMASK = 0x15;

        const int NOISE_LONG = 0x4000;
        const int NOISE_SHORT = 93;


        /* vblank length table used for squares, triangle, noise */
        static byte[] vbl_length =
{
   5, 127, 10, 1, 19,  2, 40,  3, 80,  4, 30,  5, 7,  6, 13,  7,
   6,   8, 12, 9, 24, 10, 48, 11, 96, 12, 36, 13, 8, 14, 16, 15
};

        /* frequency limit of square channels */
        static int[] freq_limit =
{
   0x3FF, 0x555, 0x666, 0x71C, 0x787, 0x7C1, 0x7E0, 0x7F0,
};

        /* table of noise frequencies */
        static int[] noise_freq =
{
   4, 8, 16, 32, 64, 96, 128, 160, 202, 254, 380, 508, 762, 1016, 2034, 2046
};

        /* dpcm transfer freqs */
        static int[] dpcm_clocks =
{
   428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 85, 72, 54
};

        /* ratios of pos/neg pulse for square waves */
        /* 2/16 = 12.5%, 4/16 = 25%, 8/16 = 50%, 12/16 = 75% */
        static int[] duty_lut =
{
   2, 4, 8, 12
};


        class square_t
        {
            public byte[] regs = new byte[4];
            public int vbl_length;
            public int freq;
            public float phaseacc;
            public float output_vol;
            public float env_phase;
            public float sweep_phase;
            public byte adder;
            public byte env_vol;
            public bool enabled;
        }
        class triangle_t
        {
            public byte[] regs = new byte[4]; /* regs[1] unused */
            public int linear_length;
            public int vbl_length;
            public int write_latency;
            public float phaseacc;
            public float output_vol;
            public byte adder;
            public bool counter_started;
            public bool enabled;
        }
        class noise_t
        {
            public byte[] regs = new byte[4]; /* regs[1] unused */
            public int cur_pos;
            public int vbl_length;
            public float phaseacc;
            public float output_vol;
            public float env_phase;
            public byte env_vol;
            public bool enabled;
        }
        class dpcm_t
        {
            public byte[] regs = new byte[4];
            public uint address;
            public uint length;
            public int bits_left;
            public float phaseacc;
            public float output_vol;
            public byte cur_byte;
            public bool enabled;
            public bool irq_occurred;
            public _BytePtr cpu_mem;
            public sbyte vol;
        }
        class apu_t
        {
            public apu_t()
            {
                dpcm = new dpcm_t();
                noi = new noise_t();
                tri = new triangle_t();
                squ[0] = new square_t();
                squ[1] = new square_t();
            }

            /* Sound channels */
            public square_t[] squ = new square_t[2];
            public triangle_t tri;
            public noise_t noi;
            public dpcm_t dpcm;

            /* APU registers */
            public byte[] regs = new byte[22];

            /* Sound pointers */
            public byte[] buffer;

#if USE_QUEUE

   /* Event queue */
   queue_t queue[QUEUE_SIZE];
   int head,tail;

#else

            public int buf_pos;

#endif
        }
        apu_t[] APU = new apu_t[Mame.MAX_NESPSG];
        apu_t cur;

        static float apu_incsize;           /* Adjustment increment */
        static ushort samps_per_sync;        /* Number of samples per vsync */
        static ushort buffer_size;           /* Actual buffer size in bytes */
        static ushort real_rate;             /* Actual playback rate */
        static ushort chip_max;              /* Desired number of chips in use */
        static byte[] noise_lut = new byte[NOISE_LONG]; /* Noise sample lookup table */
        static ushort[] vbl_times = new ushort[0x20];       /* VBL durations in samples */
        static uint[] sync_times1 = new uint[SYNCS_MAX1]; /* Samples per sync table */
        static uint[] sync_times2 = new uint[SYNCS_MAX2]; /* Samples per sync table */
        static int channel;

        public override int chips_clock(Mame.MachineSound msound)
        {
            return 0;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((NESinterface)msound.sound_interface).num;
        }
        public override int start(Mame.MachineSound msound)
        {
            NESinterface intf = (NESinterface)msound.sound_interface;
            int i;

            /* Initialize global variables */
            samps_per_sync = (ushort)(Mame.Machine.sample_rate / Mame.Machine.drv.frames_per_second);
            buffer_size = samps_per_sync;
            real_rate = (ushort)(samps_per_sync * Mame.Machine.drv.frames_per_second);
            chip_max = (ushort)intf.num;
            apu_incsize = (float)(Mame.cpu_n2a03.N2A03_DEFAULTCLOCK / (float)real_rate);

            /* Use initializer calls */
            create_noise(noise_lut, 13, NOISE_LONG);
            create_vbltimes(vbl_times, vbl_length, samps_per_sync);
            create_syncs(samps_per_sync);

            /* Adjust buffer size if 16 bits */
            buffer_size += samps_per_sync;

            /* Initialize individual chips */
            for (i = 0; i < chip_max; i++)
            {
                APU[i] = new apu_t();

                //memset(cur,0,sizeof(apu_t));

                APU[i].buffer = new byte[buffer_size];

#if USE_QUEUE
     cur.head=0;cur.tail=QUEUE_MAX;
#endif
                (APU[i].dpcm).cpu_mem = Mame.memory_region(intf.region[i]);
            }

            channel = Mame.mixer_allocate_channels(chip_max, intf.volume);
            for (i = 0; i < chip_max; i++)
            {
                string name = Mame.sprintf("%s #%d", Mame.sound_name(msound), i);
                Mame.mixer_set_name(channel, name);
            }

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
  if (real_rate==0) return;

  for (int i = 0;i < chip_max;i++)
  {
    apu_update(i);
#if !USE_QUEUE
    APU[i].buf_pos=0;
#endif
    Mame.mixer_play_streamed_sample_16(channel+i,new _ShortPtr(APU[i].buffer),buffer_size,real_rate);
  }
        }
        static byte apu_read(int chip, int address)
        {
            return 0;
        }
        static void apu_write(int chip, int address, byte value)
        {

        }

        static sbyte apu_square(square_t chan)
        {
            int env_delay;
            int sweep_delay;
            sbyte output;

            /* reg0: 0-3=volume, 4=envelope, 5=hold, 6-7=duty cycle
            ** reg1: 0-2=sweep shifts, 3=sweep inc/dec, 4-6=sweep length, 7=sweep on
            ** reg2: 8 bits of freq
            ** reg3: 0-2=high freq, 7-4=vbl length counter
            */

            if (false == chan.enabled)
                return 0;

            /* enveloping */
            env_delay = (int)sync_times1[chan.regs[0] & 0x0F];

            /* decay is at a rate of (env_regs + 1) / 240 secs */
            chan.env_phase -= 4;
            while (chan.env_phase < 0)
            {
                chan.env_phase += env_delay;
                if ((chan.regs[0] & 0x20)!=0)
                    chan.env_vol = (byte)((chan.env_vol + 1) & 15);
                else if (chan.env_vol < 15)
                    chan.env_vol++;
            }

            /* vbl length counter */
            if (chan.vbl_length > 0 && 0 == (chan.regs[0] & 0x20))
                chan.vbl_length--;

            if (0 == chan.vbl_length)
                return 0;

            /* freqsweeps */
            if ((chan.regs[1] & 0x80) !=0&& (chan.regs[1] & 7)!=0)
            {
                sweep_delay = (int)sync_times1[(chan.regs[1] >> 4) & 7];
                chan.sweep_phase -= 2;
                while (chan.sweep_phase < 0)
                {
                    chan.sweep_phase += sweep_delay;
                    if ((chan.regs[1] & 8)!=0)
                        chan.freq -= chan.freq >> (chan.regs[1] & 7);
                    else
                        chan.freq += chan.freq >> (chan.regs[1] & 7);
                }
            }

            if ((0 == (chan.regs[1] & 8) && (chan.freq >> 16) > freq_limit[chan.regs[1] & 7])
                || (chan.freq >> 16) < 4)
                return 0;

            chan.phaseacc -= (float)apu_incsize; /* # of cycles per sample */

            while (chan.phaseacc < 0)
            {
                chan.phaseacc += (chan.freq >> 16);
                chan.adder = (byte)((chan.adder + 1) & 0x0F);
            }

            if ((chan.regs[0] & 0x10) !=0)/* fixed volume */
                output = (sbyte)(chan.regs[0] & 0x0F);
            else
                output =(sbyte)( 0x0F - chan.env_vol);

            if (chan.adder < (duty_lut[chan.regs[0] >> 6]))
                output =(sbyte) -output;

            return (sbyte)output;
        }
        sbyte apu_noise(noise_t chan)
        {
            int freq, env_delay;
            byte outvol;
            byte output;

            /* reg0: 0-3=volume, 4=envelope, 5=hold
            ** reg2: 7=small(93 byte) sample,3-0=freq lookup
            ** reg3: 7-4=vbl length counter
            */

            if (false == chan.enabled)
                return 0;

            /* enveloping */
            env_delay = (int)sync_times1[chan.regs[0] & 0x0F];

            /* decay is at a rate of (env_regs + 1) / 240 secs */
            chan.env_phase -= 4;
            while (chan.env_phase < 0)
            {
                chan.env_phase += env_delay;
                if ((chan.regs[0] & 0x20)!=0)
                    chan.env_vol = (byte)((chan.env_vol + 1) & 15);
                else if (chan.env_vol < 15)
                    chan.env_vol++;
            }

            /* length counter */
            if (0 == (chan.regs[0] & 0x20))
            {
                if (chan.vbl_length > 0)
                    chan.vbl_length--;
            }

            if (0 == chan.vbl_length)
                return 0;

            freq = noise_freq[chan.regs[2] & 0x0F];
            chan.phaseacc -= (float)apu_incsize; /* # of cycles per sample */
            while (chan.phaseacc < 0)
            {
                chan.phaseacc += freq;

                chan.cur_pos++;
                if (NOISE_SHORT == chan.cur_pos && (chan.regs[2] & 0x80)!=0)
                    chan.cur_pos = 0;
                else if (NOISE_LONG == chan.cur_pos)
                    chan.cur_pos = 0;
            }

            if ((chan.regs[0] & 0x10)!=0) /* fixed volume */
                outvol = (byte)(chan.regs[0] & 0x0F);
            else
                outvol = (byte)(0x0F - chan.env_vol);

            output = noise_lut[chan.cur_pos];
            if (output > outvol)
                output = outvol;

            if ((noise_lut[chan.cur_pos] & 0x80)!=0) /* make it negative */
                output =(byte)((sbyte) -output);

            return (sbyte)output;
        }
        sbyte apu_triangle(triangle_t chan)
        {
            int freq;
            sbyte output;
            /* reg0: 7=holdnote, 6-0=linear length counter
            ** reg2: low 8 bits of frequency
            ** reg3: 7-3=length counter, 2-0=high 3 bits of frequency
            */

            if (false == chan.enabled)
                return 0;

            if (false == chan.counter_started && 0 == (chan.regs[0] & 0x80))
            {
                if (chan.write_latency!=0)
                    chan.write_latency--;
                if (0 == chan.write_latency)
                    chan.counter_started = true;
            }

            if (chan.counter_started)
            {
                if (chan.linear_length > 0)
                    chan.linear_length--;
                if (chan.vbl_length !=0&& 0 == (chan.regs[0] & 0x80))
                    chan.vbl_length--;

                if (0 == chan.vbl_length)
                    return 0;
            }

            if (0 == chan.linear_length)
                return 0;

            freq = (((chan.regs[3] & 7) << 8) + chan.regs[2]) + 1;

            if (freq < 4) /* inaudible */
                return 0;

            chan.phaseacc -= (float)apu_incsize; /* # of cycles per sample */
            while (chan.phaseacc < 0)
            {
                chan.phaseacc += freq;
                chan.adder = (byte)((chan.adder + 1) & 0x1F);

                output =(sbyte)( (chan.adder & 7) << 1);
                if ((chan.adder & 8)!=0)
                    output = (sbyte)(0x10 - output);
                if ((chan.adder & 0x10)!=0)
                    output = (sbyte)-output;

                chan.output_vol = output;
            }

            return (sbyte)chan.output_vol;
        }
        void apu_dpcmreset(dpcm_t chan)
        {
            chan.address = (uint)(0xC000 + (ushort)(chan.regs[2] << 6));
            chan.length = (uint)((ushort)(chan.regs[3] << 4) + 1);
            chan.bits_left =(int)( chan.length << 3);
            chan.irq_occurred = false;
        }
        sbyte apu_dpcm(dpcm_t chan)
        {
            int freq, bit_pos;

            /* reg0: 7=irq gen, 6=looping, 3-0=pointer to clock table
            ** reg1: output dc level, 7 bits unsigned
            ** reg2: 8 bits of 64-byte aligned address offset : $C000 + (value * 64)
            ** reg3: length, (value * 16) + 1
            */

            if (chan.enabled)
            {
                freq = dpcm_clocks[chan.regs[0] & 0x0F];
                chan.phaseacc -= (float)apu_incsize; /* # of cycles per sample */

                while (chan.phaseacc < 0)
                {
                    chan.phaseacc += freq;

                    if (0 == chan.length)
                    {
                        if ((chan.regs[0] & 0x40)!=0)
                            apu_dpcmreset(chan);
                        else
                        {
                            if ((chan.regs[0] & 0x80)!=0) /* IRQ Generator */
                            {
                                chan.irq_occurred = true;
                                Mame.cpu_n2a03.n2a03_irq();
                            }
                            break;
                        }
                    }

                    chan.bits_left--;
                    bit_pos = 7 - (chan.bits_left & 7);
                    if (7 == bit_pos)
                    {
                        chan.cur_byte = chan.cpu_mem[chan.address];
                        chan.address++;
                        chan.length--;
                    }

                    if ((chan.cur_byte & (1 << bit_pos))!=0)
                        //            chan.regs[1]++;
                        chan.vol++;
                    else
                        //            chan.regs[1]--;
                        chan.vol--;
                }
            }

            if (chan.vol > 63)
                chan.vol = 63;
            else if (chan.vol < -64)
                chan.vol = -64;

            return (sbyte)(chan.vol >> 1);
        }
   static _ShortPtr buffer16 = null;
        void apu_update(int chip)
{
   int accum;
   int endp = Mame.sound_scalebufferpos(samps_per_sync);
   int elapsed;

#if USE_QUEUE
   queue_t *q=NULL;

   elapsed=0;
#endif

   cur= APU[chip];
   buffer16  = new _ShortPtr(cur.buffer);

#if !USE_QUEUE
   /* Recall last position updated and restore pointers */
   elapsed = cur.buf_pos;
   buffer16.offset += elapsed*2;
#endif

   while (elapsed<endp)
   {
#if USE_QUEUE
      while (apu_queuenotempty(chip) && (cur.queue[cur.head].pos==elapsed))
      {
         q = apu_dequeue(chip);
         apu_regwrite(chip,q.reg,q.val);
      }
#endif
      elapsed++;

      accum = apu_square(cur.squ[0]);
      accum += apu_square(cur.squ[1]);
      accum += apu_triangle(cur.tri);
      accum += apu_noise(cur.noi);
      accum += apu_dpcm(cur.dpcm);

      /* 8-bit clamps */
      if (accum > 127)
         accum = 127;
      else if (accum < -128)
         accum = -128;

      buffer16.write16(0, (ushort)(accum << 8));
      buffer16.offset += 2;
   }
#if !USE_QUEUE
   cur.buf_pos = endp;
#endif
}

        static void create_vbltimes(ushort[] table, byte[] vbl, uint rate)
        {
            for (int i = 0; i < 0x20; i++)
                table[i] = (ushort)(vbl[i] * rate);
        }

        /* INITIALIZE SAMPLE TIMES IN TERMS OF VSYNCS */
        static void create_syncs(uint sps)
        {
            int i;
            uint val = sps;

            for (i = 0; i < SYNCS_MAX1; i++)
            {
                sync_times1[i] = val;
                val += sps;
            }

            val = 0;
            for (i = 0; i < SYNCS_MAX2; i++)
            {
                sync_times2[i] = val;
                sync_times2[i] >>= 2;
                val += sps;
            }
        }

        /* INITIALIZE NOISE LOOKUP TABLE */
        static int m = 0x0011;
        static void create_noise(byte[] buf, int bits, int size)
        {
            int xor_val, i;

            for (i = 0; i < size; i++)
            {
                xor_val = m & 1;
                m >>= 1;
                xor_val ^= (m & 1);
                m |= xor_val << (bits - 1);

                buf[i] = (byte)m;
            }
        }




        public static int NESPSG_0_r(int offset) { return apu_read(0, offset); }
        public static int NESPSG_1_r(int offset) { return apu_read(1, offset); }
        public static void NESPSG_0_w(int offset, int data) { apu_write(0, offset, (byte)data); }
        public static void NESPSG_1_w(int offset, int data) { apu_write(1, offset, (byte)data); }
    }
}
