using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedPacketLib;
using Butterfly.Util;
using Butterfly.Messages;
using Butterfly.Core;
using ConnectionManager;
using Butterfly.Messages.ClientMessages;
using Butterfly.HabboHotel.GameClients;
using HabboEncryption;

namespace Butterfly.Net
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


        public void handlePacketDataDecrypted(byte[] data)
        {
            try
            {
                int pos = 0;
                // Si se trata de un paquete encriptado, poner en cola.
                if (currentClient != null)
                {
                    // Si no se trata de un paquete encriptado...
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

                            if (MessageId == 1615)
                            {
                                return;
                            }
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
                            //con.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void handlePacketData(byte[] data)
        {
            int pos = 0;
            //bool alreadyDecrypted = false;
            /**
            // Si se trata de un paquete encriptado, poner en cola.
            if (currentClient != null && currentClient.CryptoInitialized)
            {
                /*
                PacketQueue.paqueteEnCola rc4ToAdd = new PacketQueue.paqueteEnCola();
                rc4ToAdd.GameClient = currentClient;
                rc4ToAdd.datos = data;
                switch (currentClient.DesignedHandler)
                {
                    case 1:
                        {
                            //lock (GameClientManager.RC4ToDecrypt_1)
                                GameClientManager.RC4ToDecrypt_1.Enqueue(rc4ToAdd);
                            //Logging.WriteLine("Suerte 1");
                            break;
                        }

                    case 2:
                        {
                            //lock (GameClientManager.RC4ToDecrypt_2)
                                GameClientManager.RC4ToDecrypt_2.Enqueue(rc4ToAdd);
                            //Logging.WriteLine("Suerte 2");
                            break;
                        }

                    case 3:
                        {
                            //lock (GameClientManager.RC4ToDecrypt_3)
                                GameClientManager.RC4ToDecrypt_3.Enqueue(rc4ToAdd);
                            //Logging.WriteLine("Suerte 3");
                            break;
                        }

                    case 4:
                        {
                            lock (GameClientManager.RC4ToDecrypt_4)
                                GameClientManager.RC4ToDecrypt_4.Enqueue(rc4ToAdd);
                            //Logging.WriteLine("Suerte 4");
                            break;
                        }

                    case 5:
                        {
                            lock (GameClientManager.RC4ToDecrypt_5)
                                GameClientManager.RC4ToDecrypt_5.Enqueue(rc4ToAdd);
                            //Logging.WriteLine("Suerte 5");
                            break;
                        }

                }
                return;
                 * **/
                /**
                try
                {
                    data = RC4.Decipher(ref currentClient.table, ref currentClient.i, ref currentClient.j, data);
                }
                catch (Exception e)
                {
                    Logging.LogException("RC4ParsingException: " + e.ToString());
                    return;
                }
                alreadyDecrypted = true;
                
            }
        
            **/
            // Si no se trata de un paquete encriptado...
            //Logging.WriteLine("Aparente paquete:" + data.ToString());
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

                    if (MessageId == 1615)
                    {
                        return;
                    }
                    if (onNewPacket != null)
                    {
                        //if (alreadyDecrypted==true||MessageId == HabboEvents.Incoming.CheckReleaseMessageEvent || MessageId == HabboEvents.Incoming.InitCrypto || MessageId == HabboEvents.Incoming.SecretKey)
                        //{
                            using (ClientMessage message = ClientMessageFactory.GetClientMessage(MessageId, Content))
                            {
                                onNewPacket.Invoke(message);
                            }
                        //}
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
