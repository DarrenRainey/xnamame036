#define IR_TIMING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_irobot : Mame.GameDriver
    {
        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];

        static Mame.osd_bitmap polybitmap1, polybitmap2, polybitmap;


        static Mame.MemoryReadAddress[] readmem =
{
    new Mame.MemoryReadAddress( 0x0000, 0x07ff, Mame.MRA_RAM ),
    new Mame.MemoryReadAddress( 0x0800, 0x0fff, Mame.MRA_BANK2 ),
    new Mame.MemoryReadAddress( 0x1000, 0x103f, Mame.input_port_0_r ),
    new Mame.MemoryReadAddress( 0x1040, 0x1040, Mame.input_port_1_r ),
    new Mame.MemoryReadAddress( 0x1080, 0x1080, irobot_status_r ),
    new Mame.MemoryReadAddress( 0x10c0, 0x10c0, Mame.input_port_3_r ),
    new Mame.MemoryReadAddress( 0x1200, 0x12ff, Mame.MRA_RAM ),
    new Mame.MemoryReadAddress( 0x1300, 0x13ff, irobot_control_r ),
    new Mame.MemoryReadAddress( 0x1400, 0x143f, Pokey.quad_pokey_r ),
    new Mame.MemoryReadAddress( 0x1c00, 0x1fff, Mame.MRA_RAM ),
    new Mame.MemoryReadAddress( 0x2000, 0x3fff, irobot_sharedmem_r ),
    new Mame.MemoryReadAddress( 0x4000, 0x5fff, Mame.MRA_BANK1 ),
    new Mame.MemoryReadAddress( 0x6000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )	/* end of table */
};

        static Mame.MemoryWriteAddress[] writemem =
{
    new Mame.MemoryWriteAddress( 0x0000, 0x07ff, Mame.MWA_RAM ),
    new Mame.MemoryWriteAddress( 0x0800, 0x0fff, Mame.MWA_BANK2 ),
    new Mame.MemoryWriteAddress( 0x1100, 0x1100, irobot_clearirq ),
    new Mame.MemoryWriteAddress( 0x1140, 0x1140, irobot_statwr_w ),
    new Mame.MemoryWriteAddress( 0x1180, 0x1180, irobot_out0_w ),
    new Mame.MemoryWriteAddress( 0x11c0, 0x11c0, irobot_rom_banksel ),
    new Mame.MemoryWriteAddress( 0x1200, 0x12ff, irobot_nvram_w, nvram, nvram_size ),
    new Mame.MemoryWriteAddress( 0x1400, 0x143f, Pokey.quad_pokey_w ),
    new Mame.MemoryWriteAddress( 0x1800, 0x18ff, irobot_paletteram_w ),
    new Mame.MemoryWriteAddress( 0x1900, 0x19ff, Mame.MWA_RAM ),            /* Watchdog reset */
    new Mame.MemoryWriteAddress( 0x1a00, 0x1a00, irobot_clearfirq ),
    new Mame.MemoryWriteAddress( 0x1b00, 0x1bff, irobot_control_w ),
    new Mame.MemoryWriteAddress( 0x1c00, 0x1fff, Mame.MWA_RAM, Generic.videoram, Generic.videoram_size ),
    new Mame.MemoryWriteAddress( 0x2000, 0x3fff, irobot_sharedmem_w),
    new Mame.MemoryWriteAddress( 0x4000, 0xffff, Mame.MWA_ROM ),
    new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static bool irvg_clear;
        static bool irvg_vblank, irvg_running, irmb_running;
        static object irscanline_timer;
        static object irvg_timer, irbm_timer;

        static _BytePtr[] comRAM = new _BytePtr[2];
        static _BytePtr mbRAM, mbROM;
        static byte irobot_control_num = 0;
        static byte irobot_statwr, irobot_out0, irobot_outx, irobot_mpage;

        static _BytePtr irobot_combase_mb;
        static _BytePtr irobot_combase;


        static 
void irobot_paletteram_w(int offset,int data)
{
    int r,g,b;
	int bits,intensity;
    int color;

    color = ((data << 1) | (offset & 0x01)) ^ 0x1ff;
    intensity = color & 0x07;
    bits = (color >> 3) & 0x03;
    b = 8 * bits * intensity;
    bits = (color >> 5) & 0x03;
    g = 8 * bits * intensity;
    bits = (color >> 7) & 0x03;
    r = 8 * bits * intensity;
    Mame.palette_change_color((offset >> 1) & 0x3F, (byte)r, (byte)g, (byte)b);
}

        static int BYTE_XOR_LE(int x)
        {
#if WINDOWS
            return (x ^ 1);
#else
            return x;
#endif
        }


        static byte irobot_bufsel, irobot_alphamap;


        static void irobot_rom_banksel(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            switch ((data & 0x0E) >> 1)
            {
                case 0: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x10000)); break;
                case 1: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x12000)); break;
                case 2: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x14000)); break;
                case 3: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x16000)); break;
                case 4: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x18000)); break;
                case 5: Mame.cpu_setbank(1, new _BytePtr(RAM, 0x1A000)); break;
            }
            Mame.osd_led_w(0, data & 0x10);
            Mame.osd_led_w(1, data & 0x20);
        }


        static void irobot_nvram_w(int offset, int data)
        {
            nvram[offset] = (byte)(data & 0x0f);
        }
        static int irobot_status_r(int offset)
        {
            int d = 0;

            //if (errorlog)
            //{
            //    fprintf(errorlog, "status read. ");
            //    IR_CPU_STATE;
            //}

            if (!irmb_running) d |= 0x20;
            if (irvg_running) d |= 0x40;

            //        d = (irmb_running * 0x20) | (irvg_running * 0x40);
            if (irvg_vblank) d = d | 0x80;
#if IR_TIMING
            /* flags are cleared by callbacks */
#else
    irmb_running = false;
    irvg_running = false;
#endif
            return d;
        }
        static int irobot_control_r(int offset)
        {

            if (irobot_control_num == 0)
                return Mame.readinputport(5);
            else if (irobot_control_num == 1)
                return Mame.readinputport(6);
            return 0;

        }
        static int irobot_sharedmem_r(int offset)
        {
            if (irobot_outx == 3)
                return mbRAM[BYTE_XOR_LE(offset)];

            if (irobot_outx == 2)
                return irobot_combase[BYTE_XOR_LE(offset & 0xFFF)];

            if (irobot_outx == 0)
                return mbROM[((irobot_mpage & 1) << 13) + BYTE_XOR_LE(offset)];

            if (irobot_outx == 1)
                return mbROM[0x4000 + ((irobot_mpage & 3) << 13) + BYTE_XOR_LE(offset)];

            return 0xFF;
        }
        static void irobot_clearirq(int offest, int data)
        {
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, Mame.CLEAR_LINE);
        }
        static void irobot_clearfirq(int offset, int data)
        {
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_FIRQ_LINE, Mame.CLEAR_LINE);
        }
        static void IR_CPU_STATE() { }
        static void irvg_done_callback(int param)
        {
            //if (errorlog)fprintf(errorlog, "vg done. ");
            IR_CPU_STATE();
            irvg_running = false;
        }
        static void irobot_poly_clear()
        {

            if (irobot_bufsel != 0)
                Mame.osd_clearbitmap(polybitmap2);
            else
                Mame.osd_clearbitmap(polybitmap1);
        }
        static void irobot_statwr_w(int offset, int data)
        {
            //if (errorlog)
            //{
            //    fprintf (errorlog, "write %2x ", data);
            //    IR_CPU_STATE;
            //}

            irobot_combase = comRAM[data >> 7];
            irobot_combase_mb = comRAM[(data >> 7) ^ 1];
            irobot_bufsel = (byte)(data & 0x02);
            if (((data & 0x01) == 0x01) && (!irvg_clear))
                irobot_poly_clear();

            irvg_clear = (data & 0x01) != 0;

            if ((data & 0x04) != 0 && (irobot_statwr & 0x04) == 0)
            {
                run_video();
#if IR_TIMING
                if (!irvg_running)
                {
                    //if (errorlog) fprintf(errorlog,"vg start ");
                    IR_CPU_STATE();
                    irvg_timer = Mame.Timer.timer_set(Mame.Timer.TIME_IN_MSEC(10), 0, irvg_done_callback);
                }
                else
                {
                    //if (errorlog) fprintf (errorlog, "vg start [busy!] ");
                    IR_CPU_STATE();
                    Mame.Timer.timer_reset(irvg_timer, Mame.Timer.TIME_IN_MSEC(10));
                }
#endif
                irvg_running = true;
            }
            if ((data & 0x10) != 0 && (irobot_statwr & 0x10) == 0)
                irmb_run();
            irobot_statwr = (byte)data;
        }

        static void irobot_control_w(int offset, int data)
        {
            irobot_control_num = (byte)(offset & 0x03);
        }
        class irmb_ops
        {
            public irmb_ops nxtop;
            public uint func;
            public uint diradd;
            public uint latchmask;
            public uint[] areg;
            public uint[] breg;
            public byte cycles;
            public byte diren;
            public byte flags;
            public byte ramsel;
        }
        static irmb_ops[] mbops;

        const byte FL_MULT = 0x01;
        const byte FL_shift = 0x02;
        const byte FL_MBMEMDEC = 0x04;
        const byte FL_ADDEN = 0x08;
        const byte FL_DPSEL = 0x10;
        const byte FL_carry = 0x20;
        const byte FL_DIV = 0x40;
        const byte FL_MBRW = 0x80;

        static uint irmb_latch;
        static uint[] irmb_regs = new uint[16];




        static void irobot_out0_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

            irobot_out0 = (byte)data;
            switch (data & 0x60)
            {
                case 0:
                    Mame.cpu_setbank(2, new _BytePtr(RAM, 0x1C000));
                    break;
                case 0x20:
                    Mame.cpu_setbank(2, new _BytePtr(RAM, 0x1C800));
                    break;
                case 0x40:
                    Mame.cpu_setbank(2, new _BytePtr(RAM, 0x1D000));
                    break;
            }
            irobot_outx = (byte)((data & 0x18) >> 3);
            irobot_mpage = (byte)((data & 0x06) >> 1);
            irobot_alphamap = (byte)(data & 0x80);
        }
        static void irobot_sharedmem_w(int offset, int data)
        {
            if (irobot_outx == 3)
                mbRAM[BYTE_XOR_LE(offset)] = (byte)data;

            if (irobot_outx == 2)
                irobot_combase[BYTE_XOR_LE(offset & 0xFFF)] = (byte)data;
        }
        static void run_video()
        {
            throw new Exception();
        }
        static void irmb_run()
        {
            throw new Exception();
        }
        class machine_driver_irobot : Mame.MachineDriver
        {
            public machine_driver_irobot()
            {
            }
            public override void init_machine()
            {
                throw new NotImplementedException();
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                throw new NotImplementedException();
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                throw new NotImplementedException();
            }
            public override int vh_start()
            {
                throw new NotImplementedException();
            }
            public override void vh_stop()
            {
                throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new NotImplementedException();
            }
            public override void vh_eof_callback()
            {
                throw new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            throw new NotImplementedException();
        }
        Mame.RomModule[] rom_irobot()
        {
            ROM_START("irobot");
            ROM_REGION(0x20000, Mame.REGION_CPU1); /* 64k for code + 48K Banked ROM*/
            ROM_LOAD("136029.208", 0x06000, 0x2000, 0xb4d0be59);
            ROM_LOAD("136029.209", 0x08000, 0x4000, 0xf6be3cd0);
            ROM_LOAD("136029.210", 0x0c000, 0x4000, 0xc0eb2133);
            ROM_LOAD("136029.405", 0x10000, 0x4000, 0x9163efe4);
            ROM_LOAD("136029.206", 0x14000, 0x4000, 0xe114a526);
            ROM_LOAD("136029.207", 0x18000, 0x4000, 0xb4556cb0);

            ROM_REGION(0x14000, Mame.REGION_CPU2);  /* mathbox region */
            ROM_LOAD_ODD("ir103.bin", 0x0000, 0x2000, 0x0c83296d);	/* ROM data from 0000-bfff */
            ROM_LOAD_EVEN("ir104.bin", 0x0000, 0x2000, 0x0a6cdcca);
            ROM_LOAD_ODD("ir101.bin", 0x4000, 0x4000, 0x62a38c08);
            ROM_LOAD_EVEN("ir102.bin", 0x4000, 0x4000, 0x9d588f22);
            ROM_LOAD("ir111.bin", 0xc000, 0x0400, 0x9fbc9bf3);	/* program ROMs from c000-f3ff */
            ROM_LOAD("ir112.bin", 0xc400, 0x0400, 0xb2713214);
            ROM_LOAD("ir113.bin", 0xc800, 0x0400, 0x7875930a);
            ROM_LOAD("ir114.bin", 0xcc00, 0x0400, 0x51d29666);
            ROM_LOAD("ir115.bin", 0xd000, 0x0400, 0x00f9b304);
            ROM_LOAD("ir116.bin", 0xd400, 0x0400, 0x326aba54);
            ROM_LOAD("ir117.bin", 0xd800, 0x0400, 0x98efe8d0);
            ROM_LOAD("ir118.bin", 0xdc00, 0x0400, 0x4a6aa7f9);
            ROM_LOAD("ir119.bin", 0xe000, 0x0400, 0xa5a13ad8);
            ROM_LOAD("ir120.bin", 0xe400, 0x0400, 0x2a083465);
            ROM_LOAD("ir121.bin", 0xe800, 0x0400, 0xadebcb99);
            ROM_LOAD("ir122.bin", 0xec00, 0x0400, 0xda7b6f79);
            ROM_LOAD("ir123.bin", 0xf000, 0x0400, 0x39fff18f);
            /* RAM data from 10000-11fff */
            /* COMRAM from   12000-13fff */

            ROM_REGION(0x800, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("136029.124", 0x0000, 0x0800, 0x848948b6);

            ROM_REGION(0x0020, Mame.REGION_PROMS);
            ROM_LOAD("ir125.bin", 0x0000, 0x0020, 0x446335ba);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_irobot()
        {

            INPUT_PORTS_START("irobot");
            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_SERVICE(0x10, IP_ACTIVE_LOW);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_COIN2);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_LOW, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_LOW, (uint)inptports.IPT_START2);
            PORT_BIT(0x80, IP_ACTIVE_LOW, (uint)inptports.IPT_START1);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x02, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x04, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x08, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x10, IP_ACTIVE_LOW, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN); /* MB DONE */
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);/* EXT DONE */
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_VBLANK);

            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x00, "Coins Per Credit");
            PORT_DIPSETTING(0x00, "1 Coin 1 Credit");
            PORT_DIPSETTING(0x01, "2 Coins 1 Credit");
            PORT_DIPSETTING(0x02, "3 Coins 1 Credit");
            PORT_DIPSETTING(0x03, "4 Coins 1 Credit");
            PORT_DIPNAME(0x0c, 0x00, "Right Coin");
            PORT_DIPSETTING(0x00, "1 Coin for 1 Coin Unit");
            PORT_DIPSETTING(0x04, "1 Coin for 4 Coin Units");
            PORT_DIPSETTING(0x08, "1 Coin for 5 Coin Units");
            PORT_DIPSETTING(0x0c, "1 Coin for 6 Coin Units");
            PORT_DIPNAME(0x10, 0x00, "Left Coin");
            PORT_DIPSETTING(0x00, "1 Coin for 1 Coin Unit");
            PORT_DIPSETTING(0x10, "1 Coin for 2 Coin Units");
            PORT_DIPNAME(0xe0, 0x00, "Bonus Adder");
            PORT_DIPSETTING(0x00, "None");
            PORT_DIPSETTING(0x20, "1 Credit for 2 Coin Units");
            PORT_DIPSETTING(0xa0, "1 Credit for 3 Coin Units");
            PORT_DIPSETTING(0x40, "1 Credit for 4 Coin Units");
            PORT_DIPSETTING(0x80, "1 Credit for 5 Coin Units");
            PORT_DIPSETTING(0x60, "2 Credits for 4 Coin Units");
            PORT_DIPSETTING(0xe0, DEF_STR("Free Play"));

            PORT_START("DSW2");
            PORT_DIPNAME(0x01, 0x01, "Language");
            PORT_DIPSETTING(0x01, "English");
            PORT_DIPSETTING(0x00, "German");
            PORT_DIPNAME(0x02, 0x02, "Min Game Time");
            PORT_DIPSETTING(0x00, "90 Sec");
            PORT_DIPSETTING(0x02, "3 Lives");
            PORT_DIPNAME(0x0c, 0x0c, DEF_STR("Bonus Life"));
            PORT_DIPSETTING(0x08, "None");
            PORT_DIPSETTING(0x0c, "20000");
            PORT_DIPSETTING(0x00, "30000");
            PORT_DIPSETTING(0x04, "50000");
            PORT_DIPNAME(0x30, 0x30, DEF_STR("Lives"));
            PORT_DIPSETTING(0x20, "2");
            PORT_DIPSETTING(0x30, "3");
            PORT_DIPSETTING(0x00, "4");
            PORT_DIPSETTING(0x10, "5");
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Difficulty"));
            PORT_DIPSETTING(0x00, "Easy");
            PORT_DIPSETTING(0x40, "Medium");
            PORT_DIPNAME(0x80, 0x80, "Demo Mode");
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("IN4");
            PORT_ANALOG(0xff, 0x80, (uint)inptports.IPT_AD_STICK_Y | IPF_CENTER, 70, 50, 95, 159);

            PORT_START("IN5");
            PORT_ANALOG(0xff, 0x80, (uint)inptports.IPT_AD_STICK_X | IPF_REVERSE | IPF_CENTER, 50, 50, 95, 159);

            return INPUT_PORTS_END;
        }
        public driver_irobot()
        {

            drv = new machine_driver_irobot();
            year = "1983";
            name = "irobot";
            description = "I, Robot";
            manufacturer = "Atari";
            flags = Mame.ROT0 | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_irobot();
            rom = rom_irobot();
            drv.HasNVRAMhandler = true;
        }
    }
}
