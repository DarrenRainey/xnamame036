using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{
    partial class Mame
    {
        static void memset(byte[] b, byte v, int l)
        {
            for (int i = 0; i < l; i++) b[i] = v;
        }
        static void memset(_BytePtr b, byte v, int l)
        {
            for (int i = 0; i < l; i++) b[i] = v;
        }
        static void memcpy(byte[] dst, int doff, byte[] src, int soff, int l)
        {
            Buffer.BlockCopy(src, soff, dst, doff, l);
        }
        static Stream fopen(string name, string mode)
        {
            try
            {
                if (mode == "rb")
                {                    
                    return new FileStream(name, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    return new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        static void fclose(Stream f)
        {
            f.Close();
        }

    }
}
