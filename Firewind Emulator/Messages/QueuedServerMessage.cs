﻿using System;
using Firewind.Messages;
using Firewind.Net;
using ConnectionManager;
using System.IO;
using System.Collections.Generic;

namespace Firewind.Messages
{
    public class QueuedServerMessage
    {
        private List<byte> packet;
        private ConnectionInformation userConnection;

        internal byte[] getPacket
        {
            get
            {
                return packet.ToArray();
            }
        }

        public QueuedServerMessage(ConnectionInformation connection)
        {
            this.userConnection = connection;
            this.packet = new List<byte>();
        }

        internal void Dispose()
        {
            packet.Clear();
            userConnection = null;
        }

        private void appendBytes(byte[] bytes)
        {
            packet.AddRange(bytes);
        }

        internal void appendResponse(ServerMessage message)
        {
            appendBytes(message.GetBytes());
        }

        internal void addBytes(byte[] bytes)
        {
            appendBytes(bytes);
        }

        internal void sendResponse()
        {
            userConnection.SendMuchData(packet.ToArray());
            Dispose();
        }
    }
}
