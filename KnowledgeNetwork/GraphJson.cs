using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeNetwork
{
    public class GraphJson
    {
        public string id;
        public string type;
        public string label;
        public List<Edge> outE;
        public Dictionary<dynamic, dynamic> query;
    }

    public class Edge
    {

    }
}
