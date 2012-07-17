using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_tecmo : Mame.GameDriver
    {
        public static _BytePtr tecmo_videoram = new _BytePtr(1);
        public static _BytePtr tecmo_colorram = new _BytePtr(1);
        public static _BytePtr tecmo_videoram2 = new _BytePtr(1);
        public static _BytePtr tecmo_colorram2 = new _BytePtr(1);
        public static _BytePtr tecmo_scroll = new _BytePtr(1);
        public static int[] tecmo_videoram2_size = new int[1];
        public static byte[] dirtybuffer2;
        public static Mame.osd_bitmap tmpbitmap2, tmpbitmap3;
        public static int video_type = 0;

        public static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xf000, 0xf7ff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xf800, 0xf800, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xf801, 0xf801, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xf802, 0xf802, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0xf803, 0xf803, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xf804, 0xf804, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0xf805, 0xf805, Mame.input_port_5_r ),
	new Mame.MemoryReadAddress( 0xf806, 0xf806, Mame.input_port_6_r ),
	new Mame.MemoryReadAddress( 0xf807, 0xf807, Mame.input_port_7_r ),
	new Mame.MemoryReadAddress( 0xf808, 0xf808, Mame.input_port_8_r ),
	new Mame.MemoryReadAddress( 0xf809, 0xf809, Mame.input_port_9_r ),
	new Mame.MemoryReadAddress( 0xf80f, 0xf80f, Mame.input_port_10_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        public static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        public static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x2000, 0x207f, Mame.MWA_RAM ),	/* Silkworm set #2 has a custom CPU which */
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, YM3812.YM3812_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xa001, 0xa001, YM3812.YM3812_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, tecmo_adpcm_start_w ),
	new Mame.MemoryWriteAddress( 0xc400, 0xc400, tecmo_adpcm_end_w ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, tecmo_adpcm_trigger_w ),
	new Mame.MemoryWriteAddress( 0xcc00, 0xcc00, Mame.MWA_NOP ),	/* NMI acknowledge? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        public static Mame.GfxLayout tecmo_charlayout =
new Mame.GfxLayout(
    8, 8,	/* 8*8 characters */
    1024,	/* 1024 characters */
    4,	/* 4 bits per pixel */
    new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
    new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
    new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
    32 * 8	/* every char takes 32 consecutive bytes */
);
        static Mame.GfxLayout silkworm_spritelayout =
new Mame.GfxLayout(
   16, 16,	/* 16*16 sprites */
   2048,	/* 2048 sprites */
   4,	/* 4 bits per pixel */
   new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
   new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			32*8+0*4, 32*8+1*4, 32*8+2*4, 32*8+3*4, 32*8+4*4, 32*8+5*4, 32*8+6*4, 32*8+7*4 },
   new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			16*32, 17*32, 18*32, 19*32, 20*32, 21*32, 22*32, 23*32 },
   128 * 8	/* every sprite takes 128 consecutive bytes */
);

        static Mame.GfxLayout silkworm_spritelayout2x =
        new Mame.GfxLayout(
            32, 32,	/* 32*32 sprites */
            512,	/* 512 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			32*8+0*4, 32*8+1*4, 32*8+2*4, 32*8+3*4, 32*8+4*4, 32*8+5*4, 32*8+6*4, 32*8+7*4,
			128*8+0*4, 128*8+1*4, 128*8+2*4, 128*8+3*4, 128*8+4*4, 128*8+5*4, 128*8+6*4, 128*8+7*4,
			160*8+0*4, 160*8+1*4, 160*8+2*4, 160*8+3*4, 160*8+4*4, 160*8+5*4, 160*8+6*4, 160*8+7*4 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			16*32, 17*32, 18*32, 19*32, 20*32, 21*32, 22*32, 23*32,
			64*32, 65*32, 66*32, 67*32, 68*32, 69*32, 70*32, 71*32,
			80*32, 81*32, 82*32, 83*32, 84*32, 85*32, 86*32, 87*32 },
            512 * 8	/* every sprite takes 512 consecutive bytes */
        );

        static Mame.GfxLayout silkworm_spritelayout8x8 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 xprites */
            8192,	/* 8192 sprites */
            4,		/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
            new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );
        public static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, tecmo_charlayout,        256, 16),	/* colors 256 - 511 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, silkworm_spritelayout8x8,  0, 16),	/* colors   0 - 255 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, silkworm_spritelayout,     0, 16),	/* 16x16 sprites */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, silkworm_spritelayout2x,   0, 16),	/* double size hack */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, silkworm_spritelayout,   512, 16),	/* bg#1 colors 512 - 767 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX4, 0, silkworm_spritelayout,   768, 16),	/* bg#2 colors 768 - 1023 */
};
        public static YM3526interface ym3812_interface =
        new YM3526interface(
            1,			/* 1 chip (no more supported) */
            4000000,	/* 4 MHz ? */
            new int[] { 255 }		/* (not supported) */
        );

        /* ADPCM chip is a MSM5205 @ 400kHz */
        public static ADPCMinterface adpcm_interface =
        new ADPCMinterface(
            1,			/* 1 channel */
            8333,       /* 8000Hz playback */
            Mame.REGION_SOUND1,	/* memory region 3 */
            null,			/* init function */
            new int[] { 255 }
        );
        static int adpcm_start, adpcm_end;

        public static void tecmo_adpcm_start_w(int offset, int data)
        {
            adpcm_start = data << 8;
        }
        public static void tecmo_adpcm_end_w(int offset, int data)
        {
            adpcm_end = (data + 1) << 8;
        }
        public static void tecmo_adpcm_trigger_w(int offset, int data)
        {
            ADPCM.ADPCM_setvol(0, (data & 0x0f) * 0x11);
            if ((data & 0x0f) != 0)	/* maybe this selects the volume? */
                if (adpcm_start < 0x8000)
                    ADPCM.ADPCM_play(0, adpcm_start, (adpcm_end - adpcm_start) * 2);
        }
        public static void tecmo_videoram_w(int offset, int data)
        {
            if (tecmo_videoram[offset] != data)
            {
                dirtybuffer2[offset] = 1;
                tecmo_videoram[offset] = (byte)data;
            }
        }
        public static void tecmo_colorram_w(int offset, int data)
        {
            if (tecmo_colorram[offset] != data)
            {
                dirtybuffer2[offset] = 1;
                tecmo_colorram[offset] = (byte)data;
            }
        }
        public static void tecmo_bankswitch_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            bankaddress = 0x10000 + ((data & 0xf8) << 8);
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }
        public static void tecmo_sound_command_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.cpu_cause_interrupt(1, Mame.cpu_Z80.Z80_NMI_INT);
        }

        public static int tecmo_vh_start()
        {
            if (Generic.generic_vh_start() != 0) return 1;

            dirtybuffer2 = new byte[Generic.videoram_size[0]];

            for (int i = 0; i < Generic.videoram_size[0]; i++)
                dirtybuffer2[i] = 1;

            /* the background area is twice as wide as the screen */
            tmpbitmap2 = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);
            tmpbitmap3 = Mame.osd_new_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);
            /* 0x100 is the background color */
            Mame.palette_transparent_color = 0x100;

            return 0;
        }
        public static void tecmo_vh_stop()
        {
            Mame.osd_free_bitmap(tmpbitmap3);
            Mame.osd_free_bitmap(tmpbitmap2);
            dirtybuffer2 = null;
            Generic.generic_vh_stop();
        }
        static void tecmo_draw_sprites(Mame.osd_bitmap bitmap, int priority)
        {
            /* draw all visible sprites of specified priority */
            for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
            {
                int flags = Generic.spriteram[offs + 3];


                if ((flags >> 6) == priority)
                {
                    int bank = Generic.spriteram[offs + 0];
                    if ((bank & 4) != 0)
                    { /* visible */
                        int which = Generic.spriteram[offs + 1];
                        int code;
                        int size = (Generic.spriteram[offs + 2] & 3);
                        /* 0 = 8x8 1 = 16x16 2 = 32x32 3 = 64x64 */
                        if (size == 3) continue;	/* not used by these games */

                        if (video_type != 0)
                            code = (which) + ((bank & 0xf8) << 5); /* silkworm */
                        else
                            code = (which) + ((bank & 0xf0) << 4); /* rygar */

                        if (size == 1) code >>= 2;
                        else if (size == 2) code >>= 4;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[size + 1],
                                (uint)code,
                                (uint)flags & 0xf, /* color */
                                (bank & 1) != 0, /* flipx */
                                (bank & 2) != 0, /* flipy */

                                Generic.spriteram[offs + 5] - ((flags & 0x10) << 4), /* sx */
                                Generic.spriteram[offs + 4] - ((flags & 0x20) << 3), /* sy */

                                Mame.Machine.drv.visible_area,
                                priority == 3 ? Mame.TRANSPARENCY_THROUGH : Mame.TRANSPARENCY_PEN,
                                priority == 3 ? Mame.palette_transparent_pen : 0);
                    }
                }
            }
        }

        public static void tecmo_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int offs;


            Mame.palette_init_used_colors();

            {
                int color, code, i;
                int[] colmask = new int[16];
                int pal_base;


                pal_base = Mame.Machine.drv.gfxdecodeinfo[5].color_codes_start;

                for (color = 0; color < 16; color++) colmask[color] = 0;

                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (video_type == 2)	/* Gemini Wing */
                    {
                        code = Generic.videoram[offs] + 16 * (Generic.colorram[offs] & 0x70);
                        color = Generic.colorram[offs] & 0x0f;
                    }
                    else
                    {
                        code = Generic.videoram[offs] + 256 * (Generic.colorram[offs] & 0x07);
                        color = Generic.colorram[offs] >> 4;
                    }

                    colmask[color] |= (int)Mame.Machine.gfx[5].pen_usage[code];
                }

                for (color = 0; color < 16; color++)
                {
                    if ((colmask[color] & (1 << 0)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                    for (i = 1; i < 16; i++)
                    {
                        if ((colmask[color] & (1 << i)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                    }
                }


                pal_base = Mame.Machine.drv.gfxdecodeinfo[4].color_codes_start;

                for (color = 0; color < 16; color++) colmask[color] = 0;

                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (video_type == 2)	/* Gemini Wing */
                    {
                        code = tecmo_videoram[offs] + 16 * (tecmo_colorram[offs] & 0x70);
                        color = tecmo_colorram[offs] & 0x0f;
                    }
                    else
                    {
                        code = tecmo_videoram[offs] + 256 * (tecmo_colorram[offs] & 0x07);
                        color = tecmo_colorram[offs] >> 4;
                    }

                    colmask[color] |= (int)Mame.Machine.gfx[4].pen_usage[code];
                }

                for (color = 0; color < 16; color++)
                {
                    if ((colmask[color] & (1 << 0)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                    for (i = 1; i < 16; i++)
                    {
                        if ((colmask[color] & (1 << i)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                    }
                }


                pal_base = Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start;

                for (color = 0; color < 16; color++) colmask[color] = 0;

                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
                {
                    int flags = Generic.spriteram[offs + 3];
                    int bank = Generic.spriteram[offs + 0];
                    if ((bank & 4) != 0)
                    { /* visible */
                        int which = Generic.spriteram[offs + 1];
                        int size = (Generic.spriteram[offs + 2] & 3);
                        /* 0 = 8x8 1 = 16x16 2 = 32x32 3 = 64x64 */
                        if (size == 3) continue;	/* not used by these games */


                        if (video_type != 0)
                            code = (which) + ((bank & 0xf8) << 5); /* silkworm */
                        else
                            code = (which) + ((bank & 0xf0) << 4); /* rygar */

                        if (size == 1) code >>= 2;
                        else if (size == 2) code >>= 4;

                        color = flags & 0xf;

                        colmask[color] |= (int)Mame.Machine.gfx[size + 1].pen_usage[code];
                    }
                }

                for (color = 0; color < 16; color++)
                {
                    if ((colmask[color] & (1 << 0)) != 0)
                        Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                    for (i = 1; i < 16; i++)
                    {
                        if ((colmask[color] & (1 << i)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                    }
                }


                pal_base = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;

                for (color = 0; color < 16; color++) colmask[color] = 0;

                for (offs = tecmo_videoram2_size[0] - 1; offs >= 0; offs--)
                {
                    code = tecmo_videoram2[offs] + ((tecmo_colorram2[offs] & 0x03) << 8);
                    color = tecmo_colorram2[offs] >> 4;
                    colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
                }

                for (color = 0; color < 16; color++)
                {
                    for (i = 1; i < 16; i++)
                    {
                        if ((colmask[color] & (1 << i)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                    }
                }
            }

            if (Mame.palette_recalc() != null)
            {
                for (int i = 0; i < Generic.videoram_size[0]; i++)
                {
                    Generic.dirtybuffer[i] = true;
                    dirtybuffer2[i] = 1;
                }
            }


            /* draw the background. */
            for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
            {
                if (Generic.dirtybuffer[offs])
                {
                    int code, color, sx, sy;

                    if (video_type == 2)	/* Gemini Wing */
                    {
                        code = Generic.videoram[offs] + 16 * (Generic.colorram[offs] & 0x70);
                        color = Generic.colorram[offs] & 0x0f;
                    }
                    else
                    {
                        code = Generic.videoram[offs] + 256 * (Generic.colorram[offs] & 0x07);
                        color = Generic.colorram[offs] >> 4;
                    }
                    sx = offs % 32;
                    sy = offs / 32;

                    Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[5],
                            (uint)code,
                            (uint)color,
                            false, false,
                            16 * sx, 16 * sy,
                            null, Mame.TRANSPARENCY_NONE, 0);

                    Generic.dirtybuffer[offs] = false;
                }

                if (dirtybuffer2[offs] != 0)
                {
                    int code, color, sx, sy;

                    if (video_type == 2)	/* Gemini Wing */
                    {
                        code = tecmo_videoram[offs] + 16 * (tecmo_colorram[offs] & 0x70);
                        color = tecmo_colorram[offs] & 0x0f;
                    }
                    else
                    {
                        code = tecmo_videoram[offs] + 256 * (tecmo_colorram[offs] & 0x07);
                        color = tecmo_colorram[offs] >> 4;
                    }
                    sx = offs % 32;
                    sy = offs / 32;

                    Mame.drawgfx(tmpbitmap3, Mame.Machine.gfx[4],
                            (uint)code,
                            (uint)color,
                            false, false,
                            16 * sx, 16 * sy,
                            null, Mame.TRANSPARENCY_NONE, 0);
                }
                dirtybuffer2[offs] = 0;
            }


            /* copy the temporary bitmap to the screen */
            {
                int scrollx, scrolly;

                /* draw background tiles */
                scrollx = -tecmo_scroll[3] - 256 * (tecmo_scroll[4] & 1) - 48;
                scrolly = -tecmo_scroll[5];

                Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, new int[] { scrollx }, 1, new int[] { scrolly },
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                /* sprites will be drawn with TRANSPARENCY_THROUGH and appear behind the background */
                tecmo_draw_sprites(bitmap, 3); /* this should never draw anything, but just in case... */

                tecmo_draw_sprites(bitmap, 2);

                /* draw foreground tiles */
                scrollx = -tecmo_scroll[0] - 256 * (tecmo_scroll[1] & 1) - 48;
                scrolly = -tecmo_scroll[2];
                Mame.copyscrollbitmap(bitmap, tmpbitmap3, 1, new int[] { scrollx }, 1, new int[] { scrolly },
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);
            }

            tecmo_draw_sprites(bitmap, 1);

            /* draw the frontmost playfield. They are characters, but draw them as sprites */
            for (offs = tecmo_videoram2_size[0] - 1; offs >= 0; offs--)
            {
                int sx = offs % 32;
                int sy = offs / 32;

                Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                        (uint)(tecmo_videoram2[offs] + ((tecmo_colorram2[offs] & 0x03) << 8)),
                        (uint)(tecmo_colorram2[offs] >> 4),
                        false, false,
                        8 * sx, 8 * sy,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }

            tecmo_draw_sprites(bitmap, 0);
        }

        public override void driver_init()
        {
            //nothing
        }
    }
    class driver_rygar : driver_tecmo
    {




        static Mame.MemoryWriteAddress[] rygar_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Mame.MWA_RAM, tecmo_videoram2, tecmo_videoram2_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Mame.MWA_RAM, tecmo_colorram2 ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd9ff, tecmo_videoram_w, tecmo_videoram ),
	new Mame.MemoryWriteAddress( 0xda00, 0xdbff, tecmo_colorram_w, tecmo_colorram ),
	new Mame.MemoryWriteAddress( 0xdc00, 0xddff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xde00, 0xdfff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xe800, 0xefff, Mame.paletteram_xxxxBBBBRRRRGGGG_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf7ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xf800, 0xf805, Mame.MWA_RAM, tecmo_scroll ),
	new Mame.MemoryWriteAddress( 0xf806, 0xf806, tecmo_sound_command_w ),
	new Mame.MemoryWriteAddress( 0xf807, 0xf807, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( 0xf808, 0xf808, tecmo_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xf809, 0xf809, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( 0xf80b, 0xf80b, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] rygar_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] rygar_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, YM3812.YM3812_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8001, 0x8001, YM3812.YM3812_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, tecmo_adpcm_start_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, tecmo_adpcm_end_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe000, tecmo_adpcm_trigger_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf000, Mame.MWA_NOP ),	/* NMI acknowledge? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};





        /* the only difference in rygar_spritelayout is that half as many sprites are present */

        static Mame.GfxLayout rygar_spritelayout = /* only difference is half as many sprites as silkworm */
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            1024,	/* 1024 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			32*8+0*4, 32*8+1*4, 32*8+2*4, 32*8+3*4, 32*8+4*4, 32*8+5*4, 32*8+6*4, 32*8+7*4 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			16*32, 17*32, 18*32, 19*32, 20*32, 21*32, 22*32, 23*32 },
            128 * 8	/* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout rygar_spritelayout2x =
        new Mame.GfxLayout(
            32, 32,	/* 32*32 sprites */
            256,	/* 512 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
            new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
			32*8+0*4, 32*8+1*4, 32*8+2*4, 32*8+3*4, 32*8+4*4, 32*8+5*4, 32*8+6*4, 32*8+7*4,
			128*8+0*4, 128*8+1*4, 128*8+2*4, 128*8+3*4, 128*8+4*4, 128*8+5*4, 128*8+6*4, 128*8+7*4,
			160*8+0*4, 160*8+1*4, 160*8+2*4, 160*8+3*4, 160*8+4*4, 160*8+5*4, 160*8+6*4, 160*8+7*4 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			16*32, 17*32, 18*32, 19*32, 20*32, 21*32, 22*32, 23*32,
			64*32, 65*32, 66*32, 67*32, 68*32, 69*32, 70*32, 71*32,
			80*32, 81*32, 82*32, 83*32, 84*32, 85*32, 86*32, 87*32 },
            512 * 8	/* every sprite takes 512 consecutive bytes */
        );

        static Mame.GfxLayout rygar_spritelayout8x8 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 xprites */
            4096,	/* 8192 sprites */
            4,		/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the bitplanes are packed in one nibble */
            new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );



        static Mame.GfxDecodeInfo[] rygar_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, tecmo_charlayout,     256, 16 ),	/* colors 256 - 511 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, rygar_spritelayout8x8,  0, 16 ),	/* colors   0 - 255 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, rygar_spritelayout,     0, 16 ),	/* 16x16 sprites */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, rygar_spritelayout2x,   0, 16 ),	/* double size hack */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, rygar_spritelayout,   512, 16 ),	/* bg#1 colors 512 - 767 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX4, 0, rygar_spritelayout,   768, 16 ),	/* bg#2 colors 768 - 1023 */
};

        static YM3526interface rygar_ym3812_interface =
        new YM3526interface(
            1,			/* 1 chip (no more supported) */
            4000000,	/* 4 MHz ? */
            new int[] { 255 }		/* (not supported) */
        );

        class machine_driver_rygar : Mame.MachineDriver
        {
            public machine_driver_rygar()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 7600000, readmem, rygar_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 4000000, rygar_sound_readmem, rygar_sound_writemem, null, null, Mame.interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;

                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = rygar_gfxdecodeinfo;
                total_colors = 1024;
                color_table_len = 1024;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, rygar_ym3812_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_ADPCM, adpcm_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                video_type = 0;
                return tecmo_vh_start();
            }
            public override void vh_stop()
            {
                tecmo_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                tecmo_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_rygar()
        {

            ROM_START("rygar");
            ROM_REGION(0x18000, Mame.REGION_CPU1);/* 64k for code */
            ROM_LOAD("5.5p", 0x00000, 0x08000, 0x062cd55d); /* code */
            ROM_LOAD("cpu_5m.bin", 0x08000, 0x04000, 0x7ac5191b); /* code */
            ROM_LOAD("cpu_5j.bin", 0x10000, 0x08000, 0xed76d606); /* banked at f000-f7ff */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("cpu_4h.bin", 0x0000, 0x2000, 0xe4a2fa87);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("cpu_8k.bin", 0x00000, 0x08000, 0x4d482fb6);	/* characters */

            ROM_REGION(0x20000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("vid_6k.bin", 0x00000, 0x08000, 0xaba6db9e);	/* sprites */
            ROM_LOAD("vid_6j.bin", 0x08000, 0x08000, 0xae1f2ed6);	/* sprites */
            ROM_LOAD("vid_6h.bin", 0x10000, 0x08000, 0x46d9e7df);	/* sprites */
            ROM_LOAD("vid_6g.bin", 0x18000, 0x08000, 0x45839c9a);	/* sprites */

            ROM_REGION(0x20000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("vid_6p.bin", 0x00000, 0x08000, 0x9eae5f8e);
            ROM_LOAD("vid_6o.bin", 0x08000, 0x08000, 0x5a10a396);
            ROM_LOAD("vid_6n.bin", 0x10000, 0x08000, 0x7b12cf3f);
            ROM_LOAD("vid_6l.bin", 0x18000, 0x08000, 0x3cea7eaa);

            ROM_REGION(0x20000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("vid_6f.bin", 0x00000, 0x08000, 0x9840edd8);
            ROM_LOAD("vid_6e.bin", 0x08000, 0x08000, 0xff65e074);
            ROM_LOAD("vid_6c.bin", 0x10000, 0x08000, 0x89868c85);
            ROM_LOAD("vid_6b.bin", 0x18000, 0x08000, 0x35389a7b);

            ROM_REGION(0x4000, Mame.REGION_SOUND1);	/* ADPCM samples */
            ROM_LOAD("cpu_1f.bin", 0x0000, 0x4000, 0x3cc98c5a);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_rygar()
        {

            INPUT_PORTS_START("rygar");
            PORT_START();	/* IN0 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);

            PORT_START();	/* IN1 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN2 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);

            PORT_START();	/* IN3 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN4 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* DSWA bit 0-3 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x0C, 0x00, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0C, DEF_STR("1C_3C"));

            PORT_START();	/* DSWA bit 4-7 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x04, DEF_STR(Upright));
            PORT_DIPSETTING(0x00, DEF_STR(Cocktail));
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));

            PORT_START();	/* DSWB bit 0-3 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "50000 200000 500000");
            PORT_DIPSETTING(0x01, "100000 300000 600000");
            PORT_DIPSETTING(0x02, "200000 500000");
            PORT_DIPSETTING(0x03, "100000");
            PORT_DIPNAME(0x04, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));

            PORT_START();	/* DSWB bit 4-7 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x01, "Normal");
            PORT_DIPSETTING(0x02, "Hard");
            PORT_DIPSETTING(0x03, "Hardest");
            PORT_DIPNAME(0x04, 0x00, "2P Can Start Anytime");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x04, DEF_STR(Yes));
            PORT_DIPNAME(0x08, 0x08, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR(No)); ;
            PORT_DIPSETTING(0x08, DEF_STR(Yes));

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public driver_rygar()
        {
            drv = new machine_driver_rygar();
            year = "1986";
            name = "rygar";
            description = "Rygar (US set 1)";
            manufacturer = "Tecmo";
            flags = Mame.ROT0;
            input_ports = input_ports_rygar();
            rom = rom_rygar();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_silkworm : driver_tecmo
    {
        static Mame.MemoryWriteAddress[] silkworm_writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0xc000, 0xc1ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress(0xc200, 0xc3ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress(0xc400, 0xc5ff, tecmo_videoram_w, tecmo_videoram ),
	new Mame.MemoryWriteAddress(0xc600, 0xc7ff, tecmo_colorram_w, tecmo_colorram ),
	new Mame.MemoryWriteAddress(0xc800, 0xcbff, Mame.MWA_RAM, tecmo_videoram2, tecmo_videoram2_size ),
	new Mame.MemoryWriteAddress(0xcc00, 0xcfff, Mame.MWA_RAM, tecmo_colorram2 ),
	new Mame.MemoryWriteAddress(0xd000, 0xdfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress(0xe000, 0xe7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress(0xe800, 0xefff, Mame.paletteram_xxxxBBBBRRRRGGGG_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress(0xf000, 0xf7ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0xf800, 0xf805, Mame.MWA_RAM, tecmo_scroll ),
	new Mame.MemoryWriteAddress(0xf806, 0xf806, tecmo_sound_command_w ),
	new Mame.MemoryWriteAddress(0xf807, 0xf807, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress(0xf808, 0xf808, tecmo_bankswitch_w ),
	new Mame.MemoryWriteAddress(0xf809, 0xf809,Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress(0xf80b, 0xf80b, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress(-1 )	/* end of table */
};

        class machine_driver_silkworm : Mame.MachineDriver
        {
            public machine_driver_silkworm()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 7600000, readmem, silkworm_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 4000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_tecmo.gfxdecodeinfo;
                total_colors = 1024;
                color_table_len = 1024;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, ym3812_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_ADPCM, adpcm_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                video_type = 1;
                return tecmo_vh_start();
            }
            public override void vh_stop()
            {
                tecmo_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                tecmo_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        Mame.RomModule[] rom_silkworm()
        {
            ROM_START("silkworm");
            ROM_REGION(0x20000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("silkworm.4", 0x00000, 0x10000, 0xa5277cce);	/* c000-ffff is not used */
            ROM_LOAD("silkworm.5", 0x10000, 0x10000, 0xa6c7bb51);	/* banked at f000-f7ff */

            ROM_REGION(0x20000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("silkworm.3", 0x0000, 0x8000, 0xb589f587);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("silkworm.2", 0x00000, 0x08000, 0xe80a1cd9);	/* characters */

            ROM_REGION(0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("silkworm.6", 0x00000, 0x10000, 0x1138d159);	/* sprites */
            ROM_LOAD("silkworm.7", 0x10000, 0x10000, 0xd96214f7);	/* sprites */
            ROM_LOAD("silkworm.8", 0x20000, 0x10000, 0x0494b38e);	/* sprites */
            ROM_LOAD("silkworm.9", 0x30000, 0x10000, 0x8ce3cdf5);	/* sprites */

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("silkworm.10", 0x00000, 0x10000, 0x8c7138bb);	/* tiles #1 */
            ROM_LOAD("silkworm.11", 0x10000, 0x10000, 0x6c03c476);	/* tiles #1 */
            ROM_LOAD("silkworm.12", 0x20000, 0x10000, 0xbb0f568f);	/* tiles #1 */
            ROM_LOAD("silkworm.13", 0x30000, 0x10000, 0x773ad0a4);	/* tiles #1 */

            ROM_REGION(0x40000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("silkworm.14", 0x00000, 0x10000, 0x409df64b);	/* tiles #2 */
            ROM_LOAD("silkworm.15", 0x10000, 0x10000, 0x6e4052c9);	/* tiles #2 */
            ROM_LOAD("silkworm.16", 0x20000, 0x10000, 0x9292ed63);	/* tiles #2 */
            ROM_LOAD("silkworm.17", 0x30000, 0x10000, 0x3fa4563d);	/* tiles #2 */

            ROM_REGION(0x8000, Mame.REGION_SOUND1);	/* ADPCM samples */
            ROM_LOAD("silkworm.1", 0x0000, 0x8000, 0x5b553644);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_silkworm()
        {
            INPUT_PORTS_START("silkworm");
            PORT_START();	/* IN0 bit 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);

            PORT_START();	/* IN0 bit 4-7 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* unused? */

            PORT_START();	/* IN1 bit 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);

            PORT_START();	/* IN1 bit 4-7 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* unused? */

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* DSWA bit 0-3 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x0C, 0x00, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0C, DEF_STR("1C_3C"));

            PORT_START();	/* DSWA bit 4-7 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPNAME(0x04, 0x00, "A 7");	/* unused? */
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));

            PORT_START();	/* DSWB bit 0-3 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "50000 200000 500000");
            PORT_DIPSETTING(0x01, "100000 300000 800000");
            PORT_DIPSETTING(0x02, "50000 200000");
            PORT_DIPSETTING(0x03, "100000 300000");
            PORT_DIPSETTING(0x04, "50000");
            PORT_DIPSETTING(0x05, "100000");
            PORT_DIPSETTING(0x06, "200000");
            PORT_DIPSETTING(0x07, "None");
            PORT_DIPNAME(0x08, 0x00, "B 4");	/* unused? */
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));

            PORT_START();	/* DSWB bit 4-7 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x00, "0");
            PORT_DIPSETTING(0x01, "1");
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x05, "5");
            /* 0x06 and 0x07 are the same as 0x00 */
            PORT_DIPNAME(0x08, 0x00, "Allow Continue");
            PORT_DIPSETTING(0x08, DEF_STR(No));
            PORT_DIPSETTING(0x00, DEF_STR(Yes));

            PORT_START();	/* COIN */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            return INPUT_PORTS_END;

        }
        public driver_silkworm()
        {
            drv = new machine_driver_silkworm();
            year = "1988";
            name = "silkworm";
            description = "Silkworm (set 1)";
            manufacturer = "Tecmo";
            flags = Mame.ROT0;
            input_ports = input_ports_silkworm();
            rom = rom_silkworm();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_gemini : driver_tecmo
    {
        static Mame.MemoryWriteAddress[] gemini_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Mame.MWA_RAM, tecmo_videoram2, tecmo_videoram2_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Mame.MWA_RAM, tecmo_colorram2 ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd9ff, tecmo_videoram_w, tecmo_videoram ),
	new Mame.MemoryWriteAddress( 0xda00, 0xdbff, tecmo_colorram_w, tecmo_colorram ),
	new Mame.MemoryWriteAddress( 0xdc00, 0xddff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xde00, 0xdfff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Mame.paletteram_xxxxBBBBRRRRGGGG_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xe800, 0xefff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf7ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xf800, 0xf805, Mame.MWA_RAM, tecmo_scroll ),
	new Mame.MemoryWriteAddress( 0xf806, 0xf806, tecmo_sound_command_w ),
	new Mame.MemoryWriteAddress( 0xf807, 0xf807, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( 0xf808, 0xf808, tecmo_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xf809, 0xf809, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( 0xf80b, 0xf80b, Mame.MWA_NOP ),	/* ???? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        class machine_driver_gemini : Mame.MachineDriver
        {
            public machine_driver_gemini()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 7600000, readmem, gemini_writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 4000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_tecmo.gfxdecodeinfo;
                total_colors = 1024;
                color_table_len = 1024;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, ym3812_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_ADPCM, adpcm_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                video_type = 2;
                return tecmo_vh_start();
            }
            public override void vh_stop()
            {
                tecmo_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                tecmo_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        Mame.RomModule[] rom_gemini()
        {
            ROM_START("gemini");
            ROM_REGION(0x20000, Mame.REGION_CPU1);/* 64k for code */
            ROM_LOAD("gw04-5s.rom", 0x00000, 0x10000, 0xff9de855);	/* c000-ffff is not used */
            ROM_LOAD("gw05-6s.rom", 0x10000, 0x10000, 0x5a6947a9);	/* banked at f000-f7ff */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("gw03-5h.rom", 0x0000, 0x8000, 0x9bc79596);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gw02-3h.rom", 0x00000, 0x08000, 0x7acc8d35);	/* characters */

            ROM_REGION(0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gw06-1c.rom", 0x00000, 0x10000, 0x4ea51631);	/* sprites */
            ROM_LOAD("gw07-1d.rom", 0x10000, 0x10000, 0xda42637e);	/* sprites */
            ROM_LOAD("gw08-1f.rom", 0x20000, 0x10000, 0x0b4e8d70);	/* sprites */
            ROM_LOAD("gw09-1h.rom", 0x30000, 0x10000, 0xb65c5e4c);	/* sprites */

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gw10-1n.rom", 0x00000, 0x10000, 0x5e84cd4f);	/* tiles #1 */
            ROM_LOAD("gw11-2na.rom", 0x10000, 0x10000, 0x08b458e1);	/* tiles #1 */
            ROM_LOAD("gw12-2nb.rom", 0x20000, 0x10000, 0x229c9714);	/* tiles #1 */
            ROM_LOAD("gw13-3n.rom", 0x30000, 0x10000, 0xc5dfaf47);	/* tiles #1 */

            ROM_REGION(0x40000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gw14-1r.rom", 0x00000, 0x10000, 0x9c10e5b5);	/* tiles #2 */
            ROM_LOAD("gw15-2ra.rom", 0x10000, 0x10000, 0x4cd18cfa);	/* tiles #2 */
            ROM_LOAD("gw16-2rb.rom", 0x20000, 0x10000, 0xf911c7be);	/* tiles #2 */
            ROM_LOAD("gw17-3r.rom", 0x30000, 0x10000, 0x79a9ce25);	/* tiles #2 */

            ROM_REGION(0x8000, Mame.REGION_SOUND1);	/* ADPCM samples */
            ROM_LOAD("gw01-6a.rom", 0x0000, 0x8000, 0xd78afa05);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_gemini()
        {
            INPUT_PORTS_START("gemini");
            PORT_START();	/* IN0 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);

            PORT_START();	/* IN1 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN2 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);

            PORT_START();	/* IN3 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN4 bits 0-3 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);

            PORT_START();	/* DSWA bit 0-3 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x06, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x08, 0x00, "Final Round Continuation");
            PORT_DIPSETTING(0x00, "Round 6");
            PORT_DIPSETTING(0x08, "Round 7");

            PORT_START();	/* DSWA bit 4-7 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x06, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x08, 0x00, "Buy in During Final Round");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x08, DEF_STR(Yes));

            PORT_START();	/* DSWB bit 0-3 */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x04, "Normal");
            PORT_DIPSETTING(0x08, "Hard");
            PORT_DIPSETTING(0x0c, "Hardest");

            PORT_START();	/* DSWB bit 4-7 */
            PORT_DIPNAME(0x07, 0x00, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "50000 200000");
            PORT_DIPSETTING(0x01, "50000 300000");
            PORT_DIPSETTING(0x02, "100000 500000");
            PORT_DIPSETTING(0x03, "50000");
            PORT_DIPSETTING(0x04, "100000");
            PORT_DIPSETTING(0x05, "200000");
            PORT_DIPSETTING(0x06, "300000");
            PORT_DIPSETTING(0x07, "None");
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();	/* unused? */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public driver_gemini()
        {
            drv = new machine_driver_gemini();
            year = "1987";
            name = "gemini";
            description = "Gemini Wing";
            manufacturer = "Tecmo";
            flags = Mame.ROT90;
            input_ports = input_ports_gemini();
            rom = rom_gemini();
            drv.HasNVRAMhandler = false;
        }
    }
}
