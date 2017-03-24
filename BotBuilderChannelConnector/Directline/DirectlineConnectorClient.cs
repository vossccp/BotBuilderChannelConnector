using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class DirectlineConnectorClient : IConnectorClient
    {
        public DirectlineConnectorClient(DirectlineChat chat)
        {
            Conversations = new DirectlineConversation(chat);
        }

        public IConversations Conversations { get; }

        public Uri BaseUri
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public JsonSerializerSettings SerializationSettings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public JsonSerializerSettings DeserializationSettings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ServiceClientCredentials Credentials
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IAttachments Attachments
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }        

        public void Dispose()
        {
        }
    }
}
