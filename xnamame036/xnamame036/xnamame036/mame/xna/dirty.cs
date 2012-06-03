using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int MAX_GFX_WIDTH = 1600;
        const int MAX_GFX_HEIGHT = 1600;
        public const int DIRTY_H = 256;
        public const int DIRTY_V = MAX_GFX_HEIGHT / 16;

        static void MARKDIRTY(int x, int y)
        { 
            dirty_new[(y) / 16 * DIRTY_H + (x) / 16] = 1; 
        }
    }
}
