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
            struct Channel
            {
                //$420b
                public bool dma_enabled;

                //$420c
                public bool hdma_enabled;

                //$43x0
                public byte dmap;
                public bool direction;
                public bool hdma_indirect;
                public bool reversexfer;
                public bool fixedxfer;
                public byte xfermode;

                //$43x1
                public byte destaddr;

                //$43x2-$43x3
                public ushort srcaddr;

                //$43x4
                public byte srcbank;

                //$43x5-$43x6
                //public union {
                //public ushort xfersize;
                public ushort hdma_iaddr;
                //};

                //$43x7
                public byte hdma_ibank;

                //$43x8-$43x9
                public ushort hdma_addr;

                //$43xa
                public byte hdma_line_counter;

                //$43xb/$43xf
                public byte unknown;

                //internal variables
                public bool hdma_completed;
                public bool hdma_do_transfer;
            }
            Channel[] channel = new Channel[8];

            void dma_add_clocks(uint clocks)
            {
                status.dma_clocks += clocks;
                add_clocks(clocks);
                scheduler.sync_cpucop();
                scheduler.sync_cpuppu();
            }

            bool dma_addr_valid(uint abus)
            {
                //reads from B-bus or S-CPU registers are invalid
                if ((abus & 0x40ff00) == 0x2100) return false;  //$[00-3f|80-bf]:[2100-21ff]
                if ((abus & 0x40fe00) == 0x4000) return false;  //$[00-3f|80-bf]:[4000-41ff]
                if ((abus & 0x40ffe0) == 0x4200) return false;  //$[00-3f|80-bf]:[4200-421f]
                if ((abus & 0x40ff80) == 0x4300) return false;  //$[00-3f|80-bf]:[4300-437f]
                return true;
            }

            byte dma_read(uint abus)
            {
                if (dma_addr_valid(abus) == false) return 0x00;  //does not return S-CPU MDR
                return bus.read(abus);
            }

            void dma_transfer(bool direction, byte bbus, uint abus)
            {
                if (direction == false)
                {
                    //a.b transfer (to $21xx)
                    if (bbus == 0x80 && ((abus & 0xfe0000) == 0x7e0000 || (abus & 0x40e000) == 0x0000))
                    {
                        //illegal WRAM.WRAM transfer (bus conflict)
                        //read most likely occurs; no write occurs
                        //read is irrelevent, as it cannot be observed by software
                        dma_add_clocks(8);
                    }
                    else
                    {
                        dma_add_clocks(4);
                        byte data = dma_read(abus);
                        dma_add_clocks(4);
                        bus.write(0x2100u | bbus, data);
                    }
                }
                else
                {
                    //b.a transfer (from $21xx)
                    if (bbus == 0x80 && ((abus & 0xfe0000) == 0x7e0000 || (abus & 0x40e000) == 0x0000))
                    {
                        //illegal WRAM.WRAM transfer (bus conflict)
                        //no read occurs; write does occur
                        dma_add_clocks(8);
                        bus.write(abus, 0x00);  //does not write S-CPU MDR
                    }
                    else
                    {
                        dma_add_clocks(4);
                        byte data = bus.read(0x2100u | bbus);
                        dma_add_clocks(4);
                        if (dma_addr_valid(abus) == true)
                        {
                            bus.write(abus, data);
                        }
                    }
                }
            }

            /*****
             * address calculation functions
             *****/

            byte dma_bbus(byte i, byte index)
            {
                switch (channel[i].xfermode)
                {
                    default:
                    case 0: return (channel[i].destaddr);                       //0
                    case 1: return (byte)(channel[i].destaddr + (index & 1));         //0,1
                    case 2: return (channel[i].destaddr);                       //0,0
                    case 3: return (byte)(channel[i].destaddr + ((index >> 1) & 1));  //0,0,1,1
                    case 4: return (byte)(channel[i].destaddr + (index & 3));         //0,1,2,3
                    case 5: return (byte)(channel[i].destaddr + (index & 1));         //0,1,0,1
                    case 6: return (channel[i].destaddr);                       //0,0     [2]
                    case 7: return (byte)(channel[i].destaddr + ((index >> 1) & 1));  //0,0,1,1 [3]
                }
            }

            uint dma_addr(byte i)
            {
                uint r = (uint)(channel[i].srcbank << 16) | (channel[i].srcaddr);

                if (channel[i].fixedxfer == false)
                {
                    if (channel[i].reversexfer == false)
                    {
                        channel[i].srcaddr++;
                    }
                    else
                    {
                        channel[i].srcaddr--;
                    }
                }

                return r;
            }

            uint hdma_addr(byte i)
            {
                return (uint)(channel[i].srcbank << 16) | (channel[i].hdma_addr++);
            }

            uint hdma_iaddr(byte i)
            {
                return (uint)(channel[i].hdma_ibank << 16) | (channel[i].hdma_iaddr++);
            }

            /*****
             * DMA functions
             *****/

            byte dma_enabled_channels()
            {
                byte r = 0;
                for (uint i = 0; i < 8; i++)
                {
                    if (channel[i].dma_enabled) r++;
                }
                return r;
            }

            void dma_run()
            {
                dma_add_clocks(8);
                cycle_edge();

                for (uint i = 0; i < 8; i++)
                {
                    if (channel[i].dma_enabled == false) continue;
                    dma_add_clocks(8);
                    cycle_edge();

                    uint index = 0;
                    do
                    {
                        dma_transfer(channel[i].direction, dma_bbus((byte)i, (byte)index++), dma_addr((byte)i));
                        cycle_edge();
                    } while (channel[i].dma_enabled && --channel[i].hdma_iaddr != 0);

                    channel[i].dma_enabled = false;
                }

                status.irq_lock = true;
                _event.enqueue(2, EventIrqLockRelease);
            }

            /*****
             * HDMA functions
             *****/

            bool hdma_active(byte i)
            {
                return (channel[i].hdma_enabled && !channel[i].hdma_completed);
            }

            bool hdma_active_after(byte i)
            {
                for (uint n = i + 1u; n < 8; n++)
                {
                    if (hdma_active((byte)n) == true) return true;
                }
                return false;
            }

            byte hdma_enabled_channels()
            {
                byte r = 0;
                for (uint i = 0; i < 8; i++)
                {
                    if (channel[i].hdma_enabled) r++;
                }
                return r;
            }

            byte hdma_active_channels()
            {
                byte r = 0;
                for (uint i = 0; i < 8; i++)
                {
                    if (hdma_active((byte)i) == true) r++;
                }
                return r;
            }

            void hdma_update(byte i)
            {
                channel[i].hdma_line_counter = dma_read(hdma_addr(i));
                dma_add_clocks(8);

                channel[i].hdma_completed = (channel[i].hdma_line_counter == 0);
                channel[i].hdma_do_transfer = !channel[i].hdma_completed;

                if (channel[i].hdma_indirect)
                {
                    channel[i].hdma_iaddr = (ushort)(dma_read(hdma_addr(i)) << 8);
                    dma_add_clocks(8);

                    if (!channel[i].hdma_completed || hdma_active_after(i))
                    {
                        channel[i].hdma_iaddr >>= 8;
                        channel[i].hdma_iaddr |= (ushort)(dma_read(hdma_addr(i)) << 8);
                        dma_add_clocks(8);
                    }
                }
            }

            void hdma_run()
            {
                dma_add_clocks(8);

                for (uint i = 0; i < 8; i++)
                {
                    if (hdma_active((byte)i) == false) continue;
                    channel[i].dma_enabled = false;  //HDMA run during DMA will stop DMA mid-transfer

                    if (channel[i].hdma_do_transfer)
                    {
                        uint[] transfer_length = { 1, 2, 2, 4, 4, 4, 2, 4 };
                        uint length = transfer_length[channel[i].xfermode];
                        for (uint index = 0; index < length; index++)
                        {
                            uint addr = !channel[i].hdma_indirect ? hdma_addr((byte)i) : hdma_iaddr((byte)i);
                            dma_transfer(channel[i].direction, dma_bbus((byte)i, (byte)index), addr);
                        }
                    }
                }

                for (uint i = 0; i < 8; i++)
                {
                    if (hdma_active((byte)i) == false) continue;

                    channel[i].hdma_line_counter--;
                    channel[i].hdma_do_transfer = (channel[i].hdma_line_counter & 0x80) != 0;
                    if ((channel[i].hdma_line_counter & 0x7f) == 0)
                    {
                        hdma_update((byte)i);
                    }
                    else
                    {
                        dma_add_clocks(8);
                    }
                }

                status.irq_lock = true;
                _event.enqueue(2, EventIrqLockRelease);
            }

            void hdma_init_reset()
            {
                for (uint i = 0; i < 8; i++)
                {
                    channel[i].hdma_completed = false;
                    channel[i].hdma_do_transfer = false;
                }
            }

            void hdma_init()
            {
                dma_add_clocks(8);

                for (uint i = 0; i < 8; i++)
                {
                    if (!channel[i].hdma_enabled) continue;
                    channel[i].dma_enabled = false;  //HDMA init during DMA will stop DMA mid-transfer

                    channel[i].hdma_addr = channel[i].srcaddr;
                    hdma_update((byte)i);
                }

                status.irq_lock = true;
                _event.enqueue(2, EventIrqLockRelease);
            }

            /*****
             * power / reset functions
             *****/

            void dma_power()
            {
                for (uint i = 0; i < 8; i++)
                {
                    channel[i].dmap = 0xff;
                    channel[i].direction = true;
                    channel[i].hdma_indirect = true;
                    channel[i].reversexfer = true;
                    channel[i].fixedxfer = true;
                    channel[i].xfermode = 7;

                    channel[i].destaddr = 0xff;

                    channel[i].srcaddr = 0xffff;
                    channel[i].srcbank = 0xff;

                    //  channel[i].xfersize          = 0xffff;
                    channel[i].hdma_iaddr = 0xffff;  //union with xfersize
                    channel[i].hdma_ibank = 0xff;

                    channel[i].hdma_addr = 0xffff;
                    channel[i].hdma_line_counter = 0xff;
                    channel[i].unknown = 0xff;
                }
            }

            void dma_reset()
            {
                for (uint i = 0; i < 8; i++)
                {
                    channel[i].dma_enabled = false;
                    channel[i].hdma_enabled = false;

                    channel[i].hdma_completed = false;
                    channel[i].hdma_do_transfer = false;
                }
            }


        }
    }
}
