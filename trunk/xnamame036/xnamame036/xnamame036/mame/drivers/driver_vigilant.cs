using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_vigilant : Mame.GameDriver
    {

        static Mame.rectangle topvisiblearea =
        new Mame.rectangle(
            16 * 8, 48 * 8 - 1,
            0 * 8, 6 * 8 - 1
        );
        static Mame.rectangle bottomvisiblearea =
        new Mame.rectangle(
            16 * 8, 48 * 8 - 1,
            6 * 8, 32 * 8 - 1
        );

        static _BytePtr vigilant_paletteram = new _BytePtr(1);
        static _BytePtr vigilant_sprite_paletteram = new _BytePtr(1);

        static int horiz_scroll_low = 0;
        static int horiz_scroll_high = 0;
        static int rear_horiz_scroll_low = 0;
        static int rear_horiz_scroll_high = 0;
        static int rear_color = 0;
        static int rear_disable = 1;

        static int rear_refresh = 1;

        static Mame.osd_bitmap bg_bitmap;


        static Mame.MemoryReadAddress[] vigilant_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc020, 0xc0df, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc800, 0xcfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xd000, 0xdfff, Generic.videoram_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] vigilant_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff,Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc020, 0xc0df,Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xc800, 0xcfff, vigilant_paletteram_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.IOReadPort[] vigilant_readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r ),
	new Mame.IOReadPort( 0x01, 0x01, Mame.input_port_1_r ),
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r ),
	new Mame.IOReadPort( 0x03, 0x03, Mame.input_port_3_r ),
	new Mame.IOReadPort( 0x04, 0x04, Mame.input_port_4_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] vigilant_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, M72.m72_sound_command_w ),  /* SD */
	new Mame.IOWritePort( 0x01, 0x01, vigilant_out2_w ), /* OUT2 */
	new Mame.IOWritePort( 0x04, 0x04, vigilant_bank_select_w ), /* PBANK */
	new Mame.IOWritePort( 0x80, 0x81, vigilant_horiz_scroll_w ), /* HSPL, HSPH */
	new Mame.IOWritePort( 0x82, 0x83, vigilant_rear_horiz_scroll_w ), /* RHSPL, RHSPH */
	new Mame.IOWritePort( 0x84, 0x84, vigilant_rear_color_w ), /* RCOD */
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(0xf000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff,Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff,Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x01, 0x01, YM2151.YM2151_status_port_0_r ),
	new Mame.IOReadPort( 0x80, 0x80, Mame.soundlatch_r ),	/* SDRE */
	new Mame.IOReadPort( 0x84, 0x84, M72.m72_sample_r ),	/* S ROM C */
	new Mame.IOReadPort( -1)	/* end of table */
};

        static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, YM2151.YM2151_register_port_0_w ),
	new Mame.IOWritePort( 0x01, 0x01, YM2151.YM2151_data_port_0_w ),
	new Mame.IOWritePort( 0x80, 0x81, M72.vigilant_sample_addr_w ),	/* STL / STH */
	new Mame.IOWritePort( 0x82, 0x82, M72.m72_sample_w ),			/* COUNT UP */
	new Mame.IOWritePort( 0x83, 0x83, M72.m72_sound_irq_ack_w ),	/* IRQ clear */
	new Mame.IOWritePort( -1 )	/* end of table */
};



        static void vigilant_rear_color_w(int offset, int data)
        {
            rear_disable = data & 0x40;
            rear_color = (data & 0x0d);
        }
        static void vigilant_rear_horiz_scroll_w(int offset, int data)
        {
            if (offset == 0)
                rear_horiz_scroll_low = data;
            else
                rear_horiz_scroll_high = (data & 0x07) * 256;
        }
        static void vigilant_horiz_scroll_w(int offset, int data)
        {
            if (offset == 0)
                horiz_scroll_low = data;
            else
                horiz_scroll_high = (data & 0x01) * 256;
        }
        static void vigilant_paletteram_w(int offset, int data)
        {
            int bank, r, g, b;

            Mame.paletteram[offset] = (byte)data;

            bank = offset & 0x400;
            offset &= 0xff;

            r = (Mame.paletteram[bank + offset + 0x000] << 3) & 0xFF;
            g = (Mame.paletteram[bank + offset + 0x100] << 3) & 0xFF;
            b = (Mame.paletteram[bank + offset + 0x200] << 3) & 0xFF;

            Mame.palette_change_color((bank >> 2) + offset, (byte)r, (byte)g, (byte)b);
        }
        static void vigilant_bank_select_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            bankaddress = 0x10000 + (data & 0x07) * 0x4000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }
        static void vigilant_out2_w(int offset, int data)
        {
            /* D0 = FILP = Flip screen? */
            /* D1 = COA1 = Coin Counter A? */
            /* D2 = COB1 = Coin Counter B? */

            /* The hardware has both coin counters hooked up to a single meter. */
            Mame.coin_counter_w(0, data & 0x02);
            Mame.coin_counter_w(1, data & 0x04);
        }


        static Mame.GfxLayout text_layout =
        new Mame.GfxLayout(
            8, 8, /* tile size */
            4096, /* number of tiles */
            4, /* bits per pixel */
            new uint[] { 64 * 1024 * 8, 64 * 1024 * 8 + 4, 0, 4 }, /* plane offsets */
            new uint[] { 0, 1, 2, 3, 64 + 0, 64 + 1, 64 + 2, 64 + 3 }, /* x offsets */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 }, /* y offsets */
            128
        );

        static Mame.GfxLayout sprite_layout =
        new Mame.GfxLayout(
            16, 16,	/* tile size */
            4096,	/* number of sprites ($1000) */
            4,		/* bits per pixel */
            new uint[] { 0x40000 * 8, 0x40000 * 8 + 4, 0, 4 }, /* plane offsets */
            new uint[]{ /* x offsets */
		0x00*8+0,0x00*8+1,0x00*8+2,0x00*8+3,
		0x10*8+0,0x10*8+1,0x10*8+2,0x10*8+3,
		0x20*8+0,0x20*8+1,0x20*8+2,0x20*8+3,
		0x30*8+0,0x30*8+1,0x30*8+2,0x30*8+3
	},
            new uint[]{ /* y offsets */
		0x00*8, 0x01*8, 0x02*8, 0x03*8,
		0x04*8, 0x05*8, 0x06*8, 0x07*8,
		0x08*8, 0x09*8, 0x0A*8, 0x0B*8,
		0x0C*8, 0x0D*8, 0x0E*8, 0x0F*8
	},
            0x40 * 8
        );

        static Mame.GfxLayout back_layout =
        new Mame.GfxLayout(
            32, 1, /* tile size */
            3 * 512 * 8, /* number of tiles */
            4, /* bits per pixel */
            new uint[] { 0, 2, 4, 6 }, /* plane offsets */
            new uint[]{ 0*8+1, 0*8,  1*8+1, 1*8, 2*8+1, 2*8, 3*8+1, 3*8, 4*8+1, 4*8, 5*8+1, 5*8,
	6*8+1, 6*8, 7*8+1, 7*8, 8*8+1, 8*8, 9*8+1, 9*8, 10*8+1, 10*8, 11*8+1, 11*8,
	12*8+1, 12*8, 13*8+1, 13*8, 14*8+1, 14*8, 15*8+1, 15*8 }, /* x offsets */
            new uint[] { 0 }, /* y offsets */
            16 * 8
        );

        static Mame.GfxDecodeInfo[] vigilant_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, text_layout,   256, 16 ),	/* colors 256-511 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, sprite_layout,   0, 16 ),	/* colors   0-255 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, back_layout,   512,  2 ),	/* actually the background uses colors */
													/* 256-511, but giving it exclusive */
													/* pens we can handle it more easily. */
};

        //static struct GfxDecodeInfo kikcubic_gfxdecodeinfo[] =
        //{
        //    { REGION_GFX1, 0, &text_layout,   0, 16 },
        //    { REGION_GFX2, 0, &sprite_layout, 0, 16 },
        //    { -1 } /* end of array */
        //};



        static YM2151interface ym2151_interface =
        new YM2151interface(
            1,			/* 1 chip */
            3579645,	/* 3.579645 MHz */
            new int[] { YM2151.YM3012_VOL(55, Mame.MIXER_PAN_LEFT, 55, Mame.MIXER_PAN_RIGHT) },
           new YM2151irqhandler[] { M72.m72_ym2151_irq_handler },
            new Mame.mem_write_handler[] { null }
        );

        static Mame.DACinterface dac_interface =
        new Mame.DACinterface(
            1,
            new int[] { 100 }
        );

        class machine_driver_vigilant : Mame.MachineDriver
        {
            public machine_driver_vigilant()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3579645, vigilant_readmem, vigilant_writemem, vigilant_readport, vigilant_writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3579645, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.nmi_interrupt, 128));
                frames_per_second = 55;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(16 * 8, (64 - 16) * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = vigilant_gfxdecodeinfo;
                total_colors = 512 + 32;
                color_table_len = 512 + 32;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                //xxxsound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                //xxxsound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                M72.m72_init_sound();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
            }
            public override int vh_start()
            {
                Generic.generic_vh_start();

                if ((bg_bitmap = Mame.osd_create_bitmap(512 * 3, 256)) == null)
                {
                    Generic.generic_vh_stop();
                    return 1;
                }

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(bg_bitmap);
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                if (rear_disable != 0)	 /* opaque foreground */
                {
                    for (int i = 0; i < 8; i++)
                        Mame.palette_used_colors[256 + 16 * i] = Mame.PALETTE_COLOR_USED;
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                        Mame.palette_used_colors[256 + 16 * i] = Mame.PALETTE_COLOR_TRANSPARENT;
                }


                /* copy the background palette */
                for (int i = 0; i < 16; i++)
                {
                    int r, g, b;


                    r = (Mame.paletteram[0x400 + 16 * rear_color + i] << 3) & 0xFF;
                    g = (Mame.paletteram[0x500 + 16 * rear_color + i] << 3) & 0xFF;
                    b = (Mame.paletteram[0x600 + 16 * rear_color + i] << 3) & 0xFF;

                    Mame.palette_change_color(512 + i, (byte)r, (byte)g, (byte)b);

                    r = (Mame.paletteram[0x400 + 16 * rear_color + 32 + i] << 3) & 0xFF;
                    g = (Mame.paletteram[0x500 + 16 * rear_color + 32 + i] << 3) & 0xFF;
                    b = (Mame.paletteram[0x600 + 16 * rear_color + 32 + i] << 3) & 0xFF;

                    Mame.palette_change_color(512 + 16 + i, (byte)r, (byte)g, (byte)b);
                }

                if (Mame.palette_recalc() != null)
                {
                    Generic.SetDirtyBuffer(true);
                    //memset(dirtybuffer, 1, videoram_size);
                    rear_refresh = 1;
                }

                if (rear_disable != 0)	 /* opaque foreground */
                {
                    draw_foreground(bitmap, 0, 1);
                    draw_sprites(bitmap, bottomvisiblearea);
                    draw_foreground(bitmap, 1, 1);
                }
                else
                {
                    draw_background(bitmap);
                    draw_foreground(bitmap, 0, 0);
                    draw_sprites(bitmap, bottomvisiblearea);
                    draw_foreground(bitmap, 1, 0); // priority tiles
                }
            }
            static void update_background()
            {
                uint charcode = 0;

                /* There are only three background ROMs */
                for (int page = 0; page < 3; page++)
                {
                    for (int row = 0; row < 256; row++)
                    {
                        for (int col = 0; col < 512; col += 32)
                        {
                            Mame.drawgfx(bg_bitmap,
                                    Mame.Machine.gfx[2],
                                    charcode,
                                    row < 128 ? 0u : 1u,
                                    false, false,
                                    512 * page + col, row,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                            charcode++;
                        }
                    }
                }
            }
            static void draw_sprites(Mame.osd_bitmap bitmap, Mame.rectangle clip)
            {
                int offs;

                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
                {
                    int code, color, sx, sy, flipx, flipy, h, y;

                    code = Generic.spriteram[offs + 4] | ((Generic.spriteram[offs + 5] & 0x0f) << 8);
                    color = Generic.spriteram[offs + 0] & 0x0f;
                    sx = (Generic.spriteram[offs + 6] | ((Generic.spriteram[offs + 7] & 0x01) << 8));
                    sy = 256 + 128 - (Generic.spriteram[offs + 2] | ((Generic.spriteram[offs + 3] & 0x01) << 8));
                    flipx = Generic.spriteram[offs + 5] & 0x40;
                    flipy = Generic.spriteram[offs + 5] & 0x80;
                    h = 1 << ((Generic.spriteram[offs + 5] & 0x30) >> 4);
                    sy -= 16 * h;

                    for (y = 0; y < h; y++)
                    {
                        int c = code;

                        if (flipy != 0) c += h - 1 - y;
                        else c += y;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)c,
                                (uint)color,
                                flipx != 0, flipy != 0,
                                sx, sy + 16 * y,
                                clip, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }

            static void draw_background(Mame.osd_bitmap bitmap)
            {
                int scrollx = 0x17a + 16 * 8 - (rear_horiz_scroll_low + rear_horiz_scroll_high);


                if (rear_refresh != 0)
                {
                    update_background();
                    rear_refresh = 0;
                }

                Mame.copyscrollbitmap(bitmap, bg_bitmap, 1, new int[] { scrollx }, 0, new int[] { 0 }, bottomvisiblearea, Mame.TRANSPARENCY_NONE, 0);
            }
            static void draw_foreground(Mame.osd_bitmap bitmap, int priority, int opaque)
            {
                int offs;
                int scroll = -(horiz_scroll_low + horiz_scroll_high);


                for (offs = 0; offs < Generic.videoram_size[0]; offs += 2)
                {
                    int sy = 8 * ((offs / 2) / 64);
                    int sx = 8 * ((offs / 2) % 64);
                    int attributes = Generic.videoram[offs + 1];
                    int color = attributes & 0x0F;
                    int tile_number = Generic.videoram[offs] | ((attributes & 0xF0) << 4);

                    if (priority != 0)	 /* foreground */
                    {
                        if ((color & 0x0c) == 0x0c)	/* mask sprites */
                        {
                            if (sy >= 48)
                            {
                                sx = (sx + scroll) & 0x1ff;

                                if (sx > 16 * 8 - 8 && sx < 48 * 8)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                            (uint)tile_number,
                                            (uint)color,
                                            false, false,
                                            sx, sy,
                                            bottomvisiblearea, Mame.TRANSPARENCY_PENS, 0x00ff);
                                }
                            }
                        }
                    }
                    else	 /* background */
                    {
                        if (Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1])
                        {
                            Generic.dirtybuffer[offs] = Generic.dirtybuffer[offs + 1] = false;

                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    (uint)tile_number,
                                    (uint)color,
                                    false, false,
                                    sx, sy,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }

                if (priority == 0)
                {
                    Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, topvisiblearea, Mame.TRANSPARENCY_NONE, 0);
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[] { scroll }, 0, new int[] { 0 }, bottomvisiblearea,
                            opaque != 0 ? Mame.TRANSPARENCY_NONE : Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);
                }
            }

            public override void vh_eof_callback()
            {
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_vigilant()
        {
            ROM_START("vigilant");
            ROM_REGION(0x30000, Mame.REGION_CPU1); /* 64k for code + 128k for bankswitching */
            ROM_LOAD("g07_c03.bin", 0x00000, 0x08000, 0x9dcca081);
            ROM_LOAD("j07_c04.bin", 0x10000, 0x10000, 0xe0159105);
            /* 0x20000-0x2ffff empty */

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64k for sound */
            ROM_LOAD("g05_c02.bin", 0x00000, 0x10000, 0x10582b2d);

            ROM_REGION(0x20000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("f05_c08.bin", 0x00000, 0x10000, 0x01579d20);
            ROM_LOAD("h05_c09.bin", 0x10000, 0x10000, 0x4f5872f0);

            ROM_REGION(0x80000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("n07_c12.bin", 0x00000, 0x10000, 0x10af8eb2);
            ROM_LOAD("k07_c10.bin", 0x10000, 0x10000, 0x9576f304);
            ROM_LOAD("o07_c13.bin", 0x20000, 0x10000, 0xb1d9d4dc);
            ROM_LOAD("l07_c11.bin", 0x30000, 0x10000, 0x4598be4a);
            ROM_LOAD("t07_c16.bin", 0x40000, 0x10000, 0xf5425e42);
            ROM_LOAD("p07_c14.bin", 0x50000, 0x10000, 0xcb50a17c);
            ROM_LOAD("v07_c17.bin", 0x60000, 0x10000, 0x959ba3c7);
            ROM_LOAD("s07_c15.bin", 0x70000, 0x10000, 0x7f2e91c5);

            ROM_REGION(0x30000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("d01_c05.bin", 0x00000, 0x10000, 0x81b1ee5c);
            ROM_LOAD("e01_c06.bin", 0x10000, 0x10000, 0xd0d33673);
            ROM_LOAD("f01_c07.bin", 0x20000, 0x10000, 0xaae81695);

            ROM_REGION(0x10000, Mame.REGION_SOUND1); /* samples */
            ROM_LOAD("d04_c01.bin", 0x00000, 0x10000, 0x9b85101d);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_vigilant()
        {

            INPUT_PORTS_START("vigilant");
            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0xF0, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);

            PORT_START();
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x04, "Normal");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x08, 0x08, "Decrease of Energy");
            PORT_DIPSETTING(0x08, "Slow");
            PORT_DIPSETTING(0x00, "Fast");
            /* TODO: support the different settings which happen in Coin Mode 2 */
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR(Coinage));
            PORT_DIPSETTING(0xa0, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0xb0, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xd0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("8C_3C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("5C_3C"));
            PORT_DIPSETTING(0x30, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));

            PORT_START();
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x02, DEF_STR(Cocktail));
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x10, DEF_STR(Yes));
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Stop Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            return INPUT_PORTS_END;
        }
        public driver_vigilant()
        {
            drv = new machine_driver_vigilant();
            year = "1988";
            name = "vigilant";
            description = "Vigilante (World)";
            manufacturer = "Irem";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_vigilant();
            rom = rom_vigilant();
            drv.HasNVRAMhandler = false;
        }
    }
}
