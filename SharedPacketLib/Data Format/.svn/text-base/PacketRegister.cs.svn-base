using System;
using System.Collections.Generic;
using System.Text;
using SharedPacketLib.DataPackets;


namespace SharedPacketLib.Data_Format
{
    public class PacketRegistery<E, Z>
    {

        /// <summary>
        /// Basic handeler of the packet registery
        /// </summary>
        /// <param name="packet"></param>
        private delegate void Handeler(Z packet);
        /// <summary>
        /// Array containing information about packet details;
        /// </summary>
        private Dictionary<E, handlePacket> packetRegister;

        /// <summary>
        /// The main delegate to handle packets
        /// </summary>
        /// <param name="packet"></param>
        public delegate void handlePacket(Z packet);
        /// <summary>
        /// creates a new packet register
        /// </summary>
        public PacketRegistery()
        {
            packetRegister = new Dictionary<E, handlePacket>();
        }

        /// <summary>
        /// registers a packet
        /// </summary>
        /// <param name="opCode">Server opcode</param>
        /// <param name="handeler">Handeler</param>
        public void registerPacket(E opCode, handlePacket handeler)
        {
            if (packetRegister.ContainsKey(opCode))
            {
                packetRegister.Remove(opCode);
            }
            packetRegister.Add(opCode, handeler);
        }


        /// <summary>
        /// Gets a packet handeler for the given item
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public handlePacket getHandeler(E code)
        {
            if (packetRegister.ContainsKey(code))
                return packetRegister[code];
            return null;
        }

        /// <summary>
        /// Removes a packet from the registery
        /// </summary>
        /// <param name="code">The server opcode which is no longer valid</param>
        public void removePacket(E code)
        {
            if (packetRegister.ContainsKey(code))
                packetRegister.Remove(code);
        }

        /// <summary>
        /// Clears the entire packet register
        /// </summary>
        public void clear()
        {
            this.packetRegister.Clear();
        }
    }

}
