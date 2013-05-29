using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{

    partial class SNES
    {
        public abstract partial class CPUcore
        {
            public regs_t regs = new regs_t();
            public reg24_t aa = new reg24_t(), rd = new reg24_t();
            public byte sp, dp;

            public abstract void op_io();
            public abstract byte op_read(uint addr);
            public abstract void op_write(uint addr, byte data);
            public abstract void last_cycle();
            public abstract bool interrupt_pending();

            opcode[][] op_table = new opcode[5][];

            const byte table_EM = 0, table_MX = 1,table_Mx=2,table_mX=3,table_mx=4;
        }
    }
}