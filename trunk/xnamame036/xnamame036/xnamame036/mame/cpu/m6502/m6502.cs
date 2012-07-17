using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_m6502_2 : cpu_interface
        {
            public cpu_m6502_2()
            {
                cpu_num = CPU_M6502;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = 0;// M6502_INT_NONE;
                irq_int = 1;// M6502_INT_IRQ;
                nmi_int = 2;// M6502_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 3;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6502_ICount;
            }
            /* set to 1 to test cur_mrhard/cur_wmhard to avoid calls */
            public delegate void opcode();
            /****************************************************************************

             * The 6502 registers.

             ****************************************************************************/
            class m6502_Regs
            {
                public byte subtype; /* currently selected cpu sub type */
                public opcode[] insn; /* pointer to the function pointer table */
                public PAIR ppc; /* previous program counter */
                public PAIR pc; /* program counter */
                public PAIR sp; /* stack pointer (always 100 - 1FF) */
                public PAIR zp; /* zero page address */
                public PAIR ea; /* effective address */
                public byte a; /* Accumulator */
                public byte x; /* X index register */
                public byte y; /* Y index register */
                public byte p; /* Processor status */
                public byte pending_irq; /* nonzero if an IRQ is pending */
                public byte after_cli; /* pending IRQ and last insn cleared I */
                public byte nmi_state;
                public byte irq_state;
                public byte so_state;
                public irqcallback irq_callback; /* IRQ callback */
            }

            static int[] m6502_ICount = new int[1];

            static m6502_Regs m6502 = new m6502_Regs();




            static void m6502_00() { m6502_ICount[0] -= 7; m6502.pc.wl++; cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bh); m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bl); m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, (int)m6502.p | 0x10); m6502.sp.bl--; m6502.p = (byte)((m6502.p | 0x04) & ~0x08); m6502.pc.bl = (byte)cpu_readmem16((int)0xfffe); m6502.pc.bh = (byte)cpu_readmem16((int)0xfffe + 1); change_pc16(m6502.pc.d); } /* 7 BRK */
            static void m6502_20() { m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bh); m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bl); m6502.sp.bl--; m6502.ea.bh = cpu_readop_arg((uint)m6502.pc.wl++); m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } /* 6 JSR */
            static void m6502_40() { m6502_ICount[0] -= 6; m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d); m6502.sp.bl++; m6502.pc.bl = (byte)cpu_readmem16((int)m6502.sp.d); m6502.sp.bl++; m6502.pc.bh = (byte)cpu_readmem16((int)m6502.sp.d); m6502.p |= 0x20; if ((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) == 0) { ; m6502.after_cli = 1; } change_pc16(m6502.pc.d); } /* 6 RTI */
            static void m6502_60() { m6502_ICount[0] -= 6; m6502.sp.bl++; m6502.pc.bl = (byte)cpu_readmem16((int)m6502.sp.d); m6502.sp.bl++; m6502.pc.bh = (byte)cpu_readmem16((int)m6502.sp.d); m6502.pc.wl++; change_pc16(m6502.pc.d); } /* 6 RTS */
            static void m6502_80() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_a0() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 2 LDY IMM */
            static void m6502_c0() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01; if (((byte)(m6502.y - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80)); } /* 2 CPY IMM */
            static void m6502_e0() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01; if (((byte)(m6502.x - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80)); } /* 2 CPX IMM */

            static void m6502_10() { int tmp; if ((m6502.p & 0x80) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BPL REL */
            static void m6502_30() { int tmp; if ((m6502.p & 0x80) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BMI REL */
            static void m6502_50() { int tmp; if ((m6502.p & 0x02) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BVC REL */
            static void m6502_70() { int tmp; if ((m6502.p & 0x02) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BVS REL */
            static void m6502_90() { int tmp; if ((m6502.p & 0x01) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BCC REL */
            static void m6502_b0() { int tmp; if ((m6502.p & 0x01) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BCS REL */
            static void m6502_d0() { int tmp; if ((m6502.p & 0x02) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BNE REL */
            static void m6502_f0() { int tmp; if ((m6502.p & 0x02) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BEQ REL */

            static void m6502_01() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 ORA IDX */
            static void m6502_21() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 AND IDX */
            static void m6502_41() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 EOR IDX */
            static void m6502_61() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 ADC IDX */
            static void m6502_81() { int tmp; m6502_ICount[0] -= 6; tmp = m6502.a; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 STA IDX */
            static void m6502_a1() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 LDA IDX */
            static void m6502_c1() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 6 CMP IDX */
            static void m6502_e1() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 SBC IDX */

            static void m6502_11() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 ORA IDY */
            static void m6502_31() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 AND IDY */
            static void m6502_51() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 EOR IDY */
            static void m6502_71() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 ADC IDY */
            static void m6502_91() { int tmp; m6502_ICount[0] -= 6; tmp = m6502.a; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 STA IDY */
            static void m6502_b1() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 LDA IDY */
            static void m6502_d1() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 5 CMP IDY */
            static void m6502_f1() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 SBC IDY */

            static void m6502_02() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_22() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_42() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_62() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_82() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_a2() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 LDX IMM */
            static void m6502_c2() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_e2() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_12() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_32() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_52() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_72() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_92() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_b2() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_d2() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_f2() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_03() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_23() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_43() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_63() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_83() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_a3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_c3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_e3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_13() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_33() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_53() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_73() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_93() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_b3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_d3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_f3() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_04() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_24() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~(0x80 | 0x02 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x02)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; } /* 3 BIT ZPG */
            static void m6502_44() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_64() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_84() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.y; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 STY ZPG */
            static void m6502_a4() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 3 LDY ZPG */
            static void m6502_c4() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01; if (((byte)(m6502.y - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80)); } /* 3 CPY ZPG */
            static void m6502_e4() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01; if (((byte)(m6502.x - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80)); } /* 3 CPX ZPG */

            static void m6502_14() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_34() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_54() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_74() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_94() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.y; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STY ZPX */
            static void m6502_b4() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 4 LDY ZPX */
            static void m6502_d4() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_f4() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_05() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ORA ZPG */
            static void m6502_25() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 AND ZPG */
            static void m6502_45() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 EOR ZPG */
            static void m6502_65() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ADC ZPG */
            static void m6502_85() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.a; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 STA ZPG */
            static void m6502_a5() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 LDA ZPG */
            static void m6502_c5() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 3 CMP ZPG */
            static void m6502_e5() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 SBC ZPG */

            static void m6502_15() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ORA ZPX */
            static void m6502_35() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 AND ZPX */
            static void m6502_55() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 EOR ZPX */
            static void m6502_75() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ZPX */
            static void m6502_95() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.a; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STA ZPX */
            static void m6502_b5() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LDA ZPX */
            static void m6502_d5() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 4 CMP ZPX */
            static void m6502_f5() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ZPX */

            static void m6502_06() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 ASL ZPG */
            static void m6502_26() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 ROL ZPG */
            static void m6502_46() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 LSR ZPG */
            static void m6502_66() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 ROR ZPG */
            static void m6502_86() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.x; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 STX ZPG */
            static void m6502_a6() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 3 LDX ZPG */
            static void m6502_c6() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 DEC ZPG */
            static void m6502_e6() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 INC ZPG */

            static void m6502_16() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ASL ZPX */
            static void m6502_36() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ROL ZPX */
            static void m6502_56() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 LSR ZPX */
            static void m6502_76() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ROR ZPX */
            static void m6502_96() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.x; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.y); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STX ZPY */
            static void m6502_b6() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.y); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 4 LDX ZPY */
            static void m6502_d6() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DEC ZPX */
            static void m6502_f6() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 INC ZPX */

            static void m6502_07() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_27() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_47() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_67() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_87() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_a7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_c7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_e7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_17() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_37() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_57() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_77() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_97() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_b7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_d7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_f7() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_08() { m6502_ICount[0] -= 2; cpu_writemem16((int)m6502.sp.d, (int)m6502.p); m6502.sp.bl--; } /* 2 PHP */
            static void m6502_28() { m6502_ICount[0] -= 2; if ((m6502.p & 0x04) != 0) { m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d); if ((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) == 0) { ; m6502.after_cli = 1; } } else { m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d); } m6502.p |= 0x20; } /* 2 PLP */
            static void m6502_48() { m6502_ICount[0] -= 2; cpu_writemem16((int)m6502.sp.d, (int)m6502.a); m6502.sp.bl--; } /* 2 PHA */
            static void m6502_68() { m6502_ICount[0] -= 2; m6502.sp.bl++; m6502.a = (byte)cpu_readmem16((int)m6502.sp.d); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 PLA */
            static void m6502_88() { m6502_ICount[0] -= 2; m6502.y = (byte)--m6502.y; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 2 DEY */
            static void m6502_a8() { m6502_ICount[0] -= 2; m6502.y = m6502.a; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 2 TAY */
            static void m6502_c8() { m6502_ICount[0] -= 2; m6502.y = (byte)++m6502.y; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 2 INY */
            static void m6502_e8() { m6502_ICount[0] -= 2; m6502.x = (byte)++m6502.x; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 INX */

            static void m6502_18() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x01); } /* 2 CLC */
            static void m6502_38() { m6502_ICount[0] -= 2; m6502.p |= 0x01; } /* 2 SEC */
            static void m6502_58() { m6502_ICount[0] -= 2; if ((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) != 0) { m6502.after_cli = 1; } m6502.p &= unchecked((byte)~0x04); } /* 2 CLI */
            static void m6502_78() { m6502_ICount[0] -= 2; m6502.p |= 0x04; } /* 2 SEI */
            static void m6502_98() { m6502_ICount[0] -= 2; m6502.a = m6502.y; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 TYA */
            static void m6502_b8() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x02); } /* 2 CLV */
            static void m6502_d8() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x08); } /* 2 CLD */
            static void m6502_f8() { m6502_ICount[0] -= 2; m6502.p |= 0x08; } /* 2 SED */

            static void m6502_09() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ORA IMM */
            static void m6502_29() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 AND IMM */
            static void m6502_49() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 EOR IMM */
            static void m6502_69() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ADC IMM */
            static void m6502_89() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_a9() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 LDA IMM */
            static void m6502_c9() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 2 CMP IMM */
            static void m6502_e9() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 SBC IMM */

            static void m6502_19() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ORA ABY */
            static void m6502_39() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 AND ABY */
            static void m6502_59() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 EOR ABY */
            static void m6502_79() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABY */
            static void m6502_99() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.a; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 STA ABY */
            static void m6502_b9() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LDA ABY */
            static void m6502_d9() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 4 CMP ABY */
            static void m6502_f9() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABY */

            static void m6502_0a() { int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ; } /* 2 ASL A */
            static void m6502_2a() { int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ; } /* 2 ROL A */
            static void m6502_4a() { int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ; } /* 2 LSR A */
            static void m6502_6a() { int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ; } /* 2 ROR A */
            static void m6502_8a() { m6502_ICount[0] -= 2; m6502.a = m6502.x; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 TXA */
            static void m6502_aa() { m6502_ICount[0] -= 2; m6502.x = m6502.a; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 TAX */
            static void m6502_ca() { m6502_ICount[0] -= 2; m6502.x = (byte)--m6502.x; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 DEX */
            static void m6502_ea() { m6502_ICount[0] -= 2; ; } /* 2 NOP */

            static void m6502_1a() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_3a() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_5a() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_7a() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_9a() { m6502_ICount[0] -= 2; m6502.sp.bl = m6502.x; } /* 2 TXS */
            static void m6502_ba() { m6502_ICount[0] -= 2; m6502.x = m6502.sp.bl; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 TSX */
            static void m6502_da() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_fa() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_0b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_2b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_4b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_6b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_8b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_ab() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_cb() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_eb() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_1b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_3b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_5b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_7b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_9b() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_bb() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_db() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_fb() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_0c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_2c() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~(0x80 | 0x02 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x02)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; } /* 4 BIT ABS */
            static void m6502_4c() { m6502_ICount[0] -= 3; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); if (m6502.ea.d == m6502.ppc.d && m6502.pending_irq == 0 && m6502.after_cli == 0) if (m6502_ICount[0] > 0) m6502_ICount[0] = 0; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } /* 3 JMP ABS */
            static void m6502_6c() { int tmp; m6502_ICount[0] -= 5; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = (byte)cpu_readmem16((int)m6502.ea.d); m6502.ea.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.ea.d); m6502.ea.bl = (byte)tmp; if (m6502.ea.d == m6502.ppc.d && m6502.pending_irq == 0 && m6502.after_cli == 0) if (m6502_ICount[0] > 0) m6502_ICount[0] = 0; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } /* 5 JMP IND */
            static void m6502_8c() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.y; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STY ABS */
            static void m6502_ac() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 4 LDY ABS */
            static void m6502_cc() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01; if (((byte)(m6502.y - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80)); } /* 4 CPY ABS */
            static void m6502_ec() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01; if (((byte)(m6502.x - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80)); } /* 4 CPX ABS */

            static void m6502_1c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_3c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_5c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_7c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_9c() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_bc() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80)); } /* 4 LDY ABX */
            static void m6502_dc() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_fc() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_0d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ORA ABS */
            static void m6502_2d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 AND ABS */
            static void m6502_4d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 EOR ABS */
            static void m6502_6d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABS */
            static void m6502_8d() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.a; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STA ABS */
            static void m6502_ad() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LDA ABS */
            static void m6502_cd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 4 CMP ABS */
            static void m6502_ed() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABS */

            static void m6502_1d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ORA ABX */
            static void m6502_3d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 AND ABX */
            static void m6502_5d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 EOR ABX */
            static void m6502_7d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABX */
            static void m6502_9d() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.a; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 STA ABX */
            static void m6502_bd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LDA ABX */
            static void m6502_dd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 4 CMP ABX */
            static void m6502_fd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABX */

            static void m6502_0e() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ASL ABS */
            static void m6502_2e() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ROL ABS */
            static void m6502_4e() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 LSR ABS */
            static void m6502_6e() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ROR ABS */
            static void m6502_8e() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.x; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 STX ABS */
            static void m6502_ae() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 4 LDX ABS */
            static void m6502_ce() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DEC ABS */
            static void m6502_ee() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 INC ABS */

            static void m6502_1e() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 ASL ABX */
            static void m6502_3e() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 ROL ABX */
            static void m6502_5e() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 LSR ABX */
            static void m6502_7e() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 ROR ABX */
            static void m6502_9e() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_be() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 4 LDX ABY */
            static void m6502_de() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 DEC ABX */
            static void m6502_fe() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 INC ABX */

            static void m6502_0f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_2f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_4f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_6f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_8f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_af() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_cf() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_ef() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            static void m6502_1f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_3f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_5f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_7f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_9f() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_bf() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_df() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */
            static void m6502_ff() { m6502_ICount[0] -= 2; printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)(m6502.pc.wl - 1) & 0xffff)); } /* 2 ILL */

            /* and here's the array of function pointers */

            static opcode[] insn6502 = {
 m6502_00,m6502_01,m6502_02,m6502_03,m6502_04,m6502_05,m6502_06,m6502_07,
 m6502_08,m6502_09,m6502_0a,m6502_0b,m6502_0c,m6502_0d,m6502_0e,m6502_0f,
 m6502_10,m6502_11,m6502_12,m6502_13,m6502_14,m6502_15,m6502_16,m6502_17,
 m6502_18,m6502_19,m6502_1a,m6502_1b,m6502_1c,m6502_1d,m6502_1e,m6502_1f,
 m6502_20,m6502_21,m6502_22,m6502_23,m6502_24,m6502_25,m6502_26,m6502_27,
 m6502_28,m6502_29,m6502_2a,m6502_2b,m6502_2c,m6502_2d,m6502_2e,m6502_2f,
 m6502_30,m6502_31,m6502_32,m6502_33,m6502_34,m6502_35,m6502_36,m6502_37,
 m6502_38,m6502_39,m6502_3a,m6502_3b,m6502_3c,m6502_3d,m6502_3e,m6502_3f,
 m6502_40,m6502_41,m6502_42,m6502_43,m6502_44,m6502_45,m6502_46,m6502_47,
 m6502_48,m6502_49,m6502_4a,m6502_4b,m6502_4c,m6502_4d,m6502_4e,m6502_4f,
 m6502_50,m6502_51,m6502_52,m6502_53,m6502_54,m6502_55,m6502_56,m6502_57,
 m6502_58,m6502_59,m6502_5a,m6502_5b,m6502_5c,m6502_5d,m6502_5e,m6502_5f,
 m6502_60,m6502_61,m6502_62,m6502_63,m6502_64,m6502_65,m6502_66,m6502_67,
 m6502_68,m6502_69,m6502_6a,m6502_6b,m6502_6c,m6502_6d,m6502_6e,m6502_6f,
 m6502_70,m6502_71,m6502_72,m6502_73,m6502_74,m6502_75,m6502_76,m6502_77,
 m6502_78,m6502_79,m6502_7a,m6502_7b,m6502_7c,m6502_7d,m6502_7e,m6502_7f,
 m6502_80,m6502_81,m6502_82,m6502_83,m6502_84,m6502_85,m6502_86,m6502_87,
 m6502_88,m6502_89,m6502_8a,m6502_8b,m6502_8c,m6502_8d,m6502_8e,m6502_8f,
 m6502_90,m6502_91,m6502_92,m6502_93,m6502_94,m6502_95,m6502_96,m6502_97,
 m6502_98,m6502_99,m6502_9a,m6502_9b,m6502_9c,m6502_9d,m6502_9e,m6502_9f,
 m6502_a0,m6502_a1,m6502_a2,m6502_a3,m6502_a4,m6502_a5,m6502_a6,m6502_a7,
 m6502_a8,m6502_a9,m6502_aa,m6502_ab,m6502_ac,m6502_ad,m6502_ae,m6502_af,
 m6502_b0,m6502_b1,m6502_b2,m6502_b3,m6502_b4,m6502_b5,m6502_b6,m6502_b7,
 m6502_b8,m6502_b9,m6502_ba,m6502_bb,m6502_bc,m6502_bd,m6502_be,m6502_bf,
 m6502_c0,m6502_c1,m6502_c2,m6502_c3,m6502_c4,m6502_c5,m6502_c6,m6502_c7,
 m6502_c8,m6502_c9,m6502_ca,m6502_cb,m6502_cc,m6502_cd,m6502_ce,m6502_cf,
 m6502_d0,m6502_d1,m6502_d2,m6502_d3,m6502_d4,m6502_d5,m6502_d6,m6502_d7,
 m6502_d8,m6502_d9,m6502_da,m6502_db,m6502_dc,m6502_dd,m6502_de,m6502_df,
 m6502_e0,m6502_e1,m6502_e2,m6502_e3,m6502_e4,m6502_e5,m6502_e6,m6502_e7,
 m6502_e8,m6502_e9,m6502_ea,m6502_eb,m6502_ec,m6502_ed,m6502_ee,m6502_ef,
 m6502_f0,m6502_f1,m6502_f2,m6502_f3,m6502_f4,m6502_f5,m6502_f6,m6502_f7,
 m6502_f8,m6502_f9,m6502_fa,m6502_fb,m6502_fc,m6502_fd,m6502_fe,m6502_ff
};
            static void m65c02_80() { int tmp; if (true) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 2 BRA */
            static void m65c02_12() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ORA ZPI */
            static void m65c02_32() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 AND ZPI */
            static void m65c02_52() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 EOR ZPI */
            static void m65c02_72() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ADC ZPI */
            static void m65c02_92() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.a; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 STA ZPI */
            static void m65c02_b2() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 LDA ZPI */
            static void m65c02_d2() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); } /* 3 CMP ZPI */
            static void m65c02_f2() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 SBC ZPI */
            static void m65c02_04() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; else m6502.p &= unchecked((byte)~0x02); tmp |= m6502.a; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 TSB ZPG */


            static void m65c02_64() { int tmp; m6502_ICount[0] -= 2; tmp = 0; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 STZ ZPG */





            static void m65c02_14() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; else m6502.p &= unchecked((byte)~0x02); tmp &= ~m6502.a; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 TRB ZPG */
            static void m65c02_34() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~(0x80 | 0x02 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x02)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; } /* 4 BIT ZPX */

            static void m65c02_74() { int tmp; m6502_ICount[0] -= 4; tmp = 0; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STZ ZPX */
            static void m65c02_07() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 0); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB0 ZPG */
            static void m65c02_27() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 2); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB2 ZPG */
            static void m65c02_47() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 4); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB4 ZPG */
            static void m65c02_67() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 6); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB6 ZPG */
            static void m65c02_87() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 0); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB0 ZPG */
            static void m65c02_a7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 2); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB2 ZPG */
            static void m65c02_c7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 4); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB4 ZPG */
            static void m65c02_e7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 6); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB6 ZPG */

            static void m65c02_17() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 1); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB1 ZPG */
            static void m65c02_37() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 3); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB3 ZPG */
            static void m65c02_57() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 5); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB5 ZPG */
            static void m65c02_77() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp &= ~(1 << 7); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RMB7 ZPG */
            static void m65c02_97() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 1); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB1 ZPG */
            static void m65c02_b7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 3); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB3 ZPG */
            static void m65c02_d7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 5); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB5 ZPG */
            static void m65c02_f7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (1 << 7); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SMB7 ZPG */
            static void m65c02_89() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~(0x80 | 0x02 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x02)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; } /* 2 BIT IMM */
            static void m65c02_1a() { m6502_ICount[0] -= 2; m6502.a = (byte)++m6502.a; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 INA */
            static void m65c02_3a() { m6502_ICount[0] -= 2; m6502.a = (byte)--m6502.a; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 DEA */
            static void m65c02_5a() { m6502_ICount[0] -= 3; cpu_writemem16((int)m6502.sp.d, (int)m6502.y); m6502.sp.bl--; } /* 3 PHY */
            static void m65c02_7a() { m6502_ICount[0] -= 4; m6502.sp.bl++; m6502.y = (byte)cpu_readmem16((int)m6502.sp.d); } /* 4 PLY */


            static void m65c02_da() { m6502_ICount[0] -= 3; cpu_writemem16((int)m6502.sp.d, (int)m6502.x); m6502.sp.bl--; } /* 3 PHX */
            static void m65c02_fa() { m6502_ICount[0] -= 4; m6502.sp.bl++; m6502.x = (byte)cpu_readmem16((int)m6502.sp.d); } /* 4 PLX */
            static void m65c02_0c() { int tmp; m6502_ICount[0] -= 2; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; else m6502.p &= unchecked((byte)~0x02); tmp |= m6502.a; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 TSB ABS */
            static void m65c02_1c() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; else m6502.p &= unchecked((byte)~0x02); tmp &= ~m6502.a; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 TRB ABS */
            static void m65c02_3c() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~(0x80 | 0x02 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x02)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02; } /* 4 BIT ABX */

            static void m65c02_7c() { int tmp; m6502_ICount[0] -= 2; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = (byte)cpu_readmem16((int)m6502.ea.d); m6502.ea.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.ea.d); m6502.ea.bl = (byte)tmp; if (m6502.ea.bl + m6502.x > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.x; if (m6502.ea.d == m6502.ppc.d && m6502.pending_irq == 0 && m6502.after_cli == 0) if (m6502_ICount[0] > 0) m6502_ICount[0] = 0; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } /* 6 JMP IAX */
            static void m65c02_9c() { int tmp; m6502_ICount[0] -= 4; tmp = 0; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 STZ ABS */
            static void m65c02_9e() { int tmp; m6502_ICount[0] -= 5; tmp = 0; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 STZ ABX */




            static void m65c02_0f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 0)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR0 ZPG */
            static void m65c02_2f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 2)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR2 ZPG */
            static void m65c02_4f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 4)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR4 ZPG */
            static void m65c02_6f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 6)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR6 ZPG */
            static void m65c02_8f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 0)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS0 ZPG */
            static void m65c02_af() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 2)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS2 ZPG */
            static void m65c02_cf() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 4)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS4 ZPG */
            static void m65c02_ef() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 6)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS6 ZPG */

            static void m65c02_1f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 1)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR1 ZPG */
            static void m65c02_3f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 3)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR3 ZPG */
            static void m65c02_5f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 5)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR5 ZPG */
            static void m65c02_7f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 7)) == 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBR7 ZPG */
            static void m65c02_9f() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 1)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS1 ZPG */
            static void m65c02_bf() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 3)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS3 ZPG */
            static void m65c02_df() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 5)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS5 ZPG */
            static void m65c02_ff() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); if ((tmp & (1 << 7)) != 0) { tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } else { m6502.pc.wl++; m6502_ICount[0] -= 2; }; } /* 5 BBS7 ZPG */

            static opcode[] insn65c02 = {
 m6502_00,m6502_01,m6502_02,m6502_03,m65c02_04,m6502_05,m6502_06,m65c02_07,
 m6502_08,m6502_09,m6502_0a,m6502_0b,m65c02_0c,m6502_0d,m6502_0e,m65c02_0f,
 m6502_10,m6502_11,m65c02_12,m6502_13,m65c02_14,m6502_15,m6502_16,m65c02_17,
 m6502_18,m6502_19,m65c02_1a,m6502_1b,m65c02_1c,m6502_1d,m6502_1e,m65c02_1f,
 m6502_20,m6502_21,m6502_22,m6502_23,m6502_24,m6502_25,m6502_26,m65c02_27,
 m6502_28,m6502_29,m6502_2a,m6502_2b,m6502_2c,m6502_2d,m6502_2e,m65c02_2f,
 m6502_30,m6502_31,m65c02_32,m6502_33,m65c02_34,m6502_35,m6502_36,m65c02_37,
 m6502_38,m6502_39,m65c02_3a,m6502_3b,m65c02_3c,m6502_3d,m6502_3e,m65c02_3f,
 m6502_40,m6502_41,m6502_42,m6502_43,m6502_44,m6502_45,m6502_46,m65c02_47,
 m6502_48,m6502_49,m6502_4a,m6502_4b,m6502_4c,m6502_4d,m6502_4e,m65c02_4f,
 m6502_50,m6502_51,m65c02_52,m6502_53,m6502_54,m6502_55,m6502_56,m65c02_57,
 m6502_58,m6502_59,m65c02_5a,m6502_5b,m6502_5c,m6502_5d,m6502_5e,m65c02_5f,
 m6502_60,m6502_61,m6502_62,m6502_63,m65c02_64,m6502_65,m6502_66,m65c02_67,
 m6502_68,m6502_69,m6502_6a,m6502_6b,m6502_6c,m6502_6d,m6502_6e,m65c02_6f,
 m6502_70,m6502_71,m65c02_72,m6502_73,m65c02_74,m6502_75,m6502_76,m65c02_77,
 m6502_78,m6502_79,m65c02_7a,m6502_7b,m65c02_7c,m6502_7d,m6502_7e,m65c02_7f,
 m65c02_80,m6502_81,m6502_82,m6502_83,m6502_84,m6502_85,m6502_86,m65c02_87,
 m6502_88,m65c02_89,m6502_8a,m6502_8b,m6502_8c,m6502_8d,m6502_8e,m65c02_8f,
 m6502_90,m6502_91,m65c02_92,m6502_93,m6502_94,m6502_95,m6502_96,m65c02_97,
 m6502_98,m6502_99,m6502_9a,m6502_9b,m65c02_9c,m6502_9d,m65c02_9e,m65c02_9f,
 m6502_a0,m6502_a1,m6502_a2,m6502_a3,m6502_a4,m6502_a5,m6502_a6,m65c02_a7,
 m6502_a8,m6502_a9,m6502_aa,m6502_ab,m6502_ac,m6502_ad,m6502_ae,m65c02_af,
 m6502_b0,m6502_b1,m65c02_b2,m6502_b3,m6502_b4,m6502_b5,m6502_b6,m65c02_b7,
 m6502_b8,m6502_b9,m6502_ba,m6502_bb,m6502_bc,m6502_bd,m6502_be,m65c02_bf,
 m6502_c0,m6502_c1,m6502_c2,m6502_c3,m6502_c4,m6502_c5,m6502_c6,m65c02_c7,
 m6502_c8,m6502_c9,m6502_ca,m6502_cb,m6502_cc,m6502_cd,m6502_ce,m65c02_cf,
 m6502_d0,m6502_d1,m65c02_d2,m6502_d3,m6502_d4,m6502_d5,m6502_d6,m65c02_d7,
 m6502_d8,m6502_d9,m65c02_da,m6502_db,m6502_dc,m6502_dd,m6502_de,m65c02_df,
 m6502_e0,m6502_e1,m6502_e2,m6502_e3,m6502_e4,m6502_e5,m6502_e6,m65c02_e7,
 m6502_e8,m6502_e9,m6502_ea,m6502_eb,m6502_ec,m6502_ed,m6502_ee,m65c02_ef,
 m6502_f0,m6502_f1,m65c02_f2,m6502_f3,m6502_f4,m6502_f5,m6502_f6,m65c02_f7,
 m6502_f8,m6502_f9,m65c02_fa,m6502_fb,m6502_fc,m6502_fd,m6502_fe,m65c02_ff
};
            /*****************************************************************************

             *****************************************************************************

             *

             *	 overrides for 65C02 opcodes

             *

             *****************************************************************************

             * op	 temp	  cycles			 rdmem	 opc  wrmem   ********************/
            static void m65sc02_63() { m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bh); m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bl); m6502.sp.bl--; m6502.ea.bh = cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl = (ushort)(m6502.pc.wl + (short)(m6502.ea.wl - 1)); m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d); } /* ? BSR */
            /* following 16 instructions only 65sc02 in documentation */
            static opcode[] insn65sc02 = {
 m6502_00,m6502_01,m6502_02,m6502_03,m65c02_04,m6502_05,m6502_06,m65c02_07,
 m6502_08,m6502_09,m6502_0a,m6502_0b,m65c02_0c,m6502_0d,m6502_0e,m65c02_0f,
 m6502_10,m6502_11,m65c02_12,m6502_13,m65c02_14,m6502_15,m6502_16,m65c02_17,
 m6502_18,m6502_19,m65c02_1a,m6502_1b,m65c02_1c,m6502_1d,m6502_1e,m65c02_1f,
 m6502_20,m6502_21,m6502_22,m6502_23,m6502_24,m6502_25,m6502_26,m65c02_27,
 m6502_28,m6502_29,m6502_2a,m6502_2b,m6502_2c,m6502_2d,m6502_2e,m65c02_2f,
 m6502_30,m6502_31,m65c02_32,m6502_33,m65c02_34,m6502_35,m6502_36,m65c02_37,
 m6502_38,m6502_39,m65c02_3a,m6502_3b,m65c02_3c,m6502_3d,m6502_3e,m65c02_3f,
 m6502_40,m6502_41,m6502_42,m6502_43,m6502_44,m6502_45,m6502_46,m65c02_47,
 m6502_48,m6502_49,m6502_4a,m6502_4b,m6502_4c,m6502_4d,m6502_4e,m65c02_4f,
 m6502_50,m6502_51,m65c02_52,m6502_53,m6502_54,m6502_55,m6502_56,m65c02_57,
 m6502_58,m6502_59,m65c02_5a,m6502_5b,m6502_5c,m6502_5d,m6502_5e,m65c02_5f,
 m6502_60,m6502_61,m6502_62,m65sc02_63,m65c02_64,m6502_65,m6502_66,m65c02_67,
 m6502_68,m6502_69,m6502_6a,m6502_6b,m6502_6c,m6502_6d,m6502_6e,m65c02_6f,
 m6502_70,m6502_71,m65c02_72,m6502_73,m65c02_74,m6502_75,m6502_76,m65c02_77,
 m6502_78,m6502_79,m65c02_7a,m6502_7b,m65c02_7c,m6502_7d,m6502_7e,m65c02_7f,
 m65c02_80,m6502_81,m6502_82,m6502_83,m6502_84,m6502_85,m6502_86,m65c02_87,
 m6502_88,m65c02_89,m6502_8a,m6502_8b,m6502_8c,m6502_8d,m6502_8e,m65c02_8f,
 m6502_90,m6502_91,m65c02_92,m6502_93,m6502_94,m6502_95,m6502_96,m65c02_97,
 m6502_98,m6502_99,m6502_9a,m6502_9b,m65c02_9c,m6502_9d,m65c02_9e,m65c02_9f,
 m6502_a0,m6502_a1,m6502_a2,m6502_a3,m6502_a4,m6502_a5,m6502_a6,m65c02_a7,
 m6502_a8,m6502_a9,m6502_aa,m6502_ab,m6502_ac,m6502_ad,m6502_ae,m65c02_af,
 m6502_b0,m6502_b1,m65c02_b2,m6502_b3,m6502_b4,m6502_b5,m6502_b6,m65c02_b7,
 m6502_b8,m6502_b9,m6502_ba,m6502_bb,m6502_bc,m6502_bd,m6502_be,m65c02_bf,
 m6502_c0,m6502_c1,m6502_c2,m6502_c3,m6502_c4,m6502_c5,m6502_c6,m65c02_c7,
 m6502_c8,m6502_c9,m6502_ca,m6502_cb,m6502_cc,m6502_cd,m6502_ce,m65c02_cf,
 m6502_d0,m6502_d1,m65c02_d2,m6502_d3,m6502_d4,m6502_d5,m6502_d6,m65c02_d7,
 m6502_d8,m6502_d9,m65c02_da,m6502_db,m6502_dc,m6502_dd,m6502_de,m65c02_df,
 m6502_e0,m6502_e1,m6502_e2,m6502_e3,m6502_e4,m6502_e5,m6502_e6,m65c02_e7,
 m6502_e8,m6502_e9,m6502_ea,m6502_eb,m6502_ec,m6502_ed,m6502_ee,m65c02_ef,
 m6502_f0,m6502_f1,m65c02_f2,m6502_f3,m6502_f4,m6502_f5,m6502_f6,m65c02_f7,
 m6502_f8,m6502_f9,m65c02_fa,m6502_fb,m6502_fc,m6502_fd,m6502_fe,m65c02_ff
};
            /*****************************************************************************

             *****************************************************************************

             *

             *	 overrides for 6510 opcodes

             *

             *****************************************************************************

             ********** insn   temp 	cycles			   rdmem   opc	wrmem	**********/
            static void m6510_80() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_82() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */

            static void m6510_c2() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_e2() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_03() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 SLO IDX */
            static void m6510_23() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 RLA IDX */
            static void m6510_43() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 SRE IDX */
            static void m6510_63() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 RRA IDX */
            static void m6510_83() { int tmp; m6502_ICount[0] -= 6; tmp = m6502.a & m6502.x; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 SAX IDX */
            static void m6510_a3() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 LAX IDX */
            static void m6510_c3() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 DCP IDX */
            static void m6510_e3() { int tmp; m6502_ICount[0] -= 7; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 ISB IDX */

            static void m6510_13() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SLO IDY */
            static void m6510_33() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RLA IDY */
            static void m6510_53() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SRE IDY */
            static void m6510_73() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RRA IDY */
            static void m6510_93() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.a & m6502.x; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SAH IDY */
            static void m6510_b3() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 LAX IDY */
            static void m6510_d3() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DCP IDY */
            static void m6510_f3() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ISB IDY */

            static void m6510_04() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */

            static void m6510_44() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_64() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */





            static void m6510_14() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_34() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_54() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_74() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */


            static void m6510_d4() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_f4() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_07() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SLO ZPG */
            static void m6510_27() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RLA ZPG */
            static void m6510_47() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SRE ZPG */
            static void m6510_67() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 RRA ZPG */
            static void m6510_87() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.a & m6502.x; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 3 SAX ZPG */
            static void m6510_a7() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 LAX ZPG */
            static void m6510_c7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 DCP ZPG */
            static void m6510_e7() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 ISB ZPG */

            static void m6510_17() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SLO ZPX */
            static void m6510_37() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RLA ZPX */
            static void m6510_57() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SRE ZPX */
            static void m6510_77() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RRA ZPX */
            static void m6510_97() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.a & m6502.x; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SAX ZPY */
            static void m6510_b7() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LAX ZPY */
            static void m6510_d7() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DCP ZPX */
            static void m6510_f7() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ISB ZPX */
            static void m6510_89() { m6502_ICount[0] -= 2; m6502.pc.wl++; } /* 2 DOP */
            static void m6510_1a() { m6502_ICount[0] -= 2; ; } /* 2 NOP */
            static void m6510_3a() { m6502_ICount[0] -= 2; ; } /* 2 NOP */
            static void m6510_5a() { m6502_ICount[0] -= 2; ; } /* 2 NOP */
            static void m6510_7a() { m6502_ICount[0] -= 2; ; } /* 2 NOP */


            static void m6510_da() { m6502_ICount[0] -= 2; ; } /* 2 NOP */
            static void m6510_fa() { m6502_ICount[0] -= 2; ; } /* 2 NOP */

            static void m6510_0b() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a & 0x80) != 0) m6502.p |= 0x01; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ANC IMM */
            static void m6510_2b() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a & 0x80) != 0) m6502.p |= 0x01; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ANC IMM */
            static void m6510_4b() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); tmp = (byte)(m6502.a & tmp); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 2 ASR IMM */
            static void m6510_6b() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); tmp = (byte)(m6502.a & tmp); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 2 ARR IMM */
            static void m6510_8b() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); tmp = (byte)(m6502.x & tmp); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 2 AXA IMM */
            static void m6510_ab() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 LAX IMM */
            static void m6510_cb() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); m6502.x &= m6502.a; if (m6502.x >= tmp) m6502.p |= 0x01; m6502.x = (byte)(m6502.x - tmp); if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80)); } /* 2 ASX IMM */
            static void m6510_eb() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 SBC IMM */

            static void m6510_1b() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SLO ABY */
            static void m6510_3b() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 RLA ABY */
            static void m6510_5b() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SRE ABY */
            static void m6510_7b() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 RRA ABY */
            static void m6510_9b() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.sp.bl = (byte)(m6502.a & m6502.x); tmp &= (byte)(cpu_readop_arg((uint)(m6502.pc.wl + 1) & 0xffff) + 1); m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SSH ABY */
            static void m6510_bb() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.sp.bl &= (byte)tmp; m6502.a = m6502.x = m6502.sp.bl; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 AST ABY */
            static void m6510_db() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DCP ABY */
            static void m6510_fb() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ISB ABY */

            static void m6510_0c() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_1c() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_3c() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_5c() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_7c() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_9c() { int tmp; m6502_ICount[0] -= 5; tmp = m6502.y & (byte)(cpu_readop_arg((uint)(m6502.pc.wl + 1) & 0xffff) + 1); m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SYH ABX */

            static void m6510_dc() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_fc() { m6502_ICount[0] -= 2; m6502.pc.wl += 2; } /* 2 TOP */
            static void m6510_0f() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SLO ABS */
            static void m6510_2f() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RLA ABS */
            static void m6510_4f() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SRE ABS */
            static void m6510_6f() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RRA ABS */
            static void m6510_8f() { int tmp; m6502_ICount[0] -= 4; tmp = m6502.a & m6502.x; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SAX ABS */
            static void m6510_af() { int tmp; m6502_ICount[0] -= 5; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LAX ABS */
            static void m6510_cf() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 DCP ABS */
            static void m6510_ef() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 6 ISB ABS */

            static void m6510_1f() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); m6502.a |= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SLO ABX */
            static void m6510_3f() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; m6502.a &= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RLA ABX */
            static void m6510_5f() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; m6502.a ^= (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 SRE ABX */
            static void m6510_7f() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x02; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 4 RRA ABX */
            static void m6510_9f() { int tmp; m6502_ICount[0] -= 6; tmp = m6502.a & m6502.x; tmp &= (cpu_readop_arg((uint)(m6502.pc.wl + 1) & 0xffff) + 1); m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 5 SAH ABY */
            static void m6510_bf() { int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = m6502.x = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 LAX ABY */
            static void m6510_df() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 DCP ABX */
            static void m6510_ff() { int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((m6502.p & 0x08) != 0) { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0)); } else { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); cpu_writemem16((int)m6502.ea.d, (int)tmp); } /* 7 ISB ABX */

            static opcode[] insn6510 = {
 m6502_00,m6502_01,m6502_02,m6510_03,m6510_04,m6502_05,m6502_06,m6510_07,
 m6502_08,m6502_09,m6502_0a,m6510_0b,m6510_0c,m6502_0d,m6502_0e,m6510_0f,
 m6502_10,m6502_11,m6502_12,m6510_13,m6510_14,m6502_15,m6502_16,m6510_17,
 m6502_18,m6502_19,m6510_1a,m6510_1b,m6510_1c,m6502_1d,m6502_1e,m6510_1f,
 m6502_20,m6502_21,m6502_22,m6510_23,m6502_24,m6502_25,m6502_26,m6510_27,
 m6502_28,m6502_29,m6502_2a,m6510_2b,m6502_2c,m6502_2d,m6502_2e,m6510_2f,
 m6502_30,m6502_31,m6502_32,m6510_33,m6510_34,m6502_35,m6502_36,m6510_37,
 m6502_38,m6502_39,m6510_3a,m6510_3b,m6510_3c,m6502_3d,m6502_3e,m6510_3f,
 m6502_40,m6502_41,m6502_42,m6510_43,m6510_44,m6502_45,m6502_46,m6510_47,
 m6502_48,m6502_49,m6502_4a,m6510_4b,m6502_4c,m6502_4d,m6502_4e,m6510_4f,
 m6502_50,m6502_51,m6502_52,m6510_53,m6510_54,m6502_55,m6502_56,m6510_57,
 m6502_58,m6502_59,m6510_5a,m6510_5b,m6510_5c,m6502_5d,m6502_5e,m6510_5f,
 m6502_60,m6502_61,m6502_62,m6510_63,m6510_64,m6502_65,m6502_66,m6510_67,
 m6502_68,m6502_69,m6502_6a,m6510_6b,m6502_6c,m6502_6d,m6502_6e,m6510_6f,
 m6502_70,m6502_71,m6502_72,m6510_73,m6510_74,m6502_75,m6502_76,m6510_77,
 m6502_78,m6502_79,m6510_7a,m6510_7b,m6510_7c,m6502_7d,m6502_7e,m6510_7f,
 m6510_80,m6502_81,m6510_82,m6510_83,m6502_84,m6502_85,m6502_86,m6510_87,
 m6502_88,m6510_89,m6502_8a,m6510_8b,m6502_8c,m6502_8d,m6502_8e,m6510_8f,
 m6502_90,m6502_91,m6502_92,m6510_93,m6502_94,m6502_95,m6502_96,m6510_97,
 m6502_98,m6502_99,m6502_9a,m6510_9b,m6510_9c,m6502_9d,m6502_9e,m6510_9f,
 m6502_a0,m6502_a1,m6502_a2,m6510_a3,m6502_a4,m6502_a5,m6502_a6,m6510_a7,
 m6502_a8,m6502_a9,m6502_aa,m6510_ab,m6502_ac,m6502_ad,m6502_ae,m6510_af,
 m6502_b0,m6502_b1,m6502_b2,m6510_b3,m6502_b4,m6502_b5,m6502_b6,m6510_b7,
 m6502_b8,m6502_b9,m6502_ba,m6510_bb,m6502_bc,m6502_bd,m6502_be,m6510_bf,
 m6502_c0,m6502_c1,m6510_c2,m6510_c3,m6502_c4,m6502_c5,m6502_c6,m6510_c7,
 m6502_c8,m6502_c9,m6502_ca,m6510_cb,m6502_cc,m6502_cd,m6502_ce,m6510_cf,
 m6502_d0,m6502_d1,m6502_d2,m6510_d3,m6510_d4,m6502_d5,m6502_d6,m6510_d7,
 m6502_d8,m6502_d9,m6510_da,m6510_db,m6510_dc,m6502_dd,m6502_de,m6510_df,
 m6502_e0,m6502_e1,m6510_e2,m6510_e3,m6502_e4,m6502_e5,m6502_e6,m6510_e7,
 m6502_e8,m6502_e9,m6502_ea,m6510_eb,m6502_ec,m6502_ed,m6502_ee,m6510_ef,
 m6502_f0,m6502_f1,m6502_f2,m6510_f3,m6510_f4,m6502_f5,m6502_f6,m6510_f7,
 m6502_f8,m6502_f9,m6510_fa,m6510_fb,m6510_fc,m6502_fd,m6502_fe,m6510_ff
};
            /*****************************************************************************

             *****************************************************************************

             *

             *	 overrides for 2a03 opcodes

             *

             *****************************************************************************

             ********** insn   temp 	cycles			   rdmem   opc	wrmem	**********/
            static void n2a03_61() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 ADC IDX */



            static void n2a03_e1() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 SBC IDX */




            static void n2a03_71() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 ADC IDY */



            static void n2a03_f1() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 SBC IDY */
            static void n2a03_65() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ADC ZPG */



            static void n2a03_e5() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 SBC ZPG */




            static void n2a03_75() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ZPX */



            static void n2a03_f5() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg((uint)m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ZPX */
            static void n2a03_69() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ADC IMM */



            static void n2a03_e9() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg((uint)m6502.pc.wl++); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 SBC IMM */




            static void n2a03_79() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABY */



            static void n2a03_f9() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABY */
            static void n2a03_6d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABS */



            static void n2a03_ed() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABS */




            static void n2a03_7d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABX */



            static void n2a03_fd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.bh = (byte)cpu_readop_arg((uint)m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x02 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x02; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABX */
            static opcode[] insn2a03 = {
 m6502_00,m6502_01,m6502_02,m6502_03,m6502_04,m6502_05,m6502_06,m6502_07,
 m6502_08,m6502_09,m6502_0a,m6502_0b,m6502_0c,m6502_0d,m6502_0e,m6502_0f,
 m6502_10,m6502_11,m6502_12,m6502_13,m6502_14,m6502_15,m6502_16,m6502_17,
 m6502_18,m6502_19,m6502_1a,m6502_1b,m6502_1c,m6502_1d,m6502_1e,m6502_1f,
 m6502_20,m6502_21,m6502_22,m6502_23,m6502_24,m6502_25,m6502_26,m6502_27,
 m6502_28,m6502_29,m6502_2a,m6502_2b,m6502_2c,m6502_2d,m6502_2e,m6502_2f,
 m6502_30,m6502_31,m6502_32,m6502_33,m6502_34,m6502_35,m6502_36,m6502_37,
 m6502_38,m6502_39,m6502_3a,m6502_3b,m6502_3c,m6502_3d,m6502_3e,m6502_3f,
 m6502_40,m6502_41,m6502_42,m6502_43,m6502_44,m6502_45,m6502_46,m6502_47,
 m6502_48,m6502_49,m6502_4a,m6502_4b,m6502_4c,m6502_4d,m6502_4e,m6502_4f,
 m6502_50,m6502_51,m6502_52,m6502_53,m6502_54,m6502_55,m6502_56,m6502_57,
 m6502_58,m6502_59,m6502_5a,m6502_5b,m6502_5c,m6502_5d,m6502_5e,m6502_5f,
 m6502_60,n2a03_61,m6502_62,m6502_63,m6502_64,n2a03_65,m6502_66,m6502_67,
 m6502_68,n2a03_69,m6502_6a,m6502_6b,m6502_6c,n2a03_6d,m6502_6e,m6502_6f,
 m6502_70,n2a03_71,m6502_72,m6502_73,m6502_74,n2a03_75,m6502_76,m6502_77,
 m6502_78,n2a03_79,m6502_7a,m6502_7b,m6502_7c,n2a03_7d,m6502_7e,m6502_7f,
 m6502_80,m6502_81,m6502_82,m6502_83,m6502_84,m6502_85,m6502_86,m6502_87,
 m6502_88,m6502_89,m6502_8a,m6502_8b,m6502_8c,m6502_8d,m6502_8e,m6502_8f,
 m6502_90,m6502_91,m6502_92,m6502_93,m6502_94,m6502_95,m6502_96,m6502_97,
 m6502_98,m6502_99,m6502_9a,m6502_9b,m6502_9c,m6502_9d,m6502_9e,m6502_9f,
 m6502_a0,m6502_a1,m6502_a2,m6502_a3,m6502_a4,m6502_a5,m6502_a6,m6502_a7,
 m6502_a8,m6502_a9,m6502_aa,m6502_ab,m6502_ac,m6502_ad,m6502_ae,m6502_af,
 m6502_b0,m6502_b1,m6502_b2,m6502_b3,m6502_b4,m6502_b5,m6502_b6,m6502_b7,
 m6502_b8,m6502_b9,m6502_ba,m6502_bb,m6502_bc,m6502_bd,m6502_be,m6502_bf,
 m6502_c0,m6502_c1,m6502_c2,m6502_c3,m6502_c4,m6502_c5,m6502_c6,m6502_c7,
 m6502_c8,m6502_c9,m6502_ca,m6502_cb,m6502_cc,m6502_cd,m6502_ce,m6502_cf,
 m6502_d0,m6502_d1,m6502_d2,m6502_d3,m6502_d4,m6502_d5,m6502_d6,m6502_d7,
 m6502_d8,m6502_d9,m6502_da,m6502_db,m6502_dc,m6502_dd,m6502_de,m6502_df,
 m6502_e0,n2a03_e1,m6502_e2,m6502_e3,m6502_e4,n2a03_e5,m6502_e6,m6502_e7,
 m6502_e8,n2a03_e9,m6502_ea,m6502_eb,m6502_ec,n2a03_ed,m6502_ee,m6502_ef,
 m6502_f0,n2a03_f1,m6502_f2,m6502_f3,m6502_f4,n2a03_f5,m6502_f6,m6502_f7,
 m6502_f8,n2a03_f9,m6502_fa,m6502_fb,m6502_fc,n2a03_fd,m6502_fe,m6502_ff
};


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
                return m6502_info(context, regnum);
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
                return m6502_execute(cycles);
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new m6502_Regs();
            }
            public override uint get_pc()
            {
                return m6502_get_pc();
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
                m6502_reset(param);
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                m6502_set_irq_callback(callback);
            }
            public override void set_irq_line(int irqline, int linestate)
            {
                m6502_set_irq_line(irqline, linestate);
            }
            public override void set_nmi_line(int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_op_base(int pc)
            {
                cpu_setOPbase16(pc,0);
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
            void m6502_reset(object param)
            {
                m6502.subtype = 0;
                m6502.insn = insn6502;

                /* wipe out the rest of the m6502 structure */
                /* read the reset vector into PC */
                m6502.pc.bl = (byte)cpu_readmem16((int)0xfffc);
                m6502.pc.bh = (byte)cpu_readmem16((int)0xfffc + 1);

                m6502.sp.d = 0x01ff; /* stack pointer starts at page 1 offset FF */
                m6502.p = 0x20 | 0x04 | 0x02; /* set T, I and Z flags */
                m6502.pending_irq = 0; /* nonzero if an IRQ is pending */
                m6502.after_cli = 0; /* pending IRQ and last insn cleared I */
                m6502.irq_callback = null;

                change_pc16(m6502.pc.d);
            }

            void m6502_exit()
            {
                /* nothing to do yet */
            }

            uint m6502_get_context(ref object dst)
            {
                if (dst != null)
                    dst = m6502;
                return 1;
            }

            void m6502_set_context(object src)
            {
                if (src != null)
                {
                    m6502 = (m6502_Regs)src;
                    change_pc(m6502.pc.d);
                }
            }

            uint m6502_get_pc()
            {
                return m6502.pc.d;
            }

            void m6502_set_pc(uint val)
            {
                m6502.pc.wl = (ushort)val;
                change_pc(m6502.pc.d);
            }

            uint m6502_get_sp()
            {
                return m6502.sp.bl;
            }

            void m6502_set_sp(uint val)
            {
                m6502.sp.bl = (byte)val;
            }




            void take_irq()
            {
                if ((m6502.p & 0x04) == 0)
                {
                    m6502.ea.d = 0xfffe;
                    m6502_ICount[0] -= 7;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bh); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bl); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.p & ~0x10); m6502.sp.bl--;
                    m6502.p = (byte)((m6502.p & ~0x08) | 0x04); /* knock out D and set I flag */
                    m6502.pc.bl = (byte)cpu_readmem16((int)m6502.ea.d);
                    m6502.pc.bh = (byte)cpu_readmem16((int)m6502.ea.d + 1);

                    /* call back the cpuintrf to let it clear the line */
                    if (m6502.irq_callback != null) m6502.irq_callback(0);
                    change_pc16(m6502.pc.d);
                }
                m6502.pending_irq = 0;
            }

            int m6502_execute(int cycles)
            {
                m6502_ICount[0] = cycles;

                change_pc16(m6502.pc.d);

                do
                {
                    byte op;
                    m6502.ppc.d = m6502.pc.d;

                    op = cpu_readop((uint)m6502.pc.wl++);
                    /* if an irq is pending, take it now */
                    if (m6502.pending_irq != 0 && op == 0x78)
                        take_irq();

                    m6502.insn[op]();

                    /* check if the I flag was just reset (interrupts enabled) */
                    if (m6502.after_cli != 0)
                    {
                        m6502.after_cli = 0;
                        if (m6502.irq_state != CLEAR_LINE)
                        {
                            m6502.pending_irq = 1;
                        }
                        else
                        {
                            ;
                        }
                    }
                    else
                        if (m6502.pending_irq != 0)
                            take_irq();

                } while (m6502_ICount[0] > 0);

                return cycles - m6502_ICount[0];
            }

            void m6502_set_nmi_line(int state)
            {
                if (m6502.nmi_state == state) return;
                m6502.nmi_state = (byte)state;
                if (state != CLEAR_LINE)
                {
                    ;
                    m6502.ea.d = 0xfffa;
                    m6502_ICount[0] -= 7;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bh); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.pc.bl); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, (int)m6502.p & ~0x10); m6502.sp.bl--;
                    m6502.p = (byte)((m6502.p & ~0x08) | 0x04); /* knock out D and set I flag */
                    m6502.pc.bl = (byte)cpu_readmem16((int)m6502.ea.d);
                    m6502.pc.bh = (byte)cpu_readmem16((int)m6502.ea.d + 1);
                    ;
                    change_pc16(m6502.pc.d);
                }
            }

            void m6502_set_irq_line(int irqline, int state)
            {
                if (irqline == 3)
                {
                    if (m6502.so_state != 0 && state == 0)
                    {
                        ;
                        m6502.p |= 0x02;
                    }
                    m6502.so_state = (byte)state;
                    return;
                }
                m6502.irq_state = (byte)state;
                if (state != CLEAR_LINE)
                {
                    ;
                    m6502.pending_irq = 1;
                }
            }

            void m6502_set_irq_callback(irqcallback callback)
            {
                m6502.irq_callback = callback;
            }



            string m6502_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6502";
                    case CPU_INFO_FAMILY: return "Motorola 6502";
                    case CPU_INFO_VERSION: return "1.2";
                    case CPU_INFO_FILE: return "m6502.c";
                    case CPU_INFO_CREDITS: return "Copyright (c) 1998 Juergen Buchmueller, all rights reserved.";
                }
                throw new Exception();
            }



            void m65c02_reset(object param)
            {
                m6502_reset(param);
                m6502.subtype = 1;
                m6502.insn = insn65c02;
            }
            void m65c02_exit() { m6502_exit(); }
            int m65c02_execute(int cycles) { return m6502_execute(cycles); }
            uint m65c02_get_context(ref object dst) { return m6502_get_context(ref dst); }
            void m65c02_set_context(object src) { m6502_set_context(src); }
            uint m65c02_get_pc() { return m6502_get_pc(); }
            void m65c02_set_pc(uint val) { m6502_set_pc(val); }
            uint m65c02_get_sp() { return m6502_get_sp(); }
            void m65c02_set_sp(uint val) { m6502_set_sp(val); }
            void m65c02_set_nmi_line(int state) { m6502_set_nmi_line(state); }
            void m65c02_set_irq_line(int irqline, int state) { m6502_set_irq_line(irqline, state); }
            void m65c02_set_irq_callback(irqcallback callback) { m6502_set_irq_callback(callback); }
            string m65c02_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M65C02";
                    case CPU_INFO_VERSION: return "1.2";
                }
                return m6502_info(context, regnum);
            }






            void m65sc02_reset(object param)
            {
                m6502_reset(param);
                m6502.subtype = 4;
                m6502.insn = insn65sc02;
            }
            void m65sc02_exit() { m6502_exit(); }
            int m65sc02_execute(int cycles) { return m6502_execute(cycles); }
            uint m65sc02_get_context(ref object dst) { return m6502_get_context(ref dst); }
            void m65sc02_set_context(object src) { m6502_set_context(src); }
            uint m65sc02_get_pc() { return m6502_get_pc(); }
            void m65sc02_set_pc(uint val) { m6502_set_pc(val); }
            uint m65sc02_get_sp() { return m6502_get_sp(); }
            void m65sc02_set_sp(uint val) { m6502_set_sp(val); }
            void m65sc02_set_nmi_line(int state) { m6502_set_nmi_line(state); }
            void m65sc02_set_irq_line(int irqline, int state) { m6502_set_irq_line(irqline, state); }
            void m65sc02_set_irq_callback(irqcallback callback) { m6502_set_irq_callback(callback); }
            string m65sc02_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M65SC02";
                    case CPU_INFO_FAMILY: return "Metal Oxid Semiconductor MOS 6502";
                    case CPU_INFO_VERSION: return "1.0beta";
                    case CPU_INFO_CREDITS:
                        return "Copyright (c) 1998 Juergen Buchmueller\n" +
                         "Copyright (c) 2000 Peter Trauner\n" +
                         "all rights reserved.";
                }
                return m6502_info(context, regnum);
            }






            void m6510_reset(object param)
            {
                m6502_reset(param);
                m6502.subtype = 2;
                m6502.insn = insn6510;
            }
            void m6510_exit() { m6502_exit(); }
            int m6510_execute(int cycles) { return m6502_execute(cycles); }
            uint m6510_get_context(ref object dst) { return m6502_get_context(ref dst); }
            void m6510_set_context(object src) { m6502_set_context(src); }
            uint m6510_get_pc() { return m6502_get_pc(); }
            void m6510_set_pc(uint val) { m6502_set_pc(val); }
            uint m6510_get_sp() { return m6502_get_sp(); }
            void m6510_set_sp(uint val) { m6502_set_sp(val); }
            void m6510_set_nmi_line(int state) { m6502_set_nmi_line(state); }
            void m6510_set_irq_line(int irqline, int state) { m6502_set_irq_line(irqline, state); }
            void m6510_set_irq_callback(irqcallback callback) { m6502_set_irq_callback(callback); }
            string m6510_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6510";
                    case CPU_INFO_VERSION: return "1.2";
                }
                return m6502_info(context, regnum);
            }





            void n2a03_reset(object param)
            {
                m6502_reset(param);
                m6502.subtype = 3;
                m6502.insn = insn2a03;
            }
            void n2a03_exit() { m6502_exit(); }
            int n2a03_execute(int cycles) { return m6502_execute(cycles); }
            uint n2a03_get_context(ref object dst) { return m6502_get_context(ref dst); }
            void n2a03_set_context(object src) { m6502_set_context(src); }
            uint n2a03_get_pc() { return m6502_get_pc(); }
            void n2a03_set_pc(uint val) { m6502_set_pc(val); }
            uint n2a03_get_sp() { return m6502_get_sp(); }
            void n2a03_set_sp(uint val) { m6502_set_sp(val); }
            void n2a03_set_nmi_line(int state) { m6502_set_nmi_line(state); }
            void n2a03_set_irq_line(int irqline, int state) { m6502_set_irq_line(irqline, state); }
            void n2a03_set_irq_callback(irqcallback callback) { m6502_set_irq_callback(callback); }
            string n2a03_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "N2A03";
                    case CPU_INFO_VERSION: return "1.0";
                }
                return m6502_info(context, regnum);
            }

            /* The N2A03 is integrally tied to its PSG (they're on the same die).

               Bit 7 of address $4011 (the PSG's DPCM control register), when set,

               causes an IRQ to be generated.  This function allows the IRQ to be called

               from the PSG core when such an occasion arises. */
            void n2a03_irq()
            {
                take_irq();
            }



        }
    }
}
