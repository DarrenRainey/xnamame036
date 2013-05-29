using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class CPUcore
        {
            public delegate void opcode();
            opcode[] opcode_table = new opcode[256];

            void update_table()
            {
                if (regs.e)
                {
                    opcode_table = op_table[table_EM];
                }
                else if (regs.p.m)
                {
                    if (regs.p.x)
                    {
                        opcode_table = op_table[table_MX];
                    }
                    else
                    {
                        opcode_table = op_table[table_Mx];
                    }
                }
                else
                {
                    if (regs.p.x)
                    {
                        opcode_table = op_table[table_mX];
                    }
                    else
                    {
                        opcode_table = op_table[table_mx];
                    }
                }
            }
        }
    }
}
