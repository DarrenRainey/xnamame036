using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class memory
        {
            public static MappedRAM cartrom = new MappedRAM(), cartram = new MappedRAM(), cartrtc = new MappedRAM();
            public static MappedRAM bsxflash = new MappedRAM(), bsxram = new MappedRAM(), bsxpram = new MappedRAM();
            public static MappedRAM stArom = new MappedRAM(), stAram = new MappedRAM();
            public static MappedRAM stBrom = new MappedRAM(), stBram = new MappedRAM();
            public static MappedRAM gbrom = new MappedRAM(), gbram = new MappedRAM(), gbrtc = new MappedRAM();
        }
        public static Cartridge cartridge = new Cartridge();

        public partial class Cartridge
        {
            public enum Mode
            {
                ModeNormal,
                ModeBsxSlotted,
                ModeBsx,
                ModeSufamiTurbo,
                ModeSuperGameBoy,
            };

            public enum Type
            {
                TypeNormal,
                TypeBsxSlotted,
                TypeBsxBios,
                TypeBsx,
                TypeSufamiTurboBios,
                TypeSufamiTurbo,
                TypeSuperGameBoy1Bios,
                TypeSuperGameBoy2Bios,
                TypeGameBoy,
                TypeUnknown,
            };

            public enum Region
            {
                NTSC,
                PAL,
            };

            public enum MemoryMapper
            {
                LoROM,
                HiROM,
                ExLoROM,
                ExHiROM,
                SuperFXROM,
                SA1ROM,
                SPC7110ROM,
                BSCLoROM,
                BSCHiROM,
                BSXROM,
                STROM,
            };

            public enum DSP1MemoryMapper
            {
                DSP1Unmapped,
                DSP1LoROM1MB,
                DSP1LoROM2MB,
                DSP1HiROM,
            };

            public bool loaded;     //is a base cartridge inserted?
            public uint crc32;  //crc32 of all cartridges (base+slot(s))

            public Mode mode;
            public Type type;
            public Region region;
            public MemoryMapper mapper;
            public DSP1MemoryMapper dsp1_mapper;

            public bool has_bsx_slot;
            public bool has_superfx;
            public bool has_sa1;
            public bool has_srtc;
            public bool has_sdd1;
            public bool has_spc7110;
            public bool has_spc7110rtc;
            public bool has_cx4;
            public bool has_dsp1;
            public bool has_dsp2;
            public bool has_dsp3;
            public bool has_dsp4;
            public bool has_obc1;
            public bool has_st010;
            public bool has_st011;
            public bool has_st018;
            public bool has_msu;

            enum HeaderField
            {
                CartName = 0x00,
                Mapper = 0x15,
                RomType = 0x16,
                RomSize = 0x17,
                RamSize = 0x18,
                CartRegion = 0x19,
                Company = 0x1a,
                Version = 0x1b,
                Complement = 0x1c,  //inverse checksum
                Checksum = 0x1e,
                ResetVector = 0x3c,
            };
            uint ram_size;

            public void load(Mode cartridge_mode)
            {
                mode = cartridge_mode;
                read_header(memory.cartrom.data(), memory.cartrom.size());

                if (ram_size > 0)
                {
                    memory.cartram.map(allocate<byte>((int)ram_size, 0xff), ram_size);
                }

                if (has_srtc || has_spc7110rtc)
                {
                    memory.cartrtc.map(allocate<byte>(20, 0xff), 20);
                }

                if (mode == Mode.ModeBsx)
                {
                    memory.bsxram.map(allocate<byte>(32 * 1024, 0xff), 32 * 1024);
                    memory.bsxpram.map(allocate<byte>(512 * 1024, 0xff), 512 * 1024);
                }

                if (mode == Mode.ModeSufamiTurbo)
                {
                    if (memory.stArom.data()!=null) memory.stAram.map(allocate<byte>(128 * 1024, 0xff), 128 * 1024);
                    if (memory.stBrom.data() != null) memory.stBram.map(allocate<byte>(128 * 1024, 0xff), 128 * 1024);
                }

                if (mode == Mode.ModeSuperGameBoy)
                {
                    if (memory.gbrom.data() != null)
                    {
                        uint _ram_size = gameboy_ram_size();
                        uint rtc_size = gameboy_rtc_size();

                        if (_ram_size != 0) memory.gbram.map(allocate<byte>((int)_ram_size, 0xff), _ram_size);
                        if (rtc_size != 0) memory.gbrtc.map(allocate<byte>((int)rtc_size, 0x00), rtc_size);
                    }
                }

                memory.cartrom.write_protect(true);
                memory.cartram.write_protect(false);
                memory.cartrtc.write_protect(false);
                memory.bsxflash.write_protect(true);
                memory.bsxram.write_protect(false);
                memory.bsxpram.write_protect(false);
                memory.stArom.write_protect(true);
                memory.stAram.write_protect(false);
                memory.stBrom.write_protect(true);
                memory.stBram.write_protect(false);
                memory.gbrom.write_protect(true);
                memory.gbram.write_protect(false);
                memory.gbrtc.write_protect(false);

                uint checksum = unchecked((uint)~0);
                for (uint n = 0; n < memory.cartrom.size(); n++) checksum = crc32_adjust(checksum, memory.cartrom[n]);
                if (memory.bsxflash.size() != 0 && memory.bsxflash.size() != unchecked((uint)~0))
                    for (uint n = 0; n < memory.bsxflash.size(); n++) checksum = crc32_adjust(checksum, memory.bsxflash[n]);
                if (memory.stArom.size() != 0 && memory.stArom.size() != unchecked((uint)~0))
                    for (uint n = 0; n < memory.stArom.size(); n++) checksum = crc32_adjust(checksum, memory.stArom[n]);
                if (memory.stBrom.size() != 0 && memory.stBrom.size() != unchecked((uint)~0))
                    for (uint n = 0; n < memory.stBrom.size(); n++) checksum = crc32_adjust(checksum, memory.stBrom[n]);
                if (memory.gbrom.size() != 0 && memory.gbrom.size() != unchecked((uint)~0))
                    for (uint n = 0; n < memory.gbrom.size(); n++) checksum = crc32_adjust(checksum, memory.gbrom[n]);
                crc32 = ~checksum;

                bus.load_cart();
                system.serialize_init();
                loaded = true;
            }
            public void unload()
            {
                memory.cartrom.reset();
                memory.cartram.reset();
                memory.cartrtc.reset();
                memory.bsxflash.reset();
                memory.bsxram.reset();
                memory.bsxpram.reset();
                memory.stArom.reset();
                memory.stAram.reset();
                memory.stBrom.reset();
                memory.stBram.reset();
                memory.gbrom.reset();
                memory.gbram.reset();
                memory.gbrtc.reset();

                if (loaded == false) return;
                bus.unload_cart();
                loaded = false;
            }
        }

    }
}