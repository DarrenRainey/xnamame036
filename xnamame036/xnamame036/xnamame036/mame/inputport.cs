using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    using InputCode = System.UInt32;
    using InputSeq = List<System.UInt32>;

    partial class Mame
    {
        const string MAMECFGSTRING_V8 = "MAMECFG_8";
        const string MAMEDEFSTRING_V8 = "MAMEDEFSTRING_V8_7";
        public class InputPortTiny
        {
            public InputPortTiny(ushort mask, ushort default_value, uint type, string name)
            {
                this.mask = mask; this.default_value = default_value; this.type = type; this.name = name;
            }
            public ushort mask, default_value;
            public uint type;
            public string name;
        }
        public class InputPort
        {
            public ushort mask, default_value;
            public uint type;
            public string name;
            public uint[] seq = new uint[SEQ_MAX];
            public InputPort()
            {
                for (int i = 0; i < SEQ_MAX; i++)
                    seq[i] = 0;
            }
        }
        const int MAX_INPUT_PORTS = 20;

        static ushort[] input_port_value = new ushort[MAX_INPUT_PORTS];
        static ushort[] input_vblank = new ushort[MAX_INPUT_PORTS];
        static int[] input_analog = new int[MAX_INPUT_PORTS];
        static int[] input_analog_current_value = new int[MAX_INPUT_PORTS];
        static int[] input_analog_init = new int[MAX_INPUT_PORTS];
        static int[] input_analog_previous_value = new int[MAX_INPUT_PORTS];
        /* Assuming a maxium of one analog input device per port BW 101297 */
        const int OSD_MAX_JOY_ANALOG = 4;

        static int[] mouse_delta_x = new int[OSD_MAX_JOY_ANALOG], mouse_delta_y = new int[OSD_MAX_JOY_ANALOG];
        static int[] analog_current_x = new int[OSD_MAX_JOY_ANALOG], analog_current_y = new int[OSD_MAX_JOY_ANALOG];
        static int[] analog_previous_x = new int[OSD_MAX_JOY_ANALOG], analog_previous_y = new int[OSD_MAX_JOY_ANALOG];


        class ipd
        {
            public uint type;
            public string name;
            public InputCode[] seq;
            public ipd(uint type, string name, InputCode[] seq) { this.type = type; this.name = name; this.seq = seq; }
        }
        static uint[] SEQ_DEF_0 { get { return SEQ_DEF_1((uint)InputCodes.CODE_NONE); } }
        static uint[] SEQ_DEF_1(InputCode a) { return SEQ_DEF_2(a, (uint)InputCodes.CODE_NONE); }
        static uint[] SEQ_DEF_2(InputCode a, InputCode b) { return SEQ_DEF_3(a, b, (uint)InputCodes.CODE_NONE); }
        static uint[] SEQ_DEF_3(InputCode a, InputCode b, InputCode c) { return SEQ_DEF_4(a, b, c, (uint)InputCodes.CODE_NONE); }
        static uint[] SEQ_DEF_4(InputCode a, InputCode b, InputCode c, InputCode d) { return SEQ_DEF_5(a, b, c, d, (int)InputCodes.CODE_NONE); }
        static uint[] SEQ_DEF_5(InputCode a, InputCode b, InputCode c, InputCode d, InputCode e) { return SEQ_DEF_6(a, b, c, d, e, (uint)InputCodes.CODE_NONE); }
        static uint[] SEQ_DEF_6(InputCode a, InputCode b, InputCode c, InputCode d, InputCode e, InputCode f)
        {
            return new uint[] { a, b, c, d, e, f, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE, (uint)InputCodes.CODE_NONE };
        }

        static ipd[] inputport_defaults =
{
	new ipd( (uint)ports.inptports.IPT_UI_CONFIGURE,         "Config Menu",       SEQ_DEF_3((int)InputCodes.KEYCODE_TAB,(int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_1_BUTTON6) ),
	new ipd( (uint)ports.inptports.IPT_UI_ON_SCREEN_DISPLAY, "On Screen Display", SEQ_DEF_1((int)InputCodes.KEYCODE_TILDE) ),
	new ipd( (uint)ports.inptports.IPT_UI_PAUSE,             "Pause",             SEQ_DEF_1((int)InputCodes.KEYCODE_P) ),
	new ipd( (uint)ports.inptports.IPT_UI_RESET_MACHINE,     "Reset Game",        SEQ_DEF_1((int)InputCodes.KEYCODE_F3) ),
	new ipd( (uint)ports.inptports.IPT_UI_SHOW_GFX,          "Show Gfx",          SEQ_DEF_1((int)InputCodes.KEYCODE_F4) ),
	new ipd( (uint)ports.inptports.IPT_UI_FRAMESKIP_DEC,     "Frameskip Dec",     SEQ_DEF_1((int)InputCodes.KEYCODE_F8) ),
	new ipd( (uint)ports.inptports.IPT_UI_FRAMESKIP_INC,     "Frameskip Inc",     SEQ_DEF_1((int)InputCodes.KEYCODE_F9) ),
	new ipd( (uint)ports.inptports.IPT_UI_THROTTLE,          "Throttle",          SEQ_DEF_1((int)InputCodes.KEYCODE_F10) ),
	new ipd( (uint)ports.inptports.IPT_UI_SHOW_FPS,          "Show FPS",          SEQ_DEF_5((int)InputCodes.KEYCODE_F11, (int)InputCodes.CODE_NOT, (int)InputCodes.KEYCODE_LCONTROL, (int)InputCodes.CODE_NOT, (int)InputCodes.KEYCODE_LSHIFT) ),
	new ipd( (uint)ports.inptports.IPT_UI_SHOW_PROFILER,     "Show Profiler",     SEQ_DEF_2((int)InputCodes.KEYCODE_F11, (int)InputCodes.KEYCODE_LSHIFT) ),

	new ipd( (uint)ports.inptports.IPT_UI_SNAPSHOT,          "Save Snapshot",     SEQ_DEF_1((int)InputCodes.KEYCODE_F12) ),
	new ipd( (uint)ports.inptports.IPT_UI_TOGGLE_CHEAT,      "Toggle Cheat",      SEQ_DEF_1((int)InputCodes.KEYCODE_F5) ),
	new ipd( (uint)ports.inptports.IPT_UI_UP,                "UI Up",             SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( (uint)ports.inptports.IPT_UI_DOWN,              "UI Down",           SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_UI_LEFT,              "UI Left",           SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( (uint)ports.inptports.IPT_UI_RIGHT,             "UI Right",          SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_UI_SELECT,            "UI Select",         SEQ_DEF_3((int)InputCodes.KEYCODE_ENTER, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON1) ),
	new ipd( (uint)ports.inptports.IPT_UI_CANCEL,            "UI Cancel",         SEQ_DEF_1((int)InputCodes.KEYCODE_ESC) ),
	new ipd( (uint)ports.inptports.IPT_UI_PAN_UP,            "Pan Up",            SEQ_DEF_3((int)InputCodes.KEYCODE_PGUP, (int)InputCodes.CODE_NOT, (int)InputCodes.KEYCODE_LSHIFT) ),
	new ipd( (uint)ports.inptports.IPT_UI_PAN_DOWN,          "Pan Down",          SEQ_DEF_3((int)InputCodes.KEYCODE_PGDN, (int)InputCodes.CODE_NOT, (int)InputCodes.KEYCODE_LSHIFT) ),
	new ipd( (uint)ports.inptports.IPT_UI_PAN_LEFT,          "Pan Left",          SEQ_DEF_2((int)InputCodes.KEYCODE_PGUP, (int)InputCodes.KEYCODE_LSHIFT) ),
	new ipd( (uint)ports.inptports.IPT_UI_PAN_RIGHT,         "Pan Right",         SEQ_DEF_2((int)InputCodes.KEYCODE_PGDN, (int)InputCodes.KEYCODE_LSHIFT) ),
	new ipd( (uint)ports.inptports.IPT_START1, "1 Player Start",  SEQ_DEF_3((int)InputCodes.KEYCODE_1,(int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_1_BUTTON7) ),
	new ipd( (uint)ports.inptports.IPT_START2, "2 Players Start", SEQ_DEF_3((int)InputCodes.KEYCODE_2, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_1_BUTTON8) ),
	new ipd( (uint)ports.inptports.IPT_START3, "3 Players Start", SEQ_DEF_1((int)InputCodes.KEYCODE_3) ),
	new ipd( (uint)ports.inptports.IPT_START4, "4 Players Start", SEQ_DEF_1((int)InputCodes.KEYCODE_4) ),
	new ipd( (uint)ports.inptports.IPT_COIN1,  "Coin 1",          SEQ_DEF_3((int)InputCodes.KEYCODE_5,(int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_1_BUTTON10) ),
	new ipd( (uint)ports.inptports.IPT_COIN2,  "Coin 2",          SEQ_DEF_1((int)InputCodes.KEYCODE_6) ),
	new ipd( (uint)ports.inptports.IPT_COIN3,  "Coin 3",          SEQ_DEF_1((int)InputCodes.KEYCODE_7) ),
	new ipd( (uint)ports.inptports.IPT_COIN4,  "Coin 4",          SEQ_DEF_1((int)InputCodes.KEYCODE_8) ),
	new ipd( (uint)ports.inptports.IPT_SERVICE1, "Service 1",     SEQ_DEF_1((int)InputCodes.KEYCODE_9) ),
	new ipd( (uint)ports.inptports.IPT_SERVICE2, "Service 2",     SEQ_DEF_1((int)InputCodes.KEYCODE_0) ),
	new ipd( (uint)ports.inptports.IPT_SERVICE3, "Service 3",     SEQ_DEF_1((int)InputCodes.KEYCODE_MINUS) ),
	new ipd( (uint)ports.inptports.IPT_SERVICE4, "Service 4",     SEQ_DEF_1((int)InputCodes.KEYCODE_EQUALS) ),
	new ipd( (uint)ports.inptports.IPT_TILT,   "Tilt",            SEQ_DEF_1((int)InputCodes.KEYCODE_T) ),

	new ipd( (uint)ports.inptports.IPT_JOYSTICK_UP         | ports.IPF_PLAYER1, "P1 Up",          SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP)    ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_DOWN       | ports.IPF_PLAYER1, "P1 Down",        SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN,(int)InputCodes. CODE_OR, (int)InputCodes.JOYCODE_1_DOWN)  ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_LEFT       | ports.IPF_PLAYER1, "P1 Left",        SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT)  ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_RIGHT      | ports.IPF_PLAYER1, "P1 Right",       SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON1             | ports.IPF_PLAYER1, "P1 Button 1",    SEQ_DEF_3((int)InputCodes.KEYCODE_A, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON1) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON2             | ports.IPF_PLAYER1, "P1 Button 2",    SEQ_DEF_3((int)InputCodes.KEYCODE_LALT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON2) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON3             | ports.IPF_PLAYER1, "P1 Button 3",    SEQ_DEF_3((int)InputCodes.KEYCODE_SPACE, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON3) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON4             | ports.IPF_PLAYER1, "P1 Button 4",    SEQ_DEF_3((int)InputCodes.KEYCODE_LSHIFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON4) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON5             | ports.IPF_PLAYER1, "P1 Button 5",    SEQ_DEF_3((int)InputCodes.KEYCODE_Z, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON5) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON6             | ports.IPF_PLAYER1, "P1 Button 6",    SEQ_DEF_3((int)InputCodes.KEYCODE_X, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON6) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON7             | ports.IPF_PLAYER1, "P1 Button 7",    SEQ_DEF_1((int)InputCodes.KEYCODE_C) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON8             | ports.IPF_PLAYER1, "P1 Button 8",    SEQ_DEF_1((int)InputCodes.KEYCODE_V) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON9             | ports.IPF_PLAYER1, "P1 Button 9",    SEQ_DEF_1((int)InputCodes.KEYCODE_B) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_UP    | ports.IPF_PLAYER1, "P1 Right/Up",    SEQ_DEF_3((int)InputCodes.KEYCODE_I,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON2) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_DOWN  | ports.IPF_PLAYER1, "P1 Right/Down",  SEQ_DEF_3((int)InputCodes.KEYCODE_K,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON3) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_LEFT  | ports.IPF_PLAYER1, "P1 Right/Left",  SEQ_DEF_3((int)InputCodes.KEYCODE_J,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON1) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_RIGHT | ports.IPF_PLAYER1, "P1 Right/Right", SEQ_DEF_3((int)InputCodes.KEYCODE_L,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON4) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_UP     | ports.IPF_PLAYER1, "P1 Left/Up",     SEQ_DEF_3((int)InputCodes.KEYCODE_E,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_DOWN   | ports.IPF_PLAYER1, "P1 Left/Down",   SEQ_DEF_3((int)InputCodes.KEYCODE_D,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_LEFT   | ports.IPF_PLAYER1, "P1 Left/Left",   SEQ_DEF_3((int)InputCodes.KEYCODE_S,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_RIGHT  | ports.IPF_PLAYER1, "P1 Left/Right",  SEQ_DEF_3((int)InputCodes.KEYCODE_F,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),

	new ipd( (uint)ports.inptports.IPT_JOYSTICK_UP         | ports.IPF_PLAYER2, "P2 Up",          SEQ_DEF_3((int)InputCodes.KEYCODE_R, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_UP)    ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_DOWN       | ports.IPF_PLAYER2, "P2 Down",        SEQ_DEF_3((int)InputCodes.KEYCODE_F, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_DOWN)  ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_LEFT       | ports.IPF_PLAYER2, "P2 Left",        SEQ_DEF_3((int)InputCodes.KEYCODE_D, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_LEFT)  ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_RIGHT      | ports.IPF_PLAYER2, "P2 Right",       SEQ_DEF_3((int)InputCodes.KEYCODE_G, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON1             | ports.IPF_PLAYER2, "P2 Button 1",    SEQ_DEF_3((int)InputCodes.KEYCODE_A, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_BUTTON1) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON2             | ports.IPF_PLAYER2, "P2 Button 2",    SEQ_DEF_3((int)InputCodes.KEYCODE_S, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_BUTTON2) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON3             | ports.IPF_PLAYER2, "P2 Button 3",    SEQ_DEF_3((int)InputCodes.KEYCODE_Q, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_BUTTON3) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON4             | ports.IPF_PLAYER2, "P2 Button 4",    SEQ_DEF_3((int)InputCodes.KEYCODE_W, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_2_BUTTON4) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON5             | ports.IPF_PLAYER2, "P2 Button 5",    SEQ_DEF_1((int)InputCodes.JOYCODE_2_BUTTON5) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON6             | ports.IPF_PLAYER2, "P2 Button 6",    SEQ_DEF_1((int)InputCodes.JOYCODE_2_BUTTON6) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON7             | ports.IPF_PLAYER2, "P2 Button 7",    SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_BUTTON8             | ports.IPF_PLAYER2, "P2 Button 8",    SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_BUTTON9             | ports.IPF_PLAYER2, "P2 Button 9",    SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_UP    | ports.IPF_PLAYER2, "P2 Right/Up",    SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_DOWN  | ports.IPF_PLAYER2, "P2 Right/Down",  SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_LEFT  | ports.IPF_PLAYER2, "P2 Right/Left",  SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKRIGHT_RIGHT | ports.IPF_PLAYER2, "P2 Right/Right", SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_UP     | ports.IPF_PLAYER2, "P2 Left/Up",     SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_DOWN   | ports.IPF_PLAYER2, "P2 Left/Down",   SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_LEFT   | ports.IPF_PLAYER2, "P2 Left/Left",   SEQ_DEF_0 ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICKLEFT_RIGHT  | ports.IPF_PLAYER2, "P2 Left/Right",  SEQ_DEF_0 ),

	new ipd(  (uint)ports.inptports.IPT_JOYSTICK_UP         | ports.IPF_PLAYER3, "P3 Up",          SEQ_DEF_3((int)InputCodes.KEYCODE_I, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_UP)    ),
	new ipd(  (uint)ports.inptports.IPT_JOYSTICK_DOWN       | ports.IPF_PLAYER3, "P3 Down",        SEQ_DEF_3((int)InputCodes.KEYCODE_K, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_DOWN)  ),
	new ipd(  (uint)ports.inptports.IPT_JOYSTICK_LEFT       | ports.IPF_PLAYER3, "P3 Left",        SEQ_DEF_3((int)InputCodes.KEYCODE_J, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_LEFT)  ),
	new ipd(  (uint)ports.inptports.IPT_JOYSTICK_RIGHT      | ports.IPF_PLAYER3, "P3 Right",       SEQ_DEF_3((int)InputCodes.KEYCODE_L, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_RIGHT) ),
	new ipd(  (uint)ports.inptports.IPT_BUTTON1             | ports.IPF_PLAYER3, "P3 Button 1",    SEQ_DEF_3((int)InputCodes.KEYCODE_RCONTROL, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_BUTTON1) ),
	new ipd(  (uint)ports.inptports.IPT_BUTTON2             | ports.IPF_PLAYER3, "P3 Button 2",    SEQ_DEF_3((int)InputCodes.KEYCODE_RSHIFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_BUTTON2) ),
	new ipd(  (uint)ports.inptports.IPT_BUTTON3             | ports.IPF_PLAYER3, "P3 Button 3",    SEQ_DEF_3((int)InputCodes.KEYCODE_ENTER,(int)InputCodes. CODE_OR, (int)InputCodes.JOYCODE_3_BUTTON3) ),
	new ipd(  (uint)ports.inptports.IPT_BUTTON4             | ports.IPF_PLAYER3, "P3 Button 4",    SEQ_DEF_1((int)InputCodes.JOYCODE_3_BUTTON4) ),

	new ipd( (uint)ports.inptports.IPT_JOYSTICK_UP         | ports.IPF_PLAYER4, "P4 Up",          SEQ_DEF_1((int)InputCodes.JOYCODE_4_UP) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_DOWN       | ports.IPF_PLAYER4, "P4 Down",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_LEFT       | ports.IPF_PLAYER4, "P4 Left",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_LEFT) ),
	new ipd( (uint)ports.inptports.IPT_JOYSTICK_RIGHT      | ports.IPF_PLAYER4, "P4 Right",       SEQ_DEF_1((int)InputCodes.JOYCODE_4_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON1             | ports.IPF_PLAYER4, "P4 Button 1",    SEQ_DEF_1((int)InputCodes.JOYCODE_4_BUTTON1) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON2             | ports.IPF_PLAYER4, "P4 Button 2",    SEQ_DEF_1((int)InputCodes.JOYCODE_4_BUTTON2) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON3             | ports.IPF_PLAYER4, "P4 Button 3",    SEQ_DEF_1((int)InputCodes.JOYCODE_4_BUTTON3) ),
	new ipd( (uint)ports.inptports.IPT_BUTTON4             | ports.IPF_PLAYER4, "P4 Button 4",    SEQ_DEF_1((int)InputCodes.JOYCODE_4_BUTTON4) ),

	new ipd( (uint)ports.inptports.IPT_PEDAL	                |ports.IPF_PLAYER1, "Pedal 1",        SEQ_DEF_3((int)InputCodes.KEYCODE_LCONTROL, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_BUTTON1) ),
	new ipd( ((uint)ports.inptports.IPT_PEDAL+(uint)ports.inptports.IPT_EXTENSION) |ports.IPF_PLAYER1, "P1 Auto Release <Y/N>", SEQ_DEF_1((int)InputCodes.KEYCODE_Y) ),
	new ipd( (uint)ports.inptports.IPT_PEDAL                 |ports.IPF_PLAYER2, "Pedal 2",        SEQ_DEF_3((int)InputCodes.KEYCODE_A,(int)InputCodes. CODE_OR, (int)InputCodes.JOYCODE_2_BUTTON1) ),
	new ipd( ((uint)ports.inptports.IPT_PEDAL+(uint)ports.inptports.IPT_EXTENSION) |ports.IPF_PLAYER2, "P2 Auto Release <Y/N>", SEQ_DEF_1((int)InputCodes.KEYCODE_Y) ),
	new ipd( (uint)ports.inptports.IPT_PEDAL                 |ports.IPF_PLAYER3, "Pedal 3",        SEQ_DEF_3((int)InputCodes.KEYCODE_RCONTROL, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_BUTTON1) ),
	new ipd( ((uint)ports.inptports.IPT_PEDAL+(uint)ports.inptports.IPT_EXTENSION) |ports.IPF_PLAYER3, "P3 Auto Release <Y/N>", SEQ_DEF_1((int)InputCodes.KEYCODE_Y) ),
	new ipd( (uint)ports.inptports.IPT_PEDAL                 |ports.IPF_PLAYER4, "Pedal 4",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_BUTTON1) ),
	new ipd( ((uint)ports.inptports.IPT_PEDAL+(uint)ports.inptports.IPT_EXTENSION) |ports.IPF_PLAYER4, "P4 Auto Release <Y/N>", SEQ_DEF_1((int)InputCodes.KEYCODE_Y) ),

	new ipd( (uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER1,  "Paddle",        SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle",        SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT)  ),
	new ipd( (uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER2,  "Paddle 2",      SEQ_DEF_3((int)InputCodes.KEYCODE_D, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle 2",      SEQ_DEF_3((int)InputCodes.KEYCODE_G, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER3,  "Paddle 3",      SEQ_DEF_3((int)InputCodes.KEYCODE_J, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_LEFT) ),
	new ipd(((uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle 3",      SEQ_DEF_3((int)InputCodes.KEYCODE_L, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER4,  "Paddle 4",      SEQ_DEF_1((int)InputCodes.JOYCODE_4_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle 4",      SEQ_DEF_1((int)InputCodes.JOYCODE_4_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER1,  "Paddle V",          SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle V",          SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER2,  "Paddle V 2",        SEQ_DEF_3((int)InputCodes.KEYCODE_R,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_UP) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle V 2",      SEQ_DEF_3((int)InputCodes.KEYCODE_F, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER3,  "Paddle V 3",        SEQ_DEF_3((int)InputCodes.KEYCODE_I, (int)InputCodes.CODE_OR,(int)InputCodes.JOYCODE_3_UP) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle V 3",      SEQ_DEF_3((int)InputCodes.KEYCODE_K, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER4,  "Paddle V 4",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_UP) ),
	new ipd( ((uint)ports.inptports.IPT_PADDLE_V |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,             "Paddle V 4",      SEQ_DEF_1((int)InputCodes.JOYCODE_4_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER1,    "Dial",          SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT,(int)InputCodes. CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,               "Dial",          SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER2,    "Dial 2",        SEQ_DEF_3((int)InputCodes.KEYCODE_D, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,               "Dial 2",      SEQ_DEF_3((int)InputCodes.KEYCODE_G, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER3,    "Dial 3",        SEQ_DEF_3((int)InputCodes.KEYCODE_J, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,               "Dial 3",      SEQ_DEF_3((int)InputCodes.KEYCODE_L, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER4,    "Dial 4",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,               "Dial 4",      SEQ_DEF_1((int)InputCodes.JOYCODE_4_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER1,  "Dial V",          SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,             "Dial V",          SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER2,  "Dial V 2",        SEQ_DEF_3((int)InputCodes.KEYCODE_R, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_UP) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,             "Dial V 2",      SEQ_DEF_3((int)InputCodes.KEYCODE_F, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER3,  "Dial V 3",        SEQ_DEF_3((int)InputCodes.KEYCODE_I, (int)InputCodes.CODE_OR,(int)InputCodes. JOYCODE_3_UP) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,             "Dial V 3",      SEQ_DEF_3((int)InputCodes.KEYCODE_K, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_DOWN) ),
	new ipd( (uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER4,  "Dial V 4",        SEQ_DEF_1((int)InputCodes.JOYCODE_4_UP) ),
	new ipd( ((uint)ports.inptports.IPT_DIAL_V |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,             "Dial V 4",      SEQ_DEF_1((int)InputCodes.JOYCODE_4_DOWN) ),

	new ipd( (uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER1, "Track X",   SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,                 "Track X",   SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER2, "Track X 2", SEQ_DEF_3((int)InputCodes.KEYCODE_D, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,                 "Track X 2", SEQ_DEF_3((int)InputCodes.KEYCODE_G, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER3, "Track X 3", SEQ_DEF_3((int)InputCodes.KEYCODE_J, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,                 "Track X 3", SEQ_DEF_3((int)InputCodes.KEYCODE_L, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_RIGHT) ),
	new ipd( (uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER4, "Track X 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_X |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,                 "Track X 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_RIGHT) ),

	new ipd((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER1, "Track Y",   SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,                 "Track Y",   SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER2, "Track Y 2", SEQ_DEF_3((int)InputCodes.KEYCODE_R,(int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_UP) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,                 "Track Y 2", SEQ_DEF_3((int)InputCodes.KEYCODE_F, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_DOWN) ),
	new ipd((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER3, "Track Y 3", SEQ_DEF_3((int)InputCodes.KEYCODE_I, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_UP) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,                 "Track Y 3", SEQ_DEF_3((int)InputCodes.KEYCODE_K, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_DOWN) ),
	new ipd((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER4, "Track Y 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_UP) ),
	new ipd( ((uint)ports.inptports.IPT_TRACKBALL_Y |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,                 "Track Y 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_DOWN) ),

	new ipd((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER1, "AD Stick X",   SEQ_DEF_3((int)InputCodes.KEYCODE_LEFT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick X",   SEQ_DEF_3((int)InputCodes.KEYCODE_RIGHT, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_RIGHT) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER2, "AD Stick X 2", SEQ_DEF_3((int)InputCodes.KEYCODE_D, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick X 2", SEQ_DEF_3((int)InputCodes.KEYCODE_G, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_RIGHT) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER3, "AD Stick X 3", SEQ_DEF_3((int)InputCodes.KEYCODE_J, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick X 3", SEQ_DEF_3((int)InputCodes.KEYCODE_L, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_RIGHT) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER4, "AD Stick X 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_LEFT) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_X |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick X 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_RIGHT) ),

	new ipd((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER1, "AD Stick Y",   SEQ_DEF_3((int)InputCodes.KEYCODE_UP, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_UP) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER1)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick Y",   SEQ_DEF_3((int)InputCodes.KEYCODE_DOWN, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_1_DOWN) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER2, "AD Stick Y 2", SEQ_DEF_3((int)InputCodes.KEYCODE_R, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_UP) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER2)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick Y 2", SEQ_DEF_3((int)InputCodes.KEYCODE_F, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_2_DOWN) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER3, "AD Stick Y 3", SEQ_DEF_3((int)InputCodes.KEYCODE_I, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_UP) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER3)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick Y 3", SEQ_DEF_3((int)InputCodes.KEYCODE_K, (int)InputCodes.CODE_OR, (int)InputCodes.JOYCODE_3_DOWN) ),
	new ipd((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER4, "AD Stick Y 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_UP) ),
	new ipd( ((uint)ports.inptports.IPT_AD_STICK_Y |ports.IPF_PLAYER4)+(uint)ports.inptports.IPT_EXTENSION,                "AD Stick Y 4", SEQ_DEF_1((int)InputCodes.JOYCODE_4_DOWN) ),

	new ipd((uint)ports.inptports.IPT_UNKNOWN,             "UNKNOWN",         SEQ_DEF_0 ),
	new ipd((uint)ports.inptports.IPT_END,                 null,                 SEQ_DEF_0 )	/* returned when there is no match */
};

        static ipd[] inputport_defaults_backup = new ipd[inputport_defaults.Length];


        uint input_port_count(InputPortTiny[] src)
        {
            uint total;
            int sidx = 0;
            total = 0;
            while (src[sidx].type != (int)ports.inptports.IPT_END)
            {
                int type = (int)(src[sidx].type & ~ports.IPF_MASK);
                if (type > (int)ports.inptports.IPT_ANALOG_START && type < (int)ports.inptports.IPT_ANALOG_END)
                    total += 2;
                else if (type != (int)ports.inptports.IPT_EXTENSION)
                    ++total;
                ++sidx;
            }

            ++total; /* for IPT_END */

            return total;
        }
        InputCode IP_GET_CODE_OR1(InputPortTiny port) { return port.mask; }
        InputCode IP_GET_CODE_OR2(InputPortTiny port) { return port.default_value; }
        InputPort[] input_port_allocate(InputPortTiny[] src)
        {
            int dst;
            int sidx = 0;
            InputPort[] _base;
            uint total;

            total = input_port_count(src);

            _base = new InputPort[total];
            dst = 0;

            while (src[sidx].type != (int)ports.inptports.IPT_END)
            {
                int type = (int)(src[sidx].type & ~ports.IPF_MASK);
                int ext;
                int src_end;
                InputCode seq_default;

                if (type > (int)ports.inptports.IPT_ANALOG_START && type < (int)ports.inptports.IPT_ANALOG_END)
                    src_end = sidx + 2;
                else
                    src_end = sidx + 1;

                switch (type)
                {
                    case (int)ports.inptports.IPT_END:
                    case (int)ports.inptports.IPT_PORT:
                    case (int)ports.inptports.IPT_DIPSWITCH_NAME:
                    case (int)ports.inptports.IPT_DIPSWITCH_SETTING:
                        seq_default = (int)InputCodes.CODE_NONE;
                        break;
                    default:
                        seq_default = (int)InputCodes.CODE_DEFAULT;
                        break;
                }

                ext = src_end;
                while (sidx < src_end)
                {
                    _base[dst] = new InputPort();
                    _base[dst].type = src[sidx].type;
                    _base[dst].mask = src[sidx].mask;
                    _base[dst].default_value = src[sidx].default_value;
                    _base[dst].name = src[sidx].name;

                    if (src[ext].type == (int)ports.inptports.IPT_EXTENSION)
                    {
                        InputCode or1 = IP_GET_CODE_OR1(src[ext]);
                        InputCode or2 = IP_GET_CODE_OR2(src[ext]);

                        if (or1 < (int)InputCodes.__code_max)
                        {
                            if (or2 < (int)InputCodes.__code_max)
                                seq_set_3(_base[dst].seq, or1, (int)InputCodes.CODE_OR, or2);
                            else
                                seq_set_1(_base[dst].seq, or1);
                        }
                        else
                        {
                            if (or1 == (int)InputCodes.CODE_NONE)
                                seq_set_1(_base[dst].seq, or2);
                            else
                                seq_set_1(_base[dst].seq, or1);
                        }

                        ++ext;
                    }
                    else
                    {
                        seq_set_1(_base[dst].seq, seq_default);
                    }

                    ++sidx;
                    ++dst;
                }

                sidx = ext;
            }
            _base[dst] = new InputPort();
            _base[dst].type = (int)ports.inptports.IPT_END;

            return _base;
        }
        public static int readinputport(int port)
        {
            InputPort _in;

            /* Update analog ports on demand */
            _in = Machine.input_ports[input_analog[port]];
            if (_in != null)
            {
                scale_analog_port(port);
            }

            return input_port_value[port];
        }
        static void scale_analog_port(int port)
        {
int _in;
	int delta,current,sensitivity;

	_in = input_analog[port];
	sensitivity = (int)IP_GET_SENSITIVITY(Machine.input_ports,_in);

	delta = cpu_scalebyfcount(input_analog_current_value[port] - input_analog_previous_value[port]);

	current = input_analog_previous_value[port] + delta;

	input_port_value[port] &= (ushort)~Machine.input_ports[input_analog[port]].mask;
    input_port_value[port] |= (ushort)(((current * sensitivity + 50) / 100) & Machine.input_ports[input_analog[port]].mask);

	//if (playback)		readword(playback,&input_port_value[port]);
	//if (record)		writeword(record,input_port_value[port]);
        }
        public static int input_port_0_r(int offset) { return readinputport(0); }
        public static int input_port_1_r(int offset) { return readinputport(1); }
        public static int input_port_2_r(int offset) { return readinputport(2); }
        public static int input_port_3_r(int offset) { return readinputport(3); }
        public static int input_port_4_r(int offset) { return readinputport(4); }
        public static int input_port_5_r(int offset) { return readinputport(5); }
        public static int input_port_6_r(int offset) { return readinputport(6); }
        public static int input_port_7_r(int offset) { return readinputport(7); }
        public static int input_port_8_r(int offset) { return readinputport(8); }
        public static int input_port_9_r(int offset) { return readinputport(9); }
        public static int input_port_10_r(int offset) { return readinputport(10); }
        public static int input_port_11_r(int offset) { return readinputport(11); }
        public static int input_port_12_r(int offset) { return readinputport(12); }
        public static int input_port_13_r(int offset) { return readinputport(13); }
        public static int input_port_14_r(int offset) { return readinputport(14); }
        public static int input_port_15_r(int offset) { return readinputport(15); }

        void input_port_free(InputPort[] dst)
        {
            dst = null;
        }
        static int memcmp(byte[] b1, byte[] b2, int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (b1[i] < b2[i]) return -1;
                else if (b1[i] > b2[i]) return 1;
            }
            return 0;
        }
        void load_default_keys()
        {
            object f;


            osd_customize_inputport_defaults(inputport_defaults);
            for (int i = 0; i < inputport_defaults.Length; i++)
                inputport_defaults_backup[i] = inputport_defaults[i];
            f = osd_fopen("default", null, OSD_FILETYPE_CONFIG, 0);
            if (f != null)
            {
                byte[] buf = new byte[8];
                int version;

                /* read header */
                if (osd_fread(f, buf, 8) != 8)
                    goto getout;

                if (memcmp(buf, Encoding.Default.GetBytes(MAMEDEFSTRING_V8), 8) == 0)
                    version = 8;
                else
                    goto getout;	/* header invalid */

                for (; ; )
                {
                    uint type = 0;
                    uint[] def_seq = new uint[16];
                    uint[] seq = new uint[16];
                    int i;

                    if (readint(f, ref type) != 0)
                        goto getout;

                    if (seq_read(f, ref def_seq) != 0)
                        goto getout;
                    if (seq_read(f, ref seq) != 0)
                        goto getout;

                    i = 0;
                    while (inputport_defaults[i].type != (uint)ports.inptports.IPT_END)
                    {
                        if (inputport_defaults[i].type == type)
                        {
                            /* load stored settings only if the default hasn't changed */
                            if (seq_cmp(inputport_defaults[i].seq, def_seq) == 0)
                                seq_copy(inputport_defaults[i].seq, seq);
                        }

                        i++;
                    }
                }

            getout:
                osd_fclose(f);
            }
        }
        static int seq_read(object f, ref uint[] seq)
        {
            int j, len;
            uint i = 0;
            ushort w = 0;

            if (readword(f, ref w) != 0)
                return -1;

            len = w;
            seq_set_0(seq);
            for (j = 0; j < len; ++j)
            {
                if (readint(f, ref i) != 0)
                    return -1;
                seq[j] = savecode_to_code(i);
            }

            return 0;
        }

        static int readword(object f, ref ushort num)
        {
            byte[] b = new byte[2];
            if (osd_fread(f, b, 2) != 2) return -1;
            num = BitConverter.ToUInt16(b, 0);

            return 0;
        }
        static int readint(object f, ref uint num)
        {
            byte[] b = new byte[4];
            if (osd_fread(f, b, 4) != 4) return -1;
            num = BitConverter.ToUInt32(b, 0);

            return 0;
        }
        int load_input_port_settings()
        {
            object f;

            load_default_keys();
            if ((f = osd_fopen(Machine.gamedrv.name, null, OSD_FILETYPE_CONFIG, 0)) != null)
            {
                uint total, savedtotal=0;
                byte[] buf = new byte[8];
                int i;
                int version;

                int _in = 0;

                /* calculate the size of the array */
                total = 0;
                while (Machine.input_ports_default[_in].type != (uint)ports.inptports.IPT_END)
                {
                    total++;
                    _in++;
                }

                /* read header */
                if (osd_fread(f, buf, 8) != 8)
                    goto getout;

                if (memcmp(buf, Encoding.Default.GetBytes(MAMECFGSTRING_V8), 8) == 0)
                    version = 8;
                else
                    goto getout;	/* header invalid */

                /* read array size */
                if (readint(f, ref savedtotal) != 0)
                    goto getout;
                if (total != savedtotal)
                    goto getout;	/* different size */

                /* read the original settings and compare them with the ones defined in the driver */
                _in = 0;
                while (Machine.input_ports_default[_in].type != (uint)ports.inptports.IPT_END)
                {
                    InputPort saved = new InputPort();

                    if (input_port_read(f, ref saved) != 0)
                        goto getout;

                    if (Machine.input_ports_default[_in].mask != saved.mask ||
                        Machine.input_ports_default[_in].default_value != saved.default_value ||
                        Machine.input_ports_default[_in].type != saved.type ||
                        seq_cmp(Machine.input_ports_default[_in].seq, saved.seq) != 0)
                        goto getout;	/* the default values are different */

                    _in++;
                }

                /* read the current settings */
                _in = 0;
                while (Machine.input_ports[_in].type != (uint)ports.inptports.IPT_END)
                {
                    if (input_port_read(f, ref Machine.input_ports[_in]) != 0)
                        goto getout;
                    _in++;
                }

                /* Clear the coin & ticket counters/flags - LBO 042898 */
                for (i = 0; i < COIN_COUNTERS; i++)
                    coins[i] = lastcoin[i] = coinlockedout[i] = 0;
                dispensed_tickets = 0;

                /* read in the coin/ticket counters */
                for (i = 0; i < COIN_COUNTERS; i++)
                {
                    if (readint(f, ref coins[i]) != 0)
                        goto getout;
                }
                if (readint(f, ref dispensed_tickets) != 0)
                    goto getout;

                mixer_read_config(f);

            getout:
                osd_fclose(f);
            }
            /* All analog ports need initialization */
            {
                int i;
                for (i = 0; i < MAX_INPUT_PORTS; i++)
                    input_analog_init[i] = 1;
            }


            update_input_ports();

            /* if we didn't find a saved config, return 0 so the main core knows that it */
            /* is the first time the game is run and it should diplay the disclaimer. */
            if (f != null) return 1;
            else return 0;
        }
        static int input_port_read(object f, ref InputPort _in)
        {
            uint i = 0;
            ushort w = 0;
            if (readint(f, ref i) != 0)
                return -1;
            _in.type = i;

            if (readword(f, ref w) != 0)
                return -1;
            _in.mask = w;

            if (readword(f, ref w) != 0)
                return -1;
            _in.default_value = w;

            if (seq_read_ver(f, ref _in.seq) != 0)
                return -1;

            return 0;
        }
        static int seq_read_ver(object f, ref uint[] seq)
        {
            int j, len;
            uint i = 0;
            ushort w = 0;

            if (readword(f, ref w) != 0)
                return -1;

            len = w;
            seq_set_0(seq);
            for (j = 0; j < len; ++j)
            {
                if (readint(f, ref i) != 0)
                    return -1;
                seq[j] = savecode_to_code(i);
            }

            return 0;
        }
        const int MAX_INPUT_BITS = 1024;
        const int MAX_JOYSTICKS = 3;
        const int MAX_PLAYERS = 4;
        static int[] impulsecount = new int[MAX_INPUT_BITS];
        static int[] waspressed = new int[MAX_INPUT_BITS];
        static int update_serial_number = 1;
        static int[,] joyserial = new int[MAX_JOYSTICKS * MAX_PLAYERS, 4];
        void update_input_ports()
        {
            int port, ib;
            int _in;

            /* clear all the values before proceeding */
            for (port = 0; port < MAX_INPUT_PORTS; port++)
            {
                input_port_value[port] = 0;
                input_vblank[port] = 0;
                input_analog[port] = 0;
            }


            _in = 0;//Machine.input_ports;

            if (Machine.input_ports[_in].type == (InputCode)ports.inptports.IPT_END) return; 	/* nothing to do */

            /* make sure the InputPort definition is correct */
            if (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_PORT)
            {
                printf("Error in InputPort definition: expecting PORT_START\n");
                return;
            }
            else _in++;

            /* scan all the joystick ports */
            port = 0;
            while (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END && port < MAX_INPUT_PORTS)
            {
                while (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END && Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_PORT)
                {
                    if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) >= (InputCode)ports.inptports.IPT_JOYSTICK_UP &&
                        (Machine.input_ports[_in].type & ~ports.IPF_MASK) <= (InputCode)ports.inptports.IPT_JOYSTICKLEFT_RIGHT)
                    {
                        InputCode[] seq= input_port_seq(_in);

                        if (seq_get_1(seq) != 0 && seq_get_1(seq) != (int)InputCodes.CODE_NONE)
                        {
                            int joynum, joydir, player;

                            player = 0;
                            if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER2)
                                player = 1;
                            else if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER3)
                                player = 2;
                            else if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER4)
                                player = 3;

                            joynum = (int)(player * MAX_JOYSTICKS + ((Machine.input_ports[_in].type & ~ports.IPF_MASK) - (InputCode)ports.inptports.IPT_JOYSTICK_UP) / 4);
                            joydir = (int)(((Machine.input_ports[_in].type & ~ports.IPF_MASK) - (InputCode)ports.inptports.IPT_JOYSTICK_UP) % 4);

                            if (seq_pressed(seq))
                            {
                                if (joyserial[joynum, joydir] == 0)
                                    joyserial[joynum, joydir] = update_serial_number;
                            }
                            else
                                joyserial[joynum, joydir] = 0;
                        }
                    }
                    _in++;
                }

                port++;
                if (Machine.input_ports[_in].type == (InputCode)ports.inptports.IPT_PORT) _in++;
            }
            update_serial_number += 1;

            _in = 0;//Machine.input_ports;

            /* already made sure the InputPort definition is correct */
            _in++;


            /* scan all the input ports */
            port = 0;
            ib = 0;
            while (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END && port < MAX_INPUT_PORTS)
            {
                int start;


                /* first of all, scan the whole input port definition and build the */
                /* default value. I must do it before checking for input because otherwise */
                /* multiple keys associated with the same input bit wouldn't work (the bit */
                /* would be reset to its default value by the second entry, regardless if */
                /* the key associated with the first entry was pressed) */
                start = _in;// Machine.input_ports[_in];
                while (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END && Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_PORT)
                {
                    if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) != (InputCode)ports.inptports.IPT_DIPSWITCH_SETTING &&	/* skip dipswitch definitions */
                        (Machine.input_ports[_in].type & ~ports.IPF_MASK) != (InputCode)ports.inptports.IPT_EXTENSION)			/* skip analog extension fields */
                    {
                        input_port_value[port] = (ushort)((input_port_value[port] & ~Machine.input_ports[_in].mask) | (Machine.input_ports[_in].default_value & Machine.input_ports[_in].mask));
                    }

                    _in++;
                }

                /* now get back to the beginning of the input port and check the input bits. */
                for (_in = start;
                     Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END && Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_PORT;
                     _in++, ib++)
                {

                    if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) != (InputCode)ports.inptports.IPT_DIPSWITCH_SETTING &&	/* skip dipswitch definitions */
                            (Machine.input_ports[_in].type & ~ports.IPF_MASK) != (InputCode)ports.inptports.IPT_EXTENSION)		/* skip analog extension fields */
                    {
                        if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) == (InputCode)ports.inptports.IPT_VBLANK)
                        {
                            input_vblank[port] ^= Machine.input_ports[_in].mask;
                            input_port_value[port] ^= Machine.input_ports[_in].mask;
                            if (Machine.drv.vblank_duration == 0)
                                printf("Warning: you are using IPT_VBLANK with vblank_duration = 0. You need to increase vblank_duration for IPT_VBLANK to work.\n");
                        }
                        /* If it's an analog control, handle it appropriately */
                        else if (((Machine.input_ports[_in].type & ~ports.IPF_MASK) > (InputCode)ports.inptports.IPT_ANALOG_START)
                              && ((Machine.input_ports[_in].type & ~ports.IPF_MASK) < (InputCode)ports.inptports.IPT_ANALOG_END)) /* LBO 120897 */
                        {
                            input_analog[port] = _in;// Machine.input_ports[_in];
                            /* reset the analog port on first access */
                            if (input_analog_init[port] != 0)
                            {
                                input_analog_init[port] = 0;
                                input_analog_current_value[port] = input_analog_previous_value[port] = (int)(Machine.input_ports[_in].default_value * 100 / IP_GET_SENSITIVITY(_in));
                            }
                        }
                        else
                        {
                            InputCode[] seq;

                            seq = input_port_seq(_in);

                            if (seq_pressed(seq))
                            {
                                /* skip if coin input and it's locked out */
                                if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) >= (InputCode)ports.inptports.IPT_COIN1 &&
                                    (Machine.input_ports[_in].type & ~ports.IPF_MASK) <= (InputCode)ports.inptports.IPT_COIN4 &&
                                    coinlockedout[(Machine.input_ports[_in].type & ~ports.IPF_MASK) - (InputCode)ports.inptports.IPT_COIN1] != 0)
                                {
                                    continue;
                                }

                                /* if IPF_RESET set, reset the first CPU */
                                if ((Machine.input_ports[_in].type & ports.IPF_RESETCPU) != 0 && waspressed[ib] == 0)
                                    cpu_set_reset_line(0, PULSE_LINE);

                                if ((Machine.input_ports[_in].type & ports.IPF_IMPULSE) != 0)
                                {
                                    if (IP_GET_IMPULSE(_in) == 0)
                                        printf("error in input port definition: IPF_IMPULSE with length = 0\n");
                                    if (waspressed[ib] == 0)
                                        impulsecount[ib] = (int)IP_GET_IMPULSE(_in);
                                    /* the input bit will be toggled later */
                                }
                                else if ((Machine.input_ports[_in].type & ports.IPF_TOGGLE) != 0)
                                {
                                    if (waspressed[ib] == 0)
                                    {
                                        Machine.input_ports[_in].default_value ^= Machine.input_ports[_in].mask;
                                        input_port_value[port] ^= Machine.input_ports[_in].mask;
                                    }
                                }
                                else if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) >= (InputCode)ports.inptports.IPT_JOYSTICK_UP &&
                                        (Machine.input_ports[_in].type & ~ports.IPF_MASK) <= (InputCode)ports.inptports.IPT_JOYSTICKLEFT_RIGHT)
                                {

                                    int joynum, joydir, mask, player;


                                    player = 0;
                                    if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER2) player = 1;
                                    else if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER3) player = 2;
                                    else if ((Machine.input_ports[_in].type & ports.IPF_PLAYERMASK) == ports.IPF_PLAYER4) player = 3;

                                    joynum = (int)(player * MAX_JOYSTICKS + ((Machine.input_ports[_in].type & ~ports.IPF_MASK) - (InputCode)ports.inptports.IPT_JOYSTICK_UP) / 4);
                                    joydir = (int)(((Machine.input_ports[_in].type & ~ports.IPF_MASK) - (InputCode)ports.inptports.IPT_JOYSTICK_UP) % 4);

                                    mask = Machine.input_ports[_in].mask;


                                    /* avoid movement in two opposite directions */
                                    if (joyserial[joynum, joydir ^ 1] != 0)
                                        mask = 0;
                                    else if ((Machine.input_ports[_in].type & ports.IPF_4WAY) != 0)
                                    {
                                        int mru_dir = joydir;
                                        int mru_serial = 0;
                                        int dir;


                                        /* avoid diagonal movements, use mru button */
                                        for (dir = 0; dir < 4; dir++)
                                        {
                                            if (joyserial[joynum, dir] > mru_serial)
                                            {
                                                mru_serial = joyserial[joynum, dir];
                                                mru_dir = dir;
                                            }
                                        }

                                        if (mru_dir != joydir)
                                            mask = 0;
                                    }

                                    input_port_value[port] ^= (ushort)mask;
                                }
                                else
                                    input_port_value[port] ^= Machine.input_ports[_in].mask;

                                waspressed[ib] = 1;
                            }
                            else
                                waspressed[ib] = 0;

                            if ((Machine.input_ports[_in].type & ports.IPF_IMPULSE) != 0 && impulsecount[ib] > 0)
                            {
                                impulsecount[ib]--;
                                waspressed[ib] = 1;
                                input_port_value[port] ^= Machine.input_ports[_in].mask;
                            }
                        }
                    }
                }

                port++;
                if (Machine.input_ports[_in].type == (InputCode)ports.inptports.IPT_PORT) _in++;
            }

            //if (playback)
            //{
            //    int i;

            //    for (i = 0; i < MAX_INPUT_PORTS; i ++)
            //        readword(playback,&input_port_value[i]);
            //}
            //if (record)
            //{
            //    int i;

            //    for (i = 0; i < MAX_INPUT_PORTS; i ++)
            //        writeword(record,input_port_value[i]);
            //}

        }
        static uint[] ip_none = SEQ_DEF_1((InputCode)InputCodes.CODE_NONE);
        uint[] input_port_seq(int _in)
        {
            int i, type;

            while (seq_get_1(Machine.input_ports[_in].seq) == (int)InputCodes.CODE_PREVIOUS) _in--;

            if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) == (InputCode)ports.inptports.IPT_EXTENSION)
            {
                type = (int)(Machine.input_ports[_in - 1].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK));
                /* if port is disabled, or cheat with cheats disabled, return no key */
                if ((Machine.input_ports[_in - 1].type & ports.IPF_UNUSED) != 0 || (!options.cheat && (Machine.input_ports[_in - 1].type & ports.IPF_CHEAT) != 0))
                    return ip_none;
            }
            else
            {
                type = (int)(Machine.input_ports[_in].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK));
                /* if port is disabled, or cheat with cheats disabled, return no key */
                if ((Machine.input_ports[_in].type & ports.IPF_UNUSED) != 0 || (!options.cheat && (Machine.input_ports[_in].type & ports.IPF_CHEAT) != 0))
                    return ip_none;
            }

            if (seq_get_1(Machine.input_ports[_in].seq) != (int)InputCodes.CODE_DEFAULT)
                return Machine.input_ports[_in].seq;

            i = 0;

            while (inputport_defaults[i].type != (InputCode)ports.inptports.IPT_END &&
                    inputport_defaults[i].type != type)
                i++;

            if ((Machine.input_ports[_in].type & ~ports.IPF_MASK) == (InputCode)ports.inptports.IPT_EXTENSION)
                return inputport_defaults[i + 1].seq;
            else
                return inputport_defaults[i].seq;
        }
        uint[] input_port_seq(InputPort[] input_ports, int _in)
        {
            int i, type;

            while (seq_get_1(input_ports[_in].seq) == (int)InputCodes.CODE_PREVIOUS) _in--;

            if ((input_ports[_in].type & ~ports.IPF_MASK) == (InputCode)ports.inptports.IPT_EXTENSION)
            {
                type = (int)(input_ports[_in - 1].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK));
                /* if port is disabled, or cheat with cheats disabled, return no key */
                if ((input_ports[_in - 1].type & ports.IPF_UNUSED) != 0 || (!options.cheat && (input_ports[_in - 1].type & ports.IPF_CHEAT) != 0))
                    return ip_none;
            }
            else
            {
                type = (int)(input_ports[_in].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK));
                /* if port is disabled, or cheat with cheats disabled, return no key */
                if ((input_ports[_in].type & ports.IPF_UNUSED) != 0 || (!options.cheat && (input_ports[_in].type & ports.IPF_CHEAT) != 0))
                    return ip_none;
            }

            if (seq_get_1(input_ports[_in].seq) != (int)InputCodes.CODE_DEFAULT)
                return input_ports[_in].seq;

            i = 0;

            while (inputport_defaults[i].type != (InputCode)ports.inptports.IPT_END &&
                    inputport_defaults[i].type != type)
                i++;

            if ((input_ports[_in].type & ~ports.IPF_MASK) == (InputCode)ports.inptports.IPT_EXTENSION)
                return inputport_defaults[i + 1].seq;
            else
                return inputport_defaults[i].seq;
        }

        bool seq_pressed(InputCode[] code)
        {
            int j;
            bool res = true;
            bool invert = false;
            int count = 0;

            for (j = 0; j < SEQ_MAX; ++j)
            {
                switch (code[j])
                {
                    case (int)InputCodes.CODE_NONE:
                        return res && count != 0;
                    case (int)InputCodes.CODE_OR:
                        if (res && count != 0)
                            return true;
                        res = true;
                        count = 0;
                        break;
                    case (int)InputCodes.CODE_NOT:
                        invert = !invert;
                        break;
                    default:
                        if (res && (code_pressed(code[j])) == invert)
                            res = false;
                        invert = false;
                        ++count;
                        break;
                }
            }
            return res && count != 0;
        }
        uint IP_GET_IMPULSE(int port)
        {
            return ((Machine.input_ports[port].type >> 8) & 0xff);
        }
        uint IP_GET_SENSITIVITY(int port) { return (Machine.input_ports[port + 1].type >> 8) & 0xff; }
        static void writeint(object f, uint num)
        {
            osd_fwrite(f, BitConverter.GetBytes(num), 4);
        }
        static void writeword(object f, ushort num)
        {
            osd_fwrite(f, BitConverter.GetBytes(num), 2);
        }
        static void seq_write(object f, InputCode[] seq)
        {
            int j, len;
            for (len = 0; len < SEQ_MAX; ++len)
                if (seq[len] == (InputCode)InputCodes.CODE_NONE)
                    break;
            writeword(f, (ushort)len);
            for (j = 0; j < len; ++j)
            {
                writeint(f, code_to_savecode(seq[j]));
            }
        }
        static void save_default_keys()
        {
            object f;


            if ((f = osd_fopen("default", null, OSD_FILETYPE_CONFIG, 1)) != null)
            {
                int i;


                /* write header */
                osd_fwrite(f, Encoding.Default.GetBytes(MAMEDEFSTRING_V8), 8);

                i = 0;
                while (inputport_defaults[i].type != (InputCode)ports.inptports.IPT_END)
                {
                    writeint(f, inputport_defaults[i].type);

                    seq_write(f, inputport_defaults_backup[i].seq);
                    seq_write(f, inputport_defaults[i].seq);

                    i++;
                }

                osd_fclose(f);
            }
            for (int i = 0; i < inputport_defaults_backup.Length; i++)
                inputport_defaults[i] = inputport_defaults_backup[i];
            //Buffer.BlockCopy(inputport_defaults_backup, 0, inputport_defaults, 0,inputport_defaults_backup.Length);
        }
        static void input_port_write(object f, InputPort _in)
        {
            writeint(f, _in.type);
            writeword(f, _in.mask);
            writeword(f, _in.default_value);
            seq_write(f, _in.seq);
        }
        void save_input_port_settings()
        {

            object f;

            save_default_keys();

            if ((f = osd_fopen(Machine.gamedrv.name, null, OSD_FILETYPE_CONFIG, 1)) != null)
            {
                int _in;
                int total;
                int i;


                _in = 0;

                /* calculate the size of the array */
                total = 0;
                while (Machine.input_ports_default[_in].type != (InputCode)ports.inptports.IPT_END)
                {
                    total++;
                    _in++;
                }

                /* write header */
                osd_fwrite(f, Encoding.Default.GetBytes(MAMECFGSTRING_V8), 8);
                /* write array size */
                writeint(f, (uint)total);
                /* write the original settings as defined in the driver */
                _in = 0;
                while (Machine.input_ports_default[_in].type != (InputCode)ports.inptports.IPT_END)
                {
                    input_port_write(f, Machine.input_ports_default[_in]);
                    _in++;
                }
                /* write the current settings */
                _in = 0;
                while (Machine.input_ports[_in].type != (InputCode)ports.inptports.IPT_END)
                {
                    input_port_write(f, Machine.input_ports[_in]);
                    _in++;
                }

                /* write out the coin/ticket counters for this machine - LBO 042898 */
                for (i = 0; i < COIN_COUNTERS; i++)
                    writeint(f, coins[i]);
                writeint(f, dispensed_tickets);

                mixer_write_config(f);

                osd_fclose(f);
            }

        }
        uint[] input_port_type_seq(int type)
        {
            uint i;

            i = 0;

            while (inputport_defaults[i].type != (int)ports.inptports.IPT_END &&
                    inputport_defaults[i].type != type)
                i++;

            return inputport_defaults[i].seq;
        }
        void inputport_vblank_end()
        {
            int port;
            int i;


            for (port = 0; port < MAX_INPUT_PORTS; port++)
            {
                if (input_vblank[port] != 0)
                {
                    input_port_value[port] ^= input_vblank[port];
                    input_vblank[port] = 0;
                }
            }

            /* poll all the analog joysticks */
            osd_poll_joysticks();

            /* update the analog devices */
            for (i = 0; i < OSD_MAX_JOY_ANALOG; i++)
            {
                /* update the analog joystick position */
                analog_previous_x[i] = analog_current_x[i];
                analog_previous_y[i] = analog_current_y[i];
                osd_analogjoy_read(i, ref analog_current_x[i], ref analog_current_y[i]);

                /* update mouse/trackball position */
                osd_trak_read(i, ref mouse_delta_x[i], ref mouse_delta_y[i]);
            }

            for (i = 0; i < MAX_INPUT_PORTS; i++)
            {
                if (input_analog[i] != 0)
                {
                    update_analog_port(i);
                }
            }
        }
        uint IP_GET_DELTA(InputPort[] ports, int index) 
        {
            return (ports[index + 1].type >> 16) & 0xff;
        }
        static uint IP_GET_SENSITIVITY(InputPort[] ports, int index)
        {
            return (ports[index + 1].type >> 8) & 0xff;
        }
        ushort IP_GET_MIN(InputPort[] ports, int index)
        {
            return ports[index + 1].mask;
        }
        ushort IP_GET_MAX(InputPort[] ports, int index)
        {
            return (ports[index + 1].default_value);
        }
        void update_analog_port(int port)
        {
            int current, delta, type, sensitivity, min, max, default_value;
            int axis;
            bool is_stick;
            bool check_bounds;
            InputCode[] incseq;
            InputCode[] decseq;
            int keydelta;
            int player;

            /* get input definition */
            //_in = input_analog[port];

            /* if we're not cheating and this is a cheat-only port, bail */
            if (!options.cheat && (Machine.input_ports[input_analog[port]].type & ports.IPF_CHEAT) != 0) return;
            type = (int)(Machine.input_ports[input_analog[port]].type & ~ports.IPF_MASK);

            decseq = input_port_seq(port );
            incseq = input_port_seq(port + 1);

            keydelta = (int)IP_GET_DELTA(Machine.input_ports, input_analog[port]);// (int)(input_analog[port + +1].type >> 16) & 0xff;//IP_GET_DELTA

            switch (type)
            {
                case (int)ports.inptports.IPT_PADDLE:
                    axis = X_AXIS; is_stick = false; check_bounds = true; break;
                case (int)ports.inptports.IPT_PADDLE_V:
                    axis = Y_AXIS; is_stick = false; check_bounds = true; break;
                case (int)ports.inptports.IPT_DIAL:
                    axis = X_AXIS; is_stick = false; check_bounds = false; break;
                case (int)ports.inptports.IPT_DIAL_V:
                    axis = Y_AXIS; is_stick = false; check_bounds = false; break;
                case (int)ports.inptports.IPT_TRACKBALL_X:
                    axis = X_AXIS; is_stick = false; check_bounds = false; break;
                case (int)ports.inptports.IPT_TRACKBALL_Y:
                    axis = Y_AXIS; is_stick = false; check_bounds = false; break;
                case (int)ports.inptports.IPT_AD_STICK_X:
                    axis = X_AXIS; is_stick = true; check_bounds = true; break;
                case (int)ports.inptports.IPT_AD_STICK_Y:
                    axis = Y_AXIS; is_stick = true; check_bounds = true; break;
                case (int)ports.inptports.IPT_PEDAL:
                    axis = Y_AXIS; is_stick = false; check_bounds = true; break;
                default:
                    /* Use some defaults to prevent crash */
                    axis = X_AXIS; is_stick = false; check_bounds = false;
                    printf("Oops, polling non analog device in update_analog_port()????\n");
                    break;
            }


            sensitivity =(int) IP_GET_SENSITIVITY(Machine.input_ports,input_analog[port]);// (int)(input_analog[port + 1].type >> 8) & 0xff;//ip_get_sensitivity
            min = IP_GET_MIN(Machine.input_ports, input_analog[port]);// (input_analog[port + 1].mask);//IP_GET_MIN
            max = IP_GET_MAX(Machine.input_ports, input_analog[port]);//(input_analog[port +  1].default_value);//IP_GET_MAX
            default_value = Machine.input_ports[input_analog[port]].default_value * 100 / sensitivity;
            /* extremes can be either signed or unsigned */
            if (min > max)
            {
                if (Machine.input_ports[input_analog[port]].mask > 0xff) min = min - 0x10000;
                else min = min - 0x100;
            }

            input_analog_previous_value[port] = input_analog_current_value[port];

            /* if IPF_CENTER go back to the default position */
            /* sticks are handled later... */
            if ((Machine.input_ports[input_analog[port]].type & ports.IPF_CENTER) != 0 && (!is_stick))
                input_analog_current_value[port] = Machine.input_ports[input_analog[port]].default_value * 100 / sensitivity;

            current = input_analog_current_value[port];

            delta = 0;

            switch (Machine.input_ports[input_analog[port]].type & ports.IPF_PLAYERMASK)
            {
                case ports.IPF_PLAYER2: player = 1; break;
                case ports.IPF_PLAYER3: player = 2; break;
                case ports.IPF_PLAYER4: player = 3; break;
                case ports.IPF_PLAYER1:
                default: player = 0; break;
            }

            if (axis == X_AXIS)
                delta = mouse_delta_x[player];
            else
                delta = mouse_delta_y[player];

            if (seq_pressed(decseq)) delta -= keydelta;

            if (type != (int)ports.inptports.IPT_PEDAL)
            {
                if (seq_pressed(incseq)) delta += keydelta;
            }
            else
            {
                /* is this cheesy or what? */
                if (delta == 0 && seq_get_1(incseq) == (int)InputCodes.KEYCODE_Y) delta += keydelta;
                delta = -delta;
            }

            if ((Machine.input_ports[input_analog[port]].type & ports.IPF_REVERSE) != 0) delta = -delta;

            if (is_stick)
            {
                int _new, prev;

                /* center stick */
                if ((delta == 0) && (Machine.input_ports[input_analog[port]].type & ports.IPF_CENTER) != 0)
                {
                    if (current > default_value)
                        delta = -100 / sensitivity;
                    if (current < default_value)
                        delta = 100 / sensitivity;
                }

                /* An analog joystick which is not at zero position (or has just */
                /* moved there) takes precedence over all other computations */
                /* analog_x/y holds values from -128 to 128 (yes, 128, not 127) */

                if (axis == X_AXIS)
                {
                    _new = analog_current_x[player];
                    prev = analog_previous_x[player];
                }
                else
                {
                    _new = analog_current_y[player];
                    prev = analog_previous_y[player];
                }

                if ((_new != 0) || (_new - prev != 0))
                {
                    delta = 0;

                    if ((Machine.input_ports[input_analog[port]].type & ports.IPF_REVERSE) != 0)
                    {
                        _new = -_new;
                        prev = -prev;
                    }

                    /* apply sensitivity using a logarithmic scale */
                    if (Machine.input_ports[input_analog[port]].mask > 0xff)
                    {
                        if (_new > 0)
                        {
                            current = (int)((Math.Pow(_new / 32768.0, 100.0 / sensitivity) * (max - Machine.input_ports[input_analog[port]].default_value)
                                    + Machine.input_ports[input_analog[port]].default_value) * 100 / sensitivity);
                        }
                        else
                        {
                            current = (int)((Math.Pow(-_new / 32768.0, 100.0 / sensitivity) * (min - Machine.input_ports[input_analog[port]].default_value)
                                    + Machine.input_ports[input_analog[port]].default_value) * 100 / sensitivity);
                        }
                    }
                    else
                    {
                        if (_new > 0)
                        {
                            current = (int)((Math.Pow(_new / 128.0, 100.0 / sensitivity) * (max - Machine.input_ports[input_analog[port]].default_value)
                                    + Machine.input_ports[input_analog[port]].default_value) * 100 / sensitivity);
                        }
                        else
                        {
                            current = (int)((Math.Pow(-_new / 128.0, 100.0 / sensitivity) * (min - Machine.input_ports[input_analog[port]].default_value)
                                    + Machine.input_ports[input_analog[port]].default_value) * 100 / sensitivity);
                        }
                    }
                }
            }

            current += delta;

            if (check_bounds)
            {
                if ((current * sensitivity + 50) / 100 < min)
                    current = (min * 100 + sensitivity / 2) / sensitivity;
                if ((current * sensitivity + 50) / 100 > max)
                    current = (max * 100 + sensitivity / 2) / sensitivity;
            }

            input_analog_current_value[port] = current;
        }
        string input_port_name(InputPort[] inputports, int _in)
        {
            int i;
            uint type;

            if (inputports[_in].name != ports.IP_NAME_DEFAULT) return inputports[_in].name;

            i = 0;

            if ((inputports[_in].type & ~ports.IPF_MASK) == (int)ports.inptports.IPT_EXTENSION)
                type = inputports[_in - 1].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK);
            else
                type = inputports[_in].type & (~ports.IPF_MASK | ports.IPF_PLAYERMASK);

            while (inputport_defaults[i].type != (int)ports.inptports.IPT_END &&
                    inputport_defaults[i].type != type)
                i++;

            if ((inputports[_in].type & ~ports.IPF_MASK) == (int)ports.inptports.IPT_EXTENSION)
                return inputport_defaults[i + 1].name;
            else
                return inputport_defaults[i].name;
        }

    }
}
