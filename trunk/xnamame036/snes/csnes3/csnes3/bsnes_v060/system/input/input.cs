using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        static Input input = new Input();
        class Input
        {
            public
              enum Device
            {
                DeviceNone,
                DeviceJoypad,
                DeviceMultitap,
                DeviceMouse,
                DeviceSuperScope,
                DeviceJustifier,
                DeviceJustifiers,
            };

            public enum JoypadID
            {
                JoypadB = 0, JoypadY = 1,
                JoypadSelect = 2, JoypadStart = 3,
                JoypadUp = 4, JoypadDown = 5,
                JoypadLeft = 6, JoypadRight = 7,
                JoypadA = 8, JoypadX = 9,
                JoypadL = 10, JoypadR = 11,
            };

            public enum MouseID
            {
                MouseX = 0, MouseY = 1,
                MouseLeft = 2, MouseRight = 3,
            };

            public enum SuperScopeID
            {
                SuperScopeX = 0, SuperScopeY = 1,
                SuperScopeTrigger = 2, SuperScopeCursor = 3,
                SuperScopeTurbo = 4, SuperScopePause = 5,
            };

            public enum JustifierID
            {
                JustifierX = 0, JustifierY = 1,
                JustifierTrigger = 2, JustifierStart = 3,
            };

            public byte port_read(bool port)
            {
                throw new Exception();
            }
            public void port_set_device(bool port, uint device)
                            {
                throw new Exception();
            }

            public void init()
            {
                throw new Exception();
            }

            public void poll()
            {
                throw new Exception();
            }

            public void update()
            {
                throw new Exception();
            }


            //light guns (Super Scope, Justifier(s)) strobe IOBit whenever the CRT
            //beam cannon is detected. this needs to be tested at the cycle level
            //(hence inlining here for speed) to avoid 'dead space' during DRAM refresh.
            //iobit is updated during port_set_device(),
            //latchx, latchy are updated during update() (once per frame)
            public void tick()
            {
                //only test if Super Scope or Justifier is connected
                if (iobit && cpu.vcounter() == latchy && cpu.hcounter() == latchx)
                {
                    ppu.latch_counters();
                }
            }

            bool iobit;
            short latchx, latchy;

            struct port_t
            {
                uint device;
                uint counter0;  //read counters
                uint counter1;

                struct superscope_t
                {
                    int x, y;

                    bool trigger;
                    bool cursor;
                    bool turbo;
                    bool pause;
                    bool offscreen;

                    bool turbolock;
                    bool triggerlock;
                    bool pauselock;
                } superscope_t superscope;

                struct justifier_t
                {
                    bool active;

                    int x1, x2;
                    int y1, y2;

                    bool trigger1, trigger2;
                    bool start1, start2;
                } justifier_t justifier;
            } port_t[] port = new port_t[2];

        };
    }
}
