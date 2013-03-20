using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Groups.Types
{
    class GroupMember
    {
        public uint GroupId { get; set; }
        public uint UserId { get; set; }
        public int Rank { get; set; }
        public bool IsPending { get; set; }

        public GroupMember(DataRow Data)
        {
            this.GroupId = (uint)Data["group_id"];
            this.UserId = (uint)Data["user_id"];
            this.Rank = (int)Data["rank"];
            this.IsPending = (((string)Data["is_pending"] == "1") ? true : false);
        }
    }
}
