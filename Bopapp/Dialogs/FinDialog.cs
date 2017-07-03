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
using System.Web;
using System.Net.Http.Headers;

namespace Bopapp.Dialogs
{
    public static class DialogContext
    {
        static string context;
        public static void clear()
        {
            context = null;
        }

        public static void update(string s) => context = s;

        public static string peek() => context;
        static DialogContext() { context = null; }
    }

    [Serializable]
    public class FinDialog : IDialog<object>
    {
        string endpoint = ConfigurationManager.AppSettings["Endpoint"];
        string authKey = ConfigurationManager.AppSettings["AuthKey"];


        public readonly List<string> unuseful_word =new List<string>{"是","有"};
        public readonly List<string> qlocation_word = new List<string> {"在哪里","在哪儿","在哪" };


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(AnswerAsync);
        }

        public virtual async Task AnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> para)
        {

            var message = await para;
            string question = message.Text.Replace(" ","");
            question = message.Text.Replace("你们学校", "大连理工大学");
            question = message.Text.Replace("你们", "大连理工大学");

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

            if (DialogContext.peek() != null)
            {
                keyword.Add(DialogContext.peek());
                DialogContext.clear();
            }

            foreach(var i in qlocation_word)
                if (question.IndexOf(i) != -1 && !keyword.Contains("位于"))
                {
                    keyword.Add("位于");
                }

            for (int i = 0; i != words.Count(); i++)
            {
                if ( (chara[i] == "n" || chara[i] == "v" || chara[i] == "nt" || chara[i]=="nz") && (!unuseful_word.Contains(words[i]))  )
                {
                    await context.PostAsync("keyword: "+words[i]);
                    keyword.Add(words[i]);
                }
                if (chara[i] == "nr" && i - 1 >= 0 && chara[i - 1] == "nr")
                {
                    await context.PostAsync("keyword:"+words[i - 1] + words[i]);
                    keyword.Add(words[i - 1] + words[i]);
                }
                if (chara[i] == "nr")
                {
                    await context.PostAsync("keyword:"+words[i]);
                    keyword.Add(words[i]);
                }
            }

            if (keyword.Count() <= 1)
            {
                await context.PostAsync("请更加具体的描述您的问题。");
                context.Wait(AnswerAsync);
                return;
            }

            bool newres = false;string stmp=null;string r1 = null;string r2=null;
            do
            {
                for (int i = 0; i != keyword.Count(); i++)
                {
                    for (int j = i + 1; j != keyword.Count(); j++)
                    {
                        if((stmp=Cache.query(keyword[i],keyword[j]))!=null)
                        {
                            r1 = keyword[i];r2 = keyword[j];
                            newres = true;
                            break;
                        }
                    }
                    if (newres)
                        break;
                }
                if (newres)
                {
                    await context.PostAsync("removing " + r1);
                    await context.PostAsync("removing " + r2);
                    await context.PostAsync("adding " + stmp);
                    keyword.Remove(r1);keyword.Remove(r2);
                    keyword.Add(stmp);
                    newres = false;
                }
                if (keyword.Count() == 1)
                {
                    await context.PostAsync(keyword[0]);
                    DialogContext.update(keyword[0]);
                    context.Wait(AnswerAsync);
                    return;
                }
            } while (newres == true);


            for (int i = 0; i != keyword.Count(); i++)
                for (int j = i + 1; j != keyword.Count(); j++)
                {
                    if (keyword[i] == keyword[j] && (question.IndexOf("是不是") != -1 || question.IndexOf("是否") != -1 || (question.IndexOf("是") != -1 && question.IndexOf("吗") != -1)))
                    {
                        await context.PostAsync("是");
                        DialogContext.update("是");
                        context.Wait(AnswerAsync);
                        return;
                    }
                }


            string ans = await TryAll(keyword, question);
            if (ans == "没有相关信息。" && stmp != null)
                ans = stmp;

            await context.PostAsync(ans);
            DialogContext.update(ans);
            context.Wait(AnswerAsync);
        }

        public async Task<string> TryAll(List<string> data,string question)
        {
            if(data.Count == 1)
            {
                return "请更加具体的描述您的问题。";
            }

            if (data.Count == 2)
            {
                string ret = await query2(data[0], data[1], question);
                if (ret == "True"||ret=="true") return "是";
                if (ret == "False"||ret=="false") return "否";

                DialogContext.update(ret);
                return ret;
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                    for (int j = 0; j < data.Count; j++)
                    {
                        if (i == j)
                            continue;
                        string res = await query2(data[i], data[j], question);
                        if (res!= "没有相关信息。"&&res!=null)
                        {
                            if ((question.IndexOf("是不是") != -1 || question.IndexOf("是否") != -1 || (question.IndexOf("是")!=-1&&question.IndexOf("吗")!=-1)) && data.Contains(res))
                            {
                                DialogContext.update("是");
                                return "是";
                            }
                            else
                            {
                                if (res == "True" || res=="true") return "是";
                                if (res == "False" || res=="false") return "否";
                                DialogContext.update(res);
                                return res;
                            }
                        }
                    }
            }


            if ((question.IndexOf("是不是") != -1 || question.IndexOf("是否") != -1 || (question.IndexOf("是") != -1 && question.IndexOf("吗") != -1)))
            {
                DialogContext.update("否");
                return "否";
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