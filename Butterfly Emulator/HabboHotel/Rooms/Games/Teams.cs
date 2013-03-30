using System;
using System.Text;
using System.Collections.Generic;
using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Games
{
    public enum Team
    {
        none = 0,
        red = 1,
        green = 2,
        blue = 3,
        yellow = 4
    }

    public class TeamManager
    {
        public string Game;
        public List<RoomUser> BlueTeam;
        public List<RoomUser> RedTeam;
        public List<RoomUser> YellowTeam;
        public List<RoomUser> GreenTeam;

        public static TeamManager createTeamforGame(string Game)
        {
            TeamManager t = new TeamManager();
            t.Game = Game;
            t.BlueTeam = new List<RoomUser>();
            t.RedTeam = new List<RoomUser>();
            t.GreenTeam = new List<RoomUser>();
            t.YellowTeam = new List<RoomUser>();
            return t;
        }

        public bool CanEnterOnTeam(Team t)
        {
            if (t.Equals(Team.blue))
                return (BlueTeam.Count < 5);
            else if (t.Equals(Team.red))
                return (RedTeam.Count < 5);
            else if (t.Equals(Team.yellow))
                return (YellowTeam.Count < 5);
            else if (t.Equals(Team.green))
                return (GreenTeam.Count < 5);
            return false;
        }

        public void AddUser(RoomUser user)
        {
            //Logging.WriteLine("Add user to team!  (" + Game + ")");
            if (user.team.Equals(Team.blue))
                this.BlueTeam.Add(user);
            else if (user.team.Equals(Team.red))
                this.RedTeam.Add(user);
            else if (user.team.Equals(Team.yellow))
                this.YellowTeam.Add(user);
            else if (user.team.Equals(Team.green))
                this.GreenTeam.Add(user);

            switch (Game.ToLower())
            {
                case "banzai":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        foreach (RoomItem Item in room.GetRoomItemHandler().mFloorItems.Values)
                        {
                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateblue))
                            {
                                ((StringData)Item.data).Data = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigatered))
                            {
                                ((StringData)Item.data).Data = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigategreen))
                            {
                                ((StringData)Item.data).Data = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateyellow))
                            {
                                ((StringData)Item.data).Data = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
                case "freeze":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        foreach (RoomItem Item in room.GetRoomItemHandler().mFloorItems.Values)
                        {
                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezebluegate))
                            {
                                ((StringData)Item.data).Data = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezeredgate))
                            {
                                ((StringData)Item.data).Data = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezegreengate))
                            {
                                ((StringData)Item.data).Data = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezeyellowgate))
                            {
                                ((StringData)Item.data).Data = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
            }
        }

        public void OnUserLeave(RoomUser user)
        {
            //Logging.WriteLine("remove user from team! (" + Game + ")");
            if (user.team.Equals(Team.blue))
                this.BlueTeam.Remove(user);
            else if (user.team.Equals(Team.red))
                this.RedTeam.Remove(user);
            else if (user.team.Equals(Team.yellow))
                this.YellowTeam.Remove(user);
            else if (user.team.Equals(Team.green))
                this.GreenTeam.Remove(user);

            switch (Game.ToLower())
            {
                case "banzai":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        foreach (RoomItem Item in room.GetRoomItemHandler().mFloorItems.Values)
                        {
                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateblue))
                            {
                                ((StringData)Item.data).Data = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigatered))
                            {
                                ((StringData)Item.data).Data = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigategreen))
                            {
                                ((StringData)Item.data).Data = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.banzaigateyellow))
                            {
                                ((StringData)Item.data).Data = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
                case "freeze":
                    {
                        Room room = user.GetClient().GetHabbo().CurrentRoom;
                        foreach (RoomItem Item in room.GetRoomItemHandler().mFloorItems.Values)
                        {
                            if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezebluegate))
                            {
                                ((StringData)Item.data).Data = BlueTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezeredgate))
                            {
                                ((StringData)Item.data).Data = RedTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezegreengate))
                            {
                                ((StringData)Item.data).Data = GreenTeam.Count.ToString();
                                Item.UpdateState();
                            }
                            else if (Item.GetBaseItem().InteractionType.Equals(InteractionType.freezeyellowgate))
                            {
                                ((StringData)Item.data).Data = YellowTeam.Count.ToString();
                                Item.UpdateState();
                            }
                        }
                        break;
                    }
            }
        }

    }
}
