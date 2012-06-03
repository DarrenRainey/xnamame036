using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_phoenix : Mame.GameDriver
    {
        static byte[] ram_page1;
        static byte[] ram_page2;
        static byte[] current_ram_page;
        static int current_ram_page_index;
        static byte bg_scroll;
        static int palette_bank;
        static int protection_question;
        static int BACKGROUND_VIDEORAM_OFFSET = 0x0800;


        static int phoenix_paged_ram_r(int offset)
        {
            return current_ram_page[offset];
        }
        static void phoenix_paged_ram_w(int offset, int data)
        {
            if ((offset >= BACKGROUND_VIDEORAM_OFFSET) &&
                (offset < BACKGROUND_VIDEORAM_OFFSET + Generic.videoram_size[0]))
            {
                /* Background video RAM */
                if (data != current_ram_page[offset])
                {
                    Generic.dirtybuffer[offset - BACKGROUND_VIDEORAM_OFFSET] = true;
                }
            }

            current_ram_page[offset] = (byte)data;
        }
        static void phoenix_videoreg_w(int offset, int data)
        {
            if (current_ram_page_index != (data & 1))
            {
                /* Set memory bank */
                current_ram_page_index = data & 1;

                current_ram_page = current_ram_page_index != 0 ? ram_page2 : ram_page1;
                Generic.SetDirtyBuffer(true);//memset(dirtybuffer, 1, videoram_size);
            }

            if (palette_bank != ((data >> 1) & 1))
            {
                palette_bank = (data >> 1) & 1;

                Generic.SetDirtyBuffer(true);// memset(dirtybuffer, 1, videoram_size);
            }

            protection_question = data & 0xfc;

            /* I think bits 2 and 3 are used for something else in Pleiads as well,
               they are set in the routine starting at location 0x06bc */

            /* send two bits to sound control C (not sure if they are there) */
            Mame.pleiads.pleiads_sound_control_c_w(offset, data);
        }

        static void phoenix_scroll_w(int offset, int data)
        {
            bg_scroll = (byte)data;
        }
        static int phoenix_input_port_0_r(int offset)
        {
            int ret = Mame.input_port_0_r(0) & 0xf7;

            /* handle Pleiads protection */
            switch (protection_question)
            {
                case 0x00:
                case 0x20:
                    /* Bit 3 is 0 */
                    break;
                case 0x0c:
                case 0x30:
                    /* Bit 3 is 1 */
                    ret |= 0x08;
                    break;
                default:
                    Mame.printf("Unknown protection question %02X at %04X\n", protection_question, Mame.cpu_get_pc());
                    break;
            }

            return ret;
        }

        const int VMIN = 0;
        const int VMAX = 32767;

        static int sound_latch_a;
        static int sound_latch_b;

        static int channel;

        static int tone1_vco1_cap;
        static int tone1_level;
        static int tone2_level;

        static uint[] poly18 = null;
        static void phoenix_sound_control_a_w(int offset, int data)
        {
            if (data == sound_latch_a)
                return;

            Mame.stream_update(channel, 0);
            sound_latch_a = data;

            tone1_vco1_cap = (sound_latch_a >> 4) & 3;
            if ((sound_latch_a & 0x20) != 0)
                tone1_level = VMAX * 10000 / (10000 + 10000);
            else
                tone1_level = VMAX;
        }

        static void phoenix_sound_control_b_w(int offset, int data)
        {
            if (data == sound_latch_b)
                return;

            Mame.stream_update(channel, 0);
            sound_latch_b = data;

            if ((sound_latch_b & 0x20) != 0)
                tone2_level = VMAX * 10 / 11;
            else
                tone2_level = VMAX;

            /* eventually change the tune that the MM6221AA is playing */
            Mame.tms36xx.mm6221aa_tune_w(0, sound_latch_b >> 6);
        }


        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x3fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x4000, 0x4fff, phoenix_paged_ram_r ),	/* 2 pages selected by Bit 0 of videoregister */
	new Mame.MemoryReadAddress( 0x7000, 0x73ff, phoenix_input_port_0_r ), /* IN0 */
	new Mame.MemoryReadAddress( 0x7800, 0x7bff, Mame.input_port_1_r ), 		/* DSW */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] phoenix_writemem =		
{																
	new Mame.MemoryWriteAddress( 0x0000, 0x3fff, Mame.MWA_ROM ),								
	new Mame.MemoryWriteAddress( 0x4000, 0x4fff, phoenix_paged_ram_w ),  /* 2 pages selected by Bit 0 of the video register */ 
	new Mame.MemoryWriteAddress( 0x5000, 0x53ff, phoenix_videoreg_w ), 					
	new Mame.MemoryWriteAddress( 0x5800, 0x5bff, phoenix_scroll_w ),	/* the game sometimes writes at mirror addresses */ 	
	new Mame.MemoryWriteAddress( 0x6000, 0x63ff, phoenix_sound_control_a_w ),			
	new Mame.MemoryWriteAddress( 0x6800, 0x6bff, phoenix_sound_control_b_w ),			
	new Mame.MemoryWriteAddress( -1 )	/* end of table */									
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 256 * 8 * 8, 0 }, /* the two bitplanes are separated */
            new uint[] { 7, 6, 5, 4, 3, 2, 1, 0 }, /* pretty straightforward layout */
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8 /* every char takes 8 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,	  0, 16 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, charlayout, 16*4, 16 ),
};

        //static struct TMS36XXinterface phoenix_tms36xx_interface =
        //{
        //    1,
        //    { 50 }, 		/* mixing levels */
        //    { MM6221AA },	/* TMS36xx subtype(s) */
        //    { 372  },		/* base frequency */
        //    { {0.50,0,0,1.05,0,0} }, /* decay times of voices */
        //    { 0.21 },       /* tune speed (time between beats) */
        //};

        //static struct CustomSound_interface phoenix_custom_interface =
        //{
        //    phoenix_sh_start,
        //    phoenix_sh_stop,
        //    phoenix_sh_update
        //};
        class machine_driver_phoenix : Mame.MachineDriver
        {
            public machine_driver_phoenix()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_8080, 3072000, readmem, phoenix_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 31 * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_phoenix.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 16 * 4 + 16 * 4;

                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                //sound.Add();
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
                //#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
                //#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])
                int cpi = 0;
                int pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int bit0 = (color_prom[cpi] >> 0) & 0x01;
                    int bit1 = (color_prom[(uint)cpi+Mame.Machine.drv.total_colors] >> 0) & 0x01;
                    palette[pi++] = (byte)(0x55 * bit0 + 0xaa * bit1);
                    bit0 = (color_prom[cpi] >> 2) & 0x01;
                    bit1 = (color_prom[(uint)cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                    palette[pi++] = (byte)(0x55 * bit0 + 0xaa * bit1);
                    bit0 = (color_prom[cpi] >> 1) & 0x01;
                    bit1 = (color_prom[(uint)cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                    palette[pi++] = (byte)(0x55 * bit0 + 0xaa * bit1);

                    cpi++;
                }

                /* first bank of characters use colors 0-31 and 64-95 */
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + 4 * i + j * 4 * 8, (ushort)(i + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + 4 * i + j * 4 * 8 + 1, (ushort)(8 + i + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + 4 * i + j * 4 * 8 + 2, (ushort)(2 * 8 + i + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + 4 * i + j * 4 * 8 + 3, (ushort)(3 * 8 + i + j * 64));
                    }
                }

                /* second bank of characters use colors 32-63 and 96-127 */
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + 4 * i + j * 4 * 8, (ushort)(i + 32 + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + 4 * i + j * 4 * 8 + 1, (ushort)(8 + i + 32 + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + 4 * i + j * 4 * 8 + 2, (ushort)(2 * 8 + i + 32 + j * 64));
                        colortable.write16(Mame.Machine.drv.gfxdecodeinfo[1].color_codes_start + 4 * i + j * 4 * 8 + 3, (ushort)(3 * 8 + i + 32 + j * 64));
                    }
                }
            }
            public override int vh_start()
            {
                ram_page1 = new byte[0x1000];

                ram_page2 = new byte[0x1000];

                current_ram_page = null;
                current_ram_page_index = -1;

                Generic.videoram_size[0] = 0x0340;
                return Generic.generic_vh_start();
            }
            public override void vh_stop()
            {
                ram_page1 = null;
                ram_page2 = null;

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
                        int sx, sy, code;

                        Generic.dirtybuffer[offs] = false;

                        code = current_ram_page[offs + BACKGROUND_VIDEORAM_OFFSET];

                        sx = offs % 32;
                        sy = offs / 32;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                (uint)code,
                                (uint)((code >> 5) + 8 * palette_bank),
                                false,false,
                                8 * sx, 8 * sy,
                                null, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the character mapped graphics */
                {
                    int scroll;


                    scroll = -bg_scroll;

                    Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1,new int[] {scroll}, 0, null, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }


                /* draw the frontmost playfield. They are characters, but draw them as sprites */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    int sx, sy, code;

                    code = current_ram_page[offs];

                    sx = offs % 32;
                    sy = offs / 32;

                    if (sx >= 1)
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                               (uint) code,
                                (uint)((code >> 5) + 8 * palette_bank),
                                false,false,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                    else
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)code,
                                (uint)((code >> 5) + 8 * palette_bank),
                                false,false,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        Mame.InputPortTiny[] input_ports_phoenix()
        {

            INPUT_PORTS_START("phoenix");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, IPT_UNUSED);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);

            PORT_START("DSW0");
            PORT_DIPNAME(0x03, 0x00, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "3");
            PORT_DIPSETTING(0x01, "4");
            PORT_DIPSETTING(0x02, "5");
            PORT_DIPSETTING(0x03, "6");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "3000");
            PORT_DIPSETTING(0x04, "4000");
            PORT_DIPSETTING(0x08, "5000");
            PORT_DIPSETTING(0x0c, "6000");
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Coinage"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_1C"));
            PORT_DIPNAME(0x20, 0x20, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_VBLANK);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_phoenix()
        {
            ROM_START("phoenix");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("ic45", 0x0000, 0x0800, 0x9f68086b);
            ROM_LOAD("ic46", 0x0800, 0x0800, 0x273a4a82);
            ROM_LOAD("ic47", 0x1000, 0x0800, 0x3d4284b9);
            ROM_LOAD("ic48", 0x1800, 0x0800, 0xcb5d9915);
            ROM_LOAD("ic49", 0x2000, 0x0800, 0xa105e4e7);
            ROM_LOAD("ic50", 0x2800, 0x0800, 0xac5e9ec1);
            ROM_LOAD("ic51", 0x3000, 0x0800, 0x2eab35b4);
            ROM_LOAD("ic52", 0x3800, 0x0800, 0xaff8e9c5);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ic23", 0x0000, 0x0800, 0x3c7e623f);
            ROM_LOAD("ic24", 0x0800, 0x0800, 0x59916d3b);

            ROM_REGION(0x1000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("ic39", 0x0000, 0x0800, 0x53413e8f);
            ROM_LOAD("ic40", 0x0800, 0x0800, 0x0be2ba91);

            ROM_REGION(0x0200, Mame.REGION_PROMS);
            ROM_LOAD("ic40_b.bin", 0x0000, 0x0100, 0x79350b25);  /* palette low bits */
            ROM_LOAD("ic41_a.bin", 0x0100, 0x0100, 0xe176b768);  /* palette high bits */
            return ROM_END;
        }
        public override void driver_init()
        {
            //none
        }
        public driver_phoenix()
        {
            drv = new machine_driver_phoenix();
            year = "1980";
            name = "phoenix";
            description = "Phoenix (Amstar)";
            manufacturer = "Amstar";
            flags = Mame.ROT90 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_phoenix();
            rom = rom_phoenix();
            drv.HasNVRAMhandler = false;
        }
    }
}
