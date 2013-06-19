using Firewind.HabboHotel.Pets;
using Firewind.HabboHotel.Rooms.Units.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firewind.HabboHotel.Rooms.Units
{
    class PetBot : RoomAI
    {
        internal int Type;
        internal int OwnerID;
        internal string OwnerName;

        internal Pet PetData;
        internal int RarityLevel;
        internal bool HaveSaddle;
        internal bool IsMounted;

        internal override int GetTypeID()
        {
            // "PET" - 2
            return 2;
        }

        public PetBot(int virtualID, Pet petData, Room room)
            : base(virtualID, room)
        {
            this.PetData = petData;
            this.RarityLevel = 0;
            this.HaveSaddle = false;
            this.IsMounted = false;

            // Initialize AI
            this.BaseAI = new PetAI(this);

            // Load data
            LoadFromPetData();
        }

        private void LoadFromPetData()
        {
            this.X = PetData.X;
            this.Y = PetData.Y;
            this.Z = PetData.Z;
            this.OwnerID = PetData.OwnerId;
            this.OwnerName = PetData.OwnerName;
            this.Type = (int)PetData.Type;
        }

        internal override void Serialize(Messages.ServerMessage Message)
        {
            base.Serialize(Message);

            Message.AppendInt32(Type); // subType
            Message.AppendInt32(OwnerID); // userid
            Message.AppendString(OwnerName); // username
            Message.AppendInt32(RarityLevel); // raritylevel

            Message.AppendBoolean(HaveSaddle);
            Message.AppendBoolean(IsMounted);

            Message.AppendBoolean(false); // can breed?
            Message.AppendBoolean(false);
            Message.AppendBoolean(false);
            Message.AppendBoolean(false); // something with breeding permissions

            Message.AppendInt32(0); // something to do with monster plants?
            Message.AppendString(""); // something to do with monster plants?
        }
    }
}
