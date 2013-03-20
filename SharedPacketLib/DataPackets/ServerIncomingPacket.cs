using SharedPacketLib.Data_Format.Data_Exception;

namespace SharedPacketLib.DataPackets
{
    public class ServerIncomingPacket : AbstractIncomingPacket
    {
        private ServerOpCode mOpcode;

        public ServerOpCode Opcode
        {
            get { return mOpcode; }
        }

        public ServerIncomingPacket(byte[] bytes) : base(bytes)
        {
            if (ServerOpcodeCheck.isOpCode(base.packetID))
            {
                this.mOpcode = (ServerOpCode)base.packetID;
            }
            else
            {
                throw new PacketMalformedException("Was not an serveropcode! [" + base.packetID +"].", this);
            }
            
        }

       

    }
}
