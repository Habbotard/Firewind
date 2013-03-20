using SharedPacketLib.Data_Format.Data_Exception;
namespace SharedPacketLib.DataPackets
{
    public class ClientIncomingPacket : AbstractIncomingPacket
    {
        private ClientOpCode mOpcode;

        public ClientOpCode Opcode
        {
            get { return mOpcode; }
        }

        public ClientIncomingPacket(byte[] bytes)
            : base(bytes)
        {
            if (ClientOpcodeCheck.isOpCode(base.packetID))
            {
                this.mOpcode = (ClientOpCode)base.packetID;
            }
            else
            {
                throw new PacketMalformedException("Invalid packet bytes data. Must atleast be 4 bytes.", this);
            }
        }



    }
}
