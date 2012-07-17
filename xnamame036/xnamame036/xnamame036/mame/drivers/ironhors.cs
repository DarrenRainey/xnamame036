using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    class driver_ironhors : Mame.GameDriver
    {
        class machine_driver_ironhors : Mame.MachineDriver
        {
            public machine_driver_ironhors()
            {
                throw new Exception();
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
        Mame.RomModule[] rom_ironhors()
        {
            ROM_START("ironhors");
            ROM_REGION(0x10000, Mame.REGION_CPU1);	/* 64k for code */
            ROM_LOAD("13c_h03.bin", 0x4000, 0x8000, 0x24539af1);
            ROM_LOAD("12c_h02.bin", 0xc000, 0x4000, 0xfab07f86);

            ROM_REGION(0x10000, Mame.REGION_CPU2);    /* 64k for audio cpu */
            ROM_LOAD("10c_h01.bin", 0x0000, 0x4000, 0x2b17930f);

            ROM_REGION(0x10000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("09f_h07.bin", 0x00000, 0x8000, 0xc761ec73);
            ROM_LOAD("06f_h04.bin", 0x08000, 0x8000, 0xc1486f61);

            ROM_REGION(0x10000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("08f_h06.bin", 0x00000, 0x8000, 0xf21d8c93);
            ROM_LOAD("07f_h05.bin", 0x08000, 0x8000, 0x60107859);

            ROM_REGION(0x0500, Mame.REGION_PROMS);
            ROM_LOAD("03f_h08.bin", 0x0000, 0x0100, 0x9f6ddf83); /* palette red */
            ROM_LOAD("04f_h09.bin", 0x0100, 0x0100, 0xe6773825); /* palette green */
            ROM_LOAD("05f_h10.bin", 0x0200, 0x0100, 0x30a57860); /* palette blue */
            ROM_LOAD("10f_h12.bin", 0x0300, 0x0100, 0x5eb33e73); /* character lookup table */
            ROM_LOAD("10f_h11.bin", 0x0400, 0x0100, 0xa63e37d8); /* sprite lookup table */
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_ironhors()
        {
            throw new Exception();
        }
        public driver_ironhors()
        {
            drv = new machine_driver_ironhors();
            year = "986";
            name = "ironhors";
            description = "Iron Horse";
            manufacturer = "Konami";
            flags = Mame.ROT0;
            input_ports = input_ports_ironhors();
            rom = rom_ironhors();
            drv.HasNVRAMhandler = false;
        }
    }
}
