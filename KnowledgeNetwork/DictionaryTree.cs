using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace KnowledgeNetwork
{
    public class DictionaryTree
    {
        static Node root=new Node();

        public static void LoadDic(string filename)
        {
            root = new Node();
            StreamReader sr = new StreamReader(filename,Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] words = line.Split(' ');Node p = root;

                char[] arr = words[0].ToCharArray();
                for (int i = 0; i != arr.Length; i++)
                {
                    bool flag = false;
                    foreach (var j in p.next)
                    {
                        if (j.v == arr[i])
                        {
                            flag = true;
                            p = j;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Node tmp = new Node(arr[i]);
                        p.next.Add(tmp);
                        p = tmp;
                    }
                }
                p.data = words[1];
            }
        }

        public static string GetSynonyms(string str)
        {
            Node p=root;
            char[] arr = str.ToCharArray();
            for(int i=0;i!=arr.Length;i++)
            {
                bool suc = false;
                foreach(var j in p.next)
                {
                    if (j.v == arr[i])
                    {
                        suc = true;
                        p = j;
                        break;
                    }
                }
                if (suc == false)
                    return null;
            }
            return p.data;
        }

    }

    class Node
    {
        public List<Node> next;
        public char v;
        public string data;
        public Node() { next = new List<Node>(); }
        public Node(char val) { v = val; next = new List<Node>(); }
        public Node(string str) { data = str; next = new List<Node>(); }
    }
}