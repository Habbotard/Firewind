using System;

using Firewind.Messages;
using Firewind.HabboHotel.Rooms;
using Firewind;
using Firewind.Core;

namespace Firewind.HabboHotel.Navigators
{
    internal enum PublicImageType
    {
        INTERNAL = 0,
        EXTERNAL = 1
    }

    internal enum PublicItemType
    {
        //TAG(1), FLAT(2), PUBLIC_FLAT(3), CATEGORY(4);
        NONE = 0,
        TAG = 1,
        FLAT = 2,
        PUBLIC_FLAT = 3,
        CATEGORY = 4
    }

    internal class PublicItem
    {
        private readonly Int32 BannerId;

        internal int Type;

        internal string Caption;
        internal string Image;
        internal PublicImageType ImageType;

        internal UInt32 RoomId;

        internal Int32 ParentId;

        internal Int32 Id
        {
            get { return BannerId; }
        }

        internal RoomData RoomData
        {
            get
            {
                if (RoomId == 0)
                {
                    return new RoomData();
                }

                if (FirewindEnvironment.GetGame() == null)
                    throw new NullReferenceException();

                if (FirewindEnvironment.GetGame().GetRoomManager() == null)
                    throw new NullReferenceException();

                if (FirewindEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId) == null)
                    throw new NullReferenceException();

                return FirewindEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            }
        }

        internal string Description;
        internal Boolean Recommended;
        internal int CategoryId;
        internal PublicItemType itemType;
        internal string TagsToSearch;

        internal PublicItem(int mId, int mType, string mCaption, string mDescription, string mImage, PublicImageType mImageType, uint mRoomId, int mCategoryId, int mParentId, Boolean mRecommand, int mTypeOfData, string mTags)
        {
            BannerId = mId;
            Type = mType;
            Caption = mCaption;
            Description = mDescription;
            Image = mImage;
            ImageType = mImageType;
            RoomId = mRoomId;
            ParentId = mParentId;
            CategoryId = mCategoryId;
            Recommended = mRecommand;
            TagsToSearch = mTags;

            if (mTypeOfData == 1)
                itemType = PublicItemType.TAG;
            else if (mTypeOfData == 2)
                itemType = PublicItemType.FLAT;
            else if (mTypeOfData == 3)
                itemType = PublicItemType.PUBLIC_FLAT;
            else if (mTypeOfData == 4)
                itemType = PublicItemType.CATEGORY;
            else
                itemType = PublicItemType.NONE;
        }

        internal RoomData RoomInfo
        {
            get
            {
                return FirewindEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);
            }
        }

        internal void Serialize(ServerMessage Message)
        {
            try
            {
                Message.AppendInt32(Id);
                Message.AppendString(this.itemType != PublicItemType.PUBLIC_FLAT ? Caption : String.Empty);
                Message.AppendString(Description); // description
                Message.AppendInt32(Type); // image size
                Message.AppendString(this.itemType == PublicItemType.PUBLIC_FLAT ? Caption : String.Empty);
                Message.AppendString(Image);
                Message.AppendInt32(ParentId > 0 ? ParentId : 0);
                Message.AppendInt32(RoomInfo.UsersNow);
                Message.AppendInt32(itemType == PublicItemType.NONE ? 0 : (itemType == PublicItemType.TAG ? 1 : (itemType == PublicItemType.FLAT ? 2 : (itemType == PublicItemType.PUBLIC_FLAT ? 2 : (itemType == PublicItemType.CATEGORY ? 4 : 0)))));

                if (this.itemType == PublicItemType.TAG)
                {
                    Message.AppendString(TagsToSearch); return;
                }
                else if (this.itemType == PublicItemType.CATEGORY)
                {
                    Message.AppendBoolean(false); return;
                }
                else if (this.itemType == PublicItemType.FLAT)
                {
                    this.RoomInfo.Serialize(Message, false);
                }
                else if (this.itemType == PublicItemType.PUBLIC_FLAT)
                {
                    this.RoomInfo.Serialize(Message, false);
                }
                /*if (!Category)
                {
                    Message.AppendInt32(Id);

                    Message.AppendString((Type == 1) ? Caption : RoomData.Name);

                    Message.AppendString(RoomData.Description);
                    Message.AppendInt32(Type);
                    Message.AppendString(Caption);
                    Message.AppendString((ImageType == PublicImageType.EXTERNAL) ? Image : string.Empty);
                    Message.AppendInt32(ParentId);
                    Message.AppendInt32(RoomData.UsersNow);
                    Message.AppendInt32(3);
                    Message.AppendString((ImageType == PublicImageType.INTERNAL) ? Image : string.Empty);
                    Message.AppendUInt(1337);
                    Message.AppendBoolean(true);
                    Message.AppendString(RoomData.CCTs);
                    Message.AppendInt32(RoomData.UsersMax);
                    Message.AppendUInt(RoomId);
                }
                else if (Category)
                {
                    Message.AppendInt32(Id);
                    Message.AppendString(Caption);
                    Message.AppendString(string.Empty);
                    Message.AppendBoolean(true);
                    Message.AppendString(string.Empty);
                    Message.AppendString(Image);
                    Message.AppendBoolean(false);
                    Message.AppendBoolean(false);
                    Message.AppendInt32(4);
                    Message.AppendBoolean(false);
                }  */
            }
            catch (Exception e)
            {
                Logging.WriteLine("Exception on publicitems composing: " + e.ToString());
            }
        }
    }
}