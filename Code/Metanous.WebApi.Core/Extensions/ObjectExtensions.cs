using System;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Metanous.WebApi.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string GenerateETag<T>(this T obj, JsonSerializerSettings settings = null)
        {
            var serializedObj = Serialize(obj, settings);
            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                return $"{BitConverter.ToString(cryptoServiceProvider.ComputeHash(serializedObj)).Replace("-", "")}";
            }
        }

        private static byte[] Serialize<T>(T obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
            }

            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            var json = JsonConvert.SerializeObject(obj, settings);
            var bytes = new byte[json.Length * sizeof(char)];
            Buffer.BlockCopy(json.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
