using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static bPPU ppu = new bPPU();
        class bPPU : PPU
        {
            byte region;
            uint line;




            public override void enter()
            {
                throw new NotImplementedException();
            }
            public override void enable_renderer(bool r)
            {
                base.enable_renderer(r);
            }
            public override bool renderer_enabled()
            {
                return base.renderer_enabled();
            }
            public override void frame()
            {
                base.frame();
            }
            public override bool hires()
            {
                throw new NotImplementedException();
            }
            public override bool interlace()
            {
                throw new NotImplementedException();
            }
            public override void latch_counters()
            {
                throw new NotImplementedException();
            }
            public override bool overscan()
            {
                throw new NotImplementedException();
            }
            public override void power()
            {
                base.power();
            }
            public override void reset()
            {
                base.reset();
            }
        }
    }
}
