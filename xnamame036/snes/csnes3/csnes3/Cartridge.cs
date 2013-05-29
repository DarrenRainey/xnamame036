using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace csnes3.bsnes_v060.SNES
{
    partial class SNES
    {
        public static CartridgeSlot cartridgeSlot = new CartridgeSlot();
        public class CartridgeSlot
        {
            public string name, fileName, baseName, slotAName, slotBName;
            public bool patchApplied;

            public struct _Information
            {
                public string name;
                public string region;
                public uint romSize, ramSize;
            }
            public _Information Information;
            void loadCheats()
            {
            }
            public bool loadNormal(string _base)
            {
                unload();
                if (loadCartridge(baseName = _base, SNES.memory.cartrom) == false) return false;
                SNES.msu._base(Path.GetFileNameWithoutExtension(baseName));
                SNES.cartridge.load(SNES.Cartridge.Mode.ModeNormal);

                loadMemory(baseName, ".srm", SNES.memory.cartram);
                loadMemory(baseName, ".rtc", SNES.memory.cartrtc);

                fileName = baseName;
                name = (Path.GetFileNameWithoutExtension(baseName));

                //utility.modifySystemState(Utility.LoadCartridge);

                cartridgeSlot.loadCheats();
                SNES.system.power();


                return true;
            }
            bool loadMemory(string filname, string extenstion, MappedRAM memory)
            {
                throw new Exception();
            }
            bool loadCartridge(string filename, MappedRAM memory)
            {
                byte[] data=null;
                uint size=0;
                audio.clear();
                if (reader.direct_load(filename, ref data, ref size) == false) return false;
                memory.copy(data, size);
                return true;
            }

            bool saveMemory(string filename, string extension, MappedRAM memory)
            {
                if (memory.size() == 0 || memory.size() == unchecked((uint)-1U)) return false;
                throw new Exception();
                //string name=filename+ SNES.config.path.save+extension;

                //file fp;
                //if(fp.open(name, file.mode_write) == false) return false;

                //fp.write(memory.data(), memory.size());
                //fp.close();
                //return true;
            }
            void saveMemory()
            {
                if (SNES.cartridge.loaded == false) return;

                switch (SNES.cartridge.mode)
                {
                    case SNES.Cartridge.Mode.ModeNormal:
                    case SNES.Cartridge.Mode.ModeBsxSlotted:
                        {
                            saveMemory(baseName, ".srm", SNES.memory.cartram);
                            saveMemory(baseName, ".rtc", SNES.memory.cartrtc);
                        } break;

                    case SNES.Cartridge.Mode.ModeBsx:
                        {
                            saveMemory(baseName, ".srm", SNES.memory.bsxram);
                            saveMemory(baseName, ".psr", SNES.memory.bsxpram);
                        } break;

                    case SNES.Cartridge.Mode.ModeSufamiTurbo:
                        {
                            saveMemory(slotAName, ".srm", SNES.memory.stAram);
                            saveMemory(slotBName, ".srm", SNES.memory.stBram);
                        } break;

                    case SNES.Cartridge.Mode.ModeSuperGameBoy:
                        {
                            saveMemory(slotAName, ".sav", SNES.memory.gbram);
                            saveMemory(slotAName, ".rtc", SNES.memory.gbrtc);
                        } break;
                }
            }

            void unload()
            {
                if (SNES.cartridge.loaded == false) return;
                //utility.modifySystemState(Utility.UnloadCartridge);
                SNES.system.unload();
                cartridgeSlot.saveMemory();
                SNES.cartridge.unload();
            }
        }
    }
}