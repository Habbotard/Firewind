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
    }
}
