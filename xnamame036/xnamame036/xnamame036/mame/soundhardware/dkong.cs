using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_dkong
    {
        static int walk = 0; /* used to determine if dkongjr is walking or climbing? */

        static void dkong_sh_w(int offset, int data)
        {
            if (data != 0)
                Mame.cpu_cause_interrupt(1, Mame.cpu_i8039.I8039_EXT_INT);
        }


        static int[] state = new int[8];
        static void dkong_sh1_w(int offset, int data)
        {
            if (state[offset] != data)
            {
                if (data != 0)
                    Mame.sample_start(offset, offset, false);

                state[offset] = data;
            }
        }

        static int death = 0;
        static void dkongjr_sh_death_w(int offset, int data)
        {
            if (death != data)
            {
                if (data != 0)
                    Mame.sample_stop(7);
                Mame.sample_start(6, 4, false);

                death = data;
            }
        }

        static int drop = 0;
        static void dkongjr_sh_drop_w(int offset, int data)
        {
            if (drop != data)
            {
                if (data!=0)
                    Mame.sample_start(7, 5, false);

                drop = data;
            }
        }

        static int roar = 0;
        static void dkongjr_sh_roar_w(int offset, int data)
        {
            if (roar != data)
            {
                if (data!=0)
                    Mame.sample_start(7, 2, false);
                roar = data;
            }
        }

        static int jump = 0;
        static void dkongjr_sh_jump_w(int offset, int data)
        {
            if (jump != data)
            {
                if (data!=0)
                    Mame.sample_start(6, 0, false);

                jump = data;
            }
        }

        static int land = 0;
        static void dkongjr_sh_land_w(int offset, int data)
        {
            if (land != data)
            {
                if (data!=0)
                    Mame.sample_stop(7);
                Mame.sample_start(4, 1, false);

                land = data;
            }
        }

        static int climb = 0;
        static void dkongjr_sh_climb_w(int offset, int data)
        {
            if (climb != data)
            {
                if (data != 0 && walk == 0)
                {
                    Mame.sample_start(3, 3, false);
                }
                else if (data != 0 && walk == 1)
                {
                    Mame.sample_start(3, 6, false);
                }
                climb = data;
            }
        }

        static int snapjaw = 0;
        static void dkongjr_sh_snapjaw_w(int offset, int data)
        {
            if (snapjaw != data)
            {
                if (data != 0)
                    Mame.sample_stop(7);
                Mame.sample_start(4, 7, false);

                snapjaw = data;
            }
        }

        static void dkongjr_sh_walk_w(int offset, int data)
        {
            if (walk != data)
            {
                walk = data;
            }
        }
    }
}
