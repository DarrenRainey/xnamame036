using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_terracre : Mame.GameDriver
    {

        static _BytePtr terrac_ram = new _BytePtr(1);
        static _BytePtr terrac_videoram = new _BytePtr(1);
        static int[] terrac_videoram_size = new int[1];
        static _BytePtr terrac_scrolly = new _BytePtr(2);
        static Mame.osd_bitmap tmpbitmap2;
        static byte[] dirtybuffer2;
        static _BytePtr spritepalettebank;

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x000000, 0x01ffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x020000, 0x0201ff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0x020200, 0x021fff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0x022000, 0x022fff, terrac_videoram2_r ),
	new Mame.MemoryReadAddress( 0x023000, 0x023fff, Mame.MRA_BANK3 ),
	new Mame.MemoryReadAddress( 0x024000, 0x024007, terracre_r_read ),
	new Mame.MemoryReadAddress( 0x028000, 0x0287ff, Mame.MRA_BANK4 ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x01ffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x020000, 0x0201ff, Mame.MWA_BANK1, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x020200, 0x021fff, Mame.MWA_BANK2, terrac_ram ),
	new Mame.MemoryWriteAddress( 0x022000, 0x022fff, terrac_videoram2_w, terrac_videoram, terrac_videoram_size ),
	new Mame.MemoryWriteAddress( 0x023000, 0x023fff, Mame.MWA_BANK3 ),
	new Mame.MemoryWriteAddress( 0x026000, 0x02600f, terracre_r_write ),
	new Mame.MemoryWriteAddress( 0x028000, 0x0287ff, Mame.MWA_BANK4, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc000, 0xcfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xcfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};


        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x04, 0x04, soundlatch_clear ),
	new Mame.IOReadPort( 0x06, 0x06, Mame.soundlatch_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] sound_writeport_3526 =
{
	new Mame.IOWritePort( 0x00, 0x00, YM3526.YM3526_control_port_0_w ),
	new Mame.IOWritePort( 0x01, 0x01, YM3526.YM3526_write_port_0_w ),
	new Mame.IOWritePort( 0x02, 0x03, DAC.DAC_signed_data_w ),	/* 2 channels */
	new Mame.IOWritePort( -1 )	/* end of table */
};

        //static Mame.IOWritePort[] sound_writeport_2203 =
        //{
        //    new Mame.IOWritePort( 0x00, 0x00, YM2203_control_port_0_w ),
        //    new Mame.IOWritePort( 0x01, 0x01, YM2203_write_port_0_w ),
        //    new Mame.IOWritePort( 0x02, 0x03, DAC_signed_data_w ),	/* 2 channels */
        //    new Mame.IOWritePort( -1 )	/* end of table */
        //};


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[] { 4, 0, 12, 8, 20, 16, 28, 24 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8	/* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout backlayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 chars */
            512,	/* 512 characters */
            4,		/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 }, /* plane offset */
            new uint[]{ 4, 0, 12, 8, 20, 16, 28, 24,
		32+4, 32+0, 32+12, 32+8, 32+20, 32+16, 32+28, 32+24, },
            new uint[]{ 0*64, 1*64, 2*64, 3*64, 4*64, 5*64, 6*64, 7*64,
		8*64, 9*64, 10*64, 11*64, 12*64, 13*64, 14*64, 15*64 },
            128 * 8   /* every char takes 128 consecutive bytes  */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 characters */
            512,	/* 512 sprites */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[]{ 4, 0, 4+0x8000*8, 0+0x8000*8, 12, 8, 12+0x8000*8, 8+0x8000*8,
		20, 16, 20+0x8000*8, 16+0x8000*8, 28, 24, 28+0x8000*8, 24+0x8000*8 },
            new uint[]{ 0*32, 1*32, 2*32, 3*32, 4*32, 5*32, 6*32, 7*32,
          8*32, 9*32, 10*32, 11*32, 12*32, 13*32, 14*32, 15*32 },
            64 * 8	/* every char takes 64 consecutive bytes  */
        );
        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout,            0,   1 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX2, 0, backlayout,         1*16,  16 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0, spritelayout, 1*16+16*16, 256 ),
};
        static YM3526interface ym3526_interface = new YM3526interface(1, 4000000, new int[] { 255 });
        static Mame.DACinterface dac_interface = new Mame.DACinterface(2, new int[] { 50, 50 });

        static int soundlatch_clear(int offset)
        {
            Mame.soundlatch_clear_w(0, 0);
            return 0;
        }
        static int terracre_r_read(int offset)
        {
            switch (offset)
            {
                case 0: /* Player controls */
                    return Mame.readinputport(0);

                case 2: /* Dipswitch 0xf000*/
                    return Mame.readinputport(1);

                case 4: /* Start buttons & Dipswitch */
                    return Mame.readinputport(2) << 8;

                case 6: /* Dipswitch???? */
                    return (Mame.readinputport(4) << 8) | Mame.readinputport(3);
            }
            return 0xffff;
        }
        static void terracre_r_write(int offset, int data)
        {
            switch (offset)
            {
                //		case 0: /* ??? */
                //			break;
                case 2: /* Scroll Y */
                    Mame.COMBINE_WORD_MEM(terrac_scrolly, 0, data);
                    return;
                    break;
                //		case 4: /* ??? */
                //			break;
                //		case 0xa: /* ??? */
                //			break;
                case 0xc: /* sound command */
                    Mame.soundlatch_w(offset, ((data & 0x7f) << 1) | 1);
                    return;
                    break;
                //		case 0xe: /* ??? */
                //			break;
            }

            //if (errorlog) fprintf(errorlog, "OUTPUT [%x] <- %08x\n", offset, data);
        }
        static void terrac_videoram2_w(int offset, int data)
        {
            int oldword = terrac_videoram.READ_WORD(offset);
            int newword = Mame.COMBINE_WORD(oldword, data);

            if (oldword != newword)
            {
                terrac_videoram.WRITE_WORD(offset, (ushort)newword);
                dirtybuffer2[offset] = 1;
                dirtybuffer2[offset + 1] = 1;
            }
        }

        static int terrac_videoram2_r(int offset)
        {
            return terrac_videoram.READ_WORD(offset);
        }
        class machine_driver_terracre : Mame.MachineDriver
        {
            public machine_driver_terracre()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 8000000, readmem, writemem, null, null, Mame.m68_level1_irq, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 4000000, sound_readmem, sound_writemem, sound_readport, sound_writeport_3526, Mame.interrupt, 128));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_terracre.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 1 * 16 + 16 * 16 + 16 * 256;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3526, ym3526_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                uint pi = 0, cpi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0, bit1, bit2, bit3;

                    bit0 = (color_prom[cpi] >> 0) & 0x01;
                    bit1 = (color_prom[cpi] >> 1) & 0x01;
                    bit2 = (color_prom[cpi] >> 2) & 0x01;
                    bit3 = (color_prom[cpi] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                    bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                    palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);

                    cpi++;
                }

                cpi += 2 * Mame.Machine.drv.total_colors;
                /* color_prom now points to the beginning of the lookup table */


                /* characters use colors 0-15 */
                for (int i = 0; i < TOTAL_COLORS(0); i++)
                    COLOR(colortable, 0, i, i);

                /* background tiles use colors 192-255 in four banks */
                /* the bottom two bits of the color code select the palette bank for */
                /* pens 0-7; the top two bits for pens 8-15. */
                for (int i = 0; i < TOTAL_COLORS(1); i++)
                {
                    if ((i & 8) != 0)
                        COLOR(colortable, 1, i, 192 + (i & 0x0f) + ((i & 0xc0) >> 2));
                    else COLOR(colortable, 1, i, 192 + (i & 0x0f) + ((i & 0x30) >> 0));
                }

                /* sprites use colors 128-191 in four banks */
                /* The lookup table tells which colors to pick from the selected bank */
                /* the bank is selected by another PROM and depends on the top 8 bits of */
                /* the sprite code. The PROM selects the bank *separately* for pens 0-7 and */
                /* 8-15 (like for tiles). */
                for (int i = 0; i < TOTAL_COLORS(2) / 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if ((i & 8) != 0)
                            COLOR(colortable, 2, i + j * (TOTAL_COLORS(2) / 16), 128 + ((j & 0x0c) << 2) + (color_prom[cpi] & 0x0f));
                        else
                            COLOR(colortable, 2, i + j * (TOTAL_COLORS(2) / 16), 128 + ((j & 0x03) << 4) + (color_prom[cpi] & 0x0f));
                    }

                    cpi++;
                }

                /* color_prom now points to the beginning of the sprite palette bank table */
                spritepalettebank = new _BytePtr(color_prom, (int)cpi);	/* we'll need it at run time */
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0) return 1;
                dirtybuffer2 = new byte[terrac_videoram_size[0]];
                for (int i = 0; i < terrac_videoram_size[0]; i++)
                    dirtybuffer2[i] = 1;
                tmpbitmap2 = Mame.osd_new_bitmap(4 * Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);
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
                int offs, x, y;


                for (y = 0; y < 64; y++)
                {
                    for (x = 0; x < 16; x++)
                    {
                        if ((dirtybuffer2[x * 2 + y * 64]) != 0 || (dirtybuffer2[x * 2 + y * 64 + 1] != 0))
                        {
                            int code = terrac_videoram.READ_WORD(x * 2 + y * 64) & 0x01ff;
                            int color = (terrac_videoram.READ_WORD(x * 2 + y * 64) & 0x7800) >> 11;

                            dirtybuffer2[x * 2 + y * 64] = dirtybuffer2[x * 2 + y * 64 + 1] = 0;

                            Mame.drawgfx(tmpbitmap2, Mame.Machine.gfx[1],
                                    (uint)code,
                                    (uint)color,
                                    false, false,
                                    16 * y, 16 * x,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }

                /* copy the background graphics */
                if ((terrac_scrolly.READ_WORD(0) & 0x2000) != 0)	/* background disable */
                    Mame.fillbitmap(bitmap, Mame.Machine.pens[0], Mame.Machine.drv.visible_area);
                else
                {
                    int scrollx = -terrac_scrolly.READ_WORD(0);

                    Mame.copyscrollbitmap(bitmap, tmpbitmap2, 1, new int[] { scrollx }, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }



                for (x = 0; x < Generic.spriteram_size[0]; x += 8)
                {
                    int code;
                    int attr = Generic.spriteram.READ_WORD(x + 4) & 0xff;
                    int color = (attr & 0xf0) >> 4;
                    int flipx = attr & 0x04;
                    int flipy = attr & 0x08;
                    int sx, sy;

                    sx = (Generic.spriteram.READ_WORD(x + 6) & 0xff) - 0x80 + 256 * (attr & 1);
                    sy = 240 - (Generic.spriteram.READ_WORD(x) & 0xff);

                    code = (Generic.spriteram.READ_WORD(x + 2) & 0xff) + ((attr & 0x02) << 7);

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2],
                            (uint)code,
                            (uint)(color + 16 * (spritepalettebank[code >> 1] & 0x0f)),
                            flipx != 0, flipy != 0,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }


                for (offs = Generic.videoram_size[0] - 2; offs >= 0; offs -= 2)
                {
                    int sx, sy;


                    sx = (offs / 2) / 32;
                    sy = (offs / 2) % 32;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            (uint)Generic.videoram.READ_WORD(offs) & 0xff,
                            0,
                            false, false,
                            8 * sx, 8 * sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 15);
                }
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            //nothing
        }
        Mame.RomModule[] rom_terracre()
        {
            ROM_START("terracre");
            ROM_REGION(0x20000, Mame.REGION_CPU1);	/* 128K for 68000 code */
            ROM_LOAD_ODD("1a_4b.rom", 0x00000, 0x4000, 0x76f17479);
            ROM_LOAD_EVEN("1a_4d.rom", 0x00000, 0x4000, 0x8119f06e);
            ROM_LOAD_ODD("1a_6b.rom", 0x08000, 0x4000, 0xba4b5822);
            ROM_LOAD_EVEN("1a_6d.rom", 0x08000, 0x4000, 0xca4852f6);
            ROM_LOAD_ODD("1a_7b.rom", 0x10000, 0x4000, 0xd0771bba);
            ROM_LOAD_EVEN("1a_7d.rom", 0x10000, 0x4000, 0x029d59d9);
            ROM_LOAD_ODD("1a_9b.rom", 0x18000, 0x4000, 0x69227b56);
            ROM_LOAD_EVEN("1a_9d.rom", 0x18000, 0x4000, 0x5a672942);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for sound cpu */
            ROM_LOAD("2a_15b.rom", 0x0000, 0x4000, 0x604c3b11);
            ROM_LOAD("2a_17b.rom", 0x4000, 0x4000, 0xaffc898d);
            ROM_LOAD("2a_18b.rom", 0x8000, 0x4000, 0x302dc0ab);

            ROM_REGION(0x02000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2a_16b.rom", 0x00000, 0x2000, 0x591a3804); /* tiles */

            ROM_REGION(0x10000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1a_15f.rom", 0x00000, 0x8000, 0x984a597f); /* Background */
            ROM_LOAD("1a_17f.rom", 0x08000, 0x8000, 0x30e297ff);

            ROM_REGION(0x10000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("2a_6e.rom", 0x00000, 0x4000, 0xbcf7740b); /* Sprites */
            ROM_LOAD("2a_7e.rom", 0x04000, 0x4000, 0xa70b565c);
            ROM_LOAD("2a_6g.rom", 0x08000, 0x4000, 0x4a9ec3e6);
            ROM_LOAD("2a_7g.rom", 0x0c000, 0x4000, 0x450749fc);

            ROM_REGION(0x0500, Mame.REGION_PROMS);
            ROM_LOAD("tc1a_10f.bin", 0x0000, 0x0100, 0xce07c544);	/* red component */
            ROM_LOAD("tc1a_11f.bin", 0x0100, 0x0100, 0x566d323a);/* green component */
            ROM_LOAD("tc1a_12f.bin", 0x0200, 0x0100, 0x7ea63946);	/* blue component */
            ROM_LOAD("tc2a_2g.bin", 0x0300, 0x0100, 0x08609bad);	/* sprite lookup table */
            ROM_LOAD("tc2a_4e.bin", 0x0400, 0x0100, 0x2c43991f);	/* sprite palette bank */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_terracre()
        {


            INPUT_PORTS_START("terracre");
            PORT_START();	/* Player 1 controls */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* Player 2 controls */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* Coin, Start, Test, Dipswitch */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_SERVICE(0x20, IP_ACTIVE_LOW);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();	/* Dipswitch */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Lives));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "6");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x0c, "20000 60000");
            PORT_DIPSETTING(0x08, "30000 70000");
            PORT_DIPSETTING(0x04, "40000 80000");
            PORT_DIPSETTING(0x00, "50000 90000");
            PORT_DIPNAME(0x10, 0x10, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x10, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x20, DEF_STR(Cocktail));
            PORT_DIPNAME(0x40, 0x40, DEF_STR(Unknown));
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();	/* Dipswitch */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x00, DEF_STR(Free_Play));
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x00, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_6C"));
            PORT_DIPNAME(0x10, 0x10, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x10, "Easy");
            PORT_DIPSETTING(0x00, "Hard");
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Unknown));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BITX(0x40, 0x40, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            return INPUT_PORTS_END;
        }
        public driver_terracre()
        {
            drv = new machine_driver_terracre();
            year = "1985";
            name = "terracre";
            description = "Terra Cresta (YM3526 set 1)";
            manufacturer = "Nichibutsu";
            flags = Mame.ROT270;
            input_ports = input_ports_terracre();
            rom = rom_terracre();
            drv.HasNVRAMhandler = false;
        }
    }
}
