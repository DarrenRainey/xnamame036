using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static Scheduler scheduler = new Scheduler();

        class Scheduler
        {
            public struct Clock
            {
                public uint cpu_freq;
                public uint cop_freq;
                public uint smp_freq;

                public long cpucop;
                public long cpuppu;
                public long cpusmp;
                public long smpdsp;
            }
            public Clock clock;
            enum sync_t { SyncNone, SyncCpu, SyncAll };
            sync_t sync;
            object thread_snes, thread_cpu, thread_cop, thread_smp, thread_ppu, thread_dsp, thread_active;
            public void co_switch(object t)
            {
                throw new Exception();
            }
            public void addclocks_cpu(uint clocks)
            {
                clock.cpucop -= (long)(clocks * (ulong)clock.cop_freq);
                clock.cpuppu -= clocks;
                clock.cpusmp -= (long)(clocks * (ulong)clock.smp_freq);
            }
            public void sync_cpucop()
            {
                if (clock.cpucop < 0)
                {
                    thread_active = thread_cop;
                    co_switch(thread_cop);
                }
            }
            public void sync_copcpu()
            {
                if (clock.cpucop >= 0 && sync != sync_t.SyncAll)
                {
                    thread_active = thread_cpu;
                    co_switch(thread_cpu);
                }
            }
            public void sync_cpuppu()
            {
                if (clock.cpuppu < 0)
                {
                    thread_active = thread_ppu;
                    co_switch(thread_ppu);
                }
            }
            public void sync_ppucpu()
            {
                if (clock.cpuppu >= 0 && sync != sync_t.SyncAll)
                {
                    thread_active = thread_cpu;
                    co_switch(thread_cpu);
                }
            }
            public void sync_cpusmp()
            {
                if (clock.cpusmp < 0)
                {
                    thread_active = thread_smp;
                    co_switch(thread_smp);
                }
            }
            public void sync_smpcpu()
            {
                if (clock.cpusmp >= 0 && sync != sync_t.SyncAll)
                {
                    thread_active = thread_cpu;
                    co_switch(thread_cpu);
                }
            }
            void sync_smpdsp()
            {
                if (clock.smpdsp < 0 && sync != sync_t.SyncAll)
                {
                    thread_active = thread_dsp;
                    co_switch(thread_dsp);
                }
            }
            public void sync_dspsmp()
            {
                if (clock.smpdsp >= 0 && sync != sync_t.SyncAll)
                {
                    thread_active = thread_smp;
                    co_switch(thread_smp);
                }
            }
        }
    }
}