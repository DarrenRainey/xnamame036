using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_gberet : Mame.GameDriver
    {
        public static _BytePtr gberet_videoram = new _BytePtr();
        public static _BytePtr gberet_colorram = new _BytePtr();
        public static _BytePtr gberet_spritebank = new _BytePtr();
        public static _BytePtr gberet_scrollram = new _BytePtr();
        static Mame.tilemap bg_tilemap;
        static int interruptenable;
        static int flipscreen;
        static int sprites_type;

        static void gberet_coincounter_w(int offset, int data)
        {
            /* bits 0/1 = coin counters */
            Mame.coin_counter_w(0, data & 1);
            Mame.coin_counter_w(1, data & 2);
        }
        Mame.InputPortTiny[] input_ports_gberet()
        {
            INPUT_PORTS_START("gberet");
            PORT_START("IN0");      /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);

            PORT_START("IN1");      /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);

            PORT_START("IN2");  /* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);

            PORT_START("DSW0"); /* DSW0 */
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x05, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x04, ipdn_defaultstrings["3C_2C"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["4C_3C"]);
            PORT_DIPSETTING(0x0f, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["3C_4C"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0x0e, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x06, ipdn_defaultstrings["2C_5C"]);
            PORT_DIPSETTING(0x0d, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x0c, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x0b, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0x0a, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPSETTING(0x09, ipdn_defaultstrings["1C_7C"]);
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x20, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x50, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x40, ipdn_defaultstrings["3C_2C"]);
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["4C_3C"]);
            PORT_DIPSETTING(0xf0, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x30, ipdn_defaultstrings["3C_4C"]);
            PORT_DIPSETTING(0x70, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0xe0, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x60, ipdn_defaultstrings["2C_5C"]);
            PORT_DIPSETTING(0xd0, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0xc0, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0xb0, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0xa0, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPSETTING(0x90, ipdn_defaultstrings["1C_7C"]);
            /* 0x00 is invalid */

            PORT_START("DSW1"); /* DSW1 */
            PORT_DIPNAME(0x03, 0x02, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x02, "3");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright)); ;
            PORT_DIPSETTING(0x04, DEF_STR(Cocktail));
            PORT_DIPNAME(0x18, 0x18, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x18, "30000 70000");
            PORT_DIPSETTING(0x10, "40000 80000");
            PORT_DIPSETTING(0x08, "50000 100000");
            PORT_DIPSETTING(0x00, "50000 200000");
            PORT_DIPNAME(0x60, 0x60, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x60, "Easy");
            PORT_DIPSETTING(0x40, "Medium");
            PORT_DIPSETTING(0x20, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x80, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START("DSW2"); /* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x02, "Controls");
            PORT_DIPSETTING(0x02, "Single");
            PORT_DIPSETTING(0x00, "Dual");
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Unknown));
            PORT_DIPSETTING(0x04, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_gberet()
        {
            ROM_START("gberet");
            ROM_REGION(0x10000, Mame.REGION_CPU1);      /* 64k for code */
            ROM_LOAD("c10_l03.bin", 0x0000, 0x4000, 0xae29e4ff);
            ROM_LOAD("c08_l02.bin", 0x4000, 0x4000, 0x240836a5);
            ROM_LOAD("c07_l01.bin", 0x8000, 0x4000, 0x41fa3e1f);

            ROM_REGION(0x04000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("f03_l07.bin", 0x00000, 0x4000, 0x4da7bd1b);

            ROM_REGION(0x10000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("e05_l06.bin", 0x00000, 0x4000, 0x0f1cb0ca);
            ROM_LOAD("e04_l05.bin", 0x04000, 0x4000, 0x523a8b66);
            ROM_LOAD("f04_l08.bin", 0x08000, 0x4000, 0x883933a4);
            ROM_LOAD("e03_l04.bin", 0x0c000, 0x4000, 0xccecda4c);

            ROM_REGION(0x0220, Mame.REGION_PROMS);
            ROM_LOAD("577h09", 0x0000, 0x0020, 0xc15e7c80); /* palette */
            ROM_LOAD("577h10", 0x0020, 0x0100, 0xe9de1e53); /* sprites */
            ROM_LOAD("577h11", 0x0120, 0x0100, 0x2a1a992b); /* characters */
            return ROM_END;
        }
        static Mame.MemoryReadAddress[] readmem = 
        {
                new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
                new Mame.MemoryReadAddress( 0xc000, 0xe03f, Mame.MRA_RAM ),
                new Mame.MemoryReadAddress( 0xf200, 0xf200, Mame.input_port_4_r ),      /* DSW1 */
                new Mame.MemoryReadAddress( 0xf400, 0xf400, Mame.input_port_5_r ),      /* DSW2 */
                new Mame.MemoryReadAddress( 0xf600, 0xf600, Mame.input_port_3_r ),      /* DSW0 */
                new Mame.MemoryReadAddress( 0xf601, 0xf601, Mame.input_port_1_r ),      /* IN1 */
                new Mame.MemoryReadAddress( 0xf602, 0xf602, Mame.input_port_0_r ),      /* IN0 */
                new Mame.MemoryReadAddress( 0xf603, 0xf603, Mame.input_port_2_r ),      /* IN2 */
                new Mame.MemoryReadAddress( 0xf800, 0xf800, Mame.MRA_NOP ),     /* gberetb only - IRQ acknowledge */
                new Mame.MemoryReadAddress( -1 )        /* end of table */
        };

        static Mame.MemoryWriteAddress[] writemem = 
        {
                new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
                new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, gberet_colorram_w, gberet_colorram ),
                new Mame.MemoryWriteAddress( 0xc800, 0xcfff, gberet_videoram_w, gberet_videoram ),
                new Mame.MemoryWriteAddress( 0xd000, 0xd0bf, Mame.MWA_RAM, Generic.spriteram_2 ),
                new Mame.MemoryWriteAddress( 0xd100, 0xd1bf, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
                new Mame.MemoryWriteAddress( 0xd200, 0xdfff, Mame.MWA_RAM ),
                new Mame.MemoryWriteAddress( 0xe000, 0xe03f, gberet_scroll_w, gberet_scrollram ),
                new Mame.MemoryWriteAddress( 0xe043, 0xe043, Mame.MWA_RAM, gberet_spritebank ),
                new Mame.MemoryWriteAddress( 0xe044, 0xe044, gberet_e044_w ),
                new Mame.MemoryWriteAddress( 0xf000, 0xf000, gberet_coincounter_w ),
                new Mame.MemoryWriteAddress( 0xf200, 0xf200, Mame.MWA_NOP ),            /* Loads the snd command into the snd latch */
                new Mame.MemoryWriteAddress( 0xf400, 0xf400, SN76496.SN76496_0_w ),     /* This address triggers the SN chip to read the data port. */
        //      { 0xf600, 0xf600, MWA_NOP },
                new Mame.MemoryWriteAddress( -1 )       /* end of table */
        };
        static Mame.GfxLayout charlayout = new Mame.GfxLayout(
                8, 8,    /* 8*8 characters */
                512,    /* 512 characters */
                4,      /* 4 bits per pixel */
                new uint[] { 0, 1, 2, 3 },       /* the four bitplanes are packed in one nibble */
                new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
                new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
                32 * 8    /* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout = new Mame.GfxLayout(

                16, 16,  /* 16*16 sprites */
                512,    /* 512 sprites */
                4,      /* 4 bits per pixel */
                new uint[] { 0, 1, 2, 3 },       /* the four bitplanes are packed in one nibble */
                new uint[]{ 0*4, 1*4, 2*4, 3*4, 4*4, 5*4, 6*4, 7*4,
                        32*8+0*4, 32*8+1*4, 32*8+2*4, 32*8+3*4, 32*8+4*4, 32*8+5*4, 32*8+6*4, 32*8+7*4 },
                new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
                        64*8+0*32, 64*8+1*32, 64*8+2*32, 64*8+3*32, 64*8+4*32, 64*8+5*32, 64*8+6*32, 64*8+7*32 },
                128 * 8   /* every sprite takes 128 consecutive bytes */
        );
        static Mame.GfxDecodeInfo[] gfxdecodeinfo = 
        {
                new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,       0, 16 ),
                new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout, 16*16, 16 ),
        };

        public static void gberet_vh_convert_color_prom(byte[] palette, ushort[] colortable, _BytePtr color_prom)
        {

            uint cpi = 0;
            int pi = 0;
            for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
            {
                int bit0 = (color_prom[cpi] >> 0) & 0x01;
                int bit1 = (color_prom[cpi] >> 1) & 0x01;
                int bit2 = (color_prom[cpi] >> 2) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                bit0 = (color_prom[cpi] >> 3) & 0x01;
                bit1 = (color_prom[cpi] >> 4) & 0x01;
                bit2 = (color_prom[cpi] >> 5) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                bit0 = 0;
                bit1 = (color_prom[cpi] >> 6) & 0x01;
                bit2 = (color_prom[cpi] >> 7) & 0x01;
                palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                cpi++;
            }
            for (int i = 0; i < (Mame.Machine.gfx[1].total_colors * Mame.Machine.gfx[1].color_granularity); i++)
            {
                if ((color_prom[cpi] & 0x0f) != 0)
                {
                    COLOR(colortable, 1, i, (ushort)(((color_prom[cpi]) & 0x0f)));
                }
                else
                {
                    COLOR(colortable, 1, i, 0);
                }
                cpi++;
            }
            for (int i = 0; i < (Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity); i++)
            {
                COLOR(colortable, 0, i, (ushort)(((color_prom[cpi++]) & 0x0f) + 0x10));
            }
        }
        static void get_tile_info(int col, int row)
        {
            int tile_index = row * 64 + col;
            byte attr = gberet_colorram[tile_index];
            Mame.SET_TILE_INFO(0, gberet_videoram[tile_index] + ((attr & 0x40) << 2), attr & 0x0f);
            Mame.tile_info.flags = (byte)(Mame.TILE_FLIPYX((attr & 0x30) >> 4) | Mame.TILE_SPLIT((attr & 0x80) >> 7));

        }
        public static int gberet_vh_start()
        {
            bg_tilemap = Mame.tilemap_create(get_tile_info, Mame.TILEMAP_SPLIT, 8, 8, 64, 32);

            if (bg_tilemap == null)
                return 0;

            bg_tilemap.transmask[0] = 0x0001; /* split type 0 has pen 1 transparent in front half */
            bg_tilemap.transmask[1] = 0xffff; /* split type 1 is totally transparent in front half */
            Mame.tilemap_set_scroll_rows(bg_tilemap, 32);

            return 0;
        }
        public static void gberet_videoram_w(int offset, int data)
        {
            if (gberet_videoram[offset] != data)
            {
                gberet_videoram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, offset % 64, offset / 64);
            }
        }

        public static void gberet_colorram_w(int offset, int data)
        {
            if (gberet_colorram[offset] != data)
            {
                gberet_colorram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bg_tilemap, offset % 64, offset / 64);
            }
        }
        public static void gberet_e044_w(int offset, int data)
        {
            /* bit 0 enables interrupts */
            interruptenable = data & 1;

            /* bit 3 flips screen */
            flipscreen = data & 0x08;
            Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);

            /* don't know about the other bits */
        }

        public static void gberet_scroll_w(int offset, int data)
        {
            int scroll;

            gberet_scrollram[offset] = (byte)data;

            scroll = gberet_scrollram[offset & 0x1f] | (gberet_scrollram[offset | 0x20] << 8);
            Mame.tilemap_set_scrollx(bg_tilemap, offset & 0x1f, scroll);
        }

        public static void gberetb_scroll_w(int offset, int data)
        {
            if (offset != 0) data |= 0x100;

            for (offset = 6; offset < 29; offset++)
                Mame.tilemap_set_scrollx(bg_tilemap, offset, data + 64 - 8);
        }


        public static int gberet_interrupt()
        {
            if (Mame.cpu_getiloops() == 0) return Mame.interrupt();
            else if ((Mame.cpu_getiloops() % 2) != 0)
            {
                if ((interruptenable) != 0) return Mame.nmi_interrupt();
            }

            return Mame.ignore_interrupt();
        }
        static void draw_sprites0(Mame.osd_bitmap bitmap)
        {
            int offs;
            _BytePtr sr;

            if ((gberet_spritebank[0] & 0x08) != 0)
                sr = Generic.spriteram_2;
            else sr = Generic.spriteram;

            for (offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
            {
                if (sr[offs + 3] != 0)
                {
                    int sx, sy;

                    sx = (sr[offs + 2] - 2 * (sr[offs + 1] & 0x80));
                    sy = sr[offs + 3];
                    if (sprites_type != 0) sy = 240 - sy;
                    bool flipx = (sr[offs + 1] & 0x10) != 0;
                    bool flipy = (sr[offs + 1] & 0x20) != 0;

                    if (flipscreen != 0)
                    {
                        sx = 240 - sx;
                        sy = 240 - sy;
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                                 (uint)(sr[offs + 0] + ((sr[offs + 1] & 0x40) << 2)),
                                                 (uint)(sr[offs + 1] & 0x0f),
                                                flipx, flipy,
                                                sx, sy,
                                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0);
                }
            }
        }
        static Mame.SN76496interface sn76496_interface =
        new Mame.SN76496interface(
            1,  /* 1 chips */
            new int[] { 18432000 / 12 },  /* 2H (generated by a custom IC) */
            new int[] { 100 }
        );
        class machine_driver_gberet : Mame.MachineDriver
        {
            public machine_driver_gberet()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6, readmem, writemem, null, null, gberet_interrupt, 32));
                frames_per_second = 30;
                vblank_duration = Mame.DEFAULT_30HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;

                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_gberet.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 2 * 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SN76496, sn76496_interface));
            }
            public override void init_machine()
            {
            }
            public override void nvram_handler(object file, int read_or_write)
            {
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                gberet_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return gberet_vh_start();
            }
            public override void vh_stop()
            {

            }
            public override void vh_eof_callback()
            {
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.tilemap_update(Mame.ALL_TILEMAPS);
                Mame.tilemap_render(Mame.ALL_TILEMAPS);
                Mame.tilemap_draw(bitmap, bg_tilemap, Mame.TILEMAP_BACK);
                draw_sprites0(bitmap);
                Mame.tilemap_draw(bitmap, bg_tilemap, Mame.TILEMAP_FRONT);
            }
        }

        public override void driver_init()
        {
            sprites_type = 0;
        }
        public driver_gberet()
        {
            drv = new machine_driver_gberet();
            year = "1985";
            name = "gberet";
            description = "Green Beret";
            manufacturer = "Konami";
            flags = Mame.ROT0;
            input_ports = input_ports_gberet();
            rom = rom_gberet();
            drv.HasNVRAMhandler = false;
        }
    }
}
