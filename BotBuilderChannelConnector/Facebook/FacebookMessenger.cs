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

            builder
                .RegisterInstance(configs);

            builder
                .Register(c =>
                {
                    var activity = c.Resolve<IMessageActivity>();
                    var cfgs = c.Resolve<FacebookConfig[]>();
                    return cfgs.Single(cc => cc.PageId == activity.Recipient.Id);
                })
                .As<FacebookConfig>();

            builder
                .Register(c =>
                {
                    var cfg = c.Resolve<FacebookConfig>();
                    return new FacebookConnectorClientFactory(cfg.PageId);
                })
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
