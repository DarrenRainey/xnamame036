using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public class Timer
        {
            public const int SUSPEND_REASON_HALT = 0x0001;
            public const int SUSPEND_REASON_RESET = 0x0002;
            public const int SUSPEND_REASON_SPIN = 0x0004;
            public const int SUSPEND_REASON_TRIGGER = 0x0008;
            public const int SUSPEND_REASON_DISABLE = 0x0010;
            public const int SUSPEND_ANY_REASON = -1;

            public const double TIME_NOW = 0.0;
            public const double TIME_NEVER = 1.0e30;

            const int MAX_TIMERS = 256;

            public static double TIME_IN_USEC(double us) { return ((double)(us) * (1.0 / 1000000.0)); }
            public static double TIME_IN_NSEC(double us) { return ((double)(us) * (1.0 / 1000000000.0)); }
            public static double TIME_IN_MSEC(double ms) { return ((double)(ms) * (1.0 / 1000.0)); }
            public static double TIME_IN_HZ(double hz) { return 1.0 / hz; }
            public delegate void timer_callback(int i);

            public class timer_entry
            {
                public timer_entry next, prev;
                public timer_callback callback;
                public int callback_param;
                public int enabled;
                public double period, start, expire;
            }
            public delegate void burn_delegate(int cycles);
            public class cpu_entry
            {
                public int[] icount;
                public burn_delegate burn;
                public int index;
                public int suspended;
                public int trigger;
                public int nocount;
                public int lost;
                public double time;
                public double sec_to_cycles;
                public double cycles_to_sec;
                public double overclock;
            }

            /* conversion constants */
            static double[] cycles_to_sec = new double[MAX_CPU];
            static double[] sec_to_cycles = new double[MAX_CPU];

            /* list of per-CPU timer data */
            static cpu_entry[] cpudata = new cpu_entry[MAX_CPU + 1];
            static int lastcpu;
            static int activecpu;
            static int last_activecpu;

            /* list of active timers */
            static timer_entry[] timers = new timer_entry[MAX_TIMERS];
            static timer_entry timer_head;
            static timer_entry timer_free_head;

            /* other internal states */
            static double base_time;
            static double global_offset;
            static timer_entry callback_timer;
            static int callback_timer_modified;

            /*
 *		return the current absolute time
 */
            public static double getabsolutetime()
            {
                if (activecpu > 0 && (cpudata[activecpu].icount[0] + cpudata[activecpu].lost) > 0)
                    return base_time - ((double)(cpudata[activecpu].icount[0] + cpudata[activecpu].lost) * cpudata[activecpu].cycles_to_sec);
                else
                    return base_time;
            }


            /*
             *		adjust the current CPU's timer so that a new event will fire at the right time
             */
            public static void timer_adjust(timer_entry timer, double time, double period)
            {
                int newicount, diff;

                /* compute a new icount for the current CPU */
                if (period == TIME_NOW)
                    newicount = 0;
                else
                    newicount = (int)((timer.expire - time) * cpudata[activecpu].sec_to_cycles) + 1;

                /* determine if we're scheduled to run more cycles */
                diff = cpudata[activecpu].icount[0] - newicount;

                /* if so, set the new icount and compute the amount of "lost" time */
                if (diff > 0)
                {
                    cpudata[activecpu].lost += diff;
                    if (cpudata[activecpu].burn != null)
                        cpudata[activecpu].burn(diff);  /* let the CPU burn the cycles */
                    else
                        cpudata[activecpu].icount[0] = newicount;  /* CPU doesn't care */
                }
            }


            /*
             *		allocate a new timer
             */
            public static timer_entry timer_new()
            {
                timer_entry timer;

                /* remove an empty entry */
                if (timer_free_head == null)
                    return null;
                timer = timer_free_head;
                timer_free_head = timer.next;

                return timer;
            }


            /*
             *		insert a new timer into the list at the appropriate location
             */
            public static void timer_list_insert(timer_entry timer)
            {
                double expire = timer.enabled != 0 ? timer.expire : TIME_NEVER;
                timer_entry t, lt = null;

                /* loop over the timer list */
                for (t = timer_head; t != null; lt = t, t = t.next)
                {
                    /* if the current list entry expires after us, we should be inserted before it */
                    /* note that due to floating point rounding, we need to allow a bit of slop here */
                    /* because two equal entries -- within rounding precision -- need to sort in */
                    /* the order they were inserted into the list */
                    if ((t.expire - expire) > TIME_IN_NSEC(1))
                    {
                        /* link the new guy in before the current list entry */
                        timer.prev = t.prev;
                        timer.next = t;

                        if (t.prev != null)
                            t.prev.next = timer;
                        else
                            timer_head = timer;
                        t.prev = timer;
                        return;
                    }
                }

                /* need to insert after the last one */
                if (lt != null)
                    lt.next = timer;
                else
                    timer_head = timer;
                timer.prev = lt;
                timer.next = null;
            }
            public static void timer_list_remove(timer_entry timer)
            {
                /* remove it from the list */
                if (timer.prev != null)
                    timer.prev.next = timer.next;
                else
                    timer_head = timer.next;
                if (timer.next != null)
                    timer.next.prev = timer.prev;
            }
            public static void timer_init()
            {
                /* keep a local copy of how many total CPU's */
                lastcpu = cpu_gettotalcpu() - 1;

                /* we need to wait until the first call to timer_cyclestorun before using real CPU times */
                base_time = 0.0;
                global_offset = 0.0;
                callback_timer = null;
                callback_timer_modified = 0;

                /* reset the timers */
                //memset(timers, 0, sizeof(timers));
                for (int i = 0; i < MAX_TIMERS; i++)
                    timers[i] = new timer_entry();

                /* initialize the lists */
                timer_head = null;
                timer_free_head = timers[0];
                for (int i = 0; i < MAX_TIMERS - 1; i++)
                    timers[i].next = timers[i + 1];

                /* reset the CPU timers */
                //memset(cpudata, 0, sizeof(cpudata));
                for (int i = 0; i < cpudata.Length; i++)
                    cpudata[i] = new cpu_entry();
                activecpu = -1;
                last_activecpu = lastcpu;

                /* compute the cycle times */
                for (int i = 0; i <= lastcpu; i++)
                {
                    /* make a pointer to this CPU's interface functions */
                    cpudata[i].icount = cpuintf[Machine.drv.cpu[i].cpu_type & ~CPU_FLAGS_MASK].icount;
                    cpudata[i].burn = cpuintf[Machine.drv.cpu[i].cpu_type & ~CPU_FLAGS_MASK].burn;

                    /* get the CPU's overclocking factor */
                    cpudata[i].overclock = cpuintf[Machine.drv.cpu[i].cpu_type & ~CPU_FLAGS_MASK].overclock;

                    /* everyone is active but suspended by the reset line until further notice */
                    cpudata[i].suspended = SUSPEND_REASON_RESET;

                    /* set the CPU index */
                    cpudata[i].index = i;

                    /* compute the cycle times */
                    cpudata[i].sec_to_cycles = sec_to_cycles[i] = cpudata[i].overclock * Machine.drv.cpu[i].cpu_clock;
                    cpudata[i].cycles_to_sec = cycles_to_sec[i] = 1.0 / sec_to_cycles[i];
                }
            }


            public static double timer_get_overclock(int cpunum)
            {
                cpu_entry cpu = cpudata[cpunum];
                return cpu.overclock;
            }

            public static void timer_set_overclock(int cpunum, double overclock)
            {
                cpu_entry cpu = cpudata[cpunum];
                cpu.overclock = overclock;
                cpu.sec_to_cycles = sec_to_cycles[cpunum] = cpu.overclock * Machine.drv.cpu[cpunum].cpu_clock;
                cpu.cycles_to_sec = cycles_to_sec[cpunum] = 1.0 / sec_to_cycles[cpunum];
            }

            public static timer_entry timer_pulse(double period, int param, timer_callback callback)
            {
                double time = getabsolutetime();
                timer_entry timer;

                /* allocate a new entry */
                timer = timer_new();
                if (timer == null)
                    return null;

                /* fill in the record */
                timer.callback = callback;
                timer.callback_param = param;
                timer.enabled = 1;
                timer.period = period;

                /* compute the time of the next firing and insert into the list */
                timer.start = time;
                timer.expire = time + period;
                timer_list_insert(timer);

                /* if we're supposed to fire before the end of this cycle, adjust the counter */
                if (activecpu != 0 && timer.expire < base_time)
                    timer_adjust(timer, time, period);

#if VERBOSE
		verbose_print("T=%.6g: New pulse=%08X, period=%.6g\n", time + global_offset, timer, period);
#endif

                /* return a handle */
                return timer;
            }

            public static timer_entry timer_set(double duration, int param, timer_callback callback)
            {
                double time = getabsolutetime();
                timer_entry timer;

                /* allocate a new entry */
                timer = timer_new();
                if (timer == null)
                    return null;

                /* fill in the record */
                timer.callback = callback;
                timer.callback_param = param;
                timer.enabled = 1;
                timer.period = 0;

                /* compute the time of the next firing and insert into the list */
                timer.start = time;
                timer.expire = time + duration;
                timer_list_insert(timer);

                /* if we're supposed to fire before the end of this cycle, adjust the counter */
                if (activecpu != 0 && timer.expire < base_time)
                    timer_adjust(timer, time, duration);

#if VERBOSE
		verbose_print("T=%.6g: New oneshot=%08X, duration=%.6g\n", time + global_offset, timer, duration);
#endif

                /* return a handle */
                return timer;
            }

            public static void timer_reset(object which, double duration)
            {
                double time = getabsolutetime();
                timer_entry timer = (timer_entry)which;

                /* compute the time of the next firing */
                timer.start = time;
                timer.expire = time + duration;

                /* remove the timer and insert back into the list */
                timer_list_remove(timer);
                timer_list_insert(timer);

                /* if we're supposed to fire before the end of this cycle, adjust the counter */
                if (activecpu != 0 && timer.expire < base_time)
                    timer_adjust(timer, time, duration);

                /* if this is the callback timer, mark it modified */
                if (timer == callback_timer)
                    callback_timer_modified = 1;

#if VERBOSE
		verbose_print("T=%.6g: Reset %08X, duration=%.6g\n", time + global_offset, timer, duration);
#endif
            }

            public static void timer_remove(object which)
            {
                timer_entry timer = (timer_entry)which;

                /* remove it from the list */
                timer_list_remove(timer);

                /* free it up by adding it back to the free list */
                timer.next = timer_free_head;
                timer_free_head = timer;

#if VERBOSE
		verbose_print("T=%.6g: Removed %08X\n", getabsolutetime() + global_offset, timer);
#endif
            }

            public static int timer_enable(timer_entry which, int enable)
            {
                timer_entry timer = (timer_entry)which;
                int old;

#if VERBOSE
		if (enable) verbose_print("T=%.6g: Enabled %08X\n", getabsolutetime() + global_offset, timer);
		else verbose_print("T=%.6g: Disabled %08X\n", getabsolutetime() + global_offset, timer);
#endif

                /* set the enable flag */
                old = timer.enabled;
                timer.enabled = enable;

                /* remove the timer and insert back into the list */
                timer_list_remove(timer);
                timer_list_insert(timer);

                return old;
            }

            public static double timer_timeelapsed(timer_entry which)
            {
                double time = getabsolutetime();
                timer_entry timer = (timer_entry)which;

                return time - timer.start;
            }

            public static double timer_timeleft(timer_entry which)
            {
                double time = getabsolutetime();
                timer_entry timer = (timer_entry)which;

                return timer.expire - time;
            }

            public static double timer_get_time()
            {
                return global_offset + getabsolutetime();
            }

            public static double timer_starttime(timer_entry which)
            {
                timer_entry timer = (timer_entry)which;
                return global_offset + timer.start;
            }

            public static double timer_firetime(timer_entry which)
            {
                timer_entry timer = (timer_entry)which;
                return global_offset + timer.expire;
            }

            public static bool timer_schedule_cpu(ref int cpu, ref int cycles)
            {
                double end;

                /* then see if there are any CPUs that aren't suspended and haven't yet been updated */
                if (pick_cpu(ref cpu, ref cycles, timer_head.expire) )
                    return true;

                /* everyone is up-to-date; expire any timers now */
                end = timer_head.expire;
                while (timer_head.expire <= end)
                {
                    timer_entry timer = timer_head;

                    /* the base time is now the time of the timer */
                    base_time = timer.expire;

                    /* set the global state of which callback we're in */
                    callback_timer_modified = 0;
                    callback_timer = timer;

                    /* call the callback */
                    if (timer.callback != null)
                    {
                        timer.callback(timer.callback_param);
                    }

                    /* clear the callback timer global */
                    callback_timer = null;

                    /* reset or remove the timer, but only if it wasn't modified during the callback */
                    if (callback_timer_modified == 0)
                    {
                        if (timer.period != 0)
                        {
                            timer.start = timer.expire;
                            timer.expire += timer.period;

                            timer_list_remove(timer);
                            timer_list_insert(timer);
                        }
                        else
                            timer_remove(timer);
                    }
                }

                /* reset scheduling so it starts with CPU 0 */
                last_activecpu = lastcpu;

                /* go back to scheduling */
                return pick_cpu(ref cpu, ref cycles, timer_head.expire);
            }

            public static void timer_update_cpu(int cpunum, int ran)
            {
                cpu_entry cpu = cpudata[cpunum];

                /* update the time if we haven't been suspended */
                if (cpu.suspended == 0)
                {
                    cpu.time += (double)(ran - cpu.lost) * cpu.cycles_to_sec;
                    cpu.lost = 0;
                }

                /* time to renormalize? */
                if (cpu.time >= 1.0)
                {
                    timer_entry timer;
                    double one = 1.0;
                    int c;


                    /* renormalize all the CPU timers */
                    for (c = 0; c <= lastcpu; c++)
                        cpudata[c].time -= one;

                    /* renormalize all the timers' times */
                    for (timer = timer_head; timer != null; timer = timer.next)
                    {
                        timer.start -= one;
                        timer.expire -= one;
                    }

                    /* renormalize the global timers */
                    global_offset += one;
                }

                /* now stop counting cycles */
                base_time = cpu.time;
                activecpu = -1;
            }

            public static void timer_suspendcpu(int cpunum, int suspend, int reason)
            {
                //cpu_entry cpu = cpudata[cpunum];
                int nocount = cpudata[cpunum].nocount;
                int old = cpudata[cpunum].suspended;

#if VERBOSE
		if (suspend) verbose_print("T=%.6g: Suspending CPU %d\n", getabsolutetime() + global_offset, cpunum);
		else verbose_print("T=%.6g: Resuming CPU %d\n", getabsolutetime() + global_offset, cpunum);
#endif

                /* mark the CPU */
                if (suspend != 0)
                    cpudata[cpunum].suspended |= reason;
                else
                    cpudata[cpunum].suspended &= ~reason;
                cpudata[cpunum].nocount = 0;

                /* if this is the active CPU and we're halting, stop immediately */
                if (activecpu != 0 && cpunum == activecpu && old == 0 && cpudata[cpunum].suspended != 0)
                {
                    /* set the CPU's time to the current time */
                    cpudata[cpunum].time = base_time = getabsolutetime();	/* ASG 990225 - also set base_time */
                    cpudata[cpunum].lost = 0;

                    /* no more instructions */
                    if (cpudata[cpunum].burn != null)
                        cpudata[cpunum].burn(cpudata[cpunum].icount[0]); /* let the CPU burn the cycles */
                    else
                        cpudata[cpunum].icount[0] = 0;	/* CPU doesn't care */
                }

                /* else if we're unsuspending a CPU, reset its time */
                else if (old != 0 && cpudata[cpunum].suspended == 0 && nocount == 0)
                {
                    double time = getabsolutetime();

                    /* only update the time if it's later than the CPU's time */
                    if (time > cpudata[cpunum].time)
                        cpudata[cpunum].time = time;
                    cpudata[cpunum].lost = 0;
                }
            }

            public static void timer_holdcpu(int cpunum, int hold, int reason)
            {
                cpu_entry cpu = cpudata[cpunum];

                /* same as suspend */
                timer_suspendcpu(cpunum, hold, reason);

                /* except that we don't count time */
                if (hold != 0)
                    cpu.nocount = 1;
            }

            public static bool timer_iscpususpended(int cpunum, int reason)
            {
                cpu_entry cpu = cpudata[cpunum];
                return ((cpu.suspended & reason) != 0 && cpu.nocount == 0) ? true : false;
            }

            public static int timer_iscpuheld(int cpunum, int reason)
            {
                cpu_entry cpu = cpudata[cpunum];
                return ((cpu.suspended & reason) != 0 && cpu.nocount != 0) ? 1 : 0;
            }

            public static void timer_suspendcpu_trigger(int cpunum, int trigger)
            {
                cpu_entry cpu = cpudata[cpunum];

                /* suspend the CPU immediately if it's not already */
                timer_suspendcpu(cpunum, 1, SUSPEND_REASON_TRIGGER);

                /* set the trigger */
                cpu.trigger = trigger;
            }

            public static void timer_holdcpu_trigger(int cpunum, int trigger)
            {
                cpu_entry cpu = cpudata[cpunum];

                /* suspend the CPU immediately if it's not already */
                timer_holdcpu(cpunum, 1, SUSPEND_REASON_TRIGGER);

                /* set the trigger */
                cpu.trigger = trigger;
            }

            public static void timer_trigger(int trigger)
            {
                int cpu;

                /* cause an immediate resynchronization */
                if (activecpu !=-1)
                {
                    int left = cpudata[activecpu].icount[0];
                    if (left > 0)
                    {
                        cpudata[activecpu].lost += left;
                        if (cpudata[activecpu].burn != null)
                            cpudata[activecpu].burn(left); /* let the CPU burn the cycles */
                        else
                            cpudata[activecpu].icount[0] = 0; /* CPU doesn't care */
                    }
                }

                /* look for suspended CPUs waiting for this trigger and unsuspend them */
                for (cpu = 0; cpu <= lastcpu; cpu++)
                {
                    if (cpudata[cpu].suspended != 0 && cpudata[cpu].trigger == trigger)
                    {
                        timer_suspendcpu(cpudata[cpu].index, 0, SUSPEND_REASON_TRIGGER);
                        cpudata[cpu].trigger = 0;
                    }
                }
            }

            public static bool pick_cpu(ref int cpunum, ref int cycles, double end)
            {
                int cpu = last_activecpu;

                /* look for a CPU that isn't suspended and hasn't run its full timeslice yet */
                do
                {
                    /* wrap around */
                    cpu++;
                    if (cpu > lastcpu)
                        cpu = 0;

                    /* if this CPU is suspended, just bump its time */
                    if (cpudata[cpu].suspended != 0)
                    {
                        /* ASG 990225 - defer this update until the slice has finished */
                        /*			if (!cpu.nocount)
                                    {
                                        cpu.time = end;
                                        cpu.lost = 0;
                                    }*/
                    }

                    /* if this CPU isn't suspended and has time left.... */
                    else if (cpudata[cpu].time < end)
                    {
                        /* mark the CPU active, and remember the CPU number locally */
                        activecpu = last_activecpu = cpu;

                        /* return the number of cycles to execute and the CPU number */
                        cpunum = cpudata[cpu].index;
                        cycles = (int)((double)(end - cpudata[cpu].time) * cpudata[cpu].sec_to_cycles);

                        if (cycles > 0)
                        {
                            /* remember the base time for this CPU */
                            base_time = cpudata[cpu].time + ((double)cycles * cpudata[cpu].cycles_to_sec);

                            return true;
                        }
                    }
                }
                while (cpu != last_activecpu);

                /* ASG 990225 - bump all suspended CPU times after the slice has finished */
                for (cpu = 0; cpu <= lastcpu; cpu++)
                    if (cpudata[cpu].suspended != 0 && cpudata[cpu].nocount == 0)
                    {
                        cpudata[cpu].time = end;
                        cpudata[cpu].lost = 0;
                    }

                return false;
            }
            public static void timer_remove(timer_entry timer)
            {
                timer_list_remove(timer);
                timer.next = timer_free_head;
                timer_free_head = timer;
            }
        }
    }
}