using System;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Database_Manager.Database.Session_Details.Interfaces;

namespace Butterfly.HabboHotel.ChatMessageStorage
{
    class ChatMessageFactory
    {
        internal static ChatMessage CreateMessage(string message, GameClient user, Room room)
        {
            uint userID = user.GetHabbo().Id;
            string username = user.GetHabbo().Username;
            uint roomID = room.RoomId;
            string roomName = room.Name;
            bool isPublic = room.IsPublic;
            DateTime timeSpoken = DateTime.Now;

            ChatMessage chatMessage = new ChatMessage(userID, username, roomID, roomName, isPublic, message, timeSpoken);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().getQueryreactor())
            {
                dbClient.setQuery("INSERT into `chatlogs`(`user_id`, `room_id`, `hour`, `minute`, `full_date`, `timestamp`, `message`, `user_name`) VALUES(" + userID + ", " + roomID + ", " + timeSpoken.Hour + ", " + timeSpoken.Minute + ", '" + timeSpoken.ToString() + "', " + ButterflyEnvironment.GetUnixTimestamp() + ", @msg, '" + user.GetHabbo().Username + "');");
                dbClient.addParameter("msg", message);
                dbClient.runQuery();
            }

            return chatMessage;
        }
    }
}
