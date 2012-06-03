using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAX_MSM5205 = 4;
    }
    class MSM5205
    {
        struct MSM5205Voice
        {
            public int stream;
            public object timer;
            public int data;
            public int vclk;
            public int reset;
            public int prescaler;
            public int bitwidth;
            public int signal;
            public int step;
        }
        static MSM5205Voice[] msm5205 = new MSM5205Voice[Mame.MAX_MSM5205];
        public static void MSM5205_data_w(int num, int data)
        {
            if (msm5205[num].bitwidth == 4)
                msm5205[num].data = data & 0x0f;
            else
                msm5205[num].data = (data & 0x07) << 1; /* unknown */
        }

    }
}
