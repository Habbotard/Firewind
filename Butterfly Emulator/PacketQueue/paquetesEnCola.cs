using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Butterfly.HabboHotel.GameClients;
namespace Butterfly.PacketQueue
{
    public class paqueteEnCola
    {
        public GameClient GameClient { get; set; }
        public byte[] datos { get; set; }
    }
}
