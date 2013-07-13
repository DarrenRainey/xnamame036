using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class SpriteManager
        {
            public const byte SPRITE_LIST_BACK_TO_FRONT = 0x0;
            public const byte SPRITE_LIST_FRONT_TO_BACK = 0x1;
            public const byte SPRITE_LIST_RAW_DATA = 0x2;
            public const byte SPRITE_LIST_FLIPX = 0x4;
            public const byte SPRITE_LIST_FLIPY = 0x8;
            public enum SpriteType { SPRITE_TYPE_STACK, SPRITE_TYPE_UNPACK, SPRITE_TYPE_ZOOM };
            public class sprite
            {
                public int priority, flags;

                public byte[] pen_data;	/* points to top left corner of tile data */
                public int line_offset;

                public ushort[] pal_data;
                public uint pen_usage;

                public int x_offset, y_offset;
                public int tile_width, tile_height;
                public int total_width, total_height;	/* in screen coordinates */
                public int x, y;

                public int shadow_pen;

                /* private */
                public sprite next;
                /* private */
                public int mask_offset;
            }

            public class sprite_list
            {
                public SpriteType sprite_type;
                public int num_sprites;
                public int flags;
                public int max_priority;
                public int transparent_pen;
                public int special_pen;

                public sprite[] sprite;
                public sprite_list next; /* resource tracking */
            }

            static int orientation, screen_width, screen_height;
            static int screen_clip_left, screen_clip_top, screen_clip_right, screen_clip_bottom;
            static _BytePtr screen_baseaddr;
            static int screen_line_offset;

            static sprite_list first_sprite_list = null; /* used for resource tracking */
            static int FlickeringInvisible;

            static ushort[] shade_table;
            static _BytePtr mask_buffer = null;
            static int mask_buffer_size = 0, mask_buffer_used = 0;

            public static void sprite_init()
            {
                rectangle clip = Machine.drv.visible_area;
                int left = clip.min_x;
                int top = clip.min_y;
                int right = clip.max_x + 1;
                int bottom = clip.max_y + 1;

                osd_bitmap bitmap = Machine.scrbitmap;
                screen_baseaddr = bitmap.line[0];
                screen_line_offset = (int)(bitmap.line[1].offset - bitmap.line[0].offset);

                orientation = Machine.orientation;
                screen_width = Machine.scrbitmap.width;
                screen_height = Machine.scrbitmap.height;

                if ((orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    { int t = left; left = top; top = t; }
                    { int t = right; right = bottom; bottom = t; }
                }
                if ((orientation & ORIENTATION_FLIP_X) != 0)
                {
                    { int t = left; left = right; right = t; }
                    left = screen_width - left;
                    right = screen_width - right;
                }
                if ((orientation & ORIENTATION_FLIP_Y) != 0)
                {
                    { int t = top; top = bottom; bottom = t; }
                    top = screen_height - top;
                    bottom = screen_height - bottom;
                }

                screen_clip_left = left;
                screen_clip_right = right;
                screen_clip_top = top;
                screen_clip_bottom = bottom;
            }
            static void mask_buffer_dispose()
            {
                mask_buffer = null;
                mask_buffer_size = 0;
            }
            public static void sprite_close()
            {
                sprite_list sprite_list = first_sprite_list;
                mask_buffer_dispose();

                while (sprite_list != null)
                {
                    sprite_list next = sprite_list.next;
                    sprite_list.sprite = null;
                    sprite_list = null;
                    sprite_list = next;
                }
                first_sprite_list = null;
            }
            public static sprite_list sprite_list_create(int num_sprites, int flags)
            {
                sprite[] sprite = new SpriteManager.sprite[num_sprites];
                sprite_list sprite_list = new sprite_list();

                sprite_list.num_sprites = num_sprites;
                sprite_list.special_pen = -1;
                sprite_list.sprite = sprite;
                sprite_list.flags = flags;

                /* resource tracking */
                sprite_list.next = first_sprite_list;
                first_sprite_list = sprite_list;

                return sprite_list; /* warning: no error checking! */
            }
            public static void sprite_set_shade_table(ushort[]  table)
            {
                shade_table = table;
            }


        }
    }
}