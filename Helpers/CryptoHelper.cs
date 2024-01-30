using System;
using System.Security.Cryptography;
using System.Text;

namespace STTNote.Helpers
{
    public static class CryptoHelper
    {
        public static string? Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;
            using (var sha256 = new SHA256Managed())
            {
                var encoding = Encoding.UTF8.GetBytes(plainText);
                var hash = sha256.ComputeHash(encoding);
                var bitConvert = BitConverter.ToString(hash);
                return bitConvert.Replace("-", "");
            }
        }
    }
}