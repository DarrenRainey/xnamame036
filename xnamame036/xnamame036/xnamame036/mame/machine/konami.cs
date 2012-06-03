using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class Konami
    {
        static byte decodebyte(byte opcode, ushort address)
        {
            /*
            >
            > CPU_D7 = (EPROM_D7 & ~ADDRESS_1) | (~EPROM_D7 & ADDRESS_1)  >
            > CPU_D6 = EPROM_D6
            >
            > CPU_D5 = (EPROM_D5 & ADDRESS_1) | (~EPROM_D5 & ~ADDRESS_1) >
            > CPU_D4 = EPROM_D4
            >
            > CPU_D3 = (EPROM_D3 & ~ADDRESS_3) | (~EPROM_D3 & ADDRESS_3) >
            > CPU_D2 = EPROM_D2
            >
            > CPU_D1 = (EPROM_D1 & ADDRESS_3) | (~EPROM_D1 & ~ADDRESS_3) >
            > CPU_D0 = EPROM_D0
            >
            */
            byte xormask;


            xormask = 0;
            if ((address & 0x02) != 0) xormask |= 0x80;
            else xormask |= 0x20;
            if ((address & 0x08) != 0) xormask |= 0x08;
            else xormask |= 0x02;

            return (byte)(opcode ^ xormask);
        }
        static void decode(int cpu)
        {
            _BytePtr rom = Mame.memory_region(Mame.REGION_CPU1 + cpu);
            int diff = Mame.memory_region_length(Mame.REGION_CPU1 + cpu) / 2;

            Mame.memory_set_opcode_base(cpu, new _BytePtr(rom, diff));

            for (int A = 0; A < diff; A++)
            {
                rom[A + diff] = decodebyte(rom[A], (ushort)A);
            }
            int a = 0;
        }
        public static void konami1_decode()
        {
            decode(0);
        }
        public static void konami1_decode_cpu4()
        {
            decode(3);
        }
    }
}
