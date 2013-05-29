using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{

    //PPUcounter emulates the H/V latch counters of the S-PPU2.
    //
    //real hardware has the S-CPU maintain its own copy of these counters that are
    //updated based on the state of the S-PPU Vblank and Hblank pins. emulating this
    //would require full lock-step synchronization for every clock tick.
    //to bypass this and allow the two to run out-of-order, both the CPU and PPU
    //classes inherit PPUcounter and keep their own counters.
    //the timers are kept in sync, as the only differences occur on V=240 and V=261,
    //based on interlace. thus, we need only synchronize and fetch interlace at any
    //point before this in the frame, which is handled internally by this class at
    //V=128.


    partial class SNES
    {
        class PPUcounter
        {
            struct Status
            {
                public bool interlace, field;
                public ushort vcounter, hcounter;
            }
            Status status;
            class History
            {
                public bool[] field = new bool[2048];
                public ushort[] vcounter = new ushort[2048], hcounter = new ushort[2048];
                public int index;
            }
            History history = new History();
            public void tick()
            {
                throw new Exception();
            }
            public ushort vcounter()
            {
                throw new Exception();
            }
            public ushort hcounter()
            {
                throw new Exception();
            }
            public ushort vcounter(uint offset)
            {
                throw new Exception();
            }
            public ushort hcounter(uint offset)
            {
                throw new Exception();
            }
            public ushort lineclocks()
            {
                if (system.region == (uint)System.Region.NTSC && status.interlace == false && vcounter() == 240 && field() == true) return 1360;
                return 1364;
            }
            public bool field()
            {
                throw new Exception();
            }
            public bool field(uint offset)
            {
                throw new Exception();
            }
            public void reset()
            {
                status.interlace = false;
                status.field = false;
                status.vcounter = 0;
                status.hcounter = 0;
                history.index = 0;

                for (uint i = 0; i < 2048; i++)
                {
                    history.field[i] = false;
                    history.vcounter[i] = 0;
                    history.hcounter[i] = 0;
                }
            }
            public void serialize(Serializer s)
            {
                s.integer(status.interlace);
                s.integer(status.field);
                s.integer(status.vcounter);
                s.integer(status.hcounter);

                s.array(history.field);
                s.array(history.vcounter);
                s.array(history.hcounter);
                s.integer(history.index);
            }
        }
        abstract class PPU : PPUcounter
        {
            public abstract void enter();
            ushort[] output;

            struct Status
            {
                public bool render_output;
                public bool frame_executed;
                public bool frames_updated;
                public uint frames_rendered;
                public uint frames_executed;
            }

            Status status;
            byte ppu1_version, ppu2_version;

            public abstract bool interlace();
            public abstract bool overscan();
            public abstract bool hires();
            public abstract void latch_counters();
            public virtual void frame()
            {
                throw new Exception();
            }
            public virtual void power()
            {
                ppu1_version = (byte)config.ppu1.version;
                ppu2_version = (byte)config.ppu2.version;
            }
            public virtual void reset()
            {
                base.reset();
            }
            public virtual void enable_renderer(bool r)
            {
                status.render_output = r;
            }
            public virtual bool renderer_enabled()
            {
                return status.render_output;
            }
            public PPU()
            {
                output = new ushort[512 * 480];

                status.render_output = true;
                status.frames_updated = false;
                status.frames_rendered = 0;
                status.frames_executed = 0;
            }
        }
    }
}
