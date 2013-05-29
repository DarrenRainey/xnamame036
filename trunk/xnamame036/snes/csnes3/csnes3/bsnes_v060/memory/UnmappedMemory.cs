using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public class UnmappedMemory : Memory
        {
            public override uint size()
            {
                return 16 * 1024 * 1024;
            }
            public override byte read(uint addr)
            {
                return cpu.regs.mdr;
            }
            public override void write(uint addr, byte data) { }
        }
    }
}