using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_baraduke : Mame.GameDriver
    {
        static int inputport_selected;
        static _BytePtr sharedram = new _BytePtr(1);
        static _BytePtr baraduke_textram = new _BytePtr(1);
        static _BytePtr spriteram = new _BytePtr(1);
        static _BytePtr baraduke_videoram = new _BytePtr(1);
        static Mame.tilemap[] tilemap = new Mame.tilemap[2];
        static int[] xscroll = new int[2], yscroll = new int[2];

        static Mame.MemoryReadAddress[] baraduke_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x17ff, Mame.MRA_RAM ),				/* RAM */
	new Mame.MemoryReadAddress( 0x1800, 0x1fff, Mame.MRA_RAM ),				/* Sprite RAM */
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, baraduke_videoram_r ),	/* Video RAM */
	new Mame.MemoryReadAddress( 0x4000, 0x40ff, Namco.namcos1_wavedata_r ),		/* PSG device, shared RAM */
	new Mame.MemoryReadAddress( 0x4000, 0x43ff, baraduke_sharedram_r ),	/* shared RAM with the MCU */
	new Mame.MemoryReadAddress( 0x4800, 0x4fff, Mame.MRA_RAM ),				/* video RAM (text layer) */
	new Mame.MemoryReadAddress( 0x6000, 0xffff, Mame.MRA_ROM ),				/* ROM */
	new Mame.MemoryReadAddress( -1 )
};

        static Mame.MemoryWriteAddress[] baraduke_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x17ff, Mame.MWA_RAM ),				/* RAM */
	new Mame.MemoryWriteAddress( 0x1800, 0x1fff,Mame.MWA_RAM, spriteram ),	/* Sprite RAM */
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, baraduke_videoram_w, baraduke_videoram ),/* Video RAM */
	new Mame.MemoryWriteAddress( 0x4000, 0x40ff, Namco.namcos1_wavedata_w ),		/* PSG device, shared RAM */
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff, baraduke_sharedram_w,sharedram ),/* shared RAM with the MCU */
	new Mame.MemoryWriteAddress( 0x4800, 0x4fff, Mame.MWA_RAM, baraduke_textram ),	/* video RAM (text layer) */
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, Mame.watchdog_reset_w ),		/* watchdog reset */
	new Mame.MemoryWriteAddress( 0x8800, 0x8800, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0xb000, 0xb002, baraduke_scroll0_w ),		/* scroll (layer 0) */
	new Mame.MemoryWriteAddress( 0xb004, 0xb006, baraduke_scroll1_w ),		/* scroll (layer 1) */
	new Mame.MemoryWriteAddress( 0x6000, 0xffff, Mame.MWA_ROM ),				/* ROM */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.MemoryReadAddress[] mcu_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_r ),/* internal registers */
	new Mame.MemoryReadAddress( 0x0080, 0x00ff, Mame.MRA_RAM ),					/* built in RAM */
	new Mame.MemoryReadAddress( 0x1000, 0x10ff, Namco.namcos1_wavedata_r ),			/* PSG device, shared RAM */
	new Mame.MemoryReadAddress( 0x1105, 0x1105, soundkludge ),	/* cures speech */
	new Mame.MemoryReadAddress( 0x1100, 0x113f, Mame.MRA_RAM ),					/* PSG device */
	new Mame.MemoryReadAddress( 0x1000, 0x13ff, baraduke_sharedram_r ),		/* shared RAM with the 6809 */
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_ROM ),					/* MCU external ROM */
	new Mame.MemoryReadAddress( 0xc000, 0xc800, Mame.MRA_RAM ),					/* RAM */
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),					/* MCU internal ROM */
	new Mame.MemoryReadAddress( -1 )
};

        static Mame.MemoryWriteAddress[] mcu_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_w ),/* internal registers */
	new Mame.MemoryWriteAddress( 0x0080, 0x00ff, Mame.MWA_RAM )	,			/* built in RAM */
	new Mame.MemoryWriteAddress( 0x1000, 0x10ff, Namco.namcos1_wavedata_w, Namco.namco_wavedata ),/* PSG device, shared RAM */
	new Mame.MemoryWriteAddress( 0x1100, 0x113f, Namco.namcos1_sound_w, Namco.namco_soundregs ),/* PSG device */
	new Mame.MemoryWriteAddress( 0x1000, 0x13ff, baraduke_sharedram_w ),	/* shared RAM with the 6809 */
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x8800, 0x8800, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x8000, 0xbfff, Mame.MWA_ROM ),				/* MCU external ROM */
	new Mame.MemoryWriteAddress( 0xc000, 0xc800, Mame.MWA_RAM ),				/* RAM */
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_ROM ),				/* MCU internal ROM */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.IOReadPort[] mcu_readport =
{
	new Mame.IOReadPort( Mame.cpu_hd63701.HD63701_PORT1, Mame.cpu_hd63701.HD63701_PORT1, inputport_r),			/* input ports read */
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] mcu_writeport =
{
	new Mame.IOWritePort( Mame.cpu_hd63701.HD63701_PORT1, Mame.cpu_hd63701.HD63701_PORT1, inputport_select_w ),	/* input port select */
	new Mame.IOWritePort( Mame.cpu_hd63701.HD63701_PORT2, Mame.cpu_hd63701.HD63701_PORT2, baraduke_lamps_w ),		/* lamps */
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.GfxLayout text_layout =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            2,			/* 2 bits per pixel */
            new uint[] { 0, 4 },	/* the bitplanes are packed in the same byte */
            new uint[] { 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout tile_layout1 =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            3,			/* 3 bits per pixel */
            new uint[] { 0x8000 * 8, 0, 4 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 8, 2 * 8, 4 * 8, 6 * 8, 8 * 8, 10 * 8, 12 * 8, 14 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout tile_layout2 =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            3,			/* 3 bits per pixel */
            new uint[] { 0x8000 * 8 + 4, 0x2000 * 8, 0x2000 * 8 + 4 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 8, 2 * 8, 4 * 8, 6 * 8, 8 * 8, 10 * 8, 12 * 8, 14 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout tile_layout3 =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            3,			/* 3 bits per pixel */
            new uint[] { 0xa000 * 8, 0x4000 * 8, 0x4000 * 8 + 4 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 8, 2 * 8, 4 * 8, 6 * 8, 8 * 8, 10 * 8, 12 * 8, 14 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout tile_layout4 =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            3,			/* 3 bits per pixel */
            new uint[] { 0xa000 * 8 + 4, 0x6000 * 8, 0x6000 * 8 + 4 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 8, 2 * 8, 4 * 8, 6 * 8, 8 * 8, 10 * 8, 12 * 8, 14 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,		/* 16*16 sprites */
            512,		/* 512 sprites */
            4,			/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
		8*4, 9*4, 10*4, 11*4, 12*4, 13*4, 14*4, 15*4 },
            new uint[]{ 8*8*0, 8*8*1, 8*8*2, 8*8*3, 8*8*4, 8*8*5, 8*8*6, 8*8*7,
	8*8*8, 8*8*9, 8*8*10, 8*8*11, 8*8*12, 8*8*13, 8*8*14, 8*8*15 },
            128 * 8		/* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, text_layout,	0, 512),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tile_layout1,	0, 256),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tile_layout2,	0, 256),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tile_layout3,	0, 256),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tile_layout4,	0, 256),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout,	0, 128),
};

        static Namco_interface namco_interface =
        new Namco_interface(
            49152000 / 2048, 		/* 24000Hz */
            8,					/* number of voices */
            100,				/* playback volume */
            -1,					/* memory region */
            false					/* stereo */
        );

        static void inputport_select_w(int offset, int data)
        {
            if ((data & 0xf0) == 0x60)
                inputport_selected = data & 0x07;
        }
        static int reverse_bitstrm(int data)
        {
            return ((data & 0x01) << 4) | ((data & 0x02) << 2) | (data & 0x04)
                            | ((data & 0x08) >> 2) | ((data & 0x10) >> 4);
        }
        static int inputport_r(int offset)
        {
            int data = 0;

            switch (inputport_selected)
            {
                case 0x00:	/* DSW A (bits 0-4) */
                    data = ~(reverse_bitstrm(Mame.readinputport(0) & 0x1f)); break;
                case 0x01:	/* DSW A (bits 5-7), DSW B (bits 0-1) */
                    data = ~(reverse_bitstrm((((Mame.readinputport(0) & 0xe0) >> 5) | ((Mame.readinputport(1) & 0x03) << 3)))); break;
                case 0x02:	/* DSW B (bits 2-6) */
                    data = ~(reverse_bitstrm(((Mame.readinputport(1) & 0x7c) >> 2))); break;
                case 0x03:	/* DSW B (bit 7), DSW C (bits 0-3) */
                    data = ~(reverse_bitstrm((((Mame.readinputport(1) & 0x80) >> 7) | ((Mame.readinputport(2) & 0x0f) << 1)))); break;
                case 0x04:	/* coins, start */
                    data = ~(Mame.readinputport(3)); break;
                case 0x05:	/* 2P controls */
                    data = ~(Mame.readinputport(5)); break;
                case 0x06:	/* 1P controls */
                    data = ~(Mame.readinputport(4)); break;
                default:
                    data = 0xff;
                    break;
            }

            return data;
        }
        static void baraduke_lamps_w(int offset, int data)
        {
            Mame.osd_led_w(0, (data & 0x08) >> 3);
            Mame.osd_led_w(1, (data & 0x10) >> 4);
        }
        static int counter;
        static int soundkludge(int offset)
        {
            return ((counter++) >> 4) & 0xff;
        }
        static int baraduke_sharedram_r(int offset)
        {
            return sharedram[offset];
        }
        static void baraduke_sharedram_w(int offset, int val)
        {
            sharedram[offset] = (byte)val;
        }
        static int baraduke_videoram_r(int offset)
        {
            return baraduke_videoram[offset];
        }

        static void baraduke_videoram_w(int offset, int data)
        {
            if (baraduke_videoram[offset] != data)
            {
                baraduke_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(tilemap[offset / 0x1000], (offset / 2) % 64, ((offset % 0x1000) / 2) / 64);
            }
        }
        static void scroll_w(int layer, int offset, int data)
        {
            int[] xdisp = { 26, 24 };

            switch (offset)
            {

                case 0:	/* high scroll x */
                    xscroll[layer] = (xscroll[layer] & 0xff) | (data << 8);
                    break;
                case 1:	/* low scroll x */
                    xscroll[layer] = (xscroll[layer] & 0xff00) | data;
                    break;
                case 2:	/* scroll y */
                    yscroll[layer] = data;
                    break;
            }

            Mame.tilemap_set_scrollx(tilemap[layer], 0, xscroll[layer] + xdisp[layer]);
            Mame.tilemap_set_scrolly(tilemap[layer], 0, yscroll[layer] + 25);
        }

        static void baraduke_scroll0_w(int offset, int data)
        {
            scroll_w(0, offset, data);
        }
        static void baraduke_scroll1_w(int offset, int data)
        {
            scroll_w(1, offset, data);
        }



        class machine_driver_baraduke : Mame.MachineDriver
        {
            public machine_driver_baraduke()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, baraduke_readmem, baraduke_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD63701, 49152000 / 32, mcu_readmem, mcu_writemem, mcu_readport, mcu_writeport, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_baraduke.gfxdecodeinfo;
                total_colors = 2048;
                color_table_len = 2048 * 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                int pi = 0, cpi = 0;
                for (int i = 0; i < 2048; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi + 048] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi + 048] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi + 048] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi + 048] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    /* green component */
                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    /* blue component */
                    bit0 = (color_prom[cpi] >> 4) & 0x01;
                    bit1 = (color_prom[cpi] >> 5) & 0x01;
                    bit2 = (color_prom[cpi] >> 6) & 0x01;
                    bit3 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }
            }

            static void get_tile_info0(int col, int row)
            {
                int tile_index = 2 * (64 * row + col);
                byte attr = baraduke_videoram[tile_index + 1];
                byte code = baraduke_videoram[tile_index];

                Mame.SET_TILE_INFO(1 + ((attr & 0x02) >> 1), code | ((attr & 0x01) << 8), attr);
            }
            static void get_tile_info1(int col, int row)
            {
                int tile_index = 2 * (64 * row + col);
                byte attr = baraduke_videoram[0x1000 + tile_index + 1];
                byte code = baraduke_videoram[0x1000 + tile_index];

                Mame.SET_TILE_INFO(3 + ((attr & 0x02) >> 1), code | ((attr & 0x01) << 8), attr);
            }
            public override int vh_start()
            {
                tilemap[0] = Mame.tilemap_create(get_tile_info0, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 32);
                tilemap[1] = Mame.tilemap_create(get_tile_info1, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 32);

                if (tilemap[0] != null && tilemap[1] != null)
                {
                    tilemap[0].transparent_pen = 7;
                    tilemap[1].transparent_pen = 7;

                    return 0;
                }
                return 1;
            }
            public override void vh_stop()
            {
                //nothing
            }
            static void draw_sprites(Mame.osd_bitmap bitmap, int priority)
            {
                Mame.rectangle clip = Mame.Machine.drv.visible_area;

                _BytePtr source = new _BytePtr(spriteram);
                _BytePtr finish = new _BytePtr(spriteram, 0x0800 - 16);/* the last is NOT a sprite */

                int sprite_xoffs = spriteram[0x07f5] - 256 * (spriteram[0x07f4] & 1) + 16;
                int sprite_yoffs = spriteram[0x07f7] - 256 * (spriteram[0x07f6] & 1);

                while (source.offset < finish.offset)
                {
                    /*
                        source[4]	S-FT ---P
                        source[5]	TTTT TTTT
                        source[6]   CCCC CCCX
                        source[7]	XXXX XXXX
                        source[8]	---T -S-F
                        source[9]   YYYY YYYY
                    */
                    {
                        byte attrs = source[4];
                        byte attr2 = source[8];
                        byte color = source[6];
                        int sx = source[7] + (color & 0x01) * 256; /* need adjust for left clip */
                        int sy = -source[9];
                        bool flipx = (attrs & 0x20) != 0;
                        bool flipy = (attr2 & 0x01) != 0;
                        byte tall =(byte)(attr2 & 0x04);
                        byte wide =(byte)(attrs & 0x80);
                        int pri = attrs & 0x01;
                        int sprite_number = (source[5] & 0xff) * 4;
                        int row, col;

                        if (pri == priority)
                        {
                            if ((attrs & 0x10) != 0 && wide==0) sprite_number += 1;
                            if ((attr2 & 0x10) != 0 && tall==0) sprite_number += 2;
                            color = (byte)(color >> 1);

                            if (sx > 512 - 32) sx -= 512;

                            if (flipx && wide==0) sx -= 16;
                            if (tall==0) sy += 16;
                            if (tall==0 && (attr2 & 0x10) != 0 && flipy) sy -= 16;

                            sx += sprite_xoffs;
                            sy -= sprite_yoffs;

                            for (row = 0; row <= tall; row++)
                            {
                                for (col = 0; col <= wide; col++)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[5],
                                            (uint)(sprite_number + 2 * row + col),
                                            color,
                                            flipx, flipy,
                                            -87 + (sx + 16 * (flipx ? 1 - col : col)),
                                            209 + (sy + 16 * (flipy ? 1 - row : row)),
                                            clip,
                                            Mame.TRANSPARENCY_PEN, 0x0f);
                                }
                            }
                        }
                    }
                    source.offset += 16;
                }
            }

            static void mark_textlayer_colors()
            {
                ushort[] palette_map = new ushort[512];
                Array.Clear(palette_map, 0, 512);
                //memset (palette_map, 0, sizeof (palette_map));

                for (int offs = 0; offs < 0x400; offs++)
                {
                    palette_map[(baraduke_textram[offs + 0x400] << 2) & 0x1ff] |= 0xffff;
                }

                /* now build the final table */
                for (int i = 0; i < 512; i++)
                {
                    int usage = palette_map[i];
                    if (usage != 0)
                    {
                        for (int j = 0; j < 4; j++)
                            if ((usage & (1 << j)) != 0)
                                Mame.palette_used_colors[i * 4 + j] |= Mame.PALETTE_COLOR_VISIBLE;
                    }
                }
            }

            static void mark_sprites_colors()
            {
                _BytePtr source = new _BytePtr(spriteram);
                _BytePtr finish = new _BytePtr(spriteram, 0x0800 - 16);/* the last is NOT a sprite */

                ushort[] palette_map = new ushort[128];
                Array.Clear(palette_map, 0, 128);
                //memset (palette_map, 0, sizeof (palette_map));

                while (source.offset < finish.offset)
                {
                    palette_map[source[6] >> 1] |= 0xffff;
                    source.offset += 16;
                }

                /* now build the final table */
                for (int i = 0; i < 128; i++)
                {
                    int usage = palette_map[i];
                    if (usage != 0)
                    {
                        for (int j = 0; j < 16; j++)
                            if ((usage & (1 << j)) != 0)
                                Mame.palette_used_colors[i * 16 + j] |= Mame.PALETTE_COLOR_VISIBLE;
                    }
                }
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.tilemap_update(Mame.ALL_TILEMAPS);

                Mame.palette_init_used_colors();
                mark_textlayer_colors();
                mark_sprites_colors();
                if (Mame.palette_recalc() != null)
                    Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);

                Mame.tilemap_draw(bitmap, tilemap[1], Mame.TILEMAP_IGNORE_TRANSPARENCY);
                draw_sprites(bitmap, 0);
                Mame.tilemap_draw(bitmap, tilemap[0], 0);
                draw_sprites(bitmap, 1);

                for (int offs = 0x400 - 1; offs > 0; offs--)
                {
                    int mx, my, sx, sy;

                    mx = offs % 32;
                    my = offs / 32;

                    if (my < 2)
                    {
                        if (mx < 2 || mx >= 30) continue; /* not visible */
                        sx = my + 34; sy = mx - 2;
                    }
                    else if (my >= 30)
                    {
                        if (mx < 2 || mx >= 30) continue; /* not visible */
                        sx = my - 30; sy = mx - 2;
                    }
                    else
                    {
                        sx = mx + 2; sy = my - 2;
                    }
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0], baraduke_textram[offs],
                            (uint)(baraduke_textram[offs + 0x400] << 2) & 0x1ff,
                            false, false, sx * 8, sy * 8,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 3);
                }
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_baraduke()
        {

            ROM_START("baraduke");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 6809 code */
            ROM_LOAD("prg1.9c", 0x6000, 0x02000, 0xea2ea790);
            ROM_LOAD("prg2.9a", 0x8000, 0x04000, 0x9a0a9a87);
            ROM_LOAD("prg3.9b", 0xc000, 0x04000, 0x383e5458);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* MCU code */
            ROM_LOAD("prg4.3b", 0x8000, 0x4000, 0xabda0fe7);	/* subprogram for the MCU */
            ROM_LOAD("pl1-mcu.bin", 0xf000, 0x1000, 0x6ef08fb3);	/* The MCU internal code is missing */
            /* Using Pacland code (probably similar) */
            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ch1.3j", 0x00000, 0x2000, 0x706b7fee);	/* characters */

            ROM_REGION(0x0c000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ch2.4p", 0x00000, 0x4000, 0xb0bb0710);	/* tiles */
            ROM_LOAD("ch3.4n", 0x04000, 0x4000, 0x0d7ebec9);
            ROM_LOAD("ch4.4m", 0x08000, 0x4000, 0xe5da0896);

            ROM_REGION(0x10000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("obj1.8k", 0x00000, 0x4000, 0x87a29acc);	/* sprites */
            ROM_LOAD("obj2.8l", 0x04000, 0x4000, 0x72b6d20c);
            ROM_LOAD("obj3.8m", 0x08000, 0x4000, 0x3076af9c);
            ROM_LOAD("obj4.8n", 0x0c000, 0x4000, 0x8b4c09a3);

            ROM_REGION(0x1000, Mame.REGION_PROMS);
            ROM_LOAD("prmcolbg.1n", 0x0000, 0x0800, 0x0d78ebc6);	/* Blue + Green palette */
            ROM_LOAD("prmcolr.2m", 0x0800, 0x0800, 0x03f7241f);	/* Red palette */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_baraduke()
        {
            INPUT_PORTS_START("baraduke");
            PORT_START();	/* DSW A */
            PORT_SERVICE(0x01, IP_ACTIVE_HIGH);
            PORT_DIPNAME(0x06, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x04, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x06, "5");
            PORT_DIPNAME(0x18, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x18, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));

            PORT_START();	/* DSW B */
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x02, "Every 10k");
            PORT_DIPSETTING(0x00, "10k and every 20k");
            PORT_DIPSETTING(0x01, "Every 20k");
            PORT_DIPSETTING(0x03, "None");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x08, "Easy");
            PORT_DIPSETTING(0x00, "Normal");
            PORT_DIPSETTING(0x04, "Hard");
            PORT_DIPSETTING(0x0c, "Very hard");
            PORT_BITX(0x10, 0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack test", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, "Freeze");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, "Allow continue from last level");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x40, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START();	/* DSW C */
            PORT_DIPNAME(0x01, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x01, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x02, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE); /* Another service dip */
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();/* IN 0 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN 1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN 2 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        public driver_baraduke()
        {
            drv = new machine_driver_baraduke();
            year = "1985";
            name = "baraduke";
            description = "baraduke";
            manufacturer = "Namco";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_baraduke();
            rom = rom_baraduke();
            drv.HasNVRAMhandler = false;
        }
    }
}
