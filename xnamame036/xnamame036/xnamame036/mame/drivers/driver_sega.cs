using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_sega : Mame.GameDriver
    {
        public static void COINAGE()
        {
            PORT_START();
            PORT_DIPNAME(0x0f, 0x0c, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x09, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x05, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x0d, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x03, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x0b, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0f, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x07, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0x0a, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_6C"));
            PORT_DIPNAME(0xf0, 0xc0, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x90, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x50, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0xd0, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x30, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0xb0, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0xf0, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x70, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0xa0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_6C"));
        }
        static _BytePtr sega_mem = new _BytePtr(1);
        static byte mult1;
        static short result;
        static byte ioSwitch;

        public static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0xbfff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0xc800, 0xcfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xd000, 0xdfff, Mame.MRA_RAM ),			/* sound ram */
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        public static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0xFFFF, sega_wr, sega_mem ),
	new Mame.MemoryWriteAddress(0xe000, 0xefff, Mame.MWA_RAM, Vector.vectorram, Vector.vectorram_size ),	/* handled by the above, */
												/* here only to initialize the pointer */
	new Mame.MemoryWriteAddress( -1 )
};
        public static int sega_interrupt()
        {
            if ((Mame.input_port_5_r(0) & 0x01) != 0)
                return Mame.nmi_interrupt();
            else
                return Mame.interrupt();
        }
        static void sega_wr(int offset, int data)
        {
            int pc, off;

            pc = (int)Mame.cpu_getpreviouspc();
            off = offset;

            /* Check if this is a valid PC (ie not a spurious stack write) */
            if (pc != -1)
            {
                int op, page;
                uint bad;

                op = sega_mem[pc] & 0xFF;
                if (op == 0x32)
                {
                    bad = (uint)(sega_mem[pc + 1] & 0xFF);
                    page = (sega_mem[pc + 2] & 0xFF) << 8;
                    segar.sega_decrypt(pc, ref bad);
                    off = (int)((page & 0xFF00) | (bad & 0x00FF));
                }
            }


            /* MWA_ROM */
            if ((off >= 0x0000) && (off <= 0xbfff))
            {
                ;
            }
            /* MWA_RAM */
            else if ((off >= 0xc800) && (off <= 0xefff))
            {
                sega_mem[off] = (byte)data;
            }
        }

        public override void driver_init()
        {

        }
        public static void sega_mult1_w(int offset, int data)
        {
            mult1 = (byte)data;
        }

        public static void sega_mult2_w(int offset, int data)
        {
            /* Curiously, the multiply is _only_ calculated by writes to this port. */
            result = (short)(mult1 * data);
        }

        public static void sega_switch_w(int offset, int data)
        {
            ioSwitch = (byte)data;
            /*	if (errorlog) fprintf (errorlog,"ioSwitch: %02x\n",ioSwitch); */
        }
        public static int sega_mult_r(int offset)
        {
            int c;

            c = result & 0xff;
            result >>= 8;
            return (c);
        }
        public static int sega_read_ports(int offset)
        {
            int dip1, dip2;

            dip1 = Mame.input_port_6_r(offset);
            dip2 = Mame.input_port_7_r(offset);

            switch (offset)
            {
                case 0:
                    return ((Mame.input_port_0_r(0) & 0xF0) | ((dip2 & 0x08) >> 3) |
                         ((dip2 & 0x80) >> 6) | ((dip1 & 0x08) >> 1) | ((dip1 & 0x80) >> 4));
                case 1:
                    return ((Mame.input_port_1_r(0) & 0xF0) | ((dip2 & 0x04) >> 2) |
                         ((dip2 & 0x40) >> 5) | ((dip1 & 0x04) >> 0) | ((dip1 & 0x40) >> 3));
                case 2:
                    return ((Mame.input_port_2_r(0) & 0xF0) | ((dip2 & 0x02) >> 1) |
                         ((dip2 & 0x20) >> 4) | ((dip1 & 0x02) << 1) | ((dip1 & 0x20) >> 2));
                case 3:
                    return ((Mame.input_port_3_r(0) & 0xF0) | ((dip2 & 0x01) >> 0) |
                         ((dip2 & 0x10) >> 3) | ((dip1 & 0x01) << 2) | ((dip1 & 0x10) >> 1));
            }

            return 0;
        }


        static int sign;
        static int spinner;
        public static int sega_IN4_r(int offset)
        {

            /*
             * The values returned are always increasing.  That is, regardless of whether
             * you turn the spinner left or right, the self-test should always show the
             * number as increasing. The direction is only reflected in the least
             * significant bit.
             */

            int delta;

            if ((ioSwitch & 1) != 0) /* ioSwitch = 0x01 or 0xff */
                return Mame.readinputport(4);

            /* else ioSwitch = 0xfe */

            /* I'm sure this can be further simplified ;-) BW */
            delta = Mame.readinputport(8);
            if (delta != 0)
            {
                sign = delta >> 7;
                if (sign != 0)
                    delta = 0x80 - delta;
                spinner += delta;
            }
            return (~((spinner << 1) | sign));
        }
    }
    class machine_driver_sega : Mame.MachineDriver
    {

        const byte VEC_SHIFT = 15;	/* do not use a higher value. Values will overflow */

        static int width, height, cent_x, cent_y, min_x, min_y, max_x, max_y;
        static int[] sinTable, cosTable;
        static int intensity;
        public override void init_machine()
        {
           //none
        }
        public override void nvram_handler(object file, int read_or_write)
        {
            throw new NotImplementedException();
        }
        public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
        {
            int i;
            /* Bits are. Red: 6&5 (0x60), Green: 4&3 (0x18), Blue: 2&1 (0x06) */
            for (i = 0; i < 128; i += 2)
            {
                palette[3 * i] = (byte)(85 * ((i >> 5) & 0x3));
                palette[3 * i + 1] = (byte)(85 * ((i >> 3) & 0x3));
                palette[3 * i + 2] = (byte)(85 * ((i >> 1) & 0x3));
                /* Set the color table */
                colortable[i]= (ushort)i;
            }
            /*
             * Fill in the holes with good anti-aliasing colors.  This is a very good
             * range of colors based on the previous palette entries.     .ac JAN2498
             */
            i = 1;
            for (int r = 0; r <= 6; r++)
            {
                for (int g = 0; g <= 6; g++)
                {
                    for (int b = 0; b <= 6; b++)
                    {
                        if (((r | g | b) & 0x1) == 0) continue;
                        if ((g == 5 || g == 6) && (b == 1 || b == 2 || r == 1 || r == 2)) continue;
                        if ((g == 3 || g == 4) && (b == 1 || r == 1)) continue;
                        if ((b == 6 || r == 6) && (g == 1 || g == 2)) continue;
                        if ((r == 5) && (b == 1)) continue;
                        if ((b == 5) && (r == 1)) continue;
                        palette[3 * i] = (byte)((255 * r) / 6);
                        palette[3 * i + 1] = (byte)((255 * g) / 6);
                        palette[3 * i + 2] = (byte)((255 * b) / 6);
                        colortable[i]= (ushort)i;
                        if (i < 128)
                            i += 2;
                        else
                            i++;
                    }
                }
            }
            /* There are still 4 colors left, just going to put some grays in. */
            for (i = 252; i <= 255; i++)
            {
                palette[3 * i] =
                palette[3 * i + 1] =
                palette[3 * i + 2] = (byte)(107 + (42 * (i - 252)));
            }
        }
        public override int vh_start()
        {
            int i;

            if (Vector.vectorram_size[0] == 0)
                return 1;
            min_x = Mame.Machine.drv.visible_area.min_x;
            min_y = Mame.Machine.drv.visible_area.min_y;
            max_x = Mame.Machine.drv.visible_area.max_x;
            max_y = Mame.Machine.drv.visible_area.max_y;
            width = max_x - min_x;
            height = max_y - min_y;
            cent_x = (max_x + min_x) / 2;
            cent_y = (max_y + min_y) / 2;

            Vector.vector_set_shift(VEC_SHIFT);

            /* allocate memory for the sine and cosine lookup tables ASG 080697 */
            sinTable = new int[0x400];
            cosTable = new int[0x400];

            /* generate the sine/cosine lookup tables */
            for (i = 0; i < 0x400; i++)
            {
                double angle = ((2.0 * Math.PI) / (double)0x400) * (double)i;
                double temp;

                temp = Math.Sin(angle);
                if (temp < 0)
                    sinTable[i] = (int)(temp * (double)(1 << VEC_SHIFT) - 0.5);
                else
                    sinTable[i] = (int)(temp * (double)(1 << VEC_SHIFT) + 0.5);

                temp = Math.Cos(angle);
                if (temp < 0)
                    cosTable[i] = (int)(temp * (double)(1 << VEC_SHIFT) - 0.5);
                else
                    cosTable[i] = (int)(temp * (double)(1 << VEC_SHIFT) + 0.5);

            }

            return Vector.vector_vh_start();
        }
        public override void vh_stop()
        {
            throw new NotImplementedException();
        }
        void sega_generate_vector_list()
        {
            int deltax, deltay;
            int currentX, currentY;

            int vectorIndex;
            int symbolIndex;

            int rotate, scale;
            int attrib;

            int angle, length;
            int color;

            int draw;

            Vector.vector_clear_list();

            symbolIndex = 0;	/* Reset vector PC to 0 */

            /*
             * walk the symbol list until 'last symbol' set
             */

            do
            {
                draw = Vector.vectorram[symbolIndex++];

                if ((draw & 1) != 0)	/* if symbol active */
                {
                    currentX = Vector.vectorram[symbolIndex + 0] | (Vector.vectorram[symbolIndex + 1] << 8);
                    currentY = Vector.vectorram[symbolIndex + 2] | (Vector.vectorram[symbolIndex + 3] << 8);
                    vectorIndex = Vector.vectorram[symbolIndex + 4] | (Vector.vectorram[symbolIndex + 5] << 8);
                    rotate = Vector.vectorram[symbolIndex + 6] | (Vector.vectorram[symbolIndex + 7] << 8);
                    scale = Vector.vectorram[symbolIndex + 8];

                    currentX = ((currentX & 0x7ff) - min_x) << VEC_SHIFT;
                    currentY = (max_y - (currentY & 0x7ff)) << VEC_SHIFT;
                    Vector.vector_add_point(currentX, currentY, 0, 0);
                    vectorIndex &= 0xfff;

                    /* walk the vector list until 'last vector' bit */
                    /* is set in attributes */

                    do
                    {
                        attrib = Vector.vectorram[vectorIndex + 0];
                        length = Vector.vectorram[vectorIndex + 1];
                        angle = Vector.vectorram[vectorIndex + 2] | (Vector.vectorram[vectorIndex + 3] << 8);

                        vectorIndex += 4;

                        /* calculate deltas based on len, angle(s), and scale factor */

                        angle = (angle + rotate) & 0x3ff;
                        deltax = sinTable[angle] * scale * length;
                        deltay = cosTable[angle] * scale * length;

                        currentX += deltax >> 7;
                        currentY -= deltay >> 7;

                        color = attrib & 0x7e;
                        if ((attrib & 1) != 0 && color != 0)
                        {
                            if (Vector.translucency != 0)
                                intensity = 0xa0; /* leave room for translucency */
                            else
                                intensity = 0xff;
                        }
                        else
                            intensity = 0;
                        Vector.vector_add_point(currentX, currentY, color, intensity);

                    } while ((attrib & 0x80) == 0);
                }

                symbolIndex += 9;
                if (symbolIndex >= Vector.vectorram_size[0])
                    break;

            } while ((draw & 0x80) == 0);
        }
        public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            sega_generate_vector_list();
            Vector.vector_vh_update(bitmap, full_refresh);
        }
        public override void vh_eof_callback()
        {
            //none
        }
    }
    class driver_zektor : driver_sega
    {
        static string[] zektor_sample_names = {
	"*zektor",
	"zk01.wav",  /* 1 */
	"zk02.wav",
	"zk03.wav",
	"zk04.wav",
	"zk05.wav",
	"zk06.wav",
	"zk07.wav",
	"zk08.wav",
	"zk09.wav",
	"zk0a.wav",
	"zk0b.wav",
	"zk0c.wav",
	"zk0d.wav",
	"zk0e.wav",
	"zk0f.wav",
	"zk10.wav",
	"zk11.wav",
	"zk12.wav",
	"zk13.wav",
	"elim1.wav",  /* 19 fireball */
	"elim2.wav",  /* 20 bounce */
	"elim3.wav",  /* 21 Skitter */
	"elim4.wav",  /* 22 Eliminator */
	"elim5.wav",  /* 23 Electron */
	"elim6.wav",  /* 24 fire */
	"elim7.wav",  /* 25 thrust */
	"elim8.wav",  /* 26 Electron */
	"elim9.wav",  /* 27 small explosion */
	"elim10.wav", /* 28 med explosion */
	"elim11.wav", /* 29 big explosion */
				  /* Missing Zizzer */
				  /* Missing City fly by */
				  /* Missing Rotation Rings */


    null	/* end of array */
};
        static Mame.Samplesinterface zektor_samples_interface = new Mame.Samplesinterface(12, 25, zektor_sample_names);
        public static Mame.IOReadPort[] zektor_readport =
{
	new  Mame.IOReadPort(0x3f, 0x3f, Sega.sega_sh_r ),
	new  Mame.IOReadPort(0xbe, 0xbe, driver_sega.sega_mult_r ),
	new  Mame.IOReadPort(0xf8, 0xfb, driver_sega.sega_read_ports ),
	new  Mame.IOReadPort(0xfc, 0xfc, driver_sega.sega_IN4_r ),
	new  Mame.IOReadPort(-1 )	/* end of table */
};

        static Mame.IOWritePort[] zektor_writeport =
{
	new Mame.IOWritePort( 0x38, 0x38, Sega.sega_sh_speech_w ),
    new Mame.IOWritePort( 0x3e, 0x3e, Sega.zektor1_sh_w ),
    new Mame.IOWritePort( 0x3f, 0x3f, Sega.zektor2_sh_w ),
	new Mame.IOWritePort( 0xbd, 0xbd, driver_sega.sega_mult1_w ),
	new Mame.IOWritePort( 0xbe, 0xbe, driver_sega.sega_mult2_w ),
	new Mame.IOWritePort( 0xf8, 0xf8, driver_sega.sega_switch_w ),
	new Mame.IOWritePort( 0xf9, 0xf9, Mame.coin_counter_w ), /* 0x80 = enable, 0x00 = disable */
	new Mame.IOWritePort( -1 )	/* end of table */
};
        
        class machine_driver_zektor : machine_driver_sega
        {
            public machine_driver_zektor()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3867120, readmem, writemem, zektor_readport, zektor_writeport, null, 0, driver_sega.sega_interrupt, 40));
                frames_per_second = 40;
                vblank_duration = 0;
                cpu_slices_per_frame = 1;
                screen_width = 400;
                screen_height = 300;
                visible_area = new Mame.rectangle(512, 1536, 624, 1432);
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;

                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, zektor_samples_interface));
            }
            public override void init_machine()
            {
                //none
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }

        }
        public override void driver_init()
        {
            segar.sega_security(82);
        }
        Mame.RomModule[] rom_zektor()
        {
            ROM_START("zektor");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("1611.cpu", 0x0000, 0x0800, 0x6245aa23);
            ROM_LOAD("1586.rom", 0x0800, 0x0800, 0xefeb4fb5);
            ROM_LOAD("1587.rom", 0x1000, 0x0800, 0xdaa6c25c);
            ROM_LOAD("1588.rom", 0x1800, 0x0800, 0x62b67dde);
            ROM_LOAD("1589.rom", 0x2000, 0x0800, 0xc2db0ba4);
            ROM_LOAD("1590.rom", 0x2800, 0x0800, 0x4d948414);
            ROM_LOAD("1591.rom", 0x3000, 0x0800, 0xb0556a6c);
            ROM_LOAD("1592.rom", 0x3800, 0x0800, 0x750ecadf);
            ROM_LOAD("1593.rom", 0x4000, 0x0800, 0x34f8850f);
            ROM_LOAD("1594.rom", 0x4800, 0x0800, 0x52b22ab2);
            ROM_LOAD("1595.rom", 0x5000, 0x0800, 0xa704d142);
            ROM_LOAD("1596.rom", 0x5800, 0x0800, 0x6975e33d);
            ROM_LOAD("1597.rom", 0x6000, 0x0800, 0xd48ab5c2);
            ROM_LOAD("1598.rom", 0x6800, 0x0800, 0xab54a94c);
            ROM_LOAD("1599.rom", 0x7000, 0x0800, 0xc9d4f3a5);
            ROM_LOAD("1600.rom", 0x7800, 0x0800, 0x893b7dbc);
            ROM_LOAD("1601.rom", 0x8000, 0x0800, 0x867bdf4f);
            ROM_LOAD("1602.rom", 0x8800, 0x0800, 0xbd447623);
            ROM_LOAD("1603.rom", 0x9000, 0x0800, 0x9f8f10e8);
            ROM_LOAD("1604.rom", 0x9800, 0x0800, 0xad2f0f6c);
            ROM_LOAD("1605.rom", 0xa000, 0x0800, 0xe27d7144);
            ROM_LOAD("1606.rom", 0xa800, 0x0800, 0x7965f636);
            return ROM_END;

        }
        Mame.InputPortTiny[] input_ports_zektor()
        {
            INPUT_PORTS_START("zektor");
            PORT_START();//	/* IN0 - port 0xf8 */
            /* The next bit is referred to as the Service switch in the self test - it just adds a credit */
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 3);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 3);
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 3);

            PORT_START();	/* IN1 - port 0xf9 */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN2 - port 0xfa */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN3 - port 0xfb */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN4 - port 0xfc - read in machine/sega.c */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN5 - FAKE */
            /* This fake input port is used to get the status of the F2 key, */
            /* and activate the test mode, which is triggered by a NMI */
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, DEF_STR("Service Mode"), (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);

            PORT_START();	/* FAKE */
            /* This fake input port is used for DIP Switch 1 */
            PORT_DIPNAME(0x03, 0x01, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x03, "10000");
            PORT_DIPSETTING(0x01, "20000");
            PORT_DIPSETTING(0x02, "30000");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Normal");
            PORT_DIPSETTING(0x04, "Hard");
            PORT_DIPSETTING(0x0c, "Very Hard");
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Lives"));
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x20, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x30, "5");
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));

            PORT_START();
            PORT_DIPNAME(0x0f, 0x0c, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x09, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x05, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0x04, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x0c, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x0d, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x03, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0x0b, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x02, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x0f, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x07, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0x0a, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x0e, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x01, DEF_STR("1C_6C"));
            PORT_DIPNAME(0xf0, 0xc0, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x80, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x90, "2 Coins/1 Credit 5/3 6/4");
            PORT_DIPSETTING(0x50, "2 Coins/1 Credit 4/3");
            PORT_DIPSETTING(0x40, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0xd0, "1 Coin/1 Credit 5/6");
            PORT_DIPSETTING(0x30, "1 Coin/1 Credit 4/5");
            PORT_DIPSETTING(0xb0, "1 Coin/1 Credit 2/3");
            PORT_DIPSETTING(0x20, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0xf0, "1 Coin/2 Credits 4/9");
            PORT_DIPSETTING(0x70, "1 Coin/2 Credits 5/11");
            PORT_DIPSETTING(0xa0, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x60, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0xe0, DEF_STR("1C_5C"));
            PORT_DIPSETTING(0x10, DEF_STR("1C_6C"));


            PORT_START();      /* IN8 - FAKE port for the dial */
            PORT_ANALOG(0xff, 0x00, (uint)inptports.IPT_DIAL | IPF_CENTER, 100, 10, 0, 0);
            return INPUT_PORTS_END;
        }
        public driver_zektor()
        {
            drv = new machine_driver_zektor();
            year = "1982";
            name = "zektor";
            description = "Zektor";
            manufacturer = "Sega";
            flags = Mame.ROT0;
            input_ports = input_ports_zektor();
            rom = rom_zektor();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_tacscan : driver_sega
    {
        static string[] tacscan_sample_names = {
	"*tacscan",
	/* Player ship thrust sounds */
	"01.wav",
	"02.wav",
	"03.wav",
        "plaser.wav",
	"pexpl.wav",
	"pship.wav",
	"tunnelh.wav",
	"sthrust.wav",
	"slaser.wav",
	"sexpl.wav",
	"eshot.wav",
	"eexpl.wav",
        "tunnelw.wav",
        "flight1.wav",
        "flight2.wav",
        "flight3.wav",
        "flight4.wav",
        "flight5.wav",
        "formatn.wav",
        "warp.wav",
        "credit.wav",
        "1up.wav",

    null	/* end of array */
};
        static Mame.Samplesinterface tacscan_samples_interface =
new Mame.Samplesinterface(
	12,	/* 12 channels */
	25,	/* volume */
	tacscan_sample_names
);

//        static Mame.CustomSoundInterface tacscan_custom_interface =
//new Mame.CustomSoundInterface(
//    tacscan_sh_start,
//    0,
//    tacscan_sh_update
//);
        static Mame.IOWritePort[] tacscan_writeport =
{
	new Mame.IOWritePort( 0x3f, 0x3f, tacscan_sh_w ),
	new Mame.IOWritePort( 0xbd, 0xbd, sega_mult1_w ),
	new Mame.IOWritePort( 0xbe, 0xbe, sega_mult2_w ),
	new Mame.IOWritePort( 0xf8, 0xf8, sega_switch_w ),
	new Mame.IOWritePort( 0xf9, 0xf9, Mame.coin_counter_w ), /* 0x80 = enable, 0x00 = disable */
	new Mame.IOWritePort( -1 )	/* end of table */
};
const byte 	shipStop =0x10;
const byte  shipLaser =0x18;
const byte 	shipExplosion= 0x20;
const byte 	shipDocking= 0x28;
const byte 	shipRoar =0x40;
const byte 	tunnelHigh =0x48;
const byte 	stingerThrust =0x50;
const byte 	stingerLaser =0x51;
const byte 	stingerStop =0x52;
const byte 	stingerExplosion= 0x54;
const byte 	enemyBullet0 =0x61;
const byte 	enemyBullet1 =0x62;
const byte 	enemyBullet2 =0x63;
const byte 	enemyExplosion0 =0x6c;
const byte 	enemyExplosion1 =0x6d;
const byte 	enemyExplosion2 =0x6e;
const byte  tunnelw   =0x09;
const byte  flight1   =0x36;
const byte  flight2   =0x3b;
const byte  flight3   =0x3d;
const byte  flight4   =0x3e;
const byte  flight5   =0x3f;
const byte  warp      =0x37;
const byte  formation =0x0b;
const byte  nothing1  =0x1a;
const byte  nothing2  =0x1b;
const byte  extralife =0x1c;
const byte credit = 0x2c;

const byte kVoiceShipRoar = 5;
const byte 	kVoiceShip =1;
const byte 	kVoiceTunnel= 2;
const byte 	kVoiceStinger= 3;
const byte 	kVoiceEnemy =4;
const byte  kVoiceExtra =8;
const byte  kVoiceForm =7;
const byte  kVoiceWarp =6;
const byte  kVoiceExtralife =9;

static int roarPlaying;	/* Is the ship roar noise playing? */

        static void tacscan_sh_w(int offset, int data)
        {
            int sound;   /* index into the sample name array in drivers/sega.c */
            int voice = 0; /* which voice to play the sound on */
            int loop;    /* is this sound continuous? */

            loop = 0;
            switch (data)
            {
                case shipRoar:
                    /* Play the increasing roar noise */
                    voice = kVoiceShipRoar;
                    sound = 0;
                    roarPlaying = 1;
                    break;
                case shipStop:
                    /* Play the decreasing roar noise */
                    voice = kVoiceShipRoar;
                    sound = 2;
                    roarPlaying = 0;
                    break;
                case shipLaser:
                    voice = kVoiceShip;
                    sound = 3;
                    break;
                case shipExplosion:
                    voice = kVoiceShip;
                    sound = 4;
                    break;
                case shipDocking:
                    voice = kVoiceShip;
                    sound = 5;
                    break;
                case tunnelHigh:
                    voice = kVoiceTunnel;
                    sound = 6;
                    break;
                case stingerThrust:
                    voice = kVoiceStinger;
                    sound = 7;
                    loop = 0; //leave off sound gets stuck on
                    break;
                case stingerLaser:
                    voice = kVoiceStinger;
                    sound = 8;
                    loop = 0;
                    break;
                case stingerExplosion:
                    voice = kVoiceStinger;
                    sound = 9;
                    break;
                case stingerStop:
                    voice = kVoiceStinger;
                    sound = -1;
                    break;
                case enemyBullet0:
                case enemyBullet1:
                case enemyBullet2:
                    voice = kVoiceEnemy;
                    sound = 10;
                    break;
                case enemyExplosion0:
                case enemyExplosion1:
                case enemyExplosion2:
                    voice = kVoiceTunnel;
                    sound = 11;
                    break;
                case tunnelw: voice = kVoiceShip;
                    sound = 12;
                    break;
                case flight1: voice = kVoiceExtra;
                    sound = 13;
                    break;
                case flight2: voice = kVoiceExtra;
                    sound = 14;
                    break;
                case flight3: voice = kVoiceExtra;
                    sound = 15;
                    break;
                case flight4: voice = kVoiceExtra;
                    sound = 16;
                    break;
                case flight5: voice = kVoiceExtra;
                    sound = 17;
                    break;
                case formation:
                    voice = kVoiceForm;
                    sound = 18;
                    break;
                case warp: voice = kVoiceExtra;
                    sound = 19;
                    break;
                case extralife: voice = kVoiceExtralife;
                    sound = 20;
                    break;
                case credit: voice = kVoiceExtra;
                    sound = 21;
                    break;

                default:

                    /* don't play anything */
                    sound = -1;
                    break;
            }
            if (sound != -1)
            {
                Mame.sample_stop(voice);
                /* If the game is over, turn off the stinger noise */
                if (data == shipStop)
                    Mame.sample_stop(kVoiceStinger);
                Mame.sample_start(voice, sound, loop);
            }
        }

        class machine_driver_tacscan : machine_driver_sega
        {
            public machine_driver_tacscan()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 3867120, readmem, writemem, driver_zektor.zektor_readport, tacscan_writeport, null, 0, sega_interrupt, 40));
                frames_per_second = 40;
                vblank_duration = 0;
                cpu_slices_per_frame = 1;
                screen_width = 400;
                screen_height = 300;
                visible_area = new Mame.rectangle(496, 1552, 592, 1456);
                total_colors = 256;
                color_table_len = 256;
                video_attributes = Mame.VIDEO_TYPE_VECTOR;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, tacscan_samples_interface));
                //sound.Add(new Mame.MachineSound(Mame.SOUND_CUSTOM, tacscan_custom_interface));
            }
        }
        Mame.InputPortTiny[] input_ports_tacscan()
        {
            INPUT_PORTS_START("tacscan");
            PORT_START();	/* IN0 - port 0xf8 */
            /* The next bit is referred to as the Service switch in the self test - it just adds a credit */
            PORT_BIT_IMPULSE(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3, 3);
            PORT_BIT_IMPULSE(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2, 3);
            PORT_BIT_IMPULSE(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1, 3);

            PORT_START();	/* IN1 - port 0xf9 */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN2 - port 0xfa */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN3 - port 0xfb */
            PORT_BIT(0xf0, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* IN4 - port 0xfc - read in machine/sega.c */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0xf0, IP_ACTIVE_HIGH, IPT_UNUSED);

            PORT_START();	/* IN5 - FAKE */
            /* This fake input port is used to get the status of the F2 key, */
            /* and activate the test mode, which is triggered by a NMI */
            PORT_BITX(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, DEF_STR("Service Mode"), (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);

            PORT_START();	/* FAKE */
            /* This fake input port is used for DIP Switch 1 */
            PORT_DIPNAME(0x03, 0x01, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x03, "10000");
            PORT_DIPSETTING(0x01, "20000");
            PORT_DIPSETTING(0x02, "30000");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPNAME(0x0c, 0x00, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x08, "Normal");
            PORT_DIPSETTING(0x04, "Hard");
            PORT_DIPSETTING(0x0c, "Very Hard");
            PORT_DIPNAME(0x30, 0x30, "Number of Ships");
            PORT_DIPSETTING(0x00, "2");
            PORT_DIPSETTING(0x20, "4");
            PORT_DIPSETTING(0x10, "6");
            PORT_DIPSETTING(0x30, "8");
            PORT_DIPNAME(0x40, 0x00, "Demo Sounds?");
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x80, DEF_STR("Upright"));
            PORT_DIPSETTING(0x00, DEF_STR("Cocktail"));

            COINAGE();

            PORT_START();      /* IN8 - FAKE port for the dial */
            PORT_ANALOG(0xff, 0x00, (uint)inptports.IPT_DIAL | IPF_CENTER, 100, 10, 0, 0);
            return INPUT_PORTS_END;
        }
        Mame.RomModule[] rom_tacscan()
        {
            ROM_START("tacscan");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("1711a", 0x0000, 0x0800, 0x0da13158);
            ROM_LOAD("1670c", 0x0800, 0x0800, 0x98de6fd5);
            ROM_LOAD("1671a", 0x1000, 0x0800, 0xdc400074);
            ROM_LOAD("1672a", 0x1800, 0x0800, 0x2caf6f7e);
            ROM_LOAD("1673a", 0x2000, 0x0800, 0x1495ce3d);
            ROM_LOAD("1674a", 0x2800, 0x0800, 0xab7fc5d9);
            ROM_LOAD("1675a", 0x3000, 0x0800, 0xcf5e5016);
            ROM_LOAD("1676a", 0x3800, 0x0800, 0xb61a3ab3);
            ROM_LOAD("1677a", 0x4000, 0x0800, 0xbc0273b1);
            ROM_LOAD("1678b", 0x4800, 0x0800, 0x7894da98);
            ROM_LOAD("1679a", 0x5000, 0x0800, 0xdb865654);
            ROM_LOAD("1680a", 0x5800, 0x0800, 0x2c2454de);
            ROM_LOAD("1681a", 0x6000, 0x0800, 0x77028885);
            ROM_LOAD("1682a", 0x6800, 0x0800, 0xbabe5cf1);
            ROM_LOAD("1683a", 0x7000, 0x0800, 0x1b98b618);
            ROM_LOAD("1684a", 0x7800, 0x0800, 0xcb3ded3b);
            ROM_LOAD("1685a", 0x8000, 0x0800, 0x43016a79);
            ROM_LOAD("1686a", 0x8800, 0x0800, 0xa4397772);
            ROM_LOAD("1687a", 0x9000, 0x0800, 0x002f3bc4);
            ROM_LOAD("1688a", 0x9800, 0x0800, 0x0326d87a);
            ROM_LOAD("1709a", 0xa000, 0x0800, 0xf35ed1ec);
            ROM_LOAD("1710a", 0xa800, 0x0800, 0x6203be22);
            return ROM_END;
        }
        public override void driver_init()
        {
            segar.sega_security(76);
        }
        public driver_tacscan()
        {
            drv = new machine_driver_tacscan();
            year = "1982";
            name = "tacscan";
            description = "Tac/Scan";
            manufacturer = "Sega";
            flags = Mame.ROT270;
            input_ports = input_ports_tacscan();
            rom = rom_tacscan();
            drv.HasNVRAMhandler = false;
        }
    }
}
