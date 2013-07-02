using System;
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
                String Message = FirewindEnvironment.FilterInjectionChars(Request.ReadString());

                int Junk = Request.ReadInt32();
                int Type = Request.ReadInt32();
                int ReportedUser = Request.ReadInt32();

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

            int UserId = Request.ReadInt32();

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

            Session.SendMessage(ModerationTool.SerializeUserChatlog(Request.ReadInt32()));
        }

        internal void ModGetRoomChatlog()
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            int Junk = Request.ReadInt32();
            uint RoomId = Request.ReadUInt32();

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

            uint RoomId = Request.ReadUInt32();
            RoomData Data = FirewindEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Session.SendMessage(ModerationTool.SerializeRoomTool(Data));
        }

        internal void ModPickTicket()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Junk = Request.ReadInt32();
            uint TicketId = Request.ReadUInt32();
            FirewindEnvironment.GetGame().GetModerationTool().PickTicket(Session, TicketId);
        }

        internal void ModReleaseTicket()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int amount = Request.ReadInt32();

            for (int i = 0; i < amount; i++)
            {
                uint TicketId = Request.ReadUInt32();

                FirewindEnvironment.GetGame().GetModerationTool().ReleaseTicket(Session, TicketId);
            }
        }

        internal void CloseIssues()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Request.ReadInt32(); // result, 1 = useless, 2 = abusive, 3 = resolved
            for(int i = 0; i < Request.ReadInt32(); i++)
                FirewindEnvironment.GetGame().GetModerationTool().CloseTicket(Session, Request.ReadUInt32(), Result);
        }

        internal void ModGetTicketChatlog()
        {
            
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            SupportTicket Ticket = FirewindEnvironment.GetGame().GetModerationTool().GetTicket(Request.ReadUInt32());

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

            int UserId = Request.ReadInt32();

            Session.SendMessage(ModerationTool.SerializeRoomVisits(UserId));
        }

        internal void ModSendRoomAlert()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int One = Request.ReadInt32();
            int Two = Request.ReadInt32();
            String Message = Request.ReadString();

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
            uint RoomId = Request.ReadUInt32();
            Boolean ActOne = (Request.ReadInt32() == 1); // set room lock to doorbell
            Boolean ActTwo = (Request.ReadInt32() == 1); // set room to inappropiate
            Boolean ActThree = (Request.ReadInt32() == 1); // kick all users

            ModerationTool.PerformRoomAction(Session, RoomId, ActThree, ActOne, ActTwo);
        }

        internal void ModSendUserCaution()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int UserId = Request.ReadInt32();
            String Message = Request.ReadString();
            Logging.WriteLine("UserId: " + UserId + "; Message => " + Message);
            ModerationTool.AlertUser(Session, UserId, Message, true);
        }

        internal void ModSendUserMessage()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int UserId = Request.ReadInt32();
            String Message = Request.ReadString();

            ModerationTool.AlertUser(Session, UserId, Message, false);
        }

        internal void ModKickUser()
        {
            if (!Session.GetHabbo().HasFuse("fuse_kick"))
            {
                return;
            }

            int UserId = Request.ReadInt32();
            String Message = Request.ReadString();

            ModerationTool.KickUser(Session, UserId, Message, false);
        }

        internal void ModBanUser()
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
            {
                return;
            }

            int UserId = Request.ReadInt32();
            String Message = Request.ReadString();
            int Length = Request.ReadInt32() * 3600;

            ModerationTool.BanUser(Session, UserId, Length, Message);
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
