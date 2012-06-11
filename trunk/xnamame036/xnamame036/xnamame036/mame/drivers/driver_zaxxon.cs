using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_zaxxon : Mame.GameDriver
    {
        public static _BytePtr zaxxon_char_color_bank = new _BytePtr(1);
        public static _BytePtr zaxxon_background_color_bank = new _BytePtr(1);
        public static _BytePtr zaxxon_background_enable = new _BytePtr(1);
        public static _BytePtr zaxxon_background_position = new _BytePtr(1);
        static _BytePtr color_codes;
        static Mame.osd_bitmap backgroundbitmap1, backgroundbitmap2;
        static int zaxxon_vid_type;

        const int ZAXXON_VID = 0, CONGO_VID = 1, FUTSPY_VID = 2;

        static Mame.MemoryReadAddress[] readmem =  {
new Mame.MemoryReadAddress( 0x0000, 0x4fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xa000, 0xa0ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0xc001, 0xc001, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0xc002, 0xc002, Mame.input_port_3_r ),	/* DSW0 */
	new Mame.MemoryReadAddress( 0xc003, 0xc003, Mame.input_port_4_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0xc100, 0xc100, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
                                                       };
        static Mame.MemoryWriteAddress[] writemem =  {
new Mame.MemoryWriteAddress( 0x0000, 0x4fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0x6000, 0x6fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress(0x8000, 0x83ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress(0xa000, 0xa0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress(0xc000, 0xc002, Mame.MWA_NOP ),	/* coin enables */
	new Mame.MemoryWriteAddress(0xc003, 0xc004, Mame.coin_counter_w ),
	new Mame.MemoryWriteAddress(0xff3c, 0xff3e, zaxxon_sound_w ),
	new Mame.MemoryWriteAddress(0xfff0, 0xfff0, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress(0xfff1, 0xfff1, Mame.MWA_RAM, zaxxon_char_color_bank ),
	new Mame.MemoryWriteAddress(0xfff8, 0xfff9, Mame.MWA_RAM, zaxxon_background_position ),
	new Mame.MemoryWriteAddress(0xfffa, 0xfffa, Mame.MWA_RAM, zaxxon_background_color_bank ),
	new Mame.MemoryWriteAddress(0xfffb, 0xfffb, Mame.MWA_RAM, zaxxon_background_enable ),
	new Mame.MemoryWriteAddress(-1 )  /* end of table */
                                                        };
        const int TOTAL_SOUNDS = 22;
        static int[] soundplaying = new int[TOTAL_SOUNDS];
        class _sa
        {
            public _sa(int channel) { this.channel = channel; }
            public _sa(int channel, int num, int looped, int stoppable, int restartable)
            {
                this.channel = channel; this.num = num; this.looped = looped; this.stoppable = stoppable; this.restartable = restartable;
            }
            public int channel;
            public int num;
            public int looped;
            public int stoppable;
            public int restartable;
        }
        static _sa[] sa =
{
	new _sa(  0,  0, 1, 1, 1 ),	/* Line  4 - Homing Missile  (channel 1) */
	new _sa(  1,  1, 0, 1, 1 ),	/* Line  5 - Base Missile */
	new _sa(  2,  2, 1, 1, 1 ),	/* Line  6 - Laser (force field) (channel 1) */
	new _sa(  3,  3, 1, 1, 1 ),	/* Line  7 - Battleship (end of level boss) (channel 1) */
	new _sa( -1 ),					/* Line  8 - unused */
	new _sa( -1 ),					/* Line  9 - unused */
	new _sa( -1 ),					/* Line 10 - unused */
	new _sa( -1 ),					/* Line 11 - unused */
	new _sa(  4,  4, 0, 0, 1 ),	/* Line 12 - S-Exp (enemy explosion) */
	new _sa(  5,  5, 0, 0, 0 ),	/* Line 13 - M-Exp (ship explosion) (channel 1) */
	new _sa( -1 ),					/* Line 14 - unused */
	new _sa(  6,  6, 0, 0, 1 ),	/* Line 15 - Cannon (ship fire) */
	new _sa(  7,  7, 0, 0, 1 ),	/* Line 16 - Shot (enemy fire) */
	new _sa( -1 ),					/* Line 17 - unused */
	new _sa(  8,  8, 0, 0, 1 ),	/* Line 18 - Alarm 2 (target lock) */
	new _sa(  9,  9, 0, 0, 0 ),	/* Line 19 - Alarm 3 (low fuel) (channel 1) */
	new _sa( -1 ),					/* Line 20 - unused */
	new _sa( -1 ),					/* Line 21 - unused */
	new _sa( -1 ),					/* Line 22 - unused */
	new _sa( -1 ),					/* Line 23 - unused */
	new _sa( 10, 10, 1, 1, 1 ),	/* background */
	new _sa( 11, 11, 1, 1, 1 ),	/* background */
};

        public static void zaxxon_sound_w(int offset, int data)
        {
            int line;
            int noise;


            if (offset == 0)
            {
                /* handle background rumble */
                switch (data & 0x0c)
                {
                    case 0x04:
                        soundplaying[20] = 0;
                        Mame.sample_stop(sa[20].channel);
                        if (soundplaying[21] == 0)
                        {
                            soundplaying[21] = 1;
                            Mame.sample_start(sa[21].channel, sa[21].num, sa[21].looped);
                        }
                        Mame.sample_set_volume(sa[21].channel, 128 + 40 * (data & 0x03));
                        break;
                    case 0x00:
                    case 0x08:
                        if (soundplaying[20] == 0)
                        {
                            soundplaying[20] = 1;
                            Mame.sample_start(sa[20].channel, sa[20].num, sa[20].looped);
                        }
                        Mame.sample_set_volume(sa[20].channel, 128 + 40 * (data & 0x03));
                        soundplaying[21] = 0;
                        Mame.sample_stop(sa[21].channel);
                        break;
                    case 0x0c:
                        soundplaying[20] = 0;
                        Mame.sample_stop(sa[20].channel);
                        soundplaying[21] = 0;
                        Mame.sample_stop(sa[21].channel);
                        break;
                }
            }

            for (line = 0; line < 8; line++)
            {
                noise = 8 * offset + line - 4;

                /* the first four sound lines are handled separately */
                if (noise >= 0)
                {
                    if ((data & (1 << line)) == 0)
                    {
                        /* trigger sound */
                        if (soundplaying[noise] == 0)
                        {
                            soundplaying[noise] = 1;
                            if (sa[noise].channel != -1)
                            {
                                if (sa[noise].restartable != 0 || !Mame.sample_playing(sa[noise].channel))
                                    Mame.sample_start(sa[noise].channel, sa[noise].num, sa[noise].looped);
                            }
                        }
                    }
                    else
                    {
                        if (soundplaying[noise] != 0)
                        {
                            soundplaying[noise] = 0;
                            if (sa[noise].channel != -1 && sa[noise].stoppable != 0)
                                Mame.sample_stop(sa[noise].channel);
                        }
                    }
                }
            }

        }
        static int zaxxon_interrupt()
        {
            if ((Mame.readinputport(5) & 1) != 0)	/* get status of the F2 key */
                return Mame.nmi_interrupt();	/* trigger self test */
            else return Mame.interrupt();
        }


        static string[] zaxxon_sample_names =
{
	"*zaxxon",
	"03.wav",	/* Homing Missile */
	"02.wav",	/* Base Missile */
	"01.wav",	/* Laser (force field) */
	"00.wav",	/* Battleship (end of level boss) */
	"11.wav",	/* S-Exp (enemy explosion) */
	"10.wav",	/* M-Exp (ship explosion) */
	"08.wav", 	/* Cannon (ship fire) */
	"23.wav",	/* Shot (enemy fire) */
	"21.wav",	/* Alarm 2 (target lock) */
	"20.wav",	/* Alarm 3 (low fuel) */
	"05.wav",	/* initial background noise */
	"04.wav",	/* looped asteroid noise */
    null,
};

        static Mame.Samplesinterface zaxxon_samples_interface =
        new Mame.Samplesinterface(
            12,	/* 12 channels */
            25,	/* volume */
            zaxxon_sample_names
        );
        static Mame.GfxLayout zaxxon_charlayout1 = new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            3,	/* 3 bits per pixel (actually 2, the third plane is 0) */
            new uint[] { 2 * 256 * 8 * 8, 256 * 8 * 8, 0 },	/* the bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );

        static Mame.GfxLayout zaxxon_charlayout2 = new Mame.GfxLayout(
           8, 8,	/* 8*8 characters */
           1024,	/* 1024 characters */
           3,	/* 3 bits per pixel */
           new uint[] { 2 * 1024 * 8 * 8, 1024 * 8 * 8, 0 },	/* the bitplanes are separated */
           new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
           new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
           8 * 8	/* every char takes 8 consecutive bytes */
       );

        static Mame.GfxLayout spritelayout = new Mame.GfxLayout(
           32, 32,	/* 32*32 sprites */
           64,	/* 64 sprites */
           3,	/* 3 bits per pixel */
           new uint[] { 2 * 64 * 128 * 8, 64 * 128 * 8, 0 },	/* the bitplanes are separated */
           new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 16*8+4, 16*8+5, 16*8+6, 16*8+7,
			24*8+0, 24*8+1, 24*8+2, 24*8+3, 24*8+4, 24*8+5, 24*8+6, 24*8+7 },
           new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8,
			64*8, 65*8, 66*8, 67*8, 68*8, 69*8, 70*8, 71*8,
			96*8, 97*8, 98*8, 99*8, 100*8, 101*8, 102*8, 103*8 },
           128 * 8	/* every sprite takes 128 consecutive bytes */
       );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, zaxxon_charlayout1,   0, 32 ),	/* characters */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, zaxxon_charlayout2,   0, 32 ),	/* background tiles */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout,  0, 32 ),			/* sprites */
};
        static void zaxxon_init_machine()
        {
            zaxxon_vid_type = 0;
        }
        public class machine_driver_zaxxon : Mame.MachineDriver
        {
            public machine_driver_zaxxon()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, writemem, null, null, zaxxon_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = 2500;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_zaxxon.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 32 * 32;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, zaxxon_samples_interface));
            }
            public override void init_machine()
            {
                zaxxon_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint cpi = 0;
                int pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
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

                /* color_prom now points to the beginning of the character color codes */
                color_codes = new _BytePtr(color_prom, (int)cpi);	/* we'll need it later */


                /* all gfx elements use the same palette */
                for (int i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
                    colortable[Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i]= (ushort)i;
            }
            public override int vh_start()
            {
                Mame.osd_bitmap prebitmap;
                int width, height;


                if (Generic.generic_vh_start() != 0)
                    return 1;

                /* for speed, backgrounds are arranged differently if axis is swapped */
                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                {
                    height = 512;
                    width = 2303 + 32;
                }
                else
                {
                    /* leave a screenful of black pixels at each end */
                    height = 256 + 4096 + 256;
                    width = 256;
                }
                /* large bitmap for the precalculated background */
                if ((backgroundbitmap1 = Mame.osd_create_bitmap(width, height)) == null)
                {
                    vh_stop();
                    return 1;
                }

                if (zaxxon_vid_type == ZAXXON_VID || zaxxon_vid_type == FUTSPY_VID)
                {
                    if ((backgroundbitmap2 = Mame.osd_create_bitmap(width, height)) == null)
                    {
                        vh_stop();
                        return 1;
                    }
                }

                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                {
                    /* create a temporary bitmap to prepare the background before converting it */
                    if ((prebitmap = Mame.osd_create_bitmap(256, 4096)) == null)
                    {
                        vh_stop();
                        return 1;
                    }
                }
                else
                    prebitmap = backgroundbitmap1;

                /* prepare the background */
                create_background(backgroundbitmap1, prebitmap, 0);

                if (zaxxon_vid_type == ZAXXON_VID || zaxxon_vid_type == FUTSPY_VID)
                {
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) == 0)
                        prebitmap = backgroundbitmap2;

                    /* prepare a second background with different colors, used in the death sequence */
                    create_background(backgroundbitmap2, prebitmap, 16);
                }

                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                    Mame.osd_free_bitmap(prebitmap);

                return 0;
            }
            public override void vh_stop()
            {
                if (backgroundbitmap1 != null) Mame.osd_free_bitmap(backgroundbitmap1);
                if (backgroundbitmap2 != null) Mame.osd_free_bitmap(backgroundbitmap2);
                Generic.generic_vh_stop();
            }
            static void copy_pixel(Mame.osd_bitmap dst_bm, int dx, int dy, Mame.osd_bitmap src_bm, int sx, int sy)
            {
                Mame.plot_pixel(dst_bm, dx, dy, Mame.read_pixel(src_bm, sx, sy));
            }

            static void create_background(Mame.osd_bitmap dst_bm, Mame.osd_bitmap src_bm, int col)
            {
                for (int offs = 0; offs < 0x4000; offs++)
                {
                    int sy = 8 * (offs / 32);
                    int sx = 8 * (offs % 32);

                    if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) == 0)
                        /* leave screenful of black pixels at end */
                        sy += 256;

                    Mame.drawgfx(src_bm, Mame.Machine.gfx[1],
                            (uint)(Mame.memory_region(Mame.REGION_GFX4)[offs] + 256 * (Mame.memory_region(Mame.REGION_GFX4)[0x4000 + offs] & 3)),
                            (uint)(col + (Mame.memory_region(Mame.REGION_GFX4)[0x4000 + offs] >> 4)),
                            false, false,
                            sx, sy,
                            null, Mame.TRANSPARENCY_NONE, 0);
                }

                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                {
                    /* the background is stored as a rectangle, but is drawn by the hardware skewed: */
                    /* go right two pixels, then up one pixel. Doing the conversion at run time would */
                    /* be extremely expensive, so we do it now. To save memory, we squash the image */
                    /* horizontally (doing line shifts at run time is much less expensive than doing */
                    /* column shifts) */
                    for (int offs = -510; offs < 4096; offs += 2)
                    {
                        int sx = (2302 - 510 / 2) - offs / 2;

                        for (int sy = 0; sy < 512; sy += 2)
                        {
                            if (offs + sy >= 0 && offs + sy < 4096)
                            {
                                copy_pixel(dst_bm, sx, 511 - sy, src_bm, sy / 2, 4095 - (offs + sy));
                                copy_pixel(dst_bm, sx, 511 - (sy + 1), src_bm, sy / 2, 4095 - (offs + sy + 1));
                            }
                        }
                    }
                }
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int offs;


                /* copy the background */
                /* TODO: there's a bug here which shows only in test mode. The background doesn't */
                /* cover the whole screen, so the image is not fully overwritten and part of the */
                /* character color test screen remains on screen when it is replaced by the background */
                /* color test. */
                if (zaxxon_background_enable[0] != 0)
                {
                    int i, skew, scroll;
                    Mame.rectangle clip = new Mame.rectangle();


                    if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                    {
                        /* standard rotation - skew background horizontally */
                        if (zaxxon_vid_type == CONGO_VID)
                            scroll = 1023 + 63 - (zaxxon_background_position[0] + 256 * zaxxon_background_position[1]);
                        else
                            scroll = 2048 + 63 - (zaxxon_background_position[0] + 256 * (zaxxon_background_position[1] & 7));

                        skew = 128 - 512 + 2 * Mame.Machine.drv.visible_area.min_x;

                        clip.min_y = Mame.Machine.drv.visible_area.min_y;
                        clip.max_y = Mame.Machine.drv.visible_area.max_y;

                        for (i = Mame.Machine.drv.visible_area.min_x; i <= Mame.Machine.drv.visible_area.max_x; i++)
                        {
                            clip.min_x = i;
                            clip.max_x = i;

                            if ((zaxxon_vid_type == ZAXXON_VID || zaxxon_vid_type == FUTSPY_VID)
                                 && (zaxxon_background_color_bank[0] & 1) != 0)
                                Mame.copybitmap(bitmap, backgroundbitmap2, false, false, -scroll, skew, clip, Mame.TRANSPARENCY_NONE, 0);
                            else
                                Mame.copybitmap(bitmap, backgroundbitmap1, false, false, -scroll, skew, clip, Mame.TRANSPARENCY_NONE, 0);

                            skew += 2;
                        }
                    }
                    else
                    {
                        /* skew background up one pixel every 2 horizontal pixels */
                        if (zaxxon_vid_type == CONGO_VID)
                            scroll = 2050 + 2 * (zaxxon_background_position[0] + 256 * zaxxon_background_position[1])
                                - backgroundbitmap1.height + 256;
                        else
                            scroll = 2 * (zaxxon_background_position[0] + 256 * (zaxxon_background_position[1] & 7))
                                - backgroundbitmap1.height + 256;

                        skew = 72 - (255 - Mame.Machine.drv.visible_area.max_y);

                        clip.min_x = Mame.Machine.drv.visible_area.min_x;
                        clip.max_x = Mame.Machine.drv.visible_area.max_x;

                        for (i = Mame.Machine.drv.visible_area.max_y; i >= Mame.Machine.drv.visible_area.min_y; i -= 2)
                        {
                            clip.min_y = i - 1;
                            clip.max_y = i;

                            if ((zaxxon_vid_type == ZAXXON_VID || zaxxon_vid_type == FUTSPY_VID)
                                 && (zaxxon_background_color_bank[0] & 1) != 0)
                                Mame.copybitmap(bitmap, backgroundbitmap2, false, false, skew, scroll, clip, Mame.TRANSPARENCY_NONE, 0);
                            else
                                Mame.copybitmap(bitmap, backgroundbitmap1, false, false, skew, scroll, clip, Mame.TRANSPARENCY_NONE, 0);

                            skew--;
                        }
                    }
                }
                else
                    Mame.fillbitmap(bitmap, Mame.Machine.pens[0], Mame.Machine.drv.visible_area);

                draw_sprites(bitmap);

                /* draw the frontmost playfield. They are characters, but draw them as sprites */
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx, sy;
                    int color;


                    sy = offs / 32;
                    sx = offs % 32;

                    if (zaxxon_vid_type == CONGO_VID)
                        color = Generic.colorram[offs];
                    else
                        /* not sure about the color code calculation - char_color_bank is used only in test mode */
                        color = (color_codes[sx + 32 * (sy / 4)] & 0x0f) + 16 * (zaxxon_char_color_bank[0] & 1);

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            Generic.videoram[offs],
                            (uint)color,
                            false, false,
                            8 * sx, 8 * sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }
            static uint[] sprpri = new uint[0x100]; /* this really should not be more
		                             * than 0x1e, but I did not want to check
		                             * for 0xff which is set when sprite is off
		                             * -V-
		                             */
            static void draw_sprites(Mame.osd_bitmap bitmap)
            {
                int offs;

                if (zaxxon_vid_type == CONGO_VID)
                {
                    int i;

                    /* Draw the sprites. Note that it is important to draw them exactly in this */
                    /* order, to have the correct priorities. */
                    /* Sprites actually start at 0xff * [0xc031], it seems to be static tho'*/
                    /* The number of active sprites is stored at 0xc032 */

                    for (offs = 0x1e * 0x20; offs >= 0x00; offs -= 0x20)
                        sprpri[Generic.spriteram[offs + 1]] = (uint)offs;

                    for (i = 0x1e; i >= 0; i--)
                    {
                        offs = (int)sprpri[i];

                        if (Generic.spriteram[offs + 2] != 0xff)
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                    (uint)(Generic.spriteram[offs + 2 + 1] & 0x7f),
                                    (uint)(Generic.spriteram[offs + 2 + 2]),
                                    (Generic.spriteram[offs + 2 + 2] & 0x80) != 0, (Generic.spriteram[offs + 2 + 1] & 0x80) != 0,
                                    ((Generic.spriteram[offs + 2 + 3] + 16) & 0xff) - 31, 255 - Generic.spriteram[offs + 2] - 15,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
                else if (zaxxon_vid_type == FUTSPY_VID)
                {
                    /* Draw the sprites. Note that it is important to draw them exactly in this */
                    /* order, to have the correct priorities. */
                    for (offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                    {
                        if (Generic.spriteram[offs] != 0xff)
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                    (uint)(Generic.spriteram[offs + 1] & 0x7f),
                                    (uint)(Generic.spriteram[offs + 2] & 0x3f),
                                    (Generic.spriteram[offs + 1] & 0x80) != 0, (Generic.spriteram[offs + 1] & 0x80) != 0,	/* ?? */
                                    ((Generic.spriteram[offs + 3] + 16) & 0xff) - 32, 255 - Generic.spriteram[offs] - 16,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
                else
                {
                    /* Draw the sprites. Note that it is important to draw them exactly in this */
                    /* order, to have the correct priorities. */
                    for (offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                    {
                        if (Generic.spriteram[offs] != 0xff)
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                    (uint)(Generic.spriteram[offs + 1] & 0x3f),
                                    (uint)(Generic.spriteram[offs + 2] & 0x3f),
                                    (Generic.spriteram[offs + 1] & 0x40) != 0, (Generic.spriteram[offs + 1] & 0x80) != 0,
                                    ((Generic.spriteram[offs + 3] + 16) & 0xff) - 32, 255 - Generic.spriteram[offs] - 16,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public static Mame.InputPortTiny[] input_ports_zaxxon()
        {
            INPUT_PORTS_START("zaxxon");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);	/* the self test calls this UP */
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY);	/* the self test calls this DOWN */
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);	/* button 2 - unused */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);	/* the self test calls this UP */
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);	/* the self test calls this DOWN */
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_UNKNOWN);	/* button 2 - unused */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_START1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_START2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, IPT_UNUSED);
            /* the coin inputs must stay active for exactly one frame, otherwise */
            /* the game will keep inserting coins. */
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN2, 1);
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_HIGH, (int)inptports.IPT_COIN3, 1);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x03, ipdn_defaultstrings["Bonus Life"]);
            PORT_DIPSETTING(0x03, "10000");
            PORT_DIPSETTING(0x01, "20000");
            PORT_DIPSETTING(0x02, "30000");
            PORT_DIPSETTING(0x00, "40000");
            /* The Super Zaxxon manual lists the following as unused. */
            PORT_DIPNAME(0x0c, 0x0c, "Difficulty???");
            PORT_DIPSETTING(0x0c, "Easy?");
            PORT_DIPSETTING(0x04, "Medium?");
            PORT_DIPSETTING(0x08, "Hard?");
            PORT_DIPSETTING(0x00, "Hardest?");
            PORT_DIPNAME(0x30, 0x30, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x20, "5");
            PORT_BITX(0, 0x00, (int)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "Infinite", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x40, 0x40, ipdn_defaultstrings["Demo Sounds"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x40, ipdn_defaultstrings["On"]);
            PORT_DIPNAME(0x80, 0x00, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["Cocktail"]);

            PORT_START("DSW1");
            PORT_DIPNAME(0x0f, 0x03, ipdn_defaultstrings["Coin B"]);
            PORT_DIPSETTING(0x0f, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x0b, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x06, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x0a, "2 Coins/1 Credit 3/2 4/3");
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x02, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x0c, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x04, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x0d, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x08, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0x00, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x05, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x09, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0x0e, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPNAME(0xf0, 0x30, ipdn_defaultstrings["Coin A"]);
            PORT_DIPSETTING(0xf0, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x70, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0xb0, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x60, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0xa0, "2 Coins/1 Credit 3/2 4/3");
            PORT_DIPSETTING(0x30, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x20, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0xc0, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x40, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0xd0, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x80, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0x00, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x50, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x90, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0xe0, ipdn_defaultstrings["1C_6C"]);

            PORT_START("FAKE");
            /* This fake input port is used to get the status of the F2 key, */
            /* and activate the test mode, which is triggered by a NMI */
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_SERVICE, ipdn_defaultstrings["Service Mode"], (int)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_zaxxon()
        {
            ROM_START("zaxxon");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("zaxxon.3", 0x0000, 0x2000, 0x6e2b4a30);
            ROM_LOAD("zaxxon.2", 0x2000, 0x2000, 0x1c9ea398);
            ROM_LOAD("zaxxon.1", 0x4000, 0x1000, 0x1c123ef9);

            ROM_REGION(0x1800, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("zaxxon.14", 0x0000, 0x0800, 0x07bf8c52);	/* characters */
            ROM_LOAD("zaxxon.15", 0x0800, 0x0800, 0xc215edcb);
            /* 1000-17ff empty space to convert the characters as 3bpp instead of 2 */

            ROM_REGION(0x6000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("zaxxon.6", 0x0000, 0x2000, 0x6e07bb68);	/* background tiles */
            ROM_LOAD("zaxxon.5", 0x2000, 0x2000, 0x0a5bce6a);
            ROM_LOAD("zaxxon.4", 0x4000, 0x2000, 0xa5bf1465);

            ROM_REGION(0x6000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("zaxxon.11", 0x0000, 0x2000, 0xeaf0dd4b);	/* sprites */
            ROM_LOAD("zaxxon.12", 0x2000, 0x2000, 0x1c5369c7);
            ROM_LOAD("zaxxon.13", 0x4000, 0x2000, 0xab4e8a9a);

            ROM_REGION(0x8000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);	/* background tilemaps converted in vh_start */
            ROM_LOAD("zaxxon.8", 0x0000, 0x2000, 0x28d65063);
            ROM_LOAD("zaxxon.7", 0x2000, 0x2000, 0x6284c200);
            ROM_LOAD("zaxxon.10", 0x4000, 0x2000, 0xa95e61fd);
            ROM_LOAD("zaxxon.9", 0x6000, 0x2000, 0x7e42691f);

            ROM_REGION(0x0200, Mame.REGION_PROMS);
            ROM_LOAD("zaxxon.u98", 0x0000, 0x0100, 0x6cc6695b); /* palette */
            ROM_LOAD("zaxxon.u72", 0x0100, 0x0100, 0xdeaa21f7); /* char lookup table */
            return ROM_END;
        }
        public override void driver_init()
        {
            //none
        }
        public driver_zaxxon()
        {
            drv = new machine_driver_zaxxon();
            year = "1982";
            name = "zaxxon";
            description = "Zaxxon (set1)";
            manufacturer = "Sega";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_zaxxon();
            rom = rom_zaxxon();
        }
    }
    class driver_szaxxon : Mame.GameDriver
    {
        public override void driver_init()
        {
            segacrpt.szaxxon_decode();
        }
        Mame.RomModule[] rom_szaxxon()
        {

            ROM_START("szaxxon");
            ROM_REGION(2 * 0x10000, Mame.REGION_CPU1);	/* 64k for code + 64k for decrypted opcodes */
            ROM_LOAD("suzaxxon.3", 0x0000, 0x2000, 0xaf7221da);
            ROM_LOAD("suzaxxon.2", 0x2000, 0x2000, 0x1b90fb2a);
            ROM_LOAD("suzaxxon.1", 0x4000, 0x1000, 0x07258b4a);

            ROM_REGION(0x1800, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("suzaxxon.14", 0x0000, 0x0800, 0xbccf560c);	/* characters */
            ROM_LOAD("suzaxxon.15", 0x0800, 0x0800, 0xd28c628b);
            /* 1000-17ff empty space to convert the characters as 3bpp instead of 2 */

            ROM_REGION(0x6000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("suzaxxon.6", 0x0000, 0x2000, 0xf51af375);	/* background tiles */
            ROM_LOAD("suzaxxon.5", 0x2000, 0x2000, 0xa7de021d);
            ROM_LOAD("suzaxxon.4", 0x4000, 0x2000, 0x5bfb3b04);

            ROM_REGION(0x6000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("suzaxxon.11", 0x0000, 0x2000, 0x1503ae41);	/* sprites */
            ROM_LOAD("suzaxxon.12", 0x2000, 0x2000, 0x3b53d83f);
            ROM_LOAD("suzaxxon.13", 0x4000, 0x2000, 0x581e8793);

            ROM_REGION(0x8000, Mame.REGION_GFX4 | Mame.REGIONFLAG_DISPOSE);	/* background tilemaps converted in vh_start */
            ROM_LOAD("suzaxxon.8", 0x0000, 0x2000, 0xdd1b52df);
            ROM_LOAD("suzaxxon.7", 0x2000, 0x2000, 0xb5bc07f0);
            ROM_LOAD("suzaxxon.10", 0x4000, 0x2000, 0x68e84174);
            ROM_LOAD("suzaxxon.9", 0x6000, 0x2000, 0xa509994b);

            ROM_REGION(0x0200, Mame.REGION_PROMS);
            ROM_LOAD("suzaxxon.u98", 0x0000, 0x0100, 0x15727a9f); /* palette */
            ROM_LOAD("suzaxxon.u72", 0x0100, 0x0100, 0xdeaa21f7); /* char lookup table */
            return ROM_END;
        }
        public driver_szaxxon()
        {
            drv = new driver_zaxxon.machine_driver_zaxxon();
            year = "1982";
            name = "szaxxon";
            description = "Super Zaxxon";
            manufacturer = "Sega";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = driver_zaxxon.input_ports_zaxxon();
            rom = rom_szaxxon();
        }
    }
}
