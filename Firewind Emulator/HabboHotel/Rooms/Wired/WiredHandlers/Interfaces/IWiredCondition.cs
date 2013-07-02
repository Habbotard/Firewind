using Firewind.HabboHotel.Rooms.Units;
namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    interface IWiredCondition : IWiredTrigger
    {
        bool AllowsExecution(RoomUnit user);
    }
}
