using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class williams
    {
        static _BytePtr scanline_dirty;
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
		if (williams_blitter_remap!=0)
            copy_pixels_remap_8(Mame.Machine.scrbitmap, clip);
		else
            copy_pixels_8(Mame.Machine.scrbitmap, clip);
	}
	else
	{
		if (williams_blitter_remap!=0)
            copy_pixels_remap_16(Mame.Machine.scrbitmap, clip);
		else
            copy_pixels_16(Mame.Machine.scrbitmap, clip);
	}

	/* optionally erase from lines 24 downward */
	if (blaster_erase_screen!=0 && clip.max_y > 24)
	{
		int offset, count;

		/* don't erase above row 24 */
		if (clip.min_y < 24) clip.min_y = 24;

		/* erase the memory associated with this area */
		count = clip.max_y - clip.min_y + 1;
		for (offset = clip.min_y; offset < Generic.videoram_size[0]; offset += 0x100)
            for (int i=0;i<count;i++)

			williams_videoram[offset+i]=0;
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
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
            {
                temp = Mame.Machine.scrbitmap.width - 1;
                x1 = temp - x1;
                x2 = temp - x2;
                temp = x1; x1 = x2; x2 = temp;
            }

            /* flip Y */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
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
            _ShortPtr pens = Mame.Machine.pens;
            int pairs = (clip.max_x - clip.min_x + 1) / 2;
            int xoffset = clip.min_x;
            int x, y;

            /* standard case */
            if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY)==0)
            {
                /* loop over rows */
                for (y = clip.min_y; y <= clip.max_y; y++)
                {
                    _BytePtr source = new _BytePtr(williams_videoram,  y + 256 * (xoffset / 2));
                    _BytePtr dest;

                    /* skip if not dirty */
                    if (scanline_dirty[y]==0) continue;
                    scanline_dirty[y] = 0;
                    mark_dirty(clip.min_x, y, clip.max_x, y);

                    /* compute starting destination pixel based on flip */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)==0)
                        dest = new _BytePtr(bitmap.line[y]);
                    else
                        dest = new _BytePtr(bitmap.line[bitmap.height - 1 - y]);

                    /* non-X-flipped case */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)==0)
                    {
                        dest.offset += xoffset;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset += 2)
                        {
                            int pix = source[0];
                            dest[0] = (byte)pens.read16(pix >> 4);
                            dest[1] = (byte)pens.read16(pix & 0x0f);
                        }
                    }

                    /* X-flipped case */
                    else
                    {
                        dest.offset += bitmap.width - xoffset;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset -= 2)
                        {
                            int pix = source[0];
                            dest[-1] = (byte)pens.read16(pix >> 4);
                            dest[-2] = (byte)pens.read16(pix & 0x0f);
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
                    _BytePtr source = new _BytePtr(williams_videoram,  y + 256 * (xoffset / 2));
                    _BytePtr  dest;

                    /* skip if not dirty */
                    if (scanline_dirty[y]==0) continue;
                    scanline_dirty[y] = 0;
                    mark_dirty(clip.min_x, y, clip.max_x, y);

                    /* compute starting destination pixel based on flip */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)==0)
                        dest = new _BytePtr(bitmap.line[0], y);
                    else
                        dest = new _BytePtr(bitmap.line[0], bitmap.width - 1 - y);

                    /* non-Y-flipped case */
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)==0)
                    {
                        dest.offset += xoffset * dy;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset += dy + dy)
                        {
                            int pix = source[0];
                            dest[0] = (byte)pens.read16(pix >> 4);
                            dest[dy] = (byte)pens.read16(pix & 0x0f);
                        }
                    }

                    /* Y-flipped case */
                    else
                    {
                        dest.offset += (bitmap.height - xoffset) * dy;
                        for (x = 0; x < pairs; x++, source.offset += 256, dest.offset -= dy + dy)
                        {
                            int pix = source[0];
                            dest[-dy] = (byte)pens.read16(pix >> 4);
                            dest[-dy - dy] = (byte)pens.read16(pix & 0x0f);
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
            if (Mame.palette_recalc() != null || full_refresh!=0)
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
    }
}
