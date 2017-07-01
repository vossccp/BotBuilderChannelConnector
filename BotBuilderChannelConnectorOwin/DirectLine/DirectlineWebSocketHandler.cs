using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	using WebSocketSendAsync = Func<ArraySegment<byte>, int, bool, CancellationToken, Task>;

	public class DirectlineWebSocketHandler
	{
		public static JsonSerializerSettings JsonSerializerSettings { get; set; }

		public static TimeSpan TokenExpirationTime = TimeSpan.FromSeconds(1800);

		static readonly ConcurrentDictionary<string, DirectlineWebSocketHandler> HandlersByToken
			= new ConcurrentDictionary<string, DirectlineWebSocketHandler>();

		public static DirectlineWebSocketHandler GetByToken(string token)
		{
			// Token to be used only one time to establish connection

			if (HandlersByToken.TryRemove(token, out DirectlineWebSocketHandler handler))
			{
				if (!handler.IsTokeExpired)
				{
					return handler;
				}
			}
			return null;
		}

		public static DirectlineWebSocketHandler Create(DirectlineChat chat)
		{
			var handler = new DirectlineWebSocketHandler(chat);
			HandlersByToken.TryAdd(handler.Token, handler);
			return handler;
		}

		WebSocketSendAsync sendAsync;

		public string Token { get; }
		public DateTime DateIssued { get; }
		public DirectlineChat Chat { get; }

		int Watermark { get; set; }

		public bool IsTokeExpired => DateTime.UtcNow > DateIssued.Add(TokenExpirationTime);

		DirectlineWebSocketHandler(DirectlineChat chat)
		{
			Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace(" ", "");
			DateIssued = DateTime.UtcNow;
			Chat = chat;
			Watermark = 0;
		}

		public async Task OpenAsync(WebSocketSendAsync sendAsync)
		{
			this.sendAsync = sendAsync;

			Trace.WriteLine("Opening web socket connection");

			Chat.OnActivityAdded = async activity => await SendActivitiesAsync(new[] { activity });

			var activities = (await Chat.GetActvitiesAsync()).Skip(Watermark).ToArray();
			await SendActivitiesAsync(activities);
		}

		public void Close()
		{
			Chat.OnActivityAdded = null;
			sendAsync = null;
		}

		async Task SendActivitiesAsync(Activity[] activities, bool addWatermark = true)
		{
			if (sendAsync == null) return;

			Watermark = Watermark + activities.Length;
			var result = new
			{
				Activities = activities,
				Watermark = addWatermark ? Watermark : (int?)null
			};

			var json = JsonConvert.SerializeObject(result, JsonSerializerSettings);

			var bytes = Encoding.UTF8.GetBytes(json);
			try
			{
				await sendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), 1, true, CancellationToken.None);
			}
			catch (Exception e)
			{
				Trace.WriteLine($"Error sending to websocket {e}");
				throw;
			}
		}
	}
}
