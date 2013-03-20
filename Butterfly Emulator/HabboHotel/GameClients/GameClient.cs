﻿using System;
using System.Collections.Generic;
using Butterfly.Core;
using Butterfly.HabboHotel.Misc;
using Butterfly.HabboHotel.Pathfinding;
using Butterfly.HabboHotel.Users;
using Butterfly.HabboHotel.Users.UserDataManagement;
using Butterfly.HabboHotel.Users.Competitions;
using Butterfly.Messages;
using Butterfly.Net;
using Butterfly.Util;
using ConnectionManager;
using System.Drawing;
using HabboEncryption;
using HabboEvents;
using System.Threading;
using System.Threading.Tasks;
using Butterfly.HabboHotel.Rooms;
namespace Butterfly.HabboHotel.GameClients
{
    public class GameClient
    {
        private uint Id;

        private ConnectionInformation Connection;
        private GameClientMessageHandler MessageHandler;

        private Habbo Habbo;

        internal DateTime TimePingedReceived;
        internal DateTime TimePingSent;
        internal DateTime TimePingLastControl;
        internal DateTime bannertimmer;

        internal bool SetDoorPos;
        internal Point newDoorPos;
        internal  GamePacketParser packetParser;

        internal int DesignedHandler = 1;
        internal TaskFactory listaTareas=new TaskFactory();
        // Campos para la RC4
        internal int[] table = new int[256];
        internal int i = 0, j = 0;
        internal bool CryptoInitialized = false;

        internal uint ConnectionID
        {
            get
            {
                return Id;
            }
        }

        internal int CurrentRoomUserID;
        internal string MachineId;

        internal GameClient(uint ClientId, ConnectionInformation pConnection)
        {
            Id = ClientId;
            Connection = pConnection;
            SetDoorPos = false;
            CurrentRoomUserID = -1;
            packetParser = new GamePacketParser(this);
        }


        void SwitchParserRequest()
        {
            if (MessageHandler == null)
            {
                InitHandler();
            }
            packetParser.SetConnection(Connection);
            packetParser.onNewPacket += new GamePacketParser.HandlePacket(parser_onNewPacket);
            byte[] data = (Connection.parser as InitialPacketParser).currentData;
            Connection.parser.Dispose();
            Connection.parser = packetParser;
            Connection.parser.handlePacketData(data);
        }

        void parser_onNewPacket(ClientMessage Message)
        {
            try
            {
                MessageHandler.HandleRequest(Message);
            }
            catch (Exception e) { Logging.LogPacketException(Message.ToString(), e.ToString()); }
        }

        void PolicyRequest()
        {
            Connection.SendData(ButterflyEnvironment.GetDefaultEncoding().GetBytes(CrossdomainPolicy.GetXmlPolicy()));
        }

        internal ConnectionInformation GetConnection()
        {
            return Connection;
        }

        internal GameClientMessageHandler GetMessageHandler()
        {
            return MessageHandler;
        }

        internal bool gotTheThing = false;

        internal Habbo GetHabbo()
        {
            return Habbo;
        }

        internal void StartConnection()
        {
            if (Connection == null)
            {
                return;
            }

            TimePingedReceived = DateTime.Now;

            (Connection.parser as InitialPacketParser).PolicyRequest += new InitialPacketParser.NoParamDelegate(PolicyRequest);
            (Connection.parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(SwitchParserRequest);

            Connection.startPacketProcessing();
        }

        internal void InitHandler()
        {
            MessageHandler = new GameClientMessageHandler(this);
        }

        internal bool tryLogin(string AuthTicket)
        {
            try
            {
                string ip = GetConnection().getIp();
                byte errorCode = 0;
                UserData userData = UserDataFactory.GetUserData(AuthTicket, ip, out errorCode);
                if (errorCode == 1)
                {
                    SendNotifWithScroll(LanguageLocale.GetValue("login.invalidsso"));
                    return false;
                }
                else if (errorCode == 2)
                {
                    SendNotifWithScroll(LanguageLocale.GetValue("login.loggedin"));
                    return false;
                }


                ButterflyEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                this.Habbo = userData.user;
                userData.user.LoadData(userData);

                if (userData.user.Username == null)
                {
                    SendBanMessage("You have no username.");
                    return false;
                }
                string banReason = ButterflyEnvironment.GetGame().GetBanManager().GetBanReason(userData.user.Username, ip);
                if (!string.IsNullOrEmpty(banReason))
                {
                    SendBanMessage(banReason);
                    return false;
                }

                userData.user.Init(this, userData);

                QueuedServerMessage response = new QueuedServerMessage(Connection);

                ServerMessage UniqueId = new ServerMessage(Outgoing.UniqueID);
                UniqueId.AppendString(this.MachineId);
                response.appendResponse(UniqueId);

                ServerMessage authok = new ServerMessage(Outgoing.AuthenticationOK);
                response.appendResponse(authok);

                ServerMessage HomeRoom = new ServerMessage(Outgoing.HomeRoom);
                HomeRoom.AppendUInt(this.GetHabbo().HomeRoom); // first home
                HomeRoom.AppendUInt(this.GetHabbo().HomeRoom); // first home
                //response.appendResponse(HomeRoom);
                SendMessage(HomeRoom);

                ServerMessage FavouriteRooms = new ServerMessage(Outgoing.FavouriteRooms);
                FavouriteRooms.AppendInt32(30); // max rooms
                FavouriteRooms.AppendInt32(userData.user.FavoriteRooms.Count);
                foreach (uint Id in userData.user.FavoriteRooms.ToArray())
                {
                    FavouriteRooms.AppendUInt(Id);
                }
                response.appendResponse(FavouriteRooms);

                ServerMessage fuserights = new ServerMessage(Outgoing.Fuserights);
                if (GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip")) // VIP 
                    fuserights.AppendInt32(2);
                else if (GetHabbo().GetSubscriptionManager().HasSubscription("habbo_club")) // HC
                    fuserights.AppendInt32(1);
                else
                    fuserights.AppendInt32(0);
                fuserights.AppendUInt(this.GetHabbo().Rank);
                response.appendResponse(fuserights);

                ServerMessage minimails = new ServerMessage(Outgoing.CurrentMinimails);
                minimails.AppendInt32(2); // current minimails
                //response.appendResponse(undef);

                ServerMessage bools1 = new ServerMessage(Outgoing.AvailabilityStatus);
                bools1.AppendBoolean(true);
                bools1.AppendBoolean(false);
                response.appendResponse(bools1);

                ServerMessage bools2 = new ServerMessage(Outgoing.InfoFeedEnable);
                bools2.AppendBoolean(false);
                response.appendResponse(bools2);

                ServerMessage setRanking = new ServerMessage(Outgoing.SerializeCompetitionWinners);
                setRanking.AppendString("hlatCompetitions"); // competition type
                setRanking.AppendInt32(Ranking.getCompetitionForInfo("hlatCompetitions").Count);
                int i = 0;
                foreach (Ranking r in Ranking.getCompetitionForInfo("hlatCompetitions"))
                {
                    i++;
                    setRanking.AppendUInt(r.UserId);
                    Habbo data = ButterflyEnvironment.getHabboForId(r.UserId);
                    setRanking.AppendString((data != null) ? data.Username : "Desconocido");
                    setRanking.AppendString((data != null) ? data.Look : "sh-907-96.hd-3096-3.he-3082-91.lg-3018-81.ch-660-95.hr-9534-34");
                    setRanking.AppendInt32(i); // position
                    setRanking.AppendInt32(r.Score);
                }
                response.appendResponse(setRanking);
                response.sendResponse();


                if (ButterflyEnvironment.GetGame().GetClientManager().pixelsOnLogin > 0)
                {
                    PixelManager.GivePixels(this, ButterflyEnvironment.GetGame().GetClientManager().pixelsOnLogin);
                }

                if (ButterflyEnvironment.GetGame().GetClientManager().creditsOnLogin > 0)
                {
                    userData.user.Credits += ButterflyEnvironment.GetGame().GetClientManager().creditsOnLogin;
                    userData.user.UpdateCreditsBalance();
                }

                //Logging.WriteLine("[" + Habbo.Username + "] logged in");

                if (userData.user.HasFuse("fuse_mod"))
                {
                    this.SendMessage(ButterflyEnvironment.GetGame().GetModerationTool().SerializeTool());
                    ButterflyEnvironment.GetGame().GetModerationTool().SerializeOpenTickets(ref response, userData.userID);
                }

                if (LanguageLocale.welcomeAlertEnabled)
                {
                    this.SendMOTD(LanguageLocale.welcomeAlert);
                }


                return true;

                /*userData.user.SerializeQuests(ref response);

                List<string> Rights = ButterflyEnvironment.GetGame().GetRoleManager().GetRightsForHabbo(userData.user);

                ServerMessage appendingResponse = new ServerMessage(2);
                appendingResponse.Init(2);
                appendingResponse.AppendInt32(Rights.Count);

                foreach (string Right in Rights)
                {
                    appendingResponse.AppendString(Right);
                }

                response.appendResponse(appendingResponse);

                if (userData.user.HasFuse("fuse_mod"))
                {
                    response.appendResponse(ButterflyEnvironment.GetGame().GetModerationTool().SerializeTool());
                    ButterflyEnvironment.GetGame().GetModerationTool().SerializeOpenTickets(ref response, userData.userID);
                }

                response.appendResponse(userData.user.GetAvatarEffectsInventoryComponent().Serialize());

                appendingResponse.Init(290);
                appendingResponse.AppendBoolean(true);
                appendingResponse.AppendBoolean(false);
                response.appendResponse(appendingResponse);

                appendingResponse.Init(3);
                response.appendResponse(appendingResponse);

                appendingResponse.Init(517);
                appendingResponse.AppendBoolean(true);
                response.appendResponse(appendingResponse);

                //if (PixelManager.NeedsUpdate(this))
                //    PixelManager.GivePixels(this);

                if (ButterflyEnvironment.GetGame().GetClientManager().pixelsOnLogin > 0)
                {
                    PixelManager.GivePixels(this, ButterflyEnvironment.GetGame().GetClientManager().pixelsOnLogin);
                }

                if (ButterflyEnvironment.GetGame().GetClientManager().creditsOnLogin > 0)
                {
                    userData.user.Credits += ButterflyEnvironment.GetGame().GetClientManager().creditsOnLogin;
                    userData.user.UpdateCreditsBalance();
                }

                if (userData.user.HomeRoom > 0)
                {
                    appendingResponse.Init(455);
                    appendingResponse.AppendUInt(userData.user.HomeRoom);
                    response.appendResponse(appendingResponse);
                }

                appendingResponse.Init(458);
                appendingResponse.AppendInt32(30);
                appendingResponse.AppendInt32(userData.user.FavoriteRooms.Count);

                foreach (uint Id in userData.user.FavoriteRooms.ToArray())
                {
                    appendingResponse.AppendUInt(Id);
                }

                response.appendResponse(appendingResponse);

                if (userData.user.HasFuse("fuse_use_club_badge") && !userData.user.GetBadgeComponent().HasBadge("ACH_BasicClub1"))
                {
                    userData.user.GetBadgeComponent().GiveBadge("ACH_BasicClub1", true);
                }
                else if (!userData.user.HasFuse("fuse_use_club_badge") && userData.user.GetBadgeComponent().HasBadge("ACH_BasicClub1"))
                {
                    userData.user.GetBadgeComponent().RemoveBadge("ACH_BasicClub1");
                }


                if (!userData.user.GetBadgeComponent().HasBadge("Z63"))
                    userData.user.GetBadgeComponent().GiveBadge("Z63", true);

                appendingResponse.Init(2);
                appendingResponse.AppendInt32(0);

                if (userData.user.HasFuse("fuse_use_vip_outfits")) // VIP 
                    appendingResponse.AppendInt32(2);
                else if (userData.user.HasFuse("fuse_furni_chooser")) // HC
                    appendingResponse.AppendInt32(1);
                else
                    appendingResponse.AppendInt32(0);

                appendingResponse.AppendInt32(0);
                response.appendResponse(appendingResponse);

                appendingResponse.Init(2);
                appendingResponse.AppendInt32(Rights.Count);

                foreach (string Right in Rights)
                {
                    appendingResponse.AppendString(Right);
                }

                response.appendResponse(appendingResponse);

                if (LanguageLocale.welcomeAlertEnabled)
                {
                    ServerMessage alert = new ServerMessage(810);
                    alert.AppendUInt(1);
                    alert.AppendString(LanguageLocale.welcomeAlert);
                    response.appendResponse(alert);
                }

                response.sendResponse();
                Logging.WriteLine("[" + Habbo.Username + "] logged in");

                return true;*/
            }
            catch (UserDataNotFoundException e)
            {
                SendNotifWithScroll(LanguageLocale.GetValue("login.invalidsso") + "extra data: " + e.ToString());
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Invalid Dario bug duing user login: " + e.ToString());
                //SendNotif("Login error: " + e.ToString());
                SendNotifWithScroll("Login error: " + e.ToString());
            }
            return false;
        }
        internal void SendNotifWithScroll(string message)
        {
            ServerMessage notif = new ServerMessage(Outgoing.SendNotif);
            notif.AppendString(message);
            notif.AppendString(""); // link
            SendMessage(notif);
            /*
            ServerMessage notification = new ServerMessage(810);
            notification.AppendUInt(1);
            notification.AppendString(message);

            SendMessage(notification);*/
        }

        internal void SendBanMessage(string Message)
        {
            ServerMessage BanMessage = new ServerMessage(35);
            BanMessage.AppendStringWithBreak(LanguageLocale.GetValue("moderation.banmessage"), 13);
            BanMessage.AppendString(Message);
            GetConnection().SendData(BanMessage.GetBytes());
        }

        internal void SendNotif(string Message)
        {
            SendNotif(Message, false);
        }

        internal void SendBroadcastMessage(string Message)
        {
            ServerMessage notif = new ServerMessage(Outgoing.BroadcastMessage);
            notif.AppendString(Message);
            notif.AppendString(""); // link
            SendMessage(notif);
        }
        internal void SendMOTD(string[] messages)
        {
            ServerMessage notif = new ServerMessage(Outgoing.MOTDNotification);

            notif.AppendInt32(messages.Length);
            foreach(string message in messages)
               notif.AppendString(message);

            SendMessage(notif);
        }
        internal void SendMOTD(string message)
        {
            SendMOTD(new[] { message });
        }

        internal void SendNotif(string Message, Boolean FromHotelManager)
        {
            ServerMessage notif = new ServerMessage(Outgoing.SendNotif);
            notif.AppendString(Message);
            notif.AppendString(""); // link
            SendMessage(notif);
        }

        internal void Stop()
        {
            if (GetMessageHandler() != null)
                MessageHandler.Destroy();

            if (GetHabbo() != null)
                Habbo.OnDisconnect();
            CurrentRoomUserID = -1;

            this.MessageHandler = null;
            this.Habbo = null;
            this.Connection = null;
        }

        private bool Disconnected = false;

        internal void Disconnect()
        {
            if (GetHabbo() != null && GetHabbo().GetInventoryComponent() != null)
                GetHabbo().GetInventoryComponent().RunDBUpdate();
            if (!Disconnected)
            {
                if (Connection != null)
                    Connection.Dispose();
                Disconnected = true;
            }
        }

        internal void HandleConnectionData(ref byte[] data)
        {
            if (data[0] == 64)
            {
                int pos = 0;

                while (pos < data.Length)
                {
                    try
                    {
                        int MessageLength = Base64Encoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++] });
                        int MessageId = Base64Encoding.DecodeInt32(new byte[] { data[pos++], data[pos++] });

                        byte[] Content = new byte[MessageLength - 2];

                        for (int i = 0; i < Content.Length; i++)
                        {
                            Content[i] = data[pos++];
                        }

                        ClientMessage Message = new ClientMessage(MessageId, Content);

                        if (MessageHandler == null)
                        {
                            InitHandler(); //Never ever register the packets BEFORE you receive any data.
                        }

                        //DateTime PacketMsgStart = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        Logging.HandleException(e, "packet handling");
                        Disconnect();
                    }
                }
            }
            else
            {
                Connection.SendData(ButterflyEnvironment.GetDefaultEncoding().GetBytes(CrossdomainPolicy.GetXmlPolicy()));
            }
        }

        internal void SendMessage(ServerMessage Message)
        {
            //Logging.WriteLine("SENDED [" + Message.Id + "] => " + Message.ToString().Replace(Convert.ToChar(0).ToString(), "{char0}"));
            //if (Message == null)
            //    return;
            //if (GetConnection() == null)
            //    return;
            GetConnection().SendData(Message.GetBytes());
        }

        internal void UnsafeSendMessage(ServerMessage Message)
        {
            //Logging.WriteLine("SENDED [" + Message.Id + "] => " + Message.ToString().Replace(Convert.ToChar(0).ToString(), "{char0}"));
            if (Message == null)
                return;
            if (GetConnection() == null)
                return;
            GetConnection().SendUnsafeData(Message.GetBytes());
        }
    }
}
