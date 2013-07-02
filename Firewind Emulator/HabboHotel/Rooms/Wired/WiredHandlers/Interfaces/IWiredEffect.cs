using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    interface IWiredEffect
    {
        bool Handle(RoomUnit unit, Team team, RoomItem item);
        bool IsSpecial(out SpecialEffects action);
    }
}
