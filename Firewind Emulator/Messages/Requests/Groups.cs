using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;
using System;
using HabboEvents;
using Firewind.HabboHotel.Rooms;
using System.Collections.Generic;
using Firewind.HabboHotel.Groups;

namespace Firewind.Messages
{
    internal partial class GameClientMessageHandler
    {
        public const int GROUP_CREDIT_COST = 10;
        // ID: 2282
        public void CreateGuild()
        {
            // string, string, int, int, int, int[]
            string name = Request.PopFixedString();
            string desc = Request.PopFixedString();
            int roomID = Request.PopWiredInt32();
            int color1 = Request.PopWiredInt32();
            int color2 = Request.PopWiredInt32();

            int[] badgeData = new int[Request.PopWiredInt32()];
            for (int i = 0; i < badgeData.Length; i++)
            {
                badgeData[i] = Request.PopWiredInt32();
            }
        }

        // ID: 2616
        public void UpdateGuildBadge()
        {
            // int, int[]
            int guildID = Request.PopWiredInt32();
            int[] newBadgeData = new int[Request.PopWiredInt32()];
            for (int i = 0; i < newBadgeData.Length; i++)
            {
                newBadgeData[i] = Request.PopWiredInt32();
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

            Response.AppendInt32(0); // IDK what next array actually is
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
        }
    }
}
