﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace xnamame036.mame
{
    partial class Mame
    {
        static ushort[] palette_16bit_lookup;

        static uint[] palette = new uint[256];
        static uint[] blit_buffer;
        static byte[] back_buffer;
        void set_color(int index, RGB entry)
        {
            palette[index] = new Color(entry.r, entry.g, entry.b).PackedValue;
        }
        void set_color(int index, byte r, byte g, byte b)
        {
            palette[index] = new Color(r, g, b).PackedValue;
        }
        static void blitscreen_dirty1_vga()
        {
            int w, h;
            
            while (true)
            {
                blitHandle.WaitOne();
                Buffer.BlockCopy(scrbitmap.line[0].buffer, 0, back_buffer, 0, scrbitmap.line[0].buffer.Length);
                {
                    int sbi = scrbitmap.line[skiplines].offset + skipcolumns;
                    sbi = 0;
                    //w = drv.screen_width;// gfx_display_columns;
                    //h = drv.screen_height;// gfx_display_lines;
                    //w =  gfx_display_columns;
                    //h =  gfx_display_lines;

                    w = scrbitmap.width;
                    h = scrbitmap.height;
                    for (int y = 0; y < h; y++)
                    {

                        for (int x = 0; x < w; x++)
                        {
                            //blit_buffer[x + (y * w)] = palette[scrbitmap.line[skiplines].buffer[sbi + x + (y * w)]];
                            blit_buffer[x + (y * w)] = palette[back_buffer[sbi + x + (y * w)]];
                        }
                    }
                }
                lock (win_video_window)
                {
                    Game1.graphics.GraphicsDevice.Textures[0] = null;
                    win_video_window.SetData<uint>(blit_buffer);
                }
            }
        }

    }
}
