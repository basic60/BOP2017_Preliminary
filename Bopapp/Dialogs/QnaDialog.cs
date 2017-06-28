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
                await context.PostAsync(words[i]+" "+chara[i]);
                if (chara[i] == "n" || chara[i] == "v")
                {
                    keyword.Add(chara[i]);
                }
            }

            /*var extractor = new TfidfExtractor();
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
            }*/


            context.Wait(AnswerAsync);
        }

        public string TryAll(List<string> data)
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

            }
            return null;
        }
    }
}