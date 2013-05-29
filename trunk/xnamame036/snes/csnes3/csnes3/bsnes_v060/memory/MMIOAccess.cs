using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    class MMIOAccess:Memory
    {
        MMIO[] mmio = new MMIO[0x4000];

        public void dmap(uint addr, MMIO access)
        {
            mmio[(addr - 0x2000) & 0x3fff] = access;
        }
        public override byte read(uint addr)
        {
            return mmio[(addr - 0x2000) & 0x3fff].mmio_read(addr);
        }
        public override void write(uint addr, byte data)
        {
            mmio[(addr - 0x2000) & 0x3fff].mmio_write(addr, data);
        }
        public void map(uint addr, MMIO access)
        {
            mmio[(addr - 0x2000) & 0x3fff] = access;
        }
    }
}
