using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class DeflaterHuffman
    {
         private static String bit4Reverse =
    "\000\010\004\014\002\012\006\016\001\011\005\015\003\013\007\017";

        public static short bitReverse(int value)
        {

            return (short)(bit4Reverse[value & 0xf] << 12
                    | bit4Reverse[(value >> 4) & 0xf] << 8
                    | bit4Reverse[(value >> 8) & 0xf] << 4
                    | bit4Reverse[value >> 12]);
        }
    }
}
