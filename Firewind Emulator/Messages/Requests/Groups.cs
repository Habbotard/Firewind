using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;
using System;

namespace Firewind.Messages
{
    internal partial class GameClientMessageHandler
    {
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
            // REPLY ID: 3529
            // Array(int,string,bool), bool, int, string, string, int, int, int, int, int, bool, string, array(???), string, int
        }
    }
}
