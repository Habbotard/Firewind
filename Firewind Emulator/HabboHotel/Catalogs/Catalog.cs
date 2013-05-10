﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Firewind.Core;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.Users.Inventory;
using Firewind.Messages;
using Database_Manager.Database.Session_Details.Interfaces;
using HabboEvents;
using Firewind.HabboHotel.Rooms;


namespace Firewind.HabboHotel.Catalogs
{
    class Catalog
    {
        internal Dictionary<int, CatalogPage> Pages;
        internal List<EcotronReward> EcotronRewards;

        private Marketplace Marketplace;

        private ServerMessage[] mCataIndexCache;
        //private Task mFurniIDCYcler;

        internal Catalog()
        {
            Marketplace = new Marketplace();
        }

        internal void Initialize(IQueryAdapter dbClient)
        {
            Pages = new Dictionary<int, CatalogPage>();
            EcotronRewards = new List<EcotronReward>();

            dbClient.setQuery("SELECT * FROM catalog_pages ORDER BY order_num");
            DataTable Data = dbClient.getTable();

            dbClient.setQuery("SELECT * FROM ecotron_rewards ORDER BY item_id");
            DataTable EcoData = dbClient.getTable();

            Hashtable CataItems = new Hashtable();
            dbClient.setQuery("SELECT id,item_ids,catalog_name,cost_credits,cost_pixels,cost_points,amount,page_id,song_id,limited_sells,limited_stack,offer_active FROM catalog_items");
            DataTable CatalogueItems = dbClient.getTable();

            if (CatalogueItems != null)
            {
                foreach (DataRow Row in CatalogueItems.Rows)
                {
                    if (string.IsNullOrEmpty(Row["item_ids"].ToString()) || (int)Row["amount"] <= 0)
                    {
                        continue;
                    }
                    CataItems.Add(Convert.ToUInt32(Row["id"]), new CatalogItem(Row));
                    //Items.Add(new CatalogItem((uint)Row["id"], (string)Row["catalog_name"], (string)Row["item_ids"], (int)Row["cost_credits"], (int)Row["cost_pixels"], (int)Row["amount"]));
                }
            }

            if (Data != null)
            {
                foreach (DataRow Row in Data.Rows)
                {
                    Boolean Visible = false;
                    Boolean Enabled = false;

                    if (Row["visible"].ToString() == "1")
                    {
                        Visible = true;
                    }

                    if (Row["enabled"].ToString() == "1")
                    {
                        Enabled = true;
                    }

                    Pages.Add((int)Row["id"], new CatalogPage((int)Row["id"], (int)Row["parent_id"],
                        (string)Row["caption"], Visible, Enabled, Convert.ToUInt32(Row["min_rank"]),
                        FirewindEnvironment.EnumToBool(Row["club_only"].ToString()), (int)Row["icon_color"],
                        (int)Row["icon_image"], (string)Row["page_layout"], (string)Row["page_headline"],
                        (string)Row["page_teaser"], (string)Row["page_special"], (string)Row["page_text1"],
                        (string)Row["page_text2"], (string)Row["page_text_details"], (string)Row["page_text_teaser"], ref CataItems));
                }
            }

            if (EcoData != null)
            {
                foreach (DataRow Row in EcoData.Rows)
                {
                    EcotronRewards.Add(new EcotronReward(Convert.ToUInt32(Row["display_id"]), Convert.ToUInt32(Row["item_id"]), Convert.ToUInt32(Row["reward_level"])));
                }
            }

            RestackByFrontpage();
        }

        internal void RestackByFrontpage()
        {
            CatalogPage fronpage = Pages[1];
            Dictionary<int, CatalogPage> restOfCata = new Dictionary<int, CatalogPage>(Pages);

            restOfCata.Remove(1);
            Pages.Clear();

            Pages.Add(fronpage.PageId, fronpage);

            foreach (KeyValuePair<int, CatalogPage> pair in restOfCata)
                Pages.Add(pair.Key, pair.Value);
        }

        internal void InitCache()
        {
            mCataIndexCache = new ServerMessage[10]; //Max 7 ranks

            for (int i = 1; i < 10; i++)
            {
                mCataIndexCache[i] = SerializeIndexForCache(i);
            }

            foreach (CatalogPage Page in Pages.Values)
            {
                Page.InitMsg();
            }
        }

        internal CatalogItem FindItem(uint ItemId)
        {
            foreach (CatalogPage Page in Pages.Values)
            {
                if (Page.Items.ContainsKey(ItemId))
                    return (CatalogItem)Page.Items[ItemId];
            }

            return null;
        }

        //internal Boolean IsItemInCatalog(uint BaseId)
        //{
        //    DataRow Row = null;

        //    using (DatabaseClient dbClient = FirewindEnvironment.GetDatabase().GetClient())
        //    {
        //        Row = dbClient.getRow("SELECT id FROM catalog_items WHERE item_ids = '" + BaseId + "' LIMIT 1");
        //    }

        //    if (Row != null)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        internal int GetTreeSize(int rank, int TreeId)
        {
            int i = 0;

            foreach (CatalogPage Page in Pages.Values)
            {
                if (Page.MinRank > rank)
                {
                    continue;
                }

                if (Page.ParentId == TreeId)
                {
                    i++;
                }
            }


            return i;
        }

        internal CatalogPage GetPage(int Page)
        {
            if (!Pages.ContainsKey(Page))
            {
                return null;
            }

            return Pages[Page];
        }

        internal void HandlePurchase(GameClient Session, int PageId, uint ItemId, string extraParameter, int buyAmount, Boolean IsGift, string GiftUser, string GiftMessage, int GiftSpriteId, int GiftLazo, int GiftColor, bool giftShowIdentity)
        {
            int finalAmount = buyAmount;
            if (buyAmount > 5) // Possible discount!
            {
                // Nearest number that increases the amount of free items
                int nearestDiscount = ((int)Math.Floor(buyAmount / 6.0) * 6);

                // How many free ones we get
                int freeItemsCount = (nearestDiscount - 3) / 3;

                // Add 1 free if more than 42
                if (buyAmount >= 42)
                    freeItemsCount++;

                // Doesn't follow rules as it isn't dividable by 6, but still increases free items
                if (buyAmount >= 99)
                {
                    freeItemsCount = 33;
                }

                // This is how many we pay for in the end
                finalAmount = buyAmount - freeItemsCount;
            }

            //Logging.WriteLine("Amount: " + priceAmount + "; withOffer= " + finalAmount);
            CatalogPage Page;
            if (!Pages.TryGetValue(PageId, out Page))
                return;
            if (Page == null || !Page.Enabled || !Page.Visible || Session == null || Session.GetHabbo() == null)
            {
                return;
            }
            if (Page.ClubOnly && !Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_club") && !Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip"))
            {
                return;
            }
            if (Page.MinRank > Session.GetHabbo().Rank)
            {
                return;
            }
            CatalogItem Item = Page.GetItem(ItemId);

            if (Item == null)
            {
                return;
            }

            if (Item.Name.Contains("HABBO_CLUB_VIP") || Item.Name.StartsWith("DEAL_HC_") || Item.Name.Equals("deal_vip_1_year_and_badge"))
            {
                if (Item.CreditsCost > Session.GetHabbo().Credits)
                    return;

                int Months = 0;
                int Days = 0;
                if (Item.Name.Contains("HABBO_CLUB_VIP_"))
                {
                    if (Item.Name.Contains("_DAY"))
                    {
                        Days = int.Parse(Item.Name.Split('_')[3]);
                    }
                    else if (Item.Name.Contains("_MONTH"))
                    {
                        Months = int.Parse(Item.Name.Split('_')[3]);
                        Days = 31 * Months;
                    }
                }
                else if (Item.Name.Equals("deal_vip_1_year_and_badge"))
                {
                    Months = 12;
                    Days = 31 * Months;
                }
                else if (Item.Name.Equals("HABBO_CLUB_VIP_5_YEAR"))
                {
                    Months = 5 * 12;
                    Days = 31 * Months;
                }
                else if(Item.Name.StartsWith("DEAL_HC_"))
                {
                    Months = int.Parse(Item.Name.Split('_')[2]);
                    Days = 31 * Months;

                    if (Item.CreditsCost > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CreditsCost;
                        Session.GetHabbo().UpdateCreditsBalance();
                    }

                    Session.GetHabbo().GetSubscriptionManager().AddOrExtendSubscription("habbo_club", Days * 24 * 3600);
                    Session.GetHabbo().SerializeClub();
                    return;
                }

                if (Item.CreditsCost > 0)
                {
                    Session.GetHabbo().Credits -= Item.CreditsCost;
                    Session.GetHabbo().UpdateCreditsBalance();
                }

                Session.GetHabbo().GetSubscriptionManager().AddOrExtendSubscription("habbo_vip", Days * 24 * 3600);
                Session.GetHabbo().SerializeClub();

                return;
            }

            if (Item.IsLimited)
            {
                finalAmount = 1;
                buyAmount = 1;
                if (Item.LimitedStack <= Item.LimitedSelled)
                    return;
                Item.LimitedSelled++;
                using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE catalog_items SET limited_sells = " + Item.LimitedSelled + " WHERE id = " + Item.Id);
                }
                Page.InitMsg(); // update page!

                // send update
                //Session.SendMessage(Page.GetMessage);
            }
            else if (IsGift & buyAmount > 1)
            {
                finalAmount = 1;
                buyAmount = 1;
                Session.SendNotif("Lo sentimos, pero tu regalo solo puede contener un item, por lo que solo has comprado uno");
            }
            uint GiftUserId = 0;

            Boolean CreditsError = false;
            Boolean ActivityPointError = false;

            if (Session.GetHabbo().Credits < (Item.CreditsCost * finalAmount))
            {
                CreditsError = true;
            }

            if (Session.GetHabbo().Currencies.GetAmountOfCurrency(Item.ActivityPointType) < (Item.ActivityPointCost * finalAmount))
            {
                ActivityPointError = true;
            }

            if (CreditsError || ActivityPointError)
            {
                Session.GetMessageHandler().GetResponse().Init(Outgoing.NotEnoughBalance);
                Session.GetMessageHandler().GetResponse().AppendBoolean(CreditsError);
                Session.GetMessageHandler().GetResponse().AppendBoolean(ActivityPointError);
                Session.GetMessageHandler().GetResponse().AppendInt32(Item.ActivityPointType);
                Session.GetMessageHandler().SendResponse();

                return;
            }


            if (Item.CreditsCost > 0 && !IsGift)
            {
                Session.GetHabbo().Credits -= (Item.CreditsCost * finalAmount);
                Session.GetHabbo().UpdateCreditsBalance();
            }

            if (Item.ActivityPointCost > 0 && !IsGift)
            {
                Session.GetHabbo().Currencies.RemoveAmountOfCurrency(Item.ActivityPointType, Item.ActivityPointCost * finalAmount);
                Session.GetHabbo().Currencies.RefreshActivityPointsBalance(Item.ActivityPointType);
            }
            foreach (uint i in Item.Items)
            {
                //Logging.WriteLine(Item.GetBaseItem().ItemId);
                //Logging.WriteLine(Item.GetBaseItem().InteractionType.ToLower());
                // Extra Data is _NOT_ filtered at this point and MUST BE VERIFIED BELOW:
                if (IsGift)
                {
                    if (!Item.GetBaseItem(i).AllowGift)
                    {
                        return;
                    }

                    DataRow dRow;
                    using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                    {
                        dbClient.setQuery("SELECT id FROM users WHERE username = @gift_user");
                        dbClient.addParameter("gift_user", GiftUser);


                        dRow = dbClient.getRow();
                    }

                    if (dRow == null)
                    {
                        Session.GetMessageHandler().GetResponse().Init(Outgoing.GiftError);
                        Session.GetMessageHandler().GetResponse().AppendString(GiftUser);
                        Session.GetMessageHandler().SendResponse();

                        return;
                    }

                    GiftUserId = Convert.ToUInt32(dRow[0]);

                    if (GiftUserId == 0)
                    {
                        Session.GetMessageHandler().GetResponse().Init(Outgoing.GiftError);
                        Session.GetMessageHandler().GetResponse().AppendString(GiftUser);
                        Session.GetMessageHandler().SendResponse();

                        return;
                    }

                    if (Item.CreditsCost > 0 && IsGift)
                    {
                        Session.GetHabbo().Credits -= (Item.CreditsCost * finalAmount);
                        Session.GetHabbo().UpdateCreditsBalance();
                    }

                    if (Item.ActivityPointCost > 0 && IsGift)
                    {
                        Session.GetHabbo().Currencies.RemoveAmountOfCurrency(Item.ActivityPointType, Item.ActivityPointCost * finalAmount);
                        Session.GetHabbo().Currencies.RefreshActivityPointsBalance(Item.ActivityPointType);
                    }
                }


                if (IsGift && Item.GetBaseItem(i).Type == 'e')
                {
                    Session.SendNotif(LanguageLocale.GetValue("catalog.gift.send.error"));
                    return;
                }
                IRoomItemData itemData = new StringData(extraParameter);
                switch (Item.GetBaseItem(i).InteractionType)
                {
                    case InteractionType.none:
                        //itemData = new StringData(extraParameter);
                        break;

                    case InteractionType.musicdisc:
                        itemData = new StringData(Item.songID.ToString());
                        break;

                    #region Pet handling
                    case InteractionType.pet:
                        try
                        {

                            //uint count = 0;
                            //using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                            //{
                            //    dbClient.setQuery("SELECT COUNT(*) FROM user_pets WHERE user_id = " + Session.GetHabbo().Id);
                            //    count = uint.Parse(dbClient.getString());
                            //}

                            //if (count > 5)
                            //{
                            //    Session.SendNotif(LanguageLocale.GetValue("catalog.pets.maxpets"));
                            //    return;
                            //}

                            string[] Bits = extraParameter.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (!CheckPetName(PetName))
                                return;

                            //if (Race.Length != 1)
                            //    return;

                            if (Color.Length != 6)
                                return;
                        }
                        catch (Exception e)
                        {
                            Logging.WriteLine(e.ToString());
                            Logging.HandleException(e, "Catalog.HandlePurchase");
                            return;
                        }

                        break;

                    #endregion

                    case InteractionType.roomeffect:

                        Double Number = 0;

                        try
                        {
                            if (string.IsNullOrEmpty(extraParameter))
                                Number = 0;
                            else
                                Number = Double.Parse(extraParameter, FirewindEnvironment.cultureInfo);
                        }
                        catch (Exception e) { Logging.HandleException(e, "Catalog.HandlePurchase: " + extraParameter); }

                        itemData = new StringData(Number.ToString().Replace(',', '.'));
                        break; // maintain extra data // todo: validate

                    case InteractionType.postit:
                        itemData = new StringData("FFFF33");
                        break;

                    case InteractionType.dimmer:
                        itemData = new StringData("1,1,1,#000000,255");
                        break;

                    case InteractionType.trophy:
                        itemData = new StringData(String.Format("{0}\t{1}\t{2}", Session.GetHabbo().Username, DateTime.Now.ToString("d-M-yyy"), extraParameter));
                        break;

                    //case InteractionType.mannequin:
                    //    MapStuffData data = new MapStuffData();
                    //    data.Data.Add("OUTFIT_NAME", "");
                    //    data.Data.Add("FIGURE", "");
                    //    data.Data.Add("GENDER", "");
                    //    itemData = data;
                    //    break;

                    default:
                        //itemData = new StringData(extraParameter);
                        break;
                }


                Session.GetMessageHandler().GetResponse().Init(Outgoing.UpdateInventary);
                Session.GetMessageHandler().SendResponse();

                Session.GetMessageHandler().GetResponse().Init(Outgoing.SerializePurchaseInformation); // PurchaseOKMessageEvent
                Session.GetMessageHandler().GetResponse().AppendUInt(Item.GetBaseItem(i).ItemId); // offerID
                Session.GetMessageHandler().GetResponse().AppendString(Item.GetBaseItem(i).Name);  // localizationId
                Session.GetMessageHandler().GetResponse().AppendInt32(Item.CreditsCost); // priceInCredits
                Session.GetMessageHandler().GetResponse().AppendInt32(Item.ActivityPointCost); // priceInActivityPoints
                Session.GetMessageHandler().GetResponse().AppendInt32(Item.ActivityPointType); // activityPointType
                Session.GetMessageHandler().GetResponse().AppendBoolean(true); // unknown
                Session.GetMessageHandler().GetResponse().AppendInt32(1); // products count
                Session.GetMessageHandler().GetResponse().AppendString(Item.GetBaseItem(i).Type.ToString().ToLower()); // productType [i,s,e,b]
                Session.GetMessageHandler().GetResponse().AppendInt32(Item.GetBaseItem(i).SpriteId); // furniClassId
                Session.GetMessageHandler().GetResponse().AppendString(""); // extraParam
                Session.GetMessageHandler().GetResponse().AppendInt32(1); // productCount
                Session.GetMessageHandler().GetResponse().AppendInt32(0); // expiration
                Session.GetMessageHandler().GetResponse().AppendBoolean(Item.IsLimited);

                if (Item.IsLimited)
                {
                    Session.GetMessageHandler().GetResponse().AppendInt32(Item.LimitedStack); // uniqueLimitedItemSeriesSize
                    Session.GetMessageHandler().GetResponse().AppendInt32(Item.LimitedStack - Item.LimitedSelled); // uniqueLimitedItemsLeft
                }

                Session.GetMessageHandler().GetResponse().AppendInt32(0); // clubLevel
                Session.GetMessageHandler().GetResponse().AppendBoolean(false); // unknown

                Session.GetMessageHandler().SendResponse();

                if (IsGift)
                {
                    uint itemID;
                    //uint GenId = GenerateItemId();
                    Item Present = FirewindEnvironment.GetGame().GetItemManager().GetItemBySpriteID(GiftSpriteId);
                    if (Present == null)
                    {
                        Logging.LogDebug(string.Format("Somebody tried to purchase a present with invalid sprite ID: {0}", GiftSpriteId));
                    }

                    MapStuffData giftData = new MapStuffData();

                    if (giftShowIdentity)
                    {
                        giftData.Data.Add("PURCHASER_NAME", Session.GetHabbo().Username);
                        giftData.Data.Add("PURCHASER_FIGURE", Session.GetHabbo().Look);
                    }
                    giftData.Data.Add("MESSAGE", GiftMessage);
                    giftData.Data.Add("PRODUCT_CODE", "10");
                    giftData.Data.Add("EXTRA_PARAM", "test");
                    giftData.Data.Add("state", "1");

                    //Logging.WriteLine((uint)GiftSpriteId +"   -    "  +FirewindEnvironment.giftInt);
                    //Logging.WriteLine("Resultado regalo: " + FirewindEnvironment.GetGame().GetItemManager().GetItem((uint)GiftSpriteId - FirewindEnvironment.giftInt));
                    using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                    {
                        dbClient.setQuery("INSERT INTO items (base_id) VALUES (" + Present.ItemId + ")");
                        itemID = (uint)dbClient.insertQuery();

                        dbClient.runFastQuery("INSERT INTO items_users VALUES (" + itemID + "," + GiftUserId + ")");

                        if (!string.IsNullOrEmpty(GiftMessage))
                        {
                            dbClient.setQuery("INSERT INTO items_extradata VALUES (" + itemID + ",@datatype,@data,@extra)");
                            dbClient.addParameter("datatype", giftData.GetTypeID());
                            dbClient.addParameter("data", giftData.ToString());
                            dbClient.addParameter("extra", GiftColor * 1000 + GiftLazo);
                            dbClient.runQuery();
                        }

                        dbClient.setQuery("INSERT INTO user_presents (item_id,base_id,amount,extra_data) VALUES (" + itemID + "," + Item.GetBaseItem(i).ItemId + "," + Item.Amount + ",@extra_data)");
                        dbClient.addParameter("gift_message", "!" + GiftMessage);
                        dbClient.addParameter("extra_data", itemData.ToString());
                        dbClient.runQuery();
                    }

                    GameClient Receiver = FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(GiftUserId);

                    if (Receiver != null)
                    {
                        Receiver.SendNotif(LanguageLocale.GetValue("catalog.gift.received") + Session.GetHabbo().Username);
                        UserItem u = Receiver.GetHabbo().GetInventoryComponent().AddNewItem(itemID, Present.ItemId, giftData, GiftColor * 1000 + GiftLazo, false, false, 0);
                        Receiver.GetHabbo().GetInventoryComponent().SendFloorInventoryUpdate();
                        Receiver.GetMessageHandler().GetResponse().Init(Outgoing.SendPurchaseAlert);
                        Receiver.GetMessageHandler().GetResponse().AppendInt32(1); // items
                        Receiver.GetMessageHandler().GetResponse().AppendInt32(1); // type (gift) == s
                        Receiver.GetMessageHandler().GetResponse().AppendInt32(1);
                        Receiver.GetMessageHandler().GetResponse().AppendUInt(u.Id);
                        Receiver.GetMessageHandler().SendResponse();
                        InventoryComponent targetInventory = Receiver.GetHabbo().GetInventoryComponent();
                        if (targetInventory != null)
                            targetInventory.RunDBUpdate();
                    }

                    Session.SendNotif(LanguageLocale.GetValue("catalog.gift.sent"));
                }
                else
                {
                    Session.GetMessageHandler().GetResponse().Init(Outgoing.SendPurchaseAlert);
                    Session.GetMessageHandler().GetResponse().AppendInt32(1); // items
                    int Type = 2;
                    if (Item.GetBaseItem(i).Type.ToString().ToLower().Equals("s"))
                    {
                        if (Item.GetBaseItem(i).InteractionType == InteractionType.pet)
                            Type = 3;
                        else
                            Type = 1;
                    }
                    Session.GetMessageHandler().GetResponse().AppendInt32(Type);
                    List<UserItem> items = DeliverItems(Session, Item.GetBaseItem(i), (buyAmount * Item.Amount), itemData.ToString(), Item.songID);
                    Session.GetMessageHandler().GetResponse().AppendInt32(items.Count);
                    foreach (UserItem u in items)
                        Session.GetMessageHandler().GetResponse().AppendUInt(u.Id);
                    Session.GetMessageHandler().SendResponse();
                    //Logging.WriteLine("Purchased " + items.Count);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);

                    if (Item.GetBaseItem(i).InteractionType == InteractionType.pet)
                    {
                        Session.SendMessage(Session.GetHabbo().GetInventoryComponent().SerializePetInventory());
                    }

                }
            }
        }

        internal void PurchaseGift(GameClient session, int pageID, int offerID, string extraParameter, string recipient, string message, int wrapSpriteID, int strappingID, int color, bool showIdentity)
        {
            // check if it's a special gift, or just the classic free one
            bool isBasicGift = (wrapSpriteID == 0 && strappingID == 0 && color == 0);


        }

        internal static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
            {
                return false;
            }

            if (!FirewindEnvironment.IsValidAlphaNumeric(PetName))
            {
                return false;
            }

            return true;
        }

        internal List<UserItem> DeliverItems(GameClient Session, Item Item, int Amount, String ExtraData, uint songID = 0)
        {
            List<UserItem> result = new List<UserItem>();
            switch (Item.Type.ToString())
            {
                case "i":
                case "s":
                    for (int i = 0; i < Amount; i++)
                    {
                        //uint GeneratedId = GenerateItemId();
                        switch (Item.InteractionType)
                        {
                            case InteractionType.pet:

                                //int petType = int.Parse(Item.InteractionType.ToString().Replace("pet", ""));
                                int petType = int.Parse(Item.Name.Substring(Item.Name.IndexOf(' ') + 4));
                                string[] PetData = ExtraData.Split('\n');

                                Pet GeneratedPet = CreatePet(Session.GetHabbo().Id, PetData[0], petType, PetData[1], PetData[2]);

                                Session.GetHabbo().GetInventoryComponent().AddPet(GeneratedPet);
                                result.Add(Session.GetHabbo().GetInventoryComponent().AddNewItem(0, 320, new StringData("0"), 0, true, false, 0));

                                break;

                            case InteractionType.teleport:

                                UserItem one = Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, new StringData("0"), 0, true, false, 0);
                                uint idOne = one.Id;
                                UserItem two = Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, new StringData("0"), 0, true, false, 0);
                                uint idTwo = two.Id;
                                result.Add(one);
                                result.Add(two);

                                using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                                {
                                    dbClient.runFastQuery("INSERT INTO items_tele_links (tele_one_id,tele_two_id) VALUES (" + idOne + "," + idTwo + ")");
                                    dbClient.runFastQuery("INSERT INTO items_tele_links (tele_one_id,tele_two_id) VALUES (" + idTwo + "," + idOne + ")");
                                }

                                break;

                            case InteractionType.dimmer:

                                UserItem it = Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, new StringData(ExtraData), 0, true, false, 0);
                                uint id = it.Id;
                                result.Add(it);
                                using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                                {
                                    dbClient.runFastQuery("INSERT INTO items_moodlight (item_id,enabled,current_preset,preset_one,preset_two,preset_three) VALUES (" + id + ",0,1,'#000000,255,0','#000000,255,0','#000000,255,0')");
                                }


                                break;

                            case InteractionType.musicdisc:
                                {
                                    result.Add(Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, new StringData(songID.ToString()), 0, true, false, songID));
                                    break;
                                }
                            case InteractionType.mannequin:
                                MapStuffData data = new MapStuffData();
                                data.Data.Add("OUTFIT_NAME", "");
                                data.Data.Add("FIGURE", "hr-515-33.hd-600-1.ch-635-70.lg-716-66-62.sh-735-68");
                                data.Data.Add("GENDER", "M");

                                result.Add(Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, data, 0, true, false, songID));
                                break;

                            default:

                                result.Add(Session.GetHabbo().GetInventoryComponent().AddNewItem(0, Item.ItemId, new StringData(ExtraData), 0, true, false, songID));
                                break;
                        }
                    }
                    return result;

                case "e":

                    for (int i = 0; i < Amount; i++)
                    {
                        Session.GetHabbo().GetAvatarEffectsInventoryComponent().AddEffect(Item.SpriteId, 3600);
                    }

                    return result;

                default:

                    Session.SendNotif(LanguageLocale.GetValue("catalog.buyerror"));
                    return result;
            }
        }

        internal static Pet CreatePet(uint UserId, string Name, int Type, string Race, string Color)
        {
            Pet pet = new Pet(404, UserId, 0, Name, (uint)Type, Race, Color, 0, 100, 100, 0, FirewindEnvironment.GetUnixTimestamp(), 0, 0, 0.0, false);
            pet.DBState = DatabaseUpdateState.NeedsUpdate;

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("INSERT INTO user_pets (user_id,name,type,race,color,expirience,energy,createstamp) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name," + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,'" + pet.CreationStamp + "')");
                dbClient.addParameter(pet.PetId + "name", pet.Name);
                dbClient.addParameter(pet.PetId + "race", pet.Race);
                dbClient.addParameter(pet.PetId + "color", pet.Color);
                pet.PetId = (uint)dbClient.insertQuery();
            }
            return pet;
        }

        internal static Pet GeneratePetFromRow(DataRow Row)
        {
            if (Row == null)
            {
                return null;
            }

            return new Pet(Convert.ToUInt32(Row["id"]), Convert.ToUInt32(Row["user_id"]), Convert.ToUInt32(Row["room_id"]), (string)Row["name"], Convert.ToUInt32(Row["type"]), (string)Row["race"], (string)Row["color"], (int)Row["expirience"], (int)Row["energy"], (int)Row["nutrition"], (int)Row["respect"], (double)Row["createstamp"], (int)Row["x"], (int)Row["y"], (double)Row["z"], (Convert.ToInt32(Row["have_saddle"])==1));
        }

        //internal Pet GeneratePetFromRow(DataRow Row, uint PetID)
        //{
        //    if (Row == null)
        //        return null;

        //    return new Pet(PetID, (uint)Row["user_id"], (uint)Row["room_id"], (string)Row["name"], (uint)Row["type"], (string)Row["race"], (string)Row["color"], (int)Row["expirience"], (int)Row["energy"], (int)Row["nutrition"], (int)Row["respect"], (double)Row["createstamp"], (int)Row["x"], (int)Row["y"], (double)Row["z"]);
        //}

        //internal uint GenerateItemId()
        //{
        //    //uint i = 0;

        //    //using (DatabaseClient dbClient = FirewindEnvironment.GetDatabase().GetClient())
        //    //{
        //    //    i = mCacheID++;
        //    //    dbClient.runFastQuery("UPDATE item_id_generator SET id_generator = '" + mCacheID + "' LIMIT 1");
        //    //}

        //    return mCacheID++;
        //}

        internal EcotronReward GetRandomEcotronReward()
        {
            uint Level = 1;

            if (FirewindEnvironment.GetRandomNumber(1, 2000) == 2000)
            {
                Level = 5;
            }
            else if (FirewindEnvironment.GetRandomNumber(1, 200) == 200)
            {
                Level = 4;
            }
            else if (FirewindEnvironment.GetRandomNumber(1, 40) == 40)
            {
                Level = 3;
            }
            else if (FirewindEnvironment.GetRandomNumber(1, 4) == 4)
            {
                Level = 2;
            }

            List<EcotronReward> PossibleRewards = GetEcotronRewardsForLevel(Level);

            if (PossibleRewards != null && PossibleRewards.Count >= 1)
            {
                return PossibleRewards[FirewindEnvironment.GetRandomNumber(0, (PossibleRewards.Count - 1))];
            }
            else
            {
                return new EcotronReward(0, 1479, 0); // eco lamp two :D
            }
        }

        internal List<EcotronReward> GetEcotronRewardsForLevel(uint Level)
        {
            List<EcotronReward> Rewards = new List<EcotronReward>();

            foreach (EcotronReward R in EcotronRewards)
            {
                if (R.RewardLevel == Level)
                {
                    Rewards.Add(R);
                }
            }


            return Rewards;
        }

        internal ServerMessage SerializeIndexForCache(int rank)
        {
            //ServerMessage Index = new ServerMessage(126);
            //Index.AppendBoolean(false);
            //Index.AppendInt32(0);
            //Index.AppendInt32(0);
            //Index.AppendInt32(-1);
            //Index.AppendString("");
            //Index.AppendBoolean(false);
            ServerMessage Index = new ServerMessage(Outgoing.OpenShop); //Fix for r61
            Index.AppendBoolean(true);
            Index.AppendInt32(0);
            Index.AppendInt32(0);
            Index.AppendInt32(-1);
            Index.AppendString("root");
            Index.AppendString("");
            Index.AppendInt32(GetTreeSize(rank, -1));

            foreach (CatalogPage Page in Pages.Values)
            {
                if (Page.ParentId != -1 || Page.MinRank > rank)
                    continue;

                Page.Serialize(rank, Index);

                foreach (CatalogPage _Page in Pages.Values)
                {
                    if (_Page.ParentId != Page.PageId)
                        continue;

                    _Page.Serialize(rank, Index);
                }
            }
            Index.AppendBoolean(false); // is updated
            return Index;
        }

        internal ServerMessage GetIndexMessageForRank(uint Rank)
        {
            if (Rank < 1)
                Rank = 1;
            if (Rank > 10)
                Rank = 10;

            return mCataIndexCache[Rank];
        }

        internal static ServerMessage SerializePage(CatalogPage Page)
        {
            ServerMessage PageData = new ServerMessage(Outgoing.OpenShopPage);
            PageData.AppendInt32(Page.PageId);

            switch (Page.Layout)
            {
                case "frontpage":

                    PageData.AppendString("frontpage3");
                    PageData.AppendInt32(2);
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    PageData.AppendString("catalog_club_headline1");
                    //}
                    //PageData.AppendInt32(7);
                    //for (int i = 0; i < 7; i++)
                    //{
                    //    PageData.AppendString("#FEFEFE");
                    //}
                    PageData.AppendString("Bundles_ts");
                    PageData.AppendString("");
                    PageData.AppendInt32(11);
                    PageData.AppendString("");
                    PageData.AppendString("");
                    PageData.AppendString("");
                    PageData.AppendString("How to get Habbo Credits");
                    PageData.AppendString("You can get Habbo Credits via Prepaid Cards, Home Phone, Credit Card, Mobile, completing offers and more! " + Convert.ToChar(13) + Convert.ToChar(10) + Convert.ToChar(13) + Convert.ToChar(10) + "To redeem your Habbo Credits, enter your voucher code below.");
                    PageData.AppendString(Page.TextDetails);
                    PageData.AppendString("");
                    PageData.AppendString("#FEFEFE");
                    PageData.AppendString("#FEFEFE");
                    PageData.AppendString(LanguageLocale.GetValue("catalog.waystogetcredits"));
                    PageData.AppendString("credits");
                    break;

                case "recycler_info":

                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendInt32(3);
                    PageData.AppendString(Page.Text1);
                    PageData.AppendString(Page.Text2);
                    PageData.AppendString(Page.TextDetails);

                    break;

                case "recycler_prizes":

                    // Ac@aArecycler_prizesIcatalog_recycler_headline3IDe Ecotron geeft altijd een van deze beloningen:H
                    PageData.AppendString("recycler_prizes");
                    PageData.AppendInt32(1);
                    PageData.AppendString("catalog_recycler_headline3");
                    PageData.AppendInt32(1);
                    PageData.AppendString(Page.Text1);

                    break;

                case "spaces_new":

                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(1);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendInt32(1);
                    PageData.AppendString(Page.Text1);

                    break;

                case "recycler":

                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendInt32(1);
                    PageData.AppendStringWithBreak(Page.Text1, 10);
                    PageData.AppendString(Page.Text2);
                    PageData.AppendString(Page.TextDetails);

                    break;

                case "trophies":

                    PageData.AppendString("trophies");
                    PageData.AppendInt32(1);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.Text1);
                    PageData.AppendString(Page.TextDetails);

                    break;

                case "pets":

                    PageData.AppendString("pets");
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendInt32(4);
                    PageData.AppendString(Page.Text1);
                    PageData.AppendString(LanguageLocale.GetValue("catalog.pickname"));
                    PageData.AppendString(LanguageLocale.GetValue("catalog.pickcolor"));
                    PageData.AppendString(LanguageLocale.GetValue("catalog.pickrace"));

                    break;

                case "soundmachine":

                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendInt32(2);
                    PageData.AppendString(Page.Text1);
                    PageData.AppendString(Page.TextDetails);
                    break;

                case "club_buy":

                    PageData.AppendString("vip_buy"); // layout
                    PageData.AppendInt32(2);
                    PageData.AppendString("ctlg_buy_vip_header");
                    PageData.AppendString("ctlg_gift_vip_teaser");
                    PageData.AppendInt32(0);
                    break;

                case "guild_frontpage":
                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(2);
                    PageData.AppendString("catalog_groups_en");
                    PageData.AppendString("");
                    PageData.AppendInt32(3);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendString(Page.LayoutSpecial);
                    PageData.AppendString(Page.Text1);
                    break;

                default:

                    PageData.AppendString(Page.Layout);
                    PageData.AppendInt32(3);
                    PageData.AppendString(Page.LayoutHeadline);
                    PageData.AppendString(Page.LayoutTeaser);
                    PageData.AppendString(Page.LayoutSpecial);
                    PageData.AppendInt32(3);
                    PageData.AppendString(Page.Text1);
                    PageData.AppendString(Page.TextDetails);
                    PageData.AppendString(Page.TextTeaser);

                    break;
            }

            if (!Page.Layout.Equals("frontpage") && !Page.Layout.Equals("club_buy"))
            {
                PageData.AppendInt32(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    Item.Serialize(PageData);
                }
            }
            else
                PageData.AppendInt32(0);
            PageData.AppendInt32(-1);
            PageData.AppendBoolean(false);

            return PageData;
        }

        //internal ServerMessage SerializeTestIndex()
        //{
        //    ServerMessage Message = new ServerMessage(126);

        //    Message.AppendInt32(0);
        //    Message.AppendInt32(0);
        //    Message.AppendInt32(0);
        //    Message.AppendInt32(-1);
        //    Message.AppendString("");
        //    Message.AppendInt32(0);
        //    Message.AppendInt32(100);

        //    for (int i = 1; i <= 150; i++)
        //    {
        //        Message.AppendInt32(1);
        //        Message.AppendInt32(i);
        //        Message.AppendInt32(i);
        //        Message.AppendInt32(i);
        //        Message.AppendString("#" + i);
        //        Message.AppendInt32(0);
        //        Message.AppendInt32(0);
        //    }

        //    return Message;

        //   
        //}

        //internal VoucherHandler GetVoucherHandler()
        //{
        //    return VoucherHandler;
        //}

        internal Marketplace GetMarketplace()
        {
            return Marketplace;
        }
    }
}
