using System.Security.Cryptography;
using System.Text;

namespace MadreServer.Hasher
{
    public static class Hasher
    {
        public static string SHA256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string GetTimestampHash()
        {
            return SHA256(DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
        }
    }
}
