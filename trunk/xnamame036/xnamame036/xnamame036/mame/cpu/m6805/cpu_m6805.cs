using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        class cpu_m6805 : cpu_interface
        {
            public const int M6805_INT_NONE = 0;
            public const int M6805_INT_IRQ = 1;
            static int[] m6805_ICount = new int[1];
            public cpu_m6805()
            {
                cpu_num = CPU_M6805;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6805_INT_NONE;
                irq_int = M6805_INT_IRQ;
                nmi_int = -1;
                address_shift = 0;
                address_bits = 11;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 3;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6805_ICount;
            }
            static PAIR ea;

            static byte[] flags8i = /* increment */
{
0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04
};
            static byte[] flags8d = /* decrement */
{
0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,
0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04,0x04
};
            static byte[] cycles1 =
{
      /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
  /*0*/ 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,
  /*1*/ 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
  /*2*/ 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
  /*3*/ 6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 6, 0, 6, 6, 0,
  /*4*/ 4, 0, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4,
  /*5*/ 4, 0, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4,
  /*6*/ 7, 0, 0, 7, 7, 0, 7, 7, 7, 7, 7, 0, 7, 7, 0, 7,
  /*7*/ 6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 0, 6,
  /*8*/ 9, 6, 0,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
  /*9*/ 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0, 2,
  /*A*/ 2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 8, 2, 0,
  /*B*/ 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 3, 7, 4, 5,
  /*C*/ 5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 4, 8, 5, 6,
  /*D*/ 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 5, 9, 6, 7,
  /*E*/ 5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 4, 8, 5, 6,
  /*F*/ 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 3, 7, 4, 5
};
            public override void burn(int cycles)
            {
                //none
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6805";
                    case CPU_INFO_FAMILY: return "Motorola 6805";
                    case CPU_INFO_VERSION: return "1.0";
                    case CPU_INFO_FILE: return "cpu_m6805.cs";
                    case CPU_INFO_CREDITS: return "The MAME team.";
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
               //none
            }

            static void rd_s_handler_b(ref byte b)
            {
                b = ((byte)cpu_readmem16((int)((m6805.s.wl) & m6805.amask)));
                if (++m6805.s.wl > m6805.sp_mask) m6805.s.wl = (ushort)m6805.sp_low;
            }

            static void rd_s_handler_w(ref PAIR p)
            {
                p.d = 0;
                p.bh = ((byte)cpu_readmem16((int)((m6805.s.wl) & m6805.amask)));
                if (++m6805.s.wl > m6805.sp_mask) m6805.s.wl = (ushort)m6805.sp_low;
                p.bl = ((byte)cpu_readmem16((int)((m6805.s.wl) & m6805.amask)));
                if (++m6805.s.wl > m6805.sp_mask) m6805.s.wl = (ushort)m6805.sp_low;
            }

            static void wr_s_handler_b(byte b)
            {
                if (--m6805.s.wl < m6805.sp_low) m6805.s.wl = (ushort)m6805.sp_mask;
                cpu_writemem16((int)((m6805.s.wl) & m6805.amask), b);
            }

            static void wr_s_handler_w(PAIR p)
            {
                if (--m6805.s.wl < m6805.sp_low) m6805.s.wl = (ushort)m6805.sp_mask;
                cpu_writemem16((int)((m6805.s.wl) & m6805.amask), p.bl);
                if (--m6805.s.wl < m6805.sp_low) m6805.s.wl = (ushort)m6805.sp_mask;
                cpu_writemem16((int)((m6805.s.wl) & m6805.amask), p.bh);
            }

            public static void RM16(uint Addr, ref PAIR p)
            {
                p.d = 0;
                p.bh = ((byte)cpu_readmem16((int)((Addr) & m6805.amask)));
                if (++Addr > m6805.amask) Addr = 0;
                p.bl = ((byte)cpu_readmem16((int)((Addr) & m6805.amask)));
            }

            static void WM16(uint Addr, PAIR p)
            {
                cpu_writemem16((int)((Addr) & m6805.amask), p.bh);
                if (++Addr > m6805.amask) Addr = 0;
                cpu_writemem16((int)((Addr) & m6805.amask), p.bl);
            }

            static void Interrupt()
            {
                /* the 6805 latches interrupt requests internally, so we don't clear */
                /* pending_interrupts until the interrupt is taken, no matter what the */
                /* external IRQ pin does. */

                if ((m6805.pending_interrupts & (1 << 0x08)) != 0)
                {
                    wr_s_handler_w(m6805.pc);
                    wr_s_handler_b(m6805.x);
                    wr_s_handler_b(m6805.a);
                    wr_s_handler_b(m6805.cc);
                    m6805.cc |= 0x08;
                    /* no vectors supported, just do the callback to clear irq_state if needed */
                    if (m6805.irq_callback != null)
                        m6805.irq_callback(0);

                    RM16(0x1ffc, ref m6805.pc);
                    m6805.pending_interrupts &= unchecked((byte)(~(1 << 0x08)));

                    m6805_ICount[0] -= 11;

                }
                else if ((m6805.pending_interrupts & (1 | 0x1ff)) != 0 && (m6805.cc & 0x08) == 0)
                {
                    /* standard IRQ */
                    if (m6805.subtype != SUBTYPE_HD63705) m6805.pc.wl |= 0xf800;
                    wr_s_handler_w(m6805.pc);
                    wr_s_handler_b(m6805.x);
                    wr_s_handler_b(m6805.a);
                    wr_s_handler_b(m6805.cc);
                    m6805.cc |= 0x08;
                    /* no vectors supported, just do the callback to clear irq_state if needed */
                    if (m6805.irq_callback != null)
                        m6805.irq_callback(0);


                    if (m6805.subtype == SUBTYPE_HD63705)
                    {
                        /* Need to add emulation of other interrupt sources here KW-2/4/99 */
                        /* This is just a quick patch for Namco System 2 operation         */

                        if ((m6805.pending_interrupts & (1 << 0x00)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x00));
                            RM16(0x1ff8, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x01)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x01));
                            RM16(0x1fec, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x07)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x07));
                            RM16(0x1fea, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x02)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x02));
                            RM16(0x1ff6, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x03)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x03));
                            RM16(0x1ff4, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x04)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x04));
                            RM16(0x1ff2, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x05)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x05));
                            RM16(0x1ff0, ref m6805.pc);
                        }
                        else if ((m6805.pending_interrupts & (1 << 0x06)) != 0)
                        {
                            m6805.pending_interrupts &= unchecked((byte)~(1 << 0x06));
                            RM16(0x1fee, ref m6805.pc);
                        }
                    }
                    else
                    {
                        m6805.pending_interrupts &= unchecked((byte)~1);
                        RM16(m6805.amask - 5, ref m6805.pc);
                    }
                    m6805_ICount[0] -= 11;
                }
            }
            public override int execute(int cycles)
            {
                byte ireg;
                m6805_ICount[0] = cycles;

                do
                {
                    if (m6805.pending_interrupts != 0)
                        Interrupt();

                    ireg = ((byte)cpu_readop(m6805.pc.wl++));

                    switch (ireg)
                    {
                        case 0x00: brset(0x01); break;
                        case 0x01: brclr(0x01); break;
                        case 0x02: brset(0x02); break;
                        case 0x03: brclr(0x02); break;
                        case 0x04: brset(0x04); break;
                        case 0x05: brclr(0x04); break;
                        case 0x06: brset(0x08); break;
                        case 0x07: brclr(0x08); break;
                        case 0x08: brset(0x10); break;
                        case 0x09: brclr(0x10); break;
                        case 0x0A: brset(0x20); break;
                        case 0x0B: brclr(0x20); break;
                        case 0x0C: brset(0x40); break;
                        case 0x0D: brclr(0x40); break;
                        case 0x0E: brset(0x80); break;
                        case 0x0F: brclr(0x80); break;
                        case 0x10: bset(0x01); break;
                        case 0x11: bclr(0x01); break;
                        case 0x12: bset(0x02); break;
                        case 0x13: bclr(0x02); break;
                        case 0x14: bset(0x04); break;
                        case 0x15: bclr(0x04); break;
                        case 0x16: bset(0x08); break;
                        case 0x17: bclr(0x08); break;
                        case 0x18: bset(0x10); break;
                        case 0x19: bclr(0x10); break;
                        case 0x1a: bset(0x20); break;
                        case 0x1b: bclr(0x20); break;
                        case 0x1c: bset(0x40); break;
                        case 0x1d: bclr(0x40); break;
                        case 0x1e: bset(0x80); break;
                        case 0x1f: bclr(0x80); break;
                        case 0x20: bra(); break;
                        case 0x21: brn(); break;
                        case 0x22: bhi(); break;
                        case 0x23: bls(); break;
                        case 0x24: bcc(); break;
                        case 0x25: bcs(); break;
                        case 0x26: bne(); break;
                        case 0x27: beq(); break;
                        case 0x28: bhcc(); break;
                        case 0x29: bhcs(); break;
                        case 0x2a: bpl(); break;
                        case 0x2b: bmi(); break;
                        case 0x2c: bmc(); break;
                        case 0x2d: bms(); break;
                        case 0x2e: bil(); break;
                        case 0x2f: bih(); break;
                        case 0x30: neg_di(); break;
                        case 0x31: illegal(); break;
                        case 0x32: illegal(); break;
                        case 0x33: com_di(); break;
                        case 0x34: lsr_di(); break;
                        case 0x35: illegal(); break;
                        case 0x36: ror_di(); break;
                        case 0x37: asr_di(); break;
                        case 0x38: lsl_di(); break;
                        case 0x39: rol_di(); break;
                        case 0x3a: dec_di(); break;
                        case 0x3b: illegal(); break;
                        case 0x3c: inc_di(); break;
                        case 0x3d: tst_di(); break;
                        case 0x3e: illegal(); break;
                        case 0x3f: clr_di(); break;
                        case 0x40: nega(); break;
                        case 0x41: illegal(); break;
                        case 0x42: illegal(); break;
                        case 0x43: coma(); break;
                        case 0x44: lsra(); break;
                        case 0x45: illegal(); break;
                        case 0x46: rora(); break;
                        case 0x47: asra(); break;
                        case 0x48: lsla(); break;
                        case 0x49: rola(); break;
                        case 0x4a: deca(); break;
                        case 0x4b: illegal(); break;
                        case 0x4c: inca(); break;
                        case 0x4d: tsta(); break;
                        case 0x4e: illegal(); break;
                        case 0x4f: clra(); break;
                        case 0x50: negx(); break;
                        case 0x51: illegal(); break;
                        case 0x52: illegal(); break;
                        case 0x53: comx(); break;
                        case 0x54: lsrx(); break;
                        case 0x55: illegal(); break;
                        case 0x56: rorx(); break;
                        case 0x57: asrx(); break;
                        case 0x58: aslx(); break;
                        case 0x59: rolx(); break;
                        case 0x5a: decx(); break;
                        case 0x5b: illegal(); break;
                        case 0x5c: incx(); break;
                        case 0x5d: tstx(); break;
                        case 0x5e: illegal(); break;
                        case 0x5f: clrx(); break;
                        case 0x60: neg_ix1(); break;
                        case 0x61: illegal(); break;
                        case 0x62: illegal(); break;
                        case 0x63: com_ix1(); break;
                        case 0x64: lsr_ix1(); break;
                        case 0x65: illegal(); break;
                        case 0x66: ror_ix1(); break;
                        case 0x67: asr_ix1(); break;
                        case 0x68: lsl_ix1(); break;
                        case 0x69: rol_ix1(); break;
                        case 0x6a: dec_ix1(); break;
                        case 0x6b: illegal(); break;
                        case 0x6c: inc_ix1(); break;
                        case 0x6d: tst_ix1(); break;
                        case 0x6e: illegal(); break;
                        case 0x6f: clr_ix1(); break;
                        case 0x70: neg_ix(); break;
                        case 0x71: illegal(); break;
                        case 0x72: illegal(); break;
                        case 0x73: com_ix(); break;
                        case 0x74: lsr_ix(); break;
                        case 0x75: illegal(); break;
                        case 0x76: ror_ix(); break;
                        case 0x77: asr_ix(); break;
                        case 0x78: lsl_ix(); break;
                        case 0x79: rol_ix(); break;
                        case 0x7a: dec_ix(); break;
                        case 0x7b: illegal(); break;
                        case 0x7c: inc_ix(); break;
                        case 0x7d: tst_ix(); break;
                        case 0x7e: illegal(); break;
                        case 0x7f: clr_ix(); break;
                        case 0x80: rti(); break;
                        case 0x81: rts(); break;
                        case 0x82: illegal(); break;
                        case 0x83: swi(); break;
                        case 0x84: illegal(); break;
                        case 0x85: illegal(); break;
                        case 0x86: illegal(); break;
                        case 0x87: illegal(); break;
                        case 0x88: illegal(); break;
                        case 0x89: illegal(); break;
                        case 0x8a: illegal(); break;
                        case 0x8b: illegal(); break;
                        case 0x8c: illegal(); break;
                        case 0x8d: illegal(); break;
                        case 0x8e: illegal(); break;
                        case 0x8f: illegal(); break;
                        case 0x90: illegal(); break;
                        case 0x91: illegal(); break;
                        case 0x92: illegal(); break;
                        case 0x93: illegal(); break;
                        case 0x94: illegal(); break;
                        case 0x95: illegal(); break;
                        case 0x96: illegal(); break;
                        case 0x97: tax(); break;
                        case 0x98: m6805.cc &= unchecked((byte)~0x01); break;
                        case 0x99: m6805.cc |= 0x01; break;



                        case 0x9a: m6805.cc &= unchecked((byte)~0x08); break;

                        case 0x9b: m6805.cc |= 0x08; break;
                        case 0x9c: rsp(); break;
                        case 0x9d: nop(); break;
                        case 0x9e: illegal(); break;
                        case 0x9f: txa(); break;
                        case 0xa0: suba_im(); break;
                        case 0xa1: cmpa_im(); break;
                        case 0xa2: sbca_im(); break;
                        case 0xa3: cpx_im(); break;
                        case 0xa4: anda_im(); break;
                        case 0xa5: bita_im(); break;
                        case 0xa6: lda_im(); break;
                        case 0xa7: illegal(); break;
                        case 0xa8: eora_im(); break;
                        case 0xa9: adca_im(); break;
                        case 0xaa: ora_im(); break;
                        case 0xab: adda_im(); break;
                        case 0xac: illegal(); break;
                        case 0xad: bsr(); break;
                        case 0xae: ldx_im(); break;
                        case 0xaf: illegal(); break;
                        case 0xb0: suba_di(); break;
                        case 0xb1: cmpa_di(); break;
                        case 0xb2: sbca_di(); break;
                        case 0xb3: cpx_di(); break;
                        case 0xb4: anda_di(); break;
                        case 0xb5: bita_di(); break;
                        case 0xb6: lda_di(); break;
                        case 0xb7: sta_di(); break;
                        case 0xb8: eora_di(); break;
                        case 0xb9: adca_di(); break;
                        case 0xba: ora_di(); break;
                        case 0xbb: adda_di(); break;
                        case 0xbc: jmp_di(); break;
                        case 0xbd: jsr_di(); break;
                        case 0xbe: ldx_di(); break;
                        case 0xbf: stx_di(); break;
                        case 0xc0: suba_ex(); break;
                        case 0xc1: cmpa_ex(); break;
                        case 0xc2: sbca_ex(); break;
                        case 0xc3: cpx_ex(); break;
                        case 0xc4: anda_ex(); break;
                        case 0xc5: bita_ex(); break;
                        case 0xc6: lda_ex(); break;
                        case 0xc7: sta_ex(); break;
                        case 0xc8: eora_ex(); break;
                        case 0xc9: adca_ex(); break;
                        case 0xca: ora_ex(); break;
                        case 0xcb: adda_ex(); break;
                        case 0xcc: jmp_ex(); break;
                        case 0xcd: jsr_ex(); break;
                        case 0xce: ldx_ex(); break;
                        case 0xcf: stx_ex(); break;
                        case 0xd0: suba_ix2(); break;
                        case 0xd1: cmpa_ix2(); break;
                        case 0xd2: sbca_ix2(); break;
                        case 0xd3: cpx_ix2(); break;
                        case 0xd4: anda_ix2(); break;
                        case 0xd5: bita_ix2(); break;
                        case 0xd6: lda_ix2(); break;
                        case 0xd7: sta_ix2(); break;
                        case 0xd8: eora_ix2(); break;
                        case 0xd9: adca_ix2(); break;
                        case 0xda: ora_ix2(); break;
                        case 0xdb: adda_ix2(); break;
                        case 0xdc: jmp_ix2(); break;
                        case 0xdd: jsr_ix2(); break;
                        case 0xde: ldx_ix2(); break;
                        case 0xdf: stx_ix2(); break;
                        case 0xe0: suba_ix1(); break;
                        case 0xe1: cmpa_ix1(); break;
                        case 0xe2: sbca_ix1(); break;
                        case 0xe3: cpx_ix1(); break;
                        case 0xe4: anda_ix1(); break;
                        case 0xe5: bita_ix1(); break;
                        case 0xe6: lda_ix1(); break;
                        case 0xe7: sta_ix1(); break;
                        case 0xe8: eora_ix1(); break;
                        case 0xe9: adca_ix1(); break;
                        case 0xea: ora_ix1(); break;
                        case 0xeb: adda_ix1(); break;
                        case 0xec: jmp_ix1(); break;
                        case 0xed: jsr_ix1(); break;
                        case 0xee: ldx_ix1(); break;
                        case 0xef: stx_ix1(); break;
                        case 0xf0: suba_ix(); break;
                        case 0xf1: cmpa_ix(); break;
                        case 0xf2: sbca_ix(); break;
                        case 0xf3: cpx_ix(); break;
                        case 0xf4: anda_ix(); break;
                        case 0xf5: bita_ix(); break;
                        case 0xf6: lda_ix(); break;
                        case 0xf7: sta_ix(); break;
                        case 0xf8: eora_ix(); break;
                        case 0xf9: adca_ix(); break;
                        case 0xfa: ora_ix(); break;
                        case 0xfb: adda_ix(); break;
                        case 0xfc: jmp_ix(); break;
                        case 0xfd: jsr_ix(); break;
                        case 0xfe: ldx_ix(); break;
                        case 0xff: stx_ix(); break;
                    }
                    m6805_ICount[0] -= cycles1[ireg];
                } while (m6805_ICount[0] > 0);

                return cycles - m6805_ICount[0];
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new m6805_Regs();
            }
            public override uint get_pc()
            {
                return m6805.pc.wl & m6805.amask;
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
                /* Force CPU sub-type and relevant masks */
                m6805.subtype = SUBTYPE_M6805;
                m6805.amask = 0x7ff;
                m6805.sp_mask = 0x07f;
                m6805.sp_low = 0x060;
                /* Initial stack pointer */
                m6805.s.wl = (ushort)m6805.sp_mask;
                /* IRQ disabled */
                m6805.cc |= 0x08;
                RM16(m6805.amask - 1, ref m6805.pc);
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                m6805.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                if (m6805.irq_state[0] == state) return;

                m6805.irq_state[0] = state;
                if (state != CLEAR_LINE)
                    m6805.pending_interrupts |= M6805_INT_IRQ;
            }
            public override void set_nmi_line(int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_op_base(int pc)
            {
                cpu_setOPbase16(pc, 0);
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

            public const int SUBTYPE_M6805 = 0;
            public const int SUBTYPE_M68705 = 1;
            public const int SUBTYPE_HD63705 = 2;

            public class m6805_Regs
            {
                public int subtype;		/* Which sub-type is being emulated */
                public uint amask;			/* Address bus width */
                public uint sp_mask;		/* Stack pointer address mask */
                public uint sp_low; 		/* Stack pointer low water mark (or floor) */
                public PAIR pc;             /* Program counter */
                public PAIR s;				/* Stack pointer */
                public byte a;				/* Accumulator */
                public byte x;				/* Index register */
                public byte cc; 			/* Condition codes */

                public byte pending_interrupts; /* MB */
                public irqcallback irq_callback;
                public int[] irq_state = new int[8];		/* KW Additional lines for HD73705 */
                public int nmi_state;
            }
            public static m6805_Regs m6805 = new m6805_Regs();



            static void illegal()
            {
                Mame.printf("M6805: illegal opcode\n");
            }





            /* $00/$02/$04/$06/$08/$0A/$0C/$0E BRSET direct,relative ---- */
            static void brset(byte bit)
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };r = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                if ((r & bit) != 0)
                    m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                else
                    if (t == 0xfd)
                    {
                        /* speed up busy loops */
                        if (m6805_ICount[0] > 0)
                            m6805_ICount[0] = 0;
                    }
            }

            /* $01/$03/$05/$07/$09/$0B/$0D/$0F BRCLR direct,relative ---- */
            static void brclr(byte bit)
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };r = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                if ((r & bit) == 0)
                    m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                else
                {
                    /* speed up busy loops */
                    if (m6805_ICount[0] > 0)
                        m6805_ICount[0] = 0;
                }
            }






            /* $10/$12/$14/$16/$18/$1A/$1C/$1E BSET direct ---- */
            static void bset(byte bit)
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)(t | bit);
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $11/$13/$15/$17/$19/$1B/$1D/$1F BCLR direct ---- */
            static void bclr(byte bit)
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)(t & (~bit));
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }






            /* $20 BRA relative ---- */
            static void bra()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                if (t == 0xfe)
                {
                    /* speed up busy loops */
                    if (m6805_ICount[0] > 0)
                        m6805_ICount[0] = 0;
                }
            }

            /* $21 BRN relative ---- */
            static void brn()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
            }

            /* $22 BHI relative ---- */
            static void bhi()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & (0x01 | 0x02)) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $23 BLS relative ---- */
            static void bls()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & (0x01 | 0x02)) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $24 BCC relative ---- */
            static void bcc()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x01) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $25 BCS relative ---- */
            static void bcs()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x01) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $26 BNE relative ---- */
            static void bne()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x02) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $27 BEQ relative ---- */
            static void beq()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x02) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $28 BHCC relative ---- */
            static void bhcc()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x10) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $29 BHCS relative ---- */
            static void bhcs()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x10) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $2a BPL relative ---- */
            static void bpl()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x04) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $2b BMI relative ---- */
            static void bmi()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x04) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $2c BMC relative ---- */
            static void bmc()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x08) == 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $2d BMS relative ---- */
            static void bms()
            {
                {
                    byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if ((m6805.cc & 0x08) != 0)
                    { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                };
            }

            /* $2e BIL relative ---- */
            static void bil()
            {
                if (m6805.subtype == SUBTYPE_HD63705)
                {
                    {
                        byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if (m6805.nmi_state != CLEAR_LINE)
                        { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                    };
                }
                else
                {
                    {
                        byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if (m6805.irq_state[0] != CLEAR_LINE)
                        { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                    };
                }
            }

            /* $2f BIH relative ---- */
            static void bih()
            {
                if (m6805.subtype == SUBTYPE_HD63705)
                {
                    {
                        byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if (m6805.nmi_state == CLEAR_LINE)
                        { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                    };
                }
                else
                {
                    {
                        byte t; { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; if (m6805.irq_state[0] == CLEAR_LINE)
                        { m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t)); }
                    };
                }
            }






            /* $30 NEG direct -*** */
            static void neg_di()
            {
                byte t;
                ushort r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); }; r = (ushort)-t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5);
                    if (((byte)r) == 0) m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $31 ILLEGAL */

            /* $32 ILLEGAL */

            /* $33 COM direct -**1 */
            static void com_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); }; t = (byte)~t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; }; m6805.cc |= 0x01;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $34 LSR direct -0** */
            static void lsr_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                t >>= 1;
                if (((byte)t) == 0) m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $35 ILLEGAL */

            /* $36 ROR direct -*** */
            static void ror_di()
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)((m6805.cc & 0x01) << 7);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1);
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $37 ASR direct ?*** */
            static void asr_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01)); m6805.cc |= (byte)(t & 0x01);
                t >>= 1; t |= (byte)((t & 0x40) << 1);
                { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $38 LSL direct ?*** */
            static void lsl_di()
            {
                byte t;
                ushort r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $39 ROL direct -*** */
            static void rol_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.cc & 0x01);
                r |= (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $3a DEC direct -**- */
            static void dec_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                --t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $3b ILLEGAL */

            /* $3c INC direct -**- */
            static void inc_di()
            {
                byte t;
                {
                    ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                ++t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $3d TST direct -**- */
            static void tst_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
            }

            /* $3e ILLEGAL */

            /* $3f CLR direct -0100 */
            static void clr_di()
            {
                ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01)); m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), 0);
            }






            /* $40 NEGA inherent ?*** */
            static void nega()
            {
                ushort r;
                r = (ushort)-m6805.a;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5);
                    if (((byte)r) == 0) m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                m6805.a = (byte)r;
            }

            /* $41 ILLEGAL */

            /* $42 ILLEGAL */

            /* $43 COMA inherent -**1 */
            static void coma()
            {
                m6805.a = (byte)~m6805.a;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                m6805.cc |= 0x01;
            }

            /* $44 LSRA inherent -0** */
            static void lsra()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.a & 0x01);
                m6805.a >>= 1;
                if (((byte)m6805.a) == 0) m6805.cc |= 0x02;
            }

            /* $45 ILLEGAL */

            /* $46 RORA inherent -*** */
            static void rora()
            {
                byte r;
                r = (byte)((m6805.cc & 0x01) << 7);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.a & 0x01);
                r |= (byte)(m6805.a >> 1);
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
                m6805.a = r;
            }

            /* $47 ASRA inherent ?*** */
            static void asra()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.a & 0x01);
                m6805.a = (byte)((m6805.a & 0x80) | (m6805.a >> 1));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $48 LSLA inherent ?*** */
            static void lsla()
            {
                ushort r;
                r = (ushort)(m6805.a << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $49 ROLA inherent -*** */
            static void rola()
            {
                ushort t, r;
                t = m6805.a;
                r = (ushort)(m6805.cc & 0x01);
                r |= (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $4a DECA inherent -**- */
            static void deca()
            {
                --m6805.a;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= flags8d[(m6805.a) & 0xff]; };
            }

            /* $4b ILLEGAL */

            /* $4c INCA inherent -**- */
            static void inca()
            {
                ++m6805.a;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= flags8i[(m6805.a) & 0xff]; };
            }

            /* $4d TSTA inherent -**- */
            static void tsta()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $4e ILLEGAL */

            /* $4f CLRA inherent -010 */
            static void clra()
            {
                m6805.a = 0;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= 0x02;
            }






            /* $50 NEGX inherent ?*** */
            static void negx()
            {
                ushort r;
                r = (ushort)-m6805.x;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.x = (byte)r;
            }

            /* $51 ILLEGAL */

            /* $52 ILLEGAL */

            /* $53 COMX inherent -**1 */
            static void comx()
            {
                m6805.x = (byte)~m6805.x;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                m6805.cc |= 0x01;
            }

            /* $54 LSRX inherent -0** */
            static void lsrx()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.x & 0x01);
                m6805.x >>= 1;
                if (((byte)m6805.x) == 0) m6805.cc |= 0x02;
            }

            /* $55 ILLEGAL */

            /* $56 RORX inherent -*** */
            static void rorx()
            {
                byte r;
                r = (byte)((m6805.cc & 0x01) << 7);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.x & 0x01);
                r |= (byte)(m6805.x >> 1);
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
                m6805.x = r;
            }

            /* $57 ASRX inherent ?*** */
            static void asrx()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(m6805.x & 0x01);
                m6805.x = (byte)((m6805.x & 0x80) | (m6805.x >> 1));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $58 ASLX inherent ?*** */
            static void aslx()
            {
                ushort r;
                r = (ushort)(m6805.x << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.x = (byte)r;
            }

            /* $59 ROLX inherent -*** */
            static void rolx()
            {
                ushort t, r;
                t = m6805.x;
                r = (ushort)(m6805.cc & 0x01);
                r |= (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.x = (byte)r;
            }

            /* $5a DECX inherent -**- */
            static void decx()
            {
                --m6805.x;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= flags8d[(m6805.x) & 0xff]; };
            }

            /* $5b ILLEGAL */

            /* $5c INCX inherent -**- */
            static void incx()
            {
                ++m6805.x;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= flags8i[(m6805.x) & 0xff]; };
            }

            /* $5d TSTX inherent -**- */
            static void tstx()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $5e ILLEGAL */

            /* $5f CLRX inherent -010 */
            static void clrx()
            {
                m6805.x = 0;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= 0x02;
            }






            /* $60 NEG indexed, 1 byte offset -*** */
            static void neg_ix1()
            {
                byte t;
                ushort r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                }; r = (ushort)-t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $61 ILLEGAL */

            /* $62 ILLEGAL */

            /* $63 COM indexed, 1 byte offset -**1 */
            static void com_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                }; t = (byte)~t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; }; m6805.cc |= 0x01;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $64 LSR indexed, 1 byte offset -0** */
            static void lsr_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                t >>= 1;
                if (((byte)t) == 0) m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $65 ILLEGAL */

            /* $66 ROR indexed, 1 byte offset -*** */
            static void ror_ix1()
            {
                byte t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (byte)((m6805.cc & 0x01) << 7);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1);
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $67 ASR indexed, 1 byte offset ?*** */
            static void asr_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01)); m6805.cc |= (byte)(t & 0x01);
                t >>= 1; t |= (byte)((t & 0x40) << 1);
                { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $68 LSL indexed, 1 byte offset ?*** */
            static void lsl_ix1()
            {
                byte t;
                ushort r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $69 ROL indexed, 1 byte offset -*** */
            static void rol_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.cc & 0x01);
                r |= (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $6a DEC indexed, 1 byte offset -**- */
            static void dec_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                --t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $6b ILLEGAL */

            /* $6c INC indexed, 1 byte offset -**- */
            static void inc_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                ++t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $6d TST indexed, 1 byte offset -**- */
            static void tst_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
            }

            /* $6e ILLEGAL */

            /* $6f CLR indexed, 1 byte offset -0100 */
            static void clr_ix1()
            {
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01)); m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), 0);
            }






            /* $70 NEG indexed -*** */
            static void neg_ix()
            {
                byte t;
                ushort r;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); }; r = (ushort)-t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5);
                    if (((byte)r) == 0) m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $71 ILLEGAL */

            /* $72 ILLEGAL */

            /* $73 COM indexed -**1 */
            static void com_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); }; t = (byte)~t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; }; m6805.cc |= 0x01;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $74 LSR indexed -0** */
            static void lsr_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                t >>= 1;
                if (((byte)t) == 0) m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $75 ILLEGAL */

            /* $76 ROR indexed -*** */
            static void ror_ix()
            {
                byte t, r;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)((m6805.cc & 0x01) << 7);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1);
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $77 ASR indexed ?*** */
            static void asr_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                m6805.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $78 LSL indexed ?*** */
            static void lsl_ix()
            {
                byte t;
                ushort r;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); }; r = (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5);
                    if (((byte)r) == 0) m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $79 ROL indexed -*** */
            static void rol_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.cc & 0x01);
                r |= (ushort)(t << 1);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)((ea.d) & m6805.amask), r);
            }

            /* $7a DEC indexed -**- */
            static void dec_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                --t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $7b ILLEGAL */

            /* $7c INC indexed -**- */
            static void inc_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                ++t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)((ea.d) & m6805.amask), t);
            }

            /* $7d TST indexed -**- */
            static void tst_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02)); { m6805.cc |= (byte)((t & 0x80) >> 5); if (t == 0)m6805.cc |= 0x02; };
            }

            /* $7e ILLEGAL */

            /* $7f CLR indexed -0100 */
            static void clr_ix()
            {
                ea.wl = m6805.x;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01)); m6805.cc |= 0x02;
                cpu_writemem16((int)((ea.d) & m6805.amask), 0);
            }






            /* $80 RTI inherent #### */
            static void rti()
            {
                rd_s_handler_b(ref m6805.cc);
                rd_s_handler_b(ref m6805.a);
                rd_s_handler_b(ref m6805.x);
                rd_s_handler_w(ref m6805.pc);
                m6805.pc.wl &= (ushort)m6805.amask;




            }

            /* $81 RTS inherent ---- */
            static void rts()
            {
                rd_s_handler_w(ref m6805.pc);
                m6805.pc.wl &= (ushort)m6805.amask;
            }

            /* $82 ILLEGAL */

            /* $83 SWI absolute indirect ---- */
            static void swi()
            {
                wr_s_handler_w(m6805.pc);
                wr_s_handler_b(m6805.x);
                wr_s_handler_b(m6805.a);
                wr_s_handler_b(m6805.cc);
                m6805.cc |= 0x08;
                if (m6805.subtype == SUBTYPE_HD63705) RM16(0x1ffa, ref m6805.pc);
                else RM16(m6805.amask - 3, ref m6805.pc);
            }


            static void tax()
            {
                m6805.x = m6805.a;
            }

            /* $98 CLC */

            /* $99 SEC */

            /* $9A CLI */

            /* $9B SEI */

            /* $9C RSP inherent ---- */
            static void rsp()
            {
                m6805.s.wl = (ushort)m6805.sp_mask;
            }

            /* $9D NOP inherent ---- */
            static void nop()
            {
            }

            /* $9E ILLEGAL */

            /* $9F TXA inherent ---- */
            static void txa()
            {
                m6805.a = m6805.x;
            }






            /* $a0 SUBA immediate ?*** */
            static void suba_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $a1 CMPA immediate ?*** */
            static void cmpa_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $a2 SBCA immediate ?*** */
            static void sbca_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $a3 CPX immediate -*** */
            static void cpx_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $a4 ANDA immediate -**- */
            static void anda_im()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $a5 BITA immediate -**- */
            static void bita_im()
            {
                byte t, r;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                r = (byte)(m6805.a & t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $a6 LDA immediate -**- */
            static void lda_im()
            {
                { m6805.a = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $a7 ILLEGAL */

            /* $a8 EORA immediate -**- */
            static void eora_im()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $a9 ADCA immediate **** */
            static void adca_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $aa ORA immediate -**- */
            static void ora_im()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $ab ADDA immediate **** */
            static void adda_im()
            {
                ushort t, r;
                { t = ((ushort)cpu_readop_arg(m6805.pc.wl++)); };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $ac ILLEGAL */

            /* $ad BSR ---- */
            static void bsr()
            {
                byte t;
                { t = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
            }

            /* $ae LDX immediate -**- */
            static void ldx_im()
            {
                { m6805.x = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $af ILLEGAL */






            /* $b0 SUBA direct ?*** */
            static void suba_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $b1 CMPA direct ?*** */
            static void cmpa_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $b2 SBCA direct ?*** */
            static void sbca_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $b3 CPX direct -*** */
            static void cpx_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $b4 ANDA direct -**- */
            static void anda_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $b5 BITA direct -**- */
            static void bita_di()
            {
                byte t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)(m6805.a & t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $b6 LDA direct -**- */
            static void lda_di()
            {
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };m6805.a = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $b7 STA direct -**- */
            static void sta_di()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.a);
            }

            /* $b8 EORA direct -**- */
            static void eora_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $b9 ADCA direct **** */
            static void adca_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $ba ORA direct -**- */
            static void ora_di()
            {
                byte t;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = (byte)cpu_readmem16((int)((ea.d) & m6805.amask)); };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $bb ADDA direct **** */
            static void adda_di()
            {
                ushort t, r;
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $bc JMP direct -*** */
            static void jmp_di()
            {
                ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                m6805.pc.wl = ea.wl;
            }

            /* $bd JSR direct ---- */
            static void jsr_di()
            {
                ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl = ea.wl;
            }

            /* $be LDX direct -**- */
            static void ldx_di()
            {
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };m6805.x = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $bf STX direct -**- */
            static void stx_di()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.x);
            }






            /* $c0 SUBA extended ?*** */
            static void suba_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $c1 CMPA extended ?*** */
            static void cmpa_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $c2 SBCA extended ?*** */
            static void sbca_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $c3 CPX extended -*** */
            static void cpx_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $c4 ANDA extended -**- */
            static void anda_ex()
            {
                byte t;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $c5 BITA extended -**- */
            static void bita_ex()
            {
                byte t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (byte)(m6805.a & t);

                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $c6 LDA extended -**- */
            static void lda_ex()
            {
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; m6805.a = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $c7 STA extended -**- */
            static void sta_ex()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                {
                    ea.d = 0; ea.bh = (byte)cpu_readop_arg(m6805.pc.wl); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                    m6805.pc.wl += 2;
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.a);
            }

            /* $c8 EORA extended -**- */
            static void eora_ex()
            {
                byte t;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $c9 ADCA extended **** */
            static void adca_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $ca ORA extended -**- */
            static void ora_ex()
            {
                byte t;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $cb ADDA extended **** */
            static void adda_ex()
            {
                ushort t, r;
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $cc JMP extended -*** */
            static void jmp_ex()
            {
                {
                    ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                    m6805.pc.wl += 2;
                };
                m6805.pc.wl = ea.wl;
            }

            /* $cd JSR extended ---- */
            static void jsr_ex()
            {
                {
                    ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                    m6805.pc.wl += 2;
                };
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl = ea.wl;
            }

            /* $ce LDX extended -**- */
            static void ldx_ex()
            {
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; m6805.x = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $cf STX extended -**- */
            static void stx_ex()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                {
                    ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                    m6805.pc.wl += 2;
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.x);
            }






            /* $d0 SUBA indexed, 2 byte offset ?*** */
            static void suba_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $d1 CMPA indexed, 2 byte offset ?*** */
            static void cmpa_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $d2 SBCA indexed, 2 byte offset ?*** */
            static void sbca_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $d3 CPX indexed, 2 byte offset -*** */
            static void cpx_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $d4 ANDA indexed, 2 byte offset -**- */
            static void anda_ix2()
            {
                byte t;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $d5 BITA indexed, 2 byte offset -**- */
            static void bita_ix2()
            {
                byte t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (byte)(m6805.a & t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $d6 LDA indexed, 2 byte offset -**- */
            static void lda_ix2()
            {
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; m6805.a = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $d7 STA indexed, 2 byte offset -**- */
            static void sta_ix2()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; ea.wl += m6805.x;
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.a);
            }

            /* $d8 EORA indexed, 2 byte offset -**- */
            static void eora_ix2()
            {
                byte t;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $d9 ADCA indexed, 2 byte offset **** */
            static void adca_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $da ORA indexed, 2 byte offset -**- */
            static void ora_ix2()
            {
                byte t;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = (byte)cpu_readop_arg((uint)m6805.pc.wl + 1);
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $db ADDA indexed, 2 byte offset **** */
            static void adda_ix2()
            {
                ushort t, r;
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $dc JMP indexed, 2 byte offset -*** */
            static void jmp_ix2()
            {
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = (byte)cpu_readop_arg((uint)(m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; ea.wl += m6805.x;
                };
                m6805.pc.wl = ea.wl;
            }

            /* $dd JSR indexed, 2 byte offset ---- */
            static void jsr_ix2()
            {
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; ea.wl += m6805.x;
                };
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl = ea.wl;
            }

            /* $de LDX indexed, 2 byte offset -**- */
            static void ldx_ix2()
            {
                {
                    {
                        {
                            ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                            m6805.pc.wl += 2;
                        }; ea.wl += m6805.x;
                    }; m6805.x = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $df STX indexed, 2 byte offset -**- */
            static void stx_ix2()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                {
                    {
                        ea.d = 0; ea.bh = ((byte)cpu_readop_arg(m6805.pc.wl)); ea.bl = ((byte)cpu_readop_arg((uint)m6805.pc.wl + 1));
                        m6805.pc.wl += 2;
                    }; ea.wl += m6805.x;
                };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.x);
            }






            /* $e0 SUBA indexed, 1 byte offset ?*** */
            static void suba_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $e1 CMPA indexed, 1 byte offset ?*** */
            static void cmpa_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $e2 SBCA indexed, 1 byte offset ?*** */
            static void sbca_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $e3 CPX indexed, 1 byte offset -*** */
            static void cpx_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $e4 ANDA indexed, 1 byte offset -**- */
            static void anda_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $e5 BITA indexed, 1 byte offset -**- */
            static void bita_ix1()
            {
                byte t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (byte)(m6805.a & t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $e6 LDA indexed, 1 byte offset -**- */
            static void lda_ix1()
            {
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    m6805.a = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $e7 STA indexed, 1 byte offset -**- */
            static void sta_ix1()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.a);
            }

            /* $e8 EORA indexed, 1 byte offset -**- */
            static void eora_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $e9 ADCA indexed, 1 byte offset **** */
            static void adca_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $ea ORA indexed, 1 byte offset -**- */
            static void ora_ix1()
            {
                byte t;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $eb ADDA indexed, 1 byte offset **** */
            static void adda_ix1()
            {
                ushort t, r;
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $ec JMP indexed, 1 byte offset -*** */
            static void jmp_ix1()
            {
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                m6805.pc.wl = ea.wl;
            }

            /* $ed JSR indexed, 1 byte offset ---- */
            static void jsr_ix1()
            {
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl = ea.wl;
            }

            /* $ee LDX indexed, 1 byte offset -**- */
            static void ldx_ix1()
            {
                {
                    { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                    m6805.x = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask)));
                };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $ef STX indexed, 1 byte offset -**- */
            static void stx_ix1()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                { ea.d = 0; { ea.bl = ((byte)cpu_readop_arg(m6805.pc.wl++)); }; ea.wl += m6805.x; };
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.x);
            }






            /* $f0 SUBA indexed ?*** */
            static void suba_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $f1 CMPA indexed ?*** */
            static void cmpa_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $f2 SBCA indexed ?*** */
            static void sbca_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a - t - (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0)m6805.cc |= 0x02; m6805.cc |= (byte)((r & 0x100) >> 8); };
                m6805.a = (byte)r;
            }

            /* $f3 CPX indexed -*** */
            static void cpx_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.x - t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $f4 ANDA indexed -**- */
            static void anda_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.a &= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $f5 BITA indexed -**- */
            static void bita_ix()
            {
                byte t, r;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (byte)(m6805.a & t);
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((r & 0x80) >> 5); if (r == 0)m6805.cc |= 0x02; };
            }

            /* $f6 LDA indexed -**- */
            static void lda_ix()
            {
                { ea.wl = m6805.x; m6805.a = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $f7 STA indexed -**- */
            static void sta_ix()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
                ea.wl = m6805.x;
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.a);
            }

            /* $f8 EORA indexed -**- */
            static void eora_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.a ^= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $f9 ADCA indexed **** */
            static void adca_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a + t + (m6805.cc & 0x01));
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $fa ORA indexed -**- */
            static void ora_ix()
            {
                byte t;
                { ea.wl = m6805.x; t = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.a |= t;
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.a & 0x80) >> 5); if (m6805.a == 0)m6805.cc |= 0x02; };
            }

            /* $fb ADDA indexed **** */
            static void adda_ix()
            {
                ushort t, r;
                { ea.wl = m6805.x; t = ((ushort)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                r = (ushort)(m6805.a + t);
                m6805.cc &= unchecked((byte)~(0x10 | 0x04 | 0x02 | 0x01));
                {
                    m6805.cc |= (byte)((r & 0x80) >> 5); if (((byte)r) == 0) m6805.cc |= 0x02;
                    m6805.cc |= (byte)((r & 0x100) >> 8);
                };
                m6805.cc |= (byte)((m6805.a ^ t ^ r) & 0x10);
                m6805.a = (byte)r;
            }

            /* $fc JMP indexed -*** */
            static void jmp_ix()
            {
                ea.wl = m6805.x;
                m6805.pc.wl = ea.wl;
            }

            /* $fd JSR indexed ---- */
            static void jsr_ix()
            {
                ea.wl = m6805.x;
                wr_s_handler_w(m6805.pc);
                m6805.pc.wl = ea.wl;
            }

            /* $fe LDX indexed -**- */
            static void ldx_ix()
            {
                { ea.wl = m6805.x; m6805.x = ((byte)cpu_readmem16((int)((ea.d) & m6805.amask))); };
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
            }

            /* $ff STX indexed -**- */
            static void stx_ix()
            {
                m6805.cc &= unchecked((byte)~(0x04 | 0x02));
                { m6805.cc |= (byte)((m6805.x & 0x80) >> 5); if (m6805.x == 0)m6805.cc |= 0x02; };
                ea.wl = m6805.x;
                cpu_writemem16((int)((ea.d) & m6805.amask), m6805.x);
            }

        }
        class cpu_m68705 : cpu_m6805
        {
            public cpu_m68705()
                : base()
            {
                cpu_num = CPU_M68705;
            }
            public override void reset(object param)
            {
                base.reset(param);
                m6805.subtype = SUBTYPE_M68705;
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M68705";
                    case CPU_INFO_VERSION: return "1.1";
                }
                return base.cpu_info(context, regnum);
            }
        }
        class cpu_hd63705 : cpu_m6805
        {
            public cpu_hd63705()
                : base()
            {
                cpu_num = CPU_HD63705;
            }
            public override void reset(object param)
            {
                base.reset(param);
                m6805.subtype = SUBTYPE_HD63705;
                m6805.amask = 0xffff;
                m6805.sp_mask = 0x17f;
                m6805.sp_low = 0x100;
                RM16(0x1ffe, ref m6805.pc);
                m6805.s.wl = 0x17f;
            }
        }
    }
}