using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_ladybug : Mame.GameDriver
    {


        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x5fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8fff, Mame.MRA_NOP ),
	new Mame.MemoryReadAddress( 0x9000, 0x9000, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x9001, 0x9001, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x9002, 0x9002, Mame.input_port_3_r ),	/* DSW0 */
	new Mame.MemoryReadAddress( 0x9003, 0x9003, Mame.input_port_4_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0xd000, 0xd7ff, Mame.MRA_RAM ),	/* video and color RAM */
	new Mame.MemoryReadAddress( 0xe000, 0xe000, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x6000, 0x6fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x7000, 0x73ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, ladybug_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xbfff, SN76496.SN76496_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, SN76496.SN76496_1_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( -1)	/* end of table */
};


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            512,	/* 512 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 0, 512 * 8 * 8 },	/* the two bitplanes are separated */
            new uint[] { 7, 6, 5, 4, 3, 2, 1, 0 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            128,	/* 128 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 1, 0 },	/* the two bitplanes are packed in two consecutive bits */
            new uint[]{ 0, 2, 4, 6, 8, 10, 12, 14,
			8*16+0, 8*16+2, 8*16+4, 8*16+6, 8*16+8, 8*16+10, 8*16+12, 8*16+14 },
            new uint[]{ 23*16, 22*16, 21*16, 20*16, 19*16, 18*16, 17*16, 16*16,
			7*16, 6*16, 5*16, 4*16, 3*16, 2*16, 1*16, 0*16 },
            64 * 8	/* every sprite takes 64 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout2 =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 sprites */
            512,	/* 512 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 1, 0 },	/* the two bitplanes are packed in two consecutive bits */
            new uint[] { 0, 2, 4, 6, 8, 10, 12, 14 },
            new uint[] { 7 * 16, 6 * 16, 5 * 16, 4 * 16, 3 * 16, 2 * 16, 1 * 16, 0 * 16 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,      0,  8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout,  4*8, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout2, 4*8, 16 ),
};


        static Mame.SN76496interface sn76496_interface =
        new Mame.SN76496interface(
            2,	/* 2 chips */
            new int[] { 4000000, 4000000 },	/* 4 MHz */
            new int[] { 100, 100 }
        );

        static int flipscreen;


        static int ladybug_interrupt()
        {
            if ((Mame.readinputport(5) & 1) != 0)/* Left Coin */
                return Mame.nmi_interrupt();
            else if ((Mame.readinputport(5) & 2) != 0)	/* Right Coin */
                return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void ladybug_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

        class machine_driver_ladybug : Mame.MachineDriver
        {
            public machine_driver_ladybug()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, null, null, ladybug_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(1 * 8, 31 * 8 - 1, 4 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_ladybug.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 4 * 24;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SN76496, sn76496_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                for (int i = 0; i < 32; i++)
                {
                    int bit1 = (~color_prom[i] >> 0) & 0x01;
                    int bit2 = (~color_prom[i] >> 5) & 0x01;
                    palette[3 * i] = (byte)(0x47 * bit1 + 0x97 * bit2);
                    bit1 = (~color_prom[i] >> 2) & 0x01;
                    bit2 = (~color_prom[i] >> 6) & 0x01;
                    palette[3 * i + 1] = (byte)(0x47 * bit1 + 0x97 * bit2);
                    bit1 = (~color_prom[i] >> 4) & 0x01;
                    bit2 = (~color_prom[i] >> 7) & 0x01;
                    palette[3 * i + 2] = (byte)(0x47 * bit1 + 0x97 * bit2);
                }

                /* characters */
                for (int i = 0; i < 8; i++)
                {
                    colortable[4 * i]= 0;
                    colortable[4 * i + 1] =(ushort)(i + 0x08);
                    colortable[4 * i + 2] =(ushort)(i + 0x10);
                    colortable[4 * i + 3] =(ushort)(i + 0x18);
                }

                /* sprites */
                for (int i = 0; i < 4 * 8; i++)
                {
                    /* low 4 bits are for sprite n */
                    int bit0 = (color_prom[i + 32] >> 3) & 0x01;
                    int bit1 = (color_prom[i + 32] >> 2) & 0x01;
                    int bit2 = (color_prom[i + 32] >> 1) & 0x01;
                    int bit3 = (color_prom[i + 32] >> 0) & 0x01;
                    colortable[i + 4 * 8]= (ushort)(1 * bit0 + 2 * bit1 + 4 * bit2 + 8 * bit3);

                    /* high 4 bits are for sprite n + 8 */
                    bit0 = (color_prom[i + 32] >> 7) & 0x01;
                    bit1 = (color_prom[i + 32] >> 6) & 0x01;
                    bit2 = (color_prom[i + 32] >> 5) & 0x01;
                    bit3 = (color_prom[i + 32] >> 4) & 0x01;
                    colortable[i + 4 * 16]= (ushort)(1 * bit0 + 2 * bit1 + 4 * bit2 + 8 * bit3);
                }
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
                int i, offs;
                for (offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy;


                        Generic.dirtybuffer[offs] = false;

                        sx = offs % 32;
                        sy = offs / 32;

                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 32 * (Generic.colorram[offs] & 8)),
                                Generic.colorram[offs],
                                flipscreen != 0, flipscreen != 0,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int[] scroll = new int[32];
                    int sx, sy;


                    for (offs = 0; offs < 32; offs++)
                    {
                        sx = offs % 4;
                        sy = offs / 4;

                        if (flipscreen != 0)
                            scroll[31 - offs] = -Generic.videoram[32 * sx + sy];
                        else
                            scroll[offs] = -Generic.videoram[32 * sx + sy];
                    }

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 32, scroll, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                /* sprites in the columns 15, 1 and 0 are outside of the visible area */
                for (offs = Generic.spriteram_size[0] - 2 * 0x40; offs >= 2 * 0x40; offs -= 0x40)
                {
                    i = 0;
                    while (i < 0x40 && Generic.spriteram[offs + i] != 0)
                        i += 4;

                    while (i > 0)
                    {
                        /*
                         abccdddd eeeeeeee fffghhhh iiiiiiii

                         a enable?
                         b size (0 = 8x8, 1 = 16x16)
                         cc flip
                         dddd y offset
                         eeeeeeee sprite code (shift right 2 bits for 16x16 sprites)
                         fff unknown
                         g sprite bank
                         hhhh color
                         iiiiiiii x position
                        */
                        i -= 4;

                        if ((Generic.spriteram[offs + i] & 0x80) != 0)
                        {
                            if ((Generic.spriteram[offs + i] & 0x40) != 0)	/* 16x16 */
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                        (uint)((Generic.spriteram[offs + i + 1] >> 2) + 4 * (Generic.spriteram[offs + i + 2] & 0x10)),
                                        (uint)(Generic.spriteram[offs + i + 2] & 0x0f),
                                        (Generic.spriteram[offs + i] & 0x20) != 0, (Generic.spriteram[offs + i] & 0x10) != 0,
                                        Generic.spriteram[offs + i + 3],
                                        offs / 4 - 8 + (Generic.spriteram[offs + i] & 0x0f),
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                            else	/* 8x8 */
                                Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                        (uint)(Generic.spriteram[offs + i + 1] + 4 * (Generic.spriteram[offs + i + 2] & 0x10)),
                                        (uint)(Generic.spriteram[offs + i + 2] & 0x0f),
                                        (Generic.spriteram[offs + i] & 0x20) != 0, (Generic.spriteram[offs + i] & 0x10) != 0,
                                        Generic.spriteram[offs + i + 3],
                                        offs / 4 + (Generic.spriteram[offs + i] & 0x0f),
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_ladybug()
        {
            ROM_START("ladybug");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("lb1.cpu", 0x0000, 0x1000, 0xd09e0adb);
            ROM_LOAD("lb2.cpu", 0x1000, 0x1000, 0x88bc4a0a);
            ROM_LOAD("lb3.cpu", 0x2000, 0x1000, 0x53e9efce);
            ROM_LOAD("lb4.cpu", 0x3000, 0x1000, 0xffc424d7);
            ROM_LOAD("lb5.cpu", 0x4000, 0x1000, 0xad6af809);
            ROM_LOAD("lb6.cpu", 0x5000, 0x1000, 0xcf1acca4);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lb9.vid", 0x0000, 0x1000, 0x77b1da1e);
            ROM_LOAD("lb10.vid", 0x1000, 0x1000, 0xaa82e00b);

            ROM_REGION(0x2000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("lb8.cpu", 0x0000, 0x1000, 0x8b99910b);
            ROM_LOAD("lb7.cpu", 0x1000, 0x1000, 0x86a5b448);

            ROM_REGION(0x0060, Mame.REGION_PROMS);
            ROM_LOAD("10-2.vid", 0x0000, 0x0020, 0xdf091e52); /* palette */
            ROM_LOAD("10-1.vid", 0x0020, 0x0020, 0x40640d8f); /* sprite color lookup table */
            ROM_LOAD("10-3.vid", 0x0040, 0x0020, 0x27fa3a50); /* ?? */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ladybug()
        {

            INPUT_PORTS_START("ladybug");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_UNUSED);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            /* This should be connected to the 4V clock. I don't think the game uses it. */
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            /* Note that there are TWO VBlank inputs, one is active low, the other active */
            /* high. There are probably other differencies in the hardware, but emulating */
            /* them this way is enough to get the game running. */
            PORT_BIT(0xc0, 0x40, (uint)inptports.IPT_VBLANK);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_UNUSED);
            PORT_BIT(0x0e, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0xe0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* DSW0 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x03, "Easy");
            PORT_DIPSETTING(0x02, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x04, 0x04, "High Score Names");
            PORT_DIPSETTING(0x00, "3 Letters");
            PORT_DIPSETTING(0x04, "10 Letters");
            PORT_BITX(0x08, 0x08, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Rack Test", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x10, "Freeze");
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x20, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Free Play"));
            PORT_DIPSETTING(0x40, DEF_STR("No"));
            PORT_DIPSETTING(0x00, DEF_STR("Yes"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Lives"));
            PORT_DIPSETTING(0x80, "3");
            PORT_DIPSETTING(0x00, "5");

            PORT_START();	/* DSW1 */
            PORT_DIPNAME(0x0f, 0x0f, "Right Coin");
            PORT_DIPSETTING(0x06, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x0a, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x07, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x0f, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x09, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0d, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0b, DEF_STR("1C_5C"));
            /* settings 0x00 thru 0x05 all give 1 Coin/1 Credit */
            PORT_DIPNAME(0xf0, 0xf0, "Left Coin");
            PORT_DIPSETTING(0x60, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x70, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x90, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0xd0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xb0, DEF_STR("1C_5C"));
            /* settings 0x00 thru 0x50 all give 1 Coin/1 Credit */

            PORT_START();	/* FAKE */
            /* The coin slots are not memory mapped. Coin Left causes a NMI, */
            /* Coin Right an IRQ. This fake input port is used by the interrupt */
            /* handler to be notified of coin insertions. We use IMPULSE to */
            /* trigger exactly one interrupt, without having to check when the */
            /* user releases the key. */
            PORT_BIT_IMPULSE(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1, 1);
            PORT_BIT_IMPULSE(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2, 1);
            return INPUT_PORTS_END;
        }
        public driver_ladybug()
        {
            drv = new machine_driver_ladybug();
            year = "1981";
            name = "ladybug";
            description = "Lady Bug";
            manufacturer = "Universal";
            flags = Mame.ROT270;
            input_ports = input_ports_ladybug();
            rom = rom_ladybug();
            drv.HasNVRAMhandler = false;
        }
    }
}
