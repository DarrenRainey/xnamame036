using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sBus
        {
            public void serialize(Serializer s)
            {
                s.array(memory.wram.data(), memory.wram.size());
                s.array(memory.apuram.data(), memory.apuram.size());
                s.array(memory.vram.data(), memory.vram.size());
                s.array(memory.oam.data(), memory.oam.size());
                s.array(memory.cgram.data(), memory.cgram.size());
            }
        }
    }
}
