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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }
        public string DateCreated { get; set; }

        public uint OwnerId { get; set; }
        public string OwnerName { get; set; }
        public uint RoomId { get; set; }

        public int ColourOne { get; set; }
        public int ColourTwo { get; set; }

        public int GuildBase { get; set; }
        public string GuildBaseColour { get; set; }
        public string GuildStates { get; set; }

        public string HtmlColourOne { get; set; }
        public string HtmlColourTwo { get; set; }

        public string Petitions { get; set; }
        public int Type { get; set; }
        public int RightsType { get; set; }

        public List<uint> Members { get; set; }

        public Group(DataRow Data, DataTable Members)
        {
            this.Id = (int)Data["id"];
            this.Name = (string)Data["name"];
            this.Description = (string)Data["description"];
            this.Badge = (string)Data["badge"];
            this.DateCreated = (string)Data["date_created"];
            this.OwnerId = (uint)Data["owner_id"];
            this.OwnerName = (string)Data["owner_name"];
            this.RoomId = (uint)Data["room_id"];
            this.ColourOne = (int)Data["colour_one"];
            this.ColourTwo = (int)Data["colour_two"];
            this.GuildBase = (int)Data["guild_base"];
            this.GuildBaseColour = (string)Data["guild_base_colour"];
            this.GuildStates = (string)Data["guild_states"];
            this.HtmlColourOne = (string)Data["html_colour_one"];
            this.HtmlColourTwo = (string)Data["html_colour_two"];
            this.Petitions = (string)Data["petitions"];
            this.Type = (int)Data["type"];
            this.RightsType = (int)Data["rights_type"];

            this.Members = new List<uint>();

            foreach (DataRow Member in Members.Rows)
            {
                this.Members.Add((uint)Member["user_id"]);
            }
        }

        public static string GenerateBadgeImage(int backgroundID, int backgroundColor, List<Tuple<int,int,int>> parts)
        {
            // b 22 13  s03044  s27044  s1701  s01051
            StringBuilder image = new StringBuilder();
            image.Append("b");
            image.Append(backgroundID.ToString("D2"));
            image.Append(backgroundColor.ToString("D2"));

            foreach (var part in parts)
            {
                if (part.Item1 == 0)
                    continue;
                image.Append("s");
                image.Append((part.Item1 - 20).ToString("D2"));
                image.Append(part.Item2.ToString("D2"));
                image.Append(part.Item3);
            }

            return image.ToString();
        }
        public static string GenerateGuildImage(int GuildBase, int GuildBaseColor, List<int> GStates)
        {
            List<int> list = GStates;
            string str = "";
            int num = 0;
            string str2 = "b";
            if (GuildBase.ToString().Length >= 2)
            {
                str2 = str2 + GuildBase;
            }
            else
            {
                str2 = str2 + "0" + GuildBase;
            }
            str = GuildBaseColor.ToString();
            if (str.Length >= 2)
            {
                str2 = str2 + str;
            }
            else if (str.Length <= 1)
            {
                str2 = str2 + "0" + str;
            }
            int num2 = 0;
            if (list[9] != 0) // 0 3 6 9
            {
                num2 = 4;
            }
            else if (list[6] != 0)
            {
                num2 = 3;
            }
            else if (list[3] != 0)
            {
                num2 = 2;
            }
            else if (list[0] != 0)
            {
                num2 = 1;
            }
            int num3 = 0;
            for (int i = 0; i < num2; i++)
            {
                str2 = str2 + "s";
                num = list[num3] - 20;
                if (num.ToString().Length >= 2)
                {
                    str2 = str2 + num;
                }
                else
                {
                    str2 = str2 + "0" + num;
                }
                int num5 = list[1 + num3];
                str = num5.ToString();
                if (str.Length >= 2)
                {
                    str2 = str2 + str;
                }
                else if (str.Length <= 1)
                {
                    str2 = str2 + "0" + str;
                }
                str2 = str2 + list[2 + num3].ToString();
                switch (num3)
                {
                    case 0:
                        num3 = 3;
                        break;

                    case 3:
                        num3 = 6;
                        break;

                    case 6:
                        num3 = 9;
                        break;
                }
            }
            return str2;
        }
    }
}
