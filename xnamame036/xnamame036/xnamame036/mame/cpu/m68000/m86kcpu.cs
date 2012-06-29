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
            static m68k_cpu_core m68k_cpu = new m68k_cpu_core();
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
            const int M68K_CPU_MODE_68000 = 1;
            const int M68K_CPU_MODE_68010 = 2;
            const int M68K_CPU_MODE_68EC020 = 4;
            const int M68K_CPU_MODE_68020 = 8;

            const int CPU_MODE_000 = M68K_CPU_MODE_68000;
            const int CPU_MODE_010 = M68K_CPU_MODE_68010;
            const int CPU_MODE_EC020 = M68K_CPU_MODE_68EC020;
            const int CPU_MODE_020 = M68K_CPU_MODE_68020;

            const int CPU_MODE_ALL = (CPU_MODE_000 | CPU_MODE_010 | CPU_MODE_EC020 | CPU_MODE_020);
            const int CPU_MODE_010_PLUS = (CPU_MODE_010 | CPU_MODE_EC020 | CPU_MODE_020);
            const int CPU_MODE_010_LESS = (CPU_MODE_000 | CPU_MODE_010);
            const int CPU_MODE_EC020_PLUS = (CPU_MODE_EC020 | CPU_MODE_020);
            const int CPU_MODE_EC020_LESS = (CPU_MODE_000 | CPU_MODE_010 | CPU_MODE_EC020);
            const int CPU_MODE_020_PLUS = CPU_MODE_020;
            const int CPU_MODE_020_LESS = (CPU_MODE_000 | CPU_MODE_010 | CPU_MODE_EC020 | CPU_MODE_020);

            /* Function codes set by CPU during data/address bus activity */
            const int FUNCTION_CODE_USER_DATA = 1;
            const int FUNCTION_CODE_USER_PROGRAM = 2;
            const int FUNCTION_CODE_SUPERVISOR_DATA = 5;
            const int FUNCTION_CODE_SUPERVISOR_PROGRAM = 6;
            const int FUNCTION_CODE_CPU_SPACE = 7;


            static uint m68k_emulation_initialized = 0; /* flag if emulation has been initialized */
            static byte[] m68k_int_masks = { 0xfe, 0xfc, 0xf8, 0xf0, 0xe0, 0xc0, 0x80, 0x80 };

            /* Used by shift & rotate instructions */
            static byte[] m68k_shift_8_table =
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


            static UIntSubArray[] m68k_cpu_dar = { new UIntSubArray(m68k_cpu.dr), new UIntSubArray(m68k_cpu.ar) };
            /* Pointers to speed up movem instructions */
            static UIntSubArray[] m68k_movem_pi_table =
{
  new UIntSubArray( m68k_cpu.dr), new UIntSubArray(m68k_cpu.dr,1), new UIntSubArray(m68k_cpu.dr,2), new UIntSubArray(m68k_cpu.dr,3), new UIntSubArray(m68k_cpu.dr,4), new UIntSubArray(m68k_cpu.dr,5), new UIntSubArray(m68k_cpu.dr,6), new UIntSubArray(m68k_cpu.dr,7),
  new UIntSubArray( m68k_cpu.ar), new UIntSubArray(m68k_cpu.ar,1), new UIntSubArray(m68k_cpu.ar,2), new UIntSubArray(m68k_cpu.ar,3), new UIntSubArray(m68k_cpu.ar,4), new UIntSubArray(m68k_cpu.ar,5), new UIntSubArray(m68k_cpu.ar,6), new UIntSubArray(m68k_cpu.ar,7)
};
            static UIntSubArray[] m68k_movem_pd_table =
{
   new UIntSubArray(m68k_cpu.ar,7), new UIntSubArray(m68k_cpu.ar,6), new UIntSubArray(m68k_cpu.ar,5),new UIntSubArray(m68k_cpu.ar,4), new UIntSubArray(m68k_cpu.ar,3), new UIntSubArray(m68k_cpu.ar,2), new UIntSubArray(m68k_cpu.ar,1), new UIntSubArray(m68k_cpu.ar),
   new UIntSubArray(m68k_cpu.dr,7), new UIntSubArray(m68k_cpu.dr,6), new UIntSubArray(m68k_cpu.dr,5),new UIntSubArray(m68k_cpu.dr,4), new UIntSubArray(m68k_cpu.dr,3), new UIntSubArray(m68k_cpu.dr,2), new UIntSubArray(m68k_cpu.dr,1), new UIntSubArray(m68k_cpu.dr),
};

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
   0xf71f};

            static uint m68ki_read_32(uint address)
            {
                return (uint)cpu_readmem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
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
            void m68k_set_int_ack_callback(irqcallback callback)
            {
                m68k_cpu.int_ack_callback = callback != null ? callback : default_int_ack_callback;
            }

            void m68k_set_bkpt_ack_callback(_void_set_int_callback callback)
            {
                m68k_cpu.bkpt_ack_callback = callback != null ? callback : default_bkpt_ack_callback;
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
            static int MAKE_INT_8(int value)
            {
                /* Will this work for 1's complement machines? */
                return (value & 0x80) != 0 ? value | ~0xff : value & 0xff;
            }
            static int MAKE_INT_8(uint value)
            {
                /* Will this work for 1's complement machines? */
                return (int)((value & 0x80) != 0 ? value | ~0xff : value & 0xff);
            }
            static int MAKE_INT_16(uint value)
            {
                /* Will this work for 1's complement machines? */
                return (int)((value & 0x8000) != 0 ? value | ~0xffff : value & 0xffff);
            }
            static int MAKE_INT_16(int value)
            {
                /* Will this work for 1's complement machines? */
                return (value & 0x8000) != 0 ? value | ~0xffff : value & 0xffff;
            }
            static int MAKE_INT_32(int value)
            {
                /* Will this work for 1's complement machines? */
                return (int)((value & 0x80000000) != 0 ? value | ~0xffffffff : value & 0xffffffff);
            }
            static int MAKE_INT_32(uint value)
            {
                /* Will this work for 1's complement machines? */
                return (int)((value & 0x80000000) != 0 ? value | ~0xffffffff : value & 0xffffffff);
            }

            static uint m68ki_read_imm_8()
            {
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return ((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xff);

            }
            static uint m68ki_read_imm_32()
            {

                uint temp_val;


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
            static uint m68ki_read_16(uint address)
            {
                return (uint)(cpu_readmem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff)));
            }
            static uint m68ki_read_imm_16()
            {
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = (uint)((cpu_readop16((uint)((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return ((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xffff);
            }
            void m68k_set_instr_hook_callback(opcode callback)
            {
                m68k_cpu.instr_hook_callback = callback != null ? callback : default_instr_hook_callback;
            }
            static void m68ki_write_16(uint address, uint value)
            {
                cpu_writemem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_32(uint address, uint value)
            {
                cpu_writemem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_set_s_flag(int value)
            {
                /* ASG: Only do the rest if we're changing */
                value = (value != 0) ? 1 : 0;
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
            static void m68ki_write_8(uint address, uint value)
            {
                cpu_writemem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static uint m68ki_read_8(uint address)
            {
                return (uint)cpu_readmem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static void m68ki_exception(uint vector)
            {
                /* Save the old status register */
                uint old_sr = (uint)(((m68k_cpu.t1_flag != 0) ? 1 : 0 << 15) | ((m68k_cpu.t0_flag != 0) ? 1 : 0 << 14) | ((m68k_cpu.s_flag != 0) ? 1 : 0 << 13) | ((m68k_cpu.m_flag != 0) ? 1 : 0 << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1 : 0 << 4) | ((m68k_cpu.n_flag != 0) ? 1 : 0 << 3) | ((m68k_cpu.not_z_flag == 0) ? 1 : 0 << 2) | ((m68k_cpu.v_flag != 0) ? 1 : 0 << 1) | ((m68k_cpu.c_flag != 0) ? 1 : 0));

                /* Use up some clock cycles */
                m68k_clks_left[0] -= (m68k_exception_cycle_table[vector]);

                /* Turn off stopped state and trace flag, clear pending traces */
                m68k_cpu.stopped = 0;
                m68k_cpu.t1_flag = m68k_cpu.t0_flag = 0;

                /* Enter supervisor mode */
                m68ki_set_s_flag(1);
                /* Push a stack frame */
                if ((m68k_cpu.mode & (2 | 4 | 8)) != 0)
                    m68ki_write_16(m68k_cpu.ar[7] -= 2, vector << 2); /* This is format 0 */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.ppc); /* save previous PC, ie. PC that contains an offending instruction */
                m68ki_write_16(m68k_cpu.ar[7] -= 2, old_sr);
                /* Generate a new program counter from the vector */
                m68ki_set_pc(m68ki_read_32((vector << 2) + m68k_cpu.vbr));
            }
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
                m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((uint)(((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));

                m68k_clks_left[0] = 0;

                if (m68k_cpu.mode == 0) m68k_cpu.mode = 1; /* KW 990319 */
                /* The first call to this function initializes the opcode handler jump table */
                if (m68k_emulation_initialized != 0)
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
            static uint m68ki_get_ea_ix()
            {
                uint extension = m68ki_read_imm_16();
                uint ea_index = m68k_cpu_dar[(((extension) & 0x00008000) != 0) ? (int)1 : (int)0][(int)(((extension) >> 12) & 7)];
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint outer = 0;

                /* Sign-extend the index value if needed */
                if (((extension) & 0x00000800) == 0)
                    ea_index = (uint)MAKE_INT_16(ea_index);

                /* If we're running 010 or less, there's no scale or full extension word mode */
                if ((m68k_cpu.mode & (1 | 2)) != 0)
                    return (uint)(_base + ea_index + MAKE_INT_8((int)extension));

                /* Scale the index value */
                ea_index <<= (int)(((extension) >> 9) & 3);

                /* If we're using brief extension mode, we are done */
                if (((extension) & 0x00000100) == 0)
                    return (uint)(_base + ea_index + MAKE_INT_8((int)extension));

                /* Decode the long extension format */
                if (((extension) & 0x00000040) != 0)
                    ea_index = 0;
                if (((extension) & 0x00000080) != 0)
                    _base = 0;
                if (((extension) & 0x00000020) != 0)
                    _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
                if (((extension) & 7) == 0)
                    return _base + ea_index;

                if (((extension) & 0x00000002) != 0)
                    outer = ((extension) & 0x00000001) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
                if (((extension) & 0x00000004) != 0)
                    return m68ki_read_32(_base) + ea_index + outer;
                return m68ki_read_32(_base + ea_index) + outer;
            }

            static void m68ki_interrupt(uint vector)
            {
                /* Save the old status register */
                uint old_sr = (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u));

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
                if ((m68k_cpu.mode & (2 | 4 | 8)) != 0)
                    m68ki_write_16(m68k_cpu.ar[7] -= 2, vector << 2); /* This is format 0 */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_write_16(m68k_cpu.ar[7] -= 2, old_sr);
                /* Generate a new program counter from the vector */
                m68ki_set_pc(m68ki_read_32((vector << 2) + m68k_cpu.vbr));
            }
            static void m68ki_write_8_fc(uint address, uint fc, uint value)
            {
                cpu_writemem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_16_fc(uint address, uint fc, uint value)
            {
                cpu_writemem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static void m68ki_write_32_fc(uint address, uint fc, uint value)
            {
                cpu_writemem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff), (int)value);
            }
            static uint m68ki_read_8_fc(uint address, uint fc)
            {
                return (uint)cpu_readmem24((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_16_fc(uint address, uint fc)
            {
                return (uint)cpu_readmem24_word((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_read_32_fc(uint address, uint fc)
            {
                return (uint)cpu_readmem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
            }
            static uint m68ki_get_ea_ix_dst()
            {
                uint extension = m68ki_read_imm_16();
                uint ea_index = m68k_cpu_dar[(((extension) & 0x00008000) != 0) ? 1 : 0][(((extension) >> 12) & 7)];
                uint _base = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]); /* This is the only thing different from m68ki_get_ea_ix() */
                uint outer = 0;

                /* Sign-extend the index value if needed */
                if (((extension) & 0x00000800) == 0)
                    ea_index = (uint)MAKE_INT_16(ea_index);

                /* If we're running 010 or less, there's no scale or full extension word mode */
                if ((m68k_cpu.mode & (1 | 2)) != 0)
                    return (_base + ea_index + (uint)MAKE_INT_8(extension));

                /* Scale the index value */
                ea_index <<= (int)(((extension) >> 9) & 3);

                /* If we're using brief extension mode, we are done */
                if (((extension) & 0x00000100) == 0)
                    return (_base + ea_index + (uint)MAKE_INT_8(extension));

                /* Decode the long extension format */
                if (((extension) & 0x00000040) != 0)
                    ea_index = 0;
                if (((extension) & 0x00000080) != 0)
                    _base = 0;
                if (((extension) & 0x00000020) != 0)
                    _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
                if (((extension) & 7) == 0)
                    return (_base + ea_index);

                if (((extension) & 0x00000002) != 0)
                    outer = ((extension) & 0x00000001) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
                if (((extension) & 0x00000004) != 0)
                    return (m68ki_read_32(_base) + ea_index + outer);
                return (m68ki_read_32(_base + ea_index) + outer);
            }
            static void m68ki_set_ccr(uint value)
            {
                m68k_cpu.x_flag = ((value) & 0x00000010);
                m68k_cpu.n_flag = ((value) & 0x00000008);
                m68k_cpu.not_z_flag = ((value) & 0x00000004) == 0 ? 1u : 0u;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
            }
            static void m68ki_set_sm_flag(int s_value, int m_value)
            {
                /* ASG: Only do the rest if we're changing */
                s_value = (s_value != 0) ? 1 : 0;
                m_value = ((m_value != 0 && (m68k_cpu.mode & (4 | 8)) != 0) ? 1 : 0) << 1;
                if (m68k_cpu.s_flag != s_value || m68k_cpu.m_flag != m_value)
                {
                    /* Backup the old stack pointer */
                    m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))] = m68k_cpu.ar[7];
                    /* Set the S and M flags */
                    m68k_cpu.s_flag = (s_value != 0) ? 1u : 0u;
                    m68k_cpu.m_flag = (m_value != 0 && (m68k_cpu.mode & (4 | 8)) != 0) ? 1u : 0u << 1;
                    /* Set the new stack pointer */
                    m68k_cpu.ar[7] = m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))];
                }
            }
            static void m68ki_check_interrupts()
            {
                uint pending_mask = 1u << (int)m68k_cpu.int_state;
                if ((pending_mask & m68k_int_masks[m68k_cpu.int_mask]) != 0)
                    m68ki_service_interrupt(pending_mask);
            }
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
                m68k_cpu.not_z_flag = ((value) & 0x00000004) == 0 ? 1u : 0u;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
                m68ki_set_sm_flag((int)((value) & 0x00002000), (int)((value) & 0x00001000));

                /* ASG: detect changes to the INT_MASK */
                if (m68k_cpu.int_mask != old_mask)
                    m68ki_check_interrupts();
            }
            static void m68ki_service_interrupt(uint pending_mask) /* ASG: added parameter here */
            {
                uint int_level = 7;
                uint vector;

                /* Start at level 7 and then go down */
                for (; (pending_mask & (1u << (int)int_level)) == 0; int_level--) /* ASG: changed to use parameter instead of CPU_INTS_PENDING */
                    ;

                /* Get the exception vector */
                switch (vector = (uint)m68k_cpu.int_ack_callback((int)int_level))
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
                    case unchecked((uint)-1):
                        /* Use the autovectors.  This is the most commonly used implementation */
                        vector = 24 + int_level;
                        break;
                    case unchecked((uint)-2):
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
            static uint m68k_peek_pc() { return ((m68k_cpu.mode & 8) != 0 ? m68k_cpu.pc : (m68k_cpu.pc) & 0xffffff); }
            static uint m68ki_read_instruction()
            {
                if (((m68k_cpu.pc) & ~3) != m68k_cpu.pref_addr)
                {
                    m68k_cpu.pref_addr = (uint)((m68k_cpu.pc) & ~3);
                    m68k_cpu.pref_data = ((cpu_readop16(((m68k_cpu.mode & 8)!=0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) << 16) | cpu_readop16((((m68k_cpu.mode & 8)!=0 ? m68k_cpu.pref_addr : (m68k_cpu.pref_addr) & 0xffffff)) + 2));
                }
                m68k_cpu.pc += 2;
                return ((m68k_cpu.pref_data >> (int)((2 - ((m68k_cpu.pc - 2) & 2)) << 3)) & 0xffff);
            }

            static int m68k_execute(int num_clks)
            {
                /* Make sure we're not stopped */
                if (m68k_cpu.stopped == 0)
                {
                    /* Set our pool of clock cycles available */
                    m68k_clks_left[0] = num_clks;

                    /* ASG: update cycles */
                    m68k_clks_left[0] -= (int)m68k_cpu.int_cycles;
                    m68k_cpu.int_cycles = 0;

                    /* Main loop.  Keep going until we run out of clock cycles */
                    do
                    {
                        m68k_cpu.ppc = m68k_cpu.pc;

                        /* Read an instruction and call its handler */
                        m68k_cpu.ir = m68ki_read_instruction();
                        m68k_instruction_jump_table[m68k_cpu.ir]();

                        /* Trace m68k_exception, if necessary */
                        continue;
                    } while (m68k_clks_left[0] > 0);

                    /* set previous PC to current PC for the next entry into the loop */
                    m68k_cpu.ppc = m68k_cpu.pc;

                    /* ASG: update cycles */
                    m68k_clks_left[0] -= (int)m68k_cpu.int_cycles;
                    m68k_cpu.int_cycles = 0;

                    /* return how many clocks we used */
                    return num_clks - m68k_clks_left[0];
                }

                /* We get here if the CPU is stopped */
                m68k_clks_left[0] = 0;

                return num_clks;
            }

            void m68k_assert_irq(int int_line)
            {
                /* OR in the bits of the interrupt */
                int old_state = (int)m68k_cpu.int_state;
                m68k_cpu.int_state = 0; /* ASG: remove me to do proper mask setting */
                m68k_cpu.int_state |= (uint)(int_line & 7);

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
                m68k_cpu.int_state &= (uint)~int_line & 7;
                m68k_cpu.int_state = 0; /* ASG: remove me to do proper mask setting */

                /* check for interrupts again */
                m68ki_check_interrupts();
            }

        }
    }
}