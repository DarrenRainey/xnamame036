using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class rallyx
    {


        public static _BytePtr rallyx_videoram2 = new _BytePtr(1), rallyx_colorram2 = new _BytePtr(1);
        public static _BytePtr rallyx_radarx = new _BytePtr(1), rallyx_radary = new _BytePtr(1), rallyx_radarattr = new _BytePtr(1);
        public static int[] rallyx_radarram_size = new int[1];
        public static _BytePtr rallyx_scrollx = new _BytePtr(1), rallyx_scrolly = new _BytePtr(1);
        public static bool[] dirtybuffer2;	/* keep track of modified portions of the screen */
        /* to speed up video refresh */
        public static Mame.osd_bitmap tmpbitmap1;
        public static int flipscreen;


        public static Mame.rectangle spritevisiblearea =
        new Mame.rectangle(
            0 * 8, 28 * 8 - 1,
            0 * 8, 28 * 8 - 1
        );

        public static Mame.rectangle spritevisibleareaflip =
        new Mame.rectangle(
            8 * 8, 36 * 8 - 1,
            0 * 8, 28 * 8 - 1
        );

        public static Mame.rectangle radarvisiblearea =
        new Mame.rectangle(
            28 * 8, 36 * 8 - 1,
            0 * 8, 28 * 8 - 1
        );

        public static Mame.rectangle radarvisibleareaflip =
        new Mame.rectangle(
            0 * 8, 8 * 8 - 1,
            0 * 8, 28 * 8 - 1
        );
        public static void rallyx_videoram2_w(int offset, int data)
        {
            if (rallyx_videoram2[offset] != data)
            {
                dirtybuffer2[offset] = true;

                rallyx_videoram2[offset] = (byte)data;
            }
        }


        public static void rallyx_colorram2_w(int offset, int data)
        {
            if (rallyx_colorram2[offset] != data)
            {
                dirtybuffer2[offset] = true;

                rallyx_colorram2[offset] = (byte)data;
            }
        }



        public static void rallyx_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
                for (int i = 0; i < Generic.videoram_size[0]; i++)
                    dirtybuffer2[i] = true;
            }
        }

        public static int rallyx_vh_start()
        {
            if (Generic.generic_vh_start() != 0)
                return 1;

            dirtybuffer2 = new bool[Generic.videoram_size[0]];
            for (int i = 0; i < Generic.videoram_size[0]; i++) dirtybuffer2[i] = true;

            tmpbitmap1 = Mame.osd_create_bitmap(32 * 8, 32 * 8);

            return 0;
        }

        public static void rallyx_vh_stop()
        {
            Mame.osd_free_bitmap(tmpbitmap1);
            dirtybuffer2 = null;
            Generic.generic_vh_stop();
        }


    }
}
