using System;
using System.Data;
using System.Collections.Generic;
using Butterfly.HabboHotel.Items;
using Butterfly.Messages;
using Butterfly.Core;

namespace Butterfly.HabboHotel.Catalogs
{
    class CatalogItem
    {
        internal readonly uint Id;
        internal readonly string ItemIdString;
        internal List<uint> Items;
        internal readonly string Name;
        internal readonly int CreditsCost;
        internal readonly int PixelsCost;
        internal readonly int Amount;
        internal readonly int PageID;
        internal readonly int CrystalCost;
        internal readonly int OudeCredits;
        internal readonly uint songID;
        internal readonly bool IsLimited;
        internal int LimitedSelled;
        internal readonly int LimitedStack;
        internal readonly bool HaveOffer;

        internal CatalogItem(DataRow Row)
        {
            this.Id = Convert.ToUInt32(Row["id"]);
            this.Name = (string)Row["catalog_name"];
            this.ItemIdString = (string)Row["item_ids"];
            this.Items = new List<uint>();
            if (this.ItemIdString.Contains(";"))
            {
                string[] splitted = ItemIdString.Split(';');
                foreach (string s in splitted)
                {
                    if (s != "")
                        this.Items.Add((uint)int.Parse(s));
                }
            }
            else
            {
                    this.Items.Add(uint.Parse(ItemIdString));
            }
            this.PageID = (int)Row["page_id"];
            this.CreditsCost = (int)Row["cost_credits"];
            this.PixelsCost = (int)Row["cost_pixels"];
            this.Amount = (int)Row["amount"];
            this.CrystalCost = (int)Row["cost_points"];
            //this.OudeCredits = (int)Row["cost_oude_belcredits"];
            this.OudeCredits = 0;
            this.songID = Convert.ToUInt32(Row["song_id"]);
            this.LimitedSelled = (int)Row["limited_sells"];
            this.LimitedStack = (int)Row["limited_stack"];
            this.IsLimited = (this.LimitedStack > 0);
            this.HaveOffer = ((int)Row["offer_active"] == 1);
            //this.songID = 0;
        }

        internal Item GetBaseItem(uint ItemIds)
        {
            Item Return = ButterflyEnvironment.GetGame().GetItemManager().GetItem(ItemIds);
            if (Return == null)
            {
                Logging.WriteLine("UNKNOWN ItemIds: " + ItemIds);
            }

            return Return;
        }

        internal void SerializeClub(ServerMessage Message, GameClients.GameClient Session)
        {
            try
            {
                Message.AppendUInt(Id);
                Message.AppendString(Name);
                Message.AppendInt32(CreditsCost);
                Message.AppendBoolean(true); // don't know
                int Days = 0;
                int Months = 0;
                if(Name.Contains("HABBO_CLUB_VIP_"))
                {
                    if (Name.Contains("_DAY"))
                    {
                        Days = int.Parse(Name.Split('_')[3]);
                    }
                    else if (Name.Contains("_MONTH"))
                    {
                        Months = int.Parse(Name.Split('_')[3]);
                        Days = 31 * Months;
                    }
                } else if(Name.Equals("deal_vip_1_year_and_badge"))
                {
                    Months = 12;
                    Days = 31*Months;
                } else if(Name.Equals("HABBO_CLUB_VIP_5_YEAR"))
                {
                    Months = 5*12;
                    Days = 31*Months;
                }
                DateTime future = DateTime.Now;
                if (Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip"))
                {
                    Double Expire = Session.GetHabbo().GetSubscriptionManager().GetSubscription("habbo_vip").ExpireTime;
                    Double TimeLeft = Expire - ButterflyEnvironment.GetUnixTimestamp();
                    int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                    future = DateTime.Now.AddDays(TotalDaysLeft);
                }
                future = future.AddDays(Days);
                Message.AppendInt32(Months); // months
                Message.AppendInt32(Days); // days
                Message.AppendInt32(Days); // wtf
                Message.AppendInt32(future.Year); // year
                Message.AppendInt32(future.Month); // month
                Message.AppendInt32(future.Day); // day
            }
            catch
            {
                //Logging.WriteLine("Unable to load club item " + Id + ": " + Name);
            }

        }

        internal void Serialize(ServerMessage Message)
        {
            try
            {
                if (PageID == 5)
                    return; // club
                

                Message.AppendUInt(Id);
                Message.AppendString(Name);
                Message.AppendInt32(CreditsCost);

                if (CrystalCost > 0)
                {
                    Message.AppendInt32(CrystalCost);
                    Message.AppendInt32(103);
                }
                else
                {
                    Message.AppendInt32(PixelsCost);
                    Message.AppendInt32(0); // ID of currency (0 = pixel)
                }
                Message.AppendBoolean(true); // AllowBuy
                Message.AppendInt32(Items.Count); // items on pack
                // and serialize it
                foreach (uint i in Items)
                {
                    Message.AppendString(GetBaseItem(i).Type.ToString());
                    Message.AppendInt32(GetBaseItem(i).SpriteId);
                    // extradata
                    if (Name.Contains("wallpaper_single") || Name.Contains("floor_single") || Name.Contains("landscape_single"))
                    {
                        string[] Analyze = Name.Split('_');
                        Message.AppendString(Analyze[2]);
                    }
                    else if (this.songID > 0 && GetBaseItem(i).InteractionType == InteractionType.musicdisc)
                    {
                        Message.AppendString(songID.ToString());
                    }
                    else
                    {
                        Message.AppendString(string.Empty);
                    }
                    Message.AppendInt32(Amount);
                    Message.AppendInt32(-1); // getItemDuration
                }
                Message.AppendBoolean(this.IsLimited); // IsLimited
                if (this.IsLimited)
                {
                    Message.AppendInt32(this.LimitedStack);
                    Message.AppendInt32(this.LimitedStack - this.LimitedSelled);
                }
                Message.AppendInt32(0); // club_level
                Message.AppendBoolean((this.IsLimited) ? false : this.HaveOffer); // IsOffer
            }
            catch
            {
                Logging.WriteLine("Unable to load furniture item " + Id + ": " + Name);
            }

        }
    }
}
