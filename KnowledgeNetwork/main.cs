using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace KnowledgeNetwork
{
    class main
    {
        static void Main(string[] args)
        {
            List<Entity> ent=ReadFromHumanInput(@"D:\zzh\tmp\data.txt");
            SaveJson(ent, "fin.json");
        }

        static List<Entity> ReadFromHumanInput(string fname)
        {
            List<Entity> res=new List<Entity>();
            HumanInputReader hreader = new HumanInputReader(fname);
            string str;
            while ((str=hreader.readNextStr()) != "")
            {
                Console.WriteLine(str);
                Entity tmp = new Entity(str);
                str = hreader.readNextStr();
                while (str == "2" || str == "1")
                {
                    if (str == "1")
                    {
                        string t1, t2;
                        t1 = hreader.readNextStr();t2 = hreader.readNextStr();
                        tmp.AddProperty(t1, t2);
                    }
                    else if (str == "2")
                    {
                        tmp.AddIssomething(hreader.readNextStr());
                    }
                    str = hreader.readNextStr();
                }
                res.Add(tmp);
            }
            return res;
        }

        static void SaveJson(List<Entity> list,string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach(var i in list)
            {
                sw.WriteLine(JsonConvert.SerializeObject(i));
            }
            sw.Flush();sw.Close();
            Console.WriteLine("end!!");
        }
    }
}
