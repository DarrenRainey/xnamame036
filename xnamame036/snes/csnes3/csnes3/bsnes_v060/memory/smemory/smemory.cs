using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static sBus bus = new sBus();
        partial class sBus : Bus
        {
            void power()
            {
                for (uint i = 0x2000; i <= 0x5fff; i++) memory.mmio.map(i, memory.mmio_unmapped);
                for (uint i = 0; i < memory.wram.size(); i++) memory.wram[i] = (byte)config.cpu.wram_init_value;
            }

            void reset()
            {
            }

            public bool load_cart()
            {
                if (cartridge.loaded == true) return false;

                map_reset();
                map_generic();
                map_system();
                return true;
            }

            public void unload_cart()
            {
            }

        }
    }
}
