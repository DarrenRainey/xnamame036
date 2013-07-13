using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class Generic
    {
        public static _BytePtr videoram = new _BytePtr(1);
        public static _BytePtr colorram = new _BytePtr(1);
        public static _BytePtr spriteram = new _BytePtr(1);
        public static _BytePtr spriteram_2 = new _BytePtr(1);
        public static _BytePtr spriteram_3 = new _BytePtr(1);
        public static _BytePtr buffered_spriteram = new _BytePtr(1);
        public static _BytePtr buffered_spriteram_2 = new _BytePtr(1);

        public static int[] videoram_size = new int[1];
        public static int[] spriteram_size = new int[1];
        public static int[] spriteram_2_size = new int[1];
        public static int[] spriteram_3_size = new int[1];

        byte[] flip_screen = new byte[1];
        byte[] flip_screen_x = new byte[1];
        byte[] flip_screen_y = new byte[1];
        public static bool[] dirtybuffer;
        public static Mame.osd_bitmap tmpbitmap;

        public static void SetDirtyBuffer(bool value)
        {
            for (int i = 0; i < videoram_size[0]; i++) dirtybuffer[i] = value;
        }
        public static int videoram_r(int offset)
        {
            return videoram[offset];
        }
        public static int colorram_r(int offset)
        {
            return colorram[offset];
        }
        public static void buffer_spriteram_w(int offset, int data)
        {
            Buffer.BlockCopy(spriteram.buffer, spriteram.offset, buffered_spriteram.buffer, buffered_spriteram.offset, spriteram_size[0]);
            //memcpy(buffered_spriteram, spriteram, spriteram_size);
        }

         public static void videoram_w(int offset, int data)
        {
            if (videoram[offset] != data)
            {
                dirtybuffer[offset] = true;

                videoram[offset] = (byte)data;
            }
        }
        public static void colorram_w(int offset, int data)
        {
            if (colorram[offset] != data)
            {
                dirtybuffer[offset] = true;

                colorram[offset] = (byte)data;
            }
        }
        public static void generic_vh_stop()
        {
            dirtybuffer = null;
            tmpbitmap = null;
        }
        public static int generic_vh_start()
        {
            dirtybuffer = null;
            tmpbitmap = null;

            if (videoram_size[0] == 0)
            {
                System.Console.WriteLine("Error: generic_vh_start() called but videoram_size not initialized\n");
                return 1;
            }

            dirtybuffer = new bool[videoram_size[0]];
            Generic.SetDirtyBuffer(true);

            tmpbitmap = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

            return 0;
        }
        public static int generic_bitmapped_vh_start()
        {
            if ((tmpbitmap = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth)) == null)
            {
                return 1;
            }

            return 0;
        }
        public static void generic_bitmapped_vh_stop()
        {
            Mame.osd_free_bitmap(tmpbitmap);
            tmpbitmap = null;
        }
    }
}
