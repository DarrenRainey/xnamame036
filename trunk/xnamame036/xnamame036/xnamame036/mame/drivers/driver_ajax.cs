using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_ajax : Mame.GameDriver
    {
        static _BytePtr ajax_sharedram = new _BytePtr(1);

        static Mame.MemoryReadAddress[] ajax_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x01c0, ajax_ls138_f10_r ),			/* inputs + DIPSW */
	new Mame.MemoryReadAddress( 0x0800, 0x0807, konamiic.K051937_r ),					/* sprite control registers */
	new Mame.MemoryReadAddress( 0x0c00, 0x0fff, konamiic.K051960_r ),					/* sprite RAM 2128SL at J7 */
	new Mame.MemoryReadAddress( 0x1000, 0x1fff, Mame.MRA_RAM ),		/* palette */
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, ajax_sharedram_r ),			/* shared RAM with the 6809 */
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, Mame.MRA_RAM ),					/* RAM 6264L at K10*/
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, Mame.MRA_BANK2 ),					/* banked ROM */
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),					/* ROM N11 */
	new Mame.MemoryReadAddress( -1 )
};

        static Mame.MemoryWriteAddress[] ajax_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x01c0, ajax_ls138_f10_w ),			/* bankswitch + sound command + FIRQ command */
	new Mame.MemoryWriteAddress( 0x0800, 0x0807, konamiic.K051937_w ),					/* sprite control registers */
	new Mame.MemoryWriteAddress( 0x0c00, 0x0fff, konamiic.K051960_w ),					/* sprite RAM 2128SL at J7 */
	new Mame.MemoryWriteAddress( 0x1000, 0x1fff, Mame.paletteram_xBBBBBGGGGGRRRRR_swap_w, Mame.paletteram ),/* palette */
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, ajax_sharedram_w ),			/* shared RAM with the 6809 */
	new Mame.MemoryWriteAddress( 0x4000, 0x5fff, Mame.MWA_RAM ),					/* RAM 6264L at K10 */
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, Mame.MWA_ROM ),					/* banked ROM */
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),					/* ROM N11 */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.MemoryReadAddress[] ajax_readmem_2 =
{
	new Mame.MemoryReadAddress(0x0000, 0x07ff, konamiic.K051316_0_r ),		/* 051316 zoom/rotation layer */
	new Mame.MemoryReadAddress(0x1000, 0x17ff, konamiic.K051316_rom_0_r ),	/* 051316 (ROM test) */
	new Mame.MemoryReadAddress(0x2000, 0x3fff, ajax_sharedram_r ),	/* shared RAM with the 052001 */
	new Mame.MemoryReadAddress(0x4000, 0x7fff, konamiic.K052109_r ),			/* video RAM + color RAM + video registers */
	new Mame.MemoryReadAddress(0x8000, 0x9fff, Mame.MRA_BANK1 ),			/* banked ROM */
	new Mame.MemoryReadAddress(0xa000, 0xffff, Mame.MRA_ROM ),			/* ROM I16 */
	new Mame.MemoryReadAddress(-1 )
};

        static Mame.MemoryWriteAddress[] ajax_writemem_2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x07ff, konamiic.K051316_0_w ),			/* 051316 zoom/rotation layer */
	new Mame.MemoryWriteAddress( 0x0800, 0x080f, konamiic.K051316_ctrl_0_w ),		/* 051316 control registers */
	new Mame.MemoryWriteAddress( 0x1800, 0x1800, ajax_bankswitch_w_2 ),	/* bankswitch control */
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff, ajax_sharedram_w, ajax_sharedram ),/* shared RAM with the 052001 */
	new Mame.MemoryWriteAddress( 0x4000, 0x7fff, konamiic.K052109_w ),				/* video RAM + color RAM + video registers */
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, Mame.MWA_ROM ),				/* banked ROM */
	new Mame.MemoryWriteAddress( 0xa000, 0xffff, Mame.MWA_ROM ),				/* ROM I16 */
	new Mame.MemoryWriteAddress( -1 )
};

        static Mame.MemoryReadAddress[] ajax_readmem_sound =
{
	new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM ),				/* ROM F6 */
	new Mame.MemoryReadAddress(0x8000, 0x87ff, Mame.MRA_RAM ),				/* RAM 2128SL at D16 */
	new Mame.MemoryReadAddress(0xa000, 0xa00d, K007232.K007232_read_port_0_r ),	/* 007232 registers (chip 1) */
	new Mame.MemoryReadAddress(0xb000, 0xb00d, K007232.K007232_read_port_1_r ),	/* 007232 registers (chip 2) */
	new Mame.MemoryReadAddress(0xc001, 0xc001, YM2151.YM2151_status_port_0_r ),	/* YM2151 */
	new Mame.MemoryReadAddress(0xe000, 0xe000, Mame.soundlatch_r ),			/* soundlatch_r */
	new Mame.MemoryReadAddress(-1 )
};

        static Mame.MemoryWriteAddress[] ajax_writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),					/* ROM F6 */
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),					/* RAM 2128SL at D16 */
	new Mame.MemoryWriteAddress( 0x9000, 0x9000, sound_bank_w ),				/* 007232 bankswitch */
	new Mame.MemoryWriteAddress( 0xa000, 0xa00d, K007232.K007232_write_port_0_w ),		/* 007232 registers (chip 1) */
	new Mame.MemoryWriteAddress( 0xb000, 0xb00d, K007232.K007232_write_port_1_w ),		/* 007232 registers (chip 2) */
	new Mame.MemoryWriteAddress( 0xb80c, 0xb80c, k007232_extvol_w ),	/* extra volume, goes to the 007232 w/ A11 */
	                										/* selecting a different latch for the external port */
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, YM2151.YM2151_register_port_0_w ),	/* YM2151 */
	new Mame.MemoryWriteAddress( 0xc001, 0xc001, YM2151.YM2151_data_port_0_w ),		/* YM2151 */
	new Mame.MemoryWriteAddress( -1 )
};

        static void sound_bank_w(int offset, int data)
        {
            _BytePtr RAM;
            int bank_A, bank_B;

            /* banks # for the 007232 (chip 1) */
            RAM = Mame.memory_region(Mame.REGION_SOUND1);
            bank_A = 0x20000 * ((data >> 1) & 0x01);
            bank_B = 0x20000 * ((data >> 0) & 0x01);
            K007232.K007232_bankswitch(0, new _BytePtr(RAM, bank_A), new _BytePtr(RAM, bank_B));

            /* banks # for the 007232 (chip 2) */
            RAM = Mame.memory_region(Mame.REGION_SOUND2);
            bank_A = 0x20000 * ((data >> 4) & 0x03);
            bank_B = 0x20000 * ((data >> 2) & 0x03);
            K007232.K007232_bankswitch(1, new _BytePtr(RAM, bank_A), new _BytePtr(RAM, bank_B));
        }

        static void volume_callback0(int v)
        {
            K007232.K007232_set_volume(0, 0, (v >> 4) * 0x11, 0);
            K007232.K007232_set_volume(0, 1, 0, (v & 0x0f) * 0x11);
        }

        static void k007232_extvol_w(int offset, int v)
        {
            /* channel A volume (mono) */
            K007232.K007232_set_volume(1, 0, (v & 0x0f) * 0x11 / 2, (v & 0x0f) * 0x11 / 2);
        }

        static void volume_callback1(int v)
        {
            /* channel B volume/pan */
            K007232.K007232_set_volume(1, 1, (v & 0x0f) * 0x11 / 2, (v >> 4) * 0x11 / 2);
        }

        static K007232_interface k007232_interface =
        new K007232_interface(
            2,			/* number of chips */
            new int[] { Mame.REGION_SOUND1, Mame.REGION_SOUND2 },	/* memory regions */
            new int[]{ K007232.K007232_VOL(20,Mame.MIXER_PAN_CENTER,20,Mame.MIXER_PAN_CENTER),
		K007232.K007232_VOL(50,Mame.MIXER_PAN_LEFT,50,Mame.MIXER_PAN_RIGHT) },/* volume */
            new K007232_writehandler[] { volume_callback0, volume_callback1 }	/* external port callback */
        );
        static YM2151interface ym2151_interface =
new YM2151interface(
    1,
    3579545,	/* 3.58 MHz */
    new int[] { YM2151.YM3012_VOL(50, Mame.MIXER_PAN_LEFT, 50, Mame.MIXER_PAN_RIGHT) },
    null, new Mame.mem_write_handler[] { null }
);

        static int firq_enable;
        static byte ajax_priority;
        static int[] layer_colorbase = new int[3];
        static int sprite_colorbase, zoom_colorbase;

        static void ajax_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);
            int bankaddress = 0;

            /* rom select */
            if ((data & 0x80) == 0) bankaddress += 0x8000;

            /* coin counters */
            Mame.coin_counter_w(0, data & 0x20);
            Mame.coin_counter_w(1, data & 0x40);

            /* priority */
            ajax_priority = (byte)(data & 0x08);

            /* bank # (ROMS N11 and N12) */
            bankaddress += 0x10000 + (data & 0x07) * 0x2000;
            Mame.cpu_setbank(2, new _BytePtr(RAM, bankaddress));
        }
        static void ajax_lamps_w(int offset, int data)
        {
            Mame.osd_led_w(0, (data & 0x02) >> 1);	/* super weapon lamp */
            Mame.osd_led_w(1, (data & 0x04) >> 2);	/* power up lamps */
            Mame.osd_led_w(5, (data & 0x04) >> 2);	/* power up lamps */
            Mame.osd_led_w(2, (data & 0x20) >> 5);	/* start lamp */
            Mame.osd_led_w(3, (data & 0x40) >> 6);	/* game over lamps */
            Mame.osd_led_w(6, (data & 0x40) >> 6);	/* game over lamps */
            Mame.osd_led_w(4, (data & 0x80) >> 7);	/* game over lamps */
            Mame.osd_led_w(7, (data & 0x80) >> 7);	/* game over lamps */
        }
        static int ajax_ls138_f10_r(int offset)
        {
            int data = 0;

            switch ((offset & 0x01c0) >> 6)
            {
                case 0x00:	/* ??? */
                    data = (byte)Mame.rand();
                    break;
                case 0x04:	/* 2P inputs */
                    data = Mame.readinputport(5);
                    break;
                case 0x06:	/* 1P inputs + DIPSW #1 & #2 */
                    if ((offset & 0x02) != 0)
                        data = Mame.readinputport(offset & 0x01);
                    else
                        data = Mame.readinputport(3 + (offset & 0x01));
                    break;
                case 0x07:	/* DIPSW #3 */
                    data = Mame.readinputport(2);
                    break;

                default:
                    Mame.printf("%04x: (ls138_f10) read from an unknown address %02x\n", Mame.cpu_get_pc(), offset);
                    break;
            }

            return data;
        }

        static void ajax_ls138_f10_w(int offset, int data)
        {
            switch ((offset & 0x01c0) >> 6)
            {
                case 0x00:	/* NSFIRQ + AFR */
                    if (offset != 0)
                        Mame.watchdog_reset_w(0, data);
                    else
                    {
                        if (firq_enable != 0)	/* Cause interrupt on slave CPU */
                            Mame.cpu_cause_interrupt(1, Mame.cpu_m6809.M6809_INT_FIRQ);
                    }
                    break;
                case 0x01:	/* Cause interrupt on audio CPU */
                    Mame.cpu_cause_interrupt(2, Mame.cpu_Z80.Z80_IRQ_INT);
                    break;
                case 0x02:	/* Sound command number */
                    Mame.soundlatch_w(offset, data);
                    break;
                case 0x03:	/* Bankswitch + coin counters + priority*/
                    ajax_bankswitch_w(0, data);
                    break;
                case 0x05:	/* Lamps + Joystick vibration + Control panel quaking */
                    ajax_lamps_w(0, data);
                    break;

                default:
                    Mame.printf("%04x: (ls138_f10) write %02x to an unknown address %02x\n", Mame.cpu_get_pc(), data, offset);
                    break;
            }
        }
        static int ajax_sharedram_r(int offset)
        {
            return ajax_sharedram[offset];
        }
        static void ajax_sharedram_w(int offset, int data)
        {
            ajax_sharedram[offset] = (byte)data;
        }
        static void ajax_bankswitch_w_2(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU2);
            int bankaddress;

            /* enable char ROM reading through the video RAM */
            konamiic.K052109_set_RMRD_line((data & 0x40) != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);

            /* bit 5 enables 051316 wraparound */
            konamiic.K051316_wraparound_enable(0, data & 0x20);

            /* FIRQ control */
            firq_enable = data & 0x10;

            /* bank # (ROMS G16 and I16) */
            bankaddress = 0x10000 + (data & 0x0f) * 0x2000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }

        static int ajax_interrupt()
        {
            if (konamiic.K051960_is_IRQ_enabled()!=0)
                return Mame.cpu_konami.KONAMI_INT_IRQ;
            else
                return Mame.ignore_interrupt();
        }


        class machine_driver_ajax : Mame.MachineDriver
        {
            public machine_driver_ajax()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_KONAMI, 3000000, ajax_readmem, ajax_writemem, null, null, ajax_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 3000000, ajax_readmem_2, ajax_writemem_2, null, null, Mame.ignore_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3579545, ajax_readmem_sound, ajax_writemem_sound, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 10;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(14 * 8, (64 - 14) * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = null;
                total_colors = 2048;
                color_table_len = 2048;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_K007232, k007232_interface));
            }
            public override void init_machine()
            {
                firq_enable = 1;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
               //none
            }
            static void zoom_callback(ref int code, ref int color)
            {
                code |= ((color & 0x07) << 8);
                color = zoom_colorbase + ((color & 0x08) >> 3);
            }
            static void sprite_callback(ref int code, ref int color, ref int priority)
            {
                /* priority bits:
                   4 over zoom (0 = have priority)
                   5 over B    (0 = have priority) - is this used?
                   6 over A    (1 = have priority)
                */
                priority = (color & 0x70) >> 4;
                color = sprite_colorbase + (color & 0x0f);
            }
            static void tile_callback(int layer, int bank, ref int code, ref int color)
            {
                code |= ((color & 0x0f) << 8) | (bank << 12);
                color = layer_colorbase[layer] + ((color & 0xf0) >> 4);
            }


            public override int vh_start()
            {
                layer_colorbase[0] = 64;
                layer_colorbase[1] = 0;
                layer_colorbase[2] = 32;
                sprite_colorbase = 16;
                zoom_colorbase = 6;	/* == 48 since it's 7-bit graphics */
                if (konamiic.K052109_vh_start(Mame.REGION_GFX1, konamiic.NORMAL_PLANE_ORDER, tile_callback)!=0)
                    return 1;
                if (konamiic.K051960_vh_start(Mame.REGION_GFX2, konamiic.NORMAL_PLANE_ORDER, sprite_callback)!=0)
                {
                    konamiic.K052109_vh_stop();
                    return 1;
                }
                if (konamiic.K051316_vh_start_0(Mame.REGION_GFX3, 7, zoom_callback)!=0)
                {
                    konamiic.K052109_vh_stop();
                    konamiic.K051960_vh_stop();
                    return 1;
                }

                return 0;
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                konamiic.K052109_tilemap_update();
                konamiic.K051316_tilemap_update_0();

                Mame.palette_init_used_colors();
                konamiic.K051960_mark_sprites_colors();
                /* set back pen for the zoom layer */
                Mame.palette_used_colors[(zoom_colorbase + 0) * 128] = Mame.PALETTE_COLOR_TRANSPARENT;
                Mame.palette_used_colors[(zoom_colorbase + 1) * 128] = Mame.PALETTE_COLOR_TRANSPARENT;
                if (Mame.palette_recalc()!=null)
                    Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

                Mame.tilemap_render(Mame.ALL_TILEMAPS);

                /* sprite priority bits:
                   0 over zoom (0 = have priority)
                   1 over B    (0 = have priority)
                   2 over A    (1 = have priority)
                */
                if (ajax_priority!=0)
                {
                    /* basic layer order is B, zoom, A, F */

                    /* pri = 2 have priority over zoom, not over A and B - is this used? */
                    /* pri = 3 have priority over nothing - is this used? */
                    //		K051960_sprites_draw(bitmap,2,3);
                    konamiic.K052109_tilemap_draw(bitmap, 2, Mame.TILEMAP_IGNORE_TRANSPARENCY);
                    /* pri = 1 have priority over B, not over zoom and A - is this used? */
                    //		K051960_sprites_draw(bitmap,1,1);
                    konamiic.K051316_zoom_draw_0(bitmap);
                    /* pri = 0 have priority over zoom and B, not over A */
                    /* the game seems to just use pri 0. */
                    konamiic.K051960_sprites_draw(bitmap, 0, 0);
                    konamiic.K052109_tilemap_draw(bitmap, 1, 0);
                    /* pri = 4 have priority over zoom, A and B */
                    /* pri = 5 have priority over A and B, not over zoom - OPPOSITE TO BASIC ORDER! (stage 6 boss) */
                    konamiic.K051960_sprites_draw(bitmap, 4, 5);
                    /* pri = 6 have priority over zoom and A, not over B - is this used? */
                    /* pri = 7 have priority over A, not over zoom and B - is this used? */
                    //		K051960_sprites_draw(bitmap,5,7);
                    konamiic.K052109_tilemap_draw(bitmap, 0, 0);
                }
                else
                {
                    /* basic layer order is B, A, zoom, F */

                    /* pri = 2 have priority over zoom, not over A and B - is this used? */
                    /* pri = 3 have priority over nothing - is this used? */
                    //		K051960_sprites_draw(bitmap,2,3);
                    konamiic.K052109_tilemap_draw(bitmap, 2, Mame.TILEMAP_IGNORE_TRANSPARENCY);
                    /* pri = 0 have priority over zoom and B, not over A - OPPOSITE TO BASIC ORDER! */
                    /* pri = 1 have priority over B, not over zoom and A */
                    /* the game seems to just use pri 0. */
                    konamiic.K051960_sprites_draw(bitmap, 0, 1);
                    konamiic.K052109_tilemap_draw(bitmap, 1, 0);
                    konamiic.K051316_zoom_draw_0(bitmap);
                    /* pri = 4 have priority over zoom, A and B */
                    konamiic.K051960_sprites_draw(bitmap, 4, 4);
                    /* pri = 5 have priority over A and B, not over zoom - is this used? */
                    /* pri = 6 have priority over zoom and A, not over B - is this used? */
                    /* pri = 7 have priority over A, not over zoom and B - is this used? */
                    //		K051960_sprites_draw(bitmap,5,7);
                    konamiic.K052109_tilemap_draw(bitmap, 0, 0);
                }
            }
        }
        public override void driver_init()
        {
            konamiic.konami_rom_deinterleave_2(Mame.REGION_GFX1);
            konamiic.konami_rom_deinterleave_2(Mame.REGION_GFX2);
        }
        Mame.RomModule[] rom_ajax()
        {
            ROM_START("ajax");
            ROM_REGION(0x28000, Mame.REGION_CPU1);	/* 052001 code */
            ROM_LOAD("m01.n11", 0x10000, 0x08000, 0x4a64e53a);	/* banked ROM */
            ROM_CONTINUE(0x08000, 0x08000);		/* fixed ROM */
            ROM_LOAD("l02.n12", 0x18000, 0x10000, 0xad7d592b);	/* banked ROM */

            ROM_REGION(0x22000, Mame.REGION_CPU2);/* 64k + 72k for banked ROMs */
            ROM_LOAD("l05.i16", 0x20000, 0x02000, 0xed64fbb2);	/* banked ROM */
            ROM_CONTINUE(0x0a000, 0x06000);	/* fixed ROM */
            ROM_LOAD("f04.g16", 0x10000, 0x10000, 0xe0e4ec9c);/* banked ROM */

            ROM_REGION(0x10000, Mame.REGION_CPU3);/* 64k for the SOUND CPU */
            ROM_LOAD("h03.f16", 0x00000, 0x08000, 0x2ffd2afc);

            ROM_REGION(0x080000, Mame.REGION_GFX1);/* graphics (addressable by the main CPU) */
            ROM_LOAD("770c13", 0x000000, 0x040000, 0xb859ca4e);/* characters (N22) */
            ROM_LOAD("770c12", 0x040000, 0x040000, 0x50d14b72);	/* characters (K22) */

            ROM_REGION(0x100000, Mame.REGION_GFX2);	/* graphics (addressable by the main CPU) */
            ROM_LOAD("770c09", 0x000000, 0x080000, 0x1ab4a7ff);	/* sprites (N4) */
            ROM_LOAD("770c08", 0x080000, 0x080000, 0xa8e80586);	/* sprites (K4) */

            ROM_REGION(0x080000, Mame.REGION_GFX3);	/* graphics (addressable by the main CPU) */
            ROM_LOAD("770c06", 0x000000, 0x040000, 0xd0c592ee);	/* zoom/rotate (F4) */
            ROM_LOAD("770c07", 0x040000, 0x040000, 0x0b399fb1);	/* zoom/rotate (H4) */

            ROM_REGION(0x0200, Mame.REGION_PROMS);
            ROM_LOAD("63s241.j11", 0x0000, 0x0200, 0x9bdd719f);	/* priority encoder (not used) */

            ROM_REGION(0x040000, Mame.REGION_SOUND1);	/* 007232 data (chip 1) */
            ROM_LOAD("770c10", 0x000000, 0x040000, 0x7fac825f);

            ROM_REGION(0x080000, Mame.REGION_SOUND2);	/* 007232 data (chip 2) */
            ROM_LOAD("770c11", 0x000000, 0x080000, 0x299a615a);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ajax()
        {
            INPUT_PORTS_START("ajax");
            PORT_START("DSW #1");
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
            //	PORT_DIPSETTING(    0x00, "Coin Slot 2 Invalidity" )

            PORT_START();	/* DSW #2 */
            PORT_DIPNAME(0x03, 0x02, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x02, "3");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x18, 0x10, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x18, "30000 150000");
            PORT_DIPSETTING(0x10, "50000 200000");
            PORT_DIPSETTING(0x08, "30000");
            PORT_DIPSETTING(0x00, "50000");
            PORT_DIPNAME(0x60, 0x40, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x60, "Easy");
            PORT_DIPSETTING(0x40, "Normal");
            PORT_DIPSETTING(0x20, "Difficult");
            PORT_DIPSETTING(0x00, "Very difficult");
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START();	/* DSW #3 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, "Upright Controls");
            PORT_DIPSETTING(0x02, "Single");
            PORT_DIPSETTING(0x00, "Dual");
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, "Control in 3D Stages");
            PORT_DIPSETTING(0x08, "Normal");
            PORT_DIPSETTING(0x00, "Inverted");
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* COINSW & START */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);	/* service */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* PLAYER 1 INPUTS */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* PLAYER 2 INPUTS */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);
            return INPUT_PORTS_END;
        }
        public driver_ajax()
        {
            drv = new machine_driver_ajax();
            year = "1987";
            name = "ajax";
            description = "Ajax";
            manufacturer = "Konami";
            flags = Mame.ROT90;
            input_ports = input_ports_ajax();
            rom = rom_ajax();
            drv.HasNVRAMhandler = false;
        }
    }
}
