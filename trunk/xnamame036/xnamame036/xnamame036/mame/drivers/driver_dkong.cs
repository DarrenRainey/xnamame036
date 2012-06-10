using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public partial class driver_dkong : Mame.GameDriver
    {
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x5fff, Mame.MRA_ROM ),	/* DK: 0000-3fff */
	new Mame.MemoryReadAddress( 0x6000, 0x6fff, Mame.MRA_RAM ),	/* including sprites RAM */
	new Mame.MemoryReadAddress( 0x7400, 0x77ff, Mame.MRA_RAM ),	/* video RAM */
	new Mame.MemoryReadAddress( 0x7c00, 0x7c00, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x7c80, 0x7c80, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x7d00, 0x7d00, dkong_in2_r ),	/* IN2/DSW2 */
	new Mame.MemoryReadAddress( 0x7d80, 0x7d80, Mame.input_port_3_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, Mame.MRA_ROM ),	/* DK3 and bootleg DKjr only */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] dkong3_readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x5fff, Mame.MRA_ROM ),	/* DK: 0000-3fff */
	new Mame.MemoryReadAddress(0x6000, 0x6fff, Mame.MRA_RAM ),	/* including sprites RAM */
	new Mame.MemoryReadAddress(0x7400, 0x77ff, Mame.MRA_RAM ),	/* video RAM */
	new Mame.MemoryReadAddress(0x7c00, 0x7c00, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress(0x7c80, 0x7c80, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress(0x7d00, 0x7d00, Mame.input_port_2_r ),	/* IN2/DSW2 */
	new Mame.MemoryReadAddress(0x7d80, 0x7d80, Mame.input_port_3_r ),	/* DSW1 */
	new Mame.MemoryReadAddress(0x8000, 0x9fff, Mame.MRA_ROM ),	/* DK3 and bootleg DKjr only */
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        //static Mame.MemoryWriteAddress[] radarscp_writemem =
        //{
        //    new Mame.MemoryWriteAddress( 0x0000, 0x5fff, MWA_ROM ),
        //    new Mame.MemoryWriteAddress( 0x6000, 0x68ff, MWA_RAM ),
        //    new Mame.MemoryWriteAddress( 0x6900, 0x6a7f, MWA_RAM, &spriteram, &spriteram_size ),
        //    new Mame.MemoryWriteAddress( 0x6a80, 0x6fff, MWA_RAM ),
        //    new Mame.MemoryWriteAddress( 0x7000, 0x73ff, MWA_RAM ),    /* ???? */
        //    new Mame.MemoryWriteAddress( 0x7400, 0x77ff, videoram_w, &videoram, &videoram_size ),
        //    new Mame.MemoryWriteAddress( 0x7800, 0x7803, MWA_RAM ),	/* ???? */
        //    new Mame.MemoryWriteAddress( 0x7808, 0x7808, MWA_RAM ),	/* ???? */
        //    new Mame.MemoryWriteAddress( 0x7c00, 0x7c00, dkong_sh_tuneselect ),
        //    new Mame.MemoryWriteAddress( 0x7c80, 0x7c80, radarscp_grid_color_w ),
        //    new Mame.MemoryWriteAddress( 0x7d00, 0x7d02, dkong_sh1_w ),	/* walk/jump/boom sample trigger */
        //    new Mame.MemoryWriteAddress( 0x7d03, 0x7d03, dkong_sh_sound3 ),
        //    new Mame.MemoryWriteAddress( 0x7d04, 0x7d04, dkong_sh_sound4 ),
        //    new Mame.MemoryWriteAddress( 0x7d05, 0x7d05, dkong_sh_sound5 ),
        //    new Mame.MemoryWriteAddress( 0x7d80, 0x7d80, dkong_sh_w ),
        //    new Mame.MemoryWriteAddress( 0x7d81, 0x7d81, radarscp_grid_enable_w ),
        //    new Mame.MemoryWriteAddress( 0x7d82, 0x7d82, dkong_flipscreen_w ),
        //    new Mame.MemoryWriteAddress( 0x7d83, 0x7d83, MWA_RAM ),
        //    new Mame.MemoryWriteAddress( 0x7d84, 0x7d84, interrupt_enable_w ),
        //    new Mame.MemoryWriteAddress( 0x7d85, 0x7d85, MWA_RAM ),
        //    new Mame.MemoryWriteAddress( 0x7d86, 0x7d87, dkong_palettebank_w ),
        //    new Mame.MemoryWriteAddress( -1 )	/* end of table */
        //};

        static Mame.MemoryWriteAddress[] dkong_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6000, 0x68ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x6900, 0x6a7f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x6a80, 0x6fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x7000, 0x73ff, Mame.MWA_RAM ),    /* ???? */
	new Mame.MemoryWriteAddress( 0x7400, 0x77ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x7800, 0x7803, Mame.MWA_RAM ),	/* ???? */
	new Mame.MemoryWriteAddress( 0x7808, 0x7808, Mame.MWA_RAM ),	/* ???? */
	new Mame.MemoryWriteAddress( 0x7c00, 0x7c00, dkong_sh_tuneselect ),
//	new Mame.MemoryWriteAddress( 0x7c80, 0x7c80,  },
	new Mame.MemoryWriteAddress( 0x7d00, 0x7d02, dkong_sh1_w ),	/* walk/jump/boom sample trigger */
	new Mame.MemoryWriteAddress( 0x7d03, 0x7d03, dkong_sh_sound3 ),
	new Mame.MemoryWriteAddress( 0x7d04, 0x7d04, dkong_sh_sound4 ),
	new Mame.MemoryWriteAddress( 0x7d05, 0x7d05, dkong_sh_sound5 ),
	new Mame.MemoryWriteAddress( 0x7d80, 0x7d80, dkong_sh_w ),
	new Mame.MemoryWriteAddress( 0x7d81, 0x7d81, Mame.MWA_RAM ),	/* ???? */
	new Mame.MemoryWriteAddress( 0x7d82, 0x7d82, dkong_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x7d83, 0x7d83, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x7d84, 0x7d84, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0x7d85, 0x7d85, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x7d86, 0x7d87, dkong_palettebank_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.GfxLayout dkong_charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 256 * 8 * 8, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        static Mame.GfxLayout dkongjr_charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 512 * 8 * 8, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        static Mame.GfxLayout dkong_spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 128 * 16 * 16, 0 },	/* the two bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,	/* the two halves of the sprite are separated */
			64*16*16+0, 64*16*16+1, 64*16*16+2, 64*16*16+3, 64*16*16+4, 64*16*16+5, 64*16*16+6, 64*16*16+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );
        static Mame.GfxLayout dkong3_spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            256,	/* 256 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 256 * 16 * 16, 0 },	/* the two bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,	/* the two halves of the sprite are separated */
			128*16*16+0, 128*16*16+1, 128*16*16+2, 128*16*16+3, 128*16*16+4, 128*16*16+5, 128*16*16+6, 128*16*16+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );
        static Mame.GfxDecodeInfo[] dkong_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, dkong_charlayout,   0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, dkong_spritelayout, 0, 64 ),
};
        static Mame.GfxDecodeInfo[] dkongjr_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x0000, dkongjr_charlayout, 0, 64 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x0000, dkong_spritelayout, 0, 64 ),
};
        static Mame.GfxDecodeInfo[] dkong3_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, dkongjr_charlayout,   0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000, dkong3_spritelayout,  0, 64 ),
};


        static Mame.DACinterface dkong_dac_interface =
new Mame.DACinterface(
    1,
  new int[] { 55 }
);

        static string[] dkong_sample_names =
{
	"*dkong",
	"effect00.wav",
	"effect01.wav",
	"effect02.wav",
	null	/* end of array */
};

        static string[] dkongjr_sample_names =
{
	"*dkongjr",
	"jump.wav",
	"land.wav",
	"roar.wav",
	"climb.wav",   /* HC */
	"death.wav",  /* HC */
	"drop.wav",  /* HC */
	"walk.wav", /* HC */
	"snapjaw.wav",  /* HC */
	null	/* end of array */
};

        static Mame.Samplesinterface dkong_samples_interface =
        new Mame.Samplesinterface(
            8,	/* 8 channels */
            25,	/* volume */
            dkong_sample_names
        );

        static Mame.Samplesinterface dkongjr_samples_interface =
        new Mame.Samplesinterface(
            8,	/* 8 channels */
            25,	/* volume */
            dkongjr_sample_names
        );
        static Mame.MemoryReadAddress[] readmem_sound =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress (-1 )	/* end of table */
};
        static Mame.MemoryWriteAddress[] writemem_sound =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_ROM),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.IOReadPort[] readport_sound =
{
	new Mame.IOReadPort( 0x00,     0xff,     dkong_sh_gettune ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_p1, Mame.cpu_i8039.I8039_p1, dkong_sh_getp1 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_p2, Mame.cpu_i8039.I8039_p2, dkong_sh_getp2 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_t0, Mame.cpu_i8039.I8039_t0, dkong_sh_gett0 ),
	new Mame.IOReadPort( Mame.cpu_i8039.I8039_t1, Mame.cpu_i8039.I8039_t1, dkong_sh_gett1 ),
    new Mame.IOReadPort(-1),
};
        static Mame.IOWritePort[] writeport_sound =
{
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p1,Mame.cpu_i8039.I8039_p1, dkong_sh_putp1 ),
	new Mame.IOWritePort( Mame.cpu_i8039.I8039_p2,Mame.cpu_i8039.I8039_p2, dkong_sh_putp2 ),
    new Mame.IOWritePort(-1),
};
        static double envelope, tt;
        static bool decay;

        const double TSTEP = 0.001;

        static void dkong_sh_putp1(int offset, int data)
        {
            envelope = Math.Exp(-tt);
            DAC.DAC_data_w(0, (int)(data * envelope));
            if (decay) tt += TSTEP;
            else tt = 0;
        }
        static int ACTIVELOW_PORT_BIT(int P, int A, int D) { return ((P & (~(1 << A))) | ((D ^ 1) << A)); }
        static void dkong_sh_sound3(int offset, int data) { p[2] = ACTIVELOW_PORT_BIT(p[2], 5, data); }
        static void dkong_sh_sound4(int offset, int data) { t[1] = ~data & 1; }
        static void dkong_sh_sound5(int offset, int data) { t[0] = ~data & 1; }
        static void dkong_sh_tuneselect(int offset, int data) { Mame.soundlatch_w(offset, data ^ 0x0f); }

        static void dkongjr_sh_test6(int offset, int data) { p[2] = ACTIVELOW_PORT_BIT(p[2], 6, data); }
        static void dkongjr_sh_test5(int offset, int data) { p[2] = ACTIVELOW_PORT_BIT(p[2], 5, data); }
        static void dkongjr_sh_test4(int offset, int data) { p[2] = ACTIVELOW_PORT_BIT(p[2], 4, data); }
        static void dkongjr_sh_tuneselect(int offset, int data) { Mame.soundlatch_w(offset, data); }

        static void dkong_sh_putp2(int offset, int data)
        {
            /*   If P2.Bit7 . is apparently an external signal decay or other output control
             *   If P2.Bit6 . activates the external compressed sample ROM
             *   If P2.Bit4 . status code to main cpu
             *   P2.Bit2-0  . select the 256 byte bank for external ROM
             */

            decay = (data & 0x80) == 0;
            page = (data & 0x47);
            mcustatus = ((~data & 0x10) >> 4);
        }

        static int page = 0, mcustatus;
        static int[] p = { 255, 255, 255, 255, 255, 255, 255, 255 };
        static int[] t = { 1, 1 };
        static int dkong_in2_r(int offset)
        {
            return Mame.input_port_2_r(offset) | (mcustatus << 6);
        }
        static int dkong_sh_getp1(int offset) { return p[1]; }
        static int dkong_sh_getp2(int offset) { return p[2]; }
        static int dkong_sh_gett0(int offset) { return t[0]; }
        static int dkong_sh_gett1(int offset) { return t[1]; }
        static int dkong_sh_gettune(int offset)
        {
            _BytePtr SND = Mame.memory_region(Mame.REGION_CPU2);
            if ((page & 0x40) != 0)
            {
                switch (offset)
                {
                    case 0x20: return Mame.soundlatch_r(0);
                }
            }
            return (SND[2048 + (page & 7) * 256 + offset]);
        }

        class machine_driver_dkong : Mame.MachineDriver
        {
            public machine_driver_dkong()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3072000, readmem, dkong_writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_8035 | Mame.CPU_AUDIO_CPU, 6000000 / 15, readmem_sound, writemem_sound, readport_sound, writeport_sound, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_dkong.dkong_gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dkong_dac_interface));
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
                //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
                //	#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])

                int pi = 0, cpi = 0;
                for (int i = 0; i < 256; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi + 256] >> 1) & 1;
                    int bit1 = (color_prom[cpi + 256] >> 2) & 1;
                    int bit2 = (color_prom[cpi + 256] >> 3) & 1;
                    palette[pi++] = (byte)(255 - (0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2));
                    /* green component */
                    bit0 = (color_prom[cpi + 0] >> 2) & 1;
                    bit1 = (color_prom[cpi + 0] >> 3) & 1;
                    bit2 = (color_prom[cpi + 256] >> 0) & 1;
                    palette[pi++] = (byte)(255 - (0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2));
                    /* blue component */
                    bit0 = (color_prom[cpi + 0] >> 0) & 1;
                    bit1 = (color_prom[cpi + 0] >> 1) & 1;
                    palette[pi++] = (byte)(255 - (0x55 * bit0 + 0xaa * bit1));

                    cpi++;
                }

                cpi += 256;
                /* color_prom now points to the beginning of the character color codes */
                color_codes = new _BytePtr(color_prom, cpi);	/* we'll need it later */

            }
            public override int vh_start()
            {
                gfx_bank = 0;
                palette_bank = 0;

                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_bitmapped_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                draw_tiles(bitmap);
                draw_sprites(bitmap);
            }
            static void draw_tiles(Mame.osd_bitmap bitmap)
            {
                int offs;

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy;
                        int charcode, color;

                        Generic.dirtybuffer[offs] = false;

                        sx = offs % 32;
                        sy = offs / 32;

                        charcode = Generic.videoram[offs] + 256 * gfx_bank;
                        /* retrieve the character color from the PROM */
                        color = (color_codes[offs % 32 + 32 * (offs / 32 / 4)] & 0x0f) + 0x10 * palette_bank;

                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)charcode, (uint)color,
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
            }

            static void draw_sprites(Mame.osd_bitmap bitmap)
            {
                int offs;

                /* Draw the sprites. */
                for (offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
                {
                    if (Generic.spriteram[offs] != 0)
                    {
                        /* spriteram[offs + 2] & 0x40 is used by Donkey Kong 3 only */
                        /* spriteram[offs + 2] & 0x30 don't seem to be used (they are */
                        /* probably not part of the color code, since Mario Bros, which */
                        /* has similar hardware, uses a memory mapped port to change */
                        /* palette bank, so it's limited to 16 color codes) */

                        int x, y;

                        x = Generic.spriteram[offs + 3] - 8;
                        y = 240 - Generic.spriteram[offs] + 7;

                        if (flipscreen != 0)
                        {
                            x = 240 - x;
                            y = 240 - y;

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)((Generic.spriteram[offs + 1] & 0x7f) + 2 * (Generic.spriteram[offs + 2] & 0x40)),
                                    (uint)((Generic.spriteram[offs + 2] & 0x0f) + 16 * palette_bank),
                                    (Generic.spriteram[offs + 2] & 0x80) == 0, (Generic.spriteram[offs + 1] & 0x80) == 0,
                                    x, y,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        else
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)((Generic.spriteram[offs + 1] & 0x7f) + 2 * (Generic.spriteram[offs + 2] & 0x40)),
                                    (uint)((Generic.spriteram[offs + 2] & 0x0f) + 16 * palette_bank),
                                    (Generic.spriteram[offs + 2] & 0x80) != 0, (Generic.spriteram[offs + 1] & 0x80) != 0,
                                    x, y,
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
        Mame.InputPortTiny[] input_ports_dkong()
        {

            INPUT_PORTS_START("dkong");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, DEF_STR("Service Mode"), (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            //	PORT_DIPNAME( 0x01, 0x00, DEF_STR( Service_Mode ) )
            //	PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            //	PORT_DIPSETTING(    0x01, DEF_STR( On ) )
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* status from sound cpu */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPSETTING(0x03, "6");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "7000");
            PORT_DIPSETTING(0x04, "10000");
            PORT_DIPSETTING(0x08, "15000");
            PORT_DIPSETTING(0x0c, "20000");
            PORT_DIPNAME(0x70, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x70, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x50, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_4C"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_dkong()
        {
            ROM_START("dkong");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("dk.5e", 0x0000, 0x1000, 0xba70b88b);
            ROM_LOAD("dk.5c", 0x1000, 0x1000, 0x5ec461ec);
            ROM_LOAD("dk.5b", 0x2000, 0x1000, 0x1c97d324);
            ROM_LOAD("dk.5a", 0x3000, 0x1000, 0xb9005ac0);
            /* space for diagnostic ROM */

            ROM_REGION(0x1000, Mame.REGION_CPU2);	/* sound */
            ROM_LOAD("dk.3h", 0x0000, 0x0800, 0x45a4ed06);
            ROM_LOAD("dk.3f", 0x0800, 0x0800, 0x4743fe92);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("dk.3n", 0x0000, 0x0800, 0x12c8c95d);
            ROM_LOAD("dk.3p", 0x0800, 0x0800, 0x15e9c5e9);

            ROM_REGION(0x2000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("dk.7c", 0x0000, 0x0800, 0x59f8054d);
            ROM_LOAD("dk.7d", 0x0800, 0x0800, 0x672e4714);
            ROM_LOAD("dk.7e", 0x1000, 0x0800, 0xfeaa59ee);
            ROM_LOAD("dk.7f", 0x1800, 0x0800, 0x20f2ef7e);

            ROM_REGION(0x0300, Mame.REGION_PROMS);
            ROM_LOAD("dkong.2k", 0x0000, 0x0100, 0x1e82d375); /* palette low 4 bits (inverted) */
            ROM_LOAD("dkong.2j", 0x0100, 0x0100, 0x2ab01dc8); /* palette high 4 bits (inverted) */
            ROM_LOAD("dkong.5f", 0x0200, 0x0100, 0x44988665); /* character color codes on a per-column basis */
            return ROM_END;

        }
        public override void driver_init()
        {
            //none
        }
        public driver_dkong()
        {
            drv = new machine_driver_dkong();
            year = "1981";
            name = "dkong";
            description = "Donkey Kong (US)";
            manufacturer = "Nintendo of America";
            flags = Mame.ROT90;
            input_ports = input_ports_dkong();
            rom = rom_dkong();
            drv.HasNVRAMhandler = false;
        }
    }
}
