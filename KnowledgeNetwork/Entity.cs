using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeNetwork
{
    class Entity
    {
        //实体属性列表
        public Dictionary<string, string> proplist;

        public HashSet<string> islist;

        public string Name;

        public List<string> alias;

        public Entity(string name)
        {
            Name = name;
            proplist = new Dictionary<string, string>();
            islist = new HashSet<string>();
            alias = new List<string>();
        }

        public Entity()
        {
            proplist = new Dictionary<string, string>();
            islist = new HashSet<string>();
            alias = new List<string>();
        }

        public void AddProperty(string key,string value)=>proplist[key] = value;

        public string GetProperty(string key)=>proplist.ContainsKey(key) ? proplist[key] : "__failed__";

        public void AddIssomething(string key) => islist.Add(key);

        public bool IsSomething(string key) => islist.Contains(key);
    }
}
