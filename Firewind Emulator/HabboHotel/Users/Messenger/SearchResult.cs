using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firewind.Messages;
using Firewind;

namespace Firewind.HabboHotel.Users.Messenger
{
    struct SearchResult
    {
        internal int userID;
        internal string username;
        internal string motto;
        internal string look;
        internal string last_online;

        public SearchResult(int userID, string username, string motto, string look, string last_online)
        {
            this.userID = userID;
            this.username = username;
            this.motto = motto;
            this.look = look;
            this.last_online = last_online;
        }

        internal void Searialize(ServerMessage reply)
        {
            reply.AppendInt32(userID);
            reply.AppendString(username);
            reply.AppendString(motto);

            bool Online = (FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(userID) != null);

            reply.AppendBoolean(Online);
          
            reply.AppendBoolean(false);

            reply.AppendString(string.Empty);
            reply.AppendInt32(0);
            reply.AppendString(look);
            reply.AppendString(last_online);
        }
    }
}
