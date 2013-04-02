namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    interface IWiredCondition : IWiredTrigger
    {
        bool AllowsExecution(RoomUser user);
    }
}
