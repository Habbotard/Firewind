using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database_Manager.Database.Database_Exceptions
{
    
    public class QueryException : Exception
    {
        private string query;
        /// <summary>
        /// Constructs a new Query exception
        /// </summary>
        /// <param name="message">The message that was given</param>
        /// <param name="inner">Inner exception</param>
        /// <param name="query">The query it was </param>
        public QueryException(string message, string query) : base(message) 
        {
            this.query = query;
        }

        /// <summary>
        /// Returns the malfunctioning query
        /// </summary>
        /// <returns>The query which caused an error</returns>
        public string getQuery()
        {
            return query;
        }
    }
    
}
