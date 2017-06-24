using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeNetwork
{
    public class Node
    {
        public string Name;
        public string[] alias;
        public Dictionary<string, string> edge;
        public bool HasEdge(string name) => edge.ContainsKey(name);
    }
}
