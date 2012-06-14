using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace xnamame036.mame
{
    partial class Mame
    {
        struct RGB
        {
            public byte r, g, b;
        }
        const int MAX_X_MULTIPLY = 4, MAX_Y_MULTIPLY = 3, MAX_X_MULTIPLY16 = 4, MAX_Y_MULTIPLY16 = 2;
        const int BACKGROUND = 0;
        static osd_bitmap scrbitmap;
        static int modifiable_palette;
        static int screen_colors;
        static _BytePtr current_palette;
        static bool[] dirtycolor;
        static bool dirtypalette;
        static int dirty_bright;
        static int frameskip;
        static int[] bright_lookup = new int[256];

        static int frameskip_counter;
        static int frames_displayed;
        static long start_time, end_time;    /* to calculate fps average on exit */
        const int FRAMES_TO_SKIP = 20;       /* skip the first few frames from the FPS calculation */
        /* to avoid counting the copyright and info screens */


        static bool warming_up;
        static bool autoframeskip;
        const int FRAMESKIP_LEVELS = 12;

        /* type of monitor output- */
        /* Standard PC, NTSC, PAL or Arcade */
        static int game_width;
        static int game_height;
        static int game_attributes;
        static int video_sync;
        static int wait_vsync;
        static int use_triplebuf;
        static int vsync_frame_rate;
        static int skiplines;
        static int skipcolumns;
        static int scanlines;
        static int stretch;
        static int use_mmx;
        static int use_tweaked;
        static int use_vesa;
        static int use_dirty;
        static float osd_gamma_correction = 1.0f;
        static int brightness;
        static float brightness_paused_adjust;
        
        
        static int gfx_width;
        static int gfx_height;


        
        static int auto_resolution;
        static int viswidth;
        static int visheight;
        static int skiplinesmax;
        static int skipcolumnsmax;
        static int skiplinesmin;
        static int skipcolumnsmin;

        static bool vector_game;


        static int gfx_xoffset;
        static int gfx_yoffset;
        static int gfx_display_lines;
        static int gfx_display_columns;
        static int xmultiply, ymultiply;
        static bool throttle = true;       /* toggled by F10 */


        const int _SKIP = 20;       /* skip the first few frames from the FPS calculation */
        /* to avoid counting the copyright and info screens */
        static byte[] grid1 = new byte[DIRTY_V * DIRTY_H];
        static byte[] grid2 = new byte[DIRTY_V * DIRTY_H];
        public static byte[] dirty_old = grid1;
        public static byte[] dirty_new = grid2;
        public static void osd_mark_dirty(int _x1, int _y1, int _x2, int _y2, int ui)
        {
            if (use_dirty != 0)
            {
                int x, y;

                //        if (errorlog) fprintf(errorlog, "mark_dirty %3d,%3d - %3d,%3d\n", _x1,_y1, _x2,_y2);

                _x1 -= skipcolumns;
                _x2 -= skipcolumns;
                _y1 -= skiplines;
                _y2 -= skiplines;

                if (_y1 >= gfx_display_lines || _y2 < 0 || _x1 > gfx_display_columns || _x2 < 0) return;
                if (_y1 < 0) _y1 = 0;
                if (_y2 >= gfx_display_lines) _y2 = gfx_display_lines - 1;
                if (_x1 < 0) _x1 = 0;
                if (_x2 >= gfx_display_columns) _x2 = gfx_display_columns - 1;

                for (y = _y1; y <= _y2 + 15; y += 16)
                    for (x = _x1; x <= _x2 + 15; x += 16)
                        MARKDIRTY(x, y);
            }
        }
        static osd_bitmap osd_create_display(int width, int height, int depth, int attributes)
        {
            printf("width %d, height %d\n", width, height);

            brightness = 100;
            brightness_paused_adjust = 1.0f;
            dirty_bright = 1;

            if (frameskip < 0) frameskip = 0;
            if (frameskip >= FRAMESKIP_LEVELS) frameskip = FRAMESKIP_LEVELS - 1;            

            /* Look if this is a vector game */
            if ((Machine.drv.video_attributes & VIDEO_TYPE_VECTOR) != 0)
                vector_game = true;
            else
                vector_game = false;


            if (use_dirty == -1)	/* dirty=auto in mame.cfg? */
            {
                /* Is the game using a dirty system? */
                if ((Machine.drv.video_attributes & VIDEO_SUPPORTS_DIRTY) != 0 || vector_game)
                    use_dirty = 1;
                else
                    use_dirty = 0;
            }

            select_display_mode(depth);

            if (vector_game)
            {
                scale_vectorgames(gfx_width, gfx_height, ref width, ref height);
            }

            game_width = width;
            game_height = height;
            game_attributes = attributes;

            if (depth == 16)
                scrbitmap = osd_new_bitmap(width, height, 16);
            else
                scrbitmap = osd_new_bitmap(width, height, 8);

            if (scrbitmap == null) return null;

            ///* find a VESA driver for 15KHz modes just in case we need it later on */
            //    if (scanrate15KHz)
            //        getSVGA15KHzdriver (&SVGA15KHzdriver);
            //    else
            //        SVGA15KHzdriver = 0;


            if (osd_set_display(width, height, attributes) == 0)
                return null;

            /* center display based on visible area */
            if (vector_game)
                adjust_display(0, 0, width - 1, height - 1, depth);
            else
            {
                rectangle vis = Machine.drv.visible_area;
                adjust_display(vis.min_x, vis.min_y, vis.max_x, vis.max_y, depth);
            }

            /*Check for SVGA 15.75KHz mode (req. for 15.75KHz Arcade Monitor Modes)
              need to do this here, as the double params will be set up correctly */
            //if (use_vesa == 1 && scanrate15KHz)
            //{
            //    int dbl;
            //    dbl = (ymultiply >= 2);
            //    /* check that we found a driver */
            //    if (!SVGA15KHzdriver)
            //    {
            //        printf ("\nUnable to find 15.75KHz SVGA driver for %dx%d\n", gfx_width, gfx_height);
            //        return 0;
            //    }
            //    if(errorlog)
            //        fprintf (errorlog, "Using %s 15.75KHz SVGA driver\n", SVGA15KHzdriver.name);
            //    /*and try to set the mode */
            //    if (!SVGA15KHzdriver.setSVGA15KHzmode (dbl, gfx_width, gfx_height))
            //    {
            //        printf ("\nUnable to set SVGA 15.75KHz mode %dx%d (driver: %s)\n", gfx_width, gfx_height, SVGA15KHzdriver.name);
            //        return 0;
            //    }
            //    /* if we're doubling, we might as well have scanlines */
            //    /* the 15.75KHz driver is going to drop every other line anyway -
            //        so we can avoid drawing them and save some time */
            //    if(dbl)
            //        scanlines=1;
            //}

            return scrbitmap;
        }
        /* set the actual display screen but don't allocate the screen bitmap */
        static int osd_set_display(int width, int height, int attributes)
        {
            ////mode_adjust adjust_array;

            int i;
            /* moved 'found' to here (req. for 15.75KHz Arcade Monitor Modes) */
            int found;
          
            if (gfx_height == 0 || gfx_width == 0)
            {
                printf("Please specify height AND width (e.g. -640x480)\n");
                return 0;
            }

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                int temp;

                temp = width;
                width = height;
                height = temp;
            }
            /* Mark the dirty buffers as dirty */

            if (use_dirty != 0)
            {
                if (vector_game)
                    /* vector games only use one dirty buffer */
                    init_dirty(0);
                else
                    init_dirty(1);
                swap_dirty();
                init_dirty(1);
            }
            if (dirtycolor != null)
            {
                for (i = 0; i < screen_colors; i++)
                    dirtycolor[i] = true;
                dirtypalette = true;
            }
            /* handle special 15.75KHz modes, these now include SVGA modes */
            found = 0;
            /*move video freq set to here, as we need to set it explicitly for the 15.75KHz modes */
            //videofreq = vgafreq;

            use_vesa = 0;
            if (use_vesa != 0)
            {
                /*removed local 'found' */
                int bits;

                
                found = 0;
                bits = scrbitmap.depth;

                /* Try the specified vesamode, 565 and 555 for 16 bit color modes, */
                /* doubled resolution in case of noscanlines and if not succesful  */
                /* repeat for all "lower" VESA modes. NS/BW 19980102 */

                while (found == 0)
                {
                    set_color_depth(bits);

                    /* allocate a wide enough virtual screen if possible */
                    /* we round the width (in dwords) to be an even multiple 256 - that */
                    /* way, during page flipping only one byte of the video RAM */
                    /* address changes, therefore preventing flickering. */
                    //if (bits == 8)
                    //    triplebuf_page_width = (gfx_width + 0x3ff) & ~0x3ff;
                    //else
                    //    triplebuf_page_width = (gfx_width + 0x1ff) & ~0x1ff;

                    /* don't ask for a larger screen if triplebuffer not requested - could */
                    /* cause problems in some cases. */
                    //err = 1;
                    //if (use_triplebuf!=0)
                    //    err = set_gfx_mode(mode,gfx_width,gfx_height,3*triplebuf_page_width,0);
                    //if (err!=0)
                    //{
                    //    /* if we're using a SVGA 15KHz driver - tell Allegro the virtual screen width */
                    //    if(SVGA15KHzdriver)
                    //        err = set_gfx_mode(mode,gfx_width,gfx_height,SVGA15KHzdriver.getlogicalwidth(gfx_width),0);
                    //    else
                    //        err = set_gfx_mode(mode,gfx_width,gfx_height,0,0);
                    //}

                    //printf ("Trying ");
                    //    if      (mode == GFX_VESA1)
                    //        printf ( "VESA1");
                    //    else if (mode == GFX_VESA2B)
                    //        printf ( "VESA2B");
                    //    else if (mode == GFX_VESA2L)
                    //        printf ( "VESA2L");
                    //    else if (mode == GFX_VESA3)
                    //        fprintf ( "VESA3");
                    //    printf ( "  %dx%d, %d bit\n",
                    //            gfx_width, gfx_height, bits);
                    //}

                    //if (err == 0)
                    //{
                    //    found = 1;
                    //    /* replace gfx_mode with found mode */
                    //    gfx_mode = mode;
                    //    continue;
                    //}
                    //else if (errorlog)
                    //    fprintf (errorlog,"%s\n",allegro_error);

                    /* Now adjust parameters for the next loop */

                    /* try 5-5-5 in case there is no 5-6-5 16 bit color mode */
                    if (scrbitmap.depth == 16)
                    {
                        if (bits == 16)
                        {
                            bits = 15;
                            continue;
                        }
                        else
                            bits = 16; /* reset to 5-6-5 */
                    }

                    /* try VESA modes in VESA3-VESA2L-VESA2B-VESA1 order */

                    //if (mode == GFX_VESA3)
                    //{
                    //    mode = GFX_VESA2L;
                    //    continue;
                    //}
                    //else if (mode == GFX_VESA2L)
                    //{
                    //    mode = GFX_VESA2B;
                    //    continue;
                    //}
                    //else if (mode == GFX_VESA2B)
                    //{
                    //    mode = GFX_VESA1;
                    //    continue;
                    //}
                    //else if (mode == GFX_VESA1)
                    //    mode = gfx_mode; /* restart with the mode given in mame.cfg */

                    /* try higher resolutions */
                    if (auto_resolution != 0)
                    {
                        if (stretch != 0 && gfx_width <= 512)
                        {
                            /* low res VESA mode not available, try an high res one */
                            gfx_width *= 2;
                            gfx_height *= 2;
                            continue;
                        }

                        /* try next higher resolution */
                        if (gfx_height < 300 && gfx_width < 400)
                        {
                            gfx_width = 400;
                            gfx_height = 300;
                            continue;
                        }
                        else if (gfx_height < 384 && gfx_width < 512)
                        {
                            gfx_width = 512;
                            gfx_height = 384;
                            continue;
                        }
                        else if (gfx_height < 480 && gfx_width < 640)
                        {
                            gfx_width = 640;
                            gfx_height = 480;
                            continue;
                        }
                        else if (gfx_height < 600 && gfx_width < 800)
                        {
                            gfx_width = 800;
                            gfx_height = 600;
                            continue;
                        }
                        else if (gfx_height < 768 && gfx_width < 1024)
                        {
                            gfx_width = 1024;
                            gfx_height = 768;
                            continue;
                        }
                    }

                    /* If there was no continue up to this point, we give up */
                    break;
                }

                if (found == 0)
                {
                    printf("\nNo %d-bit %dx%d VESA mode available.\n",
                            scrbitmap.depth, gfx_width, gfx_height);
                    printf("\nPossible causes:\n" +
        "1) Your video card does not support VESA modes at all. Almost all\n" +
        "   video cards support VESA modes natively these days, so you probably\n" +
        "   have an older card which needs some driver loaded first.\n" +
        "   In case you can't find such a driver in the software that came with\n" +
        "   your video card, Scitech Display Doctor or (for S3 cards) S3VBE\n" +
        "   are good alternatives.\n" +
        "2) Your VESA implementation does not support this resolution. For example,\n" +
        "   '-320x240', '-400x300' and '-512x384' are only supported by a few\n" +
        "   implementations.\n" +
        "3) Your video card doesn't support this resolution at this color depth.\n" +
        "   For example, 1024x768 in 16 bit colors requires 2MB video memory.\n" +
        "   You can either force an 8 bit video mode ('-depth 8') or use a lower\n" +
        "   resolution ('-640x480', '-800x600').\n");
                    return 0;
                }

            }

            vsync_frame_rate = (int)Machine.drv.frames_per_second;

            warming_up = true;

            return 1;
        }
        /* center image inside the display based on the visual area */
        static void adjust_display(int xmin, int ymin, int xmax, int ymax, int depth)
        {
            int temp;
            int w, h;
            int act_width;

            /* if it's a SVGA arcade monitor mode, get the memory width of the mode */
            /* this could be double the width of the actual mode set */

            act_width = gfx_width;


            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                temp = xmin; xmin = ymin; ymin = temp;
                temp = xmax; xmax = ymax; ymax = temp;
                w = Machine.drv.screen_height;
                h = Machine.drv.screen_width;
            }
            else
            {
                w = Machine.drv.screen_width;
                h = Machine.drv.screen_height;
            }

            if (!vector_game)
            {
                if ((Machine.orientation & ORIENTATION_FLIP_X) != 0)
                {
                    temp = w - xmin - 1;
                    xmin = w - xmax - 1;
                    xmax = temp;
                }
                if ((Machine.orientation & ORIENTATION_FLIP_Y) != 0)
                {
                    temp = h - ymin - 1;
                    ymin = h - ymax - 1;
                    ymax = temp;
                }
            }

            viswidth = xmax - xmin + 1;
            visheight = ymax - ymin + 1;


            /* setup xmultiply to handle SVGA driver's (possible) double width */
            xmultiply = act_width / gfx_width;
            ymultiply = 1;

            if (use_vesa != 0 && !vector_game)
            {
                if (stretch != 0)
                {
                    if ((Machine.orientation & ORIENTATION_SWAP_XY) == 0 &&
                            (Machine.drv.video_attributes & VIDEO_DUAL_MONITOR) == 0)
                    {
                        /* horizontal, non dual monitor games may be stretched at will */
                        while ((xmultiply + 1) * viswidth <= act_width)
                            xmultiply++;
                        while ((ymultiply + 1) * visheight <= gfx_height)
                            ymultiply++;
                    }
                    else
                    {
                        int tw, th;

                        tw = act_width;
                        th = gfx_height;

                        if ((Machine.drv.video_attributes & VIDEO_PIXEL_ASPECT_RATIO_MASK)
                                == VIDEO_PIXEL_ASPECT_RATIO_1_2)
                        {
                            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                                tw /= 2;
                            else th /= 2;
                        }

                        /* Hack for 320x480 and 400x600 "vmame" video modes */
                        if ((gfx_width == 320 && gfx_height == 480) ||
                                (gfx_width == 400 && gfx_height == 600))
                            th /= 2;

                        /* maintain aspect ratio for other games */
                        while ((xmultiply + 1) * viswidth <= tw &&
                                (ymultiply + 1) * visheight <= th)
                        {
                            xmultiply++;
                            ymultiply++;
                        }

                        if ((Machine.drv.video_attributes & VIDEO_PIXEL_ASPECT_RATIO_MASK)
                                == VIDEO_PIXEL_ASPECT_RATIO_1_2)
                        {
                            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                                xmultiply *= 2;
                            else ymultiply *= 2;
                        }

                        /* Hack for 320x480 and 400x600 "vmame" video modes */
                        if ((gfx_width == 320 && gfx_height == 480) ||
                                (gfx_width == 400 && gfx_height == 600))
                            ymultiply *= 2;
                    }
                }
                else
                {
                    if ((Machine.drv.video_attributes & VIDEO_PIXEL_ASPECT_RATIO_MASK)
                            == VIDEO_PIXEL_ASPECT_RATIO_1_2)
                    {
                        if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                            xmultiply *= 2;
                        else ymultiply *= 2;
                    }

                    /* Hack for 320x480 and 400x600 "vmame" video modes */
                    if ((gfx_width == 320 && gfx_height == 480) ||
                            (gfx_width == 400 && gfx_height == 600))
                        ymultiply *= 2;
                }
            }

            if (depth == 16)
            {
                if (xmultiply > MAX_X_MULTIPLY16) xmultiply = MAX_X_MULTIPLY16;
                if (ymultiply > MAX_Y_MULTIPLY16) ymultiply = MAX_Y_MULTIPLY16;
            }
            else
            {
                if (xmultiply > MAX_X_MULTIPLY) xmultiply = MAX_X_MULTIPLY;
                if (ymultiply > MAX_Y_MULTIPLY) ymultiply = MAX_Y_MULTIPLY;
            }

            gfx_display_lines = visheight;
            gfx_display_columns = viswidth;

            gfx_xoffset = (act_width - viswidth * xmultiply) / 2;
            if (gfx_display_columns > act_width / xmultiply)
                gfx_display_columns = act_width / xmultiply;

            gfx_yoffset = (gfx_height - visheight * ymultiply) / 2;
            if (gfx_display_lines > gfx_height / ymultiply)
                gfx_display_lines = gfx_height / ymultiply;


            skiplinesmin = ymin;
            skiplinesmax = visheight - gfx_display_lines + ymin;
            skipcolumnsmin = xmin;
            skipcolumnsmax = viswidth - gfx_display_columns + xmin;

            /* Align on a quadword !*/
            gfx_xoffset &= ~7;

            /* the skipcolumns from mame.cfg/cmdline is relative to the visible area */
            skipcolumns = xmin + skipcolumns;
            skiplines = ymin + skiplines;

            /* Just in case the visual area doesn't fit */
            if (gfx_xoffset < 0)
            {
                skipcolumns -= gfx_xoffset;
                gfx_xoffset = 0;
            }
            if (gfx_yoffset < 0)
            {
                skiplines -= gfx_yoffset;
                gfx_yoffset = 0;
            }

            /* Failsafe against silly parameters */
            if (skiplines < skiplinesmin)
                skiplines = skiplinesmin;
            if (skipcolumns < skipcolumnsmin)
                skipcolumns = skipcolumnsmin;
            if (skiplines > skiplinesmax)
                skiplines = skiplinesmax;
            if (skipcolumns > skipcolumnsmax)
                skipcolumns = skipcolumnsmax;

            //printf("gfx_width = %d gfx_height = %d\n" +
            //            "gfx_xoffset = %d gfx_yoffset = %d\n" +
            //            "xmin %d ymin %d xmax %d ymax %d\n" +
            //            "skiplines %d skipcolumns %d\n" +
            //            "gfx_display_lines %d gfx_display_columns %d\n" +
            //            "xmultiply %d ymultiply %d\n",
            //            gfx_width, gfx_height,
            //            gfx_xoffset, gfx_yoffset,
            //            xmin, ymin, xmax, ymax, skiplines, skipcolumns, gfx_display_lines, gfx_display_columns, xmultiply, ymultiply);

            set_ui_visarea(skipcolumns, skiplines, skipcolumns + gfx_display_columns - 1, skiplines + gfx_display_lines - 1);

            /* round to a multiple of 4 to avoid missing pixels on the right side */
            //gfx_display_columns = (gfx_display_columns + 3) & ~3;
        }
        static void init_dirty(byte dirty)
        {
            memset(dirty_new, dirty, MAX_GFX_WIDTH / 16 * MAX_GFX_HEIGHT / 16);
        }
        const int safety = 16;
        public static void osd_free_bitmap(osd_bitmap bitmap)
        {
            bitmap.line = null;
            bitmap._private = null;
        }
        public static osd_bitmap osd_new_bitmap(int width, int height, int depth)       /* ASG 980209 */
        {
            osd_bitmap bitmap;
            //throw new Exception();
            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                int temp;

                temp = width;
                width = height;
                height = temp;
            }

            if ((bitmap = new osd_bitmap()) != null)
            {
                int i, rowlen, rdwidth;
                _BytePtr bm;

                if (depth != 8 && depth != 16) depth = 8;

                bitmap.depth = depth;
                bitmap.width = width;
                bitmap.height = height;

                rdwidth = (width + 7) & ~7;     /* round width to a quadword */
                if (depth == 16)
                    rowlen = 2 * (rdwidth + 2 * safety);
                else
                    rowlen = (rdwidth + 2 * safety);

                if ((bm = new _BytePtr((height+2*safety) * rowlen)) == null)
                {
                    bitmap = null;
                    return null;
                }

                /* clear ALL bitmap, including safety area, to avoid garbage on right */
                /* side of screen is width is not a multiple of 4 */
                memset(bm, 0, (height + 2 * safety) * rowlen);
                //Array.Clear(bm.buffer, bm.offset, (height + 2 * safety) * rowlen);
                if ((bitmap.line = new _BytePtr[((height + 2 * safety))]) == null)
                {
                    bm = null;
                    bitmap = null;
                    return null;
                }

                for (i = 0; i < height + 2 * safety; i++)
                {
                    if (depth == 16)
                        bitmap.line[i] = new _BytePtr(bm, (i * rowlen + 2 * safety));
                    else
                        bitmap.line[i] = new _BytePtr(bm, (i * rowlen + safety));
                    bitmap.line[i].offset += safety;
                }
                //bitmap.line.offset += safety;

                bitmap._private = bm;

                osd_clearbitmap(bitmap);
            }

            return bitmap;
        }
        public static void osd_clearbitmap(osd_bitmap bitmap)
        {
            int i;


            for (i = 0; i < bitmap.height; i++)
            {
                if (bitmap.depth == 16)
                    memset(bitmap.line[i], 0, 2 * bitmap.width);
                else
                    memset(bitmap.line[i], BACKGROUND, bitmap.width);
            }


            if (bitmap == scrbitmap)
            {
                osd_mark_dirty(0, 0, bitmap.width - 1, bitmap.height - 1, 1);
                bitmap_dirty = 1;
            }
        }

        static void set_color_depth(int bits)
        {
            //throw new Exception();
        }
        //static update_screen_delegate[][][][] updaters8;
        //static update_screen_delegate[][][][] updaters16;
        //static update_screen_delegate[][][][] updaters16_palettized;

        static ushort makecol(byte r, byte g, byte b)
        {
            uint v = new Color(r, g, b).PackedValue;
            //Red shift from 24 to 16, masking but 5 MSBs 
            ushort o = (ushort)((v >> 8) & 0xf800);

            /* Green shift from 16 to 11, masking 6 MSBs */
            o |= (ushort)((v >> 5) & 0x07e0);

            /* Blue shift from 8 to 5, masking 5 MSBs */
            o |= (ushort)((v >> 3) & 0x001f);

            return o;
        }
        static void osd_close_display()
        {
            //nothing throw new Exception();
        }

        static int[][] skiptable =
	{
		new int[]{ 0,0,0,0,0,0,0,0,0,0,0,0 },
		new int[]{ 0,0,0,0,0,0,0,0,0,0,0,1 },
		new int[]{ 0,0,0,0,0,1,0,0,0,0,0,1 },
		new int[]{ 0,0,0,1,0,0,0,1,0,0,0,1 },
		new int[]{ 0,0,1,0,0,1,0,0,1,0,0,1 },
		new int[]{ 0,1,0,0,1,0,1,0,0,1,0,1 },
		new int[]{ 0,1,0,1,0,1,0,1,0,1,0,1 },
		new int[]{ 0,1,0,1,1,0,1,0,1,1,0,1 },
		new int[]{ 0,1,1,0,1,1,0,1,1,0,1,1 },
		new int[]{ 0,1,1,1,0,1,1,1,0,1,1,1 },
		new int[]{ 0,1,1,1,1,1,0,1,1,1,1,1 },
		new int[]{ 0,1,1,1,1,1,1,1,1,1,1,1 }
	};
        public static int osd_skip_this_frame()
        {
            return skiptable[frameskip][frameskip_counter];
        }

        static int[][] waittable =
	{
		new int[]{ 1,1,1,1,1,1,1,1,1,1,1,1 },
		new int[]{ 2,1,1,1,1,1,1,1,1,1,1,0 },
		new int[]{ 2,1,1,1,1,0,2,1,1,1,1,0 },
		new int[]{ 2,1,1,0,2,1,1,0,2,1,1,0 },
		new int[]{ 2,1,0,2,1,0,2,1,0,2,1,0 },
		new int[]{ 2,0,2,1,0,2,0,2,1,0,2,0 },
		new int[]{ 2,0,2,0,2,0,2,0,2,0,2,0 },
		new int[]{ 2,0,2,0,0,3,0,2,0,0,3,0 },
		new int[]{ 3,0,0,3,0,0,3,0,0,3,0,0 },
		new int[]{ 4,0,0,0,4,0,0,0,4,0,0,0 },
		new int[]{ 6,0,0,0,0,0,6,0,0,0,0,0 },
		new int[]{12,0,0,0,0,0,0,0,0,0,0,0 }
	};
        static int showfps, showfpstemp;
        static long prev_measure, this_frame_base, prev;
        static int speed = 100;
        static int vups, vfcount;
        static long last1, last2;
        static int frameskipadjust;

        long ticksPerFrame, ticksSinceLastFrame;
        void osd_update_video_and_audio()
        {
            long curr;
            bool need_to_clear_bitmap = false;
            bool already_synced;

            if (warming_up)
            {
                /* first time through, initialize timer */
                prev_measure = (ticker() - (long)(FRAMESKIP_LEVELS * TICKS_PER_SEC / Machine.drv.frames_per_second));
                warming_up = false;
                ticksPerFrame = (long)(TICKS_PER_SEC / Machine.drv.frames_per_second);
            }

            if (frameskip_counter == 0)
                this_frame_base = (prev_measure + (long)(FRAMESKIP_LEVELS * TICKS_PER_SEC / Machine.drv.frames_per_second));

            if (throttle)
            {
                /* if too much time has passed since last sound update, disable throttling */
                /* temporarily - we wouldn't be able to keep synch anyway. */
                curr = ticker();
                if ((curr - last1) > 2 * TICKS_PER_SEC / Machine.drv.frames_per_second)
                    throttle = false;
                last1 = curr;

                already_synced = xna_update_audio();

                throttle = true;
            }
            else
                already_synced = xna_update_audio();

            if (osd_skip_this_frame() == 0)
            {
                if (showfpstemp != 0)
                {
                    showfpstemp--;
                    if (showfps == 0 && showfpstemp == 0)
                    {
                        need_to_clear_bitmap = true;
                    }
                }

                if (input_ui_pressed((int)ports.inptports.IPT_UI_SHOW_FPS))
                {
                    if (showfpstemp != 0)
                    {
                        showfpstemp = 0;
                        need_to_clear_bitmap = true;
                    }
                    else
                    {
                        showfps ^= 1;
                        if (showfps == 0)
                        {
                            need_to_clear_bitmap = true;
                        }
                    }
                }

                /* now wait until it's time to update the screen */
                //while ((ticker() - ticksSinceLastFrame) < ticksPerFrame)System.Threading.Thread.Sleep(0);
                if (throttle)
                {
                    if (video_sync != 0)
                    {
                        do
                        {
                            //vsync();
                            curr = ticker();
                        } while ((TICKS_PER_SEC / (curr - last2)) > Machine.drv.frames_per_second * 11 / 10);

                        last2 = curr;
                    }
                    else
                    {
                        /* wait for video sync but use normal throttling */
                        //                        if (wait_vsync != 0)
                        //vsync();
                        while ((ticker() - ticksSinceLastFrame) < ticksPerFrame)
                            ;
                        curr = ticker();

                        if (!already_synced)
                        {
                            /* wait only if the audio update hasn't synced us already */

                            long target = this_frame_base + (long)(frameskip_counter * TICKS_PER_SEC / Machine.drv.frames_per_second);

                            if (curr - target < 0)
                            {
                                do
                                {
                                    curr = ticker();
                                } while (curr - target < 0);
                            }
                        }
                    }
                }
                else curr = ticker();

                /* for the FPS average calculation */
                if (++frames_displayed == FRAMES_TO_SKIP)
                    start_time = curr;
                else
                    end_time = curr;

                if (frameskip_counter == 0)
                {
                    int divdr = (int)(Machine.drv.frames_per_second * (curr - prev_measure) / (100 * FRAMESKIP_LEVELS));
                    speed = (int)((TICKS_PER_SEC + divdr / 2) / divdr);

                    prev_measure = curr;
                }

                prev = curr;

                vfcount += waittable[frameskip][frameskip_counter];
                if (vfcount >= Machine.drv.frames_per_second)
                {
                    vfcount = 0;
                    vups = AvgDvg.vector_updates;
                    AvgDvg.vector_updates = 0;
                }

                if (showfps != 0 || showfpstemp != 0)
                {
                    int divdr = 100 * FRAMESKIP_LEVELS;
                    int fps = (int)(Machine.drv.frames_per_second * (FRAMESKIP_LEVELS - frameskip) * speed + (divdr / 2)) / divdr;
                    string buf = sprintf("%s%2d%4d%%%4d/%d fps", autoframeskip ? "auto" : "fskp", frameskip, speed, fps, (int)(Machine.drv.frames_per_second + 0.5));
                    ui_text(buf, Machine.uiwidth - (buf.Length) * Machine.uifontwidth, 0);
                    if (vector_game)
                    {
                        buf += sprintf(" %d vector updates", vups);
                        ui_text(buf, Machine.uiwidth - (buf.Length) * Machine.uifontwidth, Machine.uifontheight);
                    }
                }

                if (scrbitmap.depth == 8)
                {
                    if (dirty_bright != 0)
                    {
                        dirty_bright = 0;
                        for (int i = 0; i < 256; i++)
                        {
                            float rate = (float)(brightness * brightness_paused_adjust * Math.Pow(i / 255.0, 1 / osd_gamma_correction) / 100);
                            bright_lookup[i] = (int)(255 * rate + 0.5);
                        }
                    }
                    if (dirtypalette)
                    {
                        dirtypalette = false;
                        for (int i = 0; i < screen_colors; i++)
                        {
                            if (dirtycolor[i])
                            {
                                RGB adjusted_palette;

                                dirtycolor[i] = false;

                                adjusted_palette.r = (byte)(current_palette[3 * i + 0]);// << 3);
                                adjusted_palette.g = (byte)(current_palette[3 * i + 1]);// << 3);
                                adjusted_palette.b = (byte)(current_palette[3 * i + 2]);// << 3);
                                if (i != Machine.uifont.colortable[1])	/* don't adjust the user interface text */
                                {
                                    adjusted_palette.r = (byte)bright_lookup[adjusted_palette.r];
                                    adjusted_palette.g = (byte)bright_lookup[adjusted_palette.g];
                                    adjusted_palette.b = (byte)bright_lookup[adjusted_palette.b];
                                }
                                else
                                {
                                    
                                    //adjusted_palette.r >>= 2;
                                    //adjusted_palette.g >>= 2;
                                    //adjusted_palette.b >>= 2;
                                }
                                set_color(i, adjusted_palette);
                            }
                        }
                    }
                }
                else
                {
                    if (dirty_bright != 0)
                    {
                        dirty_bright = 0;
                        for (int i = 0; i < 256; i++)
                        {
                            float rate = (float)(brightness * brightness_paused_adjust * Math.Pow(i / 255.0, 1 / osd_gamma_correction) / 100);
                            bright_lookup[i] = (int)(255 * rate + 0.5);
                        }
                    }
                    if (dirtypalette)
                    {
                        if (use_dirty != 0) init_dirty(1);	/* have to redraw the whole screen */

                        dirtypalette = false;
                        for (int i = 0; i < screen_colors; i++)
                        {
                            if (dirtycolor[i])
                            {
                                byte r, g, b;

                                dirtycolor[i] = false;

                                r = current_palette[3 * i + 0];
                                g = current_palette[3 * i + 1];
                                b = current_palette[3 * i + 2];
                                if (i != Machine.uifont.colortable[1])	/* don't adjust the user interface text */
                                {
                                    r = (byte)bright_lookup[r];
                                    g = (byte)bright_lookup[g];
                                    b = (byte)bright_lookup[b];
                                }
                                palette_16bit_lookup[i] = (ushort)(makecol((byte)r, (byte)g, (byte)b));// * 0x10001);
                                set_color(i, r, g, b);
                            }
                        }
                    }
                }

                /* copy the bitmap to screen memory */
                //doupdate_screen();
                (blitHandle as AutoResetEvent).Set();

                if (need_to_clear_bitmap)
                    osd_clearbitmap(scrbitmap);

                if (use_dirty != 0)
                {
                    if (!vector_game)
                        swap_dirty();
                    init_dirty(0);
                }

                if (need_to_clear_bitmap)
                    osd_clearbitmap(scrbitmap);

                if (throttle && autoframeskip && frameskip_counter == 0)
                {
                    /* adjust speed to video refresh rate if vsync is on */
                    int adjspeed = (int)(speed * Machine.drv.frames_per_second / vsync_frame_rate);

                    if (adjspeed >= 100)
                    {
                        frameskipadjust++;
                        if (frameskipadjust >= 3)
                        {
                            frameskipadjust = 0;
                            if (frameskip > 0) frameskip--;
                        }
                    }
                    else
                    {
                        if (adjspeed < 80)
                            frameskipadjust -= (90 - adjspeed) / 5;
                        else
                        {
                            /* don't push frameskip too far if we are close to 100% speed */
                            if (frameskip < 8)
                                frameskipadjust--;
                        }

                        while (frameskipadjust <= -2)
                        {
                            frameskipadjust += 2;
                            if (frameskip < FRAMESKIP_LEVELS - 1) frameskip++;
                        }
                    }
                }
            }

            /* Check for PGUP, PGDN and pan screen */
            //xxxpan_display();

            if (input_ui_pressed((int)ports.inptports.IPT_UI_FRAMESKIP_INC))
            {
                if (autoframeskip)
                {
                    autoframeskip = false;
                    frameskip = 0;
                }
                else
                {
                    if (frameskip == FRAMESKIP_LEVELS - 1)
                    {
                        frameskip = 0;
                        autoframeskip = true;
                    }
                    else
                        frameskip++;
                }

                if (showfps == 0)
                    showfpstemp = (int)(2 * Machine.drv.frames_per_second);

                /* reset the frame counter every time the frameskip key is pressed, so */
                /* we'll measure the average FPS on a consistent status. */
                frames_displayed = 0;
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_FRAMESKIP_DEC))
            {
                if (autoframeskip)
                {
                    autoframeskip = false;
                    frameskip = FRAMESKIP_LEVELS - 1;
                }
                else
                {
                    if (frameskip == 0)
                        autoframeskip = true;
                    else
                        frameskip--;
                }

                if (showfps == 0)
                    showfpstemp = (int)(2 * Machine.drv.frames_per_second);

                /* reset the frame counter every time the frameskip key is pressed, so */
                /* we'll measure the average FPS on a consistent status. */
                frames_displayed = 0;
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_THROTTLE))
            {
                throttle ^= true;

                /* reset the frame counter every time the throttle key is pressed, so */
                /* we'll measure the average FPS on a consistent status. */
                frames_displayed = 0;
            }

            frameskip_counter = (frameskip_counter + 1) % FRAMESKIP_LEVELS;
            ticksSinceLastFrame = ticker();
        }
        static void select_display_mode(int depth)
        {
            int width, height;

            if (vector_game)
            {
                width = Machine.drv.screen_width;
                height = Machine.drv.screen_height;
            }
            else
            {
                width = Machine.drv.visible_area.max_x - Machine.drv.visible_area.min_x + 1;
                height = Machine.drv.visible_area.max_y - Machine.drv.visible_area.min_y + 1;
            }

            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
            {
                int temp;

                temp = width;
                width = height;
                height = temp;
            }
            /* If no VESA resolution has been given, we choose a sensible one. */
            /* 640x480, 800x600 and 1024x768 are common to all VESA drivers. */

            //gfx_width = Math.Max(Game1.graphics.PreferredBackBufferWidth, drv.screen_width);
            //gfx_height = Math.Max(Game1.graphics.PreferredBackBufferHeight, drv.screen_height);

            if (gfx_width == 0 && gfx_height == 0)
            {
                auto_resolution = 1;
                use_vesa = 1;

                /* vector games use 640x480 as default */
                if (vector_game)
                {
                    gfx_width = 640;
                    gfx_height = 480;
                }
                else
                {
                    int xm, ym;

                    xm = ym = 1;

                    if ((Machine.drv.video_attributes & VIDEO_PIXEL_ASPECT_RATIO_MASK)
                            == VIDEO_PIXEL_ASPECT_RATIO_1_2)
                    {
                        if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                            xm++;
                        else ym++;
                    }

                    if (scanlines != 0 && stretch != 0)
                    {
                        if (ym == 1)
                        {
                            xm *= 2;
                            ym *= 2;
                        }

                        /* see if pixel doubling can be applied at 640x480 */
                        if (ym * height <= 480 && xm * width <= 640 &&
                                (xm > 1 || (ym + 1) * height > 768 || (xm + 1) * width > 1024))
                        {
                            gfx_width = 640;
                            gfx_height = 480;
                        }
                        /* see if pixel doubling can be applied at 800x600 */
                        else if (ym * height <= 600 && xm * width <= 800 &&
                                (xm > 1 || (ym + 1) * height > 768 || (xm + 1) * width > 1024))
                        {
                            gfx_width = 800;
                            gfx_height = 600;
                        }
                        /* don't use 1024x768 right away. If 512x384 is available, it */
                        /* will provide hardware scanlines. */

                        if (ym > 1 && xm > 1)
                        {
                            xm /= 2;
                            ym /= 2;
                        }
                    }

                    if (gfx_width == 0 && gfx_height == 0)
                    {
                        if (ym * height <= 240 && xm * width <= 320)
                        {
                            gfx_width = 320;
                            gfx_height = 240;
                        }
                        else if (ym * height <= 300 && xm * width <= 400)
                        {
                            gfx_width = 400;
                            gfx_height = 300;
                        }
                        else if (ym * height <= 384 && xm * width <= 512)
                        {
                            gfx_width = 512;
                            gfx_height = 384;
                        }
                        else if (ym * height <= 480 && xm * width <= 640 &&
                                (stretch == 0 || (ym + 1) * height > 768 || (xm + 1) * width > 1024))
                        {
                            gfx_width = 640;
                            gfx_height = 480;
                        }
                        else if (ym * height <= 600 && xm * width <= 800 &&
                                (stretch == 0 || (ym + 1) * height > 768 || (xm + 1) * width > 1024))
                        {
                            gfx_width = 800;
                            gfx_height = 600;
                        }
                        else
                        {
                            gfx_width = 1024;
                            gfx_height = 768;
                        }
                    }

                }
            }
        }
        static void swap_dirty()
        {
        }
        void osd_save_snapshot()
        {
            throw new Exception();
        }
        void osd_pause(bool paused)
        {
            if (paused) brightness_paused_adjust = 0.65f;
            else brightness_paused_adjust = 1.0f;

            for (int i = 0; i < screen_colors; i++)
                dirtycolor[i] = true;
            dirtypalette = true;
            dirty_bright = 1;
        }
        static void osd_modify_pen(int pen, byte red, byte green, byte blue)
        {
            if (modifiable_palette == 0)
            {
                printf("error: osd_modify_pen() called with modifiable_palette == 0\n");
                return;
            }


            if (current_palette[3 * pen + 0] != red ||
                    current_palette[3 * pen + 1] != green ||
                    current_palette[3 * pen + 2] != blue)
            {
                current_palette[3 * pen + 0] = red;
                current_palette[3 * pen + 1] = green;
                current_palette[3 * pen + 2] = blue;

                dirtycolor[pen] = true;
                dirtypalette = true;
            }
        }
        public static byte getr(int pen)
        {
            throw new Exception();
        }
        public static byte getb(int pen)
        {
            throw new Exception();
        }
        public static byte getg(int pen)
        {
            throw new Exception();
        }
        public static void osd_get_pen(int pen, ref byte red, ref byte green, ref byte blue)
        {
            if (scrbitmap.depth != 8 && modifiable_palette == 0)
            {
                red = getr(pen);
                green = getg(pen);
                blue = getb(pen);
            }
            else
            {
                red = current_palette[3 * pen];
                green = current_palette[3 * pen + 1];
                blue = current_palette[3 * pen + 2];
            }
        }
    }
}
