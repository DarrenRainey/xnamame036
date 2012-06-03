using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace xnamame036.mame
{
    partial class Mame
    {
        const byte MAX_CONFIG_LINE_SIZE = 48;
        public class mem_range
        {
            public uint cpu, addr, num_bytes, start_value, end_value;
            public mem_range next;
        }
        public class _state
        {
            public bool hiscores_have_been_loaded;
            public mem_range mem_range = null;
        }
        _state state = new _state();
        void computer_writemem_byte(int cpu, int addr, int value)
        {
            int oldcpu = cpu_getactivecpu();
            memorycontextswap(cpu);
            //MEMORY_WRITE(cpu, addr, value);
            cpuintf[Machine.drv.cpu[cpu].cpu_type & ~CPU_FLAGS_MASK].memory_write(addr, value);
            memorycontextswap(oldcpu);
        }

        int computer_readmem_byte(int cpu, int addr)
        {
            int oldcpu = cpu_getactivecpu(), result;
            memorycontextswap(cpu);
            //result = MEMORY_READ(cpu, addr);
            result = cpuintf[Machine.drv.cpu[cpu].cpu_type & ~CPU_FLAGS_MASK].memory_read(addr);
            memorycontextswap(oldcpu);
            return result;
        }

        void hs_init()
        {
            mem_range mem_range = state.mem_range;
            state.hiscores_have_been_loaded = false;

            while (mem_range != null)
            {
                computer_writemem_byte(
                    (int)mem_range.cpu,
                    (int)mem_range.addr,
                    (int)~mem_range.start_value
                );

                computer_writemem_byte(
                    (int)mem_range.cpu,
                    (int)(mem_range.addr + mem_range.num_bytes - 1),
                    (int)~mem_range.end_value
                );
                mem_range = mem_range.next;
            }
        }
        void hs_open(string name)
        {
            if (File.Exists("hiscore.dat"))
            {
                throw new Exception();
            }
        }
        void hs_close()
        {
            if (state.hiscores_have_been_loaded) hs_save();
            hs_free();
        }
        void hs_save()
        {
            throw new Exception();
        }
        void hs_free()
        {
            state.mem_range = null;
        }
        bool safe_to_load()
        {
            mem_range mem_range = state.mem_range;
            while (mem_range != null)
            {
                if (computer_readmem_byte((int)mem_range.cpu, (int)mem_range.addr) !=
                    mem_range.start_value)
                {
                    return false;
                }
                if (computer_readmem_byte((int)mem_range.cpu, (int)(mem_range.addr + mem_range.num_bytes - 1)) !=
                    mem_range.end_value)
                {
                    return false;
                }
                mem_range = mem_range.next;
            }
            return true;
        }
        void hs_update()
        {
            if (state.mem_range != null)
            {
                if (!state.hiscores_have_been_loaded)
                {
                    if (safe_to_load()) hs_load();
                }
            }
        }
        void hs_load()
        {
            object f = osd_fopen(Machine.gamedrv.name, null, OSD_FILETYPE_HIGHSCORE, 0);
            state.hiscores_have_been_loaded = true;
            if (f != null)
            {
                throw new Exception();
            }
        }
    }
}
