using Autofac;
using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

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
            ChannelConnector.AddDirectlineConfig(configs);

            var httpConfig = new HttpConfiguration();
            foreach (var directlineConfig in configs)
            {
                httpConfig.Properties.TryAdd(directlineConfig.ApiKey, directlineConfig);
            }

            httpConfig.Properties.TryAdd("callback", onActivityAsync);
            var formatter = httpConfig.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            httpConfig.Routes.MapHttpRoute
            (
                name: "directline",
                routeTemplate: "directline/{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }
            );

            return appBuilder.UseWebApi(httpConfig);
        }
    }
}
