using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public class UnmappedMMIO : MMIO
        {
            public byte mmio_read(uint addr)
            {
                return cpu.regs.mdr;
            }
            public void mmio_write(uint addr, byte data) { }
        }
    }
}