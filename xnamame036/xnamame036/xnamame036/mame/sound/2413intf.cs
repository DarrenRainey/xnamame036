using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const int MAX_2413 = MAX_3812;

        
    }
    public class YM2413interface : YM3812interface 
    {
        public YM2413interface(int num, int baseclock, int[] mixing_level) : base(num, baseclock, mixing_level) { }
    }
}
