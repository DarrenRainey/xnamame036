using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_jackal : Mame.GameDriver
    {
        static _BytePtr jackal_scrollram = new _BytePtr(1);
        static _BytePtr jackal_videoctrl = new _BytePtr(1);
        static _BytePtr jackal_rambank = null;
        static _BytePtr jackal_spritebank = null;
        static Mame.MemoryReadAddress[] jackal_readmem =
{
	new Mame.MemoryReadAddress( 0x0010, 0x0010, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x0011, 0x0011, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x0012, 0x0012, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x0013, 0x0013, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x0014, 0x0014, rotary_0_r ),
	new Mame.MemoryReadAddress( 0x0015, 0x0015, rotary_1_r ),
	new Mame.MemoryReadAddress( 0x0018, 0x0018, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0x0020, 0x005f, jackal_zram_r ),	/* MAIN   Z RAM,SUB    Z RAM */
	new Mame.MemoryReadAddress( 0x0060, 0x1fff, jackal_commonram_r ),	/* M COMMON RAM,S COMMON RAM */
	new Mame.MemoryReadAddress( 0x2000, 0x2fff, jackal_voram_r ),	/* MAIN V O RAM,SUB  V O RAM */
	new Mame.MemoryReadAddress( 0x3000, 0x3fff, jackal_spriteram_r ),	/* MAIN V O RAM,SUB  V O RAM */
	new Mame.MemoryReadAddress( 0x4000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] jackal_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0003, Mame.MWA_RAM, jackal_videoctrl ),	/* scroll + other things */
	new Mame.MemoryWriteAddress( 0x0004, 0x0004, jackal_interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x0019, 0x0019, Mame.MWA_NOP ),	/* possibly watchdog reset */
	new Mame.MemoryWriteAddress( 0x001c, 0x001c, jackal_rambank_w ),
	new Mame.MemoryWriteAddress( 0x0020, 0x005f, jackal_zram_w ),
	new Mame.MemoryWriteAddress( 0x0060, 0x1fff, jackal_commonram_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2fff, jackal_voram_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3fff, jackal_spriteram_w),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] jackal_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x2001, 0x2001, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x43ff, Mame.MRA_RAM ),		/* COLOR RAM (Self test only check 0x4000-0x423f */
	new Mame.MemoryReadAddress( 0x6000, 0x605f, Mame.MRA_RAM ),		/* SOUND RAM (Self test check 0x6000-605f, 0x7c00-0x7fff */
	new Mame.MemoryReadAddress( 0x6060, 0x7fff, jackal_commonram1_r ), /* COMMON RAM */
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1)	/* end of taMame.ble */
};

        static Mame.MemoryWriteAddress[] jackal_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, YM2151.YM2151_register_port_0_w),
	new Mame.MemoryWriteAddress( 0x2001, 0x2001, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff, Mame.paletteram_xBBBBBGGGGGRRRRR_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x6000, 0x605f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x6060, 0x7fff, jackal_commonram1_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            4096,	/* 4096 characters */
            8,	/* 8 bits per pixel (!) */
            new uint[] { 0, 1, 2, 3, 0x20000 * 8 + 0, 0x20000 * 8 + 1, 0x20000 * 8 + 2, 0x20000 * 8 + 3 },
            new uint[] { 0 * 4, 1 * 4, 0x40000 * 8 + 0 * 4, 0x40000 * 8 + 1 * 4, 2 * 4, 3 * 4, 0x40000 * 8 + 2 * 4, 0x40000 * 8 + 3 * 4 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            1024,	/* 1024 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the four bitplanes are packed in one nibble */
            new uint[]{ 0*4, 1*4, 0x40000*8+0*4, 0x40000*8+1*4, 2*4, 3*4, 0x40000*8+2*4, 0x40000*8+3*4,
			16*8+0*4, 16*8+1*4, 16*8+0x40000*8+0*4, 16*8+0x40000*8+1*4, 16*8+2*4, 16*8+3*4, 16*8+0x40000*8+2*4, 16*8+0x40000*8+3*4 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			16*16, 17*16, 18*16, 19*16, 20*16, 21*16, 22*16, 23*16 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout8 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            4096,	/* 4096 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },	/* the four bitplanes are packed in one nibble */
            new uint[] { 0 * 4, 1 * 4, 0x40000 * 8 + 0 * 4, 0x40000 * 8 + 1 * 4, 2 * 4, 3 * 4, 0x40000 * 8 + 2 * 4, 0x40000 * 8 + 3 * 4 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout topgunbl_charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            4096,	/* 4096 characters */
            8,	/* 8 bits per pixel (!) */
            new uint[] { 0, 1, 2, 3, 0x20000 * 8 + 0, 0x20000 * 8 + 1, 0x20000 * 8 + 2, 0x20000 * 8 + 3 },
            new uint[] { 2 * 4, 3 * 4, 0 * 4, 1 * 4, 6 * 4, 7 * 4, 4 * 4, 5 * 4 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8	/* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout topgunbl_spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            1024,	/* 1024 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[]{ 2*4, 3*4, 0*4, 1*4, 6*4, 7*4, 4*4, 5*4,
			32*8+2*4, 32*8+3*4, 32*8+0*4, 32*8+1*4, 32*8+6*4, 32*8+7*4, 32*8+4*4, 32*8+5*4 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
			16*32, 17*32, 18*32, 19*32, 20*32, 21*32, 22*32, 23*32 },
            128 * 8	/* every char takes 32 consecutive bytes */
        );

        static new Mame.GfxLayout topgunbl_spritelayout8 =
            new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            4096,	/* 4096 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[] { 2 * 4, 3 * 4, 0 * 4, 1 * 4, 6 * 4, 7 * 4, 4 * 4, 5 * 4 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8	/* every char takes 32 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] jackal_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x00000, charlayout,          256,  1 ),	/* colors 256-511 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x10000, spritelayout,        512, 16 ),	/* colors   0- 15 with lookup */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x30000, spritelayout,  512+16*16, 16 ),	/* colors  16- 31 with lookup */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x10000, spritelayout8,       512, 16 ),  /* to handle 8x8 sprites */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x30000, spritelayout8, 512+16*16, 16 ),  /* to handle 8x8 sprites */
};

        static Mame.GfxDecodeInfo[] topgunbl_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x00000, topgunbl_charlayout,          256,  1 ),	/* colors 256-511 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x40000, topgunbl_spritelayout,        512, 16 ),	/* colors   0- 15 with lookup */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x60000, topgunbl_spritelayout,  512+16*16, 16 ),	/* colors  16- 31 with lookup */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x40000, topgunbl_spritelayout8,       512, 16 ),	/* to handle 8x8 sprites */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x60000, topgunbl_spritelayout8, 512+16*16, 16 ),	/* to handle 8x8 sprites */
};



        static YM2151interface ym2151_interface =
        new YM2151interface(
            1,
            3580000,
            new int[] { YM2151.YM3012_VOL(50, Mame.MIXER_PAN_LEFT, 50, Mame.MIXER_PAN_RIGHT) },
            new YM2151writehandler[] { null }
        );

        static int jackal_zram_r(int offset)
        {
            return jackal_rambank[0x0020 + offset];
        }
        static int jackal_commonram_r(int offset)
        {
            return jackal_rambank[0x0060 + offset];
        }
        static int jackal_commonram1_r(int offset)
        {
            return (Mame.memory_region(Mame.REGION_CPU1))[0x0060 + offset];
        }
        static int jackal_voram_r(int offset)
        {
            return jackal_rambank[0x2000 + offset];
        }
        static int jackal_spriteram_r(int offset)
        {
            return jackal_spritebank[0x3000 + offset];
        }
        static void jackal_rambank_w(int offset, int data)
        {
            jackal_rambank = new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), ((data & 0x10) << 12));
            jackal_spritebank = new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), ((data & 0x08) << 13));
            Mame.cpu_setbank(1, new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), ((data & 0x20) << 11) + 0x4000));
        }
        static void jackal_zram_w(int offset, int data)
        {
            jackal_rambank[0x0020 + offset] = (byte)data;
        }
        static void jackal_commonram_w(int offset, int data)
        {
            jackal_rambank[0x0060 + offset] = (byte)data;
        }
        static void jackal_commonram1_w(int offset, int data)
        {
            (Mame.memory_region(Mame.REGION_CPU1))[0x0060 + offset] = (byte)data;
            (Mame.memory_region(Mame.REGION_CPU2))[0x6060 + offset] = (byte)data;
        }
        static void jackal_voram_w(int offset, int data)
        {
            if ((offset & 0xF800) == 0)
            {
                Generic.dirtybuffer[offset & 0x3FF] = true;
            }
            jackal_rambank[0x2000 + offset] = (byte)data;
        }
        static void jackal_spriteram_w(int offset, int data)
        {
            jackal_spritebank[0x3000 + offset] = (byte)data;
        }
        static int rotary_0_r(int offset)
        {
            return (1 << (Mame.readinputport(6) * 8 / 256)) ^ 0xff;
        }
        static int rotary_1_r(int offset)
        {
            return (1 << (Mame.readinputport(7) * 8 / 256)) ^ 0xff;
        }
        static byte intenable;

        static void jackal_interrupt_enable_w(int offset, int data)
        {
            intenable = (byte)data;
        }
        static int jackal_interrupt()
        {
            if ((intenable & 0x02) != 0) return Mame.nmi_interrupt();
            if ((intenable & 0x08) != 0) return Mame.cpu_m6809.M6809_INT_IRQ;
            if ((intenable & 0x10) != 0) return Mame.cpu_m6809.M6809_INT_FIRQ;
            return Mame.ignore_interrupt();
        }
        class machine_driver_jackal : Mame.MachineDriver
        {
            public machine_driver_jackal()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, jackal_readmem, jackal_writemem, null, null, jackal_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, jackal_sound_readmem, jackal_sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 10;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                gfxdecodeinfo = jackal_gfxdecodeinfo;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 2 * 8, 30 * 8 - 1);
                total_colors = 512;
                color_table_len = 512 + 16 * 16 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
            }
            public override void init_machine()
            {
                Mame.cpu_setbank(1, new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), 0x4000));
                jackal_rambank = Mame.memory_region(Mame.REGION_CPU1);
                jackal_spritebank = Mame.memory_region(Mame.REGION_CPU1);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {

                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    COLOR(colortable, 1, i, color_prom[0] & 0x0f);
                    color_prom.offset++;
                }
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                {
                    COLOR(colortable, 2, i, (color_prom[0] & 0x0f) + 16);
                    color_prom.offset++;
                }
            }
            public override int vh_start()
            {
                Generic.videoram_size[0] = 0x400;

                Generic.dirtybuffer = new bool[Generic.videoram_size[0]];
                Generic.SetDirtyBuffer(true);
                Generic.tmpbitmap = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(Generic.tmpbitmap);
                Generic.dirtybuffer = null;
                Generic.tmpbitmap = null;
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                _BytePtr sr, ss;
                int offs, i;
                _BytePtr RAM = (Mame.memory_region(Mame.REGION_CPU1));


                if (Mame.palette_recalc() != null)
                {
                    Generic.SetDirtyBuffer(true);
                    //memset(dirtybuffer,1,videoram_size);
                }

                jackal_scrollram = new _BytePtr(RAM, 0x0020);
                Generic.colorram = new _BytePtr(RAM, 0x2000);
                Generic.videoram = new _BytePtr(RAM, 0x2400);

                Generic.spriteram_size[0] = 0x500;

                if ((jackal_videoctrl[0x03] & 0x08) != 0)
                {
                    sr = new _BytePtr(RAM, 0x03800);	// Sprite 2
                    ss = new _BytePtr(RAM, 0x13800);	// Additional Sprite 2
                }
                else
                {
                    sr = new _BytePtr(RAM, 0x03000);	// Sprite 1
                    ss = new _BytePtr(RAM, 0x13000);	// Additional Sprite 1
                }

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy;

                        Generic.dirtybuffer[offs] = false;

                        sx = offs % 32;
                        sy = offs / 32;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                            (uint)(Generic.videoram[offs] + ((Generic.colorram[offs] & 0xc0) << 2) + ((Generic.colorram[offs] & 0x30) << 6)),
                            0,//colorram[offs] & 0x0f, there must be a PROM like in Contra
                            (Generic.colorram[offs] & 0x10) != 0, (Generic.colorram[offs] & 0x20) != 0,
                            8 * sx, 8 * sy,
                            null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int h_scroll_num = 0, v_scroll_num = 0;
                    int[] h_scroll = new int[32], v_scroll = new int[32];

                    if ((jackal_videoctrl[2] & 0x08) != 0)
                    {
                        h_scroll_num = 32;
                        for (i = 0; i < 32; i++)
                            h_scroll[i] = -(jackal_scrollram[i]);
                    }

                    if ((jackal_videoctrl[2] & 0x04) != 0)
                    {
                        v_scroll_num = 32;
                        for (i = 0; i < 32; i++)
                            v_scroll[i] = -(jackal_scrollram[i]);
                    }

                    if (jackal_videoctrl[0] != 0)
                    {
                        v_scroll_num = 1;
                        v_scroll[0] = -(jackal_videoctrl[0]);
                    }

                    if (jackal_videoctrl[1] != 0)
                    {
                        h_scroll_num = 1;
                        h_scroll[0] = -(jackal_videoctrl[1]);
                    }

                    if ((h_scroll_num == 0) && (v_scroll_num == 0))
                        Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    else
                        Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, h_scroll_num, h_scroll, v_scroll_num, v_scroll, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                /* Draw the sprites. */
                {
                    byte sr1, sr2, sr3, sr4, sr5;
                    int spritenum, sx, sy, color;
                    byte sn1, sn2, sp;
                    bool flipx, flipy;

                    for (offs = 0; offs < 0x0F5; /* offs += 5 */ )
                    {
                        sn1 = ss[offs++]; // offs+0
                        sn2 = ss[offs++]; // offs+1
                        sy = ss[offs++]; // offs+2
                        sx = ss[offs++]; // offs+3
                        sp = ss[offs++]; // offs+4

                        flipx = (sp & 0x20) != 0;
                        flipy = (sp & 0x40) != 0;
                        color = ((sn2 & 0xf0) >> 4);

                        if ((sp & 0xC) == 0)
                        {
                            spritenum = sn1 + ((sn2 & 0x3) << 8);

                            if (sy > 0xF0) sy = sy - 256;
                            if ((sp & 0x01) != 0) sx = sx - 256;

                            if ((sp & 0x10) != 0)
                            {
                                if ((sx > -16) || (sx < 0xF0))
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy + 16 : sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)spritenum + 1,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy + 16 : sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)spritenum + 2,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)spritenum + 3,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                            }
                            else
                            {
                                if ((sx > -8) || (sx < 0xF0))
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                            }
                        }
                        else if ((sx < 0xF0) && (sp & 0x01) == 0)
                        {
                            spritenum = sn1 * 4 + ((sn2 & (8 + 4)) >> 2) + ((sn2 & (2 + 1)) << 10);

                            if ((sp & 0x0C) == 0x0C)
                            {
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[4],
                                    (uint)spritenum,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            }
                            if ((sp & 0x0C) == 0x08)
                            {
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[4],
                                    (uint)spritenum,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[4],
                                    (uint)spritenum - 2,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy + 8,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            }
                            if ((sp & 0x0C) == 0x04)
                            {
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[4],
                                    (uint)spritenum,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[4],
                                    (uint)spritenum + 1,
                                    (uint)color,
                                    flipx, flipy,
                                    sx + 8, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            }
                        }
                    }

                    for (offs = 0; offs < 0x11D; offs += 5)
                    {
                        if ((sr[offs + 2] < 0xF0) && (sr[offs + 4] & 0x01) == 0)
                        {
                            sr1 = sr[offs];
                            sr2 = sr[offs + 1];
                            sr3 = sr[offs + 2];
                            sr4 = sr[offs + 3];
                            sr5 = sr[offs + 4];

                            sy = sr3;
                            sx = sr4;

                            flipx = (sr5 & 0x20) != 0;
                            flipy = (sr5 & 0x40) != 0;
                            color = ((sr2 & 0xf0) >> 4);

                            spritenum = sr1 + ((sr2 & 0x3) << 8);

                            if ((sr5 & 0xC) != 0)   /* half sized sprite */
                            {

                                spritenum = sr1 * 4 + ((sr2 & (8 + 4)) >> 2) + ((sr2 & (2 + 1)) << 10);

                                if ((sr5 & 0x0C) == 0x0C)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                       Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                if ((sr5 & 0x0C) == 0x08)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum - 2,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy + 8,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                if ((sr5 & 0x0C) == 0x04)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum + 1,
                                        (uint)color,
                                        flipx, flipy,
                                        sx + 8, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }

                            }
                            else
                            {
                                if ((sr5 & 0x10) != 0)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy + 16 : sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum + 1,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy + 16 : sy,
                                       Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum + 2,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum + 3,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                else
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            }
                        }
                    }

                    for (offs = 0x4F1; offs >= 0x11D; offs -= 5)
                    {
                        if ((sr[offs + 2] < 0xF0) && (sr[offs + 4] & 0x01) == 0)
                        {
                            sr1 = sr[offs];
                            sr2 = sr[offs + 1];
                            sr3 = sr[offs + 2];
                            sr4 = sr[offs + 3];
                            sr5 = sr[offs + 4];

                            sy = sr3;
                            sx = sr4;

                            flipx = (sr5 & 0x20) != 0;
                            flipy = (sr5 & 0x40) != 0;
                            color = ((sr2 & 0xf0) >> 4);

                            if ((sr[offs + 4] & 0xC) != 0)    /* half sized sprite */
                            {

                                spritenum = sr1 * 4 + ((sr2 & (8 + 4)) >> 2) + ((sr2 & (2 + 1)) << 10);

                                if ((sr5 & 0x0C) == 0x0C)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                if ((sr5 & 0x0C) == 0x08)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                       Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum - 2,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy + 8,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                if ((sr5 & 0x0C) == 0x04)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[3],
                                        (uint)spritenum + 1,
                                        (uint)color,
                                        flipx, flipy,
                                        sx + 8, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }

                            }
                            else
                            {
                                spritenum = sr1 + ((sr2 & 0x3) << 8);

                                if ((sr5 & 0x10) != 0)
                                {
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy + 16 : sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)(spritenum + 1),
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy + 16 : sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)(spritenum + 2),
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx + 16 : sx, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)(spritenum + 3),
                                        (uint)color,
                                        flipx, flipy,
                                        flipx ? sx : sx + 16, flipy ? sy : sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                                }
                                else
                                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)spritenum,
                                        (uint)color,
                                        flipx, flipy,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            }
                        }
                    }
                }
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_jackal()
        {

            ROM_START("jackal");
            ROM_REGION(0x20000, Mame.REGION_CPU1);	/* Banked 64k for 1st CPU */
            ROM_LOAD("j-v02.rom", 0x04000, 0x8000, 0x0b7e0584);
            ROM_CONTINUE(0x14000, 0x8000);
            ROM_LOAD("j-v03.rom", 0x0c000, 0x4000, 0x3e0dfb83);

            ROM_REGION(0x10000, Mame.REGION_CPU2);     /* 64k for 2nd cpu (Graphics & Sound)*/
            ROM_LOAD("631t01.bin", 0x8000, 0x8000, 0xb189af6a);

            ROM_REGION(0x80000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("631t04.bin", 0x00000, 0x20000, 0x457f42f0);
            ROM_LOAD("631t06.bin", 0x20000, 0x20000, 0x2d10e56e);
            ROM_LOAD("631t05.bin", 0x40000, 0x20000, 0x732b3fc1);
            ROM_LOAD("631t07.bin", 0x60000, 0x20000, 0x4961c397);

            ROM_REGION(0x0200, Mame.REGION_PROMS);/* color lookup tables */
            ROM_LOAD("631r08.bpr", 0x0000, 0x0100, 0x7553a172);
            ROM_LOAD("631r09.bpr", 0x0100, 0x0100, 0xa74dd86c);
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_jackal()
        {

            INPUT_PORTS_START("jackal");
            PORT_START("DSW1");
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x02, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x01, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0x0f, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x07, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0x0d, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0b, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x0a, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x09, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x20, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x50, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x10, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x70, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0xd0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xb0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0xa0, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, "Invalid");

            PORT_START("IN1");
            /* note that button 3 for player 1 and 2 are exchanged */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("DSW2");
            PORT_DIPNAME(0x03, 0x02, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x02, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x04, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x18, 0x18, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x18, "30000 150000");
            PORT_DIPSETTING(0x10, "50000 200000");
            PORT_DIPSETTING(0x08, "30000");
            PORT_DIPSETTING(0x00, "50000");
            PORT_DIPNAME(0x60, 0x60, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x60, "Easy");
            PORT_DIPSETTING(0x40, "Medium");
            PORT_DIPSETTING(0x20, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off")); ;
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("DSW3");
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x02, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x00, "Sound Mode");
            PORT_DIPSETTING(0x04, "Mono");
            PORT_DIPSETTING(0x00, "Stereo");
            PORT_DIPNAME(0x08, 0x00, "Sound Adj");
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x08, DEF_STR("Cocktail"));

            /* the rotary controls work in topgunbl only */
            PORT_START();	/* player 1 8-way rotary control - converted in rotary_0_r() */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_DIAL, 25, 10, 0, 0, (ushort)Mame.InputCodes.KEYCODE_Z, (ushort)Mame.InputCodes.KEYCODE_X, 0, 0);

            PORT_START();	/* player 2 8-way rotary control - converted in rotary_1_r() */
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_DIAL | IPF_PLAYER2, 25, 10, 0, 0, (ushort)Mame.InputCodes.KEYCODE_N, (ushort)Mame.InputCodes.KEYCODE_M, 0, 0);
            return INPUT_PORTS_END;
        }
        public driver_jackal()
        {
            drv = new machine_driver_jackal();
            year = "1986";
            name = "jackal";
            description = "Jackal (World)";
            manufacturer = "Konami";
            flags = Mame.ROT90;
            input_ports = input_ports_jackal();
            rom = rom_jackal();
            drv.HasNVRAMhandler = false;
        }
    }
}
