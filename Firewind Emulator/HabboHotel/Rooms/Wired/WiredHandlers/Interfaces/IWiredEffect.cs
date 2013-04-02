using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Rooms.Games;

namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    interface IWiredEffect
    {
        bool Handle(RoomUser user, Team team, RoomItem item);
        bool IsSpecial(out SpecialEffects action);
    }
}
