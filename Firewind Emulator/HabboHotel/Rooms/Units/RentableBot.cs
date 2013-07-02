using Firewind.HabboHotel.Rooms.Units.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class RentableBot : RoomAI
    {
        internal int OwnerID;
        internal char Gender;
        internal int TimeLeft;

        internal override int GetTypeID()
        {
            // "RENTABLE_BOT" - 4
            return 4;
        }

        public RentableBot(int virtualID, Room room, AIBase ai, int ownerID, char gender, int timeLeft)
            : base(virtualID, ai, room)
        {
            this.OwnerID = ownerID;
            this.Gender = gender;
            this.TimeLeft = timeLeft;
        }

        public RentableBot() : base()
        {

        }

        internal override void Serialize(Messages.ServerMessage Message)
        {
            base.Serialize(Message);
            Message.AppendString(Gender.ToString());
            Message.AppendInt32(TimeLeft);
        }
    }
}
