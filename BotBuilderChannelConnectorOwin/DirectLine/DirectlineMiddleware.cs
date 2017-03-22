using Microsoft.Bot.Connector;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public class DirectlineMiddleware : OwinMiddleware
    {
        readonly DirectlineConfig[] configs;

        public DirectlineMiddleware(OwinMiddleware next, DirectlineConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
            : base(next)
        {
            this.configs = configs;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out AuthenticationHeaderValue value))
            {
                var config = configs.SingleOrDefault(c => c.ApiKey == value.Parameter);
                if (config == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
                if (!context.Request.Uri.LocalPath.StartsWith(config.Path))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                if (context.Request.Uri.LocalPath.EndsWith("conversations"))
                {
                    HandleConversationsRequests(context);
                }

                if (context.Request.Uri.LocalPath.EndsWith("activities"))
                {
                    HandleActivitiesRequests(context);
                }
            }


            //if(context.Request. == "OPTIONS")
            //{
            //    string origin = context.Request.Headers.Get("Origin");

            //    context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
            //    context.Response.Headers.Add("Access-Control-Allow-Credentials", new string[] { "true" });
            //    context.Response.Headers.Add("Access-Control-Allow-Headers", new string[] { "authorization, x-requested-with" });
            //    return;
            //}

            await Next.Invoke(context);
        }

        static string GetConversationId(Uri uri)
        {
            for (int i = 0; i < uri.Segments.Length; i++)
            {
                var segment = uri.Segments[i];

                if (segment.StartsWith("conversations") && i < uri.Segments.Length - 1)
                {
                    return uri.Segments[i + 1];
                }
            }
            return null;
        }

        private void HandleActivitiesRequests(IOwinContext context)
        {
            var conversationId = GetConversationId(context.Request.Uri);
            if (context.Request.Method == "GET")
            {
                var result = new Activity[0];

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(result));
            }
        }

        void HandleConversationsRequests(IOwinContext context)
        {
            if (context.Request.Method == "POST")
            {
                // create a new Conversation

                var response = new DirectlineConversation
                {
                    Id = Guid.NewGuid().ToShortGuid(),
                    Token = "ABC",
                    ExpiresIn = 1800,
                    StreamUrl = "wss://directline.botframework.com/v3/directline/conversations/KzsR7O3ubDneY3xw1G4h0/stream?watermark=-&t=8GBHONEMRyM.dAA.SwB6AHMAUgA3AE8AMwB1AGIARABuAGUAWQAzAHgAdwAxAEcANABoADAA.GgQKx_mi0gE.UfaufUv7sfw.VPVbgGKtQFIb3ZGzjUgHN1ksIjy6WJlvq9ivh13pJLU"
                };

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(response));

                return;
            }
        }
    }
}
