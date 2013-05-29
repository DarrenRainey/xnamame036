using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    public class Bus
    {
        struct Page
        {
            public Memory access;
            public uint offset;
        }
        Page[] page = new Page[65536];
        public enum MapMode { MapDirect, MapLinear, MapShadow };

        public void map(uint addr, Memory access, uint offset)
        {
            page[addr >> 8].access = access;
            page[addr >> 8].offset = offset - addr;
        }
        public void map(MapMode mode, byte bank_lo, byte bank_hi, ushort addr_lo, ushort addr_hi, Memory access, uint offset=0, uint size=0)
        {
            if (access.size() == unchecked((uint)-1U)) return;

            byte page_lo = (byte)(addr_lo >> 8);
            byte page_hi = (byte)(addr_hi >> 8);
            uint index = 0;

            switch (mode)
            {
                case MapMode.MapDirect:
                    {
                        for (uint bank = bank_lo; bank <= bank_hi; bank++)
                        {
                            for (uint page = page_lo; page <= page_hi; page++)
                            {
                                map((bank << 16) + (page << 8), access, (bank << 16) + (page << 8));
                            }
                        }
                    } break;

                case MapMode.MapLinear:
                    {
                        for (uint bank = bank_lo; bank <= bank_hi; bank++)
                        {
                            for (uint page = page_lo; page <= page_hi; page++)
                            {
                                map((bank << 16) + (page << 8), access, mirror(offset + index, access.size()));
                                index += 256;
                                if (size != 0) index %= size;
                            }
                        }
                    } break;

                case MapMode.MapShadow:
                    {
                        for (uint bank = bank_lo; bank <= bank_hi; bank++)
                        {
                            index += (uint)(page_lo * 256);
                            if (size != 0) index %= size;

                            for (uint page = page_lo; page <= page_hi; page++)
                            {
                                map((bank << 16) + (page << 8), access, mirror(offset + index, access.size()));
                                index += 256;
                                if (size != 0) index %= size;
                            }

                            index += (uint)((255 - page_hi) * 256);
                            if (size != 0) index %= size;
                        }
                    } break;
            }
        }
        public uint mirror(uint addr, uint size)
        {
            uint _base = 0;
            if (size != 0)
            {
                uint mask = 1 << 23;
                while (addr >= size)
                {
                    while ((addr & mask) == 0) mask >>= 1;
                    addr -= mask;
                    if (size > mask)
                    {
                        size -= mask;
                        _base += mask;
                    }
                    mask >>= 1;
                }
                _base += addr;
            }
            return _base;
        }
        public byte read(uint addr)
        {
            return page[addr >> 8].access.read(page[addr >> 8].offset+addr);
        }
        public void write(uint addr, byte data)
        {
            page[addr >> 8].access.write(page[addr >> 8].offset + addr, data);
        }
    }
}
