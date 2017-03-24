using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public static class FacebookMessenger
    {
        public static FacebookConfig[] configs;

        public static FacebookConfig GetConfig(string pageId)
        {
            if (configs == null)
            {
                throw new BotConnectorException("Facebook channels are not configured");
            }
            return configs.SingleOrDefault(c => c.PageId == pageId);
        }

        public static void Configure(FacebookConfig[] configs)
        {
            var builder = new ContainerBuilder();

            builder
                .Register(c =>
                {
                    var activity = c.Resolve<IMessageActivity>();
                    return new DirectConnectorClientFactory(activity);
                })
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
