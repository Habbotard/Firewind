using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units.AI
{
    class GenericAI : AIBase
    {
        internal GenericAI(RoomAI unit)
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

        internal override void OnUserChat(RoomUser user, string text, bool shout)
        {

        }

        internal override void OnCycle()
        {
            throw new NotImplementedException();
        }
    }
}
