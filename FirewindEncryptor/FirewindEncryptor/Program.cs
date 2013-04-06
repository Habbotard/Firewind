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
        private static byte[] ENCRYPTION_KEY = {161, 221, 123, 139,  28, 54, 120, 60, 240, 209, 139, 68, 73, 120, 222, 43};
        static void Main(string[] args)
        {
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
