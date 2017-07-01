using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

	using WebSocketCloseAsync =
		Func<int /* closeStatus */, string /* closeDescription */, CancellationToken /* cancel */, Task>;

	using WebSocketReceiveAsync =
		Func<ArraySegment<byte> /* data */, CancellationToken /* cancel */, Task<Tuple<int /* messageType */, bool /* endOfMessage */, int /* count */>>>;

	using WebSocketSendAsync =
		Func<ArraySegment<byte> /* data */, int /* messageType */, bool /* endOfMessage */, CancellationToken /* cancel */, Task>;

	using WebSocketReceiveResult = Tuple<int, // type
		bool, // end of message?
		int>; // count

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
						var sendAsync = (WebSocketSendAsync)websocketContext["websocket.SendAsync"];
						var receiveAsync = (WebSocketReceiveAsync)websocketContext["websocket.ReceiveAsync"];
						var closeAsync = (WebSocketCloseAsync)websocketContext["websocket.CloseAsync"];
						var callCancelled = (CancellationToken)websocketContext["websocket.CallCancelled"];

						await handler.OpenAsync(sendAsync);

						var buffer = new byte[64];

						await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
						while (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out object status) || (int)status == 0)
						{
							// No need to receive any data from the websocket
							// therefore, anything dealing with receiving data is omitted
							await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
						}

						handler.Close();

						await closeAsync
						(
							(int)websocketContext["websocket.ClientCloseStatus"],
							(string)websocketContext["websocket.ClientCloseDescription"],
							callCancelled
						);
					});

					context.Response.StatusCode = (int)HttpStatusCode.OK;
				}
				else
				{
					Trace.WriteLine($"Not handler for token {token[0]}");
				}
			}
			else
			{
				Trace.WriteLine("No token");
			}

			return Task.FromResult<object>(null);
		}
	}
}
