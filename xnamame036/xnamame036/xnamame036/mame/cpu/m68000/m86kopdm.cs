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
            static void m68000_dbt()
            {
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_dbf()
            {
                UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint res = ((d_reg[0] - 1) & 0xffff);

                d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                if (res != 0xffff)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_dbhi()
            {
                if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbls()
            {
                if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbcc()
            {
                if ((m68k_cpu.c_flag != 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbcs()
            {
                if ((m68k_cpu.c_flag == 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbne()
            {
                if ((m68k_cpu.not_z_flag == 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbeq()
            {
                if ((m68k_cpu.not_z_flag != 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbvc()
            {
                if ((m68k_cpu.v_flag != 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbvs()
            {
                if ((m68k_cpu.v_flag == 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbpl()
            {
                if ((m68k_cpu.n_flag != 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbmi()
            {
                if ((m68k_cpu.n_flag == 0))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbge()
            {
                if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dblt()
            {
                if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dbgt()
            {
                if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_dble()
            {
                if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    UIntSubArray d_reg = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                    uint res = ((d_reg[0] - 1) & 0xffff);

                    d_reg[0] = (uint)((d_reg[0]) & ~0xffff) | res;
                    if (res != 0xffff)
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
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


            static void m68000_divs_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7]));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 6);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 6);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix()));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32()));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 12);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 12);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                int src = MAKE_INT_16(m68ki_read_16(ea));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix()));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divs_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                int src = MAKE_INT_16(m68ki_read_imm_16());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    int quotient = MAKE_INT_32(d_dst[0]) / src;
                    int remainder = MAKE_INT_32(d_dst[0]) % src;

                    if (quotient == MAKE_INT_16(quotient))
                    {
                        m68k_cpu.not_z_flag = (uint)quotient;
                        m68k_cpu.n_flag = (uint)((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (uint)(((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (158 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (158 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 6);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 6);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_get_ea_ix());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_read_imm_32());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 12);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 12);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_16(ea);

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 8);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 8);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_get_ea_pcix());

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 10);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 10);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68000_divu_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_16();

                m68k_cpu.c_flag = 0;

                if (src != 0)
                {
                    uint quotient = d_dst[0] / src;
                    uint remainder = d_dst[0] % src;

                    if (quotient < 0x10000)
                    {
                        m68k_cpu.not_z_flag = quotient;
                        m68k_cpu.n_flag = ((quotient) & 0x8000);
                        m68k_cpu.v_flag = 0;
                        d_dst[0] = (((quotient) & 0xffff) | (remainder << 16));
                        m68k_clks_left[0] -= (140 + 4);
                        return;
                    }
                    m68k_cpu.v_flag = 1;
                    m68k_clks_left[0] -= (140 + 4);
                    return;
                }
                m68ki_interrupt(5);
            }


            static void m68020_divl_d_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 10);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 14);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)(uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint divisor = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 16);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 12);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 14);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
                                    quotient = (uint)(-quotient);
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68020_divl_i_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                        if (((word2) & 0x00000400) != 0)
                        {
                            /* if dividing the upper long does not clear it, we're overflowing. */
                            if ((dividend_hi / divisor) != 0)
                            {
                                m68k_cpu.v_flag = 1;
                                m68k_clks_left[0] -= (78 + 8);
                                return;
                            }

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (((dividend_hi) & 0x80000000) != 0)
                                {
                                    dividend_neg = 1;
                                    dividend_hi = (uint)((-dividend_hi) - ((dividend_lo != 0) ? 1 : 0));
                                    dividend_lo = (uint)(-dividend_lo);
                                }
                                if (((divisor) & 0x80000000) != 0)
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

                            if (((word2) & 0x00000800) != 0) /* signed */
                            {
                                if (dividend_neg != 0)
                                {
                                    remainder = (uint)(-remainder);
                                    quotient = (uint)(-quotient);
                                }
                                if (divisor_neg != 0)
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
                        if (((word2) & 0x00000800) != 0) /* signed */
                        {
                            m68k_cpu.dr[word2 & 7] = (uint)(MAKE_INT_32(dividend_lo) % MAKE_INT_32(divisor));
                            m68k_cpu.dr[(word2 >> 12) & 7] = (uint)(MAKE_INT_32(dividend_lo) / MAKE_INT_32(divisor));
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


            static void m68000_eor_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7])) & 0xff)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_eor_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_eor_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_eor_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_eor_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_eor_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_eor_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_eor_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_eor_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_eor_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_eor_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7])) & 0xffff)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_eor_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_eor_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_eor_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_eor_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_eor_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_eor_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_eor_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_eor_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]) ^= (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_eor_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eor_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eor_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_eor_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_eor_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_eor_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_eor_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_eori_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_8()) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_eori_ai_8()
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


            static void m68000_eori_pi_8()
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


            static void m68000_eori_pi7_8()
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


            static void m68000_eori_pd_8()
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


            static void m68000_eori_pd7_8()
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


            static void m68000_eori_di_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eori_ix_8()
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


            static void m68000_eori_aw_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp ^ m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eori_al_8()
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


            static void m68000_eori_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_16()) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_eori_ai_16()
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


            static void m68000_eori_pi_16()
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


            static void m68000_eori_pd_16()
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


            static void m68000_eori_di_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eori_ix_16()
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


            static void m68000_eori_aw_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp ^ m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_eori_al_16()
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


            static void m68000_eori_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]) ^= m68ki_read_imm_32();

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_eori_ai_32()
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


            static void m68000_eori_pi_32()
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


            static void m68000_eori_pd_32()
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


            static void m68000_eori_di_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_eori_ix_32()
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


            static void m68000_eori_aw_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp ^ m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_eori_al_32()
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


            static void m68000_eori_to_ccr()
            {
                m68ki_set_ccr((uint)(((m68k_cpu.x_flag != 0) ? 1 : 0 << 4) | ((m68k_cpu.n_flag != 0) ? 1 : 0 << 3) | ((m68k_cpu.not_z_flag == 0) ? 1 : 0 << 2) | ((m68k_cpu.v_flag != 0) ? 1 : 0 << 1) | ((m68k_cpu.c_flag != 0) ? 1 : 0)) ^ m68ki_read_imm_16());
                m68k_clks_left[0] -= (20);
            }


            static void m68000_eori_to_sr()
            {
                uint eor_val = m68ki_read_imm_16();

                if (m68k_cpu.s_flag != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr((((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)) ^ eor_val);
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_exg_dd()
            {
                UIntSubArray reg_a = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                UIntSubArray reg_b = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint tmp = reg_a[0];

                reg_a[0] = reg_b[0];
                reg_b[0] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m68000_exg_aa()
            {
                UIntSubArray reg_a = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);
                UIntSubArray reg_b = new UIntSubArray(m68k_cpu.ar, (int)m68k_cpu.ir & 7);
                uint tmp = reg_a[0];

                reg_a[0] = reg_b[0];
                reg_b[0] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m68000_exg_da()
            {
                UIntSubArray reg_a = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                UIntSubArray reg_b = new UIntSubArray(m68k_cpu.ar, (int)m68k_cpu.ir & 7);
                uint tmp = reg_a[0];

                reg_a[0] = reg_b[0];
                reg_b[0] = tmp;

                m68k_clks_left[0] -= (6);
            }


            static void m68000_ext_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);

                d_dst[0] = (uint)(((d_dst[0]) & ~0xffff) | ((d_dst[0]) & 0xff) | (((d_dst[0]) & 0x80) != 0 ? 0xff00 : 0));

                m68k_cpu.n_flag = ((d_dst[0]) & 0x8000);
                m68k_cpu.not_z_flag = ((d_dst[0]) & 0xffff);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_ext_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);

                d_dst[0] = ((d_dst[0]) & 0xffff) | (((d_dst[0]) & 0x8000) != 0 ? 0xffff0000 : 0);

                m68k_cpu.n_flag = ((d_dst[0]) & 0x80000000);
                m68k_cpu.not_z_flag = d_dst[0];
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68020_extb()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);

                    d_dst[0] = ((d_dst[0]) & 0xff) | (((d_dst[0]) & 0x80) != 0 ? 0xffffff00 : 0);

                    m68k_cpu.n_flag = ((d_dst[0]) & 0x80000000);
                    m68k_cpu.not_z_flag = d_dst[0];
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }

            static void m68000_illegal()
            {
                m68ki_exception(4);
            }

            static void m68000_jmp_ai()
            {
                m68ki_set_pc((m68k_cpu.ar[m68k_cpu.ir & 7]));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m68000_jmp_di()
            {
                m68ki_set_pc(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m68000_jmp_ix()
            {
                m68ki_set_pc(m68ki_get_ea_ix());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 14);
            }


            static void m68000_jmp_aw()
            {
                m68ki_set_pc((uint)MAKE_INT_16(m68ki_read_imm_16()));
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m68000_jmp_al()
            {
                m68ki_set_pc(m68ki_read_imm_32());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m68000_jmp_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                m68ki_set_pc(ea);
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 10);
            }


            static void m68000_jmp_pcix()
            {
                m68ki_set_pc(m68ki_get_ea_pcix());
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (0 + 14);
            }


            static void m68000_jsr_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m68000_jsr_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m68000_jsr_ix()
            {
                uint ea = m68ki_get_ea_ix();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 22);
            }


            static void m68000_jsr_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m68000_jsr_al()
            {
                uint ea = m68ki_read_imm_32();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m68000_jsr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 18);
            }


            static void m68000_jsr_pcix()
            {
                uint ea = m68ki_get_ea_pcix();

                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68ki_set_pc(ea);
                m68k_clks_left[0] -= (0 + 22);
            }


            static void m68000_lea_ai()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                m68k_clks_left[0] -= (0 + 4);
            }


            static void m68000_lea_di()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m68000_lea_ix()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_get_ea_ix();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m68000_lea_aw()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_imm_16());
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m68000_lea_al()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_imm_32();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m68000_lea_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ea;
                m68k_clks_left[0] -= (0 + 8);
            }


            static void m68000_lea_pcix()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_get_ea_pcix();
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m68000_link_16_a7()
            {
                m68k_cpu.ar[7] -= 4;
                m68ki_write_32(m68k_cpu.ar[7], m68k_cpu.ar[7]);
                m68k_cpu.ar[7] = (m68k_cpu.ar[7] + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (16);
            }


            static void m68000_link_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, m68k_cpu.ir & 7);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, a_dst[0]);
                a_dst[0] = m68k_cpu.ar[7];
                m68k_cpu.ar[7] = (m68k_cpu.ar[7] + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (16);
            }


            static void m68020_link_32_a7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68k_cpu.ar[7] -= 4;
                    m68ki_write_32(m68k_cpu.ar[7], m68k_cpu.ar[7]);
                    m68k_cpu.ar[7] = (m68k_cpu.ar[7] + m68ki_read_imm_32());
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_link_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, m68k_cpu.ir & 7);

                    m68ki_write_32(m68k_cpu.ar[7] -= 4, a_dst[0]);
                    a_dst[0] = m68k_cpu.ar[7];
                    m68k_cpu.ar[7] = (m68k_cpu.ar[7] + m68ki_read_imm_32());
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_lsr_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint res = src >> (int)shift;

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = shift > 8 ? 0 : (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_lsr_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = src >> (int)shift;

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_lsr_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = d_dst[0];
                uint res = src >> (int)shift;

                d_dst[0] = res;

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_lsr_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xff);
                uint res = src >> (int)shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 8)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffffff00;
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


            static void m68000_lsr_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = src >> (int)shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 16)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffff0000;
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


            static void m68000_lsr_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = d_dst[0];
                uint res = src >> (int)shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        d_dst[0] = res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = 0;
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] = 0;
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


            static void m68000_lsr_ea_ai()
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


            static void m68000_lsr_ea_pi()
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


            static void m68000_lsr_ea_pd()
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


            static void m68000_lsr_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_lsr_ea_ix()
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


            static void m68000_lsr_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = 0;
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_lsr_ea_al()
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


            static void m68000_lsl_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((src << (int)shift) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = shift > 8 ? 0 : (src >> (int)(8 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_lsl_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((src << (int)shift) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(16 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_lsl_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = d_dst[0];
                uint res = (src << (int)shift);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(32 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_lsl_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((src << (int)shift) & 0xff);

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 8)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(8 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffffff00;
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


            static void m68000_lsl_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((src << (int)shift) & 0xffff);

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift <= 16)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(16 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x8000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffff0000;
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


            static void m68000_lsl_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = d_dst[0];
                uint res = (src << (int)shift);

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        d_dst[0] = res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(32 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80000000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] = 0;
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


            static void m68000_lsl_ea_ai()
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


            static void m68000_lsl_ea_pi()
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


            static void m68000_lsl_ea_pd()
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


            static void m68000_lsl_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_lsl_ea_ix()
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


            static void m68000_lsl_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_lsl_ea_al()
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


            static void m68000_move_dd_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_move_dd_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_dd_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_dd_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_dd_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_move_dd_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_move_dd_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_move_dd_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_move_dd_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_move_dd_i_8()
            {
                uint res = m68ki_read_imm_8();
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_ai_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_ai_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_ai_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_ai_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_ai_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_ai_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_ai_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_ai_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_ai_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_ai_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi7_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pi_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pi7_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi7_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi7_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi7_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pi7_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pi7_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi7_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi7_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi7_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pi7_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi7_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi7_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[7] += 2) - 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pi_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pi_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pi_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd7_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pd_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pd7_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd7_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd7_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd7_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pd7_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pd7_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd7_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd7_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd7_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pd7_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd7_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd7_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = m68k_cpu.ar[7] -= 2;

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pd_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pd_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pd_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_di_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_di_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_di_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_di_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_di_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_di_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_di_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_di_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_di_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_di_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_ix_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_move_ix_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_ix_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_ix_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_ix_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m68000_move_ix_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m68000_move_ix_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m68000_move_ix_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m68000_move_ix_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m68000_move_ix_i_8()
            {
                uint res = m68ki_read_imm_8();

                m68ki_write_8(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_aw_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_aw_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_aw_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_aw_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_aw_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_aw_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_aw_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_aw_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_aw_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_aw_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_al_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_al_ai_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_al_pi_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_al_pi7_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_al_pd_8()
            {
                uint res = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m68000_move_al_pd7_8()
            {
                uint res = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m68000_move_al_di_8()
            {
                uint res = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_ix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_al_aw_8()
            {
                uint res = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_al_8()
            {
                uint res = m68ki_read_8(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_al_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_8(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_pcix_8()
            {
                uint res = m68ki_read_8(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_al_i_8()
            {
                uint res = m68ki_read_imm_8();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_8(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_dd_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_move_dd_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_move_dd_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_dd_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_dd_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_move_dd_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_move_dd_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_move_dd_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_move_dd_i_16()
            {
                uint res = m68ki_read_imm_16();
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_move_ai_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_ai_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_ai_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_ai_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_ai_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_ai_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_ai_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_ai_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_ai_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_ai_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pi_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pi_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pi_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pi_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pi_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pi_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pi_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pd_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_move_pd_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_pd_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_move_pd_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_move_pd_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_move_pd_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_move_pd_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2;

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_move_di_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_di_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_di_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_di_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_di_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_di_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_di_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_di_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_di_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_ix_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_move_ix_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_move_ix_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_ix_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_ix_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m68000_move_ix_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m68000_move_ix_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m68000_move_ix_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_move_ix_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m68000_move_ix_i_16()
            {
                uint res = m68ki_read_imm_16();

                m68ki_write_16(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_move_aw_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_aw_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_aw_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_aw_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_aw_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_aw_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_aw_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_aw_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_aw_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_aw_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_al_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_al_a_16()
            {
                uint res = (((m68k_cpu.ar[m68k_cpu.ir & 7])) & 0xffff);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_al_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_al_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_al_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 6);
            }


            static void m68000_move_al_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_al_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_al_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_pcix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_al_i_16()
            {
                uint res = m68ki_read_imm_16();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_16(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 4);
            }


            static void m68000_move_dd_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_move_dd_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_move_dd_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_dd_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_move_dd_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_move_dd_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m68000_move_dd_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_move_dd_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m68000_move_dd_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_move_dd_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m68000_move_dd_i_32()
            {
                uint res = m68ki_read_imm_32();
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_move_ai_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_ai_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_ai_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_ai_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_ai_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_ai_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_ai_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_ai_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_ai_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_move_ai_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_ai_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_ai_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pi_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_pi_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_pi_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pi_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pi_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_pi_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pi_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_pi_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pi_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_move_pi_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pi_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_pi_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pd_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_pd_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_pd_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pd_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_pd_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_pd_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pd_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_pd_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pd_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_move_pd_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_pd_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_move_pd_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4;

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_di_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_di_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_di_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_di_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_di_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_di_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_di_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m68000_move_di_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_di_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)(uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 16);
            }


            static void m68000_move_di_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_di_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m68000_move_di_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_ix_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_move_ix_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_move_ix_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m68000_move_ix_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m68000_move_ix_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 10);
            }


            static void m68000_move_ix_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m68000_move_ix_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 14);
            }


            static void m68000_move_ix_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m68000_move_ix_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 16);
            }


            static void m68000_move_ix_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 12);
            }


            static void m68000_move_ix_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 14);
            }


            static void m68000_move_ix_i_32()
            {
                uint res = m68ki_read_imm_32();

                m68ki_write_32(m68ki_get_ea_ix_dst(), res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (18 + 8);
            }


            static void m68000_move_aw_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_aw_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_move_aw_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_aw_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_aw_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 10);
            }


            static void m68000_move_aw_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_aw_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m68000_move_aw_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_aw_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 16);
            }


            static void m68000_move_aw_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 12);
            }


            static void m68000_move_aw_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 14);
            }


            static void m68000_move_aw_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (16 + 8);
            }


            static void m68000_move_al_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20);
            }


            static void m68000_move_al_a_32()
            {
                uint res = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20);
            }


            static void m68000_move_al_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_move_al_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_move_al_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m68000_move_al_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_move_al_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68000_move_al_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_move_al_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68000_move_al_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_32(ea);
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_move_al_pcix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_pcix());
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68000_move_al_i_32()
            {
                uint res = m68ki_read_imm_32();
                uint ea_dst = m68ki_read_imm_32();

                m68ki_write_32(ea_dst, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_movea_d_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m68000_movea_a_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m68000_movea_ai_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_movea_pi_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_movea_pd_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_movea_di_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_movea_ix_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_movea_aw_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_movea_al_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_movea_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(ea));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_movea_pcix_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_movea_i_16()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_16(m68ki_read_imm_16());
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_movea_d_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m68000_movea_a_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = ((m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4);
            }


            static void m68000_movea_ai_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_movea_pi_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_movea_pd_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_movea_di_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_movea_ix_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_get_ea_ix());
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m68000_movea_aw_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_movea_al_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_read_imm_32());
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m68000_movea_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(ea);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_movea_pcix_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_32(m68ki_get_ea_pcix());
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m68000_movea_i_32()
            {
                (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) = m68ki_read_imm_32();
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68010_move_fr_ccr_d()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) =(uint) (((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u));
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_ai()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((m68k_cpu.ar[m68k_cpu.ir & 7]), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_pi()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_pd()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)));
                    m68k_clks_left[0] -= (8 + 6);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_di()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_ix()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(m68ki_get_ea_ix(), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0?1u:0u))));
                    m68k_clks_left[0] -= (8 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_aw()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16((uint)MAKE_INT_16(m68ki_read_imm_16()), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68010_move_fr_ccr_al()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    m68ki_write_16(m68ki_read_imm_32(), (((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_move_to_ccr_d()
            {
                m68ki_set_ccr((m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (12);
            }


            static void m68000_move_to_ccr_ai()
            {
                m68ki_set_ccr(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_to_ccr_pi()
            {
                m68ki_set_ccr(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_to_ccr_pd()
            {
                m68ki_set_ccr(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_move_to_ccr_di()
            {
                m68ki_set_ccr(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_to_ccr_ix()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_to_ccr_aw()
            {
                m68ki_set_ccr(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_to_ccr_al()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_move_to_ccr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                m68ki_set_ccr(m68ki_read_16(ea));
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_move_to_ccr_pcix()
            {
                m68ki_set_ccr(m68ki_read_16(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_move_to_ccr_i()
            {
                m68ki_set_ccr(m68ki_read_imm_16());
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_move_fr_sr_d()
            {
                if ((m68k_cpu.mode & 1) !=0|| m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u));
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                if ((m68k_cpu.mode & 1)!=0 || m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);

                if ((m68k_cpu.mode & 1) !=0|| m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);

                if ((m68k_cpu.mode & 1) !=0|| m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) +(uint) MAKE_INT_16(m68ki_read_imm_16()));

                if ((m68k_cpu.mode & 1)!=0 || m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_ix()
            {
                uint ea = m68ki_get_ea_ix();

                if ((m68k_cpu.mode & 1)!=0 || m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) |( (m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                if ((m68k_cpu.mode & 1) !=0|| m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)));
                    m68k_clks_left[0] -= (8 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_sr_al()
            {
                uint ea = m68ki_read_imm_32();

                if ((m68k_cpu.mode & 1) !=0|| m68k_cpu.s_flag!=0) /* NS990408 */
                {
                    m68ki_write_16(ea, (((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)));
                    m68k_clks_left[0] -= (8 + 12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_d()
            {
                if (m68k_cpu.s_flag!=0)
                {
                    m68ki_set_sr((m68k_cpu.dr[m68k_cpu.ir & 7]));
                    m68k_clks_left[0] -= (12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_ai()
            {
                uint new_sr = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_pi()
            {
                uint new_sr = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_pd()
            {
                uint new_sr = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 6);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_di()
            {
                uint new_sr = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_ix()
            {
                uint new_sr = m68ki_read_16(m68ki_get_ea_ix());

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_aw()
            {
                uint new_sr = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_al()
            {
                uint new_sr = m68ki_read_16(m68ki_read_imm_32());

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 12);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint new_sr = m68ki_read_16(ea);

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 8);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_pcix()
            {
                uint new_sr = m68ki_read_16(m68ki_get_ea_pcix());

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 10);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_sr_i()
            {
                uint new_sr = m68ki_read_imm_16();

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] -= (12 + 4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_fr_usp()
            {
                if (m68k_cpu.s_flag!=0)
                {
                    (m68k_cpu.ar[m68k_cpu.ir & 7]) = m68k_cpu.sp[0];
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_move_to_usp()
            {
                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.sp[0] = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68010_movec_cr()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    if (m68k_cpu.s_flag!=0)
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
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.caar;
                                    return;
                                }
                                m68000_illegal();
                                break;
                            case 0x803: /* MSP */
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
                                {
                                    m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] = m68k_cpu.sp[3];
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x804: /* ISP */
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68010_movec_rc()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    if (m68k_cpu.s_flag!=0)
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
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
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
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
                                {
                                    m68k_cpu.caar = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x803: /* MSP */
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
                                {
                                    m68k_cpu.sp[3] = m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7] & 7;
                                    return;
                                }
                                m68000_illegal();
                                return;
                            case 0x804: /* ISP */
                                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_movem_pd_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        ea -= 2;
                        m68ki_write_16(ea, m68k_movem_pd_table[i][0]);
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                m68k_clks_left[0] -=(int) ((count << 2) + 8);
            }


            static void m68000_movem_pd_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        ea -= 4;
                        m68ki_write_32(ea, m68k_movem_pd_table[i][0]);
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 8);
            }


            static void m68000_movem_pi_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 <<(int) i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                m68k_clks_left[0] -= (int)((count << 2) + 12);
            }


            static void m68000_movem_pi_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                (m68k_cpu.ar[m68k_cpu.ir & 7]) = ea;
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)(int)((count << 3) + 12);
            }


            static void m68000_movem_re_ai_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_16(ea, m68k_movem_pi_table[i][0]);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 4 + 4);
            }


            static void m68000_movem_re_di_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_16(ea, m68k_movem_pi_table[i][0]);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 4 + 8);
            }


            static void m68000_movem_re_ix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_16(ea, m68k_movem_pi_table[i][0]);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 4 + 10);
            }


            static void m68000_movem_re_aw_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_16(ea, m68k_movem_pi_table[i][0]);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 4 + 8);
            }


            static void m68000_movem_re_al_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_16(ea, m68k_movem_pi_table[i][0]);
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -=(int) ((count << 2) + 4 + 12);
            }


            static void m68000_movem_re_ai_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_32(ea, m68k_movem_pi_table[i][0]);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 4 + 4);
            }


            static void m68000_movem_re_di_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_32(ea, m68k_movem_pi_table[i][0]);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 4 + 8);
            }


            static void m68000_movem_re_ix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_32(ea, m68k_movem_pi_table[i][0]);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 4 + 10);
            }


            static void m68000_movem_re_aw_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_32(ea, m68k_movem_pi_table[i][0]);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 4 + 8);
            }


            static void m68000_movem_re_al_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68ki_write_32(ea, m68k_movem_pi_table[i][0]);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 4 + 12);
            }


            static void m68000_movem_er_ai_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 4);
            }


            static void m68000_movem_er_di_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 8);
            }


            static void m68000_movem_er_ix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 10);
            }


            static void m68000_movem_er_aw_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea =(uint) MAKE_INT_16(m68ki_read_imm_16());
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 8);
            }


            static void m68000_movem_er_al_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 12);
            }


            static void m68000_movem_er_pcdi_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 8);
            }


            static void m68000_movem_er_pcix_16()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_pcix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = (uint)MAKE_INT_16(((m68ki_read_16(ea)) & 0xffff));
                        ea += 2;
                        count++;
                    }
                m68k_clks_left[0] -= (int)((count << 2) + 8 + 10);
            }


            static void m68000_movem_er_ai_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -=(int) ((count << 3) + 8 + 4);
            }


            static void m68000_movem_er_di_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 8 + 8);
            }


            static void m68000_movem_er_ix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -=(int) ((count << 3) + 8 + 10);
            }


            static void m68000_movem_er_aw_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -=(int) ((count << 3) + 8 + 8);
            }


            static void m68000_movem_er_al_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 8 + 12);
            }


            static void m68000_movem_er_pcdi_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 8 + 8);
            }


            static void m68000_movem_er_pcix_32()
            {
                uint i = 0;
                uint register_list = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_pcix();
                uint count = 0;

                for (; i < 16; i++)
                    if ((register_list & (1 << (int)i))!=0)
                    {
                        m68k_movem_pi_table[i][0] = m68ki_read_32(ea);
                        ea += 4;
                        count++;
                    }
                /* ASG: changed from (count << 4) to (count << 3) */
                m68k_clks_left[0] -= (int)((count << 3) + 8 + 10);
            }


            static void m68000_movep_re_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(((m68ki_read_imm_16()) & 0xffff));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea, ((src >> 8) & 0xff));
                m68ki_write_8(ea += 2, ((src) & 0xff));
                m68k_clks_left[0] -= (16);
            }


            static void m68000_movep_re_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(((m68ki_read_imm_16()) & 0xffff));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                m68ki_write_8(ea, ((src >> 24) & 0xff));
                m68ki_write_8(ea += 2, ((src >> 16) & 0xff));
                m68ki_write_8(ea += 2, ((src >> 8) & 0xff));
                m68ki_write_8(ea += 2, ((src) & 0xff));
                m68k_clks_left[0] -= (24);
            }


            static void m68000_movep_er_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(((m68ki_read_imm_16()) & 0xffff));
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);

                d_dst[0] =(uint) ((d_dst[0]) & ~0xffff) | ((m68ki_read_8(ea) << 8) + m68ki_read_8(ea + 2));
                m68k_clks_left[0] -= (16);
            }


            static void m68000_movep_er_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(((m68ki_read_imm_16()) & 0xffff));

                (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) = (m68ki_read_8(ea) << 24) + (m68ki_read_8(ea + 2) << 16)
                 + (m68ki_read_8(ea + 4) << 8) + m68ki_read_8(ea + 6);
                m68k_clks_left[0] -= (24);
            }


            static void m68010_moves_ai_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8)) != 0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 18);
                    if (((next_word) & 0x00000800) != 0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000) != 0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pi_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] =(uint) MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pi7_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] =(uint)( ((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pd_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pd7_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_di_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_ix_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_aw_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_8_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_al_8()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_8(m68ki_read_8_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xff) | m68ki_read_8_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_ai_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pi_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] =(uint) MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_pd_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_di_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_ix_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_aw_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (0 + 20);
                    if (((next_word) & 0x00000800)!=0) /* Register to memory */
                    {
                        m68ki_write_16_fc(ea, m68k_cpu.dfc, m68k_cpu_dar[next_word >> 15][(next_word >> 12) & 7]);
                        return;
                    }
                    if (((next_word) & 0x00008000)!=0) /* Memory to address register */
                    {
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_al_16()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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
                        m68k_cpu.ar[(next_word >> 12) & 7] = (uint)MAKE_INT_16(m68ki_read_16_fc(ea, m68k_cpu.sfc));
                        return;
                    }
                    /* Memory to data register */
                    m68k_cpu.dr[(next_word >> 12) & 7] = (uint)(((m68k_cpu.dr[(next_word >> 12) & 7]) & ~0xffff) | m68ki_read_16_fc(ea, m68k_cpu.sfc));
                    return;
                }
                m68000_illegal();
            }


            static void m68010_moves_ai_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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


            static void m68010_moves_pi_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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


            static void m68010_moves_pd_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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


            static void m68010_moves_di_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

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


            static void m68010_moves_ix_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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


            static void m68010_moves_aw_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
                {
                    uint next_word = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

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


            static void m68010_moves_al_32()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8))!=0)
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


            static void m68000_moveq()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) = (uint)MAKE_INT_8(((m68k_cpu.ir) & 0xff));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_muls_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7])) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54);
            }


            static void m68000_muls_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68000_muls_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68000_muls_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 6);
            }


            static void m68000_muls_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())))) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_muls_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix())) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m68000_muls_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()))) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_muls_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32())) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 12);
            }


            static void m68000_muls_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = (uint)MAKE_INT_16(m68ki_read_16(ea)) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_muls_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix())) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m68000_muls_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (uint)MAKE_INT_16(m68ki_read_imm_16()) * (uint)MAKE_INT_16(((d_dst[0]) & 0xffff));

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68000_mulu_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54);
            }


            static void m68000_mulu_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68000_mulu_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68000_mulu_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 6);
            }


            static void m68000_mulu_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_mulu_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16(m68ki_get_ea_ix()) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m68000_mulu_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_mulu_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16(m68ki_read_imm_32()) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 12);
            }


            static void m68000_mulu_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = m68ki_read_16(ea) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 8);
            }


            static void m68000_mulu_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_16(m68ki_get_ea_pcix()) * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 10);
            }


            static void m68000_mulu_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint res = m68ki_read_imm_16() * ((d_dst[0]) & 0xffff);

                d_dst[0] = res;

                m68k_cpu.not_z_flag = res;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (54 + 4);
            }


            static void m68020_mull_d_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = (uint)(((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u));

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (uint)((lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff));

                    if ((((word2) & 0x00000800)!=0 && neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1 : 0));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000) !=0&& hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1) ?1u:0u+ ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000) ==0&& hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1) ?1u:0u+ ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000) !=0&& hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0)?1u:0u);
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = (uint)(((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1:0));

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (uint)(((lo < r1)?1:0) + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff));

                    if (((word2) & 0x00000800) !=0&& neg!=0)
                    {
                        hi = (uint)((-hi) - ((lo != 0)?1u:0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || ((((lo) & 0x80000000)==0) && (hi==0)))?1u:0u;
                    m68k_clks_left[0] -= (43 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo =  (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1) ?1u:0u+ ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if (((word2) & 0x00000800) !=0&& neg!=0)
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000) !=0&& hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if ((((word2) & 0x00000400)) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000) ==0&& hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1) ?1u:0u+ ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg!=0))
                    {
                        hi = (uint)((-hi) - ((lo != 0)?1u:0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if ((((word2) & 0x00000400)) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000) !=0&& hi == 0xffffffff) || (((lo) & 0x80000000) ==0&& hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 16);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint src = m68ki_read_32(ea);
                    uint dst = m68k_cpu.dr[(word2 >> 12) & 7];
                    uint neg = ((src ^ dst) & 0x80000000);
                    uint r1;
                    uint r2;
                    uint r3;
                    uint lo;
                    uint hi;

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = (uint)(((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u));

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (uint)((lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff));

                    if ((((word2) & 0x00000800)!=0 && neg!=0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if ((((word2) & 0x00000400)) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000)==0 && hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800) !=0&& neg!=0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000)!=0 && hi == 0xffffffff) || (((lo) & 0x80000000) ==0&& hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_mull_i_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

                    if (((word2) & 0x00000800) != 0) /* signed */
                    {
                        if (((src) & 0x80000000) != 0)
                            src = (uint)(-src);
                        if (((dst) & 0x80000000) != 0)
                            dst = (uint)(-dst);
                    }

                    r1 = ((src) & 0xffff) * ((dst) & 0xffff);
                    r2 = ((src >> 16) & 0xffff) * ((dst) & 0xffff);
                    r3 = ((src) & 0xffff) * ((dst >> 16) & 0xffff);
                    lo = (uint)(r1 + ((r2 << 16) & ~0xffff));
                    hi = ((src >> 16) & 0xffff) * ((dst >> 16) & 0xffff) + ((lo < r1)?1u:0u);

                    r1 = lo;
                    lo = (uint)(r1 + ((r3 << 16) & ~0xffff));
                    hi += (lo < r1)?1u:0u + ((r2 >> 16) & 0xffff) + ((r3 >> 16) & 0xffff);

                    if ((((word2) & 0x00000800)!=0 && neg != 0))
                    {
                        hi = (uint)((-hi) - ((lo != 0) ? 1u : 0u));
                        lo = (uint)(-lo);
                    }

                    m68k_cpu.dr[(word2 >> 12) & 7] = lo;
                    if (((word2) & 0x00000400) != 0)
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
                    m68k_cpu.v_flag = ((((lo) & 0x80000000) !=0&& hi == 0xffffffff) || (((lo) & 0x80000000) ==0&& hi==0))?1u:0u;
                    m68k_clks_left[0] -= (43 + 8);
                    return;
                }
                m68000_illegal();
            }
        

        }
    }
}