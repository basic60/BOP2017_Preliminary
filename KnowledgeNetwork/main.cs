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
            var posSeg = new PosSegmenter();
           
            var s ="��������ѧλ�����";

            var tokens = posSeg.Cut(s);
            Console.WriteLine(string.Join("", tokens.Select(token => string.Format("{0}  {1}\n", token.Word, token.Flag))));


            Execute();
        //Console.WriteLine("abc".IndexOf("490560645"));
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
                { "asd","g.V('������')"},
              /*  { "��ӵ�ַ1",    "g.addV('�ص�').property('id', '�й�������ʡ�����иʾ������蹤·2��')" },
                { "��ӵ�ַ2",    "g.addV('�ʱ�').property('id', '116024')" },
                { "��ӵ�ַ3",    "g.addV('�绰').property('id', '0411-84708320')" },
                { "��ӵ�ַ4",    "g.addV('����').property('id', 'office@dlut.edu.cn')" },
                { "��ӵ�ַ5",    "g.addV('��ַ').property('id', 'www.dlut.edu.cn')" },

                { "add edge1",   "g.V('��������ѧ').addE('�ص�').to(g.V('�й�������ʡ�����иʾ������蹤·2��'))" },
                { "add edge2",   "g.V('��������ѧ').addE('�ʱ�').to(g.V('116024'))" },
                { "add edge3",   "g.V('��������ѧ').addE('�绰').to(g.V('0411-84708320'))" },
                { "add edge4",   "g.V('��������ѧ').addE('����').to(g.V('office@dlut.edu.cn'))" },
                { "add edge5",   "g.V('��������ѧ').addE('��ַ').to(g.V('www.dlut.edu.cn'))" },*/
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

                        // Console.WriteLine(result.properties.ToString());
                        if(result.properties["12312312"]!=null)
                            Console.WriteLine(result.properties["234234"][0].value);
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
                { "drop vertex",      "g.V().drop()" },
                { "��ӹ�����",    "g.addV('��').property('id', '������')" },
                { "���zzz",    "g.addV('��').property('id', 'zzz').property('Ժʿ', false)" },
                { "��Ӵ�",    "g.addV('ѧУ').property('id', '��������ѧ').property('���', '��').property('��Уʱ��', '1949��4��').property('211����', true).property('985����', true)"},
                { "add edge",      "g.V('��������ѧ').addE('У��').to(g.V('������'))" },
                { "update property", "g.V('������').property('Ժʿ',true)"},
                { "output",   "g.V().hasLabel('��').values('Ժʿ')" }, //��keyΪԺʿ������value���
                { "Traverse",       "g.V('��������ѧ').outE('У��').inV().hasLabel('��')" },
                { "CountVertices",  "g.V().count()" },
                { "CountEdges",     "g.E().count()" },
                { "DropVertex",     "g.V('zzz').drop()" },
                { "Loop",           "g.V('��������ѧ').repeat(out()).until(has('id', '������')).path()" },
                //{ "Traverse 2x",    "g.V('thomas').outE('knows').inV().hasLabel('person').outE('knows').inV().hasLabel('person')" },
                //{ "DropEdge",       "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()" },
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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 