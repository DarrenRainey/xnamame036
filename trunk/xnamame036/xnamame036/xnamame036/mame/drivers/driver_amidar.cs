using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_amidar : Mame.GameDriver
    {
        static _BytePtr amidar_attributesram = new _BytePtr(1);
        static int[] flipscreen = new int[2];
        static Mame.rectangle spritevisiblearea = new Mame.rectangle(2 * 8 + 1, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
        static Mame.rectangle spritevisibleareaflipx = new Mame.rectangle(0 * 8, 30 * 8 - 2, 2 * 8, 30 * 8 - 1);



        static Mame.MemoryReadAddress[] amidar_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x4fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x87ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9000, 0x93ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x9800, 0x985f, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xa800, 0xa800, Mame.watchdog_reset_r ),
	new Mame.MemoryReadAddress( 0xb000, 0xb000, Mame.input_port_0_r ),	/* IN0 */
	new Mame.MemoryReadAddress( 0xb010, 0xb010, Mame.input_port_1_r ),	/* IN1 */
	new Mame.MemoryReadAddress( 0xb020, 0xb020, Mame.input_port_2_r ),	/* IN2 */
	new Mame.MemoryReadAddress( 0xb820, 0xb820, Mame.input_port_3_r ),	/* DSW */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x4fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x87ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0x9000, 0x93ff, Generic.videoram_w,  Generic.videoram,  Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x9800, 0x983f, amidar_attributes_w,  amidar_attributesram ),
	new Mame.MemoryWriteAddress( 0x9840, 0x985f, Mame.MWA_RAM,  Generic.spriteram,  Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x9860, 0x987f, Mame.MWA_NOP ),
	new Mame.MemoryWriteAddress( 0xa008, 0xa008, Mame.interrupt_enable_w ),
	new Mame.MemoryWriteAddress( 0xa010, 0xa010, amidar_flipx_w ),
	new Mame.MemoryWriteAddress( 0xa018, 0xa018, amidar_flipy_w ),
	new Mame.MemoryWriteAddress( 0xa030, 0xa030, amidar_coina_w ),
	new Mame.MemoryWriteAddress( 0xa038, 0xa038, amidar_coinb_w ),
	new Mame.MemoryWriteAddress( 0xb800, 0xb800, Mame.soundlatch_w ) ,
	new Mame.MemoryWriteAddress( 0xb810, 0xb810, scramble_sh_irqtrigger_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static void amidar_coina_w(int offset, int data)
        {
            Mame.coin_counter_w(0, data);
            Mame.coin_counter_w(0, 0);
        }

        static void amidar_coinb_w(int offset, int data)
        {
            Mame.coin_counter_w(1, data);
            Mame.coin_counter_w(1, 0);
        }



        static Mame.MemoryReadAddress[] sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x1fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x83ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x1fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0x8000, 0x83ff, Mame.MWA_RAM ),
//	new Mame.MemoryWriteAddress( 0x9000, 0x9000, MWA_NOP ),
//	new Mame.MemoryWriteAddress( 0x9080, 0x9080, MWA_NOP ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};



        static Mame.IOReadPort[] sound_readport =
{
	new Mame.IOReadPort( 0x80, 0x80, ay8910.AY8910_read_port_0_r ),
	new Mame.IOReadPort( 0x20, 0x20, ay8910.AY8910_read_port_1_r ),
	new Mame.IOReadPort( -1 )	/* end of table */
};

        static Mame.IOWritePort[] sound_writeport =
{
	new Mame.IOWritePort( 0x40, 0x40, ay8910.AY8910_control_port_0_w ),
	new Mame.IOWritePort( 0x80, 0x80, ay8910.AY8910_write_port_0_w ),
	new Mame.IOWritePort( 0x10, 0x10, ay8910.AY8910_control_port_1_w ),
	new Mame.IOWritePort( 0x20, 0x20, ay8910.AY8910_write_port_1_w ),
	new Mame.IOWritePort( -1 )	/* end of table */
};

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,	/* 2 bits per pixel */
            new uint[] { 0, 256 * 8 * 8 },	/* the two bitplanes are separated */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every char takes 8 consecutive bytes */
        );
        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            64,	/* 64 sprites */
            2,	/* 2 bits per pixel */
            new uint[] { 0, 64 * 16 * 16 },	/* the two bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, charlayout,     0, 8 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, spritelayout,   0, 8 ),
};



        static void amidar_flipx_w(int offset, int data)
        {
            if (flipscreen[0] != (data & 1))
            {
                flipscreen[0] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        static void amidar_flipy_w(int offset, int data)
        {
            if (flipscreen[1] != (data & 1))
            {
                flipscreen[1] = data & 1;
                Generic.SetDirtyBuffer(true);
            }
        }
        static void amidar_attributes_w(int offset, int data)
        {
            if ((offset & 1) != 0 && amidar_attributesram[offset] != data)
            {
                for (int i = offset / 2; i < Generic.videoram_size[0]; i += 32)
                    Generic.dirtybuffer[i] = true;
            }

            amidar_attributesram[offset] = (byte)data;
        }
        
/* The timer clock which feeds the upper 4 bits of    					*/
/* AY-3-8910 port A is based on the same clock        					*/
/* feeding the sound CPU Z80.  It is a divide by      					*/
/* 5120, formed by a standard divide by 512,        					*/
/* followed by a divide by 10 using a 4 bit           					*/
/* bi-quinary count sequence. (See LS90 data sheet    					*/
/* for an example).                                   					*/
/*																		*/
/* Bit 4 comes from the output of the divide by 1024  					*/
/*       0, 1, 0, 1, 0, 1, 0, 1, 0, 1									*/
/* Bit 5 comes from the QC output of the LS90 producing a sequence of	*/
/* 		 0, 0, 1, 1, 0, 0, 1, 1, 1, 0									*/
/* Bit 6 comes from the QD output of the LS90 producing a sequence of	*/
/*		 0, 0, 0, 0, 1, 0, 0, 0, 0, 1									*/
/* Bit 7 comes from the QA output of the LS90 producing a sequence of	*/
/*		 0, 0, 0, 0, 0, 1, 1, 1, 1, 1			 						*/

static int [] scramble_timer ={0x00, 0x10, 0x20, 0x30, 0x40, 0x90, 0xa0, 0xb0, 0xa0, 0xd0};

static int clock;
static int last_totalcycles = 0;
static int scramble_portB_r(int offset)
{
	/* need to protect from totalcycles overflow */

	/* number of Z80 clock cycles to count */

	int current_totalcycles;

	current_totalcycles = Mame.cpu_gettotalcycles();
	clock = (clock + (current_totalcycles-last_totalcycles)) % 5120;

	last_totalcycles = current_totalcycles;

	return scramble_timer[clock/512];
}
static int last;
static void scramble_sh_irqtrigger_w(int offset, int data)
{
	if (last == 0 && (data & 0x08) != 0)
	{
		/* setting bit 3 low then high triggers IRQ on the sound CPU */
		Mame.cpu_cause_interrupt(1, Mame.cpu_Z80.Z80_IRQ_INT);
	}

	last = data & 0x08;
}

        static AY8910interface ay8910_interface =
        new AY8910interface(
            2,	/* 2 chips */
            14318000 / 8,	/* 1.78975 Mhz */
            new int[] { Mame.MIXERG(30, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER), Mame.MIXERG(30, Mame.MIXER_GAIN_2x, Mame.MIXER_PAN_CENTER) },
            new AY8910portRead[] { Mame.soundlatch_r, null },
            new AY8910portRead[] { scramble_portB_r, null },
            new AY8910portWrite[] { null, null },
            new AY8910portWrite[] { null, null }, null
        );
        class machine_driver_amidar : Mame.MachineDriver
        {
            public machine_driver_amidar()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 6, amidar_readmem, writemem, null, null, Mame.nmi_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80 | Mame.CPU_AUDIO_CPU, 14318000 / 8, sound_readmem, sound_writemem, sound_readport, sound_writeport, Mame.ignore_interrupt, 1));
                frames_per_second = 60;
                vblank_duration = 2500;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 32 * 8 - 1);
                gfxdecodeinfo = driver_amidar.gfxdecodeinfo;
                total_colors = 32;
                color_table_len = 32;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY;
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
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
	//#define TOTAL_COLORS(gfxn) (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity)
	//#define COLOR(gfxn,offs) (colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs])

                uint pi = 0, cpi = 0;
	for (int i = 0;i < Mame.Machine.drv.total_colors;i++)
	{
		int bit0,bit1,bit2;


		/* red component */
		bit0 = (color_prom[cpi] >> 0) & 0x01;
		bit1 = (color_prom[cpi] >> 1) & 0x01;
		bit2 = (color_prom[cpi] >> 2) & 0x01;
		palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
		/* green component */
		bit0 = (color_prom[cpi] >> 3) & 0x01;
		bit1 = (color_prom[cpi] >> 4) & 0x01;
		bit2 = (color_prom[cpi] >> 5) & 0x01;
        palette[pi++] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
		/* blue component */
		bit0 = (color_prom[cpi] >> 6) & 0x01;
		bit1 = (color_prom[cpi] >> 7) & 0x01;
        palette[pi++] = (byte)(0x4f * bit0 + 0xa8 * bit1);

		cpi++;
	}


	/* characters and sprites use the same palette */
    for (int i = 0; i < Mame.Machine.gfx[0].total_colors * Mame.Machine.gfx[0].color_granularity; i++)
	{
        if ((i & 3) != 0)
            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i, (ushort)i);
        else
            colortable.write16(Mame.Machine.drv.gfxdecodeinfo[0].color_codes_start + i, 0); 	/* 00 is always black, regardless of the contents of the PROM */
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
                        int sx, sy;


                        Generic.dirtybuffer[offs] = false;

                        sx = offs % 32;
                        sy = offs / 32;

                        if (flipscreen[0]!=0) sx = 31 - sx;
                        if (flipscreen[1]!=0) sy = 31 - sy;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                Generic.videoram[offs],
                                (uint)(amidar_attributesram[2 * (offs % 32) + 1] & 0x07),
                                flipscreen[0]!=0, flipscreen[1]!=0,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }


                /* copy the temporary bitmap to the screen */
                Mame.copybitmap(bitmap,Generic.tmpbitmap, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (int offs = Generic.spriteram_size[0] - 4; offs >= 0; offs -= 4)
                {
                    int sx, sy;
                    bool flipx, flipy;

                    sx = (Generic.spriteram[offs + 3] + 1) & 0xff;	/* ??? */
                    sy = 240 - Generic.spriteram[offs];
                    flipx = (Generic.spriteram[offs + 1] & 0x40)!=0;
                    flipy = (Generic.spriteram[offs + 1] & 0x80)!=0;

                    if (flipscreen[0]!=0)
                    {
                        sx = 241 - sx;	/* note: 241, not 240 */
                        flipx = !flipx;
                    }
                    if (flipscreen[1]!=0)
                    {
                        sy = 240 - sy;
                        flipy = !flipy;
                    }

                    /* Sprites #0, #1 and #2 need to be offset one pixel to be correctly */
                    /* centered on the ladders in Turtles (we move them down, but since this */
                    /* is a rotated game, we actually move them left). */
                    /* Note that the adjustement must be done AFTER handling flipscreen, thus */
                    /* proving that this is a hardware related "feature" */
                    if (offs <= 2 * 4) sy++;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)(Generic.spriteram[offs + 1] & 0x3f),
                            (uint)(Generic.spriteram[offs + 2] & 0x07),
                            flipx, flipy,
                            sx, sy,
                            flipscreen[0] !=0? spritevisibleareaflipx : spritevisiblearea, Mame.TRANSPARENCY_PEN, 0);
                }
            }
            public override void vh_eof_callback()
            {
                //none
            }
        }
        public override void driver_init()
        {
           //none
        }
        Mame.InputPortTiny[] input_ports_amidar()
        {
            INPUT_PORTS_START("amidar");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably space for button 2 */
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START("IN1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_BITX(0, 0x00, (uint)inptports.IPT_DIPSWITCH_SETTING | IPF_CHEAT, "255", IP_KEY_NONE, IP_JOY_NONE);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);	/* probably space for player 2 button 2 */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            PORT_DIPNAME(0x02, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x02, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x00, "30000 70000");
            PORT_DIPSETTING(0x04, "50000 80000");
            PORT_DIPNAME(0x08, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x08, DEF_STR("Cocktail"));
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_DIPNAME(0x20, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_DIPNAME(0x80, 0x00, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("DSW");
            PORT_DIPNAME(0x0f, 0x0f, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x04, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x0a, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x02, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x08, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0x0f, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x0e, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x06, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0x0b, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0d, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x09, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0xf0, 0xf0, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x40, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x80, DEF_STR("4C_3C"));
            PORT_DIPSETTING(0xf0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0xe0, DEF_STR("2C_3C"));
            PORT_DIPSETTING(0x70, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x60, DEF_STR("2C_5C"));
            PORT_DIPSETTING(0xb0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xd0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x50, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x90, DEF_STR("1C_7C"));
            PORT_DIPSETTING(0x00, "Disable All Coins");
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_amidar()
        {
            ROM_START("amidar");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("amidar.2c", 0x0000, 0x1000, 0xc294bf27);
            ROM_LOAD("amidar.2e", 0x1000, 0x1000, 0xe6e96826);
            ROM_LOAD("amidar.2f", 0x2000, 0x1000, 0x3656be6f);
            ROM_LOAD("amidar.2h", 0x3000, 0x1000, 0x1be170bd);

            ROM_REGION(0x10000, Mame.REGION_CPU2);	/* 64k for the audio CPU */
            ROM_LOAD("amidar.5c", 0x0000, 0x1000, 0xc4b66ae4);
            ROM_LOAD("amidar.5d", 0x1000, 0x1000, 0x806785af);

            ROM_REGION(0x1000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("amidar.5f", 0x0000, 0x0800, 0x5e51e84d);
            ROM_LOAD("amidar.5h", 0x0800, 0x0800, 0x2f7f1c30);

            ROM_REGION(0x0020, Mame.REGION_PROMS);
            ROM_LOAD("amidar.clr", 0x0000, 0x0020, 0xf940dcc3);
            return ROM_END;
        }
        public driver_amidar()
        {
            drv = new machine_driver_amidar();
            year = "1981";
            name = "amidar";
            description = "Amidar";
            manufacturer = "Konami";
            flags = Mame.ROT90;
            input_ports = input_ports_amidar();
            rom = rom_amidar();
            drv.HasNVRAMhandler = false;
        }
    }
}
