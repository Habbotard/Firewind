using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units.AI
{
    class SpyAI : AIBase
    {
        internal SpyAI(RoomAI unit)
            : base(unit)
        {
        }

        internal override void OnSelfEnterRoom()
        {
            throw new NotImplementedException();
        }

        internal override void OnSelfLeaveRoom(bool Kicked)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserEnterRoom(RoomUser User)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserLeaveRoom(GameClients.GameClient Client)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserSay(RoomUser User, string Message)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserShout(RoomUser User, string Message)
        {
            throw new NotImplementedException();
        }

        internal override void OnCycle()
        {
            throw new NotImplementedException();
        }
    }
}
