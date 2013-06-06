using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers
{
    struct UnitWalksFurniValue
    {
        internal readonly RoomUnit unit;
        internal readonly RoomItem item;

        public UnitWalksFurniValue(RoomUnit unit, RoomItem item)
        {
            this.unit = unit;
            this.item = item;
        }
    }
}
