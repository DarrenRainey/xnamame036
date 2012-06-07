using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{    
    class driver_gtmr : Mame.GameDriver
    {
        public static int shogwarr_mcu_status, shogwarr_mcu_command_offset;
        public static _BytePtr mcu_ram, gtmr_mcu_com = new _BytePtr(8);

        public static Mame.tilemap bg_tilemap, fg_tilemap;
        public static Mame.osd_bitmap kaneko16_bg15_bitmap;
        public static int flipsprites;
        public static _BytePtr kaneko16_bgram = new _BytePtr(1);
        public static _BytePtr kaneko16_fgram = new _BytePtr(1);
        public static _BytePtr kaneko16_layers1_regs = new _BytePtr(1);
        public static _BytePtr kaneko16_layers2_regs = new _BytePtr(1);
        public static _BytePtr kaneko16_screen_regs = new _BytePtr(1);
        public static _BytePtr kaneko16_bg15_select = new _BytePtr(1);
        public static _BytePtr kaneko16_bg15_reg = new _BytePtr(1);
        public static int kaneko16_spritetype;

        static int gtmr_wheel_r(int offset)
        {
            if ((Mame.readinputport(4) & 0x1800) == 0x10)	// DSW setting
                return Mame.readinputport(5) << 8;			// 360° Wheel
            else
                return Mame.readinputport(5);				// 270° Wheel
        }
        static int bank0;
        static void gtmr_oki_0_bank_w(int offset, int data)
        {
            okim6295.OKIM6295_set_bank_base(0, okim6295.ALL_VOICES, 0x10000 * (data & 0xF));
            bank0 = (data & 0xF);
        }
        static void gtmr_oki_1_bank_w(int offset, int data)
        {
            okim6295.OKIM6295_set_bank_base(1, okim6295.ALL_VOICES, 0x40000 * (data & 0x1));
        }
        static int pend = 0;
        static void gtmr_oki_0_data_w(int offset, int data)
        {

            if (pend != 0) pend = 0;
            else
            {
                if ((data & 0x80) != 0)
                {
                    int samp = data & 0x7f;

                    pend = 1;
                    if (samp < 0x20)
                    {
                        okim6295.OKIM6295_set_bank_base(0, okim6295.ALL_VOICES, 0);
                        //				if (errorlog) fprintf(errorlog, "Setting OKI0 bank to zero\n");
                    }
                    else
                        okim6295.OKIM6295_set_bank_base(0, okim6295.ALL_VOICES, 0x10000 * bank0);
                }
            }

            okim6295.OKIM6295_data_0_w(offset, data);
        }
        static void gtmr_oki_1_data_w(int offset, int data)
        {
            okim6295.OKIM6295_data_1_w(offset, data);
        }
        public const byte GTMR_INTERRUPTS_NUM = 3;
        public static int gtmr_interrupt()
        {
            switch (Mame.cpu_getiloops())
            {
                case 2: return 3;
                case 1: return 4;
                case 0: return 5;
                default: return 0;
            }
        }
        public static void get_fg_tile_info(int col, int row)
        {
            int tile_index = col + row * FG_NX;
            int code_hi = kaneko16_fgram.read16(tile_index * 4 + 0);
            int code_lo = kaneko16_fgram.read16(tile_index * 4 + 2);
            Mame.SET_TILE_INFO(FG_GFX, code_lo, (code_hi >> 2) & 0x3f);
            Mame.tile_info.flags =(byte) Mame.TILE_FLIPXY(code_hi & 3);
            Mame.tile_info.priority = (byte)((code_hi >> 8) & 3);
        }
        public static void get_bg_tile_info(int col, int row)
        {
            int tile_index = col + row * BG_NX;
            int code_hi = kaneko16_bgram.read16(tile_index * 4 + 0);
            int code_lo = kaneko16_bgram.read16(tile_index * 4 + 2);
            Mame.SET_TILE_INFO(BG_GFX, code_lo, (code_hi >> 2) & 0x3f);
            Mame.tile_info.flags = (byte)Mame.TILE_FLIPXY(code_hi & 3);
        }

        static void gtmr_mcu_run()
        {
            int mcu_command = mcu_ram.read16(0x0010);
            int mcu_offset = mcu_ram.read16(0x0012);
            int mcu_data = mcu_ram.read16(0x0014);

            //if (errorlog) fprintf(errorlog,"CPU #0 PC %06X : MCU executed command: %04X %04X %04X\n",cpu_get_pc(),mcu_command,mcu_offset,mcu_data);

            switch (mcu_command >> 8)
            {

                case 0x02: // Read from NVRAM
                    {
                        object f;
                        if ((f = Mame.osd_fopen(Mame.Machine.gamedrv.name, null, Mame.OSD_FILETYPE_NVRAM, 0)) != null)
                        {
                            Mame.osd_fread(f, new _BytePtr(mcu_ram, mcu_offset), 128);
                            Mame.osd_fclose(f);
                        }
                    }
                    break;

                case 0x42: // Write to NVRAM
                    {
                        object f;
                        if ((f = Mame.osd_fopen(Mame.Machine.gamedrv.name, null, Mame.OSD_FILETYPE_NVRAM, 1)) != null)
                        {
                            Mame.osd_fwrite(f, new _BytePtr(mcu_ram, mcu_offset), 128);
                            Mame.osd_fclose(f);
                        }
                    }
                    break;

                case 0x03: // DSW
                    {
                        mcu_ram.write16(mcu_offset, (ushort)Mame.readinputport(4));
                    }
                    break;

                case 0x04: // TEST (2 versions)
                    {
                        if (Mame.Machine.gamedrv is driver_gtmr)
                        {
                            /* MCU writes the string "MM0525-TOYBOX199" to shared ram */
                            mcu_ram.write16(mcu_offset + 0x00, 0x4d4d);
                            mcu_ram.write16(mcu_offset + 0x02, 0x3035);
                            mcu_ram.write16(mcu_offset + 0x04, 0x3235);
                            mcu_ram.write16(mcu_offset + 0x06, 0x2d54);
                            mcu_ram.write16(mcu_offset + 0x08, 0x4f59);
                            mcu_ram.write16(mcu_offset + 0x0a, 0x424f);
                            mcu_ram.write16(mcu_offset + 0x0c, 0x5831);
                            mcu_ram.write16(mcu_offset + 0x0e, 0x3939);
                        }

                        if (Mame.Machine.gamedrv is driver_gtmre)
                        {
                            /* MCU writes the string "USMM0713-TB1994 " to shared ram */
                            mcu_ram.write16(mcu_offset + 0x00, 0x5553);
                            mcu_ram.write16(mcu_offset + 0x02, 0x4d4d);
                            mcu_ram.write16(mcu_offset + 0x04, 0x3037);
                            mcu_ram.write16(mcu_offset + 0x06, 0x3133);
                            mcu_ram.write16(mcu_offset + 0x08, 0x2d54);
                            mcu_ram.write16(mcu_offset + 0x0a, 0x4231);
                            mcu_ram.write16(mcu_offset + 0x0c, 0x3939);
                            mcu_ram.write16(mcu_offset + 0x0e, 0x3420);
                        }
                    }
                    break;
            }

        }
        static void gtmr_mcu_com0_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(gtmr_mcu_com, 0 * 2, data);
            if (gtmr_mcu_com.read16(0) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(2) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(4) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(6) != 0xFFFF) return;
            Array.Clear(gtmr_mcu_com.buffer, gtmr_mcu_com.offset, 8);
            gtmr_mcu_run();
        }
        static void gtmr_mcu_com1_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(gtmr_mcu_com, 1 * 2, data);
            if (gtmr_mcu_com.read16(0) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(2) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(4) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(6) != 0xFFFF) return;
            Array.Clear(gtmr_mcu_com.buffer, gtmr_mcu_com.offset, 8);
            gtmr_mcu_run();
        }
        static void gtmr_mcu_com2_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(gtmr_mcu_com, 2 * 2, data);
            if (gtmr_mcu_com.read16(0) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(2) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(4) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(6) != 0xFFFF) return;
            Array.Clear(gtmr_mcu_com.buffer, gtmr_mcu_com.offset, 8);
            gtmr_mcu_run();
        }
        static void gtmr_mcu_com3_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(gtmr_mcu_com, 3 * 2, data);
            if (gtmr_mcu_com.read16(0) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(2) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(4) != 0xFFFF) return;
            if (gtmr_mcu_com.read16(6) != 0xFFFF) return;

            Array.Clear(gtmr_mcu_com.buffer, gtmr_mcu_com.offset, 8);
            gtmr_mcu_run();
        }

        static void kaneko16_paletteram_w(int offset, int data)
        {
            /*	byte 0    	byte 1		*/
            /*	xGGG GGRR   RRRB BBBB	*/
            /*	x432 1043 	2104 3210	*/

            int newword, r, g, b;

            Mame.COMBINE_WORD_MEM(Mame.paletteram, offset, data);

            newword = Mame.paletteram.read16(offset);
            r = (newword >> 5) & 0x1f;
            g = (newword >> 10) & 0x1f;
            b = (newword >> 0) & 0x1f;

            Mame.palette_change_color(offset / 2, (byte)((r * 0xFF) / 0x1F),
                                             (byte)((g * 0xFF) / 0x1F),
                                             (byte)((b * 0xFF) / 0x1F));
        }
        public const byte FG_GFX = 0, FG_NX = 0x20, FG_NY = 0x20;
        public const byte BG_GFX = 0, BG_NX = 0x20, BG_NY = 0x20;
        static void kaneko16_fgram_w(int offset, int data)
        {
            int old_data, new_data;

            old_data = kaneko16_fgram.read16(offset);
            Mame.COMBINE_WORD_MEM(kaneko16_fgram, offset, data);
            new_data = kaneko16_fgram.read16(offset);

            if (old_data != new_data)
                Mame.tilemap_mark_tile_dirty(fg_tilemap, (offset / 4) % FG_NX, (offset / 4) / FG_NX);
        }
        static void kaneko16_bgram_w(int offset, int data)
        {
            int old_data, new_data;

            old_data = kaneko16_bgram.read16(offset);
            Mame.COMBINE_WORD_MEM(kaneko16_bgram, offset, data);
            new_data = kaneko16_bgram.read16(offset);

            if (old_data != new_data)
                Mame.tilemap_mark_tile_dirty(bg_tilemap, (offset / 4) % BG_NX, (offset / 4) / BG_NX);
        }
        static void gtmr_paletteram_w(int offset, int data)
        {
            if (offset < 0x10000) kaneko16_paletteram_w(offset, data);
            else Mame.COMBINE_WORD_MEM(Mame.paletteram, offset, data);
        }
        static int kaneko16_screen_regs_r(int offset)
        {
            return kaneko16_screen_regs.read16(offset);
        }
        static void kaneko16_screen_regs_w(int offset, int data)
        {
            int new_data;

            Mame.COMBINE_WORD_MEM(kaneko16_screen_regs, offset, data);
            new_data = kaneko16_screen_regs.read16(offset);

            switch (offset)
            {
                case 0x00: flipsprites = new_data & 3; break;
            }

            //if (errorlog) fprintf(errorlog, "CPU #0 PC %06X : Warning, screen reg %04X <- %04X\n", cpu_get_pc(), offset, data);
        }

        static void kaneko16_layers1_regs_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(kaneko16_layers1_regs, offset, data);
        }
        static void kaneko16_layers1_w(int offset, int data)
        {
            if (offset < 0x1000) kaneko16_fgram_w(offset, data);
            else
            {
                if (offset < 0x2000) kaneko16_bgram_w((offset - 0x1000), data);
                else
                {
                    Mame.COMBINE_WORD_MEM(kaneko16_fgram, offset, data);
                }
            }
        }
        static void kaneko16_layers2_regs_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(kaneko16_layers2_regs, offset, data);
        }
        static int kaneko16_rnd_r(int offset)
        {
            return Mame.rand();
        }
        static void kaneko16_coin_lockout_w(int offset, int data)
        {
            if ((data & 0xff000000) == 0)
            {
                Mame.coin_lockout_w(0, ((~data) >> 11) & 1);
                Mame.coin_lockout_w(1, ((~data) >> 10) & 1);
            }
        }
        public static Mame.MemoryReadAddress[] gtmr_readmem =
{
	new Mame.MemoryReadAddress( 0x000000, 0x0ffffd, Mame.MRA_ROM					),	// ROM
	new Mame.MemoryReadAddress( 0x0ffffe, 0x0fffff, gtmr_wheel_r				),	// Wheel Value
	new Mame.MemoryReadAddress( 0x100000, 0x10ffff, Mame.MRA_BANK1					),	// RAM
	new Mame.MemoryReadAddress( 0x200000, 0x20ffff, Mame.MRA_BANK2					),	// Shared With MCU
	new Mame.MemoryReadAddress( 0x300000, 0x327fff, Mame.MRA_BANK3					),	// Palette (300000-30ffff)
	new Mame.MemoryReadAddress( 0x400000, 0x401fff, Mame.MRA_BANK4					),	// Sprites
	new Mame.MemoryReadAddress( 0x500000, 0x503fff, Mame.MRA_BANK5					),	// Layers 1
	new Mame.MemoryReadAddress( 0x580000, 0x583fff, Mame.MRA_BANK6					),	// Layers 2
	new Mame.MemoryReadAddress( 0x600000, 0x60000f, Mame.MRA_BANK7					),	// Layers 1 Regs
	new Mame.MemoryReadAddress( 0x680000, 0x68000f, Mame.MRA_BANK8					),	// Layers 2 Regs
	new Mame.MemoryReadAddress( 0x700000, 0x70001f, kaneko16_screen_regs_r	),	// Screen Regs ?
	new Mame.MemoryReadAddress( 0x800000, 0x800001, okim6295.OKIM6295_status_0_r		),	// Samples
	new Mame.MemoryReadAddress( 0x880000, 0x880001, okim6295.OKIM6295_status_1_r		),
	new Mame.MemoryReadAddress( 0x900014, 0x900015, kaneko16_rnd_r			),	// Random Number ?
	new Mame.MemoryReadAddress( 0xa00000, 0xa00001,Mame.watchdog_reset_r			),	// Watchdog
	new Mame.MemoryReadAddress( 0xb00000, 0xb00001,Mame.input_port_0_r			),	// Inputs
	new Mame.MemoryReadAddress( 0xb00002, 0xb00003,Mame.input_port_1_r			),
	new Mame.MemoryReadAddress( 0xb00004, 0xb00005,Mame.input_port_2_r			),
	new Mame.MemoryReadAddress( 0xb00006, 0xb00007,Mame.input_port_3_r			),
	new Mame.MemoryReadAddress( 0xd00000, 0xd00001,Mame.MRA_NOP					),	// ? (bit 0)
	new Mame.MemoryReadAddress( -1 )
};

        public static Mame.MemoryWriteAddress[] gtmr_writemem =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x0fffff, Mame.MWA_ROM					),	// ROM
	new Mame.MemoryWriteAddress( 0x100000, 0x10ffff, Mame.MWA_BANK1					),	// RAM
	new Mame.MemoryWriteAddress( 0x200000, 0x20ffff, Mame.MWA_BANK2, mcu_ram		),	// Shared With MCU
	new Mame.MemoryWriteAddress( 0x2a0000, 0x2a0001, gtmr_mcu_com0_w			),	// To MCU ?
	new Mame.MemoryWriteAddress( 0x2b0000, 0x2b0001, gtmr_mcu_com1_w			),
	new Mame.MemoryWriteAddress( 0x2c0000, 0x2c0001, gtmr_mcu_com2_w			),
	new Mame.MemoryWriteAddress( 0x2d0000, 0x2d0001, gtmr_mcu_com3_w			),
	new Mame.MemoryWriteAddress( 0x300000, 0x327fff, gtmr_paletteram_w, Mame.paletteram					),	// Palette
	new Mame.MemoryWriteAddress( 0x400000, 0x401fff, Mame.MWA_BANK4, Generic.spriteram, Generic.spriteram_size			),	// Sprites
	new Mame.MemoryWriteAddress( 0x500000, 0x503fff, kaneko16_layers1_w, kaneko16_fgram				),	// Layers 1
	new Mame.MemoryWriteAddress( 0x580000, 0x583fff, Mame.MWA_BANK6											),	// Layers 2
	new Mame.MemoryWriteAddress( 0x600000, 0x60000f, kaneko16_layers1_regs_w, kaneko16_layers1_regs	),	// Layers 1 Regs
	new Mame.MemoryWriteAddress( 0x680000, 0x68000f, kaneko16_layers2_regs_w, kaneko16_layers2_regs	),	// Layers 2 Regs
	new Mame.MemoryWriteAddress( 0x700000, 0x70001f, kaneko16_screen_regs_w, kaneko16_screen_regs		),	// Screen Regs ?
	new Mame.MemoryWriteAddress( 0x800000, 0x800001, gtmr_oki_0_data_w			),	// Samples
	new Mame.MemoryWriteAddress( 0x880000, 0x880001, gtmr_oki_1_data_w			),
	new Mame.MemoryWriteAddress( 0xa00000, 0xa00001, Mame.watchdog_reset_w			),	// Watchdog
	new Mame.MemoryWriteAddress( 0xb80000, 0xb80001, kaneko16_coin_lockout_w	),	// Coin Lockout
//	new Mame.MemoryWriteAddress( 0xc00000, 0xc00001, MWA_NOP					},	// ?
	new Mame.MemoryWriteAddress( 0xe00000, 0xe00001, gtmr_oki_0_bank_w			),	// Samples Bankswitching
	new Mame.MemoryWriteAddress( 0xe80000, 0xe80001, gtmr_oki_1_bank_w			),
	new Mame.MemoryWriteAddress( -1 )
};
        static Mame.GfxLayout layout_4bit_2M =
            new Mame.GfxLayout(
                16, 16,
    (0x200000) * 8 / (16 * 16 * 4),
    4,
    new uint[] { 0, 1, 2, 3 },
    new uint[]{0*4,1*4,2*4,3*4,4*4,5*4,6*4,7*4, 
	 0*4+32*8,1*4+32*8,2*4+32*8,3*4+32*8,4*4+32*8,5*4+32*8,6*4+32*8,7*4+32*8},
    new uint[]{0*32,1*32,2*32,3*32,4*32,5*32,6*32,7*32,
	 0*32+32*16,1*32+32*16,2*32+32*16,3*32+32*16,4*32+32*16,5*32+32*16,6*32+32*16,7*32+32*16},
    16 * 16 * 4);
        static Mame.GfxLayout layout_8bit_8M =
            new Mame.GfxLayout(
                16, 16,
                0x200000 * 8 / (16 * 16 * 8),
                8,
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[]{0*8,1*8,2*8,3*8,4*8,5*8,6*8,7*8, 
	 0*8+64*8,1*8+64*8,2*8+64*8,3*8+64*8,4*8+64*8,5*8+64*8,6*8+64*8,7*8+64*8},
    new uint[]{0*64,1*64,2*64,3*64,4*64,5*64,6*64,7*64,
	 0*64+64*16,1*64+64*16,2*64+64*16,3*64+64*16,4*64+64*16,5*64+64*16,6*64+64*16,7*64+64*16},
    16 * 16 * 8);
        public static Mame.GfxDecodeInfo[] gtmr_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, layout_4bit_2M,	0,			0x40 ), // [0] Layers
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, layout_8bit_8M,	0x40 * 256,	0x40 ), // [1] Sprites
};

        public static OKIM6295interface gtmr_okim6295_interface =
new OKIM6295interface(
    2,
    new int[] { 12000, 12000 },	/* ? everything seems synced, using 12KHz */
    new int[] { Mame.REGION_SOUND1, Mame.REGION_SOUND2 },
    new int[] { 50, 50 }
);

        class machine_driver_gtmr : Mame.MachineDriver
        {
            public machine_driver_gtmr()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 16000000, gtmr_readmem, gtmr_writemem, null, null, gtmr_interrupt, driver_gtmr.GTMR_INTERRUPTS_NUM));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 320;
                screen_height = 240;
                visible_area = new Mame.rectangle(0, 320 - 1, 0, 240 - 1);
                gfxdecodeinfo = gtmr_gfxdecodeinfo;
                total_colors = 0x10000 / 2;
                color_table_len = 0x10000 / 2;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_UPDATE_AFTER_VBLANK;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_OKIM6295, gtmr_okim6295_interface));
            }
            public override void init_machine()
            {
                kaneko16_bgram = new _BytePtr(kaneko16_fgram, 0x1000);
                kaneko16_spritetype = 1;	// "standard" sprites

                Array.Clear(gtmr_mcu_com.buffer, gtmr_mcu_com.offset, 8);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                bg_tilemap = Mame.tilemap_create(get_bg_tile_info,
                                     Mame.TILEMAP_TRANSPARENT, /* to handle the optional hi-color bg */
                                     16, 16,
                                     BG_NX, BG_NY);

                fg_tilemap = Mame.tilemap_create(driver_gtmr.get_fg_tile_info,
                                            Mame.TILEMAP_TRANSPARENT,
                                            16, 16,
                                            FG_NX, FG_NY);

                if (bg_tilemap != null && fg_tilemap != null)
                {
                    /*
                    gtmr background:
                            flipscreen off: write (x)-$33
                            [x=fetch point (e.g. scroll *left* with incresing x)]

                            flipscreen on:  write (x+320)+$33
                            [x=fetch point (e.g. scroll *right* with incresing x)]

                            W = 320+$33+$33 = $1a6 = 422

                    berlwall background:
                    6940 off	1a5 << 6
                    5680 on		15a << 6
                    */
                    int xdim = Mame.Machine.drv.screen_width;
                    int ydim = Mame.Machine.drv.screen_height;
                    int dx, dy;

                    //		dx   = (422 - xdim) / 2;
                    switch (xdim)
                    {
                        case 320: dx = 0x33; dy = 0; break;
                        case 256: dx = 0x5b; dy = -8; break;

                        default: dx = dy = 0; break;
                    }

                    Mame.tilemap_set_scrolldx(driver_gtmr.bg_tilemap, -dx, xdim + dx - 1);
                    Mame.tilemap_set_scrolldx(driver_gtmr.fg_tilemap, -(dx + 2), xdim + (dx + 2) - 1);

                    Mame.tilemap_set_scrolldy(driver_gtmr.bg_tilemap, -dy, ydim + dy - 1);
                    Mame.tilemap_set_scrolldy(driver_gtmr.fg_tilemap, -dy, ydim + dy - 1);

                    driver_gtmr.bg_tilemap.transparent_pen = 0;
                    driver_gtmr.fg_tilemap.transparent_pen = 0;
                    return 0;
                }
                else
                    return 1;
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new NotImplementedException();
            }
        }

        void kaneko16_unscramble_tiles(int region)
        {
            _BytePtr RAM = Mame.memory_region(region);
            int size = Mame.memory_region_length(region);
            int i;

            for (i = 0; i < size; i++)
            {
                RAM[i] = (byte)(((RAM[i] & 0xF0) >> 4) + ((RAM[i] & 0x0F) << 4));
            }
        }
        public override void driver_init()
        {
            kaneko16_unscramble_tiles(Mame.REGION_GFX1);
        }
        Mame.RomModule[] rom_gtmr()
        {

            ROM_START("gtmr");

            ROM_REGION(0x100000, Mame.REGION_CPU1);	/* 68000 Code */
            ROM_LOAD_EVEN("u2.bin", 0x000000, 0x080000, 0x031799f7);
            ROM_LOAD_ODD("u1.bin", 0x000000, 0x080000, 0x6238790a);

            ROM_REGION(0x010000, Mame.REGION_CPU2);		/* MCU Code */
            ROM_LOAD("mcu_code", 0x000000, 0x010000, 0x00000000);

            ROM_REGION(0x200000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);	/* Tiles (scrambled) */
            ROM_LOAD("gmmu52.bin", 0x000000, 0x200000, 0xb15f6b7f);

            ROM_REGION(0x900000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);	/* Sprites */
            ROM_LOAD("gmmu27.bin", 0x000000, 0x200000, 0xc0ab3efc);
            ROM_LOAD("gmmu28.bin", 0x200000, 0x200000, 0xcf6b23dc);
            ROM_LOAD("gmmu29.bin", 0x400000, 0x200000, 0x8f27f5d3);
            ROM_LOAD("gmmu30.bin", 0x600000, 0x080000, 0xe9747c8c);
            /* codes 6800-7fff are explicitly skipped */
            //	ROM_LOAD_GFX_EVEN( "gmmu64.bin",  0x700000, 0x100000, 0x57d77b33 )	// HALVES IDENTICAL
            //	ROM_LOAD_GFX_ODD(  "gmmu65.bin",  0x700000, 0x100000, 0x05b8bdca )	// HALVES IDENTICAL
            /* wrong tiles: 	gtmr	77e0 ; gtmralt	81c4 81e0 81c4 */
            ROM_LOAD("sprites", 0x700000, 0x100000, 0x00000000);

            ROM_REGION(0x100000, Mame.REGION_SOUND1);	/* Samples */
            ROM_LOAD("gmmu23.bin", 0x000000, 0x100000, 0xb9cbfbee);	// 16 x $10000

            ROM_REGION(0x100000, Mame.REGION_SOUND2);	/* Samples */
            ROM_LOAD("gmmu24.bin", 0x000000, 0x100000, 0x380cdc7c);	//  2 x $40000 - HALVES IDENTICAL

            return ROM_END;
        }
        public static Mame.InputPortTiny[] input_ports_gtmr()
        {
            INPUT_PORTS_START("gtmr");

            PORT_START();	// IN0 - Player 1 - b00000.w
            PORT_BIT(0x0100, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x0200, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x0400, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x0800, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x1000, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);// swapped for consistency:
            PORT_BIT(0x2000, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);// button1 is usually accel.
            PORT_BIT(0x4000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x8000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	// IN1 - Player 2 - b00002.w
            PORT_BIT(0x0100, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x0200, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x0400, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x0800, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x1000, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);// swapped for consistency:
            PORT_BIT(0x2000, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2); // button1 is usually accel.
            PORT_BIT(0x4000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x8000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	// IN2 - Coins - b00004.w
            PORT_BIT(0x0100, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x0200, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x0400, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x0800, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x1000, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE);	// test
            PORT_BIT(0x2000, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x4000, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);	// operator's facility
            PORT_BIT(0x8000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	// IN3 - Seems unused ! - b00006.w
            PORT_BIT(0x0100, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x0200, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x0400, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x0800, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x1000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x2000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x4000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x8000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	// IN4 - DSW from the MCU - 101265.b <- 206000.b
            PORT_SERVICE(0x0100, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x0200, 0x0200, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x0200, DEF_STR("Off"));
            PORT_DIPSETTING(0x0000, DEF_STR("On"));
            PORT_DIPNAME(0x0400, 0x0400, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x0400, DEF_STR("Upright"));
            PORT_DIPSETTING(0x0000, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x1800, 0x1800, "Controls");
            PORT_DIPSETTING(0x1800, "1 Joystick");
            PORT_DIPSETTING(0x0800, "2 Joysticks");
            PORT_DIPSETTING(0x1000, "Wheel (360)");
            PORT_DIPSETTING(0x0000, "Wheel (270)");
            PORT_DIPNAME(0x2000, 0x2000, "Use Brake");
            PORT_DIPSETTING(0x0000, DEF_STR("Off"));
            PORT_DIPSETTING(0x2000, DEF_STR("On"));
            PORT_DIPNAME(0xc000, 0xc000, "National Anthem & Flag");
            PORT_DIPSETTING(0xc000, "Use Memory");
            PORT_DIPSETTING(0x8000, "Anthem Only");
            PORT_DIPSETTING(0x4000, "Flag Only");
            PORT_DIPSETTING(0x0000, "None");

            PORT_START();	// IN5 - Wheel - 100015.b <- ffffe.b
            PORT_ANALOG(0x00ff, 0x0080, (uint)inptports.IPT_AD_STICK_X | IPF_CENTER, 30, 1, 0x00, 0xff);

            return INPUT_PORTS_END;
        }
        public driver_gtmr()
        {
            drv = new machine_driver_gtmr();
            year = "1994";
            name = "gtmr";
            description = "Great 1000 Miles Rally";
            manufacturer = "Kaneko";
            flags = Mame.ROT0_16BIT;
            input_ports = input_ports_gtmr();
            rom = rom_gtmr();
            drv.HasNVRAMhandler = true;
        }
    }
    class driver_gtmre : Mame.GameDriver
    {
        public override void driver_init()
        {
            throw new NotImplementedException();
        }
        Mame.RomModule[] rom_gtmre()
        {
            ROM_START("gtmre");

            ROM_REGION(0x100000, Mame.REGION_CPU1);		/* 68000 Code */
            ROM_LOAD_EVEN("gmmu2.bin", 0x000000, 0x080000, 0x36dc4aa9);
            ROM_LOAD_ODD("gmmu1.bin", 0x000000, 0x080000, 0x8653c144);

            ROM_REGION(0x010000, Mame.REGION_CPU2);		/* MCU Code */
            ROM_LOAD("mcu_code", 0x000000, 0x010000, 0x00000000);

            ROM_REGION(0x200000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);	/* Tiles (scrambled) */
            ROM_LOAD("gmmu52.bin", 0x000000, 0x200000, 0xb15f6b7f);

            ROM_REGION(0x900000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);/* Sprites */
            ROM_LOAD("gmmu27.bin", 0x000000, 0x200000, 0xc0ab3efc);
            ROM_LOAD("gmmu28.bin", 0x200000, 0x200000, 0xcf6b23dc);
            ROM_LOAD("gmmu29.bin", 0x400000, 0x200000, 0x8f27f5d3);
            ROM_LOAD("gmmu30.bin", 0x600000, 0x080000, 0xe9747c8c);
            /* codes 6800-6fff are explicitly skipped */
            ROM_LOAD_GFX_EVEN("gmmu64.bin", 0x700000, 0x100000, 0x57d77b33);	// HALVES IDENTICAL
            ROM_LOAD_GFX_ODD("gmmu65.bin", 0x700000, 0x100000, 0x05b8bdca);// HALVES IDENTICAL

            ROM_REGION(0x100000, Mame.REGION_SOUND1);	/* Samples */
            ROM_LOAD("gmmu23.bin", 0x000000, 0x100000, 0xb9cbfbee);	// 16 x $10000

            ROM_REGION(0x100000, Mame.REGION_SOUND2);/* Samples */
            ROM_LOAD("gmmu24.bin", 0x000000, 0x100000, 0x380cdc7c);	//  2 x $40000 - HALVES IDENTICAL

            return ROM_END;
        }
        public driver_gtmre()
        {
            throw new Exception();
            //drv = new machine_driver_gtmr();
            year = "1994";
            name = "gtmre";
            description = "Great 1000 Miles Rally (Evolution Model)";
            manufacturer = "Kaneko";
            flags = Mame.ROT0_16BIT;
            input_ports = driver_gtmr.input_ports_gtmr();
            rom = rom_gtmre();
            drv.HasNVRAMhandler = true;
        }
    }
}
