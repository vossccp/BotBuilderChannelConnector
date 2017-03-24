using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public class FacebookConnectorClient : IConnectorClient
    {
        public FacebookConnectorClient(FacebookClient client)
        {
            Conversations = new FacebookConversation(client);
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