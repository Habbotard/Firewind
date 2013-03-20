using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketTest
{
    class BufferUnderflowException : Exception
    {
        private string p;

        public BufferUnderflowException(string p) : base(p)
        {
        }
    }
}
