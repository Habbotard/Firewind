using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class Pet : RoomAI
    {
        internal int Type;
        internal int OwnerID;
        internal string OwnerName;

        internal int RarityLevel;
        internal bool HaveSaddle;
        internal bool IsMounted;

        internal override int GetTypeID()
        {
            // "PET" - 2
            return 2;
        }

        internal override void Serialize(Messages.ServerMessage Message)
        {
            base.Serialize(Message);

            Message.AppendInt32(Type); // subType
            Message.AppendInt32(OwnerID); // userid
            Message.AppendString(OwnerName); // username
            Message.AppendInt32(RarityLevel); // raritylevel

            Message.AppendBoolean(HaveSaddle);
            Message.AppendBoolean(IsMounted);

            Message.AppendBoolean(false); // can breed?
            Message.AppendBoolean(false);
            Message.AppendBoolean(false);
            Message.AppendBoolean(false); // something with breeding permissions

            Message.AppendInt32(0); // something to do with monster plants?
            Message.AppendString(""); // something to do with monster plants?
        }

        internal override void OnSelfEnterRoom()
        {
            throw new NotImplementedException();
        }

        internal override void OnSelfLeaveRoom(bool Kicked)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserEnterRoom(RoomUser User)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserLeaveRoom(GameClients.GameClient Client)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserSay(RoomUser User, string Message)
        {
            throw new NotImplementedException();
        }

        internal override void OnUserShout(RoomUser User, string Message)
        {
            throw new NotImplementedException();
        }

        internal override void OnCycle()
        {
            throw new NotImplementedException();
        }
    }
}
