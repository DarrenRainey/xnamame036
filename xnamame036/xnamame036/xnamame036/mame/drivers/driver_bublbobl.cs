using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_bublbobl : Mame.GameDriver
    {
        static _BytePtr bublbobl_sharedram1 = new _BytePtr(1);
        static _BytePtr bublbobl_sharedram2 = new _BytePtr(1);
        static _BytePtr bublbobl_objectram = new _BytePtr(1);
        static int[] bublbobl_objectram_size = new int[1];

        static Mame.MemoryReadAddress[] bublbobl_readmem =
{
    new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
    new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_BANK1 ),
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, Mame.MRA_RAM ),
    new Mame.MemoryReadAddress( 0xe000, 0xf7ff, bublbobl_sharedram1_r ),
	new Mame.MemoryReadAddress( 0xf800, 0xf9ff, Mame.paletteram_r ),
    new Mame.MemoryReadAddress( 0xfc00, 0xfcff, bublbobl_sharedram2_r ),
    new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] bublbobl_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0xbfff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xc000, 0xdcff, Mame.MWA_RAM, Generic.videoram,Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xdd00, 0xdfff, Mame.MWA_RAM, bublbobl_objectram, bublbobl_objectram_size ),
	new Mame.MemoryWriteAddress( 0xe000, 0xf7ff, bublbobl_sharedram1_w, bublbobl_sharedram1 ),
	new Mame.MemoryWriteAddress( 0xf800, 0xf9ff, Mame.paletteram_RRRRGGGGBBBBxxxx_swap_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0xfa00, 0xfa00, bublbobl_sound_command_w ),
	new Mame.MemoryWriteAddress( 0xfa80, 0xfa80, Mame.watchdog_reset_w ),	/* not sure - could go to the 68705 */
	new Mame.MemoryWriteAddress( 0xfb40, 0xfb40, bublbobl_bankswitch_w ),
	new Mame.MemoryWriteAddress( 0xfc00, 0xfcff, bublbobl_sharedram2_w, bublbobl_sharedram2 ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.MemoryReadAddress[] m68705_readmem =
{
	new Mame.MemoryReadAddress(0x0000, 0x0000, bublbobl_68705_portA_r ),
	new Mame.MemoryReadAddress(0x0001, 0x0001, bublbobl_68705_portB_r ),
	new Mame.MemoryReadAddress(0x0002, 0x0002, Mame.input_port_0_r ),	/* COIN */
	new Mame.MemoryReadAddress(0x0010, 0x007f, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress(0x0080, 0x07ff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress(-1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] m68705_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0000, bublbobl_68705_portA_w ),
	new Mame.MemoryWriteAddress( 0x0001, 0x0001, bublbobl_68705_portB_w ),
	new Mame.MemoryWriteAddress( 0x0004, 0x0004, bublbobl_68705_ddrA_w ),
	new Mame.MemoryWriteAddress( 0x0005, 0x0005, bublbobl_68705_ddrB_w ),
	new Mame.MemoryWriteAddress( 0x0010, 0x007f, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x0080, 0x07ff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static Mame.MemoryReadAddress[] bublbobl_readmem2 =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xe000, 0xf7ff, bublbobl_sharedram1_r ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] bublbobl_writemem2 =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xe000, 0xf7ff, bublbobl_sharedram1_w ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};


        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x8fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9000, 0x9000, ym2203.YM2203_status_port_0_r ),
	new Mame.MemoryReadAddress( 0x9001, 0x9001, ym2203.YM2203_read_port_0_r ),
	new Mame.MemoryReadAddress( 0xa000, 0xa000, YM3812.YM3526_status_port_0_r ),
	new Mame.MemoryReadAddress( 0xb000, 0xb000, Mame.soundlatch_r ),
	new Mame.MemoryReadAddress( 0xb001, 0xb001, Mame.MRA_NOP ),	/* ??? */
	new Mame.MemoryReadAddress( 0xe000, 0xefff,Mame.MRA_ROM ),	/* space for diagnostic ROM? */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x8fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x9000, 0x9000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0x9001, 0x9001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, YM3812.YM3526_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xa001, 0xa001, YM3812.YM3526_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb000, Mame.MWA_NOP ),	/* ??? */
	new Mame.MemoryWriteAddress( 0xb001, 0xb001, bublbobl_sh_nmi_enable_w ),
	new Mame.MemoryWriteAddress( 0xb002, 0xb002, bublbobl_sh_nmi_disable_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, Mame.MWA_ROM ),	/* space for diagnostic ROM? */
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* the characters are 8x8 pixels */
            256 * 8 * 8,	/* 256 chars per bank * 8 banks per ROM pair * 8 ROM pairs */
            4,	/* 4 bits per pixel */
            new uint[] { 0, 4, 8 * 0x8000 * 8, 8 * 0x8000 * 8 + 4 },
            new uint[] { 3, 2, 1, 0, 8 + 3, 8 + 2, 8 + 1, 8 + 0 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8	/* every char takes 16 bytes in two ROMs */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	/* read all graphics into one big graphics region */
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0x00000, charlayout, 0, 16 ),
};
        static void irqhandler(int irq)
        {
            Mame.cpu_set_irq_line(2, 0, irq != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        static int bublbobl_sharedram1_r(int offset)
        {
            return bublbobl_sharedram1[offset];
        }
        static int bublbobl_sharedram2_r(int offset)
        {
            return bublbobl_sharedram2[offset];
        }
        static void bublbobl_sharedram1_w(int offset, int data)
        {
            bublbobl_sharedram1[offset] = (byte)data;
        }
        static void bublbobl_sharedram2_w(int offset, int data)
        {
            bublbobl_sharedram2[offset] = (byte)data;
        }
        static int sound_nmi_enable, pending_nmi;
        static void nmi_callback(int param)
        {
            if (sound_nmi_enable != 0) Mame.cpu_cause_interrupt(2, Mame.cpu_Z80.Z80_NMI_INT);
            else pending_nmi = 1;
        }
        static void bublbobl_sound_command_w(int offset, int data)
        {
            Mame.soundlatch_w(offset, data);
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, data, nmi_callback);
        }
        static void bublbobl_sh_nmi_disable_w(int offset, int data)
        {
            sound_nmi_enable = 0;
        }
        static void bublbobl_sh_nmi_enable_w(int offset, int data)
        {
            sound_nmi_enable = 1;
            if (pending_nmi != 0)	/* probably wrong but commands go lost otherwise */
            {
                Mame.cpu_cause_interrupt(2, Mame.cpu_Z80.Z80_NMI_INT);
                pending_nmi = 0;
            }
        }
        static void bublbobl_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            if ((data & 3) == 0) { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x8000)); }
            else { Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000 + 0x4000 * ((data & 3) - 1))); }
        }
        static int bublbobl_m68705_interrupt()
        {
            /* I don't know how to handle the interrupt line so I just toggle it every time. */
            if ((Mame.cpu_getiloops() & 1) != 0)
                Mame.cpu_set_irq_line(3, 0, Mame.CLEAR_LINE);
            else
                Mame.cpu_set_irq_line(3, 0, Mame.ASSERT_LINE);

            return 0;
        }

        static byte portA_in, portA_out, ddrA;

        static int bublbobl_68705_portA_r(int offset)
        {
            //if (errorlog) fprintf(errorlog,"%04x: 68705 port A read %02x\n",cpu_get_pc(),portA_in);
            return (portA_out & ddrA) | (portA_in & ~ddrA);
        }
        static void bublbobl_68705_portA_w(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"%04x: 68705 port A write %02x\n",cpu_get_pc(),data);
            portA_out = (byte)data;
        }
        static void bublbobl_68705_ddrA_w(int offset, int data)
        {
            ddrA = (byte)data;
        }
        static byte portB_in, portB_out, ddrB;
        static int bublbobl_68705_portB_r(int offset)
        {
            return (portB_out & ddrB) | (portB_in & ~ddrB);
        }

        static int address, latch;

        static void bublbobl_68705_portB_w(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"%04x: 68705 port B write %02x\n",cpu_get_pc(),data);

            if ((ddrB & 0x01) != 0 && (~data & 0x01) != 0 && (portB_out & 0x01) != 0)
            {
                portA_in = (byte)latch;
            }
            if ((ddrB & 0x02) != 0 && (data & 0x02) != 0 && (~portB_out & 0x02) != 0) /* positive edge trigger */
            {
                address = (address & 0xff00) | portA_out;
                //if (errorlog) fprintf(errorlog,"%04x: 68705 address %02x\n",cpu_get_pc(),portA_out);
            }
            if ((ddrB & 0x04) != 0 && (data & 0x04) != 0 && (~portB_out & 0x04) != 0) /* positive edge trigger */
            {
                address = (address & 0x00ff) | ((portA_out & 0x0f) << 8);
            }
            if ((ddrB & 0x10) != 0 && (~data & 0x10) != 0 && (portB_out & 0x10) != 0)
            {
                if ((data & 0x08) != 0)	/* read */
                {
                    if ((address & 0x0f00) == 0x0000)
                    {
                        //if (errorlog) fprintf(errorlog,"%04x: 68705 read input port %02x\n",cpu_get_pc(),address);
                        latch = Mame.readinputport((address & 3) + 1);
                    }
                    else if ((address & 0x0f00) == 0x0c00)
                    {
                        //if (errorlog) fprintf(errorlog,"%04x: 68705 read %02x from address %04x\n",cpu_get_pc(),bublbobl_sharedram2[address],address);
                        latch = bublbobl_sharedram2[address & 0x00ff];
                    }
                    //else
                      //  Mame.printf("%04x: 68705 unknown read address %04x\n", Mame.cpu_get_pc(), address);
                }
                else	/* write */
                {
                    if ((address & 0x0f00) == 0x0c00)
                    {
                        //if (errorlog) fprintf(errorlog,"%04x: 68705 write %02x to address %04x\n",cpu_get_pc(),portA_out,address);
                        bublbobl_sharedram2[address & 0x00ff] = portA_out;
                    }
                    else
                        Mame.printf("%04x: 68705 unknown write to address %04x\n", Mame.cpu_get_pc(), address);
                }
            }
            if ((ddrB & 0x20) != 0 && (~data & 0x20) != 0 && (portB_out & 0x20) != 0)
            {
                /* hack to get random EXTEND letters (who is supposed to do this? 68705? PAL?) */
                bublbobl_sharedram2[0x7c] = (byte)(Mame.rand() % 6);

                Mame.cpu_irq_line_vector_w(0, 0, bublbobl_sharedram2[0]);
                Mame.cpu_set_irq_line(0, 0, Mame.HOLD_LINE);
            }
            if ((ddrB & 0x40) != 0 && (~data & 0x40) != 0 && (portB_out & 0x40) != 0)
            {
                Mame.printf("%04x: 68705 unknown port B bit %02x\n", Mame.cpu_get_pc(), data);
            }
            if ((ddrB & 0x80) != 0 && (~data & 0x80) != 0 && (portB_out & 0x80) != 0)
            {
                Mame.printf("%04x: 68705 unknown port B bit %02x\n", Mame.cpu_get_pc(), data);
            }

            portB_out = (byte)data;
        }
        static void bublbobl_68705_ddrB_w(int offset, int data)
        {
            ddrB = (byte)data;
        }
        static YM2203interface ym2203_interface =
new YM2203interface(
	1,			/* 1 chip */
	3000000,	/* 3 MHz ??? (hand tuned) */
	new int[]{ ym2203.YM2203_VOL(25,25) },
new AY8910portRead[]{null},
	new AY8910portRead[]{ null },
	new AY8910portWrite[]{ null },
	new AY8910portWrite[]{ null },
	new AY8910handler[]{ irqhandler }
);


static YM3526interface ym3526_interface =
new YM3526interface(
	1,			/* 1 chip (no more supported) */
	3000000,	/* 3 MHz ??? (hand tuned) */
	new int[]{ 255 }		/* (not supported) */
);
        class machine_driver_bublbobl : Mame.MachineDriver
        {
            public machine_driver_bublbobl()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 6000000, bublbobl_readmem, bublbobl_writemem, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 6000000, bublbobl_readmem2, bublbobl_writemem2, null, null, Mame.interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 4000000, sound_readmem, sound_writemem, null, null, Mame.ignore_interrupt, 0));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M68705, 4000000 / 2, m68705_readmem, m68705_writemem, null, null, bublbobl_m68705_interrupt, 2));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_bublbobl.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM3526, ym3526_interface));

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
                for (int i = 0; i < Mame.Machine.drv.color_table_len; i++)
                    colortable[i] = (ushort)(i ^ 0x0f);
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
                int sx, sy, xc, yc;
                int gfx_num, gfx_attr, gfx_offs;

                Mame.palette_recalc();
                /* no need to check the return code since we redraw everything each frame */


                /* Bubble Bobble doesn't have a real video RAM. All graphics (characters */
                /* and sprites) are stored in the same memory region, and information on */
                /* the background character columns is stored inthe area dd00-dd3f */

                /* This clears & redraws the entire screen each pass */
                Mame.fillbitmap(bitmap, Mame.Machine.gfx[0].colortable[0], Mame.Machine.drv.visible_area);

                sx = 0;
                for (int offs = 0; offs < bublbobl_objectram_size[0]; offs += 4)
                {
                    int height;

                    /* skip empty sprites */
                    /* this is dword aligned so the UINT32 * cast shouldn't give problems */
                    /* on any architecture */
                    if (bublbobl_objectram.read32(offs) == 0)
                        continue;

                    gfx_num = bublbobl_objectram[offs + 1];
                    gfx_attr = bublbobl_objectram[offs + 3];

                    if ((gfx_num & 0x80) == 0)	/* 16x16 sprites */
                    {
                        gfx_offs = ((gfx_num & 0x1f) * 0x80) + ((gfx_num & 0x60) >> 1) + 12;
                        height = 2;
                    }
                    else	/* tilemaps (each sprite is a 16x256 column) */
                    {
                        gfx_offs = ((gfx_num & 0x3f) * 0x80);
                        height = 32;
                    }

                    if ((gfx_num & 0xc0) == 0xc0)	/* next column */
                        sx += 16;
                    else
                    {
                        sx = bublbobl_objectram[offs + 2];
                        if ((gfx_attr & 0x40) !=0)sx -= 256;
                    }
                    sy = 256 - height * 8 - (bublbobl_objectram[offs + 0]);

                    for (xc = 0; xc < 2; xc++)
                    {
                        for (yc = 0; yc < height; yc++)
                        {
                            int goffs, code, color, flipx, flipy, x, y;

                            goffs = gfx_offs + xc * 0x40 + yc * 0x02;
                            code = Generic.videoram[goffs] + 256 * (Generic.videoram[goffs + 1] & 0x03) + 1024 * (gfx_attr & 0x0f);
                            color = (Generic.videoram[goffs + 1] & 0x3c) >> 2;
                            flipx = Generic.videoram[goffs + 1] & 0x40;
                            flipy = Generic.videoram[goffs + 1] & 0x80;
                            x = sx + xc * 8;
                            y = (sy + yc * 8) & 0xff;

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                    (uint)code,
                                    (uint)color,
                                    flipx!=0, flipy!=0,
                                    x, y,
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
        Mame.RomModule[] rom_bublbobl()
        {
            ROM_START("bublbobl");
            ROM_REGION(0x1c000, Mame.REGION_CPU1);	/* 64k+64k for the first CPU */
            ROM_LOAD("a78-06.51", 0x00000, 0x8000, 0x32c8305b);
            ROM_LOAD("a78-05.52", 0x08000, 0x4000, 0x53f4bc6e);	/* banked at 8000-bfff. I must load */
            ROM_CONTINUE(0x10000, 0xc000);			/* bank 0 at 8000 because the code falls into */
            /* it from 7fff, so bank switching wouldn't work */
            ROM_REGION(0x80000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a78-09.12", 0x00000, 0x8000, 0x20358c22);    /* 1st plane */
            ROM_LOAD("a78-10.13", 0x08000, 0x8000, 0x930168a9);
            ROM_LOAD("a78-11.14", 0x10000, 0x8000, 0x9773e512);
            ROM_LOAD("a78-12.15", 0x18000, 0x8000, 0xd045549b);
            ROM_LOAD("a78-13.16", 0x20000, 0x8000, 0xd0af35c5);
            ROM_LOAD("a78-14.17", 0x28000, 0x8000, 0x7b5369a8);
            /* 0x30000-0x3ffff empty */
            ROM_LOAD("a78-15.30", 0x40000, 0x8000, 0x6b61a413);   /* 2nd plane */
            ROM_LOAD("a78-16.31", 0x48000, 0x8000, 0xb5492d97);
            ROM_LOAD("a78-17.32", 0x50000, 0x8000, 0xd69762d5);
            ROM_LOAD("a78-18.33", 0x58000, 0x8000, 0x9f243b68);
            ROM_LOAD("a78-19.34", 0x60000, 0x8000, 0x66e9438c);
            ROM_LOAD("a78-20.35", 0x68000, 0x8000, 0x9ef863ad);
            /* 0x70000-0x7ffff empty */

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("a78-08.37", 0x0000, 0x08000, 0xae11a07b);

            ROM_REGION(0x10000, Mame.REGION_CPU3);/* 64k for the third CPU */
            ROM_LOAD("a78-07.46", 0x0000, 0x08000, 0x4f9a26e8);

            ROM_REGION(0x0800, Mame.REGION_CPU4);	/* 2k for the microcontroller */
            ROM_LOAD("68705.bin", 0x0000, 0x0800, 0x78caa635);	/* from a pirate board */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_bublbobl()
        {

            INPUT_PORTS_START("bublbobl");
            PORT_START();     /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();     /* DSW0 */
            PORT_DIPNAME(0x01, 0x00, "Language");
            PORT_DIPSETTING(0x01, "Japanese");
            PORT_DIPSETTING(0x00, "English");
            PORT_DIPNAME(0x02, 0x02, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x02, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x00, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_2C"));

            PORT_START();     /* DSW1 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x08, "20000 80000");
            PORT_DIPSETTING(0x0c, "30000 100000");
            PORT_DIPSETTING(0x04, "40000 200000");
            PORT_DIPSETTING(0x00, "50000 250000");
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Lives));
            PORT_DIPSETTING(0x10, "1");
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x20, "5");
            PORT_DIPNAME(0xc0, 0xc0, "Spare");
            PORT_DIPSETTING(0x00, "A");
            PORT_DIPSETTING(0x40, "B");
            PORT_DIPSETTING(0x80, "C");
            PORT_DIPSETTING(0xc0, "D");

            PORT_START();    /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);

            PORT_START();    /* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_2WAY | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_2WAY | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public driver_bublbobl()
        {
            drv = new machine_driver_bublbobl();
            year = "1986";
            name = "bublbobl";
            description = "Bubble Bobble";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_bublbobl();
            rom = rom_bublbobl();
            drv.HasNVRAMhandler = false;
        }
    }
}
