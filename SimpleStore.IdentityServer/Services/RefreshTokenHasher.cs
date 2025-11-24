using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace SimpleStore.IdentityServer.Services
{
    public class RefreshTokenHasher
    {
        public static byte[] GenerateSalt(int length = 16)
        {
            var salt = new byte[length];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public static byte[] HashToken(string token, byte[] salt, int iterations = 100000)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            return new Rfc2898DeriveBytes(tokenBytes, salt, iterations, HashAlgorithmName.SHA256)
                .GetBytes(32); // 32 bytes output hash
        }

        public static bool VerifyHashToken(string token, string salt, string hashedToken)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var hashToVerify = HashToken(token, saltBytes);

            var hashedTokenBytes = Convert.FromBase64String(hashedToken);
            return hashToVerify.SequenceEqual(hashedTokenBytes);
        }
    }
}
