#define IR_TIMING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_irobot : Mame.GameDriver
    {
        static _BytePtr nvram = new _BytePtr(1);
        static int[] nvram_size = new int[1];

        static Mame.osd_bitmap polybitmap1, polybitmap2, polybitmapt;


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

        static Mame.GfxLayout charlayout = new Mame.GfxLayout(
            8, 8,    /* 8*8 characters */
            64,    /* 64 characters */
            1,      /* 1 bit per pixel */
            new uint[] { 0 }, /* the bitplanes are packed in one nibble */
            new uint[] { 4, 5, 6, 7, 12, 13, 14, 15 },
            new uint[] { 0 * 16, 1 * 16, 2 * 16, 3 * 16, 4 * 16, 5 * 16, 6 * 16, 7 * 16 },
            16 * 8   /* every char takes 16 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] gfxdecodeinfo = { new Mame.GfxDecodeInfo(Mame.REGION_GFX1, 0, charlayout, 64, 16) };

        static POKEYinterface pokey_interface =
        new POKEYinterface(
            4,	/* 4 chips */
            1250000,	/* 1.25 MHz??? */
            new int[] { 25, 25, 25, 25 },
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
            new pot_delegate[] { Mame.input_port_4_r, null, null, null },
             new pot_delegate[] { null, null, null, null },
             new pokey_serout[] { null, null, null, null },
             new Mame.irqcallback[] { null, null, null, null }

        );

        static bool irvg_clear;
        static bool irvg_vblank, irvg_running, irmb_running;
        static object irscanline_timer;
        static object irvg_timer;

        static _BytePtr[] comRAM = new _BytePtr[2];
        static _BytePtr mbRAM, mbROM;
        static byte irobot_control_num = 0;
        static byte irobot_statwr, irobot_out0, irobot_outx, irobot_mpage;

        static _BytePtr irobot_combase_mb;
        static _BytePtr irobot_combase;
        static byte irobot_bufsel, irobot_alphamap;

        static void irobot_paletteram_w(int offset, int data)
        {
            int color = ((data << 1) | (offset & 0x01)) ^ 0x1ff;
            int intensity = color & 0x07;
            int bits = (color >> 3) & 0x03;
            int b = 8 * bits * intensity;
            bits = (color >> 5) & 0x03;
            int g = 8 * bits * intensity;
            bits = (color >> 7) & 0x03;
            int r = 8 * bits * intensity;
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
        
        static void irvg_done_callback(int param)
        {
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
            irobot_combase = comRAM[data >> 7];
            irobot_combase_mb = comRAM[(data >> 7) ^ 1];
            irobot_bufsel = (byte)(data & 0x02);
            if (((data & 0x01) == 0x01) && (!irvg_clear))
                irobot_poly_clear();

            irvg_clear = (data & 0x01) != 0;

            if ((data & 0x04) != 0 && (irobot_statwr & 0x04) == 0)
            {
                run_video();
                if (!irvg_running)
                {
                    irvg_timer = Mame.Timer.timer_set(Mame.Timer.TIME_IN_MSEC(10), 0, irvg_done_callback);
                }
                else
                {
                    Mame.Timer.timer_reset(irvg_timer, Mame.Timer.TIME_IN_MSEC(10));
                }
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
            public int nxtop;
            public uint func;
            public uint diradd;
            public uint latchmask;
            public UIntSubArray areg;
            public UIntSubArray breg;
            public byte cycles;
            public byte diren;
            public byte flags;
            public byte ramsel;
        }
        static irmb_ops[] mbops = new irmb_ops[1024];
        static int[] irmb_stack = new int[16];

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


        static int ir_xmin, ir_ymin, ir_xmax, ir_ymax; /* clipping area */


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
        static int ROUND_TO_PIXEL(int x) { return (((x) >> 7) - 128); }
        static void irobot_draw_pixel(int x, int y, int col)
        {
            if (x < ir_xmin || x >= ir_xmax)
                return;
            if (y < ir_ymin || y >= ir_ymax)
                return;

            Mame.plot_pixel(polybitmapt, x, y, col);
        }
        static void irobot_draw_line(int x1, int y1, int x2, int y2, int col)
        {
            int dx, dy, sx, sy, cx, cy;

            dx = Math.Abs(x1 - x2);
            dy = Math.Abs(y1 - y2);
            sx = (x1 <= x2) ? 1 : -1;
            sy = (y1 <= y2) ? 1 : -1;
            cx = dx / 2;
            cy = dy / 2;

            if (dx >= dy)
            {
                for (; ; )
                {
                    irobot_draw_pixel(x1, y1, col);
                    if (x1 == x2) break;
                    x1 += sx;
                    cx -= dy;
                    if (cx < 0)
                    {
                        y1 += sy;
                        cx += dx;
                    }
                }
            }
            else
            {
                for (; ; )
                {
                    irobot_draw_pixel(x1, y1, col);
                    if (y1 == y2) break;
                    y1 += sy;
                    cy -= dx;
                    if (cy < 0)
                    {
                        x1 += sx;
                        cy += dy;
                    }
                }
            }
        }
        static void run_video()
        {
            int sx, sy, ex, ey, sx2, ey2;
            int color;
            uint d1;
            int lpnt, spnt, spnt2;
            int shp;
            int word1, word2;

            if (irobot_bufsel != 0)
                polybitmapt = polybitmap2;
            else
                polybitmapt = polybitmap1;

            //    if (irvg_clear) irobot_poly_clear();
            lpnt = 0;
            while (lpnt < 0xFFF)
            {
                d1 = irobot_combase.READ_WORD(lpnt);
                lpnt += 2;
                if (d1 == 0xFFFF) break;
                spnt = (int)(d1 & 0x07FF) << 1;
                shp = (int)(d1 & 0xF000) >> 12;

                /* Pixel */
                if (shp == 0x8)
                {
                    while (spnt < 0xFFE)
                    {
                        sx = irobot_combase.READ_WORD(spnt);
                        if (sx == 0xFFFF) break;
                        sy = irobot_combase.READ_WORD(spnt + 2);
                        color = Mame.Machine.pens[sy & 0x3F];
                        irobot_draw_pixel(ROUND_TO_PIXEL(sx), ROUND_TO_PIXEL(sy), color);
                        spnt += 4;
                    }//while object
                }//if point

                /* Line */
                if (shp == 0xC)
                {
                    while (spnt < 0xFFF)
                    {
                        ey = irobot_combase.READ_WORD(spnt);
                        if (ey == 0xFFFF) break;
                        ey = ROUND_TO_PIXEL(ey);
                        sy = irobot_combase.READ_WORD(spnt + 2);
                        color = Mame.Machine.pens[sy & 0x3F];
                        sy = ROUND_TO_PIXEL(sy);
                        sx = irobot_combase.READ_WORD(spnt + 6);
                        word1 = (short)irobot_combase.READ_WORD(spnt + 4);
                        ex = sx + word1 * (ey - sy + 1);
                        irobot_draw_line(ROUND_TO_PIXEL(sx), sy, ROUND_TO_PIXEL(ex), ey, color);
                        spnt += 8;
                    }//while object
                }//if line

                /* Polygon */
                if (shp == 0x4)
                {
                    spnt2 = irobot_combase.READ_WORD(spnt);
                    spnt2 = (spnt2 & 0x7FF) << 1;

                    sx = irobot_combase.READ_WORD(spnt + 2);
                    sx2 = irobot_combase.READ_WORD(spnt + 4);
                    sy = irobot_combase.READ_WORD(spnt + 6);
                    color = Mame.Machine.pens[sy & 0x3F];
                    sy = ROUND_TO_PIXEL(sy);
                    spnt += 8;

                    word1 = (short)irobot_combase.READ_WORD(spnt);
                    ey = irobot_combase.READ_WORD(spnt + 2);
                    if (word1 != -1 || ey != 0xFFFF)
                    {
                        ey = ROUND_TO_PIXEL(ey);
                        spnt += 4;

                        sx += word1;

                        word2 = (short)irobot_combase.READ_WORD(spnt2);
                        ey2 = ROUND_TO_PIXEL(irobot_combase.READ_WORD(spnt2 + 2));
                        spnt2 += 4;

                        sx2 += word2;

                        while (true)
                        {
                            if (sy >= ir_ymin && sy < ir_ymax)
                            {
                                int x1 = ROUND_TO_PIXEL(sx);
                                int x2 = ROUND_TO_PIXEL(sx2);
                                int temp;

                                if (x1 > x2) { temp = x1; x1 = x2; x2 = temp; }
                                if (x1 < ir_xmin) x1 = ir_xmin;
                                if (x2 > ir_xmax) x2 = ir_xmax;
                                if (x1 <= x2)
                                    draw_hline(x1, x2, sy, color);
                            }

                            sy++;

                            if (sy >= ey)
                            {
                                word1 = (short)irobot_combase.READ_WORD(spnt);
                                ey = irobot_combase.READ_WORD(spnt + 2);
                                if (word1 == -1 && ey == 0xFFFF)
                                    break;
                                ey = ROUND_TO_PIXEL(ey);
                                spnt += 4;
                            }
                            else
                                sx += word1;

                            if (sy >= ey2)
                            {
                                word2 = (short)irobot_combase.READ_WORD(spnt2);
                                ey2 = ROUND_TO_PIXEL(irobot_combase.READ_WORD(spnt2 + 2));
                                spnt2 += 4;
                            }
                            else
                                sx2 += word2;

                        } //while polygon
                    }//if at least 2 sides
                } //if polygon
            } //while object
        }
        static void irmb_dout(int curop, uint d)
        {
            /* Write to video com ram */
            if (mbops[curop].ramsel == 3)
                irobot_combase_mb.WRITE_WORD((int)(irmb_latch << 1) & 0xfff, (ushort)d);

            /* Write to mathox ram */
            if ((mbops[curop].flags & 0x04) == 0)
            {
                uint ad = mbops[curop].diradd | (irmb_latch & mbops[curop].latchmask);

                if (mbops[curop].diren != 0 || (irmb_latch & 0x6000) == 0)
                    mbRAM.WRITE_WORD((int)(ad << 1) & 0x1fff, (ushort)d); /* MB RAM write */
            }
        }
        static uint irmb_din(int curop)
        {
            uint d = 0;

            if ((mbops[curop].flags & 0x04) == 0 && (mbops[curop].flags & 0x80) != 0)
            {
                uint ad = mbops[curop].diradd | (irmb_latch & mbops[curop].latchmask);

                if (mbops[curop].diren != 0 || (irmb_latch & 0x6000) == 0)
                    d = mbRAM.READ_WORD((int)(ad << 1) & 0x1fff); /* MB RAM read */
                else if ((irmb_latch & 0x4000) != 0)
                    d = mbROM.READ_WORD((int)(ad << 1) + 0x4000); /* MB ROM read, CEMATH = 1 */
                else
                    d = mbROM.READ_WORD((int)(ad << 1) & 0x3fff); /* MB ROM read, CEMATH = 0 */
            }
            return d;
        }

        delegate void _drawfunc(int x1, int x2, int y, int col);
        static _drawfunc[] hline_8_table ={draw_hline_8, draw_hline_8_fx, draw_hline_8_fy, draw_hline_8_fx_fy,draw_hline_8_swap, draw_hline_8_swap_fx, draw_hline_8_swap_fy, draw_hline_8_swap_fx_fy};
        static _drawfunc[] hline_16_table = {
                                     };
        static void draw_hline_8(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[y], x1); int dx = 1; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_fx(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[y], ir_xmax - x1); int dx = -1; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_fy(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[ir_ymax - y], x1); int dx = 1; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_fx_fy(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[ir_ymax - y], ir_xmax - x1); int dx = -1; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }

        static void draw_hline_8_swap(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[x1], y); int dx = polybitmapt.line[1].offset - polybitmapt.line[0].offset; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_swap_fx(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[x1], ir_ymax - y); int dx = polybitmapt.line[1].offset - polybitmapt.line[0].offset; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_swap_fy(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[ir_xmax - x1], y); int dx = polybitmapt.line[0].offset - polybitmapt.line[1].offset; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }
        static void draw_hline_8_swap_fx_fy(int x1, int x2, int y, int col) { _BytePtr dest = new _BytePtr(polybitmapt.line[ir_xmax - x1], ir_ymax - y); int dx = polybitmapt.line[0].offset - polybitmapt.line[1].offset; for (; x1 <= x2; x1++, dest.offset += dx) dest[0] = (byte)col; }


        static _drawfunc draw_hline;
        class machine_driver_irobot : Mame.MachineDriver
        {
            public machine_driver_irobot()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6809, 1500000, readmem, writemem, null, null, Mame.ignore_interrupt, 0));
                frames_per_second = 60;
                vblank_duration = Mame.DEFAULT_REAL_60HZ_VBLANK_DURATION;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 29 * 8 - 1);
                gfxdecodeinfo = driver_irobot.gfxdecodeinfo;
                total_colors = 64 + 32;
                color_table_len = 64 + 32;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_MODIFIES_PALETTE;

                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_POKEY, pokey_interface));
            }
            public override void init_machine()
            {
                _BytePtr MB = Mame.memory_region(Mame.REGION_CPU2);

                /* initialize the memory regions */
                mbROM = new _BytePtr(MB, 0x00000);
                mbRAM = new _BytePtr(MB, 0x0c000);
                comRAM[0] = new _BytePtr(MB, 0x0e000);
                comRAM[1] = new _BytePtr(MB, 0x0f000);

                irvg_vblank = false;
                irvg_running = false;
                irmb_running = false;

                /* set an initial timer to go off on scanline 0 */
                irscanline_timer = Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, scanline_callback);

                irobot_rom_banksel(0, 0);
                irobot_out0_w(0, 0);
                irobot_combase = comRAM[0];
                irobot_combase_mb = comRAM[1];
                irobot_outx = 0;
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write != 0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file != null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                    else
                        Array.Clear(nvram.buffer, nvram.offset, nvram_size[0]);
                }
            }
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
            {
                /* the palette will be initialized by the game. We just set it to some */
                /* pre-cooked values so the startup copyright notice can be displayed. */
                int pi = 0;
                for (int i = 0; i < 64; i++)
                {
                    palette[pi++] = (byte)(((i & 1) >> 0) * 0xff);
                    palette[pi++] = (byte)(((i & 2) >> 1) * 0xff);
                    palette[pi++] = (byte)(((i & 4) >> 2) * 0xff);
                }
                uint cpi = 0;
                /* Convert the color prom for the text palette */
                for (int i = 0; i < 32; i++)
                {
                    uint r, g, b;
                    uint bits, intensity;
                    uint color;

                    color = color_prom[cpi];
                    intensity = color & 0x03;
                    bits = (color >> 6) & 0x03;
                    r = 16 * bits * intensity;
                    bits = (color >> 4) & 0x03;
                    g = 16 * bits * intensity;
                    bits = (color >> 2) & 0x03;
                    b = 16 * bits * intensity;
                    palette[pi++] = (byte)r;
                    palette[pi++] = (byte)g;
                    palette[pi++] = (byte)b;
                    cpi++;
                }

                /* polygons */
                for (ushort i = 0; i < 64; i++)
                    colortable[i]= i;

                /* text */
                for (ushort i = 0; i < TOTAL_COLORS(0); i++)
                {
                    COLOR(colortable, 0, i, ((i & 0x18) | ((i & 0x01) << 2) | ((i & 0x06) >> 1)) + 64);
                }
            }
            public override int vh_start()
            {
                /* Setup 2 bitmaps for the polygon generator */
                polybitmap1 = Mame.osd_create_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);
                polybitmap2 = Mame.osd_create_bitmap(Mame.Machine.drv.screen_width, Mame.Machine.drv.screen_height);

                /* Set clipping */
                ir_xmin = ir_ymin = 0;
                ir_xmax = Mame.Machine.drv.screen_width;
                ir_ymax = Mame.Machine.drv.screen_height;

                /* Compute orientation parameters */
                if (polybitmap1.depth == 8)
                    draw_hline = hline_8_table[Mame.Machine.orientation & Mame.ORIENTATION_MASK];
                else
                    draw_hline = hline_16_table[Mame.Machine.orientation & Mame.ORIENTATION_MASK];

                return 0;
            }
            public override void vh_stop()
            {
                // throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                Mame.palette_recalc();
                int offs, y;
                /* copy the polygon bitmap */
                if (irobot_bufsel != 0)
                    Mame.copybitmap(bitmap, polybitmap1, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                else
                    Mame.copybitmap(bitmap, polybitmap2, false, false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

                /* redraw the non-zero characters in the alpha layer */
                for (y = offs = 0; y < 32; y++)
                    for (int x = 0; x < 32; x++, offs++)
                        if (Generic.videoram[offs] != 0)
                        {
                            int code = Generic.videoram[offs] & 0x3f;
                            int color = ((Generic.videoram[offs] & 0xC0) >> 6) | (irobot_alphamap >> 3);

                            Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                                    (uint)code, (uint)color,
                                    false, false,
                                    8 * x, 8 * y,
                                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_COLOR, 64);
                        }
            }
            public override void vh_eof_callback()
            {
                //
            }

        }
        static void scanline_callback(int scanline)
        {
            if (scanline == 0) irvg_vblank = false;
            if (scanline == 224) irvg_vblank = true;
            
            /* set the IRQ line state based on the 32V line state */
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, (scanline & 32) != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);

            /* set a callback for the next 32-scanline increment */
            scanline += 32;
            if (scanline >= 256) scanline = 0;
            irscanline_timer = Mame.Timer.timer_set(Mame.cpu_getscanlinetime(scanline), scanline, scanline_callback);
        }
        void load_oproms()
        {
            _BytePtr MB = Mame.memory_region(Mame.REGION_CPU2);

            /* allocate RAM */
            //mbops =new irmb_ops[1024];

            for (int i = 0; i < 1024; i++)
            {
                int nxtadd, func, ramsel, diradd, latchmask, dirmask, time;
                mbops[i] = new irmb_ops();
                mbops[i].areg = new UIntSubArray(irmb_regs, (int)(MB[0xC000 + i] & 0x0F));
                mbops[i].breg = new UIntSubArray(irmb_regs, (int)(MB[0xC400 + i] & 0x0F));
                func = (MB[0xC800 + i] & 0x0F) << 5;
                func |= ((MB[0xCC00 + i] & 0x0F) << 1);
                func |= (MB[0xD000 + i] & 0x08) >> 3;
                time = MB[0xD000 + i] & 0x03;
                mbops[i].flags = (byte)((MB[0xD000 + i] & 0x04) >> 2);
                nxtadd = (MB[0xD400 + i] & 0x0C) >> 2;
                diradd = MB[0xD400 + i] & 0x03;
                nxtadd |= ((MB[0xD800 + i] & 0x0F) << 6);
                nxtadd |= ((MB[0xDC00 + i] & 0x0F) << 2);
                diradd |= (MB[0xE000 + i] & 0x0F) << 2;
                func |= (MB[0xE400 + i] & 0x0E) << 9;
                mbops[i].flags |= (byte)((MB[0xE400 + i] & 0x01) << 1);
                mbops[i].flags |= (byte)((MB[0xE800 + i] & 0x0F) << 2);
                mbops[i].flags |= (byte)(((MB[0xEC00 + i] & 0x01) << 6));
                mbops[i].flags |= (byte)((MB[0xEC00 + i] & 0x08) << 4);
                ramsel = (MB[0xEC00 + i] & 0x06) >> 1;
                diradd |= (MB[0xF000 + i] & 0x03) << 6;

                if ((mbops[i].flags & FL_shift) != 0) func |= 0x200;

                mbops[i].func = (uint)func;
                mbops[i].nxtop = nxtadd;// mbops[nxtadd];

                /* determine the number of 12MHz cycles for this operation */
                if (time == 3)
                    mbops[i].cycles = 2;
                else
                    mbops[i].cycles = (byte)(3 + time);

                /* precompute the hardcoded address bits and the mask to be used on the latch value */
                if (ramsel == 0)
                {
                    dirmask = 0x00FC;
                    latchmask = 0x3000;
                }
                else
                {
                    dirmask = 0x0000;
                    latchmask = 0x3FFC;
                }
                if ((ramsel & 2) != 0)
                    latchmask |= 0x0003;
                else
                    dirmask |= 0x0003;

                mbops[i].ramsel = (byte)ramsel;
                mbops[i].diradd = (uint)(diradd & dirmask);
                mbops[i].latchmask = (uint)latchmask;
                mbops[i].diren = (ramsel == 0) ? (byte)1 : (byte)0;

#if DISASSEMBLE_MB_ROM
		disassemble_instruction(&mbops[i]);
#endif
            }
        }
        public override void driver_init()
        {
            for (int i = 0; i < 16; i++)
            {
                irmb_stack[i] = 0;
                irmb_regs[i] = 0;
            }
            irmb_latch = 0;
            load_oproms();
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
