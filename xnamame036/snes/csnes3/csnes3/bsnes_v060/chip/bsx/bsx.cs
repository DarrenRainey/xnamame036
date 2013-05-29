using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        class BSXBase : MMIO
        {
            public byte mmio_read(uint addr)
            {
                throw new NotImplementedException();
            }
            public void mmio_write(uint addr, byte data)
            {
                throw new NotImplementedException();
            }
        }
        class BSXCart : MMIO 
        {
            public byte mmio_read(uint addr)
            {
                throw new NotImplementedException();
            }
            public void mmio_write(uint addr, byte data)
            {
                throw new NotImplementedException();
            }
        }
        class BSXFlash : Memory
        {
            public override byte read(uint addr)
            {
                throw new NotImplementedException();
            }
            public override void write(uint addr, byte data)
            {
                throw new NotImplementedException();
            }
            public override uint size()
            {
                throw new NotImplementedException();
            }
        }
    }
}
