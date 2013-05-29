using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    public interface MMIO
    {
        byte mmio_read(uint addr);
        void mmio_write(uint addr, byte data);
    }
}
