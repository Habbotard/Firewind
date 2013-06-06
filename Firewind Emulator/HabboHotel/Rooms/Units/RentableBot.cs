using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class RentableBot : RoomAI
    {
        internal override int GetTypeID()
        {
            // "RENTABLE_BOT" - 4
            return 4;
        }

                public RentableBot(int virtualID, Room room)
            : base(virtualID, room)
        {
        }
    }
}
