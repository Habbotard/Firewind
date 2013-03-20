namespace SharedPacketLib.DataPackets
{
    public class ServerOutgoingPacket : AbstractOutgoingPacket
    {
        private ClientOpCode mOpcode;

        public ServerOutgoingPacket(ClientOpCode opcode)
            : base((short)opcode)
        {
            mOpcode = opcode;
        }

        public ClientOpCode getOpCode()
        {
            return this.mOpcode;
        }
    }
}