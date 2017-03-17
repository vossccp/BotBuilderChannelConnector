using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Rest;
using Newtonsoft.Json;
using Bot.Builder.ChannelConnector.Facebook;

namespace Bot.Builder.ChannelConnector
{
    public class DirectConnectorClient : IConnectorClient
    {
        public DirectConnectorClient(FacebookClient client)
        {
            Conversations = new DirectConversation(client);
        }

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

        public IConversations Conversations { get; }

        public void Dispose()
        {
        }
    }
}