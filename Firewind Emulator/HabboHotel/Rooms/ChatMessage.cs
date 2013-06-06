using Firewind.HabboHotel.Rooms.Units;
namespace Firewind.HabboHotel.Rooms
{
    struct InvokedChatMessage
    {
        internal RoomUser user;
        internal string message;
        internal bool shout;

        public InvokedChatMessage(RoomUser user, string message, bool shout)
        {
            this.user = user;
            this.message = message;
            this.shout = shout;
        }

        internal void Dispose()
        {
            user = null;
            message = null;
        }
    }
}
