using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectionManager;
using SharedPacketLib.DataPackets;
using SharedPacketLib.Data_Format;

namespace SocketTest
{
    class ConnectionHandeler
    {
        private ConnectionManager.ConnectionInformation connection;
        private bool isConnected;
        private bool disconnecting;
        private PacketRegistery<ServerOpCode, ServerIncomingPacket> packetRegister;

        public ConnectionHandeler(ConnectionManager.ConnectionInformation connection)
        {
            this.connection = connection;
            this.packetRegister = new PacketRegistery<ServerOpCode, ServerIncomingPacket>();

            packetRegister.registerPacket(ServerOpCode.Core_client_sends_version, ServerReceivedVersion);
            connection.sendData(new SharedPacketLib.DataPackets.ServerOutgoingPacket(ClientOpCode.Core_send_version));
            this.connection.connectionChanged += connection_connectionChanged;
            (this.connection.parser as GamePacketParser).packetArrival += ConnectionHandeler_packetArrival;
        }

        private void ConnectionHandeler_packetArrival(ServerIncomingPacket packet)
        {
            PacketRegistery<ServerOpCode, ServerIncomingPacket>.handlePacket handeler = packetRegister.getHandeler(packet.Opcode);
            if (handeler != null)
                handeler.Invoke(packet);
        }

        private void connection_connectionChanged(ConnectionInformation information, ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.closed:
                    disconnect();
                    break;
            }
        }

        private void disconnect()
        {
            if (this.isConnected && !this.disconnecting)
            {
                disconnecting = true;
                this.isConnected = false;
                this.connection.connectionChanged -= connection_connectionChanged;
                this.connection.Dispose();
                
            }
        }

        private void ServerReceivedVersion(ServerIncomingPacket packet)
        {
            //Handle version packet
        }
    }
}
