using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	public class DirectlineMessagesController : ApiController
	{
		public HttpResponseMessage Get()
		{
			var owin = Request.GetOwinContext();

			var context = HttpContext.Current;

			if (context.IsWebSocketRequest || context.IsWebSocketRequestUpgrading)
			{
				context.AcceptWebSocketRequest(ProcessWebSocketSession);
			}

			return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
		}

		Task ProcessWebSocketSession(AspNetWebSocketContext context)
		{
			var handler = new DirectlineWebSocketHandler();
			return handler.ProcessWebSocketRequestAsync(context);
		}
	}
}
