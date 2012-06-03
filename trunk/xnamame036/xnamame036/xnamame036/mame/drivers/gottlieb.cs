using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame.drivers
{
   
    class driver_gottlieb : Mame.GameDriver
    {
        public override void driver_init()
        {
        }
        static int[] track = new int[2];
        static int joympx;
        static int current_frame = 0x00001;
        static int laserdisc_playing;
        static int lasermpx;
        public static _BytePtr nvram = new _BytePtr(1);
        public static int[] nvram_size = new int[1];
        public static _BytePtr gottlieb_characterram = new _BytePtr(1);
        const int MAX_CHARS = 256;
        public static _BytePtr dirtycharacter = new _BytePtr(1);
        static int background_priority = 0;
        public static _BytePtr riot_ram = new _BytePtr(1);
        static byte[] riot_regs = new byte[32];
        static bool hflip = false;
        static bool vflip = false;
        static int spritebank;
        static string[] PhonemeTable =
{
 "EH3","EH2","EH1","PA0","DT" ,"A1" ,"A2" ,"ZH",
 "AH2","I3" ,"I2" ,"I1" ,"M"  ,"N"  ,"B"  ,"V",
 "CH" ,"SH" ,"Z"  ,"AW1","NG" ,"AH1","OO1","OO",
 "L"  ,"K"  ,"J"  ,"H"  ,"G"  ,"F"  ,"D"  ,"S",
 "A"  ,"AY" ,"Y1" ,"UH3","AH" ,"P"  ,"O"  ,"I",
 "U"  ,"Y"  ,"T"  ,"R"  ,"E"  ,"W"  ,"AE" ,"AE1",
 "AW2","UH2","UH1","UH" ,"O2" ,"O1" ,"IU" ,"U1",
 "THV","TH" ,"ER" ,"EH" ,"E1" ,"AW" ,"PA1","STOP",
 null
};
        public class machine_driver_gottlieb : Mame.MachineDriver
        {
            public machine_driver_gottlieb()
            {
                cpu.Add(new Mame.MachineCPU(Mame.CPU_I86, 5000000, gottlieb_readmem, gottlieb_writemem, null, null, gottlieb_interrupt, 1));
                cpu.Add(new Mame.MachineCPU(Mame.CPU_M6502 | Mame.CPU_AUDIO_CPU, 3579545 / 4, gottlieb_sound_readmem, gottlieb_sound_writemem, null, null, Mame.ignore_interrupt, 1));
                frames_per_second = 61;
                vblank_duration = 1018;
                cpu_slices_per_frame = 1;
                screen_width = 32 * 8;
                screen_height = 32 * 8;
                visible_area = new Mame.rectangle(0 * 8, 32 * 8 - 1, 0 * 8, 30 * 8 - 1);
                gfxdecodeinfo = charROM_gfxdecodeinfo;
                total_colors = 16;
                color_table_len = 16;
                video_attributes = Mame.VIDEO_TYPE_RASTER | Mame.VIDEO_SUPPORTS_DIRTY | Mame.VIDEO_MODIFIES_PALETTE;
                //, 0, gottlieb_vh_start, gottlieb_vh_stop, gottlieb_vh_screenrefresh,
                //0,0,0,0, { { SOUND_DAC, &dac1_interface }, { SOUND_SAMPLES, &samples_interface } }, nvram_handler };        }
                sound_attributes = 0;
                sound.Add(new Mame.MachineSound(Mame.SOUND_DAC, dac1_interface));
                sound.Add(new Mame.MachineSound(Mame.SOUND_SAMPLES, samples_interface));
            }
            public override void init_machine()
            {
               //nothing
            }
            public override void nvram_handler(object file, int read_or_write)
            {
                if (read_or_write != 0)
                    Mame.osd_fwrite(file, nvram, nvram_size[0]);
                else
                {
                    if (file != null)
                        Mame.osd_fread(file, nvram, nvram_size[0]);
                    else
                        for (int i = 0; i < driver_gottlieb.nvram_size[0]; i++)
                            driver_gottlieb.nvram[i] = 0xff;
                }
            }
            public override void vh_init_palette(_BytePtr palette, _ShortPtr colortable, _BytePtr color_prom)
            {
                //nothing
            }
            public override int vh_start()
            {
                if (Generic.generic_vh_start() != 0) return 1;
                dirtycharacter = new _BytePtr(MAX_CHARS);
                for (int i = 0; i < MAX_CHARS; i++) dirtycharacter[i] = 0;
                return 0;
            }
            public override void vh_stop()
            {
                dirtycharacter = null;
                Generic.generic_vh_stop();
            }
            public override void vh_update(Mame.osd_bitmap bitmap, int full_refresh)
            {
                /* update palette */
                if (Mame.palette_recalc()!=null)
                    Generic.SetDirtyBuffer(true);// memset(dirtybuffer, 1, videoram_size);

                /* recompute character graphics */
                for (int offs = 0; offs < Mame.Machine.drv.gfxdecodeinfo[0].gfxlayout.total; offs++)
                {
                    if (dirtycharacter[offs]!=0)
                        Mame.decodechar(Mame.Machine.gfx[0], offs, gottlieb_characterram, Mame.Machine.drv.gfxdecodeinfo[0].gfxlayout);
                }


                /* for every character in the Video RAM, check if it has been modified */
                /* since last time and update it accordingly. */
                for (int offs = Generic.videoram_size[0] - 1; offs >= 0; offs--)
                {
                    if (Generic.dirtybuffer[offs] || dirtycharacter[Generic.videoram[offs]]!=0)
                    {
                        Generic.dirtybuffer[offs] = false;

                        int sx = offs % 32;
                        int sy = offs / 32;
                        if (hflip) sx = 31 - sx;
                        if (vflip) sy = 29 - sy;

                        Mame.drawgfx(Generic.tmpbitmap, Mame.Machine.gfx[0],
                                Generic.videoram[offs],
                                0,
                                hflip, vflip,
                                8 * sx, 8 * sy,
                                Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);
                    }
                }

                for (int i=0;i<MAX_CHARS;i++)dirtycharacter[i] = 0;


                /* copy the character mapped graphics */
                Mame.copybitmap(bitmap, Generic.tmpbitmap, false,false, 0, 0, Mame.Machine.drv.visible_area, Mame.TRANSPARENCY_NONE, 0);


                /* Draw the sprites. Note that it is important to draw them exactly in this */
                /* order, to have the correct priorities. */
                for (int offs = 0; offs < Generic.spriteram_size[0] - 8; offs += 4)     /* it seems there's something strange with sprites #62 and #63 */
                {
                    /* coordinates hand tuned to make the position correct in Q*Bert Qubes start */
                    /* of level animation. */
                    int sx = (Generic.spriteram[offs + 1]) - 4;
                    if (hflip) sx = 233 - sx;
                    int sy = (Generic.spriteram[offs]) - 13;
                    if (vflip) sy = 228 - sy;

                    if (Generic.spriteram[offs] !=0|| Generic.spriteram[offs + 1]!=0)	/* needed to avoid garbage on screen */
                       Mame.drawgfx(bitmap, Mame.Machine.gfx[1],
                                (uint)((255 ^ Generic.spriteram[offs + 2]) + 256 * spritebank),
                                0,
                                hflip, vflip,
                                sx, sy,
                                Mame.Machine.drv.visible_area,
                                background_priority !=0? Mame.TRANSPARENCY_THROUGH : Mame.TRANSPARENCY_PEN,
                                background_priority !=0? Mame.Machine.pens.read16(0) : 0);
                }
            }
            public override void vh_eof_callback()
            {
               //nothing
            }
        }
        public static Mame.MemoryReadAddress[] gottlieb_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x0fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x1000, 0x1fff, Mame.MRA_RAM ),	/* or ROM */
	new Mame.MemoryReadAddress( 0x2000, 0x2fff, Mame.MRA_RAM ),	/* or ROM */
	new Mame.MemoryReadAddress( 0x3800, 0x3bff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x4000, 0x4fff, Mame.MRA_RAM ),
	new Mame.MemoryReadAddress( 0x5800, 0x5800, Mame.input_port_0_r ),	/* DSW */
	new Mame.MemoryReadAddress( 0x5801, 0x5801, Mame.input_port_1_r),	/* buttons */
	new Mame.MemoryReadAddress( 0x5802, 0x5802, gottlieb_track_0_r),	/* trackball H */
	new Mame.MemoryReadAddress( 0x5803, 0x5803, gottlieb_track_1_r ),	/* trackball V */
	new Mame.MemoryReadAddress( 0x5804, 0x5804, Mame.input_port_4_r ),	/* joystick */
	new Mame.MemoryReadAddress( 0x5805, 0x5807, gottlieb_laserdisc_status_r ),
	new Mame.MemoryReadAddress( 0x6000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] gottlieb_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x0fff, Mame.MWA_RAM,nvram, nvram_size ),
	new Mame.MemoryWriteAddress( 0x1000, 0x1fff, Mame.MWA_RAM ),	/* ROM in Krull */
	new Mame.MemoryWriteAddress( 0x2000, 0x2fff, Mame.MWA_RAM ),	/* ROM in Krull and 3 Stooges */
	new Mame.MemoryWriteAddress( 0x3000, 0x30ff, Mame.MWA_RAM, Generic.spriteram, Generic.spriteram_size ),
	new Mame.MemoryWriteAddress( 0x3800, 0x3bff, Generic.videoram_w, Generic.videoram, Generic.videoram_size ),
	new Mame.MemoryWriteAddress( 0x3c00, 0x3fff, Generic.videoram_w ),	/* mirror address, some games write to it */
	new Mame.MemoryWriteAddress( 0x4000, 0x4fff, gottlieb_characterram_w, gottlieb_characterram ),
	new Mame.MemoryWriteAddress( 0x5000, 0x501f, gottlieb_paletteram_w, Mame.paletteram ),
	new Mame.MemoryWriteAddress( 0x5800, 0x5800, Mame.watchdog_reset_w ),
	new Mame.MemoryWriteAddress( 0x5801, 0x5801, gottlieb_track_reset_w ),
	new Mame.MemoryWriteAddress( 0x5802, 0x5802, gottlieb_sh_w ), /* sound/speech command */
	new Mame.MemoryWriteAddress( 0x5803, 0x5803, gottlieb_video_outputs ),       /* OUT1 */
	new Mame.MemoryWriteAddress( 0x6000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 ) /* end of table */
};
        public static Mame.MemoryReadAddress[] gottlieb_sound_readmem =
{
	new Mame.MemoryReadAddress( 0x0000, 0x01ff, riot_ram_r ),
	new Mame.MemoryReadAddress( 0x0200, 0x03ff, gottlieb_riot_r ),
	new Mame.MemoryReadAddress( 0x6000, 0x7fff, Mame.MRA_ROM ),
		 /* A15 not decoded except in expansion socket */
	new Mame.MemoryReadAddress( 0x8000, 0x81ff, riot_ram_r ),
	new Mame.MemoryReadAddress( 0x8200, 0x83ff, gottlieb_riot_r ),
	new Mame.MemoryReadAddress( 0xe000, 0xffff, Mame.MRA_ROM ),
	new Mame.MemoryReadAddress( -1 )  /* end of table */
};

        public static Mame.MemoryWriteAddress[] gottlieb_sound_writemem =
{
	new Mame.MemoryWriteAddress( 0x0000, 0x01ff, riot_ram_w, riot_ram ),
	new Mame.MemoryWriteAddress( 0x0200, 0x03ff, gottlieb_riot_w ),
	new Mame.MemoryWriteAddress( 0x1000, 0x1000, DAC.DAC_data_w ),
	new Mame.MemoryWriteAddress( 0x2000, 0x2000, gottlieb_speech_w ),
	new Mame.MemoryWriteAddress( 0x3000, 0x3000, gottlieb_speech_clock_DAC_w ),
	new Mame.MemoryWriteAddress( 0x6000, 0x7fff, Mame.MWA_ROM ),
			 /* A15 not decoded except in expansion socket */
	new Mame.MemoryWriteAddress( 0x8000, 0x81ff, riot_ram_w ),
	new Mame.MemoryWriteAddress( 0x8200, 0x83ff, gottlieb_riot_w),
	new Mame.MemoryWriteAddress( 0x9000, 0x9000, DAC.DAC_data_w ),
	new Mame.MemoryWriteAddress( 0xa000, 0xa000, gottlieb_speech_w ),
	new Mame.MemoryWriteAddress( 0xb000, 0xb000, gottlieb_speech_clock_DAC_w ),
	new Mame.MemoryWriteAddress( 0xe000, 0xffff, Mame.MWA_ROM ),
	new Mame.MemoryWriteAddress( -1 )  /* end of table */
};
        static void gottlieb_paletteram_w(int offset, int data)
        {
            int bit0, bit1, bit2, bit3;
            int r, g, b, val;


            Mame.paletteram[offset] = (byte)data;

            /* red component */
            val = Mame.paletteram[offset | 1];
            bit0 = (val >> 0) & 0x01;
            bit1 = (val >> 1) & 0x01;
            bit2 = (val >> 2) & 0x01;
            bit3 = (val >> 3) & 0x01;
            r = 0x10 * bit0 + 0x21 * bit1 + 0x46 * bit2 + 0x88 * bit3;

            /* green component */
            val = Mame.paletteram[offset & ~1];
            bit0 = (val >> 4) & 0x01;
            bit1 = (val >> 5) & 0x01;
            bit2 = (val >> 6) & 0x01;
            bit3 = (val >> 7) & 0x01;
            g = 0x10 * bit0 + 0x21 * bit1 + 0x46 * bit2 + 0x88 * bit3;

            /* blue component */
            val = Mame.paletteram[offset & ~1];
            bit0 = (val >> 0) & 0x01;
            bit1 = (val >> 1) & 0x01;
            bit2 = (val >> 2) & 0x01;
            bit3 = (val >> 3) & 0x01;
            b = 0x10 * bit0 + 0x21 * bit1 + 0x46 * bit2 + 0x88 * bit3;

            Mame.palette_change_color(offset / 2, (byte)r, (byte)g, (byte)b);
        }
        static void gottlieb_characterram_w(int offset, int data)
        {
            if (gottlieb_characterram[offset] != data)
            {
                dirtycharacter[offset / 32] = 1;
                gottlieb_characterram[offset] = (byte)data;
            }
        }
        static int last = 0;
        static void gottlieb_video_outputs(int offset, int data)
        {
            background_priority = data & 1;

            hflip = (data & 2) != 0;
            vflip = (data & 4) != 0;
            if ((data & 6) != (last & 6))
                Generic.SetDirtyBuffer(true);//memset(dirtybuffer,1,videoram_size);

            /* in Q*Bert Qubes only, bit 4 controls the sprite bank */
            spritebank = (data & 0x10) >> 4;

            if ((last & 0x20) != 0 && (data & 0x20) == 0) gottlieb_knocker();

            last = data;
        }
        static int gottlieb_track_0_r(int offset)
        {
            return Mame.input_port_2_r(offset) - track[0];
        }
        static int gottlieb_track_1_r(int offset)
        {
            return Mame.input_port_3_r(offset) - track[1];
        }
        static void gottlieb_track_reset_w(int offset, int data)
        {
            /* reset the trackball counters */
            track[0] = Mame.input_port_2_r(offset);
            track[1] = Mame.input_port_3_r(offset);
        }
        static int gottlieb_laserdisc_status_r(int offset)
        {
            switch (offset)
            {
                case 0:
                    return (current_frame >> 0) & 0xff;
                    break;
                case 1:
                    return (current_frame >> 8) & 0xff;
                    break;
                case 2:
                    if (lasermpx == 1)
                        /* bits 0-2 frame number MSN */
                        /* bit 3 audio buffer ready */
                        /* bit 4 ready to send new laserdisc command? */
                        /* bit 5 disc ready */
                        /* bit 6 break in audio trasmission */
                        /* bit 7 missing audio clock */
                        return ((current_frame >> 16) & 0x07) | 0x10 | (Mame.rand() & 0x28);
                    else	/* read audio buffer */
                        return Mame.rand();
                    break;
            }

            return 0;
        }
        static void gottlieb_laserdisc_mpx_w(int offset, int data)
        {
            lasermpx = data & 1;
        }

        static int loop;
        static int lastcmd;
        static void gottlieb_laserdisc_command_w(int offset, int data)
        {
            int cmd;

            /* commands are written in three steps, the first two the command is */
            /* written (maybe one to load the latch, the other to start the send), */
            /* the third 0 (maybe to clear the latch) */
            if (data == 0) return;
            if ((loop++ & 1) != 0) return;

            if ((data & 0xe0) != 0x20)
            {
                Mame.printf("error: laserdisc command %02x\n", data);
                return;
            }

            cmd = ((data & 0x10) >> 4) |
                    ((data & 0x08) >> 2) |
                    ((data & 0x04) >> 0) |
                    ((data & 0x02) << 2) |
                    ((data & 0x01) << 4);

            //if (errorlog) fprintf(errorlog,"laserdisc command %02x . %02x\n",data,cmd);
            if (lastcmd == 0x0b && (cmd & 0x10) != 0)	/* seek frame # */
            {
                current_frame = (current_frame << 4) | (cmd & 0x0f);
            }
            else
            {
                if (cmd == 0x04)	/* step forward */
                {
                    laserdisc_playing = 0;
                    current_frame++;
                }
                if (cmd == 0x05) laserdisc_playing = 1;	/* play */
                if (cmd == 0x0f) laserdisc_playing = 0;	/* stop */
                if (cmd == 0x0b) laserdisc_playing = 0;	/* seek frame */
                lastcmd = cmd;
            }
        }

        public static int gottlieb_interrupt()
        {
            if (laserdisc_playing != 0) current_frame++;

            return Mame.nmi_interrupt();
        }



        static int score_sample = 7;
        static int random_offset = 0;
        static void gottlieb_sh_w(int offset, int data)
        {
            data &= 0x3f;

            if ((data & 0x0f) != 0xf) /* interrupt trigered by four low bits (not all 1's) */
            {
                if (Mame.Machine.samples != null)
                {
                    if (Mame.Machine.gamedrv.name.CompareTo("reactor") == 0)	/* reactor */
                    {
                        switch (data ^ 0x3f)
                        {
                            case 53:
                            case 54:
                            case 55:
                            case 56:
                            case 57:
                            case 58:
                            case 59:
                                Mame.sample_start(0, (data ^ 0x3f) - 53, 0);
                                break;
                            case 31:
                                Mame.sample_start(0, 7, 0);
                                score_sample = 7;
                                break;
                            case 39:
                                score_sample++;
                                if (score_sample < 20) Mame.sample_start(0, score_sample, 0);
                                break;
                        }
                    }
                    else	/* qbert */
                    {
                        switch (data ^ 0x3f)
                        {
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                                Mame.sample_start(0, ((data ^ 0x3f) - 17) * 8 + random_offset, 0);
                                random_offset = (random_offset + 1) & 7;
                                break;
                            case 22:
                                Mame.sample_start(0, 40, 0);
                                break;
                            case 23:
                                Mame.sample_start(0, 41, 0);
                                break;
                            case 28:
                                Mame.sample_start(0, 42, 0);
                                break;
                            case 36:
                                Mame.sample_start(0, 43, 0);
                                break;
                        }
                    }
                }

                Mame.soundlatch_w(offset, data);

                switch (Mame.cpu_gettotalcpu())
                {
                    case 2:
                        /* Revision 1 sound board */
                        Mame.cpu_cause_interrupt(1, Mame.cpu_m6502.M6502_INT_IRQ);
                        break;
                    case 3:
                    case 4:
                        /* Revision 2 & 3 sound board */
                        Mame.cpu_cause_interrupt(Mame.cpu_gettotalcpu() - 1, Mame.cpu_m6502.M6502_INT_IRQ);
                        Mame.cpu_cause_interrupt(Mame.cpu_gettotalcpu() - 2, Mame.cpu_m6502.M6502_INT_IRQ);
                        break;
                }
            }
        }


        static void gottlieb_knocker()
        {
            if (Mame.Machine.samples != null)
            {
                if (Mame.Machine.gamedrv.name.CompareTo("reactor") == 0)	/* reactor */
                {
                }
                else	/* qbert */
                    Mame.sample_start(0, 44, 0);
            }
        }
        static int riot_ram_r(int offset)
        {
            return riot_ram[offset & 0x7f];
        }
        static int gottlieb_riot_r(int offset)
        {
            switch (offset & 0x1f)
            {
                case 0: /* port A */
                    return Mame.soundlatch_r(offset) ^ 0xff;	/* invert command */
                case 2: /* port B */
                    return 0x40;    /* say that PB6 is 1 (test SW1 not pressed) */
                case 5: /* interrupt register */
                    return 0x40;    /* say that edge detected on PA7 */
                default:
                    return riot_regs[offset & 0x1f];
            }
        }
        static void riot_ram_w(int offset, int data)
        {
            riot_ram[offset & 0x7f] = (byte)data;
        }
        static void gottlieb_riot_w(int offset, int data)
        {
            riot_regs[offset & 0x1f] = (byte)data;
        }
        static int[] queue = new int[100];
        static int pos;
        static void gottlieb_speech_w(int offset, int data)
        {

            data ^= 255;

            Mame.printf("Votrax: intonation %d, phoneme %02x %s\n", data >> 6, data & 0x3f, PhonemeTable[data & 0x3f]);

            queue[pos++] = data & 0x3f;

            if ((data & 0x3f) == 0x3f)
            {
#if false
		if (pos > 1)
		{
			int i;
			char buf[200];

			buf[0] = 0;
			for (i = 0;i < pos-1;i++)
			{
				if (queue[i] == 0x03 || queue[i] == 0x3e) strcat(buf," ");
				else strcat(buf,PhonemeTable[queue[i]]);
			}

			usrintf_showmessage(buf);
		}
#endif

                pos = 0;
            }

            /* generate a NMI after a while to make the CPU continue to send data */
            Mame.Timer.timer_set(Mame.Timer.TIME_IN_USEC(50), 0, gottlieb_nmi_generate);
        }
        static void gottlieb_nmi_generate(int param)
        {
            Mame.cpu_cause_interrupt(1, Mame.cpu_m6502.M6502_INT_NMI);
        }
        static void gottlieb_speech_clock_DAC_w(int offset, int data){ }





        /* the games can store char gfx data in either a 4k RAM area (128 chars), or */
        /* a 8k ROM area (256 chars). */
        static Mame.GfxLayout charRAMlayout =
        new Mame.GfxLayout(
            8, 8,    /* 8*8 characters */
            128,    /* 128 characters */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
        new uint[] { 0, 4, 8, 12, 16, 20, 24, 28 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8    /* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout charROMlayout =
        new Mame.GfxLayout(
            8, 8,    /* 8*8 characters */
            256,    /* 256 characters */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 1, 2, 3 },
            new uint[] { 0, 4, 8, 12, 16, 20, 24, 28 },
            new uint[] { 0 * 32, 1 * 32, 2 * 32, 3 * 32, 4 * 32, 5 * 32, 6 * 32, 7 * 32 },
            32 * 8    /* every char takes 32 consecutive bytes */
        );

        static Mame.GfxLayout spritelayout =
        new Mame.GfxLayout(
            16, 16,  /* 16*16 sprites */
            256,    /* 256 sprites */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 256 * 32 * 8, 2 * 256 * 32 * 8, 3 * 256 * 32 * 8 },
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
            32 * 8    /* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxLayout qbertqub_spritelayout =
        new Mame.GfxLayout(
            16, 16,  /* 16*16 sprites */
            512,    /* 512 sprites */
            4,      /* 4 bits per pixel */
            new uint[] { 0, 512 * 32 * 8, 2 * 512 * 32 * 8, 3 * 512 * 32 * 8 },
            new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            new uint[]{ 0*16, 1*16, 2*16, 3*16, 4*16, 5*16, 6*16, 7*16,
			8*16, 9*16, 10*16, 11*16, 12*16, 13*16, 14*16, 15*16 },
            32 * 8    /* every sprite takes 32 consecutive bytes */
        );

        static Mame.GfxDecodeInfo[] charRAM_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo( 0,           0x4000, charRAMlayout, 0, 1 ),	/* the game dynamically modifies this */
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000, spritelayout,  0, 1 ),
};

        public static Mame.GfxDecodeInfo[] charROM_gfxdecodeinfo =
{
	 new Mame.GfxDecodeInfo( Mame.REGION_GFX1, 0x0000, charROMlayout, 0, 1 ),
	new Mame.GfxDecodeInfo( Mame.REGION_GFX2, 0x0000, spritelayout,  0, 1 ),
};

        static Mame.GfxDecodeInfo[] qbertqub_gfxdecodeinfo =
{
	new Mame.GfxDecodeInfo(  Mame.REGION_GFX1, 0x0000, charROMlayout,         0, 1 ),
	new Mame.GfxDecodeInfo(  Mame.REGION_GFX2, 0x0000, qbertqub_spritelayout, 0, 1 ),
};
        public static Mame.Samplesinterface samples_interface = new Mame.Samplesinterface(1, 100, null);
        public static Mame.DACinterface dac1_interface = new Mame.DACinterface(1, new int[] { 50 });

        public static string[] qbert_sample_names =
{
	"*qbert",
	"fx_17a.wav", /* random speech, voice clock 255 */
	"fx_17b.wav", /* random speech, voice clock 255 */
	"fx_17c.wav", /* random speech, voice clock 255 */
	"fx_17d.wav", /* random speech, voice clock 255 */
	"fx_17e.wav", /* random speech, voice clock 255 */
	"fx_17f.wav", /* random speech, voice clock 255 */
	"fx_17g.wav", /* random speech, voice clock 255 */
	"fx_17h.wav", /* random speech, voice clock 255 */
	"fx_18a.wav", /* random speech, voice clock 176 */
	"fx_18b.wav", /* random speech, voice clock 176 */
	"fx_18c.wav", /* random speech, voice clock 176 */
	"fx_18d.wav", /* random speech, voice clock 176 */
	"fx_18e.wav", /* random speech, voice clock 176 */
	"fx_18f.wav", /* random speech, voice clock 176 */
	"fx_18g.wav", /* random speech, voice clock 176 */
	"fx_18h.wav", /* random speech, voice clock 176 */
	"fx_19a.wav", /* random speech, voice clock 128 */
	"fx_19b.wav", /* random speech, voice clock 128 */
	"fx_19c.wav", /* random speech, voice clock 128 */
	"fx_19d.wav", /* random speech, voice clock 128 */
	"fx_19e.wav", /* random speech, voice clock 128 */
	"fx_19f.wav", /* random speech, voice clock 128 */
	"fx_19g.wav", /* random speech, voice clock 128 */
	"fx_19h.wav", /* random speech, voice clock 128 */
	"fx_20a.wav", /* random speech, voice clock 96 */
	"fx_20b.wav", /* random speech, voice clock 96 */
	"fx_20c.wav", /* random speech, voice clock 96 */
	"fx_20d.wav", /* random speech, voice clock 96 */
	"fx_20e.wav", /* random speech, voice clock 96 */
	"fx_20f.wav", /* random speech, voice clock 96 */
	"fx_20g.wav", /* random speech, voice clock 96 */
	"fx_20h.wav", /* random speech, voice clock 96 */
	"fx_21a.wav", /* random speech, voice clock 62 */
	"fx_21b.wav", /* random speech, voice clock 62 */
	"fx_21c.wav", /* random speech, voice clock 62 */
	"fx_21d.wav", /* random speech, voice clock 62 */
	"fx_21e.wav", /* random speech, voice clock 62 */
	"fx_21f.wav", /* random speech, voice clock 62 */
	"fx_21g.wav", /* random speech, voice clock 62 */
	"fx_21h.wav", /* random speech, voice clock 62 */
	"fx_22.wav", /* EH2 with decreasing voice clock */
	"fx_23.wav", /* O1 with varying voice clock */
	"fx_28.wav", /* "hello, I'm ready" */
	"fx_36.wav", /* "byebye" */
	"knocker.wav",
	null	/* end of array */
};
    }
    class driver_qbert : driver_gottlieb
    {

        public override void driver_init()
        {
            samples_interface.samplenames = qbert_sample_names;
        }
        Mame.RomModule[] rom_qbert()
        {

            ROM_START("qbert");
            ROM_REGION(0x10000, Mame.REGION_CPU1);    /* 64k for code */
            ROM_LOAD("qb-rom2.bin", 0xa000, 0x2000, 0xfe434526);
            ROM_LOAD("qb-rom1.bin", 0xc000, 0x2000, 0x55635447);
            ROM_LOAD("qb-rom0.bin", 0xe000, 0x2000, 0x8e318641);

            ROM_REGION(0x10000, Mame.REGION_CPU2);/* 64k for sound cpu */
            ROM_LOAD("qb-snd1.bin", 0xf000, 0x800, 0x15787c07);
            ROM_RELOAD(0x7000, 0x800); /* A15 is not decoded */
            ROM_LOAD("qb-snd2.bin", 0xf800, 0x800, 0x58437508);
            ROM_RELOAD(0x7800, 0x800); /* A15 is not decoded */

            ROM_REGION(0x2000, Mame.REGION_GFX1 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("qb-bg0.bin", 0x0000, 0x1000, 0x7a9ba824);/* chars */
            ROM_LOAD("qb-bg1.bin", 0x1000, 0x1000, 0x22e5b891);

            ROM_REGION(0x8000, Mame.REGION_GFX2 | Mame.REGIONFLAG_DISPOSE);
            ROM_LOAD("qb-fg3.bin", 0x0000, 0x2000, 0xdd436d3a);	/* sprites */
            ROM_LOAD("qb-fg2.bin", 0x2000, 0x2000, 0xf69b9483);
            ROM_LOAD("qb-fg1.bin", 0x4000, 0x2000, 0x224e8356);
            ROM_LOAD("qb-fg0.bin", 0x6000, 0x2000, 0x2f695b85);
            return ROM_END;
        }
        Mame.InputPortTiny[] input_ports_qbert()
        {
            INPUT_PORTS_START("qbert");
            PORT_START();      /* DSW */
            PORT_DIPNAME(0x01, 0x00, DEF_STR("Demo Sounds"));
            PORT_DIPSETTING(0x01, DEF_STR("Off"));
            PORT_DIPSETTING(0x00, DEF_STR("On"));
            PORT_DIPNAME(0x02, 0x02, "Kicker");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x02, DEF_STR("On"));
            PORT_DIPNAME(0x04, 0x00, DEF_STR("Cabinet"));
            PORT_DIPSETTING(0x00, DEF_STR("Upright"));
            PORT_DIPSETTING(0x04, DEF_STR("Cocktail"));
            PORT_BITX(0x08, 0x00, (uint)inptports.IPT_DIPSWITCH_NAME | IPF_CHEAT, "Auto Round Advance", IP_KEY_NONE, IP_JOY_NONE);
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x08, DEF_STR("On"));
            PORT_DIPNAME(0x10, 0x00, DEF_STR("Free Play"));
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x10, DEF_STR("On"));
            PORT_DIPNAME(0x20, 0x00, "SW5");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x20, DEF_STR("On"));
            PORT_DIPNAME(0x40, 0x00, "SW7");
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x40, DEF_STR("On"));
            PORT_DIPNAME(0x80, 0x00, "SW8"); ;
            PORT_DIPSETTING(0x00, DEF_STR("Off"));
            PORT_DIPSETTING(0x80, DEF_STR("On"));
            PORT_BIT(0xe0, IP_ACTIVE_LOW, IPT_UNUSED);
            /* 0x40 must be connected to the IP16 line */

            PORT_START();     /* buttons */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_START1);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_START2);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN1);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_COIN2);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_UNKNOWN);
            PORT_SERVICE(0x40, IP_ACTIVE_LOW);
            PORT_BITX(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_SERVICE, "Select in Service Mode", (ushort)Mame.InputCodes.KEYCODE_F1, IP_JOY_NONE);

            PORT_START();	/* trackball H not used */
            PORT_BIT(0xff, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();	/* trackball V not used */
            PORT_BIT(0xff, IP_ACTIVE_LOW, IPT_UNUSED);

            PORT_START();     /* joystick */
            PORT_BIT(0x01, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY);
            PORT_BIT(0x02, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY);
            PORT_BIT(0x04, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY);
            PORT_BIT(0x08, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY);
            PORT_BIT(0x10, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_RIGHT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x20, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_LEFT | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x40, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_UP | IPF_4WAY | IPF_COCKTAIL);
            PORT_BIT(0x80, IP_ACTIVE_HIGH, (uint)inptports.IPT_JOYSTICK_DOWN | IPF_4WAY | IPF_COCKTAIL);
            return INPUT_PORTS_END;
        }
        public driver_qbert()
        {
            drv = new machine_driver_gottlieb();
            year = "1982";
            name = "qbert";
            description = "Q*bert (US)";
            manufacturer = "Gottlieb";
            flags = Mame.ROT270;
            input_ports = input_ports_qbert();
            rom = rom_qbert();
            drv.HasNVRAMhandler = false;
        }
    }
}
