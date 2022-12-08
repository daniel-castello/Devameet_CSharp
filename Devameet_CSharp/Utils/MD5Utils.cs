using System.Security.Cryptography;
using System.Text;

namespace Devameet_CSharp.Utils
{
    public class MD5Utils
    {
        public static string GenerateHashMD5(string text)
        {
            MD5 md5hash = MD5.Create();
            var bytes = md5hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append(bytes[i]);
            }

            return stringBuilder.ToString();
        }
    }
}
