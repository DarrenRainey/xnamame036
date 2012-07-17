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

        public IntSubArray(int size)
        {
            this.buffer = new int[size];
            this.offset = 0;
        }
        public IntSubArray(IntSubArray subarray, int offset = 0)
        {
            this.buffer = subarray.buffer;
            this.offset = subarray.offset + offset;
        }
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

        public UIntSubArray(UIntSubArray subarray, int offset = 0)
        {
            this.buffer = subarray.buffer;
            this.offset = subarray.offset + offset;
        }
        public UIntSubArray(uint[] buffer, int offset = 0)
        {
            this.buffer = buffer;
            this.offset = offset;
        }
        public UIntSubArray(uint[] buffer, uint offset )
        {
            this.buffer = buffer;
            this.offset = (int)offset;
        }
        public uint this[int index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }
        public uint this[uint index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }
    }
    public class UShortSubArray
    {
        ushort[] buffer;
        public int offset;

        public UShortSubArray(int size) { this.buffer = new ushort[size]; this.offset = 0; }
        public UShortSubArray(UShortSubArray subarray, int offset = 0)
        {
            this.buffer = subarray.buffer;
            this.offset = subarray.offset + offset;
        }
        public UShortSubArray(ushort[] buffer, int offset = 0)
        {
            this.buffer = buffer;
            this.offset = offset;
        }
        public ushort this[int index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }
    }
    public class ShortSubArray
    {
        short[] buffer;
        public int offset;

        public ShortSubArray(int size) { this.buffer = new short[size]; this.offset = 0; }
        public ShortSubArray(ShortSubArray subarray, int offset = 0)
        {
            this.buffer = subarray.buffer;
            this.offset = subarray.offset + offset;
        }
        public ShortSubArray(short[] buffer, int offset = 0)
        {
            this.buffer = buffer;
            this.offset = offset;
        }
        public short this[int index]
        {
            get { return buffer[index + offset]; }
            set { buffer[index + offset] = value; }
        }
    }
}
