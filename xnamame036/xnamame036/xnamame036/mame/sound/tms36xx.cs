using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const byte MAX_TMS36XX = 4;
        abstract class TMS36XXinterface
        {
            public int num;
            public int[] mixing_level = new int[MAX_TMS36XX];
        }
        public class tms36xx
        {
            const byte MM6221AA = 21;
            const byte TNS3617 = 17;
            class TMS36XX
            {
                public string subtype;		/* subtype name MM6221AA, TMS3615 or TMS3617 */
                public int channel;		/* returned by stream_init() */

                public int samplerate; 	/* from Machine.sample_rate */

                public int basefreq;		/* chip's base frequency */
                public int octave; 		/* octave select of the TMS3615 */

                public int speed;			/* speed of the tune */
                public int tune_counter;	/* tune counter */
                public int note_counter;	/* note counter */

                public int voices; 		/* active voices */
                public int shift;			/* shift toggles between 0 and 6 to allow decaying voices */
                public int[] vol = new int[12];		/* (decaying) volume of harmonics notes */
                public int[] vol_counter = new int[12];/* volume adjustment counter */
                public int[] decay = new int[12];		/* volume adjustment rate - dervied from decay */

                public int[] counter = new int[12];	/* tone frequency counter */
                public int[] frequency = new int[12];	/* tone frequency */
                public int output; 		/* output signal bits */
                public int enable; 		/* mask which harmoics */

                public int tune_num;		/* tune currently playing */
                public int tune_ofs;		/* note currently playing */
                public int tune_max;		/* end of tune */
            }
            static TMS36XXinterface intf;
            static TMS36XX[] _tms36xx = new TMS36XX[MAX_TMS36XX];
            static tms36xx()
            {
                for (int i = 0; i < MAX_TMS36XX; i++)
                    _tms36xx[i] = new TMS36XX();
            }
            public static void mm6221aa_tune_w(int chip, int tune)
            {
                TMS36XX tms = _tms36xx[chip];

                /* which tune? */
                tune &= 3;
                if (tune == tms.tune_num)
                    return;

                //LOG((errorlog,"%s tune:%X\n", tms.subtype, tune));

                /* update the stream before changing the tune */
                stream_update(tms.channel, 0);

                tms.tune_num = tune;
                tms.tune_ofs = 0;
                tms.tune_max = 96; /* fixed for now */
            }
        }
    }
}