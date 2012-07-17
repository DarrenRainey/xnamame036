using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        public const byte MAXPOKEYS = 4;

    }
    public delegate int pot_delegate(int offset);
    public delegate void pokey_serout(int offset, int data);

    public class POKEYinterface
    {
        public POKEYinterface(int num, int baseclock, int[] mixing_level,
            pot_delegate[] pot0_r, pot_delegate[] pot1_r, pot_delegate[] pot2_r,
            pot_delegate[] pot3_r, pot_delegate[] pot4_r, pot_delegate[] pot5_r,
            pot_delegate[] pot6_r, pot_delegate[] pot7_r, pot_delegate[] allpot_r,
            pot_delegate[] serin_r = null, pokey_serout[] serout_w = null, Mame.irqcallback[] interrupt_cb = null)
        {
            this.num = num; this.baseclock = baseclock; this.mixing_level = mixing_level;
            this.pot0_r = pot0_r;
            this.pot1_r = pot1_r;
            this.pot2_r = pot2_r;
            this.pot3_r = pot3_r;
            this.pot4_r = pot4_r;
            this.pot5_r = pot5_r;
            this.pot6_r = pot6_r;
            this.pot7_r = pot7_r;
            this.allpot_r = allpot_r;
            if (serin_r == null)
                this.serin_r = new pot_delegate[] { null };
            else
                this.serin_r = serin_r;
            if (serout_w == null)
                this.serout_w = new pokey_serout[] { null };
            else
                this.serout_w = serout_w;
            if (interrupt_cb == null)
                this.interrupt_cb = new Mame.irqcallback[] { null };
            else
                this.interrupt_cb = interrupt_cb;
        }
        public int num;    /* total number of pokeys in the machine */
        public int baseclock;
        public int[] mixing_level = new int[Mame.MAXPOKEYS];
        public pot_delegate[] pot0_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot1_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot2_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot3_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot4_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot5_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot6_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] pot7_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] allpot_r = new pot_delegate[Mame.MAXPOKEYS];
        public pot_delegate[] serin_r = new pot_delegate[Mame.MAXPOKEYS];
        public pokey_serout[] serout_w = new pokey_serout[Mame.MAXPOKEYS];
        public Mame.irqcallback[] interrupt_cb = new Mame.irqcallback[Mame.MAXPOKEYS];
    }
    class Pokey : Mame.snd_interface
    {
        public Pokey()
        {
            this.sound_num = Mame.SOUND_POKEY;
            this.name = "Pokey";
    }
        class POKEYregisters
        {
            public int[] counter = new int[4];		/* channel counter */
            public int[] divisor = new int[4];		/* channel divisor (modulo value) */
            public uint[] volume = new uint[4];		/* channel volume - derived */
            public byte[] output = new byte[4];		/* channel output signal (1 active, 0 inactive) */
            public bool[] audible = new bool[4];		/* channel plays an audible tone/effect */
            public uint samplerate_24_8; /* sample rate in 24.8 format */
            public uint samplepos_fract; /* sample position fractional part */
            public uint samplepos_whole; /* sample position whole part */
            public uint polyadjust;		/* polynome adjustment */
            public uint p4;              /* poly4 index */
            public uint p5;              /* poly5 index */
            public uint p9;              /* poly9 index */
            public uint p17;             /* poly17 index */
            public uint r9;				/* rand9 index */
            public uint r17;             /* rand17 index */
            public uint clockmult;		/* clock multiplier */
            public int channel;            /* streams channel */
            public Mame.Timer.timer_entry[] timer = new Mame.Timer.timer_entry[3]; 		/* timers for channel 1,2 and 4 events */
            public Mame.Timer.timer_entry rtimer;           /* timer for calculating the random offset */
            public Mame.Timer.timer_entry[] ptimer = new Mame.Timer.timer_entry[8];		/* pot timers */
            public pot_delegate[] pot_r = new pot_delegate[8];
            public pot_delegate allpot_r;
            public pot_delegate serin_r;
            public pokey_serout serout_w;
            public Mame.irqcallback interrupt_cb;
            public byte[] AUDF = new byte[4];          /* AUDFx (D200, D202, D204, D206) */
            public byte[] AUDC = new byte[4];			/* AUDCx (D201, D203, D205, D207) */
            public byte[] POTx = new byte[8];			/* POTx   (R/D200-D207) */
            public byte AUDCTL;			/* AUDCTL (W/D208) */
            public byte ALLPOT;			/* ALLPOT (R/D208) */
            public byte KBCODE;			/* KBCODE (R/D209) */
            public byte RANDOM;			/* RANDOM (R/D20A) */
            public byte SERIN;			/* SERIN  (R/D20D) */
            public byte SEROUT;			/* SEROUT (W/D20D) */
            public byte IRQST;			/* IRQST  (R/D20E) */
            public byte IRQEN;			/* IRQEN  (W/D20E) */
            public byte SKSTAT;			/* SKSTAT (R/D20F) */
            public byte SKCTL;			/* SKCTL  (W/D20F) */
        }
        Mame._stream_callback[] _update = { pokey_update, pokey_update, pokey_update, pokey_update };
        static void pokey_update(int chip, _ShortPtr buffer, int length)
        {
            uint sum = 0;
            if (_pokey[chip].output[CHAN1] != 0)
                sum += _pokey[chip].volume[CHAN1];
            if (_pokey[chip].output[CHAN2] != 0)
                sum += _pokey[chip].volume[CHAN2];
            if (_pokey[chip].output[CHAN3] != 0)
                sum += _pokey[chip].volume[CHAN3];
            if (_pokey[chip].output[CHAN4] != 0)
                sum += _pokey[chip].volume[CHAN4];
            while (length > 0)
            {
                uint _event = _pokey[chip].samplepos_whole;
                uint channel = unchecked((uint)-1);
                if (_pokey[chip].counter[CHAN1] < _event)
                {
                    _event = (uint)_pokey[chip].counter[CHAN1];
                    channel = CHAN1;
                }
                if (_pokey[chip].counter[CHAN2] < _event)
                {
                    _event = (uint)_pokey[chip].counter[CHAN2];
                    channel = CHAN2;
                }
                if (_pokey[chip].counter[CHAN3] < _event)
                {
                    _event = (uint)_pokey[chip].counter[CHAN3];
                    channel = CHAN3;
                }
                if (_pokey[chip].counter[CHAN4] < _event)
                {
                    _event = (uint)_pokey[chip].counter[CHAN4];
                    channel = CHAN4;
                }
                if (channel == unchecked((uint)-1))
                {
                    _pokey[chip].counter[CHAN1] -= (int)_event;
                    _pokey[chip].counter[CHAN2] -= (int)_event;
                    _pokey[chip].counter[CHAN3] -= (int)_event;
                    _pokey[chip].counter[CHAN4] -= (int)_event;
                    _pokey[chip].samplepos_whole -= _event;
                    _pokey[chip].polyadjust += _event;

                    /* adjust the sample position */
                    _pokey[chip].samplepos_fract += _pokey[chip].samplerate_24_8;
                    if ((_pokey[chip].samplepos_fract & 0xffffff00) != 0)
                    {
                        _pokey[chip].samplepos_whole += _pokey[chip].samplepos_fract >> 8;
                        _pokey[chip].samplepos_fract &= 0x000000ff;
                    }
                    /* store sum of output signals into the buffer */
                    buffer.write16(0, (ushort)((sum > 65535) ? 0x7fff : sum - 0x8000));
                    buffer.offset += 2;
                    length--;
                }
                else
                {
                    int toggle = 0;
                    _pokey[chip].counter[CHAN1] -= (int)_event;
                    _pokey[chip].counter[CHAN2] -= (int)_event;
                    _pokey[chip].counter[CHAN3] -= (int)_event;
                    _pokey[chip].counter[CHAN4] -= (int)_event;
                    _pokey[chip].samplepos_whole -= _event;
                    _pokey[chip].polyadjust += _event;
                    /* reset the channel counter */
                    if (_pokey[chip].audible[channel])
                        _pokey[chip].counter[channel] = _pokey[chip].divisor[channel];
                    else
                        _pokey[chip].counter[channel] = 0x7fffffff;
                    _pokey[chip].p4 = (_pokey[chip].p4 + _pokey[chip].polyadjust) % 0x0000f;
                    _pokey[chip].p5 = (_pokey[chip].p5 + _pokey[chip].polyadjust) % 0x0001f;
                    _pokey[chip].p9 = (_pokey[chip].p9 + _pokey[chip].polyadjust) % 0x001ff;
                    _pokey[chip].p17 = (_pokey[chip].p17 + _pokey[chip].polyadjust) % 0x1ffff;
                    _pokey[chip].polyadjust = 0;
                    if ((_pokey[chip].AUDC[channel] & NOTPOLY5) != 0 || poly5[_pokey[chip].p5] != 0)
                    {
                        if ((_pokey[chip].AUDC[channel] & PURE) != 0)
                            toggle = 1;
                        else
                            if ((_pokey[chip].AUDC[channel] & POLY4) != 0)
                                toggle = _pokey[chip].output[channel] == (poly4[_pokey[chip].p4] == 0 ? 1 : 0) ? 1 : 0;
                            else
                                if ((_pokey[chip].AUDCTL & POLY9) != 0)
                                    toggle = _pokey[chip].output[channel] == (poly9[_pokey[chip].p9] == 0 ? 1 : 0) ? 1 : 0;
                                else
                                    toggle = _pokey[chip].output[channel] == (poly17[_pokey[chip].p17] == 0 ? 1 : 0) ? 1 : 0;
                    }
                    if (toggle != 0)
                    {
                        if (_pokey[chip].audible[channel])
                        {
                            if (_pokey[chip].output[channel] != 0)
                                sum -= _pokey[chip].volume[channel];
                            else
                                sum += _pokey[chip].volume[channel];
                        }
                        _pokey[chip].output[channel] ^= 1;
                    }
                    /* is this a filtering channel (3/4) and is the filter active? */
                    if ((_pokey[chip].AUDCTL & ((CH1_FILTER | CH2_FILTER) & (0x10 >> (int)channel))) != 0)
                    {
                        if (_pokey[chip].output[channel] != 0)
                        {
                            _pokey[chip].output[channel - 2] = 0;
                            if (_pokey[chip].audible[channel])
                                sum -= _pokey[chip].volume[channel - 2];
                        }
                    }
                }
            }
            Mame.Timer.timer_reset(_pokey[chip].rtimer, Mame.Timer.TIME_NEVER);
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            return ((POKEYinterface)msound.sound_interface).baseclock;
        }
        public override int chips_num(Mame.MachineSound msound)
        {
           return ((POKEYinterface)msound.sound_interface).num;
        }
        public override void reset()
        {
            //nothing
        }
        public override int start(Mame.MachineSound msound)
        {
            intf = (POKEYinterface)msound.sound_interface;

            poly9 = new byte[0x1ff + 1];
            rand9 = new byte[0x1ff + 1];
            poly17 = new byte[0x1ffff + 1];
            rand17 = new byte[0x1ffff + 1];

            /* initialize the poly counters */
            poly_init(poly4, 4, 3, 1, 0x00004);
            poly_init(poly5, 5, 3, 2, 0x00008);
            poly_init(poly9, 9, 2, 7, 0x00080);
            poly_init(poly17, 17, 7, 10, 0x18000);

            /* initialize the random arrays */
            rand_init(rand9, 9, 2, 7, 0x00080);
            rand_init(rand17, 17, 7, 10, 0x18000);

            for (int chip = 0; chip < intf.num; chip++)
            {
                _pokey[chip] = new POKEYregisters();
                string name = "";

                //memset(p, 0, sizeof(struct POKEYregisters));

                _pokey[chip].samplerate_24_8 = (uint)((Mame.Machine.sample_rate) != 0 ? (intf.baseclock << 8) / Mame.Machine.sample_rate : 1);
                _pokey[chip].divisor[CHAN1] = 4;
                _pokey[chip].divisor[CHAN2] = 4;
                _pokey[chip].divisor[CHAN3] = 4;
                _pokey[chip].divisor[CHAN4] = 4;
                _pokey[chip].clockmult = DIV_64;
                _pokey[chip].KBCODE = 0x09;		 /* Atari 800 'no key' */
                _pokey[chip].SKCTL = SK_RESET;	 /* let the RNG run after reset */
                _pokey[chip].rtimer = Mame.Timer.timer_set(Mame.Timer.TIME_NEVER, chip, null);

                _pokey[chip].pot_r[0] = intf.pot0_r[chip];
                _pokey[chip].pot_r[1] = intf.pot1_r[chip];
                _pokey[chip].pot_r[2] = intf.pot2_r[chip];
                _pokey[chip].pot_r[3] = intf.pot3_r[chip];
                _pokey[chip].pot_r[4] = intf.pot4_r[chip];
                _pokey[chip].pot_r[5] = intf.pot5_r[chip];
                _pokey[chip].pot_r[6] = intf.pot6_r[chip];
                _pokey[chip].pot_r[7] = intf.pot7_r[chip];
                _pokey[chip].allpot_r = intf.allpot_r[chip];
                _pokey[chip].serin_r = intf.serin_r[chip];
                _pokey[chip].serout_w = intf.serout_w[chip];
                _pokey[chip].interrupt_cb = intf.interrupt_cb[chip];

                name = Mame.sprintf(name, "Pokey #%d", chip);
                _pokey[chip].channel = Mame.stream_init(name, intf.mixing_level[chip], Mame.Machine.sample_rate, chip, _update[chip]);

                if (_pokey[chip].channel == -1)
                {
                    Mame.printf("failed to initialize sound channel");
                    return 1;
                }
            }

            return 0;
        }
        public override void stop()
        {
            throw new NotImplementedException();
        }
        public override void update()
        {
            //nothing
        }
        const int POKEY_DEFAULT_GAIN = 32767 / 11 / 4;
        const byte SK_FRAME = 0x80;
        const byte SK_OVERRUN = 0x40;
        const byte SK_KBERR = 0x20;
        const byte SK_SERIN = 0x10;
        const byte SK_KEYBD = 0x04;
        const byte SK_SEROUT = 0x02;
        const byte SK_BREAK = 0x80;
        const byte SK_BPS = 0x70;
        const byte SK_FM = 0x08;
        const byte SK_RESET = 0x03;

        const byte SK_PADDLE = 0x04;
        const byte POT0_C = 0x00;
        const byte POT1_C = 0x01;
        const byte POT2_C = 0x02;
        const byte POT3_C = 0x03;
        const byte POT4_C = 0x04;
        const byte POT5_C = 0x05;
        const byte POT6_C = 0x06;
        const byte POT7_C = 0x07;
        const byte ALLPOT_C = 0x08;
        const byte KBCODE_C = 0x09;
        const byte RANDOM_C = 0x0a;
        const byte SERIN_C = 0x0d;
        const byte IRQST_C = 0x0e;
        const byte SKSTAT_C = 0x0f;

        const byte AUDF1_C = 0x00;
        const byte AUDC1_C = 0x01;
        const byte AUDF2_C = 0x02;
        const byte AUDC2_C = 0x03;
        const byte AUDF3_C = 0x04;
        const byte AUDC3_C = 0x05;
        const byte AUDF4_C = 0x06;
        const byte AUDC4_C = 0x07;
        const byte AUDCTL_C = 0x08;
        const byte STIMER_C = 0x09;
        const byte SKREST_C = 0x0A;
        const byte POTGO_C = 0x0B;
        const byte SEROUT_C = 0x0D;
        const byte IRQEN_C = 0x0E;
        const byte SKCTL_C = 0x0F;

        const byte CHAN1 = 0, CHAN2 = 1, CHAN3 = 2, CHAN4 = 3;
        const byte TIMER1 = 0, TIMER2 = 1, TIMER4 = 2;

        const byte DIVADD_LOCLK = 1, DIVADD_HICLK = 4, DIVADD_HICLK_JOINED = 7;
        const byte NOTPOLY5 = 0x80, POLY4 = 0x40, PURE = 0x20, VOLUME_ONLY = 0x10, VOLUME_MASK = 0x0f;

        const byte POLY9 = 0x80;
        const byte CH1_HICLK = 0x40;
        const byte CH3_HICLK = 0x20;/* selects 1.78979 MHz for Ch 3 */
        const byte CH12_JOINED = 0x10;/* clocks channel 1 w/channel 2 */
        const byte CH34_JOINED = 0x08;/* clocks channel 3 w/channel 4 */
        const byte CH1_FILTER = 0x04;/* selects channel 1 high pass filter */
        const byte CH2_FILTER = 0x02;/* selects channel 2 high pass filter */
        const byte CLK_15KHZ = 0x01;	/* selects 15.6999 kHz or 63.9211 kHz */

        const byte DIV_64 = 28;
        const byte DIV_15 = 114;

        const byte IRQ_BREAK = 0x80;/* BREAK key pressed interrupt */
        const byte IRQ_KEYBD = 0x40;/* keyboard data ready interrupt */
        const byte IRQ_SERIN = 0x20;/* serial input data ready interrupt */
        const byte IRQ_SEROR = 0x10;/* serial output register ready interrupt */
        const byte IRQ_SEROC = 0x08;/* serial output complete interrupt */
        const byte IRQ_TIMR4 = 0x04;/* timer channel #4 interrupt */
        const byte IRQ_TIMR2 = 0x02;/* timer channel #2 interrupt */
        const byte IRQ_TIMR1 = 0x01;/* timer channel #1 interrupt */

        const int FREQ_17_EXACT = 1789790;
        static byte[] rand9, rand17, poly9, poly17;
        static byte[] poly4 = new byte[0x0f], poly5 = new byte[0x1f];

        static POKEYinterface intf;
        static POKEYregisters[] _pokey = new POKEYregisters[Mame.MAXPOKEYS];

        static Pokey()
        {
            for (int i = 0; i < Mame.MAXPOKEYS; i++)
                _pokey[i] = new POKEYregisters();
        }

        static void pokey_pot_trigger(int param)
        {
            int chip = param >> 3;
            int pot = param & 7;
            POKEYregisters p = _pokey[chip];

            //LOG((errorlog, "POKEY #%d POT%d triggers after %dus\n", chip, pot, (int)(1000000ul*timer_timeelapsed(p.ptimer[pot]))));
            p.ptimer[pot] = null;
            p.ALLPOT &= (byte)~(1 << pot);	/* set the enabled timer irq status bits */
        }
        static void pokey_serout_ready(int chip)
        {
            POKEYregisters p = _pokey[chip];
            if ((p.IRQEN & IRQ_SEROR) != 0)
            {
                p.IRQST |= IRQ_SEROR;
                if (p.interrupt_cb != null)
                    p.interrupt_cb(IRQ_SEROR);
            }
        }

        static void pokey_serout_complete(int chip)
        {
            POKEYregisters p = _pokey[chip];
            if ((p.IRQEN & IRQ_SEROC) != 0)
            {
                p.IRQST |= IRQ_SEROC;
                if (p.interrupt_cb != null)
                    p.interrupt_cb(IRQ_SEROC);
            }
        }
        static void pokey_potgo(int chip)
        {
            POKEYregisters p = _pokey[chip];
            int pot;

            //LOG((errorlog, "POKEY #%d pokey_potgo\n", chip));

            p.ALLPOT = 0xff;

            for (pot = 0; pot < 8; pot++)
            {
                if (p.ptimer[pot] != null)
                {
                    Mame.Timer.timer_remove(p.ptimer[pot]);
                    p.ptimer[pot] = null;
                    p.POTx[pot] = 0xff;
                }
                if (p.pot_r[pot] != null)
                {
                    int r = p.pot_r[pot](pot);
                    //LOG((errorlog, "POKEY #%d pot_r(%d) returned $%02x\n", chip, pot, r));
                    if (r != -1)
                    {
                        if (r > 228)
                            r = 228;
                        /* final value */
                        p.POTx[pot] = (byte)r;
                        p.ptimer[pot] = Mame.Timer.timer_set(Mame.Timer.TIME_IN_USEC(r * (double)(((p.SKCTL & SK_PADDLE) != 0 ? 64.0 * 2 / 228 : 64.0) * FREQ_17_EXACT / intf.baseclock)), (chip << 3) | pot, pokey_pot_trigger);
                    }
                }
            }
        }

        static void pokey_timer_expire(int param)
        {
            int chip = param >> 3;
            int timers = param & 7;
            POKEYregisters p = _pokey[chip];

            //LOG_TIMER((errorlog, "POKEY #%d timer %d with IRQEN $%02x\n", chip, param, p.IRQEN));

            /* check if some of the requested timer interrupts are enabled */
            timers &= p.IRQEN;

            if (timers != 0)
            {
                /* set the enabled timer irq status bits */
                p.IRQST |= (byte)timers;
                /* call back an application supplied function to handle the interrupt */
                if (p.interrupt_cb != null)
                    p.interrupt_cb(timers);
            }
        }


        static int pokey_register_r(int chip, int offs)
        {
            POKEYregisters p = _pokey[chip];
            int data = 0, pot;


            switch (offs & 15)
            {
                case POT0_C:
                case POT1_C:
                case POT2_C:
                case POT3_C:
                case POT4_C:
                case POT5_C:
                case POT6_C:
                case POT7_C:
                    pot = offs & 7;
                    if (p.pot_r[pot] != null)
                    {
                        /*
                         * If the conversion is not yet finished (ptimer running),
                         * get the current value by the linear interpolation of
                         * the final value using the elapsed time.
                         */
                        if ((p.ALLPOT & (1 << pot)) != 0)
                        {
                            data = (byte)(Mame.Timer.timer_timeelapsed(p.ptimer[pot]) / (double)(((p.SKCTL & SK_PADDLE) != 0 ? 64.0 * 2 / 228 : 64.0) * FREQ_17_EXACT / intf.baseclock));
                            //LOG((errorlog,"POKEY #%d read POT%d (interpolated) $%02x\n", chip, pot, data));
                        }
                        else
                        {
                            data = p.POTx[pot];
                            //LOG((errorlog,"POKEY #%d read POT%d (final value)  $%02x\n", chip, pot, data));
                        }
                    }
                    //else if (errorlog) fprintf(errorlog,"PC %04x: warning - read p[chip] #%d POT%d\n", cpu_get_pc(), chip, pot);
                    break;

                case ALLPOT_C:
                    if (p.allpot_r != null)
                    {
                        data = p.allpot_r(offs);
                        //LOG((errorlog,"POKEY #%d ALLPOT callback $%02x\n", chip, data));
                    }
                    else
                    {
                        data = p.ALLPOT;
                        //LOG((errorlog,"POKEY #%d ALLPOT internal $%02x\n", chip, data));
                    }
                    break;

                case KBCODE_C:
                    data = p.KBCODE;
                    break;

                case RANDOM_C:
                    /****************************************************************
                     * If the 2 least significant bits of SKCTL are 0, the random
                     * number generator is disabled (SKRESET). Thanks to Eric Smith
                     * for pointing out this critical bit of info! If the random
                     * number generator is enabled, get a new random number. Take
                     * the time gone since the last read into account and read the
                     * new value from an appropriate offset in the rand17 table.
                     ****************************************************************/
                    if ((p.SKCTL & SK_RESET) != 0)
                    {
                        uint adjust = (uint)(Mame.Timer.timer_timeelapsed(p.rtimer) * intf.baseclock);
                        p.r9 = (p.r9 + adjust) % 0x001ff;
                        p.r17 = (p.r17 + adjust) % 0x1ffff;
                        if ((p.AUDCTL & POLY9) != 0)
                        {
                            p.RANDOM = rand9[p.r9];
                            //LOG_RAND((errorlog, "POKEY #%d adjust %u rand9[$%05x]: $%02x\n", chip, adjust, p.r9, p.RANDOM));
                        }
                        else
                        {
                            p.RANDOM = rand17[p.r17];
                            //LOG_RAND((errorlog, "POKEY #%d adjust %u rand17[$%05x]: $%02x\n", chip, adjust, p.r17, p.RANDOM));
                        }
                    }
                    else
                    {
                        //LOG_RAND((errorlog, "POKEY #%d rand17 freezed (SKCTL): $%02x\n", chip, p.RANDOM));
                    }
                    Mame.Timer.timer_reset(p.rtimer, Mame.Timer.TIME_NEVER);
                    data = p.RANDOM;
                    break;

                case SERIN_C:
                    if (p.serin_r != null)
                        p.SERIN = (byte)p.serin_r(offs);
                    data = p.SERIN;
                    //LOG((errorlog, "POKEY #%d SERIN  $%02x\n", chip, data));
                    break;

                case IRQST_C:
                    /* IRQST is an active low input port; we keep it active high */
                    /* internally to ease the (un-)masking of bits */
                    data = p.IRQST ^ 0xff;
                    //LOG((errorlog, "POKEY #%d IRQST  $%02x\n", chip, data));
                    break;

                case SKSTAT_C:
                    /* SKSTAT is also an active low input port */
                    data = p.SKSTAT ^ 0xff;
                    //LOG((errorlog, "POKEY #%d SKSTAT $%02x\n", chip, data));
                    break;

                default:
                    //LOG((errorlog, "POKEY #%d register $%02x\n", chip, offs));
                    break;
            }
            return data;
        }
        public static int quad_pokey_r(int offset)
        {
            int pokey_num = (offset >> 3) & ~0x04;
            int control = (offset & 0x20) >> 2;
            int pokey_reg = (offset % 8) | control;

            return pokey_register_r(pokey_num, pokey_reg);
        }

        public static void quad_pokey_w(int offset, int data)
        {
            int pokey_num = (offset >> 3) & ~0x04;
            int control = (offset & 0x20) >> 2;
            int pokey_reg = (offset % 8) | control;

            pokey_register_w(pokey_num, pokey_reg, data);
        }
        static void pokey_register_w(int chip, int offs, int data)
        {
            POKEYregisters p = _pokey[chip];
            int ch_mask = 0, new_val;


            Mame.stream_update(p.channel, 0);

            /* determine which address was changed */
            switch (offs & 15)
            {
                case AUDF1_C:
                    if (data == p.AUDF[CHAN1])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDF1  $%02x\n", chip, data));
                    p.AUDF[CHAN1] = (byte)data;
                    ch_mask = 1 << CHAN1;
                    if ((p.AUDCTL & CH12_JOINED) != 0)		/* if ch 1&2 tied together */
                        ch_mask |= 1 << CHAN2;    /* then also change on ch2 */
                    break;

                case AUDC1_C:
                    if (data == p.AUDC[CHAN1])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDC1  $%02x (%s)\n", chip, data, audc2str(data)));
                    p.AUDC[CHAN1] = (byte)data;
                    ch_mask = 1 << CHAN1;
                    break;

                case AUDF2_C:
                    if (data == p.AUDF[CHAN2])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDF2  $%02x\n", chip, data));
                    p.AUDF[CHAN2] = (byte)data;
                    ch_mask = 1 << CHAN2;
                    break;

                case AUDC2_C:
                    if (data == p.AUDC[CHAN2])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDC2  $%02x (%s)\n", chip, data, audc2str(data)));
                    p.AUDC[CHAN2] = (byte)data;
                    ch_mask = 1 << CHAN2;
                    break;

                case AUDF3_C:
                    if (data == p.AUDF[CHAN3])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDF3  $%02x\n", chip, data));
                    p.AUDF[CHAN3] = (byte)data;
                    ch_mask = 1 << CHAN3;

                    if ((p.AUDCTL & CH34_JOINED) != 0)	/* if ch 3&4 tied together */
                        ch_mask |= 1 << CHAN4;  /* then also change on ch4 */
                    break;

                case AUDC3_C:
                    if (data == p.AUDC[CHAN3])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDC3  $%02x (%s)\n", chip, data, audc2str(data)));
                    p.AUDC[CHAN3] = (byte)data;
                    ch_mask = 1 << CHAN3;
                    break;

                case AUDF4_C:
                    if (data == p.AUDF[CHAN4])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDF4  $%02x\n", chip, data));
                    p.AUDF[CHAN4] = (byte)data;
                    ch_mask = 1 << CHAN4;
                    break;

                case AUDC4_C:
                    if (data == p.AUDC[CHAN4])
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDC4  $%02x (%s)\n", chip, data, audc2str(data)));
                    p.AUDC[CHAN4] = (byte)data;
                    ch_mask = 1 << CHAN4;
                    break;

                case AUDCTL_C:
                    if (data == p.AUDCTL)
                        return;
                    //LOG_SOUND((errorlog, "POKEY #%d AUDCTL $%02x (%s)\n", chip, data, audctl2str(data)));
                    p.AUDCTL = (byte)data;
                    ch_mask = 15;       /* all channels */
                    /* determine the base multiplier for the 'div by n' calculations */
                    p.clockmult = (p.AUDCTL & CLK_15KHZ) != 0 ? DIV_15 : DIV_64;
                    break;

                case STIMER_C:
                    /* first remove any existing timers */
                    //LOG_TIMER((errorlog, "POKEY #%d STIMER $%02x\n", chip, data));
                    if (p.timer[TIMER1] != null)
                        Mame.Timer.timer_remove(p.timer[TIMER1]);
                    if (p.timer[TIMER2] != null)
                        Mame.Timer.timer_remove(p.timer[TIMER2]);
                    if (p.timer[TIMER4] != null)
                        Mame.Timer.timer_remove(p.timer[TIMER4]);
                    p.timer[TIMER1] = null;
                    p.timer[TIMER2] = null;
                    p.timer[TIMER4] = null;

                    /* reset all counters to zero (side effect) */
                    p.polyadjust = 0;
                    p.counter[CHAN1] = 0;
                    p.counter[CHAN2] = 0;
                    p.counter[CHAN3] = 0;
                    p.counter[CHAN4] = 0;

                    /* joined chan#1 and chan#2 ? */
                    if ((p.AUDCTL & CH12_JOINED) != 0)
                    {
                        if (p.divisor[CHAN2] > 4)
                        {
                            //LOG_TIMER((errorlog, "POKEY #%d timer1+2 after %d clocks\n", chip, p.divisor[CHAN2]));
                            /* set timer #1 _and_ #2 event after timer_div clocks of joined CHAN1+CHAN2 */
                            p.timer[TIMER2] =
                                Mame.Timer.timer_pulse(1.0 * p.divisor[CHAN2] / intf.baseclock,
                                    (chip << 3) | IRQ_TIMR2 | IRQ_TIMR1, pokey_timer_expire);
                        }
                    }
                    else
                    {
                        if (p.divisor[CHAN1] > 4)
                        {
                            //LOG_TIMER((errorlog, "POKEY #%d timer1 after %d clocks\n", chip, p.divisor[CHAN1]));
                            /* set timer #1 event after timer_div clocks of CHAN1 */
                            p.timer[TIMER1] =
                                Mame.Timer.timer_pulse(1.0 * p.divisor[CHAN1] / intf.baseclock,
                                    (chip << 3) | IRQ_TIMR1, pokey_timer_expire);
                        }

                        if (p.divisor[CHAN2] > 4)
                        {
                            //LOG_TIMER((errorlog, "POKEY #%d timer2 after %d clocks\n", chip, p.divisor[CHAN2]));
                            /* set timer #2 event after timer_div clocks of CHAN2 */
                            p.timer[TIMER2] =
                                Mame.Timer.timer_pulse(1.0 * p.divisor[CHAN2] / intf.baseclock,
                                    (chip << 3) | IRQ_TIMR2, pokey_timer_expire);
                        }
                    }

                    /* Note: p[chip] does not have a timer #3 */

                    if ((p.AUDCTL & CH34_JOINED) != 0)
                    {
                        /* not sure about this: if audc4 == 0000xxxx don't start timer 4 ? */
                        if ((p.AUDC[CHAN4] & 0xf0) != 0)
                        {
                            if (p.divisor[CHAN4] > 4)
                            {
                                //LOG_TIMER((errorlog, "POKEY #%d timer4 after %d clocks\n", chip, p.divisor[CHAN4]));
                                /* set timer #4 event after timer_div clocks of CHAN4 */
                                p.timer[TIMER4] =
                                    Mame.Timer.timer_pulse(1.0 * p.divisor[CHAN4] / intf.baseclock,
                                        (chip << 3) | IRQ_TIMR4, pokey_timer_expire);
                            }
                        }
                    }
                    else
                    {
                        if (p.divisor[CHAN4] > 4)
                        {
                            //LOG_TIMER((errorlog, "POKEY #%d timer4 after %d clocks\n", chip, p.divisor[CHAN4]));
                            /* set timer #4 event after timer_div clocks of CHAN4 */
                            p.timer[TIMER4] =
                                Mame.Timer.timer_pulse(1.0 * p.divisor[CHAN4] / intf.baseclock,
                                    (chip << 3) | IRQ_TIMR4, pokey_timer_expire);
                        }
                    }
                    if (p.timer[TIMER1] != null)
                        Mame.Timer.timer_enable(p.timer[TIMER1], p.IRQEN & IRQ_TIMR1);
                    if (p.timer[TIMER2] != null)
                        Mame.Timer.timer_enable(p.timer[TIMER2], p.IRQEN & IRQ_TIMR2);
                    if (p.timer[TIMER4] != null)
                        Mame.Timer.timer_enable(p.timer[TIMER4], p.IRQEN & IRQ_TIMR4);
                    break;

                case SKREST_C:
                    /* reset SKSTAT */
                    //LOG((errorlog, "POKEY #%d SKREST $%02x\n", chip, data));
                    p.SKSTAT &= unchecked((byte)~(SK_FRAME | SK_OVERRUN | SK_KBERR));
                    break;

                case POTGO_C:
                    //LOG((errorlog, "POKEY #%d POTGO  $%02x\n", chip, data));
                    pokey_potgo(chip);
                    break;

                case SEROUT_C:
                    //LOG((errorlog, "POKEY #%d SEROUT $%02x\n", chip, data));
                    if (p.serout_w != null)
                        p.serout_w(offs, data);
                    p.SKSTAT |= SK_SEROUT;
                    /*
                     * These are arbitrary values, tested with some custom boot
                     * loaders from Ballblazer and Escape from Fractalus
                     * The real times are unknown
                     */
                    Mame.Timer.timer_set(Mame.Timer.TIME_IN_USEC(200), chip, pokey_serout_ready);
                    /* 10 bits (assumption 1 start, 8 data and 1 stop bit) take how long? */
                    Mame.Timer.timer_set(Mame.Timer.TIME_IN_USEC(2000), chip, pokey_serout_complete);
                    break;

                case IRQEN_C:
                    //LOG((errorlog, "POKEY #%d IRQEN  $%02x\n", chip, data));

                    /* acknowledge one or more IRQST bits ? */
                    if ((p.IRQST & ~data) != 0)
                    {
                        /* reset IRQST bits that are masked now */
                        p.IRQST &= (byte)data;
                    }
                    else
                    {
                        /* enable/disable timers now to avoid unneeded
                           breaking of the CPU cores for masked timers */
                        if (p.timer[TIMER1] != null && ((p.IRQEN ^ data) & IRQ_TIMR1) != 0)
                            Mame.Timer.timer_enable(p.timer[TIMER1], data & IRQ_TIMR1);
                        if (p.timer[TIMER2] != null && ((p.IRQEN ^ data) & IRQ_TIMR2) != 0)
                            Mame.Timer.timer_enable(p.timer[TIMER2], data & IRQ_TIMR2);
                        if (p.timer[TIMER4] != null && ((p.IRQEN ^ data) & IRQ_TIMR4) != 0)
                            Mame.Timer.timer_enable(p.timer[TIMER4], data & IRQ_TIMR4);
                    }
                    /* store irq enable */
                    p.IRQEN = (byte)data;
                    break;

                case SKCTL_C:
                    if (data == p.SKCTL)
                        return;
                    //LOG((errorlog, "POKEY #%d SKCTL  $%02x\n", chip, data));
                    p.SKCTL = (byte)data;
                    if ((data & SK_RESET) == 0)
                    {
                        pokey_register_w(chip, IRQEN_C, 0);
                        pokey_register_w(chip, SKREST_C, 0);
                    }
                    break;
            }

            /************************************************************
             * As defined in the manual, the exact counter values are
             * different depending on the frequency and resolution:
             *	  64 kHz or 15 kHz - AUDF + 1
             *	  1.79 MHz, 8-bit  - AUDF + 4
             *	  1.79 MHz, 16-bit - AUDF[CHAN1]+256*AUDF[CHAN2] + 7
             ************************************************************/

            /* only reset the channels that have changed */

            if ((ch_mask & (1 << CHAN1)) != 0)
            {
                /* process channel 1 frequency */
                if ((p.AUDCTL & CH1_HICLK) != 0)
                    new_val = p.AUDF[CHAN1] + DIVADD_HICLK;
                else
                    new_val = (int)((p.AUDF[CHAN1] + DIVADD_LOCLK) * p.clockmult);

                //LOG_SOUND((errorlog, "POKEY #%d chan1 %d\n", chip, new_val));

                p.volume[CHAN1] = (uint)((p.AUDC[CHAN1] & VOLUME_MASK) * POKEY_DEFAULT_GAIN);
                p.divisor[CHAN1] = new_val;
                if (new_val < p.counter[CHAN1])
                    p.counter[CHAN1] = new_val;
                if (p.interrupt_cb != null && p.timer[TIMER1] != null)
                    Mame.Timer.timer_reset(p.timer[TIMER1], 1.0 * new_val / intf.baseclock);
                p.audible[CHAN1] = (!(
                    (p.AUDC[CHAN1] & VOLUME_ONLY) != 0 ||
                    (p.AUDC[CHAN1] & VOLUME_MASK) == 0 ||
                    ((p.AUDC[CHAN1] & PURE) != 0 && new_val < (p.samplerate_24_8 >> 8))));
                if (!p.audible[CHAN1])
                {
                    p.output[CHAN1] = 1;
                    p.counter[CHAN1] = 0x7fffffff;
                    /* 50% duty cycle should result in half volume */
                    p.volume[CHAN1] >>= 1;
                }
            }

            if ((ch_mask & (1 << CHAN2)) != 0)
            {
                /* process channel 2 frequency */
                if ((p.AUDCTL & CH12_JOINED) != 0)
                {
                    if ((p.AUDCTL & CH1_HICLK) != 0)
                        new_val = p.AUDF[CHAN2] * 256 + p.AUDF[CHAN1] + DIVADD_HICLK_JOINED;
                    else
                        new_val = (int)((p.AUDF[CHAN2] * 256 + p.AUDF[CHAN1] + DIVADD_LOCLK) * p.clockmult);
                    //LOG_SOUND((errorlog, "POKEY #%d chan1+2 %d\n", chip, new_val));
                }
                else
                {
                    new_val = (int)((p.AUDF[CHAN2] + DIVADD_LOCLK) * p.clockmult);
                    //LOG_SOUND((errorlog, "POKEY #%d chan2 %d\n", chip, new_val));
                }

                p.volume[CHAN2] = (uint)((p.AUDC[CHAN2] & VOLUME_MASK) * POKEY_DEFAULT_GAIN);
                p.divisor[CHAN2] = new_val;
                if (new_val < p.counter[CHAN2])
                    p.counter[CHAN2] = new_val;
                if (p.interrupt_cb != null && p.timer[TIMER2] != null)
                    Mame.Timer.timer_reset(p.timer[TIMER2], 1.0 * new_val / intf.baseclock);
                p.audible[CHAN2] = (!(
                    (p.AUDC[CHAN2] & VOLUME_ONLY) != 0 ||
                    (p.AUDC[CHAN2] & VOLUME_MASK) == 0 ||
                    ((p.AUDC[CHAN2] & PURE) != 0 && new_val < (p.samplerate_24_8 >> 8))));
                if (!p.audible[CHAN2])
                {
                    p.output[CHAN2] = 1;
                    p.counter[CHAN2] = 0x7fffffff;
                    /* 50% duty cycle should result in half volume */
                    p.volume[CHAN2] >>= 1;
                }
            }

            if ((ch_mask & (1 << CHAN3)) != 0)
            {
                /* process channel 3 frequency */
                if ((p.AUDCTL & CH3_HICLK) != 0)
                    new_val = p.AUDF[CHAN3] + DIVADD_HICLK;
                else
                    new_val = (int)((p.AUDF[CHAN3] + DIVADD_LOCLK) * p.clockmult);

                //LOG_SOUND((errorlog, "POKEY #%d chan3 %d\n", chip, new_val));

                p.volume[CHAN3] = (uint)((p.AUDC[CHAN3] & VOLUME_MASK) * POKEY_DEFAULT_GAIN);
                p.divisor[CHAN3] = new_val;
                if (new_val < p.counter[CHAN3])
                    p.counter[CHAN3] = new_val;
                /* channel 3 does not have a timer associated */
                p.audible[CHAN3] = !(
                    (p.AUDC[CHAN3] & VOLUME_ONLY) != 0 ||
                    (p.AUDC[CHAN3] & VOLUME_MASK) == 0 ||
                    ((p.AUDC[CHAN3] & PURE) != 0 && new_val < (p.samplerate_24_8 >> 8))) ||
                    (p.AUDCTL & CH1_FILTER) != 0;
                if (!p.audible[CHAN3])
                {
                    p.output[CHAN3] = 1;
                    p.counter[CHAN3] = 0x7fffffff;
                    /* 50% duty cycle should result in half volume */
                    p.volume[CHAN3] >>= 1;
                }
            }

            if ((ch_mask & (1 << CHAN4)) != 0)
            {
                /* process channel 4 frequency */
                if ((p.AUDCTL & CH34_JOINED) != 0)
                {
                    if ((p.AUDCTL & CH3_HICLK) != 0)
                        new_val = p.AUDF[CHAN4] * 256 + p.AUDF[CHAN3] + DIVADD_HICLK_JOINED;
                    else
                        new_val = (int)((p.AUDF[CHAN4] * 256 + p.AUDF[CHAN3] + DIVADD_LOCLK) * p.clockmult);
                    //LOG_SOUND((errorlog, "POKEY #%d chan3+4 %d\n", chip, new_val));
                }
                else
                {
                    new_val = (int)((p.AUDF[CHAN4] + DIVADD_LOCLK) * p.clockmult);
                    //LOG_SOUND((errorlog, "POKEY #%d chan4 %d\n", chip, new_val));
                }

                p.volume[CHAN4] = (uint)((p.AUDC[CHAN4] & VOLUME_MASK) * POKEY_DEFAULT_GAIN);
                p.divisor[CHAN4] = new_val;
                if (new_val < p.counter[CHAN4])
                    p.counter[CHAN4] = new_val;
                if (p.interrupt_cb != null && p.timer[TIMER4] != null)
                    Mame.Timer.timer_reset(p.timer[TIMER4], 1.0 * new_val / intf.baseclock);
                p.audible[CHAN4] = !(
                    (p.AUDC[CHAN4] & VOLUME_ONLY) != 0 ||
                    (p.AUDC[CHAN4] & VOLUME_MASK) == 0 ||
                    ((p.AUDC[CHAN4] & PURE) != 0 && new_val < (p.samplerate_24_8 >> 8))) ||
                    (p.AUDCTL & CH2_FILTER) != 0;
                if (!p.audible[CHAN4])
                {
                    p.output[CHAN4] = 1;
                    p.counter[CHAN4] = 0x7fffffff;
                    /* 50% duty cycle should result in half volume */
                    p.volume[CHAN4] >>= 1;
                }
            }
        }

        public static int pokey1_r(int offset)
        {
            return pokey_register_r(0, offset);
        }
        public static int pokey2_r(int offset)
        {
            return pokey_register_r(1, offset);
        }
        public static void pokey1_w(int offset, int data)
        {
            pokey_register_w(0, offset, data);
        }
        public static void pokey2_w(int offset, int data)
        {
            pokey_register_w(1, offset, data);
        }

        static void poly_init(byte[] poly, int size, int left, int right, int add)
        {
            int mask = (1 << size) - 1;
            int i, x = 0;
            int pi = 0;
            //LOG_POLY((errorlog,"poly %d\n", size));
            for (i = 0; i < mask; i++)
            {
                poly[pi++] = (byte)(x & 1);
                //		LOG_POLY((errorlog,"%05x: %d\n", x, x&1));
                /* calculate next bit */
                x = ((x << left) + (x >> right) + add) & mask;
            }
        }

        static void rand_init(byte[] rng, int size, int left, int right, int add)
        {
            int mask = (1 << size) - 1;
            int i, x = 0;
            int ri = 0;
            //	LOG_RAND((errorlog,"rand %d\n", size));
            for (i = 0; i < mask; i++)
            {
                rng[ri] = (byte)(x >> (size - 8));   /* use the upper 8 bits */
                //LOG_RAND((errorlog, "%05x: %02x\n", x, *rng));
                ri++;
                /* calculate next bit */
                x = ((x << left) + (x >> right) + add) & mask;
            }
        }
    }
}
