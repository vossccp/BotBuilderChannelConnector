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
using Vossccp.BotBuilder.ChannelConnector.Facebook;

namespace Vossccp.BotBuilder.ChannelConnector.Owin.Facebook
{
    public static class FacebookMiddlewareConfig
    {
        public static IAppBuilder UseFacebookMessenger(this IAppBuilder appBuilder, FacebookConfig config, Func<IMessageActivity, Task> onActivityAsync)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => config)
                .SingleInstance();

            builder
                .Register(c => new DirectConnectorClientFactory(c.Resolve<FacebookConfig>()))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);

            appBuilder.Use<FacebookMessangerMiddleware>(config, onActivityAsync);

            return appBuilder;
        }
    }
}
