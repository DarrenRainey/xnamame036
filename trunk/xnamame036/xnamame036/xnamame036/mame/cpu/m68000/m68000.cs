using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class m68000:cpu_interface
        {
            const byte MC68000_INT_NONE = 0;
            const byte MC68000_IRQ_1 = 1;
            const byte MC68000_IRQ_2 = 2;
            const byte MC68000_IRQ_3 = 3;
            const byte MC68000_IRQ_4 = 4;
            const byte MC68000_IRQ_5 = 5;
            const byte MC68000_IRQ_6 = 6;
            const byte MC68000_IRQ_7 = 7;
            const int MC68000_INT_ACK_AUTOVECTOR = -1;
            const int MC68000_INT_ACK_SPURIOUS = -2;
            const byte MC68000_CPU_MDOE_68000 = 1;
            const byte MC68000_CPU_MODE_68010 = 2;
            const byte MC68000_CPU_MODE_68020 = 4;
            public m68000()
            {
                cpu_num = CPU_M68000;
                num_irqs = 8;
                default_vector = -1;
                overclock = 1.0;
                no_int = MC68000_INT_NONE;
                irq_int = -1;
                nmi_int = -1;
                address_shift = 0;
                address_bits = 24;
                endianess = CPU_IS_BE;
                align_unit = 2;
                max_inst_len = 10;
                abits1 = ABITS1_24;
                abits2 = ABITS2_24;
                abitsmin = ABITS_MIN_24;
            }
            public override void burn(int cycles)
            {
                throw new NotImplementedException();
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                throw new NotImplementedException();
            }
            public override void cpu_state_load(object file)
            {
                throw new NotImplementedException();
            }
            public override void cpu_state_save(object file)
            {
                throw new NotImplementedException();
            }
            public override void exit()
            {
                throw new NotImplementedException();
            }
            public override int execute(int cycles)
            {
                throw new NotImplementedException();
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override uint get_pc()
            {
                throw new NotImplementedException();
            }
            public override uint get_reg(int regnum)
            {
                throw new NotImplementedException();
            }
            public override uint get_sp()
            {
                throw new NotImplementedException();
            }
            public override void internal_interrupt(int type)
            {
                throw new NotImplementedException();
            }
            public override int memory_read(int offset)
            {
                throw new NotImplementedException();
            }
            public override void memory_write(int offset, int data)
            {
                throw new NotImplementedException();
            }
            public override void reset(object param)
            {
                throw new NotImplementedException();
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_line(int irqline, int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_nmi_line(int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_op_base(int pc)
            {
                throw new NotImplementedException();
            }
            public override void set_pc(uint val)
            {
                throw new NotImplementedException();
            }
            public override void set_reg(int regnum, uint val)
            {
                throw new NotImplementedException();
            }
            public override void set_sp(uint val)
            {
                throw new NotImplementedException();
            }   }
    }
}