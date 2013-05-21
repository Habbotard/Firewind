﻿using System;
using System.Collections.Generic;
using System.Data;
using Firewind.Core;
using Firewind.Messages;
using HabboEvents;
using Firewind.HabboHotel.GameClients;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.HabboHotel.Groups.Types;

namespace Firewind.HabboHotel.Rooms
{
    class RoomData
    {
        internal UInt32 Id;
        internal string Name;
        internal string Description;
        internal string Owner;
        internal int OwnerId;
        internal string Password;
        internal int State;//byte
        internal int Category;//byte
        internal int UsersNow;//byte
        internal int UsersMax; //uint16
        internal string ModelName;
        internal int Score; //uint16
        internal List<string> Tags;
        internal bool AllowPets;
        internal bool AllowPetsEating;
        internal bool AllowWalkthrough;
        internal bool AllowRightsOverride;
        internal bool Hidewall;
        private RoomIcon myIcon;
        internal RoomEvent Event;
        internal string Wallpaper;
        internal string Floor;
        internal string Landscape;
        private RoomModel mModel;
        internal int WallThickness;
        internal int FloorThickness;
        internal string Badge;
        internal int GroupID;
        internal Group Group;

        internal RoomIcon Icon
        {
            get
            {
                return myIcon;
            }
        }

        internal int TagCount
        {
            get
            {
                return Tags.Count;
            }
        }

        internal RoomModel Model
        {
            get
            {
                if (mModel == null)
                    mModel = FirewindEnvironment.GetGame().GetRoomManager().GetModel(ModelName, Id);
                return mModel;
            }
        }

        internal RoomData() { }

        internal void FillNull(UInt32 pId)
        {
            this.Id = pId;
            this.Name = "Unknown Room";
            this.Description = "-";
            this.Owner = "-";
            this.Category = 0;
            this.UsersNow = 0;
            this.UsersMax = 0;
            this.ModelName = "NO_MODEL";
            this.Score = 0;
            this.Tags = new List<string>();
            this.AllowPets = true;
            this.AllowPetsEating = false;
            this.AllowWalkthrough = true;
            this.Hidewall = false;
            this.Password = "";
            this.Wallpaper = "0.0";
            this.Floor = "0.0";
            this.Landscape = "0.0";
            this.WallThickness = 0;
            this.FloorThickness = 0;
            //this.Event = null;
            this.Badge = "";
            this.AllowRightsOverride = false;
            this.myIcon = new RoomIcon(1, 1, new Dictionary<int, int>());

            mModel = FirewindEnvironment.GetGame().GetRoomManager().GetModel(ModelName, pId);
        }

        internal void Fill(DataRow Row)
        {
            this.Id = Convert.ToUInt32(Row["id"]);
            this.Name = (string)Row["caption"];
            this.Description = (string)Row["description"];
            this.Owner = (string)Row["owner"];
            this.Badge = (string)Row["badge"];
            this.OwnerId = 0;

            DataRow groupRow;
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT id FROM users WHERE username = '" + this.Owner + "'");
                int result = dbClient.getInteger();
                if(result > 0)
                    this.OwnerId = result;

                dbClient.setQuery("SELECT * FROM group WHERE roomid = @id");
                dbClient.addParameter("id", Id);
                groupRow = dbClient.getRow();
            }

            if(groupRow != null)
                Group = new Group(groupRow, new DataTable());

            switch (Row["state"].ToString().ToLower())
            {
                case "open":

                    this.State = 0;
                    break;

                case "password":

                    this.State = 2;
                    break;

                case "locked":
                default:

                    this.State = 1;
                    break;
            }

            this.Category = (int)Row["category"];
            if (!string.IsNullOrEmpty(Row["active_users"].ToString()))
                this.UsersNow = (int)Row["active_users"];
            else
                this.UsersNow = 0;
            this.UsersMax = (int)Row["users_max"];
            this.ModelName = (string)Row["model_name"];
            this.Score = (int)Row["score"];
            this.Tags = new List<string>();
            this.AllowPets = FirewindEnvironment.EnumToBool(Row["allow_pets"].ToString());
            this.AllowPetsEating = FirewindEnvironment.EnumToBool(Row["allow_pets_eat"].ToString());
            this.AllowWalkthrough = FirewindEnvironment.EnumToBool(Row["allow_walkthrough"].ToString());
            this.AllowRightsOverride = FirewindEnvironment.EnumToBool(Row["allow_rightsoverride"].ToString());
            this.Hidewall = FirewindEnvironment.EnumToBool(Row["allow_hidewall"].ToString());
            this.Password = (string)Row["password"];
            this.Wallpaper = (string)Row["wallpaper"];
            this.Floor = (string)Row["floor"];
            this.Landscape = (string)Row["landscape"];
            this.FloorThickness = Convert.ToInt32(Row["floorthickness"]);
            this.WallThickness = Convert.ToInt32(Row["wallthickness"]);
            //this.Event = null;

            Dictionary<int, int> IconItems = new Dictionary<int,int>();

            if (!string.IsNullOrEmpty(Row["icon_items"].ToString()))
            {
                foreach (string Bit in Row["icon_items"].ToString().Split('|'))
                {
                    if (string.IsNullOrEmpty(Bit))
                        continue;

                    string[] tBit = Bit.Replace('.', ',').Split(',');

                    int a = 0;
                    int b = 0;

                    int.TryParse(tBit[0], out a);
                    if (tBit.Length > 1)
                        int.TryParse(tBit[1], out b);

                    try
                    {
                        if (!IconItems.ContainsKey(a))
                            IconItems.Add(a, b);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException("Exception: " + e.ToString() + "[" + Bit + "]");
                    }
                }
            }

            this.myIcon = new RoomIcon((int)Row["icon_bg"], (int)Row["icon_fg"], IconItems);

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                this.Tags.Add(Tag);
            }

            mModel = FirewindEnvironment.GetGame().GetRoomManager().GetModel(ModelName, Id);
        }

        internal void Fill(Room Room)
        {
            this.Id = Room.RoomId;
            this.Name = Room.Name;
            this.Description = Room.Description;
            this.Owner = Room.Owner;
            this.Category = Room.Category;
            this.State = Room.State;
            this.UsersNow = Room.UsersNow;
            this.UsersMax = Room.UsersMax;
            this.ModelName = Room.ModelName;
            this.Score = Room.Score;

            this.Tags = new List<string>();
            foreach (string tag in Room.Tags.ToArray())
                this.Tags.Add(tag);
            this.AllowPets = Room.AllowPets;
            this.AllowPetsEating = Room.AllowPetsEating;
            this.AllowWalkthrough = Room.AllowWalkthrough;
            this.Hidewall = Room.Hidewall;
            this.myIcon = Room.Icon;
            this.Password = Room.Password;
            this.Event = Room.Event;
            this.Wallpaper = Room.Wallpaper;
            this.Floor = Room.Floor;
            this.Landscape = Room.Landscape;
            this.FloorThickness = Room.FloorThickness;
            this.WallThickness = Room.WallThickness;

            mModel = FirewindEnvironment.GetGame().GetRoomManager().GetModel(ModelName, Id);
        }

        internal void DeadFill(DataRow Row)
        {
            this.Id = Convert.ToUInt32(Row["id"]);
            this.Name = (string)Row["caption"];
            this.Description = (string)Row["description"];
            this.Owner = (string)Row["owner"];

            switch (Row["state"].ToString().ToLower())
            {
                case "open":

                    this.State = 0;
                    break;

                case "password":

                    this.State = 2;
                    break;

                case "locked":
                default:

                    this.State = 1;
                    break;
            }

            this.Category = (int)Row["category"];

            if (!string.IsNullOrEmpty(Row["active_users"].ToString()))
                this.UsersNow = (int)Row["active_users"];
            else
                this.UsersNow = 0;
            this.UsersMax = (int)Row["users_max"];
            this.ModelName = (string)Row["model_name"];
            this.Score = (int)Row["score"];
            this.Tags = new List<string>();
            this.AllowPets = FirewindEnvironment.EnumToBool(Row["allow_pets"].ToString());
            this.AllowPetsEating = FirewindEnvironment.EnumToBool(Row["allow_pets_eat"].ToString());
            this.AllowWalkthrough = FirewindEnvironment.EnumToBool(Row["allow_walkthrough"].ToString());
            this.AllowRightsOverride = FirewindEnvironment.EnumToBool(Row["allow_rightsoverride"].ToString());
            this.Hidewall = FirewindEnvironment.EnumToBool(Row["allow_hidewall"].ToString());
            this.Password = (string)Row["password"];
            this.Wallpaper = (string)Row["wallpaper"];
            this.Floor = (string)Row["floor"];
            this.Landscape = (string)Row["landscape"];
            this.Landscape = (string)Row["landscape"];
            this.FloorThickness = (int)Row["floorthickness"];
            //this.Event = null;

            Dictionary<int, int> IconItems = new Dictionary<int, int>();

            if (!string.IsNullOrEmpty(Row["icon_items"].ToString()))
            {
                foreach (string Bit in Row["icon_items"].ToString().Split('|'))
                {
                    if (string.IsNullOrEmpty(Bit))
                        continue;

                    string[] tBit = Bit.Replace('.', ',').Split(',');

                    int a = 0;
                    int b = 0;

                    
                    if (tBit.Length > 1)
                        int.TryParse(tBit[1], out b);
                    else
                        int.TryParse(tBit[0], out a);

                    try
                    {
                        if (!IconItems.ContainsKey(a))
                            IconItems.Add(a, b);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException("Exception: " + e.ToString() + "[" + Bit + "]");
                    }
                }
            }

            this.myIcon = new RoomIcon((int)Row["icon_bg"], (int)Row["icon_fg"], IconItems);

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                this.Tags.Add(Tag);
            }
        }

        internal void Serialize(ServerMessage Message, Boolean ShowEvents)
        {
            Message.AppendUInt(Id);

            if (Event == null || !ShowEvents)
            {
                Message.AppendBoolean(false);
                Message.AppendString(Name);
                Message.AppendBoolean(Owner != "");
                Message.AppendInt32(OwnerId);
                Message.AppendString(Owner);
                Message.AppendInt32(State); // room state
                Message.AppendInt32(UsersNow);
                Message.AppendInt32(UsersMax);
                Message.AppendString(Description);
                Message.AppendInt32(0); // dunno!
                Message.AppendInt32(2); // can trade?
                Message.AppendInt32(Score);
                Message.AppendInt32(Category);
                Message.AppendInt32(Group != null ? Group.ID : 0); // group id
                Message.AppendString(Group != null ? Group.Name : ""); // group name
                Message.AppendString(Group != null ? Group.BadgeCode : ""); // group image
                //Message.AppendInt32(1);
                //Message.AppendString("GRP");
                //Message.AppendString("");
                Message.AppendString(""); // event start time
                Message.AppendInt32(TagCount);

                foreach (string Tag in Tags)
                {
                    Message.AppendString(Tag);
                }
            }
            else
            {
                Message.AppendBoolean(true);
                Message.AppendString(Event.Name);
                Message.AppendString(Owner);
                Message.AppendInt32(State);
                Message.AppendInt32(UsersNow);
                Message.AppendInt32(UsersMax);
                Message.AppendString(Event.Description);
                Message.AppendBoolean(true);
                Message.AppendBoolean(true);
                Message.AppendInt32(Score);
                Message.AppendInt32(Event.Category);
                Message.AppendString(Event.StartTime);
                Message.AppendInt32(Event.Tags.Count);

                foreach (string Tag in Event.Tags.ToArray())
                {
                    Message.AppendString(Tag);
                }
            }
            Message.AppendInt32(0);
            Message.AppendInt32(0);
            Message.AppendInt32(0);
            Message.AppendBoolean(true);
            Message.AppendBoolean(true);
        }
    }
}
