using System;

namespace Firewind.Storage
{
    [Serializable()]
    public class DatabaseExceptionOld : Exception 
    {
        internal DatabaseExceptionOld(string sMessage) : base(sMessage) { }
    }
}
