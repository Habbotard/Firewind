using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Rooms;
using Firewind.Messages;
using System;

namespace Firewind.HabboHotel.Users.Messenger
{
    class MessengerBuddy
    {
        #region Fields
        private readonly int UserId;
        private readonly string mUsername;
        private readonly string mLook;
        private readonly string mMotto;
        private readonly string mLastOnline;

        private GameClient client;
        private Room currentRoom;
        #endregion

        #region Return values
        internal int Id
        {
            get
            {
                return UserId;
            }
        }
        
        internal bool IsOnline
        {
            get
            {
                return (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null && !client.GetHabbo().GetMessenger().AppearOffline);
            }
        }

        private GameClient Client
        {
            get
            {
                return client;
            }
            set
            {
                client = value;
            }
        }

        internal bool InRoom
        {
            get
            {
                return (currentRoom != null);
            }
        }
        
        internal Room CurrentRoom
        {
            get
            {
                return currentRoom;
            }
            set
            {
                currentRoom = value;
            }
        }
        #endregion

        #region Constructor
        internal MessengerBuddy(int UserId, string pUsername, string pLook, string pMotto, string pLastOnline)
        {
            this.UserId = UserId;
            this.mUsername = pUsername;
            this.mLook = pLook;
            this.mMotto = pMotto;
            this.mLastOnline = pLastOnline;
        }
        #endregion

        #region Methods
        internal void UpdateUser()
        {
            client = FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            UpdateUser(client);
        }

        internal void UpdateUser(GameClient client)
        {
            this.client = client;
            if (client != null && client.GetHabbo() != null)
                currentRoom = client.GetHabbo().CurrentRoom;
        }

        internal void Serialize(ServerMessage reply)
        {
            reply.AppendInt32(Convert.ToInt32(this.UserId));
            reply.AppendString(this.mUsername);
            reply.AppendInt32(1);
            bool isOnline = this.IsOnline;
            reply.AppendBoolean(isOnline);

            if (isOnline)
            {
                reply.AppendBoolean(this.InRoom);
            }
            else
            {
                reply.AppendBoolean(false);
            }

            reply.AppendString(this.mLook);
            reply.AppendInt32(0);
            reply.AppendString(this.mMotto);
            reply.AppendInt32(0);
            reply.AppendBoolean(false);
        }

        #endregion
    }
}
