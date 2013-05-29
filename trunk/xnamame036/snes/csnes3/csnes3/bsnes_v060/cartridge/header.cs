using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public partial class Cartridge
        {
            uint find_header(byte[] data, uint size)
            {
                uint score_lo = score_header(data, size, 0x007fc0);
                uint score_hi = score_header(data, size, 0x00ffc0);
                uint score_ex = score_header(data, size, 0x40ffc0);
                if (score_ex != 0) score_ex += 4;  //favor ExHiROM on images > 32mbits

                if (score_lo >= score_hi && score_lo >= score_ex)
                {
                    return 0x007fc0;
                }
                else if (score_hi >= score_ex)
                {
                    return 0x00ffc0;
                }
                else
                {
                    return 0x40ffc0;
                }
            }
            uint score_header(byte[] data, uint size, uint addr)
            {
                if (size < addr + 64) return 0;  //image too small to contain header at this location?
                int score = 0;

                ushort resetvector = (ushort)(data[addr + (int)HeaderField.ResetVector] | (data[addr + (int)HeaderField.ResetVector + 1] << 8));
                ushort checksum = (ushort)(data[addr + (int)HeaderField.Checksum] | (data[addr + (int)HeaderField.Checksum + 1] << 8));
                ushort complement = (ushort)(data[addr + (int)HeaderField.Complement] | (data[addr + (int)HeaderField.Complement + 1] << 8));

                byte resetop = data[(addr & ~0x7fff) | (resetvector & 0x7fff)];  //first opcode executed upon reset
                byte mapper = (byte)(data[addr + (int)HeaderField.Mapper] & ~0x10);                      //mask off irrelevent FastROM-capable bit

                //$00:[000-7fff] contains uninitialized RAM and MMIO.
                //reset vector must point to ROM at $00:[8000-ffff] to be considered valid.
                if (resetvector < 0x8000) return 0;

                //some images duplicate the header in multiple locations, and others have completely
                //invalid header information that cannot be relied upon.
                //below code will analyze the first opcode executed at the specified reset vector to
                //determine the probability that this is the correct header.

                //most likely opcodes
                if (resetop == 0x78  //sei
                || resetop == 0x18  //clc (clc; xce)
                || resetop == 0x38  //sec (sec; xce)
                || resetop == 0x9c  //stz $nnnn (stz $4200)
                || resetop == 0x4c  //jmp $nnnn
                || resetop == 0x5c  //jml $nnnnnn
                ) score += 8;

                //plausible opcodes
                if (resetop == 0xc2  //rep #$nn
                || resetop == 0xe2  //sep #$nn
                || resetop == 0xad  //lda $nnnn
                || resetop == 0xae  //ldx $nnnn
                || resetop == 0xac  //ldy $nnnn
                || resetop == 0xaf  //lda $nnnnnn
                || resetop == 0xa9  //lda #$nn
                || resetop == 0xa2  //ldx #$nn
                || resetop == 0xa0  //ldy #$nn
                || resetop == 0x20  //jsr $nnnn
                || resetop == 0x22  //jsl $nnnnnn
                ) score += 4;

                //implausible opcodes
                if (resetop == 0x40  //rti
                || resetop == 0x60  //rts
                || resetop == 0x6b  //rtl
                || resetop == 0xcd  //cmp $nnnn
                || resetop == 0xec  //cpx $nnnn
                || resetop == 0xcc  //cpy $nnnn
                ) score -= 4;

                //least likely opcodes
                if (resetop == 0x00  //brk #$nn
                || resetop == 0x02  //cop #$nn
                || resetop == 0xdb  //stp
                || resetop == 0x42  //wdm
                || resetop == 0xff  //sbc $nnnnnn,x
                ) score -= 8;

                //at times, both the header and reset vector's first opcode will match ...
                //fallback and rely on info validity in these cases to determine more likely header.

                //a valid checksum is the biggest indicator of a valid header.
                if ((checksum + complement) == 0xffff && (checksum != 0) && (complement != 0)) score += 4;

                if (addr == 0x007fc0 && mapper == 0x20) score += 2;  //0x20 is usually LoROM
                if (addr == 0x00ffc0 && mapper == 0x21) score += 2;  //0x21 is usually HiROM
                if (addr == 0x007fc0 && mapper == 0x22) score += 2;  //0x22 is usually ExLoROM
                if (addr == 0x40ffc0 && mapper == 0x25) score += 2;  //0x25 is usually ExHiROM

                if (data[addr + (int)HeaderField.Company] == 0x33) score += 2;        //0x33 indicates extended header
                if (data[addr + (int)HeaderField.RomType] < 0x08) score++;
                if (data[addr + (int)HeaderField.RomSize] < 0x10) score++;
                if (data[addr + (int)HeaderField.RamSize] < 0x08) score++;
                if (data[addr + (int)HeaderField.CartRegion] < 14) score++;

                if (score < 0) score = 0;
                return (uint)score;
            }
            void read_header(byte[] data, uint size)
            {
                type = Type.TypeUnknown;
                mapper = MemoryMapper.LoROM;
                dsp1_mapper = DSP1MemoryMapper.DSP1Unmapped;
                region = Region.NTSC;
                ram_size = 0;

                has_bsx_slot = false;
                has_superfx = false;
                has_sa1 = false;
                has_srtc = false;
                has_sdd1 = false;
                has_spc7110 = false;
                has_spc7110rtc = false;
                has_cx4 = false;
                has_dsp1 = false;
                has_dsp2 = false;
                has_dsp3 = false;
                has_dsp4 = false;
                has_obc1 = false;
                has_st010 = false;
                has_st011 = false;
                has_st018 = false;

                //=====================
                //detect Game Boy carts
                //=====================

                if (size >= 0x0140)
                {
                    if (data[0x0104] == 0xce && data[0x0105] == 0xed && data[0x0106] == 0x66 && data[0x0107] == 0x66
                    && data[0x0108] == 0xcc && data[0x0109] == 0x0d && data[0x010a] == 0x00 && data[0x010b] == 0x0b)
                    {
                        type = Type.TypeGameBoy;
                        return;
                    }
                }

                uint index = find_header(data, size);
                byte mapperid = data[index + (int)HeaderField.Mapper];
                byte rom_type = data[index + (int)HeaderField.RomType];
                byte rom_size = data[index + (int)HeaderField.RomSize];
                byte company = data[index + (int)HeaderField.Company];
                byte regionid = (byte)(data[index + (int)HeaderField.CartRegion] & 0x7f);

                ram_size = 1024u << (data[index + (int)HeaderField.RamSize] & 7);
                if (ram_size == 1024) ram_size = 0;  //no RAM present

                //0, 1, 13 = NTSC; 2 - 12 = PAL
                region = (regionid <= 1 || regionid >= 13) ? Region.NTSC : Region.PAL;

                //=======================
                //detect BS-X flash carts
                //=======================

                if (data[index + 0x13] == 0x00 || data[index + 0x13] == 0xff)
                {
                    if (data[index + 0x14] == 0x00)
                    {
                        byte n15 = data[index + 0x15];
                        if (n15 == 0x00 || n15 == 0x80 || n15 == 0x84 || n15 == 0x9c || n15 == 0xbc || n15 == 0xfc)
                        {
                            if (data[index + 0x1a] == 0x33 || data[index + 0x1a] == 0xff)
                            {
                                type = Type.TypeBsx;
                                mapper = MemoryMapper.BSXROM;
                                region = Region.NTSC;  //BS-X only released in Japan
                                return;
                            }
                        }
                    }
                }

                //=========================
                //detect Sufami Turbo carts
                //=========================

                if (memcmp(data, 0, "BANDAI SFC-ADX", 14) == 0)
                {
                    if (memcmp(data, 16, "SFC-ADX BACKUP", 14) == 0)
                    {
                        type = Type.TypeSufamiTurboBios;
                    }
                    else
                    {
                        type = Type.TypeSufamiTurbo;
                    }
                    mapper = MemoryMapper.STROM;
                    region = Region.NTSC;  //Sufami Turbo only released in Japan
                    return;         //RAM size handled outside this routine
                }

                //==========================
                //detect Super Game Boy BIOS
                //==========================

                if (memcmp(data, (int)index, "Super GAMEBOY2", 14) == 0)
                {
                    type = Type.TypeSuperGameBoy2Bios;
                    return;
                }

                if (memcmp(data, (int)index, "Super GAMEBOY", 13) == 0)
                {
                    type = Type.TypeSuperGameBoy1Bios;
                    return;
                }

                //=====================
                //detect standard carts
                //=====================

                //detect presence of BS-X flash cartridge connector (reads extended header information)
                if (data[index - 14] == 'Z')
                {
                    if (data[index - 11] == 'J')
                    {
                        byte n13 = data[index - 13];
                        if ((n13 >= 'A' && n13 <= 'Z') || (n13 >= '0' && n13 <= '9'))
                        {
                            if (company == 0x33 || (data[index - 10] == 0x00 && data[index - 4] == 0x00))
                            {
                                has_bsx_slot = true;
                            }
                        }
                    }
                }

                if (has_bsx_slot)
                {
                    if (memcmp(data ,(int)index, "Satellaview BS-X     ", 21)==0)
                    {
                        //BS-X base cart
                        type = Type.TypeBsxBios;
                        mapper = MemoryMapper.BSXROM;
                        region = Region.NTSC;  //BS-X only released in Japan
                        return;         //RAM size handled internally by load_cart_bsx() -> BSXCart class
                    }
                    else
                    {
                        type = Type.TypeBsxSlotted;
                        mapper = (index == 0x7fc0 ? MemoryMapper.BSCLoROM : MemoryMapper.BSCHiROM);
                        region = Region.NTSC;  //BS-X slotted cartridges only released in Japan
                    }
                }
                else
                {
                    //standard cart
                    type = Type.TypeNormal;

                    if (index == 0x7fc0 && size >= 0x401000)
                    {
                        mapper = MemoryMapper.ExLoROM;
                    }
                    else if (index == 0x7fc0 && mapperid == 0x32)
                    {
                        mapper = MemoryMapper.ExLoROM;
                    }
                    else if (index == 0x7fc0)
                    {
                        mapper = MemoryMapper.LoROM;
                    }
                    else if (index == 0xffc0)
                    {
                        mapper = MemoryMapper.HiROM;
                    }
                    else
                    {  //index == 0x40ffc0
                        mapper = MemoryMapper.ExHiROM;
                    }
                }

                if (mapperid == 0x20 && (rom_type == 0x13 || rom_type == 0x14 || rom_type == 0x15 || rom_type == 0x1a))
                {
                    has_superfx = true;
                    mapper = MemoryMapper.SuperFXROM;
                    ram_size = 1024u << (data[index - 3] & 7);
                    if (ram_size == 1024) ram_size = 0;
                }

                if (mapperid == 0x23 && (rom_type == 0x32 || rom_type == 0x34 || rom_type == 0x35))
                {
                    has_sa1 = true;
                    mapper = MemoryMapper.SA1ROM;
                }

                if (mapperid == 0x35 && rom_type == 0x55)
                {
                    has_srtc = true;
                }

                if (mapperid == 0x32 && (rom_type == 0x43 || rom_type == 0x45))
                {
                    has_sdd1 = true;
                }

                if (mapperid == 0x3a && (rom_type == 0xf5 || rom_type == 0xf9))
                {
                    has_spc7110 = true;
                    has_spc7110rtc = (rom_type == 0xf9);
                    mapper = MemoryMapper.SPC7110ROM;
                }

                if (mapperid == 0x20 && rom_type == 0xf3)
                {
                    has_cx4 = true;
                }

                if ((mapperid == 0x20 || mapperid == 0x21) && rom_type == 0x03)
                {
                    has_dsp1 = true;
                }

                if (mapperid == 0x30 && rom_type == 0x05 && company != 0xb2)
                {
                    has_dsp1 = true;
                }

                if (mapperid == 0x31 && (rom_type == 0x03 || rom_type == 0x05))
                {
                    has_dsp1 = true;
                }

                if (has_dsp1 == true)
                {
                    if ((mapperid & 0x2f) == 0x20 && size <= 0x100000)
                    {
                        dsp1_mapper = DSP1MemoryMapper.DSP1LoROM1MB;
                    }
                    else if ((mapperid & 0x2f) == 0x20)
                    {
                        dsp1_mapper = DSP1MemoryMapper.DSP1LoROM2MB;
                    }
                    else if ((mapperid & 0x2f) == 0x21)
                    {
                        dsp1_mapper = DSP1MemoryMapper.DSP1HiROM;
                    }
                }

                if (mapperid == 0x20 && rom_type == 0x05)
                {
                    has_dsp2 = true;
                }

                if (mapperid == 0x30 && rom_type == 0x05 && company == 0xb2)
                {
                    has_dsp3 = true;
                }

                if (mapperid == 0x30 && rom_type == 0x03)
                {
                    has_dsp4 = true;
                }

                if (mapperid == 0x30 && rom_type == 0x25)
                {
                    has_obc1 = true;
                }

                if (mapperid == 0x30 && rom_type == 0xf6 && rom_size >= 10)
                {
                    has_st010 = true;
                }

                if (mapperid == 0x30 && rom_type == 0xf6 && rom_size < 10)
                {
                    has_st011 = true;
                }

                if (mapperid == 0x30 && rom_type == 0xf5)
                {
                    has_st018 = true;
                }
            }
        }
    }
}
