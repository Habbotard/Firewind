using Firewind.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firewind.HabboHotel.Rooms
{
    internal interface IRoomItemData
    {
        int GetTypeID();
        void Parse(string rawData);
        void AppendToMessage(ServerMessage message);
        string ToString();
        object GetData();
    }
}
