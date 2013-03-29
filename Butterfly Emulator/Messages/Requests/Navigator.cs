using Butterfly.HabboHotel.Navigators;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Core;
using Butterfly.HabboHotel.Pathfinding;
using System;
using System.Threading;
using Database_Manager.Database.Session_Details.Interfaces;
using HabboEvents;

namespace Butterfly.Messages
{
    partial class GameClientMessageHandler
    {
        internal void AddFavorite()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            uint Id = Request.PopWiredUInt();

            RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || Session.GetHabbo().FavoriteRooms.Contains(Id) || Data.Type == "public")
            {
                // TODO: Upgrade 
                GetResponse().Init(33);
                GetResponse().AppendInt32(-9001);
                //SendResponse();

                return;
            }

            GetResponse().Init(Outgoing.FavsUpdate);
            GetResponse().AppendUInt(Id);
            GetResponse().AppendBoolean(true);
            SendResponse();

            Session.GetHabbo().FavoriteRooms.Add(Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + Session.GetHabbo().Id + "," + Id + ")");
            }
        }

        internal void RemoveFavorite()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            uint Id = Request.PopWiredUInt();

            Session.GetHabbo().FavoriteRooms.Remove(Id);

            GetResponse().Init(Outgoing.FavsUpdate);
            GetResponse().AppendUInt(Id);
            GetResponse().AppendBoolean(false);
            SendResponse();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.runFastQuery("DELETE FROM user_favorites WHERE user_id = " + Session.GetHabbo().Id + " AND room_id = " + Id + "");
            }
        }

        internal void GoToHotelView()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            if (Session.GetHabbo().InRoom)
            {
                Room currentRoom = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                if (currentRoom != null)
                    currentRoom.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);
                Session.CurrentRoomUserID = -1;
            }
        }

        internal void GetFlatCats()
        {
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeFlatCategories(Session));
        }

        internal void GetPubs()
        {
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializePublicRooms());

            uint HR = Session.GetHabbo().HomeRoom;

            /*if (HR > 0)
            {
                Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(HR);

                if (Room != null)
                {
                    this.PrepareRoomForUser(Room.RoomId, Room.Password);
                    this.GetRoomData2();
                    this.GetRoomData3();
                }
            }*/
        }

        internal void GetRoomInfo()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
     
            uint RoomId = Request.PopWiredUInt();
            bool unk = Request.PopWiredBoolean();
            bool unk2 = Request.PopWiredBoolean();

            RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null)
            {
                return;
            }

            GetResponse().Init(454);
            GetResponse().AppendInt32(0);
            Data.Serialize(GetResponse(), false);
            SendResponse();

        }

        internal void PopularRoomsSearch()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeNavigator(Session, int.Parse(Request.PopFixedString())));
        }

        internal void GetHighRatedRooms()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeNavigator(Session, -2));
        }

        internal void GetFriendsRooms()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeNavigator(Session, -4));
        }

        internal void GetRoomsWithFriends()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeNavigator(Session, -5));
        }

        internal void GetOwnRooms()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeNavigator(Session, -3));
        }

        internal void GetFavoriteRooms()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeFavoriteRooms(Session));
        }

        internal void GetRecentRooms()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeRecentRooms(Session));
        }

        internal void GetEvents()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            int Category = int.Parse(Request.PopFixedString());

            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeEventListing(Category));
        }

        internal void GetPopularTags()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializePopularRoomTags());
        }

        internal void PerformSearch()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeSearchResults(Request.PopFixedString()));
        }

        internal void PerformSearch2()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            int junk = Request.PopWiredInt32();
            Session.SendMessage(ButterflyEnvironment.GetGame().GetNavigator().SerializeSearchResults(Request.PopFixedString()));
        }

        internal void OpenFlat()
        {
            Butterfly.HabboHotel.Users.Habbo targetHabbo = Session.GetHabbo();
            if (targetHabbo == null)
            {
                return;
            }
            uint Id = Request.PopWiredUInt();
            string Password = Request.PopFixedString();
            int Junk = Request.PopWiredInt32();
            Logging.WriteLine("Loading room [" + Id + "]");
            RoomData Data = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "private")
                return;

            PrepareRoomForUser(Id, Password);
        }

        //internal void RegisterNavigator()
        //{
        //    RequestHandlers.Add(391, new RequestHandler(OpenFlat));
        //    RequestHandlers.Add(19, new RequestHandler(AddFavorite));
        //    RequestHandlers.Add(20, new RequestHandler(RemoveFavorite));
        //    RequestHandlers.Add(53, new RequestHandler(GoToHotelView));
        //    RequestHandlers.Add(151, new RequestHandler(GetFlatCats));
        //    RequestHandlers.Add(233, new RequestHandler(EnterInquiredRoom));
        //    RequestHandlers.Add(380, new RequestHandler(GetPubs));
        //    RequestHandlers.Add(385, new RequestHandler(GetRoomInfo));
        //    RequestHandlers.Add(430, new RequestHandler(GetPopularRooms));
        //    RequestHandlers.Add(431, new RequestHandler(GetHighRatedRooms));
        //    RequestHandlers.Add(432, new RequestHandler(GetFriendsRooms));
        //    RequestHandlers.Add(433, new RequestHandler(GetRoomsWithFriends));
        //    RequestHandlers.Add(434, new RequestHandler(GetOwnRooms));
        //    RequestHandlers.Add(435, new RequestHandler(GetFavoriteRooms));
        //    RequestHandlers.Add(436, new RequestHandler(GetRecentRooms));
        //    RequestHandlers.Add(439, new RequestHandler(GetEvents));
        //    RequestHandlers.Add(382, new RequestHandler(GetPopularTags));
        //    RequestHandlers.Add(437, new RequestHandler(PerformSearch));
        //    RequestHandlers.Add(438, new RequestHandler(PerformSearch2));
        //}

        //internal void UnregisterNavigator()
        //{
        //    RequestHandlers.Remove(391);
        //    RequestHandlers.Remove(19);
        //    RequestHandlers.Remove(20);
        //    RequestHandlers.Remove(53);
        //    RequestHandlers.Remove(151);
        //    RequestHandlers.Remove(233);
        //    RequestHandlers.Remove(380);
        //    RequestHandlers.Remove(385);
        //    RequestHandlers.Remove(430);
        //    RequestHandlers.Remove(431);
        //    RequestHandlers.Remove(432);
        //    RequestHandlers.Remove(433);
        //    RequestHandlers.Remove(434);
        //    RequestHandlers.Remove(435);
        //    RequestHandlers.Remove(436);
        //    RequestHandlers.Remove(439);
        //    RequestHandlers.Remove(382);
        //    RequestHandlers.Remove(437);
        //    RequestHandlers.Remove(438);
        //}
    }
}
