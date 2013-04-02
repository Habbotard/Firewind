using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firewind.HabboHotel.Items;

namespace Firewind.HabboHotel.SoundMachine
{
    class SongInstance
    {
        private SongItem mDiskItem;
        private SongData mSongData;

        public SongItem DiskItem
        {
            get
            {
                return mDiskItem;
            }
        }

        public SongData SongData
        {
            get
            {
                return mSongData;
            }
        }

        public SongInstance(SongItem Item, SongData SongData)
        {
            mDiskItem = Item;
            mSongData = SongData;
        }
    }
}
