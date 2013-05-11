using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FirewindEncryptor
{
    class Program
    {
        private static byte[] ENCRYPTION_KEY = {75, 22, 12, 44,  28, 54, 120, 53, 240, 209, 139, 63, 69, 211, 86, 23};
        static void Main(string[] args)
        {
            string s = Convert.ToBase64String(ENCRYPTION_KEY);
            if(args.Length != 1)
            {
                Console.WriteLine("WRONG ARGS LENGTH");
                Console.ReadKey(false);
                return;
            }

            byte[] assembly = File.ReadAllBytes(args[0]);
            AesCryptoServiceProvider p = new AesCryptoServiceProvider();
            p.Key = ENCRYPTION_KEY;
            p.IV = p.Key;

            File.WriteAllBytes("label", EncryptBytes(p, assembly));

            Console.WriteLine("WE'RE DONE HERE, FILE SAVED AS \"label\"");
        }
        private static byte[] EncryptBytes(SymmetricAlgorithm symAlg, byte[] inBlock)
        {
            ICryptoTransform xfrm = symAlg.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

            return outBlock;
        }

        private static byte[] DecryptBytes(SymmetricAlgorithm symAlg, byte[] inBytes)
        {
            ICryptoTransform xfrm = symAlg.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);

            return outBlock;
        }
    }
}
