using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    public class machine_driver_srdmissn : Mame.MachineDriver
    {
        public machine_driver_srdmissn()
        {
            cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 4, driver_kyugo.srdmissn_readmem, driver_kyugo.srdmissn_writemem, driver_kyugo.srdmissn_readport, driver_kyugo.srdmissn_writeport, Mame.nmi_interrupt, 1));
            cpu.Add(new Mame.MachineCPU(Mame.CPU_Z80, 18432000 / 4, driver_kyugo.srdmissn_sub_readmem, driver_kyugo.srdmissn_sub_writemem, driver_kyugo.srdmissn_sub_readport, driver_kyugo.srdmissn_sub_writeport, Mame.interrupt, 4));

            frames_per_second = 60;
            vblank_duration = Mame.DEFAULT_60HZ_VBLANK_DURATION;
            cpu_slices_per_frame = 100;
            screen_width = 64 * 8;
            screen_height = 32 * 8;
            visible_area = new Mame.rectangle(0 * 8, 36 * 8 - 1, 2 * 8, 30 * 8 - 1);
            gfxdecodeinfo = driver_kyugo.gfxdecodeinfo;
            total_colors = 256;
            color_table_len = 256;
            video_attributes = Mame.VIDEO_TYPE_RASTER;
            sound_attributes = 0;
            sound.Add(new Mame.MachineSound(Mame.SOUND_AY8910, driver_kyugo.ay8910_interface));
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
            uint pi = 0, cpi = 0;
            for (int i = 0; i < Mame.Machine.drv.total_colors; i++)
            {
                /* red component */
                int bit0 = (color_prom[cpi] >> 0) & 0x01;
                int bit1 = (color_prom[cpi] >> 1) & 0x01;
                int bit2 = (color_prom[cpi] >> 2) & 0x01;
                int bit3 = (color_prom[cpi] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* green component */
                bit0 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                /* blue component */
                bit0 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 0) & 0x01;
                bit1 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 1) & 0x01;
                bit2 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 2) & 0x01;
                bit3 = (color_prom[cpi + 2 * Mame.Machine.drv.total_colors] >> 3) & 0x01;
                palette[pi++] = (byte)(0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3);
                cpi++;
            }

            cpi += (2 * Mame.Machine.drv.total_colors);

            /* color_prom now points to the beginning of the character color codes */
            driver_kyugo.color_codes = new _BytePtr(color_prom, (int)cpi);	/* we'll need it later */
        }
        public override int vh_start()
        {
            return Generic.generic_vh_start();
        }
        public override void vh_stop()
        {
            Generic.generic_vh_stop();
        }

        static void draw_sprites(Mame.osd_bitmap bitmap)
        {
            /* sprite information is scattered through memory */
            /* and uses a portion of the text layer memory (outside the visible area) */
            _BytePtr spriteram_area1 = new _BytePtr(Generic.spriteram, 0x28);
            _BytePtr spriteram_area2 = new _BytePtr(Generic.spriteram_2, 0x28);
            _BytePtr spriteram_area3 = new _BytePtr(driver_kyugo.kyugo_videoram, 0x28);

            for (int n = 0; n < 12 * 2; n++)
            {
                int offs = 2 * (n % 12) + 64 * (n / 12);

                int sx = spriteram_area3[offs + 1] + 256 * (spriteram_area2[offs + 1] & 1);
                if (sx > 320) sx -= 512;

                int sy = 255 - spriteram_area1[offs];
                if (driver_kyugo.flipscreen != 0) sy = 240 - sy;

                int color = spriteram_area1[offs + 1] & 0x1f;

                for (int y = 0; y < 16; y++)
                {
                    int attr2 = spriteram_area2[offs + 128 * y];
                    int code = spriteram_area3[offs + 128 * y];
                    if ((attr2 & 0x01) != 0) code += 512;
                    if ((attr2 & 0x02) != 0) code += 256;
                    bool flipx = (attr2 & 0x08) != 0;
                    bool flipy = (attr2 & 0x04) != 0;
                    if (driver_kyugo.flipscreen != 0)
                    {
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                        (uint)code,
                        (uint)color,
                        flipx, flipy,
                        sx, driver_kyugo.flipscreen != 0 ? sy - 16 * y : sy + 16 * y,
                        Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
                }
            }
        }
        public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
        {
            /* back layer */
            for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
            {
                if (Generic.dirtybuffer[offs])
                {
                    Generic.dirtybuffer[offs] = false;

                    int sx = offs % 64;
                    int sy = offs / 64;
                    bool flipx = (Generic.colorram[offs] & 0x04) != 0;
                    bool flipy = (Generic.colorram[offs] & 0x08) != 0;
                    if (driver_kyugo.flipscreen == 0)
                    {
                        sx = 63 - sx;
                        sy = 31 - sy;
                        flipx = !flipx;
                        flipy = !flipy;
                    }

                    int tile = Generic.videoram[offs] + (256 * (Generic.colorram[offs] & 3));

                    Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[2],
                        (uint)tile,
                        (uint)(((Generic.colorram[offs] & 0xf0) >> 4) + (driver_kyugo.palbank << 4)),
                        flipx, flipy,
                        8 * sx, 8 * sy,
                        null, Mame.TRANSPARENCY_NONE, 0);
                }
            }

            int scrollx, scrolly;

            if (driver_kyugo.flipscreen != 0)
            {
                scrollx = -32 - ((driver_kyugo.kyugo_back_scrollY_lo[0]) + (driver_kyugo.kyugo_back_scrollY_hi * 256));
                scrolly = driver_kyugo.kyugo_back_scrollX[0];
            }
            else
            {
                scrollx = -32 - ((driver_kyugo.kyugo_back_scrollY_lo[0]) + (driver_kyugo.kyugo_back_scrollY_hi * 256));
                scrolly = -driver_kyugo.kyugo_back_scrollX[0];
            }

            /* copy the temporary bitmap to the screen */
            Mame.copyscrollbitmap(bitmap, Generic.tmpbitmap, 1, new int[] { scrollx }, 1, new int[] { scrolly }, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);

            /* sprites */
            draw_sprites(bitmap);

            /* front layer */
            for (int offs = driver_kyugo.kyugo_videoram_size[0] - 1; offs >= 0; offs--)
            {
                int sx = offs % 64;
                int sy = offs / 64;
                if (driver_kyugo.flipscreen != 0)
                {
                    sx = 35 - sx;
                    sy = 31 - sy;
                }

                int code = driver_kyugo.kyugo_videoram[offs];

                Mame.drawgfx(bitmap, Mame.Machine.gfx[0],
                    (uint)code,
                    (uint)(2 * driver_kyugo.color_codes[code / 8] + driver_kyugo.frontcolor),
                    driver_kyugo.flipscreen != 0, driver_kyugo.flipscreen != 0,
                    8 * sx, 8 * sy,
                    Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_PEN, 0);
            }
        }
        public override void vh_eof_callback()
        {
            //none
        }
    }
    partial class driver_kyugo : Mame.GameDriver
    {
        public static _BytePtr shared_ram = new _BytePtr(1);
        public static _BytePtr kyugo_videoram = new _BytePtr(1);
        public static int[] kyugo_videoram_size = new int[1];
        public static _BytePtr kyugo_back_scrollY_lo = new _BytePtr(1);
        public static _BytePtr kyugo_back_scrollX = new _BytePtr(1);

        public static byte kyugo_back_scrollY_hi;
        public static int palbank, frontcolor;
        public static int flipscreen;
        public static _BytePtr color_codes;

        public static Mame.IOReadPort[] gyrodine_readport = { new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] gyrodine_writeport = { new Mame.IOWritePort(0x00 + 0, 0x00 + 0, Mame.interrupt_enable_w), new Mame.IOWritePort(0x00 + 1, 0x00 + 1, kyugo_flipscreen_w), new Mame.IOWritePort(0x00 + 2, 0x00 + 2, sub_cpu_control_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] sonofphx_readport = { new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] sonofphx_writeport = { new Mame.IOWritePort(0x00 + 0, 0x00 + 0, Mame.interrupt_enable_w), new Mame.IOWritePort(0x00 + 1, 0x00 + 1, kyugo_flipscreen_w), new Mame.IOWritePort(0x00 + 2, 0x00 + 2, sub_cpu_control_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] flashgal_readport = { new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] flashgal_writeport = { new Mame.IOWritePort(0x40 + 0, 0x40 + 0, Mame.interrupt_enable_w), new Mame.IOWritePort(0x40 + 1, 0x40 + 1, kyugo_flipscreen_w), new Mame.IOWritePort(0x40 + 2, 0x40 + 2, sub_cpu_control_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] srdmissn_readport = { new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] srdmissn_writeport = { new Mame.IOWritePort(0x08 + 0, 0x08 + 0, Mame.interrupt_enable_w), new Mame.IOWritePort(0x08 + 1, 0x08 + 1, kyugo_flipscreen_w), new Mame.IOWritePort(0x08 + 2, 0x08 + 2, sub_cpu_control_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] gyrodine_sub_readport = { new Mame.IOReadPort(0x00 + 2, 0x00 + 2, ay8910.AY8910_read_port_0_r), new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] gyrodine_sub_writeport = { new Mame.IOWritePort(0x00 + 0, 0x00 + 0, ay8910.AY8910_control_port_0_w), new Mame.IOWritePort(0x00 + 1, 0x00 + 1, ay8910.AY8910_write_port_0_w), new Mame.IOWritePort(0xc0 + 0, 0xc0 + 0, ay8910.AY8910_control_port_1_w), new Mame.IOWritePort(0xc0 + 1, 0xc0 + 1, ay8910.AY8910_write_port_1_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] sonofphx_sub_readport = { new Mame.IOReadPort(0x00 + 2, 0x00 + 2, ay8910.AY8910_read_port_0_r), new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] sonofphx_sub_writeport = { new Mame.IOWritePort(0x00 + 0, 0x00 + 0, ay8910.AY8910_control_port_0_w), new Mame.IOWritePort(0x00 + 1, 0x00 + 1, ay8910.AY8910_write_port_0_w), new Mame.IOWritePort(0x40 + 0, 0x40 + 0, ay8910.AY8910_control_port_1_w), new Mame.IOWritePort(0x40 + 1, 0x40 + 1, ay8910.AY8910_write_port_1_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] flashgal_sub_readport = { new Mame.IOReadPort(0x00 + 2, 0x00 + 2, ay8910.AY8910_read_port_0_r), new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] flashgal_sub_writeport = { new Mame.IOWritePort(0x00 + 0, 0x00 + 0, ay8910.AY8910_control_port_0_w), new Mame.IOWritePort(0x00 + 1, 0x00 + 1, ay8910.AY8910_write_port_0_w), new Mame.IOWritePort(0x40 + 0, 0x40 + 0, ay8910.AY8910_control_port_1_w), new Mame.IOWritePort(0x40 + 1, 0x40 + 1, ay8910.AY8910_write_port_1_w), new Mame.IOWritePort(-1) };
        public static Mame.IOReadPort[] srdmissn_sub_readport = { new Mame.IOReadPort(0x80 + 2, 0x80 + 2, ay8910.AY8910_read_port_0_r), new Mame.IOReadPort(-1) };
        public static Mame.IOWritePort[] srdmissn_sub_writeport = { new Mame.IOWritePort(0x80 + 0, 0x80 + 0, ay8910.AY8910_control_port_0_w), new Mame.IOWritePort(0x80 + 1, 0x80 + 1, ay8910.AY8910_write_port_0_w), new Mame.IOWritePort(0x84 + 0, 0x84 + 0, ay8910.AY8910_control_port_1_w), new Mame.IOWritePort(0x84 + 1, 0x84 + 1, ay8910.AY8910_write_port_1_w), new Mame.IOWritePort(-1) };

        public static Mame.MemoryReadAddress[] gyrodine_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0x8000, 0x87ff, Generic.videoram_r), new Mame.MemoryReadAddress(0x8800, 0x8fff, Generic.colorram_r), new Mame.MemoryReadAddress(0x9000, 0x97ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0x9800, 0x9fff, special_spriteram_r), new Mame.MemoryReadAddress(0xa000, 0xa7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xf000, 0xf000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0xf800, 0xffff, shared_ram_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] gyrodine_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0x8000, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size), new Mame.MemoryWriteAddress(0x8800, 0x8fff, Generic.colorram_w, Generic.colorram), new Mame.MemoryWriteAddress(0x9000, 0x97ff, Mame.MWA_RAM, kyugo_videoram, kyugo_videoram_size), new Mame.MemoryWriteAddress(0x9800, 0x9fff, Mame.MWA_RAM, Generic.spriteram_2), new Mame.MemoryWriteAddress(0xa000, 0xa7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size), new Mame.MemoryWriteAddress(0xa800, 0xa800, Mame.MWA_RAM, kyugo_back_scrollY_lo), new Mame.MemoryWriteAddress(0xb000, 0xb000, kyugo_gfxctrl_w), new Mame.MemoryWriteAddress(0xb800, 0xb800, Mame.MWA_RAM, kyugo_back_scrollX), new Mame.MemoryWriteAddress(0xf000, 0xf000 + 0x7ff, shared_ram_w, shared_ram), new Mame.MemoryWriteAddress(0xf800, 0xffff, shared_ram_w), new Mame.MemoryWriteAddress(0xe000, 0xe000, Mame.watchdog_reset_w), new Mame.MemoryWriteAddress(-1) };

        public static Mame.MemoryReadAddress[] sonofphx_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0x8000, 0x87ff, Generic.videoram_r), new Mame.MemoryReadAddress(0x8800, 0x8fff, Generic.colorram_r), new Mame.MemoryReadAddress(0x9000, 0x97ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0x9800, 0x9fff, special_spriteram_r), new Mame.MemoryReadAddress(0xa000, 0xa7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xf000, 0xf000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0xf800, 0xffff, shared_ram_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] sonofphx_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0x8000, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size), new Mame.MemoryWriteAddress(0x8800, 0x8fff, Generic.colorram_w, Generic.colorram), new Mame.MemoryWriteAddress(0x9000, 0x97ff, Mame.MWA_RAM, kyugo_videoram, kyugo_videoram_size), new Mame.MemoryWriteAddress(0x9800, 0x9fff, Mame.MWA_RAM, Generic.spriteram_2), new Mame.MemoryWriteAddress(0xa000, 0xa7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size), new Mame.MemoryWriteAddress(0xa800, 0xa800, Mame.MWA_RAM, kyugo_back_scrollY_lo), new Mame.MemoryWriteAddress(0xb000, 0xb000, kyugo_gfxctrl_w), new Mame.MemoryWriteAddress(0xb800, 0xb800, Mame.MWA_RAM, kyugo_back_scrollX), new Mame.MemoryWriteAddress(0xf000, 0xf000 + 0x7ff, shared_ram_w, shared_ram), new Mame.MemoryWriteAddress(0xf800, 0xffff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000, Mame.watchdog_reset_w), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] flashgal_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0x8000, 0x87ff, Generic.videoram_r), new Mame.MemoryReadAddress(0x8800, 0x8fff, Generic.colorram_r), new Mame.MemoryReadAddress(0x9000, 0x97ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0x9800, 0x9fff, special_spriteram_r), new Mame.MemoryReadAddress(0xa000, 0xa7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xf000, 0xf000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0xf800, 0xffff, shared_ram_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] flashgal_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0x8000, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size), new Mame.MemoryWriteAddress(0x8800, 0x8fff, Generic.colorram_w, Generic.colorram), new Mame.MemoryWriteAddress(0x9000, 0x97ff, Mame.MWA_RAM, kyugo_videoram, kyugo_videoram_size), new Mame.MemoryWriteAddress(0x9800, 0x9fff, Mame.MWA_RAM, Generic.spriteram_2), new Mame.MemoryWriteAddress(0xa000, 0xa7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size), new Mame.MemoryWriteAddress(0xa800, 0xa800, Mame.MWA_RAM, kyugo_back_scrollY_lo), new Mame.MemoryWriteAddress(0xb000, 0xb000, kyugo_gfxctrl_w), new Mame.MemoryWriteAddress(0xb800, 0xb800, Mame.MWA_RAM, kyugo_back_scrollX), new Mame.MemoryWriteAddress(0xf000, 0xf000 + 0x7ff, shared_ram_w, shared_ram), new Mame.MemoryWriteAddress(0xf800, 0xffff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000, Mame.watchdog_reset_w), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] srdmissn_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0x8000, 0x87ff, Generic.videoram_r), new Mame.MemoryReadAddress(0x8800, 0x8fff, Generic.colorram_r), new Mame.MemoryReadAddress(0x9000, 0x97ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0x9800, 0x9fff, special_spriteram_r), new Mame.MemoryReadAddress(0xa000, 0xa7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xe000, 0xe000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0xf800, 0xffff, shared_ram_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] srdmissn_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0x8000, 0x87ff, Generic.videoram_w, Generic.videoram, Generic.videoram_size), new Mame.MemoryWriteAddress(0x8800, 0x8fff, Generic.colorram_w, Generic.colorram), new Mame.MemoryWriteAddress(0x9000, 0x97ff, Mame.MWA_RAM, kyugo_videoram, kyugo_videoram_size), new Mame.MemoryWriteAddress(0x9800, 0x9fff, Mame.MWA_RAM, Generic.spriteram_2), new Mame.MemoryWriteAddress(0xa000, 0xa7ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size), new Mame.MemoryWriteAddress(0xa800, 0xa800, Mame.MWA_RAM, kyugo_back_scrollY_lo), new Mame.MemoryWriteAddress(0xb000, 0xb000, kyugo_gfxctrl_w), new Mame.MemoryWriteAddress(0xb800, 0xb800, Mame.MWA_RAM, kyugo_back_scrollX), new Mame.MemoryWriteAddress(0xe000, 0xe000 + 0x7ff, shared_ram_w, shared_ram), new Mame.MemoryWriteAddress(0xf800, 0xffff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000, Mame.watchdog_reset_w), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] gyrodine_sub_readmem = { new Mame.MemoryReadAddress(0x0000, 0x1fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0x4000, 0x4000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0x0000, 0x0000 + 0x7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0x8080, 0x8080, Mame.input_port_2_r), new Mame.MemoryReadAddress(0x8040, 0x8040, Mame.input_port_3_r), new Mame.MemoryReadAddress(0x8000, 0x8000, Mame.input_port_4_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] gyrodine_sub_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x1fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0x4000, 0x4000 + 0x7ff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000 + 0x7ff, Mame.MWA_RAM), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] sonofphx_sub_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0xa000, 0xa000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0x0000, 0x0000 + 0x7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xc080, 0xc080, Mame.input_port_2_r), new Mame.MemoryReadAddress(0xc040, 0xc040, Mame.input_port_3_r), new Mame.MemoryReadAddress(0xc000, 0xc000, Mame.input_port_4_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] sonofphx_sub_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0xa000, 0xa000 + 0x7ff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000 + 0x7ff, Mame.MWA_RAM), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] flashgal_sub_readmem = { new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), new Mame.MemoryReadAddress(0xa000, 0xa000 + 0x7ff, shared_ram_r), new Mame.MemoryReadAddress(0x0000, 0x0000 + 0x7ff, Mame.MRA_RAM), new Mame.MemoryReadAddress(0xc080, 0xc080, Mame.input_port_2_r), new Mame.MemoryReadAddress(0xc040, 0xc040, Mame.input_port_3_r), new Mame.MemoryReadAddress(0xc000, 0xc000, Mame.input_port_4_r), new Mame.MemoryReadAddress(-1) };
        public static Mame.MemoryWriteAddress[] flashgal_sub_writemem = { new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), new Mame.MemoryWriteAddress(0xa000, 0xa000 + 0x7ff, shared_ram_w), new Mame.MemoryWriteAddress(0x0000, 0x0000 + 0x7ff, Mame.MWA_RAM), new Mame.MemoryWriteAddress(-1) };
        public static Mame.MemoryReadAddress[] srdmissn_sub_readmem = { 
            new Mame.MemoryReadAddress(0x0000, 0x7fff, Mame.MRA_ROM), 
            new Mame.MemoryReadAddress(0x8000, 0x8000 + 0x7ff, shared_ram_r), 
            new Mame.MemoryReadAddress(0x8800, 0x8800 + 0x7ff, Mame.MRA_RAM), 
            new Mame.MemoryReadAddress(0xf400, 0xf400, Mame.input_port_2_r), 
            new Mame.MemoryReadAddress(0xf401, 0xf401, Mame.input_port_3_r), 
            new Mame.MemoryReadAddress(0xf402, 0xf402, Mame.input_port_4_r), 
            new Mame.MemoryReadAddress(-1) 
                                                                      };
        public static Mame.MemoryWriteAddress[] srdmissn_sub_writemem = 
        { 
            new Mame.MemoryWriteAddress(0x0000, 0x7fff, Mame.MWA_ROM), 
            new Mame.MemoryWriteAddress(0x8000, 0x8000 + 0x7ff, shared_ram_w), 
            new Mame.MemoryWriteAddress(0x8800, 0x8800 + 0x7ff, Mame.MWA_RAM),
            new Mame.MemoryWriteAddress(-1)
        };


        public Mame.InputPortTiny[] input_ports_sonofphx()
        {
            INPUT_PORTS_START("sonofphx");	/* sonofphx, srdmissn, airwolf? */
            PORT_START("DSW1");
            PORT_DIPNAME(0x03, 0x03, DEF_STR("Lives"));
            PORT_DIPSETTING(0x03, "3");
            PORT_DIPSETTING(0x02, "4");
            PORT_DIPSETTING(0x01, "5");
            PORT_DIPSETTING(0x00, "6");
            PORT_DIPNAME(0x04, 0x04, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x04, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x08, 0x08, "Slow Motion");
            PORT_DIPSETTING(0x08, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_BITX(0x10, 0x10, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Invulnerability", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x10, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x20, "Sound Test");
            PORT_DIPSETTING(0x20, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x40, DEF_STR("Cocktail"));
            PORT_DIPNAME(0x80, 0x80, "Freeze");
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("DSW2");
            PORT_DIPNAME(0x07, 0x07, DEF_STR("Coin A"));
            PORT_DIPSETTING(0x02, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x01, DEF_STR("3C_2C"));
            PORT_DIPSETTING(0x07, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x06, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x05, DEF_STR("1C_3C"));
            PORT_DIPSETTING(0x04, DEF_STR("1C_4C"));
            PORT_DIPSETTING(0x03, DEF_STR("1C_6C"));
            PORT_DIPSETTING(0x00, DEF_STR("Free Play"));
            PORT_DIPNAME(0x38, 0x38, DEF_STR("Coin B"));
            PORT_DIPSETTING(0x00, DEF_STR("5C_1C"));
            PORT_DIPSETTING(0x08, DEF_STR("4C_1C"));
            PORT_DIPSETTING(0x10, DEF_STR("3C_1C"));
            PORT_DIPSETTING(0x18, DEF_STR("2C_1C"));
            PORT_DIPSETTING(0x38, DEF_STR("1C_1C"));
            PORT_DIPSETTING(0x20, DEF_STR("3C_4C"));
            PORT_DIPSETTING(0x30, DEF_STR("1C_2C"));
            PORT_DIPSETTING(0x28, DEF_STR("1C_3C"));
            PORT_DIPNAME(0x40, 0x40, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x40, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x80, DEF_STR("Unknown"));
            PORT_DIPSETTING(0x80, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));

            PORT_START("IN0");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN3);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN1");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);

            PORT_START("IN2");
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_8WAY | IPF_COCKTAIL);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON1 | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_BUTTON2 | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            return INPUT_PORTS_END;
        }

        static int shared_ram_r(int offs)
        {
            return shared_ram[offs];
        }
        static void shared_ram_w(int offs, int data)
        {
            shared_ram[offs] = (byte)data;
        }
        static int special_spriteram_r(int offs)
        {
            /* RAM is 4 bits wide, must set the high bits to 1 for the RAM test to pass */
            return Generic.spriteram_2[offs] | 0xf0;
        }
        static void kyugo_gfxctrl_w(int offset, int data)
        {
            /* bit 0 is scroll MSB */
            kyugo_back_scrollY_hi = (byte)(data & 0x01);

            /* bit 5 is front layer color (Son of Phoenix only) */
            frontcolor = (data & 0x20) >> 5;

            /* bit 6 is background palette bank */
            if (palbank != ((data & 0x40) >> 6))
            {
                palbank = (data & 0x40) >> 6;
                Generic.SetDirtyBuffer(true);
            }

            if ((data & 0x9e) != 0)
            {
                string baf = Mame.sprintf("%02x", data);
                Mame.usrintf_showmessage(baf);
            }
        }
        static void sub_cpu_control_w(int offs, int data)
        {
            if ((data & 1) != 0)
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
            else
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
        }
        static void kyugo_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (data & 0x01))
            {
                flipscreen = (data & 0x01);
                Generic.SetDirtyBuffer(true);
            }
        }


        static Mame.GfxLayout charlayout =
        new Mame.GfxLayout(
            8, 8,           /* 8*8 characters */
            256,           /* 256 characters */
            2,             /* 2 bits per pixel */
            new uint[] { 0, 4 },
            new uint[] { 0, 1, 2, 3, 8 * 8 + 0, 8 * 8 + 1, 8 * 8 + 2, 8 * 8 + 3 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            16 * 8           /* every char takes 16 bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,	/* 16*16 sprites */
            1024,	/* 1024 sprites */
            3,		/* 3 bits per pixel */
            new uint[] { 0 * 1024 * 32 * 8, 1 * 1024 * 32 * 8, 2 * 1024 * 32 * 8 },	/* the bitplanes are separated */
            new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7,
		8*8+0, 8*8+1, 8*8+2, 8*8+3, 8*8+4, 8*8+5, 8*8+6, 8*8+7 },
            new uint[]{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8,
			16*8, 17*8, 18*8, 19*8, 20*8, 21*8, 22*8, 23*8 },
            32 * 8	/* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxLayout tilelayout =
        new Mame.GfxLayout(
            8, 8,	/* 16*16 tiles */
            1024,	/* 1024 tiles */
            3,		/* 3 bits per pixel */
            new uint[] { 0 * 1024 * 8 * 8, 1 * 1024 * 8 * 8, 2 * 1024 * 8 * 8 },	/* the bitplanes are separated */
        new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 },
            new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
            8 * 8	/* every tile takes 32 consecutive bytes */
        );

        public static Mame.GfxDecodeInfo[] gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0, charlayout,		0, 64 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0, spritelayout,	0, 32 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX3, 0, tilelayout,		0, 32 ),
};

        public static AY8910interface ay8910_interface =
new AY8910interface(
    2,      /* 2 chips */
    1500000,        /* 1.5 MHz ? */
    new int[] { 30, 30 },
    new AY8910portRead[] { Mame.input_port_0_r, null },
    new AY8910portRead[] { Mame.input_port_1_r, null },
    new AY8910portWrite[] { null, null },
    new AY8910portWrite[] { null, null },
    null
);

        public override void driver_init()
        {
        }
    }

    class driver_airwolf : driver_kyugo
    {
        public override void driver_init()
        {
            //none
        }
        Mame.RomModule[] rom_airwolf()
        {
            ROM_START("airwolf");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 64k for code */
            ROM_LOAD("b.2s", 0x0000, 0x8000, 0x8c993cce);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64k for code */
            ROM_LOAD("a.7s", 0x0000, 0x8000, 0xa3c7af5c);

            ROM_REGION(0x01000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("f.4a", 0x00000, 0x1000, 0x4df44ce9); /* chars */

            ROM_REGION(0x18000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("e.6a", 0x00000, 0x2000, 0xe8fbc7d2); /* sprites - plane 0 */
            ROM_CONTINUE(0x04000, 0x2000);
            ROM_CONTINUE(0x02000, 0x2000);
            ROM_CONTINUE(0x06000, 0x2000);
            ROM_LOAD("d.8a", 0x08000, 0x2000, 0xc5d4156b); /* sprites - plane 1 */
            ROM_CONTINUE(0x0c000, 0x2000);
            ROM_CONTINUE(0x0a000, 0x2000);
            ROM_CONTINUE(0x0e000, 0x2000);
            ROM_LOAD("c.10a", 0x10000, 0x2000, 0xde91dfb1); /* sprites - plane 2 */
            ROM_CONTINUE(0x14000, 0x2000);
            ROM_CONTINUE(0x12000, 0x2000);
            ROM_CONTINUE(0x16000, 0x2000);

            ROM_REGION(0x06000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("09h_14.bin", 0x00000, 0x2000, 0x25e57e1f); /* tiles - plane 1 */
            ROM_LOAD("10h_13.bin", 0x02000, 0x2000, 0xcf0de5e9); /* tiles - plane 0 */
            ROM_LOAD("11h_12.bin", 0x04000, 0x2000, 0x4050c048);/* tiles - plane 2 */

            ROM_REGION(0x0340, Mame.REGION_PROMS);
            ROM_LOAD("01j.bin", 0x0000, 0x0100, 0x6a94b2a3); /* red */
            ROM_LOAD("01h.bin", 0x0100, 0x0100, 0xec0923d3); /* green */
            ROM_LOAD("01f.bin", 0x0200, 0x0100, 0xade97052); /* blue */
            /* 0x0300-0x031f empty - looks like there isn't a lookup table PROM */
            ROM_LOAD("02c_m1.bin", 0x0320, 0x0020, 0x83a39201); /* timing? not used */
            return ROM_END;
        }
        public driver_airwolf()
        {
            drv = new machine_driver_srdmissn();
            year = "1987";
            name = "airwolf";
            description = "Air Wolf";
            manufacturer = "Kyugo";
            flags = Mame.ROT0;
            input_ports = input_ports_sonofphx();
            rom = rom_airwolf();
            drv.HasNVRAMhandler = false;
        }
    }
    class driver_srdmissn : driver_kyugo
    {
        Mame.RomModule[] rom_srdmissn()
        {
            ROM_START("srdmissn");
            ROM_REGION(0x10000, Mame.REGION_CPU1); /* 64k for code */
            ROM_LOAD("5.t2", 0x0000, 0x4000, 0xa682b48c);
            ROM_LOAD("7.t3", 0x4000, 0x4000, 0x1719c58c);

            ROM_REGION(0x10000, Mame.REGION_CPU2); /* 64k for code */
            ROM_LOAD("1.t7", 0x0000, 0x4000, 0xdc48595e);
            ROM_LOAD("3.t8", 0x4000, 0x4000, 0x216be1e8);

            ROM_REGION(0x01000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("15.4a", 0x00000, 0x1000, 0x4961f7fd); /* chars */

            ROM_REGION(0x18000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("14.6a", 0x00000, 0x4000, 0x3d4c0447); /* sprites - plane 0 */
            ROM_LOAD("13.7a", 0x04000, 0x4000, 0x22414a67); /* sprites - plane 0 */
            ROM_LOAD("12.8a", 0x08000, 0x4000, 0x61e34283); /* sprites - plane 1 */
            ROM_LOAD("11.9a", 0x0c000, 0x4000, 0xbbbaffef); /* sprites - plane 1 */
            ROM_LOAD("10.10a", 0x10000, 0x4000, 0xde564f97); /* sprites - plane 2 */
            ROM_LOAD("9.11a", 0x14000, 0x4000, 0x890dc815); /* sprites - plane 2 */

            ROM_REGION(0x06000, Mame.REGION_GFX3 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("17.9h", 0x00000, 0x2000, 0x41211458); /* tiles - plane 1 */
            ROM_LOAD("18.10h", 0x02000, 0x2000, 0x740eccd4); /* tiles - plane 0 */
            ROM_LOAD("16.11h", 0x04000, 0x2000, 0xc1f4a5db); /* tiles - plane 2 */

            ROM_REGION(0x0340, Mame.REGION_PROMS);
            ROM_LOAD("mr.1j", 0x0000, 0x0100, 0x110a436e); /* red */
            ROM_LOAD("mg.1h", 0x0100, 0x0100, 0x0fbfd9f0); /* green */
            ROM_LOAD("mb.1f", 0x0200, 0x0100, 0xa342890c); /* blue */
            ROM_LOAD("m2.5j", 0x0300, 0x0020, 0x190a55ad); /* char lookup table */
            ROM_LOAD("m1.2c", 0x0320, 0x0020, 0x83a39201); /* timing? not used */
            return ROM_END;
        }
        public driver_srdmissn()
        {
            drv = new machine_driver_srdmissn();
            year = "1986";
            name = "srdmissn";
            description = "S.R.D. Mission";
            manufacturer = "Taito Corporation";
            flags = Mame.ROT90;
            input_ports = input_ports_sonofphx();
            rom = rom_srdmissn();
            drv.HasNVRAMhandler = false;
        }

    }
}
