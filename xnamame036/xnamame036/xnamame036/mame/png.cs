using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        class png_info
        {
            public uint width, height;
            public uint xoffset, yoffset;
            public uint xres, yres;
            public double xscale, yscale;
            public double source_gamma;
            public uint[] chromaticities = new uint[8];
            public uint resolution_unit, offset_unit, scale_unit;
            public byte bit_depth;
            public uint[] significant_bits = new uint[4];
            public uint[] background_color = new uint[4];
            public byte color_type;
            public byte compression_method;
            public byte filter_method;
            public byte interlace_method;
            public uint num_palette;
            public byte[] palette;
            public uint num_trans;
            public byte[] trans;
            public byte[] image;

            /* The rest is private and should not be used
             * by the public functions
             */
            public byte bpp;
            public uint rowbytes;
            public byte[] zimage;
            public uint zlength;
            public byte[] fimage;
        };

        static int png_read_file(object fp, png_info p)
        {
            throw new Exception();
        }
        static void png_delete_unused_colors(png_info p)
        {
            throw new Exception();
        }
        static int png_expand_buffer_8bit(png_info p)
        {
            throw new Exception();
        }
    }
}
