using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firewind.HabboHotel.ChatMessageStorage;
using Firewind.HabboHotel.GameClients;
using Firewind.HabboHotel.Misc;
using Firewind.HabboHotel.Pathfinding;
using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.RoomBots;
using Firewind.HabboHotel.Rooms.Games;
using Firewind.Messages;
using System.Drawing;
using Firewind.Core;
using HabboEvents;
using System.Data;
using Database_Manager.Database.Session_Details.Interfaces;

namespace Firewind.HabboHotel.Rooms.Units
{
    public abstract class RoomUnit : IEquatable<RoomUnit>
    {
        // ID = userID if user, botID if bot
        internal int ID;
        internal int VirtualID;
        internal int RoomID;

        internal int X;//byte
        internal int Y;//byte
        internal double Z;
        internal byte SqState;

        internal int RotHead;//byte
        internal int RotBody;//byte

        internal string Name;
        internal string Motto;
        internal string Figure;

        internal bool CanWalk;
        internal bool AllowOverride;
        internal bool TeleportEnabled;

        internal int GoalX;//byte
        internal int GoalY;//byte

        internal Boolean SetStep;
        internal int SetX;//byte
        internal int SetY;//byte
        internal double SetZ;

        internal abstract int GetTypeID(); // 1 = habbo, 2 = pet, 3 = public bot, 4 = private bot?

        internal Point Coordinate
        {
            get
            {
                return new Point(X, Y);
            }
        }

        public bool Equals(RoomUnit comparedUser)
        {
            return (comparedUser.VirtualID == this.VirtualID);
        }

        internal Boolean IsWalking;
        internal Boolean UpdateNeeded;
        internal Boolean IsAsleep;

        internal Dictionary<string, string> Statuses;

        internal RoomUnit(int virtualID, Room room)
        {
            this.RoomID = (int)room.RoomId;
            this.VirtualID = virtualID;
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this.Statuses = new Dictionary<string, string>();
            this._room = room;

            this.AllowOverride = false;
            this.CanWalk = true;

            this.SqState = 3;
        }

        internal void Dispose()
        {
            Statuses.Clear();
            _room = null;
        }

        internal void Chat(GameClient Session, string Message, bool Shout)
        {

        }

        internal void OnChat(InvokedChatMessage message)
        {
        }

        internal void ClearMovement(bool Update)
        {
            IsWalking = false;
            Statuses.Remove("mv");
            GoalX = 0;
            GoalY = 0;
            SetStep = false;
            SetX = 0;
            SetY = 0;
            SetZ = 0;

            if (Update)
            {
                UpdateNeeded = true;
            }
        }

        internal void MoveTo(Point c)
        {
            MoveTo(c.X, c.Y);
        }

        internal void MoveTo(int pX, int pY, bool pOverride)
        {
            if (GetRoom().GetGameMap().SquareHasUsers(pX, pY) && !pOverride)
                return;

            if (TeleportEnabled)
            {
                GetRoom().SendMessage(GetRoom().GetRoomItemHandler().UpdateUnitOnRoller(this, new Point(pX, pY), 0, GetRoom().GetGameMap().SqAbsoluteHeight(GoalX, GoalY)));
                GetRoom().GetRoomUserManager().UpdateUserStatus(this, false);
                return;
            }

            IsWalking = true;
            GoalX = pX;
            GoalY = pY;
        }

        internal void MoveTo(int pX, int pY)
        {
            MoveTo(pX, pY, false);
        }

        internal void UnlockWalking()
        {
            this.AllowOverride = false;
            this.CanWalk = true;
        }

        internal void SetPos(int pX, int pY, double pZ)
        {
            this.X = pX;
            this.Y = pY;
            this.Z = pZ;
        }

        internal void SetRot(int Rotation)
        {
            SetRot(Rotation, false); //**************
        }

        internal void SetRot(int Rotation, bool HeadOnly)
        {
            if (Statuses.ContainsKey("lay") || IsWalking)
            {
                return;
            }

            int diff = this.RotBody - Rotation;

            this.RotHead = this.RotBody;

            if (Statuses.ContainsKey("sit") || HeadOnly)
            {
                if (RotBody == 2 || RotBody == 4)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
                else if (RotBody == 0 || RotBody == 6)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
            }
            else if (diff <= -2 || diff >= 2)
            {
                this.RotHead = Rotation;
                this.RotBody = Rotation;
            }
            else
            {
                this.RotHead = Rotation;
            }

            this.UpdateNeeded = true;
        }

        internal void AddStatus(string Key, string Value)
        {
            Statuses[Key] = Value;
        }

        internal void RemoveStatus(string Key)
        {
            if (Statuses.ContainsKey(Key))
            {
                Statuses.Remove(Key);
            }
        }

        //internal void ResetStatus()
        //{
        //    Statusses = new Dictionary<string, string>();
        //}

        internal virtual void Serialize(ServerMessage Message)
        {
            Message.AppendInt32(ID);
            Message.AppendString(Name);
            Message.AppendString(Motto);
            Message.AppendString(Figure);
            Message.AppendInt32(VirtualID);
            Message.AppendInt32(X);
            Message.AppendInt32(Y);
            Message.AppendString(TextHandling.GetString(Z));
            Message.AppendInt32(0); // ???
            Message.AppendInt32(GetTypeID());

            // Rest is up to the derived classes
        }

        internal void SerializeStatus(ServerMessage Message)
        {
            Message.AppendInt32(VirtualID);
            Message.AppendInt32(X);
            Message.AppendInt32(Y);
            Message.AppendString(TextHandling.GetString(Z));
            Message.AppendInt32(RotHead);
            Message.AppendInt32(RotBody);
            StringBuilder StatusComposer = new StringBuilder();
            StatusComposer.Append("/");

            foreach (KeyValuePair<string, string> Status in Statuses)
            {
                StatusComposer.Append(Status.Key);

                if (Status.Value != string.Empty)
                {
                    StatusComposer.Append(" ");
                    StatusComposer.Append(Status.Value);
                }

                StatusComposer.Append("/");
            }

            StatusComposer.Append("/");
            Message.AppendString(StatusComposer.ToString());

            RemoveStatus("sign"); // fix for infinitive signs
        }

        internal void SerializeStatus(ServerMessage Message, String Status)
        {
            Message.AppendInt32(VirtualID);
            Message.AppendInt32(X);
            Message.AppendInt32(Y);
            Message.AppendString(TextHandling.GetString(Z));
            Message.AppendInt32(RotHead);
            Message.AppendInt32(RotBody);
            StringBuilder StatusComposer = new StringBuilder();
            Message.AppendString(Status);
        }

        private Room _room;
        internal Room GetRoom()
        {
            if (_room == null)
                _room = FirewindEnvironment.GetGame().GetRoomManager().GetRoom((uint)RoomID);
            return _room;
        }
    }
}
