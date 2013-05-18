using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using HabboEvents;
using Firewind.HabboHotel.Rooms;
using System.Collections.Generic;
using Firewind.HabboHotel.Groups;
using Firewind.HabboHotel.Groups.Types;

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

            Response.Init(Outgoing.GroupCreated);
            Response.AppendInt32(roomID);
            Response.AppendInt32(1);
            SendResponse();

            //ForwardToRoom(roomID);
            Response.Init(Outgoing.OwnGuilds);
            Response.AppendInt32(1); // count

            Response.AppendInt32(1); // groupId
            Response.AppendString(name); // groupName
            Response.AppendString(""); // groupName
            Response.AppendString("FFFFFF"); // color 1
            Response.AppendString("FFFFFF"); // color 2
            Response.AppendBoolean(true); // favourite

            SendResponse();
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

            Response.AppendInt32(5);

            Response.AppendInt32(10);
            Response.AppendInt32(3);
            Response.AppendInt32(4);

            Response.AppendInt32(19);
            Response.AppendInt32(11);
            Response.AppendInt32(5);

            Response.AppendInt32(19);
            Response.AppendInt32(1);
            Response.AppendInt32(3);

            Response.AppendInt32(29);
            Response.AppendInt32(11);
            Response.AppendInt32(4);

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
        public void GetGuildInfo()
        {
            // int, bool
            int groupID = Request.ReadInt32();

            // int, bool, int, string, string, string, int, string, int int, bool, string, bool, bool, string, bool, bool, int
            Response.Init(Outgoing.GroupInfo);

            Response.AppendInt32(groupID); // groupId
            Response.AppendBoolean(false); // is member?
            Response.AppendInt32(0); // type
            Response.AppendString("Awasome group"); // groupName
            Response.AppendString("This text is hardcoded!"); // description
            Response.AppendString(""); // badgeCode
            Response.AppendInt32(1); // roomId
            Response.AppendString("Awesome room"); // roomName
            Response.AppendInt32(0); // status
            Response.AppendInt32(51); // totalMembers
            Response.AppendBoolean(true); // favourite
            Response.AppendString("0-0-1024"); // date created?
            Response.AppendBoolean(true); // is owner?
            Response.AppendBoolean(true); // is admin?
            Response.AppendString("Leonislzy"); // owner name?
            Response.AppendBoolean(true); // openDetails?
            Response.AppendBoolean(false); // show group name?
            Response.AppendInt32(0); // ???

            SendResponse();
        }
    }
}
