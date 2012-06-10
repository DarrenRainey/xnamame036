using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_raiden : Mame.GameDriver
    {
        static Mame.tilemap background_layer, foreground_layer, text_layer;
        static _BytePtr raiden_back_data = new _BytePtr(1);
        static _BytePtr raiden_fore_data = new _BytePtr(1);
        static _BytePtr raiden_scroll_ram = new _BytePtr(1);
        static _BytePtr raiden_shared_ram = new _BytePtr(1);
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress(0x00000, 0x07fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(0x0a000, 0x0afff, raiden_shared_r ),
	new Mame.MemoryReadAddress(0x0b000, 0x0b000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress(0x0b001, 0x0b001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress(0x0b002, 0x0b002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress(0x0b003, 0x0b003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress(0x0d000, 0x0d00f, raiden_sound_r ),
	new Mame.MemoryReadAddress(0xa0000, 0xfffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x00000, 0x06fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x07000, 0x07fff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x0a000, 0x0afff, raiden_shared_w,raiden_shared_ram ),
	new Mame.MemoryWriteAddress( 0x0b000, 0x0b007, raiden_control_w ),
	new Mame.MemoryWriteAddress( 0x0c000, 0x0c7ff, raiden_text_w, Generic.videoram ),
	new Mame.MemoryWriteAddress( 0x0d000, 0x0d00f, Seibu.seibu_soundlatch_w, Seibu.seibu_shared_sound_ram ),
	new Mame.MemoryWriteAddress( 0x0d060, 0x0d067, Mame.MWA_RAM, raiden_scroll_ram ),
	new Mame.MemoryWriteAddress( 0xa0000, 0xfffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sub_readmem =
{
	new Mame.MemoryReadAddress( 0x00000, 0x01fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x02000, 0x027ff, raiden_background_r ),
	new Mame.MemoryReadAddress( 0x02800, 0x02fff, raiden_foreground_r ),
	new Mame.MemoryReadAddress( 0x03000, 0x03fff, Mame.paletteram_r ),
	new Mame.MemoryReadAddress( 0x04000, 0x04fff, raiden_shared_r ),
	new Mame.MemoryReadAddress( 0xc0000, 0xfffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sub_writemem =
{
	new Mame.MemoryWriteAddress( 0x00000, 0x01fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x02000, 0x027ff, raiden_background_w, raiden_back_data ),
	new Mame.MemoryWriteAddress( 0x02800, 0x02fff, raiden_foreground_w, raiden_fore_data ),
	new Mame.MemoryWriteAddress( 0x03000, 0x03fff, Mame.paletteram_xxxxBBBBGGGGRRRR_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x04000, 0x04fff, raiden_shared_w ),
	new Mame.MemoryWriteAddress( 0x07ffe, 0x0afff, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xc0000, 0xfffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        /************************* Alternate board set ************************/

        static Mame.MemoryReadAddress[] alt_readmem =
{
	new Mame.MemoryReadAddress( 0x00000, 0x07fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x08000, 0x08fff, raiden_shared_r ),
	new Mame.MemoryReadAddress( 0x0a000, 0x0a00f, raiden_sound_r ),
	new Mame.MemoryReadAddress( 0x0e000, 0x0e000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x0e001, 0x0e001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x0e002, 0x0e002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x0e003, 0x0e003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xa0000, 0xfffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] alt_writemem =
{
	new Mame.MemoryWriteAddress( 0x00000, 0x06fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x07000, 0x07fff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x08000, 0x08fff, raiden_shared_w, raiden_shared_ram ),
	new Mame.MemoryWriteAddress( 0x0a000, 0x0a00f, Seibu.seibu_soundlatch_w, Seibu.seibu_shared_sound_ram ),
	new Mame.MemoryWriteAddress( 0x0b000, 0x0b007, raiden_control_w ),
	new Mame.MemoryWriteAddress( 0x0c000, 0x0c7ff, raidena_text_w, Generic.videoram ),
	new Mame.MemoryWriteAddress( 0x0f000, 0x0f035, Mame.MWA_RAM, raiden_scroll_ram ),
	new Mame.MemoryWriteAddress( 0xa0000, 0xfffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =					
{																	
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),									
	new Mame.MemoryReadAddress( 0x2000, 0x27ff, Mame.MRA_RAM ),									
	new Mame.MemoryReadAddress( 0x4008, 0x4008, YM3812.YM3812_status_port_0_r ),						
	new Mame.MemoryReadAddress( 0x4010, 0x4012, Seibu.seibu_soundlatch_r ), 						
	new Mame.MemoryReadAddress( 0x4013, 0x4013, Mame.input_port_4_r ), 								
	new Mame.MemoryReadAddress( 0x6000, 0x6000, okim6295.OKIM6295_status_0_r ),						
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_BANK1 ),									
	new Mame.MemoryReadAddress( -1 )	/* end of table */										
};

        static Mame.MemoryWriteAddress[] sound_writemem =					
{																	
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),									
	new Mame.MemoryWriteAddress( 0x2000, 0x27ff, Mame.MWA_RAM ),									
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, Seibu.seibu_soundclear_w ),							
	new Mame.MemoryWriteAddress( 0x4002, 0x4002, Seibu.seibu_rst10_ack ), 							
	new Mame.MemoryWriteAddress( 0x4003, 0x4003, Seibu.seibu_rst18_ack ), 							
	new Mame.MemoryWriteAddress( 0x4007, 0x4007, Seibu.seibu_bank_w ),								
	new Mame.MemoryWriteAddress( 0x4008, 0x4008, YM3812.YM3812_control_port_0_w ),					
	new Mame.MemoryWriteAddress( 0x4009, 0x4009, YM3812.YM3812_write_port_0_w ),						
	new Mame.MemoryWriteAddress( 0x4018, 0x401f, Seibu.seibu_main_data_w ),							
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, okim6295.OKIM6295_data_0_w ),							
	new Mame.MemoryWriteAddress( -1 )	/* end of table */										
};

        static Mame.GfxLayout raiden_charlayout =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            2048,		/* 512 characters */
            4,			/* 4 bits per pixel */
            new uint[] { 4, 0, (0x08000 * 8) + 4, 0x08000 * 8 },
            new uint[] { 0, 1, 2, 3, 8, 9, 10, 11 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            128
        );

        static Mame.GfxLayout raiden_spritelayout =
        new Mame.GfxLayout(
          16, 16,	/* 16*16 tiles */
          4096,		/* 2048*4 tiles */
          4,		/* 4 bits per pixel */
          new uint[] { 12, 8, 4, 0 },
          new uint[]{
    0,1,2,3, 16,17,18,19,
	512+0,512+1,512+2,512+3,
	512+8+8,512+9+8,512+10+8,512+11+8,
  },
          new uint[]{
	0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
	8*32, 9*32, 10*32, 11*32, 12*32, 13*32, 14*32, 15*32,
  },
          1024
        );
        static int flipscreen, ALTERNATE;

        static Mame.GfxDecodeInfo[] raiden_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, raiden_charlayout,   768, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, raiden_spritelayout,   0, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, raiden_spritelayout, 256, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX4, 0, raiden_spritelayout, 512, 16 ),
};
        static void raiden_control_w(int offset, int data)
        {
            /* All other bits unknown - could be playfield enables */

            /* Flipscreen */
            if (offset == 6)
            {
                flipscreen = data & 0x2;
                Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
            }
        }
        static int raiden_background_r(int offset)
        {
            return raiden_back_data[offset];
        }
        static int raiden_foreground_r(int offset)
        {
            return raiden_fore_data[offset];
        }
        static void raiden_background_w(int offset, int data)
        {
            raiden_back_data[offset] = (byte)data;
            Mame.tilemap_mark_tile_dirty(background_layer, (offset / 2) / 32, (offset / 2) % 32);
        }
        static void raiden_foreground_w(int offset, int data)
        {
            raiden_fore_data[offset] = (byte)data;
            Mame.tilemap_mark_tile_dirty(foreground_layer, (offset / 2) / 32, (offset / 2) % 32);
        }
        static void raiden_text_w(int offset, int data)
        {
            Generic.videoram[offset] = (byte)data;
            Mame.tilemap_mark_tile_dirty(text_layer, (offset / 2) / 32, (offset / 2) % 32);
        }
        static void raidena_text_w(int offset, int data)
        {
            Generic.videoram[offset] = (byte)data;
            Mame.tilemap_mark_tile_dirty(text_layer, (offset / 2) % 32, (offset / 2) / 32);
        }
        static void get_back_tile_info(int col, int row)
        {
            int offs = (row * 2) + (col * 64);
            int tile = raiden_back_data[offs] + (raiden_back_data[offs + 1] << 8);
            int color = tile >> 12;

            tile = tile & 0xfff;

            Mame.SET_TILE_INFO(1, tile, color);
        }
        static void get_fore_tile_info(int col, int row)
        {
            int offs = (row * 2) + (col * 64);
            int tile = raiden_fore_data[offs] + (raiden_fore_data[offs + 1] << 8);
            int color = tile >> 12;

            tile = tile & 0xfff;

            Mame.SET_TILE_INFO(2, tile, color);
        }
        static void get_text_tile_info(int col, int row)
        {
            int offs = (row * 2) + (col * 64);
            int tile = Generic.videoram[offs] + ((Generic.videoram[offs + 1] & 0xc0) << 2);
            int color = Generic.videoram[offs + 1] & 0xf;

            Mame.SET_TILE_INFO(0, tile, color);
        }
        static void get_text_alt_tile_info(int col, int row)
        {
            int offs = (col * 2) + (row * 64);
            int tile = Generic.videoram[offs] + ((Generic.videoram[offs + 1] & 0xc0) << 2);
            int color = Generic.videoram[offs + 1] & 0xf;

            Mame.SET_TILE_INFO(0, tile, color);
        }
        static int raiden_shared_r(int offset) { return raiden_shared_ram[offset]; }
        static void raiden_shared_w(int offset, int data) { raiden_shared_ram[offset] = (byte)data; }

        static int latch = 0;
        static int raiden_sound_r(int offset)
        {
            int erg, orig, coin = Mame.readinputport(4);
            orig = Seibu.seibu_shared_sound_ram[offset];

            /* Small kludge to allows coins with sound off */
            if (coin == 0) latch = 0;
            if (offset == 4 && (Mame.Machine.sample_rate == 0) && coin != 0 && latch == 0)
            {
                latch = 1;
                return coin;
            }

            switch (offset)
            {/* misusing $d006 as a latch...but it works !*/
                case 0x04: { erg = Seibu.seibu_shared_sound_ram[6]; Seibu.seibu_shared_sound_ram[6] = 0; break; } /* just 1 time */
                case 0x06: { erg = 0xa0; break; }
                case 0x0a: { erg = 0; break; }
                default: erg = Seibu.seibu_shared_sound_ram[offset]; break;
            }
            return erg;
        }
        static int raiden_interrupt()
        {
            return 0xc8 / 4;	/* VBL */
        }
        static YM3812interface ym3812_interface = new YM3812interface(1, 4318180 / 4, new int[] { 50 }, new handlerdelegate[] { Seibu.seibu_ym3812_irqhandler });
        static OKIM6295interface okim6295_interface = new OKIM6295interface(1, new int[] { 8000 }, new int[] { Mame.REGION_SOUND1 }, new int[] { 40 });

        class machine_driver_raiden : Mame.MachineDriver
        {
            public machine_driver_raiden()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_V30, 19000000, readmem, writemem, null, null, raiden_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_V30, 19000000, sub_readmem, sub_writemem, null, null, raiden_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 4, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION * 2;
                cpu_slices_per_frame = 70;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = raiden_gfxdecodeinfo;
                total_colors = 2048;
                color_table_len = 2048;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_BUFFERS_SPRITERAM;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, ym3812_interface));
                //sound.Add(new Mame.MachineSound(Mame.SOUND_OKIM6295,okim6295_interface));
            }
            public override void init_machine()
            {
                Seibu.seibu_sound_init_2();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                background_layer = Mame.tilemap_create(
        get_back_tile_info,
        0,
        16, 16,
        32, 32
    );

                foreground_layer = Mame.tilemap_create(
                    get_fore_tile_info,
                    Mame.TILEMAP_TRANSPARENT,
                    16, 16,
                    32, 32
                );

                /* Weird - Raiden (Alternate) has different char format! */
                if (Mame.Machine.gamedrv.name.CompareTo("raiden") == 0)
                    ALTERNATE = 0;
                else
                    ALTERNATE = 1;

                /* Weird - Raiden (Alternate) has different char format! */
                if (ALTERNATE == 0)
                    text_layer = Mame.tilemap_create(
                        get_text_tile_info,
                        Mame.TILEMAP_TRANSPARENT,
                        8, 8,
                        32, 32
                    );
                else
                    text_layer = Mame.tilemap_create(
                        get_text_alt_tile_info,
                        Mame.TILEMAP_TRANSPARENT,
                        8, 8,
                        32, 32
                    );

                Mame.tilemap_set_scroll_rows(background_layer, 1);
                Mame.tilemap_set_scroll_cols(background_layer, 1);
                Mame.tilemap_set_scroll_rows(foreground_layer, 1);
                Mame.tilemap_set_scroll_cols(foreground_layer, 1);
                Mame.tilemap_set_scroll_rows(text_layer, 0);
                Mame.tilemap_set_scroll_cols(text_layer, 0);

                foreground_layer.transparent_pen = 15;
                text_layer.transparent_pen = 15;

                return 0;
            }
            public override void vh_stop()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int color, offs, sprite;
                int[] colmask = new int[16];
                int i, pal_base;

                /* Setup the tilemaps, alternate version has different scroll positions */
                if (ALTERNATE == 0)
                {
                    Mame.tilemap_set_scrollx(background_layer, 0, ((raiden_scroll_ram[1] << 8) + raiden_scroll_ram[0]));
                    Mame.tilemap_set_scrolly(background_layer, 0, ((raiden_scroll_ram[3] << 8) + raiden_scroll_ram[2]));
                    Mame.tilemap_set_scrollx(foreground_layer, 0, ((raiden_scroll_ram[5] << 8) + raiden_scroll_ram[4]));
                    Mame.tilemap_set_scrolly(foreground_layer, 0, ((raiden_scroll_ram[7] << 8) + raiden_scroll_ram[6]));
                }
                else
                {
                    Mame.tilemap_set_scrolly(background_layer, 0, ((raiden_scroll_ram[0x02] & 0x30) << 4) + ((raiden_scroll_ram[0x04] & 0x7f) << 1) + ((raiden_scroll_ram[0x04] & 0x80) >> 7));
                    Mame.tilemap_set_scrollx(background_layer, 0, ((raiden_scroll_ram[0x12] & 0x30) << 4) + ((raiden_scroll_ram[0x14] & 0x7f) << 1) + ((raiden_scroll_ram[0x14] & 0x80) >> 7));
                    Mame.tilemap_set_scrolly(foreground_layer, 0, ((raiden_scroll_ram[0x22] & 0x30) << 4) + ((raiden_scroll_ram[0x24] & 0x7f) << 1) + ((raiden_scroll_ram[0x24] & 0x80) >> 7));
                    Mame.tilemap_set_scrollx(foreground_layer, 0, ((raiden_scroll_ram[0x32] & 0x30) << 4) + ((raiden_scroll_ram[0x34] & 0x7f) << 1) + ((raiden_scroll_ram[0x34] & 0x80) >> 7));
                }

                Mame.tilemap_update(Mame.ALL_TILEMAPS);

                /* Build the dynamic palette */
                Mame.palette_init_used_colors();

                /* Sprites */
                pal_base = Mame.Machine.drv.gfxdecodeinfo[3].color_codes_start;
                for (color = 0; color < 16; color++) colmask[color] = 0;
                for (offs = 0; offs < 0x1000; offs += 8)
                {
                    color = Generic.buffered_spriteram[offs + 1] & 0xf;
                    sprite = Generic.buffered_spriteram[offs + 2] + (Generic.buffered_spriteram[offs + 3] << 8);
                    sprite &= 0x0fff;
                    colmask[color] |= (int)Mame.Machine.gfx[3].pen_usage[sprite];
                }
                for (color = 0; color < 16; color++)
                {
                    for (i = 0; i < 15; i++)
                    {
                        if ((colmask[color] & (1 << i)) != 0)
                            Mame.palette_used_colors[pal_base + 16 * color + i] = Mame.PALETTE_COLOR_USED;
                    }
                }

                if (Mame.palette_recalc() != null)
                    Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);
                Mame.tilemap_draw(bitmap, background_layer, 0);

                /* Draw sprites underneath foreground */
                draw_sprites(bitmap, 0x40);
                Mame.tilemap_draw(bitmap, foreground_layer, 0);

                /* Rest of sprites */
                draw_sprites(bitmap, 0x80);

                /* Text layer */
                Mame.tilemap_draw(bitmap, text_layer, 0);
            }
            static void draw_sprites(Mame.osd_bitmap bitmap, int pri_mask)
            {
                int offs, fx, fy, x, y, color, sprite;

                for (offs = 0x1000 - 8; offs >= 0; offs -= 8)
                {
                    /* Don't draw empty sprite table entries */
                    if (Generic.buffered_spriteram[offs + 7] != 0xf) continue;
                    if (Generic.buffered_spriteram[offs + 0] == 0x0f) continue;
                    if ((pri_mask & Generic.buffered_spriteram[offs + 5]) == 0) continue;

                    fx = Generic.buffered_spriteram[offs + 1] & 0x20;
                    fy = Generic.buffered_spriteram[offs + 1] & 0x40;
                    y = Generic.buffered_spriteram[offs + 0];
                    x = Generic.buffered_spriteram[offs + 4];

                    if ((Generic.buffered_spriteram[offs + 5] & 1) != 0) x = 0 - (0x100 - x);

                    color = Generic.buffered_spriteram[offs + 1] & 0xf;
                    sprite = Generic.buffered_spriteram[offs + 2] + (Generic.buffered_spriteram[offs + 3] << 8);
                    sprite &= 0x0fff;

                    if (flipscreen != 0)
                    {
                        x = 240 - x;
                        y = 240 - y;
                        if (fx != 0) fx = 0; else fx = 1;
                        if (fy != 0) fy = 0; else fy = 1;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                            (uint)sprite,
                            (uint)color, fx != 0, fy != 0, x, y,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);
                }
            }
            public override void vh_eof_callback()
            {
                Generic.buffer_spriteram_w(0, 0); /* Could be a memory location instead */
            }
        }

        /* Spin the sub-cpu if it is waiting on the master cpu */
        static int sub_cpu_spin(int offset)
        {
            uint pc = Mame.cpu_get_pc();
            int ret = raiden_shared_ram[0x8];

            if (offset == 1) return raiden_shared_ram[0x9];

            if (pc == 0xfcde6 && ret != 0x40)
                Mame.cpu_spin();

            return ret;
        }

        static int sub_cpu_spina(int offset)
        {
            uint pc = Mame.cpu_get_pc();
            int ret = raiden_shared_ram[0x8];

            if (offset == 1) return raiden_shared_ram[0x9];

            if (pc == 0xfcde8 && ret != 0x40)
                Mame.cpu_spin();

            return ret;
        }
        public override void driver_init()
        {
            Mame.install_mem_read_handler(1, 0x4008, 0x4009, sub_cpu_spin);
            Seibu.install_seibu_sound_speedup(2);
        }
        Mame.RomModule[] rom_raiden()
        {

            ROM_START("raiden");
            ROM_REGION(0x100000, Mame.REGION_CPU1); /* v30 main cpu */
            ROM_LOAD_V20_ODD("rai1.bin", 0x0a0000, 0x10000, 0xa4b12785);
            ROM_LOAD_V20_EVEN("rai2.bin", 0x0a0000, 0x10000, 0x17640bd5);
            ROM_LOAD_V20_ODD("rai3.bin", 0x0c0000, 0x20000, 0x9d735bf5);
            ROM_LOAD_V20_EVEN("rai4.bin", 0x0c0000, 0x20000, 0x8d184b99);

            ROM_REGION(0x100000, Mame.REGION_CPU2); /* v30 sub cpu */
            ROM_LOAD_V20_ODD("rai5.bin", 0x0c0000, 0x20000, 0x7aca6d61);
            ROM_LOAD_V20_EVEN("rai6a.bin", 0x0c0000, 0x20000, 0xe3d35cc2);

            ROM_REGION(0x18000, Mame.REGION_CPU3); /* 64k code for sound Z80 */
            ROM_LOAD("rai6.bin", 0x000000, 0x08000, 0x723a483b);
            ROM_CONTINUE(0x010000, 0x08000);

            ROM_REGION(0x010000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("rai9.bin", 0x00000, 0x08000, 0x1922b25e); /* chars */
            ROM_LOAD("rai10.bin", 0x08000, 0x08000, 0x5f90786a);

            ROM_REGION(0x080000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("raiu0919.bin", 0x00000, 0x80000, 0xda151f0b); /* tiles */

            ROM_REGION(0x080000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("raiu0920.bin", 0x00000, 0x80000, 0xac1f57ac); /* tiles */

            ROM_REGION(0x090000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("raiu165.bin", 0x00000, 0x80000, 0x946d7bde); /* sprites */

            ROM_REGION(0x10000, Mame.REGION_SOUND1);	 /* ADPCM samples */
            ROM_LOAD("rai7.bin", 0x00000, 0x10000, 0x8f927822);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_raiden()
        {

            INPUT_PORTS_START("raiden");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* Dip switch A */
            PORT_DIPNAME(0x01, 0x01, "Coin Mode");
            PORT_DIPSETTING(0x01, "A");
            PORT_DIPSETTING(0x00, "B");
            /* Coin Mode A */
            PORT_DIPNAME(0x1e, 0x1e, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x14, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0x16, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x18, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x1a, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("8C_3C"));
            PORT_DIPSETTING(0x1c, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("5C_3C"));
            PORT_DIPSETTING(0x06, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x1e, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x12, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x0a, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));

            /* Coin Mode B */
            /*	PORT_DIPNAME( 0x06, 0x06, DEF_STR( Coin_A ) )
                PORT_DIPSETTING(    0x00, "5C/1C or Free if Coin B too" )
                PORT_DIPSETTING(    0x02, DEF_STR( 3C_1C ) )
                PORT_DIPSETTING(    0x04, DEF_STR( 2C_1C ) )
                PORT_DIPSETTING(    0x06, DEF_STR( 1C_1C ) )
                PORT_DIPNAME( 0x18, 0x18, DEF_STR( Coin_B ) )
                PORT_DIPSETTING(    0x18, DEF_STR( 1C_2C ) )
                PORT_DIPSETTING(    0x10, DEF_STR( 1C_3C ) )
                PORT_DIPSETTING(    0x08, DEF_STR( 1C_5C ) )
                PORT_DIPSETTING(    0x00, "1C/6C or Free if Coin A too" ) */

            PORT_DIPNAME(0x20, 0x20, "Credits to Start");
            PORT_DIPSETTING(0x20, "1");
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Unused"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START();	/* Dip switch B */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x02, "1");
            PORT_DIPSETTING(0x01, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x08, "80000 300000");
            PORT_DIPSETTING(0x0c, "150000 400000");
            PORT_DIPSETTING(0x04, "300000 1000000");
            PORT_DIPSETTING(0x00, "1000000 5000000");
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x20, "Easy");
            PORT_DIPSETTING(0x30, "Normal");
            PORT_DIPSETTING(0x10, "Hard");
            PORT_DIPSETTING(0x00, "Very Hard");
            PORT_DIPNAME(0x40, 0x40, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x40, DEF_STR("Yes"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START();	/* Coins */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            return INPUT_PORTS_END;
        }
        public driver_raiden()
        {
            drv = new machine_driver_raiden();
            year = "1990";
            name = "raiden";
            description = "Raiden";
            manufacturer = "Seibu Kaihatsu";
            flags = Mame.ROT270;
            input_ports = input_ports_raiden();
            rom = rom_raiden();
            drv.HasNVRAMhandler = false;
        }
    }
}
