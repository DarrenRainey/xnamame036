#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        partial class cpu_m68000
        {
            delegate void _void_set_int_callback(int i);
            delegate void opcode();
            class m68k_cpu_context /* CPU Context */
            {
                public uint mode; /* CPU Operation Mode (68000, 68010, or 68020) */
                public uint sr; /* Status Register */
                public uint ppc; /* Previous program counter */
                public uint pc; /* Program Counter */
                public uint[] d = new uint[8]; /* Data Registers */
                public uint[] a = new uint[8]; /* Address Registers */
                public uint usp; /* User Stack Pointer */
                public uint isp; /* Interrupt Stack Pointer */
                public uint msp; /* Master Stack Pointer */
                public uint vbr; /* Vector Base Register.  Used in 68010+ */
                public uint sfc; /* Source Function Code.  Used in 68010+ */
                public uint dfc; /* Destination Function Code.  Used in 68010+ */
                public uint stopped; /* Stopped state: only interrupt can restart */
                public uint halted; /* Halted state: only reset can restart */
                public uint int_state; /* Current interrupt line states -- ASG: changed from ints_pending */
                public uint int_cycles; /* Extra cycles taken due to interrupts -- ASG: added */
                public uint pref_addr; /* Last prefetch address */
                public uint pref_data; /* Data in the prefetch queue */
                public irqcallback int_ack_callback; /* Interrupt Acknowledge */
                public _void_set_int_callback bkpt_ack_callback; /* Breakpoint Acknowledge */
                public opcode reset_instr_callback; /* Called when a RESET instruction is encountered */
                public _void_set_int_callback pc_changed_callback; /* Called when the PC changes by a large amount */
                public _void_set_int_callback set_fc_callback; /* Called when the CPU function code changes */
                public opcode instr_hook_callback; /* Called every instruction cycle prior to execution */
            }
            /* Data types used in this emulation core */
            /* int and uint must be at least 32 bits wide */



            /* Allow for architectures that don't have 8-bit sizes */
            /* Some > 32-bit optimizations */





            /* We have to do this because the morons at ANSI decided that shifts

             * by >= data size are undefined.

             */
            /* Access the CPU registers */
            class m68k_cpu_core
            {
                public uint mode; /* CPU Operation Mode: 68000, 68010, or 68020 */
                public uint[] dr = new uint[8]; /* Data Registers */
                public uint[] ar = new uint[8]; /* Address Registers */
                public uint ppc; /* Previous program counter */
                public uint pc; /* Program Counter */
                public uint[] sp = new uint[4]; /* User, Interrupt, and Master Stack Pointers */
                public uint vbr; /* Vector _base Register (68010+) */
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
                public irqcallback int_ack_callback; /* Interrupt Acknowledge */
                public _void_set_int_callback bkpt_ack_callback; /* Breakpoint Acknowledge */
                public opcode reset_instr_callback; /* Called when a RESET instruction is encountered */
                public _void_set_int_callback pc_changed_callback; /* Called when the PC changes by a large amount */
                public _void_set_int_callback set_fc_callback; /* Called when the CPU function code changes */
            }

            static uint m68ki_read_8(uint address)
            {
                ;
                return (uint)cpu_readmem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_16(uint address)
            {
                ;
                return (uint)cpu_readmem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_32(uint address)
            {
                ;
                return (uint)cpu_readmem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }

            static void m68ki_write_8(uint address, uint value)
            {
                ;
                cpu_writemem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_16(uint address, uint value)
            {
                ;
                cpu_writemem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_32(uint address, uint value)
            {
                ;
                cpu_writemem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }


            static uint m68ki_read_imm_8()
            {

                ;
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return (uint)((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xff);





            }
            static uint m68ki_read_imm_16()
            {

                ;
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return (uint)((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xffff);





            }
            static uint m68ki_read_imm_32()
            {

                uint temp_val;

                ;
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                temp_val = m68k_cpu.pref_data;
                m68k_cpu.pc += 2;
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                    temp_val = ((temp_val << 16) | (m68k_cpu.pref_data >> 16));
                }
                m68k_cpu.pc += 2;

                return temp_val;





            }

            static uint m68ki_read_instruction()
            {

                ;
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return (uint)((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xffff);





            }


            static uint m68ki_read_8_fc(uint address, uint fc)
            {
                ;
                return (uint)cpu_readmem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_16_fc(uint address, uint fc)
            {
                ;
                return (uint)cpu_readmem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_32_fc(uint address, uint fc)
            {
                ;
                return (uint)cpu_readmem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }

            static void m68ki_write_8_fc(uint address, uint fc, uint value)
            {
                ;
                cpu_writemem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_16_fc(uint address, uint fc, uint value)
            {
                ;
                cpu_writemem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_32_fc(uint address, uint fc, uint value)
            {
                ;
                cpu_writemem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }


            static uint m68ki_get_ea_ix()
            {
                uint extension = m68ki_read_imm_16();
                uint ea_index = m68k_cpu_dar[((extension) & 0x00008000) != 0?1:0][(((extension) >> 12) & 7)];
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint outer = 0;

                /* Sign-extend the index value if needed */
                if (((extension) & 0x00000800)==0)
                    ea_index = (uint)(short)((ea_index) & 0xffff);

                /* If we're running 010 or less, there's no scale or full extension word mode */
                if ((m68k_cpu.mode & (1 | 2)) != 0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Scale the index value */
                ea_index <<=(int) (((extension) >> 9) & 3);

                /* If we're using brief extension mode, we are done */
                if (((extension) & 0x00000100)==0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Decode the long extension format */
                if (((extension) & 0x00000040) != 0)
                    ea_index = 0;
                if (((extension) & 0x00000080) != 0)
                    _base = 0;
                if (((extension) & 0x00000020) != 0)
                    _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 7)==0)
                    return _base + ea_index;

                if (((extension) & 0x00000002)!=0)
                    outer = ((extension) & 0x00000001) != 0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 0x00000004)!=0)
                    return m68ki_read_32(_base) + ea_index + outer;
                return m68ki_read_32(_base + ea_index) + outer;
            }

            static uint m68ki_get_ea_ix_dst()
            {
                uint extension = m68ki_read_imm_16();
                uint ea_index = m68k_cpu_dar[((extension) & 0x00008000) != 0?1:0][(((extension) >> 12) & 7)];
                uint _base = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]); /* This is the only thing different from m68ki_get_ea_ix() */
                uint outer = 0;

                /* Sign-extend the index value if needed */
                if (((extension) & 0x00000800)==0)
                    ea_index = (uint)(short)((ea_index) & 0xffff);

                /* If we're running 010 or less, there's no scale or full extension word mode */
                if ((m68k_cpu.mode & (1 | 2))!=0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Scale the index value */
                ea_index <<= (int)(((extension) >> 9) & 3);

                /* If we're using brief extension mode, we are done */
                if (((extension) & 0x00000100)==0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Decode the long extension format */
                if (((extension) & 0x00000040)!=0)
                    ea_index = 0;
                if (((extension) & 0x00000080)!=0)
                    _base = 0;
                if (((extension) & 0x00000020)!=0)
                    _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 7)==0)
                    return _base + ea_index;

                if (((extension) & 0x00000002)!=0)
                    outer = ((extension) & 0x00000001) != 0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 0x00000004)!=0)
                    return m68ki_read_32(_base) + ea_index + outer;
                return m68ki_read_32(_base + ea_index) + outer;
            }

            /* Decode program counter indirect with index */
            static uint m68ki_get_ea_pcix()
            {
                uint _base = (m68k_cpu.pc += 2) - 2;
                uint extension = m68ki_read_16(_base);
                uint ea_index = m68k_cpu_dar[((extension) & 0x00008000) != 0?1:0][(((extension) >> 12) & 7)];
                uint outer = 0;

                /* Sign-extend the index value if needed */
                if (((extension) & 0x00000800)==0)
                    ea_index = (uint)(short)((ea_index) & 0xffff);

                /* If we're running 010 or less, there's no scale or full extension word mode */
                if ((m68k_cpu.mode & (1 | 2))!=0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Scale the index value */
                ea_index <<= (int)(((extension) >> 9) & 3);

                /* If we're using brief extension mode, we are done */
                if (((extension) & 0x00000100)==0)
                    return (uint)(_base + ea_index + (sbyte)((extension) & 0xff));

                /* Decode the long extension format */
                if (((extension) & 0x00000040)!=0)
                    ea_index = 0;
                if (((extension) & 0x00000080)!=0)
                    _base = 0;
                if (((extension) & 0x00000020)!=0)
                    _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 7)==0)
                    return _base + ea_index;

                if (((extension) & 0x00000002)!=0)
                    outer = ((extension) & 0x00000001)!=0 ? m68ki_read_imm_32() : (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                if (((extension) & 0x00000004)!=0)
                    return m68ki_read_32(_base) + ea_index + outer;
                return m68ki_read_32(_base + ea_index) + outer;
            }


            /* Set the S flag and change the active stack pointer. */
            static void m68ki_set_s_flag(int value)
            {
                /* ASG: Only do the rest if we're changing */
                value = (value != 0)?1:0;
                if (m68k_cpu.s_flag != value)
                {
                    /* Backup the old stack pointer */
                    m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))] = m68k_cpu.ar[7];
                    /* Set the S flag */
                    m68k_cpu.s_flag = (uint)value;
                    /* Set the new stack pointer */
                    m68k_cpu.ar[7] = m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))];
                }
            }

            /* Set the M flag and change the active stack pointer. */
            static void m68ki_set_m_flag(int value)
            {
                /* ASG: Only do the rest if we're changing */
                value = (value != 0 && (m68k_cpu.mode & (4 | 8))!=0)?1:0 << 1;
                if (m68k_cpu.m_flag != value)
                {
                    /* Backup the old stack pointer */
                    m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))] = m68k_cpu.ar[7];
                    /* Set the M flag */
                    m68k_cpu.m_flag = (uint)value;
                    /* Set the new stack pointer */
                    m68k_cpu.ar[7] = m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))];
                }
            }

            /* Set the S and M flags and change the active stack pointer. */
            static void m68ki_set_sm_flag(int s_value, int m_value)
            {
                /* ASG: Only do the rest if we're changing */
                s_value = (s_value != 0)?1:0;
                m_value = (m_value != 0 && (m68k_cpu.mode & (4 | 8))!=0)?1:0 << 1;
                if (m68k_cpu.s_flag != s_value || m68k_cpu.m_flag != m_value)
                {
                    /* Backup the old stack pointer */
                    m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))] = m68k_cpu.ar[7];
                    /* Set the S and M flags */
                    m68k_cpu.s_flag = s_value != 0?1u:0u;
                    m68k_cpu.m_flag = (m_value != 0 && (m68k_cpu.mode & (4 | 8))!=0)?1u:0u << 1;
                    /* Set the new stack pointer */
                    m68k_cpu.ar[7] = m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))];
                }
            }


            /* Set the condition code register */
            static void m68ki_set_ccr(uint value)
            {
                m68k_cpu.x_flag = ((value) & 0x00000010);
                m68k_cpu.n_flag = ((value) & 0x00000008);
                m68k_cpu.not_z_flag = ((value) & 0x00000004)==0?1u:0u;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
            }

            /* Set the status register */
            static void m68ki_set_sr(uint value)
            {
                /* ASG: detect changes to the INT_MASK */
                int old_mask = (int)m68k_cpu.int_mask;

                /* Mask out the "unimplemented" bits */
                value &= m68k_sr_implemented_bits[m68k_cpu.mode];

                /* Now set the status register */
                m68k_cpu.t1_flag = ((value) & 0x00008000);
                m68k_cpu.t0_flag = ((value) & 0x00004000);
                m68k_cpu.int_mask = (value >> 8) & 7;
                m68k_cpu.x_flag = ((value) & 0x00000010);
                m68k_cpu.n_flag = ((value) & 0x00000008);
                m68k_cpu.not_z_flag = ((value) & 0x00000004)==0?1u:0u;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
                m68ki_set_sm_flag((int)((value) & 0x00002000), (int)((value) & 0x00001000));

                /* ASG: detect changes to the INT_MASK */
                if (m68k_cpu.int_mask != old_mask)
                    m68ki_check_interrupts();
            }


            /* Set the status register */
            static void m68ki_set_sr_no_int(uint value)
            {
                /* Mask out the "unimplemented" bits */
                value &= m68k_sr_implemented_bits[m68k_cpu.mode];

                /* Now set the status register */
                m68k_cpu.t1_flag = ((value) & 0x00008000);
                m68k_cpu.t0_flag = ((value) & 0x00004000);
                m68k_cpu.int_mask = (value >> 8) & 7;
                m68k_cpu.x_flag = ((value) & 0x00000010);
                m68k_cpu.n_flag = ((value) & 0x00000008);
                m68k_cpu.not_z_flag = ((value) & 0x00000004)==0?1u:0u;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
                m68ki_set_sm_flag((int)((value) & 0x00002000), (int)((value) & 0x00001000));
            }


            /* I set the PC this way to let host programs be nicer.

             * This is mainly for programs running from separate ram banks.

             * If the host program knows where the PC is, it can offer faster

             * ram access times for data to be retrieved immediately following

             * the PC.

             */
            static void m68ki_set_pc(uint address)
            {
                /* Set the program counter */
                m68k_cpu.pc = address;
                /* Inform the host program */
                /* MAME */
                change_pc24(((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
                /*

                   m68ki_pc_changed(ADDRESS_68K(address));

                */
            }


            /* Process an exception */
            static void m68ki_exception(uint vector)
            {
                /* Save the old status register */
                uint old_sr = (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0));

                /* Use up some clock cycles */
                m68k_clks_left[0] -= (m68k_exception_cycle_table[vector]);

                /* Turn off stopped state and trace flag, clear pending traces */
                m68k_cpu.stopped = 0;
                m68k_cpu.t1_flag = m68k_cpu.t0_flag = 0;
                ;
                /* Enter supervisor mode */
                m68ki_set_s_flag(1);
                /* Push a stack frame */
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                    m68ki_write_16(m68k_cpu.ar[7] -= 2, vector << 2); /* This is format 0 */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.ppc); /* save previous PC, ie. PC that contains an offending instruction */
                m68ki_write_16(m68k_cpu.ar[7] -= 2, old_sr);
                /* Generate a new program counter from the vector */
                m68ki_set_pc(m68ki_read_32((vector << 2) + m68k_cpu.vbr));
            }


            /* Process an interrupt (or trap) */
            static void m68ki_interrupt(uint vector)
            {
                /* Save the old status register */
                uint old_sr = (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0));

                /* Use up some clock cycles */
                /* ASG: just keep them pending */
                /* USE_CLKS(m68k_exception_cycle_table[vector]);*/
                m68k_cpu.int_cycles += m68k_exception_cycle_table[vector];

                /* Turn off stopped state and trace flag, clear pending traces */
                m68k_cpu.stopped = 0;
                m68k_cpu.t1_flag = m68k_cpu.t0_flag = 0;
                ;
                /* Enter supervisor mode */
                m68ki_set_s_flag(1);
                /* Push a stack frame */
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                    m68ki_write_16(m68k_cpu.ar[7] -= 2, vector << 2); /* This is format 0 */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_write_16(m68k_cpu.ar[7] -= 2, old_sr);
                /* Generate a new program counter from the vector */
                m68ki_set_pc(m68ki_read_32((vector << 2) + m68k_cpu.vbr));
            }


            /* Service an interrupt request */
            static void m68ki_service_interrupt(uint pending_mask) /* ASG: added parameter here */
            {
                uint int_level = 7;
                uint vector;

                /* Start at level 7 and then go down */
                for (; !(pending_mask & (1 << int_level)); int_level--) /* ASG: changed to use parameter instead of CPU_INTS_PENDING */
                    ;

                /* Get the exception vector */
                switch (vector = m68k_cpu.int_ack_callback(int_level))
                {
                    case 0x00:
                    case 0x01:
                        /* vectors 0 and 1 are ignored since they are for reset only */
                        return;
                    case 0x02:
                    case 0x03:
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                    case 0x08:
                    case 0x09:
                    case 0x0a:
                    case 0x0b:
                    case 0x0c:
                    case 0x0d:
                    case 0x0e:
                    case 0x0f:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 0x28:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                    case 0x30:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                    case 0x38:
                    case 0x39:
                    case 0x3a:
                    case 0x3b:
                    case 0x3c:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                    case 0x41:
                    case 0x42:
                    case 0x43:
                    case 0x44:
                    case 0x45:
                    case 0x46:
                    case 0x47:
                    case 0x48:
                    case 0x49:
                    case 0x4a:
                    case 0x4b:
                    case 0x4c:
                    case 0x4d:
                    case 0x4e:
                    case 0x4f:
                    case 0x50:
                    case 0x51:
                    case 0x52:
                    case 0x53:
                    case 0x54:
                    case 0x55:
                    case 0x56:
                    case 0x57:
                    case 0x58:
                    case 0x59:
                    case 0x5a:
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                    case 0x60:
                    case 0x61:
                    case 0x62:
                    case 0x63:
                    case 0x64:
                    case 0x65:
                    case 0x66:
                    case 0x67:
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 0x6e:
                    case 0x6f:
                    case 0x70:
                    case 0x71:
                    case 0x72:
                    case 0x73:
                    case 0x74:
                    case 0x75:
                    case 0x76:
                    case 0x77:
                    case 0x78:
                    case 0x79:
                    case 0x7a:
                    case 0x7b:
                    case 0x7c:
                    case 0x7d:
                    case 0x7e:
                    case 0x7f:
                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0x87:
                    case 0x88:
                    case 0x89:
                    case 0x8a:
                    case 0x8b:
                    case 0x8c:
                    case 0x8d:
                    case 0x8e:
                    case 0x8f:
                    case 0x90:
                    case 0x91:
                    case 0x92:
                    case 0x93:
                    case 0x94:
                    case 0x95:
                    case 0x96:
                    case 0x97:
                    case 0x98:
                    case 0x99:
                    case 0x9a:
                    case 0x9b:
                    case 0x9c:
                    case 0x9d:
                    case 0x9e:
                    case 0x9f:
                    case 0xa0:
                    case 0xa1:
                    case 0xa2:
                    case 0xa3:
                    case 0xa4:
                    case 0xa5:
                    case 0xa6:
                    case 0xa7:
                    case 0xa8:
                    case 0xa9:
                    case 0xaa:
                    case 0xab:
                    case 0xac:
                    case 0xad:
                    case 0xae:
                    case 0xaf:
                    case 0xb0:
                    case 0xb1:
                    case 0xb2:
                    case 0xb3:
                    case 0xb4:
                    case 0xb5:
                    case 0xb6:
                    case 0xb7:
                    case 0xb8:
                    case 0xb9:
                    case 0xba:
                    case 0xbb:
                    case 0xbc:
                    case 0xbd:
                    case 0xbe:
                    case 0xbf:
                    case 0xc0:
                    case 0xc1:
                    case 0xc2:
                    case 0xc3:
                    case 0xc4:
                    case 0xc5:
                    case 0xc6:
                    case 0xc7:
                    case 0xc8:
                    case 0xc9:
                    case 0xca:
                    case 0xcb:
                    case 0xcc:
                    case 0xcd:
                    case 0xce:
                    case 0xcf:
                    case 0xd0:
                    case 0xd1:
                    case 0xd2:
                    case 0xd3:
                    case 0xd4:
                    case 0xd5:
                    case 0xd6:
                    case 0xd7:
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                    case 0xdc:
                    case 0xdd:
                    case 0xde:
                    case 0xdf:
                    case 0xe0:
                    case 0xe1:
                    case 0xe2:
                    case 0xe3:
                    case 0xe4:
                    case 0xe5:
                    case 0xe6:
                    case 0xe7:
                    case 0xe8:
                    case 0xe9:
                    case 0xea:
                    case 0xeb:
                    case 0xec:
                    case 0xed:
                    case 0xee:
                    case 0xef:
                    case 0xf0:
                    case 0xf1:
                    case 0xf2:
                    case 0xf3:
                    case 0xf4:
                    case 0xf5:
                    case 0xf6:
                    case 0xf7:
                    case 0xf8:
                    case 0xf9:
                    case 0xfa:
                    case 0xfb:
                    case 0xfc:
                    case 0xfd:
                    case 0xfe:
                    case 0xff:
                        /* The external peripheral has provided the interrupt vector to take */
                        break;
                    case -1:
                        /* Use the autovectors.  This is the most commonly used implementation */
                        vector = 24 + int_level;
                        break;
                    case -2:
                        /* Called if no devices respond to the interrupt acknowledge */
                        vector = 24;
                        break;
                    default:
                        /* Everything else is ignored */
                        return;
                }

                /* If vector is uninitialized, call the uninitialized interrupt vector */
                if (m68ki_read_32(vector << 2) == 0)
                    vector = 15;

                /* Generate an interupt */
                m68ki_interrupt(vector);

                /* Set the interrupt mask to the level of the one being serviced */
                m68k_cpu.int_mask = int_level;
            }


            /* ASG: Check for interrupts */
            static void m68ki_check_interrupts()
            {
                uint pending_mask = 1u <<(int) m68k_cpu.int_state;
                if ((pending_mask & m68k_int_masks[m68k_cpu.int_mask])!=0)
                    m68ki_service_interrupt(pending_mask);
            }
            

            static uint m68k_emulation_initialized = 0; /* flag if emulation has been initialized */
            static opcode[] m68k_instruction_jump_table = new opcode[0x10000]; /* opcode handler jump table */
            //static int[] m68k_clks_left = new int[1]; /* Number of clocks remaining */
            uint m68k_tracing = 0;
            /* Mask which bits of the SR ar eimplemented */
            static uint[] m68k_sr_implemented_bits =
{
   0x0000, /* invalid */
   0xa71f, /* 68000:   T1 -- S  -- -- I2 I1 I0 -- -- -- X  N  Z  V  C  */
   0xa71f, /* 68010:   T1 -- S  -- -- I2 I1 I0 -- -- -- X  N  Z  V  C  */
   0x0000, /* invalid */
   0xf71f, /* 68EC020: T1 T0 S  M  -- I2 I1 I0 -- -- -- X  N  Z  V  C  */
   0x0000, /* invalid */
   0x0000, /* invalid */
   0x0000, /* invalid */
   0xf71f, /* 68020:   T1 T0 S  M  -- I2 I1 I0 -- -- -- X  N  Z  V  C  */
};

            /* The CPU core */
            static m68k_cpu_core m68k_cpu = new m68k_cpu_core();

            /* Pointers to speed up address register indirect with index calculation */
            static UIntSubArray[] m68k_cpu_dar = { new UIntSubArray(m68k_cpu.dr), new UIntSubArray(m68k_cpu.ar) };

            /* Pointers to speed up movem instructions */
            static UIntSubArray[] m68k_movem_pi_table =
{
  new UIntSubArray( m68k_cpu.dr), new UIntSubArray(m68k_cpu.dr,1),new UIntSubArray(m68k_cpu.dr,2), new UIntSubArray(m68k_cpu.dr,3), new UIntSubArray(m68k_cpu.dr,4), new UIntSubArray(m68k_cpu.dr,5), new UIntSubArray(m68k_cpu.dr,6), new UIntSubArray(m68k_cpu.dr,7),
  new UIntSubArray( m68k_cpu.ar), new UIntSubArray(m68k_cpu.ar,1),new UIntSubArray(m68k_cpu.ar,2), new UIntSubArray(m68k_cpu.ar,3), new UIntSubArray(m68k_cpu.ar,4), new UIntSubArray(m68k_cpu.ar,5), new UIntSubArray(m68k_cpu.ar,6), new UIntSubArray(m68k_cpu.ar,7)
};
            static UIntSubArray[] m68k_movem_pd_table =
{
  new UIntSubArray(m68k_cpu.ar,7),new UIntSubArray(m68k_cpu.ar,6), new UIntSubArray(m68k_cpu.ar,5),new UIntSubArray(m68k_cpu.ar,4), new UIntSubArray(m68k_cpu.ar,3),new UIntSubArray(m68k_cpu.ar,2), new UIntSubArray(m68k_cpu.ar,1), new UIntSubArray(m68k_cpu.ar),
  new UIntSubArray(m68k_cpu.dr,7),new UIntSubArray(m68k_cpu.dr,6), new UIntSubArray(m68k_cpu.dr,5),new UIntSubArray(m68k_cpu.dr,4), new UIntSubArray(m68k_cpu.dr,3),new UIntSubArray(m68k_cpu.dr,2), new UIntSubArray(m68k_cpu.dr,1), new UIntSubArray(m68k_cpu.dr),
};


            /* Used when checking for pending interrupts */
            static byte[] m68k_int_masks = { 0xfe, 0xfc, 0xf8, 0xf0, 0xe0, 0xc0, 0x80, 0x80 };

            /* Used by shift & rotate instructions */
           static  byte[] m68k_shift_8_table =
{
    0x00, 0x80, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc, 0xfe, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
    0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
    0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
    0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
};
            static ushort[] m68k_shift_16_table =
{
    0x0000, 0x8000, 0xc000, 0xe000, 0xf000, 0xf800, 0xfc00, 0xfe00, 0xff00, 0xff80, 0xffc0, 0xffe0,
    0xfff0, 0xfff8, 0xfffc, 0xfffe, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff,
    0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff,
    0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff,
    0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff, 0xffff,
    0xffff, 0xffff, 0xffff, 0xffff, 0xffff
};
            static uint[] m68k_shift_32_table =
{
    0x00000000, 0x80000000, 0xc0000000, 0xe0000000, 0xf0000000, 0xf8000000, 0xfc000000, 0xfe000000,
    0xff000000, 0xff800000, 0xffc00000, 0xffe00000, 0xfff00000, 0xfff80000, 0xfffc0000, 0xfffe0000,
    0xffff0000, 0xffff8000, 0xffffc000, 0xffffe000, 0xfffff000, 0xfffff800, 0xfffffc00, 0xfffffe00,
    0xffffff00, 0xffffff80, 0xffffffc0, 0xffffffe0, 0xfffffff0, 0xfffffff8, 0xfffffffc, 0xfffffffe,
    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff,
    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff,
    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff,
    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff
};


            /* Number of clock cycles to use for exception processing.

             * I used 4 for any vectors that are undocumented for processing times.

             */
            static byte[] m68k_exception_cycle_table =
{
   40, /*  0: Reset - should never be called                                 */
   40, /*  1: Reset - should never be called                                 */
   50, /*  2: Bus Error                                (unused in emulation) */
   50, /*  3: Address Error                            (unused in emulation) */
   34, /*  4: Illegal Instruction                                            */
   38, /*  5: Divide by Zero -- ASG: changed from 42                         */
   40, /*  6: CHK -- ASG: chanaged from 44                                   */
   34, /*  7: TRAPV                                                          */
   34, /*  8: Privilege Violation                                            */
   34, /*  9: Trace                                                          */
    4, /* 10: 1010                                                           */
    4, /* 11: 1111                                                           */
    4, /* 12: RESERVED                                                       */
    4, /* 13: Coprocessor Protocol Violation           (unused in emulation) */
    4, /* 14: Format Error                             (unused in emulation) */
   44, /* 15: Uninitialized Interrupt                                        */
    4, /* 16: RESERVED                                                       */
    4, /* 17: RESERVED                                                       */
    4, /* 18: RESERVED                                                       */
    4, /* 19: RESERVED                                                       */
    4, /* 20: RESERVED                                                       */
    4, /* 21: RESERVED                                                       */
    4, /* 22: RESERVED                                                       */
    4, /* 23: RESERVED                                                       */
   44, /* 24: Spurious Interrupt                                             */
   44, /* 25: Level 1 Interrupt Autovector                                   */
   44, /* 26: Level 2 Interrupt Autovector                                   */
   44, /* 27: Level 3 Interrupt Autovector                                   */
   44, /* 28: Level 4 Interrupt Autovector                                   */
   44, /* 29: Level 5 Interrupt Autovector                                   */
   44, /* 30: Level 6 Interrupt Autovector                                   */
   44, /* 31: Level 7 Interrupt Autovector                                   */
   34, /* 32: TRAP #0 -- ASG: chanaged from 38                               */
   34, /* 33: TRAP #1                                                        */
   34, /* 34: TRAP #2                                                        */
   34, /* 35: TRAP #3                                                        */
   34, /* 36: TRAP #4                                                        */
   34, /* 37: TRAP #5                                                        */
   34, /* 38: TRAP #6                                                        */
   34, /* 39: TRAP #7                                                        */
   34, /* 40: TRAP #8                                                        */
   34, /* 41: TRAP #9                                                        */
   34, /* 42: TRAP #10                                                       */
   34, /* 43: TRAP #11                                                       */
   34, /* 44: TRAP #12                                                       */
   34, /* 45: TRAP #13                                                       */
   34, /* 46: TRAP #14                                                       */
   34, /* 47: TRAP #15                                                       */
    4, /* 48: FP Branch or Set on Unknown Condition    (unused in emulation) */
    4, /* 49: FP Inexact Result                        (unused in emulation) */
    4, /* 50: FP Divide by Zero                        (unused in emulation) */
    4, /* 51: FP Underflow                             (unused in emulation) */
    4, /* 52: FP Operand Error                         (unused in emulation) */
    4, /* 53: FP Overflow                              (unused in emulation) */
    4, /* 54: FP Signaling NAN                         (unused in emulation) */
    4, /* 55: FP Unimplemented Data Type               (unused in emulation) */
    4, /* 56: MMU Configuration Error                  (unused in emulation) */
    4, /* 57: MMU Illegal Operation Error              (unused in emulation) */
    4, /* 58: MMU Access Level Violation Error         (unused in emulation) */
    4, /* 59: RESERVED                                                       */
    4, /* 60: RESERVED                                                       */
    4, /* 61: RESERVED                                                       */
    4, /* 62: RESERVED                                                       */
    4, /* 63: RESERVED                                                       */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /*  64- 79: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /*  80- 95: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /*  96-111: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 112-127: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 128-143: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 144-159: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 160-175: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 176-191: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 192-207: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 208-223: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 224-239: User Defined */
    4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, /* 240-255: User Defined */
};

            /* Interrupt acknowledge */
            static int default_int_ack_callback_data;
            static int default_int_ack_callback(int int_level)
            {
                default_int_ack_callback_data = int_level;
                return -1;
            }

            /* Breakpoint acknowledge */
            static int default_bkpt_ack_callback_data;
            static void default_bkpt_ack_callback(int data)
            {
                default_bkpt_ack_callback_data = data;
            }

            /* Called when a reset instruction is executed */
            static void default_reset_instr_callback()
            {
            }

            /* Called when the program counter changed by a large value */
            static int default_pc_changed_callback_data;
            static void default_pc_changed_callback(int new_pc)
            {
                default_pc_changed_callback_data = new_pc;
            }

            /* Called every time there's bus activity (read/write to/from memory */
            static int default_set_fc_callback_data;
            static void default_set_fc_callback(int new_fc)
            {
                default_set_fc_callback_data = new_fc;
            }

            /* Called every instruction cycle prior to execution */
            static void default_instr_hook_callback()
            {
            }

            /* Peek at the internals of the M68K */
            int m68k_peek_dr(int reg_num) { return (int)( (reg_num < 8) ? m68k_cpu.dr[reg_num] : 0); }
            int m68k_peek_ar(int reg_num) { return (int)((reg_num < 8) ? m68k_cpu.ar[reg_num] : 0); }
            uint m68k_peek_pc() { return ((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pc : (m68k_cpu.pc) & 0xffffff); }
            uint m68k_peek_ppc() { return ((m68k_cpu.mode & 8) != 0 ? m68k_cpu.ppc : (m68k_cpu.ppc) & 0xffffff); }
            int m68k_peek_sr() { return (int) (((m68k_cpu.t1_flag != 0) ? 1 : 0 << 15) | ((m68k_cpu.t0_flag != 0) ? 1 : 0 << 14) | ((m68k_cpu.s_flag != 0) ? 1 : 0 << 13) | ((m68k_cpu.m_flag != 0) ? 1 : 0 << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 4) | ((m68k_cpu.n_flag != 0) ? 1 : 0 << 3) | ((m68k_cpu.not_z_flag == 0) ? 1 : 0 << 2) | ((m68k_cpu.v_flag != 0) ? 1 : 0 << 1) | ((m68k_cpu.c_flag != 0) ? 1 : 0)); }
            int m68k_peek_ir() { return (int)m68k_cpu.ir; }
            int m68k_peek_t1_flag() { return m68k_cpu.t1_flag != 0?1:0; }
            int m68k_peek_t0_flag() { return m68k_cpu.t0_flag != 0 ? 1 : 0; }
            int m68k_peek_s_flag() { return m68k_cpu.s_flag != 0 ? 1 : 0; }
            int m68k_peek_m_flag() { return m68k_cpu.m_flag != 0 ? 1 : 0; }
            int m68k_peek_int_mask() { return (int) m68k_cpu.int_mask; }
            int m68k_peek_x_flag() { return m68k_cpu.x_flag != 0 ? 1 : 0; }
            int m68k_peek_n_flag() { return m68k_cpu.n_flag != 0 ? 1 : 0; }
            int m68k_peek_z_flag() { return m68k_cpu.not_z_flag == 0 ? 1 : 0; }
            int m68k_peek_v_flag() { return m68k_cpu.v_flag != 0 ? 1 : 0; }
            int m68k_peek_c_flag() { return m68k_cpu.c_flag != 0 ? 1 : 0; }
            int m68k_peek_usp() { return m68k_cpu.s_flag != 0 ? (int)m68k_cpu.sp[0] : (int)m68k_cpu.ar[7]; }
            int m68k_peek_isp() { return m68k_cpu.s_flag != 0 && m68k_cpu.m_flag == 0 ? (int)m68k_cpu.ar[7] : (int)m68k_cpu.sp[1]; }
            int m68k_peek_msp() { return m68k_cpu.s_flag != 0 && m68k_cpu.m_flag == 0 ? (int)m68k_cpu.ar[7] : (int)m68k_cpu.sp[3]; }

            /* Poke data into the M68K */
            void m68k_poke_dr(int reg_num, int value) { if (reg_num < 8) m68k_cpu.dr[reg_num] = (uint)(value); }
            void m68k_poke_ar(int reg_num, int value) { if (reg_num < 8) m68k_cpu.ar[reg_num] = (uint)(value); }
            void m68k_poke_pc(uint value) { m68ki_set_pc(((m68k_cpu.mode & 8) != 0 ? value : (value) & 0xffffff)); }
            void m68k_poke_sr(int value) { m68ki_set_sr((uint)((value) & 0xffff)); }
            void m68k_poke_ir(int value) { m68k_cpu.ir = ((uint)(value) & 0xffff); }
            void m68k_poke_t1_flag(int value) { m68k_cpu.t1_flag = (value != 0)?1u:0u; }
            void m68k_poke_t0_flag(int value) { if ((m68k_cpu.mode & (4 | 8))!=0) m68k_cpu.t0_flag = (value != 0)?1u:0u; }
            void m68k_poke_s_flag(int value) { m68ki_set_s_flag(value); }
            void m68k_poke_m_flag(int value) { if ((m68k_cpu.mode & (4 | 8))!=0) m68ki_set_m_flag(value); }
            void m68k_poke_int_mask(int value) { m68k_cpu.int_mask =(uint)( value & 7); }
            void m68k_poke_x_flag(int value) { m68k_cpu.x_flag = (value != 0) ? 1u : 0u; }
            void m68k_poke_n_flag(int value) { m68k_cpu.n_flag = (value != 0) ? 1u : 0u; }
            void m68k_poke_z_flag(int value) { m68k_cpu.not_z_flag = (value == 0) ? 1u : 0u; }
            void m68k_poke_v_flag(int value) { m68k_cpu.v_flag = (value != 0) ? 1u : 0u; }
            void m68k_poke_c_flag(int value) { m68k_cpu.c_flag = (value != 0) ? 1u : 0u; }
            void m68k_poke_usp(int value)
            {
                if (m68k_cpu.s_flag!=0)
                    m68k_cpu.sp[0] = (uint)(value);
                else
                    m68k_cpu.ar[7] = (uint)(value);
            }
            void m68k_poke_isp(int value)
            {
                if (m68k_cpu.s_flag!=0 && m68k_cpu.m_flag==0)
                    m68k_cpu.ar[7] = (uint)(value);
                else
                    m68k_cpu.sp[1] = (uint)(value);
            }
            void m68k_poke_msp(int value)
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (m68k_cpu.s_flag!=0 && m68k_cpu.m_flag!=0)
                        m68k_cpu.ar[7] = (uint)(value);
                    else
                        m68k_cpu.sp[3] = (uint)(value);
                }
            }

            /* Set the callbacks */
            void m68k_set_int_ack_callback(irqcallback callback)
            {
                m68k_cpu.int_ack_callback = callback != null ? callback : default_int_ack_callback;
            }

            void m68k_set_bkpt_ack_callback(_void_set_int_callback callback)
            {
                m68k_cpu.bkpt_ack_callback = callback !=null? callback : default_bkpt_ack_callback;
            }

            void m68k_set_reset_instr_callback(opcode callback)
            {
                m68k_cpu.reset_instr_callback = callback != null ? callback : default_reset_instr_callback;
            }

            void m68k_set_pc_changed_callback(_void_set_int_callback callback)
            {
                m68k_cpu.pc_changed_callback = callback != null ? callback : default_pc_changed_callback;
            }

            void m68k_set_fc_callback(_void_set_int_callback callback)
            {
                m68k_cpu.set_fc_callback = callback != null ? callback : default_set_fc_callback;
            }

            void m68k_set_instr_hook_callback(opcode callback)
            {
                m68k_cpu.instr_hook_callback = callback != null ? callback : default_instr_hook_callback;
            }


            void m68k_set_cpu_mode(int cpu_mode)
            {
                switch (cpu_mode)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        m68k_cpu.mode = cpu_mode;
                        return;
                    default:
                        m68k_cpu.mode = 1;
                }
            }


            /* Execute some instructions until we use up num_clks clock cycles */
            /* ASG: removed per-instruction interrupt checks */
            int m68k_execute(int num_clks)
            {




                /* Make sure we're not stopped */
                if (!m68k_cpu.stopped)
                {
                    /* Set our pool of clock cycles available */
                    m68k_clks_left[0] = num_clks;

                    /* ASG: update cycles */
                    m68k_clks_left[0] -= m68k_cpu.int_cycles;
                    m68k_cpu.int_cycles = 0;

                    /* Main loop.  Keep going until we run out of clock cycles */
                    do
                    {
                        /* Set tracing accodring to T1. (T0 is done inside instruction) */
                        ; /* auto-disable (see m68kcpu.h) */

                        /* Call external hook to peek at CPU */
                        ; /* auto-disable (see m68kcpu.h) */

                        /* MAME */
                        m68k_cpu.ppc = m68k_cpu.pc;




                        /* MAME */

                        /* Read an instruction and call its handler */
                        m68k_cpu.ir = m68ki_read_instruction();
                        m68k_instruction_jump_table[m68k_cpu.ir]();

                        /* Trace m68k_exception, if necessary */
                        ; /* auto-disable (see m68kcpu.h) */
                        continue;
                    } while (m68k_clks_left[0] > 0);

                    /* set previous PC to current PC for the next entry into the loop */
                    m68k_cpu.ppc = m68k_cpu.pc;

                    /* ASG: update cycles */
                    m68k_clks_left[0] -= m68k_cpu.int_cycles;
                    m68k_cpu.int_cycles = 0;





                    /* return how many clocks we used */
                    return num_clks - m68k_clks_left[0];
                }




                /* We get here if the CPU is stopped */
                m68k_clks_left[0] = 0;





                return num_clks;
            }


            /* ASG: rewrote so that the int_line is a mask of the IPL0/IPL1/IPL2 bits */
            void m68k_assert_irq(int int_line)
            {
                /* OR in the bits of the interrupt */
                int old_state = m68k_cpu.int_state;
                m68k_cpu.int_state = 0; /* ASG: remove me to do proper mask setting */
                m68k_cpu.int_state |= int_line & 7;

                /* if it's NMI, we're edge triggered */
                if (m68k_cpu.int_state == 7)
                {
                    if (old_state != 7)
                        m68ki_service_interrupt(1 << 7);
                }

                /* other interrupts just reflect the current state */
                else
                    m68ki_check_interrupts();
            }

            /* ASG: rewrote so that the int_line is a mask of the IPL0/IPL1/IPL2 bits */
            void m68k_clear_irq(int int_line)
            {
                /* AND in the bits of the interrupt */
                m68k_cpu.int_state &= ~int_line & 7;
                m68k_cpu.int_state = 0; /* ASG: remove me to do proper mask setting */

                /* check for interrupts again */
                m68ki_check_interrupts();
            }


            /* Reset the M68K */
            void m68k_pulse_reset(object param)
            {
                m68k_cpu.halted = 0;
                m68k_cpu.stopped = 0;
                m68k_cpu.int_state = 0; /* ASG: changed from CPU_INTS_PENDING */
                m68k_cpu.t1_flag = m68k_cpu.t0_flag = 0;
                ;
                m68k_cpu.s_flag = 1;
                m68k_cpu.m_flag = 0;
                m68k_cpu.int_mask = 7;
                m68k_cpu.vbr = 0;
                m68k_cpu.ar[7] = m68ki_read_32(0);
                m68ki_set_pc(m68ki_read_32(4));

                m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));

                m68k_clks_left[0] = 0;

                if (m68k_cpu.mode == 0) m68k_cpu.mode = 1; /* KW 990319 */
                /* The first call to this function initializes the opcode handler jump table */
                if (m68k_emulation_initialized!=0)
                    return;
                else
                {
                    m68ki_build_opcode_table();
                    m68k_set_int_ack_callback(null);
                    m68k_set_bkpt_ack_callback(null);
                    m68k_set_reset_instr_callback(null);
                    m68k_set_pc_changed_callback(null);
                    m68k_set_fc_callback(null);
                    m68k_set_instr_hook_callback(null);

                    m68k_emulation_initialized = 1;
                }
            }


            /* Halt the CPU */
            void m68k_pulse_halt()
            {
                m68k_cpu.halted = 1;
            }


            /* Get and set the current CPU context */
            /* This is to allow for multiple CPUs */
            uint m68k_get_context( object dst)
            {
                if (dst!=null)
                {
                    m68k_cpu_context cpu = (m68k_cpu_context)dst;

                    cpu.mode = m68k_cpu.mode;
                    cpu.sr = (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u));
                    cpu.pc = m68k_cpu.pc;
                    memcpy(cpu.d, m68k_cpu.dr, sizeof(m68k_cpu.dr));
                    memcpy(cpu.a, m68k_cpu.ar, sizeof(m68k_cpu.ar));
                    cpu.usp = m68k_cpu.sp[0];
                    cpu.isp = m68k_cpu.sp[1];
                    cpu.msp = m68k_cpu.sp[3];
                    cpu.vbr = m68k_cpu.vbr;
                    cpu.sfc = m68k_cpu.sfc;
                    cpu.dfc = m68k_cpu.dfc;
                    cpu.stopped = m68k_cpu.stopped;
                    cpu.halted = m68k_cpu.halted;
                    cpu.int_state = m68k_cpu.int_state; /* ASG: changed from CPU_INTS_PENDING */
                    cpu.int_cycles = m68k_cpu.int_cycles; /* ASG */
                    cpu.int_ack_callback = m68k_cpu.int_ack_callback;
                    cpu.bkpt_ack_callback = m68k_cpu.bkpt_ack_callback;
                    cpu.reset_instr_callback = m68k_cpu.reset_instr_callback;
                    cpu.pc_changed_callback = m68k_cpu.pc_changed_callback;
                    cpu.set_fc_callback = m68k_cpu.set_fc_callback;
                    cpu.instr_hook_callback = m68k_cpu.instr_hook_callback;
                    cpu.pref_addr = m68k_cpu.pref_addr;
                    cpu.pref_data = m68k_cpu.pref_data;
                }
                return sizeof(m68k_cpu_context);
            }

            void m68k_set_context(object src)
            {
                if (src!=null)
                {
                    m68k_cpu_context cpu = (m68k_cpu_context)src;

                    m68k_cpu.mode = cpu.mode;
                    m68ki_set_sr_no_int(cpu.sr); /* This stays on top to prevent side-effects */
                    m68ki_set_pc(cpu.pc);
                    memcpy(m68k_cpu.dr, cpu.d, sizeof(m68k_cpu.dr));
                    memcpy(m68k_cpu.ar, cpu.a, sizeof(m68k_cpu.dr));
                    m68k_cpu.sp[0] = cpu.usp;
                    m68k_cpu.sp[1] = cpu.isp;
                    m68k_cpu.sp[3] = cpu.msp;
                    m68k_cpu.vbr = cpu.vbr;
                    m68k_cpu.sfc = cpu.sfc;
                    m68k_cpu.dfc = cpu.dfc;
                    m68k_cpu.stopped = cpu.stopped;
                    m68k_cpu.halted = cpu.halted;
                    m68k_cpu.int_state = cpu.int_state; /* ASG: changed from CPU_INTS_PENDING */
                    m68k_cpu.int_cycles = cpu.int_cycles; /* ASG */
                    m68k_cpu.int_ack_callback = cpu.int_ack_callback;
                    m68k_cpu.bkpt_ack_callback = cpu.bkpt_ack_callback;
                    m68k_cpu.reset_instr_callback = cpu.reset_instr_callback;
                    m68k_cpu.pc_changed_callback = cpu.pc_changed_callback;
                    m68k_cpu.set_fc_callback = cpu.set_fc_callback;
                    m68k_cpu.instr_hook_callback = cpu.instr_hook_callback;
                    m68k_cpu.pref_addr = cpu.pref_addr;
                    m68k_cpu.pref_data = cpu.pref_data;

                    /* ASG: check for interrupts */
                    m68ki_check_interrupts();
                }
            }


            /* Check if the instruction is a valid one */
            int m68k_is_valid_instruction(int instruction, int cpu_mode)
            {
                if (m68k_instruction_jump_table[((instruction) & 0xffff)] == m68000_illegal)
                    return 0;
                if (!(cpu_mode & (2 | 4 | 8)))
                {
                    if ((instruction & 0xfff8) == 0x4848) /* bkpt */
                        return 0;
                    if ((instruction & 0xffc0) == 0x42c0) /* move from ccr */
                        return 0;
                    if ((instruction & 0xfffe) == 0x4e7a) /* movec */
                        return 0;
                    if ((instruction & 0xff00) == 0x0e00) /* moves */
                        return 0;
                    if ((instruction & 0xffff) == 0x4e74) /* rtd */
                        return 0;
                }
                if (!(cpu_mode & (4 | 8)))
                {
                    if ((instruction & 0xf0ff) == 0x60ff) /* bcc.l */
                        return 0;
                    if ((instruction & 0xf8c0) == 0xe8c0) /* bfxxx */
                        return 0;
                    if ((instruction & 0xffc0) == 0x06c0) /* callm */
                        return 0;
                    if ((instruction & 0xf9c0) == 0x08c0) /* cas */
                        return 0;
                    if ((instruction & 0xf9ff) == 0x08fc) /* cas2 */
                        return 0;
                    if ((instruction & 0xf1c0) == 0x4100) /* chk.l */
                        return 0;
                    if ((instruction & 0xf9c0) == 0x00c0) /* chk2, cmp2 */
                        return 0;
                    if ((instruction & 0xff3f) == 0x0c3a) /* cmpi (pcdi) */
                        return 0;
                    if ((instruction & 0xff3f) == 0x0c3b) /* cmpi (pcix) */
                        return 0;
                    if ((instruction & 0xffc0) == 0x4c40) /* divl */
                        return 0;
                    if ((instruction & 0xfff8) == 0x49c0) /* extb */
                        return 0;
                    if ((instruction & 0xfff8) == 0x4808) /* link.l */
                        return 0;
                    if ((instruction & 0xffc0) == 0x4c00) /* mull */
                        return 0;
                    if ((instruction & 0xf1f0) == 0x8140) /* pack */
                        return 0;
                    if ((instruction & 0xfff0) == 0x06c0) /* rtm */
                        return 0;
                    if ((instruction & 0xf0f8) == 0x50f8) /* trapcc */
                        return 0;
                    if ((instruction & 0xff38) == 0x4a08) /* tst (a) */
                        return 0;
                    if ((instruction & 0xff3f) == 0x4a3a) /* tst (pcdi) */
                        return 0;
                    if ((instruction & 0xff3f) == 0x4a3b) /* tst (pcix) */
                        return 0;
                    if ((instruction & 0xff3f) == 0x4a3c) /* tst (imm) */
                        return 0;
                    if ((instruction & 0xf1f0) == 0x8180) /* unpk */
                        return 0;
                }
                return 1;
            }






            /* This is used to generate the opcode handler jump table */
            class opcode_handler_struct
            {
                public opcode opcode_handler; /* handler function */
                public uint bits; /* number of bits set in mask */
                public uint mask; /* mask on opcode */
                public uint match; /* what to match after masking */
                public opcode_handler_struct(opcode opcode_handler, uint bits, uint mask, uint match)
                {
                    this.opcode_handler = opcode_handler;
                    this.bits = bits;
                    this.mask = mask;
                    this.match = match;
                }
            }


            /* Opcode handler table */
            static opcode_handler_struct[] m68k_opcode_handler_table =
{
/*  opcode handler              mask   match */
 new opcode_handler_struct(m68000_1010 , 4, 0xf000, 0xa000),
 new opcode_handler_struct(m68000_1111 , 4, 0xf000, 0xf000),
 new opcode_handler_struct(m68000_abcd_rr , 10, 0xf1f8, 0xc100),
 new opcode_handler_struct(m68000_abcd_mm_ax7 , 13, 0xfff8, 0xcf08),
 new opcode_handler_struct(m68000_abcd_mm_ay7 , 13, 0xf1ff, 0xc10f),
 new opcode_handler_struct(m68000_abcd_mm_axy7 , 16, 0xffff, 0xcf0f),
 new opcode_handler_struct(m68000_abcd_mm , 10, 0xf1f8, 0xc108),
 new opcode_handler_struct(m68000_add_er_d_8 , 10, 0xf1f8, 0xd000),
 new opcode_handler_struct(m68000_add_er_ai_8 , 10, 0xf1f8, 0xd010),
 new opcode_handler_struct(m68000_add_er_pi_8 , 10, 0xf1f8, 0xd018),
 new opcode_handler_struct(m68000_add_er_pi7_8 , 13, 0xf1ff, 0xd01f),
 new opcode_handler_struct(m68000_add_er_pd_8 , 10, 0xf1f8, 0xd020),
 new opcode_handler_struct(m68000_add_er_pd7_8 , 13, 0xf1ff, 0xd027),
 new opcode_handler_struct(m68000_add_er_di_8 , 10, 0xf1f8, 0xd028),
 new opcode_handler_struct(m68000_add_er_ix_8 , 10, 0xf1f8, 0xd030),
 new opcode_handler_struct(m68000_add_er_aw_8 , 13, 0xf1ff, 0xd038),
 new opcode_handler_struct(m68000_add_er_al_8 , 13, 0xf1ff, 0xd039),
 new opcode_handler_struct(m68000_add_er_pcdi_8 , 13, 0xf1ff, 0xd03a),
 new opcode_handler_struct(m68000_add_er_pcix_8 , 13, 0xf1ff, 0xd03b),
 new opcode_handler_struct(m68000_add_er_i_8 , 13, 0xf1ff, 0xd03c),
 new opcode_handler_struct(m68000_add_er_d_16 , 10, 0xf1f8, 0xd040),
 new opcode_handler_struct(m68000_add_er_a_16 , 10, 0xf1f8, 0xd048),
 new opcode_handler_struct(m68000_add_er_ai_16 , 10, 0xf1f8, 0xd050),
 new opcode_handler_struct(m68000_add_er_pi_16 , 10, 0xf1f8, 0xd058),
 new opcode_handler_struct(m68000_add_er_pd_16 , 10, 0xf1f8, 0xd060),
 new opcode_handler_struct(m68000_add_er_di_16 , 10, 0xf1f8, 0xd068),
 new opcode_handler_struct(m68000_add_er_ix_16 , 10, 0xf1f8, 0xd070),
 new opcode_handler_struct(m68000_add_er_aw_16 , 13, 0xf1ff, 0xd078),
 new opcode_handler_struct(m68000_add_er_al_16 , 13, 0xf1ff, 0xd079),
 new opcode_handler_struct(m68000_add_er_pcdi_16 , 13, 0xf1ff, 0xd07a),
 new opcode_handler_struct(m68000_add_er_pcix_16 , 13, 0xf1ff, 0xd07b),
 new opcode_handler_struct(m68000_add_er_i_16 , 13, 0xf1ff, 0xd07c),
 new opcode_handler_struct(m68000_add_er_d_32 , 10, 0xf1f8, 0xd080),
 new opcode_handler_struct(m68000_add_er_a_32 , 10, 0xf1f8, 0xd088),
 new opcode_handler_struct(m68000_add_er_ai_32 , 10, 0xf1f8, 0xd090),
 new opcode_handler_struct(m68000_add_er_pi_32 , 10, 0xf1f8, 0xd098),
 new opcode_handler_struct(m68000_add_er_pd_32 , 10, 0xf1f8, 0xd0a0),
 new opcode_handler_struct(m68000_add_er_di_32 , 10, 0xf1f8, 0xd0a8),
 new opcode_handler_struct(m68000_add_er_ix_32 , 10, 0xf1f8, 0xd0b0),
 new opcode_handler_struct(m68000_add_er_aw_32 , 13, 0xf1ff, 0xd0b8),
 new opcode_handler_struct(m68000_add_er_al_32 , 13, 0xf1ff, 0xd0b9),
 new opcode_handler_struct(m68000_add_er_pcdi_32 , 13, 0xf1ff, 0xd0ba),
 new opcode_handler_struct(m68000_add_er_pcix_32 , 13, 0xf1ff, 0xd0bb),
 new opcode_handler_struct(m68000_add_er_i_32 , 13, 0xf1ff, 0xd0bc),
 new opcode_handler_struct(m68000_add_re_ai_8 , 10, 0xf1f8, 0xd110),
 new opcode_handler_struct(m68000_add_re_pi_8 , 10, 0xf1f8, 0xd118),
 new opcode_handler_struct(m68000_add_re_pi7_8 , 13, 0xf1ff, 0xd11f),
 new opcode_handler_struct(m68000_add_re_pd_8 , 10, 0xf1f8, 0xd120),
 new opcode_handler_struct(m68000_add_re_pd7_8 , 13, 0xf1ff, 0xd127),
 new opcode_handler_struct(m68000_add_re_di_8 , 10, 0xf1f8, 0xd128),
 new opcode_handler_struct(m68000_add_re_ix_8 , 10, 0xf1f8, 0xd130),
 new opcode_handler_struct(m68000_add_re_aw_8 , 13, 0xf1ff, 0xd138),
 new opcode_handler_struct(m68000_add_re_al_8 , 13, 0xf1ff, 0xd139),
 new opcode_handler_struct(m68000_add_re_ai_16 , 10, 0xf1f8, 0xd150),
 new opcode_handler_struct(m68000_add_re_pi_16 , 10, 0xf1f8, 0xd158),
 new opcode_handler_struct(m68000_add_re_pd_16 , 10, 0xf1f8, 0xd160),
 new opcode_handler_struct(m68000_add_re_di_16 , 10, 0xf1f8, 0xd168),
 new opcode_handler_struct(m68000_add_re_ix_16 , 10, 0xf1f8, 0xd170),
 new opcode_handler_struct(m68000_add_re_aw_16 , 13, 0xf1ff, 0xd178),
 new opcode_handler_struct(m68000_add_re_al_16 , 13, 0xf1ff, 0xd179),
 new opcode_handler_struct(m68000_add_re_ai_32 , 10, 0xf1f8, 0xd190),
 new opcode_handler_struct(m68000_add_re_pi_32 , 10, 0xf1f8, 0xd198),
 new opcode_handler_struct(m68000_add_re_pd_32 , 10, 0xf1f8, 0xd1a0),
 new opcode_handler_struct(m68000_add_re_di_32 , 10, 0xf1f8, 0xd1a8),
 new opcode_handler_struct(m68000_add_re_ix_32 , 10, 0xf1f8, 0xd1b0),
 new opcode_handler_struct(m68000_add_re_aw_32 , 13, 0xf1ff, 0xd1b8),
 new opcode_handler_struct(m68000_add_re_al_32 , 13, 0xf1ff, 0xd1b9),
 new opcode_handler_struct(m68000_adda_d_16 , 10, 0xf1f8, 0xd0c0),
 new opcode_handler_struct(m68000_adda_a_16 , 10, 0xf1f8, 0xd0c8),
 new opcode_handler_struct(m68000_adda_ai_16 , 10, 0xf1f8, 0xd0d0),
 new opcode_handler_struct(m68000_adda_pi_16 , 10, 0xf1f8, 0xd0d8),
 new opcode_handler_struct(m68000_adda_pd_16 , 10, 0xf1f8, 0xd0e0),
 new opcode_handler_struct(m68000_adda_di_16 , 10, 0xf1f8, 0xd0e8),
 new opcode_handler_struct(m68000_adda_ix_16 , 10, 0xf1f8, 0xd0f0),
 new opcode_handler_struct(m68000_adda_aw_16 , 13, 0xf1ff, 0xd0f8),
 new opcode_handler_struct(m68000_adda_al_16 , 13, 0xf1ff, 0xd0f9),
 new opcode_handler_struct(m68000_adda_pcdi_16 , 13, 0xf1ff, 0xd0fa),
 new opcode_handler_struct(m68000_adda_pcix_16 , 13, 0xf1ff, 0xd0fb),
 new opcode_handler_struct(m68000_adda_i_16 , 13, 0xf1ff, 0xd0fc),
 new opcode_handler_struct(m68000_adda_d_32 , 10, 0xf1f8, 0xd1c0),
 new opcode_handler_struct(m68000_adda_a_32 , 10, 0xf1f8, 0xd1c8),
 new opcode_handler_struct(m68000_adda_ai_32 , 10, 0xf1f8, 0xd1d0),
 new opcode_handler_struct(m68000_adda_pi_32 , 10, 0xf1f8, 0xd1d8),
 new opcode_handler_struct(m68000_adda_pd_32 , 10, 0xf1f8, 0xd1e0),
 new opcode_handler_struct(m68000_adda_di_32 , 10, 0xf1f8, 0xd1e8),
 new opcode_handler_struct(m68000_adda_ix_32 , 10, 0xf1f8, 0xd1f0),
 new opcode_handler_struct(m68000_adda_aw_32 , 13, 0xf1ff, 0xd1f8),
 new opcode_handler_struct(m68000_adda_al_32 , 13, 0xf1ff, 0xd1f9),
 new opcode_handler_struct(m68000_adda_pcdi_32 , 13, 0xf1ff, 0xd1fa),
 new opcode_handler_struct(m68000_adda_pcix_32 , 13, 0xf1ff, 0xd1fb),
 new opcode_handler_struct(m68000_adda_i_32 , 13, 0xf1ff, 0xd1fc),
 new opcode_handler_struct(m68000_addi_d_8 , 13, 0xfff8, 0x0600),
 new opcode_handler_struct(m68000_addi_ai_8 , 13, 0xfff8, 0x0610),
 new opcode_handler_struct(m68000_addi_pi_8 , 13, 0xfff8, 0x0618),
 new opcode_handler_struct(m68000_addi_pi7_8 , 16, 0xffff, 0x061f),
 new opcode_handler_struct(m68000_addi_pd_8 , 13, 0xfff8, 0x0620),
 new opcode_handler_struct(m68000_addi_pd7_8 , 16, 0xffff, 0x0627),
 new opcode_handler_struct(m68000_addi_di_8 , 13, 0xfff8, 0x0628),
 new opcode_handler_struct(m68000_addi_ix_8 , 13, 0xfff8, 0x0630),
 new opcode_handler_struct(m68000_addi_aw_8 , 16, 0xffff, 0x0638),
 new opcode_handler_struct(m68000_addi_al_8 , 16, 0xffff, 0x0639),
 new opcode_handler_struct(m68000_addi_d_16 , 13, 0xfff8, 0x0640),
 new opcode_handler_struct(m68000_addi_ai_16 , 13, 0xfff8, 0x0650),
 new opcode_handler_struct(m68000_addi_pi_16 , 13, 0xfff8, 0x0658),
 new opcode_handler_struct(m68000_addi_pd_16 , 13, 0xfff8, 0x0660),
 new opcode_handler_struct(m68000_addi_di_16 , 13, 0xfff8, 0x0668),
 new opcode_handler_struct(m68000_addi_ix_16 , 13, 0xfff8, 0x0670),
 new opcode_handler_struct(m68000_addi_aw_16 , 16, 0xffff, 0x0678),
 new opcode_handler_struct(m68000_addi_al_16 , 16, 0xffff, 0x0679),
 new opcode_handler_struct(m68000_addi_d_32 , 13, 0xfff8, 0x0680),
 new opcode_handler_struct(m68000_addi_ai_32 , 13, 0xfff8, 0x0690),
 new opcode_handler_struct(m68000_addi_pi_32 , 13, 0xfff8, 0x0698),
 new opcode_handler_struct(m68000_addi_pd_32 , 13, 0xfff8, 0x06a0),
 new opcode_handler_struct(m68000_addi_di_32 , 13, 0xfff8, 0x06a8),
 new opcode_handler_struct(m68000_addi_ix_32 , 13, 0xfff8, 0x06b0),
 new opcode_handler_struct(m68000_addi_aw_32 , 16, 0xffff, 0x06b8),
 new opcode_handler_struct(m68000_addi_al_32 , 16, 0xffff, 0x06b9),
 new opcode_handler_struct(m68000_addq_d_8 , 10, 0xf1f8, 0x5000),
 new opcode_handler_struct(m68000_addq_ai_8 , 10, 0xf1f8, 0x5010),
 new opcode_handler_struct(m68000_addq_pi_8 , 10, 0xf1f8, 0x5018),
 new opcode_handler_struct(m68000_addq_pi7_8 , 13, 0xf1ff, 0x501f),
 new opcode_handler_struct(m68000_addq_pd_8 , 10, 0xf1f8, 0x5020),
 new opcode_handler_struct(m68000_addq_pd7_8 , 13, 0xf1ff, 0x5027),
 new opcode_handler_struct(m68000_addq_di_8 , 10, 0xf1f8, 0x5028),
 new opcode_handler_struct(m68000_addq_ix_8 , 10, 0xf1f8, 0x5030),
 new opcode_handler_struct(m68000_addq_aw_8 , 13, 0xf1ff, 0x5038),
 new opcode_handler_struct(m68000_addq_al_8 , 13, 0xf1ff, 0x5039),
 new opcode_handler_struct(m68000_addq_d_16 , 10, 0xf1f8, 0x5040),
 new opcode_handler_struct(m68000_addq_a_16 , 10, 0xf1f8, 0x5048),
 new opcode_handler_struct(m68000_addq_ai_16 , 10, 0xf1f8, 0x5050),
 new opcode_handler_struct(m68000_addq_pi_16 , 10, 0xf1f8, 0x5058),
 new opcode_handler_struct(m68000_addq_pd_16 , 10, 0xf1f8, 0x5060),
 new opcode_handler_struct(m68000_addq_di_16 , 10, 0xf1f8, 0x5068),
 new opcode_handler_struct(m68000_addq_ix_16 , 10, 0xf1f8, 0x5070),
 new opcode_handler_struct(m68000_addq_aw_16 , 13, 0xf1ff, 0x5078),
 new opcode_handler_struct(m68000_addq_al_16 , 13, 0xf1ff, 0x5079),
 new opcode_handler_struct(m68000_addq_d_32 , 10, 0xf1f8, 0x5080),
 new opcode_handler_struct(m68000_addq_a_32 , 10, 0xf1f8, 0x5088),
 new opcode_handler_struct(m68000_addq_ai_32 , 10, 0xf1f8, 0x5090),
 new opcode_handler_struct(m68000_addq_pi_32 , 10, 0xf1f8, 0x5098),
 new opcode_handler_struct(m68000_addq_pd_32 , 10, 0xf1f8, 0x50a0),
 new opcode_handler_struct(m68000_addq_di_32 , 10, 0xf1f8, 0x50a8),
 new opcode_handler_struct(m68000_addq_ix_32 , 10, 0xf1f8, 0x50b0),
 new opcode_handler_struct(m68000_addq_aw_32 , 13, 0xf1ff, 0x50b8),
 new opcode_handler_struct(m68000_addq_al_32 , 13, 0xf1ff, 0x50b9),
 new opcode_handler_struct(m68000_addx_rr_8 , 10, 0xf1f8, 0xd100),
 new opcode_handler_struct(m68000_addx_rr_16 , 10, 0xf1f8, 0xd140),
 new opcode_handler_struct(m68000_addx_rr_32 , 10, 0xf1f8, 0xd180),
 new opcode_handler_struct(m68000_addx_mm_8_ax7 , 13, 0xfff8, 0xdf08),
 new opcode_handler_struct(m68000_addx_mm_8_ay7 , 13, 0xf1ff, 0xd10f),
 new opcode_handler_struct(m68000_addx_mm_8_axy7 , 16, 0xffff, 0xdf0f),
 new opcode_handler_struct(m68000_addx_mm_8 , 10, 0xf1f8, 0xd108),
 new opcode_handler_struct(m68000_addx_mm_16 , 10, 0xf1f8, 0xd148),
 new opcode_handler_struct(m68000_addx_mm_32 , 10, 0xf1f8, 0xd188),
 new opcode_handler_struct(m68000_and_er_d_8 , 10, 0xf1f8, 0xc000),
 new opcode_handler_struct(m68000_and_er_ai_8 , 10, 0xf1f8, 0xc010),
 new opcode_handler_struct(m68000_and_er_pi_8 , 10, 0xf1f8, 0xc018),
 new opcode_handler_struct(m68000_and_er_pi7_8 , 13, 0xf1ff, 0xc01f),
 new opcode_handler_struct(m68000_and_er_pd_8 , 10, 0xf1f8, 0xc020),
 new opcode_handler_struct(m68000_and_er_pd7_8 , 13, 0xf1ff, 0xc027),
 new opcode_handler_struct(m68000_and_er_di_8 , 10, 0xf1f8, 0xc028),
 new opcode_handler_struct(m68000_and_er_ix_8 , 10, 0xf1f8, 0xc030),
 new opcode_handler_struct(m68000_and_er_aw_8 , 13, 0xf1ff, 0xc038),
 new opcode_handler_struct(m68000_and_er_al_8 , 13, 0xf1ff, 0xc039),
 new opcode_handler_struct(m68000_and_er_pcdi_8 , 13, 0xf1ff, 0xc03a),
 new opcode_handler_struct(m68000_and_er_pcix_8 , 13, 0xf1ff, 0xc03b),
 new opcode_handler_struct(m68000_and_er_i_8 , 13, 0xf1ff, 0xc03c),
 new opcode_handler_struct(m68000_and_er_d_16 , 10, 0xf1f8, 0xc040),
 new opcode_handler_struct(m68000_and_er_ai_16 , 10, 0xf1f8, 0xc050),
 new opcode_handler_struct(m68000_and_er_pi_16 , 10, 0xf1f8, 0xc058),
 new opcode_handler_struct(m68000_and_er_pd_16 , 10, 0xf1f8, 0xc060),
 new opcode_handler_struct(m68000_and_er_di_16 , 10, 0xf1f8, 0xc068),
 new opcode_handler_struct(m68000_and_er_ix_16 , 10, 0xf1f8, 0xc070),
 new opcode_handler_struct(m68000_and_er_aw_16 , 13, 0xf1ff, 0xc078),
 new opcode_handler_struct(m68000_and_er_al_16 , 13, 0xf1ff, 0xc079),
 new opcode_handler_struct(m68000_and_er_pcdi_16 , 13, 0xf1ff, 0xc07a),
 new opcode_handler_struct(m68000_and_er_pcix_16 , 13, 0xf1ff, 0xc07b),
 new opcode_handler_struct(m68000_and_er_i_16 , 13, 0xf1ff, 0xc07c),
 new opcode_handler_struct(m68000_and_er_d_32 , 10, 0xf1f8, 0xc080),
 new opcode_handler_struct(m68000_and_er_ai_32 , 10, 0xf1f8, 0xc090),
 new opcode_handler_struct(m68000_and_er_pi_32 , 10, 0xf1f8, 0xc098),
 new opcode_handler_struct(m68000_and_er_pd_32 , 10, 0xf1f8, 0xc0a0),
 new opcode_handler_struct(m68000_and_er_di_32 , 10, 0xf1f8, 0xc0a8),
 new opcode_handler_struct(m68000_and_er_ix_32 , 10, 0xf1f8, 0xc0b0),
 new opcode_handler_struct(m68000_and_er_aw_32 , 13, 0xf1ff, 0xc0b8),
 new opcode_handler_struct(m68000_and_er_al_32 , 13, 0xf1ff, 0xc0b9),
 new opcode_handler_struct(m68000_and_er_pcdi_32 , 13, 0xf1ff, 0xc0ba),
 new opcode_handler_struct(m68000_and_er_pcix_32 , 13, 0xf1ff, 0xc0bb),
 new opcode_handler_struct(m68000_and_er_i_32 , 13, 0xf1ff, 0xc0bc),
 new opcode_handler_struct(m68000_and_re_ai_8 , 10, 0xf1f8, 0xc110),
 new opcode_handler_struct(m68000_and_re_pi_8 , 10, 0xf1f8, 0xc118),
 new opcode_handler_struct(m68000_and_re_pi7_8 , 13, 0xf1ff, 0xc11f),
 new opcode_handler_struct(m68000_and_re_pd_8 , 10, 0xf1f8, 0xc120),
 new opcode_handler_struct(m68000_and_re_pd7_8 , 13, 0xf1ff, 0xc127),
 new opcode_handler_struct(m68000_and_re_di_8 , 10, 0xf1f8, 0xc128),
 new opcode_handler_struct(m68000_and_re_ix_8 , 10, 0xf1f8, 0xc130),
 new opcode_handler_struct(m68000_and_re_aw_8 , 13, 0xf1ff, 0xc138),
 new opcode_handler_struct(m68000_and_re_al_8 , 13, 0xf1ff, 0xc139),
 new opcode_handler_struct(m68000_and_re_ai_16 , 10, 0xf1f8, 0xc150),
 new opcode_handler_struct(m68000_and_re_pi_16 , 10, 0xf1f8, 0xc158),
 new opcode_handler_struct(m68000_and_re_pd_16 , 10, 0xf1f8, 0xc160),
 new opcode_handler_struct(m68000_and_re_di_16 , 10, 0xf1f8, 0xc168),
 new opcode_handler_struct(m68000_and_re_ix_16 , 10, 0xf1f8, 0xc170),
 new opcode_handler_struct(m68000_and_re_aw_16 , 13, 0xf1ff, 0xc178),
 new opcode_handler_struct(m68000_and_re_al_16 , 13, 0xf1ff, 0xc179),
 new opcode_handler_struct(m68000_and_re_ai_32 , 10, 0xf1f8, 0xc190),
 new opcode_handler_struct(m68000_and_re_pi_32 , 10, 0xf1f8, 0xc198),
 new opcode_handler_struct(m68000_and_re_pd_32 , 10, 0xf1f8, 0xc1a0),
 new opcode_handler_struct(m68000_and_re_di_32 , 10, 0xf1f8, 0xc1a8),
 new opcode_handler_struct(m68000_and_re_ix_32 , 10, 0xf1f8, 0xc1b0),
 new opcode_handler_struct(m68000_and_re_aw_32 , 13, 0xf1ff, 0xc1b8),
 new opcode_handler_struct(m68000_and_re_al_32 , 13, 0xf1ff, 0xc1b9),
 new opcode_handler_struct(m68000_andi_to_ccr , 16, 0xffff, 0x023c),
 new opcode_handler_struct(m68000_andi_to_sr , 16, 0xffff, 0x027c),
 new opcode_handler_struct(m68000_andi_d_8 , 13, 0xfff8, 0x0200),
 new opcode_handler_struct(m68000_andi_ai_8 , 13, 0xfff8, 0x0210),
 new opcode_handler_struct(m68000_andi_pi_8 , 13, 0xfff8, 0x0218),
 new opcode_handler_struct(m68000_andi_pi7_8 , 16, 0xffff, 0x021f),
 new opcode_handler_struct(m68000_andi_pd_8 , 13, 0xfff8, 0x0220),
 new opcode_handler_struct(m68000_andi_pd7_8 , 16, 0xffff, 0x0227),
 new opcode_handler_struct(m68000_andi_di_8 , 13, 0xfff8, 0x0228),
 new opcode_handler_struct(m68000_andi_ix_8 , 13, 0xfff8, 0x0230),
 new opcode_handler_struct(m68000_andi_aw_8 , 16, 0xffff, 0x0238),
 new opcode_handler_struct(m68000_andi_al_8 , 16, 0xffff, 0x0239),
 new opcode_handler_struct(m68000_andi_d_16 , 13, 0xfff8, 0x0240),
 new opcode_handler_struct(m68000_andi_ai_16 , 13, 0xfff8, 0x0250),
 new opcode_handler_struct(m68000_andi_pi_16 , 13, 0xfff8, 0x0258),
 new opcode_handler_struct(m68000_andi_pd_16 , 13, 0xfff8, 0x0260),
 new opcode_handler_struct(m68000_andi_di_16 , 13, 0xfff8, 0x0268),
 new opcode_handler_struct(m68000_andi_ix_16 , 13, 0xfff8, 0x0270),
 new opcode_handler_struct(m68000_andi_aw_16 , 16, 0xffff, 0x0278),
 new opcode_handler_struct(m68000_andi_al_16 , 16, 0xffff, 0x0279),
 new opcode_handler_struct(m68000_andi_d_32 , 13, 0xfff8, 0x0280),
 new opcode_handler_struct(m68000_andi_ai_32 , 13, 0xfff8, 0x0290),
 new opcode_handler_struct(m68000_andi_pi_32 , 13, 0xfff8, 0x0298),
 new opcode_handler_struct(m68000_andi_pd_32 , 13, 0xfff8, 0x02a0),
 new opcode_handler_struct(m68000_andi_di_32 , 13, 0xfff8, 0x02a8),
 new opcode_handler_struct(m68000_andi_ix_32 , 13, 0xfff8, 0x02b0),
 new opcode_handler_struct(m68000_andi_aw_32 , 16, 0xffff, 0x02b8),
 new opcode_handler_struct(m68000_andi_al_32 , 16, 0xffff, 0x02b9),
 new opcode_handler_struct(m68000_asr_s_8 , 10, 0xf1f8, 0xe000),
 new opcode_handler_struct(m68000_asr_s_16 , 10, 0xf1f8, 0xe040),
 new opcode_handler_struct(m68000_asr_s_32 , 10, 0xf1f8, 0xe080),
 new opcode_handler_struct(m68000_asr_r_8 , 10, 0xf1f8, 0xe020),
 new opcode_handler_struct(m68000_asr_r_16 , 10, 0xf1f8, 0xe060),
 new opcode_handler_struct(m68000_asr_r_32 , 10, 0xf1f8, 0xe0a0),
 new opcode_handler_struct(m68000_asr_ea_ai , 13, 0xfff8, 0xe0d0),
 new opcode_handler_struct(m68000_asr_ea_pi , 13, 0xfff8, 0xe0d8),
 new opcode_handler_struct(m68000_asr_ea_pd , 13, 0xfff8, 0xe0e0),
 new opcode_handler_struct(m68000_asr_ea_di , 13, 0xfff8, 0xe0e8),
 new opcode_handler_struct(m68000_asr_ea_ix , 13, 0xfff8, 0xe0f0),
 new opcode_handler_struct(m68000_asr_ea_aw , 16, 0xffff, 0xe0f8),
 new opcode_handler_struct(m68000_asr_ea_al , 16, 0xffff, 0xe0f9),
 new opcode_handler_struct(m68000_asl_s_8 , 10, 0xf1f8, 0xe100),
 new opcode_handler_struct(m68000_asl_s_16 , 10, 0xf1f8, 0xe140),
 new opcode_handler_struct(m68000_asl_s_32 , 10, 0xf1f8, 0xe180),
 new opcode_handler_struct(m68000_asl_r_8 , 10, 0xf1f8, 0xe120),
 new opcode_handler_struct(m68000_asl_r_16 , 10, 0xf1f8, 0xe160),
 new opcode_handler_struct(m68000_asl_r_32 , 10, 0xf1f8, 0xe1a0),
 new opcode_handler_struct(m68000_asl_ea_ai , 13, 0xfff8, 0xe1d0),
 new opcode_handler_struct(m68000_asl_ea_pi , 13, 0xfff8, 0xe1d8),
 new opcode_handler_struct(m68000_asl_ea_pd , 13, 0xfff8, 0xe1e0),
 new opcode_handler_struct(m68000_asl_ea_di , 13, 0xfff8, 0xe1e8),
 new opcode_handler_struct(m68000_asl_ea_ix , 13, 0xfff8, 0xe1f0),
 new opcode_handler_struct(m68000_asl_ea_aw , 16, 0xffff, 0xe1f8),
 new opcode_handler_struct(m68000_asl_ea_al , 16, 0xffff, 0xe1f9),
 new opcode_handler_struct(m68000_bhi_16 , 16, 0xffff, 0x6200),
 new opcode_handler_struct(m68020_bhi_32 , 16, 0xffff, 0x62ff),
 new opcode_handler_struct(m68000_bhi_8 , 8, 0xff00, 0x6200),
 new opcode_handler_struct(m68000_bls_16 , 16, 0xffff, 0x6300),
 new opcode_handler_struct(m68020_bls_32 , 16, 0xffff, 0x63ff),
 new opcode_handler_struct(m68000_bls_8 , 8, 0xff00, 0x6300),
 new opcode_handler_struct(m68000_bcc_16 , 16, 0xffff, 0x6400),
 new opcode_handler_struct(m68020_bcc_32 , 16, 0xffff, 0x64ff),
 new opcode_handler_struct(m68000_bcc_8 , 8, 0xff00, 0x6400),
 new opcode_handler_struct(m68000_bcs_16 , 16, 0xffff, 0x6500),
 new opcode_handler_struct(m68020_bcs_32 , 16, 0xffff, 0x65ff),
 new opcode_handler_struct(m68000_bcs_8 , 8, 0xff00, 0x6500),
 new opcode_handler_struct(m68000_bne_16 , 16, 0xffff, 0x6600),
 new opcode_handler_struct(m68020_bne_32 , 16, 0xffff, 0x66ff),
 new opcode_handler_struct(m68000_bne_8 , 8, 0xff00, 0x6600),
 new opcode_handler_struct(m68000_beq_16 , 16, 0xffff, 0x6700),
 new opcode_handler_struct(m68020_beq_32 , 16, 0xffff, 0x67ff),
 new opcode_handler_struct(m68000_beq_8 , 8, 0xff00, 0x6700),
 new opcode_handler_struct(m68000_bvc_16 , 16, 0xffff, 0x6800),
 new opcode_handler_struct(m68020_bvc_32 , 16, 0xffff, 0x68ff),
 new opcode_handler_struct(m68000_bvc_8 , 8, 0xff00, 0x6800),
 new opcode_handler_struct(m68000_bvs_16 , 16, 0xffff, 0x6900),
 new opcode_handler_struct(m68020_bvs_32 , 16, 0xffff, 0x69ff),
 new opcode_handler_struct(m68000_bvs_8 , 8, 0xff00, 0x6900),
 new opcode_handler_struct(m68000_bpl_16 , 16, 0xffff, 0x6a00),
 new opcode_handler_struct(m68020_bpl_32 , 16, 0xffff, 0x6aff),
 new opcode_handler_struct(m68000_bpl_8 , 8, 0xff00, 0x6a00),
 new opcode_handler_struct(m68000_bmi_16 , 16, 0xffff, 0x6b00),
 new opcode_handler_struct(m68020_bmi_32 , 16, 0xffff, 0x6bff),
 new opcode_handler_struct(m68000_bmi_8 , 8, 0xff00, 0x6b00),
 new opcode_handler_struct(m68000_bge_16 , 16, 0xffff, 0x6c00),
 new opcode_handler_struct(m68020_bge_32 , 16, 0xffff, 0x6cff),
 new opcode_handler_struct(m68000_bge_8 , 8, 0xff00, 0x6c00),
 new opcode_handler_struct(m68000_blt_16 , 16, 0xffff, 0x6d00),
 new opcode_handler_struct(m68020_blt_32 , 16, 0xffff, 0x6dff),
 new opcode_handler_struct(m68000_blt_8 , 8, 0xff00, 0x6d00),
 new opcode_handler_struct(m68000_bgt_16 , 16, 0xffff, 0x6e00),
 new opcode_handler_struct(m68020_bgt_32 , 16, 0xffff, 0x6eff),
 new opcode_handler_struct(m68000_bgt_8 , 8, 0xff00, 0x6e00),
 new opcode_handler_struct(m68000_ble_16 , 16, 0xffff, 0x6f00),
 new opcode_handler_struct(m68020_ble_32 , 16, 0xffff, 0x6fff),
 new opcode_handler_struct(m68000_ble_8 , 8, 0xff00, 0x6f00),
 new opcode_handler_struct(m68000_bchg_r_d , 10, 0xf1f8, 0x0140),
 new opcode_handler_struct(m68000_bchg_r_ai , 10, 0xf1f8, 0x0150),
 new opcode_handler_struct(m68000_bchg_r_pi , 10, 0xf1f8, 0x0158),
 new opcode_handler_struct(m68000_bchg_r_pi7 , 13, 0xf1ff, 0x015f),
 new opcode_handler_struct(m68000_bchg_r_pd , 10, 0xf1f8, 0x0160),
 new opcode_handler_struct(m68000_bchg_r_pd7 , 13, 0xf1ff, 0x0167),
 new opcode_handler_struct(m68000_bchg_r_di , 10, 0xf1f8, 0x0168),
 new opcode_handler_struct(m68000_bchg_r_ix , 10, 0xf1f8, 0x0170),
 new opcode_handler_struct(m68000_bchg_r_aw , 13, 0xf1ff, 0x0178),
 new opcode_handler_struct(m68000_bchg_r_al , 13, 0xf1ff, 0x0179),
 new opcode_handler_struct(m68000_bchg_s_d , 13, 0xfff8, 0x0840),
 new opcode_handler_struct(m68000_bchg_s_ai , 13, 0xfff8, 0x0850),
 new opcode_handler_struct(m68000_bchg_s_pi , 13, 0xfff8, 0x0858),
 new opcode_handler_struct(m68000_bchg_s_pi7 , 16, 0xffff, 0x085f),
 new opcode_handler_struct(m68000_bchg_s_pd , 13, 0xfff8, 0x0860),
 new opcode_handler_struct(m68000_bchg_s_pd7 , 16, 0xffff, 0x0867),
 new opcode_handler_struct(m68000_bchg_s_di , 13, 0xfff8, 0x0868),
 new opcode_handler_struct(m68000_bchg_s_ix , 13, 0xfff8, 0x0870),
 new opcode_handler_struct(m68000_bchg_s_aw , 16, 0xffff, 0x0878),
 new opcode_handler_struct(m68000_bchg_s_al , 16, 0xffff, 0x0879),
 new opcode_handler_struct(m68000_bclr_r_d , 10, 0xf1f8, 0x0180),
 new opcode_handler_struct(m68000_bclr_r_ai , 10, 0xf1f8, 0x0190),
 new opcode_handler_struct(m68000_bclr_r_pi , 10, 0xf1f8, 0x0198),
 new opcode_handler_struct(m68000_bclr_r_pi7 , 13, 0xf1ff, 0x019f),
 new opcode_handler_struct(m68000_bclr_r_pd , 10, 0xf1f8, 0x01a0),
 new opcode_handler_struct(m68000_bclr_r_pd7 , 13, 0xf1ff, 0x01a7),
 new opcode_handler_struct(m68000_bclr_r_di , 10, 0xf1f8, 0x01a8),
 new opcode_handler_struct(m68000_bclr_r_ix , 10, 0xf1f8, 0x01b0),
 new opcode_handler_struct(m68000_bclr_r_aw , 13, 0xf1ff, 0x01b8),
 new opcode_handler_struct(m68000_bclr_r_al , 13, 0xf1ff, 0x01b9),
 new opcode_handler_struct(m68000_bclr_s_d , 13, 0xfff8, 0x0880),
 new opcode_handler_struct(m68000_bclr_s_ai , 13, 0xfff8, 0x0890),
 new opcode_handler_struct(m68000_bclr_s_pi , 13, 0xfff8, 0x0898),
 new opcode_handler_struct(m68000_bclr_s_pi7 , 16, 0xffff, 0x089f),
 new opcode_handler_struct(m68000_bclr_s_pd , 13, 0xfff8, 0x08a0),
 new opcode_handler_struct(m68000_bclr_s_pd7 , 16, 0xffff, 0x08a7),
 new opcode_handler_struct(m68000_bclr_s_di , 13, 0xfff8, 0x08a8),
 new opcode_handler_struct(m68000_bclr_s_ix , 13, 0xfff8, 0x08b0),
 new opcode_handler_struct(m68000_bclr_s_aw , 16, 0xffff, 0x08b8),
 new opcode_handler_struct(m68000_bclr_s_al , 16, 0xffff, 0x08b9),
 new opcode_handler_struct(m68020_bfchg_d , 13, 0xfff8, 0xeac0),
 new opcode_handler_struct(m68020_bfchg_ai , 13, 0xfff8, 0xead0),
 new opcode_handler_struct(m68020_bfchg_di , 13, 0xfff8, 0xeae8),
 new opcode_handler_struct(m68020_bfchg_ix , 13, 0xfff8, 0xeaf0),
 new opcode_handler_struct(m68020_bfchg_aw , 16, 0xffff, 0xeaf8),
 new opcode_handler_struct(m68020_bfchg_al , 16, 0xffff, 0xeaf9),
 new opcode_handler_struct(m68020_bfclr_d , 13, 0xfff8, 0xecc0),
 new opcode_handler_struct(m68020_bfclr_ai , 13, 0xfff8, 0xecd0),
 new opcode_handler_struct(m68020_bfclr_di , 13, 0xfff8, 0xece8),
 new opcode_handler_struct(m68020_bfclr_ix , 13, 0xfff8, 0xecf0),
 new opcode_handler_struct(m68020_bfclr_aw , 16, 0xffff, 0xecf8),
 new opcode_handler_struct(m68020_bfclr_al , 16, 0xffff, 0xecf9),
 new opcode_handler_struct(m68020_bfexts_d , 13, 0xfff8, 0xebc0),
 new opcode_handler_struct(m68020_bfexts_ai , 13, 0xfff8, 0xebd0),
 new opcode_handler_struct(m68020_bfexts_di , 13, 0xfff8, 0xebe8),
 new opcode_handler_struct(m68020_bfexts_ix , 13, 0xfff8, 0xebf0),
 new opcode_handler_struct(m68020_bfexts_aw , 16, 0xffff, 0xebf8),
 new opcode_handler_struct(m68020_bfexts_al , 16, 0xffff, 0xebf9),
 new opcode_handler_struct(m68020_bfexts_pcdi , 16, 0xffff, 0xebfa),
 new opcode_handler_struct(m68020_bfexts_pcix , 16, 0xffff, 0xebfb),
 new opcode_handler_struct(m68020_bfextu_d , 13, 0xfff8, 0xe9c0),
 new opcode_handler_struct(m68020_bfextu_ai , 13, 0xfff8, 0xe9d0),
 new opcode_handler_struct(m68020_bfextu_di , 13, 0xfff8, 0xe9e8),
 new opcode_handler_struct(m68020_bfextu_ix , 13, 0xfff8, 0xe9f0),
 new opcode_handler_struct(m68020_bfextu_aw , 16, 0xffff, 0xe9f8),
 new opcode_handler_struct(m68020_bfextu_al , 16, 0xffff, 0xe9f9),
 new opcode_handler_struct(m68020_bfextu_pcdi , 16, 0xffff, 0xe9fa),
 new opcode_handler_struct(m68020_bfextu_pcix , 16, 0xffff, 0xe9fb),
 new opcode_handler_struct(m68020_bfffo_d , 13, 0xfff8, 0xedc0),
 new opcode_handler_struct(m68020_bfffo_ai , 13, 0xfff8, 0xedd0),
 new opcode_handler_struct(m68020_bfffo_di , 13, 0xfff8, 0xede8),
 new opcode_handler_struct(m68020_bfffo_ix , 13, 0xfff8, 0xedf0),
 new opcode_handler_struct(m68020_bfffo_aw , 16, 0xffff, 0xedf8),
 new opcode_handler_struct(m68020_bfffo_al , 16, 0xffff, 0xedf9),
 new opcode_handler_struct(m68020_bfffo_pcdi , 16, 0xffff, 0xedfa),
 new opcode_handler_struct(m68020_bfffo_pcix , 16, 0xffff, 0xedfb),
 new opcode_handler_struct(m68020_bfins_d , 13, 0xfff8, 0xefc0),
 new opcode_handler_struct(m68020_bfins_ai , 13, 0xfff8, 0xefd0),
 new opcode_handler_struct(m68020_bfins_di , 13, 0xfff8, 0xefe8),
 new opcode_handler_struct(m68020_bfins_ix , 13, 0xfff8, 0xeff0),
 new opcode_handler_struct(m68020_bfins_aw , 16, 0xffff, 0xeff8),
 new opcode_handler_struct(m68020_bfins_al , 16, 0xffff, 0xeff9),
 new opcode_handler_struct(m68020_bfset_d , 13, 0xfff8, 0xeec0),
 new opcode_handler_struct(m68020_bfset_ai , 13, 0xfff8, 0xeed0),
 new opcode_handler_struct(m68020_bfset_di , 13, 0xfff8, 0xeee8),
 new opcode_handler_struct(m68020_bfset_ix , 13, 0xfff8, 0xeef0),
 new opcode_handler_struct(m68020_bfset_aw , 16, 0xffff, 0xeef8),
 new opcode_handler_struct(m68020_bfset_al , 16, 0xffff, 0xeef9),
 new opcode_handler_struct(m68020_bftst_d , 13, 0xfff8, 0xe8c0),
 new opcode_handler_struct(m68020_bftst_ai , 13, 0xfff8, 0xe8d0),
 new opcode_handler_struct(m68020_bftst_di , 13, 0xfff8, 0xe8e8),
 new opcode_handler_struct(m68020_bftst_ix , 13, 0xfff8, 0xe8f0),
 new opcode_handler_struct(m68020_bftst_aw , 16, 0xffff, 0xe8f8),
 new opcode_handler_struct(m68020_bftst_al , 16, 0xffff, 0xe8f9),
 new opcode_handler_struct(m68020_bftst_pcdi , 16, 0xffff, 0xe8fa),
 new opcode_handler_struct(m68020_bftst_pcix , 16, 0xffff, 0xe8fb),
 new opcode_handler_struct(m68010_bkpt , 13, 0xfff8, 0x4848),
 new opcode_handler_struct(m68000_bra_16 , 16, 0xffff, 0x6000),
 new opcode_handler_struct(m68020_bra_32 , 16, 0xffff, 0x60ff),
 new opcode_handler_struct(m68000_bra_8 , 8, 0xff00, 0x6000),
 new opcode_handler_struct(m68000_bset_r_d , 10, 0xf1f8, 0x01c0),
 new opcode_handler_struct(m68000_bset_r_ai , 10, 0xf1f8, 0x01d0),
 new opcode_handler_struct(m68000_bset_r_pi , 10, 0xf1f8, 0x01d8),
 new opcode_handler_struct(m68000_bset_r_pi7 , 13, 0xf1ff, 0x01df),
 new opcode_handler_struct(m68000_bset_r_pd , 10, 0xf1f8, 0x01e0),
 new opcode_handler_struct(m68000_bset_r_pd7 , 13, 0xf1ff, 0x01e7),
 new opcode_handler_struct(m68000_bset_r_di , 10, 0xf1f8, 0x01e8),
 new opcode_handler_struct(m68000_bset_r_ix , 10, 0xf1f8, 0x01f0),
 new opcode_handler_struct(m68000_bset_r_aw , 13, 0xf1ff, 0x01f8),
 new opcode_handler_struct(m68000_bset_r_al , 13, 0xf1ff, 0x01f9),
 new opcode_handler_struct(m68000_bset_s_d , 13, 0xfff8, 0x08c0),
 new opcode_handler_struct(m68000_bset_s_ai , 13, 0xfff8, 0x08d0),
 new opcode_handler_struct(m68000_bset_s_pi , 13, 0xfff8, 0x08d8),
 new opcode_handler_struct(m68000_bset_s_pi7 , 16, 0xffff, 0x08df),
 new opcode_handler_struct(m68000_bset_s_pd , 13, 0xfff8, 0x08e0),
 new opcode_handler_struct(m68000_bset_s_pd7 , 16, 0xffff, 0x08e7),
 new opcode_handler_struct(m68000_bset_s_di , 13, 0xfff8, 0x08e8),
 new opcode_handler_struct(m68000_bset_s_ix , 13, 0xfff8, 0x08f0),
 new opcode_handler_struct(m68000_bset_s_aw , 16, 0xffff, 0x08f8),
 new opcode_handler_struct(m68000_bset_s_al , 16, 0xffff, 0x08f9),
 new opcode_handler_struct(m68000_bsr_16 , 16, 0xffff, 0x6100),
 new opcode_handler_struct(m68020_bsr_32 , 16, 0xffff, 0x61ff),
 new opcode_handler_struct(m68000_bsr_8 , 8, 0xff00, 0x6100),
 new opcode_handler_struct(m68000_btst_r_d , 10, 0xf1f8, 0x0100),
 new opcode_handler_struct(m68000_btst_r_ai , 10, 0xf1f8, 0x0110),
 new opcode_handler_struct(m68000_btst_r_pi , 10, 0xf1f8, 0x0118),
 new opcode_handler_struct(m68000_btst_r_pi7 , 13, 0xf1ff, 0x011f),
 new opcode_handler_struct(m68000_btst_r_pd , 10, 0xf1f8, 0x0120),
 new opcode_handler_struct(m68000_btst_r_pd7 , 13, 0xf1ff, 0x0127),
 new opcode_handler_struct(m68000_btst_r_di , 10, 0xf1f8, 0x0128),
 new opcode_handler_struct(m68000_btst_r_ix , 10, 0xf1f8, 0x0130),
 new opcode_handler_struct(m68000_btst_r_aw , 13, 0xf1ff, 0x0138),
 new opcode_handler_struct(m68000_btst_r_al , 13, 0xf1ff, 0x0139),
 new opcode_handler_struct(m68000_btst_r_pcdi , 13, 0xf1ff, 0x013a),
 new opcode_handler_struct(m68000_btst_r_pcix , 13, 0xf1ff, 0x013b),
 new opcode_handler_struct(m68000_btst_r_i , 13, 0xf1ff, 0x013c),
 new opcode_handler_struct(m68000_btst_s_d , 13, 0xfff8, 0x0800),
 new opcode_handler_struct(m68000_btst_s_ai , 13, 0xfff8, 0x0810),
 new opcode_handler_struct(m68000_btst_s_pi , 13, 0xfff8, 0x0818),
 new opcode_handler_struct(m68000_btst_s_pi7 , 16, 0xffff, 0x081f),
 new opcode_handler_struct(m68000_btst_s_pd , 13, 0xfff8, 0x0820),
 new opcode_handler_struct(m68000_btst_s_pd7 , 16, 0xffff, 0x0827),
 new opcode_handler_struct(m68000_btst_s_di , 13, 0xfff8, 0x0828),
 new opcode_handler_struct(m68000_btst_s_ix , 13, 0xfff8, 0x0830),
 new opcode_handler_struct(m68000_btst_s_aw , 16, 0xffff, 0x0838),
 new opcode_handler_struct(m68000_btst_s_al , 16, 0xffff, 0x0839),
 new opcode_handler_struct(m68000_btst_s_pcdi , 16, 0xffff, 0x083a),
 new opcode_handler_struct(m68000_btst_s_pcix , 16, 0xffff, 0x083b),
 new opcode_handler_struct(m68020_callm_ai , 13, 0xfff8, 0x06d0),
 new opcode_handler_struct(m68020_callm_di , 13, 0xfff8, 0x06e8),
 new opcode_handler_struct(m68020_callm_ix , 13, 0xfff8, 0x06f0),
 new opcode_handler_struct(m68020_callm_aw , 16, 0xffff, 0x06f8),
 new opcode_handler_struct(m68020_callm_al , 16, 0xffff, 0x06f9),
 new opcode_handler_struct(m68020_callm_pcdi , 16, 0xffff, 0x06fa),
 new opcode_handler_struct(m68020_callm_pcix , 16, 0xffff, 0x06fb),
 new opcode_handler_struct(m68020_cas_ai_8 , 13, 0xfff8, 0x0ad0),
 new opcode_handler_struct(m68020_cas_pi_8 , 13, 0xfff8, 0x0ad8),
 new opcode_handler_struct(m68020_cas_pi7_8 , 16, 0xffff, 0x0adf),
 new opcode_handler_struct(m68020_cas_pd_8 , 13, 0xfff8, 0x0ae0),
 new opcode_handler_struct(m68020_cas_pd7_8 , 16, 0xffff, 0x0ae7),
 new opcode_handler_struct(m68020_cas_di_8 , 13, 0xfff8, 0x0ae8),
 new opcode_handler_struct(m68020_cas_ix_8 , 13, 0xfff8, 0x0af0),
 new opcode_handler_struct(m68020_cas_aw_8 , 16, 0xffff, 0x0af8),
 new opcode_handler_struct(m68020_cas_al_8 , 16, 0xffff, 0x0af9),
 new opcode_handler_struct(m68020_cas_ai_16 , 13, 0xfff8, 0x0cd0),
 new opcode_handler_struct(m68020_cas_pi_16 , 13, 0xfff8, 0x0cd8),
 new opcode_handler_struct(m68020_cas_pd_16 , 13, 0xfff8, 0x0ce0),
 new opcode_handler_struct(m68020_cas_di_16 , 13, 0xfff8, 0x0ce8),
 new opcode_handler_struct(m68020_cas_ix_16 , 13, 0xfff8, 0x0cf0),
 new opcode_handler_struct(m68020_cas_aw_16 , 16, 0xffff, 0x0cf8),
 new opcode_handler_struct(m68020_cas_al_16 , 16, 0xffff, 0x0cf9),
 new opcode_handler_struct(m68020_cas_ai_32 , 13, 0xfff8, 0x0ed0),
 new opcode_handler_struct(m68020_cas_pi_32 , 13, 0xfff8, 0x0ed8),
 new opcode_handler_struct(m68020_cas_pd_32 , 13, 0xfff8, 0x0ee0),
 new opcode_handler_struct(m68020_cas_di_32 , 13, 0xfff8, 0x0ee8),
 new opcode_handler_struct(m68020_cas_ix_32 , 13, 0xfff8, 0x0ef0),
 new opcode_handler_struct(m68020_cas_aw_32 , 16, 0xffff, 0x0ef8),
 new opcode_handler_struct(m68020_cas_al_32 , 16, 0xffff, 0x0ef9),
 new opcode_handler_struct(m68020_cas2_16 , 16, 0xffff, 0x0cfc),
 new opcode_handler_struct(m68020_cas2_32 , 16, 0xffff, 0x0efc),
 new opcode_handler_struct(m68000_chk_d_16 , 10, 0xf1f8, 0x4180),
 new opcode_handler_struct(m68000_chk_ai_16 , 10, 0xf1f8, 0x4190),
 new opcode_handler_struct(m68000_chk_pi_16 , 10, 0xf1f8, 0x4198),
 new opcode_handler_struct(m68000_chk_pd_16 , 10, 0xf1f8, 0x41a0),
 new opcode_handler_struct(m68000_chk_di_16 , 10, 0xf1f8, 0x41a8),
 new opcode_handler_struct(m68000_chk_ix_16 , 10, 0xf1f8, 0x41b0),
 new opcode_handler_struct(m68000_chk_aw_16 , 13, 0xf1ff, 0x41b8),
 new opcode_handler_struct(m68000_chk_al_16 , 13, 0xf1ff, 0x41b9),
 new opcode_handler_struct(m68000_chk_pcdi_16 , 13, 0xf1ff, 0x41ba),
 new opcode_handler_struct(m68000_chk_pcix_16 , 13, 0xf1ff, 0x41bb),
 new opcode_handler_struct(m68000_chk_i_16 , 13, 0xf1ff, 0x41bc),
 new opcode_handler_struct(m68020_chk_d_32 , 10, 0xf1f8, 0x4100),
 new opcode_handler_struct(m68020_chk_ai_32 , 10, 0xf1f8, 0x4110),
 new opcode_handler_struct(m68020_chk_pi_32 , 10, 0xf1f8, 0x4118),
 new opcode_handler_struct(m68020_chk_pd_32 , 10, 0xf1f8, 0x4120),
 new opcode_handler_struct(m68020_chk_di_32 , 10, 0xf1f8, 0x4128),
 new opcode_handler_struct(m68020_chk_ix_32 , 10, 0xf1f8, 0x4130),
 new opcode_handler_struct(m68020_chk_aw_32 , 13, 0xf1ff, 0x4138),
 new opcode_handler_struct(m68020_chk_al_32 , 13, 0xf1ff, 0x4139),
 new opcode_handler_struct(m68020_chk_pcdi_32 , 13, 0xf1ff, 0x413a),
 new opcode_handler_struct(m68020_chk_pcix_32 , 13, 0xf1ff, 0x413b),
 new opcode_handler_struct(m68020_chk_i_32 , 13, 0xf1ff, 0x413c),
 new opcode_handler_struct(m68020_chk2_cmp2_ai_8 , 13, 0xfff8, 0x00d0),
 new opcode_handler_struct(m68020_chk2_cmp2_di_8 , 13, 0xfff8, 0x00e8),
 new opcode_handler_struct(m68020_chk2_cmp2_ix_8 , 13, 0xfff8, 0x00f0),
 new opcode_handler_struct(m68020_chk2_cmp2_aw_8 , 16, 0xffff, 0x00f8),
 new opcode_handler_struct(m68020_chk2_cmp2_al_8 , 16, 0xffff, 0x00f9),
 new opcode_handler_struct(m68020_chk2_cmp2_pcdi_8 , 16, 0xffff, 0x00fa),
 new opcode_handler_struct(m68020_chk2_cmp2_pcix_8 , 16, 0xffff, 0x00fb),
 new opcode_handler_struct(m68020_chk2_cmp2_ai_16 , 13, 0xfff8, 0x02d0),
 new opcode_handler_struct(m68020_chk2_cmp2_di_16 , 13, 0xfff8, 0x02e8),
 new opcode_handler_struct(m68020_chk2_cmp2_ix_16 , 13, 0xfff8, 0x02f0),
 new opcode_handler_struct(m68020_chk2_cmp2_aw_16 , 16, 0xffff, 0x02f8),
 new opcode_handler_struct(m68020_chk2_cmp2_al_16 , 16, 0xffff, 0x02f9),
 new opcode_handler_struct(m68020_chk2_cmp2_pcdi_16, 16, 0xffff, 0x02fa),
 new opcode_handler_struct(m68020_chk2_cmp2_pcix_16, 16, 0xffff, 0x02fb),
 new opcode_handler_struct(m68020_chk2_cmp2_ai_32 , 13, 0xfff8, 0x04d0),
 new opcode_handler_struct(m68020_chk2_cmp2_di_32 , 13, 0xfff8, 0x04e8),
 new opcode_handler_struct(m68020_chk2_cmp2_ix_32 , 13, 0xfff8, 0x04f0),
 new opcode_handler_struct(m68020_chk2_cmp2_aw_32 , 16, 0xffff, 0x04f8),
 new opcode_handler_struct(m68020_chk2_cmp2_al_32 , 16, 0xffff, 0x04f9),
 new opcode_handler_struct(m68020_chk2_cmp2_pcdi_32, 16, 0xffff, 0x04fa),
 new opcode_handler_struct(m68020_chk2_cmp2_pcix_32, 16, 0xffff, 0x04fb),
 new opcode_handler_struct(m68000_clr_d_8 , 13, 0xfff8, 0x4200),
 new opcode_handler_struct(m68000_clr_ai_8 , 13, 0xfff8, 0x4210),
 new opcode_handler_struct(m68000_clr_pi_8 , 13, 0xfff8, 0x4218),
 new opcode_handler_struct(m68000_clr_pi7_8 , 16, 0xffff, 0x421f),
 new opcode_handler_struct(m68000_clr_pd_8 , 13, 0xfff8, 0x4220),
 new opcode_handler_struct(m68000_clr_pd7_8 , 16, 0xffff, 0x4227),
 new opcode_handler_struct(m68000_clr_di_8 , 13, 0xfff8, 0x4228),
 new opcode_handler_struct(m68000_clr_ix_8 , 13, 0xfff8, 0x4230),
 new opcode_handler_struct(m68000_clr_aw_8 , 16, 0xffff, 0x4238),
 new opcode_handler_struct(m68000_clr_al_8 , 16, 0xffff, 0x4239),
 new opcode_handler_struct(m68000_clr_d_16 , 13, 0xfff8, 0x4240),
 new opcode_handler_struct(m68000_clr_ai_16 , 13, 0xfff8, 0x4250),
 new opcode_handler_struct(m68000_clr_pi_16 , 13, 0xfff8, 0x4258),
 new opcode_handler_struct(m68000_clr_pd_16 , 13, 0xfff8, 0x4260),
 new opcode_handler_struct(m68000_clr_di_16 , 13, 0xfff8, 0x4268),
 new opcode_handler_struct(m68000_clr_ix_16 , 13, 0xfff8, 0x4270),
 new opcode_handler_struct(m68000_clr_aw_16 , 16, 0xffff, 0x4278),
 new opcode_handler_struct(m68000_clr_al_16 , 16, 0xffff, 0x4279),
 new opcode_handler_struct(m68000_clr_d_32 , 13, 0xfff8, 0x4280),
 new opcode_handler_struct(m68000_clr_ai_32 , 13, 0xfff8, 0x4290),
 new opcode_handler_struct(m68000_clr_pi_32 , 13, 0xfff8, 0x4298),
 new opcode_handler_struct(m68000_clr_pd_32 , 13, 0xfff8, 0x42a0),
 new opcode_handler_struct(m68000_clr_di_32 , 13, 0xfff8, 0x42a8),
 new opcode_handler_struct(m68000_clr_ix_32 , 13, 0xfff8, 0x42b0),
 new opcode_handler_struct(m68000_clr_aw_32 , 16, 0xffff, 0x42b8),
 new opcode_handler_struct(m68000_clr_al_32 , 16, 0xffff, 0x42b9),
 new opcode_handler_struct(m68000_cmp_d_8 , 10, 0xf1f8, 0xb000),
 new opcode_handler_struct(m68000_cmp_ai_8 , 10, 0xf1f8, 0xb010),
 new opcode_handler_struct(m68000_cmp_pi_8 , 10, 0xf1f8, 0xb018),
 new opcode_handler_struct(m68000_cmp_pi7_8 , 13, 0xf1ff, 0xb01f),
 new opcode_handler_struct(m68000_cmp_pd_8 , 10, 0xf1f8, 0xb020),
 new opcode_handler_struct(m68000_cmp_pd7_8 , 13, 0xf1ff, 0xb027),
 new opcode_handler_struct(m68000_cmp_di_8 , 10, 0xf1f8, 0xb028),
 new opcode_handler_struct(m68000_cmp_ix_8 , 10, 0xf1f8, 0xb030),
 new opcode_handler_struct(m68000_cmp_aw_8 , 13, 0xf1ff, 0xb038),
 new opcode_handler_struct(m68000_cmp_al_8 , 13, 0xf1ff, 0xb039),
 new opcode_handler_struct(m68000_cmp_pcdi_8 , 13, 0xf1ff, 0xb03a),
 new opcode_handler_struct(m68000_cmp_pcix_8 , 13, 0xf1ff, 0xb03b),
 new opcode_handler_struct(m68000_cmp_i_8 , 13, 0xf1ff, 0xb03c),
 new opcode_handler_struct(m68000_cmp_d_16 , 10, 0xf1f8, 0xb040),
 new opcode_handler_struct(m68000_cmp_a_16 , 10, 0xf1f8, 0xb048),
 new opcode_handler_struct(m68000_cmp_ai_16 , 10, 0xf1f8, 0xb050),
 new opcode_handler_struct(m68000_cmp_pi_16 , 10, 0xf1f8, 0xb058),
 new opcode_handler_struct(m68000_cmp_pd_16 , 10, 0xf1f8, 0xb060),
 new opcode_handler_struct(m68000_cmp_di_16 , 10, 0xf1f8, 0xb068),
 new opcode_handler_struct(m68000_cmp_ix_16 , 10, 0xf1f8, 0xb070),
 new opcode_handler_struct(m68000_cmp_aw_16 , 13, 0xf1ff, 0xb078),
 new opcode_handler_struct(m68000_cmp_al_16 , 13, 0xf1ff, 0xb079),
 new opcode_handler_struct(m68000_cmp_pcdi_16 , 13, 0xf1ff, 0xb07a),
 new opcode_handler_struct(m68000_cmp_pcix_16 , 13, 0xf1ff, 0xb07b),
 new opcode_handler_struct(m68000_cmp_i_16 , 13, 0xf1ff, 0xb07c),
 new opcode_handler_struct(m68000_cmp_d_32 , 10, 0xf1f8, 0xb080),
 new opcode_handler_struct(m68000_cmp_a_32 , 10, 0xf1f8, 0xb088),
 new opcode_handler_struct(m68000_cmp_ai_32 , 10, 0xf1f8, 0xb090),
 new opcode_handler_struct(m68000_cmp_pi_32 , 10, 0xf1f8, 0xb098),
 new opcode_handler_struct(m68000_cmp_pd_32 , 10, 0xf1f8, 0xb0a0),
 new opcode_handler_struct(m68000_cmp_di_32 , 10, 0xf1f8, 0xb0a8),
 new opcode_handler_struct(m68000_cmp_ix_32 , 10, 0xf1f8, 0xb0b0),
 new opcode_handler_struct(m68000_cmp_aw_32 , 13, 0xf1ff, 0xb0b8),
 new opcode_handler_struct(m68000_cmp_al_32 , 13, 0xf1ff, 0xb0b9),
 new opcode_handler_struct(m68000_cmp_pcdi_32 , 13, 0xf1ff, 0xb0ba),
 new opcode_handler_struct(m68000_cmp_pcix_32 , 13, 0xf1ff, 0xb0bb),
 new opcode_handler_struct(m68000_cmp_i_32 , 13, 0xf1ff, 0xb0bc),
 new opcode_handler_struct(m68000_cmpa_d_16 , 10, 0xf1f8, 0xb0c0),
 new opcode_handler_struct(m68000_cmpa_a_16 , 10, 0xf1f8, 0xb0c8),
 new opcode_handler_struct(m68000_cmpa_ai_16 , 10, 0xf1f8, 0xb0d0),
 new opcode_handler_struct(m68000_cmpa_pi_16 , 10, 0xf1f8, 0xb0d8),
 new opcode_handler_struct(m68000_cmpa_pd_16 , 10, 0xf1f8, 0xb0e0),
 new opcode_handler_struct(m68000_cmpa_di_16 , 10, 0xf1f8, 0xb0e8),
 new opcode_handler_struct(m68000_cmpa_ix_16 , 10, 0xf1f8, 0xb0f0),
 new opcode_handler_struct(m68000_cmpa_aw_16 , 13, 0xf1ff, 0xb0f8),
 new opcode_handler_struct(m68000_cmpa_al_16 , 13, 0xf1ff, 0xb0f9),
 new opcode_handler_struct(m68000_cmpa_pcdi_16 , 13, 0xf1ff, 0xb0fa),
 new opcode_handler_struct(m68000_cmpa_pcix_16 , 13, 0xf1ff, 0xb0fb),
 new opcode_handler_struct(m68000_cmpa_i_16 , 13, 0xf1ff, 0xb0fc),
 new opcode_handler_struct(m68000_cmpa_d_32 , 10, 0xf1f8, 0xb1c0),
 new opcode_handler_struct(m68000_cmpa_a_32 , 10, 0xf1f8, 0xb1c8),
 new opcode_handler_struct(m68000_cmpa_ai_32 , 10, 0xf1f8, 0xb1d0),
 new opcode_handler_struct(m68000_cmpa_pi_32 , 10, 0xf1f8, 0xb1d8),
 new opcode_handler_struct(m68000_cmpa_pd_32 , 10, 0xf1f8, 0xb1e0),
 new opcode_handler_struct(m68000_cmpa_di_32 , 10, 0xf1f8, 0xb1e8),
 new opcode_handler_struct(m68000_cmpa_ix_32 , 10, 0xf1f8, 0xb1f0),
 new opcode_handler_struct(m68000_cmpa_aw_32 , 13, 0xf1ff, 0xb1f8),
 new opcode_handler_struct(m68000_cmpa_al_32 , 13, 0xf1ff, 0xb1f9),
 new opcode_handler_struct(m68000_cmpa_pcdi_32 , 13, 0xf1ff, 0xb1fa),
 new opcode_handler_struct(m68000_cmpa_pcix_32 , 13, 0xf1ff, 0xb1fb),
 new opcode_handler_struct(m68000_cmpa_i_32 , 13, 0xf1ff, 0xb1fc),
 new opcode_handler_struct(m68000_cmpi_d_8 , 13, 0xfff8, 0x0c00),
 new opcode_handler_struct(m68000_cmpi_ai_8 , 13, 0xfff8, 0x0c10),
 new opcode_handler_struct(m68000_cmpi_pi_8 , 13, 0xfff8, 0x0c18),
 new opcode_handler_struct(m68000_cmpi_pi7_8 , 16, 0xffff, 0x0c1f),
 new opcode_handler_struct(m68000_cmpi_pd_8 , 13, 0xfff8, 0x0c20),
 new opcode_handler_struct(m68000_cmpi_pd7_8 , 16, 0xffff, 0x0c27),
 new opcode_handler_struct(m68000_cmpi_di_8 , 13, 0xfff8, 0x0c28),
 new opcode_handler_struct(m68000_cmpi_ix_8 , 13, 0xfff8, 0x0c30),
 new opcode_handler_struct(m68000_cmpi_aw_8 , 16, 0xffff, 0x0c38),
 new opcode_handler_struct(m68000_cmpi_al_8 , 16, 0xffff, 0x0c39),
 new opcode_handler_struct(m68020_cmpi_pcdi_8 , 16, 0xffff, 0x0c3a),
 new opcode_handler_struct(m68020_cmpi_pcix_8 , 16, 0xffff, 0x0c3b),
 new opcode_handler_struct(m68000_cmpi_d_16 , 13, 0xfff8, 0x0c40),
 new opcode_handler_struct(m68000_cmpi_ai_16 , 13, 0xfff8, 0x0c50),
 new opcode_handler_struct(m68000_cmpi_pi_16 , 13, 0xfff8, 0x0c58),
 new opcode_handler_struct(m68000_cmpi_pd_16 , 13, 0xfff8, 0x0c60),
 new opcode_handler_struct(m68000_cmpi_di_16 , 13, 0xfff8, 0x0c68),
 new opcode_handler_struct(m68000_cmpi_ix_16 , 13, 0xfff8, 0x0c70),
 new opcode_handler_struct(m68000_cmpi_aw_16 , 16, 0xffff, 0x0c78),
 new opcode_handler_struct(m68000_cmpi_al_16 , 16, 0xffff, 0x0c79),
 new opcode_handler_struct(m68020_cmpi_pcdi_16 , 16, 0xffff, 0x0c7a),
 new opcode_handler_struct(m68020_cmpi_pcix_16 , 16, 0xffff, 0x0c7b),
 new opcode_handler_struct(m68000_cmpi_d_32 , 13, 0xfff8, 0x0c80),
 new opcode_handler_struct(m68000_cmpi_ai_32 , 13, 0xfff8, 0x0c90),
 new opcode_handler_struct(m68000_cmpi_pi_32 , 13, 0xfff8, 0x0c98),
 new opcode_handler_struct(m68000_cmpi_pd_32 , 13, 0xfff8, 0x0ca0),
 new opcode_handler_struct(m68000_cmpi_di_32 , 13, 0xfff8, 0x0ca8),
 new opcode_handler_struct(m68000_cmpi_ix_32 , 13, 0xfff8, 0x0cb0),
 new opcode_handler_struct(m68000_cmpi_aw_32 , 16, 0xffff, 0x0cb8),
 new opcode_handler_struct(m68000_cmpi_al_32 , 16, 0xffff, 0x0cb9),
 new opcode_handler_struct(m68020_cmpi_pcdi_32 , 16, 0xffff, 0x0cba),
 new opcode_handler_struct(m68020_cmpi_pcix_32 , 16, 0xffff, 0x0cbb),
 new opcode_handler_struct(m68000_cmpm_8_ax7 , 13, 0xfff8, 0xbf08),
 new opcode_handler_struct(m68000_cmpm_8_ay7 , 13, 0xf1ff, 0xb10f),
 new opcode_handler_struct(m68000_cmpm_8_axy7 , 16, 0xffff, 0xbf0f),
 new opcode_handler_struct(m68000_cmpm_8 , 10, 0xf1f8, 0xb108),
 new opcode_handler_struct(m68000_cmpm_16 , 10, 0xf1f8, 0xb148),
 new opcode_handler_struct(m68000_cmpm_32 , 10, 0xf1f8, 0xb188),
 new opcode_handler_struct(m68020_cpbcc , 6, 0xf180, 0xf080),
 new opcode_handler_struct(m68020_cpdbcc , 10, 0xf1f8, 0xf048),
 new opcode_handler_struct(m68020_cpgen , 7, 0xf1c0, 0xf000),
 new opcode_handler_struct(m68020_cpscc , 7, 0xf1c0, 0xf040),
 new opcode_handler_struct(m68020_cptrapcc , 10, 0xf1f8, 0xf078),
 new opcode_handler_struct(m68000_dbt , 13, 0xfff8, 0x50c8),
 new opcode_handler_struct(m68000_dbf , 13, 0xfff8, 0x51c8),
 new opcode_handler_struct(m68000_dbhi , 13, 0xfff8, 0x52c8),
 new opcode_handler_struct(m68000_dbls , 13, 0xfff8, 0x53c8),
 new opcode_handler_struct(m68000_dbcc , 13, 0xfff8, 0x54c8),
 new opcode_handler_struct(m68000_dbcs , 13, 0xfff8, 0x55c8),
 new opcode_handler_struct(m68000_dbne , 13, 0xfff8, 0x56c8),
 new opcode_handler_struct(m68000_dbeq , 13, 0xfff8, 0x57c8),
 new opcode_handler_struct(m68000_dbvc , 13, 0xfff8, 0x58c8),
 new opcode_handler_struct(m68000_dbvs , 13, 0xfff8, 0x59c8),
 new opcode_handler_struct(m68000_dbpl , 13, 0xfff8, 0x5ac8),
 new opcode_handler_struct(m68000_dbmi , 13, 0xfff8, 0x5bc8),
 new opcode_handler_struct(m68000_dbge , 13, 0xfff8, 0x5cc8),
 new opcode_handler_struct(m68000_dblt , 13, 0xfff8, 0x5dc8),
 new opcode_handler_struct(m68000_dbgt , 13, 0xfff8, 0x5ec8),
 new opcode_handler_struct(m68000_dble , 13, 0xfff8, 0x5fc8),
 new opcode_handler_struct(m68000_divs_d_16 , 10, 0xf1f8, 0x81c0),
 new opcode_handler_struct(m68000_divs_ai_16 , 10, 0xf1f8, 0x81d0),
 new opcode_handler_struct(m68000_divs_pi_16 , 10, 0xf1f8, 0x81d8),
 new opcode_handler_struct(m68000_divs_pd_16 , 10, 0xf1f8, 0x81e0),
 new opcode_handler_struct(m68000_divs_di_16 , 10, 0xf1f8, 0x81e8),
 new opcode_handler_struct(m68000_divs_ix_16 , 10, 0xf1f8, 0x81f0),
 new opcode_handler_struct(m68000_divs_aw_16 , 13, 0xf1ff, 0x81f8),
 new opcode_handler_struct(m68000_divs_al_16 , 13, 0xf1ff, 0x81f9),
 new opcode_handler_struct(m68000_divs_pcdi_16 , 13, 0xf1ff, 0x81fa),
 new opcode_handler_struct(m68000_divs_pcix_16 , 13, 0xf1ff, 0x81fb),
 new opcode_handler_struct(m68000_divs_i_16 , 13, 0xf1ff, 0x81fc),
 new opcode_handler_struct(m68000_divu_d_16 , 10, 0xf1f8, 0x80c0),
 new opcode_handler_struct(m68000_divu_ai_16 , 10, 0xf1f8, 0x80d0),
 new opcode_handler_struct(m68000_divu_pi_16 , 10, 0xf1f8, 0x80d8),
 new opcode_handler_struct(m68000_divu_pd_16 , 10, 0xf1f8, 0x80e0),
 new opcode_handler_struct(m68000_divu_di_16 , 10, 0xf1f8, 0x80e8),
 new opcode_handler_struct(m68000_divu_ix_16 , 10, 0xf1f8, 0x80f0),
 new opcode_handler_struct(m68000_divu_aw_16 , 13, 0xf1ff, 0x80f8),
 new opcode_handler_struct(m68000_divu_al_16 , 13, 0xf1ff, 0x80f9),
 new opcode_handler_struct(m68000_divu_pcdi_16 , 13, 0xf1ff, 0x80fa),
 new opcode_handler_struct(m68000_divu_pcix_16 , 13, 0xf1ff, 0x80fb),
 new opcode_handler_struct(m68000_divu_i_16 , 13, 0xf1ff, 0x80fc),
 new opcode_handler_struct(m68020_divl_d_32 , 13, 0xfff8, 0x4c40),
 new opcode_handler_struct(m68020_divl_ai_32 , 13, 0xfff8, 0x4c50),
 new opcode_handler_struct(m68020_divl_pi_32 , 13, 0xfff8, 0x4c58),
 new opcode_handler_struct(m68020_divl_pd_32 , 13, 0xfff8, 0x4c60),
 new opcode_handler_struct(m68020_divl_di_32 , 13, 0xfff8, 0x4c68),
 new opcode_handler_struct(m68020_divl_ix_32 , 13, 0xfff8, 0x4c70),
 new opcode_handler_struct(m68020_divl_aw_32 , 16, 0xffff, 0x4c78),
 new opcode_handler_struct(m68020_divl_al_32 , 16, 0xffff, 0x4c79),
 new opcode_handler_struct(m68020_divl_pcdi_32 , 16, 0xffff, 0x4c7a),
 new opcode_handler_struct(m68020_divl_pcix_32 , 16, 0xffff, 0x4c7b),
 new opcode_handler_struct(m68020_divl_i_32 , 16, 0xffff, 0x4c7c),
 new opcode_handler_struct(m68000_eor_d_8 , 10, 0xf1f8, 0xb100),
 new opcode_handler_struct(m68000_eor_ai_8 , 10, 0xf1f8, 0xb110),
 new opcode_handler_struct(m68000_eor_pi_8 , 10, 0xf1f8, 0xb118),
 new opcode_handler_struct(m68000_eor_pi7_8 , 13, 0xf1ff, 0xb11f),
 new opcode_handler_struct(m68000_eor_pd_8 , 10, 0xf1f8, 0xb120),
 new opcode_handler_struct(m68000_eor_pd7_8 , 13, 0xf1ff, 0xb127),
 new opcode_handler_struct(m68000_eor_di_8 , 10, 0xf1f8, 0xb128),
 new opcode_handler_struct(m68000_eor_ix_8 , 10, 0xf1f8, 0xb130),
 new opcode_handler_struct(m68000_eor_aw_8 , 13, 0xf1ff, 0xb138),
 new opcode_handler_struct(m68000_eor_al_8 , 13, 0xf1ff, 0xb139),
 new opcode_handler_struct(m68000_eor_d_16 , 10, 0xf1f8, 0xb140),
 new opcode_handler_struct(m68000_eor_ai_16 , 10, 0xf1f8, 0xb150),
 new opcode_handler_struct(m68000_eor_pi_16 , 10, 0xf1f8, 0xb158),
 new opcode_handler_struct(m68000_eor_pd_16 , 10, 0xf1f8, 0xb160),
 new opcode_handler_struct(m68000_eor_di_16 , 10, 0xf1f8, 0xb168),
 new opcode_handler_struct(m68000_eor_ix_16 , 10, 0xf1f8, 0xb170),
 new opcode_handler_struct(m68000_eor_aw_16 , 13, 0xf1ff, 0xb178),
 new opcode_handler_struct(m68000_eor_al_16 , 13, 0xf1ff, 0xb179),
 new opcode_handler_struct(m68000_eor_d_32 , 10, 0xf1f8, 0xb180),
 new opcode_handler_struct(m68000_eor_ai_32 , 10, 0xf1f8, 0xb190),
 new opcode_handler_struct(m68000_eor_pi_32 , 10, 0xf1f8, 0xb198),
 new opcode_handler_struct(m68000_eor_pd_32 , 10, 0xf1f8, 0xb1a0),
 new opcode_handler_struct(m68000_eor_di_32 , 10, 0xf1f8, 0xb1a8),
 new opcode_handler_struct(m68000_eor_ix_32 , 10, 0xf1f8, 0xb1b0),
 new opcode_handler_struct(m68000_eor_aw_32 , 13, 0xf1ff, 0xb1b8),
 new opcode_handler_struct(m68000_eor_al_32 , 13, 0xf1ff, 0xb1b9),
 new opcode_handler_struct(m68000_eori_to_ccr , 16, 0xffff, 0x0a3c),
 new opcode_handler_struct(m68000_eori_to_sr , 16, 0xffff, 0x0a7c),
 new opcode_handler_struct(m68000_eori_d_8 , 13, 0xfff8, 0x0a00),
 new opcode_handler_struct(m68000_eori_ai_8 , 13, 0xfff8, 0x0a10),
 new opcode_handler_struct(m68000_eori_pi_8 , 13, 0xfff8, 0x0a18),
 new opcode_handler_struct(m68000_eori_pi7_8 , 16, 0xffff, 0x0a1f),
 new opcode_handler_struct(m68000_eori_pd_8 , 13, 0xfff8, 0x0a20),
 new opcode_handler_struct(m68000_eori_pd7_8 , 16, 0xffff, 0x0a27),
 new opcode_handler_struct(m68000_eori_di_8 , 13, 0xfff8, 0x0a28),
 new opcode_handler_struct(m68000_eori_ix_8 , 13, 0xfff8, 0x0a30),
 new opcode_handler_struct(m68000_eori_aw_8 , 16, 0xffff, 0x0a38),
 new opcode_handler_struct(m68000_eori_al_8 , 16, 0xffff, 0x0a39),
 new opcode_handler_struct(m68000_eori_d_16 , 13, 0xfff8, 0x0a40),
 new opcode_handler_struct(m68000_eori_ai_16 , 13, 0xfff8, 0x0a50),
 new opcode_handler_struct(m68000_eori_pi_16 , 13, 0xfff8, 0x0a58),
 new opcode_handler_struct(m68000_eori_pd_16 , 13, 0xfff8, 0x0a60),
 new opcode_handler_struct(m68000_eori_di_16 , 13, 0xfff8, 0x0a68),
 new opcode_handler_struct(m68000_eori_ix_16 , 13, 0xfff8, 0x0a70),
 new opcode_handler_struct(m68000_eori_aw_16 , 16, 0xffff, 0x0a78),
 new opcode_handler_struct(m68000_eori_al_16 , 16, 0xffff, 0x0a79),
 new opcode_handler_struct(m68000_eori_d_32 , 13, 0xfff8, 0x0a80),
 new opcode_handler_struct(m68000_eori_ai_32 , 13, 0xfff8, 0x0a90),
 new opcode_handler_struct(m68000_eori_pi_32 , 13, 0xfff8, 0x0a98),
 new opcode_handler_struct(m68000_eori_pd_32 , 13, 0xfff8, 0x0aa0),
 new opcode_handler_struct(m68000_eori_di_32 , 13, 0xfff8, 0x0aa8),
 new opcode_handler_struct(m68000_eori_ix_32 , 13, 0xfff8, 0x0ab0),
 new opcode_handler_struct(m68000_eori_aw_32 , 16, 0xffff, 0x0ab8),
 new opcode_handler_struct(m68000_eori_al_32 , 16, 0xffff, 0x0ab9),
 new opcode_handler_struct(m68000_exg_dd , 10, 0xf1f8, 0xc140),
 new opcode_handler_struct(m68000_exg_aa , 10, 0xf1f8, 0xc148),
 new opcode_handler_struct(m68000_exg_da , 10, 0xf1f8, 0xc188),
 new opcode_handler_struct(m68000_ext_16 , 13, 0xfff8, 0x4880),
 new opcode_handler_struct(m68000_ext_32 , 13, 0xfff8, 0x48c0),
 new opcode_handler_struct(m68020_extb , 13, 0xfff8, 0x49c0),
 new opcode_handler_struct(m68000_illegal , 16, 0xffff, 0x4afc),
 new opcode_handler_struct(m68000_jmp_ai , 13, 0xfff8, 0x4ed0),
 new opcode_handler_struct(m68000_jmp_di , 13, 0xfff8, 0x4ee8),
 new opcode_handler_struct(m68000_jmp_ix , 13, 0xfff8, 0x4ef0),
 new opcode_handler_struct(m68000_jmp_aw , 16, 0xffff, 0x4ef8),
 new opcode_handler_struct(m68000_jmp_al , 16, 0xffff, 0x4ef9),
 new opcode_handler_struct(m68000_jmp_pcdi , 16, 0xffff, 0x4efa),
 new opcode_handler_struct(m68000_jmp_pcix , 16, 0xffff, 0x4efb),
 new opcode_handler_struct(m68000_jsr_ai , 13, 0xfff8, 0x4e90),
 new opcode_handler_struct(m68000_jsr_di , 13, 0xfff8, 0x4ea8),
 new opcode_handler_struct(m68000_jsr_ix , 13, 0xfff8, 0x4eb0),
 new opcode_handler_struct(m68000_jsr_aw , 16, 0xffff, 0x4eb8),
 new opcode_handler_struct(m68000_jsr_al , 16, 0xffff, 0x4eb9),
 new opcode_handler_struct(m68000_jsr_pcdi , 16, 0xffff, 0x4eba),
 new opcode_handler_struct(m68000_jsr_pcix , 16, 0xffff, 0x4ebb),
 new opcode_handler_struct(m68000_lea_ai , 10, 0xf1f8, 0x41d0),
 new opcode_handler_struct(m68000_lea_di , 10, 0xf1f8, 0x41e8),
 new opcode_handler_struct(m68000_lea_ix , 10, 0xf1f8, 0x41f0),
 new opcode_handler_struct(m68000_lea_aw , 13, 0xf1ff, 0x41f8),
 new opcode_handler_struct(m68000_lea_al , 13, 0xf1ff, 0x41f9),
 new opcode_handler_struct(m68000_lea_pcdi , 13, 0xf1ff, 0x41fa),
 new opcode_handler_struct(m68000_lea_pcix , 13, 0xf1ff, 0x41fb),
 new opcode_handler_struct(m68000_link_16_a7 , 16, 0xffff, 0x4e57),
 new opcode_handler_struct(m68000_link_16 , 13, 0xfff8, 0x4e50),
 new opcode_handler_struct(m68020_link_32_a7 , 16, 0xffff, 0x480f),
 new opcode_handler_struct(m68020_link_32 , 13, 0xfff8, 0x4808),
 new opcode_handler_struct(m68000_lsr_s_8 , 10, 0xf1f8, 0xe008),
 new opcode_handler_struct(m68000_lsr_s_16 , 10, 0xf1f8, 0xe048),
 new opcode_handler_struct(m68000_lsr_s_32 , 10, 0xf1f8, 0xe088),
 new opcode_handler_struct(m68000_lsr_r_8 , 10, 0xf1f8, 0xe028),
 new opcode_handler_struct(m68000_lsr_r_16 , 10, 0xf1f8, 0xe068),
 new opcode_handler_struct(m68000_lsr_r_32 , 10, 0xf1f8, 0xe0a8),
 new opcode_handler_struct(m68000_lsr_ea_ai , 13, 0xfff8, 0xe2d0),
 new opcode_handler_struct(m68000_lsr_ea_pi , 13, 0xfff8, 0xe2d8),
 new opcode_handler_struct(m68000_lsr_ea_pd , 13, 0xfff8, 0xe2e0),
 new opcode_handler_struct(m68000_lsr_ea_di , 13, 0xfff8, 0xe2e8),
 new opcode_handler_struct(m68000_lsr_ea_ix , 13, 0xfff8, 0xe2f0),
 new opcode_handler_struct(m68000_lsr_ea_aw , 16, 0xffff, 0xe2f8),
 new opcode_handler_struct(m68000_lsr_ea_al , 16, 0xffff, 0xe2f9),
 new opcode_handler_struct(m68000_lsl_s_8 , 10, 0xf1f8, 0xe108),
 new opcode_handler_struct(m68000_lsl_s_16 , 10, 0xf1f8, 0xe148),
 new opcode_handler_struct(m68000_lsl_s_32 , 10, 0xf1f8, 0xe188),
 new opcode_handler_struct(m68000_lsl_r_8 , 10, 0xf1f8, 0xe128),
 new opcode_handler_struct(m68000_lsl_r_16 , 10, 0xf1f8, 0xe168),
 new opcode_handler_struct(m68000_lsl_r_32 , 10, 0xf1f8, 0xe1a8),
 new opcode_handler_struct(m68000_lsl_ea_ai , 13, 0xfff8, 0xe3d0),
 new opcode_handler_struct(m68000_lsl_ea_pi , 13, 0xfff8, 0xe3d8),
 new opcode_handler_struct(m68000_lsl_ea_pd , 13, 0xfff8, 0xe3e0),
 new opcode_handler_struct(m68000_lsl_ea_di , 13, 0xfff8, 0xe3e8),
 new opcode_handler_struct(m68000_lsl_ea_ix , 13, 0xfff8, 0xe3f0),
 new opcode_handler_struct(m68000_lsl_ea_aw , 16, 0xffff, 0xe3f8),
 new opcode_handler_struct(m68000_lsl_ea_al , 16, 0xffff, 0xe3f9),
 new opcode_handler_struct(m68000_move_dd_d_8 , 10, 0xf1f8, 0x1000),
 new opcode_handler_struct(m68000_move_dd_ai_8 , 10, 0xf1f8, 0x1010),
 new opcode_handler_struct(m68000_move_dd_pi_8 , 10, 0xf1f8, 0x1018),
 new opcode_handler_struct(m68000_move_dd_pi7_8 , 13, 0xf1ff, 0x101f),
 new opcode_handler_struct(m68000_move_dd_pd_8 , 10, 0xf1f8, 0x1020),
 new opcode_handler_struct(m68000_move_dd_pd7_8 , 13, 0xf1ff, 0x1027),
 new opcode_handler_struct(m68000_move_dd_di_8 , 10, 0xf1f8, 0x1028),
 new opcode_handler_struct(m68000_move_dd_ix_8 , 10, 0xf1f8, 0x1030),
 new opcode_handler_struct(m68000_move_dd_aw_8 , 13, 0xf1ff, 0x1038),
 new opcode_handler_struct(m68000_move_dd_al_8 , 13, 0xf1ff, 0x1039),
 new opcode_handler_struct(m68000_move_dd_pcdi_8 , 13, 0xf1ff, 0x103a),
 new opcode_handler_struct(m68000_move_dd_pcix_8 , 13, 0xf1ff, 0x103b),
 new opcode_handler_struct(m68000_move_dd_i_8 , 13, 0xf1ff, 0x103c),
 new opcode_handler_struct(m68000_move_ai_d_8 , 10, 0xf1f8, 0x1080),
 new opcode_handler_struct(m68000_move_ai_ai_8 , 10, 0xf1f8, 0x1090),
 new opcode_handler_struct(m68000_move_ai_pi_8 , 10, 0xf1f8, 0x1098),
 new opcode_handler_struct(m68000_move_ai_pi7_8 , 13, 0xf1ff, 0x109f),
 new opcode_handler_struct(m68000_move_ai_pd_8 , 10, 0xf1f8, 0x10a0),
 new opcode_handler_struct(m68000_move_ai_pd7_8 , 13, 0xf1ff, 0x10a7),
 new opcode_handler_struct(m68000_move_ai_di_8 , 10, 0xf1f8, 0x10a8),
 new opcode_handler_struct(m68000_move_ai_ix_8 , 10, 0xf1f8, 0x10b0),
 new opcode_handler_struct(m68000_move_ai_aw_8 , 13, 0xf1ff, 0x10b8),
 new opcode_handler_struct(m68000_move_ai_al_8 , 13, 0xf1ff, 0x10b9),
 new opcode_handler_struct(m68000_move_ai_pcdi_8 , 13, 0xf1ff, 0x10ba),
 new opcode_handler_struct(m68000_move_ai_pcix_8 , 13, 0xf1ff, 0x10bb),
 new opcode_handler_struct(m68000_move_ai_i_8 , 13, 0xf1ff, 0x10bc),
 new opcode_handler_struct(m68000_move_pi_d_8 , 10, 0xf1f8, 0x10c0),
 new opcode_handler_struct(m68000_move_pi_ai_8 , 10, 0xf1f8, 0x10d0),
 new opcode_handler_struct(m68000_move_pi_pi_8 , 10, 0xf1f8, 0x10d8),
 new opcode_handler_struct(m68000_move_pi_pi7_8 , 13, 0xf1ff, 0x10df),
 new opcode_handler_struct(m68000_move_pi_pd_8 , 10, 0xf1f8, 0x10e0),
 new opcode_handler_struct(m68000_move_pi_pd7_8 , 13, 0xf1ff, 0x10e7),
 new opcode_handler_struct(m68000_move_pi_di_8 , 10, 0xf1f8, 0x10e8),
 new opcode_handler_struct(m68000_move_pi_ix_8 , 10, 0xf1f8, 0x10f0),
 new opcode_handler_struct(m68000_move_pi_aw_8 , 13, 0xf1ff, 0x10f8),
 new opcode_handler_struct(m68000_move_pi_al_8 , 13, 0xf1ff, 0x10f9),
 new opcode_handler_struct(m68000_move_pi_pcdi_8 , 13, 0xf1ff, 0x10fa),
 new opcode_handler_struct(m68000_move_pi_pcix_8 , 13, 0xf1ff, 0x10fb),
 new opcode_handler_struct(m68000_move_pi_i_8 , 13, 0xf1ff, 0x10fc),
 new opcode_handler_struct(m68000_move_pi7_d_8 , 13, 0xfff8, 0x1ec0),
 new opcode_handler_struct(m68000_move_pi7_ai_8 , 13, 0xfff8, 0x1ed0),
 new opcode_handler_struct(m68000_move_pi7_pi_8 , 13, 0xfff8, 0x1ed8),
 new opcode_handler_struct(m68000_move_pi7_pi7_8 , 16, 0xffff, 0x1edf),
 new opcode_handler_struct(m68000_move_pi7_pd_8 , 13, 0xfff8, 0x1ee0),
 new opcode_handler_struct(m68000_move_pi7_pd7_8 , 16, 0xffff, 0x1ee7),
 new opcode_handler_struct(m68000_move_pi7_di_8 , 13, 0xfff8, 0x1ee8),
 new opcode_handler_struct(m68000_move_pi7_ix_8 , 13, 0xfff8, 0x1ef0),
 new opcode_handler_struct(m68000_move_pi7_aw_8 , 16, 0xffff, 0x1ef8),
 new opcode_handler_struct(m68000_move_pi7_al_8 , 16, 0xffff, 0x1ef9),
 new opcode_handler_struct(m68000_move_pi7_pcdi_8 , 16, 0xffff, 0x1efa),
 new opcode_handler_struct(m68000_move_pi7_pcix_8 , 16, 0xffff, 0x1efb),
 new opcode_handler_struct(m68000_move_pi7_i_8 , 16, 0xffff, 0x1efc),
 new opcode_handler_struct(m68000_move_pd_d_8 , 10, 0xf1f8, 0x1100),
 new opcode_handler_struct(m68000_move_pd_ai_8 , 10, 0xf1f8, 0x1110),
 new opcode_handler_struct(m68000_move_pd_pi_8 , 10, 0xf1f8, 0x1118),
 new opcode_handler_struct(m68000_move_pd_pi7_8 , 13, 0xf1ff, 0x111f),
 new opcode_handler_struct(m68000_move_pd_pd_8 , 10, 0xf1f8, 0x1120),
 new opcode_handler_struct(m68000_move_pd_pd7_8 , 13, 0xf1ff, 0x1127),
 new opcode_handler_struct(m68000_move_pd_di_8 , 10, 0xf1f8, 0x1128),
 new opcode_handler_struct(m68000_move_pd_ix_8 , 10, 0xf1f8, 0x1130),
 new opcode_handler_struct(m68000_move_pd_aw_8 , 13, 0xf1ff, 0x1138),
 new opcode_handler_struct(m68000_move_pd_al_8 , 13, 0xf1ff, 0x1139),
 new opcode_handler_struct(m68000_move_pd_pcdi_8 , 13, 0xf1ff, 0x113a),
 new opcode_handler_struct(m68000_move_pd_pcix_8 , 13, 0xf1ff, 0x113b),
 new opcode_handler_struct(m68000_move_pd_i_8 , 13, 0xf1ff, 0x113c),
 new opcode_handler_struct(m68000_move_pd7_d_8 , 13, 0xfff8, 0x1f00),
 new opcode_handler_struct(m68000_move_pd7_ai_8 , 13, 0xfff8, 0x1f10),
 new opcode_handler_struct(m68000_move_pd7_pi_8 , 13, 0xfff8, 0x1f18),
 new opcode_handler_struct(m68000_move_pd7_pi7_8 , 16, 0xffff, 0x1f1f),
 new opcode_handler_struct(m68000_move_pd7_pd_8 , 13, 0xfff8, 0x1f20),
 new opcode_handler_struct(m68000_move_pd7_pd7_8 , 16, 0xffff, 0x1f27),
 new opcode_handler_struct(m68000_move_pd7_di_8 , 13, 0xfff8, 0x1f28),
 new opcode_handler_struct(m68000_move_pd7_ix_8 , 13, 0xfff8, 0x1f30),
 new opcode_handler_struct(m68000_move_pd7_aw_8 , 16, 0xffff, 0x1f38),
 new opcode_handler_struct(m68000_move_pd7_al_8 , 16, 0xffff, 0x1f39),
 new opcode_handler_struct(m68000_move_pd7_pcdi_8 , 16, 0xffff, 0x1f3a),
 new opcode_handler_struct(m68000_move_pd7_pcix_8 , 16, 0xffff, 0x1f3b),
 new opcode_handler_struct(m68000_move_pd7_i_8 , 16, 0xffff, 0x1f3c),
 new opcode_handler_struct(m68000_move_di_d_8 , 10, 0xf1f8, 0x1140),
 new opcode_handler_struct(m68000_move_di_ai_8 , 10, 0xf1f8, 0x1150),
 new opcode_handler_struct(m68000_move_di_pi_8 , 10, 0xf1f8, 0x1158),
 new opcode_handler_struct(m68000_move_di_pi7_8 , 13, 0xf1ff, 0x115f),
 new opcode_handler_struct(m68000_move_di_pd_8 , 10, 0xf1f8, 0x1160),
 new opcode_handler_struct(m68000_move_di_pd7_8 , 13, 0xf1ff, 0x1167),
 new opcode_handler_struct(m68000_move_di_di_8 , 10, 0xf1f8, 0x1168),
 new opcode_handler_struct(m68000_move_di_ix_8 , 10, 0xf1f8, 0x1170),
 new opcode_handler_struct(m68000_move_di_aw_8 , 13, 0xf1ff, 0x1178),
 new opcode_handler_struct(m68000_move_di_al_8 , 13, 0xf1ff, 0x1179),
 new opcode_handler_struct(m68000_move_di_pcdi_8 , 13, 0xf1ff, 0x117a),
 new opcode_handler_struct(m68000_move_di_pcix_8 , 13, 0xf1ff, 0x117b),
 new opcode_handler_struct(m68000_move_di_i_8 , 13, 0xf1ff, 0x117c),
 new opcode_handler_struct(m68000_move_ix_d_8 , 10, 0xf1f8, 0x1180),
 new opcode_handler_struct(m68000_move_ix_ai_8 , 10, 0xf1f8, 0x1190),
 new opcode_handler_struct(m68000_move_ix_pi_8 , 10, 0xf1f8, 0x1198),
 new opcode_handler_struct(m68000_move_ix_pi7_8 , 13, 0xf1ff, 0x119f),
 new opcode_handler_struct(m68000_move_ix_pd_8 , 10, 0xf1f8, 0x11a0),
 new opcode_handler_struct(m68000_move_ix_pd7_8 , 13, 0xf1ff, 0x11a7),
 new opcode_handler_struct(m68000_move_ix_di_8 , 10, 0xf1f8, 0x11a8),
 new opcode_handler_struct(m68000_move_ix_ix_8 , 10, 0xf1f8, 0x11b0),
 new opcode_handler_struct(m68000_move_ix_aw_8 , 13, 0xf1ff, 0x11b8),
 new opcode_handler_struct(m68000_move_ix_al_8 , 13, 0xf1ff, 0x11b9),
 new opcode_handler_struct(m68000_move_ix_pcdi_8 , 13, 0xf1ff, 0x11ba),
 new opcode_handler_struct(m68000_move_ix_pcix_8 , 13, 0xf1ff, 0x11bb),
 new opcode_handler_struct(m68000_move_ix_i_8 , 13, 0xf1ff, 0x11bc),
 new opcode_handler_struct(m68000_move_aw_d_8 , 13, 0xfff8, 0x11c0),
 new opcode_handler_struct(m68000_move_aw_ai_8 , 13, 0xfff8, 0x11d0),
 new opcode_handler_struct(m68000_move_aw_pi_8 , 13, 0xfff8, 0x11d8),
 new opcode_handler_struct(m68000_move_aw_pi7_8 , 16, 0xffff, 0x11df),
 new opcode_handler_struct(m68000_move_aw_pd_8 , 13, 0xfff8, 0x11e0),
 new opcode_handler_struct(m68000_move_aw_pd7_8 , 16, 0xffff, 0x11e7),
 new opcode_handler_struct(m68000_move_aw_di_8 , 13, 0xfff8, 0x11e8),
 new opcode_handler_struct(m68000_move_aw_ix_8 , 13, 0xfff8, 0x11f0),
 new opcode_handler_struct(m68000_move_aw_aw_8 , 16, 0xffff, 0x11f8),
 new opcode_handler_struct(m68000_move_aw_al_8 , 16, 0xffff, 0x11f9),
 new opcode_handler_struct(m68000_move_aw_pcdi_8 , 16, 0xffff, 0x11fa),
 new opcode_handler_struct(m68000_move_aw_pcix_8 , 16, 0xffff, 0x11fb),
 new opcode_handler_struct(m68000_move_aw_i_8 , 16, 0xffff, 0x11fc),
 new opcode_handler_struct(m68000_move_al_d_8 , 13, 0xfff8, 0x13c0),
 new opcode_handler_struct(m68000_move_al_ai_8 , 13, 0xfff8, 0x13d0),
 new opcode_handler_struct(m68000_move_al_pi_8 , 13, 0xfff8, 0x13d8),
 new opcode_handler_struct(m68000_move_al_pi7_8 , 16, 0xffff, 0x13df),
 new opcode_handler_struct(m68000_move_al_pd_8 , 13, 0xfff8, 0x13e0),
 new opcode_handler_struct(m68000_move_al_pd7_8 , 16, 0xffff, 0x13e7),
 new opcode_handler_struct(m68000_move_al_di_8 , 13, 0xfff8, 0x13e8),
 new opcode_handler_struct(m68000_move_al_ix_8 , 13, 0xfff8, 0x13f0),
 new opcode_handler_struct(m68000_move_al_aw_8 , 16, 0xffff, 0x13f8),
 new opcode_handler_struct(m68000_move_al_al_8 , 16, 0xffff, 0x13f9),
 new opcode_handler_struct(m68000_move_al_pcdi_8 , 16, 0xffff, 0x13fa),
 new opcode_handler_struct(m68000_move_al_pcix_8 , 16, 0xffff, 0x13fb),
 new opcode_handler_struct(m68000_move_al_i_8 , 16, 0xffff, 0x13fc),
 new opcode_handler_struct(m68000_move_dd_d_16 , 10, 0xf1f8, 0x3000),
 new opcode_handler_struct(m68000_move_dd_a_16 , 10, 0xf1f8, 0x3008),
 new opcode_handler_struct(m68000_move_dd_ai_16 , 10, 0xf1f8, 0x3010),
 new opcode_handler_struct(m68000_move_dd_pi_16 , 10, 0xf1f8, 0x3018),
 new opcode_handler_struct(m68000_move_dd_pd_16 , 10, 0xf1f8, 0x3020),
 new opcode_handler_struct(m68000_move_dd_di_16 , 10, 0xf1f8, 0x3028),
 new opcode_handler_struct(m68000_move_dd_ix_16 , 10, 0xf1f8, 0x3030),
 new opcode_handler_struct(m68000_move_dd_aw_16 , 13, 0xf1ff, 0x3038),
 new opcode_handler_struct(m68000_move_dd_al_16 , 13, 0xf1ff, 0x3039),
 new opcode_handler_struct(m68000_move_dd_pcdi_16 , 13, 0xf1ff, 0x303a),
 new opcode_handler_struct(m68000_move_dd_pcix_16 , 13, 0xf1ff, 0x303b),
 new opcode_handler_struct(m68000_move_dd_i_16 , 13, 0xf1ff, 0x303c),
 new opcode_handler_struct(m68000_move_ai_d_16 , 10, 0xf1f8, 0x3080),
 new opcode_handler_struct(m68000_move_ai_a_16 , 10, 0xf1f8, 0x3088),
 new opcode_handler_struct(m68000_move_ai_ai_16 , 10, 0xf1f8, 0x3090),
 new opcode_handler_struct(m68000_move_ai_pi_16 , 10, 0xf1f8, 0x3098),
 new opcode_handler_struct(m68000_move_ai_pd_16 , 10, 0xf1f8, 0x30a0),
 new opcode_handler_struct(m68000_move_ai_di_16 , 10, 0xf1f8, 0x30a8),
 new opcode_handler_struct(m68000_move_ai_ix_16 , 10, 0xf1f8, 0x30b0),
 new opcode_handler_struct(m68000_move_ai_aw_16 , 13, 0xf1ff, 0x30b8),
 new opcode_handler_struct(m68000_move_ai_al_16 , 13, 0xf1ff, 0x30b9),
 new opcode_handler_struct(m68000_move_ai_pcdi_16 , 13, 0xf1ff, 0x30ba),
 new opcode_handler_struct(m68000_move_ai_pcix_16 , 13, 0xf1ff, 0x30bb),
 new opcode_handler_struct(m68000_move_ai_i_16 , 13, 0xf1ff, 0x30bc),
 new opcode_handler_struct(m68000_move_pi_d_16 , 10, 0xf1f8, 0x30c0),
 new opcode_handler_struct(m68000_move_pi_a_16 , 10, 0xf1f8, 0x30c8),
 new opcode_handler_struct(m68000_move_pi_ai_16 , 10, 0xf1f8, 0x30d0),
 new opcode_handler_struct(m68000_move_pi_pi_16 , 10, 0xf1f8, 0x30d8),
 new opcode_handler_struct(m68000_move_pi_pd_16 , 10, 0xf1f8, 0x30e0),
 new opcode_handler_struct(m68000_move_pi_di_16 , 10, 0xf1f8, 0x30e8),
 new opcode_handler_struct(m68000_move_pi_ix_16 , 10, 0xf1f8, 0x30f0),
 new opcode_handler_struct(m68000_move_pi_aw_16 , 13, 0xf1ff, 0x30f8),
 new opcode_handler_struct(m68000_move_pi_al_16 , 13, 0xf1ff, 0x30f9),
 new opcode_handler_struct(m68000_move_pi_pcdi_16 , 13, 0xf1ff, 0x30fa),
 new opcode_handler_struct(m68000_move_pi_pcix_16 , 13, 0xf1ff, 0x30fb),
 new opcode_handler_struct(m68000_move_pi_i_16 , 13, 0xf1ff, 0x30fc),
 new opcode_handler_struct(m68000_move_pd_d_16 , 10, 0xf1f8, 0x3100),
 new opcode_handler_struct(m68000_move_pd_a_16 , 10, 0xf1f8, 0x3108),
 new opcode_handler_struct(m68000_move_pd_ai_16 , 10, 0xf1f8, 0x3110),
 new opcode_handler_struct(m68000_move_pd_pi_16 , 10, 0xf1f8, 0x3118),
 new opcode_handler_struct(m68000_move_pd_pd_16 , 10, 0xf1f8, 0x3120),
 new opcode_handler_struct(m68000_move_pd_di_16 , 10, 0xf1f8, 0x3128),
 new opcode_handler_struct(m68000_move_pd_ix_16 , 10, 0xf1f8, 0x3130),
 new opcode_handler_struct(m68000_move_pd_aw_16 , 13, 0xf1ff, 0x3138),
 new opcode_handler_struct(m68000_move_pd_al_16 , 13, 0xf1ff, 0x3139),
 new opcode_handler_struct(m68000_move_pd_pcdi_16 , 13, 0xf1ff, 0x313a),
 new opcode_handler_struct(m68000_move_pd_pcix_16 , 13, 0xf1ff, 0x313b),
 new opcode_handler_struct(m68000_move_pd_i_16 , 13, 0xf1ff, 0x313c),
 new opcode_handler_struct(m68000_move_di_d_16 , 10, 0xf1f8, 0x3140),
 new opcode_handler_struct(m68000_move_di_a_16 , 10, 0xf1f8, 0x3148),
 new opcode_handler_struct(m68000_move_di_ai_16 , 10, 0xf1f8, 0x3150),
 new opcode_handler_struct(m68000_move_di_pi_16 , 10, 0xf1f8, 0x3158),
 new opcode_handler_struct(m68000_move_di_pd_16 , 10, 0xf1f8, 0x3160),
 new opcode_handler_struct(m68000_move_di_di_16 , 10, 0xf1f8, 0x3168),
 new opcode_handler_struct(m68000_move_di_ix_16 , 10, 0xf1f8, 0x3170),
 new opcode_handler_struct(m68000_move_di_aw_16 , 13, 0xf1ff, 0x3178),
 new opcode_handler_struct(m68000_move_di_al_16 , 13, 0xf1ff, 0x3179),
 new opcode_handler_struct(m68000_move_di_pcdi_16 , 13, 0xf1ff, 0x317a),
 new opcode_handler_struct(m68000_move_di_pcix_16 , 13, 0xf1ff, 0x317b),
 new opcode_handler_struct(m68000_move_di_i_16 , 13, 0xf1ff, 0x317c),
 new opcode_handler_struct(m68000_move_ix_d_16 , 10, 0xf1f8, 0x3180),
 new opcode_handler_struct(m68000_move_ix_a_16 , 10, 0xf1f8, 0x3188),
 new opcode_handler_struct(m68000_move_ix_ai_16 , 10, 0xf1f8, 0x3190),
 new opcode_handler_struct(m68000_move_ix_pi_16 , 10, 0xf1f8, 0x3198),
 new opcode_handler_struct(m68000_move_ix_pd_16 , 10, 0xf1f8, 0x31a0),
 new opcode_handler_struct(m68000_move_ix_di_16 , 10, 0xf1f8, 0x31a8),
 new opcode_handler_struct(m68000_move_ix_ix_16 , 10, 0xf1f8, 0x31b0),
 new opcode_handler_struct(m68000_move_ix_aw_16 , 13, 0xf1ff, 0x31b8),
 new opcode_handler_struct(m68000_move_ix_al_16 , 13, 0xf1ff, 0x31b9),
 new opcode_handler_struct(m68000_move_ix_pcdi_16 , 13, 0xf1ff, 0x31ba),
 new opcode_handler_struct(m68000_move_ix_pcix_16 , 13, 0xf1ff, 0x31bb),
 new opcode_handler_struct(m68000_move_ix_i_16 , 13, 0xf1ff, 0x31bc),
 new opcode_handler_struct(m68000_move_aw_d_16 , 13, 0xfff8, 0x31c0),
 new opcode_handler_struct(m68000_move_aw_a_16 , 13, 0xfff8, 0x31c8),
 new opcode_handler_struct(m68000_move_aw_ai_16 , 13, 0xfff8, 0x31d0),
 new opcode_handler_struct(m68000_move_aw_pi_16 , 13, 0xfff8, 0x31d8),
 new opcode_handler_struct(m68000_move_aw_pd_16 , 13, 0xfff8, 0x31e0),
 new opcode_handler_struct(m68000_move_aw_di_16 , 13, 0xfff8, 0x31e8),
 new opcode_handler_struct(m68000_move_aw_ix_16 , 13, 0xfff8, 0x31f0),
 new opcode_handler_struct(m68000_move_aw_aw_16 , 16, 0xffff, 0x31f8),
 new opcode_handler_struct(m68000_move_aw_al_16 , 16, 0xffff, 0x31f9),
 new opcode_handler_struct(m68000_move_aw_pcdi_16 , 16, 0xffff, 0x31fa),
 new opcode_handler_struct(m68000_move_aw_pcix_16 , 16, 0xffff, 0x31fb),
 new opcode_handler_struct(m68000_move_aw_i_16 , 16, 0xffff, 0x31fc),
 new opcode_handler_struct(m68000_move_al_d_16 , 13, 0xfff8, 0x33c0),
 new opcode_handler_struct(m68000_move_al_a_16 , 13, 0xfff8, 0x33c8),
 new opcode_handler_struct(m68000_move_al_ai_16 , 13, 0xfff8, 0x33d0),
 new opcode_handler_struct(m68000_move_al_pi_16 , 13, 0xfff8, 0x33d8),
 new opcode_handler_struct(m68000_move_al_pd_16 , 13, 0xfff8, 0x33e0),
 new opcode_handler_struct(m68000_move_al_di_16 , 13, 0xfff8, 0x33e8),
 new opcode_handler_struct(m68000_move_al_ix_16 , 13, 0xfff8, 0x33f0),
 new opcode_handler_struct(m68000_move_al_aw_16 , 16, 0xffff, 0x33f8),
 new opcode_handler_struct(m68000_move_al_al_16 , 16, 0xffff, 0x33f9),
 new opcode_handler_struct(m68000_move_al_pcdi_16 , 16, 0xffff, 0x33fa),
 new opcode_handler_struct(m68000_move_al_pcix_16 , 16, 0xffff, 0x33fb),
 new opcode_handler_struct(m68000_move_al_i_16 , 16, 0xffff, 0x33fc),
 new opcode_handler_struct(m68000_move_dd_d_32 , 10, 0xf1f8, 0x2000),
 new opcode_handler_struct(m68000_move_dd_a_32 , 10, 0xf1f8, 0x2008),
 new opcode_handler_struct(m68000_move_dd_ai_32 , 10, 0xf1f8, 0x2010),
 new opcode_handler_struct(m68000_move_dd_pi_32 , 10, 0xf1f8, 0x2018),
 new opcode_handler_struct(m68000_move_dd_pd_32 , 10, 0xf1f8, 0x2020),
 new opcode_handler_struct(m68000_move_dd_di_32 , 10, 0xf1f8, 0x2028),
 new opcode_handler_struct(m68000_move_dd_ix_32 , 10, 0xf1f8, 0x2030),
 new opcode_handler_struct(m68000_move_dd_aw_32 , 13, 0xf1ff, 0x2038),
 new opcode_handler_struct(m68000_move_dd_al_32 , 13, 0xf1ff, 0x2039),
 new opcode_handler_struct(m68000_move_dd_pcdi_32 , 13, 0xf1ff, 0x203a),
 new opcode_handler_struct(m68000_move_dd_pcix_32 , 13, 0xf1ff, 0x203b),
 new opcode_handler_struct(m68000_move_dd_i_32 , 13, 0xf1ff, 0x203c),
 new opcode_handler_struct(m68000_move_ai_d_32 , 10, 0xf1f8, 0x2080),
 new opcode_handler_struct(m68000_move_ai_a_32 , 10, 0xf1f8, 0x2088),
 new opcode_handler_struct(m68000_move_ai_ai_32 , 10, 0xf1f8, 0x2090),
 new opcode_handler_struct(m68000_move_ai_pi_32 , 10, 0xf1f8, 0x2098),
 new opcode_handler_struct(m68000_move_ai_pd_32 , 10, 0xf1f8, 0x20a0),
 new opcode_handler_struct(m68000_move_ai_di_32 , 10, 0xf1f8, 0x20a8),
 new opcode_handler_struct(m68000_move_ai_ix_32 , 10, 0xf1f8, 0x20b0),
 new opcode_handler_struct(m68000_move_ai_aw_32 , 13, 0xf1ff, 0x20b8),
 new opcode_handler_struct(m68000_move_ai_al_32 , 13, 0xf1ff, 0x20b9),
 new opcode_handler_struct(m68000_move_ai_pcdi_32 , 13, 0xf1ff, 0x20ba),
 new opcode_handler_struct(m68000_move_ai_pcix_32 , 13, 0xf1ff, 0x20bb),
 new opcode_handler_struct(m68000_move_ai_i_32 , 13, 0xf1ff, 0x20bc),
 new opcode_handler_struct(m68000_move_pi_d_32 , 10, 0xf1f8, 0x20c0),
 new opcode_handler_struct(m68000_move_pi_a_32 , 10, 0xf1f8, 0x20c8),
 new opcode_handler_struct(m68000_move_pi_ai_32 , 10, 0xf1f8, 0x20d0),
 new opcode_handler_struct(m68000_move_pi_pi_32 , 10, 0xf1f8, 0x20d8),
 new opcode_handler_struct(m68000_move_pi_pd_32 , 10, 0xf1f8, 0x20e0),
 new opcode_handler_struct(m68000_move_pi_di_32 , 10, 0xf1f8, 0x20e8),
 new opcode_handler_struct(m68000_move_pi_ix_32 , 10, 0xf1f8, 0x20f0),
 new opcode_handler_struct(m68000_move_pi_aw_32 , 13, 0xf1ff, 0x20f8),
 new opcode_handler_struct(m68000_move_pi_al_32 , 13, 0xf1ff, 0x20f9),
 new opcode_handler_struct(m68000_move_pi_pcdi_32 , 13, 0xf1ff, 0x20fa),
 new opcode_handler_struct(m68000_move_pi_pcix_32 , 13, 0xf1ff, 0x20fb),
 new opcode_handler_struct(m68000_move_pi_i_32 , 13, 0xf1ff, 0x20fc),
 new opcode_handler_struct(m68000_move_pd_d_32 , 10, 0xf1f8, 0x2100),
 new opcode_handler_struct(m68000_move_pd_a_32 , 10, 0xf1f8, 0x2108),
 new opcode_handler_struct(m68000_move_pd_ai_32 , 10, 0xf1f8, 0x2110),
 new opcode_handler_struct(m68000_move_pd_pi_32 , 10, 0xf1f8, 0x2118),
 new opcode_handler_struct(m68000_move_pd_pd_32 , 10, 0xf1f8, 0x2120),
 new opcode_handler_struct(m68000_move_pd_di_32 , 10, 0xf1f8, 0x2128),
 new opcode_handler_struct(m68000_move_pd_ix_32 , 10, 0xf1f8, 0x2130),
 new opcode_handler_struct(m68000_move_pd_aw_32 , 13, 0xf1ff, 0x2138),
 new opcode_handler_struct(m68000_move_pd_al_32 , 13, 0xf1ff, 0x2139),
 new opcode_handler_struct(m68000_move_pd_pcdi_32 , 13, 0xf1ff, 0x213a),
 new opcode_handler_struct(m68000_move_pd_pcix_32 , 13, 0xf1ff, 0x213b),
 new opcode_handler_struct(m68000_move_pd_i_32 , 13, 0xf1ff, 0x213c),
 new opcode_handler_struct(m68000_move_di_d_32 , 10, 0xf1f8, 0x2140),
 new opcode_handler_struct(m68000_move_di_a_32 , 10, 0xf1f8, 0x2148),
 new opcode_handler_struct(m68000_move_di_ai_32 , 10, 0xf1f8, 0x2150),
 new opcode_handler_struct(m68000_move_di_pi_32 , 10, 0xf1f8, 0x2158),
 new opcode_handler_struct(m68000_move_di_pd_32 , 10, 0xf1f8, 0x2160),
 new opcode_handler_struct(m68000_move_di_di_32 , 10, 0xf1f8, 0x2168),
 new opcode_handler_struct(m68000_move_di_ix_32 , 10, 0xf1f8, 0x2170),
 new opcode_handler_struct(m68000_move_di_aw_32 , 13, 0xf1ff, 0x2178),
 new opcode_handler_struct(m68000_move_di_al_32 , 13, 0xf1ff, 0x2179),
 new opcode_handler_struct(m68000_move_di_pcdi_32 , 13, 0xf1ff, 0x217a),
 new opcode_handler_struct(m68000_move_di_pcix_32 , 13, 0xf1ff, 0x217b),
 new opcode_handler_struct(m68000_move_di_i_32 , 13, 0xf1ff, 0x217c),
 new opcode_handler_struct(m68000_move_ix_d_32 , 10, 0xf1f8, 0x2180),
 new opcode_handler_struct(m68000_move_ix_a_32 , 10, 0xf1f8, 0x2188),
 new opcode_handler_struct(m68000_move_ix_ai_32 , 10, 0xf1f8, 0x2190),
 new opcode_handler_struct(m68000_move_ix_pi_32 , 10, 0xf1f8, 0x2198),
 new opcode_handler_struct(m68000_move_ix_pd_32 , 10, 0xf1f8, 0x21a0),
 new opcode_handler_struct(m68000_move_ix_di_32 , 10, 0xf1f8, 0x21a8),
 new opcode_handler_struct(m68000_move_ix_ix_32 , 10, 0xf1f8, 0x21b0),
 new opcode_handler_struct(m68000_move_ix_aw_32 , 13, 0xf1ff, 0x21b8),
 new opcode_handler_struct(m68000_move_ix_al_32 , 13, 0xf1ff, 0x21b9),
 new opcode_handler_struct(m68000_move_ix_pcdi_32 , 13, 0xf1ff, 0x21ba),
 new opcode_handler_struct(m68000_move_ix_pcix_32 , 13, 0xf1ff, 0x21bb),
 new opcode_handler_struct(m68000_move_ix_i_32 , 13, 0xf1ff, 0x21bc),
 new opcode_handler_struct(m68000_move_aw_d_32 , 13, 0xfff8, 0x21c0),
 new opcode_handler_struct(m68000_move_aw_a_32 , 13, 0xfff8, 0x21c8),
 new opcode_handler_struct(m68000_move_aw_ai_32 , 13, 0xfff8, 0x21d0),
 new opcode_handler_struct(m68000_move_aw_pi_32 , 13, 0xfff8, 0x21d8),
 new opcode_handler_struct(m68000_move_aw_pd_32 , 13, 0xfff8, 0x21e0),
 new opcode_handler_struct(m68000_move_aw_di_32 , 13, 0xfff8, 0x21e8),
 new opcode_handler_struct(m68000_move_aw_ix_32 , 13, 0xfff8, 0x21f0),
 new opcode_handler_struct(m68000_move_aw_aw_32 , 16, 0xffff, 0x21f8),
 new opcode_handler_struct(m68000_move_aw_al_32 , 16, 0xffff, 0x21f9),
 new opcode_handler_struct(m68000_move_aw_pcdi_32 , 16, 0xffff, 0x21fa),
 new opcode_handler_struct(m68000_move_aw_pcix_32 , 16, 0xffff, 0x21fb),
 new opcode_handler_struct(m68000_move_aw_i_32 , 16, 0xffff, 0x21fc),
 new opcode_handler_struct(m68000_move_al_d_32 , 13, 0xfff8, 0x23c0),
 new opcode_handler_struct(m68000_move_al_a_32 , 13, 0xfff8, 0x23c8),
 new opcode_handler_struct(m68000_move_al_ai_32 , 13, 0xfff8, 0x23d0),
 new opcode_handler_struct(m68000_move_al_pi_32 , 13, 0xfff8, 0x23d8),
 new opcode_handler_struct(m68000_move_al_pd_32 , 13, 0xfff8, 0x23e0),
 new opcode_handler_struct(m68000_move_al_di_32 , 13, 0xfff8, 0x23e8),
 new opcode_handler_struct(m68000_move_al_ix_32 , 13, 0xfff8, 0x23f0),
 new opcode_handler_struct(m68000_move_al_aw_32 , 16, 0xffff, 0x23f8),
 new opcode_handler_struct(m68000_move_al_al_32 , 16, 0xffff, 0x23f9),
 new opcode_handler_struct(m68000_move_al_pcdi_32 , 16, 0xffff, 0x23fa),
 new opcode_handler_struct(m68000_move_al_pcix_32 , 16, 0xffff, 0x23fb),
 new opcode_handler_struct(m68000_move_al_i_32 , 16, 0xffff, 0x23fc),
 new opcode_handler_struct(m68000_movea_d_16 , 10, 0xf1f8, 0x3040),
 new opcode_handler_struct(m68000_movea_a_16 , 10, 0xf1f8, 0x3048),
 new opcode_handler_struct(m68000_movea_ai_16 , 10, 0xf1f8, 0x3050),
 new opcode_handler_struct(m68000_movea_pi_16 , 10, 0xf1f8, 0x3058),
 new opcode_handler_struct(m68000_movea_pd_16 , 10, 0xf1f8, 0x3060),
 new opcode_handler_struct(m68000_movea_di_16 , 10, 0xf1f8, 0x3068),
 new opcode_handler_struct(m68000_movea_ix_16 , 10, 0xf1f8, 0x3070),
 new opcode_handler_struct(m68000_movea_aw_16 , 13, 0xf1ff, 0x3078),
 new opcode_handler_struct(m68000_movea_al_16 , 13, 0xf1ff, 0x3079),
 new opcode_handler_struct(m68000_movea_pcdi_16 , 13, 0xf1ff, 0x307a),
 new opcode_handler_struct(m68000_movea_pcix_16 , 13, 0xf1ff, 0x307b),
 new opcode_handler_struct(m68000_movea_i_16 , 13, 0xf1ff, 0x307c),
 new opcode_handler_struct(m68000_movea_d_32 , 10, 0xf1f8, 0x2040),
 new opcode_handler_struct(m68000_movea_a_32 , 10, 0xf1f8, 0x2048),
 new opcode_handler_struct(m68000_movea_ai_32 , 10, 0xf1f8, 0x2050),
 new opcode_handler_struct(m68000_movea_pi_32 , 10, 0xf1f8, 0x2058),
 new opcode_handler_struct(m68000_movea_pd_32 , 10, 0xf1f8, 0x2060),
 new opcode_handler_struct(m68000_movea_di_32 , 10, 0xf1f8, 0x2068),
 new opcode_handler_struct(m68000_movea_ix_32 , 10, 0xf1f8, 0x2070),
 new opcode_handler_struct(m68000_movea_aw_32 , 13, 0xf1ff, 0x2078),
 new opcode_handler_struct(m68000_movea_al_32 , 13, 0xf1ff, 0x2079),
 new opcode_handler_struct(m68000_movea_pcdi_32 , 13, 0xf1ff, 0x207a),
 new opcode_handler_struct(m68000_movea_pcix_32 , 13, 0xf1ff, 0x207b),
 new opcode_handler_struct(m68000_movea_i_32 , 13, 0xf1ff, 0x207c),
 new opcode_handler_struct(m68010_move_fr_ccr_d , 13, 0xfff8, 0x42c0),
 new opcode_handler_struct(m68010_move_fr_ccr_ai , 13, 0xfff8, 0x42d0),
 new opcode_handler_struct(m68010_move_fr_ccr_pi , 13, 0xfff8, 0x42d8),
 new opcode_handler_struct(m68010_move_fr_ccr_pd , 13, 0xfff8, 0x42e0),
 new opcode_handler_struct(m68010_move_fr_ccr_di , 13, 0xfff8, 0x42e8),
 new opcode_handler_struct(m68010_move_fr_ccr_ix , 13, 0xfff8, 0x42f0),
 new opcode_handler_struct(m68010_move_fr_ccr_aw , 16, 0xffff, 0x42f8),
 new opcode_handler_struct(m68010_move_fr_ccr_al , 16, 0xffff, 0x42f9),
 new opcode_handler_struct(m68000_move_to_ccr_d , 13, 0xfff8, 0x44c0),
 new opcode_handler_struct(m68000_move_to_ccr_ai , 13, 0xfff8, 0x44d0),
 new opcode_handler_struct(m68000_move_to_ccr_pi , 13, 0xfff8, 0x44d8),
 new opcode_handler_struct(m68000_move_to_ccr_pd , 13, 0xfff8, 0x44e0),
 new opcode_handler_struct(m68000_move_to_ccr_di , 13, 0xfff8, 0x44e8),
 new opcode_handler_struct(m68000_move_to_ccr_ix , 13, 0xfff8, 0x44f0),
 new opcode_handler_struct(m68000_move_to_ccr_aw , 16, 0xffff, 0x44f8),
 new opcode_handler_struct(m68000_move_to_ccr_al , 16, 0xffff, 0x44f9),
 new opcode_handler_struct(m68000_move_to_ccr_pcdi , 16, 0xffff, 0x44fa),
 new opcode_handler_struct(m68000_move_to_ccr_pcix , 16, 0xffff, 0x44fb),
 new opcode_handler_struct(m68000_move_to_ccr_i , 16, 0xffff, 0x44fc),
 new opcode_handler_struct(m68000_move_fr_sr_d , 13, 0xfff8, 0x40c0),
 new opcode_handler_struct(m68000_move_fr_sr_ai , 13, 0xfff8, 0x40d0),
 new opcode_handler_struct(m68000_move_fr_sr_pi , 13, 0xfff8, 0x40d8),
 new opcode_handler_struct(m68000_move_fr_sr_pd , 13, 0xfff8, 0x40e0),
 new opcode_handler_struct(m68000_move_fr_sr_di , 13, 0xfff8, 0x40e8),
 new opcode_handler_struct(m68000_move_fr_sr_ix , 13, 0xfff8, 0x40f0),
 new opcode_handler_struct(m68000_move_fr_sr_aw , 16, 0xffff, 0x40f8),
 new opcode_handler_struct(m68000_move_fr_sr_al , 16, 0xffff, 0x40f9),
 new opcode_handler_struct(m68000_move_to_sr_d , 13, 0xfff8, 0x46c0),
 new opcode_handler_struct(m68000_move_to_sr_ai , 13, 0xfff8, 0x46d0),
 new opcode_handler_struct(m68000_move_to_sr_pi , 13, 0xfff8, 0x46d8),
 new opcode_handler_struct(m68000_move_to_sr_pd , 13, 0xfff8, 0x46e0),
 new opcode_handler_struct(m68000_move_to_sr_di , 13, 0xfff8, 0x46e8),
 new opcode_handler_struct(m68000_move_to_sr_ix , 13, 0xfff8, 0x46f0),
 new opcode_handler_struct(m68000_move_to_sr_aw , 16, 0xffff, 0x46f8),
 new opcode_handler_struct(m68000_move_to_sr_al , 16, 0xffff, 0x46f9),
 new opcode_handler_struct(m68000_move_to_sr_pcdi , 16, 0xffff, 0x46fa),
 new opcode_handler_struct(m68000_move_to_sr_pcix , 16, 0xffff, 0x46fb),
 new opcode_handler_struct(m68000_move_to_sr_i , 16, 0xffff, 0x46fc),
 new opcode_handler_struct(m68000_move_fr_usp , 13, 0xfff8, 0x4e68),
 new opcode_handler_struct(m68000_move_to_usp , 13, 0xfff8, 0x4e60),
 new opcode_handler_struct(m68010_movec_cr , 16, 0xffff, 0x4e7a),
 new opcode_handler_struct(m68010_movec_rc , 16, 0xffff, 0x4e7b),
 new opcode_handler_struct(m68000_movem_pd_16 , 13, 0xfff8, 0x48a0),
 new opcode_handler_struct(m68000_movem_pd_32 , 13, 0xfff8, 0x48e0),
 new opcode_handler_struct(m68000_movem_pi_16 , 13, 0xfff8, 0x4c98),
 new opcode_handler_struct(m68000_movem_pi_32 , 13, 0xfff8, 0x4cd8),
 new opcode_handler_struct(m68000_movem_re_ai_16 , 13, 0xfff8, 0x4890),
 new opcode_handler_struct(m68000_movem_re_di_16 , 13, 0xfff8, 0x48a8),
 new opcode_handler_struct(m68000_movem_re_ix_16 , 13, 0xfff8, 0x48b0),
 new opcode_handler_struct(m68000_movem_re_aw_16 , 16, 0xffff, 0x48b8),
 new opcode_handler_struct(m68000_movem_re_al_16 , 16, 0xffff, 0x48b9),
 new opcode_handler_struct(m68000_movem_re_ai_32 , 13, 0xfff8, 0x48d0),
 new opcode_handler_struct(m68000_movem_re_di_32 , 13, 0xfff8, 0x48e8),
 new opcode_handler_struct(m68000_movem_re_ix_32 , 13, 0xfff8, 0x48f0),
 new opcode_handler_struct(m68000_movem_re_aw_32 , 16, 0xffff, 0x48f8),
 new opcode_handler_struct(m68000_movem_re_al_32 , 16, 0xffff, 0x48f9),
 new opcode_handler_struct(m68000_movem_er_ai_16 , 13, 0xfff8, 0x4c90),
 new opcode_handler_struct(m68000_movem_er_di_16 , 13, 0xfff8, 0x4ca8),
 new opcode_handler_struct(m68000_movem_er_ix_16 , 13, 0xfff8, 0x4cb0),
 new opcode_handler_struct(m68000_movem_er_aw_16 , 16, 0xffff, 0x4cb8),
 new opcode_handler_struct(m68000_movem_er_al_16 , 16, 0xffff, 0x4cb9),
 new opcode_handler_struct(m68000_movem_er_pcdi_16 , 16, 0xffff, 0x4cba),
 new opcode_handler_struct(m68000_movem_er_pcix_16 , 16, 0xffff, 0x4cbb),
 new opcode_handler_struct(m68000_movem_er_ai_32 , 13, 0xfff8, 0x4cd0),
 new opcode_handler_struct(m68000_movem_er_di_32 , 13, 0xfff8, 0x4ce8),
 new opcode_handler_struct(m68000_movem_er_ix_32 , 13, 0xfff8, 0x4cf0),
 new opcode_handler_struct(m68000_movem_er_aw_32 , 16, 0xffff, 0x4cf8),
 new opcode_handler_struct(m68000_movem_er_al_32 , 16, 0xffff, 0x4cf9),
 new opcode_handler_struct(m68000_movem_er_pcdi_32 , 16, 0xffff, 0x4cfa),
 new opcode_handler_struct(m68000_movem_er_pcix_32 , 16, 0xffff, 0x4cfb),
 new opcode_handler_struct(m68000_movep_er_16 , 10, 0xf1f8, 0x0108),
 new opcode_handler_struct(m68000_movep_er_32 , 10, 0xf1f8, 0x0148),
 new opcode_handler_struct(m68000_movep_re_16 , 10, 0xf1f8, 0x0188),
 new opcode_handler_struct(m68000_movep_re_32 , 10, 0xf1f8, 0x01c8),
 new opcode_handler_struct(m68010_moves_ai_8 , 13, 0xfff8, 0x0e10),
 new opcode_handler_struct(m68010_moves_pi_8 , 13, 0xfff8, 0x0e18),
 new opcode_handler_struct(m68010_moves_pi7_8 , 16, 0xffff, 0x0e1f),
 new opcode_handler_struct(m68010_moves_pd_8 , 13, 0xfff8, 0x0e20),
 new opcode_handler_struct(m68010_moves_pd7_8 , 16, 0xffff, 0x0e27),
 new opcode_handler_struct(m68010_moves_di_8 , 13, 0xfff8, 0x0e28),
 new opcode_handler_struct(m68010_moves_ix_8 , 13, 0xfff8, 0x0e30),
 new opcode_handler_struct(m68010_moves_aw_8 , 16, 0xffff, 0x0e38),
 new opcode_handler_struct(m68010_moves_al_8 , 16, 0xffff, 0x0e39),
 new opcode_handler_struct(m68010_moves_ai_16 , 13, 0xfff8, 0x0e50),
 new opcode_handler_struct(m68010_moves_pi_16 , 13, 0xfff8, 0x0e58),
 new opcode_handler_struct(m68010_moves_pd_16 , 13, 0xfff8, 0x0e60),
 new opcode_handler_struct(m68010_moves_di_16 , 13, 0xfff8, 0x0e68),
 new opcode_handler_struct(m68010_moves_ix_16 , 13, 0xfff8, 0x0e70),
 new opcode_handler_struct(m68010_moves_aw_16 , 16, 0xffff, 0x0e78),
 new opcode_handler_struct(m68010_moves_al_16 , 16, 0xffff, 0x0e79),
 new opcode_handler_struct(m68010_moves_ai_32 , 13, 0xfff8, 0x0e90),
 new opcode_handler_struct(m68010_moves_pi_32 , 13, 0xfff8, 0x0e98),
 new opcode_handler_struct(m68010_moves_pd_32 , 13, 0xfff8, 0x0ea0),
 new opcode_handler_struct(m68010_moves_di_32 , 13, 0xfff8, 0x0ea8),
 new opcode_handler_struct(m68010_moves_ix_32 , 13, 0xfff8, 0x0eb0),
 new opcode_handler_struct(m68010_moves_aw_32 , 16, 0xffff, 0x0eb8),
 new opcode_handler_struct(m68010_moves_al_32 , 16, 0xffff, 0x0eb9),
 new opcode_handler_struct(m68000_moveq , 5, 0xf100, 0x7000),
 new opcode_handler_struct(m68000_muls_d_16 , 10, 0xf1f8, 0xc1c0),
 new opcode_handler_struct(m68000_muls_ai_16 , 10, 0xf1f8, 0xc1d0),
 new opcode_handler_struct(m68000_muls_pi_16 , 10, 0xf1f8, 0xc1d8),
 new opcode_handler_struct(m68000_muls_pd_16 , 10, 0xf1f8, 0xc1e0),
 new opcode_handler_struct(m68000_muls_di_16 , 10, 0xf1f8, 0xc1e8),
 new opcode_handler_struct(m68000_muls_ix_16 , 10, 0xf1f8, 0xc1f0),
 new opcode_handler_struct(m68000_muls_aw_16 , 13, 0xf1ff, 0xc1f8),
 new opcode_handler_struct(m68000_muls_al_16 , 13, 0xf1ff, 0xc1f9),
 new opcode_handler_struct(m68000_muls_pcdi_16 , 13, 0xf1ff, 0xc1fa),
 new opcode_handler_struct(m68000_muls_pcix_16 , 13, 0xf1ff, 0xc1fb),
 new opcode_handler_struct(m68000_muls_i_16 , 13, 0xf1ff, 0xc1fc),
 new opcode_handler_struct(m68000_mulu_d_16 , 10, 0xf1f8, 0xc0c0),
 new opcode_handler_struct(m68000_mulu_ai_16 , 10, 0xf1f8, 0xc0d0),
 new opcode_handler_struct(m68000_mulu_pi_16 , 10, 0xf1f8, 0xc0d8),
 new opcode_handler_struct(m68000_mulu_pd_16 , 10, 0xf1f8, 0xc0e0),
 new opcode_handler_struct(m68000_mulu_di_16 , 10, 0xf1f8, 0xc0e8),
 new opcode_handler_struct(m68000_mulu_ix_16 , 10, 0xf1f8, 0xc0f0),
 new opcode_handler_struct(m68000_mulu_aw_16 , 13, 0xf1ff, 0xc0f8),
 new opcode_handler_struct(m68000_mulu_al_16 , 13, 0xf1ff, 0xc0f9),
 new opcode_handler_struct(m68000_mulu_pcdi_16 , 13, 0xf1ff, 0xc0fa),
 new opcode_handler_struct(m68000_mulu_pcix_16 , 13, 0xf1ff, 0xc0fb),
 new opcode_handler_struct(m68000_mulu_i_16 , 13, 0xf1ff, 0xc0fc),
 new opcode_handler_struct(m68020_mull_d_32 , 13, 0xfff8, 0x4c00),
 new opcode_handler_struct(m68020_mull_ai_32 , 13, 0xfff8, 0x4c10),
 new opcode_handler_struct(m68020_mull_pi_32 , 13, 0xfff8, 0x4c18),
 new opcode_handler_struct(m68020_mull_pd_32 , 13, 0xfff8, 0x4c20),
 new opcode_handler_struct(m68020_mull_di_32 , 13, 0xfff8, 0x4c28),
 new opcode_handler_struct(m68020_mull_ix_32 , 13, 0xfff8, 0x4c30),
 new opcode_handler_struct(m68020_mull_aw_32 , 16, 0xffff, 0x4c38),
 new opcode_handler_struct(m68020_mull_al_32 , 16, 0xffff, 0x4c39),
 new opcode_handler_struct(m68020_mull_pcdi_32 , 16, 0xffff, 0x4c3a),
 new opcode_handler_struct(m68020_mull_pcix_32 , 16, 0xffff, 0x4c3b),
 new opcode_handler_struct(m68020_mull_i_32 , 16, 0xffff, 0x4c3c),
 new opcode_handler_struct(m68000_nbcd_d , 13, 0xfff8, 0x4800),
 new opcode_handler_struct(m68000_nbcd_ai , 13, 0xfff8, 0x4810),
 new opcode_handler_struct(m68000_nbcd_pi , 13, 0xfff8, 0x4818),
 new opcode_handler_struct(m68000_nbcd_pi7 , 16, 0xffff, 0x481f),
 new opcode_handler_struct(m68000_nbcd_pd , 13, 0xfff8, 0x4820),
 new opcode_handler_struct(m68000_nbcd_pd7 , 16, 0xffff, 0x4827),
 new opcode_handler_struct(m68000_nbcd_di , 13, 0xfff8, 0x4828),
 new opcode_handler_struct(m68000_nbcd_ix , 13, 0xfff8, 0x4830),
 new opcode_handler_struct(m68000_nbcd_aw , 16, 0xffff, 0x4838),
 new opcode_handler_struct(m68000_nbcd_al , 16, 0xffff, 0x4839),
 new opcode_handler_struct(m68000_neg_d_8 , 13, 0xfff8, 0x4400),
 new opcode_handler_struct(m68000_neg_ai_8 , 13, 0xfff8, 0x4410),
 new opcode_handler_struct(m68000_neg_pi_8 , 13, 0xfff8, 0x4418),
 new opcode_handler_struct(m68000_neg_pi7_8 , 16, 0xffff, 0x441f),
 new opcode_handler_struct(m68000_neg_pd_8 , 13, 0xfff8, 0x4420),
 new opcode_handler_struct(m68000_neg_pd7_8 , 16, 0xffff, 0x4427),
 new opcode_handler_struct(m68000_neg_di_8 , 13, 0xfff8, 0x4428),
 new opcode_handler_struct(m68000_neg_ix_8 , 13, 0xfff8, 0x4430),
 new opcode_handler_struct(m68000_neg_aw_8 , 16, 0xffff, 0x4438),
 new opcode_handler_struct(m68000_neg_al_8 , 16, 0xffff, 0x4439),
 new opcode_handler_struct(m68000_neg_d_16 , 13, 0xfff8, 0x4440),
 new opcode_handler_struct(m68000_neg_ai_16 , 13, 0xfff8, 0x4450),
 new opcode_handler_struct(m68000_neg_pi_16 , 13, 0xfff8, 0x4458),
 new opcode_handler_struct(m68000_neg_pd_16 , 13, 0xfff8, 0x4460),
 new opcode_handler_struct(m68000_neg_di_16 , 13, 0xfff8, 0x4468),
 new opcode_handler_struct(m68000_neg_ix_16 , 13, 0xfff8, 0x4470),
 new opcode_handler_struct(m68000_neg_aw_16 , 16, 0xffff, 0x4478),
 new opcode_handler_struct(m68000_neg_al_16 , 16, 0xffff, 0x4479),
 new opcode_handler_struct(m68000_neg_d_32 , 13, 0xfff8, 0x4480),
 new opcode_handler_struct(m68000_neg_ai_32 , 13, 0xfff8, 0x4490),
 new opcode_handler_struct(m68000_neg_pi_32 , 13, 0xfff8, 0x4498),
 new opcode_handler_struct(m68000_neg_pd_32 , 13, 0xfff8, 0x44a0),
 new opcode_handler_struct(m68000_neg_di_32 , 13, 0xfff8, 0x44a8),
 new opcode_handler_struct(m68000_neg_ix_32 , 13, 0xfff8, 0x44b0),
 new opcode_handler_struct(m68000_neg_aw_32 , 16, 0xffff, 0x44b8),
 new opcode_handler_struct(m68000_neg_al_32 , 16, 0xffff, 0x44b9),
 new opcode_handler_struct(m68000_negx_d_8 , 13, 0xfff8, 0x4000),
 new opcode_handler_struct(m68000_negx_ai_8 , 13, 0xfff8, 0x4010),
 new opcode_handler_struct(m68000_negx_pi_8 , 13, 0xfff8, 0x4018),
 new opcode_handler_struct(m68000_negx_pi7_8 , 16, 0xffff, 0x401f),
 new opcode_handler_struct(m68000_negx_pd_8 , 13, 0xfff8, 0x4020),
 new opcode_handler_struct(m68000_negx_pd7_8 , 16, 0xffff, 0x4027),
 new opcode_handler_struct(m68000_negx_di_8 , 13, 0xfff8, 0x4028),
 new opcode_handler_struct(m68000_negx_ix_8 , 13, 0xfff8, 0x4030),
 new opcode_handler_struct(m68000_negx_aw_8 , 16, 0xffff, 0x4038),
 new opcode_handler_struct(m68000_negx_al_8 , 16, 0xffff, 0x4039),
 new opcode_handler_struct(m68000_negx_d_16 , 13, 0xfff8, 0x4040),
 new opcode_handler_struct(m68000_negx_ai_16 , 13, 0xfff8, 0x4050),
 new opcode_handler_struct(m68000_negx_pi_16 , 13, 0xfff8, 0x4058),
 new opcode_handler_struct(m68000_negx_pd_16 , 13, 0xfff8, 0x4060),
 new opcode_handler_struct(m68000_negx_di_16 , 13, 0xfff8, 0x4068),
 new opcode_handler_struct(m68000_negx_ix_16 , 13, 0xfff8, 0x4070),
 new opcode_handler_struct(m68000_negx_aw_16 , 16, 0xffff, 0x4078),
 new opcode_handler_struct(m68000_negx_al_16 , 16, 0xffff, 0x4079),
 new opcode_handler_struct(m68000_negx_d_32 , 13, 0xfff8, 0x4080),
 new opcode_handler_struct(m68000_negx_ai_32 , 13, 0xfff8, 0x4090),
 new opcode_handler_struct(m68000_negx_pi_32 , 13, 0xfff8, 0x4098),
 new opcode_handler_struct(m68000_negx_pd_32 , 13, 0xfff8, 0x40a0),
 new opcode_handler_struct(m68000_negx_di_32 , 13, 0xfff8, 0x40a8),
 new opcode_handler_struct(m68000_negx_ix_32 , 13, 0xfff8, 0x40b0),
 new opcode_handler_struct(m68000_negx_aw_32 , 16, 0xffff, 0x40b8),
 new opcode_handler_struct(m68000_negx_al_32 , 16, 0xffff, 0x40b9),
 new opcode_handler_struct(m68000_nop , 16, 0xffff, 0x4e71),
 new opcode_handler_struct(m68000_not_d_8 , 13, 0xfff8, 0x4600),
 new opcode_handler_struct(m68000_not_ai_8 , 13, 0xfff8, 0x4610),
 new opcode_handler_struct(m68000_not_pi_8 , 13, 0xfff8, 0x4618),
 new opcode_handler_struct(m68000_not_pi7_8 , 16, 0xffff, 0x461f),
 new opcode_handler_struct(m68000_not_pd_8 , 13, 0xfff8, 0x4620),
 new opcode_handler_struct(m68000_not_pd7_8 , 16, 0xffff, 0x4627),
 new opcode_handler_struct(m68000_not_di_8 , 13, 0xfff8, 0x4628),
 new opcode_handler_struct(m68000_not_ix_8 , 13, 0xfff8, 0x4630),
 new opcode_handler_struct(m68000_not_aw_8 , 16, 0xffff, 0x4638),
 new opcode_handler_struct(m68000_not_al_8 , 16, 0xffff, 0x4639),
 new opcode_handler_struct(m68000_not_d_16 , 13, 0xfff8, 0x4640),
 new opcode_handler_struct(m68000_not_ai_16 , 13, 0xfff8, 0x4650),
 new opcode_handler_struct(m68000_not_pi_16 , 13, 0xfff8, 0x4658),
 new opcode_handler_struct(m68000_not_pd_16 , 13, 0xfff8, 0x4660),
 new opcode_handler_struct(m68000_not_di_16 , 13, 0xfff8, 0x4668),
 new opcode_handler_struct(m68000_not_ix_16 , 13, 0xfff8, 0x4670),
 new opcode_handler_struct(m68000_not_aw_16 , 16, 0xffff, 0x4678),
 new opcode_handler_struct(m68000_not_al_16 , 16, 0xffff, 0x4679),
 new opcode_handler_struct(m68000_not_d_32 , 13, 0xfff8, 0x4680),
 new opcode_handler_struct(m68000_not_ai_32 , 13, 0xfff8, 0x4690),
 new opcode_handler_struct(m68000_not_pi_32 , 13, 0xfff8, 0x4698),
 new opcode_handler_struct(m68000_not_pd_32 , 13, 0xfff8, 0x46a0),
 new opcode_handler_struct(m68000_not_di_32 , 13, 0xfff8, 0x46a8),
 new opcode_handler_struct(m68000_not_ix_32 , 13, 0xfff8, 0x46b0),
 new opcode_handler_struct(m68000_not_aw_32 , 16, 0xffff, 0x46b8),
 new opcode_handler_struct(m68000_not_al_32 , 16, 0xffff, 0x46b9),
 new opcode_handler_struct(m68000_or_er_d_8 , 10, 0xf1f8, 0x8000),
 new opcode_handler_struct(m68000_or_er_ai_8 , 10, 0xf1f8, 0x8010),
 new opcode_handler_struct(m68000_or_er_pi_8 , 10, 0xf1f8, 0x8018),
 new opcode_handler_struct(m68000_or_er_pi7_8 , 13, 0xf1ff, 0x801f),
 new opcode_handler_struct(m68000_or_er_pd_8 , 10, 0xf1f8, 0x8020),
 new opcode_handler_struct(m68000_or_er_pd7_8 , 13, 0xf1ff, 0x8027),
 new opcode_handler_struct(m68000_or_er_di_8 , 10, 0xf1f8, 0x8028),
 new opcode_handler_struct(m68000_or_er_ix_8 , 10, 0xf1f8, 0x8030),
 new opcode_handler_struct(m68000_or_er_aw_8 , 13, 0xf1ff, 0x8038),
 new opcode_handler_struct(m68000_or_er_al_8 , 13, 0xf1ff, 0x8039),
 new opcode_handler_struct(m68000_or_er_pcdi_8 , 13, 0xf1ff, 0x803a),
 new opcode_handler_struct(m68000_or_er_pcix_8 , 13, 0xf1ff, 0x803b),
 new opcode_handler_struct(m68000_or_er_i_8 , 13, 0xf1ff, 0x803c),
 new opcode_handler_struct(m68000_or_er_d_16 , 10, 0xf1f8, 0x8040),
 new opcode_handler_struct(m68000_or_er_ai_16 , 10, 0xf1f8, 0x8050),
 new opcode_handler_struct(m68000_or_er_pi_16 , 10, 0xf1f8, 0x8058),
 new opcode_handler_struct(m68000_or_er_pd_16 , 10, 0xf1f8, 0x8060),
 new opcode_handler_struct(m68000_or_er_di_16 , 10, 0xf1f8, 0x8068),
 new opcode_handler_struct(m68000_or_er_ix_16 , 10, 0xf1f8, 0x8070),
 new opcode_handler_struct(m68000_or_er_aw_16 , 13, 0xf1ff, 0x8078),
 new opcode_handler_struct(m68000_or_er_al_16 , 13, 0xf1ff, 0x8079),
 new opcode_handler_struct(m68000_or_er_pcdi_16 , 13, 0xf1ff, 0x807a),
 new opcode_handler_struct(m68000_or_er_pcix_16 , 13, 0xf1ff, 0x807b),
 new opcode_handler_struct(m68000_or_er_i_16 , 13, 0xf1ff, 0x807c),
 new opcode_handler_struct(m68000_or_er_d_32 , 10, 0xf1f8, 0x8080),
 new opcode_handler_struct(m68000_or_er_ai_32 , 10, 0xf1f8, 0x8090),
 new opcode_handler_struct(m68000_or_er_pi_32 , 10, 0xf1f8, 0x8098),
 new opcode_handler_struct(m68000_or_er_pd_32 , 10, 0xf1f8, 0x80a0),
 new opcode_handler_struct(m68000_or_er_di_32 , 10, 0xf1f8, 0x80a8),
 new opcode_handler_struct(m68000_or_er_ix_32 , 10, 0xf1f8, 0x80b0),
 new opcode_handler_struct(m68000_or_er_aw_32 , 13, 0xf1ff, 0x80b8),
 new opcode_handler_struct(m68000_or_er_al_32 , 13, 0xf1ff, 0x80b9),
 new opcode_handler_struct(m68000_or_er_pcdi_32 , 13, 0xf1ff, 0x80ba),
 new opcode_handler_struct(m68000_or_er_pcix_32 , 13, 0xf1ff, 0x80bb),
 new opcode_handler_struct(m68000_or_er_i_32 , 13, 0xf1ff, 0x80bc),
 new opcode_handler_struct(m68000_or_re_ai_8 , 10, 0xf1f8, 0x8110),
 new opcode_handler_struct(m68000_or_re_pi_8 , 10, 0xf1f8, 0x8118),
 new opcode_handler_struct(m68000_or_re_pi7_8 , 13, 0xf1ff, 0x811f),
 new opcode_handler_struct(m68000_or_re_pd_8 , 10, 0xf1f8, 0x8120),
 new opcode_handler_struct(m68000_or_re_pd7_8 , 13, 0xf1ff, 0x8127),
 new opcode_handler_struct(m68000_or_re_di_8 , 10, 0xf1f8, 0x8128),
 new opcode_handler_struct(m68000_or_re_ix_8 , 10, 0xf1f8, 0x8130),
 new opcode_handler_struct(m68000_or_re_aw_8 , 13, 0xf1ff, 0x8138),
 new opcode_handler_struct(m68000_or_re_al_8 , 13, 0xf1ff, 0x8139),
 new opcode_handler_struct(m68000_or_re_ai_16 , 10, 0xf1f8, 0x8150),
 new opcode_handler_struct(m68000_or_re_pi_16 , 10, 0xf1f8, 0x8158),
 new opcode_handler_struct(m68000_or_re_pd_16 , 10, 0xf1f8, 0x8160),
 new opcode_handler_struct(m68000_or_re_di_16 , 10, 0xf1f8, 0x8168),
 new opcode_handler_struct(m68000_or_re_ix_16 , 10, 0xf1f8, 0x8170),
 new opcode_handler_struct(m68000_or_re_aw_16 , 13, 0xf1ff, 0x8178),
 new opcode_handler_struct(m68000_or_re_al_16 , 13, 0xf1ff, 0x8179),
 new opcode_handler_struct(m68000_or_re_ai_32 , 10, 0xf1f8, 0x8190),
 new opcode_handler_struct(m68000_or_re_pi_32 , 10, 0xf1f8, 0x8198),
 new opcode_handler_struct(m68000_or_re_pd_32 , 10, 0xf1f8, 0x81a0),
 new opcode_handler_struct(m68000_or_re_di_32 , 10, 0xf1f8, 0x81a8),
 new opcode_handler_struct(m68000_or_re_ix_32 , 10, 0xf1f8, 0x81b0),
 new opcode_handler_struct(m68000_or_re_aw_32 , 13, 0xf1ff, 0x81b8),
 new opcode_handler_struct(m68000_or_re_al_32 , 13, 0xf1ff, 0x81b9),
 new opcode_handler_struct(m68000_ori_to_ccr , 16, 0xffff, 0x003c),
 new opcode_handler_struct(m68000_ori_to_sr , 16, 0xffff, 0x007c),
 new opcode_handler_struct(m68000_ori_d_8 , 13, 0xfff8, 0x0000),
 new opcode_handler_struct(m68000_ori_ai_8 , 13, 0xfff8, 0x0010),
 new opcode_handler_struct(m68000_ori_pi_8 , 13, 0xfff8, 0x0018),
 new opcode_handler_struct(m68000_ori_pi7_8 , 16, 0xffff, 0x001f),
 new opcode_handler_struct(m68000_ori_pd_8 , 13, 0xfff8, 0x0020),
 new opcode_handler_struct(m68000_ori_pd7_8 , 16, 0xffff, 0x0027),
 new opcode_handler_struct(m68000_ori_di_8 , 13, 0xfff8, 0x0028),
 new opcode_handler_struct(m68000_ori_ix_8 , 13, 0xfff8, 0x0030),
 new opcode_handler_struct(m68000_ori_aw_8 , 16, 0xffff, 0x0038),
 new opcode_handler_struct(m68000_ori_al_8 , 16, 0xffff, 0x0039),
 new opcode_handler_struct(m68000_ori_d_16 , 13, 0xfff8, 0x0040),
 new opcode_handler_struct(m68000_ori_ai_16 , 13, 0xfff8, 0x0050),
 new opcode_handler_struct(m68000_ori_pi_16 , 13, 0xfff8, 0x0058),
 new opcode_handler_struct(m68000_ori_pd_16 , 13, 0xfff8, 0x0060),
 new opcode_handler_struct(m68000_ori_di_16 , 13, 0xfff8, 0x0068),
 new opcode_handler_struct(m68000_ori_ix_16 , 13, 0xfff8, 0x0070),
 new opcode_handler_struct(m68000_ori_aw_16 , 16, 0xffff, 0x0078),
 new opcode_handler_struct(m68000_ori_al_16 , 16, 0xffff, 0x0079),
 new opcode_handler_struct(m68000_ori_d_32 , 13, 0xfff8, 0x0080),
 new opcode_handler_struct(m68000_ori_ai_32 , 13, 0xfff8, 0x0090),
 new opcode_handler_struct(m68000_ori_pi_32 , 13, 0xfff8, 0x0098),
 new opcode_handler_struct(m68000_ori_pd_32 , 13, 0xfff8, 0x00a0),
 new opcode_handler_struct(m68000_ori_di_32 , 13, 0xfff8, 0x00a8),
 new opcode_handler_struct(m68000_ori_ix_32 , 13, 0xfff8, 0x00b0),
 new opcode_handler_struct(m68000_ori_aw_32 , 16, 0xffff, 0x00b8),
 new opcode_handler_struct(m68000_ori_al_32 , 16, 0xffff, 0x00b9),
 new opcode_handler_struct(m68020_pack_rr , 10, 0xf1f8, 0x8140),
 new opcode_handler_struct(m68020_pack_mm_ax7 , 13, 0xf1ff, 0x814f),
 new opcode_handler_struct(m68020_pack_mm_ay7 , 13, 0xfff8, 0x8f48),
 new opcode_handler_struct(m68020_pack_mm_axy7 , 16, 0xffff, 0x8f4f),
 new opcode_handler_struct(m68020_pack_mm , 10, 0xf1f8, 0x8148),
 new opcode_handler_struct(m68000_pea_ai , 13, 0xfff8, 0x4850),
 new opcode_handler_struct(m68000_pea_di , 13, 0xfff8, 0x4868),
 new opcode_handler_struct(m68000_pea_ix , 13, 0xfff8, 0x4870),
 new opcode_handler_struct(m68000_pea_aw , 16, 0xffff, 0x4878),
 new opcode_handler_struct(m68000_pea_al , 16, 0xffff, 0x4879),
 new opcode_handler_struct(m68000_pea_pcdi , 16, 0xffff, 0x487a),
 new opcode_handler_struct(m68000_pea_pcix , 16, 0xffff, 0x487b),
 new opcode_handler_struct(m68000_rst , 16, 0xffff, 0x4e70),
 new opcode_handler_struct(m68000_ror_s_8 , 10, 0xf1f8, 0xe018),
 new opcode_handler_struct(m68000_ror_s_16 , 10, 0xf1f8, 0xe058),
 new opcode_handler_struct(m68000_ror_s_32 , 10, 0xf1f8, 0xe098),
 new opcode_handler_struct(m68000_ror_r_8 , 10, 0xf1f8, 0xe038),
 new opcode_handler_struct(m68000_ror_r_16 , 10, 0xf1f8, 0xe078),
 new opcode_handler_struct(m68000_ror_r_32 , 10, 0xf1f8, 0xe0b8),
 new opcode_handler_struct(m68000_ror_ea_ai , 13, 0xfff8, 0xe6d0),
 new opcode_handler_struct(m68000_ror_ea_pi , 13, 0xfff8, 0xe6d8),
 new opcode_handler_struct(m68000_ror_ea_pd , 13, 0xfff8, 0xe6e0),
 new opcode_handler_struct(m68000_ror_ea_di , 13, 0xfff8, 0xe6e8),
 new opcode_handler_struct(m68000_ror_ea_ix , 13, 0xfff8, 0xe6f0),
 new opcode_handler_struct(m68000_ror_ea_aw , 16, 0xffff, 0xe6f8),
 new opcode_handler_struct(m68000_ror_ea_al , 16, 0xffff, 0xe6f9),
 new opcode_handler_struct(m68000_rol_s_8 , 10, 0xf1f8, 0xe118),
 new opcode_handler_struct(m68000_rol_s_16 , 10, 0xf1f8, 0xe158),
 new opcode_handler_struct(m68000_rol_s_32 , 10, 0xf1f8, 0xe198),
 new opcode_handler_struct(m68000_rol_r_8 , 10, 0xf1f8, 0xe138),
 new opcode_handler_struct(m68000_rol_r_16 , 10, 0xf1f8, 0xe178),
 new opcode_handler_struct(m68000_rol_r_32 , 10, 0xf1f8, 0xe1b8),
 new opcode_handler_struct(m68000_rol_ea_ai , 13, 0xfff8, 0xe7d0),
 new opcode_handler_struct(m68000_rol_ea_pi , 13, 0xfff8, 0xe7d8),
 new opcode_handler_struct(m68000_rol_ea_pd , 13, 0xfff8, 0xe7e0),
 new opcode_handler_struct(m68000_rol_ea_di , 13, 0xfff8, 0xe7e8),
 new opcode_handler_struct(m68000_rol_ea_ix , 13, 0xfff8, 0xe7f0),
 new opcode_handler_struct(m68000_rol_ea_aw , 16, 0xffff, 0xe7f8),
 new opcode_handler_struct(m68000_rol_ea_al , 16, 0xffff, 0xe7f9),
 new opcode_handler_struct(m68000_roxr_s_8 , 10, 0xf1f8, 0xe010),
 new opcode_handler_struct(m68000_roxr_s_16 , 10, 0xf1f8, 0xe050),
 new opcode_handler_struct(m68000_roxr_s_32 , 10, 0xf1f8, 0xe090),
 new opcode_handler_struct(m68000_roxr_r_8 , 10, 0xf1f8, 0xe030),
 new opcode_handler_struct(m68000_roxr_r_16 , 10, 0xf1f8, 0xe070),
 new opcode_handler_struct(m68000_roxr_r_32 , 10, 0xf1f8, 0xe0b0),
 new opcode_handler_struct(m68000_roxr_ea_ai , 13, 0xfff8, 0xe4d0),
 new opcode_handler_struct(m68000_roxr_ea_pi , 13, 0xfff8, 0xe4d8),
 new opcode_handler_struct(m68000_roxr_ea_pd , 13, 0xfff8, 0xe4e0),
 new opcode_handler_struct(m68000_roxr_ea_di , 13, 0xfff8, 0xe4e8),
 new opcode_handler_struct(m68000_roxr_ea_ix , 13, 0xfff8, 0xe4f0),
 new opcode_handler_struct(m68000_roxr_ea_aw , 16, 0xffff, 0xe4f8),
 new opcode_handler_struct(m68000_roxr_ea_al , 16, 0xffff, 0xe4f9),
 new opcode_handler_struct(m68000_roxl_s_8 , 10, 0xf1f8, 0xe110),
 new opcode_handler_struct(m68000_roxl_s_16 , 10, 0xf1f8, 0xe150),
 new opcode_handler_struct(m68000_roxl_s_32 , 10, 0xf1f8, 0xe190),
 new opcode_handler_struct(m68000_roxl_r_8 , 10, 0xf1f8, 0xe130),
 new opcode_handler_struct(m68000_roxl_r_16 , 10, 0xf1f8, 0xe170),
 new opcode_handler_struct(m68000_roxl_r_32 , 10, 0xf1f8, 0xe1b0),
 new opcode_handler_struct(m68000_roxl_ea_ai , 13, 0xfff8, 0xe5d0),
 new opcode_handler_struct(m68000_roxl_ea_pi , 13, 0xfff8, 0xe5d8),
 new opcode_handler_struct(m68000_roxl_ea_pd , 13, 0xfff8, 0xe5e0),
 new opcode_handler_struct(m68000_roxl_ea_di , 13, 0xfff8, 0xe5e8),
 new opcode_handler_struct(m68000_roxl_ea_ix , 13, 0xfff8, 0xe5f0),
 new opcode_handler_struct(m68000_roxl_ea_aw , 16, 0xffff, 0xe5f8),
 new opcode_handler_struct(m68000_roxl_ea_al , 16, 0xffff, 0xe5f9),
 new opcode_handler_struct(m68010_rtd , 16, 0xffff, 0x4e74),
 new opcode_handler_struct(m68000_rte , 16, 0xffff, 0x4e73),
 new opcode_handler_struct(m68020_rtm , 12, 0xfff0, 0x06c0),
 new opcode_handler_struct(m68000_rtr , 16, 0xffff, 0x4e77),
 new opcode_handler_struct(m68000_rts , 16, 0xffff, 0x4e75),
 new opcode_handler_struct(m68000_sbcd_rr , 10, 0xf1f8, 0x8100),
 new opcode_handler_struct(m68000_sbcd_mm_ax7 , 13, 0xfff8, 0x8f08),
 new opcode_handler_struct(m68000_sbcd_mm_ay7 , 13, 0xf1ff, 0x810f),
 new opcode_handler_struct(m68000_sbcd_mm_axy7 , 16, 0xffff, 0x8f0f),
 new opcode_handler_struct(m68000_sbcd_mm , 10, 0xf1f8, 0x8108),
 new opcode_handler_struct(m68000_st_d , 13, 0xfff8, 0x50c0),
 new opcode_handler_struct(m68000_st_ai , 13, 0xfff8, 0x50d0),
 new opcode_handler_struct(m68000_st_pi , 13, 0xfff8, 0x50d8),
 new opcode_handler_struct(m68000_st_pi7 , 16, 0xffff, 0x50df),
 new opcode_handler_struct(m68000_st_pd , 13, 0xfff8, 0x50e0),
 new opcode_handler_struct(m68000_st_pd7 , 16, 0xffff, 0x50e7),
 new opcode_handler_struct(m68000_st_di , 13, 0xfff8, 0x50e8),
 new opcode_handler_struct(m68000_st_ix , 13, 0xfff8, 0x50f0),
 new opcode_handler_struct(m68000_st_aw , 16, 0xffff, 0x50f8),
 new opcode_handler_struct(m68000_st_al , 16, 0xffff, 0x50f9),
 new opcode_handler_struct(m68000_sf_d , 13, 0xfff8, 0x51c0),
 new opcode_handler_struct(m68000_sf_ai , 13, 0xfff8, 0x51d0),
 new opcode_handler_struct(m68000_sf_pi , 13, 0xfff8, 0x51d8),
 new opcode_handler_struct(m68000_sf_pi7 , 16, 0xffff, 0x51df),
 new opcode_handler_struct(m68000_sf_pd , 13, 0xfff8, 0x51e0),
 new opcode_handler_struct(m68000_sf_pd7 , 16, 0xffff, 0x51e7),
 new opcode_handler_struct(m68000_sf_di , 13, 0xfff8, 0x51e8),
 new opcode_handler_struct(m68000_sf_ix , 13, 0xfff8, 0x51f0),
 new opcode_handler_struct(m68000_sf_aw , 16, 0xffff, 0x51f8),
 new opcode_handler_struct(m68000_sf_al , 16, 0xffff, 0x51f9),
 new opcode_handler_struct(m68000_shi_d , 13, 0xfff8, 0x52c0),
 new opcode_handler_struct(m68000_shi_ai , 13, 0xfff8, 0x52d0),
 new opcode_handler_struct(m68000_shi_pi , 13, 0xfff8, 0x52d8),
 new opcode_handler_struct(m68000_shi_pi7 , 16, 0xffff, 0x52df),
 new opcode_handler_struct(m68000_shi_pd , 13, 0xfff8, 0x52e0),
 new opcode_handler_struct(m68000_shi_pd7 , 16, 0xffff, 0x52e7),
 new opcode_handler_struct(m68000_shi_di , 13, 0xfff8, 0x52e8),
 new opcode_handler_struct(m68000_shi_ix , 13, 0xfff8, 0x52f0),
 new opcode_handler_struct(m68000_shi_aw , 16, 0xffff, 0x52f8),
 new opcode_handler_struct(m68000_shi_al , 16, 0xffff, 0x52f9),
 new opcode_handler_struct(m68000_sls_d , 13, 0xfff8, 0x53c0),
 new opcode_handler_struct(m68000_sls_ai , 13, 0xfff8, 0x53d0),
 new opcode_handler_struct(m68000_sls_pi , 13, 0xfff8, 0x53d8),
 new opcode_handler_struct(m68000_sls_pi7 , 16, 0xffff, 0x53df),
 new opcode_handler_struct(m68000_sls_pd , 13, 0xfff8, 0x53e0),
 new opcode_handler_struct(m68000_sls_pd7 , 16, 0xffff, 0x53e7),
 new opcode_handler_struct(m68000_sls_di , 13, 0xfff8, 0x53e8),
 new opcode_handler_struct(m68000_sls_ix , 13, 0xfff8, 0x53f0),
 new opcode_handler_struct(m68000_sls_aw , 16, 0xffff, 0x53f8),
 new opcode_handler_struct(m68000_sls_al , 16, 0xffff, 0x53f9),
 new opcode_handler_struct(m68000_scc_d , 13, 0xfff8, 0x54c0),
 new opcode_handler_struct(m68000_scc_ai , 13, 0xfff8, 0x54d0),
 new opcode_handler_struct(m68000_scc_pi , 13, 0xfff8, 0x54d8),
 new opcode_handler_struct(m68000_scc_pi7 , 16, 0xffff, 0x54df),
 new opcode_handler_struct(m68000_scc_pd , 13, 0xfff8, 0x54e0),
 new opcode_handler_struct(m68000_scc_pd7 , 16, 0xffff, 0x54e7),
 new opcode_handler_struct(m68000_scc_di , 13, 0xfff8, 0x54e8),
 new opcode_handler_struct(m68000_scc_ix , 13, 0xfff8, 0x54f0),
 new opcode_handler_struct(m68000_scc_aw , 16, 0xffff, 0x54f8),
 new opcode_handler_struct(m68000_scc_al , 16, 0xffff, 0x54f9),
 new opcode_handler_struct(m68000_scs_d , 13, 0xfff8, 0x55c0),
 new opcode_handler_struct(m68000_scs_ai , 13, 0xfff8, 0x55d0),
 new opcode_handler_struct(m68000_scs_pi , 13, 0xfff8, 0x55d8),
 new opcode_handler_struct(m68000_scs_pi7 , 16, 0xffff, 0x55df),
 new opcode_handler_struct(m68000_scs_pd , 13, 0xfff8, 0x55e0),
 new opcode_handler_struct(m68000_scs_pd7 , 16, 0xffff, 0x55e7),
 new opcode_handler_struct(m68000_scs_di , 13, 0xfff8, 0x55e8),
 new opcode_handler_struct(m68000_scs_ix , 13, 0xfff8, 0x55f0),
 new opcode_handler_struct(m68000_scs_aw , 16, 0xffff, 0x55f8),
 new opcode_handler_struct(m68000_scs_al , 16, 0xffff, 0x55f9),
 new opcode_handler_struct(m68000_sne_d , 13, 0xfff8, 0x56c0),
 new opcode_handler_struct(m68000_sne_ai , 13, 0xfff8, 0x56d0),
 new opcode_handler_struct(m68000_sne_pi , 13, 0xfff8, 0x56d8),
 new opcode_handler_struct(m68000_sne_pi7 , 16, 0xffff, 0x56df),
 new opcode_handler_struct(m68000_sne_pd , 13, 0xfff8, 0x56e0),
 new opcode_handler_struct(m68000_sne_pd7 , 16, 0xffff, 0x56e7),
 new opcode_handler_struct(m68000_sne_di , 13, 0xfff8, 0x56e8),
 new opcode_handler_struct(m68000_sne_ix , 13, 0xfff8, 0x56f0),
 new opcode_handler_struct(m68000_sne_aw , 16, 0xffff, 0x56f8),
 new opcode_handler_struct(m68000_sne_al , 16, 0xffff, 0x56f9),
 new opcode_handler_struct(m68000_seq_d , 13, 0xfff8, 0x57c0),
 new opcode_handler_struct(m68000_seq_ai , 13, 0xfff8, 0x57d0),
 new opcode_handler_struct(m68000_seq_pi , 13, 0xfff8, 0x57d8),
 new opcode_handler_struct(m68000_seq_pi7 , 16, 0xffff, 0x57df),
 new opcode_handler_struct(m68000_seq_pd , 13, 0xfff8, 0x57e0),
 new opcode_handler_struct(m68000_seq_pd7 , 16, 0xffff, 0x57e7),
 new opcode_handler_struct(m68000_seq_di , 13, 0xfff8, 0x57e8),
 new opcode_handler_struct(m68000_seq_ix , 13, 0xfff8, 0x57f0),
 new opcode_handler_struct(m68000_seq_aw , 16, 0xffff, 0x57f8),
 new opcode_handler_struct(m68000_seq_al , 16, 0xffff, 0x57f9),
 new opcode_handler_struct(m68000_svc_d , 13, 0xfff8, 0x58c0),
 new opcode_handler_struct(m68000_svc_ai , 13, 0xfff8, 0x58d0),
 new opcode_handler_struct(m68000_svc_pi , 13, 0xfff8, 0x58d8),
 new opcode_handler_struct(m68000_svc_pi7 , 16, 0xffff, 0x58df),
 new opcode_handler_struct(m68000_svc_pd , 13, 0xfff8, 0x58e0),
 new opcode_handler_struct(m68000_svc_pd7 , 16, 0xffff, 0x58e7),
 new opcode_handler_struct(m68000_svc_di , 13, 0xfff8, 0x58e8),
 new opcode_handler_struct(m68000_svc_ix , 13, 0xfff8, 0x58f0),
 new opcode_handler_struct(m68000_svc_aw , 16, 0xffff, 0x58f8),
 new opcode_handler_struct(m68000_svc_al , 16, 0xffff, 0x58f9),
 new opcode_handler_struct(m68000_svs_d , 13, 0xfff8, 0x59c0),
 new opcode_handler_struct(m68000_svs_ai , 13, 0xfff8, 0x59d0),
 new opcode_handler_struct(m68000_svs_pi , 13, 0xfff8, 0x59d8),
 new opcode_handler_struct(m68000_svs_pi7 , 16, 0xffff, 0x59df),
 new opcode_handler_struct(m68000_svs_pd , 13, 0xfff8, 0x59e0),
 new opcode_handler_struct(m68000_svs_pd7 , 16, 0xffff, 0x59e7),
 new opcode_handler_struct(m68000_svs_di , 13, 0xfff8, 0x59e8),
 new opcode_handler_struct(m68000_svs_ix , 13, 0xfff8, 0x59f0),
 new opcode_handler_struct(m68000_svs_aw , 16, 0xffff, 0x59f8),
 new opcode_handler_struct(m68000_svs_al , 16, 0xffff, 0x59f9),
 new opcode_handler_struct(m68000_spl_d , 13, 0xfff8, 0x5ac0),
 new opcode_handler_struct(m68000_spl_ai , 13, 0xfff8, 0x5ad0),
 new opcode_handler_struct(m68000_spl_pi , 13, 0xfff8, 0x5ad8),
 new opcode_handler_struct(m68000_spl_pi7 , 16, 0xffff, 0x5adf),
 new opcode_handler_struct(m68000_spl_pd , 13, 0xfff8, 0x5ae0),
 new opcode_handler_struct(m68000_spl_pd7 , 16, 0xffff, 0x5ae7),
 new opcode_handler_struct(m68000_spl_di , 13, 0xfff8, 0x5ae8),
 new opcode_handler_struct(m68000_spl_ix , 13, 0xfff8, 0x5af0),
 new opcode_handler_struct(m68000_spl_aw , 16, 0xffff, 0x5af8),
 new opcode_handler_struct(m68000_spl_al , 16, 0xffff, 0x5af9),
 new opcode_handler_struct(m68000_smi_d , 13, 0xfff8, 0x5bc0),
 new opcode_handler_struct(m68000_smi_ai , 13, 0xfff8, 0x5bd0),
 new opcode_handler_struct(m68000_smi_pi , 13, 0xfff8, 0x5bd8),
 new opcode_handler_struct(m68000_smi_pi7 , 16, 0xffff, 0x5bdf),
 new opcode_handler_struct(m68000_smi_pd , 13, 0xfff8, 0x5be0),
 new opcode_handler_struct(m68000_smi_pd7 , 16, 0xffff, 0x5be7),
 new opcode_handler_struct(m68000_smi_di , 13, 0xfff8, 0x5be8),
 new opcode_handler_struct(m68000_smi_ix , 13, 0xfff8, 0x5bf0),
 new opcode_handler_struct(m68000_smi_aw , 16, 0xffff, 0x5bf8),
 new opcode_handler_struct(m68000_smi_al , 16, 0xffff, 0x5bf9),
 new opcode_handler_struct(m68000_sge_d , 13, 0xfff8, 0x5cc0),
 new opcode_handler_struct(m68000_sge_ai , 13, 0xfff8, 0x5cd0),
 new opcode_handler_struct(m68000_sge_pi , 13, 0xfff8, 0x5cd8),
 new opcode_handler_struct(m68000_sge_pi7 , 16, 0xffff, 0x5cdf),
 new opcode_handler_struct(m68000_sge_pd , 13, 0xfff8, 0x5ce0),
 new opcode_handler_struct(m68000_sge_pd7 , 16, 0xffff, 0x5ce7),
 new opcode_handler_struct(m68000_sge_di , 13, 0xfff8, 0x5ce8),
 new opcode_handler_struct(m68000_sge_ix , 13, 0xfff8, 0x5cf0),
 new opcode_handler_struct(m68000_sge_aw , 16, 0xffff, 0x5cf8),
 new opcode_handler_struct(m68000_sge_al , 16, 0xffff, 0x5cf9),
 new opcode_handler_struct(m68000_slt_d , 13, 0xfff8, 0x5dc0),
 new opcode_handler_struct(m68000_slt_ai , 13, 0xfff8, 0x5dd0),
 new opcode_handler_struct(m68000_slt_pi , 13, 0xfff8, 0x5dd8),
 new opcode_handler_struct(m68000_slt_pi7 , 16, 0xffff, 0x5ddf),
 new opcode_handler_struct(m68000_slt_pd , 13, 0xfff8, 0x5de0),
 new opcode_handler_struct(m68000_slt_pd7 , 16, 0xffff, 0x5de7),
 new opcode_handler_struct(m68000_slt_di , 13, 0xfff8, 0x5de8),
 new opcode_handler_struct(m68000_slt_ix , 13, 0xfff8, 0x5df0),
 new opcode_handler_struct(m68000_slt_aw , 16, 0xffff, 0x5df8),
 new opcode_handler_struct(m68000_slt_al , 16, 0xffff, 0x5df9),
 new opcode_handler_struct(m68000_sgt_d , 13, 0xfff8, 0x5ec0),
 new opcode_handler_struct(m68000_sgt_ai , 13, 0xfff8, 0x5ed0),
 new opcode_handler_struct(m68000_sgt_pi , 13, 0xfff8, 0x5ed8),
 new opcode_handler_struct(m68000_sgt_pi7 , 16, 0xffff, 0x5edf),
 new opcode_handler_struct(m68000_sgt_pd , 13, 0xfff8, 0x5ee0),
 new opcode_handler_struct(m68000_sgt_pd7 , 16, 0xffff, 0x5ee7),
 new opcode_handler_struct(m68000_sgt_di , 13, 0xfff8, 0x5ee8),
 new opcode_handler_struct(m68000_sgt_ix , 13, 0xfff8, 0x5ef0),
 new opcode_handler_struct(m68000_sgt_aw , 16, 0xffff, 0x5ef8),
 new opcode_handler_struct(m68000_sgt_al , 16, 0xffff, 0x5ef9),
 new opcode_handler_struct(m68000_sle_d , 13, 0xfff8, 0x5fc0),
 new opcode_handler_struct(m68000_sle_ai , 13, 0xfff8, 0x5fd0),
 new opcode_handler_struct(m68000_sle_pi , 13, 0xfff8, 0x5fd8),
 new opcode_handler_struct(m68000_sle_pi7 , 16, 0xffff, 0x5fdf),
 new opcode_handler_struct(m68000_sle_pd , 13, 0xfff8, 0x5fe0),
 new opcode_handler_struct(m68000_sle_pd7 , 16, 0xffff, 0x5fe7),
 new opcode_handler_struct(m68000_sle_di , 13, 0xfff8, 0x5fe8),
 new opcode_handler_struct(m68000_sle_ix , 13, 0xfff8, 0x5ff0),
 new opcode_handler_struct(m68000_sle_aw , 16, 0xffff, 0x5ff8),
 new opcode_handler_struct(m68000_sle_al , 16, 0xffff, 0x5ff9),
 new opcode_handler_struct(m68000_stop , 16, 0xffff, 0x4e72),
 new opcode_handler_struct(m68000_sub_er_d_8 , 10, 0xf1f8, 0x9000),
 new opcode_handler_struct(m68000_sub_er_ai_8 , 10, 0xf1f8, 0x9010),
 new opcode_handler_struct(m68000_sub_er_pi_8 , 10, 0xf1f8, 0x9018),
 new opcode_handler_struct(m68000_sub_er_pi7_8 , 13, 0xf1ff, 0x901f),
 new opcode_handler_struct(m68000_sub_er_pd_8 , 10, 0xf1f8, 0x9020),
 new opcode_handler_struct(m68000_sub_er_pd7_8 , 13, 0xf1ff, 0x9027),
 new opcode_handler_struct(m68000_sub_er_di_8 , 10, 0xf1f8, 0x9028),
 new opcode_handler_struct(m68000_sub_er_ix_8 , 10, 0xf1f8, 0x9030),
 new opcode_handler_struct(m68000_sub_er_aw_8 , 13, 0xf1ff, 0x9038),
 new opcode_handler_struct(m68000_sub_er_al_8 , 13, 0xf1ff, 0x9039),
 new opcode_handler_struct(m68000_sub_er_pcdi_8 , 13, 0xf1ff, 0x903a),
 new opcode_handler_struct(m68000_sub_er_pcix_8 , 13, 0xf1ff, 0x903b),
 new opcode_handler_struct(m68000_sub_er_i_8 , 13, 0xf1ff, 0x903c),
 new opcode_handler_struct(m68000_sub_er_d_16 , 10, 0xf1f8, 0x9040),
 new opcode_handler_struct(m68000_sub_er_a_16 , 10, 0xf1f8, 0x9048),
 new opcode_handler_struct(m68000_sub_er_ai_16 , 10, 0xf1f8, 0x9050),
 new opcode_handler_struct(m68000_sub_er_pi_16 , 10, 0xf1f8, 0x9058),
 new opcode_handler_struct(m68000_sub_er_pd_16 , 10, 0xf1f8, 0x9060),
 new opcode_handler_struct(m68000_sub_er_di_16 , 10, 0xf1f8, 0x9068),
 new opcode_handler_struct(m68000_sub_er_ix_16 , 10, 0xf1f8, 0x9070),
 new opcode_handler_struct(m68000_sub_er_aw_16 , 13, 0xf1ff, 0x9078),
 new opcode_handler_struct(m68000_sub_er_al_16 , 13, 0xf1ff, 0x9079),
 new opcode_handler_struct(m68000_sub_er_pcdi_16 , 13, 0xf1ff, 0x907a),
 new opcode_handler_struct(m68000_sub_er_pcix_16 , 13, 0xf1ff, 0x907b),
 new opcode_handler_struct(m68000_sub_er_i_16 , 13, 0xf1ff, 0x907c),
 new opcode_handler_struct(m68000_sub_er_d_32 , 10, 0xf1f8, 0x9080),
 new opcode_handler_struct(m68000_sub_er_a_32 , 10, 0xf1f8, 0x9088),
 new opcode_handler_struct(m68000_sub_er_ai_32 , 10, 0xf1f8, 0x9090),
 new opcode_handler_struct(m68000_sub_er_pi_32 , 10, 0xf1f8, 0x9098),
 new opcode_handler_struct(m68000_sub_er_pd_32 , 10, 0xf1f8, 0x90a0),
 new opcode_handler_struct(m68000_sub_er_di_32 , 10, 0xf1f8, 0x90a8),
 new opcode_handler_struct(m68000_sub_er_ix_32 , 10, 0xf1f8, 0x90b0),
 new opcode_handler_struct(m68000_sub_er_aw_32 , 13, 0xf1ff, 0x90b8),
 new opcode_handler_struct(m68000_sub_er_al_32 , 13, 0xf1ff, 0x90b9),
 new opcode_handler_struct(m68000_sub_er_pcdi_32 , 13, 0xf1ff, 0x90ba),
 new opcode_handler_struct(m68000_sub_er_pcix_32 , 13, 0xf1ff, 0x90bb),
 new opcode_handler_struct(m68000_sub_er_i_32 , 13, 0xf1ff, 0x90bc),
 new opcode_handler_struct(m68000_sub_re_ai_8 , 10, 0xf1f8, 0x9110),
 new opcode_handler_struct(m68000_sub_re_pi_8 , 10, 0xf1f8, 0x9118),
 new opcode_handler_struct(m68000_sub_re_pi7_8 , 13, 0xf1ff, 0x911f),
 new opcode_handler_struct(m68000_sub_re_pd_8 , 10, 0xf1f8, 0x9120),
 new opcode_handler_struct(m68000_sub_re_pd7_8 , 13, 0xf1ff, 0x9127),
 new opcode_handler_struct(m68000_sub_re_di_8 , 10, 0xf1f8, 0x9128),
 new opcode_handler_struct(m68000_sub_re_ix_8 , 10, 0xf1f8, 0x9130),
 new opcode_handler_struct(m68000_sub_re_aw_8 , 13, 0xf1ff, 0x9138),
 new opcode_handler_struct(m68000_sub_re_al_8 , 13, 0xf1ff, 0x9139),
 new opcode_handler_struct(m68000_sub_re_ai_16 , 10, 0xf1f8, 0x9150),
 new opcode_handler_struct(m68000_sub_re_pi_16 , 10, 0xf1f8, 0x9158),
 new opcode_handler_struct(m68000_sub_re_pd_16 , 10, 0xf1f8, 0x9160),
 new opcode_handler_struct(m68000_sub_re_di_16 , 10, 0xf1f8, 0x9168),
 new opcode_handler_struct(m68000_sub_re_ix_16 , 10, 0xf1f8, 0x9170),
 new opcode_handler_struct(m68000_sub_re_aw_16 , 13, 0xf1ff, 0x9178),
 new opcode_handler_struct(m68000_sub_re_al_16 , 13, 0xf1ff, 0x9179),
 new opcode_handler_struct(m68000_sub_re_ai_32 , 10, 0xf1f8, 0x9190),
 new opcode_handler_struct(m68000_sub_re_pi_32 , 10, 0xf1f8, 0x9198),
 new opcode_handler_struct(m68000_sub_re_pd_32 , 10, 0xf1f8, 0x91a0),
 new opcode_handler_struct(m68000_sub_re_di_32 , 10, 0xf1f8, 0x91a8),
 new opcode_handler_struct(m68000_sub_re_ix_32 , 10, 0xf1f8, 0x91b0),
 new opcode_handler_struct(m68000_sub_re_aw_32 , 13, 0xf1ff, 0x91b8),
 new opcode_handler_struct(m68000_sub_re_al_32 , 13, 0xf1ff, 0x91b9),
 new opcode_handler_struct(m68000_suba_d_16 , 10, 0xf1f8, 0x90c0),
 new opcode_handler_struct(m68000_suba_a_16 , 10, 0xf1f8, 0x90c8),
 new opcode_handler_struct(m68000_suba_ai_16 , 10, 0xf1f8, 0x90d0),
 new opcode_handler_struct(m68000_suba_pi_16 , 10, 0xf1f8, 0x90d8),
 new opcode_handler_struct(m68000_suba_pd_16 , 10, 0xf1f8, 0x90e0),
 new opcode_handler_struct(m68000_suba_di_16 , 10, 0xf1f8, 0x90e8),
 new opcode_handler_struct(m68000_suba_ix_16 , 10, 0xf1f8, 0x90f0),
 new opcode_handler_struct(m68000_suba_aw_16 , 13, 0xf1ff, 0x90f8),
 new opcode_handler_struct(m68000_suba_al_16 , 13, 0xf1ff, 0x90f9),
 new opcode_handler_struct(m68000_suba_pcdi_16 , 13, 0xf1ff, 0x90fa),
 new opcode_handler_struct(m68000_suba_pcix_16 , 13, 0xf1ff, 0x90fb),
 new opcode_handler_struct(m68000_suba_i_16 , 13, 0xf1ff, 0x90fc),
 new opcode_handler_struct(m68000_suba_d_32 , 10, 0xf1f8, 0x91c0),
 new opcode_handler_struct(m68000_suba_a_32 , 10, 0xf1f8, 0x91c8),
 new opcode_handler_struct(m68000_suba_ai_32 , 10, 0xf1f8, 0x91d0),
 new opcode_handler_struct(m68000_suba_pi_32 , 10, 0xf1f8, 0x91d8),
 new opcode_handler_struct(m68000_suba_pd_32 , 10, 0xf1f8, 0x91e0),
 new opcode_handler_struct(m68000_suba_di_32 , 10, 0xf1f8, 0x91e8),
 new opcode_handler_struct(m68000_suba_ix_32 , 10, 0xf1f8, 0x91f0),
 new opcode_handler_struct(m68000_suba_aw_32 , 13, 0xf1ff, 0x91f8),
 new opcode_handler_struct(m68000_suba_al_32 , 13, 0xf1ff, 0x91f9),
 new opcode_handler_struct(m68000_suba_pcdi_32 , 13, 0xf1ff, 0x91fa),
 new opcode_handler_struct(m68000_suba_pcix_32 , 13, 0xf1ff, 0x91fb),
 new opcode_handler_struct(m68000_suba_i_32 , 13, 0xf1ff, 0x91fc),
 new opcode_handler_struct(m68000_subi_d_8 , 13, 0xfff8, 0x0400),
 new opcode_handler_struct(m68000_subi_ai_8 , 13, 0xfff8, 0x0410),
 new opcode_handler_struct(m68000_subi_pi_8 , 13, 0xfff8, 0x0418),
 new opcode_handler_struct(m68000_subi_pi7_8 , 16, 0xffff, 0x041f),
 new opcode_handler_struct(m68000_subi_pd_8 , 13, 0xfff8, 0x0420),
 new opcode_handler_struct(m68000_subi_pd7_8 , 16, 0xffff, 0x0427),
 new opcode_handler_struct(m68000_subi_di_8 , 13, 0xfff8, 0x0428),
 new opcode_handler_struct(m68000_subi_ix_8 , 13, 0xfff8, 0x0430),
 new opcode_handler_struct(m68000_subi_aw_8 , 16, 0xffff, 0x0438),
 new opcode_handler_struct(m68000_subi_al_8 , 16, 0xffff, 0x0439),
 new opcode_handler_struct(m68000_subi_d_16 , 13, 0xfff8, 0x0440),
 new opcode_handler_struct(m68000_subi_ai_16 , 13, 0xfff8, 0x0450),
 new opcode_handler_struct(m68000_subi_pi_16 , 13, 0xfff8, 0x0458),
 new opcode_handler_struct(m68000_subi_pd_16 , 13, 0xfff8, 0x0460),
 new opcode_handler_struct(m68000_subi_di_16 , 13, 0xfff8, 0x0468),
 new opcode_handler_struct(m68000_subi_ix_16 , 13, 0xfff8, 0x0470),
 new opcode_handler_struct(m68000_subi_aw_16 , 16, 0xffff, 0x0478),
 new opcode_handler_struct(m68000_subi_al_16 , 16, 0xffff, 0x0479),
 new opcode_handler_struct(m68000_subi_d_32 , 13, 0xfff8, 0x0480),
 new opcode_handler_struct(m68000_subi_ai_32 , 13, 0xfff8, 0x0490),
 new opcode_handler_struct(m68000_subi_pi_32 , 13, 0xfff8, 0x0498),
 new opcode_handler_struct(m68000_subi_pd_32 , 13, 0xfff8, 0x04a0),
 new opcode_handler_struct(m68000_subi_di_32 , 13, 0xfff8, 0x04a8),
 new opcode_handler_struct(m68000_subi_ix_32 , 13, 0xfff8, 0x04b0),
 new opcode_handler_struct(m68000_subi_aw_32 , 16, 0xffff, 0x04b8),
 new opcode_handler_struct(m68000_subi_al_32 , 16, 0xffff, 0x04b9),
 new opcode_handler_struct(m68000_subq_d_8 , 10, 0xf1f8, 0x5100),
 new opcode_handler_struct(m68000_subq_ai_8 , 10, 0xf1f8, 0x5110),
 new opcode_handler_struct(m68000_subq_pi_8 , 10, 0xf1f8, 0x5118),
 new opcode_handler_struct(m68000_subq_pi7_8 , 13, 0xf1ff, 0x511f),
 new opcode_handler_struct(m68000_subq_pd_8 , 10, 0xf1f8, 0x5120),
 new opcode_handler_struct(m68000_subq_pd7_8 , 13, 0xf1ff, 0x5127),
 new opcode_handler_struct(m68000_subq_di_8 , 10, 0xf1f8, 0x5128),
 new opcode_handler_struct(m68000_subq_ix_8 , 10, 0xf1f8, 0x5130),
 new opcode_handler_struct(m68000_subq_aw_8 , 13, 0xf1ff, 0x5138),
 new opcode_handler_struct(m68000_subq_al_8 , 13, 0xf1ff, 0x5139),
 new opcode_handler_struct(m68000_subq_d_16 , 10, 0xf1f8, 0x5140),
 new opcode_handler_struct(m68000_subq_a_16 , 10, 0xf1f8, 0x5148),
 new opcode_handler_struct(m68000_subq_ai_16 , 10, 0xf1f8, 0x5150),
 new opcode_handler_struct(m68000_subq_pi_16 , 10, 0xf1f8, 0x5158),
 new opcode_handler_struct(m68000_subq_pd_16 , 10, 0xf1f8, 0x5160),
 new opcode_handler_struct(m68000_subq_di_16 , 10, 0xf1f8, 0x5168),
 new opcode_handler_struct(m68000_subq_ix_16 , 10, 0xf1f8, 0x5170),
 new opcode_handler_struct(m68000_subq_aw_16 , 13, 0xf1ff, 0x5178),
 new opcode_handler_struct(m68000_subq_al_16 , 13, 0xf1ff, 0x5179),
 new opcode_handler_struct(m68000_subq_d_32 , 10, 0xf1f8, 0x5180),
 new opcode_handler_struct(m68000_subq_a_32 , 10, 0xf1f8, 0x5188),
 new opcode_handler_struct(m68000_subq_ai_32 , 10, 0xf1f8, 0x5190),
 new opcode_handler_struct(m68000_subq_pi_32 , 10, 0xf1f8, 0x5198),
 new opcode_handler_struct(m68000_subq_pd_32 , 10, 0xf1f8, 0x51a0),
 new opcode_handler_struct(m68000_subq_di_32 , 10, 0xf1f8, 0x51a8),
 new opcode_handler_struct(m68000_subq_ix_32 , 10, 0xf1f8, 0x51b0),
 new opcode_handler_struct(m68000_subq_aw_32 , 13, 0xf1ff, 0x51b8),
 new opcode_handler_struct(m68000_subq_al_32 , 13, 0xf1ff, 0x51b9),
 new opcode_handler_struct(m68000_subx_rr_8 , 10, 0xf1f8, 0x9100),
 new opcode_handler_struct(m68000_subx_rr_16 , 10, 0xf1f8, 0x9140),
 new opcode_handler_struct(m68000_subx_rr_32 , 10, 0xf1f8, 0x9180),
 new opcode_handler_struct(m68000_subx_mm_8_ax7 , 13, 0xfff8, 0x9f08),
 new opcode_handler_struct(m68000_subx_mm_8_ay7 , 13, 0xf1ff, 0x910f),
 new opcode_handler_struct(m68000_subx_mm_8_axy7 , 16, 0xffff, 0x9f0f),
 new opcode_handler_struct(m68000_subx_mm_8 , 10, 0xf1f8, 0x9108),
 new opcode_handler_struct(m68000_subx_mm_16 , 10, 0xf1f8, 0x9148),
 new opcode_handler_struct(m68000_subx_mm_32 , 10, 0xf1f8, 0x9188),
 new opcode_handler_struct(m68000_swap , 13, 0xfff8, 0x4840),
 new opcode_handler_struct(m68000_tas_d , 13, 0xfff8, 0x4ac0),
 new opcode_handler_struct(m68000_tas_ai , 13, 0xfff8, 0x4ad0),
 new opcode_handler_struct(m68000_tas_pi , 13, 0xfff8, 0x4ad8),
 new opcode_handler_struct(m68000_tas_pi7 , 16, 0xffff, 0x4adf),
 new opcode_handler_struct(m68000_tas_pd , 13, 0xfff8, 0x4ae0),
 new opcode_handler_struct(m68000_tas_pd7 , 16, 0xffff, 0x4ae7),
 new opcode_handler_struct(m68000_tas_di , 13, 0xfff8, 0x4ae8),
 new opcode_handler_struct(m68000_tas_ix , 13, 0xfff8, 0x4af0),
 new opcode_handler_struct(m68000_tas_aw , 16, 0xffff, 0x4af8),
 new opcode_handler_struct(m68000_tas_al , 16, 0xffff, 0x4af9),
 new opcode_handler_struct(m68000_trap , 12, 0xfff0, 0x4e40),
 new opcode_handler_struct(m68020_trapt_0 , 16, 0xffff, 0x50fc),
 new opcode_handler_struct(m68020_trapt_16 , 16, 0xffff, 0x50fa),
 new opcode_handler_struct(m68020_trapt_32 , 16, 0xffff, 0x50fb),
 new opcode_handler_struct(m68020_trapf_0 , 16, 0xffff, 0x51fc),
 new opcode_handler_struct(m68020_trapf_16 , 16, 0xffff, 0x51fa),
 new opcode_handler_struct(m68020_trapf_32 , 16, 0xffff, 0x51fb),
 new opcode_handler_struct(m68020_traphi_0 , 16, 0xffff, 0x52fc),
 new opcode_handler_struct(m68020_traphi_16 , 16, 0xffff, 0x52fa),
 new opcode_handler_struct(m68020_traphi_32 , 16, 0xffff, 0x52fb),
 new opcode_handler_struct(m68020_trapls_0 , 16, 0xffff, 0x53fc),
 new opcode_handler_struct(m68020_trapls_16 , 16, 0xffff, 0x53fa),
 new opcode_handler_struct(m68020_trapls_32 , 16, 0xffff, 0x53fb),
 new opcode_handler_struct(m68020_trapcc_0 , 16, 0xffff, 0x54fc),
 new opcode_handler_struct(m68020_trapcc_16 , 16, 0xffff, 0x54fa),
 new opcode_handler_struct(m68020_trapcc_32 , 16, 0xffff, 0x54fb),
 new opcode_handler_struct(m68020_trapcs_0 , 16, 0xffff, 0x55fc),
 new opcode_handler_struct(m68020_trapcs_16 , 16, 0xffff, 0x55fa),
 new opcode_handler_struct(m68020_trapcs_32 , 16, 0xffff, 0x55fb),
 new opcode_handler_struct(m68020_trapne_0 , 16, 0xffff, 0x56fc),
 new opcode_handler_struct(m68020_trapne_16 , 16, 0xffff, 0x56fa),
 new opcode_handler_struct(m68020_trapne_32 , 16, 0xffff, 0x56fb),
 new opcode_handler_struct(m68020_trapeq_0 , 16, 0xffff, 0x57fc),
 new opcode_handler_struct(m68020_trapeq_16 , 16, 0xffff, 0x57fa),
 new opcode_handler_struct(m68020_trapeq_32 , 16, 0xffff, 0x57fb),
 new opcode_handler_struct(m68020_trapvc_0 , 16, 0xffff, 0x58fc),
 new opcode_handler_struct(m68020_trapvc_16 , 16, 0xffff, 0x58fa),
 new opcode_handler_struct(m68020_trapvc_32 , 16, 0xffff, 0x58fb),
 new opcode_handler_struct(m68020_trapvs_0 , 16, 0xffff, 0x59fc),
 new opcode_handler_struct(m68020_trapvs_16 , 16, 0xffff, 0x59fa),
 new opcode_handler_struct(m68020_trapvs_32 , 16, 0xffff, 0x59fb),
 new opcode_handler_struct(m68020_trappl_0 , 16, 0xffff, 0x5afc),
 new opcode_handler_struct(m68020_trappl_16 , 16, 0xffff, 0x5afa),
 new opcode_handler_struct(m68020_trappl_32 , 16, 0xffff, 0x5afb),
 new opcode_handler_struct(m68020_trapmi_0 , 16, 0xffff, 0x5bfc),
 new opcode_handler_struct(m68020_trapmi_16 , 16, 0xffff, 0x5bfa),
 new opcode_handler_struct(m68020_trapmi_32 , 16, 0xffff, 0x5bfb),
 new opcode_handler_struct(m68020_trapge_0 , 16, 0xffff, 0x5cfc),
 new opcode_handler_struct(m68020_trapge_16 , 16, 0xffff, 0x5cfa),
 new opcode_handler_struct(m68020_trapge_32 , 16, 0xffff, 0x5cfb),
 new opcode_handler_struct(m68020_traplt_0 , 16, 0xffff, 0x5dfc),
 new opcode_handler_struct(m68020_traplt_16 , 16, 0xffff, 0x5dfa),
 new opcode_handler_struct(m68020_traplt_32 , 16, 0xffff, 0x5dfb),
 new opcode_handler_struct(m68020_trapgt_0 , 16, 0xffff, 0x5efc),
 new opcode_handler_struct(m68020_trapgt_16 , 16, 0xffff, 0x5efa),
 new opcode_handler_struct(m68020_trapgt_32 , 16, 0xffff, 0x5efb),
 new opcode_handler_struct(m68020_traple_0 , 16, 0xffff, 0x5ffc),
 new opcode_handler_struct(m68020_traple_16 , 16, 0xffff, 0x5ffa),
 new opcode_handler_struct(m68020_traple_32 , 16, 0xffff, 0x5ffb),
 new opcode_handler_struct(m68000_trapv , 16, 0xffff, 0x4e76),
 new opcode_handler_struct(m68000_tst_d_8 , 13, 0xfff8, 0x4a00),
 new opcode_handler_struct(m68000_tst_ai_8 , 13, 0xfff8, 0x4a10),
 new opcode_handler_struct(m68000_tst_pi_8 , 13, 0xfff8, 0x4a18),
 new opcode_handler_struct(m68000_tst_pi7_8 , 16, 0xffff, 0x4a1f),
 new opcode_handler_struct(m68000_tst_pd_8 , 13, 0xfff8, 0x4a20),
 new opcode_handler_struct(m68000_tst_pd7_8 , 16, 0xffff, 0x4a27),
 new opcode_handler_struct(m68000_tst_di_8 , 13, 0xfff8, 0x4a28),
 new opcode_handler_struct(m68000_tst_ix_8 , 13, 0xfff8, 0x4a30),
 new opcode_handler_struct(m68000_tst_aw_8 , 16, 0xffff, 0x4a38),
 new opcode_handler_struct(m68000_tst_al_8 , 16, 0xffff, 0x4a39),
 new opcode_handler_struct(m68020_tst_pcdi_8 , 16, 0xffff, 0x4a3a),
 new opcode_handler_struct(m68020_tst_pcix_8 , 16, 0xffff, 0x4a3b),
 new opcode_handler_struct(m68020_tst_imm_8 , 16, 0xffff, 0x4a3c),
 new opcode_handler_struct(m68000_tst_d_16 , 13, 0xfff8, 0x4a40),
 new opcode_handler_struct(m68020_tst_a_16 , 13, 0xfff8, 0x4a48),
 new opcode_handler_struct(m68000_tst_ai_16 , 13, 0xfff8, 0x4a50),
 new opcode_handler_struct(m68000_tst_pi_16 , 13, 0xfff8, 0x4a58),
 new opcode_handler_struct(m68000_tst_pd_16 , 13, 0xfff8, 0x4a60),
 new opcode_handler_struct(m68000_tst_di_16 , 13, 0xfff8, 0x4a68),
 new opcode_handler_struct(m68000_tst_ix_16 , 13, 0xfff8, 0x4a70),
 new opcode_handler_struct(m68000_tst_aw_16 , 16, 0xffff, 0x4a78),
 new opcode_handler_struct(m68000_tst_al_16 , 16, 0xffff, 0x4a79),
 new opcode_handler_struct(m68020_tst_pcdi_16 , 16, 0xffff, 0x4a7a),
 new opcode_handler_struct(m68020_tst_pcix_16 , 16, 0xffff, 0x4a7b),
 new opcode_handler_struct(m68020_tst_imm_16 , 16, 0xffff, 0x4a7c),
 new opcode_handler_struct(m68000_tst_d_32 , 13, 0xfff8, 0x4a80),
 new opcode_handler_struct(m68020_tst_a_32 , 13, 0xfff8, 0x4a88),
 new opcode_handler_struct(m68000_tst_ai_32 , 13, 0xfff8, 0x4a90),
 new opcode_handler_struct(m68000_tst_pi_32 , 13, 0xfff8, 0x4a98),
 new opcode_handler_struct(m68000_tst_pd_32 , 13, 0xfff8, 0x4aa0),
 new opcode_handler_struct(m68000_tst_di_32 , 13, 0xfff8, 0x4aa8),
 new opcode_handler_struct(m68000_tst_ix_32 , 13, 0xfff8, 0x4ab0),
 new opcode_handler_struct(m68000_tst_aw_32 , 16, 0xffff, 0x4ab8),
 new opcode_handler_struct(m68000_tst_al_32 , 16, 0xffff, 0x4ab9),
 new opcode_handler_struct(m68020_tst_pcdi_32 , 16, 0xffff, 0x4aba),
 new opcode_handler_struct(m68020_tst_pcix_32 , 16, 0xffff, 0x4abb),
 new opcode_handler_struct(m68020_tst_imm_32 , 16, 0xffff, 0x4abc),
 new opcode_handler_struct(m68000_unlk_a7 , 16, 0xffff, 0x4e5f),
 new opcode_handler_struct(m68000_unlk , 13, 0xfff8, 0x4e58),
 new opcode_handler_struct(m68020_unpk_rr , 10, 0xf1f8, 0x8180),
 new opcode_handler_struct(m68020_unpk_mm_ax7 , 13, 0xf1ff, 0x818f),
 new opcode_handler_struct(m68020_unpk_mm_ay7 , 13, 0xfff8, 0x8f88),
 new opcode_handler_struct(m68020_unpk_mm_axy7 , 16, 0xffff, 0x8f8f),
 new opcode_handler_struct(m68020_unpk_mm , 10, 0xf1f8, 0x8188),
};

            static int compare_nof_true_bits(opcode_handler_struct aptr, opcode_handler_struct bptr)
            {
                opcode_handler_struct a = aptr, b = bptr;
                if (a.bits != b.bits)
                    return (int)(a.bits - b.bits);
                if (a.mask != b.mask)
                    return (int)(a.mask - b.mask);
                return (int)(a.match - b.match);
            }
            /* Build the opcode handler jump table */
            void m68ki_build_opcode_table()
            {
                opcode_handler_struct ostruct;
                uint table_length = 0;
                int i, j;

                for (ostruct = m68k_opcode_handler_table; ostruct.opcode_handler != 0; ostruct++)
                    table_length++;

                //qsort((void *)m68k_opcode_handler_table, table_length, sizeof(m68k_opcode_handler_table[0]), compare_nof_true_bits);
                m68k_opcode_handler_table.ToList().Sort(compare_nof_true_bits);

                for (i = 0; i < 0x10000; i++)
                {
                    /* default to illegal */
                    m68k_instruction_jump_table[i] = m68000_illegal;
                }

                ostruct = m68k_opcode_handler_table;
                while (ostruct.mask != 0xff00)
                {
                    for (i = 0; i < 0x10000; i++)
                    {
                        if ((i & ostruct.mask) == ostruct.match)
                        {
                            m68k_instruction_jump_table[i] = ostruct.opcode_handler;
                        }
                    }
                    ostruct++;
                }
                while (ostruct.mask == 0xff00)
                {
                    for (i = 0; i <= 0xff; i++)
                        m68k_instruction_jump_table[ostruct.match | i] = ostruct.opcode_handler;
                    ostruct++;
                }
                while (ostruct.mask == 0xf1f8)
                {
                    for (i = 0; i < 8; i++)
                    {
                        for (j = 0; j < 8; j++)
                        {
                            m68k_instruction_jump_table[ostruct.match | (i << 9) | j] = ostruct.opcode_handler;
                        }
                    }
                    ostruct++;
                }
                while (ostruct.mask == 0xfff0)
                {
                    for (i = 0; i <= 0x0f; i++)
                        m68k_instruction_jump_table[ostruct.match | i] = ostruct.opcode_handler;
                    ostruct++;
                }
                while (ostruct.mask == 0xf1ff)
                {
                    for (i = 0; i <= 0x07; i++)
                        m68k_instruction_jump_table[ostruct.match | (i << 9)] = ostruct.opcode_handler;
                    ostruct++;
                }
                while (ostruct.mask == 0xfff8)
                {
                    for (i = 0; i <= 0x07; i++)
                        m68k_instruction_jump_table[ostruct.match | i] = ostruct.opcode_handler;
                    ostruct++;
                }
                while (ostruct.mask == 0xffff)
                {
                    m68k_instruction_jump_table[ostruct.match] = ostruct.opcode_handler;
                    ostruct++;
                }
            }

            static void m6000_nbcd_d()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (6);
            }
            static void m6000_nbcd_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_nbcd_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_nbcd_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_nbcd_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_nbcd_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_nbcd_di()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_nbcd_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_nbcd_aw()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_nbcd_al()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    m68ki_write_8(ea, res);

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_neg_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (uint)((-dst) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_neg_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_neg_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_neg_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_neg_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_neg_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_neg_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_neg_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_neg_aw_8()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_neg_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_neg_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = *d_dst;
                uint res = ((-dst) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_neg_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_neg_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_neg_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_neg_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_neg_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_neg_aw_16()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_16(ea);
                uint res = ((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_neg_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_neg_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)(-dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (6);
            }


            static void m6000_neg_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_neg_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_neg_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_neg_di_32()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_neg_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_neg_aw_32()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_32(ea);
                uint res = (-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_neg_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0)?1u:0u;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_negx_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_negx_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_negx_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_negx_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_negx_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_negx_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_negx_di_8()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_negx_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_negx_aw_8()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_negx_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)(uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_negx_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_negx_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_negx_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_negx_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_negx_di_16()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_negx_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_negx_aw_16()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_negx_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_negx_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m6000_negx_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_negx_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_negx_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_negx_di_32()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_negx_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_negx_aw_32()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_negx_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_nop()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (4);
            }


            static void m6000_not_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((~m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_not_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_not_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_not_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_not_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_not_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_not_di_8()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_not_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_not_aw_8()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_not_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_not_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((~m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] =(uint) ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_not_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_not_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_not_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_not_di_16()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_not_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_not_aw_16()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_not_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_not_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = m68k_cpu.dr[m68k_cpu.ir & 7] = (~m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6);
            }


            static void m6000_not_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_not_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_not_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_not_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_not_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_not_aw_32()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_not_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_or_er_d_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_or_er_ai_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_pi_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_pi7_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_pd_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7]))))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_or_er_pd7_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((m68k_cpu.ar[7] -= 2)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_or_er_di_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff))))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_ix_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_get_ea_ix()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_or_er_aw_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_al_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_read_imm_32()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_or_er_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(ea))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_pcix_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_get_ea_pcix()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_or_er_i_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_8())) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_d_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_or_er_ai_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_pi_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_pd_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_or_er_di_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff))))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_ix_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_get_ea_ix()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_or_er_aw_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff)))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_al_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_read_imm_32()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_or_er_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(ea))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_or_er_pcix_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_get_ea_pcix()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_or_er_i_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_16())) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_or_er_d_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (m68k_cpu.dr[m68k_cpu.ir & 7])));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_or_er_ai_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_or_er_pi_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_or_er_pd_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_or_er_di_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_or_er_ix_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_get_ea_ix())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_or_er_aw_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_or_er_al_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_read_imm_32())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m6000_or_er_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(ea)));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_or_er_pcix_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_get_ea_pcix())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_or_er_i_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_32()));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_or_re_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_or_re_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_or_re_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_or_re_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_or_re_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_or_re_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_or_re_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_or_re_aw_8()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_or_re_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_or_re_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_or_re_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_or_re_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_or_re_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_or_re_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_or_re_aw_16()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_or_re_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_or_re_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_or_re_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_or_re_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_or_re_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_or_re_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_or_re_aw_32()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_or_re_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_ori_d_8()
            {
                uint res = ((((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_8())) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_ori_ai_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_ori_pi_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_ori_pi7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_ori_pd_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_ori_pd7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_ori_di_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_ori_ix_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_get_ea_ix();
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_ori_aw_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_ori_al_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_read_imm_32();
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_ori_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_16()) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_ori_ai_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_ori_pi_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_ori_pd_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_ori_di_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_ori_ix_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_ori_aw_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_ori_al_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_ori_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_ori_ai_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_ori_pi_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_ori_pd_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m6000_ori_di_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_ori_ix_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_get_ea_ix();
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m6000_ori_aw_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_ori_al_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_read_imm_32();
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m6000_ori_to_ccr()
            {
                m68ki_set_ccr(((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)) | m68ki_read_imm_16());
                m68k_clks_left[0] -= (20);
            }


            static void m6000_ori_to_sr()
            {
                uint or_val = m68ki_read_imm_16();

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr((((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)) | or_val);
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6020_pack_rr()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) + m68ki_read_imm_16();

                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xff) | ((src >> 4) & 0x00f0) | (src & 0x000f);
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_pack_mm_ax7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_16((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);

                    src = (((src >> 8) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_pack_mm_ay7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_32(m68k_cpu.ar[7] -= 4);

                    src = (((src >> 16) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    /* I hate the way Motorola changes where Rx and Ry are */
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_pack_mm_axy7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_32(m68k_cpu.ar[7] -= 4);

                    src = (((src >> 16) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_pack_mm()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_16((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);

                    src = (((src >> 8) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_pea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m6000_pea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m6000_pea_ix()
            {
                uint ea = m68ki_get_ea_ix();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m6000_pea_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m6000_pea_al()
            {
                uint ea = m68ki_read_imm_32();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m6000_pea_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m6000_pea_pcix()
            {
                uint ea = m68ki_get_ea_pcix();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m6000_rst()
            {
                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (132);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_ror_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint shift = orig_shift & 7;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = ((((src) >> (int)(shift)) | ((src) << (int)(8 - (shift)))) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)((shift - 1) & 7)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
            }


            static void m6000_ror_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((((src) >>(int) (shift)) | ((src) << (int)(16 - (shift)))) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] =(uint) ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m6000_ror_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((shift) < 32 ? (src) >> (int)(shift) : 0) | ((32 - (shift)) < 32 ? (src) << (int)(32 - (shift)) : 0));

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m6000_ror_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 7;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = (int)((((src) >> (int)(shift)) | ((src) << (8 - (int)(shift)))) & 0xff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;
                    m68k_cpu.c_flag = (int)(src >> ((shift - 1) & 7)) & 1;
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_ror_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 15;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((((src) >> (shift)) | ((src) << (16 - (shift)))) & 0xffff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    m68k_cpu.c_flag = (src >> ((shift - 1) & 15)) & 1;
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_ror_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 31;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((shift) < 32 ? (src) >> (shift) : 0) | ((32 - (shift)) < 32 ? (src) << (32 - (shift)) : 0));

                m68k_clks_left[0] -= ((orig_shift << 1) + 8);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = res;
                    m68k_cpu.c_flag = (src >> ((shift - 1) & 31)) & 1;
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_ror_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_ror_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_ror_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_ror_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_ror_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_ror_ea_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_ror_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_rol_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint shift = orig_shift & 7;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = ((((((src) << (shift)) | ((src) >> (8 - (shift)))) & 0xff)) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (8 - orig_shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
            }


            static void m6000_rol_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((((((src) << (shift)) | ((src) >> (16 - (shift)))) & 0xffff)) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (16 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_rol_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((((shift) < 32 ? (src) << (shift) : 0) | ((32 - (shift)) < 32 ? (src) >> (32 - (shift)) : 0)));

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (32 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 8);
            }


            static void m6000_rol_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 7;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = ((((((src) << (shift)) | ((src) >> (8 - (shift)))) & 0xff)) & 0xff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    if (shift != 0)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;
                        m68k_cpu.c_flag = (src >> (8 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }
                    m68k_cpu.c_flag = src & 1;
                    m68k_cpu.n_flag = ((src) & 0x80);
                    m68k_cpu.not_z_flag = src;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_rol_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 15;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((((((src) << (shift)) | ((src) >> (16 - (shift)))) & 0xffff)) & 0xffff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    if (shift != 0)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                        m68k_cpu.c_flag = (src >> (16 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x8000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }
                    m68k_cpu.c_flag = src & 1;
                    m68k_cpu.n_flag = ((src) & 0x8000);
                    m68k_cpu.not_z_flag = src;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_rol_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 31;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((((shift) < 32 ? (src) << (shift) : 0) | ((32 - (shift)) < 32 ? (src) >> (32 - (shift)) : 0)));

                m68k_clks_left[0] -= ((orig_shift << 1) + 8);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                    m68k_cpu.c_flag = (src >> (32 - shift)) & 1;
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_rol_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_rol_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_rol_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_rol_ea_di()
            {
                uint ea =(uint) ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_rol_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_rol_ea_aw()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_rol_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_roxr_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint tmp = (uint)((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m6000_roxr_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint tmp = (uint)((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m6000_roxr_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((((shift) < 32 ? (src) >> (shift) : 0) | ((33 - (shift)) < 32 ? (src) << (33 - (shift)) : 0)) & ~(1 << (32 - shift))) | (((m68k_cpu.x_flag != 0)?1u:0u) << (32 - shift)));
                uint new_x_flag = src & (1 << (shift - 1));

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.c_flag = m68k_cpu.x_flag = new_x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m6000_roxr_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 9;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint tmp = (uint)((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_clks_left[0] -=(int) ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] =(uint) ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxr_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 17;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (shift)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxr_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 33;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((((shift) < 32 ? (src) >> (shift) : 0) | ((33 - (shift)) < 32 ? (src) << (33 - (shift)) : 0)) & ~(1 << (32 - shift))) | (((m68k_cpu.x_flag != 0)?1u:0u) << (32 - shift)));
                uint new_x_flag = src & (1 << (shift - 1));

                m68k_clks_left[0] -= ((orig_shift << 1) + 8);
                if (shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = res;
                    m68k_cpu.x_flag = new_x_flag;
                }
                else
                    res = src;
                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxr_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_roxr_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_roxr_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_roxr_ea_di()
            {
                uint ea =(uint) ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_roxr_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_roxr_ea_aw()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_roxr_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_roxl_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 8)) << (shift)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 8)) >> (9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_roxl_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (shift)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_roxl_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((((shift) < 32 ? (src) << (shift) : 0) | ((33 - (shift)) < 32 ? (src) >> (33 - (shift)) : 0)) & ~(1 << (shift - 1))) | (((m68k_cpu.x_flag != 0)?1u:0u) << (shift - 1)));
                uint new_x_flag = src & (1 << (32 - shift));

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.c_flag = m68k_cpu.x_flag = new_x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;

                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_roxl_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 9;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 8)) << (shift)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 8)) >> (9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxl_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 17;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (shift)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_clks_left[0] -= ((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxl_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 33;
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (((((shift) < 32 ? (src) << (shift) : 0) | ((33 - (shift)) < 32 ? (src) >> (33 - (shift)) : 0)) & ~(1 << (shift - 1))) | (((m68k_cpu.x_flag != 0)?1u:0u) << (shift - 1)));
                uint new_x_flag = src & (1 << (32 - shift));

                m68k_clks_left[0] -= ((orig_shift << 1) + 8);
                if (shift != 0)
                {
                    m68k_cpu.dr[m68k_cpu.ir & 7] = res;
                    m68k_cpu.x_flag = new_x_flag;
                }
                else
                    res = src;
                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_roxl_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_roxl_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_roxl_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_roxl_ea_di()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_roxl_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_roxl_ea_aw()
            {
                uint ea = (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_roxl_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0)?1u:0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6010_rtd()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint new_pc = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.ar[7] += (uint)(short)((m68ki_read_imm_16()) & 0xffff);
                    m68ki_set_pc(new_pc);
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_rte()
            {
                uint new_sr;
                uint new_pc;
                uint format_word;

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    new_sr = m68ki_read_16((m68k_cpu.ar[7] += 2) - 2);
                    new_pc = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);
                    m68ki_set_pc(new_pc);
                    if ((m68k_cpu.mode & (2 | 4 | 8))==0)
                    {
                        m68ki_set_sr(new_sr);
                        m68k_clks_left[0] -= (20);
                        return;
                    }
                    format_word = (m68ki_read_16((m68k_cpu.ar[7] += 2) - 2) >> 12) & 0xf;
                    m68ki_set_sr(new_sr);
                    /* I'm ignoring code 8 (bus error and address error) */
                    if (format_word != 0)
                        /* Generate a new program counter from the format error vector */
                        m68ki_set_pc(m68ki_read_32((14 << 2) + m68k_cpu.vbr));
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6020_rtm()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    ;


                    m68k_clks_left[0] -= (19);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_rtr()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_set_ccr(m68ki_read_16((m68k_cpu.ar[7] += 2) - 2));
                m68ki_set_pc(m68ki_read_32((m68k_cpu.ar[7] += 4) - 4));
                m68k_clks_left[0] -= (20);
            }


            static void m6000_rts()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_set_pc(m68ki_read_32((m68k_cpu.ar[7] += 4) - 4));
                m68k_clks_left[0] -= (16);
            }


            static void m6000_sbcd_rr()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0)?1u:0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ?1:0)!= 0)
                    res += 0xa0;

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | ((res) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (6);
            }


            static void m6000_sbcd_mm_ax7()
            {
                uint src = m68ki_read_8(--((m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0)?1u:0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99)?1:0) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_sbcd_mm_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0)?1u:0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99)?1:0) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_sbcd_mm_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0)?1u:0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1 : 0) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_sbcd_mm()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0)?1u:0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1 : 0) != 0)
                    res += 0xa0;

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                m68ki_write_8(ea, res);

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_st_d()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                m68k_clks_left[0] -= (6);
            }


            static void m6000_st_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_st_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_st_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_st_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), 0xff);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_st_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), 0xff);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_st_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), 0xff);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_st_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), 0xff);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_st_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), 0xff);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_st_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), 0xff);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sf_d()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sf_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sf_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sf_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sf_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sf_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sf_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sf_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sf_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sf_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_shi_d()
            {
                if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_shi_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_shi_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_shi_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_shi_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_shi_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_shi_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_shi_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_shi_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_shi_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sls_d()
            {
                if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sls_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sls_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sls_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sls_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sls_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sls_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sls_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sls_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sls_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_scc_d()
            {
                if ((m68k_cpu.c_flag == 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_scc_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scc_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scc_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scc_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_scc_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_scc_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.c_flag == 0) ? 0xffu : 0u);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_scc_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_scc_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_scc_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_scs_d()
            {
                if ((m68k_cpu.c_flag != 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_scs_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag != 0) ? 0xffu : 0u);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scs_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scs_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_scs_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_scs_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_scs_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_scs_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_scs_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_scs_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sne_d()
            {
                if ((m68k_cpu.not_z_flag != 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sne_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sne_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sne_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sne_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sne_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sne_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sne_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sne_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sne_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_seq_d()
            {
                if ((m68k_cpu.not_z_flag == 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_seq_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_seq_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_seq_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_seq_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_seq_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_seq_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_seq_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_seq_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_seq_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_svc_d()
            {
                if ((m68k_cpu.v_flag == 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_svc_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svc_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svc_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svc_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_svc_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_svc_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_svc_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_svc_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_svc_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.v_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_svs_d()
            {
                if ((m68k_cpu.v_flag != 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_svs_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svs_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svs_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_svs_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_svs_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_svs_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_svs_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_svs_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_svs_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.v_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_spl_d()
            {
                if ((m68k_cpu.n_flag == 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_spl_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_spl_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_spl_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_spl_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_spl_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_spl_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_spl_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_spl_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_spl_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.n_flag == 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_smi_d()
            {
                if ((m68k_cpu.n_flag != 0))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_smi_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_smi_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_smi_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_smi_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_smi_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_smi_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_smi_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_smi_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_smi_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.n_flag != 0) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sge_d()
            {
                if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sge_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sge_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sge_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sge_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sge_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sge_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sge_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sge_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sge_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_slt_d()
            {
                if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_slt_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_slt_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_slt_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_slt_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_slt_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_slt_di()
            {
                m68ki_write_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_slt_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_slt_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_slt_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sgt_d()
            {
                if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sgt_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sgt_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sgt_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sgt_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sgt_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sgt_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sgt_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sgt_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sgt_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sle_d()
            {
                if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sle_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sle_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sle_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sle_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sle_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sle_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sle_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sle_aw()
            {
                m68ki_write_8((short)((m68ki_read_imm_16()) & 0xffff), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sle_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xff : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_stop()
            {
                uint new_sr = m68ki_read_imm_16();

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.stopped = 1;
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] = 0;
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_sub_er_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sub_er_ai_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_pi_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_pi7_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_pd_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_sub_er_pd7_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_sub_er_di_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_ix_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(m68ki_get_ea_ix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_sub_er_aw_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_al_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(m68ki_read_imm_32());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_sub_er_pcdi_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint src = m68ki_read_8(ea);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_pcix_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_8(m68ki_get_ea_pcix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_sub_er_i_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_imm_8();
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sub_er_a_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_sub_er_ai_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_pi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_pd_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_sub_er_di_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_ix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_get_ea_ix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_sub_er_aw_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_al_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_read_imm_32());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_sub_er_pcdi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_sub_er_pcix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_get_ea_pcix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_sub_er_i_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_imm_16();
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_sub_er_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_sub_er_a_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_sub_er_ai_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_sub_er_pi_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_sub_er_pd_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_sub_er_di_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_sub_er_ix_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32(m68ki_get_ea_ix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_sub_er_aw_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_sub_er_al_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32(m68ki_read_imm_32());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m6000_sub_er_pcdi_32()
            {
                //m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint src = m68ki_read_32(ea);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_sub_er_pcix_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_32(m68ki_get_ea_pcix());
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_sub_er_i_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_imm_32();
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_sub_re_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sub_re_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sub_re_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sub_re_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sub_re_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sub_re_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sub_re_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sub_re_aw_8()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sub_re_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sub_re_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sub_re_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_sub_re_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_sub_re_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sub_re_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_sub_re_aw_16()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_sub_re_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_sub_re_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_sub_re_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_sub_re_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_sub_re_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_sub_re_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_sub_re_aw_32()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_sub_re_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_suba_d_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff));
                m68k_clks_left[0] -= (8);
            }


            static void m6000_suba_a_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] =( m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)(((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff));
                m68k_clks_left[0] -= (8);
            }


            static void m6000_suba_ai_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))) & 0xffff));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_suba_pi_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))) & 0xffff));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_suba_pd_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))) & 0xffff));
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_suba_di_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)))) & 0xffff));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_suba_ix_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(m68ki_get_ea_ix())) & 0xffff));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_suba_aw_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff))) & 0xffff));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_suba_al_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(m68ki_read_imm_32())) & 0xffff));
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_suba_pcdi_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(ea)) & 0xffff));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_suba_pcix_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_16(m68ki_get_ea_pcix())) & 0xffff));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_suba_i_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (short)((m68ki_read_imm_16()) & 0xffff));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_suba_d_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (8);
            }


            static void m6000_suba_a_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

               m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - (m68k_cpu.ar[m68k_cpu.ir & 7]));
               m68k_clks_left[0] -= (8);
            }


            static void m6000_suba_ai_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_suba_pi_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4)));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m6000_suba_pd_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4)));
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_suba_di_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff))));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_suba_ix_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_suba_aw_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff)));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_suba_al_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m6000_suba_pcdi_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(ea));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m6000_suba_pcix_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_32(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m6000_suba_i_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] - m68ki_read_imm_32());
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m6000_subi_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = m68ki_read_imm_8();
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subi_ai_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_subi_pi_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_subi_pi7_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_subi_pd_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_subi_pd7_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_subi_di_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subi_ix_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_subi_aw_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subi_al_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_subi_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = m68ki_read_imm_16();
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subi_ai_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_subi_pi_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_subi_pd_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_subi_di_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subi_ix_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_subi_aw_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subi_al_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_subi_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = m68ki_read_imm_32();
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (dst - src);

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (16);
            }


            static void m6000_subi_ai_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_subi_pi_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_subi_pd_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m6000_subi_di_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_subi_ix_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m6000_subi_aw_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_subi_al_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m6000_subq_d_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = ((dst - src) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_subq_ai_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_subq_pi_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_subq_pi7_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_subq_pd_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_subq_pd7_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_subq_di_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_subq_ix_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_subq_aw_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_subq_al_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_subq_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_subq_a_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68k_cpu.ar[m68k_cpu.ir & 7] = (m68k_cpu.ar[m68k_cpu.ir & 7] - ((((m68k_cpu.ir >> 9) - 1) & 7) + 1));
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subq_ai_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_subq_pi_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_subq_pd_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_subq_di_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_subq_ix_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_subq_aw_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_subq_al_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_subq_d_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (dst - src);

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subq_a_32()
            {
                //uint* a_dst = &(m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68k_cpu.ar[m68k_cpu.ir & 7] = (m68k_cpu.ar[m68k_cpu.ir & 7] - ((((m68k_cpu.ir >> 9) - 1) & 7) + 1));
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subq_ai_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subq_pi_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_subq_pd_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_subq_di_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_subq_ix_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_subq_aw_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_subq_al_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_subx_rr_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_subx_rr_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_subx_rr_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];
                uint res = (dst - src - ((m68k_cpu.x_flag != 0)?1u:0u));

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m6000_subx_mm_8_ax7()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m6000_subx_mm_8_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m6000_subx_mm_8_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m6000_subx_mm_8()
            {
                uint src = m68ki_read_8(--((m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea = --((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m6000_subx_mm_16()
            {
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0)?1u:0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (18);
            }


            static void m6000_subx_mm_32()
            {
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src - ((m68k_cpu.x_flag != 0)?1u:0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (30);
            }


            static void m6000_swap()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.dr[m68k_cpu.ir & 7] ^= (*d_dst >> 16) & 0x0000ffff;
                m68k_cpu.dr[m68k_cpu.ir & 7] ^= (*d_dst << 16) & 0xffff0000;
                m68k_cpu.dr[m68k_cpu.ir & 7] ^= (*d_dst >> 16) & 0x0000ffff;

                m68k_cpu.n_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80000000);
                m68k_cpu.not_z_flag = m68k_cpu.dr[m68k_cpu.ir & 7];
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_tas_d()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.not_z_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                m68k_cpu.n_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.dr[m68k_cpu.ir & 7] |= 0x80;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_tas_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_tas_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_tas_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_tas_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m6000_tas_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m6000_tas_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_tas_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m6000_tas_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_tas_al()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m6000_trap()
            {
                m68ki_interrupt(32 + (m68k_cpu.ir & 0xf)); /* HJB 990403 */
            }


            static void m6020_trapt_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapt_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapt_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapf_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapf_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapf_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traphi_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traphi_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traphi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapls_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapls_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapls_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcc_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcc_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcc_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcs_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcs_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapcs_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.c_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapne_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapne_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapne_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapeq_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapeq_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapeq_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvc_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvc_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvc_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvs_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvs_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapvs_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.v_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trappl_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trappl_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trappl_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapmi_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapmi_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapmi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.n_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapge_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapge_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapge_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traplt_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traplt_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traplt_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapgt_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapgt_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_trapgt_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traple_0()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traple_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m6020_traple_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m6000_trapv()
            {
                if (!m68k_cpu.v_flag)
                {
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_interrupt(7); /* HJB 990403 */
            }


            static void m6000_tst_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_tst_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_tst_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_tst_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_tst_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_tst_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_tst_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_tst_aw_8()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6020_tst_pcdi_8()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                    uint res = m68ki_read_8(ea);

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_pcix_8()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_8(m68ki_get_ea_pcix());

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_imm_8()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_imm_8();

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_tst_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6020_tst_a_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_tst_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_tst_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_tst_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_tst_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_tst_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6020_tst_pcdi_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                    uint res = m68ki_read_16(ea);

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_pcix_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_16(m68ki_get_ea_pcix());

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_imm_16()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_imm_16();

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_tst_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6020_tst_a_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_tst_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_tst_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_tst_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_tst_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m6000_tst_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_tst_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m6020_tst_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                    uint res = m68ki_read_32(ea);

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_32(m68ki_get_ea_pcix());

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_tst_imm_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint res = m68ki_read_imm_32();

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_unlk_a7()
            {
                m68k_cpu.ar[7] = m68ki_read_32(m68k_cpu.ar[7]);
                m68k_clks_left[0] -= (12);
            }


            static void m6000_unlk()
            {
                //uint* a_dst = &(m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68k_cpu.ar[7] = m68k_cpu.ar[m68k_cpu.ir & 7];
                m68k_cpu.ar[m68k_cpu.ir & 7] = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);
                m68k_clks_left[0] -= (12);
            }


            static void m6020_unpk_rr()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | (((((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16()) & 0xffff);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_unpk_mm_ax7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_8(--(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]));

                    src = (((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, (src >> 8) & 0xff);
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, src & 0xff);
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_unpk_mm_ay7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);

                    src = (((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16();
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), (src >> 8) & 0xff);
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), src & 0xff);
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_unpk_mm_axy7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);

                    src = (((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, (src >> 8) & 0xff);
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, src & 0xff);
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_unpk_mm()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint src = m68ki_read_8(--(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]));

                    src = (((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16();
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), (src >> 8) & 0xff);
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), src & 0xff);
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }
            static void m6000_dbt()
            {
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbf()
            {
                //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                if (res != 0xffff)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (14);
            }


            static void m6000_dbhi()
            {
                if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbls()
            {
                if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbcc()
            {
                if ((m68k_cpu.c_flag != 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbcs()
            {
                if ((m68k_cpu.c_flag == 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbne()
            {
                if ((m68k_cpu.not_z_flag == 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbeq()
            {
                if ((m68k_cpu.not_z_flag != 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbvc()
            {
                if ((m68k_cpu.v_flag != 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbvs()
            {
                if ((m68k_cpu.v_flag == 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);
                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res!= 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbpl()
            {
                if ((m68k_cpu.n_flag != 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbmi()
            {
                if ((m68k_cpu.n_flag == 0))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbge()
            {
                if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dblt()
            {
                if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dbgt()
            {
                if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_dble()
            {
                if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    //uint* d_reg = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7] - 1) & 0xffff);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (short)((m68ki_read_16(m68k_cpu.pc)) & 0xffff);
                        m68k_clks_left[0] -= (10);
                        return;
                    }
                    m68k_cpu.pc += 2;
                    m68k_clks_left[0] -= (14);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_divs_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_ai_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_pi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_pd_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 6);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 6);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_di_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)))) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_ix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16(m68ki_get_ea_ix())) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_aw_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff))) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_al_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16(m68ki_read_imm_32())) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 12);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 12);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_pcdi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                int src = (short)((m68ki_read_16(ea)) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_pcix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_16(m68ki_get_ea_pcix())) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divs_i_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                int src = (short)((m68ki_read_imm_16()) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) / src;
                    int remainder = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) % src;

                    if (quotient == (short)((quotient) & 0xffff))
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_ai_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_pi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_pd_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 6);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 6);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_di_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_ix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_get_ea_ix());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_aw_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_al_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_read_imm_32());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 12);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 12);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_pcdi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint src = m68ki_read_16(ea);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_pcix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_16(m68ki_get_ea_pcix());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        *d_dst = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6000_divu_i_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint src = m68ki_read_imm_16();

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] / src;
                    uint remainder = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m6020_divl_d_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400)!=0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor)!=0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78);
                                return;
                            }

                            if (((word2) & 0x00000800)!=0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000)!=0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0)?1u:0u);
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000)!=0)
                                {
                                    divisor_neg = 1;
                                    divisor = (uint)(-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)!=0) /* signed */
                            {
                                if (dividend_neg!=0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg!=0)
                                    quotient =(uint) (-quotient);
                            }

                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)!=0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)((int)(dividend_lo) % (int)(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] =(uint)( (int)(dividend_lo) / (int)(divisor));
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_divl_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400)!=0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor)!=0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800)!=0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000)!=0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1u : 0u));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000)!=0)
                                {
                                    divisor_neg = 1;
                                    divisor =(uint) (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)!=0) /* signed */
                            {
                                if (dividend_neg!=0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg!=0)
                                    quotient = (uint)(-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 8);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)!=0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)((int)(dividend_lo) % (int)(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)((int)(dividend_lo) / (int)(divisor));
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 8);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 8);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 8);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 10);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 10);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 10);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 12);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 12);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(m68ki_get_ea_ix());
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 14);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 14);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 14);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 12);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 12);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(m68ki_read_imm_32());
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 16);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 16);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 16);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                    uint divisor = m68ki_read_32(ea);
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 12);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 12);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(m68ki_get_ea_pcix());
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 14);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 14);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 14);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6020_divl_i_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_imm_32();
                    uint dividend_hi = m68k_cpu.dr[word2 & 7];
                    uint dividend_lo = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint quotient = 0;
                    uint remainder = 0;
                    uint dividend_neg = 0;
                    uint divisor_neg = 0;
                    int i;

                    if (divisor != 0)
                    {
                        /* quad / long : long quotient, long remainder */
                        if (((word2) & 0x00000400))
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if (dividend_hi / divisor)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000))
                                {
                                    dividend_neg = 1;
                                    dividend_hi = ((-dividend_hi) - (dividend_lo != 0));
                                    dividend_lo = (-dividend_lo);
                                }
                                if (((divisor) & 0x80000000))
                                {
                                    divisor_neg = 1;
                                    divisor = (-divisor);
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_hi >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }
                            for (i = 31; i >= 0; i--)
                            {
                                quotient <<= 1;
                                remainder = (remainder << 1) + ((dividend_lo >> i) & 1);
                                if (remainder >= divisor)
                                {
                                    remainder -= divisor;
                                    quotient++;
                                }
                            }

                            if (((word2) & 0x00000800)) /* signed */
                            {
                                if (dividend_neg)
                                {
                                    remainder = (-remainder);
                                    quotient = (-quotient);
                                }
                                if (divisor_neg)
                                    quotient = (-quotient);
                            }
                            m68k_cpu.dr[word2 & 7] = remainder;
                            m68k_cpu.dr[(word2 >> 12) & 7] = quotient;

                            m68k_cpu.n_flag = ((quotient) & 0x80000000);
                            m68k_cpu.not_z_flag = quotient;
                            m68k_cpu.v_flag = 0;
                            m68k_clks_left[0] -= (78 + 8);
                            return;
                        }

                        /* long / long: long quotient, maybe long remainder */
                        if (((word2) & 0x00000800)) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (int)(dividend_lo) % (int)(divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (int)(dividend_lo) / (int)(divisor);
                        }
                        else
                        {
                            m68k_cpu.dr[word2 & 7] = (dividend_lo) % (divisor);
                            m68k_cpu.dr[(word2 >> 12) & 7] = (dividend_lo) / (divisor);
                        }

                        m68k_cpu.n_flag = ((quotient) & 0x80000000);
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (78 + 8);
                        return;
                    }
                    m68ki_interrupt(5);
                    return;
                }
                m68000_illegal();
            }
            static void m6000_eor_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7])) & 0xff)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }
            static void m6000_eor_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }
            static void m6000_eor_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }
            static void m6000_eor_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }
            static void m6000_eor_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }
            static void m6000_eor_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }
            static void m6000_eor_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }
            static void m6000_eor_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }
            static void m6000_eor_aw_8()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }
            static void m6000_eor_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }
            static void m6000_eor_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7])) & 0xffff)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }
            static void m6000_eor_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }
            static void m6000_eor_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }
            static void m6000_eor_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }
            static void m6000_eor_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }
            static void m6000_eor_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }
            static void m6000_eor_aw_16()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }
            static void m6000_eor_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }
            static void m6000_eor_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }
            static void m6000_eor_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eor_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eor_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_eor_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_eor_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_eor_aw_32()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_eor_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_eori_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_8()) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_eori_ai_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_eori_pi_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_eori_pi7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_eori_pd_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_eori_pd7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_eori_di_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eori_ix_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_eori_aw_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eori_al_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_read_imm_32();
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_eori_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_16()) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_eori_ai_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_eori_pi_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_eori_pd_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_eori_di_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eori_ix_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_eori_aw_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_eori_al_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_eori_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_32();

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_eori_ai_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_eori_pi_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_eori_pd_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m6000_eori_di_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_eori_ix_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m6000_eori_aw_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_eori_al_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_read_imm_32();
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m6000_eori_to_ccr()
            {
                m68ki_set_ccr(((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)) ^ m68ki_read_imm_16());
                m68k_clks_left[0] -= (20);
            }


            static void m6000_eori_to_sr()
            {
                uint eor_val = m68ki_read_imm_16();

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr((((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)) ^ eor_val);
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_exg_dd()
            {
                //uint* reg_a = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                //uint* reg_b = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint tmp = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = m68k_cpu.dr[m68k_cpu.ir & 7];
                m68k_cpu.dr[m68k_cpu.ir & 7] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m6000_exg_aa()
            {
                //uint* reg_a = &(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                //uint* reg_b = &(m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint tmp = m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7];

                m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7] = m68k_cpu.ar[m68k_cpu.ir & 7];
                m68k_cpu.ar[m68k_cpu.ir & 7] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m6000_exg_da()
            {
                //uint* reg_a = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                //uint* reg_b = &(m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint tmp = m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7];

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = m68k_cpu.ar[m68k_cpu.ir & 7];
                m68k_cpu.ar[m68k_cpu.ir & 7] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m6000_ext_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff) | (((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80) ? 0xff00 : 0);

                m68k_cpu.n_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x8000);
                m68k_cpu.not_z_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_ext_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff) | (((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x8000) ? 0xffff0000 : 0);

                m68k_cpu.n_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80000000);
                m68k_cpu.not_z_flag = m68k_cpu.dr[m68k_cpu.ir & 7];
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6020_extb()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);

                    m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff) | (((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80) ? 0xffffff00 : 0);

                    m68k_cpu.n_flag = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0x80000000);
                    m68k_cpu.not_z_flag = m68k_cpu.dr[m68k_cpu.ir & 7];
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_illegal()
            {
                m68ki_exception(4);
                ;






            }


            static void m6000_jmp_ai()
            {
                m68ki_set_pc((m68k_cpu.ar[m68k_cpu.ir & 7]));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m6000_jmp_di()
            {
                m68ki_set_pc(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m6000_jmp_ix()
            {
                m68ki_set_pc(m68ki_get_ea_ix());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 14);
            }


            static void m6000_jmp_aw()
            {
                m68ki_set_pc((short)((m68ki_read_imm_16()) & 0xffff));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m6000_jmp_al()
            {
                m68ki_set_pc(m68ki_read_imm_32());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m6000_jmp_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                m68ki_set_pc(ea);
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m6000_jmp_pcix()
            {
                m68ki_set_pc(m68ki_get_ea_pcix());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 14);
            }


            static void m6000_jsr_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m6000_jsr_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m6000_jsr_ix()
            {
                uint ea = m68ki_get_ea_ix();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 22);
            }


            static void m6000_jsr_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m6000_jsr_al()
            {
                uint ea = m68ki_read_imm_32();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m6000_jsr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m6000_jsr_pcix()
            {
                uint ea = m68ki_get_ea_pcix();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 22);
            }


            static void m6000_lea_ai()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                m68k_clks_left[0] -= (0 + 4);
            }


            static void m6000_lea_di()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m6000_lea_ix()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_get_ea_ix();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m6000_lea_aw()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_imm_16()) & 0xffff);
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m6000_lea_al()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_imm_32();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m6000_lea_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ea;
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m6000_lea_pcix()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_get_ea_pcix();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m6000_link_16_a7()
            {
                m68k_cpu.ar[7] -= 4;
                m68ki_write_32(m68k_cpu.ar[7], m68k_cpu.ar[7]);
                m68k_cpu.ar[7] = (m68k_cpu.ar[7] + (short)((m68ki_read_imm_16()) & 0xffff));
                m68k_clks_left[0] -= (16);
            }


            static void m6000_link_16()
            {
                //uint* a_dst = &(m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.ar[m68k_cpu.ir & 7]);
                m68k_cpu.ar[m68k_cpu.ir & 7] = m68k_cpu.ar[7];
                m68k_cpu.ar[7] = (m68k_cpu.ar[7] + (short)((m68ki_read_imm_16()) & 0xffff));
                m68k_clks_left[0] -= (16);
            }


            static void m6020_link_32_a7()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    m68k_cpu.ar[7] -= 4;
                    m68ki_write_32(m68k_cpu.ar[7], m68k_cpu.ar[7]);
                    m68k_cpu.ar[7] = (m68k_cpu.ar[7] + m68ki_read_imm_32());
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_link_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    //uint* a_dst = &(m68k_cpu.ar[m68k_cpu.ir & 7]);

                    m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.ar[m68k_cpu.ir & 7]);
                    m68k_cpu.ar[m68k_cpu.ir & 7] = m68k_cpu.ar[7];
                    m68k_cpu.ar[7] = (m68k_cpu.ar[7] + m68ki_read_imm_32());
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_lsr_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = src >> (int)shift;

                m68k_cpu.dr[m68k_cpu.ir & 7]= (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = shift > 8 ? 0 : (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -=(int) ((shift << 1) + 6);
            }


            static void m6000_lsr_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = src >> (int)shift;

                m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m6000_lsr_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = src >>(int) shift;

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m6000_lsr_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = src >> (int)shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 8)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res);
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] &= 0xffffff00;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = 0;
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsr_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = src >> (int)shift;

                m68k_clks_left[0] -=(int) ((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 16)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res);
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] &= 0xffff0000;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = 0;
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsr_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = src >>(int) shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] = 0;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = (shift == 32 ? ((src) & 0x80000000) : 0);
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsr_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_lsr_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_lsr_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_lsr_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_lsr_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_lsr_ea_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_lsr_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_lsl_s_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = ((src << shift) & 0xff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = shift > 8 ? 0 : (src >> (8 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_lsl_s_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((src << (int)shift) & 0xffff);

                m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (16 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= ((shift << 1) + 6);
            }


            static void m6000_lsl_s_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (src << shift);

                m68k_cpu.dr[m68k_cpu.ir & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (32 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m6000_lsl_r_8()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xff);
                uint res = ((src << shift) & 0xff);

                m68k_clks_left[0] -= ((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 8)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (8 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] &= 0xffffff00;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = 0;
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsl_r_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & 0xffff);
                uint res = ((src << shift) & 0xffff);

                m68k_clks_left[0] -= ((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 16)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = ((m68k_cpu.dr[m68k_cpu.ir & 7]) & ~0xffff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (16 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x8000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] &= 0xffff0000;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = 0;
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsl_r_32()
            {
                //uint* d_dst = &(m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = m68k_cpu.dr[m68k_cpu.ir & 7];
                uint res = (src << (int)shift);

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        m68k_cpu.dr[m68k_cpu.ir & 7] = res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (32 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80000000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    m68k_cpu.dr[m68k_cpu.ir & 7] = 0;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = (shift == 32 ? src & 1 : 0);
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = 0;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m6000_lsl_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_lsl_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_lsl_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_lsl_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_lsl_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_lsl_ea_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_lsl_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_dd_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_move_dd_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_dd_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_dd_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_dd_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_move_dd_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_move_dd_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_move_dd_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_move_dd_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_move_dd_i_8()
            {
                uint res = m68ki_read_imm_8();
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_ai_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_ai_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_ai_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_ai_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_ai_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_ai_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_ai_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_ai_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_ai_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_ai_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi7_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pi_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pi7_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi7_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi7_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi7_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pi7_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pi7_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi7_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi7_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi7_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pi7_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi7_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi7_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pi_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pi_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pi_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd7_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pd_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pd7_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd7_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd7_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd7_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pd7_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pd7_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd7_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd7_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd7_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pd7_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd7_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd7_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pd_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pd_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pd_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_di_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_di_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_di_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_di_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_di_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_di_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_di_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_di_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_di_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_di_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_ix_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m6000_move_ix_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_ix_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_ix_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_ix_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m6000_move_ix_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m6000_move_ix_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m6000_move_ix_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m6000_move_ix_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m6000_move_ix_i_8()
            {
                uint res = m68ki_read_imm_8();

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_aw_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_aw_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_aw_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_aw_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_aw_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_aw_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_aw_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_aw_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_aw_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_aw_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_al_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_al_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_al_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_al_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_al_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m6000_move_al_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m6000_move_al_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_al_aw_8()
            {
                uint res = m68ki_read_8((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_al_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_8(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_al_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_dd_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_move_dd_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_move_dd_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_dd_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_dd_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_move_dd_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_move_dd_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_move_dd_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_move_dd_i_16()
            {
                uint res = m68ki_read_imm_16();
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_move_ai_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_ai_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_ai_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_ai_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_ai_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_ai_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_ai_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_ai_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_ai_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_ai_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pi_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pi_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pi_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pi_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pi_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pi_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pi_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pd_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m6000_move_pd_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_pd_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m6000_move_pd_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m6000_move_pd_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m6000_move_pd_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m6000_move_pd_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m6000_move_di_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_di_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_di_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_di_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_di_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_di_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_di_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_di_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_di_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_ix_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m6000_move_ix_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m6000_move_ix_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_ix_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_ix_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m6000_move_ix_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m6000_move_ix_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m6000_move_ix_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m6000_move_ix_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m6000_move_ix_i_16()
            {
                uint res = m68ki_read_imm_16();

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m6000_move_aw_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_aw_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_aw_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_aw_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_aw_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_aw_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_aw_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_aw_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_aw_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_aw_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_al_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_al_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_al_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_al_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_al_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m6000_move_al_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_al_aw_16()
            {
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_al_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_al_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m6000_move_dd_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_move_dd_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_move_dd_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_dd_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_move_dd_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_move_dd_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m6000_move_dd_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_move_dd_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m6000_move_dd_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_move_dd_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m6000_move_dd_i_32()
            {
                uint res = m68ki_read_imm_32();
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_move_ai_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_ai_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_ai_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_ai_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_ai_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_ai_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_ai_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_ai_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_ai_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_move_ai_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_ai_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_ai_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pi_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_pi_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_pi_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pi_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pi_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_pi_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pi_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_pi_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pi_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_move_pi_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pi_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_pi_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pd_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_pd_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_pd_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pd_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_pd_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_pd_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pd_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_pd_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pd_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m6000_move_pd_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_pd_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m6000_move_pd_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_di_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_di_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_di_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_di_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_di_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_di_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_di_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m6000_move_di_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_di_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 16);
            }


            static void m6000_move_di_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_di_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m6000_move_di_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_ix_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_move_ix_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18);
            }


            static void m6000_move_ix_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m6000_move_ix_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m6000_move_ix_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 10);
            }


            static void m6000_move_ix_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m6000_move_ix_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 14);
            }


            static void m6000_move_ix_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m6000_move_ix_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 16);
            }


            static void m6000_move_ix_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m6000_move_ix_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 14);
            }


            static void m6000_move_ix_i_32()
            {
                uint res = m68ki_read_imm_32();

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m6000_move_aw_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_aw_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m6000_move_aw_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_aw_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_aw_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m6000_move_aw_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_aw_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m6000_move_aw_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_aw_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 16);
            }


            static void m6000_move_aw_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m6000_move_aw_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m6000_move_aw_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (short)((m68ki_read_imm_16()) & 0xffff);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m6000_move_al_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20);
            }


            static void m6000_move_al_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20);
            }


            static void m6000_move_al_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_move_al_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_move_al_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m6000_move_al_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_move_al_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m6000_move_al_aw_32()
            {
                uint res = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_move_al_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m6000_move_al_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_32(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m6000_move_al_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m6000_move_al_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m6000_movea_d_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_movea_a_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)(((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                m68k_clks_left[0] -= (4);
            }


            static void m6000_movea_ai_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))) & 0xffff);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_movea_pi_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))) & 0xffff);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_movea_pd_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))) & 0xffff);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m6000_movea_di_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)))) & 0xffff);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_movea_ix_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(m68ki_get_ea_ix())) & 0xffff);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_movea_aw_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff))) & 0xffff);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_movea_al_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(m68ki_read_imm_32())) & 0xffff);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_movea_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(ea)) & 0xffff);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_movea_pcix_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_16(m68ki_get_ea_pcix())) & 0xffff);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_movea_i_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (short)((m68ki_read_imm_16()) & 0xffff);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m6000_movea_d_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m6000_movea_a_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m6000_movea_ai_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_movea_pi_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6000_movea_pd_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m6000_movea_di_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_movea_ix_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_get_ea_ix());
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m6000_movea_aw_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_movea_al_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_read_imm_32());
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m6000_movea_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(ea);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m6000_movea_pcix_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_get_ea_pcix());
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m6000_movea_i_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_imm_32();
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m6010_move_fr_ccr_d()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0));
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_ai()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((m68k_cpu.ar[m68k_cpu.ir & 7]), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_pi()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_pd()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 6);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_di()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_ix()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(m68ki_get_ea_ix(), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_aw()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((short)((m68ki_read_imm_16()) & 0xffff), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_move_fr_ccr_al()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(m68ki_read_imm_32(), ((((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_move_to_ccr_d()
            {
                m68ki_set_ccr((m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (12);
            }


            static void m6000_move_to_ccr_ai()
            {
                m68ki_set_ccr(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_to_ccr_pi()
            {
                m68ki_set_ccr(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_to_ccr_pd()
            {
                m68ki_set_ccr(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m6000_move_to_ccr_di()
            {
                m68ki_set_ccr(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff))));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_to_ccr_ix()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_to_ccr_aw()
            {
                m68ki_set_ccr(m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff)));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_to_ccr_al()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m6000_move_to_ccr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                m68ki_set_ccr(m68ki_read_16(ea));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m6000_move_to_ccr_pcix()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m6000_move_to_ccr_i()
            {
                m68ki_set_ccr(m68ki_read_imm_16());
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m6000_move_fr_sr_d()
            {
                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0));
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_ix()
            {
                uint ea = m68ki_get_ea_ix();

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_aw()
            {
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_sr_al()
            {
                uint ea = m68ki_read_imm_32();

                if ((m68k_cpu.mode & 1) || m68k_cpu.s_flag) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) << 15) | ((m68k_cpu.t0_flag != 0) << 14) | ((m68k_cpu.s_flag != 0) << 13) | ((m68k_cpu.m_flag != 0) << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0)?1u:0u) << 4) | ((m68k_cpu.n_flag != 0) << 3) | ((m68k_cpu.not_z_flag == 0) << 2) | ((m68k_cpu.v_flag != 0) << 1) | (m68k_cpu.c_flag != 0)));
                    m68k_clks_left[0] -= (8 + 12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_d()
            {
                if (m68k_cpu.s_flag)
                {
                    m68ki_set_sr((m68k_cpu.dr[m68k_cpu.ir & 7]));
                    m68k_clks_left[0] -= (12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_ai()
            {
                uint new_sr = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_pi()
            {
                uint new_sr = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_pd()
            {
                uint new_sr = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_di()
            {
                uint new_sr = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_ix()
            {
                uint new_sr = m68ki_read_16(m68ki_get_ea_ix());

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_aw()
            {
                uint new_sr = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff));

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_al()
            {
                uint new_sr = m68ki_read_16(m68ki_read_imm_32());

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint new_sr = m68ki_read_16(ea);

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_pcix()
            {
                uint new_sr = m68ki_read_16(m68ki_get_ea_pcix());

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_sr_i()
            {
                uint new_sr = m68ki_read_imm_16();

                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_fr_usp()
            {
                if (m68k_cpu.s_flag)
                {
                    (m68k_cpu.ar[m68k_cpu.ir & 7]) = m68k_cpu.sp[0];
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6000_move_to_usp()
            {
                if (m68k_cpu.s_flag)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.sp[0] = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m6010_movec_cr()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    if (m68k_cpu.s_flag)
                    {
                        uint next_word = m68ki_read_imm_16();

                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_clks_left[0] -= (12);
                        switch (next_word & 0xfff)
                        {
                            case 0x000: /* SFC */
                                m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.sfc;
                                return;
                            case 0x001: /* DFC */
                                m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.dfc;
                                return;
                            case 0x002: /* CACR */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.cacr;
                                    return;
                                }
                                return;
                            case 0x800: /* USP */
                                m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.sp[0];
                                return;
                            case 0x801: /* VBR */
                                m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.vbr;
                                return;
                            case 0x802: /* CAAR */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.caar;
                                    return;
                                }
                                m68000_illegal();
                                break;
                            case 0x803: /* MSP */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.sp[3];
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x804: /* ISP */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.sp[1];
                                    return;
                                }
                                m68000_illegal();
                                return;
                            default:



                                m68000_illegal();
                                return;
                        }
                    }
                    m68ki_exception(8);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_movec_rc()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    if (m68k_cpu.s_flag)
                    {
                        uint next_word = m68ki_read_imm_16();

                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_clks_left[0] -= (10);
                        switch (next_word & 0xfff)
                        {
                            case 0x000: /* SFC */
                                m68k_cpu.sfc = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                return;
                            case 0x001: /* DFC */
                                m68k_cpu.dfc = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                return;
                            case 0x002: /* CACR */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu.cacr = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x800: /* USP */
                                m68k_cpu.sp[0] = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7];
                                return;
                            case 0x801: /* VBR */
                                m68k_cpu.vbr = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7];
                                return;
                            case 0x802: /* CAAR */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu.caar = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x803: /* MSP */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu.sp[3] = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x804: /* ISP */
                                if ((m68k_cpu.mode & (4 | 8))!=0)
                                {
                                    m68k_cpu.sp[1] = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            default:



                                m68000_illegal();
                                return;
                        }
                    }
                    m68ki_exception(8);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_movem_pd_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        ea -= 2;
                        m68ki_write_16(ea, *(m68k_movem_pd_table[i]));
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                m68k_clks_left[0] -= ((count << 2) + 8);
            }


            static void m6000_movem_pd_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        ea -= 4;
                        m68ki_write_32(ea, *(m68k_movem_pd_table[i]));
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8);
            }


            static void m6000_movem_pi_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                m68k_clks_left[0] -= ((count << 2) + 12);
            }


            static void m6000_movem_pi_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 12);
            }


            static void m6000_movem_re_ai_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_16(ea, *(m68k_movem_pi_table[i]));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 4 + 4);
            }


            static void m6000_movem_re_di_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_16(ea, *(m68k_movem_pi_table[i]));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 4 + 8);
            }


            static void m6000_movem_re_ix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_16(ea, *(m68k_movem_pi_table[i]));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 4 + 10);
            }


            static void m6000_movem_re_aw_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_16(ea, *(m68k_movem_pi_table[i]));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 4 + 8);
            }


            static void m6000_movem_re_al_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_16(ea, *(m68k_movem_pi_table[i]));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 4 + 12);
            }


            static void m6000_movem_re_ai_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_32(ea, *(m68k_movem_pi_table[i]));
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 4 + 4);
            }


            static void m6000_movem_re_di_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_32(ea, *(m68k_movem_pi_table[i]));
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 4 + 8);
            }


            static void m6000_movem_re_ix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_32(ea, *(m68k_movem_pi_table[i]));
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 4 + 10);
            }


            static void m6000_movem_re_aw_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_32(ea, *(m68k_movem_pi_table[i]));
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 4 + 8);
            }


            static void m6000_movem_re_al_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        m68ki_write_32(ea, *(m68k_movem_pi_table[i]));
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 4 + 12);
            }


            static void m6000_movem_er_ai_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 4);
            }


            static void m6000_movem_er_di_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 8);
            }


            static void m6000_movem_er_ix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 10);
            }


            static void m6000_movem_er_aw_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 8);
            }


            static void m6000_movem_er_al_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 12);
            }


            static void m6000_movem_er_pcdi_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 8);
            }


            static void m6000_movem_er_pcix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_pcix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = (short)((((m68ki_read_16(ea)) & 0xffff)) & 0xffff);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= ((count << 2) + 8 + 10);
            }


            static void m6000_movem_er_ai_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 4);
            }


            static void m6000_movem_er_di_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 8);
            }


            static void m6000_movem_er_ix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 10);
            }


            static void m6000_movem_er_aw_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (short)((m68ki_read_imm_16()) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 8);
            }


            static void m6000_movem_er_al_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 12);
            }


            static void m6000_movem_er_pcdi_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 8);
            }


            static void m6000_movem_er_pcix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_pcix();
                uint count = 0;

                for (; i < 16; i++)
                    if (register_list & (1 << i))
                    {
                        *(m68k_movem_pi_table[i]) = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= ((count << 3) + 8 + 10);
            }


            static void m6000_movep_re_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((((m68ki_read_imm_16()) & 0xffff)) & 0xffff);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea, ((src >> 8) & 0xff));
                m68ki_write_8(ea += 2, ((src) & 0xff));
                m68k_clks_left[0] -= (16);
            }


            static void m6000_movep_re_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((((m68ki_read_imm_16()) & 0xffff)) & 0xffff);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea, ((src >> 24) & 0xff));
                m68ki_write_8(ea += 2, ((src >> 16) & 0xff));
                m68ki_write_8(ea += 2, ((src >> 8) & 0xff));
                m68ki_write_8(ea += 2, ((src) & 0xff));
                m68k_clks_left[0] -= (24);
            }


            static void m6000_movep_er_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((((m68ki_read_imm_16()) & 0xffff)) & 0xffff);
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & ~0xffff) | ((m68ki_read_8(ea) << 8) + m68ki_read_8(ea + 2));
                m68k_clks_left[0] -= (16);
            }


            static void m6000_movep_er_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((((m68ki_read_imm_16()) & 0xffff)) & 0xffff);

                (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) = (m68ki_read_8(ea) << 24) + (m68ki_read_8(ea + 2) << 16)
                 + (m68ki_read_8(ea + 4) << 8) + m68ki_read_8(ea + 6);
                m68k_clks_left[0] -= (24);
            }


            static void m6010_moves_ai_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 18);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pi_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pi7_8()
            {
                
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[7] += 2) - 2);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pd_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pd7_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[7] -= 2);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_di_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_ix_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 24);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_aw_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_al_8()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 24);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (sbyte)((m68ki_read_8_fc(ea, m68k_cpu.sfc)) & 0xff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_ai_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 18);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pi_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pd_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_di_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_ix_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 24);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_aw_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_al_16()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 24);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (short)((m68ki_read_16_fc(ea, m68k_cpu.sfc)) & 0xffff);
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_ai_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 8);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pi_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 8);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_pd_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 10);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_di_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 12);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_ix_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 14);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_aw_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (short)((m68ki_read_imm_16()) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 12);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6010_moves_al_32()
            {
                if( (m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 16);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_32_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    /* Memory to register */
                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68ki_read_32_fc(ea, m68k_cpu.sfc);
                    return;
                }
                m68000_illegal();
            }


            static void m6000_moveq()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) = (sbyte)((((m68k_cpu.ir) & 0xff)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m6000_muls_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54);
            }


            static void m6000_muls_ai_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6000_muls_pi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6000_muls_pd_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 6);
            }


            static void m6000_muls_di_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)))) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_muls_ix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16(m68ki_get_ea_ix())) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m6000_muls_aw_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff))) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_muls_al_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16(m68ki_read_imm_32())) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 12);
            }


            static void m6000_muls_pcdi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = (short)((m68ki_read_16(ea)) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_muls_pcix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_16(m68ki_get_ea_pcix())) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m6000_muls_i_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (short)((m68ki_read_imm_16()) & 0xffff) * (short)((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff)) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6000_mulu_d_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54);
            }


            static void m6000_mulu_ai_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6000_mulu_pi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6000_mulu_pd_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 6);
            }


            static void m6000_mulu_di_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff))) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_mulu_ix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16(m68ki_get_ea_ix()) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m6000_mulu_aw_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16((short)((m68ki_read_imm_16()) & 0xffff)) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_mulu_al_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16(m68ki_read_imm_32()) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 12);
            }


            static void m6000_mulu_pcdi_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                uint res = m68ki_read_16(ea) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m6000_mulu_pcix_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_16(m68ki_get_ea_pcix()) * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m6000_mulu_i_16()
            {
                //uint* d_dst = &(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = m68ki_read_imm_16() * ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0xffff);

                m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m6020_mull_d_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) && neg)!=0)
                    {
                        hi = ((-hi) - (lo != 0)?1u:0u);
                        lo =(uint) (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) && neg)!=0)
                    {
                        hi = ((-hi) - (lo != 0)?1u:0u);
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 8);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) && neg)!=0)
                    {
                        hi = ((-hi) - (lo != 0)?1u:0u);
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 8);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) && neg!=0))
                    {
                        hi = ((-hi) - ((lo != 0)?1u:0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 10);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)((m68ki_read_imm_16()) & 0xffff)));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg!=0)
                    {
                        hi = ((-hi) - ((lo != 0)?1u:0u));
                        lo = (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 12);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(m68ki_get_ea_ix());
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)) /* signed */
                    {
                        if (((src) & 0x80000000))
                            src = (-src);
                        if (((dst) & 0x80000000))
                            dst = (-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg)
                    {
                        hi = ((-hi) - (lo != 0));
                        lo = (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400))
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 14);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32((short)((m68ki_read_imm_16()) & 0xffff));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)) /* signed */
                    {
                        if (((src) & 0x80000000))
                            src = (-src);
                        if (((dst) & 0x80000000))
                            dst = (-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg)
                    {
                        hi = ((-hi) - (lo != 0));
                        lo = (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400))
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 12);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(m68ki_read_imm_32());
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)) /* signed */
                    {
                        if (((src) & 0x80000000))
                            src = (-src);
                        if (((dst) & 0x80000000))
                            dst = (-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg)
                    {
                        hi = ((-hi) - (lo != 0));
                        lo = (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400))
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 16);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 16);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (short)((m68ki_read_16(old_pc)) & 0xffff);
                    uint src = m68ki_read_32(ea);
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)) /* signed */
                    {
                        if (((src) & 0x80000000))
                            src = (-src);
                        if (((dst) & 0x80000000))
                            dst = (-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg)
                    {
                        hi = ((-hi) - (lo != 0));
                        lo = (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400))
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 12);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(m68ki_get_ea_pcix());
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = r1 + ((r2 << 16) & ~0xffff);
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + (lo < r1);

                    r1 = lo;
                    lo = r1 + ((r3 << 16) & ~0xffff);
                    hi += (lo < r1) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) && neg!=0)
                    {
                        hi = ((-hi) - ((lo != 0)?1u:0u));
                        lo =(uint) (-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 14);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag =((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m6020_mull_i_32()
            {
                if ((m68k_cpu.mode & (4 | 8))!=0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_imm_32();
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800)!=0) /* signed */
                    {
                        if (((src) & 0x80000000)!=0)
                            src =(uint) (-src);
                        if (((dst) & 0x80000000)!=0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = (uint)((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (uint)((lo < r1)?1u:0u) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg!=0))
                    {
                        hi = (uint)((-hi) - ((lo != 0)?1u:0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400)!=0)
                    {
                        m68k_cpu.dr[word2 & 7] = hi;
                        m68k_cpu.n_flag = ((hi) & 0x80000000);
                        m68k_cpu.not_z_flag = hi | lo;
                        m68k_cpu.v_flag = 0;
                        m68k_clks_left[0] -= (43 + 8);
                        return;
                    }

                    m68k_cpu.n_flag = ((lo) & 0x80000000);
                    m68k_cpu.not_z_flag = lo;
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }


        }
    }
}

#endif