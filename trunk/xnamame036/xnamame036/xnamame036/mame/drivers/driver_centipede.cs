using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_centipede : Mame.GameDriver
    {
        static int flipscreen;
        static Mame.rectangle spritevisiblearea = new Mame.rectangle(1 * 8, 31 * 8 - 1, 0 * 8, 30 * 8 - 1);
        static int powerup_counter;

        static void centiped_led_w(int offset, int data)
        {
            Mame.osd_led_w(offset, ~data >> 7);
        }
        static int centipdb_rand_r(int offset)
        {
            return Mame.rand() % 0xff;
        }
        static void centipdb_AY8910_w(int offset, int data)
        {
            ay8910.AY8910_control_port_0_w(0, offset);
            ay8910.AY8910_write_port_0_w(0, data);
        }
        static int centipdb_AY8910_r(int offset)
        {
            ay8910.AY8910_control_port_0_w(0, offset);
            return ay8910.AY8910_read_port_0_r(0);
        }
        static int oldpos1, sign1;
        static int centiped_IN0_r(int offset)
        {
            int newpos = Mame.readinputport(6);
            if (newpos != oldpos1)
            {
                sign1 = (newpos - oldpos1) & 0x80;
                oldpos1 = newpos;
            }

            return ((Mame.readinputport(0) & 0x70) | (oldpos1 & 0x0f) | sign1);
        }

        static int oldpos2, sign2;
        static int centiped_IN2_r(int offset)
        {
            int newpos = Mame.readinputport(2);
            if (newpos != oldpos2)
            {
                sign2 = (newpos - oldpos2) & 0x80;
                oldpos2 = newpos;
            }

            return ((oldpos2 & 0x0f) | sign2);
        }
        static void centiped_vh_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 0x80))
            {
                flipscreen = data & 0x80;
                Generic.SetDirtyBuffer(true);
            }
        }
        static void centiped_paletteram_w(int offset, int data)
        {
            Mame.paletteram[offset] = (byte)data;

            /* the char palette will be effectively updated by the next interrupt handler */

            if (offset >= 12 && offset < 16)	/* sprites palette */
            {
                int start = Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start;

                setcolor(start + (offset - 12), data);
            }
        }

        static Mame.MemoryWriteAddress[] centiped_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x03ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0400, 0x07bf, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x07c0, 0x07ff, Mame.MWA_RAM, Generic.spriteram ),
	new Mame.MemoryWriteAddress( 0x1000, 0x100f, Pokey.pokey1_w ),
	new Mame.MemoryWriteAddress( 0x1400, 0x140f, centiped_paletteram_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x1600, 0x163f, atari_vg.atari_vg_earom_w ),
	new Mame.MemoryWriteAddress( 0x1680, 0x1680, atari_vg.atari_vg_earom_ctrl ),
	new Mame.MemoryWriteAddress( 0x1800, 0x1800, Mame.MWA_NOP ),	/* IRQ acknowldege */
	new Mame.MemoryWriteAddress( 0x1c00, 0x1c02, Mame.coin_counter_w ),
	new Mame.MemoryWriteAddress( 0x1c03, 0x1c04, centiped_led_w ),
	new Mame.MemoryWriteAddress( 0x1c07, 0x1c07, centiped_vh_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x3fff,Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.MemoryReadAddress[] centiped_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x03ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0400, 0x07ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x0800, 0x0800, Mame.input_port_4_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0x0801, 0x0801, Mame.input_port_5_r ),	/* DSW2 */
	new Mame.MemoryReadAddress( 0x0c00, 0x0c00, centiped_IN0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0x0c01, 0x0c01, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0x0c02, 0x0c02, centiped_IN2_r ),	/* IN2 */	/* JB 971220 */
	new Mame.MemoryReadAddress( 0x0c03, 0x0c03, Mame.input_port_3_r ),	/* IN3 */
	new Mame.MemoryReadAddress( 0x1000, 0x100f, Pokey.pokey1_r ),
	new Mame.MemoryReadAddress( 0x1700, 0x173f, atari_vg.atari_vg_earom_r ),
	new Mame.MemoryReadAddress( 0x2000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xf800, 0xffff, Mame.MRA_ROM ),	/* for the reset / interrupt vectors */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 256 * 8 * 8, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            8, 16,	/* 16*8 sprites */
            128,	/* 64 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 128 * 16 * 8, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			8*8, 9*8, 10*8, 11*8, 12*8, 13*8, 14*8, 15*8 },
            16 * 8	/* every sprite takes 16 consecutive bytes */
        );



        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,   4, 4 ),	/* 4 color codes to support midframe */
												/* palette changes in test mode */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, spritelayout, 0, 1 ),
};



        static POKEYinterface pokey_interface =
        new POKEYinterface(
            1,	/* 1 chip */
            12096000 / 8,	/* 1.512 MHz */
            new int[] { 100 },
            /* The 8 pot handlers */
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            new pot_delegate[] { null },
            /* The allpot handler */
            new pot_delegate[] { null }
        );
        
        static void setcolor(int pen, int data)
        {
            int r = 0xff * ((~data >> 0) & 1);
            int g = 0xff * ((~data >> 1) & 1);
            int b = 0xff * ((~data >> 2) & 1);

            if ((~data & 0x08) != 0)/* alternate = 1 */
            {
                /* when blue component is not 0, decrease it. When blue component is 0, */
                /* decrease green component. */
                if (b != 0) b = 0xc0;
                else if (g != 0) g = 0xc0;
            }

            Mame.palette_change_color(pen, (byte)r, (byte)g, (byte)b);
        }
        static int centiped_interrupt()
        {
            int slice = 3 - Mame.cpu_getiloops();
            int start = Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start;

            /* set the palette for the previous screen slice to properly support */
            /* midframe palette changes in test mode */
            for (int offset = 4; offset < 8; offset++)
                setcolor(4 * slice + start + (offset - 4), Mame.paletteram[offset]);

            /* Centipede doesn't like to receive interrupts just after a reset. */
            /* The only workaround I've found is to wait a little before starting */
            /* to generate them. */
            if (powerup_counter == 0)
                return Mame.interrupt();
            else
            {
                powerup_counter--;
                return Mame.ignore_interrupt();
            }
        }
        class machine_driver_centipede : Mame.MachineDriver
        {
            public machine_driver_centipede()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502, 12096000 / 8, centiped_readmem, centiped_writemem, null, null, centiped_interrupt, 4));
                frames_per_second = 60;
                vblank_duration = 1460;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_centipede.gfxdecodeinfo;
                total_colors = 4 + 4 * 4;
                color_table_len = 4 + 4 * 4;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_POKEY, pokey_interface));
            }
            public override void init_machine()
            {
                powerup_counter = 10;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                atari_vg.atari_vg_earom_handler(file, read_or_write);
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                if (Mame.palette_recalc() != null || full_refresh != 0)
                    Generic.SetDirtyBuffer(true);

                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                               (uint) ((Generic.videoram[offs] & 0x3f) + 0x40),
                                (uint)(sy + 1) / 8,	/* support midframe palette changes in test mode */
                                flipscreen!=0, flipscreen!=0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                /* Draw the sprites */
                for (int offs = 0; offs < 0x10; offs++)
                {
                    int spritenum = Generic.spriteram[offs] & 0x3f;
                    if ((spritenum & 1)!=0) spritenum = spritenum / 2 + 64;
                    else spritenum = spritenum / 2;

                    int flipx = (Generic.spriteram[offs] & 0x80) ^ flipscreen;
                    int x = Generic.spriteram[offs + 0x20];
                    int y = 240 - Generic.spriteram[offs + 0x10];

                    /* Centipede is unusual because the sprite color code specifies the */
                    /* colors to use one by one, instead of a combination code. */
                    /* bit 5-4 = color to use for pen 11 */
                    /* bit 3-2 = color to use for pen 10 */
                    /* bit 1-0 = color to use for pen 01 */
                    /* pen 00 is transparent */
                    int color = Generic.spriteram[offs + 0x30];
                    Mame.Machine.gfx[1].colortable[3]=Mame.Machine.pens[Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + ((color >> 4) & 3)];
                    Mame.Machine.gfx[1].colortable[2]=Mame.Machine.pens[Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + ((color >> 2) & 3)];
                    Mame.Machine.gfx[1].colortable[1]=Mame.Machine.pens[Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + ((color >> 0) & 3)];

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)spritenum, 0,
                            flipscreen!=0, flipx!=0,
                            x, y,
                            spritevisiblearea, Mame.TRANSPARENCY_PEN, 0);

                    /* mark tiles underneath as dirty */
                    int sx = x >> 3;
                    int sy = y >> 3;

                    {
                        int max_x = 1;
                        int max_y = 2;

                        if ((x & 0x07)!=0) max_x++;
                        if ((y & 0x0f)!=0) max_y++;

                        for (int y2 = sy; y2 < sy + max_y; y2++)
                        {
                            for (int x2 = sx; x2 < sx + max_x; x2++)
                            {
                                if ((x2 < 32) && (y2 < 30) && (x2 >= 0) && (y2 >= 0))
                                    Generic.dirtybuffer[x2 + 32 * y2] = true;
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
        Mame.InputPortTiny[] input_ports_centipede()
        {
            INPUT_PORTS_START("centiped");
            PORT_START("IN0");
            /* The lower 4 bits and bit 7 are for trackball x input. */
            /* They are handled by fake input port 6 and a custom routine. */
            PORT_BIT(0x0f, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x10, DEF_STR("Cocktail"));
            PORT_SERVICE(0x20, IP_ACTIVE_LOW);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_VBLANK);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);

            PORT_START("IN2");
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_TRACKBALL_Y, 50, 10, 0, 0, IP_KEY_NONE, IP_KEY_NONE, IP_JOY_NONE, IP_JOY_NONE);
            /* The lower 4 bits are the input, and bit 7 is the direction. */
            /* The state of bit 7 does not change if the trackball is not moved.*/

            PORT_START("IN3");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);

            PORT_START("IN4");
            PORT_DIPNAME(0x03, 0x00, "Language");
            PORT_DIPSETTING(0x00, "English");
            PORT_DIPSETTING(0x01, "German");
            PORT_DIPSETTING(0x02, "French");
            PORT_DIPSETTING(0x03, "Spanish");
            PORT_DIPNAME(0x0c, 0x04, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x04, "3");
            PORT_DIPSETTING(0x08, "4");
            PORT_DIPSETTING(0x0c, "5");
            PORT_DIPNAME(0x30, 0x10, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "10000");
            PORT_DIPSETTING(0x10, "12000");
            PORT_DIPSETTING(0x20, "15000");
            PORT_DIPSETTING(0x30, "20000");
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x40, "Easy");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x80, 0x00, "Credit Minimum");
            PORT_DIPSETTING(0x00, "1");
            PORT_DIPSETTING(0x80, "2");

            PORT_START("IN5");
            PORT_DIPNAME(0x03, 0x02, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x03, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0x0c, 0x00, "Right Coin");
            PORT_DIPSETTING(0x00, "*1");
            PORT_DIPSETTING(0x04, "*4");
            PORT_DIPSETTING(0x08, "*5");
            PORT_DIPSETTING(0x0c, "*6");
            PORT_DIPNAME(0x10, 0x00, "Left Coin");
            PORT_DIPSETTING(0x00, "*1");
            PORT_DIPSETTING(0x10, "*2");
            PORT_DIPNAME(0xe0, 0x00, "Bonus Coins");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPSETTING(0x20, "3 credits/2 coins");
            PORT_DIPSETTING(0x40, "5 credits/4 coins");
            PORT_DIPSETTING(0x60, "6 credits/4 coins");
            PORT_DIPSETTING(0x80, "6 credits/5 coins");
            PORT_DIPSETTING(0xa0, "4 credits/3 coins");

            PORT_START("IN6");// fake trackball input port. */							
            PORT_ANALOGX(0xff, 0x00, (uint)inptports.IPT_TRACKBALL_X | IPF_REVERSE, 50, 10, 0, 0, IP_KEY_NONE, IP_KEY_NONE, IP_JOY_NONE, IP_JOY_NONE);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_centipede()
        {
            ROM_START("centiped");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("centiped.307", 0x2000, 0x0800, 0x5ab0d9de);
            ROM_LOAD("centiped.308", 0x2800, 0x0800, 0x4c07fd3e);
            ROM_LOAD("centiped.309", 0x3000, 0x0800, 0xff69b424);
            ROM_LOAD("centiped.310", 0x3800, 0x0800, 0x44e40fa4);
            ROM_RELOAD(0xf800, 0x0800);	/* for the reset and interrupt vectors */

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("centiped.211", 0x0000, 0x0800, 0x880acfb9);
            ROM_LOAD("centiped.212", 0x0800, 0x0800, 0xb1397029);
            return ROM_END;
        }
        public driver_centipede()
        {
            drv = new machine_driver_centipede();
            year = "1980";
            name = "centiped";
            description = "Centipede (revision 3)";
            manufacturer = "Atari";
            flags = Mame.ROT270;
            input_ports = input_ports_centipede();
            rom = rom_centipede();
            drv.HasNVRAMhandler = true;
        }
    }
}
