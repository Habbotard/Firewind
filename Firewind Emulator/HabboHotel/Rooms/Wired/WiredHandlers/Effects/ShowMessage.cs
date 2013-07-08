using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Firewind.Messages;
using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.Messages.Headers;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    class ShowMessage : IWiredTrigger, IWiredEffect
    {
        private WiredHandler handler;
        private uint itemID;
        
        private string message;

        public ShowMessage(string message, WiredHandler handler, uint itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
        }

        public bool Handle(RoomUnit unit, Team team, RoomItem item)
        {
            RoomUser user = unit as RoomUser;
            if (user != null && user.GetClient() != null)
            {
                ServerMessage servermsg = new ServerMessage();
                servermsg.Init(Outgoing.Whisp);
                servermsg.AppendInt32(user.VirtualID);
                servermsg.AppendString(message);
                servermsg.AppendInt32(0);
                servermsg.AppendInt32(0);
                servermsg.AppendInt32(-1);

                user.GetClient().SendMessage(servermsg);
                handler.OnEvent(itemID);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            message = null;
        }

        public bool IsSpecial(out SpecialEffects function)
        {
            function = SpecialEffects.None;
            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, (int)itemID, "integer", string.Empty, message, false);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.setQuery("SELECT trigger_data FROM trigger_item WHERE trigger_id = @id ");
            dbClient.addParameter("id", (int)this.itemID);
            this.message = dbClient.getString();
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.runFastQuery("DELETE FROM trigger_item WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
