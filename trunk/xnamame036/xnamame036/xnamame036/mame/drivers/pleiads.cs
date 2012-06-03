using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class pleiads
        {

            static int channel;

            static int sound_latch_a;
            static int sound_latch_b;
            static int sound_latch_c;	/* part of the videoreg_w latch */

            public static void pleiads_sound_control_c_w(int offset, int data)
            {
                if (data == sound_latch_c)
                    return;

                //if (errorlog) fprintf(errorlog, "pleiads_sound_control_c_w $%02x\n", data);
                Mame.stream_update(channel, 0);
                sound_latch_c = data;
            }
        }
    }
}