using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int HASH_INFO_NO_DUMP = 0, HASH_INFO_BAD_DUMP = 1;
        const int HASH_CRC = 1 << 0;
        const int HASH_SHA1 = 1 << 1;
        const int HASH_MD5 = 1 << 2;

        const int HASH_NUM_FUNCTIONS = 3;
        const int HASH_BUF_SIZE = 256;

        void hash_compute(ref string dst, byte[] data, uint length, uint functions)
        {
            throw new Exception();
        }
        void hash_data_clear(ref string hash) { hash = ""; }
        int hash_data_insert_binary_checksum(ref string d, uint function, ref string checksum)
        {
            throw new Exception();
        }
    }
}
