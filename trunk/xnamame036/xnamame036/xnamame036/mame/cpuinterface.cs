using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int TRIGGER_TIMESLICE = -1000, TRIGGER_INT = -2000, TRIGGER_YIELDTIME = -3000, TRIGGER_SUSPENDTIME = -4000;

        public const int CPU_Z80 = 1;
        public const int CPU_8080 = 2;
        public const int CPU_I8039 = 3;
        public const int CPU_M6800 = 4;
        public const int CPU_M6802 = 5;
        public const int CPU_M6803 = 6;
        public const int CPU_M6805 = 7;
        public const byte CPU_M6808 = 8;

        public const int CPU_M6809 = 9;

        public const int CPU_HD63701 = 10;

        public const int CPU_M68705 = 11;
        public const int CPU_HD63705 = 12;
        public const int CPU_KONAMI = 13;
        public const int CPU_M6502 = 14;
        public const byte CPU_CCPU = 15;
        public const byte CPU_V30 = 16;
        public const int CPU_M68000 = 17;
        public const byte CPU_HD6309 = 18;
        public const int CPU_I8035 = 19;






        public const byte CPU_M6510 = 120;
        public const byte CPU_M6801 = 121;
        public const byte CPU_M65C02 = 122;
        
        public const byte CPU_M65CE02 = 124;
        public const byte CPU_M65SC02 = 125;
        public const byte CPU_N2A03 = 126;
        public const byte CPU_V33 = 130;
        public const byte CPU_V20 = 131;
        public const byte CPU_N7751 = 132;
        public const byte CPU_I8048 = 133;
        public const byte CPU_I86 = 117;
        public const int CPU_M68010 = 118;

        public const int CPU_M6509 = 119;


        public const byte Z80_MAXDAISY = 4;
        public const byte Z80_INT_REQ = 0x01;
        public const byte Z80_INT_IEO = 0x02;

        public const int REG_PREVIOUSPC = -1;
        public const int REG_SP_CONTENTS = -2;

        public delegate int irqcallback(int irqline);
        public delegate int timedinteerruptcallback();
        public abstract class cpu_interface
        {
            public uint cpu_num;
            public abstract void reset(object param);
            public abstract void exit();
            public abstract int execute(int cycles);
            public abstract void burn(int cycles);
            public abstract void create_context(ref object reg);
            public abstract uint get_context(ref object reg);
            public abstract void set_context(object reg);
            public abstract uint get_pc();
            public abstract void set_pc(uint val);
            public abstract uint get_sp();
            public abstract void set_sp(uint val);
            public abstract uint get_reg(int regnum);
            public abstract void set_reg(int regnum, uint val);
            public abstract void set_nmi_line(int linestate);
            public abstract void set_irq_line(int irqline, int linestate);
            public abstract void set_irq_callback(irqcallback callback);
            public abstract void internal_interrupt(int type);
            public abstract void cpu_state_save(object file);
            public abstract void cpu_state_load(object file);
            public abstract string cpu_info(object context, int regnum);
            public abstract uint cpu_dasm(ref string buffer, uint pc);
            public uint num_irqs;
            public int default_vector;
            public int[] icount;
            public double overclock;
            public int no_int, irq_int, nmi_int;
            public abstract int memory_read(int offset);
            public abstract void memory_write(int offset, int data);
            public abstract void set_op_base(int pc);
            public int address_shift;
            public uint address_bits, endianess, align_unit, max_inst_len;
            public uint abits1, abits2, abitsmin;
        };

        static cpu_interface[] cpuintf = {
                                             new cpu_Dummy(),
                                             new cpu_Z80(),
                                             new cpu_i8085(),
                                             new cpu_i8039(),
                                             new cpu_m6800(),
                                             new cpu_m6802(),
                                             new cpu_m6803(),
                                             new cpu_m6805(),
                                             new cpu_m6808(),
                                             new cpu_m6809(),
                                             new cpu_hd63701(),
                                             new cpu_m68705(),
                                             new cpu_hd63705(),
                                             new cpu_konami(),
                                             new cpu_m6502(),
                                             new cpu_ccpu(),
                                             new cpu_v30(),
                                             new cpu_m68000(),
                                             new cpu_hd6309(),
                                             new cpu_i8035(),
                                             //new cpu_i86(),
                                             
                                             
                                             
                                             

                                         };
        static int CPU_COUNT = cpuintf.Length;
        class cpuinfo
        {
            public cpuinfo(cpu_interface intf) { this.intf = intf; }
            public cpu_interface intf; 	/* pointer to the interface functions */
            public int iloops; 					/* number of interrupts remaining this frame */
            public int totalcycles;				/* total CPU cycles executed */
            public int vblankint_countdown;		/* number of vblank callbacks left until we interrupt */
            public int vblankint_multiplier;		/* number of vblank callbacks per interrupt */
            public object vblankint_timer;			/* reference to elapsed time counter */
            public double vblankint_period;		/* timing period of the VBLANK interrupt */
            public object timedint_timer;			/* reference to this CPU's timer */
            public double timedint_period; 		/* timing period of the timed interrupt */
            public object context;					/* dynamically allocated context buffer */
            public bool save_context;				/* need to context switch this CPU? yes or no */
        }
        public const int MAX_IRQ_LINES = 8;
        public const int NEW_INTERRUPT_SYSTEM = 1;
        public const int CLEAR_LINE = 0;
        public const int ASSERT_LINE = 1;
        public const int HOLD_LINE = 2;
        public const int PULSE_LINE = 3;
        public const int MAX_REGS = 64;
        const int CPU_INFO_REG = 0;
        const int CPU_INFO_FLAGS = MAX_REGS;
        const int CPU_INFO_NAME = MAX_REGS + 1;
        const int CPU_INFO_FAMILY = MAX_REGS + 2;
        const int CPU_INFO_VERSION = MAX_REGS + 3;
        const int CPU_INFO_FILE = MAX_REGS + 4;
        const int CPU_INFO_CREDITS = MAX_REGS + 5;

        const int CPU_IS_LE = 0, CPU_IS_BE = 1;

        static List<cpuinfo> cpu = new List<cpuinfo>();

        static int activecpu, totalcpu;
        static int cycles_running;	/* number of cycles that the CPU emulation was requested to run */
        /* (needed by cpu_getfcount) */
        static bool have_to_reset;

        static int[] interrupt_enable = new int[MAX_CPU];
        static int[] interrupt_vector = new int[MAX_CPU];

        static int[] irq_line_state = new int[MAX_CPU * MAX_IRQ_LINES];
        static int[] irq_line_vector = new int[MAX_CPU * MAX_IRQ_LINES];

        static int watchdog_counter;

        static Timer.timer_entry vblank_timer;
        static int vblank_countdown;
        static int vblank_multiplier;
        static double vblank_period;

        static Timer.timer_entry refresh_timer;
        static double refresh_period;
        static double refresh_period_inv;

        static Timer.timer_entry timeslice_timer;
        static double timeslice_period;

        static double scanline_period;
        static double scanline_period_inv;

        static int usres; /* removed from cpu_run and made global */
        static int vblank;
        static int current_frame;

        //public delegate int irqcallback(int i);
        static irqcallback[] cpu_irq_callbacks = {
	cpu_0_irq_callback,
	cpu_1_irq_callback,
	cpu_2_irq_callback,
	cpu_3_irq_callback
};

        static int cpu_0_irq_callback(int irqline)
        {
            if (irq_line_state[0 * MAX_IRQ_LINES + irqline] == HOLD_LINE)
            {
                SETIRQLINE(0, irqline, CLEAR_LINE);
                irq_line_state[0 * MAX_IRQ_LINES + irqline] = CLEAR_LINE;
            }
            //LOG((errorlog, "cpu_0_irq_callback(%d) $%04x\n", irqline, irq_line_vector[0 * MAX_IRQ_LINES + irqline]));
            return irq_line_vector[0 * MAX_IRQ_LINES + irqline];
        }

        static int cpu_1_irq_callback(int irqline)
        {
            if (irq_line_state[1 * MAX_IRQ_LINES + irqline] == HOLD_LINE)
            {
                SETIRQLINE(1, irqline, CLEAR_LINE);
                irq_line_state[1 * MAX_IRQ_LINES + irqline] = CLEAR_LINE;
            }
            //LOG((errorlog, "cpu_1_irq_callback(%d) $%04x\n", irqline, irq_line_vector[1 * MAX_IRQ_LINES + irqline]));
            return irq_line_vector[1 * MAX_IRQ_LINES + irqline];
        }

        static int cpu_2_irq_callback(int irqline)
        {
            if (irq_line_state[2 * MAX_IRQ_LINES + irqline] == HOLD_LINE)
            {
                SETIRQLINE(2, irqline, CLEAR_LINE);
                irq_line_state[2 * MAX_IRQ_LINES + irqline] = CLEAR_LINE;
            }
            //LOG((errorlog, "cpu_2_irq_callback(%d) $%04x\n", irqline, irq_line_vector[2 * MAX_IRQ_LINES + irqline]));
            return irq_line_vector[2 * MAX_IRQ_LINES + irqline];
        }

        static int cpu_3_irq_callback(int irqline)
        {
            if (irq_line_state[3 * MAX_IRQ_LINES + irqline] == HOLD_LINE)
            {
                SETIRQLINE(3, irqline, CLEAR_LINE);
                irq_line_state[3 * MAX_IRQ_LINES + irqline] = CLEAR_LINE;
            }
            //LOG((errorlog, "cpu_3_irq_callback(%d) $%04x\n", irqline, irq_line_vector[2 * MAX_IRQ_LINES + irqline]));
            return irq_line_vector[3 * MAX_IRQ_LINES + irqline];
        }


        static void cpu_clear_pending_interrupts(int cpunum)
        {
            Timer.timer_set(Timer.TIME_NOW, cpunum, cpu_clearintcallback);
        }
        static void cpu_clearintcallback(int param)
        {
            /* clear the interrupts */
            cpu_clear_interrupts(param);
        }

        static void cpu_clear_interrupts(int cpunum)
        {
            int oldactive = activecpu;

            /* swap to the CPU's context */
            activecpu = cpunum;
            memorycontextswap(activecpu);
            if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

            /* clear NMI line */
            SETNMILINE(activecpu, CLEAR_LINE);

            /* clear all IRQ lines */
            for (int i = 0; i < cpu[activecpu].intf.num_irqs; i++)
                SETIRQLINE(activecpu, i, CLEAR_LINE);

            /* update the CPU's context */
            if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref cpu[activecpu].context);
            activecpu = oldactive;
            if (activecpu >= 0) memorycontextswap(activecpu);
        }
        public static int cpu_gettotalcpu()
        {
            return totalcpu;
        }
        public static int cpu_getactivecpu() { return activecpu < 0 ? 0 : activecpu; }

        public static void cpu_setactivecpu(int cpunum)
        {
            activecpu = cpunum;
        }

        public static int cpu_scalebyfcount(int value)
        {
            int result = (int)((double)value * Timer.timer_timeelapsed(refresh_timer) * refresh_period_inv);
            if (value >= 0) return (result < value) ? result : value;
            else return (result > value) ? result : value;
        }
        public static void cpu_yielduntil_trigger(int trigger)
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            Timer.timer_holdcpu_trigger(cpunum, trigger);
        }
        public static void cpu_yield()
        {
            cpu_yielduntil_trigger(TRIGGER_TIMESLICE);
        }
        public static void cpu_triggertime(double duration, int trigger)
        {
            Timer.timer_set(duration, trigger, cpu_trigger);
        }
        static int timetrig = 0;
        public static void cpu_yielduntil_time(double duration)
        {

            cpu_yielduntil_trigger(TRIGGER_YIELDTIME + timetrig);
            cpu_triggertime(duration, TRIGGER_YIELDTIME + timetrig);
            timetrig = (timetrig + 1) & 255;
        }







        public static void interrupt_enable_w(int offset, int data)
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            interrupt_enable[cpunum] = data;

            /* make sure there are no queued interrupts */
            if (data == 0) cpu_clear_pending_interrupts(cpunum);
        }
        public static void watchdog_reset_w(int offset, int data)
        {
            watchdog_counter = (int)Machine.drv.frames_per_second;
        }

        public static int watchdog_reset_r(int offset)
        {
            watchdog_counter = (int)Machine.drv.frames_per_second;
            return 0;
        }
        static void RESET(int index) { cpu[index].intf.reset(Machine.drv.cpu[index].reset_param); }
        static int EXECUTE(int index, int cycles) { return cpu[index].intf.execute(cycles); }
        static uint GETCONTEXT(int index, ref object context) { return cpu[index].intf.get_context(ref context); }
        static void SETCONTEXT(int index, object context) { cpu[index].intf.set_context(context); }
        static int INT_TYPE_NONE(int index) { return cpu[index].intf.no_int; }
        static int INT_TYPE_IRQ(int index) { return cpu[index].intf.irq_int; }
        static void SETNMILINE(int index, int state) { cpu[index].intf.set_nmi_line(state); }
        static void SETIRQLINE(int index, int line, int state) { cpu[index].intf.set_irq_line(line, state); }
        static void SETIRQCALLBACK(int index, irqcallback callback) { cpu[index].intf.set_irq_callback(callback); }
        static uint GETPC(int index) { return cpu[index].intf.get_pc(); }
        static int INT_TYPE_NMI(int index) { return cpu[index].intf.nmi_int; }
        static void SET_OP_BASE(int index, int pc) { cpu[index].intf.set_op_base(pc); }
        static uint GETREG(int index, int regnum) { return cpu[index].intf.get_reg(regnum); }

        public static int interrupt()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            int val;

            if (interrupt_enable[cpunum] == 0)
                return INT_TYPE_NONE(cpunum);

            val = INT_TYPE_IRQ(cpunum);
            if (val == -1000)
                val = interrupt_vector[cpunum];

            return val;
        }
        public static int nmi_interrupt()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;

            if (interrupt_enable[cpunum] == 0)
                return INT_TYPE_NONE(cpunum);

            return INT_TYPE_NMI(cpunum);
        }
        public static void interrupt_vector_w(int offset, int data)
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            if (interrupt_vector[cpunum] != data)
            {
                //LOG((errorlog,"CPU#%d interrupt_vector_w $%02x\n", cpunum, data));
                interrupt_vector[cpunum] = data;

                /* make sure there are no queued interrupts */
                cpu_clear_pending_interrupts(cpunum);
            }
        }

        public static void cpu_spin()
        {
            cpu_spinuntil_trigger(TRIGGER_TIMESLICE);
        }
        public static void cpu_spinuntil_trigger(int trigger)
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            Timer.timer_suspendcpu_trigger(cpunum, trigger);
        }
        public static void cpu_spinuntil_int()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            cpu_spinuntil_trigger(TRIGGER_INT + cpunum);
        }

        string IFC_INFO(int cpu, object context, int regnum)
        {
            return cpuintf[cpu].cpu_info(context, regnum);
        }
        static int CPU_TYPE(int index) { return (index < Machine.drv.cpu.Count) ? Machine.drv.cpu[index].cpu_type & ~CPU_FLAGS_MASK : 0; }
        int CPU_AUDIO(int index) { return Machine.drv.cpu[index].cpu_type & CPU_AUDIO_CPU; }
        string cputype_name(int cpu_type)
        {
            cpu_type &= ~CPU_FLAGS_MASK;
            if (cpu_type < CPU_COUNT)
                return IFC_INFO(cpu_type, null, CPU_INFO_NAME);
            return "";
        }


        void cpu_init()
        {
            int i;

            /* Verify the order of entries in the cpuintf[] array */
            for (i = 0; i < CPU_COUNT; i++)
            {
                if (cpuintf[i].cpu_num != i)
                {
                    printf("CPU #%d [%s] wrong ID %d: check enum CPU_... in src/driver.h!\n", i, cputype_name(i), cpuintf[i].cpu_num);
                    throw new Exception();
                }
            }

            /* count how many CPUs we have to emulate */
            totalcpu = 0;

            while (totalcpu < MAX_CPU)
            {
                if (CPU_TYPE(totalcpu) == CPU_DUMMY) break;
                totalcpu++;
            }

            /* zap the CPU data structure */
            //memset(cpu, 0, sizeof(cpu));
            cpu.Clear();

            /* Set up the interface functions */
            for (i = 0; i < MAX_CPU; i++)
                cpu.Add(new cpuinfo(cpuintf[CPU_TYPE(i)]));

            /* reset the timer system */
            Timer.timer_init();
            timeslice_timer = refresh_timer = vblank_timer = null;
        }
        public static void cpu_set_reset_line(int cpunum, int state)
        {
            Timer.timer_set(Timer.TIME_NOW, (cpunum & 7) | (state << 3), cpu_resetcallback);
        }
        static void cpu_reset_cpu(int cpunum)
        {
            int oldactive = activecpu;

            /* swap to the CPU's context */
            activecpu = cpunum;
            memorycontextswap(activecpu);
            if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

            /* reset the CPU */
            RESET(cpunum);

            /* Set the irq callback for the cpu */
            SETIRQCALLBACK(cpunum, cpu_irq_callbacks[cpunum]);

            /* update the CPU's context */
            if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref cpu[activecpu].context);
            activecpu = oldactive;
            if (activecpu >= 0) memorycontextswap(activecpu);
        }
        static void cpu_resetcallback(int param)
        {
            int state = param >> 3;
            int cpunum = param & 7;

            /* reset the CPU */
            if (state == PULSE_LINE)
                cpu_reset_cpu(cpunum);
            else if (state == ASSERT_LINE)
            {
                /* ASG - do we need this?		cpu_reset_cpu(cpunum);*/
                Timer.timer_suspendcpu(cpunum, 1, Timer.SUSPEND_REASON_RESET);	/* halt cpu */
            }
            else if (state == CLEAR_LINE)
            {
                if (Timer.timer_iscpususpended(cpunum, Timer.SUSPEND_REASON_RESET))
                    cpu_reset_cpu(cpunum);
                Timer.timer_suspendcpu(cpunum, 0, Timer.SUSPEND_REASON_RESET);	/* restart cpu */
            }
        }
        string cputype_core_file(int cpu_type)
        {
            cpu_type &= ~CPU_FLAGS_MASK;
            return IFC_INFO(cpu_type, null, CPU_INFO_FILE);
        }
        string cpunum_core_file(int cpunum)
        {
            return cputype_core_file(CPU_TYPE(cpunum));
        }













        void cpu_run()
        {
            /* determine which CPUs need a context switch */
            for (int i = 0; i < totalcpu; i++)
            {
                // allocate a context buffer for the CPU. We autoalloc context in cpudrivere here!!
                cpu[i].intf.create_context(ref cpu[i].context);

                //int size = (int)cpu[i].intf.get_context(ref cpu[i].context);
                //if (size == 0)
                //{
                //    /* That can't really be true */
                //    throw new Exception(sprintf("CPU #%d claims to need no context buffer!\n", i));
                //}


                /* Save if there is another CPU of the same type */
                cpu[i].save_context = false;

                for (int j = 0; j < totalcpu; j++)
                    if (i != j && cpunum_core_file(i).CompareTo(cpunum_core_file(j)) == 0)
                        cpu[i].save_context = true;

                for (int j = 0; j < MAX_IRQ_LINES; j++)
                {
                    irq_line_state[i * MAX_IRQ_LINES + j] = CLEAR_LINE;
                    irq_line_vector[i * MAX_IRQ_LINES + j] = cpuintf[CPU_TYPE(i)].default_vector;
                }
            }

        reset:
            /* read hi scores information from hiscore.dat */
            hs_open(Machine.gamedrv.name);
            hs_init();

            /* initialize the various timers (suspends all CPUs at startup) */
            cpu_inittimers();
            watchdog_counter = -1;

            /* reset sound chips */
            sound_reset();

            /* enable all CPUs (except for audio CPUs if the sound is off) */
            for (int i = 0; i < totalcpu; i++)
            {
                if (CPU_AUDIO(i) == 0 || Machine.sample_rate != 0)
                {
                    Timer.timer_suspendcpu(i, 0, Timer.SUSPEND_REASON_RESET);
                }
                else
                {
                    Timer.timer_suspendcpu(i, 1, Timer.SUSPEND_REASON_DISABLE);
                }
            }

            have_to_reset = false;
            vblank = 0;

            System.Console.WriteLine("Machine reset\n");

            /* start with interrupts enabled, so the generic routine will work even if */
            /* the machine doesn't have an interrupt enable port */
            for (int i = 0; i < MAX_CPU; i++)
            {
                interrupt_enable[i] = 1;
                interrupt_vector[i] = 0xff;
            }

            /* do this AFTER the above so init_machine() can use cpu_halt() to hold the */
            /* execution of some CPUs, or disable interrupts */
            Machine.drv.init_machine();

            /* reset each CPU */
            for (int i = 0; i < totalcpu; i++)
            {
                /* swap memory contexts and reset */
                memorycontextswap(i);
                if (cpu[i].save_context) SETCONTEXT(i, cpu[i].context);
                activecpu = i;
                RESET(i);

                /* Set the irq callback for the cpu */
                SETIRQCALLBACK(i, cpu_irq_callbacks[i]);

                /* save the CPU context if necessary */
                if (cpu[i].save_context) GETCONTEXT(i, ref cpu[i].context);

                /* reset the total number of cycles */
                cpu[i].totalcycles = 0;
            }

            /* reset the globals */
            cpu_vblankreset();
            current_frame = 0;

            /* loop until the user quits */
            usres = 0;
            while (usres == 0)
            {
                int cpunum = 0;

                /* was machine_reset() called? */
                if (have_to_reset)
                {

                    goto reset;
                }

                /* ask the timer system to schedule */
                if (Timer.timer_schedule_cpu(ref cpunum, ref cycles_running))
                {
                    int ran;


                    /* switch memory and CPU contexts */
                    activecpu = cpunum;
                    memorycontextswap(activecpu);
                    if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

                    /* make sure any bank switching is reset */
                    SET_OP_BASE(activecpu, (int)GETPC(activecpu));

                    /* run for the requested number of cycles */
                    ran = EXECUTE(activecpu, cycles_running);

                    /* update based on how many cycles we really ran */
                    cpu[activecpu].totalcycles += ran;

                    /* update the contexts */
                    if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref cpu[activecpu].context);
                    activecpu = -1;

                    /* update the timer with how long we actually ran */
                    Timer.timer_update_cpu(cpunum, ran);
                }

            }

            /* write hi scores to disk - No scores saving if cheat */
            hs_close();

            /* shut down the CPU cores */
            for (int i = 0; i < totalcpu; i++)
            {
                /* if the CPU core defines an exit function, call it now */
                cpu[i].intf.exit();

                /* free the context buffer for that CPU */
                if (cpu[i].context != null)
                {
                    //free( cpu[i].context );
                    cpu[i].context = null;
                }
            }
            totalcpu = 0;
        }
        void cpu_inittimers()
        {
            double first_time;
            int i, max, ipf;

            /* remove old timers */
            if (timeslice_timer != null)
                Timer.timer_remove(timeslice_timer);
            if (refresh_timer != null)
                Timer.timer_remove(refresh_timer);
            if (vblank_timer != null)
                Timer.timer_remove(vblank_timer);

            /* allocate a dummy timer at the minimum frequency to break things up */
            ipf = Machine.drv.cpu_slices_per_frame;
            if (ipf <= 0)
                ipf = 1;
            timeslice_period = Timer.TIME_IN_HZ(Machine.drv.frames_per_second * ipf);
            timeslice_timer = Timer.timer_pulse(timeslice_period, 0, cpu_timeslicecallback);

            /* allocate an infinite timer to track elapsed time since the last refresh */
            refresh_period = Timer.TIME_IN_HZ(Machine.drv.frames_per_second);
            refresh_period_inv = 1.0 / refresh_period;
            refresh_timer = Timer.timer_set(Timer.TIME_NEVER, 0, null);

            /* while we're at it, compute the scanline times */
            if (Machine.drv.vblank_duration != 0)
                scanline_period = (refresh_period - Timer.TIME_IN_USEC(Machine.drv.vblank_duration)) /
                        (double)(Machine.drv.visible_area.max_y - Machine.drv.visible_area.min_y + 1);
            else
                scanline_period = refresh_period / (double)Machine.drv.screen_height;
            scanline_period_inv = 1.0 / scanline_period;

            /*
             *		The following code finds all the CPUs that are interrupting in sync with the VBLANK
             *		and sets up the VBLANK timer to run at the minimum number of cycles per frame in
             *		order to service all the synced interrupts
             */

            /* find the CPU with the maximum interrupts per frame */
            max = 1;
            for (i = 0; i < totalcpu; i++)
            {
                ipf = Machine.drv.cpu[i].vblank_interrupts_per_frame;
                if (ipf > max)
                    max = ipf;
            }

            /* now find the LCD with the rest of the CPUs (brute force - these numbers aren't huge) */
            vblank_multiplier = max;
            while (true)
            {
                for (i = 0; i < totalcpu; i++)
                {
                    ipf = Machine.drv.cpu[i].vblank_interrupts_per_frame;
                    if (ipf > 0 && (vblank_multiplier % ipf) != 0)
                        break;
                }
                if (i == totalcpu)
                    break;
                vblank_multiplier += max;
            }

            /* initialize the countdown timers and intervals */
            for (i = 0; i < totalcpu; i++)
            {
                ipf = Machine.drv.cpu[i].vblank_interrupts_per_frame;
                if (ipf > 0)
                    cpu[i].vblankint_countdown = cpu[i].vblankint_multiplier = vblank_multiplier / ipf;
                else
                    cpu[i].vblankint_countdown = cpu[i].vblankint_multiplier = -1;
            }

            /* allocate a vblank timer at the frame rate * the LCD number of interrupts per frame */
            vblank_period = Timer.TIME_IN_HZ(Machine.drv.frames_per_second * vblank_multiplier);
            vblank_timer = Timer.timer_pulse(vblank_period, 0, cpu_vblankcallback);
            vblank_countdown = vblank_multiplier;

            /*
             *		The following code creates individual timers for each CPU whose interrupts are not
             *		synced to the VBLANK, and computes the typical number of cycles per interrupt
             */

            /* start the CPU interrupt timers */
            for (i = 0; i < totalcpu; i++)
            {
                ipf = Machine.drv.cpu[i].vblank_interrupts_per_frame;

                /* remove old timers */
                if (cpu[i].vblankint_timer != null)
                    Timer.timer_remove(cpu[i].vblankint_timer);
                if (cpu[i].timedint_timer != null)
                    Timer.timer_remove(cpu[i].timedint_timer);

                /* compute the average number of cycles per interrupt */
                if (ipf <= 0)
                    ipf = 1;
                cpu[i].vblankint_period = Timer.TIME_IN_HZ(Machine.drv.frames_per_second * ipf);
                cpu[i].vblankint_timer = Timer.timer_set(Timer.TIME_NEVER, 0, null);

                /* see if we need to allocate a CPU timer */
                ipf = Machine.drv.cpu[i].timed_interrupts_per_second;
                if (ipf != 0)
                {
                    cpu[i].timedint_period = cpu_computerate(ipf);
                    cpu[i].timedint_timer = Timer.timer_pulse(cpu[i].timedint_period, i, cpu_timedintcallback);
                }
            }

            /* note that since we start the first frame on the refresh, we can't pulse starting
               immediately; instead, we back up one VBLANK period, and inch forward until we hit
               positive time. That time will be the time of the first VBLANK timer callback */
            Timer.timer_remove(vblank_timer);

            first_time = -Timer.TIME_IN_USEC(Machine.drv.vblank_duration) + vblank_period;
            while (first_time < 0)
            {
                cpu_vblankcallback(-1);
                first_time += vblank_period;
            }
            vblank_timer = Timer.timer_set(first_time, 0, cpu_firstvblankcallback);
        }
        void cpu_firstvblankcallback(int param)
        {
            /* now that we're synced up, pulse from here on out */
            vblank_timer = Timer.timer_pulse(vblank_period, param, cpu_vblankcallback);

            /* but we need to call the standard routine as well */
            cpu_vblankcallback(param);
        }
        static void cpu_manualnmicallback(int param)
        {
            int cpunum, state, oldactive;
            cpunum = param & 7;
            state = param >> 3;

            /* swap to the CPU's context */
            oldactive = activecpu;
            activecpu = cpunum;
            memorycontextswap(activecpu);
            if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

            //LOG((errorlog,"cpu_manualnmicallback %d,%d\n",cpunum,state));

            switch (state)
            {
                case PULSE_LINE:
                    SETNMILINE(cpunum, ASSERT_LINE);
                    SETNMILINE(cpunum, CLEAR_LINE);
                    break;
                case HOLD_LINE:
                case ASSERT_LINE:
                    SETNMILINE(cpunum, ASSERT_LINE);
                    break;
                case CLEAR_LINE:
                    SETNMILINE(cpunum, CLEAR_LINE);
                    break;
                default:
                    printf("cpu_manualnmicallback cpu #%d unknown state %d\n", cpunum, state);
                    break;
            }
            /* update the CPU's context */
            if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref  cpu[activecpu].context);
            activecpu = oldactive;
            if (activecpu >= 0) memorycontextswap(activecpu);

            /* generate a trigger to unsuspend any CPUs waiting on the interrupt */
            if (state != CLEAR_LINE)
                Timer.timer_trigger(TRIGGER_INT + cpunum);
        }
        static void cpu_manualirqcallback(int param)
        {
            int cpunum, irqline, state, oldactive;

            irqline = param & 7;
            cpunum = (param >> 3) & 7;
            state = param >> 6;

            /* swap to the CPU's context */
            oldactive = activecpu;
            activecpu = cpunum;
            memorycontextswap(activecpu);
            if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

            //LOG((errorlog,"cpu_manualirqcallback %d,%d,%d\n",cpunum,irqline,state));

            irq_line_state[cpunum * MAX_IRQ_LINES + irqline] = state;
            switch (state)
            {
                case PULSE_LINE:
                    SETIRQLINE(cpunum, irqline, ASSERT_LINE);
                    SETIRQLINE(cpunum, irqline, CLEAR_LINE);
                    break;
                case HOLD_LINE:
                case ASSERT_LINE:
                    SETIRQLINE(cpunum, irqline, ASSERT_LINE);
                    break;
                case CLEAR_LINE:
                    SETIRQLINE(cpunum, irqline, CLEAR_LINE);
                    break;
                default:
                    printf("cpu_manualirqcallback cpu #%d, line %d, unknown state %d\n", cpunum, irqline, state);
                    break;
            }

            /* update the CPU's context */
            if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref cpu[activecpu].context);
            activecpu = oldactive;
            if (activecpu >= 0) memorycontextswap(activecpu);

            /* generate a trigger to unsuspend any CPUs waiting on the interrupt */
            if (state != CLEAR_LINE)
                Timer.timer_trigger(TRIGGER_INT + cpunum);
        }
        void cpu_timeslicecallback(int param)
        {
            Timer.timer_trigger(TRIGGER_TIMESLICE);
        }
        static bool cpu_getstatus(int cpunum)
        {
            if (cpunum >= MAX_CPU) return false;

            return !Timer.timer_iscpususpended(cpunum, Timer.SUSPEND_REASON_HALT | Timer.SUSPEND_REASON_RESET | Timer.SUSPEND_REASON_DISABLE);
        }

        public static int ignore_interrupt()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return INT_TYPE_NONE(cpunum);
        }
        static void cpu_manualintcallback(int param)
        {
            int intnum = param >> 3;
            int cpunum = param & 7;

            /* generate the interrupt */
            cpu_generate_interrupt(cpunum, null, intnum);
        }
        /* these are available externally, for the timer system */
        public static int cycles_currently_ran()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return cycles_running - cpu[cpunum].intf.icount[0];
        }

        public static int cycles_left_to_run()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return cpu[cpunum].intf.icount[0];
        }


        public static int cpu_getcurrentframe() { return current_frame; }
        public static int cpu_gettotalcycles()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return cpu[cpunum].totalcycles + cycles_currently_ran();
        }
        public static void cpu_cause_interrupt(int cpunum, int type)
        {
            /* don't trigger interrupts on suspended CPUs */
            if (!cpu_getstatus(cpunum)) return;

            Timer.timer_set(Timer.TIME_NOW, (cpunum & 7) | (type << 3), cpu_manualintcallback);
        }
        public static void cpu_set_halt_line(int cpunum, int state)
        {
            Timer.timer_set(Timer.TIME_NOW, (cpunum & 7) | (state << 3), cpu_haltcallback);
        }
        static void cpu_haltcallback(int param)
        {
            int state = param >> 3;
            int cpunum = param & 7;

            /* reset the CPU */
            if (state == ASSERT_LINE)
                Timer.timer_suspendcpu(cpunum, 1, Timer.SUSPEND_REASON_HALT);	/* halt cpu */
            else if (state == CLEAR_LINE)
                Timer.timer_suspendcpu(cpunum, 0, Timer.SUSPEND_REASON_HALT);	/* restart cpu */
        }

        static void cpu_generate_interrupt(int cpunum, vblank_interrupt_callback func, int num)
        {
            int oldactive = activecpu;

            /* don't trigger interrupts on suspended CPUs */
            if (!cpu_getstatus(cpunum)) return;

            /* swap to the CPU's context */
            activecpu = cpunum;
            memorycontextswap(activecpu);
            if (cpu[activecpu].save_context) SETCONTEXT(activecpu, cpu[activecpu].context);

            /* cause the interrupt, calling the function if it exists */
            if (func != null) num = func();

            /* wrapper for the new interrupt system */
            if (num != INT_TYPE_NONE(cpunum))
            {
                //LOG((errorlog,"CPU#%d interrupt type $%04x: ", cpunum, num));
                /* is it the NMI type interrupt of that CPU? */
                if (num == INT_TYPE_NMI(cpunum))
                {

                    //			LOG((errorlog,"NMI\n"));
                    cpu_manualnmicallback(cpunum | (PULSE_LINE << 3));

                }
                else
                {
                    int irq_line;

                    switch (CPU_TYPE(cpunum))
                    {

                        case CPU_Z80: irq_line = 0; break;//LOG((errorlog,"Z80 IRQ\n")); break;

                        case CPU_8080:
                            switch (num)
                            {
                                case cpu_i8085.I8080_INTR: irq_line = 0; //LOG((errorlog,"I8080 INTR\n")); 
                                    break;
                                default: irq_line = 0; //LOG((errorlog,"I8080 unknown\n"));
                                    break;
                            }
                            break;


                        //case CPU_8085A:
                        //    switch (num)
                        //    {
                        //    case I8085_INTR:		irq_line = 0; break;
                        //    case I8085_RST55:		irq_line = 1;  break;
                        //    case I8085_RST65:		irq_line = 2;  break;
                        //    case I8085_RST75:		irq_line = 3;  break;
                        //    default:				irq_line = 0; break;
                        //    }
                        //    break;


                        case CPU_M6502: irq_line = 0; break;


                        case CPU_M65C02: irq_line = 0; break;
                        case CPU_M65SC02: irq_line = 0; break;
                        case CPU_M65CE02: irq_line = 0; break;
                        case CPU_M6509: irq_line = 0; break;
                        case CPU_M6510: irq_line = 0; break;
                        case CPU_N2A03: irq_line = 0; break;
#if (HAS_H6280)
			case CPU_H6280:
				switch (num)
				{
				case H6280_INT_IRQ1:	irq_line = 0; LOG((errorlog,"H6280 INT 1\n")); break;
				case H6280_INT_IRQ2:	irq_line = 1; LOG((errorlog,"H6280 INT 2\n")); break;
				case H6280_INT_TIMER:	irq_line = 2; LOG((errorlog,"H6280 TIMER INT\n")); break;
				default:				irq_line = 0; LOG((errorlog,"H6280 unknown\n"));
				}
				break;
#endif
			case CPU_I86:				irq_line = 0; break;
			case CPU_V20:				irq_line = 0; break;
			case CPU_V30:				irq_line = 0; break;
			case CPU_V33:				irq_line = 0; break;
			case CPU_I8035: 			irq_line = 0; break;
			case CPU_I8039: 			irq_line = 0; break;
			case CPU_I8048: 			irq_line = 0; break;
            case CPU_N7751: irq_line = 0; break; 

                        case CPU_M6800: irq_line = 0; break;//LOG((errorlog,"M6800 IRQ\n")); break;

                        case CPU_M6801: irq_line = 0; break;
                        case CPU_M6802: irq_line = 0; break;
                        case CPU_M6803: irq_line = 0; break;
                        case CPU_M6808: irq_line = 0; break;
                        case CPU_HD63701: irq_line = 0; break;

                        case CPU_M6805: irq_line = 0; break;
                        case CPU_M68705: irq_line = 0; break;
                        case CPU_HD63705: irq_line = 0; break;
                        case CPU_HD6309:
                            switch (num)
                            {
                                case cpu_m6809.HD6309_INT_IRQ: irq_line = 0; break;
                                case cpu_m6809.HD6309_INT_FIRQ: irq_line = 1; break;
                                default: irq_line = 0; break;
                            }
                            break;

                        case CPU_M6809:
                            switch (num)
                            {
                                case cpu_m6809.M6809_INT_IRQ: irq_line = 0; break;//LOG((errorlog,"M6809 IRQ\n")); break;
                                case cpu_m6809.M6809_INT_FIRQ: irq_line = 1; break;//LOG((errorlog,"M6809 FIRQ\n")); break;
                                default: irq_line = 0; break;//LOG((errorlog,"M6809 unknown\n"));
                            }
                            break;
                        case CPU_KONAMI:
                            switch (num)
                            {
                                case cpu_konami.KONAMI_INT_IRQ: irq_line = 0; break;
                                case cpu_konami.KONAMI_INT_FIRQ: irq_line = 1; break;
                                default: irq_line = 0; break;
                            }
                            break;
                        //case CPU_M68000:
                        //    switch (num)
                        //    {
                        //    case MC68000_IRQ_1: 	irq_line = 1; LOG((errorlog,"M68K IRQ1\n")); break;
                        //    case MC68000_IRQ_2: 	irq_line = 2; LOG((errorlog,"M68K IRQ2\n")); break;
                        //    case MC68000_IRQ_3: 	irq_line = 3; LOG((errorlog,"M68K IRQ3\n")); break;
                        //    case MC68000_IRQ_4: 	irq_line = 4; LOG((errorlog,"M68K IRQ4\n")); break;
                        //    case MC68000_IRQ_5: 	irq_line = 5; LOG((errorlog,"M68K IRQ5\n")); break;
                        //    case MC68000_IRQ_6: 	irq_line = 6; LOG((errorlog,"M68K IRQ6\n")); break;
                        //    case MC68000_IRQ_7: 	irq_line = 7; LOG((errorlog,"M68K IRQ7\n")); break;
                        //    default:				irq_line = 0; LOG((errorlog,"M68K unknown\n"));
                        //    }
                        //    /* until now only auto vector interrupts supported */
                        //    num = MC68000_INT_ACK_AUTOVECTOR;
                        //    break;
                        //case CPU_M68010:
                        //    switch (num)
                        //    {
                        //    case MC68010_IRQ_1: 	irq_line = 1; LOG((errorlog,"M68010 IRQ1\n")); break;
                        //    case MC68010_IRQ_2: 	irq_line = 2; LOG((errorlog,"M68010 IRQ2\n")); break;
                        //    case MC68010_IRQ_3: 	irq_line = 3; LOG((errorlog,"M68010 IRQ3\n")); break;
                        //    case MC68010_IRQ_4: 	irq_line = 4; LOG((errorlog,"M68010 IRQ4\n")); break;
                        //    case MC68010_IRQ_5: 	irq_line = 5; LOG((errorlog,"M68010 IRQ5\n")); break;
                        //    case MC68010_IRQ_6: 	irq_line = 6; LOG((errorlog,"M68010 IRQ6\n")); break;
                        //    case MC68010_IRQ_7: 	irq_line = 7; LOG((errorlog,"M68010 IRQ7\n")); break;
                        //    default:				irq_line = 0; LOG((errorlog,"M68010 unknown\n"));
                        //    }
                        //    /* until now only auto vector interrupts supported */
                        //    num = MC68000_INT_ACK_AUTOVECTOR;
                        //    break;
                        //case CPU_M68020:
                        //    switch (num)
                        //    {
                        //    case MC68020_IRQ_1: 	irq_line = 1; LOG((errorlog,"M68020 IRQ1\n")); break;
                        //    case MC68020_IRQ_2: 	irq_line = 2; LOG((errorlog,"M68020 IRQ2\n")); break;
                        //    case MC68020_IRQ_3: 	irq_line = 3; LOG((errorlog,"M68020 IRQ3\n")); break;
                        //    case MC68020_IRQ_4: 	irq_line = 4; LOG((errorlog,"M68020 IRQ4\n")); break;
                        //    case MC68020_IRQ_5: 	irq_line = 5; LOG((errorlog,"M68020 IRQ5\n")); break;
                        //    case MC68020_IRQ_6: 	irq_line = 6; LOG((errorlog,"M68020 IRQ6\n")); break;
                        //    case MC68020_IRQ_7: 	irq_line = 7; LOG((errorlog,"M68020 IRQ7\n")); break;
                        //    default:				irq_line = 0; LOG((errorlog,"M68020 unknown\n"));
                        //    }
                        //    /* until now only auto vector interrupts supported */
                        //    num = MC68000_INT_ACK_AUTOVECTOR;
                        //    break;
#if (HAS_M68EC020)
			case CPU_M68EC020:
				switch (num)
				{
				case MC68EC020_IRQ_1:	irq_line = 1; LOG((errorlog,"M68EC020 IRQ1\n")); break;
				case MC68EC020_IRQ_2:	irq_line = 2; LOG((errorlog,"M68EC020 IRQ2\n")); break;
				case MC68EC020_IRQ_3:	irq_line = 3; LOG((errorlog,"M68EC020 IRQ3\n")); break;
				case MC68EC020_IRQ_4:	irq_line = 4; LOG((errorlog,"M68EC020 IRQ4\n")); break;
				case MC68EC020_IRQ_5:	irq_line = 5; LOG((errorlog,"M68EC020 IRQ5\n")); break;
				case MC68EC020_IRQ_6:	irq_line = 6; LOG((errorlog,"M68EC020 IRQ6\n")); break;
				case MC68EC020_IRQ_7:	irq_line = 7; LOG((errorlog,"M68EC020 IRQ7\n")); break;
				default:				irq_line = 0; LOG((errorlog,"M68EC020 unknown\n"));
				}
				/* until now only auto vector interrupts supported */
				num = MC68000_INT_ACK_AUTOVECTOR;
				break;
#endif
#if HAS_T11
			case CPU_T11:
				switch (num)
				{
				case T11_IRQ0:			irq_line = 0; LOG((errorlog,"T11 IRQ0\n")); break;
				case T11_IRQ1:			irq_line = 1; LOG((errorlog,"T11 IRQ1\n")); break;
				case T11_IRQ2:			irq_line = 2; LOG((errorlog,"T11 IRQ2\n")); break;
				case T11_IRQ3:			irq_line = 3; LOG((errorlog,"T11 IRQ3\n")); break;
				default:				irq_line = 0; LOG((errorlog,"T11 unknown\n"));
				}
				break;
#endif
#if HAS_S2650
			case CPU_S2650: 			irq_line = 0; LOG((errorlog,"S2650 IRQ\n")); break;
#endif
#if HAS_TMS34010
			case CPU_TMS34010:
				switch (num)
				{
				case TMS34010_INT1: 	irq_line = 0; LOG((errorlog,"TMS34010 INT1\n")); break;
				case TMS34010_INT2: 	irq_line = 1; LOG((errorlog,"TMS34010 INT2\n")); break;
				default:				irq_line = 0; LOG((errorlog,"TMS34010 unknown\n"));
				}
				break;
#endif
                        /*#if HAS_TMS9900
			case CPU_TMS9900:	irq_line = 0; LOG((errorlog,"TMS9900 IRQ\n")); break;
#endif*/
#if (HAS_TMS9900) || (HAS_TMS9940) || (HAS_TMS9980) || (HAS_TMS9985) || (HAS_TMS9989) || (HAS_TMS9995) || (HAS_TMS99105A) || (HAS_TMS99110A)
#if (HAS_TMS9900)
			case CPU_TMS9900:
#endif
#if (HAS_TMS9940)
			case CPU_TMS9940:
#endif
#if (HAS_TMS9980)
			case CPU_TMS9980:
#endif
#if (HAS_TMS9985)
			case CPU_TMS9985:
#endif
#if (HAS_TMS9989)
			case CPU_TMS9989:
#endif
#if (HAS_TMS9995)
			case CPU_TMS9995:
#endif
#if (HAS_TMS99105A)
			case CPU_TMS99105A:
#endif
#if (HAS_TMS99110A)
			case CPU_TMS99110A:
#endif
				LOG((errorlog,"Please use the new interrupt scheme for your new developments !\n"));
				irq_line = 0;
				break;
#endif
#if HAS_Z8000
			case CPU_Z8000:
				switch (num)
				{
				case Z8000_NVI: 		irq_line = 0; LOG((errorlog,"Z8000 NVI\n")); break;
				case Z8000_VI:			irq_line = 1; LOG((errorlog,"Z8000 VI\n")); break;
				default:				irq_line = 0; LOG((errorlog,"Z8000 unknown\n"));
				}
				break;
#endif
#if HAS_TMS320C10
			case CPU_TMS320C10:
				switch (num)
				{
				case TMS320C10_ACTIVE_INT:	irq_line = 0; LOG((errorlog,"TMS32010 INT\n")); break;
				case TMS320C10_ACTIVE_BIO:	irq_line = 1; LOG((errorlog,"TMS32010 BIO\n")); break;
				default:					irq_line = 0; LOG((errorlog,"TMS32010 unknown\n"));
				}
				break;
#endif
#if HAS_ADSP2100
			case CPU_ADSP2100:
				switch (num)
				{
				case ADSP2100_IRQ0: 		irq_line = 0; LOG((errorlog,"ADSP2100 IRQ0\n")); break;
				case ADSP2100_IRQ1: 		irq_line = 1; LOG((errorlog,"ADSP2100 IRQ1\n")); break;
				case ADSP2100_IRQ2: 		irq_line = 2; LOG((errorlog,"ADSP2100 IRQ1\n")); break;
				case ADSP2100_IRQ3: 		irq_line = 3; LOG((errorlog,"ADSP2100 IRQ1\n")); break;
				default:					irq_line = 0; LOG((errorlog,"ADSP2100 unknown\n"));
				}
				break;
#endif
                        default:
                            irq_line = 0;
                            /* else it should be an IRQ type; assume line 0 and store vector */
                            //LOG((errorlog,"unknown IRQ\n"));
                            break;
                    }
                    cpu_irq_line_vector_w(cpunum, irq_line, num);
                    cpu_manualirqcallback(irq_line | (cpunum << 3) | (HOLD_LINE << 6));
                }
            }

            /* update the CPU's context */
            if (cpu[activecpu].save_context) GETCONTEXT(activecpu, ref cpu[activecpu].context);
            activecpu = oldactive;
            if (activecpu >= 0) memorycontextswap(activecpu);

            /* trigger already generated by cpu_manualirqcallback or cpu_manualnmicallback */
        }
        public static void cpu_irq_line_vector_w(int cpunum, int irqline, int vector)
        {
            cpunum &= (MAX_CPU - 1);
            irqline &= (MAX_IRQ_LINES - 1);
            if (irqline < cpu[cpunum].intf.num_irqs)
            {
                //LOG((errorlog,"cpu_irq_line_vector_w(%d,%d,$%04x)\n",cpunum,irqline,vector));
                irq_line_vector[cpunum * MAX_IRQ_LINES + irqline] = vector;
                return;
            }
            //LOG((errorlog, "cpu_irq_line_vector_w CPU#%d irqline %d > max irq lines\n", cpunum, irqline));
        }
        void cpu_timedintcallback(int param)
        {
            /* bail if there is no routine */
            //if (Machine.drv.cpu[param].timed_interrupt==null)
            //return;

            /* generate the interrupt */
            cpu_generate_interrupt(param, Machine.drv.cpu[param].timed_interrupt, 0);
        }
        void cpu_vblankintcallback(int param)
        {
            if (Machine.drv.cpu[param].vblank_interrupt != null)
                cpu_generate_interrupt(param, Machine.drv.cpu[param].vblank_interrupt, 0);

            /* update the counters */
            cpu[param].iloops--;
        }
        void machine_reset()
        {
            /* write hi scores to disk - No scores saving if cheat */
            hs_close();

            have_to_reset = true;
        }
        /***************************************************************************
          Video update callback. This is called a game-dependent amount of time
          after the VBLANK in order to trigger a video update.
        ***************************************************************************/
        void cpu_updatecallback(int param)
        {
            /* update the screen if we didn't before */
            if ((Machine.drv.video_attributes & VIDEO_UPDATE_AFTER_VBLANK) != 0)
                usres = updatescreen();
            vblank = 0;

            /* update IPT_VBLANK input ports */
            inputport_vblank_end();

            /* check the watchdog */
            if (watchdog_counter > 0)
            {
                if (--watchdog_counter == 0)
                {
                    printf("reset caused by the watchdog\n");
                    machine_reset();
                }
            }

            current_frame++;

            /* reset the refresh timer */
            Timer.timer_reset(refresh_timer, Timer.TIME_NEVER);
        }
        void cpu_vblankcallback(int param)
        {
            /* loop over CPUs */
            for (int i = 0; i < totalcpu; i++)
            {
                /* if the interrupt multiplier is valid */
                if (cpu[i].vblankint_multiplier != -1)
                {
                    /* decrement; if we hit zero, generate the interrupt and reset the countdown */
                    if (--cpu[i].vblankint_countdown == 0)
                    {
                        if (param != -1)
                            cpu_vblankintcallback(i);
                        cpu[i].vblankint_countdown = cpu[i].vblankint_multiplier;
                        Timer.timer_reset(cpu[i].vblankint_timer, Timer.TIME_NEVER);
                    }
                }

                /* else reset the VBLANK timer if this is going to be a real VBLANK */
                else if (vblank_countdown == 1)
                    Timer.timer_reset(cpu[i].vblankint_timer, Timer.TIME_NEVER);
            }

            /* is it a real VBLANK? */
            if (--vblank_countdown == 0)
            {
                /* do we update the screen now? */
                if ((Machine.drv.video_attributes & VIDEO_UPDATE_AFTER_VBLANK) == 0)
                    usres = updatescreen();

                /* Set the timer to update the screen */
                Timer.timer_set(Timer.TIME_IN_USEC(Machine.drv.vblank_duration), 0, cpu_updatecallback);
                vblank = 1;

                /* reset the globals */
                cpu_vblankreset();

                /* reset the counter */
                vblank_countdown = vblank_multiplier;
            }
        }
        double cpu_computerate(int value)
        {
            /* values equal to zero are zero */
            if (value <= 0)
                return 0.0;

            /* values above between 0 and 50000 are in Hz */
            if (value < 50000)
                return Timer.TIME_IN_HZ(value);

            /* values greater than 50000 are in nanoseconds */
            else
                return Timer.TIME_IN_NSEC(value);
        }
        /***************************************************************************
         VBLANK reset. Called at the start of emulation and once per VBLANK in
         order to update the input ports and reset the interrupt counter.
        ***************************************************************************/
        void cpu_vblankreset()
        {
            /* read hi scores from disk */
            hs_update();

            /* read keyboard & update the status of the input ports */
            update_input_ports();

            /* reset the cycle counters */
            for (int i = 0; i < totalcpu; i++)
            {
                if (!Timer.timer_iscpususpended(i, Timer.SUSPEND_ANY_REASON))
                    cpu[i].iloops = Machine.drv.cpu[i].vblank_interrupts_per_frame - 1;
                else
                    cpu[i].iloops = -1;
            }
        }
        static void change_pc_generic(uint pc, int abits2, int abitsmin, int shift, setopbase setop)
        {
            if (cur_mrhard[pc >> (abits2 + abitsmin + shift)] != ophw)
                setop((int)pc, shift);
        }
        static void change_pc(uint pc)
        {
            change_pc_generic(pc, ABITS2_16, ABITS_MIN_16, 0, cpu_setOPbase16);
        }
        static void change_pc16(uint pc)
        {
            change_pc_generic(pc, ABITS2_16, ABITS_MIN_16, 0, cpu_setOPbase16);
        }
        static void change_pc20(uint pc)
        {
            change_pc_generic(pc, ABITS2_20, ABITS_MIN_20, 0, cpu_setOPbase20);
        }


        public delegate void setopbase(int pc, int shift);
        //#define SETOPBASE(name,abits,shift)														
        static void cpu_setOPbase16(int pc, int shift)
        {
            byte hw;

            pc = (int)((uint)pc >> shift);

            /* allow overrides */
            if (OPbasefunc != null)
            {
                pc = (int)OPbasefunc((int)pc);
                if (pc == -1)
                    return;
            }

            /* perform the lookup */
            hw = cur_mrhard[(uint)pc >> (ABITS2_16 + ABITS_MIN_16)];
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = readhardware[(hw << MH_SBITS) + (((uint)pc >> ABITS_MIN_16) & MHMASK(ABITS2_16))];
            }
            ophw = hw;

            /* RAM or banked memory */
            if (hw <= HT_BANKMAX)
            {
                if (hw == 1)
                {
                    int a = 0;
                }
                SET_OP_RAMROM(new _BytePtr(cpu_bankbase[hw], (-memoryreadoffset[hw])));
                return;
            }

            /* do not support on callback memory region */
            //printf("CPU #%d PC %04x: warning - op-code execute on mapped i/o\n",	cpu_getactivecpu(),cpu_get_pc());									
        }


        static void cpu_setOPbase20(int pc, int shift)
        {
            byte hw;

            pc = (int)((uint)pc >> shift);

            /* allow overrides */
            if (OPbasefunc != null)
            {
                pc = (int)OPbasefunc((int)pc);
                if (pc == -1)
                    return;
            }

            /* perform the lookup */
            hw = cur_mrhard[(uint)pc >> (ABITS2_20 + ABITS_MIN_20)];
            if (hw >= MH_HARDMAX)
            {
                hw -= MH_HARDMAX;
                hw = readhardware[(hw << MH_SBITS) + (((uint)pc >> ABITS_MIN_20) & MHMASK(ABITS2_20))];
            }
            ophw = hw;

            /* RAM or banked memory */
            if (hw <= HT_BANKMAX)
            {
                if (hw == 1)
                {
                    int a = 0;
                }
                SET_OP_RAMROM(new _BytePtr(cpu_bankbase[hw], (-memoryreadoffset[hw])));
                return;
            }

            /* do not support on callback memory region */
            //printf("CPU #%d PC %04x: warning - op-code execute on mapped i/o\n",	cpu_getactivecpu(),cpu_get_pc());									
        }

        static void SET_OP_RAMROM(_BytePtr _base)
        {
            OP_ROM = new _BytePtr(_base, (OP_ROM.offset - OP_RAM.offset));
            OP_RAM = new _BytePtr(_base);
        }
        public static int cpu_getiloops()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return cpu[cpunum].iloops;
        }
        public static uint cpu_get_pc()
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return GETPC(cpunum);
        }
        public static uint cpu_get_reg(int regnum)
        {
            int cpunum = (activecpu < 0) ? 0 : activecpu;
            return GETREG(cpunum, regnum);
        }
        public static void cpu_set_irq_line(int cpunum, int irqline, int state)
        {
            /* don't trigger interrupts on suspended CPUs */
            if (!cpu_getstatus(cpunum)) return;

            //LOG((errorlog,"cpu_set_irq_line(%d,%d,%d)\n",cpunum,irqline,state));
            Timer.timer_set(Timer.TIME_NOW, (irqline & 7) | ((cpunum & 7) << 3) | (state << 6), cpu_manualirqcallback);
        }
        public static void cpu_trigger(int trigger)
        {
            Timer.timer_trigger(trigger);
        }
        public static uint cpu_getpreviouspc() { return cpu_get_reg(REG_PREVIOUSPC); }
        public static void cpu_set_nmi_line(int cpunum, int state)
        {
            /* don't trigger interrupts on suspended CPUs */
            if (!cpu_getstatus(cpunum)) return;

            //LOG((errorlog,"cpu_set_nmi_line(%d,%d)\n",cpunum,state));
            Timer.timer_set(Timer.TIME_NOW, (cpunum & 7) | (state << 3), cpu_manualnmicallback);
        }
        public static double cpu_getscanlineperiod()
        {
            return scanline_period;
        }
        public static int cpu_getscanline()
        {
            return (int)(Timer.timer_timeelapsed(refresh_timer) * scanline_period_inv);
        }
        public static double cpu_getscanlinetime(int scanline)
        {
            double ret;
            double scantime = Timer.timer_starttime(refresh_timer) + (double)scanline * scanline_period;
            double abstime = Timer.timer_get_time();
            if (abstime >= scantime) scantime += Timer.TIME_IN_HZ(Machine.drv.frames_per_second);
            ret = scantime - abstime;
            if (ret < Timer.TIME_IN_NSEC(1))
            {
                ret = Timer.TIME_IN_HZ(Machine.drv.frames_per_second);
            }

            return ret;
        }
    }
}
