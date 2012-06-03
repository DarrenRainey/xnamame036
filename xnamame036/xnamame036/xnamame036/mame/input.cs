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
        /* Flags used in saving codes to file */
        const uint SAVECODE_FLAGS_TYPE_NONE = 0x00000000;
        const uint SAVECODE_FLAGS_TYPE_STANDARD = 0x10000000; /* standard code */
        const uint SAVECODE_FLAGS_TYPE_KEYBOARD_OS = 0x20000000; /* keyboard os depend code */
        const uint SAVECODE_FLAGS_TYPE_JOYSTICK_OS = 0x30000000; /* joystick os depend code */
        const uint SAVECODE_FLAGS_TYPE_MASK = 0xF0000000;

        const int SEQ_MAX = 16;
        class KeyboardInfo
        {
            public KeyboardInfo(string name, uint code, uint standardcode)
            {
                this.name = name; this.code = code; this.standardcode = standardcode;
            }
            public string name;
            public uint code, standardcode;
        }
        class JoystickInfo
        {
            public string name;
            public uint code, standardcode;
        }

        public struct code_info
        {
            public bool memory;
            public uint oscode, type;
        }
        public static code_info[] code_map;
        public static uint code_mac;
        public enum InputCodes
        {
            /* key */
            KEYCODE_A, KEYCODE_B, KEYCODE_C, KEYCODE_D, KEYCODE_E, KEYCODE_F,
            KEYCODE_G, KEYCODE_H, KEYCODE_I, KEYCODE_J, KEYCODE_K, KEYCODE_L,
            KEYCODE_M, KEYCODE_N, KEYCODE_O, KEYCODE_P, KEYCODE_Q, KEYCODE_R,
            KEYCODE_S, KEYCODE_T, KEYCODE_U, KEYCODE_V, KEYCODE_W, KEYCODE_X,
            KEYCODE_Y, KEYCODE_Z, KEYCODE_0, KEYCODE_1, KEYCODE_2, KEYCODE_3,
            KEYCODE_4, KEYCODE_5, KEYCODE_6, KEYCODE_7, KEYCODE_8, KEYCODE_9,
            KEYCODE_0_PAD, KEYCODE_1_PAD, KEYCODE_2_PAD, KEYCODE_3_PAD, KEYCODE_4_PAD,
            KEYCODE_5_PAD, KEYCODE_6_PAD, KEYCODE_7_PAD, KEYCODE_8_PAD, KEYCODE_9_PAD,
            KEYCODE_F1, KEYCODE_F2, KEYCODE_F3, KEYCODE_F4, KEYCODE_F5,
            KEYCODE_F6, KEYCODE_F7, KEYCODE_F8, KEYCODE_F9, KEYCODE_F10,
            KEYCODE_F11, KEYCODE_F12,
            KEYCODE_ESC, KEYCODE_TILDE, KEYCODE_MINUS, KEYCODE_EQUALS, KEYCODE_BACKSPACE,
            KEYCODE_TAB, KEYCODE_OPENBRACE, KEYCODE_CLOSEBRACE, KEYCODE_ENTER, KEYCODE_COLON,
            KEYCODE_QUOTE, KEYCODE_BACKSLASH, KEYCODE_BACKSLASH2, KEYCODE_COMMA, KEYCODE_STOP,
            KEYCODE_SLASH, KEYCODE_SPACE, KEYCODE_INSERT, KEYCODE_DEL,
            KEYCODE_HOME, KEYCODE_END, KEYCODE_PGUP, KEYCODE_PGDN, KEYCODE_LEFT,
            KEYCODE_RIGHT, KEYCODE_UP, KEYCODE_DOWN,
            KEYCODE_SLASH_PAD, KEYCODE_ASTERISK, KEYCODE_MINUS_PAD, KEYCODE_PLUS_PAD,
            KEYCODE_DEL_PAD, KEYCODE_ENTER_PAD, KEYCODE_PRTSCR, KEYCODE_PAUSE,
            KEYCODE_LSHIFT, KEYCODE_RSHIFT, KEYCODE_LCONTROL, KEYCODE_RCONTROL,
            KEYCODE_LALT, KEYCODE_RALT, KEYCODE_SCRLOCK, KEYCODE_NUMLOCK, KEYCODE_CAPSLOCK,
            KEYCODE_LWIN, KEYCODE_RWIN, KEYCODE_MENU,

            /* joy */
            JOYCODE_1_LEFT, JOYCODE_1_RIGHT, JOYCODE_1_UP, JOYCODE_1_DOWN,
            JOYCODE_1_BUTTON1, JOYCODE_1_BUTTON2, JOYCODE_1_BUTTON3,
            JOYCODE_1_BUTTON4, JOYCODE_1_BUTTON5, JOYCODE_1_BUTTON6,
            JOYCODE_1_BUTTON7, JOYCODE_1_BUTTON8, JOYCODE_1_BUTTON9, JOYCODE_1_BUTTON10,
            JOYCODE_2_LEFT, JOYCODE_2_RIGHT, JOYCODE_2_UP, JOYCODE_2_DOWN,
            JOYCODE_2_BUTTON1, JOYCODE_2_BUTTON2, JOYCODE_2_BUTTON3,
            JOYCODE_2_BUTTON4, JOYCODE_2_BUTTON5, JOYCODE_2_BUTTON6,
            JOYCODE_2_BUTTON7, JOYCODE_2_BUTTON8, JOYCODE_2_BUTTON9, JOYCODE_2_BUTTON10,
            JOYCODE_3_LEFT, JOYCODE_3_RIGHT, JOYCODE_3_UP, JOYCODE_3_DOWN,
            JOYCODE_3_BUTTON1, JOYCODE_3_BUTTON2, JOYCODE_3_BUTTON3,
            JOYCODE_3_BUTTON4, JOYCODE_3_BUTTON5, JOYCODE_3_BUTTON6,
            JOYCODE_4_LEFT, JOYCODE_4_RIGHT, JOYCODE_4_UP, JOYCODE_4_DOWN,
            JOYCODE_4_BUTTON1, JOYCODE_4_BUTTON2, JOYCODE_4_BUTTON3,
            JOYCODE_4_BUTTON4, JOYCODE_4_BUTTON5, JOYCODE_4_BUTTON6,

            __code_max, /* Temination of standard code */

            /* special */
            CODE_NONE = 0x8000, /* no code, also marker of sequence end */
            CODE_OTHER, /* OS code not mapped to any other code */
            CODE_DEFAULT, /* special for input port definitions */
            CODE_PREVIOUS, /* special for input port definitions */
            CODE_NOT, /* operators for sequences */
            CODE_OR,/* operators for sequences */
            IP_KEY_DEFAULT=CODE_DEFAULT,
            IP_JOY_DEFAULT=CODE_DEFAULT,
            IP_KEY_PREVIOUS=CODE_PREVIOUS,
            IP_JOY_PREVIOUS=CODE_PREVIOUS,
            IP_KEY_NONE=CODE_NONE,
            IP_JOY_NONE=CODE_NONE
        };
        uint __code_joy_first = (uint)InputCodes.JOYCODE_1_LEFT;
        uint __code_joy_last = (uint)InputCodes.JOYCODE_4_BUTTON6;
        uint __code_key_first = (uint)InputCodes.KEYCODE_A;
        uint __code_key_last = (uint)InputCodes.KEYCODE_MENU;
        /* Subtype of codes */
        const uint CODE_TYPE_NONE = 0U; /* code not assigned */
        const uint CODE_TYPE_KEYBOARD_OS = 1U; /* os depend code */
        const uint CODE_TYPE_KEYBOARD_STANDARD = 2U; /* standard code */
        const uint CODE_TYPE_JOYSTICK_OS = 3U; /* os depend code */
        const uint CODE_TYPE_JOYSTICK_STANDARD = 4U; /* standard code */

        int code_init()
        {

            /* allocate */
            code_map = new code_info[(int)InputCodes.__code_max];

            code_mac = 0;

            /* insert all known codes */
            for (int i = 0; i < (int)InputCodes.__code_max; ++i)
            {
                code_map[code_mac].memory = false;
                code_map[code_mac].oscode = 0; /* not used */

                if (__code_key_first <= i && i <= __code_key_last)
                    code_map[code_mac].type = CODE_TYPE_KEYBOARD_STANDARD;
                else if (__code_joy_first <= i && i <= __code_joy_last)
                    code_map[code_mac].type = CODE_TYPE_JOYSTICK_STANDARD;
                else
                    code_map[code_mac].type = CODE_TYPE_NONE; /* never happen */
                ++code_mac;
            }

            return 0;
        }
        static void seq_set_0(InputCode[] a)
        {
            for (int j = 0; j < SEQ_MAX; ++j)
                a[j] = CODE_TYPE_NONE;
        }

        static void seq_set_1(InputCode[] a, InputCode code)
        {
            int j;
            a[0] = code;
            for (j = 1; j < SEQ_MAX; ++j)
                a[j] = (int)InputCodes.CODE_NONE;
        }

        static void seq_set_2(InputCode[] a, InputCode code1, InputCode code2)
        {
            int j;
            a[0] = code1;
            a[1] = code2;
            for (j = 2; j < SEQ_MAX; ++j)
                a[j] = (int)InputCodes.CODE_NONE;
        }

        static void seq_set_3(InputCode[] a, InputCode code1, InputCode code2, InputCode code3)
        {
            int j;
            a[0] = code1;
            a[1] = code2;
            a[2] = code3;
            for (j = 3; j < SEQ_MAX; ++j)
                a[j] = (int)InputCodes.CODE_NONE;
        }
        uint code_read_async()
        {
            uint i;

            /* Update the table */
            internal_code_update();

            for (i = 0; i < code_mac; ++i)
                if (code_pressed_memory(i))
                    return i;


            return (uint)InputCodes.CODE_NONE;
        }
        static KeyboardInfo internal_code_find_keyboard_standard_os(uint oscode)
        {
            foreach (KeyboardInfo keyinfo in osd_get_key_list())
            {
                if (keyinfo.code == oscode && keyinfo.standardcode != (InputCode)InputCodes.CODE_OTHER)
                    return keyinfo;
            }
            return null;
        }
        static JoystickInfo internal_code_find_joystick_standard_os(uint oscode)
        {
            foreach (JoystickInfo joyinfo in osd_get_joy_list())
            {
                if (joyinfo == null) break;
                if (joyinfo.code == oscode && joyinfo.standardcode != (InputCode)InputCodes.CODE_OTHER)
                    return joyinfo;
            }
            return null;
        }

        /* Find a osdepend code in the table */
        static uint code_find_os(uint oscode, uint type)
        {
            //const struct KeyboardInfo *keyinfo;
            //const struct JoystickInfo *joyinfo;

            /* Search on the main table */
            for (uint i = (int)InputCodes.__code_max; i < code_mac; ++i)
                if (code_map[i].type == type && code_map[i].oscode == oscode)
                    return i;

            /* Search in the OSD tables for a standard code */
            switch (type)
            {
                case CODE_TYPE_KEYBOARD_OS:
                    KeyboardInfo keyinfo = internal_code_find_keyboard_standard_os(oscode);
                    if (keyinfo != null)
                        return keyinfo.standardcode;
                    break;
                case CODE_TYPE_JOYSTICK_OS:
                    JoystickInfo joyinfo = internal_code_find_joystick_standard_os(oscode);
                    if (joyinfo != null)
                        return joyinfo.standardcode;
                    break;
            }

            /* os code not found */
            return (int)InputCodes.CODE_NONE;
        }
        /* Add a new osdepend code in the table */
        static void code_add_os(uint oscode, uint type)
        {
            Array.Resize(ref code_map, (int)code_mac + 1);
            code_map[code_mac].memory = false;
            code_map[code_mac].oscode = oscode;
            code_map[code_mac].type = type;
            ++code_mac;
        }

        void internal_code_update()
        {
            /* add only osdepend code because all standard codes are already present */
            foreach (KeyboardInfo keyinfo in osd_get_key_list())
            {
                if (keyinfo.standardcode == (uint)InputCodes.CODE_OTHER)
                    if (code_find_os(keyinfo.code, CODE_TYPE_KEYBOARD_OS) == (uint)InputCodes.CODE_NONE)
                        code_add_os(keyinfo.code, CODE_TYPE_KEYBOARD_OS);
            }

            foreach (JoystickInfo joyinfo in osd_get_joy_list())
            {
                if (joyinfo == null) break;
                if (joyinfo.standardcode == (uint)InputCodes.CODE_OTHER)
                    if (code_find_os(joyinfo.code, CODE_TYPE_JOYSTICK_OS) == (uint)InputCodes.CODE_NONE)
                        code_add_os(joyinfo.code, CODE_TYPE_JOYSTICK_OS);
            }
        }
        KeyboardInfo internal_code_find_keyboard_standard(uint code)
        {
            foreach (KeyboardInfo keyinfo in osd_get_key_list())
            {
                if (keyinfo.standardcode == code) return keyinfo;
            }
            return null;
        }
        bool internal_code_pressed(uint code)
        {
            KeyboardInfo keyinfo;
            JoystickInfo joyinfo;
            switch (code_map[code].type)
            {
                case CODE_TYPE_KEYBOARD_STANDARD:
                    keyinfo = internal_code_find_keyboard_standard(code);
                    if (keyinfo != null)
                        return osd_is_key_pressed((int)keyinfo.code);
                    break;
                case CODE_TYPE_KEYBOARD_OS:
                    keyinfo = internal_code_find_keyboard_os(code);
                    if (keyinfo != null)
                        return osd_is_key_pressed((int)keyinfo.code);
                    break;
                case CODE_TYPE_JOYSTICK_STANDARD:
                    joyinfo = internal_code_find_joystick_standard(code);
                    if (joyinfo != null)
                        return osd_is_joy_pressed((int)joyinfo.code);
                    break;
                case CODE_TYPE_JOYSTICK_OS:
                    joyinfo = internal_code_find_joystick_os(code);
                    if (joyinfo != null)
                        return osd_is_joy_pressed((int)joyinfo.code);
                    break;
            }
            return false;
        }
        JoystickInfo internal_code_find_joystick_standard(uint code)
        {
            foreach (JoystickInfo joyinfo in osd_get_joy_list())
            {
                if (joyinfo == null) break;
                if (joyinfo.standardcode == code)
                    return joyinfo;
            }
            return null;
        }
        JoystickInfo internal_code_find_joystick_os(uint code)
        {
            foreach (JoystickInfo joyinfo in osd_get_joy_list())
            {
                if (joyinfo.standardcode == (uint)InputCodes.CODE_OTHER && joyinfo.code == code_map[code].oscode)
                    return joyinfo;
            }
            return null;
        }
        bool code_pressed_memory(uint code)
        {
            bool pressed;


            pressed = internal_code_pressed(code);

            if (pressed)
            {
                if (!code_map[code].memory)
                {
                    code_map[code].memory = true;
                }
                else
                    pressed = false;
            }
            else
                code_map[code].memory = false;


            return pressed;
        }
        uint code_read_sync()
        {
            uint code;
            uint oscode;

            /* now let the OS process it */
            oscode = (uint)osd_wait_keypress();

            /* convert the code */
            code = keyoscode_to_code(oscode);

            while (code == (uint)InputCodes.CODE_NONE)
                code = code_read_async();

            return code;
        }
        static uint keyoscode_to_code(uint oscode)
        {
            InputCode code;

            if (oscode == OSD_KEY_NONE)
                return (uint)InputCodes.CODE_NONE;

            code = code_find_os(oscode, CODE_TYPE_KEYBOARD_OS);

            /* insert if missing */
            if (code == (uint)InputCodes.CODE_NONE)
            {
                code_add_os(oscode, CODE_TYPE_KEYBOARD_OS);
                /* this fail only if the realloc call in code_add_os fail */
                code = code_find_os(oscode, CODE_TYPE_KEYBOARD_OS);
            }

            return code;
        }
        static InputCode joyoscode_to_code(uint oscode)
        {
            InputCode code = code_find_os(oscode, CODE_TYPE_JOYSTICK_OS);

            /* insert if missing */
            if (code == (InputCode)InputCodes.CODE_NONE)
            {
                code_add_os(oscode, CODE_TYPE_JOYSTICK_OS);
                /* this fail only if the realloc call in code_add_os fail */
                code = code_find_os(oscode, CODE_TYPE_JOYSTICK_OS);
            }

            return code;
        }

        int keyboard_read_sync() { return (int)code_read_sync(); }

        bool code_pressed(uint code)
        {
            bool pressed;

            pressed = internal_code_pressed(code);

            return pressed;
        }
        bool keyboard_pressed(int code)
        {
            return code_pressed((uint)code);
        }
        InputCode seq_get_1(InputCode[] a)
        {
            return a[0];
        }
        struct ui_info
        {
            public int memory;
        }
        ui_info[] ui_map = new ui_info[(int)ports.inptports.__ipt_max];
        bool input_ui_pressed(int code)
        {
            bool pressed;

            pressed = seq_pressed(input_port_type_seq(code));

            if (pressed)
            {
                if (ui_map[code].memory == 0)
                {
                    ui_map[code].memory = 1;
                }
                else
                    pressed = false;
            }
            else
                ui_map[code].memory = 0;

            return pressed;
        }
        static int counter1, inputdelay;
        bool input_ui_pressed_repeat(int code, int speed)
        {
            bool pressed;


            pressed = seq_pressed(input_port_type_seq(code));

            if (pressed)
            {
                if (ui_map[code].memory == 0)
                {
                    ui_map[code].memory = 1;
                    inputdelay = 3;
                    counter1 = 0;
                }
                else if (++counter1 > inputdelay * speed * Machine.drv.frames_per_second / 60)
                {
                    inputdelay = 1;
                    counter = 0;
                }
                else
                    pressed = false;
            }
            else
                ui_map[code].memory = 0;


            return pressed;
        }
        static int counter2;
        static int keydelay;
        bool code_pressed_memory_repeat(InputCode code, int speed)
        {
            bool pressed;


            pressed = internal_code_pressed(code);

            if (pressed)
            {
                if (!code_map[code].memory)
                {
                    code_map[code].memory = true;
                    keydelay = 3;
                    counter2 = 0;
                }
                else if (++counter2 > keydelay * speed * Machine.drv.frames_per_second / 60)
                {
                    keydelay = 1;
                    counter = 0;
                }
                else
                    pressed = false;
            }
            else
                code_map[code].memory = false;


            return pressed;
        }
        KeyboardInfo internal_code_find_keyboard_os(uint code)
        {
            foreach (KeyboardInfo keyinfo in osd_get_key_list())
            {
                if (keyinfo.standardcode == (uint)InputCodes.CODE_OTHER && keyinfo.code == code_map[code].oscode)
                    return keyinfo;
            }
            return null;
        }

        bool keyboard_pressed_memory(int code)
        {
            return code_pressed_memory((uint)code);
        }
        string internal_code_name(uint code)
        {
            KeyboardInfo keyinfo;
            JoystickInfo joyinfo;
            switch (code_map[code].type)
            {
                case CODE_TYPE_KEYBOARD_STANDARD:
                    keyinfo = internal_code_find_keyboard_standard(code);
                    if (keyinfo != null)
                        return keyinfo.name;
                    break;
                case CODE_TYPE_KEYBOARD_OS:
                    keyinfo = internal_code_find_keyboard_os(code);
                    if (keyinfo != null)
                        return keyinfo.name;
                    break;
                case CODE_TYPE_JOYSTICK_STANDARD:
                    joyinfo = internal_code_find_joystick_standard(code);
                    if (joyinfo != null)
                        return joyinfo.name;
                    break;
                case CODE_TYPE_JOYSTICK_OS:
                    joyinfo = internal_code_find_joystick_os(code);
                    if (joyinfo != null)
                        return joyinfo.name;
                    break;
            }
            return "n/a";
        }
        string code_name(InputCode code)
        {
            if (code < code_mac)
                return internal_code_name(code);

            switch (code)
            {
                case (InputCode)InputCodes.CODE_NONE: return "None";
                case (InputCode)InputCodes.CODE_NOT: return "not";
                case (InputCode)InputCodes.CODE_OR: return "or";
            }

            return "n/a";
        }
        void seq_name(uint[] code, ref string buffer, uint max)
        {
            int j;
            StringBuilder dest = new StringBuilder();
            for (j = 0; j < SEQ_MAX; ++j)
            {
                string name;

                if ((code)[j] == (uint)InputCodes.CODE_NONE)
                    break;

                if (j != 0 && 1 + 1 <= max)
                {
                    dest.Append(' ');
                    //dest += 1;
                    max -= 1;
                }

                name = code_name(code[j]);
                if (name == null)
                    break;

                if (name.Length + 1 <= max)
                {
                    dest.Append(name);
                    //dest += strlen(name);
                    max -= (uint)name.Length;
                }
            }

            if (dest.ToString() == buffer && 4 + 1 <= max)
                dest.Append("None");
            else

                buffer = dest.ToString();
        }
        static uint[] record_seq = new uint[SEQ_MAX];
        static int record_count;
        static long record_last;
        const int RECORD_TIME = 10000000 * 2 / 3;
        void seq_read_async_start()
        {

            record_count = 0;
            record_last = DateTime.Now.Ticks;

            /* reset code memory, otherwise this memory may interferes with the input memory */
            for (uint i = 0; i < code_mac; ++i)
                code_map[i].memory = true;
        }
        /* Record a key/joy sequence
	return <0 if more input is needed
	return ==0 if sequence succesfully recorded
	return >0 if aborted
*/
        int seq_read_async(uint[] seq, bool first)
        {
            InputCode newkey;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                return 1;

            if (record_count == SEQ_MAX
                || (record_count > 0 && DateTime.Now.Ticks > record_last + RECORD_TIME))
            {
                int k = 0;
                if (!first)
                {
                    /* search the first space free */
                    while (k < SEQ_MAX && (seq)[k] != (InputCode)InputCodes.CODE_NONE)
                        ++k;
                }

                /* if no space restart */
                if (k + record_count + ((k != 0) ? 1 : 0) > SEQ_MAX)
                    k = 0;

                /* insert */
                if (k + record_count + ((k != 0) ? 1 : 0) <= SEQ_MAX)
                {
                    int j;
                    if (k != 0)
                        (seq)[k++] = (InputCode)InputCodes.CODE_OR;
                    for (j = 0; j < record_count; ++j, ++k)
                        (seq)[k] = record_seq[j];
                }
                /* fill to end */
                while (k < SEQ_MAX)
                {
                    (seq)[k] = (InputCode)InputCodes.CODE_NONE;
                    ++k;
                }

                if (!seq_valid(seq))
                    seq_set_1(seq, (InputCode)InputCodes.CODE_NONE);

                return 0;
            }

            newkey = code_read_async();

            if (newkey != (InputCode)InputCodes.CODE_NONE)
            {
                /* if code is duplicate negate the code */
                if (record_count != 0 && newkey == record_seq[record_count - 1])
                    record_seq[record_count - 1] = (InputCode)InputCodes.CODE_NOT;

                record_seq[record_count++] = newkey;
                record_last = DateTime.Now.Ticks;
            }

            return -1;
        }
        bool seq_valid(uint[] seq)
        {
            bool positive = false;
            bool pred_not = false;
            bool operand = false;
            for (int j = 0; j < SEQ_MAX; ++j)
            {
                switch ((seq)[j])
                {
                    case (uint)InputCodes.CODE_NONE:
                        break;
                    case (uint)InputCodes.CODE_OR:
                        if (!operand || !positive)
                            return false;
                        pred_not = false;
                        positive = false;
                        operand = false;
                        break;
                    case (uint)InputCodes.CODE_NOT:
                        if (pred_not)
                            return false;
                        pred_not = !pred_not;
                        operand = false;
                        break;
                    default:
                        if (!pred_not)
                            positive = true;
                        pred_not = false;
                        operand = true;
                        break;
                }
            }
            return positive && operand;
        }
        /* Convert one code to one saved code */
        static uint code_to_savecode(InputCode code)
        {
            if (code < (InputCode)InputCodes.__code_max || code >= code_mac)
                /* if greather than code_mac is a special CODE like CODE_OR */
                return code | SAVECODE_FLAGS_TYPE_STANDARD;

            switch (code_map[code].type)
            {
                case CODE_TYPE_KEYBOARD_OS: return code_map[code].oscode | SAVECODE_FLAGS_TYPE_KEYBOARD_OS;
                case CODE_TYPE_JOYSTICK_OS: return code_map[code].oscode | SAVECODE_FLAGS_TYPE_JOYSTICK_OS;
            }

            /* never happen */

            return 0;
        }
        static void seq_set_0(InputSeq a)
        {
            int j;
            for (j = 0; j < SEQ_MAX; ++j)
                a[j] = (uint)InputCodes.CODE_NONE;
        }

        static void seq_set_1(InputSeq a, InputCode code)
        {
            int j;
            a[0] = code;
            for (j = 1; j < SEQ_MAX; ++j)
                a[j] = (uint)InputCodes.CODE_NONE;
        }

        static void seq_set_2(InputSeq a, InputCode code1, InputCode code2)
        {
            int j;
            a[0] = code1;
            a[1] = code2;
            for (j = 2; j < SEQ_MAX; ++j)
                a[j] = (uint)InputCodes.CODE_NONE;
        }

        static void seq_set_3(InputSeq a, InputCode code1, InputCode code2, InputCode code3)
        {
            int j;
            a[0] = code1;
            a[1] = code2;
            a[2] = code3;
            for (j = 3; j < SEQ_MAX; ++j)
                a[j] = (uint)InputCodes.CODE_NONE;
        }

        static void seq_copy(uint[] a, uint[] b)
        {
            int j;
            for (j = 0; j < SEQ_MAX; ++j)
                a[j] = b[j];
        }

        static int seq_cmp(uint[] a, uint[] b)
        {
            int j;
            for (j = 0; j < SEQ_MAX; ++j)
                if (a[j] != b[j])
                    return -1;
            return 0;
        }
        static InputCode savecode_to_code(uint savecode)
        {
            uint type = savecode & SAVECODE_FLAGS_TYPE_MASK;
            uint code = savecode & ~SAVECODE_FLAGS_TYPE_MASK;

            switch (type)
            {
                case SAVECODE_FLAGS_TYPE_STANDARD:
                    return code;
                case SAVECODE_FLAGS_TYPE_KEYBOARD_OS:
                    return keyoscode_to_code(code);
                case SAVECODE_FLAGS_TYPE_JOYSTICK_OS:
                    return joyoscode_to_code(code);
            }

            /* never happen */

            return (InputCode)InputCodes.CODE_NONE;
        }

    }
}
