using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Rooms.Units.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    abstract class RoomAI : RoomUnit
    {
        public AIBase BaseAI;

        public RoomAI(int virtualID, AIBase ai, Room room)
            : base(virtualID, room)
        {
            this.BaseAI = ai;
        }

        public RoomAI() : base()
        {
        }

        internal override void OnCycle()
        {
            base.OnCycle();
            BaseAI.OnCycle();
        }
    }
}
