using Butterfly.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Rooms
{
    internal interface IRoomItemData
    {
        int GetType();
        void Parse(string rawData);
        void AppendToMessage(ServerMessage message);
        string ToString();
        object GetData();
    }
}
