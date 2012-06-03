#define RAW_VECTORS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class cpu_ccpu : cpu_interface
        {
            public const byte CCPU_PORT_IOSWITCHES = 0,
                              CCPU_PORT_IOINPUTS = 1,
                              CCPU_PORT_IOOUTPUTS = 2,
                              CCPU_PORT_IN_JOYSTICKX = 3,
                              CCPU_PORT_IN_JOYSTICKY = 4,
                              CCPU_PORT_MAX = 5,

                              CCPU_MEMSIZE_4K = 0,
                              CCPU_MEMSIZE_8K = 1,
                              CCPU_MEMSIZE_16K = 2,
                              CCPU_MEMSIZE_32K = 3,

                              CCPU_MONITOR_BILEV = 0,
                              CCPU_MONITOR_16LEV = 1,
                              CCPU_MONITOR_64LEV = 2,
                              CCPU_MONITOR_WOWCOL = 3;




            class _ccpuRegs
            {
                public ushort accVal;
                public ushort cmpVal;
                public byte pa0;
                public byte cFlag;
                public ushort eRegPC;
                public ushort eRegA;
                public ushort eRegB;
                public ushort eRegI;
                public ushort eRegJ;
                public byte eRegP;
                public byte eCState;
            }
            static _ccpuRegs ccpuRegs = new _ccpuRegs();
            static int[] ccpu_ICount = new int[1];
            public cpu_ccpu()
            {
                cpu_num = CPU_CCPU;
                num_irqs = 2;
                default_vector = 0;
                overclock = 1.0;
                no_int = 0;
                irq_int = -1;
                nmi_int = -1;
                address_shift = 0;
                address_bits = 15;
                endianess = CPU_IS_LE;
                align_unit = 1;
                max_inst_len = 3;
                abits1 = ABITS1_16;
                abits2 = ABITS2_16;
                abitsmin = ABITS_MIN_16;
                icount = ccpu_ICount;
                icount[0] = 1000;
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
                    case CPU_INFO_NAME: return "CCPU";
                    case CPU_INFO_FAMILY: return "Cinematronics CPU";
                    case CPU_INFO_VERSION: return "1.0";
                    case CPU_INFO_FILE: return "cpu_ccpu.cs";
                    case CPU_INFO_CREDITS: return "Copyright(int opcode){return 1997/1998 Jeff Mitchell and the Retrocade Alliance\nCopyright 1997 Zonn Moore";
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
                //nothing
            }
            public override int execute(int cycles)
            {
                int newCycles = (int)cineExec((uint)cycles);
                return newCycles;
            }
            public override uint get_context(ref object reg)
            {
                throw new NotImplementedException();
            }
            public override void create_context(ref object reg)
            {
                reg = new _ccpuRegs();
            }
            public override uint get_pc()
            {
                scCpuStruct context;
                context.accVal = cmp_old;
                context.cmpVal = cmp_new;
                context.pa0 = acc_a0;
                context.cFlag = (byte)((flag_C >> 8) & 0xFF);
                context.eRegPC = register_PC;
                context.eRegA = register_A;
                context.eRegB = register_B;
                context.eRegI = register_I;
                context.eRegJ = register_J;
                context.eRegP = register_P;
                context.eCState = state;
                return context.eRegPC;
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
                cineReset();
            }
            public override void set_context(object reg)
            {
                throw new NotImplementedException();
            }
            public override void set_irq_callback(irqcallback callback)
            {
                //nothing
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

            static byte vgShiftLength = 0; /* number of shifts loaded into length reg */
            static uint vgColour = 0;
            static int ccpu_jmi_dip = 0;     /* as set by cineSetJMI */
            static int ccpu_msize = 0;       /* as set by cineSetMSize */
            static int ccpu_monitor = 0;     /* as set by cineSetMonitor */
            static bool bailOut = false;

            /* C-CPU context information begins --  */
            static uint register_PC = 0; /* C-CPU registers; program counter */
            static uint register_A = 0;  /* A-Register (accumulator) */
            static uint register_B = 0;  /* B-Register (accumulator) */
            static byte register_I = 0;  /* I-Register (last access RAM location) */
            static uint register_J = 0;  /* J-Register (target address for JMP opcodes) */
            static byte register_P = 0;  /* Page-Register (4 bits) */
            static uint FromX = 0;       /* X-Register (start of a vector) */
            static uint FromY = 0;       /* Y-Register (start of a vector) */
            static uint register_T = 0;  /* T-Register (vector draw length timer) */
            static uint flag_C = 0;      /* C-CPU flags; carry. Is word sized, instead
                                  * of byte, so we can do direct assignment
                                  * and then change to BYTE during inspection.
                                  */

            static uint cmp_old = 0;     /* last accumulator value */
            static uint cmp_new = 0;     /* new accumulator value */
            static byte acc_a0 = 0;      /* bit0 of A-reg at last accumulator access */
            enum CINESTATE
            {
                state_A = 0,
                state_AA,
                state_B,
                state_BB
            } ;

            struct scCpuStruct
            {
                public uint accVal;				/* CCPU Accumulator value */
                public uint cmpVal;				/* Comparison value */
                public byte pa0;
                public byte cFlag;
                public uint eRegPC;
                public uint eRegA;
                public uint eRegB;
                public uint eRegI;
                public uint eRegJ;
                public byte eRegP;
                public CINESTATE eCState;
            };


            static CINESTATE state = CINESTATE.state_A;/* C-CPU state machine current state */

            static uint[] ram = new uint[256];       /* C-CPU ram (for all pages) */
            static bool bNewFrame;
            static bool bFlipX, bFlipY, bSwapXY, bOverlay;

            public static void ccpu_Config(int jmi, int msize, int monitor)
            {
                cineSetJMI(jmi);
                cineSetMSize(msize);
                cineSetMonitor(monitor);
            }
            static void cineSetJMI(int j)
            {
                ccpu_jmi_dip = j;
                /*
                    if (ccpu_jmi_dip)
                        fprintf (stderr, "CCPU JMI Set: Yes.\n");
                    else
                        fprintf (stderr, "CCPU JMI Set: No.\n");
                */
            }
            static void cineSetMSize(int m)
            {
                ccpu_msize = m;
                /*
                    switch (m)
                    {
                        case 0:
                            fprintf (stderr, "CCPU Address Space: 4k\n");
                            break;
                        case 1:
                            fprintf (stderr, "CCPU Address Space: 8k\n");
                            break;
                        case 2:
                            fprintf (stderr, "CCPU Address Space: 16k\n");
                            break;
                        case 3:
                            fprintf (stderr, "CCPU Address Space: 32k\n");
                            break;
                        default:
                            fprintf (stderr, "CCPU Address Space: Error\n");
                            break;
                    }
                */
            }
            static void cineSetMonitor(int m)
            {
                ccpu_monitor = m;
                /*
                    switch (m)
                    {
                        case 1:
                            fprintf (stderr, "CCPU Monitor: 16-colour\n");
                            break;
                        case 2:
                            fprintf (stderr, "CCPU Monitor: 64-colour\n");
                            break;
                        case 3:
                            fprintf (stderr, "CCPU Monitor: War-of-the-Worlds-colour\n");
                            break;
                        default:
                            fprintf (stderr, "CCPU Monitor: bi-level-display\n");
                            break;
                    }
                */
            }
            static void cineReset()
            {
                /* zero registers */
                register_PC = 0;
                register_A = 0;
                register_B = 0;
                register_I = 0;
                register_J = 0;
                register_P = 0;
                FromX = 0;
                FromY = 0;
                register_T = 0;

                /* zero flags */
                flag_C = 0;

                /* reset state */
                state = CINESTATE.state_A;

                /* reset RAM */
                Array.Clear(ram, 0, ram.Length);//memset(ram, 0, sizeof(ram));

                /* reset internal state */
                cmp_old = 0;
                cmp_new = 0;
                acc_a0 = 0;
            }
            uint cineExec(uint cycles)
            {
                ccpu_ICount[0] = (int)cycles;
                bailOut = false;

                do
                {
                    int opcode;

                    /*
                     * goto the correct piece of code
                     * for the current opcode. That piece of code will set the state
                     * for the next run, as well.
                     */

                    opcode = cpu_readop(register_PC++);
                    state = cineops[(int)state][opcode](opcode);
                    ccpu_ICount[0] -= ccpu_cycles[opcode];


                    /*
                     * the opcode code has set a state and done mischief with flags and
                     * the program counter; now jump back to the top and run through another
                     * opcode.
                     */
                    if (bailOut)
                        /*			ccpu_ICount = 0; */
                        ccpu_ICount[0] -= 100;
                }
                while (ccpu_ICount[0] > 0);

                return (uint)(cycles - ccpu_ICount[0]);
            }
            static byte[] ccpu_cycles =
{
	/*    0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F */
  /*0*/	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* LDA */
  /*1*/	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* INP */
  /*2*/	  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* ADD */
  /*3*/	  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* SUB */
  /*4*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* LDJ */
  /*5*/	  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, /* Jumps: 2 extra cycles if jump made */
  /*6*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* ADD */
  /*7*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* SUB */
  /*8*/	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* LDP */
  /*9*/	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, /* OUT */
  /*A*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* LDA */
  /*B*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* CMP */
  /*C*/	  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, /* LDI */
  /*D*/	  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, /* STA */
  /*E*/	  1,  2,  7,  2,  0,  0,  2,  2,  2,  2,  2,  1,  1,  1,  1,  1, /* LTT and WAI have special timing */
  /*F*/	  1,  2,  7,  2,  0,  0,  2,  2,  2,  2,  2,  1,  1,  1,  1,  1  /* same as E */
};
            delegate CINESTATE opcode_func(int opcode);
            /* the main opcode table */
            static opcode_func[][] cineops =
{
	new opcode_func[]{
		/* table for state "A" -- Use this table if the last opcode was not
		 * an ACC related opcode, and was not a B flip/flop operation.
		 * Translation:
		 *   Any ACC related routine will use A-reg and go on to opCodeTblAA
		 *   Any B flip/flop instructions will jump to opCodeTblB
		 *   All other instructions remain in opCodeTblA
		 *   JMI will use the current sign of the A-reg
		 */
		opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA,
		opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA, opLDAimm_A_AA,
		opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,
		opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,    opINP_A_AA,

		opADDimmX_A_AA,opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA,
		opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA, opADDimm_A_AA,
		opSUBimmX_A_AA,opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA,
		opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA, opSUBimm_A_AA,

		opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,
		opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,  opLDJimm_A_A,
		tJPP_A_B,      tJMI_A_B,      opJDR_A_B,     opJLT_A_B,     opJEQ_A_B,     opJNC_A_B,     opJA0_A_B,     opNOP_A_B,
		opJMP_A_A,     tJMI_A_A,      opJDR_A_A,     opJLT_A_A,     opJEQ_A_A,     opJNC_A_A,     opJA0_A_A,     opNOP_A_A,

		opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA,
		opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA, opADDdir_A_AA,
		opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA,
		opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA, opSUBdir_A_AA,

		opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,
		opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,  opLDPimm_A_A,
		tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,
		tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,      tOUT_A_A,

		opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA,
		opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA, opLDAdir_A_AA,
		opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA,
		opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA, opCMPdir_A_AA,

		opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,
		opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,  opLDIdir_A_A,
		opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,
		opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,  opSTAdir_A_A,

		opVDR_A_A,     opLDJirg_A_A,  opXLT_A_AA,    opMULirg_A_AA, opLLT_A_AA,    opWAI_A_A,     opSTAirg_A_A,  opADDirg_A_AA,
		opSUBirg_A_AA, opANDirg_A_AA, opLDAirg_A_AA, opLSRe_A_AA,   opLSLe_A_AA,   opASRe_A_AA,   opASRDe_A_AA,  opLSLDe_A_AA,
		opVIN_A_A,     opLDJirg_A_A,  opXLT_A_AA,    opMULirg_A_AA, opLLT_A_AA,    opWAI_A_A,     opSTAirg_A_A,  opAWDirg_A_AA,
		opSUBirg_A_AA, opANDirg_A_AA, opLDAirg_A_AA, opLSRf_A_AA,   opLSLf_A_AA,   opASRf_A_AA,   opASRDf_A_AA,  opLSLDf_A_AA
	},

	new opcode_func[]{
		/* opcode table AA -- Use this table if the last opcode was an ACC
		 * related opcode. Translation:
		 *   Any ACC related routine will use A-reg and remain in OpCodeTblAA
		 *   Any B flip/flop instructions will jump to opCodeTblB
		 *   All other instructions will jump to opCodeTblA
		 *   JMI will use the sign of acc_old
		 */

		opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA,
		opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA, opLDAimm_AA_AA,
		opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,
		opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,    opINP_AA_AA,

		opADDimmX_AA_AA,opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA,
		opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA, opADDimm_AA_AA,
		opSUBimmX_AA_AA,opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA,
		opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA, opSUBimm_AA_AA,

		opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,
		opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,  opLDJimm_AA_A,
		tJPP_AA_B,      tJMI_AA_B,      opJDR_AA_B,     opJLT_AA_B,     opJEQ_AA_B,     opJNC_AA_B,     opJA0_AA_B,     opNOP_AA_B,
		opJMP_AA_A,     tJMI_AA_A,      opJDR_AA_A,     opJLT_AA_A,     opJEQ_AA_A,     opJNC_AA_A,     opJA0_AA_A,     opNOP_AA_A,

		opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA,
		opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA, opADDdir_AA_AA,
		opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA,
		opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA, opSUBdir_AA_AA,

		opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,
		opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,  opLDPimm_AA_A,
		tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,
		tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,      tOUT_AA_A,

		opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA,
		opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA, opLDAdir_AA_AA,
		opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA,
		opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA, opCMPdir_AA_AA,

		opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,
		opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,  opLDIdir_AA_A,
		opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,
		opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,  opSTAdir_AA_A,

		opVDR_AA_A,     opLDJirg_AA_A,  opXLT_AA_AA,    opMULirg_AA_AA, opLLT_AA_AA,    opWAI_AA_A,     opSTAirg_AA_A,  opADDirg_AA_AA,
		opSUBirg_AA_AA, opANDirg_AA_AA, opLDAirg_AA_AA, opLSRe_AA_AA,   opLSLe_AA_AA,   opASRe_AA_AA,   opASRDe_AA_AA,  opLSLDe_AA_AA,
		opVIN_AA_A,     opLDJirg_AA_A,  opXLT_AA_AA,    opMULirg_AA_AA, opLLT_AA_AA,    opWAI_AA_A,     opSTAirg_AA_A,  opAWDirg_AA_AA,
		opSUBirg_AA_AA, opANDirg_AA_AA, opLDAirg_AA_AA, opLSRf_AA_AA,   opLSLf_AA_AA,   opASRf_AA_AA,   opASRDf_AA_AA,  opLSLDf_AA_AA
	},

	new opcode_func[]{
		/* opcode table B -- use this table if the last opcode was a B-reg flip/flop
		 * Translation:
		 *   Any ACC related routine uses B-reg, and goes to opCodeTblAA
		 *   All other instructions will jump to table opCodeTblBB (including
		 *     B flip/flop related instructions)
		 *   JMI will use current sign of the A-reg
		 */
		opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA,
		opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA, opLDAimm_B_AA,
		opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,
		opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,    opINP_B_AA,

		opADDimmX_B_AA,opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA,
		opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA, opADDimm_B_AA,
		opSUBimmX_B_AA,opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA,
		opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA, opSUBimm_B_AA,

		opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB,
		opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB, opLDJimm_B_BB,
		tJPP_B_BB,     tJMI_B_BB1,    opJDR_B_BB,    opJLT_B_BB,    opJEQ_B_BB,    opJNC_B_BB,    opJA0_B_BB,    opNOP_B_BB,
		opJMP_B_BB,    tJMI_B_BB2,    opJDR_B_BB,    opJLT_B_BB,    opJEQ_B_BB,    opJNC_B_BB,    opJA0_B_BB,    opNOP_B_BB,

		opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA,
		opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA, opADDdir_B_AA,
		opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA,
		opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA, opSUBdir_B_AA,

		opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB,
		opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB, opLDPimm_B_BB,
		tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,
		tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,     tOUT_B_BB,

		opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA,
		opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA, opLDAdir_B_AA,
		opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA,
		opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA, opCMPdir_B_AA,

		opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB,
		opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB, opLDIdir_B_BB,
		opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB,
		opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB, opSTAdir_B_BB,

		opVDR_B_BB,    opLDJirg_B_BB, opXLT_B_AA,    opMULirg_B_AA, opLLT_B_AA,    opWAI_B_BB,    opSTAirg_B_BB, opADDirg_B_AA,
		opSUBirg_B_AA, opANDirg_B_AA, opLDAirg_B_AA, opLSRe_B_AA,   opLSLe_B_AA,   opASRe_B_AA,   opASRDe_B_AA,  opLSLDe_B_AA,
		opVIN_B_BB,    opLDJirg_B_BB, opXLT_B_AA,    opMULirg_B_AA, opLLT_B_AA,    opWAI_B_BB,    opSTAirg_B_BB, opAWDirg_B_AA,
		opSUBirg_B_AA, opANDirg_B_AA, opLDAirg_B_AA, opLSRf_B_AA,   opLSLf_B_AA,   opASRf_B_AA,   opASRDf_B_AA,  opLSLDf_B_AA,
	},

	new opcode_func[]{
		/* opcode table BB -- use this table if the last opcode was not an ACC
		 * related opcode, but instruction before that was a B-flip/flop instruction.
		 * Translation:
		 *   Any ACC related routine will use A-reg and go to opCodeTblAA
		 *   Any B flip/flop instructions will jump to opCodeTblB
		 *   All other instructions will jump to table opCodeTblA
		 *   JMI will use the current state of the B-reg
		 */
		opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA,
		opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA, opLDAimm_BB_AA,
		opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,
		opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,    opINP_BB_AA,

		opADDimmX_BB_AA,opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA,
		opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA, opADDimm_BB_AA,
		opSUBimmX_BB_AA,opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA,
		opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA, opSUBimm_BB_AA,

		opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,
		opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,  opLDJimm_BB_A,
		tJPP_BB_B,      tJMI_BB_B,      opJDR_BB_B,     opJLT_BB_B,     opJEQ_BB_B,     opJNC_BB_B,     opJA0_BB_B,     opNOP_BB_B,
		opJMP_BB_A,     tJMI_BB_A,      opJDR_BB_A,     opJLT_BB_A,     opJEQ_BB_A,     opJNC_BB_A,     opJA0_BB_A,     opNOP_BB_A,

		opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA,
		opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA, opADDdir_BB_AA,
		opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA,
		opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA, opSUBdir_BB_AA,

		opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,
		opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,  opLDPimm_BB_A,
		tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,
		tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,      tOUT_BB_A,

		opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA,
		opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA, opLDAdir_BB_AA,
		opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA,
		opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA, opCMPdir_BB_AA,

		opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,
		opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,  opLDIdir_BB_A,
		opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,
		opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,  opSTAdir_BB_A,

		opVDR_BB_A,     opLDJirg_BB_A,  opXLT_BB_AA,    opMULirg_BB_AA, opLLT_BB_AA,    opWAI_BB_A,     opSTAirg_BB_A,  opADDirg_BB_AA,
		opSUBirg_BB_AA, opANDirg_BB_AA, opLDAirg_BB_AA, opLSRe_BB_AA,   opLSLe_BB_AA,   opASRe_BB_AA,   opASRDe_BB_AA,  opLSLDe_BB_AA,
		opVIN_BB_A,     opLDJirg_BB_A,  opXLT_BB_AA,    opMULirg_BB_AA, opLLT_BB_AA,    opWAI_BB_A,     opSTAirg_BB_A,  opAWDirg_BB_AA,
		opSUBirg_BB_AA, opANDirg_BB_AA, opLDAirg_BB_AA, opLSRf_BB_AA,   opLSLf_BB_AA,   opASRf_BB_AA,   opASRDf_BB_AA,  opLSLDf_BB_AA,
	}
};
            static void JMP()
            {
                register_PC = ((register_PC - 1) & 0xF000) + register_J; ccpu_ICount[0] -= 2;
            }
            static void SETA0(uint var) { acc_a0 = (byte)var; }
            static void SETFC(uint val) { flag_C = val; }
            static uint CCPU_READPORT(int a) { return (uint)cpu_readport(a); }
            static CINESTATE opINP_A_AA(int opcode)
            {
                /*
                 * bottom 4 bits of opcode are the position of the bit we want;
                 * obtain input value, shift over that no, and truncate to last bit.
                 * NOTE: Masking 0x07 does interesting things on Sundance and
                 * others, but masking 0x0F makes RipOff and others actually work :)
                 */

                cmp_new = (CCPU_READPORT(CCPU_PORT_IOINPUTS) >> (opcode & 0x0F)) & 0x01;

                SETA0(register_A);               /* save old accA bit0 */
                SETFC(register_A);

                cmp_old = register_A;               /* save old accB */
                register_A = cmp_new;               /* load new accB; zero other bits */

                return CINESTATE.state_AA;
            }
            static CINESTATE opINP_B_AA(int opcode)
            {
                /*
                 * bottom 3 bits of opcode are the position of the bit we want;
                 * obtain Switches value, shift over that no, and truncate to last bit.
                 */

                cmp_new = (CCPU_READPORT(CCPU_PORT_IOSWITCHES) >> (opcode & 0x07)) & 0x01;

                SETA0(register_A);               /* save old accA bit0 */
                SETFC(register_A);

                cmp_old = register_B;               /* save old accB */
                register_B = cmp_new;               /* load new accB; zero other bits */

                return CINESTATE.state_AA;
            }
            static CINESTATE opOUTsnd_A(int opcode)
            {
                if ((register_A & 0x01)==0)
                    cpu_writeport(CCPU_PORT_IOOUTPUTS, (int)(CCPU_READPORT(CCPU_PORT_IOOUTPUTS) | (0x01 << (opcode & 0x07))));
                else
                    cpu_writeport(CCPU_PORT_IOOUTPUTS, (int)(CCPU_READPORT(CCPU_PORT_IOOUTPUTS) & ~(0x01 << (opcode & 0x07))));

                if ((opcode & 0x07) == 0x05)
                {
                    /* reset coin counter */
                }

                return CINESTATE.state_A;
            }
            static CINESTATE opOUTbi_A_A(int opcode)
            {
                if ((opcode & 0x07) != 6)
                    return opOUTsnd_A(opcode);

                vgColour = (register_A & 0x01)!=0 ?(uint) 0x0f :(uint) 0x07;

                return CINESTATE.state_A;
            }
            static CINESTATE opOUT16_A_A(int opcode)
            {
                if ((opcode & 0x07) != 6)
                    return opOUTsnd_A(opcode);

                if ((register_A & 0x1) != 1)
                {
                    vgColour = FromX & 0x0F;

                    if (vgColour == 0)
                        vgColour = 1;
                }

                return CINESTATE.state_A;
            }
            static CINESTATE opOUT64_A_A(int opcode)
            {
                return CINESTATE.state_A;
            }
            static CINESTATE opOUTWW_A_A(int opcode)
            {
                if ((opcode & 0x07) != 6)
                    return opOUTsnd_A(opcode);

                if ((register_A & 0x1) == 1)
                {
                    uint temp_word = ~FromX & 0x0FFF;
                    if (temp_word == 0)   /* black */
                        vgColour = 0;
                    else
                    {   /* non-black */
                        if ((temp_word & 0x0888) != 0)
                            /* bright */
                            vgColour = ((temp_word >> 1) & 0x04) | ((temp_word >> 6) & 0x02) | ((temp_word >> 11) & 0x01) | 0x08;
                        else if ((temp_word & 0x0444) != 0)
                            /* dim bits */
                            vgColour = (temp_word & 0x04) | ((temp_word >> 5) & 0x02) | ((temp_word >> 10) & 0x01);
                    }
                } /* colour change? == 1 */

                return CINESTATE.state_A;
            }
            static CINESTATE opOUTsnd_B(int opcode)
            {
                return CINESTATE.state_BB;
            }
            static CINESTATE opOUTbi_B_BB(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x07);

                if ((temp_byte - 0x06) != 0)
                    return opOUTsnd_B(opcode);

                vgColour = ((register_B & 0x01) << 3) | 0x07;

                return CINESTATE.state_BB;
            }
            static CINESTATE opOUT16_B_BB(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x07);

                if ((temp_byte - 0x06) != 0)
                    return opOUTsnd_B(opcode);

                if ((register_B & 0xFF) != 1)
                {
                    vgColour = FromX & 0x0F;

                    if (vgColour == 0)
                        vgColour = 1;
                }

                return CINESTATE.state_BB;
            }
            static CINESTATE opOUT64_B_BB(int opcode)
            {
                return CINESTATE.state_BB;
            }
            static CINESTATE opOUTWW_B_BB(int opcode)
            {
                return CINESTATE.state_BB;
            }
            static CINESTATE opLDAimm_A_AA(int opcode)
            {
                uint temp_word = (uint)(opcode & 0x0F);   /* pick up immediate value */
                temp_word <<= 8;                          /* LDAimm is the HIGH nibble!*/

                cmp_new = temp_word;                      /* set new comparison flag */

                SETA0(register_A);                     /* save old accA bit0 */
                SETFC(register_A);                     /* ??? clear carry? */

                cmp_old = register_A;                     /* step back cmp flag */
                register_A = temp_word;                   /* set the register */

                return CINESTATE.state_AA;                           /* swap state and end opcode */
            }
            static CINESTATE opLDAimm_B_AA(int opcode)
            {
                uint temp_word = (uint)(opcode & 0x0F);   /* pick up immediate value */
                temp_word <<= 8;                          /* LDAimm is the HIGH nibble!*/

                cmp_new = temp_word;                      /* set new comparison flag */

                SETA0(register_A);                     /* save old accA bit0 */
                SETFC(register_A);

                cmp_old = register_B;                     /* step back cmp flag */
                register_B = temp_word;                   /* set the register */

                return CINESTATE.state_AA;
            }
            static CINESTATE opLDAdir_A_AA(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);        /* snag imm value */

                register_I = (byte)((register_P << 4) + temp_byte);  /* set I register */

                cmp_new = ram[register_I];                  /* new acc value */

                SETA0(register_A);                          /* back up bit0 */
                SETFC(register_A);

                cmp_old = register_A;                          /* store old acc */
                register_A = cmp_new;                          /* store new acc */

                return CINESTATE.state_AA;
            }
            static CINESTATE opLDAdir_B_AA(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);        /* snag imm value */

                register_I = (byte)((register_P << 4) + temp_byte);  /* set I register */

                cmp_new = ram[register_I];                  /* new acc value */

                SETA0(register_A);                          /* back up bit0 */
                SETFC(register_A);

                cmp_old = register_B;                          /* store old acc */
                register_B = cmp_new;                          /* store new acc */

                return CINESTATE.state_AA;
            }
            static CINESTATE opLDAirg_A_AA(int opcode)
            {
                cmp_new = ram[register_I];

                SETA0(register_A);
                SETFC(register_A);

                cmp_old = register_A;
                register_A = cmp_new;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLDAirg_B_AA(int opcode)
            {
                cmp_new = ram[register_I];

                SETA0(register_A);
                SETFC(register_A);

                cmp_old = register_B;
                register_B = cmp_new;

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDimm_A_AA(int opcode)
            {
                uint temp_word = (uint)(opcode & 0x0F);     /* get imm value */

                cmp_new = temp_word;                        /* new acc value */
                SETA0(register_A);                       /* save old accA bit0 */
                cmp_old = register_A;                       /* store old acc for later */

                register_A += temp_word;                    /* add values */
                SETFC(register_A);                       /* store carry and extra */
                register_A &= 0xFFF;                        /* toss out >12bit carry */

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDimm_B_AA(int opcode)
            {
                uint temp_word = (uint)(opcode & 0x0F);     /* get imm value */

                cmp_new = temp_word;                        /* new acc value */
                SETA0(register_A);                       /* save old accA bit0 */
                cmp_old = register_B;                       /* store old acc for later */

                register_B += temp_word;                    /* add values */
                SETFC(register_B);                       /* store carry and extra */
                register_B &= 0xFFF;                        /* toss out >12bit carry */

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDimmX_A_AA(int opcode)
            {
                cmp_new = cpu_readop(register_PC++);       /* get extended value */
                SETA0(register_A);                       /* save old accA bit0 */
                cmp_old = register_A;                       /* store old acc for later */

                register_A += cmp_new;                      /* add values */
                SETFC(register_A);                       /* store carry and extra */
                register_A &= 0xFFF;                        /* toss out >12bit carry */

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDimmX_B_AA(int opcode)
            {
                cmp_new = cpu_readop(register_PC++);       /* get extended value */
                SETA0(register_A);                       /* save old accA bit0 */
                cmp_old = register_B;                       /* store old acc for later */

                register_B += cmp_new;                      /* add values */
                SETFC(register_B);                       /* store carry and extra */
                register_B &= 0xFFF;                        /* toss out >12bit carry */

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDdir_A_AA(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);         /* fetch imm value */

                register_I = (byte)((register_P << 4) + temp_byte);   /* set regI addr */

                cmp_new = ram[register_I];                   /* fetch imm real value */
                SETA0(register_A);                           /* store bit0 */
                cmp_old = register_A;                           /* store old acc value */

                register_A += cmp_new;                          /* do acc operation */
                SETFC(register_A);                           /* store carry and extra */
                register_A &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opADDdir_B_AA(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);         /* fetch imm value */

                register_I = (byte)((register_P << 4) + temp_byte);   /* set regI addr */

                cmp_new = ram[register_I];                   /* fetch imm real value */
                SETA0(register_A);                           /* store bit0 */
                cmp_old = register_B;                           /* store old acc value */

                register_B += cmp_new;                          /* do acc operation */
                SETFC(register_B);                           /* store carry and extra */
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opAWDirg_A_AA(int opcode)
            {
                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_A;

                register_A += cmp_new;
                SETFC(register_A);
                register_A &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opAWDirg_B_AA(int opcode)
            {
                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_B;

                register_B += cmp_new;
                SETFC(register_B);
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBimm_A_AA(int opcode)
            {
                /*
                 * 	SUBtractions are negate-and-add instructions of the CCPU; what
                 * 	a pain in the ass.
                 */

                uint temp_word = (uint)(opcode & 0x0F);

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_A;

                temp_word = (temp_word ^ 0xFFF) + 1;         /* ones compliment */
                register_A += temp_word;                       /* add */
                SETFC(register_A);                          /* pick up top bits */
                register_A &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBimm_B_AA(int opcode)
            {
                /*
                 * SUBtractions are negate-and-add instructions of the CCPU; what
                 * a pain in the ass.
                 */

                uint temp_word = (uint)(opcode & 0x0F);

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_B;

                temp_word = (temp_word ^ 0xFFF) + 1;         /* ones compliment */
                register_B += temp_word;                       /* add */
                SETFC(register_B);                          /* pick up top bits */
                register_B &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBimmX_A_AA(int opcode)
            {
                uint temp_word = cpu_readop(register_PC++);       /* snag imm value */

                cmp_new = temp_word;                          /* save cmp value */
                SETA0(register_A);                         /* store bit0 */
                cmp_old = register_A;                         /* back up regA */

                temp_word = (temp_word ^ 0xFFF) + 1;        /* ones compliment */
                register_A += temp_word;                      /* add */
                SETFC(register_A);                         /* pick up top bits */
                register_A &= 0x0FFF;                         /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBimmX_B_AA(int opcode)
            {
                uint temp_word = cpu_readop(register_PC++);       /* snag imm value */

                cmp_new = temp_word;                          /* save cmp value */
                SETA0(register_A);                         /* store bit0 */
                cmp_old = register_B;                         /* back up regA */

                temp_word = (temp_word ^ 0xFFF) + 1;        /* ones compliment */
                register_B += temp_word;                      /* add */
                SETFC(register_B);                         /* pick up top bits */
                register_B &= 0x0FFF;                         /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBdir_A_AA(int opcode)
            {
                uint temp_word = (uint)(opcode & 0x0F);         /* fetch imm value */

                register_I = (byte)((register_P << 4) + temp_word);   /* set regI addr */

                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_A;

                temp_word = (cmp_new ^ 0xFFF) + 1;           /* ones compliment */
                register_A += temp_word;                       /* add */
                SETFC(register_A);                          /* pick up top bits */
                register_A &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBdir_B_AA(int opcode)
            {
                uint temp_word;
                byte temp_byte = (byte)(opcode & 0x0F);         /* fetch imm value */

                register_I = (byte)((register_P << 4) + temp_byte);   /* set regI addr */

                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_B;

                temp_word = (cmp_new ^ 0xFFF) + 1;           /* ones compliment */
                register_B += temp_word;                       /* add */
                SETFC(register_B);                          /* pick up top bits */
                register_B &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBirg_A_AA(int opcode)
            {
                uint temp_word;

                /* sub [i] */
                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_A;

                temp_word = (cmp_new ^ 0xFFF) + 1;           /* ones compliment */
                register_A += temp_word;                       /* add */
                SETFC(register_A);                          /* pick up top bits */
                register_A &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opSUBirg_B_AA(int opcode)
            {
                uint temp_word;

                /* sub [i] */
                cmp_new = ram[register_I];
                SETA0(register_A);
                cmp_old = register_B;

                temp_word = (cmp_new ^ 0xFFF) + 1;           /* ones compliment */
                register_B += temp_word;                       /* add */
                SETFC(register_B);                          /* pick up top bits */
                register_B &= 0x0FFF;                          /* mask final regA value */

                return CINESTATE.state_AA;
            }
            static CINESTATE opCMPdir_A_AA(int opcode)
            {
                /*
                 * compare direct mode; don't modify regs, just set carry flag or not.
                 */

                uint temp_word;
                byte temp_byte = (byte)(opcode & 0x0F);       /* obtain relative addr */

                register_I = (byte)((register_P << 4) + temp_byte); /* build real addr */

                temp_word = ram[register_I];
                cmp_new = temp_word;                          /* new acc value */
                SETA0(register_A);                         /* backup bit0 */
                cmp_old = register_A;                         /* backup old acc */

                temp_word = (temp_word ^ 0xFFF) + 1;        /* ones compliment */
                temp_word += register_A;
                SETFC(temp_word);                          /* pick up top bits */

                return CINESTATE.state_AA;
            }
            static CINESTATE opCMPdir_B_AA(int opcode)
            {
                uint temp_word;
                byte temp_byte = (byte)(opcode & 0x0F);       /* obtain relative addr */

                register_I = (byte)((register_P << 4) + temp_byte); /* build real addr */

                temp_word = ram[register_I];
                cmp_new = temp_word;                          /* new acc value */
                SETA0(register_A);                         /* backup bit0 */
                cmp_old = register_B;                         /* backup old acc */

                temp_word = (temp_word ^ 0xFFF) + 1;        /* ones compliment */
                temp_word += register_B;
                SETFC(temp_word);                          /* pick up top bits */

                return CINESTATE.state_AA;
            }
            static CINESTATE opANDirg_A_AA(int opcode)
            {
                cmp_new = ram[register_I];                /* new acc value */
                SETA0(register_A);
                SETFC(register_A);
                cmp_old = register_A;

                register_A &= cmp_new;

                return CINESTATE.state_AA;
            }
            static CINESTATE opANDirg_B_AA(int opcode)
            {
                cmp_new = ram[register_I];                /* new acc value */
                SETA0(register_A);
                SETFC(register_A);
                cmp_old = register_B;

                register_B &= cmp_new;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLDJimm_A_A(int opcode)
            {
                byte temp_byte = cpu_readop(register_PC++);      /* upper part of address */
                temp_byte = (byte)((temp_byte << 4) |             /* Silly CCPU; Swap */
                            (temp_byte >> 4));              /* nibbles */

                /* put the upper 8 bits above the existing 4 bits */
                register_J = (uint)((opcode & 0x0F) | (temp_byte << 4));

                return CINESTATE.state_A;
            }
            static CINESTATE opLDJimm_B_BB(int opcode)
            {
                byte temp_byte = cpu_readop(register_PC++);      /* upper part of address */
                temp_byte = (byte)((temp_byte << 4) |             /* Silly CCPU; Swap */
                            (temp_byte >> 4));              /* nibbles */

                /* put the upper 8 bits above the existing 4 bits */
                register_J = (uint)((opcode & 0x0F) | (temp_byte << 4));

                return CINESTATE.state_BB;
            }
            static CINESTATE opLDJirg_A_A(int opcode)
            {
                /* load J reg from value at last dir addr */
                register_J = ram[register_I];
                return CINESTATE.state_A;
            }
            static CINESTATE opLDJirg_B_BB(int opcode)
            {
                register_J = ram[register_I];
                return CINESTATE.state_BB;
            }
            static CINESTATE opLDPimm_A_A(int opcode)
            {
                /* load page register from immediate */
                register_P = (byte)(opcode & 0x0F);  /* set page register */
                return CINESTATE.state_A;
            }
            static CINESTATE opLDPimm_B_BB(int opcode)
            {
                /* load page register from immediate */
                register_P = (byte)(opcode & 0x0F);  /* set page register */
                return CINESTATE.state_BB;
            }
            static CINESTATE opLDIdir_A_A(int opcode)
            {
                /* load regI directly .. */

                byte temp_byte = (byte)((register_P << 4) +           /* get ram page ... */
                         (opcode & 0x0F)); /* and imm half of ram addr.. */

                register_I = (byte)(ram[temp_byte] & 0xFF);      /* set/mask new register_I */

                return CINESTATE.state_A;
            }
            static CINESTATE opLDIdir_B_BB(int opcode)
            {
                byte temp_byte = (byte)((register_P << 4) +           /* get ram page ... */
                         (opcode & 0x0F)); /* and imm half of ram addr.. */

                register_I = (byte)(ram[temp_byte] & 0xFF);      /* set/mask new register_I */

                return CINESTATE.state_BB;
            }
            static CINESTATE opSTAdir_A_A(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);        /* snag imm value */

                register_I = (byte)((register_P << 4) + temp_byte);  /* set I register */

                ram[register_I] = register_A;               /* store acc to RAM */

                return CINESTATE.state_A;
            }
            static CINESTATE opSTAdir_B_BB(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0x0F);        /* snag imm value */

                register_I = (byte)((register_P << 4) + temp_byte);  /* set I register */

                ram[register_I] = register_B;               /* store acc to RAM */

                return CINESTATE.state_BB;
            }
            static CINESTATE opSTAirg_A_A(int opcode)
            {
                /*
                 * STA into address specified in regI. Nice and easy :)
                 */

                ram[register_I] = register_A;               /* store acc */

                return CINESTATE.state_A;
            }
            static CINESTATE opSTAirg_B_BB(int opcode)
            {
                ram[register_I] = register_B;               /* store acc */

                return CINESTATE.state_BB;
            }
            static CINESTATE opXLT_A_AA(int opcode)
            {
                /*
                 * XLT is weird; it loads the current accumulator with the bytevalue
                 * at ROM location pointed to by the accumulator; this allows the
                 * program to read the program itself..
                 * 		NOTE! Next opcode is *IGNORED!* because of a twisted side-effect
                 */

                cmp_new = cpu_readop(((register_PC - 1) & 0xF000) + register_A);   /* store new acc value */
                SETA0(register_A);           /* store bit0 */
                SETFC(register_A);
                cmp_old = register_A;           /* back up acc */

                register_A = cmp_new;           /* new acc value */

                register_PC++;               /* bump PC twice because XLT is fucked up */
                return CINESTATE.state_AA;
            }
            static CINESTATE opXLT_B_AA(int opcode)
            {
                cmp_new = cpu_readop(((register_PC - 1) & 0xF000) + register_B);   /* store new acc value */
                SETA0(register_A);           /* store bit0 */
                SETFC(register_A);
                cmp_old = register_B;           /* back up acc */

                register_B = cmp_new;           /* new acc value */

                register_PC++;               /* bump PC twice because XLT is fucked up */
                return CINESTATE.state_AA;
            }
            static CINESTATE opMULirg_A_AA(int opcode)
            {
                byte temp_byte = (byte)(opcode & 0xFF);    /* (for ease and speed) */
                uint temp_word = ram[register_I];               /* pick up ram value */

                cmp_new = temp_word;

                temp_word <<= 4;                              /* shift into ADD position */
                register_B <<= 4;                             /* get sign bit 15 */
                register_B |= (register_A >> 8);            /* bring in A high nibble */

                register_A = ((register_A & 0xFF) << 8) | /* shift over 8 bits */
                          temp_byte;  /* pick up opcode */

                if ((register_A & 0x100)!=0)
                {        				   /* 1bit shifted out? */
                    register_A = (register_A >> 8) |
                                 ((register_B & 0xFF) << 8);

                    SETA0(register_A & 0xFF);                  /* store bit0 */

                    register_A >>= 1;
                    register_A &= 0xFFF;

                    register_B = SAR(register_B, 4);
                    cmp_old = register_B & 0x0F;

                    register_B = SAR(register_B, 1);

                    register_B &= 0xFFF;
                    register_B += cmp_new;

                    SETFC(register_B);

                    register_B &= 0xFFF;
                }
                else
                {
                    register_A = (register_A >> 8) |    /* Bhigh | Alow */
                                 ((register_B & 0xFF) << 8);

                    temp_word = register_A & 0xFFF;

                    SETA0(temp_word & 0xFF);                   /* store bit0 */
                    cmp_old = temp_word;

                    temp_word += cmp_new;
                    SETFC(temp_word);

                    register_A >>= 1;
                    register_A &= 0xFFF;

                    register_B = SAR(register_B, 5);
                    register_B &= 0xFFF;
                }

                return CINESTATE.state_AA;
            }
            static uint SAR(uint var, int arg) { return (uint)((short)var >> arg); }
            static CINESTATE opMULirg_B_AA(int opcode)
            {
                uint temp_word = ram[register_I];

                cmp_new = temp_word;
                cmp_old = register_B;
                SETA0(register_A & 0xFF);

                register_B <<= 4;

                register_B = SAR(register_B, 5);

                if ((register_A & 0x01)!=0)
                {
                    register_B += temp_word;
                    SETFC(register_B);
                    register_B &= 0x0FFF;
                }
                else
                {
                    temp_word += register_B;
                    SETFC(temp_word);
                }

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSRe_A_AA(int opcode)
            {
                /*
                 * EB; right shift pure; fill new bit with zero.
                 */

                uint temp_word = 0x0BEB;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_A;

                temp_word += register_A;
                SETFC(temp_word);

                register_A >>= 1;
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSRe_B_AA(int opcode)
            {
                uint temp_word = 0x0BEB;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_B;

                temp_word += register_B;
                SETFC(temp_word);

                register_B >>= 1;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSRf_A_AA(int opcode)
            {
                printf("opLSRf 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSRf_B_AA(int opcode)
            {
                printf("opLSRf 2\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLe_A_AA(int opcode)
            {
                /*
                 * EC; left shift pure; fill new bit with zero *
                 */

                uint temp_word = 0x0CEC;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_A;

                temp_word += register_A;
                SETFC(temp_word);

                register_A <<= 1;
                register_A &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLe_B_AA(int opcode)
            {
                uint temp_word = 0x0CEC;                          /* data register */

                cmp_new = temp_word;                         /* magic value */
                SETA0(register_A);                        /* back up bit0 */
                cmp_old = register_B;                        /* store old acc */

                temp_word += register_B;                     /* add to acc */
                SETFC(temp_word);                         /* store carry flag */
                register_B <<= 1;                            /* add regA to itself */
                register_B &= 0xFFF;                         /* toss excess bits */

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLf_A_AA(int opcode)
            {
                printf("opLSLf 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLf_B_AA(int opcode)
            {
                printf("opLSLf 2\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opASRe_A_AA(int opcode)
            {
                /* agh! I dislike these silly 12bit processors :P */

                cmp_new = 0x0DED;
                SETA0(register_A);           /* store bit0 */
                SETFC(register_A);
                cmp_old = register_A;

                register_A <<= 4; /* get sign bit */
                register_A = SAR(register_A, 5);
                register_A &= 0xFFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opASRe_B_AA(int opcode)
            {
                cmp_new = 0x0DED;
                SETA0(register_A);
                SETFC(register_A);
                cmp_old = register_B;

                register_B <<= 4;
                register_B = SAR(register_B, 5);
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opASRf_A_AA(int opcode)
            {
                printf("opASRf 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opASRf_B_AA(int opcode)
            {
                printf("opASRf 2\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opASRDe_A_AA(int opcode)
            {
                /*
                 * Arithmetic shift right of D (A+B) .. B is high (sign bits).
                 * divide by 2, but leave the sign bit the same. (ie: 1010 -> 1001)
                 */
                uint temp_word = 0x0EEE;
                uint temp_word_2;

                cmp_new = temp_word;          /* save new acc value */
                SETA0(register_A & 0xFF);  /* save old accA bit0 */
                cmp_old = register_A;         /* save old acc */

                temp_word += register_A;
                SETFC(temp_word);

                register_A <<= 4;
                register_B <<= 4;

                temp_word_2 = (register_B >> 4) << 15;
                register_B = SAR(register_B, 5);
                register_A = (register_A >> 1) | temp_word_2;
                register_A >>= 4;

                register_B &= 0x0FFF;
                return CINESTATE.state_AA;
            }
            static CINESTATE opASRDe_B_AA(int opcode)
            {
                uint temp_word = 0x0EEE;

                cmp_new = temp_word;
                SETA0(register_A & 0xFF);
                cmp_old = register_B;

                temp_word += register_B;
                SETFC(temp_word);
                register_B <<= 4;
                register_B = SAR(register_B, 5);
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opASRDf_A_AA(int opcode)
            {
                printf("opASRDf 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opASRDf_B_AA(int opcode)
            {
                printf("opASRDf 2\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLDe_A_AA(int opcode)
            {
                /* LSLDe -- Left shift through both accumulators; lossy in middle. */

                uint temp_word = 0x0FEF;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_A;

                temp_word += register_A;
                SETFC(temp_word);
                register_A <<= 1;                             /* logical shift left */
                register_A &= 0xFFF;

                register_B <<= 1;
                register_B &= 0xFFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLDe_B_AA(int opcode)
            {
                printf("opLSLD 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLDf_A_AA(int opcode)
            {
                /* LSLDf */

                uint temp_word = 0x0FFF;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_A;

                temp_word += register_A;
                SETFC(temp_word);

                register_A <<= 1;
                register_A &= 0x0FFF;

                register_B <<= 1;
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opLSLDf_B_AA(int opcode)
            {
                /* not 'the same' as the A->AA version above */

                uint temp_word = 0x0FFF;

                cmp_new = temp_word;
                SETA0(register_A);
                cmp_old = register_B;

                temp_word += register_B;
                SETFC(temp_word);

                register_B <<= 1;
                register_B &= 0x0FFF;

                return CINESTATE.state_AA;
            }
            static CINESTATE opJMP_A_A(int opcode)
            {
                /*
                 * simple jump; change PC and continue..
                 */

                JMP();
                return CINESTATE.state_A;
            }
            static CINESTATE opJMP_B_BB(int opcode)
            {
                JMP();
                return CINESTATE.state_BB;
            }
            static CINESTATE opJEI_A_A(int opcode)
            {
                if ((FromX & 0x800)!=0)
                    FromX |= 0xF000;
                if ((CCPU_READPORT(CCPU_PORT_IOOUTPUTS) & 0x80)==0)
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKY) - (int)FromX) < 0x800)
                        JMP();
                }
                else
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKX) - (int)FromX) < 0x800)
                        JMP();
                }

                return CINESTATE.state_A;
            }
            static CINESTATE opJEI_B_BB(int opcode)
            {
                if ((FromX & 0x800)!=0)
                    FromX |= 0xF000;
                if ((CCPU_READPORT(CCPU_PORT_IOOUTPUTS) & 0x80)==0)
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKY) - (int)FromX) < 0x800)
                        JMP();
                }
                else
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKX) - (int)FromX) < 0x800)
                        JMP();
                }

                return CINESTATE.state_BB;
            }
            static CINESTATE opJEI_A_B(int opcode)
            {
                if ((FromX & 0x800)!=0)
                    FromX |= 0xF000;
                if ((CCPU_READPORT(CCPU_PORT_IOOUTPUTS) & 0x80)==0)
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKY) - (int)FromX) < 0x800)
                        JMP();
                }
                else
                {
                    if ((CCPU_READPORT(CCPU_PORT_IN_JOYSTICKX) - (int)FromX) < 0x800)
                        JMP();
                }

                return CINESTATE.state_B;
            }
            static CINESTATE opJMI_A_A(int opcode)
            {
                /*
                 * previous instruction was not an ACC instruction, nor was the
                 * instruction twice back a USB, therefore minus flag test the
                 * current A-reg
                 */

                /* negative acc? */
                if ((register_A & 0x800)!=0)
                    JMP();	  /* yes -- do jump */

                return CINESTATE.state_A;
            }
            static CINESTATE opJMI_AA_A(int opcode)
            {
                /* previous acc negative? Jump if so... */
                if ((cmp_old & 0x800)!=0)
                    JMP();

                return CINESTATE.state_A;
            }
            static CINESTATE opJMI_BB_A(int opcode)
            {
                if ((register_B & 0x800)!=0)
                    JMP();

                return CINESTATE.state_A;
            }
            static CINESTATE opJMI_B_BB(int opcode)
            {
                if ((register_A & 0x800)!=0)
                    JMP();

                return CINESTATE.state_BB;
            }
            static CINESTATE opJLT_A_A(int opcode)
            {
                /* jump if old acc equals new acc */

                if (cmp_new < cmp_old)
                    JMP();

                return CINESTATE.state_A;
            }
            static CINESTATE opJLT_B_BB(int opcode)
            {
                if (cmp_new < cmp_old)
                    JMP();

                return CINESTATE.state_BB;
            }
            static CINESTATE opJEQ_A_A(int opcode)
            {
                /* jump if equal */

                if (cmp_new == cmp_old)
                    JMP();

                return CINESTATE.state_A;
            }
            static CINESTATE opJEQ_B_BB(int opcode)
            {
                if (cmp_new == cmp_old)
                    JMP();

                return CINESTATE.state_BB;
            }
            static CINESTATE opJA0_A_A(int opcode)
            {
                if ((acc_a0 & 0x01) != 0)
                    JMP();

                return CINESTATE.state_A;
            }
            static CINESTATE opJA0_B_BB(int opcode)
            {
                if ((acc_a0 & 0x01) != 0)
                    JMP();

                return CINESTATE.state_BB;
            }
            static CINESTATE opJNC_A_A(int opcode)
            {
                if ((((flag_C >> 8) & 0xFF) & 0xF0)==0)
                    JMP(); /* no carry, so jump */

                return CINESTATE.state_A;
            }
            static CINESTATE opJNC_B_BB(int opcode)
            {
                if ((((flag_C >> 8) & 0xFF) & 0xF0)==0)
                    JMP(); /* no carry, so jump */

                return CINESTATE.state_BB;
            }
            static CINESTATE opJDR_A_A(int opcode)
            {
                /*
                 * Calculate number of cycles executed since
                 * last 'VDR' instruction, add two and use as
                 * cycle count, never branch
                 */
                return CINESTATE.state_A;
            }
            static CINESTATE opJDR_B_BB(int opcode)
            {
                /*
                 * Calculate number of cycles executed since
                 * last 'VDR' instruction, add two and use as
                 * cycle count, never branch
                 */
                return CINESTATE.state_BB;
            }
            static CINESTATE opNOP_A_A(int opcode)
            {
                return CINESTATE.state_A;
            }
            static CINESTATE opNOP_B_BB(int opcode)
            {
                return CINESTATE.state_BB;
            }
            static CINESTATE opJPP32_A_B(int opcode)
            {
                /*
                 * 00 = Offset 0000h
                 * 01 = Offset 1000h
                 * 02 = Offset 2000h
                 * 03 = Offset 3000h
                 * 04 = Offset 4000h
                 * 05 = Offset 5000h
                 * 06 = Offset 6000h
                 * 07 = Offset 7000h
                 */
                uint temp_word = (uint)((register_P & 0x07) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_B;
            }
            static CINESTATE opJPP32_B_BB(int opcode)
            {
                uint temp_word = (uint)((register_P & 0x07) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_BB;
            }
            static CINESTATE opJPP16_A_B(int opcode)
            {
                /*
                 * 00 = Offset 0000h
                 * 01 = Offset 1000h
                 * 02 = Offset 2000h
                 * 03 = Offset 3000h
                 */
                uint temp_word = (uint)((register_P & 0x03) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_B;
            }
            static CINESTATE opJPP16_B_BB(int opcode)
            {
                uint temp_word = (uint)((register_P & 0x03) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_BB;
            }
            static CINESTATE opJMP_A_B(int opcode)
            {
                JMP();
                return CINESTATE.state_B;
            }
            static CINESTATE opJPP8_A_B(int opcode)
            {
                /*
                 * "long jump"; combine P and J to jump to a new far location (that can
                 * 	be more than 12 bits in address). After this jump, further jumps
                 * are local to this new page.
                 */
                uint temp_word = (uint)(((register_P & 0x03) - 1) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_B;
            }
            static CINESTATE opJPP8_B_BB(int opcode)
            {
                uint temp_word = (uint)(((register_P & 0x03) - 1) << 12);  /* rom offset */
                register_PC = register_J + temp_word;
                return CINESTATE.state_BB;
            }
            static CINESTATE opJMI_A_B(int opcode)
            {
                if ((register_A & 0x800) != 0)
                    JMP();

                return CINESTATE.state_B;
            }
            static CINESTATE opJMI_AA_B(int opcode)
            {
                printf("opJMI 3\n");
                return CINESTATE.state_B;
            }
            static CINESTATE opJMI_BB_B(int opcode)
            {
                printf("opJMI 4\n");
                return CINESTATE.state_B;
            }
            static CINESTATE opJLT_A_B(int opcode)
            {
                if (cmp_new < cmp_old)
                    JMP();

                return CINESTATE.state_B;
            }
            static CINESTATE opJEQ_A_B(int opcode)
            {
                if (cmp_new == cmp_old)
                    JMP();

                return CINESTATE.state_B;
            }
            static CINESTATE opJA0_A_B(int opcode)
            {
                if ((acc_a0 & 0x01) != 0)
                    JMP();

                return CINESTATE.state_B;
            }
            static CINESTATE opJNC_A_B(int opcode)
            {
                if ((((flag_C >> 8) & 0xFF) & 0x0F0) == 0)
                    JMP(); /* if no carry, jump */

                return CINESTATE.state_B;
            }
            static CINESTATE opJDR_A_B(int opcode)
            {
                /* register_PC++; */
                printf("The hell? No PC incrementing?\n");
                return CINESTATE.state_B;
            }
            static CINESTATE opNOP_A_B(int opcode)
            {
                return CINESTATE.state_B;
            }
            static CINESTATE opLLT_A_AA(int opcode)
            {
                byte temp_byte = 0;

                while (true)
                {
                    uint temp_word = register_A >> 8;  /* register_A's high bits */
                    temp_word &= 0x0A;                   /* only want PA11 and PA9 */

                    if (temp_word != 0)
                    {
                        temp_word ^= 0x0A;                   /* flip the bits */

                        if (temp_word != 0)
                            break;                	        /* if not zero, mismatch found */
                    }

                    temp_word = register_B >> 8;         /* regB's top bits */
                    temp_word &= 0x0A;                   /* only want SA11 and SA9 */

                    if (temp_word != 0)
                    {
                        temp_word ^= 0x0A;                   /* flip bits */

                        if (temp_word != 0)
                            break;                          /* if not zero, mismatch found */
                    }

                    register_A <<= 1;                    /* shift regA */
                    register_B <<= 1;                    /* shift regB */

                    temp_byte++;
                    if (temp_byte == 0)
                        return CINESTATE.state_AA;
                    /* try again */
                }

                vgShiftLength = temp_byte;
                register_A &= 0x0FFF;
                register_B &= 0x0FFF;
                return CINESTATE.state_AA;
            }
            static CINESTATE opLLT_B_AA(int opcode)
            {
                printf("opLLT 1\n");
                return CINESTATE.state_AA;
            }
            static CINESTATE opVIN_A_A(int opcode)
            {
                /* set the starting address of a vector */

                FromX = register_A & 0xFFF;            /* regA goes to x-coord */
                FromY = register_B & 0xFFF;            /* regB goes to y-coord */

                return CINESTATE.state_A;
            }
            static CINESTATE opVIN_B_BB(int opcode)
            {
                FromX = register_A & 0xFFF;            /* regA goes to x-coord */
                FromY = register_B & 0xFFF;            /* regB goes to y-coord */

                return CINESTATE.state_BB;
            }
            static CINESTATE opWAI_A_A(int opcode)
            {
                /* wait for a tick on the watchdog */
                bNewFrame = true;
                bailOut = true;
                return state;
            }
            static CINESTATE opWAI_B_BB(int opcode)
            {
                bNewFrame = true;
                bailOut = true;
                return state;
            }
            static CINESTATE opVDR_A_A(int opcode)
            {
                /* set ending points and draw the vector, or buffer for a later draw. */
                int ToX = (int)(register_A & 0xFFF);
                int ToY = (int)(register_B & 0xFFF);

                /*
                 * shl 20, sar 20; this means that if the CCPU reg should be -ve,
                 * we should be negative as well.. sign extended.
                 */
                if ((FromX & 0x800) != 0)
                    FromX |= 0xFFFFF000;
                if ((ToX & 0x800) != 0)
                    ToX |= unchecked((int)0xFFFFF000);
                if ((FromY & 0x800) != 0)
                    FromY |= 0xFFFFF000;
                if ((ToY & 0x800) != 0)
                    ToY |=unchecked((int) 0xFFFFF000);

                /* figure out the vector */
                ToX -= (int)FromX;
                ToX = (int)SAR((uint)ToX, vgShiftLength);
                ToX += (int)FromX;

                ToY -= (int)FromY;
                ToY = (int)SAR((uint)ToY, vgShiftLength);
                ToY += (int)FromY;

                /* do orientation flipping, etc. */
                /* NOTE: this has been removed on the assumption that the vector draw routine can do it all */
#if !RAW_VECTORS
                if (bFlipX)
                {
                    ToX = sdwGameXSize - ToX;
                    FromX = sdwGameXSize - FromX;
                }

                if (bFlipY)
                {
                    ToY = sdwGameYSize - ToY;
                    FromY = sdwGameYSize - FromY;
                }

                FromX += sdwXOffset;
                ToX += sdwXOffset;

                FromY += sdwYOffset;
                ToY += sdwYOffset;

                /* check real coords */
                if (bSwapXY)
                {
                    uint temp_word;

                    temp_word = (uint)ToY;
                    ToY = ToX;
                    ToX = (int)temp_word;

                    temp_word = FromY;
                    FromY = FromX;
                    FromX = temp_word;
                }
#endif

                /* render the line */
                cinemat.CinemaVectorData((int)FromX, (int)FromY, ToX, ToY, (int)vgColour);

                return CINESTATE.state_A;
            }
            static CINESTATE opVDR_B_BB(int opcode)
            {
                printf("opVDR B 1\n");
                return CINESTATE.state_BB;
            }
            static CINESTATE tJPP_A_B(int opcode)
            {
                /* MSIZE -- 0 = 4k, 1 = 8k, 2 = 16k, 3 = 32k */
                switch (ccpu_msize)
                {
                    case CCPU_MEMSIZE_4K:
                    case CCPU_MEMSIZE_8K:
                        return opJPP8_A_B(opcode);
                    case CCPU_MEMSIZE_16K:
                        return opJPP16_A_B(opcode);
                    case CCPU_MEMSIZE_32K:
                        return opJPP32_A_B(opcode);
                }
                printf("Out of range JPP!\n");
                return opJPP32_A_B(opcode);
            }
            static CINESTATE tJPP_B_BB(int opcode)
            {
                /* MSIZE -- 0 = 4k, 1 = 8k, 2 = 16k, 3 = 32k */
                switch (ccpu_msize)
                {
                    case CCPU_MEMSIZE_4K:
                    case CCPU_MEMSIZE_8K:
                        return opJPP8_B_BB(opcode);
                    case CCPU_MEMSIZE_16K:
                        return opJPP16_B_BB(opcode);
                    case CCPU_MEMSIZE_32K:
                        return opJPP32_B_BB(opcode);
                }
                printf("Out of range JPP!\n");
                return state;
            }
            static CINESTATE tJMI_A_B(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_A_B(opcode) : opJEI_A_B(opcode);
            }
            static CINESTATE tJMI_A_A(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_A_A(opcode) : opJEI_A_A(opcode);
            }
            static CINESTATE tJMI_AA_B(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_AA_B(opcode) : opJEI_AA_B(opcode);
            }
            static CINESTATE tJMI_AA_A(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_AA_A(opcode) : opJEI_A_A(opcode);
            }
            static CINESTATE tJMI_B_BB1(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_B_BB(opcode) : opJEI_B_BB(opcode);
            }
            static CINESTATE tJMI_BB_B(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_BB_B(opcode) : opJEI_A_B(opcode);
            }
            static CINESTATE tJMI_BB_A(int opcode)
            {
                return (ccpu_jmi_dip) != 0 ? opJMI_BB_A(opcode) : opJEI_A_A(opcode);
            }
            static CINESTATE tOUT_A_A(int opcode)
            {
                switch (ccpu_monitor)
                {
                    case CCPU_MONITOR_16LEV:
                        return opOUT16_A_A(opcode);
                    case CCPU_MONITOR_64LEV:
                        return opOUT64_A_A(opcode);
                    case CCPU_MONITOR_WOWCOL:
                        return opOUTWW_A_A(opcode);
                    default:
                        return opOUTbi_A_A(opcode);
                }
            }
            static CINESTATE tOUT_B_BB(int opcode)
            {
                switch (ccpu_monitor)
                {
                    case CCPU_MONITOR_16LEV:
                        return opOUT16_B_BB(opcode);
                    case CCPU_MONITOR_64LEV:
                        return opOUT64_B_BB(opcode);
                    case CCPU_MONITOR_WOWCOL:
                        return opOUTWW_B_BB(opcode);
                    default:
                        return opOUTbi_B_BB(opcode);
                }
            }
            static CINESTATE opINP_AA_AA(int opcode) { return opINP_A_AA(opcode); }
            static CINESTATE opINP_BB_AA(int opcode) { return opINP_A_AA(opcode); }
            static CINESTATE opOUTbi_AA_A(int opcode) { return opOUTbi_A_A(opcode); }
            static CINESTATE opOUTbi_BB_A(int opcode) { return opOUTbi_A_A(opcode); }
            static CINESTATE opOUT16_AA_A(int opcode) { return opOUT16_A_A(opcode); }
            static CINESTATE opOUT16_BB_A(int opcode) { return opOUT16_A_A(opcode); }
            static CINESTATE opOUT64_AA_A(int opcode) { return opOUT64_A_A(opcode); }
            static CINESTATE opOUT64_BB_A(int opcode) { return opOUT64_A_A(opcode); }
            static CINESTATE opOUTWW_AA_A(int opcode) { return opOUTWW_A_A(opcode); }
            static CINESTATE opOUTWW_BB_A(int opcode) { return opOUTWW_A_A(opcode); }
            static CINESTATE opLDAimm_AA_AA(int opcode) { return opLDAimm_A_AA(opcode); }
            static CINESTATE opLDAimm_BB_AA(int opcode) { return opLDAimm_A_AA(opcode); }
            static CINESTATE opLDAdir_AA_AA(int opcode) { return opLDAdir_A_AA(opcode); }
            static CINESTATE opLDAdir_BB_AA(int opcode) { return opLDAdir_A_AA(opcode); }
            static CINESTATE opLDAirg_AA_AA(int opcode) { return opLDAirg_A_AA(opcode); }
            static CINESTATE opLDAirg_BB_AA(int opcode) { return opLDAirg_A_AA(opcode); }
            static CINESTATE opADDimm_AA_AA(int opcode) { return opADDimm_A_AA(opcode); }
            static CINESTATE opADDimm_BB_AA(int opcode) { return opADDimm_A_AA(opcode); }
            static CINESTATE opADDimmX_AA_AA(int opcode) { return opADDimmX_A_AA(opcode); }
            static CINESTATE opADDimmX_BB_AA(int opcode) { return opADDimmX_A_AA(opcode); }
            static CINESTATE opADDdir_AA_AA(int opcode) { return opADDdir_A_AA(opcode); }
            static CINESTATE opADDdir_BB_AA(int opcode) { return opADDdir_A_AA(opcode); }
            static CINESTATE opAWDirg_AA_AA(int opcode) { return opAWDirg_A_AA(opcode); }
            static CINESTATE opAWDirg_BB_AA(int opcode) { return opAWDirg_A_AA(opcode); }
            static CINESTATE opADDirg_A_AA(int opcode) { return opAWDirg_A_AA(opcode); }
            static CINESTATE opADDirg_AA_AA(int opcode) { return opAWDirg_A_AA(opcode); }
            static CINESTATE opADDirg_BB_AA(int opcode) { return opAWDirg_A_AA(opcode); }
            static CINESTATE opADDirg_B_AA(int opcode) { return opAWDirg_B_AA(opcode); }
            static CINESTATE opSUBimm_AA_AA(int opcode) { return opSUBimm_A_AA(opcode); }
            static CINESTATE opSUBimm_BB_AA(int opcode) { return opSUBimm_A_AA(opcode); }
            static CINESTATE opSUBimmX_AA_AA(int opcode) { return opSUBimmX_A_AA(opcode); }
            static CINESTATE opSUBimmX_BB_AA(int opcode) { return opSUBimmX_A_AA(opcode); }
            static CINESTATE opSUBdir_AA_AA(int opcode) { return opSUBdir_A_AA(opcode); }
            static CINESTATE opSUBdir_BB_AA(int opcode) { return opSUBdir_A_AA(opcode); }
            static CINESTATE opSUBirg_AA_AA(int opcode) { return opSUBirg_A_AA(opcode); }
            static CINESTATE opSUBirg_BB_AA(int opcode) { return opSUBirg_A_AA(opcode); }
            static CINESTATE opCMPdir_AA_AA(int opcode) { return opCMPdir_A_AA(opcode); }
            static CINESTATE opCMPdir_BB_AA(int opcode) { return opCMPdir_A_AA(opcode); }
            static CINESTATE opANDirg_AA_AA(int opcode) { return opANDirg_A_AA(opcode); }
            static CINESTATE opANDirg_BB_AA(int opcode) { return opANDirg_A_AA(opcode); }
            static CINESTATE opLDJimm_AA_A(int opcode) { return opLDJimm_A_A(opcode); }
            static CINESTATE opLDJimm_BB_A(int opcode) { return opLDJimm_A_A(opcode); }
            static CINESTATE opLDJirg_AA_A(int opcode) { return opLDJirg_A_A(opcode); }
            static CINESTATE opLDJirg_BB_A(int opcode) { return opLDJirg_A_A(opcode); }
            static CINESTATE opLDPimm_AA_A(int opcode) { return opLDPimm_A_A(opcode); }
            static CINESTATE opLDPimm_BB_A(int opcode) { return opLDPimm_A_A(opcode); }
            static CINESTATE opLDIdir_AA_A(int opcode) { return opLDIdir_A_A(opcode); }
            static CINESTATE opLDIdir_BB_A(int opcode) { return opLDIdir_A_A(opcode); }
            static CINESTATE opSTAdir_AA_A(int opcode) { return opSTAdir_A_A(opcode); }
            static CINESTATE opSTAdir_BB_A(int opcode) { return opSTAdir_A_A(opcode); }
            static CINESTATE opSTAirg_AA_A(int opcode) { return opSTAirg_A_A(opcode); }
            static CINESTATE opSTAirg_BB_A(int opcode) { return opSTAirg_A_A(opcode); }
            static CINESTATE opXLT_AA_AA(int opcode) { return opXLT_A_AA(opcode); }
            static CINESTATE opXLT_BB_AA(int opcode) { return opXLT_A_AA(opcode); }
            static CINESTATE opMULirg_AA_AA(int opcode) { return opMULirg_A_AA(opcode); }
            static CINESTATE opMULirg_BB_AA(int opcode) { return opMULirg_A_AA(opcode); }
            static CINESTATE opLSRe_AA_AA(int opcode) { return opLSRe_A_AA(opcode); }
            static CINESTATE opLSRe_BB_AA(int opcode) { return opLSRe_A_AA(opcode); }
            static CINESTATE opLSRf_AA_AA(int opcode) { return opLSRf_A_AA(opcode); }
            static CINESTATE opLSRf_BB_AA(int opcode) { return opLSRf_A_AA(opcode); }
            static CINESTATE opLSLe_AA_AA(int opcode) { return opLSLe_A_AA(opcode); }
            static CINESTATE opLSLe_BB_AA(int opcode) { return opLSLe_A_AA(opcode); }
            static CINESTATE opLSLf_AA_AA(int opcode) { return opLSLf_A_AA(opcode); }
            static CINESTATE opLSLf_BB_AA(int opcode) { return opLSLf_A_AA(opcode); }
            static CINESTATE opASRe_AA_AA(int opcode) { return opASRe_A_AA(opcode); }
            static CINESTATE opASRe_BB_AA(int opcode) { return opASRe_A_AA(opcode); }
            static CINESTATE opASRf_AA_AA(int opcode) { return opASRf_A_AA(opcode); }
            static CINESTATE opASRf_BB_AA(int opcode) { return opASRf_A_AA(opcode); }
            static CINESTATE opASRDe_AA_AA(int opcode) { return opASRDe_A_AA(opcode); }
            static CINESTATE opASRDe_BB_AA(int opcode) { return opASRDe_A_AA(opcode); }
            static CINESTATE opASRDf_AA_AA(int opcode) { return opASRDf_A_AA(opcode); }
            static CINESTATE opASRDf_BB_AA(int opcode) { return opASRDf_A_AA(opcode); }
            static CINESTATE opLSLDe_AA_AA(int opcode) { return opLSLDe_A_AA(opcode); }
            static CINESTATE opLSLDe_BB_AA(int opcode) { return opLSLDe_A_AA(opcode); }
            static CINESTATE opLSLDf_AA_AA(int opcode) { return opLSLDf_A_AA(opcode); }
            static CINESTATE opLSLDf_BB_AA(int opcode) { return opLSLDf_A_AA(opcode); }
            static CINESTATE opJMP_AA_A(int opcode) { return opJMP_A_A(opcode); }
            static CINESTATE opJMP_BB_A(int opcode) { return opJMP_A_A(opcode); }
            static CINESTATE opJEI_AA_A(int opcode) { return opJEI_A_A(opcode); }
            static CINESTATE opJEI_BB_A(int opcode) { return opJEI_A_A(opcode); }
            static CINESTATE opJEI_AA_B(int opcode) { return opJEI_A_B(opcode); }
            static CINESTATE opJEI_BB_B(int opcode) { return opJEI_A_B(opcode); }
            static CINESTATE opJLT_AA_A(int opcode) { return opJLT_A_A(opcode); }
            static CINESTATE opJLT_BB_A(int opcode) { return opJLT_A_A(opcode); }
            static CINESTATE opJEQ_AA_A(int opcode) { return opJEQ_A_A(opcode); }
            static CINESTATE opJEQ_BB_A(int opcode) { return opJEQ_A_A(opcode); }
            static CINESTATE opJA0_AA_A(int opcode) { return opJA0_A_A(opcode); }
            static CINESTATE opJA0_BB_A(int opcode) { return opJA0_A_A(opcode); }
            static CINESTATE opJNC_AA_A(int opcode) { return opJNC_A_A(opcode); }
            static CINESTATE opJNC_BB_A(int opcode) { return opJNC_A_A(opcode); }
            static CINESTATE opJDR_AA_A(int opcode) { return opJDR_A_A(opcode); }
            static CINESTATE opJDR_BB_A(int opcode) { return opJDR_A_A(opcode); }
            static CINESTATE opNOP_AA_A(int opcode) { return opNOP_A_A(opcode); }
            static CINESTATE opNOP_BB_A(int opcode) { return opNOP_A_A(opcode); }
            static CINESTATE opJPP32_AA_B(int opcode) { return opJPP32_A_B(opcode); }
            static CINESTATE opJPP32_BB_B(int opcode) { return opJPP32_A_B(opcode); }
            static CINESTATE opJPP16_AA_B(int opcode) { return opJPP16_A_B(opcode); }
            static CINESTATE opJPP16_BB_B(int opcode) { return opJPP16_A_B(opcode); }
            static CINESTATE opJPP8_AA_B(int opcode) { return opJPP8_A_B(opcode); }
            static CINESTATE opJPP8_BB_B(int opcode) { return opJPP8_A_B(opcode); }
            static CINESTATE opJLT_AA_B(int opcode) { return opJLT_A_B(opcode); }
            static CINESTATE opJLT_BB_B(int opcode) { return opJLT_A_B(opcode); }
            static CINESTATE opJEQ_AA_B(int opcode) { return opJEQ_A_B(opcode); }
            static CINESTATE opJEQ_BB_B(int opcode) { return opJEQ_A_B(opcode); }
            static CINESTATE opJA0_AA_B(int opcode) { return opJA0_A_B(opcode); }
            static CINESTATE opJA0_BB_B(int opcode) { return opJA0_A_B(opcode); }
            static CINESTATE opJNC_AA_B(int opcode) { return opJNC_A_B(opcode); }
            static CINESTATE opJNC_BB_B(int opcode) { return opJNC_A_B(opcode); }
            static CINESTATE opJDR_AA_B(int opcode) { return opJDR_A_B(opcode); }
            static CINESTATE opJDR_BB_B(int opcode) { return opJDR_A_B(opcode); }
            static CINESTATE opNOP_AA_B(int opcode) { return opNOP_A_B(opcode); }
            static CINESTATE opNOP_BB_B(int opcode) { return opNOP_A_B(opcode); }
            static CINESTATE opLLT_AA_AA(int opcode) { return opLLT_A_AA(opcode); }
            static CINESTATE opLLT_BB_AA(int opcode) { return opLLT_A_AA(opcode); }
            static CINESTATE opVIN_AA_A(int opcode) { return opVIN_A_A(opcode); }
            static CINESTATE opVIN_BB_A(int opcode) { return opVIN_A_A(opcode); }
            static CINESTATE opWAI_AA_A(int opcode) { return opWAI_A_A(opcode); }
            static CINESTATE opWAI_BB_A(int opcode) { return opWAI_A_A(opcode); }
            static CINESTATE opVDR_AA_A(int opcode) { return opVDR_A_A(opcode); }
            static CINESTATE opVDR_BB_A(int opcode) { return opVDR_A_A(opcode); }
            static CINESTATE tOUT_AA_A(int opcode) { return tOUT_A_A(opcode); }
            static CINESTATE tOUT_BB_A(int opcode) { return tOUT_B_BB(opcode); }
            static CINESTATE tJMI_B_BB2(int opcode) { return tJMI_B_BB1(opcode); }
            static CINESTATE tJPP_AA_B(int opcode) { return tJPP_A_B(opcode); }
            static CINESTATE tJPP_BB_B(int opcode) { return tJPP_A_B(opcode); }
        }
    }
}
