using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class kabuki
    {
        static int bitswap1(int src, int key, int select)
        {
            if ((select & (1 << ((key >> 0) & 7))) != 0)
                src = (src & 0xfc) | ((src & 0x01) << 1) | ((src & 0x02) >> 1);
            if ((select & (1 << ((key >> 4) & 7))) != 0)
                src = (src & 0xf3) | ((src & 0x04) << 1) | ((src & 0x08) >> 1);
            if ((select & (1 << ((key >> 8) & 7))) != 0)
                src = (src & 0xcf) | ((src & 0x10) << 1) | ((src & 0x20) >> 1);
            if ((select & (1 << ((key >> 12) & 7))) != 0)
                src = (src & 0x3f) | ((src & 0x40) << 1) | ((src & 0x80) >> 1);

            return src;
        }

        static int bitswap2(int src, int key, int select)
        {
            if ((select & (1 << ((key >> 12) & 7))) != 0)
                src = (src & 0xfc) | ((src & 0x01) << 1) | ((src & 0x02) >> 1);
            if ((select & (1 << ((key >> 8) & 7))) != 0)
                src = (src & 0xf3) | ((src & 0x04) << 1) | ((src & 0x08) >> 1);
            if ((select & (1 << ((key >> 4) & 7))) != 0)
                src = (src & 0xcf) | ((src & 0x10) << 1) | ((src & 0x20) >> 1);
            if ((select & (1 << ((key >> 0) & 7))) != 0)
                src = (src & 0x3f) | ((src & 0x40) << 1) | ((src & 0x80) >> 1);

            return src;
        }

        static int bytedecode(int src, int swap_key1, int swap_key2, int xor_key, int select)
        {
            src = bitswap1(src, swap_key1 & 0xffff, select & 0xff);
            src = ((src & 0x7f) << 1) | ((src & 0x80) >> 7);
            src = bitswap2(src, swap_key1 >> 16, select & 0xff);
            src ^= xor_key;
            src = ((src & 0x7f) << 1) | ((src & 0x80) >> 7);
            src = bitswap2(src, swap_key2 & 0xffff, select >> 8);
            src = ((src & 0x7f) << 1) | ((src & 0x80) >> 7);
            src = bitswap1(src, swap_key2 >> 16, select >> 8);
            return src;
        }
        static void kabuki_decode(_BytePtr src, _BytePtr dest_op, _BytePtr dest_data, int base_addr, int length, int swap_key1, int swap_key2, int addr_key, int xor_key)
        {
            for (int A = 0; A < length; A++)
            {
                /* decode opcodes */
                int select = (A + base_addr) + addr_key;
                dest_op[A] = (byte)bytedecode(src[A], swap_key1, swap_key2, xor_key, select);

                /* decode data */
                select = ((A + base_addr) ^ 0x1fc0) + addr_key + 1;
                dest_data[A] = (byte)bytedecode(src[A], swap_key1, swap_key2, xor_key, select);
            }
        }
        static void mitchell_decode(int swap_key1, int swap_key2, int addr_key, int xor_key)
        {
            _BytePtr rom = Mame.memory_region(Mame.REGION_CPU1);
            int diff = Mame.memory_region_length(Mame.REGION_CPU1) / 2;

            Mame.memory_set_opcode_base(0, new _BytePtr(rom, diff));
            kabuki_decode(rom, new _BytePtr(rom, diff), rom, 0x0000, 0x8000, swap_key1, swap_key2, addr_key, xor_key);
            for (int i = 0x10000; i < diff; i += 0x4000)
                kabuki_decode(new _BytePtr(rom, i), new _BytePtr(rom, i + diff), new _BytePtr(rom, i), 0x8000, 0x4000, swap_key1, swap_key2, addr_key, xor_key);
        }
        public static void pang_decode() { mitchell_decode(0x01234567, 0x76543210, 0x6548, 0x24); }
    }
}
