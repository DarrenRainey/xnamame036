using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public partial class cpu_nec
        {
static byte GetMemB(uint Seg,uint Off)
 {
  var v = ((byte)cpu_readmem20((int)(((I.seg_prefix !=0&& (Seg==DS || Seg==SS)) ? I.prefix_base : I._base[Seg])+(Off))));nec_ICount[0]-=6;
   return v;
}
static ushort GetMemW(uint Seg,uint Off)
 {
  nec_ICount[0]-=10;
  return (ushort)(GetMemB(Seg,Off)+(ushort)(GetMemB(Seg,(Off)+1)<<8));
}

static byte ReadByte(uint ea){nec_ICount[0]-=6;return (byte)cpu_readmem20((int)(ea));}
static ushort ReadWord(uint ea){ nec_ICount[0]-=10;return (ushort)(cpu_readmem20((int)(ea))+(cpu_readmem20((int)((ea)+1))<<8));}

static uint EA;
static uint EO; /* HJB 12/13/98 effective offset of the address (before segment is added) */

static uint EA_000() { nec_ICount[0]-=7; EO=(ushort)(I.regs.w[BW]+I.regs.w[IX]); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_001() { nec_ICount[0]-=8; EO=(ushort)(I.regs.w[BW]+I.regs.w[IY]); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_002() { nec_ICount[0]-=8; EO=(ushort)(I.regs.w[BP]+I.regs.w[IX]); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+EO; return EA; }
static uint EA_003() { nec_ICount[0]-=7; EO=(ushort)(I.regs.w[BP]+I.regs.w[IY]); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+EO; return EA; }
static uint EA_004() { nec_ICount[0]-=5; EO=I.regs.w[IX]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_005() { nec_ICount[0]-=5; EO=I.regs.w[IY]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_006() { nec_ICount[0]-=6; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)(((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_007() { nec_ICount[0]-=5; EO=I.regs.w[BW]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }

static uint EA_100() { nec_ICount[0]-=11; EO=(ushort)(I.regs.w[BW]+I.regs.w[IX]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_101() { nec_ICount[0]-=12; EO=(ushort)(I.regs.w[BW]+I.regs.w[IY]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_102() { nec_ICount[0]-=12; EO=(ushort)(I.regs.w[BP]+I.regs.w[IX]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+EO; return EA; }
static uint EA_103() { nec_ICount[0]-=11; EO=(ushort)(I.regs.w[BP]+I.regs.w[IY]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+EO; return EA; }
static uint EA_104() { nec_ICount[0]-=9; EO=(ushort)(I.regs.w[IX]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_105() { nec_ICount[0]-=9; EO=(ushort)(I.regs.w[IY]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }
static uint EA_106() { nec_ICount[0]-=9; EO=(ushort)(I.regs.w[BP]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+EO; return EA; }
static uint EA_107() { nec_ICount[0]-=9; EO=(ushort)(I.regs.w[BW]+(sbyte)((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+EO; return EA; }

static uint EA_200() { nec_ICount[0]-=11; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)( ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=(uint)(I.regs.w[BW]+I.regs.w[IX]); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(ushort)EO; return EA; }
static uint EA_201() { nec_ICount[0]-=12; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)( ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=(uint)(I.regs.w[BW]+I.regs.w[IY]); EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(ushort)EO; return EA; }
static uint EA_202() { nec_ICount[0]-=12; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)( ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=(uint)(I.regs.w[BP]+I.regs.w[IX]); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+(ushort)EO; return EA; }
static uint EA_203() { nec_ICount[0]-=11; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)( ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=(uint)(I.regs.w[BP]+I.regs.w[IY]); EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+(ushort)EO; return EA; }
static uint EA_204() { nec_ICount[0]-=9; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)(((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=I.regs.w[IX]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(ushort)EO; return EA; }
static uint EA_205() { nec_ICount[0]-=9; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)(((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=I.regs.w[IY]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(ushort)EO; return EA; }
static uint EA_206() { nec_ICount[0]-=9; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)(((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=I.regs.w[BP]; EA=((I.seg_prefix !=0&& (SS==DS || SS==SS)) ? I.prefix_base : I._base[SS])+(ushort)EO; return EA; }
static uint EA_207() { nec_ICount[0]-=9; EO=((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); EO+=(uint)(((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))<<8); EO+=I.regs.w[BW]; EA=((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(ushort)EO; return EA; }

delegate uint GetEA_opcode();
static GetEA_opcode[] GetEA={
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




static ushort GetRMWord(uint ModRM)
{
 if (ModRM >= 0xc0)
 return I.regs.w[(int)Mod_RM.RM.w[ModRM]];
 else
  {
   GetEA[ModRM]();
   return ReadWord( EA );
 }
}
static byte GetRMByte(uint ModRM)
{
 if (ModRM >= 0xc0)
  return I.regs.b[(int)Mod_RM.RM.b[ModRM]] ;
 else
  return ReadByte( GetEA[ModRM]() );
}
static void i_add_br8() /* Opcode 0x00 - ADD */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
 nec_ICount[0]-=3;
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
}

static void i_add_wr16() /* Opcode 0x01 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
 nec_ICount[0]-=3;
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
}

static void i_add_r8b() /* Opcode 0x02 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
 nec_ICount[0]-=3;
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
}

static void i_add_r16w() /* Opcode 0x03 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
 nec_ICount[0]-=3;
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
}

static void i_add_ald8() /* Opcode 0x04 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
 nec_ICount[0]-=4;
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 I.regs.b[AL]=(byte)dst;
}

static void i_add_axd16() /* Opcode 0x05 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 nec_ICount[0]-=4;
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 I.regs.w[AW]=(ushort)dst;
}

static void i_push_es() /* Opcode 0x06 */
{
 nec_ICount[0]-=3;
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.sregs[ES])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.sregs[ES])>>8); }; };
}

static void i_pop_es() /* Opcode 0x07 */
{
 { I.sregs[ES] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[ES] = (uint)(I.sregs[ES] << 4);
 nec_ICount[0]-=2;
}

static void i_or_br8() /* Opcode 0x08 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
 nec_ICount[0]-=3;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
}

static void i_or_wr16() /* Opcode 0x09 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
 nec_ICount[0]-=3;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
}

static void i_or_r8b() /* Opcode 0x0a */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
 nec_ICount[0]-=3;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
}

static void i_or_r16w() /* Opcode 0x0b */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
 nec_ICount[0]-=3;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
}

static void i_or_ald8() /* Opcode 0x0c */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
 nec_ICount[0]-=4;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 I.regs.b[AL]=(byte)dst;
}

static void i_or_axd16() /* Opcode 0x0d */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 nec_ICount[0]-=4;
    dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 I.regs.w[AW]=(ushort)dst;
}

static void i_push_cs() /* Opcode 0x0e */
{
 nec_ICount[0]-=3;
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.sregs[CS])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.sregs[CS])>>8); }; };
}
static void i_pre_nec() /* Opcode 0x0f */
{
    uint Opcode = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint ModRM;
    uint tmp;
    uint tmp2;

 switch (Opcode) {
  case 0x10 : // 0F 10 47 30 - TEST1 [bx+30h],cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=3;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-12; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 = (uint)(I.regs.b[CL] & 0x7);
   I.ZeroVal = (tmp & bytes[tmp2])!=0 ? 1 : 0;
//			SetZF(tmp & (1<<tmp2));
   break;
  case 0x11 : // 0F 11 47 30 - TEST1 [bx+30h],cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
      if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=3;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-12; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = (uint)(I.regs.b[CL] & 0xF);
   I.ZeroVal =( tmp & bytes[tmp2])!=0 ? 1 : 0;
//			SetZF(tmp & (1<<tmp2));
   break;


  case 0x12 : // 0F 12 [mod:000:r/m] - CLR1 reg/m8,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      /* need the long if due to correct cycles OB[19.07.99] */
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=5;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte( EA);
       nec_ICount[0]=old-14; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 =(uint)( I.regs.b[CL] & 0x7); /* hey its a Byte so &07 NOT &0f */
   tmp &= (uint)~(bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;

  case 0x13 : // 0F 13 [mod:000:r/m] - CLR1 reg/m16,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
   if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=5;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-14; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = (uint)(I.regs.b[CL] & 0xF); /* this time its a word */
   tmp &= (uint)~(bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;

  case 0x14 : // 0F 14 47 30 - SET1 [bx+30h],cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=4;
      }
      else {
       int old=nec_ICount[0];
        GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-13;
      }
   tmp2 = (uint)(I.regs.b[CL] & 0x7);
   tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  case 0x15 : // 0F 15 C6 - SET1 si,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
     //tmp = GetRMWord(ModRM);
     if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=4;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-13;
   }
   tmp2 = (uint)(I.regs.b[CL] & 0xF);
   tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;
  case 0x16 : // 0F 16 C6 - NOT1 si,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      /* need the long if due to correct cycles OB[19.07.99] */
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=4;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-18; /* my source says 18 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 = (uint)(I.regs.b[CL] & 0x7); /* hey its a Byte so &07 NOT &0f */
   if ((tmp & bytes[tmp2])!=0)
    tmp &= (uint)~(bytes[tmp2]);
   else
    tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  case 0x17 : // 0F 17 C6 - NOT1 si,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
   if (ModRM >= 0xc0) {
    tmp=(uint)(I.regs.w[(int)Mod_RM.RM.w[ModRM]]);
    nec_ICount[0]-=4;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-18; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = (uint)(I.regs.b[CL] & 0xF); /* this time its a word */
   if ((tmp & bytes[tmp2])!=0)
    tmp &= (uint)~(bytes[tmp2]);
   else
    tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;
  case 0x18 : // 0F 18 XX - TEST1 [bx+30h],07
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
     //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=4;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-13; /* my source says 15 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0xF;
   I.ZeroVal =( tmp & (bytes[tmp2]))!=0 ? 1 : 0;
//			SetZF(tmp & (1<<tmp2));
   break;
  case 0x19 : // 0F 19 XX - TEST1 [bx+30h],07
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
      if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=4;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-13; /* my source says 14 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0xf;
   I.ZeroVal = (tmp & (bytes[tmp2]) )!=0? 1 : 0;
//			SetZF(tmp & (1<<tmp2));
   break;
  case 0x1a : // 0F 1A 06 - CLR1 si,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=6;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-15; /* my source says 15 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0x7;
   tmp &= (uint)~(bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  case 0x1B : // 0F 1B 06 - CLR1 si,cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
      if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=6;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-15; /* my source says 15 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0xF;
   tmp &=(uint) ~(bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;
  case 0x1C : // 0F 1C 47 30 - SET1 [bx+30h],cl
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
     //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=5;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-14; /* my source says 15 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
      }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0x7;
   tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  case 0x1D : // 0F 1D C6 - SET1 si,cl
   //if (errorlog) fprintf(errorlog,"PC=%06x : Set1 ",cpu_get_pc()-2);
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=5;
    //if (errorlog) fprintf(errorlog,"reg=%04x ->",tmp);
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM](); // calculate EA
    tmp=ReadWord(EA); // read from EA
    nec_ICount[0]=old-14;
    //if (errorlog) fprintf(errorlog,"[%04x]=%04x ->",EA,tmp);
   }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0xF;
   tmp |= (bytes[tmp2]);
   //if (errorlog) fprintf(errorlog,"%04x",tmp);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;
  case 0x1e : // 0F 1e C6 - NOT1 si,07
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=5;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-19;
      }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0x7;
   if ((tmp & bytes[tmp2])!=0)
    tmp &=(uint) ~(bytes[tmp2]);
   else
    tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  case 0x1f : // 0F 1f C6 - NOT1 si,07
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMWord(ModRM);
      if (ModRM >= 0xc0) {
    tmp=I.regs.w[(int)Mod_RM.RM.w[ModRM]];
    nec_ICount[0]-=5;
   }
   else {
    int old=nec_ICount[0];
    GetEA[ModRM]();
    tmp=ReadWord(EA);
    nec_ICount[0]=old-19; /* my source says 15 cycles everytime and not

   											   ModRM-dependent like GetEA[] does..hmmm */
   }
   tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   tmp2 &= 0xF;
   if ((tmp & bytes[tmp2])!=0)
    tmp &= (uint)~(bytes[tmp2]);
   else
    tmp |= (bytes[tmp2]);
   { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
   break;
  case 0x20 : { // 0F 20 59 - add4s
   int count = (I.regs.b[CL]+1)/2; // length in words !
   int i;
        uint di = I.regs.w[IY];
   uint si = I.regs.w[IX];
   I.ZeroVal = 0;
   I.CarryVal = 0; // NOT ADC
   for (i=0;i<count;i++) {
    int v1,v2;
    int result;
    tmp = GetMemB(DS, si);
    tmp2 = GetMemB(ES, di);

    v1 = (int)((tmp>>4)*10 + (tmp&0xf));
    v2 = (int)((tmp2>>4)*10 + (tmp2&0xf));
    result = v1+v2+I.CarryVal;
    I.CarryVal = result > 99 ? 1 : 0;
    result = result % 100;
    v1 = ((result/10)<<4) | (result % 10);
    { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(di)),(int)(v1)); }
    if (v1!=0) I.ZeroVal = 1;
    si++;
    di++;
   }
   I.OverVal = I.CarryVal;
   nec_ICount[0]-=7+19*count; // 7+19n, n #operand words
   } break;

  case 0x22 : { // 0F 22 59 - sub4s
   int count = (I.regs.b[CL]+1)/2;
   int i;
       uint di = I.regs.w[IY];
   uint si = I.regs.w[IX];
   I.ZeroVal = 0;
   I.CarryVal = 0; // NOT ADC
   for (i=0;i<count;i++) {
    int v1,v2;
    int result;
    tmp = GetMemB(ES, di);
    tmp2 = GetMemB(DS, si);

    v1 = (int)((tmp>>4)*10 + (tmp&0xf));
    v2 = (int)((tmp2>>4)*10 + (tmp2&0xf));
    if (v1 < (v2+I.CarryVal)) {
     v1+=100;
     result = v1-(v2+I.CarryVal);
     I.CarryVal = 1;
    } else {
     result = v1-(v2+I.CarryVal);
     I.CarryVal = 0;
    }
    v1 = ((result/10)<<4) | (result % 10);
    { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(di)),(int)(v1)); }
    if (v1!=0) I.ZeroVal = 1;
    si++;
    di++;
   }
   I.OverVal = I.CarryVal;
   nec_ICount[0]-=7+19*count;
   } break;

  case 0x25 :

   //if (errorlog) fprintf(errorlog,"PC=%06x : MOVSPA\n",cpu_get_pc()-2);
   nec_ICount[0]-=16;
   break;
  case 0x26 : { // 0F 22 59 - cmp4s
   int count = (I.regs.b[CL]+1)/2;
   int i;
         uint di = I.regs.w[IY];
   uint si = I.regs.w[IX];
   I.ZeroVal = 0;
   I.CarryVal = 0; // NOT ADC
   for (i=0;i<count;i++) {
    int v1,v2;
    int result;
    tmp = GetMemB(ES, di);
    tmp2 = GetMemB(DS, si);

    v1 = (int)((tmp>>4)*10 + (tmp&0xf));
    v2 = (int)((tmp2>>4)*10 + (tmp2&0xf));
    if (v1 < (v2+I.CarryVal)) {
     v1+=100;
     result = v1-(v2+I.CarryVal);
     I.CarryVal = 1;
    } else {
     result = v1-(v2+I.CarryVal);
     I.CarryVal = 0;
    }
    v1 = ((result/10)<<4) | (result % 10);
//				PutMemB(ES, di,v1)	/* no store, only compare */
    if (v1!=0) I.ZeroVal = 1;
    si++;
    di++;
   }
   I.OverVal = I.CarryVal;
   nec_ICount[0]-=7+19*count; // 7+19n, n #operand bytes
   } break;
  case 0x28 : // 0F 28 C7 - ROL4 bh
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=25;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-28;
      }
   tmp <<= 4;
   tmp |= (uint)(I.regs.b[AL] & 0xF);
   I.regs.b[AL] = (byte)((I.regs.b[AL] & 0xF0) | ((tmp>>8)&0xF));
   tmp &= 0xff;
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;
  // Is this a REAL instruction??
  case 0x29 : // 0F 29 C7 - ROL4 bx

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

   //if (errorlog) fprintf(errorlog,"PC=%06x : ROL4 %02x\n",cpu_get_pc()-3,ModRM);
   break;

  case 0x2A : // 0F 2a c2 - ROR4 bh
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
      //tmp = GetRMByte(ModRM);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
       nec_ICount[0]-=29;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       nec_ICount[0]=old-33;
      }
   tmp2 = (uint)((I.regs.b[AL] & 0xF)<<4);
   I.regs.b[AL] =(byte)( (I.regs.b[AL] & 0xF0) | (tmp&0xF));
   tmp = tmp2 | (tmp>>4);
   { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp); }; };
   break;

  case 0x2B : // 0F 2b c2 - ROR4 bx
   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

   //if (errorlog) fprintf(errorlog,"PC=%06x : ROR4 %02x\n",cpu_get_pc()-3,ModRM);
   break;
  case 0x2D : // 0Fh 2Dh <1111 1RRR>

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : BRKCS %02x\n",cpu_get_pc()-3,ModRM);
   nec_ICount[0]-=15;// checked !
   break;

  case 0x31: // 0F 31 [mod:reg:r/m] - INS reg8,reg8 or INS reg8,imm4

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : INS ",cpu_get_pc()-2);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
          //if (errorlog) fprintf(errorlog,"ModRM=%04x \n",ModRM);
       nec_ICount[0]-=29;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       //if (errorlog) fprintf(errorlog,"ModRM=%04x  Byte=%04x\n",EA,tmp);
       nec_ICount[0]=old-33;
      }

   break;
  case 0x33: // 0F 33 [mod:reg:r/m] - EXT reg8,reg8 or EXT reg8,imm4

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : EXT ",cpu_get_pc()-2);
      if (ModRM >= 0xc0) {
       tmp=I.regs.b[(int)Mod_RM.RM.b[ModRM]];
          //if (errorlog) fprintf(errorlog,"ModRM=%04x \n",ModRM);
       nec_ICount[0]-=29;
      }
      else {
       int old=nec_ICount[0];
       GetEA[ModRM]();
       tmp=ReadByte(EA);
       //if (errorlog) fprintf(errorlog,"ModRM=%04x  Byte=%04x\n",EA,tmp);
       nec_ICount[0]=old-33;
      }
   //NEC_ICount[0]-=26; /* 26 -55 clocks ....*/
   break;
  case 0x91:

   //if (errorlog) fprintf(errorlog,"PC=%06x : RETRBI\n",cpu_get_pc()-2);
   nec_ICount[0]-=12;
   break;

  case 0x94:

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

   //if (errorlog) fprintf(errorlog,"PC=%06x : TSCSW %02x\n",cpu_get_pc()-3,ModRM);
   nec_ICount[0]-=11;
   break;
  case 0x95:

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : MOVSPB %02x\n",cpu_get_pc()-3,ModRM);
   nec_ICount[0]-=11;
   break;
  case 0xbe:

   //if (errorlog) fprintf(errorlog,"PC=%06x : STOP\n",cpu_get_pc()-2);
   nec_ICount[0]-=2; /* of course this is crap */
   break;
  case 0xe0:


   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : BRKXA %02x\n",cpu_get_pc()-3,ModRM);
   nec_ICount[0]-=12;
   break;
  case 0xf0:

   ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   //if (errorlog) fprintf(errorlog,"PC=%06x : RETXA %02x\n",cpu_get_pc()-3,ModRM);
   nec_ICount[0]-=12;
   break;
  case 0xff: /* 0F ff imm8 - BRKEM */

   ModRM=((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
   nec_ICount[0]-=38;
   nec_interrupt(ModRM,true);
   break;
  default :
   break;
 }
}



static void i_adc_br8() /* Opcode 0x10 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_adc_wr16() /* Opcode 0x11 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
}

static void i_adc_r8b() /* Opcode 0x12 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_adc_r16w() /* Opcode 0x13 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
 nec_ICount[0]-=3;
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_adc_ald8() /* Opcode 0x14 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 I.regs.b[AL] = (byte)dst;
 nec_ICount[0]-=4;
}
static void i_adc_axd16() /* Opcode 0x15 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 I.regs.w[AW]=(ushort)dst;
 nec_ICount[0]-=4;
}

static void i_push_ss() /* Opcode 0x16 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.sregs[SS])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.sregs[SS])>>8); }; };
 nec_ICount[0]-=10; /* OPCODE.LST says 8-12...so 10 */
}

static void i_pop_ss() /* Opcode 0x17 */
{
 { I.sregs[SS] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[SS] = (uint)(I.sregs[SS] << 4);
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))](); /* no interrupt before next instruction */
 nec_ICount[0]-=10; /* OPCODE.LST says 8-12...so 10 */
}

static void i_sbb_br8() /* Opcode 0x18 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    src+=(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_sbb_wr16() /* Opcode 0x19 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    src+=(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
}

static void i_sbb_r8b() /* Opcode 0x1a */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
    src+=(uint)(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_sbb_r16w() /* Opcode 0x1b */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    src+=(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]= (ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_sbb_ald8() /* Opcode 0x1c */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    src+=(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 I.regs.b[AL] =(byte) dst;
 nec_ICount[0]-=4;
}

static void i_sbb_axd16() /* Opcode 0x1d */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    src+=(I.CarryVal!=0?(byte)1:(byte)0);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 I.regs.w[AW]=(ushort)dst;
 nec_ICount[0]-=4;
}

static void i_push_ds() /* Opcode 0x1e */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.sregs[DS])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.sregs[DS])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_pop_ds() /* Opcode 0x1f */
{
 { I.sregs[DS] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[DS] =(uint) (I.sregs[DS] << 4);
 nec_ICount[0]-=10;
}

static void i_and_br8() /* Opcode 0x20 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_and_wr16() /* Opcode 0x21 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
}

static void i_and_r8b() /* Opcode 0x22 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_and_r16w() /* Opcode 0x23 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_and_ald8() /* Opcode 0x24 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 I.regs.b[AL] = (byte)dst;
 nec_ICount[0]-=4;
}

static void i_and_axd16() /* Opcode 0x25 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 I.regs.w[AW]=(ushort)dst;
 nec_ICount[0]-=4;
}

static void i_es() /* Opcode 0x26 */
{
    I.seg_prefix=1;
 I.prefix_base=I._base[ES];
 nec_ICount[0]-=2;
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))]();
}

static void i_daa() /* Opcode 0x27 */
{
 if ((I.AuxVal!=0?(byte)1:(byte)0)!=0 || ((I.regs.b[AL] & 0xf) > 9))
 {
  int tmp;
  I.regs.b[AL] = (byte)(tmp = I.regs.b[AL] + 6);
  I.AuxVal = 1;
  I.CarryVal |= tmp & 0x100;
 }

 if ((I.CarryVal!=0?(byte)1:(byte)0)!=0 || (I.regs.b[AL] > 0x9f))
 {
  I.regs.b[AL] += 0x60;
  I.CarryVal = 1;
 }

 I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(I.regs.b[AL]);
 nec_ICount[0]-=3;
}


static void i_sub_br8() /* Opcode 0x28 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
 { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;

}

static void i_sub_wr16() /* Opcode 0x29 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
}

static void i_sub_r8b() /* Opcode 0x2a */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_sub_r16w() /* Opcode 0x2b */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_sub_ald8() /* Opcode 0x2c */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 I.regs.b[AL] = (byte)dst;
 nec_ICount[0]-=4;
}

static void i_sub_axd16() /* Opcode 0x2d */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 I.regs.w[AW]=(ushort)dst;
 nec_ICount[0]-=4;
}

static void i_cs() /* Opcode 0x2e */
{
    I.seg_prefix=1;
 I.prefix_base=I._base[CS];
 nec_ICount[0]-=2;
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))]();
}

static void i_das() /* Opcode 0x2f */
{
 if ((I.AuxVal!=0?(byte)1:(byte)0) !=0|| ((I.regs.b[AL] & 0xf) > 9))
 {
  int tmp;
  I.regs.b[AL] = (byte)(tmp = I.regs.b[AL] - 6);
  I.AuxVal = 1;
  I.CarryVal |= tmp & 0x100;
 }

 if ((I.CarryVal!=0?(byte)1:(byte)0) !=0|| (I.regs.b[AL] > 0x9f))
 {
  I.regs.b[AL] -= 0x60;
  I.CarryVal = 1;
 }

 I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(I.regs.b[AL]);
 nec_ICount[0]-=7;
}

static void i_xor_br8() /* Opcode 0x30 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_xor_wr16() /* Opcode 0x31 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
}

static void i_xor_r8b() /* Opcode 0x32 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
 dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_xor_r16w() /* Opcode 0x33 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_xor_ald8() /* Opcode 0x34 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 I.regs.b[AL] = (byte)dst;
 nec_ICount[0]-=4;
}

static void i_xor_axd16() /* Opcode 0x35 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 I.regs.w[AW]=(ushort)dst;
 nec_ICount[0]-=4;
}

static void i_ss() /* Opcode 0x36 */
{
    I.seg_prefix=1;
 I.prefix_base=I._base[SS];
 nec_ICount[0]-=2;
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))]();
}

static void i_aaa() /* Opcode 0x37 */
{
 if ((I.AuxVal!=0?(byte)1:(byte)0)!=0 || ((I.regs.b[AL] & 0xf) > 9))
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
 nec_ICount[0]-=3;
}

static void i_cmp_br8() /* Opcode 0x38 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_cmp_wr16() /* Opcode 0x39 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_cmp_r8b() /* Opcode 0x3a */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint src = GetRMByte(ModRM);
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_cmp_r16w() /* Opcode 0x3b */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_cmp_ald8() /* Opcode 0x3c */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
    nec_ICount[0]-=4;
}

static void i_cmp_axd16() /* Opcode 0x3d */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
    nec_ICount[0]-=4;
}

static void i_ds() /* Opcode 0x3e */
{
    I.seg_prefix=1;
 I.prefix_base=I._base[DS];
 nec_ICount[0]-=2;
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))]();
}

static void i_aas() /* Opcode 0x3f */
{
 if ((I.AuxVal!=0?(byte)1:(byte)0) !=0|| ((I.regs.b[AL] & 0xf) > 9))
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
 nec_ICount[0]-=3;
}
static void i_inc_ax() /* Opcode 0x40 */
{
    { uint tmp = (uint)I.regs.w[AW]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[AW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_cx() /* Opcode 0x41 */
{
    { uint tmp = (uint)I.regs.w[CW]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[CW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_dx() /* Opcode 0x42 */
{
    { uint tmp = (uint)I.regs.w[DW]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[DW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_bx() /* Opcode 0x43 */
{
    { uint tmp = (uint)I.regs.w[BW]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[BW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_sp() /* Opcode 0x44 */
{
    { uint tmp = (uint)I.regs.w[SP]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[SP]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_bp() /* Opcode 0x45 */
{
    { uint tmp = (uint)I.regs.w[BP]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[BP]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_si() /* Opcode 0x46 */
{
    { uint tmp = (uint)I.regs.w[IX]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[IX]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_inc_di() /* Opcode 0x47 */
{
    { uint tmp = (uint)I.regs.w[IY]; uint tmp1 = tmp+1; I.OverVal = (tmp == 0x7fff)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[IY]=(ushort)tmp1; nec_ICount[0]-=2; };
}
static void i_dec_ax() /* Opcode 0x48 */
{
    { uint tmp = (uint)I.regs.w[AW]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[AW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_cx() /* Opcode 0x49 */
{
    { uint tmp = (uint)I.regs.w[CW]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[CW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_dx() /* Opcode 0x4a */
{
    { uint tmp = (uint)I.regs.w[DW]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[DW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_bx() /* Opcode 0x4b */
{
    { uint tmp = (uint)I.regs.w[BW]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[BW]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_sp() /* Opcode 0x4c */
{
    { uint tmp = (uint)I.regs.w[SP]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[SP]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_bp() /* Opcode 0x4d */
{
    { uint tmp = (uint)I.regs.w[BP]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[BP]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_si() /* Opcode 0x4e */
{
    { uint tmp = (uint)I.regs.w[IX]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[IX]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_dec_di() /* Opcode 0x4f */
{
    { uint tmp = (uint)I.regs.w[IY]; uint tmp1 = tmp-1; I.OverVal = (tmp == 0x8000)?1:0; I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1); I.regs.w[IY]=(ushort)tmp1; nec_ICount[0]-=2; };
}

static void i_push_ax() /* Opcode 0x50 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[AW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[AW])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_cx() /* Opcode 0x51 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[CW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[CW])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_dx() /* Opcode 0x52 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[DW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[DW])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_bx() /* Opcode 0x53 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BW])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_sp() /* Opcode 0x54 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[SP])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[SP])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_bp() /* Opcode 0x55 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BP])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BP])>>8); }; };
 nec_ICount[0]-=10;
}


static void i_push_si() /* Opcode 0x56 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[IX])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[IX])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_push_di() /* Opcode 0x57 */
{
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[IY])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[IY])>>8); }; };
 nec_ICount[0]-=10;
}

static void i_pop_ax() /* Opcode 0x58 */
{
 { I.regs.w[AW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_cx() /* Opcode 0x59 */
{
 { I.regs.w[CW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_dx() /* Opcode 0x5a */
{
 { I.regs.w[DW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_bx() /* Opcode 0x5b */
{
 { I.regs.w[BW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_sp() /* Opcode 0x5c */
{
 { I.regs.w[SP] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_bp() /* Opcode 0x5d */
{
 { I.regs.w[BP] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_si() /* Opcode 0x5e */
{
 { I.regs.w[IX] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pop_di() /* Opcode 0x5f */
{
 { I.regs.w[IY] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=10;
}

static void i_pusha() /* Opcode 0x60 */
{
 uint tmp=I.regs.w[SP];
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[AW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[AW])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[CW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[CW])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[DW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[DW])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BW])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BW])>>8); }; };
    { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(tmp)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(tmp)>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BP])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BP])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[IX])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[IX])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[IY])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[IY])>>8); }; };
 nec_ICount[0]-=51;
}

static void i_popa() /* Opcode 0x61 */
{
    uint tmp;
 { I.regs.w[IY] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[IX] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[BP] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
    { tmp = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[BW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[DW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[CW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.regs.w[AW] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=59;
}

static void i_bound() /* Opcode 0x62  BOUND or CHKIND (on NEC)*/
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    int low = (short)GetRMWord(ModRM);
    int high= (short)ReadWord(EA+2);
    int tmp= (short)I.regs.w[(int)Mod_RM.reg.w[ModRM]];
    if (tmp<low || tmp>high) {
  /* OB: on NECs CS:IP points to instruction

		       FOLLOWING the BOUND instruction ! */
  // I.ip-=2;
  nec_interrupt(5,false);
    }
  nec_ICount[0]-=20;
}

static void i_brkn() /* Opcode 0x63 BRKN -  Break to Native Mode */
{

 uint int_vector;
 int_vector = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 //if (errorlog) fprintf(errorlog,"PC=%06x : BRKN %02x\n",cpu_get_pc()-2,int_vector);
}
static void repc(int flagval)
{
    /* Handles repc- and repnc- prefixes. flagval is the value of ZF for the

       loop  to continue for CMPS and SCAS instructions. */
 uint next = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)));
 uint count = I.regs.w[CW];

    switch(next)
    {
    case 0x26: /* ES: */
        I.seg_prefix=1;
  I.prefix_base=I._base[ES];
  nec_ICount[0]-=2;
  repc(flagval);
  break;
    case 0x2e: /* CS: */
        I.seg_prefix=1;
  I.prefix_base=I._base[CS];
  nec_ICount[0]-=2;
  repc(flagval);
  break;
    case 0x36: /* SS: */
        I.seg_prefix=1;
  I.prefix_base=I._base[SS];
  nec_ICount[0]-=2;
  repc(flagval);
  break;
    case 0x3e: /* DS: */
        I.seg_prefix=1;
  I.prefix_base=I._base[DS];
  nec_ICount[0]-=2;
  repc(flagval);
  break;
    case 0x6c: /* REP INSB */
  nec_ICount[0]-=(int)(9-count);
  for (; ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
           i_insb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0x6d: /* REP INSW */
  nec_ICount[0]-=(int)(9-count);
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
           i_insw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0x6e: /* REP OUTSB */
  nec_ICount[0]-=(int)(9-count);
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
            i_outsb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0x6f: /* REP OUTSW */
  nec_ICount[0]-=(int)(9-count);
  for (; ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
            i_outsw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xa4: /* REP MOVSB */
  nec_ICount[0]-=(int)(9-count);
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_movsb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xa5: /* REP MOVSW */
  nec_ICount[0]-=(int)(9-count);
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_movsw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xa6: /* REP(N)E CMPSB */
  nec_ICount[0]-=9;
  for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_cmpsb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xa7: /* REP(N)E CMPSW */
  nec_ICount[0]-=9;
  for (I.ZeroVal =flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_cmpsw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xaa: /* REP STOSB */
  nec_ICount[0]-=(int)(9-count);
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_stosb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xab: /* REP STOSW */
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_stosw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xac: /* REP LODSB */
  nec_ICount[0]-=9;
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_lodsb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xad: /* REP LODSW */
  nec_ICount[0]-=9;
  for (;((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_lodsw();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xae: /* REP(N)E SCASB */
  nec_ICount[0]-=9;
  for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_scasb();
  I.regs.w[CW]=(ushort)count;
  break;
    case 0xaf: /* REP(N)E SCASW */
  nec_ICount[0]-=9;
  for (I.ZeroVal =flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && ((I.CarryVal!=0?(byte)1:(byte)0)==flagval)&&(count > 0); count--)
   i_scasw();
  I.regs.w[CW]=(ushort)count;
  break;
    default:
  nec_instruction[next]();
  break;
    }
}

static void i_repnc() /* Opcode 0x64 */
{
    repc(0);
}

static void i_repc() /* Opcode 0x65 */
{
    repc(1);
}

static void i_push_d16() /* Opcode 0x68 */
{
    uint tmp = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    tmp += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(tmp)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(tmp)>>8); }; };
    nec_ICount[0]-=12;
}

static void i_imul_d16() /* Opcode 0x69 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    uint src2=((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    src2+=(uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)))<<8);
    dst = (uint)((int)((short)src)*(int)((short)src2));
 I.CarryVal = I.OverVal = (((int)dst) >> 15 != 0) && (((int)dst) >> 15 != -1)?1:0;
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?38:47;
}

static void i_push_d8() /* Opcode 0x6a */
{
    uint tmp = (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)))));
    { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(tmp)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(tmp)>>8); }; };
    nec_ICount[0]-=7;
}

static void i_imul_d8() /* Opcode 0x6b */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint src = GetRMWord(ModRM);
    uint src2= (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)))));
    dst = (uint)((int)((short)src)*(int)((short)src2));
 I.CarryVal = I.OverVal = (((int)dst) >> 15 != 0) && (((int)dst) >> 15 != -1)?1:0;
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    nec_ICount[0]-=(ModRM >=0xc0 )?31:39;
}

static void i_insb() /* Opcode 0x6c */
{
 nec_ICount[0]-=5;
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)(cpu_readport(I.regs.w[DW]))); };
 I.regs.w[IY]+= (ushort)(-2 * I.DF + 1);
}

static void i_insw() /* Opcode 0x6d */
{
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)(cpu_readport(I.regs.w[DW]))); };
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY]+1)),(int)(cpu_readport(I.regs.w[DW]+1))); };
//if (errorlog) fprintf(errorlog,"%04x:  insw\n",cpu_get_pc());
 I.regs.w[IY]+= (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=8;
}

static void i_outsb() /* Opcode 0x6e */
{
 cpu_writeport(I.regs.w[DW],GetMemB(DS,I.regs.w[IX]));
 I.regs.w[IY]+= (ushort)(-2 * I.DF + 1);
 nec_ICount[0]-=8;
}

static void i_outsw() /* Opcode 0x6f */
{
 cpu_writeport(I.regs.w[DW],GetMemB(DS,I.regs.w[IX]));
 cpu_writeport(I.regs.w[DW]+1,GetMemB(DS,(uint)(I.regs.w[IX]+1)));
 I.regs.w[IY]+= (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=8;
}

static void i_jo() /* Opcode 0x70 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.OverVal!=0?(byte)1:(byte)0)!=0)
 {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jno() /* Opcode 0x71 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.OverVal!=0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jb() /* Opcode 0x72 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.CarryVal!=0?(byte)1:(byte)0)!=0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnb() /* Opcode 0x73 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.CarryVal!=0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jz() /* Opcode 0x74 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.ZeroVal==0?(byte)1:(byte)0)!=0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnz() /* Opcode 0x75 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if ((I.ZeroVal==0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jbe() /* Opcode 0x76 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if ((I.CarryVal!=0?(byte)1:(byte)0)!=0 || (I.ZeroVal==0?(byte)1:(byte)0)!=0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnbe() /* Opcode 0x77 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if (!((I.CarryVal!=0?(byte)1:(byte)0) !=0|| (I.ZeroVal==0?(byte)1:(byte)0)!=0)) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_js() /* Opcode 0x78 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if ((I.SignVal<0?(byte)1:(byte)0)!=0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jns() /* Opcode 0x79 */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if ((I.SignVal<0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jp() /* Opcode 0x7a */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if (parity_table[(byte)I.ParityVal]!=0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnp() /* Opcode 0x7b */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if (parity_table[(byte)I.ParityVal]==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jl() /* Opcode 0x7c */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if (((I.SignVal<0?(byte)1:(byte)0)!=(I.OverVal!=0?(byte)1:(byte)0))&&(I.ZeroVal==0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnl() /* Opcode 0x7d */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if ((I.ZeroVal==0?(byte)1:(byte)0)!=0||((I.SignVal<0?(byte)1:(byte)0)==(I.OverVal!=0?(byte)1:(byte)0))) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jle() /* Opcode 0x7e */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if ((I.ZeroVal==0?(byte)1:(byte)0)!=0||((I.SignVal<0?(byte)1:(byte)0)!=(I.OverVal!=0?(byte)1:(byte)0))) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_jnle() /* Opcode 0x7f */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    if (((I.SignVal<0?(byte)1:(byte)0)==(I.OverVal!=0?(byte)1:(byte)0))&&(I.ZeroVal==0?(byte)1:(byte)0)==0) {
  I.ip = (ushort)(I.ip+tmp);
  nec_ICount[0]-=14;
  change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=4;
}

static void i_80pre() /* Opcode 0x80 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint dst = GetRMByte(ModRM);
    uint src = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=(ModRM >=0xc0 )?4:18;

    switch (ModRM & 0x38)
    {
    case 0x00: /* ADD eb,d8 */
        { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x08: /* OR eb,d8 */
        dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x10: /* ADC eb,d8 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
        { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x18: /* SBB eb,b8 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
        { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x20: /* AND eb,d8 */
        dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x28: /* SUB eb,d8 */
        { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x30: /* XOR eb,d8 */
        dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
    case 0x38: /* CMP eb,d8 */
        { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 break;
    }
}

static void i_81pre() /* Opcode 0x81 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint dst = GetRMWord(ModRM);
    uint src = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    src+= (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 nec_ICount[0]-=(ModRM >=0xc0 )?4:26;

    switch (ModRM & 0x38)
    {
    case 0x00: /* ADD ew,d16 */
        { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x08: /* OR ew,d16 */
        dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x10: /* ADC ew,d16 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
  { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x18: /* SBB ew,d16 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x20: /* AND ew,d16 */
        dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x28: /* SUB ew,d16 */
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x30: /* XOR ew,d16 */
        dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x38: /* CMP ew,d16 */
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 break;
    }
}

static void i_82pre() /* Opcode 0x82 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 uint dst = GetRMByte(ModRM);
 uint src = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=(ModRM >=0xc0 )?4:18;

    switch (ModRM & 0x38)
    {
 case 0x00: /* ADD eb,d8 */
  { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x08: /* OR eb,d8 */
  dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x10: /* ADC eb,d8 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
  { uint res=dst+src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x18: /* SBB eb,d8 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
  { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x20: /* AND eb,d8 */
  dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x28: /* SUB eb,d8 */
  { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x30: /* XOR eb,d8 */
  dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 break;
 case 0x38: /* CMP eb,d8 */
  { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 break;
    }
}

static void i_83pre() /* Opcode 0x83 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint dst = GetRMWord(ModRM);
    uint src = (ushort)((short)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)))));
 nec_ICount[0]-=(ModRM >=0xc0 )?4:26;

    switch (ModRM & 0x38)
    {
    case 0x00: /* ADD ew,d16 */
        { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x08: /* OR ew,d16 */
        dst|=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x10: /* ADC ew,d16 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
        { uint res=dst+src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((res) ^ (src)) & ((res) ^ (dst)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x18: /* SBB ew,d16 */
        src+=(I.CarryVal!=0?(byte)1:(byte)0);
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x20: /* AND ew,d16 */
        dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x28: /* SUB ew,d16 */
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x30: /* XOR ew,d16 */
        dst^=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    case 0x38: /* CMP ew,d16 */
        { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 break;
    }
}

static void i_test_br8() /* Opcode 0x84 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    nec_ICount[0]-=(ModRM >=0xc0 )?2:10;
}

static void i_test_wr16() /* Opcode 0x85 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    nec_ICount[0]-=(ModRM >=0xc0 )?2:14;
}

static void i_xchg_br8() /* Opcode 0x86 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.b[(int)Mod_RM.reg.b[ModRM]]; uint dst = GetRMByte(ModRM);
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=(byte)dst;
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)src; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)src); }; };
    // V30
    if (ModRM >= 0xc0) nec_ICount[0]-=3;
    else nec_ICount[0]-=(EO & 1 )!=0?24:16;
}

static void i_xchg_wr16() /* Opcode 0x87 */
{
    uint ModRM = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint src = I.regs.w[(int)Mod_RM.reg.w[ModRM]]; uint dst = GetRMWord(ModRM);
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)dst;
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)src; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(src)); cpu_writemem20((int)((EA)+1),(int)(src)>>8); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?3:24;
}

static void i_mov_br8() /* Opcode 0x88 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    byte src = I.regs.b[(int)Mod_RM.reg.b[ModRM]];
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)src; else { nec_ICount[0]-=7; cpu_writemem20((int)(GetEA[ModRM]()),(int)src); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:9;
}

static void i_mov_wr16() /* Opcode 0x89 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort src = I.regs.w[(int)Mod_RM.reg.w[ModRM]];
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)src; else { GetEA[ModRM](); { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(src)); cpu_writemem20((int)((EA)+1),(int)(src)>>8); }; } };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:13;
}

static void i_mov_r8b() /* Opcode 0x8a */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    byte src = GetRMByte(ModRM);
    I.regs.b[(int)Mod_RM.reg.b[ModRM]]=src;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:11;
}

static void i_mov_r16w() /* Opcode 0x8b */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort src = GetRMWord(ModRM);
    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=src;
    nec_ICount[0]-=(ModRM >=0xc0 )?2:15;
}

static void i_mov_wsreg() /* Opcode 0x8c */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 if ((ModRM & 0x20)!=0) return; /* HJB 12/13/98 1xx is invalid */
 { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)I.sregs[(ModRM & 0x38) >> 3]; else { GetEA[ModRM](); { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(I.sregs[(ModRM & 0x38) >> 3])); cpu_writemem20((int)((EA)+1),(int)(I.sregs[(ModRM & 0x38) >> 3])>>8); }; } };
 nec_ICount[0]-=(ModRM >=0xc0 )?2:12;
}

static void i_lea() /* Opcode 0x8d */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 GetEA[ModRM]();
 I.regs.w[(int)Mod_RM.reg.w[ModRM]]=(ushort)EO; /* HJB 12/13/98 effective offset (no segment part) */
 nec_ICount[0]-=4;
}

static void i_mov_sregw() /* Opcode 0x8e */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort src = GetRMWord(ModRM);
 nec_ICount[0]-=(ModRM >=0xc0 )?2:13;
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
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))]();
 break;
    case 0x08: /* mov cs,ew */
 break; /* doesn't do a jump far */
    }
}

static void i_popw() /* Opcode 0x8f */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort tmp;
    { tmp = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
    { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { GetEA[ModRM](); { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; } };
    nec_ICount[0]-=21;
}
static void i_nop() /* Opcode 0x90 */
{
    /* this is XchgAWReg(AW); */
 nec_ICount[0]-=2;
}

static void i_xchg_axcx() /* Opcode 0x91 */
{
    { ushort tmp; tmp = I.regs.w[CW]; I.regs.w[CW] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axdx() /* Opcode 0x92 */
{
    { ushort tmp; tmp = I.regs.w[DW]; I.regs.w[DW] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axbx() /* Opcode 0x93 */
{
    { ushort tmp; tmp = I.regs.w[BW]; I.regs.w[BW] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axsp() /* Opcode 0x94 */
{
    { ushort tmp; tmp = I.regs.w[SP]; I.regs.w[SP] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axbp() /* Opcode 0x95 */
{
    { ushort tmp; tmp = I.regs.w[BP]; I.regs.w[BP] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axsi() /* Opcode 0x96 */
{
    { ushort tmp; tmp = I.regs.w[IX]; I.regs.w[IX] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_xchg_axdi() /* Opcode 0x97 */
{
    { ushort tmp; tmp = I.regs.w[IY]; I.regs.w[IY] = I.regs.w[AW]; I.regs.w[AW] = tmp; nec_ICount[0]-=3; };
}

static void i_cbw() /* Opcode 0x98 */
{
 nec_ICount[0]-=2;
 I.regs.b[AH] = (I.regs.b[AL] & 0x80)!=0 ?(byte) 0xff :(byte) 0;
}

static void i_cwd() /* Opcode 0x99 */
{
 nec_ICount[0]-=5;
 I.regs.w[DW] = (I.regs.b[AH] & 0x80) !=0? (ushort)0xffff :(ushort) 0;
}

static void i_call_far()
{
    uint tmp, tmp2;

 tmp = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp +=(uint)( ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp2 += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.sregs[CS])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.sregs[CS])>>8); }; };
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.ip)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.ip)>>8); }; };

 I.ip = (ushort)tmp;
 I.sregs[CS] = (ushort)tmp2;
 I._base[CS] = (uint)(I.sregs[CS] << 4);
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=39;
}

static void i_wait() /* Opcode 0x9b */
{
 nec_ICount[0]-=7; /* 2+5n (n = number of times POLL pin sampled) */
}
static void i_pushf() /* Opcode 0x9c */
{
    { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)((ushort)((I.CarryVal!=0?(byte)1:(byte)0) | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal!=0?(byte)1:(byte)0) << 4) | ((I.ZeroVal==0?(byte)1:(byte)0) << 6) | ((I.SignVal<0?(byte)1:(byte)0) << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal!=0?(byte)1:(byte)0) << 11)| ((I.MF!=0?(byte)1:(byte)0) << 15)) | 0xf000)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)((ushort)((I.CarryVal!=0?(byte)1:(byte)0) | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal!=0?(byte)1:(byte)0) << 4) | ((I.ZeroVal==0?(byte)1:(byte)0) << 6) | ((I.SignVal<0?(byte)1:(byte)0) << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal!=0?(byte)1:(byte)0) << 11)| ((I.MF!=0?(byte)1:(byte)0) << 15)) | 0xf000)>>8); }; };
    nec_ICount[0]-=10;
}

static void i_popf() /* Opcode 0x9d */
{
    uint tmp;
    { tmp = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
    { I.CarryVal = (int)((tmp) & 1); I.ParityVal = ((tmp) & 4)==0?1:0; I.AuxVal = (int)((tmp) & 16); I.ZeroVal = ((tmp) & 64)==0?1:0; I.SignVal = ((tmp) & 128)!=0 ? -1 : 0; I.TF = (byte)(((tmp) & 256) == 256?1:0); I.IF = (byte)(((tmp) & 512) == 512?1:0); I.DF = (byte)(((tmp) & 1024) == 1024?1:0); I.OverVal = (int)((tmp) & 2048); I.MF = (byte)(((tmp) & 0x8000) == 0x8000?1:0); };
 nec_ICount[0]-=10;
 if (I.TF!=0) nec_trap();
}

static void i_sahf() /* Opcode 0x9e */
{
 uint tmp = (uint)(((ushort)((I.CarryVal!=0?(byte)1:(byte)0) | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal!=0?(byte)1:(byte)0) << 4) | ((I.ZeroVal==0?(byte)1:(byte)0) << 6) | ((I.SignVal<0?(byte)1:(byte)0) << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal!=0?(byte)1:(byte)0) << 11)| ((I.MF!=0?(byte)1:(byte)0) << 15)) & 0xff00) | (I.regs.b[AH] & 0xd5));
    { I.CarryVal = (int)((tmp) & 1); I.ParityVal = ((tmp) & 4)==0?1:0; I.AuxVal = (int)((tmp) & 16); I.ZeroVal = ((tmp) & 64)==0?1:0; I.SignVal = ((tmp) & 128)!=0 ? -1 : 0; I.TF = (byte)(((tmp) & 256) == 256?1:0); I.IF = (byte)(((tmp) & 512) == 512?1:0); I.DF = (byte)(((tmp) & 1024) == 1024?1:0); I.OverVal = (int)((tmp) & 2048); I.MF = (byte)(((tmp) & 0x8000) == 0x8000?1:0); };
    nec_ICount[0]-=3;
}

static void i_lahf() /* Opcode 0x9f */
{
 I.regs.b[AH] = (byte)((ushort)((I.CarryVal!=0?(byte)1:(byte)0) | (parity_table[(byte)I.ParityVal] << 2) | ((I.AuxVal!=0?(byte)1:(byte)0) << 4) | ((I.ZeroVal==0?(byte)1:(byte)0) << 6) | ((I.SignVal<0?(byte)1:(byte)0) << 7) | (I.TF << 8) | (I.IF << 9) | (I.DF << 10) | ((I.OverVal!=0?(byte)1:(byte)0) << 11)| ((I.MF!=0?(byte)1:(byte)0) << 15)) & 0xff);
 nec_ICount[0]-=2;
}

static void i_mov_aldisp() /* Opcode 0xa0 */
{
    uint addr;

 addr = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 addr +=(uint)( ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 I.regs.b[AL] =(byte) GetMemB(DS, addr);
 nec_ICount[0]-=10;
}

static void i_mov_axdisp() /* Opcode 0xa1 */
{
    uint addr;

 addr = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 addr +=(uint)( ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 I.regs.b[AL] = GetMemB(DS, addr);
 I.regs.b[AH] = GetMemB(DS, addr+1);
 nec_ICount[0]-=14;
}


static void i_mov_dispal() /* Opcode 0xa2 */
{
    uint addr;
 addr = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 addr +=(uint)( ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(addr)),(int)(I.regs.b[AL])); };
 nec_ICount[0]-=9;
}

static void i_mov_dispax() /* Opcode 0xa3 */
{
    uint addr;
 addr = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 addr += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(addr)),(int)(I.regs.b[AL])); };
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (DS==DS || DS==SS)) ? I.prefix_base : I._base[DS])+(addr+1)),(int)(I.regs.b[AH])); };
 nec_ICount[0]-=13;
}

static void i_movsb() /* Opcode 0xa4 */
{
 byte tmp = GetMemB(DS,I.regs.w[IX]);
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)(tmp)); };
 I.regs.w[IY] +=(ushort)( -2 * I.DF + 1);
 I.regs.w[IX] +=(ushort)( -2 * I.DF + 1);
 nec_ICount[0]-=19; // 11+8n
}

static void i_movsw() /* Opcode 0xa5 */
{
 ushort tmp = GetMemW(DS,I.regs.w[IX]);
 { nec_ICount[0]-=11; { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)((byte)(tmp))); }; { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+((I.regs.w[IY])+1)),(int)((byte)((tmp)>>8))); }; };
 I.regs.w[IY] +=(ushort)(-4 * I.DF + 2);
 I.regs.w[IX] +=(ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=19; // 11+8n
}

static void i_cmpsb() /* Opcode 0xa6 */
{
 uint dst = GetMemB(ES, I.regs.w[IY]);
 uint src = GetMemB(DS, I.regs.w[IX]);
    { uint res=src-dst; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((src) ^ (dst)) & ((src) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((dst) ^ (src))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); src=(byte)res; }; /* opposite of the usual convention */
 I.regs.w[IY] +=(ushort)(-2 * I.DF + 1);
 I.regs.w[IX] +=(ushort)(-2 * I.DF + 1);
 nec_ICount[0]-=14;
}

static void i_cmpsw() /* Opcode 0xa7 */
{
 uint dst = GetMemW(ES, I.regs.w[IY]);
 uint src = GetMemW(DS, I.regs.w[IX]);
    { uint res=src-dst; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((src) ^ (dst)) & ((src) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((dst) ^ (src))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); src=(ushort)res; }; /* opposite of the usual convention */
 I.regs.w[IY] += (ushort)(-4 * I.DF + 2);
 I.regs.w[IX] += (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=14;
}

static void i_test_ald8() /* Opcode 0xa8 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.b[AL];
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
    nec_ICount[0]-=4;
}

static void i_test_axd16() /* Opcode 0xa9 */
{
    uint src = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++))); uint dst = I.regs.w[AW]; src += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    dst&=src; I.CarryVal=I.OverVal=I.AuxVal=0; I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
    nec_ICount[0]-=4;
}

static void i_stosb() /* Opcode 0xaa */
{
 { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)(I.regs.b[AL])); };
 I.regs.w[IY] += (ushort)(-2 * I.DF + 1);
 nec_ICount[0]-=5;
}

static void i_stosw() /* Opcode 0xab */
{
 { nec_ICount[0]-=11; { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+(I.regs.w[IY])),(int)((byte)(I.regs.w[AW]))); }; { nec_ICount[0]-=7; cpu_writemem20((int)(((I.seg_prefix !=0&& (ES==DS || ES==SS)) ? I.prefix_base : I._base[ES])+((I.regs.w[IY])+1)),(int)((byte)((I.regs.w[AW])>>8))); }; };
//	PutMemB(ES,I.regs.w[IY],I.regs.b[AL]); /* MISH */
//	PutMemB(ES,I.regs.w[IY]+1,I.regs.b[AH]);
 I.regs.w[IY] += (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=5;
}

static void i_lodsb() /* Opcode 0xac */
{
 I.regs.b[AL] = GetMemB(DS,I.regs.w[IX]);
 I.regs.w[IX] += (ushort)(-2 * I.DF + 1);
 nec_ICount[0]-=10;
}

static void i_lodsw() /* Opcode 0xad */
{
 I.regs.w[AW] = GetMemW(DS,I.regs.w[IX]);
 I.regs.w[IX] += (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=10;
}

static void i_scasb() /* Opcode 0xae */
{
 uint src = GetMemB(ES, I.regs.w[IY]);
 uint dst = I.regs.b[AL];
    { uint res=dst-src; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); dst=(byte)res; };
 I.regs.w[IY] += (ushort)(-2 * I.DF + 1);
 nec_ICount[0]-=12;
}

static void i_scasw() /* Opcode 0xaf */
{
 uint src = GetMemW(ES, I.regs.w[IY]);
 uint dst = I.regs.w[AW];
    { uint res=dst-src; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((dst) ^ (src)) & ((dst) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((src) ^ (dst))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); dst=(ushort)res; };
 I.regs.w[IY] += (ushort)(-4 * I.DF + 2);
 nec_ICount[0]-=12;
}

static void i_mov_ald8() /* Opcode 0xb0 */
{
 I.regs.b[AL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_cld8() /* Opcode 0xb1 */
{
 I.regs.b[CL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_dld8() /* Opcode 0xb2 */
{
 I.regs.b[DL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_bld8() /* Opcode 0xb3 */
{
 I.regs.b[BL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_ahd8() /* Opcode 0xb4 */
{
 I.regs.b[AH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_chd8() /* Opcode 0xb5 */
{
 I.regs.b[CH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_dhd8() /* Opcode 0xb6 */
{
 I.regs.b[DH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_bhd8() /* Opcode 0xb7 */
{
 I.regs.b[BH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_axd16() /* Opcode 0xb8 */
{
 I.regs.b[AL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[AH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_cxd16() /* Opcode 0xb9 */
{
 I.regs.b[CL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[CH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_dxd16() /* Opcode 0xba */
{
 I.regs.b[DL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[DH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_bxd16() /* Opcode 0xbb */
{
 I.regs.b[BL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[BH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_spd16() /* Opcode 0xbc */
{
 I.regs.b[SPL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[SPH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_bpd16() /* Opcode 0xbd */
{
 I.regs.b[BPL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[BPH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_sid16() /* Opcode 0xbe */
{
 I.regs.b[IXL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[IXH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void i_mov_did16() /* Opcode 0xbf */
{
 I.regs.b[IYL] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[IYH] = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=4;
}

static void nec_rotate_shift_Byte(uint ModRM, int count)
{
  uint src = (uint)GetRMByte(ModRM);
  uint dst=src;

 if (count < 0) /* FETCH must come _after_ GetRMWord */
  count = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

  if (count==0)
  {
 nec_ICount[0]-=8; /* or 7 if dest is in memory */
  }
  else if (count==1)
  {
    nec_ICount[0]-=(ModRM >=0xc0 )?2:16;
    switch (ModRM & 0x38)
    {
      case 0x00: /* ROL eb,1 */
 I.CarryVal = (int)(src & 0x80);
        dst=(src<<1)+(I.CarryVal!=0?(byte)1:(byte)0);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 I.OverVal = (int)((src^dst)&0x80);
 break;
      case 0x08: /* ROR eb,1 */
 I.CarryVal = (int)(src & 0x01);
        dst =(uint)( (((I.CarryVal!=0?(byte)1:(byte)0)<<8)+src) >> 1);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 I.OverVal = (int)((src^dst)&0x80);
 break;
      case 0x10: /* RCL eb,1 */
        dst=(src<<1)+(I.CarryVal!=0?(byte)1:(byte)0);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
        I.CarryVal =(int) ((dst) & 0x100);
 I.OverVal = (int)((src^dst)&0x80);
 break;
      case 0x18: /* RCR eb,1 */
        dst =(uint)( (((I.CarryVal!=0?(byte)1:(byte)0)<<8)+src) >> 1);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 I.CarryVal =(int)( src & 0x01);
 I.OverVal =(int)( (src^dst)&0x80);
 break;
      case 0x20: /* SHL eb,1 */
      case 0x30:
        dst = src << 1;
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
        I.CarryVal =(int) ((dst) & 0x100);
 I.OverVal =(int)( (src^dst)&0x80);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 break;
      case 0x28: /* SHR eb,1 */
        dst = src >> 1;
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 I.CarryVal = (int)(src & 0x01);
 I.OverVal = (int)(src & 0x80);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 break;
      case 0x38: /* SAR eb,1 */
        dst = (uint)(((sbyte)src) >> 1);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)dst); }; };
 I.CarryVal = (int)(src & 0x01);
 I.OverVal = 0;
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 break;
    }
  }
  else
  {
    nec_ICount[0]-=(ModRM >=0xc0 )?7+4*count:19+4*count;
    switch (ModRM & 0x38)
    {
      case 0x00: /* ROL eb,count */
 for (; count > 0; count--)
 {
   I.CarryVal = (int)(dst & 0x80);
          dst = (uint)((dst << 1) + (I.CarryVal!=0?(byte)1:(byte)0));
 }
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
     case 0x08: /* ROR eb,count */
 for (; count > 0; count--)
 {
   I.CarryVal = (int)(dst & 0x01);
          dst =(uint)( (dst >> 1) + ((I.CarryVal!=0?(byte)1:(byte)0) << 7));
 }
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
      case 0x10: /* RCL eb,count */
 for (; count > 0; count--)
 {
          dst =(uint)( (dst << 1) + (I.CarryVal!=0?(byte)1:(byte)0));
          I.CarryVal =(int) ((dst) & 0x100);
 }
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
      case 0x18: /* RCR eb,count */
 for (; count > 0; count--)
 {
          dst =(uint)( ((I.CarryVal!=0?(byte)1:(byte)0)<<8)+dst);
   I.CarryVal = (int)(dst & 0x01);
          dst >>= 1;
 }
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
      case 0x20:
      case 0x30: /* SHL eb,count */
        dst <<= count;
        I.CarryVal =(int) ((dst) & 0x100);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
      case 0x28: /* SHR eb,count */
        dst >>= count-1;
 I.CarryVal =(int)( dst & 0x1);
        dst >>= 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 I.AuxVal = 1;
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
      case 0x38: /* SAR eb,count */
        dst =(uint)( ((sbyte)dst) >> (count-1));
 I.CarryVal =(int)( dst & 0x1);
        dst = (uint)(((sbyte)((byte)dst)) >> 1);
        I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(dst);
 I.AuxVal = 1;
        { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)dst; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)dst); }; };
 break;
    }
  }
}


static void nec_rotate_shift_Word(uint ModRM, int count)
{
 uint src = GetRMWord(ModRM);
 uint dst=src;

 if (count < 0) /* FETCH must come _after_ GetRMWord */
  count = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

  if (count==0)
  {
 nec_ICount[0]-=8; /* or 7 if dest is in memory */
  }
  else if (count==1)
  {
    nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
    switch (ModRM & 0x38)
    {
      case 0x00: /* ROL ew,1 */
 I.CarryVal = (int)(src & 0x8000);
        dst=(uint)((src<<1)+(I.CarryVal!=0?(byte)1:(byte)0));
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 I.OverVal = (int)((src^dst)&0x8000);
 break;
      case 0x08: /* ROR ew,1 */
 I.CarryVal = (int)(src & 0x01);
        dst = (uint)((((I.CarryVal!=0?(byte)1:(byte)0)<<16)+src) >> 1);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 I.OverVal = (int)((src^dst)&0x8000);
 break;
      case 0x10: /* RCL ew,1 */
        dst=(uint)((src<<1)+(I.CarryVal!=0?(byte)1:(byte)0));
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
        I.CarryVal = (int)((dst) & 0x10000);
 I.OverVal = (int)((src^dst)&0x8000);
 break;
      case 0x18: /* RCR ew,1 */
        dst = (uint)((((I.CarryVal!=0?(byte)1:(byte)0)<<16)+src) >> 1);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 I.CarryVal = (int)(src & 0x01);
 I.OverVal = (int)((src^dst)&0x8000);
 break;
      case 0x20: /* SHL ew,1 */
      case 0x30:
        dst = src << 1;
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
        I.CarryVal = (int)((dst) & 0x10000);
 I.OverVal =(int)( (src^dst)&0x8000);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 break;
      case 0x28: /* SHR ew,1 */
        dst = src >> 1;
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 I.CarryVal =(int)( src & 0x01);
 I.OverVal = (int)(src & 0x8000);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 break;
      case 0x38: /* SAR ew,1 */
        dst =(uint)( ((short)src) >> 1);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 I.CarryVal = (int)(src & 0x01);
 I.OverVal = 0;
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 break;

    }
  }
  else
  {

    nec_ICount[0]-=(ModRM >=0xc0 )?7+count*4:27+count*4;
    switch (ModRM & 0x38)
    {
      case 0x00: /* ROL ew,count */
      for (; count > 0; count--)
 {
   I.CarryVal = (int)(dst & 0x8000);
          dst = (uint)((dst << 1) + (I.CarryVal!=0?(byte)1:(byte)0));
 }
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x08: /* ROR ew,count */
 for (; count > 0; count--)
 {
   I.CarryVal =(int)( dst & 0x01);
          dst =(uint) ((dst >> 1) + ((I.CarryVal!=0?(byte)1:(byte)0) << 15));
 }
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x10: /* RCL ew,count */
 for (; count > 0; count--)
 {
          dst =(uint)( (dst << 1) + (I.CarryVal!=0?(byte)1:(byte)0));
          I.CarryVal = (int)((dst) & 0x10000);
 }
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x18: /* RCR ew,count */
 for (; count > 0; count--)
 {
          dst = (uint)(dst + ((I.CarryVal!=0?(byte)1:(byte)0) << 16));
   I.CarryVal =(int)( dst & 0x01);
           dst >>= 1;
 }
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x20:
      case 0x30: /* SHL ew,count */
        dst <<= count;
        I.CarryVal = (int)((dst) & 0x10000);
 I.AuxVal = 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x28: /* SHR ew,count */
        dst >>= count-1;
 I.CarryVal = (int)(dst & 0x1);
        dst >>= 1;
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 I.AuxVal = 1;
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
      case 0x38: /* SAR ew,count */
        dst = (uint)(((short)dst) >> (count-1));
 I.CarryVal = (int)(dst & 0x01);
        dst = (uint)(((short)((ushort)dst)) >> 1);
        I.SignVal=I.ZeroVal=I.ParityVal=(short)(dst);
 I.AuxVal = 1;
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)dst; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(dst)); cpu_writemem20((int)((EA)+1),(int)(dst)>>8); }; };
 break;
    }
  }
}


static void i_rotshft_bd8() /* Opcode 0xc0 */
{
    uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    nec_rotate_shift_Byte(ModRM,-1);
}

static void i_rotshft_wd8() /* Opcode 0xc1 */
{
    uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    nec_rotate_shift_Word(ModRM,-1);
}


static void i_ret_d16() /* Opcode 0xc2 */
{
 uint count = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 count += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 { I.ip = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I.regs.w[SP]+=(ushort)count;
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=22; // near 20-24
}

static void i_ret() /* Opcode 0xc3 */
{
 { I.ip = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=17; // near 15-19
}

static void i_les_dw() /* Opcode 0xc4 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort tmp = GetRMWord(ModRM);

    I.regs.w[(int)Mod_RM.reg.w[ModRM]]= tmp;
    I.sregs[ES] = ReadWord(EA+2);
    I._base[ES] = (uint)(I.sregs[ES] << 4);
    nec_ICount[0]-=22; /* 18-26 */
}

static void i_lds_dw() /* Opcode 0xc5 */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    ushort tmp = GetRMWord(ModRM);

    I.regs.w[(int)Mod_RM.reg.w[ModRM]]=tmp;
    I.sregs[DS] = ReadWord(EA+2);
    I._base[DS] =(uint) (I.sregs[DS] << 4);
    nec_ICount[0]-=22; /* 18-26 */
}

static void i_mov_bd8() /* Opcode 0xc6 */
{
    uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))); else { GetEA[ModRM](); { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)))); }; } };
    nec_ICount[0]-=(ModRM >=0xc0 )?4:11;
}

static void i_mov_wd16() /* Opcode 0xc7 */
{
    uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    { ushort val; if (ModRM >= 0xc0) { I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)(cpu_readop_arg((uint)((I._base[CS]+I.ip)))+(cpu_readop_arg((uint)((I._base[CS]+I.ip+1)))<<8)); I.ip+=2; } else { GetEA[ModRM](); { val=(ushort)(cpu_readop_arg((uint)((I._base[CS]+I.ip)))+(cpu_readop_arg((uint)((I._base[CS]+I.ip+1)))<<8)); I.ip+=2; } { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(val)); cpu_writemem20((int)((EA)+1),(int)(val)>>8); }; } };
    nec_ICount[0]-=(ModRM >=0xc0 )?4:15;
}

static void i_enter() /* Opcode 0xc8 */
{
    uint nb = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint i,level;

    nec_ICount[0]-=23;
    nb += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
    level = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BP])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BP])>>8); }; };
    I.regs.w[BP]=I.regs.w[SP];
    I.regs.w[SP] -= (ushort)nb;
    for (i=1;i<level;i++) {
 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(GetMemW(SS,I.regs.w[BP]-i*2))); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(GetMemW(SS,I.regs.w[BP]-i*2))>>8); }; };
 nec_ICount[0]-=16;
    }
    if (level!=0) { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.regs.w[BP])); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.regs.w[BP])>>8); }; };
}

static void i_leave() /* Opcode 0xc9 */
{
 I.regs.w[SP]=I.regs.w[BP];
 { I.regs.w[BP] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 nec_ICount[0]-=8;
}

static void i_retf_d16() /* Opcode 0xca */
{
 uint count = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 count +=(uint)( ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);
 { I.ip = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.sregs[CS] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[CS] =(uint) (I.sregs[CS] << 4);
 I.regs.w[SP]+=(ushort)count;
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=25; // 21-29
}

static void i_retf() /* Opcode 0xcb */
{
 { I.ip = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.sregs[CS] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[CS] =(uint) (I.sregs[CS] << 4);
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=28; // 24-32
}

static void i_int3() /* Opcode 0xcc */
{
 nec_ICount[0]-=38; // 38-50
 nec_interrupt(3,false);
}

static void i_int() /* Opcode 0xcd */
{
 uint int_num = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=38; // 38-50
 nec_interrupt(int_num,false);
}

static void i_into() /* Opcode 0xce */
{
    if ((I.OverVal!=0?(byte)1:(byte)0)!=0) {
 nec_ICount[0]-=52;
 nec_interrupt(4,false);
    } else nec_ICount[0]-=3; /* 3 or 52! */
}

static void i_iret() /* Opcode 0xcf */
{
 { I.ip = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 { I.sregs[CS] = ReadWord(((I._base[SS]+I.regs.w[SP]))); I.regs.w[SP]+=2; };
 I._base[CS] = (uint)(I.sregs[CS] << 4);
     i_popf();
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=32; // 27-39
}

static void i_rotshft_b() /* Opcode 0xd0 */
{
 nec_rotate_shift_Byte(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))),1);
}


static void i_rotshft_w() /* Opcode 0xd1 */
{
 nec_rotate_shift_Word(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))),1);
}


static void i_rotshft_bcl() /* Opcode 0xd2 */
{
 nec_rotate_shift_Byte(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))),I.regs.b[CL]);
}

static void i_rotshft_wcl() /* Opcode 0xd3 */
{
 nec_rotate_shift_Word(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))),I.regs.b[CL]);
}

/* OB: Opcode works on NEC V-Series but not the Variants 		*/
/*     one could specify any byte value as operand but the NECs */
/*     always substitute 0x0a.									*/
static void i_aam() /* Opcode 0xd4 */
{
 uint mult=((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));

 if (mult == 0)
  nec_interrupt(0,false);
    else
    {
  I.regs.b[AH] = (byte)(I.regs.b[AL] / 10);
  I.regs.b[AL] %= 10;
  I.SignVal=I.ZeroVal=I.ParityVal=(short)(I.regs.w[AW]);
  nec_ICount[0]-=15;
    }
}

/* OB: Opcode works on NEC V-Series but not the Variants 	*/
/*     one could specify any byte value as operand but the NECs */
/*     always substitute 0x0a.					*/
static void i_aad() /* Opcode 0xd5 */
{
 uint mult=((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))); /* eat operand = ignore ! */

 I.regs.b[AL] =(byte)( I.regs.b[AH] * 10 + I.regs.b[AL]);
 I.regs.b[AH] = 0;

 I.ZeroVal = (int)(I.regs.b[AL]);
 I.ParityVal = (int)(I.regs.b[AL]);
 I.SignVal = 0;
 nec_ICount[0]-=7;
 mult=0;
}

static void i_setalc() /* Opcode 0xd6 */
{

 I.regs.b[AL] = ((I.CarryVal!=0?(byte)1:(byte)0))!=0?(byte)0xff:(byte)0x00;
 nec_ICount[0]-=3; // V30
}

static void i_xlat() /* Opcode 0xd7 */
{
 uint dest = (uint)(I.regs.w[BW]+I.regs.b[AL]);
 I.regs.b[AL] = GetMemB(DS, dest);
 nec_ICount[0]-=9; // V30
}

static void i_escape() /* Opcodes 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde and 0xdf */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 nec_ICount[0]-=2; // dont found any info :-(, set same as hlt
     GetRMByte(ModRM);
}

static void i_loopne() /* Opcode 0xe0 */
{
    int disp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    uint tmp = (uint)(I.regs.w[CW]-1);

    I.regs.w[CW]=(ushort)tmp;

    if ((I.ZeroVal==0?(byte)1:(byte)0)==0 && tmp!=0) {
 nec_ICount[0]-=14;
 I.ip = (ushort)(I.ip+disp);
 change_pc20((uint)(I._base[CS]+I.ip));
    } else nec_ICount[0]-=5;
}

static void i_loope() /* Opcode 0xe1 */
{
    int disp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
    uint tmp = (uint)(I.regs.w[CW]-1);
    I.regs.w[CW]=(ushort)tmp;

    if ((I.ZeroVal==0?(byte)1:(byte)0) !=0&& tmp!=0) {
 nec_ICount[0]-=14;
 I.ip = (ushort)(I.ip+disp);
 change_pc20((uint)(I._base[CS]+I.ip));
   } else nec_ICount[0]-=5;
}

static void i_loop() /* Opcode 0xe2 */
{
 int disp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 uint tmp = (uint)(I.regs.w[CW]-1);

 I.regs.w[CW]=(ushort)tmp;

    if (tmp!=0) {
 nec_ICount[0]-=13;
 I.ip = (ushort)(I.ip+disp);
 change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=5;
}

static void i_jcxz() /* Opcode 0xe3 */
{
 int disp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 if (I.regs.w[CW] == 0) {
 nec_ICount[0]-=13;
 I.ip = (ushort)(I.ip+disp);
 change_pc20((uint)(I._base[CS]+I.ip));
 } else nec_ICount[0]-=5;
}

static void i_inal() /* Opcode 0xe4 */
{
 uint port = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[AL] = (byte)cpu_readport((int)port);
 nec_ICount[0]-=9;
}

static void i_inax() /* Opcode 0xe5 */
{
 uint port = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 I.regs.b[AL] = (byte)cpu_readport((int)port);
 I.regs.b[AH] = (byte)cpu_readport((int)port+1);
 nec_ICount[0]-=13;
}

static void i_outal() /* Opcode 0xe6 */
{
 uint port = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 cpu_writeport((int)port,I.regs.b[AL]);
 nec_ICount[0]-=8;
}

static void i_outax() /* Opcode 0xe7 */
{
 uint port = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 cpu_writeport((int)port,I.regs.b[AL]);
 cpu_writeport((int)port+1,I.regs.b[AH]);
 nec_ICount[0]-=12;
}

static void i_call_d16() /* Opcode 0xe8 */
{
 uint tmp = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.ip)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.ip)>>8); }; };
 I.ip = (ushort)(I.ip+(short)tmp);
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=24; // 21-29
}


static void i_jmp_d16() /* Opcode 0xe9 */
{
 int tmp = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp += (int)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 I.ip = (ushort)(I.ip+(short)tmp);
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=15;
}

static void i_jmp_far() /* Opcode 0xea */
{
    uint tmp,tmp1;

 tmp = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 tmp1 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 tmp1 += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

 I.sregs[CS] = (ushort)tmp1;
 I._base[CS] = (uint)(I.sregs[CS] << 4);
 I.ip = (ushort)tmp;
 change_pc20((uint)(I._base[CS]+I.ip));
 nec_ICount[0]-=27; // 27-35
}

static void i_jmp_d8() /* Opcode 0xeb */
{
 int tmp = (int)((sbyte)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))));
 I.ip = (ushort)(I.ip+tmp);
 nec_ICount[0]-=12;
}

static void i_inaldx() /* Opcode 0xec */
{
 I.regs.b[AL] = (byte)cpu_readport((int)I.regs.w[DW]);
 nec_ICount[0]-=8;
}

static void i_inaxdx() /* Opcode 0xed */
{
 uint port = I.regs.w[DW];

 I.regs.b[AL] = (byte)cpu_readport((int)port);
 I.regs.b[AH] = (byte)cpu_readport((int)port+1);
 nec_ICount[0]-=12;
}

static void i_outdxal() /* Opcode 0xee */
{
 cpu_writeport((int)I.regs.w[DW],I.regs.b[AL]);
 nec_ICount[0]-=8;
}

static void i_outdxax() /* Opcode 0xef */
{
 uint port = I.regs.w[DW];
 cpu_writeport((int)port,I.regs.b[AL]);
 cpu_writeport((int)port+1,I.regs.b[AH]);
 nec_ICount[0]-=12;
}

/* I think thats not a V20 instruction...*/
static void i_lock() /* Opcode 0xf0 */
{
 nec_ICount[0]-=2;
 nec_instruction[((byte)cpu_readop((uint)(I._base[CS]+I.ip++)))](); /* un-interruptible */
}


static void i_brks() /* Opcode 0xf1 - Break to Security Mode */
{
 uint int_vector;
 int_vector=((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
 //if (errorlog) fprintf(errorlog,"PC=%06x : BRKS %02x\n",cpu_get_pc()-2,int_vector);
}


static void rep(int flagval)
{
    /* Handles rep- and repnz- prefixes. flagval is the value of ZF for the

       loop  to continue for CMPS and SCAS instructions. */
 uint next = ((byte)cpu_readop((uint)(I._base[CS]+I.ip++)));
 uint count = I.regs.w[CW];

    switch(next)
    {
     case 0x26: /* ES: */
   I.seg_prefix=1;
   I.prefix_base=I._base[ES];
   nec_ICount[0]-=2;
   rep(flagval);
   break;
     case 0x2e: /* CS: */
   I.seg_prefix=1;
   I.prefix_base=I._base[CS];
   nec_ICount[0]-=2;
   rep(flagval);
   break;
    case 0x36: /* SS: */
        I.seg_prefix=1;
 I.prefix_base=I._base[SS];
 nec_ICount[0]-=2;
 rep(flagval);
 break;
    case 0x3e: /* DS: */
        I.seg_prefix=1;
 I.prefix_base=I._base[DS];
 nec_ICount[0]-=2;
 rep(flagval);
 break;
    case 0x6c: /* REP INSB */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
         i_insb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0x6d: /* REP INSW */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
         i_insw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0x6e: /* REP OUTSB */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
           i_outsb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0x6f: /* REP OUTSW */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
            i_outsw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xa4: /* REP MOVSB */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
  i_movsb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xa5: /* REP MOVSW */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
  i_movsw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xa6: /* REP(N)E CMPSB */
 nec_ICount[0]-=9;
 for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && (count > 0); count--)
  i_cmpsb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xa7: /* REP(N)E CMPSW */
 nec_ICount[0]-=9;
 for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && (count > 0); count--)
  i_cmpsw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xaa: /* REP STOSB */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
  i_stosb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xab: /* REP STOSW */
 nec_ICount[0]-=(int)(9-count);
 for (; count > 0; count--)
  i_stosw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xac: /* REP LODSB */
 nec_ICount[0]-=9;
 for (; count > 0; count--)
  i_lodsb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xad: /* REP LODSW */
 nec_ICount[0]-=9;
 for (; count > 0; count--)
  i_lodsw();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xae: /* REP(N)E SCASB */
 nec_ICount[0]-=9;
 for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && (count > 0); count--)
  i_scasb();
 I.regs.w[CW]=(ushort)count;
 break;
    case 0xaf: /* REP(N)E SCASW */
 nec_ICount[0]-=9;
 for (I.ZeroVal = flagval==0?1:0; ((I.ZeroVal==0?(byte)1:(byte)0) == flagval) && (count > 0); count--)
  i_scasw();
 I.regs.w[CW]=(ushort)count;
 break;
    default:
 nec_instruction[next]();
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
 nec_ICount[0]=0;
}

static void i_cmc() /* Opcode 0xf5 */
{
 I.CarryVal = (I.CarryVal!=0?(byte)1:(byte)0)==0?1:0;
 nec_ICount[0]-=2;
}

static void i_f6pre()
{
 /* Opcode 0xf6 */
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint tmp = (uint)GetRMByte(ModRM);
    uint tmp2;


    switch (ModRM & 0x38)
    {
    case 0x00: /* TEST Eb, data8 */
    case 0x08: /* ??? */
  tmp &= (uint)((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
  I.CarryVal = I.OverVal = I.AuxVal = 0;
  I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(tmp);
  nec_ICount[0]-=(ModRM >=0xc0 )?4:11;
  break;

    case 0x10: /* NOT Eb */
  { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)~tmp; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)~tmp); }; };
  nec_ICount[0]-=(ModRM >=0xc0 )?2:16;
  break;
    case 0x18: /* NEG Eb */
         tmp2=0;
         { uint res=tmp2-tmp; I.CarryVal =(int) ((res) & 0x100); I.OverVal =(int)(((tmp2) ^ (tmp)) & ((tmp2) ^ (res)) & 0x80); I.AuxVal =(int) (((res) ^ ((tmp) ^ (tmp2))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(res); tmp2=(byte)res; };
         { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)tmp2; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)tmp2); }; };
         nec_ICount[0]-=(ModRM >=0xc0 )?2:16;
  break;
    case 0x20: /* MUL AL, Eb */
  {
   ushort result;
   tmp2 = I.regs.b[AL];

   I.SignVal = (int)((sbyte)tmp2);
   I.ParityVal = (int)(tmp2);

   result = (ushort)((ushort)tmp2*tmp);
   I.regs.w[AW]=(ushort)result;

   I.ZeroVal = (int)(I.regs.w[AW]);
   I.CarryVal = I.OverVal = (I.regs.b[AH] != 0)?1:0;
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?30:36;
  break;
    case 0x28: /* IMUL AL, Eb */
  {
   short result;

   tmp2 = (uint)I.regs.b[AL];

   I.SignVal = (int)((sbyte)tmp2);
   I.ParityVal = (int)(tmp2);

   result = (short)((short)((sbyte)tmp2)*(short)((sbyte)tmp));
   I.regs.w[AW]=(ushort)result;

   I.ZeroVal = (int)(I.regs.w[AW]);

   I.CarryVal = I.OverVal = ((result >> 7 != 0) && (result >> 7 != -1))?1:0;
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?30:39;
  break;
    case 0x30: /* IYV AL, Ew */
  {
   ushort result;

   result = I.regs.w[AW];

   if (tmp!=0)
   {
    tmp2 = result % tmp;
    result = (ushort)(result / tmp);
    if (result > 0xff)
    {
     nec_interrupt(0,false);
     break;
    }
    else
    {
     I.regs.b[AL] = (byte)result;
     I.regs.b[AH] = (byte)tmp2;
    }
   }
   else
   {
    nec_interrupt(0,false);
    break;
   }
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?25:35;
  break;
    case 0x38: /* IIYV AL, Ew */
  {

   short result;

   result = (short)I.regs.w[AW];

   if (tmp!=0)
   {
    tmp2 =(uint)( result % (short)((sbyte)tmp));

    if ((result /= (short)((sbyte)tmp)) > 0xff)
    {
     nec_interrupt(0,false);
     break;
    }
    else
    {
     I.regs.b[AL] =(byte)result;
     I.regs.b[AH] =(byte)tmp2;
    }
   }
   else
   {
    nec_interrupt(0,false);
    break;
   }
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?43:53;
  break;
    }
}


static void i_f7pre()
{
 /* Opcode 0xf7 */
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint tmp = GetRMWord(ModRM);
    uint tmp2;


    switch (ModRM & 0x38)
    {
    case 0x00: /* TEST Ew, data16 */
    case 0x08: /* ??? */
  tmp2 = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
  tmp2 += (uint)(((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++))) << 8);

  tmp &= tmp2;

  I.CarryVal = I.OverVal = I.AuxVal = 0;
  I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp);
  nec_ICount[0]-=(ModRM >=0xc0 )?4:15;
  break;
    case 0x10: /* NOT Ew */
  tmp = ~tmp;
  { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp)); cpu_writemem20((int)((EA)+1),(int)(tmp)>>8); }; };
  nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
  break;

    case 0x18: /* NEG Ew */
        tmp2 = 0;
        { uint res=tmp2-tmp; I.CarryVal = (int)((res) & 0x10000); I.OverVal =(int)(((tmp2) ^ (tmp)) & ((tmp2) ^ (res)) & 0x8000); I.AuxVal =(int) (((res) ^ ((tmp) ^ (tmp2))) & 0x10); I.SignVal=I.ZeroVal=I.ParityVal=(short)(res); tmp2=(ushort)res; };
        { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)tmp2; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)(tmp2)); cpu_writemem20((int)((EA)+1),(int)(tmp2)>>8); }; };
 nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
 break;
    case 0x20: /* MUL AW, Ew */
  {
   uint result;
   tmp2 = I.regs.w[AW];

   I.SignVal = (int)((short)tmp2);
   I.ParityVal = (int)(tmp2);

   result = (uint)tmp2*tmp;
   I.regs.w[AW]=(ushort)result;
            result >>= 16;
   I.regs.w[DW]=(ushort)result;

   I.ZeroVal = (int)(I.regs.w[AW] | I.regs.w[DW]);
   I.CarryVal = I.OverVal = (I.regs.w[DW] != 0)?1:0;
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?30:36;
  break;

    case 0x28: /* IMUL AW, Ew */
  nec_ICount[0]-=150;
  {
   int result;

   tmp2 = I.regs.w[AW];

   I.SignVal = (int)((short)tmp2);
   I.ParityVal = (int)(tmp2);

   result = (int)((short)tmp2)*(int)((short)tmp);
   I.CarryVal = I.OverVal = ((result >> 15 != 0) && (result >> 15 != -1))?1:0;

   I.regs.w[AW]=(ushort)result;
   result = (ushort)(result >> 16);
   I.regs.w[DW]=(ushort)result;

   I.ZeroVal = (int)(I.regs.w[AW] | I.regs.w[DW]);
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?34:44;
  break;
    case 0x30: /* IYV AW, Ew */
  {
   uint result;

            result = (((uint)I.regs.w[DW]) << 16) | I.regs.w[AW];

   if (tmp!=0)
   {
    tmp2 = result % tmp;
    if ((result /= tmp) > 0xffff)
    {
     nec_interrupt(0,false);
     break;
    }
    else
    {
                    I.regs.w[AW]=(ushort)result;
                    I.regs.w[DW]=(ushort)tmp2;
    }
   }
   else
   {
    nec_interrupt(0,false);
    break;
   }
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?25:35;
  break;
    case 0x38: /* IIYV AW, Ew */
  {
   int result;

   result = (int)(((uint)I.regs.w[DW] << 16) + I.regs.w[AW]);

   if (tmp!=0)
   {
    tmp2 = (uint)(result % (int)((short)tmp));
    if ((result /= (int)((short)tmp)) > 0xffff)
    {
     nec_interrupt(0,false);
     break;
    }
    else
    {
     I.regs.w[AW]=(ushort)result;
     I.regs.w[DW]=(ushort)tmp2;
    }
   }
   else
   {
    nec_interrupt(0,false);
    break;
   }
  }
  nec_ICount[0]-=(ModRM >=0xc0 )?43:53;
  break;
    }
}


static void i_clc() /* Opcode 0xf8 */
{
 I.CarryVal = 0;
 nec_ICount[0]-=2;
}

static void i_stc() /* Opcode 0xf9 */
{
 I.CarryVal = 1;
 nec_ICount[0]-=2;
}

static void i_di() /* Opcode 0xfa */
{
 I.IF = (0);
 nec_ICount[0]-=2;
}

static void i_ei() /* Opcode 0xfb */
{
 I.IF = (1);
 nec_ICount[0]-=2;
}

static void i_cld() /* Opcode 0xfc */
{
 I.DF = (0);
 nec_ICount[0]-=2;
}

static void i_std() /* Opcode 0xfd */
{
 I.DF = (1);
 nec_ICount[0]-=2;
}

static void i_fepre() /* Opcode 0xfe */
{
    uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint tmp = GetRMByte(ModRM);
    uint tmp1;

    if ((ModRM & 0x38) == 0) /* INC eb */
    {
  tmp1 = tmp+1;
  I.OverVal =(int)(((tmp1) ^ (tmp)) & ((tmp1) ^ (1)) & 0x80);
    }
    else /* DEC eb */
    {
  tmp1 = tmp-1;
  I.OverVal =(int)(((tmp) ^ (1)) & ((tmp) ^ (tmp1)) & 0x80);
    }

    I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10);
    I.SignVal=I.ZeroVal=I.ParityVal=(sbyte)(tmp1);

    { if (ModRM >= 0xc0) I.regs.b[(int)Mod_RM.RM.b[ModRM]]=(byte)(byte)tmp1; else { nec_ICount[0]-=7; cpu_writemem20((int)(EA),(int)(byte)tmp1); }; };
    nec_ICount[0]-=(ModRM >=0xc0 )?2:16;
}


static void i_ffpre() /* Opcode 0xff */
{
 uint ModRM = ((byte)cpu_readop_arg((uint)(I._base[CS]+I.ip++)));
    uint tmp;
    uint tmp1;

    switch(ModRM & 0x38)
    {
    case 0x00: /* INC ew */
  tmp = GetRMWord(ModRM);
  tmp1 = tmp+1;

  /*SetOFW_Add(tmp1,tmp,1);*/
        I.OverVal = (tmp==0x7fff)?1:0; /* Mish */
  I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10);
  I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1);

  { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)(ushort)tmp1; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)((ushort)tmp1)); cpu_writemem20((int)((EA)+1),(int)((ushort)tmp1)>>8); }; };
  nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
  break;

    case 0x08: /* DEC ew */
  tmp = GetRMWord(ModRM);
  tmp1 = tmp-1;

  /*SetOFW_Sub(tmp1,1,tmp);*/
  I.OverVal = (tmp==0x8000)?1:0; /* Mish */
  I.AuxVal =(int) (((tmp1) ^ ((tmp) ^ (1))) & 0x10);
  I.SignVal=I.ZeroVal=I.ParityVal=(short)(tmp1);

  { if (ModRM >= 0xc0) I.regs.w[(int)Mod_RM.RM.w[ModRM]]=(ushort)(ushort)tmp1; else { nec_ICount[0]-=11; cpu_writemem20((int)(EA),(byte)((ushort)tmp1)); cpu_writemem20((int)((EA)+1),(int)((ushort)tmp1)>>8); }; };
  nec_ICount[0]-=(ModRM >=0xc0 )?2:24;
  break;

    case 0x10: /* CALL ew */
  tmp = GetRMWord(ModRM);
  { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.ip)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.ip)>>8); }; };
  I.ip = (ushort)tmp;
  change_pc20((uint)(I._base[CS]+I.ip));
  nec_ICount[0]-=(ModRM >=0xc0 )?16:20;
  break;

 case 0x18: /* CALL FAR ea */
  tmp = I.sregs[CS]; /* HJB 12/13/98 need to skip displacements of EA */
  tmp1 = GetRMWord(ModRM);
  I.sregs[CS] = ReadWord(EA+2);
  I._base[CS] = (uint)(I.sregs[CS] << 4);
  { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(tmp)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(tmp)>>8); }; };
  { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(I.ip)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(I.ip)>>8); }; };
  I.ip =(int) tmp1;
  change_pc20((uint)(I._base[CS]+I.ip));
  nec_ICount[0]-=(ModRM >=0xc0 )?16:26;
  break;

    case 0x20: /* JMP ea */
  nec_ICount[0]-=13;
  I.ip = GetRMWord(ModRM);
  change_pc20((uint)(I._base[CS]+I.ip));
  break;

    case 0x28: /* JMP FAR ea */
  nec_ICount[0]-=15;
  I.ip = GetRMWord(ModRM);
  I.sregs[CS] = ReadWord(EA+2);
  I._base[CS] =(uint) (I.sregs[CS] << 4);
  change_pc20((uint)(I._base[CS]+I.ip));
  break;

    case 0x30: /* PUSH ea */
  nec_ICount[0]-=4;
  tmp = GetRMWord(ModRM);
  { I.regs.w[SP]-=2; { nec_ICount[0]-=11; cpu_writemem20((int)(((I._base[SS]+I.regs.w[SP]))),(byte)(tmp)); cpu_writemem20((int)((((I._base[SS]+I.regs.w[SP])))+1),(int)(tmp)>>8); }; };
  break;
    }
}


static void i_invalid()
{
    /* makes the cpu loops forever until user resets it */
 /*	{ extern int debug_key_pressed; debug_key_pressed = 1; } */
 I.ip--;
 nec_ICount[0]-=10;
}

uint nec_get_context(ref object dst)
{
 if( dst !=null)
  dst = I;
    return 1;
}

void nec_set_context(object src)
{
 if( src !=null)
 {
  I = (nec_Regs)src;
  I._base[CS] = (uint)(I.sregs[CS] << 4);
  I._base[DS] = (uint)(I.sregs[DS] << 4);
  I._base[ES] = (uint)(I.sregs[ES] << 4);
  I._base[SS] = (uint)(I.sregs[SS] << 4);
  change_pc20((uint)(I._base[CS]+I.ip));
 }
}

uint nec_get_pc()
{
 return (I._base[CS] + (ushort)I.ip);
}

void nec_set_pc(uint val)
{
 if( val - I._base[CS] < 0x10000 )
 {
  I.ip = (int)(val - I._base[CS]);
 }
 else
 {
  I._base[CS] = val & 0xffff0;
  I.sregs[CS] = (ushort)(I._base[CS] >> 4);
  I.ip = (int)(val & 0x0000f);
 }
}

uint nec_get_sp()
{
 return I._base[SS] + I.regs.w[SP];
}

void nec_set_sp(uint val)
{
 if( val - I._base[SS] < 0x10000 )
 {
  I.regs.w[SP] = (ushort)(val - I._base[SS]);
 }
 else
 {
  I._base[SS] = val & 0xffff0;
  I.sregs[SS] =(ushort)( I._base[SS] >> 4);
  I.regs.w[SP] = (ushort)(val & 0x0000f);
 }
}

uint nec_get_reg(int regnum)
{
 switch( regnum )
 {
  case NEC_IP: return (uint)I.ip;
  case NEC_SP: return I.regs.w[SP];
  case NEC_FLAGS: //CompressFlags(); 
   return I.flags;
        case NEC_AW: return I.regs.w[AW];
  case NEC_CW: return I.regs.w[CW];
  case NEC_DW: return I.regs.w[DW];
  case NEC_BW: return I.regs.w[BW];
  case NEC_BP: return I.regs.w[BP];
  case NEC_IX: return I.regs.w[IX];
  case NEC_IY: return I.regs.w[IY];
  case NEC_ES: return I.sregs[ES];
  case NEC_CS: return I.sregs[CS];
  case NEC_SS: return I.sregs[SS];
  case NEC_DS: return I.sregs[DS];
  case NEC_VECTOR: return I.int_vector;
  case NEC_PENDING: return I.pending_irq;
  case NEC_NMI_STATE: return (uint)I.nmi_state;
  case NEC_IRQ_STATE: return (uint)I.irq_state;
  case REG_PREVIOUSPC: return 0; /* not supported */
  default:
   if( regnum <= REG_SP_CONTENTS )
   {
    uint offset = (uint)(((I._base[SS] + I.regs.w[SP])) + 2 * (REG_SP_CONTENTS - regnum));
    return (uint)(cpu_readmem20((int) offset ) | ( cpu_readmem20( (int)offset + 1) << 8 ));
   }
   break;
 }
 return 0;
}

void nec_set_reg(int regnum, uint val)
{
 switch( regnum )
 {
  case NEC_IP: I.ip = (int)val; break;
  case NEC_SP: I.regs.w[SP] = (ushort)val; break;
  case NEC_FLAGS: I.flags = (ushort)val; { I.CarryVal = (int)((val) & 1); I.ParityVal = ((val) & 4)==0?1:0; I.AuxVal = (int)((val) & 16); I.ZeroVal = ((val) & 64)==0?1:0; I.SignVal = ((val) & 128)!=0 ? -1 : 0; I.TF = (byte)(((val) & 256) == 256?1:0); I.IF = (byte)(((val) & 512) == 512?1:0); I.DF = (byte)(((val) & 1024) == 1024?1:0); I.OverVal = (int)((val) & 2048); I.MF = (byte)(((val) & 0x8000) == 0x8000?1:0); }; break;
        case NEC_AW: I.regs.w[AW] = (ushort)val; break;
  case NEC_CW: I.regs.w[CW] = (ushort)val; break;
  case NEC_DW: I.regs.w[DW] = (ushort)val; break;
  case NEC_BW: I.regs.w[BW] = (ushort)val; break;
  case NEC_BP: I.regs.w[BP] = (ushort)val; break;
  case NEC_IX: I.regs.w[IX] = (ushort)val; break;
  case NEC_IY: I.regs.w[IY] = (ushort)val; break;
  case NEC_ES: I.sregs[ES] = (ushort)val; break;
  case NEC_CS: I.sregs[CS] = (ushort)val; break;
  case NEC_SS: I.sregs[SS] = (ushort)val; break;
  case NEC_DS: I.sregs[DS] = (ushort)val; break;
  case NEC_VECTOR: I.int_vector = (byte)val; break;
  case NEC_PENDING: I.pending_irq = (byte)val; break;
  case NEC_NMI_STATE: nec_set_nmi_line((int)val); break;
  case NEC_IRQ_STATE: nec_set_irq_line(0,(int)val); break;
  default:
   if( regnum <= REG_SP_CONTENTS )
   {
    uint offset =(uint)( ((I._base[SS] + I.regs.w[SP])) + 2 * (REG_SP_CONTENTS - regnum));
    cpu_writemem20((int) offset, (int)(val & 0xff ));
    cpu_writemem20((int)( offset+1), (int)(val >> 8) & 0xff );
   }
   break;
    }
}

void nec_set_nmi_line(int state)
{
 if( I.nmi_state == state ) return;
    I.nmi_state =(sbyte) state;
 if (state != CLEAR_LINE)
 {
  I.pending_irq |= NMI_IRQ;
 }
}

void nec_set_irq_line(int irqline, int state)
{
 I.irq_state =(sbyte) state;
 if (state == CLEAR_LINE)
 {
  if (I.IF==0)
   I.pending_irq &= unchecked((byte)~INT_IRQ);
 }
 else
 {
  if (I.IF!=0)
   I.pending_irq |= INT_IRQ;
 }
}

void nec_set_irq_callback(irqcallback callback)
{
 I.irq_callback = callback;
}

int nec_execute(int cycles)
{
 nec_ICount[0]=cycles; /* ASG 971222 cycles_per_run;*/
 while(nec_ICount[0]>0)
    {

 if ((I.pending_irq !=0&& I.IF!=0) || (I.pending_irq & NMI_IRQ)!=0)
  external_int(); /* HJB 12/15/98 */

 I.seg_prefix=0;

  /* Some compilers cannot handle large case statements */
 switch(((byte)cpu_readop((uint)(I._base[CS]+I.ip++))))
 {
 case 0x00: i_add_br8(); break;
 case 0x01: i_add_wr16(); break;
 case 0x02: i_add_r8b(); break;
 case 0x03: i_add_r16w(); break;
 case 0x04: i_add_ald8(); break;
 case 0x05: i_add_axd16(); break;
 case 0x06: i_push_es(); break;
 case 0x07: i_pop_es(); break;
 case 0x08: i_or_br8(); break;
 case 0x09: i_or_wr16(); break;
 case 0x0a: i_or_r8b(); break;
 case 0x0b: i_or_r16w(); break;
 case 0x0c: i_or_ald8(); break;
 case 0x0d: i_or_axd16(); break;
 case 0x0e: i_push_cs(); break;
 case 0x0f: i_pre_nec(); break;
 case 0x10: i_adc_br8(); break;
 case 0x11: i_adc_wr16(); break;
 case 0x12: i_adc_r8b(); break;
 case 0x13: i_adc_r16w(); break;
 case 0x14: i_adc_ald8(); break;
 case 0x15: i_adc_axd16(); break;
 case 0x16: i_push_ss(); break;
 case 0x17: i_pop_ss(); break;
 case 0x18: i_sbb_br8(); break;
 case 0x19: i_sbb_wr16(); break;
 case 0x1a: i_sbb_r8b(); break;
 case 0x1b: i_sbb_r16w(); break;
 case 0x1c: i_sbb_ald8(); break;
 case 0x1d: i_sbb_axd16(); break;
 case 0x1e: i_push_ds(); break;
 case 0x1f: i_pop_ds(); break;
 case 0x20: i_and_br8(); break;
 case 0x21: i_and_wr16(); break;
 case 0x22: i_and_r8b(); break;
 case 0x23: i_and_r16w(); break;
 case 0x24: i_and_ald8(); break;
 case 0x25: i_and_axd16(); break;
 case 0x26: i_es(); break;
 case 0x27: i_daa(); break;
 case 0x28: i_sub_br8(); break;
 case 0x29: i_sub_wr16(); break;
 case 0x2a: i_sub_r8b(); break;
 case 0x2b: i_sub_r16w(); break;
 case 0x2c: i_sub_ald8(); break;
 case 0x2d: i_sub_axd16(); break;
 case 0x2e: i_cs(); break;
 case 0x2f: i_das(); break;
 case 0x30: i_xor_br8(); break;
 case 0x31: i_xor_wr16(); break;
 case 0x32: i_xor_r8b(); break;
 case 0x33: i_xor_r16w(); break;
 case 0x34: i_xor_ald8(); break;
 case 0x35: i_xor_axd16(); break;
 case 0x36: i_ss(); break;
 case 0x37: i_aaa(); break;
 case 0x38: i_cmp_br8(); break;
 case 0x39: i_cmp_wr16(); break;
 case 0x3a: i_cmp_r8b(); break;
 case 0x3b: i_cmp_r16w(); break;
 case 0x3c: i_cmp_ald8(); break;
 case 0x3d: i_cmp_axd16(); break;
 case 0x3e: i_ds(); break;
 case 0x3f: i_aas(); break;
 case 0x40: i_inc_ax(); break;
 case 0x41: i_inc_cx(); break;
 case 0x42: i_inc_dx(); break;
 case 0x43: i_inc_bx(); break;
 case 0x44: i_inc_sp(); break;
 case 0x45: i_inc_bp(); break;
 case 0x46: i_inc_si(); break;
 case 0x47: i_inc_di(); break;
 case 0x48: i_dec_ax(); break;
 case 0x49: i_dec_cx(); break;
 case 0x4a: i_dec_dx(); break;
 case 0x4b: i_dec_bx(); break;
 case 0x4c: i_dec_sp(); break;
 case 0x4d: i_dec_bp(); break;
 case 0x4e: i_dec_si(); break;
 case 0x4f: i_dec_di(); break;
 case 0x50: i_push_ax(); break;
 case 0x51: i_push_cx(); break;
 case 0x52: i_push_dx(); break;
 case 0x53: i_push_bx(); break;
 case 0x54: i_push_sp(); break;
 case 0x55: i_push_bp(); break;
 case 0x56: i_push_si(); break;
 case 0x57: i_push_di(); break;
 case 0x58: i_pop_ax(); break;
 case 0x59: i_pop_cx(); break;
 case 0x5a: i_pop_dx(); break;
 case 0x5b: i_pop_bx(); break;
 case 0x5c: i_pop_sp(); break;
 case 0x5d: i_pop_bp(); break;
 case 0x5e: i_pop_si(); break;
 case 0x5f: i_pop_di(); break;
        case 0x60: i_pusha(); break;
        case 0x61: i_popa(); break;
        case 0x62: i_bound(); break;
 case 0x63: i_invalid(); break;
 case 0x64: i_repnc(); break;
 case 0x65: i_repc(); break;
 case 0x66: i_invalid(); break;
 case 0x67: i_invalid(); break;
        case 0x68: i_push_d16(); break;
        case 0x69: i_imul_d16(); break;
        case 0x6a: i_push_d8(); break;
        case 0x6b: i_imul_d8(); break;
        case 0x6c: i_insb(); break;
        case 0x6d: i_insw(); break;
        case 0x6e: i_outsb(); break;
        case 0x6f: i_outsw(); break;
 case 0x70: i_jo(); break;
 case 0x71: i_jno(); break;
 case 0x72: i_jb(); break;
 case 0x73: i_jnb(); break;
 case 0x74: i_jz(); break;
 case 0x75: i_jnz(); break;
 case 0x76: i_jbe(); break;
 case 0x77: i_jnbe(); break;
 case 0x78: i_js(); break;
 case 0x79: i_jns(); break;
 case 0x7a: i_jp(); break;
 case 0x7b: i_jnp(); break;
 case 0x7c: i_jl(); break;
 case 0x7d: i_jnl(); break;
 case 0x7e: i_jle(); break;
 case 0x7f: i_jnle(); break;
 case 0x80: i_80pre(); break;
 case 0x81: i_81pre(); break;
 case 0x82: i_82pre(); break;
 case 0x83: i_83pre(); break;
 case 0x84: i_test_br8(); break;
 case 0x85: i_test_wr16(); break;
 case 0x86: i_xchg_br8(); break;
 case 0x87: i_xchg_wr16(); break;
 case 0x88: i_mov_br8(); break;
 case 0x89: i_mov_wr16(); break;
 case 0x8a: i_mov_r8b(); break;
 case 0x8b: i_mov_r16w(); break;
 case 0x8c: i_mov_wsreg(); break;
 case 0x8d: i_lea(); break;
 case 0x8e: i_mov_sregw(); break;
 case 0x8f: i_popw(); break;
 case 0x90: i_nop(); break;
 case 0x91: i_xchg_axcx(); break;
 case 0x92: i_xchg_axdx(); break;
 case 0x93: i_xchg_axbx(); break;
 case 0x94: i_xchg_axsp(); break;
 case 0x95: i_xchg_axbp(); break;
 case 0x96: i_xchg_axsi(); break;
 case 0x97: i_xchg_axdi(); break;
 case 0x98: i_cbw(); break;
 case 0x99: i_cwd(); break;
 case 0x9a: i_call_far(); break;
 case 0x9b: i_wait(); break;
 case 0x9c: i_pushf(); break;
 case 0x9d: i_popf(); break;
 case 0x9e: i_sahf(); break;
 case 0x9f: i_lahf(); break;
 case 0xa0: i_mov_aldisp(); break;
 case 0xa1: i_mov_axdisp(); break;
 case 0xa2: i_mov_dispal(); break;
 case 0xa3: i_mov_dispax(); break;
 case 0xa4: i_movsb(); break;
 case 0xa5: i_movsw(); break;
 case 0xa6: i_cmpsb(); break;
 case 0xa7: i_cmpsw(); break;
 case 0xa8: i_test_ald8(); break;
 case 0xa9: i_test_axd16(); break;
 case 0xaa: i_stosb(); break;
 case 0xab: i_stosw(); break;
 case 0xac: i_lodsb(); break;
 case 0xad: i_lodsw(); break;
 case 0xae: i_scasb(); break;
 case 0xaf: i_scasw(); break;
 case 0xb0: i_mov_ald8(); break;
 case 0xb1: i_mov_cld8(); break;
 case 0xb2: i_mov_dld8(); break;
 case 0xb3: i_mov_bld8(); break;
 case 0xb4: i_mov_ahd8(); break;
 case 0xb5: i_mov_chd8(); break;
 case 0xb6: i_mov_dhd8(); break;
 case 0xb7: i_mov_bhd8(); break;
 case 0xb8: i_mov_axd16(); break;
 case 0xb9: i_mov_cxd16(); break;
 case 0xba: i_mov_dxd16(); break;
 case 0xbb: i_mov_bxd16(); break;
 case 0xbc: i_mov_spd16(); break;
 case 0xbd: i_mov_bpd16(); break;
 case 0xbe: i_mov_sid16(); break;
 case 0xbf: i_mov_did16(); break;
        case 0xc0: i_rotshft_bd8(); break;
        case 0xc1: i_rotshft_wd8(); break;
 case 0xc2: i_ret_d16(); break;
 case 0xc3: i_ret(); break;
 case 0xc4: i_les_dw(); break;
 case 0xc5: i_lds_dw(); break;
 case 0xc6: i_mov_bd8(); break;
 case 0xc7: i_mov_wd16(); break;
        case 0xc8: i_enter(); break;
        case 0xc9: i_leave(); break;
 case 0xca: i_retf_d16(); break;
 case 0xcb: i_retf(); break;
 case 0xcc: i_int3(); break;
 case 0xcd: i_int(); break;
 case 0xce: i_into(); break;
 case 0xcf: i_iret(); break;
        case 0xd0: i_rotshft_b(); break;
        case 0xd1: i_rotshft_w(); break;
        case 0xd2: i_rotshft_bcl(); break;
        case 0xd3: i_rotshft_wcl(); break;
 case 0xd4: i_aam(); break;
 case 0xd5: i_aad(); break;
 case 0xd6: i_setalc(); break;
 case 0xd7: i_xlat(); break;
 case 0xd8: i_escape(); break;
 case 0xd9: i_escape(); break;
 case 0xda: i_escape(); break;
 case 0xdb: i_escape(); break;
 case 0xdc: i_escape(); break;
 case 0xdd: i_escape(); break;
 case 0xde: i_escape(); break;
 case 0xdf: i_escape(); break;
 case 0xe0: i_loopne(); break;
 case 0xe1: i_loope(); break;
 case 0xe2: i_loop(); break;
 case 0xe3: i_jcxz(); break;
 case 0xe4: i_inal(); break;
 case 0xe5: i_inax(); break;
 case 0xe6: i_outal(); break;
 case 0xe7: i_outax(); break;
 case 0xe8: i_call_d16(); break;
 case 0xe9: i_jmp_d16(); break;
 case 0xea: i_jmp_far(); break;
 case 0xeb: i_jmp_d8(); break;
 case 0xec: i_inaldx(); break;
 case 0xed: i_inaxdx(); break;
 case 0xee: i_outdxal(); break;
 case 0xef: i_outdxax(); break;
 case 0xf0: i_lock(); break;
 case 0xf1: i_invalid(); break;
 case 0xf2: i_repne(); break;
 case 0xf3: i_repe(); break;
 case 0xf4: i_hlt(); break;
 case 0xf5: i_cmc(); break;
 case 0xf6: i_f6pre(); break;
 case 0xf7: i_f7pre(); break;
 case 0xf8: i_clc(); break;
 case 0xf9: i_stc(); break;
 case 0xfa: i_di(); break;
 case 0xfb: i_ei(); break;
 case 0xfc: i_cld(); break;
 case 0xfd: i_std(); break;
 case 0xfe: i_fepre(); break;
 case 0xff: i_ffpre(); break;
 };




//if (errorlog && cpu_get_pc()>0xc0000) fprintf(errorlog,"CPU %05x\n",cpu_get_pc());

    }
 return cycles - nec_ICount[0];
}


uint nec_dasm(ref string buffer, uint pc)
{



 buffer = sprintf( "$%02X", cpu_readop(pc) );
 return 1;

}

/* Wrappers for the different CPU types */
void v20_reset(object param) { nec_reset(param); }
void v20_exit() { nec_exit(); }
int v20_execute(int cycles) { return nec_execute(cycles); }
uint v20_get_context(ref object dst) { return nec_get_context(ref dst); }
void v20_set_context(object src) { nec_set_context(src); }
uint v20_get_pc() { return nec_get_pc(); }
void v20_set_pc(uint val) { nec_set_pc(val); }
uint v20_get_sp() { return nec_get_sp(); }
void v20_set_sp(uint val) { nec_set_sp(val); }
uint v20_get_reg(int regnum) { return nec_get_reg(regnum); }
void v20_set_reg(int regnum, uint val) { nec_set_reg(regnum,val); }
void v20_set_nmi_line(int state) { nec_set_nmi_line(state); }
void v20_set_irq_line(int irqline, int state) { nec_set_irq_line(irqline,state); }
void v20_set_irq_callback(irqcallback callback) { nec_set_irq_callback(callback); }
string v20_info(object context, int regnum)
{


    switch( regnum )
    {

        case CPU_INFO_NAME: return "V20";
        case CPU_INFO_FAMILY: return "NEC V-Series";
        case CPU_INFO_VERSION: return "1.6";
        case CPU_INFO_FILE: return "nec.c";
        case CPU_INFO_CREDITS: return "Real mode NEC emulator v1.3 by Oliver Bergmann\n(initial work based on Fabrice Fabian's i86 core)";
    }
    throw new Exception();
}
uint v20_dasm(ref string buffer, uint pc) { return nec_dasm(ref buffer,pc); }

public void v30_reset(object param) { nec_reset(param); }
public void v30_exit() { nec_exit(); }
public int v30_execute(int cycles) { return nec_execute(cycles); }
public uint v30_get_context(ref object dst) { return nec_get_context(ref dst); }
public void v30_set_context(object src) { nec_set_context(src); }
public uint v30_get_pc() { return nec_get_pc(); }
public void v30_set_pc(uint val) { nec_set_pc(val); }
public uint v30_get_sp() { return nec_get_sp(); }
public void v30_set_sp(uint val) { nec_set_sp(val); }
public uint v30_get_reg(int regnum) { return nec_get_reg(regnum); }
public void v30_set_reg(int regnum, uint val) { nec_set_reg(regnum, val); }
public void v30_set_nmi_line(int state) { nec_set_nmi_line(state); }
public void v30_set_irq_line(int irqline, int state) { nec_set_irq_line(irqline, state); }
public void v30_set_irq_callback(irqcallback callback) { nec_set_irq_callback(callback); }
public string v30_info(object context, int regnum)
{


    switch( regnum )
    {

        case CPU_INFO_NAME: return "V30";
        case CPU_INFO_FAMILY: return "NEC V-Series";
        case CPU_INFO_VERSION: return "1.6";
        case CPU_INFO_FILE: return "nec.c";
        case CPU_INFO_CREDITS: return "Real mode NEC emulator v1.3 by Oliver Bergmann\n(initial work based on Fabrice Fabian's i86 core)";
    }
    throw new Exception();
}
uint v30_dasm(ref string buffer, uint pc) { return nec_dasm(ref buffer,pc); }

void v33_reset(object param) { nec_reset(param); }
void v33_exit() { nec_exit(); }
int v33_execute(int cycles) { return nec_execute(cycles); }
uint v33_get_context(ref object dst) { return nec_get_context(ref dst); }
void v33_set_context(object src) { nec_set_context(src); }
uint v33_get_pc() { return nec_get_pc(); }
void v33_set_pc(uint val) { nec_set_pc(val); }
uint v33_get_sp() { return nec_get_sp(); }
void v33_set_sp(uint val) { nec_set_sp(val); }
uint v33_get_reg(int regnum) { return nec_get_reg(regnum); }
void v33_set_reg(int regnum, uint val) { nec_set_reg(regnum,val); }
void v33_set_nmi_line(int state) { nec_set_nmi_line(state); }
void v33_set_irq_line(int irqline, int state) { nec_set_irq_line(irqline,state); }
void v33_set_irq_callback(irqcallback callback) { nec_set_irq_callback(callback); }
string v33_info(object context, int regnum)
{

    switch( regnum )
    {

        case CPU_INFO_NAME: return "V33";
        case CPU_INFO_FAMILY: return "NEC V-Series";
        case CPU_INFO_VERSION: return "1.6";
        case CPU_INFO_FILE: return "nec.c";
        case CPU_INFO_CREDITS: return "Real mode NEC emulator v1.3 by Oliver Bergmann\n(initial work based on Fabrice Fabian's i86 core)";
    }
    throw new Exception();
}
uint v33_dasm(ref string buffer, uint pc) { return nec_dasm(ref buffer,pc); }


  }
 }
}
