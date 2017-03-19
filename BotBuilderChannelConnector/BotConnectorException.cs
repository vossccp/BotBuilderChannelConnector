using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector
{
    public class BotConnectorException : Exception
    {
        public BotConnectorException(string msg)
            : base(msg)
        {
        }
    }
}
