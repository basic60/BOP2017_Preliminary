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
    /// <summary>
    /// Luis对话框，语义分析完成后的处理。
    /// </summary>
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

        #region 无法判断意图
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"不知道你在说什么，面壁去。。。T_T";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        #endregion

        #region 天气意图
        [LuisIntent("天气查询")]
        public async Task QueryWeather(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("匹配结果是查询天气。");
            context.Wait(MessageReceived);
        }
        #endregion

        #region 时间意图
        [LuisIntent("时间查询")]
        public async Task QueryTime(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("匹配结果是查询时间。");
            context.Wait(MessageReceived);
        }
        #endregion
    }
}