using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FirewindLauncher
{
    class Program
    {
        public const int VERSION = 4879;
        static void Main(string[] args)
        {
            if (!CheckHost())
                return;
            //XmlDictionaryReader json = JsonReaderWriterFactory.CreateJsonReader(DownloadInfo(ReadKeyFromConfig()), new XmlDictionaryReaderQuotas());
            // Work in progress, going to be replaced when auth is done
            AesCryptoServiceProvider p = new AesCryptoServiceProvider();
            //XElement root = XElement.Load(json);
            //byte[] key = Convert.FromBase64String((string)root.NextNode.Document);
            string[] info = DownloadInfo(ReadKeyFromConfig()); // 0=key,1=user limit,2=license holder name,3=latestver

            if (info.Length == 4 && int.Parse(info[3]) > VERSION) // New version available!
            {
                Console.WriteLine("There is a new version available.");
                Console.WriteLine("Go to the download section at getfirewind.com to get it.");
                Console.ReadKey(true);
                return;
            }
            string encodedKey = DownloadInfo(ReadKeyFromConfig())[0];
            byte[] key;

            try
            {
                key = Convert.FromBase64String(encodedKey);
            }
            catch
            {
                Console.WriteLine("Wrong authentication key or key not found!");
                Console.ReadKey(true);
                return;
            }

            if (key == null)
            {
                Console.WriteLine("Wrong authentication key!");
                Console.ReadKey(true);
                return;
            }

            p.Key = key;
            p.IV = p.Key;

            if (!CheckHost())
                return;

            byte[] decryptedEmulator = Encryption.DecryptBytes(p, Resources.label);
            Assembly emulator;

            try
            {
                emulator = Assembly.Load(decryptedEmulator);
            }
            catch 
            {
                Console.WriteLine("Wrong authentication key!");
                Console.ReadKey(true);
                return;
            }

           // Assembly emulator = Assembly.Load(Resources.label);
            MethodInfo method = emulator.EntryPoint;
            if (method != null)
            {
                object o = emulator.CreateInstance(method.Name);
                try
                {
                    method.Invoke(o, new object[] { new string[] { ReadKeyFromConfig(), DownloadInfo(ReadKeyFromConfig())[1] == "null" ? "0" : DownloadInfo(ReadKeyFromConfig())[1], DownloadInfo(ReadKeyFromConfig())[2] }});
                }
                catch (TargetInvocationException e)
                {
                    // Emulator (crash?) errors is gonna appear here?
                    Console.WriteLine(e.ToString());
                    Console.ReadKey(true);
                }
            }
        }
        private static bool CheckHost()
        {
            string ip = Dns.GetHostAddresses("getfirewind.com")[0].ToString();

            return true;
        }
        private static string[] DownloadInfo(string serial)
        {
            WebRequest req = WebRequest.Create("http://getfirewind.com/auth");

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            byte[] bytes = Encoding.UTF8.GetBytes(string.Format("key={0}", serial));
            req.ContentLength = bytes.Length;

            Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();

            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null)
                return null;

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string result = sr.ReadToEnd().Trim();
            if (result == "not_authed" || result == "expired")
                return new string[1];

            return result.Split((char)0);
        }
        private static string ReadKeyFromConfig()
        {
            if(!File.Exists("Settings\\Configuration.ini"))
            {
                Console.WriteLine("Could not find configuration.ini!");
                //Console.ReadKey(true);
                return "";
            }

            string[] lines = File.ReadAllLines("Settings\\Configuration.ini");

            foreach (string line in lines)
            {
                if (line.StartsWith("auth.key"))
                    return line.Split('=')[1];
            }
            return "";
        }
        //private static byte[] DownloadIV(string serial)
        //{
        //    using (WebClient c = new WebClient())
        //    {
        //        byte[] IV = c.DownloadData("http://aauth.getfirewind.com?i=1&k=" + serial);
        //    }
        //    return IV;
        //}
    }
}
