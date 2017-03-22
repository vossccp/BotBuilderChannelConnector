using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Facebook;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public static class FacebookMiddlewareConfig
    {
        public static IAppBuilder UseFacebookMessenger(this IAppBuilder appBuilder, FacebookConfig config, Func<IMessageActivity, Task> onActivityAsync)
        {
            return appBuilder.UseFacebookMessenger(new[] { config }, onActivityAsync);
        }

        // adds support for multiple Facebook bots: each config element represents one bot
        // All bots can then be handled through one entry point defined in onActivityAsync
        public static IAppBuilder UseFacebookMessenger(this IAppBuilder appBuilder, FacebookConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            FacebookMessenger.Configure(configs);
            return appBuilder.Use<FacebookMessangerMiddleware>(configs, onActivityAsync);
        }
    }
}
