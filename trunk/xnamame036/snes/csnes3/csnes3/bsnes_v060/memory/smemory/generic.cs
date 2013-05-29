using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sBus
        {
            void map_generic()
            {
                switch (cartridge.mapper)
                {
                    case Cartridge.MemoryMapper.LoROM:
                        {
                            map(MapMode.MapLinear, 0x00, 0x7f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x80, 0xff, 0x8000, 0xffff, memory.cartrom);
                            map_generic_sram();
                        } break;

                    case Cartridge.MemoryMapper.HiROM:
                        {
                            map(MapMode.MapShadow, 0x00, 0x3f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x40, 0x7f, 0x0000, 0xffff, memory.cartrom);
                            map(MapMode.MapShadow, 0x80, 0xbf, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0xc0, 0xff, 0x0000, 0xffff, memory.cartrom);
                            map_generic_sram();
                        } break;

                    case Cartridge.MemoryMapper.ExLoROM:
                        {
                            map(MapMode.MapLinear, 0x00, 0x3f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x40, 0x7f, 0x0000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x80, 0xbf, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0xc0, 0xff, 0x0000, 0xffff, memory.cartrom);
                            map_generic_sram();
                        } break;

                    case Cartridge.MemoryMapper.ExHiROM:
                        {
                            map(MapMode.MapShadow, 0x00, 0x3f, 0x8000, 0xffff, memory.cartrom, 0x400000);
                            map(MapMode.MapLinear, 0x40, 0x7f, 0x0000, 0xffff, memory.cartrom, 0x400000);
                            map(MapMode.MapShadow, 0x80, 0xbf, 0x8000, 0xffff, memory.cartrom, 0x000000);
                            map(MapMode.MapLinear, 0xc0, 0xff, 0x0000, 0xffff, memory.cartrom, 0x000000);
                            map_generic_sram();
                        } break;

                    case Cartridge.MemoryMapper.SuperFXROM:
                        {
                            //mapped via SuperFXBus.init();
                        } break;

                    case Cartridge.MemoryMapper.SA1ROM:
                        {
                            //mapped via SA1Bus.init();
                        } break;

                    case Cartridge.MemoryMapper.SPC7110ROM:
                        {
                            map(MapMode.MapDirect, 0x00, 0x00, 0x6000, 0x7fff, spc7110);          //save RAM w/custom logic
                            map(MapMode.MapShadow, 0x00, 0x0f, 0x8000, 0xffff, memory.cartrom);  //program ROM
                            map(MapMode.MapDirect, 0x30, 0x30, 0x6000, 0x7fff, spc7110);          //save RAM w/custom logic
                            map(MapMode.MapDirect, 0x50, 0x50, 0x0000, 0xffff, spc7110);          //decompression MMIO port
                            map(MapMode.MapShadow, 0x80, 0x8f, 0x8000, 0xffff, memory.cartrom);  //program ROM
                            map(MapMode.MapLinear, 0xc0, 0xcf, 0x0000, 0xffff, memory.cartrom);  //program ROM
                            map(MapMode.MapDirect, 0xd0, 0xff, 0x0000, 0xffff, spc7110);          //MMC-controlled data ROM
                        } break;

                    case Cartridge.MemoryMapper.BSXROM:
                        {
                            //full map is dynamically mapped by:
                            //src/chip/bsx/bsx_cart.cpp : BSXCart.update_memory_map();
                            map(MapMode.MapLinear, 0x00, 0x3f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x80, 0xbf, 0x8000, 0xffff, memory.cartrom);
                        } break;

                    case Cartridge.MemoryMapper.BSCLoROM:
                        {
                            map(MapMode.MapLinear, 0x00, 0x1f, 0x8000, 0xffff, memory.cartrom, 0x000000);
                            map(MapMode.MapLinear, 0x20, 0x3f, 0x8000, 0xffff, memory.cartrom, 0x100000);
                            map(MapMode.MapLinear, 0x70, 0x7f, 0x0000, 0x7fff, memory.cartram, 0x000000);
                            map(MapMode.MapLinear, 0x80, 0x9f, 0x8000, 0xffff, memory.cartrom, 0x200000);
                            map(MapMode.MapLinear, 0xa0, 0xbf, 0x8000, 0xffff, memory.cartrom, 0x100000);
                            map(MapMode.MapLinear, 0xc0, 0xef, 0x0000, 0xffff, bsxflash);
                            map(MapMode.MapLinear, 0xf0, 0xff, 0x0000, 0x7fff, memory.cartram, 0x000000);
                        } break;

                    case Cartridge.MemoryMapper.BSCHiROM:
                        {
                            map(MapMode.MapShadow, 0x00, 0x1f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x20, 0x3f, 0x6000, 0x7fff, memory.cartram);
                            map(MapMode.MapShadow, 0x20, 0x3f, 0x8000, 0xffff, bsxflash);
                            map(MapMode.MapLinear, 0x40, 0x5f, 0x0000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x60, 0x7f, 0x0000, 0xffff, bsxflash);
                            map(MapMode.MapShadow, 0x80, 0x9f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0xa0, 0xbf, 0x6000, 0x7fff, memory.cartram);
                            map(MapMode.MapShadow, 0xa0, 0xbf, 0x8000, 0xffff, bsxflash);
                            map(MapMode.MapLinear, 0xc0, 0xdf, 0x0000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0xe0, 0xff, 0x0000, 0xffff, bsxflash);
                        } break;

                    case Cartridge.MemoryMapper.STROM:
                        {
                            map(MapMode.MapLinear, 0x00, 0x1f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0x20, 0x3f, 0x8000, 0xffff, memory.stArom);
                            map(MapMode.MapLinear, 0x40, 0x5f, 0x8000, 0xffff, memory.stBrom);
                            map(MapMode.MapLinear, 0x60, 0x63, 0x8000, 0xffff, memory.stAram);
                            map(MapMode.MapLinear, 0x70, 0x73, 0x8000, 0xffff, memory.stBram);
                            map(MapMode.MapLinear, 0x80, 0x9f, 0x8000, 0xffff, memory.cartrom);
                            map(MapMode.MapLinear, 0xa0, 0xbf, 0x8000, 0xffff, memory.stArom);
                            map(MapMode.MapLinear, 0xc0, 0xdf, 0x8000, 0xffff, memory.stBrom);
                            map(MapMode.MapLinear, 0xe0, 0xe3, 0x8000, 0xffff, memory.stAram);
                            map(MapMode.MapLinear, 0xf0, 0xf3, 0x8000, 0xffff, memory.stBram);
                        } break;
                }
            }
            void map_generic_sram()
            {
                if (memory.cartram.size() == 0 || memory.cartram.size() == unchecked((uint)-1U)) { return; }

                map(MapMode.MapLinear, 0x20, 0x3f, 0x6000, 0x7fff, memory.cartram);
                map(MapMode.MapLinear, 0xa0, 0xbf, 0x6000, 0x7fff, memory.cartram);

                //research shows only games with very large ROM/RAM sizes require MAD-1 memory mapping of RAM
                //otherwise, default to safer, larger RAM address window
                ushort addr_hi = (ushort)((memory.cartrom.size() > 0x200000 || memory.cartram.size() > 32 * 1024) ? 0x7fff : 0xffff);
                map(MapMode.MapLinear, 0x70, 0x7f, 0x0000, addr_hi, memory.cartram);
                if (cartridge.mapper != Cartridge.MemoryMapper.LoROM) return;
                map(MapMode.MapLinear, 0xf0, 0xff, 0x0000, addr_hi, memory.cartram);
            }
        }
    }
}

