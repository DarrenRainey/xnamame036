using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_frogger
    {


        static void frogger2_vh_screenrefresh(Mame.osd_bitmap bitmap,int full_refresh)
{
	int i,offs;

	/* for every character in the Video RAM, check if it has been modified */
	/* since last time and update it accordingly. */
	for (offs = Generic.videoram_size[0] - 1;offs >= 0;offs--)
	{
		if (Generic.dirtybuffer[offs])
		{
			int sx,sy,col;

			Generic.dirtybuffer[offs] = false;

			sx = offs % 32;
			sy = offs / 32;
			col = frogger_attributesram[2 * sx + 1] & 7;
			col = ((col >> 1) & 0x03) | ((col << 2) & 0x04);

			if (flipscreen!=0)
			{
				sx = 31 - sx;
				sy = 31 - sy;
				Mame.drawgfx(Generic.tmpbitmap,Mame.Machine.gfx[0],
						Generic.videoram[offs],
						(uint)(col + (sx >= 16 ? 8 : 0)),	/* blue background in the lower 128 lines */
						flipscreen!=0,flipscreen!=0,8*sx,8*sy,
						null,Mame.TRANSPARENCY_NONE,0);
			}
			else
			{
                Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
						Generic.videoram[offs],
						(uint)(col + (sx <= 15 ? 8 : 0)),	/* blue background in the upper 128 lines */
						flipscreen!=0,flipscreen!=0,8*sx,8*sy,
						null,Mame.TRANSPARENCY_NONE,0);
			}
		}
	}


	/* copy the temporary bitmap to the screen */
	{
		int[] scroll=new int[32];

		for (i = 0;i < 32;i++)
		if (flipscreen!=0)
		{
			scroll[31-i] = frogger_attributesram[2 * i];
		}
		else
		{
			scroll[i] = -frogger_attributesram[2 * i];
		}

        Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 32, scroll, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
	}


	/* Draw the sprites. Note that it is important to draw them exactly in this */
	/* order, to have the correct priorities. */
	for (offs = Generic.spriteram_size[0] - 4;offs >= 0;offs -= 4)
	{
		if (Generic.spriteram[offs + 3] != 0)
		{
			int x,y,col;

			x = Generic.spriteram[offs + 3];
			y = Generic.spriteram[offs];
			col = Generic.spriteram[offs + 2] & 7;
			col = ((col >> 1) & 0x03) | ((col << 2) & 0x04);

			if (flipscreen!=0)
			{
				x = 242 - x;
				y = 240 - y;
				Mame.drawgfx(bitmap,Mame.Machine.gfx[1],
						(uint)(Generic.spriteram[offs + 1] & 0x3f),
						(uint)col,
						(Generic.spriteram[offs + 1] & 0x40)==0,(Generic.spriteram[offs + 1] & 0x80)==0,
						x,30*8 - y,
						Mame.Machine.drv.visible_area,Mame.TRANSPARENCY_PEN,0);
			}
			else
			{
				Mame.drawgfx(bitmap,Mame.Machine.gfx[1],
						(uint)(Generic.spriteram[offs + 1] & 0x3f),
						(uint)col,
						(Generic.spriteram[offs + 1] & 0x40)!=0,(Generic.spriteram[offs + 1] & 0x80)!=0,
						x,30*8 - y,
						Mame.Machine.drv.visible_area,Mame.TRANSPARENCY_PEN,0);
			}
		}
	}
}


    }
}
