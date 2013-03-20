using System;
using SharedPacketLib.Data_Format.Data_Exception;

namespace SharedPacketLib.DataPackets
{
    public abstract class AbstractIncomingPacket
    {
        
        private byte[] mBody;
        private int mPosition;

        protected short packetID;

        public int Size
        {
            get { return mBody.Length; }
        }

        public AbstractIncomingPacket(byte[] bytes)
        {
            if (bytes == null || bytes.Length < sizeof(short))
            {
                throw new PacketMalformedException("Invalid packet bytes data. Must atleast be 4 bytes.", this);
            }

            // Set body and read the opcode
            mBody = bytes;
            packetID = this.ReadShort();
            
        }

        public byte[] ReadBytes(int n)
        {
            byte[] bytes = new byte[n];
            for (int i = 0; i < n; i++)
            {
                bytes[i] = mBody[mPosition++];
            }

            return bytes;
        }

        public Boolean ReadBoolean()
        {
            this.checkIfHasNextLength(1);
            bool value = BitConverter.ToBoolean(mBody, mPosition);
            mPosition++;
            return value;
        }

        public byte ReadByte()
        {
            this.checkIfHasNextLength(1);
            return mBody[mPosition++];
        }


        public int ReadInt()
        {
            this.checkIfHasNextLength(sizeof(int));
            int value = BitConverter.ToInt32(mBody, mPosition);
            mPosition += sizeof(int);
            return value;
        }

        public uint ReadUInt()
        {
            this.checkIfHasNextLength(sizeof(uint));
            uint value = BitConverter.ToUInt32(mBody, mPosition);
            mPosition += sizeof(uint);
            return value;
        }


        public double ReadDouble()
        {
            this.checkIfHasNextLength(sizeof(double));
            double value = BitConverter.ToDouble(mBody, mPosition);
            mPosition += sizeof(double);
            return value;
        }

        public String ReadString()
        {
            // Read string length
            short length = this.ReadShort();

            // Use UTF8 to decode the string
            this.checkIfHasNextLength(length);
            string str = System.Text.Encoding.UTF8.GetString(mBody, mPosition, length);
            mPosition += length;

            return str;
        }
        public short ReadShort()
        {
            this.checkIfHasNextLength(sizeof(short));
            short value = BitConverter.ToInt16(mBody, mPosition);
            mPosition += sizeof(short);
            return value;
        }

        public long ReadLong()
        {
            this.checkIfHasNextLength(sizeof(long));
            long value = BitConverter.ToInt64(mBody, mPosition);
            mPosition += sizeof(long);
            return value;

        }


        /// <summary>
        /// checks if this packet has an X amount of bytes left
        /// </summary>
        /// <param name="length"></param>
        private void checkIfHasNextLength(int length)
        {
            if ((mPosition + length) > this.Size)
                throw new PacketMalformedException("Packet was too short for message", this);
        }
    }
}
