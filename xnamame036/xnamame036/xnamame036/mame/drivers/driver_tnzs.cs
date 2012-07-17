using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class tnzs : Mame.GameDriver
    {
        public static _BytePtr tnzs_objram = new _BytePtr(1);
        public static _BytePtr tnzs_workram = new _BytePtr(1);
        public static _BytePtr tnzs_vdcram = new _BytePtr(1);
        public static _BytePtr tnzs_scrollram = new _BytePtr(1);

        public const byte MAX_SAMPLES = 0x2f;
        public static int kageki_csport_sel = 0;
        public static int mcu_type;
        public const byte MCU_NONE = 0, MCU_EXTRMATN = 1, MCU_ARKANOID = 2, MCU_DRTOPPEL = 3, MCU_CHUKATAI = 4, MCU_TNZS = 5;
        static int mcu_initializing, mcu_coinage_init, mcu_command, mcu_readcredits;
        static int mcu_reportcoin;
        static int tnzs_workram_backup;
        static byte[] mcu_coinage = new byte[4];
        static byte mcu_coinsA, mcu_coinsB, mcu_credits;

        static Mame.osd_bitmap[] tnzs_column = new Mame.osd_bitmap[16];
        static int[,] tnzs_dirty_map = new int[32, 16];
        static int tnzs_screenflip, old_tnzs_screenflip;


        static Mame.GfxLayout arkanoi2_charlayout =
        new Mame.GfxLayout(
            16, 16,
            4096,
            4,
            new uint[] { 3 * 4096 * 32 * 8, 2 * 4096 * 32 * 8, 1 * 4096 * 32 * 8, 0 * 4096 * 32 * 8 },
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0,8*8+1,8*8+2,8*8+3,8*8+4,8*8+5,8*8+6,8*8+7},
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8
        );

        static Mame.GfxLayout tnzs_charlayout =
        new Mame.GfxLayout(
            16, 16,
            8192,
            4,
            new uint[] { 3 * 8192 * 32 * 8, 2 * 8192 * 32 * 8, 1 * 8192 * 32 * 8, 0 * 8192 * 32 * 8 },
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8
        );

        static Mame.GfxLayout insectx_charlayout =
        new Mame.GfxLayout(
            16, 16,
            8192,
            4,
            new uint[] { 8, 0, 8192 * 64 * 8 + 8, 8192 * 64 * 8 },
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
			8*16+0, 8*16+1, 8*16+2, 8*16+3, 8*16+4, 8*16+5, 8*16+6, 8*16+7 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
		16*16, 17*16, 18*16, 19*16, 20*16, 21*16, 22*16, 23*16 },
            64 * 8
        );

        public static Mame.GfxDecodeInfo[] arkanoi2_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, arkanoi2_charlayout, 0, 32 ),
};

        public static Mame.GfxDecodeInfo[] tnzs_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, tnzs_charlayout, 0, 32 ),
};

        public static Mame.GfxDecodeInfo[] insectx_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, insectx_charlayout, 0, 32 ),
};



        public static YM2203interface ym2203_interface =
        new YM2203interface(
            1,			/* 1 chip */
            3000000,	/* 3 MHz ??? */
            new int[] { ym2203.YM2203_VOL(30, 30) },
            new AY8910portRead[] { Mame.input_port_0_r },		/* DSW1 connected to port A */
            new AY8910portRead[] { Mame.input_port_1_r },		/* DSW2 connected to port B */
            new AY8910portWrite[] { null },
            new AY8910portWrite[] { null },
            new AY8910handler[] { null }
        );

        public static Mame.MemoryReadAddress[] readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0xbfff, Mame.MRA_BANK1 ), /* ROM + RAM */
	new Mame.MemoryReadAddress( 0xc000, 0xdfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff, tnzs_workram_r ),	/* WORK RAM (shared by the 2 z80's */
	new Mame.MemoryReadAddress( 0xf000, 0xf1ff, Mame.MRA_RAM ),	/* VDC RAM */
	new Mame.MemoryReadAddress( 0xf600, 0xf600, Mame.MRA_NOP ),	/* ? */
	new Mame.MemoryReadAddress( 0xf800, 0xfbff, Mame.MRA_RAM ),	/* not in extrmatn and arkanoi2 (PROMs instead) */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] writemem =
{
	new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress(0x8000, 0xbfff,Mame. MWA_BANK1 ),	/* ROM + RAM */
	new Mame.MemoryWriteAddress(0xc000, 0xdfff, Mame.MWA_RAM, tnzs_objram ),
	new Mame.MemoryWriteAddress(0xe000, 0xefff, tnzs_workram_w, tnzs_workram ),
	new Mame.MemoryWriteAddress(0xf000, 0xf1ff, Mame.MWA_RAM, tnzs_vdcram ),
	new Mame.MemoryWriteAddress(0xf200, 0xf3ff, Mame.MWA_RAM, tnzs_scrollram ), /* scrolling info */
	new Mame.MemoryWriteAddress(0xf400, 0xf400, Mame.MWA_NOP ),	/* ? */
	new Mame.MemoryWriteAddress(0xf600, 0xf600, tnzs_bankswitch_w ),
	new Mame.MemoryWriteAddress(0xf800, 0xfbff, Mame.paletteram_xRRRRRGGGGGBBBBB_w,Mame.paletteram ),	/* not in extrmatn and arkanoi2 (PROMs instead) */
	new Mame.MemoryWriteAddress(-1 ) /* end of table */
};

        public static Mame.MemoryReadAddress[] sub_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x7fff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( 0x8000, 0x9fff, Mame.MRA_BANK2 ),
	new Mame.MemoryReadAddress( 0xb000, 0xb000, ym2203.YM2203_status_port_0_r ),
	new Mame.MemoryReadAddress( 0xb001, 0xb001, ym2203.YM2203_read_port_0_r ),
	new Mame.MemoryReadAddress( 0xc000, 0xc001, tnzs_mcu_r ),	/* plain input ports in insectx (memory handler */
									/* changed in insectx_init() ) */
	new Mame.MemoryReadAddress( 0xd000, 0xdfff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0xe000, 0xefff, tnzs_workram_sub_r ),
	new Mame.MemoryReadAddress( 0xf000, 0xf003, arkanoi2_sh_f000_r ),	/* paddles in arkanoid2/plumppop; the ports are */
						/* read but not used by the other games, and are not read at */
						/* all by insectx. */
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] sub_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x9fff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, tnzs_bankswitch1_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb000, ym2203.YM2203_control_port_0_w ),
	new Mame.MemoryWriteAddress( 0xb001, 0xb001, ym2203.YM2203_write_port_0_w ),
	new Mame.MemoryWriteAddress( 0xc000, 0xc001, tnzs_mcu_w ),	/* not present in insectx */
	new Mame.MemoryWriteAddress( 0xd000, 0xdfff, Mame.MWA_RAM ),
	new Mame.MemoryWriteAddress( 0xe000, 0xefff, tnzs_workram_sub_w ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};


        public static int tnzs_vh_start()
        {
            int column, x, y;
            for (column = 0; column < 16; column++)
            {
                if ((tnzs_column[column] = Mame.osd_create_bitmap(32, 256)) == null)
                {
                    /* Free all the columns */
                    for (column--; column != 0; column--)
                        Mame.osd_free_bitmap(tnzs_column[column]);
                    return 1;
                }
            }

            for (x = 0; x < 32; x++)
            {
                for (y = 0; y < 16; y++)
                {
                    tnzs_dirty_map[x, y] = -1;
                }
            }

            return 0;
        }
        public static void tnzs_vh_stop()
        {
            int column;

            /* Free all the columns */
            for (column = 0; column < 16; column++)
                Mame.osd_free_bitmap(tnzs_column[column]);
        }
        public static void tnzs_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int color, code, i, offs, x, y;
            int[] colmask = new int[32];

            /* Remap dynamic palette */
            Mame.palette_init_used_colors();

            for (color = 0; color < 32; color++) colmask[color] = 0;

            /* See what colours the tiles need */
            for (offs = 32 * 16 - 1; offs >= 0; offs--)
            {
                code = tnzs_objram[offs + 0x400]
                     + 0x100 * (tnzs_objram[offs + 0x1400] & 0x1f);
                color = tnzs_objram[offs + 0x1600] >> 3;

                colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
            }

            /* See what colours the sprites need */
            for (offs = 0x1ff; offs >= 0; offs--)
            {
                code = tnzs_objram[offs]
                     + 0x100 * (tnzs_objram[offs + 0x1000] & 0x1f);
                color = tnzs_objram[offs + 0x1200] >> 3;

                colmask[color] |= (int)Mame.Machine.gfx[0].pen_usage[code];
            }

            /* Construct colour usage table */
            for (color = 0; color < 32; color++)
            {
                if ((colmask[color] & (1 << 0)) != 0)
                    Mame.palette_used_colors[16 * color] = Mame.PALETTE_COLOR_TRANSPARENT;
                for (i = 1; i < 16; i++)
                {
                    if ((colmask[color] & (1 << i)) != 0)
                        Mame.palette_used_colors[16 * color + i] = Mame.PALETTE_COLOR_USED;
                }
            }

            if (Mame.palette_recalc() != null)
            {
                for (x = 0; x < 32; x++)
                {
                    for (y = 0; y < 16; y++)
                    {
                        tnzs_dirty_map[x, y] = -1;
                    }
                }
            }
            arkanoi2_vh_screenrefresh(bitmap, full_refresh);
        }




        static int tnzs_workram_sub_r(int offset)
        {
            return tnzs_workram[offset];
        }
        static void tnzs_workram_sub_w(int offset, int data)
        {
            tnzs_workram[offset] = (byte)data;
        }
        public static int tnzs_interrupt()
        {
            int coin;

            switch (mcu_type)
            {
                case MCU_ARKANOID:
                    coin = ((Mame.readinputport(5) & 0xf000) ^ 0xd000) >> 12;
                    coin = (coin & 0x08) | ((coin & 0x03) << 1) | ((coin & 0x04) >> 2);
                    mcu_handle_coins(coin);
                    break;

                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                    coin = (((Mame.readinputport(4) & 0x30) >> 4) | ((Mame.readinputport(4) & 0x03) << 2)) ^ 0x0c;
                    mcu_handle_coins(coin);
                    break;

                case MCU_CHUKATAI:
                case MCU_TNZS:
                    coin = (((Mame.readinputport(4) & 0x30) >> 4) | ((Mame.readinputport(4) & 0x03) << 2)) ^ 0x0f;
                    mcu_handle_coins(coin);
                    break;

                case MCU_NONE:
                default:
                    break;
            }

            return 0;
        }
        static void tnzs_bankswitch1_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU2);

            //	if (errorlog) fprintf(errorlog, "PC %04x: writing %02x to bankswitch 1\n", cpu_get_pc(),data);

            /* bit 2 resets the mcu */
            if ((data & 0x04) != 0) mcu_reset();

            /* bits 0-1 select ROM bank */
            Mame.cpu_setbank(2, new _BytePtr(RAM, 0x10000 + 0x2000 * (data & 3)));
        }

        public static void tnzs_init_machine()
        {
            /* initialize the mcu simulation */
            mcu_reset();

            /* preset the banks */
            {
                _BytePtr RAM;

                RAM = Mame.memory_region(Mame.REGION_CPU1);
                Mame.cpu_setbank(1, new _BytePtr(RAM, 0x18000));

                RAM = Mame.memory_region(Mame.REGION_CPU2);
                Mame.cpu_setbank(2, new _BytePtr(RAM, 0x10000));
            }
        }


        public static int arkanoi2_sh_f000_r(int offset)
        {
            int val;

            //if (errorlog) fprintf (errorlog, "PC %04x: read input %04x\n", cpu_get_pc(), 0xf000 + offset);

            val = Mame.readinputport(5 + offset / 2);
            if ((offset & 1) != 0)
            {
                return ((val >> 8) & 0xff);
            }
            else
            {
                return val & 0xff;
            }
        }


        static void mcu_reset()
        {
            mcu_initializing = 3;
            mcu_coinage_init = 0;
            mcu_coinage[0] = 1;
            mcu_coinage[1] = 1;
            mcu_coinage[2] = 1;
            mcu_coinage[3] = 1;
            mcu_coinsA = 0;
            mcu_coinsB = 0;
            mcu_credits = 0;
            mcu_reportcoin = 0;
            mcu_command = 0;
            tnzs_workram_backup = -1;
        }
        static int insertcoin;
        static void mcu_handle_coins(int coin)
        {


            /* The coin inputs and coin counter is managed by the i8742 mcu. */
            /* Here we simulate it. */
            /* Chuka Taisen has a limit of 9 credits, so any */
            /* coins that could push it over 9 should be rejected */
            /* Coin/Play settings must also be taken into consideration */

            if ((coin & 0x08) != 0)	/* tilt */
                mcu_reportcoin = coin;
            else if (coin != 0 && coin != insertcoin)
            {
                if ((coin & 0x01) != 0)	/* coin A */
                {
                    if ((mcu_type == MCU_CHUKATAI) && ((mcu_credits + mcu_coinage[1]) > 9))
                    {
                        Mame.coin_lockout_global_w(0, 1); /* Lock all coin slots */
                    }
                    else
                    {
                        Mame.printf("Coin dropped into slot A\n");
                        Mame.coin_lockout_global_w(0, 0); /* Unlock all coin slots */
                        Mame.coin_counter_w(0, 1); Mame.coin_counter_w(0, 0); /* Count slot A */
                        mcu_coinsA++;
                        if (mcu_coinsA >= mcu_coinage[0])
                        {
                            mcu_coinsA -= mcu_coinage[0];
                            mcu_credits += mcu_coinage[1];
                        }
                    }
                }
                if ((coin & 0x02) != 0)	/* coin B */
                {
                    if ((mcu_type == MCU_CHUKATAI) && ((mcu_credits + mcu_coinage[3]) > 9))
                    {
                        Mame.coin_lockout_global_w(0, 1); /* Lock all coin slots */
                    }
                    else
                    {
                        Mame.printf("Coin dropped into slot B\n");
                        Mame.coin_lockout_global_w(0, 0); /* Unlock all coin slots */
                        Mame.coin_counter_w(1, 1); Mame.coin_counter_w(1, 0); /* Count slot B */
                        mcu_coinsB++;
                        if (mcu_coinsB >= mcu_coinage[2])
                        {
                            mcu_coinsB -= mcu_coinage[2];
                            mcu_credits += mcu_coinage[3];
                        }
                    }
                }
                if ((coin & 0x04) != 0)	/* service */
                {
                    Mame.printf("Coin dropped into service slot C\n");
                    mcu_credits++;
                }
                mcu_reportcoin = coin;
            }
            else
            {
                Mame.coin_lockout_global_w(0, 0); /* Unlock all coin slots */
                mcu_reportcoin = 0;
            }
            insertcoin = coin;
        }



        static int mcu_arkanoi2_r(int offset)
        {
            byte[] mcu_startup = { 0x55, 0xaa, 0x5a };

            //	if (errorlog) fprintf (errorlog, "PC %04x: read mcu %04x\n", cpu_get_pc(), 0xc000 + offset);

            if (offset == 0)
            {
                /* if the mcu has just been reset, return startup code */
                if (mcu_initializing != 0)
                {
                    mcu_initializing--;
                    return mcu_startup[2 - mcu_initializing];
                }

                switch (mcu_command)
                {
                    case 0x41:
                        return mcu_credits;

                    case 0xc1:
                        /* Read the credit counter or the inputs */
                        if (mcu_readcredits == 0)
                        {
                            mcu_readcredits = 1;
                            if ((mcu_reportcoin & 0x08) != 0)
                            {
                                mcu_initializing = 3;
                                return 0xee;	/* tilt */
                            }
                            else return mcu_credits;
                        }
                        else return Mame.readinputport(2);	/* buttons */

                    default:
                        Mame.printf("error, unknown mcu command\n");
                        /* should not happen */
                        return 0xff;
                        break;
                }
            }
            else
            {
                /*
                status bits:
                0 = mcu is ready to send data (read from c000)
                1 = mcu has read data (from c000)
                2 = unused
                3 = unused
                4-7 = coin code
                      0 = nothing
                      1,2,3 = coin switch pressed
                      e = tilt
                */
                if ((mcu_reportcoin & 0x08) != 0) return 0xe1;	/* tilt */
                if ((mcu_reportcoin & 0x01) != 0) return 0x11;	/* coin 1 (will trigger "coin inserted" sound) */
                if ((mcu_reportcoin & 0x02) != 0) return 0x21;	/* coin 2 (will trigger "coin inserted" sound) */
                if ((mcu_reportcoin & 0x04) != 0) return 0x31;	/* coin 3 (will trigger "coin inserted" sound) */
                return 0x01;
            }
        }

        static void mcu_arkanoi2_w(int offset, int data)
        {
            if (offset == 0)
            {
                //	if (errorlog) fprintf (errorlog, "PC %04x (re %04x): write %02x to mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), data, 0xc000 + offset);
                if (mcu_command == 0x41)
                {
                    mcu_credits = (byte)((mcu_credits + data) & 0xff);
                }
            }
            else
            {
                /*
                0xc1: read number of credits, then buttons
                0x54+0x41: add value to number of credits
                0x84: coin 1 lockout (issued only in test mode)
                0x88: coin 2 lockout (issued only in test mode)
                0x80: release coin lockout (issued only in test mode)
                during initialization, a sequence of 4 bytes sets coin/credit settings
                */
                //	if (errorlog) fprintf (errorlog, "PC %04x (re %04x): write %02x to mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), data, 0xc000 + offset);

                if (mcu_initializing != 0)
                {
                    /* set up coin/credit settings */
                    mcu_coinage[mcu_coinage_init++] = (byte)data;
                    if (mcu_coinage_init == 4) mcu_coinage_init = 0;	/* must not happen */
                }

                if (data == 0xc1)
                    mcu_readcredits = 0;	/* reset input port number */

                mcu_command = data;
            }
        }


        static int mcu_chukatai_r(int offset)
        {
            byte[] mcu_startup = { 0xa5, 0x5a, 0xaa };

            //if (errorlog) fprintf (errorlog, "PC %04x (re %04x): read mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), 0xc000 + offset);

            if (offset == 0)
            {
                /* if the mcu has just been reset, return startup code */
                if (mcu_initializing != 0)
                {
                    mcu_initializing--;
                    return mcu_startup[2 - mcu_initializing];
                }

                switch (mcu_command)
                {
                    case 0x1f:
                        return (Mame.readinputport(4) >> 4) ^ 0x0f;

                    case 0x03:
                        return Mame.readinputport(4) & 0x0f;

                    case 0x41:
                        return mcu_credits;

                    case 0x93:
                        /* Read the credit counter or the inputs */
                        if (mcu_readcredits == 0)
                        {
                            mcu_readcredits += 1;
                            if ((mcu_reportcoin & 0x08) != 0)
                            {
                                mcu_initializing = 3;
                                return 0xee;	/* tilt */
                            }
                            else return mcu_credits;
                        }
                        /* player 1 joystick and buttons */
                        if (mcu_readcredits == 1)
                        {
                            mcu_readcredits += 1;
                            return Mame.readinputport(2);
                        }
                        /* player 2 joystick and buttons */
                        if (mcu_readcredits == 2)
                        {
                            return Mame.readinputport(3);
                        }
                        break;
                    default:
                        Mame.printf("error, unknown mcu command (%02x)\n", mcu_command);
                        /* should not happen */
                        return 0xff;
                        break;
                }
            }
            else
            {
                /*
                status bits:
                0 = mcu is ready to send data (read from c000)
                1 = mcu has read data (from c000)
                2 = mcu is busy
                3 = unused
                4-7 = coin code
                      0 = nothing
                      1,2,3 = coin switch pressed
                      e = tilt
                */
                if ((mcu_reportcoin & 0x08) != 0) return 0xe1;	/* tilt */
                if ((mcu_reportcoin & 0x01) != 0) return 0x11;	/* coin A */
                if ((mcu_reportcoin & 0x02) != 0) return 0x21;	/* coin B */
                if ((mcu_reportcoin & 0x04) != 0) return 0x31;	/* coin C */
                return 0x01;
            }
            throw new Exception();
        }

        static void mcu_chukatai_w(int offset, int data)
        {
            //if (errorlog) fprintf (errorlog, "PC %04x (re %04x): write %02x to mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), data, 0xc000 + offset);

            if (offset == 0)
            {
                if (mcu_command == 0x41)
                {
                    mcu_credits = (byte)((mcu_credits + data) & 0xff);
                }
            }
            else
            {
                /*
                0x93: read number of credits, then joysticks/buttons
                0x03: read service & tilt switches
                0x1f: read coin switches
                0x4f+0x41: add value to number of credits

                during initialization, a sequence of 4 bytes sets coin/credit settings
                */

                if (mcu_initializing != 0)
                {
                    /* set up coin/credit settings */
                    mcu_coinage[mcu_coinage_init++] = (byte)data;
                    if (mcu_coinage_init == 4) mcu_coinage_init = 0;	/* must not happen */
                }

                if (data == 0x93)
                    mcu_readcredits = 0;	/* reset input port number */

                mcu_command = data;
            }
        }



        static int mcu_tnzs_r(int offset)
        {
            byte[] mcu_startup = { 0x5a, 0xa5, 0x55 };

            //if (errorlog) fprintf (errorlog, "PC %04x (re %04x): read mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), 0xc000 + offset);

            if (offset == 0)
            {
                /* if the mcu has just been reset, return startup code */
                if (mcu_initializing != 0)
                {
                    mcu_initializing--;
                    return mcu_startup[2 - mcu_initializing];
                }

                switch (mcu_command)
                {
                    case 0x01:
                        return Mame.readinputport(2) ^ 0xff;	/* player 1 joystick + buttons */

                    case 0x02:
                        return Mame.readinputport(3) ^ 0xff;	/* player 2 joystick + buttons */

                    case 0x1a:
                        return Mame.readinputport(4) >> 4;

                    case 0x21:
                        return Mame.readinputport(4) & 0x0f;

                    case 0x41:
                        return mcu_credits;

                    case 0xa0:
                        /* Read the credit counter */
                        if ((mcu_reportcoin & 0x08) != 0)
                        {
                            mcu_initializing = 3;
                            return 0xee;	/* tilt */
                        }
                        else return mcu_credits;

                    case 0xa1:
                        /* Read the credit counter or the inputs */
                        if (mcu_readcredits == 0)
                        {
                            mcu_readcredits = 1;
                            if ((mcu_reportcoin & 0x08) != 0)
                            {
                                mcu_initializing = 3;
                                return 0xee;	/* tilt */
                                //						return 0x64;	/* theres a reset input somewhere */
                            }
                            else return mcu_credits;
                        }
                        /* buttons */
                        else return ((Mame.readinputport(2) & 0xf0) | (Mame.readinputport(3) >> 4)) ^ 0xff;

                    default:
                        //if (errorlog) fprintf (errorlog, "error, unknown mcu command\n");
                        /* should not happen */
                        return 0xff;
                        
                }
            }
            else
            {
                /*
                status bits:
                0 = mcu is ready to send data (read from c000)
                1 = mcu has read data (from c000)
                2 = unused
                3 = unused
                4-7 = coin code
                      0 = nothing
                      1,2,3 = coin switch pressed
                      e = tilt
                */
                if ((mcu_reportcoin & 0x08) != 0) return 0xe1;	/* tilt */
                if (mcu_type == MCU_TNZS)
                {
                    if ((mcu_reportcoin & 0x01) != 0) return 0x31;	/* coin 1 (will trigger "coin inserted" sound) */
                    if ((mcu_reportcoin & 0x02) != 0) return 0x21;	/* coin 2 (will trigger "coin inserted" sound) */
                    if ((mcu_reportcoin & 0x04) != 0) return 0x11;	/* coin 3 (will NOT trigger "coin inserted" sound) */
                }
                else
                {
                    if ((mcu_reportcoin & 0x01) != 0) return 0x11;	/* coin 1 (will trigger "coin inserted" sound) */
                    if ((mcu_reportcoin & 0x02) != 0) return 0x21;	/* coin 2 (will trigger "coin inserted" sound) */
                    if ((mcu_reportcoin & 0x04) != 0) return 0x31;	/* coin 3 (will trigger "coin inserted" sound) */
                }
                return 0x01;
            }
        }

        static void mcu_tnzs_w(int offset, int data)
        {
            if (offset == 0)
            {
                //if (errorlog) fprintf (errorlog, "PC %04x (re %04x): write %02x to mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), data, 0xc000 + offset);
                if (mcu_command == 0x41)
                {
                    mcu_credits = (byte)((mcu_credits + data) & 0xff);
                }
            }
            else
            {
                /*
                0xa0: read number of credits
                0xa1: read number of credits, then buttons
                0x01: read player 1 joystick + buttons
                0x02: read player 2 joystick + buttons
                0x1a: read coin switches
                0x21: read service & tilt switches
                0x4a+0x41: add value to number of credits
                0x84: coin 1 lockout (issued only in test mode)
                0x88: coin 2 lockout (issued only in test mode)
                0x80: release coin lockout (issued only in test mode)
                during initialization, a sequence of 4 bytes sets coin/credit settings
                */

                //if (errorlog) fprintf (errorlog, "PC %04x (re %04x): write %02x to mcu %04x\n", cpu_get_pc(), cpu_geturnpc(), data, 0xc000 + offset);

                if (mcu_initializing != 0)
                {
                    /* set up coin/credit settings */
                    mcu_coinage[mcu_coinage_init++] = (byte)data;
                    if (mcu_coinage_init == 4) mcu_coinage_init = 0;	/* must not happen */
                }

                if (data == 0xa1)
                    mcu_readcredits = 0;	/* reset input port number */

                /* Dr Toppel decrements credits differently. So handle it */
                if ((data == 0x09) && (mcu_type == MCU_DRTOPPEL))
                    mcu_credits = (byte)((mcu_credits - 1) & 0xff);		/* Player 1 start */
                if ((data == 0x18) && (mcu_type == MCU_DRTOPPEL))
                    mcu_credits = (byte)((mcu_credits - 2) & 0xff);		/* Player 2 start */

                mcu_command = data;
            }
        }


        static int tnzs_mcu_r(int offset)
        {
            switch (mcu_type)
            {
                case MCU_ARKANOID:
                    return mcu_arkanoi2_r(offset);
                case MCU_CHUKATAI:
                    return mcu_chukatai_r(offset);
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_TNZS:
                default:
                    return mcu_tnzs_r(offset);
            }
        }

        static void tnzs_mcu_w(int offset, int data)
        {
            switch (mcu_type)
            {
                case MCU_ARKANOID:
                    mcu_arkanoi2_w(offset, data);
                    break;
                case MCU_CHUKATAI:
                    mcu_chukatai_w(offset, data);
                    break;
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_TNZS:
                default:
                    mcu_tnzs_w(offset, data);
                    break;
            }
        }

        static int tnzs_workram_r(int offset)
        {
            /* Location $EF10 workaround required to stop TNZS getting */
            /* caught in and endless loop due to shared ram sync probs */

            if ((offset == 0xf10) && (mcu_type == MCU_TNZS))
            {
                int tnzs_cpu0_pc;

                tnzs_cpu0_pc = (int)Mame.cpu_get_pc();
                switch (tnzs_cpu0_pc)
                {
                    case 0xc66:		/* tnzs */
                    case 0xc64:		/* tnzsb */
                    case 0xab8:		/* tnzs2 */
                        tnzs_workram[offset] = (byte)(tnzs_workram_backup & 0xff);
                        return tnzs_workram_backup;
                    default:
                        break;
                }
            }
            return tnzs_workram[offset];
        }
        static void tnzs_workram_w(int offset, int data)
        {
            /* Location $EF10 workaround required to stop TNZS getting */
            /* caught in and endless loop due to shared ram sync probs */

            tnzs_workram_backup = -1;

            if ((offset == 0xf10) && (mcu_type == MCU_TNZS))
            {
                int tnzs_cpu0_pc;

                tnzs_cpu0_pc = (int)Mame.cpu_get_pc();
                switch (tnzs_cpu0_pc)
                {
                    case 0xab5:		/* tnzs2 */
                        if (Mame.cpu_getpreviouspc() == 0xab4)
                            break;  /* unfortunantly tnzsb is true here too, so stop it */
                        goto case 0xc63;
                    case 0xc63:		/* tnzs */
                    case 0xc61:		/* tnzsb */
                        tnzs_workram_backup = data;
                        break;
                    default:
                        break;
                }
            }
            if (tnzs_workram_backup == -1)
                tnzs_workram[offset] = (byte)data;
        }
        static void tnzs_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            /* bit 4 resets the second CPU */
            if ((data & 0x10) != 0)
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
            else
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);

            /* bits 0-2 select RAM/ROM bank */
            //	if (errorlog) fprintf(errorlog, "PC %04x: writing %02x to bankswitch\n", cpu_get_pc(),data);
            Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000 + 0x4000 * (data & 0x07)));
        }
        static void tnzs_vh_draw_background(Mame.osd_bitmap bitmap, _BytePtr m)
        {
            int i, x, y, column, tot;
            int scrollx, scrolly;
            uint upperbits;

            /* The screen is split into 16 columns.
               So first, update the tiles. */
            for (i = 0, column = 0; column < 16; column++)
            {
                for (y = 0; y < 16; y++)
                {
                    for (x = 0; x < 2; x++, i++)
                    {
                        int tile;

                        /* Construct unique identifier for this tile/color */
                        tile = (m[i + 0x1200] << 16) | (m[i + 0x1000] << 8) | m[i];

                        if (tnzs_dirty_map[column * 2 + x, y] != tile)
                        {
                            int code, color, sx, sy;
                            bool flipx, flipy;

                            tnzs_dirty_map[column * 2 + x, y] = tile;

                            code = m[i] + ((m[i + 0x1000] & 0x1f) << 8);
                            color = (m[i + 0x1200] & 0xf8) >> 3; /* colours at d600-d7ff */
                            sx = x * 16;
                            sy = y * 16;
                            flipx = (m[i + 0x1000] & 0x80) != 0;
                            flipy = (m[i + 0x1000] & 0x40) != 0;
                            if (tnzs_screenflip != 0)
                            {
                                sy = 240 - sy;
                                flipx = !flipx;
                                flipy = !flipy;
                            }

                            Mame.drawgfx(tnzs_column[column], Mame.Machine.gfx[0],
                                    (uint)code,
                                    (uint)color,
                                    flipx, flipy,
                                    sx, sy,
                                    null, Mame.TRANSPARENCY_NONE, 0);
                        }
                    }
                }
            }

            /* If the byte at f301 has bit 0 clear, then don't draw the
               background tiles -WRONG- */

            /* The byte at f200 is the y-scroll value for the first column.
               The byte at f204 is the LSB of x-scroll value for the first column.

               The other columns follow at 16-byte intervals.

               The 9th bit of each x-scroll value is combined into 2 bytes
               at f302-f303 */

            /* f301 seems to control how many columns are drawn but it's not clear how. */
            /* Arkanoid 2 also uses f381, which TNZS always leaves at 00. */
            /* Maybe it's a background / foreground thing? In Arkanoid 2, f381 contains */
            /* the value we expect for the background stars (2E vs. 2A), while f301 the */
            /* one we expect at the beginning of a level (2C vs. 2A). */
            x = tnzs_scrollram[0x101] & 0xf;
            if (x == 1) x = 16;
            y = tnzs_scrollram[0x181] & 0xf;
            if (y == 1) y = 16;
            /* let's just pick the larger value... */
            tot = x;
            if (y > tot) tot = y;

            upperbits = (uint)(tnzs_scrollram[0x102] + tnzs_scrollram[0x103] * 256);
            /* again, it's not clear why there are two areas, but Arkanoid 2 uses these */
            /* for the end of game animation */
            upperbits |= (uint)(tnzs_scrollram[0x182] + tnzs_scrollram[0x183] * 256);

            for (column = 0; column < tot; column++)
            {
                scrollx = (int)(tnzs_scrollram[column * 16 + 4] - ((upperbits & 0x01) * 256));
                if (tnzs_screenflip != 0)
                    scrolly = tnzs_scrollram[column * 16] + 1 - 256;
                else
                    scrolly = -tnzs_scrollram[column * 16] + 1;

                Mame.copybitmap(bitmap, tnzs_column[column ^ 8], false, false, scrollx, scrolly,
                           Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0);
                Mame.copybitmap(bitmap, tnzs_column[column ^ 8], false, false, scrollx, scrolly + (16 * 16),
                           Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 0);

                upperbits >>= 1;
            }
        }

        static void tnzs_vh_draw_foreground(Mame.osd_bitmap bitmap,
                                     _BytePtr char_pointer,
                                     _BytePtr x_pointer,
                                     _BytePtr y_pointer,
                                     _BytePtr ctrl_pointer,
                                     _BytePtr color_pointer)
        {
            int i;


            /* Draw all 512 sprites */
            for (i = 0x1ff; i >= 0; i--)
            {
                int code, color, sx, sy;
                bool flipx, flipy;

                code = char_pointer[i] + ((ctrl_pointer[i] & 0x1f) << 8);
                color = (color_pointer[i] & 0xf8) >> 3;
                sx = x_pointer[i] - ((color_pointer[i] & 1) << 8);
                sy = 240 - y_pointer[i];
                flipx = (ctrl_pointer[i] & 0x80) != 0;
                flipy = (ctrl_pointer[i] & 0x40) != 0;
                if (tnzs_screenflip != 0)
                {
                    sy = 240 - sy;
                    flipx = !flipx;
                    flipy = !flipy;
                    /* hack to hide Chuka Taisens grey line, top left corner */
                    if ((sy == 0) && (code == 0)) sy += 240;
                }

                Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                        (uint)code,
                        (uint)color,
                        flipx, flipy,
                        sx, sy + 2,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }
        }

        public static void arkanoi2_vh_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            int x, y;

            /* If the byte at f300 has bit 6 set, flip the screen
               (I'm not 100% sure about this) */
            tnzs_screenflip = (tnzs_scrollram[0x100] & 0x40) >> 6;
            if (old_tnzs_screenflip != tnzs_screenflip)
            {
                for (x = 0; x < 32; x++)
                {
                    for (y = 0; y < 16; y++)
                    {
                        tnzs_dirty_map[x, y] = -1;
                    }
                }
            }
            old_tnzs_screenflip = tnzs_screenflip;


            /* Blank the background */
            Mame.fillbitmap(bitmap, Mame.Machine.pens[0], Mame.Machine.drv.visible_area);

            /* Redraw the background tiles (c400-c5ff) */
            tnzs_vh_draw_background(bitmap, new _BytePtr(tnzs_objram, 0x400));

            /* Draw the sprites on top */
            tnzs_vh_draw_foreground(bitmap,
                                    new _BytePtr(tnzs_objram, 0x0000), /*  chars : c000 */
                                    new _BytePtr(tnzs_objram, 0x0200), /*	  x : c200 */
                                    new _BytePtr(tnzs_vdcram, 0x0000), /*	  y : f000 */
                                    new _BytePtr(tnzs_objram, 0x1000), /*   ctrl : d000 */
                                    new _BytePtr(tnzs_objram, 0x1200)); /* color : d200 */
        }

        public override void driver_init()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);
            mcu_type = MCU_TNZS;

            /* there's code which falls through from the fixed ROM to bank #0, I have to */
            /* copy it there otherwise the CPU bank switching support will not catch it. */
            Buffer.BlockCopy(RAM.buffer, RAM.offset + 0x18000, RAM.buffer, RAM.offset + 0x08000, 0x4000);
            //memcpy(&RAM[0x08000],&RAM[0x18000],0x4000);
        }
    }
    class driver_arkanoi2 : tnzs
    {
        public class machine_driver_arkanoi2 : Mame.MachineDriver
        {
            public machine_driver_arkanoi2()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 8000000, readmem, writemem, null, null, tnzs_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 6000000, sub_readmem, sub_writemem, null, null, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = arkanoi2_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                tnzs_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                int pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int col = (color_prom[i] << 8) + color_prom[i + 512];
                    palette[pi++] = (byte)((col & 0x7c00) >> 7);	/* Red */
                    palette[pi++] = (byte)((col & 0x03e0) >> 2);	/* Green */
                    palette[pi++] = (byte)((col & 0x001f) << 3);	/* Blue */
                }
            }
            public override int vh_start()
            {
                return tnzs_vh_start();
            }
            public override void vh_stop()
            {
                tnzs_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                arkanoi2_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            mcu_type = MCU_ARKANOID;

            /* there's code which falls through from the fixed ROM to bank #2, I have to */
            /* copy it there otherwise the CPU bank switching support will not catch it. */
            Buffer.BlockCopy(RAM.buffer, RAM.offset + 0x18000, RAM.buffer, RAM.offset + 0x08000, 0x4000);
            //memcpy(&RAM[0x08000],&RAM[0x18000],0x4000);
        }
        Mame.RomModule[] rom_arkanoi2()
        {
            ROM_START("arkanoi2");
            ROM_REGION(0x30000, Mame.REGION_CPU1);			/* Region 0 - main cpu */
            ROM_LOAD("a2-05.rom", 0x00000, 0x08000, 0x136edf9d);
            ROM_CONTINUE(0x18000, 0x08000);		/* banked at 8000-bfff */
            /* 20000-2ffff empty */

            ROM_REGION(0x18000, Mame.REGION_CPU2);		/* Region 2 - sound cpu */
            ROM_LOAD("a2-13.rom", 0x00000, 0x08000, 0xe8035ef1);
            ROM_CONTINUE(0x10000, 0x08000);		/* banked at 8000-9fff */

            ROM_REGION(0x80000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a2-m01.bin", 0x00000, 0x20000, 0x2ccc86b4);
            ROM_LOAD("a2-m02.bin", 0x20000, 0x20000, 0x056a985f);
            ROM_LOAD("a2-m03.bin", 0x40000, 0x20000, 0x274a795f);
            ROM_LOAD("a2-m04.bin", 0x60000, 0x20000, 0x9754f703);

            ROM_REGION(0x0400, Mame.REGION_PROMS);
            ROM_LOAD("b08-08.bin", 0x00000, 0x200, 0xa4f7ebd9);	/* hi bytes */
            ROM_LOAD("b08-07.bin", 0x00200, 0x200, 0xea34d9f7);	/* lo bytes */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_arkanoi2()
        {
            INPUT_PORTS_START("arkanoi2");
            PORT_START();		/* DSW1 - IN2 */
            PORT_DIPNAME(0x01, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x01, DEF_STR(Cocktail));
            PORT_DIPNAME(0x02, 0x02, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x02, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Demo_Sounds));
            PORT_DIPSETTING(0x00, DEF_STR(Off));
            PORT_DIPSETTING(0x08, DEF_STR(On));
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Coin_A));
            PORT_DIPSETTING(0x00, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("3C_1C")); ;
            PORT_DIPSETTING(0x20, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_1C"));
            PORT_DIPNAME(0xc0, 0xc0, DEF_STR(Coin_B));
            PORT_DIPSETTING(0xc0, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x80, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x40, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x00, DEF_STR("1C_6C"));

            PORT_START();		/* DSW2 - IN3 */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Normal");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Very Hard");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "50k 150k");
            PORT_DIPSETTING(0x0c, "100k 200k");
            PORT_DIPSETTING(0x04, "50k Only");
            PORT_DIPSETTING(0x08, "100k Only");
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Lives));
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x40, 0x40, DEF_STR(Unknown));
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x00, "Allow Continue");
            PORT_DIPSETTING(0x80, DEF_STR(No)); ;
            PORT_DIPSETTING(0x00, DEF_STR(Yes));

            PORT_START();		/* IN1 - read at c000 (sound cpu) */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();		/* empty */

            PORT_START();		/* empty */

            PORT_START();		/* spinner 1 - read at f000/1 */
            PORT_ANALOG(0x0fff, 0x0000, (uint)inptports.IPT_DIAL, 70, 15, 0, 0);
            PORT_BIT(0x1000, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x2000, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x4000, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x8000, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);	/* arbitrarily assigned, handled by the mcu */

            PORT_START();		/* spinner 2 - read at f002/3 */
            PORT_ANALOG(0x0fff, 0x0000, (uint)inptports.IPT_DIAL | IPF_PLAYER2, 70, 15, 0, 0);
            PORT_BIT(0xf000, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public driver_arkanoi2()
        {
            drv = new machine_driver_arkanoi2();
            year = "1988";
            name = "arkanoi2";
            description = "Arkanoid - Revenge of DOH (World)";
            manufacturer = "Taito Corporation Japan";
            flags = Mame.ROT270;
            input_ports = input_ports_arkanoi2();
            rom = rom_arkanoi2();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_tnzs : tnzs
    {
        class machine_driver_tnzs : Mame.MachineDriver
        {
            public machine_driver_tnzs()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 12000000 / 2, readmem, writemem, null, null, tnzs_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 12000000 / 2, sub_readmem, sub_writemem, null, null, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 200;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = tnzs_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                tnzs_init_machine();
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
                return tnzs_vh_start();
            }
            public override void vh_stop()
            {
                tnzs_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                tnzs_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }

        Mame.RomModule[] rom_tnzs()
        {
            ROM_START("tnzs");
            ROM_REGION(0x30000, Mame.REGION_CPU1);/* 64k + bankswitch areas for the first CPU */
            ROM_LOAD("nzsb5310.bin", 0x00000, 0x08000, 0xa73745c6);
            ROM_CONTINUE(0x18000, 0x18000);		/* banked at 8000-bfff */

            ROM_REGION(0x18000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("nzsb5311.bin", 0x00000, 0x08000, 0x9784d443);
            ROM_CONTINUE(0x10000, 0x08000);		/* banked at 8000-9fff */

            ROM_REGION(0x100000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            /* ROMs taken from another set (the ones from this set were read incorrectly) */
            ROM_LOAD("nzsb5316.bin", 0x00000, 0x20000, 0xc3519c2a);
            ROM_LOAD("nzsb5317.bin", 0x20000, 0x20000, 0x2bf199e8);
            ROM_LOAD("nzsb5318.bin", 0x40000, 0x20000, 0x92f35ed9);
            ROM_LOAD("nzsb5319.bin", 0x60000, 0x20000, 0xedbb9581);
            ROM_LOAD("nzsb5322.bin", 0x80000, 0x20000, 0x59d2aef6);
            ROM_LOAD("nzsb5323.bin", 0xa0000, 0x20000, 0x74acfb9b);
            ROM_LOAD("nzsb5320.bin", 0xc0000, 0x20000, 0x095d0dc0);
            ROM_LOAD("nzsb5321.bin", 0xe0000, 0x20000, 0x9800c54d);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_tnzs()
        {
            INPUT_PORTS_START("tnzs");
            PORT_START();		/* DSW A */
            PORT_DIPNAME(0x01, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x01, DEF_STR(Cocktail));
            PORT_DIPNAME(0x02, 0x02, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x02, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
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

            PORT_START();		/* DSW B */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x00, "50000 150000");
            PORT_DIPSETTING(0x0c, "70000 200000");
            PORT_DIPSETTING(0x04, "100000 250000");
            PORT_DIPSETTING(0x08, "200000 300000");
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Lives));
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x00, "4");
            PORT_DIPSETTING(0x10, "5");
            PORT_DIPNAME(0x40, 0x40, "Allow Continue");
            PORT_DIPSETTING(0x00, DEF_STR(No));
            PORT_DIPSETTING(0x40, DEF_STR(Yes));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();		/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();		/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN); ;
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public override void driver_init()
        {
            base.driver_init();
        }
        public driver_tnzs()
        {
            drv = new machine_driver_tnzs();
            year = "1988";
            name = "tnzs";
            description = "The NewZealand Story(Japan)";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT0;
            input_ports = input_ports_tnzs();
            rom = rom_tnzs();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_drtoppel : tnzs
    {
        public class machine_driver_drtoppel : Mame.MachineDriver
        {
            public machine_driver_drtoppel()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 12000000 / 2, readmem, writemem, null, null, tnzs_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 12000000 / 2, sub_readmem, sub_writemem, null, null, Mame.interrupt, 1));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 100;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);
                gfxdecodeinfo = tnzs_gfxdecodeinfo;
                total_colors = 512;
                color_table_len = 512;
                video_attributes = Mame.VIDEO_TYPE_RASTER;
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_YM2203, ym2203_interface));
            }
            public override void init_machine()
            {
                tnzs_init_machine();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                int pi = 0;
                for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
                {
                    int col = (color_prom[i] << 8) + color_prom[i + 512];
                    palette[pi++] = (byte)((col & 0x7c00) >> 7);	/* Red */
                    palette[pi++] = (byte)((col & 0x03e0) >> 2);	/* Green */
                    palette[pi++] = (byte)((col & 0x001f) << 3);	/* Blue */
                }
            }
            public override int vh_start()
            {
                return tnzs_vh_start();
            }
            public override void vh_stop()
            {
                tnzs_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                arkanoi2_vh_screenrefresh(bitmap, full_refresh);
            }
            public override void vh_eof_callback()
            {
                //nothing
            }
        }
        public override void driver_init()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            mcu_type = MCU_DRTOPPEL;

            /* there's code which falls through from the fixed ROM to bank #0, I have to */
            /* copy it there otherwise the CPU bank switching support will not catch it. */
            Buffer.BlockCopy(RAM.buffer, RAM.offset + 0x18000, RAM.buffer, RAM.offset + 0x08000, 0x4000);
            //	memcpy(&RAM[0x08000],&RAM[0x18000],0x4000);
        }
        public static Mame.InputPortTiny[] input_ports_drtoppel()
        {
            INPUT_PORTS_START("drtoppel");
            PORT_START();		/* DSW A */
            PORT_DIPNAME(0x01, 0x00, DEF_STR(Cabinet));
            PORT_DIPSETTING(0x00, DEF_STR(Upright));
            PORT_DIPSETTING(0x01, DEF_STR(Cocktail));
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

            PORT_START();		/* DSW B */
            PORT_DIPNAME(0x03, 0x03, DEF_STR(Difficulty));
            PORT_DIPSETTING(0x02, "Easy");
            PORT_DIPSETTING(0x03, "Medium");
            PORT_DIPSETTING(0x01, "Hard");
            PORT_DIPSETTING(0x00, "Hardest");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR(Bonus_Life));
            PORT_DIPSETTING(0x0c, "30000");
            PORT_DIPSETTING(0x00, DEF_STR(Unknown));
            PORT_DIPSETTING(0x04, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Unknown));
            PORT_DIPNAME(0x30, 0x30, DEF_STR(Lives));
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x10, "4");
            PORT_DIPSETTING(0x00, "5");
            PORT_DIPNAME(0x40, 0x40, DEF_STR(Unknown));
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();		/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();		/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();		/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public Mame.RomModule[] rom_drtoppel()
        {
            ROM_START("drtoppel");
            ROM_REGION(0x30000, Mame.REGION_CPU1);	/* 64k + bankswitch areas for the first CPU */
            ROM_LOAD("b19-09.bin", 0x00000, 0x08000, 0x3e654f82);
            ROM_CONTINUE(0x18000, 0x08000);		/* banked at 8000-bfff */
            ROM_LOAD("b19-10.bin", 0x20000, 0x10000, 0x7e72fd25);	/* banked at 8000-bfff */

            ROM_REGION(0x18000, Mame.REGION_CPU2);/* 64k for the second CPU */
            ROM_LOAD("b19-11.bin", 0x00000, 0x08000, 0x524dc249);
            ROM_CONTINUE(0x10000, 0x08000);	/* banked at 8000-9fff */

            ROM_REGION(0x100000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("b19-01.bin", 0x00000, 0x20000, 0xa7e8a0c1);
            ROM_LOAD("b19-02.bin", 0x20000, 0x20000, 0x790ae654);
            ROM_LOAD("b19-03.bin", 0x40000, 0x20000, 0x495c4c5a);
            ROM_LOAD("b19-04.bin", 0x60000, 0x20000, 0x647007a0);
            ROM_LOAD("b19-05.bin", 0x80000, 0x20000, 0x49f2b1a5);
            ROM_LOAD("b19-06.bin", 0xa0000, 0x20000, 0x2d39f1d0);
            ROM_LOAD("b19-07.bin", 0xc0000, 0x20000, 0x8bb06f41);
            ROM_LOAD("b19-08.bin", 0xe0000, 0x20000, 0x3584b491);

            ROM_REGION(0x0400, Mame.REGION_PROMS);	/* color proms */
            ROM_LOAD("b19-13.bin", 0x0000, 0x200, 0x6a547980);	/* hi bytes */
            ROM_LOAD("b19-12.bin", 0x0200, 0x200, 0x5754e9d8);	/* lo bytes */
            return ROM_END;
        }
        public driver_drtoppel()
        {
            drv = new machine_driver_drtoppel();
            year = "1987";
            name = "drtoppel";
            description = "Dr. Toppel's Tankentai (Japan)";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT90;
            input_ports = input_ports_drtoppel();
            rom = rom_drtoppel();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_plumppop : tnzs
    {
        public override void driver_init()
        {
            base.driver_init();
        }
        Mame.RomModule[] rom_plumppop()
        {
            ROM_START("plumppop");
            ROM_REGION(0x30000, Mame.REGION_CPU1);	/* 64k + bankswitch areas for the first CPU */
            ROM_LOAD("a98-09.bin", 0x00000, 0x08000, 0x107f9e06);
            ROM_CONTINUE(0x18000, 0x08000);			/* banked at 8000-bfff */
            ROM_LOAD("a98-10.bin", 0x20000, 0x10000, 0xdf6e6af2);	/* banked at 8000-bfff */

            ROM_REGION(0x18000, Mame.REGION_CPU2);	/* 64k for the second CPU */
            ROM_LOAD("a98-11.bin", 0x00000, 0x08000, 0xbc56775c);
            ROM_CONTINUE(0x10000, 0x08000);	/* banked at 8000-9fff */

            ROM_REGION(0x100000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("a98-01.bin", 0x00000, 0x10000, 0xf3033dca);
            ROM_RELOAD(0x10000, 0x10000);
            ROM_LOAD("a98-02.bin", 0x20000, 0x10000, 0xf2d17b0c);
            ROM_RELOAD(0x30000, 0x10000);
            ROM_LOAD("a98-03.bin", 0x40000, 0x10000, 0x1a519b0a);
            ROM_RELOAD(0x40000, 0x10000);
            ROM_LOAD("a98-04.bin", 0x60000, 0x10000, 0xb64501a1);
            ROM_RELOAD(0x70000, 0x10000);
            ROM_LOAD("a98-05.bin", 0x80000, 0x10000, 0x45c36963);
            ROM_RELOAD(0x90000, 0x10000);
            ROM_LOAD("a98-06.bin", 0xa0000, 0x10000, 0xe075341b);
            ROM_RELOAD(0xb0000, 0x10000);
            ROM_LOAD("a98-07.bin", 0xc0000, 0x10000, 0x8e16cd81);
            ROM_RELOAD(0xd0000, 0x10000);
            ROM_LOAD("a98-08.bin", 0xe0000, 0x10000, 0xbfa7609a);
            ROM_RELOAD(0xf0000, 0x10000);

            ROM_REGION(0x0400, Mame.REGION_PROMS);	/* color proms */
            ROM_LOAD("a98-13.bpr", 0x0000, 0x200, 0x7cde2da5);	/* hi bytes */
            ROM_LOAD("a98-12.bpr", 0x0200, 0x200, 0x90dc9da7);	/* lo bytes */
            return ROM_END;
        }
        public driver_plumppop()
        {
            drv = new driver_drtoppel.machine_driver_drtoppel();
            year = "1987";
            name = "plumppop";
            description = "Plump Pop (Japan)";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT0;
            input_ports = driver_drtoppel.input_ports_drtoppel();
            rom = rom_plumppop();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_extrmatn : tnzs
    {
        public override void driver_init()
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            mcu_type = MCU_EXTRMATN;

            /* there's code which falls through from the fixed ROM to bank #7, I have to */
            /* copy it there otherwise the CPU bank switching support will not catch it. */
            //memcpy(&RAM[0x08000],&RAM[0x2c000],0x4000);
            Buffer.BlockCopy(RAM.buffer, RAM.offset + 0x2c000, RAM.buffer, RAM.offset + 0x08000, 0x4000);
        }
        public Mame.InputPortTiny[] input_ports_extrmatn()
        {
            INPUT_PORTS_START("extrmatn");
            PORT_START();	/* DSW A */
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Unknown));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x02, DEF_STR(Flip_Screen));
            PORT_DIPSETTING(0x02, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_SERVICE(0x04, IP_ACTIVE_LOW);
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
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

            PORT_START();		/* DSW B */
            PORT_DIPNAME(0x01, 0x01, DEF_STR(Unknown));
            PORT_DIPSETTING(0x01, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x02, 0x02, DEF_STR(Unknown));
            PORT_DIPSETTING(0x02, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x04, 0x04, DEF_STR(Unknown));
            PORT_DIPSETTING(0x04, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x08, 0x08, DEF_STR(Unknown));
            PORT_DIPSETTING(0x08, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x10, 0x10, DEF_STR(Unknown));
            PORT_DIPSETTING(0x10, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x20, 0x20, DEF_STR(Unknown));
            PORT_DIPSETTING(0x20, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x40, 0x40, DEF_STR(Unknown));
            PORT_DIPSETTING(0x40, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));
            PORT_DIPNAME(0x80, 0x80, DEF_STR(Unknown));
            PORT_DIPSETTING(0x80, DEF_STR(Off));
            PORT_DIPSETTING(0x00, DEF_STR(On));

            PORT_START();		/* IN0 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START();	/* IN1 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_PLAYER2);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_PLAYER2);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_UP | IPF_PLAYER2);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_PLAYER2);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1 | IPF_PLAYER2);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2 | IPF_PLAYER2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);

            PORT_START();	/* IN2 */
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_TILT);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }
        public Mame.RomModule[] rom_extrmatn()
        {
            ROM_START("extrmatn");
            ROM_REGION(0x30000, Mame.REGION_CPU1);		/* Region 0 - main cpu */
            ROM_LOAD("b06-20.bin", 0x00000, 0x08000, 0x04e3fc1f);
            ROM_CONTINUE(0x18000, 0x08000);	/* banked at 8000-bfff */
            ROM_LOAD("b06-21.bin", 0x20000, 0x10000, 0x1614d6a2);	/* banked at 8000-bfff */

            ROM_REGION(0x18000, Mame.REGION_CPU2);		/* Region 2 - sound cpu */
            ROM_LOAD("b06-06.bin", 0x00000, 0x08000, 0x744f2c84);
            ROM_CONTINUE(0x10000, 0x08000);	/* banked at 8000-9fff */

            ROM_REGION(0x80000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("b06-01.bin", 0x00000, 0x20000, 0xd2afbf7e);
            ROM_LOAD("b06-02.bin", 0x20000, 0x20000, 0xe0c2757a);
            ROM_LOAD("b06-03.bin", 0x40000, 0x20000, 0xee80ab9d);
            ROM_LOAD("b06-04.bin", 0x60000, 0x20000, 0x3697ace4);

            ROM_REGION(0x0400, Mame.REGION_PROMS);
            ROM_LOAD("b06-09.bin", 0x00000, 0x200, 0xf388b361);	/* hi bytes */
            ROM_LOAD("b06-08.bin", 0x00200, 0x200, 0x10c9aac3);	/* lo bytes */
            return ROM_END;
        }
        public driver_extrmatn()
        {
            drv = new driver_arkanoi2.machine_driver_arkanoi2();
            year = "1987";
            name = "extrmatn";
            description = "Extermination (US)";
            manufacturer = "[Taito] World Games";
            flags = Mame.ROT270;
            input_ports = input_ports_extrmatn();
            rom = rom_extrmatn();
            drv.HasNVRAMhandler = false;
        }
    }
}
