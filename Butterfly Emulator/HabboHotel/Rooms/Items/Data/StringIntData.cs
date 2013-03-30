using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Rooms
{
    // Used for snowwar counter?
    internal class StringIntData : IRoomItemData
    {
        // 3 = (?)                    - string, int
        internal KeyValuePair<string, int> Data;

        public int GetType()
        {
            return 3;
        }

        public void Parse(string rawData)
        {
            string[] splitted = rawData.Split(Convert.ToChar(1));
            Data = new KeyValuePair<string, int>(splitted[0], int.Parse(splitted[1]));
        }

        public void AppendToMessage(Messages.ServerMessage message)
        {
            message.AppendString(Data.Key);
            message.AppendInt32(Data.Value);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Data.Key);
            builder.Append(Convert.ToChar(1));
            builder.Append(Data.Value);

            return builder.ToString();
        }

        public object GetData()
        {
            return Data;
        }
    }
}
