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


            string str = "����ѧУ����"; int id = -1;
            StringBuilder tmp = new StringBuilder();
            if ((id = str.IndexOf("����ѧУ")) != -1)
            {

                tmp.Append(str.Substring(0, id));
                tmp.Append("��������ѧ");
                tmp.Append(str.Substring(id + 4));
            }
            Console.WriteLine(tmp.ToString());
            /*string str = "��������ѧУ���ǲ��ǹ�������";

            var posSeg = new PosSegmenter();
            var s = "��������ѧУ���ǲ��ǹ�������";

            var tokens = posSeg.Cut(s);
            Console.WriteLine(string.Join("", tokens.Select(token => string.Format("{0}  {1}\n", token.Word, token.Flag))));


            JiebaSegmenter a = new JiebaSegmenter();
            a.AddWord("��");
            var extractor = new TfidfExtractor(a);
            // ��ȡǰʮ�����������ʺͶ��ʵĹؼ���
            var keywords = extractor.ExtractTags(str, 10, Constants.NounAndVerbPos);
            Console.WriteLine(string.Join(" ", keywords));*/
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
                  { "",    "g.V('��������ѧ')" },
            };
            foreach (KeyValuePair<string, string> i in gremlinQueries)
            {
                Console.WriteLine($">>Running {i.Key} : {i.Value}");
                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, i.Value);
                while (query.HasMoreResults)
                {
                    foreach (dynamic result in await query.ExecuteNextAsync())
                    {
                        Console.WriteLine(result.id);
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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 