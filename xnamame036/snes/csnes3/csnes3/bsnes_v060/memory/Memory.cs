using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    public abstract class Memory
    {
        public virtual uint size() { return 0; }
        public abstract byte read(uint addr);
        public abstract void write(uint addr, byte data);
    }          
}
