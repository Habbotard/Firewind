using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter.Storage
{
    class DatabaseConfig
    {
        public string dbHost
        {
            get;
            set;
        }

        public int dbPort
        {
            get;
            set;
        }

        public string dbUsername
        {
            get;
            set;
        }

        public string dbPassword
        {
            get;
            set;
        }

        public string dbDatabase
        {
            get;
            set;
        }

        public int dbMinPool
        {
            get;
            set;
        }

        public int dbMaxPool
        {
            get;
            set;
        }

        public int dbPoolLife
        {
            get;
            set;
        }

        public DatabaseConfig(string dbHost, int dbPort, string dbUsername, string dbPassword, string dbDatabase, int dbMinPool, int dbMaxPool, int dbPoolLife)
        {
            this.dbHost = dbHost;
            this.dbPort = dbPort;
            this.dbUsername = dbUsername;
            this.dbPassword = dbPassword;
            this.dbDatabase = dbDatabase;
            this.dbMinPool = dbMinPool;
            this.dbMaxPool = dbMaxPool;
            this.dbPoolLife = dbPoolLife;
        }
    }
}
