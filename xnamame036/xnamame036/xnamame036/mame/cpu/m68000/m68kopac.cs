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
            static void m68000_1010()
            {
                m68ki_exception(10);
            }
            static void m68000_1111()
            {
                m68ki_exception(11);
            }

            static void m68000_abcd_rr()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((src) & 0x0f) + ((dst) & 0x0f) + ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res += 6;
                res += ((src) & 0xf0) + ((dst) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0) != 0)
                    res -= 0xa0;

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | ((res) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_abcd_mm_ax7()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((src) & 0x0f) + ((dst) & 0x0f) + ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res += 6;
                res += ((src) & 0xf0) + ((dst) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res -= 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_abcd_mm_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src) & 0x0f) + ((dst) & 0x0f) + ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res += 6;
                res += ((src) & 0xf0) + ((dst) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res -= 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_abcd_mm_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = ((src) & 0x0f) + ((dst) & 0x0f) + ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res += 6;
                res += ((src) & 0xf0) + ((dst) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res -= 0xa0;

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_abcd_mm()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src) & 0x0f) + ((dst) & 0x0f) + ((m68k_cpu.x_flag != 0) ? 1u : 0u);

                if (res > 9)
                    res += 6;
                res += ((src) & 0xf0) + ((dst) & 0xf0);
                if ((m68k_cpu.x_flag = m68k_cpu.c_flag = (res > 0x99) ? 1u : 0u) != 0)
                    res -= 0xa0;

                m68k_cpu.n_flag = ((res) & 0x80); /* officially undefined */

                m68ki_write_8(ea, res);

                if (((res) & 0xff) != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_clks_left[0] -= (18);
            }


            static void m68000_add_er_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_add_er_ai_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_pi_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_pi7_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_pd_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_add_er_pd7_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_add_er_di_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (short)(m68ki_read_imm_16() & 0xffff)));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_ix_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_add_er_aw_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_al_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((int)((d_dst[0]) & ~0xff) | res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_add_er_pcdi_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_8(ea);
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_pcix_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_8((uint)m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_add_er_i_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_8();
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_add_er_a_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_add_er_ai_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_pi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_pd_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_add_er_di_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_ix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_add_er_aw_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_al_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_add_er_pcdi_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_16(ea);
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_add_er_pcix_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_16((uint)m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_add_er_i_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_16();
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_add_er_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_add_er_a_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_add_er_ai_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_add_er_pi_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_add_er_pd_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_add_er_di_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_add_er_ix_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(m68ki_get_ea_ix());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_add_er_aw_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_add_er_al_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32(m68ki_read_imm_32());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_add_er_pcdi_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_32(ea);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_add_er_pcix_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_32((uint)m68ki_get_ea_pcix());
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_add_er_i_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = m68ki_read_imm_32();
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_add_re_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_add_re_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_add_re_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_add_re_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_add_re_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_add_re_di_8()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_add_re_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_add_re_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_add_re_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_add_re_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_add_re_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_add_re_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_add_re_di_16()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_add_re_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_add_re_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_add_re_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_add_re_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_add_re_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_add_re_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_add_re_di_32()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_add_re_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_add_re_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_add_re_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint src = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_adda_d_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (uint)(a_dst[0] + MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_adda_a_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_adda_ai_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]))));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_adda_pi_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2))));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_adda_pd_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2))));
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_adda_di_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())))));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_adda_ix_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix())));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_adda_aw_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_adda_al_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16((uint)m68ki_read_16(m68ki_read_imm_32())));
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_adda_pcdi_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16(ea)));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_adda_pcix_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_16((uint)m68ki_get_ea_pcix())));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_adda_i_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_adda_d_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (m68k_cpu.dr[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_adda_a_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + (m68k_cpu.ar[m68k_cpu.ir & 7]));
                m68k_clks_left[0] -= (8);
            }


            static void m68000_adda_ai_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7])));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_adda_pi_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4)));
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_adda_pd_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4)));
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_adda_di_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()))));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_adda_ix_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32(m68ki_get_ea_ix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_adda_aw_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16())));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_adda_al_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32(m68ki_read_imm_32()));
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_adda_pcdi_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                a_dst[0] = (a_dst[0] + m68ki_read_32(ea));
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_adda_pcix_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_32(m68ki_get_ea_pcix()));
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_adda_i_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)(m68k_cpu.ir >> 9) & 7);

                a_dst[0] = (a_dst[0] + m68ki_read_imm_32());
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_addi_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_8();
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_addi_ai_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_addi_pi_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_addi_pi7_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_addi_pd_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_addi_pd7_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_addi_di_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (uint)(((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addi_ix_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_addi_aw_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addi_al_8()
            {
                uint src = m68ki_read_imm_8();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_addi_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_16();
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_addi_ai_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_addi_pi_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_addi_pd_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_addi_di_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addi_ix_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_addi_aw_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addi_al_16()
            {
                uint src = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_addi_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = m68ki_read_imm_32();
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (16);
            }


            static void m68000_addi_ai_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_addi_pi_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_addi_pd_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m68000_addi_di_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_addi_ix_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68000_addi_aw_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_addi_al_32()
            {
                uint src = m68ki_read_imm_32();
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68000_addq_d_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_addq_ai_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_addq_pi_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_addq_pi7_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_addq_pd_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_addq_pd7_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_addq_di_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_addq_ix_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_addq_aw_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_addq_al_8()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_8(ea);
                uint res = ((src + dst) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_addq_d_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = ((src + dst) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_addq_a_16()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)m68k_cpu.ir & 7);

                a_dst[0] = (a_dst[0] + (((m68k_cpu.ir >> 9) - 1) & 7) + 1);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_addq_ai_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_addq_pi_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_addq_pd_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_addq_di_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_addq_ix_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_addq_aw_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_addq_al_16()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_16(ea);
                uint res = ((src + dst) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_addq_d_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint dst = d_dst[0];
                uint res = d_dst[0] = (src + dst);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_addq_a_32()
            {
                UIntSubArray a_dst = new UIntSubArray(m68k_cpu.ar, (int)m68k_cpu.ir & 7);

                a_dst[0] = (a_dst[0] + (((m68k_cpu.ir >> 9) - 1) & 7) + 1);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_addq_ai_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addq_pi_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_addq_pd_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_addq_di_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_addq_ix_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_get_ea_ix();
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_addq_aw_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_addq_al_32()
            {
                uint src = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint ea = m68ki_read_imm_32();
                uint dst = m68ki_read_32(ea);
                uint res = (src + dst);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_addx_rr_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_addx_rr_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_addx_rr_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)(m68k_cpu.ir >> 9) & 7);
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = d_dst[0];
                uint res = d_dst[0] = (uint)(src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_addx_mm_8_ax7()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_addx_mm_8_ay7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_addx_mm_8_axy7()
            {
                uint src = m68ki_read_8(m68k_cpu.ar[7] -= 2);
                uint ea = m68k_cpu.ar[7] -= 2;
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_addx_mm_8()
            {
                uint src = m68ki_read_8(--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint ea = --(m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint dst = m68ki_read_8(ea);
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_addx_mm_16()
            {
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 2);
                uint dst = m68ki_read_16(ea);
                uint res = (uint)((src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x8000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x8000);
                m68k_clks_left[0] -= (18);
            }


            static void m68000_addx_mm_32()
            {
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint ea = ((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) -= 4);
                uint dst = m68ki_read_32(ea);
                uint res = (uint)(src + dst + ((m68k_cpu.x_flag != 0) ? 1u : 0u));

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                if (res != 0)
                    m68k_cpu.not_z_flag = 1;
                m68k_cpu.v_flag = (((src & dst & ~res) | (~src & ~dst & res)) & 0x80000000);
                m68k_cpu.x_flag = m68k_cpu.c_flag = (((src & dst) | (~res & dst) | (src & ~res)) & 0x80000000);
                m68k_clks_left[0] -= (30);
            }


            static void m68000_and_er_d_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= ((m68k_cpu.dr[m68k_cpu.ir & 7]) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_and_er_ai_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_pi_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++)) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_pi7_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2)) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_pd_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7]))) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_and_er_pd7_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8((m68k_cpu.ar[7] -= 2)) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_and_er_di_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_ix_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(m68ki_get_ea_ix()) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_and_er_aw_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16())) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_al_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(m68ki_read_imm_32()) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_and_er_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = (uint)(old_pc + MAKE_INT_16(m68ki_read_16(old_pc)));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(ea) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_pcix_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_8(m68ki_get_ea_pcix()) | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_and_er_i_8()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_imm_8() | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_d_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= ((m68k_cpu.dr[m68k_cpu.ir & 7]) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_and_er_ai_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_pi_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_pd_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_and_er_di_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()))) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_ix_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16(m68ki_get_ea_ix()) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_and_er_aw_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_al_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16(m68ki_read_imm_32()) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_and_er_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = (uint)(old_pc + MAKE_INT_16(m68ki_read_16(old_pc)));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16(ea) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_and_er_pcix_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_16(m68ki_get_ea_pcix()) | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_and_er_i_16()
            {
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68ki_read_imm_16() | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_and_er_d_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= (m68k_cpu.dr[m68k_cpu.ir & 7]);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_and_er_ai_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_and_er_pi_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_and_er_pd_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_and_er_di_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_and_er_ix_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32(m68ki_get_ea_ix());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_and_er_aw_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_and_er_al_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32(m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_and_er_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = (uint)(old_pc + MAKE_INT_16(m68ki_read_16(old_pc)));
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32(ea);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_and_er_pcix_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_32(m68ki_get_ea_pcix());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_and_er_i_32()
            {
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) &= m68ki_read_imm_32();

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_and_re_ai_8()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_and_re_pi_8()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_and_re_pi7_8()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_and_re_pd_8()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_and_re_pd7_8()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_and_re_di_8()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_and_re_ix_8()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_and_re_aw_8()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_and_re_al_8()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_8(ea)) & 0xff);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_and_re_ai_16()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_and_re_pi_16()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_and_re_pd_16()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_and_re_di_16()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_and_re_ix_16()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_and_re_aw_16()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_and_re_al_16()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_16(ea)) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_and_re_ai_32()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_and_re_pi_32()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_and_re_pd_32()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_and_re_di_32()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_and_re_ix_32()
            {
                uint ea = m68ki_get_ea_ix();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_and_re_aw_32()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_and_re_al_32()
            {
                uint ea = m68ki_read_imm_32();
                uint res = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_andi_d_8()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) &= (m68ki_read_imm_8() | 0xffffff00)) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_andi_ai_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_andi_pi_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_andi_pi7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_andi_pd_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_andi_pd7_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_andi_di_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_andi_ix_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_andi_aw_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_andi_al_8()
            {
                uint tmp = m68ki_read_imm_8();
                uint ea = m68ki_read_imm_32();
                uint res = tmp & m68ki_read_8(ea);

                m68ki_write_8(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_andi_d_16()
            {
                uint res = (((m68k_cpu.dr[m68k_cpu.ir & 7]) &= (m68ki_read_imm_16() | 0xffff0000)) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_andi_ai_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_andi_pi_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_andi_pd_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_andi_di_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_andi_ix_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_andi_aw_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_andi_al_16()
            {
                uint tmp = m68ki_read_imm_16();
                uint ea = m68ki_read_imm_32();
                uint res = tmp & m68ki_read_16(ea);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_andi_d_32()
            {
                uint res = (m68k_cpu.dr[m68k_cpu.ir & 7]) &= (m68ki_read_imm_32());

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_andi_ai_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_andi_pi_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68000_andi_pd_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 10);
            }


            static void m68000_andi_di_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_andi_ix_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_get_ea_ix();
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68000_andi_aw_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68000_andi_al_32()
            {
                uint tmp = m68ki_read_imm_32();
                uint ea = m68ki_read_imm_32();
                uint res = tmp & m68ki_read_32(ea);

                m68ki_write_32(ea, res);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.c_flag = m68k_cpu.v_flag = 0;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68000_andi_to_ccr()
            {
                m68ki_set_ccr((((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)) & m68ki_read_imm_16());
                m68k_clks_left[0] -= (20);
            }


            static void m68000_andi_to_sr()
            {
                uint and_val = m68ki_read_imm_16();

                if (m68k_cpu.s_flag != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_set_sr((((m68k_cpu.t1_flag != 0) ? 1u : 0u << 15) | ((m68k_cpu.t0_flag != 0) ? 1u : 0u << 14) | ((m68k_cpu.s_flag != 0) ? 1u : 0u << 13) | ((m68k_cpu.m_flag != 0) ? 1u : 0u << 12) | (m68k_cpu.int_mask << 8) | ((m68k_cpu.x_flag != 0) ? 1u : 0u << 4) | ((m68k_cpu.n_flag != 0) ? 1u : 0u << 3) | ((m68k_cpu.not_z_flag == 0) ? 1u : 0u << 2) | ((m68k_cpu.v_flag != 0) ? 1u : 0u << 1) | ((m68k_cpu.c_flag != 0) ? 1u : 0u)) & and_val);
                    m68k_clks_left[0] -= (20);
                    return;
                }
                m68ki_exception(8);
            }


            static void m68000_asr_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint res = (uint)src >> (int)shift;

                if (((src) & 0x80) != 0)
                    res |= m68k_shift_8_table[shift];

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.x_flag = m68k_cpu.c_flag = shift > 7 ? m68k_cpu.n_flag : (src >> (int)(shift - 1)) & 1;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_asr_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = src >> (int)shift;

                if (((src) & 0x8000) != 0)
                    res |= m68k_shift_16_table[shift];

                d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_asr_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = (d_dst[0]);
                uint res = src >> (int)shift;

                if (((src) & 0x80000000) != 0)
                    res |= m68k_shift_32_table[shift];

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(shift - 1)) & 1;
                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_asr_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xff);
                uint res = src >>(int) shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift < 8)
                    {
                        if (((src) & 0x80) != 0)
                            res |= m68k_shift_8_table[shift];

                        d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                        m68k_cpu.c_flag = m68k_cpu.x_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    if (((src) & 0x80) != 0)
                    {
                        d_dst[0] |= 0xff;
                        m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                        m68k_cpu.n_flag = 1;
                        m68k_cpu.not_z_flag = 1;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffffff00;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
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


            static void m68000_asr_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = src >>(int) shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift < 16)
                    {
                        if (((src) & 0x8000) != 0)
                            res |= m68k_shift_16_table[shift];

                        d_dst[0] = (uint)((d_dst[0]) & ~0xffff) | res;

                        m68k_cpu.c_flag = m68k_cpu.x_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x8000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    if (((src) & 0x8000) != 0)
                    {
                        d_dst[0] |= 0xffff;
                        m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                        m68k_cpu.n_flag = 1;
                        m68k_cpu.not_z_flag = 1;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] &= 0xffff0000;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
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


            static void m68000_asr_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = (d_dst[0]);
                uint res = src >> (int)shift;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        if (((src) & 0x80000000) != 0)
                            res |= m68k_shift_32_table[shift];

                        d_dst[0] = res;

                        m68k_cpu.c_flag = m68k_cpu.x_flag = (src >> (int)(shift - 1)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80000000);
                        m68k_cpu.not_z_flag = res;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    if (((src) & 0x80000000) != 0)
                    {
                        d_dst[0] = 0xffffffff;
                        m68k_cpu.c_flag = m68k_cpu.x_flag = 1;
                        m68k_cpu.n_flag = 1;
                        m68k_cpu.not_z_flag = 1;
                        m68k_cpu.v_flag = 0;
                        return;
                    }

                    d_dst[0] = 0;
                    m68k_cpu.c_flag = m68k_cpu.x_flag = 0;
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


            static void m68000_asr_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_asr_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_asr_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_asr_ea_di()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_asr_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_asr_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_asr_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = src >> 1;

                if (((src) & 0x8000) != 0)
                    res |= 0x8000;

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
                m68k_cpu.c_flag = m68k_cpu.x_flag = src & 1;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_asl_s_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xff);
                uint res = (uint)((src <<(int) shift) & 0xff);

                d_dst[0] = (uint)((d_dst[0]) & ~0xff) | res;

                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >> (int)(8 - shift)) & 1;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                src &= m68k_shift_8_table[shift + 1];
                m68k_cpu.v_flag = !(src == 0 || (src == m68k_shift_8_table[shift + 1] && shift < 8)) ? 1u : 0u;

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
            }


            static void m68000_asl_s_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((src << (int)shift) & 0xffff);

                d_dst[0] =(uint) ((d_dst[0]) & ~0xffff) | res;

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (16 - shift)) & 1;
                src &= m68k_shift_16_table[shift + 1];
                m68k_cpu.v_flag = !(src == 0 || src == m68k_shift_16_table[shift + 1]) ? 1u : 0u;

                m68k_clks_left[0] -=(int) ((shift << 1) + 6);
            }


            static void m68000_asl_s_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (((m68k_cpu.ir >> 9) - 1) & 7) + 1;
                uint src = d_dst[0];
                uint res = (uint)(src <<(int)shift);

                d_dst[0] = res;

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (32 - shift)) & 1;
                src &= m68k_shift_32_table[shift + 1];
                m68k_cpu.v_flag = !(src == 0 || src == m68k_shift_32_table[shift + 1]) ? 1u : 0u;

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
            }


            static void m68000_asl_r_8()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xff);
                uint res = ((src <<(int) shift) & 0xff);

                m68k_clks_left[0] -= (int)((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift < 8)
                    {
                        d_dst[0] =(uint) ((d_dst[0]) & ~0xff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (8 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80);
                        m68k_cpu.not_z_flag = res;
                        src &= m68k_shift_8_table[shift + 1];
                        m68k_cpu.v_flag = !(src == 0 || src == m68k_shift_8_table[shift + 1]) ? 1u : 0u;
                        return;
                    }

                    d_dst[0] &= 0xffffff00;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = (shift == 8 ? src & 1 : 0);
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = !(src == 0) ? 1u : 0u;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m68000_asl_r_16()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = ((d_dst[0]) & 0xffff);
                uint res = ((src << (int)shift) & 0xffff);

                m68k_clks_left[0] -=(int) ((shift << 1) + 6);
                if (shift != 0)
                {
                    if (shift < 16)
                    {
                        d_dst[0] =(uint) ((d_dst[0]) & ~0xffff) | res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (16 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x8000);
                        m68k_cpu.not_z_flag = res;
                        src &= m68k_shift_16_table[shift + 1];
                        m68k_cpu.v_flag = !(src == 0 || src == m68k_shift_16_table[shift + 1]) ? 1u : 0u;
                        return;
                    }

                    d_dst[0] &= 0xffff0000;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = (shift == 16 ? src & 1 : 0);
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = (!(src == 0) ? 1u : 0u);
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m68000_asl_r_32()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint shift = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x3f;
                uint src = d_dst[0];
                uint res = (src << (int)shift);

                m68k_clks_left[0] -= (int)((shift << 1) + 8);
                if (shift != 0)
                {
                    if (shift < 32)
                    {
                        d_dst[0] = res;
                        m68k_cpu.x_flag = m68k_cpu.c_flag = (src >>(int) (32 - shift)) & 1;
                        m68k_cpu.n_flag = ((res) & 0x80000000);
                        m68k_cpu.not_z_flag = res;
                        src &= m68k_shift_32_table[shift + 1];
                        m68k_cpu.v_flag = !(src == 0 || src == m68k_shift_32_table[shift + 1]) ? 1u : 0u;
                        return;
                    }

                    d_dst[0] = 0;
                    m68k_cpu.x_flag = m68k_cpu.c_flag = (shift == 32 ? src & 1 : 0);
                    m68k_cpu.n_flag = 0;
                    m68k_cpu.not_z_flag = 0;
                    m68k_cpu.v_flag = !(src == 0) ? 1u : 0u;
                    return;
                }

                m68k_cpu.c_flag = 0;
                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = 0;
            }


            static void m68000_asl_ea_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_asl_ea_pi()
            {
                uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_asl_ea_pd()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_asl_ea_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) +(uint) MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_asl_ea_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000) ? 1u : 0u;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_asl_ea_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000)?1u:0u;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_asl_ea_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_16(ea);
                uint res = ((src << 1) & 0xffff);

                m68ki_write_16(ea, res);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.x_flag = m68k_cpu.c_flag = ((src) & 0x8000);
                src &= 0xc000;
                m68k_cpu.v_flag = !(src == 0 || src == 0xc000)?1u:0u;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_bhi_8()
            {
                if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bhi_16()
            {
                if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bhi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag == 0 && m68k_cpu.not_z_flag != 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bls_8()
            {
                if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bls_16()
            {
                if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bls_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag != 0 || m68k_cpu.not_z_flag == 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bcc_8()
            {
                if ((m68k_cpu.c_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bcc_16()
            {
                if ((m68k_cpu.c_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bcc_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag == 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bcs_8()
            {
                if ((m68k_cpu.c_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bcs_16()
            {
                if ((m68k_cpu.c_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bcs_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.c_flag != 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bne_8()
            {
                if ((m68k_cpu.not_z_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bne_16()
            {
                if ((m68k_cpu.not_z_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bne_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag != 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_beq_8()
            {
                if ((m68k_cpu.not_z_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_beq_16()
            {
                if ((m68k_cpu.not_z_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_beq_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag == 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bvc_8()
            {
                if ((m68k_cpu.v_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bvc_16()
            {
                if ((m68k_cpu.v_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bvc_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.v_flag == 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bvs_8()
            {
                if ((m68k_cpu.v_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bvs_16()
            {
                if ((m68k_cpu.v_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bvs_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.v_flag != 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bpl_8()
            {
                if ((m68k_cpu.n_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bpl_16()
            {
                if ((m68k_cpu.n_flag == 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bpl_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.n_flag == 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bmi_8()
            {
                if ((m68k_cpu.n_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bmi_16()
            {
                if ((m68k_cpu.n_flag != 0))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bmi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.n_flag != 0))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bge_8()
            {
                if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bge_16()
            {
                if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bge_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if (((m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_blt_8()
            {
                if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_blt_16()
            {
                if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_blt_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if (((m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bgt_8()
            {
                if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bgt_16()
            {
                if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bgt_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag != 0 && (m68k_cpu.n_flag == 0) == (m68k_cpu.v_flag == 0)))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_ble_8()
            {
                if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_clks_left[0] -= (8);
            }


            static void m68000_ble_16()
            {
                if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.pc += 2;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_ble_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    if ((m68k_cpu.not_z_flag == 0 || (m68k_cpu.n_flag == 0) != (m68k_cpu.v_flag == 0)))
                    {
                        ; /* auto-disable (see m68kcpu.h) */
                        m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                        m68k_clks_left[0] -= (6);
                        return;
                    }
                    m68k_cpu.pc += 4;
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bchg_r_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] ^= mask;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bchg_r_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bchg_r_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bchg_r_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bchg_r_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bchg_r_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bchg_r_di()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bchg_r_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_bchg_r_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bchg_r_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_bchg_s_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u << (int)(m68ki_read_imm_8() & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] ^= mask;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_bchg_s_ai()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bchg_s_pi()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bchg_s_pi7()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bchg_s_pd()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bchg_s_pd7()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bchg_s_di()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bchg_s_ix()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_bchg_s_aw()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bchg_s_al()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src ^ mask);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_bclr_r_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] &= ~mask;
                m68k_clks_left[0] -= (10);
            }


            static void m68000_bclr_r_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bclr_r_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bclr_r_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bclr_r_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bclr_r_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bclr_r_di()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bclr_r_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_bclr_r_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bclr_r_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_bclr_s_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u << (int)(m68ki_read_imm_8() & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] &= ~mask;
                m68k_clks_left[0] -= (14);
            }


            static void m68000_bclr_s_ai()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bclr_s_pi()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bclr_s_pi7()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bclr_s_pd()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bclr_s_pd7()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bclr_s_di()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bclr_s_ix()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_bclr_s_aw()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bclr_s_al()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src & ~mask);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68020_bfchg_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);
                uint mask = (uint)(0xffffffff << (int)(32 - width));

                (m68k_cpu.dr[m68k_cpu.ir & 7]) ^= mask;

                m68k_cpu.n_flag = (data >>(int) (width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bfchg_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
 
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) ^ (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) ^ (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68020_bfchg_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base =(uint) ((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) ^ (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) ^ (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfchg_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) ^ (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) ^ (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68020_bfchg_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base =(uint) MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) ^ (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) ^ (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfchg_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) ^ (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) ^ (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68020_bfclr_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= ~mask;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bfclr_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68020_bfclr_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) +(uint) MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfclr_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68020_bfclr_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfclr_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68020_bfexts_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (8);
            }


            static void m68020_bfexts_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 8);
            }


            static void m68020_bfexts_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) +(uint) MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfexts_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 14);
            }


            static void m68020_bfexts_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base =(uint) MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfexts_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 16);
            }


            static void m68020_bfexts_pcdi()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint _base = ea + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfexts_pcix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_pcix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                if ((m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1)!=0)
                    data |= (0xffffffff << (int)width);
                m68k_cpu.not_z_flag = data;

                m68k_cpu.dr[(word2 >> 12) & 7] = data;
                m68k_clks_left[0] -= (15 + 14);
            }


            static void m68020_bfextu_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (8);
            }


            static void m68020_bfextu_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 8);
            }


            static void m68020_bfextu_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfextu_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 14);
            }


            static void m68020_bfextu_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfextu_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 16);
            }


            static void m68020_bfextu_pcdi()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint _base = ea + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 12);
            }


            static void m68020_bfextu_pcix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_pcix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.dr[(word2 >> 12) & 7] = data;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (15 + 14);
            }


            static void m68020_bfffo_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);
                uint mask = 1u << (int)(width - 1);

                for (; mask != 0 && (data & mask) == 0; mask >>= 1, offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (18);
            }


            static void m68020_bfffo_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = 1u << (int)(width - 1);

                for (; mask!=0 && (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 8);
            }


            static void m68020_bfffo_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = 1u << (int)(int)(width - 1);

                for (; mask!=0 && (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 12);
            }


            static void m68020_bfffo_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = 1u << (int)(width - 1);

                for (; mask !=0&& (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 14);
            }


            static void m68020_bfffo_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = 1u << (int)(width - 1);

                for (; mask !=0&& (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 12);
            }


            static void m68020_bfffo_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);
                uint mask = 1u << (int)(width - 1);

                for (; mask !=0&& (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 16);
            }


            static void m68020_bfffo_pcdi()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint _base = ea + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = 1u << (int)(width - 1);

                for (; mask !=0&& (data & mask)==0; mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 12);
            }


            static void m68020_bfffo_pcix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_pcix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >>(int) (32- width);
                uint mask = 1u << (int)(width - 1);

                for (; mask !=0&& ((data & mask)==0); mask >>= 1, full_offset++)
                    ;
                m68k_cpu.dr[(word2 >> 12) & 7] = full_offset;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (28 + 14);
            }


            static void m68020_bfins_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint orig_insert = insert >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                insert = (((offset) < 32 ? (insert) >> (int)(offset) : 0) | ((32 - (offset)) < 32 ? (insert) << (int)(32 - (offset)) : 0));
                mask = ~(((offset) < 32 ? (mask) >> (int)(offset) : 0) | ((32 - (offset)) < 32 ? (mask) << (int)(32 - (offset)) : 0));

                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= mask;
                (m68k_cpu.dr[m68k_cpu.ir & 7]) |= insert;

                m68k_cpu.n_flag = orig_insert >> (int)(width - 1);
                m68k_cpu.not_z_flag = orig_insert;
                m68k_clks_left[0] -= (10);
            }


            static void m68020_bfins_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)) | (insert >> (int)offset));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))) | ((insert << (int)(8 - offset)) & 0xff));

                m68k_cpu.n_flag = ((insert) & 0x80000000);
                m68k_cpu.not_z_flag = insert;
                m68k_clks_left[0] -= (17 + 8);
            }


            static void m68020_bfins_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)) | (insert >> (int)offset));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))) | ((insert << (int)(8 - offset)) & 0xff));

                m68k_cpu.n_flag = ((insert) & 0x80000000);
                m68k_cpu.not_z_flag = insert;
                m68k_clks_left[0] -= (17 + 12);
            }


            static void m68020_bfins_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)) | (insert >> (int)offset));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))) | ((insert << (int)(8 - offset)) & 0xff));

                m68k_cpu.n_flag = ((insert) & 0x80000000);
                m68k_cpu.not_z_flag = insert;
                m68k_clks_left[0] -= (17 + 14);
            }


            static void m68020_bfins_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)) | (insert >> (int)offset));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))) | ((insert << (int)(8 - offset)) & 0xff));

                m68k_cpu.n_flag = ((insert) & 0x80000000);
                m68k_cpu.not_z_flag = insert;
                m68k_clks_left[0] -= (17 + 12);
            }


            static void m68020_bfins_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint insert = (m68k_cpu.dr[(word2 >> 12) & 7] << (int)(32 - width));
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) & ~(mask >> (int)offset)) | (insert >> (int)offset));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) & ~(mask << (int)(8 - offset))) | ((insert << (int)(8 - offset)) & 0xff));

                m68k_cpu.n_flag = ((insert) & 0x80000000);
                m68k_cpu.not_z_flag = insert;
                m68k_clks_left[0] -= (17 + 16);
            }


            static void m68020_bfset_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data =( (((offset) < 32) ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) << (int)(offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >> (int)(32 - width);
                uint mask = (0xffffffff << (int)(32 - width));

                (m68k_cpu.dr[m68k_cpu.ir & 7]) |= mask;

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (12);
            }


            static void m68020_bfset_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) | (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) | (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 8);
            }


            static void m68020_bfset_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) | (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) | (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfset_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >>(int) (32- width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) | (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) | (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 14);
            }


            static void m68020_bfset_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) | (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) | (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 12);
            }


            static void m68020_bfset_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);
                uint mask = (0xffffffff << (int)(32 - width));

                m68ki_write_32(_base, (m68ki_read_32(_base) | (mask >> (int)offset)));
                if ((width + offset) > 32)
                    m68ki_write_8(_base + 4, (m68ki_read_8(_base + 4) | (mask << (int)(8 - offset))));

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (20 + 16);
            }


            static void m68020_bftst_d()
            {
                uint word2 = m68ki_read_imm_16();
                uint offset = (((word2) & 0x00000800) != 0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : word2 >> 6) & 31;
                uint width = (((((word2) & 0x00000020) != 0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = (((offset) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) <<(int) (offset) : 0) | ((32 - (offset)) < 32 ? ((m68k_cpu.dr[m68k_cpu.ir & 7])) >> (int)(32 - (offset)) : 0)) >>(int) (32 - width);

                /* if offset + width > 32, wraps around in register */

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (6);
            }


            static void m68020_bftst_ai()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (m68k_cpu.ar[m68k_cpu.ir & 7]) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 8);
            }


            static void m68020_bftst_di()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 12);
            }


            static void m68020_bftst_ix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_ix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 14);
            }


            static void m68020_bftst_aw()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = (uint)MAKE_INT_16(m68ki_read_imm_16()) + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 12);
            }


            static void m68020_bftst_al()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_read_imm_32() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020) !=0? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 16);
            }


            static void m68020_bftst_pcdi()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800) !=0? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = (uint)old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint _base = ea + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32- width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 12);
            }


            static void m68020_bftst_pcix()
            {
                uint word2 = m68ki_read_imm_16();
                uint full_offset = ((word2) & 0x00000800)!=0 ? (uint)MAKE_INT_32(m68k_cpu.dr[(word2 >> 6) & 7]) : (word2 >> 6) & 31;
                uint _base = m68ki_get_ea_pcix() + (full_offset >> 3);
                uint offset = full_offset & 7;
                uint width = (((((word2) & 0x00000020)!=0 ? m68k_cpu.dr[word2 & 7] : word2) - 1) & 31) + 1;
                uint data = ((m68ki_read_32(_base) << (int)offset) | ((offset + width > 32) ? m68ki_read_8(_base + 4) >> (int)(8 - offset) : 0)) >> (int)(32 - width);

                m68k_cpu.n_flag = (data >> (int)(width - 1)) & 1;
                m68k_cpu.not_z_flag = data;
                m68k_clks_left[0] -= (13 + 14);
            }


            static void m68010_bkpt()
            {
                if ((m68k_cpu.mode & (2 | 4 | 8)) != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_clks_left[0] -= (11);
                }
                m68000_illegal();
            }


            static void m68000_bra_8()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                m68k_clks_left[0] -= (10);
            }


            static void m68000_bra_16()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                m68k_clks_left[0] -= (10);
            }


            static void m68020_bra_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                    m68k_clks_left[0] -= (6);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_bset_r_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] |= mask;
                m68k_clks_left[0] -= (8);
            }


            static void m68000_bset_r_ai()
            {
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bset_r_pi()
            {
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bset_r_pi7()
            {
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_bset_r_pd()
            {
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bset_r_pd7()
            {
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_bset_r_di()
            {
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bset_r_ix()
            {
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_bset_r_aw()
            {
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);
                uint mask = 1u << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_bset_r_al()
            {
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);
                uint mask = 1u <<(int) ((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_bset_s_d()
            {
                UIntSubArray d_dst = new UIntSubArray(m68k_cpu.dr, (int)m68k_cpu.ir & 7);
                uint mask = 1u << (int)(m68ki_read_imm_8() & 0x1f);

                m68k_cpu.not_z_flag = d_dst[0] & mask;
                d_dst[0] |= mask;
                m68k_clks_left[0] -= (12);
            }


            static void m68000_bset_s_ai()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bset_s_pi()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bset_s_pi7()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 4);
            }


            static void m68000_bset_s_pd()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bset_s_pd7()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = (m68k_cpu.ar[7] -= 2);
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 6);
            }


            static void m68000_bset_s_di()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bset_s_ix()
            {
                uint mask = 1u <<(int) (m68ki_read_imm_8() & 7);
                uint ea = m68ki_get_ea_ix();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_bset_s_aw()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_bset_s_al()
            {
                uint mask = 1u << (int)(m68ki_read_imm_8() & 7);
                uint ea = m68ki_read_imm_32();
                uint src = m68ki_read_8(ea);

                m68k_cpu.not_z_flag = src & mask;
                m68ki_write_8(ea, src | mask);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_bsr_8()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc);
                m68k_cpu.pc += (uint)MAKE_INT_8((int)((m68k_cpu.ir) & 0xff));
                m68k_clks_left[0] -= (18);
            }


            static void m68000_bsr_16()
            {
                ; /* auto-disable (see m68kcpu.h) */
                m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc + 2);
                m68k_cpu.pc += (uint)MAKE_INT_16(m68ki_read_16(m68k_cpu.pc));
                m68k_clks_left[0] -= (18);
            }


            static void m68020_bsr_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    ; /* auto-disable (see m68kcpu.h) */
                    m68ki_write_32(m68k_cpu.ar[7] -= 4, m68k_cpu.pc + 4);
                    m68k_cpu.pc += (m68ki_read_32(m68k_cpu.pc));
                    m68k_clks_left[0] -= (7);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_btst_r_d()
            {
                m68k_cpu.not_z_flag = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 0x1f)));
                m68k_clks_left[0] -= (6);
            }


            static void m68000_btst_r_ai()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_btst_r_pi()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++)) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_btst_r_pi7()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2)) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_btst_r_pd()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7]))) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_btst_r_pd7()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((m68k_cpu.ar[7] -= 2)) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_btst_r_di()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()))) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_btst_r_ix()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_get_ea_ix()) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_btst_r_aw()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16())) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_btst_r_al()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_read_imm_32()) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 12);
            }

            static uint m68ki_get_ea_pcix()
{
   uint _base = (m68k_cpu.pc+=2) - 2;
   uint extension = m68ki_read_16(_base);
   uint ea_index = m68k_cpu_dar[(((extension) & 0x00008000)!=0)?1u:0u][(int)(((extension)>>12)&7)];
   uint outer = 0;

   /* Sign-extend the index value if needed */
   if(((extension) & 0x00000800)==0)
      ea_index = (uint)MAKE_INT_16(ea_index);

   /* If we're running 010 or less, there's no scale or full extension word mode */
   if((m68k_cpu.mode & (1 | 2))!=0)
       return _base + ea_index + (uint)MAKE_INT_8((int)extension);

   /* Scale the index value */
   ea_index <<= (int)(((extension)>>9)&3);

   /* If we're using brief extension mode, we are done */
   if(((extension) & 0x00000100)==0)
       return _base + ea_index + (uint)MAKE_INT_8((int)extension);

   /* Decode the long extension format */
   if(((extension) & 0x00000040)!=0)
      ea_index = 0;
   if (((extension) & 0x00000080) != 0)
      _base = 0;
   if (((extension) & 0x00000020) != 0)
       _base += ((extension) & 0x00000010) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
   if(((extension)&7)==0)
      return _base + ea_index;

   if (((extension) & 0x00000002) != 0)
       outer = ((extension) & 0x00000001) != 0 ? m68ki_read_imm_32() : (uint)MAKE_INT_16(m68ki_read_imm_16());
   if(((extension) & 0x00000004)!=0)
      return m68ki_read_32(_base) + ea_index + outer;
   return m68ki_read_32(_base + ea_index) + outer;
}
            static void m68000_btst_r_pcdi()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(ea) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_btst_r_pcix()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_get_ea_pcix()) & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_btst_r_i()
            {
                m68k_cpu.not_z_flag = (uint)(m68ki_read_imm_8() & (1 << (int)((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]) & 7)));
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_btst_s_d()
            {
                m68k_cpu.not_z_flag = (uint)((m68k_cpu.dr[m68k_cpu.ir & 7]) & (1 << (int)(m68ki_read_imm_8() & 0x1f)));
                m68k_clks_left[0] -= (10);
            }


            static void m68000_btst_s_ai()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_btst_s_pi()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++)) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_btst_s_pi7()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2)) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_btst_s_pd()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7]))) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_btst_s_pd7()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((m68k_cpu.ar[7] -= 2)) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_btst_s_di()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag =(uint)( m68ki_read_8((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_btst_s_ix()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_get_ea_ix()) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_btst_s_aw()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16())) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_btst_s_al()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_read_imm_32()) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_btst_s_pcdi()
            {
                uint bit = m68ki_read_imm_8() & 7;

                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16((uint)m68ki_read_16(old_pc));
                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(ea) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_btst_s_pcix()
            {
                uint bit = m68ki_read_imm_8() & 7;

                m68k_cpu.not_z_flag = (uint)(m68ki_read_8(m68ki_get_ea_pcix()) & (1 << (int)bit));
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68020_callm_ai()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_di()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_ix()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = m68ki_get_ea_ix();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_aw()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_al()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = m68ki_read_imm_32();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 16);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_pcdi()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_callm_pcix()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint ea = m68ki_get_ea_pcix();

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.pc += 2;
                    //()ea; /* just to avoid an 'unused variable' warning */
                    ;


                    m68k_clks_left[0] -= (30 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ai_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr, (int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag != 0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pi_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pi7_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[7] += 2) - 2);
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pd_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (--(m68k_cpu.ar[m68k_cpu.ir & 7]));
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pd7_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[7] -= 2);
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_di_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ix_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_aw_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_al_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();
                    uint dst = m68ki_read_8(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xff) | dst;
                    else
                        m68ki_write_8(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ai_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pd_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2);
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_di_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ix_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_aw_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_al_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();
                    uint dst = m68ki_read_16(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = ((dst - d_src[0]) & 0xffff);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x8000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = (uint)((d_src[0]) & ~0xffff) | dst;
                    else
                        m68ki_write_16(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4);
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_get_ea_ix();
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint ea = m68ki_read_imm_32();
                    uint dst = m68ki_read_32(ea);
                    UIntSubArray d_src = new UIntSubArray(m68k_cpu.dr,(int)word2 & 7);
                    uint res = (dst - d_src[0]);

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~d_src[0] & dst & ~res) | (d_src[0] & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((d_src[0] & ~dst) | (res & ~dst) | (d_src[0] & res)) & 0x80000000);

                    if (m68k_cpu.not_z_flag!=0)
                        d_src[0] = dst;
                    else
                        m68ki_write_32(ea, m68k_cpu.dr[(word2 >> 6) & 7]);
                    m68k_clks_left[0] -= (15 + 16);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas2_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_32();
                    UIntSubArray r_src1 = new UIntSubArray(m68k_cpu.dr,(int)(word2 >> 16) & 7);
                    uint ea1 = m68k_cpu_dar[(int)word2 >> 31][(int)(word2 >> 28) & 7];
                    uint dst1 = m68ki_read_16(ea1);
                    uint res1 = ((dst1 - r_src1[0]) & 0xffff);
                    UIntSubArray r_src2 = new UIntSubArray (m68k_cpu.dr,(int)word2 & 7);
                    uint ea2 = m68k_cpu_dar[word2 >> 15][(int)(word2 >> 12) & 7];
                    uint dst2 = m68ki_read_16(ea2);
                    uint res2;

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res1) & 0x8000);
                    m68k_cpu.not_z_flag = res1;
                    m68k_cpu.v_flag = (((~r_src1[0] & dst1 & ~res1) | (r_src1[0] & ~dst1 & res1)) & 0x8000);
                    m68k_cpu.c_flag = (((r_src1[0] & ~dst1) | (res1 & ~dst1) | (r_src1[0] & res1)) & 0x8000);

                    if (m68k_cpu.not_z_flag==0)
                    {
                        res2 = ((dst2 - r_src2[0]) & 0xffff);

                        m68k_cpu.n_flag = ((res2) & 0x8000);
                        m68k_cpu.not_z_flag = res2;
                        m68k_cpu.v_flag = (((~r_src2[0] & dst2 & ~res2) | (r_src2[0] & ~dst2 & res2)) & 0x8000);
                        m68k_cpu.c_flag = (((r_src2[0] & ~dst2) | (res2 & ~dst2) | (r_src2[0] & res2)) & 0x8000);

                        if (m68k_cpu.not_z_flag==0)
                        {
                            m68ki_write_16(ea1, m68k_cpu.dr[(word2 >> 22) & 7]);
                            m68ki_write_16(ea2, m68k_cpu.dr[(word2 >> 6) & 7]);
                            m68k_clks_left[0] -= (22);
                            return;
                        }
                    }
                    r_src1[0] = ((word2) & 0x80000000) !=0? (uint)MAKE_INT_16(dst1) : ((r_src1[0]) & 0xffff) | dst1;
                    r_src2[0] = ((word2) & 0x00008000) !=0? (uint)MAKE_INT_16(dst2) : ((r_src2[0]) & 0xffff) | dst2;
                    m68k_clks_left[0] -= (25);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cas2_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_32();
                    UIntSubArray r_src1 = new UIntSubArray(m68k_cpu.dr,(int)(word2 >> 16) & 7);
                    uint ea1 = m68k_cpu_dar[word2 >> 31][(int)(word2 >> 28) & 7];
                    uint dst1 = m68ki_read_32(ea1);
                    uint res1 = (dst1 - r_src1[0]);
                    UIntSubArray r_src2 = new UIntSubArray(m68k_cpu.dr, (int)word2 & 7);
                    uint ea2 = m68k_cpu_dar[word2 >> 15][(int)(word2 >> 12) & 7];
                    uint dst2 = m68ki_read_32(ea2);
                    uint res2;

                    ; /* auto-disable (see m68kcpu.h) */
                    m68k_cpu.n_flag = ((res1) & 0x80000000);
                    m68k_cpu.not_z_flag = res1;
                    m68k_cpu.v_flag = (((~r_src1[0] & dst1 & ~res1) | (r_src1[0] & ~dst1 & res1)) & 0x80000000);
                    m68k_cpu.c_flag = (((r_src1[0] & ~dst1) | (res1 & ~dst1) | (r_src1[0] & res1)) & 0x80000000);

                    if (m68k_cpu.not_z_flag==0)
                    {
                        res2 = (dst2 - r_src2[0]);

                        m68k_cpu.n_flag = ((res2) & 0x80000000);
                        m68k_cpu.not_z_flag = res2;
                        m68k_cpu.v_flag = (((~r_src2[0] & dst2 & ~res2) | (r_src2[0] & ~dst2 & res2)) & 0x80000000);
                        m68k_cpu.c_flag = (((r_src2[0] & ~dst2) | (res2 & ~dst2) | (r_src2[0] & res2)) & 0x80000000);

                        if (m68k_cpu.not_z_flag==0)
                        {
                            m68ki_write_32(ea1, m68k_cpu.dr[(word2 >> 22) & 7]);
                            m68ki_write_32(ea2, m68k_cpu.dr[(word2 >> 6) & 7]);
                            m68k_clks_left[0] -= (22);
                            return;
                        }
                    }
                    r_src1[0] = dst1;
                    r_src2[0] = dst2;
                    m68k_clks_left[0] -= (25);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_chk_d_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7]));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_ai_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 4);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_pi_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 4);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_pd_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 6);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_di_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16((uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()))));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 8);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_ix_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix()));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 10);
                    return;
                }
                m68k_cpu.n_flag = (src < 0)?1u:0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_aw_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 8);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_al_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32()));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 12);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_pcdi_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                int bound = MAKE_INT_16(m68ki_read_16(ea));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 8);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_pcix_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix()));

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 10);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68000_chk_i_16()
            {
                int src = MAKE_INT_16((m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]));
                int bound = MAKE_INT_16(m68ki_read_imm_16());

                if (src >= 0 && src <= bound)
                {
                    m68k_clks_left[0] -= (10 + 4);
                    return;
                }
                m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                m68ki_interrupt(6);
            }


            static void m68020_chk_d_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)(m68k_cpu.dr[m68k_cpu.ir & 7]);

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 4);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_pi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 4);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_pd_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 6);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 8);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32(m68ki_get_ea_ix());

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 10);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 8);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32(m68ki_read_imm_32());

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 12);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    int bound = (int)m68ki_read_32(ea);

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 8);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_32(m68ki_get_ea_pcix());

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 10);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk_i_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    int src = (int)(m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                    int bound = (int)m68ki_read_imm_16();

                    if (src >= 0 && src <= bound)
                    {
                        m68k_clks_left[0] -= (8 + 4);
                        return;
                    }
                    m68k_cpu.n_flag = (src < 0) ? 1u : 0u;
                    m68ki_interrupt(6);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ai_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_di_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (uint)((m68k_cpu.ar[m68k_cpu.ir & 7]) + MAKE_INT_16(m68ki_read_imm_16()));
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ix_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_ix();
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_aw_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_al_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_read_imm_32();
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcdi_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcix_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_pcix();
                    uint lower_bound = m68ki_read_8(ea);
                    uint upper_bound = m68ki_read_8(ea + 1);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_8((int)lower_bound);
                        upper_bound = (uint)MAKE_INT_8((int)upper_bound);
                    }
                    else
                        src = ((src) & 0xff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ai_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 4);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_di_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag!=0 && ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ix_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_ix();
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_aw_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_al_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_read_imm_32();
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcdi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcix_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_pcix();
                    uint lower_bound = m68ki_read_16(ea);
                    uint upper_bound = m68ki_read_16(ea + 2);

                    if (((word2) & 0x00008000)!=0)
                    {
                        lower_bound = (uint)MAKE_INT_16(lower_bound);
                        upper_bound = (uint)MAKE_INT_16(upper_bound);
                    }
                    else
                        src = ((src) & 0xffff);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 10);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ai_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1U:0U;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1U:0U;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_di_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = ((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()));
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_ix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_ix();
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_aw_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = (uint)MAKE_INT_16(m68ki_read_imm_16());
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag!=0 && ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_al_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_read_imm_32();
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 16);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc +  (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = !(src == lower_bound || src == upper_bound) ? 1u : 0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag !=0&& ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 12);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_chk2_cmp2_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint word2 = m68ki_read_imm_16();
                    uint src = m68k_cpu_dar[(int)word2 >> 15][(int)(word2 >> 12) & 7];
                    uint ea = m68ki_get_ea_pcix();
                    uint lower_bound = m68ki_read_32(ea);
                    uint upper_bound = m68ki_read_32(ea + 4);

                    m68k_cpu.not_z_flag = (src == lower_bound || src == upper_bound)?1u:0u;
                    m68k_cpu.c_flag = (src < lower_bound || src > upper_bound)?1u:0u;

                    if (m68k_cpu.c_flag!=0 && ((word2) & 0x00000800)!=0) /* chk2 */
                        m68ki_interrupt(6);

                    m68k_clks_left[0] -= (18 + 14);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_clr_d_8()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffffff00;

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_clr_ai_8()
            {
                m68ki_write_8((m68k_cpu.ar[m68k_cpu.ir & 7]), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_clr_pi_8()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_clr_pi7_8()
            {
                m68ki_write_8(((m68k_cpu.ar[7] += 2) - 2), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_clr_pd_8()
            {
                m68ki_write_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_clr_pd7_8()
            {
                m68ki_write_8((m68k_cpu.ar[7] -= 2), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_clr_di_8()
            {
                m68ki_write_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_clr_ix_8()
            {
                m68ki_write_8(m68ki_get_ea_ix(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_clr_aw_8()
            {
                m68ki_write_8((uint)MAKE_INT_16(m68ki_read_imm_16()), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_clr_al_8()
            {
                m68ki_write_8(m68ki_read_imm_32(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_clr_d_16()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) &= 0xffff0000;

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (4);
            }


            static void m68000_clr_ai_16()
            {
                m68ki_write_16((m68k_cpu.ar[m68k_cpu.ir & 7]), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_clr_pi_16()
            {
                m68ki_write_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_clr_pd_16()
            {
                m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_clr_di_16()
            {
                m68ki_write_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_clr_ix_16()
            {
                m68ki_write_16(m68ki_get_ea_ix(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_clr_aw_16()
            {
                m68ki_write_16((uint)MAKE_INT_16(m68ki_read_imm_16()), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_clr_al_16()
            {
                m68ki_write_16(m68ki_read_imm_32(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68000_clr_d_32()
            {
                (m68k_cpu.dr[m68k_cpu.ir & 7]) = 0;

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (6);
            }


            static void m68000_clr_ai_32()
            {
                m68ki_write_32((m68k_cpu.ar[m68k_cpu.ir & 7]), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_clr_pi_32()
            {
                m68ki_write_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_clr_pd_32()
            {
                m68ki_write_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_clr_di_32()
            {
                m68ki_write_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_clr_ix_32()
            {
                m68ki_write_32(m68ki_get_ea_ix(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_clr_aw_32()
            {
                m68ki_write_32((uint)MAKE_INT_16(m68ki_read_imm_16()), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_clr_al_32()
            {
                m68ki_write_32(m68ki_read_imm_32(), 0);

                m68k_cpu.n_flag = m68k_cpu.v_flag = m68k_cpu.c_flag = 0;
                m68k_cpu.not_z_flag = 0;
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68000_cmp_d_8()
            {
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_cmp_ai_8()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_pi_8()
            {
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_pi7_8()
            {
                uint src = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_pd_8()
            {
                uint src = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_cmp_pd7_8()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_cmp_di_8()
            {
                uint src = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_ix_8()
            {
                uint src = m68ki_read_8(m68ki_get_ea_ix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_cmp_aw_8()
            {
                uint src = m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_al_8()
            {
                uint src = m68ki_read_8(m68ki_read_imm_32());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_cmp_pcdi_8()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_8(ea);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_pcix_8()
            {
                uint src = m68ki_read_8(m68ki_get_ea_pcix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_cmp_i_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_d_16()
            {
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_cmp_a_16()
            {
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4);
            }


            static void m68000_cmp_ai_16()
            {
                uint src = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_pi_16()
            {
                uint src = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_pd_16()
            {
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 6);
            }


            static void m68000_cmp_di_16()
            {
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_ix_16()
            {
                uint src = m68ki_read_16(m68ki_get_ea_ix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_cmp_aw_16()
            {
                uint src = m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_al_16()
            {
                uint src = m68ki_read_16(m68ki_read_imm_32());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 12);
            }


            static void m68000_cmp_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_16(ea);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 8);
            }


            static void m68000_cmp_pcix_16()
            {
                uint src = m68ki_read_16(m68ki_get_ea_pcix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 10);
            }


            static void m68000_cmp_i_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (4 + 4);
            }


            static void m68000_cmp_d_32()
            {
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmp_a_32()
            {
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmp_ai_32()
            {
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmp_pi_32()
            {
                uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmp_pd_32()
            {
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_cmp_di_32()
            {
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmp_ix_32()
            {
                uint src = m68ki_read_32(m68ki_get_ea_ix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_cmp_aw_32()
            {
                uint src = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmp_al_32()
            {
                uint src = m68ki_read_32(m68ki_read_imm_32());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_cmp_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_32(ea);
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmp_pcix_32()
            {
                uint src = m68ki_read_32(m68ki_get_ea_pcix());
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_cmp_i_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = (m68k_cpu.dr[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_d_16()
            {
                uint src = (uint)MAKE_INT_16((m68k_cpu.dr[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmpa_a_16()
            {
                uint src = (uint)MAKE_INT_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmpa_ai_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 4);
            }


            static void m68000_cmpa_pi_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2)));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 4);
            }


            static void m68000_cmpa_pd_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2)));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 6);
            }


            static void m68000_cmpa_di_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16()))));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_ix_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_ix()));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_cmpa_aw_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_al_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16(m68ki_read_imm_32()));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmpa_pcdi_16()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = (uint)MAKE_INT_16(m68ki_read_16(ea));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_pcix_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_16(m68ki_get_ea_pcix()));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_cmpa_i_16()
            {
                uint src = (uint)MAKE_INT_16(m68ki_read_imm_16());
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 4);
            }


            static void m68000_cmpa_d_32()
            {
                uint src = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmpa_a_32()
            {
                uint src = (m68k_cpu.ar[m68k_cpu.ir & 7]);
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6);
            }


            static void m68000_cmpa_ai_32()
            {
                uint src = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_pi_32()
            {
                uint src = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpa_pd_32()
            {
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 10);
            }


            static void m68000_cmpa_di_32()
            {
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmpa_ix_32()
            {
                uint src = m68ki_read_32(m68ki_get_ea_ix());
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_cmpa_aw_32()
            {
                uint src = m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmpa_al_32()
            {
                uint src = m68ki_read_32(m68ki_read_imm_32());
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 16);
            }


            static void m68000_cmpa_pcdi_32()
            {
                uint old_pc = (m68k_cpu.pc += 2) - 2;
                uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                uint src = m68ki_read_32(ea);
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 12);
            }


            static void m68000_cmpa_pcix_32()
            {
                uint src = m68ki_read_32(m68ki_get_ea_pcix());
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 14);
            }


            static void m68000_cmpa_i_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = (m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (6 + 8);
            }


            static void m68000_cmpi_d_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_cmpi_ai_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_cmpi_pi_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7])++));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_cmpi_pi7_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8(((m68k_cpu.ar[7] += 2) - 2));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_cmpi_pd_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8((--(m68k_cpu.ar[m68k_cpu.ir & 7])));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_cmpi_pd7_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8((m68k_cpu.ar[7] -= 2));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_cmpi_di_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_cmpi_ix_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8(m68ki_get_ea_ix());
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_cmpi_aw_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = (uint)m68ki_read_8((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_cmpi_al_8()
            {
                uint src = m68ki_read_imm_8();
                uint dst = m68ki_read_8(m68ki_read_imm_32());
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68020_cmpi_pcdi_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_8();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint dst = m68ki_read_8(ea);
                    uint res = ((dst - src) & 0xff);

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cmpi_pcix_8()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_8();
                    uint dst = m68ki_read_8(m68ki_get_ea_pcix());
                    uint res = ((dst - src) & 0xff);

                    m68k_cpu.n_flag = ((res) & 0x80);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_cmpi_d_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8);
            }


            static void m68000_cmpi_ai_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = m68ki_read_16((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_cmpi_pi_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = m68ki_read_16((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2));
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 4);
            }


            static void m68000_cmpi_pd_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 2));
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 6);
            }


            static void m68000_cmpi_di_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = (uint)m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) + (uint)MAKE_INT_16(m68ki_read_imm_16())));
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_cmpi_ix_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = m68ki_read_16(m68ki_get_ea_ix());
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 10);
            }


            static void m68000_cmpi_aw_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = (uint)m68ki_read_16((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 8);
            }


            static void m68000_cmpi_al_16()
            {
                uint src = m68ki_read_imm_16();
                uint dst = m68ki_read_16(m68ki_read_imm_32());
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (8 + 12);
            }


            static void m68020_cmpi_pcdi_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_16();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint dst = m68ki_read_16(ea);
                    uint res = ((dst - src) & 0xffff);

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cmpi_pcix_16()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_16();
                    uint dst = m68ki_read_16(m68ki_get_ea_pcix());
                    uint res = ((dst - src) & 0xffff);

                    m68k_cpu.n_flag = ((res) & 0x8000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_cmpi_d_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = (m68k_cpu.dr[m68k_cpu.ir & 7]);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (14);
            }


            static void m68000_cmpi_ai_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = m68ki_read_32((m68k_cpu.ar[m68k_cpu.ir & 7]));
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_cmpi_pi_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = m68ki_read_32((((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4));
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 8);
            }


            static void m68000_cmpi_pd_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) -= 4));
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 10);
            }


            static void m68000_cmpi_di_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = (uint)m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) +(uint) MAKE_INT_16(m68ki_read_imm_16())));
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_cmpi_ix_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = m68ki_read_32(m68ki_get_ea_ix());
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 14);
            }


            static void m68000_cmpi_aw_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = (uint)m68ki_read_32((uint)MAKE_INT_16(m68ki_read_imm_16()));
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 12);
            }


            static void m68000_cmpi_al_32()
            {
                uint src = m68ki_read_imm_32();
                uint dst = m68ki_read_32(m68ki_read_imm_32());
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (12 + 16);
            }


            static void m68020_cmpi_pcdi_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_32();
                    uint old_pc = (m68k_cpu.pc += 2) - 2;
                    uint ea = old_pc + (uint)MAKE_INT_16(m68ki_read_16(old_pc));
                    uint dst = m68ki_read_32(ea);
                    uint res = (dst - src);

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68020_cmpi_pcix_32()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    uint src = m68ki_read_imm_32();
                    uint dst = m68ki_read_32(m68ki_get_ea_pcix());
                    uint res = (dst - src);

                    m68k_cpu.n_flag = ((res) & 0x80000000);
                    m68k_cpu.not_z_flag = res;
                    m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                    m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                    m68k_clks_left[0] -= (8);
                    return;
                }
                m68000_illegal();
            }


            static void m68000_cmpm_8_ax7()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8((m68k_cpu.ar[7] += 2) - 2);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_cmpm_8_ay7()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_cmpm_8_axy7()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[7] += 2) - 2);
                uint dst = m68ki_read_8((m68k_cpu.ar[7] += 2) - 2);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_cmpm_8()
            {
                uint src = m68ki_read_8((m68k_cpu.ar[m68k_cpu.ir & 7])++);
                uint dst = m68ki_read_8((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7])++);
                uint res = ((dst - src) & 0xff);

                m68k_cpu.n_flag = ((res) & 0x80);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_cmpm_16()
            {
                uint src = m68ki_read_16(((m68k_cpu.ar[m68k_cpu.ir & 7]) += 2) - 2);
                uint dst = m68ki_read_16(((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 2) - 2);
                uint res = ((dst - src) & 0xffff);

                m68k_cpu.n_flag = ((res) & 0x8000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x8000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x8000);
                m68k_clks_left[0] -= (12);
            }


            static void m68000_cmpm_32()
            {
                uint src = m68ki_read_32(((m68k_cpu.ar[m68k_cpu.ir & 7]) += 4) - 4);
                uint dst = m68ki_read_32(((m68k_cpu.ar[(m68k_cpu.ir >> 9) & 7]) += 4) - 4);
                uint res = (dst - src);

                m68k_cpu.n_flag = ((res) & 0x80000000);
                m68k_cpu.not_z_flag = res;
                m68k_cpu.v_flag = (((~src & dst & ~res) | (src & ~dst & res)) & 0x80000000);
                m68k_cpu.c_flag = (((src & ~dst) | (res & ~dst) | (src & res)) & 0x80000000);
                m68k_clks_left[0] -= (20);
            }


            static void m68020_cpbcc()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    return;
                }
                m68000_1111();
            }


            static void m68020_cpdbcc()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    return;
                }
                m68000_1111();
            }


            static void m68020_cpgen()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    return;
                }
                m68000_1111();
            }


            static void m68020_cpscc()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    return;
                }
                m68000_1111();
            }


            static void m68020_cptrapcc()
            {
                if ((m68k_cpu.mode & (4 | 8)) != 0)
                {
                    return;
                }
                m68000_1111();
            }

        }
    }
}