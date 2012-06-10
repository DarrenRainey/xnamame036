using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_mpatrol : Mame.GameDriver
    {
        const byte BGHEIGHT = 64;
        static byte[] scrollreg = new byte[16];
        static byte bg1xpos, bg1ypos, bg2xpos, bg2ypos, bgcontrol;
        static Mame.osd_bitmap[] bgbitmap = new Mame.osd_bitmap[3];
        static int flipscreen;

        static Mame.GfxLayout charlayout = new Mame.GfxLayout(
    8, 8,    /* 8*8 characters */
    512,    /* 512 characters */
    2,      /* 2 bits per pixel */
    new uint[] { 0, 512 * 8 * 8 }, /* the two bitplanes are separated */
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8     /* every char takes 8 consecutive bytes */
);
        static Mame.GfxLayout spritelayout = new Mame.GfxLayout
        (
            16, 16,  /* 16*16 sprites */
            128,    /* 128 sprites */
            2,      /* 2 bits per pixel */
            new uint[] { 0, 128 * 16 * 16 },       /* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 16 * 8 + 0, 16 * 8 + 1, 16 * 8 + 2, 16 * 8 + 3, 16 * 8 + 4, 16 * 8 + 5, 16 * 8 + 6, 16 * 8 + 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 8 * 8, 9 * 8, 10 * 8, 11 * 8, 12 * 8, 13 * 8, 14 * 8, 15 * 8 },
            32 * 8    /* every sprite takes 32 consecutive bytes */
        );
        static Mame.GfxLayout bgcharlayout = new Mame.GfxLayout
        (
            32, 32,  /* 32*32 characters (actually, it is just 1 big 256x64 image) */
            8,      /* 8 characters */
            2,      /* 2 bits per pixel */
            new uint[] { 4, 0 },       /* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3, 2 * 8 + 0, 2 * 8 + 1, 2 * 8 + 2, 2 * 8 + 3, 3 * 8 + 0, 3 * 8 + 1, 3 * 8 + 2, 3 * 8 + 3, 4 * 8 + 0, 4 * 8 + 1, 4 * 8 + 2, 4 * 8 + 3, 5 * 8 + 0, 5 * 8 + 1, 5 * 8 + 2, 5 * 8 + 3, 6 * 8 + 0, 6 * 8 + 1, 6 * 8 + 2, 6 * 8 + 3, 7 * 8 + 0, 7 * 8 + 1, 7 * 8 + 2, 7 * 8 + 3 },
            new uint[] { 0 * 512, 1 * 512, 2 * 512, 3 * 512, 4 * 512, 5 * 512, 6 * 512, 7 * 512, 8 * 512, 9 * 512, 10 * 512, 11 * 512, 12 * 512, 13 * 512, 14 * 512, 15 * 512, 16 * 512, 17 * 512, 18 * 512, 19 * 512, 20 * 512, 21 * 512, 22 * 512, 23 * 512, 24 * 512, 25 * 512, 26 * 512, 27 * 512, 28 * 512, 29 * 512, 30 * 512, 31 * 512 },
            8 * 8
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo = 
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, charlayout,               0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000, spritelayout,          64*4, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x0000, bgcharlayout, 64*4+16*4+0*4,  1 ),	/* top half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0x0800, bgcharlayout, 64*4+16*4+0*4,  1 ),	/* bottom half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX4, 0x0000, bgcharlayout, 64*4+16*4+1*4,  1 ),	/* top half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX4, 0x0800, bgcharlayout, 64*4+16*4+1*4,  1 ),	/* bottom half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX5, 0x0000, bgcharlayout, 64*4+16*4+2*4,  1 ),	/* top half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX5, 0x0800, bgcharlayout, 64*4+16*4+2*4,  1 ),	/* bottom half */
};

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8800, 0x8800, mpatrol_protection_r ),
	new Mame.MemoryReadAddress( 0xd000, 0xd000, Mame.input_port_0_r ),          /* IN0 */
	new Mame.MemoryReadAddress( 0xd001, 0xd001, Mame.input_port_1_r ),          /* IN1 */
	new Mame.MemoryReadAddress( 0xd002, 0xd002, Mame.input_port_2_r ),          /* IN2 */
	new Mame.MemoryReadAddress( 0xd003, 0xd003, mpatrol_input_port_3_r ),  /* DSW1 */
	new Mame.MemoryReadAddress( 0xd004, 0xd004, Mame.input_port_4_r ),          /* DSW2 */
	new Mame.MemoryReadAddress( 0xe000, 0xe7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xc820, 0xc87f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size),
	new Mame.MemoryWriteAddress( 0xc8a0, 0xc8ff, Mame.MWA_RAM, Generic.spriteram_2),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, irem.irem_sound_cmd_w ),
	new Mame.MemoryWriteAddress( 0xd001, 0xd001, mpatrol_flipscreen_w ),	/* + coin counters */
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1)  /* end of table */
};

        /* this looks like some kind of protection. The game does strange things */
        /* if a read from this address doesn't return the value it expects. */
        static int mpatrol_protection_r(int offset)
        {
            //if (errorlog) fprintf(errorlog,"%04x: read protection\n",cpu_get_pc());
            return (int)(Mame.cpu_get_reg(Mame.cpu_Z80.Z80_DE) & 0xff);
        }

        static Mame.IOWritePort[] writeport =
{
	new   Mame.IOWritePort( 0x10, 0x1f, mpatrol_scroll_w ),
	new   Mame.IOWritePort( 0x40, 0x40, mpatrol_bg1xpos_w ),
	new   Mame.IOWritePort( 0x60, 0x60, mpatrol_bg1ypos_w ),
	new   Mame.IOWritePort( 0x80, 0x80, mpatrol_bg2xpos_w ),
	new   Mame.IOWritePort( 0xa0, 0xa0, mpatrol_bg2ypos_w ),
	new   Mame.IOWritePort( 0xc0, 0xc0, mpatrol_bgcontrol_w ),
	new   Mame.IOWritePort( -1 )  /* end of table */
};

        static void mpatrol_scroll_w(int offset, int data)
        {
            scrollreg[offset] = (byte)data;
        }

        static void mpatrol_bg1xpos_w(int offset, int data)
        {
            bg1xpos = (byte)data;
        }
        static void mpatrol_bg1ypos_w(int offset, int data)
        {
            bg1ypos = (byte)data;
        }
        static void mpatrol_bg2xpos_w(int offset, int data)
        {
            bg2xpos = (byte)data;
        }
        static void mpatrol_bg2ypos_w(int offset, int data)
        {
            bg2ypos = (byte)data;
        }
        static void mpatrol_bgcontrol_w(int offset, int data)
        {
            bgcontrol = (byte)data;
        }
        static void mpatrol_flipscreen_w(int offset, int data)
        {
            /* screen flip is handled both by software and hardware */
            data ^= ~Mame.readinputport(4) & 1;

            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }

            Mame.coin_counter_w(0, data & 0x02);
            Mame.coin_counter_w(1, data & 0x20);
        }

        static int mpatrol_input_port_3_r(int offset)
        {
            int ret = Mame.input_port_3_r(0);

            /* Based on the coin mode fill in the upper bits */
            if ((Mame.input_port_4_r(0) & 0x04) != 0)
            {
                /* Mode 1 */
                ret |= (Mame.input_port_5_r(0) << 4);
            }
            else
            {
                /* Mode 2 */
                ret |= (Mame.input_port_5_r(0) & 0xf0);
            }

            return ret;
        }



        static void get_clip(Mame.rectangle clip, int min_y, int max_y)
        {
            clip.min_x = Mame.Machine.drv.visible_area.min_x;
            clip.max_x = Mame.Machine.drv.visible_area.max_x;

            if (flipscreen != 0)
            {
                clip.min_y = Mame.Machine.drv.screen_height - 1 - max_y;
                clip.max_y = Mame.Machine.drv.screen_height - 1 - min_y;
            }
            else
            {
                clip.min_y = min_y;
                clip.max_y = max_y;
            }
        }

        class machine_driver_mpatrol : Mame.MachineDriver
        {
            public machine_driver_mpatrol()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, writemem, null, writeport, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6803 | Mame.CPU_AUDIO_CPU, 6000000 / 4, irem.irem_sound_readmem, irem.irem_sound_writemem, irem.irem_sound_readport, irem.irem_sound_writeport, null, 0));
                frames_per_second = 57;
                vblank_duration = 1790;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 1 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_mpatrol.gfxdecodeinfo;
                total_colors = 128 + 32 + 32;
                color_table_len = 64 * 4 + 16 * 4 + 3 * 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;

                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, irem.irem_ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_MSM5205, irem.irem_msm5205_interface));
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
                uint cpi = 0, pi = 0;
                /* character palette */
                for (int i = 0; i < 128; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                cpi += 128;	/* skip the bottom half of the PROM - not used */
                /* color_prom now points to the beginning of the background palette */


                /* character lookup table */
                for (int i = 0; i < TOTAL_COLORS(0) / 2; i++)
                {
                    COLOR(colortable, 0, i, i);

                    /* also create a color code with transparent pen 0 */
                    if (i % 4 == 0) COLOR(colortable, 0, i + TOTAL_COLORS(0) / 2, 0);
                    else COLOR(colortable, 0, i + TOTAL_COLORS(0) / 2, i);
                }


                /* background palette */
                /* reserve one color for the transparent pen (none of the game colors can have */
                /* these RGB components) */
                palette[pi++] = 1;
                palette[pi++] = 1;
                palette[pi++] = 1;
                cpi++;

                for (int i = 1; i < 32; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 0x01;
                    bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                /* color_prom now points to the beginning of the sprite palette */

                /* sprite palette */
                for (int i = 0; i < 32; i++)
                {
                    /* red component */
                    int bit0 = 0;
                    int bit1 = (color_prom[cpi] >> 6) & 0x01;
                    int bit2 = (color_prom[cpi] >> 7) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 0x01;
                    bit1 = (color_prom[cpi] >> 4) & 0x01;
                    bit2 = (color_prom[cpi] >> 5) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    /* blue component */
                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    cpi++;
                }

                /* color_prom now points to the beginning of the sprite lookup table */

                /* sprite lookup table */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    COLOR(colortable, 1, i, 128 + 32 + (color_prom[cpi++]));
                    if (i % 4 == 3) cpi += 4;	/* half of the PROM is unused */
                }

                cpi += 128;	/* skip the bottom half of the PROM - not used */

                /* background */
                /* the palette is a 32x8 PROM with many colors repeated. The address of */
                /* the colors to pick is as follows: */
                /* xbb00: mountains */
                /* 0xxbb: hills */
                /* 1xxbb: city */
                COLOR(colortable, 2, 0, 128);
                COLOR(colortable, 2, 1, 128 + 4);
                COLOR(colortable, 2, 2, 128 + 8);
                COLOR(colortable, 2, 3, 128 + 12);
                COLOR(colortable, 4, 0, 128);
                COLOR(colortable, 4, 1, 128 + 1);
                COLOR(colortable, 4, 2, 128 + 2);
                COLOR(colortable, 4, 3, 128 + 3);
                COLOR(colortable, 6, 0, 128);
                COLOR(colortable, 6, 1, 128 + 16 + 1);
                COLOR(colortable, 6, 2, 128 + 16 + 2);
                COLOR(colortable, 6, 3, 128 + 16 + 3);
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0)
                    return 1;

                /* prepare the background graphics */
                for (int i = 0; i < 3; i++)
                {
                    /* temp bitmap for the three background images */
                    bgbitmap[i] = Mame.osd_create_bitmap(256, BGHEIGHT);
                    for (int j = 0; j < 8; j++)
                    {
                        Mame.drawgfx(bgbitmap[i], Mame.Machine.gfx[2 + 2 * i],
                                (uint)j, 0,
                                false, false,
                                32 * j, 0,
                                null, Mame.TRANSPARENCY_NONE, 0);

                        Mame.drawgfx(bgbitmap[i], Mame.Machine.gfx[2 + 2 * i + 1],
                                (uint)j, 0,
                                false, false,
                                32 * j, (BGHEIGHT / 2),
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(bgbitmap[0]);
                Mame.osd_free_bitmap(bgbitmap[1]);
                Mame.osd_free_bitmap(bgbitmap[2]);
                Generic.generic_vh_stop();
            }
            public override void vh_eof_callback()
            {
                //none
            }
            static void get_clip(Mame.rectangle clip, int min_y, int max_y)
            {
                clip.min_x = Mame.Machine.drv.visible_area.min_x;
                clip.max_x = Mame.Machine.drv.visible_area.max_x;

                if (flipscreen != 0)
                {
                    clip.min_y = Mame.Machine.drv.screen_height - 1 - max_y;
                    clip.max_y = Mame.Machine.drv.screen_height - 1 - min_y;
                }
                else
                {
                    clip.min_y = min_y;
                    clip.max_y = max_y;
                }
            }

            static void draw_background(Mame.osd_bitmap bitmap, int xpos, int ypos, int ypos_end, int image, int transparency)
            {
                Mame.rectangle clip1 = new Mame.rectangle(), clip2 = new Mame.rectangle();

                get_clip(clip1, ypos, ypos + BGHEIGHT - 1);
                get_clip(clip2, ypos + BGHEIGHT, ypos_end);

                if (flipscreen != 0)
                {
                    xpos = 256 - xpos;
                    ypos = Mame.Machine.drv.screen_height - BGHEIGHT - ypos;
                }
                Mame.copybitmap(bitmap, bgbitmap[image], flipscreen != 0, flipscreen != 0, xpos, ypos, clip1, transparency, 128);
                Mame.copybitmap(bitmap, bgbitmap[image], flipscreen != 0, flipscreen != 0, xpos - 256, ypos, clip1, transparency, 128);
                Mame.fillbitmap(bitmap, Mame.Machine.gfx[image * 2 + 2].colortable.read16(3), clip2);
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;

                        int color = Generic.colorram[offs] & 0x1f;
                        if (sy >= 7) color += 32;	/* lines 7-31 are transparent */

                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 2 * (Generic.colorram[offs] & 0x80)),
                                (uint)color,
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the background */
                if ((bgcontrol == 0x04) || (bgcontrol == 0x03))
                {
                    Mame.rectangle clip = new Mame.rectangle();

                    get_clip(clip, 7 * 8, bg2ypos - 1);
                    Mame.fillbitmap(bitmap, Mame.Machine.pens.read16(0), clip);

                    draw_background(bitmap, bg2xpos, bg2ypos, bg1ypos + BGHEIGHT - 1, 0, Mame.TRANSPARENCY_NONE);
                    draw_background(bitmap, bg1xpos, bg1ypos, Mame.Machine.drv.visible_area.max_y,(bgcontrol == 0x04) ? 1 : 2, Mame.TRANSPARENCY_COLOR);
                }
                else Mame.fillbitmap(bitmap, Mame.Machine.pens.read16(0), Mame.Machine.drv.visible_area);


                /* copy the temporary bitmap to the screen */
                {
                    int[] scroll = new int[32];
                    Mame.rectangle clip = new Mame.rectangle();

                    clip.min_x = Mame.Machine.drv.visible_area.min_x;
                    clip.max_x = Mame.Machine.drv.visible_area.max_x;

                    if (flipscreen != 0)
                    {
                        clip.min_y = 25 * 8;
                        clip.max_y = 32 * 8 - 1;
                        Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, clip, Mame.TRANSPARENCY_NONE, 0);

                        clip.min_y = 0;
                        clip.max_y = 25 * 8 - 1;

                        for (int i = 0; i < 32; i++)
                            scroll[31 - i] = -scrollreg[i / 2];

                        Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 32, scroll, 0, null, clip, Mame.TRANSPARENCY_COLOR, 0);
                    }
                    else
                    {
                        clip.min_y = 0;
                        clip.max_y = 7 * 8 - 1;
                        Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, clip, Mame.TRANSPARENCY_NONE, 0);

                        clip.min_y = 7 * 8;
                        clip.max_y = 32 * 8 - 1;

                        for (int i = 0; i < 32; i++)
                            scroll[i] = scrollreg[i / 2];

                        Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 32, scroll, 0, null, clip, Mame.TRANSPARENCY_COLOR, 0);
                    }
                }


                /* Draw the sprites. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int sx = Generic.spriteram_2[offs + 3];
                    int sy = 241 - Generic.spriteram_2[offs];
                    bool flipx = (Generic.spriteram_2[offs + 1] & 0x40) != 0;
                    bool flipy = (Generic.spriteram_2[offs + 1] & 0x80) != 0;
                    if (flipscreen != 0)
                    {
                        flipx = !flipx;
                        flipy = !flipy;
                        sx = 240 - sx;
                        sy = 242 - sy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            Generic.spriteram_2[offs + 2],
                            (uint)(Generic.spriteram_2[offs + 1] & 0x3f),
                            flipx, flipy,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 128 + 32);
                }
                
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int sx = Generic.spriteram[offs + 3];
                    int sy = 241 - Generic.spriteram[offs];
                    bool flipx = (Generic.spriteram[offs + 1] & 0x40) != 0;
                    bool flipy = (Generic.spriteram[offs + 1] & 0x80) != 0;
                    if (flipscreen != 0)
                    {
                        flipx = !flipx;
                        flipy = !flipy;
                        sx = 240 - sx;
                        sy = 242 - sy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            Generic.spriteram[offs + 2],
                            (uint)(Generic.spriteram[offs + 1] & 0x3f),
                            flipx, flipy,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 128 + 32);
                }
            }
        }
        Mame.InputPortTiny[] input_ports_mpatrol()
        {

            INPUT_PORTS_START("mpatrol");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            /* coin input must be active for ? frames to be consistently recognized */
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3, 17);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x02, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x00, "1");
            PORT_DIPSETTING(0x01, "2");
            PORT_DIPSETTING(0x02, "3");
            PORT_DIPSETTING(0x03, "5");
            PORT_DIPNAME(0x0c, 0x0c, ipdn_defaultstrings["Bonus Life"]);
            PORT_DIPSETTING(0x0c, "10000 30000 50000");
            PORT_DIPSETTING(0x08, "20000 40000 60000");
            PORT_DIPSETTING(0x04, "10000");
            PORT_DIPSETTING(0x00, "None");
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);  /* Gets filled in based on the coin mode */

            PORT_START("DSW1");
            PORT_DIPNAME(0x01, 0x01, ipdn_defaultstrings["Flip Screen"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_DIPNAME(0x02, 0x00, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["Cocktail"]);
            PORT_DIPNAME(0x04, 0x04, "Coin Mode");
            PORT_DIPSETTING(0x04, "Mode 1");
            PORT_DIPSETTING(0x00, "Mode 2");
            PORT_DIPNAME(0x08, 0x08, ipdn_defaultstrings["Unknown"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            /* In stop mode, press 2 to stop and 1 to restart */
            PORT_BITX(0x10, 0x10, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Stop Mode", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_BITX(0x20, 0x20, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Sector Selection", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_BITX(0x40, 0x40, (int)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);

            /* Fake port to support the two different coin modes */
            PORT_START();
            PORT_DIPNAME(0x0f, 0x0f, "Coinage Mode 1");  /* mapped on coin mode 1 */
            PORT_DIPSETTING(0x09, ipdn_defaultstrings["7C_1C"]);
            PORT_DIPSETTING(0x0a, ipdn_defaultstrings["6C_1C"]);
            PORT_DIPSETTING(0x0b, ipdn_defaultstrings["5C_1C"]);
            PORT_DIPSETTING(0x0c, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x0d, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x0e, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x0f, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x06, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x05, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x04, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["1C_7C"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["1C_8C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Free Play"]);
            PORT_DIPNAME(0x30, 0x30, "Coin A  Mode 2");   /* mapped on coin mode 2 */
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x20, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x30, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Free Play"]);
            PORT_DIPNAME(0xc0, 0xc0, "Coin B  Mode 2");
            PORT_DIPSETTING(0xc0, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x40, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["1C_6C"]);
            return INPUT_PORTS_END;

        }
        Mame.RomModule[] rom_mpatrol()
        {
            ROM_START("mpatrol");
            ROM_REGION(0x10000, Mame.REGION_CPU1);     /* 64k for code */
            ROM_LOAD("mp-a.3m", 0x0000, 0x1000, 0x5873a860);
            ROM_LOAD("mp-a.3l", 0x1000, 0x1000, 0xf4b85974);
            ROM_LOAD("mp-a.3k", 0x2000, 0x1000, 0x2e1a598c);
            ROM_LOAD("mp-a.3j", 0x3000, 0x1000, 0xdd05b587);

            ROM_REGION(0x10000, Mame.REGION_CPU2);     /* 64k for code */
            ROM_LOAD("mp-snd.1a", 0xf000, 0x1000, 0x561d3108);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mp-e.3e", 0x0000, 0x1000, 0xe3ee7f75);       /* chars */
            ROM_LOAD("mp-e.3f", 0x1000, 0x1000, 0xcca6d023);

            ROM_REGION(0x2000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mp-b.3m", 0x0000, 0x1000, 0x707ace5e);      /* sprites */
            ROM_LOAD("mp-b.3n", 0x1000, 0x1000, 0x9b72133a);

            ROM_REGION(0x1000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mp-e.3l", 0x0000, 0x1000, 0xc46a7f72);      /* background graphics */

            ROM_REGION(0x1000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mp-e.3k", 0x0000, 0x1000, 0xc7aa1fb0);

            ROM_REGION(0x1000, Mame.REGION_GFX5 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("mp-e.3h", 0x0000, 0x1000, 0xa0919392);

            ROM_REGION(0x0240, Mame.REGION_PROMS);
            ROM_LOAD("2a", 0x0000, 0x0100, 0x0f193a50); /* character palette */
            ROM_LOAD("1m", 0x0100, 0x0020, 0x6a57eff2); /* background palette */
            ROM_LOAD("1c1j", 0x0120, 0x0020, 0x26979b13); /* sprite palette */
            ROM_LOAD("2hx", 0x0140, 0x0100, 0x7ae4cd97); /* sprite lookup table */
            return ROM_END;
        }
        public override void driver_init()
        {
            //none
        }
        public driver_mpatrol()
        {
            drv = new machine_driver_mpatrol();
            year = "1982";
            name = "mpatrol";
            description = "Moon Patrol (Williams)";
            manufacturer = "Namco";
            flags = Mame.ROT0;
            input_ports = input_ports_mpatrol();
            rom = rom_mpatrol();
        }
    }
}
