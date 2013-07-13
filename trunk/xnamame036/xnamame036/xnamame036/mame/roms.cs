using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public class roms
    {
        static List<Mame.RomModule> rm;

        const uint ROMFLAG_MASK = 0xf8000000;
        const uint ROMFLAG_ALTERNATE = 0x80000000;
        const uint ROMFLAG_WIDE = 0x40000000;
        const uint ROMFLAG_SWAP = 0x20000000;
        const uint ROMFLAG_NIBBLE = 0x10000000;
        const uint ROMFLAG_QUAD = 0x08000000;

        public static void ROM_START(string name) { rm = new List<Mame.RomModule>(); }
        public static Mame.RomModule[] ROM_END { get { rm.Add(new Mame.RomModule(null,0,0,0)); return rm.ToArray(); } }
        public static void ROM_REGION(uint length,uint type){rm.Add(new Mame.RomModule(null,length,0,type));}
        public static void ROM_LOAD(string name,uint offset,uint length,uint crc) { rm.Add(new Mame.RomModule(name, offset, length, crc)); }
        public static void ROM_CONTINUE(uint offset, uint length){rm.Add(new Mame.RomModule(null,offset,length,0));}
        public static void ROM_RELOAD(uint offset,uint length) { rm.Add(new Mame.RomModule("-1", offset, length, 0 ));}
        public static void ROM_LOAD_EVEN(string name,uint offset,uint length,uint crc) 
        {
            rm.Add(new Mame.RomModule(name, (uint)((offset) & ~1), (length) | ROMFLAG_ALTERNATE, crc)); 
        }
        public static void ROM_LOAD_ODD(string name,uint offset,uint length,uint crc)
        {
            rm.Add(new Mame.RomModule(name, (offset) | 1, (length) | ROMFLAG_ALTERNATE, crc));
        }
        public static void ROM_RELOAD_EVEN(uint offset,uint length)
        {
            rm.Add(new Mame.RomModule("-1", (uint)((offset) & ~1), (length) | ROMFLAG_ALTERNATE, 0));
        }
        public static void ROM_RELOAD_ODD(uint offset, uint length)
        {
            rm.Add(new Mame.RomModule("-1", (uint)((offset) |1), (length) | ROMFLAG_ALTERNATE, 0));
        }
        public static void ROM_RELOAD_V20_EVEN(uint offset, uint length)
        {
#if WINDOWS 
            ROM_RELOAD_EVEN(offset,length);
#else
            ROM_RELOAD_ODD(offset,length);
#endif
        }
        public static void ROM_RELOAD_V20_ODD( uint offset, uint length)
        {
#if WINDOWS
            ROM_RELOAD_ODD( offset, length);
#else
            ROM_RELOAD_EVEN(offset,length);
#endif
        }
        public static void ROM_LOAD_V20_EVEN(string name, uint offset, uint length, uint crc)
        {
#if WINDOWS
            ROM_LOAD_EVEN(name, offset, length, crc);
#else
            ROM_LOAD_ODD(name,offset,length,crc);
#endif
        }
        public static void ROM_LOAD_V20_ODD(string name, uint offset, uint length, uint crc)
        {
#if WINDOWS
            ROM_LOAD_ODD(name, offset, length, crc);
#else
            ROM_LOAD_EVEN(name,offset,length,crc);
#endif
        }
        public static void ROM_LOAD_GFX_EVEN(string name, uint offset, uint length, uint crc)
        {
#if WINDOWS
            ROM_LOAD_ODD(name, offset, length, crc);
#else
            ROM_LOAD_EVEN(name, offset, length, crc);
#endif
        }
        public static void ROM_LOAD_GFX_ODD(string name, uint offset, uint length, uint crc)
        {
#if WINDOWS
            ROM_LOAD_EVEN(name, offset, length, crc);
#else
            ROM_LOAD_ODD(name, offset, length, crc);
#endif
        }
    }
}
