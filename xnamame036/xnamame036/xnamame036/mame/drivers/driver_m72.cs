using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_rtype : Mame.GameDriver
    {
        static _BytePtr m72_videoram1 = new _BytePtr(1);
        static _BytePtr m72_videoram2 = new _BytePtr(1);
        static _BytePtr m72_spriteram = new _BytePtr(1);
        static _BytePtr majtitle_rowscrollram = new _BytePtr(1);
        static int rastersplit;
        static int splitline;
        static Mame.tilemap fg_tilemap, bg_tilemap;
        static int xadjust;
        static int[] scrollx1 = new int[256], scrolly1 = new int[256], scrollx2 = new int[256], scrolly2 = new int[256];
        const int VECTOR_INIT = 0, YM2151_ASSERT = 1, YM2151_CLEAR = 2, Z80_ASSERT = 3, Z80_CLEAR = 4;
        static int irqvector;
        static void setvector_callback(int param)
        {

            switch (param)
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

            Mame.cpu_irq_line_vector_w(1, 0, irqvector);
            if (irqvector == 0xff)	/* no IRQs pending */
                Mame.cpu_set_irq_line(1, 0, Mame.CLEAR_LINE);
            else	/* IRQ pending */
                Mame.cpu_set_irq_line(1, 0, Mame.ASSERT_LINE);
        }

        static void m72_ym2151_irq_handler(int irq)
        {
            if (irq != 0)
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, YM2151_ASSERT, setvector_callback);
            else
                Mame.Timer.timer_set(Mame.Timer.TIME_NOW, YM2151_CLEAR, setvector_callback);
        }

        static int m72_interrupt()
        {
            int line = 255 - Mame.cpu_getiloops();

            if (line == 255)	/* vblank */
            {
                rastersplit = 0;
                Mame.interrupt_vector_w(0, irq1);
                return Mame.interrupt();
            }
            else
            {
                if (line != splitline - 128)
                    return Mame.ignore_interrupt();

                rastersplit = line + 1;

                /* this is used to do a raster effect and show the score display at
                   the bottom of the screen or other things. The line where the
                   interrupt happens is programmable (and the interrupt can be triggered
                   multiple times, be changing the interrupt line register in the
                   interrupt handler).
                 */
                Mame.interrupt_vector_w(0, irq2);
                return Mame.interrupt();
            }
        }
        static void m72_irq_line_w(int offset, int data)
        {
            offset *= 8;
            splitline = (splitline & (0xff00 >> offset)) | (data << offset);
        }
        static void m72_scrollx1_w(int offset, int data)
        {
            int i;

            offset *= 8;
            scrollx1[rastersplit] = (scrollx1[rastersplit] & (0xff00 >> offset)) | (data << offset);

            for (i = rastersplit + 1; i < 256; i++)
                scrollx1[i] = scrollx1[rastersplit];
        }
        static void m72_scrollx2_w(int offset, int data)
        {
            int i;

            offset *= 8;
            scrollx2[rastersplit] = (scrollx2[rastersplit] & (0xff00 >> offset)) | (data << offset);

            for (i = rastersplit + 1; i < 256; i++)
                scrollx2[i] = scrollx2[rastersplit];
        }
        static void m72_scrolly1_w(int offset, int data)
        {
            int i;

            offset *= 8;
            scrolly1[rastersplit] = (scrolly1[rastersplit] & (0xff00 >> offset)) | (data << offset);

            for (i = rastersplit + 1; i < 256; i++)
                scrolly1[i] = scrolly1[rastersplit];
        }
        static void m72_scrolly2_w(int offset, int data)
        {
            int i;

            offset *= 8;
            scrolly2[rastersplit] = (scrolly2[rastersplit] & (0xff00 >> offset)) | (data << offset);

            for (i = rastersplit + 1; i < 256; i++)
                scrolly2[i] = scrolly2[rastersplit];
        }
        static void m72_spritectrl_w(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"%04x: write %02x to sprite ctrl+%d\n",cpu_get_pc(),data,offset);
            /* TODO: this is ok for R-Type, but might be wrong for others */
            if (offset == 1)
            {
                //memcpy(m72_spriteram, spriteram, spriteram_size);
                Buffer.BlockCopy(Generic.spriteram.buffer, Generic.spriteram.offset, m72_spriteram.buffer, m72_spriteram.offset, Generic.spriteram_size[0]);
                if ((data & 0x40) != 0)
                    for (int i = 0; i < Generic.spriteram_size[0]; i++)
                        Generic.spriteram[i] = 0;
                //memset(spriteram, 0, spriteram_size);
                /* bit 7 is used by bchopper, nspirit, imgfight, loht, gallop - meaning unknown */
                /* rtype2 uses bits 4,5,6 and 7 - of course it could be a different chip */
            }
        }

        static void m72_port02_w(int offset, int data)
        {
            if (offset != 0)
            {
                return;
            }

            /* bits 0/1 are coin counters */
            Mame.coin_counter_w(0, data & 0x01);
            Mame.coin_counter_w(1, data & 0x02);

            /* bit 3 is used but unknown */

            /* bit 4 resets sound CPU (active low) */
            if ((data & 0x10) != 0)
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
            else
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);

            /* other bits unknown */
        }
        static void m72_init_machine()
        {
            irq1 = 0x20;
            irq2 = 0x22;

            /* R-Type II doesn't clear the scroll registers on reset, so we have to do it ourselves */
            for (int i = 0; i < 256; i++)
                scrollx1[i] = scrolly1[i] = 0;

            M72.m72_init_sound();
        }

        static void m72_get_bg_tile_info(int col, int row)
        {
            int tile_index = 4 * (64 * row + col);
            byte attr = m72_videoram2[tile_index + 1];
            Mame.SET_TILE_INFO(2, m72_videoram2[tile_index] + ((attr & 0x3f) << 8), m72_videoram2[tile_index + 2] & 0x0f);
            Mame.tile_info.flags = (byte)Mame.TILE_FLIPYX((attr & 0xc0) >> 6);
        }

        static void m72_get_fg_tile_info(int col, int row)
        {
            int tile_index = 4 * (64 * row + col);
            byte attr = m72_videoram1[tile_index + 1];
            Mame.SET_TILE_INFO(1, m72_videoram1[tile_index] + ((attr & 0x3f) << 8), m72_videoram1[tile_index + 2] & 0x0f);
            /* bchopper: (videoram[tile_index+2] & 0x10) is used, priority? */
            Mame.tile_info.flags = (byte)Mame.TILE_FLIPYX((attr & 0xc0) >> 6);

            Mame.tile_info.priority = (byte)((m72_videoram1[tile_index + 2] & 0x80) >> 7);
        }

        static int m72_vh_start()
        {
            bg_tilemap = Mame.tilemap_create(m72_get_bg_tile_info, Mame.TILEMAP_OPAQUE, 8, 8, 64, 64);
            fg_tilemap = Mame.tilemap_create(m72_get_fg_tile_info, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 64);

            m72_spriteram = new _BytePtr(Generic.spriteram_size[0]);

            fg_tilemap.transparent_pen = 0;

            //memset(m72_spriteram,0,spriteram_size);

            xadjust = 0;

            /* improves bad gfx in nspirit (but this is not a complete fix, maybe there's a */
            /* layer enalbe register */
            for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                Mame.palette_change_color(i, 0, 0, 0);

            return 0;
        }
        static void m72_vh_stop()
        {
            m72_spriteram = null;
        }
        static void mark_sprite_colors(_BytePtr ram)
        {
            int offs, color, i;
            int[] colmask = new int[32];
            int pal_base;

            pal_base = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;

            for (color = 0; color < 32; color++) colmask[color] = 0;

            for (offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
            {
                color = ram[offs + 4] & 0x0f;
                colmask[color] |= 0xffff;
            }

            for (color = 0; color < 32; color++)
            {
                for (i = 1; i < 16; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color + i] |= Mame.PALETTE_COLOR_VISIBLE;
                }
            }
        }
        static void draw_layer(Mame.osd_bitmap bitmap,
        Mame.tilemap tilemap, int[] scrollx, int[] scrolly, int priority)
        {
            int start, i;
            /* use clip regions to split the screen */
            Mame.rectangle clip = new Mame.rectangle();

            clip.min_x = Mame.Machine.drv.visible_area.min_x;
            clip.max_x = Mame.Machine.drv.visible_area.max_x;
            start = Mame.Machine.drv.visible_area.min_y - 128;
            do
            {
                i = start;
                while (i + 1 < scrollx.Length && scrollx[i + 1] == scrollx[start] && scrolly[i + 1] == scrolly[start]
                        && i < Mame.Machine.drv.visible_area.max_y - 128)
                    i++;

                clip.min_y = start + 128;
                clip.max_y = i + 128;
                Mame.tilemap_set_clip(tilemap, clip);
                Mame.tilemap_set_scrollx(tilemap, 0, scrollx[start] + xadjust);
                Mame.tilemap_set_scrolly(tilemap, 0, scrolly[start]);
                Mame.tilemap_draw(bitmap, tilemap, priority);

                start = i + 1;
            } while (start < Mame.Machine.drv.visible_area.max_y - 128);
        }

        static void draw_bg(Mame.osd_bitmap bitmap, int priority)
        {
            draw_layer(bitmap, bg_tilemap, scrollx2, scrolly2, priority);
        }
        static void draw_fg(Mame.osd_bitmap bitmap, int priority)
        {
            draw_layer(bitmap, fg_tilemap, scrollx1, scrolly1, priority);
        }
        static void m72_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            Mame.tilemap_set_clip(fg_tilemap, null);
            Mame.tilemap_set_clip(bg_tilemap, null);

            Mame.tilemap_update(bg_tilemap);
            Mame.tilemap_update(fg_tilemap);

            Mame.palette_init_used_colors();
            mark_sprite_colors(m72_spriteram);
            if (Mame.palette_recalc() != null)
                Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

            Mame.tilemap_render(Mame.ALL_TILEMAPS);

            draw_bg(bitmap, 0);
            draw_fg(bitmap, 0);
            draw_sprites(bitmap);
            draw_fg(bitmap, 1);
        }
        static void draw_sprites(Mame.osd_bitmap bitmap)
        {
            int offs;

            for (offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
            {
                int code, color, sx, sy, flipx, flipy, w, h, x, y;

                code = m72_spriteram[offs + 2] | (m72_spriteram[offs + 3] << 8);
                color = m72_spriteram[offs + 4] & 0x0f;
                sx = -256 + (m72_spriteram[offs + 6] | ((m72_spriteram[offs + 7] & 0x03) << 8));
                sy = 512 - (m72_spriteram[offs + 0] | ((m72_spriteram[offs + 1] & 0x01) << 8));
                flipx = m72_spriteram[offs + 5] & 0x08;
                flipy = m72_spriteram[offs + 5] & 0x04;

                w = 1 << ((m72_spriteram[offs + 5] & 0xc0) >> 6);
                h = 1 << ((m72_spriteram[offs + 5] & 0x30) >> 4);
                sy -= 16 * h;

                for (x = 0; x < w; x++)
                {
                    for (y = 0; y < h; y++)
                    {
                        int c = code;

                        if (flipx != 0) c += 8 * (w - 1 - x);
                        else c += 8 * x;
                        if (flipy != 0) c += h - 1 - y;
                        else c += y;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)c,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                sx + 16 * x, sy + 16 * y,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
        }
        static int m72_palette1_r(int offset)
        {
            return Mame.paletteram[offset];
        }
        static int m72_palette2_r(int offset)
        {
            return Mame.paletteram_2[offset];
        }
        static int m72_videoram1_r(int offset)
        {
            return m72_videoram1[offset];
        }
        static int m72_videoram2_r(int offset)
        {
            return m72_videoram2[offset];
        }
        static void changecolor(int color, int r, int g, int b)
        {
            r = (r << 3) | (r >> 2);
            g = (g << 3) | (g >> 2);
            b = (b << 3) | (b >> 2);

            Mame.palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        static void m72_palette1_w(int offset, int data)
        {
            Mame.paletteram[offset] = (byte)data;
            if ((offset & 1) != 0) return;
            offset &= 0x3ff;
            changecolor(offset / 2,
                     Mame.paletteram[offset + 0x000],
                     Mame.paletteram[offset + 0x400],
                     Mame.paletteram[offset + 0x800]);
        }
        static void m72_palette2_w(int offset, int data)
        {
            Mame.paletteram_2[offset] = (byte)data;
            if ((offset & 1) != 0) return;
            offset &= 0x3ff;
            changecolor(offset / 2 + 512,
                    Mame.paletteram_2[offset + 0x000],
                    Mame.paletteram_2[offset + 0x400],
                    Mame.paletteram_2[offset + 0x800]);
        }
        static void m72_videoram1_w(int offset, int data)
        {
            if (m72_videoram1[offset] != data)
            {
                m72_videoram1[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(fg_tilemap, (offset / 4) % 64, (offset / 4) / 64);
            }
        }
        static void m72_videoram2_w(int offset, int data)
        {
            if (m72_videoram2[offset] != data)
            {
                m72_videoram2[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, (offset / 4) % 64, (offset / 4) / 64);
            }
        }

        static _BytePtr soundram = new _BytePtr(1);
        static int soundram_r(int offset)
        {
            return soundram[offset];
        }
        static void soundram_w(int offset, int data)
        {
            soundram[offset] = (byte)data;
        }

        static Mame.MemoryReadAddress[] rtype_readmem =				
{																
	new Mame.MemoryReadAddress( 0x00000, 0x40000-1, Mame.MRA_ROM ),							
	new Mame.MemoryReadAddress( 0x40000, 0x40000+0x3fff, Mame.MRA_RAM ),						
	new Mame.MemoryReadAddress( 0xc0000, 0xc03ff, Mame.MRA_RAM ),								
	new Mame.MemoryReadAddress( 0xc8000, 0xc8bff, m72_palette1_r ),						
	new Mame.MemoryReadAddress( 0xcc000, 0xccbff, m72_palette2_r ),						
	new Mame.MemoryReadAddress( 0xd0000, 0xd3fff, m72_videoram1_r ),						
	new Mame.MemoryReadAddress( 0xd8000, 0xdbfff, m72_videoram2_r ),						
	new Mame.MemoryReadAddress( 0xe0000, 0xeffff, soundram_r ),							
	new Mame.MemoryReadAddress( -1 )	/* end of table */									
};
        static Mame.MemoryWriteAddress[] rtype_writemem =			
{																
	new Mame.MemoryWriteAddress( 0x00000, 0x40000-1, Mame.MWA_ROM ),							
	new Mame.MemoryWriteAddress( 0x40000, 0x40000+0x3fff, Mame.MWA_RAM ),	/* work RAM */		
	new Mame.MemoryWriteAddress( 0xc0000, 0xc03ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),	
	new Mame.MemoryWriteAddress( 0xc8000, 0xc8bff, m72_palette1_w, Mame.paletteram ),			
	new Mame.MemoryWriteAddress( 0xcc000, 0xccbff, m72_palette2_w, Mame.paletteram_2 ),		
	new Mame.MemoryWriteAddress( 0xd0000, 0xd3fff, m72_videoram1_w, m72_videoram1 ),		
	new Mame.MemoryWriteAddress( 0xd8000, 0xdbfff, m72_videoram2_w, m72_videoram2 ),		
	new Mame.MemoryWriteAddress( 0xe0000, 0xeffff, soundram_w ),							
	new Mame.MemoryWriteAddress( -1 )	/* end of table */									
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0xffff, Mame.MWA_RAM, soundram ),
	new Mame.MemoryWriteAddress(-1 )	/* end of table */
};

        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x01, 0x01, YM2151.YM2151_status_port_0_r ),
	new Mame.IOReadPort( 0x02, 0x02, Mame.soundlatch_r ),
	new Mame.IOReadPort( 0x84, 0x84, M72.m72_sample_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, YM2151.YM2151_register_port_0_w ),
	new Mame.IOWritePort( 0x01, 0x01, YM2151.YM2151_data_port_0_w ),
	new Mame.IOWritePort( 0x06, 0x06, M72.m72_sound_irq_ack_w ),
	new Mame.IOWritePort( 0x82, 0x82, M72.m72_sample_w ),
	new Mame.IOWritePort( -1 )  /* end of table */
};
        static Mame.GfxLayout tilelayout =
new Mame.GfxLayout(
    8, 8,	/* 8*8 characters */
    Mame.RGN_FRAC(1, 4),	/* NUM characters */
    4,	/* 4 bits per pixel */
    new uint[] { Mame.RGN_FRAC(3, 4), Mame.RGN_FRAC(2, 4), Mame.RGN_FRAC(1, 4), Mame.RGN_FRAC(0, 4) },
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every char takes 8 consecutive bytes */
);

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            Mame.RGN_FRAC(1, 4),	/* NUM characters */
            4,	/* 4 bits per pixel */
            new uint[] { Mame.RGN_FRAC(3, 4), Mame.RGN_FRAC(2, 4), Mame.RGN_FRAC(1, 4), Mame.RGN_FRAC(0, 4) },
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] m72_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, spritelayout,    0, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tilelayout,    512, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, tilelayout,    512, 16 ),
};
        static int irq1, irq2;
        static YM2151interface ym2151_interface =
        new YM2151interface(
            1,			/* 1 chip */
            3579545,	/* ??? */
            new int[] { YM2151.YM3012_VOL(100, Mame.MIXER_PAN_LEFT, 100, Mame.MIXER_PAN_RIGHT) },
            new YM2151irqhandler[] { m72_ym2151_irq_handler },
            new Mame.mem_write_handler[] { null }
        );


        static Mame.IOReadPort[] readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r ),
	new Mame.IOReadPort( 0x01, 0x01, Mame.input_port_1_r ),
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r ),
	new Mame.IOReadPort( 0x03, 0x03, Mame.input_port_3_r ),
	new Mame.IOReadPort( 0x04, 0x04, Mame.input_port_4_r ),
	new Mame.IOReadPort( 0x05, 0x05, Mame.input_port_5_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        static Mame.IOWritePort[] writeport =
{
	new Mame.IOWritePort( 0x00, 0x01, M72.m72_sound_command_w ),
	new Mame.IOWritePort( 0x02, 0x03, m72_port02_w ),	/* coin counters, reset sound cpu, other stuff? */
	new Mame.IOWritePort( 0x04, 0x05, m72_spritectrl_w ),
	new Mame.IOWritePort( 0x06, 0x07, m72_irq_line_w ),
	new Mame.IOWritePort( 0x80, 0x81, m72_scrolly1_w ),
	new Mame.IOWritePort( 0x82, 0x83, m72_scrollx1_w ),
	new Mame.IOWritePort( 0x84, 0x85, m72_scrolly2_w ),
	new Mame.IOWritePort( 0x86, 0x87, m72_scrollx2_w ),
/*	new Mame.IOWritePort( 0xc0, 0xc0      trigger sample, filled by init_ function */
	new Mame.IOWritePort( -1 )  /* end of table */
};

        class machine_driver_rtype : Mame.MachineDriver
        {
            public machine_driver_rtype()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_V30, 16000000, rtype_readmem, rtype_writemem, readport, writeport, m72_interrupt, 256));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3579645, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.ignore_interrupt, 0));
                frames_per_second = 55;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 512;
                screen_height = 512;
                visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 16 * 8, (64 - 16) * 8 - 1);
                gfxdecodeinfo = m72_gfxdecodeinfo;
                total_colors = 1024;
                color_table_len = 1024;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));

            }
            public override void init_machine()
            {
                m72_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {

            }
            public override int vh_start()
            {
                return m72_vh_start();
            }
            public override void vh_stop()
            {
                m72_vh_stop();
            }
            public override void vh_eof_callback()
            {
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                m72_vh_screenrefresh(bitmap, full_refresh);
            }
        }
        public override void driver_init()
        {

        }
        Mame.RomModule[] rom_rtype()
        {

            ROM_START("rtype");
            ROM_REGION(0x100000, Mame.REGION_CPU1);
            ROM_LOAD_V20_EVEN("db_b1.bin", 0x00000, 0x10000, 0xc1865141);
            ROM_LOAD_V20_ODD("db_a1.bin", 0x00000, 0x10000, 0x5ad2bd90);
            ROM_LOAD_V20_EVEN("db_b2.bin", 0x20000, 0x10000, 0xb4f6407e);
            ROM_RELOAD_V20_EVEN(0xe0000, 0x10000);
            ROM_LOAD_V20_ODD("db_a2.bin", 0x20000, 0x10000, 0x6098d86f);
            ROM_RELOAD_V20_ODD(0xe0000, 0x10000);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            /* no ROM, program will be copied by the main CPU */

            ROM_REGION(0x80000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("cpu-00.bin", 0x00000, 0x10000, 0xdad53bc0)	/* sprites */;
            ROM_LOAD("cpu-01.bin", 0x10000, 0x10000, 0xb28d1a60);
            ROM_LOAD("cpu-10.bin", 0x20000, 0x10000, 0xd6a66298);
            ROM_LOAD("cpu-11.bin", 0x30000, 0x10000, 0xbb182f1a);
            ROM_LOAD("cpu-20.bin", 0x40000, 0x10000, 0xfc247c8a);
            ROM_LOAD("cpu-21.bin", 0x50000, 0x10000, 0x5b41f5f3);
            ROM_LOAD("cpu-30.bin", 0x60000, 0x10000, 0xeb02a1cb);
            ROM_LOAD("cpu-31.bin", 0x70000, 0x10000, 0x2bec510a);

            ROM_REGION(0x20000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("cpu-a0.bin", 0x00000, 0x08000, 0x4e212fb0);	/* tiles #1 */
            ROM_LOAD("cpu-a1.bin", 0x08000, 0x08000, 0x8a65bdff);
            ROM_LOAD("cpu-a2.bin", 0x10000, 0x08000, 0x5a4ae5b9);
            ROM_LOAD("cpu-a3.bin", 0x18000, 0x08000, 0x73327606);

            ROM_REGION(0x20000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("cpu-b0.bin", 0x00000, 0x08000, 0xa7b17491);	/* tiles #2 */
            ROM_LOAD("cpu-b1.bin", 0x08000, 0x08000, 0xb9709686);
            ROM_LOAD("cpu-b2.bin", 0x10000, 0x08000, 0x433b229a);
            ROM_LOAD("cpu-b3.bin", 0x18000, 0x08000, 0xad89b072);
            return ROM_END;
        }
        void JOYSTICK_1()
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
        }
        void JOYSTICK_2()
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
        }
        Mame.InputPortTiny[] input_ports_rtype()
        {

            INPUT_PORTS_START("rtype");
            PORT_START();
            JOYSTICK_1();
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);

            PORT_START();
            JOYSTICK_2();
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_SERVICE); /* 0x20 is another test mode */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown)); /* Probably Bonus Life */
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            /* Coin Mode 1, todo Mode 2 */
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR(Coinage));
            PORT_DIPSETTING(0xa0, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0xb0, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xd0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("8C_3C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("5C_3C"));
            PORT_DIPSETTING(0x30, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));

            PORT_START();
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x01, DEF_STR(Off));

            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x22, 0x20, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, "Upright 1 Player");
            PORT_DIPSETTING(0x20, "Upright 2 Players");
            PORT_DIPSETTING(0x22, DEF_STR(Cocktail));
            //	PORT_DIPSETTING(    0x02, "Upright 1 Player" )
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown)); /* Probably Difficulty */
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x10, DEF_STR(Yes));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        public driver_rtype()
        {
            drv = new machine_driver_rtype();
            year = "1987";
            name = "rtype";
            description = "R-Type (Japan)";
            manufacturer = "Irem";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_rtype();
            rom = rom_rtype();
            drv.HasNVRAMhandler = false;
        }
    }
}
