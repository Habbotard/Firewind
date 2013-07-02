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

            int Requests = Request.ReadInt32();


            for (int i = 0; i < Requests; i++)
            {
                Session.GetHabbo().GetMessenger().DestroyFriendship(Request.ReadInt32());
            }
        }

        internal void SearchHabbo()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.SendMessage(Session.GetHabbo().GetMessenger().PerformSearch(Request.ReadString()));
        }

        internal void AcceptRequest()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Amount = Request.ReadInt32();

            for (int i = 0; i < Amount; i++)
            {
                int RequestId = Request.ReadInt32();

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
                bool declineAll = Request.ReadBoolean();
                int count = Request.ReadInt32();

                if (declineAll)
                {
                    this.Session.GetHabbo().GetMessenger().HandleAllRequests();
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int sender = Request.ReadInt32();
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

            if (Session.GetHabbo().GetMessenger().RequestBuddy(Request.ReadString()))
            {
                FirewindEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.SOCIAL_FRIEND);
            }
        }

        internal void SendInstantMessenger()
        {
            if (FirewindEnvironment.SystemMute)
                return;
            //if the user we are sending an IM to is on IRC, get the IRC client / connection and send the data there instead of here. Then gtfo.
            int userId = Request.ReadInt32();
            string message = FirewindEnvironment.FilterInjectionChars(Request.ReadString());

            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);
        }

        internal void FollowBuddy()
        {
            int BuddyId = Request.ReadInt32();

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

            if (Data == null)
                return;

            ForwardToRoom((int)Id);

            // This will make the client send GetGuestRoom
            GetResponse().Init(Outgoing.RoomForward);
            GetResponse().AppendBoolean(false); // is public
            GetResponse().AppendUInt(Client.GetHabbo().CurrentRoomId);
            SendResponse();
        }

        internal void SendInstantInvite()
        {
            int count = Request.ReadInt32();

            List<int> UserIds = new List<int>();

            for (int i = 0; i < count; i++)
            {
                UserIds.Add(Request.ReadInt32());
            }

            string message = FirewindEnvironment.FilterInjectionChars(Request.ReadString(), true);

            ServerMessage Message = new ServerMessage(Outgoing.InstantInvite);
            Message.AppendInt32(Session.GetHabbo().Id);
            Message.AppendString(message);

            foreach (int Id in UserIds)
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
