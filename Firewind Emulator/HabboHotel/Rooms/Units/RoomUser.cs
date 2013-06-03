using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class RoomUser : RoomUnit
    {
        internal GameClient Client;

        internal override int GetTypeID()
        {
            // "USER" - 1
            return 1;
        }

        internal override void Serialize(Messages.ServerMessage Message)
        {
            base.Serialize(Message);

            Habbo User = Client.GetHabbo();
            Message.AppendString(User.Gender.ToLower());

            Message.AppendInt32(0); // group ID
            Message.AppendInt32(0); // Looks like unused
            Message.AppendString(""); // groupName

            Message.AppendString(""); // botFigure
            Message.AppendInt32(User.AchievementPoints);
        }
    }
}
