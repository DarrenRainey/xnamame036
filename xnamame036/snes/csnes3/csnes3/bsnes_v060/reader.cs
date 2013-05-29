using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static Reader reader = new Reader();

        class Reader
        {
            public bool direct_load(string filename, ref byte[] data, ref uint size)
            {
                FileStream fp = new FileStream(filename, FileMode.Open, FileAccess.Read);
                data = new byte[fp.Length];
                size=(uint)fp.Read(data, 0, data.Length);
                fp.Close();

                // Remove copier header, if it exists
                if ((size & 0x7fff) == 512)
                    Array.Copy(data, 512, data, 0, size -= 512);

                return true;
            }
        }
    }
}