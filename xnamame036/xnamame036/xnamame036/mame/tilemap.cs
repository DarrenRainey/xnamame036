using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const byte TILE_TRANSPARENT = 0, TILE_MASKED = 1, TILE_OPAQUE = 2;

        public static tilemap ALL_TILEMAPS = null;
        public const int TILEMAP_OPAQUE = 0x00;
        public const int TILEMAP_TRANSPARENT = 0x01;
        public const int TILEMAP_SPLIT = 0x02;
        public const int TILEMAP_BITMASK = 0x04;
        public const int TILE_LINE_DISABLED = unchecked((int)0x80000000);

        public const int TILEMAP_IGNORE_TRANSPARENCY = 0x10;
        public const int TILEMAP_BACK = 0x20;
        public const int TILEMAP_FRONT = 0x40;

        public const int TILEMAP_BITMASK_TRANSPARENT = 0;
        public const int TILEMAP_BITMASK_OPAQUE = -1;

        public static _BytePtr _TILEMAP_BITMASK_TRANSPARENT = new _BytePtr();
        public static _BytePtr _TILEMAP_BITMASK_OPAQUE = new _BytePtr();

        public const int TILEMAP_FLIPX = 0x01;
        public const int TILEMAP_FLIPY = 0x02;

        public const byte TILE_IGNORE_TRANSPARENCY = 0x10;

        public delegate void tilemap_draw_delegate(int a, int b);

        public class tilemap
        {
            public int dx, dx_if_flipped;
            public int dy, dy_if_flipped;
            public int scrollx_delta, scrolly_delta;

            public int type;
            public int enable;
            public int attributes;
            public int transparent_pen;
            public uint[] transmask = new uint[4];

            public int num_rows, num_cols, num_tiles;
            public int tile_width, tile_height, width, height;

            public tilemap_draw_delegate mark_visible;
            public tilemap_draw_delegate draw;
            public tilemap_draw_delegate draw_opaque;

            public _BytePtr[] pendata;
            public _BytePtr[] maskdata;
            public _ShortPtr[] paldata;
            public uint[] pen_usage;

            public byte[] priority;	/* priority for each tile */
            public _BytePtr[] priority_row;

            public byte[] visible; /* boolean flag for each tile */
            public _BytePtr[] visible_row;

            public byte[] dirty_vram; /* boolean flag for each tile */
            public byte[][] dirty_vram_row; /* TBA */

            public int[] span;	/* contains transparency type, and run length, for adjacent tiles of same transparency_type and priority */
            public int[][] span_row;

            public byte[] dirty_pixels;
            public byte[] flags;

            /* callback to interpret video VRAM for the tilemap */
            public tile_get_info_delegate tile_get_info;

            public int scrolled;
            public int scroll_rows, scroll_cols;
            public int[] rowscroll, colscroll;

            public int orientation;
            public int clip_left, clip_right, clip_top, clip_bottom;

            /* cached color data */
            public osd_bitmap pixmap;
            public int pixmap_line_offset;

            /* foreground mask - for transparent layers, or the front half of a split layer */
            public osd_bitmap fg_mask;
            public byte[] fg_mask_data;
            public _BytePtr[] fg_mask_data_row;
            public int fg_mask_line_offset;
            public ushort[] fg_span;
            public ushort[][] fg_span_row;

            /* background mask - for the back half of a split layer */
            public osd_bitmap bg_mask;
            public byte[] bg_mask_data;
            public _BytePtr[] bg_mask_data_row;
            public int bg_mask_line_offset;
            public ushort[] bg_span;
            public ushort[][] bg_span_row;

            public tilemap next; /* resource tracking */
        }
        public class _tile_info
        {
            public _BytePtr pen_data; /* pointer to gfx data */
            public _ShortPtr pal_data; /* pointer to palette */
            public _BytePtr mask_data; /* pointer to mask data (for TILEMAP_BITMASK) */
            public uint pen_usage;	/* used pens mask */
            /*
                you must set tile_info.pen_data, tile_info.pal_data and tile_info.pen_usage
                in the callback.  You can use the SET_TILE_INFO() macro below to do this.
                tile_info.flags and tile_info.priority will be automatically preset to 0,
                games that don't need them don't need to explicitly set them to 0
            */
            public byte flags; /* see below */
            public byte priority;
        }
        class _blit
        {
            public int clip_left, clip_top, clip_right, clip_bottom;
            public int source_width, source_height;

            public int dest_line_offset, source_line_offset, mask_line_offset;
            public int dest_row_offset, source_row_offset, mask_row_offset;
            public osd_bitmap screen, pixmap, bitmask;

            public _BytePtr[] mask_data_row;
            public _BytePtr[] priority_data_row, visible_row;
            public byte priority;
        }
        static _blit blit = new _blit();

        static byte[] flip_bit_table = new byte[0x100]; /* horizontal flip for 8 pixels */
        static tilemap first_tilemap; /* resource tracking */
        static int screen_width, screen_height;
        public static _tile_info tile_info = new _tile_info();

        public const byte TILE_FLIPX = 0x01;
        public const byte TILE_FLIPY = 0x02;
        public static int TILE_FLIPXY(int XY)
        {
            return ((((XY) >> 1) | ((XY) << 1)) & 3);
        }
        public static void SET_TILE_INFO(int GFX, int CODE, int COLOR)
        {
            GfxElement gfx = Machine.gfx[(GFX)];
            int _code = (int)((CODE) % gfx.total_elements);
            tile_info.pen_data = new _BytePtr(gfx.gfxdata, _code * gfx.char_modulo);
            tile_info.pal_data = new _ShortPtr(gfx.colortable, gfx.color_granularity * (COLOR));
            tile_info.pen_usage = gfx.pen_usage != null ? gfx.pen_usage[_code] : 0;
        }
        void tilemap_init()
        {
            int value, data, bit;
            for (value = 0; value < 0x100; value++)
            {
                data = 0;
                for (bit = 0; bit < 8; bit++) if (((value >> bit) & 1) != 0) data |= 0x80 >> bit;
                flip_bit_table[value] = (byte)data;
            }
            screen_width = Machine.scrbitmap.width;
            screen_height = Machine.scrbitmap.height;
            first_tilemap = null;
        }
        void tilemap_dispose(tilemap tilemap)
        {
            if (tilemap == first_tilemap)
            {
                first_tilemap = tilemap.next;
            }
            else
            {
                tilemap prev = first_tilemap;
                while (prev.next != tilemap) prev = prev.next;
                prev.next = tilemap.next;
            }

            dispose_tile_info(tilemap);
            dispose_bg_mask(tilemap);
            dispose_fg_mask(tilemap);
            dispose_pixmap(tilemap);
            tilemap = null;
        }
        static void dispose_bg_mask(tilemap tilemap)
        {
            throw new Exception();
        }
        static void dispose_fg_mask(tilemap tilemap)
        {
            throw new Exception();
        }
        static void dispose_pixmap(tilemap tilemap)
        {
            throw new Exception();
        }
        static void dispose_tile_info(tilemap tilemap)
        {
            throw new Exception();
        }
        void tilemap_close()
        {
            while (first_tilemap != null)
            {
                tilemap next = first_tilemap.next;
                tilemap_dispose(first_tilemap);
                first_tilemap = next;
            }
        }
        public static void tilemap_mark_tile_dirty(tilemap _tilemap, int col, int row)
        {
            /* convert logical coordinates to cached coordinates */
            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0) { var temp = col; col = row; row = temp; }
            if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) col = _tilemap.num_cols - 1 - col;
            if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) row = _tilemap.num_rows - 1 - row;

            //	tilemap.dirty_vram_row[row][col] = 1;
            _tilemap.dirty_vram[row * _tilemap.num_cols + col] = 1;
        }
        public static void tilemap_mark_all_tiles_dirty(tilemap _tilemap)
        {
            if (_tilemap == ALL_TILEMAPS)
            {
                _tilemap = first_tilemap;
                while (_tilemap != null)
                {
                    tilemap_mark_all_tiles_dirty(_tilemap);
                    _tilemap = _tilemap.next;
                }
            }
            else
            {
                for (int i = 0; i < _tilemap.num_tiles; i++) _tilemap.dirty_vram[i] = 1;
            }
        }
        public static void tilemap_set_flip(tilemap _tilemap, int attributes)
        {
            if (_tilemap == ALL_TILEMAPS)
            {
                _tilemap = first_tilemap;
                while (_tilemap != null)
                {
                    tilemap_set_flip(_tilemap, attributes);
                    _tilemap = _tilemap.next;
                }
            }
            else if (_tilemap.attributes != attributes)
            {
                _tilemap.attributes = attributes;

                _tilemap.orientation = Machine.orientation;

                if ((attributes & TILEMAP_FLIPY) != 0)
                {
                    _tilemap.orientation ^= ORIENTATION_FLIP_Y;
                    _tilemap.scrolly_delta = _tilemap.dy_if_flipped;
                }
                else
                {
                    _tilemap.scrolly_delta = _tilemap.dy;
                }

                if ((attributes & TILEMAP_FLIPX) != 0)
                {
                    _tilemap.orientation ^= ORIENTATION_FLIP_X;
                    _tilemap.scrollx_delta = _tilemap.dx_if_flipped;
                }
                else
                {
                    _tilemap.scrollx_delta = _tilemap.dx;
                }

                tilemap_mark_all_tiles_dirty(_tilemap);
            }
        }


        public static void tilemap_set_clip(tilemap _tilemap, rectangle clip)
        {
            int left, top, right, bottom;

            if (clip != null)
            {
                left = clip.min_x;
                top = clip.min_y;
                right = clip.max_x + 1;
                bottom = clip.max_y + 1;

                if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    var temp = left; left = top; top = temp;//SWAP(left,top)
                    temp = right; right = bottom; bottom = temp;//SWAP(right,bottom)
                }
                if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0)
                {
                    //SWAP(left,right)
                    var temp = left; left = right; right = temp;
                    left = screen_width - left;
                    right = screen_width - right;
                }
                if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0)
                {
                    //SWAP(top,bottom)
                    var temp = top; top = bottom; bottom = temp;
                    top = screen_height - top;
                    bottom = screen_height - bottom;
                }
            }
            else
            {
                left = 0;
                top = 0;
                right = _tilemap.width;
                bottom = _tilemap.height;
            }

            _tilemap.clip_left = left;
            _tilemap.clip_right = right;
            _tilemap.clip_top = top;
            _tilemap.clip_bottom = bottom;
            //if( errorlog ) fprintf( errorlog, "clip: %d,%d,%d,%d\n", left,top,right,bottom );
        }
        static void memcpybitmask8(_BytePtr dest, _BytePtr source, _BytePtr bitmask, int count)
        {
            for (; ; )
            {
                byte data = bitmask[0]; bitmask.offset++;
                if ((data & 0x80) != 0) dest[0] = source[0];
                if ((data & 0x40) != 0) dest[1] = source[1];
                if ((data & 0x20) != 0) dest[2] = source[2];
                if ((data & 0x10) != 0) dest[3] = source[3];
                if ((data & 0x08) != 0) dest[4] = source[4];
                if ((data & 0x04) != 0) dest[5] = source[5];
                if ((data & 0x02) != 0) dest[6] = source[6];
                if ((data & 0x01) != 0) dest[7] = source[7];
                if (--count == 0) break;
                source.offset += 8;
                dest.offset += 8;
            }
        }
        static void draw8x8x8BPP(int xpos, int ypos)
        {
            int x1 = xpos; int y1 = ypos; int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left; if (x2 > blit.clip_right)
                x2 = blit.clip_right; if (y1 < blit.clip_top) y1 = blit.clip_top;
            if (y2 > blit.clip_bottom) y2 = blit.clip_bottom; if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                byte priority = blit.priority;
                _BytePtr dest_baseaddr;
                _BytePtr dest_next;
                _BytePtr source_baseaddr;
                _BytePtr source_next;
                _BytePtr mask_baseaddr;
                _BytePtr mask_next;
                int c1; int c2; /* leftmost and rightmost visible columns in source tilemap */
                int y; /* current screen line to render */
                int y_next;
                dest_baseaddr = new _BytePtr(blit.screen.line[y1], xpos); /* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos; y1 -= ypos; x2 -= xpos; y2 -= ypos;
                source_baseaddr = new _BytePtr(blit.pixmap.line[y1]);
                mask_baseaddr = blit.bitmask.line[y1]; c1 = x1 / 8; /* round down */ c2 = (x2 + 8 - 1) / 8; /* round up */
                y = y1; y_next = 8 * (y1 / 8) + 8; if (y_next > y2) y_next = y2;
                {
                    int dy = y_next - y; dest_next = new _BytePtr(dest_baseaddr, dy * blit.dest_line_offset);
                    source_next = new _BytePtr(source_baseaddr, dy * blit.source_line_offset);
                    mask_next = new _BytePtr(mask_baseaddr, dy * blit.mask_line_offset);
                }
                for (; ; )
                {
                    int row = y / 8; _BytePtr mask_data = blit.mask_data_row[row];
                    _BytePtr priority_data = blit.priority_data_row[row];
                    byte tile_type; byte prev_tile_type = TILE_TRANSPARENT;
                    int x_start = x1; int x_end; int column;
                    for (column = c1; column <= c2; column++)
                    {
                        if (column == c2 || priority_data[column] != priority) tile_type = TILE_TRANSPARENT;
                        else tile_type = mask_data[column];
                        if (tile_type != prev_tile_type)
                        {
                            x_end = column * 8; if (x_end < x1) x_end = x1;
                            if (x_end > x2) x_end = x2;
                            if (prev_tile_type != TILE_TRANSPARENT)
                            {
                                if (prev_tile_type == TILE_MASKED)
                                {
                                    int count = (x_end + 7) / 8 - x_start / 8;
                                    _BytePtr mask0 = new _BytePtr(mask_baseaddr, x_start / 8);
                                    _BytePtr source0 = new _BytePtr(source_baseaddr, (x_start & 0xfff8));
                                    _BytePtr dest0 = new _BytePtr(dest_baseaddr, (x_start & 0xfff8));
                                    int i = y; for (; ; )
                                    {
                                        memcpybitmask8(dest0, source0, mask0, count);
                                        if (++i == y_next) break;
                                        dest0.offset += blit.dest_line_offset;
                                        source0.offset += blit.source_line_offset;
                                        mask0.offset += blit.mask_line_offset;
                                    }
                                }
                                else
                                { /* TILE_OPAQUE */
                                    int num_pixels = x_end - x_start;
                                    _BytePtr dest0 = new _BytePtr(dest_baseaddr, x_start);
                                    _BytePtr source0 = new _BytePtr(source_baseaddr, x_start); int i = y;
                                    for (; ; )
                                    {
                                        Buffer.BlockCopy(source0.buffer, source0.offset, dest0.buffer, dest0.offset, num_pixels);
                                        //memcpy( dest0, source0, num_pixels*sizeof(byte) );
                                        if (++i == y_next) break;
                                        dest0.offset += blit.dest_line_offset;
                                        source0.offset += blit.source_line_offset;
                                    }
                                }
                            } x_start = x_end;
                        }
                        prev_tile_type = tile_type;
                    }
                    if (y_next == y2) break; /* we are done! */ dest_baseaddr = dest_next;
                    source_baseaddr = source_next; mask_baseaddr = mask_next; y = y_next; y_next += 8;
                    if (y_next >= y2) { y_next = y2; }
                    else
                    {
                        dest_next.offset += blit.dest_row_offset;
                        source_next.offset += blit.source_row_offset;
                        mask_next.offset += blit.mask_row_offset;
                    }

                } /* process next row */
            } /* not totally clipped */
        }
        static void draw_opaque8x8x8BPP(int xpos, int ypos)
        {
            int x1 = xpos; int y1 = ypos; int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left; if (x2 > blit.clip_right) x2 = blit.clip_right;
            if (y1 < blit.clip_top) y1 = blit.clip_top; if (y2 > blit.clip_bottom) y2 = blit.clip_bottom;
            if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                byte priority = blit.priority;
                _BytePtr dest_baseaddr;
                _BytePtr dest_next;
                _BytePtr source_baseaddr;
                _BytePtr source_next;
                int c1; int c2; /* leftmost and rightmost visible columns in source tilemap */
                int y; /* current screen line to render */
                int y_next;
                dest_baseaddr = new _BytePtr(blit.screen.line[y1], xpos); /* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos;
                y1 -= ypos;
                x2 -= xpos;
                y2 -= ypos;
                source_baseaddr = new _BytePtr(blit.pixmap.line[y1]);
                c1 = x1 / 8; /* round down */
                c2 = (x2 + 8 - 1) / 8; /* round up */ y = y1;
                y_next = 8 * (y1 / 8) + 8;
                if (y_next > y2)
                    y_next = y2;
                {
                    int dy = y_next - y;
                    dest_next = new _BytePtr(dest_baseaddr, dy * blit.dest_line_offset);
                    source_next = new _BytePtr(source_baseaddr, dy * blit.source_line_offset);
                }
                for (; ; )
                {
                    int row = y / 8;
                    _BytePtr priority_data = blit.priority_data_row[row];
                    byte tile_type;
                    byte prev_tile_type = TILE_TRANSPARENT;
                    int x_start = x1; int x_end; int column;
                    for (column = c1; column <= c2; column++)
                    {
                        if (column == c2 || priority_data[column] != priority)
                            tile_type = TILE_TRANSPARENT;
                        else
                            tile_type = TILE_OPAQUE;
                        if (tile_type != prev_tile_type)
                        {
                            x_end = column * 8;
                            if (x_end < x1) x_end = x1;
                            if (x_end > x2) x_end = x2;
                            if (prev_tile_type != TILE_TRANSPARENT)
                            { /* TILE_OPAQUE */
                                int num_pixels = x_end - x_start;
                                _BytePtr dest0 = new _BytePtr(dest_baseaddr, x_start);
                                _BytePtr source0 = new _BytePtr(source_baseaddr, x_start);
                                int i = y;
                                for (; ; )
                                {
                                    Buffer.BlockCopy(source0.buffer, source0.offset, dest0.buffer, dest0.offset, num_pixels);//memcpy( dest0, source0, num_pixels*sizeof(UINT8) ); 
                                    if (++i == y_next) break;
                                    dest0.offset += blit.dest_line_offset;
                                    source0.offset += blit.source_line_offset;
                                }
                            }
                            x_start = x_end;
                        }
                        prev_tile_type = tile_type;
                    }
                    if (y_next == y2)
                        break; /* we are done! */
                    dest_baseaddr = dest_next;
                    source_baseaddr = source_next;
                    y = y_next;
                    y_next += 8;
                    if (y_next >= y2)
                    {
                        y_next = y2;
                    }
                    else
                    {
                        dest_next.offset += blit.dest_row_offset;
                        source_next.offset += blit.source_row_offset;
                    }
                } /* process next row */
            }
            /* not totally clipped */
        }
        static void mark_visible8x8x8BPP(int xpos, int ypos)
        {
            int x1 = xpos;
            int y1 = ypos;
            int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left;
            if (x2 > blit.clip_right) x2 = blit.clip_right;
            if (y1 < blit.clip_top) y1 = blit.clip_top;
            if (y2 > blit.clip_bottom) y2 = blit.clip_bottom;
            if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                int c1;
                int c2; /* leftmost and rightmost visible columns in source tilemap */
                int r1;
                int r2;
                _BytePtr[] visible_row;
                int span;
                int row; /* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos;
                y1 -= ypos;
                x2 -= xpos;
                y2 -= ypos;
                r1 = y1 / 8;
                r2 = (y2 + 8 - 1) / 8;
                c1 = x1 / 8; /* round down */
                c2 = (x2 + 8 - 1) / 8; /* round up */
                visible_row = blit.visible_row;
                span = c2 - c1;
                for (row = r1; row < r2; row++)
                {
                    for (int i = 0; i < span; i++)
                        visible_row[row][c1 + i] = 1;//memset(visible_row[row] + c1, 1, span); 
                }
            }
        }
        static void draw16x16x8BPP(int xpos, int ypos)
        {
            int x1 = xpos; int y1 = ypos; int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left;
            if (x2 > blit.clip_right)
                x2 = blit.clip_right;
            if (y1 < blit.clip_top) y1 = blit.clip_top;
            if (y2 > blit.clip_bottom) y2 = blit.clip_bottom;
            if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                byte priority = blit.priority;
                _BytePtr dest_baseaddr;
                _BytePtr dest_next;
                _BytePtr source_baseaddr;
                _BytePtr source_next;
                _BytePtr mask_baseaddr;
                _BytePtr mask_next;
                int c1;
                int c2; /* leftmost and rightmost visible columns in source tilemap */
                int y; /* current screen line to render */
                int y_next;
                dest_baseaddr = new _BytePtr(blit.screen.line[y1], xpos);/* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos;
                y1 -= ypos;
                x2 -= xpos;
                y2 -= ypos;
                source_baseaddr = new _BytePtr(blit.pixmap.line[y1]);
                mask_baseaddr = blit.bitmask.line[y1]; c1 = x1 / 16; /* round down */
                c2 = (x2 + 16 - 1) / 16; /* round up */ y = y1; y_next = 16 * (y1 / 16) + 16;
                if (y_next > y2) y_next = y2;
                {
                    int dy = y_next - y;
                    dest_next = new _BytePtr(dest_baseaddr, dy * blit.dest_line_offset);
                    source_next = new _BytePtr(source_baseaddr, dy * blit.source_line_offset);
                    mask_next = new _BytePtr(mask_baseaddr, dy * blit.mask_line_offset);
                }
                for (; ; )
                {
                    int row = y / 16;
                    _BytePtr mask_data = blit.mask_data_row[row];
                    _BytePtr priority_data = blit.priority_data_row[row];
                    byte tile_type;
                    byte prev_tile_type = TILE_TRANSPARENT;
                    int x_start = x1; int x_end;
                    int column;
                    for (column = c1; column <= c2; column++)
                    {
                        if (column == c2 || priority_data[column] != priority)
                            tile_type = TILE_TRANSPARENT;
                        else
                            tile_type = mask_data[column];
                        if (tile_type != prev_tile_type)
                        {
                            x_end = column * 16;
                            if (x_end < x1) x_end = x1;
                            if (x_end > x2) x_end = x2;
                            if (prev_tile_type != TILE_TRANSPARENT)
                            {
                                if (prev_tile_type == TILE_MASKED)
                                {
                                    int count = (x_end + 7) / 8 - x_start / 8;
                                    _BytePtr mask0 = new _BytePtr(mask_baseaddr, x_start / 8);
                                    _BytePtr source0 = new _BytePtr(source_baseaddr, (x_start & 0xfff8));
                                    _BytePtr dest0 = new _BytePtr(dest_baseaddr, (x_start & 0xfff8));
                                    int i = y;
                                    for (; ; )
                                    {
                                        memcpybitmask8(dest0, source0, mask0, count);
                                        if (++i == y_next) break;
                                        dest0.offset += blit.dest_line_offset;
                                        source0.offset += blit.source_line_offset;
                                        mask0.offset += blit.mask_line_offset;
                                    }
                                }
                                else
                                { /* TILE_OPAQUE */
                                    int num_pixels = x_end - x_start;
                                    _BytePtr dest0 = new _BytePtr(dest_baseaddr, x_start);
                                    _BytePtr source0 = new _BytePtr(source_baseaddr, x_start);
                                    int i = y;
                                    for (; ; )
                                    {
                                        //memcpy( dest0, source0, num_pixels*sizeof(UINT8) ); 
                                        Buffer.BlockCopy(source0.buffer, source0.offset, dest0.buffer, dest0.offset, num_pixels);
                                        if (++i == y_next)
                                            break;
                                        dest0.offset += blit.dest_line_offset;
                                        source0.offset += blit.source_line_offset;
                                    }
                                }
                            }
                            x_start = x_end;
                        }
                        prev_tile_type = tile_type;
                    }
                    if (y_next == y2)
                        break; /* we are done! */
                    dest_baseaddr = dest_next;
                    source_baseaddr = source_next;
                    mask_baseaddr = mask_next;
                    y = y_next;
                    y_next += 16;
                    if (y_next >= y2)
                    {
                        y_next = y2;
                    }
                    else
                    {
                        dest_next.offset += blit.dest_row_offset;
                        source_next.offset += blit.source_row_offset;
                        mask_next.offset += blit.mask_row_offset;
                    }
                } /* process next row */
            } /* not totally clipped */
        }
        static void draw_opaque16x16x8BPP(int xpos, int ypos)
        {
            int x1 = xpos; int y1 = ypos;
            int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left;
            if (x2 > blit.clip_right) x2 = blit.clip_right;
            if (y1 < blit.clip_top) y1 = blit.clip_top;
            if (y2 > blit.clip_bottom) y2 = blit.clip_bottom;
            if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                byte priority = blit.priority;
                _BytePtr dest_baseaddr;
                _BytePtr dest_next; _BytePtr source_baseaddr;
                _BytePtr source_next;
                int c1; int c2; /* leftmost and rightmost visible columns in source tilemap */
                int y; /* current screen line to render */
                int y_next;
                dest_baseaddr = new _BytePtr(blit.screen.line[y1], xpos); /* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos; y1 -= ypos; x2 -= xpos; y2 -= ypos;
                source_baseaddr = new _BytePtr(blit.pixmap.line[y1]);
                c1 = x1 / 16; /* round down */
                c2 = (x2 + 16 - 1) / 16; /* round up */ y = y1; y_next = 16 * (y1 / 16) + 16;
                if (y_next > y2)
                    y_next = y2;
                {
                    int dy = y_next - y;
                    dest_next = new _BytePtr(dest_baseaddr, dy * blit.dest_line_offset);
                    source_next = new _BytePtr(source_baseaddr, dy * blit.source_line_offset);
                }
                for (; ; )
                {
                    int row = y / 16;
                    _BytePtr priority_data = blit.priority_data_row[row];
                    byte tile_type;
                    byte prev_tile_type = TILE_TRANSPARENT;
                    int x_start = x1;
                    int x_end; int column; for (column = c1; column <= c2; column++)
                    {
                        if (column == c2 || priority_data[column] != priority)
                            tile_type = TILE_TRANSPARENT;
                        else
                            tile_type = TILE_OPAQUE;
                        if (tile_type != prev_tile_type)
                        {
                            x_end = column * 16;
                            if (x_end < x1) x_end = x1;
                            if (x_end > x2) x_end = x2;
                            if (prev_tile_type != TILE_TRANSPARENT)
                            { /* TILE_OPAQUE */
                                int num_pixels = x_end - x_start;
                                _BytePtr dest0 = new _BytePtr(dest_baseaddr, x_start);
                                _BytePtr source0 = new _BytePtr(source_baseaddr, x_start);
                                int i = y;
                                for (; ; )
                                {
                                    Buffer.BlockCopy(source0.buffer, source0.offset, dest0.buffer, dest0.offset, num_pixels);
                                    //memcpy( dest0, source0, num_pixels*sizeof(UINT8) );
                                    if (++i == y_next) break;
                                    dest0.offset += blit.dest_line_offset;
                                    source0.offset += blit.source_line_offset;
                                }
                            }
                            x_start = x_end;
                        }
                        prev_tile_type = tile_type;
                    }
                    if (y_next == y2) break; /* we are done! */
                    dest_baseaddr = dest_next;
                    source_baseaddr = source_next;
                    y = y_next;
                    y_next += 16;
                    if (y_next >= y2)
                    {
                        y_next = y2;
                    }
                    else
                    {
                        dest_next.offset += blit.dest_row_offset;
                        source_next.offset += blit.source_row_offset;
                    }
                } /* process next row */
            } /* not totally clipped */
        }
        static void mark_visible16x16x8BPP(int xpos, int ypos)
        {
            int x1 = xpos; int y1 = ypos; int x2 = xpos + blit.source_width;
            int y2 = ypos + blit.source_height; /* clip source coordinates */
            if (x1 < blit.clip_left) x1 = blit.clip_left; if (x2 > blit.clip_right) x2 = blit.clip_right;
            if (y1 < blit.clip_top) y1 = blit.clip_top; if (y2 > blit.clip_bottom) y2 = blit.clip_bottom;
            if (x1 < x2 && y1 < y2)
            { /* do nothing if totally clipped */
                int c1; int c2; /* leftmost and rightmost visible columns in source tilemap */
                int r1; int r2; _BytePtr[] visible_row; int span;
                int row; /* convert screen coordinates to source tilemap coordinates */
                x1 -= xpos; y1 -= ypos; x2 -= xpos; y2 -= ypos; r1 = y1 / 16; r2 = (y2 + 16 - 1) / 16;
                c1 = x1 / 16; /* round down */ c2 = (x2 + 16 - 1) / 16; /* round up */
                visible_row = blit.visible_row; span = c2 - c1;
                for (row = r1; row < r2; row++)
                {
                    for (int i = 0; i < span; i++)
                        visible_row[row][c1 + i] = 1;
                    //memset(visible_row[row] + c1, 1, span); 
                }
            }
        }

        static void draw32x32x8BPP(int xpos, int ypos) { throw new Exception(); }
        static void draw_opaque32x32x8BPP(int xpos, int ypos) { throw new Exception(); }
        static void mark_visible32x32x8BPP(int xpos, int ypos) { throw new Exception(); }

        static void draw8x8x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void draw_opaque8x8x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void mark_visible8x8x16BPP(int xpos, int ypos) { throw new Exception(); }

        static void draw16x16x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void draw_opaque16x16x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void mark_visible16x16x16BPP(int xpos, int ypos) { throw new Exception(); }

        static void draw32x32x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void draw_opaque32x32x16BPP(int xpos, int ypos) { throw new Exception(); }
        static void mark_visible32x32x16BPP(int xpos, int ypos) { throw new Exception(); }

        public delegate void tile_get_info_delegate(int col, int row);
        public static tilemap tilemap_create(tile_get_info_delegate tile_get_info, int type, int tile_width, int tile_height, /* in pixels */int num_cols, int num_rows /* in tiles */)
        {
            tilemap _tilemap = new tilemap();
            if (_tilemap != null)
            {
                //memset( tilemap, 0, sizeof( struct tilemap ) );

                _tilemap.orientation = Machine.orientation;
                if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    var temp = tile_width; tile_width = tile_height; tile_height = temp;
                    //SWAP( tile_width, tile_height )
                    //SWAP( num_cols,num_rows )
                    temp = num_cols; num_cols = num_rows; num_rows = temp;
                }

                _tilemap.tile_get_info = tile_get_info;
                _tilemap.enable = 1;
                tilemap_set_clip(_tilemap, Machine.drv.visible_area);

                if (Machine.scrbitmap.depth == 16)
                {
                    if (tile_width == 8 && tile_height == 8)
                    {
                        _tilemap.mark_visible = mark_visible8x8x16BPP;
                        _tilemap.draw = draw8x8x16BPP;
                        _tilemap.draw_opaque = draw_opaque8x8x16BPP;
                    }
                    else if (tile_width == 16 && tile_height == 16)
                    {
                        _tilemap.mark_visible = mark_visible16x16x16BPP;
                        _tilemap.draw = draw16x16x16BPP;
                        _tilemap.draw_opaque = draw_opaque16x16x16BPP;
                    }
                    else if (tile_width == 32 && tile_height == 32)
                    {
                        _tilemap.mark_visible = mark_visible32x32x16BPP;
                        _tilemap.draw = draw32x32x16BPP;
                        _tilemap.draw_opaque = draw_opaque32x32x16BPP;
                    }
                }
                else
                {
                    if (tile_width == 8 && tile_height == 8)
                    {
                        _tilemap.mark_visible = mark_visible8x8x8BPP;
                        _tilemap.draw = draw8x8x8BPP;
                        _tilemap.draw_opaque = draw_opaque8x8x8BPP;
                    }
                    else if (tile_width == 16 && tile_height == 16)
                    {
                        _tilemap.mark_visible = mark_visible16x16x8BPP;
                        _tilemap.draw = draw16x16x8BPP;
                        _tilemap.draw_opaque = draw_opaque16x16x8BPP;
                    }
                    else if (tile_width == 32 && tile_height == 32)
                    {
                        _tilemap.mark_visible = mark_visible32x32x8BPP;
                        _tilemap.draw = draw32x32x8BPP;
                        _tilemap.draw_opaque = draw_opaque32x32x8BPP;
                    }
                }

                if ((_tilemap.mark_visible != null && _tilemap.draw != null))
                {
                    _tilemap.type = type;

                    _tilemap.tile_width = tile_width;
                    _tilemap.tile_height = tile_height;
                    _tilemap.width = tile_width * num_cols;
                    _tilemap.height = tile_height * num_rows;

                    _tilemap.num_rows = num_rows;
                    _tilemap.num_cols = num_cols;
                    _tilemap.num_tiles = num_cols * num_rows;

                    _tilemap.scroll_rows = 1;
                    _tilemap.scroll_cols = 1;
                    _tilemap.scrolled = 1;

                    _tilemap.transparent_pen = -1; /* default (this is supplied by video driver) */

                    if (create_pixmap(_tilemap))
                    {
                        if (create_fg_mask(_tilemap))
                        {
                            if (create_bg_mask(_tilemap))
                            {
                                if (create_tile_info(_tilemap))
                                {
                                    _tilemap.next = first_tilemap;
                                    first_tilemap = _tilemap;
                                    return _tilemap;
                                }
                                dispose_bg_mask(_tilemap);
                            }
                            dispose_fg_mask(_tilemap);
                        }
                        dispose_pixmap(_tilemap);
                    }
                }
                _tilemap = null;
            }
            return null; /* error */
        }
        static bool create_tile_info(tilemap _tilemap)
        {
            int num_tiles = _tilemap.num_tiles;
            int num_cols = _tilemap.num_cols;
            int num_rows = _tilemap.num_rows;

            _tilemap.pendata = new _BytePtr[num_tiles];
            _tilemap.maskdata = new _BytePtr[num_tiles]; /* needed only for TILEMAP_BITMASK */
            _tilemap.paldata = new _ShortPtr[num_tiles];
            _tilemap.pen_usage = new uint[num_tiles];
            _tilemap.priority = new byte[num_tiles];
            _tilemap.visible = new byte[num_tiles];
            _tilemap.dirty_vram = new byte[num_tiles];
            _tilemap.dirty_pixels = new byte[num_tiles];
            _tilemap.flags = new byte[num_tiles];
            _tilemap.rowscroll = new int[_tilemap.height];
            _tilemap.colscroll = new int[_tilemap.width];

            _tilemap.priority_row = new _BytePtr[num_rows];
            _tilemap.visible_row = new _BytePtr[num_rows];

            if (_tilemap.pendata != null &&
                _tilemap.maskdata != null &&
                _tilemap.paldata != null && _tilemap.pen_usage != null &&
                _tilemap.priority != null && _tilemap.visible != null &&
                _tilemap.dirty_vram != null && _tilemap.dirty_pixels != null &&
                _tilemap.flags != null &&
                _tilemap.rowscroll != null && _tilemap.colscroll != null &&
                _tilemap.priority_row != null && _tilemap.visible_row != null)
            {
                int tile_index, row;

                for (row = 0; row < num_rows; row++)
                {
                    _tilemap.priority_row[row] = new _BytePtr(_tilemap.priority, num_cols * row);
                    _tilemap.visible_row[row] = new _BytePtr(_tilemap.visible, num_cols * row);
                }

                for (tile_index = 0; tile_index < num_tiles; tile_index++)
                {
                    _tilemap.paldata[tile_index] = null;
                }

                memset(_tilemap.priority, 0, num_tiles);
                memset(_tilemap.visible, 0, num_tiles);
                memset(_tilemap.dirty_vram, 1, num_tiles);
                memset(_tilemap.dirty_pixels, 1, num_tiles);

                return true; /* done */
            }
            dispose_tile_info(_tilemap);
            return false; /* error */
        }

        static bool create_fg_mask(tilemap _tilemap)
        {
            //if( tilemap.type == TILEMAP_OPAQUE ) return 1;

            _tilemap.fg_mask_data = new byte[_tilemap.num_tiles];
            if (_tilemap.fg_mask_data != null)
            {
                _tilemap.fg_mask_data_row = new_mask_data_table(_tilemap.fg_mask_data, _tilemap.num_cols, _tilemap.num_rows);
                if (_tilemap.fg_mask_data_row != null)
                {
                    _tilemap.fg_mask = create_bitmask(MASKROWBYTES(_tilemap.width), _tilemap.height);
                    if (_tilemap.fg_mask != null)
                    {
                        _tilemap.fg_mask_line_offset = _tilemap.fg_mask.line[1].offset - _tilemap.fg_mask.line[0].offset;
                        return true; /* done */
                    }
                    _tilemap.fg_mask_data_row = null;
                }
                _tilemap.fg_mask_data = null;
            }
            return false; /* error */
        }
        static _BytePtr[] new_mask_data_table(byte[] mask_data, int num_cols, int num_rows)
        {
            _BytePtr[] mask_data_row = new _BytePtr[num_rows];
            if (mask_data_row != null)
            {
                int row;
                for (row = 0; row < num_rows; row++) mask_data_row[row] = new _BytePtr(mask_data, num_cols * row);
            }
            return mask_data_row;
        }

        static bool create_bg_mask(tilemap _tilemap)
        {
            if ((_tilemap.type & TILEMAP_SPLIT) == 0) return true;

            _tilemap.bg_mask_data = new byte[_tilemap.num_tiles];
            if (_tilemap.bg_mask_data != null)
            {
                _tilemap.bg_mask_data_row = new_mask_data_table(_tilemap.bg_mask_data, _tilemap.num_cols, _tilemap.num_rows);
                if (_tilemap.bg_mask_data_row != null)
                {
                    _tilemap.bg_mask = create_bitmask(MASKROWBYTES(_tilemap.width), _tilemap.height);
                    if (_tilemap.bg_mask != null)
                    {
                        _tilemap.bg_mask_line_offset = _tilemap.bg_mask.line[1].offset - _tilemap.bg_mask.line[0].offset;
                        return true; /* done */
                    }
                    _tilemap.bg_mask_data_row = null;
                }
                _tilemap.bg_mask_data = null;
            }
            return false; /* error */
        }
        static int MASKROWBYTES(int W) { return (W + 7) / 8; }
        static osd_bitmap create_bitmask(int width, int height)
        {
            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                var temp = width; width = height; height = temp;
            }// SWAP(width, height);
            return osd_new_bitmap(width, height, 8);
        }

        static int draw_bitmask(osd_bitmap mask, int col, int row, int tile_width, int tile_height, _BytePtr maskdata, byte flags)
        {
            int is_opaque = 1, is_transparent = 1;

            int x, sx = tile_width * col;
            int sy, y1, y2, dy;

            if (maskdata == _TILEMAP_BITMASK_TRANSPARENT) return TILE_TRANSPARENT;
            if (maskdata == _TILEMAP_BITMASK_OPAQUE) return TILE_OPAQUE;

            if ((flags & TILE_FLIPY) != 0)
            {
                y1 = tile_height * row + tile_height - 1;
                y2 = y1 - tile_height;
                dy = -1;
            }
            else
            {
                y1 = tile_height * row;
                y2 = y1 + tile_height;
                dy = 1;
            }

            if ((flags & TILE_FLIPX) != 0)
            {
                tile_width--;
                for (sy = y1; sy != y2; sy += dy)
                {
                    _BytePtr mask_dest = new _BytePtr(mask.line[sy], sx / 8);
                    for (x = tile_width / 8; x >= 0; x--)
                    {
                        byte data = flip_bit_table[maskdata[0]]; maskdata.offset++;
                        if (data != 0x00) is_transparent = 0;
                        if (data != 0xff) is_opaque = 0;
                        mask_dest[x] = data;
                    }
                }
            }
            else
            {
                for (sy = y1; sy != y2; sy += dy)
                {
                    _BytePtr mask_dest = new _BytePtr(mask.line[sy], sx / 8);
                    for (x = 0; x < tile_width / 8; x++)
                    {
                        byte data = maskdata[0]; maskdata.offset++;
                        if (data != 0x00) is_transparent = 0;
                        if (data != 0xff) is_opaque = 0;
                        mask_dest[x] = data;
                    }
                }
            }

            if (is_transparent != 0) return TILE_TRANSPARENT;
            if (is_opaque != 0) return TILE_OPAQUE;
            return TILE_MASKED;
        }

        static void draw_mask(osd_bitmap mask, int col, int row, int tile_width, int tile_height, _BytePtr pendata, uint transmask, byte flags)
        {
            int x, bit, sx = tile_width * col;
            int sy, y1, y2, dy;

            if ((flags & TILE_FLIPY) != 0)
            {
                y1 = tile_height * row + tile_height - 1;
                y2 = y1 - tile_height;
                dy = -1;
            }
            else
            {
                y1 = tile_height * row;
                y2 = y1 + tile_height;
                dy = 1;
            }

            if ((flags & TILE_FLIPX) != 0)
            {
                tile_width--;
                for (sy = y1; sy != y2; sy += dy)
                {
                    _BytePtr mask_dest = new _BytePtr(mask.line[sy], sx / 8);
                    for (x = tile_width / 8; x >= 0; x--)
                    {
                        byte data = 0;
                        for (bit = 0; bit < 8; bit++)
                        {
                            byte p = pendata[0]; pendata.offset++;
                            data = (byte)((data >> 1) | (((1 << p) & transmask) != 0 ? 0x00 : 0x80));
                        }
                        mask_dest[x] = data;
                    }
                }
            }
            else
            {
                for (sy = y1; sy != y2; sy += dy)
                {
                    _BytePtr mask_dest = new _BytePtr(mask.line[sy], sx / 8);
                    for (x = 0; x < tile_width / 8; x++)
                    {
                        byte data = 0;
                        for (bit = 0; bit < 8; bit++)
                        {
                            byte p = pendata[0]; pendata.offset++;
                            data = (byte)((data << 1) | (((1 << p) & transmask) != 0 ? 0x00 : 0x01));
                        }
                        mask_dest[x] = data;
                    }
                }
            }
        }
        static void draw_tile(osd_bitmap pixmap, int col, int row, int tile_width, int tile_height, _BytePtr pendata, _ShortPtr paldata, byte flags)
        {
            int x, sx = tile_width * col;
            int sy, y1, y2, dy;

            if (Machine.scrbitmap.depth == 16)
            {
                if ((flags & TILE_FLIPY) != 0)
                {
                    y1 = tile_height * row + tile_height - 1;
                    y2 = y1 - tile_height;
                    dy = -1;
                }
                else
                {
                    y1 = tile_height * row;
                    y2 = y1 + tile_height;
                    dy = 1;
                }

                if ((flags & TILE_FLIPX) != 0)
                {
                    tile_width--;
                    for (sy = y1; sy != y2; sy += dy)
                    {
                        _ShortPtr dest = new _ShortPtr(pixmap.line[sy], sx);
                        for (x = tile_width; x >= 0; x--)
                        {
                            dest.write16(x, paldata.read16(pendata[0]));
                            pendata.offset++;
                        }
                    }
                }
                else
                {
                    for (sy = y1; sy != y2; sy += dy)
                    {
                        _ShortPtr dest = new _ShortPtr(pixmap.line[sy], sx);
                        for (x = 0; x < tile_width; x++)
                        {
                            dest.write16(x, paldata.read16(pendata[0]));
                            pendata.offset++;
                        }
                    }
                }
            }
            else
            {
                if ((flags & TILE_FLIPY) != 0)
                {
                    y1 = tile_height * row + tile_height - 1;
                    y2 = y1 - tile_height;
                    dy = -1;
                }
                else
                {
                    y1 = tile_height * row;
                    y2 = y1 + tile_height;
                    dy = 1;
                }

                if ((flags & TILE_FLIPX) != 0)
                {
                    tile_width--;
                    for (sy = y1; sy != y2; sy += dy)
                    {
                        _BytePtr dest = new _BytePtr(pixmap.line[sy], sx);
                        for (x = tile_width; x >= 0; x--) { dest[x] = (byte)paldata.read16(pendata[0]); pendata.offset++; }
                    }
                }
                else
                {
                    for (sy = y1; sy != y2; sy += dy)
                    {
                        _BytePtr dest = new _BytePtr(pixmap.line[sy], sx);
                        for (x = 0; x < tile_width; x++) { dest[x] = (byte)paldata.read16(pendata[0]); pendata.offset++; }
                    }
                }
            }
        }

        static bool create_pixmap(tilemap _tilemap)
        {
            _tilemap.pixmap = create_tmpbitmap(_tilemap.width, _tilemap.height);
            if (_tilemap.pixmap != null)
            {
                _tilemap.pixmap_line_offset = _tilemap.pixmap.line[1].offset - _tilemap.pixmap.line[0].offset;
                return true; /* done */
            }
            return false; /* error */
        }
        static osd_bitmap create_tmpbitmap(int width, int height)
        {
            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                var temp = width; width = height; height = temp;
            }
            return osd_new_bitmap(width, height, Machine.scrbitmap.depth);
        }
        public static void tilemap_update(tilemap _tilemap)
        {
            if (_tilemap == ALL_TILEMAPS)
            {
                _tilemap = first_tilemap;
                while (_tilemap != null)
                {
                    tilemap_update(_tilemap);
                    _tilemap = _tilemap.next;
                }
            }
            else if (_tilemap.enable != 0)
            {
                if (_tilemap.scrolled != 0)
                {
                    tilemap_draw_delegate mark_visible = _tilemap.mark_visible;

                    int rows = _tilemap.scroll_rows;
                    int[] rowscroll = _tilemap.rowscroll;
                    int cols = _tilemap.scroll_cols;
                    int[] colscroll = _tilemap.colscroll;

                    int left = _tilemap.clip_left;
                    int right = _tilemap.clip_right;
                    int top = _tilemap.clip_top;
                    int bottom = _tilemap.clip_bottom;

                    blit.source_width = _tilemap.width;
                    blit.source_height = _tilemap.height;
                    blit.visible_row = _tilemap.visible_row;

                    memset(_tilemap.visible, 0, _tilemap.num_tiles);

                    if (rows == 0 && cols == 0)
                    { /* no scrolling */
                        blit.clip_left = left;
                        blit.clip_top = top;
                        blit.clip_right = right;
                        blit.clip_bottom = bottom;

                        mark_visible(0, 0);
                    }
                    else if (rows == 0)
                    { /* scrolling columns */
                        int col, colwidth;

                        colwidth = blit.source_width / cols;

                        blit.clip_top = top;
                        blit.clip_bottom = bottom;

                        col = 0;
                        while (col < cols)
                        {
                            int cons, scroll;

                            /* count consecutive columns scrolled by the same amount */
                            scroll = colscroll[col];
                            cons = 1;
                            if (scroll != TILE_LINE_DISABLED)
                            {
                                while (col + cons < cols && colscroll[col + cons] == scroll) cons++;

                                if (scroll < 0) scroll = blit.source_height - (-scroll) % blit.source_height;
                                else scroll %= blit.source_height;

                                blit.clip_left = col * colwidth;
                                if (blit.clip_left < left) blit.clip_left = left;
                                blit.clip_right = (col + cons) * colwidth;
                                if (blit.clip_right > right) blit.clip_right = right;

                                mark_visible(0, scroll);
                                mark_visible(0, scroll - blit.source_height);
                            }
                            col += cons;
                        }
                    }
                    else if (cols == 0)
                    { /* scrolling rows */
                        int row, rowheight;

                        rowheight = blit.source_height / rows;

                        blit.clip_left = left;
                        blit.clip_right = right;

                        row = 0;
                        while (row < rows)
                        {
                            int cons, scroll;

                            /* count consecutive rows scrolled by the same amount */
                            scroll = rowscroll[row];
                            cons = 1;
                            if (scroll != TILE_LINE_DISABLED)
                            {
                                while (row + cons < rows && rowscroll[row + cons] == scroll) cons++;

                                if (scroll < 0) scroll = blit.source_width - (-scroll) % blit.source_width;
                                else scroll %= blit.source_width;

                                blit.clip_top = row * rowheight;
                                if (blit.clip_top < top) blit.clip_top = top;
                                blit.clip_bottom = (row + cons) * rowheight;
                                if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                                mark_visible(scroll, 0);
                                mark_visible(scroll - blit.source_width, 0);
                            }
                            row += cons;
                        }
                    }
                    else if (rows == 1 && cols == 1)
                    { /* XY scrolling playfield */
                        int scrollx, scrolly;

                        if (rowscroll[0] < 0) scrollx = blit.source_width - (-rowscroll[0]) % blit.source_width;
                        else scrollx = rowscroll[0] % blit.source_width;

                        if (colscroll[0] < 0) scrolly = blit.source_height - (-colscroll[0]) % blit.source_height;
                        else scrolly = colscroll[0] % blit.source_height;

                        blit.clip_left = left;
                        blit.clip_top = top;
                        blit.clip_right = right;
                        blit.clip_bottom = bottom;

                        mark_visible(scrollx, scrolly);
                        mark_visible(scrollx, scrolly - blit.source_height);
                        mark_visible(scrollx - blit.source_width, scrolly);
                        mark_visible(scrollx - blit.source_width, scrolly - blit.source_height);
                    }
                    else if (rows == 1)
                    { /* scrolling columns + horizontal scroll */
                        int col, colwidth;
                        int scrollx;

                        if (rowscroll[0] < 0) scrollx = blit.source_width - (-rowscroll[0]) % blit.source_width;
                        else scrollx = rowscroll[0] % blit.source_width;

                        colwidth = blit.source_width / cols;

                        blit.clip_top = top;
                        blit.clip_bottom = bottom;

                        col = 0;
                        while (col < cols)
                        {
                            int cons, scroll;

                            /* count consecutive columns scrolled by the same amount */
                            scroll = colscroll[col];
                            cons = 1;
                            if (scroll != TILE_LINE_DISABLED)
                            {
                                while (col + cons < cols && colscroll[col + cons] == scroll) cons++;

                                if (scroll < 0) scroll = blit.source_height - (-scroll) % blit.source_height;
                                else scroll %= blit.source_height;

                                blit.clip_left = col * colwidth + scrollx;
                                if (blit.clip_left < left) blit.clip_left = left;
                                blit.clip_right = (col + cons) * colwidth + scrollx;
                                if (blit.clip_right > right) blit.clip_right = right;

                                mark_visible(scrollx, scroll);
                                mark_visible(scrollx, scroll - blit.source_height);

                                blit.clip_left = col * colwidth + scrollx - blit.source_width;
                                if (blit.clip_left < left) blit.clip_left = left;
                                blit.clip_right = (col + cons) * colwidth + scrollx - blit.source_width;
                                if (blit.clip_right > right) blit.clip_right = right;

                                mark_visible(scrollx - blit.source_width, scroll);
                                mark_visible(scrollx - blit.source_width, scroll - blit.source_height);
                            }
                            col += cons;
                        }
                    }
                    else if (cols == 1)
                    { /* scrolling rows + vertical scroll */
                        int row, rowheight;
                        int scrolly;

                        if (colscroll[0] < 0) scrolly = blit.source_height - (-colscroll[0]) % blit.source_height;
                        else scrolly = colscroll[0] % blit.source_height;

                        rowheight = blit.source_height / rows;

                        blit.clip_left = left;
                        blit.clip_right = right;

                        row = 0;
                        while (row < rows)
                        {
                            int cons, scroll;

                            /* count consecutive rows scrolled by the same amount */
                            scroll = rowscroll[row];
                            cons = 1;
                            if (scroll != TILE_LINE_DISABLED)
                            {
                                while (row + cons < rows && rowscroll[row + cons] == scroll) cons++;

                                if (scroll < 0) scroll = blit.source_width - (-scroll) % blit.source_width;
                                else scroll %= blit.source_width;

                                blit.clip_top = row * rowheight + scrolly;
                                if (blit.clip_top < top) blit.clip_top = top;
                                blit.clip_bottom = (row + cons) * rowheight + scrolly;
                                if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                                mark_visible(scroll, scrolly);
                                mark_visible(scroll - blit.source_width, scrolly);

                                blit.clip_top = row * rowheight + scrolly - blit.source_height;
                                if (blit.clip_top < top) blit.clip_top = top;
                                blit.clip_bottom = (row + cons) * rowheight + scrolly - blit.source_height;
                                if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                                mark_visible(scroll, scrolly - blit.source_height);
                                mark_visible(scroll - blit.source_width, scrolly - blit.source_height);
                            }
                            row += cons;
                        }
                    }

                    _tilemap.scrolled = 0;
                }

                {
                    int num_pens = _tilemap.tile_width * _tilemap.tile_height; /* precalc - needed for >4bpp pen management handling */

                    int tile_index;
                    byte[] visible = _tilemap.visible;
                    byte[] dirty_vram = _tilemap.dirty_vram;
                    byte[] dirty_pixels = _tilemap.dirty_pixels;

                    _BytePtr[] pendata = _tilemap.pendata;
                    _BytePtr[] maskdata = _tilemap.maskdata;
                    _ShortPtr[] paldata = _tilemap.paldata;
                    uint[] pen_usage = _tilemap.pen_usage;

                    int tile_flip = 0;
                    if ((_tilemap.attributes & TILEMAP_FLIPX) != 0) tile_flip |= TILE_FLIPX;
                    if ((_tilemap.attributes & TILEMAP_FLIPY) != 0) tile_flip |= TILE_FLIPY;
#if !PREROTATE_GFX
                    if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                    {
                        if ((Machine.orientation & ORIENTATION_FLIP_X) != 0) tile_flip ^= TILE_FLIPY;
                        if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0) tile_flip ^= TILE_FLIPX;
                    }
                    else
                    {
                        if ((Machine.orientation & ORIENTATION_FLIP_X) != 0) tile_flip ^= TILE_FLIPX;
                        if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0) tile_flip ^= TILE_FLIPY;
                    }
#endif

                    tile_info.flags = 0;
                    tile_info.priority = 0;

                    for (tile_index = 0; tile_index < _tilemap.num_tiles; tile_index++)
                    {
                        if (visible[tile_index] != 0 && dirty_vram[tile_index] != 0)
                        {
                            int row = tile_index / _tilemap.num_cols;
                            int col = tile_index % _tilemap.num_cols;
                            int flags;

                            if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) row = _tilemap.num_rows - 1 - row;
                            if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) col = _tilemap.num_cols - 1 - col;
                            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0) { var temp = col; col = row; row = temp; }// SWAP(col,row)

                            {
                                _ShortPtr the_color = paldata[tile_index];
                                if (the_color != null)
                                {
                                    uint old_pen_usage = pen_usage[tile_index];
                                    if (old_pen_usage != 0)
                                    {
                                        palette_decrease_usage_count(the_color.offset - Machine.remapped_colortable.offset, old_pen_usage, PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                                    }
                                    else
                                    {
                                        palette_decrease_usage_countx(the_color.offset - Machine.remapped_colortable.offset, num_pens, pendata[tile_index], PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                                    }
                                }
                            }
                            _tilemap.tile_get_info(col, row);

                            flags = tile_info.flags ^ tile_flip;
                            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
                            {
                                flags =
                                    (flags & 0xfc) |
                                    ((flags & 1) << 1) | ((flags & 2) >> 1);
                            }

                            pen_usage[tile_index] = tile_info.pen_usage;
                            pendata[tile_index] = tile_info.pen_data;
                            paldata[tile_index] = tile_info.pal_data;
                            maskdata[tile_index] = tile_info.mask_data; // needed for _tilemap_BITMASK
                            _tilemap.flags[tile_index] = (byte)flags;
                            _tilemap.priority[tile_index] = tile_info.priority;


                            if (tile_info.pen_usage != 0)
                            {
                                palette_increase_usage_count(tile_info.pal_data.offset - Machine.remapped_colortable.offset, tile_info.pen_usage, PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                            }
                            else
                            {
                                palette_increase_usage_countx(tile_info.pal_data.offset - Machine.remapped_colortable.offset, num_pens, tile_info.pen_data, PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                            }

                            dirty_pixels[tile_index] = 1;
                            dirty_vram[tile_index] = 0;
                        }
                    }
                }
            }
        }
        public static void tilemap_mark_all_pixels_dirty(tilemap _tilemap)
        {
            if (_tilemap == ALL_TILEMAPS)
            {
                _tilemap = first_tilemap;
                while (_tilemap != null)
                {
                    tilemap_mark_all_pixels_dirty(_tilemap);
                    _tilemap = _tilemap.next;
                }
            }
            else
            {
                /* let's invalidate all offscreen tiles, decreasing the refcounts */
                int tile_index;
                int num_pens = _tilemap.tile_width * _tilemap.tile_height; /* precalc - needed for >4bpp pen management handling */
                for (tile_index = 0; tile_index < _tilemap.num_tiles; tile_index++)
                {
                    if (_tilemap.visible[tile_index] == 0)
                    {
                        _ShortPtr the_color = _tilemap.paldata[tile_index];
                        if (the_color != null)
                        {
                            uint old_pen_usage = _tilemap.pen_usage[tile_index];
                            if (old_pen_usage != 0)
                            {
                                palette_decrease_usage_count(the_color.offset - Machine.remapped_colortable.offset, old_pen_usage, PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                            }
                            else
                            {
                                palette_decrease_usage_countx(the_color.offset - Machine.remapped_colortable.offset, num_pens, _tilemap.pendata[tile_index], PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
                            }
                            _tilemap.paldata[tile_index] = null;
                        }
                        _tilemap.dirty_vram[tile_index] = 1;
                    }
                }
                memset(_tilemap.dirty_pixels, 1, _tilemap.num_tiles);
            }
        }
        public static void tilemap_render(tilemap _tilemap)
        {
            if (_tilemap == ALL_TILEMAPS)
            {
                _tilemap = first_tilemap;
                while (_tilemap != null)
                {
                    tilemap_render(_tilemap);
                    _tilemap = _tilemap.next;
                }
            }
            else if (_tilemap.enable != 0)
            {
                int type = _tilemap.type;
                int transparent_pen = _tilemap.transparent_pen;
                uint[] transmask = _tilemap.transmask;

                int tile_width = _tilemap.tile_width;
                int tile_height = _tilemap.tile_height;

                byte[] dirty_pixels = _tilemap.dirty_pixels;
                byte[] visible = _tilemap.visible;
                int tile_index = 0; // LBO - CWPro4 bug workaround
                int row, col;

                for (row = 0; row < _tilemap.num_rows; row++)
                {
                    for (col = 0; col < _tilemap.num_cols; col++)
                    {
                        if (dirty_pixels[tile_index] != 0 && visible[tile_index] != 0)
                        {
                            uint pen_usage = _tilemap.pen_usage[tile_index];
                            _BytePtr pendata = _tilemap.pendata[tile_index];
                            byte flags = _tilemap.flags[tile_index];

                            draw_tile(
                                _tilemap.pixmap,
                                col, row, tile_width, tile_height,
                                pendata,
                                _tilemap.paldata[tile_index],
                                flags);
                            if ((type & TILEMAP_BITMASK) != 0)
                            {
                                _tilemap.fg_mask_data_row[row][col] = (byte)
                                    draw_bitmask(_tilemap.fg_mask,
                                        col, row, tile_width, tile_height,
                                        _tilemap.maskdata[tile_index], flags);
                            }
                            else if ((type & TILEMAP_SPLIT) != 0)
                            {
                                int pen_mask = (transparent_pen < 0) ? 0 : (1 << transparent_pen);

                                if ((flags & TILE_IGNORE_TRANSPARENCY) != 0)
                                {
                                    _tilemap.fg_mask_data_row[row][col] = TILE_OPAQUE;
                                    _tilemap.bg_mask_data_row[row][col] = TILE_OPAQUE;
                                }
                                else if (pen_mask == pen_usage)
                                { /* totally transparent */
                                    _tilemap.fg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                    _tilemap.bg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                }
                                else
                                {
                                    uint fg_transmask = transmask[(flags >> 2) & 3];
                                    uint bg_transmask = (uint)((~fg_transmask) | pen_mask);
                                    if ((pen_usage & fg_transmask) == 0)
                                    { /* foreground totally opaque */
                                        _tilemap.fg_mask_data_row[row][col] = TILE_OPAQUE;
                                        _tilemap.bg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                    }
                                    else if ((pen_usage & bg_transmask) == 0)
                                    { /* background totally opaque */
                                        _tilemap.fg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                        _tilemap.bg_mask_data_row[row][col] = TILE_OPAQUE;
                                    }
                                    else if ((pen_usage & ~bg_transmask) == 0)
                                    { /* background transparent */
                                        draw_mask(_tilemap.fg_mask,
                                            col, row, tile_width, tile_height,
                                            pendata, fg_transmask, flags);
                                        _tilemap.fg_mask_data_row[row][col] = TILE_MASKED;
                                        _tilemap.bg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                    }
                                    else if ((pen_usage & ~fg_transmask) == 0)
                                    { /* foreground transparent */
                                        draw_mask(_tilemap.bg_mask,
                                            col, row, tile_width, tile_height,
                                            pendata, bg_transmask, flags);
                                        _tilemap.fg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                        _tilemap.bg_mask_data_row[row][col] = TILE_MASKED;
                                    }
                                    else
                                    { /* split tile - opacity in both foreground and background */
                                        draw_mask(_tilemap.fg_mask,
                                            col, row, tile_width, tile_height,
                                            pendata, fg_transmask, flags);
                                        draw_mask(_tilemap.bg_mask,
                                            col, row, tile_width, tile_height,
                                            pendata, bg_transmask, flags);
                                        _tilemap.fg_mask_data_row[row][col] = TILE_MASKED;
                                        _tilemap.bg_mask_data_row[row][col] = TILE_MASKED;
                                    }
                                }
                            }
                            else if (type == TILEMAP_TRANSPARENT)
                            {
                                uint fg_transmask = 1u << transparent_pen;
                                if ((flags & TILE_IGNORE_TRANSPARENCY) != 0) fg_transmask = 0;

                                if (pen_usage == fg_transmask)
                                {
                                    _tilemap.fg_mask_data_row[row][col] = TILE_TRANSPARENT;
                                }
                                else if ((pen_usage & fg_transmask) != 0)
                                {
                                    draw_mask(_tilemap.fg_mask,
                                        col, row, tile_width, tile_height,
                                        pendata, fg_transmask, flags);
                                    _tilemap.fg_mask_data_row[row][col] = TILE_MASKED;
                                }
                                else
                                {
                                    _tilemap.fg_mask_data_row[row][col] = TILE_OPAQUE;
                                }
                            }
                            else
                            {
                                _tilemap.fg_mask_data_row[row][col] = TILE_OPAQUE;
                            }

                            dirty_pixels[tile_index] = 0;
                        }
                        tile_index++;
                    } /* next col */
                } /* next row */
            }
        }

        static void palette_increase_usage_count(int table_offset, uint usage_mask, int color_flags)
        {
            /* if we are not dynamically reducing the palette, return immediately. */
            if (palette_used_colors == null) return;

            while (usage_mask != 0)
            {
                if ((usage_mask & 1) != 0)
                {
                    if ((color_flags & PALETTE_COLOR_VISIBLE) != 0)
                        pen_visiblecount.write32(Machine.game_colortable.read16(table_offset), pen_visiblecount.read32(Machine.game_colortable.read16(table_offset)) + 1);
                    if ((color_flags & PALETTE_COLOR_CACHED) != 0)
                        pen_cachedcount.write32(Machine.game_colortable.read16(table_offset), pen_cachedcount.read32(Machine.game_colortable.read16(table_offset)) + 1);
                }
                table_offset++;
                usage_mask >>= 1;
            }
        }

        static void palette_decrease_usage_count(int table_offset, uint usage_mask, int color_flags)
        {
            /* if we are not dynamically reducing the palette, return immediately. */
            if (palette_used_colors == null) return;

            while (usage_mask != 0)
            {
                if ((usage_mask & 1) != 0)
                {
                    ushort index = Machine.game_colortable.read16(table_offset);
                    if ((color_flags & PALETTE_COLOR_VISIBLE) != 0)
                        pen_visiblecount.write32(index, pen_visiblecount.read32(index) - 1);
                    //[Machine.game_colortable[table_offset]]--;
                    if ((color_flags & PALETTE_COLOR_CACHED) != 0)
                        pen_cachedcount.write32(index, pen_cachedcount.read32(index) - 1);
                    //pen_cachedcount[Machine.game_colortable[table_offset]]--;
                }
                table_offset++;
                usage_mask >>= 1;
            }
        }
        static void palette_increase_usage_countx(int table_offset, int num_pens, _BytePtr pen_data, int color_flags)
        {
            byte[] flag = new byte[256];
            //memset(flag,0,256);

            while (num_pens-- != 0)
            {
                int pen = pen_data[num_pens];
                if (flag[pen] == 0)
                {
                    if ((color_flags & PALETTE_COLOR_VISIBLE) != 0)
                    {
                        var t = pen_visiblecount.read32(Machine.game_colortable.read16(table_offset + pen));
                        pen_visiblecount.write32(Machine.game_colortable.read16(table_offset + pen), t + 1);
                    }
                    if ((color_flags & PALETTE_COLOR_CACHED) != 0)
                        pen_cachedcount[Machine.game_colortable.read16(table_offset + pen)]++;
                    flag[pen] = 1;
                }
            }
        }

        static void palette_decrease_usage_countx(int table_offset, int num_pens, _BytePtr pen_data, int color_flags)
        {
            byte[] flag = new byte[256];
            //memset(flag,0,256);

            while (num_pens-- != 0)
            {
                int pen = pen_data[num_pens];
                if (flag[pen] == 0)
                {
                    if ((color_flags & PALETTE_COLOR_VISIBLE) != 0)
                        pen_visiblecount[Machine.game_colortable[table_offset + pen]]--;
                    if ((color_flags & PALETTE_COLOR_CACHED) != 0)
                        pen_cachedcount[Machine.game_colortable[table_offset + pen]]--;
                    flag[pen] = 1;
                }
            }
        }
        public static void tilemap_draw(osd_bitmap dest, tilemap _tilemap, int priority)
        {
            int xpos, ypos;

            if (_tilemap.enable != 0)
            {
                tilemap_draw_delegate draw;

                int rows = _tilemap.scroll_rows;
                int[] rowscroll = _tilemap.rowscroll;
                int cols = _tilemap.scroll_cols;
                int[] colscroll = _tilemap.colscroll;

                int left = _tilemap.clip_left;
                int right = _tilemap.clip_right;
                int top = _tilemap.clip_top;
                int bottom = _tilemap.clip_bottom;

                int tile_height = _tilemap.tile_height;

                blit.screen = dest;
                blit.dest_line_offset = dest.line[1].offset - dest.line[0].offset;

                blit.pixmap = _tilemap.pixmap;
                blit.source_line_offset = _tilemap.pixmap_line_offset;

                if (_tilemap.type == TILEMAP_OPAQUE || (priority & TILEMAP_IGNORE_TRANSPARENCY) != 0)
                {
                    draw = _tilemap.draw_opaque;
                }
                else
                {
                    draw = _tilemap.draw;

                    if ((priority & TILEMAP_BACK) != 0)
                    {
                        blit.bitmask = _tilemap.bg_mask;
                        blit.mask_line_offset = _tilemap.bg_mask_line_offset;
                        blit.mask_data_row = _tilemap.bg_mask_data_row;
                    }
                    else
                    {
                        blit.bitmask = _tilemap.fg_mask;
                        blit.mask_line_offset = _tilemap.fg_mask_line_offset;
                        blit.mask_data_row = _tilemap.fg_mask_data_row;
                    }

                    blit.mask_row_offset = tile_height * blit.mask_line_offset;
                }

                if (dest.depth == 16)
                {
                    blit.dest_line_offset /= 2;
                    blit.source_line_offset /= 2;
                }

                blit.source_row_offset = tile_height * blit.source_line_offset;
                blit.dest_row_offset = tile_height * blit.dest_line_offset;

                blit.priority_data_row = _tilemap.priority_row;
                blit.source_width = _tilemap.width;
                blit.source_height = _tilemap.height;
                blit.priority = (byte)(priority & 0xf);

                if (rows == 0 && cols == 0)
                { /* no scrolling */
                    blit.clip_left = left;
                    blit.clip_top = top;
                    blit.clip_right = right;
                    blit.clip_bottom = bottom;

                    draw(0, 0);
                }
                else if (rows == 0)
                { /* scrolling columns */
                    int col = 0;
                    int colwidth = blit.source_width / cols;

                    blit.clip_top = top;
                    blit.clip_bottom = bottom;

                    while (col < cols)
                    {
                        int cons = 1;
                        int scrolly = colscroll[col];

                        /* count consecutive columns scrolled by the same amount */
                        if (scrolly != TILE_LINE_DISABLED)
                        {
                            while (col + cons < cols && colscroll[col + cons] == scrolly) cons++;

                            if (scrolly < 0)
                            {
                                scrolly = blit.source_height - (-scrolly) % blit.source_height;
                            }
                            else
                            {
                                scrolly %= blit.source_height;
                            }

                            blit.clip_left = col * colwidth;
                            if (blit.clip_left < left) blit.clip_left = left;
                            blit.clip_right = (col + cons) * colwidth;
                            if (blit.clip_right > right) blit.clip_right = right;

                            for (
                                ypos = scrolly - blit.source_height;
                                ypos < blit.clip_bottom;
                                ypos += blit.source_height)
                            {
                                draw(0, ypos);
                            }
                        }
                        col += cons;
                    }
                }
                else if (cols == 0)
                { /* scrolling rows */
                    int row = 0;
                    int rowheight = blit.source_height / rows;

                    blit.clip_left = left;
                    blit.clip_right = right;

                    while (row < rows)
                    {
                        int cons = 1;
                        int scrollx = rowscroll[row];

                        /* count consecutive rows scrolled by the same amount */
                        if (scrollx != TILE_LINE_DISABLED)
                        {
                            while (row + cons < rows && rowscroll[row + cons] == scrollx) cons++;

                            if (scrollx < 0)
                            {
                                scrollx = blit.source_width - (-scrollx) % blit.source_width;
                            }
                            else
                            {
                                scrollx %= blit.source_width;
                            }

                            blit.clip_top = row * rowheight;
                            if (blit.clip_top < top) blit.clip_top = top;
                            blit.clip_bottom = (row + cons) * rowheight;
                            if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                            for (
                                xpos = scrollx - blit.source_width;
                                xpos < blit.clip_right;
                                xpos += blit.source_width
                            )
                            {
                                draw(xpos, 0);
                            }
                        }
                        row += cons;
                    }
                }
                else if (rows == 1 && cols == 1)
                { /* XY scrolling playfield */
                    int scrollx = rowscroll[0];
                    int scrolly = colscroll[0];

                    if (scrollx < 0)
                    {
                        scrollx = blit.source_width - (-scrollx) % blit.source_width;
                    }
                    else
                    {
                        scrollx = scrollx % blit.source_width;
                    }

                    if (scrolly < 0)
                    {
                        scrolly = blit.source_height - (-scrolly) % blit.source_height;
                    }
                    else
                    {
                        scrolly = scrolly % blit.source_height;
                    }

                    blit.clip_left = left;
                    blit.clip_top = top;
                    blit.clip_right = right;
                    blit.clip_bottom = bottom;

                    for (
                        ypos = scrolly - blit.source_height;
                        ypos < blit.clip_bottom;
                        ypos += blit.source_height
                    )
                    {
                        for (
                            xpos = scrollx - blit.source_width;
                            xpos < blit.clip_right;
                            xpos += blit.source_width
                        )
                        {
                            draw(xpos, ypos);
                        }
                    }
                }
                else if (rows == 1)
                { /* scrolling columns + horizontal scroll */
                    int col = 0;
                    int colwidth = blit.source_width / cols;
                    int scrollx = rowscroll[0];

                    if (scrollx < 0)
                    {
                        scrollx = blit.source_width - (-scrollx) % blit.source_width;
                    }
                    else
                    {
                        scrollx = scrollx % blit.source_width;
                    }

                    blit.clip_top = top;
                    blit.clip_bottom = bottom;

                    while (col < cols)
                    {
                        int cons = 1;
                        int scrolly = colscroll[col];

                        /* count consecutive columns scrolled by the same amount */
                        if (scrolly != TILE_LINE_DISABLED)
                        {
                            while (col + cons < cols && colscroll[col + cons] == scrolly) cons++;

                            if (scrolly < 0)
                            {
                                scrolly = blit.source_height - (-scrolly) % blit.source_height;
                            }
                            else
                            {
                                scrolly %= blit.source_height;
                            }

                            blit.clip_left = col * colwidth + scrollx;
                            if (blit.clip_left < left) blit.clip_left = left;
                            blit.clip_right = (col + cons) * colwidth + scrollx;
                            if (blit.clip_right > right) blit.clip_right = right;

                            for (
                                ypos = scrolly - blit.source_height;
                                ypos < blit.clip_bottom;
                                ypos += blit.source_height
                            )
                            {
                                draw(scrollx, ypos);
                            }

                            blit.clip_left = col * colwidth + scrollx - blit.source_width;
                            if (blit.clip_left < left) blit.clip_left = left;
                            blit.clip_right = (col + cons) * colwidth + scrollx - blit.source_width;
                            if (blit.clip_right > right) blit.clip_right = right;

                            for (
                                ypos = scrolly - blit.source_height;
                                ypos < blit.clip_bottom;
                                ypos += blit.source_height
                            )
                            {
                                draw(scrollx - blit.source_width, ypos);
                            }
                        }
                        col += cons;
                    }
                }
                else if (cols == 1)
                { /* scrolling rows + vertical scroll */
                    int row = 0;
                    int rowheight = blit.source_height / rows;
                    int scrolly = colscroll[0];

                    if (scrolly < 0)
                    {
                        scrolly = blit.source_height - (-scrolly) % blit.source_height;
                    }
                    else
                    {
                        scrolly = scrolly % blit.source_height;
                    }

                    blit.clip_left = left;
                    blit.clip_right = right;

                    while (row < rows)
                    {
                        int cons = 1;
                        int scrollx = rowscroll[row];

                        /* count consecutive rows scrolled by the same amount */

                        if (scrollx != TILE_LINE_DISABLED)
                        {
                            while (row + cons < rows && rowscroll[row + cons] == scrollx) cons++;

                            if (scrollx < 0)
                            {
                                scrollx = blit.source_width - (-scrollx) % blit.source_width;
                            }
                            else
                            {
                                scrollx %= blit.source_width;
                            }

                            blit.clip_top = row * rowheight + scrolly;
                            if (blit.clip_top < top) blit.clip_top = top;
                            blit.clip_bottom = (row + cons) * rowheight + scrolly;
                            if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                            for (
                                xpos = scrollx - blit.source_width;
                                xpos < blit.clip_right;
                                xpos += blit.source_width
                            )
                            {
                                draw(xpos, scrolly);
                            }

                            blit.clip_top = row * rowheight + scrolly - blit.source_height;
                            if (blit.clip_top < top) blit.clip_top = top;
                            blit.clip_bottom = (row + cons) * rowheight + scrolly - blit.source_height;
                            if (blit.clip_bottom > bottom) blit.clip_bottom = bottom;

                            for (
                                xpos = scrollx - blit.source_width;
                                xpos < blit.clip_right;
                                xpos += blit.source_width
                            )
                            {
                                draw(xpos, scrolly - blit.source_height);
                            }
                        }
                        row += cons;
                    }
                }
            }
        }
        public static void tilemap_set_scrollx(tilemap _tilemap, int which, int value)
        {
            value = _tilemap.scrollx_delta - value;

            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) which = _tilemap.scroll_cols - 1 - which;
                if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) value = screen_height - _tilemap.height - value;
                if (_tilemap.colscroll[which] != value)
                {
                    _tilemap.scrolled = 1;
                    _tilemap.colscroll[which] = value;
                }
            }
            else
            {
                if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) which = _tilemap.scroll_rows - 1 - which;
                if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) value = screen_width - _tilemap.width - value;
                if (_tilemap.rowscroll[which] != value)
                {
                    _tilemap.scrolled = 1;
                    _tilemap.rowscroll[which] = value;
                }
            }
        }
        public static void tilemap_set_scrolly(tilemap _tilemap, int which, int value)
        {
            value = _tilemap.scrolly_delta - value;

            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) which = _tilemap.scroll_rows - 1 - which;
                if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) value = screen_width - _tilemap.width - value;
                if (_tilemap.rowscroll[which] != value)
                {
                    _tilemap.scrolled = 1;
                    _tilemap.rowscroll[which] = value;
                }
            }
            else
            {
                if ((_tilemap.orientation & ORIENTATION_FLIP_X) != 0) which = _tilemap.scroll_cols - 1 - which;
                if ((_tilemap.orientation & ORIENTATION_FLIP_Y) != 0) value = screen_height - _tilemap.height - value;
                if (_tilemap.colscroll[which] != value)
                {
                    _tilemap.scrolled = 1;
                    _tilemap.colscroll[which] = value;
                }
            }
        }
        public static void tilemap_set_scroll_cols(tilemap _tilemap, int n)
        {
            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if (_tilemap.scroll_rows != n)
                {
                    _tilemap.scroll_rows = n;
                    _tilemap.scrolled = 1;
                }
            }
            else
            {
                if (_tilemap.scroll_cols != n)
                {
                    _tilemap.scroll_cols = n;
                    _tilemap.scrolled = 1;
                }
            }
        }
        public static void tilemap_set_scrolldx(tilemap tilemap, int dx, int dx_if_flipped)
        {
            tilemap.dx = dx;
            tilemap.dx_if_flipped = dx_if_flipped;
            tilemap.scrollx_delta = (tilemap.attributes & TILEMAP_FLIPX) != 0 ? dx_if_flipped : dx;
        }

        public static void tilemap_set_scrolldy(tilemap tilemap, int dy, int dy_if_flipped)
        {
            tilemap.dy = dy;
            tilemap.dy_if_flipped = dy_if_flipped;
            tilemap.scrolly_delta = (tilemap.attributes & TILEMAP_FLIPY) != 0 ? dy_if_flipped : dy;
        }
        public static void tilemap_set_scroll_rows(tilemap _tilemap, int n)
        {
            if ((_tilemap.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if (_tilemap.scroll_cols != n)
                {
                    _tilemap.scroll_cols = n;
                    _tilemap.scrolled = 1;
                }
            }
            else
            {
                if (_tilemap.scroll_rows != n)
                {
                    _tilemap.scroll_rows = n;
                    _tilemap.scrolled = 1;
                }
            }
        }

    }
}
