using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class irem
    {

        public static Mame.MemoryReadAddress[] irem_sound_readmem =
{
    new Mame.MemoryReadAddress( 0x0000, 0x001f, Mame.cpu_m6803.m6803_internal_registers_r ),
    new Mame.MemoryReadAddress( 0x0080, 0x00ff, Mame.MRA_RAM ),
    new Mame.MemoryReadAddress( 0x4000, 0xffff, Mame.MRA_ROM ),
    new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        public static Mame.MemoryWriteAddress[] irem_sound_writemem =
{
    new Mame.MemoryWriteAddress( 0x0000, 0x001f, Mame.cpu_m6803.m6803_internal_registers_w ),
    new Mame.MemoryWriteAddress( 0x0080, 0x00ff, Mame.MWA_RAM ),
    new Mame.MemoryWriteAddress( 0x0801, 0x0802, MSM5205.MSM5205_data_w ),
    new Mame.MemoryWriteAddress( 0x9000, 0x9000, Mame.MWA_NOP ),    /* IACK */
    new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
    new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        public static Mame.IOReadPort[] irem_sound_readport = 
        {
            new Mame.IOReadPort(Mame.cpu_m6803.M6803_PORT1, Mame.cpu_m6803.M6803_PORT1, irem_port1_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
        };
        public static Mame.IOWritePort[] irem_sound_writeport =
{
	new Mame.IOWritePort( Mame.cpu_m6803.M6803_PORT1, Mame.cpu_m6803.M6803_PORT1, irem_port1_w ),
	new Mame.IOWritePort( Mame.cpu_m6803.M6803_PORT2, Mame.cpu_m6803.M6803_PORT2, irem_port2_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};
        public static AY8910interface irem_ay8910_interface =
            new AY8910interface(2,910000,new int[]{20,20},new AY8910portRead[]{Mame.soundlatch_r,null},new AY8910portRead[]{null,null},
                new AY8910portWrite[]{null,null},new AY8910portWrite[]{irem_adpcm_reset_w,null},new AY8910handler[]{null,null});

        public static MSM5205_interface irem_msm5205_interface = new MSM5205_interface(2, 384000, new MSM5205.irqcallback[] { irem_adpcm_int, null }, new int[] { MSM5205.MSM5205_S96_4B, MSM5205.MSM5205_S96_4B }, new int[] { 100, 100 });

        public static void irem_sound_cmd_w(int offset, int data)
        {
            if ((data & 0x80) == 0)
                Mame.soundlatch_w(0, data & 0x7f);
            else
                Mame.cpu_set_irq_line(1, 0, Mame.HOLD_LINE);
        }
        static int port1, port2;

        static void irem_port1_w(int offset, int data)
        {
            port1 = data;
        }

        static void irem_port2_w(int offset, int data)
        {
            /* write latch */
            if ((port2 & 0x01) != 0 && (data & 0x01) == 0)
            {
                /* control or data port? */
                if ((port2 & 0x04) != 0)
                {
                    /* PSG 0 or 1? */
                    if ((port2 & 0x10) != 0)
                        ay8910.AY8910_control_port_1_w(0, port1);
                    else if ((port2 & 0x08) != 0)
                        ay8910.AY8910_control_port_0_w(0, port1);
                }
                else
                {
                    /* PSG 0 or 1? */
                    if ((port2 & 0x10) != 0)
                        ay8910.AY8910_write_port_1_w(0, port1);
                    else if ((port2 & 0x08) != 0)
                        ay8910.AY8910_write_port_0_w(0, port1);
                }
            }
            port2 = data;
        }

        static int irem_port1_r(int offset)
        {
            /* PSG 0 or 1? */
            if ((port2 & 0x10) != 0)
                return ay8910.AY8910_read_port_1_r(0);
            else if ((port2 & 0x08) != 0)
                return ay8910.AY8910_read_port_0_r(0);
            else return 0xff;
        }

        static void irem_adpcm_reset_w(int offset, int data)
        {
            MSM5205.MSM5205_reset_w(0, data & 1);
            MSM5205.MSM5205_reset_w(1, data & 2);
        }
        static void irem_adpcm_int(int data)
        {
            Mame.cpu_set_nmi_line(1, Mame.PULSE_LINE);
        }
    }
}
