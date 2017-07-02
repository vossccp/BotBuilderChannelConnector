using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	public class DirectlineToken
	{
		public static TimeSpan TokenExpirationTime = TimeSpan.FromSeconds(1800);

		public DirectlineToken()
		{
			Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace(" ", "");
			DateIssued = DateTime.UtcNow;
		}

		public string Token { get; }

		public DateTime DateIssued { get; }

		public bool IsTokeExpired => DateTime.UtcNow > DateIssued.Add(TokenExpirationTime);

		protected bool Equals(DirectlineToken other)
		{
			return string.Equals(Token, other.Token) && DateIssued.Equals(other.DateIssued);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DirectlineToken) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Token.GetHashCode() * 397) ^ DateIssued.GetHashCode();
			}
		}
	}
}
