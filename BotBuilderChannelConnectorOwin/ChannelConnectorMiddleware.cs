using Microsoft.Bot.Connector;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin
{
    public abstract class ChannelConnectorMiddleware : OwinMiddleware
    {
        readonly Func<IMessageActivity, Task> onActivityAsync;

        protected ChannelConnectorMiddleware(OwinMiddleware next, Func<IMessageActivity, Task> onActivityAsync)
            : base(next)
        {
            this.onActivityAsync = onActivityAsync;
        }

        protected async Task OnMessageReceived(IMessageActivity activity)
        {
            Trace.TraceInformation("Recieved activity {0} for {1}", activity.Id, activity.Recipient.Id);
            try
            {
                await onActivityAsync(activity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
