using System;
using System.Text;
using SharedPacketLib.DataPackets;

namespace SharedPacketLib.Data_Format.Data_Exception
{
    public abstract class SendableException : Exception, ISendable
    {
        public SendableException()
            : base()
        { }
        public SendableException(string message)
            : base(message)
        { }

        #region ISendable Members

        public abstract AbstractOutgoingPacket getPacket();


        #endregion
    }
}
