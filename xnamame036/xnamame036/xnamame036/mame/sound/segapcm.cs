using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class SEGAPCMinterface
    {
        public int mode, bank, region, volume;
        public SEGAPCMinterface(int mode, int bank, int region, int volume)
        {
            this.mode = mode; this.bank = bank; this.region = region; this.volume = volume;
        }
    }
    class segapcm : Mame.snd_interface
    {
        public segapcm()
        {
            this.sound_num = Mame.SOUND_SEGAPCM;
            this.name = "SEGAPCM";
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return 0;// throw new NotImplementedException();
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return 0;// throw new NotImplementedException();
        }
        public override int start(Mame.MachineSound msound)
        {
            SEGAPCMinterface intf = (SEGAPCMinterface)msound.sound_interface;
            if (Mame.Machine.sample_rate == 0) return 0;
            if (SEGAPCMInit(msound, intf.bank & 0x00ffffff, intf.mode, Mame.memory_region(intf.region), intf.volume) != 0)
                return 1;
            return 0;
        }
        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void reset()
        {
            //throw new NotImplementedException();
        }
        public override void update()
        {
            throw new NotImplementedException();
        }
        const int SEGA_SAMPLE_RATE = 15800;
        const int SEGA_SAMPLE_SHIFT = 5;
        const int SEGA_SAMPLE_RATE_OLD = 15800 * 2;
        const int SEGA_SAMPLE_SHIFT_OLD = 5;
        static int[][] SEGAPCM_samples = {
	new int[]{ SEGA_SAMPLE_RATE, SEGA_SAMPLE_SHIFT, },
	new int[]{ SEGA_SAMPLE_RATE_OLD, SEGA_SAMPLE_SHIFT_OLD, },
};

        int SEGAPCMInit(Mame.MachineSound msound, int banksize, int mode, _BytePtr inpcm, int volume)
        {
            int i;
            int rate = Mame.Machine.sample_rate;
            buffer_len = (int)(rate / Mame.Machine.drv.frames_per_second);
            emulation_rate = (int)(buffer_len * Mame.Machine.drv.frames_per_second);
            sample_rate = SEGAPCM_samples[mode][0];
            sample_shift = SEGAPCM_samples[mode][1];
            pcm_rom = inpcm;

            //printf( "segaPCM in\n" );

            /**** interface init ****/
            spcm.bankshift = banksize & 0xffffff;
            if ((banksize >> 16) == 0x00)
            {
                spcm.bankmask = (BANK_MASK7 >> 16) & 0x00ff;	/* default */
            }
            else
            {
                spcm.bankmask = (banksize >> 16) & 0x00ff;
            }

            for (i = 0; i < SEGAPCM_MAX; i++)
            {
                spcm.gain[i, L_PAN] = spcm.gain[i, R_PAN] = 0;
                spcm.vol[i, L_PAN] = spcm.vol[i, R_PAN] = 0;
                spcm.addr_l[i] = 0;
                spcm.addr_h[i] = 0;
                spcm.bank[i] = 0;
                spcm.end_h[i] = 0;
                spcm.delta_t[i] = 0x80;
                spcm.flag[i] = 1;
                spcm.add_addr[i] = 0;
                spcm.step[i] = (uint)(((float)sample_rate / (float)emulation_rate) * (float)(0x80 << 5));
                spcm.pcmd[i] = 0;
            }
            //printf( "segaPCM work init end\n" );

            {
                string[] name = new string[LR_PAN];
                int[] vol = new int[2];
                name[0] = Mame.sprintf("%s L", Mame.sound_name(msound));
                name[1] = Mame.sprintf("%s R", Mame.sound_name(msound));
                vol[0] = (Mame.MIXER_PAN_LEFT << 8) | (volume & 0xff);
                vol[1] = (Mame.MIXER_PAN_RIGHT << 8) | (volume & 0xff);
                stream = Mame.stream_init_multi(LR_PAN, name, vol, rate, 0, SEGAPCMUpdate);
            }
            //printf( "segaPCM end\n" );
            return 0;
        }
        static void SEGAPCMUpdate(int num, _ShortPtr[] buffer, int length)
        {
            throw new Exception();
        }
        public const int SEGAPCM_SAMPLE15K = 0, SEGAPCM_SAMPLE32K = 1;

        public const int BANK_256 = 11;
        public const int BANK_512 = 12;
        public const int BANK_12M = 13;
        public const int BANK_MASK7 = 0x70 << 16;
        public const int BANK_MASKF = 0xf0 << 16;
        public const int BANK_MASKF8 = 0xf8 << 16;
        public const int SEGAPCM_MAX = 16;
        public const int L_PAN = 0, R_PAN = 1, LR_PAN = 2;
        class SEGAPCM
        {

            public byte[] writeram = new byte[0x1000];

            public byte[,] gain = new byte[SEGAPCM_MAX, LR_PAN];
            public byte[] addr_l = new byte[SEGAPCM_MAX];
            public byte[] addr_h = new byte[SEGAPCM_MAX];
            public byte[] bank = new byte[SEGAPCM_MAX];
            public byte[] end_h = new byte[SEGAPCM_MAX];
            public byte[] delta_t = new byte[SEGAPCM_MAX];

            public int[,] vol = new int[SEGAPCM_MAX, LR_PAN];

            public uint[] add_addr = new uint[SEGAPCM_MAX];
            public uint[] step = new uint[SEGAPCM_MAX];
            public int[] flag = new int[SEGAPCM_MAX];
            public int bankshift;
            public int bankmask;

            public int[] pcmd = new int[SEGAPCM_MAX];
            public int[] pcma = new int[SEGAPCM_MAX];
        }

        static SEGAPCM spcm = new SEGAPCM();
        static int emulation_rate;
        static int buffer_len;
        static _BytePtr pcm_rom;
        static int sample_rate, sample_shift, stream;

        public static int SEGAPCMReadReg(int r)
        {
            return spcm.writeram[r & 0x07ff];
        }
        public static void SEGAPCMWriteReg(int r, int v)
        {
            int rate;
            int lv, rv, cen;

            int channel = (r >> 3) & 0x0f;

            spcm.writeram[r & 0x07ff] = (byte)v;		/* write value data */

            switch ((r & 0x87))
            {
                case 0x00:
                case 0x01:
                case 0x84:
                case 0x85:
                case 0x87:
                    break;

                case 0x02:
                    spcm.gain[channel, L_PAN] = (byte)(v & 0xff);
                remake_vol:
                    lv = spcm.gain[channel, L_PAN]; rv = spcm.gain[channel, R_PAN];
                    cen = (lv + rv) / 4;
                    //			spcm.vol[channel][L_PAN] = (lv + cen)<<1;
                    //			spcm.vol[channel][R_PAN] = (rv + cen)<<1;
                    spcm.vol[channel, L_PAN] = (lv + cen) * 9 / 5;	// too much clipping
                    spcm.vol[channel, R_PAN] = (rv + cen) * 9 / 5;
                    break;
                case 0x03:
                    spcm.gain[channel, R_PAN] = (byte)(v & 0xff);
                    goto remake_vol;


                case 0x04:
                    spcm.addr_l[channel] = (byte)v;
                    break;
                case 0x05:
                    spcm.addr_h[channel] = (byte)v;
                    break;
                case 0x06:
                    spcm.end_h[channel] = (byte)v;
                    break;
                case 0x07:
                    spcm.delta_t[channel] = (byte)v;
                    rate = (v & 0x00ff) << sample_shift;
                    spcm.step[channel] = (uint)(((float)sample_rate / (float)emulation_rate) * (float)rate);
                    break;
                case 0x86:
                    spcm.bank[channel] = (byte)v;
                    if ((v & 1) != 0) spcm.flag[channel] = 1; /* stop D/A */
                    else
                    {
                        /**** start D/A ****/
                        //				spcm.flag[channel] = 0;
                        spcm.flag[channel] = 2;

                        //				spcm.add_addr[channel] = (( (((int)spcm.addr_h[channel]<<8)&0xff00) |
                        //					  (spcm.addr_l[channel]&0x00ff) ) << PCM_ADDR_SHIFT) &0x0ffff000;
                        spcm.pcmd[channel] = 0;
                    }
                    break;
                /*
                default:
                    printf( "unknown %d = %02x : %02x\n", channel, r, v );
                    break;
                */
            }
        }

    }
}
