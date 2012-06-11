using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class williams
    {
        public static _BytePtr williams_bank_base = new _BytePtr(1);
        public static _BytePtr defender_bank_base = new _BytePtr(1);

        /* RAM globals */
        public static _BytePtr williams_videoram = new _BytePtr(1);
        public static _BytePtr williams2_paletteram = new _BytePtr(1);

        /* blitter variables */
        public static _BytePtr williams_blitterram = new _BytePtr(1);
        public static byte williams_blitter_xor;
        public static byte williams_blitter_remap;
        public static byte williams_blitter_clip;
        public static ushort sinistar_clip;
        public static byte williams_cocktail;

        /* Blaster extra variables */
        public static _BytePtr blaster_video_bits = new _BytePtr(1);
        public static _BytePtr blaster_color_zero_table = new _BytePtr(1);
        public static _BytePtr blaster_color_zero_flags = new _BytePtr(1);
        public static _BytePtr blaster_remap = new _BytePtr(1);
        public static _BytePtr blaster_remap_lookup = new _BytePtr(1);
        public static byte blaster_erase_screen;
        public static ushort blaster_back_color;

        /* tilemap variables */
        public byte williams2_tilemap_mask;
        public static _BytePtr williams2_row_to_palette = new _BytePtr(1); /* take care of IC79 and J1/J2 */
        public byte williams2_M7_flip;
        public sbyte williams2_videoshift;
        public byte williams2_special_bg_color;
        public static byte williams2_fg_color; /* IC90 */
        public static byte williams2_bg_color; /* IC89 */

        /* later-Williams video control variables */
        public static _BytePtr williams2_blit_inhibit = new _BytePtr(1);
        public static _BytePtr williams2_xscroll_low = new _BytePtr(1);
        public static _BytePtr williams2_xscroll_high = new _BytePtr(1);

       public static uint[] defender_bank_list;

        public static _6821pia.pia6821_interface williams_pia_1_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ Mame.input_port_2_r, null, null, null, null, null,
            /*outputs: A/B,CA/B2       */ null, williams_snd_cmd_w, null, null,
            /*irqs   : A/B             */ williams_main_irq, williams_main_irq
);

        /* Generic PIA 2, maps to DAC data in and sound IRQs */
        public static _6821pia.pia6821_interface williams_snd_pia_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ null, null, null, null, null, null,
            /*outputs: A/B,CA/B2       */ DAC.DAC_data_w, null, null, null,
            /*irqs   : A/B             */ williams_snd_irq, williams_snd_irq
);
        static byte williams2_bank;

        public static void williams_init_machine()
        {
            /* reset the PIAs */
            _6821pia.pia_reset();

            /* set a timer to go off every 16 scanlines, to toggle the VA11 line and update the screen */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams_va11_callback);

            /* also set a timer to go off on scanline 240 */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(240), 0, williams_count240_callback);
        }
        static void williams_va11_callback(int scanline)
        {
            /* the IRQ signal comes into CB1, and is set to VA11 */
            _6821pia.pia_1_cb1_w(0, scanline & 0x20);

            /* update the screen while we're here */
            williams_vh_update(scanline);

            /* set a timer for the next update */
            scanline += 16;
            if (scanline >= 256) scanline = 0;
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(scanline), scanline, williams_va11_callback);
        }
        static void williams_count240_callback(int param)
        {
            /* the COUNT240 signal comes into CA1, and is set to the logical AND of VA10-VA13 */
            _6821pia.pia_1_ca1_w(0, 1);

            /* set a timer to turn it off once the scanline counter resets */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams_count240_off_callback);

            /* set a timer for next frame */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(240), 0, williams_count240_callback);
        }
        static void williams_count240_off_callback(int param)
        {
            /* the COUNT240 signal comes into CA1, and is set to the logical AND of VA10-VA13 */
            _6821pia.pia_1_ca1_w(0, 0);
        }



        public static void williams2_init_machine()
        {
            /* reset the PIAs */
            _6821pia.pia_reset();

            /* make sure our banking is reset */
            williams2_bank_select(0, 0);

            /* set a timer to go off every 16 scanlines, to toggle the VA11 line and update the screen */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams2_va11_callback);

            /* also set a timer to go off on scanline 254 */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(254), 0, williams2_endscreen_callback);
        }
        static void williams2_bank_select(int offset, int data)
        {
            uint[] bank = { 0, 0x10000, 0x20000, 0x10000, 0, 0x30000, 0x40000, 0x30000 };

            /* select bank index (only lower 3 bits used by IC56) */
            williams2_bank = (byte)(data & 0x07);

            /* bank 0 references videoram */
            if (williams2_bank == 0)
            {
                Mame.cpu_setbank(1, williams_videoram);
                Mame.cpu_setbank(2, new _BytePtr(williams_videoram, 0x8000));
            }

            /* other banks reference ROM plus either palette RAM or the top of videoram */
            else
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

                Mame.cpu_setbank(1, new _BytePtr(RAM, (int)bank[williams2_bank]));

                if ((williams2_bank & 0x03) == 0x03)
                {
                    Mame.cpu_setbank(2, williams2_paletteram);
                }
                else
                {
                    Mame.cpu_setbank(2, new _BytePtr(williams_videoram, 0x8000));
                }
            }

            /* regardless, the top 2k references videoram */
            Mame.cpu_setbank(3, new _BytePtr(williams_videoram, 0x8800));
        }


        static void williams_main_irq(int state)
        {
            /* IRQ to the main CPU */
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, state != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        static void williams_deferred_snd_cmd_w(int param)
        {
            _6821pia.pia_2_portb_w(0, param);
            _6821pia.pia_2_cb1_w(0, (param == 0xff) ? 0 : 1);
        }

        static void williams_snd_irq(int state)
        {
            /* IRQ to the sound CPU */
            Mame.cpu_set_irq_line(1, Mame.cpu_m6800.M6800_IRQ_LINE, state != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }

        static void williams_snd_cmd_w(int offset, int cmd)
        {
            /* the high two bits are set externally, and should be 1 */
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, cmd | 0xc0, williams_deferred_snd_cmd_w);
        }


        public static void defender_bank_select_w(int offset, int data)
        {
            uint bank_offset = defender_bank_list[data & 7];

            /* set bank address */
            Mame.cpu_setbank(2, new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), (int)bank_offset));

            /* if the bank maps into normal RAM, it represents I/O space */
            if (bank_offset < 0x10000)
            {
                Mame.cpu_setbankhandler_r(2, defender_io_r);
                Mame.cpu_setbankhandler_w(2, defender_io_w);
            }

            /* otherwise, it's ROM space */
            else
            {
                Mame.cpu_setbankhandler_r(2, Mame.MRA_BANK2_handler);
                Mame.cpu_setbankhandler_w(2, Mame.MWA_ROM_handler);
            }
        }
        static void defender_io_w(int offset, int data)
        {
            /* write the data through */
            defender_bank_base[offset] = (byte)data;

            /* watchdog */
            if (offset == 0x03fc)
                Mame.watchdog_reset_w(offset, data);

            /* palette */
            else if (offset < 0x10)
                Mame.paletteram_BBGGGRRR_w(offset, data);

            /* PIAs */
            else if (offset >= 0x0c00 && offset < 0x0c04)
                _6821pia.pia_1_w(offset & 3, data);
            else if (offset >= 0x0c04 && offset < 0x0c08)
                _6821pia.pia_0_w(offset & 3, data);
        }

        static int defender_io_r(int offset)
        {
            /* PIAs */
            if (offset >= 0x0c00 && offset < 0x0c04)
                return _6821pia.pia_1_r(offset & 3);
            else if (offset >= 0x0c04 && offset < 0x0c08)
                return _6821pia.pia_0_r(offset & 3);

            /* video counter */
            else if (offset == 0x800)
                return williams_video_counter_r(offset);

            /* If not bank 0 then return banked RAM */
            return defender_bank_base[offset];
        }

        public static int defender_input_port_0_r(int offset)
        {
            int keys, altkeys;

            /* read the standard keys and the cheat keys */
            keys = Mame.readinputport(0);
            altkeys = Mame.readinputport(3);

            /* modify the standard keys with the cheat keys */
            if (altkeys != 0)
            {
                keys |= altkeys;
                if (Mame.memory_region(Mame.REGION_CPU1)[0xa0bb] == 0xfd)
                {
                    if ((keys & 0x02) != 0)
                        keys = (keys & 0xfd) | 0x40;
                    else if ((keys & 0x40) != 0)
                        keys = (keys & 0xbf) | 0x02;
                }
            }

            return keys;
        }
        static void williams2_va11_callback(int scanline)
        {
            /* the IRQ signal comes into CB1, and is set to VA11 */
            _6821pia.pia_0_cb1_w(0, scanline & 0x20);
            _6821pia.pia_1_ca1_w(0, scanline & 0x20);

            /* update the screen while we're here */
            williams2_vh_update(scanline);

            /* set a timer for the next update */
            scanline += 16;
            if (scanline >= 256) scanline = 0;
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(scanline), scanline, williams2_va11_callback);
        }
        public static void williams_videoram_w(int offset, int data)
        {
            /* only update if different */
            if (williams_videoram[offset] != data)
            {
                /* store to videoram and mark the scanline dirty */
                williams_videoram[offset] = (byte)data;
                scanline_dirty[offset % 256] = 1;
            }
        }

        public static int williams_vh_start()
        {
            /* allocate space for video RAM and dirty scanlines */
            williams_videoram = new _BytePtr(Generic.videoram_size[0] + 256);

            scanline_dirty = new _BytePtr(williams_videoram, Generic.videoram_size[0]);
            for (int i = 0; i < Generic.videoram_size[0]; i++) williams_videoram[i] = 0;
            for (int i = 0; i < 256; i++) scanline_dirty[i] = 1;

            /* pick the blitters */
            blitter_table = williams_blitters;
            if (williams_blitter_remap != 0) blitter_table = blaster_blitters;
            if (williams_blitter_clip != 0) blitter_table = sinistar_blitters;

            /* reset special-purpose flags */
            blaster_remap_lookup = null;
            blaster_erase_screen = 0;
            blaster_back_color = 0;
            sinistar_clip = 0xffff;

            return 0;
        }

        public static void williams_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            if (Mame.palette_recalc() != null || full_refresh != 0)
                for (int i = 0; i < 256; i++)
                    scanline_dirty[i] = 1;
        }
        public static void williams_vh_stop()
        {
            /* free any remap lookup tables */
            blaster_remap_lookup = null;

            williams_videoram = null;
            scanline_dirty = null;
        }

        static void williams2_endscreen_callback(int param)
        {
            /* the /ENDSCREEN signal comes into CA1 */
            _6821pia.pia_0_ca1_w(0, 0);

            /* set a timer to turn it off once the scanline counter resets */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(8), 0, williams2_endscreen_off_callback);

            /* set a timer for next frame */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(254), 0, williams2_endscreen_callback);
        }
        static _BytePtr scanline_dirty;

        public static int williams_video_counter_r(int offset)
        {
            return Mame.cpu_getscanline() & 0xfc;
        }
        public static void williams_vh_update(int counter)
        {
            Mame.rectangle clip = new Mame.rectangle();

            /* wrap around at the bottom */
            if (counter == 0) counter = 256;

            /* determine the clip rect */
            clip.min_x = Mame.Machine.drv.visible_area.min_x;
            clip.max_x = Mame.Machine.drv.visible_area.max_x;
            clip.min_y = counter - 16;
            clip.max_y = clip.min_y + 15;

            /* combine the clip rect with the visible rect */
            if (Mame.Machine.drv.visible_area.min_y > clip.min_y)
                clip.min_y = Mame.Machine.drv.visible_area.min_y;
            if (Mame.Machine.drv.visible_area.max_y < clip.max_y)
                clip.max_y = Mame.Machine.drv.visible_area.max_y;

            /* copy */
            if (Mame.Machine.scrbitmap.depth == 8)
            {
                if (williams_blitter_remap != 0)
                    copy_pixels_remap_8(Mame.Machine.scrbitmap, clip);
                else
                    copy_pixels_8(Mame.Machine.scrbitmap, clip);
            }
            else
            {
                if (williams_blitter_remap != 0)
                    copy_pixels_remap_16(Mame.Machine.scrbitmap, clip);
                else
                    copy_pixels_16(Mame.Machine.scrbitmap, clip);
            }

            /* optionally erase from lines 24 downward */
            if (blaster_erase_screen != 0 && clip.max_y > 24)
            {
                int offset, count;

                /* don't erase above row 24 */
                if (clip.min_y < 24) clip.min_y = 24;

                /* erase the memory associated with this area */
                count = clip.max_y - clip.min_y + 1;
                for (offset = clip.min_y; offset < Generic.videoram_size[0]; offset += 0x100)
                    for (int i = 0; i < count; i++)

                        williams_videoram[offset + i] = 0;
            }
        }
        static void copy_pixels_remap_8(Mame.osd_bitmap bitmap, Mame.rectangle clip)
        {
            throw new Exception();
        }
        static void mark_dirty(int x1, int y1, int x2, int y2)
        {
            int temp;

            /* swap X/Y */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
            {
                temp = x1; x1 = y1; y1 = temp;
                temp = x2; x2 = y2; y2 = temp;
            }

            /* flip X */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
            {
                temp = Mame.Machine.scrbitmap.width - 1;
                x1 = temp - x1;
                x2 = temp - x2;
                temp = x1; x1 = x2; x2 = temp;
            }

            /* flip Y */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
            {
                temp = Mame.Machine.scrbitmap.height - 1;
                y1 = temp - y1;
                y2 = temp - y2;
                temp = y1; y1 = y2; y2 = temp;
            }

            /* mark it */
            Mame.osd_mark_dirty(x1, y1, x2, y2, 0);
        }

        static void copy_pixels_8(Mame.osd_bitmap bitmap, Mame.rectangle clip)
        {
            ushort[] pens = Mame.Machine.pens;
            int pairs = (clip.max_x - clip.min_x + 1) / 2;
            int xoffset = clip.min_x;
            int x, y;

            /* standard case */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) == 0)
            {
                /* loop over rows */
                for (y = clip.min_y; y <= clip.max_y; y++)
                {
                    _BytePtr source = new _BytePtr(williams_videoram, y + 256 * (xoffset / 2));
                    _BytePtr dest;

                    /* skip if not dirty */
                    if (scanline_dirty[y] == 0) continue;
                    scanline_dirty[y] = 0;
                    mark_dirty(clip.min_x, y, clip.max_x, y);

                    /* compute starting destination pixel based on flip */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) == 0)
                        dest = new _BytePtr(bitmap.line[y]);
                    else
                        dest = new _BytePtr(bitmap.line[bitmap.height - 1 - y]);

                    /* non-X-flipped case */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) == 0)
                    {
                        dest.offset += xoffset;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset += 2)
                        {
                            int pix = source[0];
                            dest[0] = (byte)pens[pix >> 4];
                            dest[1] = (byte)pens[pix & 0x0f];
                        }
                    }

                    /* X-flipped case */
                    else
                    {
                        dest.offset += bitmap.width - xoffset;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset -= 2)
                        {
                            int pix = source[0];
                            dest[-1] = (byte)pens[pix >> 4];
                            dest[-2] = (byte)pens[pix & 0x0f];
                        }
                    }
                }
            }

            /* X/Y swapped case */
            else
            {
                int dy = (bitmap.line[1].offset - bitmap.line[0].offset);

                /* loop over rows */
                for (y = clip.min_y; y <= clip.max_y; y++)
                {
                    _BytePtr source = new _BytePtr(williams_videoram, y + 256 * (xoffset / 2));
                    _BytePtr dest;

                    /* skip if not dirty */
                    if (scanline_dirty[y] == 0) continue;
                    scanline_dirty[y] = 0;
                    mark_dirty(clip.min_x, y, clip.max_x, y);

                    /* compute starting destination pixel based on flip */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) == 0)
                        dest = new _BytePtr(bitmap.line[0], y);
                    else
                        dest = new _BytePtr(bitmap.line[0], bitmap.width - 1 - y);

                    /* non-Y-flipped case */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) == 0)
                    {
                        dest.offset += xoffset * dy;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset += dy + dy)
                        {
                            int pix = source[0];
                            dest[0] = (byte)pens[pix >> 4];
                            dest[dy] = (byte)pens[pix & 0x0f];
                        }
                    }

                    /* Y-flipped case */
                    else
                    {
                        dest.offset += (bitmap.height - xoffset) * dy;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset -= dy + dy)
                        {
                            int pix = source[0];
                            dest[-dy] = (byte)pens[pix >> 4];
                            dest[-dy - dy] = (byte)pens[pix & 0x0f];
                        }
                    }
                }
            }
        }
        static void copy_pixels_remap_16(Mame.osd_bitmap bitmap, Mame.rectangle clip)
        {
            throw new Exception();
        }
        static void copy_pixels_16(Mame.osd_bitmap bitmap, Mame.rectangle clip)
        {
            throw new Exception();
        }
        public static void williams2_vh_update(int counter)
        {
            throw new Exception();
        }
        delegate void _blitter(int sstart, int dstart, int w, int h, int data);
        static _blitter[] blitter_table;

        static _blitter[] williams_blitters = 
            {
	williams_blit_opaque,
	williams_blit_transparent,
	williams_blit_opaque_solid,
	williams_blit_transparent_solid
};
        static _blitter[] blaster_blitters;
        static _blitter[] sinistar_blitters;


        static void williams_blit_opaque(int sstart, int dstart, int w, int h, int data)
        {
            int source, sxadv, syadv;
            int dest, dxadv, dyadv;
            int i, j, solid;
            int keepmask;

            /* compute how much to advance in the x and y loops */
            sxadv = (data & 0x01) != 0 ? 0x100 : 1;
            syadv = (data & 0x01) != 0 ? 1 : w;
            dxadv = (data & 0x02) != 0 ? 0x100 : 1;
            dyadv = (data & 0x02) != 0 ? 1 : w;

            /* determine the common mask */
            keepmask = 0x00;
            if ((data & 0x80) != 0) keepmask |= 0xf0;
            if ((data & 0x40) != 0) keepmask |= 0x0f;
            if (keepmask == 0xff)
                return;

            /* set the solid pixel value to the mask value */
            solid = williams_blitterram[1];

            /* first case: no shifting */
            if ((data & 0x20) == 0)
            {
                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* loop over the width */
                    for (j = w; j > 0; j--)
                    {
                        int srcdata = Mame.cpu_readmem16(source);
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            // srcdata = (srcdata);
                            pix = (pix & keepmask) | (srcdata & ~keepmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
            /* second case: shifted one pixel */
            else
            {
                /* swap halves of the keep mask and the solid color */
                keepmask = ((keepmask & 0xf0) >> 4) | ((keepmask & 0x0f) << 4);
                solid = ((solid & 0xf0) >> 4) | ((solid & 0x0f) << 4);

                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    int pixdata, srcdata, shiftedmask;

                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* left edge case */
                    pixdata = Mame.cpu_readmem16(source);
                    srcdata = (pixdata >> 4) & 0x0f;
                    shiftedmask = keepmask | 0xf0;
                    {
                        int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                        // srcdata = (srcdata);
                        pix = (pix & shiftedmask) | (srcdata & ~shiftedmask);
                        if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                    };

                    source = (source + sxadv) & 0xffff;
                    dest = (dest + dxadv) & 0xffff;

                    /* loop over the width */
                    for (j = w - 1; j > 0; j--)
                    {
                        pixdata = (pixdata << 8) | Mame.cpu_readmem16(source);
                        srcdata = (pixdata >> 4) & 0xff;
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            //srcdata = (srcdata);
                            pix = (pix & keepmask) | (srcdata & ~keepmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    /* right edge case */
                    srcdata = (pixdata << 4) & 0xf0;
                    shiftedmask = keepmask | 0x0f;
                    {
                        int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                        //srcdata = (srcdata);
                        pix = (pix & shiftedmask) | (srcdata & ~shiftedmask);
                        if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                    };

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
        }
        static void williams_blit_transparent(int sstart, int dstart, int w, int h, int data)
        {
            int source, sxadv, syadv;
            int dest, dxadv, dyadv;
            int i, j, solid;
            int keepmask;

            /* compute how much to advance in the x and y loops */
            sxadv = (data & 0x01) != 0 ? 0x100 : 1;
            syadv = (data & 0x01) != 0 ? 1 : w;
            dxadv = (data & 0x02) != 0 ? 0x100 : 1;
            dyadv = (data & 0x02) != 0 ? 1 : w;

            /* determine the common mask */
            keepmask = 0x00;
            if ((data & 0x80) != 0) keepmask |= 0xf0;
            if ((data & 0x40) != 0) keepmask |= 0x0f;
            if (keepmask == 0xff)
                return;

            /* set the solid pixel value to the mask value */
            solid = williams_blitterram[1];

            /* first case: no shifting */
            if ((data & 0x20) == 0)
            {
                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* loop over the width */
                    for (j = w; j > 0; j--)
                    {
                        int srcdata = Mame.cpu_readmem16(source);
                        {
                            srcdata = (srcdata); if (srcdata != 0)
                            {
                                int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                                int tempmask = keepmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                                if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (srcdata & ~tempmask);
                                if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                            }
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
            /* second case: shifted one pixel */
            else
            {
                /* swap halves of the keep mask and the solid color */
                keepmask = ((keepmask & 0xf0) >> 4) | ((keepmask & 0x0f) << 4);
                solid = ((solid & 0xf0) >> 4) | ((solid & 0x0f) << 4);

                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    int pixdata, srcdata, shiftedmask;

                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* left edge case */
                    pixdata = Mame.cpu_readmem16(source);
                    srcdata = (pixdata >> 4) & 0x0f;
                    shiftedmask = keepmask | 0xf0;
                    {
                        srcdata = (srcdata); if (srcdata != 0)
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            int tempmask = shiftedmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                            if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (srcdata & ~tempmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        }
                    };

                    source = (source + sxadv) & 0xffff;
                    dest = (dest + dxadv) & 0xffff;

                    /* loop over the width */
                    for (j = w - 1; j > 0; j--)
                    {
                        pixdata = (pixdata << 8) | Mame.cpu_readmem16(source);
                        srcdata = (pixdata >> 4) & 0xff;
                        {
                            srcdata = (srcdata); if (srcdata != 0)
                            {
                                int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                                int tempmask = keepmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                                if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (srcdata & ~tempmask);
                                if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                            }
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    /* right edge case */
                    srcdata = (pixdata << 4) & 0xf0;
                    shiftedmask = keepmask | 0x0f;
                    {
                        srcdata = (srcdata); if (srcdata != 0)
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            int tempmask = shiftedmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                            if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (srcdata & ~tempmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        }
                    };

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
        }
        static void williams_blit_transparent_solid(int sstart, int dstart, int w, int h, int data)
        {
            int source, sxadv, syadv;
            int dest, dxadv, dyadv;
            int i, j, solid;
            int keepmask;

            /* compute how much to advance in the x and y loops */
            sxadv = (data & 0x01) != 0 ? 0x100 : 1;
            syadv = (data & 0x01) != 0 ? 1 : w;
            dxadv = (data & 0x02) != 0 ? 0x100 : 1;
            dyadv = (data & 0x02) != 0 ? 1 : w;

            /* determine the common mask */
            keepmask = 0x00;
            if ((data & 0x80) != 0) keepmask |= 0xf0;
            if ((data & 0x40) != 0) keepmask |= 0x0f;
            if (keepmask == 0xff)
                return;

            /* set the solid pixel value to the mask value */
            solid = williams_blitterram[1];

            /* first case: no shifting */
            if ((data & 0x20) == 0)
            {
                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* loop over the width */
                    for (j = w; j > 0; j--)
                    {
                        int srcdata = Mame.cpu_readmem16(source);
                        {
                            srcdata = (srcdata); if (srcdata != 0)
                            {
                                int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                                int tempmask = keepmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                                if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (solid & ~tempmask);
                                if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                            }
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
            /* second case: shifted one pixel */
            else
            {
                /* swap halves of the keep mask and the solid color */
                keepmask = ((keepmask & 0xf0) >> 4) | ((keepmask & 0x0f) << 4);
                solid = ((solid & 0xf0) >> 4) | ((solid & 0x0f) << 4);

                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    int pixdata, srcdata, shiftedmask;

                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* left edge case */
                    pixdata = Mame.cpu_readmem16(source);
                    srcdata = (pixdata >> 4) & 0x0f;
                    shiftedmask = keepmask | 0xf0;
                    {
                        srcdata = (srcdata); if (srcdata != 0)
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            int tempmask = shiftedmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                            if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (solid & ~tempmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        }
                    };

                    source = (source + sxadv) & 0xffff;
                    dest = (dest + dxadv) & 0xffff;

                    /* loop over the width */
                    for (j = w - 1; j > 0; j--)
                    {
                        pixdata = (pixdata << 8) | Mame.cpu_readmem16(source);
                        srcdata = (pixdata >> 4) & 0xff;
                        {
                            srcdata = (srcdata); if (srcdata != 0)
                            {
                                int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                                int tempmask = keepmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                                if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (solid & ~tempmask);
                                if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                            }
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    /* right edge case */
                    srcdata = (pixdata << 4) & 0xf0;
                    shiftedmask = keepmask | 0x0f;
                    {
                        srcdata = (srcdata); if (srcdata != 0)
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            int tempmask = shiftedmask; if ((srcdata & 0xf0) == 0) tempmask |= 0xf0;
                            if ((srcdata & 0x0f) == 0) tempmask |= 0x0f; pix = (pix & tempmask) | (solid & ~tempmask);
                            if (dest < 0x9800) williams_videoram[dest] = (byte)pix; else Mame.cpu_writemem16(dest, pix);
                        }
                    };

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
        }
        static void williams_blit_opaque_solid(int sstart, int dstart, int w, int h, int data)
        {
            int source, sxadv, syadv;
            int dest, dxadv, dyadv;
            int i, j, solid;
            int keepmask;

            /* compute how much to advance in the x and y loops */
            sxadv = (data & 0x01) != 0 ? 0x100 : 1;
            syadv = (data & 0x01) != 0 ? 1 : w;
            dxadv = (data & 0x02) != 0 ? 0x100 : 1;
            dyadv = (data & 0x02) != 0 ? 1 : w;

            /* determine the common mask */
            keepmask = 0x00;
            if ((data & 0x80) != 0) keepmask |= 0xf0;
            if ((data & 0x40) != 0) keepmask |= 0x0f;
            if (keepmask == 0xff)
                return;

            /* set the solid pixel value to the mask value */
            solid = williams_blitterram[1];

            /* first case: no shifting */
            if ((data & 0x20) == 0)
            {
                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* loop over the width */
                    for (j = w; j > 0; j--)
                    {
                        int srcdata = Mame.cpu_readmem16(source);
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            pix = (pix & keepmask) | (solid & ~keepmask); if (dest < 0x9800) williams_videoram[dest] = (byte)pix;
                            else Mame.cpu_writemem16(dest, pix);
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
            /* second case: shifted one pixel */
            else
            {
                /* swap halves of the keep mask and the solid color */
                keepmask = ((keepmask & 0xf0) >> 4) | ((keepmask & 0x0f) << 4);
                solid = ((solid & 0xf0) >> 4) | ((solid & 0x0f) << 4);

                /* loop over the height */
                for (i = 0; i < h; i++)
                {
                    int pixdata, srcdata, shiftedmask;

                    source = sstart & 0xffff;
                    dest = dstart & 0xffff;

                    /* left edge case */
                    pixdata = Mame.cpu_readmem16(source);
                    srcdata = (pixdata >> 4) & 0x0f;
                    shiftedmask = keepmask | 0xf0;
                    {
                        int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                        pix = (pix & shiftedmask) | (solid & ~shiftedmask); if (dest < 0x9800) williams_videoram[dest] = (byte)pix;
                        else Mame.cpu_writemem16(dest, pix);
                    };

                    source = (source + sxadv) & 0xffff;
                    dest = (dest + dxadv) & 0xffff;

                    /* loop over the width */
                    for (j = w - 1; j > 0; j--)
                    {
                        pixdata = (pixdata << 8) | Mame.cpu_readmem16(source);
                        srcdata = (pixdata >> 4) & 0xff;
                        {
                            int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                            pix = (pix & keepmask) | (solid & ~keepmask); if (dest < 0x9800) williams_videoram[dest] = (byte)pix;
                            else Mame.cpu_writemem16(dest, pix);
                        };

                        source = (source + sxadv) & 0xffff;
                        dest = (dest + dxadv) & 0xffff;
                    }

                    /* right edge case */
                    srcdata = (pixdata << 4) & 0xf0;
                    shiftedmask = keepmask | 0x0f;
                    {
                        int pix = ((dest < 0x9800) ? williams_videoram[dest] : Mame.cpu_readmem16(dest));
                        pix = (pix & shiftedmask) | (solid & ~shiftedmask); if (dest < 0x9800) williams_videoram[dest] = (byte)pix;
                        else Mame.cpu_writemem16(dest, pix);
                    };

                    sstart += syadv;
                    dstart += dyadv;
                }
            }
        }





        static void williams2_endscreen_off_callback(int param)
        {
            /* the /ENDSCREEN signal comes into CA1 */
            _6821pia.pia_0_ca1_w(0, 1);
        }
    }
}
namespace xnamame036.mame.drivers
{
    //Setup global static variables to be used across the williams drivers here
    class driver_defender : Mame.GameDriver
    {        static Mame.MemoryReadAddress[] defender_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x97ff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x9800, 0xbfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0xd000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};


        static Mame.MemoryWriteAddress[] defender_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x97ff, williams.williams_videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x9800, 0xbfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_BANK2, williams.defender_bank_base ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc00f, Mame.MWA_RAM, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, williams.defender_bank_select_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x007f, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0400, 0x0403, _6821pia.pia_2_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x8403, _6821pia.pia_2_r ),	/* used by Colony 7, perhaps others? */
	new Mame.MemoryReadAddress( 0xb000, 0xffff, Mame.MRA_ROM ),	/* most games start at $F000; Sinistar starts at $B000 */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};


static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x007f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0400, 0x0403,_6821pia.pia_2_w ),
	new Mame.MemoryWriteAddress( 0x8400, 0x8403,_6821pia.pia_2_w ),	/* used by Colony 7, perhaps others? */
	new Mame.MemoryWriteAddress( 0xb000, 0xffff, Mame.MWA_ROM ),	/* most games start at $F000; Sinistar starts at $B000 */
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static ushort cmos_base;
        static ushort cmos_length;

        /* banking addresses set by the drivers */
        
        _BytePtr mayday_protection;

        /* internal bank switching tracking */
        static byte blaster_bank;
        static byte vram_bank;
        byte williams2_bank;

        /* switches controlled by $c900 */
        ushort sinistar_clip;
        byte williams_cocktail;

        static Mame.DACinterface dac_interface = new Mame.DACinterface(1, new int[] { 50 });
        /* other stuff */
        static ushort joust2_current_sound_data;






        class machine_driver_defender : Mame.MachineDriver
        {
            public machine_driver_defender()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1000000, defender_readmem, defender_writemem, null, null, Mame.ignore_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6808 | Mame.CPU_AUDIO_CPU, 3579000 / 4, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 304;
                screen_height = 256;
                visible_area = new Mame.rectangle(6, 298 - 1, 7, 247 - 1);
                total_colors = 16;
                color_table_len = 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                /* standard init */
                williams.williams_init_machine();

                /* make sure the banking is reset to 0 */
                williams.defender_bank_select_w(0, 0);
                Mame.cpu_setbank(1, williams.williams_videoram);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
               _BytePtr ram = Mame.memory_region(Mame.REGION_CPU1);

                if (read_or_write!=0)
                    Mame.osd_fwrite(file, new _BytePtr(ram, cmos_base), cmos_length);
                else
                {
                    if (file!=null)
                        Mame.osd_fread(file, new _BytePtr(ram, cmos_base), cmos_length);
                    else
                        for (int i = 0; i < cmos_length; i++)
                        {
                            ram.buffer[ram.offset + i + cmos_base] = 0;
                        }
                        //memset(&ram[cmos_base], 0, cmos_length);
                }
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
               //nothing
            }
            public override int vh_start()
            {
                return williams.williams_vh_start();
            }
            public override void vh_stop()
            {
                williams.williams_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                williams.williams_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        void CONFIGURE_CMOS(ushort a, ushort l)
        {
            cmos_base = a; cmos_length = l;
        }
        static _6821pia.pia6821_interface defender_pia_0_intf =
new _6821pia.pia6821_interface(
            /*inputs : A/B,CA/B1,CA/B2 */ williams.defender_input_port_0_r, Mame.input_port_1_r, null, null, null, null,
            /*outputs: A/B,CA/B2       */ null, null, null, null,
            /*irqs   : A/B             */ null, null
);

        void CONFIGURE_PIAS(_6821pia.pia6821_interface a, _6821pia.pia6821_interface b, _6821pia.pia6821_interface c)
        {
            _6821pia.pia_unconfig();
            _6821pia.pia_config(0, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), a);
            _6821pia.pia_config(1, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), b);
            _6821pia.pia_config(2, (byte)(_6821pia.PIA_STANDARD_ORDERING | _6821pia.PIA_8BIT), c);
        }
        public override void driver_init()
        {
            uint[] bank = { 0x0c000, 0x10000, 0x11000, 0x12000, 0x0c000, 0x0c000, 0x0c000, 0x13000 };
            williams.defender_bank_list = bank;

            /* CMOS configuration */
            CONFIGURE_CMOS(0xc400, 0x100);

            /* PIA configuration */
            CONFIGURE_PIAS(defender_pia_0_intf, williams.williams_pia_1_intf, williams.williams_snd_pia_intf);
        }
        Mame.InputPortTiny[] input_ports_defender()
        {
            INPUT_PORTS_START("defender");
            PORT_START("IN0");
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, "Fire", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2, "Thrust", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3, "Smart Bomb", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON4, "Hyperspace", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BITX(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON6, "Reverse", (ushort)Mame.InputCodes.IP_KEY_DEFAULT, (ushort)Mame.InputCodes.IP_JOY_DEFAULT);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0xfe, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BITX(0x01, IP_ACTIVE_HIGH, 0, "Auto Up", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, 0, "Advance", (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, 0, "High Score Reset", (ushort)Mame.InputCodes.KEYCODE_7, IP_JOY_NONE);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_TILT);

            PORT_START("IN3");      /* IN3 - fake port for better joystick control */
            /* This fake port is handled via defender_input_port_1 */
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_CHEAT);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_CHEAT);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_defender()
        {
            ROM_START("defender");
            ROM_REGION(0x14000, Mame.REGION_CPU1);
            ROM_LOAD("defend.1", 0x0d000, 0x0800, 0xc3e52d7e);
            ROM_LOAD("defend.4", 0x0d800, 0x0800, 0x9a72348b);
            ROM_LOAD("defend.2", 0x0e000, 0x1000, 0x89b75984);
            ROM_LOAD("defend.3", 0x0f000, 0x1000, 0x94f51e9b);
            /* bank 0 is the place for CMOS ram */
            ROM_LOAD("defend.9", 0x10000, 0x0800, 0x6870e8a5);
            ROM_LOAD("defend.12", 0x10800, 0x0800, 0xf1f88938);
            ROM_LOAD("defend.8", 0x11000, 0x0800, 0xb649e306);
            ROM_LOAD("defend.11", 0x11800, 0x0800, 0x9deaf6d9);
            ROM_LOAD("defend.7", 0x12000, 0x0800, 0x339e092e);
            ROM_LOAD("defend.10", 0x12800, 0x0800, 0xa543b167);
            ROM_RELOAD(0x13800, 0x0800);
            ROM_LOAD("defend.6", 0x13000, 0x0800, 0x65f4efd1);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for the sound CPU */
            ROM_LOAD("defend.snd", 0xf800, 0x0800, 0xfefd5b48);
            return ROM_END;
        }
        public driver_defender()
        {
            drv = new machine_driver_defender();
            year = "1980";
            name = "defender";
            description = "Defender (Red label)";
            manufacturer = "Williams";
            flags = Mame.ROT0;
            input_ports = input_ports_defender();
            rom = rom_defender();
            drv.HasNVRAMhandler = true;
        }
    }
}
