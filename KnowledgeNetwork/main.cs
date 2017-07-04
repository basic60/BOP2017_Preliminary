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
using JiebaNet;

namespace KnowledgeNetwork
{
    using JiebaNet.Analyser;
    using JiebaNet.Segmenter;
    using JiebaNet.Segmenter.PosSeg;
    using Newtonsoft.Json;
    class main
    {
        static void Main(string[] args)
        {
            List<string> a = new List<string>();
            a.Add("123");a.Add("123");
            Console.WriteLine(a.Count());
            a.Remove("123");
            Console.WriteLine(a.Count());


            var posSeg = new PosSegmenter();
           
            var s = "我的办公地点在哪";

            var tokens = posSeg.Cut(s);
            Console.WriteLine(string.Join("", tokens.Select(token => string.Format("{0}  {1}\n", token.Word, token.Flag))));
           Execute();
    }

        static void Execute()
        {
            string endpoint = ConfigurationManager.AppSettings["Endpoint"];
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            using (DocumentClient client = new DocumentClient(
                new Uri(endpoint),
                authKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
            {
                    main p = new main();
                    p.Query(client).Wait();

            }
        }

        public async Task Query(DocumentClient client)
        {
            DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("graphdb_dut"),
                new DocumentCollection { Id = "persons" },
                new RequestOptions { OfferThroughput = 1000 });

            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
            {
{"1","g.addV('学院').property('id','大连理工大学白俄罗斯国立大学联合学院')"},
{"2","g.V('大连理工大学白俄罗斯国立大学联合学院').property('创建','2017年4月10日')"},
{"3","g.V('大连理工大学白俄罗斯国立大学联合学院').property('电话','0411-84706615')"},
{"4","g.V('大连理工大学白俄罗斯国立大学联合学院').property('邮箱','dbji@dlut.edu.cn')"},
{"5","g.addV('学院').property('id','国际教育学院')"},
{"6","g.V('国际教育学院').property('创建','2013年3月8日')"},
            };
            
            foreach (KeyValuePair<string, string> i in gremlinQueries)
            {
                Console.WriteLine($">>Running {i.Key} : {i.Value}");
                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, i.Value);
                while (query.HasMoreResults)
                {
                    Console.WriteLine("more result");
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine($"\t>>>>>>>>>>>>>>>>>>>>>>>>>> {JsonConvert.SerializeObject(result)}\n");
                    }
                }
                Console.WriteLine("========================================================");
                Console.WriteLine();
            }

            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        public async Task Sample(DocumentClient client)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb_dut" });

            DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("graphdb_dut"),
                new DocumentCollection { Id = "persons" },
                new RequestOptions { OfferThroughput = 1000 });
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
            {

            };

            foreach(KeyValuePair<string,string> i in gremlinQueries)
            {
                Console.WriteLine($">>Running {i.Key} : {i.Value}");
                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, i.Value);
                while (query.HasMoreResults)
                {
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
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 