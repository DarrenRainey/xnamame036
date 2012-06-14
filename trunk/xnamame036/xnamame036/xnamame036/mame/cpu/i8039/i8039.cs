using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_i8039 : cpu_interface
        {
            public const int I8039_IGNORE_INT = 0;  /* Ignore interrupt                     */
            public const int I8039_EXT_INT = 1;	/* Execute a normal extern interrupt	*/
            public const int I8039_TIMER_INT = 2;/* Execute a Timer interrupt			*/
            public const int I8039_COUNT_INT = 4;	/* Execute a Counter interrupt			*/

            public const int I8039_p0 = 0x100;   /* Not used */
            public const int I8039_p1 = 0x101;
            public const int I8039_p2 = 0x102;
            public const int I8039_p4 = 0x104;
            public const int I8039_p5 = 0x105;
            public const int I8039_p6 = 0x106;
            public const int I8039_p7 = 0x107;
            public const int I8039_t0 = 0x110;
            public const int I8039_t1 = 0x111;
            public const int I8039_bus = 0x120;

            static int[] i8039_ICount = new int[1];
            static int[] i8035_ICount = i8039_ICount;

            public cpu_i8039()
            {
                cpu_num = CPU_I8039;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = I8039_IGNORE_INT;
                irq_int = I8039_EXT_INT;
                nmi_int = -1;
                address_bits = 16;
                address_shift = 0;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 2;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = i8039_ICount;
                Setup_OpCodes();
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
                    case CPU_INFO_NAME: return "I8039";
                    case CPU_INFO_FAMILY: return "Intel 8039";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "i8039.cs";
                    case CPU_INFO_CREDITS: return "Copyright (C) 1997 by Mirko Buffoni\nBased on the original work (C) 1997 by Dan Boris";
                }
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
            public override void exit()
            {
                //nothing
            }
            public override int execute(int cycles)
            {
                uint opcode, T1;
                int count;

                i8039_ICount[0] = cycles;

                do
                {
                    switch (R.pending_irq)
                    {
                        case I8039_COUNT_INT:
                        case I8039_TIMER_INT:
                            count = Timer_IRQ();
                            i8039_ICount[0] -= count;
                            if (R.timerON)  /* NS990113 */
                                R.masterClock += count;
                            R.t_flag = true;
                            break;
                        case I8039_EXT_INT:
                            if (R.irq_callback != null) R.irq_callback(0);
                            count = Ext_IRQ();
                            i8039_ICount[0] -= count;
                            if (R.timerON)  /* NS990113 */
                                R.masterClock += count;
                            break;
                    }
                    R.pending_irq = I8039_IGNORE_INT;

                    R.PREPC = R.PC;

                    opcode = cpu_readop(R.PC.wl);

                    /*      if (errorlog) fprintf(errorlog, "I8039:  PC = %04x,  opcode = %02x\n", R.PC.wl, opcode); */

                    R.PC.wl++;
                    i8039_ICount[0] -= (int)opcode_main[opcode].cycles;
                    opcode_main[opcode].function();

                    if (R.countON)  /* NS990113 */
                    {
                        T1 = test_r(1);
                        if ((((T1 - Old_T1) > 0) ? true : false))
                        {   /* Handle COUNTER IRQs */
                            R.timer++;
                            if (R.timer == 0) R.pending_irq = I8039_COUNT_INT;

                            Old_T1 = (byte)T1;
                        }
                    }

                    if (R.timerON)
                    {                        /* Handle TIMER IRQs */
                        R.masterClock += (int)opcode_main[opcode].cycles;
                        if (R.masterClock >= 32)
                        {  /* NS990113 */
                            R.masterClock -= 32;
                            R.timer++;
                            if (R.timer == 0) R.pending_irq = I8039_TIMER_INT;
                        }
                    }
                } while (i8039_ICount[0] > 0);

                return cycles - i8039_ICount[0];

            }
            static byte test_r(int a)
            {
                return (byte)cpu_readport(I8039_t0 + a);
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new I8039_Regs();
            }
            public override uint get_pc()
            {
                return R.PC.d;
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
                R.PC.wl = 0;
                R.SP = 0;
                R.A = 0;
                R.PSW = 0x08;		/* Start with Carry SET, Bit 4 is always SET */
                memset(R.RAM, 0x0, 128);
                R.bus = 0;
                R.irq_executing = I8039_IGNORE_INT;
                R.pending_irq = I8039_IGNORE_INT;

                R.A11ff = R.A11 = 0;
                R.timerON = R.countON = false;
                R.tirq_en = R.xirq_en = false;
                R.xirq_en = false;	/* NS990113 */
                R.timerON = true;	/* Mario Bros. doesn't work without this */
                R.masterClock = 0;

            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                R.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                R.irq_state = state;
                if (state == CLEAR_LINE)
                    R.pending_irq &= ~I8039_EXT_INT;
                else
                    R.pending_irq |= I8039_EXT_INT;

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
            public class I8039_Regs
            {
                public PAIR PREPC, PC;
                public byte A, SP, PSW;
                public byte[] RAM = new byte[256];
                public byte bus, f1;

                public int pending_irq, irq_executing, masterClock, regPtr;
                public bool t_flag, timerON, countON, xirq_en, tirq_en;
                public byte timer;
                public ushort A11, A11ff;
                public int irq_state;
                public irqcallback irq_callback;
            }
            static I8039_Regs R = new I8039_Regs();
            static byte Old_T1;
            delegate void opcode();
            class s_opcode
            {
                public uint cycles;
                public opcode function;
                public s_opcode(uint cycles, opcode function) { this.cycles = cycles; this.function = function; }
            }
            static s_opcode[] opcode_main = new s_opcode[256];
            int Timer_IRQ()
            {
                if (R.tirq_en && R.irq_executing == 0)
                {
                    //if (errorlog) fprintf(errorlog, "I8039:  TIMER INTERRUPT\n");
                    R.irq_executing = I8039_TIMER_INT;
                    push(R.PC.bl);
                    push((byte)((R.PC.bh & 0x0f) | (R.PSW & 0xf0)));
                    R.PC.wl = 0x07;
#if MESS
			change_pc(0x07);
#endif
                    R.A11ff = R.A11;
                    R.A11 = 0;
                    return 2;		/* 2 clock cycles used */
                }
                return 0;
            }

            int Ext_IRQ()
            {
                if (R.xirq_en)
                {
                    //if (errorlog) fprintf(errorlog, "I8039:  EXT INTERRUPT\n");
                    R.irq_executing = I8039_EXT_INT;
                    push(R.PC.bl);
                    push((byte)((R.PC.bh & 0x0f) | (R.PSW & 0xf0)));
                    R.PC.wl = 0x03;
                    R.A11ff = R.A11;
                    R.A11 = 0;
                    return 2;       /* 2 clock cycles used */
                }
                return 0;
            }
            static void push(byte d)
            {
                R.RAM[8 + R.SP++] = d;
                R.SP = (byte)(R.SP & 0x0f);
                R.PSW = (byte)(R.PSW & 0xf8);
                R.PSW = (byte)(R.PSW | (R.SP >> 1));
            }
            void Setup_OpCodes()
            {
                opcode_main[0] = new s_opcode(1, nop);
                opcode_main[1] = new s_opcode(0, illegal);
                opcode_main[2] = new s_opcode(2, outl_bus_a);
                opcode_main[3] = new s_opcode(2, add_a_n);
                opcode_main[4] = new s_opcode(2, jmp);
                opcode_main[5] = new s_opcode(1, en_i);
                opcode_main[6] = new s_opcode(0, illegal);
                opcode_main[7] = new s_opcode(1, dec_a);
                opcode_main[8] = new s_opcode(2, ins_a_bus);
                opcode_main[9] = new s_opcode(2, in_a_p1);
                opcode_main[10] = new s_opcode(2, in_a_p2);
                opcode_main[11] = new s_opcode(0, illegal);
                opcode_main[12] = new s_opcode(2, movd_a_p4);
                opcode_main[13] = new s_opcode(2, movd_a_p5);
                opcode_main[14] = new s_opcode(2, movd_a_p6);
                opcode_main[15] = new s_opcode(2, movd_a_p7);
                opcode_main[16] = new s_opcode(1, inc_xr0);
                opcode_main[17] = new s_opcode(1, inc_xr1);
                opcode_main[18] = new s_opcode(2, jb_0);
                opcode_main[19] = new s_opcode(2, adc_a_n);
                opcode_main[20] = new s_opcode(2, call);
                opcode_main[21] = new s_opcode(1, dis_i);
                opcode_main[22] = new s_opcode(2, jtf);
                opcode_main[23] = new s_opcode(1, inc_a);
                opcode_main[24] = new s_opcode(1, inc_r0);
                opcode_main[25] = new s_opcode(1, inc_r1);
                opcode_main[26] = new s_opcode(1, inc_r2);
                opcode_main[27] = new s_opcode(1, inc_r3);
                opcode_main[28] = new s_opcode(1, inc_r4);
                opcode_main[29] = new s_opcode(1, inc_r5);
                opcode_main[30] = new s_opcode(1, inc_r6);
                opcode_main[31] = new s_opcode(1, inc_r7);
                opcode_main[32] = new s_opcode(1, xch_a_xr0);
                opcode_main[33] = new s_opcode(1, xch_a_xr1);
                opcode_main[34] = new s_opcode(0, illegal);
                opcode_main[35] = new s_opcode(2, mov_a_n);
                opcode_main[36] = new s_opcode(2, jmp_1);
                opcode_main[37] = new s_opcode(1, en_tcnti);
                opcode_main[38] = new s_opcode(2, jnt_0);
                opcode_main[39] = new s_opcode(1, clr_a);
                opcode_main[40] = new s_opcode(1, xch_a_r0);
                opcode_main[41] = new s_opcode(1, xch_a_r1);
                opcode_main[42] = new s_opcode(1, xch_a_r2);
                opcode_main[43] = new s_opcode(1, xch_a_r3);
                opcode_main[44] = new s_opcode(1, xch_a_r4);
                opcode_main[45] = new s_opcode(1, xch_a_r5);
                opcode_main[46] = new s_opcode(1, xch_a_r6);
                opcode_main[47] = new s_opcode(1, xch_a_r7);
                opcode_main[48] = new s_opcode(1, xchd_a_xr0);
                opcode_main[49] = new s_opcode(1, xchd_a_xr1);
                opcode_main[50] = new s_opcode(2, jb_1);
                opcode_main[51] = new s_opcode(0, illegal);
                opcode_main[52] = new s_opcode(2, call_1);
                opcode_main[53] = new s_opcode(1, dis_tcnti);
                opcode_main[54] = new s_opcode(2, jt_0);
                opcode_main[55] = new s_opcode(1, cpl_a);
                opcode_main[56] = new s_opcode(0, illegal);
                opcode_main[57] = new s_opcode(2, outl_p1_a);
                opcode_main[58] = new s_opcode(2, outl_p2_a);
                opcode_main[59] = new s_opcode(0, illegal);
                opcode_main[60] = new s_opcode(2, movd_p4_a);
                opcode_main[61] = new s_opcode(2, movd_p5_a);
                opcode_main[62] = new s_opcode(2, movd_p6_a);
                opcode_main[63] = new s_opcode(2, movd_p7_a);
                opcode_main[64] = new s_opcode(1, orl_a_xr0);
                opcode_main[65] = new s_opcode(1, orl_a_xr1);
                opcode_main[66] = new s_opcode(1, mov_a_t);
                opcode_main[67] = new s_opcode(2, orl_a_n);
                opcode_main[68] = new s_opcode(2, jmp_2);
                opcode_main[69] = new s_opcode(1, strt_cnt);
                opcode_main[70] = new s_opcode(2, jnt_1);
                opcode_main[71] = new s_opcode(1, swap_a);
                opcode_main[72] = new s_opcode(1, orl_a_r0);
                opcode_main[73] = new s_opcode(1, orl_a_r1);
                opcode_main[74] = new s_opcode(1, orl_a_r2);
                opcode_main[75] = new s_opcode(1, orl_a_r3);
                opcode_main[76] = new s_opcode(1, orl_a_r4);
                opcode_main[77] = new s_opcode(1, orl_a_r5);
                opcode_main[78] = new s_opcode(1, orl_a_r6);
                opcode_main[79] = new s_opcode(1, orl_a_r7);
                opcode_main[80] = new s_opcode(1, anl_a_xr0);
                opcode_main[81] = new s_opcode(1, anl_a_xr1);
                opcode_main[82] = new s_opcode(2, jb_2);
                opcode_main[83] = new s_opcode(2, anl_a_n);
                opcode_main[84] = new s_opcode(2, call_2);
                opcode_main[85] = new s_opcode(1, strt_t);
                opcode_main[86] = new s_opcode(2, jt_1);
                opcode_main[87] = new s_opcode(1, daa_a);
                opcode_main[88] = new s_opcode(1, anl_a_r0);
                opcode_main[89] = new s_opcode(1, anl_a_r1);
                opcode_main[90] = new s_opcode(1, anl_a_r2);
                opcode_main[91] = new s_opcode(1, anl_a_r3);
                opcode_main[92] = new s_opcode(1, anl_a_r4);
                opcode_main[93] = new s_opcode(1, anl_a_r5);
                opcode_main[94] = new s_opcode(1, anl_a_r6);
                opcode_main[95] = new s_opcode(1, anl_a_r7);
                opcode_main[96] = new s_opcode(1, add_a_xr0);
                opcode_main[97] = new s_opcode(1, add_a_xr1);
                opcode_main[98] = new s_opcode(1, mov_t_a);
                opcode_main[99] = new s_opcode(0, illegal);
                opcode_main[100] = new s_opcode(2, jmp_3);
                opcode_main[101] = new s_opcode(1, stop_tcnt);
                opcode_main[102] = new s_opcode(0, illegal);
                opcode_main[103] = new s_opcode(1, rrc_a);
                opcode_main[104] = new s_opcode(1, add_a_r0);
                opcode_main[105] = new s_opcode(1, add_a_r1);
                opcode_main[106] = new s_opcode(1, add_a_r2);
                opcode_main[107] = new s_opcode(1, add_a_r3);
                opcode_main[108] = new s_opcode(1, add_a_r4);
                opcode_main[109] = new s_opcode(1, add_a_r5);
                opcode_main[110] = new s_opcode(1, add_a_r6);
                opcode_main[111] = new s_opcode(1, add_a_r7);
                opcode_main[112] = new s_opcode(1, adc_a_xr0);
                opcode_main[113] = new s_opcode(1, adc_a_xr1);
                opcode_main[114] = new s_opcode(2, jb_3);
                opcode_main[115] = new s_opcode(0, illegal);
                opcode_main[116] = new s_opcode(2, call_3);
                opcode_main[117] = new s_opcode(1, ento_clk);
                opcode_main[118] = new s_opcode(2, jf_1);
                opcode_main[119] = new s_opcode(1, rr_a);
                opcode_main[120] = new s_opcode(1, adc_a_r0);
                opcode_main[121] = new s_opcode(1, adc_a_r1);
                opcode_main[122] = new s_opcode(1, adc_a_r2);
                opcode_main[123] = new s_opcode(1, adc_a_r3);
                opcode_main[124] = new s_opcode(1, adc_a_r4);
                opcode_main[125] = new s_opcode(1, adc_a_r5);
                opcode_main[126] = new s_opcode(1, adc_a_r6);
                opcode_main[127] = new s_opcode(1, adc_a_r7);
                opcode_main[128] = new s_opcode(2, movx_a_xr0);
                opcode_main[129] = new s_opcode(2, movx_a_xr1);
                opcode_main[130] = new s_opcode(0, illegal);
                opcode_main[131] = new s_opcode(2, ret);
                opcode_main[132] = new s_opcode(2, jmp_4);
                opcode_main[133] = new s_opcode(1, clr_f0);
                opcode_main[134] = new s_opcode(2, jni);
                opcode_main[135] = new s_opcode(0, illegal);
                opcode_main[136] = new s_opcode(2, orl_bus_n);
                opcode_main[137] = new s_opcode(2, orl_p1_n);
                opcode_main[138] = new s_opcode(2, orl_p2_n);
                opcode_main[139] = new s_opcode(0, illegal);
                opcode_main[140] = new s_opcode(2, orld_p4_a);
                opcode_main[141] = new s_opcode(2, orld_p5_a);
                opcode_main[142] = new s_opcode(2, orld_p6_a);
                opcode_main[143] = new s_opcode(2, orld_p7_a);
                opcode_main[144] = new s_opcode(2, movx_xr0_a);
                opcode_main[145] = new s_opcode(2, movx_xr1_a);
                opcode_main[146] = new s_opcode(2, jb_4);
                opcode_main[147] = new s_opcode(2, retr);
                opcode_main[148] = new s_opcode(2, call_4);
                opcode_main[149] = new s_opcode(1, cpl_f0);
                opcode_main[150] = new s_opcode(2, jnz);
                opcode_main[151] = new s_opcode(1, clr_c);
                opcode_main[152] = new s_opcode(2, anl_bus_n);
                opcode_main[153] = new s_opcode(2, anl_p1_n);
                opcode_main[154] = new s_opcode(2, anl_p2_n);
                opcode_main[155] = new s_opcode(0, illegal);
                opcode_main[156] = new s_opcode(2, anld_p4_a);
                opcode_main[157] = new s_opcode(2, anld_p5_a);
                opcode_main[158] = new s_opcode(2, anld_p6_a);
                opcode_main[159] = new s_opcode(2, anld_p7_a);
                opcode_main[160] = new s_opcode(1, mov_xr0_a);
                opcode_main[161] = new s_opcode(1, mov_xr1_a);
                opcode_main[162] = new s_opcode(0, illegal);
                opcode_main[163] = new s_opcode(2, movp_a_xa);
                opcode_main[164] = new s_opcode(2, jmp_5);
                opcode_main[165] = new s_opcode(1, clr_f1);
                opcode_main[166] = new s_opcode(0, illegal);
                opcode_main[167] = new s_opcode(1, cpl_c);
                opcode_main[168] = new s_opcode(1, mov_r0_a);
                opcode_main[169] = new s_opcode(1, mov_r1_a);
                opcode_main[170] = new s_opcode(1, mov_r2_a);
                opcode_main[171] = new s_opcode(1, mov_r3_a);
                opcode_main[172] = new s_opcode(1, mov_r4_a);
                opcode_main[173] = new s_opcode(1, mov_r5_a);
                opcode_main[174] = new s_opcode(1, mov_r6_a);
                opcode_main[175] = new s_opcode(1, mov_r7_a);
                opcode_main[176] = new s_opcode(2, mov_xr0_n);
                opcode_main[177] = new s_opcode(2, mov_xr1_n);
                opcode_main[178] = new s_opcode(2, jb_5);
                opcode_main[179] = new s_opcode(2, jmpp_xa);
                opcode_main[180] = new s_opcode(2, call_5);
                opcode_main[181] = new s_opcode(1, cpl_f1);
                opcode_main[182] = new s_opcode(2, jf0);
                opcode_main[183] = new s_opcode(0, illegal);
                opcode_main[184] = new s_opcode(2, mov_r0_n);
                opcode_main[185] = new s_opcode(2, mov_r1_n);
                opcode_main[186] = new s_opcode(2, mov_r2_n);
                opcode_main[187] = new s_opcode(2, mov_r3_n);
                opcode_main[188] = new s_opcode(2, mov_r4_n);
                opcode_main[189] = new s_opcode(2, mov_r5_n);
                opcode_main[190] = new s_opcode(2, mov_r6_n);
                opcode_main[191] = new s_opcode(2, mov_r7_n);
                opcode_main[192] = new s_opcode(0, illegal);
                opcode_main[193] = new s_opcode(0, illegal);
                opcode_main[194] = new s_opcode(0, illegal);
                opcode_main[195] = new s_opcode(0, illegal);
                opcode_main[196] = new s_opcode(2, jmp_6);
                opcode_main[197] = new s_opcode(1, sel_rb0);
                opcode_main[198] = new s_opcode(2, jz);
                opcode_main[199] = new s_opcode(1, mov_a_psw);
                opcode_main[200] = new s_opcode(1, dec_r0);
                opcode_main[201] = new s_opcode(1, dec_r1);
                opcode_main[202] = new s_opcode(1, dec_r2);
                opcode_main[203] = new s_opcode(1, dec_r3);
                opcode_main[204] = new s_opcode(1, dec_r4);
                opcode_main[205] = new s_opcode(1, dec_r5);
                opcode_main[206] = new s_opcode(1, dec_r6);
                opcode_main[207] = new s_opcode(1, dec_r7);
                opcode_main[208] = new s_opcode(1, xrl_a_xr0);
                opcode_main[209] = new s_opcode(1, xrl_a_xr1);
                opcode_main[210] = new s_opcode(2, jb_6);
                opcode_main[211] = new s_opcode(2, xrl_a_n);
                opcode_main[212] = new s_opcode(2, call_6);
                opcode_main[213] = new s_opcode(1, sel_rb1);
                opcode_main[214] = new s_opcode(0, illegal);
                opcode_main[215] = new s_opcode(1, mov_psw_a);
                opcode_main[216] = new s_opcode(1, xrl_a_r0);
                opcode_main[217] = new s_opcode(1, xrl_a_r1);
                opcode_main[218] = new s_opcode(1, xrl_a_r2);
                opcode_main[219] = new s_opcode(1, xrl_a_r3);
                opcode_main[220] = new s_opcode(1, xrl_a_r4);
                opcode_main[221] = new s_opcode(1, xrl_a_r5);
                opcode_main[222] = new s_opcode(1, xrl_a_r6);
                opcode_main[223] = new s_opcode(1, xrl_a_r7);
                opcode_main[224] = new s_opcode(0, illegal);
                opcode_main[225] = new s_opcode(0, illegal);
                opcode_main[226] = new s_opcode(0, illegal);
                opcode_main[227] = new s_opcode(2, movp3_a_xa);
                opcode_main[228] = new s_opcode(2, jmp_7);
                opcode_main[229] = new s_opcode(1, sel_mb0);
                opcode_main[230] = new s_opcode(2, jnc);
                opcode_main[231] = new s_opcode(1, rl_a);
                opcode_main[232] = new s_opcode(2, djnz_r0);
                opcode_main[233] = new s_opcode(2, djnz_r1);
                opcode_main[234] = new s_opcode(2, djnz_r2);
                opcode_main[235] = new s_opcode(2, djnz_r3);
                opcode_main[236] = new s_opcode(2, djnz_r4);
                opcode_main[237] = new s_opcode(2, djnz_r5);
                opcode_main[238] = new s_opcode(2, djnz_r6);
                opcode_main[239] = new s_opcode(2, djnz_r7);
                opcode_main[240] = new s_opcode(1, mov_a_xr0);
                opcode_main[241] = new s_opcode(1, mov_a_xr1);
                opcode_main[242] = new s_opcode(2, jb_7);
                opcode_main[243] = new s_opcode(0, illegal);
                opcode_main[244] = new s_opcode(2, call_7);
                opcode_main[245] = new s_opcode(1, sel_mb1);
                opcode_main[246] = new s_opcode(2, jc);
                opcode_main[247] = new s_opcode(1, rlc_a);
                opcode_main[248] = new s_opcode(1, mov_a_r0);
                opcode_main[249] = new s_opcode(1, mov_a_r1);
                opcode_main[250] = new s_opcode(1, mov_a_r2);
                opcode_main[251] = new s_opcode(1, mov_a_r3);
                opcode_main[252] = new s_opcode(1, mov_a_r4);
                opcode_main[253] = new s_opcode(1, mov_a_r5);
                opcode_main[254] = new s_opcode(1, mov_a_r6);
                opcode_main[255] = new s_opcode(1, mov_a_r7);

           }
            static uint I8039_RDMEM_OPCODE()
            {
                uint retval;
                retval = cpu_readop_arg(R.PC.wl);
                R.PC.wl++;
                return retval;
            }
            static void M_ILLEGAL()
            {
                printf("I8039:  PC = %04x,  Illegal opcode = %02x\n", R.PC.wl - 1, cpu_readmem16(R.PC.wl - 1));
            }
            static void M_UNDEFINED()
            {
                printf("I8039:  PC = %04x,  Unimplemented opcode = %02x\n", R.PC.wl - 1, cpu_readmem16(R.PC.wl - 1));
            }
            static byte pull()
            {
                R.SP = (byte)((R.SP + 15) & 0x0f);		/*  if (--R.SP < 0) R.SP = 15;  */
                R.PSW = (byte)(R.PSW & 0xf8);
                R.PSW = (byte)(R.PSW | (R.SP >> 1));
                /* regPTR = ((M_By) ? 24 : 0);  regPTR should not change */
                return R.RAM[8 + R.SP];
            }
            static void M_CALL(ushort addr)
            {
                push(R.PC.bl);
                push((byte)((R.PC.bh & 0x0f) | (R.PSW & 0xf0)));
                R.PC.wl = addr;
#if MESS
		change_pc(addr);
#endif

            }
            static void daa_a()
            {
                if ((R.A & 0x0f) > 0x09 || (R.PSW & A_FLAG) != 0)
                    R.A += 0x06;
                if ((R.A & 0xf0) > 0x90 || (R.PSW & C_FLAG) != 0)
                {
                    R.A += 0x60;
                    SET(C_FLAG);
                }
                else CLR(C_FLAG);
            }
            const byte C_FLAG = 0x80, A_FLAG = 0x40, F_FLAG = 0x20, B_FLAG = 0x10;
            static void CLR(byte flag) { R.PSW &= (byte)~flag; }
            static void SET(byte flag) { R.PSW |= flag; }
            static byte bus_r() { return (byte)cpu_readport(I8039_bus); }
            static void bus_w(int v)
            {
                cpu_writeport(I8039_bus, v);
            }
            static void port_w(int a, int v) { cpu_writeport(I8039_p0 + a, v); }
            static byte port_r(int a) { return (byte)cpu_readport(I8039_p0 + a); }
            static void M_XCHD(byte addr)
            {
                byte dat = (byte)(R.A & 0x0f);
                R.A &= 0xf0;
                R.A |= (byte)(R.RAM[addr] & 0x0f);
                R.RAM[addr] &= 0xf0;
                R.RAM[addr] |= dat;
            }
            static void M_ADD(byte dat)
            {
                ushort temp;
                CLR(C_FLAG | A_FLAG);
                if ((R.A & 0xf) + (dat & 0xf) > 0xf) SET(A_FLAG);
                temp = (ushort)(R.A + dat);
                if (temp > 0xff) SET(C_FLAG);
                R.A = (byte)(temp & 0xff);
            }
            static void M_ADDC(byte dat)
            {
                ushort temp;

                CLR(A_FLAG);
                if ((R.A & 0xf) + (dat & 0xf) + ((R.PSW & C_FLAG) >> 7) > 0xf) SET(A_FLAG);
                temp = (ushort)(R.A + dat + ((R.PSW & C_FLAG) >> 7));
                CLR(C_FLAG);
                if (temp > 0xff) SET(C_FLAG);
                R.A = (byte)(temp & 0xff);
            }
            static void illegal() { M_ILLEGAL(); }

            static void add_a_n() { M_ADD((byte)I8039_RDMEM_OPCODE()); }
            static void add_a_r0() { M_ADD(R.RAM[R.regPtr]); }
            static void add_a_r1() { M_ADD(R.RAM[R.regPtr + 1]); }
            static void add_a_r2() { M_ADD(R.RAM[R.regPtr + 2]); }
            static void add_a_r3() { M_ADD(R.RAM[R.regPtr + 3]); }
            static void add_a_r4() { M_ADD(R.RAM[R.regPtr + 4]); }
            static void add_a_r5() { M_ADD(R.RAM[R.regPtr + 5]); }
            static void add_a_r6() { M_ADD(R.RAM[R.regPtr + 6]); }
            static void add_a_r7() { M_ADD(R.RAM[R.regPtr + 7]); }
            static void add_a_xr0() { M_ADD(R.RAM[R.RAM[R.regPtr] & 0x7f]); }
            static void add_a_xr1() { M_ADD(R.RAM[R.RAM[R.regPtr + 1] & 0x7f]); }
            static void adc_a_n() { M_ADDC((byte)I8039_RDMEM_OPCODE()); }
            static void adc_a_r0() { M_ADDC(R.RAM[R.regPtr]); }
            static void adc_a_r1() { M_ADDC(R.RAM[R.regPtr + 1]); }
            static void adc_a_r2() { M_ADDC(R.RAM[R.regPtr + 2]); }
            static void adc_a_r3() { M_ADDC(R.RAM[R.regPtr + 3]); }
            static void adc_a_r4() { M_ADDC(R.RAM[R.regPtr + 4]); }
            static void adc_a_r5() { M_ADDC(R.RAM[R.regPtr + 5]); }
            static void adc_a_r6() { M_ADDC(R.RAM[R.regPtr + 6]); }
            static void adc_a_r7() { M_ADDC(R.RAM[R.regPtr + 7]); }
            static void adc_a_xr0() { M_ADDC(R.RAM[R.RAM[R.regPtr] & 0x7f]); }
            static void adc_a_xr1() { M_ADDC(R.RAM[R.RAM[R.regPtr + 1] & 0x7f]); }
            static void anl_a_n() { R.A &= (byte)I8039_RDMEM_OPCODE(); }
            static void anl_a_r0() { R.A &= R.RAM[R.regPtr]; }
            static void anl_a_r1() { R.A &= R.RAM[R.regPtr + 1]; }
            static void anl_a_r2() { R.A &= R.RAM[R.regPtr + 2]; }
            static void anl_a_r3() { R.A &= R.RAM[R.regPtr + 3]; }
            static void anl_a_r4() { R.A &= R.RAM[R.regPtr + 4]; }
            static void anl_a_r5() { R.A &= R.RAM[R.regPtr + 5]; }
            static void anl_a_r6() { R.A &= R.RAM[R.regPtr + 6]; }
            static void anl_a_r7() { R.A &= R.RAM[R.regPtr + 7]; }
            static void anl_a_xr0() { R.A &= R.RAM[R.RAM[R.regPtr] & 0x7f]; }
            static void anl_a_xr1() { R.A &= R.RAM[R.RAM[R.regPtr + 1] & 0x7f]; }
            static void anl_bus_n() { bus_w((int)(bus_r() & I8039_RDMEM_OPCODE())); }
            static void anl_p1_n() { port_w(1, (int)(port_r(1) & I8039_RDMEM_OPCODE())); }
            static void anl_p2_n() { port_w(2, (int)(port_r(2) & I8039_RDMEM_OPCODE())); }
            static void anld_p4_a() { port_w(4, (int)(port_r(4) & I8039_RDMEM_OPCODE())); }
            static void anld_p5_a() { port_w(5, (int)(port_r(5) & I8039_RDMEM_OPCODE())); }
            static void anld_p6_a() { port_w(6, (int)(port_r(6) & I8039_RDMEM_OPCODE())); }
            static void anld_p7_a() { port_w(7, (int)(port_r(7) & I8039_RDMEM_OPCODE())); }
            static void call() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | R.A11)); }
            static void call_1() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x100 | R.A11)); }
            static void call_2() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x200 | R.A11)); }
            static void call_3() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x300 | R.A11)); }
            static void call_4() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x400 | R.A11)); }
            static void call_5() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x500 | R.A11)); }
            static void call_6() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x600 | R.A11)); }
            static void call_7() { byte i = (byte)I8039_RDMEM_OPCODE(); M_CALL((ushort)(i | 0x700 | R.A11)); }
            static void clr_a() { R.A = 0; }
            static void clr_c() { CLR(C_FLAG); }
            static void clr_f0() { CLR(F_FLAG); }
            static void clr_f1() { R.f1 = 0; }
            static void cpl_a() { R.A ^= 0xff; }
            static void cpl_c() { R.PSW ^= C_FLAG; }
            static void cpl_f0() { R.PSW ^= F_FLAG; }
            static void cpl_f1() { R.f1 ^= 1; }
            static void dec_a() { R.A--; }
            static void dec_r0() { R.RAM[R.regPtr]--; }
            static void dec_r1() { R.RAM[R.regPtr + 1]--; }
            static void dec_r2() { R.RAM[R.regPtr + 2]--; }
            static void dec_r3() { R.RAM[R.regPtr + 3]--; }
            static void dec_r4() { R.RAM[R.regPtr + 4]--; }
            static void dec_r5() { R.RAM[R.regPtr + 5]--; }
            static void dec_r6() { R.RAM[R.regPtr + 6]--; }
            static void dec_r7() { R.RAM[R.regPtr + 7]--; }
            static void dis_i() { R.xirq_en = false; }
            static void dis_tcnti() { R.tirq_en = false; }
#if MESS
	static void djnz_r0()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr]--; if (R.RAM[R.regPtr] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); }}
	static void djnz_r1()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+1]--; if (R.RAM[R.regPtr+1] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r2()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+2]--; if (R.RAM[R.regPtr+2] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r3()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+3]--; if (R.RAM[R.regPtr+3] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r4()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+4]--; if (R.RAM[R.regPtr+4] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r5()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+5]--; if (R.RAM[R.regPtr+5] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r6()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+6]--; if (R.RAM[R.regPtr+6] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void djnz_r7()	 { byte i=I8039_RDMEM_OPCODE(); R.RAM[R.regPtr+7]--; if (R.RAM[R.regPtr+7] != 0) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
#else
            static void djnz_r0() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr]--; if (R.RAM[R.regPtr] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r1() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 1]--; if (R.RAM[R.regPtr + 1] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r2() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 2]--; if (R.RAM[R.regPtr + 2] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r3() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 3]--; if (R.RAM[R.regPtr + 3] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r4() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 4]--; if (R.RAM[R.regPtr + 4] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r5() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 5]--; if (R.RAM[R.regPtr + 5] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r6() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 6]--; if (R.RAM[R.regPtr + 6] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void djnz_r7() { byte i = (byte)I8039_RDMEM_OPCODE(); R.RAM[R.regPtr + 7]--; if (R.RAM[R.regPtr + 7] != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
#endif
            static void en_i() { R.xirq_en = true; if (R.irq_state != CLEAR_LINE) R.pending_irq |= I8039_EXT_INT; }
            static void en_tcnti() { R.tirq_en = true; }
            static void ento_clk() { M_UNDEFINED(); }
            static void in_a_p1() { R.A = port_r(1); }
            static void in_a_p2() { R.A = port_r(2); }
            static void ins_a_bus() { R.A = bus_r(); }
            static void inc_a() { R.A++; }
            static void inc_r0() { R.RAM[R.regPtr]++; }
            static void inc_r1() { R.RAM[R.regPtr + 1]++; }
            static void inc_r2() { R.RAM[R.regPtr + 2]++; }
            static void inc_r3() { R.RAM[R.regPtr + 3]++; }
            static void inc_r4() { R.RAM[R.regPtr + 4]++; }
            static void inc_r5() { R.RAM[R.regPtr + 5]++; }
            static void inc_r6() { R.RAM[R.regPtr + 6]++; }
            static void inc_r7() { R.RAM[R.regPtr + 7]++; }
            static void inc_xr0() { R.RAM[R.RAM[R.regPtr] & 0x7f]++; }
            static void inc_xr1() { R.RAM[R.RAM[R.regPtr + 1] & 0x7f]++; }

            /* static void jmp()		{ byte i=cpu_readop(R.PC.wl); R.PC.wl = i | R.A11; }
             */

            static void jmp()
            {
                byte i = cpu_readop(R.PC.wl);
                ushort oldpc, newpc;

                oldpc = (ushort)(R.PC.wl - 1);
                R.PC.wl = (ushort)(i | R.A11);
#if MESS
	  change_pc(R.PC.wl);
#endif
                newpc = R.PC.wl;
                if (newpc == oldpc) { if (i8039_ICount[0] > 0) i8039_ICount[0] = 0; } /* speed up busy loop */
                else if (newpc == oldpc - 1 && cpu_readop(newpc) == 0x00)	/* NOP - Gyruss */
                { if (i8039_ICount[0] > 0) i8039_ICount[0] = 0; }
            }

#if MESS
	static void jmp_1() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x100 | R.A11; change_pc(R.PC.wl); }
	static void jmp_2() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x200 | R.A11; change_pc(R.PC.wl); }
	static void jmp_3() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x300 | R.A11; change_pc(R.PC.wl); }
	static void jmp_4() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x400 | R.A11; change_pc(R.PC.wl); }
	static void jmp_5() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x500 | R.A11; change_pc(R.PC.wl); }
	static void jmp_6() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x600 | R.A11; change_pc(R.PC.wl); }
	static void jmp_7() 	 { byte i=cpu_readop(R.PC.wl); R.PC.wl = i | 0x700 | R.A11; change_pc(R.PC.wl); }
	static void jmpp_xa()	 { UINT16 addr = (R.PC.wl & 0xf00) | R.A; R.PC.wl = (R.PC.wl & 0xf00) | cpu_readmem16(addr); change_pc(R.PC.wl); }
	static void jb_0()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x01) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_1()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x02) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_2()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x04) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_3()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x08) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_4()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x10) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_5()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x20) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_6()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x40) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jb_7()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A & 0x80) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jf0()		 { byte i=I8039_RDMEM_OPCODE(); if (M_F0y) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jf_1()		 { byte i=I8039_RDMEM_OPCODE(); if (R.f1)	{ R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jnc()		 { byte i=I8039_RDMEM_OPCODE(); if (M_Cn)	{ R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jc()		 { byte i=I8039_RDMEM_OPCODE(); if ( ((R.PSW & C_FLAG) >> 7))	{ R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jni()		 { byte i=I8039_RDMEM_OPCODE(); if (R.irq_state != CLEAR_LINE) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jnt_0() 	 { byte i=I8039_RDMEM_OPCODE(); if (!test_r(0)) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jt_0()		 { byte i=I8039_RDMEM_OPCODE(); if (test_r(0))  { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jnt_1() 	 { byte i=I8039_RDMEM_OPCODE(); if (!test_r(1)) { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jt_1()		 { byte i=I8039_RDMEM_OPCODE(); if (test_r(1))  { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jnz()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A != 0)	 { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jz()		 { byte i=I8039_RDMEM_OPCODE(); if (R.A == 0)	 { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); } }
	static void jtf()		 { byte i=I8039_RDMEM_OPCODE(); if (R.t_flag)	 { R.PC.wl = (R.PC.wl & 0xf00) | i; change_pc(R.PC.wl); R.t_flag = 0; } }
#else
            static void jmp_1() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x100 | R.A11); }
            static void jmp_2() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x200 | R.A11); }
            static void jmp_3() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x300 | R.A11); }
            static void jmp_4() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x400 | R.A11); }
            static void jmp_5() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x500 | R.A11); }
            static void jmp_6() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x600 | R.A11); }
            static void jmp_7() { byte i = cpu_readop(R.PC.wl); R.PC.wl = (ushort)(i | 0x700 | R.A11); }
            static void jmpp_xa() { ushort addr = (ushort)((R.PC.wl & 0xf00) | R.A); R.PC.wl = (ushort)((R.PC.wl & 0xf00) | cpu_readmem16(addr)); }
            static void jb_0() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x01) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_1() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x02) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_2() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x04) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_3() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x08) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_4() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x10) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_5() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x20) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_6() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x40) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jb_7() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((R.A & 0x80) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jf0() { byte i = (byte)I8039_RDMEM_OPCODE(); if (((R.PSW & F_FLAG)) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jf_1() { byte i = (byte)I8039_RDMEM_OPCODE(); if (R.f1 != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jnc() { byte i = (byte)I8039_RDMEM_OPCODE(); if ((((R.PSW & C_FLAG) >> 7)) == 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jc() { byte i = (byte)I8039_RDMEM_OPCODE(); if (((R.PSW & C_FLAG) >> 7) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jni() { byte i = (byte)I8039_RDMEM_OPCODE(); if (R.irq_state != CLEAR_LINE) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jnt_0() { byte i = (byte)I8039_RDMEM_OPCODE(); if (test_r(0) == 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jt_0() { byte i = (byte)I8039_RDMEM_OPCODE(); if (test_r(0) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jnt_1() { byte i = (byte)I8039_RDMEM_OPCODE(); if (test_r(1) == 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jt_1() { byte i = (byte)I8039_RDMEM_OPCODE(); if (test_r(1) != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jnz() { byte i = (byte)I8039_RDMEM_OPCODE(); if (R.A != 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jz() { byte i = (byte)I8039_RDMEM_OPCODE(); if (R.A == 0) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); } }
            static void jtf() { byte i = (byte)I8039_RDMEM_OPCODE(); if (R.t_flag) { R.PC.wl = (ushort)((R.PC.wl & 0xf00) | i); R.t_flag = false; } }
#endif

            static void mov_a_n() { R.A = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_a_r0() { R.A = R.RAM[R.regPtr]; }
            static void mov_a_r1() { R.A = R.RAM[R.regPtr + 1]; }
            static void mov_a_r2() { R.A = R.RAM[R.regPtr + 2]; }
            static void mov_a_r3() { R.A = R.RAM[R.regPtr + 3]; }
            static void mov_a_r4() { R.A = R.RAM[R.regPtr + 4]; }
            static void mov_a_r5() { R.A = R.RAM[R.regPtr + 5]; }
            static void mov_a_r6() { R.A = R.RAM[R.regPtr + 6]; }
            static void mov_a_r7() { R.A = R.RAM[R.regPtr + 7]; }
            static void mov_a_psw() { R.A = R.PSW; }
            static void mov_a_xr0() { R.A = R.RAM[R.RAM[R.regPtr] & 0x7f]; }
            static void mov_a_xr1() { R.A = R.RAM[R.RAM[R.regPtr + 1] & 0x7f]; }
            static void mov_r0_a() { R.RAM[R.regPtr] = R.A; }
            static void mov_r1_a() { R.RAM[R.regPtr + 1] = R.A; }
            static void mov_r2_a() { R.RAM[R.regPtr + 2] = R.A; }
            static void mov_r3_a() { R.RAM[R.regPtr + 3] = R.A; }
            static void mov_r4_a() { R.RAM[R.regPtr + 4] = R.A; }
            static void mov_r5_a() { R.RAM[R.regPtr + 5] = R.A; }
            static void mov_r6_a() { R.RAM[R.regPtr + 6] = R.A; }
            static void mov_r7_a() { R.RAM[R.regPtr + 7] = R.A; }
            static void mov_psw_a() { R.PSW = R.A; R.regPtr = ((((R.PSW & B_FLAG))) != 0 ? 24 : 0); R.SP = (byte)((R.PSW & 7) << 1); }
            static void mov_r0_n() { R.RAM[R.regPtr] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r1_n() { R.RAM[R.regPtr + 1] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r2_n() { R.RAM[R.regPtr + 2] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r3_n() { R.RAM[R.regPtr + 3] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r4_n() { R.RAM[R.regPtr + 4] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r5_n() { R.RAM[R.regPtr + 5] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r6_n() { R.RAM[R.regPtr + 6] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_r7_n() { R.RAM[R.regPtr + 7] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_a_t() { R.A = R.timer; }
            static void mov_t_a() { R.timer = R.A; }
            static void mov_xr0_a() { R.RAM[R.RAM[R.regPtr] & 0x7f] = R.A; }
            static void mov_xr1_a() { R.RAM[R.RAM[R.regPtr + 1] & 0x7f] = R.A; }
            static void mov_xr0_n() { R.RAM[R.RAM[R.regPtr] & 0x7f] = (byte)I8039_RDMEM_OPCODE(); }
            static void mov_xr1_n() { R.RAM[R.RAM[R.regPtr + 1] & 0x7f] = (byte)I8039_RDMEM_OPCODE(); }
            static void movd_a_p4() { R.A = port_r(4); }
            static void movd_a_p5() { R.A = port_r(5); }
            static void movd_a_p6() { R.A = port_r(6); }
            static void movd_a_p7() { R.A = port_r(7); }
            static void movd_p4_a() { port_w(4, R.A); }
            static void movd_p5_a() { port_w(5, R.A); }
            static void movd_p6_a() { port_w(6, R.A); }
            static void movd_p7_a() { port_w(7, R.A); }
            static void movp_a_xa() { R.A = (byte)cpu_readmem16((R.PC.wl & 0x0f00) | R.A); }
            static void movp3_a_xa() { R.A = (byte)cpu_readmem16(0x300 | R.A); }
            static void movx_a_xr0() { R.A = (byte)cpu_readport(R.RAM[R.regPtr]); }
            static void movx_a_xr1() { R.A = (byte)cpu_readport(R.RAM[R.regPtr + 1]); }
            static void movx_xr0_a() { cpu_writeport(R.RAM[R.regPtr], R.A); }
            static void movx_xr1_a() { cpu_writeport(R.RAM[R.regPtr + 1], R.A); }
            static void nop() { }
            static void orl_a_n() { R.A |= (byte)I8039_RDMEM_OPCODE(); }
            static void orl_a_r0() { R.A |= R.RAM[R.regPtr]; }
            static void orl_a_r1() { R.A |= R.RAM[R.regPtr + 1]; }
            static void orl_a_r2() { R.A |= R.RAM[R.regPtr + 2]; }
            static void orl_a_r3() { R.A |= R.RAM[R.regPtr + 3]; }
            static void orl_a_r4() { R.A |= R.RAM[R.regPtr + 4]; }
            static void orl_a_r5() { R.A |= R.RAM[R.regPtr + 5]; }
            static void orl_a_r6() { R.A |= R.RAM[R.regPtr + 6]; }
            static void orl_a_r7() { R.A |= R.RAM[R.regPtr + 7]; }
            static void orl_a_xr0() { R.A |= R.RAM[R.RAM[R.regPtr] & 0x7f]; }
            static void orl_a_xr1() { R.A |= R.RAM[R.RAM[R.regPtr + 1] & 0x7f]; }
            static void orl_bus_n() { bus_w((int)(bus_r() | I8039_RDMEM_OPCODE())); }
            static void orl_p1_n() { port_w(1, (int)(port_r(1) | I8039_RDMEM_OPCODE())); }
            static void orl_p2_n() { port_w(2, (int)(port_r(2) | I8039_RDMEM_OPCODE())); }
            static void orld_p4_a() { port_w(4, port_r(4) | R.A); }
            static void orld_p5_a() { port_w(5, port_r(5) | R.A); }
            static void orld_p6_a() { port_w(6, port_r(6) | R.A); }
            static void orld_p7_a() { port_w(7, port_r(7) | R.A); }
            static void outl_bus_a() { bus_w(R.A); }
            static void outl_p1_a() { port_w(1, R.A); }
            static void outl_p2_a() { port_w(2, R.A); }
#if MESS
	static void ret()		 { R.PC.wl = ((pull() & 0x0f) << 8); R.PC.wl |= pull(); change_pc(R.PC.wl); }
#else
            static void ret() { R.PC.wl = (ushort)((pull() & 0x0f) << 8); R.PC.wl |= pull(); }
#endif

            static void retr()
            {
                byte i = pull();
                R.PC.wl = (ushort)(((i & 0x0f) << 8) | pull());
#if MESS
		change_pc(R.PC.wl);
#endif
                R.irq_executing = I8039_IGNORE_INT;
                //	R.A11 = R.A11ff;	/* NS990113 */
                R.PSW = (byte)((R.PSW & 0x0f) | (i & 0xf0));   /* Stack is already changed by pull */
                R.regPtr = ((((R.PSW & B_FLAG))) != 0 ? 24 : 0);
            }
            static void rl_a() { byte i = (byte)(R.A & 0x80); R.A <<= 1; if (i != 0) R.A |= 0x01; else R.A &= 0xfe; }
            /* NS990113 */
            static void rlc_a() { byte i = (byte)((R.PSW & C_FLAG) >> 7); if ((R.A & 0x80) != 0) SET(C_FLAG); else CLR(C_FLAG); R.A <<= 1; if (i != 0) R.A |= 0x01; else R.A &= 0xfe; }
            static void rr_a() { byte i = (byte)(R.A & 1); R.A >>= 1; if (i != 0) R.A |= 0x80; else R.A &= 0x7f; }
            /* NS990113 */
            static void rrc_a() { byte i = (byte)((R.PSW & C_FLAG) >> 7); if ((R.A & 1) != 0) SET(C_FLAG); else CLR(C_FLAG); R.A >>= 1; if (i != 0) R.A |= 0x80; else R.A &= 0x7f; }
            static void sel_mb0() { R.A11 = 0; R.A11ff = 0; }
            static void sel_mb1() { R.A11ff = 0x800; if (R.irq_executing == I8039_IGNORE_INT) R.A11 = 0x800; }
            static void sel_rb0() { CLR(B_FLAG); R.regPtr = 0; }
            static void sel_rb1() { SET(B_FLAG); R.regPtr = 24; }
            static void stop_tcnt() { R.timerON = R.countON = false; }
            static void strt_cnt() { R.countON = true; Old_T1 = test_r(1); }	/* NS990113 */
            static void strt_t() { R.timerON = true; R.masterClock = 0; }	/* NS990113 */
            static void swap_a() { byte i = (byte)(R.A >> 4); R.A <<= 4; R.A |= i; }
            static void xch_a_r0() { byte i = R.A; R.A = R.RAM[R.regPtr]; R.RAM[R.regPtr] = i; }
            static void xch_a_r1() { byte i = R.A; R.A = R.RAM[R.regPtr + 1]; R.RAM[R.regPtr + 1] = i; }
            static void xch_a_r2() { byte i = R.A; R.A = R.RAM[R.regPtr + 2]; R.RAM[R.regPtr + 2] = i; }
            static void xch_a_r3() { byte i = R.A; R.A = R.RAM[R.regPtr + 3]; R.RAM[R.regPtr + 3] = i; }
            static void xch_a_r4() { byte i = R.A; R.A = R.RAM[R.regPtr + 4]; R.RAM[R.regPtr + 4] = i; }
            static void xch_a_r5() { byte i = R.A; R.A = R.RAM[R.regPtr + 5]; R.RAM[R.regPtr + 5] = i; }
            static void xch_a_r6() { byte i = R.A; R.A = R.RAM[R.regPtr + 6]; R.RAM[R.regPtr + 6] = i; }
            static void xch_a_r7() { byte i = R.A; R.A = R.RAM[R.regPtr + 7]; R.RAM[R.regPtr + 7] = i; }
            static void xch_a_xr0() { byte i = R.A; R.A = R.RAM[R.RAM[R.regPtr] & 0x7f]; R.RAM[R.RAM[R.regPtr] & 0x7f] = i; }
            static void xch_a_xr1() { byte i = R.A; R.A = R.RAM[R.RAM[R.regPtr + 1] & 0x7f]; R.RAM[R.RAM[R.regPtr + 1] & 0x7f] = i; }
            static void xchd_a_xr0() { M_XCHD((byte)(R.RAM[R.regPtr] & 0x7f)); }
            static void xchd_a_xr1() { M_XCHD((byte)(R.RAM[R.regPtr + 1] & 0x7f)); }
            static void xrl_a_n() { R.A ^= (byte)I8039_RDMEM_OPCODE(); }
            static void xrl_a_r0() { R.A ^= R.RAM[R.regPtr]; }
            static void xrl_a_r1() { R.A ^= R.RAM[R.regPtr + 1]; }
            static void xrl_a_r2() { R.A ^= R.RAM[R.regPtr + 2]; }
            static void xrl_a_r3() { R.A ^= R.RAM[R.regPtr + 3]; }
            static void xrl_a_r4() { R.A ^= R.RAM[R.regPtr + 4]; }
            static void xrl_a_r5() { R.A ^= R.RAM[R.regPtr + 5]; }
            static void xrl_a_r6() { R.A ^= R.RAM[R.regPtr + 6]; }
            static void xrl_a_r7() { R.A ^= R.RAM[R.regPtr + 7]; }
            static void xrl_a_xr0() { R.A ^= R.RAM[R.RAM[R.regPtr] & 0x7f]; }
            static void xrl_a_xr1() { R.A ^= R.RAM[R.RAM[R.regPtr + 1] & 0x7f]; }
        }
        public class cpu_i8035:cpu_i8039
        {
            public cpu_i8035()
                : base()
            {
                cpu_num = CPU_I8035;
               
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "I8035";
                    case CPU_INFO_VERSION: return "1.1";
                }
                return base.cpu_info(context, regnum);
            }
        }
    }
}