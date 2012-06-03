using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
    partial class driver_dkong
    {
        static int flipscreen;
        static int gfx_bank,palette_bank,grid_on;
        static _BytePtr color_codes = new _BytePtr(1);

        static void dkong_flipscreen_w(int offset, int data)
        {
            if (flipscreen != (~data & 1))
            {
                flipscreen = ~data & 1;
                Generic.SetDirtyBuffer(true); 
            }
        }
        static void dkong_palettebank_w(int offset, int data)
        {
            int newbank= palette_bank;
            if ((data & 1)!=0)
                newbank |= 1 << offset;
            else
                newbank &= ~(1 << offset);

            if (palette_bank != newbank)
            {
                palette_bank = newbank;
                Generic.SetDirtyBuffer(true);
            }
        }
    }
}
