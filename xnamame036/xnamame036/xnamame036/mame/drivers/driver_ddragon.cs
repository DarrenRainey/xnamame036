using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_ddragonb : Mame.GameDriver
    {
        public static int dd_sub_cpu_busy;
        public static int sprite_irq, sound_irq, ym_irq;
        public static _BytePtr dd_videoram = new _BytePtr(1);
        public static int dd_scrollx_hi, dd_scrolly_hi;
        public static _BytePtr dd_scrollx_lo = new _BytePtr(1);
        public static _BytePtr dd_scrolly_lo = new _BytePtr(1);
        public static _BytePtr dd_spriteram = new _BytePtr(1);
        public static int dd2_video;

        public static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x2000, 0x2fff, dd_spriteram_r ),
	new Mame.MemoryReadAddress( 0x3000, 0x37ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x3800, 0x3800, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x3801, 0x3801, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x3802, 0x3802, port4_r ),
	new Mame.MemoryReadAddress( 0x3803, 0x3803, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x3804, 0x3804, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x3805, 0x3fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x4000, 0x7fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1000, 0x11ff, Mame.paletteram_xxxxBBBBGGGGRRRR_split1_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x1200, 0x13ff, Mame.paletteram_xxxxBBBBGGGGRRRR_split2_w, Mame.paletteram_2 ),
	new Mame.MemoryWriteAddress( 0x1400, 0x17ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1800, 0x1fff, Mame.MWA_RAM, Generic.videoram ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2fff, dd_spriteram_w, dd_spriteram ),
	new Mame.MemoryWriteAddress( 0x3000, 0x37ff, dd_background_w, dd_videoram ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3807, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x3808, 0x3808, dd_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x3809, 0x3809, Mame.MWA_RAM, dd_scrollx_lo ),
	new Mame.MemoryWriteAddress( 0x380a, 0x380a, Mame.MWA_RAM, dd_scrolly_lo ),
	new Mame.MemoryWriteAddress( 0x380b, 0x380b, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x380c, 0x380d, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x380e, 0x380e, cpu_sound_command_w ),
	new Mame.MemoryWriteAddress( 0x380f, 0x380f, dd_forcedIRQ_w ),
	new Mame.MemoryWriteAddress( 0x3810, 0x3fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.MemoryReadAddress[] sub_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8fff, dd_spriteram_r ),
	new Mame.MemoryReadAddress( 0xc000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sub_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8fff, dd_spriteram_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x1000, 0x1000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x1800, 0x1800, dd_adpcm_status_r ),
	new Mame.MemoryReadAddress( 0x2800, 0x2801, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2800, YM2151.YM2151_register_port_0_w ),
	new Mame.MemoryWriteAddress( 0x2801, 0x2801, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3807, dd_adpcm_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout char_layout =
new Mame.GfxLayout(
    8, 8, /* 8*8 chars */
    1024, /* 'num' characters */
    4, /* 4 bits per pixel */
    new uint[] { 0, 2, 4, 6 }, /* plane offset */
    new uint[] { 1, 0, 65, 64, 129, 128, 193, 192 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    32 * 8 /* every char takes 32 consecutive bytes */
);
        public static Mame.GfxLayout tile_layout =
new Mame.GfxLayout(
    16, 16, /* 16x16 chars */
    2048, /* 'num' characters */
    4, /* 4 bits per pixel */
    new uint[] { 0x20000 * 8 + 0, 0x20000 * 8 + 4, 0, 4 }, /* plane offset */
    new uint[]{ 3, 2, 1, 0, 16*8+3, 16*8+2, 16*8+1, 16*8+0, 
	          32*8+3,32*8+2 ,32*8+1 ,32*8+0 ,48*8+3 ,48*8+2 ,48*8+1 ,48*8+0 },
    new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8, 
	          8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
    64 * 8 /* every char takes 64 consecutive bytes */
);
        static Mame.GfxLayout sprite_layout =
new Mame.GfxLayout(
16, 16, /* 16x16 chars */
2048 * 2, /* 'num' characters */
4, /* 4 bits per pixel */
new uint[] { 0x40000 * 8 + 0, 0x40000 * 8 + 4, 0, 4 }, /* plane offset */
new uint[]{ 3, 2, 1, 0, 16*8+3, 16*8+2, 16*8+1, 16*8+0, 
	          32*8+3,32*8+2 ,32*8+1 ,32*8+0 ,48*8+3 ,48*8+2 ,48*8+1 ,48*8+0 },
new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8, 
	          8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
64 * 8 /* every char takes 64 consecutive bytes */
);
        static Mame.GfxDecodeInfo[] gfxdecodeinfo = 
        {
            new Mame.GfxDecodeInfo(Mame.REGION_GFX1,0,char_layout,0,8),
            new Mame.GfxDecodeInfo(Mame.REGION_GFX2,0,sprite_layout,128,8),
            new Mame.GfxDecodeInfo(Mame.REGION_GFX3,0,tile_layout,256,8)
        };


        public static YM2151interface ym2151_interface =
        new YM2151interface(
            1,			/* 1 chip */
            3579545,	/* ??? */
            new int[] { YM2151.YM3012_VOL(60, Mame.MIXER_PAN_LEFT, 60, Mame.MIXER_PAN_RIGHT) },
            new YM2151irqhandler[] { dd_irq_handler }, new YM2151writehandler[] { null }
        );

        static ADPCMinterface adpcm_interface =
        new ADPCMinterface(
            2,			/* 2 channels */
            8000,       /* 8000Hz playback */
            Mame.REGION_SOUND1,	/* memory region 4 */
            null,			/* init function */
            new int[] { 50, 50 }
        );

        static OKIM6295interface okim6295_interface =
        new OKIM6295interface(
            1,              /* 1 chip */
            new int[] { 8000 },           /* frequency (Hz) */
            new int[] { Mame.REGION_SOUND1 },  /* memory region */
            new int[] { 15 }
        );

        public static int dd_interrupt()
        {
            Mame.cpu_set_irq_line(0, 1, Mame.HOLD_LINE); /* hold the FIRQ line */
            Mame.cpu_set_nmi_line(0, Mame.PULSE_LINE); /* pulse the NMI line */
            return Mame.cpu_m6809.M6809_INT_NONE;
        }
        static int port4_r(int offset)
        {
            int port = Mame.readinputport(4);

            return port | dd_sub_cpu_busy;
        }
        public static void dd_spriteram_w(int offset, int data)
        {
            if (Mame.cpu_getactivecpu() == 1 && offset == 0)
                dd_sub_cpu_busy = 0x10;

            dd_spriteram[offset] = (byte)data;
        }
        public static void dd_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            dd_scrolly_hi = ((data & 0x02) << 7);
            dd_scrollx_hi = ((data & 0x01) << 8);

            if ((data & 0x10) == 0x10)
            {
                dd_sub_cpu_busy = 0x00;
            }
            else if (dd_sub_cpu_busy == 0x00)
                Mame.cpu_cause_interrupt(1, sprite_irq);

            Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000 + (0x4000 * ((data >> 5) & 7))));
        }
        public static void cpu_sound_command_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.cpu_cause_interrupt(2, sound_irq);
        }
        static int[] start = new int[2], end = new int[2];
        static void dd_adpcm_w(int offset, int data)
        {
            int chip = offset & 1;

            offset >>= 1;

            switch (offset)
            {
                case 3:
                    break;

                case 2:
                    start[chip] = data & 0x7f;
                    break;

                case 1:
                    end[chip] = data & 0x7f;
                    break;

                case 0:
                    ADPCM.ADPCM_play(chip, 0x10000 * chip + start[chip] * 0x200, (end[chip] - start[chip]) * 0x400);
                    break;
            }
        }

        static int dd_adpcm_status_r(int offset)
        {
            return (ADPCM.ADPCM_playing(0) + (ADPCM.ADPCM_playing(1) << 1));
        }

        public static void dd_forcedIRQ_w(int offset, int data)
        {
            Mame.cpu_cause_interrupt(0, Mame.cpu_m6809.M6809_INT_IRQ);
        }
        public static void dd_background_w(int offset, int val)
        {
            if (dd_videoram[offset] != val)
            {
                dd_videoram[offset] = (byte)val;
                Generic.dirtybuffer[offset / 2] = true;
            }
        }
        static void dd_irq_handler(int irq)
        {
            Mame.cpu_set_irq_line(2, ym_irq, irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        public static int dd_spriteram_r(int offset)
        {
            return dd_spriteram[offset];
        }

        public static int ddragon_vh_start()
        {
            Generic.dirtybuffer = new bool[0x400];
            if (Generic.dirtybuffer != null)
            {
                for (int i = 0; i < 0x400; i++) Generic.dirtybuffer[i] = true;

                Generic.tmpbitmap = Mame.osd_new_bitmap(
                        Mame.Machine.drv.screen_width * 2,
                        Mame.Machine.drv.screen_height * 2,
                        Mame.Machine.scrbitmap.depth);

                if (Generic.tmpbitmap != null) return 0;

                Generic.dirtybuffer = null;
            }

            return 1;
        }
        public static void ddragon_vh_stop()
        {
            Mame.osd_free_bitmap(Generic.tmpbitmap);
            Generic.dirtybuffer = null;
        }
        public static void ddragon_vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            if (Mame.palette_recalc() != null)
                for (int i = 0; i < 0x400; i++) Generic.dirtybuffer[i] = true;

            dd_draw_background(bitmap);
            dd_draw_sprites(bitmap);
            dd_draw_foreground(bitmap);
        }
        static void dd_draw_foreground(Mame.osd_bitmap bitmap)
        {
            Mame.GfxElement gfx = Mame.Machine.gfx[0];
            _BytePtr source = new _BytePtr(Generic.videoram);

            for (int sy = 0; sy < 256; sy += 8)
            {
                for (int sx = 0; sx < 256; sx += 8)
                {
                    int attributes = source[0];
                    int tile_number = source[1] + 256 * (attributes & 7);
                    int color = (attributes >> 5) & 0x7;

                    if (tile_number != 0)
                    {
                        Mame.drawgfx(bitmap, gfx, (uint)tile_number,
                        (uint)color,
                        false, false, /* no flip */
                        sx, sy,
                        null, /* no need to clip */
                        Mame.TRANSPARENCY_PEN, 0);
                    }
                    source.offset += 2;
                }
            }
        }
        static void dd_draw_sprites(Mame.osd_bitmap bitmap)
        {
            Mame.rectangle clip = Mame.Machine.drv.visible_area;
            Mame.GfxElement gfx = Mame.Machine.gfx[1];

            _BytePtr src = new _BytePtr(dd_spriteram, 0x800);
            int i;

            for (i = 0; i < (64 * 5); i += 5)
            {
                int attr = src[i + 1];
                if ((attr & 0x80) != 0)
                { /* visible */
                    int sx = 240 - src[i + 4] + ((attr & 2) << 7);
                    int sy = 240 - src[i + 0] + ((attr & 1) << 8);
                    int size = (attr & 0x30) >> 4;
                    bool flipx = (attr & 8) != 0;
                    bool flipy = (attr & 4) != 0;

                    uint which;
                    uint color;

                    if (dd2_video != 0)
                    {
                        color = ((uint)src[i + 2] >> 5);
                        which = (uint)src[i + 3] + (((uint)src[i + 2] & 0x1f) << 8);
                    }
                    else
                    {
                        color = ((uint)src[i + 2] >> 4) & 0x07;
                        which = (uint)src[i + 3] + (((uint)src[i + 2] & 0x0f) << 8);
                    }

                    switch (size)
                    {
                        case 0: /* normal */
                            Mame.drawgfx(bitmap, gfx, (which + 0), color, flipx, flipy, sx, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            break;

                        case 1: /* double y */
                            Mame.drawgfx(bitmap, gfx, (which + 0), color, flipx, flipy, sx, sy - 16, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            Mame.drawgfx(bitmap, gfx, (which + 1), color, flipx, flipy, sx, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            break;

                        case 2: /* double x */
                            Mame.drawgfx(bitmap, gfx, (which + 0), color, flipx, flipy, sx - 16, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            Mame.drawgfx(bitmap, gfx, (which + 2), color, flipx, flipy, sx, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            break;

                        case 3:
                            Mame.drawgfx(bitmap, gfx, (which + 0), color, flipx, flipy, sx - 16, sy - 16, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            Mame.drawgfx(bitmap, gfx, (which + 1), color, flipx, flipy, sx - 16, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            Mame.drawgfx(bitmap, gfx, (which + 2), color, flipx, flipy, sx, sy - 16, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            Mame.drawgfx(bitmap, gfx, (which + 3), color, flipx, flipy, sx, sy, clip, Mame.TRANSPARENCY_PEN, 0); ;
                            break;
                    }
                }
            }
        }

        static void dd_draw_background(Mame.osd_bitmap bitmap)
        {
            Mame.GfxElement gfx = Mame.Machine.gfx[2];

            int scrollx = -dd_scrollx_hi - (dd_scrollx_lo[0]);
            int scrolly = -dd_scrolly_hi - (dd_scrolly_lo[0]);

            for (int offset = 0; offset < 0x400; offset++)
            {
                int attributes = dd_videoram[offset * 2];
                int color = (attributes >> 3) & 0x7;
                if (Generic.dirtybuffer[offset])
                {
                    int tile_number = dd_videoram[offset * 2 + 1] + ((attributes & 7) << 8);
                    int xflip = attributes & 0x40;
                    int yflip = attributes & 0x80;
                    int sx = 16 * (((offset >> 8) & 1) * 16 + (offset & 0xff) % 16);
                    int sy = 16 * (((offset >> 9) & 1) * 16 + (offset & 0xff) / 16);

                    /* CALB ????
                                  if( sx<0 || sx>=512 || sy<0 || sy>=512 ) ExitToShell();*/

                    Mame.drawgfx(Generic.tmpbitmap, gfx,
                        (uint)tile_number,
                        (uint)color,
                        xflip != 0, yflip != 0,
                        sx, sy,
                        null, Mame.TRANSPARENCY_NONE, 0);

                    Generic.dirtybuffer[offset] = false;
                }
            }

            Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap,
                    1, new int[] { scrollx }, 1, new int[] { scrolly },
                    Mame.Machine.drv.visible_area,
                    Mame.TRANSPARENCY_NONE, 0);
        }
        class machine_driver_ddragonb : Mame.MachineDriver
        {
            public machine_driver_ddragonb()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD6309, 3579545, readmem, writemem, null, null, dd_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD6309, 12000000 / 3, sub_readmem, sub_writemem, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD6309 | Mame.CPU_AUDIO_CPU, 3579545, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;

                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_ddragonb.gfxdecodeinfo;
                total_colors = 384;
                color_table_len = 384;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_ADPCM, adpcm_interface));
            }
            public override void init_machine()
            {
                sprite_irq = Mame.cpu_m6809.M6809_INT_NMI;
                sound_irq = Mame.cpu_m6809.M6809_INT_IRQ;
                ym_irq = 1;//M6809_INT_FIRQ;
                dd2_video = 0;
                dd_sub_cpu_busy = 0x10;
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
                return ddragon_vh_start();
            }
            public override void vh_stop()
            {
                ddragon_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                ddragon_vh_update(bitmap, full_refresh);
            }

            public override void vh_eof_callback()
            {//nothing
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_ddragonb()
        {

            ROM_START("ddragon");
            ROM_REGION(0x28000, Mame.REGION_CPU1);	/* 64k for code + bankswitched memory */
            ROM_LOAD("ic26", 0x08000, 0x08000, 0x42045dfd);
            ROM_LOAD("21j-2-3", 0x10000, 0x08000, 0x5779705e); /* banked at 0x4000-0x8000 */
            ROM_LOAD("21j-3", 0x18000, 0x08000, 0x3bdea613); /* banked at 0x4000-0x8000 */
            ROM_LOAD("ic23", 0x20000, 0x08000, 0x728f87b9); /* banked at 0x4000-0x8000 */

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* sprite cpu */
            /* missing mcu code */
            ROM_LOAD("ic38", 0xc000, 0x4000, 0xf5232d03);

            ROM_REGION(0x10000, Mame.REGION_CPU3); /* audio cpu */
            ROM_LOAD("21j-0-1", 0x08000, 0x08000, 0x9efa95bb);

            ROM_REGION(0x08000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("21j-5", 0x00000, 0x08000, 0x7a8b8db4);/* 0,1,2,3 */ /* text */

            ROM_REGION(0x80000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("21j-a", 0x00000, 0x10000, 0x574face3);/* 0,1 */ /* sprites */
            ROM_LOAD("21j-b", 0x10000, 0x10000, 0x40507a76);/* 0,1 */ /* sprites */
            ROM_LOAD("21j-c", 0x20000, 0x10000, 0xbb0bc76f);/* 0,1 */ /* sprites */
            ROM_LOAD("21j-d", 0x30000, 0x10000, 0xcb4f231b);/* 0,1 */ /* sprites */
            ROM_LOAD("21j-e", 0x40000, 0x10000, 0xa0a0c261);/* 2,3 */ /* sprites */
            ROM_LOAD("21j-f", 0x50000, 0x10000, 0x6ba152f6);/* 2,3 */ /* sprites */
            ROM_LOAD("21j-g", 0x60000, 0x10000, 0x3220a0b6);/* 2,3 */ /* sprites */
            ROM_LOAD("21j-h", 0x70000, 0x10000, 0x65c7517d);/* 2,3 */ /* sprites */

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("21j-8", 0x00000, 0x10000, 0x7c435887); /* 0,1 */ /* tiles */
            ROM_LOAD("21j-9", 0x10000, 0x10000, 0xc6640aed); /* 0,1 */ /* tiles */
            ROM_LOAD("21j-i", 0x20000, 0x10000, 0x5effb0a0); /* 2,3 */ /* tiles */
            ROM_LOAD("21j-j", 0x30000, 0x10000, 0x5fb42e7c); /* 2,3 */ /* tiles */

            ROM_REGION(0x20000, Mame.REGION_SOUND1); /* adpcm samples */
            ROM_LOAD("21j-6", 0x00000, 0x10000, 0x34755de3);
            ROM_LOAD("21j-7", 0x10000, 0x10000, 0x904de6f8);

            ROM_REGION(0x0300, Mame.REGION_PROMS);
            ROM_LOAD("21j-k-0", 0x0000, 0x0100, 0xfdb130a9);
            ROM_LOAD("21j-l-0", 0x0100, 0x0200, 0x46339529);
            return ROM_END;
        }
        public static void COMMON_PORT4()/* bit 0x10 is sprite CPU busy signal */
        {
            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_VBLANK);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
        }
        public static void COMMON_INPUT_PORTS()
        {
            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_START();
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
            PORT_DIPNAME(0x40, 0x40, "Screen Orientation?");
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
        }
        Mame.InputPortTiny[] input_ports_ddragonb()
        {
            INPUT_PORTS_START("dd1");
            COMMON_INPUT_PORTS();

            PORT_START();      /* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Normal");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Very Hard");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x04, DEF_STR("On"));
            PORT_BIT(0x08, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x10, "20k");
            PORT_DIPSETTING(0x00, "40k");
            PORT_DIPSETTING(0x30, "30k and every 60k");
            PORT_DIPSETTING(0x20, "40k and every 80k");
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR("Lives"));
            PORT_DIPSETTING(0xc0, "2");
            PORT_DIPSETTING(0x80, "3");
            PORT_DIPSETTING(0x40, "4");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "Infinite", IP_KEY_NONE, IP_JOY_NONE);

            COMMON_PORT4();
            return INPUT_PORTS_END;
        }
        public driver_ddragonb()
        {
            drv = new machine_driver_ddragonb();
            year = "1987";
            name = "ddragonb";
            description = "Double Dragon (bootleg)";
            manufacturer = "bootleg";
            flags = Mame.ROT0;
            input_ports = input_ports_ddragonb();
            rom = rom_ddragonb();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_ddragon2 : Mame.GameDriver
    {
        static Mame.MemoryWriteAddress[] dd2_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x17ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1800, 0x1fff, Mame.MWA_RAM, Generic.videoram ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2fff, driver_ddragonb.dd_spriteram_w, driver_ddragonb.dd_spriteram ),
	new Mame.MemoryWriteAddress( 0x3000, 0x37ff, driver_ddragonb.dd_background_w, driver_ddragonb.dd_videoram ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3807,  Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x3808, 0x3808, driver_ddragonb.dd_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x3809, 0x3809,  Mame.MWA_RAM, driver_ddragonb.dd_scrollx_lo ),
	new Mame.MemoryWriteAddress( 0x380a, 0x380a,  Mame.MWA_RAM, driver_ddragonb.dd_scrolly_lo ),
	new Mame.MemoryWriteAddress( 0x380b, 0x380b,  Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x380c, 0x380d,  Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x380e, 0x380e, driver_ddragonb.cpu_sound_command_w ),
	new Mame.MemoryWriteAddress( 0x380f, 0x380f, driver_ddragonb.dd_forcedIRQ_w ),
	new Mame.MemoryWriteAddress( 0x3810, 0x3bff,  Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3dff,  Mame.paletteram_xxxxBBBBGGGGRRRR_split1_w,Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x3e00, 0x3fff,  Mame.paletteram_xxxxBBBBGGGGRRRR_split2_w,Mame.paletteram_2 ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff,  Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout dd2_char_layout = 
	new Mame.GfxLayout( 
		8,8, /* 8*8 chars */ 
		2048, /* 'num' characters */ 
		4, /* 4 bits per pixel */ 
		new uint[]{ 0, 2, 4, 6 }, /* plane offset */ 
new uint[]		{ 1, 0, 65, 64, 129, 128, 193, 192 }, 
		new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },	
		32*8 /* every char takes 32 consecutive bytes */ 
	);

	static Mame.GfxLayout dd2_sprite_layout = 
	new Mame.GfxLayout( 
		16,16, /* 16x16 chars */ 
		2048*3, /* 'num' characters */ 
		4, /* 4 bits per pixel */
        new uint[] { 0x60000 * 8 + 0, 0x60000 * 8 + 4, 0, 4 }, /* plane offset */ 
		new uint[]{ 3, 2, 1, 0, 16*8+3, 16*8+2, 16*8+1, 16*8+0, 
	          32*8+3,32*8+2 ,32*8+1 ,32*8+0 ,48*8+3 ,48*8+2 ,48*8+1 ,48*8+0 }, 
		new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8, 
	          8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 }, 
		64*8 /* every char takes 64 consecutive bytes */ 
	);
        static Mame.GfxDecodeInfo[] dd2_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, dd2_char_layout,	    0, 8 ),	/* 8x8 chars */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, dd2_sprite_layout, 128, 8 ),	/* 16x16 sprites */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, driver_ddragonb.tile_layout,       256, 8 ),	/* 16x16 background tiles */
};
        
static Mame.MemoryReadAddress[] dd2_sub_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, driver_ddragonb.dd_spriteram_r ),
	new Mame.MemoryReadAddress( 0xd000, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress[] dd2_sub_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, driver_ddragonb.dd_spriteram_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xffff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

static Mame.MemoryReadAddress[] dd2_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8801, 0x8801, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x9800, 0x9800, okim6295.OKIM6295_status_0_r ),
	new Mame.MemoryReadAddress( 0xA000, 0xA000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress[] dd2_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8800, 0x8800, YM2151.YM2151_register_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8801, 0x8801, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress( 0x9800, 0x9800, okim6295.OKIM6295_data_0_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static OKIM6295interface okim6295_interface =
new OKIM6295interface(
	1,              /* 1 chip */
	new int[]{ 8000 },           /* frequency (Hz) */
new int[]	{ Mame.REGION_SOUND1 },  /* memory region */
	new int[]{ 15 }
);
        class machine_driver_ddragon2 : Mame.MachineDriver
        {
            public machine_driver_ddragon2()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_HD6309, 3579545, driver_ddragonb.readmem, dd2_writemem, null, null, driver_ddragonb.dd_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 12000000 / 3, dd2_sub_readmem, dd2_sub_writemem, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3579545, dd2_sound_readmem, dd2_sound_writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = dd2_gfxdecodeinfo;
                total_colors = 384;
                color_table_len = 384;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, driver_ddragonb.ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_OKIM6295, okim6295_interface));
            }
            public override void init_machine()
            {
                driver_ddragonb.sprite_irq = Mame.cpu_Z80.Z80_NMI_INT;
                driver_ddragonb.sound_irq = Mame.cpu_Z80.Z80_NMI_INT;
                driver_ddragonb.ym_irq = 0;//-1000;
                driver_ddragonb.dd2_video = 1;
                driver_ddragonb.dd_sub_cpu_busy = 0x10;
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
                return driver_ddragonb.ddragon_vh_start();
            }
            public override void vh_stop()
            {
                driver_ddragonb.ddragon_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_ddragonb.ddragon_vh_update(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        Mame.InputPortTiny[] input_ports_ddragon2()
        {
            INPUT_PORTS_START("dd2");
            driver_ddragonb.COMMON_INPUT_PORTS();

            PORT_START();      /* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Normal");
            PORT_DIPSETTING(0x01, "Medium");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x04, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x08, "Hurricane Kick");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Normal");
            PORT_DIPNAME(0x30, 0x30, "Timer");
            PORT_DIPSETTING(0x00, "60");
            PORT_DIPSETTING(0x10, "65");
            PORT_DIPSETTING(0x30, "70");
            PORT_DIPSETTING(0x20, "80");
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR("Lives"));
            PORT_DIPSETTING(0xc0, "1");
            PORT_DIPSETTING(0x80, "2");
            PORT_DIPSETTING(0x40, "3");
            PORT_DIPSETTING(0x00, "4");

            driver_ddragonb.COMMON_PORT4();
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_ddragon2()
        {
            ROM_START("ddragon2");
            ROM_REGION(0x28000, Mame.REGION_CPU1);	/* region#0: 64k for code */
            ROM_LOAD("26a9-04.bin", 0x08000, 0x8000, 0xf2cfc649);
            ROM_LOAD("26aa-03.bin", 0x10000, 0x8000, 0x44dd5d4b);
            ROM_LOAD("26ab-0.bin", 0x18000, 0x8000, 0x49ddddcd);
            ROM_LOAD("26ac-02.bin", 0x20000, 0x8000, 0x097eaf26);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* region#2: sprite CPU 64kb (Upper 16kb = 0) */
            ROM_LOAD("26ae-0.bin", 0x00000, 0x10000, 0xea437867);

            ROM_REGION(0x10000, Mame.REGION_CPU3); /* region#3: music CPU, 64kb */
            ROM_LOAD("26ad-0.bin", 0x00000, 0x8000, 0x75e36cd6);

            ROM_REGION(0x10000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("26a8-0.bin", 0x00000, 0x10000, 0x3ad1049c); /* 0,1,2,3 */ /* text */

            ROM_REGION(0xc0000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("26j0-0.bin", 0x00000, 0x20000, 0xdb309c84);/* 0,1 */ /* sprites */
            ROM_LOAD("26j1-0.bin", 0x20000, 0x20000, 0xc3081e0c);/* 0,1 */ /* sprites */
            ROM_LOAD("26af-0.bin", 0x40000, 0x20000, 0x3a615aad);/* 0,1 */ /* sprites */
            ROM_LOAD("26j2-0.bin", 0x60000, 0x20000, 0x589564ae);/* 2,3 */ /* sprites */
            ROM_LOAD("26j3-0.bin", 0x80000, 0x20000, 0xdaf040d6);/* 2,3 */ /* sprites */
            ROM_LOAD("26a10-0.bin", 0xa0000, 0x20000, 0x6d16d889);/* 2,3 */ /* sprites */

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("26j4-0.bin", 0x00000, 0x20000, 0xa8c93e76); /* 0,1 */ /* tiles */
            ROM_LOAD("26j5-0.bin", 0x20000, 0x20000, 0xee555237); /* 2,3 */ /* tiles */

            ROM_REGION(0x40000, Mame.REGION_SOUND1);/* region#4: adpcm */
            ROM_LOAD("26j6-0.bin", 0x00000, 0x20000, 0xa84b2a29);
            ROM_LOAD("26j7-0.bin", 0x20000, 0x20000, 0xbc6a48d5);
            return ROM_END;
        }
        public override void driver_init()
        {
           //nothing
        }
        public driver_ddragon2()
        {
            drv = new machine_driver_ddragon2();
            year = "1988";
            name = "ddragon2";
            description = "Double Dragon II - The Revenge";
            manufacturer = "Tecnos";
            flags = Mame.ROT0;
            input_ports = input_ports_ddragon2();
            rom = rom_ddragon2();
            drv.HasNVRAMhandler = false;
        }
    }
}
