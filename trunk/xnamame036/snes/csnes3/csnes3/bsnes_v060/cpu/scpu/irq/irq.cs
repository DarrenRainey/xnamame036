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
            bool nmi_test()
            {
                if (!status.nmi_transition) return false;
                status.nmi_transition = false;
                regs.wai = false;
                return true;
            }

            bool irq_test()
            {
                if (!status.irq_transition && !regs.irq) return false;
                status.irq_transition = false;
                regs.wai = false;
                return !regs.p.i;
            }
            void poll_interrupts()
            {
                //NMI hold
                if (status.nmi_hold)
                {
                    status.nmi_hold = false;
                    if (status.nmi_enabled) status.nmi_transition = true;
                }

                //NMI test
                bool nmi_valid = (ppuCounter.vcounter(2) >= (!ppu.overscan() ? 225 : 240));
                if (!status.nmi_valid && nmi_valid)
                {
                    //0->1 edge sensitive transition
                    status.nmi_line = true;
                    status.nmi_hold = true;  //hold /NMI for four cycles
                }
                else if (status.nmi_valid && !nmi_valid)
                {
                    //1->0 edge sensitive transition
                    status.nmi_line = false;
                }
                status.nmi_valid = nmi_valid;

                //IRQ hold
                status.irq_hold = false;
                if (status.irq_line)
                {
                    if (status.virq_enabled || status.hirq_enabled) status.irq_transition = true;
                }

                //IRQ test
                bool irq_valid = (status.virq_enabled || status.hirq_enabled);
                if (irq_valid)
                {
                    if ((status.virq_enabled && ppuCounter.vcounter(10) != (status.virq_pos))
                    || (status.hirq_enabled && ppuCounter.hcounter(10) != (status.hirq_pos + 1) * 4)
                    || (status.virq_pos != 0 && ppuCounter.vcounter(6) == 0)  //IRQs cannot trigger on last dot of field
                    ) irq_valid = false;
                }
                if (!status.irq_valid && irq_valid)
                {
                    //0->1 edge sensitive transition
                    status.irq_line = true;
                    status.irq_hold = true;  //hold /IRQ for four cycles
                }
                status.irq_valid = irq_valid;
            }
            void nmitimen_update(byte data)
            {
                bool nmi_enabled = status.nmi_enabled;
                bool virq_enabled = status.virq_enabled;
                bool hirq_enabled = status.hirq_enabled;
                status.nmi_enabled = (data & 0x80) != 0;
                status.virq_enabled = (data & 0x20) != 0;
                status.hirq_enabled = (data & 0x10) != 0;

                //0->1 edge sensitive transition
                if (!nmi_enabled && status.nmi_enabled && status.nmi_line)
                {
                    status.nmi_transition = true;
                }

                //?->1 level sensitive transition
                if (status.virq_enabled && !status.hirq_enabled && status.irq_line)
                {
                    status.irq_transition = true;
                }

                if (!status.virq_enabled && !status.hirq_enabled)
                {
                    status.irq_line = false;
                    status.irq_transition = false;
                }

                status.irq_lock = true;
                _event.enqueue(2, EventIrqLockRelease);
            }
            bool rdnmi()
            {
                bool result = status.nmi_line;
                if (!status.nmi_hold)
                {
                    status.nmi_line = false;
                }
                return result;
            }
            bool timeup()
            {
                bool result = status.irq_line;
                if (!status.irq_hold)
                {
                    status.irq_line = false;
                    status.irq_transition = false;
                }
                return result;
            }
        }
    }
}
