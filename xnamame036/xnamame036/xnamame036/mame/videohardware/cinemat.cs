using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class cinemat
    {

        const byte VEC_SHIFT = 16;

        const byte RED = 0x04;
        const byte GREEN = 0x02;
        const byte BLUE = 0x01;
        const byte WHITE = RED | GREEN | BLUE;

        static int cinemat_monitor_type;
        static int cinemat_overlay_req;
        static int cinemat_backdrop_req;
        static int cinemat_screenh;
        static Mame.artwork_element cinemat_simple_overlay;

        static bool color_display;
        static Mame.artwork backdrop;
        static Mame.artwork overlay;
        static Mame.artwork spacewar_panel;
        static Mame.artwork spacewar_pressed_panel;

        Mame.artwork_element[] starcas_overlay =
{
	new Mame.artwork_element(new Mame.rectangle(0, 400-1, 0, 300-1), new byte[]{0, 6, 25}, 64),
	new Mame.artwork_element(new Mame.rectangle( 200, 49, 150, -1),new byte[]{33, 0, 16}, 64),
	new Mame.artwork_element(new Mame.rectangle( 200, 38, 150, -1),new byte[]{32, 15, 0}, 64),
	new Mame.artwork_element(new Mame.rectangle( 200, 29, 150, -1),new byte[]{30, 33, 0}, 64),
	new Mame.artwork_element(new Mame.rectangle(-1,-1,-1,-1),new byte[]{0,0,0},0)
};

        Mame.artwork_element[] tailg_overlay =
{
	new Mame.artwork_element(new Mame.rectangle(0, 400-1, 0, 300-1), new byte[]{0, 64, 64}, 32),
	new Mame.artwork_element(new Mame.rectangle(-1,-1,-1,-1),new byte[]{0,0,0},0)
};

        Mame.artwork_element[] sundance_overlay =
{
	new Mame.artwork_element(new Mame.rectangle(0, 400-1, 0, 300-1), new byte[]{32, 32, 0}, 32),
	new Mame.artwork_element(new Mame.rectangle(-1,-1,-1,-1),new byte[]{0,0,0},0)
};

        Mame.artwork_element[] solarq_overlay =
{
	new Mame.artwork_element(new Mame.rectangle(0, 400-1, 0, 300-1), new byte[]{0, 6, 25}, 64),
	new Mame.artwork_element(new Mame.rectangle( 0,  399, 0,    19),new byte[]{31, 0, 12}, 64),
	new Mame.artwork_element(new Mame.rectangle( 200, 12, 150,  -1),new byte[]{31, 31, 0}, 64),
	new Mame.artwork_element(new Mame.rectangle(-1,-1,-1,-1),new byte[]{0,0,0},0)
};

        public static void cinemat_select_artwork(int monitor_type, int overlay_req, int backdrop_req, Mame.artwork_element simple_overlay)
        {
            cinemat_monitor_type = monitor_type;
            cinemat_overlay_req = overlay_req;
            cinemat_backdrop_req = backdrop_req;
            cinemat_simple_overlay = simple_overlay;
        }
        static void shade_fill(_BytePtr palette, int rgb, int start_index, int end_index, int start_inten, int end_inten)
        {
            int i, inten, index_range, inten_range;

            index_range = end_index - start_index;
            inten_range = end_inten - start_inten;
            for (i = start_index; i <= end_index; i++)
            {
                inten = start_inten + (inten_range) * (i - start_index) / (index_range);
                palette[3 * i] = (rgb & RED) != 0 ? (byte)inten : (byte)0;
                palette[3 * i + 1] = (rgb & GREEN) != 0 ? (byte)inten : (byte)0;
                palette[3 * i + 2] = (rgb & BLUE) != 0 ? (byte)inten : (byte)0;
            }
        }


        public static void cinemat_init_colors(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
        {
            int i, j, k, nextcol;
            string filename = "";

            int[] trcl1 = { 0, 0, 2, 2, 1, 1 };
            int[] trcl2 = { 1, 2, 0, 1, 0, 2 };
            int[] trcl3 = { 2, 1, 1, 0, 2, 0 };

            overlay = null;
            backdrop = null;

            /* initialize the first 8 colors with the basic colors */
            for (i = 0; i < 8; i++)
            {
                palette[3 * i] = (i & RED) != 0 ? (byte)0xff : (byte)0;
                palette[3 * i + 1] = (i & GREEN) != 0 ? (byte)0xff : (byte)0;
                palette[3 * i + 2] = (i & BLUE) != 0 ? (byte)0xff : (byte)0;
            }

            shade_fill(palette, WHITE, 8, 23, 0, 255);
            nextcol = 24;

            /* fill the rest of the 256 color entries depending on the game */
            switch (cinemat_monitor_type)
            {
                case Mame.cpu_ccpu.CCPU_MONITOR_BILEV:
                case Mame.cpu_ccpu.CCPU_MONITOR_16LEV:
                    color_display = false;
                    /* Attempt to load backdrop if requested */
                    if (cinemat_backdrop_req != 0)
                    {
                        filename = Mame.sprintf("%sb.png", Mame.Machine.gamedrv.name);
                        if ((backdrop = Mame.artwork_load(filename, nextcol, (int)(Mame.Machine.drv.total_colors - nextcol))) != null)
                        {
                            for (i = 0; i < 3 * backdrop.num_pens_used; i++)
                                palette[i + 3 * backdrop.start_pen] = backdrop.orig_palette[i];
                            nextcol += backdrop.num_pens_used;
                        }
                    }
                    /* Attempt to load overlay if requested */
                    if (cinemat_overlay_req != 0)
                    {
                        filename = Mame.sprintf("%so.png", Mame.Machine.gamedrv.name);
                        /* Attempt to load artwork from file */
                        overlay = Mame.artwork_load(filename, nextcol, (int)(Mame.Machine.drv.total_colors - nextcol));

                        if ((overlay == null) && (cinemat_simple_overlay != null))
                        {
                            /* no overlay file found - use simple artwork */
                            Mame.artwork_elements_scale(cinemat_simple_overlay,
                                                   Mame.Machine.scrbitmap.width,
                                                   Mame.Machine.scrbitmap.height);
                            overlay = Mame.artwork_create(new Mame.artwork_element[] { cinemat_simple_overlay }, nextcol,
                                                   (int)(Mame.Machine.drv.total_colors - nextcol));
                        }

                        if (overlay != null)
                            Mame.overlay_set_palette(overlay, palette, (int)(Mame.Machine.drv.total_colors - nextcol));
                    }
                    break;

                case Mame.cpu_ccpu.CCPU_MONITOR_WOWCOL:
                    color_display = true;
                    /* TODO: support real color */
                    /* put in 40 shades for red, blue and magenta */
                    shade_fill(palette, RED, 8, 47, 10, 250);
                    shade_fill(palette, BLUE, 48, 87, 10, 250);
                    shade_fill(palette, RED | BLUE, 88, 127, 10, 250);

                    /* put in 20 shades for yellow and green */
                    shade_fill(palette, GREEN, 128, 147, 10, 250);
                    shade_fill(palette, RED | GREEN, 148, 167, 10, 250);

                    /* and 14 shades for cyan and white */
                    shade_fill(palette, BLUE | GREEN, 168, 181, 10, 250);
                    shade_fill(palette, WHITE, 182, 194, 10, 250);

                    /* Fill in unused gaps with more anti-aliasing colors. */
                    /* There are 60 slots available.           .ac JAN2498 */
                    i = 195;
                    for (j = 0; j < 6; j++)
                    {
                        for (k = 7; k <= 16; k++)
                        {
                            palette[3 * i + trcl1[j]] = (byte)(((256 * k) / 16) - 1);
                            palette[3 * i + trcl2[j]] = (byte)(((128 * k) / 16) - 1);
                            palette[3 * i + trcl3[j]] = 0;
                            i++;
                        }
                    }
                    break;
            }
        }
        public static int cinemat_vh_start()
        {
            Vector.vector_set_shift(VEC_SHIFT);

            if (backdrop != null)
            {
                Mame.backdrop_refresh(backdrop);
                Mame.backdrop_refresh_tables(backdrop);
            }

            if (overlay != null) Mame.overlay_remap(overlay);
            cinemat_screenh = Mame.Machine.drv.visible_area.max_y - Mame.Machine.drv.visible_area.min_y;
            return Vector.vector_vh_start();
        }
        public static void cinemat_vh_stop()
        {
            if (backdrop != null) Mame.artwork_free(ref backdrop);
            if (overlay != null) Mame.artwork_free(ref overlay);
            Vector.vector_vh_stop();
        }
        static int lastx, lasty;
        public static void CinemaVectorData(int fromx, int fromy, int tox, int toy, int color)
        {

            fromy = cinemat_screenh - fromy;
            toy = cinemat_screenh - toy;

            if (fromx != lastx || fromx != lasty)
                Vector.vector_add_point(fromx << VEC_SHIFT, fromy << VEC_SHIFT, 0, 0);

            if (color_display)
                Vector.vector_add_point(tox << VEC_SHIFT, toy << VEC_SHIFT, color & 0x07, (color & 0x08) != 0 ? 0x80 : 0x40);
            else
                Vector.vector_add_point(tox << VEC_SHIFT, toy << VEC_SHIFT, WHITE, color * 12);

            lastx = tox;
            lasty = toy;
        }
        public static void cinemat_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            if (backdrop != null && overlay != null)
                Vector.vector_vh_update_artwork(bitmap, overlay, backdrop, full_refresh);
            else if (overlay != null)
                Vector.vector_vh_update_overlay(bitmap, overlay, full_refresh);
            else if (backdrop != null)
                Vector.vector_vh_update_backdrop(bitmap, backdrop, full_refresh);
            else
                Vector.vector_vh_update(bitmap, full_refresh);
            Vector.vector_clear_list();
        }

    }
}
