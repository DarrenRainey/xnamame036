using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace xnamame036.mame
{
    partial class Mame
    {
        public static string[] drivers = { "pacman" ,"invaders","1942"};
        Hashtable GameDrivers = new Hashtable();

        void InitGameDriverList()
        {
            foreach (Type driverType in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (driverType.Namespace == "xnamame036.mame.drivers" && driverType.Name.StartsWith("driver_"))
                {
                    GameDrivers[driverType.Name.Substring(7)] = driverType;
                }
            }
        }
        GameDriver GetDriver(string drivername)
        {
            return (GameDriver)Activator.CreateInstance((Type)GameDrivers[drivername]);
        }
        public delegate int vblank_interrupt_callback();
        public class MachineCPU
        {
            public MachineCPU(int cpu_type, int cpu_clock,
                MemoryReadAddress[] memory_read, MemoryWriteAddress[] memory_write,
            IOReadPort[] port_read, IOWritePort[] port_write,
                vblank_interrupt_callback vblank_interrupt, int vblank_interrupts_per_frame,
                vblank_interrupt_callback interrupt = null, int ips = 0)
            {
                this.cpu_type = cpu_type; this.cpu_clock = cpu_clock;
                this.memory_read = memory_read; this.memory_write = memory_write;
                this.port_read = port_read; this.port_write = port_write;
                this.vblank_interrupt = vblank_interrupt; this.vblank_interrupts_per_frame = vblank_interrupts_per_frame;
                this.timed_interrupt = interrupt;
                this.timed_interrupts_per_second = ips;
            }
            public int cpu_type;	/* see #defines below. */
            public int cpu_clock;	/* in Hertz */
            public MemoryReadAddress[] memory_read;
            public MemoryWriteAddress[] memory_write;
            public IOReadPort[] port_read;
            public IOWritePort[] port_write;
            public vblank_interrupt_callback vblank_interrupt;
            public int vblank_interrupts_per_frame;    /* usually 1 */
            /* use this for interrupts which are not tied to vblank 	*/
            /* usually frequency in Hz, but if you need 				*/
            /* greater precision you can give the period in nanoseconds */
            public vblank_interrupt_callback timed_interrupt;
            public int timed_interrupts_per_second;
            /* pointer to a parameter to pass to the CPU cores reset function */
            public object reset_param;
        }
        public abstract class MachineDriver
        {
            public List<MachineCPU> cpu = new List<MachineCPU>();
            public float frames_per_second;
            public int vblank_duration;
            public int cpu_slices_per_frame;
            public abstract void init_machine();
            /* video hardware */
            public int screen_width, screen_height;
            public rectangle visible_area;
            public GfxDecodeInfo[] gfxdecodeinfo;
            public uint total_colors;	/* palette is 3*total_colors bytes long */
            public uint color_table_len;	/* length in shorts of the color lookup table */
            public abstract void vh_init_palette(_BytePtr palette, ushort[] colortable, _BytePtr color_prom);

            public int video_attributes;	/* ASG 081897 */

            public abstract void vh_eof_callback();	/* called every frame after osd_update_video_and_audio() */
            /* This is useful when there are operations that need */
            /* to be performed every frame regardless of frameskip, */
            /* e.g. sprite buffering or collision detection. */
            public abstract int vh_start();
            public abstract void vh_stop();
            public abstract void vh_update(osd_bitmap bitmap, int full_refresh);

            /* sound hardware */
            public int sound_attributes;
            public List<MachineSound> sound = new List<MachineSound>();

            /*
               use this to manage nvram/eeprom/cmos/etc.
               It is called before the emulation starts and after it ends. Note that it is
               NOT called when the game is reset, since it is not needed.
               file == 0, read_or_write == 0 . first time the game is run, initialize nvram
               file != 0, read_or_write == 0 . load nvram from disk
               file == 0, read_or_write != 0 . not allowed
               file != 0, read_or_write != 0 . save nvram to disk
             */
            public bool HasNVRAMhandler;
            public abstract void nvram_handler(object file, int read_or_write);
        }
        public abstract class GameDriver : ports
        {
            public string source_File;
            public GameDriver clone_of = null;
            public string name;
            public string description;
            public string year;
            public string manufacturer;
            public MachineDriver drv;
            public InputPortTiny[] input_ports;
            public abstract void driver_init();
            public RomModule[] rom;
            public uint flags;
            public static int TOTAL_COLORS(int gfxn){return (Machine.gfx[gfxn].total_colors * Machine.gfx[gfxn].color_granularity);}
            public static void COLOR(ushort[] colortable, int gfxn, int offs, int value) { colortable[Machine.drv.gfxdecodeinfo[gfxn].color_codes_start + offs]= (ushort)value; }
        }
        public const ushort ROT0 = 0x0000;
        public const ushort ROT90 = (ORIENTATION_SWAP_XY | ORIENTATION_FLIP_X);	/* rotate clockwise 90 degrees */
        public const ushort ROT180 = (ORIENTATION_FLIP_X | ORIENTATION_FLIP_Y);	/* rotate 180 degrees */
        public const ushort ROT270 = (ORIENTATION_SWAP_XY | ORIENTATION_FLIP_Y);	/* rotate counter-clockwise 90 degrees */
        public const ushort ROT0_16BIT = (ROT0 | GAME_REQUIRES_16BIT);
        public const ushort ROT90_16BIT = (ROT90 | GAME_REQUIRES_16BIT);
        public const ushort ROT180_16BIT = (ROT180 | GAME_REQUIRES_16BIT);
        public const ushort ROT270_16BIT = (ROT270 | GAME_REQUIRES_16BIT);

        public const ushort CPU_AUDIO_CPU = 0x8000;
        public const ushort CPU_16BIT_PORT = 0x4000;
        public const ushort CPU_FLAGS_MASK = 0xff00;
        const int MAX_CPU = 8;
        const int MAX_SOUND = 5;

        /* VBlank is the period when the video beam is outside of the visible area and */
        /* returns from the bottom to the top of the screen to prepare for a new video frame. */
        /* VBlank duration is an important factor in how the game renders itself. MAME */
        /* generates the vblank_interrupt, lets the game run for vblank_duration microseconds, */
        /* and then updates the screen. This faithfully reproduces the behaviour of the real */
        /* hardware. In many cases, the game does video related operations both in its vblank */
        /* interrupt, and in the normal game code; it is therefore important to set up */
        /* vblank_duration accurately to have everything properly in sync. An example of this */
        /* is Commando: if you set vblank_duration to 0, therefore redrawing the screen BEFORE */
        /* the vblank interrupt is executed, sprites will be misaligned when the screen scrolls. */

        /* Here are some predefined, TOTALLY ARBITRARY values for vblank_duration, which should */
        /* be OK for most cases. I have NO IDEA how accurate they are compared to the real */
        /* hardware, they could be completely wrong. */
        public const ushort DEFAULT_60HZ_VBLANK_DURATION = 0;
        public const ushort DEFAULT_30HZ_VBLANK_DURATION = 0;
        /* If you use IPT_VBLANK, you need a duration different from 0. */
        public const ushort DEFAULT_REAL_60HZ_VBLANK_DURATION = 2500;
        public const ushort DEFAULT_REAL_30HZ_VBLANK_DURATION = 2500;



        /* flags for video_attributes */

        /* bit 0 of the video attributes indicates raster or vector video hardware */
        public const ushort VIDEO_TYPE_RASTER = 0x0000;
        public const ushort VIDEO_TYPE_VECTOR = 0x0001;

        /* bit 1 of the video attributes indicates whether or not dirty rectangles will work */
        public const ushort VIDEO_SUPPORTS_DIRTY = 0x0002;

        /* bit 2 of the video attributes indicates whether or not the driver modifies the palette */
        public const ushort VIDEO_MODIFIES_PALETTE = 0x0004;

        /* ASG 980417 - added: */
        /* bit 4 of the video attributes indicates that the driver wants its refresh after */
        /*       the VBLANK instead of before. */
        public const ushort VIDEO_UPDATE_BEFORE_VBLANK = 0x0000;
        public const ushort VIDEO_UPDATE_AFTER_VBLANK = 0x0010;

        /* In most cases we assume pixels are square (1:1 aspect ratio) but some games need */
        /* different proportions, e.g. 1:2 for Blasteroids */
        public const ushort VIDEO_PIXEL_ASPECT_RATIO_MASK = 0x0020;
        public const ushort VIDEO_PIXEL_ASPECT_RATIO_1_1 = 0x0000;
        public const ushort VIDEO_PIXEL_ASPECT_RATIO_1_2 = 0x0020;

        public const ushort VIDEO_DUAL_MONITOR = 0x0040;

        /* Mish 181099:  See comments in vidhrdw/generic.c for details */
        public const ushort VIDEO_BUFFERS_SPRITERAM = 0x0080;

        /* flags for sound_attributes */
        public const ushort SOUND_SUPPORTS_STEREO = 0x0001;

        /* values for the flags field */

        public const ushort ORIENTATION_MASK = 0x0007;
        public const ushort ORIENTATION_FLIP_X = 0x0001;	/* mirror everything in the X direction */
        public const ushort ORIENTATION_FLIP_Y = 0x0002;	/* mirror everything in the Y direction */
        public const ushort ORIENTATION_SWAP_XY = 0x0004;	/* mirror along the top-left/bottom-right diagonal */

        public const ushort GAME_NOT_WORKING = 0x0008;
        public const ushort GAME_WRONG_COLORS = 0x0010;	/* colors are totally wrong */
        public const ushort GAME_IMPERFECT_COLORS = 0x0020;	/* colors are not 100% accurate, but close */
        public const ushort GAME_NO_SOUND = 0x0040;	/* sound is missing */
        public const ushort GAME_IMPERFECT_SOUND = 0x0080;	/* sound is known to be wrong */
        public const ushort GAME_REQUIRES_16BIT = 0x0100;	/* cannot fit in 256 colors */
        public const ushort GAME_NO_COCKTAIL = 0x0200;	/* screen flip support is missing */
        public const ushort NOT_A_DRIVER = 0x4000;	/* set by the fake "root" driver_ and by "containers" */
    }
}
