using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_brkthru : Mame.GameDriver
    {
        static _BytePtr brkthru_nmi_enable = new _BytePtr(1);
        static _BytePtr brkthru_videoram = new _BytePtr(1);
        static int[] brkthru_videoram_size = new int[1];
        static int nmi_enable;
        static _BytePtr brkthru_scroll = new _BytePtr(1);
        static int bgscroll, bgbasecolor, flipscreen;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Mame.MRA_RAM ),		/* Plane 0: Text */
	new Mame.MemoryReadAddress( 0x0400, 0x0bff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0c00, 0x0fff, Mame.MRA_RAM ),		/* Plane 2  Background */
	new Mame.MemoryReadAddress( 0x1000, 0x10ff, Mame.MRA_RAM ),		/* Plane 1: Sprites */
	new Mame.MemoryReadAddress( 0x1100, 0x17ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x1800, 0x1800, Mame.input_port_0_r),	/* player controls, player start */
	new Mame.MemoryReadAddress( 0x1801, 0x1801, Mame.input_port_1_r),	/* cocktail player controls */
	new Mame.MemoryReadAddress( 0x1802, 0x1802, Mame.input_port_3_r),	/* DSW 0 */
	new Mame.MemoryReadAddress( 0x1803, 0x1803, Mame.input_port_2_r),	/* coin input & DSW */
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x4000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x03ff, Mame.MWA_RAM, brkthru_videoram, brkthru_videoram_size ),
	new Mame.MemoryWriteAddress( 0x0400, 0x0bff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0c00, 0x0fff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x1000, 0x10ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x1100, 0x17ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1800, 0x1801, brkthru_1800_w ),	/* bg scroll and color, ROM bank selection, flip screen */
	new Mame.MemoryWriteAddress( 0x1802, 0x1802, brkthru_soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x1803, 0x1803, brkthru_1803_w ),	/* NMI enable, + ? */
	new Mame.MemoryWriteAddress( 0x2000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x4000, 0x4000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, ym2203.YM2203_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, YM3812.YM3812_control_port_0_w  ),
	new Mame.MemoryWriteAddress( 0x2001, 0x2001, YM3812.YM3812_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x6001, 0x6001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        
static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
	8,8,	/* 8*8 chars */
	256,	/* 256 characters */
	3,	/* 3 bits per pixel */
	new uint[]{ 512*8*8+4, 0, 4 },	/* plane offset */
	new uint[]{ 256*8*8+0, 256*8*8+1, 256*8*8+2, 256*8*8+3, 0, 1, 2, 3 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	8*8	/* every char takes 8 consecutive bytes */
);

static Mame.GfxLayout tilelayout1 =
new Mame.GfxLayout(
	16,16,	/* 16*16 tiles */
	128,	/* 128 tiles */
	3,	/* 3 bits per pixel */
	new uint[]{ 0x4000*8+4, 0, 4 },	/* plane offset */
	new uint[]{ 0, 1, 2, 3, 1024*8*8+0, 1024*8*8+1, 1024*8*8+2, 1024*8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+1024*8*8+0, 16*8+1024*8*8+1, 16*8+1024*8*8+2, 16*8+1024*8*8+3 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
	32*8	/* every tile takes 32 consecutive bytes */
);

static Mame. GfxLayout tilelayout2 =
new Mame.GfxLayout(
	16,16,	/* 16*16 tiles */
	128,	/* 128 tiles */
	3,	/* 3 bits per pixel */
	new uint[]{ 0x3000*8+0, 0, 4 },	/* plane offset */
	new uint[]{ 0, 1, 2, 3, 1024*8*8+0, 1024*8*8+1, 1024*8*8+2, 1024*8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+1024*8*8+0, 16*8+1024*8*8+1, 16*8+1024*8*8+2, 16*8+1024*8*8+3 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
	32*8	/* every tile takes 32 consecutive bytes */
);

static Mame.GfxLayout spritelayout =
new Mame.GfxLayout(
	16,16,	/* 16*16 sprites */
	1024,	/* 1024 sprites */
	3,	/* 3 bits per pixel */
	new uint[]{ 2*1024*32*8, 1024*32*8, 0 },	/* plane offset */
	new uint[]{ 16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7,
			0, 1, 2, 3, 4, 5, 6, 7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
	32*8	/* every sprite takes 32 consecutive bytes */
);

static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x00000,charlayout,      0, 1 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x00000,tilelayout1, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x01000,tilelayout2, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x08000,tilelayout1, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x09000,tilelayout2, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x10000,tilelayout1, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x11000,tilelayout2, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x18000,tilelayout1, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x19000,tilelayout2, 8+8*8, 16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x00000,spritelayout,    8, 8 ),
};

static void irqhandler(int linestate)
{
    Mame.cpu_set_irq_line(1, 0, linestate);
    //cpu_cause_interrupt(1,M6809_INT_IRQ);
}
        static int brkthru_interrupt()
        {
            if (Mame.cpu_getiloops() == 0)
            {
                if (nmi_enable != 0) return Mame.nmi_interrupt();
            }
            else
            {
                /* generate IRQ on coin insertion */
                if ((Mame.readinputport(2) & 0xe0) != 0xe0) return Mame.interrupt();
            }

            return Mame.ignore_interrupt();
        }
        static void brkthru_1803_w(int offset, int data)
        {
            /* bit 0 = NMI enable */
            nmi_enable = ~data & 1;

            /* bit 1 = ? maybe IRQ acknowledge */
        }
        static void darwin_0803_w(int offset, int data)
        {
            /* bit 0 = NMI enable */
            /*nmi_enable = ~data & 1;*/
            //if (errorlog) fprintf(errorlog, "0803 %02X\n", data);
            nmi_enable = data;
            /* bit 1 = ? maybe IRQ acknowledge */
        }

        static void brkthru_soundlatch_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.cpu_cause_interrupt(1, Mame.cpu_m6809.M6809_INT_NMI);
        }
        static void brkthru_1800_w(int offset,int data)
{
	if (offset == 0)	/* low 8 bits of scroll */
		bgscroll = (bgscroll & 0x100) | data;
	else if (offset == 1)
	{
		int bankaddress;
		_BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


		/* bit 0-2 = ROM bank select */
		bankaddress = 0x10000 + (data & 0x07) * 0x2000;
        Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));

		/* bit 3-5 = background tiles color code */
		if (((data & 0x38) >> 2) != bgbasecolor)
		{
			bgbasecolor = (data & 0x38) >> 2;
            Generic.SetDirtyBuffer(true);
		}

		/* bit 6 = screen flip */
		if (flipscreen != (data & 0x40))
		{
			flipscreen = data & 0x40;
            Generic.SetDirtyBuffer(true);
		}

		/* bit 7 = high bit of scroll */
		bgscroll = (bgscroll & 0xff) | ((data & 0x80) << 1);
	}
}
        static YM2203interface ym2203_interface =
new YM2203interface(
	1,
	1500000,	/* Unknown */
	new int[]{ ym2203.YM2203_VOL(25,25) },
	new AY8910portRead[]{null},
	new AY8910portRead[]{null},
	new AY8910portWrite[]{null},
	new AY8910portWrite[]{null},
    new AY8910handler[]{null}
);

static YM3526interface ym3526_interface =
new YM3526interface(
	1,			/* 1 chip (no more supported) */
	3000000,	/* 3.000000 MHz ? (partially supported) */
	new int[]{ 255 },		/* (not supported) */
	new handlerdelegate[]{ irqhandler }
);

        class machine_driver_brkthru : Mame.MachineDriver
        {
            public machine_driver_brkthru()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1250000, readmem, writemem, null, null, brkthru_interrupt, 2));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1250000, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 58;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 1 * 8, 31 * 8 - 1);
                gfxdecodeinfo = driver_brkthru.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 8 + 8 * 8 + 16 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3526, ym3526_interface));
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
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0, bit1, bit2, bit3;


                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi] >> 4) & 0x01;
                    bit1 = (color_prom[cpi] >> 5) & 0x01;
                    bit2 = (color_prom[cpi] >> 6) & 0x01;
                    bit3 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                /* characters use colors 0-7 */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable, 0, i, i);

                /* background tiles use colors 128-255 */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable, 1, i, i + 128);

                /* sprites use colors 64-127 */
                for (int i = 0; i < TOTAL_COLORS(9); i++)
                    COLOR(colortable, 9, i, i + 64);
            }
            public override int vh_start()
            {
                Generic.dirtybuffer = new bool[Generic.videoram_size[0]];

                Generic.SetDirtyBuffer(true);

                /* the background area is twice as wide as the screen */
                Generic.tmpbitmap = Mame.osd_create_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(Generic.tmpbitmap);
            }
            public override void vh_eof_callback()
            {
                //
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                for (int offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    if (Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1])
                    {
                        Generic.dirtybuffer[offs] = Generic.dirtybuffer[offs + 1] = false;

                        int sx = (offs / 2) / 16;
                        int sy = (offs / 2) % 16;
                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 15 - sy;
                        }

                        int code = Generic.videoram[offs] + 256 * (Generic.videoram[offs + 1] & 3);
                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[1 + (code >> 7)],
                                (uint)(code & 0x7f),
                                (uint)(bgbasecolor + ((Generic.videoram[offs + 1] & 0x04) >> 2)),
                                flipscreen != 0, flipscreen != 0,
                                16 * sx, 16 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the background graphics */
                {
                    int scroll;


                    if (flipscreen != 0) scroll = 256 + bgscroll;
                    else scroll = -bgscroll;
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[] { scroll }, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
                {
                    if ((Generic.spriteram[offs] & 0x01) != 0)	/* enable */
                    {

                        /* the meaning of bit 3 of [offs] is unknown */

                        int sx = 240 - Generic.spriteram[offs + 3];
                        if (sx < -7) sx += 256;
                        int sy = 240 - Generic.spriteram[offs + 2];
                        int code = Generic.spriteram[offs + 1] + 128 * (Generic.spriteram[offs] & 0x06);
                        int color = (Generic.spriteram[offs] & 0xe0) >> 5;
                        if (flipscreen != 0)
                        {
                            sx = 240 - sx;
                            sy = 240 - sy;
                        }

                        if ((Generic.spriteram[offs] & 0x10) != 0)	/* double height */
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)(code & ~1),
                                    (uint)(color),
                                    flipscreen != 0, flipscreen != 0,
                                    sx, flipscreen != 0 ? sy + 16 : sy - 16,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)(code | 1),
                                    (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);

                            /* redraw with wraparound */
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)(code & ~1),
                                    (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, (flipscreen != 0 ? sy + 16 : sy - 16) + 256,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)(code | 1),
                                    (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy + 256,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        else
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)code,
                                   (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);

                            /* redraw with wraparound */
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[9],
                                    (uint)code,
                                   (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy + 256,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }


                /* draw the frontmost playfield. They are characters, but draw them as sprites */
                for (int offs = brkthru_videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx = offs % 32;
                    int sy = offs / 32;
                    if (flipscreen != 0)
                    {
                        sx = 31 - sx;
                        sy = 31 - sy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            brkthru_videoram[offs],
                            0,
                            flipscreen != 0, flipscreen != 0,
                            8 * sx, 8 * sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_brkthru()
        {
            ROM_START( "brkthru" );
	ROM_REGION( 0x20000, Mame.REGION_CPU1 ) ;    /* 64k for main CPU + 64k for banked ROMs */
	ROM_LOAD( "brkthru.1",    0x04000, 0x4000, 0xcfb4265f );
	ROM_LOAD( "brkthru.2",    0x08000, 0x8000, 0xfa8246d9 );
	ROM_LOAD( "brkthru.4",    0x10000, 0x8000, 0x8cabf252 );
	ROM_LOAD( "brkthru.3",    0x18000, 0x8000, 0x2f2c40c2 );

	ROM_REGION( 0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE );
	ROM_LOAD( "brkthru.12",   0x00000, 0x2000, 0x58c0b29b );	/* characters */

	ROM_REGION( 0x20000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE );
	/* background */
	/* we do a lot of scatter loading here, to place the data in a format */
	/* which can be decoded by MAME's standard functions */
	ROM_LOAD( "brkthru.7",    0x00000, 0x4000, 0x920cc56a );	/* bitplanes 1,2 for bank 1,2 */
	ROM_CONTINUE(             0x08000, 0x4000 )				;/* bitplanes 1,2 for bank 3,4 */
	ROM_LOAD( "brkthru.6",    0x10000, 0x4000, 0xfd3cee40 );	/* bitplanes 1,2 for bank 5,6 */
	ROM_CONTINUE(             0x18000, 0x4000 )				;/* bitplanes 1,2 for bank 7,8 */
	ROM_LOAD( "brkthru.8",    0x04000, 0x1000, 0xf67ee64e )	;/* bitplane 3 for bank 1,2 */
	ROM_CONTINUE(             0x06000, 0x1000 );
	ROM_CONTINUE(             0x0c000, 0x1000 )	;			/* bitplane 3 for bank 3,4 */
	ROM_CONTINUE(             0x0e000, 0x1000 );
	ROM_CONTINUE(             0x14000, 0x1000 )	;			/* bitplane 3 for bank 5,6 */
	ROM_CONTINUE(             0x16000, 0x1000 );
	ROM_CONTINUE(             0x1c000, 0x1000 )	;			/* bitplane 3 for bank 7,8 */
	ROM_CONTINUE(             0x1e000, 0x1000 );

	ROM_REGION( 0x18000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE );
	ROM_LOAD( "brkthru.9",    0x00000, 0x8000, 0xf54e50a7 )	;/* sprites */
	ROM_LOAD( "brkthru.10",   0x08000, 0x8000, 0xfd156945 );
	ROM_LOAD( "brkthru.11",   0x10000, 0x8000, 0xc152a99b );

	ROM_REGION( 0x0200, Mame.REGION_PROMS );
	ROM_LOAD( "brkthru.13",   0x0000, 0x0100, 0xaae44269 ) ;/* red and green component */
	ROM_LOAD( "brkthru.14",   0x0100, 0x0100, 0xf2d4822a ) ;/* blue component */

	ROM_REGION( 0x10000, Mame.REGION_CPU2 );	/* 64K for sound CPU */
	ROM_LOAD( "brkthru.5",    0x8000, 0x8000, 0xc309435f );
return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_brkthru()
        {
            INPUT_PORTS_START("brkthru");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_VBLANK);/* used only by the self test */

            PORT_START();	/* IN2 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x01, "5");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "99", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Unknown));
            PORT_DIPSETTING(0x04, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x10, DEF_STR(Yes));
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 2);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 2);
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 2);

            PORT_START();      /* DSW0 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x00, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x00, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x10, 0x10, DEF_STR(Unknown));
            PORT_DIPSETTING(0x10, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Unknown));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x40, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x40, DEF_STR(Cocktail));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            return INPUT_PORTS_END;
        }
        public driver_brkthru()
        {
            drv = new machine_driver_brkthru();
            year = "1986";
            name = "brkthru";
            description = "Break Thru (US)";
            manufacturer ="Data East USA";
            flags = Mame.ROT0;
            input_ports = input_ports_brkthru();
            rom = rom_brkthru();
            drv.HasNVRAMhandler = false;
        }
    }
}
