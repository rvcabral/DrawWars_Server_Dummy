using DrawWars.Api.Models.Headers;
using DrawWars.Api.Models.Settings;
using DrawWars.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DrawWars.Api.Utils
{
    public static class CryptoUtils
    {
        private static byte[] Key => Convert.FromBase64String(Settings.AesKey);

        internal static CryptoSettings Settings { get; set; }
        
        public static string HashPassword(string password, byte[] salt = null)
        {
            var passBytes = Encoding.UTF8.GetBytes(password);
            salt = salt ?? GenerateSalt();

            var saltedPass = passBytes.Concat(salt).ToArray();

            byte[] hash;
            using (var sha256Hash = SHA256.Create())
                hash = sha256Hash.ComputeHash(saltedPass);

            var saltedHash = hash.Concat(salt).ToArray();

            return Convert.ToBase64String(saltedHash);
        }

        public static bool ComparePassword(string password, string hash)
        {
            var salt = RetrieveSalt(hash);

            var newHash = HashPassword(password, salt);

            return hash == newHash;
        }

        public static string CypherHeader(DrawWarsUser user)
        {
            var result = new SessionHeader();

            byte[] idCypher;

            using (var aes = Aes.Create())
            {
                result.IV = aes.IV;

                var encryptor = aes.CreateEncryptor(Key, result.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(user.Id.ToString());
                    }

                    idCypher = ms.ToArray();
                }
            }

            result.UserId = Convert.ToBase64String(idCypher);

            var jsonHeader = JsonConvert.SerializeObject(result);
            var headerValue = Encoding.UTF8.GetBytes(jsonHeader);

            return Convert.ToBase64String(headerValue);
        }

        public static int GetUserIdFromHeader(string header)
        {
            var headerValue = Convert.FromBase64String(header);
            var jsonHeader = Encoding.UTF8.GetString(headerValue);
            var sessionHeader = JsonConvert.DeserializeObject<SessionHeader>(jsonHeader);

            var idCypher = Convert.FromBase64String(sessionHeader.UserId);

            using (var aes = Aes.Create())
            {
                aes.Key = Key;

                var encryptor = aes.CreateDecryptor(aes.Key, sessionHeader.IV);
                using (var ms = new MemoryStream(idCypher))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Read))
                using (var sw = new StreamReader(cs))
                {
                    return int.Parse(sw.ReadToEnd());
                }
            }
        }

        #region Private Utils

        private static byte[] RetrieveSalt(string hash)
        {
            var saltedHash = Convert.FromBase64String(hash);

            return saltedHash.Skip(256 / 8).ToArray();
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[32];

            using (var random = new RNGCryptoServiceProvider())
                random.GetNonZeroBytes(salt);

            return salt;
        }

        #endregion
    }
}
