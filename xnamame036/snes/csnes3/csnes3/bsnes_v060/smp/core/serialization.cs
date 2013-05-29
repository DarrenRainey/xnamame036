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
            public void core_serialize(Serializer s)
            {
                s.integer(regs.pc);
                s.integer(regs.a);
                s.integer(regs.x);
                s.integer(regs.y);
                s.integer(regs.sp);
                s.integer(regs.p.n);
                s.integer(regs.p.v);
                s.integer(regs.p.p);
                s.integer(regs.p.b);
                s.integer(regs.p.h);
                s.integer(regs.p.i);
                s.integer(regs.p.z);
                s.integer(regs.p.c);

                s.integer(dp);
                s.integer(sp);
                s.integer(rd);
                s.integer(wr);
                s.integer(bit);
                s.integer(ya);
            }
        }
    }
}
