﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firewind.Core
{
    enum ExceptionType
    {
        UserException,
        SQLException,
        FatalException, //Normally caused by Dario
        ThreadedException,
        StandardException,
        DDOSException
    }
}
