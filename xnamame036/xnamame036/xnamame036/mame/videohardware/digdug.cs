using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_digdug
    {
        static _BytePtr digdug_vlatches = new _BytePtr(1);
        static int playfield, alphacolor, playenable, playcolor;

        static int pflastindex = -1, pflastcolor = -1;
        static bool flipscreen;

        static void digdug_vh_convert_color_prom(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
        {
            for (int i = 0; i < 32; i++)
            {
                int bit0 = (color_prom[31 - i] >> 0) & 0x01;
                int bit1 = (color_prom[31 - i] >> 1) & 0x01;
                int bit2 = (color_prom[31 - i] >> 2) & 0x01;
                palette[3 * i] =(byte)( 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                bit0 = (color_prom[31 - i] >> 3) & 0x01;
                bit1 = (color_prom[31 - i] >> 4) & 0x01;
                bit2 = (color_prom[31 - i] >> 5) & 0x01;
                palette[3 * i + 1] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                bit0 = 0;
                bit1 = (color_prom[31 - i] >> 6) & 0x01;
                bit2 = (color_prom[31 - i] >> 7) & 0x01;
                palette[3 * i + 2] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
            }

            /* characters */
            for (int i = 0; i < 8; i++)
            {
                colortable.write16(i * 2 + 0,  0);
                colortable.write16(i * 2 + 1,(ushort)( 31 - i * 2));
            }
            /* sprites */
            for (int i = 0 * 4; i < 64 * 4; i++)
                colortable.write16(8 * 2 + i, (ushort)(31 - ((color_prom[i + 32] & 0x0f) + 0x10)));
            /* playfield */
            for (int i = 64 * 4; i < 128 * 4; i++)
                colortable.write16(8 * 2 + i,  (ushort)(31 - (color_prom[i + 32] & 0x0f)));
        }
        static int digdug_vh_start()
        {
            if (Generic.generic_vh_start() != 0)
                return 1;

            pflastindex = -1;
            pflastcolor = -1;

            return 0;
        }
        static void digdug_vh_stop()
        {
            Generic.generic_vh_stop();
        }
        static void digdug_vh_latch_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                    playfield = (playfield & ~1) | (data & 1);
                    break;

                case 1:
                    playfield = (playfield & ~2) | ((data << 1) & 2);
                    break;

                case 2:
                    alphacolor = data & 1;
                    break;

                case 3:
                    playenable = data & 1;
                    break;

                case 4:
                    playcolor = (playcolor & ~1) | (data & 1);
                    break;

                case 5:
                    playcolor = (playcolor & ~2) | ((data << 1) & 2);
                    break;
            }
        }


        static void digdug_draw_sprite(Mame.osd_bitmap dest, uint code, uint color, bool flipx, bool flipy, int sx, int sy)
        {
            Mame.drawgfx(dest, Mame.Machine.gfx[1], code, color, flipx, flipy, sx, sy, Mame.Machine.drv.visible_area,
                Mame.TRANSPARENCY_PEN, 0);
        }



        static void digdug_flipscreen_w(int offset, int data)
        {
            if (flipscreen != ((data & 1)!=0))
            {
                flipscreen = (data & 1)!=0;
                Generic.SetDirtyBuffer(true);
            }
        }

        /***************************************************************************

          Draw the game screen in the given osd_bitmap.
          Do NOT call osd_update_display() from this function, it will be called by
          the main emulation engine.

        ***************************************************************************/
        static void digdug_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int offs, pfindex, pfcolor;
            _BytePtr pf;

            /* determine the playfield */
            if (playenable != 0)
            {
                pfindex = pfcolor = -1;
                pf = null;
            }
            else
            {
                pfindex = playfield;
                pfcolor = playcolor;
                pf = new _BytePtr(Mame.memory_region(Mame.REGION_GFX4), (pfindex << 10));
            }

            /* force a full update if the playfield has changed */
            if (pfindex != pflastindex || pfcolor != pflastcolor)
            {
                Generic.SetDirtyBuffer(true);
            }
            pflastindex = pfindex;
            pflastcolor = pfcolor;

            pfcolor <<= 4;

            /* for every character in the Video RAM, check if it has been modified */
            /* since last time and update it accordingly. */
            for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
            {
                if (Generic.dirtybuffer[offs])
                {
                    byte pfval, vrval;
                    int sx, sy, mx, my;

                    Generic.dirtybuffer[offs] = false;

                    /* Even if digdug's screen is 28x36, the memory layout is 32x32. We therefore */
                    /* have to convert the memory coordinates into screen coordinates. */
                    /* Note that 32*32 = 1024, while 28*36 = 1008: therefore 16 bytes of Video RAM */
                    /* don't map to a screen position. We don't check that here, however: range */
                    /* checking is performed by drawgfx(). */

                    mx = offs % 32;
                    my = offs / 32;

                    if (my <= 1)
                    {
                        sx = my + 34;
                        sy = mx - 2;
                    }
                    else if (my >= 30)
                    {
                        sx = my - 30;
                        sy = mx - 2;
                    }
                    else
                    {
                        sx = mx + 2;
                        sy = my - 2;
                    }

                    if (flipscreen )
                    {
                        sx = 35 - sx;
                        sy = 27 - sy;
                    }

                    vrval = Generic.videoram[offs];
                    if (pf != null)
                    {
                        /* first draw the playfield */
                        pfval = pf[offs];
                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[2],
                                pfval,
                                (uint)((pfval >> 4) + pfcolor),
                                flipscreen, flipscreen,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                        /* overlay with the character */
                        if ((vrval & 0x7f) != 0x7f)
                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    vrval,
                                    (uint)((vrval >> 5) | ((vrval >> 4) & 1)),
                                    flipscreen, flipscreen,
                                    8 * sx, 8 * sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                    else
                    {
                        /* just draw the character */
                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                vrval,
                                (uint)((vrval >> 5) | ((vrval >> 4) & 1)),
                                flipscreen, flipscreen,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }
            }

            /* copy the temporary bitmap to the screen */
            Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

            /* Draw the sprites. */
            for (offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
            {
                /* is it on? */
                if ((Generic.spriteram_3[offs + 1] & 2) == 0)
                {
                    int sprite = Generic.spriteram[offs];
                    int color = Generic.spriteram[offs + 1];
                    int x = Generic.spriteram_2[offs + 1] - 40;
                    int y = 28 * 8 - Generic.spriteram_2[offs];
                    bool flipx = (Generic.spriteram_3[offs] & 1) != 0;
                    bool flipy = (Generic.spriteram_3[offs] & 2) != 0;

                    if (flipscreen)
                    {
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    if (x < 8) x += 256;

                    /* normal size? */
                    if (sprite < 0x80)
                        digdug_draw_sprite(bitmap, (uint)sprite, (uint)color, flipx, flipy, x, y);

                    /* double size? */
                    else
                    {
                        sprite = (sprite & 0xc0) | ((sprite & ~0xc0) << 2);
                        if (!flipx && !flipy)
                        {
                            digdug_draw_sprite(bitmap, 2 + (uint)sprite, (uint)color, flipx, flipy, x, y);
                            digdug_draw_sprite(bitmap, 3 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y);
                            digdug_draw_sprite(bitmap, (uint)sprite, (uint)color, flipx, flipy, x, y - 16);
                            digdug_draw_sprite(bitmap, 1 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y - 16);
                        }
                        else if (flipx && flipy)
                        {
                            digdug_draw_sprite(bitmap, 1 + (uint)sprite, (uint)color, flipx, flipy, x, y);
                            digdug_draw_sprite(bitmap, (uint)sprite, (uint)color, flipx, flipy, x + 16, y);
                            digdug_draw_sprite(bitmap, 3 + (uint)sprite, (uint)color, flipx, flipy, x, y - 16);
                            digdug_draw_sprite(bitmap, 2 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y - 16);
                        }
                        else if (flipy)
                        {
                            digdug_draw_sprite(bitmap, (uint)sprite, (uint)color, flipx, flipy, x, y);
                            digdug_draw_sprite(bitmap, 1 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y);
                            digdug_draw_sprite(bitmap, 2 + (uint)sprite, (uint)color, flipx, flipy, x, y - 16);
                            digdug_draw_sprite(bitmap, 3 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y - 16);
                        }
                        else /* flipx */
                        {
                            digdug_draw_sprite(bitmap, 3 + (uint)sprite, (uint)color, flipx, flipy, x, y);
                            digdug_draw_sprite(bitmap, 2 + (uint)sprite, (uint)color, flipx, flipy, x + 16, y);
                            digdug_draw_sprite(bitmap, 1 + (uint)sprite, (uint)color, flipx, flipy, x, y - 16);
                            digdug_draw_sprite(bitmap, (uint)sprite, (uint)color, flipx, flipy, x + 16, y - 16);
                        }
                    }
                }
            }
        }
    }
}
