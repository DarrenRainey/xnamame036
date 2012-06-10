using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_tp84 : Mame.GameDriver
    {
        static _BytePtr sharedram = new _BytePtr(1);
        static _BytePtr tp84_videoram2 = new _BytePtr(1);
        static _BytePtr tp84_colorram2 = new _BytePtr(1);
        static _BytePtr tp84_scrollx = new _BytePtr(1);
        static _BytePtr tp84_scrolly = new _BytePtr(1);
        static _BytePtr dirtybuffer2 = new _BytePtr(1);
        static int col0;
        static Mame.osd_bitmap tmpbitmap2;
        static Mame.rectangle topvisiblearea = new Mame.rectangle(0 * 8, 2 * 8 - 1, 2 * 8, 30 * 8 - 1);
        static Mame.rectangle bottomvisiblearea = new Mame.rectangle(30 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);

        static int sharedram_r(int offset)
        {
            return sharedram[offset];
        }
        static void sharedram_w(int offset, int data)
        {
            sharedram[offset] = (byte)data;
        }
        static int tp84_sh_timer_r(int offset)
        {
            /* main xtal 14.318MHz, divided by 4 to get the CPU clock, further */
            /* divided by 2048 to get this timer */
            /* (divide by (2048/2), and not 1024, because the CPU cycle counter is */
            /* incremented every other state change of the clock) */
            return (Mame.cpu_gettotalcycles() / (2048 / 2)) & 0x0f;
        }

        static void tp84_filter_w(int offset, int data)
        {
            int C;

            /* 76489 #0 */
            C = 0;
            if ((offset & 0x008) != 0) C += 47000;	/*  47000pF = 0.047uF */
            if ((offset & 0x010) != 0) C += 470000;	/* 470000pF = 0.47uF */
            Mame.set_RC_filter(0, 1000, 2200, 1000, C);

            /* 76489 #1 (optional) */
            C = 0;
            if ((offset & 0x020) != 0) C += 47000;	/*  47000pF = 0.047uF */
            if ((offset & 0x040) != 0) C += 470000;	/* 470000pF = 0.47uF */
            //	set_RC_filter(1,1000,2200,1000,C);

            /* 76489 #2 */
            C = 0;
            if ((offset & 0x080) != 0) C += 470000;	/* 470000pF = 0.47uF */
            Mame.set_RC_filter(1, 1000, 2200, 1000, C);

            /* 76489 #3 */
            C = 0;
            if ((offset & 0x100) != 0) C += 470000;	/* 470000pF = 0.47uF */
            Mame.set_RC_filter(2, 1000, 2200, 1000, C);
        }

        static void tp84_sh_irqtrigger_w(int offset, int data)
        {
            Mame.cpu_cause_interrupt(2, 0xff);
        }

        /* CPU 1 read addresses */
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x2800, 0x2800, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0x2820, 0x2820, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0x2840, 0x2840, Mame.input_port_2_r ),
	new Mame.MemoryReadAddress( 0x2860, 0x2860, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x3000, 0x3000, Mame.input_port_4_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x4fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x5000, 0x57ff, sharedram_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        /* CPU 1 write addresses */
        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, Mame.MWA_RAM ), /*Watch dog?*/
	new Mame.MemoryWriteAddress( 0x2800, 0x2800, tp84_col0_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3800, tp84_sh_irqtrigger_w ),
	new Mame.MemoryWriteAddress( 0x3a00, 0x3a00, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3c00, Mame.MWA_RAM, tp84_scrollx ), /* Y scroll */
	new Mame.MemoryWriteAddress( 0x3e00, 0x3e00, Mame.MWA_RAM, tp84_scrolly ), /* X scroll */
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff, Generic.videoram_w, Generic.videoram , Generic.videoram_size),
	new Mame.MemoryWriteAddress( 0x4400, 0x47ff, tp84_videoram2_w, tp84_videoram2 ),
	new Mame.MemoryWriteAddress( 0x4800, 0x4bff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0x4c00, 0x4fff, tp84_colorram2_w, tp84_colorram2 ),
	new Mame.MemoryWriteAddress( 0x5000, 0x57ff, sharedram_w, sharedram ),
	new Mame.MemoryWriteAddress( 0x5000, 0x5177, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),	/* FAKE (see below) */
	new Mame.MemoryWriteAddress( 0x8000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        /* CPU 2 read addresses */
        static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0000, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x2000, 0x2000, tp84_beam_r ), /* beam position */
	new Mame.MemoryReadAddress( 0x6000, 0x67ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, sharedram_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        /* CPU 2 write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0000, Mame.MWA_RAM ), /* Watch dog ?*/
	new Mame.MemoryWriteAddress( 0x4000, 0x4000, tp84_catchloop_w ), /* IRQ enable */ /* JB 970829 */
	new Mame.MemoryWriteAddress( 0x6000, 0x67ff, Mame.MWA_RAM ),
//	new Mame.MemoryWriteAddress( 0x67a0, 0x67ff, MWA_RAM, &spriteram, &spriteram_size },	/* REAL (multiplexed) */
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, sharedram_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x43ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0x8000, 0x8000, tp84_sh_timer_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x43ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa1ff, tp84_filter_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xc001, 0xc001, SN76496.SN76496_0_w ),
	new Mame.MemoryWriteAddress( 0xc003, 0xc003, SN76496.SN76496_1_w ),
	new Mame.MemoryWriteAddress( 0xc004, 0xc004, SN76496.SN76496_2_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            1024,	/* 1024 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 4, 0 },	/* the two bitplanes for 4 pixels are packed into one byte */
            new uint[] { 0, 1, 2, 3, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },	/* bits are packed in groups of four */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8	/* every char takes 16 bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 256 * 64 * 8 + 4, 256 * 64 * 8 + 0, 4, 0 },
            new uint[]{ 0, 1, 2, 3, 8*8+0, 8*8+1, 8*8+2, 8*8+3,
			16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8	/* every sprite takes 64 bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
    {
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,        0, 64*8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout, 64*4*8, 16*8 ),
};

        /* JB 970829 - just give it what it wants
        F104: LDX   $6400
        F107: LDU   $6402
        F10A: LDA   $640B
        F10D: BEQ   $F13B
        F13B: LDX   $6404
        F13E: LDU   $6406
        F141: LDA   $640C
        F144: BEQ   $F171
        F171: LDA   $2000	; read beam
        F174: ADDA  #$20
        F176: BCC   $F104
        */
        static int tp84_beam_r(int offset)
        {
            //	return cpu_getscanline();
            return 255; /* always return beam position 255 */ /* JB 970829 */
        }

        /* JB 970829 - catch a busy loop for CPU 1
            E0ED: LDA   #$01
            E0EF: STA   $4000
            E0F2: BRA   $E0ED
        */
        static void tp84_catchloop_w(int offset, int data)
        {
            if (Mame.cpu_get_pc() == 0xe0f2) Mame.cpu_spinuntil_int();
        }

        static Mame.SN76496interface sn76496_interface =
        new Mame.SN76496interface(
            3,	/* 3 chips */
            new int[] { 14318180 / 8, 14318180 / 8, 14318180 / 8 },
            new int[] { 75, 75, 75 }
        );
        static void tp84_videoram2_w(int offset, int data)
        {
            if (tp84_videoram2[offset] != data)
            {
                dirtybuffer2[offset] = 1;

                tp84_videoram2[offset] = (byte)data;
            }
        }
        static void tp84_colorram2_w(int offset, int data)
        {
            if (tp84_colorram2[offset] != data)
            {
                dirtybuffer2[offset] = 1;

                tp84_colorram2[offset] = (byte)data;
            }
        }
        /*****
          col0 is a register to index the color Proms
        *****/
        static void tp84_col0_w(int offset, int data)
        {
            if (col0 != data)
            {
                col0 = data;
                for (int i = 0; i < Generic.videoram_size[0]; i++)
                {
                    Generic.dirtybuffer[i] = true;
                    dirtybuffer2[i] = 1;
                }
            }
        }

        class machine_driver_tp84 : Mame.MachineDriver
        {
            public machine_driver_tp84()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1500000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1500000, readmem_cpu2, writemem_cpu2, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318180 / 4, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                gfxdecodeinfo = driver_tp84.gfxdecodeinfo;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                total_colors = 256;
                color_table_len = 4096;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SN76496, sn76496_interface));
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
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x42 * bit2 + 0x90 * bit3);
                    /* green component */
                    bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x42 * bit2 + 0x90 * bit3);
                    /* blue component */
                    bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x42 * bit2 + 0x90 * bit3);

                    cpi++;
                }

                cpi += (2 * Mame.Machine.drv.total_colors);
                /* color_prom now points to the beginning of the lookup table */

                /* characters use colors 128-255 */
                for (int i = 0; i < TOTAL_COLORS(0)/ 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                        COLOR(colortable,0, i + 256 * j, (ushort)(color_prom[cpi] + 128 + 16 * j));

                    cpi++;
                }

                /* sprites use colors 0-127 */
                for (int i = 0; i < TOTAL_COLORS(1) / 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (color_prom[cpi] != 0)
                            COLOR(colortable,1, i + 256 * j,(ushort)(color_prom[cpi] + 16 * j));
                        else
                            COLOR(colortable, 1, i + 256 * j, 0);	/* preserve transparency */
                    }

                    cpi++;
                }
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0)
                    return 1;

                dirtybuffer2 = new _BytePtr(Generic.videoram_size[0]);
                for (int i = 0; i < Generic.videoram_size[0]; i++) dirtybuffer2[i] = 1;

                tmpbitmap2 = Mame.osd_create_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

                return 0;
            }
            public override void vh_stop()
            {
                dirtybuffer2 = null;
                Mame.osd_free_bitmap(tmpbitmap2);
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                int coloffset = ((col0 & 0x18) << 1) + ((col0 & 0x07) << 6);

                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + ((Generic.colorram[offs] & 0x30) << 4)),
                                (uint)((Generic.colorram[offs] & 0x0f) + coloffset),
                                (Generic.colorram[offs] & 0x40) != 0, (Generic.colorram[offs] & 0x80) != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }

                    if (dirtybuffer2[offs] != 0)
                    {
                        dirtybuffer2[offs] = 0;

                        int sx = offs % 32;
                        int sy = offs / 32;

                        /* Skip the middle of the screen, this ram seem to be used as normal ram. */
                        if (sx < 2 || sx >= 30)
                            Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[0],
                                    (uint)(tp84_videoram2[offs] + ((tp84_colorram2[offs] & 0x30) << 4)),
                                    (uint)((tp84_colorram2[offs] & 0x0f) + coloffset),
                                    (tp84_colorram2[offs] & 0x40) != 0, (tp84_colorram2[offs] & 0x80) != 0,
                                    8 * sx, 8 * sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int scrollx = -tp84_scrollx[0];
                    int scrolly = -tp84_scrolly[0];

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[] { scrollx }, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                /* Draw the sprites. */
                coloffset = ((col0 & 0x07) << 4);
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int sx, sy;
                    bool flipx, flipy;


                    sx = Generic.spriteram[offs + 0];
                    sy = 240 - Generic.spriteram[offs + 3];
                    flipx = (Generic.spriteram[offs + 2] & 0x40) == 0;
                    flipy = (Generic.spriteram[offs + 2] & 0x80) != 0;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)Generic.spriteram[offs + 1],
                            (uint)((Generic.spriteram[offs + 2] & 0x0f) + coloffset),
                            flipx, flipy,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0);
                }


                /* Copy the frontmost playfield. */
                Mame.copybitmap(bitmap, tmpbitmap2, false, false, 0, 0, topvisiblearea, Mame.TRANSPARENCY_NONE, 0);
                Mame.copybitmap(bitmap, tmpbitmap2, false, false, 0, 0, bottomvisiblearea, Mame.TRANSPARENCY_NONE, 0);
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
        Mame.InputPortTiny[] input_ports_tp84()
        {

            INPUT_PORTS_START("tp84");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("DSW0");
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
            PORT_DIPSETTING(0x00, "Invalid");

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x02, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "2");
            PORT_DIPSETTING(0x02, "3");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "7");
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x18, 0x18, "Bonus");
            PORT_DIPSETTING(0x18, "10000 50000");
            PORT_DIPSETTING(0x10, "20000 60000");
            PORT_DIPSETTING(0x08, "30000 70000");
            PORT_DIPSETTING(0x00, "40000 80000");
            PORT_DIPNAME(0x60, 0x60, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x60, "Easy");
            PORT_DIPSETTING(0x40, "Normal");
            PORT_DIPSETTING(0x20, "Medium");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_tp84()
        {
            ROM_START("tp84");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("tp84_7j.bin", 0x8000, 0x2000, 0x605f61c7);
            ROM_LOAD("tp84_8j.bin", 0xa000, 0x2000, 0x4b4629a4);
            ROM_LOAD("tp84_9j.bin", 0xc000, 0x2000, 0xdbd5333b);
            ROM_LOAD("tp84_10j.bin", 0xe000, 0x2000, 0xa45237c4);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("tp84_10d.bin", 0xe000, 0x2000, 0x36462ff1);

            ROM_REGION(0x10000, Mame.REGION_CPU3);	/* 64k for code of sound cpu Z80 */
            ROM_LOAD("tp84s_6a.bin", 0x0000, 0x2000, 0xc44414da);

            ROM_REGION(0x4000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("tp84_2j.bin", 0x0000, 0x2000, 0x05c7508f); /* chars */
            ROM_LOAD("tp84_1j.bin", 0x2000, 0x2000, 0x498d90b7);

            ROM_REGION(0x8000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("tp84_12a.bin", 0x0000, 0x2000, 0xcd682f30);/* sprites */
            ROM_LOAD("tp84_13a.bin", 0x2000, 0x2000, 0x888d4bd6);
            ROM_LOAD("tp84_14a.bin", 0x4000, 0x2000, 0x9a220b39);
            ROM_LOAD("tp84_15a.bin", 0x6000, 0x2000, 0xfac98397);

            ROM_REGION(0x0500, Mame.REGION_PROMS);
            ROM_LOAD("tp84_2c.bin", 0x0000, 0x0100, 0xd737eaba); /* palette red component */
            ROM_LOAD("tp84_2d.bin", 0x0100, 0x0100, 0x2f6a9a2a); /* palette green component */
            ROM_LOAD("tp84_1e.bin", 0x0200, 0x0100, 0x2e21329b); /* palette blue component */
            ROM_LOAD("tp84_1f.bin", 0x0300, 0x0100, 0x61d2d398); /* char lookup table */
            ROM_LOAD("tp84_16c.bin", 0x0400, 0x0100, 0x13c4e198); /* sprite lookup table */
            return ROM_END;
        }
        public driver_tp84()
        {
            drv = new machine_driver_tp84();
            year = "1984";
            name = "tp84";
            description = "Time Pilot '84 (set 1)";
            manufacturer = "Konami";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_tp84();
            rom = rom_tp84();
            drv.HasNVRAMhandler = false;
        }
    }
}
