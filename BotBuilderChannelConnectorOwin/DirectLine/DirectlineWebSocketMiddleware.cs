using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	using WebSocketAccept =
		Action<IDictionary<string, object>, /* options */ Func<IDictionary<string, object>, Task>>; // callback

	public class DirectlineWebSocketMiddleware : OwinMiddleware
	{
		readonly DirectlineWebSocketHandlerRegistry handlerRegistry;

		public DirectlineWebSocketMiddleware(OwinMiddleware next, DirectlineWebSocketHandlerRegistry handlerRegistry)
			: base(next)
		{
			this.handlerRegistry = handlerRegistry;
		}

		static bool server = false;

		public override Task Invoke(IOwinContext context)
		{
			var accept = context.Get<WebSocketAccept>("websocket.Accept");
			if (accept == null)
			{
				// Not a websocket request
				//
				// Note: Windows 7 doenst nativly support WebSockets 
				// and will not return a socket despite the request being a propper 
				// web socket request
				return Next.Invoke(context);
			}

			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

			var token = context.Request.Query
				.SingleOrDefault(p => p.Key == "t").Value;

			if (token.Length == 0)
			{
				Trace.TraceWarning("No token");
				return Task.CompletedTask;
			}

			var handler = handlerRegistry.GetByToken(token[0]);
			if (handler != null)
			{
				accept(context.Environment, async websocketContext =>
				{
					var webSocket = new OwinWebSocket(websocketContext);

					await handler.OpenAsync(webSocket);

					var buffer = new byte[64];
					while (webSocket.State == WebSocketState.Open)
					{
							// No need to receive any data from the websocket
							// therefore, anything dealing with receiving data is omitted							
							await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer));
					}

					handler.Close();

					await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription);
				});

				context.Response.StatusCode = (int)HttpStatusCode.SwitchingProtocols;
			}
			else
			{
				Trace.TraceWarning($"Not handler for token {token[0]}");
			}

			return Task.CompletedTask;
		}
	}
}
