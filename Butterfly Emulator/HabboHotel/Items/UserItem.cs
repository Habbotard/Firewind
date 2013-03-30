using System;
using Butterfly.Core;
using Butterfly.HabboHotel.Catalogs;
using Butterfly.Messages;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.HabboHotel.Items
{
    class UserItem
    {
        internal UInt32 Id;
        internal UInt32 BaseItem;
        internal IRoomItemData Data;
        internal int Extra;
        private Item mBaseItem;
        internal bool isWallItem;

        internal UserItem(UInt32 Id, UInt32 BaseItem, IRoomItemData data, int extra)
        {
            this.Id = Id;
            this.BaseItem = BaseItem;
            this.Data = data;
            this.Extra = extra;
            this.mBaseItem = GetBaseItem();
            if (mBaseItem == null)
            {
                Logging.WriteLine("Unknown baseItem ID: " + BaseItem);
                Logging.LogException("Unknown baseItem ID: " + BaseItem);
                return;
            }
            this.isWallItem = (mBaseItem.Type == 'i');
        }

        //internal void Serialize(ServerMessage Message)
        //{

        //    Message.AppendUInt(Id);
        //    Message.AppendInt32(0);
        //    if (mBaseItem == null)
        //        Logging.LogException("Unknown base: " + BaseItem);
        //    Message.AppendString(mBaseItem.Type.ToString().ToUpper());
        //    Message.AppendUInt(Id);
        //    Message.AppendInt32(mBaseItem.SpriteId);

        //    if (mBaseItem.Name.Contains("a2"))
        //        Message.AppendInt32(3);
        //    else if (mBaseItem.Name.Contains("wallpaper"))
        //        Message.AppendInt32(2);
        //    else if (mBaseItem.Name.Contains("landscape"))
        //        Message.AppendInt32(4);
        //    else
        //        Message.AppendInt32(0);

        //    Message.AppendString(ExtraData);
        //    Message.AppendBoolean(mBaseItem.AllowRecycle);
        //    Message.AppendBoolean(mBaseItem.AllowTrade);
        //    Message.AppendBoolean(mBaseItem.AllowInventoryStack);
        //    Message.AppendBoolean(Marketplace.CanSellItem(this));
        //    Message.AppendInt32(-1);

        //    if (mBaseItem.Type == 's')
        //    {
        //        Message.AppendString("");
        //        Message.AppendInt32(-1);
        //    }
        //}

        internal void SerializeWall(ServerMessage Message, Boolean Inventory)
        {
            Message.AppendUInt(Id);
            Message.AppendString(mBaseItem.Type.ToString().ToUpper());
            Message.AppendUInt(Id);
            Message.AppendInt32(GetBaseItem().SpriteId);

            if (GetBaseItem().Name.Contains("a2"))
            {
                Message.AppendInt32(3);
            }
            else if (GetBaseItem().Name.Contains("wallpaper"))
            {
                Message.AppendInt32(2);
            }
            else if (GetBaseItem().Name.Contains("landscape"))
            {
                Message.AppendInt32(4);
            }
            else
            {
                Message.AppendInt32(1);
            }
            int result = 0;
            //if (this.GetBaseItem().InteractionType == InteractionType.gift && ExtraData.Contains(Convert.ToChar(5).ToString()))
            //{
            //    int color = int.Parse(ExtraData.Split((char)5)[1]);
            //    int lazo = int.Parse(ExtraData.Split((char)5)[2]);
            //    result = color * 1000 + lazo;
            //}
            Message.AppendInt32(result);
            Message.AppendString(Data.ToString());
            Message.AppendBoolean(GetBaseItem().AllowRecycle);
            Message.AppendBoolean(GetBaseItem().AllowTrade);
            Message.AppendBoolean(GetBaseItem().AllowInventoryStack);
            Message.AppendBoolean(Marketplace.CanSellItem(this));
            Message.AppendInt32(-1);
        }

        internal void SerializeFloor(ServerMessage Message, Boolean Inventory)
        {
            Message.AppendUInt(Id);
            Message.AppendString(mBaseItem.Type.ToString().ToUpper());
            Message.AppendUInt(Id);
            Message.AppendInt32(GetBaseItem().SpriteId);
            Message.AppendInt32(Extra); // extra

            Message.AppendInt32(Data.GetType());
            Data.AppendToMessage(Message);

            Message.AppendBoolean(GetBaseItem().AllowRecycle);
            Message.AppendBoolean(GetBaseItem().AllowTrade);
            Message.AppendBoolean(GetBaseItem().AllowInventoryStack);
            Message.AppendBoolean(Marketplace.CanSellItem(this));
            Message.AppendInt32(-1);
            Message.AppendString("");
            Message.AppendInt32(0);
        }

        internal Item GetBaseItem()
        {
            return ButterflyEnvironment.GetGame().GetItemManager().GetItem(BaseItem);
        }
    }
}
