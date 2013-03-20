using System;

using Butterfly.Messages;
using Butterfly.HabboHotel.Rooms;
using Database_Manager.Database.Session_Details.Interfaces;
using HabboEvents;
namespace Butterfly.HabboHotel.Support
{
    enum TicketStatus
    {
        OPEN = 0,
        PICKED = 1,
        RESOLVED = 2,
        ABUSIVE = 3,
        INVALID = 4,
        DELETED = 5
    }

    class SupportTicket
    {
        private UInt32 Id;
        internal Int32 Score;
        internal Int32 Type;

        internal TicketStatus Status;

        internal UInt32 SenderId;
        internal UInt32 ReportedId;
        internal UInt32 ModeratorId;

        internal String Message;

        internal UInt32 RoomId;
        internal String RoomName;

        internal Double Timestamp;

        private string SenderName;
        private string ReportedName;
        private string ModName;

        internal int TabId
        {
            get
            {
                if (Status == TicketStatus.OPEN)
                {
                    return 1;
                }

                if (Status == TicketStatus.PICKED)
                {
                    return 2;
                }

                if (Status == TicketStatus.ABUSIVE || Status == TicketStatus.INVALID || Status == TicketStatus.RESOLVED)
                    return 2;

                if (Status == TicketStatus.DELETED)
                    return 3;

                return 0;
            }
        }

        internal UInt32 TicketId
        {
            get
            {
                return Id;
            }
        }

        internal SupportTicket(UInt32 Id, int Score, int Type, UInt32 SenderId, UInt32 ReportedId, String Message, UInt32 RoomId, String RoomName, Double Timestamp)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            this.Status = TicketStatus.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            this.ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;

            this.SenderName = ButterflyEnvironment.GetGame().GetClientManager().GetNameById(SenderId);
            this.ReportedName = ButterflyEnvironment.GetGame().GetClientManager().GetNameById(ReportedId);
            this.ModName = ButterflyEnvironment.GetGame().GetClientManager().GetNameById(ModeratorId);
        }

        internal SupportTicket(UInt32 Id, int Score, int Type, UInt32 SenderId, UInt32 ReportedId, String Message, UInt32 RoomId, String RoomName, Double Timestamp, object senderName, object reportedName, object modName)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            this.Status = TicketStatus.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            this.ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;

            if (senderName == DBNull.Value)
                this.SenderName = string.Empty;
            else
                this.SenderName = (string)senderName;

            if (reportedName == DBNull.Value)
                this.ReportedName = string.Empty;
            else
                this.ReportedName = (string)reportedName;

            if (modName == DBNull.Value)
                this.ModName = string.Empty;
            else
                this.ModName = (string)modName;
        }

        internal void Pick(UInt32 pModeratorId, Boolean UpdateInDb)
        {
            this.Status = TicketStatus.PICKED;
            this.ModeratorId = pModeratorId;
            this.Timestamp = ButterflyEnvironment.GetUnixTimestamp();
            if (UpdateInDb)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = " + pModeratorId + ", timestamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "' WHERE id = " + Id + "");
                }
            }
        }

        internal void Close(TicketStatus NewStatus, Boolean UpdateInDb)
        {
            this.Status = NewStatus;

            if (UpdateInDb)
            {
                String dbType = "";

                switch (NewStatus)
                {
                    case TicketStatus.ABUSIVE:

                        dbType = "abusive";
                        break;

                    case TicketStatus.INVALID:

                        dbType = "invalid";
                        break;

                    case TicketStatus.RESOLVED:
                    default:

                        dbType = "resolved";
                        break;
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE moderation_tickets SET status = '" + dbType + "' WHERE id = " + Id + "");
                }
            }
        }

        internal void Release(Boolean UpdateInDb)
        {
            this.Status = TicketStatus.OPEN;

            if (UpdateInDb)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = " + Id + "");
                }
            }
        }

        internal void Delete(Boolean UpdateInDb)
        {
            this.Status = TicketStatus.DELETED;

            if (UpdateInDb)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = " + Id + "");
                }
            }
        }

        internal ServerMessage Serialize()
        {
            /*
             * ServerMessage Issue = new ServerMessage(ServerEvents.MOD_ADDISSUE);
		Issue.writeInt(C.Id); // id
        Issue.writeInt(C.State); // state 
        Issue.writeInt(1); // cat id
        Issue.writeInt(C.Category); // reported cat id
        Issue.writeInt((Environment.getIntUnixTimestamp() - C.Timestamp)); // timestamp
        Issue.writeInt(C.Priority); // priority
        Issue.writeInt(C.ReporterId); // reporter id
        Issue.writeUTF(Habbo.UsersbyId.get(C.ReporterId).UserName); // reporter name
        Issue.writeInt(C.ReportedId); // reported id
        Issue.writeUTF((Habbo.UsersbyId.containsKey(C.ReportedId)) ? Habbo.UsersbyId.get(C.ReportedId).UserName : "Usuario desconocido"); // reported name
        Issue.writeInt(0); // mod id
        Issue.writeUTF((Habbo.UsersbyId.containsKey(C.PickedBy)) ? Habbo.UsersbyId.get(C.PickedBy).UserName : "Usuario desconocido"); // mod name
        Issue.writeUTF(C.Message); // issue message
        Issue.writeInt(C.RoomId); // room id
        Room RoomData = Room.Rooms.get(C.RoomId);
        Issue.writeUTF(RoomData.Name); // room name
        Issue.writeInt(0); // room type: 0 private - 1 public
        // if private
        if(RoomEvent.GetEventForRoomId(C.RoomId)==null)
        	Issue.writeUTF("-1");
        else {
        	RoomEvent E = RoomEvent.GetEventForRoomId(C.RoomId);
        	Issue.writeUTF(E.OwnerId + "");
            Issue.writeUTF(Habbo.UsersbyId.get(E.OwnerId).UserName);
            Issue.writeUTF(E.RoomId + "");
            Issue.writeInt(E.Category);
            Issue.writeUTF(E.Title);
            Issue.writeUTF(E.Description);
            Issue.writeUTF(E.Created);
            Issue.writeInt(E.Tags.size());
            Iterator zreader = E.Tags.iterator();
            while(zreader.hasNext())
            {
            	String tag = (String)zreader.next();
            	Issue.writeUTF(tag);
            }
        }
        Issue.writeInt(C.Category); // cat of room
        Issue.writeInt(0); // not defined
             */
            ServerMessage message = new ServerMessage(Outgoing.SerializeIssue);
            message.AppendUInt(Id); // id
            message.AppendInt32(TabId); // state
            message.AppendInt32(1); // cat
            message.AppendInt32(Type); // cat id
            message.AppendInt32(11); // Fix para que empieze a contar desde 0 el ticket
            // message.AppendInt32((int)Timestamp); // -->> timestamp
            message.AppendInt32(Score); // priority
            message.AppendUInt(SenderId); // sender id
            message.AppendString(SenderName); // sender name
            message.AppendUInt(ReportedId); // reported id
            message.AppendString(ReportedName); // reported name
            message.AppendUInt((Status == TicketStatus.PICKED) ? ModeratorId : 0); // mod id
            message.AppendString(ModName); // mod name
            message.AppendString(this.Message); // issue message
            message.AppendUInt(RoomId); // roomid
            message.AppendString(RoomName);
            RoomData data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (data != null)
            {
                message.AppendInt32(data.IsPublicRoom ? 1 : 0); // is room public?
                // have event
                if (ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId) != null)
                {
                    Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
                    if (room.HasOngoingEvent)
                    {
                        room.Event.SerializeTo(data, message);
                    }
                    else
                        message.AppendString("-1"); // event
                }
                message.AppendInt32(data.Category);
                message.AppendInt32(0); // ??
                message.AppendString(""); // ??
                message.AppendInt32(0); // ??
                message.AppendString(""); // ??
            }
            else
            {
                message.AppendInt32(0); // is room public?
                message.AppendString("-1"); // event
                message.AppendInt32(0); // category
                message.AppendInt32(0); // ??
                message.AppendString(""); // ??
                message.AppendInt32(0); // ??
                message.AppendString(""); // ??
            }
            /*message.AppendBoolean(false); //retryidon'tknowshit
            message.AppendInt32(0); //same*/
            return message;
        }
    }
}
