using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        partial class Cartridge
        {
            public void serialize(Serializer s)
            {
                if (memory.cartram.size() != 0 && memory.cartram.size() != unchecked((uint)~0))
                {
                    s.array(memory.cartram.data(), memory.cartram.size());
                }

                if (memory.cartrtc.size() != 0 && memory.cartrtc.size() != unchecked((uint)~0))
                {
                    s.array(memory.cartrtc.data(), memory.cartrtc.size());
                }

                if (memory.bsxram.size() != 0 && memory.bsxram.size() != unchecked((uint)~0))
                {
                    s.array(memory.bsxram.data(), memory.bsxram.size());
                }

                if (memory.bsxpram.size() != 0 && memory.bsxpram.size() != unchecked((uint)~0))
                {
                    s.array(memory.bsxpram.data(), memory.bsxpram.size());
                }

                if (memory.stAram.size() != 0 && memory.stAram.size() != unchecked((uint)~0))
                {
                    s.array(memory.stAram.data(), memory.stAram.size());
                }

                if (memory.stBram.size() != 0 && memory.stBram.size() != unchecked((uint)~0))
                {
                    s.array(memory.stBram.data(), memory.stBram.size());
                }

                if (memory.gbram.size() != 0 && memory.gbram.size() != unchecked((uint)~0))
                {
                    s.array(memory.gbram.data(), memory.gbram.size());
                }

                if (memory.gbrtc.size() != 0 && memory.gbrtc.size() != unchecked((uint)~0))
                {
                    s.array(memory.gbrtc.data(), memory.gbrtc.size());
                }
            }
        }
    }
}
