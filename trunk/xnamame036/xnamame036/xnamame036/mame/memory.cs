#define HAS_Z80

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    using MHELE = System.Byte;
    partial class Mame
    {
        public delegate int mem_read_handler(int offset);
        public delegate void mem_write_handler(int offset, int data);
        public delegate int opbase_handler(int address);

        public const int MRA_NOP = 0,
            MRA_RAM = -1,
            MRA_ROM = -2,
            MRA_BANK1 = -10,
            MRA_BANK2 = -11,
            MRA_BANK3 = -12,
            MRA_BANK4 = -13,
            MRA_BANK5 = -14,
            MRA_BANK6 = -15,
            MRA_BANK7 = -16,
            MRA_BANK8 = -17,
            MRA_BANK9 = -18,
            MRA_BANK10 = -19,
            MRA_BANK11 = -20,
            MRA_BANK12 = -21,
            MRA_BANK13 = -22,
            MRA_BANK14 = -23,
            MRA_BANK15 = -24,
            MRA_BANK16 = -25;
        public const int MWA_NOP = 0,
            MWA_RAM = -1, MWA_ROM = -2,
            MWA_RAMROM = -3,
            MWA_BANK1 = -10,
            MWA_BANK2 = -11,
            MWA_BANK3 = -12,
            MWA_BANK4 = -13,
            MWA_BANK5 = -14,
            MWA_BANK6 = -15,
            MWA_BANK7 = -16,
            MWA_BANK8 = -17,
            MWA_BANK9 = -18,
            MWA_BANK10 = -19,
            MWA_BANK11 = -20,
            MWA_BANK12 = -21,
            MWA_BANK13 = -22,
            MWA_BANK14 = -23,
            MWA_BANK15 = -24,
            MWA_BANK16 = -25;

        static void MWA_RAM_handler(int address, int data) { }
        static void MWA_BANK1_handler(int address, int data) { }
        static void MWA_BANK2_handler(int address, int data) { }
        static void MWA_BANK3_handler(int address, int data) { }
        static void MWA_BANK4_handler(int address, int data) { }
        static void MWA_BANK5_handler(int address, int data) { }
        static void MWA_BANK6_handler(int address, int data) { }
        static void MWA_BANK7_handler(int address, int data) { }
        static void MWA_BANK8_handler(int address, int data) { }
        static void MWA_BANK9_handler(int address, int data) { }
        static void MWA_BANK10_handler(int address, int data) { }
        static void MWA_BANK11_handler(int address, int data) { }
        static void MWA_BANK12_handler(int address, int data) { }
        static void MWA_BANK13_handler(int address, int data) { }
        static void MWA_BANK14_handler(int address, int data) { }
        static void MWA_BANK15_handler(int address, int data) { }
        static void MWA_BANK16_handler(int address, int data) { }
        public static void MWA_NOP_handler(int address, int data) { }
        public static void MWA_RAMROM_handler(int address, int data) { }
        public static void MWA_ROM_handler(int address, int data) { }
        public static int MRA_ROM_handler(int offset) { return cpu_bankbase[0][offset]; }
        public static int MRA_RAM_handler(int offset) { return cpu_bankbase[0][offset]; }
        public static int MRA_BANK1_handler(int offset) { return cpu_bankbase[1][offset]; }
        public static int MRA_BANK2_handler(int offset) { return cpu_bankbase[2][offset]; }
        public static int MRA_BANK3_handler(int offset) { return cpu_bankbase[3][offset]; }
        public static int MRA_BANK4_handler(int offset) { return cpu_bankbase[4][offset]; }
        public static int MRA_BANK5_handler(int offset) { return cpu_bankbase[5][offset]; }
        public static int MRA_BANK6_handler(int offset) { return cpu_bankbase[6][offset]; }
        public static int MRA_BANK7_handler(int offset) { return cpu_bankbase[7][offset]; }
        public static int MRA_BANK8_handler(int offset) { return cpu_bankbase[8][offset]; }
        public static int MRA_BANK9_handler(int offset) { return cpu_bankbase[9][offset]; }
        public static int MRA_BANK10_handler(int offset) { return cpu_bankbase[10][offset]; }
        public static int MRA_BANK11_handler(int offset) { return cpu_bankbase[11][offset]; }
        public static int MRA_BANK12_handler(int offset) { return cpu_bankbase[12][offset]; }
        public static int MRA_BANK13_handler(int offset) { return cpu_bankbase[13][offset]; }
        public static int MRA_BANK14_handler(int offset) { return cpu_bankbase[14][offset]; }
        public static int MRA_BANK15_handler(int offset) { return cpu_bankbase[15][offset]; }
        public static int MRA_BANK16_handler(int offset) { return cpu_bankbase[16][offset]; }
        public static int MRA_NOP_handler(int offset) { return cpu_bankbase[16][offset]; }

        public class MemoryReadAddress
        {
            public int start = -1, end = -1;
            public mem_read_handler handler = null;				/* see special values below */
            public int _handler = -120365;

            public MemoryReadAddress(int start) { this.start = start; }
            public MemoryReadAddress(int start, int end, int _handler) { this.start = start; this.end = end; this._handler = _handler; }
            public MemoryReadAddress(int start, int end, mem_read_handler handler) { this.start = start; this.end = end; this.handler = handler; }
        }
        public class MemoryWriteAddress
        {
            public int start = -1, end = -1;
            public mem_write_handler handler = null;
            public int _handler = -120365;
            public _BytePtr _base = null;
            public int[] size = null;

            public MemoryWriteAddress(int start) { this.start = start; }
            public MemoryWriteAddress(int start, int end, int _handler) { this.start = start; this.end = end; this._handler = _handler; }
            public MemoryWriteAddress(int start, int end, mem_write_handler handler) { this.start = start; this.end = end; this.handler = handler; }
            public MemoryWriteAddress(int start, int end, mem_write_handler handler, _BytePtr _base) { this.start = start; this.end = end; this.handler = handler; this._base = _base; }
            public MemoryWriteAddress(int start, int end, int _handler, _BytePtr _base) { this.start = start; this._handler = _handler; this.end = end; this._base = _base; }
            public MemoryWriteAddress(int start, int end, int _handler, _BytePtr _base, int[] size) { this.start = start; this._handler = _handler; this.end = end; this._base = _base; this.size = size; }
            public MemoryWriteAddress(int start, int end, mem_write_handler handler, _BytePtr _base, int[] size) { this.start = start; this.end = end; this.handler = handler; this._base = _base; this.size = size; }
        }
        public class IOReadPort
        {
            public IOReadPort() { }
            public IOReadPort(int start) { this.start = start; }
            public IOReadPort(int start, int end, mem_read_handler handler)
            {
                this.start = start; this.end = end; this.handler = handler;
            }
            public int start, end;
            public mem_read_handler handler;
        }
        public class IOWritePort
        {
            public IOWritePort() { }
            public IOWritePort(int start) { this.start = start; }
            public IOWritePort(int start, int end, mem_write_handler handler) { this.start = start; this.end = end; this.handler = handler; }
            public int start, end;
            public mem_write_handler handler;
        }
        public static  int IORP_NOP(int offset) { return 0; }
        public static  void IOWP_NOP(int offset, int data) { }

        static void memorycontextswap(int activecpu)
        {
            cpu_bankbase[0] = ramptr[activecpu];

            cur_mrhard = cur_mr_element[activecpu];
            cur_mwhard = cur_mw_element[activecpu];

            /* ASG: port speedup */
            cur_readport = readport[activecpu];
            cur_writeport = writeport[activecpu];
            cur_portmask = portmask[activecpu];

            OPbasefunc = setOPbasefunc[activecpu];

            /* op code memory pointer */
            ophw = HT_RAM;
            OP_RAM = cpu_bankbase[0];
            OP_ROM = romptr[activecpu];
        }
        const int MH_SBITS = 8;
        const int MH_PBITS = 8;
        const int MH_ELEMAX = 64;
        const int MH_HARDMAX = 64;

        const int ABITS1_16 = 12;
        const int ABITS2_16 = 4;
        const int ABITS_MIN_16 = 0;

        const int ABITS1_20 = 12;
        const int ABITS2_20 = 8;
        const int ABITS_MIN_20=0;

        const int ABITS1_24=15;
        const int ABITS2_24 = 8;
        const int ABITS_MIN_24 = 1;






        public static _BytePtr OP_RAM = new _BytePtr();
        public static _BytePtr OP_ROM = new _BytePtr();



        static MHELE ophw;				/* op-code hardware number */
        const int MAX_EXT_MEMORY = 64;
        struct ExtMemory
        {
            public int start, end, region;
            public _BytePtr data;
        }
        static ExtMemory[] ext_memory = new ExtMemory[MAX_EXT_MEMORY];

        static _BytePtr[] ramptr = new _BytePtr[MAX_CPU], romptr = new _BytePtr[MAX_CPU];

        /* element shift bits, mask bits */
        static int[,] mhshift = new int[MAX_CPU, 3], mhmask = new int[MAX_CPU, 3];

        /* pointers to port structs */
        /* ASG: port speedup */
        static IOReadPort[][] readport = new IOReadPort[MAX_CPU][];
        static IOWritePort[][] writeport = new IOWritePort[MAX_CPU][];
        static int[] portmask = new int[MAX_CPU];
        static int[] readport_size = new int[MAX_CPU];
        static int[] writeport_size = new int[MAX_CPU];
        /* HJB 990210: removed 'static' for access by assembly CPU core memory handlers */
        static IOReadPort[] cur_readport;
        static IOWritePort[] cur_writeport;
        static int cur_portmask;

        /* current hardware element map */
        static MHELE[][] cur_mr_element = new MHELE[MAX_CPU][];
        static MHELE[][] cur_mw_element = new MHELE[MAX_CPU][];

        /* sub memory/port hardware element map */
        /* HJB 990210: removed 'static' for access by assembly CPU core memory handlers */
        static MHELE[] readhardware = new MHELE[MH_ELEMAX << MH_SBITS];	/* mem/port read  */
        static MHELE[] writehardware = new MHELE[MH_ELEMAX << MH_SBITS]; /* mem/port write */

        /* memory hardware element map */
        /* value:                      */
        const int HT_RAM = 0;		/* RAM direct        */
        const int HT_BANK1 = 1;		/* bank memory #1    */
        const int HT_BANK2 = 2;		/* bank memory #2    */
        const int HT_BANK3 = 3;		/* bank memory #3    */
        const int HT_BANK4 = 4;		/* bank memory #4    */
        const int HT_BANK5 = 5;		/* bank memory #5    */
        const int HT_BANK6 = 6;		/* bank memory #6    */
        const int HT_BANK7 = 7;		/* bank memory #7    */
        const int HT_BANK8 = 8;		/* bank memory #8    */
        const int HT_BANK9 = 9;		/* bank memory #9    */
        const int HT_BANK10 = 10;	/* bank memory #10   */
        const int HT_BANK11 = 11;	/* bank memory #11   */
        const int HT_BANK12 = 12;	/* bank memory #12   */
        const int HT_BANK13 = 13;	/* bank memory #13   */
        const int HT_BANK14 = 14;	/* bank memory #14   */
        const int HT_BANK15 = 15;	/* bank memory #15   */
        const int HT_BANK16 = 16;	/* bank memory #16   */
        const int HT_NON = 17;	/* non mapped memory */
        const int HT_NOP = 18;	/* NOP memory        */
        const int HT_RAMROM = 19;	/* RAM ROM memory    */
        const int HT_ROM = 20;	/* ROM memory        */

        const int HT_USER = 21;	/* user functions    */
        /* [MH_HARDMAX]-0xff	  link to sub memory element  */
        /*                        (value-MH_HARDMAX)<<MH_SBITS . element bank */
        const int MAX_BANKS = 16;

        const int HT_BANKMAX = (HT_BANK1 + MAX_BANKS - 1);

        /* memory hardware handler */
        /* HJB 990210: removed 'static' for access by assembly CPU core memory handlers */
        static mem_read_handler[] memoryreadhandler = new mem_read_handler[MH_HARDMAX];
        static int[] memoryreadoffset = new int[MH_HARDMAX];
        static mem_write_handler[] memorywritehandler = new mem_write_handler[MH_HARDMAX];
        static int[] memorywriteoffset = new int[MH_HARDMAX];

        /* bank ram base address; RAM is bank 0 */
        static _BytePtr[] cpu_bankbase = new _BytePtr[HT_BANKMAX + 1];
        static int[] bankreadoffset = new int[HT_BANKMAX + 1];
        static int[] bankwriteoffset = new int[HT_BANKMAX + 1];

        /* override OP base handler */
        static opbase_handler[] setOPbasefunc = new opbase_handler[MAX_CPU];
        static opbase_handler OPbasefunc;

        /* current cpu current hardware element map point */
        static MHELE[] cur_mrhard;
        static MHELE[] cur_mwhard;


        static int rdelement_max = 0;
        static int wrelement_max = 0;
        static int rdhard_max = HT_USER;
        static int wrhard_max = HT_USER;



        static uint ADDRESS_BITS(int index) { return (cpuintf[Machine.drv.cpu[index].cpu_type & ~CPU_FLAGS_MASK].address_bits); }
        static uint ABITS1(int index) { return (cpuintf[Machine.drv.cpu[index].cpu_type & ~CPU_FLAGS_MASK].abits1); }
        static uint ABITS2(int index) { return (cpuintf[Machine.drv.cpu[index].cpu_type & ~CPU_FLAGS_MASK].abits2); }
        static uint ABITS3(int index) { return (0); }
        static uint ABITSMIN(int index) { return (cpuintf[Machine.drv.cpu[index].cpu_type & ~CPU_FLAGS_MASK].abitsmin); }
        static uint MHMASK(int abits) { return (0xffffffff >> (32 - abits)); }

        static _BytePtr memory_find_base(int cpu, int offset)
        {
            int region = REGION_CPU1 + cpu;

            /* look in external memory first */
            foreach (ExtMemory ext in ext_memory)
            {
                if (ext.data == null) break;
                if (ext.region == region && ext.start <= offset && ext.end >= offset)
                    return new _BytePtr(ext.data, (offset - ext.start));
            }
            return new _BytePtr(ramptr[cpu], offset);
        }
        IOReadPort[] empty_readport = { new IOReadPort(-1) };
        IOWritePort[] empty_writeport = { new IOWritePort(-1) };

        static int mrh_ram(int offset) { return cpu_bankbase[0][offset]; }
        static int mrh_bank1(int offset) { return cpu_bankbase[1][offset]; }
        static int mrh_bank2(int offset) { return cpu_bankbase[2][offset]; }
        static int mrh_bank3(int offset) { return cpu_bankbase[3][offset]; }
        static int mrh_bank4(int offset) { return cpu_bankbase[4][offset]; }
        static int mrh_bank5(int offset) { return cpu_bankbase[5][offset]; }
        static int mrh_bank6(int offset) { return cpu_bankbase[6][offset]; }
        static int mrh_bank7(int offset) { return cpu_bankbase[7][offset]; }
        static int mrh_bank8(int offset) { return cpu_bankbase[8][offset]; }
        static int mrh_bank9(int offset) { return cpu_bankbase[9][offset]; }
        static int mrh_bank10(int offset) { return cpu_bankbase[10][offset]; }
        static int mrh_bank11(int offset) { return cpu_bankbase[11][offset]; }
        static int mrh_bank12(int offset) { return cpu_bankbase[12][offset]; }
        static int mrh_bank13(int offset) { return cpu_bankbase[13][offset]; }
        static int mrh_bank14(int offset) { return cpu_bankbase[14][offset]; }
        static int mrh_bank15(int offset) { return cpu_bankbase[15][offset]; }
        static int mrh_bank16(int offset) { return cpu_bankbase[16][offset]; }
        static mem_read_handler[] bank_read_handler =
{
	mrh_ram,   mrh_bank1,  mrh_bank2,  mrh_bank3,  mrh_bank4,  mrh_bank5,  mrh_bank6,  mrh_bank7,
	mrh_bank8, mrh_bank9,  mrh_bank10, mrh_bank11, mrh_bank12, mrh_bank13, mrh_bank14, mrh_bank15,
	mrh_bank16
};
        static int mrh_error(int offset)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %04x: warning - read %02x from unmapped memory address %04x\n",cpu_getactivecpu(),cpu_get_pc(),cpu_bankbase[0][offset],offset);
            return cpu_bankbase[0][offset];
        }

        static int mrh_error_sparse(int offset)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: warning - read unmapped memory address %08x\n",cpu_getactivecpu(),cpu_get_pc(),offset);
            return 0;
        }

        static int mrh_error_sparse_bit(int offset)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: warning - read unmapped memory bit addr %08x (byte addr %08x)\n",cpu_getactivecpu(),cpu_get_pc(),offset<<3, offset);
            return 0;
        }

        static int mrh_nop(int offset)
        {
            return 0;
        }

        public static void mwh_ram(int offset, int data) { cpu_bankbase[0][offset] = (byte)data; }
        public static void mwh_bank1(int offset, int data) { cpu_bankbase[1][offset] = (byte)data; }
        public static void mwh_bank2(int offset, int data) { cpu_bankbase[2][offset] = (byte)data; }
        public static void mwh_bank3(int offset, int data) { cpu_bankbase[3][offset] = (byte)data; }
        public static void mwh_bank4(int offset, int data) { cpu_bankbase[4][offset] = (byte)data; }
        public static void mwh_bank5(int offset, int data) { cpu_bankbase[5][offset] = (byte)data; }
        public static void mwh_bank6(int offset, int data) { cpu_bankbase[6][offset] = (byte)data; }
        public static void mwh_bank7(int offset, int data) { cpu_bankbase[7][offset] = (byte)data; }
        public static void mwh_bank8(int offset, int data) { cpu_bankbase[8][offset] = (byte)data; }
        public static void mwh_bank9(int offset, int data) { cpu_bankbase[9][offset] = (byte)data; }
        public static void mwh_bank10(int offset, int data) { cpu_bankbase[10][offset] = (byte)data; }
        public static void mwh_bank11(int offset, int data) { cpu_bankbase[11][offset] = (byte)data; }
        public static void mwh_bank12(int offset, int data) { cpu_bankbase[12][offset] = (byte)data; }
        public static void mwh_bank13(int offset, int data) { cpu_bankbase[13][offset] = (byte)data; }
        public static void mwh_bank14(int offset, int data) { cpu_bankbase[14][offset] = (byte)data; }
        public static void mwh_bank15(int offset, int data) { cpu_bankbase[15][offset] = (byte)data; }
        public static void mwh_bank16(int offset, int data) { cpu_bankbase[16][offset] = (byte)data; }
        static mem_write_handler[] bank_write_handler =
{
	mwh_ram,   mwh_bank1,  mwh_bank2,  mwh_bank3,  mwh_bank4,  mwh_bank5,  mwh_bank6,  mwh_bank7,
	mwh_bank8, mwh_bank9,  mwh_bank10, mwh_bank11, mwh_bank12, mwh_bank13, mwh_bank14, mwh_bank15,
	mwh_bank16
};

        static void mwh_error(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %04x: warning - write %02x to unmapped memory address %04x\n",cpu_getactivecpu(),cpu_get_pc(),data,offset);
            cpu_bankbase[0][offset] = (byte)data;
        }
        static void mwh_error_sparse(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: warning - write %02x to unmapped memory address %08x\n",cpu_getactivecpu(),cpu_get_pc(),data,offset);
        }
        static void mwh_error_sparse_bit(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %08x: warning - write %02x to unmapped memory bit addr %08x\n",cpu_getactivecpu(),cpu_get_pc(),data,offset<<3);
        }
        static void mwh_rom(int offset, int data)
        {
            //if (errorlog) fprintf(errorlog,"CPU #%d PC %04x: warning - write %02x to ROM address %04x\n",cpu_getactivecpu(),cpu_get_pc(),data,offset);
        }
        static void mwh_ramrom(int offset, int data)
        {
            cpu_bankbase[0][offset] = cpu_bankbase[0][(uint)(offset + (OP_ROM.offset - OP_RAM.offset))] = (byte)data;
        }

        static void mwh_nop(int offset, int data)
        {
        }
        static int memory_allocate_ext()
        {
            int ext = 0;
            int cpu;

            /* a change for MESS */
            if (Machine.gamedrv.rom == null) return 1;

            /* loop over all CPUs */
            for (cpu = 0; cpu < cpu_gettotalcpu(); cpu++)
            {
                //const struct MemoryReadAddress *mra;
                //const struct MemoryWriteAddress *mwa;

                int region = REGION_CPU1 + cpu;
                int size = memory_region_length(region);

                /* now it's time to loop */
                while (true)
                {
                    int lowest = 0x7fffffff, end, lastend;

                    /* find the base of the lowest memory region that extends past the end */
                    for (int mra=0;mra<Machine.drv.cpu[cpu].memory_read.Length && Machine.drv.cpu[cpu].memory_read[mra].start != -1;mra++)
                        if (Machine.drv.cpu[cpu].memory_read[mra].end >= size && Machine.drv.cpu[cpu].memory_read[mra].start < lowest) lowest = Machine.drv.cpu[cpu].memory_read[mra].start;

                    for (int mwa = 0; mwa < Machine.drv.cpu[cpu].memory_write.Length && Machine.drv.cpu[cpu].memory_write[mwa].start != -1; mwa++)
                        if (Machine.drv.cpu[cpu].memory_write[mwa].end >= size && Machine.drv.cpu[cpu].memory_write[mwa].start < lowest) lowest = Machine.drv.cpu[cpu].memory_read[mwa].start;

                    /* done if nothing found */
                    if (lowest == 0x7fffffff)
                        break;

                    /* now loop until we find the end of this contiguous block of memory */
                    lastend = -1;
                    end = lowest;
                    while (end != lastend)
                    {
                        lastend = end;

                        /* find the base of the lowest memory region that extends past the end */
                        for (int mra = 0; mra < Machine.drv.cpu[cpu].memory_read.Length && Machine.drv.cpu[cpu].memory_read[mra].start != -1; mra++)
                            if (Machine.drv.cpu[cpu].memory_read[mra].start <= end && Machine.drv.cpu[cpu].memory_read[mra].end > end) end = Machine.drv.cpu[cpu].memory_read[mra].end + 1;
                        
                         for (int mwa = 0; mwa < Machine.drv.cpu[cpu].memory_write.Length && Machine.drv.cpu[cpu].memory_write[mwa].start != -1; mwa++)
                            if (Machine.drv.cpu[cpu].memory_write[mwa].start <= end && Machine.drv.cpu[cpu].memory_write[mwa].end > end) end = Machine.drv.cpu[cpu].memory_write[mwa].end + 1;
                        
                    }

                    /* time to allocate */
                    ext_memory[ext].start = lowest;
                    ext_memory[ext].end = end - 1;
                    ext_memory[ext].region = region;
                    ext_memory[ext].data = new _BytePtr(end - lowest);

                    size = ext_memory[ext].end + 1;
                    ext++;
                }
            }

            return 1;
        }
        public static void memory_set_opcode_base(int cpu, _BytePtr _base)
        {
            romptr[cpu] = _base;
        }
        public static byte cpu_readop(uint a) { return OP_ROM.buffer[OP_ROM.offset + a]; }
        public static byte cpu_readop_arg(uint a) { return OP_RAM.buffer[OP_RAM.offset + a]; }

        int memory_init()
        {
            int i, cpu;

            MHELE hardware;
            int abits1, abits2, abits3, abitsmin;
            rdelement_max = 0;
            wrelement_max = 0;
            rdhard_max = HT_USER;
            wrhard_max = HT_USER;

            for (cpu = 0; cpu < MAX_CPU; cpu++)
                cur_mr_element[cpu] = cur_mw_element[cpu] = null;

            ophw = 0xff;

            /* ASG 980121 -- allocate external memory */
            if (memory_allocate_ext() == 0)
                return 0;

            for (cpu = 0; cpu < cpu_gettotalcpu(); cpu++)
            {
                setOPbasefunc[cpu] = null;

                ramptr[cpu] = romptr[cpu] = memory_region(REGION_CPU1 + cpu);

                /* initialize the memory base pointers for memory hooks */
                int _mra = 0;
                if (Machine.drv.cpu[cpu].memory_read != null && Machine.drv.cpu[cpu].memory_read[_mra] != null)
                {
                    while (Machine.drv.cpu[cpu].memory_read[_mra].start != -1)
                    {
                        //				if (_mra.base) *_mra.base = memory_find_base (cpu, _mra.start);
                        //				if (_mra.size) *_mra.size = _mra.end - _mra.start + 1;
                        _mra++;
                    }
                }
                int _mwa = 0;
                if (Machine.drv.cpu[cpu].memory_write != null && Machine.drv.cpu[cpu].memory_write[_mwa] != null)
                {
                    while (Machine.drv.cpu[cpu].memory_write[_mwa].start != -1)
                    {
                        if (Machine.drv.cpu[cpu].memory_write[_mwa]._base != null)
                        {
                            _BytePtr b = memory_find_base(cpu, Machine.drv.cpu[cpu].memory_write[_mwa].start);
                            Machine.drv.cpu[cpu].memory_write[_mwa]._base.buffer = b.buffer;
                            Machine.drv.cpu[cpu].memory_write[_mwa]._base.offset = b.offset;
                        }
                        if (Machine.drv.cpu[cpu].memory_write[_mwa].size != null)
                        {
                            Machine.drv.cpu[cpu].memory_write[_mwa].size[0] = Machine.drv.cpu[cpu].memory_write[_mwa].end - Machine.drv.cpu[cpu].memory_write[_mwa].start + 1;
                        }
                        _mwa++;
                    }
                }

                /* initialize port structures */
                readport_size[cpu] = 0;
                writeport_size[cpu] = 0;
                readport[cpu] = null;
                writeport[cpu] = null;

                /* install port handlers - at least an empty one */
                int ioread = 0;
                if (Machine.drv.cpu[cpu].port_read == null) Machine.drv.cpu[cpu].port_read = empty_readport;

                while (true)
                {
                    if (install_port_read_handler_common(cpu, Machine.drv.cpu[cpu].port_read[ioread].start, Machine.drv.cpu[cpu].port_read[ioread].end, Machine.drv.cpu[cpu].port_read[ioread].handler, 0) == null)
                    {
                        memory_shutdown();
                        return 0;
                    }

                    if (Machine.drv.cpu[cpu].port_read[ioread].start == -1) break;

                    ioread++;
                }


                int iowrite = 0;
                if (Machine.drv.cpu[cpu].port_write == null) Machine.drv.cpu[cpu].port_write = empty_writeport;

                while (true)
                {
                    if (install_port_write_handler_common(cpu, Machine.drv.cpu[cpu].port_write[iowrite].start, Machine.drv.cpu[cpu].port_write[iowrite].end, Machine.drv.cpu[cpu].port_write[iowrite].handler, 0) == null)
                    {
                        memory_shutdown();
                        return 0;
                    }

                    if (Machine.drv.cpu[cpu].port_write[iowrite].start == -1) break;

                    iowrite++;
                }

                portmask[cpu] = 0xffff;
#if HAS_Z80
                if ((Machine.drv.cpu[cpu].cpu_type & ~CPU_FLAGS_MASK) == CPU_Z80 &&
                    (Machine.drv.cpu[cpu].cpu_type & CPU_16BIT_PORT) == 0)
                    portmask[cpu] = 0xff;
#endif
            }

            /* initialize grobal handler */
            for (i = 0; i < MH_HARDMAX; i++)
            {
                memoryreadoffset[i] = 0;
                memorywriteoffset[i] = 0;
            }
            /* bank memory */
            for (i = 1; i <= MAX_BANKS; i++)
            {
                memoryreadhandler[i] = bank_read_handler[i];
                memorywritehandler[i] = bank_write_handler[i];
            }
            /* non map memory */
            memoryreadhandler[HT_NON] = mrh_error;
            memorywritehandler[HT_NON] = mwh_error;
            /* NOP memory */
            memoryreadhandler[HT_NOP] = mrh_nop;
            memorywritehandler[HT_NOP] = mwh_nop;
            /* RAMROM memory */
            memorywritehandler[HT_RAMROM] = mwh_ramrom;
            /* ROM memory */
            memorywritehandler[HT_ROM] = mwh_rom;

            /* if any CPU is 21-bit or more, we change the error handlers to be more benign */
            for (cpu = 0; cpu < cpu_gettotalcpu(); cpu++)
                if (ADDRESS_BITS(cpu) >= 21)
                {
                    memoryreadhandler[HT_NON] = mrh_error_sparse;
                    memorywritehandler[HT_NON] = mwh_error_sparse;
#if HAS_TMS34010
            if ((Machine.drv.cpu[cpu].cpu_type & ~CPU_FLAGS_MASK)==CPU_TMS34010)
			{
				memoryreadhandler[HT_NON] = mrh_error_sparse_bit;
				memorywritehandler[HT_NON] = mwh_error_sparse_bit;
			}
#endif
                }

            for (cpu = 0; cpu < cpu_gettotalcpu(); cpu++)
            {
                /* cpu selection */
                abits1 = (int)ABITS1(cpu);
                abits2 = (int)ABITS2(cpu);
                abits3 = (int)ABITS3(cpu);
                abitsmin = (int)ABITSMIN(cpu);

                /* element shifter , mask set */
                mhshift[cpu, 0] = (abits2 + abits3);
                mhshift[cpu, 1] = abits3;			/* 2nd */
                mhshift[cpu, 2] = 0;				/* 3rd (used by set_element)*/
                mhmask[cpu, 0] = (int)MHMASK(abits1);		/*1st(used by set_element)*/
                mhmask[cpu, 1] = (int)MHMASK(abits2);		/*2nd*/
                mhmask[cpu, 2] = (int)MHMASK(abits3);		/*3rd*/

                /* allocate current element */
                if ((cur_mr_element[cpu] = new MHELE[sizeof(MHELE) << abits1]) == null)
                {
                    memory_shutdown();
                    return 0;
                }
                if ((cur_mw_element[cpu] = new MHELE[sizeof(MHELE) << abits1]) == null)
                {
                    memory_shutdown();
                    return 0;
                }

                /* initialize curent element table */
                for (i = 0; i < (1 << abits1); i++)
                {
                    cur_mr_element[cpu][i] = HT_NON;	/* no map memory */
                    cur_mw_element[cpu][i] = HT_NON;	/* no map memory */
                }

                //memoryread = Machine.drv.cpu[cpu].memory_read;
                //memorywrite = Machine.drv.cpu[cpu].memory_write;

                /* memory read handler build */
                if (Machine.drv.cpu[cpu].memory_read != null)
                {
                    int mra = 0;
                    while (Machine.drv.cpu[cpu].memory_read[mra].start != -1) mra++;
                    mra--;

                    while (mra >= 0)
                    {
                        mem_read_handler handler = Machine.drv.cpu[cpu].memory_read[mra].handler;
                        int _handler = Machine.drv.cpu[cpu].memory_read[mra]._handler;

                        switch (_handler)
                        {
                            case MRA_RAM:
                            case MRA_ROM:
                                hardware = HT_RAM;	/* sprcial case ram read */
                                break;
                            case MRA_BANK1:
                            case MRA_BANK2:
                            case MRA_BANK3:
                            case MRA_BANK4:
                            case MRA_BANK5:
                            case MRA_BANK6:
                            case MRA_BANK7:
                            case MRA_BANK8:
                            case MRA_BANK9:
                            case MRA_BANK10:
                            case MRA_BANK11:
                            case MRA_BANK12:
                            case MRA_BANK13:
                            case MRA_BANK14:
                            case MRA_BANK15:
                            case MRA_BANK16:
                                {
                                    hardware = (byte)((int)MRA_BANK1 - (int)_handler + 1);
                                    memoryreadoffset[hardware] = bankreadoffset[hardware] = Machine.drv.cpu[cpu].memory_read[mra].start;
                                    cpu_bankbase[hardware] = memory_find_base(cpu, Machine.drv.cpu[cpu].memory_read[mra].start);
                                    break;
                                }
                            case MRA_NOP:
                                hardware = HT_NOP;
                                break;
                            default:
                                /* create newer hardware handler */
                                if (rdhard_max == MH_HARDMAX)
                                {
                                    printf("read memory hardware pattern over !\n");
                                    hardware = 0;
                                }
                                else
                                {
                                    /* regist hardware function */
                                    hardware = (byte)rdhard_max++;
                                    memoryreadhandler[hardware] = handler;
                                    memoryreadoffset[hardware] = Machine.drv.cpu[cpu].memory_read[mra].start;
                                }
                                break;
                        }

                        /* hardware element table make */
                        set_element(cpu, new _BytePtr(cur_mr_element[cpu]),
                            (int)(((uint)Machine.drv.cpu[cpu].memory_read[mra].start) >> abitsmin),
                            (int)(((uint)Machine.drv.cpu[cpu].memory_read[mra].end) >> abitsmin),
                            hardware, new _BytePtr(readhardware), ref rdelement_max);

                        mra--;
                    }
                }

                /* memory write handler build */
                if (Machine.drv.cpu[cpu].memory_write != null)
                {
                    int mwa = 0;
                    while (Machine.drv.cpu[cpu].memory_write[mwa].start != -1) mwa++;
                    mwa--;

                    while (mwa >= 0)
                    {
                        mem_write_handler handler = Machine.drv.cpu[cpu].memory_write[mwa].handler;
                        int _handler = Machine.drv.cpu[cpu].memory_write[mwa]._handler;

                        switch (_handler)
                        {
                            case MWA_RAM:
                                hardware = HT_RAM;	/* sprcial case ram write */
                                break;
                            case MWA_BANK1:
                            case MWA_BANK2:
                            case MWA_BANK3:
                            case MWA_BANK4:
                            case MWA_BANK5:
                            case MWA_BANK6:
                            case MWA_BANK7:
                            case MWA_BANK8:
                            case MWA_BANK9:
                            case MWA_BANK10:
                            case MWA_BANK11:
                            case MWA_BANK12:
                            case MWA_BANK13:
                            case MWA_BANK14:
                            case MWA_BANK15:
                            case MWA_BANK16:
                                {
                                    hardware = (byte)((int)MWA_BANK1 - (int)_handler + 1);
                                    memorywriteoffset[hardware] = bankwriteoffset[hardware] = Machine.drv.cpu[cpu].memory_write[mwa].start;
                                    cpu_bankbase[hardware] = memory_find_base(cpu, Machine.drv.cpu[cpu].memory_write[mwa].start);
                                    break;
                                }
                            case MWA_NOP:
                                hardware = HT_NOP;
                                break;
                            case MWA_RAMROM:
                                hardware = HT_RAMROM;
                                break;
                            case MWA_ROM:
                                hardware = HT_ROM;
                                break;
                            default:
                                /* create newer hardware handler */
                                if (wrhard_max == MH_HARDMAX)
                                {
                                    printf("write memory hardware pattern over !\n");
                                    hardware = 0;
                                }
                                else
                                {
                                    /* regist hardware function */
                                    hardware = (byte)wrhard_max++;
                                    memorywritehandler[hardware] = handler;
                                    memorywriteoffset[hardware] = Machine.drv.cpu[cpu].memory_write[mwa].start;
                                }
                                break;
                        }

                        /* hardware element table make */
                        set_element(cpu, new _BytePtr(cur_mw_element[cpu]),
                            (int)(((uint)Machine.drv.cpu[cpu].memory_write[mwa].start) >> abitsmin),
                            (int)(((uint)Machine.drv.cpu[cpu].memory_write[mwa].end) >> abitsmin),
                            hardware, new _BytePtr(writehardware), ref wrelement_max);

                        mwa--;
                    }
                }
            }

            return 1;	/* ok */
        }
        static void set_element(int cpu, _BytePtr celement, int sp, int ep, MHELE type, _BytePtr subelement, ref int ele_max)
        {
            int i;
            int edepth = 0;
            int shift, mask;
            _BytePtr eele = celement;
            _BytePtr sele = celement;
            _BytePtr ele;
            int ss, sb, eb, ee;

            if ((uint)sp > (uint)ep) return;
            do
            {
                mask = mhmask[cpu, edepth];
                shift = mhshift[cpu, edepth];

                /* center element */
                ss = (int)((uint)sp >> shift);
                sb = (int)((uint)sp != 0 ? ((uint)(sp - 1) >> shift) + 1 : 0);
                eb = (int)(((uint)(ep + 1) >> shift) - 1);
                ee = (int)((uint)ep >> shift);

                if (sb <= eb)
                {
                    if ((sb | mask) == (eb | mask))
                    {
                        /* same reasion */
                        ele = (sele != null ? sele : eele);
                        for (i = sb; i <= eb; i++)
                        {
                            ele[i & mask] = type;
                        }
                    }
                    else
                    {
                        if (sele != null) for (i = sb; i <= (sb | mask); i++)
                                sele[i & mask] = type;
                        if (eele != null) for (i = eb & (~mask); i <= eb; i++)
                                eele[i & mask] = type;
                    }
                }

                edepth++;

                if (ss == sb) sele = null;
                else sele = get_element(sele, ss & mask, mhmask[cpu, edepth],
                                            subelement, ref ele_max);
                if (ee == eb) eele = null;
                else eele = get_element(eele, ee & mask, mhmask[cpu, edepth],
                                            subelement, ref ele_max);

            } while (sele != null || eele != null);
        }
        static _BytePtr get_element(_BytePtr element, int ad, int elemask, _BytePtr subelement, ref int ele_max)
        {
            MHELE hw = element[ad];
            int i, ele;
            int banks = (elemask / (1 << MH_SBITS)) + 1;

            if (hw >= MH_HARDMAX) return new _BytePtr(subelement, (hw - MH_HARDMAX) << MH_SBITS);

            /* create new element block */
            if ((ele_max) + banks > MH_ELEMAX)
            {
                printf("memory element size over \n");
                return null;
            }
            /* get new element nunber */
            ele = ele_max;
            (ele_max) += banks;


            /* set link mark to current element */
            element[ad] = (byte)(ele + MH_HARDMAX);
            /* get next subelement top */
            subelement = new _BytePtr(subelement, ele << MH_SBITS);
            /* initialize new block */
            for (i = 0; i < (1 << MH_SBITS); i++)
                subelement[i] = hw;

            return subelement;
        }

        void memory_shutdown()
        {

            for (int cpu = 0; cpu < MAX_CPU; cpu++)
            {
                if (cur_mr_element[cpu] != null)
                {
                    cur_mr_element[cpu] = null;
                }
                if (cur_mw_element[cpu] != null)
                {
                    cur_mw_element[cpu] = null;
                }

                if (readport[cpu] != null)
                {
                    readport[cpu] = null;
                }

                if (writeport[cpu] != null)
                {
                    writeport[cpu] = null;
                }
            }

            /* ASG 980121 -- free all the external memory */
            ext_memory = null;
        }

        IOReadPort[] install_port_read_handler_common(int cpu, int start, int end, mem_read_handler handler, int install_at_beginning)
        {
            int i, oldsize;

            oldsize = readport_size[cpu];
            readport_size[cpu]++;

            if (readport[cpu] == null)
            {
                readport[cpu] = new IOReadPort[readport_size[cpu]];
            }
            else
            {
                Array.Resize<IOReadPort>(ref readport[cpu], readport_size[cpu]);
            }

            if (readport[cpu] == null) return null;

            if (install_at_beginning != 0)
            {
                /* can't do a single memcpy because it doesn't handle overlapping regions correctly??? */
                for (i = oldsize; i >= 1; i--)
                {
                    Array.Copy(readport[cpu], i - 1, readport[cpu], i, 1);
                    //memcpy(&readport[cpu][i], &readport[cpu][i - 1], sizeof(struct IOReadPort));
                }

                i = 0;
            }
            else
            {
                i = oldsize;
            }

            readport[cpu][i] = new IOReadPort();
            readport[cpu][i].start = start;
            readport[cpu][i].end = end;
            readport[cpu][i].handler = handler;

            return readport[cpu];
        }

        IOWritePort[] install_port_write_handler_common(int cpu, int start, int end, mem_write_handler handler, int install_at_beginning)
        {
            int i, oldsize;

            oldsize = writeport_size[cpu];
            writeport_size[cpu]++;

            if (writeport[cpu] == null)
            {
                writeport[cpu] = new IOWritePort[writeport_size[cpu]];
            }
            else
            {
                Array.Resize<IOWritePort>(ref writeport[cpu], writeport_size[cpu]);
            }

            if (writeport[cpu] == null) return null;

            if (install_at_beginning != 0)
            {
                /* can't do a single memcpy because it doesn't handle overlapping regions correctly??? */
                for (i = oldsize; i >= 1; i--)
                {
                    Array.Copy(writeport[cpu], i - 1, writeport[cpu], i, 1);
                    //memcpy(&writeport[cpu][i], &writeport[cpu][i - 1], sizeof(struct IOWritePort));
                }

                i = 0;
            }
            else
            {
                i = oldsize;
            }
            writeport[cpu][i] = new IOWritePort();
            writeport[cpu][i].start = start;
            writeport[cpu][i].end = end;
            writeport[cpu][i].handler = handler;

            return writeport[cpu];
        }
        const byte TYPE_8BIT = 0, TYPE_16BIT_BE = 1, TYPE_16BIT_LE = 2, CAN_BE_MISALIGNED = 0, ALWAYS_ALIGNED = 1;
#if !XBOX
        static int BYTE_XOR_BE(int a) { return a ^ 1; }
        static int BYTE_XOR_LE(int a) { return a; }
#else
        static int BYTE_XOR_BE(int a) { return a ; }
        static int BYTE_XOR_LE(int a){return a^1;}
#endif
        public static int cpu_readmem16(int address)
        {
            MHELE hw = cur_mrhard[(uint)address >> (ABITS2_16 + ABITS_MIN_16)];
            /* for compatibility with setbankhandler, 8-bit systems must call handlers */
            /* for banked memory reads/writes */
            if (hw == HT_RAM)
            {
                //return cpu_bankbase[HT_RAM][address];
                return cpu_bankbase[HT_RAM].buffer[cpu_bankbase[HT_RAM].offset + address];
            }

            /* second-level lookup */
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = readhardware[(hw << MH_SBITS) + (((uint)address >> ABITS_MIN_16) & MHMASK(ABITS2_16))];

                /* for compatibility with setbankhandler, 8-bit systems must call handlers */
                /* for banked memory reads/writes */
                if (hw == HT_RAM)
                    return cpu_bankbase[HT_RAM][address];
            }

            /* fall back to handler */
            return memoryreadhandler[hw](address - memoryreadoffset[hw]);
        }
        public static void cpu_writemem16(int address, int data)
        {
            /* first-level lookup */
            MHELE hw = cur_mwhard[(uint)address >> (ABITS2_16 + ABITS_MIN_16)];

            /* for compatibility with setbankhandler, 8-bit systems must call handlers */
            /* for banked memory reads/writes */
            if (hw == HT_RAM)
            {
                cpu_bankbase[HT_RAM].buffer[cpu_bankbase[HT_RAM].offset + address] = (byte)data;
                return;
            }

            /* second-level lookup */
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = writehardware[(hw << MH_SBITS) + (((uint)address >> ABITS_MIN_16) & MHMASK(ABITS2_16))];

                /* for compatibility with setbankhandler, 8-bit systems must call handlers */
                /* for banked memory reads/writes */
                if (hw == HT_RAM)
                {
                    cpu_bankbase[HT_RAM][address] = (byte)data;
                    return;
                }
            }

            memorywritehandler[hw](address - memorywriteoffset[hw], data);
        }
        public static int cpu_readmem20(int address)
        {
            MHELE hw;
            hw = cur_mrhard[(uint)address >> (ABITS2_20 + ABITS_MIN_20)];
            if (TYPE_8BIT == TYPE_8BIT && hw == HT_RAM)
                return cpu_bankbase[HT_RAM][address];
            else if (TYPE_8BIT != TYPE_8BIT && hw <= HT_BANKMAX)
            {
                if (TYPE_8BIT == TYPE_16BIT_BE)
                    return cpu_bankbase[hw][BYTE_XOR_BE(address) - memoryreadoffset[hw]];
                else if (TYPE_8BIT == TYPE_16BIT_LE)
                    return cpu_bankbase[hw][BYTE_XOR_LE(address) - memoryreadoffset[hw]];
            }
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = readhardware[(hw << MH_SBITS) + (((uint)address >> ABITS_MIN_20) & MHMASK(ABITS2_20))];
                if (TYPE_8BIT == TYPE_8BIT && hw == HT_RAM)
                    return cpu_bankbase[HT_RAM][address];
                else if (TYPE_8BIT != TYPE_8BIT && hw <= HT_BANKMAX)
                {
                    if (TYPE_8BIT == TYPE_16BIT_BE)
                        return cpu_bankbase[hw][BYTE_XOR_BE(address) - memoryreadoffset[hw]];
                    else if (TYPE_8BIT == TYPE_16BIT_LE)
                        return cpu_bankbase[hw][BYTE_XOR_LE(address) - memoryreadoffset[hw]];
                }
            }
            if (TYPE_8BIT == TYPE_8BIT) return memoryreadhandler[hw](address - memoryreadoffset[hw]);
            else
            {
                int shift = (address & 1) << 3;
                int data = memoryreadhandler[hw]((address & ~1) - memoryreadoffset[hw]);
                if (TYPE_8BIT == TYPE_16BIT_BE) return (data >> (shift ^ 8)) & 0xff;
                else if (TYPE_8BIT == TYPE_16BIT_LE) return (data >> shift) & 0xff;
            }
        }

        public static int cpu_readmem201(int address)
        {
            MHELE hw = cur_mrhard[(uint)address >> (ABITS2_20 + ABITS_MIN_20)];
            /* for compatibility with setbankhandler, 8-bit systems must call handlers */
            /* for banked memory reads/writes */
            if (hw == HT_RAM)
            {
                //return cpu_bankbase[HT_RAM][address];
                return cpu_bankbase[HT_RAM].buffer[cpu_bankbase[HT_RAM].offset + address];
            }

            /* second-level lookup */
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = readhardware[(hw << MH_SBITS) + (((uint)address >> ABITS_MIN_20) & MHMASK(ABITS2_20))];

                /* for compatibility with setbankhandler, 8-bit systems must call handlers */
                /* for banked memory reads/writes */
                if (hw == HT_RAM)
                    return cpu_bankbase[HT_RAM][address];
            }

            /* fall back to handler */
            return memoryreadhandler[hw](address - memoryreadoffset[hw]);
        }
        public static void cpu_writemem20(int address, int data)
        {
            /* first-level lookup */
            MHELE hw = cur_mwhard[(uint)address >> (ABITS2_20 + ABITS_MIN_20)];

            /* for compatibility with setbankhandler, 8-bit systems must call handlers */
            /* for banked memory reads/writes */
            if (hw == HT_RAM)
            {
                cpu_bankbase[HT_RAM].buffer[cpu_bankbase[HT_RAM].offset + address] = (byte)data;
                return;
            }

            /* second-level lookup */
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = writehardware[(hw << MH_SBITS) + (((uint)address >> ABITS_MIN_20) & MHMASK(ABITS2_20))];

                /* for compatibility with setbankhandler, 8-bit systems must call handlers */
                /* for banked memory reads/writes */
                if (hw == HT_RAM)
                {
                    cpu_bankbase[HT_RAM][address] = (byte)data;
                    return;
                }
            }

            memorywritehandler[hw](address - memorywriteoffset[hw], data);
        }

        public static int cpu_readport(int port)
        {
            int iorp = 0;

            port &= cur_portmask;

            /* search the handlers. The order is as follows: first the dynamically installed
               handlers are searched, followed by the static ones in whatever order they were
               specified in the driver */
            while (cur_readport[iorp].start != -1)
            {
                if (port >= cur_readport[iorp].start && port <= cur_readport[iorp].end)
                {
                    mem_read_handler handler = cur_readport[iorp].handler;


                    if (handler == null) return 0;
                    else return handler(port - cur_readport[iorp].start);
                }

                iorp++;
            }

            //printf("CPU #%d PC %04x: warning - read unmapped I/O port %02x\n", cpu_getactivecpu(), cpu_get_pc(), port);
            return 0;
        }
        public static void cpu_writeport(int port, int value)
        {
            int iowp = 0;

            port &= cur_portmask;

            /* search the handlers. The order is as follows: first the dynamically installed
               handlers are searched, followed by the static ones in whatever order they were
               specified in the driver */
            while (cur_writeport[iowp].start != -1)
            {
                if (port >= cur_writeport[iowp].start && port <= cur_writeport[iowp].end)
                {
                    mem_write_handler handler = cur_writeport[iowp].handler;


                    if (handler == null) return;
                    else handler(port - cur_writeport[iowp].start, value);

                    return;
                }

                iowp++;
            }

            //printf("CPU #%d PC %04x: warning - write %02x to unmapped I/O port %02x\n", cpu_getactivecpu(), cpu_get_pc(), value, port);
        }
        public static void cpu_setbank(int bank, _BytePtr _base)
        {
            if (bank >= 1 && bank <= MAX_BANKS)
            {
                cpu_bankbase[bank] = _base;
                if (ophw == bank)
                {
                    ophw = 0xff;
                    cpu_setOPbase16((int)cpu_get_pc(), 0);
                }
            }
        }
        public static _BytePtr install_mem_read_handler(int cpu, int start, int end, mem_read_handler handler)
{
	MHELE hardware = 0;
	int abitsmin;
	int i, hw_set;

            abitsmin = (int)ABITSMIN (cpu);

	/* see if this function is already registered */
	hw_set = 0;
	for ( i = 0 ; i < MH_HARDMAX ; i++)
	{
		/* record it if it matches */
		if (( memoryreadhandler[i] == handler ) &&
			(  memoryreadoffset[i] == start))
		{
			printf("handler match - use old one\n");
			hardware =  (byte)i;
			hw_set = 1;
		}
	}            
	if (handler == MRA_RAM_handler || handler == MRA_ROM_handler){hardware = HT_RAM;	/* sprcial case ram read */hw_set = 1;}
	else if (handler == MRA_BANK1_handler){hardware = 1;hw_set = 1;			}
            else if (handler == MRA_BANK2_handler){hardware = 2;hw_set = 1;			}
		else if (handler == MRA_BANK3_handler ){hardware = 3;hw_set = 1;			}
		else if (handler == MRA_BANK4_handler ){hardware = 4;hw_set = 1;			}
		else if (handler == MRA_BANK5_handler){hardware = 5;hw_set = 1;			}
		else if (handler == MRA_BANK6_handler ){hardware = 6;hw_set = 1;			}
		else if (handler == MRA_BANK7_handler ){hardware = 7;hw_set = 1;			}
		else if (handler == MRA_BANK8_handler ){hardware = 8;hw_set = 1;			}
		else if (handler == MRA_BANK9_handler ){hardware = 9;hw_set = 1;			}
		else if (handler == MRA_BANK10_handler ){hardware = 10;hw_set = 1;			}
		else if (handler == MRA_BANK11_handler ){hardware = 11;hw_set = 1;			}
		else if (handler == MRA_BANK12_handler ){hardware = 12;hw_set = 1;			}
		else if (handler == MRA_BANK13_handler ){hardware = 13;hw_set = 1;			}
		else if (handler == MRA_BANK14_handler ){hardware = 14;hw_set = 1;			}
		else if (handler == MRA_BANK15_handler ){hardware = 15;hw_set = 1;			}
		else if (handler == MRA_BANK16_handler){hardware = 16;hw_set = 1;			}            
		
		else if (handler == MRA_NOP_handler)
    {
			hardware = HT_NOP;
			hw_set = 1;
        }
	
	if (hw_set==0)  /* no match */
	{
		/* create newer hardware handler */
		if( rdhard_max == MH_HARDMAX )
		{
			printf( "read memory hardware pattern over !\n");
			printf( "Failed to install new memory handler.\n");
			return memory_find_base(cpu, start);
		}
		else
		{
			/* register hardware function */
			hardware = (byte)rdhard_max++;
			memoryreadhandler[hardware] = handler;
			memoryreadoffset[hardware] = start;
		}
	}
	/* set hardware element table entry */
	set_element( cpu , new _BytePtr(cur_mr_element[cpu]),
		(int)(((uint) start) >> abitsmin) ,
        (int)(((uint)end) >> abitsmin),
		hardware , new _BytePtr(readhardware) , ref rdelement_max );
	//if (errorlog) fprintf(errorlog, "Done installing new memory handler.\n");
	//if (errorlog){fprintf(errorlog,"used read  elements %d/%d , functions %d/%d\n",rdelement_max,MH_ELEMAX , rdhard_max,MH_HARDMAX );}
	return memory_find_base(cpu, start);
}
        public static _BytePtr install_mem_write_handler(int cpu, int start, int end, mem_write_handler handler)
        {
            MHELE hardware = 0;
            int abitsmin;
            int i, hw_set;
            //    if (errorlog) fprintf(errorlog, "Install new memory write handler:\n");
            //    if (errorlog) fprintf(errorlog, "             cpu: %d\n", cpu);
            //    if (errorlog) fprintf(errorlog, "           start: 0x%08x\n", start);
            //    if (errorlog) fprintf(errorlog, "             end: 0x%08x\n", end);
            //#ifdef __LP64__
            //    if (errorlog) fprintf(errorlog, " handler address: 0x%016lx\n", (unsigned long) handler);
            //#else
            //    if (errorlog) fprintf(errorlog, " handler address: 0x%08x\n", (unsigned int) handler);
            //#endif
            abitsmin = (int)ABITSMIN(cpu);

            /* see if this function is already registered */
            hw_set = 0;
            for (i = 0; i < MH_HARDMAX; i++)
            {
                /* record it if it matches */
                if ((memorywritehandler[i] == handler) &&
                    (memorywriteoffset[i] == start))
                {
                    printf("handler match - use old one\n");
                    hardware = (byte)i;
                    hw_set = 1;
                }
            }

            if (handler == MWA_RAM_handler)
            {
                hardware = HT_RAM;	/* sprcial case ram write */
                hw_set = 1;
            }
            else if (handler == MWA_BANK1_handler) { hardware = 1; hw_set = 1; }
            else if (handler == MWA_BANK2_handler) { hardware = 2; hw_set = 1; }
            else if (handler == MWA_BANK3_handler) { hardware = 3; hw_set = 1; }
            else if (handler == MWA_BANK4_handler) { hardware = 4; hw_set = 1; }
            else if (handler == MWA_BANK5_handler) { hardware = 5; hw_set = 1; }
            else if (handler == MWA_BANK6_handler) { hardware = 6; hw_set = 1; }
            else if (handler == MWA_BANK7_handler) { hardware = 7; hw_set = 1; }
            else if (handler == MWA_BANK8_handler) { hardware = 8; hw_set = 1; }
            else if (handler == MWA_BANK9_handler) { hardware = 9; hw_set = 1; }
            else if (handler == MWA_BANK10_handler) { hardware = 10; hw_set = 1; }
            else if (handler == MWA_BANK11_handler) { hardware = 11; hw_set = 1; }
            else if (handler == MWA_BANK12_handler) { hardware = 12; hw_set = 1; }
            else if (handler == MWA_BANK13_handler) { hardware = 13; hw_set = 1; }
            else if (handler == MWA_BANK14_handler) { hardware = 14; hw_set = 1; }
            else if (handler == MWA_BANK15_handler) { hardware = 15; hw_set = 1; }
            else if (handler == MWA_BANK16_handler) { hardware = 16; hw_set = 1; }
            else if (handler == MWA_NOP_handler) { hardware = HT_NOP; hw_set = 1; }
            else if (handler == MWA_RAMROM_handler) { hardware = HT_RAMROM; hw_set = 1; }
            else if (handler == MWA_ROM_handler) { hardware = HT_ROM; hw_set = 1; }
            if (hw_set == 0)  /* no match */
            {
                /* create newer hardware handler */
                if (wrhard_max == MH_HARDMAX)
                {
                    printf("write memory hardware pattern over !\n");
                    printf("Failed to install new memory handler.\n");

                    return memory_find_base(cpu, start);
                }
                else
                {
                    /* register hardware function */
                    hardware = (byte)wrhard_max++;
                    memorywritehandler[hardware] = handler;
                    memorywriteoffset[hardware] = start;
                }
            }
            /* set hardware element table entry */
            set_element(cpu, new _BytePtr(cur_mw_element[cpu]),
                (int)(((uint)start) >> abitsmin),
                (int)(((uint)end) >> abitsmin),
                hardware, new _BytePtr(writehardware), ref wrelement_max);
            printf("Done installing new memory handler.\n");
            printf("used write elements %d/%d , functions %d/%d\n", wrelement_max, MH_ELEMAX, wrhard_max, MH_HARDMAX);

            return memory_find_base(cpu, start);
        }
        public static void cpu_setOPbaseoverride(int cpu, opbase_handler function)
        {
            setOPbasefunc[cpu] = function;
            if (cpu == cpu_getactivecpu())
                OPbasefunc = function;
        }

        public static void cpu_setbankhandler_r(int bank, mem_read_handler handler)
        {
            int offset = 0;
            MHELE hardware;

            if (handler == MRA_RAM_handler || handler == MRA_ROM_handler) { handler = mrh_ram; }
            else if (handler== MRA_NOP_handler){handler = mrh_nop;}
            else if (handler == MRA_BANK1_handler){hardware=1;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK2_handler){hardware=2;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK3_handler){hardware=3;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK4_handler){hardware=4;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK5_handler){hardware=5;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK6_handler){hardware=6;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK7_handler){hardware=7;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK8_handler){hardware=8;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK9_handler){hardware=9;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
               else if (handler ==MRA_BANK10_handler){hardware=10;handler=bank_read_handler[hardware];offset=bankreadoffset[hardware];}
            else if (handler == MRA_BANK11_handler) { hardware = 11; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
            else if (handler == MRA_BANK12_handler) { hardware = 12; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
            else if (handler == MRA_BANK13_handler) { hardware = 13; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
            else if (handler == MRA_BANK14_handler) { hardware = 14; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
            else if (handler == MRA_BANK15_handler) { hardware = 15; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
            else if (handler == MRA_BANK16_handler) { hardware = 16; handler = bank_read_handler[hardware]; offset = bankreadoffset[hardware]; }
                else 
                    offset = bankreadoffset[bank];
            
            memoryreadoffset[bank] = offset;
            memoryreadhandler[bank] = handler;
        }
        public static void cpu_setbankhandler_w(int bank, mem_write_handler handler)
        {
            int offset = 0;
            MHELE hardware;

            if (handler == MWA_RAM_handler) { handler = mwh_ram; }
            else if (handler == MWA_RAMROM_handler) { handler = mwh_ramrom; }
            else if (handler == MWA_ROM_handler) { handler = mwh_rom; }
            else if (handler == MWA_NOP_handler) { handler = mwh_nop; }
            else if (handler == MWA_BANK1_handler) { hardware = 1; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK2_handler) { hardware = 2; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK3_handler) { hardware = 3; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK4_handler) { hardware = 4; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK5_handler) { hardware = 5; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK6_handler) { hardware = 6; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK7_handler) { hardware = 7; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK8_handler) { hardware = 8; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK9_handler) { hardware = 9; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK10_handler) { hardware = 10; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK11_handler) { hardware = 11; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK12_handler) { hardware = 12; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK13_handler) { hardware = 13; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK14_handler) { hardware = 14; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK15_handler) { hardware = 15; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else if (handler == MWA_BANK16_handler) { hardware = 16; handler = bank_write_handler[hardware]; offset = bankwriteoffset[hardware]; }
            else
                offset = bankwriteoffset[bank];

            memorywriteoffset[bank] = offset;
            memorywritehandler[bank] = handler;
        }
    }
}
