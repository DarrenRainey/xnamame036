using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class DACinterface
        {
            public DACinterface(int num, int[] mixing_level)
            {
                this.num = num; this.mixing_level = mixing_level;
            }
            public int num;
            public int[] mixing_level;
        }
    }
    public class DAC : Mame.snd_interface
    {
        public DAC()
        {
            this.name = "DAC";
            this.sound_num = Mame.SOUND_DAC;
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return 0;// throw new NotImplementedException();
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return ((Mame.DACinterface)msound.sound_interface).num;
        }
        public override void reset()
        {
            //none
        }
        static void DAC_update(int num, _ShortPtr buffer, int length)
        {
            int _out = output[num];
            int bi = 0;
            while (length-- != 0) buffer.write16(bi++, (ushort)_out);
        }

        static void DAC_build_voltable()
        {
            /* build volume table (linear) */
            for (int i = 0; i < 256; i++)
            {
                UnsignedVolTable[i] = i * 0x101 / 2;	/* range      0..32767 */
                SignedVolTable[i] = i * 0x101 - 0x8000;	/* range -32768..32767 */
            }
        }
        public static void DAC_signed_data_16_w(int num, int data)
        {
            int _out = data - 0x8000;	/* range -32768..32767 */

            if (output[num] != _out)
            {
                /* update the output buffer before changing the registers */
                Mame.stream_update(channel[num], 0);
                output[num] = _out;
            }
        }

        public override int start(Mame.MachineSound msound)
        {
            Mame.DACinterface intf = (Mame.DACinterface)msound.sound_interface;


            DAC_build_voltable();

            for (int i = 0; i < intf.num; i++)
            {
                string name = Mame.sprintf("DAC #%d", i);
                channel[i] = Mame.stream_init(name, intf.mixing_level[i], Mame.Machine.sample_rate,
                        i, DAC_update);

                if (channel[i] == -1)
                    return 1;

                output[i] = 0;
            }

            return 0;
        }
        public override void stop()
        {
            //none
        }
        public override void update()
        {
            //nonehrow new NotImplementedException();
        }
        const byte MAX_DAC = 4;
        static int[] channel = new int[MAX_DAC];
        static int[] output = new int[MAX_DAC];
        static int[] UnsignedVolTable = new int[256];
        static int[] SignedVolTable = new int[256];


        public static void DAC_data_w(int num, int data)
        {
            int _out = UnsignedVolTable[data];

            if (output[num] != _out)
            {
                /* update the output buffer before changing the registers */
                Mame.stream_update(channel[num], 0);
                output[num] = _out;
            }
        }
        public static void DAC_signed_data_w(int num, int data)
        {
            int _out = SignedVolTable[data];

            if (output[num] != _out)
            {
                /* update the output buffer before changing the registers */
                Mame.stream_update(channel[num], 0);
                output[num] = _out;
            }
        }


    }
}
