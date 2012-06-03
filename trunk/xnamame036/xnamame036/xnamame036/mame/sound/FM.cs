#define FM_LFO_SUPPORT
//#define FM_SEG_SUPPORT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.sound
{
    class FM
    {
        const byte FM_OUTPUT_BIT = 16;


        const byte YM2203_NUMBUF = 1;

#if FM_STEREO_MIX
#else
        const byte YM2151_NUMBUF = 2;
        const byte YM2608_NUMBUF = 2;
        const byte YM2610_NUMBUF = 2;
        const byte YM2612_NUMBUF = 2;
#endif

        delegate void FM_TIMERHANDLER(int n, int c, int cnt, double stepTime);
        delegate void FM_IRQHANDLER(int n, int iq);
        delegate void _EG(FM_SLOT SLOT);
        class FM_SLOT
        {
            int[] DT;			/* detune          :DT_TABLE[DT]       */
            int DT2;			/* multiple,Detune2:(DT2<<4)|ML for OPM*/
            int TL;				/* total level     :TL << 8            */
            byte KSR;			/* key scale rate  :3-KSR              */
            int[] AR;	/* attack rate     :&AR_TABLE[AR<<1]   */
            int[] DR;	/* decay rate      :&DR_TABLE[DR<<1]   */
            int[] SR;	/* sustin rate     :&DR_TABLE[SR<<1]   */
            int SL;			/* sustin level    :SL_TABLE[SL]       */
            int[] RR;	/* release rate    :&DR_TABLE[RR<<2+2] */
            byte SEG;			/* SSG EG type     :SSGEG              */
            byte ksr;			/* key scale rate  :kcode>>(3-KSR)     */
            uint mul;			/* multiple        :ML_TABLE[ML]       */
            /* Phase Generator */
            uint Cnt;			/* frequency count :                   */
            uint Incr;		/* frequency step  :                   */
            /* Envelope Generator */
            _EG eg_next;	/* pointer of phase handler */
            int evc;			/* envelope counter                    */
            int eve;			/* envelope counter end point          */
            int evs;			/* envelope counter step               */
            int evsa;			/* envelope step for Attack            */
            int evsd;			/* envelope step for Decay             */
            int evss;			/* envelope step for Sustain           */
            int evsr;			/* envelope step for Release           */
            int TLL;			/* adjusted TotalLevel                 */
            /* LFO */
            byte amon;			/* AMS enable flag              */
            uint ams;			/* AMS depth level of this SLOT */
        }
        class FM_CH
        {
            public FM_CH()
            {
                for (int i=0;i<4;i++)
                    SLOT[i] = new FM_SLOT();
            }
            FM_SLOT[] SLOT = new FM_SLOT[4];
            byte PAN;			/* PAN :NONE,LEFT,RIGHT or CENTER */
            byte ALGO;			/* Algorythm                      */
            byte FB;			/* shift count of self feed back  */
            int[] op1_out = new int[2];	/* op1 output for beedback        */
            /* Algorythm (connection) */
            int[] connect1;		/* pointer of SLOT1 output    */
            int[] connect2;		/* pointer of SLOT2 output    */
            int[] connect3;		/* pointer of SLOT3 output    */
            int[] connect4;		/* pointer of SLOT4 output    */
            /* LFO */
            int pms;				/* PMS depth level of channel */
            uint ams;				/* AMS depth level of channel */
            /* Phase Generator */
            uint fc;			/* fnum,blk    :adjusted to sampling rate */
            byte fn_h;			/* freq latch  :                   */
            byte kcode;		/* key code    :                   */
        }
        class FM_ST
        {
            byte index;		/* chip index (number of chip) */
            int clock;			/* master clock  (Hz)  */
            int rate;			/* sampling rate (Hz)  */
            double freqbase;	/* frequency base      */
            double TimerBase;	/* Timer base time     */
            byte address;		/* address register    */
            byte irq;			/* interrupt level     */
            byte irqmask;		/* irq mask            */
            byte status;		/* status flag         */
            uint mode;		/* mode  CSM / 3SLOT   */
            int TA;				/* timer a             */
            int TAC;			/* timer a counter     */
            byte TB;			/* timer b             */
            int TBC;			/* timer b counter     */
            /* speedup customize */
            /* local time tables */
            int[,] DT_TABLE = new int[8, 32];	/* DeTune tables       */
            int[] AR_TABLE = new int[94];		/* Atttack rate tables */
            int[] DR_TABLE = new int[94];		/* Decay rate tables   */
            /* Extention Timer and IRQ handler */
            FM_TIMERHANDLER Timer_Handler;
            FM_IRQHANDLER IRQ_Handler;
            /* timer model single / interval */
            byte timermodel;
        }
        class YM_DELTAT
        {
            _BytePtr memory;
            int memory_size;
            double freqbase;
            int[] output_pointer; /* pointer of output pointers */
            int output_range;

            byte[] reg = new byte[16];
            byte portstate, portcontrol;
            int portshift;

            byte flag;          /* port state        */
            byte flagMask;      /* arrived flag mask */
            byte now_data;
            uint now_addr;
            uint now_step;
            uint step;
            uint start;
            uint end;
            uint delta;
            int volume;
            int[] pan;        /* &output_pointer[pan] */
            int /*adpcmm,*/ adpcmx, adpcmd;
            int adpcml;			/* hiro-shi!! */

            /* leveling and re-sampling state for DELTA-T */
            int volume_w_step;   /* volume with step rate */
            int next_leveling;   /* leveling value        */
            int sample_step;     /* step of re-sampling   */

            byte arrivedFlag;    /* flag of arrived end address */
        }
        const int SIN_ENT = 2048;
        const int ENV_BITS = 16;
        const int EG_ENT = 4096;
        const double EG_STEP = 96.0 / EG_ENT;

        static int[] TLTABLE;
        static int[][] SIN_TABLE = new int[SIN_ENT][];
#if FM_SEG_SUPPORT
#else
        static int[] ENV_CURVE = new int[2*EG_ENT+1];
#endif
    }
}
