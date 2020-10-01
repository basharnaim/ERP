using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library.Context.Utility
{
    public class PasswordManager
    {
        private string _passPhrase = string.Empty;
        private string _saltValue = string.Empty;
        private string _hashAlgorithm = string.Empty;
        private int _passwordIterations = 0;
        private string _initVector = string.Empty;
        private int _keySize = 0;

        public static string HashCode(string str)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create().ComputeHash(new ASCIIEncoding().GetBytes(str)));
            }
            catch (Exception ex)
            {
                return "Error in HashCode : " + ex.Message;
            }
        }

        public PasswordManager()
        {
            _passPhrase = "Pas5pr@se";       // can be any string
            _saltValue = "s@1tValue";        // can be any string
            _hashAlgorithm = "SHA1";         // can be "MD5"
            _passwordIterations = 2;         // can be any number
            _initVector = "@1B2c3D4e5F6g7H8";// must be 16 bytes
            _keySize = 256;                  // can be 192 or 128
        }

        /// <summary>
        /// This is for encryption.
        /// </summary>
        /// <param name="plainText">The string that is to be encrypt.</param>
        /// <returns>Returns an encrypted string.</returns>
        public string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);

            byte[] keyBytes = password.GetBytes(_keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }

        /// <summary>
        /// This is for decryption.
        /// </summary>
        /// <param name="cipherText">The string that is to be decrypt.</param>
        /// <returns>Returns a plain string.</returns>
        public string Decrypt(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);

            byte[] keyBytes = password.GetBytes(_keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }
    }
}
