using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {
        string cheatfile = "CHEAT.DAT";
        //char database[CHEAT_FILENAME_MAXLEN+1];

        string helpfile = "CHEAT.HLP";

        int fastsearch = 2;
        int sologame = 0;

        static int he_did_cheat;

        public struct TextLine
        {
            public int number;
            public string data;
        }
        const int MAX_TEXT_LINE = 1000;

        public TextLine[] HelpLine = new TextLine[MAX_TEXT_LINE];

        ExtMemory[] StartRam = new ExtMemory[MAX_EXT_MEMORY];
        ExtMemory[] BackupRam = new ExtMemory[MAX_EXT_MEMORY];
        ExtMemory[] FlagTable = new ExtMemory[MAX_EXT_MEMORY];
        ExtMemory[] OldBackupRam = new ExtMemory[MAX_EXT_MEMORY];
        ExtMemory[] OldFlagTable = new ExtMemory[MAX_EXT_MEMORY];

        void StopCheat()
        {
            reset_table(ref StartRam);
            reset_table(ref BackupRam);
            reset_table(ref FlagTable);

            reset_table(ref OldBackupRam);
            reset_table(ref OldFlagTable);

            reset_texttable( HelpLine);
        }
        static void reset_table(ref ExtMemory[] table)
        {
            ExtMemory[] ext = table;

            for (int i = 0; i < table.Length; i++)
                table[i].data = null;
            table = null;
        }
        static void reset_texttable(TextLine[] table)
        {
            throw new Exception();
        }
        void InitCheat()
        {
            throw new Exception();
        }
        void DoCheat()
        {
            throw new Exception();
        }
        int cheat_menu()
        {
            throw new Exception();
        }
    }
}
