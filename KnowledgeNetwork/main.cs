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
            Tuple<string, string> a = new Tuple<string, string>("1","2");
            Tuple<string, string> b = new Tuple<string, string>("1", "25445");

            Dictionary<Tuple<string, string>, int> z = new Dictionary<Tuple<string, string>, int>();
            z[a] = 12312;
            z[b] = 456;
            Console.WriteLine(z[a]);
            Console.WriteLine(z[b]);

            StreamReader sr = new StreamReader("1.txt", Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] split = line.Split(' ');
                foreach (var i in split)
                {
                    Console.Write(i+" ");
                }
                Console.WriteLine();
            }
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
{"1","g.V('��Դ�붯��ѧԺ').property('��Ժ��','�����š��������º��֡���������ѩ�����¼�ģ')"},
{"2","g.V('��������ѧ').property('У��','3��')"},


            };
            
            foreach (KeyValuePair<string, string> i in gremlinQueries)
            {
                Console.WriteLine($">>Running {i.Key} : {i.Value}");
                try
                {
                    IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, i.Value);
                    while (query.HasMoreResults)
                    {
                        Console.WriteLine("more result");
                        foreach (dynamic result in await query.ExecuteNextAsync())
                        {
                            Console.WriteLine($"\t>>>>>>>>>>>>>>>>>>>>>>>>>> {JsonConvert.SerializeObject(result)}\n");
                            try
                            {
                                if (result.properties["85464"] != null)
                                {

                                    Console.WriteLine(result.properties["���"][0].value);

                                }
                            }
                            catch
                            {
                                continue;
                            }

                        }
                    }
                }
                catch
                {
                    continue;
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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 