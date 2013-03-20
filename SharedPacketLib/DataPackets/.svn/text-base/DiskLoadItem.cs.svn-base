using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SharedPacketLib.DataPackets
{
    public class DiskLoadItem : AbstractIncomingPacket
    {
        public DiskLoadItem(string fileName) : base(File.ReadAllBytes(fileName))
        {
            base.ReadShort();
        }
    }
}
