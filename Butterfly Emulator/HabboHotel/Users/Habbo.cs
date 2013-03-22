using System;
using System.Collections.Generic;
using System.Data;
using Butterfly.Core;
using Butterfly.HabboHotel.Achievements;
using Butterfly.HabboHotel.ChatMessageStorage;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users.Badges;
using Butterfly.HabboHotel.Users.Inventory;
using Butterfly.HabboHotel.Users.Messenger;
using Butterfly.HabboHotel.Users.Subscriptions;
using Butterfly.Messages;
using Butterfly.ServerManager;
using Database_Manager.Database.Session_Details.Interfaces;
using Butterfly.HabboHotel.Users.UserDataManagement;
using Butterfly.HabboHotel.Groups;
using System.Collections;
using HabboEvents;

namespace Butterfly.HabboHotel.Users
{
    class Habbo
    {


        internal UInt32 Id;

        internal string Username;
        internal string RealName;

        internal int forceRot = -1;
        internal int buyItemLoop = 1;

        internal uint Rank;
        internal string Motto;

        internal string tempLook;
        internal string tempGender;

        internal string Look;
        internal string Gender;
        internal Int32 Credits;

        internal int AchievementPoints;
        internal Int32 ActivityPoints;
        internal Double LastActivityPointsUpdate;
        internal bool Muted;
        internal int Respect;

        internal int DailyRespectPoints;
        internal int DailyPetRespectPoints;

        internal uint LoadingRoom;
        internal Boolean LoadingChecksPassed;
        internal uint CurrentRoomId;
        internal uint HomeRoom;

        internal string LastOnline;

        internal bool IsTeleporting;
        internal uint TeleportingRoomID;
        internal UInt32 TeleporterId;

        internal ArrayList FavoriteRooms;
        internal List<UInt32> MutedUsers;
        internal List<string> Tags;
        internal Dictionary<string, UserAchievement> Achievements;
        internal List<uint> RatedRooms;

        private SubscriptionManager SubscriptionManager;
        private HabboMessenger Messenger;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        private AvatarEffectsInventoryComponent AvatarEffectsInventoryComponent;

        private ChatMessageManager chatMessageManager;

        private GameClient mClient;

        internal bool SpectatorMode;
        internal bool Disconnected;

        internal bool CalledGuideBot;
        internal bool MutantPenalty;

        internal bool HasFriendRequestsDisabled;

        internal List<RoomData> UsersRooms;

        internal bool spamProtectionBol = false;
        internal int spamProtectionCount = 1;
        internal int spamProtectionTime = 0;
        internal int spamProtectionAbuse = 0;
        internal DateTime spamFloodTime;

        //internal List<uint> CompletedQuests;
        internal Dictionary<uint, int> quests;
        internal uint CurrentQuestId;
        internal uint LastCompleted;
        internal int CurrentQuestProgress;
        internal uint LastQuestId;

        internal int LevelBuilder;
        internal int LevelSocial;
        internal int LevelIdentity;
        internal int LevelExplorer;

        internal int VipPoints;


        internal Boolean InRoom
        {
            get
            {
                if (CurrentRoomId >= 1)
                {
                    return true;
                }

                return false;
            }
        }

        internal Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                {
                    return null;
                }

                return ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(CurrentRoomId);
            }
        }

        internal Habbo(UInt32 Id, string Username, string RealName,
            uint Rank, string Motto, string Look, string Gender, Int32 Credits, Int32 VipPoints,
            Int32 ActivityPoints, Double LastActivityPointsUpdate, bool Muted,
            UInt32 HomeRoom, Int32 Respect, Int32 DailyRespectPoints, Int32 DailyPetRespectPoints,
            bool MutantPenalty, bool HasFriendRequestsDisabled, uint currentQuestID, int currentQuestProgress, DataRow groupRow, int achievementPoints,
            string LastOnline)
        {
            this.Id = Id;
            this.Username = Username;
            this.RealName = RealName;
            this.Rank = Rank;
            this.Motto = Motto;
            this.Look = ButterflyEnvironment.FilterFigure(Look.ToLower());

            this.Gender = Gender.ToLower();
            this.Credits = Credits;
            this.VipPoints = VipPoints;
            this.ActivityPoints = ActivityPoints;
            this.AchievementPoints = achievementPoints;
            this.LastActivityPointsUpdate = LastActivityPointsUpdate;
            this.Muted = Muted;
            this.LoadingRoom = 0;
            this.LoadingChecksPassed = false;
            this.CurrentRoomId = 0;
            this.HomeRoom = HomeRoom;
            this.FavoriteRooms = new ArrayList();
            this.MutedUsers = new List<uint>();
            this.Tags = new List<string>();
            this.Achievements = new Dictionary<string, UserAchievement>();
            this.RatedRooms = new List<uint>();
            this.Respect = Respect;
            this.DailyRespectPoints = DailyRespectPoints;
            this.DailyPetRespectPoints = DailyPetRespectPoints;
            this.CalledGuideBot = false;
            this.MutantPenalty = MutantPenalty;
            this.IsTeleporting = false;
            this.TeleporterId = 0;
            this.UsersRooms = new List<RoomData>();
            this.HasFriendRequestsDisabled = HasFriendRequestsDisabled;
            this.LastOnline = LastOnline;

            this.LastQuestId = 0;
            this.CurrentQuestId = currentQuestID;
            this.CurrentQuestProgress = currentQuestProgress;

        }

        internal void InitInformation(UserData data)
        {

            this.SubscriptionManager = new SubscriptionManager(Id, data);
            this.BadgeComponent = new BadgeComponent(Id, data);
            this.quests = data.quests;
            this.chatMessageManager = new ChatMessageManager();

            this.Messenger = new HabboMessenger(Id);
            this.Messenger.Init(data.friends, data.requests);

            this.SpectatorMode = false;
            this.Disconnected = false;
            this.UsersRooms = data.rooms;
        }

        internal void Init(GameClient client, UserData data)
        {
            this.mClient = client;

            this.SubscriptionManager = new SubscriptionManager(Id, data);
            this.BadgeComponent = new BadgeComponent(Id, data);
            this.InventoryComponent = InventoryGlobal.GetInventory(Id, client, data);
            this.InventoryComponent.SetActiveState(client);
            this.AvatarEffectsInventoryComponent = new AvatarEffectsInventoryComponent(Id, client, data);
            this.quests = data.quests;
            this.chatMessageManager = new ChatMessageManager();

            this.Messenger = new HabboMessenger(Id);
            this.Messenger.Init(data.friends, data.requests);

            this.SpectatorMode = false;
            this.Disconnected = false;
            this.UsersRooms = data.rooms;
        }

        //internal HabboData GetUserData
        //{
        //    get
        //    {
        //        return mUserData;
        //    }
        //}

        internal void UpdateRooms(IQueryAdapter dbClient)
        {
            UsersRooms.Clear();
            dbClient.setQuery("SELECT rooms.*, room_active.active_users FROM rooms LEFT JOIN room_active ON (room_active.roomid = rooms.id) WHERE owner = @name ORDER BY id ASC");
            dbClient.addParameter("name", Username);
            DataTable dbTable = dbClient.getTable();

            foreach (DataRow Row in dbTable.Rows)
                UsersRooms.Add(ButterflyEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToUInt32(Row["id"]), Row));
        }

        internal void LoadData(UserData data)
        {
            LoadAchievements(data.achievements);
            LoadFavorites(data.favouritedRooms);
            LoadMutedUsers(data.ignores);
            LoadTags(data.tags);
        }

        internal void SerializeQuests(ref QueuedServerMessage response)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().GetList(mClient, null);
        }

        internal bool HasFuse(string Fuse)
        {
            if (ButterflyEnvironment.GetGame().GetRoleManager().RankHasRight(Rank, Fuse))
            {
                return true;
            }

            foreach (string SubscriptionId in GetSubscriptionManager().SubList)
            {
                if (ButterflyEnvironment.GetGame().GetRoleManager().SubHasRight(SubscriptionId, Fuse))
                {
                    return true;
                }
            }

            return false;
        }

        internal void LoadFavorites(List<uint> roomID)
        {
            FavoriteRooms = new ArrayList();
            foreach (uint id in roomID)
            {
                FavoriteRooms.Add(id);
            }
        }

        internal void LoadMutedUsers(List<uint> usersMuted)
        {
            this.MutedUsers = usersMuted;
        }

        internal void LoadTags(List<string> tags)
        {
            this.Tags = tags;

            //if (Tags.Count >= 5)
            //    ButterflyEnvironment.GetGame().GetAchievementManager().UnlockAchievement(GetClient(), 7, 1);
        }

        internal void SerializeClub()
        {
            GameClient Client = GetClient();
            ServerMessage club = new ServerMessage(Outgoing.SerializeClub);
            club.AppendString("club_habbo");

            if (GetSubscriptionManager().HasSubscription("habbo_vip"))
            {
                Double Expire = GetSubscriptionManager().GetSubscription("habbo_vip").ExpireTime;
                Double TimeLeft = Expire - ButterflyEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                int MonthsLeft = TotalDaysLeft / 31;

                if (MonthsLeft >= 1) MonthsLeft--;

                club.AppendInt32(TotalDaysLeft - (MonthsLeft * 31));
                club.AppendInt32(2); // ??
                club.AppendInt32(MonthsLeft);
                club.AppendInt32(1); // type
                club.AppendBoolean(true);
                club.AppendBoolean(true);
                club.AppendInt32(0);
                club.AppendInt32(0); // days i have on hc
                club.AppendInt32(0); // days i have on vip
            }
            else
            {
                club.AppendInt32(0);
                club.AppendInt32(0); // ??
                club.AppendInt32(0);
                club.AppendInt32(0); // type
                club.AppendBoolean(false);
                club.AppendBoolean(true);
                club.AppendInt32(0);
                club.AppendInt32(0); // days i have on hc
                club.AppendInt32(0); // days i have on vip
            }

            Client.SendMessage(club);

            ServerMessage fuserights = new ServerMessage(Outgoing.Fuserights);
            if (GetSubscriptionManager().HasSubscription("habbo_vip")) // VIP 
                fuserights.AppendInt32(2);
            else if (GetSubscriptionManager().HasSubscription("habbo_club")) // HC
                fuserights.AppendInt32(1);
            else
                fuserights.AppendInt32(0);
            fuserights.AppendUInt(Rank);

            Client.SendMessage(fuserights);
        }

        internal void LoadAchievements(Dictionary<string, UserAchievement> achievements)
        {
            this.Achievements = achievements;
        }

        private bool HabboinfoSaved = false;

        internal string GetQueryString
        {
            get
            {
                HabboinfoSaved = true;
                return "UPDATE users SET users.last_online = '" + DateTime.Now.ToString() + "', activity_points = '" + ActivityPoints + "', vip_points = " + VipPoints + ", activity_points_lastupdate = '" + LastActivityPointsUpdate + "', credits = '" + Credits + "', achievement_points = " + AchievementPoints + " WHERE id = '" + Id + "'; "
                    + "DELETE FROM user_online WHERE userid = " + Id + "; ";
            }
        }

        internal void OnDisconnect()
        {

            if (this.Disconnected)
                return;

            this.Disconnected = true;

            ButterflyEnvironment.GetGame().GetClientManager().UnregisterClient(Id, Username);
            SessionManagement.IncreaseDisconnection();
            //Logging.WriteLine(Username + " has logged out.");


            if (!HabboinfoSaved)
            {
                HabboinfoSaved = true;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.runFastQuery("UPDATE users SET users.last_online = '" + DateTime.Now.ToString() + "', activity_points = " + ActivityPoints + ", vip_points = " + VipPoints + ", activity_points_lastupdate = '" + LastActivityPointsUpdate + "', credits = " + Credits + ", achievement_points = " + AchievementPoints + " WHERE id = " + Id + " ;");
                }
            }

            if (InRoom && CurrentRoom != null)
            {
                CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(mClient, false, false);
            }

            if (Messenger != null)
            {
                Messenger.AppearOffline = true;
                Messenger.Destroy();
            }

            if (SubscriptionManager != null)
                SubscriptionManager.Clear();

            if (AvatarEffectsInventoryComponent != null)
                AvatarEffectsInventoryComponent.Dispose();

            if (InventoryComponent != null)
            {
                InventoryComponent.SetIdleState();
                InventoryComponent.RunDBUpdate();
            }

            this.mClient = null;
        }

        internal void InitMessenger()
        {
            GameClient Client = GetClient();
            if (Client == null)
                return;
            Messenger.OnStatusChanged(false);

            Client.SendMessage(Messenger.SerializeFriends());
            Client.SendMessage(Messenger.SerializeRequests());

            // TODO: Change spanishfuck into locale message
            if (Messenger.requests.Count > ButterflyEnvironment.friendRequestLimit)
                Client.SendNotif("Tienes más de " + ButterflyEnvironment.friendRequestLimit + " peticiones de amigos, por lo que solo hemos cargado " + ButterflyEnvironment.friendRequestLimit + " y las próximas serán cargadas una vez aceptes estas y reinicies sesión!");
        }

        internal void UpdateCreditsBalance()
        {
            mClient.GetMessageHandler().GetResponse().Init(Outgoing.CreditsBalance);
            mClient.GetMessageHandler().GetResponse().AppendString(Credits + ".0");
            mClient.GetMessageHandler().SendResponse();
        }

        internal void UpdateActivityPointsBalance(Boolean InDatabase)
        {
            UpdateActivityPointsBalance(0);
        }

        internal void UpdateActivityPointsBalance(int NotifAmount)
        {
            if (mClient == null || mClient.GetMessageHandler() == null || mClient.GetMessageHandler().GetResponse() == null)
                return;

            mClient.GetMessageHandler().GetResponse().Init(Outgoing.ActivityPoints);
            mClient.GetMessageHandler().GetResponse().AppendInt32(2); // count
            mClient.GetMessageHandler().GetResponse().AppendInt32(0); // type
            mClient.GetMessageHandler().GetResponse().AppendInt32(ActivityPoints);
            mClient.GetMessageHandler().GetResponse().AppendInt32(103);
            mClient.GetMessageHandler().GetResponse().AppendInt32(VipPoints);
            mClient.GetMessageHandler().SendResponse();

        }

        internal void Mute()
        {
            if (!this.Muted)
            {
                GetClient().SendNotif(LanguageLocale.GetValue("moderation.muted"));
                this.Muted = true;
            }
        }

        internal void Unmute()
        {
            if (this.Muted)
            {
                GetClient().SendNotif(LanguageLocale.GetValue("moderation.unmuted"));
                this.Muted = false;
            }
        }

        private GameClient GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
        }

        internal SubscriptionManager GetSubscriptionManager()
        {
            return SubscriptionManager;
        }

        internal HabboMessenger GetMessenger()
        {
            return Messenger;
        }

        internal BadgeComponent GetBadgeComponent()
        {
            return BadgeComponent;
        }

        internal InventoryComponent GetInventoryComponent()
        {
            return InventoryComponent;
        }

        internal AvatarEffectsInventoryComponent GetAvatarEffectsInventoryComponent()
        {
            return AvatarEffectsInventoryComponent;
        }

        internal void RunDBUpdate(IQueryAdapter dbClient)
        {
            dbClient.runFastQuery("UPDATE users SET users.last_online = '" + DateTime.Now.ToString() + "', activity_points = '" + ActivityPoints + "', vip_points = " + VipPoints + ", activity_points_lastupdate = '" + LastActivityPointsUpdate + "', credits = '" + Credits + "' WHERE id = '" + Id + "'; ");
        }

        internal ChatMessageManager GetChatMessageManager()
        {
            return chatMessageManager;
        }

        internal void GiveUserCrystals(int p)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("UPDATE users SET crystals = crystals + " + p + " WHERE id = '" + Id + "'");
            }
        }

        internal int GetQuestProgress(uint p)
        {
            int progress = 0;
            quests.TryGetValue(p, out progress);
            return progress;
        }

        internal UserAchievement GetAchievementData(string p)
        {
            UserAchievement achievement = null;
            Achievements.TryGetValue(p, out achievement);
            return achievement;
        }
    }
}
