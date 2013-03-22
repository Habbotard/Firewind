using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FirewindLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Work in progress, going to be replaced when auth is done
            //AesCryptoServiceProvider p = new AesCryptoServiceProvider();
            //p.Key = DownloadKey("");
            //p.IV = DownloadIV("");
            //byte[] decryptedEmulator = Encryption.DecryptBytes(p, Resources.label);
            //Assembly emulator = Assembly.Load(decryptedEmulator);

            Assembly emulator = Assembly.Load(Resources.label);
            MethodInfo method = emulator.EntryPoint;
            if (method != null)
            {
                object o = emulator.CreateInstance(method.Name);
                try
                {
                    method.Invoke(o, null);
                }
                catch (TargetInvocationException e)
                {
                    // Emulator (crash?) errors is gonna appear here?
                    Console.WriteLine(e.ToString());
                    Console.ReadKey(true);
                }
            }
        }
        //private static byte[] DownloadKey(string serial)
        //{
        //    using (WebClient c = new WebClient())
        //    {
        //        byte[] key = c.DownloadData("http://aauth.getfirewind.com?i=0&k=" + serial);
        //    }
        //    return key;
        //}
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
