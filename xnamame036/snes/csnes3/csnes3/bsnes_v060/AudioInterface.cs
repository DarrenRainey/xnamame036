using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static AudioInterface audio = new AudioInterface();

        class Audio
        {
            public void clear()
            {
                throw new Exception();
            }
        }
        class AudioInterface
        {
            Audio p;
            uint volume;
            int[] r_left = new int[4], r_right = new int[4];
            double r_step, r_frac;
            public void clear()
            {
                r_frac = 0;
                r_left[0] = r_left[1] = r_left[2] = r_left[3] = 0;
                r_right[0] = r_right[1] = r_right[2] = r_right[3] = 0;
                if (p!=null) p.clear();
            }
        }
    }
}
