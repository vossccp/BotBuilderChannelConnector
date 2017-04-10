using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Bot.Builder.ChannelConnector.Directline;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public class DirectlineAuthorizeAttribute : AuthorizeAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null)
            {
                if (actionContext.RequestContext.Configuration.Properties.TryGetValue(auth.Parameter, out object config))
                {
                    var directlineConfig = config as DirectlineConfig;
                    if (directlineConfig != null)
                    {
                        var principal = new GenericPrincipal
                        (
                            identity: new GenericIdentity(directlineConfig.ApiKey),
                            roles: new string[] { }
                        );
                        actionContext.ControllerContext.RequestContext.Principal = principal;
                        return Task.CompletedTask;
                    }
                }
            }

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            return Task.CompletedTask;
        }
    }
}
