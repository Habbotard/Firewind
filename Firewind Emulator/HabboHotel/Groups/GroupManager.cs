﻿using Firewind.HabboHotel.Groups.Types;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Users;
using System.Collections;

namespace Firewind.HabboHotel.Groups
{
    class GroupManager
    {
        internal Dictionary<int, Group> Groups;
        private Dictionary<int, int> _groupReferences;
        private Queue _groupDeleteQueue;
        private DateTime lastCycle;

        internal GroupManager(IQueryAdapter dbClient)
        {
            this.lastCycle = DateTime.Now;
            this.Groups = new Dictionary<int, Group>();
            this._groupReferences = new Dictionary<int, int>();
            this._groupDeleteQueue = new Queue();
            //LoadGroups(dbClient);

            //FirewindEnvironment.GetGame().GetClientManager().OnLoggedInClient += GroupManager_OnLoggedInClient;
        }

        //private void GroupManager_OnLoggedInClient(GameClient client)
        //{
        //    // This is called when an user sucessfully authed.
        //    Habbo user = client.GetHabbo();
        //    if (user == null || user.Groups.Count == 0)
        //        return;

        //    //client.GetConnection().connectionChanged += GroupManager_connectionChanged;

        //    foreach (int groupID in user.Groups)
        //    {
        //        if (GetGroup(groupID) == null)
        //            return;
        //        // Add one more count to reference counter
        //        if (!_groupReferences.ContainsKey(groupID))
        //            _groupReferences.Add(groupID, 1);
        //        else
        //            _groupReferences[groupID]++;
        //    }
        //}

        //void GroupManager_connectionChanged(ConnectionManager.ConnectionInformation information, ConnectionManager.ConnectionState state)
        //{
        //    // Called when user connection is changed (AKA disconnect)
        //    if (state != ConnectionManager.ConnectionState.closed)
        //        return;

        //    GameClient client = FirewindEnvironment.GetGame().GetClientManager().GetClient((uint)information.getConnectionID());
        //    Habbo user = client.GetHabbo();

        //    if (user == null || user.Groups.Count == 0)
        //        return;

        //    lock (_groupDeleteQueue.SyncRoot)
        //    {
        //        foreach (int groupID in user.Groups)
        //        {
        //            if (!_groupReferences.ContainsKey(groupID))
        //                continue;

        //            if (--_groupReferences[groupID] <= 0) // No more references, we can delete this group from cache
        //            {
        //                _groupReferences.Remove(groupID);
        //                if (!Groups.ContainsKey(groupID))
        //                    continue;
        //                _groupDeleteQueue.Enqueue(groupID);
        //            }
        //        }
        //    }
        //}

        internal void OnCycle()
        {
            if ((DateTime.Now - lastCycle).Seconds >= 10)
            {
                if (_groupDeleteQueue.Count > 0)
                {
                    lock (_groupDeleteQueue.SyncRoot)
                    {
                        while (_groupDeleteQueue.Count > 0)
                        {
                            int groupID = (int)_groupDeleteQueue.Dequeue();
                            if (!Groups.ContainsKey(groupID))
                                continue;

                            // TODO: Dispose group somehow?
                            //Group group = Groups[groupID];
                            Groups.Remove(groupID);
                        }
                    }
                }
            }
        }

        public Group GetGroup(int id)
        {
            if (Groups.ContainsKey(id))
                return Groups[id];

            if (LoadGroup(id)) // try to load it
                return Groups[id];
            return null;
        }

        internal bool LoadGroup(int id)
        {
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT * FROM groups WHERE id = @id");
                dbClient.addParameter("id", id);
                DataRow row = dbClient.getRow();

                if (row == null || row.ItemArray.Length == 0)
                    return false;

                Group group = new Group(row, dbClient);

                Groups.Add(id, group);
                return true;
            }
        }

        public Dictionary<int, Group> GetGroups()
        {
            return this.Groups;
        }

        public Group CreateGroup(GameClient creator, string name, string description, int roomID, int color1, int color2, List<Tuple<int, int, int>> badgeData)
        {
            // We call this method after doing all checks.
            int groupID;
            //string badgeCode = Group.GenerateBadgeImage(badgeData);
            string createTime = DateTime.Now.ToString("d-M-yyyy");
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                // Insert the group
                dbClient.setQuery("INSERT INTO groups(name,description,badge_data,users_id,rooms_id,color1,color2,date_created) VALUES(@name,@desc,@badge,@ownerid,@roomid,@color1,@color2,@date)");
                dbClient.addParameter("name", name);
                dbClient.addParameter("desc", description);
                dbClient.addParameter("ownerid", creator.GetHabbo().Id);
                dbClient.addParameter("roomid", roomID);
                dbClient.addParameter("color1", color1);
                dbClient.addParameter("color2", color2);
                dbClient.addParameter("badge", Group.ConvertBadgeForDatabase(badgeData));
                dbClient.addParameter("date", createTime);
                groupID = (int)dbClient.insertQuery();

                // Create membership for owner
                dbClient.setQuery("INSERT INTO group_memberships VALUES(@id,@groupid,3,1)");
                dbClient.addParameter("id", creator.GetHabbo().Id);
                dbClient.addParameter("groupid", groupID);
                dbClient.runQuery();

                // Update room
                dbClient.runFastQuery("UPDATE rooms SET groups_id = " + groupID + " WHERE id = " + roomID);
            }

            Group group = new Group()
            {
                ID = groupID,
                Name = name,
                Description = description,
                RoomID = roomID,
                ColorID1 = color1,
                ColorID2 = color2,
                BadgeData = badgeData,
                DateCreated = createTime
                
            };
            group.Members.Add(creator.GetHabbo().Id);
            string s = group.BadgeCode;

            return group;
        }

        public List<Group> GetGroups(List<int> idList)
        {
            List<Group> groups = new List<Group>();
            foreach (int id in idList)
            {
                Group g = GetGroup(id);
                if (g == null)
                    continue;
                groups.Add(g);
            }

            return groups;
        }

        public List<Group> GetMemberships(int userID)
        {
            List<Group> groups = new List<Group>();
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT groups_id FROM group_memberships WHERE users_id = @id");
                dbClient.addParameter("id", userID);

                foreach (DataRow row in dbClient.getTable().Rows)
                    groups.Add(GetGroup(Convert.ToInt32(row["groups_id"])));
            }

            return groups;
        }
    }
}
