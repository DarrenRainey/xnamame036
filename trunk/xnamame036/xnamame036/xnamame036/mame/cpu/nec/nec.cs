using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public partial class Mame
    {
        public partial class cpu_nec : cpu_interface
        {
            public const byte NEC_INT_NONE = 0, NEC_NMI_INT = 2;
           
            public class necbasicregs
            {                   /* eight general registers */
                public  byte[] b = new byte[16];
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
                            return (ushort)(b[ 1 + index * 2] << 8 | b[index * 2]);
                        }
                        set
                        {
                            b[ index * 2] = (byte)value;
                            b[ index * 2 + 1] = (byte)(value >> 8);
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

                public  _w w;
                public necbasicregs() { w = new _w(b); }

            } 
            class nec_Regs
            {
                public void Clear()
                {
                    regs = new necbasicregs();
                    ip = 0; flags = 0; AuxVal = 0; OverVal = 0; SignVal = 0; ZeroVal = 0; CarryVal = 0; ParityVal = 0;
                    pending_irq = 0; nmi_state = 0; irq_state = 0; prefix_base = 0; seg_prefix = 0;
                    Array.Clear(_base, 0, 4);
                    Array.Clear(sregs, 0, 4);
                }
                public necbasicregs regs = new necbasicregs();
                public int ip;
                public ushort flags;
                public uint[] _base = new uint[4];
                public ushort[] sregs = new ushort[4];
                public irqcallback irq_callback;
                public int AuxVal, OverVal, SignVal, ZeroVal, CarryVal, ParityVal;
                public byte TF, IF, DF, MF;
                public byte pending_irq;
                public sbyte nmi_state, irq_state;
                public uint prefix_base;
                public byte seg_prefix;
            }
            const byte ES = 0, CS = 1, SS = 2, DS = 3;
            const byte AW = 0, CW = 1, DW = 2, BW = 3, SP = 4, BP = 5, IX = 6, IY = 7;
            enum SREGS { ES, CS, SS, DS } ;
            enum WREGS { AW, CW, DW, BW, SP, BP, IX, IY } ;

            

#if WINDOWS
            const byte AL = 0, AH = 1, CL = 2, CH = 3, DL = 4, DH = 5, BL = 6, BH = 7, SPL = 8, SPH = 9, BPL = 10, BPH = 11, IXL = 12, IXH = 13, IYL = 14, IYH = 15;
 enum BREGS{ AL,AH,CL,CH,DL,DH,BL,BH,SPL,SPH,BPL,BPH,IXL,IXH,IYL,IYH } ;
#else
 enum BREGS{ AH,AL,CH,CL,DH,DL,BH,BL,SPH,SPL,BPH,BPL,IXH,IXL,IYH,IYL } ;
#endif


            const byte INT_IRQ = 0x01;
            const byte NMI_IRQ = 0x02;


            static ushort[] bytes = {
	1,2,4,8,16,32,64,128,256,
	512,1024,2048,4096,8192,16384,32768,65336
};


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
 
            static byte[] parity_table = new byte[256];

            public static int[] nec_ICount = new int[1];

            static nec_Regs I = new nec_Regs();

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
                    case CPU_INFO_NAME: return "V30";
                    case CPU_INFO_FAMILY: return "NEC V-Series";
                    case CPU_INFO_VERSION: return "1.6";
                    case CPU_INFO_FILE: return "nec.cs";
                    case CPU_INFO_CREDITS: return "Real mode NEC emulator v1.3 by Oliver Bergmann\n(initial work based on Fabrice Fabian's i86 core)";
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
                return nec_execute(cycles);
            }
            public override uint get_context(ref object reg)
            {
                reg = I;
                return 1;
            }
            public override void create_context(ref object reg)
            {
                reg = new nec_Regs();
            }
            public override uint get_pc()
            {
                return (I._base[CS] + (ushort)I.ip);
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
                BREGS[] reg_name = { BREGS.AL, BREGS.CL, BREGS.DL, BREGS.BL, BREGS.AH, BREGS.CH, BREGS.DH, BREGS.BH };
                I.Clear();
                //memset( &I, 0, sizeof(I) );

                I.sregs[CS] = 0xffff;
                I._base[CS] = (uint)(I.sregs[CS] << 4);

                {
                    if (cur_mrhard[((I._base[CS] + I.ip)) >> (8 + 0 + 0)] != ophw)
                        Mame.cpu_setOPbase20((int)(I._base[CS] + I.ip), 0);
                };

                for (i = 0; i < 256; i++)
                {
                    for (j = i, c = 0; j > 0; j >>= 1)
                        if ((j & 1) != 0) c++;
                    parity_table[i] = (c & 1) == 0 ? (byte)1 : (byte)0;
                }

                I.ZeroVal = I.ParityVal = 1;
                I.MF = (1); /* set the mode-flag = native mode */

                for (i = 0; i < 256; i++)
                {
                    Mod_RM.reg.b[i] = (BREGS)reg_name[(i & 0x38) >> 3];
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
                if (reg != null)
                {
                    I = (nec_Regs)reg;
                    I._base[CS] = (uint)(I.sregs[CS] << 4);
                    I._base[DS] = (uint)(I.sregs[DS] << 4);
                    I._base[ES] = (uint)(I.sregs[ES] << 4);
                    I._base[SS] = (uint)(I.sregs[SS] << 4);
                    if (cur_mrhard[((I._base[CS] + I.ip)) >> (8 + 0 + 0)] != ophw)
                        Mame.cpu_setOPbase20((int)(I._base[CS] + I.ip), 0);
                }
            }
            public override void set_irq_callback(irqcallback callback)
            {
                I.irq_callback = callback;
            }
            public override void set_irq_line(int irqline, int state)
            {
                I.irq_state = (sbyte)state;
                if (state == CLEAR_LINE)
                {
                    if (I.IF == 0)
                        I.pending_irq &= unchecked((byte)~INT_IRQ);
                }
                else
                {
                    if (I.IF != 0)
                        I.pending_irq |= INT_IRQ;
                }
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

        }

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
        }
    }
}
