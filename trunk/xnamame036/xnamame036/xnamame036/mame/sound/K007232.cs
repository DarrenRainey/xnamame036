using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAX_K007232 = 3;
    }
    public delegate void K007232_writehandler(int a);
    public class K007232_interface
    {
        public int num_chips;
        public int[] bank = new int[Mame.MAX_K007232];
        public int[] volume = new int[Mame.MAX_K007232];
        public K007232_writehandler[] portwritehandler = new K007232_writehandler[Mame.MAX_K007232];

        public K007232_interface(int num_chips, int[] bank, int[] volume, K007232_writehandler[] writehandler)
        {
            this.num_chips = num_chips; this.bank = bank; this.volume = volume;
            this.portwritehandler = writehandler;
        }
    }
    public class K007232 : Mame.snd_interface
    {
        public K007232()
        {
            this.name = "007232";
            this.sound_num = Mame.SOUND_K007232;
            for (int i = 0; i < Mame.MAX_K007232; i++)
            {
                kpcm[i] = new kdacApcm();
            }
        }

        public static int K007232_VOL(int LVol, int LPan, int RVol, int RPan) { return ((LVol) | ((LPan) << 8) | ((RVol) << 16) | ((RPan) << 24)); }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return 0;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((K007232_interface)msound.sound_interface).num_chips;
        }
        public override int start(Mame.MachineSound msound)
        {

            intf = (K007232_interface)msound.sound_interface;

            /* Set up the chips */
            for (int j = 0; j < intf.num_chips; j++)
            {
                //char buf[2][40];
                string[] name = new string[2];
                int[] vol = new int[2];
                kpcm[j] = new kdacApcm();
                kpcm[j].pcmbuf[0] = Mame.memory_region(intf.bank[j]);
                kpcm[j].pcmbuf[1] = Mame.memory_region(intf.bank[j]);

                for (int i = 0; i < KDAC_A_PCM_MAX; i++)
                {
                    kpcm[j].start[i] = 0;
                    kpcm[j].step[i] = 0;
                    kpcm[j].play[i] = 0;
                    kpcm[j].loop[i] = 0;
                }
                kpcm[j].vol[0][0] = 255;	/* channel A output to output A */
                kpcm[j].vol[0][1] = 0;
                kpcm[j].vol[1][0] = 0;
                kpcm[j].vol[1][1] = 255;	/* channel B output to output B */

                for (int i = 0; i < 0x10; i++) kpcm[j].wreg[i] = 0;

                for (int i = 0; i < 2; i++)
                {
                    name[i] = Mame.sprintf("007232 #%d Ch %c", j, 'A' + i);
                }

                vol[0] = intf.volume[j] & 0xffff;
                vol[1] = intf.volume[j] >> 16;

                pcm_chan[j] = Mame.stream_init_multi(2, name, vol, Mame.Machine.sample_rate, j, KDAC_A_update);
            }

            KDAC_A_make_fncode();

            return 0;
        }

        static void KDAC_A_make_fncode()
        {
#if false
  int i, j, k;
  float fn;
  for( i = 0; i < 0x200; i++ )  fncode[i] = 0;

  i = 0;
  while( (int)kdaca_fn[i][0] != -1 ){
    fncode[(int)kdaca_fn[i][0]] = kdaca_fn[i][1];
    i++;
  }

  i = j = 0;
  while( i < 0x200 ){
    if( fncode[i] != 0 ){
      if( i != j ){
	fn = (fncode[i] - fncode[j]) / (i - j);
	for( k = 1; k < (i-j); k++ )
	  fncode[k+j] = fncode[j] + fn*k;
	j = i;
      }
    }
    i++;
  }
#if false
 	for( i = 0; i < 0x200; i++ )
  if (errorlog) fprintf( errorlog,"fncode[%04x] = %.2f\n", i, fncode[i] );
#endif

#else
            for (int i = 0; i < 0x200; i++)
            {
                fncode[i] = (0x200 * 55) / (0x200 - i);
                //    if (errorlog) fprintf( errorlog,"2 : fncode[%04x] = %.2f\n", i, fncode[i] );
            }

#endif
        }

        static void KDAC_A_update(int chip, _ShortPtr[] buffer, int buffer_len)
        {
            Array.Clear(buffer[0].buffer,buffer[0].offset,buffer_len);
            Array.Clear(buffer[1].buffer,buffer[1].offset,buffer_len);
	//memset(buffer[0],0,buffer_len * sizeof(INT16));
	//memset(buffer[1],0,buffer_len * sizeof(INT16));

	for( int i = 0; i < KDAC_A_PCM_MAX; i++ )
	{
		if (kpcm[chip].play[i]!=0)
		{
			int volA,volB,j,_out;
			uint addr, old_addr;

			/**** PCM setup ****/
			addr = kpcm[chip].start[i] + ((kpcm[chip].addr[i]>>BASE_SHIFT)&0x000fffff);
			volA = 2 * kpcm[chip].vol[i][0];
			volB = 2 * kpcm[chip].vol[i][1];
			for( j = 0; j < buffer_len; j++ )
			{
				old_addr = addr;
				addr = kpcm[chip].start[i] + ((kpcm[chip].addr[i]>>BASE_SHIFT)&0x000fffff);
				while (old_addr <= addr)
				{
					if ((kpcm[chip].pcmbuf[i][old_addr] & 0x80)!=0)
					{
						/* end of sample */

						if (kpcm[chip].loop[i]!=0)
						{
							/* loop to the beginning */
							addr = kpcm[chip].start[i];
							kpcm[chip].addr[i] = 0;
						}
						else
						{
							/* stop sample */
							kpcm[chip].play[i] = 0;
						}
						break;
					}

					old_addr++;
				}

				if (kpcm[chip].play[i] == 0)
					break;

				kpcm[chip].addr[i] += kpcm[chip].step[i];

				_out = (kpcm[chip].pcmbuf[i][addr] & 0x7f) - 0x40;

				buffer[0].write16(j,(ushort)(buffer[0].read16(j) +_out * volA));
				buffer[1].write16(j,(ushort)(buffer[1].read16(j)+ _out * volB));
			}
		}
	}
        }
        public override void stop()
        {
            //nothing
        }
        public override void reset()
        {
            //nothing
        }
        public override void update()
        {
            //nothing
        }
        const byte KDAC_A_PCM_MAX = 2;

        class kdacApcm
        {
            public kdacApcm()
            {
                vol[0] = new byte[2];
                vol[1] = new byte[2];
            }
            public byte[][] vol = new byte[KDAC_A_PCM_MAX][];
            public uint[] addr = new uint[KDAC_A_PCM_MAX];
            public uint[] start = new uint[KDAC_A_PCM_MAX];
            public uint[] step = new uint[KDAC_A_PCM_MAX];
            public int[] play = new int[KDAC_A_PCM_MAX];
            public int[] loop = new int[KDAC_A_PCM_MAX];
            public byte[] wreg = new byte[0x10];
            public _BytePtr[] pcmbuf = new _BytePtr[2];
        }
        static kdacApcm[] kpcm = new kdacApcm[Mame.MAX_K007232];
        static float[] fncode = new float[0x200];
        static int[] pcm_chan = new int[Mame.MAX_K007232];
        static K007232_interface intf;
        const byte BASE_SHIFT = 12;


        public static void K007232_set_volume(int chip, int channel, int volumeA, int volumeB)
        {
            kpcm[chip].vol[channel][0] = (byte)volumeA;
            kpcm[chip].vol[channel][1] = (byte)volumeB;
        }
        public static void K007232_bankswitch(int chip, _BytePtr ptr_A, _BytePtr ptr_B)
        {
            kpcm[chip].pcmbuf[0] = ptr_A;
            kpcm[chip].pcmbuf[1] = ptr_B;
        }
        static void K007232_WriteReg(int r, int v, int chip)
        {
            int data;

            if (Mame.Machine.sample_rate == 0) return;

            Mame.stream_update(pcm_chan[chip], 0);

            kpcm[chip].wreg[r] = (byte)v;			/* stock write data */

            if (r == 0x05)
            {
                if (kpcm[chip].start[0] < 0x20000)
                {
                    kpcm[chip].play[0] = 1;
                    kpcm[chip].addr[0] = 0;
                }
            }
            else if (r == 0x0b)
            {
                if (kpcm[chip].start[1] < 0x20000)
                {
                    kpcm[chip].play[1] = 1;
                    kpcm[chip].addr[1] = 0;
                }
            }
            else if (r == 0x0d)
            {
                /* select if sample plays once or looped */
                kpcm[chip].loop[0] = v & 0x01;
                kpcm[chip].loop[1] = v & 0x02;
                return;
            }
            else if (r == 0x0c)
            {
                /* external port, usually volume control */
                if (intf.portwritehandler[chip] != null) intf.portwritehandler[chip](v);
                return;
            }
            else
            {
                int reg_port;

                reg_port = 0;
                if (r >= 0x06)
                {
                    reg_port = 1;
                    r -= 0x06;
                }

                switch (r)
                {
                    case 0x00:
                    case 0x01:
                        /**** address step ****/
                        data = (int)(((((uint)kpcm[chip].wreg[reg_port * 0x06 + 0x01]) << 8) & 0x0100) | (((uint)kpcm[chip].wreg[reg_port * 0x06 + 0x00]) & 0x00ff));
#if false
				if( !reg_port && r == 1 )
				if (errorlog) fprintf( errorlog, "%04x\n" ,data );
#endif

                        kpcm[chip].step[reg_port] = (uint)(
                            ((7850.0 / (float)Mame.Machine.sample_rate)) *
                            (fncode[data] / (440.00 / 2)) *
                            ((float)3580000 / (float)4000000) *
                            (1 << BASE_SHIFT));
                        break;

                    case 0x02:
                    case 0x03:
                    case 0x04:
                        /**** start address ****/
                        kpcm[chip].start[reg_port] =
                            ((((uint)kpcm[chip].wreg[reg_port * 0x06 + 0x04] << 16) & 0x00010000) |
                            (((uint)kpcm[chip].wreg[reg_port * 0x06 + 0x03] << 8) & 0x0000ff00) |
                            (((uint)kpcm[chip].wreg[reg_port * 0x06 + 0x02]) & 0x000000ff));
                        break;
                }
            }
        }
        static int K007232_ReadReg(int r, int chip)
        {
            if (r == 0x05)
            {
                if (kpcm[chip].start[0] < 0x20000)
                {
                    kpcm[chip].play[0] = 1;
                    kpcm[chip].addr[0] = 0;
                }
            }
            else if (r == 0x0b)
            {
                if (kpcm[chip].start[1] < 0x20000)
                {
                    kpcm[chip].play[1] = 1;
                    kpcm[chip].addr[1] = 0;
                }
            }
            return 0;
        }
        public static void K007232_write_port_0_w(int r, int v)
        {
            K007232_WriteReg(r, v, 0);
        }

        public static int K007232_read_port_0_r(int r)
        {
            return K007232_ReadReg(r, 0);
        }

        public static void K007232_write_port_1_w(int r, int v)
        {
            K007232_WriteReg(r, v, 1);
        }

        public static int K007232_read_port_1_r(int r)
        {
            return K007232_ReadReg(r, 1);
        }
    }
}
