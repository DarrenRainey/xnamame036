using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_blockout : Mame.GameDriver
    {
        static _BytePtr blockout_videoram = new _BytePtr(1);
        static _BytePtr blockout_frontvideoram = new _BytePtr(1);
        static _BytePtr blockout_frontcolor = new _BytePtr(1);
        static _BytePtr ram_blockout = new _BytePtr(1);

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x000000, 0x03ffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x100000, 0x10000b, blockout_input_r ),
	new Mame.MemoryReadAddress( 0x180000, 0x1bffff, blockout_videoram_r ),
	new Mame.MemoryReadAddress( 0x1d4000, 0x1dffff, Mame.MRA_BANK1 ),	/* work RAM */
	new Mame.MemoryReadAddress( 0x1f4000, 0x1fffff, Mame.MRA_BANK2 ),	/* work RAM */
	new Mame.MemoryReadAddress( 0x200000, 0x207fff, blockout_frontvideoram_r ),
	new Mame.MemoryReadAddress( 0x208000, 0x21ffff, Mame.MRA_BANK3 ),	/* ??? */
	new Mame.MemoryReadAddress( 0x280200, 0x2805ff, Mame.paletteram_word_r ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x000000, 0x03ffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x100014, 0x100017, blockout_sound_command_w ),
	new Mame.MemoryWriteAddress( 0x180000, 0x1bffff, blockout_videoram_w, blockout_videoram ),
	new Mame.MemoryWriteAddress( 0x1d4000, 0x1dffff, Mame.MWA_BANK1, ram_blockout ),	/* work RAM */
	new Mame.MemoryWriteAddress( 0x1f4000, 0x1fffff, Mame.MWA_BANK2 ),	/* work RAM */
	new Mame.MemoryWriteAddress( 0x200000, 0x207fff, blockout_frontvideoram_w, blockout_frontvideoram ),
	new Mame.MemoryWriteAddress( 0x208000, 0x21ffff, Mame.MWA_BANK3 ),	/* ??? */
	new Mame.MemoryWriteAddress( 0x280002, 0x280003, blockout_frontcolor_w ),
	new Mame.MemoryWriteAddress( 0x280200, 0x2805ff, blockout_paletteram_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x8801, 0x8801, YM2151.YM2151_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x9800, 0x9800, okim6295.OKIM6295_status_0_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xa000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x8800, 0x8800, YM2151.YM2151_register_port_0_w ),
	new Mame.MemoryWriteAddress( 0x8801, 0x8801, YM2151.YM2151_data_port_0_w ),
	new Mame.MemoryWriteAddress( 0x9800, 0x9800, okim6295.OKIM6295_data_0_w),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static YM2151interface ym2151_interface =
new YM2151interface(
    1,			/* 1 chip */
    3579545,	/* 3.579545 Mhz (?) */
    new int[] { YM2151.YM3012_VOL(60, Mame.MIXER_PAN_LEFT, 60, Mame.MIXER_PAN_RIGHT) },
    new YM2151irqhandler[] { blockout_irq_handler },
    new Mame.mem_write_handler[]{null}
);

        static OKIM6295interface okim6295_interface =
        new OKIM6295interface(
            1,                  /* 1 chip */
            new int[] { 8000 },           /* 8000Hz frequency */
            new int[] { Mame.REGION_SOUND1 },              /* memory region 3 */
            new int[] { 50 }
        );

        static void blockout_irq_handler(int irq)
        {
            Mame.cpu_set_irq_line(1, 0, irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
            /* cpu_cause_interrupt(1,0xff); */
        }
        static int blockout_interrupt()
        {
            /* interrupt 6 is vblank */
            /* interrupt 5 reads coin inputs - might have to be triggered only */
            /* when a coin is inserted */
            return 6 - Mame.cpu_getiloops();
        }
        static void updatepixels(int x, int y)
        {
            int color;

            if (x < Mame.Machine.drv.visible_area.min_x ||
                    x > Mame.Machine.drv.visible_area.max_x ||
                    y < Mame.Machine.drv.visible_area.min_y ||
                    y > Mame.Machine.drv.visible_area.max_y)
                return;

            int front = blockout_videoram.READ_WORD(y * 512 + x);
            int back = blockout_videoram.READ_WORD(0x20000 + y * 512 + x);

            if ((front >> 8) != 0) color = front >> 8;
            else color = (back >> 8) + 256;
            Mame.plot_pixel(Generic.tmpbitmap, x, y, Mame.Machine.pens[color]);

            if ((front & 0xff) != 0) color = front & 0xff;
            else color = (back & 0xff) + 256;
            Mame.plot_pixel(Generic.tmpbitmap, x + 1, y, Mame.Machine.pens[color]);
        }
        static void blockout_frontvideoram_w(int offset, int data)
        {
            Mame.COMBINE_WORD_MEM(blockout_frontvideoram, offset, data);
        }
        static int blockout_frontvideoram_r(int offset)
        {
            return blockout_frontvideoram.READ_WORD(offset);
        }
        static void blockout_videoram_w(int offset, int data)
        {
            int oldword = blockout_videoram.READ_WORD(offset);
            int newword = Mame.COMBINE_WORD(oldword, data);

            if (oldword != newword)
            {
                blockout_videoram.WRITE_WORD(offset, (ushort)newword);
                updatepixels(offset % 512, (offset / 512) % 256);
            }
        }
        static int blockout_videoram_r(int offset)
        {
            return blockout_videoram.READ_WORD(offset);
        }
        static int blockout_input_r(int offset)
        {
            switch (offset)
            {
                case 0:
                    return Mame.input_port_0_r(offset);
                case 2:
                    return Mame.input_port_1_r(offset);
                case 4:
                    return Mame.input_port_2_r(offset);
                case 6:
                    return Mame.input_port_3_r(offset);
                case 8:
                    return Mame.input_port_4_r(offset);
                default:
                    Mame.printf("PC %06x - read input port %06x\n", Mame.cpu_get_pc(), 0x100000 + offset);
                    return 0;
            }
        }
        static void blockout_sound_command_w(int offset, int data)
        {
            switch (offset)
            {
                case 0:
                    Mame.soundlatch_w(offset, data);
                    Mame.cpu_cause_interrupt(1, Mame.cpu_Z80.Z80_NMI_INT);
                    break;
                case 2:
                    /* don't know, maybe reset sound CPU */
                    break;
            }
        }
        static void setcolor(int color, int rgb)
        {
            int bit0, bit1, bit2, bit3;
            int r, g, b;


            /* red component */
            bit0 = (rgb >> 0) & 0x01;
            bit1 = (rgb >> 1) & 0x01;
            bit2 = (rgb >> 2) & 0x01;
            bit3 = (rgb >> 3) & 0x01;
            r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

            /* green component */
            bit0 = (rgb >> 4) & 0x01;
            bit1 = (rgb >> 5) & 0x01;
            bit2 = (rgb >> 6) & 0x01;
            bit3 = (rgb >> 7) & 0x01;
            g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

            /* blue component */
            bit0 = (rgb >> 8) & 0x01;
            bit1 = (rgb >> 9) & 0x01;
            bit2 = (rgb >> 10) & 0x01;
            bit3 = (rgb >> 11) & 0x01;
            b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

            Mame.palette_change_color(color, (byte)r, (byte)g, (byte)b);
        }
        static void blockout_paletteram_w(int offset, int data)
        {
            int oldword = Mame.paletteram.READ_WORD(offset);
            int newword = Mame.COMBINE_WORD(oldword, data);


            Mame.paletteram.WRITE_WORD(offset, (ushort)newword);

            setcolor(offset / 2, newword);
        }
        static void blockout_frontcolor_w(int offset, int data)
        {
            setcolor(512, data);
        }

        class machine_driver_blockout : Mame.MachineDriver
        {
            public machine_driver_blockout()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68000, 8760000, readmem, writemem, null, null, blockout_interrupt, 2));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 3579545, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 320;
                screen_height = 256;
                visible_area = new Mame.rectangle(0, 319, 8, 247);
                gfxdecodeinfo = null;
                total_colors = 513;
                color_table_len = 0;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = Mame.SOUND_SUPPORTS_STEREO;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2151, ym2151_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_OKIM6295, okim6295_interface));
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
                //nothing
            }
            public override int vh_start()
            {
                Generic.tmpbitmap = Mame.osd_new_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height, Mame.Machine.scrbitmap.depth);
                return 0;
            }
            public override void vh_stop()
            {
                Mame.osd_free_bitmap(Generic.tmpbitmap);
                Generic.tmpbitmap = null;
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                if (Mame.palette_recalc() != null)
                {
                    /* if we ran out of palette entries, rebuild the whole screen */
                    for (int y = 0; y < 256; y++)
                    {
                        for (int x = 0; x < 320; x += 2)
                        {
                            updatepixels(x, y);
                        }
                    }
                }

                Mame.copybitmap(bitmap, Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                {
                    int color = Mame.Machine.pens[512];

                    for (int y = 0; y < 256; y++)
                    {
                        for (int x = 0; x < 320; x += 8)
                        {
                            int d = blockout_frontvideoram.READ_WORD(y * 128 + (x / 4));

                            if (d != 0)
                            {
                                if ((d & 0x80) != 0) Mame.plot_pixel(bitmap, x, y, color);
                                if ((d & 0x40) != 0) Mame.plot_pixel(bitmap, x + 1, y, color);
                                if ((d & 0x20) != 0) Mame.plot_pixel(bitmap, x + 2, y, color);
                                if ((d & 0x10) != 0) Mame.plot_pixel(bitmap, x + 3, y, color);
                                if ((d & 0x08) != 0) Mame.plot_pixel(bitmap, x + 4, y, color);
                                if ((d & 0x04) != 0) Mame.plot_pixel(bitmap, x + 5, y, color);
                                if ((d & 0x02) != 0) Mame.plot_pixel(bitmap, x + 6, y, color);
                                if ((d & 0x01) != 0) Mame.plot_pixel(bitmap, x + 7, y, color);
                            }
                        }
                    }
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
        Mame.RomModule[] rom_blockout()
        {
            ROM_START("blockout");
            ROM_REGION(0x40000, Mame.REGION_CPU1);/* 2*128k for 68000 code */
            ROM_LOAD_EVEN("bo29a0-2.bin", 0x00000, 0x20000, 0xb0103427);
            ROM_LOAD_ODD("bo29a1-2.bin", 0x00000, 0x20000, 0x5984d5a2);

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for the audio CPU */
            ROM_LOAD("bo29e3-0.bin", 0x0000, 0x8000, 0x3ea01f78);

            ROM_REGION(0x20000, Mame.REGION_SOUND1);	/* 128k for ADPCM samples - sound chip is OKIM6295 */
            ROM_LOAD("bo29e2-0.bin", 0x0000, 0x20000, 0x15c5a99d);

            ROM_REGION(0x0100, Mame.REGION_PROMS);
            ROM_LOAD("mb7114h.25", 0x0000, 0x0100, 0xb25bbda7);	/* unknown */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_blockout()
        {
            INPUT_PORTS_START("blockout");
            PORT_START();      /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();     /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON4 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_PLAYER2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);

            PORT_START();
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Coinage));
            PORT_DIPSETTING(0x00, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            /* the following two are supposed to control Coin 2, but they don't work. */
            /* This happens on the original board too. */
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Unknown));	/* unused? */
            PORT_DIPSETTING(0x04, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));	/* unused? */
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, "1 Coin to Continue");
            PORT_DIPSETTING(0x10, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x20, DEF_STR(On));
            PORT_DIPNAME(0x40, 0x40, DEF_STR(Unknown));	/* unused? */
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));	/* unused? */
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Normal");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Very Hard");
            PORT_DIPNAME(0x04, 0x04, "Rotate Buttons");
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x04, "3");
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, DEF_STR(Unknown));
            PORT_DIPSETTING(0x10, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Unknown));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            return INPUT_PORTS_END;
        }
        public driver_blockout()
        {
            drv = new machine_driver_blockout();
            year = "1989";
            name = "blockout";
            description = "Block Out (set 1)";
            manufacturer = "Technos + California Dreams";
            flags = Mame.ROT0;
            input_ports = input_ports_blockout();
            rom = rom_blockout();
            drv.HasNVRAMhandler = false;
        }
    }
}
