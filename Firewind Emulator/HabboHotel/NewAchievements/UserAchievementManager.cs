using System.Collections.Generic;
using Firewind.HabboHotel.GameClients;

namespace Firewind.HabboHotel.NewAchievements
{
    class UserAchievementManager
    {
        private Dictionary<uint, Achievement> achivements;
        private GameClient client;

        public UserAchievementManager(GameClient client, Dictionary<uint, Achievement> achievements)
        {
            this.client = client;
            this.achivements = achievements;
        }
    }
}
