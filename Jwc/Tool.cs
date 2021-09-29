using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jwc
{
    internal class Tool
    {
        internal static string GenerateFakeFinger()
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var finger = new string(Enumerable.Repeat(chars, 32).Select(s => s[random.Next(chars.Length)]).ToArray());
            return finger;
        }

        internal static string Md5(string oldstring)
        {
            byte[] byteOld = Encoding.UTF8.GetBytes(oldstring);
            byte[] byteNew = MD5.Create().ComputeHash(byteOld);
            StringBuilder sb = new();
            foreach (var b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        internal static string Sha1(string oldstring)
        {
            byte[] byteOld = Encoding.UTF8.GetBytes(oldstring);
            byte[] byteNew = SHA1.Create().ComputeHash(byteOld);
            StringBuilder sb = new();
            foreach (var b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
