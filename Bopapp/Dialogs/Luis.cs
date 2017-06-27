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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
namespace Bopapp.Dialogs
{
    /// <summary>
    /// Luis对话框，语义分析完成后的处理。
    /// </summary>
    [LuisModel("fdede7ca-c1c3-43ca-b4c7-375ec27aa5e3", "7791146f5ae6432ba902a046e1a76ff9",LuisApiVersion.V2, @"southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class LuisDialog : LuisDialog<object>
    {
        string endpoint = ConfigurationManager.AppSettings["Endpoint"];
        string authKey = ConfigurationManager.AppSettings["AuthKey"];

        public LuisDialog()
        {
            
        }


        public LuisDialog(ILuisService service) : base(service)
        {
        }

        #region 无法判断意图
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"没有相关信息。";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        #endregion

        #region 判断是非
        [LuisIntent("判断是非")]
        public async Task QueryTrueOrFalse(IDialogContext context, LuisResult result)
        {
            context.PostAsync("判断是非。");
            string question = result.Query;StringBuilder query_str = new StringBuilder();
            bool Flag = false;
            List<string> para = new List<string>();
            foreach (var i in result.Entities)
            { 
                para.Add(i.Entity);
               // await context.PostAsync(i.Entity);
            }


            if (para.Count() == 2)
            {
                query_str.Append($"g.V('{para[0]}').outE('{para[1]}').inV()");
            }
            else if (para.Count() == 3)
            {
                query_str.Append($"g.V('{para[0]}').outE('{para[1]}').inV().has('id','{para[2]}')");
            }
            else
            {
                Flag = true;
                await context.PostAsync("to be continued");
            }

            if (!Flag)
            {
                using (DocumentClient client = new DocumentClient(new Uri(endpoint), authKey,
                 new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
                {
                    DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                       UriFactory.CreateDatabaseUri("graphdb_dut"),
                       new DocumentCollection { Id = "persons" },
                       new RequestOptions { OfferThroughput = 1000 });

                    IDocumentQuery<dynamic> query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                    if (!query_result.HasMoreResults)
                    {
                        query_str.Clear();
                        query_str.Append($"g.V('{para[0]}'");
                        query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                        while (query_result.HasMoreResults)
                        {
                            foreach (dynamic i in await query_result.ExecuteNextAsync())
                            {
                                if (i.properties.ContainsKey(para[1]))
                                {
                                    var res = i.properties[para[1]];
                                    if (res == false)
                                        await context.PostAsync("否。");
                                    else
                                        await context.PostAsync("是。");
                                }
                                else
                                {
                                    await context.PostAsync("没有相关信息。");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (para.Count() == 2)
                        {
                            bool suc = false;
                            while (query_result.HasMoreResults)
                            {
                                foreach (dynamic i in await query_result.ExecuteNextAsync())
                                {
                                    if (question.IndexOf(i.id) != -1)
                                    {
                                        suc = true;
                                        await context.PostAsync("是");
                                    }
                                }
                            }
                            if(!suc)
                                await context.PostAsync("否");

                        }
                        else
                            await context.PostAsync("是");
                    }
                }
            }
            context.Wait(MessageReceived);
        }
        #endregion

        #region 查询事物 
        [LuisIntent("查询事物")]
        public async Task QueryThing(IDialogContext context, LuisResult luis_res)
        {
            await context.PostAsync("查询事物");
            StringBuilder tmp = new StringBuilder();
            if (luis_res.Entities.Count() == 2)
            {
                tmp.Append($"g.V('{luis_res.Entities[0]}').outE('{luis_res.Entities[1]}').inV()");
            }
            else
            {
                await context.PostAsync("to be continued");
                return;
            }

            string query_str = tmp.ToString();
            using (DocumentClient client = new DocumentClient(new Uri(endpoint), authKey,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp }))
            {
                DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                   UriFactory.CreateDatabaseUri("graphdb_dut"),
                   new DocumentCollection { Id = "persons" },
                   new RequestOptions { OfferThroughput = 1000 });

                IDocumentQuery<dynamic> query_result = client.CreateGremlinQuery<dynamic>(graph, query_str.ToString());
                while (query_result.HasMoreResults)
                {
                    foreach (dynamic i in await query_result.ExecuteNextAsync())
                    {
                        await context.PostAsync(i.id);
                    }
                }
            }
        }
        #endregion

        #region 查询时间
        [LuisIntent("查询时间")]
        public async Task QueryTime(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("查询时间。");
            context.Wait(MessageReceived);
        }
        #endregion

        #region 查询数量
        [LuisIntent("查询数量")]
        public async Task QueryNumber(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("查询数量。");
            context.Wait(MessageReceived);
        }
        #endregion
    }
}