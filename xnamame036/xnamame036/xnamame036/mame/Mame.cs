using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    public partial class Mame
    {
        public const int MAX_GFX_ELEMENTS = 32;
        public const int MAX_MEMORY_REGIONS = 32;

        public class RunningMachine
        {
            public _BytePtr[] memory_region = new _BytePtr[MAX_MEMORY_REGIONS];
            public uint[] memory_region_length = new uint[MAX_MEMORY_REGIONS];	/* some drivers might find this useful */
            public int[] memory_region_type = new int[MAX_MEMORY_REGIONS];
            public GfxElement[] gfx = new GfxElement[MAX_GFX_ELEMENTS];	/* graphic sets (chars, sprites) */
            public osd_bitmap scrbitmap;	/* bitmap to draw into */
            public _ShortPtr pens;	/* remapped palette pen numbers. When you write */
            /* directly to a bitmap, never use absolute values, */
            /* use this array to get the pen number. For example, */
            /* if you want to use color #6 in the palette, use */
            /* pens[6] instead of just 6. */
            public _ShortPtr game_colortable;	/* lookup table used to map gfx pen numbers */
            /* to color numbers */
            public _ShortPtr remapped_colortable;	/* the above, already remapped through */
            /* Machine.pens */
            public GameDriver gamedrv;	/* contains the definition of the game machine */
            public MachineDriver drv;	/* same as gamedrv.drv */
            public int color_depth;	/* video color depth: 8 or 16 */
            public int sample_rate;	/* the digital audio sample rate; 0 if sound is disabled. */
            /* This is set to a default value, or a value specified by */
            /* the user; osd_init() is allowed to change it to the actual */
            /* sample rate supported by the audio card. */
            public int obsolete;	// was sample_bits;	/* 8 or 16 */
            public GameSamples samples;	/* samples loaded from disk */
            public InputPort[] input_ports;	/* the input ports definition from the driver */
            /* is copied here and modified (load settings from disk, */
            /* remove cheat commands, and so on) */
            public InputPort[] input_ports_default; /* original input_ports without modifications */
            public int orientation;	/* see #defines in driver.h */
            public GfxElement uifont;	/* font used by DisplayText() */
            public int uifontwidth, uifontheight;
            public int uixmin, uiymin;
            public int uiwidth, uiheight;
            public int ui_orientation;
        }
        public class ImageFile
        {
            public string name;
            public int type;
        }
        public class GameOptions
        {
            public int mame_debug;
            public bool cheat;
            public int gui_host;

            public int samplerate = 44100;
            public bool use_samples;
            public bool use_emulated_ym3812;

            public int color_depth = 8;	/* 8 or 16, any other value means auto */
            public bool norotate;
            public bool ror;
            public bool rol;
            public bool flipx;
            public bool flipy;
            public int beam;
            public int flicker;
            public int translucency;
            public int antialias;
            public bool use_artwork;
            public bool crc_only;

        }
        static RunningMachine machine = new RunningMachine();
        public static RunningMachine Machine = machine;
        static GameDriver gamedrv;
        static MachineDriver drv;

        public static GameOptions options = new GameOptions();

        int mame_debug;
        int bailing;
        int settingsloaded;
        static int bitmap_dirty;
        int ignorecfg;
        bool need_to_clear_bitmap;

        public void Run()
        {
            //These driver seems to work OK
           // main("pacman");               
            //main("invaders");
            //main("1942");
            //main("1943"); 
           // main("zaxxon");
            //main("szaxxon");
            //main("galaga");
            //main("digdug");
            //main("dkong");
            //main("mario");//Seems to work, graphics problems
            //main("bombjack"); 
            //main("amidar");
            //main("phoenix");
            //main("locomotn");
            //main("tp84");
            //main("phozon"); 
            //main("mappy"); 
            //main("gunfight");
            ////main("boothill");
            //main("arkanoid");
            //main("frogger"); 
            //main("gunsmoke"); 
            //main("qix"); 
            //main("mpatrol");  // sound not implemented yet
            //main("pbaction");
            //main("citycon");
            //main("jackal");
            //main("speedbal"); 
            //main("gyruss"); 
            //main("rocnrope");
            //main("galaxian");// sound from samples not good
            //main("ladybug"); 




            //Here are some vector games
            //main("zektor");
            //main("tacscan");
            //main("asteroid");
            //main("llander");//artwork/overlay not implemented
            //main("speedfrk"); //ccpu (vector game)

            //-------------------------------------------------
            //These drivers have problems that should be fixable
            
             
            //main("pang"); // needs sound. graphics glitches

            //main("centipede"); // Screen layout not correct. graphics or cpu related ?

            //main("defender"); //graphics/palette problems



            //-------------------------------------------------
            //These drivers I would like to implement

            main("ldrun");

            //main("galaga88");   //6809,hd63701                    
            //main("marble");//m68010,m6502
            
            
            //main("ajax"); // does not work. tilemap implementation problems, or konami chip
            //main("kangaroo"); //z80
            //main("irobot");//6809
            
            //main("gtmr"); // 68000 
            //main("gtmre"); // 68000
            //main("qbert"); // I8086,M6502
            //main("outrun"); // 680000, Z80


            //-------------------------------------------------
            //These drivers have major problems
            
           //main("airwolf"); // stops/hangs during init of system (displays status screen though),submem check
            //main("srdmissn"); // same problems as airwolf, driver related ? memory handling related ?
            //main("raiden");  //cpu problems ?
            
        }
        public void main(string game)
        {
            int res;
            InitGameDriverList();
            init_ticker();
            ignorecfg = 0;

            if (soundcard == 0)
            {    /* silence, this would be -1 if unknown in which case all roms are loaded */
                Machine.sample_rate = 0; /* update the Machine structure to show that sound is disabled */
                options.samplerate = 0;
            }
            res = run_game(game);
        }
        int run_game(string game)
        {
            int err;

            Machine.gamedrv = gamedrv = GetDriver(game);
            Machine.drv = drv = gamedrv.drv;

            /* copy configuration */
            if (options.color_depth == 16 ||
                    (options.color_depth != 8 && (Machine.gamedrv.flags & GAME_REQUIRES_16BIT) != 0))
                Machine.color_depth = 16;
            else
                Machine.color_depth = 8;
            Machine.sample_rate = options.samplerate;

            /* get orientation right */
            Machine.orientation = (int)(gamedrv.flags & ORIENTATION_MASK);
            Machine.ui_orientation = ROT0;
            if (options.norotate)
                Machine.orientation = ROT0;
            if (options.ror)
            {
                /* if only one of the components is inverted, switch them */
                if ((Machine.orientation & ROT180) == ORIENTATION_FLIP_X ||
                        (Machine.orientation & ROT180) == ORIENTATION_FLIP_Y)
                    Machine.orientation ^= ROT180;

                Machine.orientation ^= ROT90;

                /* if only one of the components is inverted, switch them */
                if ((Machine.ui_orientation & ROT180) == ORIENTATION_FLIP_X ||
                        (Machine.ui_orientation & ROT180) == ORIENTATION_FLIP_Y)
                    Machine.ui_orientation ^= ROT180;

                Machine.ui_orientation ^= ROT90;
            }
            if (options.rol)
            {
                /* if only one of the components is inverted, switch them */
                if ((Machine.orientation & ROT180) == ORIENTATION_FLIP_X ||
                        (Machine.orientation & ROT180) == ORIENTATION_FLIP_Y)
                    Machine.orientation ^= ROT180;

                Machine.orientation ^= ROT270;

                /* if only one of the components is inverted, switch them */
                if ((Machine.ui_orientation & ROT180) == ORIENTATION_FLIP_X ||
                        (Machine.ui_orientation & ROT180) == ORIENTATION_FLIP_Y)
                    Machine.ui_orientation ^= ROT180;

                Machine.ui_orientation ^= ROT270;
            }
            if (options.flipx)
            {
                Machine.orientation ^= ORIENTATION_FLIP_X;
                Machine.ui_orientation ^= ORIENTATION_FLIP_X;
            }
            if (options.flipy)
            {
                Machine.orientation ^= ORIENTATION_FLIP_Y;
                Machine.ui_orientation ^= ORIENTATION_FLIP_Y;
            }
            set_pixel_functions();

            /* Do the work*/
            err = 1;
            bailing = 0;
            if (osd_init() == 0)
            {
                if (init_machine() == 0)
                {
                    if (run_machine() == 0)
                        err = 0;
                    else if (bailing == 0)
                    {
                        bailing = 1;
                        printf("Unable to start machine emulation\n");
                    }

                    shutdown_machine();
                }
                else if (bailing == 0)
                {
                    bailing = 1;
                    printf("Unable to initialize machine emulation\n");
                }

                osd_exit();
            }
            else if (bailing == 0)
            {
                bailing = 1;
                printf("Unable to initialize system\n");
            }

            return err;
        }
        int init_machine()
        {
            int i;

            if (code_init() != 0)
                goto _out;

            for (i = 0; i < MAX_MEMORY_REGIONS; i++)
            {
                Machine.memory_region[i] = null;
                Machine.memory_region_length[i] = 0;
                Machine.memory_region_type[i] = 0;
            }

            if (gamedrv.input_ports != null)
            {
                Machine.input_ports = input_port_allocate(gamedrv.input_ports);
                if (Machine.input_ports == null)
                    goto out_code;
                Machine.input_ports_default = input_port_allocate(gamedrv.input_ports);
                if (Machine.input_ports_default == null)
                {
                    input_port_free(Machine.input_ports);
                    Machine.input_ports = null;
                    goto out_code;
                }
            }

            if (readroms() != 0)
                goto out_free;

            /* Mish:  Multi-session safety - set spriteram size to zero before memory map is set up */
            Generic.spriteram_size[0] = Generic.spriteram_2_size[0] = 0;

            /* first of all initialize the memory handlers, which could be used by the */
            /* other initialization routines */
            cpu_init();

            /* load input ports settings (keys, dip switches, and so on) */
            settingsloaded = load_input_port_settings();

            if (memory_init() == 0)
                goto out_free;

            gamedrv.driver_init();

            return 0;

        out_free:
            input_port_free(Machine.input_ports);
            Machine.input_ports = null;
            input_port_free(Machine.input_ports_default);
            Machine.input_ports_default = null;
        out_code:
            code_close();
        _out:
            return 1;
        }
        void code_close()
        {
            code_mac = 0;
            code_map = null;
        }
        void shutdown_machine()
        {
            for (int i = 0; i < MAX_MEMORY_REGIONS; i++)
            {
                Machine.memory_region[i] = null;
                Machine.memory_region_length[i] = 0;
                Machine.memory_region_type[i] = 0;
            }
            input_port_free(Machine.input_ports);
            Machine.input_ports = null;
            input_port_free(Machine.input_ports_default);
            machine.input_ports_default = null;

            code_close();
        }
        int run_machine()
        {
            int res = 1;


            if (vh_open() == 0)
            {
                tilemap_init();
                SpriteManager.sprite_init();
                gfxobj_init();
                if (drv.vh_start() == 0)      /* start the video hardware */
                {
                    if (sound_start() == 0) /* start the audio hardware */
                    {
                        /* free memory regions allocated with REGIONFLAG_DISPOSE (typically gfx roms) */
                        for (int region = 0; region < MAX_MEMORY_REGIONS; region++)
                        {
                            if ((Machine.memory_region_type[region] & REGIONFLAG_DISPOSE) != 0)
                            {
                                /* invalidate contents to avoid subtle bugs */
                                for (int i = 0; i < memory_region_length(region); i++)
                                    memory_region(region)[i] = (byte)rand();
                                Machine.memory_region[region] = null;
                            }
                        }

                        if (settingsloaded == 0)
                        {
                            /* if there is no saved config, it must be first time we run this game, */
                            /* so show the disclaimer. */
                            if (showcopyright() != 0) goto userquit;
                        }

                        if (showgamewarnings() == 0)  /* show info about incorrect behaviour (wrong colors etc.) */
                        {
                            /* shut down the leds (work around Allegro hanging bug in the DOS port) */
                            osd_led_w(0, 1);
                            osd_led_w(1, 1);
                            osd_led_w(2, 1);
                            osd_led_w(3, 1);
                            osd_led_w(0, 0);
                            osd_led_w(1, 0);
                            osd_led_w(2, 0);
                            osd_led_w(3, 0);

                            init_user_interface();

                            /* disable cheat if no roms */
                            if (gamedrv.rom == null) options.cheat = false;

                            if (options.cheat) InitCheat();

                            if (drv.HasNVRAMhandler)
                            {
                                object f= osd_fopen(Machine.gamedrv.name, null, OSD_FILETYPE_NVRAM, 0);
                                drv.nvram_handler(f, 0);
                                if (f != null) osd_fclose(f);
                            }

                            cpu_run();      /* run the emulation! */

                            if (drv.HasNVRAMhandler)
                            {
                                object f;

                                if ((f = osd_fopen(Machine.gamedrv.name, null, OSD_FILETYPE_NVRAM, 1)) != null)
                                {
                                    drv.nvram_handler(f, 1);
                                    osd_fclose(f);
                                }
                            }

                            if (options.cheat) StopCheat();

                            /* save input ports settings */
                            save_input_port_settings();
                        }

                    userquit:
                        /* the following MUST be done after hiscore_save() otherwise */
                        /* some 68000 games will not work */
                        sound_stop();
                        drv.vh_stop();

                        res = 0;
                    }
                    else if (bailing == 0)
                    {
                        bailing = 1;
                        printf("Unable to start audio emulation\n");
                    }
                }
                else if (bailing == 0)
                {
                    bailing = 1;
                    printf("Unable to start video emulation\n");
                }

                gfxobj_close();
                SpriteManager.sprite_close();
                tilemap_close();
                vh_close();
            }
            else if (bailing == 0)
            {
                bailing = 1;
                printf("Unable to initialize display\n");
            }

            return res;
        }
        static bool mame_highscore_enabled()
        {
            /* disable high score when cheats are used */
            if (he_did_cheat != 0) return false;

            return true;
        }
        static int vh_open()
        {
            for (int i = 0; i < MAX_GFX_ELEMENTS; i++) Machine.gfx[i] = null;
            Machine.uifont = null;

            if (palette_start() != 0)
            {
                vh_close();
                return 1;
            }

            /* convert the gfx ROMs into character sets. This is done BEFORE calling the driver's */
            /* convert_color_prom() routine (in palette_init()) because it might need to check the */
            /* Machine.gfx[] data */
            if (drv.gfxdecodeinfo != null)
            {
                for (int i = 0; i < drv.gfxdecodeinfo.Length && i < MAX_GFX_ELEMENTS && drv.gfxdecodeinfo[i].memory_region != -1; i++)
                {
                    int reglen = 8 * memory_region_length(drv.gfxdecodeinfo[i].memory_region);
                    GfxLayout glcopy = new GfxLayout();

                    glcopy = drv.gfxdecodeinfo[i].gfxlayout;

                    if (IS_FRAC(glcopy.total))
                        glcopy.total = (uint)(reglen / glcopy.charincrement * FRAC_NUM(glcopy.total) / FRAC_DEN(glcopy.total));
                    for (int j = 0; j < glcopy.planeoffset.Length && j < MAX_GFX_PLANES; j++)
                    {
                        if (IS_FRAC(glcopy.planeoffset[j]))
                        {
                            glcopy.planeoffset[j] = (uint)(FRAC_OFFSET(glcopy.planeoffset[j]) + reglen * FRAC_NUM(glcopy.planeoffset[j]) / FRAC_DEN(glcopy.planeoffset[j]));
                        }
                    }
                    for (int j = 0; j < MAX_GFX_SIZE; j++)
                    {
                        if (j < glcopy.xoffset.Length && IS_FRAC(glcopy.xoffset[j]))
                        {
                            glcopy.xoffset[j] = (uint)(FRAC_OFFSET(glcopy.xoffset[j]) + reglen * FRAC_NUM(glcopy.xoffset[j]) / FRAC_DEN(glcopy.xoffset[j]));
                        }
                        if (j < glcopy.yoffset.Length && IS_FRAC(glcopy.yoffset[j]))
                        {
                            glcopy.yoffset[j] = (uint)(FRAC_OFFSET(glcopy.yoffset[j]) + reglen * FRAC_NUM(glcopy.yoffset[j]) / FRAC_DEN(glcopy.yoffset[j]));
                        }
                    }

                    if ((Machine.gfx[i] = decodegfx(new _BytePtr(memory_region(drv.gfxdecodeinfo[i].memory_region), drv.gfxdecodeinfo[i].start), glcopy)) == null)
                    {
                        vh_close();
                        return 1;
                    }
                    if (Machine.remapped_colortable != null)
                        Machine.gfx[i].colortable = new _ShortPtr(Machine.remapped_colortable, (int)drv.gfxdecodeinfo[i].color_codes_start * sizeof(short));
                    Machine.gfx[i].total_colors = drv.gfxdecodeinfo[i].total_color_codes;
                }
            }
            /* create the display bitmap, and allocate the palette */
            if ((Machine.scrbitmap = osd_create_display(
                    drv.screen_width, drv.screen_height,
                    Machine.color_depth,
                    drv.video_attributes)) == null)
            {
                vh_close();
                return 1;
            }

            //blit_buffer = new uint[drv.screen_width * drv.screen_height];
            blit_buffer = new uint[Machine.scrbitmap.width*Machine.scrbitmap.height];
            back_buffer = new byte[Machine.scrbitmap.line[0].buffer.Length];
            /* create spriteram buffers if necessary */
            if ((drv.video_attributes & VIDEO_BUFFERS_SPRITERAM) != 0)
            {
                if (Generic.spriteram_size[0] != 0)
                {
                    Generic.buffered_spriteram = new _BytePtr(Generic.spriteram_size[0]);
                    if (Generic.buffered_spriteram[0] != 0) { vh_close(); return 1; }
                    if (Generic.spriteram_2_size[0] != 0) Generic.buffered_spriteram_2 = new _BytePtr(Generic.spriteram_2_size[0]);
                    if (Generic.spriteram_2_size[0] != 0 && Generic.buffered_spriteram_2 != null) { vh_close(); return 1; }
                }
                else
                {
                    printf("vh_open():  Video buffers spriteram but spriteram_size is 0\n");
                    Generic.buffered_spriteram = null;
                    Generic.buffered_spriteram_2 = null;
                }
            }

            /* build our private user interface font */
            /* This must be done AFTER osd_create_display() so the function knows the */
            /* resolution we are running at and can pick a different font depending on it. */
            /* It must be done BEFORE palette_init() because that will also initialize */
            /* (through osd_allocate_colors()) the uifont colortable. */
            if ((Machine.uifont = builduifont()) == null)
            {
                vh_close();
                return 1;
            }

            /* initialize the palette - must be done after osd_create_display() */
            if (palette_init() != 0)
            {
                vh_close();
                return 1;
            }


            osd_create_backbuffer(Machine.scrbitmap.width,Machine.scrbitmap.height);

            return 0;
        }
        static void vh_close()
        {
            for (int i = 0; i < MAX_GFX_ELEMENTS; i++)
            {
                Machine.gfx[i] = null;
            }
            Machine.uifont = null;
            osd_close_display();
            palette_stop();

            if ((drv.video_attributes & VIDEO_BUFFERS_SPRITERAM) != 0)
            {
                Generic.buffered_spriteram = null;
                Generic.buffered_spriteram_2 = null;
            }
        }

        int updatescreen()
        {
            /* update sound */
            sound_update();

            if (osd_skip_this_frame() == 0)
            {
                if (need_to_clear_bitmap)
                {
                    osd_clearbitmap(Machine.scrbitmap);
                    need_to_clear_bitmap = false;
                }
                drv.vh_update(Machine.scrbitmap, bitmap_dirty);  /* update screen */
                bitmap_dirty = 0;
            }

            /* the user interface must be called between vh_update() and osd_update_video_and_audio(), */
            /* to allow it to overlay things on the game display. We must call it even */
            /* if the frame is skipped, to keep a consistent timing. */
            if (handle_user_interface())
                /* quit if the user asked to */
                return 1;

            osd_update_video_and_audio();

            drv.vh_eof_callback();

            return 0;
        }
    }
}
