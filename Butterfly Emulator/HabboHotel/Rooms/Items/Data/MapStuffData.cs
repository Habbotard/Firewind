using Butterfly.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Rooms
{
    // Used for presents & mannequins
    internal class MapStuffData : IRoomItemData
    {
        // 1 = (MapStuffData)         - int i, foreach i { string, string }
        internal Dictionary<string, string> Data;

        internal MapStuffData()
        {
            Data = new Dictionary<string, string>();
        }
        public int GetTypeID()
        {
            return 1;
        }

        public void Parse(string rawData)
        {
            string[] splitted = rawData.Split(Convert.ToChar(1));

            for (int i = 0; i < splitted.Length; i++)
            {
                Data.Add(splitted[i], splitted[++i]);
            }
        }

        public void AppendToMessage(ServerMessage message)
        {
            message.AppendInt32(Data.Count);
            foreach (KeyValuePair<string, string> pair in Data)
            {
                message.AppendString(pair.Key);
                message.AppendString(pair.Value);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in Data)
            {
                builder.Append(Convert.ToChar(1));
                builder.Append(pair.Key);
                builder.Append(Convert.ToChar(1));
                builder.Append(pair.Value);
            }

            return builder.ToString().Substring(1);
        }

        public object GetData()
        {
            return Data;
        }
    }
}
