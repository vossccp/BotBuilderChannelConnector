using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public static class DirectLineMiddlewareConfig
    {
        public static IAppBuilder UseDirectline(this IAppBuilder appBuilder, DirectlineConfig config, Func<IMessageActivity, Task> onActivityAsync)
        {
            return appBuilder.UseDirectline(new[] { config }, onActivityAsync);
        }

        public static IAppBuilder UseDirectline(this IAppBuilder appBuilder, DirectlineConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            return appBuilder.Use<DirectlineMiddleware>(configs, onActivityAsync);
        }
    }
}
