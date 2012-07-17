using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_vulgus : Mame.GameDriver
    {
        static _BytePtr vulgus_bgvideoram = new _BytePtr(1);
        static _BytePtr vulgus_bgcolorram = new _BytePtr(1);
        static int[] vulgus_bgvideoram_size = new int[1];
        static _BytePtr vulgus_scrolllow = new _BytePtr(1);
        static _BytePtr vulgus_scrollhigh = new _BytePtr(1);
        static _BytePtr vulgus_palette_bank = new _BytePtr(1);
        static bool[] dirtybuffer2;
        static Mame.osd_bitmap tmpbitmap2;
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x9fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xc001, 0xc001, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xc002, 0xc002, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0xc003, 0xc003, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0xc004, 0xc004, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0xd000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x9fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xc802, 0xc803, Mame.MWA_RAM, vulgus_scrolllow ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, vulgus_control_w ),
	new Mame.MemoryWriteAddress( 0xc805, 0xc805, vulgus_palette_bank_w, vulgus_palette_bank ),
	new Mame.MemoryWriteAddress( 0xc902, 0xc903, Mame.MWA_RAM, vulgus_scrollhigh ),
	new Mame.MemoryWriteAddress( 0xcc00, 0xcc7f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xd800, 0xdbff, vulgus_bgvideoram_w, vulgus_bgvideoram, vulgus_bgvideoram_size ),
	new Mame.MemoryWriteAddress( 0xdc00, 0xdfff, vulgus_bgcolorram_w, vulgus_bgcolorram ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};



        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8001, 0x8001, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, ay8910.AY8910_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xc001, 0xc001, ay8910.AY8910_write_port_1_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,		/* 2 bits per pixel */
            new uint[] { 4, 0 },
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );
        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 tiles */
            512,	/* 512 tiles */
            3,	/* 3 bits per pixel */
            new uint[] { 0, 512 * 32 * 8, 2 * 512 * 32 * 8 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
		16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            32 * 8	/* every tile takes 32 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 256 * 64 * 8 + 4, 256 * 64 * 8, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8+0, 8+1, 8+2, 8+3,
			32*8+0, 32*8+1, 32*8+2, 32*8+3, 33*8+0, 33*8+1, 33*8+2, 33*8+3 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,           0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, tilelayout,  64*4+16*16, 32*4 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout,      64*4, 16 ),
};



        static AY8910interface ay8910_interface =
        new AY8910interface(
            2,	/* 2 chips */
            1500000,	/* 1.5 MHz ? */
            new int[] { 25, 25 },
            new AY8910portRead[] { null, null },
            new AY8910portRead[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null },
            new AY8910handler[] { null, null }
        );

        static void vulgus_bgvideoram_w(int offset, int data)
        {
            if (vulgus_bgvideoram[offset] != data)
            {
                dirtybuffer2[offset] = true;

                vulgus_bgvideoram[offset] = (byte)data;
            }
        }
        static void vulgus_bgcolorram_w(int offset, int data)
        {
            if (vulgus_bgcolorram[offset] != data)
            {
                dirtybuffer2[offset] = true;

                vulgus_bgcolorram[offset] = (byte)data;
            }
        }
        static void vulgus_palette_bank_w(int offset, int data)
        {
            if (vulgus_palette_bank[0] != data)
            {
                for (int i = 0; i < vulgus_bgvideoram_size[0]; i++)
                    dirtybuffer2[i] = true;
                vulgus_palette_bank[0] = (byte)data;
            }
        }

        static void vulgus_control_w(int offset, int data)
        {
            /* bit 0-1 coin counters */
            Mame.coin_counter_w(0, data & 1);
            Mame.coin_counter_w(1, data & 2);

            /* bit 7   flip screen
               in vulgus this is active LO, in vulgusj this is active HI !!! */
        }
        class machine_driver_vulgus : Mame.MachineDriver
        {
            public machine_driver_vulgus()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, null, null, driver_1942.c1942_interrupt, 2));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 8));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_vulgus.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 16 * 16 + 4 * 32 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910,ay8910_interface));
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
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] =(byte)( 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi+Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++]= (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi+2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                cpi += 2 * Mame.Machine.drv.total_colors;
                /* color_prom now points to the beginning of the lookup table */


                /* characters use colors 32-47 (?) */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable,0, i, color_prom[cpi++] + 32);

                /* sprites use colors 16-31 */
                for (int i = 0; i < TOTAL_COLORS(2); i++)
                {
                    COLOR(colortable, 2, i, color_prom[cpi++] + 16);
                }

                /* background tiles use colors 0-15, 64-79, 128-143, 192-207 in four banks */
                for (int i = 0; i < TOTAL_COLORS(1) / 4; i++)
                {
                    COLOR(colortable,1, i,color_prom[cpi]);
                    COLOR(colortable,1, i + 32 * 8,color_prom[cpi] + 64);
                    COLOR(colortable,1, i + 2 * 32 * 8,color_prom[cpi] + 128);
                    COLOR(colortable,1, i + 3 * 32 * 8,color_prom[cpi] + 192);
                    cpi++;
                }
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0)
                    return 1;

                dirtybuffer2 = new bool[vulgus_bgvideoram_size[0]];
                for (int i = 0; i < vulgus_bgvideoram_size[0]; i++)
                    dirtybuffer2[i] = true;

                /* the background area is twice as tall and twice as large as the screen */
                tmpbitmap2 = Mame.osd_create_bitmap(2 * Mame.Machine.drv.screen_width, 2 * Mame.Machine.drv.screen_height);

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(tmpbitmap2);
                dirtybuffer2 = null;
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int[] scrolly ={ -(vulgus_scrolllow[0] + 256 * vulgus_scrollhigh[0])};
                int []scrollx = {-(vulgus_scrolllow[1] + 256 * vulgus_scrollhigh[1])};

                for (int offs = vulgus_bgvideoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (dirtybuffer2[offs])
                    {
                        //			int minx,maxx,miny,maxy;

                        int sx = (offs % 32);
                        int sy = (offs / 32);

                        /* between level Vulgus changes the palette bank every frame. Redrawing */
                        /* the whole background every time would slow the game to a crawl, so here */
                        /* we check and redraw only the visible tiles */
                        /*
                                    minx = (sx + scrollx) & 0x1ff;
                                    maxx = (sx + 15 + scrollx) & 0x1ff;
                                    if (minx > maxx) minx = maxx - 15;
                                    miny = (sy + scrolly) & 0x1ff;
                                    maxy = (sy + 15 + scrolly) & 0x1ff;
                                    if (miny > maxy) miny = maxy - 15;

                                    if (minx + 15 >= Machine.drv.visible_area.min_x &&
                                            maxx - 15 <= Machine.drv.visible_area.max_x &&
                                            miny + 15 >= Machine.drv.visible_area.min_y &&
                                            maxy - 15 <= Machine.drv.visible_area.max_y)
                        */
                        {
                            dirtybuffer2[offs] = false;

                            Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[1],
                                    (uint)(vulgus_bgvideoram[offs] + 2 * (vulgus_bgcolorram[offs] & 0x80)),
                                    (uint)((vulgus_bgcolorram[offs] & 0x1f) + 32 * vulgus_palette_bank[0]),
                                    (vulgus_bgcolorram[offs] & 0x20)!=0, (vulgus_bgcolorram[offs] & 0x40)!=0,
                                    16 * sy, 16 * sx,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }


                /* copy the background graphics */
                Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, scrollx, 1, scrolly, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int code = Generic.spriteram[offs];
                    int col = Generic.spriteram[offs + 1] & 0x0f;
                    int sx = Generic.spriteram[offs + 3];
                    int sy = Generic.spriteram[offs + 2];

                    int i = (Generic.spriteram[offs + 1] & 0xc0) >> 6;
                    if (i == 2) i = 3;

                    do
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                (uint)(code + i),
                                (uint)col,
                                false,false,
                                sx, sy + 16 * i,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);

                        /* draw again with wraparound */
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                (uint)(code + i),
                                (uint)col,
                                false,false,
                                sx, sy + 16 * i - 256,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);
                        i--;
                    } while (i >= 0);
                }


                /* draw the frontmost playfield. They are characters, but draw them as sprites */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx = 8 * (offs % 32);
                    int sy = 8 * (offs / 32);

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            (uint)(Generic.videoram[offs] + 2 * (Generic.colorram[offs] & 0x80)),
                            (uint)(Generic.colorram[offs] & 0x3f),
                            false,false,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 47);
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
        Mame.RomModule[] rom_vulgus()
        {
            ROM_START("vulgus");
            ROM_REGION(0x1c000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("v2", 0x0000, 0x2000, 0x3e18ff62);
            ROM_LOAD("v3", 0x2000, 0x2000, 0xb4650d82);
            ROM_LOAD("v4", 0x4000, 0x2000, 0x5b26355c);
            ROM_LOAD("v5", 0x6000, 0x2000, 0x4ca7f10e);
            ROM_LOAD("1-8n.bin", 0x8000, 0x2000, 0x6ca5ca41);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("1-11c.bin", 0x0000, 0x2000, 0x3bd2acf4);

            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1-3d.bin", 0x00000, 0x2000, 0x8bc5d7a5);/* characters */

            ROM_REGION(0x0c000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2-2a.bin", 0x00000, 0x2000, 0xe10aaca1);/* tiles */
            ROM_LOAD("2-3a.bin", 0x02000, 0x2000, 0x8da520da);
            ROM_LOAD("2-4a.bin", 0x04000, 0x2000, 0x206a13f1);
            ROM_LOAD("2-5a.bin", 0x06000, 0x2000, 0xb6d81984);
            ROM_LOAD("2-6a.bin", 0x08000, 0x2000, 0x5a26b38f);
            ROM_LOAD("2-7a.bin", 0x0a000, 0x2000, 0x1e1ca773);

            ROM_REGION(0x08000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2-2n.bin", 0x00000, 0x2000, 0x6db1b10d);/* sprites */
            ROM_LOAD("2-3n.bin", 0x02000, 0x2000, 0x5d8c34ec);
            ROM_LOAD("2-4n.bin", 0x04000, 0x2000, 0x0071a2e3);
            ROM_LOAD("2-5n.bin", 0x06000, 0x2000, 0x4023a1ec);

            ROM_REGION(0x0600, Mame.REGION_PROMS);
            ROM_LOAD("e8.bin", 0x0000, 0x0100, 0x06a83606);	/* red component */
            ROM_LOAD("e9.bin", 0x0100, 0x0100, 0xbeacf13c);	/* green component */
            ROM_LOAD("e10.bin", 0x0200, 0x0100, 0xde1fb621);	/* blue component */
            ROM_LOAD("d1.bin", 0x0300, 0x0100, 0x7179080d);	/* char lookup table */
            ROM_LOAD("j2.bin", 0x0400, 0x0100, 0xd0842029);	/* sprite lookup table */
            ROM_LOAD("c9.bin", 0x0500, 0x0100, 0x7a1f0bd6);	/* tile lookup table */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_vulgus()
        {

            INPUT_PORTS_START("vulgus");
            PORT_START();    /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);   /* probably unused */
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START();    /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();     /* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();    /* DSW0 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x01, "1");
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x00, "5");
            /* these are the settings for the second coin input, but it seems that the */
            /* game only supports one */
            PORT_DIPNAME(0x1c, 0x1c, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x10, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x18, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x1c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x14, DEF_STR("1C_3C"));
            /*	PORT_DIPSETTING(    0x00, "Invalid" ) disables both coins */
            PORT_DIPNAME(0xe0, 0xe0, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x80, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x40, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0xa0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));

            PORT_START();    /* DSW1 */
            /* not sure about difficulty
               Code perform a read and (& 0x03). NDMix*/
            PORT_DIPNAME(0x03, 0x03, "Difficulty?");
            PORT_DIPSETTING(0x02, "Easy?");
            PORT_DIPSETTING(0x03, "Normal?");
            PORT_DIPSETTING(0x01, "Hard?");
            PORT_DIPSETTING(0x00, "Hardest?");
            PORT_DIPNAME(0x04, 0x04, "Demo Music");
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));
            PORT_DIPNAME(0x70, 0x70, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x30, "10000 50000");
            PORT_DIPSETTING(0x50, "10000 60000");
            PORT_DIPSETTING(0x10, "10000 70000");
            PORT_DIPSETTING(0x70, "20000 60000");
            PORT_DIPSETTING(0x60, "20000 70000");
            PORT_DIPSETTING(0x20, "20000 80000");
            PORT_DIPSETTING(0x40, "30000 70000");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x80, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x80, DEF_STR(Cocktail));
            return INPUT_PORTS_END;
        }
        public driver_vulgus()
        {
            drv = new machine_driver_vulgus();
            year = "1984";
            name = "vulgus";
            description = "Vulgus (set 1)";
            manufacturer = "Capcom";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_vulgus();
            rom = rom_vulgus();
            drv.HasNVRAMhandler = false;
        }
    }
}
