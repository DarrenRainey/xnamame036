using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_m6502 : cpu_interface
        {
            public const byte M6502_INT_NONE = 0;
            public const byte M6502_INT_IRQ = 1;
            public const byte M6502_INT_NMI = 2;

            public const byte M6502_SET_OVERFLOW = 3;
            public const ushort M6502_NMI_VEC = 0xfffa;
            public const ushort M6502_RST_VEC = 0xfffc;
            public const ushort M6502_IRQ_VEC = 0xfffe;
            public static int[] m6502_ICount = new int[1];
            public delegate void opcode();
            public class m6502_Regs
            {
                public byte subtype;
                public opcode[] insn;
                public PAIR ppc;
                public PAIR pc;
                public PAIR sp;
                public PAIR zp;
                public PAIR ea;
                public byte a, x, y, p;
                public byte pending_irq;
                public byte after_cli;
                public byte nmi_state;
                public byte irq_state;
                public byte so_state;
                public irqcallback irq_callback;
            }
            public static m6502_Regs m6502 = new m6502_Regs();

            public cpu_m6502()
            {
                cpu_num = CPU_M6502;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6502_INT_NONE;
                irq_int = M6502_INT_IRQ;
                nmi_int = M6502_INT_NMI;
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
            public override void burn(int cycles)
            {
                //nothing
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6502";
                    case CPU_INFO_FAMILY: return "Motorola 6502";
                    case CPU_INFO_VERSION: return "1.2";
                    case CPU_INFO_FILE: return "cpu_m6502.cs";
                    case CPU_INFO_CREDITS: return "Copyright (c) 1998 Juergen Buchmueller, all rights reserved.";
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
            public static void take_irq()
            {
                if ((m6502.p & 0x04) == 0)
                {
                    m6502.ea.d = 0xfffe;
                    m6502_ICount[0] -= 7;
                    cpu_writemem16((int)m6502.sp.d, m6502.pc.bh); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, m6502.pc.bl); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, m6502.p & ~0x10); m6502.sp.bl--;
                    m6502.p = (byte)((m6502.p & ~0x08) | 0x04); /* knock out D and set I flag */
                    m6502.pc.bl = (byte)cpu_readmem16((int)m6502.ea.d);
                    m6502.pc.bh = (byte)cpu_readmem16((int)m6502.ea.d + 1);
                    ;
                    /* call back the cpuintrf to let it clear the line */
                    if (m6502.irq_callback != null) m6502.irq_callback(0);
                    change_pc16(m6502.pc.d);
                }
                m6502.pending_irq = 0;
            }
            public override int execute(int cycles)
            {
                m6502_ICount[0] = cycles;

                change_pc16(m6502.pc.d);

                do
                {
                    byte op;
                    m6502.ppc.d = m6502.pc.d;

                    op = cpu_readop(m6502.pc.wl++);
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
            public override uint get_context(ref object reg)
            {
                reg = m6502;
                return 1;
            }
            public override void create_context(ref object reg)
            {
                reg = new m6502_Regs();
            }
            public override uint get_pc()
            {
                return m6502.pc.d;
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
                m6502.subtype = 0;
                m6502.insn = insn6502;

                /* wipe out the rest of the m6502 structure */
                /* read the reset vector into PC */
                m6502.pc.bl = (byte)cpu_readmem16(0xfffc);
                m6502.pc.bh = (byte)cpu_readmem16(0xfffc + 1);

                m6502.sp.d = 0x01ff; /* stack pointer starts at page 1 offset FF */
                m6502.p = 0x20 | 0x04 | 0x02; /* set T, I and Z flags */
                m6502.pending_irq = 0; /* nonzero if an IRQ is pending */
                m6502.after_cli = 0; /* pending IRQ and last insn cleared I */
                m6502.irq_callback = null;

                change_pc16(m6502.pc.d);
            }
            public override void set_context(object reg)
            {
                m6502 = (m6502_Regs)reg;
                change_pc(m6502.pc.d);

            }
            public override void set_irq_callback(irqcallback callback)
            {
                m6502.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                if (irqline == M6502_SET_OVERFLOW)
                {
                    if (m6502.so_state != 0 && state == 0)
                    {
                        //LOG((errorlog, "M6502#%d set overflow\n", cpu_getactivecpu()));
                        m6502.p |= 0x40;
                    }
                    m6502.so_state = (byte)state;
                    return;
                }
                m6502.irq_state = (byte)state;
                if (state != CLEAR_LINE)
                {
                    //LOG((errorlog, "M6502#%d set_irq_line(ASSERT)\n", cpu_getactivecpu()));
                    m6502.pending_irq = 1;
                }
            }
            public override void set_nmi_line(int state)
            {
                if (m6502.nmi_state == state) return;
                m6502.nmi_state = (byte)state;
                if (state != CLEAR_LINE)
                {
                    m6502.ea.d = 0xfffa;
                    m6502_ICount[0] -= 7;
                    cpu_writemem16((int)m6502.sp.d, m6502.pc.bh); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, m6502.pc.bl); m6502.sp.bl--;
                    cpu_writemem16((int)m6502.sp.d, m6502.p & ~0x10); m6502.sp.bl--;
                    m6502.p = (byte)((m6502.p & ~0x08) | 0x04); /* knock out D and set I flag */
                    m6502.pc.bl = (byte)cpu_readmem16((int)m6502.ea.d);
                    m6502.pc.bh = (byte)cpu_readmem16((int)m6502.ea.d + 1);

                    change_pc16(m6502.pc.d);
                }
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

            public static void m6502_00()
            {
                m6502_ICount[0] -= 7; m6502.pc.wl++; cpu_writemem16((int)m6502.sp.d, m6502.pc.bh); m6502.sp.bl--;
                cpu_writemem16((int)m6502.sp.d, m6502.pc.bl); m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, m6502.p | 0x10); m6502.sp.bl--;
                m6502.p = (byte)((m6502.p | 0x04) & ~0x08); m6502.pc.bl = (byte)cpu_readmem16(0xfffe); m6502.pc.bh = (byte)cpu_readmem16(0xfffe + 1); change_pc16(m6502.pc.d);
            } /* 7 BRK */
            public static void m6502_20()
            {
                m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); cpu_writemem16((int)m6502.sp.d, m6502.pc.bh);
                m6502.sp.bl--; cpu_writemem16((int)m6502.sp.d, m6502.pc.bl); m6502.sp.bl--; m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
            } /* 6 JSR */
            public static void m6502_40()
            {
                m6502_ICount[0] -= 6; m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d); m6502.sp.bl++; m6502.pc.bl = (byte)cpu_readmem16((int)m6502.sp.d);
                m6502.sp.bl++; m6502.pc.bh = (byte)cpu_readmem16((int)m6502.sp.d); m6502.p |= 0x20;
                if ((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) == 0) { ; m6502.after_cli = 1; } change_pc16(m6502.pc.d);
            } /* 6 RTI */
            public static void m6502_60()
            {
                m6502_ICount[0] -= 6; m6502.sp.bl++; m6502.pc.bl = (byte)cpu_readmem16((int)m6502.sp.d); m6502.sp.bl++;
                m6502.pc.bh = (byte)cpu_readmem16((int)m6502.sp.d); m6502.pc.wl++; change_pc16(m6502.pc.d);
            } /* 6 RTS */
            public static void m6502_80() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_a0()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.y = (byte)tmp; if ((m6502.y) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 2 LDY IMM */
            public static void m6502_c0()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01;
                if (((byte)(m6502.y - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80));
            } /* 2 CPY IMM */
            public static void m6502_e0()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01;
                if (((byte)(m6502.x - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80));
            } /* 2 CPX IMM */

            public static void m6502_10()
            {
                int tmp; if ((m6502.p & 0x80) == 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BPL REL */
            public static void m6502_30()
            {
                int tmp; if ((m6502.p & 0x80) != 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BMI REL */
            public static void m6502_50()
            {
                int tmp; if ((m6502.p & 0x40) == 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BVC REL */
            public static void m6502_70()
            {
                int tmp; if ((m6502.p & 0x40) != 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BVS REL */
            public static void m6502_90()
            {
                int tmp; if ((m6502.p & 0x01) == 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BCC REL */
            public static void m6502_b0()
            {
                int tmp; if ((m6502.p & 0x01) != 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BCS REL */
            public static void m6502_d0()
            {
                int tmp; if ((m6502.p & 0x02) == 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BNE REL */
            public static void m6502_f0()
            {
                int tmp; if ((m6502.p & 0x02) != 0)
                {
                    tmp = cpu_readop_arg(m6502.pc.wl++);
                    m6502.ea.wl = (ushort)(m6502.pc.wl + (sbyte)tmp); m6502_ICount[0] -= (m6502.pc.bh == m6502.ea.bh) ? 3 : 4; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
                }
                else
                { m6502.pc.wl++; m6502_ICount[0] -= 2; };
            } /* 2 BEQ REL */

            public static void m6502_01()
            {
                int tmp; m6502_ICount[0] -= 6;
                m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)(cpu_readmem16((int)m6502.zp.d)); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 ORA IDX */
            public static void m6502_21()
            {
                int tmp; m6502_ICount[0] -= 6;
                m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)(cpu_readmem16((int)m6502.zp.d)); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 AND IDX */
            public static void m6502_41()
            {
                int tmp; m6502_ICount[0] -= 6;
                m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)(cpu_readmem16((int)m6502.zp.d)); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 EOR IDX */
            public static void m6502_61()
            {
                int tmp; m6502_ICount[0] -= 6;
                m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)(cpu_readmem16((int)m6502.zp.d)); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01);
                    int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40;
                    if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 ADC IDX */
            public static void m6502_81()
            {
                int tmp; m6502_ICount[0] -= 6; tmp = m6502.a;
                m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d);
                m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 STA IDX */
            public static void m6502_a1()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 LDA IDX */
            public static void m6502_c1()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01;
                if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 6 CMP IDX */
            public static void m6502_e1()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d);
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01;
                    int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c;
                    int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40;
                    if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 6 SBC IDX */

            public static void m6502_11()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 ORA IDY */
            public static void m6502_31()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 AND IDY */
            public static void m6502_51()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 EOR IDY */
            public static void m6502_71()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 ADC IDY */
            public static void m6502_91()
            {
                int tmp; m6502_ICount[0] -= 6; tmp = m6502.a; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d);
                m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y;
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 STA IDY */
            public static void m6502_b1()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 LDA IDY */
            public static void m6502_d1()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 5 CMP IDY */
            public static void m6502_f1()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++;
                m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = (byte)cpu_readmem16((int)m6502.ea.d);
                if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0);
                    m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10;
                    if ((hi & 0x0f00) != 0)
                        hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c;
                    m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 5 SBC IDY */

            public static void m6502_02() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_22() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_42() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_62() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_82() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_a2()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 2 LDX IMM */
            public static void m6502_c2() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_e2() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_12() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_32() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_52() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_72() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_92() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_b2() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_d2() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_f2() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_03() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_23() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_43() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_63() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_83() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_a3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_c3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_e3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_13() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_33() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_53() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_73() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_93() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_b3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_d3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_f3() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_04() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_24()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p &= unchecked((byte)~(0x80 | 0x40 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x40)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02;
            } /* 3 BIT ZPG */
            public static void m6502_44() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_64() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_84() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.y; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, tmp); } /* 3 STY ZPG */
            public static void m6502_a4()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 3 LDY ZPG */
            public static void m6502_c4()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01; if (((byte)(m6502.y - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80));
            } /* 3 CPY ZPG */
            public static void m6502_e4()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01; if (((byte)(m6502.x - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80));
            } /* 3 CPX ZPG */

            public static void m6502_14() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_34() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_54() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_74() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_94()
            {
                int tmp; m6502_ICount[0] -= 4; tmp = m6502.y; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 4 STY ZPX */
            public static void m6502_b4()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 4 LDY ZPX */
            public static void m6502_d4() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_f4() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_05()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 ORA ZPG */
            public static void m6502_25()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = (byte)cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 AND ZPG */
            public static void m6502_45()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 EOR ZPG */
            public static void m6502_65()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c;
                    int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; }
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 ADC ZPG */
            public static void m6502_85()
            {
                int tmp; m6502_ICount[0] -= 3; tmp = m6502.a; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 3 STA ZPG */
            public static void m6502_a5()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 LDA ZPG */
            public static void m6502_c5()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 3 CMP ZPG */
            public static void m6502_e5()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c;
                    int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10;
                    if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 3 SBC ZPG */

            public static void m6502_15()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ORA ZPX */
            public static void m6502_35()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 AND ZPX */
            public static void m6502_55()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 EOR ZPX */
            public static void m6502_75()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c;
                    int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; }
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ADC ZPX */
            public static void m6502_95()
            {
                int tmp; m6502_ICount[0] -= 4; tmp = m6502.a; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x);
                m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 4 STA ZPX */
            public static void m6502_b5()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 LDA ZPX */
            public static void m6502_d5()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 4 CMP ZPX */
            public static void m6502_f5()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c;
                    int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10;
                    if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 SBC ZPX */

            public static void m6502_06()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 ASL ZPG */
            public static void m6502_26()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 ROL ZPG */
            public static void m6502_46()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 LSR ZPG */
            public static void m6502_66()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 ROR ZPG */
            public static void m6502_86() { int tmp; m6502_ICount[0] -= 3; tmp = m6502.x; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; cpu_writemem16((int)m6502.ea.d, tmp); } /* 3 STX ZPG */
            public static void m6502_a6()
            {
                int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 3 LDX ZPG */
            public static void m6502_c6()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 DEC ZPG */
            public static void m6502_e6()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 INC ZPG */

            public static void m6502_16()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ASL ZPX */
            public static void m6502_36()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ROL ZPX */
            public static void m6502_56()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 LSR ZPX */
            public static void m6502_76()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ROR ZPX */
            public static void m6502_96()
            {
                int tmp; m6502_ICount[0] -= 4; tmp = m6502.x; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.y); m6502.ea.d = m6502.zp.d;
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 4 STX ZPY */
            public static void m6502_b6()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.y); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 4 LDX ZPY */
            public static void m6502_d6()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 DEC ZPX */
            public static void m6502_f6()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d);
                tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 INC ZPX */

            public static void m6502_07() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_27() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_47() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_67() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_87() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_a7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_c7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_e7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_17() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_37() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_57() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_77() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_97() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_b7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_d7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_f7() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_08() { m6502_ICount[0] -= 2; cpu_writemem16((int)m6502.sp.d, m6502.p); m6502.sp.bl--; } /* 2 PHP */
            public static void m6502_28()
            {
                m6502_ICount[0] -= 2; if ((m6502.p & 0x04) != 0)
                {
                    m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d);
                    if ((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) == 0) { ; m6502.after_cli = 1; }
                }
                else { m6502.sp.bl++; m6502.p = (byte)cpu_readmem16((int)m6502.sp.d); } m6502.p |= 0x20;
            } /* 2 PLP */
            public static void m6502_48() { m6502_ICount[0] -= 2; cpu_writemem16((int)m6502.sp.d, m6502.a); m6502.sp.bl--; } /* 2 PHA */
            public static void m6502_68()
            {
                m6502_ICount[0] -= 2; m6502.sp.bl++; m6502.a = (byte)cpu_readmem16((int)m6502.sp.d); if ((m6502.a) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 PLA */
            public static void m6502_88()
            {
                m6502_ICount[0] -= 2; m6502.y = (byte)--m6502.y; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 2 DEY */
            public static void m6502_a8()
            {
                m6502_ICount[0] -= 2; m6502.y = m6502.a; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 2 TAY */
            public static void m6502_c8()
            {
                m6502_ICount[0] -= 2; m6502.y = (byte)++m6502.y; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 2 INY */
            public static void m6502_e8()
            {
                m6502_ICount[0] -= 2; m6502.x = (byte)++m6502.x; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 2 INX */

            public static void m6502_18() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x01); } /* 2 CLC */
            public static void m6502_38() { m6502_ICount[0] -= 2; m6502.p |= 0x01; } /* 2 SEC */
            public static void m6502_58() { m6502_ICount[0] -= 2; if (((m6502.irq_state != CLEAR_LINE) && (m6502.p & 0x04) != 0)) { ; m6502.after_cli = 1; } m6502.p &= unchecked((byte)~0x04); } /* 2 CLI */
            public static void m6502_78() { m6502_ICount[0] -= 2; m6502.p |= 0x04; } /* 2 SEI */
            public static void m6502_98()
            {
                m6502_ICount[0] -= 2; m6502.a = m6502.y; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 TYA */
            public static void m6502_b8() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x40); } /* 2 CLV */
            public static void m6502_d8() { m6502_ICount[0] -= 2; m6502.p &= unchecked((byte)~0x08); } /* 2 CLD */
            public static void m6502_f8() { m6502_ICount[0] -= 2; m6502.p |= 0x08; } /* 2 SED */

            public static void m6502_09()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.a = (byte)(m6502.a | tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 ORA IMM */
            public static void m6502_29()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.a = (byte)(m6502.a & tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 AND IMM */
            public static void m6502_49()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.a = (byte)(m6502.a ^ tmp);
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 EOR IMM */
            public static void m6502_69()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c; int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (lo > 0x09) { hi += 0x10; lo += 0x06; } if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60;
                    if ((hi & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c;
                    m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 ADC IMM */
            public static void m6502_89() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_a9()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.a = (byte)tmp;
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 LDA IMM */
            public static void m6502_c9()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); m6502.p &= unchecked((byte)~0x01);
                if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 2 CMP IMM */
            public static void m6502_e9()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c;
                    int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60;
                    if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01;
                    int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40;
                    if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 SBC IMM */

            public static void m6502_19()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ORA ABY */
            public static void m6502_39()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 AND ABY */
            public static void m6502_59()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 EOR ABY */
            public static void m6502_79()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c;
                    int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; }
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ADC ABY */
            public static void m6502_99()
            {
                int tmp; m6502_ICount[0] -= 5; tmp = m6502.a; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.wl += m6502.y; cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 STA ABY */
            public static void m6502_b9()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 LDA ABY */
            public static void m6502_d9()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 4 CMP ABY */
            public static void m6502_f9()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c;
                    int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6;
                    if ((lo & 0x80) != 0) hi -= 0x10; if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 SBC ABY */

            public static void m6502_0a()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1);
                if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ;
            } /* 2 ASL A */
            public static void m6502_2a()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01));
                tmp = (byte)tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ;
            } /* 2 ROL A */
            public static void m6502_4a()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1;
                if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ;
            } /* 2 LSR A */
            public static void m6502_6a()
            {
                int tmp; m6502_ICount[0] -= 2; tmp = m6502.a; tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01));
                tmp = (byte)(tmp >> 1); if ((tmp) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); m6502.a = (byte)tmp; ;
            } /* 2 ROR A */
            public static void m6502_8a()
            {
                m6502_ICount[0] -= 2; m6502.a = m6502.x; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 2 TXA */
            public static void m6502_aa()
            {
                m6502_ICount[0] -= 2; m6502.x = m6502.a; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 2 TAX */
            public static void m6502_ca()
            {
                m6502_ICount[0] -= 2; m6502.x = (byte)--m6502.x; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 2 DEX */
            public static void m6502_ea() { m6502_ICount[0] -= 2; ; } /* 2 NOP */

            public static void m6502_1a() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_3a() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_5a() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_7a() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_9a() { m6502_ICount[0] -= 2; m6502.sp.bl = m6502.x; } /* 2 TXS */
            public static void m6502_ba()
            {
                m6502_ICount[0] -= 2; m6502.x = m6502.sp.bl; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 2 TSX */
            public static void m6502_da() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_fa() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_0b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_2b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_4b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_6b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_8b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_ab() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_cb() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_eb() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_1b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_3b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_5b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_7b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_9b() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_bb() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_db() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_fb() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_0c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_2c()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~(0x80 | 0x40 | 0x02)); m6502.p |= (byte)(tmp & (0x80 | 0x40)); if ((tmp & m6502.a) == 0) m6502.p |= 0x02;
            } /* 4 BIT ABS */
            public static void m6502_4c()
            {
                m6502_ICount[0] -= 3; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                if (m6502.ea.d == m6502.ppc.d && m6502.pending_irq == 0 && m6502.after_cli == 0) if (m6502_ICount[0] > 0) m6502_ICount[0] = 0; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
            } /* 3 JMP ABS */
            public static void m6502_6c()
            {
                int tmp; m6502_ICount[0] -= 5; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.ea.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.ea.d); m6502.ea.bl = (byte)tmp;
                if (m6502.ea.d == m6502.ppc.d && m6502.pending_irq == 0 && m6502.after_cli == 0) if (m6502_ICount[0] > 0) m6502_ICount[0] = 0; m6502.pc.d = m6502.ea.d; change_pc16(m6502.pc.d);
            } /* 5 JMP IND */
            public static void m6502_8c()
            {
                int tmp; m6502_ICount[0] -= 4; tmp = m6502.y; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 4 STY ABS */
            public static void m6502_ac()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 4 LDY ABS */
            public static void m6502_cc()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.y >= tmp) m6502.p |= 0x01; if (((byte)(m6502.y - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.y - tmp)) & 0x80));
            } /* 4 CPY ABS */
            public static void m6502_ec()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.x >= tmp) m6502.p |= 0x01; if (((byte)(m6502.x - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.x - tmp)) & 0x80));
            } /* 4 CPX ABS */

            public static void m6502_1c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_3c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_5c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_7c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_9c() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_bc()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.y = (byte)tmp; if ((m6502.y) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.y) & 0x80));
            } /* 4 LDY ABX */
            public static void m6502_dc() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_fc() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_0d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ORA ABS */
            public static void m6502_2d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 AND ABS */
            public static void m6502_4d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 EOR ABS */
            public static void m6502_6d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c;
                    int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; }
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ADC ABS */
            public static void m6502_8d()
            {
                int tmp; m6502_ICount[0] -= 4; tmp = m6502.a; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 4 STA ABS */
            public static void m6502_ad()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 LDA ABS */
            public static void m6502_cd()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 4 CMP ABS */
            public static void m6502_ed()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c;
                    int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10;
                    if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 SBC ABS */

            public static void m6502_1d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a | tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ORA ABX */
            public static void m6502_3d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a & tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 AND ABX */
            public static void m6502_5d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)(m6502.a ^ tmp); if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 EOR ABX */
            public static void m6502_7d()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01); int lo = (m6502.a & 0x0f) + (tmp & 0x0f) + c;
                    int hi = (m6502.a & 0xf0) + (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (lo > 0x09) { hi += 0x10; lo += 0x06; }
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ hi) & 0x80) != 0) m6502.p |= 0x40; if (hi > 0x90) hi += 0x60; if ((hi & 0xff00) != 0) m6502.p |= 0x01;
                    m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((sum & 0xff00) != 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                }
                if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 ADC ABX */
            public static void m6502_9d()
            {
                int tmp; m6502_ICount[0] -= 5; tmp = m6502.a; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x; cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 STA ABX */
            public static void m6502_bd()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.a = (byte)tmp; if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 LDA ABX */
            public static void m6502_dd()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p &= unchecked((byte)~0x01); if (m6502.a >= tmp) m6502.p |= 0x01; if (((byte)(m6502.a - tmp)) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | (((byte)(m6502.a - tmp)) & 0x80));
            } /* 4 CMP ABX */
            public static void m6502_fd()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++);
                m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); if ((m6502.p & 0x08) != 0)
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c;
                    int lo = (m6502.a & 0x0f) - (tmp & 0x0f) - c; int hi = (m6502.a & 0xf0) - (tmp & 0xf0); m6502.p &= unchecked((byte)~(0x40 | 0x01));
                    if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0) m6502.p |= 0x40; if ((lo & 0xf0) != 0) lo -= 6; if ((lo & 0x80) != 0) hi -= 0x10;
                    if ((hi & 0x0f00) != 0) hi -= 0x60; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)((lo & 0x0f) + (hi & 0xf0));
                }
                else
                {
                    int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80) != 0)
                        m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum;
                } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80));
            } /* 4 SBC ABX */

            public static void m6502_0e()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d);
                m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ASL ABS */
            public static void m6502_2e()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ROL ABS */
            public static void m6502_4e()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 LSR ABS */
            public static void m6502_6e()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 ROR ABS */
            public static void m6502_8e()
            {
                int tmp; m6502_ICount[0] -= 5; tmp = m6502.x; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 5 STX ABS */
            public static void m6502_ae()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 4 LDX ABS */
            public static void m6502_ce()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 DEC ABS */
            public static void m6502_ee()
            {
                int tmp; m6502_ICount[0] -= 6; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++);
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 6 INC ABS */

            public static void m6502_1e()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 7) & 0x01)); tmp = (byte)(tmp << 1); if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 ASL ABX */
            public static void m6502_3e()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (tmp << 1) | (m6502.p & 0x01); m6502.p = (byte)((m6502.p & ~0x01) | ((tmp >> 8) & 0x01)); tmp = (byte)tmp; if ((tmp) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 ROL ABX */
            public static void m6502_5e()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)tmp >> 1; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 LSR ABX */
            public static void m6502_7e()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); tmp |= (m6502.p & 0x01) << 8; m6502.p = (byte)((m6502.p & ~0x01) | (tmp & 0x01)); tmp = (byte)(tmp >> 1); if ((tmp) == 0)
                    m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80)); cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 ROR ABX */
            public static void m6502_9e() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_be()
            {
                int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y;
                tmp = cpu_readmem16((int)m6502.ea.d); m6502.x = (byte)tmp; if ((m6502.x) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.x) & 0x80));
            } /* 4 LDX ABY */
            public static void m6502_de()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)--tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 DEC ABX */
            public static void m6502_fe()
            {
                int tmp; m6502_ICount[0] -= 7; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x;
                tmp = cpu_readmem16((int)m6502.ea.d); tmp = (byte)++tmp; if ((tmp) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02);
                else
                    m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((tmp) & 0x80));
                cpu_writemem16((int)m6502.ea.d, tmp);
            } /* 7 INC ABX */

            public static void m6502_0f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_2f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_4f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_6f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_8f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_af() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_cf() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_ef() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

            public static void m6502_1f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_3f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_5f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_7f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_9f() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_bf() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_df() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */
            public static void m6502_ff() { m6502_ICount[0] -= 2; Mame.printf("M6502 illegal opcode %04x: %02x\n", (m6502.pc.wl - 1) & 0xffff, cpu_readop((uint)((m6502.pc.wl - 1) & 0xffff))); } /* 2 ILL */

        }
        public class cpu_n2a03 : cpu_m6502
        {
            public const double N2A03_DEFAULTCLOCK = (21477272.724 / 12);

            public cpu_n2a03()
            {
                cpu_num = CPU_N2A03;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6502_INT_NONE;
                irq_int = M6502_INT_IRQ;
                nmi_int = M6502_INT_NMI;
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
            public override void reset(object param)
            {
                base.reset(param);
                m6502.subtype = 3;
                m6502.insn = insn2a03;
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "N2A03";
                    case CPU_INFO_VERSION: return "1.0";
                }
                return base.cpu_info(context, regnum);
            }
            static void n2a03_61() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 ADC IDX */
            static void n2a03_e1() { int tmp; m6502_ICount[0] -= 6; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 6 SBC IDX */
            static void n2a03_71() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 ADC IDY */
            static void n2a03_f1() { int tmp; m6502_ICount[0] -= 5; m6502.zp.bl = (byte)cpu_readop_arg(m6502.pc.wl++); m6502.ea.bl = (byte)cpu_readmem16((int)m6502.zp.d); m6502.zp.bl++; m6502.ea.bh = (byte)cpu_readmem16((int)m6502.zp.d); if (m6502.ea.bl + m6502.y > 0xff) m6502_ICount[0]--; m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 5 SBC IDY */
            static void n2a03_65() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &=unchecked((byte) ~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 ADC ZPG */
            static void n2a03_e5() { int tmp; m6502_ICount[0] -= 3; m6502.zp.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 3 SBC ZPG */
            static void n2a03_75() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) !=0)m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ZPX */
            static void n2a03_f5() { int tmp; m6502_ICount[0] -= 4; m6502.zp.bl = (byte)(cpu_readop_arg(m6502.pc.wl++) + m6502.x); m6502.ea.d = m6502.zp.d; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ZPX */
            static void n2a03_69() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 ADC IMM */
            static void n2a03_e9() { int tmp; m6502_ICount[0] -= 2; tmp = cpu_readop_arg(m6502.pc.wl++); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 2 SBC IMM */
            static void n2a03_79() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABY */
            static void n2a03_f9() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.y; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if( ((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p = (byte)((m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABY */
            static void n2a03_6d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABS */
            static void n2a03_ed() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p = (byte)((m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABS */
            static void n2a03_7d() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01); int sum = m6502.a + tmp + c; m6502.p &=unchecked((byte) ~(0x40 | 0x01)); if ((~(m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00)!=0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 ADC ABX */
            static void n2a03_fd() { int tmp; m6502_ICount[0] -= 4; m6502.ea.bl = cpu_readop_arg(m6502.pc.wl++); m6502.ea.bh = cpu_readop_arg(m6502.pc.wl++); m6502.ea.wl += m6502.x; tmp = cpu_readmem16((int)m6502.ea.d); { int c = (m6502.p & 0x01) ^ 0x01; int sum = m6502.a - tmp - c; m6502.p &= unchecked((byte)~(0x40 | 0x01)); if (((m6502.a ^ tmp) & (m6502.a ^ sum) & 0x80)!=0) m6502.p |= 0x40; if ((sum & 0xff00) == 0) m6502.p |= 0x01; m6502.a = (byte)sum; } if ((m6502.a) == 0) m6502.p =(byte)( (m6502.p & ~0x80) | 0x02); else m6502.p =(byte)( (m6502.p & ~(0x80 | 0x02)) | ((m6502.a) & 0x80)); } /* 4 SBC ABX */

            public static void n2a03_irq()
{
  take_irq();
}


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
        }

    }
}