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
using Firewind.Messages.Headers;
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

        internal RoomUnit(int virtualID, Room room) : this()
        {
            this.RoomID = (int)room.RoomId;
            this.VirtualID = virtualID;
            this._room = room;
        }

        public RoomUnit()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this.Statuses = new Dictionary<string, string>();

            this.AllowOverride = false;
            this.CanWalk = true;

            this.SqState = 3;
        }

        internal void Dispose()
        {
            Statuses.Clear();
            _room = null;
        }

        internal virtual void OnCycle()
        {

        }

        internal virtual void Chat(string Message, bool Shout)
        {
            InvokedChatMessage message = new InvokedChatMessage(this, Message, Shout);
            GetRoom().QueueChatMessage(message);
        }

        internal virtual void OnChat(InvokedChatMessage message)
        {
            string Message = message.message;

            int ChatHeader = Outgoing.Talk;

            if (message.shout)
                ChatHeader = Outgoing.Shout;

            string Site = "";

            ServerMessage ChatMessage = new ServerMessage(ChatHeader);

            ChatMessage.AppendInt32(VirtualID);
            ChatMessage.AppendString(Message);

            if (!string.IsNullOrEmpty(Site))
            {
                ChatMessage.AppendBoolean(false);
                ChatMessage.AppendBoolean(true);
                ChatMessage.AppendString(Site.Replace("http://", string.Empty));
                ChatMessage.AppendString(Site);
            }

            ChatMessage.AppendInt32(0);
            ChatMessage.AppendInt32(0);
            ChatMessage.AppendInt32(-1);

            GetRoom().GetRoomUserManager().TurnHeads(X, Y, VirtualID);

            foreach (RoomUser user in GetRoom().GetRoomUserManager().GetRoomUsers())
            {
                if (user.GetClient().GetHabbo().MutedUsers.Contains(ID))
                    continue;

                user.GetClient().SendMessage(ChatMessage);
            }


            message.Dispose();
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

        internal Point SquareInFront
        {
            get
            {
                Point Sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    Sq.Y--;
                }
                else if (RotBody == 2)
                {
                    Sq.X++;
                }
                else if (RotBody == 4)
                {
                    Sq.Y++;
                }
                else if (RotBody == 6)
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        internal Point SquareBehind
        {
            get
            {
                Point Sq = new Point(X, X);

                if (RotBody == 0)
                {
                    Sq.Y++;
                }
                else if (RotBody == 2)
                {
                    Sq.X--;
                }
                else if (RotBody == 4)
                {
                    Sq.Y--;
                }
                else if (RotBody == 6)
                {
                    Sq.X++;
                }

                return Sq;
            }
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
            Message.AppendInt32(RotBody); // ???
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
