using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class M72
    {
        const byte VECTOR_INIT = 0, YM2151_ASSERT = 1, YM2151_CLEAR = 2, Z80_ASSERT = 3, Z80_CLEAR = 4;

        static int irqvector;
        static int sample_addr;


        static void setvector_callback(int param)
{
	switch(param)
	{
		case VECTOR_INIT:
			irqvector = 0xff;
			break;

		case YM2151_ASSERT:
			irqvector &= 0xef;
			break;

		case YM2151_CLEAR:
			irqvector |= 0x10;
			break;

		case Z80_ASSERT:
			irqvector &= 0xdf;
			break;

		case Z80_CLEAR:
			irqvector |= 0x20;
			break;
	}

	Mame.cpu_irq_line_vector_w(1,0,irqvector);
	if (irqvector == 0xff)	/* no IRQs pending */
		Mame.cpu_set_irq_line(1,0,Mame.CLEAR_LINE);
	else	/* IRQ pending */
		Mame.cpu_set_irq_line(1,0,Mame.ASSERT_LINE);
}

        public static void m72_init_sound()
        {
            setvector_callback(VECTOR_INIT);
        }
        public static void m72_sound_command_w(int offset,int data)
        {
            if (offset == 0)
            {
                Mame.soundlatch_w(offset, data);
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, Z80_ASSERT, setvector_callback);
            }
        }
        public static void m72_sound_irq_ack_w(int offset, int data)
        {
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, Z80_CLEAR, setvector_callback);
        }
        public static int m72_sample_r(int offset)
        {
            return Mame.memory_region(Mame.REGION_SOUND1)[sample_addr];
        }
        public static void m72_sample_w(int offset, int data)
        {
            DAC.DAC_signed_data_w(0, data);
            sample_addr = (sample_addr + 1) & (Mame.memory_region_length(Mame.REGION_SOUND1) - 1);
        }
        public static void vigilant_sample_addr_w(int offset, int data)
        {
            if (offset == 1)
                sample_addr = (sample_addr & 0x00ff) | ((data << 8) & 0xff00);
            else
                sample_addr = (sample_addr & 0xff00) | ((data << 0) & 0x00ff);
        }
        public static void m72_ym2151_irq_handler(int irq)
        {
            if (irq!=0)
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, YM2151_ASSERT, setvector_callback);
            else
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, YM2151_CLEAR, setvector_callback);
        }
    }
}