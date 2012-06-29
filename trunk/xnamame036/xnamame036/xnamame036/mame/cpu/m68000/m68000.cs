using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_m68000 : cpu_interface
        {
            public delegate void opcode();

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

            public cpu_m68000()
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
                icount = m68k_clks_left;
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
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "68000";
                    case CPU_INFO_FAMILY: return "Motorola 68K";
                    case CPU_INFO_VERSION: return "2.1";
                    case CPU_INFO_FILE: return "m68000.cs";
                    case CPU_INFO_CREDITS: return "Copyright 1999 Karl Stenerud. All rights reserved. (2.1 fixes HJB)";
                }
                throw new Exception();
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
                return m68k_execute(cycles);
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new m68k_cpu_context();
            }
            public override uint get_pc()
            {
                return m68k_peek_pc();
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
                m68k_pulse_reset(param);            
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                m68k_set_int_ack_callback(callback);
            }
            public override void set_irq_line(int irqline, int state)
            {
                switch (state)
                {
                    case CLEAR_LINE:
                        m68k_clear_irq(irqline);
                        return;
                    case ASSERT_LINE:
                        m68k_assert_irq(irqline);
                        return;
                    default:
                        m68k_assert_irq(irqline);
                        return;
                }
            }
            public override void set_nmi_line(int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_op_base(int pc)
            {
                cpu_setOPbase24(pc,0);
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
            }
            class m68k_cpu_context
            {
                uint mode;         /* CPU Operation Mode (68000, 68010, or 68020) */
                uint sr;           /* Status Register */
                uint ppc;		   /* Previous program counter */
                uint pc;           /* Program Counter */
                uint[] d = new uint[8];         /* Data Registers */
                uint[] a = new uint[8];         /* Address Registers */
                uint usp;          /* User Stack Pointer */
                uint isp;          /* Interrupt Stack Pointer */
                uint msp;          /* Master Stack Pointer */
                uint vbr;          /* Vector Base Register.  Used in 68010+ */
                uint sfc;          /* Source Function Code.  Used in 68010+ */
                uint dfc;          /* Destination Function Code.  Used in 68010+ */
                uint stopped;      /* Stopped state: only interrupt can restart */
                uint halted;       /* Halted state: only reset can restart */
                uint int_state;	   /* Current interrupt line states -- ASG: changed from ints_pending */
                uint int_cycles;   /* Extra cycles taken due to interrupts -- ASG: added */
                uint pref_addr;    /* Last prefetch address */
                uint pref_data;    /* Data in the prefetch queue */
                //int  (*int_ack_callback)(int int_level); /* Interrupt Acknowledge */
                //void (*bkpt_ack_callback)(int data);     /* Breakpoint Acknowledge */
                //void (*reset_instr_callback)(void);      /* Called when a RESET instruction is encountered */
                //void (*pc_changed_callback)(int new_pc); /* Called when the PC changes by a large amount */
                //void (*set_fc_callback)(int new_fc);     /* Called when the CPU function code changes */
                //void (*instr_hook_callback)(void);       /* Called every instruction cycle prior to execution */
            }
            class m68k_cpu_core
            {
                public uint mode; /* CPU Operation Mode: 68000, 68010, or 68020 */
                public uint[] dr = new uint[8]; /* Data Registers */
                public uint[] ar = new uint[8]; /* Address Registers */
                public uint ppc; /* Previous program counter */
                public uint pc; /* Program Counter */
                public uint[] sp = new uint[4]; /* User, Interrupt, and Master Stack Pointers */
                public uint vbr; /* Vector Base Register (68010+) */
                public uint sfc; /* Source Function Code Register (m68010+) */
                public uint dfc; /* Destination Function Code Register (m68010+) */
                public uint cacr; /* Cache Control Register (m68020+) */
                public uint caar; /* Cacge Address Register (m68020+) */
                public uint ir; /* Instruction Register */
                public uint t1_flag; /* Trace 1 */
                public uint t0_flag; /* Trace 0 */
                public uint s_flag; /* Supervisor */
                public uint m_flag; /* Master/Interrupt state */
                public uint x_flag; /* Extend */
                public uint n_flag; /* Negative */
                public uint not_z_flag; /* Zero, inverted for speedups */
                public uint v_flag; /* Overflow */
                public uint c_flag; /* Carry */
                public uint int_mask; /* I0-I2 */
                public uint int_state; /* Current interrupt state -- ASG: changed from ints_pending */
                public uint stopped; /* Stopped state */
                public uint halted; /* Halted state */
                public uint int_cycles; /* ASG: extra cycles from generated interrupts */
                public uint pref_addr; /* Last prefetch address */
                public uint pref_data; /* Data in the prefetch queue */

                /* Callbacks to host */
                public irqcallback int_ack_callback; /* Interrupt Acknowledge */
                public _void_set_int_callback bkpt_ack_callback; /* Breakpoint Acknowledge */
                public opcode reset_instr_callback; /* Called when a RESET instruction is encountered */
                public _void_set_int_callback pc_changed_callback; /* Called when the PC changes by a large amount */
                public _void_set_int_callback set_fc_callback; /* Called when the CPU function code changes */
                public opcode instr_hook_callback; /* Called every instruction cycle prior to execution */

            } ;
            delegate void _void_set_int_callback(int i);
            //delegate int _int_set_int_callback(int i);
            static int[] m68k_clks_left=new int[1];
            static opcode[] m68k_instruction_jump_table = new opcode[0x10000];
        }
    }
}