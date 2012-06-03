using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class williams
    {
        public static void williams_init_machine()
        {
            /* reset the PIAs */
            _6821pia.pia_reset();

            /* set a timer to go off every 16 scanlines, to toggle the VA11 line and update the screen */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams_va11_callback);

            /* also set a timer to go off on scanline 240 */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(240), 0, williams_count240_callback);
        }
        static void williams_va11_callback(int scanline)
        {
            /* the IRQ signal comes into CB1, and is set to VA11 */
            _6821pia.pia_1_cb1_w(0, scanline & 0x20);

            /* update the screen while we're here */
            williams_vh_update(scanline);

            /* set a timer for the next update */
            scanline += 16;
            if (scanline >= 256) scanline = 0;
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(scanline), scanline, williams_va11_callback);
        }
        static void williams_count240_callback(int param)
        {
            /* the COUNT240 signal comes into CA1, and is set to the logical AND of VA10-VA13 */
            _6821pia.pia_1_ca1_w(0, 1);

            /* set a timer to turn it off once the scanline counter resets */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams_count240_off_callback);

            /* set a timer for next frame */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(240), 0, williams_count240_callback);
        }
        static void williams_count240_off_callback(int param)
        {
            /* the COUNT240 signal comes into CA1, and is set to the logical AND of VA10-VA13 */
            _6821pia.pia_1_ca1_w(0, 0);
        }



        public static void williams2_init_machine()
        {
            /* reset the PIAs */
            _6821pia.pia_reset();

            /* make sure our banking is reset */
            williams2_bank_select(0, 0);

            /* set a timer to go off every 16 scanlines, to toggle the VA11 line and update the screen */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(0), 0, williams2_va11_callback);

            /* also set a timer to go off on scanline 254 */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(254), 0, williams2_endscreen_callback);
        }
        static void williams2_bank_select(int offset, int data)
        {
            uint[] bank = { 0, 0x10000, 0x20000, 0x10000, 0, 0x30000, 0x40000, 0x30000 };

            /* select bank index (only lower 3 bits used by IC56) */
            williams2_bank = (byte)(data & 0x07);

            /* bank 0 references videoram */
            if (williams2_bank == 0)
            {
                Mame.cpu_setbank(1, williams_videoram);
                Mame.cpu_setbank(2, new _BytePtr(williams_videoram, 0x8000));
            }

            /* other banks reference ROM plus either palette RAM or the top of videoram */
            else
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU1);

                Mame.cpu_setbank(1, new _BytePtr(RAM, (int)bank[williams2_bank]));

                if ((williams2_bank & 0x03) == 0x03)
                {
                    Mame.cpu_setbank(2, williams2_paletteram);
                }
                else
                {
                    Mame.cpu_setbank(2, new _BytePtr(williams_videoram, 0x8000));
                }
            }

            /* regardless, the top 2k references videoram */
            Mame.cpu_setbank(3, new _BytePtr(williams_videoram, 0x8800));
        }


        static void williams_main_irq(int state)
        {
            /* IRQ to the main CPU */
            Mame.cpu_set_irq_line(0, Mame.cpu_m6809.M6809_IRQ_LINE, state != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }
        static void williams_deferred_snd_cmd_w(int param)
        {
            _6821pia.pia_2_portb_w(0, param);
            _6821pia.pia_2_cb1_w(0, (param == 0xff) ? 0 : 1);
        }

        static void williams_snd_irq(int state)
        {
            /* IRQ to the sound CPU */
            Mame.cpu_set_irq_line(1, Mame.cpu_m6800.M6800_IRQ_LINE, state != 0 ? Mame.ASSERT_LINE : Mame.CLEAR_LINE);
        }

        static void williams_snd_cmd_w(int offset, int cmd)
        {
            /* the high two bits are set externally, and should be 1 */
            Mame.Timer.timer_set(Mame.Timer.TIME_NOW, cmd | 0xc0, williams_deferred_snd_cmd_w);
        }


        public static void defender_bank_select_w(int offset, int data)
        {
            uint bank_offset = defender_bank_list[data & 7];

            /* set bank address */
            Mame.cpu_setbank(2, new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), (int)bank_offset));

            /* if the bank maps into normal RAM, it represents I/O space */
            if (bank_offset < 0x10000)
            {
                Mame.cpu_setbankhandler_r(2, defender_io_r);
                Mame.cpu_setbankhandler_w(2, defender_io_w);
            }

            /* otherwise, it's ROM space */
            else
            {
                Mame.cpu_setbankhandler_r(2, Mame.MRA_BANK2_handler);
                Mame.cpu_setbankhandler_w(2, Mame.MWA_ROM_handler);
            }
        }
        static void defender_io_w(int offset, int data)
        {
            /* write the data through */
            defender_bank_base[offset] = (byte)data;

            /* watchdog */
            if (offset == 0x03fc)
                Mame.watchdog_reset_w(offset, data);

            /* palette */
            else if (offset < 0x10)
                Mame.paletteram_BBGGGRRR_w(offset, data);

            /* PIAs */
            else if (offset >= 0x0c00 && offset < 0x0c04)
                _6821pia.pia_1_w(offset & 3, data);
            else if (offset >= 0x0c04 && offset < 0x0c08)
                _6821pia.pia_0_w(offset & 3, data);
        }

        static int defender_io_r(int offset)
        {
            /* PIAs */
            if (offset >= 0x0c00 && offset < 0x0c04)
                return _6821pia.pia_1_r(offset & 3);
            else if (offset >= 0x0c04 && offset < 0x0c08)
                return _6821pia.pia_0_r(offset & 3);

            /* video counter */
            else if (offset == 0x800)
                return williams_video_counter_r(offset);

            /* If not bank 0 then return banked RAM */
            return defender_bank_base[offset];
        }

        public static int defender_input_port_0_r(int offset)
        {
            int keys, altkeys;

            /* read the standard keys and the cheat keys */
            keys = Mame.readinputport(0);
            altkeys = Mame.readinputport(3);

            /* modify the standard keys with the cheat keys */
            if (altkeys != 0)
            {
                keys |= altkeys;
                if (Mame.memory_region(Mame.REGION_CPU1)[0xa0bb] == 0xfd)
                {
                    if ((keys & 0x02) != 0)
                        keys = (keys & 0xfd) | 0x40;
                    else if ((keys & 0x40) != 0)
                        keys = (keys & 0xbf) | 0x02;
                }
            }

            return keys;
        }
        static void williams2_va11_callback(int scanline)
        {
            /* the IRQ signal comes into CB1, and is set to VA11 */
            _6821pia.pia_0_cb1_w(0, scanline & 0x20);
            _6821pia.pia_1_ca1_w(0, scanline & 0x20);

            /* update the screen while we're here */
            williams2_vh_update(scanline);

            /* set a timer for the next update */
            scanline += 16;
            if (scanline >= 256) scanline = 0;
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(scanline), scanline, williams2_va11_callback);
        }


        static void williams2_endscreen_off_callback(int param)
        {
            /* the /ENDSCREEN signal comes into CA1 */
            _6821pia.pia_0_ca1_w(0, 1);
        }


        static void williams2_endscreen_callback(int param)
        {
            /* the /ENDSCREEN signal comes into CA1 */
            _6821pia.pia_0_ca1_w(0, 0);

            /* set a timer to turn it off once the scanline counter resets */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(8), 0, williams2_endscreen_off_callback);

            /* set a timer for next frame */
            Mame.Timer.timer_set(Mame.cpu_getscanlinetime(254), 0, williams2_endscreen_callback);
        }



    }
}
