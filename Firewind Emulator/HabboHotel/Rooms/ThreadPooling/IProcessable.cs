using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firewind.HabboHotel.Rooms.ThreadPooling
{
    interface IProcessable
    {
        void ProcessLogic();
        bool isLongRunTask();
    }
}
