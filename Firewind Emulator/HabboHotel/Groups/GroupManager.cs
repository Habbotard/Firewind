using Firewind.HabboHotel.Groups.Types;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Firewind.HabboHotel.Groups
{
    class GroupManager
    {
        internal Dictionary<int, Group> Groups;

        internal GroupManager(IQueryAdapter dbClient)
        {
            this.Groups = new Dictionary<int, Group>();
            LoadGroups(dbClient);
        }

        internal void LoadGroups(IQueryAdapter dbClient)
        {
            dbClient.setQuery("SELECT * FROM groups");
            DataTable GroupTable = dbClient.getTable();

            foreach (DataRow Group in GroupTable.Rows)
            {
                dbClient.setQuery("SELECT * FROM `group_memberships` WHERE `id` = @id");
                dbClient.addParameter("id", (int)Group["id"]);
                
                Group group = new Group(Group, dbClient.getTable());

            }
        }

        public Group GetGroup(int Id, out Group group)
        {
            if (this.GetGroups().TryGetValue(Id, out group))
                return group;
            return null;
        }

        public Dictionary<int, Group> GetGroups()
        {
            return this.Groups;
        }

        public Group CreateGroup(string name, string description, int roomID, int color1, int color2, List<Tuple<int, int, int>> badgeData)
        {
            int groupID;
            string badgeCode = Group.GenerateBadgeImage(badgeData);
            string createTime = DateTime.Now.ToString("d-M-yyyy");
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("INSERT INTO groups(name,description,badge,users_id,rooms_id,color1,color2,date_created) VALUES(@name,@desc,@badge,@ownerid,@roomid,@color1,@color2,@date)");
                dbClient.addParameter("name", name);
                dbClient.addParameter("desc", description);
                dbClient.addParameter("roomid", roomID);
                dbClient.addParameter("color1", color1);
                dbClient.addParameter("color2", color2);
                dbClient.addParameter("badge", badgeCode);
                dbClient.addParameter("date", createTime);
                groupID = (int)dbClient.insertQuery();
            }
            Group group = new Group()
            {
                ID = groupID,
                Name = name,
                Description = description,
                RoomID = roomID,
                Color1 = color1,
                Color2 = color2,
                BadgeCode = badgeCode,
                DateCreated = createTime,
                
            };
            return group;
        }
    }
}
