using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_v30 : cpu_nec
        {
            public cpu_v30()
            {
                cpu_num = CPU_V30;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = NEC_INT_NONE;
                irq_int = -1000;
                nmi_int = NEC_NMI_INT;
                address_shift = 0;
                address_bits = 20;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 5;
                abits1 = ABITS1_20;
                abits2 = ABITS2_20;
                abitsmin = ABITS_MIN_20;
                icount = nec_ICount;
            }
            public override void burn(int cycles)
            {
               
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                return v30_info(context, regnum);
            }
            public override void cpu_state_load(object file)
            {
                throw new NotImplementedException();
            }
            public override void cpu_state_save(object file)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new nec_Regs();
            }
            public override uint get_context(ref object reg)
            {
                return v30_get_context(ref reg);
            }
            public override void set_context(object reg)
            {
                v30_set_context(reg);
            }
            public override int execute(int cycles)
            {
                return v30_execute(cycles);
            }
            public override uint get_pc()
            {
                return v30_get_pc();
            }
            public override void set_pc(uint val)
            {
                throw new NotImplementedException();
            }
            public override uint get_reg(int regnum)
            {
                throw new NotImplementedException();
            }
            public override void set_reg(int regnum, uint val)
            {
                throw new NotImplementedException();
            }
            public override uint get_sp()
            {
                throw new NotImplementedException();
            }
            public override void set_sp(uint val)
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
                v30_reset(param);
            }
            public override void set_irq_callback(irqcallback callback)
            {
                v30_set_irq_callback(callback);
            }
            public override void set_irq_line(int irqline, int linestate)
            {
                v30_set_irq_line(irqline, linestate);
            }
            public override void set_nmi_line(int linestate)
            {
                v30_set_nmi_line(linestate);
            }
            public override void set_op_base(int pc)
            {
                cpu_setOPbase20(pc, 0);
            }
        }

        public partial class cpu_nec : cpu_interface
        {
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
                throw new NotImplementedException();
            }
            public override void cpu_state_load(object file)
            {
                throw new NotImplementedException();
            }
            public override void cpu_state_save(object file)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override int execute(int cycles)
            {
                throw new NotImplementedException();
            }
            public override uint get_pc()
            {
                throw new NotImplementedException();
            }
            public override void set_pc(uint val)
            {
                throw new NotImplementedException();
            }
            public override uint get_reg(int regnum)
            {
                throw new NotImplementedException();
            }
            public override void set_reg(int regnum, uint val)
            {
                throw new NotImplementedException();
            }
            public override uint get_sp()
            {
                throw new NotImplementedException();
            }
            public override void set_sp(uint val)
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
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            public override void exit()
            {
                throw new NotImplementedException();
            }

            void nec_reset(object param)
            {
                uint i, j, c;
                BREGS[] reg_name = { (BREGS)AL, (BREGS)CL, (BREGS)DL, (BREGS)BL, (BREGS)AH, (BREGS)CH, (BREGS)DH, (BREGS)BH };

                I = new nec_Regs();

                I.sregs[CS] = 0xffff;
                I._base[CS] = (uint)(I.sregs[CS] << 4);

                change_pc20((uint)(I._base[CS] + I.ip));

                for (i = 0; i < 256; i++)
                {
                    for (j = i, c = 0; j > 0; j >>= 1)
                        if ((j & 1) != 0) c++;
                    parity_table[i] = (byte)((c & 1) == 0 ? 1 : 0);
                }

                I.ZeroVal = I.ParityVal = 1;
                I.MF = 1;						/* set the mode-flag = native mode */

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

            void nec_exit()
            {
                /* nothing to do ? */
            }

            /* OB[19.07.99] added Mode Flag V30 */
            const byte ES = 0, CS = 1, SS = 2, DS = 3;
            const byte AW = 0, CW = 1, DW = 2, BW = 3, SP = 4, BP = 5, IX = 6, IY = 8;
            enum SREGS { ES, CS, SS, DS } ;
            enum WREGS { AW, CW, DW, BW, SP, BP, IX, IY } ;


#if WINDOWS
            const byte AL = 0, AH = 1, CL = 2, CH = 3, DL = 4, DH = 5, BL = 6, BH = 7, SPL = 8, SPH = 9, BPL = 10, BPH = 11, IXL = 12, IXH = 13, IYL = 14, IYH = 15;
            enum BREGS { AL, AH, CL, CH, DL, DH, BL, BH, SPL, SPH, BPL, BPH, IXL, IXH, IYL, IYH } ;
#else
enum BREGS{ AH,AL,CH,CL,DH,DL,BH,BL,SPH,SPL,BPH,BPL,IXH,IXL,IYH,IYL } BREGS;
#endif

            const byte
             NEC_IP = 1, NEC_AW = 2, NEC_CW = 3, NEC_DW = 4, NEC_BW = 5, NEC_SP = 6, NEC_BP = 7, NEC_IX = 8, NEC_IY = 9,
             NEC_FLAGS = 10, NEC_ES = 11, NEC_CS = 12, NEC_SS = 13, NEC_DS = 14,
             NEC_VECTOR = 15, NEC_PENDING = 16, NEC_NMI_STATE = 17, NEC_IRQ_STATE = 18;

            public const byte NEC_INT_NONE = 0;
            public const byte NEC_NMI_INT = 2;


            static ushort[] bytes = {
 1,2,4,8,16,32,64,128,256,
 512,1024,2048,4096,8192,16384,32768,65336
};

            /* NEC registers */
            public class necbasicregs
            {
                public byte[] b = new byte[18];
                public class _w
                {
                    byte[] b;
                    public _w(byte[] b) { this.b = b; }
                    public ushort this[int index]
                    {
                        get
                        {
                            return BitConverter.ToUInt16(b, index);
                            //return (ushort)(b[1 + index * 2] << 8 | b[index * 2]);
                        }
                        set
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, index * 2, 2);
                            //b[index * 2] = (byte)value;
                            //b[index * 2 + 1] = (byte)(value >> 8);
                        }
                    }
                }
                public _w w;
                public necbasicregs() { w = new _w(b); }
            }

            public class nec_Regs
            {
                public necbasicregs regs = new necbasicregs();
                public int ip;
                public ushort flags;
                public uint[] _base = new uint[4];
                public ushort[] sregs = new ushort[4];
                public irqcallback irq_callback;
                public int AuxVal, OverVal, SignVal, ZeroVal, CarryVal, ParityVal; /* 0 or non-0 valued flags */
                public byte TF, IF, DF, MF; /* 0 or 1 valued flags */ /* OB[19.07.99] added Mode Flag V30 */
                public byte int_vector;
                public byte pending_irq;
                public sbyte nmi_state;
                public sbyte irq_state;

                public uint prefix_base; /* base address of the latest prefix segment */
                public byte seg_prefix; /* prefix segment indicator */
            } ;
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

            static void nec_interrupt(uint int_num, bool md_flag)
            {
                uint dest_seg, dest_off;

                i_pushf();
                I.TF = I.IF = 0;
                if (md_flag) I.MF = (byte)(0); /* clear Mode-flag = start 8080 emulation mode */

                if (int_num == unchecked((uint)-1))
                {
                    int_num = (uint)I.irq_callback(0);
                    //		if (errorlog) fprintf(errorlog," (indirect ->%02d) ",int_num);
                }

                dest_off = (uint)(cpu_readmem20((int)(int_num * 4)) + (cpu_readmem20((int)((int_num * 4) + 1))) << 8); nec_ICount[0] -= 10;
                dest_seg = (uint)(cpu_readmem20((int)(int_num * 4 + 2)) + (cpu_readmem20((int)((int_num * 4 + 2) + 1))) << 8); nec_ICount[0] -= 10;

                { I.regs.w[SP] -= 2; { nec_ICount[0] -= 11; cpu_writemem20((int)(((I._base[SS] + I.regs.w[SP]))), (byte)(I.sregs[CS])); cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]))) + 1), (int)(I.sregs[CS]) >> 8); }; };
                { I.regs.w[SP] -= 2; { nec_ICount[0] -= 11; cpu_writemem20((int)(((I._base[SS] + I.regs.w[SP]))), (byte)(I.ip)); cpu_writemem20((int)((((I._base[SS] + I.regs.w[SP]))) + 1), (int)(I.ip) >> 8); }; };
                I.ip = (ushort)dest_off;
                I.sregs[CS] = (ushort)dest_seg;
                I._base[CS] = (uint)(I.sregs[CS] << 4);
                change_pc20((uint)(I._base[CS] + I.ip));
                //	if (errorlog)
                //		fprintf(errorlog,"=%06x\n",cpu_get_pc());
            }
            static void external_int()
            {
                if ((I.pending_irq & 0x02) != 0)
                {
                    nec_interrupt(2, false);
                    I.pending_irq &= unchecked((byte)~0x02);
                }
                else
                    if (I.pending_irq != 0)
                    {
                        /* the actual vector is retrieved after pushing flags */
                        /* and clearing the IF */
                        nec_interrupt(unchecked((uint)-1), false);
                    }
            }
            static void nec_trap()
            {
                nec_instruction[((byte)cpu_readop((uint)(I._base[CS] + I.ip++)))]();
                nec_interrupt(1, false);
            }

            public static int[] nec_ICount = new int[1];

            static nec_Regs I;

            /* The interrupt number of a pending external interrupt pending NMI is 2.	*/
            /* For INTR interrupts, the level is caught on the bus during an INTA cycle */

            const byte INT_IRQ = 0x01;
            const byte NMI_IRQ = 0x02;

            static byte[] parity_table = new byte[256];

            delegate void opcode();
            static opcode[] nec_instruction =
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
//    i_invalid,
 i_pre_nec /* 0x0f */,
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
    i_invalid, /* 0x63 */
    i_repnc, /* 0x64 */
    i_repc, /* 0x65 */
    i_invalid, /* 0x66 */
    i_invalid, /* 0x67 */
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
    i_setalc,
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
    i_di, /* 0xfa */
    i_ei, /* 0xfb */
    i_cld, /* 0xfc */
    i_std, /* 0xfd */
    i_fepre, /* 0xfe */
    i_ffpre /* 0xff */
};
        }
    }
}
