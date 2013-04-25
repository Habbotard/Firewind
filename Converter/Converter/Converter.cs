using Converter.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Converter
{
    class Converter
    {
        public DatabaseConfig PhoenixDbConfig;
        public DatabaseConfig ButterflyDbConfig;

        public Converter(DatabaseConfig P, DatabaseConfig B)
        {
            this.PhoenixDbConfig = P;
            this.ButterflyDbConfig = B;
        }

        public void Start()
        {
            PhoenixDatabaseManager.Initialize();
            Log.Write("Connected to the Phoenix database.");

            ButterflyDatabaseManager.Initialize();
            Log.Write("Connected to the Butterfly database.");
            
            // Buttefly Interactions
            using (SqlDatabaseClient Bfly = ButterflyDatabaseManager.GetClient())
            {
                Database.Cleanup(Bfly);

                using (SqlDatabaseClient Phx = PhoenixDatabaseManager.GetClient())
                {
                    Database.DoRoomItems(Phx, Bfly);
                    Database.DoUserItems(Phx, Bfly);
                    Database.DoWallItems(Phx, Bfly);
                    Database.DoUserItems(Phx, Bfly);
                }
            }

            Log.Write("Done!");

            Loop();
        }

        public DatabaseConfig GetDatabaseConfig(string type)
        {
            if (type == "phx")
                return this.PhoenixDbConfig;
            else
                return this.ButterflyDbConfig;
        }

        public void Loop()
        {
            while (true)
            {
                Console.ReadKey();
            }
        }

    }
}
