#define BUILD_Y8950
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class FMOPL
    {
        public delegate void OPL_TIMERHANDLER(int channel, double interval_Sec);
        public delegate void OPL_IRQHANDLER(int param, int irq);
        public delegate void OPL_UPDATEHANDLER(int param, int min_interval_us);
        public delegate void OPL_PORTHANDLER_W(int param, byte data);
        public delegate byte OPL_PORTHANDLER_R(int param);

        const byte OPL_TYPE_WAVESEL = 0x01;
        const byte OPL_TYPE_ADPCM = 0x02;
        const byte OPL_TYPE_KEYBOARD = 0x04;
        const byte OPL_TYPE_IO = 0x08;
        static int num_lock = 0;
        const double WHITE_NOISE_db = 6.0;

        public struct OPL_SLOT
        {
            public int TL;		/* total level     :TL << 8            */
            public int TLL;		/* adjusted now TL                     */
            public byte KSR;		/* key scale rate  :(shift down bit)   */
            public IntSubArray AR;		/* attack rate     :&AR_TABLE[AR<<2]   */
            public IntSubArray DR;		/* decay rate      :&DR_TALBE[DR<<2]   */
            public int SL;		/* sustin level    :SL_TALBE[SL]       */
            public IntSubArray RR;		/* release rate    :&DR_TABLE[RR<<2]   */
            public byte ksl;		/* keyscale level  :(shift down bits)  */
            public byte ksr;		/* key scale rate  :kcode>>KSR         */
            public uint mul;		/* multiple        :ML_TABLE[ML]       */
            public uint Cnt;		/* frequency count :                   */
            public uint Incr;	/* frequency step  :                   */
            /* envelope generator state */
            public byte eg_typ;	/* envelope type flag                  */
            public byte evm;		/* envelope phase                      */
            public int evc;		/* envelope counter                    */
            public int eve;		/* envelope counter end point          */
            public int evs;		/* envelope counter step               */
            public int evsa;	/* envelope step for AR :AR[ksr]       */
            public int evsd;	/* envelope step for DR :DR[ksr]       */
            public int evsr;	/* envelope step for RR :RR[ksr]       */
            /* LFO */
            public byte ams;		/* ams flag                            */
            public byte vib;		/* vibrate flag                        */
            /* wave selector */
            public IntSubArray wavetable;
        }
        public class OPL_CH
        {
            public OPL_SLOT[] SLOT = new OPL_SLOT[2];
            public byte CON;
            public byte FB;
            public int[] connect1, connect2;
            public int[] op1_out = new int[2];
            public uint block_fnum;
            public byte kcode;
            public uint fc;
            public uint ksl_base;
            public byte keyon;
        }
        public class FM_OPL
        {
            public byte type;			/* chip type                        */
            public int clock;			/* master clock  (Hz)                */
            public int rate;			/* sampling rate (Hz)                */
            public double freqbase;	/* frequency base                    */
            public double TimerBase;	/* Timer base time (==sampling time) */
            public byte address;		/* address register                  */
            public byte status;		/* status flag                       */
            public byte statusmask;	/* status mask                       */
            public uint mode;		/* Reg.08 : CSM , notesel,etc.       */
            /* Timer */
            public int[] T = new int[2];			/* timer counter       */
            public byte[] st = new byte[2];		/* timer enable        */
            /* FM channel slots */
            public OPL_CH[] P_CH;		/* pointer of CH       */
            public int max_ch;			/* maximum channel     */
            /* Rythm sention */
            public byte rythm;		/* Rythm mode , key flag */
#if BUILD_Y8950
            /* Delta-T ADPCM unit (Y8950) */
            public YM_DELTAT deltat;			/* DELTA-T ADPCM       */
#endif
            /* Keyboard / I/O interface unit (Y8950) */
            public byte portDirection;
            public byte portLatch;
            public OPL_PORTHANDLER_R porthandler_r;
            public OPL_PORTHANDLER_W porthandler_w;
            public int port_param;
            public OPL_PORTHANDLER_R keyboardhandler_r;
            public OPL_PORTHANDLER_W keyboardhandler_w;
            public int keyboard_param;
            /* time tables */
            public int[] AR_TABLE = new int[75];	/* atttack rate tables */
            public int[] DR_TABLE = new int[75];	/* decay rate tables   */
            public uint[] FN_TABLE = new uint[1024];  /* fnumber . increment counter */
            /* LFO */
            public IntSubArray ams_table;
            public IntSubArray vib_table;
            public int amsCnt;
            public int amsIncr;
            public int vibCnt;
            public int vibIncr;
            /* wave selector enable flag */
            public byte wavesel;
            /* external event callback handler */
            public OPL_TIMERHANDLER TimerHandler;		/* TIMER handler   */
            public int TimerParam;						/* TIMER parameter */
            public OPL_IRQHANDLER IRQHandler;		/* IRQ handler    */
            public int IRQParam;						/* IRQ parameter  */
            public OPL_UPDATEHANDLER UpdateHandler;	/* stream update handler   */
            public int UpdateParam;					/* stream update parameter */
        }

        const int OPL_ARRATE = 141280;  /* RATE 4 =  2826.24ms @ 3.6MHz */
        const int OPL_DRRATE = 1956000; /* RATE 4 = 39280.64ms @ 3.6MHz */

        const int DELTAT_MIXING_LEVEL = (1); /* DELTA-T ADPCM MIXING LEVEL */

        const int FREQ_BITS = 24;			/* frequency turn          */

        /* counter bits = 20 , octerve 7 */
        const int FREQ_RATE = (1 << (FREQ_BITS - 20));
        const int TL_BITS = (FREQ_BITS + 2);

        /* final output shift , limit minimum and maximum */
        const int OPL_OUTSB = (TL_BITS + 3 - 16);	/* OPL output final shift 16bit */
        const int OPL_MAXOUT = (0x7fff << OPL_OUTSB);
        const int OPL_MINOUT = (-0x8000 << OPL_OUTSB);

        /* -------------------- quality selection --------------------- */

        /* sinwave entries */
        /* used static memory = SIN_ENT * 4 (byte) */
        const int SIN_ENT = 2048;

        /* output level entries (envelope,sinwave) */
        /* envelope counter lower bits */
        const int ENV_BITS = 16;
        /* envelope output entries */
        const int EG_ENT = 4096;
        /* used dynamic memory = EG_ENT*4*4(byte)or EG_ENT*6*4(byte) */
        /* used static  memory = EG_ENT*4 (byte)                     */

        const int EG_OFF = ((2 * EG_ENT) << ENV_BITS);  /* OFF          */
        const int EG_DED = EG_OFF;
        const int EG_DST = (EG_ENT << ENV_BITS);  /* DECAY  START */
        const int EG_AED = EG_DST;
        const int EG_AST = 0;                 /* ATTACK START */

        const double EG_STEP = (96.0 / EG_ENT);/* OPL is 0.1875 dB step  */

        /* LFO table entries */
        const int VIB_ENT = 512;
        const int VIB_SHIFT = (32 - 9);
        const int AMS_ENT = 512;
        const int AMS_SHIFT = (32 - 9);

        const int VIB_RATE = 256;

        /* -------------------- local defines , macros --------------------- */

        /* register number to channel number , slot offset */
        const int SLOT1 = 0;
        const int SLOT2 = 1;

        /* envelope phase */
        const int ENV_MOD_RR = 0x00;
        const int ENV_MOD_DR = 0x01;
        const int ENV_MOD_AR = 0x02;

        const int TL_MAX = EG_ENT * 2;

        static object cur_chip = null;
        static OPL_CH[] S_CH;
        //static OPL_CH E_CH;
        static int E_CH;
        static OPL_SLOT SLOT7_1, SLOT7_2, SLOT8_1, SLOT8_2;
        static int[] outd = new int[1];
        static int ams, vib;
        static IntSubArray ams_table, vib_table;
        static int amsIncr, vibIncr;
        static int[] feedback2 = new int[1];

        static IntSubArray TL_TABLE, AMS_TABLE, VIB_TABLE;
        static IntSubArray[] SIN_TABLE;
        static int[] ENV_CURVE = new int[2 * EG_ENT + 1];

        public const byte OPL_TYPE_YM3526 = 0;
        public const byte OPL_TYPE_YM3812 = OPL_TYPE_WAVESEL;
        public const byte OPL_TYPE_Y8950 = (OPL_TYPE_ADPCM | OPL_TYPE_KEYBOARD | OPL_TYPE_IO);


        static int[] slot_array = {
	 0, 2, 4, 1, 3, 5,-1,-1,
	 6, 8,10, 7, 9,11,-1,-1,
	12,14,16,13,15,17,-1,-1,
	-1,-1,-1,-1,-1,-1,-1,-1
};
        static int[] RATE_0 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static int SC(int db) { return (int)(db * ((3 / EG_STEP) * (1 << ENV_BITS))) + EG_DST; }
        static int[] SL_TABLE ={
 SC( 0),SC( 1),SC( 2),SC(3 ),SC(4 ),SC(5 ),SC(6 ),SC( 7),
 SC( 8),SC( 9),SC(10),SC(11),SC(12),SC(13),SC(14),SC(31)
};
        static uint[] MUL_TABLE = {
/* 1/2, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15 */
   1, 1*2, 2*2, 3*2, 4*2, 5*2, 6*2, 7*2,
   8*2, 9*2,10*2,10*2,12*2,12*2,15*2,15*2
};

        const double ML = (0.1875 * 2 / EG_STEP);
        static uint[] KSL_TABLE = 
            {
	 (uint)(0.000*ML),(uint)(0.000*ML), (uint)(0.000*ML),(uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)(0.000*ML), (uint)(0.000*ML),(uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)(0.000*ML), (uint)(0.000*ML),(uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)(0.000*ML), (uint)(0.000*ML),(uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)( 0.000*ML), (uint)(0.000*ML), (uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)( 0.000*ML), (uint)(0.000*ML), (uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)( 0.750*ML), (uint)(1.125*ML), (uint)(1.500*ML),
	 (uint)(1.875*ML),(uint)( 2.250*ML), (uint)(2.625*ML), (uint)(3.000*ML),
	 (uint)(0.000*ML),(uint)( 0.000*ML),(uint)(0.000*ML), (uint)(0.000*ML),
	 (uint)(0.000*ML),(uint)( 1.125*ML),(uint)(1.875*ML), (uint)(2.625*ML),
	 (uint)(3.000*ML),(uint)( 3.750*ML),(uint)(4.125*ML), (uint)(4.500*ML),
	 (uint)(4.875*ML),(uint)( 5.250*ML),(uint)(5.625*ML), (uint)(6.000*ML),
	(uint)( 0.000*ML),(uint)( 0.000*ML),(uint)( 0.000*ML),(uint)( 1.875*ML),
	(uint)( 3.000*ML),(uint)( 4.125*ML),(uint)( 4.875*ML),(uint)( 5.625*ML),
	(uint)( 6.000*ML),(uint)( 6.750*ML),(uint)( 7.125*ML),(uint)( 7.500*ML),
	(uint)( 7.875*ML),(uint)( 8.250*ML),(uint)( 8.625*ML),(uint)( 9.000*ML),
	(uint)( 0.000*ML),(uint)( 0.000*ML),(uint)( 3.000*ML),(uint)( 4.875*ML),
	(uint)( 6.000*ML),(uint)( 7.125*ML),(uint)( 7.875*ML),(uint)( 8.625*ML),
	(uint)( 9.000*ML),(uint)( 9.750*ML),(uint)(10.125*ML),(uint)(10.500*ML),
	(uint)(10.875*ML),(uint)(11.250*ML),(uint)(11.625*ML),(uint)(12.000*ML),
	(uint)( 0.000*ML),(uint)( 3.000*ML),(uint)( 6.000*ML),(uint)( 7.875*ML),
	(uint)( 9.000*ML),(uint)(10.125*ML),(uint)(10.875*ML),(uint)(11.625*ML),
	(uint)(12.000*ML),(uint)(12.750*ML),(uint)(13.125*ML),(uint)(13.500*ML),
	(uint)(13.875*ML),(uint)(14.250*ML),(uint)(14.625*ML),(uint)(15.000*ML),
	(uint)( 0.000*ML),(uint)( 6.000*ML),(uint)( 9.000*ML),(uint)(10.875*ML),
	(uint)(12.000*ML),(uint)(13.125*ML),(uint)(13.875*ML),(uint)(14.625*ML),
	(uint)(15.000*ML),(uint)(15.750*ML),(uint)(16.125*ML),(uint)(16.500*ML),
	(uint)(16.875*ML),(uint)(17.250*ML),(uint)(17.625*ML),(uint)(18.000*ML),
	(uint)( 0.000*ML),(uint)( 9.000*ML),(uint)(12.000*ML),(uint)(13.875*ML),
	(uint)(15.000*ML),(uint)(16.125*ML),(uint)(16.875*ML),(uint)(17.625*ML),
	(uint)(18.000*ML),(uint)(18.750*ML),(uint)(19.125*ML),(uint)(19.500*ML),
	(uint)(19.875*ML),(uint)(20.250*ML),(uint)(20.625*ML),(uint)(21.000*ML),
};

        public static int OPLWrite(FM_OPL OPL, int a, int v)
        {
            if( (a&1) ==0)
	{	/* address port */
		OPL.address = (byte)(v & 0xff);
	}
	else
	{	/* data port */
		if(OPL.UpdateHandler!=null) OPL.UpdateHandler(OPL.UpdateParam,0);
		OPLWriteReg(OPL,OPL.address,v);
	}
	return OPL.status>>7;
}
        
        public static byte OPLRead(FM_OPL OPL, int a)
        {
            if( (a&1) ==0)
	{	/* status port */
		return (byte)(OPL.status & (OPL.statusmask|0x80));
	}
	/* data port */
	switch(OPL.address)
	{
	case 0x05: /* KeyBoard IN */
		if((OPL.type&OPL_TYPE_KEYBOARD)!=0)
		{
			if(OPL.keyboardhandler_r!=null)
				return OPL.keyboardhandler_r(OPL.keyboard_param);
			else
			Mame.printf("OPL:read unmapped KEYBOARD port\n");
		}
		return 0;
#if false
	case 0x0f: /* ADPCM-DATA  */
		return 0;
#endif
	case 0x19: /* I/O DATA    */
		if((OPL.type&OPL_TYPE_IO)!=0)
		{
			if(OPL.porthandler_r!=null)
				return OPL.porthandler_r(OPL.port_param);
			else
                Mame.printf("OPL:read unmapped I/O port\n");
		}
		return 0;
	case 0x1a: /* PCM-DATA    */
		return 0;
	}
	return 0;
        }
        static void OPLCloseTable()
        {
            TL_TABLE = null;
            SIN_TABLE = null;
            AMS_TABLE = null;
            VIB_TABLE = null;
        }
        static void OPL_UnLockTable()
        {
            if (num_lock != 0) num_lock--;
            if (num_lock != 0) return;
            /* last time */
            cur_chip = null;
            OPLCloseTable();
        }
        static int OPLOpenTable()
        {
            int s, t;
            double rate;
            int i, j;
            double pom;

            /* allocate dynamic tables */
            TL_TABLE = new IntSubArray(TL_MAX * 2);
            SIN_TABLE = new IntSubArray[SIN_ENT * 4];
            AMS_TABLE = new IntSubArray(AMS_ENT * 2);
            VIB_TABLE = new IntSubArray(VIB_ENT * 2);


            /* make total level table */
            for (t = 0; t < EG_ENT - 1; t++)
            {
                rate = ((1 << TL_BITS) - 1) / Math.Pow(10, EG_STEP * t / 20);	/* dB . voltage */
                TL_TABLE[t] = (int)rate;
                TL_TABLE[TL_MAX + t] = -TL_TABLE[t];
                /*		Log(LOG_INF,"TotalLevel(%3d) = %x\n",t,TL_TABLE[t]);*/
            }
            /* fill volume off area */
            for (t = EG_ENT - 1; t < TL_MAX; t++)
            {
                TL_TABLE[t] = TL_TABLE[TL_MAX + t] = 0;
            }

            /* make sinwave table (total level offet) */
            /* degree 0 = degree 180                   = off */
            SIN_TABLE[0] = SIN_TABLE[SIN_ENT / 2] = new IntSubArray(TL_TABLE, EG_ENT - 1);
            for (s = 1; s <= SIN_ENT / 4; s++)
            {
                pom = Math.Sin(2 * Math.PI * s / SIN_ENT); /* sin     */
                pom = 20 * Math.Log10(1 / pom);	   /* decibel */
                j = (int)(pom / EG_STEP);         /* TL_TABLE steps */

                /* degree 0   -  90    , degree 180 -  90 : plus section */
                SIN_TABLE[s] = SIN_TABLE[SIN_ENT / 2 - s] = new IntSubArray(TL_TABLE, j);
                /* degree 180 - 270    , degree 360 - 270 : minus section */
                SIN_TABLE[SIN_ENT / 2 + s] = SIN_TABLE[SIN_ENT - s] = new IntSubArray(TL_TABLE, TL_MAX + j);
                /*		Log(LOG_INF,"sin(%3d) = %f:%f db\n",s,pom,(double)j * EG_STEP);*/
            }
            for (s = 0; s < SIN_ENT; s++)
            {
                SIN_TABLE[SIN_ENT * 1 + s] = s < (SIN_ENT / 2) ? SIN_TABLE[s] : new IntSubArray(TL_TABLE, EG_ENT);
                SIN_TABLE[SIN_ENT * 2 + s] = SIN_TABLE[s % (SIN_ENT / 2)];
                SIN_TABLE[SIN_ENT * 3 + s] = ((s / (SIN_ENT / 4)) & 1) != 0 ? new IntSubArray(TL_TABLE, EG_ENT) : SIN_TABLE[SIN_ENT * 2 + s];
            }

            /* envelope counter . envelope output table */
            for (i = 0; i < EG_ENT; i++)
            {
                /* ATTACK curve */
                pom = Math.Pow(((double)(EG_ENT - 1 - i) / EG_ENT), 8) * EG_ENT;
                /* if( pom >= EG_ENT ) pom = EG_ENT-1; */
                ENV_CURVE[i] = (int)pom;
                /* DECAY ,RELEASE curve */
                ENV_CURVE[(EG_DST >> ENV_BITS) + i] = i;
            }
            /* off */
            ENV_CURVE[EG_OFF >> ENV_BITS] = EG_ENT - 1;
            /* make LFO ams table */
            for (i = 0; i < AMS_ENT; i++)
            {
                pom = (1.0 + Math.Sin(2 * Math.PI * i / AMS_ENT)) / 2; /* sin */
                AMS_TABLE[i] = (int)((1.0 / EG_STEP) * pom); /* 1dB   */
                AMS_TABLE[AMS_ENT + i] = (int)((4.8 / EG_STEP) * pom); /* 4.8dB */
            }
            /* make LFO vibrate table */
            for (i = 0; i < VIB_ENT; i++)
            {
                /* 100cent = 1seminote = 6% ?? */
                pom = (double)VIB_RATE * 0.06 * Math.Sin(2 * Math.PI * i / VIB_ENT); /* +-100sect step */
                VIB_TABLE[i] = (int)(VIB_RATE + (pom * 0.07)); /* +- 7cent */
                VIB_TABLE[VIB_ENT + i] = (int)(VIB_RATE + (pom * 0.14)); /* +-14cent */
                /* Log(LOG_INF,"vib %d=%d\n",i,VIB_TABLE[VIB_ENT+i]); */
            }
            return 1;
        }

        public static void OPLDestroy(ref FM_OPL OPL)
        {
            OPL_UnLockTable();
            OPL = null;
        }
        static int OPL_LockTable()
        {
            num_lock++;
            if (num_lock > 1) return 0;
            /* first time */
            cur_chip = null;
            /* allocate total level table (128kb space) */
            if (OPLOpenTable() == 0)
            {
                num_lock--;
                return -1;
            }
            return 0;
        }
        static void init_timetables(FM_OPL OPL, int ARRATE, int DRRATE)
        {
            int i;
            double rate;

            /* make attack rate & decay rate tables */
            for (i = 0; i < 4; i++) OPL.AR_TABLE[i] = OPL.DR_TABLE[i] = 0;
            for (i = 4; i <= 60; i++)
            {
                rate = OPL.freqbase;						/* frequency rate */
                if (i < 60) rate *= 1.0 + (i & 3) * 0.25;		/* b0-1 : x1 , x1.25 , x1.5 , x1.75 */
                rate *= 1 << ((i >> 2) - 1);						/* b2-5 : shift bit */
                rate *= (double)(EG_ENT << ENV_BITS);
                OPL.AR_TABLE[i] = (int)(rate / ARRATE);
                OPL.DR_TABLE[i] = (int)(rate / DRRATE);
            }
            for (i = 60; i < 75; i++)
            {
                OPL.AR_TABLE[i] = EG_AED - 1;
                OPL.DR_TABLE[i] = OPL.DR_TABLE[60];
            }
#if false
	for (i = 0;i < 64 ;i++){	/* make for overflow area */
		Log(LOG_WAR,"rate %2d , ar %f ms , dr %f ms \n",i,
			((double)(EG_ENT<<ENV_BITS) / OPL.AR_TABLE[i]) * (1000.0 / OPL.rate),
			((double)(EG_ENT<<ENV_BITS) / OPL.DR_TABLE[i]) * (1000.0 / OPL.rate) );
	}
#endif
        }
        static void OPL_initalize(FM_OPL OPL)
        {
            int fn;

            /* frequency base */
            OPL.freqbase = (OPL.rate) != 0 ? ((double)OPL.clock / OPL.rate) / 72 : 0;
            /* Timer base time */
            OPL.TimerBase = 1.0 / ((double)OPL.clock / 72.0);
            /* make time tables */
            init_timetables(OPL, OPL_ARRATE, OPL_DRRATE);
            /* make fnumber . increment counter table */
            for (fn = 0; fn < 1024; fn++)
            {
                OPL.FN_TABLE[fn] = (uint)(OPL.freqbase * fn * FREQ_RATE * (1 << 7) / 2);
            }
            /* LFO freq.table */
            OPL.amsIncr = (int)(OPL.rate != 0 ? (double)AMS_ENT * (1 << AMS_SHIFT) / OPL.rate * 3.7 * ((double)OPL.clock / 3600000) : 0);
            OPL.vibIncr = (int)(OPL.rate != 0 ? (double)VIB_ENT * (1 << VIB_SHIFT) / OPL.rate * 6.4 * ((double)OPL.clock / 3600000) : 0);
        }
        static void CALC_FCSLOT(OPL_CH CH, OPL_SLOT SLOT)
        {
            int ksr;

            /* frequency step counter */
            SLOT.Incr = CH.fc * SLOT.mul;
            ksr = CH.kcode >> SLOT.KSR;

            if (SLOT.ksr != ksr)
            {
                SLOT.ksr = (byte)ksr;
                /* attack , decay rate recalcration */
                SLOT.evsa = SLOT.AR[ksr];
                SLOT.evsd = SLOT.DR[ksr];
                SLOT.evsr = SLOT.RR[ksr];
            }
            SLOT.TLL = (int)(SLOT.TL + (CH.ksl_base >> SLOT.ksl));
        }
        static void OPL_KEYON(OPL_SLOT SLOT)
        {
            /* sin wave restart */
            SLOT.Cnt = 0;
            /* set attack */
            SLOT.evm = ENV_MOD_AR;
            SLOT.evs = SLOT.evsa;
            SLOT.evc = EG_AST;
            SLOT.eve = EG_AED;
        }
        static void OPL_KEYOFF(OPL_SLOT SLOT)
        {
            if (SLOT.evm > ENV_MOD_RR)
            {
                /* set envelope counter from envleope output */
                SLOT.evm = ENV_MOD_RR;
                if ((SLOT.evc & EG_DST) == 0)
                    //SLOT.evc = (ENV_CURVE[SLOT.evc>>ENV_BITS]<<ENV_BITS) + EG_DST;
                    SLOT.evc = EG_DST;
                SLOT.eve = EG_DED;
                SLOT.evs = SLOT.evsr;
            }
        }
        static void OPL_STATUS_RESET(FM_OPL OPL, int flag)
        {
            /* reset status flag */
            OPL.status &= (byte)~flag;
            if ((OPL.status & 0x80) != 0)
            {
                if ((OPL.status & OPL.statusmask) == 0)
                {
                    OPL.status &= 0x7f;
                    /* callback user interrupt handler (IRQ is ON to OFF) */
                    if (OPL.IRQHandler != null) OPL.IRQHandler(OPL.IRQParam, 0);
                }
            }
        }
        static void OPL_STATUS_SET(FM_OPL OPL, int flag)
        {
            /* set status flag */
            OPL.status |= (byte)flag;
            if ((OPL.status & 0x80) == 0)
            {
                if ((OPL.status & OPL.statusmask) != 0)
                {	/* IRQ on */
                    OPL.status |= 0x80;
                    /* callback user interrupt handler (IRQ is OFF to ON) */
                    if (OPL.IRQHandler != null) OPL.IRQHandler(OPL.IRQParam, 1);
                }
            }
        }

        /* IRQ mask set */
        static void OPL_STATUSMASK_SET(FM_OPL OPL, int flag)
        {
            OPL.statusmask = (byte)flag;
            /* IRQ handling check */
            OPL_STATUS_SET(OPL, 0);
            OPL_STATUS_RESET(OPL, 0);
        }

        static void OPLWriteReg(FM_OPL OPL, int r, int v)
        {
            OPL_CH CH;
            int slot;
            int block_fnum;

            switch (r & 0xe0)
            {
                case 0x00: /* 00-1f:controll */
                    switch (r & 0x1f)
                    {
                        case 0x01:
                            /* wave selector enable */
                            if ((OPL.type & OPL_TYPE_WAVESEL) != 0)
                            {
                                OPL.wavesel = (byte)(v & 0x20);
                                if (OPL.wavesel == 0)
                                {
                                    /* preset compatible mode */
                                    int c;
                                    for (c = 0; c < OPL.max_ch; c++)
                                    {
                                        OPL.P_CH[c].SLOT[SLOT1].wavetable = SIN_TABLE[0];
                                        OPL.P_CH[c].SLOT[SLOT2].wavetable = SIN_TABLE[0];
                                    }
                                }
                            }
                            return;
                        case 0x02:	/* Timer 1 */
                            OPL.T[0] = (256 - v) * 4;
                            break;
                        case 0x03:	/* Timer 2 */
                            OPL.T[1] = (256 - v) * 16;
                            return;
                        case 0x04:	/* IRQ clear / mask and Timer enable */
                            if ((v & 0x80) != 0)
                            {	/* IRQ flag clear */
                                OPL_STATUS_RESET(OPL, 0x7f);
                            }
                            else
                            {	/* set IRQ mask ,timer enable*/
                                byte st1 = (byte)(v & 1);
                                byte st2 = (byte)((v >> 1) & 1);
                                /* IRQRST,T1MSK,t2MSK,EOSMSK,BRMSK,x,ST2,ST1 */
                                OPL_STATUS_RESET(OPL, v & 0x78);
                                OPL_STATUSMASK_SET(OPL, ((~v) & 0x78) | 0x01);
                                /* timer 2 */
                                if (OPL.st[1] != st2)
                                {
                                    double interval = st2 != 0 ? (double)OPL.T[1] * OPL.TimerBase : 0.0;
                                    OPL.st[1] = st2;
                                    if (OPL.TimerHandler != null) OPL.TimerHandler(OPL.TimerParam + 1, interval);
                                }
                                /* timer 1 */
                                if (OPL.st[0] != st1)
                                {
                                    double interval = st1 != 0 ? (double)OPL.T[0] * OPL.TimerBase : 0.0;
                                    OPL.st[0] = st1;
                                    if (OPL.TimerHandler != null) OPL.TimerHandler(OPL.TimerParam + 0, interval);
                                }
                            }
                            return;
#if BUILD_Y8950
                        case 0x06:		/* Key Board OUT */
                            if ((OPL.type & OPL_TYPE_KEYBOARD) != 0)
                            {
                                if (OPL.keyboardhandler_w != null)
                                    OPL.keyboardhandler_w(OPL.keyboard_param, (byte)v);
                                else
                                    Mame.printf("OPL:write unmapped KEYBOARD port\n");
                            }
                            return;
                        case 0x07:	/* DELTA-T controll : START,REC,MEMDATA,REPT,SPOFF,x,x,RST */
                            if ((OPL.type & OPL_TYPE_ADPCM) != 0)
                                YM_DELTA_T.YM_DELTAT_ADPCM_Write(OPL.deltat, r - 0x07, v);
                            return;
                        case 0x08:	/* MODE,DELTA-T : CSM,NOTESEL,x,x,smpl,da/ad,64k,rom */
                            OPL.mode = (uint)v;
                            v &= 0x1f;	/* for DELTA-T unit */
                            goto case 0x09;
                        case 0x09:		/* START ADD */
                        case 0x0a:
                        case 0x0b:		/* STOP ADD  */
                        case 0x0c:
                        case 0x0d:		/* PRESCALE   */
                        case 0x0e:
                        case 0x0f:		/* ADPCM data */
                        case 0x10: 		/* DELTA-N    */
                        case 0x11: 		/* DELTA-N    */
                        case 0x12: 		/* EG-CTRL    */
                            if ((OPL.type & OPL_TYPE_ADPCM) != 0)
                                YM_DELTA_T.YM_DELTAT_ADPCM_Write(OPL.deltat, r - 0x07, v);
                            return;
#if false
		case 0x15:		/* DAC data    */
		case 0x16:
		case 0x17:		/* SHIFT    */
			return;
		case 0x18:		/* I/O CTRL (Direction) */
			if(OPL.type&OPL_TYPE_IO)
				OPL.portDirection = v&0x0f;
			return;
		case 0x19:		/* I/O DATA */
			if(OPL.type&OPL_TYPE_IO)
			{
				OPL.portLatch = v;
				if(OPL.porthandler_w)
					OPL.porthandler_w(OPL.port_param,v&OPL.portDirection);
			}
			return;
		case 0x1a:		/* PCM data */
			return;
#endif
#endif
                    }
                    break;
                case 0x20:	/* am,vib,ksr,eg type,mul */
                    slot = slot_array[r & 0x1f];
                    if (slot == -1) return;
                    set_mul(OPL, slot, v);
                    return;
                case 0x40:
                    slot = slot_array[r & 0x1f];
                    if (slot == -1) return;
                    set_ksl_tl(OPL, slot, v);
                    return;
                case 0x60:
                    slot = slot_array[r & 0x1f];
                    if (slot == -1) return;
                    set_ar_dr(OPL, slot, v);
                    return;
                case 0x80:
                    slot = slot_array[r & 0x1f];
                    if (slot == -1) return;
                    set_sl_rr(OPL, slot, v);
                    return;
                case 0xa0:
                    switch (r)
                    {
                        case 0xbd:
                            /* amsep,vibdep,r,bd,sd,tom,tc,hh */
                            {
                                byte rkey = (byte)(OPL.rythm ^ v);
                                OPL.ams_table = new IntSubArray(AMS_TABLE, (v & 0x80) != 0 ? AMS_ENT : 0);
                                OPL.vib_table = new IntSubArray(VIB_TABLE, (v & 0x40) != 0 ? VIB_ENT : 0);
                                OPL.rythm = (byte)(v & 0x3f);
                                if ((OPL.rythm & 0x20) != 0)
                                {
#if false
				usrintf_showmessage("OPL Rythm mode select");
#endif
                                    /* BD key on/off */
                                    if ((rkey & 0x10) != 0)
                                    {
                                        if ((v & 0x10) != 0)
                                        {
                                            OPL.P_CH[6].op1_out[0] = OPL.P_CH[6].op1_out[1] = 0;
                                            OPL_KEYON(OPL.P_CH[6].SLOT[SLOT1]);
                                            OPL_KEYON(OPL.P_CH[6].SLOT[SLOT2]);
                                        }
                                        else
                                        {
                                            OPL_KEYOFF(OPL.P_CH[6].SLOT[SLOT1]);
                                            OPL_KEYOFF(OPL.P_CH[6].SLOT[SLOT2]);
                                        }
                                    }
                                    /* SD key on/off */
                                    if ((rkey & 0x08) != 0)
                                    {
                                        if ((v & 0x08) != 0) OPL_KEYON(OPL.P_CH[7].SLOT[SLOT2]);
                                        else OPL_KEYOFF(OPL.P_CH[7].SLOT[SLOT2]);
                                    }/* TAM key on/off */
                                    if ((rkey & 0x04) != 0)
                                    {
                                        if ((v & 0x04) != 0) OPL_KEYON(OPL.P_CH[8].SLOT[SLOT1]);
                                        else OPL_KEYOFF(OPL.P_CH[8].SLOT[SLOT1]);
                                    }
                                    /* TOP-CY key on/off */
                                    if ((rkey & 0x02) != 0)
                                    {
                                        if ((v & 0x02) != 0) OPL_KEYON(OPL.P_CH[8].SLOT[SLOT2]);
                                        else OPL_KEYOFF(OPL.P_CH[8].SLOT[SLOT2]);
                                    }
                                    /* HH key on/off */
                                    if ((rkey & 0x01) != 0)
                                    {
                                        if ((v & 0x01) != 0) OPL_KEYON(OPL.P_CH[7].SLOT[SLOT1]);
                                        else OPL_KEYOFF(OPL.P_CH[7].SLOT[SLOT1]);
                                    }
                                }
                            }
                            return;
                    }
                    /* keyon,block,fnum */
                    if ((r & 0x0f) > 8) return;
                    CH = OPL.P_CH[r & 0x0f];
                    if ((r & 0x10) == 0)
                    {	/* a0-a8 */
                        block_fnum = (int)(CH.block_fnum & 0x1f00) | v;
                    }
                    else
                    {	/* b0-b8 */
                        int keyon = (v >> 5) & 1;
                        block_fnum = (int)(((v & 0x1f) << 8) | (CH.block_fnum & 0xff));
                        if (CH.keyon != keyon)
                        {
                            if ((CH.keyon = (byte)keyon) != 0)
                            {
                                CH.op1_out[0] = CH.op1_out[1] = 0;
                                OPL_KEYON(CH.SLOT[SLOT1]);
                                OPL_KEYON(CH.SLOT[SLOT2]);
                            }
                            else
                            {
                                OPL_KEYOFF(CH.SLOT[SLOT1]);
                                OPL_KEYOFF(CH.SLOT[SLOT2]);
                            }
                        }
                    }
                    /* update */
                    if (CH.block_fnum != block_fnum)
                    {
                        int blockRv = 7 - (block_fnum >> 10);
                        int fnum = block_fnum & 0x3ff;
                        CH.block_fnum = (uint)block_fnum;

                        CH.ksl_base = KSL_TABLE[block_fnum >> 6];
                        CH.fc = OPL.FN_TABLE[fnum] >> blockRv;
                        CH.kcode = (byte)(CH.block_fnum >> 9);
                        if ((OPL.mode & 0x40) != 0 && (CH.block_fnum & 0x100) != 0) CH.kcode |= 1;
                        CALC_FCSLOT(CH, CH.SLOT[SLOT1]);
                        CALC_FCSLOT(CH, CH.SLOT[SLOT2]);
                    }
                    return;
                case 0xc0:
                    /* FB,C */
                    if ((r & 0x0f) > 8) return;
                    CH = OPL.P_CH[r & 0x0f];
                    {
                        int feedback = (v >> 1) & 7;
                        CH.FB = (byte)(feedback != 0 ? (8 + 1) - feedback : 0);
                        CH.CON = (byte)(v & 1);
                        set_algorythm(CH);
                    }
                    return;
                case 0xe0: /* wave type */
                    slot = slot_array[r & 0x1f];
                    if (slot == -1) return;
                    CH = OPL.P_CH[slot / 2];
                    if (OPL.wavesel != 0)
                    {
                        /* Log(LOG_INF,"OPL SLOT %d wave select %d\n",slot,v&3); */
                        throw new Exception();
                        //CH.SLOT[slot & 1].wavetable = new IntSubArray(SIN_TABLE[(v & 0x03) * SIN_ENT]);
                    }
                    return;
            }
        }
        static void set_algorythm(OPL_CH CH)
        {
            int[] carrier = outd;
            CH.connect1 = CH.CON != 0 ? carrier : feedback2;
            CH.connect2 = carrier;
        }

        static void OPLResetChip(FM_OPL OPL)
        {
            int c, s;
            int i;

            /* reset chip */
            OPL.mode = 0;	/* normal mode */
            OPL_STATUS_RESET(OPL, 0x7f);
            /* reset with register write */
            OPLWriteReg(OPL, 0x01, 0); /* wabesel disable */
            OPLWriteReg(OPL, 0x02, 0); /* Timer1 */
            OPLWriteReg(OPL, 0x03, 0); /* Timer2 */
            OPLWriteReg(OPL, 0x04, 0); /* IRQ mask clear */
            for (i = 0xff; i >= 0x20; i--) OPLWriteReg(OPL, i, 0);
            /* reset OPerator paramater */
            for (c = 0; c < OPL.max_ch; c++)
            {
                OPL_CH CH = OPL.P_CH[c];
                /* OPL.P_CH[c].PAN = OPN_CENTER; */
                for (s = 0; s < 2; s++)
                {
                    /* wave table */
                    //throw new Exception();
                    CH.SLOT[s].wavetable = SIN_TABLE[0];
                    /* CH.SLOT[s].evm = ENV_MOD_RR; */
                    CH.SLOT[s].evc = EG_OFF;
                    CH.SLOT[s].eve = EG_OFF + 1;
                    CH.SLOT[s].evs = 0;
                }
            }
#if BUILD_Y8950
            if ((OPL.type & OPL_TYPE_ADPCM) != 0)
            {
                YM_DELTAT DELTAT = OPL.deltat;

                DELTAT.freqbase = OPL.freqbase;
                DELTAT.output_pointer = outd;
                DELTAT.portshift = 5;
                DELTAT.output_range = DELTAT_MIXING_LEVEL << TL_BITS;
                YM_DELTA_T.YM_DELTAT_ADPCM_Reset(DELTAT, 0);
            }
#endif
        }
        public static FM_OPL OPLCreate(int type, int clock, int rate)
        {
            byte[] ptr;
            FM_OPL OPL;
            int state_size;
            int max_ch = 9; /* normaly 9 channels */

            if (OPL_LockTable() == -1) return null;
            /* allocate OPL state space */
            //state_size = sizeof(FM_OPL);
            //state_size += sizeof(OPL_CH) * max_ch;
#if BUILD_Y8950
            //if ((type & OPL_TYPE_ADPCM)!=0) state_size += sizeof(YM_DELTAT);
#endif
            /* allocate memory block */
            //ptr = malloc(state_size);
            //if (ptr == NULL) return null;
            /* clear */
            //memset(ptr, 0, state_size);
            OPL = new FM_OPL();//(FM_OPL*)ptr; ptr += sizeof(FM_OPL);
            OPL.P_CH = new OPL_CH[max_ch];//(OPL_CH*)ptr; ptr += sizeof(OPL_CH) * max_ch;
            for (int i = 0; i < max_ch; i++)
                OPL.P_CH[i] = new OPL_CH();
#if BUILD_Y8950
            if ((type & OPL_TYPE_ADPCM) != 0)
                OPL.deltat = new YM_DELTAT();//(YM_DELTAT*)ptr; ptr += sizeof(YM_DELTAT);
#endif
            /* set channel state pointer */
            OPL.type = (byte)type;
            OPL.clock = clock;
            OPL.rate = rate;
            OPL.max_ch = max_ch;
            /* init grobal tables */
            OPL_initalize(OPL);
            /* reset chip */
            OPLResetChip(OPL);
            return OPL;
        }

        public static void OPLSetTimerHandler(FM_OPL OPL, OPL_TIMERHANDLER TimerHandler, int channelOffset)
        {
            OPL.TimerHandler = TimerHandler;
            OPL.TimerParam = channelOffset;
        }
        public static void OPLSetIRQHandler(FM_OPL OPL, OPL_IRQHANDLER IRQHandler, int param)
        {
            OPL.IRQHandler = IRQHandler;
            OPL.IRQParam = param;
        }
        public static void OPLSetUpdateHandler(FM_OPL OPL, OPL_UPDATEHANDLER UpdateHandler, int param)
        {
            OPL.UpdateHandler = UpdateHandler;
            OPL.UpdateParam = param;
        }
        public static void OPLSetPortHandler(FM_OPL OPL, OPL_PORTHANDLER_W PortHandler_w, OPL_PORTHANDLER_R PortHandler_r, int param)
        {
            OPL.porthandler_w = PortHandler_w;
            OPL.porthandler_r = PortHandler_r;
            OPL.port_param = param;
        }

        public static void OPLSetKeyboardHandler(FM_OPL OPL, OPL_PORTHANDLER_W KeyboardHandler_w, OPL_PORTHANDLER_R KeyboardHandler_r, int param)
        {
            OPL.keyboardhandler_w = KeyboardHandler_w;
            OPL.keyboardhandler_r = KeyboardHandler_r;
            OPL.keyboard_param = param;
        }
        static void CSMKeyControll(OPL_CH CH)
        {
            OPL_SLOT slot1 = CH.SLOT[SLOT1];
            OPL_SLOT slot2 = CH.SLOT[SLOT2];
            /* all key off */
            OPL_KEYOFF(slot1);
            OPL_KEYOFF(slot2);
            /* total level latch */
            slot1.TLL = (int)(slot1.TL + (CH.ksl_base >> slot1.ksl));
            slot1.TLL = (int)(slot1.TL + (CH.ksl_base >> slot1.ksl));
            /* key on */
            CH.op1_out[0] = CH.op1_out[1] = 0;
            OPL_KEYON(slot1);
            OPL_KEYON(slot2);
        }

        public static int OPLTimerOver(FM_OPL OPL, int c)
        {
            if (c != 0)
            {	/* Timer B */
                OPL_STATUS_SET(OPL, 0x20);
            }
            else
            {	/* Timer A */
                OPL_STATUS_SET(OPL, 0x40);
                /* CSM mode key,TL controll */
                if ((OPL.mode & 0x80) != 0)
                {	/* CSM mode total level latch and auto key on */
                    int ch;
                    if (OPL.UpdateHandler != null) OPL.UpdateHandler(OPL.UpdateParam, 0);
                    for (ch = 0; ch < 9; ch++)
                        CSMKeyControll(OPL.P_CH[ch]);
                }
            }
            /* reload timer */
            if (OPL.TimerHandler != null) OPL.TimerHandler(OPL.TimerParam + c, (double)OPL.T[c] * OPL.TimerBase);
            return OPL.status >> 7;
        }
        public static void YM3812UpdateOne(FM_OPL OPL, _ShortPtr buffer, int length)
        {
            int i;
            int data;
            _ShortPtr buf = buffer;
            uint amsCnt = (uint)OPL.amsCnt;
            uint vibCnt = (uint)OPL.vibCnt;
            byte rythm = (byte)(OPL.rythm & 0x20);
            OPL_CH CH;
            int R_CH;

            if ((object)OPL != cur_chip)
            {
                cur_chip = OPL;
                /* channel pointers */
                S_CH = OPL.P_CH;
                E_CH = 9;// S_CH[9];
                /* rythm slot */
                SLOT7_1 = S_CH[7].SLOT[SLOT1];
                SLOT7_2 = S_CH[7].SLOT[SLOT2];
                SLOT8_1 = S_CH[8].SLOT[SLOT1];
                SLOT8_2 = S_CH[8].SLOT[SLOT2];
                /* LFO state */
                amsIncr = OPL.amsIncr;
                vibIncr = OPL.vibIncr;
                ams_table = OPL.ams_table;
                vib_table = OPL.vib_table;
            }
            R_CH = rythm != 0 ? 6 : E_CH;
            for (i = 0; i < length; i++)
            {
                /*            channel A         channel B         channel C      */
                /* LFO */
                ams = ams_table[(int)((amsCnt += (uint)amsIncr) >> AMS_SHIFT)];
                vib = vib_table[(int)((vibCnt += (uint)vibIncr) >> VIB_SHIFT)];
                outd[0] = 0;
                /* FM part */
                for (int k = 0; k != R_CH; k++)
                {
                    CH = S_CH[k];
                    //for(CH=S_CH ; CH < R_CH ; CH++)
                    OPL_CALC_CH(CH);
                }
                /* Rythn part */
                if (rythm != 0)
                    OPL_CALC_RH(S_CH);
                /* limit check */
                data = Limit(outd[0], OPL_MAXOUT, OPL_MINOUT);
                /* store to sound buffer */
                buf.write16(i, (ushort)(data >> OPL_OUTSB));
            }

            OPL.amsCnt = (int)amsCnt;
            OPL.vibCnt = (int)vibCnt;
        }
        static int Limit(int val, int max, int min)
        {
            if (val > max)
                val = max;
            else if (val < min)
                val = min;

            return val;
        }
        static int OP_OUT(OPL_SLOT slot, int env, int con)
        {
            throw new Exception();
          //  return slot.wavetable[((slot.Cnt + con) / (0x1000000 / SIN_ENT)) & (SIN_ENT - 1)][env];
        }
        static uint OPL_CALC_SLOT(OPL_SLOT SLOT)
        {
            /* calcrate envelope generator */
            if ((SLOT.evc += SLOT.evs) >= SLOT.eve)
            {
                switch (SLOT.evm)
                {
                    case ENV_MOD_AR: /* ATTACK . DECAY1 */
                        /* next DR */
                        SLOT.evm = ENV_MOD_DR;
                        SLOT.evc = EG_DST;
                        SLOT.eve = SLOT.SL;
                        SLOT.evs = SLOT.evsd;
                        break;
                    case ENV_MOD_DR: /* DECAY . SL or RR */
                        SLOT.evc = SLOT.SL;
                        SLOT.eve = EG_DED;
                        if (SLOT.eg_typ != 0)
                        {
                            SLOT.evs = 0;
                        }
                        else
                        {
                            SLOT.evm = ENV_MOD_RR;
                            SLOT.evs = SLOT.evsr;
                        }
                        break;
                    case ENV_MOD_RR: /* RR . OFF */
                        SLOT.evc = EG_OFF;
                        SLOT.eve = EG_OFF + 1;
                        SLOT.evs = 0;
                        break;
                }
            }
            /* calcrate envelope */
            return (uint)(SLOT.TLL + ENV_CURVE[SLOT.evc >> ENV_BITS] + (SLOT.ams != 0 ? ams : 0));
        }
        static void OPL_CALC_CH(OPL_CH CH)
        {
            uint env_out;
            OPL_SLOT SLOT;

            feedback2[0] = 0;
            /* SLOT 1 */
            SLOT = CH.SLOT[SLOT1];
            env_out = OPL_CALC_SLOT(SLOT);
            if (env_out < EG_ENT - 1)
            {
                /* PG */
                if (SLOT.vib != 0) SLOT.Cnt += (uint)(SLOT.Incr * vib / VIB_RATE);
                else SLOT.Cnt += SLOT.Incr;
                /* connectoion */
                if (CH.FB != 0)
                {
                    int feedback1 = (CH.op1_out[0] + CH.op1_out[1]) >> CH.FB;
                    CH.op1_out[1] = CH.op1_out[0];
                    CH.connect1[0] += CH.op1_out[0] = OP_OUT(SLOT, (int)env_out, feedback1);
                }
                else
                {
                    CH.connect1[0] += OP_OUT(SLOT, (int)env_out, 0);
                }
            }
            else
            {
                CH.op1_out[1] = CH.op1_out[0];
                CH.op1_out[0] = 0;
            }
            /* SLOT 2 */
            SLOT = CH.SLOT[SLOT2];
            env_out = OPL_CALC_SLOT(SLOT);
            if (env_out < EG_ENT - 1)
            {
                /* PG */
                if (SLOT.vib != 0) SLOT.Cnt += (uint)(SLOT.Incr * vib / VIB_RATE);
                else SLOT.Cnt += SLOT.Incr;
                /* connectoion */
                outd[0] += OP_OUT(SLOT, (int)env_out, feedback2[0]);
            }
        }
        static void OPL_CALC_RH(OPL_CH[] CH)
        {
            uint env_tam, env_sd, env_top, env_hh;
            int whitenoise = (int)((Mame.rand() & 1) * (WHITE_NOISE_db / EG_STEP));
            int tone8;

            OPL_SLOT SLOT;
            int env_out;

            /* BD : same as FM serial mode and output level is large */
            feedback2[0] = 0;
            /* SLOT 1 */
            SLOT = CH[6].SLOT[SLOT1];
            env_out = (int)OPL_CALC_SLOT(SLOT);
            if (env_out < EG_ENT - 1)
            {
                /* PG */
                if (SLOT.vib != 0) SLOT.Cnt += (uint)((SLOT.Incr * vib / VIB_RATE));
                else SLOT.Cnt += SLOT.Incr;
                /* connectoion */
                if (CH[6].FB != 0)
                {
                    int feedback1 = (CH[6].op1_out[0] + CH[6].op1_out[1]) >> CH[6].FB;
                    CH[6].op1_out[1] = CH[6].op1_out[0];
                    feedback2[0] = CH[6].op1_out[0] = OP_OUT(SLOT, env_out, feedback1);
                }
                else
                {
                    feedback2[0] = OP_OUT(SLOT, env_out, 0);
                }
            }
            else
            {
                feedback2[0] = 0;
                CH[6].op1_out[1] = CH[6].op1_out[0];
                CH[6].op1_out[0] = 0;
            }
            /* SLOT 2 */
            SLOT = CH[6].SLOT[SLOT2];
            env_out = (int)OPL_CALC_SLOT(SLOT);
            if (env_out < EG_ENT - 1)
            {
                /* PG */
                if (SLOT.vib != 0) SLOT.Cnt += (uint)(SLOT.Incr * vib / VIB_RATE);
                else SLOT.Cnt += SLOT.Incr;
                /* connectoion */
                outd[0] += OP_OUT(SLOT, env_out, feedback2[0]) * 2;
            }

            // SD  (17) = mul14[fnum7] + white noise
            // TAM (15) = mul15[fnum8]
            // TOP (18) = fnum6(mul18[fnum8]+whitenoise)
            // HH  (14) = fnum7(mul18[fnum8]+whitenoise) + white noise
            env_sd = (uint)(OPL_CALC_SLOT(SLOT7_2) + whitenoise);
            env_tam = OPL_CALC_SLOT(SLOT8_1);
            env_top = OPL_CALC_SLOT(SLOT8_2);
            env_hh = (uint)(OPL_CALC_SLOT(SLOT7_1) + whitenoise);

            /* PG */
            if (SLOT7_1.vib != 0) SLOT7_1.Cnt += (uint)(2 * SLOT7_1.Incr * vib / VIB_RATE);
            else SLOT7_1.Cnt += 2 * SLOT7_1.Incr;
            if (SLOT7_2.vib != 0) SLOT7_2.Cnt += (uint)((CH[7].fc * 8) * vib / VIB_RATE);
            else SLOT7_2.Cnt += (CH[7].fc * 8);
            if (SLOT8_1.vib != 0) SLOT8_1.Cnt += (uint)(SLOT8_1.Incr * vib / VIB_RATE);
            else SLOT8_1.Cnt += SLOT8_1.Incr;
            if (SLOT8_2.vib != 0) SLOT8_2.Cnt += (uint)((CH[8].fc * 48) * vib / VIB_RATE);
            else SLOT8_2.Cnt += (CH[8].fc * 48);

            tone8 = OP_OUT(SLOT8_2, whitenoise, 0);

            /* SD */
            if (env_sd < EG_ENT - 1)
                outd[0] += OP_OUT(SLOT7_1, (int)env_sd, 0) * 8;
            /* TAM */
            if (env_tam < EG_ENT - 1)
                outd[0] += OP_OUT(SLOT8_1, (int)env_tam, 0) * 2;
            /* TOP-CY */
            if (env_top < EG_ENT - 1)
                outd[0] += OP_OUT(SLOT7_2, (int)env_top, tone8) * 2;
            /* HH */
            if (env_hh < EG_ENT - 1)
                outd[0] += OP_OUT(SLOT7_2, (int)env_hh, tone8) * 2;
        }
        /* set multi,am,vib,EG-TYP,KSR,mul */
        static void set_mul(FM_OPL OPL, int slot, int v)
        {
            OPL_CH CH = OPL.P_CH[slot / 2];
            OPL_SLOT SLOT = CH.SLOT[slot & 1];

            SLOT.mul = MUL_TABLE[v & 0x0f];
            SLOT.KSR = ((v & 0x10) != 0) ? (byte)0 : (byte)2;
            SLOT.eg_typ = (byte)((v & 0x20) >> 5);
            SLOT.vib = (byte)((v & 0x40));
            SLOT.ams = (byte)(v & 0x80);
            CALC_FCSLOT(CH, SLOT);
        }

        /* set ksl & tl */
        static void set_ksl_tl(FM_OPL OPL, int slot, int v)
        {
            OPL_CH CH = OPL.P_CH[slot / 2];
            OPL_SLOT SLOT = CH.SLOT[slot & 1];
            int ksl = v >> 6; /* 0 / 1.5 / 3 / 6 db/OCT */

            SLOT.ksl = (byte)(ksl != 0 ? 3 - ksl : 31);
            SLOT.TL = (int)((v & 0x3f) * (0.75 / EG_STEP)); /* 0.75db step */

            if ((OPL.mode & 0x80) == 0)
            {	/* not CSM latch total level */
                SLOT.TLL = (int)(SLOT.TL + (CH.ksl_base >> SLOT.ksl));
            }
        }

        /* set attack rate & decay rate  */
        static void set_ar_dr(FM_OPL OPL, int slot, int v)
        {
            OPL_CH CH = OPL.P_CH[slot / 2];
            OPL_SLOT SLOT = CH.SLOT[slot & 1];
            int ar = v >> 4;
            int dr = v & 0x0f;

            SLOT.AR = ar != 0 ? new IntSubArray(OPL.AR_TABLE, ar << 2) : new IntSubArray(RATE_0);
            SLOT.evsa = SLOT.AR[SLOT.ksr];
            if (SLOT.evm == ENV_MOD_AR) SLOT.evs = SLOT.evsa;

            SLOT.DR = dr != 0 ? new IntSubArray(OPL.DR_TABLE, dr << 2) : new IntSubArray(RATE_0);
            SLOT.evsd = SLOT.DR[SLOT.ksr];
            if (SLOT.evm == ENV_MOD_DR) SLOT.evs = SLOT.evsd;
        }

        /* set sustain level & release rate */
        static void set_sl_rr(FM_OPL OPL, int slot, int v)
        {
            OPL_CH CH = OPL.P_CH[slot / 2];
            OPL_SLOT SLOT = CH.SLOT[slot & 1];
            int sl = v >> 4;
            int rr = v & 0x0f;

            SLOT.SL = SL_TABLE[sl];
            if (SLOT.evm == ENV_MOD_DR) SLOT.eve = SLOT.SL;
            SLOT.RR = new IntSubArray(OPL.DR_TABLE, rr << 2);
            SLOT.evsr = SLOT.RR[SLOT.ksr];
            if (SLOT.evm == ENV_MOD_RR) SLOT.evs = SLOT.evsr;
        }

    }
}
