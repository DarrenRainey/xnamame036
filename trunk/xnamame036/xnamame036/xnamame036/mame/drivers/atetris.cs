using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_atetris : Mame.GameDriver
    {
        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];
        const int BANK0 = 0x10000;
        const int BANK1 = 0x4000;

        static int slapstic_primed = 0;
        static int slapstic_bank = BANK0;
        static int slapstic_nextbank = -1;
        static int slapstic_75xxcnt = 0;
        static int slapstic_last60xx = 0;
        static int slapstic_last75xx = 0;
        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x20ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x2400, 0x25ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x2800, 0x280f, Pokey.pokey1_r ),
	new Mame.MemoryReadAddress( 0x2810, 0x281f, Pokey.pokey2_r ),
	new Mame.MemoryReadAddress( 0x4000, 0x7fff, atetris_slapstic_r ),
	new Mame.MemoryReadAddress( 0x8000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x1000, 0x1fff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x2000, 0x20ff,Mame.paletteram_RRRGGGBB_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x2400, 0x25ff, Mame.MWA_RAM, nvram, nvram_size ),
	new Mame.MemoryWriteAddress( 0x2800, 0x280f, Pokey.pokey1_w ),
	new Mame.MemoryWriteAddress( 0x2810, 0x281f, Pokey.pokey2_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x3400, 0x3400, Mame.MWA_NOP ),  // EEPROM enable
	new Mame.MemoryWriteAddress( 0x3800, 0x3800, Mame.MWA_NOP ),  // ???
	new Mame.MemoryWriteAddress( 0x3c00, 0x3c00, Mame.MWA_NOP ),  // ???
	new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
                8, 8,    /* 8*8 chars */
                2048,   /* 2048 characters */
                4,      /* 4 bits per pixel */
                new uint[] { 0, 1, 2, 3 },  // The 4 planes are packed together
                new uint[] { 0 * 4, 1 * 4, 2 * 4, 3 * 4, 4 * 4, 5 * 4, 6 * 4, 7 * 4 },
                new uint[] { 0 * 4 * 8, 1 * 4 * 8, 2 * 4 * 8, 3 * 4 * 8, 4 * 4 * 8, 5 * 4 * 8, 6 * 4 * 8, 7 * 4 * 8 },
                8 * 8 * 4     /* every char takes 32 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, charlayout, 0, 16 ),
};

        static POKEYinterface pokey_interface =
        new POKEYinterface(
            2,      /* 2 chips */
            1789790,	/* ? */
            new int[] { 50, 50 },
            /* The 8 pot handlers */
              new pot_delegate[] { null, null, null, null },
          new pot_delegate[] { null, null, null, null },
              new pot_delegate[] { null, null, null, null },
          new pot_delegate[] { null, null, null, null },
              new pot_delegate[] { null, null, null, null },
              new pot_delegate[] { null, null, null, null },
              new pot_delegate[] { null, null, null, null },
              new pot_delegate[] { null, null, null, null },
            /* The allpot handler */
             new pot_delegate[] { Mame.input_port_0_r, Mame.input_port_1_r },
             new pot_delegate[] { null, null, null, null },
             new pokey_serout[] { null, null, null, null },
             new Mame.irqcallback[] { null, null, null, null }
        );
        static int atetris_slapstic_r(int offset)
        {
            if (slapstic_nextbank != -1)
            {
                slapstic_bank = slapstic_nextbank;
                slapstic_nextbank = -1;
            }

            if ((offset & 0xff00) == 0x2000 ||
                (offset & 0xff00) == 0x3500)
            {
                if (offset == 0x2000)
                {
                    // Reset
                    slapstic_75xxcnt = 0;
                    slapstic_last60xx = 0;
                    slapstic_primed = 1;
                }
                else if (offset >= 0x3500)
                {
                    slapstic_75xxcnt++;
                    slapstic_last75xx = (offset & 0xff);
                }
                else
                {
                    if (slapstic_primed != 0)
                        switch (offset & 0xff)
                        {
                            case 0x80:
                                {
                                    slapstic_nextbank = BANK0;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 0 at %04X\n", cpu_get_pc());
#endif
                                }
                                break;

                            case 0x90:
                                if ((slapstic_75xxcnt == 0) ||
                                    (slapstic_75xxcnt == 2 && slapstic_last60xx == 0x90))
                                {
                                    slapstic_nextbank = BANK1;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 1 at %04X\n", cpu_get_pc());
#endif
                                }
                                else
                                {
                                    slapstic_nextbank = BANK0;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 0 at %04X\n", cpu_get_pc());
#endif
                                }
                                break;

                            case 0xa0:
                                if (slapstic_last60xx == 0xb0)
                                {
                                    slapstic_nextbank = BANK1;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 1 at %04X\n", cpu_get_pc());
#endif
                                }
                                else
                                {
                                    slapstic_nextbank = BANK0;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 0 at %04X\n", cpu_get_pc());
#endif
                                }
                                break;

                            case 0xb0:
                                if (slapstic_75xxcnt == 6 && slapstic_last60xx == 0xb0 &&
                                    slapstic_last75xx == 0x53)
                                {
                                    slapstic_nextbank = BANK1;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 1 at %04X\n", cpu_get_pc());
#endif
                                }
                                else
                                {
                                    slapstic_nextbank = BANK0;
#if LOG_SLAPSTICK
                    if (errorlog) fprintf(errorlog, "Selecting Bank 0 at %04X\n", cpu_get_pc());
#endif
                                }
                                break;

                            default:
                                slapstic_primed = 0;
                                break;
                        }

                    slapstic_last60xx = (offset & 0xff);
                    slapstic_75xxcnt = 0;
                }
            }
            else
            {
                slapstic_primed = 0;
            }

            return Mame.memory_region(Mame.REGION_CPU1)[slapstic_bank + offset];
        }


        class machine_driver_atetris : Mame.MachineDriver
        {
            public machine_driver_atetris()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502, 1750000, readmem, writemem, null, null, Mame.interrupt, 4));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 42 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 42 * 8 - 1, 0 * 8, 30 * 8 - 1);
                gfxdecodeinfo = driver_atetris.gfxdecodeinfo;
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_POKEY, pokey_interface));
            }
            public override void init_machine()
            {
                //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write!=0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file != null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                    else
                        for (int i = 0; i < nvram_size[0]; i++)
                            nvram.buffer[i] = 0xff;
                }
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
                /* recalc the palette if necessary */
                if (Mame.palette_recalc() != null)
                    Generic.SetDirtyBuffer(true);


                /* for every character in the backround RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = 0; offs < Generic.videoram_size[0]; offs += 2)
                {
                    if (!Generic.dirtybuffer[offs] && !Generic.dirtybuffer[offs + 1]) continue;

                    Generic.dirtybuffer[offs] = Generic.dirtybuffer[offs + 1] = false;

                    int sy = 8 * (offs / 128);
                    int sx = 4 * (offs % 128);

                    if (sx >= 42 * 8) continue;

                    int charcode = Generic.videoram[offs] | ((Generic.videoram[offs + 1] & 0x07) << 8);

                    int color = ((Generic.videoram[offs + 1] & 0xf0) >> 4);

                    Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                            (uint)charcode,
                            (uint)color,
                            false,false,
                            sx, sy,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                Mame.copybitmap(bitmap, Generic.tmpbitmap,false,false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {

            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);


            // Move the lower 16k to 0x10000
            Buffer.BlockCopy(RAM.buffer, RAM.offset, RAM.buffer, RAM.offset + 0x10000, 0x4000);
            Array.Clear(RAM.buffer, RAM.offset, 0x4000);
            //memcpy(&RAM[0x10000], &RAM[0x00000], 0x4000);
            //memset(&RAM[0x00000], 0, 0x4000);
        }
        Mame.RomModule[] rom_atetris()
        {
            ROM_START("atetris");
            ROM_REGION(0x14000, Mame.REGION_CPU1);     /* 80k for code */
            ROM_LOAD("1100.45f", 0x0000, 0x10000, 0x2acbdb09);

            ROM_REGION(0x10000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("1101.35a", 0x0000, 0x10000, 0x84a1939f);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_atetris()
        {
            INPUT_PORTS_START("atetris");
            // These ports are read via the Pokeys
            PORT_START();      /* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BITX(0x04, 0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_TOGGLE, "Freeze", (ushort)Mame.InputCodes.KEYCODE_5, IP_JOY_NONE);
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x04, DEF_STR(On));
            PORT_BITX(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, "Freeze Step", (ushort)Mame.InputCodes.KEYCODE_6, IP_JOY_NONE);
            PORT_BIT(0x30, IP_ACTIVE_HIGH, IPT_UNUSED);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_VBLANK);
            PORT_SERVICE(0x80, IP_ACTIVE_HIGH);

            PORT_START();      /* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER1 | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER1 | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER1 | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2 | IPF_8WAY);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2 | IPF_8WAY);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2 | IPF_8WAY);
            return INPUT_PORTS_END;
        }
        public driver_atetris()
        {
            drv = new machine_driver_atetris();
            year = "1988";
            name = "atetris";
            description = "Tetris (set 1";
            manufacturer = "Atari";
            flags = Mame.ROT0;
            input_ports = input_ports_atetris();
            rom = rom_atetris();
            drv.HasNVRAMhandler = true;
        }
    }
}
