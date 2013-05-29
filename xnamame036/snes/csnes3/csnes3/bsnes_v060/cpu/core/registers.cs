using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public class regs_t
        {
            public reg24_t pc = new reg24_t();
            public reg16_t a = new reg16_t(), x = new reg16_t(), y = new reg16_t(), z = new reg16_t(), s = new reg16_t(), d = new reg16_t();
            public flag_t p = new flag_t();
            public byte db;
            public bool e;
            public bool irq;
            public bool wai;
            public byte mdr;

            
        }
        public class flag_t
        {
            public bool n, v, m, x, d, i, z, c;
            public byte Val
            {
                get { return (byte)( (n ? 1 : 0 << 7) + (v ? 1 : 0 << 6) + (m ? 1 : 0 << 5) + (x ? 1 : 0 << 4) + (d ? 1 : 0 << 3) + (i ? 1 : 0 << 2) + (z ? 1 : 0 << 1) + (c ? 1 : 0 << 0)); }
                set
                {
                    n = (value & 0x80) != 0; v = (value & 0x40) != 0; m = (value & 0x20) != 0; x = (value & 0x10) != 0;
                    n = (value & 0x80) != 0; v = (value & 0x40) != 0; m = (value & 0x20) != 0; x = (value & 0x10) != 0;
                    d = (value & 0x08) != 0; i = (value & 0x04) != 0; z = (value & 0x02) != 0; c = (value & 0x01) != 0;
                }
            }
        }
        [StructLayout(LayoutKind.Explicit)]
        public class reg16_t
        {
            [FieldOffset(0)]
            public ushort w;
#if WINDOWS
            [FieldOffset(0)]
            public byte l;
            [FieldOffset(1)]
            public byte h;
#else
[FieldOffset(0)]
            public byte h;
            [FieldOffset(1)]
            public byte l;
#endif
        }
        [StructLayout(LayoutKind.Explicit)]
        public class reg24_t
        {
            [FieldOffset(0)]
            public uint d;
#if WINDOWS
            [FieldOffset(0)]
            public ushort w;
            [FieldOffset(2)]
            public ushort wh;
            [FieldOffset(0)]
            public byte l;
            [FieldOffset(1)]
            public byte h;
            [FieldOffset(2)]
            public byte b;
            [FieldOffset(3)]
            public byte bh;
#else
            [FieldOffset(0)]
            public ushort wh;
            [FieldOffset(2)]
            public ushort w;
            [FieldOffset(0)]
            public byte bh;
            [FieldOffset(1)]
            public byte b;
            [FieldOffset(2)]
            public byte h;
            [FieldOffset(3)]
            public byte l;

#endif
        }
    }
}
