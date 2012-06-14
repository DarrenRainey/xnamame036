using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_1943 : Mame.GameDriver
    {
        static _BytePtr c1943_scrollx = new _BytePtr(1);
        static _BytePtr c1943_scrolly = new _BytePtr(1);
        static _BytePtr c1943_bgscrolly = new _BytePtr(1);

        static int chon, objon, sc1on, sc2on;
        static int flipscreen;

        static Mame.osd_bitmap sc2bitmap;
        static Mame.osd_bitmap sc1bitmap;
        static byte[][][] sc1map, sc2map;

        /* this is a protection check. The game crashes (thru a jump to 0x8000) */
        /* if a read from this address doesn't return the value it expects. */
        static int c1943_protection_r(int offset)
        {
            int data = (int)(Mame.cpu_get_reg(Mame.cpu_Z80.Z80_BC) >> 8);
            Mame.printf("protection read, PC: %04x Result:%02x\n", Mame.cpu_get_pc(), data);
            return  data;
        }



        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(0x8000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress(0xd000, 0xd7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(0xc000, 0xc000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress(0xc001, 0xc001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress(0xc002, 0xc002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress(0xc003, 0xc003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress(0xc004, 0xc004, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress(0xc007, 0xc007, c1943_protection_r ),
	new Mame.MemoryReadAddress(0xe000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, c1943_c804_w ),	/* ROM bank switch, screen flip */
	new Mame.MemoryWriteAddress( 0xc806, 0xc806, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0xc807, 0xc807, Mame.MWA_NOP ), 	/* protection chip write (we don't emulate it) */
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd801, Mame.MWA_RAM, c1943_scrolly ),
	new Mame.MemoryWriteAddress( 0xd802, 0xd802, Mame.MWA_RAM, c1943_scrollx ),
	new Mame.MemoryWriteAddress( 0xd803, 0xd804, Mame.MWA_RAM, c1943_bgscrolly ),
	new Mame.MemoryWriteAddress( 0xd806, 0xd806, c1943_d806_w ),	/* sprites, bg1, bg2 enable */
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xc7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc800, 0xc800, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe001, 0xe001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe002, 0xe002, ym2203.YM2203_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xe003, 0xe003, ym2203.YM2203_write_port_1_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
    8, 8,	/* 8*8 characters */
    2048,	/* 2048 characters */
    2,	/* 2 bits per pixel */
    new uint[] { 4, 0 },
    new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
    new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
    16 * 8	/* every char takes 16 consecutive bytes */
);
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            2048,	/* 2048 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 2048 * 64 * 8 + 4, 2048 * 64 * 8 + 0, 4, 0 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3, 32 * 8 + 0, 32 * 8 + 1, 32 * 8 + 2, 32 * 8 + 3, 33 * 8 + 0, 33 * 8 + 1, 33 * 8 + 2, 33 * 8 + 3 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16, 8 * 16, 9 * 16, 10 * 16, 11 * 16, 12 * 16, 13 * 16, 14 * 16, 15 * 16 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );
        static Mame.GfxLayout fgtilelayout =
        new Mame.GfxLayout(
            32, 32,  /* 32*32 tiles */
            512,    /* 512 tiles */
            4,      /* 4 bits per pixel */
            new uint[] { 512 * 256 * 8 + 4, 512 * 256 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			64*8+0, 64*8+1, 64*8+2, 64*8+3, 65*8+0, 65*8+1, 65*8+2, 65*8+3,
			128*8+0, 128*8+1, 128*8+2, 128*8+3, 129*8+0, 129*8+1, 129*8+2, 129*8+3,
			192*8+0, 192*8+1, 192*8+2, 192*8+3, 193*8+0, 193*8+1, 193*8+2, 193*8+3 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16,
			16*16, 17*16, 18*16, 19*16, 20*16, 21*16, 22*16, 23*16,
			24*16, 25*16, 26*16, 27*16, 28*16, 29*16, 30*16, 31*16 },
            256 * 8	/* every tile takes 256 consecutive bytes */
        );
        static Mame.GfxLayout bgtilelayout =
        new Mame.GfxLayout(
            32, 32,  /* 32*32 tiles */
            128,    /* 128 tiles */
            4,      /* 4 bits per pixel */
            new uint[] { 128 * 256 * 8 + 4, 128 * 256 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			64*8+0, 64*8+1, 64*8+2, 64*8+3, 65*8+0, 65*8+1, 65*8+2, 65*8+3,
			128*8+0, 128*8+1, 128*8+2, 128*8+3, 129*8+0, 129*8+1, 129*8+2, 129*8+3,
			192*8+0, 192*8+1, 192*8+2, 192*8+3, 193*8+0, 193*8+1, 193*8+2, 193*8+3 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16,
			16*16, 17*16, 18*16, 19*16, 20*16, 21*16, 22*16, 23*16,
			24*16, 25*16, 26*16, 27*16, 28*16, 29*16, 30*16, 31*16 },
            256 * 8	/* every tile takes 256 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,                  0, 32 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, fgtilelayout,             32*4, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, bgtilelayout,       32*4+16*16, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX4, 0, spritelayout, 32*4+16*16+16*16, 16 ),
};


        static YM2203interface ym2203_interface =
        new YM2203interface(
            2,			/* 2 chips */
            1500000,	/* 1.5 MHz */
            new int[] { ym2203.YM2203_VOL(10, 15), ym2203.YM2203_VOL(10, 15) },
            new AY8910portRead[] { null, null },
            new AY8910portRead[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null }, null
        );

        static void c1943_c804_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            /* bits 0 and 1 are coin counters */
            Mame.coin_counter_w(0, data & 1);
            Mame.coin_counter_w(1, data & 2);

            /* bits 2, 3 and 4 select the ROM bank */
            bankaddress = 0x10000 + (data & 0x1c) * 0x1000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));

            /* bit 5 resets the sound CPU - we ignore it */

            /* bit 6 flips screen */
            if (flipscreen != (data & 0x40))
            {
                flipscreen = data & 0x40;
            }

            /* bit 7 enables characters */
            chon = data & 0x80;
        }
        static void c1943_d806_w(int offset, int data)
        {
            /* bit 4 enables bg 1 */
            sc1on = data & 0x10;

            /* bit 5 enables bg 2 */
            sc2on = data & 0x20;

            /* bit 6 enables sprites */
            objon = data & 0x40;
        }
        class machine_driver_1943 : Mame.MachineDriver
        {
            public machine_driver_1943()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 6000000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 4));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_1943.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 32 * 4 + 16 * 16 + 16 * 16 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint cpi = 0, pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                cpi += (2 * Mame.Machine.drv.total_colors);
                /* color_prom now points to the beginning of the lookup table */

                /* characters use colors 64-79 */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable,0,i, (ushort)((color_prom[cpi++]) + 64));
                
                cpi += 128;	/* skip the bottom half of the PROM - not used */

                /* foreground tiles use colors 0-63 */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    /* color 0 MUST map to pen 0 in order for transparency to work */
                    if ((i % Mame.Machine.gfx[1].color_granularity) == 0)
                        COLOR(colortable,1,i, 0);
                    else
                        COLOR(colortable,1,i, color_prom[cpi] + 16 * (color_prom[cpi+256] & 0x03));
                    cpi++;
                }
                cpi += (uint)TOTAL_COLORS(1);

                /* background tiles use colors 0-63 */
                for (int i = 0; i <TOTAL_COLORS(2); i++)
                {
                    COLOR(colortable, 2,i, (ushort)(color_prom[cpi] + 16 * (color_prom[cpi+256] & 0x03)));
                    cpi++;
                }
                cpi += (uint)TOTAL_COLORS(2);

                /* sprites use colors 128-255 */
                /* bit 3 of BMPROM.07 selects priority over the background, but we handle */
                /* it differently for speed reasons */
                for (int i = 0; i <TOTAL_COLORS(3); i++)
                {
                    COLOR(colortable, 3, i, (ushort)(color_prom[cpi] + 16 * (color_prom[cpi+256] & 0x07) + 128));
                    cpi++;
                }
                color_prom.offset = (int)(cpi + TOTAL_COLORS(3));   
            }
            public override int vh_start()
            {
                sc2bitmap = Mame.osd_create_bitmap(9 * 32, 8 * 32);
                sc1bitmap = Mame.osd_create_bitmap(9 * 32, 9 * 32);

                if (Generic.generic_vh_start() == 1)
                {
                    Mame.osd_free_bitmap(sc2bitmap);
                    Mame.osd_free_bitmap(sc1bitmap);
                    return 1;
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            sc1map[i][j][l] = 0xff;
                            if (j < 8)
                                sc2map[i][j][l] = 0xff;
                        }
                    }
                }

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(sc2bitmap);
                Mame.osd_free_bitmap(sc1bitmap);
            }
            public override void vh_eof_callback()
            {
                //none
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int  bg_scrollx;
                int top, left, xscroll, yscroll;

                /* TODO: support flipscreen */
                if (sc2on != 0)
                {
                    _BytePtr p = new _BytePtr(Mame.memory_region(Mame.REGION_GFX5), 0x8000);
                   int bg_scrolly = c1943_bgscrolly[0] + 256 * c1943_bgscrolly[1];
                   int offs = 16 * ((bg_scrolly >> 5) + 8);

                    top = 8 - (bg_scrolly >> 5) % 9;

                    bg_scrolly &= 0x1f;

                    for (int sy = 0; sy < 9; sy++)
                    {
                        int ty = (sy + top) % 9;
                        byte[][] map = sc2map[ty];
                        int mpi = 0;
                        offs &= 0x7fff; /* Enforce limits (for top of scroll) */
                      
                        for (int sx = 0; sx < 8; sx++)
                        {
                            int offset = offs + 2 * sx;
                            int tile = p[offset];
                            int attr = p[offset + 1];

                            if (tile != map[mpi][0] || attr != map[mpi][1])
                            {
                                map[mpi][0] = (byte)tile;
                                map[mpi][1] = (byte)attr;
                                Mame.drawgfx(sc2bitmap, Mame.Machine.gfx[2],
                                        (uint)tile,
                                        (uint)((attr & 0x3c) >> 2),
                                        (attr & 0x40) != 0, (attr & 0x80) != 0,
                                        (8 - ty) * 32, sx * 32,
                                        null,
                                        Mame.TRANSPARENCY_NONE, 0);
                            }
                            mpi++;
                        }
                        offs -= 0x10;
                    }

                    xscroll = (top * 32 - bg_scrolly);
                    yscroll = 0;
                    Mame.copyscrollbitmap(bitmap, sc2bitmap,
                        1, new int[] { xscroll },
                        1, new int[] { yscroll },
                        Mame.Machine.drv.visible_area,
                        Mame.TRANSPARENCY_NONE, 0);
                }
                else 
                    Mame.fillbitmap(bitmap, Mame.Machine.pens[0], Mame.Machine.drv.visible_area);


                if (objon != 0)
                {
                    /* Draw the sprites which don't have priority over the foreground. */
                    for (int offs = Generic.spriteram_size[0] - 32; offs >= 0; offs -= 32)
                    {
                        int color = Generic.spriteram[offs + 1] & 0x0f;
                        if (color == 0x0a || color == 0x0b)	/* the priority is actually selected by */
                        /* bit 3 of BMPROM.07 */
                        {
                            int sx = Generic.spriteram[offs + 3] - ((Generic.spriteram[offs + 1] & 0x10) << 4);
                            int sy = Generic.spriteram[offs + 2];
                            if (flipscreen != 0)
                            {
                                sx = 240 - sx;
                                sy = 240 - sy;
                            }

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                    (uint)(Generic.spriteram[offs] + ((Generic.spriteram[offs + 1] & 0xe0) << 3)),
                                    (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }


                /* TODO: support flipscreen */
                if (sc1on != 0)
                {
                    _BytePtr p = Mame.memory_region(Mame.REGION_GFX5);

                    int bg_scrolly = c1943_scrolly[0] + 256 * c1943_scrolly[1];
                    bg_scrollx = c1943_scrollx[0];
                    int offs = 16 * ((bg_scrolly >> 5) + 8) + 2 * (bg_scrollx >> 5);
                    if ((bg_scrollx & 0x80) != 0) offs -= 0x10;

                    top = 8 - (bg_scrolly >> 5) % 9;
                    left = (bg_scrollx >> 5) % 9;

                    bg_scrolly &= 0x1f;
                    bg_scrollx &= 0x1f;

                    for (int sy = 0; sy < 9; sy++)
                    {
                        int ty = (sy + top) % 9;
                        offs &= 0x7fff; /* Enforce limits (for top of scroll) */

                        for (int sx = 0; sx < 9; sx++)
                        {
                            int tile, attr, offset;
                            int tx = (sx + left) % 9;
                            byte[] map = sc1map[ty][tx];
                            offset = offs + (sx * 2);

                            tile = p[offset];
                            attr = p[offset + 1];

                            if (tile != map[0] || attr != map[1])
                            {
                                map[0] = (byte)tile;
                                map[1] = (byte)attr;
                                tile += 256 * (attr & 0x01);
                                Mame.drawgfx(sc1bitmap, Mame.Machine.gfx[1],
                                        (uint)tile,
                                        (uint)((attr & 0x3c) >> 2),
                                        (attr & 0x40) != 0, (attr & 0x80) != 0,
                                        (8 - ty) * 32, tx * 32,
                                        null,
                                        Mame.TRANSPARENCY_NONE, 0);
                            }
                        }
                        offs -= 0x10;
                    }

                    xscroll = (top * 32 - bg_scrolly);
                    yscroll = -(left * 32 + bg_scrollx);
                    Mame.copyscrollbitmap(bitmap, sc1bitmap,
                       1, new int[] { xscroll },
                        1, new int[] { yscroll },
                        Mame.Machine.drv.visible_area,
                        Mame.TRANSPARENCY_COLOR, 0);
                }

                if (objon != 0)
                {
                    /* Draw the sprites which have priority over the foreground. */
                    for (int offs = Generic.spriteram_size[0] - 32; offs >= 0; offs -= 32)
                    {
                        int color = Generic.spriteram[offs + 1] & 0x0f;
                        if (color != 0x0a && color != 0x0b)	/* the priority is actually selected by */
                        /* bit 3 of BMPROM.07 */
                        {
                            int sx = Generic.spriteram[offs + 3] - ((Generic.spriteram[offs + 1] & 0x10) << 4);
                            int sy = Generic.spriteram[offs + 2];
                            if (flipscreen != 0)
                            {
                                sx = 240 - sx;
                                sy = 240 - sy;
                            }

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                    (uint)(Generic.spriteram[offs] + ((Generic.spriteram[offs + 1] & 0xe0) << 3)),
                                    (uint)color,
                                    flipscreen != 0, flipscreen != 0,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }


                if (chon != 0)
                {
                    /* draw the frontmost playfield. They are characters, but draw them as sprites */
                    for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                    {
                        int sx = offs % 32;
                        int sy = offs / 32;
                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + ((Generic.colorram[offs] & 0xe0) << 3)),
                                (uint)(Generic.colorram[offs] & 0x1f),
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 79);
                    }
                }
            }
        }
        Mame.InputPortTiny[] input_ports_1943()
        {
            INPUT_PORTS_START("1943");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* actually, this is VBLANK */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* Button 3, probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* Button 3, probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("DSW0");
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x0f, "1 (Easiest)");
            PORT_DIPSETTING(0x0e, "2");
            PORT_DIPSETTING(0x0d, "3");
            PORT_DIPSETTING(0x0c, "4");
            PORT_DIPSETTING(0x0b, "5");
            PORT_DIPSETTING(0x0a, "6");
            PORT_DIPSETTING(0x09, "7");
            PORT_DIPSETTING(0x08, "8");
            PORT_DIPSETTING(0x07, "9");
            PORT_DIPSETTING(0x06, "10");
            PORT_DIPSETTING(0x05, "11");
            PORT_DIPSETTING(0x04, "12");
            PORT_DIPSETTING(0x03, "13");
            PORT_DIPSETTING(0x02, "14");
            PORT_DIPSETTING(0x01, "15");
            PORT_DIPSETTING(0x00, "16 (Hardest)");
            PORT_DIPNAME(0x10, 0x10, "2 Players Game");
            PORT_DIPSETTING(0x00, "1 Credit");
            PORT_DIPSETTING(0x10, "2 Credits");
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x40, "Freeze");
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x07, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_5C"));
            PORT_DIPNAME(0x38, 0x38, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x38, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x28, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x18, DEF_STR("1C_5C"));
            PORT_DIPNAME(0x40, 0x40, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x40, DEF_STR("Yes"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_1943()
        {

            ROM_START("1943");
            ROM_REGION(0x30000, Mame.REGION_CPU1);	/* 64k for code + 128k for the banked ROMs images */
            ROM_LOAD("1943.01", 0x00000, 0x08000, 0xc686cc5c);
            ROM_LOAD("1943.02", 0x10000, 0x10000, 0xd8880a41);
            ROM_LOAD("1943.03", 0x20000, 0x10000, 0x3f0ee26c);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("1943.05", 0x00000, 0x8000, 0xee2bd2d7);

            ROM_REGION(0x8000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1943.04", 0x00000, 0x8000, 0x46cb9d3d);	/* characters */

            ROM_REGION(0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1943.15", 0x00000, 0x8000, 0x6b1a0443);	/* bg tiles */
            ROM_LOAD("1943.16", 0x08000, 0x8000, 0x23c908c2);
            ROM_LOAD("1943.17", 0x10000, 0x8000, 0x46bcdd07);
            ROM_LOAD("1943.18", 0x18000, 0x8000, 0xe6ae7ba0);
            ROM_LOAD("1943.19", 0x20000, 0x8000, 0x868ababc);
            ROM_LOAD("1943.20", 0x28000, 0x8000, 0x0917e5d4);
            ROM_LOAD("1943.21", 0x30000, 0x8000, 0x9bfb0d89);
            ROM_LOAD("1943.22", 0x38000, 0x8000, 0x04f3c274);

            ROM_REGION(0x10000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1943.24", 0x00000, 0x8000, 0x11134036);	/* fg tiles */
            ROM_LOAD("1943.25", 0x08000, 0x8000, 0x092cf9c1);

            ROM_REGION(0x40000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1943.06", 0x00000, 0x8000, 0x97acc8af);	/* sprites */
            ROM_LOAD("1943.07", 0x08000, 0x8000, 0xd78f7197);
            ROM_LOAD("1943.08", 0x10000, 0x8000, 0x1a626608);
            ROM_LOAD("1943.09", 0x18000, 0x8000, 0x92408400);
            ROM_LOAD("1943.10", 0x20000, 0x8000, 0x8438a44a);
            ROM_LOAD("1943.11", 0x28000, 0x8000, 0x6c69351d);
            ROM_LOAD("1943.12", 0x30000, 0x8000, 0x5e7efdb7);
            ROM_LOAD("1943.13", 0x38000, 0x8000, 0x1143829a);

            ROM_REGION(0x10000, Mame.REGION_GFX5);	/* tilemaps */
            ROM_LOAD("1943.14", 0x0000, 0x8000, 0x4d3c6401);	/* front background */
            ROM_LOAD("1943.23", 0x8000, 0x8000, 0xa52aecbd);	/* back background */

            ROM_REGION(0x0c00, Mame.REGION_PROMS);
            ROM_LOAD("bmprom.01", 0x0000, 0x0100, 0x74421f18);	/* red component */
            ROM_LOAD("bmprom.02", 0x0100, 0x0100, 0xac27541f);	/* green component */
            ROM_LOAD("bmprom.03", 0x0200, 0x0100, 0x251fb6ff);	/* blue component */
            ROM_LOAD("bmprom.05", 0x0300, 0x0100, 0x206713d0);	/* char lookup table */
            ROM_LOAD("bmprom.10", 0x0400, 0x0100, 0x33c2491c);	/* foreground lookup table */
            ROM_LOAD("bmprom.09", 0x0500, 0x0100, 0xaeea4af7);	/* foreground palette bank */
            ROM_LOAD("bmprom.12", 0x0600, 0x0100, 0xc18aa136);	/* background lookup table */
            ROM_LOAD("bmprom.11", 0x0700, 0x0100, 0x405aae37);	/* background palette bank */
            ROM_LOAD("bmprom.08", 0x0800, 0x0100, 0xc2010a9e);	/* sprite lookup table */
            ROM_LOAD("bmprom.07", 0x0900, 0x0100, 0xb56f30c3);	/* sprite palette bank */
            ROM_LOAD("bmprom.04", 0x0a00, 0x0100, 0x91a8a2e1);	/* priority encoder / palette selector (not used) */
            ROM_LOAD("bmprom.06", 0x0b00, 0x0100, 0x0eaf5158);	/* video timing (not used) */
            return ROM_END;
        }
        public override void driver_init()
        {
            sc1map = new byte[9][][];
            for (int i = 0; i < 9; i++)
            {
                sc1map[i] = new byte[9][];
                for (int j = 0; j < 9; j++)
                    sc1map[i][j] = new byte[2];
            }
            sc2map = new byte[9][][];
            for (int i = 0; i < 9; i++)
            {
                sc2map[i] = new byte[8][];
                for (int j = 0; j < 8; j++)
                    sc2map[i][j] = new byte[2];

            }
        }
        public driver_1943()
        {
            drv = new machine_driver_1943();
            year = "1987";
            name = "1943";
            description = "1943 - The Battle of Midway (US)";
            manufacturer = "Capcom";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_1943();
            rom = rom_1943();
            drv.HasNVRAMhandler = false;
        }
    }
}
