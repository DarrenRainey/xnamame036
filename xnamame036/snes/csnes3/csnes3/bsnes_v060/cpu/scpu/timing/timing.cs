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
            const int
    EventNone = 0,
    EventIrqLockRelease = 1,
    EventAluLockRelease = 2,
    EventDramRefresh = 3,
    EventHdmaInit = 4,
    EventHdmaRun = 5,

    //cycle edge
    EventFlagHdmaInit = 1 << 0,
    EventFlagHdmaRun = 1 << 1;

            uint cycle_edge_state;
            uint dma_counter() { return (status.dma_counter + hcounter()) & 7; }
            public override void last_cycle()
            {
                if (!status.irq_lock)
                {
                    status.nmi_pending |= nmi_test();
                    status.irq_pending |= irq_test();

                    status.interrupt_pending = (status.nmi_pending || status.irq_pending);
                }
            }

            void add_clocks(uint clocks)
            {
                _event.tick(clocks);
                uint ticks = clocks >> 1;
                while (ticks-- != 0)
                {
                    ppuCounter.tick();
                    if ((ppuCounter.hcounter() & 2) != 0)
                    {
                        input.tick();
                        poll_interrupts();
                    }
                }
                scheduler.addclocks_cpu(clocks);
            }
            void timing_power() { }
            void timing_reset()
            {
                _event.reset();

                status.clock_count = 0;
                status.line_clocks = ppuCounter.lineclocks();

                status.irq_lock = false;
                status.alu_lock = false;
                status.dram_refresh_position = (cpu_version == 1 ? 530u : 538u);
                _event.enqueue(status.dram_refresh_position, EventDramRefresh);

                status.nmi_valid = false;
                status.nmi_line = false;
                status.nmi_transition = false;
                status.nmi_pending = false;
                status.nmi_hold = false;

                status.irq_valid = false;
                status.irq_line = false;
                status.irq_transition = false;
                status.irq_pending = false;
                status.irq_hold = false;

                status.reset_pending = true;
                status.interrupt_pending = true;
                status.interrupt_vector = 0xfffc;  //reset vector address

                status.dma_active = false;
                status.dma_counter = 0;
                status.dma_clocks = 0;
                status.dma_pending = false;
                status.hdma_pending = false;
                status.hdma_mode = false;

                cycle_edge_state = 0;
            }
            void cycle_edge()
            {
                while (cycle_edge_state != 0)
                {
                    switch ((cycle_edge_state & -cycle_edge_state))
                    {
                        case EventFlagHdmaInit:
                            {
                                hdma_init_reset();
                                if (hdma_enabled_channels() != 0)
                                {
                                    status.hdma_pending = true;
                                    status.hdma_mode = false;
                                }
                            } break;

                        case EventFlagHdmaRun:
                            {
                                if (hdma_active_channels() != 0)
                                {
                                    status.hdma_pending = true;
                                    status.hdma_mode = true;
                                }
                            } break;
                    }

                    cycle_edge_state = cycle_edge_state & (cycle_edge_state - 1);
                }

                //H/DMA pending && DMA inactive?
                //.. Run one full CPU cycle
                //.. HDMA pending && HDMA enabled ? DMA sync + HDMA run
                //.. DMA pending && DMA enabled ? DMA sync + DMA run
                //.... HDMA during DMA && HDMA enabled ? DMA sync + HDMA run
                //.. Run one bus CPU cycle
                //.. CPU sync

                if (status.dma_active == true)
                {
                    if (status.hdma_pending)
                    {
                        status.hdma_pending = false;
                        if (hdma_enabled_channels() != 0)
                        {
                            dma_add_clocks(8 - dma_counter());
                            if (status.hdma_mode == false)
                                hdma_init();
                            else
                                hdma_run();
                            if (dma_enabled_channels() == 0)
                            {
                                add_clocks(status.clock_count - (status.dma_clocks % status.clock_count));
                                status.dma_active = false;
                            }
                        }
                    }

                    if (status.dma_pending)
                    {
                        status.dma_pending = false;
                        if (dma_enabled_channels() != 0)
                        {
                            dma_add_clocks(8 - dma_counter());
                            dma_run();
                            add_clocks(status.clock_count - (status.dma_clocks % status.clock_count));
                            status.dma_active = false;
                        }
                    }
                }

                if (status.dma_active == false)
                {
                    if (status.dma_pending || status.hdma_pending)
                    {
                        status.dma_clocks = 0;
                        status.dma_active = true;
                    }
                }
            }
        }
    }
}
