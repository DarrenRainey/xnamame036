using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
namespace xnamame036.mame
{
    partial class Mame
    {
        delegate void update_screen_delegate();

        public static Texture2D win_video_window;
        static System.Threading.WaitHandle blitHandle = new AutoResetEvent(false);
        static Thread blitterThread;
        int osd_init()
        {
            config();

            int width = Machine.drv.screen_width;
            int height = Machine.drv.screen_height;
            if ((Machine.orientation & ROT90) != 0)
            {
                height = Machine.drv.screen_width;
                width = Machine.drv.screen_height;
            }
            else
            {
                height = Machine.drv.screen_height;
                width = Machine.drv.screen_width;
            }


            xna_init_input();

            return 0;
        }
        static void osd_create_backbuffer(int width, int height)
        {
            win_video_window = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            blitterThread = new Thread(blitscreen_dirty1_vga);
            blitterThread.Start();
        }
        void osd_exit()
        {
            blitterThread.Abort();
        }
        static int osd_allocate_colors(uint totalcolors, byte[] palette, ushort[] pens, int modifiable)
        {
            int i;

            modifiable_palette = modifiable;
            screen_colors = (int)totalcolors;
            if (scrbitmap.depth != 8)
                screen_colors += 2;
            else screen_colors = 256;

            dirtycolor = new bool[screen_colors];
            current_palette = new _BytePtr(3 * screen_colors * sizeof(byte));
            palette_16bit_lookup = new ushort[screen_colors];
            if (dirtycolor == null || current_palette == null || palette_16bit_lookup == null)
                return 1;

            for (i = 0; i < screen_colors; i++)
                dirtycolor[i] = true;
            dirtypalette = true;
            for (i = 0; i < screen_colors; i++)
                current_palette[3 * i + 0] = current_palette[3 * i + 1] = current_palette[3 * i + 2] = 0;

            if (scrbitmap.depth != 8 && modifiable == 0)
            {
                int r, g, b;

                uint p = 0;// (uint)pens.offset;
                for (i = 0; i < totalcolors; i++)
                {
                    r = (int)(255 * brightness * Math.Pow(palette[3 * i + 0] / 255.0, 1 / osd_gamma_correction) / 100);
                    g = (int)(255 * brightness * Math.Pow(palette[3 * i + 1] / 255.0, 1 / osd_gamma_correction) / 100);
                    b = (int)(255 * brightness * Math.Pow(palette[3 * i + 2] / 255.0, 1 / osd_gamma_correction) / 100);
                    pens[(int)p++] = makecol((byte)r, (byte)g, (byte)b);
                }

                Machine.uifont.colortable[0] = makecol(0x00, 0x00, 0x00);
                Machine.uifont.colortable[1] = makecol(0xff, 0xff, 0xff);
                Machine.uifont.colortable[2] = makecol(0xff, 0xff, 0xff);
                Machine.uifont.colortable[3] = makecol(0x00, 0x00, 0x00);
            }
            else
            {
                if (scrbitmap.depth == 8 && totalcolors >= 255)
                {
                    int bestblack, bestwhite;
                    int bestblackscore, bestwhitescore;


                    bestblack = bestwhite = 0;
                    bestblackscore = 3 * 255 * 255;
                    bestwhitescore = 0;
                    for (i = 0; i < totalcolors; i++)
                    {
                        int r = palette[3 * i + 0];
                        int g = palette[3 * i + 1];
                        int b = palette[3 * i + 2];
                        int score = r * r + g * g + b * b;

                        if (score < bestblackscore)
                        {
                            bestblack = i;
                            bestblackscore = score;
                        }
                        if (score > bestwhitescore)
                        {
                            bestwhite = i;
                            bestwhitescore = score;
                        }
                    }

                    for (i = 0; i < totalcolors; i++)
                        pens[i] = (ushort)i;

                    /* map black to pen 0, otherwise the screen border will not be black */
                    pens[bestblack] = 0;
                    pens[0] = (ushort)bestblack;

                    Machine.uifont.colortable[0] = pens[bestblack];
                    Machine.uifont.colortable[1] = pens[bestwhite];
                    Machine.uifont.colortable[2] = pens[bestwhite];
                    Machine.uifont.colortable[3] = pens[bestblack];
                }
                else
                {
                    /* reserve color 1 for the user interface text */
                    current_palette[3 * 1 + 0] = current_palette[3 * 1 + 1] = current_palette[3 * 1 + 2] = 0xff;
                    Machine.uifont.colortable[0] = 0;
                    Machine.uifont.colortable[1] = 1;
                    Machine.uifont.colortable[2] = 1;
                    Machine.uifont.colortable[3] = 0;

                    /* fill the palette starting from the end, so we mess up badly written */
                    /* drivers which don't go through Machine.pens[] */
                    for (i = 0; i < totalcolors; i++)
                        pens[i] = (ushort)((screen_colors - 1) - i);
                }

                for (i = 0; i < totalcolors; i++)
                {
                    current_palette[3 * pens[i] + 0] = palette[3 * i];
                    current_palette[3 * pens[i] + 1] = palette[3 * i + 1];
                    current_palette[3 * pens[i] + 2] = palette[3 * i + 2];
                }
            }

            return 0;
        }
    }
}
