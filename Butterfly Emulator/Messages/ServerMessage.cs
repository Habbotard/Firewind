using System;
using System.Collections.Generic;
using System.Text;

using Butterfly.Util;
using System.IO;
using Butterfly.Core;
using HabboEncryption;

namespace Butterfly.Messages
{
    internal class ServerMessage
    {
        private List<byte> Message = new List<byte>();
        private int MessageId = 0;

        public int Id
        {
            get
            {
                return MessageId;
            }
        }

        public ServerMessage()
        {
        }

        public ServerMessage(int Header)
        {
            Init(Header);
        }

        public void Init(int Header)
        {
            Message = new List<byte>();
            this.MessageId = Header;
            AppendShort(Header);
        }

        public void setInt(int i, int startOn)
        {
            try
            {
                List<byte> n = new List<byte>();
                n = Message;
                List<byte> intvalue = AppendBytesTo(BitConverter.GetBytes(i), true);
                n.RemoveRange(startOn, intvalue.Count);
                n.InsertRange(startOn, intvalue);
                Message = n;
            }
            catch (Exception e)
            {
                Logging.WriteLine("Error on setInt: " + e.ToString());
            }
        }

        public void AppendByte(byte b)
        {
            // do nothing...
        }

        public void AppendShort(int i)
        {
            Int16 s = (Int16)i;
            AppendBytes(BitConverter.GetBytes(s), true);
        }

        public void AppendInt32(int i)
        {
            AppendBytes(BitConverter.GetBytes(i), true);
        }

        public void AppendUInt(uint i)
        {
            AppendInt32((int)i);
        }

        public void AppendRawInt32(int i)
        {
            AppendString(i.ToString());
        }

        public void AppendRawUInt(uint i)
        {
            AppendRawInt32((int)i);
        }

        public void AppendStringWithBreak(String s)
        {
            AppendString(s);
        }

        public void AppendStringWithBreak(String s, string u)
        {
            AppendString(s);
        }

        public void AppendStringWithBreak(String s, byte BreakChar)
        {
            AppendString(s);
        }

        public void AppendBoolean(bool b)
        {
            AppendBytes(new byte[] { (byte)(b ? 1 : 0) }, false);
        }

        public void AppendString(string s)
        {
            AppendShort(s.Length);
            AppendBytes(ButterflyEnvironment.GetDefaultEncoding().GetBytes(s), false);
        }

        public void AppendBytes(byte[] b, bool IsInt)
        {
            if (IsInt)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                    Message.Add(b[i]);
            }
            else
                Message.AddRange(b);
        }

        public List<byte> AppendBytesTo(byte[] b, bool IsInt)
        {
            List<byte> message = new List<byte>();
            if (IsInt)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                    message.Add(b[i]);
            }
            else
                message.AddRange(b);
            return message;
        }

        public byte[] GetBytes()
        {
            List<byte> Final = new List<byte>();
            Final.AddRange(BitConverter.GetBytes(Message.Count)); // packet len
            Final.Reverse();
            Final.AddRange(Message); // Add Packet
            return Final.ToArray();
        }

        public override string ToString()
        {
            return (ButterflyEnvironment.GetDefaultEncoding().GetString(GetBytes()));
        }


        /*private UInt32 MessageId;

        internal UInt32 Id
        {
            get
            {
                return MessageId;
            }
        }

        private List<byte> Body;

        internal string Header
        {
            get
            {
                return ButterflyEnvironment.GetDefaultEncoding().GetString(Base64Encoding.Encodeuint(MessageId, 2));
            }
        }


        internal int Length
        {
            get
            {
                return (int)Body.Count;
            }
        }

        internal ServerMessage() { }

        internal ServerMessage(uint _MessageId)
        {
            Init(_MessageId);
        }

        public override string ToString()
        {
            return Header + ButterflyEnvironment.GetDefaultEncoding().GetString(Body.ToArray());
        }

        //internal string ToBodyString()
        //{
        //    return ButterflyEnvironment.GetDefaultEncoding().GetString(Body.ToArray());
        //}

        //internal void Clear()
        //{
        //    Body.Clear();
        //}

        internal void Init(UInt32 _MessageId)
        {
            MessageId = _MessageId;
            Body = new List<byte>();
        }

        internal void AppendByte(byte b)
        {
            Body.Add(b);
        }

        internal void AppendBytes(byte[] Data)
        {
            if (Data == null || Data.Length == 0)
                return;

            Body.AddRange(Data);
        }

        internal void AppendString(string s, Encoding encoding)
        {
            if (s == null || s.Length == 0)
            {
                return;
            }
            AppendInt16(s.Length);
            AppendBytes(encoding.GetBytes(s));
        }

        internal void AppendString(string s)
        {
            AppendString(s, ButterflyEnvironment.GetDefaultEncoding());
        }

        internal void AppendStringWithBreak(string s)
        {
            AppendStringWithBreak(s, 2);
        }

        internal void AppendStringWithBreak(string s, string u)
        {
            AppendStringWithBreak(s, 2);
        }

        internal void AppendStringWithBreak(string s, byte BreakChar)
        {
            AppendString(s);
            AppendByte(BreakChar);
        }

        internal void AppendInt32(Int32 i)
        {
            AppendBytes(Encoding.Default.GetBytes(HabboEncoding.EncodeInt32(i).ToString()));
        }

        internal void AppendInt16(Int32 i)
        {
            AppendBytes(Encoding.Default.GetBytes(HabboEncoding.EncodeInt16(i).ToString()));
        }

        internal void AppendRawInt32(Int32 i)
        {
            AppendString(i.ToString(), Encoding.ASCII);
        }

        internal void AppendUInt(uint i)
        {
            AppendInt32((int)i);
        }

        internal void AppendRawUInt(uint i)
        {
            AppendRawInt32((int)i);
        }

        internal void AppendBoolean(Boolean Bool)
        {
            Body.Add(Bool ? (byte)1 : (byte)0);
        }

        internal byte[] GetBytes()
        {
            byte[] Data = new byte[Length + 2];
            byte[] Len = Encoding.Default.GetBytes(HabboEncoding.EncodeInt32((int)Length + 2));
            byte[] Header = Encoding.Default.GetBytes(HabboEncoding.EncodeInt16((int)MessageId));
            byte[] realData = Body.ToArray();
            Data[0] = Len[0];
            Data[1] = Len[1];
            Data[2] = Len[2];
            Data[3] = Len[3];
            Data[4] = Header[4];
            Data[5] = Header[5];

            for (int i = 0; i < Length; i++)
            {
                Data[i + 5] = realData[i];
            }

            return Data;
        }*/
    }
}
