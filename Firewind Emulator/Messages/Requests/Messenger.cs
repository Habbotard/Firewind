using System;
using System.Collections.Generic;

using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Rooms;
using Firewind.HabboHotel.Users.Messenger;
using Firewind.HabboHotel.Pathfinding;
using HabboEvents;
using Firewind.Core;

namespace Firewind.Messages
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

            if (Session.GetHabbo().GetMessenger().myFriends + Amount >= FirewindEnvironment.FriendLimit)
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
                    return;
                }

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(massRequest.To))
                {
                    Session.GetHabbo().GetMessenger().CreateFriendship(massRequest.From);
                }

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
        }

        internal void DeclineFriend()
        {
            if (this.Session.GetHabbo().GetMessenger() != null)
            {
                bool declineAll = Request.PopWiredBoolean();
                int count = Request.PopWiredInt32();

                if (declineAll)
                {
                    this.Session.GetHabbo().GetMessenger().HandleAllRequests();
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        uint sender = Request.PopWiredUInt();
                        this.Session.GetHabbo().GetMessenger().HandleRequest(sender);
                    }
                }
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
                FirewindEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.SOCIAL_FRIEND);
            }
        }

        internal void SendInstantMessenger()
        {
            if (FirewindEnvironment.SystemMute)
                return;
            //if the user we are sending an IM to is on IRC, get the IRC client / connection and send the data there instead of here. Then gtfo.
            uint userId = Request.PopWiredUInt();
            string message = FirewindEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);
        }

        internal void FollowBuddy()
        {
            uint BuddyId = Request.PopWiredUInt();

            GameClient Client = FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(BuddyId);

            if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = FirewindEnvironment.GetGame().GetRoomManager().GetRoom(Client.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            uint Id = Room.RoomId;
            string Password = "";

            RoomData Data = FirewindEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "private")
                return;

            //PrepareRoomForUser(Id, Password);


            //FG" + Encoding.encodeVL64(Core.RoomID) + "@@M
            // D^HjTX]X
            GetResponse().Init(Outgoing.FollowBuddy);
            GetResponse().AppendBoolean(Room.IsPublic);
            GetResponse().AppendUInt(Client.GetHabbo().CurrentRoomId);
            SendResponse();

            GetResponse().Init(Outgoing.GetGuestRoomResult);
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

            string message = FirewindEnvironment.FilterInjectionChars(Request.PopFixedString(), true);

            ServerMessage Message = new ServerMessage(Outgoing.InstantInvite);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendString(message);

            foreach (UInt32 Id in UserIds)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(Id))
                    continue;

                GameClient Client = FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);

                if (Client == null)
                {
                    return;
                }

                Client.SendMessage(Message);
            }
        }
    }
}
