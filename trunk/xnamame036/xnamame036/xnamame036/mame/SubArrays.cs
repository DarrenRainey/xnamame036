using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class IntSubArray
    {
        public int[] buffer;
        public int offset;

        public IntSubArray(int[] buffer, int offset = 0)
        {
            this.buffer = buffer;
            this.offset = offset;
        }
        public int this[int index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }

    }
    public class UIntSubArray
    {
        uint[] buffer;
        int offset;

        public UIntSubArray(uint[] buffer, int offset = 0)
        {
            this.buffer = buffer;
            this.offset = offset;
        }
        public uint this[int index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }

    }
}
