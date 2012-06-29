#define HAS_YM2151

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAX_2151 = 3;
    }
    
    public delegate void YM2151irqhandler(int i);
    class YM2151interface
    {
        public int num;
        public int baseclock;
        public int[] volume = new int[Mame.MAX_2151];
        public YM2151irqhandler[] irqhandler = new YM2151irqhandler[Mame.MAX_2151];
        public Mame.mem_write_handler[] portwritehandler;
        public YM2151interface(int num, int baseclock, int[] volume, YM2151irqhandler[] irqhandler, Mame.mem_write_handler[] writehandler)
        {
            this.num = num;
            this.volume = volume;
            this.baseclock = baseclock;
            this.irqhandler = irqhandler;
            this.portwritehandler = writehandler;
        }
    }
    class YM2151 : Mame.snd_interface
    {

        public YM2151()
        {
            this.name = "YM-2151";
            this.sound_num = Mame.SOUND_YM2151;
        }
        public static int YM3012_VOL(int LVol, int LPan, int RVol, int RPan)
        {
            return (Mame.MIXER(LVol, LPan) | (Mame.MIXER(RVol, RPan) << 16));
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((YM2151interface)msound.sound_interface).baseclock;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((YM2151interface)msound.sound_interface).num;
        }
        public override int start(Mame.MachineSound msound)
        {
            return my_YM2151_sh_start(msound, 0);
        }
        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void reset()
        {
            for (int i = 0; i < intf.num; i++)
                switch (FMMode)
                {
                    case CHIP_YM2151_DAC: FM.OPMResetChip(i); break;
                    //case CHIP_YM2151_ALT: fm.YM2151ResetChip(i); break;
                }
        }
        public override void update()
        {
            //nothing
        }
        public static int YM2151_status_port_0_r(int offset)
        {
            switch (FMMode)
            {
#if (HAS_YM2151)
                case CHIP_YM2151_DAC:
                    return FM.YM2151Read(0, 1);
#endif
#if (HAS_YM2151_ALT)
	case CHIP_YM2151_ALT:
		return YM2151ReadStatus(0);
#endif
            }
            return 0;
        }
        static int lastreg0, lastreg1, lastreg2;
        public static void YM2151_register_port_0_w(int offset, int data)
        {
            lastreg0 = data;
        }
        public static void YM2151UpdateRequest(int chip)
        {
            Mame.stream_update(stream[chip], 0);
        }
        public static void YM2151_data_port_0_w(int offset, int data)
        {
            switch (FMMode)
            {
#if (HAS_YM2151)
                case CHIP_YM2151_DAC:
                    FM.YM2151Write(0, 0, (byte)lastreg0);
                    FM.YM2151Write(0, 1, (byte)data);
                    break;
#endif
#if (HAS_YM2151_ALT)
	case CHIP_YM2151_ALT:
		YM2151UpdateRequest(0);
		YM2151WriteReg(0,lastreg0,data);
		break;
#endif
            }
        }
        static YM2151interface intf;
        const byte YM2151_NUMBUF = 2;
        static int FMMode;
        const byte CHIP_YM2151_DAC = 4;
        const byte CHIP_YM2151_ALT = 5;
        static int[] stream = new int[Mame.MAX_2151];
        static object[][] Timer = new object[Mame.MAX_2151][];

        static void IRQHandler(int n, int irq)
        {
            if (intf.irqhandler != null && intf.irqhandler[n] != null) intf.irqhandler[n](irq);
        }
        static void timer_callback_2151(int param)
        {
            int n = param & 0x7f;
            int c = param >> 7;

            Timer[n][c] = null;
            FM.YM2151TimerOver(n, c);
        }
        static void TimerHandler(int n, int c, int count, double stepTime)
        {
            if (count == 0)
            {	/* Reset FM Timer */
                if (Timer[n][c] != null)
                {
                    Mame.Timer.timer_remove(Timer[n][c]);
                    Timer[n][c] =null;
                }
            }
            else
            {	/* Start FM Timer */
                double timeSec = (double)count * stepTime;

                if (Timer[n][c] == null)
                {
                    Timer[n][c] = Mame.Timer.timer_set(timeSec, (c << 7) | n, timer_callback_2151);
                }
            }
        }

        static int my_YM2151_sh_start(Mame.MachineSound msound, int mode)
        {
            int i, j;
            int rate = Mame.Machine.sample_rate;

            string[] name = new string[YM2151_NUMBUF];
            int mixed_vol;
            int[] vol = new int[YM2151_NUMBUF];

            if (rate == 0) rate = 1000;	/* kludge to prevent nasty crashes */

            intf = (YM2151interface)msound.sound_interface;

            if (mode != 0) FMMode = CHIP_YM2151_ALT;
            else FMMode = CHIP_YM2151_DAC;

            switch (FMMode)
            {
#if (HAS_YM2151)
                case CHIP_YM2151_DAC:	/* Tatsuyuki's */
                    /* stream system initialize */
                    for (i = 0; i < intf.num; i++)
                    {
                        mixed_vol = intf.volume[i];
                        /* stream setup */
                        for (j = 0; j < YM2151_NUMBUF; j++)
                        {

                            vol[j] = mixed_vol & 0xffff;
                            mixed_vol >>= 16;
                            name[j] = Mame.sprintf("%s #%d Ch%d", Mame.sound_name(msound), i, j + 1);
                        }
                        stream[i] = Mame.stream_init_multi(YM2151_NUMBUF, name, vol, rate, i, FM.OPMUpdateOne);
                    }
                    /* Set Timer handler */
                    for (i = 0; i < intf.num; i++)
                    {
                        Timer[i] = new object[2];
                        Timer[i][0] = null;
                        Timer[i][1] = null;
                    }
                    if (FM.OPMInit(intf.num, intf.baseclock, Mame.Machine.sample_rate, TimerHandler, IRQHandler) == 0)
                    {
                        /* set port handler */
                        for (i = 0; i < intf.num; i++)
                            FM.OPMSetPortHandler(i, intf.portwritehandler[i]);
                        return 0;
                    }
                    /* error */
                    return 1;
#endif
#if (HAS_YM2151_ALT)
	case CHIP_YM2151_ALT:	/* Jarek's */
		/* stream system initialize */
		for (i = 0;i < intf.num;i++)
		{
			/* stream setup */
			mixed_vol = intf.volume[i];
			for (j = 0 ; j < YM2151_NUMBUF ; j++)
			{
				name[j]=buf[j];
				vol[j] = mixed_vol & 0xffff;
				mixed_vol>>=16;
				sprintf(buf[j],"%s #%d Ch%d",sound_name(msound),i,j+1);
			}
			stream[i] = stream_init_multi(YM2151_NUMBUF,
				name,vol,rate,i,YM2151UpdateOne);
		}
		if (YM2151Init(intf.num,intf.baseclock,Machine.sample_rate) == 0)
		{
			for (i = 0; i < intf.num; i++)
			{
				YM2151SetIrqHandler(i,intf.irqhandler[i]);
				YM2151SetPortWriteHandler(i,intf.portwritehandler[i]);
			}
			return 0;
		}
		return 1;
#endif
            }
            return 1;

        }
    }
}
