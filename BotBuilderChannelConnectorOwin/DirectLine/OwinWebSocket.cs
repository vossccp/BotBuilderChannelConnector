using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

	using wsWebSocketReceiveResult = Tuple<int, // type
		bool, // end of message?
		int>; // count


	public class OwinWebSocket : WebSocket
	{
		readonly IDictionary<string, object> websocketContext;
		readonly WebSocketSendAsync wsSendAsync;
		readonly WebSocketReceiveAsync wsReceiveAsync;
		readonly WebSocketCloseAsync wsCloseAsync;		

		public OwinWebSocket(IDictionary<string, object> websocketContext)
		{
			this.websocketContext = websocketContext;

			wsSendAsync = (WebSocketSendAsync)websocketContext["websocket.SendAsync"];
			wsCloseAsync = (WebSocketCloseAsync)websocketContext["websocket.CloseAsync"];
			wsReceiveAsync = (WebSocketReceiveAsync)websocketContext["websocket.ReceiveAsync"];			
		}

		CancellationToken WsCancellationToken
			=> (CancellationToken) websocketContext["websocket.CallCancelled"];

		static WebSocketMessageType OpCodeToEnum(int messageType)
		{
			switch (messageType)
			{
				case 0x1:
					return WebSocketMessageType.Text;
				case 0x2:
					return WebSocketMessageType.Binary;
				case 0x8:
					return WebSocketMessageType.Close;
				default:
					throw new ArgumentOutOfRangeException(nameof(messageType), messageType, string.Empty);
			}
		}

		static int EnumToOpCode(WebSocketMessageType webSocketMessageType)
		{
			switch (webSocketMessageType)
			{
				case WebSocketMessageType.Text:
					return 0x1;
				case WebSocketMessageType.Binary:
					return 0x2;
				case WebSocketMessageType.Close:
					return 0x8;
				default:
					throw new ArgumentOutOfRangeException(nameof(webSocketMessageType), webSocketMessageType, string.Empty);
			}
		}

		public override void Abort()
		{
			throw new NotImplementedException();
		}

		public override async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			await wsCloseAsync((int)closeStatus, statusDescription, cancellationToken);
		}

		public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription)
		{
			return CloseAsync(closeStatus, statusDescription, WsCancellationToken);
		}

		public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{			
		}

		public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			var result = await wsReceiveAsync(buffer, cancellationToken);
			var messageType = OpCodeToEnum(result.Item1);
			return new WebSocketReceiveResult(result.Item3, messageType, result.Item2);
		}

		public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer)
		{
			return ReceiveAsync(buffer, WsCancellationToken);
		}

		public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
			CancellationToken cancellationToken)
		{
			var messageTypeInt = EnumToOpCode(messageType);
			await wsSendAsync(buffer, messageTypeInt, endOfMessage, cancellationToken);
		}

		public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage)
		{
			return SendAsync(buffer, messageType, endOfMessage, WsCancellationToken);
		}

		public override WebSocketCloseStatus? CloseStatus
		{
			get
			{
				if (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out object status))
				{
					return null;
				}

				var closeValue = (int) status;
				if (closeValue == 0)
				{
					return null;
				}

				return (WebSocketCloseStatus) status;
			}
		}

		public override string CloseStatusDescription
		{
			get
			{
				if (!websocketContext.TryGetValue("websocket.ClientCloseDescription", out object description))
				{
					return null;
				}
				return (string) description;
			}
		}

		public override string SubProtocol => throw new NotImplementedException();

		public override WebSocketState State
		{
			get
			{
				var closeStatus = CloseStatus;
				if (closeStatus == null)
				{
					return WebSocketState.Open;
				}
				return WebSocketState.Closed;
			}
		}
	}
}
