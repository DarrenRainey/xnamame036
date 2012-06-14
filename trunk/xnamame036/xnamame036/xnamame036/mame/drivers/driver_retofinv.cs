using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_retofinv : Mame.GameDriver
    {
        static int[] retofinv_videoram_size = new int[1];
        static _BytePtr retofinv_sprite_ram1 = new _BytePtr(1);
        static _BytePtr retofinv_sprite_ram2 = new _BytePtr(1);
        static _BytePtr retofinv_sprite_ram3 = new _BytePtr(1);
        static _BytePtr retofinv_fg_videoram = new _BytePtr(1);
        static _BytePtr retofinv_bg_videoram = new _BytePtr(1);
        static _BytePtr retofinv_fg_colorram = new _BytePtr(1);
        static _BytePtr retofinv_bg_colorram = new _BytePtr(1);
        static _BytePtr retofinv_fg_char_bank = new _BytePtr(1);
        static _BytePtr retofinv_bg_char_bank = new _BytePtr(1);

        static byte cpu0_me000 = 0, cpu0_me800_last = 0, cpu2_m6000 = 0;

        static byte flipscreen = 0;
        static bool[] bg_dirtybuffer;
        static uint bg_bank; /* last background bank active, 0 or 1 */
        static Mame.osd_bitmap bitmap_bg;
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x5fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x7b00, 0x7b00, protection_2_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, retofinv_fg_videoram_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x87ff, retofinv_fg_colorram_r ),
	new Mame.MemoryReadAddress( 0x8800, 0x9fff, retofinv_shared_ram_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xa3ff, retofinv_bg_videoram_r ),
	new Mame.MemoryReadAddress( 0xa400, 0xa7ff, retofinv_bg_colorram_r ),
	new Mame.MemoryReadAddress( 0xc800, 0xc800, Mame.MRA_NOP ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xc001, 0xc001, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0xc002, 0xc002, protection_2_r ),
	new Mame.MemoryReadAddress( 0xc003, 0xc003, protection_3_r ),
	new Mame.MemoryReadAddress( 0xc004, 0xc004, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xc005, 0xc005, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xc006, 0xc006, Mame.input_port_5_r ),
	new Mame.MemoryReadAddress( 0xc007, 0xc007, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xe000, retofinv_protection_r ),
	new Mame.MemoryReadAddress( 0xf800, 0xf800, cpu0_mf800_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x7fff, 0x7fff, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, retofinv_fg_videoram_w, retofinv_fg_videoram, retofinv_videoram_size ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, retofinv_fg_colorram_w, retofinv_fg_colorram ),
	new Mame.MemoryWriteAddress( 0x8800, 0x9fff, retofinv_shared_ram_w ),
	new Mame.MemoryWriteAddress( 0x8f00, 0x8f7f, Mame.MWA_RAM, retofinv_sprite_ram1 ),	/* covered by the above, */
	new Mame.MemoryWriteAddress( 0x9700, 0x977f, Mame.MWA_RAM, retofinv_sprite_ram2 ),	/* here only to */
	new Mame.MemoryWriteAddress( 0x9f00, 0x9f7f, Mame.MWA_RAM, retofinv_sprite_ram3 ),	/* initialize the pointers */
	new Mame.MemoryWriteAddress( 0xa000, 0xa3ff, retofinv_bg_videoram_w, retofinv_bg_videoram ),
	new Mame.MemoryWriteAddress( 0xa400, 0xa7ff, retofinv_bg_colorram_w, retofinv_bg_colorram ),
	new Mame.MemoryWriteAddress( 0xb800, 0xb800, retofinv_flip_screen_w ),
	new Mame.MemoryWriteAddress( 0xb801, 0xb801, Mame.MWA_RAM, retofinv_fg_char_bank ),
	new Mame.MemoryWriteAddress( 0xb802, 0xb802, Mame.MWA_RAM, retofinv_bg_char_bank ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xc801, 0xc801, reset_cpu2_w ),
	new Mame.MemoryWriteAddress( 0xc802, 0xc802, reset_cpu1_w ),
	new Mame.MemoryWriteAddress( 0xc803, 0xc803, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xc805, 0xc805, cpu1_halt_w ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd800, soundcommand_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xe800, 0xe800, retofinv_protection_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_sub =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, retofinv_fg_videoram_r ),
	new Mame.MemoryReadAddress( 0x8400, 0x87ff, retofinv_fg_colorram_r ),
	new Mame.MemoryReadAddress( 0x8800, 0x9fff, retofinv_shared_ram_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xa3ff, retofinv_bg_videoram_r ),
	new Mame.MemoryReadAddress( 0xa400, 0xa7ff, retofinv_bg_colorram_r ),
	new Mame.MemoryReadAddress( 0xc804, 0xc804, Mame.MRA_NOP ),
	new Mame.MemoryReadAddress( 0xe000, 0xe000, retofinv_protection_r ),
	new Mame.MemoryReadAddress( 0xe800, 0xe800, Mame.MRA_NOP ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_sub =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, retofinv_fg_videoram_w ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, retofinv_fg_colorram_w ),
	new Mame.MemoryWriteAddress( 0x8800, 0x9fff, retofinv_shared_ram_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa3ff, retofinv_bg_videoram_w ),
	new Mame.MemoryWriteAddress( 0xa400, 0xa7ff, retofinv_bg_colorram_w ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] readmem_sound =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x2000, 0x27ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x4000, 0x4000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xe000, Mame.MRA_NOP ),  		/* Rom version ? */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x2000, 0x27ff, Mame.MWA_RAM),
	new Mame.MemoryWriteAddress( 0x6000, 0x6000, cpu2_m6000_w ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, SN76496.SN76496_0_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, SN76496.SN76496_1_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static void retofinv_init_machine()
        {
            cpu0_me800_last = 0;
            cpu2_m6000 = 0;
        }

        static int retofinv_shared_ram_r(int offset)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);
            return RAM[0x8800 + offset];
        }

        static void retofinv_shared_ram_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);
            RAM[0x8800 + offset] = (byte)data;
        }

        static void retofinv_protection_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            if ((cpu0_me800_last & 0x80) != 0)
            {
                cpu0_me000 = (byte)data;
                cpu0_me800_last = 0;
                return;
            }
            else if (data < 0x10)
            {
                switch (data)
                {
                    case 0x01:
                        cpu0_me000 = (byte)(((cpu0_me000 >> 3) & 3) + 1);
                        break;
                    case 0x02:
                        cpu0_me000 = (byte)(cpu0_me000 & 3);
                        break;
                    default:
                        cpu0_me000 = cpu0_me000;
                        break;
                }
            }
            else if (data > 0x0f)
            {
                switch (data)
                {
                    case 0x30:
                        cpu0_me000 = cpu0_me000;
                        break;
                    case 0x40:
                        cpu0_me000 = RAM[0x9800];
                        break;
                    case 0x41:
                        cpu0_me000 = RAM[0x9801];
                        break;
                    case 0x42:
                        cpu0_me000 = RAM[0x9802];
                        break;
                    default:
                        cpu0_me000 = 0x3b;
                        break;
                }
            }
            cpu0_me800_last = (byte)data;
        }

        static int retofinv_protection_r(int offset)
        {
            return cpu0_me000;
        }

        static void reset_cpu2_w(int offset, int data)
        {
            if (data != 0)
                Mame.cpu_set_reset_line(2, Mame.PULSE_LINE);
        }

        static void reset_cpu1_w(int offset, int data)
        {
            if (data != 0)
                Mame.cpu_set_reset_line(1, Mame.PULSE_LINE);
        }

        static void cpu1_halt_w(int offset, int data)
        {
            Mame.cpu_set_halt_line(1, data != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }

        static int protection_2_r(int offset)
        {
            return 0;
        }

        static int protection_3_r(int offset)
        {
            return 0x30;
        }

        static void cpu2_m6000_w(int offset, int data)
        {
            cpu2_m6000 = (byte)data;
        }

        static int cpu0_mf800_r(int offset)
        {
            return cpu2_m6000;
        }

        static void soundcommand_w(int offset, int data)
        {
            Mame.soundlatch_w(0, data);
            Mame.cpu_set_irq_line(2, 0, Mame.HOLD_LINE);
        }

        static void retofinv_flip_screen_w(int offset, int data)
        {
            flipscreen = (byte)data;
            for (int i = 0; i < retofinv_videoram_size[0]; i++) bg_dirtybuffer[i] = true;//memset(bg_dirtybuffer,1,retofinv_videoram_size);
            Mame.fillbitmap(bitmap_bg, Mame.Machine.pens[0], null);
        }

        static int retofinv_bg_videoram_r(int offset)
        {
            return retofinv_bg_videoram[offset];
        }

        static int retofinv_fg_videoram_r(int offset)
        {
            return retofinv_fg_videoram[offset];
        }

        static int retofinv_bg_colorram_r(int offset)
        {
            return retofinv_bg_colorram[offset];
        }

        static int retofinv_fg_colorram_r(int offset)
        {
            return retofinv_fg_colorram[offset];
        }

        static void retofinv_bg_videoram_w(int offset, int data)
        {
            if (retofinv_bg_videoram[offset] != data)
            {
                bg_dirtybuffer[offset] = true;
                retofinv_bg_videoram[offset] = (byte)data;
            }
        }

        static void retofinv_fg_videoram_w(int offset, int data)
        {
            if (retofinv_fg_videoram[offset] != data)
                retofinv_fg_videoram[offset] = (byte)data;
        }

        static void retofinv_bg_colorram_w(int offset, int data)
        {
            if (retofinv_bg_colorram[offset] != data)
            {
                bg_dirtybuffer[offset] = true;
                retofinv_bg_colorram[offset] = (byte)data;
            }
        }

        static void retofinv_fg_colorram_w(int offset, int data)
        {
            if (retofinv_fg_colorram[offset] != data)
                retofinv_fg_colorram[offset] = (byte)data;
        }
        static Mame.SN76496interface sn76496_interface =
new Mame.SN76496interface(
    2,		/* 2 chips */
    new int[] { 3072000, 3072000 },	/* ??? */
    new int[] { 80, 80 }
);

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            1,	/* 1 bits per pixel */
            new uint[] { 0 },
        new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },		/* x bit */
            new uint[] { 56, 48, 40, 32, 24, 16, 8, 0 },	/* y bit */
            8 * 8 	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout bglayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 0x2000 * 8 + 4, 0x2000 * 8, 4 },
            new uint[] { 8 * 8 + 3, 8 * 8 + 2, 8 * 8 + 1, 8 * 8 + 0, 3, 2, 1, 0 },
            new uint[] { 7 * 8, 6 * 8, 5 * 8, 4 * 8, 3 * 8, 2 * 8, 1 * 8, 0 * 8 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 characters */
            256,	/* 256 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 0x4000 * 8 + 4, 0x4000 * 8, 4 },
            new uint[]{ 24*8+3, 24*8+2, 24*8+1, 24*8+0, 16*8+3, 16*8+2, 16*8+1, 16*8+0,
	  8*8+3, 8*8+2, 8*8+1, 8*8+0, 3, 2, 1, 0 },
            new uint[]{ 39*8, 38*8, 37*8, 36*8, 35*8, 34*8, 33*8, 32*8,
	  7*8, 6*8, 5*8, 4*8, 3*8, 2*8, 1*8, 0*8 },
            64 * 8	/* every char takes 64 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,             0, 256 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, bglayout,           256*2,  64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout, 64*16+256*2,  64 ),
};


        class machine_driver_retofinv : Mame.MachineDriver
        {
            public machine_driver_retofinv()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem_sub, writemem_sub, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem_sound, writemem_sound, null, null, Mame.nmi_interrupt, 2));

                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_retofinv.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 256 * 2 + 64 * 16 + 64 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SN76496, sn76496_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            static uint adj_data(uint v)
            {
                /* reverse bits 4-7 */
                return (v & 0xF) |
                        ((v & 0x80) >> 3) | ((v & 0x40) >> 1) | ((v & 0x20) << 1) | ((v & 0x10) << 3);
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint cpi = 0;
                int pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0, bit1, bit2, bit3;

                    bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    bit0 = (color_prom[cpi + 1 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 1 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 1 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 1 * Mame.Machine.drv.total_colors] >> 3) & 0x01;

                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi + 0 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 0 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 0 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 0 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                cpi += 2 * Mame.Machine.drv.total_colors;
                /* color_prom now points to the beginning of the lookup table */

                /* foreground colors */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                {
                    if ((i % 2) != 0)
                        COLOR(colortable, 0, i, i / 2);
                    else
                        COLOR(colortable, 0, i, 0);
                }

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                    COLOR(colortable, 2, i, (int)adj_data(color_prom[cpi++]));

                /* background bank 0 (gameplay) */
                /* background bank 1 (title screen) */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable, 1, i, (int)adj_data(color_prom[(uint)(cpi + i)]));
            }
            public override int vh_start()
            {
                bg_dirtybuffer = new bool[retofinv_videoram_size[0]];
                bitmap_bg = Mame.osd_create_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

                for (int i = 0; i < retofinv_videoram_size[0]; i++) bg_dirtybuffer[i] = true;
                bg_bank = 0;
                return 0;
            }
            public override void vh_stop()
            {

                bg_dirtybuffer = null;
                Mame.osd_free_bitmap(bitmap_bg);
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                retofinv_draw_background(bitmap);
                retofinv_render_sprites(bitmap);
                retofinv_draw_foreground(bitmap);
            }

            void retofinv_render_sprites(Mame.osd_bitmap bitmap)
            {
                int offs, sx, sy, flipx, flipy, tile, palette, size;
                int tileofs0, tileofs1, tileofs2, tileofs3;

                for (offs = 0; offs < 127; offs += 2)
                {
                    {
                        sx = 311 - (((retofinv_sprite_ram2[offs + 1] & 127) << 1) +
                                  ((retofinv_sprite_ram3[offs + 1] & 128) >> 7) +
                                  ((retofinv_sprite_ram2[offs + 1] & 128) << 1));

                        sy = ((retofinv_sprite_ram2[offs] & 127) << 1) +
                                  ((retofinv_sprite_ram3[offs] & 128) >> 7) +
                                  ((retofinv_sprite_ram2[offs] & 128) << 1);

                        tile = retofinv_sprite_ram1[offs];
                        size = retofinv_sprite_ram3[offs];
                        palette = retofinv_sprite_ram1[offs + 1] & 0x3f;

                        flipx = 0;
                        flipy = 0;
                        tileofs0 = 0;
                        tileofs1 = 1;
                        tileofs2 = 2;
                        tileofs3 = 3;

                        if (flipscreen != 0)
                        {
                            tileofs0 = 2;
                            tileofs2 = 0;
                            tileofs1 = 3;
                            tileofs3 = 1;
                            flipx = flipy = 1;
                        }

                        if ((size & 12) == 0)
                        {
                            /* Patch for disappearing invadres' missile,
                                     could it be Z80 bug ? */
                            if (tile == 0x98) tile--;

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)tile,
                                        (uint)palette,
                                        flipx != 0, flipy != 0,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        if ((size & 4) != 0)
                        {
                            if ((size & 8) != 0 && (flipscreen) != 0) sx -= 16;
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)(tile + tileofs0),
                                        (uint)palette,
                                        flipx != 0, flipy != 0,
                                        sx, sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)(tile + tileofs2),
                                        (uint)palette,
                                        flipx != 0, flipy != 0,
                                        sx, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        if ((size & 8) != 0)
                        {
                            if (flipscreen != 0) sx += 32;
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)(tile + tileofs1),
                                        (uint)palette,
                                        flipx != 0, flipy != 0,
                                        sx - 16, sy + 16,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)(tile + tileofs3),
                                        (uint)palette,
                                        flipx != 0, flipy != 0,
                                        sx - 16, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
            }


            void retofinv_draw_background(Mame.osd_bitmap bitmap)
            {
                int x, y, offs;
                int sx, sy, tile, palette;
                bool bg_dirtybank;

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */

                /* if bank differ redraw all */
                bg_dirtybank = ((retofinv_bg_char_bank[0] & 1) != bg_bank);

                /* save active bank */
                bg_bank = (uint)(retofinv_bg_char_bank[0] & 1);

                for (y = 31; y >= 0; y--)
                {
                    for (x = 31; x >= 0; x--)
                    {
                        offs = y * 32 + x;

                        if (bg_dirtybank || bg_dirtybuffer[offs])
                        {
                            sx = 31 - x;
                            sy = 31 - y;

                            if (flipscreen != 0)
                            {
                                sx = 31 - sx;
                                sy = 31 - sy;
                            }

                            bg_dirtybuffer[offs] = false;
                            tile = (int)(retofinv_bg_videoram[offs] + 256 * bg_bank);
                            palette = retofinv_bg_colorram[offs] & 0x3f;

                            Mame.drawgfx(bitmap_bg, Mame.Machine.gfx[1],
                                    (uint)tile,
                                    (uint)palette,
                                    flipscreen != 0, flipscreen != 0,
                                    8 * sx + 16, 8 * sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }

                Mame.copybitmap(bitmap, bitmap_bg, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
            }


            void retofinv_draw_foreground(Mame.osd_bitmap bitmap)
            {
                int x, y, offs;
                int sx, sy, tile, palette, flipx, flipy;

                for (x = 0; x < 32; x++)
                {
                    for (y = 30; y <= 31; y++)
                    {
                        offs = y * 32 + x;

                        sx = ((62 - y) + 3) << 3;
                        sy = (31 - x) << 3;

                        flipx = flipy = 0;

                        if (flipscreen != 0)
                        {
                            sx = 280 - sx;
                            sy = 248 - sy;
                            flipx = flipy = 1;
                        }

                        tile = retofinv_fg_videoram[offs] + (retofinv_fg_char_bank[0] * 256);
                        palette = retofinv_fg_colorram[offs];

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                     (uint)tile,
                                      (uint)palette,
                                      flipx != 0, flipy != 0,
                                      sx, sy,
                                      Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                for (x = 29; x >= 2; x--)
                {
                    for (y = 31; y >= 0; y--)
                    {
                        offs = x * 32 + y;
                        sy = ((31 - x) << 3);
                        sx = ((33 - y)) << 3;

                        flipx = flipy = 0;

                        if (flipscreen != 0)
                        {
                            sx = 280 - sx;
                            sy = 248 - sy;
                            flipx = flipy = 1;
                        }

                        tile = retofinv_fg_videoram[offs] + (retofinv_fg_char_bank[0] * 256);
                        palette = retofinv_fg_colorram[offs];

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                      (uint)tile,
                                      (uint)palette,
                                      flipx != 0, flipy != 0,
                                      sx, sy,
                                      Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }

                for (x = 0; x < 32; x++)
                {
                    for (y = 1; y >= 0; y--)
                    {
                        offs = y * 32 + x;
                        sx = (1 - y) << 3;
                        sy = (31 - x) << 3;

                        flipx = flipy = 0;

                        if (flipscreen != 0)
                        {
                            sx = 280 - sx;
                            sy = 248 - sy;
                            flipx = flipy = 1;
                        }

                        tile = retofinv_fg_videoram[offs] + (retofinv_fg_char_bank[0] * 256);
                        palette = retofinv_fg_colorram[offs];

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                      (uint)tile,
                                      (uint)palette,
                                      flipx != 0, flipy != 0,
                                      sx, sy,
                                      Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
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
            //nothing
        }
        Mame.RomModule[] rom_retofinv()
        {

            ROM_START("retofinv");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("ic70.rom", 0x0000, 0x2000, 0xeae7459d);
            ROM_LOAD("ic71.rom", 0x2000, 0x2000, 0x72895e37);
            ROM_LOAD("ic72.rom", 0x4000, 0x2000, 0x505dd20b);

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for code */
            ROM_LOAD("ic62.rom", 0x0000, 0x2000, 0xd2899cc1);

            ROM_REGION(0x10000, Mame.REGION_CPU3);	/* 64k for sound cpu */
            ROM_LOAD("ic17.rom", 0x0000, 0x2000, 0x9025abea);

            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ic61.rom", 0x0000, 0x2000, 0x4e3f501c);

            ROM_REGION(0x04000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ic55.rom", 0x0000, 0x2000, 0xef7f8651);
            ROM_LOAD("ic56.rom", 0x2000, 0x2000, 0x03b40905);

            ROM_REGION(0x08000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ic8.rom", 0x0000, 0x2000, 0x6afdeec8);
            ROM_LOAD("ic9.rom", 0x2000, 0x2000, 0xd3dc9da3);
            ROM_LOAD("ic10.rom", 0x4000, 0x2000, 0xd10b2eed);
            ROM_LOAD("ic11.rom", 0x6000, 0x2000, 0x00ca6b3d);

            ROM_REGION(0x0b00, Mame.REGION_PROMS);
            ROM_LOAD("74s287.p6", 0x0000, 0x0100, 0x50030af0);	/* palette blue bits   */
            ROM_LOAD("74s287.o6", 0x0100, 0x0100, 0xe8f34e11);	/* palette green bits */
            ROM_LOAD("74s287.q5", 0x0200, 0x0100, 0xe9643b8b);	/* palette red bits  */
            ROM_LOAD("82s191n", 0x0300, 0x0800, 0x93c891e3);	/* lookup table */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_retofinv()
        {
            INPUT_PORTS_START("retofinv");
            PORT_START();     /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();     /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);

            PORT_START();     /* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);

            PORT_START();     /* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x03, "30k, 80k & every 80k");
            PORT_DIPSETTING(0x02, "30k, 80k");
            PORT_DIPSETTING(0x01, "30k");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Free Play"));
            PORT_DIPSETTING(0x04, DEF_STR("No"));
            PORT_DIPSETTING(0x00, DEF_STR("Yes"));
            PORT_DIPNAME(0x18, 0x08, DEF_STR("Lives"));
            PORT_DIPSETTING(0x18, "1");
            PORT_DIPSETTING(0x10, "2");
            PORT_DIPSETTING(0x08, "3");
            PORT_DIPSETTING(0x00, "4");
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x80, DEF_STR("Cocktail"));

            PORT_START();      /* DSW3 modified by Shingo Suzuki 1999/11/03 */
            PORT_BITX(0x01, 0x01, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Push Start to Skip Stage", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x02, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x04, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x08, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x10, "Coin Per Play Display");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x10, DEF_STR("Yes"));
            PORT_DIPNAME(0x20, 0x20, "Year Display");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x20, DEF_STR("Yes"));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x80, "A and B");
            PORT_DIPSETTING(0x00, "A only");

            PORT_START();     /* DSW2 */
            PORT_DIPNAME(0x0f, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x0f, DEF_STR("9C_1C"));
            PORT_DIPSETTING(0x0e, DEF_STR("8C_1C"));
            PORT_DIPSETTING(0x0d, DEF_STR("7C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0x0b, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x0a, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x09, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_8C"));
            PORT_DIPNAME(0xf0, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0xf0, DEF_STR("9C_1C"));
            PORT_DIPSETTING(0xe0, DEF_STR("8C_1C"));
            PORT_DIPSETTING(0xd0, DEF_STR("7C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0xb0, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x90, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_8C"));
            return INPUT_PORTS_END;
        }
        public driver_retofinv()
        {
            drv = new machine_driver_retofinv();
            year = "1985";
            name = "retofinv";
            description = "Return of the Invaders";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT270;
            input_ports = input_ports_retofinv();
            rom = rom_retofinv();
            drv.HasNVRAMhandler = false;
        }
    }
}
