using System;
using System.Collections.Generic;

using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users.Messenger;
using Butterfly.HabboHotel.Pathfinding;
using HabboEvents;
using Butterfly.Core;

namespace Butterfly.Messages
{
    partial class GameClientMessageHandler
    {
        internal void InitMessenger()
        {
            Session.GetHabbo().InitMessenger();
        }

        internal void EnterInquiredRoom()
        {
            // ???????????????????????????????
            // I like cake :D
        }

        internal void FriendsListUpdate()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            //Session.SendMessage(Session.GetHabbo().GetMessenger().SerializeUpdates());
        }

        internal void RemoveBuddy()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Requests = Request.PopWiredInt32();

            for (int i = 0; i < Requests; i++)
            {
                Session.GetHabbo().GetMessenger().DestroyFriendship(Request.PopWiredUInt());
            }
        }

        internal void SearchHabbo()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.SendMessage(Session.GetHabbo().GetMessenger().PerformSearch(Request.PopFixedString()));
        }

        internal void AcceptRequest()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Amount = Request.PopWiredInt32();

            if (Session.GetHabbo().GetMessenger().myFriends + Amount >= ButterflyEnvironment.FriendLimit)
            {
                Session.SendNotif(LanguageLocale.GetValue("friends.full"));
                return;
            }

            for (int i = 0; i < Amount; i++)
            {
                uint RequestId = Request.PopWiredUInt();

                MessengerRequest massRequest = Session.GetHabbo().GetMessenger().GetRequest(RequestId);

                if (massRequest == null)
                {
                    continue;
                }

                if (massRequest.To != Session.GetHabbo().Id)
                {
                    // not this user's request. filthy haxxor!
                    return;
                }

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(massRequest.To))
                {
                    Session.GetHabbo().GetMessenger().CreateFriendship(massRequest.From);
                }

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
        }

        internal void DeclineRequest()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            // Remove all = @f I H
            // Remove specific = @f H I <reqid>

            int Mode = Request.PopWiredInt32();
            //int Amount = Request.PopWiredInt32();

            if (Mode == 0)
            {
                uint RequestId = Request.PopWiredUInt();

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
            else
            {
                Session.GetHabbo().GetMessenger().HandleAllRequests();
            }
        }

        internal void RequestBuddy()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            if (Session.GetHabbo().GetMessenger().RequestBuddy(Request.PopFixedString()))
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.SOCIAL_FRIEND);
            }
        }

        internal void SendInstantMessenger()
        {
            if (ButterflyEnvironment.SystemMute)
                return;
            //if the user we are sending an IM to is on IRC, get the IRC client / connection and send the data there instead of here. Then gtfo.
            uint userId = Request.PopWiredUInt();
            string message = ButterflyEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);
        }

        internal void FollowBuddy()
        {
            uint BuddyId = Request.PopWiredUInt();

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(BuddyId);

            if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Client.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            uint Id = Room.RoomId;
            string Password = "";

            RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "private")
                return;

            //PrepareRoomForUser(Id, Password);


            //FG" + Encoding.encodeVL64(Core.RoomID) + "@@M
            // D^HjTX]X
            GetResponse().Init(Outgoing.FollowBuddy);
            GetResponse().AppendBoolean(Room.IsPublic);
            GetResponse().AppendUInt(Client.GetHabbo().CurrentRoomId);
            SendResponse();

            GetResponse().Init(Outgoing.RoomData);
            GetResponse().AppendBoolean(false);
            GetResponse().AppendUInt(Room.RoomId);
            GetResponse().AppendBoolean(false);
            GetResponse().AppendString(Room.Name);
            GetResponse().AppendBoolean(true);
            GetResponse().AppendInt32(Room.OwnerId);
            GetResponse().AppendString(Room.Owner);
            GetResponse().AppendInt32(Room.State); // room state
            GetResponse().AppendInt32(Room.UsersNow);
            GetResponse().AppendInt32(Room.UsersMax);
            GetResponse().AppendString(Room.Description);
            GetResponse().AppendInt32(0); // dunno!
            GetResponse().AppendInt32((Room.Category == 9) ? 2 : 0); // can trade!
            GetResponse().AppendInt32(Room.Score);
            GetResponse().AppendInt32(Room.Category);
            GetResponse().AppendInt32(0);
            GetResponse().AppendInt32(0);
            GetResponse().AppendString("");
            GetResponse().AppendInt32(Room.TagCount);

            foreach (string Tag in Room.Tags)
            {
                GetResponse().AppendString(Tag);
            }
            GetResponse().AppendInt32(0);
            GetResponse().AppendInt32(0);
            GetResponse().AppendInt32(0);
            GetResponse().AppendBoolean(true);
            GetResponse().AppendBoolean(true);
            GetResponse().AppendBoolean(false);
            GetResponse().AppendString("");
            SendResponse();

            PrepareRoomForUser(Client.GetHabbo().CurrentRoomId, Password);
        }

        internal void SendInstantInvite()
        {
            int count = Request.PopWiredInt32();

            List<UInt32> UserIds = new List<uint>();

            for (int i = 0; i < count; i++)
            {
                UserIds.Add(Request.PopWiredUInt());
            }

            string message = ButterflyEnvironment.FilterInjectionChars(Request.PopFixedString(), true);

            ServerMessage Message = new ServerMessage(Outgoing.InstantInvite);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendString(message);

            foreach (UInt32 Id in UserIds)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(Id))
                    continue;

                GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);

                if (Client == null)
                {
                    return;
                }

                Client.SendMessage(Message);
            }
        }
    }
}
