using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeNetwork
{
    class main
    {
        static void Main(string[] args)
        {
            Entity a = new Entity();
            a.add_property("开始","技术");
            Console.WriteLine(a.get_property(""));
        }
    }
}
