using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
        public class Pengo
        {
            static int gfx_bank;
            static bool flipscreen;
            static int xoffsethack;
            static Mame.rectangle spritevisiblearea = new Mame.rectangle(2 * 8, 34 * 8 - 1, 0 * 8, 28 * 8 - 1);

            public static void pengo_flipscreen_w(int offset, int data)
            {
                if (flipscreen != ((data & 1)!=0))
                {
                    flipscreen = (data & 1)!=0;
                    Generic.SetDirtyBuffer(true);
                }
            }
            public static void pengo_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
            {
                for (int offs = Generic.videoram_size[0] - 1; offs > 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int mx, my, sx, sy;

                        Generic.dirtybuffer[offs] = false;
                        mx = offs % 32;
                        my = offs / 32;

                        if (my < 2)
                        {
                            if (mx < 2 || mx >= 30) continue; /* not visible */
                            sx = my + 34;
                            sy = mx - 2;
                        }
                        else if (my >= 30)
                        {
                            if (mx < 2 || mx >= 30) continue; /* not visible */
                            sx = my - 30;
                            sy = mx - 2;
                        }
                        else
                        {
                            sx = mx + 2;
                            sy = my - 2;
                        }

                        if (flipscreen)
                        {
                            sx = 35 - sx;
                            sy = 27 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[gfx_bank * 2],
                                Generic.videoram[offs],
                                (uint)Generic.colorram[offs] & 0x1f,
                                flipscreen, flipscreen,
                                sx * 8, sy * 8,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (int offs = Generic.spriteram_size[0] - 2; offs > 2 * 2; offs -= 2)
                {
                    int sx = 272 - Generic.spriteram_2[offs + 1];
                    int sy = Generic.spriteram_2[offs] - 31;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[gfx_bank * 2 + 1],
                            (uint)Generic.spriteram[offs] >> 2,
                            (uint)Generic.spriteram[offs + 1] & 0x1f,
                            (Generic.spriteram[offs] & 1)!=0, (Generic.spriteram[offs] & 2)!=0,
                            sx, sy,
                            spritevisiblearea, Mame.TRANSPARENCY_COLOR, 0);

                    /* also plot the sprite with wraparound (tunnel in Crush Roller) */
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[gfx_bank * 2 + 1],
                            (uint)Generic.spriteram[offs] >> 2,
                            (uint)Generic.spriteram[offs + 1] & 0x1f,
                            (Generic.spriteram[offs] & 1)!=0, (Generic.spriteram[offs] & 2)!=0,
                            sx - 256, sy,
                            spritevisiblearea, Mame.TRANSPARENCY_COLOR, 0);
                }
                /* In the Pac Man based games (NOT Pengo) the first two sprites must be offset */
                /* one pixel to the left to get a more correct placement */
                for (int offs = 2 * 2; offs >= 0; offs -= 2)
                {
                    int sx = 272 - Generic.spriteram_2[offs + 1];
                    int sy = Generic.spriteram_2[offs] - 31;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[gfx_bank * 2 + 1],
                            (uint)Generic.spriteram[offs] >> 2,
                            (uint)Generic.spriteram[offs + 1] & 0x1f,
                            (Generic.spriteram[offs] & 1)!=0, (Generic.spriteram[offs] & 2)!=0,
                            sx, sy + xoffsethack,
                            spritevisiblearea, Mame.TRANSPARENCY_COLOR, 0);

                    /* also plot the sprite with wraparound (tunnel in Crush Roller) */
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[gfx_bank * 2 + 1],
                            (uint)Generic.spriteram[offs] >> 2,
                            (uint)Generic.spriteram[offs + 1] & 0x1f,
                            (Generic.spriteram[offs] & 2)!=0, (Generic.spriteram[offs] & 1)!=0,
                            sx - 256, sy + xoffsethack,
                            spritevisiblearea, Mame.TRANSPARENCY_COLOR, 0);
                }
            }
            public static int pacman_vh_start()
            {
                gfx_bank = 0;
                /* In the Pac Man based games (NOT Pengo) the first two sprites must be offset */
                /* one pixel to the left to get a more correct placement */
                xoffsethack = 1;

                return Generic.generic_vh_start();
            }
            public static int pacman_vh_convert_color_prom(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                int i;
                //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
                //#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])

                int cpi = 0;
                int pi = 0;
                for (i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] =(byte)( 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                cpi += 0x10;
                /* color_prom now points to the beginning of the lookup table */

                /* character lookup table */
                /* sprites use the same color lookup table as characters */
                for (i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
                    colortable[Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i]=(ushort)((color_prom[cpi++]) & 0x0f);

                return 0;
            }
        }
}