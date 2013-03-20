using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Butterfly.HabboHotel.GameClients;
namespace Butterfly.PacketQueue
{
    public class paqueteEnCola
    {
        public GameClient cliente { get; set; }
        public byte[] datos { get; set; }
    }

    public class paqueteAProcesar
    {
        public int messageID { get; set; }
        public byte content { get; set; }
    }
}
