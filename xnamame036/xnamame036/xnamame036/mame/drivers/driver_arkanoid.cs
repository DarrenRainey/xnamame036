using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_arkanoid : Mame.GameDriver
    {
        static int[] flipscreen = new int[2];
        static int gfxbank, palettebank;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xd001, 0xd001, ay8910.AY8910_read_port_0_r ),
	new Mame.MemoryReadAddress( 0xd00c, 0xd00c, arkanoid_68705_input_0_r ),  /* mainly an input port, with 2 bits from the 68705 */
	new Mame.MemoryReadAddress( 0xd010, 0xd010, Mame.input_port_1_r ),
	new Mame.MemoryReadAddress( 0xd018, 0xd018, arkanoid_Z80_mcu_r ),  /* input from the 68705 */
	new Mame.MemoryReadAddress( 0xe000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xd001, 0xd001, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xd008, 0xd008, arkanoid_d008_w ),	/* gfx bank, flip screen etc. */
	new Mame.MemoryWriteAddress( 0xd010, 0xd010, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0xd018, 0xd018, arkanoid_Z80_mcu_w ), /* output to the 68705 */
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe800, 0xe83f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xe840, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] boot_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xd001, 0xd001, ay8910.AY8910_read_port_0_r ),
	new Mame.MemoryReadAddress( 0xd00c, 0xd00c, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xd010, 0xd010, Mame.input_port_1_r),
	new Mame.MemoryReadAddress( 0xd018, 0xd018, arkanoid_input_2_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xf000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] boot_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xd001, 0xd001, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xd008, 0xd008, arkanoid_d008_w ),	/* gfx bank, flip screen etc. */
	new Mame.MemoryWriteAddress( 0xd010, 0xd010, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0xd018, 0xd018, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe800, 0xe83f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xe840, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.MemoryReadAddress[] mcu_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0000, arkanoid_68705_portA_r ),
	new Mame.MemoryReadAddress( 0x0001, 0x0001, arkanoid_input_2_r ),
	new Mame.MemoryReadAddress( 0x0002, 0x0002, arkanoid_68705_portC_r),
	new Mame.MemoryReadAddress( 0x0010, 0x007f,Mame.MRA_RAM),
	new Mame.MemoryReadAddress( 0x0080, 0x07ff,Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] mcu_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0000, arkanoid_68705_portA_w),
	new Mame.MemoryWriteAddress( 0x0002, 0x0002, arkanoid_68705_portC_w ),
	new Mame.MemoryWriteAddress( 0x0004, 0x0004, arkanoid_68705_ddrA_w ),
	new Mame.MemoryWriteAddress( 0x0006, 0x0006, arkanoid_68705_ddrC_w ),
	new Mame.MemoryWriteAddress( 0x0010, 0x007f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0080, 0x07ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            4096,	/* 4096 characters */
            3,	/* 3 bits per pixel */
            new uint[] { 2 * 4096 * 8 * 8, 4096 * 8 * 8, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );



        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,  0, 64 ),
	/* sprites use the same characters above, but are 16x8 */
};



        static AY8910interface ay8910_interface =
        new AY8910interface(
            1,	/* 1 chips */
            1500000,	/* 1.5 MHz ???? */
            new int[] { 33 },
            new AY8910portRead[] { null },
            new AY8910portRead[] { Mame.input_port_4_r },
            new AY8910portWrite[] { null },
            new AY8910portWrite[] { null }, null
        );
        static int arkanoid_paddle_select;
        static int z80write, fromz80, m68705write, toz80;
        static byte portA_in, portA_out, ddrA, portC_out, ddrC;

        static void arkanoid_init_machine()
        {
            portA_in = portA_out = 0;
            z80write = m68705write = 0;
        }

        static int arkanoid_Z80_mcu_r(int value)
        {
            /* return the last value the 68705 wrote, and mark that we've read it */
            m68705write = 0;
            return toz80;
        }

        static void arkanoid_Z80_mcu_w(int offset, int data)
        {
            /* a write from the Z80 has occurred, mark it and remember the value */
            z80write = 1;
            fromz80 = data;

            /* give up a little bit of time to let the 68705 detect the write */
            Mame.cpu_spinuntil_trigger(700);
        }

        static int arkanoid_68705_portA_r(int offset)
        {
            return (portA_out & ddrA) | (portA_in & ~ddrA);
        }
        static void arkanoid_68705_portA_w(int offset, int data)
        {
            portA_out = (byte)data;
        }
        static void arkanoid_68705_ddrA_w(int offset, int data)
        {
            ddrA = (byte)data;
        }
        static int arkanoid_68705_portC_r(int offset)
        {
            int res = 0;

            /* bit 0 is high on a write strobe; clear it once we've detected it */
            if (z80write != 0) res |= 0x01;

            /* bit 1 is high if the previous write has been read */
            if (m68705write == 0) res |= 0x02;

            return (portC_out & ddrC) | (res & ~ddrC);
        }
        static void arkanoid_68705_portC_w(int offset, int data)
        {
            if ((ddrC & 0x04) != 0 && (~data & 0x04) != 0 && (portC_out & 0x04) != 0)
            {
                /* mark that the command has been seen */
                Mame.cpu_trigger(700);

                /* return the last value the Z80 wrote */
                z80write = 0;
                portA_in = (byte)fromz80;
            }
            if ((ddrC & 0x08) != 0 && (~data & 0x08) != 0 && (portC_out & 0x08) != 0)
            {
                /* a write from the 68705 to the Z80; remember its value */
                m68705write = 1;
                toz80 = portA_out;
            }

            portC_out = (byte)(byte)data;
        }
        static void arkanoid_68705_ddrC_w(int offset, int data)
        {
            ddrC = (byte)data;
        }
        static int arkanoid_68705_input_0_r(int offset)
        {
            int res = Mame.input_port_0_r(offset) & 0x3f;

            /* bit 0x40 comes from the sticky bit */
            if (z80write == 0) res |= 0x40;

            /* bit 0x80 comes from a write latch */
            if (m68705write == 0) res |= 0x80;

            return res;
        }
        static int arkanoid_input_2_r(int offset)
        {
            if (arkanoid_paddle_select != 0)
            {
                return Mame.input_port_3_r(offset);
            }
            else
            {
                return Mame.input_port_2_r(offset);
            }
        }
        static void arkanoid_d008_w(int offset, int data)
        {
            /* bits 0 and 1 flip X and Y, I don't know which is which */
            if (flipscreen[0] != (data & 0x01))
            {
                flipscreen[0] = data & 0x01;
                Generic.SetDirtyBuffer(true);// memset(dirtybuffer, 1, videoram_size);
            }
            if (flipscreen[1] != (data & 0x02))
            {
                flipscreen[1] = data & 0x02;
                Generic.SetDirtyBuffer(true);//memset(dirtybuffer, 1, videoram_size);
            }

            /* bit 2 selects the input paddle */
            arkanoid_paddle_select = data & 0x04;

            /* bit 3 is coin lockout (but not the service coin) */
            Mame.coin_lockout_w(0, (data & 0x08) == 0 ? 1 : 0);
            Mame.coin_lockout_w(1, (data & 0x08) == 0 ? 1 : 0);

            /* bit 4 is unknown */

            /* bits 5 and 6 control gfx bank and palette bank. They are used together */
            /* so I don't know which is which. */
            if (gfxbank != ((data & 0x20) >> 5))
            {
                gfxbank = (data & 0x20) >> 5;
                Generic.SetDirtyBuffer(true);// memset(dirtybuffer, 1, videoram_size);
            }
            if (palettebank != ((data & 0x40) >> 6))
            {
                palettebank = (data & 0x40) >> 6;
                Generic.SetDirtyBuffer(true);//memset(dirtybuffer, 1, videoram_size);
            }

            /* bit 7 is unknown */
        }



        class machine_driver_arkanoid : Mame.MachineDriver
        {
            public machine_driver_arkanoid()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 6000000, readmem, writemem, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68705, 500000, mcu_readmem, mcu_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_arkanoid.gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
            }
            public override void init_machine()
            {
                arkanoid_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[cpi] >> 1) & 0x01;
                    int bit2 = (color_prom[cpi] >> 2) & 0x01;
                    int bit3 = (color_prom[cpi] >> 3) & 0x01;
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
                //none
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                for (int offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    int offs2;

                    offs2 = offs / 2;
                    if (Generic.dirtybuffer[offs] || Generic.dirtybuffer[offs + 1])
                    {
                        int sx, sy, code;


                        Generic.dirtybuffer[offs] = false;
                        Generic.dirtybuffer[offs + 1] = false;

                        sx = offs2 % 32;
                        sy = offs2 / 32;

                        if (flipscreen[0] != 0) sx = 31 - sx;
                        if (flipscreen[1] != 0) sy = 31 - sy;

                        code = Generic.videoram[offs + 1] + ((Generic.videoram[offs] & 0x07) << 8) + 2048 * gfxbank;
                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)(((Generic.videoram[offs] & 0xf8) >> 3) + 32 * palettebank),
                                flipscreen[0] != 0, flipscreen[1] != 0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. */
                for (int offs = 0; offs < Generic.spriteram_size[0]; offs += 4)
                {
                    int sx, sy, code;


                    sx = Generic.spriteram[offs];
                    sy = 248 - Generic.spriteram[offs + 1];
                    if (flipscreen[0] != 0) sx = 248 - sx;
                    if (flipscreen[1] != 0) sy = 248 - sy;

                    code = Generic.spriteram[offs + 3] + ((Generic.spriteram[offs + 2] & 0x03) << 8) + 1024 * gfxbank;
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            (uint)(2 * code),
                            (uint)(((Generic.spriteram[offs + 2] & 0xf8) >> 3) + 32 * palettebank),
                            flipscreen[0] != 0, flipscreen[1] != 0,
                            sx, sy + (flipscreen[1] != 0 ? 8 : -8),
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            (uint)(2 * code + 1),
                            (uint)(((Generic.spriteram[offs + 2] & 0xf8) >> 3) + 32 * palettebank),
                            flipscreen[0] != 0, flipscreen[1] != 0,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }
        }
        public override void driver_init()
        {
            //none
        }
        Mame.InputPortTiny[] input_ports_arkanoid()
        {
            INPUT_PORTS_START("arkanoid");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* input from the 68705, some bootlegs need it to be 1 */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);	/* input from the 68705 */

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0xf8, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2 - spinner Player 1");
            PORT_ANALOG(0xff, 0x00, (uint)inptports.IPT_DIAL, 30, 15, 0, 0);

            PORT_START("IN3 - spinner Player 2");
            PORT_ANALOG(0xff, 0x00, (uint)inptports.IPT_DIAL | IPF_COCKTAIL, 30, 15, 0, 0);

            PORT_START("DSW1");
            PORT_DIPNAME(0x01, 0x00, "Allow Continue");
            PORT_DIPSETTING(0x01, DEF_STR("No"));
            PORT_DIPSETTING(0x00, DEF_STR("Yes"));
            PORT_DIPNAME(0x02, 0x02, DEF_STR("Flip Screen"));
            PORT_DIPSETTING(0x02, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x08, "Easy");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x10, 0x10, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x10, "20000 60000");
            PORT_DIPSETTING(0x00, "20000");
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Lives"));
            PORT_DIPSETTING(0x20, "3");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_6C"));
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_arkanoid()
        {
            ROM_START("arkanoid");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("a75_01-1.rom", 0x0000, 0x8000, 0x5bcda3b0);
            ROM_LOAD("a75_11.rom", 0x8000, 0x8000, 0xeafd7191);

            ROM_REGION(0x0800, Mame.REGION_CPU2);	/* 8k for the microcontroller */
            ROM_LOAD("arkanoid.uc", 0x0000, 0x0800, 0x515d77b6);

            ROM_REGION(0x18000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a75_03.rom", 0x00000, 0x8000, 0x038b74ba);
            ROM_LOAD("a75_04.rom", 0x08000, 0x8000, 0x71fae199);
            ROM_LOAD("a75_05.rom", 0x10000, 0x8000, 0xc76374e2);

            ROM_REGION(0x0600, Mame.REGION_PROMS);
            ROM_LOAD("07.bpr", 0x0000, 0x0200, 0x0af8b289);	/* red component */
            ROM_LOAD("08.bpr", 0x0200, 0x0200, 0xabb002fb);	/* green component */
            ROM_LOAD("09.bpr", 0x0400, 0x0200, 0xa7c6c277);	/* blue component */
            return ROM_END;
        }
        public driver_arkanoid()
        {
            drv = new machine_driver_arkanoid();
            year = "1986";
            name = "arkanoid";
            description = "Arkanoid (World)";
            manufacturer = "Taito Corporation Japan";
            flags = Mame.ROT90;
            input_ports = input_ports_arkanoid();
            rom = rom_arkanoid();
            drv.HasNVRAMhandler = false;
        }
    }
}
