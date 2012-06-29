using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_battlane : Mame.GameDriver
    {
        class machine_driver_battlane : Mame.MachineDriver
        {
            public machine_driver_battlane()
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
            public override void vh_init_palette(byte[] palette, ushort[] colortable, _BytePtr color_prom)
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
            public override void vh_eof_callback()
            {
                throw new NotImplementedException();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                throw new NotImplementedException();
            }
        }
        public override void driver_init()
        {
            throw new NotImplementedException();
        }
        Mame.RomModule[] rom_battlane()
        {
            throw new Exception();
        }
        Mame.InputPortTiny[] input_ports_battlane()
        {
            throw new Exception();
        }
        public driver_battlane()
        {
            drv = new machine_driver_battlane();
            year = "1986";
            name = "battlane";
            description = "Battle Lane Vol. 5 (set 1)";
            manufacturer = "Technos (Taito license)";
            flags = Mame.ROT90 | Mame.GAME_WRONG_COLORS | Mame.GAME_NO_COCKTAIL;
            input_ports = input_ports_battlane();
            rom = rom_battlane();
            drv.HasNVRAMhandler = false;
        }
    }
}
