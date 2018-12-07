using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LibRes.App.Utils
{
    public static class EncryptionUtil
    {
        /// <summary>
        ///     Performs AES based encryption on a data string and returns the hash.
        /// </summary>
        /// <returns>Hash based of the encrypted string.</returns>
        /// <param name="plainString">String to be encrypted.</param>
        public static string Encrypt(string plainString)
        {
            const string encryptionKey = "MAKV2SPBNI99212";
            var clearBytes = Encoding.Unicode.GetBytes(plainString);
            string encryptedString;
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                Debug.Assert(encryptor != null, nameof(encryptor) + " != null");
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    encryptedString = Convert.ToBase64String(ms.ToArray());
                }
            }

            return encryptedString;
        }

        /// <summary>
        ///     Decrypts AES encrypted string and returns the plain value.
        /// </summary>
        /// <returns>Plain text value.</returns>
        /// <param name="cipherText">The encrypted string to be decrypted.</param>
        public static string Decrypt(string cipherText)
        {
            const string encryptionKey = "MAKV2SPBNI99212";
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                Debug.Assert(encryptor != null, nameof(encryptor) + " != null");
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }
    }
}