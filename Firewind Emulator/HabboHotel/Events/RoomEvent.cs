using System;
using System.Collections.Generic;
using Firewind.HabboHotel.GameClients;
using Firewind.Messages;
using System.Collections;
using HabboEvents;

namespace Firewind.HabboHotel.Rooms
{
    class RoomEvent
    {
        internal string Name;
        internal string Description;
        internal int Category;
        internal ArrayList Tags;
        internal string StartTime;

        internal UInt32 RoomId;

        internal RoomEvent(UInt32 RoomId, string Name, string Description, int Category, List<string> tags)
        {
            this.RoomId = RoomId;
            this.Name = Name;
            this.Description = Description;
            this.Category = Category;

            this.StartTime = DateTime.Now.ToShortTimeString();

            this.Tags = new ArrayList();

            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    this.Tags.Add(tag);
                }
            }
        }

        internal ServerMessage Serialize(GameClient Session)
        {
            ServerMessage Message = new ServerMessage(Outgoing.RoomEvent);
            Message.AppendString(Session.GetHabbo().Id + "");
            Message.AppendString(Session.GetHabbo().Username);
            Message.AppendString(RoomId + "");
            Message.AppendInt32(Category);
            Message.AppendString(Name);
            Message.AppendString(Description);
            Message.AppendString(StartTime);
            Message.AppendInt32(Tags.Count);

            foreach (string Tag in Tags.ToArray())
            {
                Message.AppendString(Tag);
            }
            return Message;
        }

        internal void SerializeTo(RoomData data, ServerMessage Message)
        {
            Message.AppendString(data.OwnerId + "");
            Message.AppendString(data.Owner);
            Message.AppendString(RoomId + "");
            Message.AppendInt32(Category);
            Message.AppendString(Name);
            Message.AppendString(Description);
            Message.AppendString(StartTime);
            Message.AppendInt32(Tags.Count);

            foreach (string Tag in Tags.ToArray())
            {
                Message.AppendString(Tag);
            }
        }
    }
}
