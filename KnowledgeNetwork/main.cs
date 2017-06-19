using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KnowledgeNetwork
{
    class main
    {
        static void Main(string[] args)
        {
            Entity a = new Entity();
            a.AddProperty("sdf", "4564564");
            a.AddProperty("2342", "ggfhfg");
            Console.WriteLine(a.GetProperty("2342"));
            Console.WriteLine(JsonConvert.SerializeObject(a));
            Console.WriteLine(JsonConvert.SerializeObject(a));

            string str = JsonConvert.SerializeObject(a);
            Entity b = JsonConvert.DeserializeObject<Entity>(str);
            Console.WriteLine(b.GetProperty("sdf"));
        }

        void save(List<Entity> list)
        {
            foreach(var i in list)
            {

            }
        }
    }
}
