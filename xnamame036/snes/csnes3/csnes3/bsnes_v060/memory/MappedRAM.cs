using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    class MappedRAM:Memory
    {
        byte[] data_;
        uint size_;
        bool write_protect_;

        public MappedRAM()
        {
            data_ = null;
            size_ = unchecked((uint)-1);
            write_protect_ = false;
        }
        public void reset()
        {
            data_ = null;
            size_ = unchecked((uint)-1);
            write_protect_ = false;
        }
        public void map(byte[] source, uint length)
        {
            reset();
            data_=source;
            size_ = data_ !=null&& length > 0 ? length : unchecked((uint)-1);
        }
        public void copy(byte[] data, uint size)
        {
            if (data_ == null)
            {
                size_ = (uint)((size & ~255) + (((size & 255)!=0?1:0) << 8));
                data_ = new byte[size_];
            }
            Array.Copy(data, data_, Math.Min(size_, size));
        }
        public void write_protect(bool status) { write_protect_ = status; }
        public byte[] data() { return data_; }
        public override uint size() { return size_; }
        public override byte read(uint addr)
        {
            return data_[addr];
        }
        public override void write(uint addr, byte data)
        {
            if (!write_protect_) data_[addr] = data;
        }
        public byte this[uint addr]
        {
            get { return data_[addr]; }
            set { data_[addr] = value; }
        }

    }
}
