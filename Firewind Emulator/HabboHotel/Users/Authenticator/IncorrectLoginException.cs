using System;

namespace Firewind.HabboHotel.Users.Authenticator
{
    [Serializable()]
    public class IncorrectLoginException : Exception
    {
        internal IncorrectLoginException(string Reason) : base(Reason) { }
    }
}
