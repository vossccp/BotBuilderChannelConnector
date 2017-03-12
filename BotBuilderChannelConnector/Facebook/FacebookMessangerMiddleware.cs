using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vossccp.BotBuilder.ChannelConnector.Facebook.Schema;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
{
    public class FacebookMessangerMiddleware : OwinMiddleware
    {
        readonly FacebookConfig config;
        readonly Func<IMessageActivity, Task> onActivityAsync;

        public FacebookMessangerMiddleware(OwinMiddleware next, FacebookConfig config, Func<IMessageActivity, Task> onActivityAsync)
            : base(next)
        {
            this.config = config;
            this.onActivityAsync = onActivityAsync;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Uri.LocalPath.Equals("/messages", StringComparison.InvariantCultureIgnoreCase))
            {
                if (context.Request.Query["hub.mode"] == "subscribe")
                {
                    await Subscribe(context);
                    return;
                }

                if (context.Request.Method == "POST")
                {
                    try
                    {
                        await MessageReceived(context);
                        return;
                    }
                    catch(Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        Trace.WriteLine(exception.StackTrace);

                        if(exception.InnerException!=null)
                        {
                            Trace.WriteLine(exception.InnerException.Message);
                        }

                        throw;
                    }
                }
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        async Task MessageReceived(IOwinContext context)
        {            
            var content = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var message = JsonConvert.DeserializeObject<FacebookRequestMessage>(content);
            var activities = message.ToMessageActivities();

            foreach (var activity in activities)
            {
                Trace.TraceInformation("Recieved activity {0}", activity.Id);
                await onActivityAsync(activity);
            }
        }

        async Task Subscribe(IOwinContext context)
        {
            Trace.TraceInformation("Received subscribtion request");

            var verifyToken = context.Request.Query["hub.verify_token"];
            if (Equals(config.VerifyToken, verifyToken))
            {
                await context.Response.WriteAsync(context.Request.Query["hub.challenge"]);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
