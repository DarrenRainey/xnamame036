using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_xsleena : Mame.GameDriver
    {
        static _BytePtr xain_sharedram = new _BytePtr(1);
        static _BytePtr xain_charram = new _BytePtr(1);
        static _BytePtr xain_bgram0 = new _BytePtr(1);
        static _BytePtr xain_bgram1 = new _BytePtr(1);
        static Mame.tilemap char_tilemap, bgram0_tilemap, bgram1_tilemap;
        static int flipscreen;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, xain_sharedram_r ),
	new Mame.MemoryReadAddress( 0x2000, 0x37ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x3a00, 0x3a00, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x3a01, 0x3a01, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x3a02, 0x3a02, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x3a03, 0x3a03, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x3a04, 0x3a04, xain_68705_r ),	/* from the 68705 */
	new Mame.MemoryReadAddress( 0x3a05, 0x3a05, Mame.input_port_4_r ),
//	new Mame.MemoryReadAddress( 0x3a06, 0x3a06, MRA_NOP },	/* ?? read (and discarded) on startup. Maybe reset the 68705 */
	new Mame.MemoryReadAddress( 0x4000, 0x7fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, xain_sharedram_w, xain_sharedram ),
	new Mame.MemoryWriteAddress( 0x2000, 0x27ff, xain_charram_w, xain_charram ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2fff, xain_bgram1_w, xain_bgram1 ),
	new Mame.MemoryWriteAddress( 0x3000, 0x37ff, xain_bgram0_w, xain_bgram0 ),
	new Mame.MemoryWriteAddress( 0x3800, 0x397f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x3a00, 0x3a01, xain_scrollxP1_w ),
	new Mame.MemoryWriteAddress( 0x3a02, 0x3a03, xain_scrollyP1_w ),
	new Mame.MemoryWriteAddress( 0x3a04, 0x3a05, xain_scrollxP0_w ),
	new Mame.MemoryWriteAddress( 0x3a06, 0x3a07, xain_scrollyP0_w ),
	new Mame.MemoryWriteAddress( 0x3a08, 0x3a08, xain_sound_command_w ),
	new Mame.MemoryWriteAddress( 0x3a09, 0x3a09, Mame.MWA_NOP ),	/* NMI acknowledge */
	new Mame.MemoryWriteAddress( 0x3a0a, 0x3a0a, xain_firqA_clear_w ),
	new Mame.MemoryWriteAddress( 0x3a0b, 0x3a0b, xain_irqA_clear_w ),
	new Mame.MemoryWriteAddress( 0x3a0c, 0x3a0c, xain_irqB_assert_w ),
	new Mame.MemoryWriteAddress( 0x3a0d, 0x3a0d, xain_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x3a0e, 0x3a0e, xain_68705_w ),	/* to 68705 */
	new Mame.MemoryWriteAddress( 0x3a0f, 0x3a0f, xainCPUA_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3dff, Mame.paletteram_xxxxBBBBGGGGRRRR_split1_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x3e00, 0x3fff, Mame.paletteram_xxxxBBBBGGGGRRRR_split2_w, Mame.paletteram_2 ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmemB =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, xain_sharedram_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x7fff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writememB =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, xain_sharedram_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, xain_irqA_assert_w ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2800, xain_irqB_clear_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, xainCPUB_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x07ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x1000, 0x1000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x07ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2800, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x2801, 0x2801, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, ym2203.YM2203_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0x3001, 0x3001, ym2203.YM2203_write_port_1_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 chars */
            1024,	/* 1024 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 2, 4, 6 },	/* plane offset */
            new uint[] { 1, 0, 8 * 8 + 1, 8 * 8 + 0, 16 * 8 + 1, 16 * 8 + 0, 24 * 8 + 1, 24 * 8 + 0 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            32 * 8	/* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            16, 16,	/* 8*8 chars */
            4 * 512,	/* 512 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0x8000 * 4 * 8 + 0, 0x8000 * 4 * 8 + 4, 0, 4 },	/* plane offset */
            new uint[]{ 3, 2, 1, 0, 16*8+3, 16*8+2, 16*8+1, 16*8+0,
	  32*8+3,32*8+2 ,32*8+1 ,32*8+0 ,48*8+3 ,48*8+2 ,48*8+1 ,48*8+0 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
	  8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            64 * 8	/* every char takes 64 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,   0, 8 ),	/* 8x8 text */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tilelayout, 256, 8 ),	/* 16x16 Background */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, tilelayout, 384, 8 ),	/* 16x16 Background */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX4, 0, tilelayout, 128, 8 ),	/* Sprites */
};



        /* handler called by the 2203 emulator when the internal timers cause an IRQ */
        static void irqhandler(int irq)
        {
            Mame.cpu_set_irq_line(2, Mame.cpu_m6809.M6809_FIRQ_LINE, irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }

        static YM2203interface ym2203_interface =
        new YM2203interface(
            2,			/* 2 chips */
            3000000,	/* 3 MHz ??? */
            new int[] { ym2203.YM2203_VOL(40, 50), ym2203.YM2203_VOL(40, 50) },
            new AY8910portRead[] { null, null },
            new AY8910portRead[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910handler[] { irqhandler, null }
        );



        static int xain_sharedram_r(int offset)
        {
            return xain_sharedram[offset];
        }
        static void xain_sharedram_w(int offset, int data)
        {
            /* locations 003d and 003e are used as a semaphores between CPU A and B, */
            /* so let's resync every time they are changed to avoid deadlocks */
            if ((offset == 0x003d || offset == 0x003e)
                    && xain_sharedram[offset] != data)
                Mame.cpu_yield();
            xain_sharedram[offset] = (byte)data;
        }
        static void xainCPUA_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            if ((data & 0x08) != 0) { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000)); }
            else { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x4000)); }
        }
        static void xainCPUB_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU2);

            if ((data & 0x01) != 0) { Mame.cpu_setbank(2, new _BytePtr(RAM, 0x10000)); }
            else { Mame.cpu_setbank(2, new _BytePtr(RAM, 0x4000)); }
        }
        static void xain_sound_command_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.cpu_cause_interrupt(2, Mame.cpu_m6809.M6809_INT_IRQ);
        }
        static void xain_irqA_assert_w(int offset, int data)
        {
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, Mame.ASSERT_LINE);
        }
        static void xain_irqA_clear_w(int offset, int data)
        {
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, Mame.CLEAR_LINE);
        }
        static void xain_firqA_clear_w(int offset, int data)
        {
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_FIRQ_LINE, Mame.CLEAR_LINE);
        }
        static void xain_irqB_assert_w(int offset, int data)
        {
            Mame.cpu_set_irq_line(1, Mame.cpu_m6809.M6809_IRQ_LINE, Mame.ASSERT_LINE);
        }
        static void xain_irqB_clear_w(int offset, int data)
        {
            Mame.cpu_set_irq_line(1, Mame.cpu_m6809.M6809_IRQ_LINE, Mame.CLEAR_LINE);
        }
        static int xain_68705_r(int offset)
        {
            //	if (errorlog) fprintf(errorlog,"read 68705\n");
            return 0x4d;	/* fake P5 checksum test pass */
        }
        static void xain_68705_w(int offset, int data)
        {
            //	if (errorlog) fprintf(errorlog,"write %02x to 68705\n",data);
        }
        static int xainA_interrupt()
        {
            /* returning nmi on iloops() == 0 will cause lockups because the nmi handler */
            /* waits for the vblank bit to be clear and there are other places in the code */
            /* that wait for it to be set */
            if (Mame.cpu_getiloops() == 2)
                Mame.cpu_set_nmi_line(0, Mame.PULSE_LINE);
            else
                Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_FIRQ_LINE, Mame.ASSERT_LINE);
            return Mame.cpu_m6809.M6809_INT_NONE;
        }

        static void get_bgram0_tile_info(int col, int row)
        {
            int addr = (col & 0xf) | ((col & 0x10) << 4) | ((row & 0xf) << 4) | ((row & 0x10) << 5);
            int attr = xain_bgram0[addr | 0x400];
            Mame.SET_TILE_INFO(2, xain_bgram0[addr] | ((attr & 7) << 8), (attr & 0x70) >> 4);
            Mame.tile_info.flags = (attr & 0x80) != 0 ? (byte)Mame.TILE_FLIPX : (byte)0;
        }

        static void get_bgram1_tile_info(int col, int row)
        {
            int addr = (col & 0xf) | ((col & 0x10) << 4) | ((row & 0xf) << 4) | ((row & 0x10) << 5);
            int attr = xain_bgram1[addr | 0x400];
            Mame.SET_TILE_INFO(1, xain_bgram1[addr] | ((attr & 7) << 8), (attr & 0x70) >> 4);
            Mame.tile_info.flags = (attr & 0x80) != 0 ? (byte)Mame.TILE_FLIPX : (byte)0;
        }
        static void get_char_tile_info(int col, int row)
        {
            int addr = col + row * 32;
            int attr = xain_charram[addr | 0x400];
            Mame.SET_TILE_INFO(0, xain_charram[addr] | ((attr & 3) << 8), (attr & 0xe0) >> 5);
        }
        static void xain_bgram0_w(int offset, int data)
        {
            if (xain_bgram0[offset] != data)
            {
                xain_bgram0[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bgram0_tilemap,
                         ((offset >> 4) & 0x10) | (offset & 0xf),
                         ((offset >> 5) & 0x10) | ((offset >> 4) & 0xf));
            }
        }
        static void xain_bgram1_w(int offset, int data)
        {
            if (xain_bgram1[offset] != data)
            {
                xain_bgram1[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(bgram1_tilemap,
                         ((offset >> 4) & 0x10) | (offset & 0xf),
                         ((offset >> 5) & 0x10) | ((offset >> 4) & 0xf));
            }
        }
        static void xain_charram_w(int offset, int data)
        {
            if (xain_charram[offset] != data)
            {
                xain_charram[offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(char_tilemap, offset & 0x1f, (offset & 0x3e0) >> 5);
            }
        }

        static byte[] xain_scrollxP0 = new byte[2];
        static byte[] xain_scrollyP0 = new byte[2];
        static byte[] xain_scrollxP1 = new byte[2];
        static byte[] xain_scrollyP1 = new byte[2];
        static void xain_scrollxP0_w(int offset, int data)
        {

            xain_scrollxP0[offset] = (byte)data;
            Mame.tilemap_set_scrollx(bgram0_tilemap, 0, xain_scrollxP0[0] | (xain_scrollxP0[1] << 8));
        }
        static void xain_scrollyP0_w(int offset, int data)
        {


            xain_scrollyP0[offset] = (byte)data;
            Mame.tilemap_set_scrolly(bgram0_tilemap, 0, xain_scrollyP0[0] | (xain_scrollyP0[1] << 8));
        }
        static void xain_scrollxP1_w(int offset, int data)
        {

            xain_scrollxP1[offset] = (byte)data;
            Mame.tilemap_set_scrollx(bgram1_tilemap, 0, xain_scrollxP1[0] | (xain_scrollxP1[1] << 8));
        }
        static void xain_scrollyP1_w(int offset, int data)
        {
            xain_scrollyP1[offset] = (byte)data;
            Mame.tilemap_set_scrolly(bgram1_tilemap, 0, xain_scrollyP1[0] | (xain_scrollyP1[1] << 8));
        }
        static void xain_flipscreen_w(int offset, int data)
        {
            flipscreen = data & 1;
            Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
        }


        class machine_driver_xsleena : Mame.MachineDriver
        {
            public machine_driver_xsleena()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, readmem, writemem, null, null, xainA_interrupt, 4));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, readmemB, writememB, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809 | Mame.CPU_AUDIO_CPU, 2000000, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_xsleena.gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512*2;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
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
                bgram0_tilemap = Mame.tilemap_create(
             get_bgram0_tile_info,
             Mame.TILEMAP_OPAQUE,
             16, 16,
             32, 32);
                bgram1_tilemap = Mame.tilemap_create(
                        get_bgram1_tile_info,
                        Mame.TILEMAP_TRANSPARENT,
                        16, 16,
                        32, 32);
                char_tilemap = Mame.tilemap_create(
                        get_char_tile_info,
                        Mame.TILEMAP_TRANSPARENT,
                        8, 8,
                        32, 32);

                bgram1_tilemap.transparent_pen = 0;
                char_tilemap.transparent_pen = 0;

                return 0;
            }
            public override void vh_stop()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.tilemap_update(Mame.ALL_TILEMAPS);

                Mame.palette_init_used_colors();
                for (int i = 0; i < 128; i++) // sprites
                    Mame.palette_used_colors[i + 128] = Mame.PALETTE_COLOR_USED;
                
                if (Mame.palette_recalc() != null)
                    Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);

                Mame.tilemap_draw(bitmap, bgram0_tilemap, 0);
                Mame.tilemap_draw(bitmap, bgram1_tilemap, 0);
                draw_sprites(bitmap);
                Mame.tilemap_draw(bitmap, char_tilemap, 0);
            }
            static void draw_sprites(Mame.osd_bitmap bitmap)
            {
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
                {
                    int attr = Generic.spriteram[offs + 1];
                    int numtile = Generic.spriteram[offs + 2] | ((attr & 7) << 8);
                    int color = (attr & 0x38) >> 3;

                    int sx = 239 - Generic.spriteram[offs + 3];
                    if (sx <= -7) sx += 256;
                    int sy = 240 - Generic.spriteram[offs];
                    if (sy <= -7) sy += 256;
                    bool flipx = (attr & 0x40) != 0;
                    if (flipscreen != 0)
                    {
                        sx = 239 - sx;
                        sy = 240 - sy;
                        flipx = !flipx;
                    }

                    if ((attr & 0x80) != 0)	/* double height */
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                (uint)numtile,
                                (uint)color,
                                flipx, flipscreen != 0,
                                sx - 1, flipscreen != 0 ? sy + 16 : sy - 16,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                (uint)(numtile + 1),
                                (uint)color,
                                flipx, flipscreen != 0,
                                sx - 1, sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                    else
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                (uint)numtile,
                                (uint)color,
                                flipx, flipscreen != 0,
                                sx, sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            /* do the same patch as the bootleg xsleena */
            RAM[0xd488] = 0x12;
            RAM[0xd489] = 0x12;
            RAM[0xd48a] = 0x12;
            RAM[0xd48b] = 0x12;
            RAM[0xd48c] = 0x12;
            RAM[0xd48d] = 0x12;
        }
        Mame.RomModule[] rom_xain()
        {

            ROM_START("xsleena");
            ROM_REGION(0x14000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("s-10.7d", 0x08000, 0x8000, 0x370164be);
            ROM_LOAD("s-11.7c", 0x04000, 0x4000, 0xd22bf859);
            ROM_CONTINUE(0x10000, 0x4000);

            ROM_REGION(0x14000, Mame.REGION_CPU2);	/* 64k for code */
            ROM_LOAD("s-2.3b", 0x08000, 0x8000, 0xa1a860e2);
            ROM_LOAD("s-1.2b", 0x04000, 0x4000, 0x948b9757);
            ROM_CONTINUE(0x10000, 0x4000);

            ROM_REGION(0x10000, Mame.REGION_CPU3);	/* 64k for code */
            ROM_LOAD("s-3.4s", 0x8000, 0x8000, 0xa5318cb8);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("s-12.8b", 0x00000, 0x8000, 0x83c00dd8);/* chars */

            ROM_REGION(0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("s-21.16i", 0x00000, 0x8000, 0x11eb4247);/* tiles */
            ROM_LOAD("s-22.15i", 0x08000, 0x8000, 0x422b536e);
            ROM_LOAD("s-23.14i", 0x10000, 0x8000, 0x828c1b0c);
            ROM_LOAD("s-24.13i", 0x18000, 0x8000, 0xd37939e0);
            ROM_LOAD("s-13.16g", 0x20000, 0x8000, 0x8f0aa1a7);
            ROM_LOAD("s-14.15g", 0x28000, 0x8000, 0x45681910);
            ROM_LOAD("s-15.14g", 0x30000, 0x8000, 0xa8eeabc8);
            ROM_LOAD("s-16.13g", 0x38000, 0x8000, 0xe59a2f27);

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("s-6.4h", 0x00000, 0x8000, 0x5c6c453c); /* tiles */
            ROM_LOAD("s-5.4l", 0x08000, 0x8000, 0x59d87a9a);
            ROM_LOAD("s-4.4m", 0x10000, 0x8000, 0x84884a2e);
            /* 0x60000-0x67fff empty */
            ROM_LOAD("s-7.4f", 0x20000, 0x8000, 0x8d637639);
            ROM_LOAD("s-8.4d", 0x28000, 0x8000, 0x71eec4e6);
            ROM_LOAD("s-9.4c", 0x30000, 0x8000, 0x7fc9704f);
            /* 0x80000-0x87fff empty */

            ROM_REGION(0x40000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("s-25.10i", 0x00000, 0x8000, 0x252976ae);/* sprites */
            ROM_LOAD("s-26.9i", 0x08000, 0x8000, 0xe6f1e8d5);
            ROM_LOAD("s-27.8i", 0x10000, 0x8000, 0x785381ed);
            ROM_LOAD("s-28.7i", 0x18000, 0x8000, 0x59754e3d);
            ROM_LOAD("s-17.10g", 0x20000, 0x8000, 0x4d977f33);
            ROM_LOAD("s-18.9g", 0x28000, 0x8000, 0x3f3b62a0);
            ROM_LOAD("s-19.8g", 0x30000, 0x8000, 0x76641ee3);
            ROM_LOAD("s-20.7g", 0x38000, 0x8000, 0x37671f36);

            ROM_REGION(0x0100, Mame.REGION_PROMS);
            ROM_LOAD("mb7114e.59", 0x0000, 0x0100, 0xfed32888);	/* timing? (not used) */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_xain()
        {

            INPUT_PORTS_START("xsleena");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);

            PORT_START();	/* DSW0 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x10, 0x10, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x20, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x20, DEF_STR("Yes"));
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x40, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x03, "Easy");
            PORT_DIPSETTING(0x02, "Normal");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x0c, 0x0c, "Game Time");
            PORT_DIPSETTING(0x0c, "Slow");
            PORT_DIPSETTING(0x08, "Normal");
            PORT_DIPSETTING(0x04, "Fast");
            PORT_DIPSETTING(0x00, "Very Fast");
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x30, "20k 70k and every 70k");
            PORT_DIPSETTING(0x20, "30k 80k and every 80k");
            PORT_DIPSETTING(0x10, "20k and 80k");
            PORT_DIPSETTING(0x00, "30k and 80k");
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR("Lives"));
            PORT_DIPSETTING(0xc0, "3");
            PORT_DIPSETTING(0x80, "4");
            PORT_DIPSETTING(0x40, "6");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "Infinite", IP_KEY_NONE, IP_JOY_NONE);

            PORT_START();/* IN2 */
            PORT_BIT(0x03, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* when 0, 68705 is ready to send data */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* when 1, 68705 is ready to receive data */
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_VBLANK);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        public driver_xsleena()
        {
            drv = new machine_driver_xsleena();
            year = "1986";
            name = "xsleena";
            description = "Xain'd Sleena";
            manufacturer = "Technos";
            flags = Mame.ROT0;
            input_ports = input_ports_xain();
            rom = rom_xain();
            drv.HasNVRAMhandler = false;
        }
    }
}
