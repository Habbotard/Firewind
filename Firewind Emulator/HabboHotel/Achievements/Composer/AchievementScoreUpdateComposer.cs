using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firewind.Messages;
using HabboEvents;
namespace Firewind.HabboHotel.Achievements.Composer
{
    class AchievementScoreUpdateComposer
    {
        internal static ServerMessage Compose(int Score)
        {
            ServerMessage Message = new ServerMessage(Outgoing.AchievementPoints);
            Message.AppendInt32(Score);
            return Message;
        }
    }
}
