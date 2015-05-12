using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARTChessApplication
{
    class LogHandler
    {
        int numberStrMemorized;
        List<string> StrMemorized;

        public LogHandler()
        {
            numberStrMemorized = 20;
            StrMemorized = new List<string>();
        }

        public void AddString(string str)
        {
            if (StrMemorized.Count >= numberStrMemorized)
            {
                StrMemorized.RemoveAt(0);
            }
            StrMemorized.Add(str);
        }

        public string getFullLog() {
            string fullLog = "";
            foreach (string str in StrMemorized) {
                fullLog += str + "\n";
            }
            return fullLog;
        }
    }
}
