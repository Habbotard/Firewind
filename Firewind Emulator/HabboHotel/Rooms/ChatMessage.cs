using Firewind.HabboHotel.Rooms.Units;
namespace Firewind.HabboHotel.Rooms
{
    struct InvokedChatMessage
    {
        internal RoomUnit unit;
        internal string message;
        internal bool shout;

        public InvokedChatMessage(RoomUnit unit, string message, bool shout)
        {
            this.unit = unit;
            this.message = message;
            this.shout = shout;
        }

        internal void Dispose()
        {
            unit = null;
            message = null;
        }
    }
}
