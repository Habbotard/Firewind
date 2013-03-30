using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Butterfly.Core;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.Messages;
using HabboEvents;
using Database_Manager.Database.Session_Details.Interfaces;

namespace Butterfly.HabboHotel.Rooms
{
    class Trade
    {
        private TradeUser[] Users;
        private int TradeStage;
        private UInt32 RoomId;

        private UInt32 oneId;
        private UInt32 twoId;

        internal Trade(UInt32 UserOneId, UInt32 UserTwoId, UInt32 RoomId)
        {
            this.oneId = UserOneId;
            this.twoId = UserTwoId;

            this.Users = new TradeUser[2];
            this.Users[0] = new TradeUser(UserOneId, RoomId);
            this.Users[1] = new TradeUser(UserTwoId, RoomId);
            this.TradeStage = 1;
            this.RoomId = RoomId;

            foreach (TradeUser User in Users)
            {
                if (!User.GetRoomUser().Statusses.ContainsKey("trd"))
                {
                    User.GetRoomUser().AddStatus("trd", "");
                    User.GetRoomUser().UpdateNeeded = true;
                }
            }

            ServerMessage Message = new ServerMessage(Outgoing.TradeStart);
            Message.AppendUInt(UserOneId);
            Message.AppendInt32(1); // ready
            Message.AppendUInt(UserTwoId);
            Message.AppendInt32(1); // ready
            SendMessageToUsers(Message);
        }

        internal bool AllUsersAccepted
        {
            get
            {
                for (int i = 0; i < Users.Length; i++)
                {
                    if (Users[i] == null)
                        continue;
                    if (!Users[i].HasAccepted)
                        return false;
                }

                return true;
            }
        }

        internal bool ContainsUser(UInt32 Id)
        {
            for (int i = 0; i < Users.Length; i++)
            {
                if (Users[i] == null)
                    continue;
                if (Users[i].UserId == Id)
                    return true;
            }

            return false;
        }

        internal TradeUser GetTradeUser(UInt32 Id)
        {
            for (int i = 0; i < Users.Length; i++)
            {
                if (Users[i] == null)
                    continue;
                if (Users[i].UserId == Id)
                    return Users[i];
            }

            return null;
        }

        internal void OfferItem(UInt32 UserId, UserItem Item)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || Item == null || !Item.GetBaseItem().AllowTrade || User.HasAccepted || TradeStage != 1)
            {
                return;
            }

            ClearAccepted();
            lock (User.OfferedItems)
            {
                if (!User.OfferedItems.Contains(Item))
                    User.OfferedItems.Add(Item);
            }
            UpdateTradeWindow();
        }

        internal void TakeBackItem(UInt32 UserId, UserItem Item)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || Item == null || User.HasAccepted || TradeStage != 1)
            {
                return;
            }

            ClearAccepted();

            lock (User.OfferedItems)
            {
                User.OfferedItems.Remove(Item);
            }
            UpdateTradeWindow();
        }

        internal void Accept(UInt32 UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 1)
            {
                return;
            }

            User.HasAccepted = true;

            ServerMessage Message = new ServerMessage(Outgoing.TradeAcceptUpdate);
            Message.AppendUInt(UserId);
            Message.AppendInt32(1);
            SendMessageToUsers(Message);

            if (AllUsersAccepted)
            {
                SendMessageToUsers(new ServerMessage(Outgoing.TradeComplete));
                TradeStage++;
                ClearAccepted();
            }
        }

        internal void Unaccept(UInt32 UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 1 || AllUsersAccepted)
            {
                return;
            }

            User.HasAccepted = false;

            ServerMessage Message = new ServerMessage(Outgoing.TradeAcceptUpdate);
            Message.AppendUInt(UserId);
            Message.AppendInt32(0);
            SendMessageToUsers(Message);
        }

        internal void CompleteTrade(UInt32 UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 2)
            {
                return;
            }

            User.HasAccepted = true;

            ServerMessage Message = new ServerMessage(Outgoing.TradeAcceptUpdate);
            Message.AppendUInt(UserId);
            Message.AppendInt32(1);
            SendMessageToUsers(Message);

            if (AllUsersAccepted)
            {
                TradeStage = 999;
                Finnito();
                //Task pTask = new Task(Finnito);
                //pTask.Start();
            }
        }

        private void Finnito()
        {
            try
            {
                DeliverItems();
                CloseTradeClean();
            }
            catch (Exception e)
            {
                Logging.LogThreadException(e.ToString(), "Trade task");
            }
        }

        internal void ClearAccepted()
        {
            foreach (TradeUser User in Users)
            {
                User.HasAccepted = false;
            }
        }

        internal void UpdateTradeWindow()
        {
            ServerMessage Message = new ServerMessage(Outgoing.TradeUpdate);

            for (int i = 0; i < Users.Length; i++)
            {
                TradeUser User = Users[i];
                if (User == null)
                    continue;

                lock (User.OfferedItems)
                {
                    Message.AppendUInt(User.UserId);
                    Message.AppendInt32(User.OfferedItems.Count);

                    foreach (UserItem Item in User.OfferedItems)
                    {
                        Message.AppendUInt(Item.Id);
                        Message.AppendString(Item.GetBaseItem().Type.ToString().ToLower());
                        Message.AppendUInt(Item.Id);
                        Message.AppendInt32(Item.GetBaseItem().SpriteId);
                        Message.AppendInt32(0); // undef
                        Message.AppendBoolean(true);
                        Message.AppendInt32(0);
                        Message.AppendString("");
                        Message.AppendInt32(0); // xmas 09 furni had a special furni tag here, with wired day (wat?)
                        Message.AppendInt32(0); // xmas 09 furni had a special furni tag here, wired month (wat?)
                        Message.AppendInt32(0); // xmas 09 furni had a special furni tag here, wired year (wat?)

                        if (Item.GetBaseItem().Type == 's')
                        {
                            Message.AppendInt32(0);
                        }
                    }
                }
            }


            SendMessageToUsers(Message);
        }

        internal void DeliverItems()
        {
            List<UserItem> offeredItems = this.GetTradeUser(this.oneId).OfferedItems;
            List<UserItem> list2 = this.GetTradeUser(this.twoId).OfferedItems;
            lock (offeredItems)
            {
                foreach (UserItem item in offeredItems)
                {
                    if (this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().GetItem(item.Id) == null)
                    {
                        this.GetTradeUser(this.oneId).GetClient().SendNotif(LanguageLocale.GetValue("trade.failed"));
                        this.GetTradeUser(this.twoId).GetClient().SendNotif(LanguageLocale.GetValue("trade.failed"));
                        return;
                    }
                }
            }
            lock (list2)
            {
                foreach (UserItem item in list2)
                {
                    if (this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().GetItem(item.Id) == null)
                    {
                        this.GetTradeUser(this.oneId).GetClient().SendNotif(LanguageLocale.GetValue("trade.failed"));
                        this.GetTradeUser(this.twoId).GetClient().SendNotif(LanguageLocale.GetValue("trade.failed"));
                        return;
                    }
                }
            }
            lock (offeredItems)
            {
                foreach (UserItem item in offeredItems)
                {
                    this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(item.Id, false);
                    this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().AddNewItem(item.Id, item.BaseItem, item.Data, item.Extra, false, false, 0);
                    using (IQueryAdapter adapter = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                    {
                        adapter.runFastQuery(string.Concat(new object[] { "UPDATE items_users SET user_id = ", this.twoId, " WHERE item_id = ", item.Id }));
                    }
                }
            }
            lock (list2)
            {
                foreach (UserItem item in list2)
                {
                    this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(item.Id, false);
                    this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().AddNewItem(item.Id, item.BaseItem, item.Data, item.Extra, false, false, 0);
                    using (IQueryAdapter adapter = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                    {
                        adapter.runFastQuery(string.Concat(new object[] { "UPDATE items_users SET user_id = ", this.oneId, " WHERE item_id = ", item.Id }));
                    }
                }
            }
            ServerMessage message = new ServerMessage(Outgoing.SendPurchaseAlert);
            message.AppendInt32(1);
            int i = 1;
            foreach (UserItem item2 in offeredItems)
            {
                if (item2.GetBaseItem().Type.ToString().ToLower() != "s")
                {
                    i = 2;
                }
            }
            message.AppendInt32(i);
            message.AppendInt32(offeredItems.Count);
            foreach (UserItem item2 in offeredItems)
            {
                message.AppendInt32(Convert.ToInt32(item2.Id));
            }
            this.GetTradeUser(this.twoId).GetClient().SendMessage(message);
            ServerMessage message2 = new ServerMessage(Outgoing.SendPurchaseAlert);
            message2.AppendInt32(1);
            i = 1;
            foreach (UserItem item2 in list2)
            {
                if (item2.GetBaseItem().Type.ToString().ToLower() != "s")
                {
                    i = 2;
                }
            }
            message2.AppendInt32(i);
            message2.AppendInt32(list2.Count);
            foreach (UserItem item2 in list2)
            {
                message2.AppendInt32(Convert.ToInt32(item2.Id));
            }
            this.GetTradeUser(this.oneId).GetClient().SendMessage(message2);
            this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().UpdateItems(false);
            this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().UpdateItems(false);
        }

        internal void CloseTradeClean()
        {
            for (int i = 0; i < Users.Length; i++)
            {
                TradeUser User = Users[i];
                if (User == null)
                    continue;
                if (User.GetRoomUser() == null)
                    continue;

                User.GetRoomUser().RemoveStatus("trd");
                User.GetRoomUser().UpdateNeeded = true;
            }

            SendMessageToUsers(new ServerMessage(Outgoing.TradeCloseClean));
            GetRoom().ActiveTrades.Remove(this);
        }

        internal void CloseTrade(UInt32 UserId)
        {
            for (int i = 0; i < Users.Length; i++)
            {
                TradeUser User = Users[i];
                if (User == null)
                    continue;
                if (User.GetRoomUser() == null)
                    continue;

                User.GetRoomUser().RemoveStatus("trd");
                User.GetRoomUser().UpdateNeeded = true;
            }

            ServerMessage Message = new ServerMessage(Outgoing.TradeClose);
            Message.AppendUInt(UserId);
            SendMessageToUsers(Message);
        }

        internal void SendMessageToUsers(ServerMessage Message)
        {
            if (Users == null)
                return;
            for (int i = 0; i < Users.Length; i++)
            {
                TradeUser User = Users[i];
                if (User == null)
                    continue;
                if (User != null)
                    if (User.GetClient() != null)
                        User.GetClient().SendMessage(Message);
            }
        }

        private Room GetRoom()
        {
            return ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
        }
    }

    class TradeUser
    {
        internal UInt32 UserId;
        private UInt32 RoomId;
        private bool Accepted;

        internal List<UserItem> OfferedItems;

        internal bool HasAccepted
        {
            get
            {
                return Accepted;
            }

            set
            {
                Accepted = value;
            }
        }

        internal TradeUser(UInt32 UserId, UInt32 RoomId)
        {
            this.UserId = UserId;
            this.RoomId = RoomId;
            this.Accepted = false;
            this.OfferedItems = new List<UserItem>();
        }

        internal RoomUser GetRoomUser()
        {
            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);

            if (Room == null)
            {
                return null;
            }
            
            return Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
        }

        internal GameClient GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
        }
    }
}
