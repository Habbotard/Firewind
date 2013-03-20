using Butterfly.HabboHotel.Groups.Types;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.Groups
{
    class GroupManager
    {
        internal Dictionary<uint, Group> Groups;

        internal GroupManager(IQueryAdapter dbClient)
        {
            this.Groups = new Dictionary<uint, Group>();
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

        public Group GetGroup(uint Id, out Group group)
        {
            this.GetGroups().TryGetValue(Id, out group);
            return group;
        }

        public Dictionary<uint, Group> GetGroups()
        {
            return this.Groups;
        }
    }
}
