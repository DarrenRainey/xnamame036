using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class Seibu
    {
        public static int sound_cpu;
        public static _BytePtr seibu_shared_sound_ram = new _BytePtr(1);
        const byte VECTOR_INIT = 0, RST10_ASSERT = 1, RST10_CLEAR = 2, RST18_ASSERT = 3, RST18_CLEAR = 4;

        public static void seibu_bank_w(int offset, int data)
        {
            _BytePtr RAM;

            if (sound_cpu == 1) RAM = Mame.memory_region(Mame.REGION_CPU2);
            else RAM = Mame.memory_region(Mame.REGION_CPU3);

            if ((data & 1) != 0) { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x0000)); }
            else { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000)); }
        }
        public static int seibu_soundlatch_r(int offset)
        {
            return seibu_shared_sound_ram[offset << 1];
        }
        static int irq1, irq2;
        static void setvector_callback(int param)
        {

            switch (param)
            {
                case VECTOR_INIT:
                    irq1 = irq2 = 0xff;
                    break;

                case RST10_ASSERT:
                    irq1 = 0xd7;
                    break;

                case RST10_CLEAR:
                    irq1 = 0xff;
                    break;

                case RST18_ASSERT:
                    irq2 = 0xdf;
                    break;

                case RST18_CLEAR:
                    irq2 = 0xff;
                    break;
            }

            Mame.cpu_irq_line_vector_w(sound_cpu, 0, irq1 & irq2);
            if ((irq1 & irq2) == 0xff)	/* no IRQs pending */
                Mame.cpu_set_irq_line(sound_cpu, 0, Mame.CLEAR_LINE);
            else	/* IRQ pending */
                Mame.cpu_set_irq_line(sound_cpu, 0, Mame.ASSERT_LINE);
        }

        public static void seibu_rst10_ack(int offset, int data)
        {
            /* Unused for now */
        }
        public static void seibu_soundclear_w(int offset, int data)
        {
            seibu_shared_sound_ram[0] = (byte)data;
        }

        public static void seibu_soundlatch_w(int offset, int data)
        {
            seibu_shared_sound_ram[offset] = (byte)data;
            if (offset == 0xc && seibu_shared_sound_ram[0] != 0)
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, RST18_ASSERT, setvector_callback);
        }
        public static void seibu_rst18_ack(int offset, int data)
        {
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, RST18_CLEAR, setvector_callback);
        }
        public static void seibu_main_data_w(int offset, int data)
        {
            seibu_shared_sound_ram[offset << 1] = (byte)data;
        }
        public static void seibu_ym3812_irqhandler(int linestate)
        {
            if (linestate != 0)
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, RST10_ASSERT, setvector_callback);
            else
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, RST10_CLEAR, setvector_callback);
        }
        public static void seibu_sound_init_2()
        {
            sound_cpu = 2;
            setvector_callback(VECTOR_INIT);
        }
        static int sound_cpu_spin(int offset)
{
	_BytePtr RAM;

	if (sound_cpu==1) RAM = Mame.memory_region(Mame.REGION_CPU2);
    else RAM = Mame.memory_region(Mame.REGION_CPU3);

    if (Mame.cpu_get_pc() == 0x129 && RAM[0x201c] == 0)
        Mame.cpu_spinuntil_int();

	return RAM[0x201c+offset];
}
        public static void install_seibu_sound_speedup(int cpu)
        {
            Mame.install_mem_read_handler(cpu, 0x201c, 0x201d, sound_cpu_spin);
        }
    }
}
