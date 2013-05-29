using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        class sSMPTimer
        {
            public sSMPTimer(uint cycle_frequency) { this.cycle_frequency = cycle_frequency; }
            public uint cycle_frequency;
            public byte target;
            public byte stage1_ticks, stage2_ticks, stage3_ticks;
            public bool enabled;
        }
        partial class sSMP
        {
            sSMPTimer t0 = new sSMPTimer(128);
            sSMPTimer t1 = new sSMPTimer(128);
            sSMPTimer t2 = new sSMPTimer(16);
        }
    }
}
