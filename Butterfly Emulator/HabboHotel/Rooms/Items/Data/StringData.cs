using Butterfly.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Rooms
{
    // Used for most furniture
    internal class StringData : IRoomItemData
    {
        // 0 = (StringData?)          - string
        internal string Data;

        internal StringData(string data)
        {
            this.Data = data;
        }

        public int GetType()
        {
            return 0;
        }

        public void Parse(string rawData)
        {
            Data = rawData;
        }

        public void AppendToMessage(ServerMessage message)
        {
            message.AppendString(Data);
        }

        public override string ToString()
        {
            return Data;
        }

        public object GetData()
        {
            return Data;
        }
    }
}
