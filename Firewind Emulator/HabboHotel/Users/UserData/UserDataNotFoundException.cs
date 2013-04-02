using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firewind.HabboHotel.Users.UserDataManagement
{
    class UserDataNotFoundException : Exception
    {
        public UserDataNotFoundException(string reason)
            : base(reason)
        { }
    }
}
