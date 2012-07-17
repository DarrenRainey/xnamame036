using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    class vlm5030 : Mame.snd_interface
    {
        const byte IP_SIZE = 20;
        const byte FR_SIZE = 8;
        static VLM5030interface intf;
        static int channel, schannel;

        static _BytePtr VLM5030_rom;
        static int VLM5030_address_mask, VLM5030_address;
        static int pin_BSY, pin_ST, pin_RST;
        static int latch_data = 0, sampling_mode;
        static int table_h;
        const byte PH_RESET = 0;
        const byte PH_IDLE = 1;
        const byte PH_SETUP = 2;
        const byte PH_WAIT = 3;
        const byte PH_RUN = 4;
        const byte PH_STOP = 5;
        static int phase;
        /* these contain data describing the current and previous voice frames */
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
        static int sample_count = 0;       /* sample number within interp (0-19) */
        static int pitch_count = 0;

        static int[] u = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int[] x = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        /* ROM Tables */


        /* This is the energy lookup table */
        /* !!!!!!!!!! preliminary !!!!!!!!!! */
        static ushort[] energytable = new ushort[0x20];

        /* This is the pitch lookup table */
        static byte[] pitchtable =
{
   0,                               /* 0     : random mode */
   22,                              /* 1     : start=22    */
   23, 24, 25, 26, 27, 28, 29, 30,  /*  2- 9 : 1step       */
   32, 34, 36, 38, 40, 42, 44, 46,  /* 10-17 : 2step       */
   50, 54, 58, 62, 66, 70, 74, 78,  /* 18-25 : 4step       */
   86, 94, 102,110,118,             /* 26-30 : 8step       */
   255                              /* 31    : only one time ?? */
};

        /* These are the reflection coefficient lookup tables */
        /* 2's comp. */

        /* !!!!!!!!!! preliminary !!!!!!!!!! */

        /* 7bit */
        const int K1_RANGE = 0x6000;
        /* 4bit */
        const int K2_RANGE = 0x4000;
        const int K3_RANGE = 0x6000;
        const int K4_RANGE = 0x4000;
        /* 3bit */
        const int K5_RANGE = 0x6000;
        const int K6_RANGE = 0x6000;
        const int K7_RANGE = 0x5000;
        const int K8_RANGE = 0x4000;
        const int K9_RANGE = 0x5000;
        const int K10_RANGE = 0x4000;

        static int[] k1table = new int[0x80];
        static int[] k2table = new int[0x10];
        static int[] k3table = new int[0x10];
        static int[] k4table = new int[0x10];
        static int[] k5table = new int[0x08];
        static int[] k6table = new int[0x08];
        static int[] k7table = new int[0x08];
        static int[] k8table = new int[0x08];
        static int[] k9table = new int[0x08];
        static int[] k10table = new int[0x08];

        /* chirp table */
        static byte[] chirptable =
{
  0xff*9/10,
  0xff*7/10,
  0xff*5/10,
  0xff*4/10, /* non digital filter ? */
  0xff*3/10,
  0xff*3/10,
  0xff*1/10,
  0xff*1/10,
  0xff*1/10,
  0xff*1/10,
  0xff*1/10,
  0xff*1/10
};

        /* interpolation coefficients */
        static int[] interp_coeff = {
//8, 8, 8, 4, 4, 2, 2, 1
8, 8, 8, 4, 4, 2, 2, 1
};
        public vlm5030()
        {
            this.name = "VLM5030";
            this.sound_num = Mame.SOUND_VLM5030;
        }
        public override int chips_clock(Mame.MachineSound msound)
        {
            throw new NotImplementedException();
        }
        public override int chips_num(Mame.MachineSound msound)
        {
            throw new NotImplementedException();
        }
        static int get_bits(int sbit, int bits)
        {
            int offset = VLM5030_address + (sbit >> 3);
            int data;

            data = VLM5030_rom[offset & VLM5030_address_mask] |
                   (((int)VLM5030_rom[(offset + 1) & VLM5030_address_mask]) << 8);
            data >>= sbit;
            data &= (0xff >> (8 - bits));

            return data;
        }

        static int parse_frame ()
{
	byte cmd;

	/* remember previous frame */
	old_energy = new_energy;
	old_pitch = new_pitch;
	//memcpy( old_k , new_k , sizeof(old_k) );
    Array.Copy(new_k, old_k, old_k.Length);
	/* command byte check */
	cmd = VLM5030_rom[VLM5030_address&VLM5030_address_mask];
	if(( cmd & 0x01 )!=0)
	{	/* extend frame */
		new_energy = new_pitch = 0;
		//memset( new_k , 0 , sizeof(new_k));
        Array.Clear(new_k, 0, new_k.Length);
		VLM5030_address++;
		if( (cmd & 0x02 )!=0)
		{	/* end of speech */
			Mame.printf("VLM5030 %04X end \n",VLM5030_address );
			return 0;
		}
		else
		{	/* silent frame */
			int nums = ( (cmd>>2)+1 )*2;
			Mame.printf("VLM5030 %04X silent %d frame\n",VLM5030_address,nums );
			return nums * FR_SIZE;
		}
	}
	/* normal frame */

	new_pitch  = pitchtable[get_bits( 1,5)];
	new_energy = (ushort)(energytable[get_bits( 6,5)] >> 6);

	/* 10 K's */
	new_k[9] = k10table[get_bits(11,3)];
	new_k[8] = k9table[get_bits(14,3)];
	new_k[7] = k8table[get_bits(17,3)];
	new_k[6] = k7table[get_bits(20,3)];
	new_k[5] = k6table[get_bits(23,3)];
	new_k[4] = k5table[get_bits(26,3)];
	new_k[3] = k4table[get_bits(29,4)];
	new_k[2] = k3table[get_bits(33,4)];
	new_k[1] = k2table[get_bits(37,4)];
	new_k[0] = k1table[get_bits(41,7)];

	VLM5030_address+=6;
    Mame.printf("VLM5030 %04X voice \n", VLM5030_address);
	return FR_SIZE;
}
        static void vlm5030_update_callback(int num, _ShortPtr buffer, int length)
        {
            int buf_count = 0;
            int interp_effect;

            /* running */
            if (phase == PH_RUN)
            {
                /* playing speech */
                while (length > 0)
                {
                    int current_val;

                    /* check new interpolator or  new frame */
                    if (sample_count == 0)
                    {
                        sample_count = IP_SIZE;
                        /* interpolator changes */
                        if (interp_count == 0)
                        {
                            /* change to new frame */
                            interp_count = parse_frame(); /* with change phase */
                            if (interp_count == 0)
                            {
                                sample_count = 160; /* end -> stop time */
                                phase = PH_STOP;
                                goto phase_stop; /* continue to stop phase */
                            }
                            /* Set old target as new start of frame */
                            current_energy = old_energy;
                            current_pitch = old_pitch;
                            //memcpy(current_k, old_k, sizeof(current_k));
                            Array.Copy(old_k, current_k, current_k.Length);
                            /* is this a zero energy frame? */
                            if (current_energy == 0)
                            {
                                /*printf("processing frame: zero energy\n");*/
                                target_energy = 0;
                                target_pitch = current_pitch;
                                //memcpy(target_k, current_k, sizeof(target_k));
                                Array.Copy(current_k, target_k, target_k.Length);
                            }
                            else
                            {
                                /*printf("processing frame: Normal\n");*/
                                /*printf("*** Energy = %d\n",current_energy);*/
                                /*printf("proc: %d %d\n",last_fbuf_head,fbuf_head);*/
                                target_energy = new_energy;
                                target_pitch = new_pitch;
                                //memcpy(target_k, new_k, sizeof(target_k));
                                Array.Copy(new_k, target_k, target_k.Length);
                            }
                        }
                        /* next interpolator */
                        /* Update values based on step values */
                        /*printf("\n");*/
                        interp_effect = (int)(interp_coeff[(FR_SIZE - 1) - (interp_count % FR_SIZE)]);

                        current_energy +=(ushort)( (target_energy - current_energy) / interp_effect);
                        if (old_pitch != 0)
                            current_pitch += (ushort)((target_pitch - current_pitch) / interp_effect);
                        /*printf("*** Energy = %d\n",current_energy);*/
                        current_k[0] += (target_k[0] - current_k[0]) / interp_effect;
                        current_k[1] += (target_k[1] - current_k[1]) / interp_effect;
                        current_k[2] += (target_k[2] - current_k[2]) / interp_effect;
                        current_k[3] += (target_k[3] - current_k[3]) / interp_effect;
                        current_k[4] += (target_k[4] - current_k[4]) / interp_effect;
                        current_k[5] += (target_k[5] - current_k[5]) / interp_effect;
                        current_k[6] += (target_k[6] - current_k[6]) / interp_effect;
                        current_k[7] += (target_k[7] - current_k[7]) / interp_effect;
                        current_k[8] += (target_k[8] - current_k[8]) / interp_effect;
                        current_k[9] += (target_k[9] - current_k[9]) / interp_effect;
                        interp_count--;
                    }
                    /* calcrate digital filter */
                    if (old_energy == 0)
                    {
                        /* generate silent samples here */
                        current_val = 0x00;
                    }
                    else if (old_pitch == 0)
                    {
                        /* generate unvoiced samples here */
                        int randvol = (Mame.rand() % 10);
                        current_val = (randvol * current_energy) / 10;
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
                    u[9] = u[10] - ((current_k[9] * x[9]) / 32768);
                    u[8] = u[9] - ((current_k[8] * x[8]) / 32768);
                    u[7] = u[8] - ((current_k[7] * x[7]) / 32768);
                    u[6] = u[7] - ((current_k[6] * x[6]) / 32768);
                    u[5] = u[6] - ((current_k[5] * x[5]) / 32768);
                    u[4] = u[5] - ((current_k[4] * x[4]) / 32768);
                    u[3] = u[4] - ((current_k[3] * x[3]) / 32768);
                    u[2] = u[3] - ((current_k[2] * x[2]) / 32768);
                    u[1] = u[2] - ((current_k[1] * x[1]) / 32768);
                    u[0] = u[1] - ((current_k[0] * x[0]) / 32768);

                    x[9] = x[8] + ((current_k[8] * u[8]) / 32768);
                    x[8] = x[7] + ((current_k[7] * u[7]) / 32768);
                    x[7] = x[6] + ((current_k[6] * u[6]) / 32768);
                    x[6] = x[5] + ((current_k[5] * u[5]) / 32768);
                    x[5] = x[4] + ((current_k[4] * u[4]) / 32768);
                    x[4] = x[3] + ((current_k[3] * u[3]) / 32768);
                    x[3] = x[2] + ((current_k[2] * u[2]) / 32768);
                    x[2] = x[1] + ((current_k[1] * u[1]) / 32768);
                    x[1] = x[0] + ((current_k[0] * u[0]) / 32768);
                    x[0] = u[0];
                    /* clipping, buffering */
                    if (u[0] > 511)
                        buffer.write16(buf_count,  127 << 8);
                    else if (u[0] < -512)
                        buffer.write16(buf_count, unchecked( (ushort)(-128 << 8)));
                    else
                        buffer.write16(buf_count,  (ushort)(u[0] << 6));
                    buf_count++;

                    /* sample count */
                    sample_count--;
                    /* pitch */
                    pitch_count++;
                    if (pitch_count >= current_pitch)
                        pitch_count = 0;
                    /* size */
                    length--;
                }
                /*		return;*/
            }
        /* stop phase */
        phase_stop:
            switch (phase)
            {
                case PH_SETUP:
                    sample_count -= length;
                    if (sample_count <= 0)
                    {
                        Mame.printf("VLM5030 BSY=H\n");
                        /* pin_BSY = 1; */
                        phase = PH_WAIT;
                    }
                    break;
                case PH_STOP:
                    sample_count -= length;
                    if (sample_count <= 0)
                    {
                        Mame.printf("VLM5030 BSY=L\n");
                        pin_BSY = 0;
                        phase = PH_IDLE;
                    }
                    break;
            }
            /* silent buffering */
            while (length > 0)
            {
                buffer.write16(buf_count++, 0x00);
                length--;
            }
        }

        public override int start(Mame.MachineSound msound)
        {
           int emulation_rate;

	intf = (VLM5030interface)msound.sound_interface;

	Mame.Machine.samples = Mame.readsamples(intf.samplenames,Mame.Machine.gamedrv.name);

	emulation_rate = intf.baseclock / 440;
	pin_BSY = pin_RST = pin_ST  = 0;
	phase = PH_IDLE;
/*	VLM5030_VCU(intf.vcu); */

	VLM5030_rom = Mame.memory_region(intf.memory_region);
	/* memory size */
	if( intf.memory_size == 0)
		VLM5030_address_mask = Mame.memory_region_length(intf.memory_region)-1;
	else
		VLM5030_address_mask = intf.memory_size-1;

	channel = Mame.stream_init("VLM5030",intf.volume,emulation_rate /* Machine.sample_rate */,
				0,vlm5030_update_callback);
	if (channel == -1) return 1;

	schannel = Mame.mixer_allocate_channel(intf.volume);

#if true
	{
	int i;

	/* initialize energy table */
	for(i=0;i<0x20;i++)
	{
		energytable[i]=(ushort)(0x7fff*i/0x1f);
	}

	/* initialize filter table */
	for(i=-0x40 ; i<0x40 ; i++)
	{
		k1table[(i>=0) ? i : i+0x80] = i*K1_RANGE/0x40;
	}
	for(i=-0x08 ; i<0x08 ; i++)
	{
		k2table[(i>=0) ? i : i+0x10] = i*K2_RANGE/0x08;
		k3table[(i>=0) ? i : i+0x10] = i*K3_RANGE/0x08;
		k4table[(i>=0) ? i : i+0x10] = i*K4_RANGE/0x08;
	}
	for(i=-0x04 ; i<0x04 ; i++)
	{
		k5table[(i>=0) ? i : i+0x08] = i*K5_RANGE/0x04;
		k6table[(i>=0) ? i : i+0x08] = i*K6_RANGE/0x04;
		k7table[(i>=0) ? i : i+0x08] = i*K7_RANGE/0x04;
		k8table[(i>=0) ? i : i+0x08] = i*K8_RANGE/0x04;
		k9table[(i>=0) ? i : i+0x08] = i*K9_RANGE/0x04;
		k10table[(i>=0) ? i : i+0x08] = i*K10_RANGE/0x04;
	}

	}
#endif
	return 0;
        }
        public override void stop()
        {
           //nothing
        }
        public override void reset()
        {
            //nothing
        }
        public override void update()
        {
            VLM5030_update();
        }
        static void VLM5030_update()
{
	if( sampling_mode ==0)
	{
		/* docode mode */
		Mame.stream_update(channel,0);
	}
	else
	{
		/* sampling mode (check  busy flag) */
		if( pin_ST == 0 && pin_BSY == 1 )
		{
			if( !Mame.mixer_is_sample_playing(schannel) )
				pin_BSY = 0;
		}
	}
}
    }
    class VLM5030interface
    {
        public int baseclock, volume, memory_region, memory_size, vcu;
        public string[] samplenames;
        public VLM5030interface(int baseclock, int volume, int memory_region, int memory_size, int vcu, string[] samplenames = null)
        {
            this.baseclock = baseclock; this.volume = volume; this.memory_region = memory_region;
            this.memory_size = memory_size; this.vcu = vcu; this.samplenames = samplenames;
        }
    }
}
