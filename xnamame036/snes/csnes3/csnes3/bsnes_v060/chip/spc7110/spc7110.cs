using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static SPC7110 spc7110 = new SPC7110();
        
        partial class SPC7110:Memory //Must also implement mmio
        {
            public override uint size()
            {
                throw new NotImplementedException();
            }
            public override byte read(uint addr)
            {
                throw new NotImplementedException();
            }
            public override void write(uint addr, byte data)
            {
                throw new NotImplementedException();
            }
        }
    }
}
