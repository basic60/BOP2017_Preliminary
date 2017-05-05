using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Bopapp.Dialogs
{
    [LuisModel("1bf0c204-3e2b-4be3-a02a-b66f6a5f73d1", "ab09945bf0224fdb9ed956eb79683f6a")]
    [Serializable]
    public class LuisDialog : LuisDialog<object>
    {
        public LuisDialog()
        {
        }


        public LuisDialog(ILuisService service) : base(service)
        {
        }


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"不知道你在说什么，面壁去。。。T_T" + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("天气查询")]
        public async Task QueryWeather(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("匹配结果是查询天气。");
            context.Wait(MessageReceived);
        }

        [LuisIntent("时间查询")]
        public async Task QueryTime(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("匹配结果是查询时间。");
            context.Wait(MessageReceived);
        }

    }
}