using Butterfly.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Rooms
{
    // Used for guild furniture
    internal class StringArrayStuffData : IRoomItemData
    {
        // 2 = (StringArrayStuffData) - int i, foreach i { string }
        internal string[] Data;

        public int GetType()
        {
            return 2;
        }

        public void Parse(string rawData)
        {
            Data = rawData.Split(Convert.ToChar(1));
        }

        public void AppendToMessage(ServerMessage message)
        {
            message.AppendInt32(Data.Length);
            foreach (string value in Data)
                message.AppendString(value);
        }

        public string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in Data)
            {
                builder.Append(Convert.ToChar(1));
                builder.Append(value);
            }

            return builder.ToString().Substring(1);
        }

        public object GetData()
        {
            return Data;
        }
    }
}
