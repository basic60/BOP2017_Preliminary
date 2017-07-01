using JiebaNet;
using JiebaNet.Analyser;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text;
using Microsoft.Azure.Graphs.Elements;
using System.IO;


namespace Bopapp.Dialogs
{
    [Serializable]
    public class FinDialog : IDialog<object>
    {

        string endpoint = ConfigurationManager.AppSettings["Endpoint"];
        string authKey = ConfigurationManager.AppSettings["AuthKey"];


        public readonly List<string> unuseful_word =new List<string>{"是"};

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(AnswerAsync);
        }

        public virtual async Task AnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> para)
        {
            var message = await para;
            string question = message.Text;
 
            var posSeg = new PosSegmenter();
            
            var tokens = posSeg.Cut(question);
            List<string> words = new List<string>();
            List<string> chara = new List<string>();
            foreach (var i in tokens) words.Add(i.Word);
            foreach (var i in tokens) chara.Add(i.Flag);
            for(int i = 0; i != words.Count; i++)
            {
                string tmp=null;
                if ((tmp=DictionaryTree.GetSynonyms(words[i])) != null)
                {
                    words[i] = tmp;
                }
            }

            List<string> keyword = new List<string>();
            for(int i = 0; i != words.Count(); i++)
            {
                if ( (chara[i] == "n" || chara[i] == "v" || chara[i] == "nt") && (!unuseful_word.Contains(words[i]))  )
                {
                    await context.PostAsync("keyword: "+words[i]);
                    keyword.Add(words[i]);
                }
                if (chara[i] == "nr" && i - 1 >= 0 && chara[i - 1] == "nr")
                {
                    await context.PostAsync(words[i - 1] + words[i]);
                    keyword.Add(words[i - 1] + words[i]);
                }
            }

            string ans = await TryAll(keyword, question);
            await context.PostAsync(ans);
            context.Wait(AnswerAsync);
        }

        public async Task<string> TryAll(List<string> data,string question)
        {
            if (data.Count == 1 && data[0] != "大连理工大学")
            {
                data.Add("大连理工大学");
            }
            else if(data.Count == 1)
            {
                return "请问您有什么问题？";
            }

            if (data.Count == 2)
            {
                
                string ret = await query2(data[0], data[1], question);
                if (ret == "True") return "是";
                if (ret == "False") return "否";
                return ret;
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                    for (int j = i + 1; j < data.Count; j++)
                    {
                        string res = await query2(data[i], data[j], question);
                        if (res!= "没有相关信息。"&&res!=null)
                        {
                            if ((question.IndexOf("是不是") != -1 || question.IndexOf("是否") != -1) && data.Contains(res))
                            {
                                return "是";
                            }
                            else
                            {
                                if (res == "True") return "是";
                                if (res == "False") return "否";
                                return res;
                            }
                        }
                    }
            }
            return "没有相关信息。";
        }


        public async Task<string> query2(string data0,string data1,string question)
        {
            using (DocumentClient client = new DocumentClient(new Uri(endpoint), authKey,
                      new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
            {
                     DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                     UriFactory.CreateDatabaseUri("graphdb_dut"),
                     new DocumentCollection { Id = "persons" },
                     new RequestOptions { OfferThroughput = 1000 });

               
                StringBuilder query_str = new StringBuilder();
                query_str.Append($"g.V('{data0}').outE('{data1}').inV()");
                IDocumentQuery<dynamic> query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                query_str.Clear();
                while (query_result.HasMoreResults)
                {
                    foreach (dynamic i in await query_result.ExecuteNextAsync())
                        return i.id;
                }

                query_str.Append($"g.V('{data1}').outE('{data0}').inV()");
                query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                query_str.Clear();
                while (query_result.HasMoreResults)
                {
                    foreach (dynamic i in await query_result.ExecuteNextAsync())
                        return i.id;
                }

                query_str.Append($"g.V('{data0}')");
                query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                query_str.Clear();
                while (query_result.HasMoreResults)
                {
                    foreach (dynamic i in await query_result.ExecuteNextAsync())
                        if (i.properties[data1]!=null)
                            return i.properties[data1][0].value;
                }

                query_str.Append($"g.V('{data1}')");
                query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                query_str.Clear();
                while (query_result.HasMoreResults)
                {
                    foreach (dynamic i in await query_result.ExecuteNextAsync())
                        if (i.properties[data0] != null)
                            return i.properties[data1][0].value;
                }

                return "没有相关信息。";
            }
        }
    }
}