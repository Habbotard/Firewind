using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database_Manager.Database.Database_Exceptions
{
    public class DatabaseException : Exception
    {
        /// <summary>
        /// Creates a new DatabaseException
        /// </summary>
        /// <param name="message">The message which was generated</param>
        public DatabaseException(string message) : base(message) { }
    }

}
