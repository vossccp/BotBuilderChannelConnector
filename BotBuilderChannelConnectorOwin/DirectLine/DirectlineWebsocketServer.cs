using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	// Mostly adapted from here: https://github.com/learncode/NoteApp

	using WebSocketAccept =
		Action<IDictionary<string, object>, /* options */ Func<IDictionary<string, object>, Task>>; // callback

	public static class DirectlineWebsocketServer
	{
		public static void UseDirectlineWebSockets(this IAppBuilder appBuilder)
		{
			DirectlineWebSocketHandler.JsonSerializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				NullValueHandling = NullValueHandling.Ignore
			};

			appBuilder.Use(UpgradeToWebSockets);
		}

		static Task UpgradeToWebSockets(IOwinContext context, Func<Task> next)
		{
			var accept = context.Get<WebSocketAccept>("websocket.Accept");
			if (accept == null)
			{
				// Not a websocket request
				return next();
			}

			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

			var token = context.Request.Query
				.SingleOrDefault(p => p.Key == "t").Value;

			if (token.Length != 0)
			{
				var handler = DirectlineWebSocketHandler.GetByToken(token[0]);
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

					context.Response.StatusCode = (int) HttpStatusCode.OK;
				}
				else
				{
					Trace.TraceWarning($"Not handler for token {token[0]}");
				}
			}
			else
			{
				Trace.TraceWarning("No token");
			}

			return Task.CompletedTask;
		}
	}
}
