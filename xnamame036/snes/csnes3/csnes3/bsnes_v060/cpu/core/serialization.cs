using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class CPUcore
        {
            public void core_serialize(Serializer s)
            {
                s.integer(regs.pc.d);

                s.integer(regs.a.w);
                s.integer(regs.x.w);
                s.integer(regs.y.w);
                s.integer(regs.z.w);
                s.integer(regs.s.w);
                s.integer(regs.d.w);

                s.integer(regs.p.n);
                s.integer(regs.p.v);
                s.integer(regs.p.m);
                s.integer(regs.p.x);
                s.integer(regs.p.d);
                s.integer(regs.p.i);
                s.integer(regs.p.z);
                s.integer(regs.p.c);

                s.integer(regs.db);
                s.integer(regs.e);
                s.integer(regs.irq);
                s.integer(regs.wai);
                s.integer(regs.mdr);

                s.integer(aa.d);
                s.integer(rd.d);
                s.integer(sp);
                s.integer(dp);

                update_table();
            }
        }
    }
}
