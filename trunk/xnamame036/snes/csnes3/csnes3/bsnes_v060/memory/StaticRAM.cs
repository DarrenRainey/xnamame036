using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    class StaticRAM:Memory
    {
        byte[] data_;
        uint size_;

        public StaticRAM(uint n) { size_ = n; data_ = new byte[size_]; }
        public override uint size()
        {
            return 0;
        }
        public byte[] data() { return data_; }
        public override byte read(uint addr)
        {
            return data_[addr];
        }
        public override void write(uint addr, byte data)
        {
            data_[addr] = data;
        }
        public byte this[uint addr]
        {
            get { return data_[addr]; }
            set { data_[addr] = value; }
        }
    }
}
