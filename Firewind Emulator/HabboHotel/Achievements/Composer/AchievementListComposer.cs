using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firewind.Messages;
using Firewind.HabboHotel.GameClients;
using HabboEvents;
namespace Firewind.HabboHotel.Achievements.Composer
{
    class AchievementListComposer
    {
        internal static ServerMessage Compose(GameClient Session, List<Achievement> Achievements)
        {
            ServerMessage Message = new ServerMessage(Outgoing.AchievementList); //436
            Message.AppendInt32(Achievements.Count);

            foreach (Achievement Achievement in Achievements)
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Achievement.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
                int TotalLevels = Achievement.Levels.Count;

                if (TargetLevel > TotalLevels)
                {
                    TargetLevel = TotalLevels;
                }

                AchievementLevel TargetLevelData = Achievement.Levels[TargetLevel];

                Message.AppendUInt(Achievement.Id);                                                           // Unknown (ID?)
                Message.AppendInt32(TargetLevel);                                                   // Target level
                Message.AppendString(Achievement.GroupName + TargetLevel);                 // Target name/desc/badge
                Message.AppendInt32(TargetLevelData.Requirement);                                   // Progress req/target        
                Message.AppendInt32(TargetLevelData.RewardPixels);                                   // Pixel reward       
                Message.AppendInt32(TargetLevelData.RewardPoints);                                  // Unknown(??)
                Message.AppendInt32(0);
                Message.AppendInt32(UserData != null ? UserData.Progress : 0);                      // Current progress
                Message.AppendBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false);  // Set 100% completed(??)
                Message.AppendString(Achievement.Category);                                // Category
                Message.AppendString(String.Empty);
                Message.AppendInt32(TotalLevels);                                                   // Total amount of levels 
                Message.AppendInt32(0);
            }
            Message.AppendBoolean(false);
            Message.AppendBoolean(false);
            return Message;
        }
    }
}
