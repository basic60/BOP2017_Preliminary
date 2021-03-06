﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Bopapp.Dialogs
{
    public static class Cache
    {
        public static Dictionary<string, string> data;
        public static void LoadCache(string filename)
        {
            if (data != null)
                return;
            data = new Dictionary<string, string>();
            StreamReader sr = new StreamReader(filename, Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(' ');
                foreach(var i in split)
                {
                    Console.WriteLine(i);
                }
                data[split[0]+split[1]] = split[2];
            }
        }

        public static string query(string t1, string t2)
        {
            string tmp = t1 + t2;
            if (data.ContainsKey(tmp))
                return data[tmp];
            return null;
        }
        public static void clear() { data = new Dictionary<string, string>(); }
    }
}