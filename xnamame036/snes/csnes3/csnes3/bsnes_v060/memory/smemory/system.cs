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
            void map_reset()
            {
                map(MapMode.MapDirect, 0x00, 0xff, 0x0000, 0xffff, memory.memory_unmapped);
                for (uint i = 0x2000; i <= 0x5fff; i++) memory.mmio.map(i, memory.mmio_unmapped);
            }
            void map_system()
            {
                map(MapMode.MapDirect, 0x00, 0x3f, 0x2000, 0x5fff, memory.mmio);
                map(MapMode.MapDirect, 0x80, 0xbf, 0x2000, 0x5fff, memory.mmio);

                map(MapMode.MapLinear, 0x00, 0x3f, 0x0000, 0x1fff, memory.wram, 0x000000, 0x002000);
                map(MapMode.MapLinear, 0x80, 0xbf, 0x0000, 0x1fff, memory.wram, 0x000000, 0x002000);

                map(MapMode.MapLinear, 0x7e, 0x7f, 0x0000, 0xffff, memory.wram);
            }
        }
    }
}
