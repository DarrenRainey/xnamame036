using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public static System system = new System();

        public partial  class System
        {
            public enum Region { NTSC = 0, PAL = 1 };
            public enum RegionAutodetect { Autodetect = 2 };
            public enum ExpansionPortDevice { ExpansionNone = 0, ExpansionBSX = 1 };


            public uint region, expansion, serialize_size;

            public void power()
            {
                throw new Exception();
            }
            public void unload()
            {
                throw new Exception();
            }
        }
    }
}