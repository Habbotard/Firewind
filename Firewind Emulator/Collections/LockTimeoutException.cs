using System;

namespace Firewind.Collections
{
    [Serializable]
    class LockTimeoutException : Exception
    {
        public LockTimeoutException(string exception)
            : base(exception)
        {
            //Mini weini
        }
    }
}
