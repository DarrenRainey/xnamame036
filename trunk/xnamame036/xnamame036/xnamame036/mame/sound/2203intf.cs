using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class YM2203interface : AY8910interface
    {
        public YM2203interface(int num, int baseclock, int[] mixing_level, AY8910portRead[] portAread, AY8910portRead[] portBread, AY8910portWrite[] portAwrite, AY8910portWrite[] portBwrite, AY8910handler[] handler)
            : base(num, baseclock, mixing_level, portAread, portBread, portAwrite, portBwrite, handler) { }
    }
    public class ym2203 : Mame.snd_interface
    {
        const int MAX_2203 = 4;

        public static int YM2203_VOL(int FM_VOLUME, int SSG_VOLUME) { return (((FM_VOLUME) << 16) + (SSG_VOLUME)); }

        static int[] stream = new int[MAX_2203];
        static YM2203interface intf;
        static object[][] Timer = new object[MAX_2203][];


        public ym2203()
        {
            this.name = "YM-2203";
            this.sound_num = Mame.SOUND_YM2203;
            for (int i = 0; i < MAX_2203; i++) Timer[i] = new object[2];
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
           return ((YM2203interface)msound.sound_interface).baseclock; 
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((YM2203interface)msound.sound_interface).num; 
        }
        public override void reset()
        {
            for (int i = 0; i < intf.num; i++) FM.YM2203ResetChip(i);
        }
        public override int start(Mame.MachineSound msound)
        {
            int i;

            if (Mame.sndintf[Mame.SOUND_AY8910].start(msound) != 0) return 1;

            intf = (YM2203interface)msound.sound_interface;

            /* Timer Handler set */
            FMTimerInit();
            /* stream system initialize */
            for (i = 0; i < intf.num; i++)
            {
                int volume;
                string name = Mame.sprintf("%s #%d FM", Mame.sound_name(msound), i);
                volume = intf.mixing_level[i] >> 16; /* high 16 bit */
                stream[i] = Mame.stream_init(name, volume, Mame.Machine.sample_rate, i, FM.YM2203UpdateOne/*YM2203UpdateCallback*/);
            }
            /* Initialize FM emurator */
            if (FM.YM2203Init(intf.num, intf.baseclock, Mame.Machine.sample_rate, TimerHandler, IRQHandler) == 0)
            {
                /* Ready */
                return 0;
            }
            /* error */
            /* stream close */
            return 1;
        }
        public override void stop()
        {
            FM.YM2203Shutdown();
        }
        public override void update()
        {
           //none
        }
        static void timer_callback_2203(int param)
        {
            int n = param & 0x7f;
            int c = param >> 7;

            Timer[n][c] = null;
            FM.YM2203TimerOver(n, c);
        }
        static void FMTimerInit()
        {
            for (int i = 0; i < MAX_2203; i++)
            {
                Timer[i][0] = Timer[i][1] = null;
            }
        }
        static void TimerHandler(int n, int c, int count, double stepTime)
        {
            if (count == 0)
            {	/* Reset FM Timer */
                if (Timer[n][c]!=null)
                {
                    Mame.Timer.timer_remove(Timer[n][c]);
                    Timer[n][c] = null;
                }
            }
            else
            {	/* Start FM Timer */
                double timeSec = (double)count * stepTime;

                if (Timer[n][c] == null)
                {
                    Timer[n][c] = Mame.Timer.timer_set(timeSec, (c << 7) | n, timer_callback_2203);
                }
            }
        }
        static void IRQHandler(int n, int irq)
        {
            if (intf.handler[n]!=null) intf.handler[n](irq);
        }
        public static void YM2203UpdateRequest(int chip)
        {
            Mame.stream_update(stream[chip], 0);
        }




        public static int YM2203_status_port_0_r(int offset) { return FM.YM2203Read(0, 0); }
        public static int YM2203_status_port_1_r(int offset) { return FM.YM2203Read(1, 0); }
        public static int YM2203_status_port_2_r(int offset) { return FM.YM2203Read(2, 0); }
        public static int YM2203_status_port_3_r(int offset) { return FM.YM2203Read(3, 0); }
        public static int YM2203_status_port_4_r(int offset) { return FM.YM2203Read(4, 0); }

        public static int YM2203_read_port_0_r(int offset) { return FM.YM2203Read(0, 1); }
        public static int YM2203_read_port_1_r(int offset) { return FM.YM2203Read(1, 1); }
        public static int YM2203_read_port_2_r(int offset) { return FM.YM2203Read(2, 1); }
        public static int YM2203_read_port_3_r(int offset) { return FM.YM2203Read(3, 1); }
        public static int YM2203_read_port_4_r(int offset) { return FM.YM2203Read(4, 1); }

        public static void YM2203_control_port_0_w(int offset, int data)
        {
            FM.YM2203Write(0, 0, (byte)data);
        }
        public static void YM2203_control_port_1_w(int offset, int data)
        {
            FM.YM2203Write(1, 0, (byte)data);
        }
        public static void YM2203_control_port_2_w(int offset, int data)
        {
            FM.YM2203Write(2, 0, (byte)data);
        }
        public static void YM2203_control_port_3_w(int offset, int data)
        {
            FM.YM2203Write(3, 0, (byte)data);
        }
        public static void YM2203_control_port_4_w(int offset, int data)
        {
            FM.YM2203Write(4, 0, (byte)data);
        }

        public static void YM2203_write_port_0_w(int offset, int data)
        {
            FM.YM2203Write(0, 1, (byte)data);
        }
        public static void YM2203_write_port_1_w(int offset, int data)
        {
            FM.YM2203Write(1, 1, (byte)data);
        }
        public static void YM2203_write_port_2_w(int offset, int data)
        {
            FM.YM2203Write(2, 1, (byte)data);
        }
        public static void YM2203_write_port_3_w(int offset, int data)
        {
            FM.YM2203Write(3, 1, (byte)data);
        }
        public static void YM2203_write_port_4_w(int offset, int data)
        {
            FM.YM2203Write(4, 1, (byte)data);
        }
    }
}
