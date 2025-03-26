using System.Security.Cryptography;
using System.Text;

namespace MadreServer.Hasher
{
    public static class Hasher
    {
        public static string Sha256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string GetTimestampHash()
        {
            return Sha256Hash(DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
        }
    }
}
