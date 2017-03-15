using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
{
    public static class FacebookMessenger
    {
        public static void Configure(FacebookConfig config)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => config)
                .SingleInstance();

            builder
                .Register(c => new DirectConnectorClientFactory(c.Resolve<FacebookConfig>()))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
