using Microsoft.Bot.Connector;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (context.Request.Method == "POST")
            {
                Console.WriteLine(context.Request.Uri.PathAndQuery);
            }

            if(context.Request.Method == "OPTIONS")
            {
                string origin = context.Request.Headers.Get("Origin");
                context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
                return;
            }

            await Next.Invoke(context);
        }
    }
}
