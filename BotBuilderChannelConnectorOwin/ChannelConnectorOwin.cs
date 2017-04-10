using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Owin
{
    public static class ChannelConnectorOwin
    {
        public static async Task OnMessageReceived(IMessageActivity activity, Func<IMessageActivity, Task> onActivityAsync)
        {
            Trace.TraceInformation("Recieved activity {0} for {1}", activity.Id, activity.Recipient.Id);
            try
            {
                await onActivityAsync(activity);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    Trace.WriteLine(exception.InnerException.Message);
                }

                throw;
            }
        }
    }
}
