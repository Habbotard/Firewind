﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Firewind.Collections;
using Firewind.Core;
using Firewind.HabboHotel.Catalogs;
using Firewind.HabboHotel.ChatMessageStorage;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Pathfinding;
using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.RoomBots;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Rooms.RoomIvokedItems;
using Firewind.Messages;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.HabboHotel.Rooms.Wired;
using Firewind.HabboHotel.SoundMachine;
using System.Drawing;
using Firewind.HabboHotel.Groups;
using HabboEvents;
using Firewind.HabboHotel.Groups.Types;
using Firewind.HabboHotel.Misc;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Rooms
{
    

    public class Room
    {
        private UInt32 Id;
        internal string Name;
        internal string Description;
        internal string Owner;
        internal int OwnerId;
        internal string Password;
        internal int Category; //byte
        internal int State; //byte
        internal int UsersNow; //byte
        internal int UsersMax; //byte
        internal string ModelName;
        internal int Score; //short
        //internal List<string> Tags;
        private int tagCount;
        internal ArrayList Tags;
        internal bool AllowPets;
        internal bool AllowPetsEating;
        internal bool AllowWalkthrough;
        internal bool Hidewall;
        internal bool RoomMuted;
        internal int WallThickness;
        internal int FloorThickness;

        internal System.Threading.Tasks.Task procesoTask;
        internal bool procesoEnCurso = false;
        internal int procesoIntentos = 0;

        internal event RoomUserSaysDelegate OnUserSays;
        

        private Random rnd;
        private bool mCycleEnded;

        private int IdleTime; //byte

        internal TeamManager teambanzai;
        internal TeamManager teamfreeze;

        internal List<int> UsersWithRights;
        internal bool EveryoneGotRights;
        private Dictionary<int, Double> Bans;
        internal bool guideBotIsCalled;

        internal RoomEvent Event;

        internal string Wallpaper;
        internal string Floor;
        internal string Landscape;
        internal DateTime lastTimerReset;

        private GameManager game;
        private GameMap gamemap;
        private RoomItemHandling roomItemHandling;
        private RoomUnitManager roomUserManager;
        private Soccer soccer;
        private BattleBanzai banzai;
        private Freeze freeze;
        private GameItemHandler gameItemHandler;
        private WiredHandler wiredHandler;
        private RoomMusicController musicController;


        private Queue roomMessages;
        private Queue roomAlerts;
        private Queue roomBadge;
        private Queue roomKick;
        private Queue roomServerMessages;


        internal MoodlightData MoodlightData;
        internal bool isCycling = false;

        //internal SafeList<Trade> ActiveTrades;
        internal ArrayList ActiveTrades;
        
        internal bool mIsIdle = false;
        
        private ChatMessageManager chatMessageManager;
        private Queue chatMessageQueue;

        internal GameMap GetGameMap()
        {
            return gamemap;
        }

        internal RoomItemHandling GetRoomItemHandler()
        {
            return roomItemHandling;
        }

        internal RoomUnitManager GetRoomUserManager()
        {
            return roomUserManager;
        }

        internal Soccer GetSoccer()
        {
            if (soccer == null)
                soccer = new Soccer(this);
            return soccer;
        }

        internal TeamManager GetTeamManagerForBanzai()
        {
            if (teambanzai == null)
                teambanzai = TeamManager.createTeamforGame("banzai");
            return teambanzai;
        }

        internal TeamManager GetTeamManagerForFreeze()
        {
            if (teamfreeze == null)
                teamfreeze = TeamManager.createTeamforGame("freeze");
            return teamfreeze;
        }

        internal BattleBanzai GetBanzai()
        {
            if (banzai == null)
                banzai = new BattleBanzai(this);
            return banzai;
        }

        internal Freeze GetFreeze()
        {
            if (freeze == null)
                freeze = new Freeze(this);
            return freeze;
        }

        internal GameManager GetGameManager()
        {
            if (game == null)
                game = new GameManager(this);
            return game;
        }

        internal GameItemHandler GetGameItemHandler()
        {
            if (gameItemHandler == null)
                gameItemHandler = new GameItemHandler(this);
            return gameItemHandler;
        }

        internal RoomMusicController GetRoomMusicController()
        {
            if (musicController == null)
                musicController = new RoomMusicController();
            return musicController;
        }

        internal WiredHandler GetWiredHandler()
        {
            if (wiredHandler == null)
                wiredHandler = new WiredHandler(this);
            return wiredHandler;
        }

        internal bool GotMusicController()
        {
            return (musicController != null);
        }

        internal bool GotSoccer()
        {
            return (soccer != null);
        }

        internal bool GotBanzai()
        {
            return (banzai != null);
        }

        internal bool GotFreeze()
        {
            return (freeze != null);
        }

        internal bool GotWired()
        {
            return (wiredHandler != null);
        }

        internal Boolean HasOngoingEvent
        {
            get
            {
                if (Event != null)
                {
                    return true;
                }

                return false;
            }
        }

        internal Int32 UserCount
        {
            get
            {
                return roomUserManager.GetRoomUserCount();
            }
        }

        internal int TagCount
        {
            get
            {
                return Tags.Count;
            }
        }

        internal uint RoomId
        {
            get
            {
                return Id;
            }
        }
        
        internal Boolean CanTradeInRoom
        {
            get
            {
                return true;
            }
        }

        private RoomData mRoomData;

        internal RoomData RoomData
        {
            get
            {
                return mRoomData;
            }
        }

        internal ChatMessageManager GetChatMessageManager()
        {
            return chatMessageManager;
        }

        internal Room(RoomData Data)
        {
            InitializeFromRoomData(Data);
        }

        private void InitializeFromRoomData(RoomData Data)
        {
            Initialize(Data.Id, Data.Name, Data.Description, Data.Owner, Data.OwnerId, Data.Category, Data.State,
            Data.UsersMax, Data.ModelName, Data.Score, Data.Tags, Data.AllowPets, Data.AllowPetsEating,
            Data.AllowWalkthrough, Data.Hidewall, Data.Password, Data.Wallpaper, Data.Floor, Data.Landscape, Data, Data.AllowRightsOverride, Data.WallThickness, Data.FloorThickness, Data.Group);
        }

        private void Initialize(UInt32 Id, string Name, string Description, string Owner, int OwnerId, int Category,
            int State, int UsersMax, string ModelName, int Score, List<string> pTags, bool AllowPets,
            bool AllowPetsEating, bool AllowWalkthrough, bool Hidewall, string Password, string Wallpaper, string Floor,
            string Landscape, RoomData RoomData, bool RightOverride, int walltickness, int floorthickness, Group group)
        {

            this.mDisposed = false;
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.Owner = Owner;
            this.OwnerId = OwnerId;
            this.Category = Category;
            this.State = State;
            this.UsersNow = 0;
            this.UsersMax = UsersMax;
            this.ModelName = ModelName;
            this.Score = Score;

            tagCount = 0;
            this.Tags = new ArrayList();
            foreach (string tag in pTags)
            {
                tagCount++;
                Tags.Add(tag);
            }

            this.AllowPets = AllowPets;
            this.AllowPetsEating = AllowPetsEating;
            this.AllowWalkthrough = AllowWalkthrough;
            this.Hidewall = Hidewall;
            
            this.Password = Password;
            this.Bans = new Dictionary<int, double>();
            this.Wallpaper = Wallpaper;
            this.Floor = Floor;
            this.Landscape = Landscape;
            this.chatMessageManager = new ChatMessageManager();
            this.ActiveTrades = new ArrayList();


            this.mCycleEnded = false;
            
            this.mRoomData = RoomData;
            this.EveryoneGotRights = RightOverride;
            
            this.roomMessages = new Queue();
            this.chatMessageQueue = new Queue();
            this.rnd = new Random();

            this.roomMessages = new Queue();
            this.roomAlerts = new Queue();
            this.roomBadge = new Queue();
            this.roomKick = new Queue();
            this.roomServerMessages = new Queue();
            this.IdleTime = 0;
            this.RoomMuted = false;
            this.WallThickness = walltickness;
            this.FloorThickness = floorthickness;

            this.gamemap = new GameMap(this);
            this.roomItemHandling = new RoomItemHandling(this);
            this.roomUserManager = new RoomUnitManager(this);
            this.wiredHandler = new WiredHandler(this);

            this.Group = group;

            LoadRights();
            GetRoomItemHandler().LoadFurniture();
            wiredHandler.LoadWired();
            GetGameMap().GenerateMaps();
            LoadMusic();
            //LoadBots();
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("REPLACE INTO room_active VALUES (" + Id + ",1)");
            }

            FirewindEnvironment.GetGame().GetRoomManager().QueueActiveRoomAdd(mRoomData);
        }

        internal bool AllowsShous(RoomUser user, string message)
        {
            bool cycled = false;
            if (OnUserSays != null)
                OnUserSays(null, new UserSaysArgs(user, message), out cycled);

            
            return !cycled;
        }

        //private void LoadBots()
        //{
        //    List<RoomBot> bots = FirewindEnvironment.GetGame().GetBotManager().GetBotsForRoom(this.RoomId);
        //    foreach (RoomBot bot in bots)
        //    {
        //        RoomUser NewUser = DeployBot(bot);
        //        NewUser.SetPos(bot.X, bot.Y, bot.Z);
        //    }
        //}

        internal void ClearTags()
        {
            Tags.Clear();
            tagCount = 0;
        }

        internal void AddTagRange(List<string> tags)
        {
            tagCount += tags.Count;
            Tags.AddRange(tags);
        }

        //internal void InitBots()
        //{
        //    List<RoomBot> Bots = FirewindEnvironment.GetGame().GetBotManager().GetBotsForRoom(RoomId);

        //    foreach (RoomBot Bot in Bots)
        //    {
        //        DeployBot(Bot);
        //    }
        //}

        //internal void InitPets()
        //{
        //    using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
        //    {
        //        dbClient.setQuery("SELECT id, user_id, room_id, name, type, race, color, expirience, energy, nutrition, respect, createstamp, x, y, z, have_saddle FROM user_pets WHERE room_id = " + RoomId);
        //        DataTable Data = dbClient.getTable();

        //        if (Data == null)
        //            return;

        //        foreach (DataRow Row in Data.Rows)
        //        {
        //            Pet Pet = Catalog.GeneratePetFromRow(Row);
        //            List<RandomSpeech> RndSpeechList = new List<RandomSpeech>();
        //            List<BotResponse> BotResponse = new List<RoomBots.BotResponse>();
        //            //roomUserManager.DeployBot(new RoomBot(Pet.PetId, RoomId, AIType.Pet, "freeroam", Pet.Name, "", Pet.Look, Pet.X, Pet.Y, (int)Pet.Z, 0, 0, 0, 0, 0, ref RndSpeechList, ref BotResponse), Pet);
        //        }
        //    }
        //}

        //internal RoomUser DeployBot(RoomBot Bot)
        //{
        //    return roomUserManager.DeployBot(Bot, null);
        //}

        internal void QueueChatMessage(InvokedChatMessage message)
        {
            lock (chatMessageQueue.SyncRoot)
            {
                chatMessageQueue.Enqueue(message);
            }
        }

        internal void QueueRoomAlert(RoomAlert alert)
        {
            lock (roomAlerts.SyncRoot)
            {
                roomAlerts.Enqueue(alert);
            }
        }

        internal void QueueRoomKick(RoomKick kick)
        {
            lock (roomKick.SyncRoot)
            {
                roomKick.Enqueue(kick);
            }
        }

        private void WorkChatQueue()
        {
            if (chatMessageQueue.Count > 0)
            {
                lock (chatMessageQueue.SyncRoot)
                {
                    while (chatMessageQueue.Count > 0)
                    {
                        InvokedChatMessage message = (InvokedChatMessage)chatMessageQueue.Dequeue();
                        message.unit.OnChat(message);
                    }
                }
            }
        }

        private void WorkRoomKickQueue()
        {
            if (roomKick.Count > 0)
            {
                lock (roomKick.SyncRoot)
                {
                    while (roomKick.Count > 0)
                    {
                        RoomKick kick = (RoomKick)roomKick.Dequeue();


                        List<RoomUser> roomUsersToRemove = new List<RoomUser>();
                        foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                        {
                            RoomUser RoomUser = unit as RoomUser;
                            if (RoomUser == null || RoomUser.GetClient().GetHabbo().Rank >= kick.minrank)
                                continue;
                            if (kick.allert.Length > 0)
                                RoomUser.GetClient().SendNotif(LanguageLocale.GetValue("roomkick.allert") + kick.allert);

                            roomUsersToRemove.Add(RoomUser);
                            
                        }

                        foreach (RoomUser user in roomUsersToRemove)
                        {
                            GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                            user.GetClient().CurrentRoomUserID = -1;
                        }
                    }
                }
            }
        }

        private void WorkRoomAlertQueue()
        {
            if (roomAlerts.Count > 0)
            {
                lock (roomAlerts.SyncRoot)
                {
                    while (roomAlerts.Count > 0)
                    {
                        RoomAlert alert = (RoomAlert)roomAlerts.Dequeue();

                        foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                        {
                            RoomUser user = unit as RoomUser;
                            if (user == null || user.GetClient().GetHabbo().Rank >= alert.minrank)
                                continue;

                            user.GetClient().SendNotif(alert.message, false);
                        }

                    }
                }
            }
        }

        private void WorkRoomBadgeQueue()
        {
            if (roomBadge.Count > 0)
            {
                lock (roomBadge.SyncRoot)
                {
                    while (roomBadge.Count > 0)
                    {
                        string badge = (string)roomBadge.Dequeue();

                        foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                        {
                            RoomUser user = unit as RoomUser;
                            if (unit == null)
                                continue;
                            try
                            {
                                if (user.GetClient() != null && user.GetClient().GetHabbo() != null)
                                    user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(badge, true);
                            }
                            catch //(Exception e)
                            {
                                //Session.SendNotif(LanguageLocale.GetValue("roombadge.error") + e.ToString());
                            }
                        }
                    }
                }
            }
        }

        internal void QueueRoomBadge(string badge)
        {
            lock (roomBadge.SyncRoot)
            {
                roomBadge.Enqueue(badge);
            }
        }

        internal void QueueRoomMessage(ServerMessage message)
        {
            lock (roomServerMessages.SyncRoot)
            {
                roomServerMessages.Enqueue(message.GetBytes());
            }
        }

        internal void onRoomKick()
        {
            List<RoomUser> ToRemove = new List<RoomUser>();

            foreach (RoomUnit unit in roomUserManager.UnitList.Values)
            {
                RoomUser user = unit as RoomUser;
                if (user != null && user.GetClient().GetHabbo().Rank < 2)
                    ToRemove.Add(user);
            }

            for (int i = 0; i < ToRemove.Count; i++)
            {
                GetRoomUserManager().RemoveUserFromRoom(ToRemove[i].GetClient(), true, false);
                ToRemove[i].GetClient().CurrentRoomUserID = -1;
            }
        }



        internal void OnUserSay(RoomUser User, string Message, bool Shout)
        {
            foreach (RoomUnit unit in roomUserManager.UnitList.Values)
            {
                RoomAI bot = unit as RoomAI;
                if (bot == null)
                    continue;

                bot.BaseAI.OnUserChat(User, Message, Shout);
            }
        }

        internal void LoadMusic()
        {
            DataTable dTable;
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT items_rooms_songs.songid,items.* FROM items_rooms_songs LEFT JOIN items ON items.item_id = items_rooms_songs.itemid WHERE items_rooms_songs.roomid = " + this.RoomId);
                dTable = dbClient.getTable();
            }

            int songID;
            uint itemID;
            int baseID;

            foreach (DataRow dRow in dTable.Rows)
            {
                songID = (int)dRow[0];
                itemID = Convert.ToUInt32(dRow[1]);
                baseID = Convert.ToInt32(dRow[2]);

                SongItem item = new SongItem(itemID, songID, baseID);
                GetRoomMusicController().AddDisk(item);
            }
        }

        internal void LoadRights()
        {
            this.UsersWithRights = new List<int>();

            DataTable Data = new DataTable();

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT room_rights.user_id FROM room_rights WHERE room_id = " + Id);
                Data = dbClient.getTable();
            }

            if (Data == null)
                return;

            foreach (DataRow Row in Data.Rows)
            {
                this.UsersWithRights.Add(Convert.ToInt32(Row["user_id"]));
            }
        }

        internal int GetRightsLevel(GameClient Session)
        {
            try
            {
                if (Session == null || Session.GetHabbo() == null)
                    return 0;

                if (Session.GetHabbo().Username == Owner)
                {
                    return 4;
                }

                if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_any_room_controller"))
                {
                    return 4;
                }
                
                if (Session.GetHabbo().HasFuse("fuse_any_room_rights"))
                        return 3;
                
                if (UsersWithRights.Contains(Session.GetHabbo().Id))
                        return 1;
                
                if (EveryoneGotRights)
                        return 1;
               
            }
            catch (Exception e) { Logging.HandleException(e, "GetRightsLevel"); }

            return 0;
        }

        internal Boolean CheckRights(GameClient Session)
        {
            return CheckRights(Session, false);
        }

        internal Boolean CheckRights(GameClient Session, bool RequireOwnership)
        {
            try
            {
                if (Session == null || Session.GetHabbo() == null)
                    return false;

                if (Session.GetHabbo().Username == Owner)
                {
                    return true;
                }

                if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_any_room_controller"))
                {
                    return true;
                }

                if (!RequireOwnership)
                {
                    if (Session.GetHabbo().HasFuse("fuse_any_room_rights"))
                        return true;
                    if (UsersWithRights.Contains(Session.GetHabbo().Id))
                        return true;
                    if (EveryoneGotRights)
                        return true;
                }
            }
            catch (Exception e) { Logging.HandleException(e, "Room.CheckRights"); }

            return false;
        }


        private bool isCrashed = false;

        internal void ProcessRoom()
        {
            try
            {
                if (isCrashed || mDisposed)
                    return;
                try
                {
                    int idle = 0;
                    GetRoomItemHandler().OnCycle();
                    GetRoomUserManager().OnCycle(ref idle);

                    if (musicController != null)
                        musicController.Update(this);

                    if (idle > 0)
                    {
                        IdleTime++;
                    }
                    else
                    {
                        IdleTime = 0;
                    }

                    if (!mCycleEnded)
                    {
                        if (this.IdleTime >= 10)
                        {
                            FirewindEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
                            mIsIdle = false;
                            return;
                        }
                        else
                        {
                            ServerMessage Updates = GetRoomUserManager().SerializeStatusUpdates(false);

                            if (Updates != null)
                                SendMessage(Updates);
                        }
                    }
                    
                    if (gameItemHandler != null)
                        gameItemHandler.OnCycle();
                    if (game != null)
                        game.OnCycle();
                    if (GotBanzai())
                        banzai.OnCycle();
                    if (GotSoccer())
                        soccer.OnCycle();
                    if (wiredHandler != null)
                        wiredHandler.OnCycle();
                    
                    roomUserManager.UnitList.OnCycle();
                    WorkRoomAlertQueue();
                    WorkRoomBadgeQueue();
                    WorkRoomKickQueue();
                    WorkChatQueue();

                    WorkRoomServerMessageThread();

                    // Hidden license check here
                    if (FirewindEnvironment.GetRandomNumber(0, 750) == 100)
                    {
                        if (!AntiMutant.ValidateLook("", ""))
                        {
                            if (FirewindEnvironment.GetRandomNumber(0, 50) == 25)
                            {
                                Logging.LogCriticalException("Could not find main decrypted class!");
                                FirewindEnvironment.PreformShutDown();
                            }
                            throw new Exception(String.Format("Invalid byte specified after {0} in function {1}", 0x0FF, "CrackedEmulatorInit()"));
                        }
                    }
                }
                catch (Exception e)
                {
                    OnRoomCrash(e);
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Sub crash in room cycle: " + e.ToString());
            }
        }

        private void OnRoomCrash(Exception e)
        {
            Logging.LogThreadException(e.ToString(), "Room cycle task for room " + RoomId);


            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                    {
                        RoomUser user = unit as RoomUser;
                        if (user == null)
                            continue;
                        user.GetClient().SendNotif("Unhandled exception in room: " + e.ToString());
                        try
                        {
                            GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                            user.GetClient().CurrentRoomUserID = -1;
                        }
                        catch
                        { }
                    }
                }
            }
            catch { }

            FirewindEnvironment.GetGame().GetRoomManager().UnloadRoom(this);

            isCrashed = true;
        }

        #region Communication
        internal void SendMessage(ServerMessage Message)
        {
            try
            {
                if (Message == null)
                    return;
                //Logging.WriteLine("Sended message to all people: " + FirewindEnvironment.GetDefaultEncoding().GetString(Message.GetBytes()).Replace(Convert.ToChar(0).ToString(), "{char0}"));
                byte[] PacketData = Message.GetBytes();

                lock (roomServerMessages.SyncRoot)
                {
                    roomServerMessages.Enqueue(PacketData);
                }
            }
            catch (InvalidOperationException e) { Logging.HandleException(e, "Room.SendMessage"); }
        }

        internal void SendMessage(List<ServerMessage> Messages)
        {
            if (Messages.Count == 0)
                return;

            try
            {
                byte[] totalBytes = new byte[0];
                int currentWorking = 0;
                //List<byte[]> PacketData = new List<byte[]>();

                foreach (ServerMessage Message in Messages)
                {
                    byte[] toAppend = Message.GetBytes();
                    int newLength = totalBytes.Length + toAppend.Length;

                    Array.Resize(ref totalBytes, newLength);
                    for (int i = 0; i < toAppend.Length; i++)
                    {
                        totalBytes[currentWorking] = toAppend[i];
                        currentWorking++;
                    }

                    //if (Message != null)
                    //    PacketData.Add(Message.GetBytes());
                }

                lock (roomServerMessages.SyncRoot)
                {
                    roomServerMessages.Enqueue(totalBytes);
                }
            }
            catch (Exception e) { Logging.HandleException(e, "Room.SendMessage List<ServerMessage>"); }
        }

        private void WorkRoomServerMessageThread()
        {
            if (roomServerMessages.Count > 0)
            {
                List<byte> totalBytes = new List<byte>();

                lock (roomServerMessages.SyncRoot)
                {
                    while (roomServerMessages.Count > 0)
                    {
                        totalBytes.AddRange((byte[])roomServerMessages.Dequeue());
                    }
                }

                byte[] encodedPackets = totalBytes.ToArray();
                foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                {
                    RoomUser user = unit as RoomUser;
                    if (user == null)
                        continue;
                    GameClient UsersClient = user.GetClient();
                    if (UsersClient == null || UsersClient.GetConnection() == null)
                        continue;

                    UsersClient.GetConnection().SendData(encodedPackets);
                }

                totalBytes.Clear();
                encodedPackets = null;
                totalBytes = null;
            }
        }

        internal void SendMessageToUsersWithRights(ServerMessage Message)
        {
            try
            {
                byte[] PacketData = Message.GetBytes();

                foreach (RoomUnit unit in roomUserManager.UnitList.Values)
                {
                    RoomUser user = unit as RoomUser;
                    if (user == null)
                        continue;

                    GameClient UsersClient = user.GetClient();
                    if (UsersClient == null)
                        continue;

                    if (!CheckRights(UsersClient))
                        continue;

                    try
                    {
                        UsersClient.GetConnection().SendData(PacketData);
                    }
                    catch (Exception e) { Logging.HandleException(e, "Room.SendMessageToUsersWithRights"); }
                    //User.GetClient().SendMessage(Message);

                }
            }
            catch (Exception e) { Logging.HandleException(e, "Room.SendMessageToUsersWithRights"); }
        }
        #endregion
        internal void Destroy()
        {
            SendMessage(new ServerMessage(Outgoing.OutOfRoom));
            Dispose();
        }

        private bool mDisposed;
        private Group Group;

        #region IDisposable members

        private void Dispose()
        {
            if (!this.mDisposed)
            {
                mDisposed = true;
                mCycleEnded = true;
                //FirewindEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
                FirewindEnvironment.GetGame().GetRoomManager().QueueActiveRoomRemove(mRoomData);

                using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    GetRoomItemHandler().SaveFurniture(dbClient);
                    dbClient.runFastQuery("DELETE FROM room_active WHERE roomid = " + Id);
                }
                WorkRoomServerMessageThread();
                tagCount = 0;
                Tags.Clear();
                roomUserManager.UnitList.Clear();
                UsersWithRights.Clear();
                Bans.Clear();

                foreach (RoomItem item in GetRoomItemHandler().mFloorItems.Values)
                {
                    item.Destroy();
                }

                foreach (RoomItem item in GetRoomItemHandler().mWallItems.Values)
                {
                    item.Destroy();
                }
                ActiveTrades.Clear();

                //Tags = null;
                //OnUserSays = null;
                //rnd = null;
                //UsersWithRights = null;
                //Bans = null;
                //Event = null;

                //if (game != null)
                //    game.Destroy();
                //game = null;

                //if (gamemap != null)
                //    gamemap.Destroy();
                //gamemap = null;

                //if (roomItemHandling != null)
                //    roomItemHandling.Destroy();
                //roomItemHandling = null;

                //if (roomUserManager != null)
                //    roomUserManager.Destroy();
                //roomUserManager = null;

                //if (soccer != null)
                //    soccer.Destroy();
                //soccer = null;

                //if (banzai != null)
                //    banzai.Destroy();
                //banzai = null;

                //if (freeze != null)
                //    freeze.Destroy();
                //freeze = null;

                //if (gameItemHandler != null)
                //    gameItemHandler.Destroy();
                //gameItemHandler = null;

                //if (wiredHandler != null)
                //    wiredHandler.Destroy();
                //wiredHandler = null;

                //if (musicController != null)
                //    musicController.Destroy();
                //musicController = null;

                //roomMessages.Clear();
                //roomAlerts.Clear();
                //roomBadge.Clear();
                //roomKick.Clear();
                //roomServerMessages.Clear();
                //ActiveTrades.Clear();
                //chatMessageQueue.Clear();

                //roomMessages = null;
                //roomAlerts = null;
                //roomBadge = null;
                //roomKick = null;
                //roomServerMessages = null;
                //ActiveTrades = null;
                //chatMessageQueue = null;
                //MoodlightData = null;

                if (HasOngoingEvent)
                    FirewindEnvironment.GetGame().GetRoomManager().GetEventManager().QueueRemoveEvent(mRoomData, Event.Category);
            }
        }
        #endregion

        #region Room Bans
        internal Boolean UserIsBanned(int pId)
        {
            return Bans.ContainsKey(pId);
        }

        internal void RemoveBan(int pId)
        {
            Bans.Remove(pId);
        }

        internal void AddBan(int pId)
        {
            if (!Bans.ContainsKey(pId))
                Bans.Add(pId, FirewindEnvironment.GetUnixTimestamp());
        }

        internal Boolean HasBanExpired(int pId)
        {
            if (!UserIsBanned(pId))
                return true;

            Double diff = FirewindEnvironment.GetUnixTimestamp() - Bans[pId];

            if (diff > 900)
                return true;

            return false;
        }
        #endregion

        #region Trading
        internal bool HasActiveTrade(RoomUser User)
        {
            return HasActiveTrade(User.GetClient().GetHabbo().Id);
        }

        internal bool HasActiveTrade(int UserId)
        {
            foreach (Trade Trade in ActiveTrades.ToArray())
                if (Trade.ContainsUser(UserId))
                    return true;

            return false;
        }

        internal Trade GetUserTrade(int UserId)
        {
            foreach (Trade Trade in ActiveTrades.ToArray())
            {
                if (Trade.ContainsUser(UserId))
                {
                    return Trade;
                }
            }

            return null;
        }

        internal void TryStartTrade(RoomUser UserOne, RoomUser UserTwo)
        {
            if (UserOne == null || UserTwo == null || UserOne.IsTrading || UserTwo.IsTrading || HasActiveTrade(UserOne) || HasActiveTrade(UserTwo))
                return;

            ActiveTrades.Add(new Trade(UserOne.GetClient().GetHabbo().Id, UserTwo.GetClient().GetHabbo().Id, RoomId));
        }

        internal void TryStopTrade(int UserId)
        {
            Trade Trade = GetUserTrade(UserId);

            if (Trade == null)
                return;

            Trade.CloseTrade(UserId);
            ActiveTrades.Remove(Trade);
        }
        #endregion

        //This function is based on the one from "Holograph Emulator" (Wich sucks ASS!)
        //internal static string WallPositionCheck(string wallPosition)
        //{
        //    //:w=3,2 l=9,63 l
        //    try
        //    {
        //        if (wallPosition.Contains(Convert.ToChar(13)))
        //        { return null; }
        //        if (wallPosition.Contains(Convert.ToChar(9)))
        //        { return null; }

        //        string[] posD = wallPosition.Split(' ');
        //        if (posD[2] != "l" && posD[2] != "r")
        //            return null;

        //        string[] widD = posD[0].Substring(3).Split(',');
        //        int widthX = int.Parse(widD[0]);
        //        int widthY = int.Parse(widD[1]);

        //        string[] lenD = posD[1].Substring(2).Split(',');
        //        int lengthX = int.Parse(lenD[0]);
        //        int lengthY = int.Parse(lenD[1]);

        //        return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
        //    }
        //    catch (Exception e)
        //    {
        //        Logging.HandleException(e, "Room.WallPositionCheck");
        //        return null;
        //    }
        //}



        internal void SetMaxUsers(int MaxUsers)
        {
            this.UsersMax = MaxUsers;
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                dbClient.runFastQuery("UPDATE rooms SET users_max = " + MaxUsers + " WHERE id = " + RoomId);
        }

        internal void FlushSettings()
        {
            List<ServerMessage> messages = new List<ServerMessage>();

            lock (GetRoomItemHandler().mFloorItems)
            {
                foreach (RoomItem Item in GetRoomItemHandler().mFloorItems.Values)
                {
                    ServerMessage Message = new ServerMessage(94);
                    Message.AppendRawUInt(Item.Id);
                    Message.AppendStringWithBreak("");
                    Message.AppendBoolean(false);
                    messages.Add(Message);
                }
            }


            lock (GetRoomItemHandler().mWallItems)
            {
                foreach (RoomItem Item in GetRoomItemHandler().mWallItems.Values)
                {
                    ServerMessage Message = new ServerMessage(84);
                    Message.AppendRawUInt(Item.Id);
                    Message.AppendStringWithBreak("");
                    Message.AppendBoolean(false);
                    messages.Add(Message);
                }
            }

            SendMessage(messages);


            mCycleEnded = true;
            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                GetRoomItemHandler().SaveFurniture(dbClient);

            Tags.Clear();
            UsersWithRights.Clear();
            Bans.Clear();
            ActiveTrades.Clear();


            if (GotFreeze())
                freeze = new Freeze(this);
            if (GotBanzai())
                banzai = new BattleBanzai(this);
            if (GotSoccer())
                soccer = new Soccer(this);
            if (gameItemHandler != null)
                gameItemHandler = new GameItemHandler(this);
        }

        internal void ReloadSettings()
        {
            RoomData data = FirewindEnvironment.GetGame().GetRoomManager().GenerateRoomData(this.RoomId);
            InitializeFromRoomData(data);
            //InitBots();
            //InitPets();
        }

        internal void UpdateFurniture()
        {
            List<ServerMessage> messages = new List<ServerMessage>();
            RoomItem[] items = GetRoomItemHandler().mFloorItems.Values.ToArray();
            foreach (RoomItem item in items) // Toarray
            {
                ServerMessage Message = new ServerMessage(93);
                item.Serialize(Message, this.OwnerId);
                messages.Add(Message);
            }
            Array.Clear(items, 0, items.Length);
            items = null;


            RoomItem[] wallItems = GetRoomItemHandler().mWallItems.Values.ToArray();

            foreach (RoomItem item in wallItems)
            {
                ServerMessage Message = new ServerMessage(83);
                item.Serialize(Message, this.OwnerId);
                messages.Add(Message);
            }
            Array.Clear(wallItems, 0, wallItems.Length);
            wallItems = null;

            SendMessage(messages);
        }
    }
}
