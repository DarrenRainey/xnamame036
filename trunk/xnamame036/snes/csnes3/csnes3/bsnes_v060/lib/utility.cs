using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public static T[] allocate<T>(int size, T defaultvalue)
        {
            T[] t = new T[size];
            for (int i = 0; i < size; i++)
                t[i] = defaultvalue;
            return t;
        }
    }
}
