using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        class cpu_i86 : cpu_interface
        {
            const int I86_INT_NONE = 0, I86_NMI_INT = 2;
            static int[] i86_ICount = new int[1];

            const byte ES = 0, CS = 1, SS = 2, DS = 3;
            const byte AX = 0, CX = 1, DX = 2, BX = 3, SP = 4, BP = 5, SI = 6, DI = 7;
            const byte AL = 0, AH = 1, CL = 2, CH = 3, DL = 4, DH = 5, BL = 6, BH = 7, SPL = 8, SPH = 9, BPL = 10, BPH = 11, SIL = 12, SIH = 13, DIL = 14, DIH = 15;
            enum SREGS { ES, CS, SS, DS };
            enum WREGS { AX, CX, DX, BX, SP, BP, SI, DI };
#if WINDOWS
            enum BREGS { AL, AH, CL, CH, DL, DH, BL, BH, SPL, SPH, BPL, BPH, SIL, SIH, DIL, DIH } ;
#else
            enum BREGS { AH,AL,CH,CL,DH,DL,BH,BL,SPH,SPL,BPH,BPL,SIH,SIL,DIH,DIL };
#endif

            public cpu_i86()
            {
                cpu_num = CPU_I86;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = I86_INT_NONE;
                irq_int = -1000;
                nmi_int = I86_NMI_INT;
                address_shift = 0;
                address_bits = 20;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 5;
                abits1 = ABITS1_20;
                abits2 = ABITS2_20;
                abitsmin = ABITS_MIN_20;
                icount = i86_ICount;
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
                    case CPU_INFO_NAME: return "I86";
                    case CPU_INFO_FAMILY: return "Intel 80x86";
                    case CPU_INFO_VERSION: return "1.4";
                    case CPU_INFO_FILE: return "i86.cs";
                    case CPU_INFO_CREDITS: return "Real mode i286 emulator v1.4 by Fabrice Frances\n(initial work I._based on David Hedley's pcemu)";
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
                i86_ICount[0] = cycles; /* ASG 971222 cycles_per_run;*/
                while (i86_ICount[0] > 0)
                {
                    if ((I.pending_irq != 0 && I.IF != 0) || (I.pending_irq & 0x02) != 0)
                        external_int(); /* HJB 12/15/98 */

                    seg_prefix = 0;
                    instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();


                }
                return cycles - i86_ICount[0];
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new i86_Regs();
            }
            public override uint get_pc()
            {
                return (uint)((I._base[CS] + (ushort)I.ip) & I.amask);
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
                uint i, j, c;
                BREGS[] reg_name = { (BREGS)AL, (BREGS)CL, (BREGS)DL, (BREGS)BL, (BREGS)AH, (BREGS)CH, (BREGS)DH, (BREGS)BH };
                I.Clear();
                //memset( &I, 0, sizeof(I) );

                /* If a reset parameter is given, take it as pointer to an address mask */
                if (param != null)
                    I.amask = (int)param;
                else
                    I.amask = 0x00ffff;
                I.sregs[CS] = 0xffff;
                I._base[CS] = (uint)I.sregs[CS] << 4;

                Mame.change_pc20((uint)((I._base[CS] + I.ip) & I.amask));

                for (i = 0; i < 256; i++)
                {
                    for (j = i, c = 0; j > 0; j >>= 1)
                        if ((j & 1) != 0) c++;

                    parity_table[i] = (!((c & 1) != 0) ? (byte)1 : (byte)0);
                }

                I.ZeroVal = I.ParityVal = 1;

                for (i = 0; i < 256; i++)
                {
                    Mod_RM.reg.b[i] = reg_name[(i & 0x38) >> 3];
                    Mod_RM.reg.w[i] = (WREGS)((i & 0x38) >> 3);
                }

                for (i = 0xc0; i < 0x100; i++)
                {
                    Mod_RM.RM.w[i] = (WREGS)(i & 7);
                    Mod_RM.RM.b[i] = (BREGS)reg_name[i & 7];
                }
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                I.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_nmi_line(int linestate)
            {
                throw new NotImplementedException();
            }
            public override void set_op_base(int pc)
            {
                Mame.cpu_setOPbase20(pc, 0);
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
            public class i86basicregs
            {                   /* eight general registers */
                public byte[] b = new byte[16];
                public class _w
                {
                    byte[] b;
                    public _w(byte[] b)
                    {
                        this.b = b;
                    }
                    public ushort this[int index]
                    {
                        get
                        {
                            return (ushort)(b[ 1 + index * 2] << 8 | b[ index * 2]); 
                            
                        }
                        set
                        {
                            b[index * 2] = (byte)value;
                            b[index * 2 + 1] = (byte)(value >> 8); 
                        }
                    }
                    public ushort this[uint index]
                    {
                        get
                        {
                            throw new Exception();
                        }
                        set
                        {
                            throw new Exception();
                        }
                    }
                }

                public _w w;
                public i86basicregs() { w = new _w(b); }

            }
            class i86_Regs
            {
                public void Clear()
                {
                    regs = new i86basicregs();
                    ip = 0; flags = 0; AuxVal = 0; OverVal = 0; SignVal = 0; ZeroVal = 0; CarryVal = 0; ParityVal = 0;
                    pending_irq = 0; nmi_state = 0; irq_state = 0;
                    Array.Clear(_base, 0, 4);
                    Array.Clear(sregs, 0, 4);
                }
                public i86basicregs regs = new i86basicregs();
                public int amask;
                public int ip;
                public ushort flags;
                public uint[] _base = new uint[4];
                public ushort[] sregs = new ushort[4];
                public irqcallback irq_callback;
                public int AuxVal, OverVal, SignVal, ZeroVal, CarryVal, ParityVal;
                public byte TF, IF, DF, MF;
                public byte int_vector;
                public byte pending_irq;
                public sbyte nmi_state, irq_state;
            }
            static i86_Regs I = new i86_Regs();
            static uint prefix_base;
            static byte seg_prefix;
            static byte[] parity_table = new byte[256];
            class MOD_RM
            {
                public class REG
                {
                    public WREGS[] w = new WREGS[256];
                    public BREGS[] b = new BREGS[256];
                }
                public REG reg = new REG();
                public class rm
                {
                    public WREGS[] w = new WREGS[256];
                    public BREGS[] b = new BREGS[256];
                }
                public rm RM = new rm();
            }
            static MOD_RM Mod_RM = new MOD_RM();



            static void i86_interrupt(uint int_num)
            {
                uint dest_seg, dest_off;

                i_pushf();
                I.TF = I.IF = 0;

                if (int_num == unchecked((uint)-1))
                    int_num = (uint)I.irq_callback(0);

                dest_off = (uint)(cpu_readmem20((int)(int_num * 4) & I.amask) + (cpu_readmem20((int)((int_num * 4) + 1) & I.amask) << 8)); i86_ICount[0] -= 10;
                dest_seg = (uint)(cpu_readmem20((int)(int_num * 4 + 2) & I.amask) + (cpu_readmem20((int)((int_num * 4 + 2) + 1) & I.amask) << 8)); i86_ICount[0] -= 10;

                {
                    I.regs.w[SP] -= 2;
                    {
                        i86_ICount[0] -= 11;
                        cpu_writemem20((int)(((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask, (byte)(I.sregs[CS]));
                        cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask, (I.sregs[CS]) >> 8);
                    };
                };
                {
                    I.regs.w[SP] -= 2;
                    {
                        i86_ICount[0] -= 11;
                        cpu_writemem20((int)(((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask, (byte)(I.ip));
                        cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask, (I.ip) >> 8);
                    };
                };
                I.ip = (ushort)dest_off;
                I.sregs[CS] = (ushort)dest_seg;
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                Mame.change_pc20((uint)((I._base[CS] + I.ip) & I.amask));

            }
            static void i_pushf() /* Opcode 0x9c */
            {
                i86_ICount[0] -= 3;
                I.regs.w[SP] -= 2;
                i86_ICount[0] -= 11;
                cpu_writemem20((int)(((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask, (byte)((ushort)((I.CarryVal != 0) ? 1 : 0 | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal != 0) ? 1 : 0 << 4) | ((I.ZeroVal == 0) ? 1 : 0 << 6) | ((I.SignVal < 0) ? 1 : 0 << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal != 0) ? 1 : 0 << 11)) | 0xf000));
                cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask, ((ushort)((I.CarryVal != 0) ? 1 : 0 | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal != 0) ? 1 : 0 << 4) | ((I.ZeroVal == 0) ? 1 : 0 << 6) | ((I.SignVal < 0) ? 1 : 0 << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal != 0) ? 1 : 0 << 11)) | 0xf000) >> 8);
            }
            static void external_int()
            {
                if ((I.pending_irq & 0x02) != 0)
                {
                    i86_interrupt(2);
                    I.pending_irq &= unchecked((byte)~0x02);
                }
                else
                    if (I.pending_irq != 0)
                    {
                        /* the actual vector is retrieved after pushing flags */
                        /* and clearing the IF */
                        i86_interrupt(unchecked((uint)-1));
                    }
            }
            delegate void opcode();
            static opcode[] instruction =
{
    i_add_br8, /* 0x00 */
    i_add_wr16, /* 0x01 */
    i_add_r8b, /* 0x02 */
    i_add_r16w, /* 0x03 */
    i_add_ald8, /* 0x04 */
    i_add_axd16, /* 0x05 */
    i_push_es, /* 0x06 */
    i_pop_es, /* 0x07 */
    i_or_br8, /* 0x08 */
    i_or_wr16, /* 0x09 */
    i_or_r8b, /* 0x0a */
    i_or_r16w, /* 0x0b */
    i_or_ald8, /* 0x0c */
    i_or_axd16, /* 0x0d */
    i_push_cs, /* 0x0e */
    i_invalid,
    i_adc_br8, /* 0x10 */
    i_adc_wr16, /* 0x11 */
    i_adc_r8b, /* 0x12 */
    i_adc_r16w, /* 0x13 */
    i_adc_ald8, /* 0x14 */
    i_adc_axd16, /* 0x15 */
    i_push_ss, /* 0x16 */
    i_pop_ss, /* 0x17 */
    i_sbb_br8, /* 0x18 */
    i_sbb_wr16, /* 0x19 */
    i_sbb_r8b, /* 0x1a */
    i_sbb_r16w, /* 0x1b */
    i_sbb_ald8, /* 0x1c */
    i_sbb_axd16, /* 0x1d */
    i_push_ds, /* 0x1e */
    i_pop_ds, /* 0x1f */
    i_and_br8, /* 0x20 */
    i_and_wr16, /* 0x21 */
    i_and_r8b, /* 0x22 */
    i_and_r16w, /* 0x23 */
    i_and_ald8, /* 0x24 */
    i_and_axd16, /* 0x25 */
    i_es, /* 0x26 */
    i_daa, /* 0x27 */
    i_sub_br8, /* 0x28 */
    i_sub_wr16, /* 0x29 */
    i_sub_r8b, /* 0x2a */
    i_sub_r16w, /* 0x2b */
    i_sub_ald8, /* 0x2c */
    i_sub_axd16, /* 0x2d */
    i_cs, /* 0x2e */
    i_das, /* 0x2f */
    i_xor_br8, /* 0x30 */
    i_xor_wr16, /* 0x31 */
    i_xor_r8b, /* 0x32 */
    i_xor_r16w, /* 0x33 */
    i_xor_ald8, /* 0x34 */
    i_xor_axd16, /* 0x35 */
    i_ss, /* 0x36 */
    i_aaa, /* 0x37 */
    i_cmp_br8, /* 0x38 */
    i_cmp_wr16, /* 0x39 */
    i_cmp_r8b, /* 0x3a */
    i_cmp_r16w, /* 0x3b */
    i_cmp_ald8, /* 0x3c */
    i_cmp_axd16, /* 0x3d */
    i_ds, /* 0x3e */
    i_aas, /* 0x3f */
    i_inc_ax, /* 0x40 */
    i_inc_cx, /* 0x41 */
    i_inc_dx, /* 0x42 */
    i_inc_bx, /* 0x43 */
    i_inc_sp, /* 0x44 */
    i_inc_bp, /* 0x45 */
    i_inc_si, /* 0x46 */
    i_inc_di, /* 0x47 */
    i_dec_ax, /* 0x48 */
    i_dec_cx, /* 0x49 */
    i_dec_dx, /* 0x4a */
    i_dec_bx, /* 0x4b */
    i_dec_sp, /* 0x4c */
    i_dec_bp, /* 0x4d */
    i_dec_si, /* 0x4e */
    i_dec_di, /* 0x4f */
    i_push_ax, /* 0x50 */
    i_push_cx, /* 0x51 */
    i_push_dx, /* 0x52 */
    i_push_bx, /* 0x53 */
    i_push_sp, /* 0x54 */
    i_push_bp, /* 0x55 */
    i_push_si, /* 0x56 */
    i_push_di, /* 0x57 */
    i_pop_ax, /* 0x58 */
    i_pop_cx, /* 0x59 */
    i_pop_dx, /* 0x5a */
    i_pop_bx, /* 0x5b */
    i_pop_sp, /* 0x5c */
    i_pop_bp, /* 0x5d */
    i_pop_si, /* 0x5e */
    i_pop_di, /* 0x5f */
    i_pusha, /* 0x60 */
    i_popa, /* 0x61 */
    i_bound, /* 0x62 */
    i_invalid,
    i_invalid,
    i_invalid,
    i_invalid,
    i_invalid,
    i_push_d16, /* 0x68 */
    i_imul_d16, /* 0x69 */
    i_push_d8, /* 0x6a */
    i_imul_d8, /* 0x6b */
    i_insb, /* 0x6c */
    i_insw, /* 0x6d */
    i_outsb, /* 0x6e */
    i_outsw, /* 0x6f */
    i_jo, /* 0x70 */
    i_jno, /* 0x71 */
    i_jb, /* 0x72 */
    i_jnb, /* 0x73 */
    i_jz, /* 0x74 */
    i_jnz, /* 0x75 */
    i_jbe, /* 0x76 */
    i_jnbe, /* 0x77 */
    i_js, /* 0x78 */
    i_jns, /* 0x79 */
    i_jp, /* 0x7a */
    i_jnp, /* 0x7b */
    i_jl, /* 0x7c */
    i_jnl, /* 0x7d */
    i_jle, /* 0x7e */
    i_jnle, /* 0x7f */
    i_80pre, /* 0x80 */
    i_81pre, /* 0x81 */
 i_82pre, /* 0x82 */
    i_83pre, /* 0x83 */
    i_test_br8, /* 0x84 */
    i_test_wr16, /* 0x85 */
    i_xchg_br8, /* 0x86 */
    i_xchg_wr16, /* 0x87 */
    i_mov_br8, /* 0x88 */
    i_mov_wr16, /* 0x89 */
    i_mov_r8b, /* 0x8a */
    i_mov_r16w, /* 0x8b */
    i_mov_wsreg, /* 0x8c */
    i_lea, /* 0x8d */
    i_mov_sregw, /* 0x8e */
    i_popw, /* 0x8f */
    i_nop, /* 0x90 */
    i_xchg_axcx, /* 0x91 */
    i_xchg_axdx, /* 0x92 */
    i_xchg_axbx, /* 0x93 */
    i_xchg_axsp, /* 0x94 */
    i_xchg_axbp, /* 0x95 */
    i_xchg_axsi, /* 0x97 */
    i_xchg_axdi, /* 0x97 */
    i_cbw, /* 0x98 */
    i_cwd, /* 0x99 */
    i_call_far, /* 0x9a */
    i_wait, /* 0x9b */
    i_pushf, /* 0x9c */
    i_popf, /* 0x9d */
    i_sahf, /* 0x9e */
    i_lahf, /* 0x9f */
    i_mov_aldisp, /* 0xa0 */
    i_mov_axdisp, /* 0xa1 */
    i_mov_dispal, /* 0xa2 */
    i_mov_dispax, /* 0xa3 */
    i_movsb, /* 0xa4 */
    i_movsw, /* 0xa5 */
    i_cmpsb, /* 0xa6 */
    i_cmpsw, /* 0xa7 */
    i_test_ald8, /* 0xa8 */
    i_test_axd16, /* 0xa9 */
    i_stosb, /* 0xaa */
    i_stosw, /* 0xab */
    i_lodsb, /* 0xac */
    i_lodsw, /* 0xad */
    i_scasb, /* 0xae */
    i_scasw, /* 0xaf */
    i_mov_ald8, /* 0xb0 */
    i_mov_cld8, /* 0xb1 */
    i_mov_dld8, /* 0xb2 */
    i_mov_bld8, /* 0xb3 */
    i_mov_ahd8, /* 0xb4 */
    i_mov_chd8, /* 0xb5 */
    i_mov_dhd8, /* 0xb6 */
    i_mov_bhd8, /* 0xb7 */
    i_mov_axd16, /* 0xb8 */
    i_mov_cxd16, /* 0xb9 */
    i_mov_dxd16, /* 0xba */
    i_mov_bxd16, /* 0xbb */
    i_mov_spd16, /* 0xbc */
    i_mov_bpd16, /* 0xbd */
    i_mov_sid16, /* 0xbe */
    i_mov_did16, /* 0xbf */
    i_rotshft_bd8, /* 0xc0 */
    i_rotshft_wd8, /* 0xc1 */
    i_ret_d16, /* 0xc2 */
    i_ret, /* 0xc3 */
    i_les_dw, /* 0xc4 */
    i_lds_dw, /* 0xc5 */
    i_mov_bd8, /* 0xc6 */
    i_mov_wd16, /* 0xc7 */
    i_enter, /* 0xc8 */
    i_leave, /* 0xc9 */
    i_retf_d16, /* 0xca */
    i_retf, /* 0xcb */
    i_int3, /* 0xcc */
    i_int, /* 0xcd */
    i_into, /* 0xce */
    i_iret, /* 0xcf */
    i_rotshft_b, /* 0xd0 */
    i_rotshft_w, /* 0xd1 */
    i_rotshft_bcl, /* 0xd2 */
    i_rotshft_wcl, /* 0xd3 */
    i_aam, /* 0xd4 */
    i_aad, /* 0xd5 */
    i_invalid,
    i_xlat, /* 0xd7 */
    i_escape, /* 0xd8 */
    i_escape, /* 0xd9 */
    i_escape, /* 0xda */
    i_escape, /* 0xdb */
    i_escape, /* 0xdc */
    i_escape, /* 0xdd */
    i_escape, /* 0xde */
    i_escape, /* 0xdf */
    i_loopne, /* 0xe0 */
    i_loope, /* 0xe1 */
    i_loop, /* 0xe2 */
    i_jcxz, /* 0xe3 */
    i_inal, /* 0xe4 */
    i_inax, /* 0xe5 */
    i_outal, /* 0xe6 */
    i_outax, /* 0xe7 */
    i_call_d16, /* 0xe8 */
    i_jmp_d16, /* 0xe9 */
    i_jmp_far, /* 0xea */
    i_jmp_d8, /* 0xeb */
    i_inaldx, /* 0xec */
    i_inaxdx, /* 0xed */
    i_outdxal, /* 0xee */
    i_outdxax, /* 0xef */
    i_lock, /* 0xf0 */
    i_invalid, /* 0xf1 */
    i_repne, /* 0xf2 */
    i_repe, /* 0xf3 */
    i_hlt, /* 0xf4 */
    i_cmc, /* 0xf5 */
    i_f6pre, /* 0xf6 */
    i_f7pre, /* 0xf7 */
    i_clc, /* 0xf8 */
    i_stc, /* 0xf9 */
    i_cli, /* 0xfa */
    i_sti, /* 0xfb */
    i_cld, /* 0xfc */
    i_std, /* 0xfd */
    i_fepre, /* 0xfe */
    i_ffpre /* 0xff */
};

            static uint EA;
            static uint EO; /* HJB 12/13/98 effective offset of the address (before segment is added) */

            static uint EA_000() { i86_ICount[0] -= 7; EO = (ushort)(I.regs.w[BX] + I.regs.w[SI]); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_001() { i86_ICount[0] -= 8; EO = (ushort)(I.regs.w[BX] + I.regs.w[DI]); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_002() { i86_ICount[0] -= 8; EO = (ushort)(I.regs.w[BP] + I.regs.w[SI]); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + EO; return EA; }
            static uint EA_003() { i86_ICount[0] -= 7; EO = (ushort)(I.regs.w[BP] + I.regs.w[DI]); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + EO; return EA; }
            static uint EA_004() { i86_ICount[0] -= 5; EO = I.regs.w[SI]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_005() { i86_ICount[0] -= 5; EO = I.regs.w[DI]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_006() { i86_ICount[0] -= 6; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_007() { i86_ICount[0] -= 5; EO = I.regs.w[BX]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }

            static uint EA_100() { i86_ICount[0] -= 11; EO = (ushort)(I.regs.w[BX] + I.regs.w[SI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_101() { i86_ICount[0] -= 12; EO = (ushort)(I.regs.w[BX] + I.regs.w[DI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_102() { i86_ICount[0] -= 12; EO = (ushort)(I.regs.w[BP] + I.regs.w[SI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + EO; return EA; }
            static uint EA_103() { i86_ICount[0] -= 11; EO = (ushort)(I.regs.w[BP] + I.regs.w[DI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + EO; return EA; }
            static uint EA_104() { i86_ICount[0] -= 9; EO = (ushort)(I.regs.w[SI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_105() { i86_ICount[0] -= 9; EO = (ushort)(I.regs.w[DI] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }
            static uint EA_106() { i86_ICount[0] -= 9; EO = (ushort)(I.regs.w[BP] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + EO; return EA; }
            static uint EA_107() { i86_ICount[0] -= 9; EO = (ushort)(I.regs.w[BX] + (sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + EO; return EA; }

            static uint EA_200() { i86_ICount[0] -= 11; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += (uint)(I.regs.w[BX] + I.regs.w[SI]); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (ushort)EO; return EA; }
            static uint EA_201() { i86_ICount[0] -= 12; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += (uint)(I.regs.w[BX] + I.regs.w[DI]); EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (ushort)EO; return EA; }
            static uint EA_202() { i86_ICount[0] -= 12; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += (uint)(I.regs.w[BP] + I.regs.w[SI]); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + (ushort)EO; return EA; }
            static uint EA_203() { i86_ICount[0] -= 11; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += (uint)(I.regs.w[BP] + I.regs.w[DI]); EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + (ushort)EO; return EA; }
            static uint EA_204() { i86_ICount[0] -= 9; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += I.regs.w[SI]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (ushort)EO; return EA; }
            static uint EA_205() { i86_ICount[0] -= 9; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += I.regs.w[DI]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (ushort)EO; return EA; }
            static uint EA_206() { i86_ICount[0] -= 9; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += I.regs.w[BP]; EA = ((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + (ushort)EO; return EA; }
            static uint EA_207() { i86_ICount[0] -= 9; EO = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); EO += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8; EO += I.regs.w[BX]; EA = ((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (ushort)EO; return EA; }
            delegate uint ea_opcode();
            static ea_opcode[] GetEA ={
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,
 EA_000, EA_001, EA_002, EA_003, EA_004, EA_005, EA_006, EA_007,

 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,
 EA_100, EA_101, EA_102, EA_103, EA_104, EA_105, EA_106, EA_107,

 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207,
 EA_200, EA_201, EA_202, EA_203, EA_204, EA_205, EA_206, EA_207
};

            #region OpCodes

            static void i_add_br8() /* Opcode 0x00 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                {
                    uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80);
                    I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res;
                };
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }
            static void i_add_wr16() /* Opcode 0x01 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }
                i86_ICount[0] -= 3;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }
            static void i_add_r8b() /* Opcode 0x02 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }
            static void i_add_r16w() /* Opcode 0x03 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }
                i86_ICount[0] -= 3;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }
            static void i_add_ald8() /* Opcode 0x04 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[AL] = (byte)dst;
            }
            static void i_add_axd16() /* Opcode 0x05 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[AX] = (ushort)dst;
            }
            static void i_push_es() /* Opcode 0x06 */
            {
                i86_ICount[0] -= 3;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.sregs[ES])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.sregs[ES]) >> 8)); }; };
            }
            static void i_pop_es() /* Opcode 0x07 */
            {
                { I.sregs[ES] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[ES] = (uint)(I.sregs[ES] << 4);
                i86_ICount[0] -= 2;
            }
            static void i_or_br8() /* Opcode 0x08 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }
            static void i_or_wr16() /* Opcode 0x09 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                }
                i86_ICount[0] -= 3;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }
            static void i_or_r8b() /* Opcode 0x0a */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }
            static void i_or_r16w() /* Opcode 0x0b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                }
                i86_ICount[0] -= 3;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }
            static void i_or_ald8() /* Opcode 0x0c */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[AL] = (byte)dst;
            }
            static void i_or_axd16() /* Opcode 0x0d */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[AX] = (ushort)dst;
            }
            static void i_push_cs() /* Opcode 0x0e */
            {
                i86_ICount[0] -= 3;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.sregs[CS])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.sregs[CS]) >> 8)); }; };
            }

            /* Opcode 0x0f invalid */

            static void i_adc_br8() /* Opcode 0x10 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }

            static void i_adc_wr16() /* Opcode 0x11 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }

            static void i_adc_r8b() /* Opcode 0x12 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }

            static void i_adc_r16w() /* Opcode 0x13 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_adc_ald8() /* Opcode 0x14 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[AL] = (byte)dst;
            }

            static void i_adc_axd16() /* Opcode 0x15 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[AX] = (ushort)dst;
            }

            static void i_push_ss() /* Opcode 0x16 */
            {
                i86_ICount[0] -= 3;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.sregs[SS])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.sregs[SS]) >> 8)); }; };
            }

            static void i_pop_ss() /* Opcode 0x17 */
            {
                i86_ICount[0] -= 2;
                { I.sregs[SS] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[SS] = (uint)(I.sregs[SS] << 4);
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))](); /* no interrupt before next instruction */
            }

            static void i_sbb_br8() /* Opcode 0x18 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (uint)((byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask)));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }

            static void i_sbb_wr16() /* Opcode 0x19 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM]();
                    i86_ICount[0] -= 10;
                    dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }

            static void i_sbb_r8b() /* Opcode 0x1a */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (uint)((byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask)));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }

            static void i_sbb_r16w() /* Opcode 0x1b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_sbb_ald8() /* Opcode 0x1c */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[AL] = (byte)dst;
            }

            static void i_sbb_axd16() /* Opcode 0x1d */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                src += (I.CarryVal != 0) ? 1u : 0u;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[AX] = (ushort)dst;
            }

            static void i_push_ds() /* Opcode 0x1e */
            {
                i86_ICount[0] -= 3;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.sregs[DS])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.sregs[DS]) >> 8)); }; };
            }

            static void i_pop_ds() /* Opcode 0x1f */
            {
                { I.sregs[DS] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[DS] = (uint)(I.sregs[DS] << 4);
                i86_ICount[0] -= 2;
            }

            static void i_and_br8() /* Opcode 0x20 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }

            static void i_and_wr16() /* Opcode 0x21 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }

            static void i_and_r8b() /* Opcode 0x22 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }

            static void i_and_r16w() /* Opcode 0x23 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_and_ald8() /* Opcode 0x24 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[AL] = (byte)dst;
            }

            static void i_and_axd16() /* Opcode 0x25 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[AX] = (ushort)dst;
            }

            static void i_es() /* Opcode 0x26 */
            {
                seg_prefix = 1;
                prefix_base = I._base[ES];
                i86_ICount[0] -= 2;
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
            }

            static void i_daa() /* Opcode 0x27 */
            {
                if ((I.AuxVal != 0) || ((I.regs.b[AL] & 0xf) > 9))
                {
                    int tmp;
                    I.regs.b[AL] = (byte)(tmp = I.regs.b[AL] + 6);
                    I.AuxVal = 1;
                    I.CarryVal |= tmp & 0x100;
                }

                if ((I.CarryVal != 0) || (I.regs.b[AL] > 0x9f))
                {
                    I.regs.b[AL] += 0x60;
                    I.CarryVal = 1;
                }

                I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(I.regs.b[AL]);
                i86_ICount[0] -= 4;
            }

            static void i_sub_br8() /* Opcode 0x28 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }

            static void i_sub_wr16() /* Opcode 0x29 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM]();
                    i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }

            static void i_sub_r8b() /* Opcode 0x2a */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }

            static void i_sub_r16w() /* Opcode 0x2b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_sub_ald8() /* Opcode 0x2c */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.b[AL] = (byte)dst;
            }

            static void i_sub_axd16() /* Opcode 0x2d */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[AX] = (ushort)dst;
            }

            static void i_cs() /* Opcode 0x2e */
            {
                seg_prefix = 1;
                prefix_base = I._base[CS];
                i86_ICount[0] -= 2;
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
            }

            static void i_das() /* Opcode 0x2f */
            {
                if ((I.AuxVal != 0) || ((I.regs.b[AL] & 0xf) > 9))
                {
                    int tmp;
                    I.regs.b[AL] = (byte)(tmp = I.regs.b[AL] - 6);
                    I.AuxVal = 1;
                    I.CarryVal |= tmp & 0x100;
                }

                if ((I.CarryVal != 0) || (I.regs.b[AL] > 0x9f))
                {
                    I.regs.b[AL] -= 0x60;
                    I.CarryVal = 1;
                }

                I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(I.regs.b[AL]);
                i86_ICount[0] -= 4;
            }

            static void i_xor_br8() /* Opcode 0x30 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
            }

            static void i_xor_wr16() /* Opcode 0x31 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
            }

            static void i_xor_r8b() /* Opcode 0x32 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
            }

            static void i_xor_r16w() /* Opcode 0x33 */
            {

                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_xor_ald8() /* Opcode 0x34 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                I.regs.b[AL] = (byte)dst;
            }

            static void i_xor_axd16() /* Opcode 0x35 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                I.regs.w[AX] = (ushort)dst;
            }

            static void i_ss() /* Opcode 0x36 */
            {
                seg_prefix = 1;
                prefix_base = I._base[SS];
                i86_ICount[0] -= 2;
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
            }

            static void i_aaa() /* Opcode 0x37 */
            {
                if ((I.AuxVal != 0) || ((I.regs.b[AL] & 0xf) > 9))
                {
                    I.regs.b[AL] += 6;
                    I.regs.b[AH] += 1;
                    I.AuxVal = 1;
                    I.CarryVal = 1;
                }
                else
                {
                    I.AuxVal = 0;
                    I.CarryVal = 0;
                }
                I.regs.b[AL] &= 0x0F;
                i86_ICount[0] -= 8;
            }

            static void i_cmp_br8() /* Opcode 0x38 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
            }

            static void i_cmp_wr16() /* Opcode 0x39 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
            }

            static void i_cmp_r8b() /* Opcode 0x3a */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
            }

            static void i_cmp_r16w() /* Opcode 0x3b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
            }

            static void i_cmp_ald8() /* Opcode 0x3c */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
            }

            static void i_cmp_axd16() /* Opcode 0x3d */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX]; src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
            }

            static void i_ds() /* Opcode 0x3e */
            {
                seg_prefix = 1;
                prefix_base = I._base[DS];
                i86_ICount[0] -= 2;
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
            }

            static void i_aas() /* Opcode 0x3f */
            {
                if ((I.AuxVal != 0) || ((I.regs.b[AL] & 0xf) > 9))
                {
                    I.regs.b[AL] -= 6;
                    I.regs.b[AH] -= 1;
                    I.AuxVal = 1;
                    I.CarryVal = 1;
                }
                else
                {
                    I.AuxVal = 0;
                    I.CarryVal = 0;
                }
                I.regs.b[AL] &= 0x0F;
                i86_ICount[0] -= 8;
            }
            static void i_inc_ax() /* Opcode 0x40 */
            {
                { uint tmp = (uint)I.regs.w[AX]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[AX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_cx() /* Opcode 0x41 */
            {
                { uint tmp = (uint)I.regs.w[CX]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[CX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_dx() /* Opcode 0x42 */
            {
                { uint tmp = (uint)I.regs.w[DX]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[DX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_bx() /* Opcode 0x43 */
            {
                { uint tmp = (uint)I.regs.w[BX]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[BX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_sp() /* Opcode 0x44 */
            {
                { uint tmp = (uint)I.regs.w[SP]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[SP] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_bp() /* Opcode 0x45 */
            {
                { uint tmp = (uint)I.regs.w[BP]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[BP] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_si() /* Opcode 0x46 */
            {
                { uint tmp = (uint)I.regs.w[SI]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[SI] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_inc_di() /* Opcode 0x47 */
            {
                { uint tmp = (uint)I.regs.w[DI]; uint tmp1 = tmp + 1; I.OverVal = (int)((tmp1 ^ tmp) & (tmp1 ^ 1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[DI] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }
            static void i_dec_ax() /* Opcode 0x48 */
            {
                { uint tmp = (uint)I.regs.w[AX]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[AX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_cx() /* Opcode 0x49 */
            {
                { uint tmp = (uint)I.regs.w[CX]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[CX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_dx() /* Opcode 0x4a */
            {
                { uint tmp = (uint)I.regs.w[DX]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[DX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_bx() /* Opcode 0x4b */
            {
                { uint tmp = (uint)I.regs.w[BX]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[BX] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_sp() /* Opcode 0x4c */
            {
                { uint tmp = (uint)I.regs.w[SP]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[SP] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_bp() /* Opcode 0x4d */
            {
                { uint tmp = (uint)I.regs.w[BP]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[BP] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_si() /* Opcode 0x4e */
            {
                { uint tmp = (uint)I.regs.w[SI]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[SI] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_dec_di() /* Opcode 0x4f */
            {
                { uint tmp = (uint)I.regs.w[DI]; uint tmp1 = tmp - 1; I.OverVal = (int)((tmp ^ 1) & (tmp ^ tmp1) & 0x8000); I.AuxVal = (int)((tmp1 ^ (tmp ^ 1)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1); I.regs.w[DI] = (ushort)tmp1; i86_ICount[0] -= 3; };
            }

            static void i_push_ax() /* Opcode 0x50 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[AX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[AX]) >> 8)); }; };
            }

            static void i_push_cx() /* Opcode 0x51 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[CX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[CX]) >> 8)); }; };
            }

            static void i_push_dx() /* Opcode 0x52 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[DX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[DX]) >> 8)); }; };
            }

            static void i_push_bx() /* Opcode 0x53 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BX]) >> 8)); }; };
            }

            static void i_push_sp() /* Opcode 0x54 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[SP])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[SP]) >> 8)); }; };
            }

            static void i_push_bp() /* Opcode 0x55 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BP])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BP]) >> 8)); }; };
            }


            static void i_push_si() /* Opcode 0x56 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[SI])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[SI]) >> 8)); }; };
            }

            static void i_push_di() /* Opcode 0x57 */
            {
                i86_ICount[0] -= 4;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[DI])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[DI]) >> 8)); }; };
            }

            static void i_pop_ax() /* Opcode 0x58 */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[AX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_cx() /* Opcode 0x59 */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[CX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_dx() /* Opcode 0x5a */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[DX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_bx() /* Opcode 0x5b */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[BX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_sp() /* Opcode 0x5c */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[SP] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_bp() /* Opcode 0x5d */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[BP] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_si() /* Opcode 0x5e */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[SI] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pop_di() /* Opcode 0x5f */
            {
                i86_ICount[0] -= 2;
                { I.regs.w[DI] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_pusha() /* Opcode 0x60 */
            {
                uint tmp = I.regs.w[SP];
                i86_ICount[0] -= 17;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[AX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[AX]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[CX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[CX]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[DX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[DX]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BX])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BX]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((tmp) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BP])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BP]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[SI])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[SI]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[DI])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[DI]) >> 8)); }; };
            }

            static void i_popa() /* Opcode 0x61 */
            {
                uint tmp;
                i86_ICount[0] -= 19;
                { I.regs.w[DI] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[SI] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[BP] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { tmp = (uint)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[BX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[DX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[CX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.regs.w[AX] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_bound() /* Opcode 0x62 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                int low; if ((ModRM) >= 0xc0)
                    low = (short)I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; low = (short)cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8));
                }
                int high = (short)(cpu_readmem20((int)((EA + 2) & I.amask) + (cpu_readmem20((int)((EA + 2) + 1) & I.amask) << 8))); i86_ICount[0] -= 10;
                int tmp = (short)I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                if (tmp < low || tmp > high)
                {
                    I.ip -= 2;
                    i86_interrupt(5);
                }
            }

            static void i_push_d16() /* Opcode 0x68 */
            {
                uint tmp = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 3;
                tmp += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((tmp) >> 8)); }; };
            }

            static void i_imul_d16() /* Opcode 0x69 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                uint src2 = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                i86_ICount[0] -= 150;
                dst = (uint)((int)((short)src) * (int)((short)src2));
                I.CarryVal = I.OverVal = (((int)dst) >> 15 != 0) && (((int)dst) >> 15 != -1) ? 1 : 0;
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }


            static void i_push_d8() /* Opcode 0x6a */
            {
                uint tmp = (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)))));
                i86_ICount[0] -= 3;
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((tmp) >> 8)); }; };
            }

            static void i_imul_d8() /* Opcode 0x6b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                uint src2 = (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)))));

                i86_ICount[0] -= 150;
                dst = (uint)((int)((short)src) * (int)((short)src2));
                I.CarryVal = I.OverVal = (((int)dst) >> 15 != 0) && (((int)dst) >> 15 != -1) ? 1 : 0;
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
            }

            static void i_insb() /* Opcode 0x6c */
            {
                i86_ICount[0] -= 5;
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)(cpu_readport(I.regs.w[DX]))); };
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
            }

            static void i_insw() /* Opcode 0x6d */
            {
                i86_ICount[0] -= 5;
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)(cpu_readport(I.regs.w[DX]))); };
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI] + 1)) & I.amask), (int)(cpu_readport(I.regs.w[DX] + 1))); };
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
            }

            static void i_outsb() /* Opcode 0x6e */
            {
                i86_ICount[0] -= 5;
                cpu_writeport(I.regs.w[DX], ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask)))); i86_ICount[0] -= 6;
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
            }

            static void i_outsw() /* Opcode 0x6f */
            {
                i86_ICount[0] -= 5;
                cpu_writeport(I.regs.w[DX], ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask)))); i86_ICount[0] -= 6;
                cpu_writeport(I.regs.w[DX] + 1, ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI] + 1)) & I.amask)))); i86_ICount[0] -= 6;
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
            }

            static void i_jo() /* Opcode 0x70 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.OverVal != 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jno() /* Opcode 0x71 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (!(I.OverVal != 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jb() /* Opcode 0x72 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.CarryVal != 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnb() /* Opcode 0x73 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (!(I.CarryVal != 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jz() /* Opcode 0x74 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.ZeroVal == 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnz() /* Opcode 0x75 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (!(I.ZeroVal == 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jbe() /* Opcode 0x76 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.CarryVal != 0) || (I.ZeroVal == 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnbe() /* Opcode 0x77 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (!((I.CarryVal != 0) || (I.ZeroVal == 0)))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_js() /* Opcode 0x78 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.SignVal < 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jns() /* Opcode 0x79 */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (!(I.SignVal < 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jp() /* Opcode 0x7a */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (parity_table[(byte)I.ParityVal] != 0)
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnp() /* Opcode 0x7b */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (parity_table[(byte)I.ParityVal] == 0)
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jl() /* Opcode 0x7c */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (((I.SignVal < 0) != (I.OverVal != 0)) && !(I.ZeroVal == 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnl() /* Opcode 0x7d */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.ZeroVal == 0) || ((I.SignVal < 0) == (I.OverVal != 0)))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jle() /* Opcode 0x7e */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if ((I.ZeroVal == 0) || ((I.SignVal < 0) != (I.OverVal != 0)))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_jnle() /* Opcode 0x7f */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                if (((I.SignVal < 0) == (I.OverVal != 0)) && !(I.ZeroVal == 0))
                {
                    I.ip = (ushort)(I.ip + tmp);
                    i86_ICount[0] -= 16;
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 4;
            }

            static void i_80pre() /* Opcode 0x80 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                uint src = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* ADD eb,d8 */
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x08: /* OR eb,d8 */
                        dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x10: /* ADC eb,d8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x18: /* SBB eb,b8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x20: /* AND eb,d8 */
                        dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x28: /* SUB eb,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x30: /* XOR eb,d8 */
                        dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x38: /* CMP eb,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        break;
                }
            }


            static void i_81pre() /* Opcode 0x81 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                uint src = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 2;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* ADD ew,d16 */
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x08: /* OR ew,d16 */
                        dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x10: /* ADC ew,d16 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x18: /* SBB ew,d16 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x20: /* AND ew,d16 */
                        dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x28: /* SUB ew,d16 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x30: /* XOR ew,d16 */
                        dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x38: /* CMP ew,d16 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        break;
                }
            }

            static void i_82pre() /* Opcode 0x82 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                uint src = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 2;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* ADD eb,d8 */
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x08: /* OR eb,d8 */
                        dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x10: /* ADC eb,d8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x18: /* SBB eb,d8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x20: /* AND eb,d8 */
                        dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x28: /* SUB eb,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x30: /* XOR eb,d8 */
                        dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                        break;
                    case 0x38: /* CMP eb,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x100); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x80); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                        break;
                }
            }

            static void i_83pre() /* Opcode 0x83 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM]();
                    i86_ICount[0] -= 10;
                    dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                uint src = (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)))));
                i86_ICount[0] -= 2;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* ADD ew,d8 */
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x08: /* OR ew,d8 */
                        dst |= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x10: /* ADC ew,d8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst + src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((res ^ src) & (res ^ dst) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x18: /* SBB ew,d8 */
                        src += (I.CarryVal != 0) ? 1u : 0u;
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x20: /* AND ew,d8 */
                        dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x28: /* SUB ew,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x30: /* XOR ew,d8 */
                        dst ^= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                        break;
                    case 0x38: /* CMP ew,d8 */
                        { uint res = dst - src; I.CarryVal = (int)(res & 0x10000); I.OverVal = (int)((dst ^ src) & (dst ^ res) & 0x8000); I.AuxVal = (int)((res ^ (src ^ dst)) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                        break;
                }
            }

            static void i_test_br8() /* Opcode 0x84 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
            }

            static void i_test_wr16() /* Opcode 0x85 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 3;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
            }

            static void i_xchg_br8() /* Opcode 0x86 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; dst = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 4;
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = (byte)dst;
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)src; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)src); }; };
            }

            static void i_xchg_wr16() /* Opcode 0x87 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                uint dst;
                if ((ModRM) >= 0xc0)
                    dst = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM]();
                    i86_ICount[0] -= 10;
                    dst = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 4;
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)dst;
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)src; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(src)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((src) >> 8)); }; };
            }

            static void i_mov_br8() /* Opcode 0x88 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                byte src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
                i86_ICount[0] -= 2;
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = src; else { i86_ICount[0] -= 7; cpu_writemem20((int)((GetEA[ModRM]()) & I.amask), (int)src); }; };
            }

            static void i_mov_wr16() /* Opcode 0x89 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
                i86_ICount[0] -= 2;
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = src; else { GetEA[ModRM](); { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(src)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((src) >> 8)); }; } };
            }

            static void i_mov_r8b() /* Opcode 0x8a */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                byte src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                i86_ICount[0] -= 2;
                I.regs.b[(int)Mod_RM.reg.b[ModRM]] = src;
            }

            static void i_mov_r16w() /* Opcode 0x8b */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (ushort)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }
                i86_ICount[0] -= 2;
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = src;
            }

            static void i_mov_wsreg() /* Opcode 0x8c */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 2;
                if ((ModRM & 0x20) != 0) return; /* HJB 12/13/98 1xx is invalid */
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = I.sregs[(ModRM & 0x38) >> 3]; else { GetEA[ModRM](); { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(I.sregs[(ModRM & 0x38) >> 3])); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((I.sregs[(ModRM & 0x38) >> 3]) >> 8)); }; } };
            }

            static void i_lea() /* Opcode 0x8d */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 2;
                GetEA[ModRM]();
                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = (ushort)EO; /* HJB 12/13/98 effective offset (no segment part) */
            }

            static void i_mov_sregw() /* Opcode 0x8e */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (ushort)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)((EA) + 1) & I.amask) << 8)));
                }

                i86_ICount[0] -= 2;
                switch (ModRM & 0x38)
                {
                    case 0x00: /* mov es,ew */
                        I.sregs[ES] = src;
                        I._base[ES] = (uint)(I.sregs[ES] << 4);
                        break;
                    case 0x18: /* mov ds,ew */
                        I.sregs[DS] = src;
                        I._base[DS] = (uint)(I.sregs[DS] << 4);
                        break;
                    case 0x10: /* mov ss,ew */
                        I.sregs[SS] = src;
                        I._base[SS] = (uint)(I.sregs[SS] << 4); /* no interrupt allowed before next instr */
                        instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
                        break;
                    case 0x08: /* mov cs,ew */
                        break; /* doesn't do a jump far */
                }
            }

            static void i_popw() /* Opcode 0x8f */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort tmp;
                { tmp = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                i86_ICount[0] -= 4;
                { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = tmp; else { GetEA[ModRM](); { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((tmp) >> 8)); }; } };
            }
            static void i_nop() /* Opcode 0x90 */
            {
                /* this is XchgAXReg(AX); */
                i86_ICount[0] -= 3;
            }

            static void i_xchg_axcx() /* Opcode 0x91 */
            {
                { ushort tmp; tmp = I.regs.w[CX]; I.regs.w[CX] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axdx() /* Opcode 0x92 */
            {
                { ushort tmp; tmp = I.regs.w[DX]; I.regs.w[DX] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axbx() /* Opcode 0x93 */
            {
                { ushort tmp; tmp = I.regs.w[BX]; I.regs.w[BX] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axsp() /* Opcode 0x94 */
            {
                { ushort tmp; tmp = I.regs.w[SP]; I.regs.w[SP] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axbp() /* Opcode 0x95 */
            {
                { ushort tmp; tmp = I.regs.w[BP]; I.regs.w[BP] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axsi() /* Opcode 0x96 */
            {
                { ushort tmp; tmp = I.regs.w[SI]; I.regs.w[SI] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_xchg_axdi() /* Opcode 0x97 */
            {
                { ushort tmp; tmp = I.regs.w[DI]; I.regs.w[DI] = I.regs.w[AX]; I.regs.w[AX] = tmp; i86_ICount[0] -= 3; };
            }

            static void i_cbw() /* Opcode 0x98 */
            {
                i86_ICount[0] -= 2;
                I.regs.b[AH] = (I.regs.b[AL] & 0x80) != 0 ? (byte)0xff : (byte)0;
            }

            static void i_cwd() /* Opcode 0x99 */
            {
                i86_ICount[0] -= 5;
                I.regs.w[DX] = (I.regs.b[AH] & 0x80) != 0 ? (ushort)0xffff : (ushort)0;
            }

            static void i_call_far()
            {
                uint tmp, tmp2;

                tmp = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8;

                tmp2 = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp2 += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8;

                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.sregs[CS])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.sregs[CS]) >> 8)); }; };
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.ip)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.ip) >> 8)); }; };

                I.ip = (ushort)tmp;
                I.sregs[CS] = (ushort)tmp2;
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                i86_ICount[0] -= 14;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_wait() /* Opcode 0x9b */
            {
                i86_ICount[0] -= 4;
            }
            static void i_popf() /* Opcode 0x9d */
            {
                uint tmp = (uint)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2;
                i86_ICount[0] -= 2;
                { I.CarryVal = (int)((tmp) & 1); I.ParityVal = (((tmp) & 4) == 0 ? 1 : 0); I.AuxVal = (int)((tmp) & 16); I.ZeroVal = (((tmp) & 64) == 0 ? 1 : 0); I.SignVal = ((tmp) & 128) != 0 ? -1 : 0; I.TF = (((tmp) & 256) == 256) ? (byte)1 : (byte)0; I.IF = (((tmp) & 512) == 512) ? (byte)1 : (byte)0; I.DF = (((tmp) & 1024) == 1024) ? (byte)1 : (byte)0; I.OverVal = (int)((tmp) & 2048); };

                if (I.TF != 0) trap();
            }
            static void i_sahf() /* Opcode 0x9e */
            {
                uint tmp = (uint)(((ushort)((I.CarryVal != 0) ? 1 : 0 | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal != 0) ? 1 : 0 << 4) | ((I.ZeroVal == 0) ? 1 : 0 << 6) | ((I.SignVal < 0) ? 1 : 0 << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal != 0) ? 1 : 0 << 11)) & 0xff00) | (I.regs.b[AH] & 0xd5));

                { I.CarryVal = (int)((tmp) & 1); I.ParityVal = (((tmp) & 4) == 0 ? 1 : 0); I.AuxVal = (int)((tmp) & 16); I.ZeroVal = (((tmp) & 64) == 0 ? 1 : 0); I.SignVal = ((tmp) & 128) != 0 ? -1 : 0; I.TF = (((tmp) & 256) == 256) ? (byte)1 : (byte)0; I.IF = (((tmp) & 512) == 512) ? (byte)1 : (byte)0; I.DF = (((tmp) & 1024) == 1024) ? (byte)1 : (byte)0; I.OverVal = (int)((tmp) & 2048); };
            }

            static void i_lahf() /* Opcode 0x9f */
            {
                I.regs.b[AH] = (byte)((ushort)((I.CarryVal != 0) ? (byte)1 : (byte)0 | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal != 0) ? (byte)1 : (byte)0 << 4) | ((I.ZeroVal == 0) ? (byte)1 : (byte)0 << 6) | ((I.SignVal < 0) ? (byte)1 : (byte)0 << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal != 0) ? (byte)1 : (byte)0 << 11)) & 0xff);
                i86_ICount[0] -= 4;
            }

            static void i_mov_aldisp() /* Opcode 0xa0 */
            {
                uint addr;

                addr = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                addr += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                i86_ICount[0] -= 4;
                I.regs.b[AL] = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr)) & I.amask))); i86_ICount[0] -= 6;
            }

            static void i_mov_axdisp() /* Opcode 0xa1 */
            {
                uint addr;

                addr = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                addr += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                i86_ICount[0] -= 4;
                I.regs.b[AL] = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr)) & I.amask))); i86_ICount[0] -= 6;
                I.regs.b[AH] = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr + 1)) & I.amask))); i86_ICount[0] -= 6;
            }

            static void i_mov_dispal() /* Opcode 0xa2 */
            {
                uint addr;

                addr = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                addr += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                i86_ICount[0] -= 3;
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr)) & I.amask), (int)(I.regs.b[AL])); };
            }

            static void i_mov_dispax() /* Opcode 0xa3 */
            {
                uint addr;

                addr = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                addr += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                i86_ICount[0] -= 3;
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr)) & I.amask), (int)(I.regs.b[AL])); };
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (addr + 1)) & I.amask), (int)(I.regs.b[AH])); };
            }

            static void i_movsb() /* Opcode 0xa4 */
            {
                byte tmp = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask))); i86_ICount[0] -= 6;
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)(tmp)); };
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
                I.regs.w[SI] += (ushort)(-2 * I.DF + 1);
                i86_ICount[0] -= 5;
            }

            static void i_movsw() /* Opcode 0xa5 */
            {
                ushort tmp = (ushort)(((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask)))
                    + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + ((I.regs.w[SI]) + 1)) & I.amask))) << 8)));
                i86_ICount[0] -= 6;
                i86_ICount[0] -= 6;
                i86_ICount[0] -= 10;
                { i86_ICount[0] -= 11; { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)((byte)(tmp))); }; { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + ((I.regs.w[DI]) + 1)) & I.amask), (int)((byte)((tmp) >> 8))); }; };
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
                I.regs.w[SI] += (ushort)(-4 * I.DF + 2);
                i86_ICount[0] -= 5;
            }

            static void i_cmpsb() /* Opcode 0xa6 */
            {
                uint dst = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask))); i86_ICount[0] -= 6;
                uint src = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask))); i86_ICount[0] -= 6;
                { uint res = src - dst; I.CarryVal = (int)((res) & 0x100); I.OverVal = (int)(((src) ^ (dst)) & ((src) ^ (res)) & 0x80); I.AuxVal = (int)(((res) ^ ((dst) ^ (src))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); src = (byte)res; }; /* opposite of the usual convention */
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
                I.regs.w[SI] += (ushort)(-2 * I.DF + 1);
                i86_ICount[0] -= 10;
            }

            static void i_cmpsw() /* Opcode 0xa7 */
            {
                uint dst = (uint)((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask))) +
                    (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + ((I.regs.w[DI]) + 1)) & I.amask))) << 8)); i86_ICount[0] -= 6; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                uint src = (uint)((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask))) + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + ((I.regs.w[SI]) + 1)) & I.amask))) << 8)); i86_ICount[0] -= 6; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                { uint res = src - dst; I.CarryVal = (int)((res) & 0x10000); I.OverVal = (int)(((src) ^ (dst)) & ((src) ^ (res)) & 0x8000); I.AuxVal = (int)(((res) ^ ((dst) ^ (src))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); src = (ushort)res; }; /* opposite of the usual convention */
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
                I.regs.w[SI] += (ushort)(-4 * I.DF + 2);
                i86_ICount[0] -= 10;
            }

            static void i_test_ald8() /* Opcode 0xa8 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.b[AL];
                i86_ICount[0] -= 4;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
            }

            static void i_test_axd16() /* Opcode 0xa9 */
            {
                uint src = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))); uint dst = I.regs.w[AX];
                src += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                i86_ICount[0] -= 4;
                dst &= src; I.CarryVal = I.OverVal = I.AuxVal = 0; I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
            }

            static void i_stosb() /* Opcode 0xaa */
            {
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)(I.regs.b[AL])); };
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
                i86_ICount[0] -= 4;
            }

            static void i_stosw() /* Opcode 0xab */
            {
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask), (int)(I.regs.b[AL])); };
                { i86_ICount[0] -= 7; cpu_writemem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI] + 1)) & I.amask), (int)(I.regs.b[AH])); };
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
                i86_ICount[0] -= 4;
            }

            static void i_lodsb() /* Opcode 0xac */
            {
                I.regs.b[AL] = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask))); i86_ICount[0] -= 6;
                I.regs.w[SI] += (ushort)(-2 * I.DF + 1);
                i86_ICount[0] -= 6;
            }

            static void i_lodsw() /* Opcode 0xad */
            {
                I.regs.w[AX] = (ushort)((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (I.regs.w[SI])) & I.amask))) + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + ((I.regs.w[SI]) + 1)) & I.amask))) << 8)); i86_ICount[0] -= 6; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                I.regs.w[SI] += (ushort)(-4 * I.DF + 2);
                i86_ICount[0] -= 6;
            }

            static void i_scasb() /* Opcode 0xae */
            {
                uint src = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask))); i86_ICount[0] -= 6;
                uint dst = I.regs.b[AL];
                { uint res = dst - src; I.CarryVal = (int)((res) & 0x100); I.OverVal = (int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal = (int)(((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); dst = (byte)res; };
                I.regs.w[DI] += (ushort)(-2 * I.DF + 1);
                i86_ICount[0] -= 9;
            }

            static void i_scasw() /* Opcode 0xaf */
            {
                uint src = (ushort)((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + (I.regs.w[DI])) & I.amask))) + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (ES == DS || ES == SS)) ? prefix_base : I._base[ES]) + ((I.regs.w[DI]) + 1)) & I.amask))) << 8)); i86_ICount[0] -= 6; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                uint dst = I.regs.w[AX];
                { uint res = dst - src; I.CarryVal = (int)((res) & 0x10000); I.OverVal = (int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal = (int)(((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); dst = (ushort)res; };
                I.regs.w[DI] += (ushort)(-4 * I.DF + 2);
                i86_ICount[0] -= 9;
            }

            static void i_mov_ald8() /* Opcode 0xb0 */
            {
                I.regs.b[AL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_cld8() /* Opcode 0xb1 */
            {
                I.regs.b[CL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_dld8() /* Opcode 0xb2 */
            {
                I.regs.b[DL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_bld8() /* Opcode 0xb3 */
            {
                I.regs.b[BL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_ahd8() /* Opcode 0xb4 */
            {
                I.regs.b[AH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_chd8() /* Opcode 0xb5 */
            {
                I.regs.b[CH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_dhd8() /* Opcode 0xb6 */
            {
                I.regs.b[DH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_bhd8() /* Opcode 0xb7 */
            {
                I.regs.b[BH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_axd16() /* Opcode 0xb8 */
            {
                I.regs.b[AL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[AH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_cxd16() /* Opcode 0xb9 */
            {
                I.regs.b[CL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[CH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_dxd16() /* Opcode 0xba */
            {
                I.regs.b[DL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[DH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_bxd16() /* Opcode 0xbb */
            {
                I.regs.b[BL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[BH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_spd16() /* Opcode 0xbc */
            {
                I.regs.b[SPL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[SPH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_bpd16() /* Opcode 0xbd */
            {
                I.regs.b[BPL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[BPH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_sid16() /* Opcode 0xbe */
            {
                I.regs.b[SIL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[SIH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void i_mov_did16() /* Opcode 0xbf */
            {
                I.regs.b[DIL] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                I.regs.b[DIH] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
            }

            static void rotate_shift_Byte(uint ModRM, uint count)
            {
                uint src;
                if ((ModRM) >= 0xc0)
                    src = (uint)I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6;
                    src = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                uint dst = src;

                if (count == 0)
                {
                    i86_ICount[0] -= 8; /* or 7 if dest is in memory */
                }
                else if (count == 1)
                {
                    i86_ICount[0] -= 2;
                    switch (ModRM & 0x38)
                    {
                        case 0x00: /* ROL eb,1 */
                            I.CarryVal = (int)(src & 0x80);
                            dst = (uint)((src << 1) + ((I.CarryVal != 0) ? 1 : 0));
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.OverVal = (int)((src ^ dst) & 0x80);
                            break;
                        case 0x08: /* ROR eb,1 */
                            I.CarryVal = (int)(src & 0x01);
                            dst = (uint)((((I.CarryVal != 0) ? 1 : 0 << 8) + src) >> 1);
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.OverVal = (int)((src ^ dst) & 0x80);
                            break;
                        case 0x10: /* RCL eb,1 */
                            dst = (uint)((src << 1) + ((I.CarryVal != 0) ? 1 : 0));
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.CarryVal = (int)((dst) & 0x100);
                            I.OverVal = (int)((src ^ dst) & 0x80);
                            break;
                        case 0x18: /* RCR eb,1 */
                            dst = (uint)((((I.CarryVal != 0) ? 1 : 0 << 8) + src) >> 1);
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = (int)((src ^ dst) & 0x80);
                            break;
                        case 0x20: /* SHL eb,1 */
                        case 0x30:
                            dst = src << 1;
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.CarryVal = (int)((dst) & 0x100);
                            I.OverVal = (int)((src ^ dst) & 0x80);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            break;
                        case 0x28: /* SHR eb,1 */
                            dst = src >> 1;
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = (int)(src & 0x80);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            break;
                        case 0x38: /* SAR eb,1 */
                            dst = (uint)((sbyte)src) >> 1;
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)dst); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = 0;
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            break;
                    }
                }
                else
                {
                    i86_ICount[0] -= (int)(8 + 4 * count); /* or 7+4*count if dest is in memory */
                    switch (ModRM & 0x38)
                    {
                        case 0x00: /* ROL eb,count */
                            for (; count > 0; count--)
                            {
                                I.CarryVal = (int)(dst & 0x80);
                                dst = (uint)((dst << 1) + ((I.CarryVal != 0) ? 1 : 0));
                            }
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x08: /* ROR eb,count */
                            for (; count > 0; count--)
                            {
                                I.CarryVal = (int)(dst & 0x01);
                                dst = (uint)((dst >> 1) + ((I.CarryVal != 0) ? 1 : 0 << 7));
                            }
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x10: /* RCL eb,count */
                            for (; count > 0; count--)
                            {
                                dst = (uint)((dst << 1) + ((I.CarryVal != 0) ? 1 : 0));
                                I.CarryVal = (int)((dst) & 0x100);
                            }
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x18: /* RCR eb,count */
                            for (; count > 0; count--)
                            {
                                dst = (uint)((I.CarryVal != 0) ? 1 : 0 << 8) + dst;
                                I.CarryVal = (int)(dst & 0x01);
                                dst >>= 1;
                            }
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x20:
                        case 0x30: /* SHL eb,count */
                            dst <<= (int)count;
                            I.CarryVal = (int)((dst) & 0x100);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x28: /* SHR eb,count */
                            dst >>= (int)count - 1;
                            I.CarryVal = (int)(dst & 0x1);
                            dst >>= 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            I.AuxVal = 1;
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                        case 0x38: /* SAR eb,count */
                            dst = (uint)(((sbyte)dst) >> (int)(count - 1));
                            I.CarryVal = (int)(dst & 0x1);
                            dst = (uint)((sbyte)((byte)dst)) >> 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(dst);
                            I.AuxVal = 1;
                            { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)dst; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)dst); }; };
                            break;
                    }
                }
            }

            static void rotate_shift_Word(uint ModRM, uint count)
            {
                uint src;
                if ((ModRM) >= 0xc0)
                    src = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; src = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }
                uint dst = src;

                if (count == 0)
                {
                    i86_ICount[0] -= 8; /* or 7 if dest is in memory */
                }
                else if (count == 1)
                {
                    i86_ICount[0] -= 2;
                    switch (ModRM & 0x38)
                    {
                        case 0x00: /* ROL ew,1 */
                            I.CarryVal = (int)(src & 0x8000);
                            dst = (uint)((src << 1) + ((I.CarryVal != 0) ? 1 : 0));
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.OverVal = (int)((src ^ dst) & 0x8000);
                            break;
                        case 0x08: /* ROR ew,1 */
                            I.CarryVal = (int)(src & 0x01);
                            dst = (uint)(((I.CarryVal != 0) ? 1 : 0 << 16) + src) >> 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.OverVal = (int)((src ^ dst) & 0x8000);
                            break;
                        case 0x10: /* RCL ew,1 */
                            dst = (src << 1) + ((I.CarryVal != 0) ? 1u : 0u);
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.CarryVal = (int)((dst) & 0x10000);
                            I.OverVal = (int)((src ^ dst) & 0x8000);
                            break;
                        case 0x18: /* RCR ew,1 */
                            dst = (((I.CarryVal != 0) ? 1u : 0u << 16) + src) >> 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = (int)((src ^ dst) & 0x8000);
                            break;
                        case 0x20: /* SHL ew,1 */
                        case 0x30:
                            dst = src << 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.CarryVal = (int)((dst) & 0x10000);
                            I.OverVal = (int)((src ^ dst) & 0x8000);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            break;
                        case 0x28: /* SHR ew,1 */
                            dst = src >> 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = (int)(src & 0x8000);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            break;
                        case 0x38: /* SAR ew,1 */
                            dst = (uint)(((short)src) >> 1);
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            I.CarryVal = (int)(src & 0x01);
                            I.OverVal = 0;
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            break;

                    }
                }
                else
                {
                    i86_ICount[0] -= (int)(+4 * count); /* or 7+4*count if dest is in memory */

                    switch (ModRM & 0x38)
                    {
                        case 0x00: /* ROL ew,count */
                            for (; count > 0; count--)
                            {
                                I.CarryVal = (int)(dst & 0x8000);
                                dst = (dst << 1) + ((I.CarryVal != 0) ? 1u : 0u);
                            }
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x08: /* ROR ew,count */
                            for (; count > 0; count--)
                            {
                                I.CarryVal = (int)(dst & 0x01);
                                dst = (dst >> 1) + ((I.CarryVal != 0) ? 1u : 0u << 15);
                            }
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x10: /* RCL ew,count */
                            for (; count > 0; count--)
                            {
                                dst = (dst << 1) + ((I.CarryVal != 0) ? 1u : 0u);
                                I.CarryVal = (int)((dst) & 0x10000);
                            }
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x18: /* RCR ew,count */
                            for (; count > 0; count--)
                            {
                                dst = dst + ((I.CarryVal != 0) ? 1u : 0u << 16);
                                I.CarryVal = (int)(dst & 0x01);
                                dst >>= 1;
                            }
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x20:
                        case 0x30: /* SHL ew,count */
                            dst <<= (int)count;
                            I.CarryVal = (int)((dst) & 0x10000);
                            I.AuxVal = 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x28: /* SHR ew,count */
                            dst >>= (int)count - 1;
                            I.CarryVal = (int)(dst & 0x1);
                            dst >>= 1;
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            I.AuxVal = 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                        case 0x38: /* SAR ew,count */
                            dst = (uint)(((short)dst) >> (int)(count - 1));
                            I.CarryVal = (int)(dst & 0x01);
                            dst = (uint)(((short)((ushort)dst)) >> 1);
                            I.SignVal = I.ZeroVal = I.ParityVal = (short)(dst);
                            I.AuxVal = 1;
                            { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)dst; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(dst)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((dst) >> 8)); }; };
                            break;
                    }
                }
            }


            static void i_rotshft_bd8() /* Opcode 0xc0 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint count = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                rotate_shift_Byte(ModRM, count);
            }

            static void i_rotshft_wd8() /* Opcode 0xc1 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint count = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                rotate_shift_Word(ModRM, count);
            }


            static void i_ret_d16() /* Opcode 0xc2 */
            {
                uint count = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                count += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                { I.ip = (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I.regs.w[SP] += (ushort)count;
                i86_ICount[0] -= 14;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_ret() /* Opcode 0xc3 */
            {
                I.ip = (cpu_readmem20((int)((((I._base[SS]+I.regs.w[SP])&I.amask))&I.amask)+(cpu_readmem20((int)(((((I._base[SS]+I.regs.w[SP])&I.amask))+1)&I.amask)<<8))));
                i86_ICount[0] -= 10;
                I.regs.w[SP] += 2; 
                i86_ICount[0] -= 10;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_les_dw() /* Opcode 0xc4 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort tmp;
                if ((ModRM) >= 0xc0)
                    tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (ushort)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }

                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = tmp;
                I.sregs[ES] = (ushort)(cpu_readmem20((int)((EA + 2) & I.amask) + (cpu_readmem20((int)(((EA + 2) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                I._base[ES] = (uint)(I.sregs[ES] << 4);
                i86_ICount[0] -= 4;
            }

            static void i_lds_dw() /* Opcode 0xc5 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                ushort tmp;
                if ((ModRM) >= 0xc0)
                    tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (ushort)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }

                I.regs.w[(int)Mod_RM.reg.w[ModRM]] = tmp;
                I.sregs[DS] = (ushort)(cpu_readmem20((int)((EA + 2) & I.amask) + (cpu_readmem20((int)(((EA + 2) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                I._base[DS] = (uint)(I.sregs[DS] << 4);
                i86_ICount[0] -= 4;
            }

            static void i_mov_bd8() /* Opcode 0xc6 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))); else { GetEA[ModRM](); { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)))); }; } };
            }

            static void i_mov_wd16() /* Opcode 0xc7 */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 4;
                { ushort val; if (ModRM >= 0xc0) { I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)(cpu_readop_arg((uint)(((I._base[CS] + I.ip) & I.amask))) + (cpu_readop_arg((uint)(((I._base[CS] + I.ip + 1) & I.amask)) << 8))); I.ip += 2; } else { GetEA[ModRM](); { val = (ushort)(cpu_readop_arg((uint)(((I._base[CS] + I.ip) & I.amask))) + (cpu_readop_arg((uint)(((I._base[CS] + I.ip + 1) & I.amask)) << 8))); I.ip += 2; } { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(val)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((val) >> 8)); }; } };
            }

            static void i_enter() /* Opcode 0xc8 */
            {
                uint nb = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint i, level;

                i86_ICount[0] -= 11;
                nb += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                level = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BP])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BP]) >> 8)); }; };
                I.regs.w[BP] = I.regs.w[SP];
                I.regs.w[SP] -= (ushort)nb;
                for (i = 1; i < level; i++)
                {
                    {
                        I.regs.w[SP] -= 2;
                        {
                            i86_ICount[0] -= 11;
                            cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + (I.regs.w[BP] - i * 2)) & I.amask))) + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + ((I.regs.w[BP] - i * 2) + 1)) & I.amask))) << 8)))); i86_ICount[0] -= 6; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                            cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((((ushort)((byte)cpu_readmem20((int)((((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + (I.regs.w[BP] - i * 2)) & I.amask))) + (ushort)(((byte)cpu_readmem20((int)((((seg_prefix != 0 && (SS == DS || SS == SS)) ? prefix_base : I._base[SS]) + ((I.regs.w[BP] - i * 2) + 1)) & I.amask))) << 8))) >> 8)); i86_ICount[0] -= 6;
                        };
                    }; i86_ICount[0] -= 6; i86_ICount[0] -= 10;
                    i86_ICount[0] -= 4;
                }
                if (level != 0) { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.regs.w[BP])); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.regs.w[BP]) >> 8)); }; };
            }

            static void i_leave() /* Opcode 0xc9 */
            {
                i86_ICount[0] -= 5;
                I.regs.w[SP] = I.regs.w[BP];
                { I.regs.w[BP] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
            }

            static void i_retf_d16() /* Opcode 0xca */
            {
                uint count = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                count += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);
                { I.ip = (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.sregs[CS] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                I.regs.w[SP] += (ushort)count;
                i86_ICount[0] -= 13;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_retf() /* Opcode 0xcb */
            {
                { I.ip = (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.sregs[CS] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                i86_ICount[0] -= 14;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_int3() /* Opcode 0xcc */
            {
                i86_ICount[0] -= 16;
                i86_interrupt(3);
            }

            static void i_int() /* Opcode 0xcd */
            {
                uint int_num = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 15;
                i86_interrupt(int_num);
            }

            static void i_into() /* Opcode 0xce */
            {
                if ((I.OverVal != 0))
                {
                    i86_ICount[0] -= 17;
                    i86_interrupt(4);
                }
                else i86_ICount[0] -= 4;
            }

            static void i_iret() /* Opcode 0xcf */
            {
                i86_ICount[0] -= 12;
                { I.ip = (cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                { I.sregs[CS] = (ushort)(cpu_readmem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask) + (cpu_readmem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10; I.regs.w[SP] += 2; };
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                i_popf();
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_rotshft_b() /* Opcode 0xd0 */
            {
                rotate_shift_Byte(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))), 1);
            }


            static void i_rotshft_w() /* Opcode 0xd1 */
            {
                rotate_shift_Word(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))), 1);
            }


            static void i_rotshft_bcl() /* Opcode 0xd2 */
            {
                rotate_shift_Byte(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))), I.regs.b[CL]);
            }

            static void i_rotshft_wcl() /* Opcode 0xd3 */
            {
                rotate_shift_Word(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))), I.regs.b[CL]);
            }

            static void i_aam() /* Opcode 0xd4 */
            {
                uint mult = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 83;
                if (mult == 0)
                    i86_interrupt(0);
                else
                {
                    I.regs.b[AH] = (byte)(I.regs.b[AL] / mult);
                    I.regs.b[AL] %= (byte)mult;

                    I.SignVal = I.ZeroVal = I.ParityVal = (short)(I.regs.w[AX]);
                }
            }


            static void i_aad() /* Opcode 0xd5 */
            {
                uint mult = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 60;
                I.regs.b[AL] = (byte)(I.regs.b[AH] * mult + I.regs.b[AL]);
                I.regs.b[AH] = 0;

                I.ZeroVal = (int)(I.regs.b[AL]);
                I.ParityVal = (int)(I.regs.b[AL]);
                I.SignVal = 0;
            }

            static void i_xlat() /* Opcode 0xd7 */
            {
                uint dest = (uint)I.regs.w[BX] + I.regs.b[AL];

                i86_ICount[0] -= 5;
                I.regs.b[AL] = ((byte)cpu_readmem20((int)((((seg_prefix != 0 && (DS == DS || DS == SS)) ? prefix_base : I._base[DS]) + (dest)) & I.amask))); i86_ICount[0] -= 6;
            }

            static void i_escape() /* Opcodes 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde and 0xdf */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                i86_ICount[0] -= 2;
                int a = 0;
                if ((ModRM) >= 0xc0)
                    a = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; var b = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
            }

            static void i_loopne() /* Opcode 0xe0 */
            {
                int disp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                uint tmp = (uint)(I.regs.w[CX] - 1);

                I.regs.w[CX] = (ushort)tmp;

                if (!(I.ZeroVal == 0) && tmp != 0)
                {
                    i86_ICount[0] -= 19;
                    I.ip = (ushort)(I.ip + disp);
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 5;
            }

            static void i_loope() /* Opcode 0xe1 */
            {
                int disp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                uint tmp = (uint)(I.regs.w[CX] - 1);

                I.regs.w[CX] = (ushort)tmp;

                if ((I.ZeroVal == 0) && tmp != 0)
                {
                    i86_ICount[0] -= 18;
                    I.ip = (ushort)(I.ip + disp);
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 6;
            }

            static void i_loop() /* Opcode 0xe2 */
            {
                int disp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                uint tmp = (uint)(I.regs.w[CX] - 1);

                I.regs.w[CX] = (ushort)tmp;

                if (tmp != 0)
                {
                    i86_ICount[0] -= 17;
                    I.ip = (ushort)(I.ip + disp);
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 5;
            }

            static void i_jcxz() /* Opcode 0xe3 */
            {
                int disp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));

                if (I.regs.w[CX] == 0)
                {
                    i86_ICount[0] -= 18;
                    I.ip = (ushort)(I.ip + disp);
                    change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                }
                else i86_ICount[0] -= 6;
            }

            static void i_inal() /* Opcode 0xe4 */
            {
                uint port = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 10;
                I.regs.b[AL] = (byte)cpu_readport((int)port);
            }

            static void i_inax() /* Opcode 0xe5 */
            {
                uint port = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 14;
                I.regs.b[AL] = (byte)cpu_readport((int)port);
                I.regs.b[AH] = (byte)cpu_readport((int)port + 1);
            }

            static void i_outal() /* Opcode 0xe6 */
            {
                uint port = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 10;
                cpu_writeport((int)port, I.regs.b[AL]);
            }

            static void i_outax() /* Opcode 0xe7 */
            {
                uint port = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));

                i86_ICount[0] -= 14;
                cpu_writeport((int)port, I.regs.b[AL]);
                cpu_writeport((int)port + 1, I.regs.b[AH]);
            }

            static void i_call_d16() /* Opcode 0xe8 */
            {
                uint tmp = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.ip)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.ip) >> 8)); }; };
                I.ip = (ushort)(I.ip + (short)tmp);
                i86_ICount[0] -= 12;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }


            static void i_jmp_d16() /* Opcode 0xe9 */
            {
                int tmp = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp += ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8;

                I.ip = (ushort)(I.ip + (short)tmp);
                i86_ICount[0] -= 15;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_jmp_far() /* Opcode 0xea */
            {
                uint tmp, tmp1;

                tmp = (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp += (uint)(((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                tmp1 = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                tmp1 += (uint)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))) << 8;

                I.sregs[CS] = (ushort)tmp1;
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                I.ip = (ushort)tmp;
                i86_ICount[0] -= 15;
                change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
            }

            static void i_jmp_d8() /* Opcode 0xeb */
            {
                int tmp = (int)((sbyte)((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask))));
                I.ip = (ushort)(I.ip + tmp);
                i86_ICount[0] -= 15;
            }

            static void i_inaldx() /* Opcode 0xec */
            {
                i86_ICount[0] -= 8;
                I.regs.b[AL] = (byte)cpu_readport(I.regs.w[DX]);
            }

            static void i_inaxdx() /* Opcode 0xed */
            {
                uint port = I.regs.w[DX];

                i86_ICount[0] -= 12;
                I.regs.b[AL] = (byte)cpu_readport((int)port);
                I.regs.b[AH] = (byte)cpu_readport((int)port + 1);
            }

            static void i_outdxal() /* Opcode 0xee */
            {
                i86_ICount[0] -= 8;
                cpu_writeport(I.regs.w[DX], I.regs.b[AL]);
            }

            static void i_outdxax() /* Opcode 0xef */
            {
                uint port = I.regs.w[DX];

                i86_ICount[0] -= 12;
                cpu_writeport((int)port, I.regs.b[AL]);
                cpu_writeport((int)port + 1, I.regs.b[AH]);
            }

            static void i_lock() /* Opcode 0xf0 */
            {
                i86_ICount[0] -= 2;
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))](); /* un-interruptible */
            }

            static void rep(int flagval)
            {
                /* Handles rep- and repnz- prefixes. flagval is the value of ZF for the

                   loop  to continue for CMPS and SCAS instructions. */
                uint next = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint count = I.regs.w[CX];

                switch (next)
                {
                    case 0x26: /* ES: */
                        seg_prefix = 1;
                        prefix_base = I._base[ES];
                        i86_ICount[0] -= 2;
                        rep(flagval);
                        break;
                    case 0x2e: /* CS: */
                        seg_prefix = 1;
                        prefix_base = I._base[CS];
                        i86_ICount[0] -= 2;
                        rep(flagval);
                        break;
                    case 0x36: /* SS: */
                        seg_prefix = 1;
                        prefix_base = I._base[SS];
                        i86_ICount[0] -= 2;
                        rep(flagval);
                        break;
                    case 0x3e: /* DS: */
                        seg_prefix = 1;
                        prefix_base = I._base[DS];
                        i86_ICount[0] -= 2;
                        rep(flagval);
                        break;
                    case 0x6c: /* REP INSB */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_insb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0x6d: /* REP INSW */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_insw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0x6e: /* REP OUTSB */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_outsb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0x6f: /* REP OUTSW */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_outsw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xa4: /* REP MOVSB */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_movsb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xa5: /* REP MOVSW */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_movsw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xa6: /* REP(N)E CMPSB */
                        i86_ICount[0] -= 9;
                        for (I.ZeroVal = flagval == 0 ? 1 : 0; ((I.ZeroVal == 0) == (flagval != 0)) && (count > 0); count--)
                            i_cmpsb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xa7: /* REP(N)E CMPSW */
                        i86_ICount[0] -= 9;
                        for (I.ZeroVal = flagval == 0 ? 1 : 0; ((I.ZeroVal == 0) == (flagval != 0)) && (count > 0); count--)
                            i_cmpsw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xaa: /* REP STOSB */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_stosb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xab: /* REP STOSW */
                        i86_ICount[0] -= (int)(9 - count);
                        for (; count > 0; count--)
                            i_stosw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xac: /* REP LODSB */
                        i86_ICount[0] -= 9;
                        for (; count > 0; count--)
                            i_lodsb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xad: /* REP LODSW */
                        i86_ICount[0] -= 9;
                        for (; count > 0; count--)
                            i_lodsw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xae: /* REP(N)E SCASB */
                        i86_ICount[0] -= 9;
                        for (I.ZeroVal = flagval == 0 ? 1 : 0; ((I.ZeroVal == 0) == (flagval != 0)) && (count > 0); count--)
                            i_scasb();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    case 0xaf: /* REP(N)E SCASW */
                        i86_ICount[0] -= 9;
                        for (I.ZeroVal = flagval == 0 ? 1 : 0; ((I.ZeroVal == 0) == (flagval != 0)) && (count > 0); count--)
                            i_scasw();
                        I.regs.w[CX] = (ushort)count;
                        break;
                    default:
                        instruction[next]();
                        break;
                }
            }

            static void i_repne() /* Opcode 0xf2 */
            {
                rep(0);
            }

            static void i_repe() /* Opcode 0xf3 */
            {
                rep(1);
            }

            static void i_hlt() /* Opcode 0xf4 */
            {
                i86_ICount[0] = 0;
            }

            static void i_cmc() /* Opcode 0xf5 */
            {
                i86_ICount[0] -= 2;
                I.CarryVal = !(I.CarryVal != 0) ? 1 : 0;
            }

            static void i_f6pre()
            {
                /* Opcode 0xf6 */
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint tmp;
                if ((ModRM) >= 0xc0)
                    tmp = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; tmp = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                uint tmp2;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* TEST Eb, data8 */
                    case 0x08: /* ??? */
                        i86_ICount[0] -= 5;
                        tmp &= ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));

                        I.CarryVal = I.OverVal = I.AuxVal = 0;
                        I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(tmp);
                        break;

                    case 0x10: /* NOT Eb */
                        i86_ICount[0] -= 3;
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)~tmp; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)~tmp); }; };
                        break;

                    case 0x18: /* NEG Eb */
                        i86_ICount[0] -= 3;
                        tmp2 = 0;
                        { uint res = tmp2 - tmp; I.CarryVal = (int)((res) & 0x100); I.OverVal = (int)(((tmp2) ^ (tmp)) & ((tmp2) ^ (res)) & 0x80); I.AuxVal = (int)(((res) ^ ((tmp) ^ (tmp2))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(res); tmp2 = (byte)res; };
                        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)tmp2; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)tmp2); }; };
                        break;
                    case 0x20: /* MUL AL, Eb */
                        i86_ICount[0] -= 77;
                        {
                            ushort result;
                            tmp2 = I.regs.b[AL];

                            I.SignVal = (int)((sbyte)tmp2);
                            I.ParityVal = (int)(tmp2);

                            result = (ushort)((ushort)tmp2 * tmp);
                            I.regs.w[AX] = (ushort)result;

                            I.ZeroVal = (int)(I.regs.w[AX]);
                            I.CarryVal = I.OverVal = (I.regs.b[AH] != 0) ? 1 : 0;
                        }
                        break;
                    case 0x28: /* IMUL AL, Eb */
                        i86_ICount[0] -= 80;
                        {
                            short result;

                            tmp2 = (uint)I.regs.b[AL];

                            I.SignVal = (int)((sbyte)tmp2);
                            I.ParityVal = (int)(tmp2);

                            result = (short)(((sbyte)tmp2) * (short)((sbyte)tmp));
                            I.regs.w[AX] = (ushort)result;

                            I.ZeroVal = (int)(I.regs.w[AX]);

                            I.CarryVal = I.OverVal = (result >> 7 != 0) && (result >> 7 != -1) ? 1 : 0;
                        }
                        break;
                    case 0x30: /* DIV AL, Ew */
                        i86_ICount[0] -= 90;
                        {
                            ushort result;

                            result = I.regs.w[AX];

                            if (tmp != 0)
                            {
                                if ((result / tmp) > 0xff)
                                {
                                    i86_interrupt(0);
                                    break;
                                }
                                else
                                {
                                    I.regs.b[AH] = (byte)(result % tmp);
                                    I.regs.b[AL] = (byte)(result / tmp);
                                }
                            }
                            else
                            {
                                i86_interrupt(0);
                                break;
                            }
                        }
                        break;
                    case 0x38: /* IDIV AL, Ew */
                        i86_ICount[0] -= 106;
                        {

                            short result;

                            result = (short)(I.regs.w[AX]);

                            if (tmp != 0)
                            {
                                tmp2 = (uint)(result % (short)((sbyte)tmp));

                                if ((result /= (short)((sbyte)tmp)) > 0xff)
                                {
                                    i86_interrupt(0);
                                    break;
                                }
                                else
                                {
                                    I.regs.b[AL] = (byte)(result);
                                    I.regs.b[AH] = (byte)(tmp2);
                                }
                            }
                            else
                            {
                                i86_interrupt(0);
                                break;
                            }
                        }
                        break;
                }
            }


            static void i_f7pre()
            {
                /* Opcode 0xf7 */
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint tmp;
                if ((ModRM) >= 0xc0)
                    tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                else
                {
                    GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                }
                uint tmp2;


                switch (ModRM & 0x38)
                {
                    case 0x00: /* TEST Ew, data16 */
                    case 0x08: /* ??? */
                        i86_ICount[0] -= 3;
                        tmp2 = ((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask)));
                        tmp2 += (uint)(((byte)cpu_readop_arg((uint)((I._base[CS] + I.ip++) & I.amask))) << 8);

                        tmp &= tmp2;

                        I.CarryVal = I.OverVal = I.AuxVal = 0;
                        I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp);
                        break;

                    case 0x10: /* NOT Ew */
                        i86_ICount[0] -= 3;
                        tmp = ~tmp;
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)tmp; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((tmp) >> 8)); }; };
                        break;

                    case 0x18: /* NEG Ew */
                        i86_ICount[0] -= 3;
                        tmp2 = 0;
                        { uint res = tmp2 - tmp; I.CarryVal = (int)((res) & 0x10000); I.OverVal = (int)(((tmp2) ^ (tmp)) & ((tmp2) ^ (res)) & 0x8000); I.AuxVal = (int)(((res) ^ ((tmp) ^ (tmp2))) & 0x10); I.SignVal = I.ZeroVal = I.ParityVal = (short)(res); tmp2 = (ushort)res; };
                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)tmp2; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)(tmp2)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)((tmp2) >> 8)); }; };
                        break;
                    case 0x20: /* MUL AX, Ew */
                        i86_ICount[0] -= 129;
                        {
                            uint result;
                            tmp2 = I.regs.w[AX];

                            I.SignVal = (int)((short)tmp2);
                            I.ParityVal = (int)(tmp2);

                            result = (uint)tmp2 * tmp;
                            I.regs.w[AX] = (ushort)result;
                            result >>= 16;
                            I.regs.w[DX] = (ushort)result;

                            I.ZeroVal = (int)(I.regs.w[AX] | I.regs.w[DX]);
                            I.CarryVal = I.OverVal = (I.regs.w[DX] != 0) ? 1 : 0;
                        }
                        break;

                    case 0x28: /* IMUL AX, Ew */
                        i86_ICount[0] -= 150;
                        {
                            int result;

                            tmp2 = I.regs.w[AX];

                            I.SignVal = (int)((short)tmp2);
                            I.ParityVal = (int)(tmp2);

                            result = (int)((short)tmp2) * (int)((short)tmp);
                            I.CarryVal = I.OverVal = (result >> 15 != 0) && (result >> 15 != -1) ? 1 : 0;

                            I.regs.w[AX] = (ushort)result;
                            result = (ushort)(result >> 16);
                            I.regs.w[DX] = (ushort)result;

                            I.ZeroVal = (int)(I.regs.w[AX] | I.regs.w[DX]);
                        }
                        break;
                    case 0x30: /* DIV AX, Ew */
                        i86_ICount[0] -= 158;
                        {
                            uint result;

                            result = (uint)((I.regs.w[DX] << 16) + I.regs.w[AX]);

                            if (tmp != 0)
                            {
                                tmp2 = result % tmp;
                                if ((result / tmp) > 0xffff)
                                {
                                    i86_interrupt(0);
                                    break;
                                }
                                else
                                {
                                    I.regs.w[DX] = (ushort)tmp2;
                                    result /= tmp;
                                    I.regs.w[AX] = (ushort)result;
                                }
                            }
                            else
                            {
                                i86_interrupt(0);
                                break;
                            }
                        }
                        break;
                    case 0x38: /* IDIV AX, Ew */
                        i86_ICount[0] -= 180;
                        {
                            int result;

                            result = (I.regs.w[DX] << 16) + I.regs.w[AX];

                            if (tmp != 0)
                            {
                                tmp2 = (uint)(result % (int)((short)tmp));
                                if ((result /= (int)((short)tmp)) > 0xffff)
                                {
                                    i86_interrupt(0);
                                    break;
                                }
                                else
                                {
                                    I.regs.w[AX] = (ushort)result;
                                    I.regs.w[DX] = (ushort)tmp2;
                                }
                            }
                            else
                            {
                                i86_interrupt(0);
                                break;
                            }
                        }
                        break;
                }
            }


            static void i_clc() /* Opcode 0xf8 */
            {
                i86_ICount[0] -= 2;
                I.CarryVal = 0;
            }

            static void i_stc() /* Opcode 0xf9 */
            {
                i86_ICount[0] -= 2;
                I.CarryVal = 1;
            }

            static void i_cli() /* Opcode 0xfa */
            {
                i86_ICount[0] -= 2;
                I.IF = 0;
            }

            static void i_sti() /* Opcode 0xfb */
            {
                i86_ICount[0] -= 2;
                I.IF = 1;
            }

            static void i_cld() /* Opcode 0xfc */
            {
                i86_ICount[0] -= 2;
                I.DF = 0;
            }

            static void i_std() /* Opcode 0xfd */
            {
                i86_ICount[0] -= 2;
                I.DF = 1;
            }

            static void i_fepre() /* Opcode 0xfe */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint tmp;
                if ((ModRM) >= 0xc0)
                    tmp = I.regs.b[(int)Mod_RM.RM.b[ModRM]];
                else
                {
                    i86_ICount[0] -= 6; tmp = (byte)cpu_readmem20((int)((GetEA[ModRM]()) & I.amask));
                }
                uint tmp1;

                i86_ICount[0] -= 3; /* 2 if dest is in memory */
                if ((ModRM & 0x38) == 0) /* INC eb */
                {
                    tmp1 = tmp + 1;
                    I.OverVal = (int)(((tmp1) ^ (tmp)) & ((tmp1) ^ (1)) & 0x80);
                }
                else /* DEC eb */
                {
                    tmp1 = tmp - 1;
                    I.OverVal = (int)(((tmp) ^ (1)) & ((tmp) ^ (tmp1)) & 0x80);
                }

                I.AuxVal = (int)(((tmp1) ^ ((tmp) ^ (1))) & 0x10);
                I.SignVal = I.ZeroVal = I.ParityVal = (sbyte)(tmp1);

                { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]] = (byte)tmp1; else { i86_ICount[0] -= 7; cpu_writemem20((int)((EA) & I.amask), (int)(byte)tmp1); }; };
            }


            static void i_ffpre() /* Opcode 0xff */
            {
                uint ModRM = ((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)));
                uint tmp;
                uint tmp1;

                switch (ModRM & 0x38)
                {
                    case 0x00: /* INC ew */
                        i86_ICount[0] -= 3; /* 2 if dest is in memory */
                        if ((ModRM) >= 0xc0)
                            tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        tmp1 = tmp + 1;

                        I.OverVal = (int)(((tmp1) ^ (tmp)) & ((tmp1) ^ (1)) & 0x8000);
                        I.AuxVal = (int)(((tmp1) ^ ((tmp) ^ (1))) & 0x10);
                        I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1);

                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)tmp1; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)((ushort)tmp1)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)(((ushort)tmp1) >> 8)); }; };
                        break;

                    case 0x08: /* DEC ew */
                        i86_ICount[0] -= 3; /* 2 if dest is in memory */
                        if ((ModRM) >= 0xc0)
                            tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        tmp1 = tmp - 1;

                        I.OverVal = (int)(((tmp) ^ (1)) & ((tmp) ^ (tmp1)) & 0x8000);
                        I.AuxVal = (int)(((tmp1) ^ ((tmp) ^ (1))) & 0x10);
                        I.SignVal = I.ZeroVal = I.ParityVal = (short)(tmp1);

                        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]] = (ushort)tmp1; else { i86_ICount[0] -= 11; cpu_writemem20((int)((EA) & I.amask), (byte)((ushort)tmp1)); cpu_writemem20((int)(((EA) + 1) & I.amask), (int)(((ushort)tmp1) >> 8)); }; };
                        break;

                    case 0x10: /* CALL ew */
                        i86_ICount[0] -= 9; /* 8 if dest is in memory */
                        if ((ModRM) >= 0xc0)
                            tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.ip)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.ip) >> 8)); }; };
                        I.ip = (ushort)tmp;
                        change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                        break;

                    case 0x18: /* CALL FAR ea */
                        i86_ICount[0] -= 11;
                        tmp = I.sregs[CS]; /* HJB 12/13/98 need to skip displacements of EA */
                        if ((ModRM) >= 0xc0)
                            tmp1 = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; tmp1 = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        I.sregs[CS] = (ushort)(cpu_readmem20((int)((EA + 2) & I.amask) + (cpu_readmem20((int)(((EA + 2) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                        I._base[CS] = (uint)(I.sregs[CS] << 4);
                        { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((tmp) >> 8)); }; };
                        { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(I.ip)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((I.ip) >> 8)); }; };
                        I.ip = (int)tmp1;
                        change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                        break;

                    case 0x20: /* JMP ea */
                        i86_ICount[0] -= 11; /* 8 if address in memory */
                        if ((ModRM) >= 0xc0)
                            I.ip = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; I.ip = (int)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                        break;

                    case 0x28: /* JMP FAR ea */
                        i86_ICount[0] -= 4;
                        if ((ModRM) >= 0xc0)
                            I.ip = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; I.ip = (int)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        I.sregs[CS] = (ushort)(cpu_readmem20((int)((EA + 2) & I.amask) + (cpu_readmem20((int)(((EA + 2) + 1) & I.amask) << 8)))); i86_ICount[0] -= 10;
                        I._base[CS] = (uint)(I.sregs[CS] << 4);
                        change_pc20((uint)((I._base[CS] + I.ip) & I.amask));
                        break;

                    case 0x30: /* PUSH ea */
                        i86_ICount[0] -= 3;
                        if ((ModRM) >= 0xc0)
                            tmp = I.regs.w[(int)Mod_RM.RM.w[ModRM]];
                        else
                        {
                            GetEA[ModRM](); i86_ICount[0] -= 10; tmp = (uint)(cpu_readmem20((int)((EA) & I.amask) + (cpu_readmem20((int)(((EA) + 1) & I.amask) << 8))));
                        }
                        { I.regs.w[SP] -= 2; { i86_ICount[0] -= 11; cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]) & I.amask)) & I.amask), (byte)(tmp)); cpu_writemem20((int)(((((I._base[SS] + I.regs.w[SP]) & I.amask)) + 1) & I.amask), (int)((tmp) >> 8)); }; };
                        break;
                }
            }

            static void trap()
            {
                instruction[((byte)cpu_readop((uint)((I._base[CS] + I.ip++) & I.amask)))]();
                i86_interrupt(1);
            }
            static void i_invalid()
            {
                /* makes the cpu loops forever until user resets it */
                /*	{ extern int debug_key_pressed; debug_key_pressed = 1; } */
                I.ip--;
                i86_ICount[0] -= 10;
            }
            #endregion
        }
    }
}