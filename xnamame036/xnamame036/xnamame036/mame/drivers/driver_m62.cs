using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_m62 : Mame.GameDriver
    {
        public static Mame.GfxLayout tilelayout_1024 = new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
    1024,	/* NUM characters */
    3,	/* 3 bits per pixel */
    new uint[] { 2 * 1024 * 8 * 8, 1024 * 8 * 8, 0 },
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every char takes 8 consecutive bytes */
            );
        public static Mame.GfxLayout tilelayout_2048 = new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
    2048,	/* NUM characters */
    3,	/* 3 bits per pixel */
    new uint[] { 2 * 2048 * 8 * 8, 2048 * 8 * 8, 0 },
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every char takes 8 consecutive bytes */
            );
        public static Mame.GfxLayout spritelayout_256 =
            new Mame.GfxLayout(
                16, 16,	/* 16*16 sprites */
    256,	/* NUM sprites */
    3,	/* 3 bits per pixel */
    new uint[] { 2 * 256 * 32 * 8, 256 * 32 * 8, 0 },
    new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,                                                  
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
    new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,                                  
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
    32 * 8	/* every sprite takes 32 consecutive bytes */
                );
        public static Mame.GfxLayout spritelayout_512 =
     new Mame.GfxLayout(
         16, 16,	/* 16*16 sprites */
512,	/* NUM sprites */
3,	/* 3 bits per pixel */
new uint[] { 2 * 512 * 32 * 8, 512 * 32 * 8, 0 },
new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,                                                  
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,                                  
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
32 * 8	/* every sprite takes 32 consecutive bytes */
         );
        public static Mame.GfxLayout spritelayout_1024 =
new Mame.GfxLayout(
 16, 16,	/* 16*16 sprites */
1024,	/* NUM sprites */
3,	/* 3 bits per pixel */
new uint[] { 2 * 1024 * 32 * 8, 1024 * 32 * 8, 0 },
new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,                                                  
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,                                  
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
32 * 8	/* every sprite takes 32 consecutive bytes */
 );
        public static int flipscreen;
        public static new _BytePtr sprite_height_prom = new _BytePtr(1);
        static int kidniki_background_bank;
        public static int irem_background_hscroll;
        public static int irem_background_vscroll;
        static int kidniki_text_vscroll;
        static int spelunk2_palbank;

        static _BytePtr irem_textram = new _BytePtr(1);
        int irem_textram_size;


        static Mame.rectangle kungfum_spritevisiblearea =
        new Mame.rectangle(
            16 * 8, (64 - 16) * 8 - 1,
            10 * 8, 32 * 8 - 1
        );
        static Mame.rectangle kungfum_flipspritevisiblearea =
        new Mame.rectangle(
            16 * 8, (64 - 16) * 8 - 1,
            0 * 8, 22 * 8 - 1
        );

        public static void IN0_PORT()
        {
            /* Start 1 & 2 also restarts and freezes the game with stop mode on 
               and are used in test mode to enter and esc the various tests */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            /* service coin must be active for 19 frames to be consistently recognized */
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 19);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);
        }
        public static void IN1_PORT()
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN); /* probably unused */
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN); /* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
        }
        public static void IN2_PORT()
        {
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN); /* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
        }
        public static void COINAGE_DSW()
        {
            /* TODO: support the different settings which happen in Coin Mode 2 */
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coinage")); /* mapped on coin mode 1 */
            PORT_DIPSETTING(0x90, DEF_STR("7C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("6C_1C"));
            PORT_DIPSETTING(0xb0, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xd0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_8C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            /* setting 0x80 give 1 Coin/1 Credit */
        }

        public static void COINAGE2_DSW()
        {
            /* TODO: support the different settings which happen in Coin Mode 2 */
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coinage"));/* mapped on coin mode 1 */
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
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
        }
        public override void driver_init()
        {
        }
        public static void irem_background_hscroll_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                    irem_background_hscroll = (irem_background_hscroll & 0xff00) | data;
                    break;

                case 1:
                    irem_background_hscroll = (irem_background_hscroll & 0xff) | (data << 8);
                    break;
            }
        }
        public static void irem_flipscreen_w(int offset, int data)
        {
            /* screen flip is handled both by software and hardware */
            data ^= ~Mame.readinputport(4) & 1;

            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }

            Mame.coin_counter_w(0, data & 2);
            Mame.coin_counter_w(1, data & 4);
        }
        public static void irem_vh_convert_color_prom(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
        {
            uint pi = 0, cpi = 0;
            for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
            {
                int bit0, bit1, bit2, bit3;

                /* red component */
                bit0 = (color_prom[cpi] >> 0) & 0x01;
                bit1 = (color_prom[cpi] >> 1) & 0x01;
                bit2 = (color_prom[cpi] >> 2) & 0x01;
                bit3 = (color_prom[cpi] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* green component */
                bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* blue component */
                bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                cpi++;
            }

            cpi += 2 * Mame.Machine.drv.total_colors;
            /* color_prom now points to the beginning of the sprite height table */

            sprite_height_prom = new _BytePtr(color_prom, (int)cpi);	/* we'll need this at run time */
        }
        public static void draw_priority_sprites(Mame.osd_bitmap bitmap, int prioritylayer)
        {
            /* sprites must be drawn in this order to get correct priority */
            for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
            {
                int i, incr, code, col, sx, sy;
                bool flipx, flipy;

                if (prioritylayer == 0 || (prioritylayer != 0 && (Generic.spriteram[offs] & 0x10) != 0))
                {
                    code = Generic.spriteram[offs + 4] + ((Generic.spriteram[offs + 5] & 0x07) << 8);
                    col = Generic.spriteram[offs + 0] & 0x0f;
                    sx = 256 * (Generic.spriteram[offs + 7] & 1) + Generic.spriteram[offs + 6];
                    sy = 256 + 128 - 15 - (256 * (Generic.spriteram[offs + 3] & 1) + Generic.spriteram[offs + 2]);
                    flipx = (Generic.spriteram[offs + 5] & 0x40) != 0;
                    flipy = (Generic.spriteram[offs + 5] & 0x80) != 0;

                    i = sprite_height_prom[(code >> 5) & 0x1f];
                    if (i == 1)	/* double height */
                    {
                        code &= ~1;
                        sy -= 16;
                    }
                    else if (i == 2)	/* quadruple height */
                    {
                        i = 3;
                        code &= ~3;
                        sy -= 3 * 16;
                    }

                    if (flipscreen != 0)
                    {
                        sx = 496 - sx;
                        sy = 242 - i * 16 - sy;	/* sprites are slightly misplaced by the hardware */
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    if (flipy)
                    {
                        incr = -1;
                        code += i;
                    }
                    else incr = 1;

                    do
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)(code + i * incr),
                                (uint)col,
                                flipx, flipy,
                                sx, sy + 16 * i,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);

                        i--;
                    } while (i >= 0);
                }
            }
        }
        public static void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            ldrun_draw_background(bitmap, 0);
            draw_priority_sprites(bitmap, 0);
            ldrun_draw_background(bitmap, 1);
            draw_priority_sprites(bitmap, 1);
        }
        public static void ldrun_draw_background(Mame.osd_bitmap bitmap, int prioritylayer)
        {
            /* for every character in the Video RAM, check if it has been modified */
            /* since last time and update it accordingly. */
            for (int offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
            {
                if ((Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1]) && !(prioritylayer == 0 && (Generic.videoram[offs + 1] & 0x04) != 0))
                {
                    Generic.dirtybuffer[offs] = false;
                    Generic.dirtybuffer[offs + 1] = false;

                    int sx = (offs / 2) % 64;
                    int sy = (offs / 2) / 64;
                    bool flipx = (Generic.videoram[offs + 1] & 0x20) != 0;

                    if (flipscreen != 0)
                    {
                        sx = 63 - sx;
                        sy = 31 - sy;
                        flipx = !flipx;
                    }

                    Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                            (uint)(Generic.videoram[offs] + ((Generic.videoram[offs + 1] & 0xc0) << 2)),
                            (uint)(Generic.videoram[offs + 1] & 0x1f),
                            flipx, flipscreen != 0,
                            8 * sx, 8 * sy,
                            null, Mame.TRANSPARENCY_NONE, 0);
                }
            }


            {
                int scrolly;	/* ldrun3 only */

                if (flipscreen != 0)
                    scrolly = irem_background_vscroll;
                else
                    scrolly = -irem_background_vscroll;

                if (prioritylayer != 0)
                {
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.Machine.pens.read16(0));
                }
                else
                {
                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }
        }
    }
    class driver_ldrun : driver_m62
    {

        static Mame.GfxDecodeInfo[] ldrun_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, tilelayout_1024,      0, 32 ),	/* use colors   0-255 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, spritelayout_256,   256, 32 ),	/* use colors 256-511 */
};

        static Mame.MemoryReadAddress[] ldrun_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xd000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] ldrun_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        public static Mame.IOReadPort[] ldrun_readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r),   /* coin */
	new Mame.IOReadPort( 0x01, 0x01, Mame.input_port_1_r),   /* player 1 control */
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r),   /* player 2 control */
	new Mame.IOReadPort( 0x03, 0x03, Mame.input_port_3_r),   /* DSW 1 */
	new Mame.IOReadPort( 0x04, 0x04, Mame.input_port_4_r),   /* DSW 2 */
	new Mame.IOReadPort( -1 )	/* end of table */
};
        static Mame.IOWritePort[] ldrun_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, irem.irem_sound_cmd_w ),
	new Mame.IOWritePort( 0x01, 0x01, driver_m62.irem_flipscreen_w ),	/* + coin counters */
	new Mame.IOWritePort( -1 )	/* end of table */
};

        class machine_driver_ldrun : Mame.MachineDriver
        {
            public machine_driver_ldrun()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, ldrun_readmem, ldrun_writemem, ldrun_readport, ldrun_writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 55;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = ldrun_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));

            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                irem_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                driver_m62.irem_background_hscroll = 0;
                driver_m62.irem_background_vscroll = 0;
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_m62.vh_update(bitmap, full_refresh);
            }
        }
        public override void driver_init()
        {

        }
        Mame.RomModule[] rom_ldrun()
        {
            ROM_START("ldrun");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("lr-a-4e", 0x0000, 0x2000, 0x5d7e2a4d);
            ROM_LOAD("lr-a-4d", 0x2000, 0x2000, 0x96f20473);
            ROM_LOAD("lr-a-4b", 0x4000, 0x2000, 0xb041c4a9);
            ROM_LOAD("lr-a-4a", 0x6000, 0x2000, 0x645e42aa);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU (6803) */
            ROM_LOAD("lr-a-3f", 0xc000, 0x2000, 0x7a96accd);
            ROM_LOAD("lr-a-3h", 0xe000, 0x2000, 0x3f7f3939);

            ROM_REGION(0x6000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr-e-2d", 0x0000, 0x2000, 0x24f9b58d);	/* characters */
            ROM_LOAD("lr-e-2j", 0x2000, 0x2000, 0x43175e08);
            ROM_LOAD("lr-e-2f", 0x4000, 0x2000, 0xe0317124);

            ROM_REGION(0x6000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr-b-4k", 0x0000, 0x2000, 0x8141403e);/* sprites */
            ROM_LOAD("lr-b-3n", 0x2000, 0x2000, 0x55154154);
            ROM_LOAD("lr-b-4c", 0x4000, 0x2000, 0x924e34d0);

            ROM_REGION(0x0720, Mame.REGION_PROMS);
            ROM_LOAD("lr-e-3m", 0x0000, 0x0100, 0x53040416);	/* character palette red component */
            ROM_LOAD("lr-b-1m", 0x0100, 0x0100, 0x4bae1c25);/* sprite palette red component */
            ROM_LOAD("lr-e-3l", 0x0200, 0x0100, 0x67786037);	/* character palette green component */
            ROM_LOAD("lr-b-1n", 0x0300, 0x0100, 0x9cd3db94);	/* sprite palette green component */
            ROM_LOAD("lr-e-3n", 0x0400, 0x0100, 0x5b716837);	/* character palette blue component */
            ROM_LOAD("lr-b-1l", 0x0500, 0x0100, 0x08d8cf9a);	/* sprite palette blue component */
            ROM_LOAD("lr-b-5p", 0x0600, 0x0020, 0xe01f69e2);	/* sprite height, one entry per 32 */
            /* sprites. Used at run time! */
            ROM_LOAD("lr-b-6f", 0x0620, 0x0100, 0x34d88d3c);	/* video timing? - common to the other games */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ldrun()
        {
            INPUT_PORTS_START("ldrun");
            PORT_START();	/* IN0 */
            IN0_PORT();

            PORT_START();	/* IN1 */
            IN1_PORT();

            PORT_START();	/* IN2 */
            IN2_PORT();

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, "Timer Speed");
            PORT_DIPSETTING(0x03, "Slow");
            PORT_DIPSETTING(0x02, "Medium");
            PORT_DIPSETTING(0x01, "Fast");
            PORT_DIPSETTING(0x00, "Fastest");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Lives"));
            PORT_DIPSETTING(0x08, "2");
            PORT_DIPSETTING(0x0c, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x00, "5");
            COINAGE_DSW();

            PORT_START();	/* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x02, DEF_STR("Cocktail"));
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, "Unknown");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x10, 0x10, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Freeze", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In level selection mode, press 1 to select and 2 to restart */
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Level Selection Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        public driver_ldrun()
        {
            drv = new machine_driver_ldrun();
            year = "1984";
            name = "ldrun";
            description = "Lode Runner (set 1)";
            manufacturer = "Irem (licensed from Broderbund)";
            flags = Mame.ROT0;
            input_ports = input_ports_ldrun();
            rom = rom_ldrun();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_ldrun2 : driver_m62
    {
        static Mame.MemoryReadAddress[] ldrun2_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xd000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] ldrun2_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x9fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOReadPort[] ldrun2_readport =
{
	new Mame.IOReadPort( 0x00, 0x00, Mame.input_port_0_r ),   /* coin */
	new Mame.IOReadPort( 0x01, 0x01, Mame.input_port_1_r ),   /* player 1 control */
	new Mame.IOReadPort( 0x02, 0x02, Mame.input_port_2_r ),   /* player 2 control */
	new Mame.IOReadPort( 0x03, 0x03, Mame.input_port_3_r ),   /* DSW 1 */
	new Mame.IOReadPort( 0x04, 0x04, Mame.input_port_4_r ),   /* DSW 2 */
	new Mame.IOReadPort( 0x80, 0x80, ldrun2_bankswitch_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] ldrun2_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, irem.irem_sound_cmd_w ),
	new Mame.IOWritePort( 0x01, 0x01, irem_flipscreen_w),	/* + coin counters */
	new Mame.IOWritePort( 0x80, 0x81, ldrun2_bankswitch_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.GfxDecodeInfo[] ldrun2_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, tilelayout_1024,      0, 32 ),	/* use colors   0-255 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout_512,   256, 32 ),	/* use colors 256-511 */
};
        static int[] bankcontrol = new int[2];
        static int ldrun2_bankswap;

        static int ldrun2_bankswitch_r(int offset)
        {
            if (ldrun2_bankswap != 0)
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


                ldrun2_bankswap--;

                /* swap to bank #1 on second read */
                if (ldrun2_bankswap == 0)
                    Mame.cpu_setbank(1, new _BytePtr(RAM, 0x12000));
            }
            return 0;
        }
        static void ldrun2_bankswitch_w(int offset, int data)
        {
            int bankaddress;

            int[] banks =
	{
		0,0,0,0,0,1,0,1,0,0,
		0,1,1,1,1,1,0,0,0,0,
		1,0,1,1,1,1,1,1,1,1
	};
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            bankcontrol[offset] = data;
            if (offset == 0)
            {
                if (data < 1 || data > 30)
                {
                    Mame.printf("unknown bank select %02x\n", data);
                    return;
                }
                bankaddress = 0x10000 + (banks[data - 1] * 0x2000);
                Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
            }
            else
            {
                if (bankcontrol[0] == 0x01 && data == 0x0d)
                    /* special case for service mode */
                    ldrun2_bankswap = 2;
                else ldrun2_bankswap = 0;
            }
        }
        class machine_driver_ldrun2 : Mame.MachineDriver
        {
            public machine_driver_ldrun2()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, ldrun2_readmem, ldrun2_writemem, ldrun2_readport, ldrun2_writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 55;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = ldrun2_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));

            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                irem_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                driver_m62.irem_background_hscroll = 0;
                driver_m62.irem_background_vscroll = 0;
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_m62.vh_update(bitmap, full_refresh);
            }
        }
        public override void driver_init() { }
        Mame.RomModule[] rom_ldrun2()
        {

            ROM_START("ldrun2");
            ROM_REGION(0x14000, Mame.REGION_CPU1);	/* 64k for code + 16k for banks */
            ROM_LOAD("lr2-a-4e.a", 0x00000, 0x2000, 0x22313327);
            ROM_LOAD("lr2-a-4d", 0x02000, 0x2000, 0xef645179);
            ROM_LOAD("lr2-a-4a.a", 0x04000, 0x2000, 0xb11ddf59);
            ROM_LOAD("lr2-a-4a", 0x06000, 0x2000, 0x470cc8a1);
            ROM_LOAD("lr2-h-1c.a", 0x10000, 0x2000, 0x7ebcadbc);	/* banked at 8000-9fff */
            ROM_LOAD("lr2-h-1d.a", 0x12000, 0x2000, 0x64cbb7f9);	/* banked at 8000-9fff */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU (6803) */
            ROM_LOAD("lr2-a-3e", 0xa000, 0x2000, 0x853f3898);
            ROM_LOAD("lr2-a-3f", 0xc000, 0x2000, 0x7a96accd);
            ROM_LOAD("lr2-a-3h", 0xe000, 0x2000, 0x2a0e83ca);

            ROM_REGION(0x6000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr2-h-1e", 0x00000, 0x2000, 0x9d63a8ff);	/* characters */
            ROM_LOAD("lr2-h-1j", 0x02000, 0x2000, 0x40332bbd);
            ROM_LOAD("lr2-h-1h", 0x04000, 0x2000, 0x9404727d);

            ROM_REGION(0xc000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr2-b-4k", 0x00000, 0x2000, 0x79909871);	/* sprites */
            ROM_LOAD("lr2-b-4f", 0x02000, 0x2000, 0x06ba1ef4);
            ROM_LOAD("lr2-b-3n", 0x04000, 0x2000, 0x3cc5893f);
            ROM_LOAD("lr2-b-4n", 0x06000, 0x2000, 0x49c12f42);
            ROM_LOAD("lr2-b-4c", 0x08000, 0x2000, 0xfbe6d24c);
            ROM_LOAD("lr2-b-4e", 0x0a000, 0x2000, 0x75172d1f);

            ROM_REGION(0x0720, Mame.REGION_PROMS);
            ROM_LOAD("lr2-h-3m", 0x0000, 0x0100, 0x2c5d834b);	/* character palette red component */
            ROM_LOAD("lr2-b-1m", 0x0100, 0x0100, 0x4ec9bb3d);	/* sprite palette red component */
            ROM_LOAD("lr2-h-3l", 0x0200, 0x0100, 0x3ae69aca);	/* character palette green component */
            ROM_LOAD("lr2-b-1n", 0x0300, 0x0100, 0x1daf1fa4);	/* sprite palette green component */
            ROM_LOAD("lr2-h-3n", 0x0400, 0x0100, 0x2b28aec5);	/* character palette blue component */
            ROM_LOAD("lr2-b-1l", 0x0500, 0x0100, 0xc8fb708a);	/* sprite palette blue component */
            ROM_LOAD("lr2-b-5p", 0x0600, 0x0020, 0xe01f69e2);	/* sprite height, one entry per 32 */
            /* sprites. Used at run time! */
            ROM_LOAD("lr2-b-6f", 0x0620, 0x0100, 0x34d88d3c);	/* video timing? - common to the other games */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ldrun2()
        {

            INPUT_PORTS_START("ldrun2");
            PORT_START();/* IN0 */
            IN0_PORT();

            PORT_START();	/* IN1 */
            IN1_PORT();

            PORT_START();	/* IN2 */
            IN2_PORT();

            PORT_START();/* DSW1 */
            PORT_DIPNAME(0x01, 0x01, "Timer Speed");
            PORT_DIPSETTING(0x01, "Slow");
            PORT_DIPSETTING(0x00, "Fast");
            PORT_DIPNAME(0x02, 0x02, "Game Speed");
            PORT_DIPSETTING(0x00, "Low");
            PORT_DIPSETTING(0x02, "High");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Lives"));
            PORT_DIPSETTING(0x08, "2");
            PORT_DIPSETTING(0x0c, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x00, "5");
            COINAGE_DSW();

            PORT_START();	/* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x02, DEF_STR("Cocktail"));
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, "Unknown");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In freeze mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x10, 0x10, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Freeze", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In level selection mode, press 1 to select and 2 to restart */
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Level Selection Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        public driver_ldrun2()
        {
            drv = new machine_driver_ldrun2();
            year = "1984";
            name = "ldrun2";
            description = "Lode Runner II - The Bungeling Strikes Back";
            manufacturer = "Irem (licensed from Broderbund)";
            flags = Mame.ROT0;
            input_ports = input_ports_ldrun2();
            rom = rom_ldrun2();
            drv.HasNVRAMhandler = false;
        }
    }

    class driver_ldrun3 : driver_m62
    {
        static Mame.MemoryReadAddress[] ldrun3_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc800, 0xc800, ldrun3_prot_5_r ),
	new Mame.MemoryReadAddress( 0xcc00, 0xcc00, ldrun3_prot_7_r ),
	new Mame.MemoryReadAddress( 0xcfff, 0xcfff, ldrun3_prot_7_r ),
	new Mame.MemoryReadAddress( 0xd000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] ldrun3_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOWritePort[] ldrun3_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, irem.irem_sound_cmd_w ),
	new Mame.IOWritePort( 0x01, 0x01, driver_m62.irem_flipscreen_w ),	/* + coin counters */
	new Mame.IOWritePort( 0x80, 0x80, ldrun3_vscroll_w),
	/* 0x81 used too, don't know what for */
	new Mame.IOWritePort( -1 )	/* end of table */
};
        static Mame.GfxDecodeInfo[] ldrun3_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, tilelayout_2048,      0, 32 ),	/* use colors   0-255 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout_512,   256, 32 ),	/* use colors 256-511 */
};
        public static void ldrun3_vscroll_w(int offset, int data)
        {
            driver_m62.irem_background_vscroll = data;
        }

        static int ldrun3_prot_5_r(int offset)
        {
            return 5;
        }

        static int ldrun3_prot_7_r(int offset)
        {
            return 7;
        }

        class machine_driver_ldrun3 : Mame.MachineDriver
        {
            public machine_driver_ldrun3()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, ldrun3_readmem, ldrun3_writemem, driver_ldrun.ldrun_readport, ldrun3_writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 55;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = ldrun3_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));

            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                irem_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                driver_m62.irem_background_hscroll = 0;
                driver_m62.irem_background_vscroll = 0;
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                driver_m62.vh_update(bitmap, full_refresh);
            }
        }
        public override void driver_init() { }
        Mame.RomModule[] rom_ldrun3()
        {

            ROM_START("ldrun3");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("lr3-a-4e", 0x0000, 0x4000, 0x5b334e8e);
            ROM_LOAD("lr3-a-4d.a", 0x4000, 0x4000, 0xa84bc931);
            ROM_LOAD("lr3-a-4b.a", 0x8000, 0x4000, 0xbe09031d);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU (6803) */
            ROM_LOAD("lr3-a-3d", 0x8000, 0x4000, 0x28be68cd);
            ROM_LOAD("lr3-a-3f", 0xc000, 0x4000, 0xcb7186b7);

            ROM_REGION(0xc000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr3-n-2a", 0x00000, 0x4000, 0xf9b74dee);	/* characters */
            ROM_LOAD("lr3-n-2c", 0x04000, 0x4000, 0xfef707ba);
            ROM_LOAD("lr3-n-2b", 0x08000, 0x4000, 0xaf3d27b9);

            ROM_REGION(0xc000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr3-b-4k", 0x00000, 0x4000, 0x63f070c7);	/* sprites */
            ROM_LOAD("lr3-b-3n", 0x04000, 0x4000, 0xeab7ad91);
            ROM_LOAD("lr3-b-4c", 0x08000, 0x4000, 0x1a460a46);

            ROM_REGION(0x0820, Mame.REGION_PROMS);
            ROM_LOAD("lr3-n-2l", 0x0000, 0x0100, 0xe880b86b); /* character palette red component */
            ROM_LOAD("lr3-b-1m", 0x0100, 0x0100, 0xf02d7167); /* sprite palette red component */
            ROM_LOAD("lr3-n-2k", 0x0200, 0x0100, 0x047ee051); /* character palette green component */
            ROM_LOAD("lr3-b-1n", 0x0300, 0x0100, 0x9e37f181); /* sprite palette green component */
            ROM_LOAD("lr3-n-2m", 0x0400, 0x0100, 0x69ad8678); /* character palette blue component */
            ROM_LOAD("lr3-b-1l", 0x0500, 0x0100, 0x5b11c41d); /* sprite palette blue component */
            ROM_LOAD("lr3-b-5p", 0x0600, 0x0020, 0xe01f69e2);	/* sprite height, one entry per 32 */
            /* sprites. Used at run time! */
            ROM_LOAD("lr3-n-4f", 0x0620, 0x0100, 0xdf674be9);	/* unknown */
            ROM_LOAD("lr3-b-6f", 0x0720, 0x0100, 0x34d88d3c);	/* video timing? - common to the other games */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ldrun3()
        {

            INPUT_PORTS_START("ldrun3");
            PORT_START();	/* IN0 */
            IN0_PORT();

            PORT_START();	/* IN1 */
            IN1_PORT();

            PORT_START();/* IN2 */
            IN2_PORT();

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x01, 0x01, "Timer Speed");
            PORT_DIPSETTING(0x01, "Slow");
            PORT_DIPSETTING(0x00, "Fast");
            PORT_DIPNAME(0x02, 0x02, "Game Speed");
            PORT_DIPSETTING(0x00, "Low");
            PORT_DIPSETTING(0x02, "High");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Lives"));
            PORT_DIPSETTING(0x08, "2");
            PORT_DIPSETTING(0x0c, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x00, "5");
            COINAGE_DSW();

            PORT_START();	/* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x02, DEF_STR("Cocktail"));
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, "Unknown");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x10, 0x10, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Freeze", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* In level selection mode, press 1 to select and 2 to restart */
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Level Selection Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }

        public driver_ldrun3()
        {
            drv = new machine_driver_ldrun3();
            year = "1985";
            name = "ldrun3";
            description = "Lode Runner III - Majin No Fukkatsu";
            manufacturer = "Irem (licensed from Broderbund)";
            flags = Mame.ROT0;
            input_ports = input_ports_ldrun3();
            rom = rom_ldrun3();
            drv.HasNVRAMhandler = false;
        }
    }

    class driver_ldrun4 : driver_m62
    {

        static Mame.MemoryReadAddress[] ldrun4_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xd000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] ldrun4_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, ldrun4_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOWritePort[] ldrun4_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, irem.irem_sound_cmd_w ),
	new Mame.IOWritePort( 0x01, 0x01, driver_m62.irem_flipscreen_w ),	/* + coin counters */
	new Mame.IOWritePort( 0x82, 0x83, ldrun4_hscroll_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};
        static Mame.GfxDecodeInfo[] ldrun4_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, tilelayout_2048,      0, 32 ),	/* use colors   0-255 */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout_1024,  256, 32 ),	/* use colors 256-511 */
};
        static void ldrun4_hscroll_w(int offset, int data)
        {
            driver_m62.irem_background_hscroll_w(offset ^ 1, data);
        }
        static void ldrun4_bankswitch_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            bankaddress = 0x10000 + ((data & 0x01) * 0x4000);
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }
        class machine_driver_ldrun4 : Mame.MachineDriver
        {
            public machine_driver_ldrun4()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, ldrun4_readmem, ldrun4_writemem, driver_ldrun.ldrun_readport, ldrun4_writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 55;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 64 * 8;
                screen_height = 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                visible_area = new Mame.rectangle(8 * 8, (64 - 8) * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = ldrun4_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));

            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                irem_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                driver_m62.irem_background_hscroll = 0;
                driver_m62.irem_background_vscroll = 0;
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                ldrun4_draw_background(bitmap);
                draw_sprites(bitmap, Mame.Machine.drv.visible_area, Mame.Machine.drv.visible_area);
            }
            static void draw_sprites(Mame.osd_bitmap bitmap,
                         Mame.rectangle spritevisiblearea,
                         Mame.rectangle flipspritevisiblearea)
            {
                int offs;

                /* sprites must be drawn in this order to get correct priority */
                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 8)
                {
                    int i, incr, code, col, flipx, flipy, sx, sy;


                    code = Generic.spriteram[offs + 4] + ((Generic.spriteram[offs + 5] & 0x07) << 8);
                    col = Generic.spriteram[offs + 0] & 0x1f;
                    sx = 256 * (Generic.spriteram[offs + 7] & 1) + Generic.spriteram[offs + 6];
                    sy = 256 + 128 - 15 - (256 * (Generic.spriteram[offs + 3] & 1) + Generic.spriteram[offs + 2]);
                    flipx = Generic.spriteram[offs + 5] & 0x40;
                    flipy = Generic.spriteram[offs + 5] & 0x80;

                    i = sprite_height_prom[(code >> 5) & 0x1f];
                    if (i == 1)	/* double height */
                    {
                        code &= ~1;
                        sy -= 16;
                    }
                    else if (i == 2)	/* quadruple height */
                    {
                        i = 3;
                        code &= ~3;
                        sy -= 3 * 16;
                    }

                    if (flipscreen != 0)
                    {
                        sx = 496 - sx;
                        sy = 242 - i * 16 - sy;	/* sprites are slightly misplaced by the hardware */
                        flipx = flipx == 0 ? 1 : 0;
                        flipy = flipy == 0 ? 1 : 0;
                    }

                    if (flipy != 0)
                    {
                        incr = -1;
                        code += i;
                    }
                    else incr = 1;

                    do
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)(code + i * incr), (uint)col,
                                flipx != 0, flipy != 0,
                                sx, sy + 16 * i,
                                flipscreen != 0 ? flipspritevisiblearea : spritevisiblearea, Mame.TRANSPARENCY_PEN, 0);

                        i--;
                    } while (i >= 0);
                }
            }


            void ldrun4_draw_background(Mame.osd_bitmap bitmap)
            {
                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    if (Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1])
                    {
                        int sx, sy;


                        Generic.dirtybuffer[offs] = false;
                        Generic.dirtybuffer[offs + 1] = false;

                        sx = (offs / 2) % 64;
                        sy = (offs / 2) / 64;

                        if (flipscreen != 0)
                        {
                            sx = 63 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + ((Generic.videoram[offs + 1] & 0xc0) << 2) + ((Generic.videoram[offs + 1] & 0x20) << 5)),
                                (uint)(Generic.videoram[offs + 1] & 0x1f),
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                {
                    int scrollx;

                    if (flipscreen != 0)
                        scrollx = irem_background_hscroll + 2;
                    else
                        scrollx = -irem_background_hscroll + 2;

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[] { scrollx }, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }

        }
        public override void driver_init() { }
        Mame.RomModule[] rom_ldrun4()
        {

            ROM_START("ldrun4");
            ROM_REGION(0x18000, Mame.REGION_CPU1);	/* 64k for code + 32k for banked ROM */
            ROM_LOAD("lr4-a-4e", 0x00000, 0x4000, 0x5383e9bf);
            ROM_LOAD("lr4-a-4d.c", 0x04000, 0x4000, 0x298afa36);
            ROM_LOAD("lr4-v-4k", 0x10000, 0x8000, 0x8b248abd);/* banked at 8000-bfff */

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for the audio CPU (6803) */
            ROM_LOAD("lr4-a-3d", 0x8000, 0x4000, 0x86c6d445);
            ROM_LOAD("lr4-a-3f", 0xc000, 0x4000, 0x097c6c0a);

            ROM_REGION(0xc000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr4-v-2b", 0x00000, 0x4000, 0x4118e60a);	/* characters */
            ROM_LOAD("lr4-v-2d", 0x04000, 0x4000, 0x542bb5b5);
            ROM_LOAD("lr4-v-2c", 0x08000, 0x4000, 0xc765266c);

            ROM_REGION(0x18000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lr4-b-4k", 0x00000, 0x4000, 0xe7fe620c);	/* sprites */
            ROM_LOAD("lr4-b-4f", 0x04000, 0x4000, 0x6f0403db);
            ROM_LOAD("lr4-b-3n", 0x08000, 0x4000, 0xad1fba1b);
            ROM_LOAD("lr4-b-4n", 0x0c000, 0x4000, 0x0e568fab);
            ROM_LOAD("lr4-b-4c", 0x10000, 0x4000, 0x82c53669);
            ROM_LOAD("lr4-b-4e", 0x14000, 0x4000, 0x767a1352);

            ROM_REGION(0x0820, Mame.REGION_PROMS);
            ROM_LOAD("lr4-v-1m", 0x0000, 0x0100, 0xfe51bf1d); /* character palette red component */
            ROM_LOAD("lr4-b-1m", 0x0100, 0x0100, 0x5d8d17d0); /* sprite palette red component */
            ROM_LOAD("lr4-v-1n", 0x0200, 0x0100, 0xda0658e5); /* character palette green component */
            ROM_LOAD("lr4-b-1n", 0x0300, 0x0100, 0xda1129d2); /* sprite palette green component */
            ROM_LOAD("lr4-v-1p", 0x0400, 0x0100, 0x0df23ebe); /* character palette blue component */
            ROM_LOAD("lr4-b-1l", 0x0500, 0x0100, 0x0d89b692); /* sprite palette blue component */
            ROM_LOAD("lr4-b-5p", 0x0600, 0x0020, 0xe01f69e2);	/* sprite height, one entry per 32 */
            /* sprites. Used at run time! */
            ROM_LOAD("lr4-v-4h", 0x0620, 0x0100, 0xdf674be9);	/* unknown */
            ROM_LOAD("lr4-b-6f", 0x0720, 0x0100, 0x34d88d3c);	/* video timing? - common to the other games */
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_ldrun4()
        {

            INPUT_PORTS_START("ldrun4");
            PORT_START();	/* IN0 */
            IN0_PORT();

            PORT_START();	/* IN1 */
            IN1_PORT();

            PORT_START();	/* IN2 */
            IN2_PORT();

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x01, 0x01, "Timer Speed");
            PORT_DIPSETTING(0x01, "Slow");
            PORT_DIPSETTING(0x00, "Fast");
            PORT_DIPNAME(0x02, 0x02, "2 Players Game");
            PORT_DIPSETTING(0x00, "1 Credit");
            PORT_DIPSETTING(0x02, "2 Credits");
            PORT_DIPNAME(0x0c, 0x0c, "1 Player Lives");
            PORT_DIPSETTING(0x08, "2");
            PORT_DIPSETTING(0x0c, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x00, "5");
            COINAGE_DSW();

            PORT_START();	/* DSW2 */
            PORT_DIPNAME(0x01, 0x01, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, "2 Players Lives");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPSETTING(0x00, "6");
            /* This activates a different coin mode. Look at the dip switch setting schematic */
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, "Unknown");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x10, "Allow 2 Players Game");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x10, DEF_STR("Yes"));
            /* In level selection mode, press 1 to select and 2 to restart */
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Level Selection Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x80, 0x80, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_TOGGLE, "Service Mode (must set 2P game to No)", (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        public driver_ldrun4()
        {
            drv = new machine_driver_ldrun4();
            year = "1986";
            name = "ldrun4";
            description = "Lode Runner IV - Teikoku Karano Dasshutsu";
            manufacturer = "Irem (licensed from Broderbund)";
            flags = Mame.ROT0;
            input_ports = input_ports_ldrun4();
            rom = rom_ldrun4();
            drv.HasNVRAMhandler = false;
        }
    }

}
