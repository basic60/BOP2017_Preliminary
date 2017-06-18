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
        Dictionary<string, string> proplist;

        public Entity()=> proplist=new Dictionary<string, string>();

        public void add_property(string key,string value)=>proplist[key] = value;

        public string get_property(string key)=>proplist.ContainsKey(key) ? proplist[key] : "__failed__";
    }
}
