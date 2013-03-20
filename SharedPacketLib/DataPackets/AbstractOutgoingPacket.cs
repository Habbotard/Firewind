using System;
using System.IO;
using Helper;

namespace SharedPacketLib.DataPackets
{
    public abstract class AbstractOutgoingPacket
    {
        private MemoryStream mStream;
        private byte[] sendPacket;
        public short Opcode
        {
            get;
            private set;
        }

        public AbstractOutgoingPacket(short opcode)
        {   
            mStream = new MemoryStream(64);
            this.WriteShort(opcode);

        }
       
        public byte[] GetPacketData()
        {
            
            // Add length header infront of data bytes (data bytes includes opcode bytes)
            if (sendPacket == null)
            {
                byte[] dataBytes = mStream.ToArray();
                byte[] headerBytes = BitConverter.GetBytes((short)dataBytes.Length);
                byte[] packetBytes = ArrayHelper.combineArrays(headerBytes, dataBytes);
                this.sendPacket = packetBytes;
            }

            
            // This is the data to send
            return this.sendPacket;
        }

        public void WriteBytes(byte[] value)
        {
            mStream.Write(value, 0, value.Length);
        }

        public void WriteByte(Byte value)
        {
            mStream.WriteByte(value);
        }
        public void WriteShort(short value)
        {
            mStream.Write(BitConverter.GetBytes(value), 0 , sizeof(short));
        }
        public void WriteBoolean(Boolean value)
        {
            mStream.Write(BitConverter.GetBytes(value), 0, sizeof(Boolean));
        }

        public void WriteInt(int value)
        {
            mStream.Write(BitConverter.GetBytes(value), 0, sizeof(int));
        }
        public void WriteInt(uint value)
        {
            mStream.Write(BitConverter.GetBytes(value), 0, sizeof(uint));
        }

        public void WriteLong(long value)
        { 
            mStream.Write(BitConverter.GetBytes(value), 0, sizeof(long));
        }

        public void WriteDouble(double value)
        {
            mStream.Write(BitConverter.GetBytes(value), 0, sizeof(double));
        }

        public void WriteString(String value)
        {
            // Write string length
            this.WriteShort((short)value.Length);

            // Write string characters
            mStream.Write(System.Text.Encoding.UTF8.GetBytes(value), 0, value.Length);
        }
    }
}
