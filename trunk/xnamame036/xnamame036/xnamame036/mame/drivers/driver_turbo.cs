using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_turbo : Mame.GameDriver
    {
        static byte turbo_opa, turbo_opb, turbo_opc;
        static byte turbo_ipa, turbo_ipb, turbo_ipc;
        static byte turbo_fbpla, turbo_fbcol;
        static byte[] turbo_segment_data = new byte[32];
        static byte turbo_speed;

        /* local data */
        static byte segment_address, segment_increment;
        static byte osel, bsel, accel;



        /* external definitions */
        static _BytePtr turbo_sprite_position = new _BytePtr(1);
        static byte turbo_collision;

        /* internal data */
        static _BytePtr sprite_gfxdata, sprite_priority;
        static _BytePtr road_gfxdata, road_palette, road_enable_collide;
        static _BytePtr back_gfxdata, back_palette;
        static _BytePtr overall_priority, collision_map;
        const int VIEW_WIDTH = 32 * 8;
        const int VIEW_HEIGHT = 28 * 8;

        /* sprite tracking */
        class sprite_params_data
        {
            public UIntSubArray _base;
            public int offset, rowbytes;
            public int yscale, miny, maxy;
            public int xscale, xoffs;
        };
        static sprite_params_data[] sprite_params = new sprite_params_data[16];
        static uint[] sprite_expanded_data;

        /* orientation */
        static Mame.rectangle game_clip = new Mame.rectangle(0, VIEW_WIDTH - 1, 64, 64 + VIEW_HEIGHT - 1);
        static Mame.rectangle adjusted_clip;
        static int startx, starty, deltax, deltay;

        /* misc other stuff */
        static ushort[] back_expanded_data;
        static ushort[] road_expanded_palette;
        static bool drew_frame;



        static Mame.GfxLayout numlayout =
        new Mame.GfxLayout(
            10, 8,	/* 10*8 characters */
            16,		/* 16 characters */
            1,		/* 1 bit per pixel */
            new uint[] { 0 },	/* bitplane offsets */
                new uint[] { 9 * 8, 8 * 8, 7 * 8, 6 * 8, 5 * 8, 4 * 8, 3 * 8, 2 * 8, 1 * 8, 0 * 8 },
                new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            10 * 8	/* every character uses 10 consecutive bytes */
        );

        static Mame.GfxLayout tachlayout =
        new Mame.GfxLayout(
            16, 1,	/* 16*1 characters */
            2,		/* 2 characters */
            1,		/* 1 bit per pixel */
            new uint[] { 0 },	/* bitplane offsets */
            new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new uint[] { 0 },
            1 * 8		/* every character uses 1 consecutive byte */
        );

        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,	/* 8*8 characters */
            256,	/* 256 characters */
            2,		/* 2 bits per pixel */
            new uint[] { 256 * 8 * 8, 0 },	/* bitplane offsets */
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8		/* every character uses 8 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX4, 0x0000, numlayout,	512,   1 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX4, 0x0100, tachlayout,	512,   3 ),
	new Mame.GfxDecodeInfo(Mame.REGION_GFX3, 0x0000, charlayout,	512,   3 ),
};


        /*********************************************************************
         * Sound interfaces
         *********************************************************************/

        static string[] sample_names =
{
	"*turbo",
	"01.wav",		/* Trig1 */
	"02.wav",		/* Trig2 */
	"03.wav",		/* Trig3 */
	"04.wav",		/* Trig4 */
	"05.wav",		/* Screech */
	"06.wav",		/* Crash */
	"skidding.wav",	/* Spin */
	"idle.wav",		/* Idle */
	"ambulanc.wav",	/* Ambulance */
	null
};

        static Mame.Samplesinterface samples_interface =
        new Mame.Samplesinterface(
            8,			/* eight channels */
            25,			/* volume */
            sample_names
        );

        static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x5fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xb000, 0xb1ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xe000, 0xe7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xf000, 0xf7ff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xf800, 0xf803, Mame.ppi_8255.ppi8255_0_r ),
	new Mame.MemoryReadAddress( 0xf900, 0xf903, Mame.ppi_8255.ppi8255_1_r ),
	new Mame.MemoryReadAddress( 0xfa00, 0xfa03, Mame.ppi_8255.ppi8255_2_r ),
	new Mame.MemoryReadAddress( 0xfb00, 0xfb03, Mame.ppi_8255.ppi8255_3_r ),
	new Mame.MemoryReadAddress( 0xfc00, 0xfcff, turbo_8279_r ),
	new Mame.MemoryReadAddress( 0xfd00, 0xfdff, Mame.input_port_0_r ),
	new Mame.MemoryReadAddress( 0xfe00, 0xfeff, turbo_collision_r ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};
        static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x5fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa0ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0xa800, 0xa807, turbo_coin_and_lamp_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb1ff, Mame.MWA_RAM, turbo_sprite_position ),
	new Mame.MemoryWriteAddress( 0xb800, 0xb800, Mame.MWA_NOP ),	/* resets the analog wheel value */
	new Mame.MemoryWriteAddress( 0xe000, 0xe7ff, Mame.MWA_RAM, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0xe800, 0xe800, turbo_collision_clear_w ),
	new Mame.MemoryWriteAddress( 0xf000, 0xf7ff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xf800, 0xf803, Mame.ppi_8255.ppi8255_0_w ),
	new Mame.MemoryWriteAddress( 0xf900, 0xf903, Mame.ppi_8255.ppi8255_1_w ),
	new Mame.MemoryWriteAddress( 0xfa00, 0xfa03, Mame.ppi_8255.ppi8255_2_w ),
	new Mame.MemoryWriteAddress( 0xfb00, 0xfb03, Mame.ppi_8255.ppi8255_3_w ),
	new Mame.MemoryWriteAddress( 0xfc00, 0xfcff, turbo_8279_w ),
	new Mame.MemoryWriteAddress( -1 )	/* end of table */
};
        static int turbo_collision_r(int offset)
        {
            return Mame.readinputport(3) | (turbo_collision & 15);
        }
        static void turbo_collision_clear_w(int offset, int data)
        {
            turbo_collision = 0;
        }
        static void turbo_coin_and_lamp_w(int offset, int data)
        {
            data &= 1;
            switch (offset & 7)
            {
                case 0:		/* Coin Meter 1 */
                case 1:		/* Coin Meter 2 */
                case 2:		/* n/c */
                    break;

                case 3:		/* Start Lamp */
                    Mame.osd_led_w(0, data);
                    break;

                case 4:		/* n/c */
                default:
                    break;
            }
        }

        static int turbo_8279_r(int offset)
        {
            if ((offset & 1) == 0)
                return Mame.readinputport(1);  /* DSW 1 */
            else
            {
                Mame.printf("read 0xfc%02x\n", offset);
                return 0x10;
            }
        }

        static void turbo_8279_w(int offset, int data)
        {
            switch (offset & 1)
            {
                case 0x00:
                    turbo_segment_data[segment_address * 2] = (byte)(data & 15);
                    turbo_segment_data[segment_address * 2 + 1] = (byte)((data >> 4) & 15);
                    segment_address = (byte)((segment_address + segment_increment) & 15);
                    break;

                case 0x01:
                    switch (data & 0xe0)
                    {
                        case 0x80:
                            segment_address = (byte)(data & 15);
                            segment_increment = 0;
                            break;
                        case 0x90:
                            segment_address = (byte)(data & 15);
                            segment_increment = 1;
                            break;
                        case 0xc0: Array.Clear(turbo_segment_data, 0, 32);
                            //memset(turbo_segment_data, 0, 32);
                            break;
                    }
                    break;
            }
        }

        static int portA_r(int chip)
        {
            if (chip == 3)
                return Mame.readinputport(4);	 /* Wheel */
            return 0;
        }

        static int portB_r(int chip)
        {
            if (chip == 3)
                return Mame.readinputport(2);	/* DSW 2 */
            return 0;
        }

        static void portA_w(int chip, int data)
        {
            switch (chip)
            {
                case 0: /* signals 0PA0 to 0PA7 */
                    turbo_opa = (byte)data;
                    break;

                case 1: /* signals 1PA0 to 1PA7 */
                    turbo_ipa = (byte)data;
                    break;

                case 2: /* signals 2PA0 to 2PA7 */
                    /*
                        2PA0 = /CRASH
                        2PA1 = /TRIG1
                        2PA2 = /TRIG2
                        2PA3 = /TRIG3
                        2PA4 = /TRIG4
                        2PA5 = OSEL0
                        2PA6 = /SLIP
                        2PA7 = /CRASHL
                    */
                    /* missing short crash sample, but I've never seen it triggered */
                    if ((data & 0x02) == 0) Mame.sample_start(0, 0, false);
                    if ((data & 0x04) == 0) Mame.sample_start(0, 1, false);
                    if ((data & 0x08) == 0) Mame.sample_start(0, 2, false);
                    if ((data & 0x10) == 0) Mame.sample_start(0, 3, false);
                    if ((data & 0x40) == 0) Mame.sample_start(1, 4, false);
                    if ((data & 0x80) == 0) Mame.sample_start(2, 5, false);
                    osel = (byte)((osel & 6) | ((data >> 5) & 1));
                    update_samples();
                    break;
            }
        }

        static void portB_w(int chip, int data)
        {
            switch (chip)
            {
                case 0: /* signals 0PB0 to 0PB7 */
                    turbo_opb = (byte)data;
                    break;

                case 1: /* signals 1PB0 to 1PB7 */
                    turbo_ipb = (byte)data;
                    break;

                case 2: /* signals 2PB0 to 2PB7 */
                    /*
                        2PB0 = ACC0
                        2PB1 = ACC1
                        2PB2 = ACC2
                        2PB3 = ACC3
                        2PB4 = ACC4
                        2PB5 = ACC5
                        2PB6 = /AMBU
                        2PB7 = /SPIN
                    */
                    accel = (byte)(data & 0x3f);
                    update_samples();
                    if ((data & 0x40) == 0)
                    {
                        if (!Mame.sample_playing(7))
                            Mame.sample_start(7, 8, false);
                        else
                            Mame.printf("ambu didnt start\n");
                    }
                    else
                        Mame.sample_stop(7);
                    if ((data & 0x80) == 0) Mame.sample_start(3, 6, false);
                    break;
            }
        }

        static void portC_w(int chip, int data)
        {
            switch (chip)
            {
                case 0: /* signals 0PC0 to 0PC7 */
                    turbo_opc = (byte)data;
                    break;

                case 1: /* signals 1PC0 to 1PC7 */
                    turbo_ipc = (byte)data;
                    break;

                case 2: /* signals 2PC0 to 2PC7 */
                    /*
                        2PC0 = OSEL1
                        2PC1 = OSEL2
                        2PC2 = BSEL0
                        2PC3 = BSEL1
                        2PC4 = SPEED0
                        2PC5 = SPEED1
                        2PC6 = SPEED2
                        2PC7 = SPEED3
                    */
                    turbo_speed = (byte)((data >> 4) & 0x0f);
                    bsel = (byte)((data >> 2) & 3);
                    osel = (byte)((osel & 1) | ((data & 3) << 1));
                    update_samples();
                    break;

                case 3:
                    /* bit 0-3 = signals PLA0 to PLA3 */
                    /* bit 4-6 = signals COL0 to COL2 */
                    /* bit 7 = unused */
                    turbo_fbpla = (byte)(data & 0x0f);
                    turbo_fbcol = (byte)((data & 0x70) >> 4);
                    break;
            }
        }
        static void update_samples()
        {
            /* accelerator sounds */
            /* BSEL == 3 -. off */
            /* BSEL == 2 -. standard */
            /* BSEL == 1 -. tunnel */
            /* BSEL == 0 -. ??? */
            if (bsel == 3 && Mame.sample_playing(6))
                Mame.sample_stop(6);
            else if (bsel != 3 && !Mame.sample_playing(6))
                Mame.sample_start(6, 7, true);
            if (Mame.sample_playing(6))
                //		sample_set_freq(6, 44100 * (accel & 0x3f) / 7 + 44100);
                Mame.sample_set_freq(6, (int)(44100 * (accel & 0x3f) / 5.25 + 44100));
        }
        static Mame.ppi_8255.ppi8255_interface intf = new Mame.ppi_8255.ppi8255_interface(
             4, portA_r, portB_r, null, portA_w, portB_w, portC_w
             );
        class machine_driver_turbo : Mame.MachineDriver
        {
            public machine_driver_turbo()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 5000000, readmem, writemem, null, null, Mame.interrupt, 1));
                frames_per_second = 30;
                vblank_duration = Mame.DEFAULT_REAL_30HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 35 * 8;
                visible_area = new Mame.rectangle(1 * 8, 32 * 8 - 1, 0 * 8, 35 * 8 - 1);
                gfxdecodeinfo = driver_turbo.gfxdecodeinfo;
                total_colors = 512 + 6;
                color_table_len = 512 + 6;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                //xxx sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));

            }
            public override void init_machine()
            {
                Mame.ppi_8255.ppi8255_init(intf);
                segment_address = segment_increment = 0;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                int cpi = 0;
                for (int i = 0; i < 512; i++, cpi++)
                {
                    /* bits 4,5,6 of the index are inverted before being used as addresses */
                    /* to save ourselves lots of trouble, we will undo the inversion when */
                    /* generating the palette */
                    int adjusted_index = i ^ 0x70;

                    /* red component */
                    int bit0 = (color_prom[cpi] >> 0) & 1;
                    int bit1 = (color_prom[cpi] >> 1) & 1;
                    int bit2 = (color_prom[cpi] >> 2) & 1;
                    palette[adjusted_index * 3 + 0] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    /* green component */
                    bit0 = (color_prom[cpi] >> 3) & 1;
                    bit1 = (color_prom[cpi] >> 4) & 1;
                    bit2 = (color_prom[cpi] >> 5) & 1;
                    palette[adjusted_index * 3 + 1] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);

                    /* blue component */
                    bit0 = 0;
                    bit1 = (color_prom[cpi] >> 6) & 1;
                    bit2 = (color_prom[cpi] >> 7) & 1;
                    palette[adjusted_index * 3 + 2] = (byte)(0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2);
                }

                /* LED segments colors: black and red */
                palette[512 * 3 + 0] = 0x00;
                palette[512 * 3 + 1] = 0x00;
                palette[512 * 3 + 2] = 0x00;
                palette[513 * 3 + 0] = 0xff;
                palette[513 * 3 + 1] = 0x00;
                palette[513 * 3 + 2] = 0x00;
                /* Tachometer colors: Led colors + yellow and green */
                palette[514 * 3 + 0] = 0x00;
                palette[514 * 3 + 1] = 0x00;
                palette[514 * 3 + 2] = 0x00;
                palette[515 * 3 + 0] = 0xff;
                palette[515 * 3 + 1] = 0xff;
                palette[515 * 3 + 2] = 0x00;
                palette[516 * 3 + 0] = 0x00;
                palette[516 * 3 + 1] = 0x00;
                palette[516 * 3 + 2] = 0x00;
                palette[517 * 3 + 0] = 0x00;
                palette[517 * 3 + 1] = 0xff;
                palette[517 * 3 + 2] = 0x00;
            }
            public override int vh_start()
            {
                uint[] sprite_expand = new uint[16];
                uint[] dst;
                ushort[] bdst;
                _BytePtr src;

                /* allocate the expanded sprite data */
                int sprite_length = Mame.memory_region_length(Mame.REGION_GFX1);
                int sprite_bank_size = sprite_length / 8;
                sprite_expanded_data = new uint[sprite_length*2];

                /* allocate the expanded background data */

                int back_length = Mame.memory_region_length(Mame.REGION_GFX3);
                back_expanded_data = new ushort[back_length/2];

                /* allocate the expanded road palette */
                road_expanded_palette = new ushort[0x40];

                /* determine ROM/PROM addresses */
                sprite_gfxdata = Mame.memory_region(Mame.REGION_GFX1);
                sprite_priority = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0200);

                road_gfxdata = Mame.memory_region(Mame.REGION_GFX2);
                road_palette = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0b00);
                road_enable_collide = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0b40);

                back_gfxdata = Mame.memory_region(Mame.REGION_GFX3);
                back_palette = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0a00);

                overall_priority = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0600);
                collision_map = new _BytePtr(Mame.memory_region(Mame.REGION_PROMS), 0x0b60);

                /* compute the sprite expansion array */
                for (int i = 0; i < 16; i++)
                {
                    uint value = 0;
                    if ((i & 1) != 0) value |= 0x00000001;
                    if ((i & 2) != 0) value |= 0x00000100;
                    if ((i & 4) != 0) value |= 0x00010000;
                    if ((i & 8) != 0) value |= 0x01000000;

                    /* special value for the end-of-row */
                    if ((i & 0x0c) == 0x04) value = 0x12345678;

                    sprite_expand[i] = value;
                }

                /* expand the sprite ROMs */
                src = new _BytePtr(sprite_gfxdata);
                dst = sprite_expanded_data;
                for (int i = 0; i < 8; i++)
                {
                    int di = 0;
                    /* expand this bank */
                    for (int j = 0; j < sprite_bank_size; j++)
                    {
                        dst[di++] = sprite_expand[src[0] >> 4];
                        dst[di++] = sprite_expand[src[0] & 15];
                        src.offset++;
                    }

                    /* shift for the next bank */
                    for (int j = 0; j < 16; j++)
                        if (sprite_expand[j] != 0x12345678) sprite_expand[j] <<= 1;
                }

                /* expand the background ROMs */
                src = new _BytePtr(back_gfxdata);
                bdst = back_expanded_data;
                int bdi = 0;
                for (int i = 0; i < back_length / 2; i++, src.offset++)
                {
                    int bits1 = src[0];
                    int bits2 = src[back_length / 2];
                    int newbits = 0;

                    for (int j = 0; j < 8; j++)
                    {
                        newbits |= ((bits1 >> (j ^ 7)) & 1) << (j * 2);
                        newbits |= ((bits2 >> (j ^ 7)) & 1) << (j * 2 + 1);
                    }
                    bdst[bdi++] = (ushort)newbits;
                }

                /* expand the road palette */
                src = road_palette;
                bdst = road_expanded_palette;
                bdi = 0;
                for (int i = 0; i < 0x20; i++, src.offset++)
                    bdst[bdi++] = (ushort)(src[0] | (src[0x20] << 8));

                /* set the default drawing parameters */
                startx = game_clip.min_x;
                starty = game_clip.min_y;
                deltax = deltay = 1;
                adjusted_clip = game_clip;

                /* adjust our parameters for the specified orientation */
                if (Mame.Machine.orientation != 0)
                {
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                    {
                        int temp = startx; startx = starty; starty = temp;
                        temp = adjusted_clip.min_x; adjusted_clip.min_x = adjusted_clip.min_y; adjusted_clip.min_y = temp;
                        temp = adjusted_clip.max_x; adjusted_clip.max_x = adjusted_clip.max_y; adjusted_clip.max_y = temp;
                    }
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_X) != 0)
                    {
                        startx = adjusted_clip.max_x;
                        if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) == 0) deltax = -deltax;
                        else deltay = -deltay;
                    }
                    if ((Mame.Machine.orientation & Mame.ORIENTATION_FLIP_Y) != 0)
                    {
                        starty = adjusted_clip.max_y;
                        if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) == 0) deltay = -deltay;
                        else deltax = -deltax;
                    }
                }

                /* other stuff */
                drew_frame = false;

                for (int i = 0; i < 16; i++)
                    sprite_params[i] = new sprite_params_data();

                /* return success */
                return 0;
            }
            public override void vh_stop()
            {
                sprite_expanded_data = null;
                back_expanded_data = null;
                road_expanded_palette = null;
            }

            static void update_sprite_info()
            {
                /* first loop over all sprites and update those whose scanlines intersect ours */
                for (int i = 0; i < 16; i++)
                {
                    _BytePtr sprite_base = new _BytePtr(Generic.spriteram, 16 * i);

                    /* snarf all the data */
                    sprite_params[i]._base = new UIntSubArray(sprite_expanded_data, (i & 7) * 0x8000);
                    sprite_params[i].offset = (sprite_base[6] + 256 * sprite_base[7]) & 0x7fff;
                    sprite_params[i].rowbytes = (short)(sprite_base[4] + 256 * sprite_base[5]);
                    sprite_params[i].miny = sprite_base[0];
                    sprite_params[i].maxy = sprite_base[1];
                    sprite_params[i].xscale = ((5 * 256 - 4 * sprite_base[2]) << 16) / (5 * 256);
                    sprite_params[i].yscale = (4 << 16) / (sprite_base[3] + 4);
                    sprite_params[i].xoffs = -1;
                }

                /* now find the X positions */
                for (int i = 0; i < 0x200; i++)
                {
                    int value = turbo_sprite_position[i];
                    if (value != 0)
                    {
                        int _base = (i & 0x100) >> 5;
                        for (int which = 0; which < 8; which++)
                            if ((value & (1 << which)) != 0)
                                sprite_params[_base + which].xoffs = i & 0xff;
                    }
                }
            }
            static void draw_one_sprite(sprite_params_data data, uint[] dest, int xclip, int scanline)
            {
                int xstep = data.xscale;
                int xoffs = data.xoffs;
                int xcurr, offset;
                UIntSubArray src;

                /* xoffs of -1 means don't draw */
                if (xoffs == -1) return;

                /* clip to the road */
                xcurr = 0;
                if (xoffs < xclip)
                {
                    /* the pixel clock starts on xoffs regardless of clipping; take this into account */
                    xcurr = ((xclip - xoffs) * xstep) & 0xffff;
                    xoffs = xclip;
                }

                /* compute the current data offset */
                scanline = ((scanline - data.miny) * data.yscale) >> 16;
                offset = data.offset + (scanline + 1) * data.rowbytes;

                /* determine the bitmap location */
                src = new UIntSubArray(data._base, (int)(offset & 0x7fff));

                /* loop over columns */
                while (xoffs < VIEW_WIDTH)
                {
                    uint srcval = src[xcurr >> 16];

                    /* stop on the end-of-row signal */
                    if (srcval == 0x12345678) break;
                    dest[xoffs++] |= srcval;
                    xcurr += xstep;
                }
            }
            static void draw_road_sprites(uint[] dest, int scanline)
            {
                int[] param_list = { 0, 8, 1, 9, 2, 10 };

                /* loop over the road sprites */
                for (int i = 0; i < 6; i++)
                {
                    /* if the sprite intersects this scanline, draw it */
                    if (scanline >= sprite_params[param_list[i]].miny && scanline < sprite_params[param_list[i]].maxy)
                        draw_one_sprite(sprite_params[param_list[i]], dest, 0, scanline);
                }
            }
            static void draw_offroad_sprites(uint[] dest, int road_column, int scanline)
            {
                int[] param_list = { 3, 11, 4, 12, 5, 13, 6, 14, 7, 15 };

                /* loop over the offroad sprites */
                for (int i = 0; i < 10; i++)
                {
                    /* if the sprite intersects this scanline, draw it */
                    if (scanline >= sprite_params[param_list[i]].miny && scanline < sprite_params[param_list[i]].maxy)
                        draw_one_sprite(sprite_params[param_list[i]], dest, road_column, scanline);
                }
            }
            static void draw_scores(Mame.osd_bitmap bitmap)
            {
                Mame.rectangle clip;
                int offs, x, y;

                /* current score */
                offs = 31;
                for (y = 0; y < 5; y++, offs--)
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                            turbo_segment_data[offs],
                            0,
                            false, false,
                            14 * 8, (2 + y) * 8,
                            Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                /* high scores */
                for (x = 0; x < 5; x++)
                {
                    offs = 6 + x * 5;
                    for (y = 0; y < 5; y++, offs--)
                        Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                turbo_segment_data[offs],
                                0,
                                false, false,
                                (20 + 2 * x) * 8, (2 + y) * 8,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }

                /* tachometer */
                clip = Mame.Machine.drv.visible_area;
                clip.min_x = 5 * 8;
                clip.max_x = clip.min_x + 1;
                for (y = 0; y < 22; y++)
                {
                    byte[] led_color = { 2, 2, 2, 2, 1, 1, 0, 0, 0, 0, 0 };
                    int code = ((y / 2) <= turbo_speed) ? 0 : 1;

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                            (uint)code,
                            led_color[y / 2],
                            false, false,
                            5 * 8, y * 2 + 8,
                            clip, Mame.TRANSPARENCY_NONE, 0);
                    if (y % 3 == 2)
                        clip.max_x++;
                }

                /* shifter status */
                if ((Mame.readinputport(0) & 0x04) != 0)
                {
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2], 'H', 0, false, false, 2 * 8, 3 * 8, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2], 'I', 0, false, false, 2 * 8, 4 * 8, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
                else
                {
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2], 'L', 0, false, false, 2 * 8, 3 * 8, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    Mame.drawgfx(bitmap, Mame.Machine.gfx[2], 'O', 0, false, false, 2 * 8, 4 * 8, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                }
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* update the sprite data */
                update_sprite_info();

                /* perform the actual drawing */
                draw_everything(bitmap,true);
                
                //if (bitmap.depth == 8)
                //    draw_everything_core_8(bitmap);
                //else
                //    draw_everything_core_16(bitmap);

                /* draw the LEDs for the scores */
                draw_scores(bitmap);

                /* indicate that we drew this frame, so that the eof callback doesn't bother doing anything */
                drew_frame = true;
            }
            public override void vh_eof_callback()
            {
                /* only do collision checking if we didn't draw */
                if (!drew_frame)
                {
                    update_sprite_info();
                    draw_everything(Mame.Machine.scrbitmap,false);
                }
                drew_frame = false;
            }
            static void draw_everything(Mame.osd_bitmap bitmap, bool fullDraw)
            {
                _BytePtr _base = new _BytePtr(bitmap.line[starty], startx);
                uint[] sprite_buffer = new uint[(32 * 8) + 256];

                _BytePtr overall_priority_base = new _BytePtr(overall_priority, (turbo_fbpla & 8) << 6);
                _BytePtr sprite_priority_base = new _BytePtr(sprite_priority, (turbo_fbpla & 7) << 7);
                _BytePtr road_gfxdata_base = new _BytePtr(road_gfxdata, (turbo_opc << 5) & 0x7e0);
                UShortSubArray road_palette_base = new UShortSubArray(road_expanded_palette, (turbo_fbcol & 1) << 4);

                int dx = deltax, dy = deltay, rowsize = (bitmap.line[1].offset - bitmap.line[0].offset) * 8 / bitmap.depth;
                UShortSubArray colortable;
                int x, y, i;

                /* expand the appropriate delta */
                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                    dx *= rowsize;
                else
                    dy *= rowsize;

                /* determine the color offset */
                colortable = new UShortSubArray(Mame.Machine.pens, (turbo_fbcol & 6) << 6);

                /* loop over rows */
                for (y = 4; y < (28 * 8) - 4; y++, _base.offset += dy)
                {
                    int sel, coch, babit, slipar_acciar, area, area1, area2, area3, area4, area5, road = 0;
                    uint[] sprite_data = sprite_buffer;
                    _BytePtr dest = new _BytePtr(_base);

                    /* compute the Y sum between opa and the current scanline (p. 141) */
                    int va = (y + turbo_opa) & 0xff;

                    /* the upper bit of OPC inverts the road */
                    if ((turbo_opc & 0x80) == 0) va ^= 0xff;

                    /* clear the sprite buffer and draw the road sprites */
                    Array.Clear(sprite_buffer, 0, 32 * 8);
                    //memset(sprite_buffer, 0, (32*8) * sizeof(UINT32));
                    draw_road_sprites(sprite_buffer, y);

                    /* loop over 8-pixel chunks */
                    int destoffset = dx * 8;
                    //dest.offset += dx * 8;
                    int sdi = 8;// sprite_data += 8;
                    for (x = 8; x < (32 * 8); x += 8)
                    {
                        int area5_buffer = road_gfxdata_base[0x4000 + (x >> 3)];
                        byte back_data = Generic.videoram[(y / 8) * 32 + (x / 8) - 33];
                        ushort backbits_buffer = back_expanded_data[(back_data << 3) | (y & 7)];

                        //int destoff = 0;// dest.offset;
                        /* loop over columns */
                        for (i = 0; i < 8; i++, destoffset += dx)
                        {
                            uint sprite = sprite_data[sdi++];

                            /* compute the X sum between opb and the current column; only the carry matters (p. 141) */
                            int carry = (x + i + turbo_opb) >> 8;

                            /* the carry selects which inputs to use (p. 141) */
                            if (carry != 0)
                            {
                                sel = turbo_ipb;
                                coch = turbo_ipc >> 4;
                            }
                            else
                            {
                                sel = turbo_ipa;
                                coch = turbo_ipc & 15;
                            }

                            /* at this point we also compute area5 (p. 141) */
                            area5 = (area5_buffer >> 3) & 0x10;
                            area5_buffer <<= 1;

                            /* now look up the rest of the road bits (p. 142) */
                            area1 = road_gfxdata[0x0000 | ((sel & 15) << 8) | va];
                            area1 = ((area1 + x + i) >> 8) & 0x01;
                            area2 = road_gfxdata[0x1000 | ((sel & 15) << 8) | va];
                            area2 = ((area2 + x + i) >> 7) & 0x02;
                            area3 = road_gfxdata[0x2000 | ((sel >> 4) << 8) | va];
                            area3 = ((area3 + x + i) >> 6) & 0x04;
                            area4 = road_gfxdata[0x3000 | ((sel >> 4) << 8) | va];
                            area4 = ((area4 + x + i) >> 5) & 0x08;

                            /* compute the final area value and look it up in IC18/PR1115 (p. 144) */
                            area = area5 | area4 | area3 | area2 | area1;
                            babit = road_enable_collide[area] & 0x07;

                            /* note: SLIPAR is 0 on the road surface only */
                            /*		 ACCIAR is 0 on the road surface and the striped edges only */
                            slipar_acciar = road_enable_collide[area] & 0x30;
                            if (road == 0 && (slipar_acciar & 0x20) != 0)
                            {
                                road = 1;
                                draw_offroad_sprites(sprite_buffer, x + i + 2, y);
                            }

                            /* perform collision detection here */
                            turbo_collision |= collision_map[(uint)((sprite >> 24) & 7) | (uint)(slipar_acciar >> 1)];

                            /* we only need to continue if we're actually drawing */
                            if (true)
                            {
                                int bacol, red, grn, blu, priority, backbits, mx;

                                /* also use the coch value to look up color info in IC13/PR1114 and IC21/PR1117 (p. 144) */
                                bacol = road_palette_base[coch & 15];

                                /* at this point, do the character lookup */
                                backbits = backbits_buffer & 3;
                                backbits_buffer >>= 2;
                                backbits = back_palette[backbits | (back_data & 0xfc)];

                                /* look up the sprite priority in IC11/PR1122 */
                                priority = sprite_priority_base[sprite >> 25];

                                /* use that to look up the overall priority in IC12/PR1123 */
                                mx = overall_priority_base[(uint)((priority & 7) | ((sprite >> 21) & 8) | ((back_data >> 3) & 0x10) | ((backbits << 2) & 0x20) | (babit << 6))];

                                /* the input colors consist of a mix of sprite, road and 1's & 0's */
                                red = (int)(0x040000 | ((bacol & 0x001f) << 13) | ((backbits & 1) << 12) | ((sprite << 4) & 0x0ff0));
                                grn = (int)(0x080000 | ((bacol & 0x03e0) << 9) | ((backbits & 2) << 12) | ((sprite >> 3) & 0x1fe0));
                                blu = (int)(0x100000 | ((bacol & 0x7c00) << 5) | ((backbits & 4) << 12) | ((sprite >> 10) & 0x3fc0));

                                /* we then go through a muxer; normally these values are inverted, but */
                                /* we've already taken care of that when we generated the palette */
                                red = (red >> mx) & 0x10;
                                grn = (grn >> mx) & 0x20;
                                blu = (blu >> mx) & 0x40;
                                dest[destoffset] = (byte)colortable[mx | red | grn | blu];
                                //dest.offset++;
                            }
                        }
                    }
                }
            }
            static void draw_everything_core_16(Mame.osd_bitmap bitmap)
            {
                throw new Exception();
            }
            static void draw_minimal(Mame.osd_bitmap bitmap)
            {
                _BytePtr _base = new _BytePtr(bitmap.line[starty], startx);
                uint[] sprite_buffer = new uint[(32 * 8) + 256];

                _BytePtr overall_priority_base = new _BytePtr(overall_priority, (turbo_fbpla & 8) << 6);
                _BytePtr sprite_priority_base = new _BytePtr(sprite_priority, (turbo_fbpla & 7) << 7);
                _BytePtr road_gfxdata_base = new _BytePtr(road_gfxdata, (turbo_opc << 5) & 0x7e0);
                UShortSubArray road_palette_base = new UShortSubArray(road_expanded_palette, (turbo_fbcol & 1) << 4);

                int dx = deltax, dy = deltay, rowsize = (bitmap.line[1].offset - bitmap.line[0].offset) * 8 / bitmap.depth;
                UShortSubArray colortable;
                int x, y, i;

                /* expand the appropriate delta */
                if ((Mame.Machine.orientation & Mame.ORIENTATION_SWAP_XY) != 0)
                    dx *= rowsize;
                else
                    dy *= rowsize;

                /* determine the color offset */
                colortable = new UShortSubArray(Mame.Machine.pens, (turbo_fbcol & 6) << 6);

                /* loop over rows */
                for (y = 4; y < (28 * 8) - 4; y++, _base.offset += dy)
                {
                    int sel, coch, babit, slipar_acciar, area, area1, area2, area3, area4, area5, road = 0;
                    uint[] sprite_data = sprite_buffer;
                    _BytePtr dest = new _BytePtr(_base);

                    /* compute the Y sum between opa and the current scanline (p. 141) */
                    int va = (y + turbo_opa) & 0xff;

                    /* the upper bit of OPC inverts the road */
                    if ((turbo_opc & 0x80) == 0) va ^= 0xff;

                    /* clear the sprite buffer and draw the road sprites */
                    Array.Clear(sprite_buffer, 0, 32 * 8);// memset(sprite_buffer, 0, (32 * 8) * sizeof(UINT32));
                    draw_road_sprites(sprite_buffer, y);

                    /* loop over 8-pixel chunks */
                    dest.offset += dx * 8;
                    int sdi = 8;// sprite_data += 8;
                    for (x = 8; x < (32 * 8); x += 8)
                    {
                        int area5_buffer = road_gfxdata_base[0x4000 + (x >> 3)];
                        byte back_data = Generic.videoram[(y / 8) * 32 + (x / 8) - 33];
                        ushort backbits_buffer = back_expanded_data[(back_data << 3) | (y & 7)];

                        /* loop over columns */
                        for (i = 0; i < 8; i++, dest.offset += dx)
                        {
                            uint sprite = sprite_data[sdi++];

                            /* compute the X sum between opb and the current column; only the carry matters (p. 141) */
                            int carry = (x + i + turbo_opb) >> 8;

                            /* the carry selects which inputs to use (p. 141) */
                            if (carry != 0)
                            {
                                sel = turbo_ipb;
                                coch = turbo_ipc >> 4;
                            }
                            else
                            {
                                sel = turbo_ipa;
                                coch = turbo_ipc & 15;
                            }

                            /* at this point we also compute area5 (p. 141) */
                            area5 = (area5_buffer >> 3) & 0x10;
                            area5_buffer <<= 1;

                            /* now look up the rest of the road bits (p. 142) */
                            area1 = road_gfxdata[0x0000 | ((sel & 15) << 8) | va];
                            area1 = ((area1 + x + i) >> 8) & 0x01;
                            area2 = road_gfxdata[0x1000 | ((sel & 15) << 8) | va];
                            area2 = ((area2 + x + i) >> 7) & 0x02;
                            area3 = road_gfxdata[0x2000 | ((sel >> 4) << 8) | va];
                            area3 = ((area3 + x + i) >> 6) & 0x04;
                            area4 = road_gfxdata[0x3000 | ((sel >> 4) << 8) | va];
                            area4 = ((area4 + x + i) >> 5) & 0x08;

                            /* compute the final area value and look it up in IC18/PR1115 (p. 144) */
                            area = area5 | area4 | area3 | area2 | area1;
                            babit = road_enable_collide[area] & 0x07;

                            /* note: SLIPAR is 0 on the road surface only */
                            /*		 ACCIAR is 0 on the road surface and the striped edges only */
                            slipar_acciar = road_enable_collide[area] & 0x30;
                            if (road == 0 && (slipar_acciar & 0x20) != 0)
                            {
                                road = 1;
                                draw_offroad_sprites(sprite_buffer, x + i + 2, y);
                            }

                            /* perform collision detection here */
                            turbo_collision |= collision_map[(uint)(((sprite >> 24) & 7) | (slipar_acciar >> 1))];

                            /* we only need to continue if we're actually drawing */
                            if (false)
                            {
                                //int bacol, red, grn, blu, priority, backbits, mx;

                                ///* also use the coch value to look up color info in IC13/PR1114 and IC21/PR1117 (p. 144) */
                                //bacol = road_palette_base[coch & 15];

                                ///* at this point, do the character lookup */
                                //backbits = backbits_buffer & 3;
                                //backbits_buffer >>= 2;
                                //backbits = back_palette[backbits | (back_data & 0xfc)];

                                ///* look up the sprite priority in IC11/PR1122 */
                                //priority = sprite_priority_base[sprite >> 25];

                                ///* use that to look up the overall priority in IC12/PR1123 */
                                //mx = overall_priority_base[(priority & 7) | ((sprite >> 21) & 8) | ((back_data >> 3) & 0x10) | ((backbits << 2) & 0x20) | (babit << 6)];

                                ///* the input colors consist of a mix of sprite, road and 1's & 0's */
                                //red = 0x040000 | ((bacol & 0x001f) << 13) | ((backbits & 1) << 12) | ((sprite << 4) & 0x0ff0);
                                //grn = 0x080000 | ((bacol & 0x03e0) << 9) | ((backbits & 2) << 12) | ((sprite >> 3) & 0x1fe0);
                                //blu = 0x100000 | ((bacol & 0x7c00) << 5) | ((backbits & 4) << 12) | ((sprite >> 10) & 0x3fc0);

                                ///* we then go through a muxer; normally these values are inverted, but */
                                ///* we've already taken care of that when we generated the palette */
                                //red = (red >> mx) & 0x10;
                                //grn = (grn >> mx) & 0x20;
                                //blu = (blu >> mx) & 0x40;
                                //*dest = colortable[mx | red | grn | blu];
                            }
                        }
                    }
                }
            }

        }

        public override void driver_init()
        {
            byte[] led_number_data =
	{
		0x3e,0x41,0x41,0x41,0x00,0x41,0x41,0x41,0x3e,0x00,
		0x00,0x01,0x01,0x01,0x00,0x01,0x01,0x01,0x00,0x00,
		0x3e,0x01,0x01,0x01,0x3e,0x40,0x40,0x40,0x3e,0x00,
		0x3e,0x01,0x01,0x01,0x3e,0x01,0x01,0x01,0x3e,0x00,
		0x00,0x41,0x41,0x41,0x3e,0x01,0x01,0x01,0x00,0x00,
		0x3e,0x40,0x40,0x40,0x3e,0x01,0x01,0x01,0x3e,0x00,
		0x3e,0x40,0x40,0x40,0x3e,0x41,0x41,0x41,0x3e,0x00,
		0x3e,0x01,0x01,0x01,0x00,0x01,0x01,0x01,0x00,0x00,
		0x3e,0x41,0x41,0x41,0x3e,0x41,0x41,0x41,0x3e,0x00,
		0x3e,0x41,0x41,0x41,0x3e,0x01,0x01,0x01,0x3e,0x00
	};

            byte[] led_tach_data =
	{
		0xff,0x00
	};
            Array.Clear(Mame.memory_region(Mame.REGION_GFX4).buffer, Mame.memory_region(Mame.REGION_GFX4).offset, Mame.memory_region_length(Mame.REGION_GFX4));//	memset(memory_region(REGION_GFX4), 0, memory_region_length(REGION_GFX4));
            Buffer.BlockCopy(led_number_data, 0, Mame.memory_region(Mame.REGION_GFX4).buffer, Mame.memory_region(Mame.REGION_GFX4).offset, led_number_data.Length);//memcpy(memory_region(REGION_GFX4), led_number_data, sizeof(led_number_data));
            Buffer.BlockCopy(led_tach_data, 0, Mame.memory_region(Mame.REGION_GFX4).buffer, Mame.memory_region(Mame.REGION_GFX4).offset + 0x100, led_tach_data.Length);// memcpy(memory_region(REGION_GFX4) + 0x100, led_tach_data, sizeof(led_tach_data));
            //rom_decode();
        }

        static void rom_decode()
        {
            /*
             * The table is arranged this way (second half is mirror image of first)
             *
             *		0  1  2	 3	4  5  6	 7	8  9  A	 B	C  D  E	 F
             *
             * 0   00 00 00 00 01 01 01 01 02 02 02 02 03 03 03 03
             * 1   04 04 04 04 05 05 05 05 06 06 06 06 07 07 07 07
             * 2   08 08 08 08 09 09 09 09 0A 0A 0A 0A 0B 0B 0B 0B
             * 3   0C 0C 0C 0C 0D 0D 0D 0D 0E 0E 0E 0E 0F 0F 0F 0F
             * 4   10 10 10 10 11 11 11 11 12 12 12 12 13 13 13 13
             * 5   14 14 14 14 15 15 15 15 16 16 16 16 17 17 17 17
             * 6   18 18 18 18 19 19 19 19 1A 1A 1A 1A 1B 1B 1B 1B
             * 7   1C 1C 1C 1C 1D 1D 1D 1D 1E 1E 1E 1E 1F 1F 1F 1F
             * 8   1F 1F 1F 1F 1E 1E 1E 1E 1D 1D 1D 1D 1C 1C 1C 1C
             * 9   1B 1B 1B 1B 1A 1A 1A 1A 19 19 19 19 18 18 18 18
             * A   17 17 17 17 16 16 16 16 15 15 15 15 14 14 14 14
             * B   13 13 13 13 12 12 12 12 11 11 11 11 10 10 10 10
             * C   0F 0F 0F 0F 0E 0E 0E 0E 0D 0D 0D 0D 0C 0C 0C 0C
             * D   0B 0B 0B 0B 0A 0A 0A 0A 09 09 09 09 08 08 08 08
             * E   07 07 07 07 06 06 06 06 05 05 05 05 04 04 04 04
             * F   03 03 03 03 02 02 02 02 01 01 01 01 00 00 00 00
             *
             */

            byte[][] xortable =
	{
		/* Table 0 */
		/* 0x0000-0x3ff */
		/* 0x0800-0xbff */
		/* 0x4000-0x43ff */
		/* 0x4800-0x4bff */
		new byte[]{ 0x00,0x44,0x0c,0x48,0x00,0x44,0x0c,0x48,
		  0xa0,0xe4,0xac,0xe8,0xa0,0xe4,0xac,0xe8,
		  0x60,0x24,0x6c,0x28,0x60,0x24,0x6c,0x28,
		  0xc0,0x84,0xcc,0x88,0xc0,0x84,0xcc,0x88 },

		/* Table 1 */
		/* 0x0400-0x07ff */
		/* 0x0c00-0x0fff */
		/* 0x1400-0x17ff */
		/* 0x1c00-0x1fff */
		/* 0x2400-0x27ff */
		/* 0x2c00-0x2fff */
		/* 0x3400-0x37ff */
		/* 0x3c00-0x3fff */
		/* 0x4400-0x47ff */
		/* 0x4c00-0x4fff */
		/* 0x5400-0x57ff */
		/* 0x5c00-0x5fff */
		new byte[]{ 0x00,0x44,0x18,0x5c,0x14,0x50,0x0c,0x48,
		  0x28,0x6c,0x30,0x74,0x3c,0x78,0x24,0x60,
		  0x60,0x24,0x78,0x3c,0x74,0x30,0x6c,0x28,
		  0x48,0x0c,0x50,0x14,0x5c,0x18,0x44,0x00 }, //0x00 --> 0x10 ?

		/* Table 2 */
		/* 0x1000-0x13ff */
		/* 0x1800-0x1bff */
		/* 0x5000-0x53ff */
		/* 0x5800-0x5bff */
		new byte[]{ 0x00,0x00,0x28,0x28,0x90,0x90,0xb8,0xb8,
		  0x28,0x28,0x00,0x00,0xb8,0xb8,0x90,0x90,
		  0x00,0x00,0x28,0x28,0x90,0x90,0xb8,0xb8,
		  0x28,0x28,0x00,0x00,0xb8,0xb8,0x90,0x90 },

		/* Table 3 */
		/* 0x2000-0x23ff */
		/* 0x2800-0x2bff */
		/* 0x3000-0x33ff */
		/* 0x3800-0x3bff */
		new byte[]{ 0x00,0x14,0x88,0x9c,0x30,0x24,0xb8,0xac,
		  0x24,0x30,0xac,0xb8,0x14,0x00,0x9c,0x88,
		  0x48,0x5c,0xc0,0xd4,0x78,0x6c,0xf0,0xe4,
		  0x6c,0x78,0xe4,0xf0,0x5c,0x48,0xd4,0xc0 }
	};

            int[] findtable =
	{
		0,1,0,1, /* 0x0000-0x0fff */
		2,1,2,1, /* 0x1000-0x1fff */
		3,1,3,1, /* 0x2000-0x2fff */
		3,1,3,1, /* 0x3000-0x3fff */
		0,1,0,1, /* 0x4000-0x4fff */
		2,1,2,1	 /* 0x5000-0x5fff */
	};

            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            for (int offs = 0x0000; offs < 0x6000; offs++)
            {
                byte src = RAM[offs];
                int i = findtable[offs >> 10];
                int j = src >> 2;
                if ((src & 0x80) != 0) j ^= 0x3f;
                RAM[offs] = (byte)(src ^ xortable[i][j]);
            }
        }

        Mame.RomModule[] rom_turbo()
        {

            ROM_START("turbo");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 64k for code */
            ROM_LOAD("epr1513.bin", 0x0000, 0x2000, 0x0326adfc);
            ROM_LOAD("epr1514.bin", 0x2000, 0x2000, 0x25af63b0);
            ROM_LOAD("epr1515.bin", 0x4000, 0x2000, 0x059c1c36);

            ROM_REGION(0x20000, Mame.REGION_GFX1);/* sprite data */
            ROM_LOAD("epr1246.rom", 0x00000, 0x2000, 0x555bfe9a);
            ROM_RELOAD(0x02000, 0x2000);
            ROM_LOAD("epr1247.rom", 0x04000, 0x2000, 0xc8c5e4d5);
            ROM_RELOAD(0x06000, 0x2000);
            ROM_LOAD("epr1248.rom", 0x08000, 0x2000, 0x82fe5b94);
            ROM_RELOAD(0x0a000, 0x2000);
            ROM_LOAD("epr1249.rom", 0x0c000, 0x2000, 0xe258e009);
            ROM_LOAD("epr1250.rom", 0x0e000, 0x2000, 0xaee6e05e);
            ROM_LOAD("epr1251.rom", 0x10000, 0x2000, 0x292573de);
            ROM_LOAD("epr1252.rom", 0x12000, 0x2000, 0xaee6e05e);
            ROM_LOAD("epr1253.rom", 0x14000, 0x2000, 0x92783626);
            ROM_LOAD("epr1254.rom", 0x16000, 0x2000, 0xaee6e05e);
            ROM_LOAD("epr1255.rom", 0x18000, 0x2000, 0x485dcef9);
            ROM_LOAD("epr1256.rom", 0x1a000, 0x2000, 0xaee6e05e);
            ROM_LOAD("epr1257.rom", 0x1c000, 0x2000, 0x4ca984ce);
            ROM_LOAD("epr1258.rom", 0x1e000, 0x2000, 0xaee6e05e);

            ROM_REGION(0x4800, Mame.REGION_GFX2); /* road data */
            ROM_LOAD("epr1125.rom", 0x0000, 0x0800, 0x65b5d44b);
            ROM_LOAD("epr1126.rom", 0x0800, 0x0800, 0x685ace1b);
            ROM_LOAD("epr1127.rom", 0x1000, 0x0800, 0x9233c9ca);
            ROM_LOAD("epr1238.rom", 0x1800, 0x0800, 0xd94fd83f);
            ROM_LOAD("epr1239.rom", 0x2000, 0x0800, 0x4c41124f);
            ROM_LOAD("epr1240.rom", 0x2800, 0x0800, 0x371d6282);
            ROM_LOAD("epr1241.rom", 0x3000, 0x0800, 0x1109358a);
            ROM_LOAD("epr1242.rom", 0x3800, 0x0800, 0x04866769);
            ROM_LOAD("epr1243.rom", 0x4000, 0x0800, 0x29854c48);

            ROM_REGION(0x1000, Mame.REGION_GFX3);	/* background data */
            ROM_LOAD("epr1244.rom", 0x0000, 0x0800, 0x17f67424);
            ROM_LOAD("epr1245.rom", 0x0800, 0x0800, 0x2ba0b46b);

            ROM_REGION(0x200, Mame.REGION_GFX4);	/* number data (copied at init time) */


            ROM_REGION(0x1000, Mame.REGION_PROMS); /* various PROMs */
            ROM_LOAD("pr1121.bin", 0x0000, 0x0200, 0x7692f497);	/* palette */
            ROM_LOAD("pr1122.bin", 0x0200, 0x0400, 0x1a86ce70);	/* sprite priorities */
            ROM_LOAD("pr1123.bin", 0x0600, 0x0400, 0x02d2cb52);	/* sprite/road/background priorities */
            ROM_LOAD("pr-1118.bin", 0x0a00, 0x0100, 0x07324cfd);	/* background color table */
            ROM_LOAD("pr1114.bin", 0x0b00, 0x0020, 0x78aded46);	/* road red/green color table */
            ROM_LOAD("pr1117.bin", 0x0b20, 0x0020, 0xf06d9907);	/* road green/blue color table */
            ROM_LOAD("pr1115.bin", 0x0b40, 0x0020, 0x5394092c);	/* road collision/enable */
            ROM_LOAD("pr1116.bin", 0x0b60, 0x0020, 0x3956767d);	/* collision detection */
            ROM_LOAD("sndprom.bin", 0x0b80, 0x0020, 0xb369a6ae);
            ROM_LOAD("pr-1119.bin", 0x0c00, 0x0200, 0x628d3f1d);	/* timing - not used */
            ROM_LOAD("pr-1120.bin", 0x0e00, 0x0200, 0x591b6a68);	/* timing - not used */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_turbo()
        {

            INPUT_PORTS_START("turbo");
            PORT_START();	/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);	/* ACCEL B */
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);/* ACCEL A */
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON3 | IPF_TOGGLE);	/* SHIFT */
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);
            PORT_SERVICE(0x10, IP_ACTIVE_LOW);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN4);				/* SERVICE */
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);

            PORT_START();	/* DSW 1 */
            PORT_DIPNAME(0x03, 0x03, "Car On Extended Play");
            PORT_DIPSETTING(0x03, "1");
            PORT_DIPSETTING(0x02, "2");
            PORT_DIPSETTING(0x01, "3");
            PORT_DIPSETTING(0x00, "4");
            PORT_DIPNAME(0x04, 0x04, "Game Time");
            PORT_DIPSETTING(0x00, "Fixed (55 sec)");
            PORT_DIPSETTING(0x04, "Adjustable");
            PORT_DIPNAME(0x08, 0x00, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Hard");
            PORT_DIPNAME(0x10, 0x00, "Game Mode");
            PORT_DIPSETTING(0x10, "No Collisions (cheat)");
            PORT_DIPSETTING(0x00, "Normal");
            PORT_DIPNAME(0x20, 0x00, "Initial Entry");
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_BIT(0xc0, 0xc0, IPT_UNUSED);

            PORT_START();	/* DSW 2 */
            PORT_DIPNAME(0x03, 0x03, "Game Time");
            PORT_DIPSETTING(0x00, "60 seconds");
            PORT_DIPSETTING(0x01, "70 seconds");
            PORT_DIPSETTING(0x02, "80 seconds");
            PORT_DIPSETTING(0x03, "90 seconds");
            PORT_DIPNAME(0x1c, 0x1c, DEF_STR(Coin_B));
            PORT_DIPSETTING(0x18, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x14, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("2C_1C"));
            /*	PORT_DIPSETTING(	0x00, DEF_STR( "1C_1C" ))*/
            PORT_DIPSETTING(0x1c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x08, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_6C"));
            PORT_DIPNAME(0xe0, 0xe0, DEF_STR(Coin_A));
            PORT_DIPSETTING(0xc0, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0xa0, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("2C_1C"));
            /*	PORT_DIPSETTING(	0x00, DEF_STR( "1C_1C" ))*/
            PORT_DIPSETTING(0xe0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_6C"));

            PORT_START();	/* DSW 3 */
            PORT_BIT(0x0f, 0x00, IPT_UNUSED);	/* Merged with collision bits */
            PORT_BIT(0x30, 0x00, IPT_UNUSED);
            PORT_DIPNAME(0x40, 0x40, "Tachometer");
            PORT_DIPSETTING(0x40, "Analog (Meter)");
            PORT_DIPSETTING(0x00, "Digital (led)");
            PORT_DIPNAME(0x80, 0x80, "Sound System");
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, "Cockpit");

            PORT_START();		/* IN0 */
            PORT_ANALOG(0xff, 0, (uint)inptports.IPT_DIAL | IPF_CENTER, 10, 30, 0, 0);
            return INPUT_PORTS_END;
        }
        public driver_turbo()
        {
            drv = new machine_driver_turbo();
            year = "1981";
            name = "turbo";
            description = "Turbo";
            manufacturer = "Sega";
            flags = Mame.ROT270 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_turbo();
            rom = rom_turbo();
            drv.HasNVRAMhandler = false;
        }
    }
}
