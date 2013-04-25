using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Converter.Storage
{
    public static class ButterflyDatabaseManager
    {
        private static Dictionary<int, SqlDatabaseClient> mClients;
        private static int mStarvationCounter;
        private static int mMinPoolSize;
        private static int mMaxPoolSize;
        private static int mPoolLifetime;
        private static int mClientIdGenerator;
        private static object mSyncRoot;
        private static ManualResetEvent mPoolWait;
        private static Timer mMonitorThread;

        public static int ClientCount
        {
            get
            {
                return mClients.Count;
            }
        }

        public static void Initialize()
        {
            mClients = new Dictionary<int, SqlDatabaseClient>();
            mMinPoolSize = Application.GetConverter().GetDatabaseConfig("bfly").dbMinPool; //(int)ConfigManager.GetValue("mysql.pool.min")
            mMaxPoolSize = Application.GetConverter().GetDatabaseConfig("bfly").dbMaxPool; //(int)ConfigManager.GetValue("mysql.pool.max")
            mPoolLifetime = Application.GetConverter().GetDatabaseConfig("bfly").dbPoolLife; //(int)ConfigManager.GetValue("mysql.pool.lifetime")
            mSyncRoot = new object();
            mPoolWait = new ManualResetEvent(true);

            mMonitorThread = new Timer(new TimerCallback(ProcessMonitorThread), null, mPoolLifetime / 2, mPoolLifetime / 2);

            if (mMinPoolSize < 0)
            {
                throw new ArgumentException("(Sql) Invalid database pool size configured (less than zero).");
            }

            SetClientAmount(mMinPoolSize, "server init");
        }
        
        public static void Uninitialize()
        {
            int Attempts = 0;

            while (mClients.Count > 0)
            {
                lock (mSyncRoot)
                {
                    List<int> Removable = new List<int>();

                    foreach (SqlDatabaseClient Client in mClients.Values)
                    {
                        if (!Client.Available && Attempts <= 15)
                        {
                            continue;
                        }

                        Removable.Add(Client.Id);
                    }

                    foreach (int RemoveId in Removable)
                    {
                        mClients[RemoveId].Close();
                        mClients.Remove(RemoveId);
                    }
                }

                if (mClients.Count > 0)
                {
                    //Logging.Debug("(Sql) Waiting for all database clients to release (" + ++Attempts + ")...");
                    Thread.Sleep(100);
                }
            }
        }

        public static void ProcessMonitorThread(object state)
        {
            if (ClientCount > mMinPoolSize)
            {
                lock (mSyncRoot)
                {
                    List<int> ToDisconnect = new List<int>();

                    foreach (SqlDatabaseClient Client in mClients.Values)
                    {
                        if (Client.Available && Client.TimeInactive >= mPoolLifetime)
                        {
                            ToDisconnect.Add(Client.Id);
                        }
                    }

                    foreach (int DisconnectId in ToDisconnect)
                    {
                        mClients[DisconnectId].Close();
                        mClients.Remove(DisconnectId);
                    }

                    if (ToDisconnect.Count > 0)
                    {
                        //Logging.Debug("(Sql) Disconnected " + ToDisconnect.Count + " inactive client(s).");
                    }
                }
            }
        }

        public static void SetClientAmount(int ClientAmount, string LogReason = "Unknown")
        {
            int Diff;

            lock (mSyncRoot)
            {
                Diff = ClientAmount - ClientCount;

                if (Diff > 0)
                {
                    for (int i = 0; i < Diff; i++)
                    {
                        int NewId = GenerateClientId();
                        mClients.Add(NewId, CreateClient(NewId));
                    }
                }
                else
                {
                    int ToDestroy = -Diff;
                    int Destroyed = 0;

                    foreach (SqlDatabaseClient Client in mClients.Values)
                    {
                        if (!Client.Available)
                        {
                            continue;
                        }

                        if (Destroyed >= ToDestroy || ClientCount <= mMinPoolSize)
                        {
                            break;
                        }

                        Client.Close();
                        mClients.Remove(Client.Id);
                        Destroyed++;
                    }
                }
            }

            //Logging.Debug("(Sql) Client availability: " + ClientAmount + "; modifier: " + Diff + "; reason: "
                //+ LogReason + ".");
        }

        public static SqlDatabaseClient GetClient()
        {
            lock (mSyncRoot)
            {
                foreach (SqlDatabaseClient Client in mClients.Values)
                {
                    if (!Client.Available)
                    {
                        continue;
                    }

                    //Logging.Debug("(Sql) Assigned client " + Client.Id + ".");
                    Client.Available = false;
                    return Client;
                }

                if (mMaxPoolSize <= 0 || ClientCount < mMaxPoolSize) // Max pool size ignored if set to 0 or lower
                {
                    SetClientAmount(ClientCount + 1, "out of assignable clients in GetClient()");
                    return GetClient();
                }

                mStarvationCounter++;

                //Logging.Debug("(Sql) Client starvation; out of assignable clients/maximum pool size reached. Consider increasing the MySQL max pool size. Starvation count is " + mStarvationCounter + ".");

                // Wait until an available client returns
                Monitor.Wait(mSyncRoot);
                return GetClient();
            }
        }

        public static void PokeAllAwaiting()
        {
            lock (mSyncRoot)
            {
                Monitor.PulseAll(mSyncRoot);
            }
        }

        private static int GenerateClientId()
        {
            lock (mSyncRoot)
            {
                return mClientIdGenerator++;
            }
        }

        private static SqlDatabaseClient CreateClient(int Id)
        {
            MySqlConnection Connection = new MySqlConnection(GenerateConnectionString());
                Connection.Open();

            return new SqlDatabaseClient(Id, Connection);
        }

        public static string GenerateConnectionString()
        {
            MySqlConnectionStringBuilder ConnectionStringBuilder = new MySqlConnectionStringBuilder();
            ConnectionStringBuilder.Server = Application.GetConverter().GetDatabaseConfig("bfly").dbHost;
            ConnectionStringBuilder.Port = (uint)Application.GetConverter().GetDatabaseConfig("bfly").dbPort;
            ConnectionStringBuilder.UserID = Application.GetConverter().GetDatabaseConfig("bfly").dbUsername;
            ConnectionStringBuilder.Password = Application.GetConverter().GetDatabaseConfig("bfly").dbPassword;
            ConnectionStringBuilder.Database = Application.GetConverter().GetDatabaseConfig("bfly").dbDatabase;
            ConnectionStringBuilder.MinimumPoolSize = (uint)Application.GetConverter().GetDatabaseConfig("bfly").dbMinPool;
            ConnectionStringBuilder.MaximumPoolSize = (uint)Application.GetConverter().GetDatabaseConfig("bfly").dbMaxPool;
            return ConnectionStringBuilder.ToString();
        }
    }
}