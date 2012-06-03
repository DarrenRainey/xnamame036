#define NAMCOS1_DIRECT_DRAW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public class driver_namcos1 : Mame.GameDriver
    {
        public static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];

        public override void driver_init()
        {

        }

        static Mame.MemoryReadAddress[] main_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, namcos1.namcos1_0_banked_area0_r ),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, namcos1.namcos1_0_banked_area1_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, namcos1.namcos1_0_banked_area2_r ),
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, namcos1.namcos1_0_banked_area3_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, namcos1.namcos1_0_banked_area4_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xbfff, namcos1.namcos1_0_banked_area5_r ),
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, namcos1.namcos1_0_banked_area6_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, namcos1.namcos1_0_banked_area7_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] main_writemem =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x1fff, namcos1.namcos1_0_banked_area0_w ),
	new  Mame.MemoryWriteAddress( 0x2000, 0x3fff, namcos1.namcos1_0_banked_area1_w ),
	new  Mame.MemoryWriteAddress( 0x4000, 0x5fff, namcos1.namcos1_0_banked_area2_w ),
	new  Mame.MemoryWriteAddress( 0x6000, 0x7fff, namcos1.namcos1_0_banked_area3_w ),
	new  Mame.MemoryWriteAddress( 0x8000, 0x9fff, namcos1.namcos1_0_banked_area4_w ),
	new  Mame.MemoryWriteAddress( 0xa000, 0xbfff, namcos1.namcos1_0_banked_area5_w ),
	new  Mame.MemoryWriteAddress( 0xc000, 0xdfff, namcos1.namcos1_0_banked_area6_w ),
	new  Mame.MemoryWriteAddress( 0xe000, 0xefff, namcos1.namcos1_bankswitch_w ),
	new  Mame.MemoryWriteAddress( 0xf000, 0xf000, namcos1.namcos1_cpu_control_w ),
	new  Mame.MemoryWriteAddress( 0xf200, 0xf200, Mame.MWA_NOP ), /* watchdog? */
//	new  Mame.MemoryWriteAddress( 0xf400, 0xf400, MWA_NOP }, /* unknown */
//	new  Mame.MemoryWriteAddress( 0xf600, 0xf600, MWA_NOP }, /* unknown */
	new  Mame.MemoryWriteAddress( 0xfc00, 0xfc01, namcos1.namcos1_subcpu_bank ),
	new  Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sub_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, namcos1.namcos1_1_banked_area0_r ),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, namcos1.namcos1_1_banked_area1_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, namcos1.namcos1_1_banked_area2_r ),
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, namcos1.namcos1_1_banked_area3_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, namcos1.namcos1_1_banked_area4_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xbfff, namcos1.namcos1_1_banked_area5_r ),
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, namcos1.namcos1_1_banked_area6_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, namcos1.namcos1_1_banked_area7_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sub_writemem =
{
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, namcos1.namcos1_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, namcos1.namcos1_1_banked_area0_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, namcos1.namcos1_1_banked_area1_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x5fff, namcos1.namcos1_1_banked_area2_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, namcos1.namcos1_1_banked_area3_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, namcos1.namcos1_1_banked_area4_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xbfff, namcos1.namcos1_1_banked_area5_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdfff, namcos1.namcos1_1_banked_area6_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf000, Mame.MWA_NOP ), /* IO Chip */
	new Mame.MemoryWriteAddress( 0xf200, 0xf200, Mame.MWA_NOP ), /* watchdog? */
//	new Mame.MemoryWriteAddress( 0xf400, 0xf400, MWA_NOP ), /* unknown */
//	new Mame.MemoryWriteAddress( 0xf600, 0xf600, MWA_NOP ), /* unknown */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x3fff, Mame.MRA_BANK1 ),	/* Banked ROMs */
	new Mame.MemoryReadAddress(0x4000, 0x4001, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress(0x5000, 0x50ff, Namco.namcos1_wavedata_r ), /* PSG ( Shared ) */
	new Mame.MemoryReadAddress(0x5100, 0x513f, Namco.namcos1_sound_r ),	/* PSG ( Shared ) */
	new Mame.MemoryReadAddress(0x5140, 0x54ff, Mame.MRA_RAM ),	/* Sound RAM 1 - ( Shared ) */
	new Mame.MemoryReadAddress(0x7000, 0x77ff, Mame.MRA_BANK2 ),	/* Sound RAM 2 - ( Shared ) */
	new Mame.MemoryReadAddress(0x8000, 0x9fff, Mame.MRA_RAM),	/* Sound RAM 3 */
	new Mame.MemoryReadAddress(0xc000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0x3fff, Mame.MWA_ROM ),	/* Banked ROMs */
	new Mame.MemoryWriteAddress(0x4000, 0x4000, YM2151.YM2151_register_port_0_w ),
	new Mame.MemoryWriteAddress(0x4001, 0x4001, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress(0x5000, 0x50ff, Namco.namcos1_wavedata_w,Namco.namco_wavedata ), /* PSG ( Shared ) */
	new Mame.MemoryWriteAddress(0x5100, 0x513f, Namco.namcos1_sound_w,Namco.namco_soundregs ),	/* PSG ( Shared ) */
	new Mame.MemoryWriteAddress(0x5140, 0x54ff, Mame.MWA_RAM ),	/* Sound RAM 1 - ( Shared ) */
	new Mame.MemoryWriteAddress(0x7000, 0x77ff, Mame.MWA_BANK2 ),	/* Sound RAM 2 - ( Shared ) */
	new Mame.MemoryWriteAddress(0x8000, 0x9fff, Mame.MWA_RAM ),	/* Sound RAM 3 */
	new Mame.MemoryWriteAddress(0xc000, 0xc001, namcos1.namcos1_sound_bankswitch_w ), /* bank selector */
	new Mame.MemoryWriteAddress(0xd001, 0xd001, Mame.MWA_NOP ),	/* watchdog? */
	new Mame.MemoryWriteAddress(0xe000, 0xe000, Mame.MWA_NOP ),	/* IRQ clear ? */
	new Mame.MemoryWriteAddress(-1 )	/* end of table */
};

        static void namcos1_coin_w(int offset, int data)
        {
            //	coin_lockout_global_w(0,~data & 1);
            //	coin_counter_w(0,data & 2);
            //	coin_counter_w(1,data & 4);
        }
        static int dsw_r(int offs)
        {
            int ret = Mame.readinputport(2);

            if (offs == 0)
                ret >>= 4;

            return 0xf0 | (ret & 0x0f);
        }
        static int dac0_value, dac1_value, dac0_gain = 0, dac1_gain = 0;

        static void namcos1_update_DACs()
        {
            DAC.DAC_signed_data_16_w(0, 0x8000 + (dac0_value * dac0_gain) + (dac1_value * dac1_gain));
        }

        static void namcos1_dac_gain_w(int offset, int data)
        {
            int value;
            /* DAC0 */
            value = (data & 1) | ((data >> 1) & 2); /* GAIN0,GAIN1 */
            dac0_gain = 0x0101 * (value + 1) / 4 / 2;
            /* DAC1 */
            value = (data >> 3) & 3; /* GAIN2,GAIN3 */
            dac1_gain = 0x0101 * (value + 1) / 4 / 2;
            namcos1_update_DACs();
        }
        static Mame.IOReadPort[] mcu_readport =
{
	new Mame.IOReadPort( Mame.cpu_hd63701.HD63701_PORT1, Mame.cpu_hd63701.HD63701_PORT1, Mame.input_port_3_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] mcu_writeport =
{
	new Mame.IOWritePort( Mame.cpu_hd63701.HD63701_PORT1,Mame.cpu_hd63701.HD63701_PORT1, namcos1_coin_w ),
	new Mame.IOWritePort(Mame.cpu_hd63701.HD63701_PORT2, Mame.cpu_hd63701.HD63701_PORT2, namcos1_dac_gain_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static void namcos1_dac0_w(int offset, int data)
        {
            dac0_value = data - 0x80; /* shift zero point */
            namcos1_update_DACs();
        }

        static void namcos1_dac1_w(int offset, int data)
        {
            dac1_value = data - 0x80; /* shift zero point */
            namcos1_update_DACs();
        }
        static Mame.MemoryReadAddress[] mcu_readmem =
{
	new  Mame.MemoryReadAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_r ),
	new  Mame.MemoryReadAddress( 0x0080, 0x00ff, Mame.MRA_RAM ), /* built in RAM */
	new  Mame.MemoryReadAddress( 0x1400, 0x1400, Mame.input_port_0_r ),
	new  Mame.MemoryReadAddress( 0x1401, 0x1401, Mame.input_port_1_r ),
	new  Mame.MemoryReadAddress( 0x1000, 0x1002, dsw_r ),
	new  Mame.MemoryReadAddress( 0x4000, 0xbfff, Mame.MRA_BANK4 ), /* banked ROM */
	new  Mame.MemoryReadAddress( 0xc000, 0xc7ff, Mame.MRA_BANK3 ),
	new  Mame.MemoryReadAddress( 0xc800, 0xcfff, Mame.MRA_RAM ), /* EEPROM */
	new  Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),
	new  Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] mcu_writemem =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_w ),
	new  Mame.MemoryWriteAddress( 0x0080, 0x00ff, Mame.MWA_RAM ), /* built in RAM */
	new  Mame.MemoryWriteAddress( 0x4000, 0xbfff, Mame.MWA_ROM ),
	new  Mame.MemoryWriteAddress( 0xc000, 0xc000, namcos1.namcos1_mcu_patch_w ),
	new  Mame.MemoryWriteAddress( 0xc000, 0xc7ff, Mame.MWA_BANK3 ),
	new  Mame.MemoryWriteAddress( 0xc800, 0xcfff, Mame.MWA_RAM, nvram, nvram_size ), /* EEPROM */
	new  Mame.MemoryWriteAddress( 0xd000, 0xd000, namcos1_dac0_w ),
	new  Mame.MemoryWriteAddress( 0xd400, 0xd400, namcos1_dac1_w ),
	new  Mame.MemoryWriteAddress( 0xd800, 0xd800, namcos1.namcos1_mcu_bankswitch_w ), /* BANK selector */
	new  Mame.MemoryWriteAddress( 0xf000, 0xf000, Mame.MWA_NOP ), /* IRQ clear ? */
	new  Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.DACinterface dac_interface = new Mame.DACinterface(1, new int[] { 100 });
        static Namco_interface namco_interface = new Namco_interface(23920 / 2, 8, 50, -1, true);
        static YM2151interface ym2151_interface = new YM2151interface(1, 3579580,
            new int[] {YM2151.YM3012_VOL(80, Mame.MIXER_PAN_LEFT, 80, Mame.MIXER_PAN_RIGHT) }, null);
        
        
static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
	8,8,	/* 8*8 characters */
	16384,	/* 16384 characters max */
	1,		/* 1 bit per pixel */
	new uint[]{ 0 },	/* bitplanes offset */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	8*8 	/* every char takes 8 consecutive bytes */
);

static Mame.GfxLayout tilelayout =
new Mame.GfxLayout(
	8,8,	/* 8*8 characters */
	16384,	/* 16384 characters max */
	8,		/* 8 bits per pixel */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7 }, 	/* bitplanes offset */
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	new uint[]{ 0*64, 1*64, 2*64, 3*64, 4*64, 5*64, 6*64, 7*64 },
	64*8	/* every char takes 64 consecutive bytes */
);

static Mame.GfxLayout spritelayout =
new Mame.GfxLayout(
	32,32,	/* 32*32 sprites */
	2048,	/* 2048 sprites max */
	4,		/* 4 bits per pixel */
	new uint[]{ 0, 1, 2, 3 },  /* the bitplanes are packed */
	new uint[]{  0*4,  1*4,  2*4,  3*4,  4*4,  5*4,  6*4,  7*4,
	   8*4,  9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4,
	 256*4,257*4,258*4,259*4,260*4,261*4,262*4,263*4,
	 264*4,265*4,266*4,267*4,268*4,269*4,270*4,271*4},
	new uint[]{ 0*4*16, 1*4*16,  2*4*16,	3*4*16,  4*4*16,  5*4*16,  6*4*16,	7*4*16,
	  8*4*16, 9*4*16, 10*4*16, 11*4*16, 12*4*16, 13*4*16, 14*4*16, 15*4*16,
	 32*4*16,33*4*16, 34*4*16, 35*4*16, 36*4*16, 37*4*16, 38*4*16, 39*4*16,
	 40*4*16,41*4*16, 42*4*16, 43*4*16, 44*4*16, 45*4*16, 46*4*16, 47*4*16 },
	32*4*8*4  /* every sprite takes 512 consecutive bytes */
);

static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,		 0,   1 ),	/* character mask */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tilelayout,	128*16,   6 ),	/* characters */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout,	 0, 128 ),	/* sprites 32/16/8/4 dots */
};
        public class machine_driver_ns1 : Mame.MachineDriver
        {
            public machine_driver_ns1()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, main_readmem, main_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, sub_readmem, sub_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, sound_readmem, sound_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD63701, 49152000 / 8 / 4, mcu_readmem, mcu_writemem, mcu_readport, mcu_writeport, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 0;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_namcos1.gfxdecodeinfo;
                total_colors = 128 * 16 + 6 * 256 + 6 * 256 + 1;
                color_table_len = 128 * 16 + 6 * 256 + 1;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                //sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                //sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
                //sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                int oldcpu = Mame.cpu_getactivecpu(), i;

                /* Point all of our bankhandlers to the error handlers */
                for (i = 0; i < 8; i++)
                {
                    namcos1.namcos1_banks[0, i] = new namcos1.bankhandler();
                    namcos1.namcos1_banks[0, i].bank_handler_r = namcos1.unknown_r;
                    namcos1.namcos1_banks[0, i].bank_handler_w = namcos1.unknown_w;
                    namcos1.namcos1_banks[0, i].bank_offset = 0;
                    namcos1.namcos1_banks[1, i] = new namcos1.bankhandler();
                    namcos1.namcos1_banks[1, i].bank_handler_r = namcos1.unknown_r;
                    namcos1.namcos1_banks[1, i].bank_handler_w = namcos1.unknown_w;
                    namcos1.namcos1_banks[1, i].bank_offset = 0;
                }

                /* Prepare code for Cpu 0 */
                Mame.cpu_setactivecpu(0);
                namcos1.namcos1_bankswitch_w(0x0e00, 0x03); /* bank7 = 0x3ff(PRG7) */
                namcos1.namcos1_bankswitch_w(0x0e01, 0xff);

                /* Prepare code for Cpu 1 */
                Mame.cpu_setactivecpu(1);
                namcos1.namcos1_bankswitch_w(0x0e00, 0x03);
                namcos1.namcos1_bankswitch_w(0x0e01, 0xff);

                namcos1.namcos1_cpu1_banklatch = 0x03ff;

                /* reset starting Cpu */
                Mame.cpu_setactivecpu(oldcpu);

                /* Point mcu & sound shared RAM to destination */
                {
                    _BytePtr RAM = new _BytePtr(Namco.namco_wavedata, 0x1000); /* Ram 1, bank 1, offset 0x1000 */
                    Mame.cpu_setbank(2, RAM);
                    Mame.cpu_setbank(3, RAM);
                }

                /* In case we had some cpu's suspended, resume them now */
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
                Mame.cpu_set_reset_line(3, Mame.ASSERT_LINE);

                namcos1.namcos1_reset = 0;
                /* mcu patch data clear */
                namcos1.mcu_patch_data = 0;

                namcos1.berabohm_input_counter = 4;	/* for berabohm pressure sensitive buttons */
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write!=0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file!=null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                }
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                namcos1.namcos1_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return namcos1.namcos1_vh_start();
            }
            public override void vh_stop()
            {
                namcos1.namcos1_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                namcos1.namcos1_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
               //nothing
            }
        }
    }
    class driver_galaga88 : driver_namcos1
    {      
        void ROM_LOAD_HS(string name, uint start, uint length, uint crc)
        {
            ROM_LOAD(name, start, length, crc);
            ROM_RELOAD(start + length, length);
        }
        Mame.RomModule[] rom_galaga88()
        {
            ROM_START("galaga88");
            ROM_REGION(0x10000, Mame.REGION_CPU1);		/* 64k for the main cpu */
            /* Nothing loaded here. Bankswitching makes sure this gets the necessary code */

            ROM_REGION(0x10000, Mame.REGION_CPU2);		/* 64k for the sub cpu */
            /* Nothing loaded here. Bankswitching makes sure this gets the necessary code */

            ROM_REGION(0x2c000, Mame.REGION_CPU3);		/* 176k for the sound cpu */
            ROM_LOAD("g88_snd0.rom", 0x0c000, 0x10000, 0x164a3fdc);
            ROM_LOAD("g88_snd1.rom", 0x1c000, 0x10000, 0x16a4b784);

            ROM_REGION(0x100000, Mame.REGION_USER1);	/* 1M for ROMs */
            ROM_LOAD_HS("g88_prg7.rom", 0x00000, 0x10000, 0xdf75b7fc);
            ROM_LOAD_HS("g88_prg6.rom", 0x20000, 0x10000, 0x7e3471d3);
            ROM_LOAD_HS("g88_prg5.rom", 0x40000, 0x10000, 0x4fbd3f6c);
            ROM_LOAD_HS("g88_prg1.rom", 0xc0000, 0x10000, 0xe68cb351);
            ROM_LOAD_HS("g88_prg0.rom", 0xe0000, 0x10000, 0x0f0778ca);

            ROM_REGION(0x14000, Mame.REGION_USER2);	/* 80k for RAM */

            ROM_REGION(0xd0000, Mame.REGION_CPU4);		/* the MCU & voice */
            ROM_LOAD("ns1-mcu.bin", 0x0f000, 0x01000, 0xffb5c0bd);
            ROM_LOAD_HS("g88_vce0.rom", 0x10000, 0x10000, 0x86921dd4);
            ROM_LOAD_HS("g88_vce1.rom", 0x30000, 0x10000, 0x9c300e16);
            ROM_LOAD_HS("g88_vce2.rom", 0x50000, 0x10000, 0x5316b4b0);
            ROM_LOAD_HS("g88_vce3.rom", 0x70000, 0x10000, 0xdc077af4);
            ROM_LOAD_HS("g88_vce4.rom", 0x90000, 0x10000, 0xac0279a7);
            ROM_LOAD_HS("g88_vce5.rom", 0xb0000, 0x10000, 0x014ddba1);

            ROM_REGION(0x20000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);  /* character mask */
            ROM_LOAD("g88_chr8.rom", 0x00000, 0x20000, 0x3862ed0a);

            ROM_REGION(0x100000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE); /* characters */
            ROM_LOAD("g88_chr0.rom", 0x00000, 0x20000, 0x68559c78);
            ROM_LOAD("g88_chr1.rom", 0x20000, 0x20000, 0x3dc0f93f);
            ROM_LOAD("g88_chr2.rom", 0x40000, 0x20000, 0xdbf26f1f);
            ROM_LOAD("g88_chr3.rom", 0x60000, 0x20000, 0xf5d6cac5);

            ROM_REGION(0x100000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE); /* sprites */
            ROM_LOAD("g88_obj0.rom", 0x00000, 0x20000, 0xd7112e3f);
            ROM_LOAD("g88_obj1.rom", 0x20000, 0x20000, 0x680db8e7);
            ROM_LOAD("g88_obj2.rom", 0x40000, 0x20000, 0x13c97512);
            ROM_LOAD("g88_obj3.rom", 0x60000, 0x20000, 0x3ed3941b);
            ROM_LOAD("g88_obj4.rom", 0x80000, 0x20000, 0x370ff4ad);
            ROM_LOAD("g88_obj5.rom", 0xa0000, 0x20000, 0xb0645169);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ns1()
        {
            /* Standard Namco System 1 input port definition */
            INPUT_PORTS_START("ns1");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START("DSW1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_DIPNAME(0x08, 0x08, "Auto Data Sampling");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_DIPNAME(0x40, 0x40, "Freeze");
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);

            PORT_START("IN2 : mcu PORT2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, IPT_UNUSED); //OUT:coin lockout
            PORT_BIT(0x02, IP_ACTIVE_HIGH, IPT_UNUSED); //OUT:coin counter1
            PORT_BIT(0x04, IP_ACTIVE_HIGH, IPT_UNUSED); //OUT:coin counter2
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Service Button", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        public override void driver_init()
        {

            namcos1.namcos1_specific galaga88_specific =
           new namcos1.namcos1_specific(
               0x2d, 0x31,				/* key query , key id */
               namcos1.rev1_key_r, namcos1.rev1_key_w,	/* key handler */
               namcos1.normal_slice,			/* CPU slice normal */
               1						/* use tilemap flag : speedup optimize */
           );
            namcos1.namcos1_driver_init(galaga88_specific);
        }
        public driver_galaga88()
        {
            drv = new machine_driver_ns1();
            year = "1987";
            name = "galaga88";
            description = "Galaga '88 (set 1)";
            manufacturer = "Namco";
            flags = Mame.ROT90_16BIT;
            input_ports = input_ports_ns1();
            rom = rom_galaga88();
            drv.HasNVRAMhandler = true;
        }
    }
}
