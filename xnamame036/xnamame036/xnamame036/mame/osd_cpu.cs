using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace xnamame036.mame
{
    partial class Mame
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct PAIR
        {
            [FieldOffset(0)]
            public uint d;
#if !XBOX
            [FieldOffset(0)]
            public ushort wl;
            [FieldOffset(2)]
            public ushort wh;
            [FieldOffset(0)]
            public byte bl;
            [FieldOffset(1)]
            public byte bh;
            [FieldOffset(2)]
            public byte bh2;
            [FieldOffset(3)]
            public byte bh3;
#else
        [FieldOffset(0)]
        public ushort wh;
        [FieldOffset(2)]
        public ushort wl;
        [FieldOffset(0)]
        public byte bh3;
        [FieldOffset(1)]
        public byte bh2;
        [FieldOffset(2)]
        public byte bh;
        [FieldOffset(3)]
        public byte bl;
#endif
        }
    }
}