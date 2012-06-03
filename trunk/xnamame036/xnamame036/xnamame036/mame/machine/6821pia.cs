using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class _6821pia
    {
        public const int MAX_PIA = 8;
        public const int PIA_DDRA = 0, PIA_CTLA = 1, PIA_DDRB = 2, PIA_CTLB = 3;

        public const int PIA_STANDARD_ORDERING = 0;
        public const int PIA_ALTERNATE_ORDERING = 1;

        public const int PIA_8BIT = 0;
        public const int PIA_16BIT = 2;

        public const int PIA_LOWER = 0;
        public const int PIA_UPPER = 4;
        public const int PIA_AUTOSENSE = 8;

        public const int PIA_16BIT_LOWER = (PIA_16BIT | PIA_LOWER);
        public const int PIA_16BIT_UPPER = (PIA_16BIT | PIA_UPPER);
        public const int PIA_16BIT_AUTO = (PIA_16BIT | PIA_AUTOSENSE);

        public delegate void irqcallfunc(int state);
        public class pia6821_interface
        {
            public pia6821_interface( Mame.mem_read_handler in_a_func,  Mame.mem_read_handler in_b_func,
             Mame.mem_read_handler in_ca1_func, Mame.mem_read_handler  in_cb1_func, Mame.mem_read_handler  in_ca2_func,  Mame.mem_read_handler in_cb2_func,
             Mame.mem_write_handler out_a_func,Mame.mem_write_handler  out_b_func,
             Mame.mem_write_handler out_ca2_func, Mame.mem_write_handler out_cb2_func,
             irqcallfunc irq_a_func, irqcallfunc irq_b_func)
            {
                this.in_a_func = in_a_func;
                this.in_b_func = in_b_func;
                this.in_ca1_func = in_ca1_func;
                this.in_cb1_func = in_cb1_func;
                this.in_ca2_func = in_ca2_func;
                this.in_cb2_func = in_cb2_func;
                this.irq_a_func = irq_a_func;
                this.irq_b_func = irq_b_func;
                this.out_a_func = out_a_func;
                this.out_b_func = out_b_func;
                this.out_ca2_func = out_ca2_func;
                this.out_cb2_func = out_cb2_func;
            }

            public Mame.mem_read_handler in_a_func, in_b_func;
            public Mame.mem_read_handler in_ca1_func, in_cb1_func, in_ca2_func, in_cb2_func;
            public Mame.mem_write_handler out_a_func, out_b_func;
            public Mame.mem_write_handler out_ca2_func, out_cb2_func;
            public irqcallfunc irq_a_func, irq_b_func;
        }
        public class pia6821
        {
            public pia6821_interface intf;
            public byte addr;

            public byte in_a;
            public byte in_ca1;
            public byte in_ca2;
            public byte out_a;
            public byte out_ca2;
            public byte ddr_a;
            public byte ctl_a;
            public byte irq_a1;
            public byte irq_a2;
            public byte irq_a_state;

            public byte in_b;
            public byte in_cb1;
            public byte in_cb2;
            public byte out_b;
            public byte out_cb2;
            public byte ddr_b;
            public byte ctl_b;
            public byte irq_b1;
            public byte irq_b2;
            public byte irq_b_state;
        }
        static pia6821[] pia = new pia6821[MAX_PIA];
        static byte[] swizzle_address = { 0, 2, 1, 3 };

        public static void pia_unconfig()
        {
            for (int i = 0; i < MAX_PIA; i++) pia[i] = new pia6821();
        }
        public static void pia_config(int which, byte adressing, pia6821_interface intf)
        {
            if (which >= MAX_PIA) return;
            pia[which].intf = intf;
            pia[which].addr = adressing;
        }
        const byte PIA_IRQ1 = 0x80;
        const byte PIA_IRQ2 = 0x40;

        static bool OUTPUT_SELECTED(byte c) { return (c & 0x04) != 0; }
        static bool C2_OUTPUT(byte c) { return (c & 0x20) != 0; }
        static bool C2_STROBE_MODE(byte c) { return ((c & 0x10) == 0); }
        static bool STROBE_E_RESET(byte c) { return (c & 0x08) != 0; }
        static bool C2_INPUT(byte c) { return ((c & 0x20) == 0); }
        static bool SET_C2(byte c) { return (c & 0x08) != 0; }
        static bool C1_LOW_TO_HIGH(byte c) { return (c & 0x02) != 0; }
        static bool C1_HIGH_TO_LOW(byte c) { return (c & 0x02) == 0; }
        static bool STROBE_C1_RESET(byte c) { return (c & 0x08) == 0; }
        static bool C2_LOW_TO_HIGH(byte c) { return (c & 0x10) != 0; }
        static bool C2_HIGH_TO_LOW(byte c) { return (c & 0x10) == 0; }

        static bool IRQ1_ENABLED(byte c) { return (c & 0x01) != 0; }
        static bool IRQ1_DISABLED(byte c) { return (c & 0x01) == 0; }
        static bool IRQ2_ENABLED(byte c) { return (c & 0x08) != 0; }
        static bool IRQ2_DISABLED(byte c) { return (c & 0x08) == 0; }

        public static void pia_reset()
{
	/* zap each structure, preserving the interface and swizzle */
	for (int i = 0; i < MAX_PIA; i++)
	{
		pia6821_interface intf = pia[i].intf;
		byte addr = pia[i].addr;

        pia[i] = new pia6821();

		pia[i].intf = intf;
		pia[i].addr = addr;
	}
}

        static int pia_read(int which, int offset)
        {
            pia6821 p = pia[which];
            int val = 0;

            /* adjust offset for 16-bit and ordering */
            if ((p.addr & PIA_16BIT) != 0) offset /= 2;
            offset &= 3;
            if ((p.addr & PIA_ALTERNATE_ORDERING) != 0) offset = swizzle_address[offset];

            switch (offset)
            {
                /******************* port A output/DDR read *******************/
                case PIA_DDRA:

                    /* read output register */
                    if (OUTPUT_SELECTED(p.ctl_a))
                    {
                        /* update the input */
                        if (p.intf.in_a_func != null) p.in_a = (byte)p.intf.in_a_func(0);

                        /* combine input and output values */
                        val = (p.out_a & p.ddr_a) + (p.in_a & ~p.ddr_a);

                        /* IRQ flags implicitly cleared by a read */
                        p.irq_a1 = p.irq_a2 = 0;
                        update_6821_interrupts(p);

                        /* CA2 is configured as output and in read strobe mode */
                        if (C2_OUTPUT(p.ctl_a) && C2_STROBE_MODE(p.ctl_a))
                        {
                            /* this will cause a transition low; call the output function if we're currently high */
                            if (p.out_ca2 != 0)
                                if (p.intf.out_ca2_func != null) p.intf.out_ca2_func(0, 0);
                            p.out_ca2 = 0;

                            /* if the CA2 strobe is cleared by the E, reset it right away */
                            if (STROBE_E_RESET(p.ctl_a))
                            {
                                if (p.intf.out_ca2_func != null) p.intf.out_ca2_func(0, 1);
                                p.out_ca2 = 1;
                            }
                        }

                        //Mame.printf("PIA%d read port A = %02X\n", which, val);
                    }

                    /* read DDR register */
                    else
                    {
                        val = p.ddr_a;
                        //Mame.printf("PIA%d read DDR A = %02X\n", which, val);
                    }
                    break;

                /******************* port B output/DDR read *******************/
                case PIA_DDRB:

                    /* read output register */
                    if (OUTPUT_SELECTED(p.ctl_b))
                    {
                        /* update the input */
                        if (p.intf.in_b_func != null) p.in_b = (byte)p.intf.in_b_func(0);

                        /* combine input and output values */
                        val = (p.out_b & p.ddr_b) + (p.in_b & ~p.ddr_b);

                        /* IRQ flags implicitly cleared by a read */
                        p.irq_b1 = p.irq_b2 = 0;
                        update_6821_interrupts(p);

                        //Mame.printf("PIA%d read port B = %02X\n", which, val);
                    }

                    /* read DDR register */
                    else
                    {
                        val = p.ddr_b;
                        //Mame.printf("PIA%d read DDR B = %02X\n", which, val);
                    }
                    break;

                /******************* port A control read *******************/
                case PIA_CTLA:

                    /* Update CA1 & CA2 if callback exists, these in turn may update IRQ's */
                    if (p.intf.in_ca1_func != null) pia_set_input_ca1(which, p.intf.in_ca1_func(0));
                    if (p.intf.in_ca2_func != null) pia_set_input_ca2(which, p.intf.in_ca2_func(0));

                    /* read control register */
                    val = p.ctl_a;

                    /* set the IRQ flags if we have pending IRQs */
                    if (p.irq_a1 != 0) val |= PIA_IRQ1;
                    if (p.irq_a2 != 0 && C2_INPUT(p.ctl_a)) val |= PIA_IRQ2;

                    //Mame.printf("PIA%d read control A = %02X\n", which, val);
                    break;

                /******************* port B control read *******************/
                case PIA_CTLB:

                    /* Update CB1 & CB2 if callback exists, these in turn may update IRQ's */
                    if (p.intf.in_cb1_func != null) pia_set_input_cb1(which, p.intf.in_cb1_func(0));
                    if (p.intf.in_cb2_func != null) pia_set_input_cb2(which, p.intf.in_cb2_func(0));

                    /* read control register */
                    val = p.ctl_b;

                    /* set the IRQ flags if we have pending IRQs */
                    if (p.irq_b1 != 0) val |= PIA_IRQ1;
                    if (p.irq_b2 != 0 && C2_INPUT(p.ctl_b)) val |= PIA_IRQ2;

                    //Mame.printf("PIA%d read control B = %02X\n", which, val);
                    break;
            }

            /* adjust final output value for 16-bit */
            if ((p.addr & PIA_16BIT) != 0)
            {
                if ((p.addr & PIA_AUTOSENSE) != 0)
                    val = (val << 8) | val;
                else if ((p.addr & PIA_UPPER) != 0)
                    val <<= 8;
            }

            return val;
        }
        static void pia_write(int which, int offset, int data)
        {
            pia6821 p = pia[which];

            /* adjust offset for 16-bit and ordering */
            if ((p.addr & PIA_16BIT) != 0) offset /= 2;
            offset &= 3;
            if ((p.addr & PIA_ALTERNATE_ORDERING) != 0) offset = swizzle_address[offset];

            /* adjust data for 16-bit */
            if ((p.addr & PIA_16BIT) != 0)
            {
                if ((p.addr & PIA_AUTOSENSE) != 0)
                {
                    if ((data & 0x00ff0000) == 0)
                        data &= 0xff;
                    else
                        data = (data >> 8) & 0xff;
                }
                else if ((p.addr & PIA_UPPER) != 0)
                {
                    if ((data & 0xff000000) != 0)
                        return;
                    data = (data >> 8) & 0xff;
                }
                else
                {
                    if ((data & 0x00ff0000) != 0)
                        return;
                    data &= 0xff;
                }
            }

            switch (offset)
            {
                /******************* port A output/DDR write *******************/
                case PIA_DDRA:

                    /* write output register */
                    if (OUTPUT_SELECTED(p.ctl_a))
                    {
                        //Mame.printf("PIA%d port A write = %02X\n", which, data);

                        /* update the output value */
                        p.out_a = (byte)data;/* & p.ddr_a; */	/* NS990130 - don't mask now, DDR could change later */

                        /* send it to the output function */
                        if (p.intf.out_a_func != null && p.ddr_a != 0) p.intf.out_a_func(0, p.out_a & p.ddr_a);	/* NS990130 */
                    }

                    /* write DDR register */
                    else
                    {
                        //Mame.printf("PIA%d DDR A write = %02X\n", which, data);

                        if (p.ddr_a != data)
                        {
                            /* NS990130 - if DDR changed, call the callback again */
                            p.ddr_a = (byte)data;

                            /* send it to the output function */
                            if (p.intf.out_a_func != null && p.ddr_a != 0) p.intf.out_a_func(0, p.out_a & p.ddr_a);
                        }
                    }
                    break;

                /******************* port B output/DDR write *******************/
                case PIA_DDRB:

                    /* write output register */
                    if (OUTPUT_SELECTED(p.ctl_b))
                    {
                        //Mame.printf("PIA%d port B write = %02X\n", which, data);

                        /* update the output value */
                        p.out_b = (byte)data;/* & p.ddr_b */	/* NS990130 - don't mask now, DDR could change later */

                        /* send it to the output function */
                        if (p.intf.out_b_func != null && p.ddr_b != 0) p.intf.out_b_func(0, p.out_b & p.ddr_b);	/* NS990130 */

                        /* CB2 is configured as output and in write strobe mode */
                        if (C2_OUTPUT(p.ctl_b) && C2_STROBE_MODE(p.ctl_b))
                        {
                            /* this will cause a transition low; call the output function if we're currently high */
                            if (p.out_cb2 != 0)
                                if (p.intf.out_cb2_func != null) p.intf.out_cb2_func(0, 0);
                            p.out_cb2 = 0;

                            /* if the CB2 strobe is cleared by the E, reset it right away */
                            if (STROBE_E_RESET(p.ctl_b))
                            {
                                if (p.intf.out_cb2_func != null) p.intf.out_cb2_func(0, 1);
                                p.out_cb2 = 1;
                            }
                        }
                    }

                    /* write DDR register */
                    else
                    {
                        //Mame.printf("PIA%d DDR B write = %02X\n", which, data);

                        if (p.ddr_b != data)
                        {
                            /* NS990130 - if DDR changed, call the callback again */
                            p.ddr_b = (byte)data;

                            /* send it to the output function */
                            if (p.intf.out_b_func != null && p.ddr_b != 0) p.intf.out_b_func(0, p.out_b & p.ddr_b);
                        }
                    }
                    break;

                /******************* port A control write *******************/
                case PIA_CTLA:

                    /* Bit 7 and 6 read only - PD 16/01/00 */

                    data &= 0x3f;


                    //Mame.printf("PIA%d control A write = %02X\n", which, data);

                    /* CA2 is configured as output and in set/reset mode */
                    /* 10/22/98 - MAB/FMP - any C2_OUTPUT should affect CA2 */
                    //			if (C2_OUTPUT(data) && C2_SET_MODE(data))
                    if (C2_OUTPUT((byte)data))
                    {
                        /* determine the new value */
                        int temp = SET_C2((byte)data) ? 1 : 0;

                        /* if this creates a transition, call the CA2 output function */
                        if ((p.out_ca2 ^ temp) != 0)
                            if (p.intf.out_ca2_func != null) p.intf.out_ca2_func(0, temp);

                        /* set the new value */
                        p.out_ca2 = (byte)temp;
                    }

                    /* update the control register */
                    p.ctl_a = (byte)data;

                    /* update externals */
                    update_6821_interrupts(p);
                    break;

                /******************* port B control write *******************/
                case PIA_CTLB:

                    /* Bit 7 and 6 read only - PD 16/01/00 */

                    data &= 0x3f;

                    //Mame.printf("PIA%d control B write = %02X\n", which, data);

                    /* CB2 is configured as output and in set/reset mode */
                    /* 10/22/98 - MAB/FMP - any C2_OUTPUT should affect CB2 */
                    //			if (C2_OUTPUT(data) && C2_SET_MODE(data))
                    if (C2_OUTPUT((byte)data))
                    {
                        /* determine the new value */
                        int temp = SET_C2((byte)data) ? 1 : 0;

                        /* if this creates a transition, call the CA2 output function */
                        if ((p.out_cb2 ^ temp) != 0)
                            if (p.intf.out_cb2_func != null) p.intf.out_cb2_func(0, temp);

                        /* set the new value */
                        p.out_cb2 = (byte)temp;
                    }

                    /* update the control register */
                    p.ctl_b = (byte)data;

                    /* update externals */
                    update_6821_interrupts(p);
                    break;
            }
        }
        static void pia_set_input_a(int which, int data)
        {
            pia[which].in_a = (byte)data;

        }
        static void pia_set_input_b(int which, int data)
        {
            pia[which].in_b = (byte)data;

        }
        static void pia_set_input_ca1(int which, int data)
        {
            pia6821 p = pia[which];

            /* limit the data to 0 or 1 */
            data = data != 0 ? 1 : 0;

            /* the new state has caused a transition */
            if ((p.in_ca1 ^ data) != 0)
            {
                /* handle the active transition */
                if ((data != 0 && C1_LOW_TO_HIGH(p.ctl_a)) || (data == 0 && C1_HIGH_TO_LOW(p.ctl_a)))
                {
                    /* mark the IRQ */
                    p.irq_a1 = 1;

                    /* update externals */
                    update_6821_interrupts(p);

                    /* CA2 is configured as output and in read strobe mode and cleared by a CA1 transition */
                    if (C2_OUTPUT(p.ctl_a) && C2_STROBE_MODE(p.ctl_a) && STROBE_C1_RESET(p.ctl_a))
                    {
                        /* call the CA2 output function */
                        if (p.out_ca2 == 0)
                            if (p.intf.out_ca2_func != null) p.intf.out_ca2_func(0, 1);

                        /* clear CA2 */
                        p.out_ca2 = 1;
                    }
                }
            }

            /* set the new value for CA1 */
            p.in_ca1 = (byte)data;
        }
        static void pia_set_input_ca2(int which, int data)
        {
            pia6821 p = pia[which];

            /* limit the data to 0 or 1 */
            data = data != 0 ? 1 : 0;

            /* CA2 is in input mode */
            if (C2_INPUT(p.ctl_a))
            {
                /* the new state has caused a transition */
                if ((p.in_ca2 ^ data) != 0)
                {
                    /* handle the active transition */
                    if ((data != 0 && C2_LOW_TO_HIGH(p.ctl_a)) || (data == 0 && C2_HIGH_TO_LOW(p.ctl_a)))
                    {
                        /* mark the IRQ */
                        p.irq_a2 = 1;

                        /* update externals */
                        update_6821_interrupts(p);
                    }
                }
            }

            /* set the new value for CA2 */
            p.in_ca2 = (byte)data;
        }
        static void pia_set_input_cb1(int which, int data)
        {
            pia6821 p = pia[which];

            /* limit the data to 0 or 1 */
            data = data != 0 ? 1 : 0;

            /* the new state has caused a transition */
            if ((p.in_cb1 ^ data) != 0)
            {
                /* handle the active transition */
                if ((data != 0 && C1_LOW_TO_HIGH(p.ctl_b)) || (data == 0 && C1_HIGH_TO_LOW(p.ctl_b)))
                {
                    /* mark the IRQ */
                    p.irq_b1 = 1;

                    /* update externals */
                    update_6821_interrupts(p);

                    /* CB2 is configured as output and in write strobe mode and cleared by a CA1 transition */
                    if (C2_OUTPUT(p.ctl_b) && C2_STROBE_MODE(p.ctl_b) && STROBE_C1_RESET(p.ctl_b))
                    {
                        /* the IRQ1 flag must have also been cleared */
                        if (p.irq_b1 == 0)
                        {
                            /* call the CB2 output function */
                            if (p.out_cb2 == 0)
                                if (p.intf.out_cb2_func != null) p.intf.out_cb2_func(0, 1);

                            /* clear CB2 */
                            p.out_cb2 = 1;
                        }
                    }
                }
            }

            /* set the new value for CB1 */
            p.in_cb1 = (byte)data;
        }

        static void pia_set_input_cb2(int which, int data)
        {
            pia6821 p = pia[which];

            /* limit the data to 0 or 1 */
            data = data != 0 ? 1 : 0;

            /* CB2 is in input mode */
            if (C2_INPUT(p.ctl_b))
            {
                /* the new state has caused a transition */
                if ((p.in_cb2 ^ data) != 0)
                {
                    /* handle the active transition */
                    if ((data != 0 && C2_LOW_TO_HIGH(p.ctl_b)) || (data == 0 && C2_HIGH_TO_LOW(p.ctl_b)))
                    {
                        /* mark the IRQ */
                        p.irq_b2 = 1;

                        /* update externals */
                        update_6821_interrupts(p);
                    }
                }
            }

            /* set the new value for CA2 */
            p.in_cb2 = (byte)data;
        }








        public static int pia_0_r(int offset) { return pia_read(0, offset); }
        public static int pia_1_r(int offset) { return pia_read(1, offset); }
        public static int pia_2_r(int offset) { return pia_read(2, offset); }
        public static int pia_3_r(int offset) { return pia_read(3, offset); }
        public static int pia_4_r(int offset) { return pia_read(4, offset); }
        public static int pia_5_r(int offset) { return pia_read(5, offset); }
        public static int pia_6_r(int offset) { return pia_read(6, offset); }
        public static int pia_7_r(int offset) { return pia_read(7, offset); }

        public static void pia_0_w(int offset, int data) { pia_write(0, offset, data); }
        public static void pia_1_w(int offset, int data) { pia_write(1, offset, data); }
        public static void pia_2_w(int offset, int data) { pia_write(2, offset, data); }
        public static void pia_3_w(int offset, int data) { pia_write(3, offset, data); }
        public static void pia_4_w(int offset, int data) { pia_write(4, offset, data); }
        public static void pia_5_w(int offset, int data) { pia_write(5, offset, data); }
        public static void pia_6_w(int offset, int data) { pia_write(6, offset, data); }
        public static void pia_7_w(int offset, int data) { pia_write(7, offset, data); }

        public static void pia_0_porta_w(int offset, int data) { pia_set_input_a(0, data); }
        public static void pia_1_porta_w(int offset, int data) { pia_set_input_a(1, data); }
        public static void pia_2_porta_w(int offset, int data) { pia_set_input_a(2, data); }
        public static void pia_3_porta_w(int offset, int data) { pia_set_input_a(3, data); }
        public static void pia_4_porta_w(int offset, int data) { pia_set_input_a(4, data); }
        public static void pia_5_porta_w(int offset, int data) { pia_set_input_a(5, data); }
        public static void pia_6_porta_w(int offset, int data) { pia_set_input_a(6, data); }
        public static void pia_7_porta_w(int offset, int data) { pia_set_input_a(7, data); }
        public static void pia_0_portb_w(int offset, int data) { pia_set_input_b(0, data); }
        public static void pia_1_portb_w(int offset, int data) { pia_set_input_b(1, data); }
        public static void pia_2_portb_w(int offset, int data) { pia_set_input_b(2, data); }
        public static void pia_3_portb_w(int offset, int data) { pia_set_input_b(3, data); }
        public static void pia_4_portb_w(int offset, int data) { pia_set_input_b(4, data); }
        public static void pia_5_portb_w(int offset, int data) { pia_set_input_b(5, data); }
        public static void pia_6_portb_w(int offset, int data) { pia_set_input_b(6, data); }
        public static void pia_7_portb_w(int offset, int data) { pia_set_input_b(7, data); }
        public static int pia_0_porta_r(int offset) { return pia[0].in_a; }
        public static int pia_1_porta_r(int offset) { return pia[1].in_a; }
        public static int pia_2_porta_r(int offset) { return pia[2].in_a; }
        public static int pia_3_porta_r(int offset) { return pia[3].in_a; }
        public static int pia_4_porta_r(int offset) { return pia[4].in_a; }
        public static int pia_5_porta_r(int offset) { return pia[5].in_a; }
        public static int pia_6_porta_r(int offset) { return pia[6].in_a; }
        public static int pia_7_porta_r(int offset) { return pia[7].in_a; }
        public static int pia_0_portb_r(int offset) { return pia[0].in_b; }
        public static int pia_1_portb_r(int offset) { return pia[1].in_b; }
        public static int pia_2_portb_r(int offset) { return pia[2].in_b; }
        public static int pia_3_portb_r(int offset) { return pia[3].in_b; }
        public static int pia_4_portb_r(int offset) { return pia[4].in_b; }
        public static int pia_5_portb_r(int offset) { return pia[5].in_b; }
        public static int pia_6_portb_r(int offset) { return pia[6].in_b; }
        public static int pia_7_portb_r(int offset) { return pia[7].in_b; }
        public static void pia_0_ca1_w(int offset, int data) { pia_set_input_ca1(0, data); }
        public static void pia_1_ca1_w(int offset, int data) { pia_set_input_ca1(1, data); }
        public static void pia_2_ca1_w(int offset, int data) { pia_set_input_ca1(2, data); }
        public static void pia_3_ca1_w(int offset, int data) { pia_set_input_ca1(3, data); }
        public static void pia_4_ca1_w(int offset, int data) { pia_set_input_ca1(4, data); }
        public static void pia_5_ca1_w(int offset, int data) { pia_set_input_ca1(5, data); }
        public static void pia_6_ca1_w(int offset, int data) { pia_set_input_ca1(6, data); }
        public static void pia_7_ca1_w(int offset, int data) { pia_set_input_ca1(7, data); }
        public static void pia_0_ca2_w(int offset, int data) { pia_set_input_ca2(0, data); }
        public static void pia_1_ca2_w(int offset, int data) { pia_set_input_ca2(1, data); }
        public static void pia_2_ca2_w(int offset, int data) { pia_set_input_ca2(2, data); }
        public static void pia_3_ca2_w(int offset, int data) { pia_set_input_ca2(3, data); }
        public static void pia_4_ca2_w(int offset, int data) { pia_set_input_ca2(4, data); }
        public static void pia_5_ca2_w(int offset, int data) { pia_set_input_ca2(5, data); }
        public static void pia_6_ca2_w(int offset, int data) { pia_set_input_ca2(6, data); }
        public static void pia_7_ca2_w(int offset, int data) { pia_set_input_ca2(7, data); }
        public static void pia_0_cb1_w(int offset, int data) { pia_set_input_cb1(0, data); }
        public static void pia_1_cb1_w(int offset, int data) { pia_set_input_cb1(1, data); }
        public static void pia_2_cb1_w(int offset, int data) { pia_set_input_cb1(2, data); }
        public static void pia_3_cb1_w(int offset, int data) { pia_set_input_cb1(3, data); }
        public static void pia_4_cb1_w(int offset, int data) { pia_set_input_cb1(4, data); }
        public static void pia_5_cb1_w(int offset, int data) { pia_set_input_cb1(5, data); }
        public static void pia_6_cb1_w(int offset, int data) { pia_set_input_cb1(6, data); }
        public static void pia_7_cb1_w(int offset, int data) { pia_set_input_cb1(7, data); }
        public static void pia_0_cb2_w(int offset, int data) { pia_set_input_cb2(0, data); }
        public static void pia_1_cb2_w(int offset, int data) { pia_set_input_cb2(1, data); }
        public static void pia_2_cb2_w(int offset, int data) { pia_set_input_cb2(2, data); }
        public static void pia_3_cb2_w(int offset, int data) { pia_set_input_cb2(3, data); }
        public static void pia_4_cb2_w(int offset, int data) { pia_set_input_cb2(4, data); }
        public static void pia_5_cb2_w(int offset, int data) { pia_set_input_cb2(5, data); }
        public static void pia_6_cb2_w(int offset, int data) { pia_set_input_cb2(6, data); }
        public static void pia_7_cb2_w(int offset, int data) { pia_set_input_cb2(7, data); }
        public static int pia_0_ca1_r(int offset) { return pia[0].in_ca1; }
        public static int pia_1_ca1_r(int offset) { return pia[1].in_ca1; }
        public static int pia_2_ca1_r(int offset) { return pia[2].in_ca1; }
        public static int pia_3_ca1_r(int offset) { return pia[3].in_ca1; }
        public static int pia_4_ca1_r(int offset) { return pia[4].in_ca1; }
        public static int pia_5_ca1_r(int offset) { return pia[5].in_ca1; }
        public static int pia_6_ca1_r(int offset) { return pia[6].in_ca1; }
        public static int pia_7_ca1_r(int offset) { return pia[7].in_ca1; }
        public static int pia_0_ca2_r(int offset) { return pia[0].in_ca2; }
        public static int pia_1_ca2_r(int offset) { return pia[1].in_ca2; }
        public static int pia_2_ca2_r(int offset) { return pia[2].in_ca2; }
        public static int pia_3_ca2_r(int offset) { return pia[3].in_ca2; }
        public static int pia_4_ca2_r(int offset) { return pia[4].in_ca2; }
        public static int pia_5_ca2_r(int offset) { return pia[5].in_ca2; }
        public static int pia_6_ca2_r(int offset) { return pia[6].in_ca2; }
        public static int pia_7_ca2_r(int offset) { return pia[7].in_ca2; }
        public static int pia_0_cb1_r(int offset) { return pia[0].in_cb1; }
        public static int pia_1_cb1_r(int offset) { return pia[1].in_cb1; }
        public static int pia_2_cb1_r(int offset) { return pia[2].in_cb1; }
        public static int pia_3_cb1_r(int offset) { return pia[3].in_cb1; }
        public static int pia_4_cb1_r(int offset) { return pia[4].in_cb1; }
        public static int pia_5_cb1_r(int offset) { return pia[5].in_cb1; }
        public static int pia_6_cb1_r(int offset) { return pia[6].in_cb1; }
        public static int pia_7_cb1_r(int offset) { return pia[7].in_cb1; }
        public static int pia_0_cb2_r(int offset) { return pia[0].in_cb2; }
        public static int pia_1_cb2_r(int offset) { return pia[1].in_cb2; }
        public static int pia_2_cb2_r(int offset) { return pia[2].in_cb2; }
        public static int pia_3_cb2_r(int offset) { return pia[3].in_cb2; }
        public static int pia_4_cb2_r(int offset) { return pia[4].in_cb2; }
        public static int pia_5_cb2_r(int offset) { return pia[5].in_cb2; }
        public static int pia_6_cb2_r(int offset) { return pia[6].in_cb2; }
        public static int pia_7_cb2_r(int offset) { return pia[7].in_cb2; }

        static void update_6821_interrupts(pia6821 p)
        {
            int new_state;

            /* start with IRQ A */
            new_state = 0;
            if ((p.irq_a1 != 0 && IRQ1_ENABLED(p.ctl_a)) || (p.irq_a2 != 0 && IRQ2_ENABLED(p.ctl_a))) new_state = 1;
            if (new_state != p.irq_a_state)
            {
                p.irq_a_state = (byte)new_state;
                if (p.intf.irq_a_func != null) update_shared_irq_handler(p.intf.irq_a_func);
            }

            /* then do IRQ B */
            new_state = 0;
            if ((p.irq_b1 != 0 && IRQ1_ENABLED(p.ctl_b)) || (p.irq_b2 != 0 && IRQ2_ENABLED(p.ctl_b))) new_state = 1;
            if (new_state != p.irq_b_state)
            {
                p.irq_b_state = (byte)new_state;
                if (p.intf.irq_b_func != null) update_shared_irq_handler(p.intf.irq_b_func);
            }
        }
        static void update_shared_irq_handler(irqcallfunc irq_func)
        {
            int i;

            /* search all PIAs for this same IRQ function */
            for (i = 0; i < MAX_PIA; i++)
                if (pia[i].intf != null)
                {
                    /* check IRQ A */
                    if (pia[i].intf.irq_a_func == irq_func && pia[i].irq_a_state != 0)
                    {
                        irq_func(1);
                        return;
                    }

                    /* check IRQ B */
                    if (pia[i].intf.irq_b_func == irq_func && pia[i].irq_b_state != 0)
                    {
                        irq_func(1);
                        return;
                    }
                }

            /* if we found nothing, the state is off */
            irq_func(0);
        }

    }
}


