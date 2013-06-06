using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Firewind.Collections;
using Firewind.Core;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Pathfinding;
using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.RoomBots;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Users.Inventory;
using Firewind.Messages;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.Util;
using System.Diagnostics;
using Firewind.HabboHotel.Groups;
using HabboEvents;
using Firewind.HabboHotel.Rooms.Units;


namespace Firewind.HabboHotel.Rooms
{
    class RoomUnitManager
    {
        private Room room;

        internal Hashtable usersByUserID;
        internal event RoomEventDelegate OnUserEnter;

        private QueuedDictionary<int, RoomUnit> _unitList;
        //internal int RoomUserCounter;

        private int petCount;
        private int userCount;

        private int primaryPrivateUserID;
        private int secondaryPrivateUserID;


        internal int PetCount
        {
            get
            {
                return petCount;
            }
        }

        internal QueuedDictionary<int, RoomUnit> UnitList
        {
            get
            {
                return _unitList;
            }
        }

        internal int GetRoomUserCount()
        {
            return _unitList.Inner.Count;
        }

        public RoomUnitManager(Room room)
        {
            //this.RoomUserCounter = 0;
            this.room = room;
            this._unitList = new QueuedDictionary<int, RoomUnit>(new EventHandler(OnUserAdd), null, new EventHandler(onRemove), null);

            this.usersByUserID = new Hashtable();
            this.primaryPrivateUserID = 0;
            this.secondaryPrivateUserID = 0;
            this.ToRemove = new List<RoomUser>(room.UsersMax);

            this.petCount = 0;
            this.userCount = 0;
        }

        //internal RoomUser DeployBot(RoomAI Bot)
        //{
        //    RoomUser BotUser = new RoomUser(0, room.RoomId, primaryPrivateUserID++, room, false);
        //    int PersonalID = secondaryPrivateUserID++;
        //    BotUser.InternalRoomID = PersonalID;
        //    //this.UserList[PersonalID] = BotUser;
        //    userlist.Add(PersonalID, BotUser);
        //    DynamicRoomModel Model = room.GetGameMap().Model;

        //    if ((Bot.X > 0 && Bot.Y > 0) && Bot.X < Model.MapSizeX && Bot.Y < Model.MapSizeY)
        //    {
        //        BotUser.SetPos(Bot.X, Bot.Y, Bot.Z);
        //        BotUser.SetRot(Bot.Rot, false);
        //    }
        //    else
        //    {
        //        Bot.X = Model.DoorX;
        //        Bot.Y = Model.DoorY;

        //        BotUser.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
        //        BotUser.SetRot(Model.DoorOrientation, false);
        //    }

        //    BotUser.BotData = Bot;
        //    BotUser.BotAI = Bot.GenerateBotAI(BotUser.VirtualId);

        //    if (BotUser.IsPet)
        //    {


        //        BotUser.BotAI.Init((int)Bot.BotId, BotUser.VirtualId, room.RoomId, BotUser, room);
        //        BotUser.PetData = PetData;
        //        BotUser.PetData.VirtualId = BotUser.VirtualId;
        //    }
        //    else
        //    {
        //        BotUser.BotAI.Init(-1, BotUser.VirtualId, room.RoomId, BotUser, room);
        //    }

        //    UpdateUserStatus(BotUser, false);
        //    BotUser.UpdateNeeded = true;

        //    ServerMessage EnterMessage = new ServerMessage(Outgoing.PlaceBot);
        //    EnterMessage.AppendInt32(1);
        //    BotUser.Serialize(EnterMessage);
        //    room.SendMessage(EnterMessage);

        //    BotUser.BotAI.OnSelfEnterRoom();

        //    if (BotUser.BotData.AiType == AIType.Guide)
        //        room.guideBotIsCalled = true;
        //    if (BotUser.IsPet)
        //    {
        //        if (pets.ContainsKey(BotUser.PetData.PetId)) //Pet allready placed
        //            pets[BotUser.PetData.PetId] = BotUser;
        //        else
        //            pets.Add(BotUser.PetData.PetId, BotUser);

        //        petCount++;
        //    }

        //    return BotUser;
        //}

        //internal void RemoveBot(int VirtualId, bool Kicked)
        //{
        //    RoomUser User = GetRoomUserByVirtualId(VirtualId);

        //    if (User == null || !User.IsBot)
        //    {
        //        return;
        //    }

        //    if (User.IsPet)
        //    {
        //        pets.Remove(User.PetData.PetId);
        //        petCount--;
        //    }

        //    User.BotAI.OnSelfLeaveRoom(Kicked);

        //    ServerMessage LeaveMessage = new ServerMessage(Outgoing.UserLeftRoom);
        //    LeaveMessage.AppendRawInt32(User.VirtualId);
        //    room.SendMessage(LeaveMessage);

        //    userlist.Remove(User.InternalRoomID);
        //    //freeIDs[User.InternalRoomID] = null;
        //}


        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            byte NewCurrentUserItemEffect = room.GetGameMap().EffectMap[x, y];
            if (NewCurrentUserItemEffect > 0)
            {
                ItemEffectType Type = ByteToItemEffectEnum.Parse(NewCurrentUserItemEffect);
                if (Type != User.CurrentItemEffect)
                {
                    switch (Type)
                    {
                        case ItemEffectType.Iceskates:
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                    User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(38);
                                else
                                    User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(39);
                                User.CurrentItemEffect = ItemEffectType.Iceskates;
                                break;
                            }

                        case ItemEffectType.Normalskates:
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                {
                                    User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(55);
                                }
                                else
                                {
                                    User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(56);
                                }
                                //56=girls
                                //55=
                                User.CurrentItemEffect = Type;
                                break;
                            }
                        case ItemEffectType.Swim:
                            {
                                User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(29);
                                User.CurrentItemEffect = Type;
                                break;
                            }
                        case ItemEffectType.SwimLow:
                            {
                                User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(30);
                                User.CurrentItemEffect = Type;
                                break;
                            }
                        case ItemEffectType.SwimHalloween:
                            {
                                User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(37);
                                User.CurrentItemEffect = Type;
                                break;
                            }
                        case ItemEffectType.None:
                            {
                                User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyCustomEffect(-1);
                                User.CurrentItemEffect = Type;
                                break;
                            }
                        case ItemEffectType.PublicPool:
                            {
                                User.AddStatus("swim", string.Empty);
                                User.CurrentItemEffect = Type;
                                break;
                            }

                    }
                }
            }
            else if (User.CurrentItemEffect != ItemEffectType.None && NewCurrentUserItemEffect == 0)
            {
                User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().ApplyEffect(-1);
                User.CurrentItemEffect = ItemEffectType.None;
                User.RemoveStatus("swim");
            }
        }

        internal RoomUnit GetUnitForSquare(int x, int y)
        {
            return room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault();
        }

        internal void AddUserToRoom(GameClient Session)
        {
            RoomUser User = new RoomUser(primaryPrivateUserID++, Session, room);
            Users.Habbo loadingHabbo = User.GetClient().GetHabbo();

            if (loadingHabbo.spamProtectionBol == true)
            {
                TimeSpan SinceLastMessage = DateTime.Now - loadingHabbo.spamFloodTime;
                 if (SinceLastMessage.TotalSeconds < loadingHabbo.spamProtectionTime) {
                     ServerMessage Packet = new ServerMessage(Outgoing.FloodFilter);
                     int timeToWait = loadingHabbo.spamProtectionTime - SinceLastMessage.Seconds;
                     Packet.AppendInt32(timeToWait); //Blocked for X sec
                     User.GetClient().SendMessage(Packet);
                 }

            }
            User.ID = (int)Session.GetHabbo().Id;

            string username = Session.GetHabbo().Username;
            uint userID = (uint)User.ID;

            if (usersByUserID.ContainsKey(userID))
                usersByUserID.Remove(userID);

            usersByUserID.Add(Session.GetHabbo().Id, User);
            Session.CurrentRoomUserID = User.VirtualID;

            Session.GetHabbo().CurrentRoomId = room.RoomId;
            UnitList.Add(User.VirtualID, User);
        }

        private void OnUserAdd(object sender, EventArgs args)
        {
            try
            {
                KeyValuePair<int, RoomUnit> userPair = (KeyValuePair<int, RoomUnit>)sender;
                RoomUser user = userPair.Value as RoomUser;

                if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                    return;

                GameClient session = user.GetClient();

                if (session == null || session.GetHabbo() == null)
                    return;

                //if (userCount >= room.UsersMax && user.GetClient().GetHabbo().Rank < 4)
                //{
                //    ServerMessage msg = new ServerMessage(0xe0);
                //    msg.AppendInt32(1);
                //    session.SendMessage(msg);
                //    return;
                //}
                //if (!user.IsSpectator)
                //{
                    DynamicRoomModel Model = room.GetGameMap().Model;
                        user.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                        user.SetRot(Model.DoorOrientation, false);

                    //if (room.CheckRights(session, true))
                    //{
                    //    user.AddStatus("flatcrtl 4", "useradmin");
                    //}
                    //else if (room.CheckRights(session))
                    //{
                    //    user.AddStatus("flatcrtl 1", "");
                    //}
                    user.CurrentItemEffect = ItemEffectType.None;

                    //UpdateUserEffect(User, User.X, User.Y);

                    if (user.GetClient().GetHabbo().IsTeleporting)
                    {
                        RoomItem Item = room.GetRoomItemHandler().GetItem(user.GetClient().GetHabbo().TeleporterId);

                        if (Item != null)
                        {
                            user.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                            user.SetRot(Item.Rot, false);

                            Item.InteractingUser2 = session.GetHabbo().Id;
                            ((StringData)Item.data).Data = "2";
                            Item.UpdateState(false, true);
                        }
                    }

                    user.GetClient().GetHabbo().IsTeleporting = false;
                    user.GetClient().GetHabbo().TeleporterId = 0;

                    ServerMessage EnterMessage = new ServerMessage(Outgoing.SetRoomUser);
                    EnterMessage.AppendInt32(1);
                    user.Serialize(EnterMessage);
                    room.SendMessage(EnterMessage);


                    if (room.Owner != session.GetHabbo().Username)
                    {
                        FirewindEnvironment.GetGame().GetQuestManager().ProgressUserQuest(user.GetClient(), HabboHotel.Quests.QuestType.SOCIAL_VISIT);
                    }
                //}

                if (session.GetHabbo().GetMessenger() != null)
                    session.GetHabbo().GetMessenger().OnStatusChanged(true);


                //if (!user.IsSpectator)
                //{
                    foreach (RoomUnit unit in UnitList.Values)
                    {
                        RoomAI ai = unit as RoomAI;
                        if (ai == null)
                            continue;

                        ai.BaseAI.OnUserEnterRoom(user);
                    }
                //}

                user.GetClient().GetMessageHandler().OnRoomUserAdd();

                if (OnUserEnter != null)
                    OnUserEnter(user, null);

                if (room.GotMusicController())
                    room.GetRoomMusicController().OnNewUserEnter(user);
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }

        }

        internal void UpdateUserStats(List<RoomUser> users, Hashtable userID, Hashtable userName, int primaryID, int secondaryID)
        {
            foreach (RoomUser user in users)
            {
                _unitList.Inner.Add(user.VirtualID, user);
            }

            foreach (RoomUser user in userID.Values)
            {
                usersByUserID.Add(user.ID, user);
            }

            this.primaryPrivateUserID = primaryID;
            this.secondaryPrivateUserID = secondaryID;
            //room.InitPets();
        }

        internal void RemoveUserFromRoom(GameClient Session, Boolean NotifyClient, Boolean NotifyKick)
        {
            try
            {
                if (Session == null)
                    return;

                if (Session.GetHabbo() == null)
                    return;

                Session.GetHabbo().GetAvatarEffectsInventoryComponent().OnRoomExit();
                
                if (NotifyClient)
                {
                    if (NotifyKick)
                    {
                        Session.GetMessageHandler().GetResponse().Init(Outgoing.GenericError);
                        Session.GetMessageHandler().GetResponse().AppendInt32(4008);
                        Session.GetMessageHandler().SendResponse();
                    }

                    Session.GetMessageHandler().GetResponse().Init(Outgoing.OutOfRoom);
                    Session.GetMessageHandler().SendResponse();
                }

                RoomUser user = GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (user != null)
                {
                    if (user.Team != Team.none)
                    {
                        room.GetTeamManagerForBanzai().OnUserLeave(user);
                        room.GetTeamManagerForFreeze().OnUserLeave(user);
                    }
                    //if (User.isMounted == true)
                    //{
                    //    User.isMounted = false;
                    //    RoomUser usuarioVinculado = GetRoomUserByVirtualId((int)User.mountID);
                    //    if (usuarioVinculado != null)
                    //    {
                    //        usuarioVinculado.isMounted = false;
                    //        usuarioVinculado.mountID = 0;

                    //    }
                    //}
                        user.IsSitting = false;
                        user.IsLaying = false;

                    RemoveRoomUser(user);


                    if (Session.GetHabbo() != null)
                    {
                        //if (!User.IsSpectator)
                        {
                            if (user.CurrentItemEffect != ItemEffectType.None)
                            {
                                user.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().CurrentEffect = -1;
                            }
                            //UserMatrix[User.X, User.Y] = false;

                            if (Session.GetHabbo() != null)
                            {
                                if (room.HasActiveTrade(Session.GetHabbo().Id))
                                    room.TryStopTrade(Session.GetHabbo().Id);

                                if (Session.GetHabbo().Username == room.Owner)
                                {
                                    if (room.HasOngoingEvent)
                                    {
                                        ServerMessage Message = new ServerMessage(Outgoing.RoomEvent);
                                        Message.AppendStringWithBreak("-1");
                                        room.SendMessage(Message);

                                        FirewindEnvironment.GetGame().GetRoomManager().GetEventManager().QueueRemoveEvent(room.RoomData, room.Event.Category);
                                        room.Event = null;
                                    }
                                }
                                Session.GetHabbo().CurrentRoomId = 0;

                                try
                                {
                                    if (Session.GetHabbo().GetMessenger() != null)
                                        Session.GetHabbo().GetMessenger().OnStatusChanged(true);
                                }
                                catch { }
                            }

                            //DateTime Start = DateTime.Now;
                            //using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                            //{
                            //    //TimeSpan TimeUsed1 = DateTime.Now - Start;
                            //    //Logging.LogThreadException("Time used on sys part 2: " + TimeUsed1.Seconds + "s, " + TimeUsed1.Milliseconds + "ms", "");

                            //    //if (Session.GetHabbo() != null)
                            //    //    dbClient.runFastQuery("UPDATE user_roomvisits SET exit_timestamp = '" + FirewindEnvironment.GetUnixTimestamp() + "' WHERE room_id = '" + this.Id + "' AND user_id = '" + Id + "' ORDER BY exit_timestamp DESC LIMIT 1");
                            //    //dbClient.runFastQuery("UPDATE rooms SET users_now = " + UsersNow + " WHERE id = " + Id);
                            //    //dbClient.runFastQuery("REPLACE INTO room_active VALUES (" + RoomId + ", " + UsersNow + ")");
                            //    dbClient.runFastQuery("UPDATE room_active SET active_users = " + UsersNow);
                            //}
                        }
                    }

                    usersByUserID.Remove(user.ID);
                    //if (Session.GetHabbo() != null)
                    //    usersByUsername.Remove(Session.GetHabbo().Username.ToLower());

                    user.Dispose();
                }
                
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Error during removing user from room:" + e.ToString());
            }
        }

        /// <summary>
        /// Called when an unit is removed from UnitList dictionary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void onRemove(object sender, EventArgs args)
        {
            try
            {
                KeyValuePair<int, RoomUnit> removedPair = (KeyValuePair<int, RoomUnit>)sender;

                RoomUnit unit = removedPair.Value;
                //GameClient session = unit.GetClient();

                //int key = removedPair.Key;
                ////freeIDs[key] = null;

                //List<RoomUser> Bots = new List<RoomUser>();

                //foreach (RoomUser roomUser in UnitList.Values)
                //{
                //    if (roomUser.IsBot)
                //        Bots.Add(roomUser);
                //}

                //List<RoomUser> PetsToRemove = new List<RoomUser>();
                //foreach (RoomUser Bot in Bots)
                //{
                //    Bot.BotAI.OnUserLeaveRoom(session);

                //    if (Bot.IsPet && Bot.PetData.OwnerId == unit.userID && !room.CheckRights(session, true))
                //    {
                //        PetsToRemove.Add(Bot);
                //    }
                //}

                //foreach (RoomUser toRemove in PetsToRemove)
                //{
                //    if (unit.GetClient() == null || unit.GetClient().GetHabbo() == null || unit.GetClient().GetHabbo().GetInventoryComponent() == null)
                //        continue;

                //    unit.GetClient().GetHabbo().GetInventoryComponent().AddPet(toRemove.PetData);
                //    RemoveBot(toRemove.VirtualId, false);
                //}

                room.GetGameMap().RemoveUnitFromMap(unit, new Point(unit.X, unit.Y));

            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            UnitList.Remove(user.VirtualID);

            room.GetGameMap().Map[user.X, user.Y] = user.SqState;
            room.GetGameMap().RemoveUnitFromMap(user, new Point(user.X, user.Y));
            ServerMessage LeaveMessage = new ServerMessage(Outgoing.UserLeftRoom);
            LeaveMessage.AppendString(user.VirtualID + String.Empty);
            room.SendMessage(LeaveMessage);
        }

        internal PetBot GetPet(uint PetId)
        {
            return _unitList.Inner.Values.FirstOrDefault(t => t is PetBot && t.ID == PetId) as PetBot;
        }

        internal void UpdateUserCount(int count)
        {
            this.userCount = count;
            room.RoomData.UsersNow = count;

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("UPDATE room_active SET active_users = " + count + " WHERE roomid = " + room.RoomId);
            }

            if (room.HasOngoingEvent)
                FirewindEnvironment.GetGame().GetRoomManager().GetEventManager().QueueUpdateEvent(room.RoomData, room.Event.Category);
            FirewindEnvironment.GetGame().GetRoomManager().QueueActiveRoomUpdate(room.RoomData);
        }

        internal RoomUnit GetRoomUnitByVirtualId(int VirtualId)
        {
            return UnitList.GetValue(VirtualId);
        }

        internal RoomUser GetRoomUserByHabbo(int pId)
        {
            if (usersByUserID.ContainsKey(pId))
                return (RoomUser)usersByUserID[pId];

            return null;
        }

        internal List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> returnList = new List<RoomUser>();
            foreach (RoomUnit unit in _unitList.Values)
            {
                RoomUser user = unit as RoomUser;
                if(user != null)
                    returnList.Add(user);
            }

            return returnList;
        }

        internal List<RoomUser> GetRoomUserByRank(int minRank)
        {
            List<RoomUser> returnList = new List<RoomUser>();
            foreach (RoomUnit unit in UnitList.Values)
            {
                RoomUser user = unit as RoomUser;
                if (user != null && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank > minRank)
                    returnList.Add(user);
            }

            return returnList;
        }

        internal RoomUser GetRoomUserByHabbo(string pName)
        {
            return _unitList.Inner.Values.FirstOrDefault(t => t is RoomUser && t.Name == pName) as RoomUser;
        }


        //internal void SavePets(IQueryAdapter dbClient)
        //{
        //    try
        //    {
        //        if (GetPets().Count > 0)
        //        {
        //            AppendPetsUpdateString(dbClient);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logging.LogCriticalException("Error during saving furniture for room " + room.RoomId + ". Stack: " + e.ToString());
        //    }
        //}

        //internal void AppendPetsUpdateString(IQueryAdapter dbClient)
        //{
        //    QueryChunk inserts = new QueryChunk("INSERT INTO user_pets (id,user_id,room_id,name,type,race,color,expirience,energy,createstamp,nutrition,respect,z,y,z) VALUES ");
        //    QueryChunk updates = new QueryChunk();

        //    List<uint> petsSaved = new List<uint>();
        //    foreach (RoomUser pet in GetPets())
        //    {
        //        if (petsSaved.Contains(pet.PetData.PetId))
        //            continue;



        //        petsSaved.Add(pet.PetData.PetId);
        //        if (pet.PetData.DBState == DatabaseUpdateState.NeedsInsert)
        //        {
        //            inserts.AddParameter(pet.PetData.PetId + "name", pet.PetData.Name);
        //            inserts.AddParameter(pet.PetData.PetId + "race", pet.PetData.Race);
        //            inserts.AddParameter(pet.PetData.PetId + "color", pet.PetData.Color);
        //            inserts.AddQuery("(" + pet.PetData.PetId + "," + pet.PetData.OwnerId + "," + pet.RoomId + ",@" + pet.PetData.PetId + "name," + pet.PetData.Type + ",@" + pet.PetData.PetId + "race,@" + pet.PetData.PetId + "color,0,100,'" + pet.PetData.CreationStamp + "',0,0,0,0,0)");
        //        }
        //        else if (pet.PetData.DBState == DatabaseUpdateState.NeedsUpdate)
        //        {
        //            updates.AddParameter(pet.PetData.PetId + "name", pet.PetData.Name);
        //            updates.AddParameter(pet.PetData.PetId + "race", pet.PetData.Race);
        //            updates.AddParameter(pet.PetData.PetId + "color", pet.PetData.Color);
        //            updates.AddQuery("UPDATE user_pets SET room_id = " + pet.RoomId + ", name = @" + pet.PetData.PetId + "name, race = @" + pet.PetData.PetId + "race, color = @" + pet.PetData.PetId + "color, type = " + pet.PetData.Type + ", expirience = " + pet.PetData.Expirience + ", " +
        //                "energy = " + pet.PetData.Energy + ", nutrition = " + pet.PetData.Nutrition + ", respect = " + pet.PetData.Respect + ", createstamp = '" + pet.PetData.CreationStamp + "', x = " + pet.X + ", Y = " + pet.Y + ", Z = " + pet.Z + " WHERE id = " + pet.PetData.PetId);
        //        }

        //        pet.PetData.DBState = DatabaseUpdateState.Updated;
        //    }

        //    inserts.Execute(dbClient);
        //    updates.Execute(dbClient);

        //    inserts.Dispose();
        //    updates.Dispose();

        //    inserts = null;
        //    updates = null;
        //}

        //internal List<RoomUser> GetPets()
        //{
        //    List<KeyValuePair<int, RoomUser>> users = UserList.ToList();

        //    List<RoomUser> results = new List<RoomUser>();
        //    foreach (KeyValuePair<int, RoomUser> pair in users)
        //    {
        //        RoomUser user = pair.Value;
        //        if (user.IsPet)
        //            results.Add(user);
        //    }

        //    return results;
        //}
        //internal int GetPetCount()
        //{
        //    int count = 0;
        //    List<KeyValuePair<int, RoomUser>> users = UserList.ToList();

        //    foreach (KeyValuePair<int, RoomUser> pair in users)
        //    {
        //        RoomUser user = pair.Value;
        //        if (user.IsPet)
        //            count++;
        //    }

        //    return count;
        //}

        internal ServerMessage SerializeStatusUpdates(Boolean All)
        {
            List<RoomUnit> units;
            if (All)
                units = UnitList.Inner.Values.ToList();
            else
            {
                units = new List<RoomUnit>();
                foreach (RoomUnit unit in UnitList.Values)
                {
                    if (!unit.UpdateNeeded)
                        continue;
                    unit.UpdateNeeded = false;
                    units.Add(unit);
                }
            }

            if (units.Count == 0)
                return null;

            ServerMessage Message = new ServerMessage(Outgoing.UserUpdate);
            Message.AppendInt32(units.Count);

            foreach (RoomUnit unit in units)
                unit.SerializeStatus(Message);

            return Message;
        }

        internal void UpdateUserStatuses()
        {
            onCycleDoneDelegate userUpdate = new onCycleDoneDelegate(onUserUpdateStatus);
            UnitList.QueueDelegate(userUpdate);
        }

        private void onUserUpdateStatus()
        {
            foreach (RoomUser user in UnitList.Values)
                UpdateUserStatus(user, false);
        }

        internal void backupCounters(ref int primaryCounter, ref int secondaryCounter)
        {
            primaryCounter = primaryPrivateUserID;
            secondaryCounter = secondaryPrivateUserID;
        }

        private bool isValid(RoomUser user)
        {
            if (user.GetClient() == null)
                return false;
            if (user.GetClient().GetHabbo() == null)
            return false;
            if (user.GetClient().GetHabbo().CurrentRoomId != room.RoomId)
                return false;

            return true;
        }

        internal void UpdateUserStatus(RoomUnit unit, bool cyclegameitems)
        {
            try
            {
                if (unit == null)
                    return;
                RoomUser user = unit as RoomUser;
                bool isBot = user == null;
                if (isBot)
                    cyclegameitems = false;

                if (unit.Statuses.ContainsKey("lay") || unit.Statuses.ContainsKey("sit"))
                {
                    unit.Statuses.Remove("lay");
                    unit.Statuses.Remove("sit");
                    unit.UpdateNeeded = true;
                }

                if (unit.Statuses.ContainsKey("sign"))
                {
                    unit.Statuses.Remove("sign");
                    unit.UpdateNeeded = true;
                }

                //List<RoomItem> ItemsOnSquare = GetFurniObjects(User.X, User.Y);
                CoordItemSearch ItemSearch = new CoordItemSearch(room.GetGameMap().CoordinatedItems);
                List<RoomItem> ItemsOnSquare = ItemSearch.GetAllRoomItemForSquare(unit.X, unit.Y);
                double newZ;
                //if (user.isMounted == true && user.IsPet == false)
                //{
                //    newZ = room.GetGameMap().SqAbsoluteHeight(user.X, user.Y, ItemsOnSquare) + 1;
                //}
                //else
                {
                    newZ = room.GetGameMap().SqAbsoluteHeight(unit.X, unit.Y, ItemsOnSquare);
                }

                if (!isBot)
                {
                    if (newZ != user.Z)
                    {
                        user.Z = newZ;
                        if (user.IsFlying)
                            user.Z += 4 + (0.5 * Math.Sin(0.7 * user.FlyCounter));
                        user.UpdateNeeded = true;
                    }
                }

                DynamicRoomModel Model = room.GetGameMap().Model;
                if (Model.SqState[unit.X, unit.Y] == SquareState.SEAT || (!isBot && user.IsSitting == true || user.IsLaying == true))
                {

                    if (!isBot && user.IsSitting == true)
                    {
                        if (!user.Statuses.ContainsKey("sit"))
                        {
                            user.Statuses.Add("sit", Convert.ToString(Model.SqFloorHeight[user.X, user.Y] + 0.55).Replace(",", "."));
                        }
                        user.Z = Model.SqFloorHeight[user.X, user.Y];
                        user.UpdateNeeded = true;
                    }
                    else if (!isBot && user.IsLaying == true)
                    {
                        if (!user.Statuses.ContainsKey("lay"))
                        {
                            user.Statuses.Add("lay", Convert.ToString(Model.SqFloorHeight[user.X, user.Y] + 0.55).Replace(",", "."));
                        }
                        user.Z = Model.SqFloorHeight[user.X, user.Y];
                        user.UpdateNeeded = true;
                    }
                    else
                    {

                        if (!unit.Statuses.ContainsKey("sit"))
                        {
                            unit.Statuses.Add("sit", "1.0");
                        }

                        unit.Z = Model.SqFloorHeight[unit.X, unit.Y];
                        if (!isBot && user.IsFlying)
                            user.Z += 4 + (0.5 * Math.Sin(0.7 * user.FlyCounter));
                        unit.RotHead = Model.SqSeatRot[unit.X, unit.Y];
                        unit.RotBody = Model.SqSeatRot[unit.X, unit.Y];

                        unit.UpdateNeeded = true;
                    }
                }

                foreach (RoomItem Item in ItemsOnSquare)
                {
                    if (cyclegameitems)
                    {
                        Item.UserWalksOnFurni(user);
                    }

                    if (Item.GetBaseItem().IsSeat)
                    {
                        if (!unit.Statuses.ContainsKey("sit"))
                        {
                            unit.Statuses.Add("sit", TextHandling.GetString(Item.GetBaseItem().Height));
                        }

                        unit.Z = Item.GetZ;
                        if (!isBot && user.IsFlying)
                            user.Z += 4 + (0.5 * Math.Sin(0.7 * user.FlyCounter));
                        unit.RotHead = Item.Rot;
                        unit.RotBody = Item.Rot;

                        unit.UpdateNeeded = true;
                    }


                    switch (Item.GetBaseItem().InteractionType)
                    {
                        case InteractionType.bed:
                            {
                                if (!unit.Statuses.ContainsKey("lay"))
                                {
                                    unit.Statuses.Add("lay", TextHandling.GetString(Item.GetBaseItem().Height) + " null");
                                }

                                unit.Z = Item.GetZ;
                                if (!isBot && user.IsFlying)
                                    user.Z += 4 + (0.2 * 0.5 * Math.Sin(0.7 * user.FlyCounter));
                                unit.RotHead = Item.Rot;
                                unit.RotBody = Item.Rot;

                                unit.UpdateNeeded = true;
                                break;
                            }

                        case InteractionType.fbgate:
                            {
                                if (cyclegameitems)
                                {
                                    if (user.Team != Item.team)
                                        user.Team = Item.team;

                                    else if (user.Team == Item.team)
                                        user.Team = Team.none;

                                    if (!string.IsNullOrEmpty(Item.Figure))
                                    {
                                        //User = GetUserForSquare(Item.Coordinate.X, Item.Coordinate.Y);
                                        if (user != null)
                                        {
                                            if (user.Coordinate == Item.Coordinate)
                                            {
                                                if (user.GetClient().GetHabbo().Gender != Item.Gender && user.GetClient().GetHabbo().Look != Item.Figure)
                                                {

                                                    user.GetClient().GetHabbo().tempGender = user.GetClient().GetHabbo().Gender;
                                                    user.GetClient().GetHabbo().tempLook = user.GetClient().GetHabbo().Look;

                                                    user.GetClient().GetHabbo().Gender = Item.Gender;
                                                    user.GetClient().GetHabbo().Look = Item.Figure;
                                                }
                                                else
                                                {
                                                    user.GetClient().GetHabbo().Gender = user.GetClient().GetHabbo().tempGender;
                                                    user.GetClient().GetHabbo().Look = user.GetClient().GetHabbo().tempLook;
                                                }

                                                ServerMessage RoomUpdate = new ServerMessage(Outgoing.UpdateUserInformation);
                                                RoomUpdate.AppendInt32(user.VirtualID);
                                                RoomUpdate.AppendStringWithBreak(user.GetClient().GetHabbo().Look);
                                                RoomUpdate.AppendStringWithBreak(user.GetClient().GetHabbo().Gender.ToLower());
                                                RoomUpdate.AppendStringWithBreak(user.GetClient().GetHabbo().Motto);
                                                RoomUpdate.AppendInt32(user.GetClient().GetHabbo().AchievementPoints);
                                                room.SendMessage(RoomUpdate);
                                            }
                                        }
                                    }
                                }

                                break;
                            }

                        //33: Red
                        //34: Green
                        //35: Blue
                        //36: Yellow

                        case InteractionType.banzaigategreen:
                        case InteractionType.banzaigateblue:
                        case InteractionType.banzaigatered:
                        case InteractionType.banzaigateyellow:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = (int)Item.team + 32;
                                    TeamManager t = user.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForBanzai();
                                    AvatarEffectsInventoryComponent efectmanager = user.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent();

                                    if (user.Team != Item.team)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (user.Team != Team.none)
                                                t.OnUserLeave(user);
                                            user.Team = Item.team;
                                            t.AddUser(user);

                                            if (efectmanager.CurrentEffect != effectID)
                                                efectmanager.ApplyCustomEffect(effectID);
                                        }
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(user);
                                        if (efectmanager.CurrentEffect == effectID)
                                            efectmanager.ApplyCustomEffect(0);
                                        user.Team = Team.none;
                                    }
                                    //((StringData)Item.data).Data = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);                                
                                }
                                break;
                            }

                        case InteractionType.freezeyellowgate:
                        case InteractionType.freezeredgate:
                        case InteractionType.freezegreengate:
                        case InteractionType.freezebluegate:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = (int)Item.team + 39;
                                    TeamManager t = user.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForFreeze();
                                    //int usersOnTeam = 0;
                                    //if (((StringData)Item.data).Data != "")
                                    //usersOnTeam = int.Parse(((StringData)Item.data).Data);
                                    AvatarEffectsInventoryComponent efectmanager = user.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent();

                                    if (user.Team != Item.team)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (user.Team != Team.none)
                                                t.OnUserLeave(user);
                                            user.Team = Item.team;
                                            t.AddUser(user);

                                            if (efectmanager.CurrentEffect != effectID)
                                                efectmanager.ApplyCustomEffect(effectID);
                                        }
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(user);
                                        if (efectmanager.CurrentEffect == effectID)
                                            efectmanager.ApplyCustomEffect(0);
                                        user.Team = Team.none;
                                    }
                                    //((StringData)Item.data).Data = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);

                                    ServerMessage message = new ServerMessage(700);
                                    message.AppendBoolean((user.Team != Team.none));

                                    user.GetClient().SendMessage(message);
                                }
                                break;
                            }

                        case InteractionType.banzaitele:
                            {
                                room.GetGameItemHandler().onTeleportRoomUserEnter(user, Item);
                                break;
                            }

                    }
                }

                if (cyclegameitems)
                {
                    if (room.GotSoccer())
                        room.GetSoccer().OnUserWalk(user);

                    if (room.GotBanzai())
                        room.GetBanzai().OnUserWalk(user);

                    //if (room.GotFreeze())
                    room.GetFreeze().OnUserWalk(user);
                }
            }
            catch (Exception e)
            {
            }
        }

        internal void TurnHeads(int X, int Y, int SenderId)
        {
            foreach (RoomUser user in UnitList.Values)
            {
                if (user.ID == SenderId)
                    continue;

                user.SetRot(Rotation.Calculate(user.X, user.Y, X, Y), true); 
            }
        }

        private List<RoomUser> ToRemove;

        internal void OnCycle(ref int idleCount)
        {
            ToRemove.Clear();
            int userCounter = 0;

            foreach (RoomUser User in UnitList.Values)
            {
                if (!isValid(User))
                {
                    if (User.GetClient() != null)
                        RemoveUserFromRoom(User.GetClient(), false, false);
                    else
                        RemoveRoomUser(User);
                }

                bool updated = false;
                User.IdleTime++;

                if (!User.IsAsleep && User.IdleTime >= 600)
                {
                    User.IsAsleep = true;

                    ServerMessage FallAsleep = new ServerMessage(Outgoing.IdleStatus);
                    FallAsleep.AppendInt32(User.VirtualID);
                    FallAsleep.AppendBoolean(true);
                    room.SendMessage(FallAsleep);
                }

                if (User.NeedsAutokick && !ToRemove.Contains(User))
                {
                    ToRemove.Add(User);
                    continue;
                }

                if (User.CarryItemID > 0)
                {
                    User.CarryTimer--;
                    if (User.CarryTimer <= 0)
                        User.CarryItem(0);
                }

                if (room.GotFreeze())
                {
                    room.GetFreeze().CycleUser(User);
                }

                if (User.IsFlying)
                    User.OnFly();

                if (User.SetStep)
                {


                    if (room.GetGameMap().CanWalk(User.SetX, User.SetY, User.AllowOverride))
                    {
                        room.GetGameMap().UpdateUnitMovement(new Point(User.Coordinate.X, User.Coordinate.Y), new Point(User.SetX, User.SetY), User);
                        List<RoomItem> items = room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));

                        User.X = User.SetX;
                        User.Y = User.SetY;
                        User.Z = User.SetZ;
                        if (User.IsFlying)
                            User.Z += 4 + 0.5 * Math.Sin(0.7 * User.FlyCounter);

                        lock (items)
                        {
                            foreach (RoomItem item in items)
                            {
                                item.UserWalksOffFurni(User);
                            }
                        }

                        if (User.X == room.GetGameMap().Model.DoorX && User.Y == room.GetGameMap().Model.DoorY && !ToRemove.Contains(User))
                        {
                            ToRemove.Add(User);
                            continue;
                        }

                        UpdateUserStatus(User, true);
                    }
                    User.SetStep = false;
                }

                if (User.IsWalking && !User.Freezed)
                {
                    
                    GameMap map = room.GetGameMap();
                    SquarePoint Point = DreamPathfinder.GetNextStep(User.X, User.Y, User.GoalX, User.GoalY, map.Map, map.ItemHeightMap, 
                        map.Model.MapSizeX, map.Model.MapSizeY, User.AllowOverride, map.DiagonalEnabled);

                    if (Point.X == User.X && Point.Y == User.Y) //No path found, or reached goal (:
                    {
                        User.IsWalking = false;
                        User.RemoveStatus("mv");

                        UpdateUserStatus(User, false);

                        //if (User.isMounted == true && User.IsPet == false)
                        //{
                        //    RoomUser mascotaVinculada = GetRoomUnitByVirtualId(Convert.ToInt32(User.mountID));
                        //    mascotaVinculada.IsWalking = false;
                        //    mascotaVinculada.RemoveStatus("mv");

                        //    ServerMessage mess = new ServerMessage(Outgoing.UserUpdate);
                        //    mess.AppendInt32(1);
                        //    mascotaVinculada.SerializeStatus(mess, "");
                        //    User.GetClient().GetHabbo().CurrentRoom.SendMessage(mess);
                        //}
                    }
                    else
                    {

                        int nextX = Point.X;
                        int nextY = Point.Y;

                        User.RemoveStatus("mv");

                        double nextZ = room.GetGameMap().SqAbsoluteHeight(nextX, nextY);

                        User.Statuses.Remove("lay");
                        User.Statuses.Remove("sit");
                        string user = "";
                        string mascote = "";
                        if (!User.IsFlying)
                        {
                            //if (User.isMounted == true&&User.IsPet==false)
                            //{
                            //    RoomUser mascotaVinculada = GetRoomUnitByVirtualId(Convert.ToInt32(User.mountID));
                            //    user = ("mv " + nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));
                            //    User.AddStatus("mv", + nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));
                            //    mascote = ("mv "+ nextX + "," + nextY + "," + TextHandling.GetString(nextZ));
                            //    //Logging.WriteLine("Se handleo movimiento en la mascota id: " + GetRoomUserByVirtualId(Convert.ToInt32(User.montandoID)).InternalRoomID);
                            //}
                            //else
                            {
                                User.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));
                                
                            }

                            /**
                            if (User.montandoBol == true)
                            {
                                GetRoomUserByVirtualId(Convert.ToInt32(User.montandoID)).MoveTo(User.GoalX, User.GoalY,true);
                                User.MoveTo(User.GoalX, User.GoalY);
                                GetRoomUserByVirtualId(Convert.ToInt32(User.montandoID)).AllowOverride = true;
                                Logging.WriteLine("Se handleo movimiento en la mascota id: " + GetRoomUserByVirtualId(Convert.ToInt32(User.montandoID)).InternalRoomID);
                            }
                             */
                        }
                        else
                        {
                            User.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 4 + (0.5 * Math.Sin(0.7 * User.FlyCounter))));
                        }
                        int newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.moonwalkEnabled);

                        User.RotBody = newRot;
                        User.RotHead = newRot;

                        User.SetStep = true;
                        User.SetX = nextX;
                        User.SetY = nextY;
                        User.SetZ = nextZ;

                        UpdateUserEffect(User, User.SetX, User.SetY);
                        updated = true;

                        room.GetGameMap().Map[User.X, User.Y] = User.SqState; // REstore the old one
                        User.SqState = room.GetGameMap().Map[User.SetX, User.SetY];//Backup the new one
                        if (User.IsSitting == true)
                            User.IsLaying = false;

                        if (User.IsLaying == true)
                            User.IsLaying = false;

                        //if (User.isMounted == true&&User.IsPet==false)
                        //{
                        //    //Logging.WriteLine("Montaje");
                        //    RoomUser mascotaVinculada = GetRoomUnitByVirtualId(Convert.ToInt32(User.mountID));

                        //    // Temp fix for crash
                        //    // TODO: Remove the saddle effect
                        //    if (mascotaVinculada == null)
                        //    {
                        //        User.mountID = 0;
                        //        User.isMounted = false;
                        //        continue;
                        //    }
                        //    mascotaVinculada.RotBody = newRot;
                        //    mascotaVinculada.RotHead = newRot;

                        //    mascotaVinculada.SetStep = true;
                        //    mascotaVinculada.SetX = nextX;
                        //    mascotaVinculada.SetY = nextY;
                        //    mascotaVinculada.SetZ = nextZ;

                        //    UpdateUserEffect(mascotaVinculada, mascotaVinculada.SetX, mascotaVinculada.SetY);
                        //    updated = true;
                        //    ServerMessage mess = new ServerMessage(Outgoing.UserUpdate);
                        //    mess.AppendInt32(2);
                        //    User.SerializeStatus(mess, user);
                        //    mascotaVinculada.SerializeStatus(mess, mascote);
                        //    User.GetClient().GetHabbo().CurrentRoom.SendMessage(mess);
                        //    UpdateUserEffect(User, User.SetX, User.SetY);
                        //    //mascotaVinculada.UpdateNeeded = true;
                            
                        //}

                        if (!room.AllowWalkthrough)
                            room.GetGameMap().Map[nextX, nextY] = 0;
                    }
                    //if (!User.isMounted)
                        User.UpdateNeeded = true;
                }
                else
                {
                    if (User.Statuses.ContainsKey("mv"))
                    {
                        User.RemoveStatus("mv");
                        User.UpdateNeeded = true;
                    }
                }


                //if (User.IsBot)
                //    User.BotAI.OnTimerTick();
                //else
                {
                    userCounter++;
                }

                if (!updated)
                    UpdateUserEffect(User, User.X, User.Y);
            }

            if (userCounter == 0)
                idleCount++;


            foreach (RoomUser toRemove in ToRemove)
            {
                GameClient client = FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(toRemove.ID);
                if (client != null)
                {
                    RemoveUserFromRoom(client, true, false);
                    client.CurrentRoomUserID = -1;
                }
                else
                    RemoveRoomUser(toRemove);
            }

            if (userCount != userCounter)
            {
                UpdateUserCount(userCounter);
            }
        }

        internal void Destroy()
        {
            room = null;
            usersByUserID.Clear();
            usersByUserID = null;
            OnUserEnter = null;
            _unitList.Destroy();
            _unitList = null;
        }
    }
}
