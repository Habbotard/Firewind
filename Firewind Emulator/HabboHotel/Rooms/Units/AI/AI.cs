using Firewind.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units.AI
{
    abstract class AIBase
    {
        internal abstract void OnSelfEnterRoom();
        internal abstract void OnSelfLeaveRoom(bool Kicked);
        internal abstract void OnUserEnterRoom(RoomUser User);
        internal abstract void OnUserLeaveRoom(GameClient Client);
        internal abstract void OnUserSay(RoomUser User, string Message);
        internal abstract void OnUserShout(RoomUser User, string Message);
        internal abstract void OnCycle();
    }
}
