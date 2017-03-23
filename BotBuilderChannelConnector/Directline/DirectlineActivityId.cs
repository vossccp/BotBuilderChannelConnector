using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class DirectlineActivityId
    {
        public static DirectlineActivityId Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FormatException("Activity Id may not be empty");
            }
            var elements = str.Split('|');
            if (elements.Length != 2)
            {
                throw new FormatException("Malformed activity id");
            }

            var sequence = int.Parse(elements[1]);
            return new DirectlineActivityId(elements[0], sequence);
        }

        readonly string conversationId;
        readonly int sequence;

        public DirectlineActivityId(string conversationId, int sequence)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            this.conversationId = conversationId;
            this.sequence = sequence;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1:D7}", conversationId, sequence);
        }

        public override bool Equals(object obj)
        {
            var other = obj as DirectlineActivityId;
            if (other == null)
            {
                return false;
            }

            return other.sequence == sequence && other.conversationId == conversationId;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + conversationId.GetHashCode();
            hash = (hash * 7) + sequence.GetHashCode();
            return hash;
        }
    }
}
