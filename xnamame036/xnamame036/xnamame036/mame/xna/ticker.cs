using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        static long TICKS_PER_SEC = new TimeSpan(0, 0, 1).Ticks;

        public static void init_ticker()
        {
            TICKS_PER_SEC = 1000 * 10000;
        }
        static long ticker() { return DateTime.Now.Ticks; }
    }
}
