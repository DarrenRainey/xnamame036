#define NAMCOS1_DIRECT_DRAW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_namcos1 : Mame.GameDriver
    {
        const int NAMCOS1_MAX_BANK = 0x400;
        const int NAMCOS1_MAX_KEY = 0x100;

        const byte MAX_PLAYFIELDS = 6, MAX_SPRITES = 127;
        const int FG_OFFSET = 0x7000;

        const int SPRITECOLORS = 2048;
        const int TILECOLORS = 1536;
        const int BACKGROUNDCOLOR = SPRITECOLORS + 2 * TILECOLORS;


        static _BytePtr namcos1_videoram = new _BytePtr(1);
        static _BytePtr namcos1_paletteram = new _BytePtr(1);
        static _BytePtr namcos1_controlram = new _BytePtr(1);

        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];

        struct playfield
        {
            public _BytePtr _base;
            public int scroll_x;
            public int scroll_y;
#if NAMCOS1_DIRECT_DRAW
            public int width;
            public int height;
#endif
            public Mame.tilemap tilemap;
            public int color;
        };
        static playfield[] playfields = new playfield[MAX_PLAYFIELDS];

#if NAMCOS1_DIRECT_DRAW
        static int namcos1_tilemap_need = 0;
        static int namcos1_tilemap_used;

        static _BytePtr char_state;
        const byte CHAR_BLANK = 0, CHAR_FULL = 1;
#endif

        /* playfields maskdata for tilemap */
        static _BytePtr[] mask_ptr;
        static _BytePtr mask_data;

        /* graphic object */
        static Mame.gfx_object_list objectlist;
        static Mame.gfx_object[] objects;

        /* palette dirty information */
        static byte[] sprite_palette_state = new byte[MAX_SPRITES + 1];
        static byte[] tilemap_palette_state = new byte[MAX_PLAYFIELDS];

        /* per game scroll adjustment */
        static int[] scrolloffsX = new int[4];
        static int[] scrolloffsY = new int[4];

        static int sprite_fixed_sx;
        static int sprite_fixed_sy;
        static int flipscreen;

        static _BytePtr info_vram;
        static int info_color;



        static Mame.MemoryReadAddress[] main_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, namcos1_0_banked_area0_r),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, namcos1_0_banked_area1_r),
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, namcos1_0_banked_area2_r),
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, namcos1_0_banked_area3_r),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, namcos1_0_banked_area4_r),
	new Mame.MemoryReadAddress( 0xa000, 0xbfff, namcos1_0_banked_area5_r),
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, namcos1_0_banked_area6_r),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, namcos1_0_banked_area7_r),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] main_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, namcos1_0_banked_area0_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, namcos1_0_banked_area1_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x5fff, namcos1_0_banked_area2_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, namcos1_0_banked_area3_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, namcos1_0_banked_area4_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xbfff, namcos1_0_banked_area5_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdfff, namcos1_0_banked_area6_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, namcos1_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf000, namcos1_cpu_control_w ),
	new Mame.MemoryWriteAddress( 0xf200, 0xf200, Mame.MWA_NOP ), /* watchdog? */
//	new Mame.MemoryWriteAddress( 0xf400, 0xf400, MWA_NOP }, /* unknown */
//	new Mame.MemoryWriteAddress( 0xf600, 0xf600, MWA_NOP }, /* unknown */
	new Mame.MemoryWriteAddress( 0xfc00, 0xfc01, namcos1_subcpu_bank ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sub_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, namcos1_1_banked_area0_r ),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, namcos1_1_banked_area1_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, namcos1_1_banked_area2_r ),
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, namcos1_1_banked_area3_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, namcos1_1_banked_area4_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xbfff, namcos1_1_banked_area5_r ),
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, namcos1_1_banked_area6_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, namcos1_1_banked_area7_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sub_writemem =
{
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, namcos1_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, namcos1_1_banked_area0_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, namcos1_1_banked_area1_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x5fff, namcos1_1_banked_area2_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, namcos1_1_banked_area3_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, namcos1_1_banked_area4_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xbfff, namcos1_1_banked_area5_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdfff, namcos1_1_banked_area6_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf000, Mame.MWA_NOP ), /* IO Chip */
	new Mame.MemoryWriteAddress( 0xf200, 0xf200, Mame.MWA_NOP ), /* watchdog? */
//	new Mame.MemoryWriteAddress( 0xf400, 0xf400, MWA_NOP }, /* unknown */
//	new Mame.MemoryWriteAddress( 0xf600, 0xf600, MWA_NOP }, /* unknown */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_BANK1 ),	/* Banked ROMs */
	new Mame.MemoryReadAddress( 0x4000, 0x4001, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x5000, 0x50ff, Namco.namcos1_wavedata_r ), /* PSG ( Shared ) */
	new Mame.MemoryReadAddress( 0x5100, 0x513f, Namco.namcos1_sound_r ),	/* PSG ( Shared ) */
	new Mame.MemoryReadAddress( 0x5140, 0x54ff, Mame.MRA_RAM ),	/* Sound RAM 1 - ( Shared ) */
	new Mame.MemoryReadAddress( 0x7000, 0x77ff, Mame.MRA_BANK2 ),	/* Sound RAM 2 - ( Shared ) */
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, Mame.MRA_RAM ),	/* Sound RAM 3 */
	new Mame.MemoryReadAddress( 0xc000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM),	/* Banked ROMs */
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, YM2151.YM2151_register_port_0_w ),
	new Mame.MemoryWriteAddress( 0x4001, 0x4001, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress( 0x5000, 0x50ff, Namco.namcos1_wavedata_w,Namco.namco_wavedata ), /* PSG ( Shared ) */
	new Mame.MemoryWriteAddress( 0x5100, 0x513f, Namco.namcos1_sound_w,Namco.namco_soundregs ),	/* PSG ( Shared ) */
	new Mame.MemoryWriteAddress( 0x5140, 0x54ff, Mame.MWA_RAM ),	/* Sound RAM 1 - ( Shared ) */
	new Mame.MemoryWriteAddress( 0x7000, 0x77ff, Mame.MWA_BANK2 ),	/* Sound RAM 2 - ( Shared ) */
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, Mame.MWA_RAM ),	/* Sound RAM 3 */
	new Mame.MemoryWriteAddress( 0xc000, 0xc001, namcos1_sound_bankswitch_w ), /* bank selector */
	new Mame.MemoryWriteAddress( 0xd001, 0xd001, Mame.MWA_NOP ),	/* watchdog? */
	new Mame.MemoryWriteAddress( 0xe000, 0xe000, Mame.MWA_NOP ),	/* IRQ clear ? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] mcu_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_r ),
	new Mame.MemoryReadAddress( 0x0080, 0x00ff, Mame.MRA_RAM ), /* built in RAM */
	new Mame.MemoryReadAddress( 0x1400, 0x1400, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x1401, 0x1401, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x1000, 0x1002, dsw_r ),
	new Mame.MemoryReadAddress( 0x4000, 0xbfff, Mame.MRA_BANK4 ), /* banked ROM */
	new Mame.MemoryReadAddress( 0xc000, 0xc7ff, Mame.MRA_BANK3 ),
	new Mame.MemoryReadAddress( 0xc800, 0xcfff, Mame.MRA_RAM ), /* EEPROM */
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] mcu_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_w ),
	new Mame.MemoryWriteAddress( 0x0080, 0x00ff, Mame.MWA_RAM ), /* built in RAM */
	new Mame.MemoryWriteAddress( 0x4000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, namcos1_mcu_patch_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, Mame.MWA_BANK3 ),
	new Mame.MemoryWriteAddress( 0xc800, 0xcfff, Mame.MWA_RAM, nvram, nvram_size ), /* EEPROM */
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, namcos1_dac0_w ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd400, namcos1_dac1_w ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd800, namcos1_mcu_bankswitch_w ), /* BANK selector */
	new Mame.MemoryWriteAddress( 0xf000, 0xf000, Mame.MWA_NOP ), /* IRQ clear ? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOReadPort[] mcu_readport =
{
	new Mame.IOReadPort( Mame.cpu_hd63701.HD63701_PORT1,  Mame.cpu_hd63701.HD63701_PORT1, Mame.input_port_3_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] mcu_writeport =
{
	new Mame.IOWritePort(  Mame.cpu_hd63701.HD63701_PORT1,  Mame.cpu_hd63701.HD63701_PORT1, namcos1_coin_w ),
	new Mame.IOWritePort(  Mame.cpu_hd63701.HD63701_PORT2,  Mame.cpu_hd63701.HD63701_PORT2, namcos1_dac_gain_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};
        struct bankhandler
        {
            public Mame.mem_read_handler bank_handler_r;
            public Mame.mem_write_handler bank_handler_w;
            public int bank_offset;
            public _BytePtr bank_pointer;
        }
        static int dac0_value, dac1_value, dac0_gain = 0, dac1_gain = 0;

        static bankhandler[] namcos1_bank_element = new bankhandler[NAMCOS1_MAX_BANK];
        static bankhandler[,] namcos1_banks = new bankhandler[2, 8];

        static byte[] key = new byte[NAMCOS1_MAX_KEY];
        static _BytePtr s1ram = new _BytePtr(1);
        static int namcos1_cpu1_banklatch;
        static int namcos1_reset = 0;
        static int berabohm_input_counter;
        static int key_id, key_id_query;


        static void namcos1_coin_w(int offset, int data)
        {
            //	coin_lockout_global_w(0,~data & 1);
            //	coin_counter_w(0,data & 2);
            //	coin_counter_w(1,data & 4);
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


        static int namcos1_0_banked_area0_r(int offset) { if (namcos1_banks[0, 0].bank_handler_r != null) return namcos1_banks[0, 0].bank_handler_r(offset + namcos1_banks[0, 0].bank_offset); return namcos1_banks[0, 0].bank_pointer[offset]; }
        static int namcos1_0_banked_area1_r(int offset) { if (namcos1_banks[0, 1].bank_handler_r != null) return namcos1_banks[0, 1].bank_handler_r(offset + namcos1_banks[0, 1].bank_offset); return namcos1_banks[0, 1].bank_pointer[offset]; }
        static int namcos1_0_banked_area2_r(int offset) { if (namcos1_banks[0, 2].bank_handler_r != null) return namcos1_banks[0, 2].bank_handler_r(offset + namcos1_banks[0, 2].bank_offset); return namcos1_banks[0, 2].bank_pointer[offset]; }
        static int namcos1_0_banked_area3_r(int offset) { if (namcos1_banks[0, 3].bank_handler_r != null) return namcos1_banks[0, 3].bank_handler_r(offset + namcos1_banks[0, 3].bank_offset); return namcos1_banks[0, 3].bank_pointer[offset]; }
        static int namcos1_0_banked_area4_r(int offset) { if (namcos1_banks[0, 4].bank_handler_r != null) return namcos1_banks[0, 4].bank_handler_r(offset + namcos1_banks[0, 4].bank_offset); return namcos1_banks[0, 4].bank_pointer[offset]; }
        static int namcos1_0_banked_area5_r(int offset) { if (namcos1_banks[0, 5].bank_handler_r != null) return namcos1_banks[0, 5].bank_handler_r(offset + namcos1_banks[0, 5].bank_offset); return namcos1_banks[0, 5].bank_pointer[offset]; }
        static int namcos1_0_banked_area6_r(int offset) { if (namcos1_banks[0, 6].bank_handler_r != null) return namcos1_banks[0, 6].bank_handler_r(offset + namcos1_banks[0, 6].bank_offset); return namcos1_banks[0, 6].bank_pointer[offset]; }
        static int namcos1_0_banked_area7_r(int offset) { if (namcos1_banks[0, 7].bank_handler_r != null) return namcos1_banks[0, 7].bank_handler_r(offset + namcos1_banks[0, 7].bank_offset); return namcos1_banks[0, 7].bank_pointer[offset]; }
        static int namcos1_1_banked_area0_r(int offset) { if (namcos1_banks[1, 0].bank_handler_r != null) return namcos1_banks[1, 0].bank_handler_r(offset + namcos1_banks[1, 0].bank_offset); return namcos1_banks[1, 0].bank_pointer[offset]; }
        static int namcos1_1_banked_area1_r(int offset) { if (namcos1_banks[1, 1].bank_handler_r != null) return namcos1_banks[1, 1].bank_handler_r(offset + namcos1_banks[1, 1].bank_offset); return namcos1_banks[1, 1].bank_pointer[offset]; }
        static int namcos1_1_banked_area2_r(int offset) { if (namcos1_banks[1, 2].bank_handler_r != null) return namcos1_banks[1, 2].bank_handler_r(offset + namcos1_banks[1, 2].bank_offset); return namcos1_banks[1, 2].bank_pointer[offset]; }
        static int namcos1_1_banked_area3_r(int offset) { if (namcos1_banks[1, 3].bank_handler_r != null) return namcos1_banks[1, 3].bank_handler_r(offset + namcos1_banks[1, 3].bank_offset); return namcos1_banks[1, 3].bank_pointer[offset]; }
        static int namcos1_1_banked_area4_r(int offset) { if (namcos1_banks[1, 4].bank_handler_r != null) return namcos1_banks[1, 4].bank_handler_r(offset + namcos1_banks[1, 4].bank_offset); return namcos1_banks[1, 4].bank_pointer[offset]; }
        static int namcos1_1_banked_area5_r(int offset) { if (namcos1_banks[1, 5].bank_handler_r != null) return namcos1_banks[1, 5].bank_handler_r(offset + namcos1_banks[1, 5].bank_offset); return namcos1_banks[1, 5].bank_pointer[offset]; }
        static int namcos1_1_banked_area6_r(int offset) { if (namcos1_banks[1, 6].bank_handler_r != null) return namcos1_banks[1, 6].bank_handler_r(offset + namcos1_banks[1, 6].bank_offset); return namcos1_banks[1, 6].bank_pointer[offset]; }
        static int namcos1_1_banked_area7_r(int offset) { if (namcos1_banks[1, 7].bank_handler_r != null) return namcos1_banks[1, 7].bank_handler_r(offset + namcos1_banks[1, 7].bank_offset); return namcos1_banks[1, 7].bank_pointer[offset]; }
        static void namcos1_0_banked_area0_w(int offset, int data) { if (namcos1_banks[0, 0].bank_handler_w != null) { namcos1_banks[0, 0].bank_handler_w(offset + namcos1_banks[0, 0].bank_offset, data); return; } namcos1_banks[0, 0].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area1_w(int offset, int data) { if (namcos1_banks[0, 1].bank_handler_w != null) { namcos1_banks[0, 1].bank_handler_w(offset + namcos1_banks[0, 1].bank_offset, data); return; } namcos1_banks[0, 1].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area2_w(int offset, int data) { if (namcos1_banks[0, 2].bank_handler_w != null) { namcos1_banks[0, 2].bank_handler_w(offset + namcos1_banks[0, 2].bank_offset, data); return; } namcos1_banks[0, 2].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area3_w(int offset, int data) { if (namcos1_banks[0, 3].bank_handler_w != null) { namcos1_banks[0, 3].bank_handler_w(offset + namcos1_banks[0, 3].bank_offset, data); return; } namcos1_banks[0, 3].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area4_w(int offset, int data) { if (namcos1_banks[0, 4].bank_handler_w != null) { namcos1_banks[0, 4].bank_handler_w(offset + namcos1_banks[0, 4].bank_offset, data); return; } namcos1_banks[0, 4].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area5_w(int offset, int data) { if (namcos1_banks[0, 5].bank_handler_w != null) { namcos1_banks[0, 5].bank_handler_w(offset + namcos1_banks[0, 5].bank_offset, data); return; } namcos1_banks[0, 5].bank_pointer[offset] = (byte)data; }
        static void namcos1_0_banked_area6_w(int offset, int data) { if (namcos1_banks[0, 6].bank_handler_w != null) { namcos1_banks[0, 6].bank_handler_w(offset + namcos1_banks[0, 6].bank_offset, data); return; } namcos1_banks[0, 6].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area0_w(int offset, int data) { if (namcos1_banks[1, 0].bank_handler_w != null) { namcos1_banks[1, 0].bank_handler_w(offset + namcos1_banks[1, 0].bank_offset, data); return; } namcos1_banks[1, 0].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area1_w(int offset, int data) { if (namcos1_banks[1, 1].bank_handler_w != null) { namcos1_banks[1, 1].bank_handler_w(offset + namcos1_banks[1, 1].bank_offset, data); return; } namcos1_banks[1, 1].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area2_w(int offset, int data) { if (namcos1_banks[1, 2].bank_handler_w != null) { namcos1_banks[1, 2].bank_handler_w(offset + namcos1_banks[1, 2].bank_offset, data); return; } namcos1_banks[1, 2].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area3_w(int offset, int data) { if (namcos1_banks[1, 3].bank_handler_w != null) { namcos1_banks[1, 3].bank_handler_w(offset + namcos1_banks[1, 3].bank_offset, data); return; } namcos1_banks[1, 3].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area4_w(int offset, int data) { if (namcos1_banks[1, 4].bank_handler_w != null) { namcos1_banks[1, 4].bank_handler_w(offset + namcos1_banks[1, 4].bank_offset, data); return; } namcos1_banks[1, 4].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area5_w(int offset, int data) { if (namcos1_banks[1, 5].bank_handler_w != null) { namcos1_banks[1, 5].bank_handler_w(offset + namcos1_banks[1, 5].bank_offset, data); return; } namcos1_banks[1, 5].bank_pointer[offset] = (byte)data; }
        static void namcos1_1_banked_area6_w(int offset, int data) { if (namcos1_banks[1, 6].bank_handler_w != null) { namcos1_banks[1, 6].bank_handler_w(offset + namcos1_banks[1, 6].bank_offset, data); return; } namcos1_banks[1, 6].bank_pointer[offset] = (byte)data; }

        static int dsw_r(int offs)
        {
            int ret = Mame.readinputport(2);

            if (offs == 0)
                ret >>= 4;

            return 0xf0 | (ret & 0x0f);
        }

        static void namcos1_mcu_patch_w(int offset, int data)
        {
            //if(errorlog) fprintf(errorlog,"mcu C000 write pc=%04x data=%02x\n",cpu_get_pc(),data);
            if (mcu_patch_data == 0xa6) return;
            mcu_patch_data = data;

            Mame.mwh_bank3(offset, data);
        }
        static int mcu_patch_data;


        static void namcos1_update_DACs()
        {
            DAC.DAC_signed_data_16_w(0, 0x8000 + (dac0_value * dac0_gain) + (dac1_value * dac1_gain));
        }
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


        static void namcos1_mcu_bankswitch_w(int offset, int data)
        {
            int addr;
            /* bit 2-7 : chip select line of ROM chip */
            switch (data & 0xfc)
            {
                case 0xf8: addr = 0x10000; break; /* bit 2 : ROM 0 */
                case 0xf4: addr = 0x30000; break; /* bit 3 : ROM 1 */
                case 0xec: addr = 0x50000; break; /* bit 4 : ROM 2 */
                case 0xdc: addr = 0x70000; break; /* bit 5 : ROM 3 */
                case 0xbc: addr = 0x90000; break; /* bit 6 : ROM 4 */
                case 0x7c: addr = 0xb0000; break; /* bit 7 : ROM 5 */
                default: addr = 0x100000; /* illegal */break;
            }
            /* bit 0-1 : address line A15-A16 */
            addr += (data & 3) * 0x8000;
            if (addr >= Mame.memory_region_length(Mame.REGION_CPU4))
            {
                Mame.printf("unmapped mcu bank selected pc=%04x bank=%02x\n", Mame.cpu_get_pc(), data);
                addr = 0x4000;
            }
            Mame.cpu_setbank(4, new _BytePtr(Mame.memory_region(Mame.REGION_CPU4), addr));
        }

        static void namcos1_cpu_control_w(int offset, int data)
        {
            //	if(errorlog) fprintf(errorlog,"reset control pc=%04x %02x\n",cpu_get_pc(),data);
            if (((data & 1) ^ namcos1_reset) != 0)
            {
                namcos1_reset = data & 1;
                if (namcos1_reset != 0)
                {
                    Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
                    Mame.cpu_set_reset_line(2, Mame.CLEAR_LINE);
                    Mame.cpu_set_reset_line(3, Mame.CLEAR_LINE);
                    mcu_patch_data = 0;
                }
                else
                {
                    Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                    Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
                    Mame.cpu_set_reset_line(3, Mame.ASSERT_LINE);
                }
            }
        }
        static void namcos1_sound_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU3);
            int bank = (data >> 4) & 0x07;

            Mame.cpu_setbank(1, new _BytePtr(RAM, 0x0c000 + (0x4000 * bank)));
        }


        static int unknown_r(int offset)
        {
            Mame.printf("CPU #%d PC %04x: warning - read from unknown chip\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc());
            return 0;
        }

        static int soundram_r(int offset)
        {
            if (offset < 0x100)
                return Namco.namcos1_wavedata_r(offset);
            if (offset < 0x140)
                return Namco.namcos1_sound_r(offset - 0x100);

            /* shared ram */
            return Namco.namco_wavedata[offset];
        }

        static void soundram_w(int offset, int data)
        {
            if (offset < 0x100)
            {
                Namco.namcos1_wavedata_w(offset, data);
                return;
            }
            if (offset < 0x140)
            {
                Namco.namcos1_sound_w(offset - 0x100, data);
                return;
            }
            /* shared ram */
            Namco.namco_wavedata[offset] = (byte)data;

            //if(offset>=0x1000 && errorlog)
            //	fprintf(errorlog,"CPU #%d PC %04x: write shared ram %04x=%02x\n",cpu_getactivecpu(),cpu_get_pc(),offset,data);
        }

        /* ROM handlers */

        static void rom_w(int offset, int data)
        {
            Mame.printf("CPU #%d PC %04x: warning - write %02x to rom address %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), data, offset);
        }

        static void unknown_w(int offset, int data)
        {
            Mame.printf("CPU #%d PC %04x: warning - wrote to unknown chip\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc());
        }

        static int namcos1_paletteram_r(int offset)
        {
            return namcos1_paletteram[offset];
        }

        static void namcos1_paletteram_w(int offset, int data)
        {
            if (namcos1_paletteram[offset] != data)
            {
                namcos1_paletteram[offset] = (byte)data;
                if ((offset & 0x1fff) < 0x1800)
                {
                    if (offset < 0x2000)
                    {
                        sprite_palette_state[(offset & 0x7f0) / 16] = 1;
                    }
                    else
                    {
                        int i, color;

                        color = (offset & 0x700) / 256;
                        for (i = 0; i < MAX_PLAYFIELDS; i++)
                        {
                            if (playfields[i].color == color)
                                tilemap_palette_state[i] = 1;
                        }
                    }
                }
            }
        }

        static int chip = 0;
        static void namcos1_bankswitch_w(int offset, int data)
        {

            if ((offset & 1) != 0)
            {
                int bank = (offset >> 9) & 0x07; //0x0f;
                int cpu = Mame.cpu_getactivecpu();
                chip &= 0x0300;
                chip |= (data & 0xff);
                /* copy bank handler */
                namcos1_banks[cpu, bank].bank_handler_r = namcos1_bank_element[chip].bank_handler_r;
                namcos1_banks[cpu, bank].bank_handler_w = namcos1_bank_element[chip].bank_handler_w;
                namcos1_banks[cpu, bank].bank_offset = namcos1_bank_element[chip].bank_offset;
                namcos1_banks[cpu, bank].bank_pointer = namcos1_bank_element[chip].bank_pointer;
                //memcpy( &namcos1_banks[cpu][bank] , &namcos1_bank_element[chip] , sizeof(bankhandler));

                /* unmapped bank warning */
                if (namcos1_banks[cpu, bank].bank_handler_r == unknown_r)
                {
                    Mame.printf("CPU #%d PC %04x:warning unknown chip selected bank %x=$%04x\n", cpu, Mame.cpu_get_pc(), bank, chip);
                }

                /* renew pc base */
                //		change_pc16(cpu_get_pc());
            }
            else
            {
                chip &= 0x00ff;
                chip |= (data & 0xff) << 8;
            }
        }
        static void namcos1_subcpu_bank(int offset, int data)
        {
            int oldcpu = Mame.cpu_getactivecpu();

            //if(errorlog) fprintf(errorlog,"cpu1 bank selected %02x=%02x\n",offset,data);
            namcos1_cpu1_banklatch = (namcos1_cpu1_banklatch & 0x300) | data;
            /* Prepare code for Cpu 1 */
            Mame.cpu_setactivecpu(1);
            namcos1_bankswitch_w(0x0e00, namcos1_cpu1_banklatch >> 8);
            namcos1_bankswitch_w(0x0e01, namcos1_cpu1_banklatch & 0xff);
            /* cpu_set_reset_line(1,PULSE_LINE); */

            Mame.cpu_setactivecpu(oldcpu);
        }




















        public static void ROM_LOAD_HS(string name, uint start, uint length, uint crc)
        {
            ROM_LOAD(name, start, length, crc);
            ROM_RELOAD(start + length, length);
        }
        public static Mame.InputPortTiny[] input_ports_ns1()
        {
            INPUT_PORTS_START("ns1");
            PORT_START();		/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();		/* DSW1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_DIPNAME(0x08, 0x08, "Auto Data Sampling");
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_DIPNAME(0x40, 0x40, "Freeze");
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);

            PORT_START();	/* IN2 : mcu PORT2 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, IPT_UNUSED);//OUT:coin lockout
            PORT_BIT(0x02, IP_ACTIVE_HIGH, IPT_UNUSED);//OUT:coin counter1
            PORT_BIT(0x04, IP_ACTIVE_HIGH, IPT_UNUSED);//OUT:coin counter2
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE, "Service Button", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);
            return INPUT_PORTS_END;
        }

        static _BytePtr get_gfx_pointer(Mame.GfxElement gfxelement, int c, int line)
        {
            return new _BytePtr(gfxelement.gfxdata, (c * gfxelement.height + line) * gfxelement.line_modulo);
        }

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            16384,	/* 16384 characters max */
            1,		/* 1 bit per pixel */
            new uint[] { 0 },	/* bitplanes offset */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8 	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            16384,	/* 16384 characters max */
            8,		/* 8 bits per pixel */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 	/* bitplanes offset */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            new uint[] { 0 * 64, 1 * 64, 2 * 64, 3 * 64, 4 * 64, 5 * 64, 6 * 64, 7 * 64 },
            64 * 8	/* every char takes 64 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            32, 32,	/* 32*32 sprites */
            2048,	/* 2048 sprites max */
            4,		/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },  /* the bitplanes are packed */
            new uint[]{  0*4,  1*4,  2*4,  3*4,  4*4,  5*4,  6*4,  7*4,
	   8*4,  9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4,
	 256*4,257*4,258*4,259*4,260*4,261*4,262*4,263*4,
	 264*4,265*4,266*4,267*4,268*4,269*4,270*4,271*4},
            new uint[]{ 0*4*16, 1*4*16,  2*4*16,	3*4*16,  4*4*16,  5*4*16,  6*4*16,	7*4*16,
	  8*4*16, 9*4*16, 10*4*16, 11*4*16, 12*4*16, 13*4*16, 14*4*16, 15*4*16,
	 32*4*16,33*4*16, 34*4*16, 35*4*16, 36*4*16, 37*4*16, 38*4*16, 39*4*16,
	 40*4*16,41*4*16, 42*4*16, 43*4*16, 44*4*16, 45*4*16, 46*4*16, 47*4*16 },
            32 * 4 * 8 * 4  /* every sprite takes 512 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0,charlayout,		 0,   1 ),	/* character mask */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0,tilelayout,	128*16,   6 ),	/* characters */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0,spritelayout,	 0, 128 ),	/* sprites 32/16/8/4 dots */
};

        static namcos1_slice_timer[] normal_slice = { null };


        static YM2151interface ym2151_interface =
        new YM2151interface(
            1,			/* 1 chip */
            3579580,	/* 3.58 MHZ */
            new int[] { YM2151.YM3012_VOL(80, Mame.MIXER_PAN_LEFT, 80, Mame.MIXER_PAN_RIGHT) },
            new YM2151irqhandler[] { namcos1_sound_interrupt },
            new Mame.mem_write_handler[] { null }
        );

        static Namco_interface namco_interface =
        new Namco_interface(
            23920 / 2,	/* sample rate (approximate value) */
            8,			/* number of voices */
            50, 		/* playback volume */
            -1, 		/* memory region */
            true			/* stereo */
        );
        static void namcos1_sound_interrupt(int irq)
        {
            Mame.cpu_set_irq_line(2, Mame.cpu_m6809.M6809_FIRQ_LINE, irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        /*
            namcos1 has tow 8bit dac channel. But They are mixed before pre-amp.
            And,they are connected with pre-amp through active LPF.
            LFP info : Fco = 3.3KHz , g = -12dB/oct
        */
        static Mame.DACinterface dac_interface =
        new Mame.DACinterface(
            1,			/* 2 channel , but they are mixed by the driver */
            new int[] { 100 }	/* mixing level */
        );
        public class machine_driver_namcos1 : Mame.MachineDriver
        {
            public machine_driver_namcos1()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, main_readmem, main_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, sub_readmem, sub_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, sound_readmem, sound_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD63701, 49152000 / 8 / 3, mcu_readmem, mcu_writemem, mcu_readport, mcu_writeport, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 0;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_namcos1.gfxdecodeinfo;
                total_colors = 128 * 16 + 6 * 256 + 6 * 256 + 1;
                color_table_len = 128 * 16 + 6 * 256 + 1;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_UPDATE_BEFORE_VBLANK;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;

                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                int oldcpu = Mame.cpu_getactivecpu(), i;

                /* Point all of our bankhandlers to the error handlers */
                for (i = 0; i < 8; i++)
                {
                    namcos1_banks[0, i].bank_handler_r = unknown_r;
                    namcos1_banks[0, i].bank_handler_w = unknown_w;
                    namcos1_banks[0, i].bank_offset = 0;
                    namcos1_banks[1, i].bank_handler_r = unknown_r;
                    namcos1_banks[1, i].bank_handler_w = unknown_w;
                    namcos1_banks[1, i].bank_offset = 0;
                }

                /* Prepare code for Cpu 0 */
                Mame.cpu_setactivecpu(0);
                namcos1_bankswitch_w(0x0e00, 0x03); /* bank7 = 0x3ff(PRG7) */
                namcos1_bankswitch_w(0x0e01, 0xff);

                /* Prepare code for Cpu 1 */
                Mame.cpu_setactivecpu(1);
                namcos1_bankswitch_w(0x0e00, 0x03);
                namcos1_bankswitch_w(0x0e01, 0xff);

                namcos1_cpu1_banklatch = 0x03ff;

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

                namcos1_reset = 0;
                /* mcu patch data clear */
                mcu_patch_data = 0;

                berabohm_input_counter = 4;	/* for berabohm pressure sensitive buttons */
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write != 0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file != null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                }
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    palette[i * 3 + 0] = 0;
                    palette[i * 3 + 1] = 0;
                    palette[i * 3 + 2] = 0;
                }
            }
            static void background_get_info(int col, int row)
            {
                int tile_index = (row * 64 + col) * 2;
                int code = info_vram[tile_index + 1] + ((info_vram[tile_index] & 0x3f) << 8);
                Mame.SET_TILE_INFO(1, code, info_color);
                Mame.tile_info.mask_data = mask_ptr[code];
            }

            static void foreground_get_info(int col, int row)
            {
                int tile_index = (row * 36 + col) * 2;
                int code = info_vram[tile_index + 1] + ((info_vram[tile_index] & 0x3f) << 8);
                Mame.SET_TILE_INFO(1, code, info_color);
                Mame.tile_info.mask_data = mask_ptr[code];
            }
            static void namcos1_set_flipscreen(int flip)
            {
                int i;

                int[] pos_x = { 0x0b0, 0x0b2, 0x0b3, 0x0b4 };
                int[] pos_y = { 0x108, 0x108, 0x108, 0x008 };
                int[] neg_x = { 0x1d0, 0x1d2, 0x1d3, 0x1d4 };
                int[] neg_y = { 0x1e8, 0x1e8, 0x1e8, 0x0e8 };

                flipscreen = flip;
                if (flip == 0)
                {
                    for (i = 0; i < 4; i++)
                    {
                        scrolloffsX[i] = pos_x[i];
                        scrolloffsY[i] = pos_y[i];
                    }
                }
                else
                {
                    for (i = 0; i < 4; i++)
                    {
                        scrolloffsX[i] = neg_x[i];
                        scrolloffsY[i] = neg_y[i];
                    }
                }
#if NAMCOS1_DIRECT_DRAW
                if (namcos1_tilemap_used != 0)
#endif
                    Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? Mame.TILEMAP_FLIPX | Mame.TILEMAP_FLIPY : 0);
            }

            static void namcos1_playfield_control_w(int offs, int data)
            {
                /* 0-15 : scrolling */
                if (offs < 16)
                {
                    int whichone = offs / 4;
                    int xy = offs & 2;
                    if (xy == 0)
                    { /* scroll x */
                        if ((offs & 1) != 0)
                            playfields[whichone].scroll_x = (playfields[whichone].scroll_x & 0xff00) | data;
                        else
                            playfields[whichone].scroll_x = (playfields[whichone].scroll_x & 0xff) | (data << 8);
                    }
                    else
                    { /* scroll y */
                        if ((offs & 1) != 0)
                            playfields[whichone].scroll_y = (playfields[whichone].scroll_y & 0xff00) | data;
                        else
                            playfields[whichone].scroll_y = (playfields[whichone].scroll_y & 0xff) | (data << 8);
                    }
                }
                /* 16-21 : priority */
                else if (offs < 22)
                {
                    /* bit 0-2 priority */
                    /* bit 3   disable	*/
                    int whichone = offs - 16;
                    objects[whichone].priority = data & 7;
                    objects[whichone].visible = (data & 0xf8) != 0 ? false : true;
#if NAMCOS1_DIRECT_DRAW
                    if (namcos1_tilemap_used != 0)
#endif
                        playfields[whichone].tilemap.enable = objects[whichone].visible;
                }
                /* 22,23 unused */
                else if (offs < 24)
                {
                }
                /* 24-29 palette */
                else if (offs < 30)
                {
                    int whichone = offs - 24;
                    if (playfields[whichone].color != (data & 7))
                    {
                        playfields[whichone].color = data & 7;
                        tilemap_palette_state[whichone] = 1;
                    }
                }
            }

            int namcos1_videoram_r(int offset)
            {
                return namcos1_videoram[offset];
            }

            void namcos1_videoram_w(int offset, int data)
            {
                if (namcos1_videoram[offset] != data)
                {
                    namcos1_videoram[offset] = (byte)data;
#if NAMCOS1_DIRECT_DRAW
                    if (namcos1_tilemap_used != 0)
                    {
#endif
                        if (offset < FG_OFFSET)
                        {	/* background 0-3 */
                            int layer = offset / 0x2000;
                            int num = (offset &= 0x1fff) / 2;
                            Mame.tilemap_mark_tile_dirty(playfields[layer].tilemap, num % 64, num / 64);
                        }
                        else
                        {	/* foreground 4-5 */
                            int layer = (offset & 0x800) != 0 ? 5 : 4;
                            int num = ((offset & 0x7ff) - 0x10) / 2;
                            if (num >= 0 && num < 0x3f0)
                                Mame.tilemap_mark_tile_dirty(playfields[layer].tilemap, num % 36, num / 36);
                        }
#if NAMCOS1_DIRECT_DRAW
                    }
#endif
                }
            }

            int namcos1_paletteram_r(int offset)
            {
                return namcos1_paletteram[offset];
            }

            void namcos1_paletteram_w(int offset, int data)
            {
                if (namcos1_paletteram[offset] != data)
                {
                    namcos1_paletteram[offset] = (byte)data;
                    if ((offset & 0x1fff) < 0x1800)
                    {
                        if (offset < 0x2000)
                        {
                            sprite_palette_state[(offset & 0x7f0) / 16] = 1;
                        }
                        else
                        {
                            int i, color;

                            color = (offset & 0x700) / 256;
                            for (i = 0; i < MAX_PLAYFIELDS; i++)
                            {
                                if (playfields[i].color == color)
                                    tilemap_palette_state[i] = 1;
                            }
                        }
                    }
                }
            }

            static void namcos1_palette_refresh(int start, int offset, int num)
            {
                int color;

                offset = (offset / 0x800) * 0x2000 + (offset & 0x7ff);

                for (color = start; color < start + num; color++)
                {
                    int r, g, b;
                    r = namcos1_paletteram[offset];
                    g = namcos1_paletteram[offset + 0x0800];
                    b = namcos1_paletteram[offset + 0x1000];
                    Mame.palette_change_color(color, (byte)r, (byte)g, (byte)b);

                    if (offset >= 0x2000)
                    {
                        r = namcos1_paletteram[offset + 0x2000];
                        g = namcos1_paletteram[offset + 0x2800];
                        b = namcos1_paletteram[offset + 0x3000];
                        Mame.palette_change_color(color + TILECOLORS, (byte)r, (byte)g, (byte)b);
                    }
                    offset++;
                }
            }
            static void namcos1_displaycontrol_w(int offset, int data)
            {
                _BytePtr disp_reg = new _BytePtr(namcos1_controlram, 0xff0);
                int newflip;

                switch (offset)
                {
                    case 0x02: /* ?? */
                        break;
                    case 0x04: /* sprite offset X */
                    case 0x05:
                        sprite_fixed_sx = disp_reg[4] * 256 + disp_reg[5] - 151;
                        if (sprite_fixed_sx > 480) sprite_fixed_sx -= 512;
                        if (sprite_fixed_sx < -32) sprite_fixed_sx += 512;
                        break;
                    case 0x06: /* flip screen */
                        newflip = (disp_reg[6] & 1) ^ 0x01;
                        if (flipscreen != newflip)
                        {
                            namcos1_set_flipscreen(newflip);
                        }
                        break;
                    case 0x07: /* sprite offset Y */
                        sprite_fixed_sy = 239 - disp_reg[7];
                        break;
                    case 0x0a: /* ?? */
                        /* 00 : blazer,dspirit,quester */
                        /* 40 : others */
                        break;
                    case 0x0e: /* ?? */
                    /* 00 : blazer,dangseed,dspirit,pacmania,quester */
                    /* 06 : others */
                    case 0x0f: /* ?? */
                        /* 00 : dangseed,dspirit,pacmania */
                        /* f1 : blazer */
                        /* f8 : galaga88,quester */
                        /* e7 : others */
                        break;
                }
#if false
	{
		char buf[80];
		sprintf(buf,"%02x:%02x:%02x:%02x:%02x%02x,%02x,%02x,%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x",
		disp_reg[0],disp_reg[1],disp_reg[2],disp_reg[3],
		disp_reg[4],disp_reg[5],disp_reg[6],disp_reg[7],
		disp_reg[8],disp_reg[9],disp_reg[10],disp_reg[11],
		disp_reg[12],disp_reg[13],disp_reg[14],disp_reg[15]);
		usrintf_showmessage(buf);
	}
#endif
            }
#if NAMCOS1_DIRECT_DRAW
            static void draw_background(Mame.osd_bitmap bitmap, int layer)
            {
                _BytePtr vid = playfields[layer]._base;
                int width = playfields[layer].width;
                int height = playfields[layer].height;
                int color = objects[layer].color;
                int scrollx = playfields[layer].scroll_x;
                int scrolly = playfields[layer].scroll_y;
                int sx, sy;
                int offs_x, offs_y;
                int ox, xx;
                int max_x = Mame.Machine.drv.visible_area.max_x;
                int max_y = Mame.Machine.drv.visible_area.max_y;
                int code;

                scrollx -= scrolloffsX[layer];
                scrolly -= scrolloffsY[layer];

                if (flipscreen != 0)
                {
                    scrollx = -scrollx;
                    scrolly = -scrolly;
                }

                if (scrollx < 0) scrollx = width - (-scrollx) % width;
                else scrollx %= width;
                if (scrolly < 0) scrolly = height - (-scrolly) % height;
                else scrolly %= height;

                width /= 8;
                height /= 8;
                sx = (scrollx % 8);
                offs_x = width - (scrollx / 8);
                sy = (scrolly % 8);
                offs_y = height - (scrolly / 8);
                if (sx > 0)
                {
                    sx -= 8;
                    offs_x--;
                }
                if (sy > 0)
                {
                    sy -= 8;
                    offs_y--;
                }

                /* draw for visible area */
                offs_x *= 2;
                width *= 2;
                offs_y *= width;
                height = height * width;
                for (; sy <= max_y; offs_y += width, sy += 8)
                {
                    offs_y %= height;
                    for (ox = offs_x, xx = sx; xx <= max_x; ox += 2, xx += 8)
                    {
                        ox %= width;
                        code = vid[offs_y + ox + 1] + ((vid[offs_y + ox] & 0x3f) << 8);
                        if (char_state[code] != CHAR_BLANK)
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code, (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    flipscreen != 0 ? max_x - 7 - xx : xx,
                                    flipscreen != 0 ? max_y - 7 - sy : sy,
                                    Mame.Machine.drv.visible_area,
                                    (char_state[code] == CHAR_FULL) ? Mame.TRANSPARENCY_NONE : Mame.TRANSPARENCY_PEN,
                                    char_state[code]);
                        }
                    }
                }
            }

            static void draw_foreground(Mame.osd_bitmap bitmap, int layer)
            {
                int offs;
                _BytePtr vid = playfields[layer]._base;
                int color = objects[layer].color;
                int max_x = Mame.Machine.drv.visible_area.max_x;
                int max_y = Mame.Machine.drv.visible_area.max_y;

                for (offs = 0; offs < 36 * 28 * 2; offs += 2)
                {
                    int sx, sy, code;

                    code = vid[offs + 1] + ((vid[offs + 0] & 0x3f) << 8);
                    if (char_state[code] != CHAR_BLANK)
                    {
                        sx = ((offs / 2) % 36) * 8;
                        sy = ((offs / 2) / 36) * 8;
                        if (flipscreen != 0)
                        {
                            sx = max_x - 7 - sx;
                            sy = max_y - 7 - sy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)code, (uint)color,
                                flipscreen != 0, flipscreen != 0,
                                sx, sy,
                                Mame.Machine.drv.visible_area,
                                (char_state[code] == CHAR_FULL) ? Mame.TRANSPARENCY_NONE : Mame.TRANSPARENCY_PEN,
                                char_state[code]);
                    }
                }
            }
#endif
            void ns1_draw_tilemap(Mame.osd_bitmap bitmap, Mame.gfx_object _object)
            {
                int layer = _object.code;
#if NAMCOS1_DIRECT_DRAW
                if (namcos1_tilemap_used != 0)
#endif
                    Mame.tilemap_draw(bitmap, playfields[layer].tilemap, 0);
#if NAMCOS1_DIRECT_DRAW
                else
                {
                    if (layer < 4)
                        draw_background(bitmap, layer);
                    else
                        draw_foreground(bitmap, layer);
                }
#endif
            }
            static void namcos1_spriteram_w(int offset, int data)
            {
                int[] sprite_sizemap = { 16, 8, 32, 4 };
                int num = offset / 0x10;
                Mame.gfx_object _object = objectlist.objects[num + MAX_PLAYFIELDS];
                _BytePtr _base = new _BytePtr(namcos1_controlram, 0x0800 + num * 0x10);
                int sx, sy;
                int resize_x = 0, resize_y = 0;

                switch (offset & 0x0f)
                {
                    case 0x04:
                        /* bit.6-7 : x size (16/8/32/4) */
                        /* bit.5   : flipx */
                        /* bit.3-4 : x offset */
                        /* bit.0-2 : code.8-10 */
                        _object.width = sprite_sizemap[(data >> 6) & 3];
                        _object.flipx = ((data >> 5) & 1) ^ flipscreen;
                        _object.left = (data & 0x18) & (~(_object.width - 1));
                        _object.code = (_base[4] & 7) * 256 + _base[5];
                        resize_x = 1;
                        break;
                    case 0x05:
                        /* bit.0-7 : code.0-7 */
                        _object.code = (_base[4] & 7) * 256 + _base[5];
                        break;
                    case 0x06:
                        /* bit.1-7 : color */
                        /* bit.0   : x draw position.8 */
                        _object.color = data >> 1;
                        _object.transparency = _object.color == 0x7f ? Mame.TRANSPARENCY_PEN_TABLE : Mame.TRANSPARENCY_PEN;
#if false
		if(_object.color==0x7f && !(Machine.gamedrv.flags & GAME_REQUIRES_16BIT))
			usrintf_showmessage("This driver requires GAME_REQUIRES_16BIT flag");
#endif
                        goto case 0x07;
                    case 0x07:
                        /* bit.0-7 : x draw position.0-7 */
                        resize_x = 1;
                        break;
                    case 0x08:
                        /* bit.5-7 : priority */
                        /* bit.3-4 : y offset */
                        /* bit.1-2 : y size (16/8/32/4) */
                        /* bit.0   : flipy */
                        _object.priority = (data >> 5) & 7;
                        _object.height = sprite_sizemap[(data >> 1) & 3];
                        _object.flipy = (data & 1) ^ flipscreen;
                        _object.top = (data & 0x18) & (~(_object.height - 1));
                        goto case 0x09;
                    case 0x09:
                        /* bit.0-7 : y draw position */
                        resize_y = 1;
                        break;
                    default:
                        return;
                }
                if (resize_x != 0)
                {
                    /* sx */
                    sx = (_base[6] & 1) * 256 + _base[7];
                    sx += sprite_fixed_sx;

                    if (flipscreen != 0) sx = 210 - sx - _object.width;

                    if (sx > 480) sx -= 512;
                    if (sx < -32) sx += 512;
                    if (sx < -224) sx += 512;
                    _object.sx = sx;
                }
                if (resize_y != 0)
                {
                    /* sy */
                    sy = sprite_fixed_sy - _base[9];

                    if (flipscreen != 0) sy = 222 - sy;
                    else sy = sy - _object.height;

                    if (sy > 224) sy -= 256;
                    if (sy < -32) sy += 256;
                    _object.sy = sy;
                }
                _object.dirty_flag = Mame.GFXOBJ_DIRTY_ALL;
            }

            public override int vh_start()
            {
                int i;
                Mame.gfx_object default_object;

#if NAMCOS1_DIRECT_DRAW
                /* tilemap used flag select */
                if (Mame.Machine.scrbitmap.depth == 16)
                    /* tilemap system is not supported 16bit yet */
                    namcos1_tilemap_used = 0;
                else
                    /* selected by game option switch */
                    namcos1_tilemap_used = namcos1_tilemap_need;
#endif

                /* set table for sprite color == 0x7f */
                for (i = 0; i <= 15; i++)
                    Mame.gfx_drawmode_table[i] = Mame.DRAWMODE_SHADOW;

                /* set static memory points */
                namcos1_paletteram = Mame.memory_region(Mame.REGION_USER2);
                namcos1_controlram = new _BytePtr(Mame.memory_region(Mame.REGION_USER2), 0x8000);

                /* allocate videoram */
                namcos1_videoram = new _BytePtr(0x8000);

                //memset(namcos1_videoram,0,0x8000);

                /* initialize object manager */
                default_object = new Mame.gfx_object();
                default_object.transparency = Mame.TRANSPARENCY_PEN;
                default_object.transparent_color = 15;
                default_object.gfx = Mame.Machine.gfx[2];
                objectlist = Mame.gfxobj_create(MAX_PLAYFIELDS + MAX_SPRITES, 8, default_object);
                if (objectlist == null)
                {
                    namcos1_videoram = null;
                    return 1;
                }
                objects = objectlist.objects;

                /* setup tilemap parameter to objects */
                for (i = 0; i < MAX_PLAYFIELDS; i++)
                {
                    /* set user draw handler */
                    objects[i].special_handler = ns1_draw_tilemap;
                    objects[i].gfx = null;
                    objects[i].code = i;
                    objects[i].visible = false;
                    objects[i].color = i;
                }

                /* initialize playfields */
                for (i = 0; i < MAX_PLAYFIELDS; i++)
                {
#if NAMCOS1_DIRECT_DRAW
                    if (namcos1_tilemap_used != 0)
                    {
#endif
                        if (i < 4)
                        {
                            playfields[i]._base = new _BytePtr(namcos1_videoram, i << 13);
                            playfields[i].tilemap =
                                Mame.tilemap_create(background_get_info, Mame.TILEMAP_BITMASK
                                                , 8, 8
                                                , 64, i == 3 ? 32 : 64);
                        }
                        else
                        {
                            playfields[i]._base = new _BytePtr(namcos1_videoram, FG_OFFSET + 0x10 + ((i - 4) * 0x800));
                            playfields[i].tilemap =
                                Mame.tilemap_create(foreground_get_info, Mame.TILEMAP_BITMASK
                                                , 8, 8
                                                , 36, 28);
                        }
#if NAMCOS1_DIRECT_DRAW
                    }
                    else
                    {
                        if (i < 4)
                        {
                            playfields[i]._base = new _BytePtr(namcos1_videoram, i << 13);
                            playfields[i].width = 64 * 8;
                            playfields[i].height = (i == 3) ? 32 * 8 : 64 * 8;
                        }
                        else
                        {
                            playfields[i]._base = new _BytePtr(namcos1_videoram, FG_OFFSET + 0x10 + ((i - 4) * 0x800));
                            playfields[i].width = 36 * 8;
                            playfields[i].height = 28 * 8;
                        }
                    }
#endif
                    playfields[i].scroll_x = 0;
                    playfields[i].scroll_y = 0;
                }
                namcos1_set_flipscreen(0);

                /* initialize sprites and display controller */
                for (i = 0; i < 0x7ef; i++)
                    namcos1_spriteram_w(i, 0);
                for (i = 0; i < 0xf; i++)
                    namcos1_displaycontrol_w(i, 0);
                for (i = 0; i < 0xff; i++)
                    namcos1_playfield_control_w(i, 0);

#if NAMCOS1_DIRECT_DRAW
                if (namcos1_tilemap_used != 0)
                {
#endif
                    /* build tilemap mask data from gfx data of mask */
                    /* because this driver use ORIENTATION_ROTATE_90 */
                    /* mask data can't made by ROM image             */
                    {
                        Mame.GfxElement mask = Mame.Machine.gfx[0];
                        int total = (int)mask.total_elements;
                        int width = mask.width;
                        int height = mask.height;
                        int line, x, c;

                        mask_ptr = new _BytePtr[total];
                        if (mask_ptr == null)
                        {
                            namcos1_videoram = null;
                            return 1;
                        }
                        mask_data = new _BytePtr(total * 8);
                        if (mask_data == null)
                        {
                            namcos1_videoram = null;
                            mask_ptr = null;
                            return 1;
                        }

                        for (c = 0; c < total; c++)
                        {
                            _BytePtr src_mask = new _BytePtr(mask_data, c * 8);
                            for (line = 0; line < height; line++)
                            {
                                _BytePtr maskbm = get_gfx_pointer(mask, c, line);
                                src_mask[line] = 0;
                                for (x = 0; x < width; x++)
                                {
                                    src_mask[line] |= (byte)(maskbm[x] << (7 - x));
                                }
                            }
                            mask_ptr[c] = src_mask;
                            if (mask.pen_usage != null)
                            {
                                switch (mask.pen_usage[c])
                                {
                                    case 0x01: mask_ptr[c][0] = Mame.TILEMAP_BITMASK_TRANSPARENT; break; /* blank */
                                    case 0x02: mask_ptr[c][0] = unchecked((byte)Mame.TILEMAP_BITMASK_OPAQUE); break; /* full */
                                }
                            }
                        }
                    }

#if NAMCOS1_DIRECT_DRAW
                }
                else /* namcos1_tilemap_used */
                {

                    /* build char mask status table */
                    {
                        Mame.GfxElement mask = Mame.Machine.gfx[0];
                        Mame.GfxElement pens = Mame.Machine.gfx[1];
                        int total = (int)mask.total_elements;
                        int width = mask.width;
                        int height = mask.height;
                        int line, x, c;

                        char_state = new _BytePtr(total);
                        if (char_state == null)
                        {
                            namcos1_videoram = null;
                            return 1;
                        }

                        for (c = 0; c < total; c++)
                        {
                            byte ordata = 0;
                            byte anddata = 0xff;
                            for (line = 0; line < height; line++)
                            {
                                _BytePtr maskbm = get_gfx_pointer(mask, c, line);
                                for (x = 0; x < width; x++)
                                {
                                    ordata |= maskbm[x];
                                    anddata &= maskbm[x];
                                }
                            }
                            if (ordata == 0) char_state[c] = CHAR_BLANK;
                            else if (anddata != 0) char_state[c] = CHAR_FULL;
                            else
                            {
                                /* search non used pen */
                                byte[] penmap = new byte[256];
                                byte trans_pen;
                                Array.Clear(penmap, 0, 256);
                                for (line = 0; line < height; line++)
                                {
                                    _BytePtr pensbm = get_gfx_pointer(pens, c, line);
                                    for (x = 0; x < width; x++)
                                        penmap[pensbm[x]] = 1;
                                }
                                for (trans_pen = 2; trans_pen < 256; trans_pen++)
                                {
                                    if (penmap[trans_pen] == 0) break;
                                }
                                char_state[c] = trans_pen; /* transparency color */
                                /* fill transparency color */
                                for (line = 0; line < height; line++)
                                {
                                    _BytePtr maskbm = get_gfx_pointer(mask, c, line);
                                    _BytePtr pensbm = get_gfx_pointer(pens, c, line);
                                    for (x = 0; x < width; x++)
                                    {
                                        if (maskbm[x] == 0) pensbm[x] = trans_pen;
                                    }
                                }
                            }
                        }
                    }

                } /* namcos1_tilemap_used */
#endif

                for (i = 0; i < TILECOLORS; i++)
                {
                    Mame.palette_shadow_table[Mame.Machine.pens[i + SPRITECOLORS]] = Mame.Machine.pens[i + SPRITECOLORS + TILECOLORS];
                }

                return 0;
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                throw new NotImplementedException();
            }
        }
        static int rev1_key_r(int offset)
        {
            //	if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: keychip read %04X=%02x\n",cpu_getactivecpu(),cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static ushort divider1, divide_321 = 0;
        static ushort d1;
        static void rev1_key_w(int offset, int data)
        {
            //if(errorlog) fprintf(errorlog,"CPU #%d PC %08x: keychip write %04X=%02x\n",cpu_getactivecpu(),cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }

            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x01:
                    divider1 = (ushort)((key[0] << 8) + key[1]);
                    break;
                case 0x03:
                    {

                        ushort v1, v2;
                        ulong l = 0;

                        if (divide_321 != 0)
                            l = (ulong)(d1 << 16);

                        d1 = (ushort)((key[2] << 8) + key[3]);

                        if (divider1 == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            if (divide_321 != 0)
                            {
                                l |= d1;

                                v1 = (ushort)(l / divider1);
                                v2 = (ushort)(l % divider1);
                            }
                            else
                            {
                                v1 = (ushort)(d1 / divider1);
                                v2 = (ushort)(d1 % divider1);
                            }
                        }

                        key[2] = (byte)(v1 >> 8);
                        key[3] = (byte)v1;
                        key[0] = (byte)(v2 >> 8);
                        key[1] = (byte)v2;
                    }
                    break;
                case 0x04:
                    if (key[4] == key_id_query) /* get key number */
                        key[4] = (byte)key_id;

                    if (key[4] == 0x0c)
                        divide_321 = 1;
                    else
                        divide_321 = 0;
                    break;
            }
        }



        class namcos1_slice_timer
        {
            public int sync_cpu;	/* synchronus cpu attribute */
            public int sliceHz;	/* slice cycle				*/
            public int delayHz;	/* delay>=0 : delay cycle	*/
            /* delay<0	: slide cycle	*/
        };
        class namcos1_specific
        {
            public int key_id_query, key_id;
            public Mame.mem_read_handler key_r;
            public Mame.mem_write_handler key_w;
            /* cpu slice timer */
            public namcos1_slice_timer[] slice_timer;
            /* optimize flag , use tilemap for playfield */
            public int tilemap_use;

            public namcos1_specific(int key_id_query, int key_id, Mame.mem_read_handler key_r, Mame.mem_write_handler key_w,
                namcos1_slice_timer[] slice_timer, int tilemap_use)
            {
                this.key_id_query = key_id_query;
                this.key_id = key_id;
                this.key_r = key_r;
                this.key_w = key_w;
                this.slice_timer = slice_timer;
                this.tilemap_use = tilemap_use;
            }
        }
        static _BytePtr sound_spinlock_ram;
        static int sound_spinlock_pc;

        static int namcos1_setopbase_0(int pc)
        {
            int bank = (pc >> 13) & 7;
            if (namcos1_banks[0, bank].bank_pointer == null)
            {
                switch (-bank << 13)
                {
                    case 0: Mame.OP_RAM = Mame.OP_ROM = null; break;
                    default: throw new Exception();
                }
            }
            else
                Mame.OP_RAM = Mame.OP_ROM = new _BytePtr(namcos1_banks[0, bank].bank_pointer, -bank << 13);
            /* memory.c output warning - op-code execute on mapped i/o	*/
            /* but it is necessary to continue cpu_setOPbase16 function */
            /* for update current operationhardware(ophw) code			*/
            return pc;
        }

        static int namcos1_setopbase_1(int pc)
        {
            int bank = (pc >> 13) & 7;
            if (namcos1_banks[1, bank].bank_pointer == null)
            {
                switch (-bank << 13)
                {
                    case 0: Mame.OP_RAM = Mame.OP_ROM = null; break;
                    default: throw new Exception();
                }
            }
            else Mame.OP_RAM = Mame.OP_ROM = new _BytePtr(namcos1_banks[1, bank].bank_pointer, -bank << 13);
            /* memory.c output warning - op-code execute on mapped i/o	*/
            /* but it is necessary to continue cpu_setOPbase16 function */
            /* for update current operationhardware(ophw) code			*/
            return pc;
        }

        static void namcos1_install_bank(int start, int end, Mame.mem_read_handler hr, Mame.mem_write_handler hw,
                      int offset, _BytePtr pointer)
        {
            int i;
            for (i = start; i <= end; i++)
            {
                namcos1_bank_element[i].bank_handler_r = hr;
                namcos1_bank_element[i].bank_handler_w = hw;
                namcos1_bank_element[i].bank_offset = offset;
                namcos1_bank_element[i].bank_pointer = pointer;
                offset += 0x2000;
                if (pointer != null) pointer.offset += 0x2000;
            }
        }

        static void namcos1_install_rom_bank(int start, int end, int size, int offset)
        {
            _BytePtr BROM = Mame.memory_region(Mame.REGION_USER1);
            int step = size / 0x2000;
            while (start < end)
            {
                namcos1_install_bank(start, start + step - 1, null, rom_w, 0, new _BytePtr(BROM, offset));
                start += step;
            }
        }
        static int namcos1_videoram_r(int offset)
        {
            return namcos1_videoram[offset];
        }
        static void namcos1_videoram_w(int offset, int data)
        {
            if (namcos1_videoram[offset] != data)
            {
                namcos1_videoram[offset] = (byte)data;
#if NAMCOS1_DIRECT_DRAW
                if (namcos1_tilemap_used != 0)
                {
#endif
                    if (offset < FG_OFFSET)
                    {	/* background 0-3 */
                        int layer = offset / 0x2000;
                        int num = (offset &= 0x1fff) / 2;
                        Mame.tilemap_mark_tile_dirty(playfields[layer].tilemap, num % 64, num / 64);
                    }
                    else
                    {	/* foreground 4-5 */
                        int layer = (offset & 0x800) != 0 ? 5 : 4;
                        int num = ((offset & 0x7ff) - 0x10) / 2;
                        if (num >= 0 && num < 0x3f0)
                            Mame.tilemap_mark_tile_dirty(playfields[layer].tilemap, num % 36, num / 36);
                    }
#if NAMCOS1_DIRECT_DRAW
                }
#endif
            }
        }
        static void namcos1_playfield_control_w(int offs, int data)
        {
            /* 0-15 : scrolling */
            if (offs < 16)
            {
                int whichone = offs / 4;
                int xy = offs & 2;
                if (xy == 0)
                { /* scroll x */
                    if ((offs & 1) != 0)
                        playfields[whichone].scroll_x = (playfields[whichone].scroll_x & 0xff00) | data;
                    else
                        playfields[whichone].scroll_x = (playfields[whichone].scroll_x & 0xff) | (data << 8);
                }
                else
                { /* scroll y */
                    if ((offs & 1) != 0)
                        playfields[whichone].scroll_y = (playfields[whichone].scroll_y & 0xff00) | data;
                    else
                        playfields[whichone].scroll_y = (playfields[whichone].scroll_y & 0xff) | (data << 8);
                }
            }
            /* 16-21 : priority */
            else if (offs < 22)
            {
                /* bit 0-2 priority */
                /* bit 3   disable	*/
                int whichone = offs - 16;
                objects[whichone].priority = data & 7;
                objects[whichone].visible = (data & 0xf8) != 0 ? false : true;
#if NAMCOS1_DIRECT_DRAW
                if (namcos1_tilemap_used != 0)
#endif
                    playfields[whichone].tilemap.enable = objects[whichone].visible;
            }
            /* 22,23 unused */
            else if (offs < 24)
            {
            }
            /* 24-29 palette */
            else if (offs < 30)
            {
                int whichone = offs - 24;
                if (playfields[whichone].color != (data & 7))
                {
                    playfields[whichone].color = data & 7;
                    tilemap_palette_state[whichone] = 1;
                }
            }
        }
        static void namcos1_spriteram_w(int offset, int data)
        {
            int[] sprite_sizemap = { 16, 8, 32, 4 };
            int num = offset / 0x10;
            Mame.gfx_object _object = objectlist.objects[num + MAX_PLAYFIELDS];
            _BytePtr _base = new _BytePtr(namcos1_controlram, 0x0800 + num * 0x10);
            int sx, sy;
            int resize_x = 0, resize_y = 0;

            switch (offset & 0x0f)
            {
                case 0x04:
                    /* bit.6-7 : x size (16/8/32/4) */
                    /* bit.5   : flipx */
                    /* bit.3-4 : x offset */
                    /* bit.0-2 : code.8-10 */
                    _object.width = sprite_sizemap[(data >> 6) & 3];
                    _object.flipx = ((data >> 5) & 1) ^ flipscreen;
                    _object.left = (data & 0x18) & (~(_object.width - 1));
                    _object.code = (_base[4] & 7) * 256 + _base[5];
                    resize_x = 1;
                    break;
                case 0x05:
                    /* bit.0-7 : code.0-7 */
                    _object.code = (_base[4] & 7) * 256 + _base[5];
                    break;
                case 0x06:
                    /* bit.1-7 : color */
                    /* bit.0   : x draw position.8 */
                    _object.color = data >> 1;
                    _object.transparency = _object.color == 0x7f ? Mame.TRANSPARENCY_PEN_TABLE : Mame.TRANSPARENCY_PEN;
#if false
		if(_object.color==0x7f && !(Machine.gamedrv.flags & GAME_REQUIRES_16BIT))
			usrintf_showmessage("This driver requires GAME_REQUIRES_16BIT flag");
#endif
                    goto case 0x07;
                case 0x07:
                    /* bit.0-7 : x draw position.0-7 */
                    resize_x = 1;
                    break;
                case 0x08:
                    /* bit.5-7 : priority */
                    /* bit.3-4 : y offset */
                    /* bit.1-2 : y size (16/8/32/4) */
                    /* bit.0   : flipy */
                    _object.priority = (data >> 5) & 7;
                    _object.height = sprite_sizemap[(data >> 1) & 3];
                    _object.flipy = (data & 1) ^ flipscreen;
                    _object.top = (data & 0x18) & (~(_object.height - 1));
                    goto case 0x09;
                case 0x09:
                    /* bit.0-7 : y draw position */
                    resize_y = 1;
                    break;
                default:
                    return;
            }
            if (resize_x != 0)
            {
                /* sx */
                sx = (_base[6] & 1) * 256 + _base[7];
                sx += sprite_fixed_sx;

                if (flipscreen != 0) sx = 210 - sx - _object.width;

                if (sx > 480) sx -= 512;
                if (sx < -32) sx += 512;
                if (sx < -224) sx += 512;
                _object.sx = sx;
            }
            if (resize_y != 0)
            {
                /* sy */
                sy = sprite_fixed_sy - _base[9];

                if (flipscreen != 0) sy = 222 - sy;
                else sy = sy - _object.height;

                if (sy > 224) sy -= 256;
                if (sy < -32) sy += 256;
                _object.sy = sy;
            }
            _object.dirty_flag = Mame.GFXOBJ_DIRTY_ALL;
        }

        static void namcos1_videocontrol_w(int offset, int data)
        {
            namcos1_controlram[offset] = (byte)data;
            /* 0000-07ff work ram */
            if (offset <= 0x7ff)
                return;
            /* 0800-0fef sprite ram */
            if (offset <= 0x0fef)
            {
                namcos1_spriteram_w(offset & 0x7ff, data);
                return;
            }
            /* 0ff0-0fff display control ram */
            if (offset <= 0x0fff)
            {
                namcos1_displaycontrol_w(offset & 0x0f, data);
                return;
            }
            /* 1000-1fff control ram */
            namcos1_playfield_control_w(offset & 0xff, data);
        }
        static void namcos1_set_flipscreen(int flip)
        {
            int i;

            int[] pos_x = { 0x0b0, 0x0b2, 0x0b3, 0x0b4 };
            int[] pos_y = { 0x108, 0x108, 0x108, 0x008 };
            int[] neg_x = { 0x1d0, 0x1d2, 0x1d3, 0x1d4 };
            int[] neg_y = { 0x1e8, 0x1e8, 0x1e8, 0x0e8 };

            flipscreen = flip;
            if (flip == 0)
            {
                for (i = 0; i < 4; i++)
                {
                    scrolloffsX[i] = pos_x[i];
                    scrolloffsY[i] = pos_y[i];
                }
            }
            else
            {
                for (i = 0; i < 4; i++)
                {
                    scrolloffsX[i] = neg_x[i];
                    scrolloffsY[i] = neg_y[i];
                }
            }
#if NAMCOS1_DIRECT_DRAW
            if (namcos1_tilemap_used != 0)
#endif
                Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? Mame.TILEMAP_FLIPX | Mame.TILEMAP_FLIPY : 0);
        }

        static void namcos1_displaycontrol_w(int offset, int data)
        {
            _BytePtr disp_reg = new _BytePtr(namcos1_controlram, 0xff0);
            int newflip;

            switch (offset)
            {
                case 0x02: /* ?? */
                    break;
                case 0x04: /* sprite offset X */
                case 0x05:
                    sprite_fixed_sx = disp_reg[4] * 256 + disp_reg[5] - 151;
                    if (sprite_fixed_sx > 480) sprite_fixed_sx -= 512;
                    if (sprite_fixed_sx < -32) sprite_fixed_sx += 512;
                    break;
                case 0x06: /* flip screen */
                    newflip = (disp_reg[6] & 1) ^ 0x01;
                    if (flipscreen != newflip)
                    {
                        namcos1_set_flipscreen(newflip);
                    }
                    break;
                case 0x07: /* sprite offset Y */
                    sprite_fixed_sy = 239 - disp_reg[7];
                    break;
                case 0x0a: /* ?? */
                    /* 00 : blazer,dspirit,quester */
                    /* 40 : others */
                    break;
                case 0x0e: /* ?? */
                /* 00 : blazer,dangseed,dspirit,pacmania,quester */
                /* 06 : others */
                case 0x0f: /* ?? */
                    /* 00 : dangseed,dspirit,pacmania */
                    /* f1 : blazer */
                    /* f8 : galaga88,quester */
                    /* e7 : others */
                    break;
            }
#if false
	{
		char buf[80];
		sprintf(buf,"%02x:%02x:%02x:%02x:%02x%02x,%02x,%02x,%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x",
		disp_reg[0],disp_reg[1],disp_reg[2],disp_reg[3],
		disp_reg[4],disp_reg[5],disp_reg[6],disp_reg[7],
		disp_reg[8],disp_reg[9],disp_reg[10],disp_reg[11],
		disp_reg[12],disp_reg[13],disp_reg[14],disp_reg[15]);
		usrintf_showmessage(buf);
	}
#endif
        }

        static void namcos1_build_banks(Mame.mem_read_handler key_r, Mame.mem_write_handler key_w)
        {
            int i;

            /* S1 RAM pointer set */
            s1ram = Mame.memory_region(Mame.REGION_USER2);

            /* clear all banks to unknown area */
            for (i = 0; i < NAMCOS1_MAX_BANK; i++)
                namcos1_install_bank(i, i, unknown_r, unknown_w, 0, null);

            /* RAM 6 banks - palette */
            namcos1_install_bank(0x170, 0x172, namcos1_paletteram_r, namcos1_paletteram_w, 0, s1ram);
            /* RAM 6 banks - work ram */
            namcos1_install_bank(0x173, 0x173, null, null, 0, new _BytePtr(s1ram, 0x6000));
            /* RAM 5 banks - videoram */
            namcos1_install_bank(0x178, 0x17b, namcos1_videoram_r, namcos1_videoram_w, 0, null);
            /* key chip bank (rev1_key_w / rev2_key_w ) */
            namcos1_install_bank(0x17c, 0x17c, key_r, key_w, 0, null);
            /* RAM 7 banks - display control, playfields, sprites */
            namcos1_install_bank(0x17e, 0x17e, null, namcos1_videocontrol_w, 0, new _BytePtr(s1ram, 0x8000));
            /* RAM 1 shared ram, PSG device */
            namcos1_install_bank(0x17f, 0x17f, soundram_r, soundram_w, 0, Namco.namco_wavedata);
            /* RAM 3 banks */
            namcos1_install_bank(0x180, 0x183, null, null, 0, new _BytePtr(s1ram, 0xc000));
            /* PRG0 */
            namcos1_install_rom_bank(0x200, 0x23f, 0x20000, 0xe0000);
            /* PRG1 */
            namcos1_install_rom_bank(0x240, 0x27f, 0x20000, 0xc0000);
            /* PRG2 */
            namcos1_install_rom_bank(0x280, 0x2bf, 0x20000, 0xa0000);
            /* PRG3 */
            namcos1_install_rom_bank(0x2c0, 0x2ff, 0x20000, 0x80000);
            /* PRG4 */
            namcos1_install_rom_bank(0x300, 0x33f, 0x20000, 0x60000);
            /* PRG5 */
            namcos1_install_rom_bank(0x340, 0x37f, 0x20000, 0x40000);
            /* PRG6 */
            namcos1_install_rom_bank(0x380, 0x3bf, 0x20000, 0x20000);
            /* PRG7 */
            namcos1_install_rom_bank(0x3c0, 0x3ff, 0x20000, 0x00000);
        }
        static void namcos1_set_optimize(int optimize)
        {
#if NAMCOS1_DIRECT_DRAW
            namcos1_tilemap_need = optimize;
#endif
        }
        static void namcos1_driver_init(namcos1_specific specific)
        {
            /* keychip id */
            key_id_query = specific.key_id_query;
            key_id = specific.key_id;

            /* tilemap use optimize option */
            namcos1_set_optimize(specific.tilemap_use);

            /* build bank elements */
            namcos1_build_banks(specific.key_r, specific.key_w);

            /* override opcode handling for extended memory bank handler */
            Mame.cpu_setOPbaseoverride(0, namcos1_setopbase_0);
            Mame.cpu_setOPbaseoverride(1, namcos1_setopbase_1);

            /* sound cpu speedup optimize (auto detect) */
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU3); /* sound cpu */
                int addr, flag_ptr;

                for (addr = 0xd000; addr < 0xd0ff; addr++)
                {
                    if (RAM[addr + 0] == 0xb6 &&   /* lda xxxx */
                       RAM[addr + 3] == 0x27 &&   /* BEQ addr */
                       RAM[addr + 4] == 0xfb)
                    {
                        flag_ptr = RAM[addr + 1] * 256 + RAM[addr + 2];
                        if (flag_ptr > 0x5140 && flag_ptr < 0x5400)
                        {
                            sound_spinlock_pc = addr + 3;
                            sound_spinlock_ram = Mame.install_mem_read_handler(2, flag_ptr, flag_ptr, namcos1_sound_spinlock_r);
                            Mame.printf("Set sound cpu spinlock : pc=%04x , addr = %04x\n", sound_spinlock_pc, flag_ptr);
                            break;
                        }
                    }
                }
            }
#if NEW_TIMER
	/* all cpu's does not need synchronization to all timers */
	cpu_set_full_synchronize(SYNC_NO_CPU);
	{
		const struct namcos1_slice_timer *slice = specific.slice_timer;
		while(slice.sync_cpu != SYNC_NO_CPU)
		{
			/* start CPU slice timer */
			cpu_start_extend_time_slice(slice.sync_cpu,
				TIME_IN_HZ(slice.delayHz),TIME_IN_HZ(slice.sliceHz) );
			slice++;
		}
	}
#else
            /* compatible with old timer system */
            Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_HZ(60 * 25), 0, null);
#endif
        }
        static int namcos1_sound_spinlock_r(int offset)
        {
            if (Mame.cpu_get_pc() == sound_spinlock_pc && sound_spinlock_ram[0] == 0)
                Mame.cpu_spinuntil_int();
            return sound_spinlock_ram[0];
        }
        public override void driver_init()
        {
            namcos1_specific pacmania_specific =
      new namcos1_specific(
          0x4b, 0x12,				/* key query , key id */
          rev1_key_r, rev1_key_w,	/* key handler */
          normal_slice,			/* CPU slice normal */
          1						/* use tilemap flag : speedup optimize */
      );
            namcos1_driver_init(pacmania_specific);
        }
        Mame.RomModule[] rom_namcos1()
        {
            throw new Exception();
        }
        Mame.InputPortTiny[] input_ports_namcos1()
        {
            throw new Exception();
        }
    }

    class driver_pacmania : driver_namcos1
    {
        public override void driver_init()
        {
            base.driver_init();
        }
        Mame.RomModule[] rom_pacmania()
        {
            ROM_START("pacmania");
            ROM_REGION(0x10000, Mame.REGION_CPU1);		/* 64k for the main cpu */
            /* Nothing loaded here. Bankswitching makes sure this gets the necessary code */

            ROM_REGION(0x10000, Mame.REGION_CPU2);		/* 64k for the sub cpu */
            /* Nothing loaded here. Bankswitching makes sure this gets the necessary code */

            ROM_REGION(0x2c000, Mame.REGION_CPU3);	/* 176k for the sound cpu */
            ROM_LOAD("pm_snd0.bin", 0x0c000, 0x10000, 0xc10370fa);
            ROM_LOAD("pm_snd1.bin", 0x1c000, 0x10000, 0xf761ed5a);

            ROM_REGION(0x100000, Mame.REGION_USER1);/* 1M for ROMs */
            driver_namcos1.ROM_LOAD_HS("pm_prg7.bin", 0x00000, 0x10000, 0x462fa4fd);
            ROM_LOAD("pm_prg6.bin", 0x20000, 0x20000, 0xfe94900c);

            ROM_REGION(0x14000, Mame.REGION_USER2);	/* 80k for RAM */

            ROM_REGION(0x30000, Mame.REGION_CPU4);	/* the MCU & voice */
            ROM_LOAD("ns1-mcu.bin", 0x0f000, 0x01000, 0xffb5c0bd);
            driver_namcos1.ROM_LOAD_HS("pm_voice.bin", 0x10000, 0x10000, 0x1ad5788f);

            ROM_REGION(0x20000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);/* character mask */
            ROM_LOAD("pm_chr8.bin", 0x00000, 0x10000, 0xf3afd65d);

            ROM_REGION(0x100000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE); /* characters */
            ROM_LOAD("pm_chr0.bin", 0x00000, 0x20000, 0x7c57644c);
            ROM_LOAD("pm_chr1.bin", 0x20000, 0x20000, 0x7eaa67ed);
            ROM_LOAD("pm_chr2.bin", 0x40000, 0x20000, 0x27e739ac);
            ROM_LOAD("pm_chr3.bin", 0x60000, 0x20000, 0x1dfda293);

            ROM_REGION(0x100000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE); /* sprites */
            ROM_LOAD("pm_obj0.bin", 0x00000, 0x20000, 0xfda57e8b);
            ROM_LOAD("pm_obj1.bin", 0x20000, 0x20000, 0x4c08affe);
            return ROM_END;
        }
        public driver_pacmania()
        {
            drv = new driver_namcos1.machine_driver_namcos1();
            year = "1987";
            name = "pacmania";
            description = "Pac-Mania";
            manufacturer = "Namco";
            flags = Mame.ROT90_16BIT;
            input_ports = driver_namcos1.input_ports_ns1();
            rom = rom_pacmania();
            drv.HasNVRAMhandler = true;
        }
    }
}
