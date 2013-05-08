using System;

using System.Text;
using System.Resources;
using Firewind.Util;
using Firewind.Core;
using Firewind.Messages.ClientMessages;
using HabboEncryption;


namespace Firewind.Messages
{
    public class ClientMessage : IDisposable
    {
        private int MessageId;
        private byte[] Body;
        private int Pointer;

        internal int Id
        {
            get
            {
                return MessageId;
            }
        }

        internal int RemainingLength
        {
            get
            {
                return Body.Length - Pointer;
            }
        }

        internal int Header
        {
            get
            {
                return MessageId;
            }
        }

        internal ClientMessage(int messageID, byte[] body)
        {
            Init(messageID, body);
        }

        internal void Init(int messageID, byte[] body)
        {
            if (body == null)
                body = new byte[0];

            MessageId = messageID;
            Body = body;

            Pointer = 0;
        }

        public override string ToString()
        {
            string text = (FirewindEnvironment.GetDefaultEncoding().GetString(Body));
            for (int i = 0; i <= 32; i++)
            {
                text = text.Replace(Convert.ToChar(i).ToString(), string.Format("[{0}]", i));
            }
            return string.Format("[{0}] BODY: \"{1}\"", Header, text);
        }

        internal void AdvancePointer(int i)
        {
            Pointer += i*4;
        }

        internal byte[] ReadBytes(int Bytes)
        {
            if (Bytes > RemainingLength)
                Bytes = RemainingLength;

            byte[] data = new byte[Bytes];

            for (int i = 0; i < Bytes; i++)
                data[i] = Body[Pointer++];

            return data;
        }

        internal byte[] PlainReadBytes(int Bytes)
        {
            if (Bytes > RemainingLength)
                Bytes = RemainingLength;

            byte[] data = new byte[Bytes];

            for (int x = 0, y = Pointer; x < Bytes; x++, y++)
            {
                data[x] = Body[y];
            }

            return data;
        }

        internal byte[] ReadFixedValue()
        {
            int len = HabboEncoding.DecodeInt16(ReadBytes(2));
            return ReadBytes(len);
        }

        internal string ReadString()
        {
            string data = ReadString(FirewindEnvironment.GetDefaultEncoding());
            //CheckForExploits(data);
            return data;
        }

        internal string ReadString(Encoding encoding)
        {
            string data = encoding.GetString(ReadFixedValue());
            //CheckForExploits(data);
            return data;
        }

        internal Boolean ReadBoolean()
        {
            if (this.RemainingLength > 0 && Body[Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        internal Int32 ReadInt32()
        {
            if (RemainingLength < 1)
            {
                return 0;
            }

            byte[] Data = PlainReadBytes(4);

            Int32 i = HabboEncoding.DecodeInt32(Data);

            Pointer += 4;

            return i;
        }

        internal uint ReadUInt32()
        {
            //int i = PopWiredInt32();

            //try
            //{
            //    return (uint)i;
            //}
            //catch (Exception e)
            //{
            //    Logging.LogException("OVerflow: I: " + i);
            //    throw e;
            //}

            return uint.Parse(ReadInt32().ToString());
        }

        public void Dispose()
        {
            ClientMessageFactory.ObjectCallback(this);
            GC.SuppressFinalize(this);
        }
    }
}
