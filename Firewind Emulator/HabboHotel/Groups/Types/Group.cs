using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.HabboHotel.Rooms;
using Firewind.HabboHotel.Users;
using Firewind.HabboHotel.Users.UserDataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Firewind.HabboHotel.Groups.Types
{
    class Group
    {
        private Habbo _owner;
        private RoomData _room;
        private string _color1;
        private string _color2;
        private string _badgeCode;
        public bool AdminDecorate;
        
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
        public string BadgeCode
        {
            get
            {
                if (_badgeCode == null)
                    _badgeCode = GenerateBadgeImage(BadgeData);
                return _badgeCode;
            }
        }
        public int OwnerID { get; set; }
        public string OwnerName 
        {
            get
            {
                if (_owner == null)
                    _owner = FirewindEnvironment.getHabboForId(OwnerID);
                return _owner.Username;
            }
        }
        public int RoomID { get; set; }
        public string RoomName
        {
            get
            {
                if (_room == null)
                    _room = FirewindEnvironment.GetGame().GetRoomManager().GenerateRoomData((uint)RoomID);
                return _room.Name;
            }
        }


        public List<Tuple<int, int, int>> BadgeData
        {
            get;
            set;
        }
        public int ColorID1 { get; set; }
        public int ColorID2 { get; set; }

        public string Color1 
        {
            get
            {
                if (_color1 == null)
                    _color1 = GuildsPartsData.ColorBadges2.Find(t => t.Id == ColorID1).ExtraData1;
                return _color1;
            }
            set
            {
                ColorID1 = GuildsPartsData.ColorBadges2.Find(t => t.ExtraData1 == value).Id;
                _color1 = value;
            }
        }
        public string Color2
        {
            get
            {
                if (_color2 == null)
                    _color2 = GuildsPartsData.ColorBadges3.Find(t => t.Id == ColorID2).ExtraData1;
                return _color2;
            }
            set
            {
                ColorID2 = GuildsPartsData.ColorBadges3.Find(t => t.ExtraData1 == value).Id;
                _color2 = value;
            }
        }

        public List<int> PendingMembers { get; set; }
        public List<int> Members { get; set; }

        public int Type { get; set; }
        public int RightsType { get; set; }

        public Group(DataRow Data, IQueryAdapter dbClient)
        {
            this.ID = Convert.ToInt32(Data["id"]);
            this.Name = Data["name"].ToString();
            this.Description = Data["description"].ToString();
            this.DateCreated = Data["date_created"].ToString();
            this.OwnerID = Convert.ToInt32(Data["users_id"]);
            this.RoomID = Convert.ToInt32(Data["rooms_id"]);
            this.ColorID1 = Convert.ToInt32(Data["color1"]);
            this.ColorID2 = Convert.ToInt32(Data["color2"]);
            this.Type = Convert.ToInt32(Data["type"]);
            this.RightsType = Convert.ToInt32(Data["rights_type"]);

            // Parse badge data
            string[] rawData = Data["badge_data"].ToString().Split((char)1);
            List<Tuple<int, int, int>> badgeData = new List<Tuple<int,int,int>>();
            for (int i = 0; i < rawData.Length; i++)
            {
                int value1 = int.Parse(rawData[i++]);
                int value2 = int.Parse(rawData[i++]);
                int value3 = int.Parse(rawData[i]);
                badgeData.Add(new Tuple<int, int, int>(value1, value2, value3));
            }

            this.BadgeData = badgeData;
            this.Members = new List<int>();

            // Load members
            dbClient.setQuery("SELECT * FROM group_memberships WHERE groups_id = @id");
            dbClient.addParameter("id", ID);
            foreach (DataRow row in dbClient.getTable().Rows)
            {
                this.Members.Add((int)row["users_id"]);
            }
        }

        public Group()
        {
            this.Members = new List<int>();
        }

        public bool IsAdmin(int userID)
        {
            if (OwnerID == userID)
                return true;

            return false;
        }

        public static string GenerateBadgeImage(List<Tuple<int, int, int>> parts)
        {
            List<int> partList = new List<int>();
            for (int i = 1; i < parts.Count; i++)
            {
                var part = parts[i];
                partList.Add(part.Item1);
                partList.Add(part.Item2);
                partList.Add(part.Item3);
            }
            // b 22 13  s03044  s27044  s1701  s01051
            StringBuilder image = new StringBuilder();

            // First the background/base
            image.Append("b");
            image.Append(parts[0].Item1.ToString("D2")); // type
            image.Append(parts[0].Item2.ToString("D2")); // color
            image.Append(parts[0].Item3); // part count (shouldn't matter)

            for (int i = 1; i < parts.Count; i++)
            {
                var part = parts[i];
                //if (part.Item1 == 0)
                //    continue;

                // Badge part
                image.Append("s");
                image.Append((part.Item1).ToString("D2")); //-20 type (client adds 20 for some reason?)
                image.Append(part.Item2.ToString("D2")); // color
                image.Append(part.Item3); // position
            }

            return image.ToString();
        }

        public static string ConvertBadgeForDatabase(List<Tuple<int, int, int>> parts)
        {
            StringBuilder data = new StringBuilder();
            foreach (var part in parts)
            {
                data.Append((char)1);
                data.Append(part.Item1);
                data.Append((char)1);
                data.Append(part.Item2);
                data.Append((char)1);
                data.Append(part.Item3);
            }
            return data.ToString().Substring(1);
        }

        internal int GetMemberType(int userID)
        {
            if (OwnerID == userID)
                return 3;
            if (Members.Contains(userID))
                return 1;
            return 0;
        }
    }
}
