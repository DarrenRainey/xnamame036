using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class ZipConstants
    {
        /* The local file header */
        public int LOCHDR = 30;
        public long LOCSIG = 'P' | ('K' << 8) | (3 << 16) | (4 << 24);

        public int LOCVER = 4;
        public int LOCFLG = 6;
        public int LOCHOW = 8;
        public int LOCTIM = 10;
        public int LOCCRC = 14;
        public int LOCSIZ = 18;
        public int LOCLEN = 22;
        public int LOCNAM = 26;
        public int LOCEXT = 28;

        /* The Data descriptor */
        public long EXTSIG = 'P' | ('K' << 8) | (7 << 16) | (8 << 24);
        public int EXTHDR = 16;

        public int EXTCRC = 4;
        public int EXTSIZ = 8;
        public int EXTLEN = 12;

        /* The central directory file header */
        public long CENSIG = 'P' | ('K' << 8) | (1 << 16) | (2 << 24);
        public int CENHDR = 46;

        public int CENVEM = 4;
        public int CENVER = 6;
        public int CENFLG = 8;
        public int CENHOW = 10;
        public int CENTIM = 12;
        public int CENCRC = 16;
        public int CENSIZ = 20;
        public int CENLEN = 24;
        public int CENNAM = 28;
        public int CENEXT = 30;
        public int CENCOM = 32;
        public int CENDSK = 34;
        public int CENATT = 36;
        public int CENATX = 38;
        public int CENOFF = 42;

        /* The entries in the end of central directory */
        public long ENDSIG = 'P' | ('K' << 8) | (5 << 16) | (6 << 24);
        public int ENDHDR = 22;
         
        public int ENDSUB = 8;
        public int ENDTOT = 10;
        public int ENDSIZ = 12;
        public int ENDOFF = 16;
        public int ENDCOM = 20;
    }
}
