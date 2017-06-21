using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Bot.Connector;

namespace Bopapp.Dialogs
{
    [Serializable]
    public class qnamaker : IDialog<object>
    {
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var client = new HttpClient();
           
            var queryStr = HttpUtility.ParseQueryString(message.Text);

            client.DefaultRequestHeaders.Add("Host", "westus.api.cognitive.microsoft.com");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "27bdef54a876461ea2e90602576e4815");

            var uri = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/" +
                "knowledgebases/5d5bb1ca-087b-4a01-8691-6a9b2ca6bd14/generateAnswer";

            HttpResponseMessage response;
            byte[] byteData = Encoding.UTF8.GetBytes("{\"question\":\"hi\",\"top\":3}");
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
            await context.PostAsync(response.ToString());
            string result = await response.Content.ReadAsStringAsync();
            await context.PostAsync(result);
           
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}