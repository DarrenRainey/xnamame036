using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_galaga : Mame.GameDriver
    {
        static _BytePtr galaga_sharedram = new _BytePtr(1);
        static _BytePtr galaga_starcontrol = new _BytePtr(1);

        static Mame.MemoryReadAddress[] readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, galaga_sharedram_r ),
	new Mame.MemoryReadAddress( 0x6800, 0x6807, galaga_dsw_r ),
	new Mame.MemoryReadAddress( 0x7000, 0x700f, galaga_customio_data_r ),
	new Mame.MemoryReadAddress( 0x7100, 0x7100, galaga_customio_r ),
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, galaga_sharedram_r ),
	new Mame.MemoryReadAddress( 0x6800, 0x6807, galaga_dsw_r ),
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] readmem_cpu3 =
{
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, galaga_sharedram_r ),
	new Mame.MemoryReadAddress( 0x6800, 0x6807, galaga_dsw_r ),
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, galaga_sharedram_w, galaga_sharedram ),
	new Mame.MemoryWriteAddress( 0x6830, 0x6830, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0x7000, 0x700f, galaga_customio_data_w ),
	new Mame.MemoryWriteAddress( 0x7100, 0x7100, galaga_customio_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa005, Mame.MWA_RAM, galaga_starcontrol ),
	new Mame.MemoryWriteAddress( 0x6820, 0x6820, galaga_interrupt_enable_1_w ),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, galaga_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x6823, 0x6823, galaga_halt_w ),
	new Mame.MemoryWriteAddress( 0xa007, 0xa007, galaga_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8b80, 0x8bff, Mame.MWA_RAM,Generic.spriteram, Generic.spriteram_size ),       /* these three are here just to initialize */
	new Mame.MemoryWriteAddress( 0x9380, 0x93ff, Mame.MWA_RAM,Generic.spriteram_2 ),      /* the pointers. The actual writes are */
	new Mame.MemoryWriteAddress( 0x9b80, 0x9bff, Mame.MWA_RAM,Generic.spriteram_3 ),      /* handled by galaga_sharedram_w() */
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Mame.MWA_RAM,Generic.videoram, Generic.videoram_size ), /* dirtybuffer[] handling is not needed because */
	new Mame.MemoryWriteAddress( 0x8400, 0x87ff, Mame.MWA_RAM,Generic.colorram ), /* characters are redrawn every frame */
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, galaga_sharedram_w ),
	new Mame.MemoryWriteAddress( 0x6821, 0x6821, galaga_interrupt_enable_2_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem_cpu3 =
{
	new Mame.MemoryWriteAddress( 0x8000, 0x9fff, galaga_sharedram_w ),
	new Mame.MemoryWriteAddress( 0x6800, 0x681f, Namco.pengo_sound_w, Namco.namco_soundregs ),
	new Mame.MemoryWriteAddress( 0x6822, 0x6822, galaga_interrupt_enable_3_w ),
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,           /* 8*8 characters */
            128,           /* 128 characters */
            2,             /* 2 bits per pixel */
            new uint[] { 0, 4 },       /* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },   /* bits are packed in groups of four */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },   /* characters are rotated 90 degrees */
            16 * 8           /* every char takes 16 bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,          /* 16*16 sprites */
            128,            /* 128 sprites */
            2,              /* 2 bits per pixel */
            new uint[] { 0, 4 },       /* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 0, 1, 2, 3, 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 16 * 8 + 0, 16 * 8 + 1, 16 * 8 + 2, 16 * 8 + 3, 24 * 8 + 0, 24 * 8 + 1, 24 * 8 + 2, 24 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 32 * 8, 33 * 8, 34 * 8, 35 * 8, 36 * 8, 37 * 8, 38 * 8, 39 * 8 },
            64 * 8    /* every sprite takes 64 bytes */
        );



        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,       0, 32 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout,  32*4, 32 ),
};



        static Namco_interface namco_interface =
        new Namco_interface(
            3072000 / 32,	/* sample rate */
            3,			/* number of voices */
            100,		/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );

        static string[] galaga_sample_names =
{
	"*galaga",
	"bang.wav",
	null       /* end of array */
};

        static Mame.Samplesinterface samples_interface =
        new Mame.Samplesinterface(
            1,	/* one channel */
            80,	/* volume */
            galaga_sample_names
        );
        static bool interrupt_enable_1, interrupt_enable_2, interrupt_enable_3;

        static object nmi_timer;

        static void galaga_init_machine()
        {
            nmi_timer = null;
            galaga_halt_w(0, 0);
        }
        static int galaga_sharedram_r(int offset)
        {
            return galaga_sharedram[offset];
        }
        static void galaga_sharedram_w(int offset, int data)
        {
            if (offset < 0x800)		/* write to video RAM */
                Generic.dirtybuffer[offset & 0x3ff] = true;

            galaga_sharedram[offset] = (byte)data;
        }
        static int galaga_dsw_r(int offset)
        {
            int bit0, bit1;


            bit0 = (Mame.input_port_0_r(0) >> offset) & 1;
            bit1 = (Mame.input_port_1_r(0) >> offset) & 1;

            return bit0 | (bit1 << 1);
        }
        /***************************************************************************

 Emulate the custom IO chip.

***************************************************************************/
        static int customio_command;
        static int mode, credits;
        static int coinpercred, credpercoin;
        static byte[] customio = new byte[16];

        const int MAX_STARS = 250;
        const int STARS_COLOR_BASE = 32;


        static uint stars_scroll;
        static int flipscreen;

        struct star
        {
            public int x, y, col, set;
        };
        static star[] stars = new star[MAX_STARS];
        static int total_stars;

        static void galaga_customio_data_w(int offset, int data)
        {
            customio[offset] = (byte)data;

            //if (errorlog) fprintf(errorlog,"%04x: custom IO offset %02x data %02x\n",cpu_get_pc(),offset,data);

            switch (customio_command)
            {
                case 0xa8:
                    if (offset == 3 && data == 0x20)	/* total hack */
                        Mame.sample_start(0, 0, false);
                    break;

                case 0xe1:
                    if (offset == 7)
                    {
                        coinpercred = customio[1];
                        credpercoin = customio[2];
                    }
                    break;
            }
        }
        static int coininserted;
        static int galaga_customio_data_r(int offset)
        {
            //if (errorlog && customio_command != 0x71) fprintf(errorlog,"%04x: custom IO read offset %02x\n",cpu_get_pc(),offset);

            switch (customio_command)
            {
                case 0x71:	/* read input */
                case 0xb1:	/* only issued after 0xe1 (go into credit mode) */
                    if (offset == 0)
                    {
                        if (mode != 0)	/* switch mode */
                        {
                            /* bit 7 is the service switch */
                            return Mame.readinputport(4);
                        }
                        else	/* credits mode: return number of credits in BCD format */
                        {
                            int _in;

                            _in = Mame.readinputport(4);

                            /* check if the user inserted a coin */
                            if (coinpercred > 0)
                            {
                                if ((_in & 0x70) != 0x70 && credits < 99)
                                {
                                    coininserted++;
                                    if (coininserted >= coinpercred)
                                    {
                                        credits += credpercoin;
                                        coininserted = 0;
                                    }
                                }
                            }
                            else credits = 2;


                            /* check for 1 player start button */
                            if ((_in & 0x04) == 0)
                                if (credits >= 1) credits--;

                            /* check for 2 players start button */
                            if ((_in & 0x08) == 0)
                                if (credits >= 2) credits -= 2;

                            return (credits / 10) * 16 + credits % 10;
                        }
                    }
                    else if (offset == 1)
                        return Mame.readinputport(2);	/* player 1 input */
                    else if (offset == 2)
                        return Mame.readinputport(3);	/* player 2 input */

                    break;
            }

            return -1;
        }
        static int galaga_customio_r(int offset)
        {
            return customio_command;
        }
        static void galaga_nmi_generate(int param)
        {
            Mame.cpu_cause_interrupt(0, Mame.cpu_Z80.Z80_NMI_INT);
        }
        static void galaga_customio_w(int offset, int data)
        {
            //if (errorlog && data != 0x10 && data != 0x71) fprintf(errorlog, "%04x: custom IO command %02x\n", cpu_get_pc(), data);

            customio_command = data;

            switch (data)
            {
                case 0x10:
                    if (nmi_timer != null) Mame.Timer.timer_remove(nmi_timer);
                    nmi_timer = null;
                    return;

                case 0xa1:	/* go into switch mode */
                    mode = 1;
                    break;

                case 0xe1:	/* go into credit mode */
                    credits = 0;	/* this is a good time to reset the credits counter */
                    mode = 0;
                    break;
            }

            nmi_timer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_USEC(50), 0, galaga_nmi_generate);
        }
        static void galaga_halt_w(int offset, int data)
        {
            if ((data & 1) != 0)
            {
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
                Mame.cpu_set_reset_line(2, Mame.CLEAR_LINE);
            }
            else if (data == 0)
            {
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
            }
        }
        static void galaga_interrupt_enable_1_w(int offset, int data)
        {
            interrupt_enable_1 = (data & 1) != 0;
        }
        static int galaga_interrupt_1()
        {
            galaga_vh_interrupt();	/* update the background stars position */

            if (interrupt_enable_1) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void galaga_interrupt_enable_2_w(int offset, int data)
        {
            interrupt_enable_2 = (data & 1) != 0;
        }
        static int galaga_interrupt_2()
        {
            if (interrupt_enable_2) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void galaga_interrupt_enable_3_w(int offset, int data)
        {
            interrupt_enable_3 = !((data & 1) != 0);
        }
        static int galaga_interrupt_3()
        {
            if (interrupt_enable_3) return Mame.nmi_interrupt();
            else return Mame.ignore_interrupt();
        }
        static void galaga_flipscreen_w(int offset, int data)
        {

            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

        static void galaga_vh_interrupt()
        {
            /* this function is called by galaga_interrupt_1() */
            int s0, s1, s2;
            int[] speeds = { 2, 3, 4, 0, -4, -3, -2, 0 };


            s0 = galaga_starcontrol[0] & 1;
            s1 = galaga_starcontrol[1] & 1;
            s2 = galaga_starcontrol[2] & 1;

            stars_scroll -= (uint)speeds[s0 + s1 * 2 + s2 * 4];
        }
        class machine_driver_galaga : Mame.MachineDriver
        {
            public machine_driver_galaga()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu1, writemem_cpu1, null, null, galaga_interrupt_1, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu2, writemem_cpu2, null, null, galaga_interrupt_2, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3125000, readmem_cpu3, writemem_cpu3, null, null, galaga_interrupt_3, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 99;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_galaga.gfxdecodeinfo;
                total_colors = 32 + 64;
                color_table_len = 64 * 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
            }
            public override void init_machine()
            {
                galaga_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
                //#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])
                int cpi = 0;
                for (int i = 0; i < 32; i++)
                {
                    int bit0 = (color_prom[cpi + 31 - i] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi + 31 - i] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi + 31 - i] >> 2) & 0x01;
                    palette[3 * i] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = (color_prom[cpi + 31 - i] >> 3) & 0x01;
                    bit1 = (color_prom[cpi + 31 - i] >> 4) & 0x01;
                    bit2 = (color_prom[cpi + 31 - i] >> 5) & 0x01;
                    palette[3 * i + 1] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = 0;
                    bit1 = (color_prom[cpi + 31 - i] >> 6) & 0x01;
                    bit2 = (color_prom[cpi + 31 - i] >> 7) & 0x01;
                    palette[3 * i + 2] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                }

                cpi += 32;

                /* characters */
                for (int i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
                {
                    colortable[Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i]= (ushort)(15 - (color_prom[cpi++] & 0x0f));
                }
                cpi += 128;

                /* sprites */
                for (int i = 0; i < Mame.Machine.gfx[1].total_colors * Mame.Machine.gfx[1].color_granularity; i++)
                {
                    if (i % 4 == 0)
                        colortable[Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + i]= 0;
                    //COLOR(1,i) = 0;	/* preserve transparency */
                    else
                        //COLOR(1, i) = 15 - ((color_prom[cpi] & 0x0f)) + 0x10;
                        colortable[Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + i]= (ushort)(15 - (color_prom[cpi] & 0x0f) + 0x10);

                    cpi++;
                }

                cpi += 128;


                /* now the stars */
                for (int i = 32; i < 32 + 64; i++)
                {
                    int bits;
                    int[] map = { 0x00, 0x88, 0xcc, 0xff };

                    bits = ((i - 32) >> 0) & 0x03;
                    palette[3 * i] = (byte)map[bits];
                    bits = ((i - 32) >> 2) & 0x03;
                    palette[3 * i + 1] = (byte)map[bits];
                    bits = ((i - 32) >> 4) & 0x03;
                    palette[3 * i + 2] = (byte)map[bits];
                }
            }
            public override int vh_start()
            {

                int generator;
                int x, y;
                int set = 0;

                if (Generic.generic_vh_start() != 0)
                    return 1;

                /* precalculate the star background */
                /* this comes from the Galaxian hardware, Galaga is probably different */
                total_stars = 0;
                generator = 0;

                for (y = 0; y <= 255; y++)
                {
                    for (x = 511; x >= 0; x--)
                    {
                        int bit1, bit2;


                        generator <<= 1;
                        bit1 = (~generator >> 17) & 1;
                        bit2 = (generator >> 5) & 1;

                        if ((bit1 ^ bit2) != 0) generator |= 1;

                        if (((~generator >> 16) & 1) != 0 && (generator & 0xff) == 0xff)
                        {
                            int color;

                            color = (~(generator >> 8)) & 0x3f;
                            if (color != 0 && total_stars < MAX_STARS)
                            {
                                stars[total_stars].x = x;
                                stars[total_stars].y = y;
                                stars[total_stars].col = Mame.Machine.pens[color + STARS_COLOR_BASE];
                                stars[total_stars].set = set;
                                if (++set > 3)
                                    set = 0;

                                total_stars++;
                            }
                        }
                    }
                }

                return 0;
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int offs;


                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy, mx, my;

                        Generic.dirtybuffer[offs] = false;

                        /* Even if Galaga's screen is 28x36, the memory layout is 32x32. We therefore */
                        /* have to convert the memory coordinates into screen coordinates. */
                        /* Note that 32*32 = 1024, while 28*36 = 1008: therefore 16 bytes of Video RAM */
                        /* don't map to a screen position. We don't check that here, however: range */
                        /* checking is performed by drawgfx(). */

                        mx = offs % 32;
                        my = offs / 32;

                        if (my <= 1)
                        {
                            sx = my + 34;
                            sy = mx - 2;
                        }
                        else if (my >= 30)
                        {
                            sx = my - 30;
                            sy = mx - 2;
                        }
                        else
                        {
                            sx = mx + 2;
                            sy = my - 2;
                        }

                        if (flipscreen != 0)
                        {
                            sx = 35 - sx;
                            sy = 27 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                Generic.videoram[offs],
                                Generic.colorram[offs],
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. */
                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    if ((Generic.spriteram_3[offs + 1] & 2) == 0)
                    {
                        int code, color, sx, sy, sfa, sfb;
                        bool flipx, flipy;

                        code = Generic.spriteram[offs];
                        color = Generic.spriteram[offs + 1];
                        flipx = (Generic.spriteram_3[offs] & 1) != 0;
                        flipy = (Generic.spriteram_3[offs] & 2) != 0;
                        sx = Generic.spriteram_2[offs + 1] - 40 + 0x100 * (Generic.spriteram_3[offs + 1] & 1);
                        sy = 28 * 8 - Generic.spriteram_2[offs];
                        sfa = 0;
                        sfb = 16;

                        /* this constraint fell out of the old, pre-rotated code automatically */
                        /* we need to explicitly add it because of the way we flip Y */
                        if (sy <= -16)
                            continue;

                        if (flipscreen != 0)
                        {
                            flipx = !flipx;
                            flipy = !flipy;
                            sfa = 16;
                            sfb = 0;
                        }

                        if ((Generic.spriteram_3[offs] & 0x0c) == 0x0c)		/* double width, double height */
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code + 2, (uint)color, flipx, flipy, sx + sfa, sy - sfa,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code, (uint)color, flipx, flipy, sx + sfa, sy - sfb,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code + 3, (uint)color, flipx, flipy, sx + sfb, sy - sfa,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code + 1, (uint)color, flipx, flipy, sx + sfb, sy - sfb,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                        }
                        else if ((Generic.spriteram_3[offs] & 8) != 0)	/* double width */
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code + 2, (uint)color, flipx, flipy, sx, sy - sfa,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code, (uint)color, flipx, flipy, sx, sy - sfb,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                        }
                        else if ((Generic.spriteram_3[offs] & 4) != 0)	/* double height */
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code, (uint)color, flipx, flipy, sx + sfa, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)code + 1, (uint)color, flipx, flipy, sx + sfb, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                        }
                        else	/* normal */
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                   (uint)code, (uint)color, flipx, flipy, sx, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_THROUGH, Mame.Machine.pens[0]);
                    }
                }


                /* draw the stars */
                if ((galaga_starcontrol[5] & 1) != 0)
                {
                    int bpen;


                    bpen = Mame.Machine.pens[0];
                    for (offs = 0; offs < total_stars; offs++)
                    {
                        int x, y;
                        int set;
                        int[][] starset = { new int[] { 0, 3 }, new int[] { 0, 1 }, new int[] { 2, 3 }, new int[] { 2, 1 } };

                        set = ((galaga_starcontrol[4] << 1) | galaga_starcontrol[3]) & 3;
                        if ((stars[offs].set == starset[set][0]) ||
                            (stars[offs].set == starset[set][1]))
                        {
                            x = (int)(((stars[offs].x + stars_scroll) % 512) / 2 + 16);
                            y = (int)((stars[offs].y + (stars_scroll + stars[offs].x) / 512) % 256);

                            if (y >= Mame.Machine.drv.visible_area.min_y &&
                                y <= Mame.Machine.drv.visible_area.max_y)
                            {
                                if (Mame.read_pixel(bitmap, x, y) == bpen)
                                    Mame.plot_pixel(bitmap, x, y, stars[offs].col);
                            }
                        }
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.InputPortTiny[] input_ports_galaganm()
        {
            INPUT_PORTS_START("galaganm");
            PORT_START("DSW0");
            PORT_DIPNAME(0x07, 0x07, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x04, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            /* TODO: bonus scores are different for 5 lives */
            PORT_DIPNAME(0x38, 0x10, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x20, "20K 60K 60K");
            PORT_DIPSETTING(0x18, "20K 60K");
            PORT_DIPSETTING(0x10, "20K 70K 70K");
            PORT_DIPSETTING(0x30, "20K 80K 80K");
            PORT_DIPSETTING(0x38, "30K 80K");
            PORT_DIPSETTING(0x08, "30K 100K 100K");
            PORT_DIPSETTING(0x28, "30K 120K 120K");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0xc0, 0x80, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x80, "3");
            PORT_DIPSETTING(0x40, "4");
            PORT_DIPSETTING(0xc0, "5");

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x03, "Easy");
            PORT_DIPSETTING(0x00, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x02, "Hardest");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x04, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x10, "Freeze");
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x20, 0x20, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));

            PORT_START("FAKE");
            /* The player inputs are not memory mapped, they are handled by an I/O chip. */
            /* These fake input ports are read by galaga_customio_data_r() */
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.CODE_PREVIOUS, (ushort)Mame.InputCodes.CODE_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("FAKE");
            PORT_BIT(0x01, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_COCKTAIL);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, 1);
            PORT_BITX(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL, null, (ushort)Mame.InputCodes.CODE_PREVIOUS, (ushort)Mame.InputCodes.CODE_PREVIOUS);
            PORT_BIT(0xc0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START("FAKE");
            /* the button here is used to trigger the sound in the test screen */
            PORT_BITX(0x03, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.CODE_DEFAULT, (ushort)Mame.InputCodes.CODE_DEFAULT);
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START2, 1);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 1);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 1);
            PORT_SERVICE(0x80, IP_ACTIVE_LOW);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_galaga()
        {

            ROM_START("galaga");
            ROM_REGION(0x10000, Mame.REGION_CPU1);   /* 64k for code for the first CPU  */
            ROM_LOAD("04m_g01.bin", 0x0000, 0x1000, 0xa3a0f743);
            ROM_LOAD("04k_g02.bin", 0x1000, 0x1000, 0x43bb0d5c);
            ROM_LOAD("04j_g03.bin", 0x2000, 0x1000, 0x753ce503);
            ROM_LOAD("04h_g04.bin", 0x3000, 0x1000, 0x83874442);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for the second CPU */
            ROM_LOAD("04e_g05.bin", 0x0000, 0x1000, 0x3102fccd);

            ROM_REGION(0x10000, Mame.REGION_CPU3);    /* 64k for the third CPU  */
            ROM_LOAD("04d_g06.bin", 0x0000, 0x1000, 0x8995088d);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("07m_g08.bin", 0x0000, 0x1000, 0x58b2f47c);

            ROM_REGION(0x2000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("07e_g10.bin", 0x0000, 0x1000, 0xad447c80);
            ROM_LOAD("07h_g09.bin", 0x1000, 0x1000, 0xdd6f1afc);

            ROM_REGION(0x0320, Mame.REGION_PROMS);
            ROM_LOAD("5n.bin", 0x0000, 0x0020, 0x54603c6b);	/* palette */
            ROM_LOAD("2n.bin", 0x0020, 0x0100, 0xa547d33b);	/* char lookup table */
            ROM_LOAD("1c.bin", 0x0120, 0x0100, 0xb6f585fb);	/* sprite lookup table */
            ROM_LOAD("5c.bin", 0x0220, 0x0100, 0x8bd565f6);	/* unknown */

            ROM_REGION(0x0100, Mame.REGION_SOUND1);	/* sound prom */
            ROM_LOAD("1d.bin", 0x0000, 0x0100, 0x86d92b24);
            return ROM_END;
        }
        public driver_galaga()
        {
            drv = new machine_driver_galaga();
            year = "1981";
            name = "galaga";
            description = "Galaga (Namco)";
            manufacturer = "Namco";
            flags = Mame.ROT90;
            input_ports = input_ports_galaganm();
            rom = rom_galaga();
            drv.HasNVRAMhandler = false;
        }
    }
}
