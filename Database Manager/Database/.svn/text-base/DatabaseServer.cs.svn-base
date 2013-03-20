using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database_Manager.Database.Database_Exceptions;

namespace Database_Manager.Managers.Database
{
    public class DatabaseServer
    {
        #region Fields

        /// <summary>
        /// The network host of the database server, eg 'localhost' or '127.0.0.1'.
        /// </summary>
        private readonly string host;

        /// <summary>
        /// The network port of the database server as an unsigned 32 bit integer.
        /// </summary>
        private readonly uint port;

        /// <summary>
        /// The username to use when connecting to the database.
        /// </summary>
        private readonly string user;

        /// <summary>
        /// The password to use in combination with the username when connecting to the database.
        /// </summary>
        private readonly string password;

        /// <summary>
        /// The name of the database;
        /// </summary>
        private readonly string databaseName;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a DatabaseServer object with given details.
        /// </summary>
        /// <param name="sHost">The network host of the database server, eg 'localhost' or '127.0.0.1'.</param>
        /// <param name="Port">The network port of the database server as integer.</param>
        /// <param name="sUser">The username to use when connecting to the database.</param>
        /// <param name="sPassword">The password to use in combination with the username when connecting to the database.</param>
        public DatabaseServer(string host, uint port, string username, string password, string databaseName)
        {
            if (host == null || host.Length == 0)
                throw new DatabaseException("No host was given");
            if (username == null || username.Length == 0)
                throw new DatabaseException("No username was given");
            if(databaseName == null || databaseName.Length == 0)
                throw new DatabaseException("No database name was given");

            this.host = host;
            this.port = port;

            this.databaseName = databaseName;
            this.user = username;
            this.password = (password != null) ? password : "";
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates this class details
        /// </summary>
        /// <returns>a string contianing the username and host </returns>
        /// <example>foo@127.0.0.1</example>
        public override string ToString()
        {
            return user + "@" + host;
        }

        /// <summary>
        /// Returns the port of the database server
        /// </summary>
        /// <returns>The port of the database</returns>
        public uint getPort()
        {
            return port;
        }

        /// <summary>
        /// Returns the username of the database server
        /// </summary>
        /// <returns>The username of the database</returns>
        public string getUsername()
        {
            return user;
        }

        /// <summary>
        /// Returns the host of the database server
        /// </summary>
        /// <returns>The host of the database</returns>
        public string getHost()
        {
            return host;
        }

        /// <summary>
        /// Returns the password of the database server
        /// </summary>
        /// <returns>The password of the database</returns>
        public string getPassword()
        {
            return password;
        }

        public string getDatabaseName()
        {
            return this.databaseName;
        }
        #endregion
    }
}
