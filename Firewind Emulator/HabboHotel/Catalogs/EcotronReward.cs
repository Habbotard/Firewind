
using Firewind.HabboHotel.Items;

namespace Firewind.HabboHotel.Catalogs
{
    class EcotronReward
    {
        //internal uint Id;
        internal uint DisplayId;
        internal uint BaseId;
        internal uint RewardLevel;

        internal EcotronReward(uint DisplayId, uint BaseId, uint RewardLevel)
        {
            //this.Id = Id;
            this.DisplayId = DisplayId;
            this.BaseId = BaseId;
            this.RewardLevel = RewardLevel;
        }

        internal Item GetBaseItem()
        {
            return FirewindEnvironment.GetGame().GetItemManager().GetItem(this.BaseId);
        }
    }
}
