using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedPacketLib;
using Firewind.Util;
using Firewind.Messages;
using Firewind.Core;
using ConnectionManager;
using Firewind.Messages.ClientMessages;
using Firewind.HabboHotel.GameClients;
using HabboEncryption;

namespace Firewind.Net
{
    public class GamePacketParser : IDataParser
    {
        private ConnectionInformation con;

        public delegate void HandlePacket(ClientMessage message);
        public event HandlePacket onNewPacket;
        private GameClient currentClient;

        internal GamePacketParser(GameClient me)
        {
            currentClient = me;
        }

        public void SetConnection(ConnectionInformation con)
        {
            this.con = con;
            this.onNewPacket = null;
        }

        public void handlePacketData(byte[] data)
        {
            int pos = 0;
            while (pos < data.Length)
            {
                try
                {
                    int MessageLength = HabboEncoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++], data[pos++] });
                    if (MessageLength < 2 || MessageLength > 1024)
                    {
                        //Logging.WriteLine("bad size packet!");
                        continue;
                    }
                    int MessageId = HabboEncoding.DecodeInt16(new byte[] { data[pos++], data[pos++] });



                    byte[] Content = new byte[MessageLength - 2];

                    for (int i = 0; i < Content.Length && pos < data.Length; i++)
                    {
                        Content[i] = data[pos++];
                    }

                    //Logging.WriteLine("[REQUEST] [" + MessageId + " / len: " + MessageLength + "] => " + Encoding.Default.GetString(Content).Replace(Convert.ToChar(0).ToString(), "{char0}"));

                    if (onNewPacket != null)
                    {
                        using (ClientMessage message = ClientMessageFactory.GetClientMessage(MessageId, Content))
                        {
                            onNewPacket.Invoke(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    int MessageLength = HabboEncoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++], data[pos++] });
                    int MessageId = HabboEncoding.DecodeInt16(new byte[] { data[pos++], data[pos++] });
                    Logging.HandleException(e, "packet handling ----> " + MessageId);
                    con.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.onNewPacket = null;
        }

        public object Clone()
        {
            return new GamePacketParser(currentClient);
        }
    }
}
