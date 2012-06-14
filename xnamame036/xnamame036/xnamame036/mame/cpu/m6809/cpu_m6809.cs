using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_hd6309 : cpu_m6809
        {
            public cpu_hd6309()
            {
                cpu_num = CPU_HD6309;
                num_irqs = 2;
                default_vector = 0;
                overclock = 1.0;
                no_int = HD6309_INT_NONE;
                irq_int = HD6309_INT_IRQ;
                nmi_int = HD6309_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6809_ICount;
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "HD6309";
                }
                return base.cpu_info(context, regnum);
            }
        }
        public class cpu_m6809 : cpu_interface
        {
            public const int M6809_INT_NONE = 0;
            public const int M6809_INT_IRQ = 1;
            public const int M6809_INT_FIRQ = 2;
            public const int M6809_INT_NMI = 4;
            public const int M6809_IRQ_LINE = 0;
            public const int M6809_FIRQ_LINE = 1;

            public const int HD6309_INT_NONE = M6809_INT_NONE;
            public const int HD6309_INT_IRQ = M6809_INT_IRQ;
            public const int HD6309_INT_FIRQ = M6809_INT_FIRQ;
            public const int HD6309_INT_NMI = M6809_INT_NMI;
            

            public cpu_m6809()
            {
                cpu_num = CPU_M6809;
                num_irqs = 2;
                default_vector = 0;
                overclock = 1.0;
                no_int = M6809_INT_NONE;
                irq_int = M6809_INT_IRQ;
                nmi_int = M6809_INT_NMI;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_BE;
                align_unit = 1;
                max_inst_len = 4;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = m6809_ICount;
            }
            public override void burn(int cycles)
            {
               //none
            }
            public override uint cpu_dasm(ref string buffer, uint pc)
            {
                throw new NotImplementedException();
            }
            public override string cpu_info(object context, int regnum)
            {
                switch (regnum)
                {
                    case CPU_INFO_NAME: return "M6809";
                    case CPU_INFO_FAMILY: return "Motorola 6809";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "cpu_m6809.cs";
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
                //none
            }
            public override int execute(int cycles)
            {
                m6809_ICount[0] = cycles - m6809.extra_cycles;
                m6809.extra_cycles = 0;

                if ((m6809.int_state & (8 | 16)) != 0)
                {
                    m6809_ICount[0] = 0;
                }
                else
                {
                    do
                    {
                        m6809.ppc = m6809.pc;

                        m6809.ireg = (byte)((uint)cpu_readop(m6809.pc.d));
                        m6809.pc.wl++;

                        switch (m6809.ireg)
                        {
                            case 0x00: neg_di(); m6809_ICount[0] -= 6; break;
                            case 0x01: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x02: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x03: com_di(); m6809_ICount[0] -= 6; break;
                            case 0x04: lsr_di(); m6809_ICount[0] -= 6; break;
                            case 0x05: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x06: ror_di(); m6809_ICount[0] -= 6; break;
                            case 0x07: asr_di(); m6809_ICount[0] -= 6; break;
                            case 0x08: asl_di(); m6809_ICount[0] -= 6; break;
                            case 0x09: rol_di(); m6809_ICount[0] -= 6; break;
                            case 0x0a: dec_di(); m6809_ICount[0] -= 6; break;
                            case 0x0b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x0c: inc_di(); m6809_ICount[0] -= 6; break;
                            case 0x0d: tst_di(); m6809_ICount[0] -= 6; break;
                            case 0x0e: jmp_di(); m6809_ICount[0] -= 3; break;
                            case 0x0f: clr_di(); m6809_ICount[0] -= 6; break;
                            case 0x10: pref10(); break;
                            case 0x11: pref11(); break;
                            case 0x12: nop(); m6809_ICount[0] -= 2; break;
                            case 0x13: sync(); m6809_ICount[0] -= 4; break;
                            case 0x14: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x15: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x16: lbra(); m6809_ICount[0] -= 5; break;
                            case 0x17: lbsr(); m6809_ICount[0] -= 9; break;
                            case 0x18: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x19: daa(); m6809_ICount[0] -= 2; break;
                            case 0x1a: orcc(); m6809_ICount[0] -= 3; break;
                            case 0x1b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x1c: andcc(); m6809_ICount[0] -= 3; break;
                            case 0x1d: sex(); m6809_ICount[0] -= 2; break;
                            case 0x1e: exg(); m6809_ICount[0] -= 8; break;
                            case 0x1f: tfr(); m6809_ICount[0] -= 6; break;
                            case 0x20: bra(); m6809_ICount[0] -= 3; break;
                            case 0x21: brn(); m6809_ICount[0] -= 3; break;
                            case 0x22: bhi(); m6809_ICount[0] -= 3; break;
                            case 0x23: bls(); m6809_ICount[0] -= 3; break;
                            case 0x24: bcc(); m6809_ICount[0] -= 3; break;
                            case 0x25: bcs(); m6809_ICount[0] -= 3; break;
                            case 0x26: bne(); m6809_ICount[0] -= 3; break;
                            case 0x27: beq(); m6809_ICount[0] -= 3; break;
                            case 0x28: bvc(); m6809_ICount[0] -= 3; break;
                            case 0x29: bvs(); m6809_ICount[0] -= 3; break;
                            case 0x2a: bpl(); m6809_ICount[0] -= 3; break;
                            case 0x2b: bmi(); m6809_ICount[0] -= 3; break;
                            case 0x2c: bge(); m6809_ICount[0] -= 3; break;
                            case 0x2d: blt(); m6809_ICount[0] -= 3; break;
                            case 0x2e: bgt(); m6809_ICount[0] -= 3; break;
                            case 0x2f: ble(); m6809_ICount[0] -= 3; break;
                            case 0x30: leax(); m6809_ICount[0] -= 4; break;
                            case 0x31: leay(); m6809_ICount[0] -= 4; break;
                            case 0x32: leas(); m6809_ICount[0] -= 4; break;
                            case 0x33: leau(); m6809_ICount[0] -= 4; break;
                            case 0x34: pshs(); m6809_ICount[0] -= 5; break;
                            case 0x35: puls(); m6809_ICount[0] -= 5; break;
                            case 0x36: pshu(); m6809_ICount[0] -= 5; break;
                            case 0x37: pulu(); m6809_ICount[0] -= 5; break;
                            case 0x38: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x39: rts(); m6809_ICount[0] -= 5; break;
                            case 0x3a: abx(); m6809_ICount[0] -= 3; break;
                            case 0x3b: rti(); m6809_ICount[0] -= 6; break;
                            case 0x3c: cwai(); m6809_ICount[0] -= 20; break;
                            case 0x3d: mul(); m6809_ICount[0] -= 11; break;
                            case 0x3e: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x3f: swi(); m6809_ICount[0] -= 19; break;
                            case 0x40: nega(); m6809_ICount[0] -= 2; break;
                            case 0x41: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x42: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x43: coma(); m6809_ICount[0] -= 2; break;
                            case 0x44: lsra(); m6809_ICount[0] -= 2; break;
                            case 0x45: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x46: rora(); m6809_ICount[0] -= 2; break;
                            case 0x47: asra(); m6809_ICount[0] -= 2; break;
                            case 0x48: asla(); m6809_ICount[0] -= 2; break;
                            case 0x49: rola(); m6809_ICount[0] -= 2; break;
                            case 0x4a: deca(); m6809_ICount[0] -= 2; break;
                            case 0x4b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x4c: inca(); m6809_ICount[0] -= 2; break;
                            case 0x4d: tsta(); m6809_ICount[0] -= 2; break;
                            case 0x4e: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x4f: clra(); m6809_ICount[0] -= 2; break;
                            case 0x50: negb(); m6809_ICount[0] -= 2; break;
                            case 0x51: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x52: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x53: comb(); m6809_ICount[0] -= 2; break;
                            case 0x54: lsrb(); m6809_ICount[0] -= 2; break;
                            case 0x55: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x56: rorb(); m6809_ICount[0] -= 2; break;
                            case 0x57: asrb(); m6809_ICount[0] -= 2; break;
                            case 0x58: aslb(); m6809_ICount[0] -= 2; break;
                            case 0x59: rolb(); m6809_ICount[0] -= 2; break;
                            case 0x5a: decb(); m6809_ICount[0] -= 2; break;
                            case 0x5b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x5c: incb(); m6809_ICount[0] -= 2; break;
                            case 0x5d: tstb(); m6809_ICount[0] -= 2; break;
                            case 0x5e: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x5f: clrb(); m6809_ICount[0] -= 2; break;
                            case 0x60: neg_ix(); m6809_ICount[0] -= 6; break;
                            case 0x61: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x62: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x63: com_ix(); m6809_ICount[0] -= 6; break;
                            case 0x64: lsr_ix(); m6809_ICount[0] -= 6; break;
                            case 0x65: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x66: ror_ix(); m6809_ICount[0] -= 6; break;
                            case 0x67: asr_ix(); m6809_ICount[0] -= 6; break;
                            case 0x68: asl_ix(); m6809_ICount[0] -= 6; break;
                            case 0x69: rol_ix(); m6809_ICount[0] -= 6; break;
                            case 0x6a: dec_ix(); m6809_ICount[0] -= 6; break;
                            case 0x6b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x6c: inc_ix(); m6809_ICount[0] -= 6; break;
                            case 0x6d: tst_ix(); m6809_ICount[0] -= 6; break;
                            case 0x6e: jmp_ix(); m6809_ICount[0] -= 3; break;
                            case 0x6f: clr_ix(); m6809_ICount[0] -= 6; break;
                            case 0x70: neg_ex(); m6809_ICount[0] -= 7; break;
                            case 0x71: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x72: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x73: com_ex(); m6809_ICount[0] -= 7; break;
                            case 0x74: lsr_ex(); m6809_ICount[0] -= 7; break;
                            case 0x75: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x76: ror_ex(); m6809_ICount[0] -= 7; break;
                            case 0x77: asr_ex(); m6809_ICount[0] -= 7; break;
                            case 0x78: asl_ex(); m6809_ICount[0] -= 7; break;
                            case 0x79: rol_ex(); m6809_ICount[0] -= 7; break;
                            case 0x7a: dec_ex(); m6809_ICount[0] -= 7; break;
                            case 0x7b: illegal(); m6809_ICount[0] -= 2; break;
                            case 0x7c: inc_ex(); m6809_ICount[0] -= 7; break;
                            case 0x7d: tst_ex(); m6809_ICount[0] -= 7; break;
                            case 0x7e: jmp_ex(); m6809_ICount[0] -= 4; break;
                            case 0x7f: clr_ex(); m6809_ICount[0] -= 7; break;
                            case 0x80: suba_im(); m6809_ICount[0] -= 2; break;
                            case 0x81: cmpa_im(); m6809_ICount[0] -= 2; break;
                            case 0x82: sbca_im(); m6809_ICount[0] -= 2; break;
                            case 0x83: subd_im(); m6809_ICount[0] -= 4; break;
                            case 0x84: anda_im(); m6809_ICount[0] -= 2; break;
                            case 0x85: bita_im(); m6809_ICount[0] -= 2; break;
                            case 0x86: lda_im(); m6809_ICount[0] -= 2; break;
                            case 0x87: sta_im(); m6809_ICount[0] -= 2; break;
                            case 0x88: eora_im(); m6809_ICount[0] -= 2; break;
                            case 0x89: adca_im(); m6809_ICount[0] -= 2; break;
                            case 0x8a: ora_im(); m6809_ICount[0] -= 2; break;
                            case 0x8b: adda_im(); m6809_ICount[0] -= 2; break;
                            case 0x8c: cmpx_im(); m6809_ICount[0] -= 4; break;
                            case 0x8d: bsr(); m6809_ICount[0] -= 7; break;
                            case 0x8e: ldx_im(); m6809_ICount[0] -= 3; break;
                            case 0x8f: stx_im(); m6809_ICount[0] -= 2; break;
                            case 0x90: suba_di(); m6809_ICount[0] -= 4; break;
                            case 0x91: cmpa_di(); m6809_ICount[0] -= 4; break;
                            case 0x92: sbca_di(); m6809_ICount[0] -= 4; break;
                            case 0x93: subd_di(); m6809_ICount[0] -= 6; break;
                            case 0x94: anda_di(); m6809_ICount[0] -= 4; break;
                            case 0x95: bita_di(); m6809_ICount[0] -= 4; break;
                            case 0x96: lda_di(); m6809_ICount[0] -= 4; break;
                            case 0x97: sta_di(); m6809_ICount[0] -= 4; break;
                            case 0x98: eora_di(); m6809_ICount[0] -= 4; break;
                            case 0x99: adca_di(); m6809_ICount[0] -= 4; break;
                            case 0x9a: ora_di(); m6809_ICount[0] -= 4; break;
                            case 0x9b: adda_di(); m6809_ICount[0] -= 4; break;
                            case 0x9c: cmpx_di(); m6809_ICount[0] -= 6; break;
                            case 0x9d: jsr_di(); m6809_ICount[0] -= 7; break;
                            case 0x9e: ldx_di(); m6809_ICount[0] -= 5; break;
                            case 0x9f: stx_di(); m6809_ICount[0] -= 5; break;
                            case 0xa0: suba_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa1: cmpa_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa2: sbca_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa3: subd_ix(); m6809_ICount[0] -= 6; break;
                            case 0xa4: anda_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa5: bita_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa6: lda_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa7: sta_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa8: eora_ix(); m6809_ICount[0] -= 4; break;
                            case 0xa9: adca_ix(); m6809_ICount[0] -= 4; break;
                            case 0xaa: ora_ix(); m6809_ICount[0] -= 4; break;
                            case 0xab: adda_ix(); m6809_ICount[0] -= 4; break;
                            case 0xac: cmpx_ix(); m6809_ICount[0] -= 6; break;
                            case 0xad: jsr_ix(); m6809_ICount[0] -= 7; break;
                            case 0xae: ldx_ix(); m6809_ICount[0] -= 5; break;
                            case 0xaf: stx_ix(); m6809_ICount[0] -= 5; break;
                            case 0xb0: suba_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb1: cmpa_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb2: sbca_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb3: subd_ex(); m6809_ICount[0] -= 7; break;
                            case 0xb4: anda_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb5: bita_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb6: lda_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb7: sta_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb8: eora_ex(); m6809_ICount[0] -= 5; break;
                            case 0xb9: adca_ex(); m6809_ICount[0] -= 5; break;
                            case 0xba: ora_ex(); m6809_ICount[0] -= 5; break;
                            case 0xbb: adda_ex(); m6809_ICount[0] -= 5; break;
                            case 0xbc: cmpx_ex(); m6809_ICount[0] -= 7; break;
                            case 0xbd: jsr_ex(); m6809_ICount[0] -= 8; break;
                            case 0xbe: ldx_ex(); m6809_ICount[0] -= 6; break;
                            case 0xbf: stx_ex(); m6809_ICount[0] -= 6; break;
                            case 0xc0: subb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc1: cmpb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc2: sbcb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc3: addd_im(); m6809_ICount[0] -= 4; break;
                            case 0xc4: andb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc5: bitb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc6: ldb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc7: stb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc8: eorb_im(); m6809_ICount[0] -= 2; break;
                            case 0xc9: adcb_im(); m6809_ICount[0] -= 2; break;
                            case 0xca: orb_im(); m6809_ICount[0] -= 2; break;
                            case 0xcb: addb_im(); m6809_ICount[0] -= 2; break;
                            case 0xcc: ldd_im(); m6809_ICount[0] -= 3; break;
                            case 0xcd: std_im(); m6809_ICount[0] -= 2; break;
                            case 0xce: ldu_im(); m6809_ICount[0] -= 3; break;
                            case 0xcf: stu_im(); m6809_ICount[0] -= 3; break;
                            case 0xd0: subb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd1: cmpb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd2: sbcb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd3: addd_di(); m6809_ICount[0] -= 6; break;
                            case 0xd4: andb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd5: bitb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd6: ldb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd7: stb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd8: eorb_di(); m6809_ICount[0] -= 4; break;
                            case 0xd9: adcb_di(); m6809_ICount[0] -= 4; break;
                            case 0xda: orb_di(); m6809_ICount[0] -= 4; break;
                            case 0xdb: addb_di(); m6809_ICount[0] -= 4; break;
                            case 0xdc: ldd_di(); m6809_ICount[0] -= 5; break;
                            case 0xdd: std_di(); m6809_ICount[0] -= 5; break;
                            case 0xde: ldu_di(); m6809_ICount[0] -= 5; break;
                            case 0xdf: stu_di(); m6809_ICount[0] -= 5; break;
                            case 0xe0: subb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe1: cmpb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe2: sbcb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe3: addd_ix(); m6809_ICount[0] -= 6; break;
                            case 0xe4: andb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe5: bitb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe6: ldb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe7: stb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe8: eorb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xe9: adcb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xea: orb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xeb: addb_ix(); m6809_ICount[0] -= 4; break;
                            case 0xec: ldd_ix(); m6809_ICount[0] -= 5; break;
                            case 0xed: std_ix(); m6809_ICount[0] -= 5; break;
                            case 0xee: ldu_ix(); m6809_ICount[0] -= 5; break;
                            case 0xef: stu_ix(); m6809_ICount[0] -= 5; break;
                            case 0xf0: subb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf1: cmpb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf2: sbcb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf3: addd_ex(); m6809_ICount[0] -= 7; break;
                            case 0xf4: andb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf5: bitb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf6: ldb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf7: stb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf8: eorb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xf9: adcb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xfa: orb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xfb: addb_ex(); m6809_ICount[0] -= 5; break;
                            case 0xfc: ldd_ex(); m6809_ICount[0] -= 6; break;
                            case 0xfd: std_ex(); m6809_ICount[0] -= 6; break;
                            case 0xfe: ldu_ex(); m6809_ICount[0] -= 6; break;
                            case 0xff: stu_ex(); m6809_ICount[0] -= 6; break;
                        }
                    } while (m6809_ICount[0] > 0);

                    m6809_ICount[0] -= m6809.extra_cycles;
                    m6809.extra_cycles = 0;
                }

                return cycles - m6809_ICount[0]; /* NS 970908 */
            }
            public override uint get_context(ref object reg)
            {
                reg = m6809;
                return 1;
            }
            public override void create_context(ref object reg)
            {
                reg = new m6809_Regs();
            }
            public override uint get_pc()
            {
                return m6809.pc.wl;
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
                m6809.int_state = 0;
                m6809.nmi_state = CLEAR_LINE;
                m6809.irq_state[0] = CLEAR_LINE;
                m6809.irq_state[0] = CLEAR_LINE;

                m6809.dp.d = 0; /* Reset direct page register */

                m6809.cc |= 0x10; /* IRQ disabled */
                m6809.cc |= 0x40; /* FIRQ disabled */

                m6809.pc.d = RM16(0xfffe);
                change_pc16(m6809.pc.d);
            }
            public override void set_context(object reg)
            {
                m6809 = (m6809_Regs)reg;
                change_pc16(m6809.pc.d);
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE)
                    m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    {
                        m6809.int_state &= unchecked((byte)~8);
                        m6809.extra_cycles += 7;
                    }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                        m6809.extra_cycles += 10;
                    }
                    m6809.cc |= 0x40 | 0x10;
                    m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d);
                    m6809.irq_callback(1);
                }
                else
                    if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                    {
                        if ((m6809.int_state & 8) != 0)
                        {
                            m6809.int_state &= unchecked((byte)~8);
                            m6809.extra_cycles += 7;
                        }
                        else
                        {
                            m6809.cc |= 0x80;
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                            --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                            m6809.extra_cycles += 19;
                        }
                        m6809.cc |= 0x10;
                        m6809.pc.d = RM16(0xfff8);
                        change_pc16(m6809.pc.d);
                        m6809.irq_callback(0);
                    };
            }
            public override void set_irq_callback(irqcallback callback)
            {
                m6809.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                m6809.irq_state[irqline] = (byte)state;
                if (state == CLEAR_LINE) return;
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE) m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    }
                    m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d);
                    m6809.irq_callback(1);
                }
                else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc |= 0x80; --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 19;
                    }
                    m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8); change_pc16(m6809.pc.d);
                    m6809.irq_callback(0);
                };
            }
            public override void set_nmi_line(int state)
            {
                if (m6809.nmi_state == state) return;
                m6809.nmi_state = (byte)state;
                ;
                if (state == Mame.CLEAR_LINE) return;

                /* if the stack was not yet initialized */
                if ((m6809.int_state & 32)==0) return;

                m6809.int_state &= unchecked((byte)~16);
                /* HJB 990225: state already saved by CWAI? */
                if ((m6809.int_state & 8)!=0)
                {
                    m6809.int_state &= unchecked((byte)~8);
                    m6809.extra_cycles += 7; /* subtract +7 cycles next time */
                }
                else
                {
                    m6809.cc |= 0x80; /* save entire state */
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                    m6809.extra_cycles += 19; /* subtract +19 cycles next time */
                }
                m6809.cc |= 0x40 | 0x10; /* inhibit FIRQ and IRQ */
                m6809.pc.d = RM16(0xfffc);
                change_pc16(m6809.pc.d);
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
            class m6809_Regs
            {
                public PAIR pc; 		/* Program counter */
                public PAIR ppc;		/* Previous program counter */
                public PAIR d;			/* Accumulator a and b */
                public PAIR dp; 		/* Direct Page register (page in MSB) */
                public PAIR u, s;		/* Stack pointers */
                public PAIR x, y;		/* Index registers */
                public byte cc;
                public byte ireg;		/* First opcode */
                public byte[] irq_state = new byte[2];
                public int extra_cycles; /* cycles used up by interrupts */
                public irqcallback irq_callback;
                public byte int_state;  /* SYNC and CWAI flags */
                public byte nmi_state;
            }
            public static int[] m6809_ICount = new int[1];


            /* macros to access memory */
            /* macros for CC -- CC bits affected should be reset before calling */
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

            static m6809_Regs m6809 = new m6809_Regs();
            static PAIR ea;
            static cpu_m6809()
            {
                m6809_ICount[0] = 50000;
            }
            uint RM16(uint Addr)
            {
                uint result = (uint)((cpu_readmem16((int)Addr)) << 8);
                return (uint)(result | (cpu_readmem16((int)(Addr + 1) & 0xffff)));
            }

            void WM16(uint Addr, PAIR p)
            {
                cpu_writemem16((int)Addr, p.bh);
                cpu_writemem16((int)(Addr + 1) & 0xffff, p.bl);
            }

            void illegal()
            {
                printf("M6809: illegal opcode at %04x\n", m6809.pc.wl);
            }
            /* $00 NEG direct ?**** */
            void neg_di()
            {
                ushort r, t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = ((ushort)cpu_readmem16((int)ea.d));
                };
                r = (ushort)-t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $01 ILLEGAL */

            /* $02 ILLEGAL */

            /* $03 COM direct -**01 */
            void com_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                t = (byte)~t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
                m6809.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $04 LSR direct -0*-* */
            void lsr_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t >>= 1;
                if ((byte)t == 0) m6809.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $05 ILLEGAL */

            /* $06 ROR direct -**-* */
            void ror_di()
            {
                byte t, r;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                r = (byte)((m6809.cc & 0x01) << 7);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1);
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $07 ASR direct ?**-* */
            void asr_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = ((byte)cpu_readmem16((int)ea.d));
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $08 ASL direct ?**** */
            void asl_di()
            {
                ushort t, r;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(t << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $09 ROL direct -**** */
            void rol_di()
            {
                ushort t, r;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)((m6809.cc & 0x01) | (t << 1));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $0A DEC direct -***- */
            void dec_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = (byte)(cpu_readmem16((int)ea.d));
                };
                --t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $0B ILLEGAL */

            /* $OC INC direct -***- */
            void inc_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = (byte)(cpu_readmem16((int)ea.d));
                };
                ++t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $OD TST direct -**0- */
            void tst_di()
            {
                byte t;
                {
                    ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
            }

            /* $0E JMP direct ----- */
            void jmp_di()
            {
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $0F CLR direct -0100 */
            void clr_di()
            {
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                cpu_writemem16((int)ea.d, 0);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                m6809.cc |= 0x04;
            }

            /* $10 FLAG */

            /* $11 FLAG */

            /* $12 NOP inherent ----- */
            void nop()
            {
                ;
            }

            /* $13 SYNC inherent ----- */
            void sync()
            {
                /* SYNC stops processing instructions until an interrupt request happens. */
                /* This doesn't require the corresponding interrupt to be enabled: if it */
                /* is disabled, execution continues with the next instruction. */
                m6809.int_state |= 16; /* HJB 990227 */
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE)
                    m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    }
                    m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d);
                    m6809.irq_callback(1);
                }
                else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc |= 0x80; --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 19;
                    }
                    m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8); change_pc16(m6809.pc.d);
                    m6809.irq_callback(0);
                };
                /* if M6809_SYNC has not been cleared by CHECK_IRQ_LINES,

                    * stop execution until the interrupt lines change. */
                if ((m6809.int_state & 16) != 0)
                    if (m6809_ICount[0] > 0) m6809_ICount[0] = 0;
            }

            /* $14 ILLEGAL */

            /* $15 ILLEGAL */

            /* $16 LBRA relative ----- */
            void lbra()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.pc.wl += ea.wl;
                change_pc16(m6809.pc.d);

                if (ea.wl == 0xfffd) /* EHC 980508 speed up busy loop */
                    if (m6809_ICount[0] > 0)
                        m6809_ICount[0] = 0;
            }

            /* $17 LBSR relative ----- */
            void lbsr()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                m6809.pc.wl += ea.wl;
                change_pc16(m6809.pc.d);
            }

            /* $18 ILLEGAL */


            /* $19 DAA inherent (A) -**0* */
            void daa()
            {
                byte msn, lsn;
                ushort t, cf = 0;
                msn = (byte)(m6809.d.bh & 0xf0);
                lsn = (byte)(m6809.d.bh & 0x0f);
                if (lsn > 0x09 || (m6809.cc & 0x20) != 0) cf |= 0x06;
                if (msn > 0x80 && lsn > 0x09) cf |= 0x60;
                if (msn > 0x90 || (m6809.cc & 0x01) != 0) cf |= 0x60;
                t = (ushort)(cf + m6809.d.bh);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); /* keep carry from previous operation */
                {
                    m6809.cc |= (byte)(((byte)t & 0x80) >> 4);
                    if ((byte)t == 0) m6809.cc |= 0x04;
                }; m6809.cc |= (byte)((t & 0x100) >> 8);
                m6809.d.bh = (byte)t;
            }
            /* $1A ORCC immediate ##### */
            void orcc()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.cc |= t;
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE) m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    {
                        m6809.int_state &= unchecked((byte)~8);
                        m6809.extra_cycles += 7;
                    }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    } m6809.cc |= 0x40 | 0x10;
                    m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d); m6809.irq_callback(1);
                }
                else
                    if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                    {
                        if ((m6809.int_state & 8) != 0)
                        {
                            m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7;
                        }
                        else
                        {
                            m6809.cc |= 0x80; --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                            m6809.extra_cycles += 19;
                        } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8); change_pc16(m6809.pc.d);
                        m6809.irq_callback(0);
                    }; /* HJB 990116 */
            }

            /* $1B ILLEGAL */

            /* $1C ANDCC immediate ##### */
            void andcc()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.cc &= t;
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE) m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    {
                        m6809.int_state &= unchecked((byte)~8);
                        m6809.extra_cycles += 7;
                    }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    } m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6);
                    change_pc16(m6809.pc.d); m6809.irq_callback(1);
                }
                else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc |= 0x80; --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                        m6809.extra_cycles += 19;
                    } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8); change_pc16(m6809.pc.d);
                    m6809.irq_callback(0);
                }; /* HJB 990116 */
            }

            /* $1D SEX inherent -**0- */
            void sex()
            {
                ushort t;
                t = ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl));
                m6809.d.wl = t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x8000) >> 12); if (t == 0)m6809.cc |= 0x04; };
            }

            /* $1E EXG inherent ----- */
            void exg()
            {
                ushort t1, t2;
                byte tb;

                tb = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if (((tb ^ (tb >> 4)) & 0x08) != 0) /* HJB 990225: mixed 8/16 bit case? */
                {
                    /* transfer $ff to both registers */
                    t1 = t2 = 0xff;
                }
                else
                {
                    switch (tb >> 4)
                    {
                        case 0: t1 = m6809.d.wl; break;
                        case 1: t1 = m6809.x.wl; break;
                        case 2: t1 = m6809.y.wl; break;
                        case 3: t1 = m6809.u.wl; break;
                        case 4: t1 = m6809.s.wl; break;
                        case 5: t1 = m6809.pc.wl; break;
                        case 8: t1 = m6809.d.bh; break;
                        case 9: t1 = m6809.d.bl; break;
                        case 10: t1 = m6809.cc; break;
                        case 11: t1 = m6809.dp.bh; break;
                        default: t1 = 0xff; break;
                    }
                    switch (tb & 15)
                    {
                        case 0: t2 = m6809.d.wl; break;
                        case 1: t2 = m6809.x.wl; break;
                        case 2: t2 = m6809.y.wl; break;
                        case 3: t2 = m6809.u.wl; break;
                        case 4: t2 = m6809.s.wl; break;
                        case 5: t2 = m6809.pc.wl; break;
                        case 8: t2 = m6809.d.bh; break;
                        case 9: t2 = m6809.d.bl; break;
                        case 10: t2 = m6809.cc; break;
                        case 11: t2 = m6809.dp.bh; break;
                        default: t2 = 0xff; break;
                    }
                }
                switch (tb >> 4)
                {
                    case 0: m6809.d.wl = t2; break;
                    case 1: m6809.x.wl = t2; break;
                    case 2: m6809.y.wl = t2; break;
                    case 3: m6809.u.wl = t2; break;
                    case 4: m6809.s.wl = t2; break;
                    case 5: m6809.pc.wl = t2; change_pc16(m6809.pc.d); break;
                    case 8: m6809.d.bh = (byte)t2; break;
                    case 9: m6809.d.bl = (byte)t2; break;
                    case 10: m6809.cc = (byte)t2; break;
                    case 11: m6809.dp.bh = (byte)t2; break;
                }
                switch (tb & 15)
                {
                    case 0: m6809.d.wl = t1; break;
                    case 1: m6809.x.wl = t1; break;
                    case 2: m6809.y.wl = t1; break;
                    case 3: m6809.u.wl = t1; break;
                    case 4: m6809.s.wl = t1; break;
                    case 5: m6809.pc.wl = t1; change_pc16(m6809.pc.d); break;
                    case 8: m6809.d.bh = (byte)t1; break;
                    case 9: m6809.d.bl = (byte)t1; break;
                    case 10: m6809.cc = (byte)t1; break;
                    case 11: m6809.dp.bh = (byte)t1; break;
                }
            }

            /* $1F TFR inherent ----- */
            void tfr()
            {
                byte tb;
                ushort t;

                tb = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if (((tb ^ (tb >> 4)) & 0x08) != 0) /* HJB 990225: mixed 8/16 bit case? */
                {
                    /* transfer $ff to register */
                    t = 0xff;
                }
                else
                {
                    switch (tb >> 4)
                    {
                        case 0: t = m6809.d.wl; break;
                        case 1: t = m6809.x.wl; break;
                        case 2: t = m6809.y.wl; break;
                        case 3: t = m6809.u.wl; break;
                        case 4: t = m6809.s.wl; break;
                        case 5: t = m6809.pc.wl; break;
                        case 8: t = m6809.d.bh; break;
                        case 9: t = m6809.d.bl; break;
                        case 10: t = m6809.cc; break;
                        case 11: t = m6809.dp.bh; break;
                        default: t = 0xff; break;
                    }
                }
                switch (tb & 15)
                {
                    case 0: m6809.d.wl = t; break;
                    case 1: m6809.x.wl = t; break;
                    case 2: m6809.y.wl = t; break;
                    case 3: m6809.u.wl = t; break;
                    case 4: m6809.s.wl = t; break;
                    case 5: m6809.pc.wl = t; change_pc16(m6809.pc.d); break;
                    case 8: m6809.d.bh = (byte)t; break;
                    case 9: m6809.d.bl = (byte)t; break;
                    case 10: m6809.cc = (byte)t; break;
                    case 11: m6809.dp.bh = (byte)t; break;
                }
            }

            /* $20 BRA relative ----- */
            void bra()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t));
                change_pc16(m6809.pc.d);
                /* JB 970823 - speed up busy loops */
                if (t == 0xfe)
                    if (m6809_ICount[0] > 0) m6809_ICount[0] = 0;
            }

            /* $21 BRN relative ----- */
            void brn()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
            }

            /* $1021 LBRN relative ----- */
            void lbrn()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                m6809.pc.wl += 2;
            }

            /* $22 BHI relative ----- */
            void bhi()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                    if ((m6809.cc & (0x04 | 0x01)) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t));
                        change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1022 LBHI relative ----- */
            void lbhi()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & (0x04 | 0x01)) == 0)
                    {
                        m6809_ICount[0] -= 1; m6809.pc.wl += t.wl;
                        change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $23 BLS relative ----- */
            void bls()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & (0x04 | 0x01)) != 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1023 LBLS relative ----- */
            void lbls()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & (0x04 | 0x01)) != 0)
                    {
                        m6809_ICount[0] -= 1; m6809.pc.wl += t.wl;
                        change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $24 BCC relative ----- */
            void bcc()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x01) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1024 LBCC relative ----- */
            void lbcc()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x01) == 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $25 BCS relative ----- */
            void bcs()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x01) != 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1025 LBCS relative ----- */
            void lbcs()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x01) != 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $26 BNE relative ----- */
            void bne()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x04) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1026 LBNE relative ----- */
            void lbne()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x04) == 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $27 BEQ relative ----- */
            void beq()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x04) != 0)
                    { m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d); }
                };
            }

            /* $1027 LBEQ relative ----- */
            void lbeq()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x04) != 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $28 BVC relative ----- */
            void bvc()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x02) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1028 LBVC relative ----- */
            void lbvc()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x02) == 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $29 BVS relative ----- */
            void bvs()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x02) != 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $1029 LBVS relative ----- */
            void lbvs()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x02) != 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $2A BPL relative ----- */
            void bpl()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x08) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $102A LBPL relative ----- */
            void lbpl()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x08) == 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $2B BMI relative ----- */
            void bmi()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((m6809.cc & 0x08) != 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $102B LBMI relative ----- */
            void lbmi()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((m6809.cc & 0x08) != 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $2C BGE relative ----- */
            void bge()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if (((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) == 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $102C LBGE relative ----- */
            void lbge()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if (((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) == 0)
                    {
                        m6809_ICount[0] -= 1; m6809.pc.wl += t.wl;
                        change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $2D BLT relative ----- */
            void blt()
            {
                {
                    byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if (((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0)
                    {
                        m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d);
                    }
                };
            }

            /* $102D LBLT relative ----- */
            void lblt()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if (((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $2E BGT relative ----- */
            void bgt()
            {
                { byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if (!(((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0 || (m6809.cc & 0x04) != 0)) { m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d); } };
            }

            /* $102E LBGT relative ----- */
            void lbgt()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if (!(((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0 || (m6809.cc & 0x04) != 0)) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $2F BLE relative ----- */
            void ble()
            {
                { byte t; t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; if ((((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0 || (m6809.cc & 0x04) != 0)) { m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t)); change_pc16(m6809.pc.d); } };
            }

            /* $102F LBLE relative ----- */
            void lble()
            {
                {
                    PAIR t = new PAIR(); t.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; if ((((m6809.cc & 0x08) ^ ((m6809.cc & 0x02) << 2)) != 0 || (m6809.cc & 0x04) != 0)) { m6809_ICount[0] -= 1; m6809.pc.wl += t.wl; change_pc16(m6809.pc.d); }
                };
            }

            /* $30 LEAX indexed --*-- */
            void leax()
            {
                fetch_effective_address();
                m6809.x.wl = ea.wl;
                m6809.cc &= unchecked((byte)~(0x04));
                if ((m6809.x.wl) == 0) m6809.cc |= 0x04;
            }

            /* $31 LEAY indexed --*-- */
            void leay()
            {
                fetch_effective_address();
                m6809.y.wl = ea.wl;
                m6809.cc &= unchecked((byte)~(0x04));
                if ((m6809.y.wl) == 0) m6809.cc |= 0x04;
            }

            /* $32 LEAS indexed ----- */
            void leas()
            {
                fetch_effective_address();
                m6809.s.wl = ea.wl;
                m6809.int_state |= 32;
            }

            /* $33 LEAU indexed ----- */
            void leau()
            {
                fetch_effective_address();
                m6809.u.wl = ea.wl;
            }

            /* $34 PSHS inherent ----- */
            void pshs()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if ((t & 0x80) != 0)
                {
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                    cpu_writemem16((int)m6809.s.d, m6809.pc.bh); m6809_ICount[0] -= 2;
                }
                if ((t & 0x40) != 0)
                {
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl;
                    cpu_writemem16((int)m6809.s.d, m6809.u.bh); m6809_ICount[0] -= 2;
                }
                if ((t & 0x20) != 0)
                {
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl;
                    cpu_writemem16((int)m6809.s.d, m6809.y.bh); m6809_ICount[0] -= 2;
                }
                if ((t & 0x10) != 0)
                {
                    --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl;
                    cpu_writemem16((int)m6809.s.d, m6809.x.bh); m6809_ICount[0] -= 2;
                }
                if ((t & 0x08) != 0) { --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh); m6809_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl); m6809_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh); m6809_ICount[0] -= 1; }
                if ((t & 0x01) != 0) { --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc); m6809_ICount[0] -= 1; }
            }

            /* 35 PULS inherent ----- */
            void puls()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if ((t & 0x01) != 0) { m6809.cc = (byte)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { m6809.d.bh = (byte)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { m6809.d.bl = (byte)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x08) != 0) { m6809.dp.bh = (byte)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x10) != 0)
                {
                    m6809.x.d = (uint)(cpu_readmem16((int)m6809.s.d) << 8); m6809.s.wl++;
                    m6809.x.d |= (uint)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x20) != 0)
                {
                    m6809.y.d = (uint)(cpu_readmem16((int)m6809.s.d) << 8); m6809.s.wl++;
                    m6809.y.d |= (uint)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x40) != 0)
                {
                    m6809.u.d = (uint)cpu_readmem16((int)m6809.s.d) << 8; m6809.s.wl++;
                    m6809.u.d |= (uint)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x80) != 0)
                {
                    m6809.pc.d = (uint)(cpu_readmem16((int)m6809.s.d) << 8); m6809.s.wl++;
                    m6809.pc.d |= (uint)cpu_readmem16((int)m6809.s.d); m6809.s.wl++; change_pc16(m6809.pc.d); m6809_ICount[0] -= 2;
                }

                /* HJB 990225: moved check after all PULLs */
                if ((t & 0x01) != 0)
                {
                    if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE)
                        m6809.int_state &= unchecked((byte)~16);
                    if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                    {
                        if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                        else
                        {
                            m6809.cc &= unchecked((byte)~0x80);
                            --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                        } m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6);
                        change_pc16(m6809.pc.d); m6809.irq_callback(1);
                    }
                    else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                    {
                        if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                        else
                        {
                            m6809.cc |= 0x80; --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                            m6809.extra_cycles += 19;
                        } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8);
                        change_pc16(m6809.pc.d); m6809.irq_callback(0);
                    };
                }
            }

            /* $36 PSHU inherent ----- */
            void pshu()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if ((t & 0x80) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.pc.bl); --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.pc.bh); m6809_ICount[0] -= 2; }
                if ((t & 0x40) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.s.bl); --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.s.bh); m6809_ICount[0] -= 2; }
                if ((t & 0x20) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.y.bl); --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.y.bh); m6809_ICount[0] -= 2; }
                if ((t & 0x10) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.x.bl); --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.x.bh); m6809_ICount[0] -= 2; }
                if ((t & 0x08) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.dp.bh); ; m6809_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.d.bl); ; m6809_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.d.bh); ; m6809_ICount[0] -= 1; }
                if ((t & 0x01) != 0) { --m6809.u.wl; cpu_writemem16((int)m6809.u.d, m6809.cc); ; m6809_ICount[0] -= 1; }
            }

            /* 37 PULU inherent ----- */
            void pulu()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                if ((t & 0x01) != 0) { m6809.cc = (byte)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x02) != 0) { m6809.d.bh = (byte)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x04) != 0) { m6809.d.bl = (byte)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x08) != 0) { m6809.dp.bh = (byte)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 1; }
                if ((t & 0x10) != 0)
                {
                    m6809.x.d = (uint)(cpu_readmem16((int)m6809.u.d) << 8); m6809.u.wl++;
                    m6809.x.d |= (uint)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x20) != 0)
                {
                    m6809.y.d = (uint)(cpu_readmem16((int)m6809.u.d) << 8); m6809.u.wl++;
                    m6809.y.d |= (uint)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x40) != 0)
                {
                    m6809.s.d = (uint)(cpu_readmem16((int)m6809.u.d) << 8); m6809.u.wl++;
                    m6809.s.d |= (uint)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; m6809_ICount[0] -= 2;
                }
                if ((t & 0x80) != 0)
                {
                    m6809.pc.d = (uint)(cpu_readmem16((int)m6809.u.d) << 8); m6809.u.wl++;
                    m6809.pc.d |= (uint)cpu_readmem16((int)m6809.u.d); m6809.u.wl++; change_pc16(m6809.pc.d); m6809_ICount[0] -= 2;
                }

                /* HJB 990225: moved check after all PULLs */
                if ((t & 0x01) != 0)
                {
                    if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE)
                        m6809.int_state &= unchecked((byte)~16);
                    if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                    {
                        if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                        else
                        {
                            m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                        } m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6);

                        change_pc16(m6809.pc.d); m6809.irq_callback(1);
                    }
                    else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                    {
                        if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                        else
                        {
                            m6809.cc |= 0x80;
                            --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl;
                            cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 19;
                        } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8);
                        change_pc16(m6809.pc.d); m6809.irq_callback(0);
                    };
                }
            }

            /* $38 ILLEGAL */

            /* $39 RTS inherent ----- */
            void rts()
            {
                m6809.pc.d = (uint)(cpu_readmem16((int)m6809.s.d) << 8); m6809.s.wl++; m6809.pc.d |= (uint)cpu_readmem16((int)m6809.s.d);
                m6809.s.wl++;
                change_pc16(m6809.pc.d);
            }

            /* $3A ABX inherent ----- */
            void abx()
            {
                m6809.x.wl += m6809.d.bl;
            }

            /* $3B RTI inherent ##### */
            void rti()
            {
                byte t;
                m6809.cc = (byte)cpu_readmem16((int)m6809.s.d); m6809.s.wl++;
                t = (byte)(m6809.cc & 0x80); /* HJB 990225: entire state saved? */
                if (t != 0)
                {
                    m6809_ICount[0] -= 9;
                    m6809.d.bh = (byte)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                    m6809.d.bl = (byte)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                    m6809.dp.bh = (byte)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                    m6809.x.d = (uint)((cpu_readmem16((int)m6809.s.d)) << 8); m6809.s.wl++; m6809.x.d |= (uint)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                    m6809.y.d = (uint)((cpu_readmem16((int)m6809.s.d)) << 8); m6809.s.wl++; m6809.y.d |= (uint)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                    m6809.u.d = (uint)((cpu_readmem16((int)m6809.s.d)) << 8); m6809.s.wl++; m6809.u.d |= (uint)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                }
                m6809.pc.d = (uint)((cpu_readmem16((int)m6809.s.d)) << 8); m6809.s.wl++;
                m6809.pc.d |= (uint)(cpu_readmem16((int)m6809.s.d)); m6809.s.wl++;
                change_pc16(m6809.pc.d);
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE) m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    {
                        m6809.int_state &= unchecked((byte)~8);
                        m6809.extra_cycles += 7;
                    }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    }
                    m6809.cc |= 0x40 | 0x10; m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d);
                    m6809.irq_callback(1);
                }
                else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc |= 0x80;
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                        --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                        m6809.extra_cycles += 19;
                    } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8); change_pc16(m6809.pc.d);
                    m6809.irq_callback(0);
                }; /* HJB 990116 */
            }

            /* $3C CWAI inherent ----1 */
            void cwai()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.cc &= t;
                /*

                    * CWAI stacks the entire machine state on the hardware stack,

                    * then waits for an interrupt; when the interrupt is taken

                    * later, the state is *not* saved again after CWAI.

                    */
                m6809.cc |= 0x80; /* HJB 990225: save entire state */
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                m6809.int_state |= 8; /* HJB 990228 */
                if (m6809.irq_state[0] != CLEAR_LINE || m6809.irq_state[1] != CLEAR_LINE) m6809.int_state &= unchecked((byte)~16);
                if (m6809.irq_state[1] != CLEAR_LINE && (m6809.cc & 0x40) == 0)
                {
                    if ((m6809.int_state & 8) != 0) { m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7; }
                    else
                    {
                        m6809.cc &= unchecked((byte)~0x80);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc); m6809.extra_cycles += 10;
                    } m6809.cc |= 0x40 | 0x10;
                    m6809.pc.d = RM16(0xfff6); change_pc16(m6809.pc.d); m6809.irq_callback(1);
                }
                else if (m6809.irq_state[0] != CLEAR_LINE && (m6809.cc & 0x10) == 0)
                {
                    if ((m6809.int_state & 8) != 0)
                    {
                        m6809.int_state &= unchecked((byte)~8); m6809.extra_cycles += 7;
                    }
                    else
                    {
                        m6809.cc |= 0x80; --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.u.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh); --m6809.s.wl;
                        cpu_writemem16((int)m6809.s.d, m6809.dp.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                        --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                        m6809.extra_cycles += 19;
                    } m6809.cc |= 0x10; m6809.pc.d = RM16(0xfff8);
                    change_pc16(m6809.pc.d); m6809.irq_callback(0);
                }; /* HJB 990116 */
                if ((m6809.int_state & 8) != 0)
                    if (m6809_ICount[0] > 0)
                        m6809_ICount[0] = 0;
            }

            /* $3D MUL inherent --*-@ */
            void mul()
            {
                ushort t;
                t = (ushort)(m6809.d.bh * m6809.d.bl);
                m6809.cc &= unchecked((byte)~(0x04 | 0x01)); if (t == 0) m6809.cc |= 0x04; if ((t & 0x80) != 0) m6809.cc |= 0x01;
                m6809.d.wl = t;
            }

            /* $3E ILLEGAL */

            /* $3F SWI (SWI2 SWI3) absolute indirect ----- */
            void swi()
            {
                m6809.cc |= 0x80; /* HJB 980225: save entire state */
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                m6809.cc |= 0x40 | 0x10; /* inhibit FIRQ and IRQ */
                m6809.pc.d = RM16(0xfffa);
                change_pc16(m6809.pc.d);
            }

            /* $103F SWI2 absolute indirect ----- */
            void swi2()
            {
                m6809.cc |= 0x80; /* HJB 980225: save entire state */
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                m6809.pc.d = RM16(0xfff4);
                change_pc16(m6809.pc.d);
            }

            /* $113F SWI3 absolute indirect ----- */
            void swi3()
            {
                m6809.cc |= 0x80; /* HJB 980225: save entire state */
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.u.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.y.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.x.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.dp.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bl);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.d.bh);
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.cc);
                m6809.pc.d = RM16(0xfff2);
                change_pc16(m6809.pc.d);
            }





            /* $40 NEGA inherent ?**** */
            void nega()
            {
                ushort r;
                r = (ushort)(-m6809.d.bh);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((0 ^ m6809.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $41 ILLEGAL */

            /* $42 ILLEGAL */

            /* $43 COMA inherent -**01 */
            void coma()
            {
                m6809.d.bh = (byte)~m6809.d.bh;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                {
                    m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4);
                    if (m6809.d.bh == 0) m6809.cc |= 0x04;
                };
                m6809.cc |= 0x01;
            }

            /* $44 LSRA inherent -0*-* */
            void lsra()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bh & 0x01);
                m6809.d.bh >>= 1;
                if ((byte)m6809.d.bh == 0) m6809.cc |= 0x04;
            }

            /* $45 ILLEGAL */

            /* $46 RORA inherent -**-* */
            void rora()
            {
                byte r;
                r = (byte)((m6809.cc & 0x01) << 7);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bh & 0x01);
                r |= (byte)(m6809.d.bh >> 1);
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
                m6809.d.bh = r;
            }

            /* $47 ASRA inherent ?**-* */
            void asra()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bh & 0x01);
                m6809.d.bh = (byte)((m6809.d.bh & 0x80) | (m6809.d.bh >> 1));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $48 ASLA inherent ?**** */
            void asla()
            {
                ushort r;
                r = (ushort)(m6809.d.bh << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ m6809.d.bh ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $49 ROLA inherent -**** */
            void rola()
            {
                ushort t, r;
                t = m6809.d.bh;
                r = (ushort)((m6809.cc & 0x01) | (t << 1));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $4A DECA inherent -***- */
            void deca()
            {
                --m6809.d.bh;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8d[(m6809.d.bh) & 0xff]; };
            }

            /* $4B ILLEGAL */

            /* $4C INCA inherent -***- */
            void inca()
            {
                ++m6809.d.bh;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8i[(m6809.d.bh) & 0xff]; };
            }

            /* $4D TSTA inherent -**0- */
            void tsta()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $4E ILLEGAL */

            /* $4F CLRA inherent -0100 */
            void clra()
            {
                m6809.d.bh = 0;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); m6809.cc |= 0x04;
            }





            /* $50 NEGB inherent ?**** */
            void negb()
            {
                ushort r;
                r = (ushort)-m6809.d.bl;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((0 ^ m6809.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $51 ILLEGAL */

            /* $52 ILLEGAL */

            /* $53 COMB inherent -**01 */
            void comb()
            {
                m6809.d.bl = (byte)~m6809.d.bl;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
                m6809.cc |= 0x01;
            }

            /* $54 LSRB inherent -0*-* */
            void lsrb()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bl & 0x01);
                m6809.d.bl >>= 1;
                if ((byte)m6809.d.bl == 0) m6809.cc |= 0x04;
            }

            /* $55 ILLEGAL */

            /* $56 RORB inherent -**-* */
            void rorb()
            {
                byte r;
                r = (byte)((m6809.cc & 0x01) << 7);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bl & 0x01);
                r |= (byte)(m6809.d.bl >> 1);
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
                m6809.d.bl = r;
            }

            /* $57 ASRB inherent ?**-* */
            void asrb()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(m6809.d.bl & 0x01);
                m6809.d.bl = (byte)((m6809.d.bl & 0x80) | (m6809.d.bl >> 1));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $58 ASLB inherent ?**** */
            void aslb()
            {
                ushort r;
                r = (ushort)(m6809.d.bl << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ m6809.d.bl ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $59 ROLB inherent -**** */
            void rolb()
            {
                ushort t, r;
                t = m6809.d.bl;
                r = (ushort)(m6809.cc & 0x01);
                r |= (ushort)(t << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $5A DECB inherent -***- */
            void decb()
            {
                --m6809.d.bl;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8d[(m6809.d.bl) & 0xff]; };
            }

            /* $5B ILLEGAL */

            /* $5C INCB inherent -***- */
            void incb()
            {
                ++m6809.d.bl;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= flags8i[(m6809.d.bl) & 0xff]; };
            }

            /* $5D TSTB inherent -**0- */
            void tstb()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $5E ILLEGAL */

            /* $5F CLRB inherent -0100 */
            void clrb()
            {
                m6809.d.bl = 0;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); m6809.cc |= 0x04;
            }





            /* $60 NEG indexed ?**** */
            void neg_ix()
            {
                ushort r, t;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)-t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $61 ILLEGAL */

            /* $62 ILLEGAL */

            /* $63 COM indexed -**01 */
            void com_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)~(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
                m6809.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $64 LSR indexed -0*-* */
            void lsr_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t >>= 1; if ((byte)t == 0) m6809.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $65 ILLEGAL */

            /* $66 ROR indexed -**-* */
            void ror_ix()
            {
                byte t, r;
                fetch_effective_address();
                t = (byte)(cpu_readmem16((int)ea.d));
                r = (byte)((m6809.cc & 0x01) << 7);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $67 ASR indexed ?**-* */
            void asr_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $68 ASL indexed ?**** */
            void asl_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(t << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $69 ROL indexed -**** */
            void rol_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.cc & 0x01);
                r |= (ushort)(t << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $6A DEC indexed -***- */
            void dec_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)((cpu_readmem16((int)ea.d)) - 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $6B ILLEGAL */

            /* $6C INC indexed -***- */
            void inc_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)((cpu_readmem16((int)ea.d)) + 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $6D TST indexed -**0- */
            void tst_ix()
            {
                byte t;
                fetch_effective_address();
                t = (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
            }

            /* $6E JMP indexed ----- */
            void jmp_ix()
            {
                fetch_effective_address();
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $6F CLR indexed -0100 */
            void clr_ix()
            {
                fetch_effective_address();
                cpu_writemem16((int)ea.d, 0);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); m6809.cc |= 0x04;
            }





            /* $70 NEG extended ?**** */
            void neg_ex()
            {
                ushort r, t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                }; r = (ushort)-t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6809.cc |= 0x04; m6809.cc |= (byte)(((0 ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $71 ILLEGAL */

            /* $72 ILLEGAL */

            /* $73 COM extended -**01 */
            void com_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; t = (byte)~t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; }; m6809.cc |= 0x01;
                cpu_writemem16((int)ea.d, t);
            }

            /* $74 LSR extended -0*-* */
            void lsr_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t >>= 1; if (t == 0) m6809.cc |= 0x04;
                cpu_writemem16((int)ea.d, t);
            }

            /* $75 ILLEGAL */

            /* $76 ROR extended -**-* */
            void ror_ex()
            {
                byte t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; r = (byte)((m6809.cc & 0x01) << 7);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01)); m6809.cc |= (byte)(t & 0x01);
                r |= (byte)(t >> 1); { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, r);
            }

            /* $77 ASR extended ?**-* */
            void asr_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x01));
                m6809.cc |= (byte)(t & 0x01);
                t = (byte)((t & 0x80) | (t >> 1));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $78 ASL extended ?**** */
            void asl_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)((cpu_readmem16((int)ea.d)));
                }; r = (ushort)(t << 1);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6809.cc |= 0x04; m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $79 ROL extended -**** */
            void rol_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                }; r = (ushort)((m6809.cc & 0x01) | (t << 1));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6809.cc |= 0x04; m6809.cc |= (byte)(((t ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                cpu_writemem16((int)ea.d, r);
            }

            /* $7A DEC extended -***- */
            void dec_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; --t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= flags8d[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $7B ILLEGAL */

            /* $7C INC extended -***- */
            void inc_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; ++t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= flags8i[(t) & 0xff]; };
                cpu_writemem16((int)ea.d, t);
            }

            /* $7D TST extended -**0- */
            void tst_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                }; m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((t & 0x80) >> 4); if (t == 0)m6809.cc |= 0x04; };
            }

            /* $7E JMP extended ----- */
            void jmp_ex()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $7F CLR extended -0100 */
            void clr_ex()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                cpu_writemem16((int)ea.d, 0);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01)); m6809.cc |= 0x04;
            }






            /* $80 SUBA immediate ?**** */
            void suba_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $81 CMPA immediate ?**** */
            void cmpa_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $82 SBCA immediate ?**** */
            void sbca_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bh - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $83 SUBD (CMPD CMPU) immediate -**** */
            void subd_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $1083 CMPD immediate -**** */
            void cmpd_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $1183 CMPU immediate -**** */
            void cmpu_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.u.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $84 ANDA immediate -**0- */
            void anda_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bh &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $85 BITA immediate -**0- */
            void bita_im()
            {
                byte t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (byte)(m6809.d.bh & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $86 LDA immediate -**0- */
            void lda_im()
            {
                m6809.d.bh = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $87 STA immediate -**0- */
            void sta_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl++;
                cpu_writemem16((int)ea.d, m6809.d.bh);
            }

            /* $88 EORA immediate -**0- */
            void eora_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bh ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $89 ADCA immediate ***** */
            void adca_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bh + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $8A ORA immediate -**0- */
            void ora_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bh |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $8B ADDA immediate ***** */
            void adda_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bh + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $8C CMPX (CMPY CMPS) immediate -**** */
            void cmpx_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.x.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $108C CMPY immediate -**** */
            void cmpy_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.y.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $118C CMPS immediate -**** */
            void cmps_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.s.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $8D BSR ----- */
            void bsr()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                m6809.pc.wl += ((ushort)((t & 0x80) != 0 ? t | 0xff00 : t));
                change_pc16(m6809.pc.d);
            }

            /* $8E LDX (LDY) immediate -**0- */
            void ldx_im()
            {
                m6809.x.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
            }

            /* $108E LDY immediate -**0- */
            void ldy_im()
            {
                m6809.y.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $8F STX (STY) immediate -**0- */
            void stx_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl += 2;
                WM16(ea.d, m6809.x);
            }

            /* is this a legal instruction? */
            /* $108F STY immediate -**0- */
            void sty_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl += 2;
                WM16(ea.d, m6809.y);
            }

            /* $90 SUBA direct ?**** */
            void suba_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $91 CMPA direct ?**** */
            void cmpa_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $92 SBCA direct ?**** */
            void sbca_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bh - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $93 SUBD (CMPD CMPU) direct -**** */
            void subd_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $1093 CMPD direct -**** */
            void cmpd_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $1193 CMPU direct -**** */
            void cmpu_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.u.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.u.wl ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $94 ANDA direct -**0- */
            void anda_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bh &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $95 BITA direct -**0- */
            void bita_di()
            {
                byte t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                r = (byte)(m6809.d.bh & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $96 LDA direct -**0- */
            void lda_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.d.bh = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $97 STA direct -**0- */
            void sta_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                cpu_writemem16((int)ea.d, m6809.d.bh);
            }

            /* $98 EORA direct -**0- */
            void eora_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bh ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $99 ADCA direct ***** */
            void adca_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bh + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $9A ORA direct -**0- */
            void ora_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bh |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $9B ADDA direct ***** */
            void adda_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bh + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $9C CMPX (CMPY CMPS) direct -**** */
            void cmpx_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.x.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }


            /* $109C CMPY direct -**** */
            void cmpy_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.y.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $119C CMPS direct -**** */
            void cmps_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.s.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $9D JSR direct ----- */
            void jsr_di()
            {
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $9E LDX (LDY) direct -**0- */
            void ldx_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.x.d = RM16(ea.d); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
            }

            /* $109E LDY direct -**0- */
            void ldy_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.y.d = RM16(ea.d); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
            }

            /* $9F STX (STY) direct -**0- */
            void stx_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                WM16(ea.d, m6809.x);
            }

            /* $109F STY direct -**0- */
            void sty_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                WM16(ea.d, m6809.y);
            }


            /* $a0 SUBA indexed ?**** */
            void suba_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $a1 CMPA indexed ?**** */
            void cmpa_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $a2 SBCA indexed ?**** */
            void sbca_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bh - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $a3 SUBD (CMPD CMPU) indexed -**** */
            void subd_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $10a3 CMPD indexed -**** */
            void cmpd_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $11a3 CMPU indexed -**** */
            void cmpu_ix()
            {
                uint r;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                r = m6809.u.wl - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.u.wl ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $a4 ANDA indexed -**0- */
            void anda_ix()
            {
                fetch_effective_address();
                m6809.d.bh &= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $a5 BITA indexed -**0- */
            void bita_ix()
            {
                byte r;
                fetch_effective_address();
                r = (byte)(m6809.d.bh & (cpu_readmem16((int)ea.d)));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $a6 LDA indexed -**0- */
            void lda_ix()
            {
                fetch_effective_address();
                m6809.d.bh = (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $a7 STA indexed -**0- */
            void sta_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, m6809.d.bh);
            }

            /* $a8 EORA indexed -**0- */
            void eora_ix()
            {
                fetch_effective_address();
                m6809.d.bh ^= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $a9 ADCA indexed ***** */
            void adca_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bh + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $aA ORA indexed -**0- */
            void ora_ix()
            {
                fetch_effective_address();
                m6809.d.bh |= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $aB ADDA indexed ***** */
            void adda_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bh + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $aC CMPX (CMPY CMPS) indexed -**** */
            void cmpx_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.x.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $10aC CMPY indexed -**** */
            void cmpy_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.y.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $11aC CMPS indexed -**** */
            void cmps_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.s.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $aD JSR indexed ----- */
            void jsr_ix()
            {
                fetch_effective_address();
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $aE LDX (LDY) indexed -**0- */
            void ldx_ix()
            {
                fetch_effective_address();
                m6809.x.wl = (ushort)RM16(ea.d);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10aE LDY indexed -**0- */
            void ldy_ix()
            {
                fetch_effective_address();
                m6809.y.wl = (ushort)RM16(ea.d);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
            }

            /* $aF STX (STY) indexed -**0- */
            void stx_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
                WM16(ea.d, m6809.x);
            }

            /* $10aF STY indexed -**0- */
            void sty_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
                WM16(ea.d, m6809.y);
            }





            /* $b0 SUBA extended ?**** */
            void suba_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $b1 CMPA extended ?**** */
            void cmpa_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bh - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $b2 SBCA extended ?**** */
            void sbca_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bh - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bh = (byte)r;
            }

            /* $b3 SUBD (CMPD CMPU) extended -**** */
            void subd_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $10b3 CMPD extended -**** */
            void cmpd_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.d.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $11b3 CMPU extended -**** */
            void cmpu_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.u.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $b4 ANDA extended -**0- */
            void anda_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bh &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $b5 BITA extended -**0- */
            void bita_ex()
            {
                byte t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                r = (byte)(m6809.d.bh & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $b6 LDA extended -**0- */
            void lda_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.d.bh = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $b7 STA extended -**0- */
            void sta_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                cpu_writemem16((int)ea.d, m6809.d.bh);
            }

            /* $b8 EORA extended -**0- */
            void eora_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bh ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $b9 ADCA extended ***** */
            void adca_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bh + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $bA ORA extended -**0- */
            void ora_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bh |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bh & 0x80) >> 4); if (m6809.d.bh == 0)m6809.cc |= 0x04; };
            }

            /* $bB ADDA extended ***** */
            void adda_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bh + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bh ^ t ^ r) & 0x10) << 1);
                m6809.d.bh = (byte)r;
            }

            /* $bC CMPX (CMPY CMPS) extended -**** */
            void cmpx_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.x.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $10bC CMPY extended -**** */
            void cmpy_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.y.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $11bC CMPS extended -**** */
            void cmps_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.s.wl;
                r = d - b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
            }

            /* $bD JSR extended ----- */
            void jsr_ex()
            {
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                --m6809.s.wl; cpu_writemem16((int)m6809.s.d, m6809.pc.bl); --m6809.s.wl;
                cpu_writemem16((int)m6809.s.d, m6809.pc.bh);
                m6809.pc.d = ea.d;
                change_pc16(m6809.pc.d);
            }

            /* $bE LDX (LDY) extended -**0- */
            void ldx_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.x.d = RM16(ea.d);
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10bE LDY extended -**0- */
            void ldy_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.y.d = RM16(ea.d);
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
            }

            /* $bF STX (STY) extended -**0- */
            void stx_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.x.wl & 0x8000) >> 12); if (m6809.x.wl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                WM16(ea.d, m6809.x);
            }

            /* $10bF STY extended -**0- */
            void sty_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.y.wl & 0x8000) >> 12); if (m6809.y.wl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                WM16(ea.d, m6809.y);
            }






            /* $c0 SUBB immediate ?**** */
            void subb_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $c1 CMPB immediate ?**** */
            void cmpb_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4);
                    if ((byte)r == 0) m6809.cc |= 0x04; m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6);
                    m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $c2 SBCB immediate ?**** */
            void sbcb_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bl - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $c3 ADDD immediate -**** */
            void addd_im()
            {
                uint r, d;
                PAIR b;
                b.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                d = m6809.d.wl;
                r = d + b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $c4 ANDB immediate -**0- */
            void andb_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bl &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $c5 BITB immediate -**0- */
            void bitb_im()
            {
                byte t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (byte)(m6809.d.bl & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $c6 LDB immediate -**0- */
            void ldb_im()
            {
                m6809.d.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $c7 STB immediate -**0- */
            void stb_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl++;
                cpu_writemem16((int)ea.d, m6809.d.bl);
            }

            /* $c8 EORB immediate -**0- */
            void eorb_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bl ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $c9 ADCB immediate ***** */
            void adcb_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bl + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $cA ORB immediate -**0- */
            void orb_im()
            {
                byte t;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                m6809.d.bl |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $cB ADDB immediate ***** */
            void addb_im()
            {
                ushort t, r;
                t = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                r = (ushort)(m6809.d.bl + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $cC LDD immediate -**0- */
            void ldd_im()
            {
                m6809.d.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
            }

            /* is this a legal instruction? */
            /* $cD STD immediate -**0- */
            void std_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl += 2;
                WM16(ea.d, m6809.d);
            }

            /* $cE LDU (LDS) immediate -**0- */
            void ldu_im()
            {
                m6809.u.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10cE LDS immediate -**0- */
            void lds_im()
            {
                m6809.s.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                m6809.int_state |= 32;
            }

            /* is this a legal instruction? */
            /* $cF STU (STS) immediate -**0- */
            void stu_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl += 2;
                WM16(ea.d, m6809.u);
            }

            /* is this a legal instruction? */
            /* $10cF STS immediate -**0- */
            void sts_im()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.pc.d; m6809.pc.wl += 2;
                WM16(ea.d, m6809.s);
            }






            /* $d0 SUBB direct ?**** */
            void subb_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)((cpu_readmem16((int)ea.d))); };
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $d1 CMPB direct ?**** */
            void cmpb_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)((cpu_readmem16((int)ea.d))); };
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $d2 SBCB direct ?**** */
            void sbcb_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)((cpu_readmem16((int)ea.d))); };
                r = (ushort)(m6809.d.bl - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $d3 ADDD direct -**** */
            void addd_di()
            {
                uint r, d;
                PAIR b;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; b.d = RM16(ea.d); };
                d = m6809.d.wl;
                r = d + b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $d4 ANDB direct -**0- */
            void andb_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bl &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $d5 BITB direct -**0- */
            void bitb_di()
            {
                byte t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                r = (byte)(m6809.d.bl & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $d6 LDB direct -**0- */
            void ldb_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.d.bl = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $d7 STB direct -**0- */
            void stb_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                cpu_writemem16((int)ea.d, m6809.d.bl);
            }

            /* $d8 EORB direct -**0- */
            void eorb_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bl ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $d9 ADCB direct ***** */
            void adcb_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bl + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $dA ORB direct -**0- */
            void orb_di()
            {
                byte t;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (byte)(cpu_readmem16((int)ea.d)); };
                m6809.d.bl |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $dB ADDB direct ***** */
            void addb_di()
            {
                ushort t, r;
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; t = (ushort)(cpu_readmem16((int)ea.d)); };
                r = (ushort)(m6809.d.bl + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $dC LDD direct -**0- */
            void ldd_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.d.d = RM16(ea.d); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
            }

            /* $dD STD direct -**0- */
            void std_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                WM16(ea.d, m6809.d);
            }

            /* $dE LDU (LDS) direct -**0- */
            void ldu_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.u.d = RM16(ea.d); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10dE LDS direct -**0- */
            void lds_di()
            {
                { ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; m6809.s.d = RM16(ea.d); };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                m6809.int_state |= 32;
            }

            /* $dF STU (STS) direct -**0- */
            void stu_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                WM16(ea.d, m6809.u);
            }

            /* $10dF STS direct -**0- */
            void sts_di()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                ea.d = m6809.dp.d; ea.bl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                WM16(ea.d, m6809.s);
            }

            /* $e0 SUBB indexed ?**** */
            void subb_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $e1 CMPB indexed ?**** */
            void cmpb_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $e2 SBCB indexed ?**** */
            void sbcb_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bl - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $e3 ADDD indexed -**** */
            void addd_ix()
            {
                uint r, d;
                PAIR b;
                fetch_effective_address();
                b.d = RM16(ea.d);
                d = m6809.d.wl;
                r = d + b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $e4 ANDB indexed -**0- */
            void andb_ix()
            {
                fetch_effective_address();
                m6809.d.bl &= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $e5 BITB indexed -**0- */
            void bitb_ix()
            {
                byte r;
                fetch_effective_address();
                r = (byte)(m6809.d.bl & (cpu_readmem16((int)ea.d)));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $e6 LDB indexed -**0- */
            void ldb_ix()
            {
                fetch_effective_address();
                m6809.d.bl = (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $e7 STB indexed -**0- */
            void stb_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
                cpu_writemem16((int)ea.d, m6809.d.bl);
            }

            /* $e8 EORB indexed -**0- */
            void eorb_ix()
            {
                fetch_effective_address();
                m6809.d.bl ^= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $e9 ADCB indexed ***** */
            void adcb_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bl + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $eA ORB indexed -**0- */
            void orb_ix()
            {
                fetch_effective_address();
                m6809.d.bl |= (byte)(cpu_readmem16((int)ea.d));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $eB ADDB indexed ***** */
            void addb_ix()
            {
                ushort t, r;
                fetch_effective_address();
                t = (ushort)(cpu_readmem16((int)ea.d));
                r = (ushort)(m6809.d.bl + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $eC LDD indexed -**0- */
            void ldd_ix()
            {
                fetch_effective_address();
                m6809.d.wl = (ushort)RM16(ea.d);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02)); { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
            }

            /* $eD STD indexed -**0- */
            void std_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
                WM16(ea.d, m6809.d);
            }

            /* $eE LDU (LDS) indexed -**0- */
            void ldu_ix()
            {
                fetch_effective_address();
                m6809.u.wl = (ushort)RM16(ea.d);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10eE LDS indexed -**0- */
            void lds_ix()
            {
                fetch_effective_address();
                m6809.s.wl = (ushort)RM16(ea.d);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                m6809.int_state |= 32;
            }

            /* $eF STU (STS) indexed -**0- */
            void stu_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
                WM16(ea.d, m6809.u);
            }

            /* $10eF STS indexed -**0- */
            void sts_ix()
            {
                fetch_effective_address();
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                WM16(ea.d, m6809.s);
            }


            /* $f0 SUBB extended ?**** */
            void subb_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $f1 CMPB extended ?**** */
            void cmpb_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bl - t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
            }

            /* $f2 SBCB extended ?**** */
            void sbcb_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bl - t - (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.d.bl = (byte)r;
            }

            /* $f3 ADDD extended -**** */
            void addd_ex()
            {
                uint r, d;
                PAIR b;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; b.d = RM16(ea.d);
                };
                d = m6809.d.wl;
                r = d + b.d;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x8000) >> 12); if ((ushort)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((d ^ b.d ^ r ^ (r >> 1)) & 0x8000) >> 14); m6809.cc |= (byte)((r & 0x10000) >> 16);
                };
                m6809.d.wl = (ushort)r;
            }

            /* $f4 ANDB extended -**0- */
            void andb_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bl &= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $f5 BITB extended -**0- */
            void bitb_ex()
            {
                byte t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                r = (byte)(m6809.d.bl & t);
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((r & 0x80) >> 4); if (r == 0)m6809.cc |= 0x04; };
            }

            /* $f6 LDB extended -**0- */
            void ldb_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.d.bl = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $f7 STB extended -**0- */
            void stb_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                cpu_writemem16((int)ea.d, m6809.d.bl);
            }

            /* $f8 EORB extended -**0- */
            void eorb_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bl ^= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $f9 ADCB extended ***** */
            void adcb_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bl + t + (m6809.cc & 0x01));
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $fA ORB extended -**0- */
            void orb_ex()
            {
                byte t;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (byte)(cpu_readmem16((int)ea.d));
                };
                m6809.d.bl |= t;
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.bl & 0x80) >> 4); if (m6809.d.bl == 0)m6809.cc |= 0x04; };
            }

            /* $fB ADDB extended ***** */
            void addb_ex()
            {
                ushort t, r;
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; t = (ushort)(cpu_readmem16((int)ea.d));
                };
                r = (ushort)(m6809.d.bl + t);
                m6809.cc &= unchecked((byte)~(0x20 | 0x08 | 0x04 | 0x02 | 0x01));
                {
                    m6809.cc |= (byte)((r & 0x80) >> 4); if ((byte)r == 0) m6809.cc |= 0x04;
                    m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r ^ (r >> 1)) & 0x80) >> 6); m6809.cc |= (byte)((r & 0x100) >> 8);
                };
                m6809.cc |= (byte)(((m6809.d.bl ^ t ^ r) & 0x10) << 1);
                m6809.d.bl = (byte)r;
            }

            /* $fC LDD extended -**0- */
            void ldd_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.d.d = RM16(ea.d);
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
            }

            /* $fD STD extended -**0- */
            void std_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.d.wl & 0x8000) >> 12); if (m6809.d.wl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2;
                WM16(ea.d, m6809.d);
            }

            /* $fE LDU (LDS) extended -**0- */
            void ldu_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.u.d = RM16(ea.d);
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
            }

            /* $10fE LDS extended -**0- */
            void lds_ex()
            {
                {
                    ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                    m6809.pc.wl += 2; m6809.s.d = RM16(ea.d);
                };
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                m6809.int_state |= 32;
            }

            /* $fF STU (STS) extended -**0- */
            void stu_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.u.wl & 0x8000) >> 12); if (m6809.u.wl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                m6809.pc.wl += 2;
                WM16(ea.d, m6809.u);
            }

            /* $10fF STS extended -**0- */
            void sts_ex()
            {
                m6809.cc &= unchecked((byte)~(0x08 | 0x04 | 0x02));
                { m6809.cc |= (byte)((m6809.s.wl & 0x8000) >> 12); if (m6809.s.wl == 0)m6809.cc |= 0x04; };
                ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                m6809.pc.wl += 2;
                WM16(ea.d, m6809.s);
            }

            /* $10xx opcodes */
            void pref10()
            {
                byte ireg2 = cpu_readop(m6809.pc.d);
                m6809.pc.wl++;
                switch (ireg2)
                {
                    case 0x21: lbrn(); m6809_ICount[0] -= 5; break;
                    case 0x22: lbhi(); m6809_ICount[0] -= 5; break;
                    case 0x23: lbls(); m6809_ICount[0] -= 5; break;
                    case 0x24: lbcc(); m6809_ICount[0] -= 5; break;
                    case 0x25: lbcs(); m6809_ICount[0] -= 5; break;
                    case 0x26: lbne(); m6809_ICount[0] -= 5; break;
                    case 0x27: lbeq(); m6809_ICount[0] -= 5; break;
                    case 0x28: lbvc(); m6809_ICount[0] -= 5; break;
                    case 0x29: lbvs(); m6809_ICount[0] -= 5; break;
                    case 0x2a: lbpl(); m6809_ICount[0] -= 5; break;
                    case 0x2b: lbmi(); m6809_ICount[0] -= 5; break;
                    case 0x2c: lbge(); m6809_ICount[0] -= 5; break;
                    case 0x2d: lblt(); m6809_ICount[0] -= 5; break;
                    case 0x2e: lbgt(); m6809_ICount[0] -= 5; break;
                    case 0x2f: lble(); m6809_ICount[0] -= 5; break;

                    case 0x3f: swi2(); m6809_ICount[0] -= 20; break;

                    case 0x83: cmpd_im(); m6809_ICount[0] -= 5; break;
                    case 0x8c: cmpy_im(); m6809_ICount[0] -= 5; break;
                    case 0x8e: ldy_im(); m6809_ICount[0] -= 4; break;
                    case 0x8f: sty_im(); m6809_ICount[0] -= 4; break;

                    case 0x93: cmpd_di(); m6809_ICount[0] -= 7; break;
                    case 0x9c: cmpy_di(); m6809_ICount[0] -= 7; break;
                    case 0x9e: ldy_di(); m6809_ICount[0] -= 6; break;
                    case 0x9f: sty_di(); m6809_ICount[0] -= 6; break;

                    case 0xa3: cmpd_ix(); m6809_ICount[0] -= 7; break;
                    case 0xac: cmpy_ix(); m6809_ICount[0] -= 7; break;
                    case 0xae: ldy_ix(); m6809_ICount[0] -= 6; break;
                    case 0xaf: sty_ix(); m6809_ICount[0] -= 6; break;

                    case 0xb3: cmpd_ex(); m6809_ICount[0] -= 8; break;
                    case 0xbc: cmpy_ex(); m6809_ICount[0] -= 8; break;
                    case 0xbe: ldy_ex(); m6809_ICount[0] -= 7; break;
                    case 0xbf: sty_ex(); m6809_ICount[0] -= 7; break;

                    case 0xce: lds_im(); m6809_ICount[0] -= 4; break;
                    case 0xcf: sts_im(); m6809_ICount[0] -= 4; break;

                    case 0xde: lds_di(); m6809_ICount[0] -= 4; break;
                    case 0xdf: sts_di(); m6809_ICount[0] -= 4; break;

                    case 0xee: lds_ix(); m6809_ICount[0] -= 6; break;
                    case 0xef: sts_ix(); m6809_ICount[0] -= 6; break;

                    case 0xfe: lds_ex(); m6809_ICount[0] -= 7; break;
                    case 0xff: sts_ex(); m6809_ICount[0] -= 7; break;

                    default: illegal(); break;
                }
            }

            /* $11xx opcodes */
            void pref11()
            {
                byte ireg2 = cpu_readop(m6809.pc.d);
                m6809.pc.wl++;
                switch (ireg2)
                {
                    case 0x3f: swi3(); m6809_ICount[0] -= 20; break;

                    case 0x83: cmpu_im(); m6809_ICount[0] -= 5; break;
                    case 0x8c: cmps_im(); m6809_ICount[0] -= 5; break;

                    case 0x93: cmpu_di(); m6809_ICount[0] -= 7; break;
                    case 0x9c: cmps_di(); m6809_ICount[0] -= 7; break;

                    case 0xa3: cmpu_ix(); m6809_ICount[0] -= 7; break;
                    case 0xac: cmps_ix(); m6809_ICount[0] -= 7; break;

                    case 0xb3: cmpu_ex(); m6809_ICount[0] -= 8; break;
                    case 0xbc: cmps_ex(); m6809_ICount[0] -= 8; break;

                    default: illegal(); break;
                }
            }
            void fetch_effective_address()
            {
                byte postbyte = cpu_readop_arg(m6809.pc.d);
                m6809.pc.wl++;

                switch (postbyte)
                {
                    case 0x00: ea.wl = m6809.x.wl; m6809_ICount[0] -= 1; break;
                    case 0x01: ea.wl = (ushort)(m6809.x.wl + 1); m6809_ICount[0] -= 1; break;
                    case 0x02: ea.wl = (ushort)(m6809.x.wl + 2); m6809_ICount[0] -= 1; break;
                    case 0x03: ea.wl = (ushort)(m6809.x.wl + 3); m6809_ICount[0] -= 1; break;
                    case 0x04: ea.wl = (ushort)(m6809.x.wl + 4); m6809_ICount[0] -= 1; break;
                    case 0x05: ea.wl = (ushort)(m6809.x.wl + 5); m6809_ICount[0] -= 1; break;
                    case 0x06: ea.wl = (ushort)(m6809.x.wl + 6); m6809_ICount[0] -= 1; break;
                    case 0x07: ea.wl = (ushort)(m6809.x.wl + 7); m6809_ICount[0] -= 1; break;
                    case 0x08: ea.wl = (ushort)(m6809.x.wl + 8); m6809_ICount[0] -= 1; break;
                    case 0x09: ea.wl = (ushort)(m6809.x.wl + 9); m6809_ICount[0] -= 1; break;
                    case 0x0a: ea.wl = (ushort)(m6809.x.wl + 10); m6809_ICount[0] -= 1; break;
                    case 0x0b: ea.wl = (ushort)(m6809.x.wl + 11); m6809_ICount[0] -= 1; break;
                    case 0x0c: ea.wl = (ushort)(m6809.x.wl + 12); m6809_ICount[0] -= 1; break;
                    case 0x0d: ea.wl = (ushort)(m6809.x.wl + 13); m6809_ICount[0] -= 1; break;
                    case 0x0e: ea.wl = (ushort)(m6809.x.wl + 14); m6809_ICount[0] -= 1; break;
                    case 0x0f: ea.wl = (ushort)(m6809.x.wl + 15); m6809_ICount[0] -= 1; break;

                    case 0x10: ea.wl = (ushort)(m6809.x.wl - 16); m6809_ICount[0] -= 1; break;
                    case 0x11: ea.wl = (ushort)(m6809.x.wl - 15); m6809_ICount[0] -= 1; break;
                    case 0x12: ea.wl = (ushort)(m6809.x.wl - 14); m6809_ICount[0] -= 1; break;
                    case 0x13: ea.wl = (ushort)(m6809.x.wl - 13); m6809_ICount[0] -= 1; break;
                    case 0x14: ea.wl = (ushort)(m6809.x.wl - 12); m6809_ICount[0] -= 1; break;
                    case 0x15: ea.wl = (ushort)(m6809.x.wl - 11); m6809_ICount[0] -= 1; break;
                    case 0x16: ea.wl = (ushort)(m6809.x.wl - 10); m6809_ICount[0] -= 1; break;
                    case 0x17: ea.wl = (ushort)(m6809.x.wl - 9); m6809_ICount[0] -= 1; break;
                    case 0x18: ea.wl = (ushort)(m6809.x.wl - 8); m6809_ICount[0] -= 1; break;
                    case 0x19: ea.wl = (ushort)(m6809.x.wl - 7); m6809_ICount[0] -= 1; break;
                    case 0x1a: ea.wl = (ushort)(m6809.x.wl - 6); m6809_ICount[0] -= 1; break;
                    case 0x1b: ea.wl = (ushort)(m6809.x.wl - 5); m6809_ICount[0] -= 1; break;
                    case 0x1c: ea.wl = (ushort)(m6809.x.wl - 4); m6809_ICount[0] -= 1; break;
                    case 0x1d: ea.wl = (ushort)(m6809.x.wl - 3); m6809_ICount[0] -= 1; break;
                    case 0x1e: ea.wl = (ushort)(m6809.x.wl - 2); m6809_ICount[0] -= 1; break;
                    case 0x1f: ea.wl = (ushort)(m6809.x.wl - 1); m6809_ICount[0] -= 1; break;

                    case 0x20: ea.wl = (ushort)(m6809.y.wl); m6809_ICount[0] -= 1; break;
                    case 0x21: ea.wl = (ushort)(m6809.y.wl + 1); m6809_ICount[0] -= 1; break;
                    case 0x22: ea.wl = (ushort)(m6809.y.wl + 2); m6809_ICount[0] -= 1; break;
                    case 0x23: ea.wl = (ushort)(m6809.y.wl + 3); m6809_ICount[0] -= 1; break;
                    case 0x24: ea.wl = (ushort)(m6809.y.wl + 4); m6809_ICount[0] -= 1; break;
                    case 0x25: ea.wl = (ushort)(m6809.y.wl + 5); m6809_ICount[0] -= 1; break;
                    case 0x26: ea.wl = (ushort)(m6809.y.wl + 6); m6809_ICount[0] -= 1; break;
                    case 0x27: ea.wl = (ushort)(m6809.y.wl + 7); m6809_ICount[0] -= 1; break;
                    case 0x28: ea.wl = (ushort)(m6809.y.wl + 8); m6809_ICount[0] -= 1; break;
                    case 0x29: ea.wl = (ushort)(m6809.y.wl + 9); m6809_ICount[0] -= 1; break;
                    case 0x2a: ea.wl = (ushort)(m6809.y.wl + 10); m6809_ICount[0] -= 1; break;
                    case 0x2b: ea.wl = (ushort)(m6809.y.wl + 11); m6809_ICount[0] -= 1; break;
                    case 0x2c: ea.wl = (ushort)(m6809.y.wl + 12); m6809_ICount[0] -= 1; break;
                    case 0x2d: ea.wl = (ushort)(m6809.y.wl + 13); m6809_ICount[0] -= 1; break;
                    case 0x2e: ea.wl = (ushort)(m6809.y.wl + 14); m6809_ICount[0] -= 1; break;
                    case 0x2f: ea.wl = (ushort)(m6809.y.wl + 15); m6809_ICount[0] -= 1; break;

                    case 0x30: ea.wl = (ushort)(m6809.y.wl - 16); m6809_ICount[0] -= 1; break;
                    case 0x31: ea.wl = (ushort)(m6809.y.wl - 15); m6809_ICount[0] -= 1; break;
                    case 0x32: ea.wl = (ushort)(m6809.y.wl - 14); m6809_ICount[0] -= 1; break;
                    case 0x33: ea.wl = (ushort)(m6809.y.wl - 13); m6809_ICount[0] -= 1; break;
                    case 0x34: ea.wl = (ushort)(m6809.y.wl - 12); m6809_ICount[0] -= 1; break;
                    case 0x35: ea.wl = (ushort)(m6809.y.wl - 11); m6809_ICount[0] -= 1; break;
                    case 0x36: ea.wl = (ushort)(m6809.y.wl - 10); m6809_ICount[0] -= 1; break;
                    case 0x37: ea.wl = (ushort)(m6809.y.wl - 9); m6809_ICount[0] -= 1; break;
                    case 0x38: ea.wl = (ushort)(m6809.y.wl - 8); m6809_ICount[0] -= 1; break;
                    case 0x39: ea.wl = (ushort)(m6809.y.wl - 7); m6809_ICount[0] -= 1; break;
                    case 0x3a: ea.wl = (ushort)(m6809.y.wl - 6); m6809_ICount[0] -= 1; break;
                    case 0x3b: ea.wl = (ushort)(m6809.y.wl - 5); m6809_ICount[0] -= 1; break;
                    case 0x3c: ea.wl = (ushort)(m6809.y.wl - 4); m6809_ICount[0] -= 1; break;
                    case 0x3d: ea.wl = (ushort)(m6809.y.wl - 3); m6809_ICount[0] -= 1; break;
                    case 0x3e: ea.wl = (ushort)(m6809.y.wl - 2); m6809_ICount[0] -= 1; break;
                    case 0x3f: ea.wl = (ushort)(m6809.y.wl - 1); m6809_ICount[0] -= 1; break;

                    case 0x40: ea.wl = (ushort)(m6809.u.wl); m6809_ICount[0] -= 1; break;
                    case 0x41: ea.wl = (ushort)(m6809.u.wl + 1); m6809_ICount[0] -= 1; break;
                    case 0x42: ea.wl = (ushort)(m6809.u.wl + 2); m6809_ICount[0] -= 1; break;
                    case 0x43: ea.wl = (ushort)(m6809.u.wl + 3); m6809_ICount[0] -= 1; break;
                    case 0x44: ea.wl = (ushort)(m6809.u.wl + 4); m6809_ICount[0] -= 1; break;
                    case 0x45: ea.wl = (ushort)(m6809.u.wl + 5); m6809_ICount[0] -= 1; break;
                    case 0x46: ea.wl = (ushort)(m6809.u.wl + 6); m6809_ICount[0] -= 1; break;
                    case 0x47: ea.wl = (ushort)(m6809.u.wl + 7); m6809_ICount[0] -= 1; break;
                    case 0x48: ea.wl = (ushort)(m6809.u.wl + 8); m6809_ICount[0] -= 1; break;
                    case 0x49: ea.wl = (ushort)(m6809.u.wl + 9); m6809_ICount[0] -= 1; break;
                    case 0x4a: ea.wl = (ushort)(m6809.u.wl + 10); m6809_ICount[0] -= 1; break;
                    case 0x4b: ea.wl = (ushort)(m6809.u.wl + 11); m6809_ICount[0] -= 1; break;
                    case 0x4c: ea.wl = (ushort)(m6809.u.wl + 12); m6809_ICount[0] -= 1; break;
                    case 0x4d: ea.wl = (ushort)(m6809.u.wl + 13); m6809_ICount[0] -= 1; break;
                    case 0x4e: ea.wl = (ushort)(m6809.u.wl + 14); m6809_ICount[0] -= 1; break;
                    case 0x4f: ea.wl = (ushort)(m6809.u.wl + 15); m6809_ICount[0] -= 1; break;

                    case 0x50: ea.wl = (ushort)(m6809.u.wl - 16); m6809_ICount[0] -= 1; break;
                    case 0x51: ea.wl = (ushort)(m6809.u.wl - 15); m6809_ICount[0] -= 1; break;
                    case 0x52: ea.wl = (ushort)(m6809.u.wl - 14); m6809_ICount[0] -= 1; break;
                    case 0x53: ea.wl = (ushort)(m6809.u.wl - 13); m6809_ICount[0] -= 1; break;
                    case 0x54: ea.wl = (ushort)(m6809.u.wl - 12); m6809_ICount[0] -= 1; break;
                    case 0x55: ea.wl = (ushort)(m6809.u.wl - 11); m6809_ICount[0] -= 1; break;
                    case 0x56: ea.wl = (ushort)(m6809.u.wl - 10); m6809_ICount[0] -= 1; break;
                    case 0x57: ea.wl = (ushort)(m6809.u.wl - 9); m6809_ICount[0] -= 1; break;
                    case 0x58: ea.wl = (ushort)(m6809.u.wl - 8); m6809_ICount[0] -= 1; break;
                    case 0x59: ea.wl = (ushort)(m6809.u.wl - 7); m6809_ICount[0] -= 1; break;
                    case 0x5a: ea.wl = (ushort)(m6809.u.wl - 6); m6809_ICount[0] -= 1; break;
                    case 0x5b: ea.wl = (ushort)(m6809.u.wl - 5); m6809_ICount[0] -= 1; break;
                    case 0x5c: ea.wl = (ushort)(m6809.u.wl - 4); m6809_ICount[0] -= 1; break;
                    case 0x5d: ea.wl = (ushort)(m6809.u.wl - 3); m6809_ICount[0] -= 1; break;
                    case 0x5e: ea.wl = (ushort)(m6809.u.wl - 2); m6809_ICount[0] -= 1; break;
                    case 0x5f: ea.wl = (ushort)(m6809.u.wl - 1); m6809_ICount[0] -= 1; break;

                    case 0x60: ea.wl = (ushort)(m6809.s.wl); m6809_ICount[0] -= 1; break;
                    case 0x61: ea.wl = (ushort)(m6809.s.wl + 1); m6809_ICount[0] -= 1; break;
                    case 0x62: ea.wl = (ushort)(m6809.s.wl + 2); m6809_ICount[0] -= 1; break;
                    case 0x63: ea.wl = (ushort)(m6809.s.wl + 3); m6809_ICount[0] -= 1; break;
                    case 0x64: ea.wl = (ushort)(m6809.s.wl + 4); m6809_ICount[0] -= 1; break;
                    case 0x65: ea.wl = (ushort)(m6809.s.wl + 5); m6809_ICount[0] -= 1; break;
                    case 0x66: ea.wl = (ushort)(m6809.s.wl + 6); m6809_ICount[0] -= 1; break;
                    case 0x67: ea.wl = (ushort)(m6809.s.wl + 7); m6809_ICount[0] -= 1; break;
                    case 0x68: ea.wl = (ushort)(m6809.s.wl + 8); m6809_ICount[0] -= 1; break;
                    case 0x69: ea.wl = (ushort)(m6809.s.wl + 9); m6809_ICount[0] -= 1; break;
                    case 0x6a: ea.wl = (ushort)(m6809.s.wl + 10); m6809_ICount[0] -= 1; break;
                    case 0x6b: ea.wl = (ushort)(m6809.s.wl + 11); m6809_ICount[0] -= 1; break;
                    case 0x6c: ea.wl = (ushort)(m6809.s.wl + 12); m6809_ICount[0] -= 1; break;
                    case 0x6d: ea.wl = (ushort)(m6809.s.wl + 13); m6809_ICount[0] -= 1; break;
                    case 0x6e: ea.wl = (ushort)(m6809.s.wl + 14); m6809_ICount[0] -= 1; break;
                    case 0x6f: ea.wl = (ushort)(m6809.s.wl + 15); m6809_ICount[0] -= 1; break;

                    case 0x70: ea.wl = (ushort)(m6809.s.wl - 16); m6809_ICount[0] -= 1; break;
                    case 0x71: ea.wl = (ushort)(m6809.s.wl - 15); m6809_ICount[0] -= 1; break;
                    case 0x72: ea.wl = (ushort)(m6809.s.wl - 14); m6809_ICount[0] -= 1; break;
                    case 0x73: ea.wl = (ushort)(m6809.s.wl - 13); m6809_ICount[0] -= 1; break;
                    case 0x74: ea.wl = (ushort)(m6809.s.wl - 12); m6809_ICount[0] -= 1; break;
                    case 0x75: ea.wl = (ushort)(m6809.s.wl - 11); m6809_ICount[0] -= 1; break;
                    case 0x76: ea.wl = (ushort)(m6809.s.wl - 10); m6809_ICount[0] -= 1; break;
                    case 0x77: ea.wl = (ushort)(m6809.s.wl - 9); m6809_ICount[0] -= 1; break;
                    case 0x78: ea.wl = (ushort)(m6809.s.wl - 8); m6809_ICount[0] -= 1; break;
                    case 0x79: ea.wl = (ushort)(m6809.s.wl - 7); m6809_ICount[0] -= 1; break;
                    case 0x7a: ea.wl = (ushort)(m6809.s.wl - 6); m6809_ICount[0] -= 1; break;
                    case 0x7b: ea.wl = (ushort)(m6809.s.wl - 5); m6809_ICount[0] -= 1; break;
                    case 0x7c: ea.wl = (ushort)(m6809.s.wl - 4); m6809_ICount[0] -= 1; break;
                    case 0x7d: ea.wl = (ushort)(m6809.s.wl - 3); m6809_ICount[0] -= 1; break;
                    case 0x7e: ea.wl = (ushort)(m6809.s.wl - 2); m6809_ICount[0] -= 1; break;
                    case 0x7f: ea.wl = (ushort)(m6809.s.wl - 1); m6809_ICount[0] -= 1; break;

                    case 0x80: ea.wl = m6809.x.wl; m6809.x.wl++; m6809_ICount[0] -= 2; break;
                    case 0x81: ea.wl = m6809.x.wl; m6809.x.wl += 2; m6809_ICount[0] -= 3; break;
                    case 0x82: m6809.x.wl--; ea.wl = m6809.x.wl; m6809_ICount[0] -= 2; break;
                    case 0x83: m6809.x.wl -= 2; ea.wl = m6809.x.wl; m6809_ICount[0] -= 3; break;
                    case 0x84: ea.wl = m6809.x.wl; break;
                    case 0x85: ea.wl = (ushort)(m6809.x.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); m6809_ICount[0] -= 1; break;
                    case 0x86: ea.wl = (ushort)(m6809.x.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); m6809_ICount[0] -= 1; break;
                    case 0x87: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x88: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.x.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl))); m6809_ICount[0] -= 1; break; /* this is a hack to make Vectrex work. It should be m6809_ICount[0]-=1. Dunno where the cycle was lost :( */
                    case 0x89: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.x.wl; m6809_ICount[0] -= 4; break;
                    case 0x8a: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x8b: ea.wl = (ushort)(m6809.x.wl + m6809.d.wl); m6809_ICount[0] -= 4; break;
                    case 0x8c: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl))); m6809_ICount[0] -= 1; break;
                    case 0x8d: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl; m6809_ICount[0] -= 5; break;
                    case 0x8e: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x8f: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; m6809_ICount[0] -= 5; break;

                    case 0x90: ea.wl = m6809.x.wl; m6809.x.wl++; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break; /* Indirect ,R+ not in my specs */
                    case 0x91: ea.wl = m6809.x.wl; m6809.x.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0x92: m6809.x.wl--; ea.wl = m6809.x.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0x93: m6809.x.wl -= 2; ea.wl = m6809.x.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0x94: ea.wl = m6809.x.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 3; break;
                    case 0x95: ea.wl = (ushort)(m6809.x.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0x96: ea.wl = (ushort)(m6809.x.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0x97: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x98: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.x.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0x99: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.x.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0x9a: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x9b: ea.wl = (ushort)(m6809.x.wl + m6809.d.wl); ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0x9c: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0x9d: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;
                    case 0x9e: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0x9f: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;

                    case 0xa0: ea.wl = m6809.y.wl; m6809.y.wl++; m6809_ICount[0] -= 2; break;
                    case 0xa1: ea.wl = m6809.y.wl; m6809.y.wl += 2; m6809_ICount[0] -= 3; break;
                    case 0xa2: m6809.y.wl--; ea.wl = m6809.y.wl; m6809_ICount[0] -= 2; break;
                    case 0xa3: m6809.y.wl -= 2; ea.wl = m6809.y.wl; m6809_ICount[0] -= 3; break;
                    case 0xa4: ea.wl = m6809.y.wl; break;
                    case 0xa5: ea.wl = (ushort)(m6809.y.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); m6809_ICount[0] -= 1; break;
                    case 0xa6: ea.wl = (ushort)(m6809.y.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); m6809_ICount[0] -= 1; break;
                    case 0xa7: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xa8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.y.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        m6809_ICount[0] -= 1; break;
                    case 0xa9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.y.wl;
                        m6809_ICount[0] -= 4; break;
                    case 0xaa: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xab: ea.wl = (ushort)(m6809.y.wl + m6809.d.wl); m6809_ICount[0] -= 4; break;
                    case 0xac: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        m6809_ICount[0] -= 1; break;
                    case 0xad: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl;
                        m6809_ICount[0] -= 5; break;
                    case 0xae: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xaf: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; m6809_ICount[0] -= 5; break;

                    case 0xb0: ea.wl = m6809.y.wl; m6809.y.wl++; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xb1: ea.wl = m6809.y.wl; m6809.y.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xb2: m6809.y.wl--; ea.wl = m6809.y.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xb3: m6809.y.wl -= 2; ea.wl = m6809.y.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xb4: ea.wl = m6809.y.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 3; break;
                    case 0xb5: ea.wl = (ushort)(m6809.y.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xb6: ea.wl = (ushort)(m6809.y.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xb7: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xb8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.y.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xb9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.y.wl;
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xba: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xbb: ea.wl = (ushort)(m6809.y.wl + m6809.d.wl); ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xbc: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xbd: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl;
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;
                    case 0xbe: ea.wl = 0; break; /*   ILLEGAL*/
                    case 0xbf: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.d = RM16(ea.d);
                        m6809_ICount[0] -= 8; break;

                    case 0xc0: ea.wl = m6809.u.wl; m6809.u.wl++; m6809_ICount[0] -= 2; break;
                    case 0xc1: ea.wl = m6809.u.wl; m6809.u.wl += 2; m6809_ICount[0] -= 3; break;
                    case 0xc2: m6809.u.wl--; ea.wl = m6809.u.wl; m6809_ICount[0] -= 2; break;
                    case 0xc3: m6809.u.wl -= 2; ea.wl = m6809.u.wl; m6809_ICount[0] -= 3; break;
                    case 0xc4: ea.wl = m6809.u.wl; break;
                    case 0xc5: ea.wl = (ushort)(m6809.u.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); m6809_ICount[0] -= 1; break;
                    case 0xc6: ea.wl = (ushort)(m6809.u.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); m6809_ICount[0] -= 1; break;
                    case 0xc7: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xc8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.u.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        m6809_ICount[0] -= 1; break;
                    case 0xc9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.u.wl;
                        m6809_ICount[0] -= 4; break;
                    case 0xca: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xcb: ea.wl = (ushort)(m6809.u.wl + m6809.d.wl); m6809_ICount[0] -= 4; break;
                    case 0xcc: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        m6809_ICount[0] -= 1; break;
                    case 0xcd: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl;
                        m6809_ICount[0] -= 5; break;
                    case 0xce: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xcf: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; m6809_ICount[0] -= 5; break;

                    case 0xd0: ea.wl = m6809.u.wl; m6809.u.wl++; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xd1: ea.wl = m6809.u.wl; m6809.u.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xd2: m6809.u.wl--; ea.wl = m6809.u.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xd3: m6809.u.wl -= 2; ea.wl = m6809.u.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xd4: ea.wl = m6809.u.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 3; break;
                    case 0xd5: ea.wl = (ushort)(m6809.u.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xd6: ea.wl = (ushort)(m6809.u.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xd7: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xd8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.u.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xd9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.u.wl;
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xda: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xdb: ea.wl = (ushort)(m6809.u.wl + m6809.d.wl); ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xdc: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xdd: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl;
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;
                    case 0xde: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xdf: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;

                    case 0xe0: ea.wl = m6809.s.wl; m6809.s.wl++; m6809_ICount[0] -= 2; break;
                    case 0xe1: ea.wl = m6809.s.wl; m6809.s.wl += 2; m6809_ICount[0] -= 3; break;
                    case 0xe2: m6809.s.wl--; ea.wl = m6809.s.wl; m6809_ICount[0] -= 2; break;
                    case 0xe3: m6809.s.wl -= 2; ea.wl = m6809.s.wl; m6809_ICount[0] -= 3; break;
                    case 0xe4: ea.wl = m6809.s.wl; break;
                    case 0xe5: ea.wl = (ushort)(m6809.s.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); m6809_ICount[0] -= 1; break;
                    case 0xe6: ea.wl = (ushort)(m6809.s.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); m6809_ICount[0] -= 1; break;
                    case 0xe7: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xe8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.s.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        m6809_ICount[0] -= 1; break;
                    case 0xe9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                        m6809.pc.wl += 2; ea.wl += m6809.s.wl; m6809_ICount[0] -= 4; break;
                    case 0xea: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xeb: ea.wl = (ushort)(m6809.s.wl + m6809.d.wl); m6809_ICount[0] -= 4; break;
                    case 0xec: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++;
                        ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl))); m6809_ICount[0] -= 1; break;
                    case 0xed: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.pc.wl;
                        m6809_ICount[0] -= 5; break;
                    case 0xee: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xef: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; m6809_ICount[0] -= 5; break;

                    case 0xf0: ea.wl = m6809.s.wl; m6809.s.wl++; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xf1: ea.wl = m6809.s.wl; m6809.s.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xf2: m6809.s.wl--; ea.wl = m6809.s.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 5; break;
                    case 0xf3: m6809.s.wl -= 2; ea.wl = m6809.s.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 6; break;
                    case 0xf4: ea.wl = m6809.s.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 3; break;
                    case 0xf5: ea.wl = (ushort)(m6809.s.wl + ((ushort)((m6809.d.bl & 0x80) != 0 ? m6809.d.bl | 0xff00 : m6809.d.bl))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xf6: ea.wl = (ushort)(m6809.s.wl + ((ushort)((m6809.d.bh & 0x80) != 0 ? m6809.d.bh | 0xff00 : m6809.d.bh))); ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xf7: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xf8: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.s.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 4; break;
                    case 0xf9: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff))); m6809.pc.wl += 2; ea.wl += m6809.s.wl;
                        ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xfa: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xfb: ea.wl = (ushort)(m6809.s.wl + m6809.d.wl); ea.d = RM16(ea.d); m6809_ICount[0] -= 7; break;
                    case 0xfc: ea.wl = (cpu_readop_arg(m6809.pc.d)); m6809.pc.wl++; ea.wl = (ushort)(m6809.pc.wl + ((ushort)((ea.wl & 0x80) != 0 ? ea.wl | 0xff00 : ea.wl)));

                        m6809.pc.wl += 2; ea.wl += m6809.pc.wl; ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;
                    case 0xfe: ea.wl = 0; break; /*ILLEGAL*/
                    case 0xff: ea.d = (uint)(((cpu_readop_arg(m6809.pc.d)) << 8) | (cpu_readop_arg((m6809.pc.d + 1) & 0xffff)));
                        m6809.pc.wl += 2; ea.d = RM16(ea.d); m6809_ICount[0] -= 8; break;
                }
            }
        }
    }
}