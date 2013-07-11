﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Firewind;
using Firewind.HabboHotel.Catalogs;
using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.Rooms;
using Firewind.HabboHotel.Users.Badges;
using Firewind.HabboHotel.Users.Inventory;
using Firewind.HabboHotel.Users.Messenger;
using Firewind.HabboHotel.Users.Subscriptions;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.HabboHotel.Users;
using Firewind.HabboHotel.Users.Authenticator;
using Firewind.Core;
using Firewind.HabboHotel.Achievements;
using Firewind.HabboHotel.Rooms.Units;


namespace Firewind.HabboHotel.Users.UserDataManagement
{
    class UserDataFactory
    {
        internal static UserData GetUserData(string sessionTicket, string ip, out byte errorCode)
        {
            DataRow dUserInfo;

            DataTable dAchievements;
            DataTable dFavouriteRooms;
            DataTable dIgnores;
            DataTable dTags;
            DataTable dSubscriptions;
            DataTable dBadges;
            DataTable dInventory;
            DataTable dEffects;
            DataTable dFriends;
            DataTable dRequests;
            DataTable dRooms;
            DataTable dPets;
            DataTable dBots;
            DataTable dQuests;
            //DataTable dSongs;

            int userID;

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT * " +
                                  "FROM users " +
                                  "WHERE auth_ticket = @sso ");

                dbClient.addParameter("sso", sessionTicket);
                //dbClient.addParameter("ipaddress", ip);
                dUserInfo = dbClient.getRow();


                if (dUserInfo == null)
                {
                    errorCode = 1;
                    return null;
                    //Logging.LogException("No user found. Debug data: [" + sessionTicket + "], [" + ip + "]");
                    //throw new UserDataNotFoundException(string.Format("No user found with ip {0} and sso {1}. Use SSO: {2} ", ip, sessionTicket, FirewindEnvironment.useSSO.ToString()));
                }


                userID = Convert.ToInt32(dUserInfo["id"]);
                if (FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(userID) != null)
                {
                    errorCode = 2;
                    return null;
                }

                string creditsTimestamp = (string) dUserInfo["lastdailycredits"];
                string todayTimestamp = DateTime.Today.ToString("MM/dd");
                if (creditsTimestamp != todayTimestamp)
                {
                    dbClient.runFastQuery(
                        "UPDATE users SET credits = credits + 3000, daily_respect_points = 3, lastdailycredits = '" +
                        todayTimestamp + "' WHERE id = " + userID);
                    dUserInfo["credits"] = (int) dUserInfo["credits"] + 3000;
                }

                dbClient.setQuery("SELECT * FROM user_achievement WHERE userid = " + userID);
                dAchievements = dbClient.getTable();

                dbClient.setQuery("SELECT room_id FROM user_favorites WHERE user_id = " + userID);
                dFavouriteRooms = dbClient.getTable();

                dbClient.setQuery("SELECT ignore_id FROM user_ignores WHERE user_id = " + userID);
                dIgnores = dbClient.getTable();

                dbClient.setQuery("SELECT tag FROM user_tags WHERE user_id = " + userID);
                dTags = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_subscriptions WHERE user_id = " + userID);
                dSubscriptions = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_badges WHERE user_id = " + userID);
                dBadges = dbClient.getTable();

                dbClient.setQuery("CALL getuseritems(" + userID + ")");
                dInventory = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_effects WHERE user_id =  " + userID);
                dEffects = dbClient.getTable();

                dbClient.setQuery("SELECT users.id,users.username,users.motto,users.look,users.last_online " +
                                  "FROM users " +
                                  "JOIN messenger_friendships " +
                                  "ON users.id = messenger_friendships.sender " +
                                  "WHERE messenger_friendships.receiver = " + userID + " " +
                                  "UNION ALL " +
                                  "SELECT users.id,users.username,users.motto,users.look,users.last_online " +
                                  "FROM users " +
                                  "JOIN messenger_friendships " +
                                  "ON users.id = messenger_friendships.receiver " +
                                  "WHERE messenger_friendships.sender = " + userID);
                dFriends = dbClient.getTable();

                dbClient.setQuery("SELECT messenger_requests.sender,messenger_requests.receiver,users.username " +
                                  "FROM users " +
                                  "JOIN messenger_requests " +
                                  "ON users.id = messenger_requests.sender " +
                                  "WHERE messenger_requests.receiver = " + userID);
                dRequests = dbClient.getTable();

                dbClient.setQuery(
                    "SELECT rooms.*, room_active.active_users FROM rooms LEFT JOIN room_active ON (room_active.roomid = rooms.id) WHERE owner = @name");
                dbClient.addParameter("name", (string) dUserInfo["username"]);
                dRooms = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_pets WHERE user_id = " + userID + " AND room_id = 0");
                dPets = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_bots WHERE user_id = " + userID + "");
                dBots = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_quests WHERE user_id = " + userID + "");
                dQuests = dbClient.getTable();

                //dbClient.setQuery("SELECT item_id, song_id FROM user_items_songs WHERE user_id = " + userID);
                //dSongs = dbClient.getTable();


                /* dbClient.setQuery("UPDATE users SET ip_last = @ip WHERE id = " + userID + " LIMIT 1; " +
                                       "UPDATE user_info SET login_timestamp = '" + FirewindEnvironment.GetUnixTimestamp() + "' WHERE user_id = " + userID + " LIMIT 1; " +
                                       "REPLACE INTO user_online VALUES (" + userID + "); " +
                                       "DELETE FROM user_tickets WHERE userid = " + userID + ";");*/

                dbClient.setQuery("UPDATE users SET ip_last = @ip WHERE id = " + userID + "; " +
                                  "UPDATE user_info SET login_timestamp = '" + FirewindEnvironment.GetUnixTimestamp() +
                                  "' WHERE user_id = " + userID + " ; " +
                                  "");
                dbClient.addParameter("ip", ip);
                dbClient.runQuery();

                dbClient.runFastQuery("REPLACE INTO user_online VALUES (" + userID + ")");
            }

            Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();

            string achievementGroup;
            int achievementLevel;
            int achievementProgress;
            foreach (DataRow dRow in dAchievements.Rows)
            {
                achievementGroup = (string)dRow["group"];
                achievementLevel = (int)dRow["level"];
                achievementProgress = (int)dRow["progress"];

                UserAchievement achievement = new UserAchievement(achievementGroup, achievementLevel, achievementProgress);
                achievements.Add(achievementGroup, achievement);
            }

            List<uint> favouritedRooms = new List<uint>();

            uint favoritedRoomID;
            foreach (DataRow dRow in dFavouriteRooms.Rows)
            {
                favoritedRoomID = Convert.ToUInt32(dRow["room_id"]);
                favouritedRooms.Add(favoritedRoomID);
            }


            List<int> ignores = new List<int>();

            int ignoredUserID;
            foreach (DataRow dRow in dIgnores.Rows)
            {
                ignoredUserID = Convert.ToInt32(dRow["ignore_id"]);
                ignores.Add(ignoredUserID);
            }


            List<string> tags = new List<string>();

            string tag;
            foreach (DataRow dRow in dTags.Rows)
            {
                tag = (string)dRow["tag"];
                tags.Add(tag);
            }

            Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();

            string subscriptionID;
            int expireTimestamp;
            foreach (DataRow dRow in dSubscriptions.Rows)
            {
                subscriptionID = (string)dRow["subscription_id"];
                expireTimestamp = (int)dRow["timestamp_expire"];

                subscriptions.Add(subscriptionID, new Subscription(subscriptionID, expireTimestamp));
            }

            List<Badge> badges = new List<Badge>();

            string badgeID;
            int slotID;
            foreach (DataRow dRow in dBadges.Rows)
            {
                badgeID = (string)dRow["badge_id"];
                slotID = (int)dRow["badge_slot"];
                badges.Add(new Badge(badgeID, slotID));
            }


            List<UserItem> inventory = new List<UserItem>();

            uint itemID;
            uint baseItem;
            int dataType;
            string extradata;
            int extra;
            foreach (DataRow Row in dInventory.Rows)
            {
                itemID = Convert.ToUInt32(Row[0]);
                baseItem = Convert.ToUInt32(Row[1]);

                    IRoomItemData data;
                    if (DBNull.Value.Equals(Row[2]))
                    {
                        data = new StringData("");
                        extra = 0;
                    }
                    else
                    {
                        dataType = Convert.ToInt32(Row[2]);
                        extradata = (string)Row[3];
                        extra = Convert.ToInt32(Row[4]);
                        switch (dataType)
                        {
                            case 0:
                                data = new StringData(extradata);
                                break;
                            case 1:
                                data = new MapStuffData();
                                break;
                            case 2:
                                data = new StringArrayStuffData();
                                break;
                            case 3:
                                data = new StringIntData();
                                break;
                            default:
                                data = new StringData(extradata);
                                break;
                        }
                        try
                        {
                            data.Parse(extradata);
                        }
                        catch
                        {
                            Logging.LogException(string.Format("Error in furni data! Item ID: \"{0}\" and data: \"{1}\"", itemID, extradata.Replace(Convert.ToChar(1).ToString(), "[1]")));
                        }
                    }

                inventory.Add(new UserItem(itemID, baseItem, data, extra));
            }


            List<AvatarEffect> effects = new List<AvatarEffect>();

            int effectID;
            int duration;
            bool isActivated;
            double activatedTimeStamp;
            foreach (DataRow dRow in dEffects.Rows)
            {
                effectID = (int)dRow["effect_id"];
                duration = (int)dRow["total_duration"];
                isActivated = Convert.ToInt32(dRow["is_activated"]) == 1;
                activatedTimeStamp = (double)dRow["activated_stamp"];

                effects.Add(new AvatarEffect(effectID, duration, isActivated, activatedTimeStamp));
            }


            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();

            string username = (string)dUserInfo["username"];

            int friendID;
            string friendName;
            string friendLook;
            string friendMotto;
            string friendLastOnline;
            foreach (DataRow dRow in dFriends.Rows)
            {
                friendID = Convert.ToInt32(dRow["id"]);
                friendName = (string)dRow["username"];
                friendLook = (string)dRow["look"];
                friendMotto = (string)dRow["motto"];
                friendLastOnline = Convert.ToString(dRow["last_online"]);


                if (friendID == userID)
                    continue;

                


                if (!friends.ContainsKey(friendID))
                    friends.Add(friendID, new MessengerBuddy(friendID, friendName, friendLook, friendMotto, friendLastOnline));
            }

            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();

            int receiverID;
            int senderID;
            string requestUsername;
            foreach (DataRow dRow in dRequests.Rows)
            {
                receiverID = Convert.ToInt32(dRow["sender"]);
                senderID = Convert.ToInt32(dRow["receiver"]);

                requestUsername = (string)dRow["username"];

                if (receiverID != userID)
                {
                    if (!requests.ContainsKey(receiverID))
                        requests.Add(receiverID, new MessengerRequest(userID, receiverID, requestUsername));
                }
                else
                {
                    if (!requests.ContainsKey(senderID))
                        requests.Add(senderID, new MessengerRequest(userID, senderID, requestUsername));
                }
            }

            List<RoomData> rooms = new List<RoomData>();

            uint roomID;
            foreach (DataRow dRow in dRooms.Rows)
            {
                roomID = Convert.ToUInt32(dRow["id"]);
                rooms.Add(FirewindEnvironment.GetGame().GetRoomManager().FetchRoomData(roomID, dRow));
            }


            Dictionary<uint, Pet> pets = new Dictionary<uint, Pet>();

            Pet pet;
            foreach (DataRow dRow in dPets.Rows)
            {
                pet = Catalog.GeneratePetFromRow(dRow);
                pets.Add(pet.PetId, pet);
            }

            Dictionary<int, RentableBot> bots = new Dictionary<int, RentableBot>();

            RentableBot bot;
            foreach (DataRow row in dBots.Rows)
            {
                bot = new RentableBot();

                bot.OwnerID = Convert.ToInt32(row["user_id"]);
                bot.ID = Convert.ToInt32(row["id"]);
                bot.Name = Convert.ToString(row["name"]);
                bot.Gender = Convert.ToChar(row["gender"]);
                bot.Figure = Convert.ToString(row["figure"]);
                bot.Motto = "1 week SpyBot";
                bot.TimeLeft = 604800; // 1 week

                bots.Add(bot.ID, bot);
            }

            Dictionary<uint, int> quests = new Dictionary<uint, int>();

            uint questId;
            int progress;
            foreach (DataRow dRow in dQuests.Rows)
            {
                questId = Convert.ToUInt32(dRow["quest_id"]);
                progress = (int)dRow["progress"];
                quests.Add(questId, progress);
            }

            Hashtable songs = new Hashtable();

            //uint songItemID;
            //uint songID;
            //foreach (DataRow dRow in dSongs.Rows)
            //{
            //    songItemID = (uint)dRow[0];
            //    songID = (uint)dRow[1];

            //    SongItem song = new SongItem(songItemID, songID);
            //    songs.Add(songItemID, song);
            //}

            Habbo user = HabboFactory.GenerateHabbo(dUserInfo);

            dUserInfo = null;
            dAchievements = null;
            dFavouriteRooms = null;
            dIgnores = null;
            dTags = null;
            dSubscriptions = null;
            dBadges = null;
            dInventory = null;
            dEffects = null;
            dFriends = null;
            dRequests = null;
            dRooms = null;
            dPets = null;

            errorCode = 0;
            return new UserData(userID, achievements, favouritedRooms, ignores, tags, subscriptions, badges, inventory, effects, friends, requests, rooms, pets, quests, songs, user, bots);
        }

        internal static UserData GetUserData(int UserId)
        {
            
            byte errorCode;
            DataRow dUserInfo;
            
            DataTable dAchievements;
            DataTable dFavouriteRooms;
            DataTable dIgnores;
            DataTable dTags;
            DataTable dSubscriptions;
            DataTable dBadges;
            DataTable dInventory;
            DataTable dEffects;
            DataTable dFriends;
            DataTable dRequests;
            DataTable dRooms;
            DataTable dPets;
            DataTable dQuests;
            //DataTable dSongs;

            int userID;

            using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT users.* FROM users WHERE users.id = @id");
                dbClient.addParameter("id", UserId);
                dUserInfo = dbClient.getRow();


                if (dUserInfo == null)
                {
                    errorCode = 1;
                    return null;
                    //Logging.LogException("No user found. Debug data: [" + sessionTicket + "], [" + ip + "]");
                    //throw new UserDataNotFoundException(string.Format("No user found with ip {0} and sso {1}. Use SSO: {2} ", ip, sessionTicket, FirewindEnvironment.useSSO.ToString()));
                }


                userID = Convert.ToInt32(dUserInfo["id"]);
                if (FirewindEnvironment.GetGame().GetClientManager().GetClientByUserID(userID) != null)
                {
                    errorCode = 2;
                    return null;
                }

                /**
                string creditsTimestamp = (string)dUserInfo["lastdailycredits"];
                string todayTimestamp = DateTime.Today.ToString("MM/dd");
                if (creditsTimestamp != todayTimestamp)
                {
                    dbClient.runFastQuery("UPDATE users SET credits = credits + 3000, daily_respect_points = 3, lastdailycredits = '" + todayTimestamp + "' WHERE id = " + userID);
                    dUserInfo["credits"] = (int)dUserInfo["credits"] + 3000;
                }
                 * ***/
                /**
                dbClient.setQuery("SELECT * FROM user_achievement WHERE userid = " + userID);
                dAchievements = dbClient.getTable();

                dbClient.setQuery("SELECT room_id FROM user_favorites WHERE user_id = " + userID);
                dFavouriteRooms = dbClient.getTable();

                dbClient.setQuery("SELECT ignore_id FROM user_ignores WHERE user_id = " + userID);
                dIgnores = dbClient.getTable();

                dbClient.setQuery("SELECT tag FROM user_tags WHERE user_id = " + userID);
                dTags = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_subscriptions WHERE user_id = " + userID);
                dSubscriptions = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_badges WHERE user_id = " + userID);
                dBadges = dbClient.getTable();

                if (dbClient.dbType == Database_Manager.Database.DatabaseType.MySQL)
                    dbClient.setQuery("CALL getuseritems(" + userID + ")");
                else
                    dbClient.setQuery("EXECUTE getuseritems " + userID + "");
                dInventory = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_effects WHERE user_id =  " + userID);
                dEffects = dbClient.getTable();

                dbClient.setQuery("SELECT users.id,users.username,users.motto,users.look,users.last_online " +
                                        "FROM users " +
                                        "JOIN messenger_friendships " +
                                        "ON users.id = messenger_friendships.sender " +
                                        "WHERE messenger_friendships.receiver = " + userID + " " +
                                        "UNION ALL " +
                                        "SELECT users.id,users.username,users.motto,users.look,users.last_online " +
                                        "FROM users " +
                                        "JOIN messenger_friendships " +
                                        "ON users.id = messenger_friendships.receiver " +
                                        "WHERE messenger_friendships.sender = " + userID);
                dFriends = dbClient.getTable();

                dbClient.setQuery("SELECT messenger_requests.sender,messenger_requests.receiver,users.username " +
                                        "FROM users " +
                                        "JOIN messenger_requests " +
                                        "ON users.id = messenger_requests.sender " +
                                        "WHERE messenger_requests.receiver = " + userID);
                dRequests = dbClient.getTable();

                dbClient.setQuery("SELECT rooms.*, room_active.active_users FROM rooms LEFT JOIN room_active ON (room_active.roomid = rooms.id) WHERE owner = @name");
                dbClient.addParameter("name", (string)dUserInfo["username"]);
                dRooms = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_pets WHERE user_id = " + userID + " AND room_id = 0");
                dPets = dbClient.getTable();

                dbClient.setQuery("SELECT * FROM user_quests WHERE user_id = " + userID + "");
                dQuests = dbClient.getTable();
                **/
            }

            Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();


            /**
             *             string achievementGroup;
            int achievementLevel;
            int achievementProgress;
            foreach (DataRow dRow in dAchievements.Rows)
            {
                achievementGroup = (string)dRow["group"];
                achievementLevel = (int)dRow["level"];
                achievementProgress = (int)dRow["progress"];

                UserAchievement achievement = new UserAchievement(achievementGroup, achievementLevel, achievementProgress);
                achievements.Add(achievementGroup, achievement);
            }
            **/
            List<uint> favouritedRooms = new List<uint>();
            /**
            uint favoritedRoomID;
            foreach (DataRow dRow in dFavouriteRooms.Rows)
            {
                favoritedRoomID = Convert.ToUInt32(dRow["room_id"]);
                favouritedRooms.Add(favoritedRoomID);
            }
            **/

            List<int> ignores = new List<int>();
            /**
            uint ignoredUserID;
            foreach (DataRow dRow in dIgnores.Rows)
            {
                ignoredUserID = Convert.ToUInt32(dRow["ignore_id"]);
                ignores.Add(ignoredUserID);
            }

            **/
            List<string> tags = new List<string>();
            /**
            string tag;
            foreach (DataRow dRow in dTags.Rows)
            {
                tag = (string)dRow["tag"];
                tags.Add(tag);
            }
            */
            Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();
            /**
            string subscriptionID;
            int expireTimestamp;
            foreach (DataRow dRow in dSubscriptions.Rows)
            {
                subscriptionID = (string)dRow["subscription_id"];
                expireTimestamp = (int)dRow["timestamp_expire"];

                subscriptions.Add(subscriptionID, new Subscription(subscriptionID, expireTimestamp));
            }
            **/
            List<Badge> badges = new List<Badge>();
            /**
            string badgeID;
            int slotID;
            foreach (DataRow dRow in dBadges.Rows)
            {
                badgeID = (string)dRow["badge_id"];
                slotID = (int)dRow["badge_slot"];
                badges.Add(new Badge(badgeID, slotID));
            }

            **/
            List<UserItem> inventory = new List<UserItem>();
            /**
            uint itemID;
            uint baseItem;
            string extraData;
            foreach (DataRow dRow in dInventory.Rows)
            {
                itemID = Convert.ToUInt32(dRow[0]);
                baseItem = Convert.ToUInt32(dRow[1]);
                if (!DBNull.Value.Equals(dRow[2]))
                    extraData = (string)dRow[2];
                else
                    extraData = string.Empty;

                inventory.Add(new UserItem(itemID, baseItem, extraData));
            }

            **/
            List<AvatarEffect> effects = new List<AvatarEffect>();
            /**
            int effectID;
            int duration;
            bool isActivated;
            double activatedTimeStamp;
            foreach (DataRow dRow in dEffects.Rows)
            {
                effectID = (int)dRow["effect_id"];
                duration = (int)dRow["total_duration"];
                isActivated = FirewindEnvironment.EnumToBool((string)dRow["is_activated"]);
                activatedTimeStamp = (double)dRow["activated_stamp"];

                effects.Add(new AvatarEffect(effectID, duration, isActivated, activatedTimeStamp));
            }

            **/
            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();

            string username = (string)dUserInfo["username"];
            /**
            UInt32 friendID;
            string friendName;
            string friendLook;
            string friendMotto;
            string friendLastOnline;
            foreach (DataRow dRow in dFriends.Rows)
            {
                friendID = Convert.ToUInt32(dRow["id"]);
                friendName = (string)dRow["username"];
                friendLook = (string)dRow["look"];
                friendMotto = (string)dRow["motto"];
                friendLastOnline = (string)dRow["last_online"];


                if (friendID == userID)
                    continue;


                if (!friends.ContainsKey(friendID))
                    friends.Add(friendID, new MessengerBuddy(friendID, friendName, friendLook, friendMotto, friendLastOnline));
            }
            **/
            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
            /**
            uint receiverID;
            uint senderID;
            string requestUsername;
            foreach (DataRow dRow in dRequests.Rows)
            {
                receiverID = Convert.ToUInt32(dRow["sender"]);
                senderID = Convert.ToUInt32(dRow["receiver"]);

                requestUsername = (string)dRow["username"];

                if (receiverID != userID)
                {
                    if (!requests.ContainsKey(receiverID))
                        requests.Add(receiverID, new MessengerRequest(userID, receiverID, requestUsername));
                }
                else
                {
                    if (!requests.ContainsKey(senderID))
                        requests.Add(senderID, new MessengerRequest(userID, senderID, requestUsername));
                }
            }
            **/
            List<RoomData> rooms = new List<RoomData>();
            /**
            uint roomID;
            foreach (DataRow dRow in dRooms.Rows)
            {
                roomID = Convert.ToUInt32(dRow["id"]);
                rooms.Add(FirewindEnvironment.GetGame().GetRoomManager().FetchRoomData(roomID, dRow));
            }

            **/
            Dictionary<uint, Pet> pets = new Dictionary<uint, Pet>();
            Dictionary<int, RentableBot> bots = new Dictionary<int, RentableBot>();
            /**
            Pet pet;
            foreach (DataRow dRow in dPets.Rows)
            {
                pet = Catalog.GeneratePetFromRow(dRow);
                pets.Add(pet.PetId, pet);
            }

            **/


            Dictionary<uint, int> quests = new Dictionary<uint, int>();
            /**
            uint questId;
            int progress;
            foreach (DataRow dRow in dQuests.Rows)
            {
                questId = Convert.ToUInt32(dRow["quest_id"]);
                progress = (int)dRow["progress"];
                quests.Add(questId, progress);
            }
            **/
            Hashtable songs = new Hashtable();
            /**
            //uint songItemID;
            //uint songID;
            //foreach (DataRow dRow in dSongs.Rows)
            //{
            //    songItemID = (uint)dRow[0];
            //    songID = (uint)dRow[1];

            //    SongItem song = new SongItem(songItemID, songID);
            //    songs.Add(songItemID, song);
            //}
            **/
            Habbo user = HabboFactory.GenerateHabbo(dUserInfo);

            dUserInfo = null;
            dAchievements = null;
            dFavouriteRooms = null;
            dIgnores = null;
            dTags = null;
            dSubscriptions = null;
            dBadges = null;
            dInventory = null;
            dEffects = null;
            dFriends = null;
            dRequests = null;
            dRooms = null;
            dPets = null;

            errorCode = 0;
            return new UserData(userID, achievements, favouritedRooms, ignores, tags, subscriptions, badges, inventory, effects, friends, requests, rooms, pets, quests, songs, user, bots);
        }
    }
}
