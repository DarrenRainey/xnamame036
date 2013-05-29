using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class sCPU
        {

            byte pio() { return status.pio; }
            bool joylatch() { return status.joypad_strobe_latch; }

            //WMDATA
            byte mmio_r2180()
            {
                byte r = bus.read(0x7e0000 | status.wram_addr);
                status.wram_addr = (status.wram_addr + 1) & 0x01ffff;
                return r;
            }

            //WMDATA
            void mmio_w2180(byte data)
            {
                bus.write(0x7e0000 | status.wram_addr, data);
                status.wram_addr = (status.wram_addr + 1) & 0x01ffff;
            }

            //WMADDL
            void mmio_w2181(byte data)
            {
                status.wram_addr = (status.wram_addr & 0xffff00) | (data);
                status.wram_addr &= 0x01ffff;
            }

            //WMADDM
            void mmio_w2182(byte data)
            {
                status.wram_addr = (uint)((status.wram_addr & 0xff00ff) | (data << 8));
                status.wram_addr &= 0x01ffff;
            }

            //WMADDH
            void mmio_w2183(byte data)
            {
                status.wram_addr = (uint)((status.wram_addr & 0x00ffff) | (data << 16));
                status.wram_addr &= 0x01ffff;
            }

            //JOYSER0
            //bit 0 is shared between JOYSER0 and JOYSER1, therefore
            //strobing $4016.d0 affects both controller port latches.
            //$4017 bit 0 writes are ignored.
            void mmio_w4016(byte data)
            {
                status.joypad_strobe_latch = (data & 1) != 0;

                if (status.joypad_strobe_latch == true)
                {
                    input.poll();
                }
            }

            //JOYSER0
            //7-2 = MDR
            //1-0 = Joypad serial data
            //
            //TODO: test whether strobe latch of zero returns
            //realtime or buffered status of joypadN.b
            byte mmio_r4016()
            {
                byte r = (byte)(regs.mdr & 0xfc);
                r |= (byte)(input.port_read(false) & 3);
                return r;
            }

            //JOYSER1
            //7-5 = MDR
            //4-2 = Always 1 (pins are connected to GND)
            //1-0 = Joypad serial data
            byte mmio_r4017()
            {
                byte r = (byte)((regs.mdr & 0xe0) | 0x1c);
                r |= (byte)(input.port_read(true) & 3);
                return r;
            }

            //NMITIMEN
            void mmio_w4200(byte data)
            {
                status.auto_joypad_poll = (data & 0x01) != 0;
                nmitimen_update(data);
            }

            //WRIO
            void mmio_w4201(byte data)
            {
                if ((status.pio & 0x80) != 0 && (data & 0x80) == 0)
                {
                    ppu.latch_counters();
                }
                status.pio = data;
            }

            //WRMPYA
            void mmio_w4202(byte data)
            {
                status.mul_a = data;
            }

            //WRMPYB
            void mmio_w4203(byte data)
            {
                status.mul_b = data;
                status.r4216 = (ushort)(status.mul_a * status.mul_b);

                status.alu_lock = true;
                _event.enqueue(config.cpu.alu_mul_delay, EventAluLockRelease);
            }

            //WRDIVL
            void mmio_w4204(byte data)
            {
                status.div_a = (ushort)((status.div_a & 0xff00) | (data));
            }

            //WRDIVH
            void mmio_w4205(byte data)
            {
                status.div_a = (ushort)((status.div_a & 0x00ff) | (data << 8));
            }

            //WRDIVB
            void mmio_w4206(byte data)
            {
                status.div_b = data;
                status.r4214 = (ushort)((status.div_b) != 0 ? status.div_a / status.div_b : 0xffff);
                status.r4216 = (ushort)((status.div_b) != 0 ? status.div_a % status.div_b : status.div_a);

                status.alu_lock = true;
                _event.enqueue(config.cpu.alu_div_delay, EventAluLockRelease);
            }

            //HTIMEL
            void mmio_w4207(byte data)
            {
                status.hirq_pos = (ushort)((status.hirq_pos & ~0xff) | (data));
                status.hirq_pos &= 0x01ff;
            }

            //HTIMEH
            void mmio_w4208(byte data)
            {
                status.hirq_pos = (ushort)((status.hirq_pos & 0xff) | (data << 8));
                status.hirq_pos &= 0x01ff;
            }

            //VTIMEL
            void mmio_w4209(byte data)
            {
                status.virq_pos = (ushort)((status.virq_pos & ~0xff) | (data));
                status.virq_pos &= 0x01ff;
            }

            //VTIMEH
            void mmio_w420a(byte data)
            {
                status.virq_pos = (ushort)((status.virq_pos & 0xff) | (data << 8));
                status.virq_pos &= 0x01ff;
            }

            //DMAEN
            void mmio_w420b(byte data)
            {
                for (uint i = 0; i < 8; i++)
                {
                    channel[i].dma_enabled = (data & (1u << (int)i)) != 0;
                }
                if (data != 0) status.dma_pending = true;
            }

            //HDMAEN
            void mmio_w420c(byte data)
            {
                for (uint i = 0; i < 8; i++)
                {
                    channel[i].hdma_enabled = (data & (1u << (int)i)) != 0;
                }
            }

            //MEMSEL
            void mmio_w420d(byte data)
            {
                status.rom_speed = ((data & 1) != 0 ? 6u : 8u);
            }

            //RDNMI
            //7   = NMI acknowledge
            //6-4 = MDR
            //3-0 = CPU (5a22) version
            byte mmio_r4210()
            {
                byte r = (byte)(regs.mdr & 0x70);
                r |= (byte)((rdnmi()?1:0) << 7);
                r |= (byte)(cpu_version & 0x0f);
                return r;
            }

            //TIMEUP
            //7   = IRQ acknowledge
            //6-0 = MDR
            byte mmio_r4211()
            {
                byte r = (byte)(regs.mdr & 0x7f);
                r |= (byte)((timeup()?1:0) << 7);
                return r;
            }

            //HVBJOY
            //7   = VBLANK acknowledge
            //6   = HBLANK acknowledge
            //5-1 = MDR
            //0   = JOYPAD acknowledge
            byte mmio_r4212()
            {
                byte r = (byte)(regs.mdr & 0x3e);
                ushort vs = ppu.overscan() == false ? (ushort)225 : (ushort)240;

                //auto joypad polling
                if (vcounter() >= vs && vcounter() <= (vs + 2)) r |= 0x01;

                //hblank
                if (hcounter() <= 2 || hcounter() >= 1096) r |= 0x40;

                //vblank
                if (vcounter() >= vs) r |= 0x80;

                return r;
            }

            //RDIO
            byte mmio_r4213()
            {
                return status.pio;
            }

            //RDDIVL
            byte mmio_r4214()
            {
                if (status.alu_lock) return 0;
                return (byte)status.r4214;
            }

            //RDDIVH
            byte mmio_r4215()
            {
                if (status.alu_lock) return 0;
                return (byte)(status.r4214 >> 8);
            }

            //RDMPYL
            byte mmio_r4216()
            {
                if (status.alu_lock) return 0;
                return (byte)status.r4216;
            }

            //RDMPYH
            byte mmio_r4217()
            {
                if (status.alu_lock) return 0;
                return (byte)(status.r4216 >> 8);
            }

            //TODO: handle reads during joypad polling (v=225-227)
            byte mmio_r4218() { return status.joy1l; } //JOY1L
            byte mmio_r4219() { return status.joy1h; } //JOY1H
            byte mmio_r421a() { return status.joy2l; } //JOY2L
            byte mmio_r421b() { return status.joy2h; } //JOY2H
            byte mmio_r421c() { return status.joy3l; } //JOY3L
            byte mmio_r421d() { return status.joy3h; } //JOY3H
            byte mmio_r421e() { return status.joy4l; } //JOY4L
            byte mmio_r421f() { return status.joy4h; } //JOY4H

            //DMAPx
            byte mmio_r43x0(byte i)
            {
                return channel[i].dmap;
            }

            //BBADx
            byte mmio_r43x1(byte i)
            {
                return channel[i].destaddr;
            }

            //A1TxL
            byte mmio_r43x2(byte i)
            {
                return (byte)channel[i].srcaddr;
            }

            //A1TxH
            byte mmio_r43x3(byte i)
            {
                return (byte)(channel[i].srcaddr >> 8);
            }

            //A1Bx
            byte mmio_r43x4(byte i)
            {
                return channel[i].srcbank;
            }

            //DASxL
            //union { uint16 xfersize; uint16 hdma_iaddr; };
            byte mmio_r43x5(byte i)
            {
                return (byte)channel[i].hdma_iaddr;
            }

            //DASxH
            //union { uint16 xfersize; uint16 hdma_iaddr; };
            byte mmio_r43x6(byte i)
            {
                return (byte)(channel[i].hdma_iaddr >> 8);
            }

            //DASBx
            byte mmio_r43x7(byte i)
            {
                return channel[i].hdma_ibank;
            }

            //A2AxL
            byte mmio_r43x8(byte i)
            {
                return (byte)channel[i].hdma_addr;
            }

            //A2AxH
            byte mmio_r43x9(byte i)
            {
                return (byte)(channel[i].hdma_addr >> 8);
            }

            //NTRLx
            byte mmio_r43xa(byte i)
            {
                return channel[i].hdma_line_counter;
            }

            //???
            byte mmio_r43xb(byte i)
            {
                return channel[i].unknown;
            }

            //DMAPx
            void mmio_w43x0(byte i, byte data)
            {
                channel[i].dmap = data;
                channel[i].direction = ((data & 0x80) != 0);
                channel[i].hdma_indirect = ((data & 0x40) != 0);
                channel[i].reversexfer = ((data & 0x10) != 0);
                channel[i].fixedxfer = ((data & 0x08) != 0);
                channel[i].xfermode = (byte)(data & 7);
            }

            //DDBADx
            void mmio_w43x1(byte i, byte data)
            {
                channel[i].destaddr = data;
            }

            //A1TxL
            void mmio_w43x2(byte i, byte data)
            {
                channel[i].srcaddr = (ushort)((channel[i].srcaddr & 0xff00) | (data));
            }

            //A1TxH
            void mmio_w43x3(byte i, byte data)
            {
                channel[i].srcaddr = (ushort)((channel[i].srcaddr & 0x00ff) | (data << 8));
            }

            //A1Bx
            void mmio_w43x4(byte i, byte data)
            {
                channel[i].srcbank = data;
            }

            //DASxL
            //union { uint16 xfersize; uint16 hdma_iaddr; };
            void mmio_w43x5(byte i, byte data)
            {
                channel[i].hdma_iaddr = (ushort)((channel[i].hdma_iaddr & 0xff00) | (data));
            }

            //DASxH
            //union { uint16 xfersize; uint16 hdma_iaddr; };
            void mmio_w43x6(byte i, byte data)
            {
                channel[i].hdma_iaddr = (ushort)((channel[i].hdma_iaddr & 0x00ff) | (data << 8));
            }

            //DASBx
            void mmio_w43x7(byte i, byte data)
            {
                channel[i].hdma_ibank = data;
            }

            //A2AxL
            void mmio_w43x8(byte i, byte data)
            {
                channel[i].hdma_addr = (ushort)((channel[i].hdma_addr & 0xff00) | (data));
            }

            //A2AxH
            void mmio_w43x9(byte i, byte data)
            {
                channel[i].hdma_addr = (ushort)((channel[i].hdma_addr & 0x00ff) | (data << 8));
            }

            //NTRLx
            void mmio_w43xa(byte i, byte data)
            {
                channel[i].hdma_line_counter = data;
            }

            //???
            void mmio_w43xb(byte i, byte data)
            {
                channel[i].unknown = data;
            }

            void mmio_power()
            {
            }

            void mmio_reset()
            {
                //$2181-$2183
                status.wram_addr = 0x000000;

                //$4016-$4017
                status.joypad_strobe_latch = false;
                status.joypad1_bits = unchecked((uint)~0);
                status.joypad2_bits = unchecked((uint)~0);

                //$4200
                status.nmi_enabled = false;
                status.hirq_enabled = false;
                status.virq_enabled = false;
                status.auto_joypad_poll = false;

                //$4201
                status.pio = 0xff;

                //$4202-$4203
                status.mul_a = 0xff;
                status.mul_b = 0xff;

                //$4204-$4206
                status.div_a = 0xffff;
                status.div_b = 0xff;

                //$4207-$420a
                status.hirq_pos = 0x01ff;
                status.virq_pos = 0x01ff;

                //$420d
                status.rom_speed = 8;

                //$4214-$4217
                status.r4214 = 0x0000;
                status.r4216 = 0x0000;

                //$4218-$421f
                status.joy1l = 0x00;
                status.joy1h = 0x00;
                status.joy2l = 0x00;
                status.joy2h = 0x00;
                status.joy3l = 0x00;
                status.joy3h = 0x00;
                status.joy4l = 0x00;
                status.joy4h = 0x00;
            }
            byte mmio_read(uint addr)
            {
                addr &= 0xffff;

                //APU
                if ((addr & 0xffc0) == 0x2140)
                { //$2140-$217f
                    scheduler.sync_cpusmp();
                    return smp.port_read((byte)(addr & 3));
                }

                //DMA
                if ((addr & 0xff80) == 0x4300)
                { //$4300-$437f
                    byte i = (byte)((addr >> 4) & 7);
                    switch (addr & 0xf)
                    {
                        case 0x0: return mmio_r43x0(i);
                        case 0x1: return mmio_r43x1(i);
                        case 0x2: return mmio_r43x2(i);
                        case 0x3: return mmio_r43x3(i);
                        case 0x4: return mmio_r43x4(i);
                        case 0x5: return mmio_r43x5(i);
                        case 0x6: return mmio_r43x6(i);
                        case 0x7: return mmio_r43x7(i);
                        case 0x8: return mmio_r43x8(i);
                        case 0x9: return mmio_r43x9(i);
                        case 0xa: return mmio_r43xa(i);
                        case 0xb: return mmio_r43xb(i);
                        case 0xc: return regs.mdr; //unmapped
                        case 0xd: return regs.mdr; //unmapped
                        case 0xe: return regs.mdr; //unmapped
                        case 0xf: return mmio_r43xb(i); //mirror of $43xb
                    }
                }

                switch (addr)
                {
                    case 0x2180: return mmio_r2180();
                    case 0x4016: return mmio_r4016();
                    case 0x4017: return mmio_r4017();
                    case 0x4210: return mmio_r4210();
                    case 0x4211: return mmio_r4211();
                    case 0x4212: return mmio_r4212();
                    case 0x4213: return mmio_r4213();
                    case 0x4214: return mmio_r4214();
                    case 0x4215: return mmio_r4215();
                    case 0x4216: return mmio_r4216();
                    case 0x4217: return mmio_r4217();
                    case 0x4218: return mmio_r4218();
                    case 0x4219: return mmio_r4219();
                    case 0x421a: return mmio_r421a();
                    case 0x421b: return mmio_r421b();
                    case 0x421c: return mmio_r421c();
                    case 0x421d: return mmio_r421d();
                    case 0x421e: return mmio_r421e();
                    case 0x421f: return mmio_r421f();
                }

                return regs.mdr;
            }
            void mmio_write(uint addr, byte data)
            {
                addr &= 0xffff;

                //APU
                if ((addr & 0xffc0) == 0x2140)
                { //$2140-$217f
                    scheduler.sync_cpusmp();
                    port_write((byte)(addr & 3), data);
                    return;
                }

                //DMA
                if ((addr & 0xff80) == 0x4300)
                { //$4300-$437f
                    byte i = (byte)((addr >> 4) & 7);
                    switch (addr & 0xf)
                    {
                        case 0x0: mmio_w43x0(i, data); return;
                        case 0x1: mmio_w43x1(i, data); return;
                        case 0x2: mmio_w43x2(i, data); return;
                        case 0x3: mmio_w43x3(i, data); return;
                        case 0x4: mmio_w43x4(i, data); return;
                        case 0x5: mmio_w43x5(i, data); return;
                        case 0x6: mmio_w43x6(i, data); return;
                        case 0x7: mmio_w43x7(i, data); return;
                        case 0x8: mmio_w43x8(i, data); return;
                        case 0x9: mmio_w43x9(i, data); return;
                        case 0xa: mmio_w43xa(i, data); return;
                        case 0xb: mmio_w43xb(i, data); return;
                        case 0xc: return; //unmapped
                        case 0xd: return; //unmapped
                        case 0xe: return; //unmapped
                        case 0xf: mmio_w43xb(i, data); return; //mirror of $43xb
                    }
                }

                switch (addr)
                {
                    case 0x2180: mmio_w2180(data); return;
                    case 0x2181: mmio_w2181(data); return;
                    case 0x2182: mmio_w2182(data); return;
                    case 0x2183: mmio_w2183(data); return;
                    case 0x4016: mmio_w4016(data); return;
                    case 0x4017: return; //unmapped
                    case 0x4200: mmio_w4200(data); return;
                    case 0x4201: mmio_w4201(data); return;
                    case 0x4202: mmio_w4202(data); return;
                    case 0x4203: mmio_w4203(data); return;
                    case 0x4204: mmio_w4204(data); return;
                    case 0x4205: mmio_w4205(data); return;
                    case 0x4206: mmio_w4206(data); return;
                    case 0x4207: mmio_w4207(data); return;
                    case 0x4208: mmio_w4208(data); return;
                    case 0x4209: mmio_w4209(data); return;
                    case 0x420a: mmio_w420a(data); return;
                    case 0x420b: mmio_w420b(data); return;
                    case 0x420c: mmio_w420c(data); return;
                    case 0x420d: mmio_w420d(data); return;
                }
            }
        }
    }
}
