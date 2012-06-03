using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public  const int MAX_GFX_PLANES = 8;
        public  const int MAX_GFX_SIZE = 64;
        public  const int TRANSPARENCY_NONE = 0;
        public  const int TRANSPARENCY_PEN = 1;
        public  const int TRANSPARENCY_PENS = 4;
        public  const int TRANSPARENCY_COLOR = 2;
        public  const int TRANSPARENCY_THROUGH = 3;
        public  const int TRANSPARENCY_PEN_TABLE = 5;

        public const byte DRAWMODE_NONE = 0;
        public const byte DRAWMODE_SOURCE = 1;
        public const byte DRAWMODE_SHADOW = 2;
        public const byte DRAWMODE_HIGHLIGHT = 3;

        public static byte[] gfx_drawmode_table = new byte[256];
#if WINDOWS
         const int BL0 = 0, BL1 = 1, BL2 = 2, BL3 = 3, WL0 = 0, WL1 = 1;
#else
         const int BL0=3,BL1=2,BL2=1,BL3=0,WL0=1,WL1=0;
#endif

         public class GfxLayout
        {
            public GfxLayout() { }
            public GfxLayout(ushort width, ushort height, uint total, ushort planes, uint[] planeoffset, uint[] xoffset, uint[] yoffset, ushort charincrement)
            {
                this.width = width;
                this.height = height;
                this.total = total;
                this.planes = planes;
                this.planeoffset = planeoffset;
                this.xoffset = xoffset;
                this.yoffset = yoffset;
                this.charincrement = charincrement;
            }
            public ushort width, height; /* width and height (in pixels) of chars/sprites */
            public uint total; /* total numer of chars/sprites in the rom */
            public ushort planes; /* number of bitplanes */
            public uint[] planeoffset = new uint[MAX_GFX_PLANES]; /* start of every bitplane (in bits) */
            public uint[] xoffset = new uint[MAX_GFX_SIZE]; /* position of the bit corresponding to the pixel */
            public uint[] yoffset = new uint[MAX_GFX_SIZE]; /* of the given coordinates */
            public ushort charincrement; /* distance between two consecutive characters/sprites (in bits) */
        }
        public class GfxElement
        {
            public int width, height;

            public uint total_elements;	/* total number of characters/sprites */
            public int color_granularity;	/* number of colors for each color code */
            /* (for example, 4 for 2 bitplanes gfx) */
            public _ShortPtr colortable;	/* map color codes to screen pens */
            /* if this is 0, the function does a verbatim copy */
            public int total_colors;
            public uint[] pen_usage;	/* an array of total_elements ints. */
            /* It is a table of the pens each character uses */
            /* (bit 0 = pen 0, and so on). This is used by */
            /* drawgfgx() to do optimizations like skipping */
            /* drawing of a totally transparent characters */
            public byte[] gfxdata;	/* pixel data */
            public int line_modulo;	/* amount to add to get to the next line (usually = width) */
            public int char_modulo;	/* = line_modulo * height */
        }
        public class GfxDecodeInfo
        {
            public GfxDecodeInfo(int memory_region, int start, GfxLayout gfxlayout, int color_codes_start, int total_color_codes)
            {
                this.memory_region = memory_region;
                this.start = start;
                this.gfxlayout = gfxlayout;
                this.color_codes_start = color_codes_start;
                this.total_color_codes = total_color_codes;
            }
            public int memory_region;
            public int start;
            public GfxLayout gfxlayout;
            public int color_codes_start;
            public int total_color_codes;
        }
        public class rectangle
        {
            public rectangle() { }
            public rectangle(int min_x, int max_x, int min_y, int max_y)
            {
                this.min_x = min_x;
                this.max_x = max_x;
                this.min_y = min_y;
                this.max_y = max_y;
            }
            public int min_x, max_x, min_y, max_y;
        }
        public delegate int read_pixel_proc(osd_bitmap bitmap, int x, int y);
        public delegate void plot_pixel_proc(osd_bitmap bitmap, int x, int y, int pen);
        public static read_pixel_proc read_pixel;
        public static plot_pixel_proc plot_pixel;

        plot_pixel_proc[] pps_16_nd = { pp_16_nd, pp_16_nd_fx, pp_16_nd_fy, pp_16_nd_fxy, pp_16_nd_s, pp_16_nd_fx_s, pp_16_nd_fy_s, pp_16_nd_fxy_s };
        plot_pixel_proc[] pps_8_nd = { pp_8_nd, pp_8_nd_fx, pp_8_nd_fy, pp_8_nd_fxy, pp_8_nd_s, pp_8_nd_fx_s, pp_8_nd_fy_s, pp_8_nd_fxy_s };
        plot_pixel_proc[] pps_8_d = { pp_8_d, pp_8_d_fx, pp_8_d_fy, pp_8_d_fxy, pp_8_d_s, pp_8_d_fx_s, pp_8_d_fy_s, pp_8_d_fxy_s };

        plot_pixel_proc[] pps_16_d;
        read_pixel_proc[] rps_8 = { rp_8, rp_8_fx, rp_8_fy, rp_8_fxy, rp_8_s, rp_8_fx_s, rp_8_fy_s, rp_8_fxy_s };
        read_pixel_proc[] rps_16 = { rp_16, rp_16_fx, rp_16_fy, rp_16_fxy, rp_16_s, rp_16_fx_s, rp_16_fy_s, rp_16_fxy_s };

        static void pp_16_nd(osd_bitmap b, int x, int y, int p) { b.line[y].write16(x, (ushort)p); }
        static void pp_16_nd_fx(osd_bitmap b, int x, int y, int p) { b.line[y].write16(b.width - 1 - x, (ushort)p); }
        static void pp_16_nd_fy(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - y].write16(x, (ushort)p); }
        static void pp_16_nd_fxy(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - y].write16(b.width - 1 - x, (ushort)p); }
        static void pp_16_nd_s(osd_bitmap b, int x, int y, int p) { b.line[x].write16(y, (ushort)p); }
        static void pp_16_nd_fx_s(osd_bitmap b, int x, int y, int p) { b.line[x].write16(b.width - 1 - y, (ushort)p); }
        static void pp_16_nd_fy_s(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - x].write16(y, (ushort)p); }
        static void pp_16_nd_fxy_s(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - x].write16(b.width - 1 - y, (ushort)p); }

        static int rp_16(osd_bitmap b, int x, int y) { return b.line[y].read16(x); }
        static int rp_16_fx(osd_bitmap b, int x, int y) { return b.line[y].read16(b.width - 1 - x); }
        static int rp_16_fy(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - y].read16(x); }
        static int rp_16_fxy(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - y].read16(b.width - 1 - x); }
        static int rp_16_s(osd_bitmap b, int x, int y) { return b.line[x].read16(y); }
        static int rp_16_fx_s(osd_bitmap b, int x, int y) { return b.line[x].read16(b.width - 1 - y); }
        static int rp_16_fy_s(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - x].read16(y); }
        static int rp_16_fxy_s(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - x].read16(b.width - 1 - y); }


        static void pp_8_nd(osd_bitmap b, int x, int y, int p) { b.line[y][x] = (byte)p; }
        static void pp_8_nd_fx(osd_bitmap b, int x, int y, int p) { b.line[y][b.width - 1 - x] = (byte)p; }
        static void pp_8_nd_fy(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - y][x] = (byte)p; }
        static void pp_8_nd_fxy(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - y][b.width - 1 - x] = (byte)p; }
        static void pp_8_nd_s(osd_bitmap b, int x, int y, int p) { b.line[x][y] = (byte)p; }
        static void pp_8_nd_fx_s(osd_bitmap b, int x, int y, int p) { b.line[x][b.width - 1 - y] = (byte)p; }
        static void pp_8_nd_fy_s(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - x][y] = (byte)p; }
        static void pp_8_nd_fxy_s(osd_bitmap b, int x, int y, int p) { b.line[b.height - 1 - x][b.width - 1 - y] = (byte)p; }

        static int rp_8(osd_bitmap b, int x, int y) { return b.line[y][x]; }
        static int rp_8_fx(osd_bitmap b, int x, int y) { return b.line[y][b.width - 1 - x]; }
        static int rp_8_fy(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - y][x]; }
        static int rp_8_fxy(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - y][b.width - 1 - x]; }
        static int rp_8_s(osd_bitmap b, int x, int y) { return b.line[x][y]; }
        static int rp_8_fx_s(osd_bitmap b, int x, int y) { return b.line[x][b.width - 1 - y]; }
        static int rp_8_fy_s(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - x][y]; }
        static int rp_8_fxy_s(osd_bitmap b, int x, int y) { return b.line[b.height - 1 - x][b.width - 1 - y]; }

        static void pp_8_d(osd_bitmap b, int x, int y, int p) { b.line[y][x] = (byte)p; osd_mark_dirty(x, y, x, y, 0); }
        static void pp_8_d_fx(osd_bitmap b, int x, int y, int p) { int newx = b.width - 1 - x; b.line[y][newx] = (byte)p; osd_mark_dirty(newx, y, newx, y, 0); }
        static void pp_8_d_fy(osd_bitmap b, int x, int y, int p) { int newy = b.height - 1 - y; b.line[newy][x] = (byte)p; osd_mark_dirty(x, newy, x, newy, 0); }
        static void pp_8_d_fxy(osd_bitmap b, int x, int y, int p) { int newx = b.width - 1 - x; int newy = b.height - 1 - y; b.line[newy][newx] = (byte)p; osd_mark_dirty(newx, newy, newx, newy, 0); }
        static void pp_8_d_s(osd_bitmap b, int x, int y, int p) { b.line[x][y] = (byte)p; osd_mark_dirty(y, x, y, x, 0); }
        static void pp_8_d_fx_s(osd_bitmap b, int x, int y, int p) { int newy = b.width - 1 - y; b.line[x][newy] = (byte)p; osd_mark_dirty(newy, x, newy, x, 0); }
        static void pp_8_d_fy_s(osd_bitmap b, int x, int y, int p) { int newx = b.height - 1 - x; b.line[newx][y] = (byte)p; osd_mark_dirty(y, newx, y, newx, 0); }
        static void pp_8_d_fxy_s(osd_bitmap b, int x, int y, int p) { int newx = b.height - 1 - x; int newy = b.width - 1 - y; b.line[newx][newy] = (byte)p; osd_mark_dirty(newy, newx, newy, newx, 0); }

        static bool IS_FRAC(uint offset) { return (offset & 0x80000000) != 0; }
        static uint FRAC_NUM(uint offset) { return (((offset) >> 27) & 0x0f); }
        static uint FRAC_DEN(uint offset) { return (((offset) >> 23) & 0x0f); }
        static uint FRAC_OFFSET(uint offset) { return ((offset) & 0x007fffff); }
        void set_pixel_functions()
        {
            if (Machine.color_depth == 8)
            {
                read_pixel = rps_8[Machine.orientation];

                if ((Machine.drv.video_attributes & VIDEO_SUPPORTS_DIRTY) != 0)
                    plot_pixel = pps_8_d[Machine.orientation];
                else
                    plot_pixel = pps_8_nd[Machine.orientation];
            }
            else
            {
                read_pixel = rps_16[Machine.orientation];

                if ((Machine.drv.video_attributes & VIDEO_SUPPORTS_DIRTY) != 0)
                    plot_pixel = pps_16_d[Machine.orientation];
                else
                    plot_pixel = pps_16_nd[Machine.orientation];
            }
        }
        public static uint RGN_FRAC(int num, int den)
        {
            return (uint)(0x80000000 | (((num) & 0x0f) << 27) | (((den) & 0x0f) << 23));
        }
        static int readbit(_BytePtr src, int bitnum)
        {
            return (src[bitnum / 8] >> (7 - bitnum % 8)) & 1;
        }

        public static void decodechar(GfxElement gfx, int num, _BytePtr src, GfxLayout gl)
        {
            int plane, x, y;
            _BytePtr dp;
            int offs;


            offs = num * gl.charincrement;
            dp = new _BytePtr(gfx.gfxdata, (num * gfx.char_modulo));
            for (y = 0; y < gfx.height; y++)
            {
                int yoffs;

                yoffs = y;
#if PREROTATE_GFX
		if ((Machine.orientation & ORIENTATION_FLIP_Y)!=0)
			yoffs = gfx.height-1 - yoffs;
#endif

                for (x = 0; x < gfx.width; x++)
                {
                    int xoffs;

                    xoffs = x;
#if PREROTATE_GFX
			if ((Machine.orientation & ORIENTATION_FLIP_X)!=0)
				xoffs = gfx.width-1 - xoffs;
#endif

                    dp[x] = 0;
                    if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                    {
                        for (plane = 0; plane < gl.planes; plane++)
                        {
                            if (readbit(src, (int)(offs + gl.planeoffset[plane] + gl.yoffset[xoffs] + gl.xoffset[yoffs])) != 0)
                                dp[x] |= (byte)((1 << (gl.planes - 1 - plane)));
                        }
                    }
                    else
                    {
                        for (plane = 0; plane < gl.planes; plane++)
                        {
                            if (readbit(src, (int)(offs + gl.planeoffset[plane] + gl.yoffset[yoffs] + gl.xoffset[xoffs])) != 0)
                                dp[x] |= (byte)(1 << (gl.planes - 1 - plane));
                        }
                    }
                }
                dp.offset += gfx.line_modulo;
            }


            if (gfx.pen_usage != null)
            {
                /* fill the pen_usage array with info on the used pens */
                gfx.pen_usage[num] = 0;

                dp = new _BytePtr(gfx.gfxdata, (num * gfx.char_modulo));
                for (y = 0; y < gfx.height; y++)
                {
                    for (x = 0; x < gfx.width; x++)
                    {
                        gfx.pen_usage[num]= (uint)(gfx.pen_usage[num] | (1 << dp[x]));
                    }
                    dp.offset += gfx.line_modulo;
                }
            }
        }

        public static void drawgfx(osd_bitmap dest, GfxElement gfx, uint code, uint color, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            rectangle myclip = new rectangle();

            if (gfx == null)
            {
                usrintf_showmessage("drawgfx() gfx == 0");
                return;
            }
            if (gfx.colortable == null)
            {
                usrintf_showmessage("drawgfx() gfx.colortable == 0");
                return;
            }

            code %= gfx.total_elements;
            color %= (uint)gfx.total_colors;

            if (gfx.pen_usage != null && (transparency == TRANSPARENCY_PEN || transparency == TRANSPARENCY_PENS))
            {
                int transmask = 0;

                if (transparency == TRANSPARENCY_PEN)
                {
                    transmask = 1 << transparent_color;
                }
                else if (transparency == TRANSPARENCY_PENS)
                {
                    transmask = transparent_color;
                }

                if ((gfx.pen_usage[(int)code] & ~transmask) == 0)
                    /* character is totally transparent, no need to draw */
                    return;
                else if ((gfx.pen_usage[(int)code] & transmask) == 0 && transparency != TRANSPARENCY_THROUGH && transparency != TRANSPARENCY_PEN_TABLE)
                    /* character is totally opaque, can disable transparency */
                    transparency = TRANSPARENCY_NONE;
            }

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                int temp;

                temp = sx;
                sx = sy;
                sy = temp;

                bool tempb = flipx;
                flipx = flipy;
                flipy = tempb;

                if (clip != null)
                {
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_x;
                    myclip.min_x = clip.min_y;
                    myclip.min_y = temp;
                    temp = clip.max_x;
                    myclip.max_x = clip.max_y;
                    myclip.max_y = temp;
                    clip = myclip;
                }
            }
            if ((Machine.orientation & ORIENTATION_FLIP_X) != 0)
            {
                sx = dest.width - gfx.width - sx;
                if (clip != null)
                {
                    int temp;


                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_x;
                    myclip.min_x = dest.width - 1 - clip.max_x;
                    myclip.max_x = dest.width - 1 - temp;
                    myclip.min_y = clip.min_y;
                    myclip.max_y = clip.max_y;
                    clip = myclip;
                }
#if !PREROTATE_GFX
                flipx = !flipx;
#endif
            }
            if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0)
            {
                sy = dest.height - gfx.height - sy;
                if (clip != null)
                {
                    int temp;


                    myclip.min_x = clip.min_x;
                    myclip.max_x = clip.max_x;
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_y;
                    myclip.min_y = dest.height - 1 - clip.max_y;
                    myclip.max_y = dest.height - 1 - temp;
                    clip = myclip;
                }
#if !PREROTATE_GFX
                flipy = !flipy;
#endif
            }

            if (dest.depth != 16)
                drawgfx_core8(dest, gfx, code, color, flipx, flipy, sx, sy, clip, transparency, transparent_color);
            else
                drawgfx_core16(dest, gfx, code, color, flipx, flipy, sx, sy, clip, transparency, transparent_color);

        }

        public static GfxElement decodegfx(_BytePtr src, GfxLayout gl)
        {
            int c;
            GfxElement gfx;


            if ((gfx = new GfxElement()) == null)
                return null;

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                gfx.width = gl.height;
                gfx.height = gl.width;
            }
            else
            {
                gfx.width = gl.width;
                gfx.height = gl.height;
            }

            gfx.line_modulo = gfx.width;
            gfx.char_modulo = gfx.line_modulo * gfx.height;
            if ((gfx.gfxdata = new byte[gl.total * gfx.char_modulo]) == null)
            {
                gfx = null;
                return null;
            }

            gfx.total_elements = gl.total;
            gfx.color_granularity = 1 << gl.planes;

            gfx.pen_usage = null; /* need to make sure this is NULL if the next test fails) */
            if (gfx.color_granularity <= 32)	/* can't handle more than 32 pens */
                gfx.pen_usage = new uint[gfx.total_elements];
            /* no need to check for failure, the code can work without pen_usage */

            for (c = 0; c < gl.total; c++)
                decodechar(gfx, c, src, gl);

            return gfx;
        }
        public static void plot_pixel2(osd_bitmap bitmap1, osd_bitmap bitmap2, int x, int y, int pen)
        {
            plot_pixel(bitmap1, x, y, pen);
            plot_pixel(bitmap2, x, y, pen);
        }

        static void drawgfx_core8(osd_bitmap dest, GfxElement gfx, uint code, uint color, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            /* check bounds */
            int ox = sx;
            int oy = sy;

            int ex = sx + gfx.width - 1;
            if (sx < 0) sx = 0;
            if (clip != null && sx < clip.min_x) sx = clip.min_x;
            if (ex >= dest.width) ex = dest.width - 1;
            if (clip != null && ex > clip.max_x) ex = clip.max_x;
            if (sx > ex) return;

            int ey = sy + gfx.height - 1;
            if (sy < 0) sy = 0;
            if (clip != null && sy < clip.min_y) sy = clip.min_y;
            if (ey >= dest.height) ey = dest.height - 1;
            if (clip != null && ey > clip.max_y) ey = clip.max_y;
            if (sy > ey) return;

            osd_mark_dirty(sx, sy, ex, ey, 0);	/* ASG 971011 */

            {
                _BytePtr sd = new _BytePtr(gfx.gfxdata, (int)(code * gfx.char_modulo));		/* source data */
                int sw = ex - sx + 1;										/* source width */
                int sh = ey - sy + 1;										/* source height */
                int sm = gfx.line_modulo;								/* source modulo */
                _BytePtr dd = new _BytePtr(dest.line[sy], sx);		/* dest data */
                int dm = (int)(dest.line[1].offset - dest.line[0].offset);	/* dest modulo */
                _ShortPtr paldata = new _ShortPtr(gfx.colortable, (int)(gfx.color_granularity * color) * sizeof(short));

                if (flipx)
                {
                    //if ((sx-ox) == 0) sd += gfx.width - sw;
                    sd.offset += (gfx.width - 1 - (sx - ox));
                }
                else
                    sd.offset += (sx - ox);

                if (flipy)
                {
                    sd.offset += (sm * (gfx.height - 1 - (sy - oy)));
                    sm = -sm;
                }
                else
                    sd.offset += (sm * (sy - oy));

                switch (transparency)
                {
                    case TRANSPARENCY_NONE:
                        if (flipx)
                            blockmove_opaque_flipx8(sd, sw, sh, sm, dd, dm, paldata);
                        else
                            blockmove_opaque8(sd, sw, sh, sm, dd, dm, paldata);
                        break;
                    case TRANSPARENCY_PEN:
                        if (flipx)
                            blockmove_transpen_flipx8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transpen8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_PENS:
                        if (flipx)
                            blockmove_transmask_flipx8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transmask8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_COLOR:
                        if (flipx)
                            blockmove_transcolor_flipx8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transcolor8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_THROUGH:
                        if (flipx)
                            blockmove_transthrough_flipx8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transthrough8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_PEN_TABLE:
                        if (flipx)
                            blockmove_pen_table_flipx8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else blockmove_pen_table8(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                }
            }
        }
        static void blockmove_opaque8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata)
        {
            int end;
            srcmodulo -= srcwidth;
            dstmodulo -= srcwidth;
            while (srcheight != 0)
            {
                end = (int)(dstdata.offset + srcwidth);
                while (dstdata.offset <= end - 8)
                {
                    dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset]);
                    dstdata.buffer[dstdata.offset + 1] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 1]);
                    dstdata.buffer[dstdata.offset + 2] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 2]);
                    dstdata.buffer[dstdata.offset + 3] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 3]);
                    dstdata.buffer[dstdata.offset + 4] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 4]);
                    dstdata.buffer[dstdata.offset + 5] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 5]);
                    dstdata.buffer[dstdata.offset + 6] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 6]);
                    dstdata.buffer[dstdata.offset + 7] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 7]);
                    dstdata.offset += 8; srcdata.offset += 8;
                }
                while (dstdata.offset < end)
                {
                    dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset]);
                    dstdata.offset++;
                    srcdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_opaque_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata)
        {
            int end;
            srcmodulo += srcwidth;
            dstmodulo -= srcwidth; //srcdata += srcwidth-1;
            while (srcheight != 0)
            {
                end = (int)(dstdata.offset + srcwidth);
                while (dstdata.offset <= end - 8)
                {
                    srcdata.offset = ((int)srcdata.offset - 8);
                    dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 8]);
                    dstdata.buffer[dstdata.offset + 1] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 7]);
                    dstdata.buffer[dstdata.offset + 2] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 6]);
                    dstdata.buffer[dstdata.offset + 3] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 5]);
                    dstdata.buffer[dstdata.offset + 4] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 4]);
                    dstdata.buffer[dstdata.offset + 5] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 3]);
                    dstdata.buffer[dstdata.offset + 6] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 2]);
                    dstdata.buffer[dstdata.offset + 7] = (byte)paldata.read16(srcdata.buffer[srcdata.offset + 1]);
                    dstdata.offset += 8;
                }
                while (dstdata.offset < end)
                {
                    dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset]);
                    dstdata.offset++;
                    srcdata.offset--;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transpen8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transpen)
        {
            int end;
            int trans4;
            _IntPtr sd4;
            srcmodulo -= srcwidth;
            dstmodulo -= srcwidth;
            trans4 = transpen * 0x01010101;
            while (srcheight != 0)
            {
                end = (int)(dstdata.offset + srcwidth);
                while ((srcdata.offset & 3) != 0 && dstdata.offset < end) /* longword align */
                {
                    int col = srcdata[0];
                    srcdata.offset++;
                    if (col != transpen)
                        dstdata[0] = (byte)paldata.read16(col);
                    dstdata.offset++;
                }
                sd4 = new _IntPtr(srcdata);
                while (dstdata.offset <= end - 4)
                {
                    uint col4;
                    if ((col4 = sd4.read32(0)) != trans4)
                    {
                        uint xod4 = (uint)(col4 ^ trans4);
                        if ((xod4 & 0x000000ff) != 0)
                            dstdata[BL0] = (byte)paldata.read16((int)(col4 & 0xff));
                        if ((xod4 & 0x0000ff00) != 0)
                            dstdata[BL1] = (byte)paldata.read16((int)((col4 >> 8) & 0xff));
                        if ((xod4 & 0x00ff0000) != 0)
                            dstdata[BL2] = (byte)paldata.read16((int)((col4 >> 16) & 0xff));
                        if ((xod4 & 0xff000000) != 0)
                            dstdata[BL3] = (byte)paldata.read16((int)(col4 >> 24));
                    }
                    sd4.offset += 4;
                    dstdata.offset += 4;
                }
                srcdata = new _BytePtr(sd4);
                while (dstdata.offset < end)
                {
                    int col = srcdata[0];
                    srcdata.offset++;
                    if (col != transpen)
                        dstdata[0] = (byte)paldata.read16(col);
                    dstdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }

        static void blockmove_transpen_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transpen)
        {
            int end;
            _IntPtr sd4 = new _IntPtr(srcdata);
            srcmodulo += srcwidth;
            dstmodulo -= srcwidth;
            srcdata.offset -= 3;
            int trans4 = transpen * 0x01010101;
            while (srcheight != 0)
            {
                end = dstdata.offset + srcwidth;
                while ((srcdata.offset & 3) != 0 && dstdata.offset < end) /* longword align */
                {
                    int col = srcdata[3];
                    srcdata.offset--;
                    if (col != transpen)
                        dstdata[0] = (byte)paldata.read16(col);
                    dstdata.offset++;
                }
                sd4.offset = srcdata.offset;// = new _IntPtr(srcdata);
                while (dstdata.offset <= end - 4)
                {
                    uint col4;
                    if ((col4 = sd4.read32(0)) != trans4)
                    {
                        uint xod4 = (uint)(col4 ^ trans4);
                        if ((xod4 & 0xff000000) != 0)
                            dstdata[BL0] = (byte)paldata.read16((int)col4 >> 24);
                        if ((xod4 & 0x00ff0000) != 0)
                            dstdata[BL1] = (byte)paldata.read16((int)(col4 >> 16) & 0xff);
                        if ((xod4 & 0x0000ff00) != 0)
                            dstdata[BL2] = (byte)paldata.read16((int)(col4 >> 8) & 0xff);
                        if ((xod4 & 0x000000ff) != 0)
                            dstdata[BL3] = (byte)paldata.read16((int)col4 & 0xff);
                    }
                    sd4.offset -= 4;
                    dstdata.offset += 4;
                }
                //srcdata = new _BytePtr(sd4);
                srcdata.offset = sd4.offset;
                while (dstdata.offset < end)
                {
                    int col = srcdata[3];
                    srcdata.offset--;
                    if (col != transpen)
                        dstdata[0] = (byte)paldata.read16(col);
                    dstdata.offset++;

                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transmask8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transmask)
        {
            throw new Exception();
        }
        static void blockmove_transmask_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transmask)
        {
            throw new Exception();
        }
        static void blockmove_transcolor8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            int end;
            _ShortPtr lookupdata = new _ShortPtr(Machine.game_colortable, (int)(paldata.offset - Machine.remapped_colortable.offset));
            srcmodulo -= srcwidth;
            dstmodulo -= srcwidth;
            while (srcheight != 0)
            {
                end = (int)(dstdata.offset + srcwidth);
                while (dstdata.offset < end)
                {
                    if (lookupdata.read16(srcdata.buffer[srcdata.offset]) != transcolor)
                        dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset]);
                    srcdata.offset++;
                    dstdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transcolor_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            int end;
            _ShortPtr lookupdata = new _ShortPtr(Machine.game_colortable, (int)(paldata.offset - Machine.remapped_colortable.offset));
            srcmodulo += srcwidth;
            dstmodulo -= srcwidth; //srcdata += srcwidth-1; 
            while (srcheight != 0)
            {
                end = (int)(dstdata.offset + srcwidth);
                while (dstdata.offset < end)
                {
                    if (lookupdata.read16(srcdata.buffer[srcdata.offset]) != transcolor)
                        dstdata.buffer[dstdata.offset] = (byte)paldata.read16(srcdata.buffer[srcdata.offset]);
                    srcdata.offset--;
                    dstdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transthrough8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            int end;
            srcmodulo -= srcwidth;
            dstmodulo -= srcwidth;
            while (srcheight != 0)
            {
                end = dstdata.offset + srcwidth;
                while (dstdata.offset < end)
                {
                    if (dstdata[0] == transcolor)
                        dstdata[0] = (byte)paldata.read16(srcdata[0]);
                    srcdata.offset++;
                    dstdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transthrough_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            int end;
            srcmodulo += srcwidth;
            dstmodulo -= srcwidth;
            //srcdata += srcwidth-1; 
            while (srcheight != 0)
            {
                end = dstdata.offset + srcwidth;
                while (dstdata.offset < end)
                {
                    if (dstdata[0] == transcolor)
                        dstdata[0] = (byte)paldata.read16(srcdata[0]);
                    srcdata.offset--;
                    dstdata.offset++;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_pen_table8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            throw new Exception();
        }
        static void blockmove_pen_table_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor)
        {
            throw new Exception();
        }
        static void drawgfx_core16(osd_bitmap dest, GfxElement gfx, uint code, uint color, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            int ox; int oy; int ex; int ey;
            /* check bounds */
            ox = sx; oy = sy;
            ex = sx + gfx.width - 1; if (sx < 0) sx = 0;
            if (clip != null && sx < clip.min_x) sx = clip.min_x;
            if (ex >= dest.width) ex = dest.width - 1;
            if (clip != null && ex > clip.max_x) ex = clip.max_x;
            if (sx > ex) return; ey = sy + gfx.height - 1;
            if (sy < 0) sy = 0;
            if (clip != null && sy < clip.min_y) sy = clip.min_y;
            if (ey >= dest.height) ey = dest.height - 1;
            if (clip != null && ey > clip.max_y) ey = clip.max_y;
            if (sy > ey) return; osd_mark_dirty(sx, sy, ex, ey, 0); /* ASG 971011 */
            {
                _BytePtr sd = new _BytePtr(gfx.gfxdata, (int)(code * gfx.char_modulo)); /* source data */
                int sw = ex - sx + 1; /* source width */ 
                int sh = ey - sy + 1; /* source height */
                int sm = gfx.line_modulo; /* source modulo */
                _ShortPtr dd = new _ShortPtr(dest.line[sy], sx ); /* dest data */
                int dm = dest.line[1].offset - dest.line[0].offset; /* dest modulo */
                _ShortPtr paldata = new _ShortPtr(gfx.colortable, (int)((gfx.color_granularity * color) * 2));
                if (flipx)
                {
                    sd.offset += gfx.width - 1 - (sx - ox);
                }
                else
                    sd.offset += (sx - ox); if (flipy)
                {
                    sd.offset += sm * (gfx.height - 1 - (sy - oy)); 
                    sm = -sm;
                }
                else
                    sd.offset += sm * (sy - oy); switch (transparency)
                {
                    case TRANSPARENCY_NONE:
                        if (flipx) blockmove_opaque_flipx16(sd, sw, sh, sm, dd, dm, paldata);
                        else
                            blockmove_opaque16(sd, sw, sh, sm, dd, dm, paldata);
                        break;
                    case TRANSPARENCY_PEN:
                        if (flipx)
                            blockmove_transpen_flipx16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transpen16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_PENS: if (flipx)
                            blockmove_transmask_flipx16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transmask16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_COLOR: if (flipx)
                            blockmove_transcolor_flipx16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transcolor16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_THROUGH: if (flipx)
                            blockmove_transthrough_flipx16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_transthrough16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                    case TRANSPARENCY_PEN_TABLE: if (flipx)
                            blockmove_pen_table_flipx16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        else
                            blockmove_pen_table16(sd, sw, sh, sm, dd, dm, paldata, transparent_color);
                        break;
                }
            }
        }
         static void blockmove_opaque16 ( _BytePtr srcdata,int srcwidth,int srcheight,int srcmodulo, _ShortPtr dstdata,int dstmodulo, _ShortPtr paldata) 
         {
             //blockmove_opaque8(srcdata, srcwidth, srcheight, srcmodulo, (_BytePtr)dstdata, dstmodulo, paldata);
             //return;
             int end; 
             srcmodulo -= srcwidth;
             dstmodulo -= srcwidth;
             while (srcheight!=0)
             { 
                 end = dstdata.offset + srcwidth; 
                 while (dstdata.offset <= end - 8)
                 {
                     dstdata.write16(0,paldata.read16(srcdata[0])); 
                     dstdata.write16(1,paldata.read16(srcdata[1])); 
                     dstdata.write16(2,paldata.read16(srcdata[2])); 
                     dstdata.write16(3,paldata.read16(srcdata[3])); 
                     dstdata.write16(4,paldata.read16(srcdata[4])); 
                     dstdata.write16(5,paldata.read16(srcdata[5]));
                     dstdata.write16(6,paldata.read16(srcdata[6]));
                     dstdata.write16(7,paldata.read16(srcdata[7]));
                     dstdata.offset += 8 * 2;
                     srcdata.offset++;
                 } 
                 while (dstdata.offset < end) 
                 {
                     dstdata.write16(0,paldata.read16(srcdata[0]));
                     srcdata.offset++;
                     dstdata.offset += 2;
                 }
                 srcdata.inc(srcmodulo); 
                 dstdata.inc(dstmodulo); 
                 srcheight--;
             } 
             
         }
 static void blockmove_opaque_flipx16 ( _BytePtr srcdata,int srcwidth,int srcheight,int srcmodulo, _ShortPtr dstdata,int dstmodulo, _ShortPtr paldata) {throw new Exception();}
 static void blockmove_transpen16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transpen) { throw new Exception(); }
 static void blockmove_transpen_flipx16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transpen) { throw new Exception(); }
 static void blockmove_transmask16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transmask) { throw new Exception(); }
 static void blockmove_transmask_flipx16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transmask) { throw new Exception(); }
 static void blockmove_transcolor16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_transcolor_flipx16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_transthrough16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_transthrough_flipx16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_pen_table16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_pen_table_flipx16(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, _ShortPtr paldata, int transcolor) { throw new Exception(); }
 static void blockmove_opaque_noremap16(_ShortPtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo) { throw new Exception(); }
 static void blockmove_opaque_noremap_flipx16(_ShortPtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo) { throw new Exception(); }
 static void blockmove_transthrough_noremap16(_ShortPtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, int transcolor) { throw new Exception(); }
 static void blockmove_transthrough_noremap_flipx16(_ShortPtr srcdata, int srcwidth, int srcheight, int srcmodulo, _ShortPtr dstdata, int dstmodulo, int transcolor) { throw new Exception(); }


        public static void copybitmap(osd_bitmap dest, osd_bitmap src, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            rectangle myclip = new rectangle();

            /* if necessary, remap the transparent color */
            if (transparency == TRANSPARENCY_COLOR)
                transparent_color = Machine.pens.read16(transparent_color);

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                int temp = sx;
                sx = sy;
                sy = temp;

                bool tb = flipx;
                flipx = flipy;
                flipy = tb;

                if (clip != null)
                {
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_x;
                    myclip.min_x = clip.min_y;
                    myclip.min_y = temp;
                    temp = clip.max_x;
                    myclip.max_x = clip.max_y;
                    myclip.max_y = temp;
                    clip = myclip;
                }
            }
            if ((Machine.orientation & ORIENTATION_FLIP_X) != 0)
            {
                sx = dest.width - src.width - sx;
                if (clip != null)
                {
                    /* clip and myclip might be the same, so we need a temporary storage */
                    int temp = clip.min_x;
                    myclip.min_x = dest.width - 1 - clip.max_x;
                    myclip.max_x = dest.width - 1 - temp;
                    myclip.min_y = clip.min_y;
                    myclip.max_y = clip.max_y;
                    clip = myclip;
                }
            }
            if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0)
            {
                sy = dest.height - src.height - sy;
                if (clip != null)
                {
                    int temp;


                    myclip.min_x = clip.min_x;
                    myclip.max_x = clip.max_x;
                    /* clip and myclip might be the same, so we need a temporary storage */
                    temp = clip.min_y;
                    myclip.min_y = dest.height - 1 - clip.max_y;
                    myclip.max_y = dest.height - 1 - temp;
                    clip = myclip;
                }
            }

            if (dest.depth != 16)
                copybitmap_core8(dest, src, flipx, flipy, sx, sy, clip, transparency, transparent_color);
            else
                copybitmap_core16(dest, src, flipx, flipy, sx, sy, clip, transparency, transparent_color);
        }
        static void copybitmap_core8(osd_bitmap dest, osd_bitmap src, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            int ox; int oy; int ex; int ey; /* check bounds */
            ox = sx; 
            oy = sy; 
            ex = sx + src.width - 1;
            if (sx < 0) sx = 0;
            if (clip != null && sx < clip.min_x) sx = clip.min_x;
            if (ex >= dest.width) ex = dest.width - 1;
            if (clip != null && ex > clip.max_x) ex = clip.max_x;
            if (sx > ex) return;
            ey = sy + src.height - 1; if (sy < 0) sy = 0;
            if (clip != null && sy < clip.min_y) sy = clip.min_y;
            if (ey >= dest.height) ey = dest.height - 1;
            if (clip != null && ey > clip.max_y) ey = clip.max_y;
            if (sy > ey) return;
            
                _BytePtr sd = new _BytePtr(src.line[0]); /* source data */
                int sw = ex - sx + 1; /* source width */
                int sh = ey - sy + 1; /* source height */
                int sm = (int)(src.line[1].offset - src.line[0].offset); /* source modulo */
                _BytePtr dd = new _BytePtr(dest.line[sy], sx); /* dest data */
                int dm = (int)(dest.line[1].offset - dest.line[0].offset); /* dest modulo */
                if (flipx)
                { 
                    sd.offset += src.width - 1 - (sx - ox);
                }
                else
                    sd.offset += (sx - ox);
                if (flipy)
                { 
                    sd.offset += sm * (src.height - 1 - (sy - oy));
                    sm = -sm;
                }
                else
                    sd.offset += (sm * (sy - oy));

                switch (transparency)
                {
                    case TRANSPARENCY_NONE:
                        if (flipx)
                            blockmove_opaque_noremap_flipx8(sd, sw, sh, sm, dd, dm);
                        else
                            blockmove_opaque_noremap8(sd, sw, sh, sm, dd, dm); break;
                    case TRANSPARENCY_PEN:
                    case TRANSPARENCY_COLOR:
                        if (flipx)
                            blockmove_transpen_noremap_flipx8(sd, sw, sh, sm, dd, dm, transparent_color);
                        else
                            blockmove_transpen_noremap8(sd, sw, sh, sm, dd, dm, transparent_color);
                        break;
                    case TRANSPARENCY_THROUGH:
                        if (flipx)
                            blockmove_transthrough_noremap_flipx8(sd, sw, sh, sm, dd, dm, transparent_color);
                        else
                            blockmove_transthrough_noremap8(sd, sw, sh, sm, dd, dm, transparent_color);
                        break;
                }
            
        }
        static void blockmove_opaque_noremap8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo)
        {
            while (srcheight != 0)
            {
                Buffer.BlockCopy(srcdata.buffer, (int)srcdata.offset, dstdata.buffer, (int)dstdata.offset, srcwidth);
                //memcpy(dstdata,srcdata,srcwidth * sizeof(UINT8)); 
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_opaque_noremap_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo)
        {
            uint end;
            srcmodulo += srcwidth;
            dstmodulo -= srcwidth; //srcdata += srcwidth-1; 
            while (srcheight != 0)
            {
                end = (uint)(dstdata.offset + srcwidth);
                while (dstdata.offset <= end - 8)
                {
                    srcdata.offset -= 8;
                    dstdata[0] = srcdata[8];
                    dstdata[1] = srcdata[7];
                    dstdata[2] = srcdata[6];
                    dstdata[3] = srcdata[5];
                    dstdata[4] = srcdata[4];
                    dstdata[5] = srcdata[3];
                    dstdata[6] = srcdata[2];
                    dstdata[7] = srcdata[1];
                    dstdata.offset += 8;
                }
                while (dstdata.offset < end)
                {
                    dstdata[0] = srcdata[0];
                    dstdata.offset++; srcdata.offset--;
                }
                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void write_dword(_BytePtr address, int data)
{
  	if ((address.offset & 3)!=0)
	{
#if WINDOWS
    		address[0] = (byte)   data;
            address[1] = (byte)(data >> 8);
            address[2] = (byte)(data >> 16);
            address[3] = (byte)(data >> 24);
#else
    		*((unsigned char *)address+3) =  data;
    		*((unsigned char *)address+2) = (data >> 8);
    		*((unsigned char *)address+1) = (data >> 16);
    		*((unsigned char *)address)   = (data >> 24);
#endif
		return;
  	}
  	else
		address.write32(0,(uint)data);
}
        static void blockmove_transpen_noremap8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, int transpen)
        {
            int end;
            int trans4;
            _IntPtr sd4;

            srcmodulo -= srcwidth;
            dstmodulo -= srcwidth;

            trans4 = transpen * 0x01010101;

            while (srcheight != 0)
            {
                end = dstdata.offset + srcwidth;
                while ((srcdata.offset & 3) != 0 && dstdata.offset < end) /* longword align */
                {
                    int col = srcdata[0]; srcdata.offset++;
                    if (col != transpen) dstdata[0] = (byte)col;
                    dstdata.offset++;
                }
                sd4 = new _IntPtr(srcdata);
                while (dstdata.offset <= end - 4)
                {
                    uint col4;

                    if ((col4 = sd4.read32(0)) != trans4)
                    {
                        uint xod4;

                        xod4 = (uint)(col4 ^ trans4);
                        if ((xod4 & 0x000000ff) != 0 && (xod4 & 0x0000ff00) != 0 &&
                         (xod4 & 0x00ff0000) != 0 && (xod4 & 0xff000000) != 0)
                        {
                            //dstdata.write32(0, col4);
                            write_dword(dstdata, (int)col4);
                        }
                        else
                        {
                            if ((xod4 & 0xff000000) != 0) dstdata[BL3] = (byte)(col4 >> 24);
                            if ((xod4 & 0x00ff0000) != 0) dstdata[BL2] = (byte)(col4 >> 16);
                            if ((xod4 & 0x0000ff00) != 0) dstdata[BL1] = (byte)(col4 >> 8);
                            if ((xod4 & 0x000000ff) != 0) dstdata[BL0] = (byte)(col4);
                        }
                    }
                    sd4.offset += 4;
                    dstdata.offset += 4;
                }
                srcdata = new _BytePtr(sd4);
                while (dstdata.offset < end)
                {
                    int col = srcdata[0]; srcdata.offset++;
                    if (col != transpen) dstdata[0] = (byte)col;
                    dstdata.offset++;
                }

                srcdata.offset += srcmodulo;
                dstdata.offset += dstmodulo;
                srcheight--;
            }
        }
        static void blockmove_transpen_noremap_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, int transpen)
        {
            throw new Exception();
        }
        static void blockmove_transthrough_noremap8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, int transthrough)
        {
            throw new Exception();
        }
        static void blockmove_transthrough_noremap_flipx8(_BytePtr srcdata, int srcwidth, int srcheight, int srcmodulo, _BytePtr dstdata, int dstmodulo, int transthrough)
        {
            throw new Exception();
        }
        static void copybitmap_core16(osd_bitmap dest, osd_bitmap src, bool flipx, bool flipy, int sx, int sy, rectangle clip, int transparency, int transparent_color)
        {
            throw new Exception();
        }
        public static void copyscrollbitmap(osd_bitmap dest, osd_bitmap src, int rows, int[] rowscroll, int cols, int[] colscroll, rectangle clip, int transparency, int transparent_color)
        {
            int srcwidth, srcheight, destwidth, destheight;

            if (rows == 0 && cols == 0)
            {
                copybitmap(dest, src, false, false, 0, 0, clip, transparency, transparent_color);
                return;
            }

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                srcwidth = src.height;
                srcheight = src.width;
                destwidth = dest.height;
                destheight = dest.width;
            }
            else
            {
                srcwidth = src.width;
                srcheight = src.height;
                destwidth = dest.width;
                destheight = dest.height;
            }

            if (rows == 0)
            {
                /* scrolling columns */
                int col, colwidth;
                rectangle myclip = new rectangle();

                colwidth = srcwidth / cols;

                myclip.min_y = clip.min_y;
                myclip.max_y = clip.max_y;

                col = 0;
                while (col < cols)
                {
                    int cons, scroll;


                    /* count consecutive columns scrolled by the same amount */
                    scroll = colscroll[col];
                    cons = 1;
                    while (col + cons < cols && colscroll[col + cons] == scroll)
                        cons++;

                    if (scroll < 0) scroll = srcheight - (-scroll) % srcheight;
                    else scroll %= srcheight;

                    myclip.min_x = col * colwidth;
                    if (myclip.min_x < clip.min_x) myclip.min_x = clip.min_x;
                    myclip.max_x = (col + cons) * colwidth - 1;
                    if (myclip.max_x > clip.max_x) myclip.max_x = clip.max_x;

                    copybitmap(dest, src, false, false, 0, scroll, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, 0, scroll - srcheight, myclip, transparency, transparent_color);

                    col += cons;
                }
            }
            else if (cols == 0)
            {
                /* scrolling rows */
                int row, rowheight;
                rectangle myclip = new rectangle();

                rowheight = srcheight / rows;

                myclip.min_x = clip.min_x;
                myclip.max_x = clip.max_x;

                row = 0;
                while (row < rows)
                {
                    int cons, scroll;


                    /* count consecutive rows scrolled by the same amount */
                    scroll = rowscroll[row];
                    cons = 1;
                    while (row + cons < rows && rowscroll[row + cons] == scroll)
                        cons++;

                    if (scroll < 0) scroll = srcwidth - (-scroll) % srcwidth;
                    else scroll %= srcwidth;

                    myclip.min_y = row * rowheight;
                    if (myclip.min_y < clip.min_y) myclip.min_y = clip.min_y;
                    myclip.max_y = (row + cons) * rowheight - 1;
                    if (myclip.max_y > clip.max_y) myclip.max_y = clip.max_y;

                    copybitmap(dest, src, false, false, scroll, 0, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, scroll - srcwidth, 0, myclip, transparency, transparent_color);

                    row += cons;
                }
            }
            else if (rows == 1 && cols == 1)
            {
                /* XY scrolling playfield */
                int scrollx, scrolly, sx, sy;


                if (rowscroll[0] < 0) scrollx = srcwidth - (-rowscroll[0]) % srcwidth;
                else scrollx = rowscroll[0] % srcwidth;

                if (colscroll[0] < 0) scrolly = srcheight - (-colscroll[0]) % srcheight;
                else scrolly = colscroll[0] % srcheight;

                for (sx = scrollx - srcwidth; sx < destwidth; sx += srcwidth)
                    for (sy = scrolly - srcheight; sy < destheight; sy += srcheight)
                        copybitmap(dest, src, false, false, sx, sy, clip, transparency, transparent_color);
            }
            else if (rows == 1)
            {
                /* scrolling columns + horizontal scroll */
                int col, colwidth;
                int scrollx;
                rectangle myclip = new rectangle();

                if (rowscroll[0] < 0) scrollx = srcwidth - (-rowscroll[0]) % srcwidth;
                else scrollx = rowscroll[0] % srcwidth;

                colwidth = srcwidth / cols;

                myclip.min_y = clip.min_y;
                myclip.max_y = clip.max_y;

                col = 0;
                while (col < cols)
                {
                    int cons, scroll;


                    /* count consecutive columns scrolled by the same amount */
                    scroll = colscroll[col];
                    cons = 1;
                    while (col + cons < cols && colscroll[col + cons] == scroll)
                        cons++;

                    if (scroll < 0) scroll = srcheight - (-scroll) % srcheight;
                    else scroll %= srcheight;

                    myclip.min_x = col * colwidth + scrollx;
                    if (myclip.min_x < clip.min_x) myclip.min_x = clip.min_x;
                    myclip.max_x = (col + cons) * colwidth - 1 + scrollx;
                    if (myclip.max_x > clip.max_x) myclip.max_x = clip.max_x;

                    copybitmap(dest, src, false, false, scrollx, scroll, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, scrollx, scroll - srcheight, myclip, transparency, transparent_color);

                    myclip.min_x = col * colwidth + scrollx - srcwidth;
                    if (myclip.min_x < clip.min_x) myclip.min_x = clip.min_x;
                    myclip.max_x = (col + cons) * colwidth - 1 + scrollx - srcwidth;
                    if (myclip.max_x > clip.max_x) myclip.max_x = clip.max_x;

                    copybitmap(dest, src, false, false, scrollx - srcwidth, scroll, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, scrollx - srcwidth, scroll - srcheight, myclip, transparency, transparent_color);

                    col += cons;
                }
            }
            else if (cols == 1)
            {
                /* scrolling rows + vertical scroll */
                int row, rowheight;
                int scrolly;
                rectangle myclip = new rectangle();

                if (colscroll[0] < 0) scrolly = srcheight - (-colscroll[0]) % srcheight;
                else scrolly = colscroll[0] % srcheight;

                rowheight = srcheight / rows;

                myclip.min_x = clip.min_x;
                myclip.max_x = clip.max_x;

                row = 0;
                while (row < rows)
                {
                    int cons, scroll;

                    /* count consecutive rows scrolled by the same amount */
                    scroll = rowscroll[row];
                    cons = 1;
                    while (row + cons < rows && rowscroll[row + cons] == scroll)
                        cons++;

                    if (scroll < 0) scroll = srcwidth - (-scroll) % srcwidth;
                    else scroll %= srcwidth;

                    myclip.min_y = row * rowheight + scrolly;
                    if (myclip.min_y < clip.min_y) myclip.min_y = clip.min_y;
                    myclip.max_y = (row + cons) * rowheight - 1 + scrolly;
                    if (myclip.max_y > clip.max_y) myclip.max_y = clip.max_y;

                    copybitmap(dest, src, false, false, scroll, scrolly, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, scroll - srcwidth, scrolly, myclip, transparency, transparent_color);

                    myclip.min_y = row * rowheight + scrolly - srcheight;
                    if (myclip.min_y < clip.min_y) myclip.min_y = clip.min_y;
                    myclip.max_y = (row + cons) * rowheight - 1 + scrolly - srcheight;
                    if (myclip.max_y > clip.max_y) myclip.max_y = clip.max_y;

                    copybitmap(dest, src, false, false, scroll, scrolly - srcheight, myclip, transparency, transparent_color);
                    copybitmap(dest, src, false, false, scroll - srcwidth, scrolly - srcheight, myclip, transparency, transparent_color);

                    row += cons;
                }
            }
        }
        public static void fillbitmap(osd_bitmap dest, int pen, rectangle clip)
        {
            rectangle myclip = new rectangle();

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if (clip != null)
                {
                    myclip.min_x = clip.min_y;
                    myclip.max_x = clip.max_y;
                    myclip.min_y = clip.min_x;
                    myclip.max_y = clip.max_x;
                    clip = myclip;
                }
            }
            if ((Machine.orientation & ORIENTATION_FLIP_X) != 0)
            {
                if (clip != null)
                {
                    int temp = clip.min_x;
                    myclip.min_x = dest.width - 1 - clip.max_x;
                    myclip.max_x = dest.width - 1 - temp;
                    myclip.min_y = clip.min_y;
                    myclip.max_y = clip.max_y;
                    clip = myclip;
                }
            }
            if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0)
            {
                if (clip != null)
                {
                    myclip.min_x = clip.min_x;
                    myclip.max_x = clip.max_x;
                    int temp = clip.min_y;
                    myclip.min_y = dest.height - 1 - clip.max_y;
                    myclip.max_y = dest.height - 1 - temp;
                    clip = myclip;
                }
            }

            int sx = 0;
            int ex = dest.width - 1;
            int sy = 0;
            int ey = dest.height - 1;

            if (clip != null && sx < clip.min_x) sx = clip.min_x;
            if (clip != null && ex > clip.max_x) ex = clip.max_x;
            if (sx > ex) return;
            if (clip != null && sy < clip.min_y) sy = clip.min_y;
            if (clip != null && ey > clip.max_y) ey = clip.max_y;
            if (sy > ey) return;

            osd_mark_dirty(sx, sy, ex, ey, 0);	/* ASG 971011 */

            /* ASG 980211 */
            if (dest.depth == 16)
            {
                if ((pen >> 8) == (pen & 0xff))
                {
                    for (int y = sy; y <= ey; y++)
                    {
                        for (int k = 0; k < (ex - sx + 1) * 2; k++)
                            dest.line[y][sx * 2 + k] = (byte)(pen & 0xff);
                    }
                    //memset(&dest.line[y][sx*2],pen&0xff,(ex-sx+1)*2);
                }
                else
                {
                    _ShortPtr sp = new _ShortPtr(dest.line[sy]);
                    int x;

                    for (x = sx; x <= ex; x++)
                        sp.write16(x, (ushort)pen);
                    sp.offset += sx * 2;
                    for (int y = sy + 1; y <= ey; y++)
                    {
                        Buffer.BlockCopy(sp.buffer, sp.offset, dest.line[y].buffer, sx * 2, (ex - sx + 1) * 2);
                    }
                    //memcpy(&dest.line[y][sx*2],sp,(ex-sx+1)*2);
                }
            }
            else
            {
                for (int y = sy; y <= ey; y++)
                {
                    for (int k = 0; k < ex - sx + 1; k++) dest.line[y][sx + k] = (byte)pen;
                }
            }
        }
        static void copybitmapzoom(osd_bitmap dest_bmp,osd_bitmap source_bmp,bool flipx,bool flipy,int sz,int sy,rectangle clip,int transparency,int transparent_color,int scalex,int scaley)
        {
            throw new Exception();
        }
        public static void drawgfxzoom(  osd_bitmap dest_bmp, GfxElement gfx,
		uint code,uint color,bool flipx,bool flipy,int sx,int sy,
		 rectangle clip,int transparency,int transparent_color,int scalex, int scaley)
{
	 rectangle myclip=new rectangle();


	/* only support TRANSPARENCY_PEN and TRANSPARENCY_COLOR */
	if (transparency != TRANSPARENCY_PEN && transparency != TRANSPARENCY_COLOR)
		return;

	if (transparency == TRANSPARENCY_COLOR)
		transparent_color = Machine.pens[transparent_color];


	/*
	scalex and scaley are 16.16 fixed point numbers
	1<<15 : shrink to 50%
	1<<16 : uniform scale
	1<<17 : double to 200%
	*/


	if ((Machine.orientation & ORIENTATION_SWAP_XY)!=0)
	{
		int temp;

		temp = sx;
		sx = sy;
		sy = temp;

		var tempb = flipx;
		flipx = flipy;
		flipy = tempb;

		temp = scalex;
		scalex = scaley;
		scaley = temp;

		if (clip!=null)
		{
			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_x;
			myclip.min_x = clip.min_y;
			myclip.min_y = temp;
			temp = clip.max_x;
			myclip.max_x = clip.max_y;
			myclip.max_y = temp;
			clip = myclip;
		}
	}
	if ((Machine.orientation & ORIENTATION_FLIP_X)!=0)
	{
		sx = dest_bmp.width - ((gfx.width * scalex + 0x7fff) >> 16) - sx;
		if (clip!=null)
		{
			int temp;


			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_x;
			myclip.min_x = dest_bmp.width-1 - clip.max_x;
			myclip.max_x = dest_bmp.width-1 - temp;
			myclip.min_y = clip.min_y;
			myclip.max_y = clip.max_y;
			clip = myclip;
		}
#if !PREROTATE_GFX
		flipx = !flipx;
#endif
	}
	if ((Machine.orientation & ORIENTATION_FLIP_Y)!=0)
	{
		sy = dest_bmp.height - ((gfx.height * scaley + 0x7fff) >> 16) - sy;
		if (clip!=null)
		{
			int temp;


			myclip.min_x = clip.min_x;
			myclip.max_x = clip.max_x;
			/* clip and myclip might be the same, so we need a temporary storage */
			temp = clip.min_y;
			myclip.min_y = dest_bmp.height-1 - clip.max_y;
			myclip.max_y = dest_bmp.height-1 - temp;
			clip =myclip;
		}
#if !PREROTATE_GFX
		flipy = !flipy;
#endif
	}

	/* KW 991012 -- Added code to force clip to bitmap boundary */
	if(clip!=null)
	{
		myclip.min_x = clip.min_x;
		myclip.max_x = clip.max_x;
		myclip.min_y = clip.min_y;
		myclip.max_y = clip.max_y;

		if (myclip.min_x < 0) myclip.min_x = 0;
		if (myclip.max_x >= dest_bmp.width) myclip.max_x = dest_bmp.width-1;
		if (myclip.min_y < 0) myclip.min_y = 0;
		if (myclip.max_y >= dest_bmp.height) myclip.max_y = dest_bmp.height-1;

		clip=myclip;
	}


	/* ASG 980209 -- added 16-bit version */
	if (dest_bmp.depth != 16)
	{
		if( gfx !=null&& gfx.colortable !=null)
		{
			_ShortPtr pal = new _ShortPtr(gfx.colortable, (int)(gfx.color_granularity * (color % gfx.total_colors))); /* ASG 980209 */
			int source_base = (int)((code % gfx.total_elements) * gfx.height);

			int sprite_screen_height = (scaley*gfx.height+0x8000)>>16;
			int sprite_screen_width = (scalex*gfx.width+0x8000)>>16;

			/* compute sprite increment per screen pixel */
			int dx = (gfx.width<<16)/sprite_screen_width;
			int dy = (gfx.height<<16)/sprite_screen_height;

			int ex = sx+sprite_screen_width;
			int ey = sy+sprite_screen_height;

			int x_index_base;
			int y_index;

			if( flipx )
			{
				x_index_base = (sprite_screen_width-1)*dx;
				dx = -dx;
			}
			else
			{
				x_index_base = 0;
			}

			if( flipy )
			{
				y_index = (sprite_screen_height-1)*dy;
				dy = -dy;
			}
			else
			{
				y_index = 0;
			}

			if( clip!=null )
			{
				if( sx < clip.min_x)
				{ /* clip left */
					int pixels = clip.min_x-sx;
					sx += pixels;
					x_index_base += pixels*dx;
				}
				if( sy < clip.min_y )
				{ /* clip top */
					int pixels = clip.min_y-sy;
					sy += pixels;
					y_index += pixels*dy;
				}
				/* NS 980211 - fixed incorrect clipping */
				if( ex > clip.max_x+1 )
				{ /* clip right */
					int pixels = ex-clip.max_x-1;
					ex -= pixels;
				}
				if( ey > clip.max_y+1 )
				{ /* clip bottom */
					int pixels = ey-clip.max_y-1;
					ey -= pixels;
				}
			}

			if( ex>sx )
			{ /* skip if inner loop doesn't draw anything */
				int y;

				/* case 1: TRANSPARENCY_PEN */
				if (transparency == TRANSPARENCY_PEN)
				{
					for( y=sy; y<ey; y++ )
					{
						_BytePtr source = new _BytePtr( gfx.gfxdata,  (source_base+(y_index>>16)) * gfx.line_modulo);
						_BytePtr dest = dest_bmp.line[y];

						int x, x_index = x_index_base;
						for( x=sx; x<ex; x++ )
						{
							int c = source[x_index>>16];
							if( c != transparent_color ) dest[x] = pal[c];
							x_index += dx;
						}

						y_index += dy;
					}
				}

				/* case 2: TRANSPARENCY_COLOR */
				else if (transparency == TRANSPARENCY_COLOR)
				{
					for( y=sy; y<ey; y++ )
					{
						_BytePtr source = new _BytePtr(gfx.gfxdata , (source_base+(y_index>>16)) * gfx.line_modulo);
						_BytePtr dest = dest_bmp.line[y];

						int x, x_index = x_index_base;
						for( x=sx; x<ex; x++ )
						{
							int c = pal[source[x_index>>16]];
							if( c != transparent_color ) dest[x] = (byte)c;
							x_index += dx;
						}

						y_index += dy;
					}
				}
			}

		}
	}

	/* ASG 980209 -- new 16-bit part */
	else
	{
		if( gfx !=null&& gfx.colortable !=null)
		{
			_ShortPtr pal = new _ShortPtr(gfx.colortable, (int)(gfx.color_granularity * (color % gfx.total_colors))); /* ASG 980209 */
			int source_base = (int)((code % gfx.total_elements) * gfx.height);

			int sprite_screen_height = (scaley*gfx.height+0x8000)>>16;
			int sprite_screen_width = (scalex*gfx.width+0x8000)>>16;

			/* compute sprite increment per screen pixel */
			int dx = (gfx.width<<16)/sprite_screen_width;
			int dy = (gfx.height<<16)/sprite_screen_height;

			int ex = sx+sprite_screen_width;
			int ey = sy+sprite_screen_height;

			int x_index_base;
			int y_index;

			if( flipx )
			{
				x_index_base = (sprite_screen_width-1)*dx;
				dx = -dx;
			}
			else
			{
				x_index_base = 0;
			}

			if( flipy )
			{
				y_index = (sprite_screen_height-1)*dy;
				dy = -dy;
			}
			else
			{
				y_index = 0;
			}

			if( clip !=null)
			{
				if( sx < clip.min_x)
				{ /* clip left */
					int pixels = clip.min_x-sx;
					sx += pixels;
					x_index_base += pixels*dx;
				}
				if( sy < clip.min_y )
				{ /* clip top */
					int pixels = clip.min_y-sy;
					sy += pixels;
					y_index += pixels*dy;
				}
				/* NS 980211 - fixed incorrect clipping */
				if( ex > clip.max_x+1 )
				{ /* clip right */
					int pixels = ex-clip.max_x-1;
					ex -= pixels;
				}
				if( ey > clip.max_y+1 )
				{ /* clip bottom */
					int pixels = ey-clip.max_y-1;
					ey -= pixels;
				}
			}

			if( ex>sx )
			{ /* skip if inner loop doesn't draw anything */
				int y;

				/* case 1: TRANSPARENCY_PEN */
				if (transparency == TRANSPARENCY_PEN)
				{
					for( y=sy; y<ey; y++ )
					{
						_BytePtr source = new _BytePtr(gfx.gfxdata, (source_base+(y_index>>16)) * gfx.line_modulo);
						_ShortPtr dest = new _ShortPtr(dest_bmp.line[y]);

						int x, x_index = x_index_base;
						for( x=sx; x<ex; x++ )
						{
							int c = source[x_index>>16];
							if( c != transparent_color ) dest[x] = pal[c];
							x_index += dx;
						}

						y_index += dy;
					}
				}

				/* case 2: TRANSPARENCY_COLOR */
				else if (transparency == TRANSPARENCY_COLOR)
				{
					for( y=sy; y<ey; y++ )
					{
						_BytePtr source = new _BytePtr(gfx.gfxdata,  (source_base+(y_index>>16)) * gfx.line_modulo);
						_ShortPtr dest = new _ShortPtr(dest_bmp.line[y]);

						int x, x_index = x_index_base;
						for( x=sx; x<ex; x++ )
						{
							int c = pal[source[x_index>>16]];
							if( c != transparent_color ) dest.write16(x, (ushort) c);
							x_index += dx;
						}

						y_index += dy;
					}
				}
			}
		}
	}
}

    }
}
