using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using System.IO;

namespace KnowledgeNetwork
{
    using Newtonsoft.Json;
    class main
    {
        static void Main(string[] args)
        {
            string endpoint= ConfigurationManager.AppSettings["Endpoint"];
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            using (DocumentClient client = new DocumentClient(
                new Uri(endpoint),
                authKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
            {
                    main p = new main();
                    p.Runasync(client).Wait();
            }

            //List<Entity> ent=ReadFromHumanInput(@"D:\zzh\tmp\data.txt");
            //SaveJson(ent, "fin.json");
        }

        public async Task Runasync(DocumentClient client)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb_dut" });

            DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("graphdb_dut"),
                new DocumentCollection { Id = "persons" },
                new RequestOptions { OfferThroughput = 1000 });
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
            {
                { "drop vertex",      "g.V().drop()" },
                { "添加郭东明",    "g.addV('人').property('id', '郭东明')" },
                { "添加zzz",    "g.addV('人').property('id', 'zzz').property('院士', false)" },
                { "添加大工",    "g.addV('学校').property('id', '大连理工大学').property('简称', '大工').property('建校时间', '1949年4月').property('211工程', true).property('985工程', true)"},
                { "add edge",      "g.V('大连理工大学').addE('校长').to(g.V('郭东明'))" },
                { "update property", "g.V('郭东明').property('院士',true)"},
                { "Filter Range",   "g.V().hasLabel('人').values('院士')" }, //将key为院士的所有value输出
                { "Traverse",       "g.V('大连理工大学').outE('校长').inV().hasLabel('人')" },
                { "CountVertices",  "g.V().count()" },
                { "CountEdges",     "g.E().count()" },
                { "DropVertex",     "g.V('zzz').drop()" },
                { "Loop",           "g.V('大连理工大学').repeat(out()).until(has('id', '郭东明')).path()" },
                //{ "Traverse 2x",    "g.V('thomas').outE('knows').inV().hasLabel('person').outE('knows').inV().hasLabel('person')" },
                //{ "DropEdge",       "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()" },
            };

            foreach(KeyValuePair<string,string> i in gremlinQueries)
            {
                Console.WriteLine($">>Running {i.Key} : {i.Value}");
                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, i.Value);
                while (query.HasMoreResults)
                {
                    Console.WriteLine("ent");
                    foreach(dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine($"\t {JsonConvert.SerializeObject(result)}\n");
                    }
                }
                Console.WriteLine("========================================================");
                Console.WriteLine();
            }
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
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

        static List<Entity> LoadJson(string fname)
        {
            List<Entity> res = new List<Entity>();

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
