using Converter.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Application
    {
        public static Converter Converter;

        public static string Host;
        public static string Username;
        public static string Password;
        public static string FirewindDb;
        public static string PhoenixDb;

        static void Main(string[] args)
        {
            Log.Write("Please enter your MySQL database host");
            Host = Console.ReadLine();

            Log.Write("Please enter your MySQL username");
            Username = Console.ReadLine();

            Log.Write("Please enter your MySQL password");
            Password = Console.ReadLine();

            Log.Write("Please enter your Firewind Database name");
            FirewindDb = Console.ReadLine();

            Log.Write("Please enter your Phoenix Database name");
            PhoenixDb = Console.ReadLine();

            Log.Write("Please press enter to proceed.");
            Console.ReadLine();

            DatabaseConfig PhoenixDbConfig = new DatabaseConfig(Host, 3306, Username, Password, PhoenixDb, 1, 5, 10);
            DatabaseConfig ButterflyDbConfig = new DatabaseConfig(Host, 3306, Username, Password, FirewindDb, 1, 5, 10);

            Converter = new Converter(PhoenixDbConfig, ButterflyDbConfig);
            Converter.Start();
        }

        public static Converter GetConverter()
        {
            return Converter;
        }
    }
}
