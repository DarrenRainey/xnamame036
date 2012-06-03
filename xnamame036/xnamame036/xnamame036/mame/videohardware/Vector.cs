using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class Vector
    {
        const byte VCLEAN = 0, VDIRTY = 1, VCLIP = 2;
        public static _BytePtr vectorram = new _BytePtr(1);
        public static int[] vectorram_size = new int[1];


        static int antialias;                            /* flag for anti-aliasing */
        static int beam;                                 /* size of vector beam    */
        static int flicker;                              /* beam flicker value     */
        public static int translucency;

        static int beam_diameter_is_one;		  /* flag that beam is one pixel wide */

        static int vector_scale_x;                /* scaling to screen */
        static int vector_scale_y;                /* scaling to screen */

        static float gamma_correction = 1.2f;
        static float intensity_correction = 1.5f;

        const int MAX_POINTS = 5000;
        const int MAX_PIXELS = 850000;

        /* The vectices are buffered here */
        class point
        {
            public int x, y;
            public int col;
            public int intensity;
            public int arg1, arg2; /* start/end in pixel array or clipping info */
            public int status;         /* for dirty and clipping handling */
        }

        static point[] new_list;
        static point[] old_list;
        static int new_index;
        static int old_index;

        /* coordinates of pixels are stored here for faster removal */
        static uint[] pixel;
        static int p_index = 0;

        static uint[] pTcosin;            /* adjust line width */
        static byte[] pTinten;            /* intensity         */
        static ushort[] pTmerge;            /* mergeing pixels   */
        static ushort[] invpens;            /* maps OS colors to pens */

        static _ShortPtr pens;
        static uint total_colors;


        const byte ANTIALIAS_GUNBIT = 6;             /* 6 bits per gun in vga (1-8 valid) */
        const byte ANTIALIAS_GUNNUM = (1 << ANTIALIAS_GUNBIT);

        static byte[] Tgamma = new byte[256];         /* quick gamma anti-alias table  */
        static byte[] Tgammar = new byte[256];        /* same as above, reversed order */

        static Mame.osd_bitmap vecbitmap;
        static int vecwidth, vecheight;
        static int vecshift;
        static int xmin, ymin, xmax, ymax; /* clipping area */

        static int vector_runs;	/* vector runs per refresh */

        public static void vector_set_shift(int shift)
        {
            vecshift = shift;
        }
        static ushort find_pen(byte r, byte g, byte b)
        {
            int i, bi, ii;
            int x, y, z, bc;
            ii = 32;
            bi = 256;
            bc = 0x01000000;

            do
            {
                for (i = 0; i < total_colors; i++)
                {
                    byte r1 = 0, g1 = 0, b1 = 0;

                    Mame.osd_get_pen(pens.read16(i), ref r1, ref g1, ref b1);
                    if ((x = (int)(Math.Abs(r1 - r) + 1)) > ii) continue;
                    if ((y = (int)(Math.Abs(g1 - g) + 1)) > ii) continue;
                    if ((z = (int)(Math.Abs(b1 - b) + 1)) > ii) continue;
                    x = x * y * z;
                    if (x < bc)
                    {
                        bc = x;
                        bi = i;
                    }
                }
                ii <<= 1;
            } while (bi == 256);

            return (ushort)(bi);
        }

        delegate void _vector_pp(Mame.osd_bitmap bitmap, int x, int y, int pen);
        delegate int _vector_rp(Mame.osd_bitmap bitmap, int x, int y);
        static _vector_pp vector_pp;
        static _vector_rp vector_rp;

        static void vector_pp_8(Mame.osd_bitmap b, int x, int y, int p) { b.line[y][x] = (byte)p; }
        static void vector_pp_16(Mame.osd_bitmap b, int x, int y, int p) { b.line[y].write16(x, (ushort)p); }
        static int vector_rp_8(Mame.osd_bitmap b, int x, int y) { return b.line[y][x]; }
        static int vector_rp_16(Mame.osd_bitmap b, int x, int y) { return b.line[y].read16(x); }

        public static int vector_vh_start()
        {
            int h, i, j, k;
            int[] c = new int[3];


            /* Grab the settings for this session */
            antialias = Mame.options.antialias;
            translucency = Mame.options.translucency;
            flicker = Mame.options.flicker;
            beam = Mame.options.beam;

            pens = Mame.Machine.pens;
            total_colors = Mame.Machine.drv.total_colors;

            if (Mame.Machine.color_depth == 8)
            {
                vector_pp = vector_pp_8;
                vector_rp = vector_rp_8;
            }
            else
            {
                vector_pp = vector_pp_16;
                vector_rp = vector_rp_16;
            }

            if (beam == 0x00010000)
                beam_diameter_is_one = 1;
            else
                beam_diameter_is_one = 0;

            p_index = 0;

            new_index = 0;
            old_index = 0;
            vector_runs = 0;

            /* allocate memory for tables */
            pTcosin = new uint[(2048 + 1)];   /* yes! 2049 is correct */
            pTinten = new byte[total_colors * 256];
            pTmerge = new ushort[(total_colors * total_colors) * 2];
            invpens = new ushort[(65536)];
            pixel = new uint[MAX_PIXELS];
            old_list = new point[MAX_POINTS];
            new_list = new point[MAX_POINTS];
            for (i = 0; i < MAX_POINTS; i++) { new_list[i] = new point(); old_list[i] = new point(); }
            /* build cosine table for fixing line width in antialias */
            for (i = 0; i <= 2048; i++)
            {
                pTcosin[i] = (uint)((int)((double)(1.0 / Math.Cos(Math.Atan((double)(i) / 2048.0))) * 0x10000000 + 0.5));
            }
            for (i = 0; i < 65536; i++)
                invpens[i] = 0;
            //memset (invpens, 0, 65536 * sizeof(unsigned short));
            for (i = 0; i < total_colors; i++)
                invpens[Mame.Machine.pens.read16(i)] = (ushort)i;

            /* build anti-alias table */
            h = 256 / ANTIALIAS_GUNNUM;           /* to generate table faster */
            for (i = 0; i < 256; i += h)               /* intensity */
            {
                for (j = 0; j < total_colors; j++)               /* color */
                {
                    byte r1 = 0, g1 = 0, b1 = 0, pen, n;
                    Mame.osd_get_pen(pens.read16(j), ref r1, ref g1, ref b1);
                    pen = (byte)find_pen((byte)((r1 * (i + 1)) >> 8), (byte)((g1 * (i + 1)) >> 8), (byte)((b1 * (i + 1)) >> 8));
                    for (n = 0; n < h; n++)
                    {
                        pTinten[(i + n) * total_colors + j] = pen;
                    }
                }
            }

            /* build merge color table */
            for (i = 0; i < total_colors; i++)                /* color1 */
            {
                byte[] rgb1 = new byte[3], rgb2 = new byte[3];

                Mame.osd_get_pen(pens.read16(i), ref rgb1[0], ref rgb1[1], ref rgb1[2]);
                for (j = 0; j <= i; j++)               /* color2 */
                {
                    Mame.osd_get_pen(pens.read16(j), ref rgb2[0], ref rgb2[1], ref rgb2[2]);

                    for (k = 0; k < 3; k++)
                        if (translucency != 0) /* add gun values */
                        {
                            int tmp;
                            tmp = rgb1[k] + rgb2[k];
                            if (tmp > 255)
                                c[k] = 255;
                            else
                                c[k] = tmp;
                        }
                        else /* choose highest gun value */
                        {
                            if (rgb1[k] > rgb2[k])
                                c[k] = rgb1[k];
                            else
                                c[k] = rgb2[k];
                        }
                    pTmerge[i * total_colors + j] = pTmerge[(j * total_colors) + i] = find_pen((byte)c[0], (byte)c[1], (byte)c[2]);
                }
            }

            /* build gamma correction table */
            vector_set_gamma(gamma_correction);

            return 0;
        }
        static void vector_set_gamma(float _gamma)
        {
            gamma_correction = _gamma;

            for (int i = 0; i < 256; i++)
            {
                int h = (int)(255.0 * Math.Pow(i / 255.0, 1.0 / gamma_correction));
                if (h > 255) h = 255;
                Tgamma[i] = Tgammar[255 - i] = (byte)h;
            }
        }

        public static void vector_clear_list()
        {
            if (vector_runs == 0)
            {
                old_index = new_index;
                var tmp = old_list; old_list = new_list; new_list = tmp;
            }

            new_index = 0;
            vector_runs++;
        }
        public static void vector_add_point(int x, int y, int color, int intensity)
        {

            intensity *= (int)intensity_correction;
            if (intensity > 0xff)
                intensity = 0xff;

            if (flicker != 0 && (intensity > 0))
            {
                intensity += (intensity * (0x80 - (Mame.rand() & 0xff)) * flicker) >> 16;
                if (intensity < 0)
                    intensity = 0;
                if (intensity > 0xff)
                    intensity = 0xff;
            }
            new_list[new_index].x = x;
            new_list[new_index].y = y;
            new_list[new_index].col = color;
            new_list[new_index].intensity = intensity;
            new_list[new_index].status = VDIRTY; /* mark identical lines as clean later */

            new_index++;
            if (new_index >= MAX_POINTS)
            {
                new_index--;
                //Mame.printf("*** Warning! Vector list overflow!\n");
            }
        }
        static void vector_clear_pixels()
        {
            byte bg = (byte)pens.read16(0);

            for (int i = p_index - 1; i >= 0; i--)
            {
                int coords = (int)pixel[i];
                vector_pp(vecbitmap, coords >> 16, coords & 0x0000ffff, bg);
            }

            p_index = 0;
        }
        public static void vector_vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int i;
            int temp_x, temp_y;

            /* copy parameters */
            vecbitmap = bitmap;
            vecwidth = bitmap.width;
            vecheight = bitmap.height;

            /* setup scaling */
            temp_x = (1 << (44 - vecshift)) / (Mame.Machine.drv.visible_area.max_x - Mame.Machine.drv.visible_area.min_x);
            temp_y = (1 << (44 - vecshift)) / (Mame.Machine.drv.visible_area.max_y - Mame.Machine.drv.visible_area.min_y);

            if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
            {
                vector_scale_x = temp_x * vecheight;
                vector_scale_y = temp_y * vecwidth;
            }
            else
            {
                vector_scale_x = temp_x * vecwidth;
                vector_scale_y = temp_y * vecheight;
            }
            /* reset clipping area */
            xmin = 0; xmax = vecwidth; ymin = 0; ymax = vecheight;

            /* next call to vector_clear_list() is allowed to swap the lists */
            vector_runs = 0;

            /* mark pixels which are not idential in newlist and oldlist dirty */
            /* the old pixels which get removed are marked dirty immediately,  */
            /* new pixels are recognized by setting new.dirty                 */
            clever_mark_dirty();

            /* clear ALL pixels in the hidden map */
            vector_clear_pixels();

            /* Draw ALL lines into the hidden map. Mark only those lines with */
            /* new.dirty = 1 as dirty. Remember the pixel start/end indices  */
            int ni = 0;
            for (i = 0; i < new_index; i++)
            {
                if (new_list[ni].status == VCLIP)
                    vector_set_clip(new_list[ni].x, new_list[ni].y, new_list[ni].arg1, new_list[ni].arg2);
                else
                {
                    new_list[ni].arg1 = p_index;
                    vector_draw_to(new_list[ni].x, new_list[ni].y, new_list[ni].col, Tgamma[new_list[ni].intensity], new_list[ni].status);

                    new_list[ni].arg2 = p_index;
                }
                ni++;
            }
        }
        static void vector_set_clip(int x1, int yy1, int x2, int y2)
        {
            int orientation;
            int tmp;

            /* failsafe */
            if ((x1 >= x2) || (yy1 >= y2))
            {
                Mame.printf("Error in clipping parameters.\n");
                xmin = 0;
                ymin = 0;
                xmax = vecwidth;
                ymax = vecheight;
                return;
            }

            /* scale coordinates to display */
            x1 = vec_mult(x1 << 4, vector_scale_x);
            yy1 = vec_mult(yy1 << 4, vector_scale_y);
            x2 = vec_mult(x2 << 4, vector_scale_x);
            y2 = vec_mult(y2 << 4, vector_scale_y);

            /* fix orientation */
            orientation = Mame.Machine.orientation;
            /* swapping x/y coordinates will still have the minima in x1,yy1 */
            if ((orientation & Mame.ORIENTATION_SWAP_XY) != 0)
            {
                tmp = x1; x1 = yy1; yy1 = tmp;
                tmp = x2; x2 = y2; y2 = tmp;
            }
            /* don't forget to swap x1,x2, since x2 becomes the minimum */
            if ((orientation & Mame.ORIENTATION_FLIP_X) != 0)
            {
                x1 = ((vecwidth - 1) << 16) - x1;
                x2 = ((vecwidth - 1) << 16) - x2;
                tmp = x1; x1 = x2; x2 = tmp;
            }
            /* don't forget to swap yy1,y2, since y2 becomes the minimum */
            if ((orientation & Mame.ORIENTATION_FLIP_Y) != 0)
            {
                yy1 = ((vecheight - 1) << 16) - yy1;
                y2 = ((vecheight - 1) << 16) - y2;
                tmp = yy1; yy1 = y2; y2 = tmp;
            }

            xmin = x1 >> 16;
            ymin = yy1 >> 16;
            xmax = x2 >> 16;
            ymax = y2 >> 16;

            /* Make it foolproof by trapping rounding errors */
            if (xmin < 0) xmin = 0;
            if (ymin < 0) ymin = 0;
            if (xmax > vecwidth) xmax = vecwidth;
            if (ymax > vecheight) ymax = vecheight;
        }

        static int vec_mult(int parm1, int parm2)
        {
            int temp, result;

            temp = Math.Abs(parm1);
            result = (temp & 0x0000ffff) * (parm2 & 0x0000ffff);
            result >>= 16;
            result += (temp & 0x0000ffff) * (parm2 >> 16);
            result += (temp >> 16) * (parm2 & 0x0000ffff);
            result >>= 16;
            result += (temp >> 16) * (parm2 >> 16);

            if (parm1 < 0)
                return (-result);
            else
                return (result);
        }
        static int vec_div(int parm1, int parm2)
        {
            if ((parm2 >> 12) != 0)
            {
                parm1 = (parm1 << 4) / (parm2 >> 12);
                if (parm1 > 0x00010000)
                    return (0x00010000);
                if (parm1 < -0x00010000)
                    return (-0x00010000);
                return (parm1);
            }
            return (0x00010000);
        }
        static void clever_mark_dirty()
        {
            int i, j, min_index, last_match = 0;
            UIntSubArray coords;
            point newclip = new point(), oldclip = new point();
            bool clips_match = true;

            if (old_index < new_index)
                min_index = old_index;
            else
                min_index = new_index;

            /* Reset the active clips to invalid values */
            //memset (&newclip, 0, sizeof (newclip));
            //memset (&oldclip, 0, sizeof (oldclip));

            /* Mark vectors which are not the same in both lists as dirty */
            int ni = 0, oi = 0;
            for (i = min_index; i > 0; i--, oi++, ni++)
            {
                /* If this is a clip, we need to determine if the clip regions still match */
                if (old_list[oi].status == VCLIP || new_list[ni].status == VCLIP)
                {
                    if (old_list[oi].status == VCLIP)
                        oldclip = old_list[oi];
                    if (new_list[ni].status == VCLIP)
                        newclip = new_list[ni];
                    clips_match = (newclip.x == oldclip.x) && (newclip.y == oldclip.y) && (newclip.arg1 == oldclip.arg1) && (newclip.arg2 == oldclip.arg2);
                    if (!clips_match)
                        last_match = 0;

                    /* fall through to erase the old line if this is not a clip */
                    if (old_list[oi].status == VCLIP)
                        continue;
                }

                /* If the clips match and the vectors match, update */
                else if (clips_match && (new_list[ni].x == old_list[oi].x) && (new_list[ni].y == old_list[oi].y) &&
                    (new_list[ni].col == old_list[oi].col) && (new_list[ni].intensity == old_list[oi].intensity))
                {
                    if (last_match != 0)
                    {
                        new_list[ni].status = VCLEAN;
                        continue;
                    }
                    last_match = 1;
                }

                /* If nothing matches, remember it */
                else
                    last_match = 0;

                /* mark the pixels of the old vector dirty */
                coords = new UIntSubArray(pixel, (int)old_list[oi].arg1);
                int ci = 0;
                for (j = (old_list[oi].arg2 - old_list[oi].arg1); j > 0; j--)
                {
                    osd_mark_vector_dirty((int)coords[ci] >> 16, (int)coords[ci] & 0x0000ffff);
                    ci++;
                }
            }

            /* all old vector with index greater new_index are dirty */
            /* old = &old_list[min_index] here! */
            for (i = (old_index - min_index); i > 0; i--, oi++)
            {
                /* skip old clips */
                if (old_list[oi].status == VCLIP)
                    continue;

                /* mark the pixels of the old vector dirty */
                int ci = 0;
                coords = new UIntSubArray(pixel, (int)old_list[oi].arg1);
                for (j = (old_list[oi].arg2 - old_list[oi].arg1); j > 0; j--)
                {
                    osd_mark_vector_dirty((int)coords[ci] >> 16, (int)coords[ci] & 0x0000ffff);
                    ci++;
                }
            }
        }
        static void osd_mark_vector_dirty(int x, int y)
        {
            Mame.dirty_new[y / 16 * Mame.DIRTY_H + x / 16] = 1;
        }
        static int x1, yy1;
        static void vector_draw_to(int x2, int y2, int col, int intensity, int dirty)
        {
            byte a1;
            int orientation;
            int dx, dy, sx, sy, cx, cy, width;
            int xx, yy;

#if false
	Mame.printf("line:%d,%d nach %d,%d color %d\n",x1,yy1,x2,y2,col);
#endif

            /* [1] scale coordinates to display */

            x2 = vec_mult(x2 << 4, vector_scale_x);
            y2 = vec_mult(y2 << 4, vector_scale_y);

            /* [2] fix display orientation */

            orientation = Mame.Machine.orientation;
            if ((orientation & Mame.ORIENTATION_SWAP_XY) != 0)
            {
                int temp;
                temp = x2;
                x2 = y2;
                y2 = temp;
            }
            if ((orientation & Mame.ORIENTATION_FLIP_X) != 0)
                x2 = ((vecwidth - 1) << 16) - x2;
            if ((orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                y2 = ((vecheight - 1) << 16) - y2;

            /* [3] adjust cords if needed */

            if (antialias != 0)
            {
                if (beam_diameter_is_one != 0)
                {
                    x2 = (int)((x2 + 0x8000) & 0xffff0000);
                    y2 = (int)((y2 + 0x8000) & 0xffff0000);
                }
            }
            else /* noantialiasing */
            {
                x2 >>= 16;
                y2 >>= 16;
            }

            /* [4] handle color and intensity */

            if (intensity == 0) goto end_draw;

            col = pTinten[intensity * total_colors + col];

            /* [5] draw line */

            if (antialias != 0)
            {
                /* draw an anti-aliased line */
                dx = Math.Abs(x1 - x2);
                dy = Math.Abs(yy1 - y2);
                if (dx >= dy)
                {
                    sx = ((x1 <= x2) ? 1 : -1);
                    sy = vec_div(y2 - yy1, dx);
                    if (sy < 0)
                        dy--;
                    x1 >>= 16;
                    xx = x2 >> 16;
                    width = vec_mult(beam << 4, (int)pTcosin[Math.Abs(sy) >> 5]);
                    if (beam_diameter_is_one == 0)
                        yy1 -= width >> 1; /* start back half the diameter */
                    for (; ; )
                    {
                        dx = width;    /* init diameter of beam */
                        dy = yy1 >> 16;
                        vector_draw_aa_pixel(x1, dy++, pTinten[Tgammar[0xff & (yy1 >> 8)] * total_colors + col], dirty);
                        dx -= 0x10000 - (0xffff & yy1); /* take off amount plotted */
                        a1 = Tgamma[(dx >> 8) & 0xff];   /* calc remainder pixel */
                        dx >>= 16;                   /* adjust to pixel (solid) count */
                        while (dx-- != 0)                 /* plot rest of pixels */
                            vector_draw_aa_pixel(x1, dy++, col, dirty);
                        vector_draw_aa_pixel(x1, dy, pTinten[a1 * total_colors + col], dirty);
                        if (x1 == xx) break;
                        x1 += sx;
                        yy1 += sy;
                    }
                }
                else
                {
                    sy = ((yy1 <= y2) ? 1 : -1);
                    sx = vec_div(x2 - x1, dy);
                    if (sx < 0)
                        dx--;
                    yy1 >>= 16;
                    yy = y2 >> 16;
                    width = vec_mult(beam << 4, (int)pTcosin[Math.Abs(sx) >> 5]);
                    if (beam_diameter_is_one == 0)
                        x1 -= width >> 1; /* start back half the width */
                    for (; ; )
                    {
                        dy = width;    /* calc diameter of beam */
                        dx = x1 >> 16;
                        vector_draw_aa_pixel(dx++, yy1, pTinten[Tgammar[0xff & (x1 >> 8)] * total_colors + col], dirty);
                        dy -= 0x10000 - (0xffff & x1); /* take off amount plotted */
                        a1 = Tgamma[(dy >> 8) & 0xff];   /* remainder pixel */
                        dy >>= 16;                   /* adjust to pixel (solid) count */
                        while (dy-- != 0)                 /* plot rest of pixels */
                            vector_draw_aa_pixel(dx++, yy1, col, dirty);
                        vector_draw_aa_pixel(dx, yy1, pTinten[a1 * total_colors + col], dirty);
                        if (yy1 == yy) break;
                        yy1 += sy;
                        x1 += sx;
                    }
                }
            }
            else /* use good old Bresenham for non-antialiasing 980317 BW */
            {
                dx = Math.Abs(x1 - x2);
                dy = Math.Abs(yy1 - y2);
                sx = (x1 <= x2) ? 1 : -1;
                sy = (yy1 <= y2) ? 1 : -1;
                cx = dx / 2;
                cy = dy / 2;

                if (dx >= dy)
                {
                    for (; ; )
                    {
                        vector_draw_aa_pixel(x1, yy1, col, dirty);
                        if (x1 == x2) break;
                        x1 += sx;
                        cx -= dy;
                        if (cx < 0)
                        {
                            yy1 += sy;
                            cx += dx;
                        }
                    }
                }
                else
                {
                    for (; ; )
                    {
                        vector_draw_aa_pixel(x1, yy1, col, dirty);
                        if (yy1 == y2) break;
                        yy1 += sy;
                        cy -= dx;
                        if (cy < 0)
                        {
                            x1 += sx;
                            cy += dy;
                        }
                    }
                }
            }

        end_draw:

            x1 = x2;
            yy1 = y2;
        }
        static void vector_draw_aa_pixel(int x, int y, int col, int dirty)
        {
            if (x < xmin || x >= xmax)
                return;
            if (y < ymin || y >= ymax)
                return;

            vector_pp(vecbitmap, x, y, pens.read16(pTmerge[invpens[vector_rp(vecbitmap, x, y)] * total_colors + col]));

            if (p_index < MAX_PIXELS)
            {
                pixel[p_index] = (uint)(y | (x << 16));
                p_index++;
            }

            /* Mark this pixel as dirty */
            if (dirty != 0)
                osd_mark_vector_dirty(x, y);
        }
        public static void vector_add_clip(int x1, int yy1, int x2, int y2)
        {
            new_list[new_index].x = x1;
            new_list[new_index].y = yy1;
            new_list[new_index].arg1 = x2;
            new_list[new_index].arg2 = y2;
            new_list[new_index].status = VCLIP;

            new_index++;
            if (new_index >= MAX_POINTS)
            {
                new_index--;
                Mame.printf("*** Warning! Vector list overflow!\n");
            }
        }
        public static void vector_vh_stop()
        {
            pTcosin = null;
            pTinten = null;
            pTmerge = null;
            pixel = null;
            old_list = null;
            new_list = null;
        }
        public static void vector_vh_update_backdrop(Mame.osd_bitmap bitmap, Mame.artwork a, int full_refresh)
        {
            throw new Exception();
        }
        public static void vector_vh_update_artwork(Mame.osd_bitmap bitmap, Mame.artwork o, Mame.artwork b, int full_refresh)
        {
            throw new Exception();
        }
        public static void vector_vh_update_overlay(Mame.osd_bitmap bitmap, Mame.artwork a, int full_refresh)
{
            throw new Exception();
        }
    }
}
