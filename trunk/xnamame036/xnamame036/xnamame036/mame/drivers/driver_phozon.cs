using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_phozon : Mame.GameDriver
    {
        static _BytePtr phozon_snd_sharedram = new _BytePtr(1);
        static _BytePtr phozon_spriteram = new _BytePtr(1);
        static _BytePtr phozon_customio_1 =new _BytePtr(1), phozon_customio_2= new _BytePtr(1);
        static int credits, coincounter1, coincounter2;

        /* CPU 1 (MAIN CPU) read addresses */
        static Mame.MemoryReadAddress[] readmem_cpu1 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Generic.videoram_r ),			/* video RAM */
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Generic.colorram_r ),										/* color RAM */
	new Mame.MemoryReadAddress( 0x0800, 0x1fff, phozon_spriteram_r ),			/* shared RAM with CPU #2/sprite RAM*/
	new Mame.MemoryReadAddress( 0x4040, 0x43ff, phozon_snd_sharedram_r ),  /* shared RAM with CPU #3 */
	new Mame.MemoryReadAddress( 0x4800, 0x480f, phozon_customio_r_1 ),		/* custom I/O chip #1 interface */
	new Mame.MemoryReadAddress( 0x4810, 0x481f, phozon_customio_r_2 ),		/* custom I/O chip #2 interface */
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),										/* ROM */
	new Mame.MemoryReadAddress( -1 )																/* end of table */
};

        /* CPU 1 (MAIN CPU) write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu1 =
{
	new Mame.MemoryWriteAddress(0x0000, 0x03ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),				/* video RAM */
	new Mame.MemoryWriteAddress(0x0400, 0x07ff, Generic.colorram_w, Generic.colorram ),  /* color RAM */
	new Mame.MemoryWriteAddress(0x0800, 0x1fff, phozon_spriteram_w, phozon_spriteram ),		/* shared RAM with CPU #2/sprite RAM*/
	new Mame.MemoryWriteAddress(0x4000, 0x403f, Mame.MWA_RAM ),				/* initialized but probably unused */
	new Mame.MemoryWriteAddress(0x4040, 0x43ff, phozon_snd_sharedram_w, phozon_snd_sharedram ), /* shared RAM with CPU #3 */
	new Mame.MemoryWriteAddress(0x4800, 0x480f, phozon_customio_w_1, phozon_customio_1 ),	/* custom I/O chip #1 interface */
	new Mame.MemoryWriteAddress(0x4810, 0x481f, phozon_customio_w_2, phozon_customio_2 ),	/* custom I/O chip #2 interface */
	new Mame.MemoryWriteAddress(0x4820, 0x483f, Mame.MWA_RAM ),				/* initialized but probably unused */
	new Mame.MemoryWriteAddress(0x5000, 0x5007, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress(0x5008, 0x5008, phozon_cpu3_reset_w ),	/* reset SOUND CPU? */
	new Mame.MemoryWriteAddress(0x5009, 0x5009, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress(0x500a, 0x500b, phozon_cpu3_enable_w ),	/* SOUND CPU enable */
	new Mame.MemoryWriteAddress(0x500c, 0x500d, phozon_cpu2_enable_w ),	/* SUB CPU enable */
	new Mame.MemoryWriteAddress(0x500e, 0x500f, Mame.MWA_NOP ),				/* ??? */
	new Mame.MemoryWriteAddress(0x7000, 0x7000, Mame.watchdog_reset_w ),	 	/* watchdog reset */
	new Mame.MemoryWriteAddress(0x8000, 0xffff, Mame.MWA_ROM ),				/* ROM */
	new Mame.MemoryWriteAddress(-1 )										/* end of table */
};

        /* CPU 2 (SUB CPU) read addresses */
        static Mame.MemoryReadAddress[] readmem_cpu2 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Generic.videoram_r ),			/* video RAM */
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Generic.colorram_r ),			/* color RAM */
	new Mame.MemoryReadAddress( 0x0800, 0x1fff, phozon_spriteram_r ),	/* shared RAM with CPU #1/sprite RAM*/
	new Mame.MemoryReadAddress( 0xa000, 0xa7ff, Mame.MRA_RAM ),			/* RAM */
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_ROM ),			/* ROM */
	new Mame.MemoryReadAddress( -1 )									/* end of table */
};

        /* CPU 2 (SUB CPU) write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x03ff, Generic.videoram_w ),			/* video RAM */
	new Mame.MemoryWriteAddress( 0x0400, 0x07ff, Generic.colorram_w ),			/* color RAM */
	new Mame.MemoryWriteAddress( 0x0800, 0x1fff, phozon_spriteram_w ),	/* shared RAM with CPU #1/sprite RAM*/
	new Mame.MemoryWriteAddress( 0xa000, 0xa7ff, Mame.MWA_RAM ),			/* RAM */
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM),			/* ROM */
	new Mame.MemoryWriteAddress( -1 )									/* end of table */
};

        /* CPU 3 (SOUND CPU) read addresses */
        static Mame.MemoryReadAddress[] readmem_cpu3 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x003f, Mame.MRA_RAM ),				/* sound registers */
	new Mame.MemoryReadAddress( 0x0040, 0x03ff, phozon_snd_sharedram_r), /* shared RAM with CPU #1 */
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_ROM ),				/* ROM */
	new Mame.MemoryReadAddress( -1 )										/* end of table */
};

        /* CPU 3 (SOUND CPU) write addresses */
        static Mame.MemoryWriteAddress[] writemem_cpu3 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x003f, Namco.mappy_sound_w, Namco.namco_soundregs),/* sound registers */
	new Mame.MemoryWriteAddress( 0x0040, 0x03ff, phozon_snd_sharedram_w ),			/* shared RAM with the main CPU */
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),						/* ROM */
	new Mame.MemoryWriteAddress( -1 )												/* end of table */
};
        static Mame.GfxLayout charlayout =
new Mame.GfxLayout(
    8, 8,            /* 8*8 characters */
    256,            /* 256 characters */
    2,				/* 2 bits per pixel */
    new uint[] { 0, 4 },
    new uint[] { 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 0, 1, 2, 3 },   /* bits are packed in groups of four */
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },   /* characters are rotated 90 degrees */
    16 * 8			/* every char takes 16 bytes */
);

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,                                         /* 16*16 sprites */
            128,                                           /* 128 sprites */
            2,                                             /* 2 bits per pixel */
            new uint[] { 0, 4 },
            new uint[]{ 0, 1, 2, 3, 8*8, 8*8+1, 8*8+2, 8*8+3,
		16*8+0, 16*8+1, 16*8+2, 16*8+3, 24*8+0, 24*8+1, 24*8+2, 24*8+3 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
		32*8, 33*8, 34*8, 35*8, 36*8, 37*8, 38*8, 39*8 },
            64 * 8                                           /* every sprite takes 64 bytes */
        );

        static Mame.GfxLayout spritelayout8 =
        new Mame.GfxLayout(
            8, 8,                                         /* 16*16 sprites */
            512,                                           /* 128 sprites */
            2,                                             /* 2 bits per pixel */
            new uint[] { 0, 4 },
            new uint[] { 0, 1, 2, 3, 8 * 8, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8                                           /* every sprite takes 64 bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,       0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, charlayout,       0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout,  64*4, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout8, 64*4, 64 ),
};

        static Namco_interface namco_interface =
        new Namco_interface(
            23920,	/* sample rate (approximate value) */
            8,		/* number of voices */
            100,	/* playback volume */
            Mame.REGION_SOUND1	/* memory region */
        );


        static int phozon_spriteram_r(int offset)
        {
            return phozon_spriteram[offset];
        }

        static void phozon_spriteram_w(int offset, int data)
        {
            phozon_spriteram[offset] = (byte)data;
        }

        static int phozon_snd_sharedram_r(int offset)
        {
            return phozon_snd_sharedram[offset];
        }

        static void phozon_snd_sharedram_w(int offset, int data)
        {
            phozon_snd_sharedram[offset] = (byte)data;
        }

        /* cpu control functions */
        static void phozon_cpu2_enable_w(int offset, int data)
        {
            Mame.cpu_set_halt_line(1, offset != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }

        static void phozon_cpu3_enable_w(int offset, int data)
        {
            Mame.cpu_set_halt_line(2, offset != 0 ? Mame.CLEAR_LINE : Mame.ASSERT_LINE);
        }

        static void phozon_cpu3_reset_w(int offset, int data)
        {
            Mame.cpu_set_reset_line(2, Mame.PULSE_LINE);
        }

        static void phozon_customio_w_1(int offset, int data)
        {
            phozon_customio_1[offset] = (byte)data;
        }

        static void phozon_customio_w_2(int offset, int data)
        {
            phozon_customio_2[offset] = (byte)data;
        }

        static int[] credmoned = { 1, 1, 1, 1, 1, 2, 2, 3 };
        static int[] monedcred = { 1, 2, 3, 6, 7, 1, 3, 1 };

        static int lastval1;
        static int lastval2;

        static int phozon_customio_r_1(int offset)
        {
            int mode, val, temp1, temp2;

            mode = phozon_customio_1[8];
            if (mode == 3)	/* normal mode */
            {
                switch (offset)
                {
                    case 0:     /* Coin slots */
                        {

                            val = (Mame.readinputport(2) >> 4) & 0x03;
                            temp1 = Mame.readinputport(0) & 0x07;
                            temp2 = (Mame.readinputport(0) >> 5) & 0x07;

                            /* bit 0 is a trigger for the coin slot 1 */
                            if ((val & 1) != 0 && ((val ^ lastval1) & 1) != 0)
                            {
                                coincounter1++;
                                if (coincounter1 >= credmoned[temp1])
                                {
                                    credits += monedcred[temp1];
                                    coincounter1 -= credmoned[temp1];
                                }
                            }
                            /* bit 1 is a trigger for the coin slot 2 */
                            if ((val & 2) != 0 && ((val ^ lastval1) & 2) != 0)
                            {
                                coincounter2++;
                                if (coincounter2 >= credmoned[temp2])
                                {
                                    credits += monedcred[temp2];
                                    coincounter2 -= credmoned[temp2];
                                }
                            }

                            if (credits > 99)
                                credits = 99;

                            return lastval1 = val;
                        }
                    case 1:
                        {

                            val = Mame.readinputport(2) & 0x03;
                            temp1 = Mame.readinputport(0) & 0x07;
                            temp2 = (Mame.readinputport(0) >> 5) & 0x07;

                            /* bit 0 is a trigger for the 1 player start */
                            if ((val & 1) != 0 && ((val ^ lastval2) & 1) != 0)
                            {
                                if (credits > 0)
                                    credits--;
                                else
                                    val &= ~1;   /* otherwise you can start with no credits! */
                            }
                            /* bit 1 is a trigger for the 2 player start */
                            if ((val & 2) != 0 && ((val ^ lastval2) & 2) != 0)
                            {
                                if (credits >= 2)
                                    credits -= 2;
                                else
                                    val &= ~2;   /* otherwise you can start with no credits! */
                            }
                            return lastval2 = val;
                        }
                    case 2: return (credits / 10);      /* high BCD of credits */
                    case 3: return (credits % 10);      /* low BCD of credits */
                    case 4: return (Mame.readinputport(3) & 0x0f);   /* 1P controls */
                    case 5: return (Mame.readinputport(4) & 0x03);   /* 1P button 1 */
                    default: return 0x0;
                }
            }
            else if (mode == 5)	/* IO tests */
            {
                switch (offset)
                {
                    case 0x00: val = 0x00; break;
                    case 0x01: val = 0x02; break;
                    case 0x02: val = 0x03; break;
                    case 0x03: val = 0x04; break;
                    case 0x04: val = 0x05; break;
                    case 0x05: val = 0x06; break;
                    case 0x06: val = 0x0c; break;
                    case 0x07: val = 0x0a; break;
                    default:
                        val = phozon_customio_1[offset];
                        break;
                }
            }
            else if (mode == 1)	/* test mode controls */
            {
                switch (offset)
                {
                    case 4: return (Mame.readinputport(2) & 0x03);	/* start 1 & 2 */
                    case 5: return (Mame.readinputport(3) & 0x0f);	/* 1P controls */
                    case 7: return (Mame.readinputport(4) & 0x03);	/* 1P button 1 */
                    default:
                        return phozon_customio_1[offset];
                }
            }
            else
                val = phozon_customio_1[offset];
            return val;
        }

        static int phozon_customio_r_2(int offset)
        {
            int mode, val;

            mode = phozon_customio_2[8];
            if (mode == 8)	/* IO tests */
            {
                switch (offset)
                {
                    case 0x00: val = 0x01; break;
                    case 0x01: val = 0x0c; break;
                    default:
                        val = phozon_customio_2[offset];
                        break;
                }
            }
            else if (mode == 9)
            {
                switch (offset)	/* TODO: coinage B & check bonus life bits */
                {
                    case 0:
                        val = (Mame.readinputport(0) & 0x08) >> 3;		/* lives (bit 0) */
                        val |= (Mame.readinputport(0) & 0x01) << 2;	/* coinage A (bit 0) */
                        val |= (Mame.readinputport(0) & 0x04) << 1;	/* coinage A (bit 2) */
                        break;
                    case 1:
                        val = (Mame.readinputport(0) & 0x10) >> 4;		/* lives (bit 1) */
                        val |= (Mame.readinputport(1) & 0xc0) >> 5;	/* bonus life (bits 1 & 0) */
                        val |= (Mame.readinputport(0) & 0x02) << 2;	/* coinage A (bit 1) */
                        break;
                    case 2:
                        val = (Mame.readinputport(1) & 0x07) << 1;		/* rank */
                        break;
                    case 4:	/* some bits of coinage B (not implemented yet) */
                        val = 0;
                        break;
                    case 6:
                        val = Mame.readinputport(1) & 0x08;			/* test mode */
                        val |= (Mame.readinputport(2) & 0x80) >> 5;	/* cabinet */
                        break;
                    default:
                        val = phozon_customio_2[offset];
                        break;
                }
            }
            else
                val = phozon_customio_2[offset];
            return val;
        }


        class machine_driver_phozon : Mame.MachineDriver
        {
            public machine_driver_phozon()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu1, writemem_cpu1, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu2, writemem_cpu2, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1536000, readmem_cpu3, writemem_cpu3, null, null, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 36 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_phozon.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 64 * 8;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_NAMCO, namco_interface));
            }
            public override void init_machine()
            {
                credits = coincounter1 = coincounter2 = 0;
                Mame.cpu_set_halt_line(1, Mame.CLEAR_LINE);
                Mame.cpu_set_halt_line(2, Mame.CLEAR_LINE);
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
                //	#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])
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
                /* color_prom now points to the beginning of the lookup table */

                /* characters */
                for (int i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
                COLOR(colortable,0,i, (color_prom[cpi++] & 0x0f));
                /* sprites */
                for (int i = 0; i < Mame.Machine.gfx[2].total_colors * Mame.Machine.gfx[2].color_granularity; i++)
                COLOR(colortable,2,i, (color_prom[cpi++] & 0x0f) + 0x10);
            }
            public override int vh_start()
            {
                /* set up spriteram area */
                Generic.spriteram_size[0] = 0x80;
                Generic.spriteram = new _BytePtr(phozon_spriteram, 0x780);
                Generic.spriteram_2 = new _BytePtr(phozon_spriteram, 0x780 + 0x800);
                Generic.spriteram_3 = new _BytePtr(phozon_spriteram, 0x780 + 0x800 + 0x800);

                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }

            void phozon_draw_sprite(Mame.osd_bitmap dest, uint code, uint color, int flipx, int flipy, int sx, int sy)
            {
                Mame.drawgfx(dest, Mame.Machine.gfx[2], code, color, flipx != 0, flipy != 0, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }

            void phozon_draw_sprite8(Mame.osd_bitmap dest, uint code, uint color, int flipx, int flipy, int sx, int sy)
            {
                Mame.drawgfx(dest, Mame.Machine.gfx[3], code, color, flipx != 0, flipy != 0, sx, sy, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* for every character in the video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy, mx, my;

                        Generic.dirtybuffer[offs] = false;

                        /* Even if Phozon screen is 28x36, the memory layout is 32x32. We therefore
                        have to convert the memory coordinates into screen coordinates.
                        Note that 32*32 = 1024, while 28*36 = 1008: therefore 16 bytes of Video RAM
                        don't map to a screen position. We don't check that here, however: range
                        checking is performed by drawgfx(). */

                        mx = offs % 32;
                        my = offs / 32;

                        if (my <= 1)
                        {       /* bottom screen characters */
                            sx = my + 34;
                            sy = mx - 2;
                        }
                        else if (my >= 30)
                        {	/* top screen characters */
                            sx = my - 30;
                            sy = mx - 2;
                        }
                        else
                        {               /* middle screen characters */
                            sx = mx + 2;
                            sy = my - 2;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[(Generic.colorram[offs] & 0x80) != 0 ? 1 : 0],
                                 Generic.videoram[offs],
                                 (uint)(Generic.colorram[offs] & 0x3f),
                                false, false,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                /* Draw the sprites. */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 2)
                {
                    /* is it on? */
                    if ((Generic.spriteram_3[offs + 1] & 2) == 0)
                    {
                        uint sprite = Generic.spriteram[offs];
                        uint color = Generic.spriteram[offs + 1];
                        int x = (Generic.spriteram_2[offs + 1] - 69) + 0x100 * (Generic.spriteram_3[offs + 1] & 1);
                        int y = (Mame.Machine.drv.screen_height) - Generic.spriteram_2[offs] - 8;
                        int flipx = Generic.spriteram_3[offs] & 1;
                        int flipy = Generic.spriteram_3[offs] & 2;

                        switch (Generic.spriteram_3[offs] & 0x3c)
                        {
                            case 0x00:		/* 16x16 */
                                phozon_draw_sprite(bitmap, sprite, color, flipx, flipy, x, y);
                                break;

                            case 0x14:		/* 8x8 */
                                sprite = (uint)((sprite << 2) | ((Generic.spriteram_3[offs] & 0xc0) >> 6));
                                phozon_draw_sprite8(bitmap, sprite, color, flipx, flipy, x, y + 8);
                                break;

                            case 0x04:		/* 8x16 */
                                sprite = (uint)((sprite << 2) | ((Generic.spriteram_3[offs] & 0xc0) >> 6));
                                if (flipy == 0)
                                {
                                    phozon_draw_sprite8(bitmap, 2 + sprite, color, flipx, flipy, x, y + 8);
                                    phozon_draw_sprite8(bitmap, sprite, color, flipx, flipy, x, y);
                                }
                                else
                                {
                                    phozon_draw_sprite8(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                    phozon_draw_sprite8(bitmap, sprite, color, flipx, flipy, x, y + 8);
                                }
                                break;

                            case 0x24:		/* 8x32 */
                                sprite = (uint)((sprite << 2) | ((Generic.spriteram_3[offs] & 0xc0) >> 6));
                                if (flipy == 0)
                                {
                                    phozon_draw_sprite8(bitmap, 10 + sprite, color, flipx, flipy, x, y + 8);
                                    phozon_draw_sprite8(bitmap, 8 + sprite, color, flipx, flipy, x, y);
                                    phozon_draw_sprite8(bitmap, 2 + sprite, color, flipx, flipy, x, y - 8);
                                    phozon_draw_sprite8(bitmap, sprite, color, flipx, flipy, x, y - 16);
                                }
                                else
                                {
                                    phozon_draw_sprite8(bitmap, 10 + sprite, color, flipx, flipy, x, y - 16);
                                    phozon_draw_sprite8(bitmap, 8 + sprite, color, flipx, flipy, x, y - 8);
                                    phozon_draw_sprite8(bitmap, 2 + sprite, color, flipx, flipy, x, y);
                                    phozon_draw_sprite8(bitmap, sprite, color, flipx, flipy, x, y + 8);
                                }
                                break;

                            default:

                                phozon_draw_sprite(bitmap, (uint)Mame.rand(), color, flipx, flipy, x, y);
                                break;
                        }
                    }
                }


                /* redraw high priority chars */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if ((Generic.colorram[offs] & 0x40) != 0)
                    {
                        int sx, sy, mx, my;


                        /* Even if Phozon screen is 28x36, the memory layout is 32x32. We therefore
                        have to convert the memory coordinates into screen coordinates.
                        Note that 32*32 = 1024, while 28*36 = 1008: therefore 16 bytes of Video RAM
                        don't map to a screen position. We don't check that here, however: range
                        checking is performed by drawgfx(). */

                        mx = offs % 32;
                        my = offs / 32;

                        if (my <= 1)
                        {       /* bottom screen characters */
                            sx = my + 34;
                            sy = mx - 2;
                        }
                        else if (my >= 30)
                        {	/* top screen characters */
                            sx = my - 30;
                            sy = mx - 2;
                        }
                        else
                        {               /* middle screen characters */
                            sx = mx + 2;
                            sy = my - 2;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[(Generic.colorram[offs] & 0x80) != 0 ? 1 : 0],
                                 Generic.videoram[offs],
                                 (uint)(Generic.colorram[offs] & 0x3f),
                                false, false,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_phozon()
        {
            ROM_START("phozon");
            ROM_REGION(0x10000, Mame.REGION_CPU1);  /* 64k for code for the MAIN CPU  */
            ROM_LOAD("6e.rom", 0x8000, 0x2000, 0xa6686af1);
            ROM_LOAD("6h.rom", 0xa000, 0x2000, 0x72a65ba0);
            ROM_LOAD("6c.rom", 0xc000, 0x2000, 0xf1fda22e);
            ROM_LOAD("6d.rom", 0xe000, 0x2000, 0xf40e6df0);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for the SUB CPU */
            ROM_LOAD("9r.rom", 0xe000, 0x2000, 0x5d9f0a28);

            ROM_REGION(0x10000, Mame.REGION_CPU3);   /* 64k for the SOUND CPU */
            ROM_LOAD("3b.rom", 0xe000, 0x2000, 0x5a4b3a79);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("7j.rom", 0x0000, 0x1000, 0x27f9db5b);/* characters (set 1) */

            ROM_REGION(0x1000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("8j.rom", 0x0000, 0x1000, 0x15b12ef8);/* characters (set 2) */

            ROM_REGION(0x2000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("5t.rom", 0x0000, 0x2000, 0xd50f08f8); /* sprites */

            ROM_REGION(0x0520, Mame.REGION_PROMS);
            ROM_LOAD("red.prm", 0x0000, 0x0100, 0xa2880667); /* red palette ROM (4 bits) */
            ROM_LOAD("green.prm", 0x0100, 0x0100, 0xd6e08bef);/* green palette ROM (4 bits) */
            ROM_LOAD("blue.prm", 0x0200, 0x0100, 0xb2d69c72); /* blue palette ROM (4 bits) */
            ROM_LOAD("chr.prm", 0x0300, 0x0100, 0x429e8fee); /* characters */
            ROM_LOAD("sprite.prm", 0x0400, 0x0100, 0x9061db07); /* sprites */
            ROM_LOAD("palette.prm", 0x0500, 0x0020, 0x60e856ed); /* palette (unused?) */

            ROM_REGION(0x0100, Mame.REGION_SOUND1);/* sound PROMs */
            ROM_LOAD("sound.prm", 0x0000, 0x0100, 0xad43688f);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_phozon()
        {
            INPUT_PORTS_START("phozon");
            PORT_START("DSW0");
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x07, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x05, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_7C"));
            PORT_DIPNAME(0x18, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x10, "1");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x08, "4");
            PORT_DIPSETTING(0x18, "5");
            PORT_DIPNAME(0x60, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x60, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "0");
            PORT_DIPSETTING(0x01, "1");
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x04, "4");
            PORT_DIPSETTING(0x05, "5");
            PORT_DIPSETTING(0x06, "6");
            PORT_DIPSETTING(0x07, "7"); ;
            PORT_SERVICE(0x08, IP_ACTIVE_HIGH);
            /* Todo: those are different for 4 and 5 lives */
            PORT_DIPNAME(0xc0, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0xc0, "20k 80k");
            PORT_DIPSETTING(0x40, "30k 60k");
            PORT_DIPSETTING(0x80, "30k 120k and every 120k");
            PORT_DIPSETTING(0x00, "30k 100k");

            PORT_START("IN0");
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1, 1);
            PORT_BIT_IMPULSE(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2, 1);
            PORT_BIT_IMPULSE(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2, 1);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x80, DEF_STR("Cocktail"));

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);

            PORT_START("IN2");
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, 1);
            PORT_BITX(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            PORT_BIT_IMPULSE(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2, 1);
            PORT_BITX(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2, null, (ushort)Mame.InputCodes.IP_KEY_PREVIOUS, (ushort)Mame.InputCodes.IP_JOY_PREVIOUS);
            return INPUT_PORTS_END;
        }
        public driver_phozon()
        {
            drv = new machine_driver_phozon();
            year = "1983";
            name = "phozon";
            description = "Phozon";
            manufacturer = "Namco";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_phozon();
            rom = rom_phozon();
            drv.HasNVRAMhandler = false;
        }
    }
}
