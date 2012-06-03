using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class williams
    {
        static void williams_dac_data_w(int offset, int data)
        {
            DAC.DAC_data_w(0, data);
        }

    }
}
