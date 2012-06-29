using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{

    class YM_DELTAT
    {
        public _BytePtr memory;
        public int memory_size;
        public double freqbase;
        public int[] output_pointer; /* pointer of output pointers */
        public int output_range;

        public byte[] reg = new byte[16];
        public byte portstate, portcontrol;
        public int portshift;

        public byte flag;          /* port state        */
        public byte flagMask;      /* arrived flag mask */
        public byte now_data;
        public uint now_addr;
        public uint now_step;
        public uint step;
        public uint start;
        public uint end;
        public uint delta;
        public int volume;
        public IntSubArray pan;        /* &output_pointer[pan] */
        public int /*adpcmm,*/ adpcmx, adpcmd;
        public int adpcml;			/* hiro-shi!! */

        /* leveling and re-sampling state for DELTA-T */
        public int volume_w_step;   /* volume with step rate */
        public int next_leveling;   /* leveling value        */
        public int sample_step;     /* step of re-sampling   */

        public byte arrivedFlag;    /* flag of arrived end address */
    }
    class YM_DELTA_T
    {
        byte[] ym_deltat_memory;
        static int[] ym_deltat_decode_tableB1 = {
  1,   3,   5,   7,   9,  11,  13,  15,
  -1,  -3,  -5,  -7,  -9, -11, -13, -15,
};
        static int[] ym_deltat_decode_tableB2 = {
  57,  57,  57,  57, 77, 102, 128, 153,
  57,  57,  57,  57, 77, 102, 128, 153
};
        const int YM_DELTAT_DELTA_MAX = 24576;
        const int YM_DELTAT_DELTA_MIN = 127;
        const int YM_DELTAT_DELTA_DEF = 127;

        const int YM_DELTAT_DECODE_RANGE = 32768;
        const int YM_DELTAT_DECODE_MIN = -(YM_DELTAT_DECODE_RANGE);
        const int YM_DELTAT_DECODE_MAX = (YM_DELTAT_DECODE_RANGE) - 1;

        const byte YM_DELTAT_SHIFT = 16;

        public static void YM_DELTAT_ADPCM_Write(YM_DELTAT DELTAT, int r, int v)
        {
            if (r >= 0x10) return;
            DELTAT.reg[r] = (byte)v; /* stock data */

            switch (r)
            {
                case 0x00:	/* START,REC,MEMDATA,REPEAT,SPOFF,--,--,RESET */
#if false
		case 0x60:	/* write buffer MEMORY from PCM data port */
		case 0x20:	/* read  buffer MEMORY to   PCM data port */
#endif
                    if ((v & 0x80) != 0)
                    {
                        DELTAT.portstate = (byte)(v & 0x90); /* start req,memory mode,repeat flag copy */
                        /**** start ADPCM ****/
                        DELTAT.volume_w_step =(int)( (double)DELTAT.volume * DELTAT.step / (1 << YM_DELTAT_SHIFT));
                        DELTAT.now_addr = (DELTAT.start) << 1;
                        DELTAT.now_step = (1 << YM_DELTAT_SHIFT) - DELTAT.step;
                        /*adpcm.adpcmm   = 0;*/
                        DELTAT.adpcmx = 0;
                        DELTAT.adpcml = 0;
                        DELTAT.adpcmd = YM_DELTAT_DELTA_DEF;
                        DELTAT.next_leveling = 0;
                        DELTAT.flag = 1; /* start ADPCM */

                        if (DELTAT.step==0)
                        {
                            DELTAT.flag = 0;
                            DELTAT.portstate = 0x00;
                        }
                        /**** PCM memory check & limit check ****/
                        if (DELTAT.memory == null)
                        {			// Check memory Mapped
                            //Log(LOG_ERR,"YM Delta-T ADPCM rom not mapped\n");
                            DELTAT.flag = 0;
                            DELTAT.portstate = 0x00;
                            //if(errorlog) fprintf(errorlog,"DELTAT memory 0\n");
                        }
                        else
                        {
                            if (DELTAT.end >= DELTAT.memory_size)
                            {		// Check End in Range
                                //Log(LOG_ERR,"YM Delta-T ADPCM end out of range: $%08x\n",DELTAT.end);
                                DELTAT.end = (uint)(DELTAT.memory_size - 1);
                                //if(errorlog) fprintf(errorlog,"DELTAT end over\n");
                            }
                            if (DELTAT.start >= DELTAT.memory_size)
                            {		// Check Start in Range
                                //Log(LOG_ERR,"YM Delta-T ADPCM start out of range: $%08x\n",DELTAT.start);
                                DELTAT.flag = 0;
                                DELTAT.portstate = 0x00;
                                //if(errorlog) fprintf(errorlog,"DELTAT start under\n");
                            }
                        }
                    }
                    else if ((v & 0x01) != 0)
                    {
                        DELTAT.flag = 0;
                        DELTAT.portstate = 0x00;
                    }
                    break;
                case 0x01:	/* L,R,-,-,SAMPLE,DA/AD,RAMTYPE,ROM */
                    DELTAT.portcontrol = (byte)(v & 0xff);
                    DELTAT.pan = new IntSubArray(DELTAT.output_pointer, (v >> 6) & 0x03);
                    break;
                case 0x02:	/* Start Address L */
                case 0x03:	/* Start Address H */
                    DELTAT.start =(uint) (DELTAT.reg[0x3] * 0x0100 | DELTAT.reg[0x2]) << DELTAT.portshift;
                    break;
                case 0x04:	/* Stop Address L */
                case 0x05:	/* Stop Address H */
                    DELTAT.end = (uint)(DELTAT.reg[0x5] * 0x0100 | DELTAT.reg[0x4]) << DELTAT.portshift;
                    DELTAT.end += (uint)(1 << DELTAT.portshift) - 1;
                    break;
                case 0x06:	/* Prescale L (PCM and Recoard frq) */
                case 0x07:	/* Proscale H */
                case 0x08:	/* ADPCM data */
                    break;
                case 0x09:	/* DELTA-N L (ADPCM Playback Prescaler) */
                case 0x0a:	/* DELTA-N H */
                    DELTAT.delta = (uint)(DELTAT.reg[0xa] * 0x0100 | DELTAT.reg[0x9]);
                    DELTAT.step = (uint)((double)(DELTAT.delta * (1 << (YM_DELTAT_SHIFT - 16))) * (DELTAT.freqbase));
                    DELTAT.volume_w_step = (int)((double)DELTAT.volume * DELTAT.step / (1 << YM_DELTAT_SHIFT));
                    break;
                case 0x0b:	/* Level control (volume , voltage flat) */
                    {
                        int oldvol = DELTAT.volume;
                        DELTAT.volume = (v & 0xff) * (DELTAT.output_range / 256) / YM_DELTAT_DECODE_RANGE;
                        if (oldvol != 0)
                        {
                            DELTAT.adpcml = (int)((double)DELTAT.adpcml / (double)oldvol * (double)DELTAT.volume);
                            DELTAT.sample_step = (int)((double)DELTAT.sample_step / (double)oldvol * (double)DELTAT.volume);
                        }
                        DELTAT.volume_w_step = (int)((double)DELTAT.volume * (double)DELTAT.step / (double)(1 << YM_DELTAT_SHIFT));
                    }
                    break;
            }
        }

        public static void YM_DELTAT_ADPCM_Reset(YM_DELTAT DELTAT, int pan)
        {
            DELTAT.now_addr = 0;
            DELTAT.now_step = 0;
            DELTAT.step = 0;
            DELTAT.start = 0;
            DELTAT.end = 0;
            /* F2610.adpcm[i].delta     = 21866; */
            DELTAT.volume = 0;
            DELTAT.pan = new IntSubArray(DELTAT.output_pointer, pan);
            /* DELTAT.flagMask  = 0; */
            DELTAT.arrivedFlag = 0;
            DELTAT.flag = 0;
            DELTAT.adpcmx = 0;
            DELTAT.adpcmd = 127;
            DELTAT.adpcml = 0;
            /*DELTAT.adpcmm    = 0;*/
            DELTAT.volume_w_step = 0;
            DELTAT.next_leveling = 0;
            DELTAT.portstate = 0;
            /* DELTAT.portshift = 8; */
        }
    }
}
