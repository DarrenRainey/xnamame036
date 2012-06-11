using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class eeprom
    {
        public class EEPROM_interface
        {
            public EEPROM_interface(int address_bits, int data_bits, string cmd_read, string cmd_write, string cmd_erase, string cmd_lock = null, string cmd_unlock = null)
            {
                this.address_bits = address_bits;
                this.data_bits = data_bits;
                this.cmd_erase = cmd_erase;
                this.cmd_read = cmd_read; this.cmd_write = cmd_write;
                this.cmd_lock = cmd_lock; this.cmd_unlock = cmd_unlock;
            }
            public int address_bits;	/* EEPROM has 2^address_bits cells */
            public int data_bits;		/* every cell has this many bits (8 or 16) */
            public string cmd_read;		/*  read command string, e.g. "0110" */
            public string cmd_write;	/* write command string, e.g. "0111" */
            public string cmd_erase;	/* erase command string, or 0 if n/a */
            public string cmd_lock;		/* lock command string, or 0 if n/a */
            public string cmd_unlock;	/* unlock command string, or 0 if n/a */
        }
        const int SERIAL_BUFFER_LENGTH = 30;

        static EEPROM_interface intf;

        static int serial_count;
        static char[] serial_buffer = new char[SERIAL_BUFFER_LENGTH];
        static byte[] eeprom_data = new byte[256];
        static int eeprom_data_bits;
        static int latch, reset_line, clock_line, sending;
        static int locked;

        static void EEPROM_reset()
        {
            //if (errorlog && serial_count)fprintf(errorlog,"EEPROM reset, buffer = %s\n",serial_buffer);

            serial_count = 0;
            sending = 0;
        }
        static bool strncmp(char[] s1, string s2, int n)
        {
            if (n > s2.Length) n = s2.Length;
            return new string(s1).CompareTo(s2.Substring(0, n)) == 0;
        }
        static void EEPROM_write(int bit)
        {
#if VERBOSE
if (errorlog) fprintf(errorlog,"EEPROM write bit %d\n",bit);
#endif

            if (serial_count >= SERIAL_BUFFER_LENGTH - 1)
            {
                //if (errorlog) fprintf(errorlog, "error: EEPROM serial buffer overflow\n");
                return;
            }

            serial_buffer[serial_count++] = (bit != 0 ? '1' : '0');
            serial_buffer[serial_count] = '\0';	/* nul terminate so we can treat it as a string */

            if (intf.cmd_read != null && serial_count == (intf.cmd_read.Length + intf.address_bits) &&
                    !strncmp(serial_buffer, intf.cmd_read, intf.cmd_read.Length))
            {
                int i, address;

                address = 0;
                for (i = 0; i < intf.address_bits; i++)
                {
                    address <<= 1;
                    if (serial_buffer[i + intf.cmd_read.Length] == '1') address |= 1;
                }
                if (intf.data_bits == 16)
                    eeprom_data_bits = (eeprom_data[2 * address + 0] << 8) + eeprom_data[2 * address + 1];
                else
                    eeprom_data_bits = eeprom_data[address];
                sending = 1;
                serial_count = 0;
               // Mame.printf("EEPROM read %04x from address %02x\n", eeprom_data_bits, address);
            }
            else if (intf.cmd_erase != null && serial_count == (intf.cmd_erase.Length + intf.address_bits) &&
                    !strncmp(serial_buffer, intf.cmd_erase, intf.cmd_erase.Length))
            {
                int i, address;

                address = 0;
                for (i = 0; i < intf.address_bits; i++)
                {
                    address <<= 1;
                    if (serial_buffer[i + intf.cmd_erase.Length] == '1') address |= 1;
                }
               // Mame.printf("EEPROM erase address %02x\n", address);
                if (locked == 0)
                {
                    if (intf.data_bits == 16)
                    {
                        eeprom_data[2 * address + 0] = 0x00;
                        eeprom_data[2 * address + 1] = 0x00;
                    }
                    else
                        eeprom_data[address] = 0x00;
                }
              //  else
                   // Mame.printf("Error: EEPROM is locked\n");
                serial_count = 0;
            }
            else if (intf.cmd_write != null && serial_count == (intf.cmd_write.Length + intf.address_bits + intf.data_bits) &&
                    !strncmp(serial_buffer, intf.cmd_write, intf.cmd_write.Length))
            {
                int i, address, data;

                address = 0;
                for (i = 0; i < intf.address_bits; i++)
                {
                    address <<= 1;
                    if (serial_buffer[i + intf.cmd_write.Length] == '1') address |= 1;
                }
                data = 0;
                for (i = 0; i < intf.data_bits; i++)
                {
                    data <<= 1;
                    if (serial_buffer[i + intf.cmd_write.Length + intf.address_bits] == '1') data |= 1;
                }
                //Mame.printf("EEPROM write %04x to address %02x\n", data, address);
                if (locked == 0)
                {
                    if (intf.data_bits == 16)
                    {
                        eeprom_data[2 * address + 0] = (byte)(data >> 8);
                        eeprom_data[2 * address + 1] = (byte)(data & 0xff);
                    }
                    else
                        eeprom_data[address] = (byte)data;
                }
               // else
                   // Mame.printf("Error: EEPROM is locked\n");
                serial_count = 0;
            }
            else if (intf.cmd_lock != null && (serial_count == intf.cmd_lock.Length) &&
                    !strncmp(serial_buffer, intf.cmd_lock, intf.cmd_lock.Length))
            {
                //Mame.printf("EEPROM lock\n");
                locked = 1;
                serial_count = 0;
            }
            else if (intf.cmd_unlock != null && (serial_count == intf.cmd_unlock.Length) &&
                    !strncmp(serial_buffer, intf.cmd_unlock, intf.cmd_unlock.Length))
            {
                //Mame.printf("EEPROM unlock\n");
                locked = 0;
                serial_count = 0;
            }
        }
        public static int EEPROM_read_bit()
        {
            int res;

            if (sending != 0)
                res = (eeprom_data_bits >> intf.data_bits) & 1;
            else res = 1;

#if VERBOSE
if (errorlog) fprintf(errorlog,"read bit %d\n",res);
#endif

            return res;
        }
        public static void EEPROM_write_bit(int bit)
        {
#if VERBOSE
if (errorlog) fprintf(errorlog,"write bit %d\n",bit);
#endif
            latch = bit;
        }
        public static void EEPROM_set_clock_line(int state)
        {
#if VERBOSE
if (errorlog) fprintf(errorlog,"set clock line %d\n",state);
#endif
            if (state == Mame.PULSE_LINE || (clock_line == Mame.CLEAR_LINE && state != Mame.CLEAR_LINE))
            {
                if (reset_line == Mame.CLEAR_LINE)
                {
                    if (sending != 0)
                        eeprom_data_bits = (eeprom_data_bits << 1) | 1;
                    else
                        EEPROM_write(latch);
                }
            }

            clock_line = state;
        }
        public static void EEPROM_set_cs_line(int state)
        {
#if VERBOSE
if (errorlog) fprintf(errorlog,"set reset line %d\n",state);
#endif
            reset_line = state;

            if (reset_line != Mame.CLEAR_LINE)
                EEPROM_reset();
        }
        public static void EEPROM_load(object f)
        {
            Mame.osd_fread(f, eeprom_data, (1 << intf.address_bits) * intf.data_bits / 8);
        }
        public static void EEPROM_init(EEPROM_interface intrface)
        {
            intf = intrface;
            for (int i = 0; i < (1 << intf.address_bits) * intf.data_bits / 8; i++)
                eeprom_data[i] = 0xff; //memset(eeprom_data,0xff,(1 << intf.address_bits) * intf.data_bits / 8);
            serial_count = 0;
            latch = 0;
            reset_line = Mame.ASSERT_LINE;
            clock_line = Mame.ASSERT_LINE;
            sending = 0;
            if (intf.cmd_unlock != null) locked = 1;
            else locked = 0;
        }
        public static void EEPROM_save(object f)
        {
            Mame.osd_fwrite(f, eeprom_data, (1 << intf.address_bits) * intf.data_bits / 8);
        }

    }
}
