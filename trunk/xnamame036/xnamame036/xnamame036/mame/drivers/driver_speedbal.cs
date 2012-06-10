using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_speedbal : Mame.GameDriver
    {
        static _BytePtr speedbal_foreground_videoram = new _BytePtr(1);
        static _BytePtr speedbal_background_videoram = new _BytePtr(1);
        static _BytePtr speedbal_sprites_dataram = new _BytePtr(1);

        static int[] speedbal_foreground_videoram_size = new int[1];
        static int[] speedbal_background_videoram_size = new int[1];
        static int[] speedbal_sprites_dataram_size = new int[1];

        static _BytePtr speedbal_sharedram = new _BytePtr(1);

        static bool[] bg_dirtybuffer, ch_dirtybuffer;
        static Mame.osd_bitmap bitmap_bg, bitmap_ch;


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,    /* 8*8 characters */
            1024,   /* 1024 characters */
            4,      /* actually 2 bits per pixel - two of the planes are empty */
            new uint[] { 1024 * 16 * 8 + 4, 1024 * 16 * 8 + 0, 4, 0 },
            new uint[] { 8 + 3, 8 + 2, 8 + 1, 8 + 0, 3, 2, 1, 0 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },   /* characters are rotated 90 degrees */
            16 * 8	   /* every char takes 16 bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            16, 16,  /* 16*16 tiles */
            1024,   /* 1024 tiles */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 2, 4, 6 }, /* the bitplanes are packed in one nibble */
            new uint[]{ 0*8+0, 0*8+1, 7*8+0, 7*8+1, 6*8+0, 6*8+1, 5*8+0, 5*8+1,
			4*8+0, 4*8+1, 3*8+0, 3*8+1, 2*8+0, 2*8+1, 1*8+0, 1*8+1 },
            new uint[]{ 0*64, 1*64, 2*64, 3*64, 4*64, 5*64, 6*64, 7*64,
			8*64, 9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64 },
            128 * 8  /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,  /* 16*16 sprites */
            512,    /* 512 sprites */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 2, 4, 6 }, /* the bitplanes are packed in one nibble */
            new uint[]{ 7*8+1, 7*8+0, 6*8+1, 6*8+0, 5*8+1, 5*8+0, 4*8+1, 4*8+0,
			3*8+1, 3*8+0, 2*8+1, 2*8+0, 1*8+1, 1*8+0, 0*8+1, 0*8+0 },
            new uint[]{ 0*64, 1*64, 2*64, 3*64, 4*64, 5*64, 6*64, 7*64,
			8*64, 9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64 },
            128 * 8  /* every sprite takes 128 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,	 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tilelayout,	 512, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout,   0, 16 ),
};



        static YM3812interface ym3812_interface =
        new YM3812interface(
            1,		      /* 1 chip (no more supported) */
            3600000,	/* 3.600000 MHz ? (partially supported) */
            new int[] { 255 }	 /* (not supported) */
        );


        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress (0x0000, 0xdbff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress (0xdc00, 0xdfff, speedbal_sharedram_r ),  // shared with SOUND
	new Mame.MemoryReadAddress (0xe000, 0xe1ff, speedbal_background_videoram_r ),
	new Mame.MemoryReadAddress (0xe800, 0xefff, speedbal_foreground_videoram_r ),
	new Mame.MemoryReadAddress (0xf000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress (-1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xdbff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xdc00, 0xdfff, speedbal_sharedram_w, speedbal_sharedram ),  // shared with SOUND
	new Mame.MemoryWriteAddress( 0xe000, 0xe1ff, speedbal_background_videoram_w, speedbal_background_videoram, speedbal_background_videoram_size ),
	new Mame.MemoryWriteAddress( 0xe800, 0xefff, speedbal_foreground_videoram_w, speedbal_foreground_videoram, speedbal_foreground_videoram_size ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf5ff, Mame.paletteram_RRRRGGGGBBBBxxxx_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xf600, 0xfeff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xff00, 0xffff, Mame.MWA_RAM, speedbal_sprites_dataram, speedbal_sprites_dataram_size ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xdc00, 0xdfff, speedbal_sharedram_r ), // shared with MAIN CPU
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xdc00, 0xdfff, speedbal_sharedram_w ), // shared with MAIN CPU
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.IOReadPort[] readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r ),
	new Mame.IOReadPort( 0x10, 0x10, Mame.input_port_1_r ),
	new Mame.IOReadPort( 0x20, 0x20, Mame.input_port_2_r ),
	new Mame.IOReadPort( 0x30, 0x30, Mame.input_port_3_r ),
	new Mame.IOReadPort( -1 )  /* end of table */
};

        static Mame.IOWritePort[] writeport =
{
	new Mame.IOWritePort( -1 )  /* end of table */
};

        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort(0x00, 0x00, YM3812.YM3812_status_port_0_r ),
	new Mame.IOReadPort(-1 )  /* end of table */
};

        static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort(0x00, 0x00, YM3812.YM3812_control_port_0_w ),
	new Mame.IOWritePort(0x01, 0x01, YM3812.YM3812_write_port_0_w ),
	new Mame.IOWritePort(-1 )  /* end of table */
};

        static int speedbal_sharedram_r(int offset)
        {
            //  if (offset==0x0) speedbal_sharedram[offset]+=1;
            return speedbal_sharedram[offset];
        }
        static void speedbal_sharedram_w(int offset, int data)
        {
            speedbal_sharedram[offset] = (byte)data;
        }
        static void speedbal_foreground_videoram_w(int offset, int data)
        {
            ch_dirtybuffer[offset] = true;
            speedbal_foreground_videoram[offset] = (byte)data;
        }

        static int speedbal_foreground_videoram_r(int offset)
        {
            return speedbal_foreground_videoram[offset];
        }

        static void speedbal_background_videoram_w(int offset, int data)
        {
            bg_dirtybuffer[offset] = true;
            speedbal_background_videoram[offset] = (byte)data;
        }

        static int speedbal_background_videoram_r(int offset)
        {
            return speedbal_background_videoram[offset];
        }


        class machine_driver_speedbal : Mame.MachineDriver
        {
            public machine_driver_speedbal()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, readport, writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 2660000, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.interrupt, 8));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);


                gfxdecodeinfo = driver_speedbal.gfxdecodeinfo;
                total_colors = 768;
                color_table_len = 768;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                //sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, ym3812_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                return;
                uint cpi = 0, pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
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
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }


                /* characters */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable, 0, i, color_prom[cpi++]);

                /* tiles */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable, 1, i, color_prom[cpi++] & 0x0f);

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                    COLOR(colortable, 2, i, color_prom[cpi++] & 0x0f);

            }
            public override int vh_start()
            {
                bg_dirtybuffer = new bool[speedbal_background_videoram_size[0]];

                ch_dirtybuffer = new bool[speedbal_foreground_videoram_size[0]];

                /* foreground bitmap */
                bitmap_ch = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

                /* background bitmap */
                bitmap_bg = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width * 2, Mame.Machine.drv.screen_height * 2, Mame.Machine.scrbitmap.depth);
                for (int i = 0; i < speedbal_foreground_videoram_size[0] / 2; i++)
                {
                    ch_dirtybuffer[i] = true;
                }
                for (int i = 0; i < speedbal_background_videoram_size[0] / 2; i++)
                    bg_dirtybuffer[i] = true;

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(bitmap_ch);
                Mame.osd_free_bitmap(bitmap_bg);
                bg_dirtybuffer = null;
                ch_dirtybuffer = null;
            }
            public override void vh_eof_callback()
            {
                //none
            }
            const byte SPRITE_X = 0, SPRITE_NUMBER = 1, SPRITE_PALETTE = 2, SPRITE_Y = 3;
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {



                Mame.palette_init_used_colors();

                {

                    int[] colmask = new int[16];
                    int pal_base;


                    pal_base = Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start;

                    for (int color = 0; color < 16; color++) colmask[color] = 0;

                    for (int offs = 0; offs < speedbal_background_videoram_size[0]; offs += 2)
                    {
                        int code = speedbal_background_videoram[offs];
                        int color = speedbal_background_videoram[offs + 1];
                        code += (color & 0x30) << 4;
                        color = (color & 0x0f);
                        colmask[color] |= (int)Mame.Machine.gfx[1].pen_usage[code];
                    }

                    for (int color = 0; color < 16; color++)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            if ((colmask[color] & (1 << i)) != 0)
                                Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                        }
                    }


                    pal_base = Mame.Machine.drv.gfxdecodeinfo[2].color_codes_start;

                    for (int color = 0; color < 16; color++) colmask[color] = 0;

                    for (int offs = 0; offs < speedbal_sprites_dataram_size[0]; offs += 4)
                    {
                        _BytePtr SPTRegs;
                        int carac, f;

                        SPTRegs = new _BytePtr(speedbal_sprites_dataram, offs);

                        carac = SPTRegs[SPRITE_NUMBER];
                        int code = 0;
                        for (f = 0; f < 8; f++) code += ((carac >> f) & 1) << (7 - f);
                        int color = (SPTRegs[SPRITE_PALETTE] & 0x0f);

                        if ((SPTRegs[SPRITE_PALETTE] & 0x40) == 0) code += 256;
                        colmask[color] |= (int)Mame.Machine.gfx[2].pen_usage[code];
                    }

                    for (int color = 0; color < 16; color++)
                    {
                        if ((colmask[color] & (1 << 0)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                        for (int i = 1; i < 16; i++)
                        {
                            if ((colmask[color] & (1 << i)) != 0)
                                Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                        }
                    }


                    pal_base = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;

                    for (int color = 0; color < 16; color++) colmask[color] = 0;

                    for (int offs = 0; offs < speedbal_foreground_videoram_size[0]; offs += 2)
                    {
                        int code = speedbal_foreground_videoram[offs];
                        int color = speedbal_foreground_videoram[offs + 1];
                        code += (color & 0x30) << 4;
                        color = (color & 0x0f);
                        colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
                    }

                    for (int color = 0; color < 16; color++)
                    {
                        if ((colmask[color] & (1 << 0)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                        for (int i = 1; i < 16; i++)
                        {
                            if ((colmask[color] & (1 << i)) != 0)
                                Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                        }
                    }

                    if (Mame.palette_recalc() != null)
                    {
                        for (int i = 0; i < speedbal_foreground_videoram_size[0] / 2; i++)
                            ch_dirtybuffer[i] = true;
                        for (int i = 0; i < speedbal_background_videoram_size[0] / 2; i++)
                            bg_dirtybuffer[i] = true;
                    }
                }

                // first background
                speedbal_draw_background(bitmap_bg);
                Mame.copybitmap(bitmap, bitmap_bg, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                // second characters (general)
                speedbal_draw_foreground1(bitmap_ch);
                Mame.copybitmap(bitmap, bitmap_ch, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

                // thirth sprites
                speedbal_draw_sprites(bitmap);

            }
            static void speedbal_draw_sprites(Mame.osd_bitmap bitmap)
            {
                int SPTX, SPTY, SPTTile, SPTColor, offset, f;
                byte carac;
                _BytePtr SPTRegs;

                /* Drawing sprites: 64 in total */

                for (offset = 0; offset < speedbal_sprites_dataram_size[0]; offset += 4)
                {
                    SPTRegs = new _BytePtr(speedbal_sprites_dataram, offset);


                    SPTX = 243 - SPTRegs[SPRITE_Y];
                    SPTY = 239 - SPTRegs[SPRITE_X];

                    carac = SPTRegs[SPRITE_NUMBER];
                    SPTTile = 0;
                    for (f = 0; f < 8; f++) SPTTile += ((carac >> f) & 1) << (7 - f);
                    SPTColor = (SPTRegs[SPRITE_PALETTE] & 0x0f);

                    if ((SPTRegs[SPRITE_PALETTE] & 0x40) == 0) SPTTile += 256;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                            (uint)SPTTile,
                            (uint)SPTColor,
                            false, false,
                            SPTX, SPTY,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }

            static void speedbal_draw_background(Mame.osd_bitmap bitmap)
            {
                int sx, sy, code, tile, offset, color;

                for (offset = 0; offset < speedbal_background_videoram_size[0]; offset += 2)
                {
                    if (bg_dirtybuffer[offset])
                    {
                        bg_dirtybuffer[offset] = false;

                        tile = speedbal_background_videoram[offset + 0];
                        code = speedbal_background_videoram[offset + 1];
                        tile += (code & 0x30) << 4;
                        color = (code & 0x0f);

                        sx = 15 - (offset / 2) / 16;
                        sy = (offset / 2) % 16;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)tile,
                                (uint)color,
                                false, false,
                                16 * sx, 16 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }
            }
            static void speedbal_draw_foreground1(Mame.osd_bitmap bitmap)
            {
                int sx, sy, code, caracter, color, offset;

                for (offset = 0; offset < speedbal_foreground_videoram_size[0]; offset += 2)
                {
                    if (ch_dirtybuffer[offset])
                    {
                        caracter = speedbal_foreground_videoram[offset];
                        code = speedbal_foreground_videoram[offset + 1];
                        caracter += (code & 0x30) << 4;

                        color = (code & 0x0f);

                        sx = 31 - (offset / 2) / 32;
                        sy = (offset / 2) % 32;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)caracter,
                                (uint)color,
                                false, false,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);

                        ch_dirtybuffer[offset] = false;
                    }

                }
            }

        }

        public override void driver_init()
        {
            for (int i = 0; i < Mame.memory_region_length(Mame.REGION_GFX3); i++)
                Mame.memory_region(Mame.REGION_GFX3)[i] ^= 0xff;
        }
        Mame.RomModule[] rom_speedbal()
        {
            ROM_START("speedbal");
            ROM_REGION(0x10000, Mame.REGION_CPU1);  /* 64K for code: main */
            ROM_LOAD("sb1.bin", 0x0000, 0x8000, 0x1c242e34);
            ROM_LOAD("sb3.bin", 0x8000, 0x8000, 0x7682326a);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64K for second CPU: sound */
            ROM_LOAD("sb2.bin", 0x0000, 0x8000, 0xe6a6d9b7);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sb10.bin", 0x00000, 0x08000, 0x36dea4bf);  /* chars */

            ROM_REGION(0x20000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sb9.bin", 0x00000, 0x08000, 0xb567e85e);   /* bg tiles */
            ROM_LOAD("sb5.bin", 0x08000, 0x08000, 0xb0eae4ba);
            ROM_LOAD("sb8.bin", 0x10000, 0x08000, 0xd2bfbdb6);
            ROM_LOAD("sb4.bin", 0x18000, 0x08000, 0x1d23a130);

            ROM_REGION(0x10000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sb6.bin", 0x00000, 0x08000, 0x0e2506eb);    /* sprites */
            ROM_LOAD("sb7.bin", 0x08000, 0x08000, 0x9f1b33d1);
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_speedbal()
        {

            INPUT_PORTS_START("speedbal");
            PORT_START("DSW2");
            PORT_DIPNAME(0x07, 0x07, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x06, "70000 200000 1M");
            PORT_DIPSETTING(0x07, "70000 200000");
            PORT_DIPSETTING(0x03, "100000 300000 1M");
            PORT_DIPSETTING(0x04, "100000 300000");
            PORT_DIPSETTING(0x01, "200000 1M");
            PORT_DIPSETTING(0x05, "200000");
            /*	PORT_DIPSETTING(    0x02, "200000" ) */
            ;
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x08, 0x08, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x30, 0x30, "Difficulty 1");
            PORT_DIPSETTING(0x30, "Very Easy");
            PORT_DIPSETTING(0x20, "Easy");
            PORT_DIPSETTING(0x10, "Difficult");
            PORT_DIPSETTING(0x00, "Very Difficult");
            PORT_DIPNAME(0xc0, 0xc0, "Difficulty 2");
            PORT_DIPSETTING(0xc0, "Very Easy");
            PORT_DIPSETTING(0x80, "Easy");
            PORT_DIPSETTING(0x40, "Difficult");
            PORT_DIPSETTING(0x00, "Very Difficult");

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_5C"));
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x20, "4");
            PORT_DIPSETTING(0x10, "5");
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x40, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4 | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;

        }
        public driver_speedbal()
        {
            drv = new machine_driver_speedbal();
            year = "1987";
            name = "speedbal";
            description = "Speed Ball";
            manufacturer = "Tecfri";
            flags = Mame.ROT270;
            input_ports = input_ports_speedbal();
            rom = rom_speedbal();
            drv.HasNVRAMhandler = false;
        }
    }
}
