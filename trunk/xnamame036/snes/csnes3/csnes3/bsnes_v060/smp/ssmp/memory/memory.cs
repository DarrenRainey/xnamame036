using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sSMP
        {
            public byte port_read(byte port) {
  return memory.apuram[(uint)(0xf4u + (port & 3))];
}

public void port_write(byte port, byte data) {
  memory.apuram[(uint)(0xf4 + (port & 3))] = data;
}

        }
    }
}
