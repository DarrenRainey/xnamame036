using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class Sega
    {
        const byte MAX_SPEECH = 16;
        const int NOT_PLAYING = -1;

        static int[] queue = new int[MAX_SPEECH];
        static int queuePtr = 0;

        public static int sega_sh_r(int offset)
        {
            /* 0x80 = universal sound board ready */
            /* 0x01 = speech ready */

            if (Mame.sample_playing(0))
                return 0x81;
            else
                return 0x80;
        }
        public static void sega_sh_speech_w(int offset, int data)
        {
            int sound;

            sound = data & 0x7f;
            /* The sound numbers start at 1 but the sample name array starts at 0 */
            sound--;

            if (sound < 0)	/* Can this happen? */
                return;

            if ((data & 0x80)==0)
            {
                /* This typically comes immediately after a speech command. Purpose? */
                return;
            }
            else if (Mame.Machine.samples != null && sound < Mame.Machine.samples.sample.Length && Mame.Machine.samples.sample[sound] != null)
            {
                int newPtr;

                /* Queue the new sound */
                newPtr = queuePtr;
                while (queue[newPtr] != NOT_PLAYING)
                {
                    newPtr++;
                    if (newPtr >= MAX_SPEECH)
                        newPtr = 0;
                    if (newPtr == queuePtr)
                    {
                        /* The queue has overflowed. Oops. */
                        Mame.printf( "*** Queue overflow! queuePtr: %02d\n", queuePtr);
                        return;
                    }
                }
                queue[newPtr] = sound;
            }
        }
        public static void zektor1_sh_w(int offset, int data)
        {
            data ^= 0xff;

            /* Play fireball sample */
            if ((data & 0x02)!=0)
                Mame.sample_start(0, 19, false);

            /* Play explosion samples */
            if ((data & 0x04)!=0)
                Mame.sample_start(1, 29, false);
            if ((data & 0x08)!=0)
                Mame.sample_start(1, 28, false);
            if ((data & 0x10)!=0)
                Mame.sample_start(1, 27, false);

            /* Play bounce sample */
            if ((data & 0x20)!=0)
            {
                if (Mame.sample_playing(2))
                    Mame.sample_stop(2);
                Mame.sample_start(2, 20, false);
            }

            /* Play lazer sample */
            if ((data & 0xc0)!=0)
            {
                if (Mame.sample_playing(3))
                    Mame.sample_stop(3);
                Mame.sample_start(3, 24, false);
            }
        }

        public static void zektor2_sh_w(int offset, int data)
        {
            data ^= 0xff;

            /* Play thrust sample */
            if ((data & 0x0f)!=0)
                Mame.sample_start(4, 25, false);
            else
                Mame.sample_stop(4);

            /* Play skitter sample */
            if ((data & 0x10)!=0)
                Mame.sample_start(5, 21, false);

            /* Play eliminator sample */
            if ((data & 0x20)!=0)
                Mame.sample_start(6, 22, false);

            /* Play electron samples */
            if ((data & 0x40)!=0)
                Mame.sample_start(7, 40, false);
            if ((data & 0x80)!=0)
                Mame.sample_start(7, 41, false);
        }


    }
}
