using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_skykid : Mame.GameDriver
    {
        static int irq_disabled = 1;
        static int inputport_selected;
        static _BytePtr sharedram = new _BytePtr(1);
        static _BytePtr skykid_textram = new _BytePtr(1);
        static _BytePtr spriteram = new _BytePtr(1);
        static _BytePtr drgnbstr_videoram = new _BytePtr(1);
        static Mame.tilemap background;
        static int game;

        static Mame.MemoryReadAddress[] skykid_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_BANK1 ),				/* banked ROM */
	new Mame.MemoryReadAddress( 0x2000, 0x2fff, skykid_videoram_r ),		/* Video RAM (background) */
	new Mame.MemoryReadAddress( 0x4000, 0x47ff,Mame. MRA_RAM ),				/* video RAM (text layer) */
	new Mame.MemoryReadAddress( 0x4800, 0x5fff, Mame.MRA_RAM ),				/* RAM + Sprite RAM */
	new Mame.MemoryReadAddress( 0x6800, 0x68ff, Namco.namcos1_wavedata_r ),		/* PSG device, shared RAM */
	new Mame.MemoryReadAddress( 0x6800, 0x6bff, skykid_sharedram_r ),		/* shared RAM with the MCU */
	new Mame.MemoryReadAddress( 0x7800, 0x7800, Mame.watchdog_reset_r ),		/* watchdog reset */
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),				/* ROM */
	new Mame.MemoryReadAddress( -1 )
};

        static Mame.MemoryWriteAddress[] skykid_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),				/* banked ROM */
	new Mame.MemoryWriteAddress( 0x2000, 0x2fff, skykid_videoram_w, drgnbstr_videoram ),/* Video RAM (background) */
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM, skykid_textram ),	/* video RAM (text layer) */
	new Mame.MemoryWriteAddress( 0x4800, 0x5fff, Mame.MWA_RAM ),				/* RAM + Sprite RAM */
	new Mame.MemoryWriteAddress( 0x6000, 0x60ff, skykid_scroll_y_w ),		/* Y scroll register map */
	new Mame.MemoryWriteAddress( 0x6200, 0x63ff, skykid_scroll_x_w ),		/* X scroll register map */
	new Mame.MemoryWriteAddress( 0x6800, 0x68ff, Namco.namcos1_wavedata_w, Namco.namco_wavedata ),/* PSG device, shared RAM */
	new Mame.MemoryWriteAddress( 0x6800, 0x6bff, skykid_sharedram_w, sharedram ),	/* shared RAM with the MCU */
	new Mame.MemoryWriteAddress( 0x7000, 0x7800, skykid_irq_ctrl_w ),		/* IRQ control */
	new Mame.MemoryWriteAddress( 0x8000, 0x8800, skykid_halt_mcu_w ),		/* MCU control */
	new Mame.MemoryWriteAddress( 0x9000, 0x9800, skykid_bankswitch_w ),	/* Bankswitch control */
	new Mame.MemoryWriteAddress( 0xa000, 0xa001, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),				/* ROM */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.MemoryReadAddress[] mcu_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_r ),/* internal registers */
	new Mame.MemoryReadAddress( 0x0080, 0x00ff, Mame.MRA_RAM ),					/* built in RAM */
	new Mame.MemoryReadAddress( 0x1000, 0x10ff, Namco.namcos1_wavedata_r ),			/* PSG device, shared RAM */
	new Mame.MemoryReadAddress( 0x1100, 0x113f, Mame.MRA_RAM ),					/* PSG device */
	new Mame.MemoryReadAddress( 0x1000, 0x13ff, skykid_sharedram_r ),			/* shared RAM with the 6809 */
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_ROM ),					/* MCU external ROM */
	new Mame.MemoryReadAddress( 0xc000, 0xc800, Mame.MRA_RAM ),					/* RAM */
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),					/* MCU internal ROM */
	new Mame.MemoryReadAddress( -1 )
};

        static Mame.MemoryWriteAddress[] mcu_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x001f, Mame.cpu_hd63701.hd63701_internal_registers_w ),/* internal registers */
	new Mame.MemoryWriteAddress( 0x0080, 0x00ff, Mame.MWA_RAM ),					/* built in RAM */
	new Mame.MemoryWriteAddress( 0x1000, 0x10ff, Namco.namcos1_wavedata_w ),			/* PSG device, shared RAM */
	new Mame.MemoryWriteAddress( 0x1100, 0x113f, Namco.namcos1_sound_w, Namco.namco_soundregs ),/* PSG device */
	new Mame.MemoryWriteAddress( 0x1000, 0x13ff, skykid_sharedram_w ),			/* shared RAM with the 6809 */
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, Mame.MWA_NOP ),					/* ??? */
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, Mame.MWA_NOP ),					/* ??? */
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, Mame.MWA_NOP ),					/* ??? */
	new Mame.MemoryWriteAddress( 0x8000, 0xbfff, Mame.MWA_ROM ),					/* MCU external ROM */
	new Mame.MemoryWriteAddress( 0xc000, 0xc800, Mame.MWA_RAM ),					/* RAM */
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_ROM ),					/* MCU internal ROM */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.IOReadPort[] mcu_readport =
{
	new Mame.IOReadPort( Mame.cpu_hd63701.HD63701_PORT1, Mame.cpu_hd63701.HD63701_PORT1, inputport_r ),			/* input ports read */
    new Mame.IOReadPort(-1)
};

        static Mame.IOWritePort[] mcu_writeport =
{
	new Mame.IOWritePort( Mame.cpu_hd63701.HD63701_PORT1, Mame.cpu_hd63701.HD63701_PORT1, inputport_select_w ),	/* input port select */
	new Mame.IOWritePort( Mame.cpu_hd63701.HD63701_PORT2, Mame.cpu_hd63701.HD63701_PORT2, skykid_lamps_w ),		/* lamps */
    new Mame.IOWritePort(-1)
};

        static Mame.GfxLayout text_layout =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            2,			/* 2 bits per pixel */
            new uint[] { 0, 4 },	/* the bitplanes are packed in the same byte */
            new uint[] { 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout tile_layout =
        new Mame.GfxLayout(
            8, 8,		/* 8*8 characters */
            512,		/* 512 characters */
            2,			/* 2 bits per pixel */
            new uint[] { 0, 4 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 8, 2 * 8, 4 * 8, 6 * 8, 8 * 8, 10 * 8, 12 * 8, 14 * 8 },
            16 * 8		/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout sprite_layout1 =
        new Mame.GfxLayout(
            16, 16,       	/* 16*16 sprites */
            128,           	/* 128 sprites */
            3,              /* 3 bits per pixel */
            new uint[] { 0x4000 * 8 + 4, 0, 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8    /* every sprite takes 64 bytes */
        );

        static Mame.GfxLayout sprite_layout2 =
        new Mame.GfxLayout(
            16, 16,       	/* 16*16 sprites */
            128,           	/* 128 sprites */
            3,              /* 3 bits per pixel */
            new uint[] { 0x4000 * 8, 0x2000 * 8, 0x2000 * 8 + 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8    /* every sprite takes 64 bytes */
        );

        static Mame.GfxLayout sprite_layout3 =
        new Mame.GfxLayout(
            16, 16,       	/* 16*16 sprites */
            128,           	/* 128 sprites */
            3,              /* 3 bits per pixel */
            new uint[] { 0x8000 * 8, 0x6000 * 8, 0x6000 * 8 + 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8    /* every sprite takes 64 bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, text_layout,		0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tile_layout,		64*4, 128 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, sprite_layout1,	64*4+128*4, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, sprite_layout2,	64*4+128*4, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, sprite_layout3,	64*4+128*4, 64 ),
};


        static Namco_interface namco_interface =
            new Namco_interface(
            49152000 / 2048, 		/* 24000 Hz */
            8,					/* number of voices */
            100,				/* playback volume */
            -1,					/* memory region */
            false					/* stereo */
        );

        static int skykid_videoram_r(int offset)
        {
            return drgnbstr_videoram[offset];
        }
        static void skykid_videoram_w(int offset, int data)
        {
            if (drgnbstr_videoram[offset] != data)
            {
                drgnbstr_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(background, (offset & 0x7ff) % 64, (offset & 0x7ff) / 64);
            }
        }
        static int skykid_interrupt()
        {
            if (irq_disabled == 0)
                return Mame.cpu_m6809.M6809_INT_IRQ;
            else
                return Mame.ignore_interrupt();
        }
        static void skykid_irq_ctrl_w(int offset, int data)
        {
            irq_disabled = offset;
        }
        static void inputport_select_w(int offset, int data)
        {
            if ((data & 0xf0) == 0x60)
                inputport_selected = data & 0x07;
        }
        static int reverse_bitstrm(int data) { return ((data & 0x01) << 4) | ((data & 0x02) << 2) | (data & 0x04) | ((data & 0x08) >> 2) | ((data & 0x10) >> 4); }
        static int inputport_r(int offset)
        {
            int data = 0;

            switch (inputport_selected)
            {
                case 0x00:	/* DSW B (bits 0-4) */
                    data = ~(reverse_bitstrm(Mame.readinputport(1) & 0x1f)); break;
                case 0x01:	/* DSW B (bits 5-7), DSW A (bits 0-1) */
                    data = ~(reverse_bitstrm((((Mame.readinputport(1) & 0xe0) >> 5) | ((Mame.readinputport(0) & 0x03) << 3)))); break;
                case 0x02:	/* DSW A (bits 2-6) */
                    data = ~(reverse_bitstrm(((Mame.readinputport(0) & 0x7c) >> 2))); break;
                case 0x03:	/* DSW A (bit 7), DSW C (bits 0-3) */
                    data = ~(reverse_bitstrm((((Mame.readinputport(0) & 0x80) >> 7) | ((Mame.readinputport(2) & 0x0f) << 1)))); break;
                case 0x04:	/* coins, start */
                    data = ~(Mame.readinputport(3)); break;
                case 0x05:	/* 2P controls */
                    data = ~(Mame.readinputport(5)); break;
                case 0x06:	/* 1P controls */
                    data = ~(Mame.readinputport(4)); break;
                default:
                    data = 0xff;
                    break;
            }

            return data;
        }
        static void skykid_lamps_w(int offset, int data)
        {
            Mame.osd_led_w(0, (data & 0x08) >> 3);
            Mame.osd_led_w(1, (data & 0x10) >> 4);
        }
        static void skykid_halt_mcu_w(int offset, int data)
        {
            if (offset == 0)
            {
                Mame.cpu_set_reset_line(1, Mame.PULSE_LINE);
                Mame.cpu_set_halt_line(1, Mame.CLEAR_LINE);
            }
            else
            {
                Mame.cpu_set_halt_line(1, Mame.ASSERT_LINE);
            }
        }
        static int skykid_sharedram_r(int offset)
        {
            return sharedram[offset];
        }
        static void skykid_sharedram_w(int offset, int val)
        {
            sharedram[offset] = (byte)val;
        }
        static void get_tile_info_bg(int col, int row)
        {
            int tile_index = row * 64 + col;
            byte code = drgnbstr_videoram[tile_index];
            byte attr = drgnbstr_videoram[tile_index + 0x800];

            Mame.SET_TILE_INFO(1, code + 256 * (attr & 0x01), ((attr & 0x7e) >> 1) | ((attr & 0x01) << 6));
        }
        static void skykid_scroll_x_w(int offset, int data)
        {
            if (game != 0)
                Mame.tilemap_set_scrollx(background, 0, ((offset ^ 1) + 36) & 0x1ff);
            else
                Mame.tilemap_set_scrollx(background, 0, (189 - (offset ^ 1)) & 0x1ff);
        }
        static void skykid_scroll_y_w(int offset, int data)
        {
            if (game != 0)
                Mame.tilemap_set_scrolly(background, 0, (offset + 25) & 0xff);
            else
                Mame.tilemap_set_scrolly(background, 0, (261 - offset) & 0xff);
        }
        static int skykid_drgnbstr_common_vh_init()
        {
            background = Mame.tilemap_create(get_tile_info_bg, Mame.TILEMAP_OPAQUE, 8, 8, 64, 32);

            if (background != null)
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

                spriteram = new _BytePtr(RAM, 0x4f80);
                Generic.spriteram_2 = new _BytePtr(RAM, 0x4f80 + 0x0800);
                Generic.spriteram_3 = new _BytePtr(RAM, 0x4f80 + 0x0800 + 0x0800);
                Generic.spriteram_size[0] = 0x80;

                return 0;
            }
            return 1;
        }
        static void skykid_bankswitch_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            bankaddress = 0x10000 + (offset != 0 ? 0 : 0x2000);
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }
        class machine_driver_skykid : Mame.MachineDriver
        {
            public machine_driver_skykid()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 49152000 / 32, skykid_readmem, skykid_writemem, null, null, skykid_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD63701, 49152000 / 32, mcu_readmem, mcu_writemem, mcu_readport, mcu_writeport, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_skykid.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 128 * 4 + 64 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
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
                uint totcolors = Mame.Machine.drv.total_colors;
                uint pi = 0, cpi = 0;
                for (int i = 0; i < totcolors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi + totcolors * 0] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi + totcolors * 0] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi + totcolors * 0] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi + totcolors * 0] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    /* green component */
                    bit0 = (color_prom[cpi + totcolors * 1] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + totcolors * 1] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + totcolors * 1] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + totcolors * 1] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    /* blue component */
                    bit0 = (color_prom[cpi + totcolors * 2] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + totcolors * 2] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + totcolors * 2] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + totcolors * 2] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                /* text palette */
                int idx = 0;
                for (int i = 0; i < 64 * 4; i++)
                    colortable[idx++] = (ushort)i;

                cpi += 2 * totcolors;
                /* color_prom now points to the beginning of the lookup table */

                /* tiles lookup table */
                for (int i = 0; i < 128 * 4; i++)
                    colortable[idx++] = color_prom[cpi++];

                /* sprites lookup table */
                for (int i = 0; i < 64 * 8; i++)
                    colortable[idx++] = color_prom[cpi++];

            }
            public override int vh_start()
            {
                game = 0;
                return skykid_drgnbstr_common_vh_init();
            }
            public override void vh_stop()
            {
                //nothing
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            static void skykid_draw_sprites(Mame.osd_bitmap bitmap)
            {
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    int number = spriteram[offs] | ((Generic.spriteram_3[offs] & 0x80) << 1);
                    int color = (spriteram[offs + 1] & 0x3f);
                    int sx = 256 - ((Generic.spriteram_2[offs + 1]) + 0x100 * (Generic.spriteram_3[offs + 1] & 0x01)) + 72;
                    int sy = Generic.spriteram_2[offs] - 7;
                    int flipy = Generic.spriteram_3[offs] & 0x02;
                    int flipx = Generic.spriteram_3[offs] & 0x01;
                    int width, height;

                    if (number >= 128 * 3) continue;

                    switch (Generic.spriteram_3[offs] & 0x0c)
                    {
                        case 0x0c:	/* 2x both ways */
                            width = height = 2; number &= (~3); break;
                        case 0x08:	/* 2x vertical */
                            width = 1; height = 2; number &= (~2); break;
                        case 0x04:	/* 2x horizontal */
                            width = 2; height = 1; number &= (~1); break;
                        default:	/* normal sprite */
                            width = height = 1; sx += 16; break;
                    }

                    {
                        int[] x_offset = { 0x00, 0x01 };
                        int[] y_offset = { 0x00, 0x02 };
                        int x, y, ex, ey;

                        for (y = 0; y < height; y++)
                        {
                            for (x = 0; x < width; x++)
                            {
                                ex = flipx != 0 ? (width - 1 - x) : x;
                                ey = flipy != 0 ? (height - 1 - y) : y;

                                Mame.drawgfx(bitmap, Mame.Machine.gfx[2 + (number >> 7)],
                                    (uint)((number) + x_offset[ex] + y_offset[ey]),
                                    (uint)color,
                                    flipx != 0, flipy != 0,
                                    sx + x * 16, sy + y * 16,
                                    Mame.Machine.drv.visible_area,
                                    Mame.TRANSPARENCY_COLOR, 255);
                            }
                        }
                    }
                }
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.tilemap_update(Mame.ALL_TILEMAPS);

                if (Mame.palette_recalc() != null)
                    Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);

                Mame.tilemap_draw(bitmap, background, 0);
                skykid_draw_sprites(bitmap);

                for (int offs = 0x400 - 1; offs > 0; offs--)
                {
                    {
                        int mx, my, sx, sy;

                        mx = offs % 32;
                        my = offs / 32;

                        if (my < 2)
                        {
                            if (mx < 2 || mx >= 30) continue; /* not visible */
                            sx = my + 34; sy = mx - 2;
                        }
                        else if (my >= 30)
                        {
                            if (mx < 2 || mx >= 30) continue; /* not visible */
                            sx = my - 30; sy = mx - 2;
                        }
                        else
                        {
                            sx = mx + 2; sy = my - 2;
                        }
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0], skykid_textram[offs],
                                (uint)(skykid_textram[offs + 0x400] & 0x3f),
                                false, false, sx * 8, sy * 8,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_skykid()
        {
            ROM_START("skykid");
            ROM_REGION(0x14000, Mame.REGION_CPU1);/* 6809 code */
            ROM_LOAD("sk2-6c.bin", 0x08000, 0x4000, 0xea8a5822);
            ROM_LOAD("sk1-6b.bin", 0x0c000, 0x4000, 0x7abe6c6c);
            ROM_LOAD("sk3-6d.bin", 0x10000, 0x4000, 0x314b8765);	/* banked ROM */

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* MCU code */
            ROM_LOAD("sk4-3c.bin", 0x8000, 0x2000, 0xa460d0e0);/* subprogram for the MCU */
            ROM_LOAD("sk1-mcu.bin", 0xf000, 0x1000, 0x6ef08fb3);	/* MCU internal code */
            /* Using Pacland code (probably similar) */

            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sk6-6l.bin", 0x00000, 0x2000, 0x58b731b9);/* chars */

            ROM_REGION(0x02000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sk5-7e.bin", 0x00000, 0x2000, 0xc33a498e);

            ROM_REGION(0x0a000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("sk9-10n.bin", 0x00000, 0x4000, 0x44bb7375);/* sprites */
            ROM_LOAD("sk7-10m.bin", 0x04000, 0x4000, 0x3454671d);
            /* empty space to decode the sprites as 3bpp */

            ROM_REGION(0x0700, Mame.REGION_PROMS);
            ROM_LOAD("sk1-2n.bin", 0x0000, 0x0100, 0x0218e726);	/* red component */
            ROM_LOAD("sk2-2p.bin", 0x0100, 0x0100, 0xfc0d5b85);	/* green component */
            ROM_LOAD("sk3-2r.bin", 0x0200, 0x0100, 0xd06b620b);	/* blue component */
            ROM_LOAD("sk-5n.bin", 0x0300, 0x0200, 0xc697ac72);	/* tiles lookup table */
            ROM_LOAD("sk-6n.bin", 0x0500, 0x0200, 0x161514a4);	/* sprites lookup table */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_skykid()
        {

            INPUT_PORTS_START("skykid");
            PORT_START();	/* DSW A */
            PORT_SERVICE(0x01, IP_ACTIVE_HIGH);
            PORT_DIPNAME(0x06, 0x00, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x06, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x00, "Round Skip");
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x10, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x00, "Freeze screen");
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x20, DEF_STR(On));
            PORT_DIPNAME(0xc0, 0x00, DEF_STR(Coin_B));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));

            PORT_START();	/* DSW B */
            PORT_DIPNAME(0x03, 0x00, DEF_STR(Lives));
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "2");
            PORT_DIPSETTING(0x02, "1");
            PORT_DIPSETTING(0x03, "5");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "30k, 90k");
            PORT_DIPSETTING(0x04, "20k, 80k");
            PORT_DIPSETTING(0x08, "30k every 90k");
            PORT_DIPSETTING(0x0c, "20k every 80k");
            PORT_DIPNAME(0x10, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x10, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x20, DEF_STR(On));
            PORT_DIPNAME(0x40, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x40, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x80, DEF_STR(Cocktail));

            PORT_START();	/* DSW C */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_DIPNAME(0x04, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN 0 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START3);	/* service */
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN 1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN 2 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        public driver_skykid()
        {
            drv = new machine_driver_skykid();
            year = "1985";
            name = "skykid";
            description = "Sky Kid";
            manufacturer = "Namco";
            flags = Mame.ROT0;
            input_ports = input_ports_skykid();
            rom = rom_skykid();
            drv.HasNVRAMhandler = false;
        }
    }
}
