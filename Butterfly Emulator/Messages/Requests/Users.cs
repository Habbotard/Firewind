using System;
using System.Data;
using Butterfly.HabboHotel.Misc;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;
using Butterfly.HabboHotel.Users.Badges;
using Database_Manager.Database.Session_Details.Interfaces;
using HabboEvents;
using Butterfly.Core;

namespace Butterfly.Messages
{
    partial class GameClientMessageHandler
    {
        internal void GetUserInfo()
        {
            GetResponse().Init(Outgoing.HabboInfomation);
            GetResponse().AppendUInt(Session.GetHabbo().Id);
            GetResponse().AppendString(Session.GetHabbo().Username);
            GetResponse().AppendString(Session.GetHabbo().Look);
            GetResponse().AppendString(Session.GetHabbo().Gender.ToUpper());
            GetResponse().AppendString(Session.GetHabbo().Motto);
            GetResponse().AppendString(Session.GetHabbo().RealName);
            GetResponse().AppendBoolean(false);
            GetResponse().AppendInt32(Session.GetHabbo().Respect);
            GetResponse().AppendInt32(Session.GetHabbo().DailyRespectPoints); // respect to give away
            GetResponse().AppendInt32(Session.GetHabbo().DailyPetRespectPoints);
            GetResponse().AppendBoolean(true);
            GetResponse().AppendString(Session.GetHabbo().LastOnline); // lul
            GetResponse().AppendBoolean(false);
            GetResponse().AppendBoolean(false);
            SendResponse();

            Boolean SafeChat = true;
            Boolean IsGuide = false;
            Boolean VoteInCompetitions = false;

            GetResponse().Init(Outgoing.Allowances);
            GetResponse().AppendInt32(3); // count
            GetResponse().AppendString("SAFE_CHAT");
            GetResponse().AppendBoolean(SafeChat);
            GetResponse().AppendString((!SafeChat) ? "requirement.unfulfilled.safety_quiz_1" : "");
            GetResponse().AppendString("USE_GUIDE_TOOL");
            GetResponse().AppendBoolean(IsGuide);
            GetResponse().AppendString((!IsGuide) ? "requirement.unfulfilled.helper_level_4" : "");
            GetResponse().AppendString("VOTE_IN_COMPETITIONS");
            GetResponse().AppendBoolean(VoteInCompetitions);
            GetResponse().AppendString((!VoteInCompetitions) ? "requirement.unfulfilled.helper_level_2" : "");
            SendResponse();

            GetResponse().Init(Outgoing.AchievementPoints);
            GetResponse().AppendInt32(Session.GetHabbo().AchievementPoints);
            SendResponse();

            InitMessenger();
        }

        internal void GetBalance()
        {
            Session.GetHabbo().UpdateCreditsBalance();
            Session.GetHabbo().UpdateActivityPointsBalance(false);
        }

        internal void GetSubscriptionData()
        {
            GetResponse().Init(Outgoing.SerializeClub);
            GetResponse().AppendString("club_habbo");

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip"))
            {
                Double Expire = Session.GetHabbo().GetSubscriptionManager().GetSubscription("habbo_vip").ExpireTime;
                Double TimeLeft = Expire - ButterflyEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                /*Double Initialized = Session.GetHabbo().GetSubscriptionManager().GetSubscription("habbo_vip").ini;
                Double TimeLeft = Expire - ButterflyEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);*/
                int MonthsLeft = TotalDaysLeft / 31;

                if (MonthsLeft >= 1) MonthsLeft--;

                GetResponse().AppendInt32(TotalDaysLeft - (MonthsLeft * 31)); // days left
                GetResponse().AppendInt32(2); // days multiplier
                GetResponse().AppendInt32(MonthsLeft); // months left
                GetResponse().AppendInt32(1); // ???
                GetResponse().AppendBoolean(true); // HC PRIVILEGE
                GetResponse().AppendBoolean(true); // VIP PRIVILEGE
                GetResponse().AppendInt32(0); // days i have on hc
                GetResponse().AppendInt32(0); // days i've purchased
                GetResponse().AppendInt32(495); // value 4 groups
            }
            else
            {
                GetResponse().AppendInt32(0);
                GetResponse().AppendInt32(0); // ??
                GetResponse().AppendInt32(0);
                GetResponse().AppendInt32(0); // type
                GetResponse().AppendBoolean(false);
                GetResponse().AppendBoolean(true);
                GetResponse().AppendInt32(0);
                GetResponse().AppendInt32(0); // days i have on hc
                GetResponse().AppendInt32(0); // days i have on vip
            }

            SendResponse();
        }

        internal void GetBadges()
        {
            Session.SendMessage(Session.GetHabbo().GetBadgeComponent().Serialize());
        }

        internal void UpdateBadges()
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("UPDATE user_badges SET badge_slot = 0 WHERE user_id = " + Session.GetHabbo().Id);
            }

            for(int i = 0; i < 5; i++)
            {
                int Slot = Request.PopWiredInt32();
                string Badge = Request.PopFixedString();

                if(Badge.Length == 0)
                    continue;

                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                    return;
                
                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    dbClient.setQuery("UPDATE user_badges SET badge_slot = " + Slot + " WHERE badge_id = @badge AND user_id = " + Session.GetHabbo().Id + "");
                    dbClient.addParameter("badge", Badge);
                    dbClient.runQuery();
                }
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.PROFILE_BADGE);

            ServerMessage Message = new ServerMessage(Outgoing.UpdateBadges);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendInt32(Session.GetHabbo().GetBadgeComponent().EquippedCount);

            foreach (Badge Badge in Session.GetHabbo().GetBadgeComponent().BadgeList.Values)
            {
                if (Badge.Slot <= 0)
                {
                    continue;
                }

                Message.AppendInt32(Badge.Slot);
                Message.AppendString(Badge.Code);
            }

            if (Session.GetHabbo().InRoom && ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId) != null)
            {
                ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendMessage(Message);
            }
            else
            {
                Session.SendMessage(Message);
            }
        }

        internal void GetAchievements()
        {
            ButterflyEnvironment.GetGame().GetAchievementManager().GetList(Session, Request);
        }

        internal void PrepareCampaing()
        {
            String campaingbadge = Request.PopFixedString();

            Response.Init(Outgoing.PrepareCampaing);
            Response.AppendString(campaingbadge); // tha badge
            Response.AppendBoolean(false); // received
            SendResponse();
        }

        internal void SendCampaingData()
        {
            try
            {
                // 2012-07-09 00:00,africaDesertFurniPromo;2012-07-16 00:00,africaSavannahFurniPromo;2012-07-23 00:00,africaJungleFurniPromo[0]africaDesertFurniPromo
                //String promo = Request.PopFixedString();
                String promo = "";
                //Logging.WriteLine(promo);
                string finalpromo = "africaSavannahFurniPromo";
                /*
                string[] possiblepromos = promo.Split(';');
                DateTime current = DateTime.Now;
                foreach (string s in possiblepromos)
                {
                    if (s == "")
                        break;
                    string[] s1 = s.Split(',');
                    String hour = s1[0];
                    if (hour == "")
                        break;
                    string[] hours = hour.Split(' ')[0].Split('-');
                    string promo2 = s1[1];
                    if (promo2 == "")
                        break;
                    int Year = int.Parse(hours[0]);
                    int Month = int.Parse(hours[1]);
                    int Day = int.Parse(hours[2]);
                    if (Year >= current.Year)
                    {
                        if (Month >= current.Month)
                        {
                            if (Day >= current.Day)
                                finalpromo = promo2;
                        }
                    }
                }*/

                Response.Init(Outgoing.SendCampaingData);
                Response.AppendString(promo);
                Response.AppendString(finalpromo);
                SendResponse();

                
            }
            catch (Exception e)
            {
                //Logging.WriteLine("Weird campaing not serialized!");
            }
        }

        internal void LoadProfile()
        {
            try
            {
                int UserId = Request.PopWiredInt32();
                Boolean IsMe = Request.PopWiredBoolean();
                /* don't know
                 * if (IsMe)
                    UserId = (int)Session.GetHabbo().Id;*/

                Habbo Data = ButterflyEnvironment.getHabboForId((uint)UserId);
                if (Data == null)
                {
                    Logging.WriteLine("can't get data por profile with userid = " + UserId);
                    return;
                }

                Response.Init(Outgoing.ProfileInformation);
                Response.AppendUInt(Data.Id);
                Response.AppendString(Data.Username);
                Response.AppendString(Data.Look);
                Response.AppendString(Data.Motto);
                Response.AppendString("12/12/12"); // created
                Response.AppendInt32(Data.AchievementPoints); // Achievement Points
                Response.AppendInt32(0); //friends
                //Response.AppendString(String.Empty);
                Response.AppendBoolean(Data.Id != Session.GetHabbo().Id); // is me maybe?
                Response.AppendInt32(0); // group count
                Response.AppendString("");
                Response.AppendInt32(-1);
                Response.AppendBoolean(true); // show it
                /* group:
                 * int(Id)
                 * string(Name)
                 * String(Badge)
                 * String(FirstColor)
                 * String(SecondColor)
                 * Boolean(Fav)
                 */
                // and achiv points after dat if groups or sth???
                SendResponse();
            }
            catch (Exception e)
            {

            }
        }

        internal void ChangeLook()
        {
            if (Session.GetHabbo().MutantPenalty)
            {
                Session.SendNotif("Because of a penalty or restriction on your account, you are not allowed to change your look.");
                return;
            }

            string Gender = Request.PopFixedString().ToUpper();
            string Look = ButterflyEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.PROFILE_CHANGE_LOOK);

            Session.GetHabbo().Look = ButterflyEnvironment.FilterFigure(Look);
            Session.GetHabbo().Gender = Gender.ToLower();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("UPDATE users SET look = @look, gender = @gender WHERE id = " + Session.GetHabbo().Id);
                dbClient.addParameter("look", Look);
                dbClient.addParameter("gender", Gender);
                dbClient.runQuery();
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressUserAchievement(Session, "ACH_AvatarLooks", 1);

            Session.GetMessageHandler().GetResponse().Init(Outgoing.UpdateUserInformation);
            Session.GetMessageHandler().GetResponse().AppendInt32(-1);
            Session.GetMessageHandler().GetResponse().AppendString(Session.GetHabbo().Look);
            Session.GetMessageHandler().GetResponse().AppendString(Session.GetHabbo().Gender.ToLower());
            Session.GetMessageHandler().GetResponse().AppendString(Session.GetHabbo().Motto);
            Session.GetMessageHandler().GetResponse().AppendInt32(Session.GetHabbo().AchievementPoints);
            Session.GetMessageHandler().SendResponse();

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerMessage RoomUpdate = new ServerMessage(Outgoing.UpdateUserInformation);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendString(Session.GetHabbo().Look);
                RoomUpdate.AppendString(Session.GetHabbo().Gender.ToLower());
                RoomUpdate.AppendString(Session.GetHabbo().Motto);
                RoomUpdate.AppendInt32(Session.GetHabbo().AchievementPoints);
                Room.SendMessage(RoomUpdate);
            }
        }

        internal void ChangeMotto()
        {
            string Motto = ButterflyEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (Motto.Length == 0 || Motto == Session.GetHabbo().Motto) // Prevents spam?
            {
                return;
            }

            //if (Motto.Length < 0)
            //{
            //    return; // trying to fk the client :D
            //} Congratulations. The string length can not hold calue < 0. Stupid -_-"

            Session.GetHabbo().Motto = Motto;


            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "'");
                dbClient.addParameter("motto", Motto);
                dbClient.runQuery();
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.PROFILE_CHANGE_MOTTO);

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerMessage RoomUpdate = new ServerMessage(Outgoing.UpdateUserInformation);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendString(Session.GetHabbo().Look);
                RoomUpdate.AppendString(Session.GetHabbo().Gender.ToLower());
                RoomUpdate.AppendString(Session.GetHabbo().Motto);
                RoomUpdate.AppendInt32(Session.GetHabbo().AchievementPoints);
                Room.SendMessage(RoomUpdate);
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressUserAchievement(Session, "ACH_Motto", 1);
        }

        internal void GetWardrobe()
        {
            GetResponse().Init(Outgoing.WardrobeData);
            GetResponse().AppendInt32(Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip") ? 1 : 0);

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_vip"))
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
                {
                    //dbClient.addParameter("userid", Session.GetHabbo().Id);
                    dbClient.setQuery("SELECT slot_id, look, gender FROM user_wardrobe WHERE user_id = " + Session.GetHabbo().Id);
                    DataTable WardrobeData = dbClient.getTable();

                    if (WardrobeData == null)
                    {
                        GetResponse().AppendInt32(0);
                    }
                    else
                    {
                        GetResponse().AppendInt32(WardrobeData.Rows.Count);

                        foreach (DataRow Row in WardrobeData.Rows)
                        {
                            GetResponse().AppendUInt(Convert.ToUInt32(Row["slot_id"]));
                            GetResponse().AppendString((string)Row["look"]);
                            GetResponse().AppendString((string)Row["gender"].ToString().ToUpper());
                        }
                    }
                }

                SendResponse();
            }
        }

        internal void SaveWardrobe()
        {
            uint SlotId = Request.PopWiredUInt();

            string Look = Request.PopFixedString();
            string Gender = Request.PopFixedString();

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("SELECT null FROM user_wardrobe WHERE user_id = " + Session.GetHabbo().Id + " AND slot_id = " + SlotId + "");
                dbClient.addParameter("look", Look);
                dbClient.addParameter("gender", Gender.ToUpper());

                if (dbClient.getRow() != null)
                {
                    dbClient.setQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = " + Session.GetHabbo().Id + " AND slot_id = " + SlotId + ";");
                    dbClient.addParameter("look", Look);
                    dbClient.addParameter("gender", Gender.ToUpper());
                    dbClient.runQuery();
                }
                else
                {
                    dbClient.setQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (" + Session.GetHabbo().Id + "," + SlotId + ",@look,@gender)");
                    dbClient.addParameter("look", Look);
                    dbClient.addParameter("gender", Gender.ToUpper());
                    dbClient.runQuery();
                }
            }
        }

        internal void GetPetsInventory()
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            Session.SendMessage(Session.GetHabbo().GetInventoryComponent().SerializePetInventory());
        }


        //internal void RegisterUsers()
        //{
        //    RequestHandlers.Add(7, new RequestHandler(GetUserInfo));
        //    RequestHandlers.Add(8, new RequestHandler(GetBalance));
        //    RequestHandlers.Add(26, new RequestHandler(GetSubscriptionData));

        //    RequestHandlers.Add(157, new RequestHandler(GetBadges));
        //    RequestHandlers.Add(158, new RequestHandler(UpdateBadges));
        //    RequestHandlers.Add(370, new RequestHandler(GetAchievements));

        //    RequestHandlers.Add(44, new RequestHandler(ChangeLook));
        //    RequestHandlers.Add(484, new RequestHandler(ChangeMotto));
        //    RequestHandlers.Add(375, new RequestHandler(GetWardrobe));
        //    RequestHandlers.Add(376, new RequestHandler(SaveWardrobe));

        //    RequestHandlers.Add(404, new RequestHandler(GetInventory));
        //    RequestHandlers.Add(3000, new RequestHandler(GetPetsInventory));

        //}

        //internal void UnregisterUser()
        //{
        //    RequestHandlers.Remove(7);
        //    RequestHandlers.Remove(8);
        //    RequestHandlers.Remove(26);
        //    RequestHandlers.Remove(157);
        //    RequestHandlers.Remove(158);
        //    RequestHandlers.Remove(370);
        //    RequestHandlers.Remove(44);
        //    RequestHandlers.Remove(375);
        //    RequestHandlers.Remove(376);
        //    RequestHandlers.Remove(404);
        //    RequestHandlers.Remove(3000);
        //}
    }
}
