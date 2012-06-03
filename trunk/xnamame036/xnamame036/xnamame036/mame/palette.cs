using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const int DYNAMIC_MAX_PENS = 254;
        public const int STATIC_MAX_PENS = 256;
        public const int PALETTE_COLOR_UNUSED = 0;/* This color is not needed for this frame */
        public const int PALETTE_COLOR_VISIBLE = 1;	/* This color is currently visible */
        public const int PALETTE_COLOR_CACHED = 2;	/* This color is cached in temporary bitmaps */
        /* palette_recalc() will try to use always the same pens for the used colors; */
        /* if it is forced to rearrange the pens, it will return TRUE to signal the */
        /* driver that it must refresh the display. */
        public const int PALETTE_COLOR_TRANSPARENT_FLAG = 4;/* All colors using this attribute will be */
        /* mapped to the same pen, and no other colors will be mapped to that pen. */
        /* This way, transparencies can be handled by copybitmap(). */

        /* backwards compatibility */
        public const int PALETTE_COLOR_USED = (PALETTE_COLOR_VISIBLE | PALETTE_COLOR_CACHED);
        public const int PALETTE_COLOR_TRANSPARENT = (PALETTE_COLOR_TRANSPARENT_FLAG | PALETTE_COLOR_USED);

        static byte[][][] rgb6_to_pen = null;

        static _BytePtr game_palette;	/* RGB palette as set by the driver. */
        static _BytePtr new_palette;	/* changes to the palette are stored here before */
        /* being moved to game_palette by palette_recalc() */
        static _BytePtr palette_dirty;
        /* arrays which keep track of colors actually used, to help in the palette shrinking. */
        public static _BytePtr palette_used_colors;
        static _BytePtr old_used_colors;
        static _IntPtr pen_visiblecount, pen_cachedcount;
        static _BytePtr just_remapped;	/* colors which have been remapped in this frame, */
        /* returned by palette_recalc() */

        static int use_16bit;
        const int NO_16BIT = 0;
        const int STATIC_16BIT = 1;
        const int PALETTIZED_16BIT = 2;

        static int total_shrinked_pens;
        /// <summary>
        /// Access as shorts
        /// </summary>
        static _ShortPtr shrinked_pens;
        static _BytePtr shrinked_palette;
        /// <summary>
        /// access as shorts
        /// </summary>
        static _ShortPtr palette_map;	/* map indexes from game_palette to shrinked_palette */
        /// <summary>
        /// access as shorts
        /// </summary>
        static ushort[] pen_usage_count = new ushort[DYNAMIC_MAX_PENS];

        public static ushort palette_transparent_pen;
        public static int palette_transparent_color;


        const int BLACK_PEN = 0;
        const int TRANSPARENT_PEN = 1;
        const int RESERVED_PENS = 2;

        const int PALETTE_COLOR_NEEDS_REMAP = 0x80;

        public static _ShortPtr palette_shadow_table;
        public static _ShortPtr palette_highlight_table;

        static int palette_start()
        {
            int num;

            game_palette = new _BytePtr((int)(3 * Machine.drv.total_colors * sizeof(byte)));
            palette_map = new _ShortPtr((int)(Machine.drv.total_colors * sizeof(ushort)));
            if (Machine.drv.color_table_len != 0)
            {
                Machine.game_colortable = new _ShortPtr((int)Machine.drv.color_table_len * sizeof(ushort));
                Machine.remapped_colortable = new _ShortPtr((int)Machine.drv.color_table_len * sizeof(ushort));
            }
            else Machine.game_colortable = Machine.remapped_colortable = null;

            if (Machine.color_depth == 16 || (Machine.gamedrv.flags & GAME_REQUIRES_16BIT) != 0)
            {
                if (Machine.color_depth == 8 || Machine.drv.total_colors > 65532)
                    use_16bit = STATIC_16BIT;
                else
                    use_16bit = PALETTIZED_16BIT;
            }
            else
                use_16bit = NO_16BIT;

            switch (use_16bit)
            {
                case NO_16BIT:
                    if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
                        total_shrinked_pens = DYNAMIC_MAX_PENS;
                    else
                        total_shrinked_pens = STATIC_MAX_PENS;
                    break;
                case STATIC_16BIT:
                    total_shrinked_pens = 32768;
                    break;
                case PALETTIZED_16BIT:
                    total_shrinked_pens = (int)(Machine.drv.total_colors + RESERVED_PENS);
                    break;
            }

            shrinked_pens = new _ShortPtr((total_shrinked_pens * sizeof(short)));
            shrinked_palette = new _BytePtr((3 * total_shrinked_pens * sizeof(byte)));

            Machine.pens = new _ShortPtr((int)(Machine.drv.total_colors * sizeof(short)));

            if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
            {
                /* if the palette changes dynamically, */
                /* we'll need the usage arrays to help in shrinking. */
                palette_used_colors = new _BytePtr(((int)((1 + 1 + 1 + 3 + 1) * Machine.drv.total_colors * sizeof(byte))));
                pen_visiblecount = new _IntPtr((int)(2 * Machine.drv.total_colors * sizeof(int)));

                if (palette_used_colors == null || pen_visiblecount == null)
                {
                    palette_stop();
                    return 1;
                }

                old_used_colors = new _BytePtr(palette_used_colors, (int)Machine.drv.total_colors * sizeof(byte));
                just_remapped = new _BytePtr(old_used_colors, (int)Machine.drv.total_colors * sizeof(byte));
                new_palette = new _BytePtr(just_remapped, (int)Machine.drv.total_colors * sizeof(byte));
                palette_dirty = new _BytePtr(new_palette, 3 * (int)Machine.drv.total_colors * sizeof(byte));
                memset(palette_used_colors, PALETTE_COLOR_USED, (int)Machine.drv.total_colors * sizeof(byte));
                memset(old_used_colors, PALETTE_COLOR_UNUSED, (int)Machine.drv.total_colors * sizeof(byte));
                memset(palette_dirty, 0, (int)(Machine.drv.total_colors * sizeof(byte)));
                pen_cachedcount = new _IntPtr(pen_visiblecount, (int)Machine.drv.total_colors * sizeof(uint));
                for (int i = 0; i < Machine.drv.total_colors; i++)
                {
                    pen_visiblecount.write32(i, 0);
                    pen_cachedcount.write32(i, 0);
                }
            }
            else palette_used_colors = old_used_colors = just_remapped = new_palette = palette_dirty = null;

            if (Machine.color_depth == 8) num = 256;
            else num = 65536;
            palette_shadow_table = new _ShortPtr(2 * num * sizeof(ushort));
            if (palette_shadow_table == null)
            {
                palette_stop();
                return 1;
            }
            palette_highlight_table = new _ShortPtr(palette_shadow_table, num * sizeof(short));
            for (int i = 0; i < num; i++)
            {
                palette_shadow_table.write16(i, (ushort)i);
                palette_highlight_table.write16(i, (ushort)i);
            }

            if ((Machine.drv.color_table_len != 0 && (Machine.game_colortable == null || Machine.remapped_colortable == null))
                    || game_palette == null || palette_map == null
                    || shrinked_pens == null || shrinked_palette == null || Machine.pens == null)
            {
                palette_stop();
                return 1;
            }

            return 0;
        }
        static void palette_stop()
        {
            //free(palette_used_colors);
            palette_used_colors = old_used_colors = just_remapped = new_palette = palette_dirty = null;
            //free(pen_visiblecount);
            pen_visiblecount = null;
            //free(game_palette);
            game_palette = null;
            //free(palette_map);
            palette_map = null;
            //free(Machine.game_colortable);
            Machine.game_colortable = null;
            //free(Machine.remapped_colortable);
            Machine.remapped_colortable = null;
            //	free(shrinked_pens);
            shrinked_pens = null;
            //	free(shrinked_palette);
            shrinked_palette = null;
            //free(Machine.pens);
            Machine.pens = null;
            //free(palette_shadow_table);
            palette_shadow_table = null;
        }
        static void create_rgb_pen()
        {
            if (rgb6_to_pen != null) return;
            rgb6_to_pen = new byte[64][][];
            for (int i = 0; i < 64; i++)
            {
                rgb6_to_pen[i] = new byte[64][];
                for (int j = 0; j < 64; j++)
                {
                    rgb6_to_pen[i][j] = new byte[64];
                    for (int l = 0; l < 64; l++)
                        rgb6_to_pen[i][j][l] = DYNAMIC_MAX_PENS;
                }
            }
        }
        static void build_rgb_to_pen()
        {
            int rr, gg, bb;
            create_rgb_pen();


            //memset(rgb6_to_pen, DYNAMIC_MAX_PENS, sizeof(rgb6_to_pen));
            rgb6_to_pen[0][0][0] = BLACK_PEN;

            for (int i = 0; i < DYNAMIC_MAX_PENS; i++)
            {
                if (pen_usage_count[i] > 0)
                {
                    rr = shrinked_palette[3 * i + 0] >> 2;
                    gg = shrinked_palette[3 * i + 1] >> 2;
                    bb = shrinked_palette[3 * i + 2] >> 2;

                    if (rgb6_to_pen[rr][gg][bb] == DYNAMIC_MAX_PENS)
                    {
                        int j, max;

                        rgb6_to_pen[rr][gg][bb] = (byte)i;
                        max = pen_usage_count[i];

                        /* to reduce flickering during remaps, find the pen used by most colors */
                        for (j = i + 1; j < DYNAMIC_MAX_PENS; j++)
                        {
                            if (pen_usage_count[j] > max &&
                                    rr == (shrinked_palette[3 * j + 0] >> 2) &&
                                    gg == (shrinked_palette[3 * j + 1] >> 2) &&
                                    bb == (shrinked_palette[3 * j + 2] >> 2))
                            {
                                rgb6_to_pen[rr][gg][bb] = (byte)j;
                                max = pen_usage_count[j];
                            }
                        }
                    }
                }
            }
        }
        static int rgbpenindex(byte r, byte g, byte b)
        {
            return ((Machine.scrbitmap.depth == 16) ? ((((r) >> 3) << 10) + (((g) >> 3) << 5) + ((b) >> 3)) : ((((r) >> 5) << 5) + (((g) >> 5) << 2) + ((b) >> 6)));
        }

        static int palette_init()
        {
            /* We initialize the palette and colortable to some default values so that */
            /* drivers which dynamically change the palette don't need a vh_init_palette() */
            /* function (provided the default color table fits their needs). */

            for (int i = 0; i < Machine.drv.total_colors; i++)
            {
                game_palette[3 * i + 0] = (byte)(((i & 1) >> 0) * 0xff);
                game_palette[3 * i + 1] = (byte)(((i & 2) >> 1) * 0xff);
                game_palette[3 * i + 2] = (byte)(((i & 4) >> 2) * 0xff);
            }

            /* Preload the colortable with a default setting, following the same */
            /* order of the palette. The driver can overwrite this in */
            /* vh_init_palette() */
            for (int i = 0; i < Machine.drv.color_table_len; i++)
                Machine.game_colortable.write16(i, (ushort)(i % Machine.drv.total_colors));

            /* by default we use -1 to identify the transparent color, the driver */
            /* can modify this. */
            palette_transparent_color = -1;

            /* now the driver can modify the default values if it wants to. */
            Machine.drv.vh_init_palette(game_palette, Machine.game_colortable, memory_region(REGION_PROMS));

            switch (use_16bit)
            {
                case NO_16BIT:
                    {
                        /* initialize shrinked palette to all black */
                        for (int i = 0; i < total_shrinked_pens; i++)
                        {
                            shrinked_palette[3 * i + 0] = 0;
                            shrinked_palette[3 * i + 1] = 0;
                            shrinked_palette[3 * i + 2] = 0;
                        }

                        if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) != 0)
                        {
                            /* initialize pen usage counters */
                            for (int i = 0; i < DYNAMIC_MAX_PENS; i++)
                                pen_usage_count[i] = 0;

                            /* allocate two fixed pens at the beginning: */
                            /* transparent black */
                            pen_usage_count[TRANSPARENT_PEN] = 1;	/* so the pen will not be reused */

                            /* non transparent black */
                            pen_usage_count[BLACK_PEN] = 1;

                            /* create some defaults associations of game colors to shrinked pens. */
                            /* They will be dynamically modified at run time. */
                            for (int i = 0; i < Machine.drv.total_colors; i++)
                                palette_map.write16(i, (ushort)((i & 7) + 8));

                            if (osd_allocate_colors((uint)total_shrinked_pens, shrinked_palette, shrinked_pens, 1) != 0)
                                return 1;
                        }
                        else
                        {
                            int j, used;

                            printf("shrinking %d colors palette...\n", Machine.drv.total_colors);

                            /* shrink palette to fit */
                            used = 0;

                            for (int i = 0; i < Machine.drv.total_colors; i++)
                            {
                                for (j = 0; j < used; j++)
                                {
                                    if (shrinked_palette[3 * j + 0] == game_palette[3 * i + 0] &&
                                            shrinked_palette[3 * j + 1] == game_palette[3 * i + 1] &&
                                            shrinked_palette[3 * j + 2] == game_palette[3 * i + 2])
                                        break;
                                }

                                palette_map.write16(i, (ushort)j);

                                if (j == used)
                                {
                                    used++;
                                    if (used > total_shrinked_pens)
                                    {
                                        used = total_shrinked_pens;
                                        palette_map.write16(i, (ushort)(total_shrinked_pens - 1));
                                        usrintf_showmessage("cannot shrink static palette");
                                        printf("error: ran out of free pens to shrink the palette.\n");
                                    }
                                    else
                                    {
                                        shrinked_palette[3 * j + 0] = game_palette[3 * i + 0];
                                        shrinked_palette[3 * j + 1] = game_palette[3 * i + 1];
                                        shrinked_palette[3 * j + 2] = game_palette[3 * i + 2];
                                    }
                                }
                            }

                            printf("shrinked palette uses %d colors\n", used);

                            if (osd_allocate_colors((uint)used, shrinked_palette, shrinked_pens, 0) != 0)
                                return 1;
                        }


                        for (int i = 0; i < Machine.drv.total_colors; i++)
                            Machine.pens.write16(i, shrinked_pens.read16(palette_map.read16(i)));

                        palette_transparent_pen = shrinked_pens.read16(TRANSPARENT_PEN);	/* for dynamic palette games */
                    }
                    break;

                case STATIC_16BIT:
                    {
                        _BytePtr p = new _BytePtr(shrinked_palette.buffer, shrinked_palette.offset);

                        if (Machine.scrbitmap.depth == 16)
                        {
                            for (int r = 0; r < 32; r++)
                            {
                                for (int g = 0; g < 32; g++)
                                {
                                    for (int b = 0; b < 32; b++)
                                    {
                                        p[p.offset++] = (byte)((r << 3) | (r >> 2));
                                        p[p.offset++] = (byte)((g << 3) | (g >> 2));
                                        p[p.offset++] = (byte)((b << 3) | (b >> 2));
                                    }
                                }
                            }

                            if (osd_allocate_colors(32768, shrinked_palette, shrinked_pens, 0) != 0)
                                return 1;
                        }
                        else
                        {
                            for (int r = 0; r < 8; r++)
                            {
                                for (int g = 0; g < 8; g++)
                                {
                                    for (int b = 0; b < 4; b++)
                                    {
                                        p[p.offset++] = (byte)((r << 5) | (r << 2) | (r >> 1));
                                        p[p.offset++] = (byte)((g << 5) | (g << 2) | (g >> 1));
                                        p[p.offset++] = (byte)((b << 6) | (b << 4) | (b << 2) | b);
                                    }
                                }
                            }

                            if (osd_allocate_colors(256, shrinked_palette, shrinked_pens, 0) != 0)
                                return 1;
                        }

                        for (int i = 0; i < Machine.drv.total_colors; i++)
                        {
                            byte r = game_palette[3 * i + 0];
                            byte g = game_palette[3 * i + 1];
                            byte b = game_palette[3 * i + 2];

                            Machine.pens.write16(i, shrinked_pens.read16(rgbpenindex(r, g, b)));
                        }

                        palette_transparent_pen = shrinked_pens.read16(0);	/* we are forced to use black for the transparent pen */
                    }
                    break;

                case PALETTIZED_16BIT:
                    {
                        for (int i = 0; i < RESERVED_PENS; i++)
                        {
                            shrinked_palette[3 * i + 0] =
                            shrinked_palette[3 * i + 1] =
                            shrinked_palette[3 * i + 2] = 0;
                        }

                        for (int i = 0; i < Machine.drv.total_colors; i++)
                        {
                            shrinked_palette[3 * (i + RESERVED_PENS) + 0] = game_palette[3 * i + 0];
                            shrinked_palette[3 * (i + RESERVED_PENS) + 1] = game_palette[3 * i + 1];
                            shrinked_palette[3 * (i + RESERVED_PENS) + 2] = game_palette[3 * i + 2];
                        }

                        if (osd_allocate_colors((uint)total_shrinked_pens, shrinked_palette, shrinked_pens, (Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE)) != 0)
                            return 1;

                        for (int i = 0; i < Machine.drv.total_colors; i++)
                            Machine.pens.write16(i, shrinked_pens.read16(i + RESERVED_PENS));

                        palette_transparent_pen = shrinked_pens.read16(TRANSPARENT_PEN);	/* for dynamic palette games */
                    }
                    break;
            }

            for (int i = 0; i < Machine.drv.color_table_len; i++)
            {
                int color = Machine.game_colortable.read16(i);

                /* check for invalid colors set by Machine.drv.vh_init_palette */
                if (color < Machine.drv.total_colors)
                    Machine.remapped_colortable.write16(i, Machine.pens.read16(color));
                else
                    usrintf_showmessage("colortable[%d] (=%d) out of range (total_colors = %d)",
                            i, color, Machine.drv.total_colors);
            }

            return 0;
        }
        static int compress_palette()
        {
            int i, j, saved, r, g, b;


            build_rgb_to_pen();

            saved = 0;

            for (i = 0; i < Machine.drv.total_colors; i++)
            {
                /* merge pens of the same color */
                if ((old_used_colors[i] & PALETTE_COLOR_VISIBLE) != 0 &&
                        (old_used_colors[i] & (PALETTE_COLOR_NEEDS_REMAP | PALETTE_COLOR_TRANSPARENT_FLAG)) == 0)
                {
                    r = game_palette[3 * i + 0] >> 2;
                    g = game_palette[3 * i + 1] >> 2;
                    b = game_palette[3 * i + 2] >> 2;

                    j = rgb6_to_pen[r][g][b];

                    if (palette_map.read16(i) != j)
                    {
                        just_remapped[i] = 1;

                        pen_usage_count[palette_map.read16(i)]--;
                        if (pen_usage_count[palette_map.read16(i)] == 0)
                            saved++;
                        palette_map.write16(i, (ushort)j);
                        pen_usage_count[palette_map.read16(i)]++;
                        Machine.pens.write16(i, shrinked_pens.read16(palette_map.read16(i)));
                    }
                }
            }

#if VERBOSE
if (errorlog)
{
	int subcount[8];


	for (i = 0;i < 8;i++)
		subcount[i] = 0;

	for (i = 0;i < Machine.drv.total_colors;i++)
		subcount[palette_used_colors[i]]++;

	fprintf(errorlog,"Ran out of pens! %d colors used (%d unused, %d visible %d cached %d visible+cached, %d transparent)\n",
			subcount[PALETTE_COLOR_VISIBLE]+subcount[PALETTE_COLOR_CACHED]+subcount[PALETTE_COLOR_VISIBLE|PALETTE_COLOR_CACHED]+subcount[PALETTE_COLOR_TRANSPARENT],
			subcount[PALETTE_COLOR_UNUSED],
			subcount[PALETTE_COLOR_VISIBLE],
			subcount[PALETTE_COLOR_CACHED],
			subcount[PALETTE_COLOR_VISIBLE|PALETTE_COLOR_CACHED],
			subcount[PALETTE_COLOR_TRANSPARENT]);
	fprintf(errorlog,"Compressed the palette, saving %d pens\n",saved);
}
#endif

            return saved;
        }
        static _BytePtr palette_recalc_8()
        {
            int i, color;
            int did_remap = 0;
            int need_refresh = 0;
            int first_free_pen;
            int ran_out = 0;
            int reuse_pens = 0;
            int need, avail;

            create_rgb_pen();

            Array.Clear(just_remapped.buffer, (int)just_remapped.offset, (int)Machine.drv.total_colors);
            //memset(just_remapped,0,Machine.drv.total_colors * sizeof(unsigned char));

            /* first of all, apply the changes to the palette which were */
            /* requested since last update */
            for (color = 0; color < Machine.drv.total_colors; color++)
            {
                if (palette_dirty[color] != 0)
                {
                    int pen = palette_map.read16(color);
                    byte r = new_palette[3 * color + 0];
                    byte g = new_palette[3 * color + 1];
                    byte b = new_palette[3 * color + 2];

                    /* if the color maps to an exclusive pen, just change it */
                    if (pen_usage_count[pen] == 1)
                    {
                        palette_dirty[color] = 0;
                        game_palette[3 * color + 0] = r;
                        game_palette[3 * color + 1] = g;
                        game_palette[3 * color + 2] = b;

                        shrinked_palette[3 * pen + 0] = r;
                        shrinked_palette[3 * pen + 1] = g;
                        shrinked_palette[3 * pen + 2] = b;
                        osd_modify_pen(Machine.pens.read16(color), r, g, b);
                    }
                    else
                    {
                        if (pen < RESERVED_PENS)
                        {
                            /* the color uses a reserved pen, the only thing we can do is remap it */
                            for (i = color; i < Machine.drv.total_colors; i++)
                            {
                                if (palette_dirty[i] != 0 && palette_map.read16(i) == pen)
                                {
                                    palette_dirty[i] = 0;
                                    game_palette[3 * i + 0] = new_palette[3 * i + 0];
                                    game_palette[3 * i + 1] = new_palette[3 * i + 1];
                                    game_palette[3 * i + 2] = new_palette[3 * i + 2];
                                    old_used_colors[i] |= PALETTE_COLOR_NEEDS_REMAP;
                                }
                            }
                        }
                        else
                        {
                            /* the pen is shared with other colors, let's see if all of them */
                            /* have been changed to the same value */
                            for (i = 0; i < Machine.drv.total_colors; i++)
                            {
                                if ((old_used_colors[i] & PALETTE_COLOR_VISIBLE) != 0 &&
                                        palette_map.read16(i) == pen)
                                {
                                    if (palette_dirty[i] == 0 ||
                                            new_palette[3 * i + 0] != r ||
                                            new_palette[3 * i + 1] != g ||
                                            new_palette[3 * i + 2] != b)
                                        break;
                                }
                            }

                            if (i == Machine.drv.total_colors)
                            {
                                /* all colors sharing this pen still are the same, so we */
                                /* just change the palette. */
                                shrinked_palette[3 * pen + 0] = r;
                                shrinked_palette[3 * pen + 1] = g;
                                shrinked_palette[3 * pen + 2] = b;
                                osd_modify_pen(Machine.pens.read16(color), r, g, b);

                                for (i = color; i < Machine.drv.total_colors; i++)
                                {
                                    if (palette_dirty[i] != 0 && palette_map.read16(i) == pen)
                                    {
                                        palette_dirty[i] = 0;
                                        game_palette[3 * i + 0] = r;
                                        game_palette[3 * i + 1] = g;
                                        game_palette[3 * i + 2] = b;
                                    }
                                }
                            }
                            else
                            {
                                /* the colors sharing this pen now are different, we'll */
                                /* have to remap them. */
                                for (i = color; i < Machine.drv.total_colors; i++)
                                {
                                    if (palette_dirty[i] != 0 && palette_map.read16(i) == pen)
                                    {
                                        palette_dirty[i] = 0;
                                        game_palette[3 * i + 0] = new_palette[3 * i + 0];
                                        game_palette[3 * i + 1] = new_palette[3 * i + 1];
                                        game_palette[3 * i + 2] = new_palette[3 * i + 2];
                                        old_used_colors[i] |= PALETTE_COLOR_NEEDS_REMAP;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            need = 0;
            for (i = 0; i < Machine.drv.total_colors; i++)
            {
                if ((palette_used_colors[i] & PALETTE_COLOR_VISIBLE) != 0 && palette_used_colors[i] != old_used_colors[i])
                    need++;
            }
            if (need > 0)
            {
                avail = 0;
                for (i = 0; i < DYNAMIC_MAX_PENS; i++)
                {
                    if (pen_usage_count[i] == 0)
                        avail++;
                }

                if (need > avail)
                {
#if VERBOSE
if (errorlog) fprintf(errorlog,"Need %d new pens; %d available. I'll reuse some pens.\n",need,avail);
#endif
                    reuse_pens = 1;
                    build_rgb_to_pen();
                }
            }

            first_free_pen = RESERVED_PENS;
            for (color = 0; color < Machine.drv.total_colors; color++)
            {
                /* the comparison between palette_used_colors and old_used_colors also includes */
                /* PALETTE_COLOR_NEEDS_REMAP which might have been set previously */
                if ((palette_used_colors[color] & PALETTE_COLOR_VISIBLE) != 0 &&
                        palette_used_colors[color] != old_used_colors[color])
                {
                    byte r, g, b;


                    if ((old_used_colors[color] & PALETTE_COLOR_VISIBLE) != 0)
                    {
                        pen_usage_count[palette_map.read16(color)]--;
                        old_used_colors[color] &= unchecked((byte)~PALETTE_COLOR_VISIBLE);
                    }

                    r = game_palette[3 * color + 0];
                    g = game_palette[3 * color + 1];
                    b = game_palette[3 * color + 2];

                    if ((palette_used_colors[color] & PALETTE_COLOR_TRANSPARENT_FLAG) != 0)
                    {
                        if (palette_map.read16(color) != TRANSPARENT_PEN)
                        {
                            /* use the fixed transparent black for this */
                            did_remap = 1;
                            if ((old_used_colors[color] & palette_used_colors[color] & PALETTE_COLOR_CACHED) != 0)
                            {
                                /* the color was and still is cached, we'll have to redraw everything */
                                need_refresh = 1;
                                just_remapped[color] = 1;
                            }

                            palette_map.write16(color, TRANSPARENT_PEN);
                        }
                        pen_usage_count[palette_map.read16(color)]++;
                        Machine.pens.write16(color, shrinked_pens.read16(palette_map.read16(color)));
                        old_used_colors[color] = palette_used_colors[color];
                    }
                    else
                    {
                        if (reuse_pens != 0)
                        {
                            i = rgb6_to_pen[r >> 2][g >> 2][b >> 2];
                            if (i != DYNAMIC_MAX_PENS)
                            {
                                if (palette_map.read16(color) != (ushort)i)
                                {
                                    did_remap = 1;
                                    if ((old_used_colors[color] & palette_used_colors[color] & PALETTE_COLOR_CACHED) != 0)
                                    {
                                        /* the color was and still is cached, we'll have to redraw everything */
                                        need_refresh = 1;
                                        just_remapped[color] = 1;
                                    }

                                    palette_map.write16(color, (ushort)i);
                                }
                                pen_usage_count[palette_map.read16(color)]++;
                                Machine.pens.write16(color, shrinked_pens.read16(palette_map.read16(color)));
                                old_used_colors[color] = palette_used_colors[color];
                            }
                        }

                        /* if we still haven't found a pen, choose a new one */
                        if (old_used_colors[color] != palette_used_colors[color])
                        {
                            /* if possible, reuse the last associated pen */
                            if (pen_usage_count[palette_map.read16(color)] == 0)
                            {
                                pen_usage_count[palette_map.read16(color)]++;
                            }
                            else	/* allocate a new pen */
                            {
                            retry:
                                while (first_free_pen < DYNAMIC_MAX_PENS && pen_usage_count[first_free_pen] > 0)
                                    first_free_pen++;

                                if (first_free_pen < DYNAMIC_MAX_PENS)
                                {
                                    did_remap = 1;
                                    if ((old_used_colors[color] & palette_used_colors[color] & PALETTE_COLOR_CACHED) != 0)
                                    {
                                        /* the color was and still is cached, we'll have to redraw everything */
                                        need_refresh = 1;
                                        just_remapped[color] = 1;
                                    }

                                    palette_map.write16(color, (ushort)first_free_pen);
                                    pen_usage_count[palette_map.read16(color)]++;
                                    Machine.pens.write16(color, shrinked_pens.read16(palette_map.read16(color)));
                                }
                                else
                                {
                                    /* Ran out of pens! Let's see what we can do. */

                                    if (ran_out == 0)
                                    {
                                        ran_out++;

                                        /* from now on, try to reuse already allocated pens */
                                        reuse_pens = 1;
                                        if (compress_palette() > 0)
                                        {
                                            did_remap = 1;
                                            need_refresh = 1;	/* we'll have to redraw everything */

                                            first_free_pen = RESERVED_PENS;
                                            goto retry;
                                        }
                                    }

                                    ran_out++;

                                    /* we failed, but go on with the loop, there might */
                                    /* be some transparent pens to remap */

                                    continue;
                                }
                            }

                            {
                                int rr, gg, bb;

                                i = palette_map.read16(color);
                                rr = shrinked_palette[3 * i + 0] >> 2;
                                gg = shrinked_palette[3 * i + 1] >> 2;
                                bb = shrinked_palette[3 * i + 2] >> 2;
                                if (rgb6_to_pen[rr][gg][bb] == i)
                                    rgb6_to_pen[rr][gg][bb] = DYNAMIC_MAX_PENS;

                                shrinked_palette[3 * i + 0] = r;
                                shrinked_palette[3 * i + 1] = g;
                                shrinked_palette[3 * i + 2] = b;
                                osd_modify_pen(Machine.pens.read16(color), r, g, b);

                                r >>= 2;
                                g >>= 2;
                                b >>= 2;
                                if (rgb6_to_pen[r][g][b] == DYNAMIC_MAX_PENS)
                                    rgb6_to_pen[r][g][b] = (byte)i;
                            }

                            old_used_colors[color] = palette_used_colors[color];
                        }
                    }
                }
            }

            if (ran_out > 1)
            {

                printf("Error: no way to shrink the palette to 256 colors, left out %d colors.\n", ran_out - 1);

            }

            /* Reclaim unused pens; we do this AFTER allocating the new ones, to avoid */
            /* using the same pen for two different colors in two consecutive frames, */
            /* which might cause flicker. */
            for (color = 0; color < Machine.drv.total_colors; color++)
            {
                if ((palette_used_colors[color] & PALETTE_COLOR_VISIBLE) == 0)
                {
                    if ((old_used_colors[color] & PALETTE_COLOR_VISIBLE) != 0)
                        pen_usage_count[palette_map.read16(color)]--;
                    old_used_colors[color] = palette_used_colors[color];
                }
            }

#if PEDANTIC
	/* invalidate unused pens to make bugs in color allocation evident. */
	for (i = 0;i < DYNAMIC_MAX_PENS;i++)
	{
		if (pen_usage_count[i] == 0)
		{
			int r,g,b;
			r = rand() & 0xff;
			g = rand() & 0xff;
			b = rand() & 0xff;
			shrinked_palette[3*i + 0] = r;
			shrinked_palette[3*i + 1] = g;
			shrinked_palette[3*i + 2] = b;
			osd_modify_pen(shrinked_pens[i],r,g,b);
		}
	}
#endif

            if (did_remap != 0)
            {
                /* rebuild the color lookup table */
                for (i = 0; i < Machine.drv.color_table_len; i++)
                    Machine.remapped_colortable.write16(i, Machine.pens.read16(Machine.game_colortable.read16(i)));
            }

            if (need_refresh != 0)
            {

                return just_remapped;
            }
            else return null;
        }
        static _BytePtr palette_recalc_16_static()
        {
            throw new Exception();
        }
        static _BytePtr palette_recalc_16_palettized()
        {
            throw new Exception();
        }

        public static _BytePtr palette_recalc()
        {
            /* if we are not dynamically reducing the palette, return immediately. */
            if (palette_used_colors == null)
                return null;
            switch (use_16bit)
            {
                case NO_16BIT:
                default:
                    return palette_recalc_8();
                case STATIC_16BIT:
                    return palette_recalc_16_static();
                case PALETTIZED_16BIT:
                    return palette_recalc_16_palettized();
            }
        }
        public static void palette_change_color(int color, byte red, byte green, byte blue)
        {
            if ((Machine.drv.video_attributes & VIDEO_MODIFIES_PALETTE) == 0)
            {
                printf("Error: palette_change_color() called, but VIDEO_MODIFIES_PALETTE not set.\n");
                return;
            }

            if (color >= Machine.drv.total_colors)
            {
                printf("error: palette_change_color() called with color %d, but only %d allocated.\n", color, Machine.drv.total_colors);
                return;
            }

            switch (use_16bit)
            {
                case NO_16BIT:
                    palette_change_color_8(color, red, green, blue);
                    break;
                case STATIC_16BIT:
                    palette_change_color_16_static(color, red, green, blue);
                    break;
                case PALETTIZED_16BIT:
                    palette_change_color_16_palettized(color, red, green, blue);
                    break;
            }
        }
        static void palette_change_color_8(int color, byte red, byte green, byte blue)
        {
            int pen;

            if (color == palette_transparent_color)
            {
                osd_modify_pen(palette_transparent_pen, red, green, blue);

                if (color == -1) return;	/* by default, palette_transparent_color is -1 */
            }

            if (game_palette[3 * color + 0] == red &&
                    game_palette[3 * color + 1] == green &&
                    game_palette[3 * color + 2] == blue)
            {
                palette_dirty[color] = 0;
                return;
            }

            pen = palette_map.read16(color);

            /* if the color was used, mark it as dirty, we'll change it in palette_recalc() */
            if ((old_used_colors[color] & PALETTE_COLOR_VISIBLE) != 0)
            {
                new_palette[3 * color + 0] = red;
                new_palette[3 * color + 1] = green;
                new_palette[3 * color + 2] = blue;
                palette_dirty[color] = 1;
            }
            /* otherwise, just update the array */
            else
            {
                game_palette[3 * color + 0] = red;
                game_palette[3 * color + 1] = green;
                game_palette[3 * color + 2] = blue;
            }
        }
        static void palette_change_color_16_static(int color, byte red, byte green, byte blue)
        {
            throw new Exception();
        }
        static void palette_change_color_16_palettized(int color, byte red, byte green, byte blue)
        {
            if (color == palette_transparent_color)
            {
                osd_modify_pen(palette_transparent_pen, red, green, blue);

                if (color == -1) return;	/* by default, palette_transparent_color is -1 */
            }

            if (game_palette[3 * color + 0] == red &&
                    game_palette[3 * color + 1] == green &&
                    game_palette[3 * color + 2] == blue)
                return;

            /* Machine.pens[color] might have been remapped to transparent_pen, so I */
            /* use shrinked_pens[] directly */
            osd_modify_pen(shrinked_pens.read16(color + RESERVED_PENS), red, green, blue);
            game_palette[3 * color + 0] = red;
            game_palette[3 * color + 1] = green;
            game_palette[3 * color + 2] = blue;
        }
        public static int paletteram_r(int offset)
        {
            return paletteram[offset];
        }
        public static _BytePtr paletteram = new _BytePtr(1), paletteram_2 = new _BytePtr(1);
        static void changecolor_xxxxBBBBGGGGRRRR(int color, int data)
        {
            int r = (data >> 0) & 0x0f;
            int g = (data >> 4) & 0x0f;
            int b = (data >> 8) & 0x0f;

            r = (r << 4) | r;
            g = (g << 4) | g;
            b = (b << 4) | b;

            palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        static void changecolor_xxxxRRRRGGGGBBBB(int color, int data)
        {
            int r = (data >> 8) & 0x0f;
            int g = (data >> 4) & 0x0f;
            int b = (data >> 0) & 0x0f;

            r = (r << 4) | r;
            g = (g << 4) | g;
            b = (b << 4) | b;

            palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        static void changecolor_xBBBBBGGGGGRRRRR(int color, int data)
        {
            int r = (data >> 0) & 0x1f;
            int g = (data >> 5) & 0x1f;
            int b = (data >> 10) & 0x1f;

            r = (r << 3) | (r >> 2);
            g = (g << 3) | (g >> 2);
            b = (b << 3) | (b >> 2);

            palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        static void changecolor_RRRRGGGGBBBBxxxx(int color, int data)
        {
            int r, g, b;


            r = (data >> 12) & 0x0f;
            g = (data >> 8) & 0x0f;
            b = (data >> 4) & 0x0f;

            r = (r << 4) | r;
            g = (g << 4) | g;
            b = (b << 4) | b;

            palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        public static void paletteram_xBBBBBGGGGGRRRRR_w(int offset, int data)
        {
            paletteram[offset] = (byte)data;
            changecolor_xBBBBBGGGGGRRRRR(offset / 2, paletteram[offset & ~1] | (paletteram[offset | 1] << 8));
        }
        public static void paletteram_xxxxBBBBGGGGRRRR_w(int offset, int data)
        {
            paletteram[offset] = (byte)data;
            changecolor_xxxxBBBBGGGGRRRR(offset / 2, paletteram[offset & ~1] | (paletteram[offset | 1] << 8));
        }
        public static void paletteram_xxxxRRRRGGGGBBBB_w(int offset, int data)
        {
            paletteram[offset] = (byte)data;
            changecolor_xxxxRRRRGGGGBBBB(offset / 2, paletteram[offset & ~1] | (paletteram[offset | 1] << 8));
        }
        public static void paletteram_xBBBBBGGGGGRRRRR_swap_w(int offset, int data)
        {
            paletteram[offset] = (byte)data;
            changecolor_xBBBBBGGGGGRRRRR(offset / 2, paletteram[offset | 1] | (paletteram[offset & ~1] << 8));
        }
        public static void paletteram_RRRRGGGGBBBBxxxx_swap_w(int offset, int data)
        {
            paletteram[offset] = (byte)data;
            changecolor_RRRRGGGGBBBBxxxx(offset / 2, paletteram[offset | 1] | (paletteram[offset & ~1] << 8));
        }
        public static void paletteram_BBGGGRRR_w(int offset, int data)
        {
            byte r, g, b;
            int bit0, bit1, bit2;


            paletteram[offset] = (byte)data;

            /* red component */
            bit0 = (data >> 0) & 0x01;
            bit1 = (data >> 1) & 0x01;
            bit2 = (data >> 2) & 0x01;
            r = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
            /* green component */
            bit0 = (data >> 3) & 0x01;
            bit1 = (data >> 4) & 0x01;
            bit2 = (data >> 5) & 0x01;
            g = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
            /* blue component */
            bit0 = 0;
            bit1 = (data >> 6) & 0x01;
            bit2 = (data >> 7) & 0x01;
            b = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

            palette_change_color(offset, r, g, b);
        }
        public static int paletteram_word_r(int offset)
        {
            return paletteram.read16(offset);
        }

        public static void palette_init_used_colors()
        {
            int pen;


            /* if we are not dynamically reducing the palette, return immediately. */
            if (palette_used_colors == null) return;
            for (int i = 0; i < Machine.drv.total_colors; i++)
                palette_used_colors[i] = PALETTE_COLOR_UNUSED;
            //memset(palette_used_colors,PALETTE_COLOR_UNUSED,Machine.drv.total_colors * sizeof(unsigned char));

            for (pen = 0; pen < Machine.drv.total_colors; pen++)
            {
                if (pen_visiblecount.read32(pen) != 0) palette_used_colors[pen] |= PALETTE_COLOR_VISIBLE;
                if (pen_cachedcount.read32(pen) != 0) palette_used_colors[pen] |= PALETTE_COLOR_CACHED;
            }
        }
    }
}
