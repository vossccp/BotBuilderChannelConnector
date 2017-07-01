using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	public class DirectlineConversation
	{
		[JsonProperty("conversationId")]
		public string Id { get; set; }
		[JsonProperty("token")]
		public string Token { get; set; }
		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
		[JsonProperty("streamUrl")]
		public string StreamUrl { get; set; }
	}
}
