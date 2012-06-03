using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class timeplt
    {

        public static Mame.MemoryReadAddress[] timeplt_sound_readmem =
{
	new  Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new  Mame.MemoryReadAddress( 0x2000, 0x23ff, Mame.MRA_RAM ),
	new  Mame.MemoryReadAddress( 0x3000, 0x33ff, Mame.MRA_RAM ),
	new  Mame.MemoryReadAddress( 0x4000, 0x4000, ay8910.AY8910_read_port_0_r ),
	new  Mame.MemoryReadAddress( 0x6000, 0x6000, ay8910.AY8910_read_port_1_r ),
	new  Mame.MemoryReadAddress( -1 )	/* end of table */
};

        public static Mame.MemoryWriteAddress[] timeplt_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x23ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x3000, 0x33ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x5000, 0x5000, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, ay8910.AY8910_write_port_1_w ),
	new Mame.MemoryWriteAddress( 0x7000, 0x7000, ay8910.AY8910_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8fff, timeplt_filter_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        public static AY8910interface timeplt_ay8910_interface =
       new AY8910interface(
           2,				/* 2 chips */
           14318180 / 8,		/* 1.789772727 MHz */
           new int[] { Mame.MIXERG(30, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER), Mame.MIXERG(30, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER) },
           new AY8910portRead[] { Mame.soundlatch_r, null },
           new AY8910portRead[] { timeplt_portB_r, null },
           new AY8910portWrite[] { null, null },
           new AY8910portWrite[] { null, null }, null
       );
        static int[] timeplt_timer =
{
	0x00, 0x10, 0x20, 0x30, 0x40, 0x90, 0xa0, 0xb0, 0xa0, 0xd0
};

        static int last_totalcycles = 0;

        /* number of Z80 clock cycles to count */
        static int clock;
        static int timeplt_portB_r(int offset)
{
	/* need to protect from totalcycles overflow */
	

	int current_totalcycles;

	current_totalcycles = Mame.cpu_gettotalcycles();
	clock = (clock + (current_totalcycles-last_totalcycles)) % 5120;

	last_totalcycles = current_totalcycles;

	return timeplt_timer[clock/512];
}
        static int last;
        public static void timeplt_sh_irqtrigger_w(int offset, int data)
        {

            if (last == 0 && data != 0)
            {
                /* setting bit 0 low then high triggers IRQ on the sound CPU */
                Mame.cpu_cause_interrupt(1, 0xff);
            }

            last = data;
        }
        static void filter_w(int chip, int channel, int data)
        {
            int C = 0;

            if ((data & 1) != 0) C += 220000;	/* 220000pF = 0.220uF */
            if ((data & 2) != 0) C += 47000;	/*  47000pF = 0.047uF */
            Mame.set_RC_filter(3 * chip + channel, 1000, 5100, 0, C);
        }

        static void timeplt_filter_w(int offset, int data)
        {
            filter_w(0, 0, (offset >> 6) & 3);
            filter_w(0, 1, (offset >> 8) & 3);
            filter_w(0, 2, (offset >> 10) & 3);
            filter_w(1, 0, (offset >> 0) & 3);
            filter_w(1, 1, (offset >> 2) & 3);
            filter_w(1, 2, (offset >> 4) & 3);
        }

    }
}
