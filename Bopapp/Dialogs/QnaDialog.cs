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
    public class QnaDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(AnswerAsync);
        }

        public virtual async Task AnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> para)
        {
            var message = await para;
            string question = message.Text,answer="";
            var posSeg = new PosSegmenter();
            var tokens = posSeg.Cut(question);

            await context.PostAsync(question);

            var extractor = new TfidfExtractor();
            IEnumerable<string> keywords = extractor.ExtractTags(question, 10, Constants.NounAndVerbPos);
            if (keywords!=null)
            {
                foreach(var i in keywords)
                {
                    await context.PostAsync(i);
                }
            }
            else
            {
                await context.PostAsync("null");
            }


            await context.PostAsync(string.Join("", tokens.Select(token => string.Format("{0}  {1}\n", token.Word, token.Flag))));
            context.Wait(AnswerAsync);
        }
    }
}