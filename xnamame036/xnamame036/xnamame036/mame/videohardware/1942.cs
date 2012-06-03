using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_1942 
    {

        public static _BytePtr c1942_backgroundram = new _BytePtr();
        public static int[] c1942_backgroundram_size = new int[1];
        public static _BytePtr c1942_scroll = new _BytePtr();
        public static _BytePtr c1942_palette_bank = new _BytePtr();
        public static byte[] dirtybuffer2;
        public static Mame.osd_bitmap tmpbitmap2;
        public static bool flipscreen;

           public static void c1942_vh_convert_color_prom(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
        {

            uint cpi = 0;
            int pi = 0;
            for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
            {
                /* red component */
                int bit0 = (color_prom[cpi] >> 0) & 0x01;
                int bit1 = (color_prom[cpi] >> 1) & 0x01;
                int bit2 = (color_prom[cpi] >> 2) & 0x01;
                int bit3 = (color_prom[cpi] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* green component */
                bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* blue component */
                bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                cpi++;
            }

            cpi += 2 * Mame.Machine.drv.total_colors;
            /* color_prom now points to the beginning of the lookup table */


            /* characters use colors 128-143 */
            for (int i = 0; i < (Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity); i++)
                COLOR(colortable, 0, i, (ushort)(((color_prom[cpi++]) & 0x0f) + 128));

            /* background tiles use colors 0-63 in four banks */
            for (int i = 0; i < (Mame.Machine.gfx[1].total_colors * Mame.Machine.gfx[1].color_granularity) / 4; i++)
            {
                COLOR(colortable, 1, i, (ushort)(((color_prom[cpi]) & 0x0f)));
                COLOR(colortable, 1, i + 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 16));
                COLOR(colortable, 1, i + 2 * 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 32));
                COLOR(colortable, 1, i + 3 * 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 48));
                cpi++;
            }

            /* sprites use colors 64-79 */
            for (int i = 0; i < (Mame.Machine.gfx[2].total_colors * Mame.Machine.gfx[2].color_granularity); i++)
                COLOR(colortable, 2, i, (ushort)(((color_prom[cpi++]) & 0x0f) + 64));
        }
        public static int c1942_vh_start()
        {
            if (Generic.generic_vh_start() != 0)
                return 1;

            dirtybuffer2 = new byte[c1942_backgroundram_size[0]];
            for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                dirtybuffer2[i] = 1;

            /* the background area is twice as wide as the screen (actually twice as tall, */
            /* because this is a vertical game) */
            tmpbitmap2 = Mame.osd_create_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

            return 0;
        }

        public static void c1942_vh_stop()
        {
            Mame.osd_free_bitmap(tmpbitmap2);
            dirtybuffer2 = null;
            Generic.generic_vh_stop();
        }
        public static void c1942_background_w(int offset, int data)
        {
            if (c1942_backgroundram[offset] != data)
            {
                dirtybuffer2[offset] = 1;

                c1942_backgroundram[offset] = (byte)data;
            }
        }
        public static void c1942_palette_bank_w(int offset, int data)
        {
            if (c1942_palette_bank[0] != data)
            {
                for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                    dirtybuffer2[i] = 1;
                c1942_palette_bank[0] = (byte)data;
            }
        }
        public static void c1942_flipscreen_w(int offset, int data)
        {
            if (flipscreen != ((data & 0x80)!=0))
            {
                flipscreen = (data & 0x80) != 0;
                for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                    dirtybuffer2[i] = 1;
            }
        }
        
    }
}
