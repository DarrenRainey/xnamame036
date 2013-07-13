#define TRANSPARENT_SHADOWS
#define HANGON_DIGITAL_CONTROLS
#define SPRITE_SIDE_MARKERS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_system16 : Mame.GameDriver
    {
        static void get_bg_tile_info(int col, int row)
        {
            _ShortPtr source = new _ShortPtr(sys16_tileram);

            if (row < 32)
            {
                if (col < 64)
                {
                    source.offset += (64 * 32 * sys16_bg_page[0]) * 2;
                }
                else
                {
                    source.offset += (64 * 32 * sys16_bg_page[1]) * 2;
                }
            }
            else
            {
                if (col < 64)
                {
                    source.offset += (64 * 32 * sys16_bg_page[2]) * 2;
                }
                else
                {
                    source.offset += (64 * 32 * sys16_bg_page[3]) * 2;
                }
            }
            row = row % 32;
            col = col % 64;

            {
                int data = source[row * 64 + col];
                int tile_number = (data & 0xfff) +
                        0x1000 * ((data & sys16_tilebank_switch) != 0 ? sys16_tile_bank1 : sys16_tile_bank0);

                if (sys16_textmode == 0)
                {
                    Mame.SET_TILE_INFO(0, tile_number, (data >> 6) & 0x7f);
                }
                else
                {
                    Mame.SET_TILE_INFO(0, tile_number, (data >> 5) & 0x7f);
                }
                switch (sys16_bg_priority_mode)
                {
                    case 1:		// Alien Syndrome
                        Mame.tile_info.priority = (data & 0x8000) != 0 ? (byte)1 : (byte)0;
                        break;
                    case 2:		// Body Slam / wrestwar
                        if ((data & 0xff00) >= sys16_bg_priority_value)
                            Mame.tile_info.priority = 1;
                        else
                            Mame.tile_info.priority = 0;
                        break;
                    case 3:		// sys18 games
                        if ((data & 0x8000) != 0)
                            Mame.tile_info.priority = 2;
                        else if ((data & 0xff00) >= sys16_bg_priority_value)
                            Mame.tile_info.priority = 1;
                        else
                            Mame.tile_info.priority = 0;
                        break;
                }
            }
        }
        static void get_fg_tile_info(int col, int row)
        {
            _ShortPtr source = new _ShortPtr(sys16_tileram);

            if (row < 32)
            {
                if (col < 64)
                {
                    source.offset += (64 * 32 * sys16_fg_page[0]) * 2;
                }
                else
                {
                    source.offset += (64 * 32 * sys16_fg_page[1]) * 2;
                }
            }
            else
            {
                if (col < 64)
                {
                    source.offset += (64 * 32 * sys16_fg_page[2]) * 2;
                }
                else
                {
                    source.offset += (64 * 32 * sys16_fg_page[3]) * 2;
                }
            }
            row = row % 32;
            col = col % 64;

            {
                int data = source[row * 64 + col];
                int tile_number = (data & 0xfff) +
                        0x1000 * ((data & sys16_tilebank_switch) != 0 ? sys16_tile_bank1 : sys16_tile_bank0);

                if (sys16_textmode == 0)
                {
                    Mame.SET_TILE_INFO(0, tile_number, (data >> 6) & 0x7f);
                }
                else
                {
                    Mame.SET_TILE_INFO(0, tile_number, (data >> 5) & 0x7f);
                }
                switch (sys16_fg_priority_mode)
                {
                    case 1:		// alien syndrome
                        Mame.tile_info.priority = (data & 0x8000) != 0 ? (byte)1 : (byte)0;
                        //				if(READ_WORD(&paletteram[((data>>6)&0x7f)*16]) !=0 && tile_info.priority==1)
                        //					tile_info.flags=TILE_IGNORE_TRANSPARENCY;
                        break;

                    case 3:
                        if ((data & 0xff00) >= sys16_fg_priority_value)
                            Mame.tile_info.priority = 1;
                        else
                            Mame.tile_info.priority = 0;
                        break;

                    default:
                        if (sys16_fg_priority_mode >= 0)
                            Mame.tile_info.priority = (data & 0x8000) != 0 ? (byte)1 : (byte)0;
                        break;
                }
            }
        }
        static void get_text_tile_info(int col, int row)
        {
            _ShortPtr source = new _ShortPtr(sys16_textram);
            int tile_number = source[row * 64 + col + (64 - 40)];
            int pri = tile_number >> 8;
            if (sys16_textmode == 0)
            {
                Mame.SET_TILE_INFO(0, (tile_number & 0x1ff) + sys16_tile_bank0 * 0x1000, (tile_number >> 9) % 8);
            }
            else
            {
                Mame.SET_TILE_INFO(0, (tile_number & 0xff) + sys16_tile_bank0 * 0x1000, (tile_number >> 8) % 8);
            }
            if (pri >= sys16_textlayer_lo_min && pri <= sys16_textlayer_lo_max)
                Mame.tile_info.priority = 1;
            if (pri >= sys16_textlayer_hi_min && pri <= sys16_textlayer_hi_max)
                Mame.tile_info.priority = 0;
        }

        public static int sys16_vh_start()
        {
            if (sys16_bg1_trans == 0)
                background = Mame.tilemap_create(
                    get_bg_tile_info,
                    Mame.TILEMAP_OPAQUE,
                    8, 8,
                    64 * 2, 32 * 2);
            else
                background = Mame.tilemap_create(
            get_bg_tile_info,
            Mame.TILEMAP_TRANSPARENT,
            8, 8,
            64 * 2, 32 * 2);

            foreground = Mame.tilemap_create(
        get_fg_tile_info,
        Mame.TILEMAP_TRANSPARENT,
        8, 8,
        64 * 2, 32 * 2);

            text_layer = Mame.tilemap_create(
        get_text_tile_info,
        Mame.TILEMAP_TRANSPARENT,
        8, 8,
        40, 28);

            sprite_list = Mame.SpriteManager.sprite_list_create(NUM_SPRITES, Mame.SpriteManager.SPRITE_LIST_BACK_TO_FRONT | Mame.SpriteManager.SPRITE_LIST_RAW_DATA);

            Mame.SpriteManager.sprite_set_shade_table(shade_table);

            if (background != null && foreground != null && text_layer != null && sprite_list != null)
            {
                /* initialize all entries to black - needed for Golden Axe*/

                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    Mame.palette_change_color(i, 0, 0, 0);
                }
#if TRANSPARENT_SHADOWS
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                    Mame.palette_used_colors[i] = Mame.PALETTE_COLOR_UNUSED;
                //memset(&palette_used_colors[0], Mame.PALETTE_COLOR_UNUSED, Mame.Machine.drv.total_colors);
                if (Mame.Machine.scrbitmap.depth == 8) /* 8 bit shadows */
                {
                    for (int j = 0, i = (int)(Mame.Machine.drv.total_colors / 2); j < sys16_MaxShadowColors; i++, j++)
                    {
                        int color = j * 160 / (sys16_MaxShadowColors - 1);
                        color = color | 0x04;
                        Mame.palette_change_color(i, (byte)color, (byte)color, (byte)color);
                    }
                }
                if (sys16_MaxShadowColors == 32)
                    sys16_MaxShadowColors_Shift = ShadowColorsShift;
                else if (sys16_MaxShadowColors == 16)
                    sys16_MaxShadowColors_Shift = ShadowColorsShift + 1;

#endif
                for (int i = 0; i < MAXCOLOURS; i++)
                {
                    sys16_palettedirty[i] = 0;
                }
                sys16_freezepalette = 0;

                sprite_list.max_priority = 3;
                sprite_list.sprite_type = Mame.SpriteManager.SpriteType.SPRITE_TYPE_ZOOM;

                if (sys16_bg1_trans != 0) background.transparent_pen = 0;
                foreground.transparent_pen = 0;
                text_layer.transparent_pen = 0;

                sys16_tile_bank0 = 0;
                sys16_tile_bank1 = 1;

                sys16_fg_scrollx = 0;
                sys16_fg_scrolly = 0;

                sys16_bg_scrollx = 0;
                sys16_bg_scrolly = 0;

                sys16_refreshenable = 1;
                sys16_clear_screen = 0;

                /* common defaults */
                sys16_update_proc = null;
                sys16_spritesystem = 1;
                sys16_sprxoffset = -0xb8;
                sys16_textmode = 0;
                sys16_bgxoffset = 0;
                sys16_dactype = 0;
                sys16_bg_priority_mode = 0;
                sys16_fg_priority_mode = 0;
                sys16_spritelist_end = 0xffff;
                sys16_tilebank_switch = 0x1000;

                // Defaults for sys16 games
                sys16_textlayer_lo_min = 0;
                sys16_textlayer_lo_max = 0x7f;
                sys16_textlayer_hi_min = 0x80;
                sys16_textlayer_hi_max = 0xff;

                sys16_18_mode = 0;

#if GAMMA_ADJUST
		{
			static float sys16_orig_gamma=0;
			static float sys16_set_gamma=0;
			float cur_gamma=osd_get_gamma();

			if(sys16_orig_gamma == 0)
			{
				sys16_orig_gamma = cur_gamma;
				sys16_set_gamma = cur_gamma - 0.35;
				if (sys16_set_gamma < 0.5) sys16_set_gamma = 0.5;
				if (sys16_set_gamma > 2.0) sys16_set_gamma = 2.0;
				osd_set_gamma(sys16_set_gamma);
			}
			else
			{
				if(sys16_orig_gamma == cur_gamma)
				{
					osd_set_gamma(sys16_set_gamma);
				}
			}
		}
#endif

                return 0;
            }
            return 1;
        }


        const int MAXCOLOURS = 8192;
#if TRANSPARENT_SHADOWS
        const int ShadowColorsShift = 8;
        const int NumOfShadowColors = 32;
        public const int ShadowColorsMultiplier = 2;
        static ushort[] shade_table = new ushort[MAXCOLOURS];
        int sys16_sh_shadowpal;
#endif
        static int sys16_MaxShadowColors_Shift;
        const int NUM_SPRITES = 128;

        static Mame.SpriteManager.sprite_list sprite_list;
        /* video driver constants (potentially different for each game) */
        public static int sys16_spritesystem;
        public static int sys16_sprxoffset;
        public static int sys16_bgxoffset;
        public static int sys16_fgxoffset;
        public static int[] sys16_obj_bank;
        public static int sys16_textmode;
        public static int sys16_textlayer_lo_min;
        public static int sys16_textlayer_lo_max;
        public static int sys16_textlayer_hi_min;
        public static int sys16_textlayer_hi_max;
        static int sys16_dactype;
        public static int sys16_bg1_trans;						// alien syn + sys18
        public static int sys16_bg_priority_mode;
        public static int sys16_fg_priority_mode;
        public static int sys16_bg_priority_value;
        public static int sys16_fg_priority_value;
        static int sys16_18_mode;
        public static int sys16_spritelist_end;
        public static int sys16_tilebank_switch;
        public static int sys16_rowscroll_scroll;
        public static int sys16_quartet_title_kludge;
        public delegate void _sys16_update_proc();
        public static _sys16_update_proc sys16_update_proc;

        /* video registers */
        static int sys16_tile_bank1;
        static int sys16_tile_bank0;
        static int sys16_refreshenable;
        static int sys16_clear_screen;
        public static int sys16_bg_scrollx, sys16_bg_scrolly;
        static int[] sys16_bg_page = new int[4];
        public static int sys16_fg_scrollx, sys16_fg_scrolly;
        static int[] sys16_fg_page = new int[4];

        static int sys16_bg2_scrollx, sys16_bg2_scrolly;
        static int[] sys16_bg2_page = new int[4];
        static int sys16_fg2_scrollx, sys16_fg2_scrolly;
        static int[] sys16_fg2_page = new int[4];

        static int sys18_bg2_active;
        static int sys18_fg2_active;
        static _BytePtr sys18_splittab_bg_x;
        static _BytePtr sys18_splittab_bg_y;
        static _BytePtr sys18_splittab_fg_x;
        static _BytePtr sys18_splittab_fg_y;

        static int sys16_freezepalette;
        static int[] sys16_palettedirty = new int[MAXCOLOURS];
        static int sys16_MaxShadowColors;

        public static _BytePtr gr_ver;
        public static _BytePtr gr_hor;
        public static _BytePtr gr_pal;
        public static _BytePtr gr_flip;
        public static int gr_palette;
        public static int gr_palette_default;
        public static byte[,] gr_colorflip = new byte[2, 4];
        public static _BytePtr gr_second_road;

        public static YM2151interface ym2151_interface =
            new YM2151interface(1, 4096000, new int[] { YM2151.YM3012_VOL(40, Mame.MIXER_PAN_LEFT, 40, Mame.MIXER_PAN_RIGHT) }, new YM2151irqhandler[] { null },new Mame.mem_write_handler[]{ null});
        public static SEGAPCMinterface segapcm_interface_15k = new SEGAPCMinterface(
    segapcm.SEGAPCM_SAMPLE15K,
    segapcm.BANK_256,
    Mame.REGION_SOUND1,		// memory region
    50
);

        static Mame.tilemap background, foreground, text_layer;
        static Mame.tilemap background2, foreground2;
        static int[] old_bg_page = new int[4], old_fg_page = new int[4];
        static int old_tile_bank1, old_tile_bank0;
        static int[] old_bg2_page = new int[4], old_fg2_page = new int[4];

        public static _BytePtr sys16_tileram = new _BytePtr(1);
        public static _BytePtr sys16_textram = new _BytePtr(1);
        public static _BytePtr sys16_spriteram = new _BytePtr(1);
        public static _BytePtr sys16_workingram = new _BytePtr(1);
        public static _BytePtr sys16_extraram = new _BytePtr(1);
        public static _BytePtr sys16_extraram2 = new _BytePtr(1);
        public static _BytePtr sys16_extraram3 = new _BytePtr(1);
        public static _BytePtr sys16_extraram4 = new _BytePtr(1);
        public static _BytePtr sys16_extraram5 = new _BytePtr(1);
        public static _BytePtr sound_shared_ram = new _BytePtr(1);
        public static _BytePtr shared_ram = new _BytePtr(1);

        public static int shared_ram_r(int offset) { return shared_ram.READ_WORD(offset); }
        public static void shared_ram_w(int offset, int data) { Mame.COMBINE_WORD_MEM(shared_ram, offset, data); }
        public static int sh_io_joy_r(int offset) { return (Mame.input_port_5_r(offset) << 8) + Mame.input_port_6_r(offset); }
        public static int sh_motor_status_r(int offset) { return 0x0; }


        static void patch_codeX(int offset, int data, int cpu)
        {
            int aligned_offset = offset & 0xfffffe;
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1 + cpu);
            int old_word = RAM.READ_WORD(aligned_offset);

            if ((offset & 1) != 0)
                data = (old_word & 0xff00) | data;
            else
                data = (old_word & 0x00ff) | (data << 8);

            RAM.WRITE_WORD(aligned_offset, (ushort)data);
        }

        public static void patch_code(int offset, int data) { patch_codeX(offset, data, 0); }

        public static int sound_shared_ram_r(int offset)
        {
            return (sound_shared_ram[offset] << 8) + sound_shared_ram[offset + 1];
        }
        public static void sound_shared_ram_w(int offset, int data)
        {
            int val = (sound_shared_ram[offset] << 8) + sound_shared_ram[offset + 1];
            val = (val & (data >> 16)) | (data & 0xffff);

            sound_shared_ram[offset] = (byte)(val >> 8);
            sound_shared_ram[offset + 1] = (byte)(val & 0xff);
        }
        public static int sound2_shared_ram_r(int offset) { return sound_shared_ram[offset]; }
        public static void sound2_shared_ram_w(int offset, int data) { sound_shared_ram[offset] = (byte)data; }
        public static void sound_command_w(int offset, int data)
        {
            Mame.soundlatch_w(0, data & 0xff);
            Mame.cpu_cause_interrupt(1, 0);
        }
        public static void sound_command_nmi_w(int offset, int data)
        {
            Mame.soundlatch_w(0, data & 0xff);
            Mame.cpu_set_nmi_line(1, Mame.PULSE_LINE);
        }

        public static int sys16_tileram_r(int offset)
        {
            return sys16_tileram.READ_WORD(offset);
        }
        public static int sys16_textram_r(int offset)
        {
            return sys16_textram.READ_WORD(offset);
        }
        public static void sys16_tileram_w(int offset, int data)
        {
            int oldword = sys16_tileram.READ_WORD(offset);
            int newword = Mame.COMBINE_WORD(oldword, data);
            if (oldword != newword)
            {
                int row, col, page;
                sys16_tileram.WRITE_WORD(offset, (ushort)newword);
                offset = offset / 2;
                col = offset % 64;
                row = (offset / 64) % 32;
                page = offset / (64 * 32);

                if (sys16_bg_page[0] == page) Mame.tilemap_mark_tile_dirty(background, col, row);
                if (sys16_bg_page[1] == page) Mame.tilemap_mark_tile_dirty(background, col + 64, row);
                if (sys16_bg_page[2] == page) Mame.tilemap_mark_tile_dirty(background, col, row + 32);
                if (sys16_bg_page[3] == page) Mame.tilemap_mark_tile_dirty(background, col + 64, row + 32);

                if (sys16_fg_page[0] == page) Mame.tilemap_mark_tile_dirty(foreground, col, row);
                if (sys16_fg_page[1] == page) Mame.tilemap_mark_tile_dirty(foreground, col + 64, row);
                if (sys16_fg_page[2] == page) Mame.tilemap_mark_tile_dirty(foreground, col, row + 32);
                if (sys16_fg_page[3] == page) Mame.tilemap_mark_tile_dirty(foreground, col + 64, row + 32);

                if (sys16_18_mode != 0)
                {
                    if (sys16_bg2_page[0] == page) Mame.tilemap_mark_tile_dirty(background2, col, row);
                    if (sys16_bg2_page[1] == page) Mame.tilemap_mark_tile_dirty(background2, col + 64, row);
                    if (sys16_bg2_page[2] == page) Mame.tilemap_mark_tile_dirty(background2, col, row + 32);
                    if (sys16_bg2_page[3] == page) Mame.tilemap_mark_tile_dirty(background2, col + 64, row + 32);

                    if (sys16_fg2_page[0] == page) Mame.tilemap_mark_tile_dirty(foreground2, col, row);
                    if (sys16_fg2_page[1] == page) Mame.tilemap_mark_tile_dirty(foreground2, col + 64, row);
                    if (sys16_fg2_page[2] == page) Mame.tilemap_mark_tile_dirty(foreground2, col, row + 32);
                    if (sys16_fg2_page[3] == page) Mame.tilemap_mark_tile_dirty(foreground2, col + 64, row + 32);
                }
            }
        }
        public static void sys16_textram_w(int offset, int data)
        {
            int oldword = sys16_textram.READ_WORD(offset);
            int newword = Mame.COMBINE_WORD(oldword, data);
            if (oldword != newword)
            {
                int row, col;
                sys16_textram.WRITE_WORD(offset, (ushort)newword);
                offset = (offset / 2);
                col = (offset % 64);
                row = offset / 64;
                col -= (64 - 40);
                if (col >= 0 && col < 40 && row < 28)
                {
                    Mame.tilemap_mark_tile_dirty(text_layer, col, row);
                }
            }
        }
        public static void sys16_paletteram_w(int offset, int data)
        {
            ushort oldword = Mame.paletteram.READ_WORD(offset);
            ushort newword = (ushort)Mame.COMBINE_WORD(oldword, data);
            if (oldword != newword)
            {
                /* we can do this, because we initialize palette RAM to all black in vh_start */

                /*	   byte 0    byte 1 */
                /*	GBGR BBBB GGGG RRRR */
                /*	5444 3210 3210 3210 */

                byte r = (byte)((newword & 0x00f) << 1);
                byte g = (byte)((newword & 0x0f0) >> 2);
                byte b = (byte)((newword & 0xf00) >> 7);

                if (sys16_dactype == 0)
                {
                    /* dac_type == 0 (from GCS file) */
                    if ((newword & 0x1000) != 0) r |= 1;
                    if ((newword & 0x2000) != 0) g |= 2;
                    if ((newword & 0x8000) != 0) g |= 1;
                    if ((newword & 0x4000) != 0) b |= 1;
                }
                else if (sys16_dactype == 1)
                {
                    /* dac_type == 1 (from GCS file) Shinobi Only*/
                    if ((newword & 0x1000) != 0) r |= 1;
                    if ((newword & 0x4000) != 0) g |= 2;
                    if ((newword & 0x8000) != 0) g |= 1;
                    if ((newword & 0x2000) != 0) b |= 1;
                }

#if !TRANSPARENT_SHADOWS
		if(!sys16_freezepalette)
		{
			palette_change_color( offset/2,
				(r << 3) | (r >> 2), /* 5 bits red */
				(g << 2) | (g >> 4), /* 6 bits green */
				(b << 3) | (b >> 2) /* 5 bits blue */
			);
		}
		else
		{
			r=(r << 3) | (r >> 2); /* 5 bits red */
			g=(g << 2) | (g >> 4); /* 6 bits green */
			b=(b << 3) | (b >> 2); /* 5 bits blue */
			sys16_palettedirty[offset/2]=0xff000000+(r<<16)+(g<<8)+b;
		}
#else
                if (Mame.Machine.scrbitmap.depth == 8) /* 8 bit shadows */
                {
                    if (sys16_freezepalette == 0)
                    {
                        Mame.palette_change_color(offset / 2,
                           (byte)((r << 3) | (r >> 3)), /* 5 bits red */
                           (byte)((g << 2) | (g >> 4)), /* 6 bits green */
                           (byte)((b << 3) | (b >> 3)) /* 5 bits blue */
                        );
                    }
                    else
                    {
                        r = (byte)((r << 3) | (r >> 3)); /* 5 bits red */
                        g = (byte)((g << 2) | (g >> 4)); /* 6 bits green */
                        b = (byte)((b << 3) | (b >> 3)); /* 5 bits blue */
                        sys16_palettedirty[offset / 2] = (int)(0xff000000 + (r << 16) + (g << 8) + b);
                    }
                }
                else
                {
                    if (sys16_freezepalette == 0)
                    {
                        r = (byte)((r << 3) | (r >> 2)); /* 5 bits red */
                        g = (byte)((g << 2) | (g >> 4)); /* 6 bits green */
                        b = (byte)((b << 3) | (b >> 2)); /* 5 bits blue */

                        Mame.palette_change_color(offset / 2, r, g, b);

                        /* shadow color */

                        r = (byte)(r * 160 / 256);
                        g = (byte)(g * 160 / 256);
                        b = (byte)(b * 160 / 256);

                        Mame.palette_change_color((int)(offset / 2 + Mame.Machine.drv.total_colors / 2), r, g, b);
                    }
                    else
                    {
                        r = (byte)((r << 3) | (r >> 3)); /* 5 bits red */
                        g = (byte)((g << 2) | (g >> 4)); /* 6 bits green */
                        b = (byte)((b << 3) | (b >> 3)); /* 5 bits blue */
                        sys16_palettedirty[offset / 2] = (int)(0xff000000 + (r << 16) + (g << 8) + b);

                        r = (byte)(r * 160 / 256);
                        g = (byte)(g * 160 / 256);
                        b = (byte)(b * 160 / 256);
                        sys16_palettedirty[offset / 2 + Mame.Machine.drv.total_colors / 2] = (int)(0xff000000 + (r << 16) + (g << 8) + b);
                    }
                }
#endif
                Mame.paletteram.WRITE_WORD(offset, newword);
            }
        }
        public override void driver_init()
        {
            throw new NotImplementedException();
        }
        public void SYS16_COINAGE()
        {
            PORT_DIPNAME(0x0f, 0x0f, ipdn_defaultstrings["Coin A"]);
            PORT_DIPSETTING(0x07, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x09, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x05, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x04, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0x0f, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x01, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x02, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x03, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x06, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0x0e, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0x0d, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0x0c, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0x0b, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0x0a, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPSETTING(0x00, "Free Play (if Coin B too) or 1/1");
            PORT_DIPNAME(0xf0, 0xf0, ipdn_defaultstrings["Coin B"]);
            PORT_DIPSETTING(0x70, ipdn_defaultstrings["4C_1C"]);
            PORT_DIPSETTING(0x80, ipdn_defaultstrings["3C_1C"]);
            PORT_DIPSETTING(0x90, ipdn_defaultstrings["2C_1C"]);
            PORT_DIPSETTING(0x50, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x40, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0xf0, ipdn_defaultstrings["1C_1C"]);
            PORT_DIPSETTING(0x10, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x20, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x30, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x60, ipdn_defaultstrings["2C_3C"]);
            PORT_DIPSETTING(0xe0, ipdn_defaultstrings["1C_2C"]);
            PORT_DIPSETTING(0xd0, ipdn_defaultstrings["1C_3C"]);
            PORT_DIPSETTING(0xc0, ipdn_defaultstrings["1C_4C"]);
            PORT_DIPSETTING(0xb0, ipdn_defaultstrings["1C_5C"]);
            PORT_DIPSETTING(0xa0, ipdn_defaultstrings["1C_6C"]);
            PORT_DIPSETTING(0x00, "Free Play (if Coin A too) or 1/1");

        }
        public driver_system16()
        {

        }



        public static void sys16_onetime_init_machine()
        {
            sys16_bg1_trans = 0;
            sys16_rowscroll_scroll = 0;
            sys18_splittab_bg_x = null;
            sys18_splittab_bg_y = null;
            sys18_splittab_fg_x = null;
            sys18_splittab_fg_y = null;

            sys16_quartet_title_kludge = 0;

            sys16_custom_irq = null;

            sys16_MaxShadowColors = NumOfShadowColors;

#if SPACEHARRIER_OFFSETS
	spaceharrier_patternoffsets=0;
#endif
        }
        public static void sys16_sprite_decode2(int num_banks, int bank_size, int side_markers)
        {
            _BytePtr _base = Mame.memory_region(Mame.REGION_GFX2);
            _BytePtr temp = new _BytePtr(bank_size);
            int i;

            for (i = num_banks; i > 0; i--)
            {
                _BytePtr finish = new _BytePtr(_base, 2 * bank_size * i);
                _BytePtr dest = new _BytePtr(finish, -2 * bank_size);

                _BytePtr p1 = new _BytePtr(temp);
                _BytePtr p2 = new _BytePtr(temp, bank_size / 4);
                _BytePtr p3 = new _BytePtr(temp, bank_size / 2);
                _BytePtr p4 = new _BytePtr(temp, bank_size / 4 * 3);

                byte data;

                Buffer.BlockCopy(_base.buffer, (int)(_base.offset + bank_size * (i - 1)), temp.buffer, (int)(temp.offset), (int)bank_size);
                //memcpy (temp, _base+bank_size*(i-1), bank_size);

                /*
                    note: both pen#0 and pen#15 are transparent.
                    we replace references to pen#15 with pen#0, to simplify the sprite rendering
                */
                do
                {
                    data = p4.readinc();// *p4++;
#if SPRITE_SIDE_MARKERS
                    if (side_markers != 0)
                    {
                        if ((data & 0x0f) == 0x0f)
                        {
                            if ((data & 0xf0) != 0xf0 && (data & 0xf0) != 0)
                                dest.writeinc((byte)(data >> 4));
                            //*dest++ = data >> 4;
                            else
                                dest.writeinc(0xff);
                            dest.writeinc(0xff);
                        }
                        else if ((data & 0xf0) == 0xf0)
                        {
                            dest.writeinc(0x00);
                            if ((data & 0x0f) == 0x0f) data &= 0xf0;
                            dest.writeinc((byte)(data & 0xf));
                        }
                        else
                        {
                            dest.writeinc((byte)(data >> 4));
                            dest.writeinc((byte)(data & 0xF));
                        }
                    }
                    else
#endif
                    {
                        if ((data & 0xf0) == 0xf0) data &= 0x0f;
                        if ((data & 0x0f) == 0x0f) data &= 0xf0;
                        dest.writeinc((byte)(data >> 4));
                        dest.writeinc((byte)(data & 0xF));
                    }

                    data = p3.readinc();
#if SPRITE_SIDE_MARKERS
                    if (side_markers != 0)
                    {
                        if ((data & 0x0f) == 0x0f)
                        {
                            if ((data & 0xf0) != 0xf0 && (data & 0xf0) != 0)
                                dest.writeinc((byte)(data >> 4));
                            else
                                dest.writeinc((byte)(0xff));
                            dest.writeinc((byte)(0xff));
                        }
                        else if ((data & 0xf0) == 0xf0)
                        {
                            dest.writeinc((byte)(0x00));
                            if ((data & 0x0f) == 0x0f) data &= 0xf0;
                            dest.writeinc((byte)(data & 0xf));
                        }
                        else
                        {
                            dest.writeinc((byte)(data >> 4));
                            dest.writeinc((byte)(data & 0xF));
                        }
                    }
                    else
#endif
                    {
                        if ((data & 0xf0) == 0xf0) data &= 0x0f;
                        if ((data & 0x0f) == 0x0f) data &= 0xf0;
                        dest.writeinc((byte)(data >> 4));
                        dest.writeinc((byte)(data & 0xF));
                    }


                    data = p2.readinc();
#if SPRITE_SIDE_MARKERS
                    if (side_markers != 0)
                    {
                        if ((data & 0x0f) == 0x0f)
                        {
                            if ((data & 0xf0) != 0xf0 && (data & 0xf0) != 0)
                                dest.writeinc((byte)(data >> 4));
                            else
                                dest.writeinc((byte)(0xff));
                            dest.writeinc((byte)(0xff));
                        }
                        else if ((data & 0xf0) == 0xf0)
                        {
                            dest.writeinc((byte)(0x00));
                            if ((data & 0x0f) == 0x0f) data &= 0xf0;
                            dest.writeinc((byte)(data & 0xf));
                        }
                        else
                        {
                            dest.writeinc((byte)(data >> 4));
                            dest.writeinc((byte)(data & 0xF));
                        }
                    }
                    else
#endif
                    {
                        if ((data & 0xf0) == 0xf0) data &= 0x0f;
                        if ((data & 0x0f) == 0x0f) data &= 0xf0;
                        dest.writeinc((byte)(data >> 4));
                        dest.writeinc((byte)(data & 0xF));
                    }

                    data = p1.readinc();
#if SPRITE_SIDE_MARKERS
                    if (side_markers != 0)
                    {
                        if ((data & 0x0f) == 0x0f)
                        {
                            if ((data & 0xf0) != 0xf0 && (data & 0xf0) != 0)
                                dest.writeinc((byte)(data >> 4));
                            else
                                dest.writeinc((byte)(0xff));
                            dest.writeinc((byte)(0xff));
                        }
                        else if ((data & 0xf0) == 0xf0)
                        {
                            dest.writeinc((byte)(0x00));
                            if ((data & 0x0f) == 0x0f) data &= 0xf0;
                            dest.writeinc((byte)(data & 0xf));
                        }
                        else
                        {
                            dest.writeinc((byte)(data >> 4));
                            dest.writeinc((byte)(data & 0xF));
                        }
                    }
                    else
#endif
                    {
                        if ((data & 0xf0) == 0xf0) data &= 0x0f;
                        if ((data & 0x0f) == 0x0f) data &= 0xf0;
                        dest.writeinc((byte)(data >> 4));
                        dest.writeinc((byte)(data & 0xF));
                    }

                } while (dest.offset < finish.offset);
            }
            //free( temp );
        }

        static int gr_bitmap_width;

        public static void generate_gr_screen(int w, int bitmap_width, int skip, int start_color, int end_color, int source_size)
        {
            _BytePtr buf;
            _BytePtr gr = Mame.memory_region(Mame.REGION_GFX3);
            _BytePtr grr = null;
            int center_offset = 0;

            buf = new _BytePtr(source_size);

            gr_bitmap_width = bitmap_width;
            Buffer.BlockCopy(gr.buffer, gr.offset, buf.buffer, buf.offset, source_size);
            //memcpy(buf,gr,source_size);
            for (int i = 0; i < 256 * bitmap_width; i++) gr[i] = 0;
            //memset(gr,0,256*bitmap_width);

            if (w != gr_bitmap_width)
            {
                if (skip > 0) // needs mirrored RHS
                    grr = gr;
                else
                {
                    center_offset = (gr_bitmap_width - w);
                    gr.offset += center_offset / 2;
                }
            }

            // build gr_bitmap
            for (int i = 0; i < 256; i++)
            {
                byte last_bit;
                byte[] color_data = new byte[4];

                color_data[0] = (byte)start_color; color_data[1] = (byte)(start_color + 1);
                color_data[2] = (byte)(start_color + 2); color_data[3] = (byte)(start_color + 3);
                last_bit = (byte)((((buf[0] & 0x80) == 0) ? 1 : 0) | ((((buf[0x4000] & 0x80) == 0) ? 1 : 0) << 1));
                for (int j = 0; j < w / 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        byte bit = (byte)((((buf[0] & 0x80) == 0) ? 1 : 0) | ((((buf[0x4000] & 0x80) == 0) ? 1 : 0) << 1));
                        if (bit != last_bit && bit == 0 && i > 1)
                        { // color flipped to 0,advance color[0]
                            if (color_data[0] + end_color <= end_color)
                            {
                                color_data[0] += (byte)end_color;
                            }
                            else
                            {
                                color_data[0] -= (byte)end_color;
                            }
                        }
                        gr[0] = color_data[bit];
                        last_bit = bit;
                        buf[0] <<= 1; buf[0x4000] <<= 1; gr.offset++;
                    }
                    buf.offset++;
                }

                if (grr != null)
                { // need mirrored RHS
                    _BytePtr _gr = new _BytePtr(gr, -1);
                    _gr.offset -= skip;
                    for (int j = 0; j < w - skip; j++)
                    {
                        gr.writeinc(_gr.readdec());
                    }
                    for (int j = 0; j < skip; j++) gr.writeinc(0);
                }
                else if (center_offset != 0)
                {
                    gr.offset += center_offset;
                }
            }

            int ii = 1;
            while ((1 << ii) < gr_bitmap_width) ii++;
            gr_bitmap_width = ii; // power of 2

        }



        delegate void sys16_cust_irq();
        static sys16_cust_irq sys16_custom_irq;

        public static int sys16_interrupt()
        {
            if (sys16_custom_irq != null) sys16_custom_irq();
            return 4; /* Interrupt vector 4, used by VBlank */
        }
        public static Mame.GfxLayout charlayout1 =
new Mame.GfxLayout(
    8, 8,	/* 8*8 chars */
    8192,	/* 8192 chars */
    3,	/* 3 bits per pixel */
    new uint[] { 0x20000 * 8, 0x10000 * 8, 0 },
    new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
    new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
    8 * 8	/* every sprite takes 8 consecutive bytes */
);

        public static Mame.GfxDecodeInfo[] gfx1 =
        {
            new Mame.GfxDecodeInfo(Mame.REGION_GFX1,0x00000,charlayout1,0,256),
        };
        public static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x01, 0x01, YM2151.YM2151_status_port_0_r ),
	new Mame.IOReadPort( 0xc0, 0xc0, Mame.soundlatch_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        public static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x00, 0x00, YM2151.YM2151_register_port_0_w ),
	new Mame.IOWritePort( 0x01, 0x01, YM2151.YM2151_data_port_0_w ),
	new Mame.IOWritePort( -1 )
};
        public static void set_refresh(int data)
        {
            sys16_refreshenable = data & 0x20;
            sys16_clear_screen = data & 1;
        }

        public static void set_fg_page(int data)
        {
            sys16_fg_page[0] = data >> 12;
            sys16_fg_page[1] = (data >> 8) & 0xf;
            sys16_fg_page[2] = (data >> 4) & 0xf;
            sys16_fg_page[3] = data & 0xf;
        }

        public static void set_bg_page(int data)
        {
            sys16_bg_page[0] = data >> 12;
            sys16_bg_page[1] = (data >> 8) & 0xf;
            sys16_bg_page[2] = (data >> 4) & 0xf;
            sys16_bg_page[3] = data & 0xf;
        }
    }
    class driver_outrun : driver_system16
    {

#if HANGON_DIGITAL_CONTROLS
        static int or_io_brake_r(int offset)
        {
            int data = Mame.input_port_1_r(offset);

            switch (data & 3)
            {
                case 3: return 0xff00;	// both
                case 1: return 0xff00;  // brake
                case 2: return 0x0000;  // accel
                case 0: return 0x0000;  // neither
            }
            return 0x0000;
        }
        static int or_io_acc_steer_r(int offset)
        {
            int data = Mame.input_port_1_r(offset);
            int ret = Mame.input_port_0_r(offset) << 8;

            switch (data & 3)
            {
                case 3: return 0x00 | ret;	// both
                case 1: return 0x00 | ret;  // brake
                case 2: return 0xff | ret;  // accel
                case 0: return 0x00 | ret;  // neither
            }
            return 0x00 | ret;
        }
#else
static int or_io_acc_steer_r( int offset ){ return (input_port_0_r( offset ) << 8) + input_port_1_r( offset ); }
static int or_io_brake_r( int offset ){ return input_port_5_r( offset ) << 8; }
#endif

        static int or_gear = 0;

        static int or_io_service_r(int offset)
        {
            int ret = Mame.input_port_2_r(offset);
            int data = Mame.input_port_1_r(offset);
            if ((data & 4) != 0) or_gear = 0;
            else if ((data & 8) != 0) or_gear = 1;

            if (or_gear != 0) ret |= 0x10;
            else ret &= 0xef;

            return ret;
        }
        static int or_reset2_r(int offset)
        {
            Mame.cpu_set_reset_line(2, Mame.PULSE_LINE);
            return 0;
        }
        static void outrun_sound_write_w(int offset, int data)
        {
            sound_shared_ram[0] = (byte)(data & 0xff);
        }

        static Mame.MemoryReadAddress[] outrun_readmem =
{
	new Mame.MemoryReadAddress( 0x000000, 0x03ffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x060892, 0x060893, or_io_acc_steer_r ),
	new Mame.MemoryReadAddress( 0x060894, 0x060895, or_io_brake_r ),
	new Mame.MemoryReadAddress( 0x060900, 0x060907, sound_shared_ram_r ),		//???
	new Mame.MemoryReadAddress( 0x060000, 0x067fff, Mame.MRA_BANK7 ),

	new Mame.MemoryReadAddress( 0x100000, 0x10ffff, sys16_tileram_r ),
	new Mame.MemoryReadAddress( 0x110000, 0x110fff, sys16_textram_r ),
    
	new Mame.MemoryReadAddress( 0x130000, 0x130fff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0x120000, 0x121fff, Mame.paletteram_word_r ),
    
	new Mame.MemoryReadAddress( 0x140010, 0x140011, or_io_service_r ),
	new Mame.MemoryReadAddress( 0x140014, 0x140015, Mame.input_port_3_r ),
	new Mame.MemoryReadAddress( 0x140016, 0x140017, Mame.input_port_4_r ),
    
	new Mame.MemoryReadAddress( 0x140000, 0x140071, Mame.MRA_BANK5 ),		//io
	new Mame.MemoryReadAddress( 0x200000, 0x23ffff, Mame.MRA_BANK8 ),
	new Mame.MemoryReadAddress( 0x260000, 0x267fff, shared_ram_r ),
	new Mame.MemoryReadAddress( 0xe00000, 0xe00001, or_reset2_r ),

	new Mame.MemoryReadAddress(-1)
};

        static Mame.MemoryWriteAddress[] outrun_writemem =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x03ffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x060900, 0x060907, sound_shared_ram_w ),		//???
	new Mame.MemoryWriteAddress( 0x060000, 0x067fff, Mame.MWA_BANK7,sys16_extraram5 ),
    
	new Mame.MemoryWriteAddress( 0x100000, 0x10ffff, sys16_tileram_w,sys16_tileram ),
	new Mame.MemoryWriteAddress( 0x110000, 0x110fff, sys16_textram_w,sys16_textram ),
    
	new Mame.MemoryWriteAddress( 0x130000, 0x130fff, Mame.MWA_BANK2,sys16_spriteram ),
	new Mame.MemoryWriteAddress( 0x120000, 0x121fff, sys16_paletteram_w, Mame.paletteram ),
    
	new Mame.MemoryWriteAddress( 0x140000, 0x140071, Mame.MWA_BANK5,sys16_extraram3 ),		//io
	new Mame.MemoryWriteAddress( 0x200000, 0x23ffff, Mame.MWA_BANK8 ),
	new Mame.MemoryWriteAddress( 0x260000, 0x267fff, shared_ram_w, shared_ram ),
	new Mame.MemoryWriteAddress( 0xffff06, 0xffff07, outrun_sound_write_w ),
    
	new Mame.MemoryWriteAddress(-1)
    };

        static Mame.MemoryReadAddress[] outrun_readmem2 =
{
	new Mame.MemoryReadAddress( 0x000000, 0x03ffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x060000, 0x067fff, shared_ram_r ),
	new Mame.MemoryReadAddress( 0x080000, 0x09ffff, Mame.MRA_BANK3 ),		// gr
    
	new Mame.MemoryReadAddress(-1)
};

        static Mame.MemoryWriteAddress[] outrun_writemem2 =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x03ffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x060000, 0x067fff, shared_ram_w ),
	new Mame.MemoryWriteAddress( 0x080000, 0x09ffff, Mame.MWA_BANK3,sys16_extraram ),		// gr
    
	new Mame.MemoryWriteAddress(-1)
};

        // Outrun

        static Mame.MemoryReadAddress[] outrun_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xf000, 0xf0ff, segapcm.SEGAPCMReadReg ),
	new Mame.MemoryReadAddress( 0xf100, 0xf7ff, Mame.MRA_NOP ),
	new Mame.MemoryReadAddress( 0xf800, 0xf807, sound2_shared_ram_r ),
	new Mame.MemoryReadAddress( 0xf808, 0xffff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] outrun_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf0ff, segapcm.SEGAPCMWriteReg ),
	new Mame.MemoryWriteAddress( 0xf100, 0xf7ff, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xf800, 0xf807, sound2_shared_ram_w,sound_shared_ram ),
	new Mame.MemoryWriteAddress( 0xf808, 0xffff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static int or_interrupt()
        {
            int intleft = Mame.cpu_getiloops();
            if (intleft != 0) return 2;
            else return 4;
        }
        static void outrun_update_proc()
        {
            int data;
            sys16_fg_scrollx = sys16_textram.READ_WORD(0x0e98);
            sys16_bg_scrollx = sys16_textram.READ_WORD(0x0e9a);
            sys16_fg_scrolly = sys16_textram.READ_WORD(0x0e90);
            sys16_bg_scrolly = sys16_textram.READ_WORD(0x0e92);
            set_fg_page(sys16_textram.READ_WORD(0x0e80));
            set_bg_page(sys16_textram.READ_WORD(0x0e82));

            set_refresh(sys16_extraram5.READ_WORD(0xb6e));
            data = sys16_extraram5.READ_WORD(0xb6c);

            if ((data & 0x2) != 0)
            {
                Mame.osd_led_w(0, 1);
                Mame.osd_led_w(2, 1);
            }
            else
            {
                Mame.osd_led_w(0, 0);
                Mame.osd_led_w(2, 0);
            }

            if ((data & 0x4) != 0)
                Mame.osd_led_w(1, 1);
            else
                Mame.osd_led_w(1, 0);
        }
        class machine_driver_outrun : Mame.MachineDriver
        {
            public machine_driver_outrun()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 12000000, outrun_readmem, outrun_writemem, null, null, or_interrupt, 2));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 4096000, outrun_sound_readmem, outrun_sound_writemem, sound_readport, sound_writeport, Mame.ignore_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 12000000, outrun_readmem2, outrun_writemem2, null, null, sys16_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = 100;
                cpu_slices_per_frame = 4;

                screen_width = 40 * 8;
                screen_height = 28 * 8;
                visible_area = new Mame.rectangle(0 * 8, 40 * 8 - 1, 0 * 8, 28 * 8 - 1);
                gfxdecodeinfo = driver_outrun.gfx1;
                total_colors = 4096 * ShadowColorsMultiplier;
                color_table_len = 4096 * ShadowColorsMultiplier;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE | Mame.VIDEO_UPDATE_AFTER_VBLANK;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SEGAPCM, segapcm_interface_15k));
            }
            public override void init_machine()
            {
                int[] bank = { 07, 00, 01, 04, 05, 02, 03, 06, 00, 00, 00, 00, 00, 00, 00, 00 };
                sys16_obj_bank = bank;
                sys16_spritesystem = 7;
                sys16_textlayer_lo_min = 0;
                sys16_textlayer_lo_max = 0;
                sys16_textlayer_hi_min = 0;
                sys16_textlayer_hi_max = 0xff;
                sys16_sprxoffset = -0xc0;

                // cpu 0 reset opcode resets cpu 2?
                patch_code(0x7d44, 0x4a);
                patch_code(0x7d45, 0x79);
                patch_code(0x7d46, 0x00);
                patch_code(0x7d47, 0xe0);
                patch_code(0x7d48, 0x00);
                patch_code(0x7d49, 0x00);

                // *forced sound cmd
                patch_code(0x55ed, 0x00);

                // rogue tile on music selection screen
                //	patch_code( 0x38545, 0x80);

                // *freeze time
                //	patch_code( 0xb6b6, 0x4e);
                //	patch_code( 0xb6b7, 0x71);

                Mame.cpu_setbank(8, Mame.memory_region(Mame.REGION_CPU3));

                sys16_update_proc = outrun_update_proc;

                gr_ver = new _BytePtr(sys16_extraram);
                gr_hor = new _BytePtr(gr_ver, 0x400);
                gr_flip = new _BytePtr(gr_ver, 0xc00);
                gr_palette = 0xf00 / 2;
                gr_palette_default = 0x800 / 2;
                gr_colorflip[0, 0] = 0x08 / 2;
                gr_colorflip[0, 1] = 0x04 / 2;
                gr_colorflip[0, 2] = 0x00 / 2;
                gr_colorflip[0, 3] = 0x00 / 2;
                gr_colorflip[1, 0] = 0x0a / 2;
                gr_colorflip[1, 1] = 0x06 / 2;
                gr_colorflip[1, 2] = 0x02 / 2;
                gr_colorflip[1, 3] = 0x00 / 2;

                gr_second_road = new _BytePtr(sys16_extraram, 0x10000);
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
            }
            public override int vh_start()
            {
                return driver_system16.sys16_vh_start();
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            sys16_onetime_init_machine();
            sys16_sprite_decode2(4, 0x040000, 0);
            generate_gr_screen(512, 2048, 0, 0, 3, 0x8000);
        }
        Mame.RomModule[] rom_outrun()
        {
            ROM_START("outrun");
            ROM_REGION(0x040000, Mame.REGION_CPU1);/* 68000 code */
            ROM_LOAD_EVEN("10380a", 0x000000, 0x10000, 0x434fadbc);
            ROM_LOAD_ODD("10382a", 0x000000, 0x10000, 0x1ddcc04e);
            ROM_LOAD_EVEN("10381a", 0x020000, 0x10000, 0xbe8c412b);
            ROM_LOAD_ODD("10383a", 0x020000, 0x10000, 0xdcc586e7);

            ROM_REGION(0x30000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE); /* tiles */
            ROM_LOAD("10268", 0x00000, 0x08000, 0x95344b04);
            ROM_LOAD("10232", 0x08000, 0x08000, 0x776ba1eb);
            ROM_LOAD("10267", 0x10000, 0x08000, 0xa85bb823);
            ROM_LOAD("10231", 0x18000, 0x08000, 0x8908bcbf);
            ROM_LOAD("10266", 0x20000, 0x08000, 0x9f6f1a74);
            ROM_LOAD("10230", 0x28000, 0x08000, 0x686f5e50);

            ROM_REGION(0x100000 * 2, Mame.REGION_GFX2); /* sprites */
            ROM_LOAD("10371", 0x000000, 0x010000, 0x0a1c98de);
            ROM_CONTINUE(0x080000, 0x010000);
            ROM_LOAD("10373", 0x010000, 0x010000, 0x339f8e64);
            ROM_CONTINUE(0x090000, 0x010000);
            ROM_LOAD("10375", 0x020000, 0x010000, 0x62a472bd);
            ROM_CONTINUE(0x0a0000, 0x010000);
            ROM_LOAD("10377", 0x030000, 0x010000, 0xc86daecb);
            ROM_CONTINUE(0x0b0000, 0x010000);

            ROM_LOAD("10372", 0x040000, 0x010000, 0x1640ad1f);
            ROM_CONTINUE(0x0c0000, 0x010000);
            ROM_LOAD("10374", 0x050000, 0x010000, 0x22744340);
            ROM_CONTINUE(0x0d0000, 0x010000);
            ROM_LOAD("10376", 0x060000, 0x010000, 0x8337ace7);
            ROM_CONTINUE(0x0e0000, 0x010000);
            ROM_LOAD("10378", 0x070000, 0x010000, 0x544068fd);
            ROM_CONTINUE(0x0f0000, 0x010000);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* sound CPU */
            ROM_LOAD("10187", 0x00000, 0x008000, 0xa10abaa9);

            ROM_REGION(0x38000, Mame.REGION_SOUND1); /* Sega PCM sound data */
            ROM_LOAD("10193", 0x00000, 0x008000, 0xbcd10dde);
            ROM_RELOAD(0x30000, 0x008000); // twice??
            ROM_LOAD("10192", 0x08000, 0x008000, 0x770f1270);
            ROM_LOAD("10191", 0x10000, 0x008000, 0x20a284ab);
            ROM_LOAD("10190", 0x18000, 0x008000, 0x7cab70e2);
            ROM_LOAD("10189", 0x20000, 0x008000, 0x01366b54);
            ROM_LOAD("10188", 0x28000, 0x008000, 0xbad30ad9);

            ROM_REGION(0x40000, Mame.REGION_CPU3); /* second 68000 CPU */
            ROM_LOAD_EVEN("10327a", 0x00000, 0x10000, 0xe28a5baf);
            ROM_LOAD_ODD("10329a", 0x00000, 0x10000, 0xda131c81);
            ROM_LOAD_EVEN("10328a", 0x20000, 0x10000, 0xd5ec5e5d);
            ROM_LOAD_ODD("10330a", 0x20000, 0x10000, 0xba9ec82a);

            ROM_REGION(0x80000, Mame.REGION_GFX3);/* Road Graphics  (region size should be gr_bitmapwidth*256 )*/
            ROM_LOAD("10185", 0x0000, 0x8000, 0x22794426);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_outrun()
        {

            INPUT_PORTS_START("outrun");
            PORT_START();	/* Steering */
            PORT_ANALOG(0xff, 0x80, (int)inptports.IPT_AD_STICK_X | IPF_CENTER, 100, 3, 0x48, 0xb8);
            //	PORT_ANALOG( 0xff, 0x7f, IPT_PADDLE , 70, 3, 0x48, 0xb8 )

#if HANGON_DIGITAL_CONTROLS

            PORT_START();	/* Buttons */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON1);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (int)inptports.IPT_BUTTON4);

#else

PORT_START	/* Accel / Decel */
	PORT_ANALOG( 0xff, 0x30, IPT_AD_STICK_Y | IPF_CENTER | IPF_REVERSE, 100, 16, 0x30, 0x90 )

#endif

            PORT_START();
            PORT_BIT(0x01, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BITX(0x02, IP_ACTIVE_LOW, (int)inptports.IPT_SERVICE, ipdn_defaultstrings["Service Mode"], (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (int)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (int)inptports.IPT_START1);
            //	PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON3 )
            PORT_BIT(0x20, IP_ACTIVE_LOW, (int)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (int)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (int)inptports.IPT_COIN2);

            SYS16_COINAGE();

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x02, ipdn_defaultstrings["Cabinet"]);
            PORT_DIPSETTING(0x02, "Up Cockpit");
            PORT_DIPSETTING(0x01, "Mini Up");
            PORT_DIPSETTING(0x03, "Moving");
            //	PORT_DIPSETTING(    0x00,"No Use")
            PORT_DIPNAME(0x04, 0x00, ipdn_defaultstrings["Demo Sounds"]);
            PORT_DIPSETTING(0x04, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_DIPNAME(0x08, 0x08, ipdn_defaultstrings["Unused"]);
            PORT_DIPSETTING(0x08, ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING(0x00, ipdn_defaultstrings["On"]);
            PORT_DIPNAME(0x30, 0x30, "Time");
            PORT_DIPSETTING(0x20, "Easy");
            PORT_DIPSETTING(0x30, "Normal");
            PORT_DIPSETTING(0x10, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0xc0, 0xc0, "Enemies");
            PORT_DIPSETTING(0x80, "Easy");
            PORT_DIPSETTING(0xc0, "Normal");
            PORT_DIPSETTING(0x40, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");


#if !HANGON_DIGITAL_CONTROLS

PORT_START	/* Brake */
	PORT_ANALOG( 0xff, 0x30, IPT_AD_STICK_Y | IPF_PLAYER2 | IPF_CENTER | IPF_REVERSE, 100, 16, 0x30, 0x90 )

#endif

            return INPUT_PORTS_END;
        }
        public driver_outrun()
        {
            drv = new machine_driver_outrun();
            year = "1986";
            name = "outrun";
            description = "Out Run (set 1)";
            manufacturer = "Sega";
            flags = Mame.ROT0;
            input_ports = input_ports_outrun();
            rom = rom_outrun();
            drv.HasNVRAMhandler = false;
        }
    }
}
