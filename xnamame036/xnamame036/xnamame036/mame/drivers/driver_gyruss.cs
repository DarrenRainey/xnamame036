#define EMULATE_6809

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_gyruss : Mame.GameDriver
    {
        static _BytePtr gyruss_spritebank = new _BytePtr(1);
        static _BytePtr gyruss_6809_drawplanet = new _BytePtr(1);
        static _BytePtr gyruss_6809_drawship = new _BytePtr(1);
        static int flipscreen;
        static _BytePtr gyruss_sharedram = new _BytePtr(1);
        struct Sprites
        {
            public byte y, shape, attr, x;
        }

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9000, 0x9fff, Mame.MRA_RAM ),
#if EMULATE_6809
	new Mame.MemoryReadAddress( 0xa000, 0xa7ff, gyruss_sharedram_r ),
#else
	new Mame.MemoryReadAddress( 0xa000, 0xa7ff, Mame.MRA_RAM ),
#endif
	new Mame.MemoryReadAddress( 0xc000, 0xc000, Mame.input_port_4_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0xc080, 0xc080, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0xc0a0, 0xc0a0, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0xc0c0, 0xc0c0, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( 0xc0e0, 0xc0e0, Mame.input_port_3_r ),	/* DSW0 */
	new Mame.MemoryReadAddress( 0xc100, 0xc100, Mame.input_port_5_r ),	/* DSW2 */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),                 /* rom space+1        */
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x9000, 0x9fff,Mame.MWA_RAM ),
#if EMULATE_6809
	new Mame.MemoryWriteAddress( 0xa000, 0xa7ff, gyruss_sharedram_w, gyruss_sharedram ),
#else
	new Mame.MemoryWriteAddress( 0xa000, 0xa17f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),     /* odd frame spriteram */
	new Mame.MemoryWriteAddress( 0xa200, 0xa37f, Mame.MWA_RAM, Generic.spriteram_2 ),   /* even frame spriteram */
	new Mame.MemoryWriteAddress( 0xa700, 0xa700, Mame.MWA_RAM, &gyruss_spritebank ),
	new Mame.MemoryWriteAddress( 0xa701, 0xa701, Mame.MWA_NOP ),        /* semaphore system   */
	new Mame.MemoryWriteAddress( 0xa702, 0xa702, gyruss_queuereg_w ),       /* semaphore system   */
	new Mame.MemoryWriteAddress( 0xa7fc, 0xa7fc, Mame.MWA_RAM, &gyruss_6809_drawplanet ),
	new Mame.MemoryWriteAddress( 0xa7fd, 0xa7fd, Mame.MWA_RAM, &gyruss_6809_drawship ),
#endif
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, Mame.MWA_NOP ),	/* watchdog reset */
	new Mame.MemoryWriteAddress( 0xc080, 0xc080, gyruss_sh_irqtrigger_w ),
	new Mame.MemoryWriteAddress( 0xc100, 0xc100, Mame.soundlatch_w ),         /* command to soundb  */
	new Mame.MemoryWriteAddress( 0xc180, 0xc180, Mame.interrupt_enable_w ),      /* NMI enable         */
	new Mame.MemoryWriteAddress( 0xc185, 0xc185, gyruss_flipscreen_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};



        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x5fff, Mame.MRA_ROM ),                 /* rom soundboard     */
	new Mame.MemoryReadAddress( 0x6000, 0x63ff, Mame.MRA_RAM ),                 /* ram soundboard     */
	new Mame.MemoryReadAddress( 0x8000, 0x8000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x5fff, Mame.MWA_ROM ),                 /* rom soundboard     */
	new Mame.MemoryWriteAddress( 0x6000, 0x63ff, Mame.MWA_RAM ),                 /* ram soundboard     */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x01, 0x01, ay8910.AY8910_read_port_0_r ),
  	new Mame.IOReadPort( 0x05, 0x05, ay8910.AY8910_read_port_1_r ),
	new Mame.IOReadPort( 0x09, 0x09, ay8910.AY8910_read_port_2_r ),
  	new Mame.IOReadPort( 0x0d, 0x0d, ay8910.AY8910_read_port_3_r ),
  	new Mame.IOReadPort( 0x11, 0x11, ay8910.AY8910_read_port_4_r ),
	new Mame.IOReadPort( -1 )
};

        static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, ay8910.AY8910_control_port_0_w ),
	new Mame.IOWritePort( 0x02, 0x02, ay8910.AY8910_write_port_0_w ),
	new Mame.IOWritePort( 0x04, 0x04, ay8910.AY8910_control_port_1_w ),
	new Mame.IOWritePort( 0x06, 0x06, ay8910.AY8910_write_port_1_w ),
	new Mame.IOWritePort( 0x08, 0x08, ay8910.AY8910_control_port_2_w ),
	new Mame.IOWritePort( 0x0a, 0x0a, ay8910.AY8910_write_port_2_w ),
	new Mame.IOWritePort( 0x0c, 0x0c, ay8910.AY8910_control_port_3_w ),
	new Mame.IOWritePort( 0x0e, 0x0e, ay8910.AY8910_write_port_3_w ),
	new Mame.IOWritePort( 0x10, 0x10, ay8910.AY8910_control_port_4_w ),
	new Mame.IOWritePort( 0x12, 0x12, ay8910.AY8910_write_port_4_w ),
	new Mame.IOWritePort( 0x14, 0x14, gyruss_i8039_irq_w ),
	new Mame.IOWritePort( 0x18, 0x18, Mame.soundlatch2_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};


#if EMULATE_6809
        static Mame.MemoryReadAddress[] m6809_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0000, gyruss_scanline_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0x67ff, gyruss_sharedram_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] m6809_writemem =
{
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff,Mame. MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x4040, 0x40ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x6000, 0x67ff, gyruss_sharedram_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
#endif

        static Mame.MemoryReadAddress[] i8039_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] i8039_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.IOReadPort[] i8039_readport =
{
	new Mame.IOReadPort( 0x00, 0xff, Mame.soundlatch2_r ),
	new Mame.IOReadPort( -1 )
};

        static Mame.IOWritePort[] i8039_writeport =
{
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p1, Mame.cpu_i8039.I8039_p1, DAC.DAC_data_w ),
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p2, Mame.cpu_i8039.I8039_p2, Mame.IOWP_NOP ),
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },
            new uint[] { 0, 1, 2, 3, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8	/* every char takes 16 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout1 =
        new Mame.GfxLayout(
            8, 16,	/* 16*8 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0x4000 * 8 + 4, 0x4000 * 8 + 0, 4, 0 },
            new uint[] { 0, 1, 2, 3, 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout2 =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0x4000 * 8 + 4, 0x4000 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3,  8*8, 8*8+1, 8*8+2, 8*8+3,
		16*8+0, 16*8+1, 16*8+2, 16*8+3,  24*8, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );



        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000,charlayout,       0, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000,spritelayout1, 16*4, 16 ),	/* upper half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0010,spritelayout1, 16*4, 16 ),	/* lower half */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000,spritelayout2, 16*4, 16 ),
};



        static AY8910interface ay8910_interface =
        new AY8910interface(
            5,	/* 5 chips */
            14318180 / 8,	/* 1.789772727 MHz */
            new int[]{ Mame.MIXERG(10,Mame.MIXER_GAIN_4x,Mame.MIXER_PAN_RIGHT), Mame.MIXERG(10,Mame.MIXER_GAIN_4x,Mame.MIXER_PAN_LEFT),
			Mame.MIXERG(20,Mame.MIXER_GAIN_4x,Mame.MIXER_PAN_RIGHT), Mame.MIXERG(20,Mame.MIXER_GAIN_4x,Mame.MIXER_PAN_RIGHT), Mame.MIXERG(20,Mame.MIXER_GAIN_4x,Mame.MIXER_PAN_LEFT) },
            /*  R       L   |   R       R       L */
            /*   effects    |         music       */
            new AY8910portRead[] { null, null, gyruss_portA_r,null,null },
            new AY8910portRead[] { null,null,null,null,null },
            new AY8910portWrite[] { null, null, null, null, null },
            new AY8910portWrite[] { gyruss_filter0_w, gyruss_filter1_w,null,null,null },
            null//new AY8910handler[] { null }
        );

        static Mame.DACinterface dac_interface =
        new Mame.DACinterface(
            1,
            new int[] { Mame.MIXER(50, Mame.MIXER_PAN_LEFT) }
        );

        static int gyruss_sharedram_r(int offset)
        {
            return gyruss_sharedram[offset];
        }

        static void gyruss_sharedram_w(int offset, int data)
        {
            gyruss_sharedram[offset] = (byte)data;
        }
        static void gyruss_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        static int gyruss_scanline_r(int offset)
        {
            return Mame.cpu_scalebyfcount(256);
        }
        static int[] gyruss_timer = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x09, 0x0a, 0x0b, 0x0a, 0x0d };
        static int last_totalcycles = 0;
        static int clock;
        static int gyruss_portA_r(int offset)
        {
            int current_totalcycles;

            current_totalcycles = Mame.cpu_gettotalcycles();
            clock = (clock + (current_totalcycles - last_totalcycles)) % 10240;

            last_totalcycles = current_totalcycles;

            return gyruss_timer[clock / 1024];
        }
        static void filter_w(int chip, int data)
        {
            for (int i = 0; i < 3; i++)
            {
                int C;


                C = 0;
                if ((data & 1) != 0) C += 47000;	/* 47000pF = 0.047uF */
                if ((data & 2) != 0) C += 220000;	/* 220000pF = 0.22uF */
                data >>= 2;
                Mame.set_RC_filter(3 * chip + i, 1000, 2200, 200, C);
            }
        }
        static void gyruss_filter0_w(int offset, int data)
        {
            filter_w(0, data);
        }
        static void gyruss_filter1_w(int offset, int data)
        {
            filter_w(1, data);
        }
        static void gyruss_sh_irqtrigger_w(int offset, int data)
        {
            /* writing to this register triggers IRQ on the sound CPU */
            Mame.cpu_cause_interrupt(1, 0xff);
        }
        static void gyruss_i8039_irq_w(int offset, int data)
        {
            Mame.cpu_cause_interrupt(2, Mame.cpu_i8039.I8039_EXT_INT);
        }

        class machine_driver_gyruss : Mame.MachineDriver
        {
            public machine_driver_gyruss()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 4, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.ignore_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_I8039 | Mame.CPU_AUDIO_CPU, 8000000 / 15, i8039_readmem, i8039_writemem, i8039_readport, i8039_writeport, Mame.ignore_interrupt, 1));
#if EMULATE_6809
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 2000000, m6809_readmem, m6809_writemem, null, null, Mame.interrupt, 1));
#endif
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
#if EMULATE_6809
                cpu_slices_per_frame = 20;
#else
                cpu_slices_per_frame = 1;
#endif
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_gyruss.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 16 * 4 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                int pi = 0, cpi = 0;
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

                /* color_prom now points to the beginning of the sprite lookup table */

                /* sprites */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                    COLOR(colortable,1, i,color_prom[cpi++] & 0x0f);

                /* characters */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable,0, i,(color_prom[cpi++] & 0x0f) + 0x10);
            }
            public override int vh_start()
            {
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
#if !EMULATE_6809
	gyruss_vh_screenrefresh(bitmap, full_refresh);
#else
                gyruss_6809_vh_screenrefresh(bitmap, full_refresh);
#endif
            }
            static void gyruss_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new Exception();
            }
            static void gyruss_6809_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
            {
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;
                        bool flipx = (Generic.colorram[offs] & 0x40)!=0;
                        bool flipy = (Generic.colorram[offs] & 0x80)!=0;
                        if (flipscreen!=0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 8 * (Generic.colorram[offs] & 0x20)),
                                (uint)(Generic.colorram[offs] & 0x0f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false,false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                {
                    for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1 + (Generic.spriteram[offs + 1] & 1)],
                                (uint)(Generic.spriteram[offs + 1] / 2 + 4 * (Generic.spriteram[offs + 2] & 0x20)),
                                (uint)(Generic.spriteram[offs + 2] & 0x0f),
                                (Generic.spriteram[offs + 2] & 0x40)==0, 
                                (Generic.spriteram[offs + 2] & 0x80)!=0,
                                Generic.spriteram[offs], 240 - Generic.spriteram[offs + 3] + 1,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }


                /* redraw the characters which have priority over sprites */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx = offs % 32;
                    int sy = offs / 32;
                    bool flipx = (Generic.colorram[offs] & 0x40)!=0;
                    bool flipy = (Generic.colorram[offs] & 0x80)!=0;
                    if (flipscreen!=0)
                    {
                        sx = 31 - sx;
                        sy = 31 - sy;
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    if ((Generic.colorram[offs] & 0x10) != 0)
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 8 * (Generic.colorram[offs] & 0x20)),
                                (uint)(Generic.colorram[offs] & 0x0f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }
        }
        public override void driver_init()
        {
            Konami.konami1_decode_cpu4();
        }
        Mame.RomModule[] rom_gyruss()
        {

            ROM_START("gyruss");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("gyrussk.1", 0x0000, 0x2000, 0xc673b43d);
            ROM_LOAD("gyrussk.2", 0x2000, 0x2000, 0xa4ec03e4);
            ROM_LOAD("gyrussk.3", 0x4000, 0x2000, 0x27454a98);
            /* the diagnostics ROM would go here */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("gyrussk.1a", 0x0000, 0x2000, 0xf4ae1c17);
            ROM_LOAD("gyrussk.2a", 0x2000, 0x2000, 0xba498115);
            /* the diagnostics ROM would go here */

            ROM_REGION(0x1000, Mame.REGION_CPU3);	/* 8039 */
            ROM_LOAD("gyrussk.3a", 0x0000, 0x1000, 0x3f9b5dea);

            ROM_REGION(2 * 0x10000, Mame.REGION_CPU4);	/* 64k for code + 64k for the decrypted opcodes */
            ROM_LOAD("gyrussk.9", 0xe000, 0x2000, 0x822bf27e);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gyrussk.4", 0x0000, 0x2000, 0x27d8329b);

            ROM_REGION(0x8000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("gyrussk.6", 0x0000, 0x2000, 0xc949db10);
            ROM_LOAD("gyrussk.5", 0x2000, 0x2000, 0x4f22411a);
            ROM_LOAD("gyrussk.8", 0x4000, 0x2000, 0x47cd1fbc);
            ROM_LOAD("gyrussk.7", 0x6000, 0x2000, 0x8e8d388c);

            ROM_REGION(0x0220, Mame.REGION_PROMS);
            ROM_LOAD("gyrussk.pr3", 0x0000, 0x0020, 0x98782db3);	/* palette */
            ROM_LOAD("gyrussk.pr1", 0x0020, 0x0100, 0x7ed057de);	/* sprite lookup table */
            ROM_LOAD("gyrussk.pr2", 0x0120, 0x0100, 0xde823a81);	/* character lookup table */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_gyruss()
        {

            INPUT_PORTS_START("gyruss");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0xe0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_2WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_2WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* 1p shoot 2 - unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* 2p shoot 3 - unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* 2p shoot 2 - unused */
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("DSW0");
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
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
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

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "255", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x08, 0x08, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x08, "30000 60000");
            PORT_DIPSETTING(0x00, "40000 70000");
            PORT_DIPNAME(0x70, 0x70, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x70, "1 (Easiest)");
            PORT_DIPSETTING(0x60, "2");
            PORT_DIPSETTING(0x50, "3");
            PORT_DIPSETTING(0x40, "4");
            PORT_DIPSETTING(0x30, "5 (Average)");
            PORT_DIPSETTING(0x20, "6");
            PORT_DIPSETTING(0x10, "7");
            PORT_DIPSETTING(0x00, "8 (Hardest)");
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("DSW2");
            PORT_DIPNAME(0x01, 0x00, "Demo Music");
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            /* other bits probably unused */
            return INPUT_PORTS_END;
        }
        public driver_gyruss()
        {
            drv = new machine_driver_gyruss();
            year = "1983";
            name = "gyruss";
            description = "Gyruss (Konami)";
            manufacturer = "Konami";
            flags = Mame.ROT90;
            input_ports = input_ports_gyruss();
            rom = rom_gyruss();
            drv.HasNVRAMhandler = false;
        }
    }
}
