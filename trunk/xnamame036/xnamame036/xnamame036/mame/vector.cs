using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        static void scale_vectorgames(int gfx_width, int gfx_height, ref int width, ref int height)
        {
            double x_scale, y_scale, scale;

            if ((Machine.orientation & ORIENTATION_SWAP_XY)!=0)
            {
                x_scale = (double)gfx_width / (double)(height);
                y_scale = (double)gfx_height / (double)(width);
            }
            else
            {
                x_scale = (double)gfx_width / (double)(width);
                y_scale = (double)gfx_height / (double)(height);
            }
            if (x_scale < y_scale)
                scale = x_scale;
            else
                scale = y_scale;
            width = (int)((double)width * scale);
            height = (int)((double)height * scale);

            /* Padding to an dword value */
            width -= width % 4;
            height -= height % 4; 
        }

    }
}
