using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class RentableBot : RoomAI
    {
        internal override int GetTypeID()
        {
            // "RENTABLE_BOT" - 4
            return 4;
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
