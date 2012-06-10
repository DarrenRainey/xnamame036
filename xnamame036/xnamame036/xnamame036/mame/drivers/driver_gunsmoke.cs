using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_gunsmoke : Mame.GameDriver
    {
        static _BytePtr gunsmoke_bg_scrolly = new _BytePtr(1);
        static _BytePtr gunsmoke_bg_scrollx = new _BytePtr(1);
        static int chon, objon, bgon;
        static int sprite3bank;
        static int flipscreen;

        static Mame.osd_bitmap bgbitmap;
        static byte[][][] bgmap;//[9][9][2];

        static int gunsmoke_unknown_r(int offset)
        {
            int[] gunsmoke_fixed_data = { 0xff, 0x00, 0x00 };
            /*
            The routine at 0x0e69 tries to read data starting at 0xc4c9.
            If this value is zero, it interprets the next two bytes as a
            jump address.

            This was resulting in a reboot which happens at the end of level 3
            if you go too far to the right of the screen when fighting the level boss.

            A non-zero for the first byte seems to be harmless  (although it may not be
            the correct behaviour).

            This could be some devious protection or it could be a bug in the
            arcade game.  It's hard to tell without pulling the code apart.
            */
            return gunsmoke_fixed_data[offset];
        }

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff,Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff,Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000,Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xc001, 0xc001,Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xc002, 0xc002,Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0xc003, 0xc003,Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xc004, 0xc004,Mame.input_port_4_r ),
    new Mame.MemoryReadAddress( 0xc4c9, 0xc4cb, gunsmoke_unknown_r ),
    new Mame.MemoryReadAddress( 0xd000, 0xd3ff, Generic.videoram_r ),
    new Mame.MemoryReadAddress( 0xd400, 0xd7ff, Generic.colorram_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_RAM ), /* Work + sprite RAM */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, gunsmoke_c804_w ),	/* ROM bank switch, screen flip */
	new Mame.MemoryWriteAddress( 0xc806, 0xc806, Mame.MWA_NOP ), /* Watchdog ?? */
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xd800, 0xd801, Mame.MWA_RAM, gunsmoke_bg_scrolly ),
	new Mame.MemoryWriteAddress( 0xd802, 0xd802, Mame.MWA_RAM, gunsmoke_bg_scrollx ),
	new Mame.MemoryWriteAddress( 0xd806, 0xd806, gunsmoke_d806_w ),	/* sprites and bg enable */
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};



        static Mame.MemoryReadAddress[] sound_readmem =
{
	new  Mame.MemoryReadAddress( 0x0000, 0x7fff,Mame.MRA_ROM ),
	new  Mame.MemoryReadAddress( 0xc000, 0xc7ff,Mame.MRA_RAM ),
	new  Mame.MemoryReadAddress( 0xc800, 0xc800, Mame.soundlatch_r ),
	new  Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdfff, Mame.MWA_RAM),
	new Mame.MemoryWriteAddress( 0xe000, 0xe000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe001, 0xe001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe002, 0xe002, ym2203.YM2203_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xe003, 0xe003, ym2203.YM2203_write_port_1_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            1024,	/* 1024 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },
        new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            2048,	/* 2048 sprites */
            4,      /* 4 bits per pixel */
            new uint[] { 2048 * 64 * 8 + 4, 2048 * 64 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			32*8+0, 32*8+1, 32*8+2, 32*8+3, 33*8+0, 33*8+1, 33*8+2, 33*8+3 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            32, 32,  /* 32*32 tiles */
            512,    /* 512 tiles */
            4,      /* 4 bits per pixel */
            new uint[] { 512 * 256 * 8 + 4, 512 * 256 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			64*8+0, 64*8+1, 64*8+2, 64*8+3, 65*8+0, 65*8+1, 65*8+2, 65*8+3,
			128*8+0, 128*8+1, 128*8+2, 128*8+3, 129*8+0, 129*8+1, 129*8+2, 129*8+3,
			192*8+0, 192*8+1, 192*8+2, 192*8+3, 193*8+0, 193*8+1, 193*8+2, 193*8+3 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16,
			16*16, 17*16, 18*16, 19*16, 20*16, 21*16, 22*16, 23*16,
			24*16, 25*16, 26*16, 27*16, 28*16, 29*16, 30*16, 31*16 },
            256 * 8	/* every tile takes 256 consecutive bytes */
        );


        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,            0, 32 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tilelayout,         32*4, 16 ), /* Tiles */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout, 32*4+16*16, 16 ), /* Sprites */
};

        static YM2203interface ym2203_interface =
        new YM2203interface(
            2,			/* 2 chips */
            1500000,	/* 1.5 MHz (?) */
            new int[] { ym2203.YM2203_VOL(14, 22), ym2203.YM2203_VOL(14, 22) },
            new AY8910portRead[] { null, null },
            new AY8910portRead[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null }, null
        );

        static void gunsmoke_c804_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            /* bits 0 and 1 are for coin counters? - we ignore them */

            /* bits 2 and 3 select the ROM bank */
            int bankaddress = 0x10000 + (data & 0x0c) * 0x1000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));

            /* bit 5 resets the sound CPU? - we ignore it */

            /* bit 6 flips screen */
            if (flipscreen != (data & 0x40))
            {
                flipscreen = data & 0x40;
                //		memset(dirtybuffer,1,c1942_backgroundram_size);
            }

            /* bit 7 enables characters? */
            chon = data & 0x80;
        }
        static void gunsmoke_d806_w(int offset, int data)
        {
            /* bits 0-2 select the sprite 3 bank */
            sprite3bank = data & 0x07;

            /* bit 4 enables bg 1? */
            bgon = data & 0x10;

            /* bit 5 enables sprites? */
            objon = data & 0x20;
        }
        public class machine_driver_gunsmoke : Mame.MachineDriver
        {
            public machine_driver_gunsmoke()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 4));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_gunsmoke.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 32 * 4 + 16 * 16 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                int pi = 0;

                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0 = (color_prom[0] >> 0) & 0x01;
                    int bit1 = (color_prom[0] >> 1) & 0x01;
                    int bit2 = (color_prom[0] >> 2) & 0x01;
                    int bit3 = (color_prom[0] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    color_prom.offset++;
                }

                color_prom.offset += 2 * (int)Mame.Machine.drv.total_colors;
                /* color_prom now points to the beginning of the lookup table */

                /* characters use colors 64-79 */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                {
                    COLOR(colortable, 0, i, (ushort)(color_prom[0] + 64));
                    color_prom.offset++;
                }
                color_prom.offset += 128;	/* skip the bottom half of the PROM - not used */

                /* background tiles use colors 0-63 */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    COLOR(colortable, 1, i, (ushort)(color_prom[0] + 16 * (color_prom[256] & 0x03)));
                    color_prom.offset++;
                }

                color_prom.offset += TOTAL_COLORS(1);

                /* sprites use colors 128-255 */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                {
                    COLOR(colortable, 2, i, (ushort)(color_prom[0] + 16 * (color_prom[256] & 0x07) + 128));
                    color_prom.offset++;
                }
                color_prom.offset += TOTAL_COLORS(2);
            }
            public override int vh_start()
            {
                bgbitmap = Mame.osd_create_bitmap(9 * 32, 9 * 32);

                if (Generic.generic_vh_start() == 1)
                {
                    Mame.osd_free_bitmap(bgbitmap);
                    return 1;
                }
                bgmap = new byte[9][][];
                for (int i = 0; i < 9; i++)
                {
                    bgmap[i] = new byte[9][];
                    for (int j = 0; j < 9; j++)
                    {
                        bgmap[i][j] = new byte[2];
                        bgmap[i][j][0] = 0xff;
                        bgmap[i][j][1] = 0xff;
                    }
                }

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(bgbitmap);
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int offs, sx, sy;
                int bg_scrolly, bg_scrollx;
                _BytePtr p = Mame.memory_region(Mame.REGION_GFX4);
                int top, left, xscroll, yscroll;


                /* TODO: support flipscreen */
                if (bgon != 0)
                {
                    bg_scrolly = gunsmoke_bg_scrolly[0] + 256 * gunsmoke_bg_scrolly[1];
                    bg_scrollx = gunsmoke_bg_scrollx[0];
                    offs = 16 * ((bg_scrolly >> 5) + 8) + 2 * (bg_scrollx >> 5);
                    if ((bg_scrollx & 0x80) != 0) offs -= 0x10;

                    top = 8 - (bg_scrolly >> 5) % 9;
                    left = (bg_scrollx >> 5) % 9;

                    bg_scrolly &= 0x1f;
                    bg_scrollx &= 0x1f;

                    for (sy = 0; sy < 9; sy++)
                    {
                        int ty = (sy + top) % 9;
                        offs &= 0x7fff; /* Enforce limits (for top of scroll) */

                        for (sx = 0; sx < 9; sx++)
                        {
                            int tile, attr, offset;
                            int tx = (sx + left) % 9;
                            _BytePtr map = new _BytePtr(bgmap[ty][tx], 0);
                            offset = offs + (sx * 2);

                            tile = p[offset];
                            attr = p[offset + 1];

                            if (tile != map[0] || attr != map[1])
                            {
                                map[0] = (byte)tile;
                                map[1] = (byte)attr;
                                tile += 256 * (attr & 0x01);
                                Mame.drawgfx(bgbitmap, Mame.Machine.gfx[1],
                                        (uint)tile,
                                        (uint)(attr & 0x3c) >> 2,
                                        (attr & 0x40) != 0, (attr & 0x80) != 0,
                                        (8 - ty) * 32, tx * 32,
                                        null,
                                        Mame.TRANSPARENCY_NONE, 0);
                            }
                            map.offset += 2;
                        }
                        offs -= 0x10;
                    }

                    xscroll = (top * 32 - bg_scrolly);
                    yscroll = -(left * 32 + bg_scrollx);
                    Mame.copyscrollbitmap(bitmap, bgbitmap,
                        1, new int[] { xscroll },
                        1, new int[] { yscroll },
                        Mame.Machine.drv.visible_area,
                        Mame.TRANSPARENCY_NONE, 0);
                }
                else Mame.fillbitmap(bitmap, Mame.Machine.pens.read16(0), Mame.Machine.drv.visible_area);



                if (objon != 0)
                {
                    /* Draw the sprites. */
                    for (offs = Generic.spriteram_size[0] - 32; offs >= 0; offs -= 32)
                    {
                        int bank;
                        bool flipx, flipy;

                        bank = (Generic.spriteram[offs + 1] & 0xc0) >> 6;
                        if (bank == 3) bank += sprite3bank;

                        sx = Generic.spriteram[offs + 3] - ((Generic.spriteram[offs + 1] & 0x20) << 3);
                        sy = Generic.spriteram[offs + 2];
                        flipx = false;
                        flipy = (Generic.spriteram[offs + 1] & 0x10) != 0;
                        if (flipscreen != 0)
                        {
                            sx = 240 - sx;
                            sy = 240 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                (uint)(Generic.spriteram[offs] + 256 * bank),
                                (uint)(Generic.spriteram[offs + 1] & 0x0f),
                                flipx, flipy,
                                sx, sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }


                if (chon != 0)
                {
                    /* draw the frontmost playfield. They are characters, but draw them as sprites */
                    for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                    {
                        sx = offs % 32;
                        sy = offs / 32;
                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + ((Generic.colorram[offs] & 0xc0) << 2)),
                                (uint)(Generic.colorram[offs] & 0x1f),
                                flipscreen == 0, flipscreen == 0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 79);
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //nonethrow new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            //none
        }
        public static Mame.InputPortTiny[] input_ports_gunsmoke()
        {
            INPUT_PORTS_START("gunsmoke");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_COCKTAIL);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x03, "30k, 100k & every 100k");
            PORT_DIPSETTING(0x02, "30k, 80k & every 80k");
            PORT_DIPSETTING(0x01, "30k & 100K only");
            PORT_DIPSETTING(0x00, "30k, 100k & every 150k");
            PORT_DIPNAME(0x04, 0x04, "Demonstration");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x04, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x08, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x20, "Easy");
            PORT_DIPSETTING(0x30, "Normal");
            PORT_DIPSETTING(0x10, "Difficult");
            PORT_DIPSETTING(0x00, "Very Difficult");
            PORT_DIPNAME(0x40, 0x40, "Freeze");
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x07, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x38, 0x38, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x38, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x28, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x18, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x40, 0x40, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR("No"));
            PORT_DIPSETTING(0x40, DEF_STR("Yes"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_gunsmoke()
        {
            ROM_START("gunsmoke");
            ROM_REGION(0x20000, Mame.REGION_CPU1);     /* 2*64k for code */
            ROM_LOAD("09n_gs03.bin", 0x00000, 0x8000, 0x40a06cef); /* Code 0000-7fff */
            ROM_LOAD("10n_gs04.bin", 0x10000, 0x8000, 0x8d4b423f); /* Paged code */
            ROM_LOAD("12n_gs05.bin", 0x18000, 0x8000, 0x2b5667fb); /* Paged code */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("14h_gs02.bin", 0x00000, 0x8000, 0xcd7a2c38);

            ROM_REGION(0x04000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("11f_gs01.bin", 0x00000, 0x4000, 0xb61ece9b); /* Characters */

            ROM_REGION(0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("06c_gs13.bin", 0x00000, 0x8000, 0xf6769fc5); /* 32x32 tiles planes 2-3 */
            ROM_LOAD("05c_gs12.bin", 0x08000, 0x8000, 0xd997b78c);
            ROM_LOAD("04c_gs11.bin", 0x10000, 0x8000, 0x125ba58e);
            ROM_LOAD("02c_gs10.bin", 0x18000, 0x8000, 0xf469c13c);
            ROM_LOAD("06a_gs09.bin", 0x20000, 0x8000, 0x539f182d); /* 32x32 tiles planes 0-1 */
            ROM_LOAD("05a_gs08.bin", 0x28000, 0x8000, 0xe87e526d);
            ROM_LOAD("04a_gs07.bin", 0x30000, 0x8000, 0x4382c0d2);
            ROM_LOAD("02a_gs06.bin", 0x38000, 0x8000, 0x4cafe7a6);

            ROM_REGION(0x40000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("06n_gs22.bin", 0x00000, 0x8000, 0xdc9c508c); /* Sprites planes 2-3 */
            ROM_LOAD("04n_gs21.bin", 0x08000, 0x8000, 0x68883749); /* Sprites planes 2-3 */
            ROM_LOAD("03n_gs20.bin", 0x10000, 0x8000, 0x0be932ed); /* Sprites planes 2-3 */
            ROM_LOAD("01n_gs19.bin", 0x18000, 0x8000, 0x63072f93); /* Sprites planes 2-3 */
            ROM_LOAD("06l_gs18.bin", 0x20000, 0x8000, 0xf69a3c7c); /* Sprites planes 0-1 */
            ROM_LOAD("04l_gs17.bin", 0x28000, 0x8000, 0x4e98562a); /* Sprites planes 0-1 */
            ROM_LOAD("03l_gs16.bin", 0x30000, 0x8000, 0x0d99c3b3); /* Sprites planes 0-1 */
            ROM_LOAD("01l_gs15.bin", 0x38000, 0x8000, 0x7f14270e); /* Sprites planes 0-1 */

            ROM_REGION(0x8000, Mame.REGION_GFX4);/* background tilemaps */
            ROM_LOAD("11c_gs14.bin", 0x00000, 0x8000, 0x0af4f7eb);

            ROM_REGION(0x0800, Mame.REGION_PROMS);
            ROM_LOAD("03b_g-01.bin", 0x0000, 0x0100, 0x02f55589);	/* red component */
            ROM_LOAD("04b_g-02.bin", 0x0100, 0x0100, 0xe1e36dd9);	/* green component */
            ROM_LOAD("05b_g-03.bin", 0x0200, 0x0100, 0x989399c0);	/* blue component */
            ROM_LOAD("09d_g-04.bin", 0x0300, 0x0100, 0x906612b5);	/* char lookup table */
            ROM_LOAD("14a_g-06.bin", 0x0400, 0x0100, 0x4a9da18b);	/* tile lookup table */
            ROM_LOAD("15a_g-07.bin", 0x0500, 0x0100, 0xcb9394fc);	/* tile palette bank */
            ROM_LOAD("09f_g-09.bin", 0x0600, 0x0100, 0x3cee181e);	/* sprite lookup table */
            ROM_LOAD("08f_g-08.bin", 0x0700, 0x0100, 0xef91cdd2);	/* sprite palette bank */
            return ROM_END;
        }
        public driver_gunsmoke()
        {
            drv = new machine_driver_gunsmoke();
            year = "1985";
            name = "gunsmoke";
            description = "Gun.Smoke (World)";
            manufacturer = "Capcom";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_gunsmoke();
            rom = rom_gunsmoke();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_gunsmrom : Mame.GameDriver
    {
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_gunsmrom()
        {
            ROM_START( "gunsmrom" );
	ROM_REGION( 0x20000, Mame.REGION_CPU1 ) ;    /* 2*64k for code */
	ROM_LOAD( "9n_gs03.bin",  0x00000, 0x8000, 0x592f211b ); /* Code 0000-7fff */
	ROM_LOAD( "10n_gs04.bin", 0x10000, 0x8000, 0x8d4b423f ); /* Paged code */
	ROM_LOAD( "12n_gs05.bin", 0x18000, 0x8000, 0x2b5667fb ); /* Paged code */

	ROM_REGION( 0x10000, Mame.REGION_CPU2 );	/* 64k for the audio CPU */
	ROM_LOAD( "14h_gs02.bin", 0x00000, 0x8000, 0xcd7a2c38 );

	ROM_REGION( 0x04000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE );
	ROM_LOAD( "11f_gs01.bin", 0x00000, 0x4000, 0xb61ece9b ); /* Characters */

	ROM_REGION( 0x40000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE );
	ROM_LOAD( "06c_gs13.bin", 0x00000, 0x8000, 0xf6769fc5 ); /* 32x32 tiles planes 2-3 */
	ROM_LOAD( "05c_gs12.bin", 0x08000, 0x8000, 0xd997b78c );
	ROM_LOAD( "04c_gs11.bin", 0x10000, 0x8000, 0x125ba58e );
	ROM_LOAD( "02c_gs10.bin", 0x18000, 0x8000, 0xf469c13c );
	ROM_LOAD( "06a_gs09.bin", 0x20000, 0x8000, 0x539f182d ); /* 32x32 tiles planes 0-1 */
	ROM_LOAD( "05a_gs08.bin", 0x28000, 0x8000, 0xe87e526d );
	ROM_LOAD( "04a_gs07.bin", 0x30000, 0x8000, 0x4382c0d2 );
	ROM_LOAD( "02a_gs06.bin", 0x38000, 0x8000, 0x4cafe7a6 );

	ROM_REGION( 0x40000,Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE );
	ROM_LOAD( "06n_gs22.bin", 0x00000, 0x8000, 0xdc9c508c ); /* Sprites planes 2-3 */
	ROM_LOAD( "04n_gs21.bin", 0x08000, 0x8000, 0x68883749 ); /* Sprites planes 2-3 */
	ROM_LOAD( "03n_gs20.bin", 0x10000, 0x8000, 0x0be932ed ); /* Sprites planes 2-3 */
	ROM_LOAD( "01n_gs19.bin", 0x18000, 0x8000, 0x63072f93 ); /* Sprites planes 2-3 */
	ROM_LOAD( "06l_gs18.bin", 0x20000, 0x8000, 0xf69a3c7c ); /* Sprites planes 0-1 */
	ROM_LOAD( "04l_gs17.bin", 0x28000, 0x8000, 0x4e98562a ); /* Sprites planes 0-1 */
	ROM_LOAD( "03l_gs16.bin", 0x30000, 0x8000, 0x0d99c3b3 ); /* Sprites planes 0-1 */
	ROM_LOAD( "01l_gs15.bin", 0x38000, 0x8000, 0x7f14270e ); /* Sprites planes 0-1 */

	ROM_REGION( 0x8000, Mame.REGION_GFX4 );	/* background tilemaps */
    ROM_LOAD("11c_gs14.bin", 0x00000, 0x8000, 0x0af4f7eb);

	ROM_REGION( 0x0800, Mame.REGION_PROMS );
	ROM_LOAD( "03b_g-01.bin", 0x0000, 0x0100, 0x02f55589 );	/* red component */
	ROM_LOAD( "04b_g-02.bin", 0x0100, 0x0100, 0xe1e36dd9 );	/* green component */
	ROM_LOAD( "05b_g-03.bin", 0x0200, 0x0100, 0x989399c0 );	/* blue component */
	ROM_LOAD( "09d_g-04.bin", 0x0300, 0x0100, 0x906612b5 );	/* char lookup table */
	ROM_LOAD( "14a_g-06.bin", 0x0400, 0x0100, 0x4a9da18b );	/* tile lookup table */
	ROM_LOAD( "15a_g-07.bin", 0x0500, 0x0100, 0xcb9394fc );	/* tile palette bank */
	ROM_LOAD( "09f_g-09.bin", 0x0600, 0x0100, 0x3cee181e );	/* sprite lookup table */
	ROM_LOAD( "08f_g-08.bin", 0x0700, 0x0100, 0xef91cdd2 );	/* sprite palette bank */
return ROM_END;
        }
        public driver_gunsmrom()
        {
            drv = new driver_gunsmoke.machine_driver_gunsmoke();
            year = "1985";
            name = "gunsmrom";
            description = "Gun.Smoke (US set 1)";
            manufacturer = "Capcom  (Romstar license)";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = driver_gunsmoke.input_ports_gunsmoke();
            rom = rom_gunsmrom();
            drv.HasNVRAMhandler = false;
        }
    }
}