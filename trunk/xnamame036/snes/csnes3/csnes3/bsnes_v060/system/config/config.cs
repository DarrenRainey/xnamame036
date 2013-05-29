using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static Configuration config = new Configuration();

        class Configuration
        {
            uint controller_port1, controller_port2;
            uint expansion_port;
            uint region;

            public struct CPU
            {
                public uint version;
                public uint ntsc_clock_rate;
                public uint pal_clock_rate;
                public uint alu_mul_delay;
                public uint alu_div_delay;
                public uint wram_init_value;
            }
            public CPU cpu;

            public struct SMP
            {
                public uint ntsc_clock_rate;
                public uint pal_clock_rate;
            }
            public SMP smp;

            public struct PPU1
            {
                public uint version;
            }
            public PPU1 ppu1;

            public struct PPU2
            {
                public uint version;
            }
            public PPU2 ppu2;

            public struct SuperFX
            {
                public uint speed;
            }
            public SuperFX superfx;
            public Configuration()
            {
                controller_port1 = (uint)Input.Device.DeviceJoypad;
                controller_port2 = (uint)Input.Device.DeviceJoypad;
                expansion_port = (uint)System.ExpansionPortDevice.ExpansionBSX;
                region = (uint)System.RegionAutodetect.Autodetect;

                cpu.version = 2;
                cpu.ntsc_clock_rate = 21477272;
                cpu.pal_clock_rate = 21281370;
                cpu.alu_mul_delay = 2;
                cpu.alu_div_delay = 2;
                cpu.wram_init_value = 0x55;

                smp.ntsc_clock_rate = 24607104;  //32040.5 * 768
                smp.pal_clock_rate = 24607104;

                ppu1.version = 1;
                ppu2.version = 3;

                superfx.speed = 0;  //0 = auto-select, 1 = force 10.74MHz, 2 = force 21.48MHz
            }
        }
    }
}