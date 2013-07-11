using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using Firewind.Messages.Headers;
using Firewind.HabboHotel.Rooms;
using System.Collections.Generic;
using Firewind.HabboHotel.Groups;
using Firewind.HabboHotel.Groups.Types;
using Firewind.HabboHotel.Items;

namespace Firewind.Messages
{
    internal partial class GameClientMessageHandler
    {
        public const int GROUP_CREDIT_COST = 10;
        // ID: 2282
        public void CreateGuild()
        {
            // string, string, int, int, int, int[]

            // Step 1, parse the data
            string name = Request.ReadString();
            string desc = Request.ReadString();
            int roomID = Request.ReadInt32();
            int color1 = Request.ReadInt32();
            int color2 = Request.ReadInt32();

            int dataLength = Request.ReadInt32();
            List<Tuple<int, int, int>> badgeData = new List<Tuple<int, int, int>>();
            for (int i = 0; i < dataLength / 3; i++) // each part is 3 ints
            {
                badgeData.Add(new Tuple<int, int, int>(Request.ReadInt32(), Request.ReadInt32(), Request.ReadInt32()));
            }

            // Step 2, check the data
            // Does the user own the room?
            if (!Session.GetHabbo().UsersRooms.Exists(t => t.Id == roomID))
                return;

            // Step 3, do the work
            //ServerMessage message = new ServerMessage(Outgoing.SerializePurchaseInformation);

            //message.AppendInt32(0x1815);
            //message.AppendString("CREATE_GUILD");
            //message.AppendInt32(10);
            //message.AppendInt32(0);
            //message.AppendInt32(0);
            //message.AppendBoolean(true);
            //message.AppendInt32(0);
            //message.AppendInt32(2);
            //message.AppendBoolean(false);

            //Session.SendMessage(message);

            // All checks done, create the stuff
            Group group = FirewindEnvironment.GetGame().GetGroupManager().CreateGroup(Session, name, desc, roomID, color1, color2, badgeData);

            if (group == null)
                return; // TODO: Error message?

            Response.Init(Outgoing.GroupCreated);
            Response.AppendInt32(group.RoomID);
            Response.AppendInt32(group.ID);
            SendResponse();

            Session.GetHabbo().SendGroupList();
        }

        // ID: 2616
        public void UpdateGuildBadge()
        {
            // int, int[]
            int guildID = Request.ReadInt32();
            int[] newBadgeData = new int[Request.ReadInt32()];
            for (int i = 0; i < newBadgeData.Length; i++)
            {
                newBadgeData[i] = Request.ReadInt32();
            }
        }

        // ID: 1137
        public void StartGuildPurchase()
        {
            // REPLY ID: 3341
            // int=group cost, Array(int=roomid,string=roomname,bool)=available group rooms, Array(int,int,int="position")
            // Generate a list with rooms that aren't group rooms already
            List<RoomData> availableRooms = Session.GetHabbo().UsersRooms.FindAll(s => s.GroupID == 0);

            Response.Init(Outgoing.PurchaseGuildInfo);
            Response.AppendInt32(GROUP_CREDIT_COST); // TODO: Configurable

            Response.AppendInt32(availableRooms.Count);
            foreach (RoomData data in availableRooms)
            {
                Response.AppendInt32((int)data.Id);
                Response.AppendString(data.Name);
                Response.AppendBoolean(false); // WTF IS THIS SHIT WTF
            }

            //Response.AppendInt32(0); // IDK what next array actually is

            Response.AppendInt32(5); // group badge part count8

            Response.AppendInt32(5);
            Response.AppendInt32(11);
            Response.AppendInt32(4);

            Response.AppendInt32(6);
            Response.AppendInt32(11);
            Response.AppendInt32(4);

            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);

            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);

            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);

            SendResponse();

            // We just send blank data for now
            // fuck im lazy, leon make this real
            Response.Init(Outgoing.GuildEditorData); // i dont even know if this is the right header dammet


            Response.AppendInt32(GuildsPartsData.BaseBadges.Count);
            foreach (GuildsPartsData data in GuildsPartsData.BaseBadges)
            {
                Response.AppendInt32(data.Id);
                Response.AppendString(data.ExtraData1);
                Response.AppendString(data.ExtraData2);
            }
            Response.AppendInt32(GuildsPartsData.SymbolBadges.Count);
            foreach (GuildsPartsData data in GuildsPartsData.SymbolBadges)
            {
                Response.AppendInt32(data.Id);
                Response.AppendString(data.ExtraData1);
                Response.AppendString(data.ExtraData2);
            }
            Response.AppendInt32(GuildsPartsData.ColorBadges1.Count);
            foreach (GuildsPartsData data in GuildsPartsData.ColorBadges1)
            {
                Response.AppendInt32(data.Id);
                Response.AppendString(data.ExtraData1);
            }
            Response.AppendInt32(GuildsPartsData.ColorBadges2.Count);
            foreach (GuildsPartsData data in GuildsPartsData.ColorBadges2)
            {
                Response.AppendInt32(data.Id);
                Response.AppendString(data.ExtraData1);
            }
            Response.AppendInt32(GuildsPartsData.ColorBadges3.Count);
            foreach (GuildsPartsData data in GuildsPartsData.ColorBadges3)
            {
                Response.AppendInt32(data.Id);
                Response.AppendString(data.ExtraData1);
            }

            //// first bases
            //Response.AppendInt32(1);
            //Response.AppendInt32(1);
            //Response.AppendString("base_basic_1.gif");
            //Response.AppendString("");

            //// then symbols
            //Response.AppendInt32(1);
            //Response.AppendInt32(2);
            //Response.AppendString("symbol_background_1.gif");
            //Response.AppendString("");

            //// then color 1s
            //Response.AppendInt32(1);
            //Response.AppendInt32(3);
            //Response.AppendString("ffffff");

            //// then color 2s
            //Response.AppendInt32(1);
            //Response.AppendInt32(4);
            //Response.AppendString("ffffff");

            //// then color 3s
            //Response.AppendInt32(1);
            //Response.AppendInt32(5);
            //Response.AppendString("ffffff");

            SendResponse();
        }

        // ID: 1660
        public void GetHabboGroupDetails()
        {
            // int, bool
            int groupID = Request.ReadInt32();
            bool openWindow = Request.ReadBoolean(); // false when normal enter

            Group group = FirewindEnvironment.GetGame().GetGroupManager().GetGroup(groupID);

            if (group == null)
                return;

            // int, bool, int, string, string, string, int, string, int int, bool, string, bool, bool, string, bool, bool, int
            Response.Init(Outgoing.HabboGroupDetails);
            Response.AppendInt32(groupID); // groupId
            Response.AppendBoolean(group.Members.Contains(Session.GetHabbo().Id)); // is member?
            Response.AppendInt32(group.Type); // type
            Response.AppendString(group.Name); // groupName
            Response.AppendString(group.Description); // description
            Response.AppendString(group.BadgeCode); // badgeCode
            Response.AppendInt32(group.RoomID); // roomId
            Response.AppendString(group.RoomName); // roomName
            Response.AppendInt32(group.GetMemberType(Session.GetHabbo().Id)); // status
            Response.AppendInt32(group.Members.Count); // totalMembers
            Response.AppendBoolean(Session.GetHabbo().FavouriteGroup == groupID); // favourite
            Response.AppendString(group.DateCreated); // date created?
            Response.AppendBoolean(group.OwnerID == Session.GetHabbo().Id); // is owner?
            Response.AppendBoolean(group.GetMemberType(Session.GetHabbo().Id) >= 2); // is admin?
            Response.AppendString(group.OwnerName); // owner name?
            Response.AppendBoolean(openWindow); // openDetails
            Response.AppendBoolean(group.AdminDecorate);
            Response.AppendInt32(0); // ???

            SendResponse();
        }

        // ID: 1557
        public void GetHabboGroupsWhereMember()
        {
            // For the furni catalog page
            Session.GetHabbo().SendGroupList();
        }

        // ID: 3015
        public void GetGuildFurniInfo()
        {
            // int (item id), int (group id)
            int itemID = Request.ReadInt32();
            int groupID = Request.ReadInt32();

            if (Session.GetHabbo().CurrentRoom == null)
                return;

            RoomItem item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem((uint)itemID);
            if (item == null || item.data.GetTypeID() != 2 || (item.GetBaseItem().InteractionType != InteractionType.guildgeneric && item.GetBaseItem().InteractionType != InteractionType.guilddoor))
                return;

            Group group = FirewindEnvironment.GetGame().GetGroupManager().GetGroup(groupID);
            if (group == null)
                return;

            StringArrayStuffData data = item.data as StringArrayStuffData;
            int actualGroupID;
            if (!int.TryParse(data.Data[1], out actualGroupID) || groupID != actualGroupID) // data[1] = group id
                return;

            Response.Init(Outgoing.GuildFurniInfo);
            // int (item id),int,string,int,bool
            Response.AppendInt32(itemID);
            Response.AppendInt32(groupID);
            Response.AppendString(group.Name);
            Response.AppendInt32(group.RoomID);
            Response.AppendBoolean(group.GetMemberType(Session.GetHabbo().Id) > 0);
            SendResponse();
        }

        // ID: 1811
        public void ManageGroup()
        {
            int groupID = Request.ReadInt32();

            Group group = FirewindEnvironment.GetGame().GetGroupManager().GetGroup(groupID);

            if (group == null || group.GetMemberType(Session.GetHabbo().Id) < 3)
                return;

            List<RoomData> availableRooms = Session.GetHabbo().UsersRooms.FindAll(s => s.GroupID == 0);

            Response.Init(Outgoing.GuildEditInfo);

            Response.AppendInt32(availableRooms.Count);
            foreach (RoomData data in availableRooms)
            {
                Response.AppendInt32((int)data.Id);
                Response.AppendString(data.Name);
                Response.AppendBoolean(false); // WTF IS THIS SHIT WTF
            }
            Response.AppendBoolean(group.OwnerID == Session.GetHabbo().Id); // is owner?
            Response.AppendInt32(group.ID);
            Response.AppendString(group.Name);
            Response.AppendString(group.Description);
            Response.AppendInt32(group.RoomID); // roomId
            Response.AppendInt32(group.ColorID1);
            Response.AppendInt32(group.ColorID2);
            Response.AppendInt32(group.Type); // type
            Response.AppendInt32(Convert.ToInt32(group.AdminDecorate));
            Response.AppendBoolean(false); // locked
            Response.AppendString(""); // url

            Response.AppendInt32(group.BadgeData.Count);
            foreach (var part in group.BadgeData)
            {
                Response.AppendInt32(part.Item1);
                Response.AppendInt32(part.Item2);
                Response.AppendInt32(part.Item3);
            }

            Response.AppendString(group.BadgeCode); // badgeCode
            Response.AppendInt32(group.Members.Count); // totalMembers

            SendResponse();
        }
    }
}
