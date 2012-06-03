using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_i8085 : cpu_interface
        {
            int[] i8085_ICount = new int[1];

            const int
    I8085_PC = 1, I8085_SP = 2, I8085_AF = 3, I8085_BC = 4, I8085_DE = 5, I8085_HL = 6,
    I8085_HALT = 7, I8085_IM = 8, I8085_IREQ = 9, I8085_ISRV = 10, I8085_VECTOR = 11,
    I8085_TRAP_STATE = 12, I8085_INTR_STATE = 13,
    I8085_RST55_STATE = 14, I8085_RST65_STATE = 15, I8085_RST75_STATE = 16;

            const int I8085_INTR_LINE = 0;
            const int I8085_RST55_LINE = 1;
            const int I8085_RST65_LINE = 2;
            const int I8085_RST75_LINE = 3;

            const int I8085_NONE = 0;
            const int I8085_TRAP = 0x01;
            const int I8085_RST55 = 0x02;
            const int I8085_RST65 = 0x04;
            const int I8085_RST75 = 0x08;
            const int I8085_SID = 0x10;
            const int I8085_INTR = 0xff;

            const int I8080_PC = I8085_PC;
            const int I8080_SP = I8085_SP;
            const int I8080_BC = I8085_BC;
            const int I8080_DE = I8085_DE;
            const int I8080_HL = I8085_HL;
            const int I8080_AF = I8085_AF;
            const int I8080_HALT = I8085_HALT;
            const int I8080_IREQ = I8085_IREQ;
            const int I8080_ISRV = I8085_ISRV;
            const int I8080_VECTOR = I8085_VECTOR;
            const int I8080_TRAP_STATE = I8085_TRAP_STATE;
            const int I8080_INTR_STATE = I8085_INTR_STATE;

            const int I8080_INTR_LINE = I8085_INTR_LINE;
            const int I8080_TRAP = I8085_TRAP;
            public const int I8080_INTR = I8085_INTR;
            const int I8080_NONE = I8085_NONE;


            const byte SF = 0x80;
            const byte ZF = 0x40;
            const byte YF = 0x20;
            const byte HF = 0x10;
            const byte XF = 0x08;
            const byte VF = 0x04;
            const byte NF = 0x02;
            const byte CF = 0x01;

            const byte IM_SID = 0x80;
            const byte IM_SOD = 0x40;
            const byte IM_IEN = 0x20;
            const byte IM_TRAP = 0x10;
            const byte IM_INTR = 0x08;
            const byte IM_RST75 = 0x04;
            const byte IM_RST65 = 0x02;
            const byte IM_RST55 = 0x01;

            const int ADDR_TRAP = 0x0024;
            const int ADDR_RST55 = 0x002c;
            const int ADDR_RST65 = 0x0034;
            const int ADDR_RST75 = 0x003c;
            const int ADDR_INTR = 0x0038;

            public cpu_i8085()
            {
                cpu_num = CPU_8080;
                num_irqs = 4;
                default_vector = 255;
                overclock = 1.0;
                no_int = I8080_NONE;
                irq_int = I8080_INTR;
                nmi_int = I8080_TRAP;
                address_shift = 0;
                address_bits = 16;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 3;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = i8085_ICount;

                //SetupOpCodeLists();
            }
            delegate int _irq_callback(int i);
            delegate void _sod_callback(int state);
            class i8085_Regs
            {
                public int cputype;
                public PAIR PC, SP, AF, BC, DE, HL, XX;
                public byte HALT;
                public byte IM;
                public byte IREQ;
                public byte ISRV;
                public uint INTR;
                public uint IRQ2;
                public uint IRQ1;
                public sbyte nmi_state;
                public sbyte[] irq_state = new sbyte[4];
                public irqcallback irq_callback;
                public _sod_callback sod_callback;
            }
            static i8085_Regs I=new i8085_Regs();
            byte[] ZS = new byte[256];
            byte[] ZSP = new byte[256];

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
                    case CPU_INFO_NAME: return "8085A";
                    case CPU_INFO_FAMILY: return "Intel 8080";
                    case CPU_INFO_VERSION: return "1.1";
                    case CPU_INFO_FILE: return "cpu_i8085.cs";
                    case CPU_INFO_CREDITS: return "Copyright (c) 1999 Juergen Buchmueller, all rights reserved.";
                    default: throw new Exception();
                }
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
                reg = new i8085_Regs();
                //throw new NotImplementedException();
            }
            public override uint get_context(ref object reg)
            {
                return i8085_get_context(ref reg);
            }
            public override void exit()
            {
                //nothing
            }
            public override uint get_pc()
            {
                return I.PC.d;
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
                init_tables();
                //memset(&I, 0, sizeof(i8085_Regs));
                I.cputype = 1;
                change_pc16(I.PC.d);
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                I.irq_callback = callback;
            }
            void i8085_set_INTR(int state)
            {
                //LOG((errorlog, "i8085: INTR %d\n", state));
                if (state != 0)
                {
                    I.IREQ |= IM_INTR;				/* request INTR */
                    I.INTR = (uint)state;
                    if ((I.IM & IM_INTR) != 0) return;	/* if masked, ignore it for now */
                    if (I.ISRV == 0)					/* if no higher priority IREQ is serviced */
                    {
                        I.ISRV = IM_INTR;			/* service INTR */
                        I.IRQ2 = I.INTR;
                    }
                }
                else
                {
                    I.IREQ &= unchecked((byte)~IM_INTR); 			/* remove request for INTR */
                }
            }
            void i8085_set_RST55(int state)
            {
                //LOG((errorlog, "i8085: RST5.5 %d\n", state));
                if (state != 0)
                {
                    I.IREQ |= IM_RST55; 			/* request RST5.5 */
                    if ((I.IM & IM_RST55) != 0) return;	/* if masked, ignore it for now */
                    if (I.ISRV == 0)					/* if no higher priority IREQ is serviced */
                    {
                        I.ISRV = IM_RST55;			/* service RST5.5 */
                        I.IRQ2 = ADDR_RST55;
                    }
                }
                else
                {
                    I.IREQ &= unchecked((byte)~IM_RST55);			/* remove request for RST5.5 */
                }
            }
            void i8085_set_RST65(int state)
            {
                //LOG((errorlog, "i8085: RST6.5 %d\n", state));
                if (state != 0)
                {
                    I.IREQ |= IM_RST65; 			/* request RST6.5 */
                    if ((I.IM & IM_RST65) != 0) return;	/* if masked, ignore it for now */
                    if (I.ISRV == 0)					/* if no higher priority IREQ is serviced */
                    {
                        I.ISRV = IM_RST65;			/* service RST6.5 */
                        I.IRQ2 = ADDR_RST65;
                    }
                }
                else
                {
                    I.IREQ &= unchecked((byte)~IM_RST65);			/* remove request for RST6.5 */
                }
            }
            void i8085_set_RST75(int state)
            {
                //LOG((errorlog, "i8085: RST7.5 %d\n", state));
                if (state != 0)
                {

                    I.IREQ |= IM_RST75; 			/* request RST7.5 */
                    if ((I.IM & IM_RST75) != 0) return;	/* if masked, ignore it for now */
                    if (I.ISRV == 0)					/* if no higher priority IREQ is serviced */
                    {
                        I.ISRV = IM_RST75;			/* service RST7.5 */
                        I.IRQ2 = ADDR_RST75;
                    }
                }
                /* RST7.5 is reset only by SIM or end of service routine ! */
            }
            public override void set_irq_line(int irqline, int state)
            {
                I.irq_state[irqline] = (sbyte)state;
                if (state == CLEAR_LINE)
                {
                    if ((I.IM & IM_IEN) == 0)
                    {
                        switch (irqline)
                        {
                            case I8085_INTR_LINE: i8085_set_INTR(0); break;
                            case I8085_RST55_LINE: i8085_set_RST55(0); break;
                            case I8085_RST65_LINE: i8085_set_RST65(0); break;
                            case I8085_RST75_LINE: i8085_set_RST75(0); break;
                        }
                    }
                }
                else
                {
                    if ((I.IM & IM_IEN) != 0)
                    {
                        switch (irqline)
                        {
                            case I8085_INTR_LINE: i8085_set_INTR(1); break;
                            case I8085_RST55_LINE: i8085_set_RST55(1); break;
                            case I8085_RST65_LINE: i8085_set_RST65(1); break;
                            case I8085_RST75_LINE: i8085_set_RST75(1); break;
                        }
                    }
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
            public override int execute(int cycles)
            {
                i8085_ICount[0] = cycles;
                do
                {
                    /* interrupts enabled or TRAP pending ? */
                    if ((I.IM & IM_IEN) != 0 || (I.IREQ & IM_TRAP) != 0)
                    {
                        /* copy scheduled to executed interrupt request */
                        I.IRQ1 = I.IRQ2;
                        /* reset scheduled interrupt request */
                        I.IRQ2 = 0;
                        /* interrupt now ? */
                        if (I.IRQ1 != 0) Interrupt();
                    }

                    /* here we go... */
                    execute_one(ROP());

                } while (i8085_ICount[0] > 0);

                return cycles - i8085_ICount[0];
            }
            uint i8085_get_context(ref object dst)
            {
                dst = I;
                return 1;
            }

            static byte ROP()
            {
                return cpu_readop(I.PC.wl++);
            }
            static byte ARG()
            {
                return cpu_readop_arg(I.PC.wl++);
            }
            static ushort ARG16()
            {
                ushort w;
                w = cpu_readop_arg(I.PC.d);
                I.PC.wl++;
                w += (ushort)(cpu_readop_arg(I.PC.d) << 8);
                I.PC.wl++;
                return w;
            }
            static byte RM(uint a)
            {
                return (byte)cpu_readmem16((int)a);
            }
            static void WM(uint a, byte v)
            {
                cpu_writemem16((int)a, v);
            }
            void init_tables()
            {
                byte zs;
                int i, p;
                for (i = 0; i < 256; i++)
                {
                    zs = 0;
                    if (i == 0) zs |= ZF;
                    if ((i & 128) != 0) zs |= SF;
                    p = 0;
                    if ((i & 1) != 0) ++p;
                    if ((i & 2) != 0) ++p;
                    if ((i & 4) != 0) ++p;
                    if ((i & 8) != 0) ++p;
                    if ((i & 16) != 0) ++p;
                    if ((i & 32) != 0) ++p;
                    if ((i & 64) != 0) ++p;
                    if ((i & 128) != 0) ++p;
                    ZS[i] = zs;
                    ZSP[i] = (byte)(zs | ((p & 1) != 0 ? 0 : VF));
                }
            }
            void Interrupt()
            {

                if (I.HALT != 0)		/* if the CPU was halted */
                {
                    I.PC.wl++; 	/* skip HALT instr */
                    I.HALT = 0;
                }
                I.IM &= unchecked((byte)~IM_IEN);		/* remove general interrupt enable bit */

                if (I.ISRV == IM_INTR)
                {
                    //LOG((errorlog,"Interrupt get INTR vector\n"));
                    I.IRQ1 = (uint)I.irq_callback(0);
                }

                if (I.cputype != 0)
                {
                    if (I.ISRV == IM_RST55)
                    {
                        //LOG((errorlog,"Interrupt get RST5.5 vector\n"));
                        I.IRQ1 = (uint)I.irq_callback(1);
                    }

                    if (I.ISRV == IM_RST65)
                    {
                        //LOG((errorlog,"Interrupt get RST6.5 vector\n"));
                        I.IRQ1 = (uint)I.irq_callback(2);
                    }

                    if (I.ISRV == IM_RST75)
                    {
                        //LOG((errorlog,"Interrupt get RST7.5 vector\n"));
                        I.IRQ1 = (uint)I.irq_callback(3);
                    }
                }

                switch (I.IRQ1 & 0xff0000)
                {
                    case 0xcd0000:	/* CALL nnnn */
                        i8085_ICount[0] -= 7;
                        //M_PUSH(PC);
                        WM(--I.SP.wl, I.PC.bh);
                        WM(--I.SP.wl, I.PC.bl);
                        goto case 0xc30000;
                    case 0xc30000:	/* JMP	nnnn */
                        i8085_ICount[0] -= 10;
                        I.PC.d = I.IRQ1 & 0xffff;
                        change_pc16(I.PC.d);
                        break;
                    default:
                        switch (I.IRQ1)
                        {
                            case I8085_TRAP:
                            case I8085_RST75:
                            case I8085_RST65:
                            case I8085_RST55:
                                //M_PUSH(PC);
                                WM(--I.SP.wl, I.PC.bh);
                                WM(--I.SP.wl, I.PC.bl);

                                if (I.IRQ1 != I8085_RST75)
                                    I.PC.d = I.IRQ1;
                                else
                                    I.PC.d = 0x3c;
                                change_pc16(I.PC.d);
                                break;
                            default:
                                //LOG((errorlog, "i8085 take int $%02x\n", I.IRQ1));
                                execute_one((int)(I.IRQ1 & 0xff));
                                break;
                        }
                        break;
                }
            }
            void execute_one(int opcode)
            {
                switch (opcode)
                {
                    case 0x00: i8085_ICount[0] -= 4; /* NOP	*/
                        /* no op */
                        break;
                    case 0x01: i8085_ICount[0] -= 10; /* LXI	B,nnnn */
                        I.BC.wl = ARG16();
                        break;
                    case 0x02: i8085_ICount[0] -= 7; /* STAX B */
                        WM(I.BC.d, I.AF.bh);
                        break;
                    case 0x03: i8085_ICount[0] -= 6; /* INX	B */
                        I.BC.wl++;
                        break;
                    case 0x04: i8085_ICount[0] -= 4; /* INR	B */
                        ++I.BC.bh; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.BC.bh] | ((I.BC.bh == 0x80) ? 0x04 : 0) | ((I.BC.bh & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x05: i8085_ICount[0] -= 4; /* DCR	B */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.BC.bh == 0x80) ? 0x04 : 0) | ((I.BC.bh & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.BC.bh];
                        break;
                    case 0x06: i8085_ICount[0] -= 7; /* MVI	B,nn */
                        I.BC.bh = ARG();
                        break;
                    case 0x07: i8085_ICount[0] -= 4; /* RLC	*/
                        { I.AF.bh = (byte)((I.AF.bh << 1) | (I.AF.bh >> 7)); I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x02 + 0x01)) | (I.AF.bh & 0x01)); };
                        break;

                    case 0x08: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0x09: i8085_ICount[0] -= 11; /* DAD	B */
                        { int q = (int)(I.HL.d + I.BC.d); I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x01)) | (((I.HL.d ^ q ^ I.BC.d) >> 8) & 0x10) | ((q >> 16) & 0x01)); I.HL.wl = (ushort)q; };
                        break;
                    case 0x0a: i8085_ICount[0] -= 7; /* LDAX B */
                        I.AF.bh = RM(I.BC.d);
                        break;
                    case 0x0b: i8085_ICount[0] -= 6; /* DCX	B */
                        I.BC.wl--;
                        break;
                    case 0x0c: i8085_ICount[0] -= 4; /* INR	C */
                        ++I.BC.bl; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.BC.bl] | ((I.BC.bl == 0x80) ? 0x04 : 0) | ((I.BC.bl & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x0d: i8085_ICount[0] -= 4; /* DCR	C */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.BC.bl == 0x80) ? 0x04 : 0) | ((I.BC.bl & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.BC.bl];
                        break;
                    case 0x0e: i8085_ICount[0] -= 7; /* MVI	C,nn */
                        I.BC.bl = ARG();
                        break;
                    case 0x0f: i8085_ICount[0] -= 4; /* RRC	*/
                        { I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x02 + 0x01)) | (I.AF.bh & 0x01)); I.AF.bh = (byte)((I.AF.bh >> 1) | (I.AF.bh << 7)); };
                        break;

                    case 0x10: i8085_ICount[0] -= 8; /* ????  */
                        illegal();
                        break;
                    case 0x11: i8085_ICount[0] -= 10; /* LXI	D,nnnn */
                        I.DE.wl = ARG16();
                        break;
                    case 0x12: i8085_ICount[0] -= 7; /* STAX D */
                        WM(I.DE.d, I.AF.bh);
                        break;
                    case 0x13: i8085_ICount[0] -= 6; /* INX	D */
                        I.DE.wl++;
                        break;
                    case 0x14: i8085_ICount[0] -= 4; /* INR	D */
                        ++I.DE.bh; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.DE.bh] | ((I.DE.bh == 0x80) ? 0x04 : 0) | ((I.DE.bh & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x15: i8085_ICount[0] -= 4; /* DCR	D */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.DE.bh == 0x80) ? 0x04 : 0) | ((I.DE.bh & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.DE.bh];
                        break;
                    case 0x16: i8085_ICount[0] -= 7; /* MVI	D,nn */
                        I.DE.bh = ARG();
                        break;
                    case 0x17: i8085_ICount[0] -= 4; /* RAL	*/
                        { int c = I.AF.bl & 0x01; I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x02 + 0x01)) | (I.AF.bh >> 7)); I.AF.bh = (byte)((I.AF.bh << 1) | c); };
                        break;

                    case 0x18: i8085_ICount[0] -= 7; /* ????? */
                        illegal();
                        break;
                    case 0x19: i8085_ICount[0] -= 11; /* DAD	D */
                        { int q = (int)(I.HL.d + I.DE.d); I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x01)) | (((I.HL.d ^ q ^ I.DE.d) >> 8) & 0x10) | ((q >> 16) & 0x01)); I.HL.wl = (ushort)q; };
                        break;
                    case 0x1a: i8085_ICount[0] -= 7; /* LDAX D */
                        I.AF.bh = RM(I.DE.d);
                        break;
                    case 0x1b: i8085_ICount[0] -= 6; /* DCX	D */
                        I.DE.wl--;
                        break;
                    case 0x1c: i8085_ICount[0] -= 4; /* INR	E */
                        ++I.DE.bl; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.DE.bl] | ((I.DE.bl == 0x80) ? 0x04 : 0) | ((I.DE.bl & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x1d: i8085_ICount[0] -= 4; /* DCR	E */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.DE.bl == 0x80) ? 0x04 : 0) | ((I.DE.bl & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.DE.bl];
                        break;
                    case 0x1e: i8085_ICount[0] -= 7; /* MVI	E,nn */
                        I.DE.bl = ARG();
                        break;
                    case 0x1f: i8085_ICount[0] -= 4; /* RAR	*/
                        { int c = (I.AF.bl & 0x01) << 7; I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x02 + 0x01)) | (I.AF.bh & 0x01)); I.AF.bh = (byte)((I.AF.bh >> 1) | c); };
                        break;

                    case 0x20:
                        if (I.cputype != 0)
                        {
                            i8085_ICount[0] -= 7; /* RIM	*/
                            I.AF.bh = I.IM;
                        }
                        else
                        {
                            i8085_ICount[0] -= 7; /* ???	*/
                        }
                        break;
                    case 0x21: i8085_ICount[0] -= 10; /* LXI	H,nnnn */
                        I.HL.wl = ARG16();
                        break;
                    case 0x22: i8085_ICount[0] -= 16; /* SHLD nnnn */
                        I.XX.wl = ARG16();
                        WM(I.XX.d, I.HL.bl);
                        I.XX.wl++;
                        WM(I.XX.d, I.HL.bh);
                        break;
                    case 0x23: i8085_ICount[0] -= 6; /* INX	H */
                        I.HL.wl++;
                        break;
                    case 0x24: i8085_ICount[0] -= 4; /* INR	H */
                        ++I.HL.bh; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.HL.bh] | ((I.HL.bh == 0x80) ? 0x04 : 0) | ((I.HL.bh & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x25: i8085_ICount[0] -= 4; /* DCR	H */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.HL.bh == 0x80) ? 0x04 : 0) | ((I.HL.bh & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.HL.bh];
                        break;
                    case 0x26: i8085_ICount[0] -= 7; /* MVI	H,nn */
                        I.HL.bh = ARG();
                        break;
                    case 0x27: i8085_ICount[0] -= 4; /* DAA	*/
                        I.XX.d = I.AF.bh;
                        if ((I.AF.bl & 0x01) != 0) I.XX.d |= 0x100;
                        if ((I.AF.bl & 0x10) != 0) I.XX.d |= 0x200;
                        if ((I.AF.bl & 0x02) != 0) I.XX.d |= 0x400;
                        I.AF.wl = DAA[I.XX.d];
                        break;

                    case 0x28: i8085_ICount[0] -= 7; /* ???? */
                        illegal();
                        break;
                    case 0x29: i8085_ICount[0] -= 11; /* DAD	H */
                        { int q = (int)(I.HL.d + I.HL.d); I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x01)) | (((I.HL.d ^ q ^ I.HL.d) >> 8) & 0x10) | ((q >> 16) & 0x01)); I.HL.wl = (ushort)q; };
                        break;
                    case 0x2a: i8085_ICount[0] -= 16; /* LHLD nnnn */
                        I.XX.d = ARG16();
                        I.HL.bl = RM(I.XX.d);
                        I.XX.wl++;
                        I.HL.bh = RM(I.XX.d);
                        break;
                    case 0x2b: i8085_ICount[0] -= 6; /* DCX	H */
                        I.HL.wl--;
                        break;
                    case 0x2c: i8085_ICount[0] -= 4; /* INR	L */
                        ++I.HL.bl; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.HL.bl] | ((I.HL.bl == 0x80) ? 0x04 : 0) | ((I.HL.bl & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x2d: i8085_ICount[0] -= 4; /* DCR	L */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.HL.bl == 0x80) ? 0x04 : 0) | ((I.HL.bl & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.HL.bl];
                        break;
                    case 0x2e: i8085_ICount[0] -= 7; /* MVI	L,nn */
                        I.HL.bl = ARG();
                        break;
                    case 0x2f: i8085_ICount[0] -= 4; /* CMA	*/
                        I.AF.bh ^= 0xff;
                        I.AF.bl |= 0x10 + 0x02;
                        break;

                    case 0x30:
                        if (I.cputype != 0)
                        {
                            i8085_ICount[0] -= 7; /* SIM	*/
                            if (((I.IM ^ I.AF.bh) & 0x80) != 0)
                                if (I.sod_callback != null) I.sod_callback(I.AF.bh >> 7);
                            I.IM &= (0x80 + 0x20 + 0x10);
                            I.IM |= (byte)((I.AF.bh & ~(0x80 + 0x40 + 0x20 + 0x10)));
                            if ((I.AF.bh & 0x80) != 0) I.IM |= 0x40;
                        }
                        else
                        {
                            i8085_ICount[0] -= 4; /* ???	*/
                        }
                        break;
                    case 0x31: i8085_ICount[0] -= 10; /* LXI SP,nnnn */
                        I.SP.wl = ARG16();
                        break;
                    case 0x32: i8085_ICount[0] -= 13; /* STAX nnnn */
                        I.XX.d = ARG16();
                        WM(I.XX.d, I.AF.bh);
                        break;
                    case 0x33: i8085_ICount[0] -= 6; /* INX	SP */
                        I.SP.wl++;
                        break;
                    case 0x34: i8085_ICount[0] -= 11; /* INR	M */
                        I.XX.bl = RM(I.HL.d);
                        ++I.XX.bl; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.XX.bl] | ((I.XX.bl == 0x80) ? 0x04 : 0) | ((I.XX.bl & 0x0F) != 0 ? 0 : 0x10));
                        WM(I.HL.d, I.XX.bl);
                        break;
                    case 0x35: i8085_ICount[0] -= 11; /* DCR	M */
                        I.XX.bl = RM(I.HL.d);
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.XX.bl == 0x80) ? 0x04 : 0) | ((I.XX.bl & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.XX.bl];
                        WM(I.HL.d, I.XX.bl);
                        break;
                    case 0x36: i8085_ICount[0] -= 10; /* MVI	M,nn */
                        I.XX.bl = ARG();
                        WM(I.HL.d, I.XX.bl);
                        break;
                    case 0x37: i8085_ICount[0] -= 4; /* STC	*/
                        I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x02)) | 0x01);
                        break;

                    case 0x38: i8085_ICount[0] -= 7; /* ???? */
                        illegal();
                        break;
                    case 0x39: i8085_ICount[0] -= 11; /* DAD SP */
                        { int q = (int)(I.HL.d + I.SP.d); I.AF.bl = (byte)((I.AF.bl & ~(0x10 + 0x01)) | (((I.HL.d ^ q ^ I.SP.d) >> 8) & 0x10) | ((q >> 16) & 0x01)); I.HL.wl = (ushort)q; };
                        break;
                    case 0x3a: i8085_ICount[0] -= 13; /* LDAX nnnn */
                        I.XX.d = ARG16();
                        I.AF.bh = RM(I.XX.d);
                        break;
                    case 0x3b: i8085_ICount[0] -= 6; /* DCX	SP */
                        I.SP.wl--;
                        break;
                    case 0x3c: i8085_ICount[0] -= 4; /* INR	A */
                        ++I.AF.bh; I.AF.bl = (byte)((I.AF.bl & 0x01) | ZS[I.AF.bh] | ((I.AF.bh == 0x80) ? 0x04 : 0) | ((I.AF.bh & 0x0F) != 0 ? 0 : 0x10));
                        break;
                    case 0x3d: i8085_ICount[0] -= 4; /* DCR	A */
                        I.AF.bl = (byte)((I.AF.bl & 0x01) | 0x02 | ((I.AF.bh == 0x80) ? 0x04 : 0) | ((I.AF.bh & 0x0F) != 0 ? 0 : 0x10)); I.AF.bl |= ZS[--I.AF.bh];
                        break;
                    case 0x3e: i8085_ICount[0] -= 7; /* MVI	A,nn */
                        I.AF.bh = ARG();
                        break;
                    case 0x3f: i8085_ICount[0] -= 4; /* CMF	*/
                        I.AF.bl = (byte)(((I.AF.bl & ~(0x10 + 0x02)) | ((I.AF.bl & 0x01) << 4)) ^ 0x01);
                        break;

                    case 0x40: i8085_ICount[0] -= 4; /* MOV	B,B */
                        /* no op */
                        break;
                    case 0x41: i8085_ICount[0] -= 4; /* MOV	B,C */
                        I.BC.bh = I.BC.bl;
                        break;
                    case 0x42: i8085_ICount[0] -= 4; /* MOV	B,D */
                        I.BC.bh = I.DE.bh;
                        break;
                    case 0x43: i8085_ICount[0] -= 4; /* MOV	B,E */
                        I.BC.bh = I.DE.bl;
                        break;
                    case 0x44: i8085_ICount[0] -= 4; /* MOV	B,H */
                        I.BC.bh = I.HL.bh;
                        break;
                    case 0x45: i8085_ICount[0] -= 4; /* MOV	B,L */
                        I.BC.bh = I.HL.bl;
                        break;
                    case 0x46: i8085_ICount[0] -= 7; /* MOV	B,M */
                        I.BC.bh = RM(I.HL.d);
                        break;
                    case 0x47: i8085_ICount[0] -= 4; /* MOV	B,A */
                        I.BC.bh = I.AF.bh;
                        break;

                    case 0x48: i8085_ICount[0] -= 4; /* MOV  C,B */
                        I.BC.bl = I.BC.bh;
                        break;
                    case 0x49: i8085_ICount[0] -= 4; /* MOV	C,C */
                        /* no op */
                        break;
                    case 0x4a: i8085_ICount[0] -= 4; /* MOV	C,D */
                        I.BC.bl = I.DE.bh;
                        break;
                    case 0x4b: i8085_ICount[0] -= 4; /* MOV	C,E */
                        I.BC.bl = I.DE.bl;
                        break;
                    case 0x4c: i8085_ICount[0] -= 4; /* MOV	C,H */
                        I.BC.bl = I.HL.bh;
                        break;
                    case 0x4d: i8085_ICount[0] -= 4; /* MOV	C,L */
                        I.BC.bl = I.HL.bl;
                        break;
                    case 0x4e: i8085_ICount[0] -= 7; /* MOV	C,M */
                        I.BC.bl = RM(I.HL.d);
                        break;
                    case 0x4f: i8085_ICount[0] -= 4; /* MOV	C,A */
                        I.BC.bl = I.AF.bh;
                        break;

                    case 0x50: i8085_ICount[0] -= 4; /* MOV	D,B */
                        I.DE.bh = I.BC.bh;
                        break;
                    case 0x51: i8085_ICount[0] -= 4; /* MOV	D,C */
                        I.DE.bh = I.BC.bl;
                        break;
                    case 0x52: i8085_ICount[0] -= 4; /* MOV	D,D */
                        /* no op */
                        break;
                    case 0x53: i8085_ICount[0] -= 4; /* MOV	D,E */
                        I.DE.bh = I.DE.bl;
                        break;
                    case 0x54: i8085_ICount[0] -= 4; /* MOV	D,H */
                        I.DE.bh = I.HL.bh;
                        break;
                    case 0x55: i8085_ICount[0] -= 4; /* MOV	D,L */
                        I.DE.bh = I.HL.bl;
                        break;
                    case 0x56: i8085_ICount[0] -= 7; /* MOV	D,M */
                        I.DE.bh = RM(I.HL.d);
                        break;
                    case 0x57: i8085_ICount[0] -= 4; /* MOV	D,A */
                        I.DE.bh = I.AF.bh;
                        break;

                    case 0x58: i8085_ICount[0] -= 4; /* MOV	E,B */
                        I.DE.bl = I.BC.bh;
                        break;
                    case 0x59: i8085_ICount[0] -= 4; /* MOV	E,C */
                        I.DE.bl = I.BC.bl;
                        break;
                    case 0x5a: i8085_ICount[0] -= 4; /* MOV	E,D */
                        I.DE.bl = I.DE.bh;
                        break;
                    case 0x5b: i8085_ICount[0] -= 4; /* MOV	E,E */
                        /* no op */
                        break;
                    case 0x5c: i8085_ICount[0] -= 4; /* MOV	E,H */
                        I.DE.bl = I.HL.bh;
                        break;
                    case 0x5d: i8085_ICount[0] -= 4; /* MOV	E,L */
                        I.DE.bl = I.HL.bl;
                        break;
                    case 0x5e: i8085_ICount[0] -= 7; /* MOV	E,M */
                        I.DE.bl = RM(I.HL.d);
                        break;
                    case 0x5f: i8085_ICount[0] -= 4; /* MOV	E,A */
                        I.DE.bl = I.AF.bh;
                        break;

                    case 0x60: i8085_ICount[0] -= 4; /* MOV  H,B */
                        I.HL.bh = I.BC.bh;
                        break;
                    case 0x61: i8085_ICount[0] -= 4; /* MOV	H,C */
                        I.HL.bh = I.BC.bl;
                        break;
                    case 0x62: i8085_ICount[0] -= 4; /* MOV	H,D */
                        I.HL.bh = I.DE.bh;
                        break;
                    case 0x63: i8085_ICount[0] -= 4; /* MOV	H,E */
                        I.HL.bh = I.DE.bl;
                        break;
                    case 0x64: i8085_ICount[0] -= 4; /* MOV	H,H */
                        /* no op */
                        break;
                    case 0x65: i8085_ICount[0] -= 4; /* MOV	H,L */
                        I.HL.bh = I.HL.bl;
                        break;
                    case 0x66: i8085_ICount[0] -= 7; /* MOV	H,M */
                        I.HL.bh = RM(I.HL.d);
                        break;
                    case 0x67: i8085_ICount[0] -= 4; /* MOV	H,A */
                        I.HL.bh = I.AF.bh;
                        break;

                    case 0x68: i8085_ICount[0] -= 4; /* MOV	L,B */
                        I.HL.bl = I.BC.bh;
                        break;
                    case 0x69: i8085_ICount[0] -= 4; /* MOV	L,C */
                        I.HL.bl = I.BC.bl;
                        break;
                    case 0x6a: i8085_ICount[0] -= 4; /* MOV	L,D */
                        I.HL.bl = I.DE.bh;
                        break;
                    case 0x6b: i8085_ICount[0] -= 4; /* MOV	L,E */
                        I.HL.bl = I.DE.bl;
                        break;
                    case 0x6c: i8085_ICount[0] -= 4; /* MOV	L,H */
                        I.HL.bl = I.HL.bh;
                        break;
                    case 0x6d: i8085_ICount[0] -= 4; /* MOV	L,L */
                        /* no op */
                        break;
                    case 0x6e: i8085_ICount[0] -= 7; /* MOV	L,M */
                        I.HL.bl = RM(I.HL.d);
                        break;
                    case 0x6f: i8085_ICount[0] -= 4; /* MOV	L,A */
                        I.HL.bl = I.AF.bh;
                        break;

                    case 0x70: i8085_ICount[0] -= 7; /* MOV	M,B */
                        WM(I.HL.d, I.BC.bh);
                        break;
                    case 0x71: i8085_ICount[0] -= 7; /* MOV	M,C */
                        WM(I.HL.d, I.BC.bl);
                        break;
                    case 0x72: i8085_ICount[0] -= 7; /* MOV	M,D */
                        WM(I.HL.d, I.DE.bh);
                        break;
                    case 0x73: i8085_ICount[0] -= 7; /* MOV	M,E */
                        WM(I.HL.d, I.DE.bl);
                        break;
                    case 0x74: i8085_ICount[0] -= 7; /* MOV	M,H */
                        WM(I.HL.d, I.HL.bh);
                        break;
                    case 0x75: i8085_ICount[0] -= 7; /* MOV	M,L */
                        WM(I.HL.d, I.HL.bl);
                        break;
                    case 0x76: i8085_ICount[0] -= 4; /* HALT */
                        I.PC.wl--;
                        I.HALT = 1;
                        if (i8085_ICount[0] > 0) i8085_ICount[0] = 0;
                        break;
                    case 0x77: i8085_ICount[0] -= 7; /* MOV	M,A */
                        WM(I.HL.d, I.AF.bh);
                        break;

                    case 0x78: i8085_ICount[0] -= 4; /* MOV	A,B */
                        I.AF.bh = I.BC.bh;
                        break;
                    case 0x79: i8085_ICount[0] -= 4; /* MOV	A,C */
                        I.AF.bh = I.BC.bl;
                        break;
                    case 0x7a: i8085_ICount[0] -= 4; /* MOV	A,D */
                        I.AF.bh = I.DE.bh;
                        break;
                    case 0x7b: i8085_ICount[0] -= 4; /* MOV	A,E */
                        I.AF.bh = I.DE.bl;
                        break;
                    case 0x7c: i8085_ICount[0] -= 4; /* MOV	A,H */
                        I.AF.bh = I.HL.bh;
                        break;
                    case 0x7d: i8085_ICount[0] -= 4; /* MOV	A,L */
                        I.AF.bh = I.HL.bl;
                        break;
                    case 0x7e: i8085_ICount[0] -= 7; /* MOV	A,M */
                        I.AF.bh = RM(I.HL.d);
                        break;
                    case 0x7f: i8085_ICount[0] -= 4; /* MOV	A,A */
                        /* no op */
                        break;

                    case 0x80: i8085_ICount[0] -= 4; /* ADD	B */
                        { int q = I.AF.bh + I.BC.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.BC.bh) & 0x10) | (((I.BC.bh ^ I.AF.bh ^ 0x80) & (I.BC.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x81: i8085_ICount[0] -= 4; /* ADD	C */
                        { int q = I.AF.bh + I.BC.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.BC.bl) & 0x10) | (((I.BC.bl ^ I.AF.bh ^ 0x80) & (I.BC.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x82: i8085_ICount[0] -= 4; /* ADD	D */
                        { int q = I.AF.bh + I.DE.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.DE.bh) & 0x10) | (((I.DE.bh ^ I.AF.bh ^ 0x80) & (I.DE.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x83: i8085_ICount[0] -= 4; /* ADD	E */
                        { int q = I.AF.bh + I.DE.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.DE.bl) & 0x10) | (((I.DE.bl ^ I.AF.bh ^ 0x80) & (I.DE.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x84: i8085_ICount[0] -= 4; /* ADD	H */
                        { int q = I.AF.bh + I.HL.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.HL.bh) & 0x10) | (((I.HL.bh ^ I.AF.bh ^ 0x80) & (I.HL.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x85: i8085_ICount[0] -= 4; /* ADD	L */
                        { int q = I.AF.bh + I.HL.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.HL.bl) & 0x10) | (((I.HL.bl ^ I.AF.bh ^ 0x80) & (I.HL.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x86: i8085_ICount[0] -= 7; /* ADD	M */
                        { int q = I.AF.bh + RM(I.HL.d); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ RM(I.HL.d)) & 0x10) | (((RM(I.HL.d) ^ I.AF.bh ^ 0x80) & (RM(I.HL.d) ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x87: i8085_ICount[0] -= 4; /* ADD	A */
                        { int q = I.AF.bh + I.AF.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.AF.bh) & 0x10) | (((I.AF.bh ^ I.AF.bh ^ 0x80) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;

                    case 0x88: i8085_ICount[0] -= 4; /* ADC	B */
                        { int q = I.AF.bh + I.BC.bh + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.BC.bh) & 0x10) | (((I.BC.bh ^ I.AF.bh ^ 0x80) & (I.BC.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x89: i8085_ICount[0] -= 4; /* ADC	C */
                        { int q = I.AF.bh + I.BC.bl + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.BC.bl) & 0x10) | (((I.BC.bl ^ I.AF.bh ^ 0x80) & (I.BC.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8a: i8085_ICount[0] -= 4; /* ADC	D */
                        { int q = I.AF.bh + I.DE.bh + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.DE.bh) & 0x10) | (((I.DE.bh ^ I.AF.bh ^ 0x80) & (I.DE.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8b: i8085_ICount[0] -= 4; /* ADC	E */
                        { int q = I.AF.bh + I.DE.bl + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.DE.bl) & 0x10) | (((I.DE.bl ^ I.AF.bh ^ 0x80) & (I.DE.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8c: i8085_ICount[0] -= 4; /* ADC	H */
                        { int q = I.AF.bh + I.HL.bh + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.HL.bh) & 0x10) | (((I.HL.bh ^ I.AF.bh ^ 0x80) & (I.HL.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8d: i8085_ICount[0] -= 4; /* ADC	L */
                        { int q = I.AF.bh + I.HL.bl + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.HL.bl) & 0x10) | (((I.HL.bl ^ I.AF.bh ^ 0x80) & (I.HL.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8e: i8085_ICount[0] -= 7; /* ADC	M */
                        { int q = I.AF.bh + RM(I.HL.d) + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ RM(I.HL.d)) & 0x10) | (((RM(I.HL.d) ^ I.AF.bh ^ 0x80) & (RM(I.HL.d) ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x8f: i8085_ICount[0] -= 4; /* ADC	A */
                        { int q = I.AF.bh + I.AF.bh + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.AF.bh) & 0x10) | (((I.AF.bh ^ I.AF.bh ^ 0x80) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;

                    case 0x90: i8085_ICount[0] -= 4; /* SUB	B */
                        { int q = I.AF.bh - I.BC.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bh) & 0x10) | (((I.BC.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x91: i8085_ICount[0] -= 4; /* SUB	C */
                        { int q = I.AF.bh - I.BC.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bl) & 0x10) | (((I.BC.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x92: i8085_ICount[0] -= 4; /* SUB	D */
                        { int q = I.AF.bh - I.DE.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bh) & 0x10) | (((I.DE.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x93: i8085_ICount[0] -= 4; /* SUB	E */
                        { int q = I.AF.bh - I.DE.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bl) & 0x10) | (((I.DE.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x94: i8085_ICount[0] -= 4; /* SUB	H */
                        { int q = I.AF.bh - I.HL.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bh) & 0x10) | (((I.HL.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x95: i8085_ICount[0] -= 4; /* SUB	L */
                        { int q = I.AF.bh - I.HL.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bl) & 0x10) | (((I.HL.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x96: i8085_ICount[0] -= 7; /* SUB	M */
                        { int q = I.AF.bh - RM(I.HL.d); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ RM(I.HL.d)) & 0x10) | (((RM(I.HL.d) ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x97: i8085_ICount[0] -= 4; /* SUB	A */
                        { int q = I.AF.bh - I.AF.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.AF.bh) & 0x10) | (((I.AF.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;

                    case 0x98: i8085_ICount[0] -= 4; /* SBB	B */
                        { int q = I.AF.bh - I.BC.bh - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bh) & 0x10) | (((I.BC.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x99: i8085_ICount[0] -= 4; /* SBB	C */
                        { int q = I.AF.bh - I.BC.bl - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bl) & 0x10) | (((I.BC.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9a: i8085_ICount[0] -= 4; /* SBB	D */
                        { int q = I.AF.bh - I.DE.bh - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bh) & 0x10) | (((I.DE.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9b: i8085_ICount[0] -= 4; /* SBB	E */
                        { int q = I.AF.bh - I.DE.bl - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bl) & 0x10) | (((I.DE.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9c: i8085_ICount[0] -= 4; /* SBB	H */
                        { int q = I.AF.bh - I.HL.bh - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bh) & 0x10) | (((I.HL.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9d: i8085_ICount[0] -= 4; /* SBB	L */
                        { int q = I.AF.bh - I.HL.bl - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bl) & 0x10) | (((I.HL.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9e: i8085_ICount[0] -= 7; /* SBB	M */
                        { int q = I.AF.bh - RM(I.HL.d) - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ RM(I.HL.d)) & 0x10) | (((RM(I.HL.d) ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0x9f: i8085_ICount[0] -= 4; /* SBB	A */
                        { int q = I.AF.bh - I.AF.bh - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.AF.bh) & 0x10) | (((I.AF.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;

                    case 0xa0: i8085_ICount[0] -= 4; /* ANA	B */
                        I.AF.bh &= I.BC.bh; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa1: i8085_ICount[0] -= 4; /* ANA	C */
                        I.AF.bh &= I.BC.bl; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa2: i8085_ICount[0] -= 4; /* ANA	D */
                        I.AF.bh &= I.DE.bh; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa3: i8085_ICount[0] -= 4; /* ANA	E */
                        I.AF.bh &= I.DE.bl; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa4: i8085_ICount[0] -= 4; /* ANA	H */
                        I.AF.bh &= I.HL.bh; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa5: i8085_ICount[0] -= 4; /* ANA	L */
                        I.AF.bh &= I.HL.bl; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa6: i8085_ICount[0] -= 7; /* ANA	M */
                        I.AF.bh &= RM(I.HL.d); I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xa7: i8085_ICount[0] -= 4; /* ANA	A */
                        I.AF.bh &= I.AF.bh; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;

                    case 0xa8: i8085_ICount[0] -= 4; /* XRA	B */
                        I.AF.bh ^= I.BC.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xa9: i8085_ICount[0] -= 4; /* XRA	C */
                        I.AF.bh ^= I.BC.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xaa: i8085_ICount[0] -= 4; /* XRA	D */
                        I.AF.bh ^= I.DE.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xab: i8085_ICount[0] -= 4; /* XRA	E */
                        I.AF.bh ^= I.DE.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xac: i8085_ICount[0] -= 4; /* XRA	H */
                        I.AF.bh ^= I.HL.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xad: i8085_ICount[0] -= 4; /* XRA	L */
                        I.AF.bh ^= I.HL.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xae: i8085_ICount[0] -= 7; /* XRA	M */
                        I.AF.bh ^= RM(I.HL.d); I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xaf: i8085_ICount[0] -= 4; /* XRA	A */
                        I.AF.bh ^= I.AF.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;

                    case 0xb0: i8085_ICount[0] -= 4; /* ORA	B */
                        I.AF.bh |= I.BC.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb1: i8085_ICount[0] -= 4; /* ORA	C */
                        I.AF.bh |= I.BC.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb2: i8085_ICount[0] -= 4; /* ORA	D */
                        I.AF.bh |= I.DE.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb3: i8085_ICount[0] -= 4; /* ORA	E */
                        I.AF.bh |= I.DE.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb4: i8085_ICount[0] -= 4; /* ORA	H */
                        I.AF.bh |= I.HL.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb5: i8085_ICount[0] -= 4; /* ORA	L */
                        I.AF.bh |= I.HL.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb6: i8085_ICount[0] -= 7; /* ORA	M */
                        I.AF.bh |= RM(I.HL.d); I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xb7: i8085_ICount[0] -= 4; /* ORA	A */
                        I.AF.bh |= I.AF.bh; I.AF.bl = ZSP[I.AF.bh];
                        break;

                    case 0xb8: i8085_ICount[0] -= 4; /* CMP	B */
                        { int q = I.AF.bh - I.BC.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bh) & 0x10) | (((I.BC.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xb9: i8085_ICount[0] -= 4; /* CMP	C */
                        { int q = I.AF.bh - I.BC.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.BC.bl) & 0x10) | (((I.BC.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xba: i8085_ICount[0] -= 4; /* CMP	D */
                        { int q = I.AF.bh - I.DE.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bh) & 0x10) | (((I.DE.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xbb: i8085_ICount[0] -= 4; /* CMP	E */
                        { int q = I.AF.bh - I.DE.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.DE.bl) & 0x10) | (((I.DE.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xbc: i8085_ICount[0] -= 4; /* CMP	H */
                        { int q = I.AF.bh - I.HL.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bh) & 0x10) | (((I.HL.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xbd: i8085_ICount[0] -= 4; /* CMP	L */
                        { int q = I.AF.bh - I.HL.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.HL.bl) & 0x10) | (((I.HL.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xbe: i8085_ICount[0] -= 7; /* CMP	M */
                        { int q = I.AF.bh - RM(I.HL.d); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ RM(I.HL.d)) & 0x10) | (((RM(I.HL.d) ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xbf: i8085_ICount[0] -= 4; /* CMP	A */
                        { int q = I.AF.bh - I.AF.bh; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.AF.bh) & 0x10) | (((I.AF.bh ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;

                    case 0xc0: i8085_ICount[0] -= 5; /* RNZ	*/
                        { if ((I.AF.bl & 0x40) == 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xc1: i8085_ICount[0] -= 10; /* POP	B */
                        { I.BC.bl = RM(I.SP.wl++); I.BC.bh = RM(I.SP.wl++); };
                        break;
                    case 0xc2: i8085_ICount[0] -= 10; /* JNZ	nnnn */
                        { if ((I.AF.bl & 0x40) == 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xc3: i8085_ICount[0] -= 10; /* JMP	nnnn */
                        { if (true) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xc4: i8085_ICount[0] -= 10; /* CNZ	nnnn */
                        { if ((I.AF.bl & 0x40) == 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xc5: i8085_ICount[0] -= 11; /* PUSH B */
                        { WM(--I.SP.wl, I.BC.bh); WM(--I.SP.wl, I.BC.bl); };
                        break;
                    case 0xc6: i8085_ICount[0] -= 7; /* ADI	nn */
                        I.XX.bl = ARG();
                        { int q = I.AF.bh + I.XX.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.XX.bl) & 0x10) | (((I.XX.bl ^ I.AF.bh ^ 0x80) & (I.XX.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0xc7: i8085_ICount[0] -= 11; /* RST	0 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 0; change_pc16(I.PC.d); };
                        break;

                    case 0xc8: i8085_ICount[0] -= 5; /* RZ	*/
                        { if ((I.AF.bl & 0x40) != 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xc9: i8085_ICount[0] -= 4; /* RET	*/
                        { if (true) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xca: i8085_ICount[0] -= 10; /* JZ	nnnn */
                        { if ((I.AF.bl & 0x40) != 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xcb: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0xcc: i8085_ICount[0] -= 10; /* CZ	nnnn */
                        { if ((I.AF.bl & 0x40) != 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xcd: i8085_ICount[0] -= 10; /* CALL nnnn */
                        { if (true) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xce: i8085_ICount[0] -= 7; /* ACI	nn */
                        I.XX.bl = ARG();
                        { int q = I.AF.bh + I.XX.bl + (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | ((I.AF.bh ^ q ^ I.XX.bl) & 0x10) | (((I.XX.bl ^ I.AF.bh ^ 0x80) & (I.XX.bl ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0xcf: i8085_ICount[0] -= 11; /* RST	1 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 1; change_pc16(I.PC.d); };
                        break;

                    case 0xd0: i8085_ICount[0] -= 5; /* RNC	*/
                        { if ((I.AF.bl & 0x01) == 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xd1: i8085_ICount[0] -= 10; /* POP	D */
                        { I.DE.bl = RM(I.SP.wl++); I.DE.bh = RM(I.SP.wl++); };
                        break;
                    case 0xd2: i8085_ICount[0] -= 10; /* JNC	nnnn */
                        { if ((I.AF.bl & 0x01) == 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xd3: i8085_ICount[0] -= 11; /* OUT	nn */
                        I.XX.d = ARG(); cpu_writeport((int)I.XX.d, I.AF.bh);
                        break;
                    case 0xd4: i8085_ICount[0] -= 10; /* CNC	nnnn */
                        { if ((I.AF.bl & 0x01) == 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xd5: i8085_ICount[0] -= 11; /* PUSH D */
                        { WM(--I.SP.wl, I.DE.bh); WM(--I.SP.wl, I.DE.bl); };
                        break;
                    case 0xd6: i8085_ICount[0] -= 7; /* SUI	nn */
                        I.XX.bl = ARG();
                        { int q = I.AF.bh - I.XX.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.XX.bl) & 0x10) | (((I.XX.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0xd7: i8085_ICount[0] -= 11; /* RST	2 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 2; change_pc16(I.PC.d); };
                        break;

                    case 0xd8: i8085_ICount[0] -= 5; /* RC	*/
                        { if ((I.AF.bl & 0x01) != 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xd9: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0xda: i8085_ICount[0] -= 10; /* JC	nnnn */
                        { if ((I.AF.bl & 0x01) != 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xdb: i8085_ICount[0] -= 11; /* IN	nn */
                        I.XX.d = ARG(); I.AF.bh = (byte)cpu_readport((int)I.XX.d); ;
                        break;
                    case 0xdc: i8085_ICount[0] -= 10; /* CC	nnnn */
                        { if ((I.AF.bl & 0x01) != 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xdd: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0xde: i8085_ICount[0] -= 7; /* SBI	nn */
                        I.XX.bl = ARG();
                        { int q = I.AF.bh - I.XX.bl - (I.AF.bl & 0x01); I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.XX.bl) & 0x10) | (((I.XX.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); I.AF.bh = (byte)q; };
                        break;
                    case 0xdf: i8085_ICount[0] -= 11; /* RST	3 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 3; change_pc16(I.PC.d); };
                        break;

                    case 0xe0: i8085_ICount[0] -= 5; /* RPE	  */
                        { if ((I.AF.bl & 0x04) == 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xe1: i8085_ICount[0] -= 10; /* POP	H */
                        { I.HL.bl = RM(I.SP.wl++); I.HL.bh = RM(I.SP.wl++); };
                        break;
                    case 0xe2: i8085_ICount[0] -= 10; /* JPE	nnnn */
                        { if ((I.AF.bl & 0x04) == 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xe3: i8085_ICount[0] -= 19; /* XTHL */
                        { I.XX.bl = RM(I.SP.wl++); I.XX.bh = RM(I.SP.wl++); };
                        { WM(--I.SP.wl, I.HL.bh); WM(--I.SP.wl, I.HL.bl); };
                        I.HL.d = I.XX.d;
                        break;
                    case 0xe4: i8085_ICount[0] -= 10; /* CPE	nnnn */
                        { if ((I.AF.bl & 0x04) == 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xe5: i8085_ICount[0] -= 11; /* PUSH H */
                        { WM(--I.SP.wl, I.HL.bh); WM(--I.SP.wl, I.HL.bl); };
                        break;
                    case 0xe6: i8085_ICount[0] -= 7; /* ANI	nn */
                        I.XX.bl = ARG();
                        I.AF.bh &= I.XX.bl; I.AF.bl = (byte)(ZSP[I.AF.bh] | 0x10);
                        break;
                    case 0xe7: i8085_ICount[0] -= 11; /* RST	4 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 4; change_pc16(I.PC.d); };
                        break;

                    case 0xe8: i8085_ICount[0] -= 5; /* RPO	*/
                        { if ((I.AF.bl & 0x04) != 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xe9: i8085_ICount[0] -= 4; /* PCHL */
                        I.PC.d = I.HL.wl;
                        change_pc16(I.PC.d);
                        break;
                    case 0xea: i8085_ICount[0] -= 10; /* JPO	nnnn */
                        { if ((I.AF.bl & 0x04) != 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xeb: i8085_ICount[0] -= 4; /* XCHG */
                        I.XX.d = I.DE.d;
                        I.DE.d = I.HL.d;
                        I.HL.d = I.XX.d;
                        break;
                    case 0xec: i8085_ICount[0] -= 10; /* CPO	nnnn */
                        { if ((I.AF.bl & 0x04) != 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xed: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0xee: i8085_ICount[0] -= 7; /* XRI	nn */
                        I.XX.bl = ARG();
                        I.AF.bh ^= I.XX.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xef: i8085_ICount[0] -= 11; /* RST	5 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 5; change_pc16(I.PC.d); };
                        break;

                    case 0xf0: i8085_ICount[0] -= 5; /* RP	*/
                        { if ((I.AF.bl & 0x80) == 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xf1: i8085_ICount[0] -= 10; /* POP	A */
                        { I.AF.bl = RM(I.SP.wl++); I.AF.bh = RM(I.SP.wl++); };
                        break;
                    case 0xf2: i8085_ICount[0] -= 10; /* JP	nnnn */
                        { if ((I.AF.bl & 0x80) == 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xf3: i8085_ICount[0] -= 4; /* DI	*/
                        /* remove interrupt enable */
                        I.IM &= unchecked((byte)~0x20);
                        break;
                    case 0xf4: i8085_ICount[0] -= 10; /* CP	nnnn */
                        { if ((I.AF.bl & 0x80) == 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xf5: i8085_ICount[0] -= 11; /* PUSH A */
                        { WM(--I.SP.wl, I.AF.bh); WM(--I.SP.wl, I.AF.bl); };
                        break;
                    case 0xf6: i8085_ICount[0] -= 7; /* ORI	nn */
                        I.XX.bl = ARG();
                        I.AF.bh |= I.XX.bl; I.AF.bl = ZSP[I.AF.bh];
                        break;
                    case 0xf7: i8085_ICount[0] -= 11; /* RST	6 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 6; change_pc16(I.PC.d); };
                        break;

                    case 0xf8: i8085_ICount[0] -= 5; /* RM	*/
                        { if ((I.AF.bl & 0x80) != 0) { i8085_ICount[0] -= 6; { I.PC.bl = RM(I.SP.wl++); I.PC.bh = RM(I.SP.wl++); }; change_pc16(I.PC.d); } };
                        break;
                    case 0xf9: i8085_ICount[0] -= 6; /* SPHL */
                        I.SP.d = I.HL.d;
                        break;
                    case 0xfa: i8085_ICount[0] -= 10; /* JM	nnnn */
                        { if ((I.AF.bl & 0x80) != 0) { I.PC.wl = ARG16(); change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xfb: i8085_ICount[0] -= 4; /* EI */
                        /* set interrupt enable */
                        I.IM |= 0x20;
                        /* remove serviced IRQ flag */
                        I.IREQ &= (byte)~I.ISRV;
                        /* reset serviced IRQ */
                        I.ISRV = 0;
                        if (I.irq_state[0] != CLEAR_LINE)
                        {
                            //LOG((errorlog, "i8085 EI sets INTR\n"));
                            I.IREQ |= 0x08;
                            I.INTR = I8085_INTR;
                        }
                        if (I.cputype != 0)
                        {
                            if (I.irq_state[1] != CLEAR_LINE)
                            {
                                //LOG((errorlog, "i8085 EI sets RST5.5\n"));
                                I.IREQ |= 0x01;
                            }
                            if (I.irq_state[2] != CLEAR_LINE)
                            {
                                //LOG((errorlog, "i8085 EI sets RST6.5\n"));
                                I.IREQ |= 0x02;
                            }
                            if (I.irq_state[3] != CLEAR_LINE)
                            {
                                //LOG((errorlog, "i8085 EI sets RST7.5\n"));
                                I.IREQ |= 0x04;
                            }
                            /* find highest priority IREQ flag with

                                           IM enabled and schedule for execution */
                            if ((I.IM & 0x04) == 0 && (I.IREQ & 0x04) != 0)
                            {
                                I.ISRV = 0x04;
                                I.IRQ2 = 0x003c;
                            }
                            else
                                if ((I.IM & 0x02) == 0 && (I.IREQ & 0x02) != 0)
                                {
                                    I.ISRV = 0x02;
                                    I.IRQ2 = 0x0034;
                                }
                                else
                                    if ((I.IM & 0x01) == 0 && (I.IREQ & 0x01) != 0)
                                    {
                                        I.ISRV = 0x01;
                                        I.IRQ2 = 0x002c;
                                    }
                                    else
                                        if ((I.IM & 0x08) == 0 && (I.IREQ & 0x08) != 0)
                                        {
                                            I.ISRV = 0x08;
                                            I.IRQ2 = I.INTR;
                                        }
                        }
                        else
                        {
                            if ((I.IM & 0x08) == 0 && (I.IREQ & 0x08) != 0)
                            {
                                I.ISRV = 0x08;
                                I.IRQ2 = I.INTR;
                            }
                        }
                        break;
                    case 0xfc: i8085_ICount[0] -= 10; /* CM	nnnn */
                        { if ((I.AF.bl & 0x80) != 0) { ushort a = ARG16(); i8085_ICount[0] -= 7; { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = a; change_pc16(I.PC.d); } else I.PC.wl += 2; };
                        break;
                    case 0xfd: i8085_ICount[0] -= 4; /* ???? */
                        illegal();
                        break;
                    case 0xfe: i8085_ICount[0] -= 7; /* CPI	nn */
                        I.XX.bl = ARG();
                        { int q = I.AF.bh - I.XX.bl; I.AF.bl = (byte)(ZS[q & 255] | ((q >> 8) & 0x01) | 0x02 | ((I.AF.bh ^ q ^ I.XX.bl) & 0x10) | (((I.XX.bl ^ I.AF.bh) & (I.AF.bh ^ q) & 0x80) >> 5)); };
                        break;
                    case 0xff: i8085_ICount[0] -= 11; /* RST	7 */
                        { { WM(--I.SP.wl, I.PC.bh); WM(--I.SP.wl, I.PC.bl); }; I.PC.d = 8 * 7; change_pc16(I.PC.d); };
                        break;
                }

            }
            void illegal()
            {
                ushort pc = (ushort)(I.PC.wl - 1);
                printf("i8085 illegal instruction %04X $%02X\n", pc, cpu_readop(pc));
            }
            static ushort[] DAA = {
  (0x00<<8)   +ZF         +VF      ,
  (0x01<<8)                        ,
  (0x02<<8)                        ,
  (0x03<<8)               +VF      ,
  (0x04<<8)                        ,
  (0x05<<8)               +VF      ,
  (0x06<<8)               +VF      ,
  (0x07<<8)                        ,
  (0x08<<8)            +XF         ,
  (0x09<<8)            +XF+VF      ,
  (0x10<<8)         +HF            ,
  (0x11<<8)         +HF   +VF      ,
  (0x12<<8)         +HF   +VF      ,
  (0x13<<8)         +HF            ,
  (0x14<<8)         +HF   +VF      ,
  (0x15<<8)         +HF            ,
  (0x10<<8)                        ,
  (0x11<<8)               +VF      ,
  (0x12<<8)               +VF      ,
  (0x13<<8)                        ,
  (0x14<<8)               +VF      ,
  (0x15<<8)                        ,
  (0x16<<8)                        ,
  (0x17<<8)               +VF      ,
  (0x18<<8)            +XF+VF      ,
  (0x19<<8)            +XF         ,
  (0x20<<8)      +YF+HF            ,
  (0x21<<8)      +YF+HF   +VF      ,
  (0x22<<8)      +YF+HF   +VF      ,
  (0x23<<8)      +YF+HF            ,
  (0x24<<8)      +YF+HF   +VF      ,
  (0x25<<8)      +YF+HF            ,
  (0x20<<8)      +YF               ,
  (0x21<<8)      +YF      +VF      ,
  (0x22<<8)      +YF      +VF      ,
  (0x23<<8)      +YF               ,
  (0x24<<8)      +YF      +VF      ,
  (0x25<<8)      +YF               ,
  (0x26<<8)      +YF               ,
  (0x27<<8)      +YF      +VF      ,
  (0x28<<8)      +YF   +XF+VF      ,
  (0x29<<8)      +YF   +XF         ,
  (0x30<<8)      +YF+HF   +VF      ,
  (0x31<<8)      +YF+HF            ,
  (0x32<<8)      +YF+HF            ,
  (0x33<<8)      +YF+HF   +VF      ,
  (0x34<<8)      +YF+HF            ,
  (0x35<<8)      +YF+HF   +VF      ,
  (0x30<<8)      +YF      +VF      ,
  (0x31<<8)      +YF               ,
  (0x32<<8)      +YF               ,
  (0x33<<8)      +YF      +VF      ,
  (0x34<<8)      +YF               ,
  (0x35<<8)      +YF      +VF      ,
  (0x36<<8)      +YF      +VF      ,
  (0x37<<8)      +YF               ,
  (0x38<<8)      +YF   +XF         ,
  (0x39<<8)      +YF   +XF+VF      ,
  (0x40<<8)         +HF            ,
  (0x41<<8)         +HF   +VF      ,
  (0x42<<8)         +HF   +VF      ,
  (0x43<<8)         +HF            ,
  (0x44<<8)         +HF   +VF      ,
  (0x45<<8)         +HF            ,
  (0x40<<8)                        ,
  (0x41<<8)               +VF      ,
  (0x42<<8)               +VF      ,
  (0x43<<8)                        ,
  (0x44<<8)               +VF      ,
  (0x45<<8)                        ,
  (0x46<<8)                        ,
  (0x47<<8)               +VF      ,
  (0x48<<8)            +XF+VF      ,
  (0x49<<8)            +XF         ,
  (0x50<<8)         +HF   +VF      ,
  (0x51<<8)         +HF            ,
  (0x52<<8)         +HF            ,
  (0x53<<8)         +HF   +VF      ,
  (0x54<<8)         +HF            ,
  (0x55<<8)         +HF   +VF      ,
  (0x50<<8)               +VF      ,
  (0x51<<8)                        ,
  (0x52<<8)                        ,
  (0x53<<8)               +VF      ,
  (0x54<<8)                        ,
  (0x55<<8)               +VF      ,
  (0x56<<8)               +VF      ,
  (0x57<<8)                        ,
  (0x58<<8)            +XF         ,
  (0x59<<8)            +XF+VF      ,
  (0x60<<8)      +YF+HF   +VF      ,
  (0x61<<8)      +YF+HF            ,
  (0x62<<8)      +YF+HF            ,
  (0x63<<8)      +YF+HF   +VF      ,
  (0x64<<8)      +YF+HF            ,
  (0x65<<8)      +YF+HF   +VF      ,
  (0x60<<8)      +YF      +VF      ,
  (0x61<<8)      +YF               ,
  (0x62<<8)      +YF               ,
  (0x63<<8)      +YF      +VF      ,
  (0x64<<8)      +YF               ,
  (0x65<<8)      +YF      +VF      ,
  (0x66<<8)      +YF      +VF      ,
  (0x67<<8)      +YF               ,
  (0x68<<8)      +YF   +XF         ,
  (0x69<<8)      +YF   +XF+VF      ,
  (0x70<<8)      +YF+HF            ,
  (0x71<<8)      +YF+HF   +VF      ,
  (0x72<<8)      +YF+HF   +VF      ,
  (0x73<<8)      +YF+HF            ,
  (0x74<<8)      +YF+HF   +VF      ,
  (0x75<<8)      +YF+HF            ,
  (0x70<<8)      +YF               ,
  (0x71<<8)      +YF      +VF      ,
  (0x72<<8)      +YF      +VF      ,
  (0x73<<8)      +YF               ,
  (0x74<<8)      +YF      +VF      ,
  (0x75<<8)      +YF               ,
  (0x76<<8)      +YF               ,
  (0x77<<8)      +YF      +VF      ,
  (0x78<<8)      +YF   +XF+VF      ,
  (0x79<<8)      +YF   +XF         ,
  (0x80<<8)+SF      +HF            ,
  (0x81<<8)+SF      +HF   +VF      ,
  (0x82<<8)+SF      +HF   +VF      ,
  (0x83<<8)+SF      +HF            ,
  (0x84<<8)+SF      +HF   +VF      ,
  (0x85<<8)+SF      +HF            ,
  (0x80<<8)+SF                     ,
  (0x81<<8)+SF            +VF      ,
  (0x82<<8)+SF            +VF      ,
  (0x83<<8)+SF                     ,
  (0x84<<8)+SF            +VF      ,
  (0x85<<8)+SF                     ,
  (0x86<<8)+SF                     ,
  (0x87<<8)+SF            +VF      ,
  (0x88<<8)+SF         +XF+VF      ,
  (0x89<<8)+SF         +XF         ,
  (0x90<<8)+SF      +HF   +VF      ,
  (0x91<<8)+SF      +HF            ,
  (0x92<<8)+SF      +HF            ,
  (0x93<<8)+SF      +HF   +VF      ,
  (0x94<<8)+SF      +HF            ,
  (0x95<<8)+SF      +HF   +VF      ,
  (0x90<<8)+SF            +VF      ,
  (0x91<<8)+SF                     ,
  (0x92<<8)+SF                     ,
  (0x93<<8)+SF            +VF      ,
  (0x94<<8)+SF                     ,
  (0x95<<8)+SF            +VF      ,
  (0x96<<8)+SF            +VF      ,
  (0x97<<8)+SF                     ,
  (0x98<<8)+SF         +XF         ,
  (0x99<<8)+SF         +XF+VF      ,
  (0x00<<8)   +ZF   +HF   +VF   +CF,
  (0x01<<8)         +HF         +CF,
  (0x02<<8)         +HF         +CF,
  (0x03<<8)         +HF   +VF   +CF,
  (0x04<<8)         +HF         +CF,
  (0x05<<8)         +HF   +VF   +CF,
  (0x00<<8)   +ZF         +VF   +CF,
  (0x01<<8)                     +CF,
  (0x02<<8)                     +CF,
  (0x03<<8)               +VF   +CF,
  (0x04<<8)                     +CF,
  (0x05<<8)               +VF   +CF,
  (0x06<<8)               +VF   +CF,
  (0x07<<8)                     +CF,
  (0x08<<8)            +XF      +CF,
  (0x09<<8)            +XF+VF   +CF,
  (0x10<<8)         +HF         +CF,
  (0x11<<8)         +HF   +VF   +CF,
  (0x12<<8)         +HF   +VF   +CF,
  (0x13<<8)         +HF         +CF,
  (0x14<<8)         +HF   +VF   +CF,
  (0x15<<8)         +HF         +CF,
  (0x10<<8)                     +CF,
  (0x11<<8)               +VF   +CF,
  (0x12<<8)               +VF   +CF,
  (0x13<<8)                     +CF,
  (0x14<<8)               +VF   +CF,
  (0x15<<8)                     +CF,
  (0x16<<8)                     +CF,
  (0x17<<8)               +VF   +CF,
  (0x18<<8)            +XF+VF   +CF,
  (0x19<<8)            +XF      +CF,
  (0x20<<8)      +YF+HF         +CF,
  (0x21<<8)      +YF+HF   +VF   +CF,
  (0x22<<8)      +YF+HF   +VF   +CF,
  (0x23<<8)      +YF+HF         +CF,
  (0x24<<8)      +YF+HF   +VF   +CF,
  (0x25<<8)      +YF+HF         +CF,
  (0x20<<8)      +YF            +CF,
  (0x21<<8)      +YF      +VF   +CF,
  (0x22<<8)      +YF      +VF   +CF,
  (0x23<<8)      +YF            +CF,
  (0x24<<8)      +YF      +VF   +CF,
  (0x25<<8)      +YF            +CF,
  (0x26<<8)      +YF            +CF,
  (0x27<<8)      +YF      +VF   +CF,
  (0x28<<8)      +YF   +XF+VF   +CF,
  (0x29<<8)      +YF   +XF      +CF,
  (0x30<<8)      +YF+HF   +VF   +CF,
  (0x31<<8)      +YF+HF         +CF,
  (0x32<<8)      +YF+HF         +CF,
  (0x33<<8)      +YF+HF   +VF   +CF,
  (0x34<<8)      +YF+HF         +CF,
  (0x35<<8)      +YF+HF   +VF   +CF,
  (0x30<<8)      +YF      +VF   +CF,
  (0x31<<8)      +YF            +CF,
  (0x32<<8)      +YF            +CF,
  (0x33<<8)      +YF      +VF   +CF,
  (0x34<<8)      +YF            +CF,
  (0x35<<8)      +YF      +VF   +CF,
  (0x36<<8)      +YF      +VF   +CF,
  (0x37<<8)      +YF            +CF,
  (0x38<<8)      +YF   +XF      +CF,
  (0x39<<8)      +YF   +XF+VF   +CF,
  (0x40<<8)         +HF         +CF,
  (0x41<<8)         +HF   +VF   +CF,
  (0x42<<8)         +HF   +VF   +CF,
  (0x43<<8)         +HF         +CF,
  (0x44<<8)         +HF   +VF   +CF,
  (0x45<<8)         +HF         +CF,
  (0x40<<8)                     +CF,
  (0x41<<8)               +VF   +CF,
  (0x42<<8)               +VF   +CF,
  (0x43<<8)                     +CF,
  (0x44<<8)               +VF   +CF,
  (0x45<<8)                     +CF,
  (0x46<<8)                     +CF,
  (0x47<<8)               +VF   +CF,
  (0x48<<8)            +XF+VF   +CF,
  (0x49<<8)            +XF      +CF,
  (0x50<<8)         +HF   +VF   +CF,
  (0x51<<8)         +HF         +CF,
  (0x52<<8)         +HF         +CF,
  (0x53<<8)         +HF   +VF   +CF,
  (0x54<<8)         +HF         +CF,
  (0x55<<8)         +HF   +VF   +CF,
  (0x50<<8)               +VF   +CF,
  (0x51<<8)                     +CF,
  (0x52<<8)                     +CF,
  (0x53<<8)               +VF   +CF,
  (0x54<<8)                     +CF,
  (0x55<<8)               +VF   +CF,
  (0x56<<8)               +VF   +CF,
  (0x57<<8)                     +CF,
  (0x58<<8)            +XF      +CF,
  (0x59<<8)            +XF+VF   +CF,
  (0x60<<8)      +YF+HF   +VF   +CF,
  (0x61<<8)      +YF+HF         +CF,
  (0x62<<8)      +YF+HF         +CF,
  (0x63<<8)      +YF+HF   +VF   +CF,
  (0x64<<8)      +YF+HF         +CF,
  (0x65<<8)      +YF+HF   +VF   +CF,
  (0x60<<8)      +YF      +VF   +CF,
  (0x61<<8)      +YF            +CF,
  (0x62<<8)      +YF            +CF,
  (0x63<<8)      +YF      +VF   +CF,
  (0x64<<8)      +YF            +CF,
  (0x65<<8)      +YF      +VF   +CF,
  (0x66<<8)      +YF      +VF   +CF,
  (0x67<<8)      +YF            +CF,
  (0x68<<8)      +YF   +XF      +CF,
  (0x69<<8)      +YF   +XF+VF   +CF,
  (0x70<<8)      +YF+HF         +CF,
  (0x71<<8)      +YF+HF   +VF   +CF,
  (0x72<<8)      +YF+HF   +VF   +CF,
  (0x73<<8)      +YF+HF         +CF,
  (0x74<<8)      +YF+HF   +VF   +CF,
  (0x75<<8)      +YF+HF         +CF,
  (0x70<<8)      +YF            +CF,
  (0x71<<8)      +YF      +VF   +CF,
  (0x72<<8)      +YF      +VF   +CF,
  (0x73<<8)      +YF            +CF,
  (0x74<<8)      +YF      +VF   +CF,
  (0x75<<8)      +YF            +CF,
  (0x76<<8)      +YF            +CF,
  (0x77<<8)      +YF      +VF   +CF,
  (0x78<<8)      +YF   +XF+VF   +CF,
  (0x79<<8)      +YF   +XF      +CF,
  (0x80<<8)+SF      +HF         +CF,
  (0x81<<8)+SF      +HF   +VF   +CF,
  (0x82<<8)+SF      +HF   +VF   +CF,
  (0x83<<8)+SF      +HF         +CF,
  (0x84<<8)+SF      +HF   +VF   +CF,
  (0x85<<8)+SF      +HF         +CF,
  (0x80<<8)+SF                  +CF,
  (0x81<<8)+SF            +VF   +CF,
  (0x82<<8)+SF            +VF   +CF,
  (0x83<<8)+SF                  +CF,
  (0x84<<8)+SF            +VF   +CF,
  (0x85<<8)+SF                  +CF,
  (0x86<<8)+SF                  +CF,
  (0x87<<8)+SF            +VF   +CF,
  (0x88<<8)+SF         +XF+VF   +CF,
  (0x89<<8)+SF         +XF      +CF,
  (0x90<<8)+SF      +HF   +VF   +CF,
  (0x91<<8)+SF      +HF         +CF,
  (0x92<<8)+SF      +HF         +CF,
  (0x93<<8)+SF      +HF   +VF   +CF,
  (0x94<<8)+SF      +HF         +CF,
  (0x95<<8)+SF      +HF   +VF   +CF,
  (0x90<<8)+SF            +VF   +CF,
  (0x91<<8)+SF                  +CF,
  (0x92<<8)+SF                  +CF,
  (0x93<<8)+SF            +VF   +CF,
  (0x94<<8)+SF                  +CF,
  (0x95<<8)+SF            +VF   +CF,
  (0x96<<8)+SF            +VF   +CF,
  (0x97<<8)+SF                  +CF,
  (0x98<<8)+SF         +XF      +CF,
  (0x99<<8)+SF         +XF+VF   +CF,
  (0xA0<<8)+SF   +YF+HF   +VF   +CF,
  (0xA1<<8)+SF   +YF+HF         +CF,
  (0xA2<<8)+SF   +YF+HF         +CF,
  (0xA3<<8)+SF   +YF+HF   +VF   +CF,
  (0xA4<<8)+SF   +YF+HF         +CF,
  (0xA5<<8)+SF   +YF+HF   +VF   +CF,
  (0xA0<<8)+SF   +YF      +VF   +CF,
  (0xA1<<8)+SF   +YF            +CF,
  (0xA2<<8)+SF   +YF            +CF,
  (0xA3<<8)+SF   +YF      +VF   +CF,
  (0xA4<<8)+SF   +YF            +CF,
  (0xA5<<8)+SF   +YF      +VF   +CF,
  (0xA6<<8)+SF   +YF      +VF   +CF,
  (0xA7<<8)+SF   +YF            +CF,
  (0xA8<<8)+SF   +YF   +XF      +CF,
  (0xA9<<8)+SF   +YF   +XF+VF   +CF,
  (0xB0<<8)+SF   +YF+HF         +CF,
  (0xB1<<8)+SF   +YF+HF   +VF   +CF,
  (0xB2<<8)+SF   +YF+HF   +VF   +CF,
  (0xB3<<8)+SF   +YF+HF         +CF,
  (0xB4<<8)+SF   +YF+HF   +VF   +CF,
  (0xB5<<8)+SF   +YF+HF         +CF,
  (0xB0<<8)+SF   +YF            +CF,
  (0xB1<<8)+SF   +YF      +VF   +CF,
  (0xB2<<8)+SF   +YF      +VF   +CF,
  (0xB3<<8)+SF   +YF            +CF,
  (0xB4<<8)+SF   +YF      +VF   +CF,
  (0xB5<<8)+SF   +YF            +CF,
  (0xB6<<8)+SF   +YF            +CF,
  (0xB7<<8)+SF   +YF      +VF   +CF,
  (0xB8<<8)+SF   +YF   +XF+VF   +CF,
  (0xB9<<8)+SF   +YF   +XF      +CF,
  (0xC0<<8)+SF      +HF   +VF   +CF,
  (0xC1<<8)+SF      +HF         +CF,
  (0xC2<<8)+SF      +HF         +CF,
  (0xC3<<8)+SF      +HF   +VF   +CF,
  (0xC4<<8)+SF      +HF         +CF,
  (0xC5<<8)+SF      +HF   +VF   +CF,
  (0xC0<<8)+SF            +VF   +CF,
  (0xC1<<8)+SF                  +CF,
  (0xC2<<8)+SF                  +CF,
  (0xC3<<8)+SF            +VF   +CF,
  (0xC4<<8)+SF                  +CF,
  (0xC5<<8)+SF            +VF   +CF,
  (0xC6<<8)+SF            +VF   +CF,
  (0xC7<<8)+SF                  +CF,
  (0xC8<<8)+SF         +XF      +CF,
  (0xC9<<8)+SF         +XF+VF   +CF,
  (0xD0<<8)+SF      +HF         +CF,
  (0xD1<<8)+SF      +HF   +VF   +CF,
  (0xD2<<8)+SF      +HF   +VF   +CF,
  (0xD3<<8)+SF      +HF         +CF,
  (0xD4<<8)+SF      +HF   +VF   +CF,
  (0xD5<<8)+SF      +HF         +CF,
  (0xD0<<8)+SF                  +CF,
  (0xD1<<8)+SF            +VF   +CF,
  (0xD2<<8)+SF            +VF   +CF,
  (0xD3<<8)+SF                  +CF,
  (0xD4<<8)+SF            +VF   +CF,
  (0xD5<<8)+SF                  +CF,
  (0xD6<<8)+SF                  +CF,
  (0xD7<<8)+SF            +VF   +CF,
  (0xD8<<8)+SF         +XF+VF   +CF,
  (0xD9<<8)+SF         +XF      +CF,
  (0xE0<<8)+SF   +YF+HF         +CF,
  (0xE1<<8)+SF   +YF+HF   +VF   +CF,
  (0xE2<<8)+SF   +YF+HF   +VF   +CF,
  (0xE3<<8)+SF   +YF+HF         +CF,
  (0xE4<<8)+SF   +YF+HF   +VF   +CF,
  (0xE5<<8)+SF   +YF+HF         +CF,
  (0xE0<<8)+SF   +YF            +CF,
  (0xE1<<8)+SF   +YF      +VF   +CF,
  (0xE2<<8)+SF   +YF      +VF   +CF,
  (0xE3<<8)+SF   +YF            +CF,
  (0xE4<<8)+SF   +YF      +VF   +CF,
  (0xE5<<8)+SF   +YF            +CF,
  (0xE6<<8)+SF   +YF            +CF,
  (0xE7<<8)+SF   +YF      +VF   +CF,
  (0xE8<<8)+SF   +YF   +XF+VF   +CF,
  (0xE9<<8)+SF   +YF   +XF      +CF,
  (0xF0<<8)+SF   +YF+HF   +VF   +CF,
  (0xF1<<8)+SF   +YF+HF         +CF,
  (0xF2<<8)+SF   +YF+HF         +CF,
  (0xF3<<8)+SF   +YF+HF   +VF   +CF,
  (0xF4<<8)+SF   +YF+HF         +CF,
  (0xF5<<8)+SF   +YF+HF   +VF   +CF,
  (0xF0<<8)+SF   +YF      +VF   +CF,
  (0xF1<<8)+SF   +YF            +CF,
  (0xF2<<8)+SF   +YF            +CF,
  (0xF3<<8)+SF   +YF      +VF   +CF,
  (0xF4<<8)+SF   +YF            +CF,
  (0xF5<<8)+SF   +YF      +VF   +CF,
  (0xF6<<8)+SF   +YF      +VF   +CF,
  (0xF7<<8)+SF   +YF            +CF,
  (0xF8<<8)+SF   +YF   +XF      +CF,
  (0xF9<<8)+SF   +YF   +XF+VF   +CF,
  (0x00<<8)   +ZF   +HF   +VF   +CF,
  (0x01<<8)         +HF         +CF,
  (0x02<<8)         +HF         +CF,
  (0x03<<8)         +HF   +VF   +CF,
  (0x04<<8)         +HF         +CF,
  (0x05<<8)         +HF   +VF   +CF,
  (0x00<<8)   +ZF         +VF   +CF,
  (0x01<<8)                     +CF,
  (0x02<<8)                     +CF,
  (0x03<<8)               +VF   +CF,
  (0x04<<8)                     +CF,
  (0x05<<8)               +VF   +CF,
  (0x06<<8)               +VF   +CF,
  (0x07<<8)                     +CF,
  (0x08<<8)            +XF      +CF,
  (0x09<<8)            +XF+VF   +CF,
  (0x10<<8)         +HF         +CF,
  (0x11<<8)         +HF   +VF   +CF,
  (0x12<<8)         +HF   +VF   +CF,
  (0x13<<8)         +HF         +CF,
  (0x14<<8)         +HF   +VF   +CF,
  (0x15<<8)         +HF         +CF,
  (0x10<<8)                     +CF,
  (0x11<<8)               +VF   +CF,
  (0x12<<8)               +VF   +CF,
  (0x13<<8)                     +CF,
  (0x14<<8)               +VF   +CF,
  (0x15<<8)                     +CF,
  (0x16<<8)                     +CF,
  (0x17<<8)               +VF   +CF,
  (0x18<<8)            +XF+VF   +CF,
  (0x19<<8)            +XF      +CF,
  (0x20<<8)      +YF+HF         +CF,
  (0x21<<8)      +YF+HF   +VF   +CF,
  (0x22<<8)      +YF+HF   +VF   +CF,
  (0x23<<8)      +YF+HF         +CF,
  (0x24<<8)      +YF+HF   +VF   +CF,
  (0x25<<8)      +YF+HF         +CF,
  (0x20<<8)      +YF            +CF,
  (0x21<<8)      +YF      +VF   +CF,
  (0x22<<8)      +YF      +VF   +CF,
  (0x23<<8)      +YF            +CF,
  (0x24<<8)      +YF      +VF   +CF,
  (0x25<<8)      +YF            +CF,
  (0x26<<8)      +YF            +CF,
  (0x27<<8)      +YF      +VF   +CF,
  (0x28<<8)      +YF   +XF+VF   +CF,
  (0x29<<8)      +YF   +XF      +CF,
  (0x30<<8)      +YF+HF   +VF   +CF,
  (0x31<<8)      +YF+HF         +CF,
  (0x32<<8)      +YF+HF         +CF,
  (0x33<<8)      +YF+HF   +VF   +CF,
  (0x34<<8)      +YF+HF         +CF,
  (0x35<<8)      +YF+HF   +VF   +CF,
  (0x30<<8)      +YF      +VF   +CF,
  (0x31<<8)      +YF            +CF,
  (0x32<<8)      +YF            +CF,
  (0x33<<8)      +YF      +VF   +CF,
  (0x34<<8)      +YF            +CF,
  (0x35<<8)      +YF      +VF   +CF,
  (0x36<<8)      +YF      +VF   +CF,
  (0x37<<8)      +YF            +CF,
  (0x38<<8)      +YF   +XF      +CF,
  (0x39<<8)      +YF   +XF+VF   +CF,
  (0x40<<8)         +HF         +CF,
  (0x41<<8)         +HF   +VF   +CF,
  (0x42<<8)         +HF   +VF   +CF,
  (0x43<<8)         +HF         +CF,
  (0x44<<8)         +HF   +VF   +CF,
  (0x45<<8)         +HF         +CF,
  (0x40<<8)                     +CF,
  (0x41<<8)               +VF   +CF,
  (0x42<<8)               +VF   +CF,
  (0x43<<8)                     +CF,
  (0x44<<8)               +VF   +CF,
  (0x45<<8)                     +CF,
  (0x46<<8)                     +CF,
  (0x47<<8)               +VF   +CF,
  (0x48<<8)            +XF+VF   +CF,
  (0x49<<8)            +XF      +CF,
  (0x50<<8)         +HF   +VF   +CF,
  (0x51<<8)         +HF         +CF,
  (0x52<<8)         +HF         +CF,
  (0x53<<8)         +HF   +VF   +CF,
  (0x54<<8)         +HF         +CF,
  (0x55<<8)         +HF   +VF   +CF,
  (0x50<<8)               +VF   +CF,
  (0x51<<8)                     +CF,
  (0x52<<8)                     +CF,
  (0x53<<8)               +VF   +CF,
  (0x54<<8)                     +CF,
  (0x55<<8)               +VF   +CF,
  (0x56<<8)               +VF   +CF,
  (0x57<<8)                     +CF,
  (0x58<<8)            +XF      +CF,
  (0x59<<8)            +XF+VF   +CF,
  (0x60<<8)      +YF+HF   +VF   +CF,
  (0x61<<8)      +YF+HF         +CF,
  (0x62<<8)      +YF+HF         +CF,
  (0x63<<8)      +YF+HF   +VF   +CF,
  (0x64<<8)      +YF+HF         +CF,
  (0x65<<8)      +YF+HF   +VF   +CF,
  (0x06<<8)               +VF      ,
  (0x07<<8)                        ,
  (0x08<<8)            +XF         ,
  (0x09<<8)            +XF+VF      ,
  (0x0A<<8)            +XF+VF      ,
  (0x0B<<8)            +XF         ,
  (0x0C<<8)            +XF+VF      ,
  (0x0D<<8)            +XF         ,
  (0x0E<<8)            +XF         ,
  (0x0F<<8)            +XF+VF      ,
  (0x10<<8)         +HF            ,
  (0x11<<8)         +HF   +VF      ,
  (0x12<<8)         +HF   +VF      ,
  (0x13<<8)         +HF            ,
  (0x14<<8)         +HF   +VF      ,
  (0x15<<8)         +HF            ,
  (0x16<<8)                        ,
  (0x17<<8)               +VF      ,
  (0x18<<8)            +XF+VF      ,
  (0x19<<8)            +XF         ,
  (0x1A<<8)            +XF         ,
  (0x1B<<8)            +XF+VF      ,
  (0x1C<<8)            +XF         ,
  (0x1D<<8)            +XF+VF      ,
  (0x1E<<8)            +XF+VF      ,
  (0x1F<<8)            +XF         ,
  (0x20<<8)      +YF+HF            ,
  (0x21<<8)      +YF+HF   +VF      ,
  (0x22<<8)      +YF+HF   +VF      ,
  (0x23<<8)      +YF+HF            ,
  (0x24<<8)      +YF+HF   +VF      ,
  (0x25<<8)      +YF+HF            ,
  (0x26<<8)      +YF               ,
  (0x27<<8)      +YF      +VF      ,
  (0x28<<8)      +YF   +XF+VF      ,
  (0x29<<8)      +YF   +XF         ,
  (0x2A<<8)      +YF   +XF         ,
  (0x2B<<8)      +YF   +XF+VF      ,
  (0x2C<<8)      +YF   +XF         ,
  (0x2D<<8)      +YF   +XF+VF      ,
  (0x2E<<8)      +YF   +XF+VF      ,
  (0x2F<<8)      +YF   +XF         ,
  (0x30<<8)      +YF+HF   +VF      ,
  (0x31<<8)      +YF+HF            ,
  (0x32<<8)      +YF+HF            ,
  (0x33<<8)      +YF+HF   +VF      ,
  (0x34<<8)      +YF+HF            ,
  (0x35<<8)      +YF+HF   +VF      ,
  (0x36<<8)      +YF      +VF      ,
  (0x37<<8)      +YF               ,
  (0x38<<8)      +YF   +XF         ,
  (0x39<<8)      +YF   +XF+VF      ,
  (0x3A<<8)      +YF   +XF+VF      ,
  (0x3B<<8)      +YF   +XF         ,
  (0x3C<<8)      +YF   +XF+VF      ,
  (0x3D<<8)      +YF   +XF         ,
  (0x3E<<8)      +YF   +XF         ,
  (0x3F<<8)      +YF   +XF+VF      ,
  (0x40<<8)         +HF            ,
  (0x41<<8)         +HF   +VF      ,
  (0x42<<8)         +HF   +VF      ,
  (0x43<<8)         +HF            ,
  (0x44<<8)         +HF   +VF      ,
  (0x45<<8)         +HF            ,
  (0x46<<8)                        ,
  (0x47<<8)               +VF      ,
  (0x48<<8)            +XF+VF      ,
  (0x49<<8)            +XF         ,
  (0x4A<<8)            +XF         ,
  (0x4B<<8)            +XF+VF      ,
  (0x4C<<8)            +XF         ,
  (0x4D<<8)            +XF+VF      ,
  (0x4E<<8)            +XF+VF      ,
  (0x4F<<8)            +XF         ,
  (0x50<<8)         +HF   +VF      ,
  (0x51<<8)         +HF            ,
  (0x52<<8)         +HF            ,
  (0x53<<8)         +HF   +VF      ,
  (0x54<<8)         +HF            ,
  (0x55<<8)         +HF   +VF      ,
  (0x56<<8)               +VF      ,
  (0x57<<8)                        ,
  (0x58<<8)            +XF         ,
  (0x59<<8)            +XF+VF      ,
  (0x5A<<8)            +XF+VF      ,
  (0x5B<<8)            +XF         ,
  (0x5C<<8)            +XF+VF      ,
  (0x5D<<8)            +XF         ,
  (0x5E<<8)            +XF         ,
  (0x5F<<8)            +XF+VF      ,
  (0x60<<8)      +YF+HF   +VF      ,
  (0x61<<8)      +YF+HF            ,
  (0x62<<8)      +YF+HF            ,
  (0x63<<8)      +YF+HF   +VF      ,
  (0x64<<8)      +YF+HF            ,
  (0x65<<8)      +YF+HF   +VF      ,
  (0x66<<8)      +YF      +VF      ,
  (0x67<<8)      +YF               ,
  (0x68<<8)      +YF   +XF         ,
  (0x69<<8)      +YF   +XF+VF      ,
  (0x6A<<8)      +YF   +XF+VF      ,
  (0x6B<<8)      +YF   +XF         ,
  (0x6C<<8)      +YF   +XF+VF      ,
  (0x6D<<8)      +YF   +XF         ,
  (0x6E<<8)      +YF   +XF         ,
  (0x6F<<8)      +YF   +XF+VF      ,
  (0x70<<8)      +YF+HF            ,
  (0x71<<8)      +YF+HF   +VF      ,
  (0x72<<8)      +YF+HF   +VF      ,
  (0x73<<8)      +YF+HF            ,
  (0x74<<8)      +YF+HF   +VF      ,
  (0x75<<8)      +YF+HF            ,
  (0x76<<8)      +YF               ,
  (0x77<<8)      +YF      +VF      ,
  (0x78<<8)      +YF   +XF+VF      ,
  (0x79<<8)      +YF   +XF         ,
  (0x7A<<8)      +YF   +XF         ,
  (0x7B<<8)      +YF   +XF+VF      ,
  (0x7C<<8)      +YF   +XF         ,
  (0x7D<<8)      +YF   +XF+VF      ,
  (0x7E<<8)      +YF   +XF+VF      ,
  (0x7F<<8)      +YF   +XF         ,
  (0x80<<8)+SF      +HF            ,
  (0x81<<8)+SF      +HF   +VF      ,
  (0x82<<8)+SF      +HF   +VF      ,
  (0x83<<8)+SF      +HF            ,
  (0x84<<8)+SF      +HF   +VF      ,
  (0x85<<8)+SF      +HF            ,
  (0x86<<8)+SF                     ,
  (0x87<<8)+SF            +VF      ,
  (0x88<<8)+SF         +XF+VF      ,
  (0x89<<8)+SF         +XF         ,
  (0x8A<<8)+SF         +XF         ,
  (0x8B<<8)+SF         +XF+VF      ,
  (0x8C<<8)+SF         +XF         ,
  (0x8D<<8)+SF         +XF+VF      ,
  (0x8E<<8)+SF         +XF+VF      ,
  (0x8F<<8)+SF         +XF         ,
  (0x90<<8)+SF      +HF   +VF      ,
  (0x91<<8)+SF      +HF            ,
  (0x92<<8)+SF      +HF            ,
  (0x93<<8)+SF      +HF   +VF      ,
  (0x94<<8)+SF      +HF            ,
  (0x95<<8)+SF      +HF   +VF      ,
  (0x96<<8)+SF            +VF      ,
  (0x97<<8)+SF                     ,
  (0x98<<8)+SF         +XF         ,
  (0x99<<8)+SF         +XF+VF      ,
  (0x9A<<8)+SF         +XF+VF      ,
  (0x9B<<8)+SF         +XF         ,
  (0x9C<<8)+SF         +XF+VF      ,
  (0x9D<<8)+SF         +XF         ,
  (0x9E<<8)+SF         +XF         ,
  (0x9F<<8)+SF         +XF+VF      ,
  (0x00<<8)   +ZF   +HF   +VF   +CF,
  (0x01<<8)         +HF         +CF,
  (0x02<<8)         +HF         +CF,
  (0x03<<8)         +HF   +VF   +CF,
  (0x04<<8)         +HF         +CF,
  (0x05<<8)         +HF   +VF   +CF,
  (0x06<<8)               +VF   +CF,
  (0x07<<8)                     +CF,
  (0x08<<8)            +XF      +CF,
  (0x09<<8)            +XF+VF   +CF,
  (0x0A<<8)            +XF+VF   +CF,
  (0x0B<<8)            +XF      +CF,
  (0x0C<<8)            +XF+VF   +CF,
  (0x0D<<8)            +XF      +CF,
  (0x0E<<8)            +XF      +CF,
  (0x0F<<8)            +XF+VF   +CF,
  (0x10<<8)         +HF         +CF,
  (0x11<<8)         +HF   +VF   +CF,
  (0x12<<8)         +HF   +VF   +CF,
  (0x13<<8)         +HF         +CF,
  (0x14<<8)         +HF   +VF   +CF,
  (0x15<<8)         +HF         +CF,
  (0x16<<8)                     +CF,
  (0x17<<8)               +VF   +CF,
  (0x18<<8)            +XF+VF   +CF,
  (0x19<<8)            +XF      +CF,
  (0x1A<<8)            +XF      +CF,
  (0x1B<<8)            +XF+VF   +CF,
  (0x1C<<8)            +XF      +CF,
  (0x1D<<8)            +XF+VF   +CF,
  (0x1E<<8)            +XF+VF   +CF,
  (0x1F<<8)            +XF      +CF,
  (0x20<<8)      +YF+HF         +CF,
  (0x21<<8)      +YF+HF   +VF   +CF,
  (0x22<<8)      +YF+HF   +VF   +CF,
  (0x23<<8)      +YF+HF         +CF,
  (0x24<<8)      +YF+HF   +VF   +CF,
  (0x25<<8)      +YF+HF         +CF,
  (0x26<<8)      +YF            +CF,
  (0x27<<8)      +YF      +VF   +CF,
  (0x28<<8)      +YF   +XF+VF   +CF,
  (0x29<<8)      +YF   +XF      +CF,
  (0x2A<<8)      +YF   +XF      +CF,
  (0x2B<<8)      +YF   +XF+VF   +CF,
  (0x2C<<8)      +YF   +XF      +CF,
  (0x2D<<8)      +YF   +XF+VF   +CF,
  (0x2E<<8)      +YF   +XF+VF   +CF,
  (0x2F<<8)      +YF   +XF      +CF,
  (0x30<<8)      +YF+HF   +VF   +CF,
  (0x31<<8)      +YF+HF         +CF,
  (0x32<<8)      +YF+HF         +CF,
  (0x33<<8)      +YF+HF   +VF   +CF,
  (0x34<<8)      +YF+HF         +CF,
  (0x35<<8)      +YF+HF   +VF   +CF,
  (0x36<<8)      +YF      +VF   +CF,
  (0x37<<8)      +YF            +CF,
  (0x38<<8)      +YF   +XF      +CF,
  (0x39<<8)      +YF   +XF+VF   +CF,
  (0x3A<<8)      +YF   +XF+VF   +CF,
  (0x3B<<8)      +YF   +XF      +CF,
  (0x3C<<8)      +YF   +XF+VF   +CF,
  (0x3D<<8)      +YF   +XF      +CF,
  (0x3E<<8)      +YF   +XF      +CF,
  (0x3F<<8)      +YF   +XF+VF   +CF,
  (0x40<<8)         +HF         +CF,
  (0x41<<8)         +HF   +VF   +CF,
  (0x42<<8)         +HF   +VF   +CF,
  (0x43<<8)         +HF         +CF,
  (0x44<<8)         +HF   +VF   +CF,
  (0x45<<8)         +HF         +CF,
  (0x46<<8)                     +CF,
  (0x47<<8)               +VF   +CF,
  (0x48<<8)            +XF+VF   +CF,
  (0x49<<8)            +XF      +CF,
  (0x4A<<8)            +XF      +CF,
  (0x4B<<8)            +XF+VF   +CF,
  (0x4C<<8)            +XF      +CF,
  (0x4D<<8)            +XF+VF   +CF,
  (0x4E<<8)            +XF+VF   +CF,
  (0x4F<<8)            +XF      +CF,
  (0x50<<8)         +HF   +VF   +CF,
  (0x51<<8)         +HF         +CF,
  (0x52<<8)         +HF         +CF,
  (0x53<<8)         +HF   +VF   +CF,
  (0x54<<8)         +HF         +CF,
  (0x55<<8)         +HF   +VF   +CF,
  (0x56<<8)               +VF   +CF,
  (0x57<<8)                     +CF,
  (0x58<<8)            +XF      +CF,
  (0x59<<8)            +XF+VF   +CF,
  (0x5A<<8)            +XF+VF   +CF,
  (0x5B<<8)            +XF      +CF,
  (0x5C<<8)            +XF+VF   +CF,
  (0x5D<<8)            +XF      +CF,
  (0x5E<<8)            +XF      +CF,
  (0x5F<<8)            +XF+VF   +CF,
  (0x60<<8)      +YF+HF   +VF   +CF,
  (0x61<<8)      +YF+HF         +CF,
  (0x62<<8)      +YF+HF         +CF,
  (0x63<<8)      +YF+HF   +VF   +CF,
  (0x64<<8)      +YF+HF         +CF,
  (0x65<<8)      +YF+HF   +VF   +CF,
  (0x66<<8)      +YF      +VF   +CF,
  (0x67<<8)      +YF            +CF,
  (0x68<<8)      +YF   +XF      +CF,
  (0x69<<8)      +YF   +XF+VF   +CF,
  (0x6A<<8)      +YF   +XF+VF   +CF,
  (0x6B<<8)      +YF   +XF      +CF,
  (0x6C<<8)      +YF   +XF+VF   +CF,
  (0x6D<<8)      +YF   +XF      +CF,
  (0x6E<<8)      +YF   +XF      +CF,
  (0x6F<<8)      +YF   +XF+VF   +CF,
  (0x70<<8)      +YF+HF         +CF,
  (0x71<<8)      +YF+HF   +VF   +CF,
  (0x72<<8)      +YF+HF   +VF   +CF,
  (0x73<<8)      +YF+HF         +CF,
  (0x74<<8)      +YF+HF   +VF   +CF,
  (0x75<<8)      +YF+HF         +CF,
  (0x76<<8)      +YF            +CF,
  (0x77<<8)      +YF      +VF   +CF,
  (0x78<<8)      +YF   +XF+VF   +CF,
  (0x79<<8)      +YF   +XF      +CF,
  (0x7A<<8)      +YF   +XF      +CF,
  (0x7B<<8)      +YF   +XF+VF   +CF,
  (0x7C<<8)      +YF   +XF      +CF,
  (0x7D<<8)      +YF   +XF+VF   +CF,
  (0x7E<<8)      +YF   +XF+VF   +CF,
  (0x7F<<8)      +YF   +XF      +CF,
  (0x80<<8)+SF      +HF         +CF,
  (0x81<<8)+SF      +HF   +VF   +CF,
  (0x82<<8)+SF      +HF   +VF   +CF,
  (0x83<<8)+SF      +HF         +CF,
  (0x84<<8)+SF      +HF   +VF   +CF,
  (0x85<<8)+SF      +HF         +CF,
  (0x86<<8)+SF                  +CF,
  (0x87<<8)+SF            +VF   +CF,
  (0x88<<8)+SF         +XF+VF   +CF,
  (0x89<<8)+SF         +XF      +CF,
  (0x8A<<8)+SF         +XF      +CF,
  (0x8B<<8)+SF         +XF+VF   +CF,
  (0x8C<<8)+SF         +XF      +CF,
  (0x8D<<8)+SF         +XF+VF   +CF,
  (0x8E<<8)+SF         +XF+VF   +CF,
  (0x8F<<8)+SF         +XF      +CF,
  (0x90<<8)+SF      +HF   +VF   +CF,
  (0x91<<8)+SF      +HF         +CF,
  (0x92<<8)+SF      +HF         +CF,
  (0x93<<8)+SF      +HF   +VF   +CF,
  (0x94<<8)+SF      +HF         +CF,
  (0x95<<8)+SF      +HF   +VF   +CF,
  (0x96<<8)+SF            +VF   +CF,
  (0x97<<8)+SF                  +CF,
  (0x98<<8)+SF         +XF      +CF,
  (0x99<<8)+SF         +XF+VF   +CF,
  (0x9A<<8)+SF         +XF+VF   +CF,
  (0x9B<<8)+SF         +XF      +CF,
  (0x9C<<8)+SF         +XF+VF   +CF,
  (0x9D<<8)+SF         +XF      +CF,
  (0x9E<<8)+SF         +XF      +CF,
  (0x9F<<8)+SF         +XF+VF   +CF,
  (0xA0<<8)+SF   +YF+HF   +VF   +CF,
  (0xA1<<8)+SF   +YF+HF         +CF,
  (0xA2<<8)+SF   +YF+HF         +CF,
  (0xA3<<8)+SF   +YF+HF   +VF   +CF,
  (0xA4<<8)+SF   +YF+HF         +CF,
  (0xA5<<8)+SF   +YF+HF   +VF   +CF,
  (0xA6<<8)+SF   +YF      +VF   +CF,
  (0xA7<<8)+SF   +YF            +CF,
  (0xA8<<8)+SF   +YF   +XF      +CF,
  (0xA9<<8)+SF   +YF   +XF+VF   +CF,
  (0xAA<<8)+SF   +YF   +XF+VF   +CF,
  (0xAB<<8)+SF   +YF   +XF      +CF,
  (0xAC<<8)+SF   +YF   +XF+VF   +CF,
  (0xAD<<8)+SF   +YF   +XF      +CF,
  (0xAE<<8)+SF   +YF   +XF      +CF,
  (0xAF<<8)+SF   +YF   +XF+VF   +CF,
  (0xB0<<8)+SF   +YF+HF         +CF,
  (0xB1<<8)+SF   +YF+HF   +VF   +CF,
  (0xB2<<8)+SF   +YF+HF   +VF   +CF,
  (0xB3<<8)+SF   +YF+HF         +CF,
  (0xB4<<8)+SF   +YF+HF   +VF   +CF,
  (0xB5<<8)+SF   +YF+HF         +CF,
  (0xB6<<8)+SF   +YF            +CF,
  (0xB7<<8)+SF   +YF      +VF   +CF,
  (0xB8<<8)+SF   +YF   +XF+VF   +CF,
  (0xB9<<8)+SF   +YF   +XF      +CF,
  (0xBA<<8)+SF   +YF   +XF      +CF,
  (0xBB<<8)+SF   +YF   +XF+VF   +CF,
  (0xBC<<8)+SF   +YF   +XF      +CF,
  (0xBD<<8)+SF   +YF   +XF+VF   +CF,
  (0xBE<<8)+SF   +YF   +XF+VF   +CF,
  (0xBF<<8)+SF   +YF   +XF      +CF,
  (0xC0<<8)+SF      +HF   +VF   +CF,
  (0xC1<<8)+SF      +HF         +CF,
  (0xC2<<8)+SF      +HF         +CF,
  (0xC3<<8)+SF      +HF   +VF   +CF,
  (0xC4<<8)+SF      +HF         +CF,
  (0xC5<<8)+SF      +HF   +VF   +CF,
  (0xC6<<8)+SF            +VF   +CF,
  (0xC7<<8)+SF                  +CF,
  (0xC8<<8)+SF         +XF      +CF,
  (0xC9<<8)+SF         +XF+VF   +CF,
  (0xCA<<8)+SF         +XF+VF   +CF,
  (0xCB<<8)+SF         +XF      +CF,
  (0xCC<<8)+SF         +XF+VF   +CF,
  (0xCD<<8)+SF         +XF      +CF,
  (0xCE<<8)+SF         +XF      +CF,
  (0xCF<<8)+SF         +XF+VF   +CF,
  (0xD0<<8)+SF      +HF         +CF,
  (0xD1<<8)+SF      +HF   +VF   +CF,
  (0xD2<<8)+SF      +HF   +VF   +CF,
  (0xD3<<8)+SF      +HF         +CF,
  (0xD4<<8)+SF      +HF   +VF   +CF,
  (0xD5<<8)+SF      +HF         +CF,
  (0xD6<<8)+SF                  +CF,
  (0xD7<<8)+SF            +VF   +CF,
  (0xD8<<8)+SF         +XF+VF   +CF,
  (0xD9<<8)+SF         +XF      +CF,
  (0xDA<<8)+SF         +XF      +CF,
  (0xDB<<8)+SF         +XF+VF   +CF,
  (0xDC<<8)+SF         +XF      +CF,
  (0xDD<<8)+SF         +XF+VF   +CF,
  (0xDE<<8)+SF         +XF+VF   +CF,
  (0xDF<<8)+SF         +XF      +CF,
  (0xE0<<8)+SF   +YF+HF         +CF,
  (0xE1<<8)+SF   +YF+HF   +VF   +CF,
  (0xE2<<8)+SF   +YF+HF   +VF   +CF,
  (0xE3<<8)+SF   +YF+HF         +CF,
  (0xE4<<8)+SF   +YF+HF   +VF   +CF,
  (0xE5<<8)+SF   +YF+HF         +CF,
  (0xE6<<8)+SF   +YF            +CF,
  (0xE7<<8)+SF   +YF      +VF   +CF,
  (0xE8<<8)+SF   +YF   +XF+VF   +CF,
  (0xE9<<8)+SF   +YF   +XF      +CF,
  (0xEA<<8)+SF   +YF   +XF      +CF,
  (0xEB<<8)+SF   +YF   +XF+VF   +CF,
  (0xEC<<8)+SF   +YF   +XF      +CF,
  (0xED<<8)+SF   +YF   +XF+VF   +CF,
  (0xEE<<8)+SF   +YF   +XF+VF   +CF,
  (0xEF<<8)+SF   +YF   +XF      +CF,
  (0xF0<<8)+SF   +YF+HF   +VF   +CF,
  (0xF1<<8)+SF   +YF+HF         +CF,
  (0xF2<<8)+SF   +YF+HF         +CF,
  (0xF3<<8)+SF   +YF+HF   +VF   +CF,
  (0xF4<<8)+SF   +YF+HF         +CF,
  (0xF5<<8)+SF   +YF+HF   +VF   +CF,
  (0xF6<<8)+SF   +YF      +VF   +CF,
  (0xF7<<8)+SF   +YF            +CF,
  (0xF8<<8)+SF   +YF   +XF      +CF,
  (0xF9<<8)+SF   +YF   +XF+VF   +CF,
  (0xFA<<8)+SF   +YF   +XF+VF   +CF,
  (0xFB<<8)+SF   +YF   +XF      +CF,
  (0xFC<<8)+SF   +YF   +XF+VF   +CF,
  (0xFD<<8)+SF   +YF   +XF      +CF,
  (0xFE<<8)+SF   +YF   +XF      +CF,
  (0xFF<<8)+SF   +YF   +XF+VF   +CF,
  (0x00<<8)   +ZF   +HF   +VF   +CF,
  (0x01<<8)         +HF         +CF,
  (0x02<<8)         +HF         +CF,
  (0x03<<8)         +HF   +VF   +CF,
  (0x04<<8)         +HF         +CF,
  (0x05<<8)         +HF   +VF   +CF,
  (0x06<<8)               +VF   +CF,
  (0x07<<8)                     +CF,
  (0x08<<8)            +XF      +CF,
  (0x09<<8)            +XF+VF   +CF,
  (0x0A<<8)            +XF+VF   +CF,
  (0x0B<<8)            +XF      +CF,
  (0x0C<<8)            +XF+VF   +CF,
  (0x0D<<8)            +XF      +CF,
  (0x0E<<8)            +XF      +CF,
  (0x0F<<8)            +XF+VF   +CF,
  (0x10<<8)         +HF         +CF,
  (0x11<<8)         +HF   +VF   +CF,
  (0x12<<8)         +HF   +VF   +CF,
  (0x13<<8)         +HF         +CF,
  (0x14<<8)         +HF   +VF   +CF,
  (0x15<<8)         +HF         +CF,
  (0x16<<8)                     +CF,
  (0x17<<8)               +VF   +CF,
  (0x18<<8)            +XF+VF   +CF,
  (0x19<<8)            +XF      +CF,
  (0x1A<<8)            +XF      +CF,
  (0x1B<<8)            +XF+VF   +CF,
  (0x1C<<8)            +XF      +CF,
  (0x1D<<8)            +XF+VF   +CF,
  (0x1E<<8)            +XF+VF   +CF,
  (0x1F<<8)            +XF      +CF,
  (0x20<<8)      +YF+HF         +CF,
  (0x21<<8)      +YF+HF   +VF   +CF,
  (0x22<<8)      +YF+HF   +VF   +CF,
  (0x23<<8)      +YF+HF         +CF,
  (0x24<<8)      +YF+HF   +VF   +CF,
  (0x25<<8)      +YF+HF         +CF,
  (0x26<<8)      +YF            +CF,
  (0x27<<8)      +YF      +VF   +CF,
  (0x28<<8)      +YF   +XF+VF   +CF,
  (0x29<<8)      +YF   +XF      +CF,
  (0x2A<<8)      +YF   +XF      +CF,
  (0x2B<<8)      +YF   +XF+VF   +CF,
  (0x2C<<8)      +YF   +XF      +CF,
  (0x2D<<8)      +YF   +XF+VF   +CF,
  (0x2E<<8)      +YF   +XF+VF   +CF,
  (0x2F<<8)      +YF   +XF      +CF,
  (0x30<<8)      +YF+HF   +VF   +CF,
  (0x31<<8)      +YF+HF         +CF,
  (0x32<<8)      +YF+HF         +CF,
  (0x33<<8)      +YF+HF   +VF   +CF,
  (0x34<<8)      +YF+HF         +CF,
  (0x35<<8)      +YF+HF   +VF   +CF,
  (0x36<<8)      +YF      +VF   +CF,
  (0x37<<8)      +YF            +CF,
  (0x38<<8)      +YF   +XF      +CF,
  (0x39<<8)      +YF   +XF+VF   +CF,
  (0x3A<<8)      +YF   +XF+VF   +CF,
  (0x3B<<8)      +YF   +XF      +CF,
  (0x3C<<8)      +YF   +XF+VF   +CF,
  (0x3D<<8)      +YF   +XF      +CF,
  (0x3E<<8)      +YF   +XF      +CF,
  (0x3F<<8)      +YF   +XF+VF   +CF,
  (0x40<<8)         +HF         +CF,
  (0x41<<8)         +HF   +VF   +CF,
  (0x42<<8)         +HF   +VF   +CF,
  (0x43<<8)         +HF         +CF,
  (0x44<<8)         +HF   +VF   +CF,
  (0x45<<8)         +HF         +CF,
  (0x46<<8)                     +CF,
  (0x47<<8)               +VF   +CF,
  (0x48<<8)            +XF+VF   +CF,
  (0x49<<8)            +XF      +CF,
  (0x4A<<8)            +XF      +CF,
  (0x4B<<8)            +XF+VF   +CF,
  (0x4C<<8)            +XF      +CF,
  (0x4D<<8)            +XF+VF   +CF,
  (0x4E<<8)            +XF+VF   +CF,
  (0x4F<<8)            +XF      +CF,
  (0x50<<8)         +HF   +VF   +CF,
  (0x51<<8)         +HF         +CF,
  (0x52<<8)         +HF         +CF,
  (0x53<<8)         +HF   +VF   +CF,
  (0x54<<8)         +HF         +CF,
  (0x55<<8)         +HF   +VF   +CF,
  (0x56<<8)               +VF   +CF,
  (0x57<<8)                     +CF,
  (0x58<<8)            +XF      +CF,
  (0x59<<8)            +XF+VF   +CF,
  (0x5A<<8)            +XF+VF   +CF,
  (0x5B<<8)            +XF      +CF,
  (0x5C<<8)            +XF+VF   +CF,
  (0x5D<<8)            +XF      +CF,
  (0x5E<<8)            +XF      +CF,
  (0x5F<<8)            +XF+VF   +CF,
  (0x60<<8)      +YF+HF   +VF   +CF,
  (0x61<<8)      +YF+HF         +CF,
  (0x62<<8)      +YF+HF         +CF,
  (0x63<<8)      +YF+HF   +VF   +CF,
  (0x64<<8)      +YF+HF         +CF,
  (0x65<<8)      +YF+HF   +VF   +CF,
  (0x00<<8)   +ZF         +VF+NF   ,
  (0x01<<8)                  +NF   ,
  (0x02<<8)                  +NF   ,
  (0x03<<8)               +VF+NF   ,
  (0x04<<8)                  +NF   ,
  (0x05<<8)               +VF+NF   ,
  (0x06<<8)               +VF+NF   ,
  (0x07<<8)                  +NF   ,
  (0x08<<8)            +XF   +NF   ,
  (0x09<<8)            +XF+VF+NF   ,
  (0x04<<8)                  +NF   ,
  (0x05<<8)               +VF+NF   ,
  (0x06<<8)               +VF+NF   ,
  (0x07<<8)                  +NF   ,
  (0x08<<8)            +XF   +NF   ,
  (0x09<<8)            +XF+VF+NF   ,
  (0x10<<8)                  +NF   ,
  (0x11<<8)               +VF+NF   ,
  (0x12<<8)               +VF+NF   ,
  (0x13<<8)                  +NF   ,
  (0x14<<8)               +VF+NF   ,
  (0x15<<8)                  +NF   ,
  (0x16<<8)                  +NF   ,
  (0x17<<8)               +VF+NF   ,
  (0x18<<8)            +XF+VF+NF   ,
  (0x19<<8)            +XF   +NF   ,
  (0x14<<8)               +VF+NF   ,
  (0x15<<8)                  +NF   ,
  (0x16<<8)                  +NF   ,
  (0x17<<8)               +VF+NF   ,
  (0x18<<8)            +XF+VF+NF   ,
  (0x19<<8)            +XF   +NF   ,
  (0x20<<8)      +YF         +NF   ,
  (0x21<<8)      +YF      +VF+NF   ,
  (0x22<<8)      +YF      +VF+NF   ,
  (0x23<<8)      +YF         +NF   ,
  (0x24<<8)      +YF      +VF+NF   ,
  (0x25<<8)      +YF         +NF   ,
  (0x26<<8)      +YF         +NF   ,
  (0x27<<8)      +YF      +VF+NF   ,
  (0x28<<8)      +YF   +XF+VF+NF   ,
  (0x29<<8)      +YF   +XF   +NF   ,
  (0x24<<8)      +YF      +VF+NF   ,
  (0x25<<8)      +YF         +NF   ,
  (0x26<<8)      +YF         +NF   ,
  (0x27<<8)      +YF      +VF+NF   ,
  (0x28<<8)      +YF   +XF+VF+NF   ,
  (0x29<<8)      +YF   +XF   +NF   ,
  (0x30<<8)      +YF      +VF+NF   ,
  (0x31<<8)      +YF         +NF   ,
  (0x32<<8)      +YF         +NF   ,
  (0x33<<8)      +YF      +VF+NF   ,
  (0x34<<8)      +YF         +NF   ,
  (0x35<<8)      +YF      +VF+NF   ,
  (0x36<<8)      +YF      +VF+NF   ,
  (0x37<<8)      +YF         +NF   ,
  (0x38<<8)      +YF   +XF   +NF   ,
  (0x39<<8)      +YF   +XF+VF+NF   ,
  (0x34<<8)      +YF         +NF   ,
  (0x35<<8)      +YF      +VF+NF   ,
  (0x36<<8)      +YF      +VF+NF   ,
  (0x37<<8)      +YF         +NF   ,
  (0x38<<8)      +YF   +XF   +NF   ,
  (0x39<<8)      +YF   +XF+VF+NF   ,
  (0x40<<8)                  +NF   ,
  (0x41<<8)               +VF+NF   ,
  (0x42<<8)               +VF+NF   ,
  (0x43<<8)                  +NF   ,
  (0x44<<8)               +VF+NF   ,
  (0x45<<8)                  +NF   ,
  (0x46<<8)                  +NF   ,
  (0x47<<8)               +VF+NF   ,
  (0x48<<8)            +XF+VF+NF   ,
  (0x49<<8)            +XF   +NF   ,
  (0x44<<8)               +VF+NF   ,
  (0x45<<8)                  +NF   ,
  (0x46<<8)                  +NF   ,
  (0x47<<8)               +VF+NF   ,
  (0x48<<8)            +XF+VF+NF   ,
  (0x49<<8)            +XF   +NF   ,
  (0x50<<8)               +VF+NF   ,
  (0x51<<8)                  +NF   ,
  (0x52<<8)                  +NF   ,
  (0x53<<8)               +VF+NF   ,
  (0x54<<8)                  +NF   ,
  (0x55<<8)               +VF+NF   ,
  (0x56<<8)               +VF+NF   ,
  (0x57<<8)                  +NF   ,
  (0x58<<8)            +XF   +NF   ,
  (0x59<<8)            +XF+VF+NF   ,
  (0x54<<8)                  +NF   ,
  (0x55<<8)               +VF+NF   ,
  (0x56<<8)               +VF+NF   ,
  (0x57<<8)                  +NF   ,
  (0x58<<8)            +XF   +NF   ,
  (0x59<<8)            +XF+VF+NF   ,
  (0x60<<8)      +YF      +VF+NF   ,
  (0x61<<8)      +YF         +NF   ,
  (0x62<<8)      +YF         +NF   ,
  (0x63<<8)      +YF      +VF+NF   ,
  (0x64<<8)      +YF         +NF   ,
  (0x65<<8)      +YF      +VF+NF   ,
  (0x66<<8)      +YF      +VF+NF   ,
  (0x67<<8)      +YF         +NF   ,
  (0x68<<8)      +YF   +XF   +NF   ,
  (0x69<<8)      +YF   +XF+VF+NF   ,
  (0x64<<8)      +YF         +NF   ,
  (0x65<<8)      +YF      +VF+NF   ,
  (0x66<<8)      +YF      +VF+NF   ,
  (0x67<<8)      +YF         +NF   ,
  (0x68<<8)      +YF   +XF   +NF   ,
  (0x69<<8)      +YF   +XF+VF+NF   ,
  (0x70<<8)      +YF         +NF   ,
  (0x71<<8)      +YF      +VF+NF   ,
  (0x72<<8)      +YF      +VF+NF   ,
  (0x73<<8)      +YF         +NF   ,
  (0x74<<8)      +YF      +VF+NF   ,
  (0x75<<8)      +YF         +NF   ,
  (0x76<<8)      +YF         +NF   ,
  (0x77<<8)      +YF      +VF+NF   ,
  (0x78<<8)      +YF   +XF+VF+NF   ,
  (0x79<<8)      +YF   +XF   +NF   ,
  (0x74<<8)      +YF      +VF+NF   ,
  (0x75<<8)      +YF         +NF   ,
  (0x76<<8)      +YF         +NF   ,
  (0x77<<8)      +YF      +VF+NF   ,
  (0x78<<8)      +YF   +XF+VF+NF   ,
  (0x79<<8)      +YF   +XF   +NF   ,
  (0x80<<8)+SF               +NF   ,
  (0x81<<8)+SF            +VF+NF   ,
  (0x82<<8)+SF            +VF+NF   ,
  (0x83<<8)+SF               +NF   ,
  (0x84<<8)+SF            +VF+NF   ,
  (0x85<<8)+SF               +NF   ,
  (0x86<<8)+SF               +NF   ,
  (0x87<<8)+SF            +VF+NF   ,
  (0x88<<8)+SF         +XF+VF+NF   ,
  (0x89<<8)+SF         +XF   +NF   ,
  (0x84<<8)+SF            +VF+NF   ,
  (0x85<<8)+SF               +NF   ,
  (0x86<<8)+SF               +NF   ,
  (0x87<<8)+SF            +VF+NF   ,
  (0x88<<8)+SF         +XF+VF+NF   ,
  (0x89<<8)+SF         +XF   +NF   ,
  (0x90<<8)+SF            +VF+NF   ,
  (0x91<<8)+SF               +NF   ,
  (0x92<<8)+SF               +NF   ,
  (0x93<<8)+SF            +VF+NF   ,
  (0x94<<8)+SF               +NF   ,
  (0x95<<8)+SF            +VF+NF   ,
  (0x96<<8)+SF            +VF+NF   ,
  (0x97<<8)+SF               +NF   ,
  (0x98<<8)+SF         +XF   +NF   ,
  (0x99<<8)+SF         +XF+VF+NF   ,
  (0x34<<8)      +YF         +NF+CF,
  (0x35<<8)      +YF      +VF+NF+CF,
  (0x36<<8)      +YF      +VF+NF+CF,
  (0x37<<8)      +YF         +NF+CF,
  (0x38<<8)      +YF   +XF   +NF+CF,
  (0x39<<8)      +YF   +XF+VF+NF+CF,
  (0x40<<8)                  +NF+CF,
  (0x41<<8)               +VF+NF+CF,
  (0x42<<8)               +VF+NF+CF,
  (0x43<<8)                  +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x50<<8)               +VF+NF+CF,
  (0x51<<8)                  +NF+CF,
  (0x52<<8)                  +NF+CF,
  (0x53<<8)               +VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x60<<8)      +YF      +VF+NF+CF,
  (0x61<<8)      +YF         +NF+CF,
  (0x62<<8)      +YF         +NF+CF,
  (0x63<<8)      +YF      +VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x70<<8)      +YF         +NF+CF,
  (0x71<<8)      +YF      +VF+NF+CF,
  (0x72<<8)      +YF      +VF+NF+CF,
  (0x73<<8)      +YF         +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x80<<8)+SF               +NF+CF,
  (0x81<<8)+SF            +VF+NF+CF,
  (0x82<<8)+SF            +VF+NF+CF,
  (0x83<<8)+SF               +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x90<<8)+SF            +VF+NF+CF,
  (0x91<<8)+SF               +NF+CF,
  (0x92<<8)+SF               +NF+CF,
  (0x93<<8)+SF            +VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF,
  (0xA0<<8)+SF   +YF      +VF+NF+CF,
  (0xA1<<8)+SF   +YF         +NF+CF,
  (0xA2<<8)+SF   +YF         +NF+CF,
  (0xA3<<8)+SF   +YF      +VF+NF+CF,
  (0xA4<<8)+SF   +YF         +NF+CF,
  (0xA5<<8)+SF   +YF      +VF+NF+CF,
  (0xA6<<8)+SF   +YF      +VF+NF+CF,
  (0xA7<<8)+SF   +YF         +NF+CF,
  (0xA8<<8)+SF   +YF   +XF   +NF+CF,
  (0xA9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xA4<<8)+SF   +YF         +NF+CF,
  (0xA5<<8)+SF   +YF      +VF+NF+CF,
  (0xA6<<8)+SF   +YF      +VF+NF+CF,
  (0xA7<<8)+SF   +YF         +NF+CF,
  (0xA8<<8)+SF   +YF   +XF   +NF+CF,
  (0xA9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xB0<<8)+SF   +YF         +NF+CF,
  (0xB1<<8)+SF   +YF      +VF+NF+CF,
  (0xB2<<8)+SF   +YF      +VF+NF+CF,
  (0xB3<<8)+SF   +YF         +NF+CF,
  (0xB4<<8)+SF   +YF      +VF+NF+CF,
  (0xB5<<8)+SF   +YF         +NF+CF,
  (0xB6<<8)+SF   +YF         +NF+CF,
  (0xB7<<8)+SF   +YF      +VF+NF+CF,
  (0xB8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xB9<<8)+SF   +YF   +XF   +NF+CF,
  (0xB4<<8)+SF   +YF      +VF+NF+CF,
  (0xB5<<8)+SF   +YF         +NF+CF,
  (0xB6<<8)+SF   +YF         +NF+CF,
  (0xB7<<8)+SF   +YF      +VF+NF+CF,
  (0xB8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xB9<<8)+SF   +YF   +XF   +NF+CF,
  (0xC0<<8)+SF            +VF+NF+CF,
  (0xC1<<8)+SF               +NF+CF,
  (0xC2<<8)+SF               +NF+CF,
  (0xC3<<8)+SF            +VF+NF+CF,
  (0xC4<<8)+SF               +NF+CF,
  (0xC5<<8)+SF            +VF+NF+CF,
  (0xC6<<8)+SF            +VF+NF+CF,
  (0xC7<<8)+SF               +NF+CF,
  (0xC8<<8)+SF         +XF   +NF+CF,
  (0xC9<<8)+SF         +XF+VF+NF+CF,
  (0xC4<<8)+SF               +NF+CF,
  (0xC5<<8)+SF            +VF+NF+CF,
  (0xC6<<8)+SF            +VF+NF+CF,
  (0xC7<<8)+SF               +NF+CF,
  (0xC8<<8)+SF         +XF   +NF+CF,
  (0xC9<<8)+SF         +XF+VF+NF+CF,
  (0xD0<<8)+SF               +NF+CF,
  (0xD1<<8)+SF            +VF+NF+CF,
  (0xD2<<8)+SF            +VF+NF+CF,
  (0xD3<<8)+SF               +NF+CF,
  (0xD4<<8)+SF            +VF+NF+CF,
  (0xD5<<8)+SF               +NF+CF,
  (0xD6<<8)+SF               +NF+CF,
  (0xD7<<8)+SF            +VF+NF+CF,
  (0xD8<<8)+SF         +XF+VF+NF+CF,
  (0xD9<<8)+SF         +XF   +NF+CF,
  (0xD4<<8)+SF            +VF+NF+CF,
  (0xD5<<8)+SF               +NF+CF,
  (0xD6<<8)+SF               +NF+CF,
  (0xD7<<8)+SF            +VF+NF+CF,
  (0xD8<<8)+SF         +XF+VF+NF+CF,
  (0xD9<<8)+SF         +XF   +NF+CF,
  (0xE0<<8)+SF   +YF         +NF+CF,
  (0xE1<<8)+SF   +YF      +VF+NF+CF,
  (0xE2<<8)+SF   +YF      +VF+NF+CF,
  (0xE3<<8)+SF   +YF         +NF+CF,
  (0xE4<<8)+SF   +YF      +VF+NF+CF,
  (0xE5<<8)+SF   +YF         +NF+CF,
  (0xE6<<8)+SF   +YF         +NF+CF,
  (0xE7<<8)+SF   +YF      +VF+NF+CF,
  (0xE8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xE9<<8)+SF   +YF   +XF   +NF+CF,
  (0xE4<<8)+SF   +YF      +VF+NF+CF,
  (0xE5<<8)+SF   +YF         +NF+CF,
  (0xE6<<8)+SF   +YF         +NF+CF,
  (0xE7<<8)+SF   +YF      +VF+NF+CF,
  (0xE8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xE9<<8)+SF   +YF   +XF   +NF+CF,
  (0xF0<<8)+SF   +YF      +VF+NF+CF,
  (0xF1<<8)+SF   +YF         +NF+CF,
  (0xF2<<8)+SF   +YF         +NF+CF,
  (0xF3<<8)+SF   +YF      +VF+NF+CF,
  (0xF4<<8)+SF   +YF         +NF+CF,
  (0xF5<<8)+SF   +YF      +VF+NF+CF,
  (0xF6<<8)+SF   +YF      +VF+NF+CF,
  (0xF7<<8)+SF   +YF         +NF+CF,
  (0xF8<<8)+SF   +YF   +XF   +NF+CF,
  (0xF9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xF4<<8)+SF   +YF         +NF+CF,
  (0xF5<<8)+SF   +YF      +VF+NF+CF,
  (0xF6<<8)+SF   +YF      +VF+NF+CF,
  (0xF7<<8)+SF   +YF         +NF+CF,
  (0xF8<<8)+SF   +YF   +XF   +NF+CF,
  (0xF9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0x00<<8)   +ZF         +VF+NF+CF,
  (0x01<<8)                  +NF+CF,
  (0x02<<8)                  +NF+CF,
  (0x03<<8)               +VF+NF+CF,
  (0x04<<8)                  +NF+CF,
  (0x05<<8)               +VF+NF+CF,
  (0x06<<8)               +VF+NF+CF,
  (0x07<<8)                  +NF+CF,
  (0x08<<8)            +XF   +NF+CF,
  (0x09<<8)            +XF+VF+NF+CF,
  (0x04<<8)                  +NF+CF,
  (0x05<<8)               +VF+NF+CF,
  (0x06<<8)               +VF+NF+CF,
  (0x07<<8)                  +NF+CF,
  (0x08<<8)            +XF   +NF+CF,
  (0x09<<8)            +XF+VF+NF+CF,
  (0x10<<8)                  +NF+CF,
  (0x11<<8)               +VF+NF+CF,
  (0x12<<8)               +VF+NF+CF,
  (0x13<<8)                  +NF+CF,
  (0x14<<8)               +VF+NF+CF,
  (0x15<<8)                  +NF+CF,
  (0x16<<8)                  +NF+CF,
  (0x17<<8)               +VF+NF+CF,
  (0x18<<8)            +XF+VF+NF+CF,
  (0x19<<8)            +XF   +NF+CF,
  (0x14<<8)               +VF+NF+CF,
  (0x15<<8)                  +NF+CF,
  (0x16<<8)                  +NF+CF,
  (0x17<<8)               +VF+NF+CF,
  (0x18<<8)            +XF+VF+NF+CF,
  (0x19<<8)            +XF   +NF+CF,
  (0x20<<8)      +YF         +NF+CF,
  (0x21<<8)      +YF      +VF+NF+CF,
  (0x22<<8)      +YF      +VF+NF+CF,
  (0x23<<8)      +YF         +NF+CF,
  (0x24<<8)      +YF      +VF+NF+CF,
  (0x25<<8)      +YF         +NF+CF,
  (0x26<<8)      +YF         +NF+CF,
  (0x27<<8)      +YF      +VF+NF+CF,
  (0x28<<8)      +YF   +XF+VF+NF+CF,
  (0x29<<8)      +YF   +XF   +NF+CF,
  (0x24<<8)      +YF      +VF+NF+CF,
  (0x25<<8)      +YF         +NF+CF,
  (0x26<<8)      +YF         +NF+CF,
  (0x27<<8)      +YF      +VF+NF+CF,
  (0x28<<8)      +YF   +XF+VF+NF+CF,
  (0x29<<8)      +YF   +XF   +NF+CF,
  (0x30<<8)      +YF      +VF+NF+CF,
  (0x31<<8)      +YF         +NF+CF,
  (0x32<<8)      +YF         +NF+CF,
  (0x33<<8)      +YF      +VF+NF+CF,
  (0x34<<8)      +YF         +NF+CF,
  (0x35<<8)      +YF      +VF+NF+CF,
  (0x36<<8)      +YF      +VF+NF+CF,
  (0x37<<8)      +YF         +NF+CF,
  (0x38<<8)      +YF   +XF   +NF+CF,
  (0x39<<8)      +YF   +XF+VF+NF+CF,
  (0x34<<8)      +YF         +NF+CF,
  (0x35<<8)      +YF      +VF+NF+CF,
  (0x36<<8)      +YF      +VF+NF+CF,
  (0x37<<8)      +YF         +NF+CF,
  (0x38<<8)      +YF   +XF   +NF+CF,
  (0x39<<8)      +YF   +XF+VF+NF+CF,
  (0x40<<8)                  +NF+CF,
  (0x41<<8)               +VF+NF+CF,
  (0x42<<8)               +VF+NF+CF,
  (0x43<<8)                  +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x50<<8)               +VF+NF+CF,
  (0x51<<8)                  +NF+CF,
  (0x52<<8)                  +NF+CF,
  (0x53<<8)               +VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x60<<8)      +YF      +VF+NF+CF,
  (0x61<<8)      +YF         +NF+CF,
  (0x62<<8)      +YF         +NF+CF,
  (0x63<<8)      +YF      +VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x70<<8)      +YF         +NF+CF,
  (0x71<<8)      +YF      +VF+NF+CF,
  (0x72<<8)      +YF      +VF+NF+CF,
  (0x73<<8)      +YF         +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x80<<8)+SF               +NF+CF,
  (0x81<<8)+SF            +VF+NF+CF,
  (0x82<<8)+SF            +VF+NF+CF,
  (0x83<<8)+SF               +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x90<<8)+SF            +VF+NF+CF,
  (0x91<<8)+SF               +NF+CF,
  (0x92<<8)+SF               +NF+CF,
  (0x93<<8)+SF            +VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF,
  (0xFA<<8)+SF   +YF+HF+XF+VF+NF   ,
  (0xFB<<8)+SF   +YF+HF+XF   +NF   ,
  (0xFC<<8)+SF   +YF+HF+XF+VF+NF   ,
  (0xFD<<8)+SF   +YF+HF+XF   +NF   ,
  (0xFE<<8)+SF   +YF+HF+XF   +NF   ,
  (0xFF<<8)+SF   +YF+HF+XF+VF+NF   ,
  (0x00<<8)   +ZF         +VF+NF   ,
  (0x01<<8)                  +NF   ,
  (0x02<<8)                  +NF   ,
  (0x03<<8)               +VF+NF   ,
  (0x04<<8)                  +NF   ,
  (0x05<<8)               +VF+NF   ,
  (0x06<<8)               +VF+NF   ,
  (0x07<<8)                  +NF   ,
  (0x08<<8)            +XF   +NF   ,
  (0x09<<8)            +XF+VF+NF   ,
  (0x0A<<8)         +HF+XF+VF+NF   ,
  (0x0B<<8)         +HF+XF   +NF   ,
  (0x0C<<8)         +HF+XF+VF+NF   ,
  (0x0D<<8)         +HF+XF   +NF   ,
  (0x0E<<8)         +HF+XF   +NF   ,
  (0x0F<<8)         +HF+XF+VF+NF   ,
  (0x10<<8)                  +NF   ,
  (0x11<<8)               +VF+NF   ,
  (0x12<<8)               +VF+NF   ,
  (0x13<<8)                  +NF   ,
  (0x14<<8)               +VF+NF   ,
  (0x15<<8)                  +NF   ,
  (0x16<<8)                  +NF   ,
  (0x17<<8)               +VF+NF   ,
  (0x18<<8)            +XF+VF+NF   ,
  (0x19<<8)            +XF   +NF   ,
  (0x1A<<8)         +HF+XF   +NF   ,
  (0x1B<<8)         +HF+XF+VF+NF   ,
  (0x1C<<8)         +HF+XF   +NF   ,
  (0x1D<<8)         +HF+XF+VF+NF   ,
  (0x1E<<8)         +HF+XF+VF+NF   ,
  (0x1F<<8)         +HF+XF   +NF   ,
  (0x20<<8)      +YF         +NF   ,
  (0x21<<8)      +YF      +VF+NF   ,
  (0x22<<8)      +YF      +VF+NF   ,
  (0x23<<8)      +YF         +NF   ,
  (0x24<<8)      +YF      +VF+NF   ,
  (0x25<<8)      +YF         +NF   ,
  (0x26<<8)      +YF         +NF   ,
  (0x27<<8)      +YF      +VF+NF   ,
  (0x28<<8)      +YF   +XF+VF+NF   ,
  (0x29<<8)      +YF   +XF   +NF   ,
  (0x2A<<8)      +YF+HF+XF   +NF   ,
  (0x2B<<8)      +YF+HF+XF+VF+NF   ,
  (0x2C<<8)      +YF+HF+XF   +NF   ,
  (0x2D<<8)      +YF+HF+XF+VF+NF   ,
  (0x2E<<8)      +YF+HF+XF+VF+NF   ,
  (0x2F<<8)      +YF+HF+XF   +NF   ,
  (0x30<<8)      +YF      +VF+NF   ,
  (0x31<<8)      +YF         +NF   ,
  (0x32<<8)      +YF         +NF   ,
  (0x33<<8)      +YF      +VF+NF   ,
  (0x34<<8)      +YF         +NF   ,
  (0x35<<8)      +YF      +VF+NF   ,
  (0x36<<8)      +YF      +VF+NF   ,
  (0x37<<8)      +YF         +NF   ,
  (0x38<<8)      +YF   +XF   +NF   ,
  (0x39<<8)      +YF   +XF+VF+NF   ,
  (0x3A<<8)      +YF+HF+XF+VF+NF   ,
  (0x3B<<8)      +YF+HF+XF   +NF   ,
  (0x3C<<8)      +YF+HF+XF+VF+NF   ,
  (0x3D<<8)      +YF+HF+XF   +NF   ,
  (0x3E<<8)      +YF+HF+XF   +NF   ,
  (0x3F<<8)      +YF+HF+XF+VF+NF   ,
  (0x40<<8)                  +NF   ,
  (0x41<<8)               +VF+NF   ,
  (0x42<<8)               +VF+NF   ,
  (0x43<<8)                  +NF   ,
  (0x44<<8)               +VF+NF   ,
  (0x45<<8)                  +NF   ,
  (0x46<<8)                  +NF   ,
  (0x47<<8)               +VF+NF   ,
  (0x48<<8)            +XF+VF+NF   ,
  (0x49<<8)            +XF   +NF   ,
  (0x4A<<8)         +HF+XF   +NF   ,
  (0x4B<<8)         +HF+XF+VF+NF   ,
  (0x4C<<8)         +HF+XF   +NF   ,
  (0x4D<<8)         +HF+XF+VF+NF   ,
  (0x4E<<8)         +HF+XF+VF+NF   ,
  (0x4F<<8)         +HF+XF   +NF   ,
  (0x50<<8)               +VF+NF   ,
  (0x51<<8)                  +NF   ,
  (0x52<<8)                  +NF   ,
  (0x53<<8)               +VF+NF   ,
  (0x54<<8)                  +NF   ,
  (0x55<<8)               +VF+NF   ,
  (0x56<<8)               +VF+NF   ,
  (0x57<<8)                  +NF   ,
  (0x58<<8)            +XF   +NF   ,
  (0x59<<8)            +XF+VF+NF   ,
  (0x5A<<8)         +HF+XF+VF+NF   ,
  (0x5B<<8)         +HF+XF   +NF   ,
  (0x5C<<8)         +HF+XF+VF+NF   ,
  (0x5D<<8)         +HF+XF   +NF   ,
  (0x5E<<8)         +HF+XF   +NF   ,
  (0x5F<<8)         +HF+XF+VF+NF   ,
  (0x60<<8)      +YF      +VF+NF   ,
  (0x61<<8)      +YF         +NF   ,
  (0x62<<8)      +YF         +NF   ,
  (0x63<<8)      +YF      +VF+NF   ,
  (0x64<<8)      +YF         +NF   ,
  (0x65<<8)      +YF      +VF+NF   ,
  (0x66<<8)      +YF      +VF+NF   ,
  (0x67<<8)      +YF         +NF   ,
  (0x68<<8)      +YF   +XF   +NF   ,
  (0x69<<8)      +YF   +XF+VF+NF   ,
  (0x6A<<8)      +YF+HF+XF+VF+NF   ,
  (0x6B<<8)      +YF+HF+XF   +NF   ,
  (0x6C<<8)      +YF+HF+XF+VF+NF   ,
  (0x6D<<8)      +YF+HF+XF   +NF   ,
  (0x6E<<8)      +YF+HF+XF   +NF   ,
  (0x6F<<8)      +YF+HF+XF+VF+NF   ,
  (0x70<<8)      +YF         +NF   ,
  (0x71<<8)      +YF      +VF+NF   ,
  (0x72<<8)      +YF      +VF+NF   ,
  (0x73<<8)      +YF         +NF   ,
  (0x74<<8)      +YF      +VF+NF   ,
  (0x75<<8)      +YF         +NF   ,
  (0x76<<8)      +YF         +NF   ,
  (0x77<<8)      +YF      +VF+NF   ,
  (0x78<<8)      +YF   +XF+VF+NF   ,
  (0x79<<8)      +YF   +XF   +NF   ,
  (0x7A<<8)      +YF+HF+XF   +NF   ,
  (0x7B<<8)      +YF+HF+XF+VF+NF   ,
  (0x7C<<8)      +YF+HF+XF   +NF   ,
  (0x7D<<8)      +YF+HF+XF+VF+NF   ,
  (0x7E<<8)      +YF+HF+XF+VF+NF   ,
  (0x7F<<8)      +YF+HF+XF   +NF   ,
  (0x80<<8)+SF               +NF   ,
  (0x81<<8)+SF            +VF+NF   ,
  (0x82<<8)+SF            +VF+NF   ,
  (0x83<<8)+SF               +NF   ,
  (0x84<<8)+SF            +VF+NF   ,
  (0x85<<8)+SF               +NF   ,
  (0x86<<8)+SF               +NF   ,
  (0x87<<8)+SF            +VF+NF   ,
  (0x88<<8)+SF         +XF+VF+NF   ,
  (0x89<<8)+SF         +XF   +NF   ,
  (0x8A<<8)+SF      +HF+XF   +NF   ,
  (0x8B<<8)+SF      +HF+XF+VF+NF   ,
  (0x8C<<8)+SF      +HF+XF   +NF   ,
  (0x8D<<8)+SF      +HF+XF+VF+NF   ,
  (0x8E<<8)+SF      +HF+XF+VF+NF   ,
  (0x8F<<8)+SF      +HF+XF   +NF   ,
  (0x90<<8)+SF            +VF+NF   ,
  (0x91<<8)+SF               +NF   ,
  (0x92<<8)+SF               +NF   ,
  (0x93<<8)+SF            +VF+NF   ,
  (0x34<<8)      +YF         +NF+CF,
  (0x35<<8)      +YF      +VF+NF+CF,
  (0x36<<8)      +YF      +VF+NF+CF,
  (0x37<<8)      +YF         +NF+CF,
  (0x38<<8)      +YF   +XF   +NF+CF,
  (0x39<<8)      +YF   +XF+VF+NF+CF,
  (0x3A<<8)      +YF+HF+XF+VF+NF+CF,
  (0x3B<<8)      +YF+HF+XF   +NF+CF,
  (0x3C<<8)      +YF+HF+XF+VF+NF+CF,
  (0x3D<<8)      +YF+HF+XF   +NF+CF,
  (0x3E<<8)      +YF+HF+XF   +NF+CF,
  (0x3F<<8)      +YF+HF+XF+VF+NF+CF,
  (0x40<<8)                  +NF+CF,
  (0x41<<8)               +VF+NF+CF,
  (0x42<<8)               +VF+NF+CF,
  (0x43<<8)                  +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x4A<<8)         +HF+XF   +NF+CF,
  (0x4B<<8)         +HF+XF+VF+NF+CF,
  (0x4C<<8)         +HF+XF   +NF+CF,
  (0x4D<<8)         +HF+XF+VF+NF+CF,
  (0x4E<<8)         +HF+XF+VF+NF+CF,
  (0x4F<<8)         +HF+XF   +NF+CF,
  (0x50<<8)               +VF+NF+CF,
  (0x51<<8)                  +NF+CF,
  (0x52<<8)                  +NF+CF,
  (0x53<<8)               +VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x5A<<8)         +HF+XF+VF+NF+CF,
  (0x5B<<8)         +HF+XF   +NF+CF,
  (0x5C<<8)         +HF+XF+VF+NF+CF,
  (0x5D<<8)         +HF+XF   +NF+CF,
  (0x5E<<8)         +HF+XF   +NF+CF,
  (0x5F<<8)         +HF+XF+VF+NF+CF,
  (0x60<<8)      +YF      +VF+NF+CF,
  (0x61<<8)      +YF         +NF+CF,
  (0x62<<8)      +YF         +NF+CF,
  (0x63<<8)      +YF      +VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x6A<<8)      +YF+HF+XF+VF+NF+CF,
  (0x6B<<8)      +YF+HF+XF   +NF+CF,
  (0x6C<<8)      +YF+HF+XF+VF+NF+CF,
  (0x6D<<8)      +YF+HF+XF   +NF+CF,
  (0x6E<<8)      +YF+HF+XF   +NF+CF,
  (0x6F<<8)      +YF+HF+XF+VF+NF+CF,
  (0x70<<8)      +YF         +NF+CF,
  (0x71<<8)      +YF      +VF+NF+CF,
  (0x72<<8)      +YF      +VF+NF+CF,
  (0x73<<8)      +YF         +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x7A<<8)      +YF+HF+XF   +NF+CF,
  (0x7B<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7C<<8)      +YF+HF+XF   +NF+CF,
  (0x7D<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7E<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7F<<8)      +YF+HF+XF   +NF+CF,
  (0x80<<8)+SF               +NF+CF,
  (0x81<<8)+SF            +VF+NF+CF,
  (0x82<<8)+SF            +VF+NF+CF,
  (0x83<<8)+SF               +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x8A<<8)+SF      +HF+XF   +NF+CF,
  (0x8B<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8C<<8)+SF      +HF+XF   +NF+CF,
  (0x8D<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8E<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8F<<8)+SF      +HF+XF   +NF+CF,
  (0x90<<8)+SF            +VF+NF+CF,
  (0x91<<8)+SF               +NF+CF,
  (0x92<<8)+SF               +NF+CF,
  (0x93<<8)+SF            +VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF,
  (0x9A<<8)+SF      +HF+XF+VF+NF+CF,
  (0x9B<<8)+SF      +HF+XF   +NF+CF,
  (0x9C<<8)+SF      +HF+XF+VF+NF+CF,
  (0x9D<<8)+SF      +HF+XF   +NF+CF,
  (0x9E<<8)+SF      +HF+XF   +NF+CF,
  (0x9F<<8)+SF      +HF+XF+VF+NF+CF,
  (0xA0<<8)+SF   +YF      +VF+NF+CF,
  (0xA1<<8)+SF   +YF         +NF+CF,
  (0xA2<<8)+SF   +YF         +NF+CF,
  (0xA3<<8)+SF   +YF      +VF+NF+CF,
  (0xA4<<8)+SF   +YF         +NF+CF,
  (0xA5<<8)+SF   +YF      +VF+NF+CF,
  (0xA6<<8)+SF   +YF      +VF+NF+CF,
  (0xA7<<8)+SF   +YF         +NF+CF,
  (0xA8<<8)+SF   +YF   +XF   +NF+CF,
  (0xA9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xAA<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xAB<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xAC<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xAD<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xAE<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xAF<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xB0<<8)+SF   +YF         +NF+CF,
  (0xB1<<8)+SF   +YF      +VF+NF+CF,
  (0xB2<<8)+SF   +YF      +VF+NF+CF,
  (0xB3<<8)+SF   +YF         +NF+CF,
  (0xB4<<8)+SF   +YF      +VF+NF+CF,
  (0xB5<<8)+SF   +YF         +NF+CF,
  (0xB6<<8)+SF   +YF         +NF+CF,
  (0xB7<<8)+SF   +YF      +VF+NF+CF,
  (0xB8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xB9<<8)+SF   +YF   +XF   +NF+CF,
  (0xBA<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xBB<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xBC<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xBD<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xBE<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xBF<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xC0<<8)+SF            +VF+NF+CF,
  (0xC1<<8)+SF               +NF+CF,
  (0xC2<<8)+SF               +NF+CF,
  (0xC3<<8)+SF            +VF+NF+CF,
  (0xC4<<8)+SF               +NF+CF,
  (0xC5<<8)+SF            +VF+NF+CF,
  (0xC6<<8)+SF            +VF+NF+CF,
  (0xC7<<8)+SF               +NF+CF,
  (0xC8<<8)+SF         +XF   +NF+CF,
  (0xC9<<8)+SF         +XF+VF+NF+CF,
  (0xCA<<8)+SF      +HF+XF+VF+NF+CF,
  (0xCB<<8)+SF      +HF+XF   +NF+CF,
  (0xCC<<8)+SF      +HF+XF+VF+NF+CF,
  (0xCD<<8)+SF      +HF+XF   +NF+CF,
  (0xCE<<8)+SF      +HF+XF   +NF+CF,
  (0xCF<<8)+SF      +HF+XF+VF+NF+CF,
  (0xD0<<8)+SF               +NF+CF,
  (0xD1<<8)+SF            +VF+NF+CF,
  (0xD2<<8)+SF            +VF+NF+CF,
  (0xD3<<8)+SF               +NF+CF,
  (0xD4<<8)+SF            +VF+NF+CF,
  (0xD5<<8)+SF               +NF+CF,
  (0xD6<<8)+SF               +NF+CF,
  (0xD7<<8)+SF            +VF+NF+CF,
  (0xD8<<8)+SF         +XF+VF+NF+CF,
  (0xD9<<8)+SF         +XF   +NF+CF,
  (0xDA<<8)+SF      +HF+XF   +NF+CF,
  (0xDB<<8)+SF      +HF+XF+VF+NF+CF,
  (0xDC<<8)+SF      +HF+XF   +NF+CF,
  (0xDD<<8)+SF      +HF+XF+VF+NF+CF,
  (0xDE<<8)+SF      +HF+XF+VF+NF+CF,
  (0xDF<<8)+SF      +HF+XF   +NF+CF,
  (0xE0<<8)+SF   +YF         +NF+CF,
  (0xE1<<8)+SF   +YF      +VF+NF+CF,
  (0xE2<<8)+SF   +YF      +VF+NF+CF,
  (0xE3<<8)+SF   +YF         +NF+CF,
  (0xE4<<8)+SF   +YF      +VF+NF+CF,
  (0xE5<<8)+SF   +YF         +NF+CF,
  (0xE6<<8)+SF   +YF         +NF+CF,
  (0xE7<<8)+SF   +YF      +VF+NF+CF,
  (0xE8<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xE9<<8)+SF   +YF   +XF   +NF+CF,
  (0xEA<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xEB<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xEC<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xED<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xEE<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xEF<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xF0<<8)+SF   +YF      +VF+NF+CF,
  (0xF1<<8)+SF   +YF         +NF+CF,
  (0xF2<<8)+SF   +YF         +NF+CF,
  (0xF3<<8)+SF   +YF      +VF+NF+CF,
  (0xF4<<8)+SF   +YF         +NF+CF,
  (0xF5<<8)+SF   +YF      +VF+NF+CF,
  (0xF6<<8)+SF   +YF      +VF+NF+CF,
  (0xF7<<8)+SF   +YF         +NF+CF,
  (0xF8<<8)+SF   +YF   +XF   +NF+CF,
  (0xF9<<8)+SF   +YF   +XF+VF+NF+CF,
  (0xFA<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xFB<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xFC<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0xFD<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xFE<<8)+SF   +YF+HF+XF   +NF+CF,
  (0xFF<<8)+SF   +YF+HF+XF+VF+NF+CF,
  (0x00<<8)   +ZF         +VF+NF+CF,
  (0x01<<8)                  +NF+CF,
  (0x02<<8)                  +NF+CF,
  (0x03<<8)               +VF+NF+CF,
  (0x04<<8)                  +NF+CF,
  (0x05<<8)               +VF+NF+CF,
  (0x06<<8)               +VF+NF+CF,
  (0x07<<8)                  +NF+CF,
  (0x08<<8)            +XF   +NF+CF,
  (0x09<<8)            +XF+VF+NF+CF,
  (0x0A<<8)         +HF+XF+VF+NF+CF,
  (0x0B<<8)         +HF+XF   +NF+CF,
  (0x0C<<8)         +HF+XF+VF+NF+CF,
  (0x0D<<8)         +HF+XF   +NF+CF,
  (0x0E<<8)         +HF+XF   +NF+CF,
  (0x0F<<8)         +HF+XF+VF+NF+CF,
  (0x10<<8)                  +NF+CF,
  (0x11<<8)               +VF+NF+CF,
  (0x12<<8)               +VF+NF+CF,
  (0x13<<8)                  +NF+CF,
  (0x14<<8)               +VF+NF+CF,
  (0x15<<8)                  +NF+CF,
  (0x16<<8)                  +NF+CF,
  (0x17<<8)               +VF+NF+CF,
  (0x18<<8)            +XF+VF+NF+CF,
  (0x19<<8)            +XF   +NF+CF,
  (0x1A<<8)         +HF+XF   +NF+CF,
  (0x1B<<8)         +HF+XF+VF+NF+CF,
  (0x1C<<8)         +HF+XF   +NF+CF,
  (0x1D<<8)         +HF+XF+VF+NF+CF,
  (0x1E<<8)         +HF+XF+VF+NF+CF,
  (0x1F<<8)         +HF+XF   +NF+CF,
  (0x20<<8)      +YF         +NF+CF,
  (0x21<<8)      +YF      +VF+NF+CF,
  (0x22<<8)      +YF      +VF+NF+CF,
  (0x23<<8)      +YF         +NF+CF,
  (0x24<<8)      +YF      +VF+NF+CF,
  (0x25<<8)      +YF         +NF+CF,
  (0x26<<8)      +YF         +NF+CF,
  (0x27<<8)      +YF      +VF+NF+CF,
  (0x28<<8)      +YF   +XF+VF+NF+CF,
  (0x29<<8)      +YF   +XF   +NF+CF,
  (0x2A<<8)      +YF+HF+XF   +NF+CF,
  (0x2B<<8)      +YF+HF+XF+VF+NF+CF,
  (0x2C<<8)      +YF+HF+XF   +NF+CF,
  (0x2D<<8)      +YF+HF+XF+VF+NF+CF,
  (0x2E<<8)      +YF+HF+XF+VF+NF+CF,
  (0x2F<<8)      +YF+HF+XF   +NF+CF,
  (0x30<<8)      +YF      +VF+NF+CF,
  (0x31<<8)      +YF         +NF+CF,
  (0x32<<8)      +YF         +NF+CF,
  (0x33<<8)      +YF      +VF+NF+CF,
  (0x34<<8)      +YF         +NF+CF,
  (0x35<<8)      +YF      +VF+NF+CF,
  (0x36<<8)      +YF      +VF+NF+CF,
  (0x37<<8)      +YF         +NF+CF,
  (0x38<<8)      +YF   +XF   +NF+CF,
  (0x39<<8)      +YF   +XF+VF+NF+CF,
  (0x3A<<8)      +YF+HF+XF+VF+NF+CF,
  (0x3B<<8)      +YF+HF+XF   +NF+CF,
  (0x3C<<8)      +YF+HF+XF+VF+NF+CF,
  (0x3D<<8)      +YF+HF+XF   +NF+CF,
  (0x3E<<8)      +YF+HF+XF   +NF+CF,
  (0x3F<<8)      +YF+HF+XF+VF+NF+CF,
  (0x40<<8)                  +NF+CF,
  (0x41<<8)               +VF+NF+CF,
  (0x42<<8)               +VF+NF+CF,
  (0x43<<8)                  +NF+CF,
  (0x44<<8)               +VF+NF+CF,
  (0x45<<8)                  +NF+CF,
  (0x46<<8)                  +NF+CF,
  (0x47<<8)               +VF+NF+CF,
  (0x48<<8)            +XF+VF+NF+CF,
  (0x49<<8)            +XF   +NF+CF,
  (0x4A<<8)         +HF+XF   +NF+CF,
  (0x4B<<8)         +HF+XF+VF+NF+CF,
  (0x4C<<8)         +HF+XF   +NF+CF,
  (0x4D<<8)         +HF+XF+VF+NF+CF,
  (0x4E<<8)         +HF+XF+VF+NF+CF,
  (0x4F<<8)         +HF+XF   +NF+CF,
  (0x50<<8)               +VF+NF+CF,
  (0x51<<8)                  +NF+CF,
  (0x52<<8)                  +NF+CF,
  (0x53<<8)               +VF+NF+CF,
  (0x54<<8)                  +NF+CF,
  (0x55<<8)               +VF+NF+CF,
  (0x56<<8)               +VF+NF+CF,
  (0x57<<8)                  +NF+CF,
  (0x58<<8)            +XF   +NF+CF,
  (0x59<<8)            +XF+VF+NF+CF,
  (0x5A<<8)         +HF+XF+VF+NF+CF,
  (0x5B<<8)         +HF+XF   +NF+CF,
  (0x5C<<8)         +HF+XF+VF+NF+CF,
  (0x5D<<8)         +HF+XF   +NF+CF,
  (0x5E<<8)         +HF+XF   +NF+CF,
  (0x5F<<8)         +HF+XF+VF+NF+CF,
  (0x60<<8)      +YF      +VF+NF+CF,
  (0x61<<8)      +YF         +NF+CF,
  (0x62<<8)      +YF         +NF+CF,
  (0x63<<8)      +YF      +VF+NF+CF,
  (0x64<<8)      +YF         +NF+CF,
  (0x65<<8)      +YF      +VF+NF+CF,
  (0x66<<8)      +YF      +VF+NF+CF,
  (0x67<<8)      +YF         +NF+CF,
  (0x68<<8)      +YF   +XF   +NF+CF,
  (0x69<<8)      +YF   +XF+VF+NF+CF,
  (0x6A<<8)      +YF+HF+XF+VF+NF+CF,
  (0x6B<<8)      +YF+HF+XF   +NF+CF,
  (0x6C<<8)      +YF+HF+XF+VF+NF+CF,
  (0x6D<<8)      +YF+HF+XF   +NF+CF,
  (0x6E<<8)      +YF+HF+XF   +NF+CF,
  (0x6F<<8)      +YF+HF+XF+VF+NF+CF,
  (0x70<<8)      +YF         +NF+CF,
  (0x71<<8)      +YF      +VF+NF+CF,
  (0x72<<8)      +YF      +VF+NF+CF,
  (0x73<<8)      +YF         +NF+CF,
  (0x74<<8)      +YF      +VF+NF+CF,
  (0x75<<8)      +YF         +NF+CF,
  (0x76<<8)      +YF         +NF+CF,
  (0x77<<8)      +YF      +VF+NF+CF,
  (0x78<<8)      +YF   +XF+VF+NF+CF,
  (0x79<<8)      +YF   +XF   +NF+CF,
  (0x7A<<8)      +YF+HF+XF   +NF+CF,
  (0x7B<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7C<<8)      +YF+HF+XF   +NF+CF,
  (0x7D<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7E<<8)      +YF+HF+XF+VF+NF+CF,
  (0x7F<<8)      +YF+HF+XF   +NF+CF,
  (0x80<<8)+SF               +NF+CF,
  (0x81<<8)+SF            +VF+NF+CF,
  (0x82<<8)+SF            +VF+NF+CF,
  (0x83<<8)+SF               +NF+CF,
  (0x84<<8)+SF            +VF+NF+CF,
  (0x85<<8)+SF               +NF+CF,
  (0x86<<8)+SF               +NF+CF,
  (0x87<<8)+SF            +VF+NF+CF,
  (0x88<<8)+SF         +XF+VF+NF+CF,
  (0x89<<8)+SF         +XF   +NF+CF,
  (0x8A<<8)+SF      +HF+XF   +NF+CF,
  (0x8B<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8C<<8)+SF      +HF+XF   +NF+CF,
  (0x8D<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8E<<8)+SF      +HF+XF+VF+NF+CF,
  (0x8F<<8)+SF      +HF+XF   +NF+CF,
  (0x90<<8)+SF            +VF+NF+CF,
  (0x91<<8)+SF               +NF+CF,
  (0x92<<8)+SF               +NF+CF,
  (0x93<<8)+SF            +VF+NF+CF,
  (0x94<<8)+SF               +NF+CF,
  (0x95<<8)+SF            +VF+NF+CF,
  (0x96<<8)+SF            +VF+NF+CF,
  (0x97<<8)+SF               +NF+CF,
  (0x98<<8)+SF         +XF   +NF+CF,
  (0x99<<8)+SF         +XF+VF+NF+CF
};

        }
    }
}