using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EAGSS
{
    internal class AESEncryptionAlgorithm
    {
        private const string Key = @"67nTzL5X!!18M8wc";

        private static readonly byte[] Iv = {
                                                0x98, 0xCF, 0xE4, 0x81, 0x44, 0xA3, 0x7D, 0x8B,
                                                0xDA, 0xE2, 0x8C, 0x78, 0x0B, 0x45, 0x27, 0x73
                                            };

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="inputByteArray">input byte array</param>
        /// <returns>Encryptd byte array</returns>
        public static byte[] AESEncrypt(byte[] inputByteArray)
        {
            SymmetricAlgorithm aes = Rijndael.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Iv;
            byte[] cipherBytes;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();
                    cs.Close();
                }
                ms.Close();
            }
            return cipherBytes;
        }

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="cipherText">Encryptd byte array</param>
        /// <returns>Decrypted byte array</returns>
        public static byte[] AESDecrypt(byte[] cipherText)
        {
            SymmetricAlgorithm aes = Rijndael.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Iv;
            var decryptBytes = new byte[cipherText.Length];
            using (var ms = new MemoryStream(cipherText))
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    cs.Read(decryptBytes, 0, decryptBytes.Length);
                    cs.Close();
                }
                ms.Close();
            }
            return decryptBytes;
        }
    }
}