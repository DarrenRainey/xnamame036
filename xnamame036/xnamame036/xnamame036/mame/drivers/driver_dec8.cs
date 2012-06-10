using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_dec8 : Mame.GameDriver
    {
        static int nmi_enable, int_enable;
        static int i8751_return, i8751_value;
        static int msm5205next;
        static _BytePtr pf_video;
        static bool[] pf_dirty;
        static int[] scroll1 = new int[4], scroll2 = new int[4], pf1_attr = new int[8], pf2_attr = new int[8];
        static Mame.osd_bitmap pf1_bitmap, pf2_bitmap, tf2_bitmap;
        static _BytePtr dec8row = new _BytePtr(1);
        static int blank_tile, shackled_priority, flipscreen;

        public static Mame.MemoryReadAddress[] dec8_s_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x05ff, Mame.MRA_RAM),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] dec8_s_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x05ff, Mame.MWA_RAM),
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, ym2203.YM2203_control_port_0_w ), /* OPN */
	new Mame.MemoryWriteAddress( 0x2001, 0x2001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, YM3812.YM3812_control_port_0_w ), /* OPL */
	new Mame.MemoryWriteAddress( 0x4001, 0x4001, YM3812.YM3812_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        public static Mame.GfxLayout tiles =
        new Mame.GfxLayout(
            16, 16,
            4096,
            4,
            new uint[] { 0x60000 * 8, 0x40000 * 8, 0x20000 * 8, 0x00000 * 8 },
        new uint[]	{ 16*8, 1+(16*8), 2+(16*8), 3+(16*8), 4+(16*8), 5+(16*8), 6+(16*8), 7+(16*8),
		0,1,2,3,4,5,6,7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 8 * 8, 9 * 8, 10 * 8, 11 * 8, 12 * 8, 13 * 8, 14 * 8, 15 * 8 },
            16 * 16
        );
        public static Mame.GfxLayout charlayout_32k =
new Mame.GfxLayout(
    8, 8,
    1024,
    2,
    new uint[] { 0x4000 * 8, 0x0000 * 8 },
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every sprite takes 8 consecutive bytes */
);
        public static YM3812interface ym3812_interface =
new YM3812interface(
    1,			/* 1 chip */
    3000000,	/* 3 MHz */
    new int[] { 35 },
    new handlerdelegate[] { irqhandler }
);

        public static YM2203interface ym2203_interface =
new YM2203interface(
    1,
    1500000,	/* Should be accurate for all games, derived from 12MHz crystal */
    new int[] { ym2203.YM2203_VOL(20, 23) },
    new AY8910portRead[] { null },
    new AY8910portRead[] { null },
    new AY8910portWrite[] { null },
    new AY8910portWrite[] { null }, new AY8910handler[] { null }
);
        static void irqhandler(int linestate)
        {
            Mame.cpu_set_irq_line(1, 0, linestate); /* M6502_INT_IRQ */
        }
        public void PLAYER1_JOYSTICK()/* Player 1 controls */
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
        }
        public void PLAYER2_JOYSTICK() /* Player 2 controls */
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
        }
        public static int dec8_video_r(int offset)
        {
            return pf_video[offset];
        }
        public static void dec8_bank_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            bankaddress = 0x10000 + (data & 0x0f) * 0x4000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }
        public static void dec8_sound_w(int offset, int data)
        {
            Mame.soundlatch_w(0, data);
            Mame.cpu_cause_interrupt(1, Mame.cpu_m6502.M6502_INT_NMI);
        }

        public static void dec8_video_w(int offset, int data)
        {
            if (pf_video[offset] != data)
            {
                pf_video[offset] = (byte)data;
                pf_dirty[offset / 2] = true;
            }
        }

        public static int dec8_vh_start()
        {
            uint[] pen_usage = Mame.Machine.gfx[1].pen_usage;

            pf1_bitmap = Mame.osd_create_bitmap(512, 512);
            pf2_bitmap = Mame.osd_create_bitmap(512, 512);
            tf2_bitmap = Mame.osd_create_bitmap(512, 512);

            pf_video = new _BytePtr(0x1000);
            pf_dirty = new bool[0x800];
            for (int i = 0; i < 0x800; i++) pf_dirty[i] = true;

            /* Kludge: Find a blank tile */
            blank_tile = 0;
            for (int i = 0; i < 0xfff; i++)
                if ((pen_usage[i] & ~1) == 0)
                {
                    blank_tile = i;
                    i = 0x1000;
                }

            /* Stupid kludge - fix it later :) */
            shackled_priority = 0;
            if (Mame.Machine.gamedrv.name.CompareTo("breywood") == 0) shackled_priority = 1;
            if (Mame.Machine.gamedrv.name.CompareTo("shackled") == 0) shackled_priority = 1;

            return 0;
        }
        public static void dec8_vh_stop()
        {
            Mame.osd_free_bitmap(pf1_bitmap);
            Mame.osd_free_bitmap(pf2_bitmap);
            Mame.osd_free_bitmap(tf2_bitmap);
            pf_video = null;
            pf_dirty = null;
        }
        public static void dec8_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int my, mx, offs, color, tile;
            int scrollx, scrolly;

            for (int i = 0; i < 256; i++) Mame.palette_used_colors[i] = Mame.PALETTE_COLOR_USED;
            Mame.palette_used_colors[64] = Mame.PALETTE_COLOR_TRANSPARENT;
            Mame.palette_used_colors[192] = Mame.PALETTE_COLOR_TRANSPARENT;
            Mame.palette_used_colors[192 + 16] = Mame.PALETTE_COLOR_TRANSPARENT;
            Mame.palette_used_colors[192 + 32] = Mame.PALETTE_COLOR_TRANSPARENT;
            Mame.palette_used_colors[192 + 48] = Mame.PALETTE_COLOR_TRANSPARENT;

            if (Mame.palette_recalc() != null)
                for (int i = 0; i < 0x800; i++) pf_dirty[i] = true;

            /* Playfield 2 - Foreground */
            mx = -1; my = 0;
            for (offs = 0x800; offs < 0xc00; offs += 2)
            {
                mx++;
                if (mx == 16) { mx = 0; my++; }
                if (!pf_dirty[offs / 2]) continue; else pf_dirty[offs / 2] = false;
                tile = pf_video[offs + 1] + (pf_video[offs] << 8);
                color = ((tile & 0xf000) >> 12);
                tile = tile & 0xfff;
                Mame.drawgfx(pf2_bitmap, Mame.Machine.gfx[2], (uint)tile,
                    (uint)color, false, false, 16 * mx, 16 * my,
                    null, Mame.TRANSPARENCY_NONE, 0);
            }

            mx = -1; my = 0;
            for (offs = 0xc00; offs < 0x1000; offs += 2)
            {
                mx++;
                if (mx == 16) { mx = 0; my++; }
                if (!pf_dirty[offs / 2]) continue; else pf_dirty[offs / 2] = false;
                tile = pf_video[offs + 1] + (pf_video[offs] << 8);
                color = ((tile & 0xf000) >> 12);
                tile = tile & 0xfff;
                Mame.drawgfx(pf2_bitmap, Mame.Machine.gfx[2], (uint)tile,
                    (uint)color, false, false, (16 * mx) + 256, 16 * my,
                    null, Mame.TRANSPARENCY_NONE, 0);
            }

            /* Playfield 1 */
            mx = -1; my = 0;
            for (offs = 0x000; offs < 0x400; offs += 2)
            {
                mx++;
                if (mx == 16) { mx = 0; my++; }
                if (!pf_dirty[offs / 2]) continue; else pf_dirty[offs / 2] = false;
                tile = pf_video[offs + 1] + (pf_video[offs] << 8);
                color = ((tile & 0xf000) >> 12);
                tile = tile & 0xfff;
                Mame.drawgfx(pf1_bitmap, Mame.Machine.gfx[3], (uint)tile,
                        (uint)color, false, false, 16 * mx, 16 * my,
                        null, Mame.TRANSPARENCY_NONE, 0);
            }

            mx = -1; my = 0;
            for (offs = 0x400; offs < 0x800; offs += 2)
            {
                mx++;
                if (mx == 16) { mx = 0; my++; }
                if (!pf_dirty[offs / 2]) continue; else pf_dirty[offs / 2] = false;
                tile = pf_video[offs + 1] + (pf_video[offs] << 8);
                color = ((tile & 0xf000) >> 12);
                tile = tile & 0xfff;
                Mame.drawgfx(pf1_bitmap, Mame.Machine.gfx[3], (uint)tile,
                        (uint)color, false, false, (16 * mx) + 256, 16 * my,
                        null, Mame.TRANSPARENCY_NONE, 0);
            }

            scrolly = -((scroll1[2] << 8) + scroll1[3]);
            scrollx = -((scroll1[0] << 8) + scroll1[1]);
            Mame.copyscrollbitmap(bitmap, pf1_bitmap, 1, new int[] { scrollx }, 1, new int[] { scrolly }, null, Mame.TRANSPARENCY_NONE, 0);

            draw_sprites2(bitmap, 1);

            scrolly = -((scroll2[2] << 8) + scroll2[3]);
            scrollx = -(((scroll2[0] & 1) << 8) + scroll2[1]);
            Mame.copyscrollbitmap(bitmap, pf2_bitmap, 1, new int[] { scrollx }, 1, new int[] { scrolly }, null, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

            draw_sprites2(bitmap, 2);

            draw_characters(bitmap, 0xe0, 5);
        }
        static void draw_sprites2(Mame.osd_bitmap bitmap, int priority)
        {
            /* Sprites */
            for (int offs = 0; offs < 0x800; offs += 8)
            {
                int x, y, sprite, colour, multi, fx, fy, inc, flash;

                y = Generic.buffered_spriteram[offs + 1] + (Generic.buffered_spriteram[offs] << 8);
                if ((y & 0x8000) == 0) continue;
                x = Generic.buffered_spriteram[offs + 5] + (Generic.buffered_spriteram[offs + 4] << 8);
                colour = ((x & 0xf000) >> 12);
                flash = x & 0x800;
                if (flash != 0 && (Mame.cpu_getcurrentframe() & 1) != 0) continue;

                if (priority == 1 && (colour & 4) != 0) continue;
                if (priority == 2 && (colour & 4) == 0) continue;

                fx = y & 0x2000;
                fy = y & 0x4000;
                multi = (1 << ((y & 0x1800) >> 11)) - 1;	/* 1x, 2x, 4x, 8x height */

                /* multi = 0   1   3   7 */
                sprite = Generic.buffered_spriteram[offs + 3] + (Generic.buffered_spriteram[offs + 2] << 8);
                sprite &= 0x0fff;

                x = x & 0x01ff;
                y = y & 0x01ff;
                if (x >= 256) x -= 512;
                if (y >= 256) y -= 512;
                x = 240 - x;
                y = 240 - y;

                sprite &= ~multi;
                if (fy != 0)
                    inc = -1;
                else
                {
                    sprite += multi;
                    inc = 1;
                }

                while (multi >= 0)
                {
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)(sprite - multi * inc),
                            (uint)colour,
                            fx != 0, fy != 0,
                            x, y - 16 * multi,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    multi--;
                }
            }
        }

        static void draw_characters(Mame.osd_bitmap bitmap, int mask, int shift)
        {
            int mx, my, tile, color, offs;

            for (offs = 0x800 - 2; offs >= 0; offs -= 2)
            {
                tile = Generic.videoram[offs + 1] + ((Generic.videoram[offs] & 0xf) << 8);

                if (tile == 0) continue;

                color = (Generic.videoram[offs] & mask) >> shift;
                mx = (offs / 2) % 32;
                my = (offs / 2) / 32;

                Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                        (uint)tile, (uint)color, false, false, 8 * mx, 8 * my,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }
        }


        public static void dec8_pf1_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    pf1_attr[offset] = data;
                    break;
            }
            //if (errorlog) fprintf(errorlog, "Write %d to playfield 1 register %d\n", data, offset);
        }
        public static void dec8_pf2_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    pf2_attr[offset] = data;
                    break;
            }
            //if (errorlog) fprintf(errorlog, "Write %d to playfield 2 register %d\n", data, offset);
        }
        public static void dec8_bac06_0_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    pf1_attr[offset] = data;
                    break;

                case 0x10: /* Scroll registers */
                case 0x11:
                case 0x12:
                case 0x13:
                    scroll1[offset - 0x10] = data;
                    break;
            }
        }
        public static void dec8_bac06_1_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    pf2_attr[offset] = data;
                    break;

                case 0x10: /* Scroll registers */
                case 0x11:
                case 0x12:
                case 0x13:
                    scroll2[offset - 0x10] = data;
                    break;
            }
        }
        public static void dec8_scroll1_w(int offset, int data)
        {
            scroll1[offset] = data;
        }
        public static void dec8_scroll2_w(int offset, int data)
        {
            scroll2[offset] = data;
        }


        /* Only use with tilemap games (SRDARWIN) for now */
        static int old;
        public static void dec8_flipscreen_w(int offset, int data)
        {
            flipscreen = data;
            if (flipscreen != old)
                Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
            old = data;
        }
        public override void driver_init() { }
    }
    class driver_cobracom : driver_dec8
    {
        static Mame.MemoryReadAddress[] cobra_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x07ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0800, 0x17ff, dec8_video_r ),
	new Mame.MemoryReadAddress( 0x1800, 0x2fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x3000, 0x31ff, Mame.paletteram_r ),
	new Mame.MemoryReadAddress( 0x3800, 0x3800, Mame.input_port_0_r ), /* Player 1 */
	new Mame.MemoryReadAddress( 0x3801, 0x3801, Mame.input_port_1_r ), /* Player 2 */
	new Mame.MemoryReadAddress( 0x3802, 0x3802, Mame.input_port_3_r ), /* Dip 1 */
	new Mame.MemoryReadAddress( 0x3803, 0x3803, Mame.input_port_4_r ), /* Dip 2 */
	new Mame.MemoryReadAddress( 0x3a00, 0x3a00, Mame.input_port_2_r ), /* VBL & coins */
	new Mame.MemoryReadAddress( 0x4000, 0x7fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] cobra_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x07ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0800, 0x17ff, dec8_video_w ),
	new Mame.MemoryWriteAddress( 0x1800, 0x1fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x27ff, Mame.MWA_RAM, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2fff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x3000, 0x31ff, Mame.paletteram_xxxxBBBBGGGGRRRR_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x3200, 0x37ff, Mame.MWA_RAM ), /* Unknown, seemingly unused */
	new Mame.MemoryWriteAddress( 0x3800, 0x381f, dec8_bac06_0_w ),
	new Mame.MemoryWriteAddress( 0x3a00, 0x3a1f, dec8_bac06_1_w ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3c00, dec8_bank_w ),
	new Mame.MemoryWriteAddress( 0x3c02, 0x3c02, Generic.buffer_spriteram_w ), /* DMA */
	new Mame.MemoryWriteAddress( 0x3e00, 0x3e00, dec8_sound_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static Mame.GfxDecodeInfo[] cobracom_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout_32k, 0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tiles,         64, 4 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, tiles,        192, 4 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX4, 0, tiles,        128, 4 ),
};
        class machine_driver_cobracom : Mame.MachineDriver
        {
            public machine_driver_cobracom()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, cobra_readmem, cobra_writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502 | Mame.CPU_AUDIO_CPU, 1500000, dec8_s_readmem, dec8_s_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 58;
                vblank_duration = 529;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 1 * 8, 31 * 8 - 1);
                gfxdecodeinfo = cobracom_gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_UPDATE_BEFORE_VBLANK | Mame.VIDEO_BUFFERS_SPRITERAM;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
                //sound.Add(new Mame.MachineSound(Mame.SOUND_YM3812, ym3812_interface));
            }
            public override void init_machine()
            {
                //nothing
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
                return dec8_vh_start();
            }
            public override void vh_stop()
            {
                dec8_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                dec8_vh_screenrefresh(bitmap, full_refresh);
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
        Mame.RomModule[] rom_cobracom()
        {
            ROM_START("cobracom");
            ROM_REGION(0x30000, Mame.REGION_CPU1);
            ROM_LOAD("el11-5.bin", 0x08000, 0x08000, 0xaf0a8b05);
            ROM_LOAD("el12-4.bin", 0x10000, 0x10000, 0x7a44ef38);
            ROM_LOAD("el13.bin", 0x20000, 0x10000, 0x04505acb);

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64K for sound CPU */
            ROM_LOAD("el10-4.bin", 0x8000, 0x8000, 0xedfad118);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);/* characters */
            ROM_LOAD("el14.bin", 0x00000, 0x08000, 0x47246177);

            ROM_REGION(0x80000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);	/* sprites */
            ROM_LOAD("el00-4.bin", 0x00000, 0x10000, 0x122da2a8);
            ROM_LOAD("el01-4.bin", 0x20000, 0x10000, 0x27bf705b);
            ROM_LOAD("el02-4.bin", 0x40000, 0x10000, 0xc86fede6);
            ROM_LOAD("el03-4.bin", 0x60000, 0x10000, 0x1d8a855b);

            ROM_REGION(0x80000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);/* tiles 1 */
            ROM_LOAD("el05.bin", 0x00000, 0x10000, 0x1c4f6033);
            ROM_LOAD("el06.bin", 0x20000, 0x10000, 0xd24ba794);
            ROM_LOAD("el04.bin", 0x40000, 0x10000, 0xd80a49ce);
            ROM_LOAD("el07.bin", 0x60000, 0x10000, 0x6d771fc3);

            ROM_REGION(0x80000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);	/* tiles 2 */
            ROM_LOAD("el08.bin", 0x00000, 0x08000, 0xcb0dcf4c);
            ROM_CONTINUE(0x40000, 0x08000);
            ROM_LOAD("el09.bin", 0x20000, 0x08000, 0x1fae5be7);
            ROM_CONTINUE(0x60000, 0x08000);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_cobracom()
        {
            INPUT_PORTS_START("cobracom");
            PORT_START(); /* Player 1 controls */
            PLAYER1_JOYSTICK();
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();	/* Player 2 controls */
            PLAYER2_JOYSTICK();
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_VBLANK);

            PORT_START();	/* Dip switch bank 1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x80, DEF_STR("Cocktail"));

            PORT_START();	/* Dip switch bank 2 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "Infinite", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x04, "Easy");
            PORT_DIPSETTING(0x0c, "Normal");
            PORT_DIPSETTING(0x08, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x10, 0x10, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x10, DEF_STR("Yes"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x40, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        public driver_cobracom()
        {
            drv = new machine_driver_cobracom();
            year = "1988";
            name = "cobracom";
            description = "Cobra-Command (World revision 5)";
            manufacturer = "Data East Corporation";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_cobracom();
            rom = rom_cobracom();
            drv.HasNVRAMhandler = false;
        }
    }
}
