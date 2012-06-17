using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace xnamame036.mame
{
    public class ports : roms
    {
        static List<Mame.InputPortTiny> ip;
        public static string IP_NAME_DEFAULT = "-1";
        public static Dictionary<string, string> ipdn_defaultstrings = new Dictionary<string, string>();

        public const string Off = "Off";
        public const string On = "On";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Unknown = "Unknown";
        public const string Coinage = "Coinage";   
        public const string Coin_A = "Coin A";
        public const string Coin_B = "Coin B";
        public const string Difficulty = "Difficulty";
        public const string Free_Play = "Free Play";
        public const string Demo_Sounds = "Demo Sounds";
        public const string Flip_Screen = "Flip Screen";
        public const string Cabinet = "Cabinet";
        public const string Lives = "Lives";
        public const string Upright = "Upright";
        public const string Cocktail = "Cocktail";
        public const string Bonus_Life = "Bonus Life";

        public static string DEF_STR(string key) { return ipdn_defaultstrings[key]; }
        static ports()
        {
            ipdn_defaultstrings["Off"] = "Off";
            ipdn_defaultstrings["On"] = "On";
            ipdn_defaultstrings["No"] = "No";
            ipdn_defaultstrings["Yes"] = "Yes";
            ipdn_defaultstrings["Lives"] = "Lives";
            ipdn_defaultstrings["Bonus Life"] = "Bonus Life";
            ipdn_defaultstrings["Difficulty"] = "Difficulty";
            ipdn_defaultstrings["Demo Sounds"] = "Demo Sounds";
            ipdn_defaultstrings["Coinage"] = "Coinage";
            ipdn_defaultstrings["Coin A"] = "Coin A";
            ipdn_defaultstrings["Coin B"] = "Coin B";
            ipdn_defaultstrings["9C_1C"] = "9 Coins/1 Credit";
            ipdn_defaultstrings["8C_1C"] = "8 Coins/1 Credit";
            ipdn_defaultstrings["7C_1C"] = "7 Coins/1 Credit";
            ipdn_defaultstrings["6C_1C"] = "6 Coins/1 Credit";
            ipdn_defaultstrings["5C_1C"] = "5 Coins/1 Credit";
            ipdn_defaultstrings["4C_1C"] = "4 Coins/1 Credit";
            ipdn_defaultstrings["3C_1C"] = "3 Coins/1 Credit";
            ipdn_defaultstrings["8C_3C"] = "8 Coins/3 Credits";
            ipdn_defaultstrings["4C_2C"] = "4 Coins/2 Credits";
            ipdn_defaultstrings["2C_1C"] = "2 Coins/1 Credit";
            ipdn_defaultstrings["5C_3C"] = "5 Coins/3 Credits";
            ipdn_defaultstrings["3C_2C"] = "3 Coins/2 Credits";
            ipdn_defaultstrings["4C_3C"] = "4 Coins/3 Credits";
            ipdn_defaultstrings["4C_4C"] = "4 Coins/4 Credits";
            ipdn_defaultstrings["3C_3C"] = "3 Coins/3 Credits";
            ipdn_defaultstrings["2C_2C"] = "2 Coins/2 Credits";
            ipdn_defaultstrings["1C_1C"] = "1 Coin/1 Credit";
            ipdn_defaultstrings["4C_5C"] = "4 Coins/5 Credits";
            ipdn_defaultstrings["3C_4C"] = "3 Coins/4 Credits";
            ipdn_defaultstrings["2C_3C"] = "2 Coins/3 Credits";
            ipdn_defaultstrings["4C_7C"] = "4 Coins/7 Credits";
            ipdn_defaultstrings["2C_4C"] = "2 Coins/4 Credits";
            ipdn_defaultstrings["1C_2C"] = "1 Coin/2 Credits";
            ipdn_defaultstrings["2C_5C"] = "2 Coins/5 Credits";
            ipdn_defaultstrings["2C_6C"] = "2 Coins/6 Credits";
            ipdn_defaultstrings["1C_3C"] = "1 Coin/3 Credits";
            ipdn_defaultstrings["2C_7C"] = "2 Coins/7 Credits";
            ipdn_defaultstrings["2C_8C"] = "2 Coins/8 Credits";
            ipdn_defaultstrings["1C_4C"] = "1 Coin/4 Credits";
            ipdn_defaultstrings["1C_5C"] = "1 Coin/5 Credits";
            ipdn_defaultstrings["1C_6C"] = "1 Coin/6 Credits";
            ipdn_defaultstrings["1C_7C"] = "1 Coin/7 Credits";
            ipdn_defaultstrings["1C_8C"] = "1 Coin/8 Credits";
            ipdn_defaultstrings["1C_9C"] = "1 Coin/9 Credits";
            ipdn_defaultstrings["Free Play"] = "Free Play";
            ipdn_defaultstrings["Cabinet"] = "Cabinet";
            ipdn_defaultstrings["Upright"] = "Upright";
            ipdn_defaultstrings["Cocktail"] = "Cocktail";
            ipdn_defaultstrings["Flip Screen"] = "Flip Screen";
            ipdn_defaultstrings["Service Mode"] = "Service Mode";
            ipdn_defaultstrings["Unused"] = "Unused";
            ipdn_defaultstrings["Unknown"] = "Unknown";
        }
        public enum inptports
        {
            IPT_END = 1, IPT_PORT,
            /* use IPT_JOYSTICK for panels where the player has one single joystick */
            IPT_JOYSTICK_UP, IPT_JOYSTICK_DOWN, IPT_JOYSTICK_LEFT, IPT_JOYSTICK_RIGHT,
            /* use IPT_JOYSTICKLEFT and IPT_JOYSTICKRIGHT for dual joystick panels */
            IPT_JOYSTICKRIGHT_UP, IPT_JOYSTICKRIGHT_DOWN, IPT_JOYSTICKRIGHT_LEFT, IPT_JOYSTICKRIGHT_RIGHT,
            IPT_JOYSTICKLEFT_UP, IPT_JOYSTICKLEFT_DOWN, IPT_JOYSTICKLEFT_LEFT, IPT_JOYSTICKLEFT_RIGHT,
            IPT_BUTTON1, IPT_BUTTON2, IPT_BUTTON3, IPT_BUTTON4,	/* action buttons */
            IPT_BUTTON5, IPT_BUTTON6, IPT_BUTTON7, IPT_BUTTON8, IPT_BUTTON9,

            /* analog inputs */
            /* the "arg" field contains the default sensitivity expressed as a percentage */
            /* (100 = default, 50 = half, 200 = twice) */
            IPT_ANALOG_START,
            IPT_PADDLE, IPT_PADDLE_V,
            IPT_DIAL, IPT_DIAL_V,
            IPT_TRACKBALL_X, IPT_TRACKBALL_Y,
            IPT_AD_STICK_X, IPT_AD_STICK_Y,
            IPT_PEDAL,
            IPT_ANALOG_END,

            IPT_START1, IPT_START2, IPT_START3, IPT_START4,	/* start buttons */
            IPT_COIN1, IPT_COIN2, IPT_COIN3, IPT_COIN4,	/* coin slots */
            IPT_SERVICE1, IPT_SERVICE2, IPT_SERVICE3, IPT_SERVICE4,	/* service coin */
            IPT_SERVICE, IPT_TILT,
            IPT_DIPSWITCH_NAME, IPT_DIPSWITCH_SETTING,
            /* Many games poll an input bit to check for vertical blanks instead of using */
            /* interrupts. This special value allows you to handle that. If you set one of the */
            /* input bits to this, the bit will be inverted while a vertical blank is happening. */
            IPT_VBLANK,
            IPT_UNKNOWN,
            IPT_EXTENSION,	/* this is an extension on the previous InputPort, not a real inputport. */
            /* It is used to store additional parameters for analog inputs */

            /* the following are special codes for user interface handling - not to be used by drivers! */
            IPT_UI_CONFIGURE,
            IPT_UI_ON_SCREEN_DISPLAY,
            IPT_UI_PAUSE,
            IPT_UI_RESET_MACHINE,
            IPT_UI_SHOW_GFX,
            IPT_UI_FRAMESKIP_DEC,
            IPT_UI_FRAMESKIP_INC,
            IPT_UI_THROTTLE,
            IPT_UI_SHOW_FPS,
            IPT_UI_SNAPSHOT,
            IPT_UI_TOGGLE_CHEAT,
            IPT_UI_UP,
            IPT_UI_DOWN,
            IPT_UI_LEFT,
            IPT_UI_RIGHT,
            IPT_UI_SELECT,
            IPT_UI_CANCEL,
            IPT_UI_PAN_UP, IPT_UI_PAN_DOWN, IPT_UI_PAN_LEFT, IPT_UI_PAN_RIGHT,
            IPT_UI_SHOW_PROFILER,
            IPT_UI_SHOW_COLORS,
            IPT_UI_TOGGLE_UI,
            __ipt_max
        };
        public const ushort IP_ACTIVE_HIGH = 0x0000;
        public const ushort IP_ACTIVE_LOW = 0xffff;

        public static ushort IP_KEY_NONE = (ushort)Mame.InputCodes.CODE_NONE;
        public static ushort IP_JOY_NONE = (ushort)Mame.InputCodes.CODE_NONE;

        public const uint IPT_UNUSED = IPF_UNUSED;
        public const uint IPT_SPECIAL = IPT_UNUSED;	/* special meaning handled by custom functions */

        public const uint IPF_MASK = 0xffffff00;
        public const uint IPF_UNUSED = 0x80000000;	/* The bit is not used by this game, but is used */
        
        /* by other games running on the same hardware. */
        /* This is different from IPT_UNUSED, which marks */
        /* bits not connected to anything. */
        public const uint IPF_COCKTAIL = IPF_PLAYER2;	/* the bit is used in cocktail mode only */

        public const uint IPF_CHEAT = 0x40000000;	/* Indicates that the input bit is a "cheat" key */
        /* (providing invulnerabilty, level advance, and */
        /* so on). MAME will not recognize it when the */
        /* -nocheat command line option is specified. */

        public const uint IPF_PLAYERMASK = 0x00030000;	/* use IPF_PLAYERn if more than one person can */
        public const uint IPF_PLAYER1 = 0;         	/* play at the same time. The IPT_ should be the same */
        public const uint IPF_PLAYER2 = 0x00010000;	/* for all players (e.g. IPT_BUTTON1 | IPF_PLAYER2) */
        public const uint IPF_PLAYER3 = 0x00020000;	/* IPF_PLAYER1 is the default and can be left out to */
        public const uint IPF_PLAYER4 = 0x00030000;	/* increase readability. */

        public const uint IPF_8WAY = 0;         	/* Joystick modes of operation. 8WAY is the default, */
        public const uint IPF_4WAY = 0x00080000;	/* it prevents left/right or up/down to be pressed at */
        public const uint IPF_2WAY = 0;         	/* the same time. 4WAY prevents diagonal directions. */
        /* 2WAY should be used for joysticks wich move only */
        /* on one axis (e.g. Battle Zone) */

        public const uint IPF_IMPULSE = 0x00100000;	/* When this is set, when the key corrisponding to */
        /* the input bit is pressed it will be reported as */
        /* pressed for a certain number of video frames and */
        /* then released, regardless of the real status of */
        /* the key. This is useful e.g. for some coin inputs. */
        /* The number of frames the signal should stay active */
        /* is specified in the "arg" field. */
        public const uint IPF_TOGGLE = 0x00200000;	/* When this is set, the key acts as a toggle - press */
        /* it once and it goes on, press it again and it goes off. */
        /* useful e.g. for sone Test Mode dip switches. */
        public const uint IPF_REVERSE = 0x00400000;	/* By default, analog inputs like IPT_TRACKBALL increase */
        /* when going right/up. This flag inverts them. */

        public const uint IPF_CENTER = 0x00800000;	/* always preload in.default, autocentering the STICK/TRACKBALL */

        public const uint IPF_CUSTOM_UPDATE = 0x01000000; /* normally, analog ports are updated when they are accessed. */
        /* When this flag is set, they are never updated automatically, */
        /* it is the responsibility of the driver to call */
        /* update_analog_port(int port). */

        public const uint IPF_RESETCPU = 0x02000000;	/* when the key is pressed, reset the first CPU */

        public static void INPUT_PORTS_START(string name)
        {
            ip = new List<Mame.InputPortTiny>();
        }
        public static void PORT_START(string name = null)
        {
            ip.Add(new Mame.InputPortTiny(0, 0, (ushort)inptports.IPT_PORT, null));
        }
        public static void PORT_BIT(ushort mask, ushort def, uint type)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, type, IP_NAME_DEFAULT));
        }
        public static void PORT_BIT_IMPULSE(ushort mask, ushort def, uint type,uint duration)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, type | IPF_IMPULSE | ((duration & 0xff) << 8), IP_NAME_DEFAULT));
        }
        public static void PORT_BITX(ushort mask, ushort def, uint type, string name, ushort key, ushort joy)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, type, name));
            PORT_CODE(key, joy);
        }
        static uint IPF_SENSITIVITY(uint percent) { return ((percent & 0xff) << 8); }
        static uint IPF_DELTA(uint val) { return ((val & 0xff) << 16); }

        public static void PORT_ANALOG(ushort mask, ushort def, uint type, uint sensitivity, uint delta, ushort min, ushort max)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, type, IP_NAME_DEFAULT));
            ip.Add(new Mame.InputPortTiny(min, max, (uint)inptports.IPT_EXTENSION | IPF_SENSITIVITY(sensitivity) | IPF_DELTA(delta), IP_NAME_DEFAULT));
        }
        public static void PORT_ANALOGX(ushort mask, ushort def, uint type, uint sensitivity, uint delta, ushort min, ushort max, ushort keydec, ushort keyinc, ushort joydec, ushort joyinc)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, type, IP_NAME_DEFAULT));
            ip.Add(new Mame.InputPortTiny(min, max, (ushort)inptports.IPT_EXTENSION | IPF_SENSITIVITY(sensitivity) | IPF_DELTA(delta), IP_NAME_DEFAULT));
            PORT_CODE(keydec, joydec);
            PORT_CODE(keyinc, joyinc);
        }

        public static void PORT_CODE(ushort key, ushort joy)
        {
            ip.Add(new Mame.InputPortTiny(key, joy, (ushort)inptports.IPT_EXTENSION, null));
        }
        public static void PORT_DIPNAME(ushort mask, ushort def, string name)
        {
            ip.Add(new Mame.InputPortTiny(mask, def, (ushort)inptports.IPT_DIPSWITCH_NAME, name));
        }
        public static void PORT_DIPSETTING(ushort def, string name)
        {
            ip.Add(new Mame.InputPortTiny(0, def, (ushort)inptports.IPT_DIPSWITCH_SETTING, name));
        }
        public static void PORT_SERVICE(ushort mask, ushort def)
        {
            PORT_BITX(mask, (ushort)(mask & def), (ushort)inptports.IPT_DIPSWITCH_NAME | IPF_TOGGLE, ipdn_defaultstrings["Service Mode"], (ushort)Mame.InputCodes.KEYCODE_F2, IP_JOY_NONE);
            PORT_DIPSETTING((ushort)(mask & def), ipdn_defaultstrings["Off"]);
            PORT_DIPSETTING((ushort)(mask & ~def), ipdn_defaultstrings["On"]);
        }
        public static Mame.InputPortTiny[] INPUT_PORTS_END
        {
            get
            {
                ip.Add(new Mame.InputPortTiny(0, 0, (ushort)inptports.IPT_END, null));
                return ip.ToArray();
            }
        }
    }
}
