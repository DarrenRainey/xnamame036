using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_digdug
    {

        static _BytePtr digdug_sharedram = new _BytePtr(1);
        static bool interrupt_enable_1, interrupt_enable_2, interrupt_enable_3;

        static int credits;

        static object nmi_timer;

        static void digdig_init_machine()
        {
            credits = 0;
            nmi_timer = null;
            interrupt_enable_1 = interrupt_enable_2 = interrupt_enable_3 = false;
            digdug_halt_w(0, 0);
        }
        static int digdug_sharedram_r(int offset)
        {
            return digdug_sharedram[offset];
        }
        static void digdug_sharedram_w(int offset, int data)
        {
            /* a video ram write */
            if (offset < 0x400)
                Generic.dirtybuffer[offset] = true;

            /* location 9b3d is set to zero just before CPU 2 spins */
            if (offset == 0x1b3d && data == 0 && Mame.cpu_get_pc() == 0x1df1 && Mame.cpu_getactivecpu() == 1)
                Mame.cpu_spinuntil_int();

            digdug_sharedram[offset] = (byte)data;
        }
        static int customio_command;
        static int leftcoinpercred, leftcredpercoin;
        static int rightcoinpercred, rightcredpercoin;
        static byte[] customio = new byte[16];
        static int mode;

        static void digdug_customio_data_w(int offset, int data)
        {
            customio[offset] = (byte)data;

            //Mame.printf("%04x: custom IO offset %02x data %02x\n", Mame.cpu_get_pc(), offset, data);

            switch (customio_command)
            {
                case 0xc1:
                    if (offset == 8)
                    {
                        leftcoinpercred = customio[2] & 0x0f;
                        leftcredpercoin = customio[3] & 0x0f;
                        rightcoinpercred = customio[4] & 0x0f;
                        rightcredpercoin = customio[5] & 0x0f;
                    }
                    break;
            }
        }
        static int leftcoininserted;
        static int rightcoininserted;

        static int digdug_customio_data_r(int offset)
        {
            switch (customio_command)
            {
                case 0x71:
                    if (offset == 0)
                    {
                        if (mode != 0)	/* switch mode */
                        {
                            /* bit 7 is the service switch */
                            return Mame.readinputport(4);
                        }
                        else	/* credits mode: return number of credits in BCD format */
                        {
                            int _in;


                            _in = Mame.readinputport(4);

                            /* check if the user inserted a coin */
                            if (leftcoinpercred > 0)
                            {
                                if ((_in & 0x01) == 0 && credits < 99)
                                {
                                    leftcoininserted++;
                                    if (leftcoininserted >= leftcoinpercred)
                                    {
                                        credits += leftcredpercoin;
                                        leftcoininserted = 0;
                                    }
                                }
                                if ((_in & 0x02) == 0 && credits < 99)
                                {
                                    rightcoininserted++;
                                    if (rightcoininserted >= rightcoinpercred)
                                    {
                                        credits += rightcredpercoin;
                                        rightcoininserted = 0;
                                    }
                                }
                            }
                            else credits = 2;


                            /* check for 1 player start button */
                            if ((_in & 0x10) == 0)
                                if (credits >= 1) credits--;

                            /* check for 2 players start button */
                            if ((_in & 0x20) == 0)
                                if (credits >= 2) credits -= 2;

                            return (credits / 10) * 16 + credits % 10;
                        }
                    }
                    else if (offset == 1)
                    {
                        int p2 = Mame.readinputport(2);

                        if (mode == 0)
                        {
                            /* check directions, according to the following 8-position rule */
                            /*         0          */
                            /*        7 1         */
                            /*       6 8 2        */
                            /*        5 3         */
                            /*         4          */
                            if ((p2 & 0x01) == 0)		/* up */
                                p2 = (p2 & ~0x0f) | 0x00;
                            else if ((p2 & 0x02) == 0)	/* right */
                                p2 = (p2 & ~0x0f) | 0x02;
                            else if ((p2 & 0x04) == 0)	/* down */
                                p2 = (p2 & ~0x0f) | 0x04;
                            else if ((p2 & 0x08) == 0) /* left */
                                p2 = (p2 & ~0x0f) | 0x06;
                            else
                                p2 = (p2 & ~0x0f) | 0x08;
                        }

                        return p2;
                    }
                    else if (offset == 2)
                    {
                        int p2 = Mame.readinputport(3);

                        if (mode == 0)
                        {
                            /* check directions, according to the following 8-position rule */
                            /*         0          */
                            /*        7 1         */
                            /*       6 8 2        */
                            /*        5 3         */
                            /*         4          */
                            if ((p2 & 0x01) == 0)		/* up */
                                p2 = (p2 & ~0x0f) | 0x00;
                            else if ((p2 & 0x02) == 0)	/* right */
                                p2 = (p2 & ~0x0f) | 0x02;
                            else if ((p2 & 0x04) == 0)	/* down */
                                p2 = (p2 & ~0x0f) | 0x04;
                            else if ((p2 & 0x08) == 0) /* left */
                                p2 = (p2 & ~0x0f) | 0x06;
                            else
                                p2 = (p2 & ~0x0f) | 0x08;
                        }

                        return p2; /*p2 jochen*/
                    }
                    break;

                case 0xb1:	/* status? */
                    if (offset <= 2)
                        return 0;
                    break;

                case 0xd2:	/* checking the dipswitches */
                    if (offset == 0)
                        return Mame.readinputport(0);
                    else if (offset == 1)
                        return Mame.readinputport(1);
                    break;
            }

            return -1;
        }
        static int digdug_customio_r(int offset)
        {
            return customio_command;
        }
        static void digdug_nmi_generate(int param)
        {
            Mame.cpu_cause_interrupt(0, Mame.cpu_Z80.Z80_NMI_INT);
        }
        static void digdug_customio_w(int offset, int data)
        {
            //if (data != 0x10 && data != 0x71) Mame.printf("%04x: custom IO command %02x\n", Mame.cpu_get_pc(), data);

            customio_command = data;

            switch (data)
            {
                case 0x10:
                    if (nmi_timer != null) Mame.Timer.timer_remove(nmi_timer);
                    nmi_timer = null;
                    return;

                case 0xa1:	/* go into switch mode */
                    mode = 1;
                    break;

                case 0xc1:
                case 0xe1:	/* go into credit mode */
                    mode = 0;
                    break;

                case 0xb1:	/* status? */
                    credits = 0;	/* this is a good time to reset the credits counter */
                    break;
            }

            nmi_timer = Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_USEC(50), 0, digdug_nmi_generate);
        }
        static void digdug_halt_w(int offset, int data)
        {
            if ((data & 1) != 0)
            {
                Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
                Mame.cpu_set_reset_line(2, Mame.CLEAR_LINE);
            }
            else
            {
                Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
            }
        }
        static void digdug_interrupt_enable_1_w(int offset, int data)
        {
            interrupt_enable_1 = (data & 1) != 0;
        }
        static int digdug_interrupt_1()
        {
            if (interrupt_enable_1) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void digdug_interrupt_enable_2_w(int offset, int data)
        {
            interrupt_enable_2 = (data & 1) != 0;
        }
        static int digdug_interrupt_2()
        {
            if (interrupt_enable_2) return Mame.interrupt();
            else return Mame.ignore_interrupt();
        }
        static void digdug_interrupt_enable_3_w(int offset, int data)
        {
            interrupt_enable_3 = (data & 1) == 0;
        }
        static int digdug_interrupt_3()
        {
            if (interrupt_enable_3) return Mame.nmi_interrupt();
            else return Mame.ignore_interrupt();
        }

    }
}
