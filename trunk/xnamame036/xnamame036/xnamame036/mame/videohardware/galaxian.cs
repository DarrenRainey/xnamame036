using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_galaxian
    {

        static Mame.rectangle _spritevisiblearea = new Mame.rectangle(2 * 8 + 1, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
        static Mame.rectangle _spritevisibleareaflipx = new Mame.rectangle(0 * 8, 30 * 8 - 2, 2 * 8, 30 * 8 - 1);

        static Mame.rectangle spritevisiblearea;
        static Mame.rectangle spritevisibleareaflipx;


        const int MAX_STARS = 250;
        const int STARS_COLOR_BASE = 32;

        static _BytePtr galaxian_attributesram = new _BytePtr(1);
        static _BytePtr galaxian_bulletsram = new _BytePtr(1);

        static int[] galaxian_bulletsram_size = new int[1];
        static int stars_on, stars_blink;
        static int stars_type;		/* -1 = no stars */
        /*  0 = Galaxian stars */
        /*  1 = Scramble stars */
        /*  2 = Rescue stars (same as Scramble, but only half screen) */
        /*  3 = Mariner stars (same as Galaxian, but some parts are blanked */
        static uint stars_scroll;
        static int color_mask;
        static int mooncrst_gfxextend;
        static int pisces_gfxbank;
        static int[] jumpbug_gfxbank = new int[5];
        static int[] flipscreen = new int[2];

        static int background_on;
        static byte[] backcolor = new byte[256];
        struct star
        {
            public int x, y, code;
        };
        static star[] stars = new star[MAX_STARS];
        static int total_stars;

        static void galaxian_attributes_w(int offset, int data)
        {
            if ((offset & 1) != 0 && galaxian_attributesram[offset] != data)
            {
                int i;


                for (i = offset / 2; i < Generic.videoram_size[0]; i += 32)
                    Generic.dirtybuffer[i] = true;
            }

            galaxian_attributesram[offset] = (byte)data;
        }

        static int galaxian_vh_interrupt()
        {
            stars_scroll++;

            return Mame.nmi_interrupt();
        }
        static void galaxian_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            throw new Exception();
        }
        static void galaxian_vh_convert_color_prom(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
        {
            int i;
            //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
            //#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])


            color_mask = (Mame.Machine.gfx[0].color_granularity == 4) ? 7 : 3;
            int cpi = 0, pi = 0;
            /* first, the character/sprite palette */
            for (i = 0; i < 32; i++)
            {
                int bit0, bit1, bit2;

                /* red component */
                bit0 = (color_prom[cpi] >> 0) & 0x01;
                bit1 = (color_prom[cpi] >> 1) & 0x01;
                bit2 = (color_prom[cpi] >> 2) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                /* green component */
                bit0 = (color_prom[cpi] >> 3) & 0x01;
                bit1 = (color_prom[cpi] >> 4) & 0x01;
                bit2 = (color_prom[cpi] >> 5) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                /* blue component */
                bit0 = (color_prom[cpi] >> 6) & 0x01;
                bit1 = (color_prom[cpi] >> 7) & 0x01;
                palette[pi++] = (byte)(0x4f * bit0 + 0xa8 * bit1);

                cpi++;
            }

            /* now the stars */
            for (i = 0; i < 64; i++)
            {
                int bits;
                byte[] map = { 0x00, 0x88, 0xcc, 0xff };

                bits = (i >> 0) & 0x03;
                palette[pi++] = map[bits];
                bits = (i >> 2) & 0x03;
                palette[pi++] = map[bits];
                bits = (i >> 4) & 0x03;
                palette[pi++] = map[bits];
            }

            /* characters and sprites use the same palette */
            for (i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
            {
                /* 00 is always mapped to pen 0 */
                if ((i & (Mame.Machine.gfx[0].color_granularity - 1)) == 0)
                    colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i, 0);
            }

            /* bullets can be either white or yellow */

            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start + 0, 0);
            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start + 1, 0x0f + STARS_COLOR_BASE);	/* yellow */
            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start + 2, 0);
            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start + 3, 0x3f + STARS_COLOR_BASE);	/* white */

            /* default blue background */
            palette[pi++] = 0;
            palette[pi++] = 0;
            palette[pi++] = 0x55;

            for (i = 0; i < (Mame.Machine.gfx[3].total_colors * Mame.Machine.gfx[3].color_granularity); i++)
            {
                colortable.write16(Mame.Machine.drv.gfxdecodeinfo[3].color_codes_start + i, (ushort)(96 + (i % (Mame.Machine.drv.total_colors - 96))));
            }
        }
        static void decode_background()
{
	int i, j, k;
	byte[] tile=new byte[32*8*8];


	for (i = 0; i < 32; i++)
	{
		for (j = 0; j < 8; j++)
		{
			for (k = 0; k < 8; k++)
			{
				tile[i*64 + j*8 + k] = backcolor[i*8+j];
			}
		}

        Mame.decodechar(Mame.Machine.gfx[3], i, new _BytePtr(tile), Mame.Machine.drv.gfxdecodeinfo[3].gfxlayout);
	}
}
        static int common_vh_start()
{
	int generator;
	int x,y;


    //modify_charcode   = 0;
    //modify_spritecode = 0;

	mooncrst_gfxextend = 0;
	stars_on = 0;
	flipscreen[0] = 0;
	flipscreen[1] = 0;

	if (Generic.generic_vh_start() != 0)
		return 1;

    /* Default alternate background - Solid Blue */

    for (x=0; x<256; x++)
	{
		backcolor[x] = 0;
	}
	background_on = 0;

	decode_background();


	/* precalculate the star background */

	total_stars = 0;
	generator = 0;

	for (y = 255;y >= 0;y--)
	{
		for (x = 511;x >= 0;x--)
		{
			int bit1,bit2;


			generator <<= 1;
			bit1 = (~generator >> 17) & 1;
			bit2 = (generator >> 5) & 1;

			if ((bit1 ^ bit2)!=0) generator |= 1;

			if (((~generator >> 16) & 1)!=0 && (generator & 0xff) == 0xff)
			{
				int color;

				color = (~(generator >> 8)) & 0x3f;
				if (color !=0&& total_stars < MAX_STARS)
				{
					stars[total_stars].x = x;
					stars[total_stars].y = y;
					stars[total_stars].code = color;

					total_stars++;
				}
			}
		}
	}


	/* all the games except New Sinbad 7 clip the sprites at the top of the screen,
	   New Sinbad 7 does it at the bottom */
    //if (Mame.Machine.gamedrv == &driver_newsin7)
    //{
    //    spritevisiblearea      = &_spritevisibleareaflipx;
    //    spritevisibleareaflipx = &_spritevisiblearea;
    //}
    //else
	{
		spritevisiblearea      = _spritevisiblearea;
        spritevisibleareaflipx = _spritevisibleareaflipx;
	}


	return 0;
}

        static int galaxian_vh_start()
        {

            stars_type = 0;
            return common_vh_start();
        }
        static void galaxian_stars_w(int offset, int data)
        {
            stars_on = (data & 1);
            stars_scroll = 0;
        }
        static void galaxian_flipx_w(int offset, int data)
        {
            if (flipscreen[0] != (data & 1))
            {
                flipscreen[0] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

        static void galaxian_flipy_w(int offset, int data)
        {
            if (flipscreen[1] != (data & 1))
            {
                flipscreen[1] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

    }
}
