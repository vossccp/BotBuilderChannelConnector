using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Bot.Builder.ChannelConnector.Directline;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public class DirectlineAuthorizationFilter : IAutofacAuthorizationFilter
    {
        readonly Dictionary<string, DirectlineConfig> configs;

        public DirectlineAuthorizationFilter(Dictionary<string, DirectlineConfig> configs)
        {
            this.configs = configs;
        }

        public Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null)
            {
                if (configs.TryGetValue(auth.Parameter, out DirectlineConfig config))
                {
                    var principal = new GenericPrincipal
                    (
                        identity: new GenericIdentity(config.ApiKey),
                        roles: new string[] { }
                    );
                    actionContext.ControllerContext.RequestContext.Principal = principal;
                    return Task.CompletedTask;
                }
            }

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            return Task.CompletedTask;
        }
    }
}
