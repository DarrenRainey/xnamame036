using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public static sCPU cpu = new sCPU();

        public partial class memory
        {
            public static MMIOAccess mmio = new MMIOAccess();
            public static StaticRAM wram = new StaticRAM(128 * 1024);
            public static StaticRAM apuram = new StaticRAM(64 * 1024);
            public static StaticRAM vram = new StaticRAM(64 * 1024);
            public static StaticRAM oam = new StaticRAM(544);
            public static StaticRAM cgram = new StaticRAM(512);

            public static UnmappedMemory memory_unmapped = new UnmappedMemory();
            public static UnmappedMMIO mmio_unmapped = new UnmappedMMIO();
        }
        public partial class sCPU : CPUcore
        {
            public byte cpu_version;
            public PriorityQueue _event ;
            public struct Status
            {
                public bool interrupt_pending;
                public ushort interrupt_vector;

                public uint clock_count;
                public uint line_clocks;

                //timing
                public bool irq_lock;
                public bool alu_lock;
                public uint dram_refresh_position;

                public bool nmi_valid;
                public bool nmi_line;
                public bool nmi_transition;
                public bool nmi_pending;
                public bool nmi_hold;

                public bool irq_valid;
                public bool irq_line;
                public bool irq_transition;
                public bool irq_pending;
                public bool irq_hold;

                public bool reset_pending;

                //DMA
                public bool dma_active;
                public uint dma_counter;
                public uint dma_clocks;
                public bool dma_pending;
                public bool hdma_pending;
                public bool hdma_mode;  //0 = init, 1 = run

                //MMIO

                //$2181-$2183
                public uint wram_addr;

                //$4016-$4017
                public bool joypad_strobe_latch;
                public uint joypad1_bits;
                public uint joypad2_bits;

                //$4200
                public bool nmi_enabled;
                public bool hirq_enabled, virq_enabled;
                public bool auto_joypad_poll;

                //$4201
                public byte pio;

                //$4202-$4203
                public byte mul_a, mul_b;

                //$4204-$4206
                public ushort div_a;
                public byte div_b;

                //$4207-$420a
                public ushort hirq_pos, virq_pos;

                //$420d
                public uint rom_speed;

                //$4214-$4217
                public ushort r4214;
                public ushort r4216;

                //$4218-$421f
                public byte joy1l, joy1h;
                public byte joy2l, joy2h;
                public byte joy3l, joy3h;
                public byte joy4l, joy4h;
            }
            public Status status;
            
             PPUcounter ppuCounter = new PPUcounter();

            public sCPU()
            {
                _event = new PriorityQueue(500, queue_event);
            }
            public ushort vcounter() { return ppuCounter.vcounter(); }
            public ushort hcounter() { return ppuCounter.hcounter(); }
            public void power()
            {
                cpu_version = (byte)config.cpu.version; // from CPU()
                regs.a.w = regs.x.w = regs.y.w = 0x0000;
                regs.s.w = 0x01ff;

                mmio_power();
                dma_power();
                timing_power();

                reset();
            }
            public override bool interrupt_pending()
            {
                return status.interrupt_pending;
            }
            public void reset()
            {
                throw new Exception();
            }
        }
    }
}
