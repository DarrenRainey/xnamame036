using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        const int DT_COLOR_WHITE = 0, DT_COLOR_YELLOW = 1, DT_COLOR_RED = 2;
        const int SEL_BITS = 12;
        const int SEL_MASK = ((1 << SEL_BITS) - 1);
        static GfxElement builduifont()
        {

            byte[] fontdata6x8 =
	{
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x7c,0x80,0x98,0x90,0x80,0xbc,0x80,0x7c,0xf8,0x04,0x64,0x44,0x04,0xf4,0x04,0xf8,
		0x7c,0x80,0x98,0x88,0x80,0xbc,0x80,0x7c,0xf8,0x04,0x64,0x24,0x04,0xf4,0x04,0xf8,
		0x7c,0x80,0x88,0x98,0x80,0xbc,0x80,0x7c,0xf8,0x04,0x24,0x64,0x04,0xf4,0x04,0xf8,
		0x7c,0x80,0x90,0x98,0x80,0xbc,0x80,0x7c,0xf8,0x04,0x44,0x64,0x04,0xf4,0x04,0xf8,
		0x30,0x48,0x84,0xb4,0xb4,0x84,0x48,0x30,0x30,0x48,0x84,0x84,0x84,0x84,0x48,0x30,
		0x00,0xfc,0x84,0x8c,0xd4,0xa4,0xfc,0x00,0x00,0xfc,0x84,0x84,0x84,0x84,0xfc,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x30,0x68,0x78,0x78,0x30,0x00,0x00,
		0x80,0xc0,0xe0,0xf0,0xe0,0xc0,0x80,0x00,0x04,0x0c,0x1c,0x3c,0x1c,0x0c,0x04,0x00,
		0x20,0x70,0xf8,0x20,0x20,0xf8,0x70,0x20,0x48,0x48,0x48,0x48,0x48,0x00,0x48,0x00,
		0x00,0x00,0x30,0x68,0x78,0x30,0x00,0x00,0x00,0x30,0x68,0x78,0x78,0x30,0x00,0x00,
		0x70,0xd8,0xe8,0xe8,0xf8,0xf8,0x70,0x00,0x1c,0x7c,0x74,0x44,0x44,0x4c,0xcc,0xc0,
		0x20,0x70,0xf8,0x70,0x70,0x70,0x70,0x00,0x70,0x70,0x70,0x70,0xf8,0x70,0x20,0x00,
		0x00,0x10,0xf8,0xfc,0xf8,0x10,0x00,0x00,0x00,0x20,0x7c,0xfc,0x7c,0x20,0x00,0x00,
		0xb0,0x54,0xb8,0xb8,0x54,0xb0,0x00,0x00,0x00,0x28,0x6c,0xfc,0x6c,0x28,0x00,0x00,
		0x00,0x30,0x30,0x78,0x78,0xfc,0x00,0x00,0xfc,0x78,0x78,0x30,0x30,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x20,0x20,0x20,0x20,0x20,0x00,0x20,0x00,
		0x50,0x50,0x50,0x00,0x00,0x00,0x00,0x00,0x00,0x50,0xf8,0x50,0xf8,0x50,0x00,0x00,
		0x20,0x70,0xc0,0x70,0x18,0xf0,0x20,0x00,0x40,0xa4,0x48,0x10,0x20,0x48,0x94,0x08,
		0x60,0x90,0xa0,0x40,0xa8,0x90,0x68,0x00,0x10,0x20,0x40,0x00,0x00,0x00,0x00,0x00,
		0x20,0x40,0x40,0x40,0x40,0x40,0x20,0x00,0x10,0x08,0x08,0x08,0x08,0x08,0x10,0x00,
		0x20,0xa8,0x70,0xf8,0x70,0xa8,0x20,0x00,0x00,0x20,0x20,0xf8,0x20,0x20,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x30,0x30,0x60,0x00,0x00,0x00,0xf8,0x00,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x30,0x30,0x00,0x00,0x08,0x10,0x20,0x40,0x80,0x00,0x00,
		0x70,0x88,0x88,0x88,0x88,0x88,0x70,0x00,0x10,0x30,0x10,0x10,0x10,0x10,0x10,0x00,
		0x70,0x88,0x08,0x10,0x20,0x40,0xf8,0x00,0x70,0x88,0x08,0x30,0x08,0x88,0x70,0x00,
		0x10,0x30,0x50,0x90,0xf8,0x10,0x10,0x00,0xf8,0x80,0xf0,0x08,0x08,0x88,0x70,0x00,
		0x70,0x80,0xf0,0x88,0x88,0x88,0x70,0x00,0xf8,0x08,0x08,0x10,0x20,0x20,0x20,0x00,
		0x70,0x88,0x88,0x70,0x88,0x88,0x70,0x00,0x70,0x88,0x88,0x88,0x78,0x08,0x70,0x00,
		0x00,0x00,0x30,0x30,0x00,0x30,0x30,0x00,0x00,0x00,0x30,0x30,0x00,0x30,0x30,0x60,
		0x10,0x20,0x40,0x80,0x40,0x20,0x10,0x00,0x00,0x00,0xf8,0x00,0xf8,0x00,0x00,0x00,
		0x40,0x20,0x10,0x08,0x10,0x20,0x40,0x00,0x70,0x88,0x08,0x10,0x20,0x00,0x20,0x00,
		0x30,0x48,0x94,0xa4,0xa4,0x94,0x48,0x30,0x70,0x88,0x88,0xf8,0x88,0x88,0x88,0x00,
		0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0,0x00,0x70,0x88,0x80,0x80,0x80,0x88,0x70,0x00,
		0xf0,0x88,0x88,0x88,0x88,0x88,0xf0,0x00,0xf8,0x80,0x80,0xf0,0x80,0x80,0xf8,0x00,
		0xf8,0x80,0x80,0xf0,0x80,0x80,0x80,0x00,0x70,0x88,0x80,0x98,0x88,0x88,0x70,0x00,
		0x88,0x88,0x88,0xf8,0x88,0x88,0x88,0x00,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x00,
		0x08,0x08,0x08,0x08,0x88,0x88,0x70,0x00,0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88,0x00,
		0x80,0x80,0x80,0x80,0x80,0x80,0xf8,0x00,0x88,0xd8,0xa8,0x88,0x88,0x88,0x88,0x00,
		0x88,0xc8,0xa8,0x98,0x88,0x88,0x88,0x00,0x70,0x88,0x88,0x88,0x88,0x88,0x70,0x00,
		0xf0,0x88,0x88,0xf0,0x80,0x80,0x80,0x00,0x70,0x88,0x88,0x88,0x88,0x88,0x70,0x08,
		0xf0,0x88,0x88,0xf0,0x88,0x88,0x88,0x00,0x70,0x88,0x80,0x70,0x08,0x88,0x70,0x00,
		0xf8,0x20,0x20,0x20,0x20,0x20,0x20,0x00,0x88,0x88,0x88,0x88,0x88,0x88,0x70,0x00,
		0x88,0x88,0x88,0x88,0x88,0x50,0x20,0x00,0x88,0x88,0x88,0x88,0xa8,0xd8,0x88,0x00,
		0x88,0x50,0x20,0x20,0x20,0x50,0x88,0x00,0x88,0x88,0x88,0x50,0x20,0x20,0x20,0x00,
		0xf8,0x08,0x10,0x20,0x40,0x80,0xf8,0x00,0x30,0x20,0x20,0x20,0x20,0x20,0x30,0x00,
		0x40,0x40,0x20,0x20,0x10,0x10,0x08,0x08,0x30,0x10,0x10,0x10,0x10,0x10,0x30,0x00,
		0x20,0x50,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xfc,
		0x40,0x20,0x10,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x70,0x08,0x78,0x88,0x78,0x00,
		0x80,0x80,0xf0,0x88,0x88,0x88,0xf0,0x00,0x00,0x00,0x70,0x88,0x80,0x80,0x78,0x00,
		0x08,0x08,0x78,0x88,0x88,0x88,0x78,0x00,0x00,0x00,0x70,0x88,0xf8,0x80,0x78,0x00,
		0x18,0x20,0x70,0x20,0x20,0x20,0x20,0x00,0x00,0x00,0x78,0x88,0x88,0x78,0x08,0x70,
		0x80,0x80,0xf0,0x88,0x88,0x88,0x88,0x00,0x20,0x00,0x20,0x20,0x20,0x20,0x20,0x00,
		0x20,0x00,0x20,0x20,0x20,0x20,0x20,0xc0,0x80,0x80,0x90,0xa0,0xe0,0x90,0x88,0x00,
		0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x00,0x00,0x00,0xf0,0xa8,0xa8,0xa8,0xa8,0x00,
		0x00,0x00,0xb0,0xc8,0x88,0x88,0x88,0x00,0x00,0x00,0x70,0x88,0x88,0x88,0x70,0x00,
		0x00,0x00,0xf0,0x88,0x88,0xf0,0x80,0x80,0x00,0x00,0x78,0x88,0x88,0x78,0x08,0x08,
		0x00,0x00,0xb0,0xc8,0x80,0x80,0x80,0x00,0x00,0x00,0x78,0x80,0x70,0x08,0xf0,0x00,
		0x20,0x20,0x70,0x20,0x20,0x20,0x18,0x00,0x00,0x00,0x88,0x88,0x88,0x98,0x68,0x00,
		0x00,0x00,0x88,0x88,0x88,0x50,0x20,0x00,0x00,0x00,0xa8,0xa8,0xa8,0xa8,0x50,0x00,
		0x00,0x00,0x88,0x50,0x20,0x50,0x88,0x00,0x00,0x00,0x88,0x88,0x88,0x78,0x08,0x70,
		0x00,0x00,0xf8,0x10,0x20,0x40,0xf8,0x00,0x08,0x10,0x10,0x20,0x10,0x10,0x08,0x00,
		0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x40,0x20,0x20,0x10,0x20,0x20,0x40,0x00,
		0x00,0x68,0xb0,0x00,0x00,0x00,0x00,0x00,0x20,0x50,0x20,0x50,0xa8,0x50,0x00,0x00,
	};
#if false	   
             /* HJB 990215 unused!? */
	static unsigned char fontdata8x8[] =
	{
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
		0x3C,0x42,0x99,0xBD,0xBD,0x99,0x42,0x3C,0x3C,0x42,0x81,0x81,0x81,0x81,0x42,0x3C,
		0xFE,0x82,0x8A,0xD2,0xA2,0x82,0xFE,0x00,0xFE,0x82,0x82,0x82,0x82,0x82,0xFE,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x38,0x64,0x74,0x7C,0x38,0x00,0x00,
		0x80,0xC0,0xF0,0xFC,0xF0,0xC0,0x80,0x00,0x01,0x03,0x0F,0x3F,0x0F,0x03,0x01,0x00,
		0x18,0x3C,0x7E,0x18,0x7E,0x3C,0x18,0x00,0xEE,0xEE,0xEE,0xCC,0x00,0xCC,0xCC,0x00,
		0x00,0x00,0x30,0x68,0x78,0x30,0x00,0x00,0x00,0x38,0x64,0x74,0x7C,0x38,0x00,0x00,
		0x3C,0x66,0x7A,0x7A,0x7E,0x7E,0x3C,0x00,0x0E,0x3E,0x3A,0x22,0x26,0x6E,0xE4,0x40,
		0x18,0x3C,0x7E,0x3C,0x3C,0x3C,0x3C,0x00,0x3C,0x3C,0x3C,0x3C,0x7E,0x3C,0x18,0x00,
		0x08,0x7C,0x7E,0x7E,0x7C,0x08,0x00,0x00,0x10,0x3E,0x7E,0x7E,0x3E,0x10,0x00,0x00,
		0x58,0x2A,0xDC,0xC8,0xDC,0x2A,0x58,0x00,0x24,0x66,0xFF,0xFF,0x66,0x24,0x00,0x00,
		0x00,0x10,0x10,0x38,0x38,0x7C,0xFE,0x00,0xFE,0x7C,0x38,0x38,0x10,0x10,0x00,0x00,
		0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x1C,0x1C,0x1C,0x18,0x00,0x18,0x18,0x00,
		0x6C,0x6C,0x24,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x7C,0x28,0x7C,0x28,0x00,0x00,
		0x10,0x38,0x60,0x38,0x0C,0x78,0x10,0x00,0x40,0xA4,0x48,0x10,0x24,0x4A,0x04,0x00,
		0x18,0x34,0x18,0x3A,0x6C,0x66,0x3A,0x00,0x18,0x18,0x20,0x00,0x00,0x00,0x00,0x00,
		0x30,0x60,0x60,0x60,0x60,0x60,0x30,0x00,0x0C,0x06,0x06,0x06,0x06,0x06,0x0C,0x00,
		0x10,0x54,0x38,0x7C,0x38,0x54,0x10,0x00,0x00,0x18,0x18,0x7E,0x18,0x18,0x00,0x00,
		0x00,0x00,0x00,0x00,0x18,0x18,0x30,0x00,0x00,0x00,0x00,0x00,0x3E,0x00,0x00,0x00,
		0x00,0x00,0x00,0x00,0x18,0x18,0x00,0x00,0x00,0x04,0x08,0x10,0x20,0x40,0x00,0x00,
		0x38,0x4C,0xC6,0xC6,0xC6,0x64,0x38,0x00,0x18,0x38,0x18,0x18,0x18,0x18,0x7E,0x00,
		0x7C,0xC6,0x0E,0x3C,0x78,0xE0,0xFE,0x00,0x7E,0x0C,0x18,0x3C,0x06,0xC6,0x7C,0x00,
		0x1C,0x3C,0x6C,0xCC,0xFE,0x0C,0x0C,0x00,0xFC,0xC0,0xFC,0x06,0x06,0xC6,0x7C,0x00,
		0x3C,0x60,0xC0,0xFC,0xC6,0xC6,0x7C,0x00,0xFE,0xC6,0x0C,0x18,0x30,0x30,0x30,0x00,
		0x78,0xC4,0xE4,0x78,0x86,0x86,0x7C,0x00,0x7C,0xC6,0xC6,0x7E,0x06,0x0C,0x78,0x00,
		0x00,0x00,0x18,0x00,0x00,0x18,0x00,0x00,0x00,0x00,0x18,0x00,0x00,0x18,0x18,0x30,
		0x1C,0x38,0x70,0xE0,0x70,0x38,0x1C,0x00,0x00,0x7C,0x00,0x00,0x7C,0x00,0x00,0x00,
		0x70,0x38,0x1C,0x0E,0x1C,0x38,0x70,0x00,0x7C,0xC6,0xC6,0x1C,0x18,0x00,0x18,0x00,
		0x3C,0x42,0x99,0xA1,0xA5,0x99,0x42,0x3C,0x38,0x6C,0xC6,0xC6,0xFE,0xC6,0xC6,0x00,
		0xFC,0xC6,0xC6,0xFC,0xC6,0xC6,0xFC,0x00,0x3C,0x66,0xC0,0xC0,0xC0,0x66,0x3C,0x00,
		0xF8,0xCC,0xC6,0xC6,0xC6,0xCC,0xF8,0x00,0xFE,0xC0,0xC0,0xFC,0xC0,0xC0,0xFE,0x00,
		0xFE,0xC0,0xC0,0xFC,0xC0,0xC0,0xC0,0x00,0x3E,0x60,0xC0,0xCE,0xC6,0x66,0x3E,0x00,
		0xC6,0xC6,0xC6,0xFE,0xC6,0xC6,0xC6,0x00,0x7E,0x18,0x18,0x18,0x18,0x18,0x7E,0x00,
		0x06,0x06,0x06,0x06,0xC6,0xC6,0x7C,0x00,0xC6,0xCC,0xD8,0xF0,0xF8,0xDC,0xCE,0x00,
		0x60,0x60,0x60,0x60,0x60,0x60,0x7E,0x00,0xC6,0xEE,0xFE,0xFE,0xD6,0xC6,0xC6,0x00,
		0xC6,0xE6,0xF6,0xFE,0xDE,0xCE,0xC6,0x00,0x7C,0xC6,0xC6,0xC6,0xC6,0xC6,0x7C,0x00,
		0xFC,0xC6,0xC6,0xC6,0xFC,0xC0,0xC0,0x00,0x7C,0xC6,0xC6,0xC6,0xDE,0xCC,0x7A,0x00,
		0xFC,0xC6,0xC6,0xCE,0xF8,0xDC,0xCE,0x00,0x78,0xCC,0xC0,0x7C,0x06,0xC6,0x7C,0x00,
		0x7E,0x18,0x18,0x18,0x18,0x18,0x18,0x00,0xC6,0xC6,0xC6,0xC6,0xC6,0xC6,0x7C,0x00,
		0xC6,0xC6,0xC6,0xEE,0x7C,0x38,0x10,0x00,0xC6,0xC6,0xD6,0xFE,0xFE,0xEE,0xC6,0x00,
		0xC6,0xEE,0x3C,0x38,0x7C,0xEE,0xC6,0x00,0x66,0x66,0x66,0x3C,0x18,0x18,0x18,0x00,
		0xFE,0x0E,0x1C,0x38,0x70,0xE0,0xFE,0x00,0x3C,0x30,0x30,0x30,0x30,0x30,0x3C,0x00,
		0x60,0x60,0x30,0x18,0x0C,0x06,0x06,0x00,0x3C,0x0C,0x0C,0x0C,0x0C,0x0C,0x3C,0x00,
		0x18,0x3C,0x66,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,
		0x30,0x30,0x18,0x00,0x00,0x00,0x00,0x00,0x00,0x3C,0x06,0x3E,0x66,0x66,0x3C,0x00,
		0x60,0x7C,0x66,0x66,0x66,0x66,0x7C,0x00,0x00,0x3C,0x66,0x60,0x60,0x66,0x3C,0x00,
		0x06,0x3E,0x66,0x66,0x66,0x66,0x3E,0x00,0x00,0x3C,0x66,0x66,0x7E,0x60,0x3C,0x00,
		0x1C,0x30,0x78,0x30,0x30,0x30,0x30,0x00,0x00,0x3E,0x66,0x66,0x66,0x3E,0x06,0x3C,
		0x60,0x7C,0x76,0x66,0x66,0x66,0x66,0x00,0x18,0x00,0x38,0x18,0x18,0x18,0x18,0x00,
		0x0C,0x00,0x1C,0x0C,0x0C,0x0C,0x0C,0x38,0x60,0x60,0x66,0x6C,0x78,0x6C,0x66,0x00,
		0x38,0x18,0x18,0x18,0x18,0x18,0x18,0x00,0x00,0xEC,0xFE,0xFE,0xFE,0xD6,0xC6,0x00,
		0x00,0x7C,0x76,0x66,0x66,0x66,0x66,0x00,0x00,0x3C,0x66,0x66,0x66,0x66,0x3C,0x00,
		0x00,0x7C,0x66,0x66,0x66,0x7C,0x60,0x60,0x00,0x3E,0x66,0x66,0x66,0x3E,0x06,0x06,
		0x00,0x7E,0x70,0x60,0x60,0x60,0x60,0x00,0x00,0x3C,0x60,0x3C,0x06,0x66,0x3C,0x00,
		0x30,0x78,0x30,0x30,0x30,0x30,0x1C,0x00,0x00,0x66,0x66,0x66,0x66,0x6E,0x3E,0x00,
		0x00,0x66,0x66,0x66,0x66,0x3C,0x18,0x00,0x00,0xC6,0xD6,0xFE,0xFE,0x7C,0x6C,0x00,
		0x00,0x66,0x3C,0x18,0x3C,0x66,0x66,0x00,0x00,0x66,0x66,0x66,0x66,0x3E,0x06,0x3C,
		0x00,0x7E,0x0C,0x18,0x30,0x60,0x7E,0x00,0x0E,0x18,0x0C,0x38,0x0C,0x18,0x0E,0x00,
		0x18,0x18,0x18,0x00,0x18,0x18,0x18,0x00,0x70,0x18,0x30,0x1C,0x30,0x18,0x70,0x00,
		0x00,0x00,0x76,0xDC,0x00,0x00,0x00,0x00,0x10,0x28,0x10,0x54,0xAA,0x44,0x00,0x00,
	};
#endif
            GfxLayout fontlayout6x8 = new GfxLayout(

               6, 8,	/* 6*8 characters */
               128,	/* 128 characters */
               1,	/* 1 bit per pixel */
               new uint[] { 0 },
               new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, /* straightforward layout */
               new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
               8 * 8 /* every char takes 8 consecutive bytes */
           );
            GfxLayout fontlayout12x8 = new GfxLayout(
               12, 8,	/* 12*8 characters */
               128,	/* 128 characters */
               1,	/* 1 bit per pixel */
               new uint[] { 0 },
               new uint[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 }, /* straightforward layout */
               new uint[] { 0 * 8, 1 * 8, 2 * 8, 3 * 8, 4 * 8, 5 * 8, 6 * 8, 7 * 8 },
               8 * 8 /* every char takes 8 consecutive bytes */
           );
            GfxLayout fontlayout12x16 = new GfxLayout(
               12, 16,	/* 6*8 characters */
               128,	/* 128 characters */
               1,	/* 1 bit per pixel */
               new uint[] { 0 },
               new uint[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 }, /* straightforward layout */
               new uint[] { 0 * 8, 0 * 8, 1 * 8, 1 * 8, 2 * 8, 2 * 8, 3 * 8, 3 * 8, 4 * 8, 4 * 8, 5 * 8, 5 * 8, 6 * 8, 6 * 8, 7 * 8, 7 * 8 },
               8 * 8 /* every char takes 8 consecutive bytes */
           );
#if false	
             /* HJB 990215 unused!? */
	static struct GfxLayout fontlayout8x8 =
	{
		8,8,	/* 8*8 characters */
		128,	/* 128 characters */
		1,	/* 1 bit per pixel */
		{ 0 },
		{ 0, 1, 2, 3, 4, 5, 6, 7 }, /* straightforward layout */
		{ 0*8, 1*8, 2*8, 3*8, 4*8, 5*8, 6*8, 7*8 },
		8*8 /* every char takes 8 consecutive bytes */
	};
#endif
            GfxElement font;
            byte[] colortable = new byte[2 * 2 * sizeof(ushort)];	/* ASG 980209 */
            int trueorientation;


            /* hack: force the display into standard orientation to avoid */
            /* creating a rotated font */
            trueorientation = Machine.orientation;
            Machine.orientation = Machine.ui_orientation;

            if ((Machine.drv.video_attributes & VIDEO_PIXEL_ASPECT_RATIO_MASK)
                    == VIDEO_PIXEL_ASPECT_RATIO_1_2)
            {
                font = decodegfx(new _BytePtr(fontdata6x8), fontlayout12x8);
                Machine.uifontwidth = 12;
                Machine.uifontheight = 8;
            }
            else if (Machine.uiwidth >= 420 && Machine.uiheight >= 420)
            {
                font = decodegfx(new _BytePtr(fontdata6x8), fontlayout12x16);
                Machine.uifontwidth = 12;
                Machine.uifontheight = 16;
            }
            else
            {
                font = decodegfx(new _BytePtr(fontdata6x8), fontlayout6x8);
                Machine.uifontwidth = 6;
                Machine.uifontheight = 8;
            }

            if (font != null)
            {
                /* colortable will be set at run time */
                //memset(colortable,0,sizeof(colortable));
                font.colortable = new _ShortPtr(colortable);
                font.total_colors = 2;
            }

            Machine.orientation = trueorientation;

            return font;
        }
        static void set_ui_visarea(int xmin, int ymin, int xmax, int ymax)
        {
            int temp, w, h;

            /* special case for vectors */
            if (Machine.drv.video_attributes == VIDEO_TYPE_VECTOR)
            {
                if ((Machine.ui_orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    temp = xmin; xmin = ymin; ymin = temp;
                    temp = xmax; xmax = ymax; ymax = temp;
                }
            }
            else
            {
                if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    w = Machine.drv.screen_height;
                    h = Machine.drv.screen_width;
                }
                else
                {
                    w = Machine.drv.screen_width;
                    h = Machine.drv.screen_height;
                }

                if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                {
                    temp = w - xmin - 1;
                    xmin = w - xmax - 1;
                    xmax = temp;
                }

                if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                {
                    temp = h - ymin - 1;
                    ymin = h - ymax - 1;
                    ymax = temp;
                }

                if ((Machine.ui_orientation & ORIENTATION_SWAP_XY) != 0)
                {
                    temp = xmin; xmin = ymin; ymin = temp;
                    temp = xmax; xmax = ymax; ymax = temp;
                }

            }
            Machine.uiwidth = xmax - xmin + 1;
            Machine.uiheight = ymax - ymin + 1;
            Machine.uixmin = xmin;
            Machine.uiymin = ymin;
        }
        static string messagetext;
        static int messagecounter;
        static int setup_selected, osd_selected, jukebox_selected, single_step;
        public static void usrintf_showmessage(string text, params object[] args)
        {
            messagetext = sprintf(text, args);
            messagecounter = (int)(2 * Machine.drv.frames_per_second);
        }

        int showgamewarnings()
        {
            string buf = "";

            if ((Machine.gamedrv.flags &
                    (GAME_NOT_WORKING | GAME_WRONG_COLORS | GAME_IMPERFECT_COLORS |
                      GAME_NO_SOUND | GAME_IMPERFECT_SOUND | GAME_NO_COCKTAIL)) != 0)
            {
                int done;

                buf = "There are known problems with this game:\n\n";

                if ((Machine.gamedrv.flags & GAME_IMPERFECT_COLORS) != 0)
                {
                    buf += "The colors aren't 100% accurate.\n";
                }

                if ((Machine.gamedrv.flags & GAME_WRONG_COLORS) != 0)
                {
                    buf += "The colors are completely wrong.\n";
                }

                if ((Machine.gamedrv.flags & GAME_IMPERFECT_SOUND) != 0)
                {
                    buf += "The sound emulation isn't 100% accurate.\n";
                }

                if ((Machine.gamedrv.flags & GAME_NO_SOUND) != 0)
                {
                    buf += "The game lacks sound.\n";
                }

                if ((Machine.gamedrv.flags & GAME_NO_COCKTAIL) != 0)
                {
                    buf += "Screen flipping in cocktail mode is not supported.\n";
                }

                if ((Machine.gamedrv.flags & GAME_NOT_WORKING) != 0)
                {
                    GameDriver maindrv;
                    int foundworking;

                    buf += "THIS SYSTEM DOESN'T WORK PROPERLY";

                    if (Machine.gamedrv.clone_of != null && (Machine.gamedrv.clone_of.flags & NOT_A_DRIVER) == 0)
                        maindrv = Machine.gamedrv.clone_of;
                    else maindrv = Machine.gamedrv;

                    foundworking = 0;
                    int i = 0;
                    throw new Exception();
                    //while (drivers[i])
                    //{
                    //    if (drivers[i] == maindrv || drivers[i].clone_of == maindrv)
                    //    {
                    //        if ((drivers[i].flags & GAME_NOT_WORKING) == 0)
                    //        {
                    //            if (foundworking == 0)
                    //                strcat(buf,"\n\nThere are working clones of this game. They are:\n\n");
                    //            foundworking = 1;

                    //            sprintf(&buf[strlen(buf)],"%s\n",drivers[i].name);
                    //        }
                    //    }
                    //    i++;
                    //}
                }

                buf += "\n\nType OK to continue";

                ui_displaymessagewindow(buf);

                done = 0;
                do
                {
                    osd_update_video_and_audio();
                    osd_poll_joysticks();
                    if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                        return 1;
                    if (code_pressed_memory((uint)InputCodes.KEYCODE_O) ||
                            input_ui_pressed((int)ports.inptports.IPT_UI_LEFT))
                        done = 1;
                    if (done == 1 && (code_pressed_memory((uint)InputCodes.KEYCODE_K) ||
                            input_ui_pressed((int)ports.inptports.IPT_UI_RIGHT)))
                        done = 2;
                } while (done < 2);
            }


            osd_clearbitmap(Machine.scrbitmap);

            /* clear the input memory */
            while (code_read_async() != (uint)InputCodes.CODE_NONE) ;

            while (displaygameinfo(0) == 1)
            {
                osd_update_video_and_audio();
                osd_poll_joysticks();
            }

            osd_clearbitmap(Machine.scrbitmap);
            /* make sure that the screen is really cleared, in case autoframeskip kicked in */
            osd_update_video_and_audio();
            osd_update_video_and_audio();
            osd_update_video_and_audio();
            osd_update_video_and_audio();

            return 0;
        }
        int displaygameinfo(int selected)
        {
            int i;
            string buf = "";
            int sel;


            sel = selected - 1;


            buf = sprintf("%s\n%s %s\n\nCPU:\n", Machine.gamedrv.description, Machine.gamedrv.year, Machine.gamedrv.manufacturer);
            i = 0;
            while (i < MAX_CPU && i < Machine.drv.cpu.Count && Machine.drv.cpu[i].cpu_type != 0)
            {
                if (Machine.drv.cpu[i].cpu_clock >= 1000000)
                    buf += sprintf("%s %d.%06d MHz",
                            cputype_name(Machine.drv.cpu[i].cpu_type),
                            Machine.drv.cpu[i].cpu_clock / 1000000,
                            Machine.drv.cpu[i].cpu_clock % 1000000);
                else
                    buf += sprintf("%s %d.%03d kHz",
                            cputype_name(Machine.drv.cpu[i].cpu_type),
                            Machine.drv.cpu[i].cpu_clock / 1000,
                            Machine.drv.cpu[i].cpu_clock % 1000);

                if ((Machine.drv.cpu[i].cpu_type & CPU_AUDIO_CPU) != 0)
                    buf += " (sound)";

                buf += "\n";

                i++;
            }

            buf += "\nSound";
            if ((Machine.drv.sound_attributes & SOUND_SUPPORTS_STEREO) != 0)
                buf += " (stereo)";
            buf += ":\n";

            i = 0;
            while (i < MAX_SOUND && i < Machine.drv.sound.Count && Machine.drv.sound[i].sound_type != 0)
            {
                //if (sound_num(&Machine.drv.sound[i]))
                buf += sprintf("%dx", sound_num(Machine.drv.sound[i]));

                buf += sprintf("%s", sound_name(Machine.drv.sound[i]));

                //if (sound_clock(&Machine.drv.sound[i]))
                {
                    if (sound_clock(Machine.drv.sound[i]) >= 1000000)
                        buf += sprintf(" %d.%06d MHz",
                                sound_clock(Machine.drv.sound[i]) / 1000000,
                                sound_clock(Machine.drv.sound[i]) % 1000000);
                    else
                        buf += sprintf(" %d.%03d kHz",
                                sound_clock(Machine.drv.sound[i]) / 1000,
                                sound_clock(Machine.drv.sound[i]) % 1000);
                }

                buf += "\n";

                i++;
            }

            if ((Machine.drv.video_attributes & VIDEO_TYPE_VECTOR) != 0)
                buf += sprintf("\nVector Game\n");
            else
            {
                int pixelx, pixely, tmax, tmin, rem;

                pixelx = 4 * (Machine.drv.visible_area.max_y - Machine.drv.visible_area.min_y + 1);
                pixely = 3 * (Machine.drv.visible_area.max_x - Machine.drv.visible_area.min_x + 1);

                /* calculate MCD */
                if (pixelx >= pixely)
                {
                    tmax = pixelx;
                    tmin = pixely;
                }
                else
                {
                    tmax = pixely;
                    tmin = pixelx;
                }
                while ((rem = tmax % tmin) != 0)
                {
                    tmax = tmin;
                    tmin = rem;
                }
                /* tmin is now the MCD */

                pixelx /= tmin;
                pixely /= tmin;

                buf += sprintf("\nScreen resolution:\n");
                buf += sprintf("%d x %d (%s) %f Hz\n",
                        Machine.drv.visible_area.max_x - Machine.drv.visible_area.min_x + 1,
                        Machine.drv.visible_area.max_y - Machine.drv.visible_area.min_y + 1,
                        (Machine.gamedrv.flags & ORIENTATION_SWAP_XY) != 0 ? "V" : "H",
                        Machine.drv.frames_per_second);

            }


            if (sel == -1)
            {
                /* startup info, print MAME version and ask for any key */

                buf += "\n\tMAME ";    /* \t means that the line will be centered */

                buf += build_version;
                buf += "\n\tPress any key";
                ui_drawbox(0, 0, Machine.uiwidth, Machine.uiheight);
                ui_displaymessagewindow(buf);

                sel = 0;
                if (code_read_async() != (uint)InputCodes.CODE_NONE)
                    sel = -1;
            }
            else
            {
                /* menu system, use the normal menu keys */
                buf += "\n\t\x1a Return to Main Menu \x1b";

                ui_displaymessagewindow(buf);

                if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                    sel = -2;
            }

            if (sel == -1 || sel == -2)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;
            }

            return sel + 1;
        }

        struct DisplayText
        {
            public string text;
            public int color, x, y;
        }
        void ui_displaymessagewindow(string text)
        {
            DisplayText[] dt = new DisplayText[256];
            int curr_dt;
            int c, c2;
            char[] textcopy = new char[2048];
            int i, len, maxlen, lines;
            //char textcopy[2048];
            int leftoffs, topoffs;
            int maxcols, maxrows;

            maxcols = (Machine.uiwidth / Machine.uifontwidth) - 1;
            maxrows = (2 * Machine.uiheight - Machine.uifontheight) / (3 * Machine.uifontheight);

            /* copy text, calculate max len, count lines, wrap long lines and crop height to fit */
            maxlen = 0;
            lines = 0;
            c = 0;//(char *)text;
            c2 = 0;//textcopy;
            while (c < text.Length && text[c] != '\0')
            {
                len = 0;
                while (c < text.Length && text[c] != '\0' && text[c] != '\n')
                {
                    textcopy[c2++] = text[c++];
                    len++;
                    if (len == maxcols && text[c] != '\n')
                    {
                        /* attempt word wrap */
                        int csave = c, c2save = c2;
                        int lensave = len;

                        /* back up to last space or beginning of line */
                        while (text[c] != ' ' && text[c] != '\n' && c > 0)
                        {
                            --c; --c2; --len;
                        }
                        /* if no space was found, hard wrap instead */
                        if (text[c] != ' ')
                        {
                            c = csave; c2 = c2save; len = lensave;
                        }
                        else
                            c++;

                        textcopy[c2++] = '\n'; /* insert wrap */
                        break;
                    }
                }

                if (c < text.Length && text[c] == '\n')
                    textcopy[c2++] = text[c++];

                if (len > maxlen) maxlen = len;

                lines++;
                if (lines == maxrows)
                    break;
            }
            textcopy[c2] = '\0';

            maxlen += 1;

            leftoffs = (Machine.uiwidth - Machine.uifontwidth * maxlen) / 2;
            if (leftoffs < 0) leftoffs = 0;
            topoffs = (Machine.uiheight - (3 * lines + 1) * Machine.uifontheight / 2) / 2;

            /* black background */
            ui_drawbox(leftoffs, topoffs, maxlen * Machine.uifontwidth, (3 * lines + 1) * Machine.uifontheight / 2);

            curr_dt = 0;
            c = 0;// textcopy;
            i = 0;
            while (c < textcopy.Length && textcopy[c] != '\0')
            {
                c2 = c;
                while (c < textcopy.Length && textcopy[c] != '\0' && textcopy[c] != '\n')
                    c++;

                if (textcopy[c] == '\n')
                {
                    textcopy[c] = '\0';
                    c++;
                }

                if (textcopy[c2] == '\t')    /* center text */
                {
                    c2++;
                    dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * (c - c2)) / 2;
                }
                else
                    dt[curr_dt].x = leftoffs + Machine.uifontwidth / 2;

                dt[curr_dt].text = new string(textcopy).Substring(c2);
                dt[curr_dt].color = DT_COLOR_WHITE;
                dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                curr_dt++;

                i++;
            }

            dt[curr_dt].text = null;	/* terminate array */

            displaytext(dt, false, false);
        }
        void displaytext(DisplayText[] dta, bool erase, bool update_screen)
        {
            int trueorientation;


            if (erase)
                osd_clearbitmap(Machine.scrbitmap);


            /* hack: force the display into standard orientation to avoid */
            /* rotating the user interface */
            trueorientation = Machine.orientation;
            Machine.orientation = Machine.ui_orientation;

            osd_mark_dirty(0, 0, Machine.uiwidth - 1, Machine.uiheight - 1, 1);	/* ASG 971011 */
            foreach (DisplayText dt in dta)
            {
                if (dt.text != null)
                {
                    int x, y;
                    int c;


                    x = dt.x;
                    y = dt.y;
                    c = 0;//dt.text;

                    while (c < dt.text.Length && dt.text[c] != '\0')
                    {
                        bool wrapped = false;

                        if (dt.text[c] == '\n')
                        {
                            x = dt.x;
                            y += Machine.uifontheight + 1;
                            wrapped = true;
                        }
                        else if (dt.text[c] == ' ')
                        {
                            /* don't try to word wrap at the beginning of a line (this would cause */
                            /* an endless loop if a word is longer than a line) */
                            if (x != dt.x)
                            {
                                int nextlen = 0;
                                int nc;

                                nc = c + 1;
                                while (nc < dt.text.Length && dt.text[nc] != '\0' && dt.text[nc] != ' ' && dt.text[nc] != '\n')
                                {
                                    nextlen += Machine.uifontwidth;
                                    nc++;
                                }

                                /* word wrap */
                                if (x + Machine.uifontwidth + nextlen > Machine.uiwidth)
                                {
                                    x = dt.x;
                                    y += Machine.uifontheight + 1;
                                    wrapped = true;
                                }
                            }
                        }

                        if (!wrapped)
                        {
                            drawgfx(Machine.scrbitmap, Machine.uifont, (uint)dt.text[c], (uint)dt.color, false, false, x + Machine.uixmin, y + Machine.uiymin, null, TRANSPARENCY_NONE, 0);
                            x += Machine.uifontwidth;
                        }

                        c++;
                    }
                }
                else
                    break;
            }



            Machine.orientation = trueorientation;

            if (update_screen) osd_update_video_and_audio();
        }

        void drawhline_norotate(int x, int w, int y, ushort color)
        {
            if (Machine.scrbitmap.depth == 16)
            {
                for (int i = x; i < x + w; i++)
                {
                    Machine.scrbitmap.line[y][i * 2] = (byte)color;
                    Machine.scrbitmap.line[y][i * 2 + 1] = (byte)(color >> 8);
                }
            }
            else
                for (int i = 0; i < w; i++)
                    Machine.scrbitmap.line[y][x + i] = (byte)color;

            osd_mark_dirty(x, y, x + w - 1, y, 1);
        }
        void drawvline_norotate(int x, int y, int h, ushort color)
        {
            if (Machine.scrbitmap.depth == 16)
            {
                for (int i = y; i < y + h; i++)
                {
                    Machine.scrbitmap.line[i][x * 2] = (byte)color;
                    Machine.scrbitmap.line[i][x * 2 + 1] = (byte)(color >> 8);
                }
            }
            else
            {
                for (int i = y; i < y + h; i++)
                    Machine.scrbitmap.line[i][x] = (byte)color;
            }

            osd_mark_dirty(x, y, x, y + h - 1, 1);
        }
        void drawhline(int x, int w, int y, ushort color)
        {
            if ((Machine.ui_orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                    y = Machine.scrbitmap.width - y - 1;
                if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                    x = Machine.scrbitmap.height - x - w;

                drawvline_norotate(y, x, w, color);
            }
            else
            {
                if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                    x = Machine.scrbitmap.width - x - w;
                if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                    y = Machine.scrbitmap.height - y - 1;

                drawhline_norotate(x, w, y, color);
            }
        }
        void drawvline(int x, int y, int h, ushort color)
        {
            if ((Machine.ui_orientation & ORIENTATION_SWAP_XY) != 0)
            {
                if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                    y = Machine.scrbitmap.width - y - h;
                if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                    x = Machine.scrbitmap.height - x - 1;

                drawhline_norotate(y, h, x, color);
            }
            else
            {
                if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                    x = Machine.scrbitmap.width - x - 1;
                if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                    y = Machine.scrbitmap.height - y - h;

                drawvline_norotate(x, y, h, color);
            }
        }
        void ui_drawbox(int leftx, int topy, int width, int height)
        {
            if (leftx < 0) leftx = 0;
            if (topy < 0) topy = 0;
            if (width > Machine.uiwidth) width = Machine.uiwidth;
            if (height > Machine.uiheight) height = Machine.uiheight;

            leftx += Machine.uixmin;
            topy += Machine.uiymin;

            ushort black = Machine.uifont.colortable.read16(0);
            ushort white = Machine.uifont.colortable.read16(1);

            drawhline(leftx, width, topy, white);
            drawhline(leftx, width, topy + height - 1, white);
            drawvline(leftx, topy, height, white);
            drawvline(leftx + width - 1, topy, height, white);
            for (int y = topy + 1; y < topy + height - 1; y++) drawhline(leftx + 1, width - 2, y, black);
        }
        int showcopyright()
        {
            int done;
            string buf = sprintf("Usage of emulators in conjunction with ROMs you don't own " +
                    "is forbidden by copyright law.\n\n" +
                    "IF YOU ARE NOT LEGALLY ENTITLED TO PLAY \"%s\" ON THIS EMULATOR, " +
                    "PRESS ESC.\n\n" +
                    "Otherwise, type OK to continue",
                    Machine.gamedrv.description);
            //ui_drawbox(0, 0, 256, 241);
            ui_displaymessagewindow(buf);

            setup_selected = -1;////
            done = 0;
            do
            {
                osd_update_video_and_audio();
                osd_poll_joysticks();
                if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                {
                    setup_selected = 0;////
                    return 1;
                }
                if (keyboard_pressed_memory((int)InputCodes.KEYCODE_O) ||
                        input_ui_pressed((int)ports.inptports.IPT_UI_LEFT))
                    done = 1;
                if (done == 1 && (keyboard_pressed_memory((int)InputCodes.KEYCODE_K) ||
                        input_ui_pressed((int)ports.inptports.IPT_UI_RIGHT)))
                    done = 2;
            } while (done < 2);

            setup_selected = 0;////
            osd_clearbitmap(Machine.scrbitmap);
            osd_update_video_and_audio();

            return 0;
        }
        void init_user_interface()
        {
            setup_menu_init();
            setup_selected = 0;

            onscrd_init();
            osd_selected = 0;

            jukebox_selected = -1;

            single_step = 0;
        }
        const int MAX_SETUPMENU_ITEMS = 20;
        static string[] menu_item = new string[MAX_SETUPMENU_ITEMS];
        static int[] menu_action = new int[MAX_SETUPMENU_ITEMS];
        static int menu_total;

        const int UI_SWITCH = 0, UI_DEFCODE = 1, UI_CODE = 2, UI_ANALOG = 3, UI_CALIBRATE = 4,
            UI_STATS = 5, UI_GAMEINFO = 6, UI_HISTORY = 7, UI_CHEAT = 8, UI_RESET = 9, UI_MEMCARD = 10, UI_EXIT = 11;
        void setup_menu_init()
        {
            menu_total = 0;

            menu_item[menu_total] = "Input (general)"; menu_action[menu_total++] = UI_DEFCODE;
            menu_item[menu_total] = "Input (this game)"; menu_action[menu_total++] = UI_CODE;
            menu_item[menu_total] = "Dip Switches"; menu_action[menu_total++] = UI_SWITCH;

            /* Determine if there are any analog controls */
            {
                int _in;
                int num;

                _in = 0;//Machine.input_ports;

                num = 0;
                while (Machine.input_ports[_in].type != (int)ports.inptports.IPT_END)
                {
                    if (((Machine.input_ports[_in].type & 0xff) > (int)ports.inptports.IPT_ANALOG_START) && ((Machine.input_ports[_in].type & 0xff) < (int)ports.inptports.IPT_ANALOG_END)
                            && !(!options.cheat && (Machine.input_ports[_in].type & ports.IPF_CHEAT) != 0))
                        num++;
                    _in++;
                }

                if (num != 0)
                {
                    menu_item[menu_total] = "Analog Controls"; menu_action[menu_total++] = UI_ANALOG;
                }
            }

            /* Joystick calibration possible? */
            if ((osd_joystick_needs_calibration()))
            {
                menu_item[menu_total] = "Calibrate Joysticks"; menu_action[menu_total++] = UI_CALIBRATE;
            }

            menu_item[menu_total] = "Bookkeeping Info"; menu_action[menu_total++] = UI_STATS;
            menu_item[menu_total] = "Game Information"; menu_action[menu_total++] = UI_GAMEINFO;
            menu_item[menu_total] = "Game History"; menu_action[menu_total++] = UI_HISTORY;

            if (options.cheat)
            {
                menu_item[menu_total] = "Cheat"; menu_action[menu_total++] = UI_CHEAT;
            }

            menu_item[menu_total] = "Reset Game"; menu_action[menu_total++] = UI_RESET;
            menu_item[menu_total] = "Return to Game"; menu_action[menu_total++] = UI_EXIT;

            menu_item[menu_total] = null; /* terminate array */
        }
        const int MAX_OSD_ITEMS = 30;
        delegate void onscrd_func(int increment, int arg);
        onscrd_func[] onscrd_fnc = new onscrd_func[MAX_OSD_ITEMS];
        int[] onscrd_arg = new int[MAX_OSD_ITEMS];
        int onscrd_total_items;

        void onscrd_volume(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_mixervol(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_brightness(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_gamma(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_vector_intensity(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_overclock(int increment, int arg)
        {
            throw new Exception();
        }
        void onscrd_init()
        {
            int item, ch;


            item = 0;

            onscrd_fnc[item] = onscrd_volume;
            onscrd_arg[item] = 0;
            item++;

            for (ch = 0; ch < MIXER_MAX_CHANNELS; ch++)
            {
                if (mixer_get_name(ch) != "")
                {
                    onscrd_fnc[item] = onscrd_mixervol;
                    onscrd_arg[item] = ch;
                    item++;
                }
            }

            if (options.cheat)
            {
                for (ch = 0; ch < cpu_gettotalcpu(); ch++)
                {
                    onscrd_fnc[item] = onscrd_overclock;
                    onscrd_arg[item] = ch;
                    item++;
                }
            }

            onscrd_fnc[item] = onscrd_brightness;
            onscrd_arg[item] = 0;
            item++;

            onscrd_fnc[item] = onscrd_gamma;
            onscrd_arg[item] = 0;
            item++;

            if ((Machine.drv.video_attributes & VIDEO_TYPE_VECTOR) != 0)
            {
                onscrd_fnc[item] = onscrd_vector_intensity;
                onscrd_arg[item] = 0;
                item++;
            }

            onscrd_total_items = item;
        }
        void ui_text(string buf, int x, int y)
        {
            ui_text_ex(buf, buf.Length, x, y, DT_COLOR_WHITE);
        }
        void ui_text_ex(string buf_begin, int buf_end, int x, int y, int color)
        {
            int trueorientation;

            /* hack: force the display into standard orientation to avoid */
            /* rotating the text */
            trueorientation = Machine.orientation;
            Machine.orientation = Machine.ui_orientation;

            for (int i = 0; i < buf_end; ++i)
            {
                drawgfx(Machine.scrbitmap, Machine.uifont, buf_begin[i], (uint)color, false, false,
                        x + Machine.uixmin,
                        y + Machine.uiymin, null, TRANSPARENCY_NONE, 0);
                x += Machine.uifontwidth;
            }

            Machine.orientation = trueorientation;
        }

        int setdipswitches(int selected)
        {
            string[] menu_item = new string[128];
            string[] menu_subitem = new string[128];
            InputPort[] entry = new InputPort[128];
            int[] entryindex = new int[128];
            char[] flag = new char[40];
            int i, sel;
            InputPort[] _in;
            int total;
            int arrowize;


            sel = selected - 1;

            uint switch_name = (uint)ports.inptports.IPT_DIPSWITCH_NAME;
            uint switch_setting = (uint)ports.inptports.IPT_DIPSWITCH_SETTING;
            _in = Machine.input_ports;
            int ini = 0;
            total = 0;
            while (_in[ini].type != (uint)ports.inptports.IPT_END)
            {
                if ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_name && input_port_name(_in, ini) != null &&
                        (_in[ini].type & (uint)ports.IPF_UNUSED) == 0 &&
                        !(!options.cheat && (_in[ini].type & (uint)ports.IPF_CHEAT) != 0))
                {
                    entry[total] = _in[ini];
                    entryindex[total] = ini;
                    menu_item[total] = input_port_name(_in, ini);

                    total++;
                }

                ini++;
            }

            if (total == 0) return 0;

            menu_item[total] = "Return to Main Menu";
            menu_item[total + 1] = null;	/* terminate array */
            total++;


            for (i = 0; i < total; i++)
            {
                flag[i] = '\0'; /* TODO: flag the dip if it's not the real default */
                if (i < total - 1)
                {
                    ini = entryindex[i] + 1;
                    //_in = entry[i] + 1;
                    while ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                            _in[ini].default_value != entry[i].default_value)
                        ini++;

                    if ((_in[ini].type & ~(uint)ports.IPF_MASK) != switch_setting)
                        menu_subitem[i] = "INVALID";
                    else menu_subitem[i] = input_port_name(_in, ini);
                }
                else menu_subitem[i] = null;	/* no subitem */
            }

            arrowize = 0;
            if (sel < total - 1)
            {
                ini = entryindex[i] + 1;
                //_in = entry[sel] + 1;
                while ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                        _in[ini].default_value != entry[sel].default_value)
                    ini++;

                if ((_in[ini].type & ~(uint)ports.IPF_MASK) != switch_setting)
                    /* invalid setting: revert to a valid one */
                    arrowize |= 1;
                else
                {
                    if (((_in[ini - 1]).type & ~(uint)ports.IPF_MASK) == switch_setting &&
                            !(!options.cheat && ((_in[ini - 1]).type & (uint)ports.IPF_CHEAT) != 0))
                        arrowize |= 1;
                }
            }
            if (sel < total - 1)
            {
                ini = entryindex[sel] + 1;
                while ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                        _in[ini].default_value != entry[sel].default_value)
                    ini++;

                if ((_in[ini].type & ~(uint)ports.IPF_MASK) != switch_setting)
                    /* invalid setting: revert to a valid one */
                    arrowize |= 2;
                else
                {
                    if ((_in[ini + 1].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                            !(!options.cheat && ((_in[ini + 1]).type & (uint)ports.IPF_CHEAT) != 0))
                        arrowize |= 2;
                }
            }

            ui_displaymenu(menu_item, menu_subitem, flag, sel, arrowize);

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 8))
                sel = (sel + 1) % total;

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 8))
                sel = (sel + total - 1) % total;

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_RIGHT, 8))
            {
                if (sel < total - 1)
                {
                    ini = entryindex[sel] + 1;
                    while ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                            _in[ini].default_value != entry[sel].default_value)
                        ini++;

                    if ((_in[ini].type & ~(uint)ports.IPF_MASK) != switch_setting)
                        /* invalid setting: revert to a valid one */
                        entry[sel].default_value = (ushort)(_in[entryindex[sel] + 1].default_value & entry[sel].mask);
                    else
                    {
                        if (((_in[ini + 1]).type & ~(uint)ports.IPF_MASK) == switch_setting &&
                                !(!options.cheat && ((_in[ini + 1]).type & (uint)ports.IPF_CHEAT) != 0))
                            entry[sel].default_value = (ushort)((_in[ini + 1]).default_value & entry[sel].mask);
                    }

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;
                }
            }

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_LEFT, 8))
            {
                if (sel < total - 1)
                {
                    ini = entryindex[sel] + 1;
                    while ((_in[ini].type & ~(uint)ports.IPF_MASK) == switch_setting &&
                            _in[ini].default_value != entry[sel].default_value)
                        ini++;

                    if ((_in[ini].type & ~(uint)ports.IPF_MASK) != switch_setting)
                        /* invalid setting: revert to a valid one */
                        entry[sel].default_value = (ushort)(_in[entryindex[sel] + 1].default_value & entry[sel].mask);//(entry[sel]+1).default_value & entry[sel].mask;
                    else
                    {
                        if (((_in[ini - 1]).type & ~(uint)ports.IPF_MASK) == switch_setting &&
                                !(!options.cheat && ((_in[ini - 1]).type & (uint)ports.IPF_CHEAT) != 0))
                            entry[sel].default_value = (ushort)((_in[ini - 1]).default_value & entry[sel].mask);
                    }

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;
                }
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
            {
                if (sel == total - 1) sel = -1;
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                sel = -1;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                sel = -2;

            if (sel == -1 || sel == -2)
            {
                need_to_clear_bitmap = true;
            }

            return sel + 1;
        }
        bool record_first_insert = true;
        string[] menu_subitem_buffer = new string[400];
        int setdefcodesettings(int selected)
        {
            string[] menu_item = new string[400];
            string[] menu_subitem = new string[400];
            ipd[] entry = new ipd[400];
            char[] flag = new char[400];
            int i, sel;
            int _in;
            int total;

            sel = selected - 1;


            if (Machine.input_ports == null)
                return 0;

            _in = 0;

            total = 0;
            while (inputport_defaults[_in].type != (int)ports.inptports.IPT_END)
            {
                if (inputport_defaults[_in].name != null && (inputport_defaults[_in].type & ~(uint)ports.IPF_MASK) != (int)ports.inptports.IPT_UNKNOWN && (inputport_defaults[_in].type & (uint)ports.IPF_UNUSED) == 0
                    && !(!options.cheat && (inputport_defaults[_in].type & (uint)ports.IPF_CHEAT) != 0))
                {
                    entry[total] = inputport_defaults[_in];
                    menu_item[total] = inputport_defaults[_in].name;

                    total++;
                }

                _in++;
            }

            if (total == 0) return 0;

            menu_item[total] = "Return to Main Menu";
            menu_item[total + 1] = null;	/* terminate array */
            total++;

            for (i = 0; i < total; i++)
            {
                if (i < total - 1)
                {
                    seq_name(entry[i].seq, ref menu_subitem_buffer[i], 100);//menu_subitem_buffer[0]);
                    menu_subitem[i] = menu_subitem_buffer[i];
                }
                else
                    menu_subitem[i] = null;	/* no subitem */
                flag[i] = '\0';
            }

            if (sel > SEL_MASK)   /* are we waiting for a new key? */
            {
                int ret;

                menu_subitem[sel & SEL_MASK] = "    ";
                ui_displaymenu(menu_item, menu_subitem, flag, sel & SEL_MASK, 3);

                ret = seq_read_async(entry[sel & SEL_MASK].seq, record_first_insert);

                if (ret >= 0)
                {
                    sel &= 0xff;

                    if (ret > 0 || seq_get_1(entry[sel].seq) == (uint)InputCodes.CODE_NONE)
                    {
                        seq_set_1(entry[sel].seq, (uint)InputCodes.CODE_NONE);
                        ret = 1;
                    }

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;

                    record_first_insert = ret != 0;
                }


                return sel + 1;
            }


            ui_displaymenu(menu_item, menu_subitem, flag, sel, 0);

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 8))
            {
                sel = (sel + 1) % total;
                record_first_insert = true;
            }

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 8))
            {
                sel = (sel + total - 1) % total;
                record_first_insert = true;
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
            {
                if (sel == total - 1) sel = -1;
                else
                {
                    seq_read_async_start();

                    sel |= 1 << SEL_BITS;	/* we'll ask for a key */

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;
                }
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                sel = -1;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                sel = -2;

            if (sel == -1 || sel == -2)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;

                record_first_insert = true;
            }

            return sel + 1;
        }
        int setcodesettings(int selected)
        {
            string[] menu_item = new string[400];
            string[] menu_subitem = new string[400];
            InputPort[] entry = new InputPort[400];
            char[] flag = new char[400];
            int i, sel;
            int _in;
            int total;


            sel = selected - 1;


            if (Machine.input_ports == null)
                return 0;

            _in = 0;

            total = 0;
            while (Machine.input_ports[_in].type != (int)ports.inptports.IPT_END)
            {
                if (input_port_name(Machine.input_ports, _in) != null && seq_get_1(Machine.input_ports[_in].seq) != (uint)InputCodes.CODE_NONE && (Machine.input_ports[_in].type & ~(uint)ports.IPF_MASK) != (int)ports.inptports.IPT_UNKNOWN)
                {
                    entry[total] = Machine.input_ports[_in];
                    menu_item[total] = input_port_name(Machine.input_ports, _in);

                    total++;
                }

                _in++;
            }

            if (total == 0) return 0;

            menu_item[total] = "Return to Main Menu";
            menu_item[total + 1] = null;	/* terminate array */
            total++;

            for (i = 0; i < total; i++)
            {
                if (i < total - 1)
                {
                    seq_name(input_port_seq(entry, i), ref menu_subitem_buffer[i], 100);//sizeof(menu_subitem_buffer[0]));
                    menu_subitem[i] = menu_subitem_buffer[i];

                    /* If the key isn't the default, flag it */
                    if (seq_get_1(entry[i].seq) != (uint)InputCodes.CODE_DEFAULT)
                        flag[i] = (char)1;
                    else
                        flag[i] = '\0';

                }
                else
                    menu_subitem[i] = null;	/* no subitem */
            }

            if (sel > SEL_MASK)   /* are we waiting for a new key? */
            {
                int ret;

                menu_subitem[sel & SEL_MASK] = "    ";
                ui_displaymenu(menu_item, menu_subitem, flag, sel & SEL_MASK, 3);

                ret = seq_read_async(entry[sel & SEL_MASK].seq, record_first_insert);

                if (ret >= 0)
                {
                    sel &= 0xff;

                    if (ret > 0 || seq_get_1(entry[sel].seq) == (uint)InputCodes.CODE_NONE)
                    {
                        seq_set_1(entry[sel].seq, (uint)InputCodes.CODE_DEFAULT);
                        ret = 1;
                    }

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;

                    record_first_insert = ret != 0;
                }

                return sel + 1;
            }


            ui_displaymenu(menu_item, menu_subitem, flag, sel, 0);

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 8))
            {
                sel = (sel + 1) % total;
                record_first_insert = true;
            }

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 8))
            {
                sel = (sel + total - 1) % total;
                record_first_insert = true;
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
            {
                if (sel == total - 1) sel = -1;
                else
                {
                    seq_read_async_start();

                    sel |= 1 << SEL_BITS;	/* we'll ask for a key */

                    /* tell updatescreen() to clean after us (in case the window changes size) */
                    need_to_clear_bitmap = true;
                }
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                sel = -1;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                sel = -2;

            if (sel == -1 || sel == -2)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;

                record_first_insert = true;
            }

            return sel + 1;
        }
        int calibratejoysticks(int selected)
        {
            throw new Exception();
        }
        int settraksettings(int selected)
        {
            throw new Exception();
        }
        int mame_stats(int selected)
        {
            int sel = selected - 1;

            string buf = "";

            if (dispensed_tickets != 0)
            {
                buf = "Tickets dispensed: " + sprintf("%d\n\n", dispensed_tickets);
            }

            for (int i = 0; i < COIN_COUNTERS; i++)
            {
                buf += sprintf("Coin %c: ", i + 'A');
                if (coins[i] == 0)
                    buf += "NA";
                else
                {
                    buf += sprintf("%d", coins[i]);
                }
                if (coinlockedout[i] != 0)
                {
                    buf += " (locked)\n";
                }
                else
                {
                    buf += "\n";
                }
            }

            {
                /* menu system, use the normal menu keys */
                buf += "\n\t\x1a Return to Main Menu \x1b";

                ui_displaymessagewindow(buf);

                if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                    sel = -2;
            }

            if (sel == -1 || sel == -2)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;
            }

            return sel + 1;
        }
        void wordwrap_text_buffer(ref string buffer, int maxwidth)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(buffer);
            int width = 0;
            int bi = 0;
            while (bi < buf.Length && buf[bi] != '\0')
            {
                if (buf[bi] == '\n')
                {
                    bi++;
                    width = 0;
                    continue;
                }

                width++;

                if (width > maxwidth)
                {
                    /* backtrack until a space is found */
                    while (buf[bi] != ' ')
                    {
                        bi--;
                        width--;
                    }
                    if (width < 1) return;	/* word too long */

                    /* replace space with a newline */
                    buf[bi] = '\n';
                }
                else
                    bi++;
            }
            buffer = buf.ToString();
        }
        int displayhistory(int selected)
        {
#if !MESS
            string msg = "\tHistory not available\n\n\t\x1a Return to Main Menu \x1b";
#else
	char *msg = "\tSysInfo.dat Missing\n\n\t\x1a Return to Main Menu \x1b";
#endif
            int scroll = 0;
            string buf = null;
            int maxcols, maxrows;
            int sel;


            sel = selected - 1;


            maxcols = (Machine.uiwidth / Machine.uifontwidth) - 1;
            maxrows = (2 * Machine.uiheight - Machine.uifontheight) / (3 * Machine.uifontheight);
            maxcols -= 2;
            maxrows -= 8;

            if (buf == null)
            {
                /* allocate a buffer for the text */
                buf = "";
                {
                    /* try to load entry */
                    if (!load_driver_history(Machine.gamedrv, ref buf))
                    {
                        scroll = 0;
                        wordwrap_text_buffer(ref buf, maxcols);
                        buf += "\n\t\x1a Return to Main Menu \x1b\n";
                    }
                    else
                    {
                        buf = null;
                    }
                }
            }

            {
                if (buf != null)
                    display_scroll_message(ref scroll, maxcols, maxrows, buf);
                else
                    ui_displaymessagewindow(msg);

                if ((scroll > 0) && input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 4))
                {
                    if (scroll == 2) scroll = 0;	/* 1 would be the same as 0, but with arrow on top */
                    else scroll--;
                }

                if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 4))
                {
                    if (scroll == 0) scroll = 2;	/* 1 would be the same as 0, but with arrow on top */
                    else scroll++;
                }

                if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                    sel = -1;

                if (input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                    sel = -2;
            }

            if (sel == -1 || sel == -2)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;

                /* force buffer to be recreated */

            }

            return sel + 1;
        }
        int count_lines_in_buffer(string buffer)
        {
            int lines = 0;
            char c = '\0';
            int bi = 0;
            while (bi < buffer.Length)
            {
                c = buffer[bi++];
                if (c == '\n') lines++;
            }
            return lines;
        }
        void display_scroll_message(ref int scroll, int width, int height, string buf)
        {
            DisplayText[] dt = new DisplayText[256];
            int curr_dt = 0;
            string uparrow = "\x18";
            string downarrow = "\x19";
            string textcopy = "";
            string copy;
            int leftoffs, topoffs;
            int first = scroll;
            int buflines, showlines;
            int i;
            int bi = 0;

            /* draw box */
            leftoffs = (Machine.uiwidth - Machine.uifontwidth * (width + 1)) / 2;
            if (leftoffs < 0) leftoffs = 0;
            topoffs = (Machine.uiheight - (3 * height + 1) * Machine.uifontheight / 2) / 2;
            ui_drawbox(leftoffs, topoffs, (width + 1) * Machine.uifontwidth, (3 * height + 1) * Machine.uifontheight / 2);

            buflines = count_lines_in_buffer(buf);
            if (first > 0)
            {
                if (buflines <= height)
                    first = 0;
                else
                {
                    height--;
                    if (first > (buflines - height))
                        first = buflines - height;
                }
                scroll = first;
            }

            if (first != 0)
            {
                /* indicate that scrolling upward is possible */
                dt[curr_dt].text = uparrow;
                dt[curr_dt].color = DT_COLOR_WHITE;
                dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * uparrow.Length) / 2;
                dt[curr_dt].y = topoffs + (3 * curr_dt + 1) * Machine.uifontheight / 2;
                curr_dt++;
            }

            if ((buflines - first) > height)
                showlines = height - 1;
            else
                showlines = height;


            /* skip to first line */
            while (first > 0)
            {
                char c;

                while (bi < buf.Length)
                {
                    c = buf[bi++];
                    if (c == '\n')
                    {
                        first--;
                        break;
                    }
                }
            }

            /* copy 'showlines' lines from buffer, starting with line 'first' */
            copy = textcopy;
            for (i = 0; i < showlines; i++)
            {
                string copystart = copy;

                while (bi < buf.Length && buf[bi] != '\n')
                {
                    copystart += buf[bi++];
                }
                copystart += '\0';
                if (buf[bi] == '\n')
                    bi++;

                if (copystart[0] == '\t') /* center text */
                {
                    dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * (copy.Length - (copystart.Length - 1))) / 2;
                }
                else
                    dt[curr_dt].x = leftoffs + Machine.uifontwidth / 2;

                dt[curr_dt].text = copystart;
                dt[curr_dt].color = DT_COLOR_WHITE;
                dt[curr_dt].y = topoffs + (3 * curr_dt + 1) * Machine.uifontheight / 2;
                curr_dt++;
            }

            if (showlines == (height - 1))
            {
                /* indicate that scrolling downward is possible */
                dt[curr_dt].text = downarrow;
                dt[curr_dt].color = DT_COLOR_WHITE;
                dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * downarrow.Length) / 2;
                dt[curr_dt].y = topoffs + (3 * curr_dt + 1) * Machine.uifontheight / 2;
                curr_dt++;
            }

            dt[curr_dt].text = null;	/* terminate array */

            displaytext(dt, false, false);
        }
        static int menu_lastselected = 0;
        int setup_menu(int selected)
        {
            int sel, res;



            if (selected == -1)
                sel = menu_lastselected;
            else sel = selected - 1;

            if (sel > SEL_MASK)
            {
                switch (menu_action[sel & SEL_MASK])
                {
                    case UI_SWITCH:
                        res = setdipswitches(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

                    case UI_DEFCODE:
                        res = setdefcodesettings(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

                    case UI_CODE:
                        res = setcodesettings(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

                    case UI_ANALOG:
                        res = settraksettings(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

                    case UI_CALIBRATE:
                        res = calibratejoysticks(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;


#if !MESS
                    case UI_STATS:
                        res = mame_stats(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;
#endif

                    case UI_GAMEINFO:
                        res = displaygameinfo(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

#if MESS
			case UI_IMAGEINFO:
				res = displayimageinfo(sel >> SEL_BITS);
				if (res == -1)
				{
					menu_lastselected = sel;
					sel = -1;
				}
				else
					sel = (sel & SEL_MASK) | (res << SEL_BITS);
				break;
			case UI_FILEMANAGER:
				res = filemanager(sel >> SEL_BITS);
				if (res == -1)
				{
					menu_lastselected = sel;
					sel = -1;
				}
				else
					sel = (sel & SEL_MASK) | (res << SEL_BITS);
				break;
			case UI_TAPECONTROL:
				res = tapecontrol(sel >> SEL_BITS);
				if (res == -1)
				{
					menu_lastselected = sel;
					sel = -1;
				}
				else
					sel = (sel & SEL_MASK) | (res << SEL_BITS);
				break;
#endif

                    case UI_HISTORY:
                        res = displayhistory(sel >> SEL_BITS);
                        if (res == -1)
                        {
                            menu_lastselected = sel;
                            sel = -1;
                        }
                        else
                            sel = (sel & SEL_MASK) | (res << SEL_BITS);
                        break;

                    case UI_CHEAT:
                        osd_sound_enable(false);
                        while (seq_pressed(input_port_type_seq((int)ports.inptports.IPT_UI_SELECT)))
                            osd_update_video_and_audio();	  /* give time to the sound hardware to apply the volume change */
                        cheat_menu();
                        osd_sound_enable(true);
                        sel = sel & SEL_MASK;
                        break;


                }

                return sel + 1;
            }


            ui_displaymenu(menu_item, null, null, sel, 0);

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 8))
                sel = (sel + 1) % menu_total;

            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 8))
                sel = (sel + menu_total - 1) % menu_total;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_SELECT))
            {
                switch (menu_action[sel])
                {
                    case UI_SWITCH:
                    case UI_DEFCODE:
                    case UI_CODE:
                    case UI_ANALOG:
                    case UI_CALIBRATE:
#if !MESS
                    case UI_STATS:
                    case UI_GAMEINFO:
#else
			case UI_GAMEINFO:
			case UI_IMAGEINFO:
			case UI_FILEMANAGER:
			case UI_TAPECONTROL:
#endif
                    case UI_HISTORY:
                    case UI_CHEAT:
                    case UI_MEMCARD:
                        sel |= 1 << SEL_BITS;
                        /* tell updatescreen() to clean after us */
                        need_to_clear_bitmap = true;
                        break;

                    case UI_RESET:
                        machine_reset();
                        break;

                    case UI_EXIT:
                        menu_lastselected = 0;
                        sel = -1;
                        break;
                }
            }

            if (input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL) ||
                    input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
            {
                menu_lastselected = sel;
                sel = -1;
            }

            if (sel == -1)
            {
                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;
            }

            return sel + 1;
        }

        void displaymessage(string text)
        {
            throw new Exception();
        }
        static int lastselected = 0;
        int on_screen_display(int selected)
        {
            int increment, sel;

            if (selected == -1)
                sel = lastselected;
            else sel = selected - 1;

            increment = 0;
            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_LEFT, 8))
                increment = -1;
            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_RIGHT, 8))
                increment = 1;
            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 8))
                sel = (sel + 1) % onscrd_total_items;
            if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 8))
                sel = (sel + onscrd_total_items - 1) % onscrd_total_items;

            onscrd_fnc[sel](increment, onscrd_arg[sel]);

            lastselected = sel;

            if (input_ui_pressed((int)ports.inptports.IPT_UI_ON_SCREEN_DISPLAY))
            {
                sel = -1;

                /* tell updatescreen() to clean after us */
                need_to_clear_bitmap = true;
            }

            return sel + 1;
        }
        static int show_profiler;
        bool handle_user_interface()
        {

            /* if the user pressed F12, save the screen to a file */
            if (input_ui_pressed((int)ports.inptports.IPT_UI_SNAPSHOT))
                osd_save_snapshot();

            /* This call is for the cheat, it must be called at least each frames */
            if (options.cheat) DoCheat();

            /* if the user pressed ESC, stop the emulation */
            /* but don't quit if the setup menu is on screen */
            if (setup_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                return true;

            if (setup_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
            {
                setup_selected = -1;
                if (osd_selected != 0)
                {
                    osd_selected = 0;	/* disable on screen display */
                    /* tell updatescreen() to clean after us */
                    need_to_clear_bitmap = true;
                }
            }
            if (setup_selected != 0) setup_selected = setup_menu(setup_selected);

            if (mame_debug == 0 && osd_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_ON_SCREEN_DISPLAY))
            {
                osd_selected = -1;
                if (setup_selected != 0)
                {
                    setup_selected = 0; /* disable setup menu */
                    /* tell updatescreen() to clean after us */
                    need_to_clear_bitmap = true;
                }
            }
            if (osd_selected != 0) osd_selected = on_screen_display(osd_selected);


#if false
	if (keyboard_pressed_memory(KEYCODE_BACKSPACE))
	{
		if (jukebox_selected != -1)
		{
			jukebox_selected = -1;
			cpu_halt(0,1);
		}
		else
		{
			jukebox_selected = 0;
			cpu_halt(0,0);
		}
	}

	if (jukebox_selected != -1)
	{
		char buf[40];
		watchdog_reset_w(0,0);
		if (keyboard_pressed_memory(KEYCODE_LCONTROL))
		{
//#include "cpu/z80/z80.h"
			soundlatch_w(0,jukebox_selected);
			cpu_cause_interrupt(1,Z80_NMI_INT);
		}
		if (input_ui_pressed_repeat(IPT_UI_RIGHT,8))
		{
			jukebox_selected = (jukebox_selected + 1) & 0xff;
		}
		if (input_ui_pressed_repeat(IPT_UI_LEFT,8))
		{
			jukebox_selected = (jukebox_selected - 1) & 0xff;
		}
		if (input_ui_pressed_repeat(IPT_UI_UP,8))
		{
			jukebox_selected = (jukebox_selected + 16) & 0xff;
		}
		if (input_ui_pressed_repeat(IPT_UI_DOWN,8))
		{
			jukebox_selected = (jukebox_selected - 16) & 0xff;
		}
		sprintf(buf,"sound cmd %02x",jukebox_selected);
		displaymessage(buf);
	}
#endif


            /* if the user pressed F3, reset the emulation */
            if (input_ui_pressed((int)ports.inptports.IPT_UI_RESET_MACHINE))
                machine_reset();


            if (single_step != 0 || input_ui_pressed((int)ports.inptports.IPT_UI_PAUSE)) /* pause the game */
            {
                /*		osd_selected = 0;	   disable on screen display, since we are going   */
                /* to change parameters affected by it */

                if (single_step == 0)
                {
                    osd_sound_enable(false);
                    osd_pause(true);
                }

                while (!input_ui_pressed((int)ports.inptports.IPT_UI_PAUSE))
                {
                    if (osd_skip_this_frame() == 0)
                    {
                        if (need_to_clear_bitmap || bitmap_dirty != 0)
                        {
                            osd_clearbitmap(Machine.scrbitmap);
                            need_to_clear_bitmap = false;
                            Machine.drv.vh_update(Machine.scrbitmap, bitmap_dirty);
                            bitmap_dirty = 0;
                        }
                    }

                    if (input_ui_pressed((int)ports.inptports.IPT_UI_SNAPSHOT))
                        osd_save_snapshot();

                    if (setup_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL))
                        return true;

                    if (setup_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_CONFIGURE))
                    {
                        setup_selected = -1;
                        if (osd_selected != 0)
                        {
                            osd_selected = 0;	/* disable on screen display */
                            /* tell updatescreen() to clean after us */
                            need_to_clear_bitmap = true;
                        }
                    }
                    if (setup_selected != 0) setup_selected = setup_menu(setup_selected);

                    if (mame_debug == 0 && osd_selected == 0 && input_ui_pressed((int)ports.inptports.IPT_UI_ON_SCREEN_DISPLAY))
                    {
                        osd_selected = -1;
                        if (setup_selected != 0)
                        {
                            setup_selected = 0; /* disable setup menu */
                            /* tell updatescreen() to clean after us */
                            need_to_clear_bitmap = true;
                        }
                    }
                    if (osd_selected != 0) osd_selected = on_screen_display(osd_selected);

                    /* show popup message if any */
                    if (messagecounter > 0) displaymessage(messagetext);

                    osd_update_video_and_audio();
                    osd_poll_joysticks();
                }

                if (code_pressed((int)InputCodes.KEYCODE_LSHIFT) || code_pressed((int)InputCodes.KEYCODE_RSHIFT))
                    single_step = 1;
                else
                {
                    single_step = 0;
                    osd_pause(false);
                    osd_sound_enable(true);
                }
            }


            /* show popup message if any */
            if (messagecounter > 0)
            {
                displaymessage(messagetext);

                if (--messagecounter == 0)
                    /* tell updatescreen() to clean after us */
                    need_to_clear_bitmap = true;
            }


            if (input_ui_pressed((int)ports.inptports.IPT_UI_SHOW_PROFILER))
            {
                show_profiler ^= 1;
                if (show_profiler != 0)
                    profiler_start();
                else
                {
                    profiler_stop();
                    /* tell updatescreen() to clean after us */
                    need_to_clear_bitmap = true;
                }
            }

            if (show_profiler != 0) profiler_show();


            /* if the user pressed F4, show the character set */
            if (input_ui_pressed((int)ports.inptports.IPT_UI_SHOW_GFX))
            {
                osd_sound_enable(false);

                showcharset();

                osd_sound_enable(true);
            }

            return false;
        }
        void showcharset()
        {
            int i;
            string buf = "";
            int bank, color, firstdrawn;
            int palpage;
            int trueorientation;
            int changed;
            int game_is_neogeo = 0;
            byte[] orig_used_colors = null;


            if (palette_used_colors != null)
            {
                orig_used_colors = new byte[Machine.drv.total_colors];
                if (orig_used_colors == null) return;
                Buffer.BlockCopy(palette_used_colors.buffer, (int)palette_used_colors.offset, orig_used_colors, 0, (int)Machine.drv.total_colors);
                //memcpy(orig_used_colors,palette_used_colors,Machine.drv.total_colors );
            }

            bank = -1;
            color = 0;
            firstdrawn = 0;
            palpage = 0;

            changed = 1;

            do
            {
                int cpx, cpy, skip_chars;

                if (bank >= 0)
                {
                    cpx = Machine.uiwidth / Machine.gfx[bank].width;
                    cpy = (Machine.uiheight - Machine.uifontheight) / Machine.gfx[bank].height;
                    skip_chars = cpx * cpy;
                }
                else cpx = cpy = skip_chars = 0;

                if (changed != 0)
                {
                    int lastdrawn = 0;

                    osd_clearbitmap(Machine.scrbitmap);

                    /* validity chack after char bank change */
                    if (bank >= 0)
                    {
                        if (firstdrawn >= Machine.gfx[bank].total_elements)
                        {
                            firstdrawn = (int)(Machine.gfx[bank].total_elements - skip_chars);
                            if (firstdrawn < 0) firstdrawn = 0;
                        }
                    }

                    if (bank != 2 || game_is_neogeo == 0)
                    {
                        if (bank >= 0)
                        {
                            int table_offs;
                            bool flipx, flipy;

                            /* hack: force the display into standard orientation to avoid */
                            /* rotating the user interface */
                            trueorientation = Machine.orientation;
                            Machine.orientation = Machine.ui_orientation;

                            if (palette_used_colors != null)
                            {
                                memset(palette_used_colors, PALETTE_COLOR_TRANSPARENT, (int)Machine.drv.total_colors);
                                table_offs = (int)(Machine.gfx[bank].colortable.offset - Machine.remapped_colortable.offset
                                        + Machine.gfx[bank].color_granularity * color);
                                for (i = 0; i < Machine.gfx[bank].color_granularity; i++)
                                    palette_used_colors[Machine.game_colortable.read16(table_offs + i)] = PALETTE_COLOR_USED;
                                palette_recalc();	/* do it twice in case of previous overflow */
                                palette_recalc();	/*(we redraw the screen only when it changes) */
                            }

#if !PREROTATE_GFX
                            flipx = ((Machine.orientation ^ trueorientation) & ORIENTATION_FLIP_X) != 0;
                            flipy = ((Machine.orientation ^ trueorientation) & ORIENTATION_FLIP_Y) != 0;

                            if ((Machine.orientation & ORIENTATION_SWAP_XY) != 0)
                            {
                                bool t;
                                t = flipx; flipx = flipy; flipy = t;
                            }
#else
					flipx = false;
					flipy = false;
#endif

                            for (i = 0; i + firstdrawn < Machine.gfx[bank].total_elements && i < cpx * cpy; i++)
                            {
                                drawgfx(Machine.scrbitmap, Machine.gfx[bank],
                                        (uint)(i + firstdrawn), (uint)color,  /*sprite num, color*/
                                        flipx, flipy,
                                        (i % cpx) * Machine.gfx[bank].width + Machine.uixmin,
                                        Machine.uifontheight + (i / cpx) * Machine.gfx[bank].height + Machine.uiymin,
                                        null, TRANSPARENCY_NONE, 0);

                                lastdrawn = i + firstdrawn;
                            }

                            Machine.orientation = trueorientation;
                        }
                        else
                        {
                            int sx, sy, x, y, colors;

                            colors = (int)(Machine.drv.total_colors - 256 * palpage);
                            if (colors > 256) colors = 256;
                            if (palette_used_colors != null)
                            {
                                memset(palette_used_colors, PALETTE_COLOR_UNUSED, (int)Machine.drv.total_colors);
                                memset(new _BytePtr(palette_used_colors, (256 * palpage)), PALETTE_COLOR_USED, colors);
                                palette_recalc();	/* do it twice in case of previous overflow */
                                palette_recalc();	/*(we redraw the screen only when it changes) */
                            }

                            for (i = 0; i < 16; i++)
                            {
                                sx = 3 * Machine.uifontwidth + (Machine.uifontwidth * 4 / 3) * (i % 16);
                                string bf = sprintf("%X", i);
                                ui_text(bf, sx, 2 * Machine.uifontheight);
                                if (16 * i < colors)
                                {
                                    sy = 3 * Machine.uifontheight + (Machine.uifontheight) * (i % 16);
                                    bf = sprintf("%3X", i + 16 * palpage);
                                    ui_text(bf, 0, sy);
                                }
                            }

                            for (i = 0; i < colors; i++)
                            {
                                sx = Machine.uixmin + 3 * Machine.uifontwidth + (Machine.uifontwidth * 4 / 3) * (i % 16);
                                sy = Machine.uiymin + 2 * Machine.uifontheight + (Machine.uifontheight) * (i / 16) + Machine.uifontheight;
                                for (y = 0; y < Machine.uifontheight; y++)
                                {
                                    for (x = 0; x < Machine.uifontwidth * 4 / 3; x++)
                                    {
                                        int tx, ty;
                                        if ((Machine.ui_orientation & ORIENTATION_SWAP_XY) != 0)
                                        {
                                            ty = sx + x;
                                            tx = sy + y;
                                        }
                                        else
                                        {
                                            tx = sx + x;
                                            ty = sy + y;
                                        }
                                        if ((Machine.ui_orientation & ORIENTATION_FLIP_X) != 0)
                                            tx = Machine.scrbitmap.width - 1 - tx;
                                        if ((Machine.ui_orientation & ORIENTATION_FLIP_Y) != 0)
                                            ty = Machine.scrbitmap.height - 1 - ty;

                                        if (Machine.scrbitmap.depth == 16)
                                            throw new Exception();//Machine.scrbitmap.line[ty].write16(tx, Machine.pens[i + 256*palpage]);
                                        else
                                            Machine.scrbitmap.line[ty][tx] = (byte)Machine.pens.read16(i + 256 * palpage);
                                    }
                                }
                            }
                        }
                    }


                    if (bank >= 0)
                        buf = sprintf("GFXSET %d COLOR %2X CODE %X-%X", bank, color, firstdrawn, lastdrawn);
                    else
                        buf = "PALETTE";
                    ui_text(buf, 0, 0);

                    changed = 0;
                }

                /* Necessary to keep the video from getting stuck if a frame happens to be skipped in here */
                /* I beg to differ - the OS dependant code must not assume that */
                /* osd_skip_this_frame() is called before osd_update_video_and_audio() - NS */
                //		osd_skip_this_frame();
                osd_update_video_and_audio();

                if (code_pressed((int)InputCodes.KEYCODE_LCONTROL) || code_pressed((int)InputCodes.KEYCODE_RCONTROL))
                {
                    skip_chars = cpx;
                }
                if (code_pressed((int)InputCodes.KEYCODE_LSHIFT) || code_pressed((int)InputCodes.KEYCODE_RSHIFT))
                {
                    skip_chars = 1;
                }


                if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_RIGHT, 8))
                {
                    if (bank + 1 < MAX_GFX_ELEMENTS && Machine.gfx[bank + 1] != null)
                    {
                        bank++;
                        //				firstdrawn = 0;
                        changed = 1;
                    }
                }

                if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_LEFT, 8))
                {
                    if (bank > -1)
                    {
                        bank--;
                        //				firstdrawn = 0;
                        changed = 1;
                    }
                }

                if (code_pressed_memory_repeat((int)InputCodes.KEYCODE_PGDN, 4))
                {
                    if (bank >= 0)
                    {
                        if (firstdrawn + skip_chars < Machine.gfx[bank].total_elements)
                        {
                            firstdrawn += skip_chars;
                            changed = 1;
                        }
                    }
                    else
                    {
                        if (256 * (palpage + 1) < Machine.drv.total_colors)
                        {
                            palpage++;
                            changed = 1;
                        }
                    }
                }

                if (code_pressed_memory_repeat((int)InputCodes.KEYCODE_PGUP, 4))
                {
                    if (bank >= 0)
                    {
                        firstdrawn -= skip_chars;
                        if (firstdrawn < 0) firstdrawn = 0;
                        changed = 1;
                    }
                    else
                    {
                        if (palpage > 0)
                        {
                            palpage--;
                            changed = 1;
                        }
                    }
                }

                if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_UP, 6))
                {
                    if (bank >= 0)
                    {
                        if (color < Machine.gfx[bank].total_colors - 1)
                        {
                            color++;
                            changed = 1;
                        }
                    }
                }

                if (input_ui_pressed_repeat((int)ports.inptports.IPT_UI_DOWN, 6))
                {
                    if (color > 0)
                    {
                        color--;
                        changed = 1;
                    }
                }

                if (input_ui_pressed((int)ports.inptports.IPT_UI_SNAPSHOT))
                    osd_save_snapshot();
            } while (!input_ui_pressed((int)ports.inptports.IPT_UI_SHOW_GFX) &&
                    !input_ui_pressed((int)ports.inptports.IPT_UI_CANCEL));

            /* clear the screen before returning */
            osd_clearbitmap(Machine.scrbitmap);

            if (palette_used_colors != null)
            {
                /* this should force a full refresh by the video driver */
                memset(palette_used_colors, PALETTE_COLOR_TRANSPARENT, (int)Machine.drv.total_colors);
                palette_recalc();
                /* restore the game used colors array */
                //memcpy(palette_used_colors,orig_used_colors,Machine.drv.total_colors);
                Buffer.BlockCopy(orig_used_colors, 0, palette_used_colors.buffer, (int)palette_used_colors.offset, (int)Machine.drv.total_colors);
                orig_used_colors = null;
            }

            return;
        }
        void ui_displaymenu(string[] items, string[] subitems, char[] flag, int selected, int arrowize_subitem)
        {
            DisplayText[] dt = new DisplayText[256];
            int curr_dt;
            string lefthilight = "\x1a";
            string righthilight = "\x1b";
            string uparrow = "\x18";
            string downarrow = "\x19";
            string leftarrow = "\x11";
            string rightarrow = "\x10";
            int i, count, len, maxlen, highlen;
            int leftoffs, topoffs, visible, topitem;
            int selected_long;


            i = 0;
            maxlen = 0;
            highlen = Machine.uiwidth / Machine.uifontwidth;
            while (items[i] != null)
            {
                len = 3 + items[i].Length;
                if (subitems != null && subitems[i] != null)
                    len += 2 + subitems[i].Length;
                if (len > maxlen && len <= highlen)
                    maxlen = len;
                i++;
            }
            count = i;

            visible = Machine.uiheight / (3 * Machine.uifontheight / 2) - 1;
            topitem = 0;
            if (visible > count) visible = count;
            else
            {
                topitem = selected - visible / 2;
                if (topitem < 0) topitem = 0;
                if (topitem > count - visible) topitem = count - visible;
            }

            leftoffs = (Machine.uiwidth - maxlen * Machine.uifontwidth) / 2;
            topoffs = (Machine.uiheight - (3 * visible + 1) * Machine.uifontheight / 2) / 2;

            /* black background */
            ui_drawbox(leftoffs, topoffs, maxlen * Machine.uifontwidth, (3 * visible + 1) * Machine.uifontheight / 2);

            selected_long = 0;
            curr_dt = 0;
            for (i = 0; i < visible; i++)
            {
                int item = i + topitem;

                if (i == 0 && item > 0)
                {
                    dt[curr_dt].text = uparrow;
                    dt[curr_dt].color = DT_COLOR_WHITE;
                    dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * uparrow.Length) / 2;
                    dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                    curr_dt++;
                }
                else if (i == visible - 1 && item < count - 1)
                {
                    dt[curr_dt].text = downarrow;
                    dt[curr_dt].color = DT_COLOR_WHITE;
                    dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * downarrow.Length) / 2;
                    dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                    curr_dt++;
                }
                else
                {
                    if (subitems != null && subitems[item] != null)
                    {
                        int sublen;
                        len = items[item].Length;
                        dt[curr_dt].text = items[item];
                        dt[curr_dt].color = DT_COLOR_WHITE;
                        dt[curr_dt].x = leftoffs + 3 * Machine.uifontwidth / 2;
                        dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                        curr_dt++;
                        sublen = subitems[item].Length;
                        if (sublen > maxlen - 5 - len)
                        {
                            dt[curr_dt].text = "...";
                            sublen = dt[curr_dt].text.Length;
                            if (item == selected)
                                selected_long = 1;
                        }
                        else
                        {
                            dt[curr_dt].text = subitems[item];
                        }
                        /* If this item is flagged, draw it in inverse print */
                        dt[curr_dt].color = (flag != null && flag[item] != 0) ? DT_COLOR_YELLOW : DT_COLOR_WHITE;
                        dt[curr_dt].x = leftoffs + Machine.uifontwidth * (maxlen - 1 - sublen) - Machine.uifontwidth / 2;
                        dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                        curr_dt++;
                    }
                    else
                    {
                        dt[curr_dt].text = items[item];
                        dt[curr_dt].color = DT_COLOR_WHITE;
                        dt[curr_dt].x = (Machine.uiwidth - Machine.uifontwidth * items[item].Length) / 2;
                        dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                        curr_dt++;
                    }
                }
            }

            i = selected - topitem;
            if (subitems != null && subitems[selected] != null && arrowize_subitem != 0)
            {
                if ((arrowize_subitem & 1) != 0)
                {
                    dt[curr_dt].text = leftarrow;
                    dt[curr_dt].color = DT_COLOR_WHITE;
                    dt[curr_dt].x = leftoffs + Machine.uifontwidth * (maxlen - 2 - subitems[selected].Length) - Machine.uifontwidth / 2 - 1;
                    dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                    curr_dt++;
                }
                if ((arrowize_subitem & 2) != 0)
                {
                    dt[curr_dt].text = rightarrow;
                    dt[curr_dt].color = DT_COLOR_WHITE;
                    dt[curr_dt].x = leftoffs + Machine.uifontwidth * (maxlen - 1) - Machine.uifontwidth / 2;
                    dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                    curr_dt++;
                }
            }
            else
            {
                dt[curr_dt].text = righthilight;
                dt[curr_dt].color = DT_COLOR_WHITE;
                dt[curr_dt].x = leftoffs + Machine.uifontwidth * (maxlen - 1) - Machine.uifontwidth / 2;
                dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
                curr_dt++;
            }
            dt[curr_dt].text = lefthilight;
            dt[curr_dt].color = DT_COLOR_WHITE;
            dt[curr_dt].x = leftoffs + Machine.uifontwidth / 2;
            dt[curr_dt].y = topoffs + (3 * i + 1) * Machine.uifontheight / 2;
            curr_dt++;

            dt[curr_dt].text = null;	/* terminate array */

            displaytext(dt, false, false);

            if (selected_long != 0)
            {
                int long_dx = 0;
                int long_dy = 0;
                int long_x;
                int long_y;
                uint long_max;

                long_max = (uint)((Machine.uiwidth / Machine.uifontwidth) - 2);
                multilinebox_size(ref long_dx, ref long_dy, subitems[selected], selected, selected + subitems[selected].Length, long_max);

                long_x = Machine.uiwidth - long_dx;
                long_y = topoffs + (i + 1) * 3 * Machine.uifontheight / 2;

                /* if too low display up */
                if (long_y + long_dy > Machine.uiheight)
                    long_y = topoffs + i * 3 * Machine.uifontheight / 2 - long_dy;

                ui_multitextbox_ex(subitems, selected, subitems[selected].Length, long_max, long_x, long_y, long_dx, long_dy, DT_COLOR_WHITE);
            }
        }
        void ui_multitext_ex(string[] array, int begin, int end, uint max, int x, int y, int color)
        {
            while (begin != end)
            {
                int line_begin = begin;
                uint len = multiline_extract(array[begin],ref begin, end, max);
                ui_text_ex(array[line_begin], line_begin +(int) len, x, y, color);
                y += 3 * Machine.uifontheight / 2;
            }
        }
        void ui_multitextbox_ex(string[] array, int begin, int end, uint max, int x, int y, int dx, int dy, int color)
        {
            ui_drawbox(x, y, dx, dy);
            x += Machine.uifontwidth / 2;
            y += Machine.uifontheight / 2;
            ui_multitext_ex(array, begin, end, max, x, y, color);
        }
        uint multiline_extract(string str, ref int pbegin, int end, uint max)
        {
            uint mac = 0;
            int begin = pbegin;
            while (begin != end && mac < max)
            {
                if (begin<str.Length && str[begin] == '\n')
                {
                    pbegin = begin + 1; /* strip final space */
                    return mac;
                }
                else if (begin == ' ')
                {
                    int word_end = begin + 1;
                    while (word_end != end && str[word_end] != ' ' && str[word_end] != '\n')
                        ++word_end;
                    if (mac + word_end - begin > max)
                    {
                        if (mac != 0)
                        {
                            pbegin = begin + 1;
                            return mac; /* strip final space */
                        }
                        else
                        {
                            pbegin = (int)(begin + max);
                            return max;
                        }
                    }
                    mac += (uint)(word_end - begin);
                    begin = word_end;
                }
                else
                {
                    ++mac;
                    ++begin;
                }
            }
            if (begin != end && (str[begin] == '\n' || str[begin] == ' '))
                ++begin;
            pbegin = begin;
            return mac;
        }
        void multiline_size(ref int dx, ref int dy, string str, int begin, int end, uint max)
        {
            uint rows = 0;
            uint cols = 0;
            while (begin != end)
            {
                uint len;
                len = multiline_extract(str, ref begin, end, max);
                if (len > cols)
                    cols = len;
                ++rows;
            }
            dx = (int)(cols * Machine.uifontwidth);
            dy = (int)((rows - 1) * 3 * Machine.uifontheight / 2 + Machine.uifontheight);
        }
        void multilinebox_size(ref int dx, ref int dy, string str, int begin, int end, uint max)
        {
            multiline_size(ref dx, ref dy, str, begin, end, max);
            dx += Machine.uifontwidth;
            dy += Machine.uifontheight;
        }
    }
}
