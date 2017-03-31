using Autofac;
using Bot.Builder.ChannelConnector.Directline;
using Bot.Builder.ChannelConnector.Facebook;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector
{
    public static class ChannelConnector
    {
        public static void AddDirectlineConfig(DirectlineConfig[] configs)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(configs.ToDictionary(c => c.BotName, c => c));

            builder
                .Register(c =>
                {
                    var cfgs = c.Resolve<Dictionary<string, DirectlineConfig>>();
                    var activity = c.Resolve<IMessageActivity>();
                    return cfgs[activity.Recipient.Name];
                })
                .As<DirectlineConfig>();

            builder
                .Register(c => c.Resolve<DirectlineConfig>().ChatLog)
                .As<IChatLog>()
                .InstancePerLifetimeScope();

            Configure(builder);

            builder.Update(Conversation.Container);
        }

        public static void AddFacebookMessengerConfig(FacebookConfig[] configs)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(configs.ToDictionary(c => c.PageId, c => c));

            builder
                .Register(c =>
                {
                    var activity = c.Resolve<IMessageActivity>();
                    var cfgs = c.Resolve<Dictionary<string, FacebookConfig>>();
                    return cfgs[activity.Recipient.Id];
                })
                .As<FacebookConfig>();

            Configure(builder);

            builder.Update(Conversation.Container);
        }

        static void Configure(ContainerBuilder builder)
        {
            builder
                .Register<IConnectorClientFactory>(c =>
                {
                    var activity = c.Resolve<IMessageActivity>();

                    switch (activity.ChannelId)
                    {
                        case "facebook":
                            var fbConfig = c.Resolve<FacebookConfig>();
                            return new FacebookConnectorClientFactory(fbConfig.PageAccessToken);
                        case "directline":
                            return new DirectlineConnectorClientFactory(activity.Conversation.Id, c.Resolve<IChatLog>());
                        default:
                            throw new NotSupportedException($"{activity.ChannelId} is not supported");
                    }
                })
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();
        }
    }
}
