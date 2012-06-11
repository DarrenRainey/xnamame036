using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class AvgDvg
    {
        const byte AVGDVG_MIN = 1;
        const byte USE_DVG = 1;
        const byte USE_AVG_RBARON = 2;
        const byte USE_AVG_BZONE = 3;
        const byte USE_AVG = 4;
        const byte USE_AVG_TEMPEST = 5;
        const byte USE_AVG_MHAVOC = 6;
        const byte USE_AVG_SWARS = 7;
        const byte USE_AVG_QUANTUM = 8;
        const byte AVGDVG_MAX = 8;

        public static int vector_updates;
        const byte VEC_SHIFT = 16, BRIGHTNESS = 12;
        static bool busy = false;
        static int vectorEngine = USE_DVG;
        static int flipword = 0;
        static int[] colorram = new int[16];
        static int width, height, xcenter, ycenter, xmin, xmax, ymin, ymax;
        static int vg_step, total_length;

        static Mame.artwork backdrop = null;

        const int BANK_BITS = 13;
        const int BANK_SIZE = 1 << BANK_BITS;
        const int NUM_BANKS = 0x4000 / BANK_SIZE;
        static _BytePtr[] vectorbank = new _BytePtr[NUM_BANKS];

        public static int dvg_start()
        {
            if (AvgDvg.backdrop != null)
            {
                Mame.backdrop_refresh(backdrop);
                Mame.backdrop_refresh_tables(backdrop);
            }

            return avgdvg_init(USE_DVG);
        }

        static int map_addr(int n) { return n << 1; }
        static int VECTORRAM(int offset)
        {
            return (vectorbank[(offset) >> BANK_BITS][(offset) & (BANK_SIZE - 1)]);
        }
        static int memrdwd(int offset)
        {
            return (VECTORRAM(offset) | (VECTORRAM(offset + 1) << 8));
        }
        static int memrdwd_flip(int offset) { return (VECTORRAM(offset + 1) | (VECTORRAM(offset) << 8)); }

        const byte VEC_PAL_WHITE = 1, VEC_PAL_AQUA = 2, VEC_PAL_BZONE = 3, VEC_PAL_SWARS = 5, VEC_PAL_MULTI = 4, VEC_PAL_ASTDELUX = 6;
        const byte RED = 0x04;
        const byte GREEN = 0x02;
        const byte BLUE = 0x01;
        const byte WHITE = RED | GREEN | BLUE;

        static void shade_fill(byte[] palette, int rgb, int start_index, int end_index, int start_inten, int end_inten)
        {
            int i, inten, index_range, inten_range;

            index_range = end_index - start_index;
            inten_range = end_inten - start_inten;
            for (i = start_index; i <= end_index; i++)
            {
                inten = start_inten + (inten_range) * (i - start_index) / (index_range);
                palette[3 * i] = (byte)((rgb & RED) != 0 ? inten : 0);
                palette[3 * i + 1] = (byte)((rgb & GREEN) != 0 ? inten : 0);
                palette[3 * i + 2] = (byte)((rgb & BLUE) != 0 ? inten : 0);
            }
        }
        static void avg_init_palette(int paltype, byte[] palette, ushort[] colortable, _BytePtr color_prom)
        {
            int i, j, k;

            int[] trcl1 = { 0, 0, 2, 2, 1, 1 };
            int[] trcl2 = { 1, 2, 0, 1, 0, 2 };
            int[] trcl3 = { 2, 1, 1, 0, 2, 0 };

            /* initialize the first 8 colors with the basic colors */
            /* Only these are selected by writes to the colorram. */
            for (i = 0; i < 8; i++)
            {
                palette[3 * i] = (byte)((i & RED) != 0 ? 0xff : 0);
                palette[3 * i + 1] = (byte)((i & GREEN) != 0 ? 0xff : 0);
                palette[3 * i + 2] = (byte)((i & BLUE) != 0 ? 0xff : 0);
            }

            /* initialize the colorram */
            for (i = 0; i < 16; i++)
                colorram[i] = i & 0x07;

            /* fill the rest of the 256 color entries depending on the game */
            switch (paltype)
            {
                /* Black and White vector colors (Asteroids,Omega Race) .ac JAN2498 */
                case VEC_PAL_WHITE:
                    shade_fill(palette, RED | GREEN | BLUE, 8, 128 + 8, 0, 255);
                    colorram[1] = 7; /* BW games use only color 1 (== white) */
                    break;

                /* Monochrome Aqua colors (Asteroids Deluxe,Red Baron) .ac JAN2498 */
                case VEC_PAL_ASTDELUX:
                    /* Use backdrop if present MLR OCT0598 */
                    if ((backdrop = Mame.artwork_load("astdelux.png", 32, (int)(Mame.Machine.drv.total_colors - 32))) != null)
                    {
                        shade_fill(palette, GREEN | BLUE, 8, 23, 1, 254);
                        /* Some more anti-aliasing colors. */
                        shade_fill(palette, GREEN | BLUE, 24, 31, 1, 254);
                        for (i = 0; i < 8; i++)
                            palette[(24 + i) * 3] = 80;
                        for (i = 0; i < 3 * backdrop.num_pens_used; i++)
                            palette[i + 3 * backdrop.start_pen] = backdrop.orig_palette[i];
                        //memcpy (palette+3*backdrop.start_pen, backdrop.orig_palette,3*backdrop.num_pens_used);
                    }
                    else
                        shade_fill(palette, GREEN | BLUE, 8, 128 + 8, 1, 254);
                    colorram[1] = 3; /* for Asteroids */
                    break;

                case VEC_PAL_AQUA:
                    shade_fill(palette, GREEN | BLUE, 8, 128 + 8, 1, 254);
                    colorram[0] = 3; /* for Red Baron */
                    break;

                /* Monochrome Green/Red vector colors (Battlezone) .ac JAN2498 */
                case VEC_PAL_BZONE:
                    shade_fill(palette, RED, 8, 23, 1, 254);
                    shade_fill(palette, GREEN, 24, 31, 1, 254);
                    shade_fill(palette, WHITE, 32, 47, 1, 254);
                    /* Use backdrop if present MLR OCT0598 */
                    if ((backdrop = Mame.artwork_load("bzone.png", 48, (int)(Mame.Machine.drv.total_colors - 48))) != null)
                        for (i = 0; i < 3 * backdrop.num_pens_used; i++)
                            palette[3 * backdrop.start_pen + i] = backdrop.orig_palette[i];
                    //memcpy (palette+3*backdrop.start_pen, backdrop.orig_palette, 3*backdrop.num_pens_used);
                    break;

                /* Colored games (Major Havoc, Star Wars, Tempest) .ac JAN2498 */
                case VEC_PAL_MULTI:
                case VEC_PAL_SWARS:
                    /* put in 40 shades for red, blue and magenta */
                    shade_fill(palette, RED, 8, 47, 10, 250);
                    shade_fill(palette, BLUE, 48, 87, 10, 250);
                    shade_fill(palette, RED | BLUE, 88, 127, 10, 250);

                    /* put in 20 shades for yellow and green */
                    shade_fill(palette, GREEN, 128, 147, 10, 250);
                    shade_fill(palette, RED | GREEN, 148, 167, 10, 250);

                    /* and 14 shades for cyan and white */
                    shade_fill(palette, BLUE | GREEN, 168, 181, 10, 250);
                    shade_fill(palette, WHITE, 182, 194, 10, 250);

                    /* Fill in unused gaps with more anti-aliasing colors. */
                    /* There are 60 slots available.           .ac JAN2498 */
                    i = 195;
                    for (j = 0; j < 6; j++)
                    {
                        for (k = 7; k <= 16; k++)
                        {
                            palette[3 * i + trcl1[j]] = (byte)(((256 * k) / 16) - 1);
                            palette[3 * i + trcl2[j]] = (byte)(((128 * k) / 16) - 1);
                            palette[3 * i + trcl3[j]] = 0;
                            i++;
                        }
                    }
                    break;
                default:
                    Mame.printf("Wrong palette type in avgdvg.c");
                    break;
            }
        }
        public static void avg_init_palette_white(byte[] palette, ushort[] colortable, _BytePtr color_prom)
        {
            avg_init_palette(VEC_PAL_WHITE, palette, colortable, color_prom);
        }



        const int BZONE_TOP = 0x0050;
        const int MHAVOC_YWINDOW = 0x0048;

        const byte MAXSTACK = 8;

        const byte VCTR = 0;
        const byte HALT = 1;
        const byte SVEC = 2;
        const byte STAT = 3;
        const byte CNTR = 4;
        const byte JSRL = 5;
        const byte RTSL = 6;
        const byte JMPL = 7;
        const byte SCAL = 8;

        const byte DVCTR = 0x01;
        const byte DLABS = 0x0a;
        const byte DHALT = 0x0b;
        const byte DJSRL = 0x0c;
        const byte DRTSL = 0x0d;
        const byte DJMPL = 0x0e;
        const byte DSVEC = 0x0f;

        public static bool avgdvg_done()
        {
            if (busy)
                return false;
            else
                return true;
        }
        static void avgdvg_clr_busy(int dummy)
        {
            busy = false;
        }

        static int twos_comp_val(int num, byte bits) { return ((num & (1 << (bits - 1))) != 0 ? (num | ~((1 << bits) - 1)) : (num & ((1 << bits) - 1))); }
        static void dvg_vector_timer(int scale)
        {
            total_length += scale;
        }

        static void dvg_generate_vector_list()
        {
            int pc;
            int sp;
            int[] stack = new int[MAXSTACK];

            int scale;
            int statz;

            int currentx, currenty;

            bool done = false;

            int firstwd;
            int secondwd = 0; /* Initialize to tease the compiler */
            int opcode;

            int x, y;
            int z, temp;
            int a;

            int deltax, deltay;

            Vector.vector_clear_list();
            pc = 0;
            sp = 0;
            scale = 0;
            statz = 0;

            currentx = 0;
            currenty = 0;

            while (!done)
            {

#if VG_DEBUG
		if (vg_step)
		{
	  		if (errorlog) fprintf (errorlog,"Current beam position: (%d, %d)\n",
				currentx, currenty);
	  		getchar();
		}
#endif

                firstwd = memrdwd(map_addr(pc));
                opcode = firstwd >> 12;
#if VG_DEBUG
		if (errorlog) fprintf (errorlog,"%4x: %4x ", map_addr (pc), firstwd);
#endif
                pc++;
                if ((opcode >= 0 /* DVCTR */) && (opcode <= DLABS))
                {
                    secondwd = memrdwd(map_addr(pc));
                    pc++;
#if VG_DEBUG
			if (errorlog)
			{
				fprintf (errorlog,"%s ", dvg_mnem [opcode]);
				fprintf (errorlog,"%4x  ", secondwd);
			}
#endif
                }
#if VG_DEBUG
		else if (errorlog) fprintf (errorlog,"Illegal opcode ");
#endif

                switch (opcode)
                {
                    case 0:
#if VG_DEBUG
	 			if (errorlog) fprintf (errorlog,"Error: DVG opcode 0!  Addr %4x Instr %4x %4x\n", map_addr (pc-2), firstwd, secondwd);
				done = 1;
				break;
#endif
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        y = firstwd & 0x03ff;
                        if ((firstwd & 0x400) != 0)
                            y = -y;
                        x = secondwd & 0x3ff;
                        if ((secondwd & 0x400) != 0)
                            x = -x;
                        z = secondwd >> 12;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"(%d,%d) z: %d scal: %d", x, y, z, opcode);
#endif
                        temp = ((scale + opcode) & 0x0f);
                        if (temp > 9)
                            temp = -1;
                        deltax = (x << VEC_SHIFT) >> (9 - temp);		/* ASG 080497 */
                        deltay = (y << VEC_SHIFT) >> (9 - temp);		/* ASG 080497 */
                        currentx += deltax;
                        currenty -= deltay;
                        dvg_vector_timer(temp);

                        /* ASG 080497, .ac JAN2498 - V.V */
                        if (Vector.translucency != 0)
                            z = z * BRIGHTNESS;
                        else
                            if (z != 0) z = (z << 4) | 0x0f;
                        Vector.vector_add_point(currentx, currenty, colorram[1], z);

                        break;

                    case DLABS:
                        x = twos_comp_val(secondwd, 12);
                        y = twos_comp_val(firstwd, 12);
                        scale = (secondwd >> 12);
                        currentx = ((x - xmin) << VEC_SHIFT);		/* ASG 080497 */
                        currenty = ((ymax - y) << VEC_SHIFT);		/* ASG 080497 */
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"(%d,%d) scal: %d", x, y, secondwd >> 12);
#endif
                        break;

                    case DHALT:
#if VG_DEBUG
				if (errorlog && ((firstwd & 0x0fff) != 0))
	      				fprintf (errorlog,"(%d?)", firstwd & 0x0fff);
#endif
                        done = true;
                        break;

                    case DJSRL:
                        a = firstwd & 0x0fff;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"%4x", map_addr(a));
#endif
                        stack[sp] = pc;
                        if (sp == (MAXSTACK - 1))
                        {
                            Mame.printf("\n*** Vector generator stack overflow! ***\n");
                            done = true;
                            sp = 0;
                        }
                        else
                            sp++;
                        pc = a;
                        break;

                    case DRTSL:
#if VG_DEBUG
				if (errorlog && ((firstwd & 0x0fff) != 0))
					 fprintf (errorlog,"(%d?)", firstwd & 0x0fff);
#endif
                        if (sp == 0)
                        {
                            Mame.printf("\n*** Vector generator stack underflow! ***\n");
                            done = true;
                            sp = MAXSTACK - 1;
                        }
                        else
                            sp--;
                        pc = stack[sp];
                        break;

                    case DJMPL:
                        a = firstwd & 0x0fff;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"%4x", map_addr(a));
#endif
                        pc = a;
                        break;

                    case DSVEC:
                        y = firstwd & 0x0300;
                        if ((firstwd & 0x0400) != 0)
                            y = -y;
                        x = (firstwd & 0x03) << 8;
                        if ((firstwd & 0x04) != 0)
                            x = -x;
                        z = (firstwd >> 4) & 0x0f;
                        temp = 2 + ((firstwd >> 2) & 0x02) + ((firstwd >> 11) & 0x01);
                        temp = ((scale + temp) & 0x0f);
                        if (temp > 9)
                            temp = -1;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"(%d,%d) z: %d scal: %d", x, y, z, temp);
#endif

                        deltax = (x << VEC_SHIFT) >> (9 - temp);	/* ASG 080497 */
                        deltay = (y << VEC_SHIFT) >> (9 - temp);	/* ASG 080497 */
                        currentx += deltax;
                        currenty -= deltay;
                        dvg_vector_timer(temp);

                        /* ASG 080497, .ac JAN2498 */
                        if (Vector.translucency != 0)
                            z = z * BRIGHTNESS;
                        else
                            if (z != 0) z = (z << 4) | 0x0f;
                        Vector.vector_add_point(currentx, currenty, colorram[1], z);
                        break;

                    default:
                        Mame.printf("Unknown DVG opcode found\n");
                        done = true;
                        break;
                }
#if VG_DEBUG
      		if (errorlog) fprintf (errorlog,"\n");
#endif
            }
        }
        static void vector_timer(int deltax, int deltay)
        {
            deltax = Math.Abs(deltax);
            deltay = Math.Abs(deltay);
            total_length += Math.Max(deltax, deltay) >> VEC_SHIFT;
        }
        static void avg_generate_vector_list()
        {

            int pc;
            int sp;
            int[] stack = new int[MAXSTACK];

            int scale;
            int statz = 0;
            int sparkle = 0;
            int xflip = 0;

            int color = 0;
            int bz_col = -1; /* Battle Zone color selection */
            int ywindow = -1; /* Major Havoc Y-Window */

            int currentx, currenty;
            bool done = false;

            int firstwd, secondwd;
            int opcode;

            int x, y, z = 0, b, l, d, a;

            int deltax, deltay;


            pc = 0;
            sp = 0;
            statz = 0;
            color = 0;

            if (flipword != 0)
            {
                firstwd = memrdwd_flip(map_addr(pc));
                secondwd = memrdwd_flip(map_addr(pc + 1));
            }
            else
            {
                firstwd = memrdwd(map_addr(pc));
                secondwd = memrdwd(map_addr(pc + 1));
            }
            if ((firstwd == 0) && (secondwd == 0))
            {
                Mame.printf("VGO with zeroed vector memory\n");
                return;
            }

            /* kludge to bypass Major Havoc's empty frames. BW 980216 */
            if (vectorEngine == USE_AVG_MHAVOC && firstwd == 0xafe2)
                return;

            scale = 0;          /* ASG 080497 */
            currentx = xcenter; /* ASG 080497 */ /*.ac JAN2498 */
            currenty = ycenter; /* ASG 080497 */ /*.ac JAN2498 */

            Vector.vector_clear_list();

            while (!done)
            {

#if VG_DEBUG
		if (vg_step) getchar();
#endif

                if (flipword != 0) firstwd = memrdwd_flip(map_addr(pc));
                else firstwd = memrdwd(map_addr(pc));

                opcode = firstwd >> 13;
#if VG_DEBUG
		if (errorlog) fprintf (errorlog,"%4x: %4x ", map_addr (pc), firstwd);
#endif
                pc++;
                if (opcode == VCTR)
                {
                    if (flipword != 0) secondwd = memrdwd_flip(map_addr(pc));
                    else secondwd = memrdwd(map_addr(pc));
                    pc++;
#if VG_DEBUG
			if (errorlog) fprintf (errorlog,"%4x  ", secondwd);
#endif
                }
#if VG_DEBUG
		else if (errorlog) fprintf (errorlog,"      ");
#endif

                if ((opcode == STAT) && ((firstwd & 0x1000) != 0))
                    opcode = SCAL;

#if VG_DEBUG
		if (errorlog) fprintf (errorlog,"%s ", avg_mnem [opcode]);
#endif

                switch (opcode)
                {
                    case VCTR:

                        if (vectorEngine == USE_AVG_QUANTUM)
                        {
                            x = twos_comp_val(secondwd, 12);
                            y = twos_comp_val(firstwd, 12);
                        }
                        else
                        {
                            /* These work for all other games. */
                            x = twos_comp_val(secondwd, 13);
                            y = twos_comp_val(firstwd, 13);
                        }
                        z = (secondwd >> 12) & ~0x01;

                        /* z is the maximum DAC output, and      */
                        /* the 8 bit value from STAT does some   */
                        /* fine tuning. STATs of 128 should give */
                        /* highest intensity. */
                        if (vectorEngine == USE_AVG_SWARS)
                        {
                            if (Vector.translucency != 0)
                                z = (statz * z) / 12;
                            else
                                z = (statz * z) >> 3;
                            if (z > 0xff)
                                z = 0xff;
                        }
                        else
                        {
                            if (z == 2)
                                z = statz;
                            if (Vector.translucency != 0)
                                z = z * BRIGHTNESS;
                            else
                                if (z != 0) z = (z << 4) | 0x1f;
                        }

                        deltax = x * scale;
                        if (xflip != 0) deltax = -deltax;

                        deltay = y * scale;
                        currentx += deltax;
                        currenty -= deltay;
                        vector_timer(deltax, deltay);

                        if (sparkle != 0)
                        {
                            color = Mame.rand() & 0x07;
                        }

                        if ((vectorEngine == USE_AVG_BZONE) && (bz_col != 0))
                        {
                            if (currenty < (BZONE_TOP << 16))
                                color = 4;
                            else
                                color = 2;
                        }

                        Vector.vector_add_point(currentx, currenty, colorram[color], z);

#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"VCTR x:%d y:%d z:%d statz:%d", x, y, z, statz);
#endif
                        break;

                    case SVEC:
                        x = twos_comp_val(firstwd, 5) << 1;
                        y = twos_comp_val(firstwd >> 8, 5) << 1;
                        z = ((firstwd >> 4) & 0x0e);

                        if (vectorEngine == USE_AVG_SWARS)
                        {
                            if (Vector.translucency != 0)
                                z = (statz * z) / 12;
                            else
                                z = (statz * z) >> 3;
                            if (z > 0xff) z = 0xff;
                        }
                        else
                        {
                            if (z == 2)
                                z = statz;
                            if (Vector.translucency != 0)
                                z = z * BRIGHTNESS;
                            else
                                if (z != 0) z = (z << 4) | 0x1f;
                        }

                        deltax = x * scale;
                        if (xflip != 0) deltax = -deltax;

                        deltay = y * scale;
                        currentx += deltax;
                        currenty -= deltay;
                        vector_timer(deltax, deltay);

                        if (sparkle != 0)
                        {
                            color = Mame.rand() & 0x07;
                        }

                        Vector.vector_add_point(currentx, currenty, colorram[color], z);

#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"SVEC x:%d y:%d z:%d statz:%d", x, y, z, statz);
#endif
                        break;

                    case STAT:
                        if (vectorEngine == USE_AVG_SWARS)
                        {
                            /* color code 0-7 stored in top 3 bits of `color' */
                            color = (char)((firstwd & 0x0700) >> 8);
                            statz = (firstwd) & 0xff;
                        }
                        else
                        {
                            color = (firstwd) & 0x000f;
                            statz = (firstwd >> 4) & 0x000f;
                            if (vectorEngine == USE_AVG_TEMPEST)
                                sparkle = (firstwd & 0x0800) == 0 ? 1 : 0;
                            if (vectorEngine == USE_AVG_MHAVOC)
                            {
                                sparkle = (firstwd & 0x0800);
                                xflip = firstwd & 0x0400;
                                /* Bank switch the vector ROM for Major Havoc */
                                vectorbank[1] = new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), 0x18000 + ((firstwd & 0x300) >> 8) * 0x2000);
                            }
                            if (vectorEngine == USE_AVG_BZONE)
                            {
                                bz_col = color;
                                if (color == 0)
                                {
                                    Vector.vector_add_clip(xmin << VEC_SHIFT, BZONE_TOP << VEC_SHIFT, xmax << VEC_SHIFT, ymax << VEC_SHIFT);
                                    color = 2;
                                }
                                else
                                {
                                    Vector.vector_add_clip(xmin << VEC_SHIFT, ymin << VEC_SHIFT, xmax << VEC_SHIFT, ymax << VEC_SHIFT);
                                }
                            }
                        }
#if VG_DEBUG
				if (errorlog)
				{
					fprintf (errorlog,"STAT: statz: %d color: %d",
										 statz, color);
					if (xflip || sparkle)
						fprintf (errorlog, "xflip: %02x  sparkle: %02x\n",
											 xflip, sparkle);
				}
#endif

                        break;

                    case SCAL:
                        b = ((firstwd >> 8) & 0x07) + 8;
                        l = (~firstwd) & 0xff;
                        scale = (l << VEC_SHIFT) >> b;		/* ASG 080497 */

                        /* Y-Window toggle for Major Havoc BW 980318 */
                        if (vectorEngine == USE_AVG_MHAVOC)
                        {
                            if ((firstwd & 0x0800) != 0)
                            {
                                Mame.printf("CLIP %d\n", firstwd & 0x0800);
                                if (ywindow == 0)
                                {
                                    ywindow = 1;
                                    Vector.vector_add_clip(xmin << VEC_SHIFT, MHAVOC_YWINDOW << VEC_SHIFT, xmax << VEC_SHIFT, ymax << VEC_SHIFT);
                                }
                                else
                                {
                                    ywindow = 0;
                                    Vector.vector_add_clip(xmin << VEC_SHIFT, ymin << VEC_SHIFT, xmax << VEC_SHIFT, ymax << VEC_SHIFT);
                                }
                            }
                        }
#if VG_DEBUG
				if (errorlog)
				{
					fprintf (errorlog,"bin: %d, lin: ", b);
					if (l > 0x80)
						fprintf (errorlog,"(%d?)", l);
					else
						fprintf (errorlog,"%d", l);
					fprintf (errorlog," scale: %f", (scale/(float)(1<<VEC_SHIFT)));
				}
#endif
                        break;

                    case CNTR:
                        d = firstwd & 0xff;
#if VG_DEBUG
				if (errorlog && (d != 0x40))
					fprintf (errorlog,"%d", d);
#endif
                        currentx = xcenter;  /* ASG 080497 */ /*.ac JAN2498 */
                        currenty = ycenter;  /* ASG 080497 */ /*.ac JAN2498 */
                        Vector.vector_add_point(currentx, currenty, 0, 0);
                        break;

                    case RTSL:
#if VG_DEBUG
				if (errorlog && ((firstwd & 0x1fff) != 0))
					fprintf (errorlog,"(%d?)", firstwd & 0x1fff);
#endif
                        if (sp == 0)
                        {
                            Mame.printf("\n*** Vector generator stack underflow! ***\n");
                            done = true;
                            sp = MAXSTACK - 1;
                        }
                        else
                            sp--;

                        pc = stack[sp];
                        break;

                    case HALT:
#if VG_DEBUG
				if (errorlog && ((firstwd & 0x1fff) != 0))
					fprintf (errorlog,"(%d?)", firstwd & 0x1fff);
#endif
                        done = true;
                        break;

                    case JMPL:
                        a = firstwd & 0x1fff;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"%4x", map_addr(a));
#endif
                        /* if a = 0x0000, treat as HALT */
                        if (a == 0x0000)
                            done = true;
                        else
                            pc = a;
                        break;

                    case JSRL:
                        a = firstwd & 0x1fff;
#if VG_DEBUG
				if (errorlog) fprintf (errorlog,"%4x", map_addr(a));
#endif
                        /* if a = 0x0000, treat as HALT */
                        if (a == 0x0000)
                            done = true;
                        else
                        {
                            stack[sp] = pc;
                            if (sp == (MAXSTACK - 1))
                            {
                                Mame.printf("\n*** Vector generator stack overflow! ***\n");
                                done = true;
                                sp = 0;
                            }
                            else
                                sp++;

                            pc = a;
                        }
                        break;

                    default:
                        Mame.printf("internal error\n");
                        break;
                }
#if VG_DEBUG
		if (errorlog) fprintf (errorlog,"\n");
#endif
            }
        }
        public static void avgdvg_go(int offset, int data)
        {
            if (busy)
                return;

            vector_updates++;
            total_length = 1;
            busy = true;

            if (vectorEngine == USE_DVG)
            {
                dvg_generate_vector_list();
                Mame.Timer.timer_set(Mame.Timer.TIME_IN_NSEC(4500) * total_length, 1, avgdvg_clr_busy);
            }
            else
            {
                avg_generate_vector_list();
                if (total_length > 1)
                    Mame.Timer.timer_set(Mame.Timer.TIME_IN_NSEC(1500) * total_length, 1, avgdvg_clr_busy);
                /* this is for Major Havoc */
                else
                {
                    vector_updates--;
                    busy = false;
                }
            }
        }
        static int avgdvg_init(int vgType)
        {
            int i;

            if (Vector.vectorram_size[0] == 0)
            {
                Mame.printf("Error: vectorram_size not initialized\n");
                return 1;
            }

            /* ASG 971210 -- initialize the pages */
            for (i = 0; i < NUM_BANKS; i++)
                vectorbank[i] = new _BytePtr(Vector.vectorram, (i << BANK_BITS));
            if (vgType == USE_AVG_MHAVOC)
                vectorbank[1] = new _BytePtr(Mame.memory_region(Mame.REGION_CPU1), 0x18000);

            vectorEngine = vgType;
            if ((vectorEngine < AVGDVG_MIN) || (vectorEngine > AVGDVG_MAX))
            {
                Mame.printf("Error: unknown Atari Vector Game Type\n");
                return 1;
            }

            if (vectorEngine == USE_AVG_SWARS)
                flipword = 1;
#if !WINDOWS
	else if (vectorEngine==USE_AVG_QUANTUM)
		flipword=1;
#endif
            else
                flipword = 0;

            vg_step = 0;

            busy = false;

            xmin = Mame.Machine.drv.visible_area.min_x;
            ymin = Mame.Machine.drv.visible_area.min_y;
            xmax = Mame.Machine.drv.visible_area.max_x;
            ymax = Mame.Machine.drv.visible_area.max_y;
            width = xmax - xmin;
            height = ymax - ymin;

            xcenter = ((xmax + xmin) / 2) << VEC_SHIFT; /*.ac JAN2498 */
            ycenter = ((ymax + ymin) / 2) << VEC_SHIFT; /*.ac JAN2498 */

            Vector.vector_set_shift(VEC_SHIFT);

            if (Vector.vector_vh_start() != 0)
                return 1;

            return 0;
        }

        public static void avg_stop()
        {
            busy = false;
            Vector.vector_clear_list();

            Vector.vector_vh_stop();

            if (backdrop != null) Mame.artwork_free(ref backdrop);
            backdrop = null;
        }
        public static void dvg_stop()
        {
            busy = false;
            Vector.vector_clear_list();

            Vector.vector_vh_stop();

            if (backdrop != null) Mame.artwork_free(ref backdrop);
            backdrop = null;
        }
        public static void avg_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            if (backdrop != null)
                Vector.vector_vh_update_backdrop(bitmap, backdrop, full_refresh);
            else
                Vector.vector_vh_update(bitmap, full_refresh);
        }
        public static void dvg_screenrefresh(Mame.osd_bitmap bitmap, int full_refresh)
        {
            if (backdrop != null)
                Vector.vector_vh_update_backdrop(bitmap, backdrop, full_refresh);
            else
                Vector.vector_vh_update(bitmap, full_refresh);
        }

    }
}
