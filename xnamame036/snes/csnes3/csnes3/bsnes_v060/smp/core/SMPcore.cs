using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class SMPcore
        {
            class regya_t
            {
                public byte hi, lo;
            }
            class flag_t
            {
                public bool n, v, p, b, h, i, z, c;
            }
            class regs_t
            {
                public ushort pc;
                public byte a, x, y, sp;
                public regya_t ya = new regya_t();
                public flag_t p = new flag_t();
            }
             regs_t regs=new regs_t();
             ushort dp, sp, rd, wr, bit, ya;
        }
    }
}