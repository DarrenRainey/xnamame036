#define FM_LFO_SUPPORT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public partial class fm
    {
        const int OUTD_RIGHT = 1, OUTD_LEFT = 2, OUTD_CENTER = 3;
        const int FM_TIMER_SINGLE = 0, FM_TIMER_INTERVAL = 1;

        /* some globals */
        const byte TYPE_SSG = 0x01;   /* SSG support          */
        const byte TYPE_OPN = 0x02;   /* OPN device           */
        const byte TYPE_LFOPAN = 0x04;   /* OPN type LFO and PAN */
        const byte TYPE_6CH = 0x08;   /* FM 6CH / 3CH         */
        const byte TYPE_DAC = 0x10;   /* YM2612's DAC device  */
        const byte TYPE_ADPCM = 0x20;   /* two ADPCM unit       */

        const byte TYPE_YM2203 = (TYPE_SSG);
        const byte TYPE_YM2608 = (TYPE_SSG | TYPE_LFOPAN | TYPE_6CH | TYPE_ADPCM);
        const byte TYPE_YM2610 = (TYPE_SSG | TYPE_LFOPAN | TYPE_6CH | TYPE_ADPCM);
        const byte TYPE_YM2612 = (TYPE_6CH | TYPE_LFOPAN | TYPE_DAC);

        const int SLOT1 = 0, SLOT2 = 2, SLOT3 = 1, SLOT4 = 3;

        /* current chip state */
        static object fm_cur_chip = null;		/* pointer of current chip struct */
        static FM_ST State;			/* basic status */
        static FM_CH[] cch = new FM_CH[8];			/* pointer of FM channels */
#if FM_LFO_SUPPORT
        static uint LFOCnt, LFOIncr;	/* LFO PhaseGenerator */
#endif

        /* runtime work */
        static int[] out_ch = new int[4];		/* channel output NONE,LEFT,RIGHT or CENTER */
        static int[] pg_in1 = new int[1], pg_in2 = new int[1], pg_in3 = new int[1], pg_in4 = new int[1];	/* PG input of SLOTs */

#if FM_LFO_SUPPORT

        const int PMS_RATE = 0x400;

        /* LFO runtime work */
        static IntSubArray LFO_wave;
        static uint lfo_amd;
        static int lfo_pmd;
#endif

        const int SIN_ENT = 2048;
        const int VIB_ENT = 512;
        const int VIB_SHIFT = 32 - 9;
        const int AMS_SHIFT = 32 - 9;
        const int AMS_ENT = 512;
        const int VIB_RATE=256;

        const int EG_AST = 0;
        const int EG_AED = EG_ENT << ENV_BITS;
        const int EG_DST = EG_AED;
        const int EG_DED = (EG_DST + ((EG_ENT - 1) << ENV_BITS));
        const int EG_OFF = EG_DED;
#if FM_SEG_SUPPORT
const int  EG_UST  = ((2*EG_ENT)<<ENV_BITS);  /* start of SEG UPSISE */
const int  EG_UED  = ((3*EG_ENT)<<ENV_BITS);  /* end of SEG UPSISE */
#endif

        /* lower bits of envelope counter */
        const int ENV_BITS = 16;

        /* envelope output entries */
        const int EG_ENT = 4096;
        const double EG_STEP = (96.0 / EG_ENT); /* OPL == 0.1875 dB */
        /* attack/decay rate time rate */
        const int OPM_ARRATE = 399128;
        const int OPM_DRRATE = 5514396;
        /* It is not checked , because I haven't YM2203 rate */
        const int OPN_ARRATE = OPM_ARRATE;
        const int OPN_DRRATE = OPM_DRRATE;

        /* PG output cut off level : 78dB(14bit)? */
        const int PG_CUT_OFF = ((int)(78.0 / EG_STEP));
        /* EG output cut off level : 68dB? */
        const int EG_CUT_OFF = ((int)(68.0 / EG_STEP));

        const int FREQ_BITS = 24;		/* frequency turn          */

        /* PG counter is 21bits @oct.7 */
        //const int FREQ_RATE = (1 << (FREQ_BITS - 21));
        //const int TL_BITS = (FREQ_BITS + 2);
        /* OPbit = 14(13+sign) : TL_BITS+1(sign) / output = 16bit */
        const int TL_SHIFT = ((FREQ_BITS + 2) + 1 - (14 - 16));

        const int FM_OUTPUT_BIT = 16;
        /* output final shift */
        const int FM_OUTSB = (TL_SHIFT - FM_OUTPUT_BIT);
        const int FM_MAXOUT = ((1 << (TL_SHIFT - 1)) - 1);
        const int FM_MINOUT = (-(1 << (TL_SHIFT - 1)));
        const int TL_MAX = (PG_CUT_OFF + EG_CUT_OFF + 1);
        static int[] TL_TABLE;

        static IntSubArray[] SIN_TABLE = new IntSubArray[SIN_ENT];
        static int[] AMS_TABLE, VIB_TABLE, ENV_CURVE = new int[2 * EG_ENT + 1];

        /* envelope output curve table */
#if FM_SEG_SUPPORT
/* attack + decay + SSG upside + OFF */
static int ENV_CURVE[3*EG_ENT+1];
#else
        
        
#endif
        /* envelope counter conversion table when change Decay to Attack phase */
        static int[] DRAR_TABLE = new int[EG_ENT];

        static int[] SL_TABLE ={
 (int)(0*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(1*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(2*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(3*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(4*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(5*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(6*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(7*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,
 (int)(8*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(9*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(10*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(11*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(12*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(13*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(14*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST,(int)(31*((3/EG_STEP)*(1<<ENV_BITS)))+EG_DST
};

        //#define OPM_DTTABLE OPN_DTTABLE
        static byte[] OPN_DTTABLE ={
/* this table is YM2151 and YM2612 data */
/* FD=0 */
  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
/* FD=1 */
  0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2,
  2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 8, 8, 8, 8,
/* FD=2 */
  1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5,
  5, 6, 6, 7, 8, 8, 9,10,11,12,13,14,16,16,16,16,
/* FD=3 */
  2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7,
  8 , 8, 9,10,11,12,13,14,16,17,19,20,22,22,22,22
};
        static int[] MUL_TABLE = {
/* 1/2, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15 */
   (int)(0.50*2),(int)(1.00*2),(int)(2.00*2),(int)(3.00*2),(int)(4.00*2),(int)(5.00*2),(int)(6.00*2),(int)(7.00*2),
   (int)(8.00*2),(int)(9.00*2),(int)(10.00*2),(int)(11.00*2),(int)(12.00*2),(int)(13.00*2),(int)(14.00*2),(int)(15.00*2),
/* DT2=1 *SQL(2)   */
   (int)(0.71*2),(int)(1.41*2),(int)(2.82*2),(int)(4.24*2),(int)(5.65*2),(int)(7.07*2),(int)(8.46*2),(int)(9.89*2),
   (int)(11.30*2),(int)(12.72*2),(int)(14.10*2),(int)(15.55*2),(int)(16.96*2),(int)(18.37*2),(int)(19.78*2),(int)(21.20*2),
/* DT2=2 *SQL(2.5) */
   (int)(0.78*2),(int)(1.57*2),(int)(3.14*2),(int)(4.71*2),(int)(6.28*2),(int)(7.85*2),(int)(9.42*2),(int)(10.99*2),
   (int)(12.56*2),(int)(14.13*2),(int)(15.70*2),(int)(17.27*2),(int)(18.84*2),(int)(20.41*2),(int)(21.98*2),(int)(23.55*2),
/* DT2=3 *SQL(3)   */
   (int)(0.87*2),(int)(1.73*2),(int)(3.46*2),(int)(5.19*2),(int)(6.92*2),(int)(8.65*2),(int)(10.38*2),(int)(12.11*2),
   (int)(13.84*2),(int)(15.57*2),(int)(17.30*2),(int)(19.03*2),(int)(20.76*2),(int)(22.49*2),(int)(24.22*2),(int)(25.95*2)
};

        public delegate void FM_TIMERHANDLER(int n, int c, int ctn, double stepTime);
        public delegate void FM_IRQHANDLER(int n, int irq);
        class FM_ST
        {
            public byte index;		/* chip index (number of chip) */
            public int clock;			/* master clock  (Hz)  */
            public int rate;			/* sampling rate (Hz)  */
            public double freqbase;	/* frequency base      */
            public double TimerBase;	/* Timer base time     */
            public byte address;		/* address register    */
            public byte irq;			/* interrupt level     */
            public byte irqmask;		/* irq mask            */
            public byte status;		/* status flag         */
            public uint mode;		/* mode  CSM / 3SLOT   */
            public int TA;				/* timer a             */
            public int TAC;			/* timer a counter     */
            public byte TB;			/* timer b             */
            public int TBC;			/* timer b counter     */
            /* speedup customize */
            /* local time tables */
            public int[][] DT_TABLE;//[8][32];	/* DeTune tables       */
            public int[] AR_TABLE = new int[94];		/* Atttack rate tables */
            public int[] DR_TABLE = new int[94];		/* Decay rate tables   */
            /* Extention Timer and IRQ handler */
            public FM_TIMERHANDLER Timer_Handler;
            public FM_IRQHANDLER IRQ_Handler;
            /* timer model single / interval */
            public byte timermodel;

            public FM_ST()
            {
                DT_TABLE = new int[8][];
                for (int i = 0; i < 8; i++) DT_TABLE[i] = new int[32];
            }
        }
        delegate void _eg_next(FM_SLOT SLOT);
        class FM_SLOT
        {
            public int[] DT;			/* detune          :DT_TABLE[DT]       */
            public int DT2;			/* multiple,Detune2:(DT2<<4)|ML for OPM*/
            public int TL;				/* total level     :TL << 8            */
            public byte KSR;			/* key scale rate  :3-KSR              */
            public IntSubArray AR;	/* attack rate     :&AR_TABLE[AR<<1]   */
            public IntSubArray DR;	/* decay rate      :&DR_TABLE[DR<<1]   */
            public IntSubArray SR;	/* sustin rate     :&DR_TABLE[SR<<1]   */
            public int SL;			/* sustin level    :SL_TABLE[SL]       */
            public IntSubArray RR;	/* release rate    :&DR_TABLE[RR<<2+2] */
            public byte SEG;			/* SSG EG type     :SSGEG              */
            public byte ksr;			/* key scale rate  :kcode>>(3-KSR)     */
            public uint mul;			/* multiple        :ML_TABLE[ML]       */
            /* Phase Generator */
            public uint Cnt;			/* frequency count :                   */
            public uint Incr;		/* frequency step  :                   */
            /* Envelope Generator */
            public _eg_next eg_next;	/* pointer of phase handler */
            public int evc;			/* envelope counter                    */
            public int eve;			/* envelope counter end point          */
            public int evs;			/* envelope counter step               */
            public int evsa;			/* envelope step for Attack            */
            public int evsd;			/* envelope step for Decay             */
            public int evss;			/* envelope step for Sustain           */
            public int evsr;			/* envelope step for Release           */
            public int TLL;			/* adjusted TotalLevel                 */
            /* LFO */
            public byte amon;			/* AMS enable flag              */
            public uint ams;			/* AMS depth level of this SLOT */
        }
        class FM_CH
        {
            public FM_CH()
            {
                for (int i = 0; i < 4; i++)
                {
                    SLOT[i] = new FM_SLOT();
                }
            }
            public FM_SLOT[] SLOT = new FM_SLOT[4];
            public byte PAN;			/* PAN :NONE,LEFT,RIGHT or CENTER */
            public byte ALGO;			/* Algorythm                      */
            public byte FB;			/* shift count of self feed back  */
            public int[] op1_out = new int[2];	/* op1 output for beedback        */
            /* Algorythm (connection) */
            public IntSubArray connect1;		/* pointer of SLOT1 output    */
            public IntSubArray connect2;		/* pointer of SLOT2 output    */
            public IntSubArray connect3;		/* pointer of SLOT3 output    */
            public IntSubArray connect4;		/* pointer of SLOT4 output    */
            /* LFO */
            public int pms;				/* PMS depth level of channel */
            public uint ams;				/* AMS depth level of channel */
            /* Phase Generator */
            public uint fc;			/* fnum,blk    :adjusted to sampling rate */
            public byte fn_h;			/* freq latch  :                   */
            public byte kcode;		/* key code    :                   */
        }
        class FM_3SLOT
        {
            public uint[] fc = new uint[3];		/* fnum3,blk3  :calcrated */
            public byte[] fn_h = new byte[3];		/* freq3 latch            */
            public byte[] kcode = new byte[3];		/* key code    :          */
        };
        const int LFO_ENT = 512;
        const int LFO_SHIFT = 32 - 9;
        const int LFO_RATE = 0x10000;

        class FM_OPN
        {
            public byte type;		/* chip type         */
            public FM_ST ST = new FM_ST();				/* general state     */
            public FM_3SLOT SL3 = new FM_3SLOT();			/* 3 slot mode state */
            public FM_CH[] P_CH;			/* pointer of CH     */
            public uint[] FN_TABLE = new uint[2048]; /* fnumber . increment counter */
#if FM_LFO_SUPPORT
            /* LFO */
            public uint LFOCnt;
            public uint LFOIncr;
            public uint[] LFO_FREQ = new uint[8];/* LFO FREQ table */
            public int[] LFO_wave = new int[LFO_ENT];
#endif
        }
        class YM2203
        {
            public FM_OPN OPN = new FM_OPN();
            public FM_CH[] CH = new FM_CH[3];
            public YM2203()
            {
                for (int i = 0; i < 3; i++) CH[i] = new FM_CH();
            }
        }
        static int[] RATE_0 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static byte[] OPN_FKTABLE = { 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 3, 3, 3, 3, 3, 3 };

        static YM2203[] FM2203 = null;	/* array of YM2203's */
        static YM2151[] FMOPM = null; /* array of 2151's */
        static int YM2151NumChips;

        static int YM2203NumChips;	/* total chip */
        public static byte YM2203Read(int n, int a)
        {
            YM2203 F2203 = FM2203[n];
            int addr = F2203.OPN.ST.address;
            int ret = 0;

            if ((a & 1) == 0)
            {	/* status port */
                ret = F2203.OPN.ST.status;
            }
            else
            {	/* data port (ONLY SSG) */
                if (addr < 16) ret = ay8910.AY8910Read(n);
            }
            return (byte)ret;
        }
        public static int YM2203Write(int n, int a, byte v)
        {
            FM_OPN OPN = FM2203[n].OPN;

            if ((a & 1) == 0)
            {	/* address port */
                OPN.ST.address = (byte)(v & 0xff);
                /* Write register to SSG emurator */
                if (v < 16) ay8910.AY8910Write(n, 0, v);
                switch (OPN.ST.address)
                {
                    case 0x2d:	/* divider sel */
                        OPNSetPris(OPN, 6 * 12, 6 * 12, 4); /* OPN 1/6 , SSG 1/4 */
                        break;
                    case 0x2e:	/* divider sel */
                        OPNSetPris(OPN, 3 * 12, 3 * 12, 2); /* OPN 1/3 , SSG 1/2 */
                        break;
                    case 0x2f:	/* divider sel */
                        OPNSetPris(OPN, 2 * 12, 2 * 12, 1); /* OPN 1/2 , SSG 1/1 */
                        break;
                }
            }
            else
            {	/* data port */
                int addr = OPN.ST.address;
                switch (addr & 0xf0)
                {
                    case 0x00:	/* 0x00-0x0f : SSG section */
                        /* Write data to SSG emurator */
                        ay8910.AY8910Write(n, a, v);
                        break;
                    case 0x20:	/* 0x20-0x2f : Mode section */
                        ym2203.YM2203UpdateRequest(n);
                        /* write register */
                        OPNWriteMode(OPN, addr, v);
                        break;
                    default:	/* 0x30-0xff : OPN section */
                        ym2203.YM2203UpdateRequest(n);
                        /* write register */
                        OPNWriteReg(OPN, addr, v);
                        break;
                }
            }
            return OPN.ST.irq;
        }
        static void FM_STATUS_SET(FM_ST ST, int flag)
        {
            /* set status flag */
            ST.status |= (byte)flag;
            if ((ST.irq) == 0 && (ST.status & ST.irqmask) != 0)
            {
                ST.irq = 1;
                /* callback user interrupt handler (IRQ is OFF to ON) */
                if (ST.IRQ_Handler != null) ST.IRQ_Handler(ST.index, 1);
            }
        }
        static void FM_STATUS_RESET(FM_ST ST, int flag)
        {
            /* reset status flag */
            ST.status &= (byte)~flag;
            if ((ST.irq) != 0 && (ST.status & ST.irqmask) == 0)
            {
                ST.irq = 0;
                /* callback user interrupt handler (IRQ is ON to OFF) */
                if (ST.IRQ_Handler != null) ST.IRQ_Handler(ST.index, 0);
            }
        }
        static void TimerAOver(FM_ST ST)
        {
            /* status set if enabled */
            if ((ST.mode & 0x04) != 0) FM_STATUS_SET(ST, 0x01);
            /* clear or reload the counter */
            if (ST.timermodel == FM_TIMER_INTERVAL)
            {
                ST.TAC = (1024 - ST.TA);
                if (ST.Timer_Handler != null) ST.Timer_Handler(ST.index, 0, (int)(double)ST.TAC, ST.TimerBase);
            }
            else ST.TAC = 0;
        }
        static void TimerBOver(FM_ST ST)
        {
            /* status set if enabled */
            if ((ST.mode & 0x08) != 0) FM_STATUS_SET(ST, 0x02);
            /* clear or reload the counter */
            if (ST.timermodel == FM_TIMER_INTERVAL)
            {
                ST.TBC = (256 - ST.TB) << 4;
                if (ST.Timer_Handler != null) ST.Timer_Handler(ST.index, 1, (int)(double)ST.TBC, ST.TimerBase);
            }
            else ST.TBC = 0;
        }
        static void CSMKeyControll(FM_CH CH)
        {
            /* int ksl = KSL[CH.kcode]; */
            /* all key off */
            FM_KEYOFF(CH, SLOT1);
            FM_KEYOFF(CH, SLOT2);
            FM_KEYOFF(CH, SLOT3);
            FM_KEYOFF(CH, SLOT4);
            /* total level latch */
            CH.SLOT[SLOT1].TLL = CH.SLOT[SLOT1].TL /*+ ksl*/;
            CH.SLOT[SLOT2].TLL = CH.SLOT[SLOT2].TL /*+ ksl*/;
            CH.SLOT[SLOT3].TLL = CH.SLOT[SLOT3].TL /*+ ksl*/;
            CH.SLOT[SLOT4].TLL = CH.SLOT[SLOT4].TL /*+ ksl*/;
            /* all key on */
            FM_KEYON(CH, SLOT1);
            FM_KEYON(CH, SLOT2);
            FM_KEYON(CH, SLOT3);
            FM_KEYON(CH, SLOT4);
        }
        public static int YM2203TimerOver(int n, int c)
        {
            YM2203 F2203 = FM2203[n];

            if (c != 0)
            {	/* Timer B */
                TimerBOver(F2203.OPN.ST);
            }
            else
            {	/* Timer A */
                ym2203.YM2203UpdateRequest(n);
                /* timer update */
                TimerAOver(F2203.OPN.ST);
                /* CSM mode key,TL controll */
                if ((F2203.OPN.ST.mode & 0x80) != 0)
                {	/* CSM mode total level latch and auto key on */
                    CSMKeyControll(F2203.CH[2]);
                }
            }
            return F2203.OPN.ST.irq;
        }
        public static int YM2203Init(int num, int clock, int rate, FM_TIMERHANDLER TimerHandler, FM_IRQHANDLER IRQHandler)
        {
            int i;

            if (FM2203 != null) return (-1);	/* duplicate init. */
            fm_cur_chip = null;	/* hiro-shi!! */

            YM2203NumChips = num;

            /* allocate ym2203 state space */
            FM2203 = new YM2203[YM2203NumChips];

            /* clear */
            //memset(FM2203, 0, sizeof(YM2203) * YM2203NumChips);
            /* allocate total level table (128kb space) */
            FMInitTable();

            for (i = 0; i < YM2203NumChips; i++)
            {
                FM2203[i] = new YM2203();
                FM2203[i].OPN.ST.index = (byte)i;
                FM2203[i].OPN.type = TYPE_YM2203;
                FM2203[i].OPN.P_CH = FM2203[i].CH;
                FM2203[i].OPN.ST.clock = clock;
                FM2203[i].OPN.ST.rate = rate;
                /* FM2203[i].OPN.ST.irq = 0; */
                /* FM2203[i].OPN.ST.satus = 0; */
                FM2203[i].OPN.ST.timermodel = FM_TIMER_INTERVAL;
                /* Extend handler */
                FM2203[i].OPN.ST.Timer_Handler = TimerHandler;
                FM2203[i].OPN.ST.IRQ_Handler = IRQHandler;
                YM2203ResetChip(i);
            }
            return (0);
        }
        public static void YM2203Shutdown()
        {
            if (FM2203 == null) return;

            FMCloseTable();
            FM2203 = null;
        }
        static void FMCloseTable()
        {
            TL_TABLE = null;
        }

        static void init_timetables(ref FM_ST ST, byte[] DTTABLE, int ARRATE, int DRRATE)
        {
            int i, d;
            double rate;

            /* DeTune table */
            for (d = 0; d <= 3; d++)
            {
                for (i = 0; i <= 31; i++)
                {
                    rate = (double)DTTABLE[d * 32 + i] * ST.freqbase * FREQ_RATE;
                    ST.DT_TABLE[d][i] = (int)rate;
                    ST.DT_TABLE[d + 4][i] = (int)-rate;
                }
            }
            /* make Attack & Decay tables */
            for (i = 0; i < 4; i++) ST.AR_TABLE[i] = ST.DR_TABLE[i] = 0;
            for (i = 4; i < 64; i++)
            {
                rate = ST.freqbase;						/* frequency rate */
                if (i < 60) rate *= 1.0 + (i & 3) * 0.25;		/* b0-1 : x1 , x1.25 , x1.5 , x1.75 */
                rate *= 1 << ((i >> 2) - 1);						/* b2-5 : shift bit */
                rate *= (double)(EG_ENT << ENV_BITS);
                ST.AR_TABLE[i] = (int)(rate / ARRATE);
                ST.DR_TABLE[i] = (int)(rate / DRRATE);
            }
            ST.AR_TABLE[62] = EG_AED - 1;
            ST.AR_TABLE[63] = EG_AED - 1;
            for (i = 64; i < 94; i++)
            {	/* make for overflow area */
                ST.AR_TABLE[i] = ST.AR_TABLE[63];
                ST.DR_TABLE[i] = ST.DR_TABLE[63];
            }

#if false
	for (i = 0;i < 64 ;i++){
		Log(LOG_WAR,"rate %2d , ar %f ms , dr %f ms \n",i,
			((double)(EG_ENT<<ENV_BITS) / ST.AR_TABLE[i]) * (1000.0 / ST.rate),
			((double)(EG_ENT<<ENV_BITS) / ST.DR_TABLE[i]) * (1000.0 / ST.rate) );
	}
#endif
        }
        static void SSGClk(int chip, int clock) { ay8910.AY8910_set_clock(chip, clock); }
        static void OPNSetPris(FM_OPN OPN, int pris, int TimerPris, int SSGpris)
        {
            int i;

            /* frequency base */
            OPN.ST.freqbase = (OPN.ST.rate) != 0 ? ((double)OPN.ST.clock / OPN.ST.rate) / pris : 0;
            /* Timer base time */
            OPN.ST.TimerBase = 1.0 / ((double)OPN.ST.clock / (double)TimerPris);
            /* SSG part  priscaler set */
            if (SSGpris != 0) SSGClk(OPN.ST.index, OPN.ST.clock * 2 / SSGpris);
            /* make time tables */
            init_timetables(ref OPN.ST, OPN_DTTABLE, OPN_ARRATE, OPN_DRRATE);
            /* make fnumber . increment counter table */
            for (i = 0; i < 2048; i++)
            {
                /* it is freq table for octave 7 */
                /* opn freq counter = 20bit */
                OPN.FN_TABLE[i] = (uint)((double)i * OPN.ST.freqbase * FREQ_RATE * (1 << 7) / 2);
            }
#if FM_LFO_SUPPORT
            /* LFO wave table */
            for (i = 0; i < LFO_ENT; i++)
            {
                OPN.LFO_wave[i] = i < LFO_ENT / 2 ? i * LFO_RATE / (LFO_ENT / 2) : (LFO_ENT - i) * LFO_RATE / (LFO_ENT / 2);
            }
            /* LFO freq. table */
            {
                /* 3.98Hz,5.56Hz,6.02Hz,6.37Hz,6.88Hz,9.63Hz,48.1Hz,72.2Hz @ 8MHz */
                double[] freq_table = { 3.98, 5.56, 6.02, 6.37, 6.88, 9.63, 48.1, 72.2 };
                for (i = 0; i < 8; i++)
                {
                    OPN.LFO_FREQ[i] = (uint)((OPN.ST.rate) != 0 ? ((double)LFO_ENT * (1 << LFO_SHIFT)
                            / (OPN.ST.rate / freq_table[i]
                            * (OPN.ST.freqbase * OPN.ST.rate / (8000000.0 / 144)))) : 0);

                }
            }
#endif
            /*	Log(LOG_INF,"OPN %d set priscaler %d\n",OPN.ST.index,pris);*/
        }
        static void SSGReset(int chip) { ay8910.AY8910_reset(chip); }
        static void FM_IRQMASK_SET(ref FM_ST ST, int flag)
        {
            ST.irqmask = (byte)flag;
            /* IRQ handling check */
            FM_STATUS_SET(ST, 0);
            FM_STATUS_RESET(ST, 0);
        }
        static void FMSetMode(ref FM_ST ST, int n, int v)
        {
            /* b7 = CSM MODE */
            /* b6 = 3 slot mode */
            /* b5 = reset b */
            /* b4 = reset a */
            /* b3 = timer enable b */
            /* b2 = timer enable a */
            /* b1 = load b */
            /* b0 = load a */
            ST.mode = (uint)v;

            /* reset Timer b flag */
            if ((v & 0x20) != 0)
                FM_STATUS_RESET(ST, 0x02);
            /* reset Timer a flag */
            if ((v & 0x10) != 0)
                FM_STATUS_RESET(ST, 0x01);
            /* load b */
            if ((v & 0x02) != 0)
            {
                if (ST.TBC == 0)
                {
                    ST.TBC = (256 - ST.TB) << 4;
                    /* External timer handler */
                    if (ST.Timer_Handler != null) ST.Timer_Handler(n, 1, (int)(double)ST.TBC, ST.TimerBase);
                }
            }
            else if (ST.timermodel == FM_TIMER_INTERVAL)
            {	/* stop interbval timer */
                if (ST.TBC != 0)
                {
                    ST.TBC = 0;
                    if (ST.Timer_Handler != null) ST.Timer_Handler(n, 1, 0, ST.TimerBase);
                }
            }
            /* load a */
            if ((v & 0x01) != 0)
            {
                if (ST.TAC == 0)
                {
                    ST.TAC = (1024 - ST.TA);
                    /* External timer handler */
                    if (ST.Timer_Handler != null) ST.Timer_Handler(n, 0, (int)(double)ST.TAC, ST.TimerBase);
                }
            }
            else if (ST.timermodel == FM_TIMER_INTERVAL)
            {	/* stop interbval timer */
                if (ST.TAC != 0)
                {
                    ST.TAC = 0;
                    if (ST.Timer_Handler != null) ST.Timer_Handler(n, 0, 0, ST.TimerBase);
                }
            }
        }
        static void FM_EG_SR(FM_SLOT SLOT)
        {
            SLOT.evc = EG_OFF;
            SLOT.eve = EG_OFF + 1;
            SLOT.evs = 0;
        }
        static void FM_EG_DR(FM_SLOT SLOT)
        {
            SLOT.eg_next = FM_EG_SR;
            SLOT.evc = SLOT.SL;
            SLOT.eve = EG_DED;
            SLOT.evs = SLOT.evss;
        }
        static void FM_EG_AR(FM_SLOT SLOT)
        {
            /* next DR */
            SLOT.eg_next = FM_EG_DR;
            SLOT.evc = EG_DST;
            SLOT.eve = SLOT.SL;
            SLOT.evs = SLOT.evsd;
        }
        static void FM_EG_Release(FM_SLOT SLOT)
        {
            SLOT.evc = EG_OFF;
            SLOT.eve = EG_OFF + 1;
            SLOT.evs = 0;
        }
        static bool FM_KEY_IS(FM_SLOT SLOT) { return SLOT.eg_next != FM_EG_Release; }
        static void FM_KEYON(FM_CH CH, int s)
        {
            FM_SLOT SLOT = CH.SLOT[s];
            if (!FM_KEY_IS(SLOT))
            {
                /* restart Phage Generator */
                SLOT.Cnt = 0;
                /* phase . Attack */
#if FM_SEG_SUPPORT
		if( SLOT.SEG&8 ) SLOT.eg_next = FM_EG_SSG_AR;
		else
#endif
                SLOT.eg_next = FM_EG_AR;
                SLOT.evs = SLOT.evsa;
#if false
		/* convert decay count to attack count */
		/* --- This caused the problem by credit sound of paper boy. --- */
		SLOT.evc = EG_AST + DRAR_TABLE[ENV_CURVE[SLOT.evc>>ENV_BITS]];/* + SLOT.evs;*/
#else
                /* reset attack counter */
                SLOT.evc = EG_AST;
#endif
                SLOT.eve = EG_AED;
            }
        }
        static void FM_KEYOFF(FM_CH CH, int s)
        {
            FM_SLOT SLOT = CH.SLOT[s];
            if (FM_KEY_IS(SLOT))
            {
                /* if Attack phase then adjust envelope counter */
                if (SLOT.evc < EG_DST)
                    SLOT.evc = (ENV_CURVE[SLOT.evc >> ENV_BITS] << ENV_BITS) + EG_DST;
                /* phase . Release */
                SLOT.eg_next = FM_EG_Release;
                SLOT.eve = EG_DED;
                SLOT.evs = SLOT.evsr;
            }
        }
        static void OPNWriteMode(FM_OPN OPN, int r, int v)
        {
            byte c;
            FM_CH CH;

            switch (r)
            {
                case 0x21:	/* Test */
                    break;
#if FM_LFO_SUPPORT
                case 0x22:	/* LFO FREQ (YM2608/YM2612) */
                    if ((OPN.type & TYPE_LFOPAN) != 0)
                    {
                        OPN.LFOIncr = (v & 0x08) != 0 ? OPN.LFO_FREQ[v & 7] : 0;
                        fm_cur_chip = null;
                    }
                    break;
#endif
                case 0x24:	/* timer A High 8*/
                    OPN.ST.TA = (OPN.ST.TA & 0x03) | (((int)v) << 2);
                    break;
                case 0x25:	/* timer A Low 2*/
                    OPN.ST.TA = (OPN.ST.TA & 0x3fc) | (v & 3);
                    break;
                case 0x26:	/* timer B */
                    OPN.ST.TB = (byte)v;
                    break;
                case 0x27:	/* mode , timer controll */
                    FMSetMode(ref OPN.ST, OPN.ST.index, v);
                    break;
                case 0x28:	/* key on / off */
                    c = (byte)(v & 0x03);
                    if (c == 3) break;
                    if ((v & 0x04) != 0 && (OPN.type & TYPE_6CH) != 0) c += 3;
                    //CH = OPN.P_CH[c];
                    CH = OPN.P_CH[c];
                    /* csm mode */
                    if (c == 2 && (OPN.ST.mode & 0x80) != 0) break;
                    if ((v & 0x10) != 0) FM_KEYON(CH, SLOT1); else FM_KEYOFF(CH, SLOT1);
                    if ((v & 0x20) != 0) FM_KEYON(CH, SLOT2); else FM_KEYOFF(CH, SLOT2);
                    if ((v & 0x40) != 0) FM_KEYON(CH, SLOT3); else FM_KEYOFF(CH, SLOT3);
                    if ((v & 0x80) != 0) FM_KEYON(CH, SLOT4); else FM_KEYOFF(CH, SLOT4);
                    /*		Log(LOG_INF,"OPN %d:%d : KEY %02X\n",n,c,v&0xf0);*/
                    break;
            }
        }
        static void reset_channel(FM_ST ST, FM_CH[] CH, int chan)
        {
            int c, s;

            ST.mode = 0;	/* normal mode */
            FM_STATUS_RESET(ST, 0xff);
            ST.TA = 0;
            ST.TAC = 0;
            ST.TB = 0;
            ST.TBC = 0;

            for (c = 0; c < chan; c++)
            {
                CH[c] = new FM_CH();
                CH[c].fc = 0;
                CH[c].PAN = OUTD_CENTER;
                for (s = 0; s < 4; s++)
                {
                    CH[c].SLOT[s].SEG = 0;
                    CH[c].SLOT[s].eg_next = FM_EG_Release;
                    CH[c].SLOT[s].evc = EG_OFF;
                    CH[c].SLOT[s].eve = EG_OFF + 1;
                    CH[c].SLOT[s].evs = 0;
                }
            }
        }
        static int OPN_CHAN(int n) { return n & 3; }
        static int OPN_SLOT(int n) { return (n >> 2) & 3; }
        static void set_det_mul(FM_ST ST, FM_CH CH, FM_SLOT SLOT, int v)
        {
            SLOT.mul = (uint)(MUL_TABLE[v & 0x0f]);
            SLOT.DT = ST.DT_TABLE[(v >> 4) & 7];
            CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
        }
        static void set_tl(FM_CH CH, FM_SLOT SLOT, int v, bool csmflag)
        {
            v &= 0x7f;
            v = (v << 7) | v; /* 7bit . 14bit */
            SLOT.TL = (v * EG_ENT) >> 14;
            if (!csmflag)
            {	/* not CSM latch total level */
                SLOT.TLL = SLOT.TL /* + KSL[CH.kcode] */;
            }
        }
        static void set_ar_ksr(FM_CH CH, FM_SLOT SLOT, int v, int[] ar_table)
        {
            SLOT.KSR = (byte)(3 - (v >> 6));
            SLOT.AR = (v &= 0x1f) != 0 ? new IntSubArray(ar_table, v << 1) : new IntSubArray(RATE_0);
            SLOT.evsa = SLOT.AR[SLOT.ksr];
            if (SLOT.eg_next == FM_EG_AR) SLOT.evs = SLOT.evsa;
            CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
        }
        static void set_dr(FM_SLOT SLOT, int v, int[] dr_table)
        {
            SLOT.DR = (v &= 0x1f) != 0 ? new IntSubArray(dr_table, v << 1) : new IntSubArray(RATE_0);
            SLOT.evsd = SLOT.DR[SLOT.ksr];
            if (SLOT.eg_next == FM_EG_DR) SLOT.evs = SLOT.evsd;
        }
        static void set_sr(FM_SLOT SLOT, int v, int[] dr_table)
        {
            SLOT.SR = (v &= 0x1f) != 0 ? new IntSubArray(dr_table, v << 1) : new IntSubArray(RATE_0);
            SLOT.evss = SLOT.SR[SLOT.ksr];
            if (SLOT.eg_next == FM_EG_SR) SLOT.evs = SLOT.evss;
        }
        static void OPNWriteReg(FM_OPN OPN, int r, int v)
        {
            byte c;
            FM_CH CH;
            FM_SLOT SLOT;

            /* 0x30 - 0xff */
            if ((c = (byte)OPN_CHAN(r)) == 3) return; /* 0xX3,0xX7,0xXB,0xXF */
            if ((r >= 0x100) /* && (OPN.type & TYPE_6CH) */ ) c += 3;
            //CH = OPN.P_CH;
            CH = OPN.P_CH[c];

            SLOT = CH.SLOT[OPN_SLOT(r)];
            switch (r & 0xf0)
            {
                case 0x30:	/* DET , MUL */
                    set_det_mul(OPN.ST, CH, SLOT, v);
                    break;
                case 0x40:	/* TL */
                    set_tl(CH, SLOT, v, (c == 2) && (OPN.ST.mode & 0x80) != 0);
                    break;
                case 0x50:	/* KS, AR */
                    set_ar_ksr(CH, SLOT, v, OPN.ST.AR_TABLE);
                    break;
                case 0x60:	/*     DR */
                    /* bit7 = AMS_ON ENABLE(YM2612) */
                    set_dr(SLOT, v, OPN.ST.DR_TABLE);
#if FM_LFO_SUPPORT
                    if ((OPN.type & TYPE_LFOPAN) != 0)
                    {
                        SLOT.amon = (byte)(v >> 7);
                        SLOT.ams = CH.ams * SLOT.amon;
                    }
#endif
                    break;
                case 0x70:	/*     SR */
                    set_sr(SLOT, v, OPN.ST.DR_TABLE);
                    break;
                case 0x80:	/* SL, RR */
                    set_sl_rr(SLOT, v, OPN.ST.DR_TABLE);
                    break;
                case 0x90:	/* SSG-EG */
#if !FM_SEG_SUPPORT
                    if ((v & 0x08) != 0) Mame.printf("OPN %d,%d,%d :SSG-TYPE envelope selected (not supported )\n", OPN.ST.index, c, OPN_SLOT(r));
#endif
                    SLOT.SEG = (byte)(v & 0x0f);
                    break;
                case 0xa0:
                    switch (OPN_SLOT(r))
                    {
                        case 0:		/* 0xa0-0xa2 : FNUM1 */
                            {
                                uint fn = (uint)((((uint)((CH.fn_h) & 7)) << 8) + v);
                                byte blk = (byte)(CH.fn_h >> 3);
                                /* make keyscale code */
                                CH.kcode = (byte)((blk << 2) | OPN_FKTABLE[(fn >> 7)]);
                                /* make basic increment counter 32bit = 1 cycle */
                                CH.fc = OPN.FN_TABLE[fn] >> (7 - blk);
                                CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
                            }
                            break;
                        case 1:		/* 0xa4-0xa6 : FNUM2,BLK */
                            CH.fn_h = (byte)(v & 0x3f);
                            break;
                        case 2:		/* 0xa8-0xaa : 3CH FNUM1 */
                            if (r < 0x100)
                            {
                                uint fn = (uint)((((uint)(OPN.SL3.fn_h[c] & 7)) << 8) + v);
                                byte blk = (byte)(OPN.SL3.fn_h[c] >> 3);
                                /* make keyscale code */
                                OPN.SL3.kcode[c] = (byte)((blk << 2) | OPN_FKTABLE[(fn >> 7)]);
                                /* make basic increment counter 32bit = 1 cycle */
                                OPN.SL3.fc[c] = OPN.FN_TABLE[fn] >> (7 - blk);
                                (OPN.P_CH)[2].SLOT[SLOT1].Incr = unchecked((uint)-1);
                            }
                            break;
                        case 3:		/* 0xac-0xae : 3CH FNUM2,BLK */
                            if (r < 0x100)
                                OPN.SL3.fn_h[c] = (byte)(v & 0x3f);
                            break;
                    }
                    break;
                case 0xb0:
                    switch (OPN_SLOT(r))
                    {
                        case 0:		/* 0xb0-0xb2 : FB,ALGO */
                            {
                                int feedback = (v >> 3) & 7;
                                CH.ALGO = (byte)(v & 7);
                                CH.FB = feedback != 0 ? (byte)(8 + 1 - feedback) : (byte)0;
                                setup_connection(CH);
                            }
                            break;
                        case 1:		/* 0xb4-0xb6 : L , R , AMS , PMS (YM2612/YM2608) */
                            if ((OPN.type & TYPE_LFOPAN) != 0)
                            {
#if FM_LFO_SUPPORT
                                /* b0-2 PMS */
                                /* 0,3.4,6.7,10,14,20,40,80(cent) */
                                double[] pmd_table = { 0, 3.4, 6.7, 10, 14, 20, 40, 80 };
                                int[] amd_table = { (int)(0 / EG_STEP), (int)(1.4 / EG_STEP), (int)(5.9 / EG_STEP), (int)(11.8 / EG_STEP) };
                                CH.pms = (int)((1.5 / 1200.0) * pmd_table[(v >> 4) & 0x07] * PMS_RATE);
                                /* b4-5 AMS */
                                /* 0 , 1.4 , 5.9 , 11.8(dB) */
                                CH.ams = (uint)amd_table[(v >> 4) & 0x03];
                                CH.SLOT[SLOT1].ams = CH.ams * CH.SLOT[SLOT1].amon;
                                CH.SLOT[SLOT2].ams = CH.ams * CH.SLOT[SLOT2].amon;
                                CH.SLOT[SLOT3].ams = CH.ams * CH.SLOT[SLOT3].amon;
                                CH.SLOT[SLOT4].ams = CH.ams * CH.SLOT[SLOT4].amon;
#endif
                                /* PAN */
                                CH.PAN = (byte)((v >> 6) & 0x03); /* PAN : b6 = R , b7 = L */
                                setup_connection(CH);
                                /* Log(LOG_INF,"OPN %d,%d : PAN %d\n",n,c,CH.PAN);*/
                            }
                            break;
                    }
                    break;
            }
        }
        static void set_sl_rr(FM_SLOT SLOT, int v, int[] dr_table)
        {
            SLOT.SL = SL_TABLE[(v >> 4)];
            SLOT.RR = new IntSubArray(dr_table, ((v & 0x0f) << 2) | 2);
            SLOT.evsr = SLOT.RR[SLOT.ksr];
            if (SLOT.eg_next == FM_EG_Release) SLOT.evs = SLOT.evsr;
        }
        static void setup_connection(FM_CH CH)
        {
            IntSubArray carrier = new IntSubArray(out_ch, CH.PAN); /* NONE,LEFT,RIGHT or CENTER */

            switch (CH.ALGO)
            {
                case 0:
                    /*  PG---S1---S2---S3---S4---OUT */
                    CH.connect1 = new IntSubArray(pg_in2);
                    CH.connect2 = new IntSubArray(pg_in3);
                    CH.connect3 = new IntSubArray(pg_in4);
                    break;
                case 1:
                    /*  PG---S1-+-S3---S4---OUT */
                    /*  PG---S2-+               */
                    CH.connect1 = new IntSubArray(pg_in3);
                    CH.connect2 = new IntSubArray(pg_in3);
                    CH.connect3 = new IntSubArray(pg_in4);
                    break;
                case 2:
                    /* PG---S1------+-S4---OUT */
                    /* PG---S2---S3-+          */
                    CH.connect1 = new IntSubArray(pg_in4);
                    CH.connect2 = new IntSubArray(pg_in3);
                    CH.connect3 = new IntSubArray(pg_in4);
                    break;
                case 3:
                    /* PG---S1---S2-+-S4---OUT */
                    /* PG---S3------+          */
                    CH.connect1 = new IntSubArray(pg_in2);
                    CH.connect2 = new IntSubArray(pg_in4);
                    CH.connect3 = new IntSubArray(pg_in4);
                    break;
                case 4:
                    /* PG---S1---S2-+--OUT */
                    /* PG---S3---S4-+      */
                    CH.connect1 = new IntSubArray(pg_in2);
                    CH.connect2 = carrier;
                    CH.connect3 = new IntSubArray(pg_in4);
                    break;
                case 5:
                    /*         +-S2-+     */
                    /* PG---S1-+-S3-+-OUT */
                    /*         +-S4-+     */
                    CH.connect1 = null;	/* special case */
                    CH.connect2 = carrier;
                    CH.connect3 = carrier;
                    break;
                case 6:
                    /* PG---S1---S2-+     */
                    /* PG--------S3-+-OUT */
                    /* PG--------S4-+     */
                    CH.connect1 = new IntSubArray(pg_in2);
                    CH.connect2 = carrier;
                    CH.connect3 = carrier;
                    break;
                case 7:
                    /* PG---S1-+     */
                    /* PG---S2-+-OUT */
                    /* PG---S3-+     */
                    /* PG---S4-+     */
                    CH.connect1 = carrier;
                    CH.connect2 = carrier;
                    CH.connect3 = carrier;
                    break;
            }
            CH.connect4 = carrier;
        }
        public static void YM2203ResetChip(int num)
        {
            int i;
            FM_OPN OPN = FM2203[num].OPN;

            /* Reset Priscaler */
            OPNSetPris(OPN, 6 * 12, 6 * 12, 4); /* 1/6 , 1/4 */
            /* reset SSG section */
            SSGReset(OPN.ST.index);
            /* status clear */
            FM_IRQMASK_SET(ref OPN.ST, 0x03);
            OPNWriteMode(OPN, 0x27, 0x30); /* mode 0 , timer reset */
            reset_channel(OPN.ST, FM2203[num].CH, 3);
            /* reset OPerator paramater */
            for (i = 0xb6; i >= 0xb4; i--) OPNWriteReg(OPN, i, 0xc0); /* PAN RESET */
            for (i = 0xb2; i >= 0x30; i--) OPNWriteReg(OPN, i, 0);
            for (i = 0x26; i >= 0x20; i--) OPNWriteReg(OPN, i, 0);
        }
#if FM_LFO_SUPPORT
        static void FM_CALC_EG(ref uint OUT, FM_SLOT SLOT)
        {
            if ((SLOT.evc += SLOT.evs) >= SLOT.eve)
                SLOT.eg_next(SLOT);
            OUT = (uint)(SLOT.TLL + ENV_CURVE[SLOT.evc >> ENV_BITS]);
            if (SLOT.ams != 0)
                OUT += (SLOT.ams * lfo_amd / LFO_RATE);
        }
#else
void  FM_CALC_EG(OUT,SLOT)						
{
}
#endif
        static int FMInitTable()
        {
            int s, t;
            double rate;
            int i, j;
            double pom;

            /* allocate total level table plus+minus section */
            TL_TABLE = new int[2 * TL_MAX];

            /* make total level table */
            for (t = 0; t < TL_MAX; t++)
            {
                if (t >= PG_CUT_OFF)
                    rate = 0;	/* under cut off area */
                else
                    rate = ((1 << TL_BITS) - 1) / Math.Pow(10, EG_STEP * t / 20);	/* dB . voltage */
                TL_TABLE[t] = (int)rate;
                TL_TABLE[TL_MAX + t] = -TL_TABLE[t];
                /*		Log(LOG_INF,"TotalLevel(%3d) = %x\n",t,TL_TABLE[t]);*/
            }

            /* make sinwave table (total level offet) */

            for (s = 1; s <= SIN_ENT / 4; s++)
            {
                pom = Math.Sin(2.0 * Math.PI * s / SIN_ENT); /* sin   */
                pom = 20 * Math.Log10(1 / pom);	     /* . decibel */
                j = (int)(pom / EG_STEP);    /* TL_TABLE steps */
                /* cut off check */
                if (j > PG_CUT_OFF)
                    j = PG_CUT_OFF;
                /* degree 0   -  90    , degree 180 -  90 : plus section */
                SIN_TABLE[s] = SIN_TABLE[SIN_ENT / 2 - s] = new IntSubArray(TL_TABLE, j);
                /* degree 180 - 270    , degree 360 - 270 : minus section */
                SIN_TABLE[SIN_ENT / 2 + s] = SIN_TABLE[SIN_ENT - s] = new IntSubArray(TL_TABLE, TL_MAX + j);
                /* Log(LOG_INF,"sin(%3d) = %f:%f db\n",s,pom,(double)j * EG_STEP); */
            }
            /* degree 0 = degree 180                   = off */
            SIN_TABLE[0] = SIN_TABLE[SIN_ENT / 2] = new IntSubArray(TL_TABLE, PG_CUT_OFF);

            /* envelope counter . envelope output table */
            for (i = 0; i < EG_ENT; i++)
            {
                /* ATTACK curve */
                /* !!!!! preliminary !!!!! */
                pom = Math.Pow(((double)(EG_ENT - 1 - i) / EG_ENT), 8) * EG_ENT;
                /* if( pom >= EG_ENT ) pom = EG_ENT-1; */
                ENV_CURVE[i] = (int)pom;
                /* DECAY ,RELEASE curve */
                ENV_CURVE[(EG_DST >> ENV_BITS) + i] = i;
#if FM_SEG_SUPPORT
		/* DECAY UPSIDE (SSG ENV) */
		ENV_CURVE[(EG_UST>>ENV_BITS)+i]= EG_ENT-1-i;
#endif
            }
            /* off */
            ENV_CURVE[EG_OFF >> ENV_BITS] = EG_ENT - 1;

            /* decay to reattack envelope converttable */
            j = EG_ENT - 1;
            for (i = 0; i < EG_ENT; i++)
            {
                while (j != 0 && (ENV_CURVE[j] < i)) j--;
                DRAR_TABLE[i] = j << ENV_BITS;
                /* Log(LOG_INF,"DR %06X = %06X,AR=%06X\n",i,DRAR_TABLE[i],ENV_CURVE[DRAR_TABLE[i]>>ENV_BITS] ); */
            }
            return 1;
        }
        static void CALC_FCOUNT(FM_CH CH)
        {
            if (CH.SLOT[SLOT1].Incr == unchecked((uint)-1))
            {
                int fc = (int)CH.fc;
                int kc = CH.kcode;
                CALC_FCSLOT(CH.SLOT[SLOT1], fc, kc);
                CALC_FCSLOT(CH.SLOT[SLOT2], fc, kc);
                CALC_FCSLOT(CH.SLOT[SLOT3], fc, kc);
                CALC_FCSLOT(CH.SLOT[SLOT4], fc, kc);
            }
        }
        static void CALC_FCSLOT(FM_SLOT SLOT, int fc, int kc)
        {
            int ksr;

            /* frequency step counter */
            /* SLOT.Incr= (fc+SLOT.DT[kc])*SLOT.mul; */
            SLOT.Incr = (uint)(fc * SLOT.mul + SLOT.DT[kc]);
            ksr = kc >> SLOT.KSR;
            if (SLOT.ksr != ksr)
            {
                SLOT.ksr = (byte)ksr;
                /* attack , decay rate recalcration */
                SLOT.evsa = SLOT.AR[ksr];
                SLOT.evsd = SLOT.DR[ksr];
                SLOT.evss = SLOT.SR[ksr];
                SLOT.evsr = SLOT.RR[ksr];
            }
            SLOT.TLL = SLOT.TL /* + KSL[kc]*/;
        }
        static void FM_CALC_CH(FM_CH CH)
        {
            uint eg_out1 = 0, eg_out2 = 0, eg_out3 = 0, eg_out4 = 0;  //envelope output

            /* Phase Generator */
#if FM_LFO_SUPPORT
            int pms = lfo_pmd * CH.pms / LFO_RATE;
            if (pms != 0)
            {
                pg_in1[0] = (int)((CH.SLOT[SLOT1].Cnt += (uint)(CH.SLOT[SLOT1].Incr + (int)(pms * CH.SLOT[SLOT1].Incr) / PMS_RATE)));
                pg_in2[0] = (int)((CH.SLOT[SLOT2].Cnt += (uint)(CH.SLOT[SLOT2].Incr + (int)(pms * CH.SLOT[SLOT2].Incr) / PMS_RATE)));
                pg_in3[0] = (int)((CH.SLOT[SLOT3].Cnt += (uint)(CH.SLOT[SLOT3].Incr + (int)(pms * CH.SLOT[SLOT3].Incr) / PMS_RATE)));
                pg_in4[0] = (int)((CH.SLOT[SLOT4].Cnt += (uint)(CH.SLOT[SLOT4].Incr + (int)(pms * CH.SLOT[SLOT4].Incr) / PMS_RATE)));
            }
            else
#endif
            {
                pg_in1[0] = (int)((CH.SLOT[SLOT1].Cnt += (uint)(CH.SLOT[SLOT1].Incr)));
                pg_in2[0] = (int)((CH.SLOT[SLOT2].Cnt += (uint)(CH.SLOT[SLOT2].Incr)));
                pg_in3[0] = (int)((CH.SLOT[SLOT3].Cnt += (uint)(CH.SLOT[SLOT3].Incr)));
                pg_in4[0] = (int)((CH.SLOT[SLOT4].Cnt += (uint)(CH.SLOT[SLOT4].Incr)));
            }

            /* Envelope Generator */
            FM_CALC_EG(ref eg_out1, CH.SLOT[SLOT1]);
            FM_CALC_EG(ref eg_out2, CH.SLOT[SLOT2]);
            FM_CALC_EG(ref eg_out3, CH.SLOT[SLOT3]);
            FM_CALC_EG(ref eg_out4, CH.SLOT[SLOT4]);

            /* Connection */
            if (eg_out1 < EG_CUT_OFF)	/* SLOT 1 */
            {
                if (CH.FB != 0)
                {
                    /* with self feed back */
                    pg_in1[0] += (CH.op1_out[0] + CH.op1_out[1]) >> CH.FB;
                    CH.op1_out[1] = CH.op1_out[0];
                }
                CH.op1_out[0] = SIN_TABLE[(pg_in1[0] / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][(int)eg_out1];
                //OP_OUT(pg_in1, eg_out1);
                /* output slot1 */
                if (CH.connect1 == null)
                {
                    /* algorythm 5  */
                    pg_in2[0] += CH.op1_out[0];
                    pg_in3[0] += CH.op1_out[0];
                    pg_in4[0] += CH.op1_out[0];
                }
                else
                {
                    /* other algorythm */
                    CH.connect1[0] += CH.op1_out[0];
                }
            }
            if (eg_out2 < EG_CUT_OFF)	/* SLOT 2 */
                CH.connect2[0] += SIN_TABLE[(pg_in2[0] / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][(int)eg_out2];//
            //OP_OUT(pg_in2, eg_out2);
            if (eg_out3 < EG_CUT_OFF)	/* SLOT 3 */
                CH.connect3[0] += SIN_TABLE[(pg_in3[0] / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][(int)eg_out3];
            //OP_OUT(pg_in3, eg_out3);
            if (eg_out4 < EG_CUT_OFF)	/* SLOT 4 */
                CH.connect4[0] += SIN_TABLE[(pg_in4[0] / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][(int)eg_out4];
            //OP_OUT(pg_in4, eg_out4);
        }
        public static void YM2203UpdateOne(int num, _ShortPtr buffer, int length)
        {
            YM2203 F2203 = FM2203[num];
            FM_OPN OPN = FM2203[num].OPN;
            int i;
            //FM_CH ch;
            _ShortPtr buf = buffer;

            fm_cur_chip = F2203;
            State = F2203.OPN.ST;
            cch[0] = F2203.CH[0];
            cch[1] = F2203.CH[1];
            cch[2] = F2203.CH[2];
#if FM_LFO_SUPPORT
            /* LFO */
            lfo_amd = 0;
            lfo_pmd = 0;
#endif
            /* frequency counter channel A */
            CALC_FCOUNT(cch[0]);
            /* frequency counter channel B */
            CALC_FCOUNT(cch[1]);
            /* frequency counter channel C */
            if ((State.mode & 0xc0) != 0)
            {
                /* 3SLOT MODE */
                if (cch[2].SLOT[SLOT1].Incr == unchecked((uint)-1))
                {
                    /* 3 slot mode */
                    CALC_FCSLOT(cch[2].SLOT[SLOT1], (int)OPN.SL3.fc[1], OPN.SL3.kcode[1]);
                    CALC_FCSLOT(cch[2].SLOT[SLOT2], (int)OPN.SL3.fc[2], OPN.SL3.kcode[2]);
                    CALC_FCSLOT(cch[2].SLOT[SLOT3], (int)OPN.SL3.fc[0], OPN.SL3.kcode[0]);
                    CALC_FCSLOT(cch[2].SLOT[SLOT4], (int)cch[2].fc, cch[2].kcode);
                }
            }
            else CALC_FCOUNT(cch[2]);

            for (i = 0; i < length; i++)
            {
                /*            channel A         channel B         channel C      */
                out_ch[OUTD_CENTER] = 0;
                /* calcrate FM */
                for (int ii = 0; ii < 2; ii++)
                {
                    FM_CALC_CH(cch[ii]);
                }
                //for (ch = cch[0]; ch <= cch[2]; ch++)FM_CALC_CH(ch);
                /* limit check */
                //Limit(out_ch[OUTD_CENTER], FM_MAXOUT, FM_MINOUT);
                if (out_ch[OUTD_CENTER] > FM_MAXOUT)
                    out_ch[OUTD_CENTER] = FM_MAXOUT;
                else if (out_ch[OUTD_CENTER] < FM_MINOUT)
                    out_ch[OUTD_CENTER] = FM_MINOUT;
                /* store to sound buffer */
                buf.write16(i, (ushort)(out_ch[OUTD_CENTER] >> FM_OUTSB));
                /* timer controll */
                //INTERNAL_TIMER_A( State , cch[2] )

            }
            //INTERNAL_TIMER_B(State,length)
        }

        class YM2151
        {
            public FM_ST ST = new FM_ST();
            public FM_CH[] CH = new FM_CH[8];
            public byte ct;
            public uint NoiseCnt, NoiseIncr;
#if FM_LFO_SUPPORT
            /* LFO */
            public uint LFOCnt;
            public uint LFOIncr;
            public byte pmd;					/* LFO pmd level     */
            public byte amd;					/* LFO amd level     */
            public IntSubArray wavetype;			/* LFO waveform      */
            public int[] LFO_wave = new int[LFO_ENT * 4];	/* LFO wave tabel    */
            public byte testreg;				/* test register (LFO reset) */
#endif
            public uint[] KC_TABLE = new uint[8 * 12 * 64 + 950];/* keycode,keyfunction . count */
            public YM2151writehandler PortWrite;/*  callback when write CT0/CT1 */
        }

        static uint NoiseCnt, NoiseIncr;
        static IntSubArray[] NOISE_TABLE = new IntSubArray[SIN_ENT];

        static int[] DT2_TABLE = { 0, 384, 500, 608 };
        static int[] KC_TO_SEMITONE = {
	/*translate note code KC into more usable number of semitone*/
	0*64, 1*64, 2*64, 3*64,
	3*64, 4*64, 5*64, 6*64,
	6*64, 7*64, 8*64, 9*64,
	9*64,10*64,11*64,12*64
};
        static void OPM_CALC_FCOUNT(YM2151 OPM, FM_CH CH)
        {
            if (CH.SLOT[SLOT1].Incr == unchecked((uint)-1))
            {
                int fc = (int)CH.fc;
                int kc = CH.kcode;

                CALC_FCSLOT(CH.SLOT[SLOT1], (int)OPM.KC_TABLE[fc + CH.SLOT[SLOT1].DT2], kc);
                CALC_FCSLOT(CH.SLOT[SLOT2], (int)OPM.KC_TABLE[fc + CH.SLOT[SLOT2].DT2], kc);
                CALC_FCSLOT(CH.SLOT[SLOT3], (int)OPM.KC_TABLE[fc + CH.SLOT[SLOT3].DT2], kc);
                CALC_FCSLOT(CH.SLOT[SLOT4], (int)OPM.KC_TABLE[fc + CH.SLOT[SLOT4].DT2], kc);
            }
        }
        static void OPM_CALC_CH7(FM_CH CH)
        {
            uint eg_out1 = 0, eg_out2 = 0, eg_out3 = 0, eg_out4 = 0;  //envelope output

            /* Phase Generator */
#if FM_LFO_SUPPORT
            int pms = lfo_pmd * CH.pms / LFO_RATE;
            if (pms != 0)
            {
                pg_in1[0] = (int)(CH.SLOT[SLOT1].Cnt += (uint)(CH.SLOT[SLOT1].Incr + (int)(pms * CH.SLOT[SLOT1].Incr) / PMS_RATE));
                pg_in2[0] = (int)(CH.SLOT[SLOT2].Cnt += (uint)(CH.SLOT[SLOT2].Incr + (int)(pms * CH.SLOT[SLOT2].Incr) / PMS_RATE));
                pg_in3[0] = (int)(CH.SLOT[SLOT3].Cnt += (uint)(CH.SLOT[SLOT3].Incr + (int)(pms * CH.SLOT[SLOT3].Incr) / PMS_RATE));
                pg_in4[0] = (int)(CH.SLOT[SLOT4].Cnt += (uint)(CH.SLOT[SLOT4].Incr + (int)(pms * CH.SLOT[SLOT4].Incr) / PMS_RATE));
            }
            else
#endif
            {
                pg_in1[0] = (int)(CH.SLOT[SLOT1].Cnt += CH.SLOT[SLOT1].Incr);
                pg_in2[0] = (int)(CH.SLOT[SLOT2].Cnt += CH.SLOT[SLOT2].Incr);
                pg_in3[0] = (int)(CH.SLOT[SLOT3].Cnt += CH.SLOT[SLOT3].Incr);
                pg_in4[0] = (int)(CH.SLOT[SLOT4].Cnt += CH.SLOT[SLOT4].Incr);
            }
            /* Envelope Generator */
            FM_CALC_EG(ref eg_out1, CH.SLOT[SLOT1]);
            FM_CALC_EG(ref eg_out2, CH.SLOT[SLOT2]);
            FM_CALC_EG(ref eg_out3, CH.SLOT[SLOT3]);
            FM_CALC_EG(ref eg_out4, CH.SLOT[SLOT4]);

            /* connection */
            if (eg_out1 < EG_CUT_OFF)	/* SLOT 1 */
            {
                if (CH.FB != 0)
                {
                    /* with self feed back */
                    pg_in1[0] += (CH.op1_out[0] + CH.op1_out[1]) >> CH.FB;
                    CH.op1_out[1] = CH.op1_out[0];
                }
                CH.op1_out[0] = OP_OUT(pg_in1[0], (int)eg_out1);
                /* output slot1 */
                if (CH.connect1 == null)
                {
                    /* algorythm 5  */
                    pg_in2[0] += CH.op1_out[0];
                    pg_in3[0] += CH.op1_out[0];
                    pg_in4[0] += CH.op1_out[0];
                }
                else
                {
                    /* other algorythm */
                    CH.connect1[0] += CH.op1_out[0];
                }
            }
            if (eg_out2 < EG_CUT_OFF)	/* SLOT 2 */
                CH.connect2[0] += OP_OUT(pg_in2[0], (int)eg_out2);
            if (eg_out3 < EG_CUT_OFF)	/* SLOT 3 */
                CH.connect3[0] += OP_OUT(pg_in3[0], (int)eg_out3);
            /* SLOT 4 */
            if (NoiseIncr != 0)
            {
                NoiseCnt += NoiseIncr;
                if (eg_out4 < EG_CUT_OFF)
                    CH.connect4[0] += OP_OUTN((int)NoiseCnt, (int)eg_out4);
            }
            else
            {
                if (eg_out4 < EG_CUT_OFF)
                    CH.connect4[0] += OP_OUT(pg_in4[0], (int)eg_out4);
            }
        }
        static int OP_OUT(int PG, int EG) { return SIN_TABLE[(PG / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][EG]; }
        static int OP_OUTN(int PG, int EG) { return NOISE_TABLE[(PG / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][EG]; }

        public static void OPMUpdateOne(int num, _ShortPtr[] buffer, int length)
        {
            YM2151 OPM = FMOPM[num];
            int i;
            int amd, pmd;
            FM_CH ch;
            _ShortPtr bufL, bufR;

            /* set bufer */
            bufL = buffer[0];
            bufR = buffer[1];

            if (OPM != fm_cur_chip)
            {
                fm_cur_chip = OPM;

                State = OPM.ST;
                /* channel pointer */
                cch[0] = OPM.CH[0];
                cch[1] = OPM.CH[1];
                cch[2] = OPM.CH[2];
                cch[3] = OPM.CH[3];
                cch[4] = OPM.CH[4];
                cch[5] = OPM.CH[5];
                cch[6] = OPM.CH[6];
                cch[7] = OPM.CH[7];
                /* ch7.op4 noise mode / step */
                NoiseIncr = OPM.NoiseIncr;
                NoiseCnt = OPM.NoiseCnt;
#if FM_LFO_SUPPORT
                /* LFO */
                LFOCnt = OPM.LFOCnt;
                //LFOIncr = OPM.LFOIncr;
                if (LFOIncr == 0) { lfo_amd = 0; lfo_pmd = 0; }
                LFO_wave = OPM.wavetype;
#endif
            }
            amd = OPM.amd;
            pmd = OPM.pmd;
            if (amd == 0 && pmd == 0)
                LFOIncr = 0;
            else
                LFOIncr = OPM.LFOIncr;

            OPM_CALC_FCOUNT(OPM, cch[0]);
            OPM_CALC_FCOUNT(OPM, cch[1]);
            OPM_CALC_FCOUNT(OPM, cch[2]);
            OPM_CALC_FCOUNT(OPM, cch[3]);
            OPM_CALC_FCOUNT(OPM, cch[4]);
            OPM_CALC_FCOUNT(OPM, cch[5]);
            OPM_CALC_FCOUNT(OPM, cch[6]);
            /* CSM check */
            OPM_CALC_FCOUNT(OPM, cch[7]);

            for (i = 0; i < length; i++)
            {
#if FM_LFO_SUPPORT
                /* LFO */
                if (LFOIncr != 0)
                {
                    int depth = LFO_wave[(int)((LFOCnt += LFOIncr) >> LFO_SHIFT)];
                    lfo_amd = (uint)(depth * amd);
                    lfo_pmd = (depth - (LFO_RATE / 127 / 2)) * pmd;
                }
#endif
                /* clear output acc. */
                out_ch[OUTD_LEFT] = out_ch[OUTD_RIGHT] = out_ch[OUTD_CENTER] = 0;
                /* calcrate channel output */
                for (int k = 0; k < 6; k++)
                {
                    //for (ch = cch[0]; ch <= cch[6]; ch++)
                    FM_CALC_CH(cch[k]);
                }
                OPM_CALC_CH7(cch[7]);
                /* buffering */
#if FM_STEREO_MIX
#error
#else
                /* get left & right output with clipping */
                out_ch[OUTD_LEFT] += out_ch[OUTD_CENTER];
                Limit(ref out_ch[OUTD_LEFT], FM_MAXOUT, FM_MINOUT);
                out_ch[OUTD_RIGHT] += out_ch[OUTD_CENTER];
                Limit(ref out_ch[OUTD_RIGHT], FM_MAXOUT, FM_MINOUT);
                /* buffering */
                bufL.write16(i, (ushort)(out_ch[OUTD_LEFT] >> FM_OUTSB));
                bufR.write16(i, (ushort)(out_ch[OUTD_RIGHT] >> FM_OUTSB));
#endif
                /* timer A controll */
                //INTERNAL_TIMER_A( State , cch[7] );
            }
            //INTERNAL_TIMER_B(State, length);
            OPM.NoiseCnt = NoiseCnt;
#if FM_LFO_SUPPORT
            OPM.LFOCnt = LFOCnt;
#endif
        }
        static void Limit(ref int val, int max, int min)
        {
            if (val > max) val = max; else if (val < min) val = min;            
        }

        public static int YM2151TimerOver(int n, int c)
        {
            YM2151 F2151 = FMOPM[n];

            if (c != 0)
            {	/* Timer B */
                TimerBOver(F2151.ST);
            }
            else
            {	/* Timer A */
                mame.YM2151.YM2151UpdateRequest(n);
                /* timer update */
                TimerAOver(F2151.ST);
                /* CSM mode key,TL controll */
                if ((F2151.ST.mode & 0x80) != 0)
                {	/* CSM mode total level latch and auto key on */
                    CSMKeyControll(F2151.CH[0]);
                    CSMKeyControll(F2151.CH[1]);
                    CSMKeyControll(F2151.CH[2]);
                    CSMKeyControll(F2151.CH[3]);
                    CSMKeyControll(F2151.CH[4]);
                    CSMKeyControll(F2151.CH[5]);
                    CSMKeyControll(F2151.CH[6]);
                    CSMKeyControll(F2151.CH[7]);
                }
            }
            return F2151.ST.irq;
        }
        public static int OPMInit(int num, int clock, int rate, FM_TIMERHANDLER TimerHandler, FM_IRQHANDLER IRQHandler)
        {
            int i;

            if (FMOPM != null) return (-1);	/* duplicate init. */
            fm_cur_chip = null;	/* hiro-shi!! */

            YM2151NumChips = num;

            /* allocate ym2151 state space */
            FMOPM = new YM2151[YM2151NumChips];

            /* clear */
            //memset(FMOPM, 0, sizeof(YM2151) * YM2151NumChips);

            /* allocate total lebel table (128kb space) */
            if (FMInitTable() == 0)
            {
                FMOPM = null;
                return (-1);
            }
            for (i = 0; i < YM2151NumChips; i++)
            {
                FMOPM[i] = new YM2151();
                FMOPM[i].ST.index = (byte)i;
                FMOPM[i].ST.clock = clock;
                FMOPM[i].ST.rate = rate;
                /* FMOPM[i].ST.irq  = 0; */
                /* FMOPM[i].ST.status = 0; */
                FMOPM[i].ST.timermodel = FM_TIMER_INTERVAL;
                FMOPM[i].ST.freqbase = rate != 0 ? ((double)clock / rate) / 64 : 0;
                FMOPM[i].ST.TimerBase = 1.0 / ((double)clock / 64.0);
                /* Extend handler */
                FMOPM[i].ST.Timer_Handler = TimerHandler;
                FMOPM[i].ST.IRQ_Handler = IRQHandler;
                /* Reset callback handler of CT0/1 */
                FMOPM[i].PortWrite = null;
                OPMResetChip(i);
            }
            return (0);
        }
        static void OPMWriteReg(int n, int r, int v)
        {
            byte c;
            FM_CH CH;
            FM_SLOT SLOT;

            YM2151 OPM = FMOPM[n];

            c = (byte)(r & 7);
            CH = OPM.CH[c];
            SLOT = CH.SLOT[((r >> 3) & 3)];

            switch (r & 0xe0)
            {
                case 0x00: /* 0x00-0x1f */
                    switch (r)
                    {
#if FM_LFO_SUPPORT
                        case 0x01:	/* test */
                            if (((OPM.testreg & (OPM.testreg ^ v)) & 0x02) != 0) /* fall eggge */
                            {	/* reset LFO counter */
                                OPM.LFOCnt = 0;
                                fm_cur_chip = null;
                            }
                            OPM.testreg = (byte)v;
                            break;
#endif
                        case 0x08:	/* key on / off */
                            c = (byte)(v & 7);
                            /* CSM mode */
                            if ((OPM.ST.mode & 0x80) != 0) break;
                            CH = OPM.CH[c];
                            if ((v & 0x08) != 0) FM_KEYON(CH, SLOT1); else FM_KEYOFF(CH, SLOT1);
                            if ((v & 0x10) != 0) FM_KEYON(CH, SLOT2); else FM_KEYOFF(CH, SLOT2);
                            if ((v & 0x20) != 0) FM_KEYON(CH, SLOT3); else FM_KEYOFF(CH, SLOT3);
                            if ((v & 0x40) != 0) FM_KEYON(CH, SLOT4); else FM_KEYOFF(CH, SLOT4);
                            break;
                        case 0x0f:	/* Noise freq (ch7.op4) */
                            /* b7 = Noise enable */
                            /* b0-4 noise freq  */
                            OPM.NoiseIncr = (uint)((v & 0x80) == 0 ? 0 :
                                /* !!!!! unknown noise freqency rate !!!!! */
                                (1 << FREQ_BITS) / 65536 * (v & 0x1f) * OPM.ST.freqbase);
                            fm_cur_chip = null;
                            //#if true
                            //            if( v & 0x80 ){
                            //                Log(LOG_WAR,"OPM Noise mode selelted\n");
                            //            }
                            //#endif
                            break;
                        case 0x10:	/* timer A High 8*/
                            OPM.ST.TA = (OPM.ST.TA & 0x03) | (((int)v) << 2);
                            break;
                        case 0x11:	/* timer A Low 2*/
                            OPM.ST.TA = (OPM.ST.TA & 0x3fc) | (v & 3);
                            break;
                        case 0x12:	/* timer B */
                            OPM.ST.TB = (byte)v;
                            break;
                        case 0x14:	/* mode , timer controll */
                            FMSetMode(ref OPM.ST, n, v);
                            break;
#if FM_LFO_SUPPORT
                        case 0x18:	/* lfreq   */
                            /* f = fm * 2^(LFRQ/16) / (4295*10^6) */
                            {
                                double[] drate ={
					1.0        ,1.044273782,1.090507733,1.138788635, //0-3
					1.189207115,1.241857812,1.296839555,1.354255547, //4-7
					1.414213562,1.476826146,1.542210825,1.610490332, //8-11
					1.681792831,1.75625216 ,1.834008086,1.915206561};
                                double rate = Math.Pow(2.0, v / 16) * drate[v & 0x0f] / 4295000000.0;
                                OPM.LFOIncr = (uint)((double)LFO_ENT * (1 << LFO_SHIFT) * (OPM.ST.freqbase * 64) * rate);
                                fm_cur_chip = null;
                            }
                            break;
                        case 0x19:	/* PMD/AMD */
                            if ((v & 0x80) != 0) OPM.pmd = (byte)(v & 0x7f);
                            else OPM.amd = (byte)(v & 0x7f);
                            break;
#endif
                        case 0x1b:	/* CT , W  */
                            /* b7 = CT1 */
                            /* b6 = CT0 */
                            /* b0-2 = wave form(LFO) 0=nokogiri,1=houkei,2=sankaku,3=noise */
                            //if(OPM.ct != v)
                            {
                                OPM.ct = (byte)(v >> 6);
                                if (OPM.PortWrite != null)
                                    OPM.PortWrite(0, OPM.ct); /* bit0 = CT0,bit1 = CT1 */
                            }
#if FM_LFO_SUPPORT
                            if (OPM.wavetype != new IntSubArray(OPM.LFO_wave, (v & 3) * LFO_ENT))
                            {
                                OPM.wavetype = new IntSubArray(OPM.LFO_wave, (v & 3) * LFO_ENT);
                                fm_cur_chip = null;
                            }
#endif
                            break;
                    }
                    break;
                case 0x20:	/* 20-3f */
                    switch (((r >> 3) & 3))
                    {
                        case 0: /* 0x20-0x27 : RL,FB,CON */
                            {
                                int feedback = (v >> 3) & 7;
                                CH.ALGO = (byte)(v & 7);
                                CH.FB = (byte)(feedback != 0 ? 8 + 1 - feedback : 0);
                                /* RL order . LR order */
                                CH.PAN = (byte)(((v >> 7) & 1) | ((v >> 5) & 2));
                                setup_connection(CH);
                            }
                            break;
                        case 1: /* 0x28-0x2f : Keycode */
                            {
                                int blk = (v >> 4) & 7;
                                /* make keyscale code */
                                CH.kcode = (byte)((v >> 2) & 0x1f);
                                /* make basic increment counter 22bit = 1 cycle */
                                CH.fc = (uint)((blk * (12 * 64)) + KC_TO_SEMITONE[v & 0x0f] + CH.fn_h);
                                CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
                            }
                            break;
                        case 2: /* 0x30-0x37 : Keyfunction */
                            CH.fc -= CH.fn_h;
                            CH.fn_h = (byte)(v >> 2);
                            CH.fc += CH.fn_h;
                            CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
                            break;
#if FM_LFO_SUPPORT
                        case 3: /* 0x38-0x3f : PMS / AMS */
                            /* b0-1 AMS */
                            /* AMS * 23.90625db @ AMD=127 */
                            //CH.ams = (v & 0x03) * (23.90625/EG_STEP);
                            CH.ams = (uint)((23.90625 / EG_STEP) / (1 << (3 - (v & 3))));
                            CH.SLOT[SLOT1].ams = CH.ams * CH.SLOT[SLOT1].amon;
                            CH.SLOT[SLOT2].ams = CH.ams * CH.SLOT[SLOT2].amon;
                            CH.SLOT[SLOT3].ams = CH.ams * CH.SLOT[SLOT3].amon;
                            CH.SLOT[SLOT4].ams = CH.ams * CH.SLOT[SLOT4].amon;
                            /* b4-6 PMS */
                            /* 0,5,10,20,50,100,400,700 (cent) @ PMD=127 */
                            {
                                /* 1 octabe = 1200cent = +100%/-50% */
                                /* 100cent  = 1seminote = 6% ?? */
                                int[] pmd_table = { 0, 5, 10, 20, 50, 100, 400, 700 };
                                CH.pms = (int)((1.5 / 1200.0) * pmd_table[(v >> 4) & 0x07] * PMS_RATE);
                            }
                            break;
#endif
                    }
                    break;
                case 0x40:	/* DT1,MUL */
                    set_det_mul(OPM.ST, CH, SLOT, v);
                    break;
                case 0x60:	/* TL */
                    set_tl(CH, SLOT, v, (c == 7) && (OPM.ST.mode & 0x80) != 0);
                    break;
                case 0x80:	/* KS, AR */
                    set_ar_ksr(CH, SLOT, v, OPM.ST.AR_TABLE);
                    break;
                case 0xa0:	/* AMS EN,D1R */
                    set_dr(SLOT, v, OPM.ST.DR_TABLE);
#if FM_LFO_SUPPORT
                    /* bit7 = AMS ENABLE */
                    SLOT.amon = (byte)(v >> 7);
                    SLOT.ams = CH.ams * SLOT.amon;
#endif
                    break;
                case 0xc0:	/* DT2 ,D2R */
                    SLOT.DT2 = DT2_TABLE[v >> 6];
                    CH.SLOT[SLOT1].Incr = unchecked((uint)-1);
                    set_sr(SLOT, v, OPM.ST.DR_TABLE);
                    break;
                case 0xe0:	/* D1L, RR */
                    set_sl_rr(SLOT, v, OPM.ST.DR_TABLE);
                    break;
            }
        }
        public static void OPMResetChip(int num)
        {
            int i;
            YM2151 OPM = FMOPM[num];

            OPMInitTable(num);
            reset_channel(OPM.ST, OPM.CH, 8);
            /* status clear */
            FM_IRQMASK_SET(ref OPM.ST, 0x03);
            OPMWriteReg(num, 0x1b, 0x00);
            /* reset OPerator paramater */
            for (i = 0xff; i >= 0x20; i--) OPMWriteReg(num, i, 0);
        }
        static void OPMInitTable(int num)
        {
            YM2151 OPM = FMOPM[num];
            int i;
            double pom;
            double rate;

            if (FMOPM[num].ST.rate != 0)
                rate = (double)(1 << FREQ_BITS) / (3579545.0 / FMOPM[num].ST.clock * FMOPM[num].ST.rate);
            else rate = 1;

            for (i = 0; i < 8 * 12 * 64 + 950; i++)
            {
                /* This calculation type was used from the Jarek's YM2151 emulator */
                pom = 6.875 * Math.Pow(2, ((i + 4 * 64) * 1.5625 / 1200.0)); /*13.75Hz is note A 12semitones below A-0, so D#0 is 4 semitones above then*/
                /*calculate phase increment for above precounted Hertz value*/
                OPM.KC_TABLE[i] = (uint)(pom * rate);
                /*Log(LOG_WAR,"OPM KC %d = %x\n",i,OPM.KC_TABLE[i]);*/
            }

            /* make time tables */
            init_timetables(ref OPM.ST, OPN_DTTABLE, OPM_ARRATE, OPM_DRRATE);
#if FM_LFO_SUPPORT
            /* LFO wave table */
            for (i = 0; i < LFO_ENT; i++)
            {
                OPM.LFO_wave[i] = LFO_RATE * i / LFO_ENT / 127;
                OPM.LFO_wave[LFO_ENT + i] = (i < LFO_ENT / 2 ? 0 : LFO_RATE) / 127;
                OPM.LFO_wave[LFO_ENT * 2 + i] = LFO_RATE * (i < LFO_ENT / 2 ? i : LFO_ENT - i) / (LFO_ENT / 2) / 127;
                OPM.LFO_wave[LFO_ENT * 3 + i] = LFO_RATE * (Mame.rand() & 0xff) / 256 / 127;
            }
#endif
            /* NOISE wave table */
            for (i = 0; i < SIN_ENT; i++)
            {
                int sign = (Mame.rand() & 1) != 0 ? TL_MAX : 0;
                int lev = Mame.rand() & 0x1ff;
                //pom = lev ? 20*log10(0x200/lev) : 0;   /* decibel */
                //NOISE_TABLE[i] = &TL_TABLE[sign + (int)(pom / EG_STEP)]; /* TL_TABLE steps */
                NOISE_TABLE[i] = new IntSubArray(TL_TABLE, sign + lev * EG_ENT / 0x200); /* TL_TABLE steps */
            }
        }
        public static void OPMSetPortHander(int n, YM2151writehandler PortWrite)
        {
            FMOPM[n].PortWrite = PortWrite;
        }
        public static byte YM2151Read(int n, int a)
        {
            if ((a & 1)==0) return 0;
            else return FMOPM[n].ST.status;
        }
        public static int YM2151Write(int n, int a, byte v)
        {
            YM2151 F2151 = FMOPM[n];

            if ((a & 1)==0)
            {	/* address port */
                F2151.ST.address = (byte)(v & 0xff);
            }
            else
            {	/* data port */
                int addr = F2151.ST.address;
                xnamame036.mame.YM2151.YM2151UpdateRequest(n);
                /* write register */
                OPMWriteReg(n, addr, v);
            }
            return F2151.ST.irq;
        }
    }
}
