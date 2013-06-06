using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Users;
using Firewind.Messages;
using HabboEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    public class RoomUser : RoomUnit
    {
        internal GameClient Client;

        internal bool IsDancing;
        public int DanceID;

        internal ItemEffectType CurrentItemEffect;
        public int CarryItemID;
        public int CarryTimer;

        // Random shit
        public double FlyCounter;
        public bool IsFlying;
        public int IdleTime;
        public bool IsSitting;
        public bool IsLaying;

        // User-mode games
        public Team Team;
        public bool NeedsAutokick;
        public int CurrentEffect;
        private Room room;
        public bool throwBallAtGoal;
        internal FreezePowerUp banzaiPowerUp;
        public bool Freezed;
        public int FreezeCounter;
        public bool shieldActive;
        public int shieldCounter;
        public int FreezeLives;
        public bool IsTrading;
        public bool moonwalkEnabled;

        internal override int GetTypeID()
        {
            // "USER" - 1
            return 1;
        }

        public RoomUser(int virtualID, GameClient client, Room room) : base(virtualID, room)
        {
            // Base constructor already called
            this.ID = client.GetHabbo().Id;
            this.Client = client;
        }

        internal override void Serialize(Messages.ServerMessage Message)
        {
            base.Serialize(Message);

            Habbo User = Client.GetHabbo();
            Message.AppendString(User.Gender.ToLower());

            Message.AppendInt32(0); // group ID
            Message.AppendInt32(0); // Looks like unused
            Message.AppendString(""); // groupName

            Message.AppendString(""); // botFigure
            Message.AppendInt32(User.AchievementPoints);
        }

        internal bool IsOwner()
        {
            return Name == GetRoom().Owner;
        }

        internal GameClient GetClient()
        {
            return Client;
        }

        internal void ApplyEffect(int effectID)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null || GetClient().GetHabbo().GetAvatarEffectsInventoryComponent() == null)
                return;

            GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(effectID);
        }

        internal void CarryItem(int Item)
        {
            this.CarryItemID = Item;

            if (Item > 0)
            {
                this.CarryTimer = 240;
            }
            else
            {
                this.CarryTimer = 0;
            }

            ServerMessage Message = new ServerMessage(Outgoing.ApplyCarryItem);
            Message.AppendInt32(VirtualID);
            Message.AppendInt32(Item);
            GetRoom().SendMessage(Message);
        }
        internal void Unidle()
        {
            this.IdleTime = 0;

            if (this.IsAsleep)
            {
                this.IsAsleep = false;

                ServerMessage Message = new ServerMessage(Outgoing.IdleStatus);
                Message.AppendInt32(VirtualID);
                Message.AppendBoolean(false);

                GetRoom().SendMessage(Message);
            }
        }

        internal void OnFly()
        {
            if (FlyCounter == 0)
            {
                FlyCounter++;
                return;
            }

            double lastK = 0.5 * Math.Sin(0.7 * FlyCounter);
            FlyCounter++;
            double nextK = 0.5 * Math.Sin(0.7 * FlyCounter);
            double differance = nextK - lastK;

            GetRoom().SendMessage(GetRoom().GetRoomItemHandler().UpdateUnitOnRoller(this, this.Coordinate, 0, this.Z + differance));
        }
    }
    internal enum ItemEffectType
    {
        None,
        Swim,
        SwimLow,
        SwimHalloween,
        Iceskates,
        Normalskates,
        PublicPool
        //Skateboard?
    }

    internal static class ByteToItemEffectEnum
    {
        internal static ItemEffectType Parse(byte pByte)
        {
            switch (pByte)
            {
                case 0:
                    return ItemEffectType.None;
                case 1:
                    return ItemEffectType.Swim;
                case 2:
                    return ItemEffectType.Normalskates;
                case 3:
                    return ItemEffectType.Iceskates;
                case 4:
                    return ItemEffectType.SwimLow;
                case 5:
                    return ItemEffectType.SwimHalloween;
                case 6:
                    return ItemEffectType.PublicPool;
                default:
                    return ItemEffectType.None;
            }
        }
    }
}
