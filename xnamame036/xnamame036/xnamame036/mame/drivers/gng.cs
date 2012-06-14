using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_gng : Mame.GameDriver
    {
        static _BytePtr gng_fgvideoram = new _BytePtr(1);
        static _BytePtr gng_fgcolorram = new _BytePtr(1);
        static _BytePtr gng_bgvideoram = new _BytePtr(1);
        static _BytePtr gng_bgcolorram = new _BytePtr(1);
        static Mame.tilemap bg_tilemap, fg_tilemap;
        static int flipscreen;
        
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x2fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x3000, 0x3000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x3001, 0x3001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x3002, 0x3002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x3003, 0x3003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x3004, 0x3004, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0x3c00, 0x3c00, Mame.MRA_NOP ),    /* watchdog? */
	new Mame.MemoryReadAddress( 0x4000, 0x5fff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x6000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1dff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1e00, 0x1fff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x2000, 0x23ff, gng_fgvideoram_w, gng_fgvideoram ),
	new Mame.MemoryWriteAddress( 0x2400, 0x27ff, gng_fgcolorram_w, gng_fgcolorram ),
	new Mame.MemoryWriteAddress( 0x2800, 0x2bff, gng_bgvideoram_w, gng_bgvideoram ),
	new Mame.MemoryWriteAddress( 0x2c00, 0x2fff, gng_bgcolorram_w, gng_bgcolorram ),
	new Mame.MemoryWriteAddress( 0x3800, 0x38ff, Mame.paletteram_RRRRGGGGBBBBxxxx_split2_w, Mame.paletteram_2 ),
	new Mame.MemoryWriteAddress( 0x3900, 0x39ff, Mame.paletteram_RRRRGGGGBBBBxxxx_split1_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x3a00, 0x3a00, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x3b08, 0x3b09, gng_bgscrollx_w ),
	new Mame.MemoryWriteAddress( 0x3b0a, 0x3b0b, gng_bgscrolly_w ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3c00, Mame.MWA_NOP ),   /* watchdog? */
	new Mame.MemoryWriteAddress( 0x3d00, 0x3d00, gng_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x3e00, 0x3e00, gng_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};



static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0xc000, 0xc7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc800, 0xc800, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0xc000, 0xc7ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xe001, 0xe001, ym2203.YM2203_write_port_0_w),
	new Mame.MemoryWriteAddress( 0xe002, 0xe002, ym2203.YM2203_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xe003, 0xe003, ym2203.YM2203_write_port_1_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1)	/* end of table */
};
        
static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
	8,8,	/* 8*8 characters */
	1024,	/* 1024 characters */
	2,	/* 2 bits per pixel */
	new uint[]{ 4, 0 },
	new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3 },
	new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16 },
	16*8	/* every char takes 16 consecutive bytes */
);
static Mame.GfxLayout tilelayout =
new Mame.GfxLayout(
	16,16,	/* 16*16 tiles */
	1024,	/* 1024 tiles */
	3,	/* 3 bits per pixel */
	new uint[]{ 2*1024*32*8, 1024*32*8, 0 },	/* the bitplanes are separated */
        new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
            16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
	32*8	/* every tile takes 32 consecutive bytes */
);
static Mame.GfxLayout spritelayout =
new Mame.GfxLayout(
	16,16,	/* 16*16 sprites */
	768,	/* 768 sprites */
	4,	/* 4 bits per pixel */
	new uint[]{ 768*64*8+4, 768*64*8+0, 4, 0 },
        new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
	    32*8+0, 32*8+1, 32*8+2, 32*8+3, 33*8+0, 33*8+1, 33*8+2, 33*8+3 },
	new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
	64*8	/* every sprite takes 64 consecutive bytes */
);
        
static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,   128, 16 ),	/* colors 128-195 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tilelayout,     0,  8 ),	/* colors   0- 63 */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout,  64,  4 ),	/* colors  64-127 */
};



static  YM2203interface ym2203_interface =
new YM2203interface(
	2,			/* 2 chips */
	1500000,	/* 1.5 MHz (?) */
	new int[]{ ym2203.YM2203_VOL(10,40), ym2203.YM2203_VOL(10,40) },
	new AY8910portRead[]{null,null },
	new AY8910portRead[]{null,null },
	new AY8910portWrite[]{null,null },
	new AY8910portWrite[]{null,null },
    new AY8910handler[]{null,null}
);


        static void gng_bankswitch_w(int offset,int data)
{
	_BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


	int[] bank = { 0x10000, 0x12000, 0x14000, 0x16000, 0x04000, 0x18000 };
	Mame.cpu_setbank (1, new _BytePtr(RAM, bank[data]));
}
static void gng_fgvideoram_w(int offset, int data)
{
    if (gng_fgvideoram[offset] != data)
    {
        gng_fgvideoram[offset] = (byte)data;
        Mame.tilemap_mark_tile_dirty(fg_tilemap, offset % 32, offset / 32);
    }
}

static void gng_fgcolorram_w(int offset, int data)
{
    if (gng_fgcolorram[offset] != data)
    {
        gng_fgcolorram[offset] = (byte)data;
        Mame.tilemap_mark_tile_dirty(fg_tilemap, offset % 32, offset / 32);
    }
}

static void gng_bgvideoram_w(int offset, int data)
{
    if (gng_bgvideoram[offset] != data)
    {
        gng_bgvideoram[offset] = (byte)data;
        Mame.tilemap_mark_tile_dirty(bg_tilemap, offset / 32, offset % 32);
    }
}

static void gng_bgcolorram_w(int offset, int data)
{
    if (gng_bgcolorram[offset] != data)
    {
        gng_bgcolorram[offset] = (byte)data;
        Mame.tilemap_mark_tile_dirty(bg_tilemap, offset / 32, offset % 32);
    }
}
static byte[] scrollx = new byte[2], scrolly = new byte[2];
        static void gng_bgscrollx_w(int offset, int data)
{
	scrollx[offset] = (byte)data;
	Mame.tilemap_set_scrollx( bg_tilemap, 0, scrollx[0] + 256 * scrollx[1] );
}

static void gng_bgscrolly_w(int offset, int data)
{
    scrolly[offset] = (byte)data;
    Mame.tilemap_set_scrolly(bg_tilemap, 0, scrolly[0] + 256 * scrolly[1]);
}


static void gng_flipscreen_w(int offset, int data)
{
    flipscreen = ~data & 1;
    Mame.tilemap_set_flip(Mame.ALL_TILEMAPS, flipscreen != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
}



        class machine_driver_gng : Mame.MachineDriver
    {
        public machine_driver_gng()
        {
            cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1500000, readmem, writemem, null, null, Mame.interrupt, 1));
            cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 4));
            frames_per_second = 60;
            vblank_duration = 2500;
            cpu_slices_per_frame = 1;
            screen_width = 32 * 8;
            screen_height = 32 * 8;
            visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
            gfxdecodeinfo = driver_gng.gfxdecodeinfo;
            total_colors = 192;
            color_table_len = 192;
            video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_UPDATE_AFTER_VBLANK;
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
            fg_tilemap = Mame.tilemap_create(
        get_fg_tile_info,
        Mame.TILEMAP_TRANSPARENT,
        8, 8, /* tile width, tile height */
        32, 32 /* number of columns, number of rows */
    );

            bg_tilemap = Mame.tilemap_create(
                get_bg_tile_info,
                Mame.TILEMAP_SPLIT,
                16, 16,
                32, 32
            );

            if (fg_tilemap !=null&& bg_tilemap!=null)
            {
                fg_tilemap.transparent_pen = 3;

                bg_tilemap.transmask[0] = 0xff; /* split type 0 is totally transparent in front half */
                bg_tilemap.transmask[1] = 0x01; /* split type 1 has pen 1 transparent in front half */

                return 0;
            }

            return 1;
        }
        static void get_fg_tile_info(int col, int row)
{
	int tile_index = row*32+col;
	byte attr = gng_fgcolorram[tile_index];
    Mame.SET_TILE_INFO(0, gng_fgvideoram[tile_index] + ((attr & 0xc0) << 2), attr & 0x0f);
	Mame.tile_info.flags = (byte)Mame.TILE_FLIPYX((attr & 0x30) >> 4);
}

        static void get_bg_tile_info(int col, int row)
{
	int tile_index = col*32+row;
	byte attr = gng_bgcolorram[tile_index];
    Mame.SET_TILE_INFO(1, gng_bgvideoram[tile_index] + ((attr & 0xc0) << 2), attr & 0x07);
	Mame.tile_info.flags = (byte)(Mame.TILE_FLIPYX((attr & 0x30) >> 4) | Mame.TILE_SPLIT((attr & 0x08) >> 3));
}
        public override void vh_stop()
        {
            throw new NotImplementedException();
        }
        public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            Mame.tilemap_update(Mame.ALL_TILEMAPS);

            if (Mame.palette_recalc()!=null)
                Mame.tilemap_mark_all_pixels_dirty(Mame.ALL_TILEMAPS);

            Mame.tilemap_render(Mame.ALL_TILEMAPS);

            Mame.tilemap_draw(bitmap, bg_tilemap, Mame.TILEMAP_BACK);
            draw_sprites(bitmap);
            Mame.tilemap_draw(bitmap, bg_tilemap, Mame.TILEMAP_FRONT);
            Mame.tilemap_draw(bitmap, fg_tilemap, 0);
        }
        static void draw_sprites(Mame.osd_bitmap bitmap)
{
    Mame.GfxElement gfx = Mame.Machine.gfx[2];
	Mame.rectangle clip = Mame.Machine.drv.visible_area;

    for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
    {
        byte attributes = Generic.spriteram[offs + 1];
        int sx = Generic.spriteram[offs + 3] - 0x100 * (attributes & 0x01);
        int sy = Generic.spriteram[offs + 2];
		bool flipx = (attributes & 0x04)!=0;
		bool flipy = (attributes & 0x08)!=0;

		if (flipscreen!=0){
			sx = 240 - sx;
			sy = 240 - sy;
			flipx = !flipx;
			flipy = !flipy;
		}

        Mame.drawgfx(bitmap, gfx,
				(uint)(Generic.spriteram[offs] + ((attributes<<2) & 0x300)),
				(uint)((attributes >> 4) & 3),
				flipx,flipy,
				sx,sy,
                clip, Mame.TRANSPARENCY_PEN, 15);
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
        Mame.RomModule[] rom_gng()
        {

            ROM_START("gng");
            ROM_REGION(0x18000, Mame.REGION_CPU1);	/* 64k for code * 5 pages */
            ROM_LOAD("gg3.bin", 0x08000, 0x8000, 0x9e01c65e);
            ROM_LOAD("gg4.bin", 0x04000, 0x4000, 0x66606beb);	/* 4000-5fff is page 0 */
            ROM_LOAD("gg5.bin", 0x10000, 0x8000, 0xd6397b2b);	/* page 1, 2, 3 and 4 */

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for the audio CPU */
            ROM_LOAD("gg2.bin", 0x0000, 0x8000, 0x615f5b6f);  /* Audio CPU is a Z80 */

            ROM_REGION(0x04000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gg1.bin", 0x00000, 0x4000, 0xecfccf07);	/* characters */

            ROM_REGION(0x18000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gg11.bin", 0x00000, 0x4000, 0xddd56fa9);	/* tiles 0-1 Plane 1*/
            ROM_LOAD("gg10.bin", 0x04000, 0x4000, 0x7302529d);	/* tiles 2-3 Plane 1*/
            ROM_LOAD("gg9.bin", 0x08000, 0x4000, 0x20035bda);	/* tiles 0-1 Plane 2*/
            ROM_LOAD("gg8.bin", 0x0c000, 0x4000, 0xf12ba271);	/* tiles 2-3 Plane 2*/
            ROM_LOAD("gg7.bin", 0x10000, 0x4000, 0xe525207d);	/* tiles 0-1 Plane 3*/
            ROM_LOAD("gg6.bin", 0x14000, 0x4000, 0x2d77e9b2);	/* tiles 2-3 Plane 3*/

            ROM_REGION(0x18000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gg17.bin", 0x00000, 0x4000, 0x93e50a8f);	/* sprites 0 Plane 1-2 */
            ROM_LOAD("gg16.bin", 0x04000, 0x4000, 0x06d7e5ca);	/* sprites 1 Plane 1-2 */
            ROM_LOAD("gg15.bin", 0x08000, 0x4000, 0xbc1fe02d);	/* sprites 2 Plane 1-2 */
            ROM_LOAD("gg14.bin", 0x0c000, 0x4000, 0x6aaf12f9);	/* sprites 0 Plane 3-4 */
            ROM_LOAD("gg13.bin", 0x10000, 0x4000, 0xe80c3fca);	/* sprites 1 Plane 3-4 */
            ROM_LOAD("gg12.bin", 0x14000, 0x4000, 0x7780a925);	/* sprites 2 Plane 3-4 */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_gng()
        {

            INPUT_PORTS_START("gng");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* DSW0 */
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR(Coinage));
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
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            PORT_DIPNAME(0x10, 0x10, "Coinage affects");
            PORT_DIPSETTING(0x10, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x00, DEF_STR(Coin_B));
            PORT_DIPNAME(0x20, 0x00, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x40, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x04, DEF_STR(Cocktail));
            PORT_DIPNAME(0x18, 0x18, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x18, "20000 70000 70000");
            PORT_DIPSETTING(0x10, "30000 80000 80000");
            PORT_DIPSETTING(0x08, "20000 80000");
            PORT_DIPSETTING(0x00, "30000 80000");
            PORT_DIPNAME(0x60, 0x60, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x40, "Easy");
            PORT_DIPSETTING(0x60, "Normal");
            PORT_DIPSETTING(0x20, "Difficult");
            PORT_DIPSETTING(0x00, "Very Difficult");
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            return INPUT_PORTS_END;
        }
        public driver_gng()
        {
            drv = new machine_driver_gng();
            year = "1985";
            name = "gng";
            description = "Ghosts'n Goblins (World)";
            manufacturer = "Capcom";
            flags = Mame.ROT0;
            input_ports = input_ports_gng();
            rom = rom_gng();
            drv.HasNVRAMhandler = false;
        }
    }
}
