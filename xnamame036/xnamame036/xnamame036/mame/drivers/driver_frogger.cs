using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public partial class driver_frogger : Mame.GameDriver
    {
        static AY8910interface ay8910_interface = new AY8910interface(
            1, 14318000 / 8, new int[] { Mame.MIXERG(80, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER) },
            new AY8910portRead[] { Mame.soundlatch_r }, new AY8910portRead[] { frogger_portB_r }, new AY8910portWrite[] { null, null }, new AY8910portWrite[] { null, null }, new AY8910handler[] { });

        static _BytePtr frogger_attributesram = new _BytePtr(1);

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8800, 0x8800, Mame.watchdog_reset_r ),
	new Mame.MemoryReadAddress( 0xa800, 0xabff, Mame.MRA_RAM ),	/* video RAM */
	new Mame.MemoryReadAddress( 0xb000, 0xb05f, Mame.MRA_RAM ),	/* screen attributes, sprites */
	new Mame.MemoryReadAddress( 0xe000, 0xe000, Mame.input_port_0_r),	/* IN0 */
	new Mame.MemoryReadAddress( 0xe002, 0xe002, Mame.input_port_1_r),	/* IN1 */
	new Mame.MemoryReadAddress( 0xe004, 0xe004, Mame.input_port_2_r),	/* IN2 */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xa800, 0xabff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb03f, frogger_attributes_w, frogger_attributesram ),
	new Mame.MemoryWriteAddress( 0xb040, 0xb05f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xb808, 0xb808, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0xb80c, 0xb80c, frogger_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0xb818, 0xb818, Mame.coin_counter_w ),
	new Mame.MemoryWriteAddress( 0xb81c, 0xb81c, frogger_counterb_w ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd000, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xd002, 0xd002, frogger_sh_irqtrigger_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x17ff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x43ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0x17ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0x4000, 0x43ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1)	/* end of table */
};


        static Mame.IOWritePort[] sound_writeport = {
                                                        new Mame.IOWritePort(0x80,0x80,ay8910.AY8910_control_port_0_w),
                                                        new Mame.IOWritePort(0x40,0x40,ay8910.AY8910_write_port_0_w),
                                                        new Mame.IOWritePort(-1),
                                                    };
        static Mame.IOReadPort[] sound_readport = {
                                                      new Mame.IOReadPort(0x40,0x40,ay8910.AY8910_read_port_0_r),
                                                      new Mame.IOReadPort(-1)
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
            16, 16,	/* 16*16 sprites */
            64,	/* 64 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 64 * 16 * 16, 0 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3, 8 * 8 + 4, 8 * 8 + 5, 8 * 8 + 6, 8 * 8 + 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 16 * 8, 17 * 8, 18 * 8, 19 * 8, 20 * 8, 21 * 8, 22 * 8, 23 * 8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );
        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,     0, 16 ),
new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, spritelayout,   0,  8 ),
};

        static void frogger_counterb_w(int offset, int data)
        {
            Mame.coin_counter_w(1, data);
        }
        static int flipscreen;


        static void frogger_attributes_w(int offset, int data)
        {
            if ((offset & 1) != 0 && frogger_attributesram[offset] != data)
            {
                int i;


                for (i = offset / 2; i < Generic.videoram_size[0]; i += 32)
                    Generic.dirtybuffer[i] = true;
            }

            frogger_attributesram[offset] = (byte)data;
        }

        static void frogger_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 1))
            {
                flipscreen = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }

        class machine_driver_frogger : Mame.MachineDriver
        {
            public machine_driver_frogger()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6, readmem, writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318000 / 8, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = 2500;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_frogger.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 64;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
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
            public override void vh_eof_callback()
            {
                //None
            }
            public override void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom)
            {
                for (int i = 0; i < 32; i++)
                {
                    int bit0 = (color_prom[i] >> 0) & 0x01;
                    int bit1 = (color_prom[i] >> 1) & 0x01;
                    int bit2 = (color_prom[i] >> 2) & 0x01;
                    palette[3 * i] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = (color_prom[i] >> 3) & 0x01;
                    bit1 = (color_prom[i] >> 4) & 0x01;
                    bit2 = (color_prom[i] >> 5) & 0x01;
                    palette[3 * i + 1] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                    bit0 = 0;
                    bit1 = (color_prom[i] >> 6) & 0x01;
                    bit2 = (color_prom[i] >> 7) & 0x01;
                    palette[3 * i + 2] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                }

                /* use an otherwise unused pen for the river background */
                palette[3 * 4] = 0;
                palette[3 * 4 + 1] = 0;
                palette[3 * 4 + 2] = 0x47;

                /* normal */
                for (int i = 0; i < 4 * 8; i++)
                {
                    if ((i & 3) != 0)
                        colortable[i]= (ushort)i;
                    else colortable[i]= 0;
                }
                /* blue background (river) */
                for (int i = 4 * 8; i < 4 * 16; i++)
                {
                    if ((i & 3) != 0)
                        colortable[i]= (ushort)(i - 4 * 8);
                    else colortable[i]= 4;
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
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {

                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs])
                    {
                        int sx, sy, col;


                        Generic.dirtybuffer[offs] = false;

                        sx = offs % 32;
                        sy = offs / 32;
                        col = frogger_attributesram[2 * sx + 1] & 7;
                        col = ((col >> 1) & 0x03) | ((col << 2) & 0x04);

                        if (flipscreen != 0)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    Generic.videoram[offs],
                                    (uint)(col + (sx >= 16 ? 8 : 0)),	/* blue background in the lower 128 lines */
                                    flipscreen != 0, flipscreen != 0, 8 * sx, 8 * sy,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                        else
                        {
                            Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                    Generic.videoram[offs],
                                    (uint)(col + (sx <= 15 ? 8 : 0)),	/* blue background in the upper 128 lines */
                                    flipscreen != 0, flipscreen != 0, 8 * sx, 8 * sy,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }


                /* copy the temporary bitmap to the screen */
                {
                    int[] scroll = new int[32];
                    int s;

                    for (int i = 0; i < 32; i++)
                    {
                        s = frogger_attributesram[2 * i];
                        if (flipscreen != 0)
                        {
                            scroll[31 - i] = (((s << 4) & 0xf0) | ((s >> 4) & 0x0f));
                        }
                        else
                        {
                            scroll[i] = -(((s << 4) & 0xf0) | ((s >> 4) & 0x0f));
                        }
                    }

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 0, null, 32, scroll, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    if (Generic.spriteram[offs + 3] != 0)
                    {
                        int x, y, col;

                        x = Generic.spriteram[offs + 3];
                        y = Generic.spriteram[offs];
                        y = ((y << 4) & 0xf0) | ((y >> 4) & 0x0f);
                        col = Generic.spriteram[offs + 2] & 7;
                        col = ((col >> 1) & 0x03) | ((col << 2) & 0x04);

                        if (flipscreen != 0)
                        {
                            x = 242 - x;
                            y = 240 - y;
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)(Generic.spriteram[offs + 1] & 0x3f),
                                    (uint)col,
                                    !((Generic.spriteram[offs + 1] & 0x40) != 0), !((Generic.spriteram[offs + 1] & 0x80) != 0),
                                    x, 30 * 8 - y,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                        else
                        {
                            Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                    (uint)(Generic.spriteram[offs + 1] & 0x3f),
                                    (uint)col,
                                    (Generic.spriteram[offs + 1] & 0x40) != 0, (Generic.spriteram[offs + 1] & 0x80) != 0,
                                    x, 30 * 8 - y,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
            }
        }

        Mame.InputPortTiny[] input_ports_frogger()
        {
            INPUT_PORTS_START("frogger");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN); /* 1P shoot2 - unused */
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN); /* 1P shoot1 - unused */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_DIPNAME(0x03, 0x00, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x02, "7");
            PORT_BITX(0, 0x03, (int)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "256", (ushort)Mame.InputCodes.CODE_NONE, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN); /* 2P shoot2 - unused */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN); /* 2P shoot1 - unused */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_START1);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_DIPNAME(0x06, 0x00, ipdn_defaultstrings["Coinage"]);
            PORT_DIPSETTING(0x02, "A 2/1 B 2/1 C 2/1");
            PORT_DIPSETTING(0x04, "A 2/1 B 1/3 C 2/1");
            PORT_DIPSETTING(0x00, "A 1/1 B 1/1 C 1/1");
            PORT_DIPSETTING(0x06, "A 1/1 B 1/6 C 1/1");
            PORT_DIPNAME(0x08, 0x00, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["Cocktail"]);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, IPT_UNUSED);

            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_frogger()
        {
            ROM_START("frogger");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("frogger.26", 0x0000, 0x1000, 0x597696d6);
            ROM_LOAD("frogger.27", 0x1000, 0x1000, 0xb6e6fcc3);
            ROM_LOAD("frsm3.7", 0x2000, 0x1000, 0xaca22ae0);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("frogger.608", 0x0000, 0x0800, 0xe8ab0256);
            ROM_LOAD("frogger.609", 0x0800, 0x0800, 0x7380a48f);
            ROM_LOAD("frogger.610", 0x1000, 0x0800, 0x31d7eb27);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("frogger.606", 0x0000, 0x0800, 0xf524ee30);
            ROM_LOAD("frogger.607", 0x0800, 0x0800, 0x05f7d883);

            ROM_REGION(0x0020, Mame.REGION_PROMS);
            ROM_LOAD("pr-91.6l", 0x0000, 0x0020, 0x413703bf);
            return ROM_END;
        }

        public override void driver_init()
        {
            /* the first ROM of the second CPU has data lines D0 and D1 swapped. Decode it. */
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU2);
            for (int A = 0; A < 0x0800; A++)
                RAM[A] = (byte)((RAM[A] & 0xfc) | ((RAM[A] & 1) << 1) | ((RAM[A] & 2) >> 1));

            /* likewise, the first gfx ROM has data lines D0 and D1 swapped. Decode it. */
            RAM = Mame.memory_region(Mame.REGION_GFX1);
            for (int A = 0; A < 0x0800; A++)
                RAM[A] = (byte)((RAM[A] & 0xfc) | ((RAM[A] & 1) << 1) | ((RAM[A] & 2) >> 1));

        }
        public driver_frogger()
        {
            drv = new machine_driver_frogger();
            year = "1981";
            name = "frogger";
            description = "Frogger";
            manufacturer = "Konami";
            flags = Mame.ROT90;
            input_ports = input_ports_frogger();

            rom = rom_frogger();
        }
    }
}
