using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{

    public class _BytePtr
    {
        public byte bsize = 1;
        public byte[] buffer;
        public int offset;

        public _BytePtr() { }
        public _BytePtr(int size) { buffer = new byte[size]; }
        public _BytePtr(byte[] buffer) : this(buffer, 0) { }
        public _BytePtr(byte[] buffer, int offset) { this.buffer = buffer; this.offset = offset; }
        public _BytePtr(_BytePtr bp, int offset=0) { this.buffer = bp.buffer; this.offset = bp.offset + offset; }
        public void inc(int v = 0)
        {
            if (v == 0) offset += bsize;
            else offset += v * bsize;
        }
        public void dec(int v = 0)
        {
            if (v == 0) offset -= bsize;
            else offset -= v * bsize;
        }
        public void writeinc(byte v) { buffer[ offset++] = v; }
        public byte readinc() { return buffer[offset++]; }
        public byte readdec() { return buffer[offset--]; }
        public void write16inc(ushort value) { buffer[offset] = (byte)value; buffer[offset + 1] = (byte)(value >> 8); offset += 2; }
        
        public virtual byte this[uint index]
        {
            get { if (bsize != 1) throw new Exception(); return buffer[index + offset]; }
            set { if (bsize != 1) throw new Exception(); buffer[index + offset] = value; }
        }
        public virtual byte this[int index]
        {
            get { if (bsize != 1) throw new Exception(); return buffer[index + offset]; }
            set { if (bsize != 1) throw new Exception(); buffer[index + offset] = value; }
        }
        public ushort READ_WORD(int index)
        {
            return BitConverter.ToUInt16(buffer, index + offset);
            //return (ushort)(buffer[offset + 1 + index] << 8 | buffer[offset + index]);
        }
            public void WRITE_WORD(int index, ushort value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, (offset + index), 2);
           //buffer[offset + index] = (byte)value;
            //buffer[offset + index  + 1] = (byte)(value >> 8);
            }
            public uint READ_DWORD(int index)
            {
                return BitConverter.ToUInt32(buffer, index+offset);
            }
        public ushort read16(int index)
        {
            //return BitConverter.ToUInt16(buffer, (int)(offset + (index * 2)));
            return (ushort)(buffer[offset + 1 + index * 2] << 8 | buffer[offset + index * 2]);
        }
        public void write16(int index, ushort value)
        {
            //Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, (int)(offset + (index * 2)), 2);
            buffer[offset + index*2] = (byte)value;
            buffer[offset + index * 2 + 1] = (byte)(value >> 8);
        }
        public void write32(int index, uint value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, (int)(offset + (index * 4)), 4);
        }
        public uint read32(int index)
        {
            return BitConverter.ToUInt32(buffer, (int)(offset + (index * 4)));
        }
    }
    public class _ShortPtr : _BytePtr
    {
        public _ShortPtr(_BytePtr bp) { this.buffer = bp.buffer; this.offset = bp.offset; }
        public _ShortPtr(_ShortPtr sp) { this.buffer = sp.buffer; this.offset = sp.offset; }
        public _ShortPtr(int size)
        {
            bsize = 2;
            buffer = new byte[size];
        }
        public _ShortPtr(byte[] b) { bsize = 2; this.buffer = b; }
        public _ShortPtr(_BytePtr sp, int offset)
        {
            bsize = 2;
            this.buffer = sp.buffer;
            this.offset = sp.offset+offset;
        }
    }
    public class _IntPtr : _BytePtr
    {
        public _IntPtr() { }
        public _IntPtr(int size)
        {
            bsize = 4;
            buffer = new byte[size];
        }
        public _IntPtr(_BytePtr bp,int offset=0)
        {
            bsize = 4;
            this.buffer = bp.buffer;
            this.offset = bp.offset+offset;
        }
        public _IntPtr(_IntPtr ip, int offset)
        {
            this.buffer = ip.buffer;
            this.offset=offset;
        }
        

        //public new uint this[int index]
        //{
        //    get { return BitConverter.ToUInt32(buffer, index * 4); }
        //    set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, (int)(offset + index*4), 4); }
        //}
    }
}

