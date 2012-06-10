using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_konami : cpu_interface
        {
            public const byte KONAMI_INT_NONE = 0;
            public const byte KONAMI_INT_IRQ = 1;
            public const byte KONAMI_INT_FIRQ = 2;
            public const byte KONAMI_INT_NMI = 4;
            public const byte KONAMI_IRQ_LINE = 0;
            public const byte KONAMI_FIRQ_LINE = 1;
            static int[] konami_ICount = new int[1];

            public cpu_konami()
            {
                cpu_num = CPU_KONAMI;
                num_irqs = 2;
                default_vector = 0;
                overclock = 1.0;
                no_int = KONAMI_INT_NONE;
                irq_int = KONAMI_INT_IRQ;
                nmi_int = KONAMI_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;

                icount = konami_ICount;
                konami_ICount[0] = 50000;
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
                    case CPU_INFO_NAME: return "KONAMI";
                    case CPU_INFO_FAMILY: return "KONAMI 5000x";
                    case CPU_INFO_VERSION: return "1.0";
                    case CPU_INFO_FILE: return "konami.cs";
                    case CPU_INFO_CREDITS: return "Copyright (C) The MAME Team 1999";
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
                konami_ICount[0] = cycles - konami.extra_cycles;
                konami.extra_cycles = 0;

                if ((konami.int_state & (8 | 16)) != 0)
                {
                    konami_ICount[0] = 0;
                }
                else
                {
                    do
                    {
                        konami.ppc = konami.pc;

                        konami.ireg = (byte)((uint)cpu_readop(konami.pc.d));
                        konami.pc.wl++;

                        konami_main[konami.ireg]();

                        konami_ICount[0] -= cycles1[konami.ireg];

                    } while (konami_ICount[0] > 0);

                    konami_ICount[0] -= konami.extra_cycles;
                    konami.extra_cycles = 0;
                }

                return cycles - konami_ICount[0];
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new konami_Regs();
            }
            public override uint get_pc()
            {
                return konami.pc.wl;
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
                konami.int_state = 0;
                konami.nmi_state = CLEAR_LINE;
                konami.irq_state[0] = CLEAR_LINE;
                konami.irq_state[0] = CLEAR_LINE;

                konami.dp.d = 0; /* Reset direct page register */

                konami.cc |= 0x10; /* IRQ disabled */
                konami.cc |= 0x40; /* FIRQ disabled */

                konami.pc.d = RM16(0xfffe);
                change_pc(konami.pc.wl); /* TS 971002 */
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                konami.irq_callback = callback;
            }
            void PUSHBYTE(byte b)
            {
                --konami.s.wl;
                cpu_writemem16((int)konami.s.d, b);
            }
            void PUSHWORD(PAIR w)
            {
                --konami.s.wl;
                cpu_writemem16((int)konami.s.d, w.bl);
                --konami.s.wl;
                cpu_writemem16((int)konami.s.d, w.bh);
            }
            public override void set_irq_line(int irqline, int state)
            {
                konami.irq_state[irqline] = (byte)state;
                if (state == CLEAR_LINE) return;
                if (konami.irq_state[KONAMI_IRQ_LINE] != CLEAR_LINE ||
         konami.irq_state[KONAMI_FIRQ_LINE] != CLEAR_LINE)
                    konami.int_state &= unchecked((byte)~16); /* clear SYNC flag */
                if (konami.irq_state[KONAMI_FIRQ_LINE] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    /* fast IRQ */
                    /* state already saved by CWAI? */
                    if ((konami.int_state & 8) != 0)
                    {
                        konami.int_state &= unchecked((byte)~8);  /* clear CWAI */
                        konami.extra_cycles += 7;		 /* subtract +7 cycles */
                    }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80);				/* save 'short' state */
                        PUSHWORD(konami.ppc);
                        PUSHBYTE(konami.cc);
                        konami.extra_cycles += 10;	/* subtract +10 cycles */
                    }
                    konami.cc |= 0x40 | 0x10;			/* inhibit FIRQ and IRQ */
                    konami.pc.d = RM16(0xfff6);
                    change_pc(konami.pc.wl);					/* TS 971002 */
                    konami.irq_callback(KONAMI_FIRQ_LINE);
                }
                else
                    if (konami.irq_state[KONAMI_IRQ_LINE] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                    {
                        /* standard IRQ */
                        /* state already saved by CWAI? */
                        if ((konami.int_state & 8) != 0)
                        {
                            konami.int_state &= unchecked((byte)~8);  /* clear CWAI flag */
                            konami.extra_cycles += 7;		 /* subtract +7 cycles */
                        }
                        else
                        {
                            konami.cc |= 0x80; 				/* save entire state */
                            PUSHWORD(konami.ppc);
                            PUSHWORD(konami.u);
                            PUSHWORD(konami.y);
                            PUSHWORD(konami.x);
                            PUSHBYTE(konami.dp.bh);
                            PUSHBYTE(konami.d.bl);
                            PUSHBYTE(konami.d.bh);
                            PUSHBYTE(konami.cc);
                            konami.extra_cycles += 19;	 /* subtract +19 cycles */
                        }
                        konami.cc |= 0x10;					/* inhibit IRQ */
                        konami.pc.d = RM16(0xfff8);
                        change_pc(konami.pc.wl);					/* TS 971002 */
                        konami.irq_callback(KONAMI_IRQ_LINE);
                    }
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
            static byte[] cycles1 =
{
 /*	 0	1  2  3  4	5  6  7  8	9  A  B  C	D  E  F */
  /*0*/ 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 4, 4, 5, 5, 5, 5,
  /*1*/ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
  /*2*/ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
  /*3*/ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 7, 6,
  /*4*/ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4,
  /*5*/ 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 1, 1, 1,
  /*6*/ 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 5, 5,
  /*7*/ 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 5, 5,
  /*8*/ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5,
  /*9*/ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6,
  /*A*/ 2, 2, 2, 4, 4, 4, 4, 4, 2, 2, 2, 2, 3, 3, 2, 1,
  /*B*/ 3, 2, 2,11,22,11, 2, 4, 3, 3, 3, 3, 3, 3, 3, 3,
  /*C*/ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 3, 2,
  /*D*/ 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
  /*E*/ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
  /*F*/ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
};
            delegate void opcode();
            static opcode[] konami_main = {
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 00 */
 opcode2,opcode2,opcode2,opcode2,pshs ,pshu ,puls ,pulu ,
 lda_im ,ldb_im ,opcode2,opcode2,adda_im,addb_im,opcode2,opcode2, /* 10 */
 adca_im,adcb_im,opcode2,opcode2,suba_im,subb_im,opcode2,opcode2,
 sbca_im,sbcb_im,opcode2,opcode2,anda_im,andb_im,opcode2,opcode2, /* 20 */
 bita_im,bitb_im,opcode2,opcode2,eora_im,eorb_im,opcode2,opcode2,
 ora_im ,orb_im ,opcode2,opcode2,cmpa_im,cmpb_im,opcode2,opcode2, /* 30 */
 setline_im,opcode2,opcode2,opcode2,andcc,orcc ,exg ,tfr ,
 ldd_im ,opcode2,ldx_im ,opcode2,ldy_im ,opcode2,ldu_im ,opcode2, /* 40 */
 lds_im ,opcode2,cmpd_im,opcode2,cmpx_im,opcode2,cmpy_im,opcode2,
 cmpu_im,opcode2,cmps_im,opcode2,addd_im,opcode2,subd_im,opcode2, /* 50 */
 opcode2,opcode2,opcode2,opcode2,opcode2,illegal,illegal,illegal,
 bra ,bhi ,bcc ,bne ,bvc ,bpl ,bge ,bgt , /* 60 */
 lbra ,lbhi ,lbcc ,lbne ,lbvc ,lbpl ,lbge ,lbgt ,
 brn ,bls ,bcs ,beq ,bvs ,bmi ,blt ,ble , /* 70 */
 lbrn ,lbls ,lbcs ,lbeq ,lbvs ,lbmi ,lblt ,lble ,
 clra ,clrb ,opcode2,coma ,comb ,opcode2,nega ,negb , /* 80 */
 opcode2,inca ,incb ,opcode2,deca ,decb ,opcode2,rts ,
 tsta ,tstb ,opcode2,lsra ,lsrb ,opcode2,rora ,rorb , /* 90 */
 opcode2,asra ,asrb ,opcode2,asla ,aslb ,opcode2,rti ,
 rola ,rolb ,opcode2,opcode2,opcode2,opcode2,opcode2,opcode2, /* a0 */
 opcode2,opcode2,bsr ,lbsr ,decbjnz,decxjnz,nop ,illegal,
 abx ,daa ,sex ,mul ,lmul ,divx ,bmove ,move , /* b0 */
 lsrd ,opcode2,rord ,opcode2,asrd ,opcode2,asld ,opcode2,
 rold ,opcode2,clrd ,opcode2,negd ,opcode2,incd ,opcode2, /* c0 */
 decd ,opcode2,tstd ,opcode2,absa ,absb ,absd ,bset ,
 bset2 ,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* d0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* e0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* f0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
};

            static opcode[] konami_indexed = {
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 00 */
 leax ,leay ,leau ,leas ,illegal,illegal,illegal,illegal,
 illegal,illegal,lda_ix ,ldb_ix ,illegal,illegal,adda_ix,addb_ix, /* 10 */
 illegal,illegal,adca_ix,adcb_ix,illegal,illegal,suba_ix,subb_ix,
 illegal,illegal,sbca_ix,sbcb_ix,illegal,illegal,anda_ix,andb_ix, /* 20 */
 illegal,illegal,bita_ix,bitb_ix,illegal,illegal,eora_ix,eorb_ix,
 illegal,illegal,ora_ix ,orb_ix ,illegal,illegal,cmpa_ix,cmpb_ix, /* 30 */
 illegal,setline_ix,sta_ix,stb_ix,illegal,illegal,illegal,illegal,
 illegal,ldd_ix ,illegal,ldx_ix ,illegal,ldy_ix ,illegal,ldu_ix , /* 40 */
 illegal,lds_ix ,illegal,cmpd_ix,illegal,cmpx_ix,illegal,cmpy_ix,
 illegal,cmpu_ix,illegal,cmps_ix,illegal,addd_ix,illegal,subd_ix, /* 50 */
 std_ix ,stx_ix ,sty_ix ,stu_ix ,sts_ix ,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 60 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 70 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,clr_ix ,illegal,illegal,com_ix ,illegal,illegal, /* 80 */
 neg_ix ,illegal,illegal,inc_ix ,illegal,illegal,dec_ix ,illegal,
 illegal,illegal,tst_ix ,illegal,illegal,lsr_ix ,illegal,illegal, /* 90 */
 ror_ix ,illegal,illegal,asr_ix ,illegal,illegal,asl_ix ,illegal,
 illegal,illegal,rol_ix ,lsrw_ix,rorw_ix,asrw_ix,aslw_ix,rolw_ix, /* a0 */
 jmp_ix ,jsr_ix ,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* b0 */
 illegal,lsrd_ix,illegal,rord_ix,illegal,asrd_ix,illegal,asld_ix,
 illegal,rold_ix,illegal,clrw_ix,illegal,negw_ix,illegal,incw_ix, /* c0 */
 illegal,decw_ix,illegal,tstw_ix,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* d0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* e0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* f0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
};

            static opcode[] konami_direct = {
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 00 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,lda_di ,ldb_di ,illegal,illegal,adda_di,addb_di, /* 10 */
 illegal,illegal,adca_di,adcb_di,illegal,illegal,suba_di,subb_di,
 illegal,illegal,sbca_di,sbcb_di,illegal,illegal,anda_di,andb_di, /* 20 */
 illegal,illegal,bita_di,bitb_di,illegal,illegal,eora_di,eorb_di,
 illegal,illegal,ora_di ,orb_di ,illegal,illegal,cmpa_di,cmpb_di, /* 30 */
 illegal,setline_di,sta_di,stb_di,illegal,illegal,illegal,illegal,
 illegal,ldd_di ,illegal,ldx_di ,illegal,ldy_di ,illegal,ldu_di , /* 40 */
 illegal,lds_di ,illegal,cmpd_di,illegal,cmpx_di,illegal,cmpy_di,
 illegal,cmpu_di,illegal,cmps_di,illegal,addd_di,illegal,subd_di, /* 50 */
 std_di ,stx_di ,sty_di ,stu_di ,sts_di ,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 60 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 70 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,clr_di ,illegal,illegal,com_di ,illegal,illegal, /* 80 */
 neg_di ,illegal,illegal,inc_di ,illegal,illegal,dec_di ,illegal,
 illegal,illegal,tst_di ,illegal,illegal,lsr_di ,illegal,illegal, /* 90 */
 ror_di ,illegal,illegal,asr_di ,illegal,illegal,asl_di ,illegal,
 illegal,illegal,rol_di ,lsrw_di,rorw_di,asrw_di,aslw_di,rolw_di, /* a0 */
 jmp_di ,jsr_di ,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* b0 */
 illegal,lsrd_di,illegal,rord_di,illegal,asrd_di,illegal,asld_di,
 illegal,rold_di,illegal,clrw_di,illegal,negw_di,illegal,incw_di, /* c0 */
 illegal,decw_di,illegal,tstw_di,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* d0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* e0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* f0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
};

            static opcode[] konami_extended = {
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 00 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,lda_ex ,ldb_ex ,illegal,illegal,adda_ex,addb_ex, /* 10 */
 illegal,illegal,adca_ex,adcb_ex,illegal,illegal,suba_ex,subb_ex,
 illegal,illegal,sbca_ex,sbcb_ex,illegal,illegal,anda_ex,andb_ex, /* 20 */
 illegal,illegal,bita_ex,bitb_ex,illegal,illegal,eora_ex,eorb_ex,
 illegal,illegal,ora_ex ,orb_ex ,illegal,illegal,cmpa_ex,cmpb_ex, /* 30 */
 illegal,setline_ex,sta_ex,stb_ex,illegal,illegal,illegal,illegal,
 illegal,ldd_ex ,illegal,ldx_ex ,illegal,ldy_ex ,illegal,ldu_ex , /* 40 */
 illegal,lds_ex ,illegal,cmpd_ex,illegal,cmpx_ex,illegal,cmpy_ex,
 illegal,cmpu_ex,illegal,cmps_ex,illegal,addd_ex,illegal,subd_ex, /* 50 */
 std_ex ,stx_ex ,sty_ex ,stu_ex ,sts_ex ,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 60 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* 70 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,clr_ex ,illegal,illegal,com_ex ,illegal,illegal, /* 80 */
 neg_ex ,illegal,illegal,inc_ex ,illegal,illegal,dec_ex ,illegal,
 illegal,illegal,tst_ex ,illegal,illegal,lsr_ex ,illegal,illegal, /* 90 */
 ror_ex ,illegal,illegal,asr_ex ,illegal,illegal,asl_ex ,illegal,
 illegal,illegal,rol_ex ,lsrw_ex,rorw_ex,asrw_ex,aslw_ex,rolw_ex, /* a0 */
 jmp_ex ,jsr_ex ,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* b0 */
 illegal,lsrd_ex,illegal,rord_ex,illegal,asrd_ex,illegal,asld_ex,
 illegal,rold_ex,illegal,clrw_ex,illegal,negw_ex,illegal,incw_ex, /* c0 */
 illegal,decw_ex,illegal,tstw_ex,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* d0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* e0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal, /* f0 */
 illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
};
            static byte[] flags8i = /* increment */
{
0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x08|0x02,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08
};
            static byte[] flags8d = /* decrement */
{
0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08
};
            class konami_Regs
            {
                public PAIR pc; /* Program counter */
                public PAIR ppc; /* Previous program counter */
                public PAIR d; /* Accumulator a and b */
                public PAIR dp; /* Direct Page register (page in MSB) */
                public PAIR u, s; /* Stack pointers */
                public PAIR x, y; /* Index registers */
                public byte cc;
                public byte ireg; /* first opcode */
                public byte[] irq_state = new byte[2];
                public int extra_cycles; /* cycles used up by interrupts */
                public irqcallback irq_callback;
                public byte int_state; /* SYNC and CWAI flags */
                public byte nmi_state;
            }
            static PAIR ea;
            static int konami_Flags;
            delegate void cb(int i);
            static cb konami_cpu_setlines_callback = null;
            static konami_Regs konami = new konami_Regs();

            static uint RM16(uint Addr)
            {
                uint result = (uint)cpu_readmem16((int)Addr) << 8;
                return result | (uint)cpu_readmem16((int)(Addr + 1) & 0xffff);
            }
            static void WM16(uint Addr, PAIR p)
            {
                cpu_writemem16((int)Addr, p.bh);
                cpu_writemem16((int)(Addr + 1) & 0xffff, p.bl);
            }


            static void illegal()
            {
                printf("KONAMI: illegal opcode at %04x\n", konami.pc.wl);
            }





            /* $00 NEG direct ?**** */
            static void neg_di()
            {
                ushort r, t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)-t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04;
                    konami.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $01 ILLEGAL */

            /* $02 ILLEGAL */

            /* $03 COM direct -**01 */
            static void com_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                t = (byte)~t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
                konami.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $04 LSR direct -0*-* */
            static void lsr_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t & 0x01);
                t >>= 1;
                if ((byte)t == 0) konami.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $05 ILLEGAL */

            /* $06 ROR direct -**-* */
            static void ror_di()
            {
                byte t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)((konami.cc & 0x01) << 7);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1);
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $07 ASR direct ?**-* */
            static void asr_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $08 ASL direct ?**** */
            static void asl_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(t << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $09 ROL direct -**** */
            static void rol_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)((konami.cc & 0x01) | (t << 1));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $0A DEC direct -***- */
            static void dec_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                --t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $0B ILLEGAL */

            /* $OC INC direct -***- */
            static void inc_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                ++t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $OD TST direct -**0- */
            static void tst_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
            }

            /* $0E JMP direct ----- */
            static void jmp_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $0F CLR direct -0100 */
            static void clr_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                cpu_writemem16((int)ea.d, 0);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                konami.cc |= 0x04;
            }

            static void nop()
            {
                ;
            }

            /* $13 SYNC inherent ----- */
            static void sync()
            {
                /* SYNC stops processing instructions until an interrupt request happens. */
                /* This doesn't require the corresponding interrupt to be enabled: if it */
                /* is disabled, execution continues with the next instruction. */
                konami.int_state |= 16;
                if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                        --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                    }
                    konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6); change_pc(konami.pc.wl);
                    konami.irq_callback(1);
                }
                else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                {
                    if ((konami.int_state & 8) != 0) { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc |= 0x80; --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                    }
                    konami.cc |= 0x10; konami.pc.d = RM16(0xfff8); change_pc(konami.pc.wl);
                    konami.irq_callback(0);
                };
                /* if KONAMI_SYNC has not been cleared by CHECK_IRQ_LINES,

                    * stop execution until the interrupt lines change. */
                if ((konami.int_state & 16) != 0 && konami_ICount[0] > 0)
                    konami_ICount[0] = 0;
            }

            /* $14 ILLEGAL */

            /* $15 ILLEGAL */

            /* $16 LBRA relative ----- */
            static void lbra()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.pc.wl += ea.wl;
                change_pc(konami.pc.d);

                /* EHC 980508 speed up busy loop */
                if (ea.wl == 0xfffd && konami_ICount[0] > 0)
                    konami_ICount[0] = 0;
            }

            /* $17 LBSR relative ----- */
            static void lbsr()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                konami.pc.wl += ea.wl;
                change_pc(konami.pc.d);
            }

            /* $18 ILLEGAL */


            /* $19 DAA inherent (A) -**0* */
            static void daa()
            {
                byte msn, lsn;
                ushort t, cf = 0;
                msn = (byte)(konami.d.bh & 0xf0); lsn = (byte)(konami.d.bh & 0x0f);
                if (lsn > 0x09 || (konami.cc & 0x20) != 0) cf |= 0x06;
                if (msn > 0x80 && lsn > 0x09) cf |= 0x60;
                if (msn > 0x90 || (konami.cc & 0x01) != 0) cf |= 0x60;
                t = (ushort)(cf + konami.d.bh);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); /* keep carry from previous operation */
                { konami.cc |= (byte)(((byte)t & 0x80) >> 4); if ((byte)t == 0)konami.cc |= 0x04; }; konami.cc |= (byte)((t & 0x100) >> 8);
                konami.d.bh = (byte)t;
            }
            /* $1A ORCC immediate ##### */
            static void orcc()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.cc |= t;
                if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE)
                    konami.int_state &= unchecked((byte)~16); if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    if ((konami.int_state & 8) != 0) { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                    }
                    konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6);
                    change_pc(konami.pc.wl); konami.irq_callback(1);
                }
                else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                {
                    if ((konami.int_state & 8) != 0) { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc |= 0x80; --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                    }
                    konami.cc |= 0x10; konami.pc.d = RM16(0xfff8); change_pc(konami.pc.wl); konami.irq_callback(0);
                };
            }

            /* $1B ILLEGAL */

            /* $1C ANDCC immediate ##### */
            static void andcc()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.cc &= t;
                if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    {
                        konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7;
                    }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                    } konami.cc |= 0x40 | 0x10;
                    konami.pc.d = RM16(0xfff6); change_pc(konami.pc.wl);
                    konami.irq_callback(1);
                }
                else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                {
                    if ((konami.int_state & 8) != 0) { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc |= 0x80; --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                    }
                    konami.cc |= 0x10; konami.pc.d = RM16(0xfff8); change_pc(konami.pc.wl); konami.irq_callback(0);
                };
            }

            /* $1D SEX inherent -**0- */
            static void sex()
            {
                ushort t;
                t = (ushort)(short)(sbyte)(konami.d.bl);
                konami.d.wl = t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((t & 0x8000) >> 12); if (t == 0)konami.cc |= 0x04; };
            }

            /* $1E EXG inherent ----- */
            static void exg()
            {
                ushort t1 = 0, t2 = 0;
                byte tb;

                { tb = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                switch (tb >> 4)
                {
                    case 0: t1 = konami.d.bh; break;
                    case 1: t1 = konami.d.bl; break;
                    case 2: t1 = konami.x.wl; break;
                    case 3: t1 = konami.y.wl; break;
                    case 4: t1 = konami.s.wl; break;
                    case 5: t1 = konami.u.wl; break;
                    default: t1 = 0xff; printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };
                switch (tb & 0x0f)
                {
                    case 0: t2 = konami.d.bh; break;
                    case 1: t2 = konami.d.bl; break;
                    case 2: t2 = konami.x.wl; break;
                    case 3: t2 = konami.y.wl; break;
                    case 4: t2 = konami.s.wl; break;
                    case 5: t2 = konami.u.wl; break;
                    default: t2 = 0xff; printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };

                switch (tb >> 4)
                {
                    case 0: konami.d.bh = (byte)t2; break;
                    case 1: konami.d.bl = (byte)t2; break;
                    case 2: konami.x.wl = t2; break;
                    case 3: konami.y.wl = t2; break;
                    case 4: konami.s.wl = t2; break;
                    case 5: konami.u.wl = t2; break;
                    default: printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };
                switch (tb & 0x0f)
                {
                    case 0: konami.d.bh = (byte)t1; break;
                    case 1: konami.d.bl = (byte)t1; break;
                    case 2: konami.x.wl = t1; break;
                    case 3: konami.y.wl = t1; break;
                    case 4: konami.s.wl = t1; break;
                    case 5: konami.u.wl = t1; break;
                    default: printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };
            }

            /* $1F TFR inherent ----- */
            static void tfr()
            {
                byte tb;
                ushort t = 0;

                { tb = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                switch (tb & 0x0f)
                {
                    case 0: t = konami.d.bh; break;
                    case 1: t = konami.d.bl; break;
                    case 2: t = konami.x.wl; break;
                    case 3: t = konami.y.wl; break;
                    case 4: t = konami.s.wl; break;
                    case 5: t = konami.u.wl; break;
                    default: t = 0xff; printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };
                switch ((tb >> 4) & 0x07)
                {
                    case 0: konami.d.bh = (byte)t; break;
                    case 1: konami.d.bl = (byte)t; break;
                    case 2: konami.x.wl = t; break;
                    case 3: konami.y.wl = t; break;
                    case 4: konami.s.wl = t; break;
                    case 5: konami.u.wl = t; break;
                    default: printf("Unknown TFR/EXG idx at PC:%04x\n", konami.pc.wl); break;
                };
            }





            /* $20 BRA relative ----- */
            static void bra()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.pc.wl += (ushort)(short)(sbyte)(t);
                change_pc(konami.pc.d);
                /* JB 970823 - speed up busy loops */
                if (t == 0xfe && konami_ICount[0] > 0)
                    konami_ICount[0] = 0;
            }

            /* $21 BRN relative ----- */
            static void brn()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
            }

            /* $1021 LBRN relative ----- */
            static void lbrn()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
            }

            /* $22 BHI relative ----- */
            static void bhi()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                    if ((konami.cc & (0x04 | 0x01)) == 0) { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); }
                };
            }

            /* $1022 LBHI relative ----- */
            static void lbhi()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & (0x04 | 0x01)) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }


            /* $23 BLS relative ----- */
            static void bls()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & (0x04 | 0x01)) != 0)
                    { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); }
                };
            }

            /* $1023 LBLS relative ----- */
            static void lbls()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & (0x04 | 0x01)) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $24 BCC relative ----- */
            static void bcc()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x01) == 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1024 LBCC relative ----- */
            static void lbcc()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x01) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $25 BCS relative ----- */
            static void bcs()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x01) != 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1025 LBCS relative ----- */
            static void lbcs()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x01) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $26 BNE relative ----- */
            static void bne()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x04) == 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1026 LBNE relative ----- */
            static void lbne()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x04) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $27 BEQ relative ----- */
            static void beq()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x04) != 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1027 LBEQ relative ----- */
            static void lbeq()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x04) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $28 BVC relative ----- */
            static void bvc()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x02) == 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1028 LBVC relative ----- */
            static void lbvc()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x02) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $29 BVS relative ----- */
            static void bvs()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x02) != 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $1029 LBVS relative ----- */
            static void lbvs()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x02) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2A BPL relative ----- */
            static void bpl()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x08) == 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $102A LBPL relative ----- */
            static void lbpl()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x08) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2B BMI relative ----- */
            static void bmi()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x08) != 0)
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl);
                    }
                };
            }

            /* $102B LBMI relative ----- */
            static void lbmi()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if ((konami.cc & 0x08) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2C BGE relative ----- */
            static void bge()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if (((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) == 0)
                    { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); }
                };
            }

            /* $102C LBGE relative ----- */
            static void lbge()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    }; if (((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) == 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2D BLT relative ----- */
            static void blt()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                    if (((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) != 0)
                    { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); }
                };
            }

            /* $102D LBLT relative ----- */
            static void lblt()
            {
                {
                    PAIR t = new PAIR();
                    {
                        t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                        konami.pc.wl += 2;
                    };
                    if (((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) != 0)
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2E BGT relative ----- */
            static void bgt()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                    if ((((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) == 0) || ((konami.cc & 0x04) != 0))
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t);
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $102E LBGT relative ----- */
            static void lbgt()
            {
                {
                    PAIR t = new PAIR(); { t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                    if ((((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) == 0 || (konami.cc & 0x04) != 0))
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $2F BLE relative ----- */
            static void ble()
            {
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                    if ((((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) != 0 || (konami.cc & 0x04) != 0))
                    {
                        konami.pc.wl += (ushort)(short)(sbyte)(t);
                        change_pc(konami.pc.wl);
                    }
                };
            }

            /* $102F LBLE relative ----- */
            static void lble()
            {
                {
                    PAIR t = new PAIR(); { t.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                    if ((((konami.cc & 0x08) ^ ((konami.cc & 0x02) << 2)) != 0 || (konami.cc & 0x04) != 0))
                    {
                        konami_ICount[0] -= 1; konami.pc.wl += t.wl;
                        change_pc(konami.pc.wl);
                    }
                };
            }





            /* $30 LEAX indexed --*-- */
            static void leax()
            {
                konami.x.wl = ea.wl;
                konami.cc &= unchecked((byte)~(0x04));
                if (konami.x.wl == 0) konami.cc |= 0x04;
            }

            /* $31 LEAY indexed --*-- */
            static void leay()
            {
                konami.y.wl = ea.wl;
                konami.cc &= unchecked((byte)~(0x04));
                if (konami.y.wl == 0) konami.cc |= 0x04;
            }

            /* $32 LEAS indexed ----- */
            static void leas()
            {
                konami.s.wl = ea.wl;
                konami.int_state |= 32;
            }

            /* $33 LEAU indexed ----- */
            static void leau()
            {
                konami.u.wl = ea.wl;
            }

            /* $34 PSHS inherent ----- */
            static void pshs()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                if ((t & 0x80) != 0)
                {
                    --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.pc.bh); konami_ICount[0] -= 2;
                }
                if ((t & 0x40) != 0)
                {
                    --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.u.bh); konami_ICount[0] -= 2;
                }
                if ((t & 0x20) != 0)
                {
                    --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.y.bh); konami_ICount[0] -= 2;
                }
                if ((t & 0x10) != 0)
                {
                    --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                    cpu_writemem16((int)konami.s.d, konami.x.bh); konami_ICount[0] -= 2;
                }
                if ((t & 0x08) != 0) { --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.dp.bh); konami_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bl); konami_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bh); konami_ICount[0] -= 1; }
                if ((t & 0x01) != 0) { --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc); konami_ICount[0] -= 1; }
            }

            /* 35 PULS inherent ----- */
            static void puls()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                if ((t & 0x01) != 0) { konami.cc = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { konami.d.bh = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { konami.d.bl = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x08) != 0) { konami.dp.bh = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x10) != 0) { konami.x.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.x.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x20) != 0) { konami.y.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.y.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x40) != 0) { konami.u.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.u.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x80) != 0) { konami.pc.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.pc.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++; change_pc(konami.pc.d); konami_ICount[0] -= 2; }

                /* check after all PULLs */
                if ((t & 0x01) != 0)
                {
                    if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                    if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                    {
                        if ((konami.int_state & 8) != 0)
                        { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                        else
                        {
                            konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                        }
                        konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6); change_pc(konami.pc.wl); konami.irq_callback(1);
                    }
                    else if (konami.irq_state[0] != CLEAR_LINE &&
(konami.cc & 0x10) == 0)
                    {
                        if ((konami.int_state & 8) != 0)
                        {
                            konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7;
                        }
                        else
                        {
                            konami.cc |= 0x80; --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                        } konami.cc |= 0x10; konami.pc.d = RM16(0xfff8);
                        change_pc(konami.pc.wl); konami.irq_callback(0);
                    };
                }
            }

            /* $36 PSHU inherent ----- */
            static void pshu()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                if ((t & 0x80) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.pc.bl); --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.pc.bh); konami_ICount[0] -= 2; }
                if ((t & 0x40) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.s.bl); --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.s.bh); konami_ICount[0] -= 2; }
                if ((t & 0x20) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.y.bl); --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.y.bh); konami_ICount[0] -= 2; }
                if ((t & 0x10) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.x.bl); --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.x.bh); konami_ICount[0] -= 2; }
                if ((t & 0x08) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.dp.bh); konami_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.d.bl); konami_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.d.bh); konami_ICount[0] -= 1; }
                if ((t & 0x01) != 0) { --konami.u.wl; cpu_writemem16((int)konami.u.d, konami.cc); konami_ICount[0] -= 1; }
            }

            /* 37 PULU inherent ----- */
            static void pulu()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                if ((t & 0x01) != 0) { konami.cc = (byte)((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { konami.d.bh = (byte)((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { konami.d.bl = (byte)((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 1; }
                if ((t & 0x10) != 0) { konami.x.d = ((uint)cpu_readmem16((int)konami.u.d)) << 8; konami.u.wl++; konami.x.d |= ((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x20) != 0) { konami.y.d = ((uint)cpu_readmem16((int)konami.u.d)) << 8; konami.u.wl++; konami.y.d |= ((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x40) != 0) { konami.s.d = ((uint)cpu_readmem16((int)konami.u.d)) << 8; konami.u.wl++; konami.s.d |= ((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; konami_ICount[0] -= 2; }
                if ((t & 0x80) != 0) { konami.pc.d = ((uint)cpu_readmem16((int)konami.u.d)) << 8; konami.u.wl++; konami.pc.d |= ((uint)cpu_readmem16((int)konami.u.d)); konami.u.wl++; change_pc(konami.pc.d); konami_ICount[0] -= 2; }

                /* check after all PULLs */
                if ((t & 0x01) != 0)
                {
                    if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                    if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                    {
                        if ((konami.int_state & 8) != 0)
                        {
                            konami.int_state &= unchecked((byte)~8);
                            konami.extra_cycles += 7;
                        }
                        else
                        {
                            konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                        } konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6);
                        change_pc(konami.pc.wl); konami.irq_callback(1);
                    }
                    else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                    {
                        if ((konami.int_state & 8) != 0) { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                        else
                        {
                            konami.cc |= 0x80; --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                            cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                        } konami.cc |= 0x10; konami.pc.d = RM16(0xfff8);
                        change_pc(konami.pc.wl); konami.irq_callback(0);
                    };
                }
            }

            /* $38 ILLEGAL */

            /* $39 RTS inherent ----- */
            static void rts()
            {
                konami.pc.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.pc.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                change_pc(konami.pc.d);
            }

            /* $3A ABX inherent ----- */
            static void abx()
            {
                konami.x.wl += konami.d.bl;
            }

            /* $3B RTI inherent ##### */
            static void rti()
            {
                konami.cc = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                if ((konami.cc & 0x80) != 0) /* entire state saved? */
                {
                    konami_ICount[0] -= 9;
                    konami.d.bh = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                    konami.d.bl = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                    konami.dp.bh = (byte)((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                    konami.x.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.x.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                    konami.y.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.y.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                    konami.u.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.u.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                }
                konami.pc.d = ((uint)cpu_readmem16((int)konami.s.d)) << 8; konami.s.wl++; konami.pc.d |= ((uint)cpu_readmem16((int)konami.s.d)); konami.s.wl++;
                change_pc(konami.pc.d);
                if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                    } konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6);
                    change_pc(konami.pc.wl); konami.irq_callback(1);
                }
                else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    { konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7; }
                    else
                    {
                        konami.cc |= 0x80; --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                    } konami.cc |= 0x10; konami.pc.d = RM16(0xfff8);
                    change_pc(konami.pc.wl); konami.irq_callback(0);
                };
            }

            /* $3C CWAI inherent ----1 */
            static void cwai()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.cc &= t;
                /*

                    * CWAI stacks the entire machine state on the hardware stack,

                    * then waits for an interrupt; when the interrupt is taken

                    * later, the state is *not* saved again after CWAI.

                    */
                konami.cc |= 0x80; /* HJB 990225: save entire state */
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.dp.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bl);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc);
                konami.int_state |= 8;
                if (konami.irq_state[0] != CLEAR_LINE || konami.irq_state[1] != CLEAR_LINE) konami.int_state &= unchecked((byte)~16);
                if (konami.irq_state[1] != CLEAR_LINE && (konami.cc & 0x40) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    {
                        konami.int_state &= unchecked((byte)~8);
                        konami.extra_cycles += 7;
                    }
                    else
                    {
                        konami.cc &= unchecked((byte)~0x80); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 10;
                    } konami.cc |= 0x40 | 0x10; konami.pc.d = RM16(0xfff6);
                    change_pc(konami.pc.wl); konami.irq_callback(1);
                }
                else if (konami.irq_state[0] != CLEAR_LINE && (konami.cc & 0x10) == 0)
                {
                    if ((konami.int_state & 8) != 0)
                    {
                        konami.int_state &= unchecked((byte)~8); konami.extra_cycles += 7;
                    }
                    else
                    {
                        konami.cc |= 0x80; --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.pc.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.u.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.y.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.x.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.dp.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bl); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.d.bh); --konami.s.wl;
                        cpu_writemem16((int)konami.s.d, konami.cc); konami.extra_cycles += 19;
                    } konami.cc |= 0x10; konami.pc.d = RM16(0xfff8); change_pc(konami.pc.wl);
                    konami.irq_callback(0);
                };
                if ((konami.int_state & 8) != 0 && konami_ICount[0] > 0)
                    konami_ICount[0] = 0;
            }

            /* $3D MUL inherent --*-@ */
            static void mul()
            {
                ushort t;
                t = (ushort)(konami.d.bh * konami.d.bl);
                konami.cc &= unchecked((byte)~(0x04 | 0x01)); if ((ushort)t == 0) konami.cc |= 0x04; if ((t & 0x80) != 0) konami.cc |= 0x01;
                konami.d.wl = t;
            }

            /* $3E ILLEGAL */

            /* $3F SWI (SWI2 SWI3) absolute indirect ----- */
            static void swi()
            {
                konami.cc |= 0x80; /* HJB 980225: save entire state */
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.dp.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bl);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc);
                konami.cc |= 0x40 | 0x10; /* inhibit FIRQ and IRQ */
                konami.pc.d = RM16(0xfffa);
                change_pc(konami.pc.d);
            }

            /* $103F SWI2 absolute indirect ----- */
            static void swi2()
            {
                konami.cc |= 0x80; /* HJB 980225: save entire state */
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.dp.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bl);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc);
                konami.pc.d = RM16(0xfff4);
                change_pc(konami.pc.d);
            }

            /* $113F SWI3 absolute indirect ----- */
            static void swi3()
            {
                konami.cc |= 0x80; /* HJB 980225: save entire state */
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.u.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.y.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bl); --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.x.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.dp.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bl);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.d.bh);
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.cc);
                konami.pc.d = RM16(0xfff2);
                change_pc(konami.pc.d);
            }





            /* $40 NEGA inherent ?**** */
            static void nega()
            {
                ushort r;
                r = (ushort)(-konami.d.bh);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* $41 ILLEGAL */

            /* $42 ILLEGAL */

            /* $43 COMA inherent -**01 */
            static void coma()
            {
                konami.d.bh = (byte)~konami.d.bh;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
                konami.cc |= 0x01;
            }

            /* $44 LSRA inherent -0*-* */
            static void lsra()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bh & 0x01);
                konami.d.bh >>= 1;
                if ((byte)konami.d.bh == 0) konami.cc |= 0x04;
            }

            /* $45 ILLEGAL */

            /* $46 RORA inherent -**-* */
            static void rora()
            {
                byte r;
                r = (byte)((konami.cc & 0x01) << 7);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bh & 0x01);
                r |= (byte)(konami.d.bh >> 1);
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
                konami.d.bh = r;
            }

            /* $47 ASRA inherent ?**-* */
            static void asra()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bh & 0x01);
                konami.d.bh = (byte)((konami.d.bh & 0x80) | (konami.d.bh >> 1));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $48 ASLA inherent ?**** */
            static void asla()
            {
                ushort r;
                r = (ushort)(konami.d.bh << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ konami.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* $49 ROLA inherent -**** */
            static void rola()
            {
                ushort t, r;
                t = konami.d.bh;
                r = (ushort)((konami.cc & 0x01) | (t << 1));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* $4A DECA inherent -***- */
            static void deca()
            {
                --konami.d.bh;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8d[(konami.d.bh) & 0xff]; };
            }

            /* $4B ILLEGAL */

            /* $4C INCA inherent -***- */
            static void inca()
            {
                ++konami.d.bh;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8i[(konami.d.bh) & 0xff]; };
            }

            /* $4D TSTA inherent -**0- */
            static void tsta()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $4E ILLEGAL */

            /* $4F CLRA inherent -0100 */
            static void clra()
            {
                konami.d.bh = 0;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }





            /* $50 NEGB inherent ?**** */
            static void negb()
            {
                ushort r;
                r = (ushort)(-konami.d.bl);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $51 ILLEGAL */

            /* $52 ILLEGAL */

            /* $53 COMB inherent -**01 */
            static void comb()
            {
                konami.d.bl = (byte)~konami.d.bl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
                konami.cc |= 0x01;
            }

            /* $54 LSRB inherent -0*-* */
            static void lsrb()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bl & 0x01);
                konami.d.bl >>= 1;
                if ((byte)konami.d.bl == 0) konami.cc |= 0x04;
            }

            /* $55 ILLEGAL */

            /* $56 RORB inherent -**-* */
            static void rorb()
            {
                byte r;
                r = (byte)((konami.cc & 0x01) << 7);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bl & 0x01);
                r |= (byte)(konami.d.bl >> 1);
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
                konami.d.bl = r;
            }

            /* $57 ASRB inherent ?**-* */
            static void asrb()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(konami.d.bl & 0x01);
                konami.d.bl = (byte)((konami.d.bl & 0x80) | (konami.d.bl >> 1));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $58 ASLB inherent ?**** */
            static void aslb()
            {
                ushort r;
                r = (ushort)(konami.d.bl << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ konami.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $59 ROLB inherent -**** */
            static void rolb()
            {
                ushort t, r;
                t = konami.d.bl;
                r = (ushort)(konami.cc & 0x01);
                r |= (ushort)(t << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $5A DECB inherent -***- */
            static void decb()
            {
                --konami.d.bl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8d[(konami.d.bl) & 0xff]; };
            }

            /* $5B ILLEGAL */

            /* $5C INCB inherent -***- */
            static void incb()
            {
                ++konami.d.bl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8i[(konami.d.bl) & 0xff]; };
            }

            /* $5D TSTB inherent -**0- */
            static void tstb()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $5E ILLEGAL */

            /* $5F CLRB inherent -0100 */
            static void clrb()
            {
                konami.d.bl = 0;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }





            /* $60 NEG indexed ?**** */
            static void neg_ix()
            {
                ushort r, t;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)-t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)ea.d, r);
            }

            /* $61 ILLEGAL */

            /* $62 ILLEGAL */

            /* $63 COM indexed -**01 */
            static void com_ix()
            {
                byte t;
                t = (byte)~((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
                konami.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $64 LSR indexed -0*-* */
            static void lsr_ix()
            {
                byte t;
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t & 0x01);
                t >>= 1; if ((byte)t == 0) konami.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $65 ILLEGAL */

            /* $66 ROR indexed -**-* */
            static void ror_ix()
            {
                byte t, r;
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)((konami.cc & 0x01) << 7);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $67 ASR indexed ?**-* */
            static void asr_ix()
            {
                byte t;
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)(~(0x08 | 0x04 | 0x01)));
                konami.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >>= 1));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $68 ASL indexed ?**** */
            static void asl_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(t << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)ea.d, r);
            }

            /* $69 ROL indexed -**** */
            static void rol_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.cc & 0x01);
                r |= (ushort)(t << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)ea.d, r);
            }

            /* $6A DEC indexed -***- */
            static void dec_ix()
            {
                byte t;
                t = (byte)(((uint)cpu_readmem16((int)ea.d)) - 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $6B ILLEGAL */

            /* $6C INC indexed -***- */
            static void inc_ix()
            {
                byte t;
                t = (byte)(((uint)cpu_readmem16((int)ea.d)) + 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $6D TST indexed -**0- */
            static void tst_ix()
            {
                byte t;
                t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
            }

            /* $6E JMP indexed ----- */
            static void jmp_ix()
            {
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $6F CLR indexed -0100 */
            static void clr_ix()
            {
                cpu_writemem16((int)ea.d, 0);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }





            /* $70 NEG extended ?**** */
            static void neg_ex()
            {
                ushort r, t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d)); r = (ushort)-t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)ea.d, r);
            }

            /* $71 ILLEGAL */

            /* $72 ILLEGAL */

            /* $73 COM extended -**01 */
            static void com_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d)); t = (byte)~t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; }; konami.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $74 LSR extended -0*-* */
            static void lsr_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d)); konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01)); konami.cc |= (byte)(t & 0x01);
                t >>= 1; if ((byte)t == 0) konami.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $75 ILLEGAL */

            /* $76 ROR extended -**-* */
            static void ror_ex()
            {
                byte t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d)); r = (byte)((konami.cc & 0x01) << 7);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01)); konami.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $77 ASR extended ?**-* */
            static void asr_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d)); konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01)); konami.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $78 ASL extended ?**** */
            static void asl_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (ushort)((uint)cpu_readmem16((int)ea.d)); r = (ushort)(t << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $79 ROL extended -**** */
            static void rol_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (ushort)((uint)cpu_readmem16((int)ea.d)); r = (ushort)((konami.cc & 0x01) | (t << 1));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                cpu_writemem16((int)ea.d, r);
            }

            /* $7A DEC extended -***- */
            static void dec_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d)); --t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $7B ILLEGAL */

            /* $7C INC extended -***- */
            static void inc_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d)); ++t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $7D TST extended -**0- */
            static void tst_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d)); konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((t & 0x80) >> 4); if (t == 0)konami.cc |= 0x04; };
            }

            /* $7E JMP extended ----- */
            static void jmp_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $7F CLR extended -0100 */
            static void clr_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                cpu_writemem16((int)ea.d, 0);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }






            /* $80 SUBA immediate ?**** */
            static void suba_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $81 CMPA immediate ?**** */
            static void cmpa_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $82 SBCA immediate ?**** */
            static void sbca_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bh - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $83 SUBD (CMPD CMPU) immediate -**** */
            static void subd_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (Byte)((r & 0x10000) >> 16); };
                konami.d.wl = (byte)r;
            }

            /* $1083 CMPD immediate -**** */
            static void cmpd_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $1183 CMPU immediate -**** */
            static void cmpu_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.u.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $84 ANDA immediate -**0- */
            static void anda_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bh &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $85 BITA immediate -**0- */
            static void bita_im()
            {
                byte t, r;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (byte)(konami.d.bh & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $86 LDA immediate -**0- */
            static void lda_im()
            {
                { konami.d.bh = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $87 STA immediate -**0- */
            static void sta_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl++;
                cpu_writemem16((int)ea.d, konami.d.bh);
            }

            /* $88 EORA immediate -**0- */
            static void eora_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bh ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $89 ADCA immediate ***** */
            static void adca_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bh + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $8A ORA immediate -**0- */
            static void ora_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bh |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $8B ADDA immediate ***** */
            static void adda_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bh + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $8C CMPX (CMPY CMPS) immediate -**** */
            static void cmpx_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.x.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $108C CMPY immediate -**** */
            static void cmpy_im()
            {
                uint r, d;
                PAIR b = new PAIR();
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.y.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $118C CMPS immediate -**** */
            static void cmps_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.s.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $8D BSR ----- */
            static void bsr()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                cpu_writemem16((int)konami.s.d, konami.pc.bh);
                konami.pc.wl += (ushort)(short)(sbyte)(t);
                change_pc(konami.pc.d);
            }

            /* $8E LDX (LDY) immediate -**0- */
            static void ldx_im()
            {
                { konami.x.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
            }

            /* $108E LDY immediate -**0- */
            static void ldy_im()
            {
                { konami.y.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $8F STX (STY) immediate -**0- */
            static void stx_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl += 2;
                WM16(ea.d, konami.x);
            }

            /* is this a legal instruction? */
            /* $108F STY immediate -**0- */
            static void sty_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl += 2;
                WM16(ea.d, konami.y);
            }





            /* $90 SUBA direct ?**** */
            static void suba_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* $91 CMPA direct ?**** */
            static void cmpa_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $92 SBCA direct ?**** */
            static void sbca_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* $93 SUBD (CMPD CMPU) direct -**** */
            static void subd_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
                konami.d.wl = (ushort)r;
            }

            /* $1093 CMPD direct -**** */
            static void cmpd_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $1193 CMPU direct -**** */
            static void cmpu_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.u.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.u.wl ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $94 ANDA direct -**0- */
            static void anda_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $95 BITA direct -**0- */
            static void bita_di()
            {
                byte t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)(konami.d.bh & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $96 LDA direct -**0- */
            static void lda_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.d.bh = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $97 STA direct -**0- */
            static void sta_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                cpu_writemem16((int)ea.d, konami.d.bh);
            }

            /* $98 EORA direct -**0- */
            static void eora_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $99 ADCA direct ***** */
            static void adca_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $9A ORA direct -**0- */
            static void ora_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $9B ADDA direct ***** */
            static void adda_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $9C CMPX (CMPY CMPS) direct -**** */
            static void cmpx_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.x.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $109C CMPY direct -**** */
            static void cmpy_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.y.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $119C CMPS direct -**** */
            static void cmps_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.s.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $9D JSR direct ----- */
            static void jsr_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                cpu_writemem16((int)konami.s.d, konami.pc.bh);
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $9E LDX (LDY) direct -**0- */
            static void ldx_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.x.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
            }

            /* $109E LDY direct -**0- */
            static void ldy_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.y.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
            }

            /* $9F STX (STY) direct -**0- */
            static void stx_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, konami.x);
            }

            /* $109F STY direct -**0- */
            static void sty_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, konami.y);
            }






            /* $a0 SUBA indexed ?**** */
            static void suba_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $a1 CMPA indexed ?**** */
            static void cmpa_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $a2 SBCA indexed ?**** */
            static void sbca_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $a3 SUBD (CMPD CMPU) indexed -**** */
            static void subd_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $10a3 CMPD indexed -**** */
            static void cmpd_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $11a3 CMPU indexed -**** */
            static void cmpu_ix()
            {
                uint r;
                PAIR b;
                b.d = RM16(ea.d);
                r = konami.u.wl - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.u.wl ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $a4 ANDA indexed -**0- */
            static void anda_ix()
            {
                konami.d.bh &= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $a5 BITA indexed -**0- */
            static void bita_ix()
            {
                byte r;
                r = (byte)(konami.d.bh & ((uint)cpu_readmem16((int)ea.d)));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $a6 LDA indexed -**0- */
            static void lda_ix()
            {
                konami.d.bh = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $a7 STA indexed -**0- */
            static void sta_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, konami.d.bh);
            }

            /* $a8 EORA indexed -**0- */
            static void eora_ix()
            {
                konami.d.bh ^= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $a9 ADCA indexed ***** */
            static void adca_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04;
                    konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $aA ORA indexed -**0- */
            static void ora_ix()
            {
                konami.d.bh |= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $aB ADDA indexed ***** */
            static void adda_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $aC CMPX (CMPY CMPS) indexed -**** */
            static void cmpx_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.x.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $10aC CMPY indexed -**** */
            static void cmpy_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.y.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $11aC CMPS indexed -**** */
            static void cmps_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.s.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $aD JSR indexed ----- */
            static void jsr_ix()
            {
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                cpu_writemem16((int)konami.s.d, konami.pc.bh);
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $aE LDX (LDY) indexed -**0- */
            static void ldx_ix()
            {
                konami.x.wl = (ushort)RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
            }

            /* $10aE LDY indexed -**0- */
            static void ldy_ix()
            {
                konami.y.wl = (ushort)RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
            }

            /* $aF STX (STY) indexed -**0- */
            static void stx_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
                WM16(ea.d, konami.x);
            }

            /* $10aF STY indexed -**0- */
            static void sty_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
                WM16(ea.d, konami.y);
            }





            /* $b0 SUBA extended ?**** */
            static void suba_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $b1 CMPA extended ?**** */
            static void cmpa_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $b2 SBCA extended ?**** */
            static void sbca_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bh = (byte)r;
            }

            /* $b3 SUBD (CMPD CMPU) extended -**** */
            static void subd_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $10b3 CMPD extended -**** */
            static void cmpd_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $11b3 CMPU extended -**** */
            static void cmpu_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.u.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $b4 ANDA extended -**0- */
            static void anda_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $b5 BITA extended -**0- */
            static void bita_ex()
            {
                byte t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)(konami.d.bh & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $b6 LDA extended -**0- */
            static void lda_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.d.bh = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $b7 STA extended -**0- */
            static void sta_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                cpu_writemem16((int)ea.d, konami.d.bh);
            }

            /* $b8 EORA extended -**0- */
            static void eora_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $b9 ADCA extended ***** */
            static void adca_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $bA ORA extended -**0- */
            static void ora_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bh |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bh & 0x80) >> 4); if (konami.d.bh == 0)konami.cc |= 0x04; };
            }

            /* $bB ADDA extended ***** */
            static void adda_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bh + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bh ^ t ^ r) & 0x10) << 1);
                konami.d.bh = (byte)r;
            }

            /* $bC CMPX (CMPY CMPS) extended -**** */
            static void cmpx_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.x.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $10bC CMPY extended -**** */
            static void cmpy_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.y.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $11bC CMPS extended -**** */
            static void cmps_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.s.wl;
                r = d - b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
            }

            /* $bD JSR extended ----- */
            static void jsr_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                --konami.s.wl; cpu_writemem16((int)konami.s.d, konami.pc.bl); --konami.s.wl;
                cpu_writemem16((int)konami.s.d, konami.pc.bh);
                konami.pc.d = ea.d;
                change_pc(konami.pc.d);
            }

            /* $bE LDX (LDY) extended -**0- */
            static void ldx_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.x.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
            }

            /* $10bE LDY extended -**0- */
            static void ldy_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.y.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
            }

            /* $bF STX (STY) extended -**0- */
            static void stx_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, konami.x);
            }

            /* $10bF STY extended -**0- */
            static void sty_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.y.wl & 0x8000) >> 12); if (konami.y.wl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, konami.y);
            }






            /* $c0 SUBB immediate ?**** */
            static void subb_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $c1 CMPB immediate ?**** */
            static void cmpb_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $c2 SBCB immediate ?**** */
            static void sbcb_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bl - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $c3 ADDD immediate -**** */
            static void addd_im()
            {
                uint r, d;
                PAIR b;
                { b.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                d = konami.d.wl;
                r = d + b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $c4 ANDB immediate -**0- */
            static void andb_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bl &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $c5 BITB immediate -**0- */
            static void bitb_im()
            {
                byte t, r;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (byte)(konami.d.bl & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $c6 LDB immediate -**0- */
            static void ldb_im()
            {
                { konami.d.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $c7 STB immediate -**0- */
            static void stb_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl++;
                cpu_writemem16((int)ea.d, konami.d.bl);
            }

            /* $c8 EORB immediate -**0- */
            static void eorb_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bl ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $c9 ADCB immediate ***** */
            static void adcb_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bl + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $cA ORB immediate -**0- */
            static void orb_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                konami.d.bl |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $cB ADDB immediate ***** */
            static void addb_im()
            {
                ushort t, r;
                { t = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                r = (ushort)(konami.d.bl + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $cC LDD immediate -**0- */
            static void ldd_im()
            {
                { konami.d.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $cD STD immediate -**0- */
            static void std_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl += 2;
                WM16(ea.d, konami.d);
            }

            /* $cE LDU (LDS) immediate -**0- */
            static void ldu_im()
            {
                { konami.u.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
            }

            /* $10cE LDS immediate -**0- */
            static void lds_im()
            {
                { konami.s.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                konami.int_state |= 32;
            }

            /* is this a legal instruction? */
            /* $cF STU (STS) immediate -**0- */
            static void stu_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl += 2;
                WM16(ea.d, konami.u);
            }

            /* is this a legal instruction? */
            /* $10cF STS immediate -**0- */
            static void sts_im()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.pc.d; konami.pc.wl += 2;
                WM16(ea.d, konami.s);
            }






            /* $d0 SUBB direct ?**** */
            static void subb_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $d1 CMPB direct ?**** */
            static void cmpb_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $d2 SBCB direct ?**** */
            static void sbcb_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $d3 ADDD direct -**** */
            static void addd_di()
            {
                uint r, d;
                PAIR b;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d + b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $d4 ANDB direct -**0- */
            static void andb_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $d5 BITB direct -**0- */
            static void bitb_di()
            {
                byte t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)(konami.d.bl & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $d6 LDB direct -**0- */
            static void ldb_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.d.bl = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $d7 STB direct -**0- */
            static void stb_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                cpu_writemem16((int)ea.d, konami.d.bl);
            }

            /* $d8 EORB direct -**0- */
            static void eorb_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $d9 ADCB direct ***** */
            static void adcb_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $dA ORB direct -**0- */
            static void orb_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $dB ADDB direct ***** */
            static void addb_di()
            {
                ushort t, r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $dC LDD direct -**0- */
            static void ldd_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.d.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
            }

            /* $dD STD direct -**0- */
            static void std_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, konami.d);
            }

            /* $dE LDU (LDS) direct -**0- */
            static void ldu_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; konami.u.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
            }

            /* $10dE LDS direct -**0- */
            static void lds_di()
            {
                ea.d = konami.dp.d; { ea.bl = (byte)(uint)cpu_readop_arg(konami.pc.d); konami.pc.wl++; }; konami.s.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                konami.int_state |= 32;
            }

            /* $dF STU (STS) direct -**0- */
            static void stu_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, konami.u);
            }

            /* $10dF STS direct -**0- */
            static void sts_di()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, konami.s);
            }






            /* $e0 SUBB indexed ?**** */
            static void subb_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $e1 CMPB indexed ?**** */
            static void cmpb_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $e2 SBCB indexed ?**** */
            static void sbcb_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $e3 ADDD indexed -**** */
            static void addd_ix()
            {
                uint r, d;
                PAIR b;
                b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d + b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $e4 ANDB indexed -**0- */
            static void andb_ix()
            {
                konami.d.bl &= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if ((byte)konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $e5 BITB indexed -**0- */
            static void bitb_ix()
            {
                byte r;
                r = (byte)(konami.d.bl & ((uint)cpu_readmem16((int)ea.d)));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $e6 LDB indexed -**0- */
            static void ldb_ix()
            {
                konami.d.bl = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $e7 STB indexed -**0- */
            static void stb_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
                cpu_writemem16((int)ea.d, konami.d.bl);
            }

            /* $e8 EORB indexed -**0- */
            static void eorb_ix()
            {
                konami.d.bl ^= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $e9 ADCB indexed ***** */
            static void adcb_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t + (konami.cc & 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $eA ORB indexed -**0- */
            static void orb_ix()
            {
                konami.d.bl |= (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $eb ADDB indexed ***** */
            static void addb_ix()
            {
                ushort t, r;
                t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $ec LDD indexed -**0- */
            static void ldd_ix()
            {
                konami.d.wl = (ushort)RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
            }

            /* $eD STD indexed -**0- */
            static void std_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                WM16(ea.d, konami.d);
            }

            /* $eE LDU (LDS) indexed -**0- */
            static void ldu_ix()
            {
                konami.u.wl = (ushort)RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
            }

            /* $10eE LDS indexed -**0- */
            static void lds_ix()
            {
                konami.s.wl = (ushort)RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                konami.int_state |= 32;
            }

            /* $eF STU (STS) indexed -**0- */
            static void stu_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
                WM16(ea.d, konami.u);
            }

            /* $10eF STS indexed -**0- */
            static void sts_ix()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                WM16(ea.d, konami.s);
            }





            /* $f0 SUBB extended ?**** */
            static void subb_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $f1 CMPB extended ?**** */
            static void cmpb_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
            }

            /* $f2 SBCB extended ?**** */
            static void sbcb_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl - t - (konami.cc & 0x01));
                konami.cc &= unchecked((byte)(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.d.bl = (byte)r;
            }

            /* $f3 ADDD extended -**** */
            static void addd_ex()
            {
                uint r, d;
                PAIR b;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; b.d = RM16(ea.d);
                d = konami.d.wl;
                r = d + b.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* $f4 ANDB extended -**0- */
            static void andb_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl &= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $f5 BITB extended -**0- */
            static void bitb_ex()
            {
                byte t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                r = (byte)(konami.d.bl & t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x80) >> 4); if (r == 0)konami.cc |= 0x04; };
            }

            /* $f6 LDB extended -**0- */
            static void ldb_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.d.bl = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $f7 STB extended -**0- */
            static void stb_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                cpu_writemem16((int)ea.d, konami.d.bl);
            }

            /* $f8 EORB extended -**0- */
            static void eorb_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl ^= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $f9 ADCB extended ***** */
            static void adcb_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t + (konami.cc & 0x01));
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $fA ORB extended -**0- */
            static void orb_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));
                konami.d.bl |= t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.bl & 0x80) >> 4); if (konami.d.bl == 0)konami.cc |= 0x04; };
            }

            /* $fB ADDB extended ***** */
            static void addb_ex()
            {
                ushort t, r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (ushort)((uint)cpu_readmem16((int)ea.d));
                r = (ushort)(konami.d.bl + t);
                konami.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); konami.cc |= (byte)((r & 0x100) >> 8); };
                konami.cc |= (byte)(((konami.d.bl ^ t ^ r) & 0x10) << 1);
                konami.d.bl = (byte)r;
            }

            /* $fC LDD extended -**0- */
            static void ldd_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.d.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
            }

            /* $fD STD extended -**0- */
            static void std_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, konami.d);
            }

            /* $fE LDU (LDS) extended -**0- */
            static void ldu_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.u.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
            }

            /* $10fE LDS extended -**0- */
            static void lds_ex()
            {
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; konami.s.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                konami.int_state |= 32;
            }

            /* $fF STU (STS) extended -**0- */
            static void stu_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.u.wl & 0x8000) >> 12); if (konami.u.wl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, konami.u);
            }

            /* $10fF STS extended -**0- */
            static void sts_ex()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.s.wl & 0x8000) >> 12); if (konami.s.wl == 0)konami.cc |= 0x04; };
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, konami.s);
            }

            static void setline_im()
            {
                byte t;
                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                if (konami_cpu_setlines_callback != null)
                    konami_cpu_setlines_callback(t);
            }

            static void setline_ix()
            {
                byte t;
                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                if (konami_cpu_setlines_callback != null)
                    konami_cpu_setlines_callback(t);
            }

            static void setline_di()
            {
                byte t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                if (konami_cpu_setlines_callback != null)
                    konami_cpu_setlines_callback(t);
            }

            static void setline_ex()
            {
                byte t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                if (konami_cpu_setlines_callback != null)
                    konami_cpu_setlines_callback(t);
            }

            static void bmove()
            {
                byte t;

                while (konami.u.wl != 0)
                {
                    t = (byte)((uint)cpu_readmem16((int)konami.y.wl));
                    cpu_writemem16((int)konami.x.wl, t);
                    konami.y.wl++;
                    konami.x.wl++;
                    konami.u.wl--;
                    konami_ICount[0] -= 2;
                }
            }

            static void move()
            {
                byte t;

                t = (byte)((uint)cpu_readmem16((int)konami.y.wl));
                cpu_writemem16((int)konami.x.wl, t);
                konami.y.wl++;
                konami.x.wl++;
                konami.u.wl--;
            }

            /* CLRD inherent -0100 */
            static void clrd()
            {
                konami.d.wl = 0;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }

            /* CLRW indexed -0100 */
            static void clrw_ix()
            {
                PAIR t = new PAIR();
                t.d = 0;
                WM16(ea.d, t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }

            /* CLRW direct -0100 */
            static void clrw_di()
            {
                PAIR t = new PAIR();
                t.d = 0;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                WM16(ea.d, t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                konami.cc |= 0x04;
            }

            /* CLRW extended -0100 */
            static void clrw_ex()
            {
                PAIR t = new PAIR();
                t.d = 0;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                WM16(ea.d, t);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); konami.cc |= 0x04;
            }

            /* LSRD immediate -0*-* */
            static void lsrd()
            {
                byte t;

                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl >>= 1;
                    if ((ushort)konami.d.wl == 0) konami.cc |= 0x04;
                }
            }

            /* RORD immediate -**-* */
            static void rord()
            {
                ushort r;
                byte t;

                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                while (t-- != 0)
                {
                    r = (ushort)((konami.cc & 0x01) << 15);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    r |= (ushort)(konami.d.wl >> 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* ASRD immediate ?**-* */
            static void asrd()
            {
                byte t;

                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl = (ushort)((konami.d.wl & 0x8000) | (konami.d.wl >> 1));
                    { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                }
            }

            /* ASLD immediate ?**** */
            static void asld()
            {
                uint r;
                byte t;

                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                while (t-- != 0)
                {
                    r = (ushort)(konami.d.wl << 1);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                    konami.d.wl = (ushort)r;
                }
            }

            /* ROLD immediate -**-* */
            static void rold()
            {
                ushort r;
                byte t;

                { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    if ((konami.d.wl & 0x8000) != 0) konami.cc |= 0x01;
                    r = (ushort)(konami.cc & 0x01);
                    r |= (ushort)(konami.d.wl << 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* DECB,JNZ relative ----- */
            static void decbjnz()
            {
                --konami.d.bl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= flags8d[(konami.d.bl) & 0xff]; };
                {
                    byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                    if ((konami.cc & 0x04) == 0) { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); }
                };
            }

            /* DECX,JNZ relative ----- */
            static void decxjnz()
            {
                --konami.x.wl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.x.wl & 0x8000) >> 12); if (konami.x.wl == 0)konami.cc |= 0x04; }; /* should affect V as well? */
                { byte t; { t = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; if ((konami.cc & 0x04) == 0) { konami.pc.wl += (ushort)(short)(sbyte)(t); change_pc(konami.pc.wl); } };
            }

            static void bset()
            {
                byte t;

                while (konami.u.wl != 0)
                {
                    t = konami.d.bh;
                    cpu_writemem16((int)konami.x.d, t);
                    konami.x.wl++;
                    konami.u.wl--;
                    konami_ICount[0] -= 2;
                }
            }

            static void bset2()
            {
                while (konami.u.wl != 0)
                {
                    WM16(konami.x.d, konami.d);
                    konami.x.wl += 2;
                    konami.u.wl--;
                    konami_ICount[0] -= 3;
                }
            }

            /* LMUL inherent --*-@ */
            static void lmul()
            {
                uint t;
                t = (uint)(konami.x.wl * konami.y.wl);
                konami.x.wl = (ushort)(t >> 16);
                konami.y.wl = (ushort)(t & 0xffff);
                konami.cc &= unchecked((byte)~(0x04 | 0x01)); if (t == 0) konami.cc |= 0x04; if ((t & 0x8000) != 0) konami.cc |= 0x01;
            }

            /* DIVX inherent --*-@ */
            static void divx()
            {
                ushort t;
                byte r;
                if (konami.d.bl != 0)
                {
                    t = (ushort)(konami.x.wl / konami.d.bl);
                    r = (byte)(konami.x.wl % konami.d.bl);
                }
                else
                {
                    /* ?? */
                    t = 0;
                    r = 0;
                }
                konami.cc &= unchecked((byte)~(0x04 | 0x01)); if ((ushort)t == 0) konami.cc |= 0x04; if ((t & 0x80) != 0) konami.cc |= 0x01;
                konami.x.wl = t;
                konami.d.bl = r;
            }

            /* INCD inherent -***- */
            static void incd()
            {
                uint r;
                r = (uint)(konami.d.wl + 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* INCW direct -***- */
            static void incw_di()
            {
                PAIR t = new PAIR(), r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r = t;
                ++r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); }; ;
                WM16(ea.d, r);
            }

            /* INCW indexed -***- */
            static void incw_ix()
            {
                PAIR t = new PAIR(), r;
                t.d = RM16(ea.d);
                r = t;
                ++r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* INCW extended -***- */
            static void incw_ex()
            {
                PAIR t = new PAIR(), r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r = t;
                ++r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* DECD inherent -***- */
            static void decd()
            {
                uint r;
                r = (uint)(konami.d.wl - 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* DECW direct -***- */
            static void decw_di()
            {
                PAIR t = new PAIR(), r;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r = t;
                --r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); }; ;
                WM16(ea.d, r);
            }

            /* DECW indexed -***- */
            static void decw_ix()
            {
                PAIR t = new PAIR(), r;
                t.d = RM16(ea.d);
                r = t;
                --r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* DECW extended -***- */
            static void decw_ex()
            {
                PAIR t = new PAIR(), r;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r = t;
                --r.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* TSTD inherent -**0- */
            static void tstd()
            {
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
            }

            /* TSTW direct -**0- */
            static void tstw_di()
            {
                PAIR t = new PAIR();
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
            }

            /* TSTW indexed -**0- */
            static void tstw_ix()
            {
                PAIR t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                t.d = RM16(ea.d);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
            }

            /* TSTW extended -**0- */
            static void tstw_ex()
            {
                PAIR t;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
            }

            /* LSRW direct -0*-* */
            static void lsrw_di()
            {
                PAIR t = new PAIR();
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d >>= 1;
                if ((ushort)t.d == 0) konami.cc |= 0x04;
                WM16(ea.d, t);
            }

            /* LSRW indexed -0*-* */
            static void lsrw_ix()
            {
                PAIR t = new PAIR();
                t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d >>= 1;
                if ((ushort)t.d == 0) konami.cc |= 0x04;
                WM16(ea.d, t);
            }

            /* LSRW extended -0*-* */
            static void lsrw_ex()
            {
                PAIR t = new PAIR();
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d >>= 1;
                if ((ushort)t.d == 0) konami.cc |= 0x04;
                WM16(ea.d, t);
            }

            /* RORW direct -**-* */
            static void rorw_di()
            {
                PAIR t, r = new PAIR();
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r.d = (uint)((konami.cc & 0x01) << 15);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                r.d |= t.d >> 1;
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if (r.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, r);
            }

            /* RORW indexed -**-* */
            static void rorw_ix()
            {
                PAIR t, r = new PAIR();
                t.d = RM16(ea.d);
                r.d = (uint)(konami.cc & 0x01) << 15;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                r.d |= t.d >> 1;
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if (r.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, r);
            }

            /* RORW extended -**-* */
            static void rorw_ex()
            {
                PAIR t, r = new PAIR();
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r.d = (uint)(konami.cc & 0x01) << 15;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                r.d |= t.d >> 1;
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if (r.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, r);
            }

            /* ASRW direct ?**-* */
            static void asrw_di()
            {
                PAIR t = new PAIR();
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d = (t.d & 0x8000) | (t.d >> 1);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, t);
            }

            /* ASRW indexed ?**-* */
            static void asrw_ix()
            {
                PAIR t = new PAIR();
                t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d = (t.d & 0x8000) | (t.d >> 1);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, t);
            }

            /* ASRW extended ?**-* */
            static void asrw_ex()
            {
                PAIR t = new PAIR();
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                konami.cc |= (byte)(t.d & 0x01);
                t.d = (t.d & 0x8000) | (t.d >> 1);
                { konami.cc |= (byte)((t.d & 0x8000) >> 12); if (t.d == 0)konami.cc |= 0x04; };
                WM16(ea.d, t);
            }

            /* ASLW direct ?**** */
            static void aslw_di()
            {
                PAIR t, r = new PAIR();
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r.d = t.d << 1;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ASLW indexed ?**** */
            static void aslw_ix()
            {
                PAIR t, r = new PAIR();
                t.d = RM16(ea.d);
                r.d = t.d << 1;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ASLW extended ?**** */
            static void aslw_ex()
            {
                PAIR t, r = new PAIR();
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r.d = t.d << 1;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ROLW direct -**** */
            static void rolw_di()
            {
                PAIR t, r = new PAIR();
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r.d = (uint)(konami.cc & 0x01) | (t.d << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ROLW indexed -**** */
            static void rolw_ix()
            {
                PAIR t, r = new PAIR();
                t.d = RM16(ea.d);
                r.d = (uint)(konami.cc & 0x01) | (t.d << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ROLW extended -**** */
            static void rolw_ex()
            {
                PAIR t, r = new PAIR();
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r.d = (uint)(konami.cc & 0x01) | (t.d << 1);
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((t.d ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* NEGD inherent ?**** */
            static void negd()
            {
                uint r;
                r = (uint)-konami.d.wl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r & 0x10000) >> 16); };
                konami.d.wl = (ushort)r;
            }

            /* NEGW direct ?**** */
            static void negw_di()
            {
                PAIR r = new PAIR(), t;
                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t.d = RM16(ea.d);
                r.d = (uint)-t.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* NEGW indexed ?**** */
            static void negw_ix()
            {
                PAIR r = new PAIR(), t;
                t.d = RM16(ea.d);
                r.d = (uint)-t.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* NEGW extended ?**** */
            static void negw_ex()
            {
                PAIR r = new PAIR(), t;
                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; }; t.d = RM16(ea.d);
                r.d = (uint)-t.d;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                { konami.cc |= (byte)((r.d & 0x8000) >> 12); if ((ushort)r.d == 0)konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ t.d ^ r.d ^ (r.d >> 1)) & 0x8000) >> 14); konami.cc |= (byte)((r.d & 0x10000) >> 16); };
                WM16(ea.d, r);
            }

            /* ABSA inherent ?**** */
            static void absa()
            {
                ushort r;
                if ((konami.d.bh & 0x80) != 0)
                    r = (ushort)-konami.d.bh;
                else
                    r = konami.d.bh;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bh = (byte)r;
            }

            /* ABSB inherent ?**** */
            static void absb()
            {
                ushort r;
                if ((konami.d.bl & 0x80) != 0)
                    r = (ushort)-konami.d.bl;
                else
                    r = konami.d.bl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6);
                    konami.cc |= (byte)((r & 0x100) >> 8);
                };
                konami.d.bl = (byte)r;
            }

            /* ABSD inherent ?**** */
            static void absd()
            {
                uint r;
                if ((konami.d.wl & 0x8000) != 0)
                    r = (uint)-konami.d.wl;
                else
                    r = konami.d.wl;
                konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((0 ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    konami.cc |= (byte)((r & 0x10000) >> 16);
                };
                konami.d.wl = (ushort)r;
            }

            /* LSRD direct -0*-* */
            static void lsrd_di()
            {
                byte t;

                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl >>= 1;
                    if ((ushort)konami.d.wl == 0) konami.cc |= 0x04;
                }
            }

            /* RORD direct -**-* */
            static void rord_di()
            {
                ushort r;
                byte t;

                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    r = (ushort)((konami.cc & 0x01) << 15);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    r |= (ushort)(konami.d.wl >> 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* ASRD direct ?**-* */
            static void asrd_di()
            {
                byte t;

                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl = (ushort)((konami.d.wl & 0x8000) | (konami.d.wl >> 1));
                    { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                }
            }

            /* ASLD direct ?**** */
            static void asld_di()
            {
                uint r;
                byte t;

                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    r = (uint)konami.d.wl << 1;
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                    {
                        konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04; konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14);
                        konami.cc |= (byte)((r & 0x10000) >> 16);
                    };
                    konami.d.wl = (ushort)r;
                }
            }

            /* ROLD direct -**-* */
            static void rold_di()
            {
                ushort r;
                byte t;

                ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    if ((konami.d.wl & 0x8000) != 0)
                        konami.cc |= 0x01;
                    r = (ushort)(konami.cc & 0x01);
                    r |= (ushort)(konami.d.wl << 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* LSRD indexed -0*-* */
            static void lsrd_ix()
            {
                byte t;

                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl >>= 1;
                    if ((ushort)konami.d.wl == 0) konami.cc |= 0x04;
                }
            }

            /* RORD indexed -**-* */
            static void rord_ix()
            {
                ushort r;
                byte t;

                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                while (t-- != 0)
                {
                    r = (ushort)((konami.cc & 0x01) << 15);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    r |= (ushort)(konami.d.wl >> 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* ASRD indexed ?**-* */
            static void asrd_ix()
            {
                byte t;

                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl = (ushort)((konami.d.wl & 0x8000) | (konami.d.wl >> 1));
                    { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                }
            }

            /* ASLD indexed ?**** */
            static void asld_ix()
            {
                uint r;
                byte t;

                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                while (t-- != 0)
                {
                    r = (uint)konami.d.wl << 1;
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                    {
                        konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04;
                        konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14);
                        konami.cc |= (byte)((r & 0x10000) >> 16);
                    };
                    konami.d.wl = (ushort)r;
                }
            }

            /* ROLD indexed -**-* */
            static void rold_ix()
            {
                ushort r;
                byte t;

                t = (byte)((uint)cpu_readmem16((int)ea.wl));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    if ((konami.d.wl & 0x8000) != 0) konami.cc |= 0x01;
                    r = (ushort)(konami.cc & 0x01);
                    r |= (ushort)(konami.d.wl << 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* LSRD extended -0*-* */
            static void lsrd_ex()
            {
                byte t;

                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl >>= 1;
                    if ((ushort)konami.d.wl == 0) konami.cc |= 0x04;
                }
            }

            /* RORD extended -**-* */
            static void rord_ex()
            {
                ushort r;
                byte t;

                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    r = (ushort)((konami.cc & 0x01) << 15);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    r |= (ushort)(konami.d.wl >> 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }

            /* ASRD extended ?**-* */
            static void asrd_ex()
            {
                byte t;

                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    konami.cc |= (byte)(konami.d.wl & 0x01);
                    konami.d.wl = (ushort)((konami.d.wl & 0x8000) | (konami.d.wl >> 1));
                    { konami.cc |= (byte)((konami.d.wl & 0x8000) >> 12); if (konami.d.wl == 0)konami.cc |= 0x04; };
                }
            }

            /* ASLD extended ?**** */
            static void asld_ex()
            {
                uint r;
                byte t;

                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    r = (byte)(konami.d.wl << 1);
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                    {
                        konami.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) konami.cc |= 0x04;
                        konami.cc |= (byte)(((konami.d.wl ^ konami.d.wl ^ r ^ (r >> 1)) & 0x8000) >> 14);
                        konami.cc |= (byte)((r & 0x10000) >> 16);
                    };
                    konami.d.wl = (ushort)r;
                }
            }

            /* ROLD extended -**-* */
            static void rold_ex()
            {
                ushort r;
                byte t;

                { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                t = (byte)((uint)cpu_readmem16((int)ea.d));

                while (t-- != 0)
                {
                    konami.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                    if ((konami.d.wl & 0x8000) != 0) konami.cc |= 0x01;
                    r = (byte)(konami.cc & 0x01);
                    r |= (byte)(konami.d.wl << 1);
                    { konami.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)konami.cc |= 0x04; };
                    konami.d.wl = r;
                }
            }
            static void opcode2()
            {
                byte ireg2 = (byte)((uint)cpu_readop_arg(konami.pc.d));
                konami.pc.wl++;

                switch (ireg2)
                {
                    //	case 0x00: EA=0; break; /* auto increment */
                    //	case 0x01: EA=0; break; /* double auto increment */
                    //	case 0x02: EA=0; break; /* auto decrement */
                    //	case 0x03: EA=0; break; /* double auto decrement */
                    //	case 0x04: EA=0; break; /* postbyte offs */
                    //	case 0x05: EA=0; break; /* postword offs */
                    //	case 0x06: EA=0; break; /* normal */
                    case 0x07:
                        ea.d = 0;
                        konami_extended[konami.ireg]();
                        konami_ICount[0] -= 2;
                        return;
                    //	case 0x08: EA=0; break; /* indirect - auto increment */
                    //	case 0x09: EA=0; break; /* indirect - double auto increment */
                    //	case 0x0a: EA=0; break; /* indirect - auto decrement */
                    //	case 0x0b: EA=0; break; /* indirect - double auto decrement */
                    //	case 0x0c: EA=0; break; /* indirect - postbyte offs */
                    //	case 0x0d: EA=0; break; /* indirect - postword offs */
                    //	case 0x0e: EA=0; break; /* indirect - normal */
                    case 0x0f: /* indirect - extended */
                        {
                            ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                            konami.pc.wl += 2;
                        };
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0x10: EA=0; break; /* auto increment */
                    //	case 0x11: EA=0; break; /* double auto increment */
                    //	case 0x12: EA=0; break; /* auto decrement */
                    //	case 0x13: EA=0; break; /* double auto decrement */
                    //	case 0x14: EA=0; break; /* postbyte offs */
                    //	case 0x15: EA=0; break; /* postword offs */
                    //	case 0x16: EA=0; break; /* normal */
                    //	case 0x17: EA=0; break; /* extended */
                    //	case 0x18: EA=0; break; /* indirect - auto increment */
                    //	case 0x19: EA=0; break; /* indirect - double auto increment */
                    //	case 0x1a: EA=0; break; /* indirect - auto decrement */
                    //	case 0x1b: EA=0; break; /* indirect - double auto decrement */
                    //	case 0x1c: EA=0; break; /* indirect - postbyte offs */
                    //	case 0x1d: EA=0; break; /* indirect - postword offs */
                    //	case 0x1e: EA=0; break; /* indirect - normal */
                    //	case 0x1f: EA=0; break; /* indirect - extended */

                    /* base X */
                    case 0x20: /* auto increment */
                        ea.wl = konami.x.wl;
                        konami.x.wl++;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x21: /* double auto increment */
                        ea.wl = konami.x.wl;
                        konami.x.wl += 2;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x22: /* auto decrement */
                        konami.x.wl--;
                        ea.wl = konami.x.wl;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x23: /* double auto decrement */
                        konami.x.wl -= 2;
                        ea.wl = konami.x.wl;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x24: /* postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(ea.wl));
                        konami_ICount[0] -= 2;
                        break;
                    case 0x25: /* postword offs */
                        {
                            ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1));
                            konami.pc.wl += 2;
                        };
                        ea.wl += konami.x.wl;
                        konami_ICount[0] -= 4;
                        break;
                    case 0x26: /* normal */
                        ea.wl = konami.x.wl;
                        break;
                    //	case 0x27: EA=0; break; /* extended */
                    case 0x28: /* indirect - auto increment */
                        ea.wl = konami.x.wl;
                        konami.x.wl++;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x29: /* indirect - double auto increment */
                        ea.wl = konami.x.wl;
                        konami.x.wl += 2;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x2a: /* indirect - auto decrement */
                        konami.x.wl--;
                        ea.wl = konami.x.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x2b: /* indirect - double auto decrement */
                        konami.x.wl -= 2;
                        ea.wl = konami.x.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x2c: /* indirect - postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(ea.wl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x2d: /* indirect - postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.x.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0x2e: /* indirect - normal */
                        ea.wl = konami.x.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 3;
                        break;
                    //	case 0x2f: EA=0; break; /* indirect - extended */

                    /* base Y */
                    case 0x30: /* auto increment */
                        ea.wl = konami.y.wl;
                        konami.y.wl++;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x31: /* double auto increment */
                        ea.wl = konami.y.wl;
                        konami.y.wl += 2;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x32: /* auto decrement */
                        konami.y.wl--;
                        ea.wl = konami.y.wl;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x33: /* double auto decrement */
                        konami.y.wl -= 2;
                        ea.wl = konami.y.wl;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x34: /* postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(ea.wl));
                        konami_ICount[0] -= 2;
                        break;
                    case 0x35: /* postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.y.wl;
                        konami_ICount[0] -= 4;
                        break;
                    case 0x36: /* normal */
                        ea.wl = konami.y.wl;
                        break;
                    //	case 0x37: EA=0; break; /* extended */
                    case 0x38: /* indirect - auto increment */
                        ea.wl = konami.y.wl;
                        konami.y.wl++;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x39: /* indirect - double auto increment */
                        ea.wl = konami.y.wl;
                        konami.y.wl += 2;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x3a: /* indirect - auto decrement */
                        konami.y.wl--;
                        ea.wl = konami.y.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x3b: /* indirect - double auto decrement */
                        konami.y.wl -= 2;
                        ea.wl = konami.y.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x3c: /* indirect - postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(ea.wl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x3d: /* indirect - postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.y.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0x3e: /* indirect - normal */
                        ea.wl = konami.y.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 3;
                        break;
                    //	case 0x3f: EA=0; break; /* indirect - extended */

                    //  case 0x40: EA=0; break; /* auto increment */
                    //	case 0x41: EA=0; break; /* double auto increment */
                    //	case 0x42: EA=0; break; /* auto decrement */
                    //	case 0x43: EA=0; break; /* double auto decrement */
                    //	case 0x44: EA=0; break; /* postbyte offs */
                    //	case 0x45: EA=0; break; /* postword offs */
                    //	case 0x46: EA=0; break; /* normal */
                    //	case 0x47: EA=0; break; /* extended */
                    //	case 0x48: EA=0; break; /* indirect - auto increment */
                    //	case 0x49: EA=0; break; /* indirect - double auto increment */
                    //	case 0x4a: EA=0; break; /* indirect - auto decrement */
                    //	case 0x4b: EA=0; break; /* indirect - double auto decrement */
                    //	case 0x4c: EA=0; break; /* indirect - postbyte offs */
                    //	case 0x4d: EA=0; break; /* indirect - postword offs */
                    //	case 0x4e: EA=0; break; /* indirect - normal */
                    //	case 0x4f: EA=0; break; /* indirect - extended */

                    /* base U */
                    case 0x50: /* auto increment */
                        ea.wl = konami.u.wl;
                        konami.u.wl++;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x51: /* double auto increment */
                        ea.wl = konami.u.wl;
                        konami.u.wl += 2;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x52: /* auto decrement */
                        konami.u.wl--;
                        ea.wl = konami.u.wl;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x53: /* double auto decrement */
                        konami.u.wl -= 2;
                        ea.wl = konami.u.wl;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x54: /* postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(ea.wl));
                        konami_ICount[0] -= 2;
                        break;
                    case 0x55: /* postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.u.wl;
                        konami_ICount[0] -= 4;
                        break;
                    case 0x56: /* normal */
                        ea.wl = konami.u.wl;
                        break;
                    //	case 0x57: EA=0; break; /* extended */
                    case 0x58: /* indirect - auto increment */
                        ea.wl = konami.u.wl;
                        konami.u.wl++;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x59: /* indirect - double auto increment */
                        ea.wl = konami.u.wl;
                        konami.u.wl += 2;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x5a: /* indirect - auto decrement */
                        konami.u.wl--;
                        ea.wl = konami.u.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x5b: /* indirect - double auto decrement */
                        konami.u.wl -= 2;
                        ea.wl = konami.u.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x5c: /* indirect - postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(ea.wl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x5d: /* indirect - postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.u.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0x5e: /* indirect - normal */
                        ea.wl = konami.u.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 3;
                        break;
                    //	case 0x5f: EA=0; break; /* indirect - extended */

                    /* base S */
                    case 0x60: /* auto increment */
                        ea.d = konami.s.d;
                        konami.s.wl++;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x61: /* double auto increment */
                        ea.d = konami.s.d;
                        konami.s.wl += 2;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x62: /* auto decrement */
                        konami.s.wl--;
                        ea.d = konami.s.d;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x63: /* double auto decrement */
                        konami.s.wl -= 2;
                        ea.d = konami.s.d;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x64: /* postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(ea.wl));
                        konami_ICount[0] -= 2;
                        break;
                    case 0x65: /* postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.s.wl;
                        konami_ICount[0] -= 4;
                        break;
                    case 0x66: /* normal */
                        ea.d = konami.s.d;
                        break;
                    //	case 0x67: EA=0; break; /* extended */
                    case 0x68: /* indirect - auto increment */
                        ea.d = konami.s.d;
                        konami.s.wl++;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x69: /* indirect - double auto increment */
                        ea.d = konami.s.d;
                        konami.s.wl += 2;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x6a: /* indirect - auto decrement */
                        konami.s.wl--;
                        ea.d = konami.s.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x6b: /* indirect - double auto decrement */
                        konami.s.wl -= 2;
                        ea.d = konami.s.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x6c: /* indirect - postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(ea.wl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x6d: /* indirect - postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += konami.s.wl;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0x6e: /* indirect - normal */
                        ea.d = konami.s.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 3;
                        break;
                    //	case 0x6f: EA=0; break; /* indirect - extended */

                    /* base PC */
                    case 0x70: /* auto increment */
                        ea.d = konami.pc.d;
                        konami.pc.wl++;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x71: /* double auto increment */
                        ea.d = konami.pc.d;
                        konami.pc.wl += 2;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x72: /* auto decrement */
                        konami.pc.wl--;
                        ea.d = konami.pc.d;
                        konami_ICount[0] -= 2;
                        break;
                    case 0x73: /* double auto decrement */
                        konami.pc.wl -= 2;
                        ea.d = konami.pc.d;
                        konami_ICount[0] -= 3;
                        break;
                    case 0x74: /* postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.pc.wl - 1 + (ushort)(short)(sbyte)(ea.wl));
                        konami_ICount[0] -= 2;
                        break;
                    case 0x75: /* postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += (ushort)(konami.pc.wl - 2);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x76: /* normal */
                        ea.d = konami.pc.d;
                        break;
                    //	case 0x77: EA=0; break; /* extended */
                    case 0x78: /* indirect - auto increment */
                        ea.d = konami.pc.d;
                        konami.pc.wl++;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x79: /* indirect - double auto increment */
                        ea.d = konami.pc.d;
                        konami.pc.wl += 2;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x7a: /* indirect - auto decrement */
                        konami.pc.wl--;
                        ea.d = konami.pc.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 5;
                        break;
                    case 0x7b: /* indirect - double auto decrement */
                        konami.pc.wl -= 2;
                        ea.d = konami.pc.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 6;
                        break;
                    case 0x7c: /* indirect - postbyte offs */
                        { ea.wl = (ushort)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; };
                        ea.wl = (ushort)(konami.pc.wl - 1 + (ushort)(short)(sbyte)(ea.wl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0x7d: /* indirect - postword offs */
                        { ea.d = (((uint)cpu_readop_arg(konami.pc.d)) << 8) | ((uint)cpu_readop_arg(konami.pc.d + 1)); konami.pc.wl += 2; };
                        ea.wl += (ushort)(konami.pc.wl - 2);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0x7e: /* indirect - normal */
                        ea.d = konami.pc.d;
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 3;
                        break;
                    //	case 0x7f: EA=0; break; /* indirect - extended */

                    //  case 0x80: EA=0; break; /* register a */
                    //	case 0x81: EA=0; break; /* register b */
                    //	case 0x82: EA=0; break; /* ???? */
                    //	case 0x83: EA=0; break; /* ???? */
                    //	case 0x84: EA=0; break; /* ???? */
                    //	case 0x85: EA=0; break; /* ???? */
                    //	case 0x86: EA=0; break; /* ???? */
                    //	case 0x87: EA=0; break; /* register d */
                    //	case 0x88: EA=0; break; /* indirect - register a */
                    //	case 0x89: EA=0; break; /* indirect - register b */
                    //	case 0x8a: EA=0; break; /* indirect - ???? */
                    //	case 0x8b: EA=0; break; /* indirect - ???? */
                    //	case 0x8c: EA=0; break; /* indirect - ???? */
                    //	case 0x8d: EA=0; break; /* indirect - ???? */
                    //	case 0x8e: EA=0; break; /* indirect - register d */
                    //	case 0x8f: EA=0; break; /* indirect - ???? */
                    //	case 0x90: EA=0; break; /* register a */
                    //	case 0x91: EA=0; break; /* register b */
                    //	case 0x92: EA=0; break; /* ???? */
                    //	case 0x93: EA=0; break; /* ???? */
                    //	case 0x94: EA=0; break; /* ???? */
                    //	case 0x95: EA=0; break; /* ???? */
                    //	case 0x96: EA=0; break; /* ???? */
                    //	case 0x97: EA=0; break; /* register d */
                    //	case 0x98: EA=0; break; /* indirect - register a */
                    //	case 0x99: EA=0; break; /* indirect - register b */
                    //	case 0x9a: EA=0; break; /* indirect - ???? */
                    //	case 0x9b: EA=0; break; /* indirect - ???? */
                    //	case 0x9c: EA=0; break; /* indirect - ???? */
                    //	case 0x9d: EA=0; break; /* indirect - ???? */
                    //	case 0x9e: EA=0; break; /* indirect - register d */
                    //	case 0x9f: EA=0; break; /* indirect - ???? */
                    case 0xa0: /* register a */
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        konami_ICount[0] -= 1;
                        break;
                    case 0xa1: /* register b */
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        konami_ICount[0] -= 1;
                        break;
                    //	case 0xa2: EA=0; break; /* ???? */
                    //	case 0xa3: EA=0; break; /* ???? */
                    //	case 0xa4: EA=0; break; /* ???? */
                    //	case 0xa5: EA=0; break; /* ???? */
                    //	case 0xa6: EA=0; break; /* ???? */
                    case 0xa7: /* register d */
                        ea.wl = (ushort)(konami.x.wl + konami.d.wl);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xa8: /* indirect - register a */
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xa9: /* indirect - register b */
                        ea.wl = (ushort)(konami.x.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xaa: EA=0; break; /* indirect - ???? */
                    //	case 0xab: EA=0; break; /* indirect - ???? */
                    //	case 0xac: EA=0; break; /* indirect - ???? */
                    //	case 0xad: EA=0; break; /* indirect - ???? */
                    //	case 0xae: EA=0; break; /* indirect - ???? */
                    case 0xaf: /* indirect - register d */
                        ea.wl = (ushort)(konami.x.wl + konami.d.wl);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0xb0: /* register a */
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        konami_ICount[0] -= 1;
                        break;
                    case 0xb1: /* register b */
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        konami_ICount[0] -= 1;
                        break;
                    //	case 0xb2: EA=0; break; /* ???? */
                    //	case 0xb3: EA=0; break; /* ???? */
                    //	case 0xb4: EA=0; break; /* ???? */
                    //	case 0xb5: EA=0; break; /* ???? */
                    //	case 0xb6: EA=0; break; /* ???? */
                    case 0xb7: /* register d */
                        ea.wl = (ushort)(konami.y.wl + konami.d.wl);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xb8: /* indirect - register a */
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xb9: /* indirect - register b */
                        ea.wl = (ushort)(konami.y.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xba: EA=0; break; /* indirect - ???? */
                    //	case 0xbb: EA=0; break; /* indirect - ???? */
                    //	case 0xbc: EA=0; break; /* indirect - ???? */
                    //	case 0xbd: EA=0; break; /* indirect - ???? */
                    //	case 0xbe: EA=0; break; /* indirect - ???? */
                    case 0xbf: /* indirect - register d */
                        ea.wl = (ushort)(konami.y.wl + konami.d.wl);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    //	case 0xc0: EA=0; break; /* register a */
                    //	case 0xc1: EA=0; break; /* register b */
                    //	case 0xc2: EA=0; break; /* ???? */
                    //	case 0xc3: EA=0; break; /* ???? */
                    case 0xc4:
                        ea.d = 0;
                        konami_direct[konami.ireg]();
                        konami_ICount[0] -= 1;
                        return;
                    //	case 0xc5: EA=0; break; /* ???? */
                    //	case 0xc6: EA=0; break; /* ???? */
                    //	case 0xc7: EA=0; break; /* register d */
                    //	case 0xc8: EA=0; break; /* indirect - register a */
                    //	case 0xc9: EA=0; break; /* indirect - register b */
                    //	case 0xca: EA=0; break; /* indirect - ???? */
                    //	case 0xcb: EA=0; break; /* indirect - ???? */
                    case 0xcc: /* indirect - direct */
                        ea.d = konami.dp.d; { ea.bl = (byte)((uint)cpu_readop_arg(konami.pc.d)); konami.pc.wl++; }; ea.d = RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xcd: EA=0; break; /* indirect - ???? */
                    //	case 0xce: EA=0; break; /* indirect - register d */
                    //	case 0xcf: EA=0; break; /* indirect - ???? */
                    case 0xd0: /* register a */
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        konami_ICount[0] -= 1;
                        break;
                    case 0xd1: /* register b */
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        konami_ICount[0] -= 1;
                        break;
                    //	case 0xd2: EA=0; break; /* ???? */
                    //	case 0xd3: EA=0; break; /* ???? */
                    //	case 0xd4: EA=0; break; /* ???? */
                    //	case 0xd5: EA=0; break; /* ???? */
                    //	case 0xd6: EA=0; break; /* ???? */
                    case 0xd7: /* register d */
                        ea.wl = (ushort)(konami.u.wl + konami.d.wl);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xd8: /* indirect - register a */
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xd9: /* indirect - register b */
                        ea.wl = (ushort)(konami.u.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xda: EA=0; break; /* indirect - ???? */
                    //	case 0xdb: EA=0; break; /* indirect - ???? */
                    //	case 0xdc: EA=0; break; /* indirect - ???? */
                    //	case 0xdd: EA=0; break; /* indirect - ???? */
                    //	case 0xde: EA=0; break; /* indirect - ???? */
                    case 0xdf: /* indirect - register d */
                        ea.wl = (ushort)(konami.u.wl + konami.d.wl);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0xe0: /* register a */
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        konami_ICount[0] -= 1;
                        break;
                    case 0xe1: /* register b */
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        konami_ICount[0] -= 1;
                        break;
                    //	case 0xe2: EA=0; break; /* ???? */
                    //	case 0xe3: EA=0; break; /* ???? */
                    //	case 0xe4: EA=0; break; /* ???? */
                    //	case 0xe5: EA=0; break; /* ???? */
                    //	case 0xe6: EA=0; break; /* ???? */
                    case 0xe7: /* register d */
                        ea.wl = (ushort)(konami.s.wl + konami.d.wl);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xe8: /* indirect - register a */
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xe9: /* indirect - register b */
                        ea.wl = (ushort)(konami.s.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xea: EA=0; break; /* indirect - ???? */
                    //	case 0xeb: EA=0; break; /* indirect - ???? */
                    //	case 0xec: EA=0; break; /* indirect - ???? */
                    //	case 0xed: EA=0; break; /* indirect - ???? */
                    //	case 0xee: EA=0; break; /* indirect - ???? */
                    case 0xef: /* indirect - register d */
                        ea.wl = (ushort)(konami.s.wl + konami.d.wl);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    case 0xf0: /* register a */
                        ea.wl = (ushort)(konami.pc.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        konami_ICount[0] -= 1;
                        break;
                    case 0xf1: /* register b */
                        ea.wl = (ushort)(konami.pc.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        konami_ICount[0] -= 1;
                        break;
                    //	case 0xf2: EA=0; break; /* ???? */
                    //	case 0xf3: EA=0; break; /* ???? */
                    //	case 0xf4: EA=0; break; /* ???? */
                    //	case 0xf5: EA=0; break; /* ???? */
                    //	case 0xf6: EA=0; break; /* ???? */
                    case 0xf7: /* register d */
                        ea.wl = (ushort)(konami.pc.wl + konami.d.wl);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xf8: /* indirect - register a */
                        ea.wl = (ushort)(konami.pc.wl + (ushort)(short)(sbyte)(konami.d.bh));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    case 0xf9: /* indirect - register b */
                        ea.wl = (ushort)(konami.pc.wl + (ushort)(short)(sbyte)(konami.d.bl));
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 4;
                        break;
                    //	case 0xfa: EA=0; break; /* indirect - ???? */
                    //	case 0xfb: EA=0; break; /* indirect - ???? */
                    //	case 0xfc: EA=0; break; /* indirect - ???? */
                    //	case 0xfd: EA=0; break; /* indirect - ???? */
                    //	case 0xfe: EA=0; break; /* indirect - ???? */
                    case 0xff: /* indirect - register d */
                        ea.wl = (ushort)(konami.pc.wl + konami.d.wl);
                        ea.wl = (ushort)RM16(ea.d);
                        konami_ICount[0] -= 7;
                        break;
                    default:
                        printf("KONAMI: Unknown/Invalid postbyte at PC = %04x\n", konami.pc.wl - 1);
                        break;
                        ea.d = 0;
                }
                konami_indexed[konami.ireg]();

            }
        }
    }
}