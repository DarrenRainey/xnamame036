using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class Cartridge
        {
            uint gameboy_ram_size()
            {
                if (memory.gbrom.size() < 512) return 0;
                switch (memory.gbrom[0x0149])
                {
                    case 0x00: return 0 * 1024;
                    case 0x01: return 8 * 1024;
                    case 0x02: return 8 * 1024;
                    case 0x03: return 32 * 1024;
                    case 0x04: return 128 * 1024;
                    case 0x05: return 128 * 1024;
                    default: return 128 * 1024;
                }
            }

            uint gameboy_rtc_size()
            {
                if (memory.gbrom.size() < 512) return 0;
                if (memory.gbrom[0x0147] == 0x0f || memory.gbrom[0x0147] == 0x10) return 4;
                return 0;
            }
        }
    }
}
