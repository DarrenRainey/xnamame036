using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_m6800 : cpu_interface
        {
            public const int M6800_INT_NONE = 0;
            public const int M6800_INT_IRQ = 1;
            public const int M6800_INT_NMI = 2;
            public const int M6800_WAI = 8;
            public const int M6800_SLP = 0x10;
            public const int M6800_IRQ_LINE = 0;
            public const int M6800_TIN_LINE = 1;

            public const int M6803_DDR1 = 0x00;
            public const int M6803_DDR2 = 0x01;
            public const int M6803_PORT1 = 0x100;
            public const int M6803_PORT2 = 0x101;

            const byte TCSR_OLVL = 0x01;
            const byte TCSR_IEDG = 0x02;
            const byte TCSR_ETOI = 0x04;
            const byte TCSR_EOCI = 0x08;
            const byte TCSR_EICI = 0x10;
            const byte TCSR_TOF = 0x20;
            const byte TCSR_OCF = 0x40;
            const byte TCSR_ICF = 0x80;


            public class m6800_Regs
            {
                public PAIR ppc;			/* Previous program counter */
                public PAIR pc; 			/* Program counter */
                public PAIR s;				/* Stack pointer */
                public PAIR x;				/* Index register */
                public PAIR d;				/* Accumulators */
                public byte cc; 			/* Condition codes */
                public byte wai_state;		/* WAI opcode state ,(or sleep opcode state) */
                public byte nmi_state;		/* NMI line state */
                public byte[] irq_state = new byte[2];	/* IRQ line state [IRQ1,TIN] */
                public byte ic_eddge;		/* InputCapture eddge , b.0=fall,b.1=raise */
                public irqcallback irq_callback;
                //int		(*irq_callback)(int irqline);
                public int extra_cycles;	/* cycles used for interrupts */
                public opcode[] insn;	/* instruction table */
                public byte[] cycles;			/* clock cycle of instruction table */
                /* internal registers */
                public byte port1_ddr;
                public byte port2_ddr;
                public byte port1_data;
                public byte port2_data;
                public byte tcsr;			/* Timer Control and Status Register */
                public byte pending_tcsr;	/* pending IRQ flag for clear IRQflag process */
                public byte irq2;			/* IRQ2 flags */
                public byte ram_ctrl;
                public PAIR counter;		/* free running counter */
                public PAIR output_compare;	/* output compare       */
                public ushort input_capture;	/* input capture        */

                public PAIR timer_over;
            }
            public static m6800_Regs m6800 = new m6800_Regs();
            public static uint timer_next;
            static PAIR ea = new PAIR();
            public static int[] m6800_ICount = new int[1];
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
0x0a,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
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
            static byte[] cycles_6800 =
{
  /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
 /*0*/ 0, 2, 0, 0, 0, 0, 2, 2, 4, 4, 2, 2, 2, 2, 2, 2,
 /*1*/ 2, 2, 0, 0, 0, 0, 2, 2, 0, 2, 0, 2, 0, 0, 0, 0,
 /*2*/ 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
 /*3*/ 4, 4, 4, 4, 4, 4, 4, 4, 0, 5, 0,10, 0, 0, 9,12,
 /*4*/ 2, 0, 0, 2, 2, 0, 2, 2, 2, 2, 2, 0, 2, 2, 0, 2,
 /*5*/ 2, 0, 0, 2, 2, 0, 2, 2, 2, 2, 2, 0, 2, 2, 0, 2,
 /*6*/ 7, 0, 0, 7, 7, 0, 7, 7, 7, 7, 7, 0, 7, 7, 4, 7,
 /*7*/ 6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 3, 6,
 /*8*/ 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 3, 8, 3, 0,
 /*9*/ 3, 3, 3, 0, 3, 3, 3, 4, 3, 3, 3, 3, 4, 0, 4, 5,
 /*A*/ 5, 5, 5, 0, 5, 5, 5, 6, 5, 5, 5, 5, 6, 8, 6, 7,
 /*B*/ 4, 4, 4, 0, 4, 4, 4, 5, 4, 4, 4, 4, 5, 9, 5, 6,
 /*C*/ 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 0, 3, 0,
 /*D*/ 3, 3, 3, 0, 3, 3, 3, 4, 3, 3, 3, 3, 0, 0, 4, 5,
 /*E*/ 5, 5, 5, 0, 5, 5, 5, 6, 5, 5, 5, 5, 0, 0, 6, 7,
 /*F*/ 4, 4, 4, 0, 4, 4, 4, 5, 4, 4, 4, 4, 0, 0, 5, 6
};
            public delegate void opcode();
            static opcode[] m6800_insn = 
            {
illegal,nop, illegal,illegal,illegal,illegal,tap, tpa,
inx, dex, clv, sev, clc, sec, cli, sei,
sba, cba, illegal,illegal,illegal,illegal,tab, tba,
illegal,daa, illegal,aba, illegal,illegal,illegal,illegal,
bra, brn, bhi, bls, bcc, bcs, bne, beq,
bvc, bvs, bpl, bmi, bge, blt, bgt, ble,
tsx, ins, pula, pulb, des, txs, psha, pshb,
illegal,rts, illegal,rti, illegal,illegal,wai, swi,
nega, illegal,illegal,coma, lsra, illegal,rora, asra,
asla, rola, deca, illegal,inca, tsta, illegal,clra,
negb, illegal,illegal,comb, lsrb, illegal,rorb, asrb,
aslb, rolb, decb, illegal,incb, tstb, illegal,clrb,
neg_ix, illegal,illegal,com_ix, lsr_ix, illegal,ror_ix, asr_ix,
asl_ix, rol_ix, dec_ix, illegal,inc_ix, tst_ix, jmp_ix, clr_ix,
neg_ex, illegal,illegal,com_ex, lsr_ex, illegal,ror_ex, asr_ex,
asl_ex, rol_ex, dec_ex, illegal,inc_ex, tst_ex, jmp_ex, clr_ex,
suba_im,cmpa_im,sbca_im,illegal,anda_im,bita_im,lda_im, sta_im,
eora_im,adca_im,ora_im, adda_im,cmpx_im,bsr, lds_im, sts_im,
suba_di,cmpa_di,sbca_di,illegal,anda_di,bita_di,lda_di, sta_di,
eora_di,adca_di,ora_di, adda_di,cmpx_di,jsr_di, lds_di, sts_di,
suba_ix,cmpa_ix,sbca_ix,illegal,anda_ix,bita_ix,lda_ix, sta_ix,
eora_ix,adca_ix,ora_ix, adda_ix,cmpx_ix,jsr_ix, lds_ix, sts_ix,
suba_ex,cmpa_ex,sbca_ex,illegal,anda_ex,bita_ex,lda_ex, sta_ex,
eora_ex,adca_ex,ora_ex, adda_ex,cmpx_ex,jsr_ex, lds_ex, sts_ex,
subb_im,cmpb_im,sbcb_im,illegal,andb_im,bitb_im,ldb_im, stb_im,
eorb_im,adcb_im,orb_im, addb_im,illegal,illegal,ldx_im, stx_im,
subb_di,cmpb_di,sbcb_di,illegal,andb_di,bitb_di,ldb_di, stb_di,
eorb_di,adcb_di,orb_di, addb_di,illegal,illegal,ldx_di, stx_di,
subb_ix,cmpb_ix,sbcb_ix,illegal,andb_ix,bitb_ix,ldb_ix, stb_ix,
eorb_ix,adcb_ix,orb_ix, addb_ix,illegal,illegal,ldx_ix, stx_ix,
subb_ex,cmpb_ex,sbcb_ex,illegal,andb_ex,bitb_ex,ldb_ex, stb_ex,
eorb_ex,adcb_ex,orb_ex, addb_ex,illegal,illegal,ldx_ex, stx_ex
};
            public cpu_m6800()
            {
                cpu_num = CPU_M6800;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6800_INT_NONE;
                irq_int = M6800_INT_IRQ;
                nmi_int = M6800_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6800_ICount;
                icount[0] = 50000;
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
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6800";
                    case CPU_INFO_FAMILY: return "Motorola 6800";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "m6800.c";
                    case CPU_INFO_CREDITS: return "The MAME team.";
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
                throw new NotImplementedException();
            }
            public override int execute(int cycles)
            {
                byte ireg;
                m6800_ICount[0] = cycles;

                {
                    m6800.output_compare.wh -= m6800.counter.wh;
                    m6800.timer_over.wl -= m6800.counter.wh;
                    m6800.counter.wh = 0;
                    {
                        timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d;
                    };
                };
                {
                    m6800_ICount[0] -= m6800.extra_cycles;
                    m6800.counter.d += (uint)m6800.extra_cycles;
                    if (m6800.counter.d >= timer_next) check_timer_event();
                };
                m6800.extra_cycles = 0;

                if ((m6800.wai_state & 8) != 0)
                {
                    {
                        int cycles_to_eat;
                        cycles_to_eat = (int)(timer_next - m6800.counter.d);
                        if (cycles_to_eat > m6800_ICount[0]) cycles_to_eat = m6800_ICount[0];
                        if (cycles_to_eat > 0)
                        {
                            {
                                m6800_ICount[0] -= cycles_to_eat;
                                m6800.counter.d += (uint)cycles_to_eat;
                                if (m6800.counter.d >= timer_next) check_timer_event();
                            };
                        }
                    };
                    goto getout;
                }

                do
                {
                    m6800.ppc = m6800.pc;

                    ireg = (byte)((uint)cpu_readop(m6800.pc.d));
                    m6800.pc.wl++;

                    switch (ireg)
                    {
                        case 0x00: illegal(); break;
                        case 0x01: nop(); break;
                        case 0x02: illegal(); break;
                        case 0x03: illegal(); break;
                        case 0x04: illegal(); break;
                        case 0x05: illegal(); break;
                        case 0x06: tap(); break;
                        case 0x07: tpa(); break;
                        case 0x08: inx(); break;
                        case 0x09: dex(); break;
                        case 0x0A: m6800.cc &= 0xfd; break;
                        case 0x0B: m6800.cc |= 0x02; break;
                        case 0x0C: m6800.cc &= 0xfe; break;
                        case 0x0D: m6800.cc |= 0x01; break;
                        case 0x0E: cli(); break;
                        case 0x0F: sei(); break;
                        case 0x10: sba(); break;
                        case 0x11: cba(); break;
                        case 0x12: illegal(); break;
                        case 0x13: illegal(); break;
                        case 0x14: illegal(); break;
                        case 0x15: illegal(); break;
                        case 0x16: tab(); break;
                        case 0x17: tba(); break;
                        case 0x18: illegal(); break;
                        case 0x19: daa(); break;
                        case 0x1a: illegal(); break;
                        case 0x1b: aba(); break;
                        case 0x1c: illegal(); break;
                        case 0x1d: illegal(); break;
                        case 0x1e: illegal(); break;
                        case 0x1f: illegal(); break;
                        case 0x20: bra(); break;
                        case 0x21: brn(); break;
                        case 0x22: bhi(); break;
                        case 0x23: bls(); break;
                        case 0x24: bcc(); break;
                        case 0x25: bcs(); break;
                        case 0x26: bne(); break;
                        case 0x27: beq(); break;
                        case 0x28: bvc(); break;
                        case 0x29: bvs(); break;
                        case 0x2a: bpl(); break;
                        case 0x2b: bmi(); break;
                        case 0x2c: bge(); break;
                        case 0x2d: blt(); break;
                        case 0x2e: bgt(); break;
                        case 0x2f: ble(); break;
                        case 0x30: tsx(); break;
                        case 0x31: ins(); break;
                        case 0x32: pula(); break;
                        case 0x33: pulb(); break;
                        case 0x34: des(); break;
                        case 0x35: txs(); break;
                        case 0x36: psha(); break;
                        case 0x37: pshb(); break;
                        case 0x38: illegal(); break;
                        case 0x39: rts(); break;
                        case 0x3a: illegal(); break;
                        case 0x3b: rti(); break;
                        case 0x3c: illegal(); break;
                        case 0x3d: illegal(); break;
                        case 0x3e: wai(); break;
                        case 0x3f: swi(); break;
                        case 0x40: nega(); break;
                        case 0x41: illegal(); break;
                        case 0x42: illegal(); break;
                        case 0x43: coma(); break;
                        case 0x44: lsra(); break;
                        case 0x45: illegal(); break;
                        case 0x46: rora(); break;
                        case 0x47: asra(); break;
                        case 0x48: asla(); break;
                        case 0x49: rola(); break;
                        case 0x4a: deca(); break;
                        case 0x4b: illegal(); break;
                        case 0x4c: inca(); break;
                        case 0x4d: tsta(); break;
                        case 0x4e: illegal(); break;
                        case 0x4f: clra(); break;
                        case 0x50: negb(); break;
                        case 0x51: illegal(); break;
                        case 0x52: illegal(); break;
                        case 0x53: comb(); break;
                        case 0x54: lsrb(); break;
                        case 0x55: illegal(); break;
                        case 0x56: rorb(); break;
                        case 0x57: asrb(); break;
                        case 0x58: aslb(); break;
                        case 0x59: rolb(); break;
                        case 0x5a: decb(); break;
                        case 0x5b: illegal(); break;
                        case 0x5c: incb(); break;
                        case 0x5d: tstb(); break;
                        case 0x5e: illegal(); break;
                        case 0x5f: clrb(); break;
                        case 0x60: neg_ix(); break;
                        case 0x61: illegal(); break;
                        case 0x62: illegal(); break;
                        case 0x63: com_ix(); break;
                        case 0x64: lsr_ix(); break;
                        case 0x65: illegal(); break;
                        case 0x66: ror_ix(); break;
                        case 0x67: asr_ix(); break;
                        case 0x68: asl_ix(); break;
                        case 0x69: rol_ix(); break;
                        case 0x6a: dec_ix(); break;
                        case 0x6b: illegal(); break;
                        case 0x6c: inc_ix(); break;
                        case 0x6d: tst_ix(); break;
                        case 0x6e: jmp_ix(); break;
                        case 0x6f: clr_ix(); break;
                        case 0x70: neg_ex(); break;
                        case 0x71: illegal(); break;
                        case 0x72: illegal(); break;
                        case 0x73: com_ex(); break;
                        case 0x74: lsr_ex(); break;
                        case 0x75: illegal(); break;
                        case 0x76: ror_ex(); break;
                        case 0x77: asr_ex(); break;
                        case 0x78: asl_ex(); break;
                        case 0x79: rol_ex(); break;
                        case 0x7a: dec_ex(); break;
                        case 0x7b: illegal(); break;
                        case 0x7c: inc_ex(); break;
                        case 0x7d: tst_ex(); break;
                        case 0x7e: jmp_ex(); break;
                        case 0x7f: clr_ex(); break;
                        case 0x80: suba_im(); break;
                        case 0x81: cmpa_im(); break;
                        case 0x82: sbca_im(); break;
                        case 0x83: illegal(); break;
                        case 0x84: anda_im(); break;
                        case 0x85: bita_im(); break;
                        case 0x86: lda_im(); break;
                        case 0x87: sta_im(); break;
                        case 0x88: eora_im(); break;
                        case 0x89: adca_im(); break;
                        case 0x8a: ora_im(); break;
                        case 0x8b: adda_im(); break;
                        case 0x8c: cmpx_im(); break;
                        case 0x8d: bsr(); break;
                        case 0x8e: lds_im(); break;
                        case 0x8f: sts_im(); /* orthogonality */ break;
                        case 0x90: suba_di(); break;
                        case 0x91: cmpa_di(); break;
                        case 0x92: sbca_di(); break;
                        case 0x93: illegal(); break;
                        case 0x94: anda_di(); break;
                        case 0x95: bita_di(); break;
                        case 0x96: lda_di(); break;
                        case 0x97: sta_di(); break;
                        case 0x98: eora_di(); break;
                        case 0x99: adca_di(); break;
                        case 0x9a: ora_di(); break;
                        case 0x9b: adda_di(); break;
                        case 0x9c: cmpx_di(); break;
                        case 0x9d: jsr_di(); break;
                        case 0x9e: lds_di(); break;
                        case 0x9f: sts_di(); break;
                        case 0xa0: suba_ix(); break;
                        case 0xa1: cmpa_ix(); break;
                        case 0xa2: sbca_ix(); break;
                        case 0xa3: illegal(); break;
                        case 0xa4: anda_ix(); break;
                        case 0xa5: bita_ix(); break;
                        case 0xa6: lda_ix(); break;
                        case 0xa7: sta_ix(); break;
                        case 0xa8: eora_ix(); break;
                        case 0xa9: adca_ix(); break;
                        case 0xaa: ora_ix(); break;
                        case 0xab: adda_ix(); break;
                        case 0xac: cmpx_ix(); break;
                        case 0xad: jsr_ix(); break;
                        case 0xae: lds_ix(); break;
                        case 0xaf: sts_ix(); break;
                        case 0xb0: suba_ex(); break;
                        case 0xb1: cmpa_ex(); break;
                        case 0xb2: sbca_ex(); break;
                        case 0xb3: illegal(); break;
                        case 0xb4: anda_ex(); break;
                        case 0xb5: bita_ex(); break;
                        case 0xb6: lda_ex(); break;
                        case 0xb7: sta_ex(); break;
                        case 0xb8: eora_ex(); break;
                        case 0xb9: adca_ex(); break;
                        case 0xba: ora_ex(); break;
                        case 0xbb: adda_ex(); break;
                        case 0xbc: cmpx_ex(); break;
                        case 0xbd: jsr_ex(); break;
                        case 0xbe: lds_ex(); break;
                        case 0xbf: sts_ex(); break;
                        case 0xc0: subb_im(); break;
                        case 0xc1: cmpb_im(); break;
                        case 0xc2: sbcb_im(); break;
                        case 0xc3: illegal(); break;
                        case 0xc4: andb_im(); break;
                        case 0xc5: bitb_im(); break;
                        case 0xc6: ldb_im(); break;
                        case 0xc7: stb_im(); break;
                        case 0xc8: eorb_im(); break;
                        case 0xc9: adcb_im(); break;
                        case 0xca: orb_im(); break;
                        case 0xcb: addb_im(); break;
                        case 0xcc: illegal(); break;
                        case 0xcd: illegal(); break;
                        case 0xce: ldx_im(); break;
                        case 0xcf: stx_im(); break;
                        case 0xd0: subb_di(); break;
                        case 0xd1: cmpb_di(); break;
                        case 0xd2: sbcb_di(); break;
                        case 0xd3: illegal(); break;
                        case 0xd4: andb_di(); break;
                        case 0xd5: bitb_di(); break;
                        case 0xd6: ldb_di(); break;
                        case 0xd7: stb_di(); break;
                        case 0xd8: eorb_di(); break;
                        case 0xd9: adcb_di(); break;
                        case 0xda: orb_di(); break;
                        case 0xdb: addb_di(); break;
                        case 0xdc: illegal(); break;
                        case 0xdd: illegal(); break;
                        case 0xde: ldx_di(); break;
                        case 0xdf: stx_di(); break;
                        case 0xe0: subb_ix(); break;
                        case 0xe1: cmpb_ix(); break;
                        case 0xe2: sbcb_ix(); break;
                        case 0xe3: illegal(); break;
                        case 0xe4: andb_ix(); break;
                        case 0xe5: bitb_ix(); break;
                        case 0xe6: ldb_ix(); break;
                        case 0xe7: stb_ix(); break;
                        case 0xe8: eorb_ix(); break;
                        case 0xe9: adcb_ix(); break;
                        case 0xea: orb_ix(); break;
                        case 0xeb: addb_ix(); break;
                        case 0xec: illegal(); break;
                        case 0xed: illegal(); break;
                        case 0xee: ldx_ix(); break;
                        case 0xef: stx_ix(); break;
                        case 0xf0: subb_ex(); break;
                        case 0xf1: cmpb_ex(); break;
                        case 0xf2: sbcb_ex(); break;
                        case 0xf3: illegal(); break;
                        case 0xf4: andb_ex(); break;
                        case 0xf5: bitb_ex(); break;
                        case 0xf6: ldb_ex(); break;
                        case 0xf7: stb_ex(); break;
                        case 0xf8: eorb_ex(); break;
                        case 0xf9: adcb_ex(); break;
                        case 0xfa: orb_ex(); break;
                        case 0xfb: addb_ex(); break;
                        case 0xfc: addx_ex(); break;
                        case 0xfd: illegal(); break;
                        case 0xfe: ldx_ex(); break;
                        case 0xff: stx_ex(); break;
                    }
                    {
                        m6800_ICount[0] -= cycles_6800[ireg];
                        m6800.counter.d += cycles_6800[ireg];
                        if (m6800.counter.d >= timer_next) check_timer_event();
                    };
                } while (m6800_ICount[0] > 0);

            getout:
                {
                    m6800_ICount[0] -= m6800.extra_cycles;
                    m6800.counter.d += (uint)m6800.extra_cycles;
                    if (m6800.counter.d >= timer_next) check_timer_event();
                };
                m6800.extra_cycles = 0;

                return cycles - m6800_ICount[0];
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new m6800_Regs();
            }
            public override uint get_pc()
            {
                return m6800.pc.wl;
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
                m6800.cc |= 0x10; /* IRQ disabled */
                m6800.pc.d = RM16(0xfffe);
                change_pc(m6800.pc.d);

                /* HJB 990417 set CPU subtype (other reset functions override this) */
                //	m6800.subtype   = SUBTYPE_M6800;
                m6800.insn = m6800_insn;
                m6800.cycles = cycles_6800;

                m6800.wai_state = 0;
                m6800.nmi_state = 0;
                m6800.irq_state[0] = 0;
                m6800.irq_state[1] = 0;
                m6800.ic_eddge = 0;

                m6800.port1_ddr = 0x00;
                m6800.port2_ddr = 0x00;
                /* TODO: on reset port 2 should be read to determine the operating mode (bits 0-2) */
                m6800.tcsr = 0x00;
                m6800.pending_tcsr = 0x00;
                m6800.irq2 = 0;
                m6800.counter.d = 0x0000;
                m6800.output_compare.d = 0xffff;
                m6800.timer_over.d = 0xffff;
                m6800.ram_ctrl |= 0x40;
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                m6800.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int linestate)
            {
                int eddge;

                if (m6800.irq_state[irqline] == linestate) return;
                ;
                m6800.irq_state[irqline] = (byte)linestate;

                switch (irqline)
                {
                    case 0:
                        if (linestate == CLEAR_LINE) return;
                        break;
                    case 1:
                        eddge = (linestate == CLEAR_LINE) ? 2 : 0;
                        if (((m6800.tcsr & 0x02) ^ (linestate == CLEAR_LINE ? 0x02 : 0)) == 0)
                            return;
                        /* active eddge in */
                        m6800.tcsr |= 0x80;
                        m6800.pending_tcsr |= 0x80;
                        m6800.input_capture = m6800.counter.wl;
                        { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        if ((m6800.cc & 0x10) == 0)
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6);
                                    if (m6800.irq_callback != null) m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4);
                                }
                                else if ((m6800.irq2 & 0x20) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2);
                                }
                            }
                        }
                        break;
                    default:
                        return;
                }
                {
                    if ((m6800.cc & 0x10) == 0)
                    {
                        if (m6800.irq_state[0] != CLEAR_LINE)
                        {
                            ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8);
                            if (m6800.irq_callback != null) m6800.irq_callback(0);
                        }
                        else
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6); if (m6800.irq_callback != null)
                                        m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0) { ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4); }
                                else if ((m6800.irq2 & 0x20) != 0) { ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2); }
                            }
                        };
                    }
                }; /* HJB 990417 */

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


            static uint RM16(uint Addr)
            {
                uint result = ((uint)cpu_readmem16((int)Addr)) << 8;
                return result | ((uint)cpu_readmem16((int)(Addr + 1) & 0xffff));
            }

            static void WM16(uint Addr, PAIR p)
            {
                cpu_writemem16((int)Addr, p.bh);
                cpu_writemem16((int)(Addr + 1) & 0xffff, p.bl);
            }

            /* IRQ enter */
            public static void ENTER_INTERRUPT(string message, ushort irq_vector)
            {
                ;
                if ((m6800.wai_state & (8 | 0x10)) != 0)
                {
                    if ((m6800.wai_state & 8) != 0)
                        m6800.extra_cycles += 4;
                    m6800.wai_state &= unchecked((byte)~(8 | 0x10));
                }
                else
                {
                    cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl; cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                    cpu_writemem16((int)m6800.s.d, m6800.x.bl); --m6800.s.wl; cpu_writemem16((int)m6800.s.d, m6800.x.bh); --m6800.s.wl;
                    cpu_writemem16((int)m6800.s.d, m6800.d.bh); --m6800.s.wl;
                    cpu_writemem16((int)m6800.s.d, m6800.d.bl); --m6800.s.wl;
                    cpu_writemem16((int)m6800.s.d, m6800.cc); --m6800.s.wl;
                    m6800.extra_cycles += 12;
                }
                m6800.cc |= 0x10;
                m6800.pc.d = RM16(irq_vector);
                change_pc(m6800.pc.d);
            }

            /* check OCI or TOI */
            public static void check_timer_event()
            {
                /* OCI */
                if (m6800.counter.d >= m6800.output_compare.d)
                {
                    m6800.output_compare.wh++; // next IRQ point
                    m6800.tcsr |= 0x40;
                    m6800.pending_tcsr |= 0x40;
                    m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20));
                    if ((m6800.cc & 0x10) == 0 && (m6800.tcsr & 0x08) != 0)
                        ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4);
                }
                /* TOI */
                if (m6800.counter.d >= m6800.timer_over.d)
                {
                    m6800.timer_over.wl++; // next IRQ point



                    m6800.tcsr |= 0x20;
                    m6800.pending_tcsr |= 0x20;
                    m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20));
                    if ((m6800.cc & 0x10) == 0 && (m6800.tcsr & 0x04) != 0)
                        ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2);
                }
                /* set next event */
                { timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d; };
            }
            public static void illegal()
            {
                Mame.printf("M6808: illegal opcode: address %04X, op %02X\n", m6800.pc.wl, (int)((uint)Mame.cpu_readop_arg(m6800.pc.wl)) & 0xFF);
            }
            public static void nop() { }
            public static void tap()
            {
                m6800.cc = m6800.d.bh;

                byte ireg; m6800.ppc = m6800.pc;
                ireg = cpu_readop(m6800.pc.d);
                m6800.pc.wl++; m6800.insn[ireg]();

                m6800_ICount[0] -= m6800.cycles[ireg];
                m6800.counter.d += m6800.cycles[ireg];
                if (m6800.counter.d >= timer_next) check_timer_event();

                if ((m6800.cc & 0x10) == 0)
                {
                    if (m6800.irq_state[0] != Mame.CLEAR_LINE)
                    {
                        ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8);
                        if (m6800.irq_callback != null) m6800.irq_callback(0);
                    }
                    else
                    {
                        if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                        {
                            if ((m6800.irq2 & 0x80) != 0)
                            {
                                ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6);
                                if (m6800.irq_callback != null)
                                    m6800.irq_callback(1);
                            }
                            else if ((m6800.irq2 & 0x40) != 0)
                            {
                                ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4);
                            }
                            else if ((m6800.irq2 & 0x20) != 0)
                            {
                                ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2);
                            }
                        }
                    }

                }
            } /* HJB 990417 */
            public static void tpa()
            {
                m6800.d.bh = m6800.cc;
            }
            public static void inx()
            {
                ++m6800.x.wl;
                m6800.cc &= 0xfb; if ((ushort)m6800.x.wl == 0) m6800.cc |= 0x04;
            }
            public static void dex()
            {
                --m6800.x.wl;
                m6800.cc &= 0xfb; if ((ushort)m6800.x.wl == 0) m6800.cc |= 0x04;
            }
            public static void clv()
            {
                m6800.cc &= 0xfd;
            }
            public static void sev()
            {
                m6800.cc |= 0x02;
            }
            public static void clc()
            {
                m6800.cc &= 0xfe;
            }
            public static void sec()
            {
                m6800.cc |= 0x01;
            }
            public static void cli()
            {
                m6800.cc &= unchecked((byte)~0x10);
                {
                    byte ireg; m6800.ppc = m6800.pc; ireg = cpu_readop(m6800.pc.d); m6800.pc.wl++;
                    m6800.insn[ireg]();
                    {
                        m6800_ICount[0] -= m6800.cycles[ireg]; m6800.counter.d += m6800.cycles[ireg];
                        if (m6800.counter.d >= timer_next) check_timer_event();
                    };
                };
                {
                    if ((m6800.cc & 0x10) == 0)
                    {
                        if (m6800.irq_state[0] != CLEAR_LINE)
                        {
                            ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8); if (m6800.irq_callback != null) m6800.irq_callback(0);
                        }
                        else
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6);
                                    if (m6800.irq_callback != null) m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4);
                                }
                                else if ((m6800.irq2 & 0x20) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2);
                                }
                            }
                        };
                    }
                }; /* HJB 990417 */
            }
            public static void sei()
            {
                m6800.cc |= 0x10;
                {
                    byte ireg; m6800.ppc = m6800.pc; ireg = (cpu_readop(m6800.pc.d)); m6800.pc.wl++; m6800.insn[ireg]();
                    {
                        m6800_ICount[0] -= m6800.cycles[ireg]; m6800.counter.d += m6800.cycles[ireg]; if (m6800.counter.d >= timer_next)
                            check_timer_event();
                    };
                };
                {
                    if ((m6800.cc & 0x10) == 0)
                    {
                        if (m6800.irq_state[0] != CLEAR_LINE)
                        {
                            ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8); if (m6800.irq_callback != null) m6800.irq_callback(0);
                        }
                        else
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6);
                                    if (m6800.irq_callback != null) m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4);
                                }
                                else if ((m6800.irq2 & 0x20) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2);
                                }
                            }
                        };
                    }
                }; /* HJB 990417 */
            }
            public static void sba()
            {
                ushort t;
                t = (ushort)(m6800.d.bh - m6800.d.bl);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4);
                    if ((byte)t == 0) m6800.cc |= 0x04; m6800.cc |= (byte)(((m6800.d.bh ^ m6800.d.bl ^ t ^ (t >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((t & 0x100) >> 8);
                };
                m6800.d.bh = (byte)t;
            }
            public static void cba()
            {
                ushort t;
                t = (ushort)(m6800.d.bh - m6800.d.bl);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4);
                    if ((byte)t == 0) m6800.cc |= 0x04; m6800.cc |= (byte)(((m6800.d.bh ^ m6800.d.bl ^ t ^ (t >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((t & 0x100) >> 8);
                };
            }
            public static void tab()
            {
                m6800.d.bl = m6800.d.bh;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void tba()
            {
                m6800.d.bh = m6800.d.bl;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void daa()
            {
                byte msn, lsn;
                ushort t, cf = 0;
                msn = (byte)(m6800.d.bh & 0xf0);
                lsn = (byte)(m6800.d.bh & 0x0f);
                if (lsn > 0x09 || (m6800.cc & 0x20) != 0) cf |= 0x06;
                if (msn > 0x80 && lsn > 0x09) cf |= 0x60;
                if (msn > 0x90 || (m6800.cc & 0x01) != 0) cf |= 0x60;
                t = (ushort)(cf + m6800.d.bh);
                m6800.cc &= 0xf1; /* keep carry from previous operation */
                { m6800.cc |= (byte)(((byte)t & 0x80) >> 4); if ((byte)t == 0)m6800.cc |= 0x04; }; m6800.cc |= (byte)((t & 0x100) >> 8);
                m6800.d.bh = (byte)t;
            }
            public static void aba()
            {
                ushort t;
                t = (ushort)(m6800.d.bh + m6800.d.bl);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4);
                    if ((byte)t == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ m6800.d.bl ^ t ^ (t >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((t & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ m6800.d.bl ^ t) & 0x10) << 1);
                m6800.d.bh = (byte)t;
            }
            public static void bra()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                change_pc(m6800.pc.d);
                /* speed up busy loops */
                if (t == 0xfe)
                {
                    int cycles_to_eat; cycles_to_eat = (int)(timer_next - m6800.counter.d);
                    if (cycles_to_eat > m6800_ICount[0]) cycles_to_eat = m6800_ICount[0];
                    if (cycles_to_eat > 0)
                    {
                        {
                            m6800_ICount[0] -= cycles_to_eat;
                            m6800.counter.d += (uint)cycles_to_eat;
                            if (m6800.counter.d >= timer_next) check_timer_event();
                        };
                    }
                };
            }
            public static void brn()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
            }
            public static void bhi()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if ((m6800.cc & 0x05) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bls()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if ((m6800.cc & 0x05) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bcc()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x01) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)(((t & 0x80) != 0) ? t | 0xff00 : t)); change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bcs()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x01) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)(((t & 0x80) != 0) ? t | 0xff00 : t)); change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bne()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x04) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)(((t & 0x80) != 0) ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void beq()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x04) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)(((t & 0x80) != 0) ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bvc()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x02) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bvs()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x02) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bpl()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x08) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bmi()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; if ((m6800.cc & 0x08) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bge()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if (((m6800.cc & 0x08) ^ ((m6800.cc & 0x02) << 2)) == 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void blt()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if (((m6800.cc & 0x08) ^ ((m6800.cc & 0x02) << 2)) != 0)
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void bgt()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if (!(((m6800.cc & 0x08) ^ ((m6800.cc & 0x02) << 2)) != 0 || (m6800.cc & 0x04) != 0))
                    {
                        m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void ble()
            {
                byte t;
                {
                    t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    if (((m6800.cc & 0x08) ^ ((m6800.cc & 0x02) << 2)) != 0 || (m6800.cc & 0x04) != 0)
                    {
                        m6800.pc.wl += (ushort)(ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc(m6800.pc.d);
                    }
                };
            }
            public static void tsx()
            {
                m6800.x.wl = (ushort)(m6800.s.wl + 1);
            }
            public static void ins()
            {
                ++m6800.s.wl;
            }
            public static void pula()
            {
                m6800.s.wl++; m6800.d.bh = ((byte)cpu_readmem16((int)m6800.s.d));
            }
            public static void pulb()
            {
                m6800.s.wl++; m6800.d.bl = ((byte)cpu_readmem16((int)m6800.s.d));
            }
            public static void des()
            {
                --m6800.s.wl;
            }
            public static void txs()
            {
                m6800.s.wl = (ushort)(m6800.x.wl - 1);
            }
            public static void psha()
            {
                cpu_writemem16((int)m6800.s.d, m6800.d.bh); --m6800.s.wl;
            }
            public static void pshb()
            {
                cpu_writemem16((int)m6800.s.d, m6800.d.bl); --m6800.s.wl;
            }
            public static void pulx()
            {
                m6800.s.wl++; m6800.x.d = ((uint)cpu_readmem16((int)m6800.s.d)) << 8; m6800.s.wl++; m6800.x.d |= ((uint)cpu_readmem16((int)m6800.s.d));
            }
            public static void rts()
            {
                m6800.s.wl++; m6800.pc.d = ((uint)cpu_readmem16((int)m6800.s.d)) << 8; m6800.s.wl++; m6800.pc.d |= ((uint)cpu_readmem16((int)m6800.s.d));
                change_pc(m6800.pc.d);
            }
            public static void abx()
            {
                m6800.x.wl += m6800.d.bl;
            }
            public static void rti()
            {
                m6800.s.wl++; m6800.cc = ((byte)cpu_readmem16((int)m6800.s.d));
                m6800.s.wl++; m6800.d.bl = ((byte)cpu_readmem16((int)m6800.s.d));
                m6800.s.wl++; m6800.d.bh = ((byte)cpu_readmem16((int)m6800.s.d));
                m6800.s.wl++; m6800.x.d = ((uint)cpu_readmem16((int)m6800.s.d)) << 8; m6800.s.wl++; m6800.x.d |= ((uint)cpu_readmem16((int)m6800.s.d));
                m6800.s.wl++; m6800.pc.d = ((uint)cpu_readmem16((int)m6800.s.d)) << 8; m6800.s.wl++; m6800.pc.d |= ((uint)cpu_readmem16((int)m6800.s.d));
                change_pc(m6800.pc.d);
                {
                    if ((m6800.cc & 0x10) == 0)
                    {
                        if (m6800.irq_state[0] != CLEAR_LINE)
                        {
                            ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8);
                            if (m6800.irq_callback != null) m6800.irq_callback(0);
                        }
                        else
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6);
                                    if (m6800.irq_callback != null) m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0) { ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4); }
                                else if ((m6800.irq2 & 0x20) != 0) { ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2); }
                            }
                        };
                    }
                }; /* HJB 990417 */
            }
            public static void pshx()
            {
                cpu_writemem16((int)m6800.s.d, m6800.x.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.x.bh); --m6800.s.wl;
            }
            public static void mul()
            {
                ushort t;
                t = (ushort)(m6800.d.bh * m6800.d.bl);
                m6800.cc &= 0xfe; if ((t & 0x80) != 0) m6800.cc |= 0x01;
                m6800.d.wl = t;
            }
            public static void wai()
            {
                /*

                    * WAI stacks the entire machine state on the

                    * hardware stack, then waits for an interrupt.

                    */
                m6800.wai_state |= 8;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl; cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.x.bl); --m6800.s.wl; cpu_writemem16((int)m6800.s.d, m6800.x.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.d.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.d.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.cc); --m6800.s.wl;
                {
                    if ((m6800.cc & 0x10) == 0)
                    {
                        if (m6800.irq_state[0] != CLEAR_LINE)
                        {
                            ENTER_INTERRUPT("M6800#%d take IRQ1\n", 0xfff8); if (m6800.irq_callback != null) m6800.irq_callback(0);
                        }
                        else
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                {
                                    ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6); if (m6800.irq_callback != null)
                                        m6800.irq_callback(1);
                                }
                                else if ((m6800.irq2 & 0x40) != 0) { ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4); }
                                else if ((m6800.irq2 & 0x20) != 0) { ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2); }
                            }
                        };
                    }
                };
                if ((m6800.wai_state & 8) != 0)
                {
                    int cycles_to_eat;
                    cycles_to_eat = (int)(timer_next - m6800.counter.d);
                    if (cycles_to_eat > m6800_ICount[0]) cycles_to_eat = m6800_ICount[0];
                    if (cycles_to_eat > 0)
                    {
                        {
                            m6800_ICount[0] -= cycles_to_eat;
                            m6800.counter.d += (uint)cycles_to_eat; if (m6800.counter.d >= timer_next)
                                check_timer_event();
                        };
                    }
                };
            }
            public static void swi()
            {
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.x.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.x.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.d.bh); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.d.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.cc); --m6800.s.wl;
                m6800.cc |= 0x10;
                m6800.pc.d = RM16(0xfffa);
                change_pc(m6800.pc.d);
            }
            public static void nega()
            {
                ushort r;
                r = (ushort)-m6800.d.bh;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((0 ^ m6800.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void coma()
            {
                m6800.d.bh = (byte)~m6800.d.bh;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                }; m6800.cc |= 0x01;
            }
            public static void lsra()
            {
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bh & 0x01);
                m6800.d.bh >>= 1; if ((byte)m6800.d.bh == 0) m6800.cc |= 0x04;
            }
            public static void rora()
            {
                byte r;
                r = (byte)((m6800.cc & 0x01) << 7);
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bh & 0x01);
                r |= (byte)(m6800.d.bh >> 1);
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if (r == 0) m6800.cc |= 0x04;
                };
                m6800.d.bh = r;
            }
            public static void asra()
            {
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bh & 0x01);
                m6800.d.bh >>= 1; m6800.d.bh |= (byte)((m6800.d.bh & 0x40) << 1);
                { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void asla()
            {
                ushort r;
                r = (ushort)(m6800.d.bh << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ m6800.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void rola()
            {
                ushort t, r;
                t = m6800.d.bh; r = (ushort)(m6800.cc & 0x01);
                r |= (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void deca()
            {
                --m6800.d.bh;
                m6800.cc &= 0xf1; { m6800.cc |= flags8d[(m6800.d.bh) & 0xff]; };
            }
            public static void inca()
            {
                ++m6800.d.bh;
                m6800.cc &= 0xf1; { m6800.cc |= flags8i[(m6800.d.bh) & 0xff]; };
            }
            public static void tsta()
            {
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void clra()
            {
                m6800.d.bh = 0;
                m6800.cc &= 0xf0; m6800.cc |= 0x04;
            }
            public static void negb()
            {
                ushort r;
                r = (ushort)-m6800.d.bl;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((0 ^ m6800.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void comb()
            {
                m6800.d.bl = (byte)~m6800.d.bl;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4);
                    if (m6800.d.bl == 0) m6800.cc |= 0x04;
                }; m6800.cc |= 0x01;
            }
            public static void lsrb()
            {
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bl & 0x01);
                m6800.d.bl >>= 1; if ((byte)m6800.d.bl == 0) m6800.cc |= 0x04;
            }
            public static void rorb()
            {
                byte r;
                r = (byte)((m6800.cc & 0x01) << 7);
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bl & 0x01);
                r |= (byte)(m6800.d.bl >> 1);
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0) m6800.cc |= 0x04;
                };
                m6800.d.bl = r;
            }
            public static void asrb()
            {
                m6800.cc &= 0xf2; m6800.cc |= (byte)(m6800.d.bl & 0x01);
                m6800.d.bl >>= 1; m6800.d.bl |= (byte)((m6800.d.bl & 0x40) << 1);
                { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void aslb()
            {
                ushort r;
                r = (ushort)(m6800.d.bl << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ m6800.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void rolb()
            {
                ushort t, r;
                t = m6800.d.bl; r = (ushort)(m6800.cc & 0x01);
                r |= (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void decb()
            {
                --m6800.d.bl;
                m6800.cc &= 0xf1; { m6800.cc |= flags8d[(m6800.d.bl) & 0xff]; };
            }
            public static void incb()
            {
                ++m6800.d.bl;
                m6800.cc &= 0xf1; { m6800.cc |= flags8i[(m6800.d.bl) & 0xff]; };
            }
            public static void tstb()
            {
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4);
                    if (m6800.d.bl == 0) m6800.cc |= 0x04;
                };
            }
            public static void clrb()
            {
                m6800.d.bl = 0;
                m6800.cc &= 0xf0; m6800.cc |= 0x04;
            }
            public static void neg_ix()
            {
                ushort r, t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)-t;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void aim_ix()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; r = ((byte)cpu_readmem16((int)ea.d));
                };
                r &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void oim_ix()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; r = ((byte)cpu_readmem16((int)ea.d));
                };
                r |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void com_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                }; t = (byte)~t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4); if (t == 0)
                        m6800.cc |= 0x04;
                }; m6800.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }
            public static void lsr_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));

                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.cc &= 0xf2;
                m6800.cc |= (byte)(t & 0x01);
                t >>= 1; if ((byte)t == 0) m6800.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }
            public static void eim_ix()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    r = ((byte)cpu_readmem16((int)ea.d));
                };
                r ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void ror_ix()
            {
                byte t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                };
                r = (byte)((m6800.cc & 0x01) << 7);
                m6800.cc &= 0xf2; m6800.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void asr_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    };
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf2;
                m6800.cc |= (byte)(t & 0x01);
                t >>= 1; t |= (byte)((t & 0x40) << 1);
                { m6800.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void asl_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void rol_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.cc & 0x01); r |= (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void dec_ix()
            {
                byte t;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; --t;
                m6800.cc &= 0xf1; { m6800.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void tim_ix()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    r = ((byte)cpu_readmem16((int)ea.d));
                };
                r &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void inc_ix()
            {
                byte t;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; ++t;
                m6800.cc &= 0xf1; { m6800.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void tst_ix()
            {
                byte t;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4); if (t == 0) m6800.cc |= 0x04;
                };
            }
            public static void jmp_ix()
            {
                { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                m6800.pc.wl = ea.wl; change_pc(m6800.pc.d);
            }
            public static void clr_ix()
            {
                { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                cpu_writemem16((int)ea.d, 0);
                m6800.cc &= 0xf0; m6800.cc |= 0x04;
            }
            public static void neg_ex()
            {
                ushort r, t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)-t;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04; m6800.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void aim_di()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    r = ((byte)cpu_readmem16((int)ea.d));
                };
                r &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void oim_di()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    r = ((byte)cpu_readmem16((int)ea.d));
                };
                r |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void com_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; t = (byte)~t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6800.cc |= 0x04; };
                m6800.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }
            public static void lsr_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf2;
                m6800.cc |= (byte)(t & 0x01);
                t >>= 1;
                if ((byte)t == 0) m6800.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }
            public static void eim_di()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    r = ((byte)cpu_readmem16((int)ea.d));
                };
                r ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void ror_ex()
            {
                byte t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                };
                r = (byte)((m6800.cc & 0x01) << 7);
                m6800.cc &= 0xf2; m6800.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }
            public static void asr_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.cc &= 0xf2; m6800.cc |= (byte)(t & 0x01);
                t >>= 1; t |= (byte)((t & 0x40) << 1);
                { m6800.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6800.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void asl_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void rol_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.cc & 0x01); r |= (ushort)(t << 1);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }
            public static void dec_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; --t;
                m6800.cc &= 0xf1; { m6800.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void tim_di()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = ((byte)cpu_readmem16((int)ea.d)); };
                r &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void inc_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; ++t;
                m6800.cc &= 0xf1; { m6800.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }
            public static void tst_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((t & 0x80) >> 4);
                    if (t == 0) m6800.cc |= 0x04;
                };
            }
            public static void jmp_ex()
            {
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                m6800.pc.wl += 2; m6800.pc.wl = ea.wl; change_pc(m6800.pc.d); /* TS 971002 */
            }
            public static void clr_ex()
            {
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                m6800.pc.wl += 2; cpu_writemem16((int)ea.d, 0);
                m6800.cc &= 0xf0; m6800.cc |= 0x04;
            }
            public static void suba_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void cmpa_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbca_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bh - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void subd_im()
            {
                uint r, d;
                PAIR b;
                b.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                d = m6800.d.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void anda_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bh &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void bita_im()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (byte)(m6800.d.bh & t);
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void lda_im()
            {
                m6800.d.bh = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void sta_im()
            {
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
                ea.wl = m6800.pc.wl++; cpu_writemem16((int)ea.d, m6800.d.bh);
            }
            public static void eora_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bh ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void adca_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bh + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void ora_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bh |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void adda_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bh + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void cmpx_im()
            {
                uint r, d;
                PAIR b;
                b.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)m6800.cc |= 0x04; }; m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
            }
            public static void cpx_im()
            {
                uint r, d;
                PAIR b;
                b.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
            }
            public static void bsr()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl; cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                m6800.pc.wl += (ushort)((short)((t & 0x80) != 0 ? t | 0xff00 : t));
                change_pc(m6800.pc.d); /* TS 971002 */
            }
            public static void lds_im()
            {
                m6800.s.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
            }
            public static void sts_im()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
                { ea.wl = m6800.pc.wl; m6800.pc.wl += 2; };
                WM16(ea.d, m6800.s);
            }
            public static void suba_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void cmpa_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbca_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6800.d.bh - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void subd_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; b.d = RM16(ea.d); };
                d = m6800.d.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void anda_di()
            {
                byte t;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((byte)cpu_readmem16((int)ea.d)); }; m6800.d.bh &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void bita_di()
            {
                byte t, r;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; r = (byte)(m6800.d.bh & t);
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void lda_di()
            {
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    m6800.d.bh = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void sta_di()
            {
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                cpu_writemem16((int)ea.d, m6800.d.bh);
            }
            public static void eora_di()
            {
                byte t;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh ^= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void adca_di()
            {
                ushort t, r;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6800.d.bh + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void ora_di()
            {
                byte t;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh |= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void adda_di()
            {
                ushort t, r;
                {
                    ea.d = ((uint)cpu_readop_arg(m6800.pc.d));
                    m6800.pc.wl++;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6800.d.bh + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void cmpx_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; b.d = RM16(ea.d); };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)m6800.cc |= 0x04; };
                m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
            }
            public static void cpx_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; b.d = RM16(ea.d); };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
            }
            public static void jsr_di()
            {
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                m6800.pc.wl = ea.wl;
                change_pc(m6800.pc.d);
            }
            public static void lds_di()
            {
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.s.d = RM16(ea.d); };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
            }
            public static void sts_di()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                WM16(ea.d, m6800.s);
            }
            public static void suba_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void cmpa_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbca_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6800.d.bh - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void subd_ix()
            {
                uint r, d;
                PAIR b;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; b.d = RM16(ea.d);
                };
                d = m6800.d.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12);
                    if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void anda_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh &= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void bita_ix()
            {
                byte t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                };
                r = (byte)(m6800.d.bh & t);
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if (r == 0) m6800.cc |= 0x04;
                };
            }
            public static void lda_ix()
            {
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; m6800.d.bh = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void sta_ix()
            {
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
                {
                    ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                    m6800.pc.wl++;
                }; cpu_writemem16((int)ea.d, m6800.d.bh);
            }
            public static void eora_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh ^= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void adca_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6800.d.bh + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void ora_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh |= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void adda_ix()
            {
                ushort t, r;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6800.d.bh + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void cmpx_ix()
            {
                uint r, d;
                PAIR b;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; b.d = RM16(ea.d);
                };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12);
                    if (r == 0) m6800.cc |= 0x04;
                };
                m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
            }
            public static void cpx_ix()
            {
                uint r, d;
                PAIR b;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; b.d = RM16(ea.d);
                };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12);
                    if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
            }
            public static void jsr_ix()
            {
                {
                    ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                    m6800.pc.wl++;
                };
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                m6800.pc.wl = ea.wl;
                change_pc(m6800.pc.d);
            }
            public static void lds_ix()
            {
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; m6800.s.d = RM16(ea.d);
                };
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12);
                    if (m6800.s.wl == 0) m6800.cc |= 0x04;
                };
            }
            public static void sts_ix()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
                {
                    ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                    m6800.pc.wl++;
                };
                WM16(ea.d, m6800.s);
            }
            public static void suba_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;

            }
            public static void cmpa_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbca_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bh = (byte)r;
            }
            public static void subd_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6800.d.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12);
                    if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
                    m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void anda_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh &= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void bita_ex()
            {
                byte t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; r = (byte)(m6800.d.bh & t);
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if (r == 0) m6800.cc |= 0x04;
                };
            }
            public static void lda_ex()
            {
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    m6800.d.bh = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void sta_ex()
            {
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                m6800.pc.wl += 2; cpu_writemem16((int)ea.d, m6800.d.bh);
            }
            public static void eora_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh ^= t;
                m6800.cc &= 0xf1;
                {
                    m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4);
                    if (m6800.d.bh == 0) m6800.cc |= 0x04;
                };
            }
            public static void adca_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04; m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void ora_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bh |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bh & 0x80) >> 4); if (m6800.d.bh == 0)m6800.cc |= 0x04; };
            }
            public static void adda_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bh + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bh ^ t ^ r) & 0x10) << 1);
                m6800.d.bh = (byte)r;
            }
            public static void cmpx_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((r & 0x8000) >> 12); if (r == 0)m6800.cc |= 0x04; };
                m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);
            }
            public static void cpx_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6800.x.wl;
                r = d - b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
            }
            public static void jsr_ex()
            {
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bl); --m6800.s.wl;
                cpu_writemem16((int)m6800.s.d, m6800.pc.bh); --m6800.s.wl;
                m6800.pc.wl = ea.wl;
                change_pc(m6800.pc.d);
            }
            public static void lds_ex()
            {
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; m6800.s.d = RM16(ea.d);
                };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
            }
            public static void sts_ex()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.s.wl & 0x8000) >> 12); if (m6800.s.wl == 0)m6800.cc |= 0x04; };
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                WM16(ea.d, m6800.s);
            }
            public static void subb_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void cmpb_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbcb_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bl - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void addd_im()
            {
                uint r, d;
                PAIR b;
                b.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                d = m6800.d.wl;
                r = d + b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void andb_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bl &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void bitb_im()
            {
                byte t, r;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (byte)(m6800.d.bl & t);
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void ldb_im()
            {
                m6800.d.bl = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void stb_im()
            {
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
                ea.wl = m6800.pc.wl++; cpu_writemem16((int)ea.d, m6800.d.bl);
            }
            public static void eorb_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bl ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void adcb_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bl + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void orb_im()
            {
                byte t;
                t = ((byte)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bl |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void addb_im()
            {
                ushort t, r;
                t = ((ushort)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; r = (ushort)(m6800.d.bl + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void ldd_im()
            {
                m6800.d.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                m6800.pc.wl += 2;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
            }
            public static void std_im()
            {
                { ea.wl = m6800.pc.wl; m6800.pc.wl += 2; };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
                WM16(ea.d, m6800.d);
            }
            public static void ldx_im()
            {
                m6800.x.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                m6800.pc.wl += 2;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
            }
            public static void stx_im()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
                { ea.wl = m6800.pc.wl; m6800.pc.wl += 2; };
                WM16(ea.d, m6800.x);
            }
            public static void subb_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void cmpb_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); }; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbcb_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6800.d.bl - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void addd_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; b.d = RM16(ea.d); };
                d = m6800.d.wl;
                r = d + b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void andb_di()
            {
                byte t;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((byte)cpu_readmem16((int)ea.d)); }; m6800.d.bl &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void bitb_di()
            {
                byte t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((byte)cpu_readmem16((int)ea.d)); }; r = (byte)(m6800.d.bl & t);
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void ldb_di()
            {
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.bl = ((byte)cpu_readmem16((int)ea.d)); };
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void stb_di()
            {
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; cpu_writemem16((int)ea.d, m6800.d.bl);
            }
            public static void eorb_di()
            {
                byte t;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((byte)cpu_readmem16((int)ea.d)); }; m6800.d.bl ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void adcb_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6800.d.bl + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void orb_di()
            {
                byte t;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((byte)cpu_readmem16((int)ea.d)); }; m6800.d.bl |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void addb_di()
            {
                ushort t, r;
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; t = ((ushort)cpu_readmem16((int)ea.d)); }; r = (ushort)(m6800.d.bl + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void ldd_di()
            {
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.d.d = RM16(ea.d); };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
            }
            public static void std_di()
            {
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
                WM16(ea.d, m6800.d);
            }
            public static void ldx_di()
            {
                { ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++; m6800.x.d = RM16(ea.d); };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
            }
            public static void stx_di()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
                ea.d = ((uint)cpu_readop_arg(m6800.pc.d)); m6800.pc.wl++;
                WM16(ea.d, m6800.x);
            }
            public static void subb_ix()
            {
                ushort t, r;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void cmpb_ix()
            {
                ushort t, r;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbcb_ix()
            {
                ushort t, r;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void addd_ix()
            {
                uint r, d;
                PAIR b;
                { { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };b.d = RM16(ea.d); };
                d = m6800.d.wl;
                r = d + b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (byte)r;
            }
            public static void andb_ix()
            {
                byte t;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bl &= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void bitb_ix()
            {
                byte t, r;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; r = (byte)(m6800.d.bl & t);
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void ldb_ix()
            {
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    m6800.d.bl = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void stb_ix()
            {
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
                { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; }; cpu_writemem16((int)ea.d, m6800.d.bl);
            }
            public static void eorb_ix()
            {
                byte t;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bl ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void adcb_ix()
            {
                ushort t, r;
                {
                    { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void orb_ix()
            {
                byte t;
                {
                    {
                        ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d)));
                        m6800.pc.wl++;
                    }; t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.d.bl |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void addb_ix()
            {
                ushort t, r;
                { { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };t = ((ushort)cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6800.d.bl + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void ldd_ix()
            {
                { { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };m6800.d.d = RM16(ea.d); };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
            }
            public static void std_ix()
            {
                { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
                WM16(ea.d, m6800.d);
            }
            public static void ldx_ix()
            {
                { { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };m6800.x.d = RM16(ea.d); };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
            }
            public static void stx_ix()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
                { ea.wl = (ushort)(m6800.x.wl + (byte)((uint)cpu_readop_arg(m6800.pc.d))); m6800.pc.wl++; };
                WM16(ea.d, m6800.x);
            }
            public static void subb_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void cmpb_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t);
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                };
            }
            public static void sbcb_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl - t - (m6800.cc & 0x01));
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6800.cc |= (byte)((r & 0x100) >> 8);
                };
                m6800.d.bl = (byte)r;
            }
            public static void addd_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6800.d.wl;
                r = d + b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }
            public static void andb_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.d.bl &= t;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void bitb_ex()
            {
                byte t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                r = (byte)(m6800.d.bl & t);
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6800.cc |= 0x04; };
            }
            public static void ldb_ex()
            {
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    m6800.d.bl = ((byte)cpu_readmem16((int)ea.d));
                };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void stb_ex()
            {
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                cpu_writemem16((int)ea.d, m6800.d.bl);
            }
            public static void eorb_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bl ^= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void adcb_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl + t + (m6800.cc & 0x01));
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void orb_ex()
            {
                byte t;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((byte)cpu_readmem16((int)ea.d));
                }; m6800.d.bl |= t;
                m6800.cc &= 0xf1; { m6800.cc |= (byte)((m6800.d.bl & 0x80) >> 4); if (m6800.d.bl == 0)m6800.cc |= 0x04; };
            }
            public static void addb_ex()
            {
                ushort t, r;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                }; r = (ushort)(m6800.d.bl + t);
                m6800.cc &= 0xd0;
                {
                    m6800.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6800.cc |= (byte)((r & 0x100) >> 8);
                }; m6800.cc |= (byte)(((m6800.d.bl ^ t ^ r) & 0x10) << 1);
                m6800.d.bl = (byte)r;
            }
            public static void ldd_ex()
            {
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; m6800.d.d = RM16(ea.d);
                };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
            }
            public static void addx_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6800.x.wl;
                r = d + b.d;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14);

                    m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.x.wl = (ushort)r;
            }
            public static void std_ex()
            {
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.d.wl & 0x8000) >> 12); if (m6800.d.wl == 0)m6800.cc |= 0x04; };
                WM16(ea.d, m6800.d);
            }
            public static void ldx_ex()
            {
                {
                    ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff));
                    m6800.pc.wl += 2; m6800.x.d = RM16(ea.d);
                };
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
            }
            public static void stx_ex()
            {
                m6800.cc &= 0xf1;
                { m6800.cc |= (byte)((m6800.x.wl & 0x8000) >> 12); if (m6800.x.wl == 0)m6800.cc |= 0x04; };
                ea.d = (((uint)cpu_readop_arg(m6800.pc.d)) << 8) | ((uint)cpu_readop_arg((m6800.pc.d + 1) & 0xffff)); m6800.pc.wl += 2;
                WM16(ea.d, m6800.x);
            }
            public static void lsrd()
            {
                ushort t;
                m6800.cc &= 0xf2; t = m6800.d.wl; m6800.cc |= (byte)(t & 0x0001);
                t >>= 1; if ((ushort)t == 0) m6800.cc |= 0x04; m6800.d.wl = t;
            }
            public static void asld()
            {
                int r;
                ushort t;
                t = m6800.d.wl; r = t << 1;
                m6800.cc &= 0xf0;
                {
                    m6800.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6800.cc |= 0x04;
                    m6800.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x8000) >> 14); m6800.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6800.d.wl = (ushort)r;
            }

        }
        public class cpu_m6801 : cpu_m6800
        {

        }
        public class cpu_m6802 : cpu_m6800
        {
            public cpu_m6802()
            {
                cpu_num = CPU_M6802;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6800_INT_NONE;
                irq_int = M6800_INT_IRQ;
                nmi_int = M6800_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6800_ICount;
                icount[0] = 50000;
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6802";
                    case CPU_INFO_FAMILY: return "Motorola 6800";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "m6800.cs";
                    case CPU_INFO_CREDITS: return "The MAME team.";
                }
                throw new Exception();
            }
        }
        public class cpu_m6803 : cpu_m6800
        {
            static opcode[] m6803_insn =  {
illegal,nop, illegal,illegal,lsrd, asld, tap, tpa,
inx, dex, clv, sev, clc, sec, cli, sei,
sba, cba, illegal,illegal,illegal,illegal,tab, tba,
illegal,daa, illegal,aba, illegal,illegal,illegal,illegal,
bra, brn, bhi, bls, bcc, bcs, bne, beq,
bvc, bvs, bpl, bmi, bge, blt, bgt, ble,
tsx, ins, pula, pulb, des, txs, psha, pshb,
pulx, rts, abx, rti, pshx, mul, wai, swi,
nega, illegal,illegal,coma, lsra, illegal,rora, asra,
asla, rola, deca, illegal,inca, tsta, illegal,clra,
negb, illegal,illegal,comb, lsrb, illegal,rorb, asrb,
aslb, rolb, decb, illegal,incb, tstb, illegal,clrb,
neg_ix, illegal,illegal,com_ix, lsr_ix, illegal,ror_ix, asr_ix,
asl_ix, rol_ix, dec_ix, illegal,inc_ix, tst_ix, jmp_ix, clr_ix,
neg_ex, illegal,illegal,com_ex, lsr_ex, illegal,ror_ex, asr_ex,
asl_ex, rol_ex, dec_ex, illegal,inc_ex, tst_ex, jmp_ex, clr_ex,
suba_im,cmpa_im,sbca_im,subd_im,anda_im,bita_im,lda_im, sta_im,
eora_im,adca_im,ora_im, adda_im,cpx_im ,bsr, lds_im, sts_im,
suba_di,cmpa_di,sbca_di,subd_di,anda_di,bita_di,lda_di, sta_di,
eora_di,adca_di,ora_di, adda_di,cpx_di ,jsr_di, lds_di, sts_di,
suba_ix,cmpa_ix,sbca_ix,subd_ix,anda_ix,bita_ix,lda_ix, sta_ix,
eora_ix,adca_ix,ora_ix, adda_ix,cpx_ix ,jsr_ix, lds_ix, sts_ix,
suba_ex,cmpa_ex,sbca_ex,subd_ex,anda_ex,bita_ex,lda_ex, sta_ex,
eora_ex,adca_ex,ora_ex, adda_ex,cpx_ex ,jsr_ex, lds_ex, sts_ex,
subb_im,cmpb_im,sbcb_im,addd_im,andb_im,bitb_im,ldb_im, stb_im,
eorb_im,adcb_im,orb_im, addb_im,ldd_im, std_im, ldx_im, stx_im,
subb_di,cmpb_di,sbcb_di,addd_di,andb_di,bitb_di,ldb_di, stb_di,
eorb_di,adcb_di,orb_di, addb_di,ldd_di, std_di, ldx_di, stx_di,
subb_ix,cmpb_ix,sbcb_ix,addd_ix,andb_ix,bitb_ix,ldb_ix, stb_ix,
eorb_ix,adcb_ix,orb_ix, addb_ix,ldd_ix, std_ix, ldx_ix, stx_ix,
subb_ex,cmpb_ex,sbcb_ex,addd_ex,andb_ex,bitb_ex,ldb_ex, stb_ex,
eorb_ex,adcb_ex,orb_ex, addb_ex,ldd_ex, std_ex, ldx_ex, stx_ex
};
            public cpu_m6803()
            {
                cpu_num = CPU_M6803;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6800_INT_NONE;
                irq_int = M6800_INT_IRQ;
                nmi_int = M6800_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6800_ICount;
                icount[0] = 50000;
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6803";
                    case CPU_INFO_FAMILY: return "Motorola 6800";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "m6800.c";
                    case CPU_INFO_CREDITS: return "The MAME team.";
                }
                throw new Exception();
            }
            public override void reset(object param)
            {
                base.reset(param);
                m6800.insn = m6803_insn;
                m6800.cycles = cycles_6803;
            }
            public override int execute(int cycles)
            {
                byte ireg;
                m6800_ICount[0] = cycles;

                {
                    m6800.output_compare.wh -= m6800.counter.wh; m6800.timer_over.wl -= m6800.counter.wh;
                    m6800.counter.wh = 0;
                    {
                        timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d;
                    };
                };
                {
                    m6800_ICount[0] -= m6800.extra_cycles;
                    m6800.counter.d += (uint)m6800.extra_cycles;
                    if (m6800.counter.d >= timer_next)
                        check_timer_event();
                };
                m6800.extra_cycles = 0;

                if ((m6800.wai_state & 8) != 0)
                {
                    {
                        int cycles_to_eat; cycles_to_eat = (int)(timer_next - m6800.counter.d);
                        if (cycles_to_eat > m6800_ICount[0]) cycles_to_eat = m6800_ICount[0];
                        if (cycles_to_eat > 0)
                        {
                            {
                                m6800_ICount[0] -= cycles_to_eat;
                                m6800.counter.d += (uint)cycles_to_eat;
                                if (m6800.counter.d >= timer_next) check_timer_event();
                            };
                        }
                    };
                    goto getout;
                }

                do
                {

                    m6800.ppc = m6800.pc;

                    ireg = ((byte)cpu_readop(m6800.pc.d));
                    m6800.pc.wl++;

                    switch (ireg)
                    {
                        case 0x00: illegal(); break;
                        case 0x01: nop(); break;
                        case 0x02: illegal(); break;
                        case 0x03: illegal(); break;
                        case 0x04: lsrd(); /* 6803 only */; break;
                        case 0x05: asld(); /* 6803 only */; break;
                        case 0x06: tap(); break;
                        case 0x07: tpa(); break;
                        case 0x08: inx(); break;
                        case 0x09: dex(); break;
                        case 0x0A: m6800.cc &= 0xfd; break;
                        case 0x0B: m6800.cc |= 0x02; break;
                        case 0x0C: m6800.cc &= 0xfe; break;
                        case 0x0D: m6800.cc |= 0x01; break;
                        case 0x0E: cli(); break;
                        case 0x0F: sei(); break;
                        case 0x10: sba(); break;
                        case 0x11: cba(); break;
                        case 0x12: illegal(); break;
                        case 0x13: illegal(); break;
                        case 0x14: illegal(); break;
                        case 0x15: illegal(); break;
                        case 0x16: tab(); break;
                        case 0x17: tba(); break;
                        case 0x18: illegal(); break;
                        case 0x19: daa(); break;
                        case 0x1a: illegal(); break;
                        case 0x1b: aba(); break;
                        case 0x1c: illegal(); break;
                        case 0x1d: illegal(); break;
                        case 0x1e: illegal(); break;
                        case 0x1f: illegal(); break;
                        case 0x20: bra(); break;
                        case 0x21: brn(); break;
                        case 0x22: bhi(); break;
                        case 0x23: bls(); break;
                        case 0x24: bcc(); break;
                        case 0x25: bcs(); break;
                        case 0x26: bne(); break;
                        case 0x27: beq(); break;
                        case 0x28: bvc(); break;
                        case 0x29: bvs(); break;
                        case 0x2a: bpl(); break;
                        case 0x2b: bmi(); break;
                        case 0x2c: bge(); break;
                        case 0x2d: blt(); break;
                        case 0x2e: bgt(); break;
                        case 0x2f: ble(); break;
                        case 0x30: tsx(); break;
                        case 0x31: ins(); break;
                        case 0x32: pula(); break;
                        case 0x33: pulb(); break;
                        case 0x34: des(); break;
                        case 0x35: txs(); break;
                        case 0x36: psha(); break;
                        case 0x37: pshb(); break;
                        case 0x38: pulx(); /* 6803 only */ break;
                        case 0x39: rts(); break;
                        case 0x3a: abx(); /* 6803 only */ break;
                        case 0x3b: rti(); break;
                        case 0x3c: pshx(); /* 6803 only */ break;
                        case 0x3d: mul(); /* 6803 only */ break;
                        case 0x3e: wai(); break;
                        case 0x3f: swi(); break;
                        case 0x40: nega(); break;
                        case 0x41: illegal(); break;
                        case 0x42: illegal(); break;
                        case 0x43: coma(); break;
                        case 0x44: lsra(); break;
                        case 0x45: illegal(); break;
                        case 0x46: rora(); break;
                        case 0x47: asra(); break;
                        case 0x48: asla(); break;
                        case 0x49: rola(); break;
                        case 0x4a: deca(); break;
                        case 0x4b: illegal(); break;
                        case 0x4c: inca(); break;
                        case 0x4d: tsta(); break;
                        case 0x4e: illegal(); break;
                        case 0x4f: clra(); break;
                        case 0x50: negb(); break;
                        case 0x51: illegal(); break;
                        case 0x52: illegal(); break;
                        case 0x53: comb(); break;
                        case 0x54: lsrb(); break;
                        case 0x55: illegal(); break;
                        case 0x56: rorb(); break;
                        case 0x57: asrb(); break;
                        case 0x58: aslb(); break;
                        case 0x59: rolb(); break;
                        case 0x5a: decb(); break;
                        case 0x5b: illegal(); break;
                        case 0x5c: incb(); break;
                        case 0x5d: tstb(); break;
                        case 0x5e: illegal(); break;
                        case 0x5f: clrb(); break;
                        case 0x60: neg_ix(); break;
                        case 0x61: illegal(); break;
                        case 0x62: illegal(); break;
                        case 0x63: com_ix(); break;
                        case 0x64: lsr_ix(); break;
                        case 0x65: illegal(); break;
                        case 0x66: ror_ix(); break;
                        case 0x67: asr_ix(); break;
                        case 0x68: asl_ix(); break;
                        case 0x69: rol_ix(); break;
                        case 0x6a: dec_ix(); break;
                        case 0x6b: illegal(); break;
                        case 0x6c: inc_ix(); break;
                        case 0x6d: tst_ix(); break;
                        case 0x6e: jmp_ix(); break;
                        case 0x6f: clr_ix(); break;
                        case 0x70: neg_ex(); break;
                        case 0x71: illegal(); break;
                        case 0x72: illegal(); break;
                        case 0x73: com_ex(); break;
                        case 0x74: lsr_ex(); break;
                        case 0x75: illegal(); break;
                        case 0x76: ror_ex(); break;
                        case 0x77: asr_ex(); break;
                        case 0x78: asl_ex(); break;
                        case 0x79: rol_ex(); break;
                        case 0x7a: dec_ex(); break;
                        case 0x7b: illegal(); break;
                        case 0x7c: inc_ex(); break;
                        case 0x7d: tst_ex(); break;
                        case 0x7e: jmp_ex(); break;
                        case 0x7f: clr_ex(); break;
                        case 0x80: suba_im(); break;
                        case 0x81: cmpa_im(); break;
                        case 0x82: sbca_im(); break;
                        case 0x83: subd_im(); /* 6803 only */ break;
                        case 0x84: anda_im(); break;
                        case 0x85: bita_im(); break;
                        case 0x86: lda_im(); break;
                        case 0x87: sta_im(); break;
                        case 0x88: eora_im(); break;
                        case 0x89: adca_im(); break;
                        case 0x8a: ora_im(); break;
                        case 0x8b: adda_im(); break;
                        case 0x8c: cpx_im(); /* 6803 difference */ break;
                        case 0x8d: bsr(); break;
                        case 0x8e: lds_im(); break;
                        case 0x8f: sts_im(); /* orthogonality */ break;
                        case 0x90: suba_di(); break;
                        case 0x91: cmpa_di(); break;
                        case 0x92: sbca_di(); break;
                        case 0x93: subd_di(); /* 6803 only */ break;
                        case 0x94: anda_di(); break;
                        case 0x95: bita_di(); break;
                        case 0x96: lda_di(); break;
                        case 0x97: sta_di(); break;
                        case 0x98: eora_di(); break;
                        case 0x99: adca_di(); break;
                        case 0x9a: ora_di(); break;
                        case 0x9b: adda_di(); break;
                        case 0x9c: cpx_di(); /* 6803 difference */ break;
                        case 0x9d: jsr_di(); break;
                        case 0x9e: lds_di(); break;
                        case 0x9f: sts_di(); break;
                        case 0xa0: suba_ix(); break;
                        case 0xa1: cmpa_ix(); break;
                        case 0xa2: sbca_ix(); break;
                        case 0xa3: subd_ix(); /* 6803 only */ break;
                        case 0xa4: anda_ix(); break;
                        case 0xa5: bita_ix(); break;
                        case 0xa6: lda_ix(); break;
                        case 0xa7: sta_ix(); break;
                        case 0xa8: eora_ix(); break;
                        case 0xa9: adca_ix(); break;
                        case 0xaa: ora_ix(); break;
                        case 0xab: adda_ix(); break;
                        case 0xac: cpx_ix(); /* 6803 difference */ break;
                        case 0xad: jsr_ix(); break;
                        case 0xae: lds_ix(); break;
                        case 0xaf: sts_ix(); break;
                        case 0xb0: suba_ex(); break;
                        case 0xb1: cmpa_ex(); break;
                        case 0xb2: sbca_ex(); break;
                        case 0xb3: subd_ex(); /* 6803 only */ break;
                        case 0xb4: anda_ex(); break;
                        case 0xb5: bita_ex(); break;
                        case 0xb6: lda_ex(); break;
                        case 0xb7: sta_ex(); break;
                        case 0xb8: eora_ex(); break;
                        case 0xb9: adca_ex(); break;
                        case 0xba: ora_ex(); break;
                        case 0xbb: adda_ex(); break;
                        case 0xbc: cpx_ex(); /* 6803 difference */ break;
                        case 0xbd: jsr_ex(); break;
                        case 0xbe: lds_ex(); break;
                        case 0xbf: sts_ex(); break;
                        case 0xc0: subb_im(); break;
                        case 0xc1: cmpb_im(); break;
                        case 0xc2: sbcb_im(); break;
                        case 0xc3: addd_im(); /* 6803 only */ break;
                        case 0xc4: andb_im(); break;
                        case 0xc5: bitb_im(); break;
                        case 0xc6: ldb_im(); break;
                        case 0xc7: stb_im(); break;
                        case 0xc8: eorb_im(); break;
                        case 0xc9: adcb_im(); break;
                        case 0xca: orb_im(); break;
                        case 0xcb: addb_im(); break;
                        case 0xcc: ldd_im(); /* 6803 only */ break;
                        case 0xcd: std_im(); /* 6803 only -- orthogonality */ break;
                        case 0xce: ldx_im(); break;
                        case 0xcf: stx_im(); break;
                        case 0xd0: subb_di(); break;
                        case 0xd1: cmpb_di(); break;
                        case 0xd2: sbcb_di(); break;
                        case 0xd3: addd_di(); /* 6803 only */ break;
                        case 0xd4: andb_di(); break;
                        case 0xd5: bitb_di(); break;
                        case 0xd6: ldb_di(); break;
                        case 0xd7: stb_di(); break;
                        case 0xd8: eorb_di(); break;
                        case 0xd9: adcb_di(); break;
                        case 0xda: orb_di(); break;
                        case 0xdb: addb_di(); break;
                        case 0xdc: ldd_di(); /* 6803 only */ break;
                        case 0xdd: std_di(); /* 6803 only */ break;
                        case 0xde: ldx_di(); break;
                        case 0xdf: stx_di(); break;
                        case 0xe0: subb_ix(); break;
                        case 0xe1: cmpb_ix(); break;
                        case 0xe2: sbcb_ix(); break;
                        case 0xe3: addd_ix(); /* 6803 only */ break;
                        case 0xe4: andb_ix(); break;
                        case 0xe5: bitb_ix(); break;
                        case 0xe6: ldb_ix(); break;
                        case 0xe7: stb_ix(); break;
                        case 0xe8: eorb_ix(); break;
                        case 0xe9: adcb_ix(); break;
                        case 0xea: orb_ix(); break;
                        case 0xeb: addb_ix(); break;
                        case 0xec: ldd_ix(); /* 6803 only */ break;
                        case 0xed: std_ix(); /* 6803 only */ break;
                        case 0xee: ldx_ix(); break;
                        case 0xef: stx_ix(); break;
                        case 0xf0: subb_ex(); break;
                        case 0xf1: cmpb_ex(); break;
                        case 0xf2: sbcb_ex(); break;
                        case 0xf3: addd_ex(); /* 6803 only */ break;
                        case 0xf4: andb_ex(); break;
                        case 0xf5: bitb_ex(); break;
                        case 0xf6: ldb_ex(); break;
                        case 0xf7: stb_ex(); break;
                        case 0xf8: eorb_ex(); break;
                        case 0xf9: adcb_ex(); break;
                        case 0xfa: orb_ex(); break;
                        case 0xfb: addb_ex(); break;
                        case 0xfc: ldd_ex(); /* 6803 only */ break;
                        case 0xfd: std_ex(); /* 6803 only */ break;
                        case 0xfe: ldx_ex(); break;
                        case 0xff: stx_ex(); break;
                    }
                    {
                        m6800_ICount[0] -= cycles_6803[ireg]; m6800.counter.d += cycles_6803[ireg];
                        if (m6800.counter.d >= timer_next) check_timer_event();
                    };
                } while (m6800_ICount[0] > 0);

            getout:
                {
                    m6800_ICount[0] -= m6800.extra_cycles; m6800.counter.d += (uint)m6800.extra_cycles;
                    if (m6800.counter.d >= timer_next) check_timer_event();
                };
                m6800.extra_cycles = 0;

                return cycles - m6800_ICount[0];
            }
            static byte[] cycles_6803 =
{
  /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
 /*0*/ 0, 2, 0, 0, 3, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2,
 /*1*/ 2, 2, 0, 0, 0, 0, 2, 2, 0, 2, 0, 2, 0, 0, 0, 0,
 /*2*/ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
 /*3*/ 3, 3, 4, 4, 3, 3, 3, 3, 5, 5, 3,10, 4,10, 9,12,
 /*4*/ 2, 0, 0, 2, 2, 0, 2, 2, 2, 2, 2, 0, 2, 2, 0, 2,
 /*5*/ 2, 0, 0, 2, 2, 0, 2, 2, 2, 2, 2, 0, 2, 2, 0, 2,
 /*6*/ 6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 3, 6,
 /*7*/ 6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 3, 6,
 /*8*/ 2, 2, 2, 4, 2, 2, 2, 0, 2, 2, 2, 2, 4, 6, 3, 0,
 /*9*/ 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 4, 4,
 /*A*/ 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 6, 6, 5, 5,
 /*B*/ 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 6, 6, 5, 5,
 /*C*/ 2, 2, 2, 4, 2, 2, 2, 0, 2, 2, 2, 2, 3, 0, 3, 0,
 /*D*/ 3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
 /*E*/ 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
 /*F*/ 4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5
};

            public const int M6803_DDR1 = 0x00;
            public const int M6803_DDR2 = 0x01;
            public const int M6803_PORT1 = 0x100;
            public const int M6803_PORT2 = 0x101;

            public static int m6803_internal_registers_r(int offset)
            {
                switch (offset)
                {
                    case 0x00:
                        return m6800.port1_ddr;
                    case 0x01:
                        return m6800.port2_ddr;
                    case 0x02:
                        return (cpu_readport(0x100) & (m6800.port1_ddr ^ 0xff))
                          | (m6800.port1_data & m6800.port1_ddr);
                    case 0x03:
                        return (cpu_readport(0x101) & (m6800.port2_ddr ^ 0xff))
                          | (m6800.port2_data & m6800.port2_ddr);
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                        Mame.printf("CPU #%d PC %04x: warning - read from unsupported internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), offset);
                        return 0;
                    case 0x08:
                        m6800.pending_tcsr = 0;
                        //if (errorlog) fprintf(errorlog,"CPU #%d PC %04x: warning - read TCSR register\n",cpu_getactivecpu(),cpu_get_pc());
                        return m6800.tcsr;
                    case 0x09:
                        if ((m6800.pending_tcsr & 0x20) == 0)
                        {
                            m6800.tcsr &= unchecked((byte)~0x20);
                            { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        }
                        return m6800.counter.bh;
                    case 0x0a:
                        return m6800.counter.bl;
                    case 0x0b:
                        if ((m6800.pending_tcsr & 0x40) == 0)
                        {
                            m6800.tcsr &= unchecked((byte)~0x40);
                            { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        }
                        return m6800.output_compare.bh;
                    case 0x0c:
                        if ((m6800.pending_tcsr & 0x40) == 0)
                        {
                            m6800.tcsr &= unchecked((byte)~0x40);
                            { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        }
                        return m6800.output_compare.bl;
                    case 0x0d:
                        if ((m6800.pending_tcsr & 0x80) == 0)
                        {
                            m6800.tcsr &= unchecked((byte)~0x80);
                            { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        }
                        return (m6800.input_capture >> 0) & 0xff;
                    case 0x0e:
                        return (m6800.input_capture >> 8) & 0xff;
                    case 0x0f:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                        Mame.printf("CPU #%d PC %04x: warning - read from unsupported internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), offset);
                        return 0;
                    case 0x14:
                        Mame.printf("CPU #%d PC %04x: read RAM control register\n", cpu_getactivecpu(), cpu_get_pc());
                        return m6800.ram_ctrl;
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                    default:
                        Mame.printf("CPU #%d PC %04x: warning - read from reserved internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), offset);
                        return 0;
                }
            }

            static int latch09;
            public static void m6803_internal_registers_w(int offset, int data)
            {

                switch (offset)
                {
                    case 0x00:
                        if (m6800.port1_ddr != data)
                        {
                            m6800.port1_ddr = (byte)data;
                            cpu_writeport(0x100, (m6800.port1_data & m6800.port1_ddr)
                              | (0xff ^ m6800.port1_ddr));
                        }
                        break;
                    case 0x01:
                        if (m6800.port2_ddr != data)
                        {
                            m6800.port2_ddr = (byte)data;
                            cpu_writeport(0x101, (m6800.port2_data & m6800.port2_ddr)
                              | (0xff ^ m6800.port2_ddr));
                            Mame.printf("CPU #%d PC %04x: warning - port 2 bit 1 set as output (OLVL) - not supported\n", cpu_getactivecpu(), cpu_get_pc());
                        }
                        break;
                    case 0x02:
                        m6800.port1_data = (byte)data;
                        cpu_writeport(0x100, (m6800.port1_data & m6800.port1_ddr)
                          | (0xff ^ m6800.port1_ddr));
                        break;
                    case 0x03:
                        m6800.port2_data = (byte)data;
                        cpu_writeport(0x101, (m6800.port2_data & m6800.port2_ddr)
                          | (0xff ^ m6800.port2_ddr));
                        break;
                    case 0x04:
                    case 0x05:
                    case 0x06:
                    case 0x07:
                        Mame.printf("CPU #%d PC %04x: warning - write %02x to unsupported internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), data, offset);
                        break;
                    case 0x08:
                        m6800.tcsr = (byte)data;
                        m6800.pending_tcsr &= m6800.tcsr;
                        { m6800.irq2 = (byte)((m6800.tcsr & (m6800.tcsr << 3)) & (0x80 | 0x40 | 0x20)); };
                        if ((m6800.cc & 0x10) == 0)
                        {
                            if ((m6800.irq2 & (0x80 | 0x40 | 0x20)) != 0)
                            {
                                if ((m6800.irq2 & 0x80) != 0)
                                { ENTER_INTERRUPT("M6800#%d take ICI\n", 0xfff6); if (m6800.irq_callback != null) m6800.irq_callback(1); }
                                else if ((m6800.irq2 & 0x40) != 0) { ENTER_INTERRUPT("M6800#%d take OCI\n", 0xfff4); }
                                else if ((m6800.irq2 & 0x20) != 0)
                                { ENTER_INTERRUPT("M6800#%d take TOI\n", 0xfff2); }
                            }
                        };
                        //if (errorlog) fprintf(errorlog,"CPU #%d PC %04x: TCSR = %02x\n",cpu_getactivecpu(),cpu_get_pc(),data);
                        break;
                    case 0x09:
                        latch09 = data & 0xff; /* 6301 only */
                        m6800.counter.wl = 0xfff8;
                        m6800.timer_over.wl = m6800.counter.wh;
                        { m6800.output_compare.wh = (ushort)((m6800.output_compare.wl >= m6800.counter.wl) ? m6800.counter.wh : m6800.counter.wh + 1); { timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d; }; };
                        break;
                    case 0x0a: /* 6301 only */
                        m6800.counter.wl = (ushort)((latch09 << 8) | (data & 0xff));
                        m6800.timer_over.wl = m6800.counter.wh;
                        { m6800.output_compare.wh = (ushort)((m6800.output_compare.wl >= m6800.counter.wl) ? m6800.counter.wh : m6800.counter.wh + 1); { timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d; }; };
                        break;
                    case 0x0b:
                        if (m6800.output_compare.bh != data)
                        {
                            m6800.output_compare.bh = (byte)data;
                            { m6800.output_compare.wh = (ushort)((m6800.output_compare.wl >= m6800.counter.wl) ? m6800.counter.wh : m6800.counter.wh + 1); { timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d; }; };
                        }
                        break;
                    case 0x0c:
                        if (m6800.output_compare.bl != data)
                        {
                            m6800.output_compare.bl = (byte)data;
                            { m6800.output_compare.wh = (ushort)((m6800.output_compare.wl >= m6800.counter.wl) ? m6800.counter.wh : m6800.counter.wh + 1); { timer_next = (m6800.output_compare.d < m6800.timer_over.d) ? m6800.output_compare.d : m6800.timer_over.d; }; };
                        }
                        break;
                    case 0x0d:
                    case 0x0e:
                        Mame.printf("CPU #%d PC %04x: warning - write %02x to read only internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), data, offset);
                        break;
                    case 0x0f:
                    case 0x10:
                    case 0x11:
                    case 0x12:
                    case 0x13:
                        Mame.printf("CPU #%d PC %04x: warning - write %02x to unsupported internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), data, offset);
                        break;
                    case 0x14:
                        Mame.printf("CPU #%d PC %04x: write %02x to RAM control register\n", cpu_getactivecpu(), cpu_get_pc(), data);
                        m6800.ram_ctrl = (byte)data;
                        break;
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                    default:
                        Mame.printf("CPU #%d PC %04x: warning - write %02x to reserved internal register %02x\n", cpu_getactivecpu(), cpu_get_pc(), data, offset);
                        break;
                }
            }
        }
        public class cpu_m6808 : cpu_m6800
        {
            public cpu_m6808()
            {
                cpu_num = CPU_M6808;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6800_INT_NONE;
                irq_int = M6800_INT_IRQ;
                nmi_int = M6800_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6800_ICount;
                icount[0] = 50000;
            }
        }
        public class cpu_hd63701 : cpu_m6800
        {
            public const int HD63701_DDR1 = cpu_m6800.M6803_DDR1;
            public const int HD63701_DDR2 = cpu_m6800.M6803_DDR2;
            public const int HD63701_PORT1 = cpu_m6800.M6803_PORT1;
            public const int HD63701_PORT2 = cpu_m6800.M6803_PORT2;

            public static int hd63701_internal_registers_r(int offset)
            {
                return cpu_m6803.m6803_internal_registers_r(offset);
            }
            public static void hd63701_internal_registers_w(int offset, int data)
            {
                cpu_m6803.m6803_internal_registers_w(offset, data);
            }
            public cpu_hd63701()
            {
                cpu_num = CPU_HD63701;
                num_irqs = 1;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6800_INT_NONE;
                irq_int = M6800_INT_IRQ;
                nmi_int = M6800_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6800_ICount;
                icount[0] = 50000;
            }
            static byte[] cycles_63701 =
{
  /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
 /*0*/ 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
 /*1*/ 1, 1, 0, 0, 0, 0, 1, 1, 2, 2, 4, 1, 0, 0, 0, 0,
 /*2*/ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
 /*3*/ 1, 1, 3, 3, 1, 1, 4, 4, 4, 5, 1,10, 5, 7, 9,12,
 /*4*/ 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1,
 /*5*/ 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1,
 /*6*/ 6, 7, 7, 6, 6, 7, 6, 6, 6, 6, 6, 5, 6, 4, 3, 5,
 /*7*/ 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 4, 6, 4, 3, 5,
 /*8*/ 2, 2, 2, 3, 2, 2, 2, 0, 2, 2, 2, 2, 3, 5, 3, 0,
 /*9*/ 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 5, 4, 4,
 /*A*/ 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
 /*B*/ 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 6, 5, 5,
 /*C*/ 2, 2, 2, 3, 2, 2, 2, 0, 2, 2, 2, 2, 3, 0, 3, 0,
 /*D*/ 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
 /*E*/ 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
 /*F*/ 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5
};
        }
        public class nsc8105 : cpu_m6800
        {

            static byte[] cycles_nsc8105 =
{
  /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
 /*0*/ 0, 0, 2, 0, 0, 2, 0, 2, 4, 2, 4, 2, 2, 2, 2, 2,
 /*1*/ 2, 0, 2, 0, 0, 2, 0, 2, 0, 0, 2, 2, 0, 0, 0, 0,
 /*2*/ 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
 /*3*/ 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 5,10, 0, 9, 0,12,
 /*4*/ 2, 0, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 0, 2, 2,
 /*5*/ 2, 0, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 0, 2, 2,
 /*6*/ 7, 0, 0, 7, 7, 7, 0, 7, 7, 7, 7, 0, 7, 4, 7, 7,
 /*7*/ 6, 0, 0, 6, 6, 6, 0, 6, 6, 6, 6, 0, 6, 3, 6, 6,
 /*8*/ 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 3, 3, 8, 0,
 /*9*/ 3, 3, 3, 0, 3, 3, 3, 4, 3, 3, 3, 3, 4, 4, 0, 5,
 /*A*/ 5, 5, 5, 0, 5, 5, 5, 6, 5, 5, 5, 5, 6, 6, 8, 7,
 /*B*/ 4, 4, 4, 0, 4, 4, 4, 5, 4, 4, 4, 4, 5, 5, 9, 6,
 /*C*/ 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 0, 3, 0, 0,
 /*D*/ 3, 3, 3, 0, 3, 3, 3, 4, 3, 3, 3, 3, 0, 4, 0, 5,
 /*E*/ 5, 5, 5, 0, 5, 5, 5, 6, 5, 5, 5, 5, 0, 6, 0, 7,
 /*F*/ 4, 4, 4, 0, 4, 4, 4, 5, 4, 4, 4, 4, 4, 5, 0, 6
};
        }
    }
}