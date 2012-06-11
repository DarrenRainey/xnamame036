using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {

        public class artwork
        {
            /* Publically accessible */
            public osd_bitmap _artwork;

            /* Private - don't touch! */
            public osd_bitmap orig_artwork;	/* needed for palette recalcs */
            public osd_bitmap vector_bitmap;	/* needed to buffer the vector image in vg with overlays */
            public byte[] orig_palette;		/* needed for restoring the colors after special effects? */
            public int num_pens_used;
            public byte[] transparency;
            public int num_pens_trans;
            public int start_pen;
            public byte[] brightness;              /* brightness of each palette entry */
            public byte[] pTable;                  /* Conversion table usually used for mixing colors */
        };


        public class artwork_element
        {
            public artwork_element(rectangle box, byte[] colors, byte alpha)
            {
                this.box = box;
                this.red = colors[0]; this.green = colors[1]; this.blue = colors[2]; this.alpha = alpha;
            }
            public rectangle box;
            public byte red, green, blue, alpha;
        };
        public static void overlay_remap(artwork a)
        {
            throw new Exception();
        }
        public static void overlay_draw(osd_bitmap dest, artwork overlay)
        {
            int i, j;
            int height, width;
            osd_bitmap o = null;
            int black;

            o = overlay._artwork;
            height = overlay._artwork.height;
            width = overlay._artwork.width;
            black = Machine.pens[0];

            if (dest.depth == 8)
            {
                _BytePtr dst, ovr;

                for (j = 0; j < height; j++)
                {
                    dst = new _BytePtr(dest.line[j]);
                    ovr = new _BytePtr(o.line[j]);
                    for (i = 0; i < width; i++)
                    {
                        if (dst[0] != black)
                            dst[0] = ovr[0];
                        dst.offset++;
                        ovr.offset++;
                    }
                }
            }
            else
            {
                _ShortPtr dst, ovr;

                for (j = 0; j < height; j++)
                {
                    dst = new _ShortPtr(dest.line[j]);
                    ovr = new _ShortPtr(o.line[j]);
                    for (i = 0; i < width; i++)
                    {
                        if (dst.read16(0) != black)
                            dst.write16(0, ovr.read16(0));
                        dst.offset += 2;
                        ovr.offset += 2;
                    }
                }
            }
        }
        static artwork allocate_artwork_mem(int width, int height)
        {
            artwork a;
            int temp;

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                temp = height;
                height = width;
                width = temp;
            }

            a = new artwork();

            a.transparency = null;
            a.orig_palette = null;
            a.pTable = null;
            a.brightness = null;
            a.vector_bitmap = null;

            a.orig_artwork = osd_create_bitmap(width, height);

            fillbitmap(a.orig_artwork, 0, null);

            /* Create a second bitmap for public use */
            a._artwork = osd_create_bitmap(width, height);

            a.pTable = new byte[256 * 256];

            a.brightness = new byte[256 * 256];
            memset(a.brightness, 0, 256 * 256);

            /* Create bitmap for the vector screen */
            if ((Machine.drv.video_attributes & VIDEO_TYPE_VECTOR) != 0)
            {
                a.vector_bitmap = osd_create_bitmap(width, height);
                fillbitmap(a.vector_bitmap, 0, null);
            }

            return a;
        }
        static osd_bitmap create_circle(int r, int pen)
        {
            osd_bitmap circle;

            int x = 0, twox = 0;
            int y = r;
            int twoy = r + r;
            int p = 1 - r;
            int i;

            circle = osd_create_bitmap(twoy, twoy);

            /* background */
            fillbitmap(circle, 255, null);

            while (x < y)
            {
                x++;
                twox += 2;
                if (p < 0)
                    p += twox + 1;
                else
                {
                    y--;
                    twoy -= 2;
                    p += twox - twoy + 1;
                }

                for (i = 0; i < twox; i++)
                {
                    plot_pixel(circle, r - x + i, r - y, pen);
                    plot_pixel(circle, r - x + i, r + y - 1, pen);
                }

                for (i = 0; i < twoy; i++)
                {
                    plot_pixel(circle, r - y + i, r - x, pen);
                    plot_pixel(circle, r - y + i, r + x - 1, pen);
                }
            }
            return circle;
        }
        public static void backdrop_refresh(artwork a)
        {
            int i, j;
            int height, width;
            osd_bitmap back = null;
            osd_bitmap orig = null;
            int offset;

            offset = a.start_pen;
            back = a._artwork;
            orig = a.orig_artwork;
            height = a._artwork.height;
            width = a._artwork.width;

            if (back.depth == 8)
            {
                for (j = 0; j < height; j++)
                    for (i = 0; i < width; i++)
                        back.line[j][i] = (byte)Machine.pens[orig.line[j][i] + offset];
            }
            else
            {
                for (j = 0; j < height; j++)
                    for (i = 0; i < width; i++)
                        back.line[j].write16(i, Machine.pens[orig.line[j].read16(i + offset)]);
            }
        }
        static void backdrop_set_palette(artwork a, byte[] palette)
        {
            /* Load colors into the palette */
            if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
            {
                for (int i = 0; i < a.num_pens_used; i++)
                    palette_change_color(i + a.start_pen, palette[i * 3], palette[i * 3 + 1], palette[i * 3 + 2]);

                palette_recalc();
                backdrop_refresh(a);
            }
        }

        public static artwork artwork_create(artwork_element[] ae, int start_pen, int max_pens)
        {
            artwork a;
            osd_bitmap circle;
            int pen;

            if ((a = allocate_artwork_mem(Machine.scrbitmap.width, Machine.scrbitmap.height)) == null)
                return null;

            a.start_pen = start_pen;

            a.orig_palette = new byte[256 * 3];

            a.transparency = new byte[256];

            a.num_pens_used = 0;
            a.num_pens_trans = 0;
            int aei = 0;
            while (aei < ae.Length && ae[aei].box.min_x >= 0)
            {
                /* look if the color is already in the palette */
                pen = 0;
                while ((pen < a.num_pens_used) &&
                       ((ae[aei].red != a.orig_palette[3 * pen]) ||
                        (ae[aei].green != a.orig_palette[3 * pen + 1]) ||
                        (ae[aei].blue != a.orig_palette[3 * pen + 2]) ||
                        ((ae[aei].alpha < 255) && (ae[aei].alpha != a.transparency[pen]))))
                    pen++;

                if (pen == a.num_pens_used)
                {
                    a.orig_palette[3 * pen] = ae[aei].red;
                    a.orig_palette[3 * pen + 1] = ae[aei].green;
                    a.orig_palette[3 * pen + 2] = ae[aei].blue;
                    a.num_pens_used++;
                    if (ae[aei].alpha < 255)
                    {
                        a.transparency[pen] = ae[aei].alpha;
                        a.num_pens_trans++;
                    }
                }

                if (ae[aei].box.max_y == -1) /* circle */
                {
                    int r = ae[aei].box.max_x;

                    if ((circle = create_circle(r, pen)) != null)
                    {
                        copybitmap(a.orig_artwork, circle, false, false,
                                   ae[aei].box.min_x - r,
                                   ae[aei].box.min_y - r,
                                   null, TRANSPARENCY_PEN, 255);
                        osd_free_bitmap(circle);
                    }
                }
                else
                    fillbitmap(a.orig_artwork, pen, ae[aei].box);
                aei++;
            }

            /* Make sure we don't have too many colors */
            if (a.num_pens_used > max_pens)
            {
                printf("Too many colors in overlay.\n");
                printf("Colors found: %d  Max Allowed: %d\n", a.num_pens_used, max_pens);
                artwork_free(ref a);
                return null;
            }

            /* If the game uses dynamic colors, we assume that it's safe
               to init the palette and remap the colors now */
            if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
                backdrop_set_palette(a, a.orig_palette);

            return a;
        }
        public static void artwork_free(ref artwork a)
        {

            if (a!=null)
            {
                if (a._artwork!=null)
                    osd_free_bitmap(a._artwork);
                if (a.orig_artwork!=null)
                    osd_free_bitmap(a.orig_artwork);
                if (a.vector_bitmap!=null)
                    osd_free_bitmap(a.vector_bitmap);
                if (a.orig_palette != null)
                    a.orig_palette = null;
                if (a.transparency != null)
                    a.transparency = null;
                if (a.brightness != null)
                    a.brightness = null;
                if (a.pTable != null)
                    a.pTable = null;
                a = null;
            }
        }

        //internal static void drawgfx(osd_bitmap bitmap, GfxElement gfxElement, int p, int p_2, bool p_3, bool p_4, int sx, int sy, rectangle rectangle, int p_5, int p_6)
        //{
        //    throw new NotImplementedException();
        //}
        public static void backdrop_refresh_tables(artwork a)
        {
            throw new Exception();

        }
        public static artwork artwork_load(string filename, int start_pen, int max_pens)
        {
            return artwork_load_size(filename, start_pen, max_pens, Machine.scrbitmap.width, Machine.scrbitmap.height);
        }
        static int artwork_read_bitmap(string file_name, ref osd_bitmap bitmap, ref png_info p)
        {

            uint orientation;
            uint x, y;
            object fp;

            if ((fp = osd_fopen(Machine.gamedrv.name, file_name, OSD_FILETYPE_ARTWORK, 0)) == null)
            {
                printf("Unable to open PNG %s\n", file_name);
                return 0;
            }

            if (png_read_file(fp, p) == 0)
            {
                osd_fclose(fp);
                return 0;
            }
            osd_fclose(fp);

            if (p.bit_depth > 8)
            {
                printf("Unsupported bit depth %i (8 bit max.)\n", p.bit_depth);
                return 0;
            }

            if (p.color_type != 3)
            {
                printf("Unsupported color type %i (has to be 3)\n", p.color_type);
                return 0;
            }
            if (p.interlace_method != 0)
            {
                printf("Interlace unsupported\n");
                return 0;
            }

            /* Convert to 8 bit */
            png_expand_buffer_8bit(p);

            png_delete_unused_colors(p);

            if ((bitmap = osd_create_bitmap((int)p.width, (int)p.height)) == null)
            {
                printf("Unable to allocate memory for artwork\n");
                return 0;
            }

            orientation = (uint)Machine.orientation;

            int tmp = 0;//p.image;
            for (y = 0; y < p.height; y++)
                for (x = 0; x < p.width; x++)
                {
                    plot_pixel(bitmap, (int)x, (int)y, p.image[tmp++]);
                }

            p.image = null;
            return 1;
        }

        public static artwork artwork_load_size(string filename, int start_pen, int max_pens, int width, int height)
        {
            osd_bitmap picture = null;
            artwork a = null;
            png_info p=new png_info();

            /* If the user turned artwork off, bail */
            if (!options.use_artwork) return null;

            if ((a = allocate_artwork_mem(width, height)) == null)
                return null;

            a.start_pen = start_pen;

            if (artwork_read_bitmap(filename, ref picture, ref p) == 0)
            {
                artwork_free(ref a);
                return null;
            }

            a.num_pens_used = (int)p.num_palette;
            a.num_pens_trans = (int)p.num_trans;
            a.orig_palette = p.palette;
            a.transparency = p.trans;

            /* Make sure we don't have too many colors */
            if (a.num_pens_used > max_pens)
            {
                printf("Too many colors in overlay.\n");
                printf("Colors found: %d  Max Allowed: %d\n", a.num_pens_used, max_pens);
                artwork_free(ref a);
                osd_free_bitmap(picture);
                return null;
            }

            /* Scale the original picture to be the same size as the visible area */
            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                copybitmapzoom(a.orig_artwork, picture, false, false, 0, 0,
                           null, TRANSPARENCY_NONE, 0,
                           (a.orig_artwork.height << 16) / picture.height,
                           (a.orig_artwork.width << 16) / picture.width);
            else
                copybitmapzoom(a.orig_artwork, picture, false, false, 0, 0,
                           null, TRANSPARENCY_NONE, 0,
                           (a.orig_artwork.width << 16) / picture.width,
                           (a.orig_artwork.height << 16) / picture.height);

            /* If the game uses dynamic colors, we assume that it's safe
               to init the palette and remap the colors now */
            if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
                backdrop_set_palette(a, a.orig_palette);

            /* We don't need the original any more */
            osd_free_bitmap(picture);

            return a;
        }
        public static int overlay_set_palette(artwork a, byte[] palette, int num_shades)
{
    throw new Exception();
}
        public static void artwork_elements_scale(artwork_element ae, int width, int height)
{
            throw new Exception();
        }

    }
}
