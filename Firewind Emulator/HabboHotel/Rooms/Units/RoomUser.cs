using Database_Manager.Database.Session_Details.Interfaces;
using Firewind.Core;
using Firewind.HabboHotel.ChatMessageStorage;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Misc;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Users;
using Firewind.Messages;
using HabboEvents;
using System;
using System.Collections.Generic;
using System.Data;
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
        public bool throwBallAtGoal;
        internal FreezePowerUp banzaiPowerUp;
        public bool Freezed;
        public int FreezeCounter;
        public bool shieldActive;
        public int shieldCounter;
        public int FreezeLives;
        public bool IsTrading;
        public bool moonwalkEnabled;
        private int FloodCount;

        internal override int GetTypeID()
        {
            // "USER" - 1
            return 1;
        }

        public RoomUser(int virtualID, GameClient client, Room room) : base(virtualID, room)
        {
            // Base constructor already called
            this.Client = client;

            Habbo habbo = client.GetHabbo();
            this.ID = habbo.Id;
            this.Name = habbo.Username;
            this.Figure = habbo.Look;
            this.Motto = habbo.Motto;

        }

        internal override void OnCycle()
        {
            base.OnCycle();

            if (!IsValidUser())
            {
                if (GetClient() != null)
                   GetRoom().GetRoomUserManager().RemoveUserFromRoom(GetClient(), false, false);
                else
                    GetRoom().GetRoomUserManager().RemoveRoomUser(this);
            }

            IdleTime++;
            if (!IsAsleep && IdleTime >= 600)
            {
                IsAsleep = true;

                ServerMessage FallAsleep = new ServerMessage(Outgoing.IdleStatus);
                FallAsleep.AppendInt32(VirtualID);
                FallAsleep.AppendBoolean(true);
                GetRoom().SendMessage(FallAsleep);
            }

            // TODO: Re-add idle kicking

            if (CarryItemID > 0)
            {
                CarryTimer--;
                if (CarryTimer <= 0)
                    CarryItem(0);
            }
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

        private bool IsValidUser()
        {
            if (GetClient() == null)
                return false;
            if (GetClient().GetHabbo() == null)
                return false;
            if (GetClient().GetHabbo().CurrentRoomId != GetRoom().RoomId)
                return false;

            return true;
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

        internal override void Chat(string Message, bool Shout)
        {

            if (Client != null)
            {
                if (Client.GetHabbo().Rank < 5)
                {
                    if (GetRoom().RoomMuted)
                        return;
                }
            }

            Unidle();

                Users.Habbo clientUser = GetClient().GetHabbo();
                if (clientUser.Muted)
                {
                    GetClient().SendNotif("You are muted.");
                    return;
                }

                if (Message.StartsWith(":"))
                {
                    string[] parsedCommand = Message.Split(' ');
                    if (ChatCommandRegister.IsChatCommand(parsedCommand[0].ToLower().Substring(1)))
                    {
                        try
                        {
                            ChatCommandHandler handler = new ChatCommandHandler(Message.Split(' '), Client);

                            if (handler.WasExecuted())
                            {
                                //Logging.LogMessage(string.Format("User {0} issued command {1}", GetUsername(), Message));
                                if (Client.GetHabbo().Rank > 5)
                                {
                                    FirewindEnvironment.GetGame().GetModerationTool().LogStaffEntry(Client.GetHabbo().Username, string.Empty, "Chat command", string.Format("Issued chat command {0}", Message));
                                }
                                return;
                            }
                        }
                        catch (Exception x) { Logging.LogException("In-game command error: " + x.ToString()); }
                    }
                    if (FirewindEnvironment.IsHabin)
                    {
                        bool result;
                        using (IQueryAdapter dbClient = FirewindEnvironment.GetDatabaseManager().getQueryreactor())
                        {
                            dbClient.setQuery("SELECT id FROM users WHERE (hpo = '1' OR hpo = '1' OR hds = '1' OR hmg = '1' OR hmb = '1' OR ele = '1' OR lar = '1') AND username = @name");
                            dbClient.addParameter("name", Client.GetHabbo().Username);
                            result = dbClient.findsResult();
                            string command = parsedCommand[0].Substring(1);
                            // Fuck this command system, we make our own!
                            switch (command)
                            {
                                case "relationship":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    dbClient.setQuery("SELECT sender_id FROM users_relationships WHERE (recipent_id = @myid OR sender_id = @myid) AND accepted = '0'");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    if (dbClient.findsResult())
                                    {
                                        Client.SendMOTD("Du har allerede spurt om et forhold med denne personen, vennligst vent på et svar.");
                                        return;
                                    }
                                    dbClient.setQuery("SELECT id FROM users WHERE username = @name");
                                    dbClient.addParameter("name", parsedCommand[1]);
                                    int id = dbClient.getInteger();

                                    dbClient.setQuery("INSERT IGNORE INTO users_relationships(sender_id,recipent_id) VALUES(@myid,@hisid)");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    dbClient.addParameter("hisid", id);
                                    dbClient.runQuery();

                                    Client.SendMOTD("Du har sendt en forespørsel til " + parsedCommand[1]);
                                    return;

                                case "mystatus":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    if (false)
                                    {
                                        Client.SendMOTD("Du er i et forhold med {0}, vil du avslutte forholdet skriv :remove {0}");
                                        return;
                                    }
                                    StringBuilder statusMessage = new StringBuilder();
                                    statusMessage.AppendLine("Du har følgende forespørsler:");
                                    dbClient.setQuery("SELECT sender_id FROM users_relationships WHERE accepted = '0' AND recipent_id = @myid LIMIT 6");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    DataTable table = dbClient.getTable();
                                    foreach (DataRow row in table.Rows)
                                    {
                                        statusMessage.AppendLine(FirewindEnvironment.getHabboForId(Convert.ToInt32(row[0])).Username);
                                    }
                                    statusMessage.AppendLine("Du kan maks ha 6 forespørsler på en gang.");
                                    statusMessage.AppendLine("Skriv :accept navn for å akseptere en forespørsel.");
                                    Client.SendMOTD(statusMessage.ToString());
                                    return;

                                case "accept":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    dbClient.setQuery("SELECT sender_id FROM users_relationships WHERE (recipent_id = @myid OR sender_id = @myid) AND accepted = '1'");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    if (dbClient.findsResult())
                                    {
                                        Client.SendMOTD("Du er allerede i et forhold!");
                                        return;
                                    }
                                    dbClient.setQuery("UPDATE users_relationships SET accepted = '1' WHERE sender_id = @sid AND recipent_id = @myid LIMIT 1");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    dbClient.addParameter("sid", FirewindEnvironment.getHabboForName(parsedCommand[1]).Id);
                                    dbClient.runQuery();
                                    Client.SendMOTD("Du er nå i et forhold med " + parsedCommand[1]);
                                    return;

                                case "decline":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    dbClient.setQuery("DELETE FROM users_relationships WHERE sender_id = @sid AND recipent_id = @myid AND accepted = '0' LIMIT 1");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    dbClient.addParameter("sid", FirewindEnvironment.getHabboForName(parsedCommand[1]).Id);
                                    dbClient.runQuery();
                                    Client.SendMOTD("Du har avslått " + parsedCommand[1]);
                                    return;

                                case "declineall":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    dbClient.setQuery("DELETE FROM users_relationships WHERE recipent_id = @myid AND accepted = '0' LIMIT 1");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    dbClient.runQuery();
                                    Client.SendMOTD("Du har avslått alle.");
                                    return;

                                case "status":
                                    int userID = FirewindEnvironment.getHabboForName(parsedCommand[1]).Id;
                                    dbClient.setQuery("SELECT sender_id,recipent_id FROM users_relationships WHERE (recipent_id = @userid OR sender_id = @userid) AND accepted = '1' LIMIT 1");
                                    dbClient.addParameter("userid", userID);
                                    DataRow resultRow = dbClient.getRow();

                                    if (resultRow == null)
                                        Client.SendMOTD(parsedCommand[1] + " er singel.");
                                    else
                                    {
                                        bool isSender = Convert.ToUInt32(resultRow[0]) == userID;
                                        Client.SendMOTD(parsedCommand[1] + " er i et forhold med " + (isSender ? FirewindEnvironment.getHabboForId(Convert.ToInt32(resultRow[1])).Username : FirewindEnvironment.getHabboForId(Convert.ToInt32(resultRow[0])).Username));
                                    }
                                    return;

                                case "removerelationship":
                                    if (!result)
                                    {
                                        Client.SendMOTD("Du må være medlem av Mafia eller Police for å ha kommandoene til forhold.");
                                        break;
                                    }
                                    dbClient.setQuery("DELETE FROM users_relationships WHERE accepted = '1' AND (recipent_id = @myid OR sender_id = @myid) LIMIT 1");
                                    dbClient.addParameter("myid", Client.GetHabbo().Id);
                                    dbClient.runQuery();
                                    Client.SendMOTD("Du er ikke lenger i noen forhold.");
                                    return;
                            }
                        }
                    }
                }


                uint rank = 1;
                Message = LanguageLocale.FilterSwearwords(Message);
                if (Client != null && Client.GetHabbo() != null)
                    rank = Client.GetHabbo().Rank;
                TimeSpan SinceLastMessage = DateTime.Now - clientUser.spamFloodTime;
                if (SinceLastMessage.TotalSeconds > clientUser.spamProtectionTime && clientUser.spamProtectionBol == true)
                {
                    FloodCount = 0;
                    clientUser.spamProtectionBol = false;
                    clientUser.spamProtectionAbuse = 0;
                }
                else
                {
                    if (SinceLastMessage.TotalSeconds > 4)
                        FloodCount = 0;
                }

                if (SinceLastMessage.TotalSeconds < clientUser.spamProtectionTime && clientUser.spamProtectionBol == true)
                {
                    ServerMessage Packet = new ServerMessage(Outgoing.FloodFilter);
                    int timeToWait = clientUser.spamProtectionTime - SinceLastMessage.Seconds;
                    Packet.AppendInt32(timeToWait); //Blocked for X sec
                    GetClient().SendMessage(Packet);

                    if (FirewindEnvironment.spamBans == true)
                    {
                        clientUser.spamProtectionAbuse++;
                        GameClient toBan;
                        toBan = FirewindEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().Username);
                        if (clientUser.spamProtectionAbuse >= FirewindEnvironment.spamBans_limit)
                        {
                            FirewindEnvironment.GetGame().GetBanManager().BanUser(toBan, "SPAM*ABUSE", 800, LanguageLocale.GetValue("flood.banmessage"), false);
                        }
                        else
                        {
                            toBan.SendNotif(LanguageLocale.GetValue("flood.pleasewait").Replace("%secs%", Convert.ToString(timeToWait)));
                        }
                    }
                    return;
                }

                if (SinceLastMessage.TotalSeconds < 4 && FloodCount > 5 && rank < 5)
                {
                    ServerMessage Packet = new ServerMessage(Outgoing.FloodFilter);
                    clientUser.spamProtectionCount += 1;
                    if (clientUser.spamProtectionCount % 2 == 0)
                    {
                        clientUser.spamProtectionTime = (10 * clientUser.spamProtectionCount);
                    }
                    else
                    {
                        clientUser.spamProtectionTime = 10 * (clientUser.spamProtectionCount - 1);
                    }
                    clientUser.spamProtectionBol = true;
                    Packet.AppendInt32(clientUser.spamProtectionTime - SinceLastMessage.Seconds); //Blocked for X sec
                    GetClient().SendMessage(Packet);
                    return;
                }

                clientUser.spamFloodTime = DateTime.Now;
                FloodCount++;

                FirewindEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Client, HabboHotel.Quests.QuestType.SOCIAL_CHAT);

                GetClient().GetHabbo().GetChatMessageManager().AddMessage(ChatMessageFactory.CreateMessage(Message, this.GetClient(), this.GetRoom()));

                base.Chat(Message, Shout);
        }

        internal override void OnChat(InvokedChatMessage message)
        {
            // TODO: fire event for bots
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
