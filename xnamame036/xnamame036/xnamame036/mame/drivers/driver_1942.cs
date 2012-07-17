using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public partial class driver_1942 : Mame.GameDriver
    {

        public static _BytePtr c1942_backgroundram = new _BytePtr();
        public static int[] c1942_backgroundram_size = new int[1];
        public static _BytePtr c1942_scroll = new _BytePtr();
        public static _BytePtr c1942_palette_bank = new _BytePtr();
        public static byte[] dirtybuffer2;
        public static Mame.osd_bitmap tmpbitmap2;
        public static bool flipscreen;

        Mame.InputPortTiny[] input_ports_1942()
        {
            INPUT_PORTS_START("1942");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);	/* probably unused */

            PORT_START("DSW0");
            PORT_DIPNAME(0x07, 0x07, ipdn_defaultstrings["Coin A"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x04, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0x06, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x05, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Free Play"]);
            PORT_DIPNAME(0x08, 0x00, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Upright"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["Cocktail"]);
            PORT_DIPNAME(0x30, 0x30, ipdn_defaultstrings["Bonus Life"]);
            PORT_DIPSETTING(0x30, "20000 80000");
            PORT_DIPSETTING(0x20, "20000 100000");
            PORT_DIPSETTING(0x10, "30000 80000");
            PORT_DIPSETTING(0x00, "30000 100000");
            PORT_DIPNAME(0xc0, 0xc0, ipdn_defaultstrings["Lives"]);
            PORT_DIPSETTING(0x80, "1");
            PORT_DIPSETTING(0x40, "2");
            PORT_DIPSETTING(0xc0, "3");
            PORT_DIPSETTING(0x00, "5");

            PORT_START("DSW1");
            PORT_DIPNAME(0x07, 0x07, ipdn_defaultstrings["Coin B"]);
            PORT_DIPSETTING(0x01, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x02, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x04, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x03, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0x06, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x05, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["Free Play"]);
            PORT_SERVICE(0x08, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x10, 0x10, ipdn_defaultstrings["Flip Screen"]);
            PORT_DIPSETTING(0x10, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_DIPNAME(0x60, 0x60, ipdn_defaultstrings["Difficulty"]);
            PORT_DIPSETTING(0x40, "Easy");
            PORT_DIPSETTING(0x60, "Normal");
            PORT_DIPSETTING(0x20, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x80, 0x80, "Freeze");
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_1942()
        {
            ROM_START("1942");
            ROM_REGION(0x1c000, Mame.REGION_CPU1);	/* 64k for code + 3*16k for the banked ROMs images */
            ROM_LOAD("1-n3a.bin", 0x00000, 0x4000, 0x40201bab);
            ROM_LOAD("1-n4.bin", 0x04000, 0x4000, 0xa60ac644);
            ROM_LOAD("1-n5.bin", 0x10000, 0x4000, 0x835f7b24);
            ROM_LOAD("1-n6.bin", 0x14000, 0x2000, 0x821c6481);
            ROM_LOAD("1-n7.bin", 0x18000, 0x4000, 0x5df525e1);

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for the audio CPU */
            ROM_LOAD("1-c11.bin", 0x0000, 0x4000, 0xbd87f06b);

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1-f2.bin", 0x0000, 0x2000, 0x6ebca191);	/* characters */

            ROM_REGION(0xc000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2-a1.bin", 0x0000, 0x2000, 0x3884d9eb);	/* tiles */
            ROM_LOAD("2-a2.bin", 0x2000, 0x2000, 0x999cf6e0);
            ROM_LOAD("2-a3.bin", 0x4000, 0x2000, 0x8edb273a);
            ROM_LOAD("2-a4.bin", 0x6000, 0x2000, 0x3a2726c3);
            ROM_LOAD("2-a5.bin", 0x8000, 0x2000, 0x1bd3d8bb);
            ROM_LOAD("2-a6.bin", 0xa000, 0x2000, 0x658f02c4);

            ROM_REGION(0x10000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2-l1.bin", 0x00000, 0x4000, 0x2528bec6);	/* sprites */
            ROM_LOAD("2-l2.bin", 0x04000, 0x4000, 0xf89287aa);
            ROM_LOAD("2-n1.bin", 0x08000, 0x4000, 0x024418f8);
            ROM_LOAD("2-n2.bin", 0x0c000, 0x4000, 0xe2c7e489);

            ROM_REGION(0x0600, Mame.REGION_PROMS);
            ROM_LOAD("08e_sb-5.bin", 0x0000, 0x0100, 0x93ab8153);	/* red component */
            ROM_LOAD("09e_sb-6.bin", 0x0100, 0x0100, 0x8ab44f7d);	/* green component */
            ROM_LOAD("10e_sb-7.bin", 0x0200, 0x0100, 0xf4ade9a4);	/* blue component */
            ROM_LOAD("f01_sb-0.bin", 0x0300, 0x0100, 0x6047d91b);	/* char lookup table */
            ROM_LOAD("06d_sb-4.bin", 0x0400, 0x0100, 0x4858968d);/* tile lookup table */
            ROM_LOAD("03k_sb-8.bin", 0x0500, 0x0100, 0xf6fad943);	/* sprite lookup table */
            return ROM_END;
        }
        static Mame.MemoryReadAddress[] readmem = 
        {
            new Mame.MemoryReadAddress( 0x0000, 0x7fff,  Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff,  Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc000, 0xc000,  Mame.input_port_0_r),	/* IN0 */
	new Mame.MemoryReadAddress( 0xc001, 0xc001,  Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0xc002, 0xc002,  Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress(0xc003, 0xc003,  Mame.input_port_3_r),	/* DSW0 */
	new Mame.MemoryReadAddress( 0xc004, 0xc004,  Mame.input_port_4_r ),	/* DSW1 */
	new Mame.MemoryReadAddress( 0xd000, 0xdbff,  Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff,  Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 ),	/* end of table */
        };
        static Mame.MemoryWriteAddress[] writemem = 
        {
        new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc800, 0xc800, Mame.soundlatch_w ),
	new Mame.MemoryWriteAddress( 0xc802, 0xc803, Mame.MWA_RAM, c1942_scroll ),
	new Mame.MemoryWriteAddress( 0xc804, 0xc804, c1942_flipscreen_w ),
	new Mame.MemoryWriteAddress( 0xc805, 0xc805, c1942_palette_bank_w, c1942_palette_bank ),
	new Mame.MemoryWriteAddress( 0xc806, 0xc806, c1942_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xcc00, 0xcc7f, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xd000, 0xd3ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xd400, 0xd7ff, Generic.colorram_w, Generic.colorram ),
	new Mame.MemoryWriteAddress( 0xd800, 0xdbff, c1942_background_w, c1942_backgroundram, c1942_backgroundram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.MemoryReadAddress[] sound_readmem = 
        {
        new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x47ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x6000, 0x6000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
        };
        static Mame.MemoryWriteAddress[] sound_writemem = 
        {
        new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x4000, 0x47ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8000, ay8910.AY8910_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8001, 0x8001, ay8910.AY8910_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc000, ay8910.AY8910_control_port_1_w ),
	new Mame.MemoryWriteAddress( 0xc001, 0xc001, ay8910.AY8910_write_port_1_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
        };
        static Mame.GfxLayout charlayout = new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
    512,	/* 512 characters */
    2,	/* 2 bits per pixel */
    new uint[] { 4, 0 },
    new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3 },
    new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
    16 * 8	/* every char takes 16 consecutive bytes */
    );
        static Mame.GfxLayout tilelayout = new Mame.GfxLayout(
            16, 16,	/* 16*16 tiles */
    512,	/* 512 tiles */
    3,	/* 3 bits per pixel */
    new uint[] { 0, 512 * 32 * 8, 2 * 512 * 32 * 8 },	/* the bitplanes are separated */
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 16 * 8 + 0, 16 * 8 + 1, 16 * 8 + 2, 16 * 8 + 3, 16 * 8 + 4, 16 * 8 + 5, 16 * 8 + 6, 16 * 8 + 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 8 * 8, 9 * 8, 10 * 8, 11 * 8, 12 * 8, 13 * 8, 14 * 8, 15 * 8 },
    32 * 8	/* every tile takes 32 consecutive bytes */
            );
        static Mame.GfxLayout spritelayout = new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
    512,	/* 512 sprites */
    4,	/* 4 bits per pixel */
    new uint[] { 512 * 64 * 8 + 4, 512 * 64 * 8 + 0, 4, 0 },
    new uint[] { 0, 1, 2, 3, 8 + 0, 8 + 1, 8 + 2, 8 + 3, 32 * 8 + 0, 32 * 8 + 1, 32 * 8 + 2, 32 * 8 + 3, 33 * 8 + 0, 33 * 8 + 1, 33 * 8 + 2, 33 * 8 + 3 },
    new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16, 8 * 16, 9 * 16, 10 * 16, 11 * 16, 12 * 16, 13 * 16, 14 * 16, 15 * 16 },
    64 * 8	/* every sprite takes 64 consecutive bytes */
            );
        static Mame.GfxDecodeInfo[] gfxdecodeinfo = {
    new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,             0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, tilelayout,          64*4, 4*32 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, spritelayout, 64*4+4*32*8, 16 ),
                                                    };

        public static void c1942_vh_convert_color_prom(byte[] palette, ushort[] colortable, _BytePtr color_prom)
        {

            uint cpi = 0;
            int pi = 0;
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

            cpi += 2 * Mame.Machine.drv.total_colors;
            /* color_prom now points to the beginning of the lookup table */


            /* characters use colors 128-143 */
            for (int i = 0; i < (Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity); i++)
                COLOR(colortable, 0, i, (ushort)(((color_prom[cpi++]) & 0x0f) + 128));

            /* background tiles use colors 0-63 in four banks */
            for (int i = 0; i < (Mame.Machine.gfx[1].total_colors * Mame.Machine.gfx[1].color_granularity) / 4; i++)
            {
                COLOR(colortable, 1, i, (ushort)(((color_prom[cpi]) & 0x0f)));
                COLOR(colortable, 1, i + 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 16));
                COLOR(colortable, 1, i + 2 * 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 32));
                COLOR(colortable, 1, i + 3 * 32 * 8, (ushort)(((color_prom[cpi]) & 0x0f) + 48));
                cpi++;
            }

            /* sprites use colors 64-79 */
            for (int i = 0; i < (Mame.Machine.gfx[2].total_colors * Mame.Machine.gfx[2].color_granularity); i++)
                COLOR(colortable, 2, i, (ushort)(((color_prom[cpi++]) & 0x0f) + 64));
        }
        public static int c1942_vh_start()
        {
            if (Generic.generic_vh_start() != 0)
                return 1;

            dirtybuffer2 = new byte[c1942_backgroundram_size[0]];
            for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                dirtybuffer2[i] = 1;

            /* the background area is twice as wide as the screen (actually twice as tall, */
            /* because this is a vertical game) */
            tmpbitmap2 = Mame.osd_create_bitmap(2 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

            return 0;
        }

        public static void c1942_vh_stop()
        {
            Mame.osd_free_bitmap(tmpbitmap2);
            dirtybuffer2 = null;
            Generic.generic_vh_stop();
        }
        public static void c1942_background_w(int offset, int data)
        {
            if (c1942_backgroundram[offset] != data)
            {
                dirtybuffer2[offset] = 1;

                c1942_backgroundram[offset] = (byte)data;
            }
        }
        public static void c1942_palette_bank_w(int offset, int data)
        {
            if (c1942_palette_bank[0] != data)
            {
                for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                    dirtybuffer2[i] = 1;
                c1942_palette_bank[0] = (byte)data;
            }
        }
        public static void c1942_flipscreen_w(int offset, int data)
        {
            if (flipscreen != ((data & 0x80) != 0))
            {
                flipscreen = (data & 0x80) != 0;
                for (int i = 0; i < c1942_backgroundram_size[0]; i++)
                    dirtybuffer2[i] = 1;
            }
        }

        static void c1942_bankswitch_w(int offset, int data)
        {
            int bankaddress;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            bankaddress = 0x10000 + (data & 0x03) * 0x4000;
            Mame.cpu_setbank(1, new _BytePtr(RAM, bankaddress));
        }

        public static int c1942_interrupt()
        {
            if (Mame.cpu_getiloops() != 0) return 0x00cf;	/* RST 08h */
            else return 0x00d7;	/* RST 10h - vblank */
        }
        static AY8910interface ay8910_interface = new AY8910interface(2, 1500000, new int[] { 25, 25 }, new AY8910portRead[] { null, null }, new AY8910portRead[] { null, null }, new AY8910portWrite[] { null, null }, new AY8910portWrite[] { null, null }, null);
        class machine_driver_1942 : Mame.MachineDriver
        {
            public machine_driver_1942()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, readmem, writemem, null, null, c1942_interrupt, 2));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3000000, sound_readmem, sound_writemem, null, null, Mame.interrupt, 4));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;

                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0* 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_1942.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 64 * 4 + 4 * 32 * 8 + 16 * 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, ay8910_interface));
            }
            public override void init_machine()
            {
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                c1942_vh_convert_color_prom(palette, colortable, color_prom);
            }
            public override int vh_start()
            {
                return c1942_vh_start();
            }
            public override void vh_stop()
            {
                c1942_vh_stop();
            }
            public override void vh_eof_callback()
            {
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {

                for (int offs = c1942_backgroundram_size[0] - 1; offs >= 0; offs--)
                {
                    if ((offs & 0x10) == 0 && (dirtybuffer2[offs] != 0 || dirtybuffer2[offs + 16] != 0))
                    {
                        dirtybuffer2[offs] = dirtybuffer2[offs + 16] = 0;

                        int sx = offs / 32;
                        int sy = offs % 32;
                        bool flipx = (c1942_backgroundram[offs + 16] & 0x20) != 0;
                        bool flipy = (c1942_backgroundram[offs + 16] & 0x40) != 0;
                        if (flipscreen)
                        {
                            sx = 31 - sx;
                            sy = 15 - sy;
                            flipx = !flipx;
                            flipy = !flipy;
                        }

                        Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[1],
                                (uint)(c1942_backgroundram[offs] + 2 * (c1942_backgroundram[offs + 16] & 0x80)),
                                (uint)((c1942_backgroundram[offs + 16] & 0x1f) + 32 * c1942_palette_bank[0]),
                                flipx, flipy,
                                16 * sx, 16 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the background graphics */
                {
                    int scroll = -(c1942_scroll[0] + 256 * c1942_scroll[1]);
                    if (flipscreen) scroll = 256 - scroll;

                    Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, new int[] { scroll }, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* Draw the sprites. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int i, code, col, sx, sy, dir;


                    code = (Generic.spriteram[offs] & 0x7f) + 4 * (Generic.spriteram[offs + 1] & 0x20)
                            + 2 * (Generic.spriteram[offs] & 0x80);
                    col = Generic.spriteram[offs + 1] & 0x0f;
                    sx = Generic.spriteram[offs + 3] - 0x10 * (Generic.spriteram[offs + 1] & 0x10);
                    sy = Generic.spriteram[offs + 2];
                    dir = 1;
                    if (flipscreen)
                    {
                        sx = 240 - sx;
                        sy = 240 - sy;
                        dir = -1;
                    }

                    /* handle double / quadruple height (actually width because this is a rotated game) */
                    i = (Generic.spriteram[offs + 1] & 0xc0) >> 6;
                    if (i == 2) i = 3;

                    do
                    {
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                                (uint)(code + i), (uint)col,
                                flipscreen, flipscreen,
                                sx, sy + 16 * i * dir,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);

                        i--;
                    } while (i >= 0);
                }


                /* draw the frontmost playfield. They are characters, but draw them as sprites */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.videoram[offs] != 0x30)	/* don't draw spaces */
                    {
                        int sx = offs % 32;
                        int sy = offs / 32;
                        if (flipscreen)
                        {
                            sx = 31 - sx;
                            sy = 31 - sy;
                        }

                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                (uint)(Generic.videoram[offs] + 2 * (Generic.colorram[offs] & 0x80)),
                                (uint)(Generic.colorram[offs] & 0x3f),
                                flipscreen, flipscreen,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    }
                }
            }
        }

        public override void driver_init()
        {
            //throw new NotImplementedException();
        }
        public driver_1942()
        {
            drv = new machine_driver_1942();
            year = "1984";
            name = "1942";
            description = "1942 (set 1)";
            manufacturer = "Capcom";
            flags = Mame.ROT270;
            input_ports = input_ports_1942();
            rom = rom_1942();
            drv.HasNVRAMhandler = false;
        }
    }
}
