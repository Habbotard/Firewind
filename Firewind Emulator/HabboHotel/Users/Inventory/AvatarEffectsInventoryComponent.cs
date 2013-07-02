using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Rooms;
using Firewind.Messages;
using Firewind.HabboHotel.Users.UserDataManagement;
using Database_Manager.Database.Session_Details.Interfaces;
using HabboEvents;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Users.Inventory
{
    class AvatarEffectsInventoryComponent
    {
        private ArrayList Effects;
        private int EffectCount;
        private int UserId;
        internal int CurrentEffect;
        private GameClient mClient;

        private Queue _removeQueue;

        internal int Count
        {
            get
            {
                return Effects.Count;
            }
        }

        internal AvatarEffectsInventoryComponent(int UserId, GameClient pClient, UserData data)
        {
            _removeQueue = new Queue();
            this.mClient = pClient;
            this.Effects = new ArrayList();
            this.UserId = UserId;
            this.CurrentEffect = -1;
            this.Effects.Clear();

            StringBuilder QueryBuilder = new StringBuilder();
            foreach (AvatarEffect effect in data.effects)
            {
                if (!effect.HasExpired)
                {
                    Effects.Add(effect);
                    EffectCount++;
                }
                else
                    QueryBuilder.Append("DELETE FROM user_effects WHERE user_id = " + UserId + " AND effect_id = " + effect.EffectId + "; ");
            }

            if (QueryBuilder.Length > 0)
            {
                using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                    dbClient.runFastQuery(QueryBuilder.ToString());
            }
        }

        internal void Dispose()
        {
            EffectCount = 0;
            Effects.Clear();
            mClient = null;
        }

        internal void AddEffect(int EffectId, int Duration)
        {
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("INSERT INTO user_effects (user_id,effect_id,total_duration,is_activated,activated_stamp) VALUES (" + UserId + "," + EffectId + "," + Duration + ",'0',0)");
            }

            EffectCount++;
            Effects.Add(new AvatarEffect(EffectId, Duration, false, 0));

            GetClient().GetMessageHandler().GetResponse().Init(Outgoing.AddEffectToInventary);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(EffectId);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(Duration);
            GetClient().GetMessageHandler().SendResponse();
        }

        internal void StopEffect(int EffectId)
        {
            // REMOVE EFFECT!!!
            AvatarEffect Effect = GetEffect(EffectId, true);

            if (Effect == null || !Effect.HasExpired)
            {
                return;
            }

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("DELETE FROM user_effects WHERE user_id = " + UserId + " AND effect_id = " + EffectId + " AND is_activated = '1'");
            }

            lock (_removeQueue.SyncRoot)
            {
                _removeQueue.Enqueue(Effect);
            }
            //Effects.Remove(Effect);
            //EffectCount--;

            GetClient().GetMessageHandler().GetResponse().Init(Outgoing.StopEffect);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(EffectId);
            GetClient().GetMessageHandler().SendResponse();

            if (CurrentEffect >= 0)
            {
                ApplyEffect(-1);
            }
        }

        internal void ApplyCustomEffect(int EffectId)
        {
            Room Room = GetUserRoom();

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(GetClient().GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            CurrentEffect = EffectId;

            ServerMessage Message = new ServerMessage(Outgoing.ApplyEffects);
            Message.AppendInt32(User.VirtualID);
            Message.AppendInt32(EffectId);
            Message.AppendInt32(0);
            Room.SendMessage(Message);
        }

        internal void ApplyEffect(int EffectId)
        {
            if (!HasEffect(EffectId, true))
            {
                return;
            }

            Room Room = GetUserRoom();

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(GetClient().GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            CurrentEffect = EffectId;

            ServerMessage Message = new ServerMessage(Outgoing.ApplyEffects);
            Message.AppendInt32(User.VirtualID);
            Message.AppendInt32(EffectId);
            Message.AppendInt32(0);
            Room.SendMessage(Message);
        }

        internal void EnableEffect(int EffectId)
        {
            AvatarEffect Effect = GetEffect(EffectId, false);

            if (Effect == null || Effect.HasExpired || Effect.Activated)
            {
                return;
            }

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("UPDATE user_effects SET is_activated = '1', activated_stamp = " + FirewindEnvironment.GetUnixTimestamp() + " WHERE user_id = " + UserId + " AND effect_id = " + EffectId + "");
            }

            Effect.Activate();

            GetClient().GetMessageHandler().GetResponse().Init(Outgoing.EnableEffect);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(Effect.EffectId);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(Effect.TotalDuration);
            GetClient().GetMessageHandler().SendResponse();
        }

        internal bool HasEffect(int EffectId, bool IfEnabledOnly)
        {
            if (EffectId == -1 || EffectId == 28 || EffectId == 29)
            {
                return true;
            }

            foreach (AvatarEffect Effect in Effects)
            {
                if (IfEnabledOnly && !Effect.Activated)
                {
                    continue;
                }

                if (Effect.HasExpired)
                {
                    continue;
                }

                if (Effect.EffectId == EffectId)
                {
                    return true;
                }
            }

            return false;
        }

        internal AvatarEffect GetEffect(int EffectId, bool IfEnabledOnly)
        {
            foreach (AvatarEffect Effect in Effects)
            {
                if (IfEnabledOnly && !Effect.Activated)
                {
                    continue;
                }

                if (Effect.EffectId == EffectId)
                {
                    return Effect;
                }
            }

            return null;
        }

        internal ServerMessage Serialize()
        {
            ServerMessage Message = new ServerMessage(Outgoing.EffectsInventary);
            Message.AppendInt32(Count);

            foreach (AvatarEffect Effect in Effects)
            {
                Message.AppendInt32(Effect.EffectId);
                Message.AppendInt32(Effect.TotalDuration);
                Message.AppendInt32(!Effect.Activated ? 1 : 0);
                Message.AppendInt32(Effect.TimeLeft);
            }

            return Message;
        }

        internal void CheckExpired()
        {
            if (Effects.Count <= 0)
                return;

            foreach (AvatarEffect Effect in Effects)
            {
                if (Effect.HasExpired)
                    StopEffect(Effect.EffectId);
            }
            HandleQueue();
        }
        internal void HandleQueue()
        {
            lock (_removeQueue.SyncRoot)
            {
                while (_removeQueue.Count > 0)
                {
                    AvatarEffect effect = (AvatarEffect)_removeQueue.Dequeue();
                    Effects.Remove(effect);
                    EffectCount--;
                }
            }
        }

        private GameClient GetClient()
        {
            return mClient;
        }

        private Room GetUserRoom()
        {
            return mClient.GetHabbo().CurrentRoom;
        }

        internal void OnRoomExit()
        {
            CurrentEffect = 0;
        }
    }
}
