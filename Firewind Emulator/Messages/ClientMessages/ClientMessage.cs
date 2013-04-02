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

        //private static readonly string expl0it = Convert.ToChar(1).ToString();

        private void CheckForExploits(string packetdata)
        {
            /*if (packetdata.Contains(expl0it))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("PACKET EXPLOIT IN PACKET " + MessageId);
                builder.AppendLine("Packet content : " + ToString());
                Logging.LogCriticalException(builder.ToString());
            }*/
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

        internal string PopFixedString()
        {
            string data = PopFixedString(FirewindEnvironment.GetDefaultEncoding());
            //CheckForExploits(data);
            return data;
        }

        internal string PopFixedString(Encoding encoding)
        {
            string data = encoding.GetString(ReadFixedValue());
            //CheckForExploits(data);
            return data;
        }

        internal Int32 PopFixedInt32()
        {
            Int32 i = 0;

            string s = PopFixedString(Encoding.ASCII);

            Int32.TryParse(s, out i);

            return i;
        }

        internal Boolean PopWiredBoolean()
        {
            if (this.RemainingLength > 0 && Body[Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        internal Int32 PopWiredInt32()
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

        internal uint PopWiredUInt()
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

            return uint.Parse(PopWiredInt32().ToString());
        }

        public void Dispose()
        {
            ClientMessageFactory.ObjectCallback(this);
            GC.SuppressFinalize(this);
        }
    }
}
