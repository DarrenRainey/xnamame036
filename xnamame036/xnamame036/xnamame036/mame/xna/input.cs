using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace xnamame036.mame
{
    partial class Mame
    {
        const byte num_joysticks = 4;

        int joystick = JOY_TYPE_AUTODETECT;
        static int DICODE(int keycode) { return (keycode) & 0xff; }
        static uint KEYCODE(int dik, int vk, int ascii) { return (uint)((dik) | ((vk) << 8) | ((ascii) << 16)); }
        static int[,] key_trans_table = 
            {
                {(int)InputCodes.KEYCODE_ESC, 			(int)Keys.Escape,			27,27},
           	{ (int)InputCodes.KEYCODE_1, 			(int)Keys.D1,				'1',			'1' },
	{ (int)InputCodes.KEYCODE_2, 			(int)Keys.D2,				'2',			'2' },
	{ (int)InputCodes.KEYCODE_3, 			(int)Keys.D3,				'3',			'3' },
	{ (int)InputCodes.KEYCODE_4, 			(int)Keys.D4,				'4',			'4' },
	{ (int)InputCodes.KEYCODE_5, 			(int)Keys.D5,				'5',			'5' },
	{ (int)InputCodes.KEYCODE_6, 			(int)Keys.D6,				'6',			'6' },
	{ (int)InputCodes.KEYCODE_7, 			(int)Keys.D7,				'7',			'7' },
	{ (int)InputCodes.KEYCODE_8, 			(int)Keys.D8,				'8',			'8' },
	{ (int)InputCodes.KEYCODE_9, 			(int)Keys.D9,				'9',			'9' },
	{ (int)InputCodes.KEYCODE_0, 			(int)Keys.D0,				'0',			'0' },
	{ (int)InputCodes.KEYCODE_MINUS, 		(int)Keys.Subtract, 			'-',			'-' },
	//{ KEYCODE_EQUALS, 		(int)Keys.oem,		 	0xbb,			'=' },
	{ (int)InputCodes.KEYCODE_BACKSPACE,	(int)Keys.Back, 			(int)Keys.Back, 		8 },
	{ (int)InputCodes.KEYCODE_TAB, 			(int)Keys.Tab, 			(int)Keys.Tab, 		9 },
	{ (int)InputCodes.KEYCODE_Q, 			(int)Keys.Q,				'Q',			'Q' },
	{ (int)InputCodes.KEYCODE_W, 			(int)Keys.W,				'W',			'W' },
	{ (int)InputCodes.KEYCODE_E, 			(int)Keys.E,				'E',			'E' },
	{ (int)InputCodes.KEYCODE_R, 			(int)Keys.R,				'R',			'R' },
	{ (int)InputCodes.KEYCODE_T, 			(int)Keys.T,				'T',			'T' },
	{ (int)InputCodes.KEYCODE_Y, 			(int)Keys.Y,				'Y',			'Y' },
	{ (int)InputCodes.KEYCODE_U, 			(int)Keys.U,				'U',			'U' },
	{ (int)InputCodes.KEYCODE_I, 			(int)Keys.I,				'I',			'I' },
	{ (int)InputCodes.KEYCODE_O, 			(int)Keys.O,				'O',			'O' },
	{ (int)InputCodes.KEYCODE_P, 			(int)Keys.P,				'P',			'P' },
	{ (int)InputCodes.KEYCODE_OPENBRACE,	(int)Keys.OemOpenBrackets, 		'[',			'[' },
	{ (int)InputCodes.KEYCODE_CLOSEBRACE,	(int)Keys.OemCloseBrackets, 		']',			']' },
	{ (int)InputCodes.KEYCODE_ENTER, 		(int)Keys.Enter, 		13, 		13 },
	{ (int)InputCodes.KEYCODE_LCONTROL, 	(int)Keys.LeftControl, 		0, 	0 },
	{ (int)InputCodes.KEYCODE_A, 			(int)Keys.A,				'A',			'A' },
	{ (int)InputCodes.KEYCODE_S, 			(int)Keys.S,				'S',			'S' },
	{ (int)InputCodes.KEYCODE_D, 			(int)Keys.D,				'D',			'D' },
	{ (int)InputCodes.KEYCODE_F, 			(int)Keys.F,				'F',			'F' },
	{ (int)InputCodes.KEYCODE_G, 			(int)Keys.G,				'G',			'G' },
	{ (int)InputCodes.KEYCODE_H, 			(int)Keys.H,				'H',			'H' },
	{ (int)InputCodes.KEYCODE_J, 			(int)Keys.J,				'J',			'J' },
	{ (int)InputCodes.KEYCODE_K, 			(int)Keys.K,				'K',			'K' },
	{ (int)InputCodes.KEYCODE_L, 			(int)Keys.L,				'L',			'L' },
	{ (int)InputCodes.KEYCODE_COLON, 		(int)Keys.OemSemicolon,		';',			';' },
	{ (int)InputCodes.KEYCODE_QUOTE, 		(int)Keys.OemQuotes,		'\'',			'\'' },
	{ (int)InputCodes.KEYCODE_TILDE, 		(int)Keys.OemTilde, 			'`',			'`' },
	{ (int)InputCodes.KEYCODE_LSHIFT, 		(int)Keys.LeftShift, 		0, 		0 },
	{ (int)InputCodes.KEYCODE_BACKSLASH,	(int)Keys.OemBackslash, 		'\\',			'\\' },
	{ (int)InputCodes.KEYCODE_Z, 			(int)Keys.Z,				'Z',			'Z' },
	{ (int)InputCodes.KEYCODE_X, 			(int)Keys.X,				'X',			'X' },
	{ (int)InputCodes.KEYCODE_C, 			(int)Keys.C,				'C',			'C' },
	{ (int)InputCodes.KEYCODE_V, 			(int)Keys.V,				'V',			'V' },
	{ (int)InputCodes.KEYCODE_B, 			(int)Keys.B,				'B',			'B' },
	{ (int)InputCodes.KEYCODE_N, 			(int)Keys.N,				'N',			'N' },
	{ (int)InputCodes.KEYCODE_M, 			(int)Keys.M,				'M',			'M' },
	{ (int)InputCodes.KEYCODE_COMMA, 		(int)Keys.OemComma,			0xbc,			',' },
	{ (int)InputCodes.KEYCODE_STOP, 		(int)Keys.OemPeriod, 		0xbe,			'.' },
	{ (int)InputCodes.KEYCODE_SLASH, 		(int)Keys.Divide, 			0xbf,			'/' },
	{ (int)InputCodes.KEYCODE_RSHIFT, 		(int)Keys.RightShift, 		0, 		0 },
	{ (int)InputCodes.KEYCODE_ASTERISK, 	(int)Keys.Multiply, 		'*',	'*' },
	{ (int)InputCodes.KEYCODE_LALT, 		(int)Keys.LeftAlt, 			0, 		0 },
	{ (int)InputCodes.KEYCODE_SPACE, 		(int)Keys.Space, 			' ',		' ' },
	{ (int)InputCodes.KEYCODE_CAPSLOCK, 	(int)Keys.CapsLock, 		0, 	0 },
	{ (int)InputCodes.KEYCODE_F1, 			(int)Keys.F1,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F2, 			(int)Keys.F2,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F3, 			(int)Keys.F3,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F4, 			(int)Keys.F4,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F5, 			(int)Keys.F5,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F6, 			(int)Keys.F6,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F7, 			(int)Keys.F7,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F8, 			(int)Keys.F8,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F9, 			(int)Keys.F9,				0, 			0 },
	{ (int)InputCodes.KEYCODE_F10, 			(int)Keys.F10,			0, 		0 },
	{ (int)InputCodes.KEYCODE_NUMLOCK, 		(int)Keys.NumLock,		0, 	0 },
	{ (int)InputCodes.KEYCODE_SCRLOCK, 		(int)Keys.Scroll,			0, 		0 },
	{ (int)InputCodes.KEYCODE_7_PAD, 		(int)Keys.NumPad7,		0, 	0 },
	{ (int)InputCodes.KEYCODE_8_PAD, 		(int)Keys.NumPad8,		0, 	0 },
	{ (int)InputCodes.KEYCODE_9_PAD, 		(int)Keys.NumPad9,		0, 	0 },
//	{ (int)InputCodes.KEYCODE_MINUS_PAD,	(int)Keys.num,		VK_SUBTRACT, 	0 },
	{ (int)InputCodes.KEYCODE_4_PAD, 		(int)Keys.NumPad4,		0, 	0 },
	{ (int)InputCodes.KEYCODE_5_PAD, 		(int)Keys.NumPad5,		0, 	0 },
	{ (int)InputCodes.KEYCODE_6_PAD, 		(int)Keys.NumPad6,		0, 	0 },
	//{ KEYCODE_PLUS_PAD, 	DIK_ADD,			VK_ADD, 		0 },
	{ (int)InputCodes.KEYCODE_1_PAD, 		(int)Keys.NumPad1,		0, 	0 },
	{ (int)InputCodes.KEYCODE_2_PAD, 		(int)Keys.NumPad2,		0, 	0 },
	{ (int)InputCodes.KEYCODE_3_PAD, 		(int)Keys.NumPad3,		0, 	0 },
	{ (int)InputCodes.KEYCODE_0_PAD, 		(int)Keys.NumPad0,		0, 	0 },
	//{ KEYCODE_DEL_PAD, 		DIK_DECIMAL,		VK_DECIMAL, 	0 },
	{ (int)InputCodes.KEYCODE_F11, 			(int)Keys.F11,			0, 		0 },
	{ (int)InputCodes.KEYCODE_F12, 			(int)Keys.F12,			0, 		0 },
	//{ KEYCODE_OTHER, 		DIK_F13,			VK_F13, 		0 },
	//{ KEYCODE_OTHER, 		DIK_F14,			VK_F14, 		0 },
	//{ KEYCODE_OTHER, 		DIK_F15,			VK_F15, 		0 },
	//{ KEYCODE_ENTER_PAD,	DIK_NUMPADENTER,	VK_RETURN, 		0 },
	{ (int)InputCodes.KEYCODE_RCONTROL, 	(int)Keys.RightControl,		0, 	0 },
	{ (int)InputCodes.KEYCODE_SLASH_PAD,	(int)Keys.Divide,			0, 		0 },
	//{ KEYCODE_PRTSCR, 		DIK_SYSRQ, 			0, 				0 },
	{ (int)InputCodes.KEYCODE_RALT, 		(int)Keys.RightAlt,			0, 		0 },
	{ (int)InputCodes.KEYCODE_HOME, 		(int)Keys.Home,			0, 		0 },
	{ (int)InputCodes.KEYCODE_UP, 			(int)Keys.Up,				0, 			0 },
	{ (int)InputCodes.KEYCODE_PGUP, 		(int)Keys.PageUp,			0, 		0 },
	{ (int)InputCodes.KEYCODE_LEFT, 		(int)Keys.Left,			0, 		0 },
	{ (int)InputCodes.KEYCODE_RIGHT, 		(int)Keys.Right,			0, 		0 },
	{ (int)InputCodes.KEYCODE_END, 			(int)Keys.End,			0, 		0 },
	{ (int)InputCodes.KEYCODE_DOWN, 		(int)Keys.Down,			0, 		0 },
	{ (int)InputCodes.KEYCODE_PGDN, 		(int)Keys.PageDown,			0, 		0 },
	{ (int)InputCodes.KEYCODE_INSERT, 		(int)Keys.Insert,			0, 		0 },
	{ (int)InputCodes.KEYCODE_DEL, 			(int)Keys.Delete,			0, 		0 },
	{ (int)InputCodes.KEYCODE_LWIN, 		(int)Keys.LeftWindows,			0, 		0 },
	{ (int)InputCodes.KEYCODE_RWIN, 		(int)Keys.RightWindows,			0, 		0 },
	//{ KEYCODE_MENU, 		DIK_APPS,			VK_APPS, 		0 }
 };

        static int JOYCODE(int joy, int stick, int axis_or_button, int dir) { return ((((dir) & 0x03) << 14) | (((axis_or_button) & 0x3f) << 8) | (((stick) & 0x1f) << 3) | (((joy) & 0x07) << 0)); }
        static int[][] joyequiv =
{
	new int[]{ JOYCODE(1,1,1,1),	(int)InputCodes.JOYCODE_1_LEFT },
	new int[]{ JOYCODE(1,1,1,2),	(int)InputCodes.JOYCODE_1_RIGHT },
	new int[]{ JOYCODE(1,1,2,1),	(int)InputCodes.JOYCODE_1_UP },
	new int[]{ JOYCODE(1,1,2,2),	(int)InputCodes.JOYCODE_1_DOWN },
	new int[]{ JOYCODE(1,0,1,0),	(int)InputCodes.JOYCODE_1_BUTTON1 },
	new int[]{ JOYCODE(1,0,2,0),	(int)InputCodes.JOYCODE_1_BUTTON2 },
	new int[]{ JOYCODE(1,0,3,0),	(int)InputCodes.JOYCODE_1_BUTTON3 },
	new int[]{ JOYCODE(1,0,4,0),	(int)InputCodes.JOYCODE_1_BUTTON4 },
	new int[]{ JOYCODE(1,0,5,0),	(int)InputCodes.JOYCODE_1_BUTTON5 },
	new int[]{ JOYCODE(1,0,6,0),	(int)InputCodes.JOYCODE_1_BUTTON6 },
	new int[]{ JOYCODE(1,0,7,0),	(int)InputCodes.JOYCODE_1_BUTTON7 },
	new int[]{ JOYCODE(1,0,8,0),	(int)InputCodes.JOYCODE_1_BUTTON8 },
	new int[]{ JOYCODE(1,0,9,0),	(int)InputCodes.JOYCODE_1_BUTTON9 },
	new int[]{ JOYCODE(1,0,10,0),	(int)InputCodes.JOYCODE_1_BUTTON10 },
    


	new int[]{ JOYCODE(2,1,1,1),	(int)InputCodes.JOYCODE_2_LEFT },
	new int[]{ JOYCODE(2,1,1,2),	(int)InputCodes.JOYCODE_2_RIGHT },
	new int[]{ JOYCODE(2,1,2,1),	(int)InputCodes.JOYCODE_2_UP },
	new int[]{ JOYCODE(2,1,2,2),	(int)InputCodes.JOYCODE_2_DOWN },
	new int[]{ JOYCODE(2,0,1,0),	(int)InputCodes.JOYCODE_2_BUTTON1 },
	new int[]{ JOYCODE(2,0,2,0),	(int)InputCodes.JOYCODE_2_BUTTON2 },
	new int[]{ JOYCODE(2,0,3,0),	(int)InputCodes.JOYCODE_2_BUTTON3 },
	new int[]{ JOYCODE(2,0,4,0),	(int)InputCodes.JOYCODE_2_BUTTON4 },
	new int[]{ JOYCODE(2,0,5,0),	(int)InputCodes.JOYCODE_2_BUTTON5 },
	new int[]{ JOYCODE(2,0,6,0),	(int)InputCodes.JOYCODE_2_BUTTON6 },
	new int[]{ JOYCODE(3,1,1,1),	(int)InputCodes.JOYCODE_3_LEFT },
	new int[]{ JOYCODE(3,1,1,2),	(int)InputCodes.JOYCODE_3_RIGHT },
	new int[]{ JOYCODE(3,1,2,1),	(int)InputCodes.JOYCODE_3_UP },
	new int[]{ JOYCODE(3,1,2,2),	(int)InputCodes.JOYCODE_3_DOWN },
	new int[]{ JOYCODE(3,0,1,0),	(int)InputCodes.JOYCODE_3_BUTTON1 },
	new int[]{ JOYCODE(3,0,2,0),	(int)InputCodes.JOYCODE_3_BUTTON2 },
	new int[]{ JOYCODE(3,0,3,0),	(int)InputCodes.JOYCODE_3_BUTTON3 },
	new int[]{ JOYCODE(3,0,4,0),	(int)InputCodes.JOYCODE_3_BUTTON4 },
	new int[]{ JOYCODE(3,0,5,0),	(int)InputCodes.JOYCODE_3_BUTTON5 },
	new int[]{ JOYCODE(3,0,6,0),	(int)InputCodes.JOYCODE_3_BUTTON6 },
	new int[]{ JOYCODE(4,1,1,1),	(int)InputCodes.JOYCODE_4_LEFT },
	new int[]{ JOYCODE(4,1,1,2),	(int)InputCodes.JOYCODE_4_RIGHT },
	new int[]{ JOYCODE(4,1,2,1),	(int)InputCodes.JOYCODE_4_UP },
	new int[]{ JOYCODE(4,1,2,2),	(int)InputCodes.JOYCODE_4_DOWN },
	new int[]{ JOYCODE(4,0,1,0),	(int)InputCodes.JOYCODE_4_BUTTON1 },
	new int[]{ JOYCODE(4,0,2,0),	(int)InputCodes.JOYCODE_4_BUTTON2 },
	new int[]{ JOYCODE(4,0,3,0),	(int)InputCodes.JOYCODE_4_BUTTON3 },
	new int[]{ JOYCODE(4,0,4,0),	(int)InputCodes.JOYCODE_4_BUTTON4 },
	new int[]{ JOYCODE(4,0,5,0),	(int)InputCodes.JOYCODE_4_BUTTON5 },
	new int[]{ JOYCODE(4,0,6,0),	(int)InputCodes.JOYCODE_4_BUTTON6 },
	new int[]{ 0,0 }
};

        static KeyboardInfo[] keylist = {
            new KeyboardInfo("A",(int)Keys.A,(int)InputCodes.KEYCODE_A),
            new KeyboardInfo("B",(int)Keys.B,(int)InputCodes.KEYCODE_B),
            new KeyboardInfo("C",(int)Keys.C,(int)InputCodes.KEYCODE_C),
            new KeyboardInfo("D",(int)Keys.D,(int)InputCodes.KEYCODE_D),
            new KeyboardInfo("E",(int)Keys.E,(int)InputCodes.KEYCODE_E),
            new KeyboardInfo("F",(int)Keys.F,(int)InputCodes.KEYCODE_F),
            new KeyboardInfo("G",(int)Keys.G,(int)InputCodes.KEYCODE_G),
            new KeyboardInfo("H",(int)Keys.H,(int)InputCodes.KEYCODE_H),
            new KeyboardInfo("I",(int)Keys.I,(int)InputCodes.KEYCODE_I),
            new KeyboardInfo("J",(int)Keys.J,(int)InputCodes.KEYCODE_J),
            new KeyboardInfo("K",(int)Keys.K,(int)InputCodes.KEYCODE_K),
            new KeyboardInfo("L",(int)Keys.L,(int)InputCodes.KEYCODE_L),
            new KeyboardInfo("M",(int)Keys.M,(int)InputCodes.KEYCODE_M),
            new KeyboardInfo("N",(int)Keys.N,(int)InputCodes.KEYCODE_N),
            new KeyboardInfo("O",(int)Keys.O,(int)InputCodes.KEYCODE_O),
            new KeyboardInfo("P",(int)Keys.P,(int)InputCodes.KEYCODE_P),
            new KeyboardInfo("Q",(int)Keys.Q,(int)InputCodes.KEYCODE_Q),
            new KeyboardInfo("R",(int)Keys.R,(int)InputCodes.KEYCODE_R),
            new KeyboardInfo("S",(int)Keys.S,(int)InputCodes.KEYCODE_S),
            new KeyboardInfo("T",(int)Keys.T,(int)InputCodes.KEYCODE_T),
            new KeyboardInfo("U",(int)Keys.U,(int)InputCodes.KEYCODE_U),
            new KeyboardInfo("V",(int)Keys.V,(int)InputCodes.KEYCODE_V),
            new KeyboardInfo("W",(int)Keys.W,(int)InputCodes.KEYCODE_W),
            new KeyboardInfo("X",(int)Keys.X,(int)InputCodes.KEYCODE_X),
            new KeyboardInfo("Y",(int)Keys.Y,(int)InputCodes.KEYCODE_Y),
            new KeyboardInfo("Z",(int)Keys.Z,(int)InputCodes.KEYCODE_Z),
            new KeyboardInfo("0",(int)Keys.D0,(int)InputCodes.KEYCODE_0),
            new KeyboardInfo("1",(int)Keys.D1,(int)InputCodes.KEYCODE_1),
            new KeyboardInfo("2",(int)Keys.D2,(int)InputCodes.KEYCODE_2),
            new KeyboardInfo("3",(int)Keys.D3,(int)InputCodes.KEYCODE_3),
            new KeyboardInfo("4",(int)Keys.D4,(int)InputCodes.KEYCODE_4),
            new KeyboardInfo("5",(int)Keys.D5,(int)InputCodes.KEYCODE_5),
            new KeyboardInfo("6",(int)Keys.D6,(int)InputCodes.KEYCODE_6),
            new KeyboardInfo("7",(int)Keys.D7,(int)InputCodes.KEYCODE_7),
            new KeyboardInfo("8",(int)Keys.D8,(int)InputCodes.KEYCODE_8),
            new KeyboardInfo("9",(int)Keys.D9,(int)InputCodes.KEYCODE_9),
            new KeyboardInfo("0 PAD",(int)Keys.NumPad0,(int)InputCodes.KEYCODE_0_PAD),
            new KeyboardInfo("1 PAD",(int)Keys.NumPad1,(int)InputCodes.KEYCODE_1_PAD),
            new KeyboardInfo("2 PAD",(int)Keys.NumPad2,(int)InputCodes.KEYCODE_2_PAD),
            new KeyboardInfo("3 PAD",(int)Keys.NumPad3,(int)InputCodes.KEYCODE_3_PAD),
            new KeyboardInfo("4 PAD",(int)Keys.NumPad4,(int)InputCodes.KEYCODE_4_PAD),
            new KeyboardInfo("5 PAD",(int)Keys.NumPad5,(int)InputCodes.KEYCODE_5_PAD),
            new KeyboardInfo("6 PAD",(int)Keys.NumPad6,(int)InputCodes.KEYCODE_6_PAD),
            new KeyboardInfo("7 PAD",(int)Keys.NumPad7,(int)InputCodes.KEYCODE_7_PAD),
            new KeyboardInfo("8 PAD",(int)Keys.NumPad8,(int)InputCodes.KEYCODE_8_PAD),
            new KeyboardInfo("9 PAD",(int)Keys.NumPad9,(int)InputCodes.KEYCODE_9_PAD),
            new KeyboardInfo("F1",(int)Keys.F1,(int)InputCodes.KEYCODE_F1),
            new KeyboardInfo("F2",(int)Keys.F2,(int)InputCodes.KEYCODE_F2),
            new KeyboardInfo("F3",(int)Keys.F3,(int)InputCodes.KEYCODE_F3),
            new KeyboardInfo("F4",(int)Keys.F4,(int)InputCodes.KEYCODE_F4),
            new KeyboardInfo("F5",(int)Keys.F5,(int)InputCodes.KEYCODE_F5),
            new KeyboardInfo("F6",(int)Keys.F6,(int)InputCodes.KEYCODE_F6),
            new KeyboardInfo("F7",(int)Keys.F7,(int)InputCodes.KEYCODE_F7),
            new KeyboardInfo("F8",(int)Keys.F8,(int)InputCodes.KEYCODE_F8),
            new KeyboardInfo("F9",(int)Keys.F9,(int)InputCodes.KEYCODE_F9),
            new KeyboardInfo("F10",(int)Keys.F10,(int)InputCodes.KEYCODE_F10),
            new KeyboardInfo("F11",(int)Keys.F11,(int)InputCodes.KEYCODE_F11),
            new KeyboardInfo("F12",(int)Keys.F12,(int)InputCodes.KEYCODE_F12),
            new KeyboardInfo("ESC",(int)Keys.Escape,(int)InputCodes.KEYCODE_ESC),
            new KeyboardInfo("~",(int)Keys.OemTilde,(int)InputCodes.KEYCODE_TILDE),
            new KeyboardInfo("-", (int)Keys.OemMinus, (int)InputCodes.KEYCODE_MINUS),
            //new KeyboardInfo("=",(int)Keys.o,(int)InputCodes.KEYCODE_A),
            new KeyboardInfo("BKSPACE",(int)Keys.Back,(int)InputCodes.KEYCODE_BACKSPACE),
            new KeyboardInfo("TAB",(int)Keys.Tab,(int)InputCodes.KEYCODE_TAB),
            new KeyboardInfo("[",(int)Keys.OemOpenBrackets,(int)InputCodes.KEYCODE_OPENBRACE),
            new KeyboardInfo("]",(int)Keys.OemCloseBrackets,(int)InputCodes.KEYCODE_CLOSEBRACE),
            new KeyboardInfo("ENTER",(int)Keys.Enter,(int)InputCodes.KEYCODE_ENTER),
            new KeyboardInfo(";",(int)Keys.OemSemicolon,(int)InputCodes.KEYCODE_COLON),
            new KeyboardInfo(":",(int)Keys.OemQuotes,(int)InputCodes.KEYCODE_QUOTE),
            new KeyboardInfo("\\",(int)Keys.OemBackslash,(int)InputCodes.KEYCODE_BACKSLASH),
            //new KeyboardInfo("<",(int)Keys.OemQuotes,(int)InputCodes.KEYCODE_QUOTE),
            new KeyboardInfo(",",(int)Keys.OemComma,(int)InputCodes.KEYCODE_COMMA),
            new KeyboardInfo(".",(int)Keys.OemPeriod,(int)InputCodes.KEYCODE_STOP),
            new KeyboardInfo("/",(int)Keys.Divide,(int)InputCodes.KEYCODE_SLASH),
            new KeyboardInfo("SPACE",(int)Keys.Space,(int)InputCodes.KEYCODE_SPACE),
            new KeyboardInfo("INS",(int)Keys.Insert,(int)InputCodes.KEYCODE_INSERT),
            new KeyboardInfo("DEL",(int)Keys.Delete,(int)InputCodes.KEYCODE_DEL),
            new KeyboardInfo("HOME",(int)Keys.Home,(int)InputCodes.KEYCODE_HOME),
            new KeyboardInfo("END",(int)Keys.End,(int)InputCodes.KEYCODE_END),
            new KeyboardInfo("PGUP",(int)Keys.PageUp,(int)InputCodes.KEYCODE_PGUP),
            new KeyboardInfo("PGDN",(int)Keys.PageDown,(int)InputCodes.KEYCODE_PGDN),
            new KeyboardInfo("LEFT",(int)Keys.Left,(int)InputCodes.KEYCODE_LEFT),
            new KeyboardInfo("RIGHT",(int)Keys.Right,(int)InputCodes.KEYCODE_RIGHT),
            new KeyboardInfo("UP",(int)Keys.Up,(int)InputCodes.KEYCODE_UP),
            new KeyboardInfo("DOWN",(int)Keys.Down,(int)InputCodes.KEYCODE_DOWN),

            new KeyboardInfo(null, 0, 0)
        };

        static byte[] currkey = new byte[256];

        int osd_wait_keypress()
        {
            throw new Exception();
        }
        public static void osd_led_w(int led, int on)
        {
            //xxxthrow new Exception();
        }
        void osd_customize_inputport_defaults(ipd[] defaults)
        {
            //nont if (use_hotrod)
        }

        void osd_poll_joysticks()
        {
            if (joystick > JOY_TYPE_NONE)
                poll_joystick();
        }
        void poll_joystick()
        {
            //Nothing, we poll joysticks from game updateloop in game1.cs
        }
        bool osd_joystick_needs_calibration()
        {
            return false;
        }
        static int pressed, counter;
        bool osd_is_key_pressed(int keycode)
        {
            if (keycode >= (int)256) return false;

            if (keycode == (int)Keys.Pause)
            {
                int res;

                res = currkey[(int)Keys.Pause] ^ pressed;
                if (res != 0)
                {
                    if (counter > 0)
                    {
                        if (--counter == 0)
                            pressed = currkey[(int)Keys.Pause];
                    }
                    else counter = 10;
                }

                return res != 0;
            }

            return currkey[keycode] != 0;
        }
        static KeyboardInfo[] osd_get_key_list() { return keylist; }
        static uint mouse_b = 0;
        bool osd_is_joy_pressed(int joycode)
        {
            uint joy_num, stick;


            /* special case for mouse buttons */
            //if ((uint)joycode ==  MOUSE_BUTTON(1))return (mouse_b & 1)!=0; 
            //if ((uint)joycode ==   MOUSE_BUTTON(2))return (mouse_b & 2)!=0; 
            //if ((uint)joycode ==  MOUSE_BUTTON(3))return (mouse_b & 4)!=0; 


            joy_num = GET_JOYCODE_JOY((uint)joycode);

            /* do we have as many sticks? */
            if (joy_num == 0 || joy_num > num_joysticks)
                return false;
            joy_num--;

            stick = GET_JOYCODE_STICK((uint)joycode);
            if (stick == 0)
            {
                /* buttons */
                uint button;

                button = GET_JOYCODE_BUTTON((uint)joycode);
                if (button == 0 || button > joy[joy_num].num_buttons)
                    return false;
                button--;

                return joy[joy_num].button[button].Pressed;
            }
            else
            {
                if (stick > joy[joy_num].num_sticks)
                    return false;
                stick--;

                uint axis = GET_JOYCODE_AXIS((uint)joycode);
                uint dir = GET_JOYCODE_DIR((uint)joycode);

                if (axis == 0 || axis > joy[joy_num].stick[stick].num_axis)
                    return false;
                axis--;

                switch (dir)
                {
                    case 1:
                        return joy[joy_num].stick[stick].axis[axis].Pos; break;
                    case 2:
                        return joy[joy_num].stick[stick].axis[axis].Neg; break;
                    default:
                        return false; break;
                }
            }

            return false;
        }
        static JoystickInfo[] joylist = new JoystickInfo[256];
        class GamepadInfo
        {
            public GamePadState gamepadState;
            public int num_sticks = 1;
            public int num_buttons = 10;
            public struct Button
            {
                public string name;
                public bool Pressed;
            }
            public Button[] button = new Button[10];
            public struct Stick
            {
                public int num_axis;
                public string name;
                public struct Axis
                {
                    public string name;
                    public float pos;
                    public bool Pos, Neg;
                }
                public Axis[] axis;
            }
            public Stick[] stick = new Stick[1];
            public void SetState(GamePadState state)
            {
                stick[0].axis[0].pos = state.ThumbSticks.Left.X;
                stick[0].axis[1].pos = state.ThumbSticks.Left.Y;


                stick[0].axis[1].Pos = state.DPad.Up == ButtonState.Pressed;
                stick[0].axis[1].Neg = state.DPad.Down == ButtonState.Pressed;
                stick[0].axis[0].Neg = state.DPad.Right == ButtonState.Pressed;
                stick[0].axis[0].Pos = state.DPad.Left == ButtonState.Pressed;
                stick[0].axis[2].Pos = state.DPad.Up == ButtonState.Pressed && state.DPad.Left == ButtonState.Pressed;
                stick[0].axis[2].Neg = state.DPad.Right == ButtonState.Pressed && state.DPad.Down == ButtonState.Pressed;
                stick[0].axis[3].Pos = state.DPad.Down == ButtonState.Pressed && state.DPad.Left == ButtonState.Pressed;
                stick[0].axis[3].Neg = state.DPad.Right == ButtonState.Pressed && state.DPad.Up == ButtonState.Pressed;


                button[0].Pressed = state.Buttons.A == ButtonState.Pressed;
                button[1].Pressed = state.Buttons.B == ButtonState.Pressed;
                button[2].Pressed = state.Buttons.X == ButtonState.Pressed;
                button[3].Pressed = state.Buttons.Y == ButtonState.Pressed;
                button[4].Pressed = state.Buttons.Back == ButtonState.Pressed;
                button[5].Pressed = state.Buttons.Start == ButtonState.Pressed;
                button[6].Pressed = state.Buttons.LeftShoulder == ButtonState.Pressed;
                button[7].Pressed = state.Buttons.RightShoulder == ButtonState.Pressed;
                button[8].Pressed = state.Buttons.LeftStick == ButtonState.Pressed;
                button[9].Pressed = state.Buttons.RightStick == ButtonState.Pressed;
            }
            public GamepadInfo()
            {
                //for (int i = 0; i < 2; i++)
                //{
                //    stick[i] = new Stick();
                //    stick[i].num_axis = 2;
                //    stick[i].axis = new Stick.Axis[2];
                //    for (int j = 0; j < 1; j++)
                //        stick[i].axis[j] = new Stick.Axis();
                //    stick[i].axis[0].name = "UpDown";
                //    stick[i].axis[1].name = "LeftRight";
                //}
                stick[0] = new Stick();
                stick[0].name = "DPad";
                stick[0].num_axis = 4;
                stick[0].axis = new Stick.Axis[4];
                stick[0].axis[0].name = "Left Right";
                stick[0].axis[1].name = "Up Down";
                stick[0].axis[2].name = "LeftUp RightDown";
                stick[0].axis[3].name = "LeftDown RightUp";
                //stick[2].axis[4].name = "Down";
                //stick[2].axis[5].name = "DownLeft";
                //stick[2].axis[6].name = "Left";
                //stick[2].axis[7].name = "LeftUp";
                for (int i = 0; i < 10; i++) button = new Button[10];
                button[0].name = "A button";
                button[1].name = "Y button";
                button[2].name = "X button";
                button[3].name = "B button";
                button[4].name = "Back button";
                button[5].name = "Start button";
                button[6].name = "ShoulderLeft button";
                button[7].name = "ShoulderRight button";
                button[8].name = "LeftStick button";
                button[9].name = "RightStick button";
                //stick[0].name = "Left analog stick";
                //stick[1].name = "Right analog stick";
            }
        }
        static GamepadInfo[] joy = new GamepadInfo[4];
        static JoystickInfo[] osd_get_joy_list() { return joylist; }
        static byte[,] keyboard_state = new byte[1, 256];
        public void keyPressed(Keys c)
        {
            currkey[(int)c] = 1;
            keyboard_state[0, (int)c] = 1;
        }
        public void keyReleased(Keys c)
        {
            currkey[(int)c] = 0;
            keyboard_state[0, (int)c] = 0;
        }
        void osd_analogjoy_read(int player, ref int analog_x, ref int analog_y)
        {
            analog_x = 0;
            analog_y = 0;
            if (player + 1 > num_joysticks || joystick == JOY_TYPE_NONE)
                return;
            analog_x = (int)(joy[player].stick[0].axis[0].pos * 128);
            analog_y = (int)(joy[player].stick[0].axis[1].pos * 128);
        }
        void osd_trak_read(int player, ref int deltax, ref int deltay)
        {
            return;//xxx
        }
        static string[] joynames = new string[256];

        //static uint MOUSE_BUTTON(int button) { return (uint)JOYCODE(1, 0, button, 1); }
        static uint GET_JOYCODE_JOY(uint code) { return (((code) >> 0) & 0x07); }
        static uint GET_JOYCODE_STICK(uint code) { return (((code) >> 3) & 0x1f); }
        static uint GET_JOYCODE_AXIS(uint code) { return (((code) >> 8) & 0x3f); }
        static uint GET_JOYCODE_BUTTON(uint code) { return GET_JOYCODE_AXIS(code); }
        static uint GET_JOYCODE_DIR(uint code) { return (((code) >> 14) & 0x03); }

        public void UpdateGamepad(GamePadState state, int index)
        {
            if (joy[index] == null) return;
            joy[index].SetState(state);
        }
        void xna_init_input()
        {
            int tot, i, j, k;
            string buf = "";

            for (i = 0; i < 4; i++)
            {
                joy[i] = new GamepadInfo();
            }
            tot = 0;

            /* first of all, map mouse buttons */
            //for (j = 0; j < 3; j++)
            //{
            //    buf = sprintf("MOUSE B%d", j + 1);
            //    joynames[tot] = buf;
            //    joylist[tot] = new JoystickInfo();
            //    joylist[tot].name = joynames[tot];
            //    joylist[tot].code = MOUSE_BUTTON(j + 1);
            //    tot++;
            //}

            for (i = 0; i < num_joysticks; i++)
            {
                for (j = 0; j < joy[i].num_sticks; j++)
                {
                    for (k = 0; k < joy[i].stick[j].num_axis; k++)
                    {
                        joylist[tot] = new JoystickInfo();
                        joynames[tot] = sprintf(buf, "J%d %s %s -", i + 1, joy[i].stick[j].name, joy[i].stick[j].axis[k].name);
                        joylist[tot].name = joynames[tot];
                        joylist[tot].code = (uint)JOYCODE(i + 1, j + 1, k + 1, 1);
                        tot++;

                        joylist[tot] = new JoystickInfo();

                        joynames[tot] = sprintf("J%d %s %s +", i + 1, joy[i].stick[j].name, joy[i].stick[j].axis[k].name);
                        joylist[tot].name = joynames[tot];
                        joylist[tot].code = (uint)JOYCODE(i + 1, j + 1, k + 1, 2);
                        tot++;
                    }
                }
                for (j = 0; j < joy[i].num_buttons; j++)
                {
                    joynames[tot] = sprintf(buf, "J%d %s", i + 1, joy[i].button[j].name);
                    joylist[tot] = new JoystickInfo();
                    joylist[tot].name = joynames[tot];
                    joylist[tot].code = (uint)JOYCODE(i + 1, 0, j + 1, 0);
                    tot++;
                }
            }
            joylist[tot] = new JoystickInfo();
            /* terminate array */
            joylist[tot].name = null;
            joylist[tot].code = 0;
            joylist[tot].standardcode = 0;

            /* fill in equivalences */
            for (i = 0; i < tot; i++)
            {
                joylist[i].standardcode = (uint)InputCodes.CODE_OTHER;

                j = 0;
                while (joyequiv[j][0] != 0)
                {
                    if (joyequiv[j][0] == joylist[i].code)
                    {
                        joylist[i].standardcode = (uint)joyequiv[j][1];
                        break;
                    }
                    j++;
                }
            }
        }
    }
}
