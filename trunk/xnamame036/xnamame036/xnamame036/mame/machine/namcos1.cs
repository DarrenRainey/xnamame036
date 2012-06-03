using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class namcos1
    {

        const bool NEW_TIMER = false; /* CPU slice optimize with new timer system */

        const int NAMCOS1_MAX_BANK = 0x400;


        const int NAMCOS1_MAX_KEY = 0x100;
        static byte[] key = new byte[NAMCOS1_MAX_KEY];

        static _BytePtr s1ram;

        public static int namcos1_cpu1_banklatch;
        public static int namcos1_reset = 0;


        public static int berabohm_input_counter;



        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) Rev1 (Pacmania & Galaga 88)						   *
        *																			   *
        *******************************************************************************/

        static int key_id;
        static int key_id_query;

        public static int rev1_key_r(int offset)
        {
            //	if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: keychip read %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }


        static ushort divider1, divide_32 = 0;
        static ushort d0;

        public static void rev1_key_w(int offset, int data)
        {
            //Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }

            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x01:
                    divider1 = (ushort)((key[0] << 8) + key[1]);
                    break;
                case 0x03:
                    {
                        ushort v1, v2;
                        uint l = 0;

                        if (divide_32 != 0)
                            l = (uint)d0 << 16;

                        d0 = (ushort)((key[2] << 8) + key[3]);

                        if (divider1 == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            if (divide_32 != 0)
                            {
                                l |= d0;

                                v1 = (ushort)(l / divider1);
                                v2 = (ushort)(l % divider1);
                            }
                            else
                            {
                                v1 = (ushort)(d0 / divider1);
                                v2 = (ushort)(d0 % divider1);
                            }
                        }

                        key[2] = (byte)(v1 >> 8);
                        key[3] = (byte)v1;
                        key[0] = (byte)(v2 >> 8);
                        key[1] = (byte)v2;
                    }
                    break;
                case 0x04:
                    if (key[4] == key_id_query) /* get key number */
                        key[4] = (byte)key_id;

                    if (key[4] == 0x0c)
                        divide_32 = 1;
                    else
                        divide_32 = 0;
                    break;
            }
        }

        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) Rev2 (Dragon Spirit, Blazer, World Court)		   *
        *																			   *
        *******************************************************************************/

        static int rev2_key_r(int offset)
        {
            //Mame.printf("CPU #%d PC %08x: keychip read %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static void rev2_key_w(int offset, int data)
        {
            //Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }
            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x00:
                    if (data == 1)
                    {
                        /* fetch key ID */
                        key[3] = (byte)key_id;
                        return;
                    }
                    break;
                case 0x02:
                    /* $f2 = Dragon Spirit, $b7 = Blazer , $35($d9) = worldcourt */
                    if (key[3] == 0xf2 || key[3] == 0xb7 || key[3] == 0x35)
                    {
                        switch (key[0])
                        {
                            case 0x10: key[0] = 0x05; key[1] = 0x00; key[2] = 0xc6; break;
                            case 0x12: key[0] = 0x09; key[1] = 0x00; key[2] = 0x96; break;
                            case 0x15: key[0] = 0x0a; key[1] = 0x00; key[2] = 0x8f; break;
                            case 0x22: key[0] = 0x14; key[1] = 0x00; key[2] = 0x39; break;
                            case 0x32: key[0] = 0x31; key[1] = 0x00; key[2] = 0x12; break;
                            case 0x3d: key[0] = 0x35; key[1] = 0x00; key[2] = 0x27; break;
                            case 0x54: key[0] = 0x10; key[1] = 0x00; key[2] = 0x03; break;
                            case 0x58: key[0] = 0x49; key[1] = 0x00; key[2] = 0x23; break;
                            case 0x7b: key[0] = 0x48; key[1] = 0x00; key[2] = 0xd4; break;
                            case 0xc7: key[0] = 0xbf; key[1] = 0x00; key[2] = 0xe8; break;
                        }
                        return;
                    }
                    break;
                case 0x03:
                    /* $c2 = Dragon Spirit, $b6 = Blazer */
                    if (key[3] == 0xc2 || key[3] == 0xb6)
                    {
                        key[3] = 0x36;
                        return;
                    }
                    /* $d9 = World court */
                    if (key[3] == 0xd9)
                    {
                        key[3] = 0x35;
                        return;
                    }
                    break;
                case 0x3f:	/* Splatter House */
                    key[0x3f] = 0xb5;
                    key[0x36] = 0xb5;
                    return;
            }
            /* ?? */
            if (key[3] == 0x01)
            {
                if (key[0] == 0x40 && key[1] == 0x04 && key[2] == 0x00)
                {
                    key[1] = 0x00; key[2] = 0x10;
                    return;
                }
            }
        }

        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) for Dangerous Seed								   *
        *																			   *
        *******************************************************************************/

        static int dangseed_key_r(int offset)
        {
            //	if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: keychip read %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static void dangseed_key_w(int offset, int data)
        {
            int i;
            //	Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }

            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x50:
                    for (i = 0; i < 0x50; i++)
                    {
                        key[i] = (byte)((data >> ((i >> 4) & 0x0f)) & 0x0f);
                        key[i] |= (byte)((i & 0x0f) << 4);
                    }
                    break;

                case 0x57:
                    key[3] = (byte)key_id;
                    break;
            }
        }

        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) for Dragon Spirit								   *
        *																			   *
        *******************************************************************************/

        static int dspirit_key_r(int offset)
        {
            //Mame.printf("CPU #%d PC %08x: keychip read %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static ushort divisor1;
        static void dspirit_key_w(int offset, int data)
        {
            //	Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }
            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x00:
                    if (data == 1)
                    {
                        /* fetch key ID */
                        key[3] = (byte)key_id;
                    }
                    else
                        divisor1 = (ushort)data;
                    break;

                case 0x01:
                    if (key[3] == 0x01)
                    { /* division gets resolved on latch to $1 */
                        ushort d, v1, v2;

                        d = (ushort)((key[1] << 8) + key[2]);

                        if (divisor == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            v1 = (ushort)(d / divisor1);
                            v2 = (ushort)(d % divisor1);
                        }

                        key[0] = (byte)(v2 & 0xff);
                        key[1] = (byte)(v1 >> 8);
                        key[2] = (byte)(v1 & 0xff);

                        return;
                    }

                    if (key[3] != 0xf2)
                    { /* if its an invalid mode, clear regs */
                        key[0] = 0;
                        key[1] = 0;
                        key[2] = 0;
                    }
                    break;
                case 0x02:
                    if (key[3] == 0xf2)
                    { /* division gets resolved on latch to $2 */
                        ushort d, v1, v2;

                        d = (ushort)((key[1] << 8) + key[2]);

                        if (divisor1 == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            v1 = (ushort)(d / divisor1);
                            v2 = (ushort)(d % divisor1);
                        }

                        key[0] = (byte)(v2 & 0xff);
                        key[1] = (byte)(v1 >> 8);
                        key[2] = (byte)(v1 & 0xff);

                        return;
                    }

                    if (key[3] != 0x01)
                    { /* if its an invalid mode, clear regs */
                        key[0] = 0;
                        key[1] = 0;
                        key[2] = 0;
                    }
                    break;
                case 0x03:
                    if (key[3] != 0xf2 && key[3] != 0x01) /* if the mode is unknown return the id on $3 */
                        key[3] = (byte)key_id;
                    break;
            }
        }

        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) for Blazer										   *
        *																			   *
        *******************************************************************************/

        static int blazer_key_r(int offset)
        {
            Mame.printf("CPU #%d PC %08x: keychip read %04X=%02x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static ushort divisor;
        static void blazer_key_w(int offset, int data)
        {
            Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }
            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x00:
                    if (data == 1)
                    {
                        /* fetch key ID */
                        key[3] = (byte)key_id;
                    }
                    else
                        divisor = (ushort)data;
                    break;

                case 0x01:
                    if (key[3] != 0xb7)
                    { /* if its an invalid mode, clear regs */
                        key[0] = 0;
                        key[1] = 0;
                        key[2] = 0;
                    }
                    break;

                case 0x02:
                    if (key[3] == 0xb7)
                    { /* division gets resolved on latch to $2 */
                        ushort d, v1, v2;

                        d = (ushort)((key[1] << 8) + key[2]);

                        if (divisor == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            v1 = (ushort)(d / divisor);
                            v2 = (ushort)(d % divisor);
                        }

                        key[0] = (byte)(v2 & 0xff);
                        key[1] = (byte)(v1 >> 8);
                        key[2] = (byte)(v1 & 0xff);

                        return;
                    }

                    /* if its an invalid mode, clear regs */
                    key[0] = 0;
                    key[1] = 0;
                    key[2] = 0;
                    break;
                case 0x03:
                    if (key[3] != 0xb7)
                    { /* if the mode is unknown return the id on $3 */
                        key[3] = (byte)key_id;
                    }
                    break;
            }
        }

        /*******************************************************************************
        *																			   *
        *	Key emulation (CUS136) for World Stadium								   *
        *																			   *
        *******************************************************************************/

        static int ws_key_r(int offset)
        {
            //	if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: keychip read %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,key[offset]);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip read %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset);
                return 0;
            }
            return key[offset];
        }

        static ushort divider;
        static ushort d1;
        static void ws_key_w(int offset, int data)
        {
            //Mame.printf("CPU #%d PC %08x: keychip write %04X=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
            if (offset >= NAMCOS1_MAX_KEY)
            {
                Mame.printf("CPU #%d PC %08x: unmapped keychip write %04x=%04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), offset, data);
                return;
            }

            key[offset] = (byte)data;

            switch (offset)
            {
                case 0x01:
                    divider = (ushort)((key[0] << 8) + key[1]);
                    break;
                case 0x03:
                    {
                        ushort v1, v2;

                        d1 = (ushort)((key[2] << 8) + key[3]);

                        if (divider == 0)
                        {
                            v1 = 0xffff;
                            v2 = 0;
                        }
                        else
                        {
                            v1 = (ushort)(d1 / divider);
                            v2 = (ushort)(d1 % divider);
                        }

                        key[2] = (byte)(v1 >> 8);
                        key[3] = (byte)(v1);
                        key[0] = (byte)(v2 >> 8);
                        key[1] = (byte)(v2);
                    }
                    break;
                case 0x04:
                    key[4] = (byte)key_id;
                    break;
            }
        }

        /*******************************************************************************
        *																			   *
        *	Banking emulation (CUS117)												   *
        *																			   *
        *******************************************************************************/

        static int soundram_r(int offset)
        {
            if (offset < 0x100)
                return Namco.namcos1_wavedata_r(offset);
            if (offset < 0x140)
                return Namco.namcos1_sound_r(offset - 0x100);

            /* shared ram */
            return Namco.namco_wavedata[offset];
        }

        static void soundram_w(int offset, int data)
        {
            if (offset < 0x100)
            {
                Namco.namcos1_wavedata_w(offset, data);
                return;
            }
            if (offset < 0x140)
            {
                Namco.namcos1_sound_w(offset - 0x100, data);
                return;
            }
            /* shared ram */
            Namco.namco_wavedata[offset] = (byte)data;

            //if(offset>=0x1000 && errorlog)
            //	fprintf(errorlog,"CPU #%d PC %04x: write shared ram %04x=%02x\n",Mame.cpu_getactivecpu(),Mame.cpu_get_pc(),offset,data);
        }

        /* ROM handlers */

        static void rom_w(int offset, int data)
        {
            Mame.printf("CPU #%d PC %04x: warning - write %02x to rom address %04x\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc(), data, offset);
        }

        /* error handlers */
        public static int unknown_r(int offset)
        {
            Mame.printf("CPU #%d PC %04x: warning - read from unknown chip\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc());
            return 0;
        }

        public static void unknown_w(int offset, int data)
        {
            Mame.printf("CPU #%d PC %04x: warning - wrote to unknown chip\n", Mame.cpu_getactivecpu(), Mame.cpu_get_pc());
        }

        public class bankhandler
        {
            public Mame.mem_read_handler bank_handler_r;
            public Mame.mem_write_handler bank_handler_w;
            public int bank_offset;
            public _BytePtr bank_pointer;
        } ;

        static bankhandler[] namcos1_bank_element = new bankhandler[NAMCOS1_MAX_BANK];

        /* This is where we store our handlers */
        /* 2 cpus with 8 banks of 8k each	   */
        public static bankhandler[,] namcos1_banks = new bankhandler[2, 8];


        /* Main bankswitching routine */
        static int chip = 0;
        public static void namcos1_bankswitch_w(int offset, int data)
        {

            if ((offset & 1) != 0)
            {
                int bank = (offset >> 9) & 0x07; //0x0f;
                int cpu = Mame.cpu_getactivecpu();
                chip &= 0x0300;
                chip |= (data & 0xff);
                /* copy bank handler */
                namcos1_banks[cpu, bank].bank_handler_r = namcos1_bank_element[chip].bank_handler_r;
                namcos1_banks[cpu, bank].bank_handler_w = namcos1_bank_element[chip].bank_handler_w;
                namcos1_banks[cpu, bank].bank_offset = namcos1_bank_element[chip].bank_offset;
                namcos1_banks[cpu, bank].bank_pointer = namcos1_bank_element[chip].bank_pointer;
                //memcpy( &namcos1_banks[cpu][bank] , &namcos1_bank_element[chip] , sizeof(bankhandler));

                /* unmapped bank warning */
                if (namcos1_banks[cpu, bank].bank_handler_r == unknown_r)
                {
                    Mame.printf("CPU #%d PC %04x:warning unknown chip selected bank %x=$%04x\n", cpu, Mame.cpu_get_pc(), bank, chip);
                }

                /* renew pc base */
                //		change_pc16(cpu_get_pc());
            }
            else
            {
                chip &= 0x00ff;
                chip |= (data & 0xff) << 8;
            }
        }

        /* Sub cpu set start bank port */
        public static void namcos1_subcpu_bank(int offset, int data)
        {
            int oldcpu = Mame.cpu_getactivecpu();

            //Mame.printf("cpu1 bank selected %02x=%02x\n",offset,data);
            namcos1_cpu1_banklatch = (namcos1_cpu1_banklatch & 0x300) | data;
            /* Prepare code for Cpu 1 */
            Mame.cpu_setactivecpu(1);
            namcos1_bankswitch_w(0x0e00, namcos1_cpu1_banklatch >> 8);
            namcos1_bankswitch_w(0x0e01, namcos1_cpu1_banklatch & 0xff);
            /* cpu_set_reset_line(1,PULSE_LINE); */

            Mame.cpu_setactivecpu(oldcpu);
        }


        public static int namcos1_0_banked_area0_r(int offset) { if (namcos1_banks[0, 0].bank_handler_r != null) return namcos1_banks[0, 0].bank_handler_r(offset + namcos1_banks[0, 0].bank_offset); return namcos1_banks[0, 0].bank_pointer[offset]; }
        public static int namcos1_0_banked_area1_r(int offset) { if (namcos1_banks[0, 1].bank_handler_r != null) return namcos1_banks[0, 1].bank_handler_r(offset + namcos1_banks[0, 1].bank_offset); return namcos1_banks[0, 1].bank_pointer[offset]; }
        public static int namcos1_0_banked_area2_r(int offset) { if (namcos1_banks[0, 2].bank_handler_r != null) return namcos1_banks[0, 2].bank_handler_r(offset + namcos1_banks[0, 2].bank_offset); return namcos1_banks[0, 2].bank_pointer[offset]; }
        public static int namcos1_0_banked_area3_r(int offset) { if (namcos1_banks[0, 3].bank_handler_r != null) return namcos1_banks[0, 3].bank_handler_r(offset + namcos1_banks[0, 3].bank_offset); return namcos1_banks[0, 3].bank_pointer[offset]; }
        public static int namcos1_0_banked_area4_r(int offset) { if (namcos1_banks[0, 4].bank_handler_r != null) return namcos1_banks[0, 4].bank_handler_r(offset + namcos1_banks[0, 4].bank_offset); return namcos1_banks[0, 4].bank_pointer[offset]; }
        public static int namcos1_0_banked_area5_r(int offset) { if (namcos1_banks[0, 5].bank_handler_r != null) return namcos1_banks[0, 5].bank_handler_r(offset + namcos1_banks[0, 5].bank_offset); return namcos1_banks[0, 5].bank_pointer[offset]; }
        public static int namcos1_0_banked_area6_r(int offset) { if (namcos1_banks[0, 6].bank_handler_r != null) return namcos1_banks[0, 6].bank_handler_r(offset + namcos1_banks[0, 6].bank_offset); return namcos1_banks[0, 6].bank_pointer[offset]; }
        public static int namcos1_0_banked_area7_r(int offset) { if (namcos1_banks[0, 7].bank_handler_r != null) return namcos1_banks[0, 7].bank_handler_r(offset + namcos1_banks[0, 7].bank_offset); return namcos1_banks[0, 7].bank_pointer[offset]; }
        public static int namcos1_1_banked_area0_r(int offset) { if (namcos1_banks[1, 0].bank_handler_r != null) return namcos1_banks[1, 0].bank_handler_r(offset + namcos1_banks[1, 0].bank_offset); return namcos1_banks[1, 0].bank_pointer[offset]; }
        public static int namcos1_1_banked_area1_r(int offset) { if (namcos1_banks[1, 1].bank_handler_r != null) return namcos1_banks[1, 1].bank_handler_r(offset + namcos1_banks[1, 1].bank_offset); return namcos1_banks[1, 1].bank_pointer[offset]; }
        public static int namcos1_1_banked_area2_r(int offset) { if (namcos1_banks[1, 2].bank_handler_r != null) return namcos1_banks[1, 2].bank_handler_r(offset + namcos1_banks[1, 2].bank_offset); return namcos1_banks[1, 2].bank_pointer[offset]; }
        public static int namcos1_1_banked_area3_r(int offset) { if (namcos1_banks[1, 3].bank_handler_r != null) return namcos1_banks[1, 3].bank_handler_r(offset + namcos1_banks[1, 3].bank_offset); return namcos1_banks[1, 3].bank_pointer[offset]; }
        public static int namcos1_1_banked_area4_r(int offset) { if (namcos1_banks[1, 4].bank_handler_r != null) return namcos1_banks[1, 4].bank_handler_r(offset + namcos1_banks[1, 4].bank_offset); return namcos1_banks[1, 4].bank_pointer[offset]; }
        public static int namcos1_1_banked_area5_r(int offset) { if (namcos1_banks[1, 5].bank_handler_r != null) return namcos1_banks[1, 5].bank_handler_r(offset + namcos1_banks[1, 5].bank_offset); return namcos1_banks[1, 5].bank_pointer[offset]; }
        public static int namcos1_1_banked_area6_r(int offset) { if (namcos1_banks[1, 6].bank_handler_r != null) return namcos1_banks[1, 6].bank_handler_r(offset + namcos1_banks[1, 6].bank_offset); return namcos1_banks[1, 6].bank_pointer[offset]; }
        public static int namcos1_1_banked_area7_r(int offset) { if (namcos1_banks[1, 7].bank_handler_r != null) return namcos1_banks[1, 7].bank_handler_r(offset + namcos1_banks[1, 7].bank_offset); return namcos1_banks[1, 7].bank_pointer[offset]; }


        public static void namcos1_0_banked_area0_w(int offset, int data) { if (namcos1_banks[0, 0].bank_handler_w != null) { namcos1_banks[0, 0].bank_handler_w(offset + namcos1_banks[0, 0].bank_offset, data); return; } namcos1_banks[0, 0].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area1_w(int offset, int data) { if (namcos1_banks[0, 1].bank_handler_w != null) { namcos1_banks[0, 1].bank_handler_w(offset + namcos1_banks[0, 1].bank_offset, data); return; } namcos1_banks[0, 1].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area2_w(int offset, int data) { if (namcos1_banks[0, 2].bank_handler_w != null) { namcos1_banks[0, 2].bank_handler_w(offset + namcos1_banks[0, 2].bank_offset, data); return; } namcos1_banks[0, 2].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area3_w(int offset, int data) { if (namcos1_banks[0, 3].bank_handler_w != null) { namcos1_banks[0, 3].bank_handler_w(offset + namcos1_banks[0, 3].bank_offset, data); return; } namcos1_banks[0, 3].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area4_w(int offset, int data) { if (namcos1_banks[0, 4].bank_handler_w != null) { namcos1_banks[0, 4].bank_handler_w(offset + namcos1_banks[0, 4].bank_offset, data); return; } namcos1_banks[0, 4].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area5_w(int offset, int data) { if (namcos1_banks[0, 5].bank_handler_w != null) { namcos1_banks[0, 5].bank_handler_w(offset + namcos1_banks[0, 5].bank_offset, data); return; } namcos1_banks[0, 5].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area6_w(int offset, int data) { if (namcos1_banks[0, 6].bank_handler_w != null) { namcos1_banks[0, 6].bank_handler_w(offset + namcos1_banks[0, 6].bank_offset, data); return; } namcos1_banks[0, 6].bank_pointer[offset] = (byte)data; }
        public static void namcos1_0_banked_area7_w(int offset, int data) { if (namcos1_banks[0, 7].bank_handler_w != null) { namcos1_banks[0, 7].bank_handler_w(offset + namcos1_banks[0, 7].bank_offset, data); return; } namcos1_banks[0, 7].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area0_w(int offset, int data) { if (namcos1_banks[1, 0].bank_handler_w != null) { namcos1_banks[1, 0].bank_handler_w(offset + namcos1_banks[1, 0].bank_offset, data); return; } namcos1_banks[1, 0].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area1_w(int offset, int data) { if (namcos1_banks[1, 1].bank_handler_w != null) { namcos1_banks[1, 1].bank_handler_w(offset + namcos1_banks[1, 1].bank_offset, data); return; } namcos1_banks[1, 1].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area2_w(int offset, int data) { if (namcos1_banks[1, 2].bank_handler_w != null) { namcos1_banks[1, 2].bank_handler_w(offset + namcos1_banks[1, 2].bank_offset, data); return; } namcos1_banks[1, 2].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area3_w(int offset, int data) { if (namcos1_banks[1, 3].bank_handler_w != null) { namcos1_banks[1, 3].bank_handler_w(offset + namcos1_banks[1, 3].bank_offset, data); return; } namcos1_banks[1, 3].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area4_w(int offset, int data) { if (namcos1_banks[1, 4].bank_handler_w != null) { namcos1_banks[1, 4].bank_handler_w(offset + namcos1_banks[1, 4].bank_offset, data); return; } namcos1_banks[1, 4].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area5_w(int offset, int data) { if (namcos1_banks[1, 5].bank_handler_w != null) { namcos1_banks[1, 5].bank_handler_w(offset + namcos1_banks[1, 5].bank_offset, data); return; } namcos1_banks[1, 5].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area6_w(int offset, int data) { if (namcos1_banks[1, 6].bank_handler_w != null) { namcos1_banks[1, 6].bank_handler_w(offset + namcos1_banks[1, 6].bank_offset, data); return; } namcos1_banks[1, 6].bank_pointer[offset] = (byte)data; }
        public static void namcos1_1_banked_area7_w(int offset, int data) { if (namcos1_banks[1, 7].bank_handler_w != null) { namcos1_banks[1, 7].bank_handler_w(offset + namcos1_banks[1, 7].bank_offset, data); return; } namcos1_banks[1, 7].bank_pointer[offset] = (byte)data; }

        /*******************************************************************************
        *																			   *
        *	63701 MCU emulation (CUS64) 											   *
        *																			   *
        *******************************************************************************/

        public static int mcu_patch_data;

        public static void namcos1_cpu_control_w(int offset, int data)
        {
            //	Mame.printf("reset control pc=%04x %02x\n",Mame.cpu_get_pc(),data);
            if (((data & 1) ^ namcos1_reset) != 0)
            {
                namcos1_reset = data & 1;
                if (namcos1_reset != 0)
                {
                    Mame.cpu_set_reset_line(1, Mame.CLEAR_LINE);
                    Mame.cpu_set_reset_line(2, Mame.CLEAR_LINE);
                    Mame.cpu_set_reset_line(3, Mame.CLEAR_LINE);
                    mcu_patch_data = 0;
                }
                else
                {
                    Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
                    Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
                    Mame.cpu_set_reset_line(3, Mame.ASSERT_LINE);
                }
            }
        }

        /*******************************************************************************
        *																			   *
        *	Sound banking emulation (CUS121)										   *
        *																			   *
        *******************************************************************************/

        public static void namcos1_sound_bankswitch_w(int offset, int data)
        {
            _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU3);
            int bank = (data >> 4) & 0x07;

            Mame.cpu_setbank(1, new _BytePtr(RAM, 0x0c000 + (0x4000 * bank)));
        }

        /*******************************************************************************
        *																			   *
        *	CPU idling spinlock routine 											   *
        *																			   *
        *******************************************************************************/
        static _BytePtr sound_spinlock_ram;
        static int sound_spinlock_pc;

        /* sound cpu */
        static int namcos1_sound_spinlock_r(int offset)
        {
            if (Mame.cpu_get_pc() == sound_spinlock_pc && sound_spinlock_ram[0] == 0)
                Mame.cpu_spinuntil_int();
            return sound_spinlock_ram[0];
        }

        /*******************************************************************************
        *																			   *
        *	MCU banking emulation and patch 										   *
        *																			   *
        *******************************************************************************/

        /* mcu banked rom area select */
        public static void namcos1_mcu_bankswitch_w(int offset, int data)
        {
            int addr;
            /* bit 2-7 : chip select line of ROM chip */
            switch (data & 0xfc)
            {
                case 0xf8: addr = 0x10000; break; /* bit 2 : ROM 0 */
                case 0xf4: addr = 0x30000; break; /* bit 3 : ROM 1 */
                case 0xec: addr = 0x50000; break; /* bit 4 : ROM 2 */
                case 0xdc: addr = 0x70000; break; /* bit 5 : ROM 3 */
                case 0xbc: addr = 0x90000; break; /* bit 6 : ROM 4 */
                case 0x7c: addr = 0xb0000; break; /* bit 7 : ROM 5 */
                default: addr = 0x100000; /* illegal */break;
            }
            /* bit 0-1 : address line A15-A16 */
            addr += (data & 3) * 0x8000;
            if (addr >= Mame.memory_region_length(Mame.REGION_CPU4))
            {
                Mame.printf("unmapped mcu bank selected pc=%04x bank=%02x\n", Mame.cpu_get_pc(), data);
                addr = 0x4000;
            }
            Mame.cpu_setbank(4, new _BytePtr(Mame.memory_region(Mame.REGION_CPU4), addr));
        }

        /* This point is very obscure, but i havent found any better way yet. */
        /* Works with all games so far. 									  */

        /* patch points of memory address */
        /* CPU0/1 bank[17f][1000] */
        /* CPU2   [7000]	  */
        /* CPU3   [c000]	  */

        /* This memory point should be set $A6 by anywhere, but 		*/
        /* I found set $A6 only initialize in MCU						*/
        /* This patch kill write this data by MCU case $A6 to xx(clear) */

        public static void namcos1_mcu_patch_w(int offset, int data)
        {

            //Mame.printf("mcu C000 write pc=%04x data=%02x\n",Mame.cpu_get_pc(),data);
            if (mcu_patch_data == 0xa6) return;
            mcu_patch_data = data;

            Mame.mwh_bank3(offset, data);
        }

        /*******************************************************************************
        *																			   *
        *	Initialization															   *
        *																			   *
        *******************************************************************************/

        static int namcos1_setopbase_0(int pc)
        {
            int bank = (pc >> 13) & 7;
            Mame.OP_RAM = Mame.OP_ROM = new _BytePtr((namcos1_banks[0, bank].bank_pointer), (bank << 13));
            /* memory.c output warning - op-code execute on mapped i/o	*/
            /* but it is necessary to continue cpu_setOPbase16 function */
            /* for update current operationhardware(ophw) code			*/
            return pc;
        }

        static int namcos1_setopbase_1(int pc)
        {
            int bank = (pc >> 13) & 7;
            Mame.OP_RAM = Mame.OP_ROM = new _BytePtr((namcos1_banks[1, bank].bank_pointer), (bank << 13));
            /* memory.c output warning - op-code execute on mapped i/o	*/
            /* but it is necessary to continue cpu_setOPbase16 function */
            /* for update current operationhardware(ophw) code			*/
            return pc;
        }

        static void namcos1_install_bank(int start, int end, Mame.mem_read_handler hr, Mame.mem_write_handler hw, int offset, _BytePtr pointer)
        {
            int i;
            for (i = start; i <= end; i++)
            {
                namcos1_bank_element[i] = new bankhandler();
                namcos1_bank_element[i].bank_handler_r = hr;
                namcos1_bank_element[i].bank_handler_w = hw;
                namcos1_bank_element[i].bank_offset = offset;
                namcos1_bank_element[i].bank_pointer = pointer;
                offset += 0x2000;
                if (pointer != null) pointer.offset += 0x2000;
            }
        }

        static void namcos1_install_rom_bank(int start, int end, int size, int offset)
        {
            _BytePtr BROM = Mame.memory_region(Mame.REGION_USER1);
            int step = size / 0x2000;
            while (start < end)
            {
                namcos1_install_bank(start, start + step - 1, null, rom_w, 0, new _BytePtr(BROM, offset));
                start += step;
            }
        }

        static void namcos1_build_banks(Mame.mem_read_handler key_r, Mame.mem_write_handler key_w)
        {
            int i;

            /* S1 RAM pointer set */
            s1ram = Mame.memory_region(Mame.REGION_USER2);

            /* clear all banks to unknown area */
            for (i = 0; i < NAMCOS1_MAX_BANK; i++)
                namcos1_install_bank(i, i, unknown_r, unknown_w, 0, null);

            /* RAM 6 banks - palette */
            namcos1_install_bank(0x170, 0x172, namcos1_paletteram_r, namcos1_paletteram_w, 0, s1ram);
            /* RAM 6 banks - work ram */
            namcos1_install_bank(0x173, 0x173, null, null, 0, new _BytePtr(s1ram, 0x6000));
            /* RAM 5 banks - videoram */
            namcos1_install_bank(0x178, 0x17b, namcos1_videoram_r, namcos1_videoram_w, 0, null);
            /* key chip bank (rev1_key_w / rev2_key_w ) */
            namcos1_install_bank(0x17c, 0x17c, key_r, key_w, 0, null);
            /* RAM 7 banks - display control, playfields, sprites */
            namcos1_install_bank(0x17e, 0x17e, null, namcos1_videocontrol_w, 0, new _BytePtr(s1ram, 0x8000));
            /* RAM 1 shared ram, PSG device */
            namcos1_install_bank(0x17f, 0x17f, soundram_r, soundram_w, 0, Namco.namco_wavedata);
            /* RAM 3 banks */
            namcos1_install_bank(0x180, 0x183, null, null, 0, new _BytePtr(s1ram, 0xc000));
            /* PRG0 */
            namcos1_install_rom_bank(0x200, 0x23f, 0x20000, 0xe0000);
            /* PRG1 */
            namcos1_install_rom_bank(0x240, 0x27f, 0x20000, 0xc0000);
            /* PRG2 */
            namcos1_install_rom_bank(0x280, 0x2bf, 0x20000, 0xa0000);
            /* PRG3 */
            namcos1_install_rom_bank(0x2c0, 0x2ff, 0x20000, 0x80000);
            /* PRG4 */
            namcos1_install_rom_bank(0x300, 0x33f, 0x20000, 0x60000);
            /* PRG5 */
            namcos1_install_rom_bank(0x340, 0x37f, 0x20000, 0x40000);
            /* PRG6 */
            namcos1_install_rom_bank(0x380, 0x3bf, 0x20000, 0x20000);
            /* PRG7 */
            namcos1_install_rom_bank(0x3c0, 0x3ff, 0x20000, 0x00000);
        }

        void init_namcos1()
        {

            int oldcpu = Mame.cpu_getactivecpu(), i;

            /* Point all of our bankhandlers to the error handlers */
            for (i = 0; i < 8; i++)
            {
                namcos1_banks[0, i].bank_handler_r = unknown_r;
                namcos1_banks[0, i].bank_handler_w = unknown_w;
                namcos1_banks[0, i].bank_offset = 0;
                namcos1_banks[1, i].bank_handler_r = unknown_r;
                namcos1_banks[1, i].bank_handler_w = unknown_w;
                namcos1_banks[1, i].bank_offset = 0;
            }

            /* Prepare code for Cpu 0 */
            Mame.cpu_setactivecpu(0);
            namcos1_bankswitch_w(0x0e00, 0x03); /* bank7 = 0x3ff(PRG7) */
            namcos1_bankswitch_w(0x0e01, 0xff);

            /* Prepare code for Cpu 1 */
            Mame.cpu_setactivecpu(1);
            namcos1_bankswitch_w(0x0e00, 0x03);
            namcos1_bankswitch_w(0x0e01, 0xff);

            namcos1_cpu1_banklatch = 0x03ff;

            /* reset starting Cpu */
            Mame.cpu_setactivecpu(oldcpu);

            /* Point mcu & sound shared RAM to destination */
            {
                _BytePtr RAM = new _BytePtr(Namco.namco_wavedata, 0x1000); /* Ram 1, bank 1, offset 0x1000 */
                Mame.cpu_setbank(2, RAM);
                Mame.cpu_setbank(3, RAM);
            }

            /* In case we had some cpu's suspended, resume them now */
            Mame.cpu_set_reset_line(1, Mame.ASSERT_LINE);
            Mame.cpu_set_reset_line(2, Mame.ASSERT_LINE);
            Mame.cpu_set_reset_line(3, Mame.ASSERT_LINE);

            namcos1_reset = 0;
            /* mcu patch data clear */
            mcu_patch_data = 0;

            berabohm_input_counter = 4;	/* for berabohm pressure sensitive buttons */
        }


        /*******************************************************************************
        *																			   *
        *	driver specific initialize routine										   *
        *																			   *
        *******************************************************************************/
        public class namcos1_slice_timer
        {
            public int sync_cpu;	/* synchronus cpu attribute */
            public int sliceHz;	/* slice cycle				*/
            public int delayHz;	/* delay>=0 : delay cycle	*/
            /* delay<0	: slide cycle	*/
        };

        public class namcos1_specific
        {
            public namcos1_specific(int query, int id, Mame.mem_read_handler r, Mame.mem_write_handler w, namcos1_slice_timer[] timer, int tilemap_use)
            {
                this.key_id_query = query; this.key_id = id;
                this.key_r = r; this.key_w = w;
                this.slice_timer = timer;
                this.tilemap_use = tilemap_use;
            }
            /* keychip */
            public int key_id_query, key_id;
            public Mame.mem_read_handler key_r;
            public Mame.mem_write_handler key_w;
            /* cpu slice timer */
            public namcos1_slice_timer[] slice_timer;
            /* optimize flag , use tilemap for playfield */
            public int tilemap_use;
        };

        public static void namcos1_driver_init(namcos1_specific specific)
        {
            /* keychip id */
            key_id_query = specific.key_id_query;
            key_id = specific.key_id;

            /* tilemap use optimize option */
            namcos1_set_optimize(specific.tilemap_use);

            /* build bank elements */
            namcos1_build_banks(specific.key_r, specific.key_w);

            /* override opcode handling for extended memory bank handler */
            Mame.cpu_setOPbaseoverride(0, namcos1_setopbase_0);
            Mame.cpu_setOPbaseoverride(1, namcos1_setopbase_1);

            /* sound cpu speedup optimize (auto detect) */
            {
                _BytePtr RAM = Mame.memory_region(Mame.REGION_CPU3); /* sound cpu */
                int addr, flag_ptr;

                for (addr = 0xd000; addr < 0xd0ff; addr++)
                {
                    if (RAM[addr + 0] == 0xb6 &&   /* lda xxxx */
                       RAM[addr + 3] == 0x27 &&   /* BEQ addr */
                       RAM[addr + 4] == 0xfb)
                    {
                        flag_ptr = RAM[addr + 1] * 256 + RAM[addr + 2];
                        if (flag_ptr > 0x5140 && flag_ptr < 0x5400)
                        {
                            sound_spinlock_pc = addr + 3;
                            sound_spinlock_ram = Mame.install_mem_read_handler(2, flag_ptr, flag_ptr, namcos1_sound_spinlock_r);
                            //if (errorlog)fprintf(errorlog, "Set sound cpu spinlock : pc=%04x , addr = %04x\n", sound_spinlock_pc, flag_ptr);
                            break;
                        }
                    }
                }
            }
#if NEW_TIMER
	/* all cpu's does not need synchronization to all timers */
	cpu_set_full_synchronize(SYNC_NO_CPU);
	{
		const struct namcos1_slice_timer *slice = specific.slice_timer;
		while(slice.sync_cpu != SYNC_NO_CPU)
		{
			/* start CPU slice timer */
			cpu_start_extend_time_slice(slice.sync_cpu,
				TIME_IN_HZ(slice.delayHz),TIME_IN_HZ(slice.sliceHz) );
			slice++;
		}
	}
#else
            /* compatible with old timer system */
            Mame.Timer.timer_pulse(Mame.Timer.TIME_IN_HZ(60 * 25), 0, null);
#endif
        }

#if NEW_TIMER
/* normaly CPU slice optimize */
/* slice order is 0:2:1:x:0:3:1:x */
static const struct namcos1_slice_timer normal_slice[]={
	{ SYNC_2CPU(0,1),60*20,-60*20*2 },	/* CPU 0,1 20/vblank , slide slice */
	{ SYNC_2CPU(2,3),60*5,-(60*5*2+60*20*4) },	/* CPU 2,3 10/vblank */
	{ SYNC_NO_CPU }
};
#else
        public static namcos1_slice_timer[] normal_slice = { null };
#endif

        /*******************************************************************************
*	Shadowland / Youkai Douchuuki specific									   *
*******************************************************************************/
        void init_shadowld()
        {
            namcos1_specific shadowld_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(shadowld_specific);
        }

        /*******************************************************************************
        *	Dragon Spirit specific													   *
        *******************************************************************************/
        void init_dspirit()
        {
            namcos1_specific dspirit_specific =
          new namcos1_specific(
              0x00, 0x36,						/* key query , key id */
              dspirit_key_r, dspirit_key_w,	/* key handler */
              normal_slice,					/* CPU slice normal */
              1								/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(dspirit_specific);
        }

        /*******************************************************************************
        *	Quester specific														   *
        *******************************************************************************/
        void init_quester()
        {
            namcos1_specific quester_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(quester_specific);
        }

        /*******************************************************************************
        *	Blazer specific 														   *
        *******************************************************************************/
        void init_blazer()
        {
            namcos1_specific blazer_specific =
          new namcos1_specific(
              0x00, 0x13,					/* key query , key id */
              blazer_key_r, blazer_key_w,	/* key handler */
              normal_slice,				/* CPU slice normal */
              1							/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(blazer_specific);
        }

        /*******************************************************************************
        *	Pac-Mania / Pac-Mania (Japan) specific									   *
        *******************************************************************************/
        void init_pacmania()
        {
            namcos1_specific pacmania_specific =
          new namcos1_specific(
              0x4b, 0x12,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(pacmania_specific);
        }

        /*******************************************************************************
        *	Galaga '88 / Galaga '88 (Japan) specific								   *
        *******************************************************************************/
        void init_galaga88()
        {
            namcos1_specific galaga88_specific =
          new namcos1_specific(
              0x2d, 0x31,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(galaga88_specific);
        }

        /*******************************************************************************
        *	World Stadium specific													   *
        *******************************************************************************/
        void init_ws()
        {
            namcos1_specific ws_specific =
          new namcos1_specific(
              0xd3, 0x07,				/* key query , key id */
              ws_key_r, ws_key_w,		/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(ws_specific);
        }

        /*******************************************************************************
        *	Beraboh Man specific													   *
        *******************************************************************************/
        static int clk;
        static int[] counter = new int[4];
        static int berabohm_buttons_r(int offset)
        {
            int res;


            if (offset == 0)
            {
                if (berabohm_input_counter == 0) res = Mame.readinputport(0);
                else
                {

                    res = Mame.readinputport(4 + (berabohm_input_counter - 1));
                    if ((res & 0x80) != 0)
                    {
                        if (counter[berabohm_input_counter - 1] >= 0)
                            //					res = 0x40 | counter[berabohm_input_counter-1];	I can't get max power with this...
                            res = 0x40 | (counter[berabohm_input_counter - 1] >> 1);
                        else
                        {
                            if ((res & 0x40) != 0) res = 0x40;
                            else res = 0x00;
                        }
                    }
                    else if ((res & 0x40) != 0)
                    {
                        if (counter[berabohm_input_counter - 1] < 0x3f)
                        {
                            counter[berabohm_input_counter - 1]++;
                            res = 0x00;
                        }
                        else res = 0x7f;
                    }
                    else
                        counter[berabohm_input_counter - 1] = -1;
                }
                berabohm_input_counter = (berabohm_input_counter + 1) % 5;
            }
            else
            {
                res = 0;
                clk++;
                if ((clk & 1) != 0) res |= 0x40;
                else if (berabohm_input_counter == 4) res |= 0x10;

                res |= (Mame.readinputport(1) & 0x8f);
            }

            return res;
        }

        void init_berabohm()
        {
            namcos1_specific berabohm_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(berabohm_specific);
            Mame.install_mem_read_handler(3, 0x1400, 0x1401, berabohm_buttons_r);
        }

        /*******************************************************************************
        *	Alice in Wonderland / Marchen Maze specific 							   *
        *******************************************************************************/
        void init_alice()
        {
            namcos1_specific alice_specific =
          new namcos1_specific(
              0x5b, 0x25,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(alice_specific);
        }

        /*******************************************************************************
        *	Bakutotsu Kijuutei specific 											   *
        *******************************************************************************/
        void init_bakutotu()
        {
            namcos1_specific bakutotu_specific =
          new namcos1_specific(
              0x03, 0x22,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(bakutotu_specific);
        }

        /*******************************************************************************
        *	World Court specific													   *
        *******************************************************************************/
        void init_wldcourt()
        {
            namcos1_specific worldcourt_specific =
          new namcos1_specific(
              0x00, 0x35,				/* key query , key id */
              rev2_key_r, rev2_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(worldcourt_specific);
        }

        /*******************************************************************************
        *	Splatter House specific 												   *
        *******************************************************************************/
        void init_splatter()
        {
            namcos1_specific splatter_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev2_key_r, rev2_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(splatter_specific);
        }

        /*******************************************************************************
        *	Face Off specific														   *
        *******************************************************************************/
        void init_faceoff()
        {
            namcos1_specific faceoff_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(faceoff_specific);
        }

        /*******************************************************************************
        *	Rompers specific														   *
        *******************************************************************************/
        void init_rompers()
        {
            namcos1_specific rompers_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(rompers_specific);
            key[0x70] = 0xb6;
        }

        /*******************************************************************************
        *	Blast Off specific														   *
        *******************************************************************************/
        void init_blastoff()
        {
            namcos1_specific blastoff_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(blastoff_specific);
            key[0] = 0xb7;
        }

        /*******************************************************************************
        *	World Stadium '89 specific                                                 *
        *******************************************************************************/
        void init_ws89()
        {
            namcos1_specific ws89_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(ws89_specific);

            key[0x20] = 0xb8;
        }

        /*******************************************************************************
        *	Dangerous Seed specific 												   *
        *******************************************************************************/
        void init_dangseed()
        {
            namcos1_specific dangseed_specific =
          new namcos1_specific(
              0x00, 0x34,					/* key query , key id */
              dangseed_key_r, dangseed_key_w,	/* key handler */
              normal_slice,					/* CPU slice normal */
              1								/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(dangseed_specific);
        }

        /*******************************************************************************
        *	World Stadium '90 specific                                                 *
        *******************************************************************************/
        void init_ws90()
        {
            namcos1_specific ws90_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(ws90_specific);

            key[0x47] = 0x36;
            key[0x40] = 0x36;
        }

        /*******************************************************************************
        *	Pistol Daimyo no Bouken specific										   *
        *******************************************************************************/
        void init_pistoldm()
        {
            namcos1_specific pistoldm_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(pistoldm_specific);
            //key[0x17] = ;
            //key[0x07] = ;
            key[0x43] = 0x35;
        }

        /*******************************************************************************
        *	Souko Ban DX specific													   *
        *******************************************************************************/
        void init_soukobdx()
        {
            namcos1_specific soukobdx_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(soukobdx_specific);
            //key[0x27] = ;
            //key[0x07] = ;
            key[0x43] = 0x37;
        }

        /*******************************************************************************
        *	Puzzle Club specific													   *
        *******************************************************************************/
        void init_puzlclub()
        {
            namcos1_specific puzlclub_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(puzlclub_specific);
            key[0x03] = 0x35;
        }

        /*******************************************************************************
        *	Tank Force specific 													   *
        *******************************************************************************/
        void init_tankfrce()
        {
            namcos1_specific tankfrce_specific =
          new namcos1_specific(
              0x00, 0x00,				/* key query , key id */
              rev1_key_r, rev1_key_w,	/* key handler */
              normal_slice,			/* CPU slice normal */
              1						/* use tilemap flag : speedup optimize */
          );
            namcos1_driver_init(tankfrce_specific);
            //key[0x57] = ;
            //key[0x17] = ;
            key[0x2b] = 0xb9;
            key[0x50] = 0xb9;
        }
    }
}
