using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _002JournalEncapsulation
{
    public class JournalFactory
    {
        public static JournalInfo Greate(string path, string name)
        {
            JournalInfo objectJournal = new JournalInfo(path,name);
            return objectJournal;
        }
    }
}
