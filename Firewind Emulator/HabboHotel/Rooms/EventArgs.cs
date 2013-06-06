using System;
using Firewind.HabboHotel.Items;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.HabboHotel.Rooms.Units;

namespace Firewind.HabboHotel.Rooms
{
    public class UserSaysArgs : EventArgs
    {
        internal readonly RoomUser user;
        internal readonly string message;

        public UserSaysArgs(RoomUser user, string message)
        {
            this.user = user;
            this.message = message;
        }
    }

    public class ItemTriggeredArgs : EventArgs
    {
        internal readonly RoomUnit TriggeringUnit;
        internal readonly RoomItem TriggeringItem;

        public ItemTriggeredArgs(RoomUnit unit, RoomItem item)
        {
            this.TriggeringUnit = unit;
            this.TriggeringItem = item;
        }
    }

    public class TeamScoreChangedArgs : EventArgs
    {
        internal readonly int Points;
        internal readonly Team Team;
        internal readonly RoomUser user;

        public TeamScoreChangedArgs(int points, Team team, RoomUser user)
        {
            this.Points = points;
            this.Team = team;
            this.user = user;
        }
    }

    public class UnitWalksOnArgs : EventArgs
    {
        internal readonly RoomUnit user;

        public UnitWalksOnArgs(RoomUnit unit)
        {
            this.user = unit;
        }
    }
}
