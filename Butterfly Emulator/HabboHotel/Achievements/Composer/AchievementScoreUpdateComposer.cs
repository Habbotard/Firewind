using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Butterfly.Messages;
using HabboEvents;
namespace Butterfly.HabboHotel.Achievements.Composer
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
