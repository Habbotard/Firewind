using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database_Manager.Database.Database_Exceptions
{
    public class TransactionException : Exception
    {
        /// <summary>
        /// Creates a new transaction eception
        /// </summary>
        /// <param name="message">The message of the exception</param>
        public TransactionException(string message) : base(message) { }


    }
}
    
    
