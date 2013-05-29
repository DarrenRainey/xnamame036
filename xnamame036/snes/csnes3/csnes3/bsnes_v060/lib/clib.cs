using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static int memcmp(byte[] data, int index, string s, int l)
        {
            byte[] b = Encoding.Default.GetBytes(s);
            for (int i = 0; i < l; i++)
            {
                if (data[i+index] < b[i]) return -1;
                else if (data[i+index] > b[i]) return 1;
            }
            return 0;
        }
    }
}
