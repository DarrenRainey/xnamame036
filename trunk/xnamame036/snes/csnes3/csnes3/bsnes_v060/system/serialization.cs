using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class System
        {
            void serialize(Serializer s)
            {
                s.integer(region);
                s.integer(expansion);

                s.integer(scheduler.clock.cpu_freq);
                s.integer(scheduler.clock.smp_freq);

                s.integer(scheduler.clock.cpucop);
                s.integer(scheduler.clock.cpuppu);
                s.integer(scheduler.clock.cpusmp);
                s.integer(scheduler.clock.smpdsp);
            }
            void serialize_all(Serializer s)
            {
                bus.serialize(s);
                cartridge.serialize(s);
                system.serialize(s);
                cpu.serialize(s);
                smp.serialize(s);
                ppu.serialize(s);
                dsp.serialize(s);

                if (cartridge.mode == Cartridge.Mode.ModeSuperGameBoy) supergameboy.serialize(s);

                if (cartridge.has_superfx) superfx.serialize(s);
                if (cartridge.has_sa1) sa1.serialize(s);
                if (cartridge.has_srtc) srtc.serialize(s);
                if (cartridge.has_sdd1) sdd1.serialize(s);
                if (cartridge.has_spc7110) spc7110.serialize(s);
                if (cartridge.has_cx4) cx4.serialize(s);
                if (cartridge.has_dsp1) dsp1.serialize(s);
                if (cartridge.has_dsp2) dsp2.serialize(s);
                if (cartridge.has_obc1) obc1.serialize(s);
                if (cartridge.has_st010) st010.serialize(s);
                if (cartridge.has_msu) msu.serialize(s);
            }

            public void serialize_init()
            {
                Serializer s = new Serializer();

                byte signature = 0, version = 0, crc32 = 0;
                byte[] description = new byte[512];

                s.integer(signature);
                s.integer(version);
                s.integer(crc32);
                s.array(description);

                serialize_all(s);
                serialize_size = s.size();
            }
        }
    }
}
