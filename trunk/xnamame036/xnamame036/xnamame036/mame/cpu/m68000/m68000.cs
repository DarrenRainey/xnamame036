using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_m68000
        {
            const int M68K_IRQ_1 = 1;
            const int M68K_IRQ_2 = 2;
            const int M68K_IRQ_3 = 3;
            const int M68K_IRQ_4 = 4;
            const int M68K_IRQ_5 = 5;
            const int M68K_IRQ_6 = 6;
            const int M68K_IRQ_7 = 7;


            /* Special interrupt acknowledge values.
             * Use these as special returns from the interrupt acknowledge callback
             * (specified later in this header).
             */

            /* Causes an interrupt autovector (0x18 + interrupt level) to be taken.
             * This happens in a real 68000 if VPA or AVEC is asserted during an interrupt
             * acknowledge cycle instead of DTACK.
             */
            const int M68K_INT_ACK_AUTOVECTOR = -1;

            /* Causes the spurious interrupt vector (0x18) to be taken
             * This happens in a real 68000 if BERR is asserted during the interrupt
             * acknowledge cycle (i.e. no devices responded to the acknowledge).
             */
            const int M68K_INT_ACK_SPURIOUS = -2;
            const int M68K_CPU_MODE_68000 = 1;
            const int M68K_CPU_MODE_68010 = 2;
            const int M68K_CPU_MODE_68EC020 = 4;
            const int M68K_CPU_MODE_68020 = 8;
            /* CPU modes for deciding what to emulate */
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


            delegate int _int_ack_callback(int level);
            delegate void _bkpt_ack_callback(int data);
            delegate void _reset_instr_callback();
            delegate void _pc_changed_callback(int newpc);
            delegate void _set_fc_callback(int newfc);
            delegate void _instr_hook_callback();
            class m68k_cpu_core
            {
                public uint mode;        /* CPU Operation Mode: 68000, 68010, or 68020 */
                public uint[] dr = new uint[8];       /* Data Registers */
                public uint[] ar = new uint[8];       /* Address Registers */
                public uint ppc;		 /* Previous program counter */
                public uint pc;          /* Program Counter */
                public uint[] sp = new uint[4];       /* User, Interrupt, and Master Stack Pointers */
                public uint vbr;         /* Vector Base Register (68010+) */
                public uint sfc;         /* Source Function Code Register (m68010+) */
                public uint dfc;         /* Destination Function Code Register (m68010+) */
                public uint cacr;        /* Cache Control Register (m68020+) */
                public uint caar;        /* Cacge Address Register (m68020+) */
                public uint ir;          /* Instruction Register */
                public uint t1_flag;     /* Trace 1 */
                public uint t0_flag;     /* Trace 0 */
                public uint s_flag;      /* Supervisor */
                public uint m_flag;      /* Master/Interrupt state */
                public uint x_flag;      /* Extend */
                public uint n_flag;      /* Negative */
                public uint not_z_flag;  /* Zero, inverted for speedups */
                public uint v_flag;      /* Overflow */
                public uint c_flag;      /* Carry */
                public uint int_mask;    /* I0-I2 */
                public uint int_state;   /* Current interrupt state -- ASG: changed from ints_pending */
                public uint stopped;     /* Stopped state */
                public uint halted;      /* Halted state */
                public uint int_cycles;  /* ASG: extra cycles from generated interrupts */
                public uint pref_addr;   /* Last prefetch address */
                public uint pref_data;   /* Data in the prefetch queue */

                /* Callbacks to host */
                public _int_ack_callback int_ack_callback;  /* Interrupt Acknowledge */
                public _bkpt_ack_callback bkpt_ack_callback;     /* Breakpoint Acknowledge */
                public _reset_instr_callback reset_instr_callback;      /* Called when a RESET instruction is encountered */
                public _pc_changed_callback pc_changed_callback; /* Called when the PC changes by a large amount */
                public _set_fc_callback set_fc_callback;     /* Called when the CPU function code changes */
                public _instr_hook_callback instr_hook_callback;       /* Called every instruction cycle prior to execution */

            }
            m68k_cpu_core m68k_cpu;

            class m68k_cpu_context
            {
                public uint mode;         /* CPU Operation Mode (68000, 68010, or 68020) */
                public uint sr;           /* Status Register */
                public uint ppc;		   /* Previous program counter */
                public uint pc;           /* Program Counter */
                public uint[] d = new uint[8];         /* Data Registers */
                public uint[] a = new uint[8];         /* Address Registers */
                public uint usp;          /* User Stack Pointer */
                public uint isp;          /* Interrupt Stack Pointer */
                public uint msp;          /* Master Stack Pointer */
                public uint vbr;          /* Vector Base Register.  Used in 68010+ */
                public uint sfc;          /* Source Function Code.  Used in 68010+ */
                public uint dfc;          /* Destination Function Code.  Used in 68010+ */
                public uint stopped;      /* Stopped state: only interrupt can restart */
                public uint halted;       /* Halted state: only reset can restart */
                public uint int_state;	   /* Current interrupt line states -- ASG: changed from ints_pending */
                public uint int_cycles;   /* Extra cycles taken due to interrupts -- ASG: added */
                public uint pref_addr;    /* Last prefetch address */
                public uint pref_data;    /* Data in the prefetch queue */
                public _int_ack_callback int_ack_callback; /* Interrupt Acknowledge */
                public _bkpt_ack_callback bkpt_ack_callback;     /* Breakpoint Acknowledge */
                public _reset_instr_callback reset_instr_callback;      /* Called when a RESET instruction is encountered */
                public _pc_changed_callback pc_changed_callback; /* Called when the PC changes by a large amount */
                public _set_fc_callback set_fc_callback;     /* Called when the CPU function code changes */
                public _instr_hook_callback instr_hook_callback;       /* Called every instruction cycle prior to execution */
            }
            uint[] m68k_sr_implemented_bits =
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
            void m68ki_set_sm_flag(int s_value, int m_value)
            {
                /* ASG: Only do the rest if we're changing */
                s_value = (s_value != 0) ? 1 : 0;
                m_value = (m_value != 0 && (m68k_cpu.mode & (4 | 8)) != 0) ? 1 : 0 << 1;
                if (m68k_cpu.s_flag != s_value || m68k_cpu.m_flag != m_value)
                {
                    /* Backup the old stack pointer */
                    m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))] = m68k_cpu.ar[7];
                    /* Set the S and M flags */
                    m68k_cpu.s_flag = s_value != 0 ? 1u : 0u;
                    m68k_cpu.m_flag = (m_value != 0 && (m68k_cpu.mode & (4 | 8)) != 0) ? 1u : 0u << 1;
                    /* Set the new stack pointer */
                    m68k_cpu.ar[7] = m68k_cpu.sp[m68k_cpu.s_flag | (m68k_cpu.m_flag & (m68k_cpu.s_flag << 1))];
                }
            }
            void m68ki_set_sr_no_int(uint value)
            {
                /* Mask out the "unimplemented" bits */
                value &= m68k_sr_implemented_bits[m68k_cpu.mode];

                /* Now set the status register */
                m68k_cpu.t1_flag = ((value) & 0x00008000);
                m68k_cpu.t0_flag = ((value) & 0x00004000);
                m68k_cpu.int_mask = (value >> 8) & 7;
                m68k_cpu.x_flag = ((value) & 0x00000010);
                m68k_cpu.n_flag = ((value) & 0x00000008);

                m68k_cpu.not_z_flag = (value & 0x00000004) == 0 ? 1u : 0u;// (!((value) & 0x00000004) != 0) ? 1 : 0;
                m68k_cpu.v_flag = ((value) & 0x00000002);
                m68k_cpu.c_flag = ((value) & 0x00000001);
                m68ki_set_sm_flag((int)((value) & 0x00002000), (int)((value) & 0x00001000));
            }
            void m68ki_set_pc(uint address)
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
            void m68k_set_context(object src)
            {
                if (src != null)
                {
                    m68k_cpu_context cpu = (m68k_cpu_context)src;

                    m68k_cpu.mode = cpu.mode;
                    m68ki_set_sr_no_int(cpu.sr); /* This stays on top to prevent side-effects */
                    m68ki_set_pc(cpu.pc);
                    Array.Copy(cpu.d, m68k_cpu.dr, cpu.d.Length);
                    Array.Copy(cpu.a, m68k_cpu.ar, cpu.a.Length);
                    //memcpy(m68k_cpu.dr, cpu.d, sizeof(CPU_D));
                    //memcpy(m68k_cpu.ar, cpu.a, sizeof(CPU_D));
                    m68k_cpu.sp[0] = cpu.usp;
                    m68k_cpu.sp[1] = cpu.isp;
                    m68k_cpu.sp[3] = cpu.msp;
                    m68k_cpu.vbr = cpu.vbr;
                    m68k_cpu.sfc = cpu.sfc;
                    m68k_cpu.dfc = cpu.dfc;
                    m68k_cpu.stopped = cpu.stopped;
                    m68k_cpu.halted = cpu.halted;
                    m68k_cpu.int_state = cpu.int_state;	/* ASG: changed from CPU_INTS_PENDING */
                    m68k_cpu.int_cycles = cpu.int_cycles;	/* ASG */
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
            uint[] m68k_int_masks = {0xfe, 0xfc, 0xf8, 0xf0, 0xe0, 0xc0, 0x80, 0x80};
            void m68ki_check_interrupts()
            {
                uint pending_mask = 1u << (int)m68k_cpu.int_state;
                if ((pending_mask & m68k_int_masks[m68k_cpu.int_mask])!=0)
                    m68ki_service_interrupt(pending_mask);
            }
            uint m68ki_read_32(uint address)
{
    return (uint)Mame.cpu_readmem24_dword((int)((m68k_cpu.mode & 8) != 0 ? address : (address) & 0xffffff));
}
            void m68ki_service_interrupt(uint pending_mask) /* ASG: added parameter here */
{
   uint int_level = 7;
   uint vector;

   /* Start at level 7 and then go down */
   for(;(pending_mask & (1u<<(int)int_level))==0;int_level--) /* ASG: changed to use parameter instead of CPU_INTS_PENDING */
      ;

   /* Get the exception vector */
   switch(vector =unchecked((uint) -1))
   {
      case 0x00: case 0x01:
      /* vectors 0 and 1 are ignored since they are for reset only */
         return;
      case 0x02: case 0x03: case 0x04: case 0x05: case 0x06: case 0x07:
      case 0x08: case 0x09: case 0x0a: case 0x0b: case 0x0c: case 0x0d: case 0x0e: case 0x0f:
      case 0x10: case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17:
      case 0x18: case 0x19: case 0x1a: case 0x1b: case 0x1c: case 0x1d: case 0x1e: case 0x1f:
      case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25: case 0x26: case 0x27:
      case 0x28: case 0x29: case 0x2a: case 0x2b: case 0x2c: case 0x2d: case 0x2e: case 0x2f:
      case 0x30: case 0x31: case 0x32: case 0x33: case 0x34: case 0x35: case 0x36: case 0x37:
      case 0x38: case 0x39: case 0x3a: case 0x3b: case 0x3c: case 0x3d: case 0x3e: case 0x3f:
      case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x46: case 0x47:
      case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4e: case 0x4f:
      case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57:
      case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5e: case 0x5f:
      case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67:
      case 0x68: case 0x69: case 0x6a: case 0x6b: case 0x6c: case 0x6d: case 0x6e: case 0x6f:
      case 0x70: case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x76: case 0x77:
      case 0x78: case 0x79: case 0x7a: case 0x7b: case 0x7c: case 0x7d: case 0x7e: case 0x7f:
      case 0x80: case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87:
      case 0x88: case 0x89: case 0x8a: case 0x8b: case 0x8c: case 0x8d: case 0x8e: case 0x8f:
      case 0x90: case 0x91: case 0x92: case 0x93: case 0x94: case 0x95: case 0x96: case 0x97:
      case 0x98: case 0x99: case 0x9a: case 0x9b: case 0x9c: case 0x9d: case 0x9e: case 0x9f:
      case 0xa0: case 0xa1: case 0xa2: case 0xa3: case 0xa4: case 0xa5: case 0xa6: case 0xa7:
      case 0xa8: case 0xa9: case 0xaa: case 0xab: case 0xac: case 0xad: case 0xae: case 0xaf:
      case 0xb0: case 0xb1: case 0xb2: case 0xb3: case 0xb4: case 0xb5: case 0xb6: case 0xb7:
      case 0xb8: case 0xb9: case 0xba: case 0xbb: case 0xbc: case 0xbd: case 0xbe: case 0xbf:
      case 0xc0: case 0xc1: case 0xc2: case 0xc3: case 0xc4: case 0xc5: case 0xc6: case 0xc7:
      case 0xc8: case 0xc9: case 0xca: case 0xcb: case 0xcc: case 0xcd: case 0xce: case 0xcf:
      case 0xd0: case 0xd1: case 0xd2: case 0xd3: case 0xd4: case 0xd5: case 0xd6: case 0xd7:
      case 0xd8: case 0xd9: case 0xda: case 0xdb: case 0xdc: case 0xdd: case 0xde: case 0xdf:
      case 0xe0: case 0xe1: case 0xe2: case 0xe3: case 0xe4: case 0xe5: case 0xe6: case 0xe7:
      case 0xe8: case 0xe9: case 0xea: case 0xeb: case 0xec: case 0xed: case 0xee: case 0xef:
      case 0xf0: case 0xf1: case 0xf2: case 0xf3: case 0xf4: case 0xf5: case 0xf6: case 0xf7:
      case 0xf8: case 0xf9: case 0xfa: case 0xfb: case 0xfc: case 0xfd: case 0xfe: case 0xff:
         /* The external peripheral has provided the interrupt vector to take */
         break;
      case unchecked((uint)-1):
         /* Use the autovectors.  This is the most commonly used implementation */
         vector = 24 +int_level;
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
   if(m68ki_read_32(vector<<2) == 0)
      vector = 15;

   /* Generate an interupt */
   m68ki_interrupt(vector);

   /* Set the interrupt mask to the level of the one being serviced */
   m68k_cpu.int_mask = int_level;
}
            uint[] m68k_exception_cycle_table =
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
            void m68ki_set_s_flag(int value)
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
              void m68ki_write_8(uint address, uint value)
{
   Mame.cpu_writemem24( (int)(((m68k_cpu.mode & 8)!=0) ? address : (address)&0xffffff), (int)value);
}
  void m68ki_write_16(uint address, uint value)
{
   Mame.cpu_writemem24_word( (int)(((m68k_cpu.mode & 8)!=0) ? address : (address)&0xffffff), (int) value);
}
  void m68ki_write_32(uint address, uint value)
{
   Mame.cpu_writemem24_dword( (int)(((m68k_cpu.mode & 8)!=0) ? address : (address)&0xffffff), (int)value);
} 

            void m68ki_interrupt(uint vector)
{
    throw new Exception();
}
        }
    }
}

