﻿using System;
using Firewind.HabboHotel.Pathfinding;
using Firewind.HabboHotel.RoomBots;
using Firewind.HabboHotel.Rooms;
using Firewind.HabboHotel.Support;
using Firewind.Core;
using HabboEvents;

namespace Firewind.Messages
{
    partial class GameClientMessageHandler
    {
        internal void InitHelpTool()
        {
            Response.Init(Outgoing.OpenHelpTool);
            Response.AppendInt32(0);
            SendResponse();
        }

        internal void SubmitHelpTicket()
        {
            Boolean errorOccured = false;

            if (FirewindEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                errorOccured = true;
            }

            if (!errorOccured)
            {
                String Message = FirewindEnvironment.FilterInjectionChars(Request.PopFixedString());

                int Junk = Request.PopWiredInt32();
                int Type = Request.PopWiredInt32();
                uint ReportedUser = Request.PopWiredUInt();

                FirewindEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, Type, ReportedUser, Message);
            }

            GetResponse().Init(1693);
            GetResponse().AppendInt32(0);
            SendResponse();
        }

        internal void DeletePendingCFH()
        {
            if (!FirewindEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                return;
            }

            FirewindEnvironment.GetGame().GetModerationTool().DeletePendingTicketForUser(Session.GetHabbo().Id);

            GetResponse().Init(320);
            SendResponse();
        }

        internal void ModGetUserInfo()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();

            if (FirewindEnvironment.GetGame().GetClientManager().GetNameById(UserId) != "Unknown User")
            {
                Session.SendMessage(ModerationTool.SerializeUserInfo(UserId));
            }
            else
            {
                Session.SendNotif(LanguageLocale.GetValue("user.loadusererror"));
            }
        }

        internal void ModGetUserChatlog()
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            Session.SendMessage(ModerationTool.SerializeUserChatlog(Request.PopWiredUInt()));
        }

        internal void ModGetRoomChatlog()
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            int Junk = Request.PopWiredInt32();
            uint RoomId = Request.PopWiredUInt();

            if (FirewindEnvironment.GetGame().GetRoomManager().GetRoom(RoomId) != null)
            {
                Session.SendMessage(ModerationTool.SerializeRoomChatlog(RoomId));
            }
        }

        internal void ModGetRoomTool()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint RoomId = Request.PopWiredUInt();
            RoomData Data = FirewindEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Session.SendMessage(ModerationTool.SerializeRoomTool(Data));
        }

        internal void ModPickTicket()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Junk = Request.PopWiredInt32();
            uint TicketId = Request.PopWiredUInt();
            FirewindEnvironment.GetGame().GetModerationTool().PickTicket(Session, TicketId);
        }

        internal void ModReleaseTicket()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int amount = Request.PopWiredInt32();

            for (int i = 0; i < amount; i++)
            {
                uint TicketId = Request.PopWiredUInt();

                FirewindEnvironment.GetGame().GetModerationTool().ReleaseTicket(Session, TicketId);
            }
        }

        internal void CloseIssues()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Request.PopWiredInt32(); // result, 1 = useless, 2 = abusive, 3 = resolved
            for(int i = 0; i < Request.PopWiredInt32(); i++)
                FirewindEnvironment.GetGame().GetModerationTool().CloseTicket(Session, Request.PopWiredUInt(), Result);
        }

        internal void ModGetTicketChatlog()
        {
            
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            SupportTicket Ticket = FirewindEnvironment.GetGame().GetModerationTool().GetTicket(Request.PopWiredUInt());

            if (Ticket == null)
            {
                return;
            }

            RoomData Data = FirewindEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Ticket.RoomId);

            if (Data == null)
            {
                return;
            }

            Session.SendMessage(ModerationTool.SerializeTicketChatlog(Ticket, Data, Ticket.Timestamp));
        }

        internal void ModGetRoomVisits()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();

            Session.SendMessage(ModerationTool.SerializeRoomVisits(UserId));
        }

        internal void ModSendRoomAlert()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int One = Request.PopWiredInt32();
            int Two = Request.PopWiredInt32();
            String Message = Request.PopFixedString();

            ServerMessage Alert = new ServerMessage(Outgoing.SendNotif);
            Alert.AppendString(Message);
            Alert.AppendString("");
            Session.GetHabbo().CurrentRoom.SendMessage(Alert);
        }

        internal void ModPerformRoomAction()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }
            uint RoomId = Request.PopWiredUInt();
            Boolean ActOne = (Request.PopWiredInt32() == 1); // set room lock to doorbell
            Boolean ActTwo = (Request.PopWiredInt32() == 1); // set room to inappropiate
            Boolean ActThree = (Request.PopWiredInt32() == 1); // kick all users

            ModerationTool.PerformRoomAction(Session, RoomId, ActThree, ActOne, ActTwo);
        }

        internal void ModSendUserCaution()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();
            Logging.WriteLine("UserId: " + UserId + "; Message => " + Message);
            ModerationTool.AlertUser(Session, UserId, Message, true);
        }

        internal void ModSendUserMessage()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();

            ModerationTool.AlertUser(Session, UserId, Message, false);
        }

        internal void ModKickUser()
        {
            if (!Session.GetHabbo().HasFuse("fuse_kick"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();

            ModerationTool.KickUser(Session, UserId, Message, false);
        }

        internal void ModBanUser()
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();
            int Length = Request.PopWiredInt32() * 3600;

            ModerationTool.BanUser(Session, UserId, Length, Message);
        }

        internal void CallGuideBot()
        {
            Room Room = FirewindEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            if (Room.guideBotIsCalled)
            {
                Session.GetMessageHandler().GetResponse().Init(33);
                Session.GetMessageHandler().GetResponse().AppendInt32(4009);
                Session.GetMessageHandler().SendResponse();

                return;
            }

            if (Session.GetHabbo().CalledGuideBot)
            {
                Session.GetMessageHandler().GetResponse().Init(33);
                Session.GetMessageHandler().GetResponse().AppendInt32(4010);
                Session.GetMessageHandler().SendResponse();

                return;
            }

            RoomUser NewUser = Room.DeployBot(FirewindEnvironment.GetGame().GetBotManager().GetBot(55));
            NewUser.SetPos(Room.GetGameMap().Model.DoorX, Room.GetGameMap().Model.DoorY, Room.GetGameMap().Model.DoorZ);
            NewUser.UpdateNeeded = true;

            RoomUser RoomOwner = Room.GetRoomUserManager().GetRoomUserByHabbo(Room.Owner);

            if (RoomOwner != null)
            {
                NewUser.MoveTo(RoomOwner.Coordinate);
                NewUser.SetRot(Rotation.Calculate(NewUser.X, NewUser.Y, RoomOwner.X, RoomOwner.Y), false);
            }

            
            Session.GetHabbo().CalledGuideBot = true;
        }

        //internal void RegisterHelp()
        //{
        //    RequestHandlers.Add(416, new RequestHandler(InitHelpTool));
        //    RequestHandlers.Add(417, new RequestHandler(GetHelpCategories));
        //    RequestHandlers.Add(418, new RequestHandler(ViewHelpTopic));
        //    RequestHandlers.Add(419, new RequestHandler(SearchHelpTopics));
        //    RequestHandlers.Add(420, new RequestHandler(GetTopicsInCategory));
        //    RequestHandlers.Add(453, new RequestHandler(SubmitHelpTicket));
        //    RequestHandlers.Add(238, new RequestHandler(DeletePendingCFH));
        //    RequestHandlers.Add(440, new RequestHandler(CallGuideBot));
        //    RequestHandlers.Add(200, new RequestHandler(ModSendRoomAlert));
        //    RequestHandlers.Add(450, new RequestHandler(ModPickTicket));
        //    RequestHandlers.Add(451, new RequestHandler(ModReleaseTicket));
        //    RequestHandlers.Add(452, new RequestHandler(ModCloseTicket));
        //    RequestHandlers.Add(454, new RequestHandler(ModGetUserInfo));
        //    RequestHandlers.Add(455, new RequestHandler(ModGetUserChatlog));
        //    RequestHandlers.Add(456, new RequestHandler(ModGetRoomChatlog));
        //    RequestHandlers.Add(457, new RequestHandler(ModGetTicketChatlog));
        //    RequestHandlers.Add(458, new RequestHandler(ModGetRoomVisits));
        //    RequestHandlers.Add(459, new RequestHandler(ModGetRoomTool));
        //    RequestHandlers.Add(460, new RequestHandler(ModPerformRoomAction));
        //    RequestHandlers.Add(461, new RequestHandler(ModSendUserCaution));
        //    RequestHandlers.Add(462, new RequestHandler(ModSendUserMessage));
        //    RequestHandlers.Add(463, new RequestHandler(ModKickUser));
        //    RequestHandlers.Add(464, new RequestHandler(ModBanUser));
        //}

        //internal void UnregisterHelp()
        //{
        //    RequestHandlers.Remove(416);
        //    RequestHandlers.Remove(417);
        //    RequestHandlers.Remove(418);
        //    RequestHandlers.Remove(419);
        //    RequestHandlers.Remove(420);
        //    RequestHandlers.Remove(453);
        //    RequestHandlers.Remove(238);
        //    RequestHandlers.Remove(440);
        //    RequestHandlers.Remove(200);
        //    RequestHandlers.Remove(450);
        //    RequestHandlers.Remove(451);
        //    RequestHandlers.Remove(452);
        //    RequestHandlers.Remove(454);
        //    RequestHandlers.Remove(455);
        //    RequestHandlers.Remove(456);
        //    RequestHandlers.Remove(457);
        //    RequestHandlers.Remove(458);
        //    RequestHandlers.Remove(459);
        //    RequestHandlers.Remove(460);
        //    RequestHandlers.Remove(461);
        //    RequestHandlers.Remove(462);
        //    RequestHandlers.Remove(463);
        //    RequestHandlers.Remove(464);
        //}
    }
}
