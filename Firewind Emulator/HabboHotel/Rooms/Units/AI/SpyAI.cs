using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units.AI
{
    class SpyAI : AIBase
    {
        private string _lastUsers;
        internal SpyAI(RoomAI unit)
            : base(unit)
        {
        }

        internal override void OnSelfEnterRoom()
        {
            _unit.Chat("Hi! Next time you enter the room I'll let you know who visited while you were away..", false);
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
            if (!_unit.GetRoom().CheckRights(user.GetClient(), true))
                return;
            if (text == "yes")
            {
            }
        }

        internal override void OnCycle()
        {
            throw new NotImplementedException();
        }

        private void AddUserToList(string name)
        {
            if (_lastUsers.Length == 0)
                _lastUsers = name;
            else
                _lastUsers += ", " + name;
        }
    }
}
