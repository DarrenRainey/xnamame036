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
            static void m68000_nbcd_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                if (res != 0x9a)
                {
                    if ((res & 0x0f) == 0xa)
                        res = (res & 0xf0) + 0x10;

                    d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                    if (res != 0)
                        m68k_cpu.not_z_flag = 1;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                }
                else
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_nbcd_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_nbcd_al()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((0x9a - dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

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


            static void m68000_neg_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = (uint)((-dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_neg_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_neg_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_neg_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_neg_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_neg_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_neg_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_neg_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_neg_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_neg_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_neg_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = (uint)((-dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_neg_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_neg_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_neg_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_neg_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_neg_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_neg_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_neg_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((-dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_neg_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (uint)(-dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_neg_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_neg_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_neg_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_neg_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_neg_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_neg_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_neg_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(-dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = (res != 0) ? 1u : 0u;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_negx_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_negx_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_negx_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_negx_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_negx_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_negx_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_negx_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_negx_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_negx_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_negx_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((-dst - (((m68k_cpu.x_flag != 0) ? 1u : 0u))) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_negx_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = (uint)((-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x8000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_negx_ai_16()
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


            static void m68000_negx_pi_16()
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


            static void m68000_negx_pd_16()
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


            static void m68000_negx_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
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


            static void m68000_negx_ix_16()
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


            static void m68000_negx_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
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


            static void m68000_negx_al_16()
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


            static void m68000_negx_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (uint)(-dst - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = ((dst & res) & 0x80000000);
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((dst | res) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_negx_ai_32()
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


            static void m68000_negx_pi_32()
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


            static void m68000_negx_pd_32()
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


            static void m68000_negx_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
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


            static void m68000_negx_ix_32()
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


            static void m68000_negx_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
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


            static void m68000_negx_al_32()
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


            static void m68000_nop()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68k_clks_left[0] -= (4);
            }


            static void m68000_not_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint res = ((~d_dst[0]) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_not_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_not_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_not_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_not_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_not_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_not_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_not_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_not_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_not_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((~m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_not_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint res = ((~d_dst[0]) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_not_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_not_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_not_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_not_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_not_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_not_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_not_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((~m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_not_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint res = d_dst[0] = (~d_dst[0]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_not_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_not_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_not_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_not_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_not_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_not_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_not_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (~m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_or_er_d_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_or_er_ai_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_pi_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_pi7_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_pd_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7]))))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_or_er_pd7_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((m68k_cpu.ar[7] -= 2)))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_or_er_di_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_ix_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_get_ea_ix()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_or_er_aw_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16())))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_al_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_read_imm_32()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_or_er_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(ea))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_pcix_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_8(m68ki_get_ea_pcix()))) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_or_er_i_8()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_8())) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_d_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_or_er_ai_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_pi_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_pd_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_or_er_di_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_ix_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_get_ea_ix()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_or_er_aw_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_al_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_read_imm_32()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_or_er_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(ea))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_or_er_pcix_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_16(m68ki_get_ea_pcix()))) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_or_er_i_16()
            {
                uint res = ((((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_16())) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_or_er_d_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= (m68k_cpu.dr[m68k_cpu.ir & 7])));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_or_er_ai_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_or_er_pi_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_or_er_pd_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_or_er_di_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_or_er_ix_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_get_ea_ix())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_or_er_aw_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()))));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_or_er_al_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_read_imm_32())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_or_er_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(ea)));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_or_er_pcix_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_32(m68ki_get_ea_pcix())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_or_er_i_32()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) |= m68ki_read_imm_32()));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_or_re_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_or_re_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_or_re_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_or_re_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_or_re_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_or_re_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_or_re_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_or_re_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_or_re_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_or_re_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_or_re_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_or_re_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_or_re_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_or_re_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_or_re_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_or_re_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_or_re_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_or_re_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_or_re_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_or_re_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_or_re_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_or_re_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_or_re_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_ori_d_8()
            {
                uint res = ((((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_8())) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_ori_ai_8()
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


            static void m68000_ori_pi_8()
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


            static void m68000_ori_pi7_8()
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


            static void m68000_ori_pd_8()
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


            static void m68000_ori_pd7_8()
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


            static void m68000_ori_di_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_ori_ix_8()
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


            static void m68000_ori_aw_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = ((tmp | m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_ori_al_8()
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


            static void m68000_ori_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_16()) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_ori_ai_16()
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


            static void m68000_ori_pi_16()
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


            static void m68000_ori_pd_16()
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


            static void m68000_ori_di_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_ori_ix_16()
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


            static void m68000_ori_aw_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = ((tmp | m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_ori_al_16()
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


            static void m68000_ori_d_32()
            {
                uint res = ((m68k_cpu.dr[m68k_cpu.ir & 7]) |= m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (16);
            }


            static void m68000_ori_ai_32()
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


            static void m68000_ori_pi_32()
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


            static void m68000_ori_pd_32()
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


            static void m68000_ori_di_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_ori_ix_32()
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


            static void m68000_ori_aw_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (tmp | m68ki_read_32(ea));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_ori_al_32()
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


            static void m68000_ori_to_ccr()
            {
                m68ki_set_ccr(((((m68k_cpu.x_flag != 0) ? 1u : 0u) << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)) | m68ki_read_imm_16());
                m68k_clks_left[0] -= (20);
            }


            static void m68000_ori_to_sr()
            {
                uint or_val = m68ki_read_imm_16();

                if (m68k_cpu.s_flag!=0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr((((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0)?1u:0u)) | or_val);
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68020_pack_rr()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) + m68ki_read_imm_16();

                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xff) | ((src >> 4) & 0x00f0) | (src & 0x000f);
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_pack_mm_ax7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_16((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);

                    src = (((src >> 8) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_pack_mm_ay7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_pack_mm_axy7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_32(m68k_cpu.ar[7] -= 4);

                    src = (((src >> 16) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(m68k_cpu.ar[7] -= 2, ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_pack_mm()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_16((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);

                    src = (((src >> 8) & 0x00ff) | ((src << 8) & 0xff00)) + m68ki_read_imm_16();
                    m68ki_write_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]), ((src >> 4) & 0xf0) | (src & 0x0f));
                    m68k_clks_left[0] -= (13);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_pea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 12);
            }


            static void m68000_pea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m68000_pea_ix()
            {
                uint ea = m68ki_get_ea_ix();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m68000_pea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m68000_pea_al()
            {
                uint ea = m68ki_read_imm_32();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m68000_pea_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 16);
            }


            static void m68000_pea_pcix()
            {
                uint ea = m68ki_get_ea_pcix();

                m68ki_write_32(m68k_cpu.ar[7] -= 4, ea);
                m68k_clks_left[0] -= (0 + 20);
            }


            static void m68000_rst()
            {
                if (m68k_cpu.s_flag != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (132);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_ror_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint shift = orig_shift & 7;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((((src) >> (int)(shift)) | ((src) << (int)(8 - (shift)))) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)((shift - 1) & 7)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
            }


            static void m68000_ror_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((((src) >> (int)(shift)) | ((src) << (int)(16 - (shift)))) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_ror_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (d_dst[0]);
                uint res = (((shift) < 32 ? (src) >> (int)(shift) : 0) | ((32 - (shift)) < 32 ? (src) << (int)(32 - (shift)) : 0));

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_ror_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 7;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((((src) >> (int)(shift)) | ((src) << (int)(8 - (shift)))) & 0xff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
                    m68k_cpu.c_flag = (src >> (int)((shift - 1) & 7)) & 1;
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


            static void m68000_ror_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 15;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((((src) >> (int)(shift)) | ((src) << (int)(16 - (shift)))) & 0xffff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
                    m68k_cpu.c_flag = (src >> (int)((shift - 1) & 15)) & 1;
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


            static void m68000_ror_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 31;
                uint src = (d_dst[0]);
                uint res = (((shift) < 32 ? (src) >> (int)(shift) : 0) | ((32 - (shift)) < 32 ? (src) << (int)(32 - (shift)) : 0));

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 8);
                if (orig_shift != 0)
                {
                    d_dst[0] = res;
                    m68k_cpu.c_flag = (src >> (int)((shift - 1) & 31)) & 1;
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


            static void m68000_ror_ea_ai()
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


            static void m68000_ror_ea_pi()
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


            static void m68000_ror_ea_pd()
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


            static void m68000_ror_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_ror_ea_ix()
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


            static void m68000_ror_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = ((((src) >> (1)) | ((src) << (16 - (1)))) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = src & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_ror_ea_al()
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


            static void m68000_rol_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint shift = orig_shift & 7;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((((((src) << (int)(shift)) | ((src) >> (int)(8 - (shift)))) & 0xff)) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (uint)((int)(src >> (int)(8 - orig_shift)) & 1);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
            }


            static void m68000_rol_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((((((src) << (int)(shift)) | ((src) >> (int)(16 - (shift)))) & 0xffff)) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(16 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_rol_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (d_dst[0]);
                uint res = ((((shift) < 32 ? (src) << (int)(shift) : 0) | ((32 - (shift)) < 32 ? (src) >> (int)(32 - (shift)) : 0)));

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = (src >> (int)(32 - shift)) & 1;
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_rol_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 7;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((((((src) << (int)(shift)) | ((src) >> (int)(8 - (shift)))) & 0xff)) & 0xff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    if (shift != 0)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
                        m68k_cpu.c_flag = (src >> (int)(8 - shift)) & 1;
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


            static void m68000_rol_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 15;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((((((src) << (int)(shift)) | ((src) >> (int)(16 - (shift)))) & 0xffff)) & 0xffff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    if (shift != 0)
                    {
                        d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
                        m68k_cpu.c_flag = (src >> (int)(16 - shift)) & 1;
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


            static void m68000_rol_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift & 31;
                uint src = (d_dst[0]);
                uint res = ((((shift) < 32 ? (src) << (int)(shift) : 0) | ((32 - (shift)) < 32 ? (src) >> (int)(32 - (shift)) : 0)));

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 8);
                if (orig_shift != 0)
                {
                    d_dst[0] = res;

                    m68k_cpu.c_flag = (src >> (int)(32 - shift)) & 1;
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


            static void m68000_rol_ea_ai()
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


            static void m68000_rol_ea_pi()
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


            static void m68000_rol_ea_pd()
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


            static void m68000_rol_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_rol_ea_ix()
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


            static void m68000_rol_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = ((((((src) << (1)) | ((src) >> (16 - (1)))) & 0xffff)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = ((src) & 0x8000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_rol_ea_al()
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


            static void m68000_roxr_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (int)(9 - (shift)));
                uint res = ((tmp) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_roxr_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (int)(17 - (shift)));
                uint res = ((tmp) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_roxr_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (d_dst[0]);
                uint res = (uint)(((((shift) < 32 ? (src) >> (int)(shift) : 0) | ((33 - (shift)) < 32 ? (src) << (int)(33 - (shift)) : 0)) & ~(1 << (int)(32 - shift))) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << (int)(32 - shift)));
                uint new_x_flag = (uint)(src & (1 << (int)(shift - 1)));

                d_dst[0] = res;

                m68k_cpu.c_flag = m68k_cpu.x_flag = new_x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_roxr_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 9;
                uint src = ((d_dst[0]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (int)(9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
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


            static void m68000_roxr_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 17;
                uint src = ((d_dst[0]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (int)(17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
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


            static void m68000_roxr_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 33;
                uint src = (d_dst[0]);
                uint res = (uint)(((((shift) < 32 ? (src) >> (int)(shift) : 0) | ((33 - (shift)) < 32 ? (src) << (int)(33 - (shift)) : 0)) & ~(1 << (int)(32 - shift))) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << (int)(32 - shift)));
                uint new_x_flag = (uint)(src & (1 << (int)(shift - 1)));

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 8);
                if (shift != 0)
                {
                    d_dst[0] = res;
                    m68k_cpu.x_flag = new_x_flag;
                }
                else
                    res = src;
                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m68000_roxr_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_roxr_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_roxr_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_roxr_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_roxr_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_roxr_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_roxr_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_roxl_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (int)(9 - (shift)));
                uint res = ((tmp) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x100);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_roxl_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (int)(17 - (shift)));
                uint res = ((tmp) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_roxl_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (d_dst[0]);
                uint res = (uint)(((((shift) < 32 ? (src) << (int)(shift) : 0) | ((33 - (shift)) < 32 ? (src) >> (int)(33 - (shift)) : 0)) & ~(1 << (int)(shift - 1))) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << (int)(shift - 1)));
                uint new_x_flag = (uint)(src & (1 << (int)(32 - shift)));

                d_dst[0] = res;

                m68k_cpu.c_flag = m68k_cpu.x_flag = new_x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_roxl_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 9;
                uint src = ((d_dst[0]) & 0xff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) << (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 8)) >> (int)(9 - (shift)));
                uint res = ((tmp) & 0xff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;
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


            static void m68000_roxl_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 17;
                uint src = ((d_dst[0]) & 0xffff);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (int)(shift)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (int)(17 - (shift)));
                uint res = ((tmp) & 0xffff);

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 6);
                if (orig_shift != 0)
                {
                    d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;
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


            static void m68000_roxl_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint orig_shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint shift = orig_shift % 33;
                uint src = (d_dst[0]);
                uint res = (uint)(((((shift) < 32 ? (src) << (int)(shift) : 0) | ((33 - (shift)) < 32 ? (src) >> (int)(33 - (shift)) : 0)) & ~(1 << (int)(shift - 1))) | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << (int)(shift - 1)));
                uint new_x_flag = (uint)(src & (1 << (int)(32 - shift)));

                m68k_clks_left[0] -= (int)((orig_shift << 1) + 8);
                if (shift != 0)
                {
                    d_dst[0] = res;
                    m68k_cpu.x_flag = new_x_flag;
                }
                else
                    res = src;
                m68k_cpu.c_flag = m68k_cpu.x_flag;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m68000_roxl_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_roxl_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_roxl_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_roxl_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_roxl_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_roxl_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_roxl_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint tmp = ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) << (1)) | ((src | (((m68k_cpu.x_flag != 0) ? 1u : 0u) << 16)) >> (17 - (1)));
                uint res = ((tmp) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.x_flag = ((tmp) & 0x10000);
                m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68010_rtd()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8)) != 0)
                {
                    uint new_pc = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.ar[7] += (uint)MAKE_INT_16(m68ki_read_imm_16());
                    m68ki_set_pc(new_pc);
                    m68k_clks_left[0] -= (16);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_rte()
            {
                uint new_sr;
                uint new_pc;
                uint format_word;

                if (m68k_cpu.s_flag != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    new_sr = m68ki_read_16((m68k_cpu.ar[7] += 2) - 2);
                    new_pc = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);
                    m68ki_set_pc(new_pc);
                    if ((m68k_cpu.mode & (2 | 4 | 8)) == 0)
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


            static void m68020_rtm()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    ;


                    m68k_clks_left[0] -= (19);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_rtr()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_set_ccr(m68ki_read_16((m68k_cpu.ar[7] += 2) - 2));
                m68ki_set_pc(m68ki_read_32((m68k_cpu.ar[7] += 4) - 4));
                m68k_clks_left[0] -= (20);
            }


            static void m68000_rts()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_set_pc(m68ki_read_32((m68k_cpu.ar[7] += 4) - 4));
                m68k_clks_left[0] -= (16);
            }


            static void m68000_sbcd_rr()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0
                    )
                    res += 0xa0;

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | ((res) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_sbcd_mm_ax7()
            {
                uint src = m68ki_read_8(--((m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_sbcd_mm_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_sbcd_mm_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res += 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_sbcd_mm()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst) & 0x0f) - ((src) & 0x0f) - ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res -= 6;
                res += ((dst) & 0xf0) - ((src) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res += 0xa0;

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                m68ki_write_8(ea, res);

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_st_d()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) |= 0xff;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_st_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_st_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_st_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), 0xff);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_st_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), 0xff);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_st_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), 0xff);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_st_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), 0xff);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_st_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), 0xff);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_st_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), 0xff);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_st_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), 0xff);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sf_d()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_sf_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sf_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sf_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sf_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sf_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sf_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sf_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sf_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sf_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_shi_d()
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


            static void m68000_shi_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0u);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_shi_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_shi_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_shi_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_shi_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_shi_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_shi_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_shi_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_shi_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sls_d()
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


            static void m68000_sls_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sls_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sls_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sls_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sls_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sls_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sls_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sls_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sls_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_scc_d()
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


            static void m68000_scc_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scc_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scc_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scc_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_scc_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_scc_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_scc_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_scc_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_scc_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_scs_d()
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


            static void m68000_scs_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scs_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scs_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_scs_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_scs_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_scs_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_scs_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_scs_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_scs_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.c_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sne_d()
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


            static void m68000_sne_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sne_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sne_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sne_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sne_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sne_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sne_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sne_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sne_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_seq_d()
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


            static void m68000_seq_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_seq_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_seq_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_seq_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_seq_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_seq_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_seq_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_seq_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_seq_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_svc_d()
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


            static void m68000_svc_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svc_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svc_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svc_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_svc_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_svc_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_svc_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_svc_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_svc_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.v_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_svs_d()
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


            static void m68000_svs_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svs_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svs_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_svs_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_svs_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_svs_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_svs_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_svs_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_svs_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.v_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_spl_d()
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


            static void m68000_spl_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_spl_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_spl_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_spl_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_spl_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.n_flag == 0) ? 0xffu : 0u);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_spl_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_spl_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_spl_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_spl_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.n_flag == 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_smi_d()
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


            static void m68000_smi_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_smi_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_smi_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_smi_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_smi_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_smi_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_smi_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_smi_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_smi_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.n_flag != 0) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sge_d()
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


            static void m68000_sge_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sge_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sge_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sge_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sge_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sge_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sge_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sge_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sge_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), ((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_slt_d()
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


            static void m68000_slt_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_slt_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_slt_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_slt_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_slt_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_slt_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_slt_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_slt_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_slt_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), ((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sgt_d()
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


            static void m68000_sgt_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sgt_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sgt_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sgt_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sgt_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sgt_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sgt_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sgt_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sgt_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_sle_d()
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


            static void m68000_sle_ai()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sle_pi()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sle_pi7()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_sle_pd()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sle_pd7()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_sle_di()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sle_ix()
            {
                m68ki_write_8(m68ki_get_ea_ix(), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_sle_aw()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_sle_al()
            {
                m68ki_write_8(m68ki_read_imm_32(), (m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)) ? 0xffu : 0);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_stop()
            {
                uint new_sr = m68ki_read_imm_16();

                if (m68k_cpu.s_flag != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.stopped = 1;
                    m68ki_set_sr(new_sr);
                    m68k_clks_left[0] = 0;
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_sub_er_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_sub_er_ai_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_pi_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_pi7_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_pd_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_sub_er_pd7_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_sub_er_di_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_ix_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_sub_er_aw_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_al_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_sub_er_pcdi_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_8(ea);
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_pcix_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_sub_er_i_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_8();
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_sub_er_a_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_sub_er_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_sub_er_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_sub_er_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_sub_er_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_16(ea);
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_sub_er_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_sub_er_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_16();
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_sub_er_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_sub_er_a_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_sub_er_ai_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_sub_er_pi_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_sub_er_pd_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_sub_er_di_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_sub_er_ix_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_sub_er_aw_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_sub_er_al_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_sub_er_pcdi_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_32(ea);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_sub_er_pcix_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_sub_er_i_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_32();
                uint dst = d_dst[0];
                uint res = d_dst[0] = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_sub_re_ai_8()
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


            static void m68000_sub_re_pi_8()
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


            static void m68000_sub_re_pi7_8()
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


            static void m68000_sub_re_pd_8()
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


            static void m68000_sub_re_pd7_8()
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


            static void m68000_sub_re_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
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


            static void m68000_sub_re_ix_8()
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


            static void m68000_sub_re_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
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


            static void m68000_sub_re_al_8()
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


            static void m68000_sub_re_ai_16()
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


            static void m68000_sub_re_pi_16()
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


            static void m68000_sub_re_pd_16()
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


            static void m68000_sub_re_di_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
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


            static void m68000_sub_re_ix_16()
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


            static void m68000_sub_re_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
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


            static void m68000_sub_re_al_16()
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


            static void m68000_sub_re_ai_32()
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


            static void m68000_sub_re_pi_32()
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


            static void m68000_sub_re_pd_32()
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


            static void m68000_sub_re_di_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
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


            static void m68000_sub_re_ix_32()
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


            static void m68000_sub_re_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
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


            static void m68000_sub_re_al_32()
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


            static void m68000_suba_d_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_suba_a_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_suba_ai_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_suba_pi_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_suba_pd_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))));
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_suba_di_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())))));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_suba_ix_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix())));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_suba_aw_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_suba_al_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32())));
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_suba_pcdi_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(ea)));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_suba_pcix_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix())));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_suba_i_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_suba_d_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_suba_a_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - (m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_suba_ai_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_suba_pi_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4)));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_suba_pd_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4)));
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_suba_di_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_suba_ix_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_suba_aw_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16())));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_suba_al_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_suba_pcdi_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                a_dst[0] = (a_dst[0] - m68ki_read_32(ea));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_suba_pcix_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_32(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_suba_i_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] - m68ki_read_imm_32());
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_subi_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_8();
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subi_ai_8()
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


            static void m68000_subi_pi_8()
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


            static void m68000_subi_pi7_8()
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


            static void m68000_subi_pd_8()
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


            static void m68000_subi_pd7_8()
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


            static void m68000_subi_di_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_subi_ix_8()
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


            static void m68000_subi_aw_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_subi_al_8()
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


            static void m68000_subi_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_16();
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subi_ai_16()
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


            static void m68000_subi_pi_16()
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


            static void m68000_subi_pd_16()
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


            static void m68000_subi_di_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_subi_ix_16()
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


            static void m68000_subi_aw_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_subi_al_16()
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


            static void m68000_subi_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_32();
                uint dst = d_dst[0];
                uint res = (dst - src);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (16);
            }


            static void m68000_subi_ai_32()
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


            static void m68000_subi_pi_32()
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


            static void m68000_subi_pd_32()
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


            static void m68000_subi_di_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_subi_ix_32()
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


            static void m68000_subi_aw_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_subi_al_32()
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


            static void m68000_subq_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_subq_ai_8()
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


            static void m68000_subq_pi_8()
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


            static void m68000_subq_pi7_8()
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


            static void m68000_subq_pd_8()
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


            static void m68000_subq_pd7_8()
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


            static void m68000_subq_di_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_subq_ix_8()
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


            static void m68000_subq_aw_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_subq_al_8()
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


            static void m68000_subq_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = ((dst - src) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_subq_a_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, m68k_cpu.ir & 7);

                a_dst[0] = (a_dst[0] - ((((m68k_cpu.ir >> 9) - 1) & 7) + 1));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subq_ai_16()
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


            static void m68000_subq_pi_16()
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


            static void m68000_subq_pd_16()
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


            static void m68000_subq_di_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_subq_ix_16()
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


            static void m68000_subq_aw_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_subq_al_16()
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


            static void m68000_subq_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = (dst - src);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subq_a_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, m68k_cpu.ir & 7);

                a_dst[0] = (a_dst[0] - ((((m68k_cpu.ir >> 9) - 1) & 7) + 1));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subq_ai_32()
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


            static void m68000_subq_pi_32()
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


            static void m68000_subq_pd_32()
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


            static void m68000_subq_di_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_subq_ix_32()
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


            static void m68000_subq_aw_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_subq_al_32()
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


            static void m68000_subx_rr_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_subx_rr_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_subx_rr_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = (dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_subx_mm_8_ax7()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_subx_mm_8_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_subx_mm_8_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_subx_mm_8()
            {
                uint src = m68ki_read_8(--((m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint ea = --((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_subx_mm_16()
            {
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_subx_mm_32()
            {
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (dst - src - ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_clks_left[0] -= (30);
            }


            static void m68000_swap()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);

                d_dst[0] ^= (d_dst[0] >> 16) & 0x0000ffff;
                d_dst[0] ^= (d_dst[0] << 16) & 0xffff0000;
                d_dst[0] ^= (d_dst[0] >> 16) & 0x0000ffff;

                m68k_cpu.n_flag = ((d_dst[0]) & 0x80000000);
                m68k_cpu.not_z_flag = d_dst[0];
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_tas_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, m68k_cpu.ir & 7);

                m68k_cpu.not_z_flag = ((d_dst[0]) & 0xff);
                m68k_cpu.n_flag = ((d_dst[0]) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                d_dst[0] |= 0x80;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_tas_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_tas_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_tas_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 4);
            }


            static void m68000_tas_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m68000_tas_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 6);
            }


            static void m68000_tas_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_tas_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 10);
            }


            static void m68000_tas_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 8);
            }


            static void m68000_tas_al()
            {
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = dst;
                m68k_cpu.n_flag = ((dst) & 0x80);
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68ki_write_8(ea, dst | 0x80);
                m68k_clks_left[0] -= (14 + 12);
            }


            static void m68000_trap()
            {
                m68ki_interrupt(32 + (m68k_cpu.ir & 0xf)); /* HJB 990403 */
            }


            static void m68020_trapt_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapt_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapt_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68ki_interrupt(7); /* HJB 990403 */
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapf_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapf_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68k_clks_left[0] -= (6);
                    m68k_cpu.pc += 2;
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapf_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    m68k_clks_left[0] -= (8);
                    m68k_cpu.pc += 4;
                    return;
                }
                m68000_illegal();
            }


            static void m68020_traphi_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_traphi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_traphi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapls_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapls_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapls_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapcc_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapcc_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapcc_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapcs_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapcs_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapcs_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapne_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapne_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapne_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapeq_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapeq_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapeq_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapvc_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.v_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapvc_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapvc_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapvs_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.v_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapvs_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapvs_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trappl_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.n_flag == 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trappl_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trappl_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapmi_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.n_flag != 0))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapmi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapmi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapge_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapge_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapge_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_traplt_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_traplt_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_traplt_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapgt_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_trapgt_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_trapgt_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_traple_0()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                        m68ki_interrupt(7); /* HJB 990403 */
                    else
                        m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_traple_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_traple_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_trapv()
            {
                if (m68k_cpu.v_flag == 0)
                {
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68ki_interrupt(7); /* HJB 990403 */
            }


            static void m68000_tst_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_tst_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_tst_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_tst_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_tst_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_tst_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_tst_di_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_tst_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = m68ki_read_8(ea);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68020_tst_pcdi_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint res = m68ki_read_8(ea);

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_tst_pcix_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_tst_imm_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_tst_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7])) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68020_tst_a_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_tst_ai_16()
            {
                uint res = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_tst_pi_16()
            {
                uint res = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_tst_pd_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_tst_di_16()
            {
                uint res = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_ix_16()
            {
                uint res = m68ki_read_16(m68ki_get_ea_ix());

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_tst_aw_16()
            {
                uint res = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_al_16()
            {
                uint res = m68ki_read_16(m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68020_tst_pcdi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint res = m68ki_read_16(ea);

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_tst_pcix_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_tst_imm_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_tst_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68020_tst_a_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_tst_ai_32()
            {
                uint res = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_pi_32()
            {
                uint res = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_tst_pd_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_tst_di_32()
            {
                uint res = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_tst_ix_32()
            {
                uint res = m68ki_read_32(m68ki_get_ea_ix());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 14);
            }


            static void m68000_tst_aw_32()
            {
                uint res = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_tst_al_32()
            {
                uint res = m68ki_read_32(m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_clks_left[0] -= (4 + 16);
            }


            static void m68020_tst_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint res = m68ki_read_32(ea);

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                    m68k_clks_left[0] -= (4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_tst_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_tst_imm_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68000_unlk_a7()
            {
                m68k_cpu.ar[7] = m68ki_read_32(m68k_cpu.ar[7]);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_unlk()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, m68k_cpu.ir & 7);

                m68k_cpu.ar[7] = a_dst[0];
                a_dst[0] = m68ki_read_32((m68k_cpu.ar[7] += 4) - 4);
                m68k_clks_left[0] -= (12);
            }


            static void m68020_unpk_rr()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);

                    (m68k_cpu.dr[m68k_cpu.ir & 7]) = (uint)(((m68k_cpu.dr[m68k_cpu.ir & 7])) & ~0xffff) | (((((src << 4) & 0x0f00) | (src & 0x000f)) + m68ki_read_imm_16()) & 0xffff);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_unpk_mm_ax7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_unpk_mm_ay7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_unpk_mm_axy7()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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


            static void m68020_unpk_mm()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
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

        }
    }
}