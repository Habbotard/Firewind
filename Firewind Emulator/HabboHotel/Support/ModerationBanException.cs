using System;

namespace Firewind.HabboHotel.Support
{
    [Serializable()]
    public class ModerationBanException : Exception
    {
        internal ModerationBanException(string Reason) : base(Reason) { }
    }
}
