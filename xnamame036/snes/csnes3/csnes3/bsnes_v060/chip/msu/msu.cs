using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public static MSU msu = new MSU();

        public class MSU:MMIO
        {
            string basename;

            public byte mmio_read(uint addr)
            {
                throw new NotImplementedException();
            }
            public void mmio_write(uint addr, byte data)
            {
                throw new NotImplementedException();
            }
            public void _base(string name)
            {
                basename = name;
            }
        }
    }
}
