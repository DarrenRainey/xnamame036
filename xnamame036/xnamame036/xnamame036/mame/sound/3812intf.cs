#define OPL3CONVERTFREQUENCY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const int MAX_3812 = 2;
        public const int MAX_8950 = MAX_3812;

    }
    public delegate void handlerdelegate(int linestate);

    public class YM3812interface
    {
        public YM3812interface(int num, int baseclock, int[] mixing_level, handlerdelegate[] handler = null)
        {
            this.num = num; this.baseclock = baseclock; this.mixing_level = mixing_level; this.handler = handler;
        }
        public int num;
        public int baseclock;
        public int[] mixing_level = new int[Mame.MAX_3812];
        public handlerdelegate[] handler;
    }
    public class Y8950interface:YM3812interface
    {
        public Y8950interface(int num, int baseclock, int[] mixing_level, handlerdelegate[] handler = null):base(num,baseclock,mixing_level,handler)
        {
        }
        /* Y8950 */
        public int[] rom_region = new int[Mame.MAX_8950]; /* delta-T ADPCM ROM region */
        //int (*keyboardread[MAX_8950])(int offset);
        //void (*keyboardwrite[MAX_8950])(int offset,int data);
        //int (*portread[MAX_8950])(int offset);
        //void (*portwrite[MAX_8950])(int offset,int data);
    }
    class YM3812 : Mame.snd_interface
    {
        public YM3812()
        {
            this.sound_num = Mame.SOUND_YM3812;
            this.name = "YM-3812";
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            throw new NotImplementedException();
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            throw new NotImplementedException();
        }
        public override int start(Mame.MachineSound msound)
        {
            chiptype = fm.OPL_TYPE_YM3812;
            return OPL_sh_start(msound);
        }
        public override void stop()
        {
            emu_YM3812_sh_stop();
        }
        public override void reset()
        {
            //nothing
        }
        public override void update()
        {
            //nothing
        }
        static YM3812interface intf = null;

        static int[] stream = new int[Mame.MAX_3812];
        static object[] Timer = new object[Mame.MAX_3812 * 2];

        public delegate void controlport_write(int chip, int data);
        public delegate int statusport_read(int chip);
        public delegate void _sh_stop();
        static _sh_stop sh_stop;
        static controlport_write control_port_w;
        static controlport_write write_port_w;
        static statusport_read read_port_r;
        static statusport_read status_port_r;
        const int ym3812_StdClock = 3579545;

        class non_emu3812_state
        {
            public int address_register;
            public byte status_register;
            public byte timer_register;
            public uint timer1_val;
            public uint timer2_val;
            public object timer1;
            public object timer2;
            public int[] aOPLFreqArray = new int[16];		/* Up to 9 channels.. */
        }
        static int chiptype;
        static fm.FM_OPL[] F3812 = new fm.FM_OPL[Mame.MAX_3812];

        static non_emu3812_state[] nonemu_state;
        static double timer_step;
        static int OPL_sh_start(Mame.MachineSound msound)
        {

                sh_stop = emu_YM3812_sh_stop;
                status_port_r = emu_YM3812_status_port_r;
                control_port_w = emu_YM3812_control_port_w;
                write_port_w = emu_YM3812_write_port_w;
                read_port_r = emu_YM3812_read_port_r;
                return emu_YM3812_sh_start(msound);
        }
        static int emu_YM3812_status_port_r(int chip)
        {
            return fm.OPLRead(F3812[chip], 0);
        }
        static void emu_YM3812_control_port_w(int chip, int data)
        {
            fm.OPLWrite(F3812[chip], 0, data);
        }
        static void emu_YM3812_write_port_w(int chip, int data)
        {
            fm.OPLWrite(F3812[chip], 1, data);
        }

        static int emu_YM3812_read_port_r(int chip)
        {
            return fm.OPLRead(F3812[chip], 1);
        }


        static void emu_YM3812_sh_stop()
        {
            int i;

            for (i = 0; i < intf.num; i++)
            {
                fm.OPLDestroy(ref F3812[i]);
            }
        }



        public static int YM3812_sh_start(Mame.MachineSound msound)
        {
            chiptype = fm.OPL_TYPE_YM3812;
            return OPL_sh_start(msound);
        }
        public static void YM3812_control_port_0_w(int offset, int data)
        {
            if (control_port_w == null) return;
            control_port_w(0, data);
        }
        public static void YM3812_write_port_0_w(int offset, int data)
        {
            if (write_port_w == null) return;
            write_port_w(0, data);
        }
        public static int emu_YM3812_sh_start(Mame.MachineSound msound)
        {
            int rate = Mame.Machine.sample_rate;

            intf = (YM3812interface)msound.sound_interface;
            if (intf.num > Mame.MAX_3812) return 1;

            /* Timer state clear */
            for (int i = 0; i < Timer.Length; i++) Timer[i] = null;

            /* stream system initialize */
            for (int i = 0; i < intf.num; i++)
            {
                /* stream setup */
                string name;
                int vol = intf.mixing_level[i];
                /* emulator create */
                
                F3812[i] = fm.OPLCreate(chiptype, intf.baseclock, rate);
                if (F3812[i] == null) return 1;
                /* stream setup */
                name = Mame.sprintf("%s #%d", Mame.sound_name(msound), i);
#if HAS_Y8950
		/* ADPCM ROM DATA */
		if(chiptype == OPL_TYPE_Y8950)
		{
			F3812[i].deltat.memory = (unsigned char *)(memory_region(intf.rom_region[i]));
			F3812[i].deltat.memory_size = memory_region_length(intf.rom_region[i]);
			stream[i] = stream_init(name,vol,rate,i,Y8950UpdateHandler);
			/* port and keyboard handler */
			OPLSetPortHandler(F3812[i],Y8950PortHandler_w,Y8950PortHandler_r,i);
			OPLSetKeyboardHandler(F3812[i],Y8950KeyboardHandler_w,Y8950KeyboardHandler_r,i);
		}
		else
#endif
                stream[i] = Mame.stream_init(name, vol, rate, i, YM3812UpdateHandler);
                /* YM3812 setup */
                fm.OPLSetTimerHandler(F3812[i], TimerHandler, i * 2);
                fm.OPLSetIRQHandler(F3812[i], IRQHandler, i);
                fm.OPLSetUpdateHandler(F3812[i], Mame.stream_update, stream[i]);
            }
            return 0;
        }
        static void IRQHandler(int n, int irq)
        {
            if (intf.handler == null) return;
            if (intf.handler[n] != null) (intf.handler[n])(irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        static void timer_callback_3812(int param)
        {
            int n = param >> 1;
            int c = param & 1;
            Timer[param] = null;
            fm.OPLTimerOver(F3812[n], c);
        }

        static void TimerHandler(int c, double period)
        {
            if (period == 0)
            {	/* Reset FM Timer */
                if (Timer[c] != null)
                {
                    Mame.Timer.timer_remove(Timer[c]);
                    Timer[c] = 0;
                }
            }
            else
            {	/* Start FM Timer */
                Timer[c] = Mame.Timer.timer_set(period, c, timer_callback_3812);
            }
        }
        static void YM3812UpdateHandler(int n, _ShortPtr buf, int length)
        { 
            fm.YM3812UpdateOne(F3812[n], buf, length); 
        }

        
        public static int YM3812_status_port_0_r(int offset)
        {
            return status_port_r(0);
        }

    }
}
