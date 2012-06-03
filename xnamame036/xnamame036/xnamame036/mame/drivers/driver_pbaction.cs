using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_pbaction : Mame.GameDriver
    {
        static _BytePtr pbaction_videoram2 = new _BytePtr(1);
        static _BytePtr pbaction_colorram2 = new _BytePtr(1);
        static bool[] dirtybuffer2;
        static Mame.osd_bitmap tmpbitmap2;
        static int scroll;
        static int flipscreen;

        static void pbaction_sh_command_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.cpu_cause_interrupt(1, 0x00);
        }

static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x9fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(0xc000, 0xdfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(0xe000, 0xe07f, Mame.MRA_RAM),
	new Mame.MemoryReadAddress(0xe400, 0xe5ff, Mame.MRA_RAM),
	new Mame.MemoryReadAddress(0xe600, 0xe600, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress(0xe601, 0xe601, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress(0xe602, 0xe602, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress(0xe604, 0xe604, Mame.input_port_3_r ),	/* DSW1 */
	new Mame.MemoryReadAddress(0xe605, 0xe605, Mame.input_port_4_r ),	/* DSW2 */
	new Mame.MemoryReadAddress(0xe606, 0xe606, Mame.MRA_NOP ),	/* ??? */
	new Mame.MemoryReadAddress(-1 )  /* end of table */
};

static Mame.MemoryWriteAddress[] writemem =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x9fff, Mame.MWA_ROM ),
	new  Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new  Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram,Generic.videoram_size ),
	new  Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new  Mame.MemoryWriteAddress( 0xd800, 0xdbff, pbaction_videoram2_w, pbaction_videoram2 ),
	new  Mame.MemoryWriteAddress( 0xdc00, 0xdfff, pbaction_colorram2_w, pbaction_colorram2 ),
	new  Mame.MemoryWriteAddress( 0xe000, 0xe07f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new  Mame.MemoryWriteAddress( 0xe400, 0xe5ff, Mame.paletteram_xxxxBBBBGGGGRRRR_w, Mame.paletteram ),
	new  Mame.MemoryWriteAddress( 0xe600, 0xe600, Mame.interrupt_enable_w ),
	new  Mame.MemoryWriteAddress( 0xe604, 0xe604, pbaction_flipscreen_w ),
	new  Mame.MemoryWriteAddress( 0xe606, 0xe606, pbaction_scroll_w ),
	new  Mame.MemoryWriteAddress( 0xe800, 0xe800, pbaction_sh_command_w ),
	new  Mame.MemoryWriteAddress( -1 )  /* end of table */
};

static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff,Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff,Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8000,Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

static Mame.MemoryWriteAddress[] sound_writemem =
{
	new  Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new  Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new  Mame.MemoryWriteAddress( 0xffff, 0xffff, Mame.MWA_NOP ),	/* watchdog? */
	new  Mame.MemoryWriteAddress( -1 )	/* end of table */
};


static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x10, 0x10, ay8910.AY8910_control_port_0_w ),
	new Mame.IOWritePort( 0x11, 0x11, ay8910.AY8910_write_port_0_w ),
	new Mame.IOWritePort( 0x20, 0x20, ay8910.AY8910_control_port_1_w ),
	new Mame.IOWritePort( 0x21, 0x21, ay8910.AY8910_write_port_1_w ),
	new Mame.IOWritePort( 0x30, 0x30, ay8910.AY8910_control_port_2_w ),
	new Mame.IOWritePort( 0x31, 0x31, ay8910.AY8910_write_port_2_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};
static void pbaction_videoram2_w(int offset, int data)
{
    if (pbaction_videoram2[offset] != data)
    {
        dirtybuffer2[offset] = true;

        pbaction_videoram2[offset] = (byte)data;
    }
}
static void pbaction_colorram2_w(int offset, int data)
{
    if (pbaction_colorram2[offset] != data)
    {
        dirtybuffer2[offset] = true;

        pbaction_colorram2[offset] = (byte)data;
    }
}
static void pbaction_scroll_w(int offset, int data)
{
    scroll = -(data - 3);
}
static void pbaction_flipscreen_w(int offset, int data)
{
    if (flipscreen != (data & 1))
    {
        flipscreen = data & 1;
        Generic.SetDirtyBuffer(true);
        for (int i = 0; i < Generic.videoram[0]; i++) dirtybuffer2[i] = true;
    }
}
static Mame.GfxLayout charlayout1 =
new Mame.GfxLayout(
	8,8,	/* 8*8 characters */
	1024,	/* 1024 characters */
	3,	/* 3 bits per pixel */
	new uint[]{ 0, 1024*8*8, 2*1024*8*8 },	/* the bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	8*8	/* every char takes 8 consecutive bytes */
);
static Mame.GfxLayout charlayout2 =
new Mame.GfxLayout(
	8,8,	/* 8*8 characters */
	2048,	/* 2048 characters */
	4,	/* 4 bits per pixel */
	new uint[]{ 0, 2048*8*8, 2*2048*8*8, 3*2048*8*8 },	/* the bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7 },	/* pretty straightforward layout */
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
	8*8	/* every char takes 8 consecutive bytes */
);
static Mame.GfxLayout spritelayout1 =
new Mame.GfxLayout( 
	16,16,	/* 16*16 sprites */
	128,	/* 128 sprites */
	3,	/* 3 bits per pixel */
	new uint[]{ 0, 256*16*16, 2*256*16*16 },	/* the bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,	/* pretty straightforward layout */
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
	32*8	/* every sprite takes 32 consecutive bytes */
);
static Mame.GfxLayout spritelayout2 =
new Mame.GfxLayout(
	32,32,	/* 32*32 sprites */
	32,	/* 32 sprites */
	3,	/* 3 bits per pixel */
	new uint[]{ 0*64*32*32, 1*64*32*32, 2*64*32*32 },	/* the bitplanes are separated */
	new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,	/* pretty straightforward layout */
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7,
			32*8+0, 32*8+1, 32*8+2, 32*8+3, 32*8+4, 32*8+5, 32*8+6, 32*8+7,
			40*8+0, 40*8+1, 40*8+2, 40*8+3, 40*8+4, 40*8+5, 40*8+6, 40*8+7 },
	new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8,
			64*8, 65*8, 66*8, 67*8, 68*8, 69*8, 70*8, 71*8,
			80*8, 81*8, 82*8, 83*8, 84*8, 85*8, 86*8, 87*8 },
	128*8	/* every sprite takes 128 consecutive bytes */
);
        
static Mame.GfxDecodeInfo[] gfxdecodeinfo=
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x00000, charlayout1,    0, 16 ),	/*   0-127 characters */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0x00000,charlayout2,  128,  8 ),	/* 128-255 background */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x00000, spritelayout1,  0, 16 ),	/*   0-127 normal sprites */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x01000, spritelayout2,  0, 16 ),	/*   0-127 large sprites */
};


static AY8910interface ay8910_interface =
new AY8910interface(
	3,	/* 3 chips */
	1500000,	/* 1.5 MHz?????? */
	new int  []{ 25, 25, 25 },
	new AY8910portRead[]{ null,null,null},
    new AY8910portRead[] { null, null, null },
    new AY8910portWrite[] { null, null, null },
    new AY8910portWrite[] { null, null, null },
    null
);
        static int pbaction_interrupt()
{
	return  0x02;	/* the CPU is in Interrupt Mode 2 */
}
        class machine_driver_pbaction : Mame.MachineDriver
        {
            public machine_driver_pbaction()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem,writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3072000, sound_readmem, sound_writemem, null, sound_writeport, driver_pbaction.pbaction_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_pbaction.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
            }
            public override void init_machine()
            {
              //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
              //none
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0)
                    return 1;

                dirtybuffer2 = new bool[Generic.videoram_size[0]];
                for (int i = 0; i < Generic.videoram_size[0]; i++) dirtybuffer2[i] = true;

                tmpbitmap2 = Mame.osd_create_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

                /* leave everything at the default, but map all foreground 0 pens as transparent */
                for (int i = 0; i < 16; i++) Mame.palette_used_colors[8 * i] = Mame.PALETTE_COLOR_TRANSPARENT;

                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(tmpbitmap2);
                dirtybuffer2 = null;
                Generic.generic_vh_stop();
            }
            public override void vh_eof_callback()
            {
                //none
            }

            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* recalc the palette if necessary */
                if (Mame.palette_recalc()!=null)
                {
                    Generic.SetDirtyBuffer(true);
                    for (int i = 0; i < Generic.videoram_size[0]; i++) dirtybuffer2[i] = true;
                }


                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = true;

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
                                (uint)(Generic.videoram[offs] + 0x10 * (Generic.colorram[offs] & 0x30)),
                                (uint)(Generic.colorram[offs] & 0x0f),
                                flipx, flipy,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }

                    if (dirtybuffer2[offs])
                    {
                        dirtybuffer2[offs] = true;

                        int sx = offs % 32;
                        int sy = offs / 32;
                        bool flipy = (pbaction_colorram2[offs] & 0x80)!=0;
                        if (flipscreen!=0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[1],
                                (uint)(pbaction_videoram2[offs] + 0x10 * (pbaction_colorram2[offs] & 0x70)),
                                (uint)(pbaction_colorram2[offs] & 0x0f),
                                flipscreen!=0, flipy,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the background */
                Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, new int[]{scroll}, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    /* if next sprite is double size, skip this one */
                    if (offs > 0 && (Generic.spriteram[offs - 4] & 0x80)!=0) continue;

                    int sy;
                    int sx = Generic.spriteram[offs + 3];
                    if ((Generic.spriteram[offs] & 0x80)!=0)
                        sy = 225 - Generic.spriteram[offs + 2];
                    else
                        sy = 241 - Generic.spriteram[offs + 2];
                    bool flipx = (Generic.spriteram[offs + 1] & 0x40)!=0;
                    bool flipy = (Generic.spriteram[offs + 1] & 0x80)!=0;
                    if (flipscreen!=0)
                    {
                        if ((Generic.spriteram[offs] & 0x80)!=0)
                        {
                            sx = 224 - sx;
                            sy = 225 - sy;
                        }
                        else
                        {
                            sx = 240 - sx;
                            sy = 241 - sy;
                        }
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[(Generic.spriteram[offs] & 0x80) !=0? 3 : 2],	/* normal or double size */
                            Generic.spriteram[offs],
                            (uint)(Generic.spriteram[offs + 1] & 0x0f),
                            flipx, flipy,
                            sx + scroll, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }


                /* copy the foreground */
                Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[]{scroll}, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, Mame.palette_transparent_pen);

            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_pbaction()
        {
            ROM_START("pbaction");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("b-p7.bin", 0x0000, 0x4000, 0x8d6dcaae);
            ROM_LOAD("b-n7.bin", 0x4000, 0x4000, 0xd54d5402);
            ROM_LOAD("b-l7.bin", 0x8000, 0x2000, 0xe7412d68);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for sound board */
            ROM_LOAD("a-e3.bin", 0x0000, 0x2000, 0x0e53a91f);

            ROM_REGION(0x06000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a-s6.bin", 0x00000, 0x2000, 0x9a74a8e1);
            ROM_LOAD("a-s7.bin", 0x02000, 0x2000, 0x5ca6ad3c);
            ROM_LOAD("a-s8.bin", 0x04000, 0x2000, 0x9f00b757);

            ROM_REGION(0x10000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a-j5.bin", 0x00000, 0x4000, 0x21efe866);
            ROM_LOAD("a-j6.bin", 0x04000, 0x4000, 0x7f984c80);
            ROM_LOAD("a-j7.bin", 0x08000, 0x4000, 0xdf69e51b);
            ROM_LOAD("a-j8.bin", 0x0c000, 0x4000, 0x0094cb8b);

            ROM_REGION(0x06000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("b-c7.bin", 0x00000, 0x2000, 0xd1795ef5);
            ROM_LOAD("b-d7.bin", 0x02000, 0x2000, 0xf28df203);
            ROM_LOAD("b-f7.bin", 0x04000, 0x2000, 0xaf6e9817);
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_pbaction()
        {

            INPUT_PORTS_START("pbaction");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON4);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON3 | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON4 | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x30, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x30, "2");
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x20, "5");
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x40, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("SW1");
            PORT_DIPNAME(0x07, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x01, "70K 200K 1000K");
            PORT_DIPSETTING(0x00, "70K 200K");
            PORT_DIPSETTING(0x04, "100K 300K 1000K");
            PORT_DIPSETTING(0x03, "100K 300K");
            PORT_DIPSETTING(0x02, "100K");
            PORT_DIPSETTING(0x06, "200K 1000K");
            PORT_DIPSETTING(0x05, "200K");
            PORT_DIPSETTING(0x07, "None");
            PORT_DIPNAME(0x08, 0x00, "Extra");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Hard");
            PORT_DIPNAME(0x30, 0x00, "Difficulty (Flippers)");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x10, "Medium");
            PORT_DIPSETTING(0x20, "Hard");
            PORT_DIPSETTING(0x30, "Hardest");
            PORT_DIPNAME(0xc0, 0x00, "Difficulty (Outlanes)");
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x40, "Medium");
            PORT_DIPSETTING(0x80, "Hard");
            PORT_DIPSETTING(0xc0, "Hardest");
            return INPUT_PORTS_END;
        }
        public driver_pbaction()
        {
            drv = new machine_driver_pbaction();
            year = "1985";
            name = "pbaction";
            description = "Pinball Action (set 1)";
            manufacturer = "Tehkan";
            flags = Mame.ROT90;
            input_ports = input_ports_pbaction();
            rom = rom_pbaction();
            drv.HasNVRAMhandler = false;
        }
    }
}
