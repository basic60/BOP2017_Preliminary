using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeNetwork
{
    public class HumanInputReader
    {
        private readonly StreamReader sc;

        public HumanInputReader(string fname)
        {
            sc = new StreamReader(fname,Encoding.Default);
        }

        Queue<string> str = new Queue<string>();
        void readAndSplit()
        {
            while (str.Count() == 0)
            {
                string s; string[] tmp;
                s = sc.ReadLine();
                if (s == null) return;
                tmp = s.Split(' ');
                foreach (var i in tmp)
                {
                    if (i != "")
                        str.Enqueue(i);
                }
            }
        }
        public int readNextInt32()
        {
            int ans = 0;
            if (str.Count() == 0)
                readAndSplit();
            ans = Convert.ToInt32(str.Dequeue());
            return ans;
        }
        public long readNextInt64()
        {
            long ans = 0;
            if (str.Count() == 0)
                readAndSplit();
            ans = Convert.ToInt64(str.Dequeue());
            return ans;
        }
        public string readNextStr()
        {
            if (str.Count() == 0)
                readAndSplit();
            if (str.Count() == 0)
                return "";
            return str.Dequeue();
        }
        public double readNextDouble()
        {
            if (str.Count() == 0)
                readAndSplit();
            return Convert.ToDouble(str.Dequeue());
        }
    }
}
