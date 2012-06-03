using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class konamiic
    {
        const byte MAX_K007121 = 2;
        const byte MAX_K051316 = 3;

        public delegate void _K051960_callback(ref int code, ref int color, ref int priority);
        static int K051960_memory_region;
        static Mame.GfxElement K051960_gfx;
        static _K051960_callback K051960_callback;
        static int K051960_romoffset;
        static int K051960_spriteflip, K051960_readroms;
        static byte[] K051960_spriterombank = new byte[3];
        static _BytePtr K051960_ram = new _BytePtr(1);
        static int K051960_irq_enabled, K051960_nmi_enabled;

        static byte[][] K007121_ctrlram = new byte[MAX_K007121][];
        static int[] K007121_flipscreen = new int[MAX_K007121];


        static int[] K051316_memory_region = new int[MAX_K051316];
        static int[] K051316_gfxnum = new int[MAX_K051316];
        static int[] K051316_wraparound = new int[MAX_K051316];
        static int[][] K051316_offset = new int[MAX_K051316][];
        static int[] K051316_bpp = new int[MAX_K051316];
        public delegate void _K051316_callback(ref int code, ref int color);
        static _K051316_callback[] K051316_callback = new _K051316_callback[MAX_K051316];
        static _BytePtr[] K051316_ram = new _BytePtr[MAX_K051316];
        static _BytePtr[] K051316_ctrlram = new _BytePtr[MAX_K051316];
        static Mame.tilemap[] K051316_tilemap = new Mame.tilemap[MAX_K051316];
        static int K051316_chip_selected;

        static konamiic()
        {
            for (int i = 0; i < MAX_K007121; i++)
                K007121_ctrlram[i] = new byte[8];
            for (int i = 0; i < MAX_K051316; i++)
                K051316_offset[i] = new int[2];
            for (int i = 0; i < MAX_K051316; i++)
                K051316_ctrlram[i] = new _BytePtr(16);
        }



        public static int K051960_fetchromdata(int _byte)
        {
            int code, color, pri, off1, addr;

            addr = K051960_romoffset + (K051960_spriterombank[0] << 8) +
                    ((K051960_spriterombank[1] & 0x03) << 16);
            code = (addr & 0x3ffe0) >> 5;
            off1 = addr & 0x1f;
            color = ((K051960_spriterombank[1] & 0xfc) >> 2) + ((K051960_spriterombank[2] & 0x03) << 6);
            pri = 0;
            K051960_callback(ref code, ref color, ref pri);

            addr = (code << 7) | (off1 << 2) | _byte;
            addr &= Mame.memory_region_length(K051960_memory_region) - 1;

#if false
	usrintf_showmessage("%04x: addr %06x",cpu_get_pc(),addr);
#endif

            return Mame.memory_region(K051960_memory_region)[addr];
        }

        static int counter;
        public static int K051937_r(int offset)
        {
            if (K051960_readroms != 0 && offset >= 4 && offset < 8)
            {
                return K051960_fetchromdata(offset & 3);
            }
            else
            {
                if (offset == 0)
                {

                    /* some games need bit 0 to pulse */
                    return (counter++) & 1;
                }
                Mame.printf("%04x: read unknown 051937 address %x\n", Mame.cpu_get_pc(), offset);
                return 0;
            }
        }
        public static void K051937_w(int offset, int data)
        {
            if (offset == 0)
            {
#if MAME_DEBUG
if (data & 0xc6)
	usrintf_showmessage("051937 reg 00 = %02x",data);
#endif
                /* bit 0 is IRQ enable */
                K051960_irq_enabled = (data & 0x01);

                /* bit 1: probably FIRQ enable */

                /* bit 2 is NMI enable */
                K051960_nmi_enabled = (data & 0x04);

                /* bit 3 = flip screen */
                K051960_spriteflip = data & 0x08;

                /* bit 4 used by Devastators and TMNT, unknown */

                /* bit 5 = enable gfx ROM reading */
                K051960_readroms = data & 0x20;
#if VERBOSE
if (errorlog) fprintf(errorlog,"%04x: write %02x to 051937 address %x\n",cpu_get_pc(),data,offset);
#endif
            }
            else if (offset >= 2 && offset < 5)
            {
                K051960_spriterombank[offset - 2] = (byte)data;
            }
            else
            {
#if false
	usrintf_showmessage("%04x: write %02x to 051937 address %x",cpu_get_pc(),data,offset);
#endif
                Mame.printf("%04x: write %02x to unknown 051937 address %x\n", Mame.cpu_get_pc(), data, offset);
            }
        }

        public static int K051960_r(int offset)
        {
            if (K051960_readroms != 0)
            {
                /* the 051960 remembers the last address read and uses it when reading the sprite ROMs */
                K051960_romoffset = (offset & 0x3fc) >> 2;
                return K051960_fetchromdata(offset & 3);	/* only 88 Games reads the ROMs from here */
            }
            else
                return K051960_ram[offset];
        }
        public static void K051960_w(int offset, int data)
        {
            K051960_ram[offset] = (byte)data;
        }
        public static int K051316_r(int chip, int offset)
        {
            return K051316_ram[chip][offset];
        }
        public static int K051316_0_r(int offset)
        {
            return K051316_r(0, offset);
        }
        public static int K051316_rom_r(int chip, int offset)
        {
            if ((K051316_ctrlram[chip][0x0e] & 0x01) == 0)
            {
                int addr;

                addr = offset + (K051316_ctrlram[chip][0x0c] << 11) + (K051316_ctrlram[chip][0x0d] << 19);
                if (K051316_bpp[chip] <= 4) addr /= 2;
                addr &= Mame.memory_region_length(K051316_memory_region[chip]) - 1;

#if false
	usrintf_showmessage("%04x: offset %04x addr %04x",cpu_get_pc(),offset,addr);
#endif

                return Mame.memory_region(K051316_memory_region[chip])[addr];
            }
            else
            {
                Mame.printf("%04x: read 051316 ROM offset %04x but reg 0x0c bit 0 not clear\n", Mame.cpu_get_pc(), offset);
                return 0;
            }
        }

        public static int K051316_rom_0_r(int offset)
        {
            return K051316_rom_r(0, offset);
        }
        public static void K051316_w(int chip, int offset, int data)
        {
            if (K051316_ram[chip][offset] != data)
            {
                K051316_ram[chip][offset] = (byte)data;
                Mame.tilemap_mark_tile_dirty(K051316_tilemap[chip], offset % 32, (offset % 0x400) / 32);
            }
        }

        public static void K051316_0_w(int offset, int data)
        {
            K051316_w(0, offset, data);
        }
        public static void K051316_ctrl_w(int chip, int offset, int data)
        {
            K051316_ctrlram[chip][offset] = (byte)data;
            Mame.printf("%04x: write %02x to 051316 reg %x\n", Mame.cpu_get_pc(), data, offset);
        }

        public static void K051316_ctrl_0_w(int offset, int data)
        {
            K051316_ctrl_w(0, offset, data);
        }

        static int K052109_memory_region;
        static int K052109_gfxnum;
        public delegate void _K052109_callback(int tilemap, int bank, ref int code, ref int color);
        static _K052109_callback K052109_callback;
        static _BytePtr K052109_ram;
        static _BytePtr K052109_videoram_F, K052109_videoram2_F, K052109_colorram_F;
        static _BytePtr K052109_videoram_A, K052109_videoram2_A, K052109_colorram_A;
        static _BytePtr K052109_videoram_B, K052109_videoram2_B, K052109_colorram_B;
        static byte[] K052109_charrombank = new byte[4];
        static int has_extra_video_ram;
        static int K052109_RMRD_line;
        static int K052109_tileflip_enable;
        static int K052109_irq_enabled;
        static byte K052109_romsubbank, K052109_scrollctrl;
        static Mame.tilemap[] K052109_tilemap = new Mame.tilemap[3];

        public static int[] NORMAL_PLANE_ORDER = { 0, 1, 2, 3 };
        public static int[] REVERSE_PLANE_ORDER = { 3, 2, 1, 0 };

        static _BytePtr colorram, videoram1, videoram2;
        static int layer;

        static void K052109_get_tile_info(int col, int row)
        {
            int flipy = 0;
            int tile_index = 64 * row + col;
            int code = videoram1[tile_index] + 256 * videoram2[tile_index];
            int color = colorram[tile_index];
            int bank = K052109_charrombank[(color & 0x0c) >> 2];
            if (has_extra_video_ram != 0) bank = (color & 0x0c) >> 2;	/* kludge for X-Men */
            color = (color & 0xf3) | ((bank & 0x03) << 2);
            bank >>= 2;

            flipy = color & 0x02;

            Mame.tile_info.flags = 0;

            K052109_callback(layer, bank, ref code, ref color);

            Mame.SET_TILE_INFO(K052109_gfxnum, code, color);

            /* if the callback set flip X but it is not enabled, turn it off */
            if ((K052109_tileflip_enable & 1) == 0) Mame.tile_info.flags &= unchecked((byte)~Mame.TILE_FLIPX);

            /* if flip Y is enabled and the attribute but is set, turn it on */
            if (flipy != 0 && (K052109_tileflip_enable & 2) != 0) Mame.tile_info.flags |= Mame.TILE_FLIPY;
        }
        public static int K052109_vh_start(int gfx_memory_region, int[] planes, _K052109_callback callback)
        {
            int gfx_index;
            Mame.GfxLayout charlayout =
            new Mame.GfxLayout(
                8, 8,
                0,				/* filled in later */
                4,
                new uint[] { 0, 0, 0, 0 },	/* filled in later */
                new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
                32 * 8
            );


            /* find first empty slot to decode gfx */
            for (gfx_index = 0; gfx_index < Mame.MAX_GFX_ELEMENTS; gfx_index++)
                if (Mame.Machine.gfx[gfx_index] == null)
                    break;
            if (gfx_index == Mame.MAX_GFX_ELEMENTS)
                return 1;

            /* tweak the structure for the number of tiles we have */
            charlayout.total = (uint)Mame.memory_region_length(gfx_memory_region) / 32;
            charlayout.planeoffset[0] = (uint)planes[3] * 8;
            charlayout.planeoffset[1] = (uint)planes[2] * 8;
            charlayout.planeoffset[2] = (uint)planes[1] * 8;
            charlayout.planeoffset[3] = (uint)planes[0] * 8;

            /* decode the graphics */
            Mame.Machine.gfx[gfx_index] = Mame.decodegfx(Mame.memory_region(gfx_memory_region), charlayout);
            if (Mame.Machine.gfx[gfx_index] == null)
                return 1;

            /* set the color information */
            Mame.Machine.gfx[gfx_index].colortable = Mame.Machine.remapped_colortable;
            Mame.Machine.gfx[gfx_index].total_colors = (int)Mame.Machine.drv.color_table_len / 16;

            K052109_memory_region = gfx_memory_region;
            K052109_gfxnum = gfx_index;
            K052109_callback = callback;
            K052109_RMRD_line = Mame.CLEAR_LINE;

            has_extra_video_ram = 0;

            K052109_tilemap[0] = Mame.tilemap_create(K052109_get_tile_info, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 32);
            K052109_tilemap[1] = Mame.tilemap_create(K052109_get_tile_info, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 32);
            K052109_tilemap[2] = Mame.tilemap_create(K052109_get_tile_info, Mame.TILEMAP_TRANSPARENT, 8, 8, 64, 32);

            K052109_ram = new _BytePtr(0x6000);

            if (K052109_ram == null || K052109_tilemap[0] == null || K052109_tilemap[1] == null || K052109_tilemap[2] == null)
            {
                K052109_vh_stop();
                return 1;
            }
            for (int i = 0; i < 0x6000; i++) K052109_ram[i] = 0;//memset(K052109_ram,0,0x6000);

            K052109_colorram_F = new _BytePtr(K052109_ram, 0x0000);
            K052109_colorram_A = new _BytePtr(K052109_ram, 0x0800);
            K052109_colorram_B = new _BytePtr(K052109_ram, 0x1000);
            K052109_videoram_F = new _BytePtr(K052109_ram, 0x2000);
            K052109_videoram_A = new _BytePtr(K052109_ram, 0x2800);
            K052109_videoram_B = new _BytePtr(K052109_ram, 0x3000);
            K052109_videoram2_F = new _BytePtr(K052109_ram, 0x4000);
            K052109_videoram2_A = new _BytePtr(K052109_ram, 0x4800);
            K052109_videoram2_B = new _BytePtr(K052109_ram, 0x5000);

            K052109_tilemap[0].transparent_pen = 0;
            K052109_tilemap[1].transparent_pen = 0;
            K052109_tilemap[2].transparent_pen = 0;

            return 0;
        }
        public static void K052109_vh_stop()
        {
            K052109_ram = null;
        }
        public static int K051960_vh_start(int gfx_memory_region, int[] planes, _K051960_callback callback)
        {
            int gfx_index;
            Mame.GfxLayout spritelayout =
            new Mame.GfxLayout(
                16, 16,
                0,				/* filled in later */
                4,
                new uint[] { 0, 0, 0, 0 },	/* filled in later */
                new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 * 32 + 0, 8 * 32 + 1, 8 * 32 + 2, 8 * 32 + 3, 8 * 32 + 4, 8 * 32 + 5, 8 * 32 + 6, 8 * 32 + 7 },
                new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32, 16 * 32, 17 * 32, 18 * 32, 19 * 32, 20 * 32, 21 * 32, 22 * 32, 23 * 32 },
                128 * 8
            );


            /* find first empty slot to decode gfx */
            for (gfx_index = 0; gfx_index < Mame.MAX_GFX_ELEMENTS; gfx_index++)
                if (Mame.Machine.gfx[gfx_index] == null)
                    break;
            if (gfx_index == Mame.MAX_GFX_ELEMENTS)
                return 1;

            /* tweak the structure for the number of tiles we have */
            spritelayout.total = (uint)Mame.memory_region_length(gfx_memory_region) / 128;
            spritelayout.planeoffset[0] = (uint)planes[0] * 8;
            spritelayout.planeoffset[1] = (uint)planes[1] * 8;
            spritelayout.planeoffset[2] = (uint)planes[2] * 8;
            spritelayout.planeoffset[3] = (uint)planes[3] * 8;

            /* decode the graphics */
            Mame.Machine.gfx[gfx_index] = Mame.decodegfx(Mame.memory_region(gfx_memory_region), spritelayout);
            if (Mame.Machine.gfx[gfx_index] == null)
                return 1;

            /* set the color information */
            Mame.Machine.gfx[gfx_index].colortable = Mame.Machine.remapped_colortable;
            Mame.Machine.gfx[gfx_index].total_colors = (int)Mame.Machine.drv.color_table_len / 16;

            K051960_memory_region = gfx_memory_region;
            K051960_gfx = Mame.Machine.gfx[gfx_index];
            K051960_callback = callback;
            K051960_ram = new _BytePtr(0x400);
            if (K051960_ram == null) return 1;

            for (int i = 0; i < 0x400; i++) K051960_ram[i] = 0;//memset(K051960_ram,0,0x400);

            return 0;
        }
        public static void K051960_vh_stop()
        {
            K051960_ram = null;
        }
        public static int K051316_vh_start(int chip, int gfx_memory_region, int bpp, _K051316_callback callback)
        {
            int gfx_index;


            /* find first empty slot to decode gfx */
            for (gfx_index = 0; gfx_index < Mame.MAX_GFX_ELEMENTS; gfx_index++)
                if (Mame.Machine.gfx[gfx_index] == null)
                    break;
            if (gfx_index == Mame.MAX_GFX_ELEMENTS)
                return 1;

            if (bpp == 4)
            {
                Mame.GfxLayout charlayout =
                new Mame.GfxLayout(
                    16, 16,
                    0,				/* filled in later */
                    4,
                    new uint[] { 0, 1, 2, 3 },
                    new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4, 8 * 4, 9 * 4, 10 * 4, 11 * 4, 12 * 4, 13 * 4, 14 * 4, 15 * 4 },
                    new uint[] { 0 * 64, 1 * 64, 2 * 64, 3 * 64, 4 * 64, 5 * 64, 6 * 64, 7 * 64, 8 * 64, 9 * 64, 10 * 64, 11 * 64, 12 * 64, 13 * 64, 14 * 64, 15 * 64 },
                    128 * 8
                );


                /* tweak the structure for the number of tiles we have */
                charlayout.total = (uint)Mame.memory_region_length(gfx_memory_region) / 128;

                /* decode the graphics */
                Mame.Machine.gfx[gfx_index] = Mame.decodegfx(Mame.memory_region(gfx_memory_region), charlayout);
            }
            else if (bpp == 7)
            {
                Mame.GfxLayout charlayout =
                new Mame.GfxLayout(
                    16, 16,
                    0,				/* filled in later */
                    7,
                    new uint[] { 1, 2, 3, 4, 5, 6, 7 },
                    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8, 8 * 8, 9 * 8, 10 * 8, 11 * 8, 12 * 8, 13 * 8, 14 * 8, 15 * 8 },
                    new uint[] { 0 * 128, 1 * 128, 2 * 128, 3 * 128, 4 * 128, 5 * 128, 6 * 128, 7 * 128, 8 * 128, 9 * 128, 10 * 128, 11 * 128, 12 * 128, 13 * 128, 14 * 128, 15 * 128 },
                    256 * 8
                );


                /* tweak the structure for the number of tiles we have */
                charlayout.total = (uint)Mame.memory_region_length(gfx_memory_region) / 256;

                /* decode the graphics */
                Mame.Machine.gfx[gfx_index] = Mame.decodegfx(Mame.memory_region(gfx_memory_region), charlayout);
            }
            else
            {
                Mame.printf("K051316_vh_start supports only 4 or 7 bpp\n");
                return 1;
            }

            if (Mame.Machine.gfx[gfx_index] == null)
                return 1;

            /* set the color information */
            Mame.Machine.gfx[gfx_index].colortable = Mame.Machine.remapped_colortable;
            Mame.Machine.gfx[gfx_index].total_colors = (int)(Mame.Machine.drv.color_table_len / (1 << bpp));

            K051316_memory_region[chip] = gfx_memory_region;
            K051316_gfxnum[chip] = gfx_index;
            K051316_bpp[chip] = bpp;
            K051316_callback[chip] = callback;

            K051316_tilemap[chip] = Mame.tilemap_create(K051316_get_tile_info, Mame.TILEMAP_OPAQUE, 16, 16, 32, 32);

            K051316_ram[chip] = new _BytePtr(0x800);

            if (K051316_ram[chip] == null || K051316_tilemap[chip] == null)
            {
                K051316_vh_stop(chip);
                return 1;
            }

            Mame.tilemap_set_clip(K051316_tilemap[chip], null);

            K051316_wraparound[chip] = 0;	/* default = no wraparound */
            K051316_offset[chip][0] = K051316_offset[chip][1] = 0;

            return 0;
        }
        public static void K051316_vh_stop(int chip)
        {
            K051316_ram[chip] = null;
        }
        static void K051316_get_tile_info(int col, int row)
        {
            int tile_index = 32 * row + col;
            int code = K051316_ram[K051316_chip_selected][tile_index];
            int color = K051316_ram[K051316_chip_selected][tile_index + 0x400];

            K051316_callback[K051316_chip_selected](ref code, ref color);

            Mame.SET_TILE_INFO(K051316_gfxnum[K051316_chip_selected], code, color);
        }

        public static int K051316_vh_start_0(int gfx_memory_region, int bpp, _K051316_callback callback)
        {
            return K051316_vh_start(0, gfx_memory_region, bpp, callback);
        }







        public static int K052109_r(int offset)
        {
            if (K052109_RMRD_line == Mame.CLEAR_LINE)
            {
                if ((offset & 0x1fff) >= 0x1800)
                {
                    if (offset >= 0x180c && offset < 0x1834)
                    { /* A y scroll */	}
                    else if (offset >= 0x1a00 && offset < 0x1c00)
                    { /* A x scroll */	}
                    else if (offset == 0x1d00)
                    { /* read for bitwise operations before writing */	}
                    else if (offset >= 0x380c && offset < 0x3834)
                    { /* B y scroll */	}
                    else if (offset >= 0x3a00 && offset < 0x3c00)
                    { /* B x scroll */	}
                    else
                        Mame.printf("%04x: read from unknown 052109 address %04x\n", Mame.cpu_get_pc(), offset);
                }

                return K052109_ram[offset];
            }
            else	/* Punk Shot and TMNT read from 0000-1fff, Aliens from 2000-3fff */
            {
                int code = (offset & 0x1fff) >> 5;
                int color = K052109_romsubbank;
                int bank = K052109_charrombank[(color & 0x0c) >> 2] >> 2;   /* discard low bits (TMNT) */
                int addr;

                if (has_extra_video_ram != 0) code |= color << 8;	/* kludge for X-Men */
                else
                    K052109_callback(0, bank, ref code, ref color);

                addr = (code << 5) + (offset & 0x1f);
                addr &= Mame.memory_region_length(K052109_memory_region) - 1;

#if false
	usrintf_showmessage("%04x: off%04x sub%02x (bnk%x) adr%06x",cpu_get_pc(),offset,K052109_romsubbank,bank,addr);
#endif

                return Mame.memory_region(K052109_memory_region)[addr];
            }
        }

        public static void K052109_w(int offset, int data)
        {
            if ((offset & 0x1fff) < 0x1800) /* tilemap RAM */
            {
                if (K052109_ram[offset] != data)
                {
                    if (offset >= 0x4000) has_extra_video_ram = 1;  /* kludge for X-Men */
                    K052109_ram[offset] = (byte)data;
                    Mame.tilemap_mark_tile_dirty(K052109_tilemap[(offset & 0x1fff) / 0x800], offset % 64, (offset % 0x800) / 64);
                }
            }
            else	/* control registers */
            {
                K052109_ram[offset] = (byte)data;

                if (offset >= 0x180c && offset < 0x1834)
                { /* A y scroll */	}
                else if (offset >= 0x1a00 && offset < 0x1c00)
                { /* A x scroll */	}
                else if (offset == 0x1c80)
                {
                    if (K052109_scrollctrl != data)
                    {
#if false
usrintf_showmessage("scrollcontrol = %02x",data);
#endif
                        Mame.printf("%04x: rowscrollcontrol = %02x\n", Mame.cpu_get_pc(), data);
                        K052109_scrollctrl = (byte)data;
                    }
                }
                else if (offset == 0x1d00)
                {
#if VERBOSE
if (errorlog) fprintf(errorlog,"%04x: 052109 register 1d00 = %02x\n",cpu_get_pc(),data);
#endif
                    /* bit 2 = irq enable */
                    /* the custom chip can also generate NMI and FIRQ, for use with a 6809 */
                    K052109_irq_enabled = data & 0x04;
                }
                else if (offset == 0x1d80)
                {
                    int dirty = 0;

                    if (K052109_charrombank[0] != (data & 0x0f)) dirty |= 1;
                    if (K052109_charrombank[1] != ((data >> 4) & 0x0f)) dirty |= 2;
                    if (dirty != 0)
                    {
                        int i;

                        K052109_charrombank[0] = (byte)(data & 0x0f);
                        K052109_charrombank[1] = (byte)((data >> 4) & 0x0f);

                        for (i = 0; i < 0x1800; i++)
                        {
                            int bank = (K052109_ram[i] & 0x0c) >> 2;
                            if ((bank == 0 && (dirty & 1) != 0) || (bank == 1 && (dirty & 2) != 0))
                            {
                                Mame.tilemap_mark_tile_dirty(K052109_tilemap[(i & 0x1fff) / 0x800], i % 64, (i % 0x800) / 64);
                            }
                        }
                    }
                }
                else if (offset == 0x1e00)
                {
                    Mame.printf("%04x: 052109 register 1e00 = %02x\n", Mame.cpu_get_pc(), data);
                    K052109_romsubbank = (byte)data;
                }
                else if (offset == 0x1e80)
                {
                    Mame.printf("%04x: 052109 register 1e80 = %02x\n", Mame.cpu_get_pc(), data);
                    Mame.tilemap_set_flip(K052109_tilemap[0], (data & 1) != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
                    Mame.tilemap_set_flip(K052109_tilemap[1], (data & 1) != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
                    Mame.tilemap_set_flip(K052109_tilemap[2], (data & 1) != 0 ? (Mame.TILEMAP_FLIPY | Mame.TILEMAP_FLIPX) : 0);
                    if (K052109_tileflip_enable != ((data & 0x06) >> 1))
                    {
                        K052109_tileflip_enable = ((data & 0x06) >> 1);

                        Mame.tilemap_mark_all_tiles_dirty(K052109_tilemap[0]);
                        Mame.tilemap_mark_all_tiles_dirty(K052109_tilemap[1]);
                        Mame.tilemap_mark_all_tiles_dirty(K052109_tilemap[2]);
                    }
                }
                else if (offset == 0x1f00)
                {
                    int dirty = 0;

                    if (K052109_charrombank[2] != (data & 0x0f)) dirty |= 1;
                    if (K052109_charrombank[3] != ((data >> 4) & 0x0f)) dirty |= 2;
                    if (dirty != 0)
                    {
                        int i;

                        K052109_charrombank[2] = (byte)(data & 0x0f);
                        K052109_charrombank[3] = (byte)((data >> 4) & 0x0f);

                        for (i = 0; i < 0x1800; i++)
                        {
                            int bank = (K052109_ram[i] & 0x0c) >> 2;
                            if ((bank == 2 && (dirty & 1) != 0) || (bank == 3 && (dirty & 2) != 0))
                                Mame.tilemap_mark_tile_dirty(K052109_tilemap[(i & 0x1fff) / 0x800], i % 64, (i % 0x800) / 64);
                        }
                    }
                }
                else if (offset >= 0x380c && offset < 0x3834)
                { /* B y scroll */	}
                else if (offset >= 0x3a00 && offset < 0x3c00)
                { /* B x scroll */	}
                else
                    Mame.printf("%04x: write %02x to unknown 052109 address %04x\n", Mame.cpu_get_pc(), data, offset);
            }
        }
        public static void K052109_set_RMRD_line(int state)
        {
            K052109_RMRD_line = state;
        }
        public static void K051316_wraparound_enable(int chip, int status)
        {
            K051316_wraparound[chip] = status;
        }
        public static int K051960_is_IRQ_enabled()
        {
            return K051960_irq_enabled;
        }
        static void shuffle(_ShortPtr buf, int len)
        {
            int i;
            ushort t;

            if (len == 2) return;

            if ((len % 4) != 0) throw new Exception();   /* must not happen */

            len /= 2;

            for (i = 0; i < len / 2; i++)
            {
                t = buf.read16(len / 2 + i);
                buf.write16(len / 2 + i, buf.read16(len + i));
                buf.write16(len + i, t);
            }

            shuffle(buf, len);
            shuffle(new _ShortPtr(buf, len * 2), len);
        }
        public static void konami_rom_deinterleave_2(int mem_region)
        {
            shuffle(new _ShortPtr(Mame.memory_region(mem_region)), Mame.memory_region_length(mem_region) / 2);
        }
        public static void K052109_tilemap_update()
        {
#if false
{
usrintf_showmessage("%x %x %x %x",
	K052109_charrombank[0],
	K052109_charrombank[1],
	K052109_charrombank[2],
	K052109_charrombank[3]);
}
#endif
            if ((K052109_scrollctrl & 0x03) == 0x02)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x1a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[1], 256);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[1], 1);
                yscroll = K052109_ram[0x180c];
                Mame.tilemap_set_scrolly(K052109_tilemap[1], 0, yscroll);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = scrollram[2 * (offs & 0xfff8) + 0] + 256 * scrollram[2 * (offs & 0xfff8) + 1];
                    xscroll -= 6;
                    Mame.tilemap_set_scrollx(K052109_tilemap[1], (offs + yscroll) & 0xff, xscroll);
                }
            }
            else if ((K052109_scrollctrl & 0x03) == 0x03)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x1a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[1], 256);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[1], 1);
                yscroll = K052109_ram[0x180c];
                Mame.tilemap_set_scrolly(K052109_tilemap[1], 0, yscroll);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = scrollram[2 * offs + 0] + 256 * scrollram[2 * offs + 1];
                    xscroll -= 6;
                    Mame.tilemap_set_scrollx(K052109_tilemap[1], (offs + yscroll) & 0xff, xscroll);
                }
            }
            else if ((K052109_scrollctrl & 0x04) == 0x04)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x1800);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[1], 1);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[1], 512);
                xscroll = K052109_ram[0x1a00] + 256 * K052109_ram[0x1a01];
                xscroll -= 6;
                Mame.tilemap_set_scrollx(K052109_tilemap[1], 0, xscroll);
                for (offs = 0; offs < 512; offs++)
                {
                    yscroll = scrollram[offs / 8];
                    Mame.tilemap_set_scrolly(K052109_tilemap[1], (offs + xscroll) & 0x1ff, yscroll);
                }
            }
            else
            {
                int xscroll, yscroll;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x1a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[1], 1);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[1], 1);
                xscroll = scrollram[0] + 256 * scrollram[1];
                xscroll -= 6;
                yscroll = K052109_ram[0x180c];
                Mame.tilemap_set_scrollx(K052109_tilemap[1], 0, xscroll);
                Mame.tilemap_set_scrolly(K052109_tilemap[1], 0, yscroll);
            }

            if ((K052109_scrollctrl & 0x18) == 0x10)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x3a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[2], 256);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[2], 1);
                yscroll = K052109_ram[0x380c];
                Mame.tilemap_set_scrolly(K052109_tilemap[2], 0, yscroll);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = scrollram[2 * (offs & 0xfff8) + 0] + 256 * scrollram[2 * (offs & 0xfff8) + 1];
                    xscroll -= 6;
                    Mame.tilemap_set_scrollx(K052109_tilemap[2], (offs + yscroll) & 0xff, xscroll);
                }
            }
            else if ((K052109_scrollctrl & 0x18) == 0x18)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x3a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[2], 256);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[2], 1);
                yscroll = K052109_ram[0x380c];
                Mame.tilemap_set_scrolly(K052109_tilemap[2], 0, yscroll);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = scrollram[2 * offs + 0] + 256 * scrollram[2 * offs + 1];
                    xscroll -= 6;
                    Mame.tilemap_set_scrollx(K052109_tilemap[2], (offs + yscroll) & 0xff, xscroll);
                }
            }
            else if ((K052109_scrollctrl & 0x20) == 0x20)
            {
                int xscroll, yscroll, offs;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x3800);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[2], 1);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[2], 512);
                xscroll = K052109_ram[0x3a00] + 256 * K052109_ram[0x3a01];
                xscroll -= 6;
                Mame.tilemap_set_scrollx(K052109_tilemap[2], 0, xscroll);
                for (offs = 0; offs < 512; offs++)
                {
                    yscroll = scrollram[offs / 8];
                    Mame.tilemap_set_scrolly(K052109_tilemap[2], (offs + xscroll) & 0x1ff, yscroll);
                }
            }
            else
            {
                int xscroll, yscroll;
                _BytePtr scrollram = new _BytePtr(K052109_ram, 0x3a00);


                Mame.tilemap_set_scroll_rows(K052109_tilemap[2], 1);
                Mame.tilemap_set_scroll_cols(K052109_tilemap[2], 1);
                xscroll = scrollram[0] + 256 * scrollram[1];
                xscroll -= 6;
                yscroll = K052109_ram[0x380c];
                Mame.tilemap_set_scrollx(K052109_tilemap[2], 0, xscroll);
                Mame.tilemap_set_scrolly(K052109_tilemap[2], 0, yscroll);
            }

            tilemap0_preupdate(); Mame.tilemap_update(K052109_tilemap[0]);
            tilemap1_preupdate(); Mame.tilemap_update(K052109_tilemap[1]);
            tilemap2_preupdate(); Mame.tilemap_update(K052109_tilemap[2]);



#if false
if (keyboard_pressed(KEYCODE_F))
{
	FILE *fp;
	fp=fopen("TILE.DMP", "w+b");
	if (fp)
	{
		fwrite(K052109_ram, 0x6000, 1, fp);
		usrintf_showmessage("saved");
		fclose(fp);
	}
}
#endif
        }

        static void tilemap0_preupdate()
        {
            colorram = K052109_colorram_F;
            videoram1 = K052109_videoram_F;
            videoram2 = K052109_videoram2_F;
            layer = 0;
        }
        static void tilemap1_preupdate()
        {
            colorram = K052109_colorram_A;
            videoram1 = K052109_videoram_A;
            videoram2 = K052109_videoram2_A;
            layer = 1;
        }
        static void tilemap2_preupdate()
        {
            colorram = K052109_colorram_B;
            videoram1 = K052109_videoram_B;
            videoram2 = K052109_videoram2_B;
            layer = 2;
        }
        static void K051316_preupdate(int chip)
        {
            K051316_chip_selected = chip;
        }
        static void K051316_tilemap_update(int chip)
        {
            K051316_preupdate(chip);
            Mame.tilemap_update(K051316_tilemap[chip]);
        }
        public static void K051316_tilemap_update_0()
        {
            K051316_tilemap_update(0);
        }
        public static void K051960_sprites_draw(Mame.osd_bitmap bitmap, int min_priority, int max_priority)
        {
            byte NUM_SPRITES = 128;
            int offs, pri_code;
            int[] sortedlist = new int[NUM_SPRITES];

            for (offs = 0; offs < NUM_SPRITES; offs++)
                sortedlist[offs] = -1;

            /* prebuild a sorted table */
            for (offs = 0; offs < 0x400; offs += 8)
            {
                if ((K051960_ram[offs] & 0x80) != 0)
                    sortedlist[K051960_ram[offs] & 0x7f] = offs;
            }

            for (pri_code = 0; pri_code < NUM_SPRITES; pri_code++)
            {
                int ox, oy, code, color, pri, size, w, h, x, y, zoomx, zoomy;
                bool flipx, flipy;
                /* sprites can be grouped up to 8x8. The draw order is
                     0  1  4  5 16 17 20 21
                     2  3  6  7 18 19 22 23
                     8  9 12 13 24 25 28 29
                    10 11 14 15 26 27 30 31
                    32 33 36 37 48 49 52 53
                    34 35 38 39 50 51 54 55
                    40 41 44 45 56 57 60 61
                    42 43 46 47 58 59 62 63
                */
                int[] xoffset = { 0, 1, 4, 5, 16, 17, 20, 21 };
                int[] yoffset = { 0, 2, 8, 10, 32, 34, 40, 42 };
                int[] width = { 1, 2, 1, 2, 4, 2, 4, 8 };
                int[] height = { 1, 1, 2, 2, 2, 4, 4, 8 };


                offs = sortedlist[pri_code];
                if (offs == -1) continue;

                code = K051960_ram[offs + 2] + ((K051960_ram[offs + 1] & 0x1f) << 8);
                color = K051960_ram[offs + 3] & 0xff;
                pri = 0;

                K051960_callback(ref code, ref color, ref pri);

                if (pri < min_priority || pri > max_priority) continue;

                size = (K051960_ram[offs + 1] & 0xe0) >> 5;
                w = width[size];
                h = height[size];

                if (w >= 2) code &= ~0x01;
                if (h >= 2) code &= ~0x02;
                if (w >= 4) code &= ~0x04;
                if (h >= 4) code &= ~0x08;
                if (w >= 8) code &= ~0x10;
                if (h >= 8) code &= ~0x20;

                ox = (256 * K051960_ram[offs + 6] + K051960_ram[offs + 7]) & 0x01ff;
                oy = 256 - ((256 * K051960_ram[offs + 4] + K051960_ram[offs + 5]) & 0x01ff);
                flipx = (K051960_ram[offs + 6] & 0x02) != 0;
                flipy = (K051960_ram[offs + 4] & 0x02) != 0;
                zoomx = (K051960_ram[offs + 6] & 0xfc) >> 2;
                zoomy = (K051960_ram[offs + 4] & 0xfc) >> 2;
                zoomx = 0x10000 / 128 * (128 - zoomx);
                zoomy = 0x10000 / 128 * (128 - zoomy);

                if (K051960_spriteflip != 0)
                {
                    ox = 512 - (zoomx * w >> 12) - ox;
                    oy = 256 - (zoomy * h >> 12) - oy;
                    flipx = !flipx;
                    flipy = !flipy;
                }

                if (zoomx == 0x10000 && zoomy == 0x10000)
                {
                    int sx, sy;

                    for (y = 0; y < h; y++)
                    {
                        sy = oy + 16 * y;

                        for (x = 0; x < w; x++)
                        {
                            int c = code;

                            sx = ox + 16 * x;
                            if (flipx) c += xoffset[(w - 1 - x)];
                            else c += xoffset[x];
                            if (flipy) c += yoffset[(h - 1 - y)];
                            else c += yoffset[y];

                            /* hack to simulate shadow */
                            if ((K051960_ram[offs + 3] & 0x80) != 0)
                            {
                                int o = K051960_gfx.colortable[16 * color + 15];
                                K051960_gfx.colortable[16 * color + 15] = (byte)Mame.palette_transparent_pen;
                                Mame.drawgfx(bitmap, K051960_gfx,
                                        (uint)c,
                                        (uint)color,
                                        flipx, flipy,
                                        sx & 0x1ff, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PENS, (Mame.cpu_getcurrentframe() & 1) != 0 ? 0x8001 : 0x0001);
                                K051960_gfx.colortable.write16(16 * color + 15, (ushort)o);
                            }
                            else
                                Mame.drawgfx(bitmap, K051960_gfx,
                                        (uint)c,
                                        (uint)color,
                                        flipx, flipy,
                                        sx & 0x1ff, sy,
                                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                        }
                    }
                }
                else
                {
                    int sx, sy, zw, zh;

                    for (y = 0; y < h; y++)
                    {
                        sy = oy + ((zoomy * y + (1 << 11)) >> 12);
                        zh = (oy + ((zoomy * (y + 1) + (1 << 11)) >> 12)) - sy;

                        for (x = 0; x < w; x++)
                        {
                            int c = code;

                            sx = ox + ((zoomx * x + (1 << 11)) >> 12);
                            zw = (ox + ((zoomx * (x + 1) + (1 << 11)) >> 12)) - sx;
                            if (flipx) c += xoffset[(w - 1 - x)];
                            else c += xoffset[x];
                            if (flipy) c += yoffset[(h - 1 - y)];
                            else c += yoffset[y];

                            Mame.drawgfxzoom(bitmap, K051960_gfx,
                                    (uint)c,
                                    (uint)color,
                                    flipx, flipy,
                                    sx & 0x1ff, sy,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0,
                                    (zw << 16) / 16, (zh << 16) / 16);
                        }
                    }
                }
            }
#if false
if (keyboard_pressed(KEYCODE_D))
{
	FILE *fp;
	fp=fopen("SPRITE.DMP", "w+b");
	if (fp)
	{
		fwrite(K051960_ram, 0x400, 1, fp);
		usrintf_showmessage("saved");
		fclose(fp);
	}
}
#endif

        }
        public static void K052109_tilemap_draw(Mame.osd_bitmap bitmap, int num, int flags)
        {
            Mame.tilemap_draw(bitmap, K052109_tilemap[num], flags);
        }
        static void K051316_zoom_draw(int chip, Mame.osd_bitmap bitmap)
{
	uint startx,starty,cx,cy;
	int incxx,incxy,incyx,incyy;
	int x,sx,sy,ex,ey;
	Mame.osd_bitmap srcbitmap = K051316_tilemap[chip].pixmap;

	startx = (uint)(256 * ((short)(256 * K051316_ctrlram[chip][0x00] + K051316_ctrlram[chip][0x01])));
	incxx  =        (short)(256 * K051316_ctrlram[chip][0x02] + K051316_ctrlram[chip][0x03]);
	incyx  =        (short)(256 * K051316_ctrlram[chip][0x04] + K051316_ctrlram[chip][0x05]);
	starty =(uint)( 256 * ((short)(256 * K051316_ctrlram[chip][0x06] + K051316_ctrlram[chip][0x07])));
	incxy  =        (short)(256 * K051316_ctrlram[chip][0x08] + K051316_ctrlram[chip][0x09]);
	incyy  =        (short)(256 * K051316_ctrlram[chip][0x0a] + K051316_ctrlram[chip][0x0b]);

	startx += (uint)((Mame.Machine.drv.visible_area.min_y - (16 + K051316_offset[chip][1])) * incyx);
	starty += (uint)((Mame.Machine.drv.visible_area.min_y - (16 + K051316_offset[chip][1])) * incyy);

	startx += (uint)((Mame.Machine.drv.visible_area.min_x - (89 + K051316_offset[chip][0])) * incxx);
	starty += (uint)((Mame.Machine.drv.visible_area.min_x - (89 + K051316_offset[chip][0])) * incxy);

	sx = Mame.Machine.drv.visible_area.min_x;
	sy = Mame.Machine.drv.visible_area.min_y;
	ex = Mame.Machine.drv.visible_area.max_x;
	ey = Mame.Machine.drv.visible_area.max_y;

	if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY)!=0)
	{
		int t;

		t = (int)startx; startx = starty; starty = (uint)t;
		t = sx; sx = sy; sy = t;
		t = ex; ex = ey; ey = t;
		t = incxx; incxx = incyy; incyy = t;
		t = incxy; incxy = incyx; incyx = t;
	}

	if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X)!=0)
	{
		int w = ex - sx;

		incxy = -incxy;
		incyx = -incyx;
		startx = 0xfffff - startx;
		startx -= (uint)(incxx * w);
		starty -= (uint)(incxy * w);
	}

	if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y)!=0)
	{
		int h = ey - sy;

		incxy = -incxy;
		incyx = -incyx;
		starty = 0xfffff - starty;
		startx -=(uint)(incyx * h);
		starty -=(uint)(incyy * h);
	}

	if (bitmap.depth == 8)
	{
		_BytePtr dest;

		if (incxy == 0 && incyx == 0 && K051316_wraparound[chip]==0)
		{
			/* optimized loop for the not rotated case */

			if (incxx == 0x800)
			{
				/* optimized loop for the not zoomed case */

				/* startx is unsigned */
				startx = (uint)(((int)startx) >> 11);

				if (startx >= 512)
				{
					sx += (int)-startx;
					startx = 0;
				}

				if (sx <= ex)
				{
					while (sy <= ey)
					{
						if ((starty & 0xfff00000) == 0)
						{
							x = sx;
							cx = startx;
							cy = starty >> 11;
							dest = new _BytePtr(bitmap.line[sy], sx);
							while (x <= ex && cx < 512)
							{
								int c = srcbitmap.line[cy][cx];

								if (c != Mame.palette_transparent_pen)
									dest[0] = (byte)c;

								cx++;
								x++;
								dest.offset++;
							}
						}
						starty += (uint)incyy;
						sy++;
					}
				}
			}
			else
			{
				while ((startx & 0xfff00000) != 0 && sx <= ex)
				{
					startx += (uint)incxx;
					sx++;
				}

				if ((startx & 0xfff00000) == 0)
				{
					while (sy <= ey)
					{
						if ((starty & 0xfff00000) == 0)
						{
							x = sx;
							cx = startx;
							cy = starty >> 11;
							dest = new _BytePtr(bitmap.line[sy], sx);
							while (x <= ex && (cx & 0xfff00000) == 0)
							{
								int c = srcbitmap.line[cy][cx >> 11];

								if (c != Mame.palette_transparent_pen)
									dest[0] = (byte)c;

								cx +=(uint) incxx;
								x++;
								dest.offset++;
							}
						}
						starty += (uint)incyy;
						sy++;
					}
				}
			}
		}
		else
		{
			if (K051316_wraparound[chip]!=0)
			{
				/* plot with wraparound */
				while (sy <= ey)
				{
					x = sx;
					cx = startx;
					cy = starty;
					dest = new _BytePtr(bitmap.line[sy],sx);
					while (x <= ex)
					{
						int c = srcbitmap.line[(cy >> 11) & 0x1ff][(cx >> 11) & 0x1ff];

						if (c != Mame.palette_transparent_pen)
							dest[0] = (byte)c;

						cx += (uint)incxx;
						cy += (uint)incxy;
						x++;
						dest.offset++;
					}
					startx += (uint)incyx;
					starty += (uint)incyy;
					sy++;
				}
			}
			else
			{
				while (sy <= ey)
				{
					x = sx;
					cx = startx;
					cy = starty;
					dest = new _BytePtr(bitmap.line[sy],sx);
					while (x <= ex)
					{
						if ((cx & 0xfff00000) == 0 && (cy & 0xfff00000) == 0)
						{
							int c = srcbitmap.line[cy >> 11][cx >> 11];

							if (c != Mame.palette_transparent_pen)
								dest[0] = (byte)c;
						}

						cx += (uint)incxx;
						cy += (uint)incxy;
						x++;
						dest.offset++;
					}
					startx += (uint)incyx;
					starty += (uint)incyy;
					sy++;
				}
			}
		}
	}
	else
	{
		/* 16-bit case */

		_ShortPtr dest;

		if (incxy == 0 && incyx == 0 && K051316_wraparound[chip]==0)
		{
			/* optimized loop for the not rotated case */

			if (incxx == 0x800)
			{
				/* optimized loop for the not zoomed case */

				/* startx is unsigned */
				startx = (uint)((int)startx) >> 11;

				if (startx >= 512)
				{
					sx += (int)-startx;
					startx = 0;
				}

				if (sx <= ex)
				{
					while (sy <= ey)
					{
						if ((starty & 0xfff00000) == 0)
						{
							x = sx;
							cx = startx;
							cy = starty >> 11;
							dest = new _ShortPtr(bitmap.line[sy], sx);
							while (x <= ex && cx < 512)
							{
								int c = srcbitmap.line[cy].read16((int)cx);

								if (c != Mame.palette_transparent_pen)
									dest.write16(0, (ushort)c);

								cx++;
								x++;
								dest.offset += 2;
							}
						}
						starty += (uint)incyy;
						sy++;
					}
				}
			}
			else
			{
				while ((startx & 0xfff00000) != 0 && sx <= ex)
				{
					startx += (uint)incxx;
					sx++;
				}

				if ((startx & 0xfff00000) == 0)
				{
					while (sy <= ey)
					{
						if ((starty & 0xfff00000) == 0)
						{
							x = sx;
							cx = startx;
							cy = starty >> 11;
							dest = new _ShortPtr(bitmap.line[sy], sx);
							while (x <= ex && (cx & 0xfff00000) == 0)
							{
								int c = srcbitmap.line[cy].read16((int)cx >> 11);

								if (c !=Mame. palette_transparent_pen)
									dest.write16(0,(ushort) c);

								cx += (uint)incxx;
								x++;
								dest.offset+=2;
							}
						}
						starty += (uint)incyy;
						sy++;
					}
				}
			}
		}
		else
		{
			if (K051316_wraparound[chip]!=0)
			{
				/* plot with wraparound */
				while (sy <= ey)
				{
					x = sx;
					cx = startx;
					cy = starty;
					dest = new _ShortPtr(bitmap.line[sy], sx);
					while (x <= ex)
					{
						int c = srcbitmap.line[(cy >> 11) & 0x1ff].read16((int)(cx >> 11) & 0x1ff);

						if (c != Mame.palette_transparent_pen)
							dest.write16(0,(ushort)c);

						cx += (uint)incxx;
						cy += (uint)incxy;
						x++;
						dest.offset += 2;
					}
					startx +=(uint) incyx;
					starty += (uint)incyy;
					sy++;
				}
			}
			else
			{
				while (sy <= ey)
				{
					x = sx;
					cx = startx;
					cy = starty;
					dest = new _ShortPtr(bitmap.line[sy], sx);
					while (x <= ex)
					{
						if ((cx & 0xfff00000) == 0 && (cy & 0xfff00000) == 0)
						{
							int c = (srcbitmap.line[cy >> 11].read16((int)cx >> 11));

							if (c != Mame.palette_transparent_pen)
								dest.write16(0,(ushort) c);
						}

						cx += (uint)incxx;
						cy += (uint)incxy;
						x++;
						dest.offset += 2;
					}
					startx += (uint)incyx;
					starty += (uint)incyy;
					sy++;
				}
			}
		}
	}
#if false
	usrintf_showmessage("%02x%02x%02x%02x %02x%02x%02x%02x %02x%02x%02x%02x %02x%02x%02x%02x",
			K051316_ctrlram[chip][0x00],
			K051316_ctrlram[chip][0x01],
			K051316_ctrlram[chip][0x02],
			K051316_ctrlram[chip][0x03],
			K051316_ctrlram[chip][0x04],
			K051316_ctrlram[chip][0x05],
			K051316_ctrlram[chip][0x06],
			K051316_ctrlram[chip][0x07],
			K051316_ctrlram[chip][0x08],
			K051316_ctrlram[chip][0x09],
			K051316_ctrlram[chip][0x0a],
			K051316_ctrlram[chip][0x0b],
			K051316_ctrlram[chip][0x0c],	/* bank for ROM testing */
			K051316_ctrlram[chip][0x0d],
			K051316_ctrlram[chip][0x0e],	/* 0 = test ROMs */
			K051316_ctrlram[chip][0x0f]);
#endif
}
        public static void K051960_mark_sprites_colors()
        {
            int offs, i;

            ushort[] palette_map = new ushort[512];

            //memset (palette_map, 0, sizeof (palette_map));

            /* sprites */
            for (offs = 0x400 - 8; offs >= 0; offs -= 8)
            {
                if ((K051960_ram[offs] & 0x80) != 0)
                {
                    int code, color, pri;

                    code = K051960_ram[offs + 2] + ((K051960_ram[offs + 1] & 0x1f) << 8);
                    color = (K051960_ram[offs + 3] & 0xff);
                    pri = 0;
                    K051960_callback(ref code, ref color, ref pri);
                    palette_map[color] |= 0xffff;
                }
            }

            /* now build the final table */
            for (i = 0; i < 512; i++)
            {
                int usage = palette_map[i], j;
                if (usage != 0)
                {
                    for (j = 1; j < 16; j++)
                        if ((usage & (1 << j)) != 0)
                            Mame.palette_used_colors[i * 16 + j] |= Mame.PALETTE_COLOR_VISIBLE;
                }
            }
        }
        public static void K051316_zoom_draw_0(Mame.osd_bitmap bitmap)
        {
            K051316_zoom_draw(0, bitmap);
        }
    }
}
