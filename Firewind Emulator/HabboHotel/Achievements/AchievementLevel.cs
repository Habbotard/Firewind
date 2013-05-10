namespace Firewind.HabboHotel.Achievements
{
    struct AchievementLevel
    {
        internal readonly int Level;
        internal readonly int RewardActivityPoints;
        internal readonly byte ActivityPointType;
        internal readonly int RewardPoints;
        internal readonly int Requirement;

        public AchievementLevel(int level, int rewardActivityPoints, byte activityPointType, int rewardPoints, int requirement)
        {
            this.Level = level;
            this.RewardActivityPoints = rewardActivityPoints;
            this.ActivityPointType = activityPointType;
            this.RewardPoints = rewardPoints;
            this.Requirement = requirement;
        }
    }
}
