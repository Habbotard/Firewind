using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedPacketLib;
using Helpers;
using SharedPacketLib.DataPackets;
using SharedPacketLib.Data_Format.Data_Exception;

namespace SocketTest
{
    class GamePacketParser : IDataParser
    {
        private byte[] buffer;
        private int currentLengthOfPacket;

        public GamePacketParser()
        {
            currentLengthOfPacket = -1;
        }

        /// <summary>
        /// Is used every time a packet arrives in this connection
        /// </summary>
        /// <param name="packet">The packet of the item</param>
        public delegate void PacketArrival(ServerIncomingPacket packet);

        /// <summary>
        /// Is triggered every time a packet arrived in this location
        /// </summary>
        public event PacketArrival packetArrival;

        public void handlePacketData(byte[] packet)
        {
            doPacketChecks(packet);
        }
        private void doPacketChecks(byte[] packet)
        {
            try
            {
                if (buffer == null)
                    buffer = packet;
                else
                {
                    byte[] back = new byte[buffer.Length + packet.Length];
                    buffer.CopyTo(back, 0);
                    packet.CopyTo(back, buffer.Length);

                    packet = null;
                    buffer = back;
                }
                while (buffer != null)
                {
                    if (currentLengthOfPacket == -1)
                    {
                        buildNewPacket();
                    }
                    else
                    {
                        buildRemainingPacket();
                    }
                }
            }
            catch (BufferUnderflowException)
            {
                Out.writeError("Not enought data received! Waiting for more before parsing", Out.logFlags.lowLogLevel);
            }
        }

        private void buildNewPacket()
        {
            if (buffer.Length < (sizeof(short) * 2))
            {
                throw new BufferUnderflowException("Packet needs to be at least " + (sizeof(short) * 2) + " in size");
            }

            //Bytes we've processed so far
            //Header tells us "we have X amount of bytes"
            byte[] integer = new byte[2];
            Array.Copy(buffer, integer, 2);

            int amountOfBytes = BitConverter.ToInt16(integer, 0);

            if ((amountOfBytes > ConnectionManager.GameSocketManagerStatics.MAX_PACKET_SIZE))
            {
                //Discard the rest...
                buffer = null;
                throw new PacketMalformedException("Packet was too long to handle", null); // Packet was longer than
            }
            byte[] byteBuffer = new byte[buffer.Length - 2];
            Array.Copy(this.buffer, 2, byteBuffer, 0, byteBuffer.Length);
            buffer = byteBuffer;
            currentLengthOfPacket = amountOfBytes;
            buildRemainingPacket();

        }

        private void buildRemainingPacket()
        {
            if (buffer.Length < this.currentLengthOfPacket) // Wait for more data...
                throw new BufferUnderflowException("Not enought data in the buffer, we need to wait for more!");


            byte[] packetData = new byte[this.currentLengthOfPacket];
            Array.Copy(buffer, packetData, packetData.Length);

            if (buffer.Length == this.currentLengthOfPacket)
                buffer = null;
            else
            {
                byte[] byteBuffer = new byte[buffer.Length - this.currentLengthOfPacket];
                Array.Copy(this.buffer, this.currentLengthOfPacket, byteBuffer, 0, byteBuffer.Length);
                buffer = byteBuffer;
            }

            currentLengthOfPacket = -1;
            // Create new packet
            ServerIncomingPacket incomingPacket = new ServerIncomingPacket(packetData);

            // use C# events to tell every-one that there was a new packet
            if (packetArrival != null)
                packetArrival.Invoke(incomingPacket);
        }

        public void Dispose()
        {
            Out.writeNotification("Disposed a GamePacketParser");
            this.packetArrival = null;
            this.buffer = null;
        }

        public object Clone()
        {
            return new GamePacketParser();
        }
    }
}
