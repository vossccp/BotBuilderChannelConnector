using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Bot.Builder.ChannelConnector.Directline;
using Bot.Builder.ChannelConnector.Facebook;
using Bot.Builder.ChannelConnector.Owin.DirectLine;
using Newtonsoft.Json.Serialization;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public static class FacebookOwinConfig
    {
        public static IAppBuilder UseFacebookMessenger(this IAppBuilder appBuilder, FacebookConfig config, Func<IMessageActivity, Task> onActivityAsync)
        {
            return appBuilder.UseFacebookMessenger(new[] { config }, onActivityAsync);
        }

        // adds support for multiple Facebook bots: each config element represents one bot
        // All bots can then be handled through one entry point defined in onActivityAsync
        public static IAppBuilder UseFacebookMessenger(this IAppBuilder appBuilder, FacebookConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            ChannelConnector.AddFacebookMessengerConfig(configs);

            var builder = new ContainerBuilder();

            builder.RegisterType<FacebookMessengerController>().InstancePerRequest();
            builder.RegisterInstance(configs.ToDictionary(c => c.PageId, c => c)).SingleInstance();
            builder.RegisterInstance(onActivityAsync);

            var container = builder.Build();

            var httpConfig = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container)
            };

            var formatter = httpConfig.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            httpConfig.Routes.MapHttpRoute
            (
                name: "facebook",
                routeTemplate: "messages",
                defaults: new
                {
                    controller = "FacebookMessenger"
                }
            );

            return appBuilder.UseWebApi(httpConfig);
        }
    }
}
