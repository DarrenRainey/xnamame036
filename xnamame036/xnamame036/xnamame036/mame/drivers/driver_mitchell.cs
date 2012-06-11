using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_mitchell : Mame.GameDriver
    {
        public static _BytePtr pang_videoram = new _BytePtr(1);
        public static _BytePtr pang_colorram = new _BytePtr(1);
        public static int[] pang_videoram_size = new int[1];
        public static _BytePtr pang_objram;
        public static int flipscreen;
        public static int video_bank;
        public static Mame.tilemap bg_tilemap;
        public static int nvram_size ;
        public static _BytePtr nvram;
        public static int init_eeprom_count;
        public static YM2413interface ym2413_interface =
new YM2413interface(
    1,	/* 1 chip */
    8000000,	/* 8MHz ??? (hand tuned) */
    new int[] { 50 }	/* Volume */
);

        public static OKIM6295interface okim6295_interface =
        new OKIM6295interface(
            1,			/* 1 chip */
            new int[] { 8000 },	/* 8000Hz ??? */
            new int[] { Mame.REGION_SOUND1 },		/* memory region 2 */
            new int[] { 50 }
        );

        static void pang_video_bank_w(int offset, int data)
        {
            /* Bank handler (sets base pointers for video write) (doesn't apply to mgakuen) */
            video_bank = data;
        }
        static void pang_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            int bankaddress = 0x10000 + (data & 0x0f) * 0x4000;

            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }


        public static eeprom.EEPROM_interface eeprom_interface = new eeprom.EEPROM_interface(
            6,		/* address bits */
    16,		/* data bits */
    "0110",	/*  read command */
    "0101",	/* write command */
    "0111"	/* erase command */
            ); public override void driver_init() { }

        public static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc000, 0xc7ff, pang_paletteram_r ),	/* Banked palette RAM */
	new Mame.MemoryReadAddress( 0xc800, 0xcfff, pang_colorram_r ),	/* Attribute RAM */
	new Mame.MemoryReadAddress( 0xd000, 0xdfff, pang_videoram_r ),	/* Banked char / OBJ RAM */
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_RAM ),	/* Work RAM */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, pang_paletteram_w ),
	new Mame.MemoryWriteAddress( 0xc800, 0xcfff, pang_colorram_w, pang_colorram ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, pang_videoram_w, pang_videoram, pang_videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_RAMROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        public static Mame.IOReadPort[] readport =
{
	new Mame.IOReadPort( 0x00, 0x02, input_r ),	/* Super Pang needs a kludge to initialize EEPROM;
						the Mahjong games and Block Block need special input treatment */
	new Mame.IOReadPort( 0x03, 0x03, Mame.input_port_12_r ),	/* mgakuen only */
	new Mame.IOReadPort( 0x04, 0x04, Mame.input_port_13_r ),	/* mgakuen only */
	new Mame.IOReadPort( 0x05, 0x05, pang_port5_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        public static Mame.IOWritePort[] writeport =
{
	new Mame.IOWritePort(0x00, 0x00, pang_gfxctrl_w ),    /* Palette bank, layer enable, coin counters, more */
	new Mame.IOWritePort(0x01, 0x01, input_w ),
	new Mame.IOWritePort(0x02, 0x02, pang_bankswitch_w ),      /* Code bank register */
	new Mame.IOWritePort(0x03, 0x03, ym2413.YM2413_data_port_0_w ),
	new Mame.IOWritePort(0x04, 0x04, ym2413.YM2413_register_port_0_w ),
	new Mame.IOWritePort(0x05, 0x05, okim6295.OKIM6295_data_0_w ),
	new Mame.IOWritePort(0x06, 0x06, Mame.MWA_NOP_handler ),	/* watchdog? irq ack? */
	new Mame.IOWritePort(0x07, 0x07, pang_video_bank_w ),      /* Video RAM bank register */
	new Mame.IOWritePort(0x08, 0x08, eeprom_cs_w ),
	new Mame.IOWritePort(0x10, 0x10, eeprom_clock_w ),
	new Mame.IOWritePort(0x18, 0x18, eeprom_serial_w ),
	new Mame.IOWritePort(-1 )  /* end of table */
};
        static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
    8, 8,	/* 8*8 characters */
    32768,	/* 32768 characters */
    4,		/* 4 bits per pixel */
    new uint[] { 32768 * 16 * 8 + 4, 32768 * 16 * 8 + 0, 4, 0 },
    new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
    new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
    16 * 8    /* every char takes 16 consecutive bytes */
);
        static Mame.GfxLayout spritelayout =
new Mame.GfxLayout(
    16, 16,  /* 16*16 sprites */
    2048,   /* 2048 sprites */
    4,      /* 4 bits per pixel */
    new uint[] { 2048 * 64 * 8 + 4, 2048 * 64 * 8 + 0, 4, 0 },
    new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			32*8+0, 32*8+1, 32*8+2, 32*8+3, 33*8+0, 33*8+1, 33*8+2, 33*8+3 },
    new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
    64 * 8    /* every sprite takes 64 consecutive bytes */
);
        public static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,     0, 128 ), /* colors 0-2047 */
	new Mame.GfxDecodeInfo(  Mame.REGION_GFX2, 0, spritelayout,   0,  16 ), /* colors 0- 255 */
};
        static int pang_port5_r(int offset)
        {
            int bit;

            bit = eeprom.EEPROM_read_bit() << 7;

            /* bits 0 and (sometimes) 3 are checked in the interrupt handler. */
            /* Maybe they are vblank related, but I'm not sure. */
            /* bit 3 is checked before updating the palette so it really seems to be vblank. */
            /* Many games require two interrupts per frame and for these bits to toggle, */
            /* otherwise music doesn't work. */
            if ((Mame.cpu_getiloops() & 1) != 0) bit |= 0x01;
            else bit |= 0x08;
            if (Mame.Machine.gamedrv is driver_mgakuen2)	/* hack... music doesn't work otherwise */
                bit ^= 0x08;

            return (Mame.input_port_0_r(0) & 0x76) | bit;
        }
        static void mgakuen_videoram_w(int offset, int data)
        {
            if (pang_videoram[offset] != data)
            {
                pang_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, (offset / 2) % 64, (offset / 2) / 64);
            }
        }
        static int mgakuen_videoram_r(int offset)
        {
            return pang_videoram[offset];
        }
        static void mgakuen_objram_w(int offset, int data)
        {
            pang_objram[offset] = (byte)data;
        }
        static int mgakuen_objram_r(int offset)
        {
            return pang_objram[offset];
        }
        static void pang_videoram_w(int offset, int data)
        {
            if (video_bank != 0) mgakuen_objram_w(offset, data);
            else mgakuen_videoram_w(offset, data);
        }
        static int pang_videoram_r(int offset)
        {
            if (video_bank != 0) return mgakuen_objram_r(offset);
            else return mgakuen_videoram_r(offset);
        }
        static void pang_colorram_w(int offset, int data)
        {
            if (pang_colorram[offset] != data)
            {
                pang_colorram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, offset % 64, offset / 64);
            }
        }
        static int paletteram_bank;
        static void pang_paletteram_w(int offset, int data)
        {
            if (paletteram_bank != 0)
                Mame.paletteram_xxxxRRRRGGGGBBBB_w(offset + 0x800, data);
            else
                Mame.paletteram_xxxxRRRRGGGGBBBB_w(offset, data);
        }
        static int pang_paletteram_r(int offset)
        {
            if (paletteram_bank != 0) return Mame.paletteram_r(offset + 0x800);
            return Mame.paletteram_r(offset);
        }
        static int pang_colorram_r(int offset)
        {
            return pang_colorram[offset];
        }
        static void pang_gfxctrl_w(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"PC %04x: pang_gfxctrl_w %02x\n",cpu_get_pc(),data);
            //{
            //    char baf[40];
            //    sprintf(baf,"%02x",data);
            ////	usrintf_showmessage(baf);
            //}

            /* bit 0 is unknown (used, maybe back color enable?) */
            if (data != 0 || offset != 0)
            {
                int a = 0;
            }
            /* bit 1 is coin counter */
            Mame.coin_counter_w(0, data & 2);

            /* bit 2 is flip screen */
            if (flipscreen != (data & 0x04))
            {
                flipscreen = data & 0x04;
                Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
            }

            /* bit 3 is unknown (used, e.g. marukin pulses it on the title screen) */

            /* bit 4 selects OKI M6295 bank */
            okim6295.OKIM6295_set_bank_base(0, okim6295.ALL_VOICES, (data & 0x10) != 0 ? 0x40000 : 0x00000);

            /* bit 5 is palette RAM bank selector (doesn't apply to mgakuen) */
            paletteram_bank = data & 0x20;

            /* bits 6 and 7 are unknown, used in several places. At first I thought */
            /* they were bg and sprites enable, but this screws up spang (screen flickers */
            /* every time you pop a bubble). However, not using them as enable bits screws */
            /* up marukin - you can see partially built up screens during attract mode. */
        }
        static int keymatrix;
        static int[] dial = new int[2];
        static int dial_selected;
        static int[] dir = new int[2];

        static int block_input_r(int offset)
        {

            if (dial_selected != 0)
            {
                int delta;

                delta = (Mame.readinputport(4 + offset) - dial[offset]) & 0xff;
                if ((delta & 0x80) != 0)
                {
                    delta = (-delta) & 0xff;
                    if (dir[offset] != 0)
                    {
                        /* don't report movement on a direction change, otherwise it will stutter */
                        dir[offset] = 0;
                        delta = 0;
                    }
                }
                else if (delta > 0)
                {
                    if (dir[offset] == 0)
                    {
                        /* don't report movement on a direction change, otherwise it will stutter */
                        dir[offset] = 1;
                        delta = 0;
                    }
                }
                if (delta > 0x3f) delta = 0x3f;
                return delta << 2;
            }
            else
            {
                int res;

                res = Mame.readinputport(2 + offset) & 0xf7;
                if ((dir[offset]) != 0) res |= 0x08;

                return res;
            }
        }
        static void block_dial_control_w(int offset, int data)
        {
            if (data == 0x08)
            {
                /* reset the dial counters */
                dial[0] = Mame.readinputport(4);
                dial[1] = Mame.readinputport(5);
            }
            else if (data == 0x80)
                dial_selected = 0;
            else
                dial_selected = 1;
        }
        static int mahjong_input_r(int offset)
        {
            int i;

            for (i = 0; i < 5; i++)
                if ((keymatrix & (0x80 >> i)) != 0) return Mame.readinputport(2 + 5 * offset + i);

            return 0xff;
        }
        static void mahjong_input_select_w(int offset, int data)
        {
            keymatrix = data;
        }
        static int input_r(int offset)
        {
            switch (input_type)
            {
                case 0:
                default: return Mame.readinputport(1 + offset);
                case 1:	/* Mahjong games */
                    if (offset != 0) return mahjong_input_r(offset - 1);
                    else return Mame.readinputport(1);
                case 2:	/* Block Block - dial control */
                    if (offset != 0) return block_input_r(offset - 1);
                    else return Mame.readinputport(1);
                case 3:	/* Super Pang - simulate START 1 press to initialize EEPROM */
                    if (offset != 0 || init_eeprom_count == 0) return Mame.readinputport(1 + offset);
                    else
                    {
                        init_eeprom_count--;
                        return Mame.readinputport(1) & ~0x08;
                    }
            }
        }
        static void input_w(int offset, int data)
        {
            switch (input_type)
            {
                case 0:
                default: Mame.printf("PC %04x: write %02x to port 01\n", Mame.cpu_get_pc(), data); break;
                case 1: mahjong_input_select_w(offset, data); break;
                case 2: block_dial_control_w(offset, data); break;
            }
        }

        public static int input_type;


        static void eeprom_cs_w(int offset, int data)
        {
            eeprom.EEPROM_set_cs_line(data != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }
        static void eeprom_clock_w(int offset, int data)
        {
            eeprom.EEPROM_set_clock_line(data != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }
        static void eeprom_serial_w(int offset, int data)
        {
            eeprom.EEPROM_write_bit(data);
        }
    }
    class machine_driver_pang : Mame.MachineDriver
    {
 
        public machine_driver_pang()
        {
            cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 8000000, driver_mitchell.readmem, driver_mitchell.writemem, driver_mitchell.readport, driver_mitchell.writeport, Mame.interrupt, 2));
            frames_per_second = 60;
            vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
            cpu_slices_per_frame = 1;
            screen_width = 64 * 8;
            screen_height = 32 * 8;
            visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 1 * 8, 31 * 8 - 1);
            gfxdecodeinfo = driver_pang.gfxdecodeinfo;
            total_colors = 2048;
            color_table_len = 2048;
            video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
            sound_attributes = 0;
            sound.Add(new Mame.MachineSound(Mame.SOUND_OKIM6295, driver_pang.okim6295_interface));
            //sound.Add(new Mame.MachineSound(Mame.SOUND_YM2413, driver_pang.ym2413_interface));
        }
        public override void init_machine()
        {
            //none
        }
        public override void nvram_handler(object file, int read_or_write)
        {
            if (read_or_write != 0)
            {
                eeprom.EEPROM_save(file);					/* EEPROM */
                if (driver_mitchell.nvram_size != 0)	/* Super Pang, Block Block */
                    Mame.osd_fwrite(file, driver_mitchell.nvram, driver_mitchell.nvram_size);	/* NVRAM */
            }
            else
            {
                eeprom.EEPROM_init(driver_mitchell.eeprom_interface);

                if (file != null)
                {
                    driver_mitchell.init_eeprom_count = 0;
                    eeprom.EEPROM_load(file);					/* EEPROM */
                    if (driver_mitchell.nvram_size != 0)	/* Super Pang, Block Block */
                        Mame.osd_fread(file, driver_mitchell.nvram, driver_mitchell.nvram_size);	/* NVRAM */
                }
                else
                    driver_mitchell.init_eeprom_count = 1000;	/* for Super Pang */
            }
        }
        public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
        {
            //none
        }
        static void get_bg_tile_info(int col, int row)
        {
            int tile_index = 64 * row + col;
            byte attr = driver_mitchell.pang_colorram[tile_index];
            int code = driver_mitchell.pang_videoram[2 * tile_index] + (driver_mitchell.pang_videoram[2 * tile_index + 1] << 8);
            Mame.SET_TILE_INFO(0, code, attr & 0x7f);
            Mame.tile_info.flags = (byte)((attr & 0x80) != 0 ? Mame.TILE_FLIPX : 0);
        }

        public override int vh_start()
        {
            driver_mitchell.pang_objram = null;
            Mame.paletteram = null;

            driver_mitchell.bg_tilemap = Mame.tilemap_create(
                get_bg_tile_info,
                Mame.TILEMAP_OPAQUE,
                8, 8,
                64, 32
            );

            driver_mitchell.pang_objram = new _BytePtr(driver_mitchell.pang_videoram_size[0]);
            Array.Clear(driver_mitchell.pang_objram.buffer, driver_mitchell.pang_objram.offset, driver_mitchell.pang_videoram_size[0]);

            Mame.paletteram = new _BytePtr(2 * (int)Mame.Machine.drv.total_colors);
            Array.Clear(Mame.paletteram.buffer, Mame.paletteram.offset, (int)(2 * (int)Mame.Machine.drv.total_colors));
            Mame.palette_transparent_color = 0; /* background color (Block Block uses this on the title screen) */

            return 0;
        }
        public override void vh_stop()
        {
            throw new NotImplementedException();
        }
        static void draw_sprites(Mame.osd_bitmap bitmap)
        {
            /* the last entry is not a sprite, we skip it otherwise spang shows a bubble */
            /* moving diagonally across the screen */
            for (int offs = 0x1000 - 0x40; offs >= 0; offs -= 0x20)
            {
                int code = driver_mitchell.pang_objram[offs];
                int attr = driver_mitchell.pang_objram[offs + 1];
                int color = attr & 0x0f;
                int sx = driver_mitchell.pang_objram[offs + 3] + ((attr & 0x10) << 4);
                int sy = ((driver_mitchell.pang_objram[offs + 2] + 8) & 0xff) - 8;
                code += (attr & 0xe0) << 3;
                if (driver_mitchell.flipscreen != 0)
                {
                    sx = 496 - sx;
                    sy = 240 - sy;
                }
                Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                         (uint)code,
                         (uint)color,
                         driver_mitchell.flipscreen != 0, driver_mitchell.flipscreen != 0,
                         sx, sy,
                         Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);
            }
        }
        static void mark_sprites_palette()
        {
            int offs, color, code, attr, i;
            int[] colmask = new int[16];
            int pal_base;


            pal_base = Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start;

            for (color = 0; color < 16; color++) colmask[color] = 0;

            /* the last entry is not a sprite, we skip it otherwise spang shows a bubble */
            /* moving diagonally across the screen */
            for (offs = 0x1000 - 0x40; offs >= 0; offs -= 0x20)
            {
                attr = driver_mitchell.pang_objram[offs + 1];
                code = driver_mitchell.pang_objram[offs] + ((attr & 0xe0) << 3);
                color = attr & 0x0f;

                colmask[color] |= (int)Mame.Machine.gfx[1].pen_usage[code];
            }

            for (color = 0; color < 16; color++)
            {
                for (i = 0; i < 15; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color + i] |= Mame.PALETTE_COLOR_VISIBLE;
                }
            }
        }
        public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            Mame.tilemap_update(Mame.ALL_TILEMAPS);

            Mame.palette_init_used_colors();

            mark_sprites_palette();

            /* the following is required to make the colored background work */
            for (int i = 15; i < Mame.Machine.drv.total_colors; i += 16)
                Mame.palette_used_colors[i] = Mame.PALETTE_COLOR_TRANSPARENT;

            if (Mame.palette_recalc() != null)
                Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

            Mame.tilemap_render(Mame.ALL_TILEMAPS);

            Mame.tilemap_draw(bitmap, driver_mitchell.bg_tilemap, 0);

            draw_sprites(bitmap);
        }
        public override void vh_eof_callback()
        {
            //none
        }
    }
    class driver_mgakuen2 : Mame.GameDriver
    {
        public override void driver_init()
        {
            throw new NotImplementedException();
        }
    }
    class driver_pang : driver_mitchell
    {
        public override void driver_init()
        {
            input_type = 0;
            nvram_size = 0;
            kabuki.pang_decode();
        }
        Mame.InputPortTiny[] input_ports_pang()
        {
            INPUT_PORTS_START("pang");
            PORT_START("DSW");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* USED - handled in port5_r */
            PORT_BITX(0x02, 0x02, (uint)inptports.IPT_SERVICE, DEF_STR("Service Mode"), (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* unused? */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* USED - handled in port5_r */
            PORT_BIT(0x70, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* unused? */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* data from EEPROM */

            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);    /* probably unused */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);   /* probably unused */
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);    /* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_pang()
        {
            ROM_START("pang");
            ROM_REGION(2 * 0x30000, Mame.REGION_CPU1);	/* 192k for code + 192k for decrypted opcodes */
            ROM_LOAD("pang6.bin", 0x00000, 0x08000, 0x68be52cd);
            ROM_LOAD("pang7.bin", 0x10000, 0x20000, 0x4a2e70f6);

            ROM_REGION(0x100000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("pang_09.bin", 0x000000, 0x20000, 0x3a5883f5);	/* chars */
            ROM_LOAD("bb3.bin", 0x020000, 0x20000, 0x79a8ed08);
            /* 40000-7ffff empty */
            ROM_LOAD("pang_11.bin", 0x080000, 0x20000, 0x166a16ae);
            ROM_LOAD("bb5.bin", 0x0a0000, 0x20000, 0x2fb3db6c);
            /* c0000-fffff empty */

            ROM_REGION(0x040000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("bb10.bin", 0x000000, 0x20000, 0xfdba4f6e);	/* sprites */
            ROM_LOAD("bb9.bin", 0x020000, 0x20000, 0x39f47a63);

            ROM_REGION(0x80000, Mame.REGION_SOUND1);	/* OKIM */
            ROM_LOAD("bb1.bin", 0x00000, 0x20000, 0xc52e5b8e);
            return ROM_END;
        }
        public driver_pang()
        {
            drv = new machine_driver_pang();
            year = "1989";
            name = "pang";
            description = "Pang (World)";
            manufacturer = "Mitchel";
            flags = Mame.ROT0;
            input_ports = input_ports_pang();
            rom = rom_pang();
            drv.HasNVRAMhandler = true;
        }
    }
}
