using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileEntity.Core
{
    public static class Encription
    {

        private static byte[] _SALT;
        private  static byte[] _Key;
        private  static byte[] _Vector;
        private  static Rfc2898DeriveBytes _EncriptionGenerator;
        public static string Encript(string text, string EncriptionKey)
        {

            _SALT = Encoding.ASCII.GetBytes(EncriptionKey);            
            _EncriptionGenerator = new Rfc2898DeriveBytes(EncriptionKey, _SALT);
            _Key = _EncriptionGenerator.GetBytes(32);
            _Vector = _EncriptionGenerator.GetBytes(16);

            RijndaelManaged SecurityCipher = new RijndaelManaged { Key = _Key, IV = _Vector};

            byte[] baseText = Encoding.Unicode.GetBytes(text);

            using (ICryptoTransform encriptor = SecurityCipher.CreateEncryptor())
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encriptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(baseText, 0, baseText.Length);
                        cryptoStream.FlushFinalBlock();
                        return Convert.ToBase64String(memStream.ToArray());                        
                    }
                }
            }
        }

        public static string Decript(string text, string EncriptionKey)
        {

            _SALT = Encoding.ASCII.GetBytes(EncriptionKey);            
            _EncriptionGenerator = new Rfc2898DeriveBytes(EncriptionKey, _SALT);
            _Key = _EncriptionGenerator.GetBytes(32);
            _Vector = _EncriptionGenerator.GetBytes(16);


            RijndaelManaged SecurityCipher = new RijndaelManaged();
            byte[] encryptedData = Convert.FromBase64String(text);

            using (ICryptoTransform decryptor = SecurityCipher.CreateDecryptor(_Key, _Vector))
            {
                using (MemoryStream memStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] baseText = new byte[encryptedData.Length];
                        int cryptoStreamCount = cryptoStream.Read(baseText, 0, baseText.Length);
                        return Encoding.Unicode.GetString(baseText, 0, cryptoStreamCount);
                    }
                }
            }

        }


    }
}
