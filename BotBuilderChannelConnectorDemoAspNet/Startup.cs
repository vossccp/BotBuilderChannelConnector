using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vossccp.BotBuilder.ChannelConnector.Facebook;
using Vossccp.BotBuilder.ChannelConnector.Owin.Facebook;

namespace Vossccp.BotBuilder.ChannelConnector.Demo.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            RegisterInMemoryBotStore();

            var settings = ConfigurationManager.AppSettings;

            appBuilder.UseFacebookMessenger(
                config: new FacebookApiConfig
                {
                    Path = "/messages",
                    PageId = settings["PageId"],
                    AppId = settings["AppId"],
                    AppSecret = settings["AppSecret"],
                    VerifyToken = settings["VerificationToken"],
                    PageAccessToken = settings["PageAccessToken"]
                },
                onActivityAsync: (activity) =>
                {
                    return Conversation.SendAsync(activity, () => new EchoDialog());                    
                }
            );
        }

        static void RegisterInMemoryBotStore()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<InMemoryDataStore>()
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new CachingBotDataStore(c.Resolve<InMemoryDataStore>(), CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                .As<IBotDataStore<BotData>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}