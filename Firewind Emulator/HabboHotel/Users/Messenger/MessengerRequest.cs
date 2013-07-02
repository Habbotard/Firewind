using System;

using Firewind.Messages;
using Firewind.HabboHotel.GameClients;

namespace Firewind.HabboHotel.Users.Messenger
{
    class MessengerRequest
    {
        //private UInt32 xRequestId;

        private int ToUser;
        private int FromUser;
        private string mUsername;

        //internal UInt32 RequestId
        //{
        //    get
        //    {
        //        return FromUser;
        //    }
        //}

        internal int To
        {
            get
            {
                return ToUser;
            }
        }

        internal int From
        {
            get
            {
                return FromUser;
            }
        }

        //internal string SenderUsername
        //{
        //    get
        //    {
        //        return mUsername;
        //    }
        //}

        internal MessengerRequest(int ToUser, int FromUser, string pUsername)
        {
            //this.xRequestId = RequestId;
            this.ToUser = ToUser;  //Me
            this.FromUser = FromUser; //N00b
            this.mUsername = pUsername; //N00bs name
        }

        internal void Serialize(ServerMessage Request)
        {
            // BDhqu@UMeth0d1322033860

            Request.AppendInt32(FromUser);
            Request.AppendString(mUsername);
            Habbo user = FirewindEnvironment.getHabboForName(mUsername);
            Request.AppendString((user != null) ? user.Look : "");
        }
    }
}
