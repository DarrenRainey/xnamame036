using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    delegate void irqfunc(int state);
    class TMS5220interface
    {
        public int baseclock;
        public int mixing_level;
        public irqfunc irq;
        public TMS5220interface(int baseclock, int mixing_level, irqfunc irq)
        {
            this.baseclock = baseclock;
            this.mixing_level = mixing_level;
            this.irq = irq;
        }
    }
    class tms5220 : Mame.snd_interface
    {
        const int MAX_SAMPLE_CHUNK = 10000;
        const int FRAC_BITS = 14;
        const int FRAC_ONE = 1 << FRAC_BITS;
        const int FRAC_MASK = FRAC_ONE - 1;

        static TMS5220interface intf;
        static short last_sample, curr_sample;
        static uint source_step, source_pos;
        static int stream;

        public tms5220()
        {
            this.name = "TMS5520";
            this.sound_num = Mame.SOUND_TMS5220;
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((TMS5220interface)msound.sound_interface).baseclock;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            return 1;
        }
        public override int start(Mame.MachineSound msound)
        {
            intf = (TMS5220interface)msound.sound_interface;

            /* reset the 5220 */
            tms5220_reset();
            tms5220_set_irq(intf.irq);

            /* set the initial frequency */
            stream = -1;
            tms5220_set_frequency(intf.baseclock);
            source_pos = 0;
            last_sample = curr_sample = 0;

            /* initialize a stream */
            stream = Mame.stream_init("TMS5220", intf.mixing_level, Mame.Machine.sample_rate, 0, tms5220_update);
            if (stream == -1)
                return 1;

            /* request a sound channel */
            return 0;
        }
        void tms5220_set_frequency(int frequency)
        {
            /* skip if output frequency is zero */
            if (Mame.Machine.sample_rate==0)
                return;

            /* update the stream and compute a new step size */
            if (stream != -1)
                Mame.stream_update(stream, 0);
            source_step = (uint)((double)(frequency / 80) * (double)FRAC_ONE / (double)Mame.Machine.sample_rate);
        }

        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void reset()
        {
            //nothing
        }
        public override void update()
        {
            //intentionally blank
        }
        static void tms5220_set_irq(irqfunc func)
{
    irq_func = func;
}
        static void tms5220_reset()
{
    /* initialize the FIFO */
    Array.Clear(fifo,0,fifo.Length);
    fifo_head = fifo_tail = fifo_count = bits_taken = 0;

    /* initialize the chip state */
    speak_external = talk_status = buffer_empty = irq_pin = 0;
    buffer_low = 1;

    /* initialize the energy/pitch/k states */
    old_energy = new_energy = current_energy = target_energy = 0;
    old_pitch = new_pitch = current_pitch = target_pitch = 0;
    Array.Clear(old_k, 0, old_k.Length);
    Array.Clear(new_k, 0, new_k.Length);
    Array.Clear(current_k, 0, current_k.Length);
    Array.Clear(target_k, 0, target_k.Length);

    /* initialize the sample generators */
    interp_count = sample_count = pitch_count = 0;
    randbit = 0;
    Array.Clear(u, 0, u.Length);
    Array.Clear(x, 0, x.Length);

    #if DEBUG_5220
        f = fopen("tms.log", "w");
    #endif
}
        public static void tms5220_update(int ch, _ShortPtr buffer, int length)
        {
            short[] sample_data = new short[MAX_SAMPLE_CHUNK];
            ShortSubArray curr_data = new ShortSubArray(sample_data);
            short prev = last_sample, curr = curr_sample;
            uint final_pos;
            uint new_samples;

            /* finish off the current sample */
            if (source_pos > 0)
            {
                /* interpolate */
                while (length > 0 && source_pos < FRAC_ONE)
                {
                    buffer.write16(0, (ushort)((((int)prev * (FRAC_ONE - source_pos)) + ((int)curr * source_pos)) >> FRAC_BITS));
                    buffer.offset += 2;
                    source_pos += source_step;
                    length--;
                }

                /* if we're over, continue; otherwise, we're done */
                if (source_pos >= FRAC_ONE)
                    source_pos -= FRAC_ONE;
                else
                    return;
            }

            /* compute how many new samples we need */
            final_pos = (uint)(source_pos + length * source_step);
            new_samples = (final_pos + FRAC_ONE - 1) >> FRAC_BITS;
            if (new_samples > MAX_SAMPLE_CHUNK)
                new_samples = MAX_SAMPLE_CHUNK;

            /* generate them into our buffer */
            tms5220_process(sample_data, new_samples);
            prev = curr;
            curr = curr_data[0]; curr_data.offset++;

            /* then sample-rate convert with linear interpolation */
            while (length > 0)
            {
                /* interpolate */
                while (length > 0 && source_pos < FRAC_ONE)
                {
                    buffer.write16(0, (ushort)((((int)prev * (FRAC_ONE - source_pos)) + ((int)curr * source_pos)) >> FRAC_BITS));
                    source_pos += source_step;
                    length--;
                }

                /* if we're over, grab the next samples */
                if (source_pos >= FRAC_ONE)
                {
                    source_pos -= FRAC_ONE;
                    prev = curr;
                    curr = curr_data[0]; curr_data.offset++;
                }
            }

            /* remember the last samples */
            last_sample = prev;
            curr_sample = curr;
        }


        public static void tms5220_data_w(int offset, int data)
        {
            /* bring up to date first */
            Mame.stream_update(stream, 0);
            tms5220_data_write(data);
        }
        public static bool tms5220_ready_r()
        {
            /* bring up to date first */
            Mame.stream_update(stream, 0);
            return tms5220_ready_read();
        }

        const int FIFO_SIZE = 16;
        static byte[] fifo = new byte[FIFO_SIZE];
        static int fifo_head;
        static int fifo_tail;
        static int fifo_count;
        static int bits_taken;
        static int speak_external;
        static int talk_status;
        static int buffer_low;
        static int buffer_empty;
        static int irq_pin;
        static ushort old_energy = 0;
        static ushort old_pitch = 0;
        static int[] old_k = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static ushort new_energy = 0;
        static ushort new_pitch = 0;
        static int[] new_k = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        /* these are all used to contain the current state of the sound generation */
        static ushort current_energy = 0;
        static ushort current_pitch = 0;
        static int[] current_k = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static ushort target_energy = 0;
        static ushort target_pitch = 0;
        static int[] target_k = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static int interp_count = 0;       /* number of interp periods (0-7) */
        static int sample_count = 0;       /* sample number within interp (0-24) */
        static int pitch_count = 0;

        static int[] u = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int[] x = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static int randbit = 0;
        
        static irqfunc irq_func = null;

        static ushort[] energytable ={
0x0000,0x00C0,0x0140,0x01C0,0x0280,0x0380,0x0500,0x0740,
0x0A00,0x0E40,0x1440,0x1C80,0x2840,0x38C0,0x5040,0x7FC0};
        static ushort[] pitchtable ={
0x0000,0x1000,0x1100,0x1200,0x1300,0x1400,0x1500,0x1600,
0x1700,0x1800,0x1900,0x1A00,0x1B00,0x1C00,0x1D00,0x1E00,
0x1F00,0x2000,0x2100,0x2200,0x2300,0x2400,0x2500,0x2600,
0x2700,0x2800,0x2900,0x2A00,0x2B00,0x2D00,0x2F00,0x3100,
0x3300,0x3500,0x3600,0x3900,0x3B00,0x3D00,0x3F00,0x4200,
0x4500,0x4700,0x4900,0x4D00,0x4F00,0x5100,0x5500,0x5700,
0x5C00,0x5F00,0x6300,0x6600,0x6A00,0x6E00,0x7300,0x7700,
0x7B00,0x8000,0x8500,0x8A00,0x8F00,0x9500,0x9A00,0xA000};
        static short[] k1table ={
unchecked((short)0x82C0),unchecked((short)0x8380),unchecked((short)0x83C0),unchecked((short)0x8440),unchecked((short)0x84C0),unchecked((short)0x8540),unchecked((short)0x8600),unchecked((short)0x8780),
unchecked((short)0x8880),unchecked((short)0x8980),unchecked((short)0x8AC0),unchecked((short)0x8C00),unchecked((short)0x8D40),unchecked((short)0x8F00),unchecked((short)0x90C0),unchecked((short)0x92C0),
unchecked((short)0x9900),unchecked((short)0xA140),unchecked((short)0xAB80),unchecked((short)0xB840),unchecked((short)0xC740),unchecked((short)0xD8C0),unchecked((short)0xEBC0),0x0000,
0x1440,0x2740,0x38C0,0x47C0,0x5480,0x5EC0,0x6700,0x6D40};
        static short[] k2table ={
unchecked((short)0xAE00),unchecked((short)0xB480),unchecked((short)0xBB80),unchecked((short)0xC340),unchecked((short)0xCB80),unchecked((short)0xD440),unchecked((short)0xDDC0),unchecked((short)0xE780),
unchecked((short)0xF180),unchecked((short)0xFBC0),0x0600,0x1040,0x1A40,0x2400,0x2D40,0x3600,
0x3E40,0x45C0,0x4CC0,0x5300,0x5880,0x5DC0,0x6240,0x6640,
0x69C0,0x6CC0,0x6F80,0x71C0,0x73C0,0x7580,0x7700,0x7E80};
        static short[] k3table ={
unchecked((short)0x9200),unchecked((short)0x9F00),unchecked((short)0xAD00),unchecked((short)0xBA00),unchecked((short)0xC800),unchecked((short)0xD500),unchecked((short)0xE300),unchecked((short)0xF000),
unchecked((short)0xFE00),0x0B00,0x1900,0x2600,0x3400,0x4100,0x4F00,0x5C00};
        static short[] k4table ={
unchecked((short)0xAE00),unchecked((short)0xBC00),unchecked((short)0xCA00),unchecked((short)0xD800),unchecked((short)0xE600),unchecked((short)0xF400),0x0100,0x0F00,
0x1D00,0x2B00,0x3900,0x4700,0x5500,0x6300,0x7100,0x7E00};
        static short[] k5table ={
unchecked((short)0xAE00),unchecked((short)0xBA00),unchecked((short)0xC500),unchecked((short)0xD100),unchecked((short)0xDD00),unchecked((short)0xE800),unchecked((short)0xF400),unchecked((short)0xFF00),
0x0B00,0x1700,0x2200,0x2E00,0x3900,0x4500,0x5100,0x5C00};
        static short[] k6table ={
unchecked((short)0xC000),unchecked((short)0xCB00),unchecked((short)0xD600),unchecked((short)0xE100),unchecked((short)0xEC00),unchecked((short)0xF700),0x0300,0x0E00,
0x1900,0x2400,0x2F00,0x3A00,0x4500,0x5000,0x5B00,0x6600};
        static short[] k7table ={
unchecked((short)0xB300),unchecked((short)0xBF00),unchecked((short)0xCB00),unchecked((short)0xD700),unchecked((short)0xE300),unchecked((short)0xEF00),unchecked((short)0xFB00),0x0700,
0x1300,0x1F00,0x2B00,0x3700,0x4300,0x4F00,0x5A00,0x6600};
        static short[] k8table ={
unchecked((short)0xC000),unchecked((short)0xD800),unchecked((short)0xF000),0x0700,0x1F00,0x3700,0x4F00,0x6600};
        static short[] k9table ={
unchecked((short)0xC000),unchecked((short)0xD400),unchecked((short)0xE800),unchecked((short)0xFC00),0x1000,0x2500,0x3900,0x4D00};
        static short[] k10table ={
unchecked((short)0xCD00),unchecked((short)0xDF00),unchecked((short)0xF100),0x0400,0x1600,0x2000,0x3B00,0x4D00};
        static byte[] chirptable ={
0x00, 0x2a, (byte)0xd4, 0x32,
(byte)0xb2, 0x12, 0x25, 0x14,
0x02, (byte)0xe1, (byte)0xc5, 0x02,
0x5f, 0x5a, 0x05, 0x0f,
0x26, (byte)0xfc, (byte)0xa5, (byte)0xa5,
(byte)0xd6, (byte)0xdd, (byte)0xdc, (byte)0xfc,
0x25, 0x2b, 0x22, 0x21,
0x0f, (byte)0xff, (byte)0xf8, (byte)0xee,
(byte)0xed, (byte)0xef, (byte)0xf7, (byte)0xf6,
(byte)0xfa, 0x00, 0x03, 0x02,
0x01
};

        static byte[] interp_coeff = { 8, 8, 8, 4, 4, 2, 2, 1 };


        static void tms5220_data_write(int data)
        {
            /* add this byte to the FIFO */
            if (fifo_count < FIFO_SIZE)
            {
                fifo[fifo_tail] = (byte)data;
                fifo_tail = (fifo_tail + 1) % FIFO_SIZE;
                fifo_count++;

#if DEBUG_5220
            if (f) fprintf(f, "Added byte to FIFO (size=%2d)\n", fifo_count);
#endif
            }
            else
            {
#if DEBUG_5220
            if (f) fprintf(f, "Ran out of room in the FIFO!\n");
#endif
            }

            /* update the buffer low state */
            check_buffer_low();
        }
        static void set_interrupt_state(int state)
        {
            if (irq_func != null && state != irq_pin)
                irq_func(state);
            irq_pin = state;
        }
        static void check_buffer_low()
        {
            /* did we just become low? */
            if (fifo_count < 8)
            {
                /* generate an interrupt if necessary */
                if (buffer_low == 0)
                    set_interrupt_state(1);
                buffer_low = 1;

#if DEBUG_5220
            if (f) fprintf(f, "Buffer low set\n");
#endif
            }

            /* did we just become full? */
            else
            {
                buffer_low = 0;

#if DEBUG_5220
            if (f) fprintf(f, "Buffer low cleared\n");
#endif
            }
        }
        static bool tms5220_ready_read()
        {
            return (fifo_count < FIFO_SIZE - 1);
        }
        static int extract_bits(int count)
        {
            int val = 0;

            while (count-- != 0)
            {
                val = (val << 1) | ((fifo[fifo_head] >> bits_taken) & 1);
                bits_taken++;
                if (bits_taken >= 8)
                {
                    fifo_count--;
                    fifo_head = (fifo_head + 1) % FIFO_SIZE;
                    bits_taken = 0;
                }
            }
            return val;
        }
        static int parse_frame(int removeit)
        {
            int old_head, old_taken, old_count;
            int bits, indx, i, rep_flag;

            /* remember previous frame */
            old_energy = new_energy;
            old_pitch = new_pitch;
            for (i = 0; i < 10; i++)
                old_k[i] = new_k[i];

            /* clear out the new frame */
            new_energy = 0;
            new_pitch = 0;
            for (i = 0; i < 10; i++)
                new_k[i] = 0;

            /* if the previous frame was a stop frame, don't do anything */
            if (old_energy == (energytable[15] >> 6))
                return 1;

            /* remember the original FIFO counts, in case we don't have enough bits */
            old_count = fifo_count;
            old_head = fifo_head;
            old_taken = bits_taken;

            /* count the total number of bits available */
            bits = fifo_count * 8 - bits_taken;

            /* attempt to extract the energy index */
            bits -= 4;
            if (bits < 0)
                goto ranout;
            indx = extract_bits(4);
            new_energy = (ushort)(energytable[indx] >> 6);

            /* if the index is 0 or 15, we're done */
            if (indx == 0 || indx == 15)
            {
#if DEBUG_5220
			if (f) fprintf(f, "  (4-bit energy=%d frame)\n",new_energy);
#endif

                /* clear fifo if stop frame encountered */
                if (indx == 15)
                {
                    fifo_head = fifo_tail = fifo_count = bits_taken = 0;
                    removeit = 1;
                }
                goto done;
            }

            /* attempt to extract the repeat flag */
            bits -= 1;
            if (bits < 0)
                goto ranout;
            rep_flag = extract_bits(1);

            /* attempt to extract the pitch */
            bits -= 6;
            if (bits < 0)
                goto ranout;
            indx = extract_bits(6);
            new_pitch = (ushort)(pitchtable[indx] / 256);

            /* if this is a repeat frame, just copy the k's */
            if (rep_flag != 0)
            {
                for (i = 0; i < 10; i++)
                    new_k[i] = old_k[i];

#if DEBUG_5220
            if (f) fprintf(f, "  (11-bit energy=%d pitch=%d rep=%d frame)\n", new_energy, new_pitch, rep_flag);
#endif
                goto done;
            }

            /* if the pitch index was zero, we need 4 k's */
            if (indx == 0)
            {
                /* attempt to extract 4 K's */
                bits -= 18;
                if (bits < 0)
                    goto ranout;
                new_k[0] = k1table[extract_bits(5)];
                new_k[1] = k2table[extract_bits(5)];
                new_k[2] = k3table[extract_bits(4)];
                new_k[3] = k4table[extract_bits(4)];

#if DEBUG_5220
            if (f) fprintf(f, "  (29-bit energy=%d pitch=%d rep=%d 4K frame)\n", new_energy, new_pitch, rep_flag);
#endif
                goto done;
            }

            /* else we need 10 K's */
            bits -= 39;
            if (bits < 0)
                goto ranout;
            new_k[0] = k1table[extract_bits(5)];
            new_k[1] = k2table[extract_bits(5)];
            new_k[2] = k3table[extract_bits(4)];
            new_k[3] = k4table[extract_bits(4)];
            new_k[4] = k5table[extract_bits(4)];
            new_k[5] = k6table[extract_bits(4)];
            new_k[6] = k7table[extract_bits(4)];
            new_k[7] = k8table[extract_bits(3)];
            new_k[8] = k9table[extract_bits(3)];
            new_k[9] = k10table[extract_bits(3)];

#if DEBUG_5220
        if (f) fprintf(f, "  (50-bit energy=%d pitch=%d rep=%d 10K frame)\n", new_energy, new_pitch, rep_flag);
#endif

        done:

#if DEBUG_5220
        if (f) fprintf(f, "Parsed a frame successfully - %d bits remaining\n", bits);
#endif

            /* if we're not to remove this one, restore the FIFO */
            if (removeit == 0)
            {
                fifo_count = old_count;
                fifo_head = old_head;
                bits_taken = old_taken;
            }

            /* update the buffer_low status */
            check_buffer_low();
            return 1;

        ranout:

#if DEBUG_5220
        if (f) fprintf(f, "Ran out of bits on a parse!\n");
#endif

            /* this is an error condition; mark the buffer empty and turn off speaking */
            buffer_empty = 1;
            talk_status = speak_external = 0;
            fifo_count = fifo_head = fifo_tail = 0;

            /* generate an interrupt if necessary */
            set_interrupt_state(1);
            return 0;
        }
        static void tms5220_process(short[] buffer, uint size)
        {
            int buf_count = 0;
            int i, interp_period;

        tryagain:

            /* if we're not speaking, parse commands */
            while (speak_external == 0 && fifo_count > 0)
                process_command();

            /* if there's nothing to do, bail */
            if (size == 0)
                return;

            /* if we're empty and still not speaking, fill with nothingness */
            if (speak_external == 0)
                goto empty;

            /* if we're to speak, but haven't started, wait for the 9th byte */
            if (talk_status == 0)
            {
                if (fifo_count < 9)
                    goto empty;

                /* parse but don't remove the first frame, and set the status to 1 */
                parse_frame(0);
                talk_status = 1;
                buffer_empty = 0;
            }

            /* loop until the buffer is full or we've stopped speaking */
            while ((size > 0) && speak_external != 0)
            {
                int current_val;

                /* if we're ready for a new frame */
                if ((interp_count == 0) && (sample_count == 0))
                {
                    /* Parse a new frame */
                    if (parse_frame(1) == 0)
                        break;

                    /* Set old target as new start of frame */
                    current_energy = old_energy;
                    current_pitch = old_pitch;
                    for (i = 0; i < 10; i++)
                        current_k[i] = old_k[i];

                    /* is this a zero energy frame? */
                    if (current_energy == 0)
                    {
                        /*printf("processing frame: zero energy\n");*/
                        target_energy = 0;
                        target_pitch = current_pitch;
                        for (i = 0; i < 10; i++)
                            target_k[i] = current_k[i];
                    }

                    /* is this a stop frame? */
                    else if (current_energy == (energytable[15] >> 6))
                    {
                        /*printf("processing frame: stop frame\n");*/
                        current_energy = (ushort)(energytable[0] >> 6);
                        target_energy = current_energy;
                        speak_external = talk_status = 0;
                        interp_count = sample_count = pitch_count = 0;

                        /* generate an interrupt if necessary */
                        set_interrupt_state(1);

                        /* try to fetch commands again */
                        goto tryagain;
                    }
                    else
                    {
                        /* is this the ramp down frame? */
                        if (new_energy == (energytable[15] >> 6))
                        {
                            /*printf("processing frame: ramp down\n");*/
                            target_energy = 0;
                            target_pitch = current_pitch;
                            for (i = 0; i < 10; i++)
                                target_k[i] = current_k[i];
                        }
                        /* Reset the step size */
                        else
                        {
                            /*printf("processing frame: Normal\n");*/
                            /*printf("*** Energy = %d\n",current_energy);*/
                            /*printf("proc: %d %d\n",last_fbuf_head,fbuf_head);*/

                            target_energy = new_energy;
                            target_pitch = new_pitch;

                            for (i = 0; i < 4; i++)
                                target_k[i] = new_k[i];
                            if (current_pitch == 0)
                                for (i = 4; i < 10; i++)
                                {
                                    target_k[i] = current_k[i] = 0;
                                }
                            else
                                for (i = 4; i < 10; i++)
                                    target_k[i] = new_k[i];
                        }
                    }
                }
                else if (interp_count == 0)
                {
                    /* Update values based on step values */
                    /*printf("\n");*/

                    interp_period = sample_count / 25;
                    current_energy += (ushort)((target_energy - current_energy) / interp_coeff[interp_period]);
                    if (old_pitch != 0)
                        current_pitch += (ushort)((target_pitch - current_pitch) / interp_coeff[interp_period]);

                    /*printf("*** Energy = %d\n",current_energy);*/

                    for (i = 0; i < 10; i++)
                    {
                        current_k[i] += (target_k[i] - current_k[i]) / interp_coeff[interp_period];
                    }
                }

                if (old_energy == 0)
                {
                    /* generate silent samples here */
                    current_val = 0x00;
                }
                else if (old_pitch == 0)
                {
                    /* generate unvoiced samples here */
                    randbit = (Mame.rand() % 2) * 2 - 1;
                    current_val = (randbit * current_energy) / 4;
                }
                else
                {
                    /* generate voiced samples here */
                    if (pitch_count < chirptable.Length)
                        current_val = (chirptable[pitch_count] * current_energy) / 256;
                    else
                        current_val = 0x00;
                }

                /* Lattice filter here */

                u[10] = current_val;

                for (i = 9; i >= 0; i--)
                {
                    u[i] = u[i + 1] - ((current_k[i] * x[i]) / 32768);
                }
                for (i = 9; i >= 1; i--)
                {
                    x[i] = x[i - 1] + ((current_k[i - 1] * u[i - 1]) / 32768);
                }

                x[0] = u[0];

                /* clipping, just like the chip */

                if (u[0] > 511)
                    buffer[buf_count] = 127 << 8;
                else if (u[0] < -512)
                    buffer[buf_count] = -128 << 8;
                else
                    buffer[buf_count] = (short)(u[0] << 6);

                /* Update all counts */

                size--;
                sample_count = (sample_count + 1) % 200;

                if (current_pitch != 0)
                    pitch_count = (pitch_count + 1) % current_pitch;
                else
                    pitch_count = 0;

                interp_count = (interp_count + 1) % 25;
                buf_count++;
            }

        empty:

            while (size > 0)
            {
                buffer[buf_count] = 0x00;
                buf_count++;
                size--;
            }
        }
        static void process_command()
        {
            byte cmd;

            /* if there are stray bits, ignore them */
            if (bits_taken != 0)
            {
                bits_taken = 0;
                fifo_count--;
                fifo_head = (fifo_head + 1) % FIFO_SIZE;
            }

            /* grab a full byte from the FIFO */
            if (fifo_count > 0)
            {
                cmd = (byte)(fifo[fifo_head] & 0x70);
                fifo_count--;
                fifo_head = (fifo_head + 1) % FIFO_SIZE;

                /* only real command we handle now is speak external */
                if (cmd == 0x60)
                {
                    speak_external = 1;

                    /* according to the datasheet, this will cause an interrupt due to a BE condition */
                    if (buffer_empty == 0)
                    {
                        buffer_empty = 1;
                        set_interrupt_state(1);
                    }
                }
            }

            /* update the buffer low state */
            check_buffer_low();
        }

    }
}
