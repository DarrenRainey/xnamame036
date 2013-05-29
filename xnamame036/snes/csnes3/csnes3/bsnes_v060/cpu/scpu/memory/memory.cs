using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sCPU
        {
            byte[] apu_port = new byte[4];
            byte port_read(byte port) { return apu_port[port & 3]; }
            void port_write(byte port, byte data) { apu_port[port & 3] = data; }
            public override void op_io()
            {
                status.clock_count = 6;
                cycle_edge();
                add_clocks(6);
            }

            public override byte op_read(uint addr)
            {
                status.clock_count = speed(addr);
                cycle_edge();
                add_clocks(status.clock_count - 4);
                regs.mdr = bus.read(addr);
                add_clocks(4);
                return regs.mdr;
            }
            public uint speed(uint addr)
            {
                if ((addr & 0x408000)!=0)
                {
                    if ((addr & 0x800000) !=0)return status.rom_speed;
                    return 8;
                }
                if (((addr + 0x6000) & 0x4000) !=0) return 8;
                if (((addr - 0x4000) & 0x7e00) !=0) return 6;
                return 12;
            }
            public override void op_write(uint addr, byte data)
            {
                status.clock_count = speed(addr);
                cycle_edge();
                add_clocks(status.clock_count);
                bus.write(addr, regs.mdr = data);
            }

        }
    }
}
