namespace SharedPacketLib.DataPackets
{
    public class ClientOutgoingPacket : AbstractOutgoingPacket
    {
        private ServerOpCode mOpcode;

        public ClientOutgoingPacket(ServerOpCode opcode)
            : base((short)opcode)
        {
            this.mOpcode = opcode;
        }

        public ServerOpCode getOpCode()
        {
            return this.mOpcode;
        }
    }
}
