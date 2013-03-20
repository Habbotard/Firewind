using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using Database_Manager.Managers.Database;
using Database_Manager.Database.Session_Details.Interfaces;
using Database_Manager.Database.Database_Exceptions;
using Helpers;
using System.Collections;
using System.Threading;
using System.Text;
namespace Database_Manager.Database
{
    public class DatabaseManager
    {
        /// <summary>
        /// The server details of the database
        /// </summary>
        private DatabaseServer server;

        /// <summary>
        /// Indication of how many connections the server may have with the mysql host
        /// </summary>
        private uint maxPoolSize;

        /// <summary>
        /// Indication of the startamount of clients which will be connected to the database server at initialization
        /// </summary>
        private int beginClientAmount;

        /// <summary>
        /// The connection string which is used to connect to the database server
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Indicates if this item is connected
        /// </summary>
        private bool isConnected = false;

        /// <summary>
        /// dbClients currently unused
        /// </summary>
        private Queue dbClientCollection;
        /// <summary>
        /// Contains the stack of "broken/expired" conenctions
        /// </summary>
        private Queue refreshStack;

        private int totalCurrentConnections = 0;

        private Thread checkThread;

        /// <summary>
        /// Creates a new database manager with a max-poolsize and begin client amount
        /// </summary>
        /// <param name="maxPoolSize">An unsigned int with the indication of the max amount of connections to the database server</param>
        /// <param name="clientAmount">An unsigned int with the indication of the clients which will be connected for the server</param>
        public DatabaseManager(uint maxPoolSize, int clientAmount)
        {
            Out.writeNotification("Setting up Database manager with a maximum of [" + maxPoolSize + "] and start client amount of [" + clientAmount + "]");
            if (maxPoolSize < clientAmount)
                throw new Database_Exceptions.DatabaseException("The poolsize can not be larger than the client amount!");
            this.beginClientAmount = clientAmount;
            this.maxPoolSize = maxPoolSize;
        }

        /// <summary>
        /// Sets the server detials
        /// </summary>
        /// <param name="host">The network host of the database server, eg 'localhost' or '127.0.0.1'.</param>
        /// <param name="port">The network port of the database server as integer.</param>
        /// <param name="username">The username to use when connecting to the database.</param>
        /// <param name="password">The password to use in combination with the username when connecting to the database.</param>
        /// <returns>A boolean indicating if the data was filled in propperly</returns>
        public bool setServerDetails(string host, uint port, string username, string password, string databaseName)
        {
            try
            {
                server = new DatabaseServer(host, port, username, password, databaseName);
                Out.writeLine("Database connector succesfully set the connection details", Out.logFlags.ImportantLogLevel);
                return true;
            }
            catch (DatabaseException ex)
            {
                this.isConnected = false;
                Out.writeError("Database connector fialed to set the connection details: " + ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Initializes the connections
        /// </summary>
        /// <returns>boolean indicating if the initialization process was succesfull</returns>
        public void init()
        {
            if (isConnected)
                return;
            try
            {
                Out.writeLine("Creating new database connections.. stand by..", Out.logFlags.ImportantLogLevel);
                MySqlConnectionStringBuilder mysqlSb = new MySqlConnectionStringBuilder();

                createNewConnectionString();

                this.dbClientCollection = new Queue();
                this.refreshStack = new Queue();
                DatabaseClient dbClient;
                if (this.beginClientAmount != 0)
                {
                    for (int i = 0; i < this.beginClientAmount; i++)
                    {
                        Out.writeLine("Opening database connection [" + (i).ToString() + "] out of [" + this.beginClientAmount + "]", Out.logFlags.lowLogLevel);
                        addConnection();
                        
                    }
                }
                else
                {
                    dbClient = new DatabaseClient(this, -1);
                    dbClient.connect();
                    dbClient.disconnect();
                }
                //Out.writePlain(connectionString, Out.logFlags.lowLogLevel);

            }
            catch (MySqlException ex)
            {
                isConnected = false;
                throw new Exception("Could not connect the clients to the database: " + ex.Message);
            }

            isConnected = true;
            checkThread = new Thread(this.healthChecker);
            this.checkThread.Start();
            Out.writeLine("Created new connections: [" + beginClientAmount + "]", Out.logFlags.ImportantLogLevel);
            

        }

        /// <summary>
        /// Gets the connections string
        /// </summary>
        /// <returns>The connection string used to connect to a database server</returns>
        internal string getConnectionString()
        {
            return connectionString;
        }

        /// <summary>
        /// Returns a prepared database connection
        /// </summary>
        /// <param name="autoCommit"></param>
        /// <returns></returns>
        private DatabaseClient getClient(bool autoCommit)
        {
            try
            {
                lock (this.dbClientCollection.SyncRoot)
                {
                    DateTime now = DateTime.Now;
                    DatabaseClient toReturn;
                    while (dbClientCollection.Count > 0)
                    {
                        toReturn = dbClientCollection.Dequeue() as DatabaseClient;
                        if (toReturn.getConnectionState() == ConnectionState.Open && (now - toReturn.getLastAction()).TotalHours < 3)
                        {
                            toReturn.prepare(autoCommit);
                            return toReturn;
                        }
                        else
                        {
                            lock (this.refreshStack.SyncRoot)//database client is borked
                            {
                                this.refreshStack.Enqueue(toReturn);
                            }
                        }
                    }

                    if (this.totalCurrentConnections < maxPoolSize)
                    {
                        Out.writeSpecialLine("Out of clients", Out.logFlags.lowLogLevel, ConsoleColor.Green, ConsoleColor.Yellow, "DatabaseManager.getClient >> ");
                        addConnection();
                        return getClient(autoCommit);
                    }
                    else
                    {
                        this.maxPoolSize = (uint)(((double)maxPoolSize) * 1.5);
                        Out.writeSpecialLine("Out of clients, POOL SIZE TOO SMALL!! INCREASE IT TO [" + this.maxPoolSize + "] for propper operation..", Out.logFlags.ImportantLogLevel, ConsoleColor.Green, ConsoleColor.Yellow, "DatabaseManager.getClient >> ");
                        createNewConnectionString();
                        return getClient(autoCommit);
                    }
                }
            }
            catch (Exception ex)
            {
                Out.writeLine("problem? " + ex.Message, Out.logFlags.ImportantLogLevel);
                return null;
            }

        }

        private void addConnection()
        {
            DatabaseClient dbClient = new DatabaseClient(this,totalCurrentConnections);
            totalCurrentConnections++;
            try
            {
                dbClient.connect();
            }
            catch (Exception ex)
            {
                Out.writeLine("Error while connecting to database: " + ex.Message, Out.logFlags.ImportantLogLevel);
            }
            this.dbClientCollection.Enqueue(dbClient);
            Out.writeLine("Adding new client to the database manager", Out.logFlags.lowLogLevel);
        }

        private void setConnectionString(MySqlConnectionStringBuilder connectionString)
        {
            this.connectionString = connectionString.ToString();
            Out.writeLine(this.connectionString, Out.logFlags.lowLogLevel);
        }

        private void createNewConnectionString()
        {
            MySqlConnectionStringBuilder mysqlSb = new MySqlConnectionStringBuilder();

            // Server
            mysqlSb.Server = server.getHost();
            mysqlSb.Port = server.getPort();
            mysqlSb.UserID = server.getUsername();
            mysqlSb.Password = server.getPassword();
            // Database
            mysqlSb.Database = server.getDatabaseName();
            mysqlSb.MinimumPoolSize = (this.maxPoolSize / 2);
            mysqlSb.MaximumPoolSize = this.maxPoolSize;
            mysqlSb.AllowZeroDateTime = true;
            mysqlSb.ConvertZeroDateTime = true;


            this.setConnectionString(mysqlSb);

        }

        /// <summary>
        /// Gets a query reactor with a boolean indicating its a transaction or not
        /// </summary>
        /// <param name="autoCommit">Indicates if this is going to be a transaction or not</param>
        /// <returns>An interface with the transaction/normal query reactor</returns>
        public IQueryAdapter getQueryreactor(bool transaction)
        {
            return getClient(transaction).getQueryReactor();
        }
        /// <summary>
        /// Gets a query reactor with a boolean indicating its a transaction or not
        /// </summary>
        /// <param name="autoCommit">Indicates if this is going to be a transaction or not</param>
        /// <returns>An interface with the transaction/normal query reactor</returns>
        public IQueryAdapter getQueryreactor()
        {
            return getQueryreactor(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void destroy()
        {
            lock (this.dbClientCollection.SyncRoot)
            {
                this.isConnected = false;
                if (dbClientCollection != null)
                {
                    foreach (DatabaseClient client in this.dbClientCollection)
                    {
                        if (!client.isAvailable())
                        {
                            client.Dispose();
                        }
                        client.disconnect();
                    }
                    dbClientCollection.Clear();
                }

            }
        }

        /// <summary>
        /// Boolean indicating if this item is connected to the database
        /// </summary>
        /// <returns></returns>
        public bool isConnectedToDatabase()
        {
            return isConnected;
        }

        private void healthChecker()
        {
            while (isConnected)
            {
                if (this.refreshStack.Count > 0)
                {
                    lock (this.refreshStack.SyncRoot)
                    {
                        if (this.refreshStack.Count > 0)
                        {
                            DatabaseClient toReCheck;
                            List<DatabaseClient> failedClients = new List<DatabaseClient>(this.refreshStack.Count);
                            while (this.refreshStack.Count > 0)
                            {
                                toReCheck = this.refreshStack.Dequeue() as DatabaseClient;
                                toReCheck.disconnect();
                                toReCheck.Dispose();
                                toReCheck = new DatabaseClient(this, toReCheck.getID());
                                try
                                {
                                    toReCheck.connect();

                                    reportDone(toReCheck);
                                }
                                catch (Exception ex)
                                {
                                    failedClients.Add(toReCheck);
                                    Out.writeSeriousError("Exception while reconencting client [" + toReCheck.getID() + "] Message was -> " + ex.Message);
                                }
                            }
                            foreach (DatabaseClient client in failedClients)
                            {
                                this.refreshStack.Enqueue(client);
                            }
                        }
                    }
                }
                Thread.Sleep(100);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getOpenConnectionCount()
        {
            return this.totalCurrentConnections;
        }

        internal void reportDone(DatabaseClient databaseClient)
        {
            lock (this.dbClientCollection.SyncRoot)
            {
                this.dbClientCollection.Enqueue(databaseClient);
            }
        }

        public void dumpStatus(ref StringBuilder builder)
        {
            //if(toReturn.getConnectionState() == ConnectionState.Open && (now - toReturn.getLastAction()).TotalHours < 3)

            Console.WriteLine("=== DATABASE CONNECTOR STATUS ===");
            builder.AppendLine("=== DATABASE CONNECTOR STATUS ===");
            Console.WriteLine("Obtaining lock on dbClientConnection.SyncRoot");
            builder.AppendLine("Obtaining lock on dbClientConnection.SyncRoot");

            int readyConnection = 0;

            lock (dbClientCollection.SyncRoot)
            {
                Console.WriteLine("Lock obtained on dbClientConnection.SyncRoot");
                builder.AppendLine("Lock obtained on dbClientConnection.SyncRoot");
                DateTime now = DateTime.Now;
                foreach (DatabaseClient dbClient in dbClientCollection)
                {
                    bool wouldReturn = false;

                    double idleHours = (now - dbClient.getLastAction()).TotalHours;
                    if (dbClient.getConnectionState() == ConnectionState.Open && idleHours < 3)
                        wouldReturn = true;

                    Console.WriteLine("Would return: " + wouldReturn.ToString());
                    builder.AppendLine("Would return: " + wouldReturn.ToString());

                    Console.WriteLine("Connection state: " + dbClient.getConnectionState().ToString());
                    builder.AppendLine("Connection state: " + dbClient.getConnectionState().ToString());

                    Console.WriteLine("Idle time: " + wouldReturn);
                    builder.AppendLine("Idle time: " + wouldReturn);
                    readyConnection++;
                }
            }

            Console.WriteLine("Ready connections: " + readyConnection);
            builder.AppendLine("Ready connections: " + readyConnection);

            Console.WriteLine("Obtaining lock on refreshStack.SyncRoot");
            builder.AppendLine("Obtaining lock on refreshStack.SyncRoot");

            int count = 0;

            lock (refreshStack.SyncRoot)
            {
                Console.WriteLine("Obtained lock on refreshStack.SyncRoot");
                builder.AppendLine("Obtained lock on refreshStack.SyncRoot");

                DateTime now = DateTime.Now;
                foreach (DatabaseClient dbClient in refreshStack)
                {
                    bool wouldReturn = false;

                    double idleHours = (now - dbClient.getLastAction()).TotalHours;
                    if (dbClient.getConnectionState() == ConnectionState.Open && idleHours < 3)
                        wouldReturn = true;

                    Console.WriteLine("Would return: " + wouldReturn.ToString());
                    builder.AppendLine("Would return: " + wouldReturn.ToString());

                    Console.WriteLine("Connection state: " + dbClient.getConnectionState().ToString());
                    builder.AppendLine("Connection state: " + dbClient.getConnectionState().ToString());

                    Console.WriteLine("Idle time: " + wouldReturn);
                    builder.AppendLine("Idle time: " + wouldReturn);
                    count++;
                }
            }
            Console.WriteLine("Waiting refresh count: " + count);
            builder.AppendLine("Waiting refresh count: " + count);

            Console.WriteLine("");
            builder.AppendLine("");
        }
    }
}
