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
        public static void Configure(FacebookConfig[] configs)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => configs)
                .SingleInstance();

            builder
                .Register(c =>
                {
                    var activity = c.Resolve<IMessageActivity>();
                    var fbConfigs = c.Resolve<FacebookConfig[]>();
                    var fbConfig = fbConfigs.SingleOrDefault(cfg => cfg.PageId == activity.Recipient.Id);

                    if (fbConfig == null)
                    {
                        string msg = $"No Facebook Configuration for PageId {activity.Recipient.Id}";
                        throw new BotConnectorException(msg);
                    }

                    return new DirectConnectorClientFactory(fbConfig);
                })
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
