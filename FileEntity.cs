using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileJsonEntity
{

    public class FileEntity<T> : IDisposable
    {
        private  string _JsonFileName;
        private  string _JsonDir;
        private  string _FullPath;
        private T _Entity;
        private List<T> _Entities;
        private  byte[] _SALT;
        private  byte[] _Key;
        private  byte[] _Vector;
        private  Rfc2898DeriveBytes _EncriptionGenerator;
        private  bool _Encript;
        private  string _EncriptionKey;

        public FileEntity(string FileName, string Dir, bool Encript = false, string EncrptionKey = default(string))
        {
            _JsonFileName = FileName;
            _JsonDir = Dir;
            _FullPath = _JsonDir + _JsonFileName;
            _Entities = new List<T>();
            _Entity = default(T);
            _Encript = Encript;
            _EncriptionKey = EncrptionKey;
            _SALT = Encoding.ASCII.GetBytes(EncrptionKey);            
            _EncriptionGenerator = new Rfc2898DeriveBytes(EncrptionKey, _SALT);
            _Key = _EncriptionGenerator.GetBytes(32);
            _Vector = _EncriptionGenerator.GetBytes(16);
        }

        private string Encript(string text)
        {
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

        private string Decript(string text)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] encryptedData = Convert.FromBase64String(text);

            using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(_Key, _Vector))
            {
                using (MemoryStream memStream = new MemoryStream())
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

        private bool CheckJsonFileExist(string FullPath)
        {
            return File.Exists(FullPath);
        }

        private void CreateJsonFile(string FullPath)
        {
            File.WriteAllLines(FullPath, new string [] { EncriptValidator("[ ]")});
        }

        private string EncriptValidator(string text)
        {
            return _Encript ? Encript(text) : text;
        }

        private string DecriptValidator(string text)
        {
            return _Encript ? Decript(text) : text;
        }

        private void ValidateJsonFile( string FullPath)
        {
            if (!CheckJsonFileExist(_FullPath))
            {
                CreateJsonFile(_FullPath);
            }
        }

        public T Add( T Entity)
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            _Entities = jObjectArray.ToObject<List<T>>();

            _Entities.Add(Entity);

            _Entity = Entity;

            string jsonOutFile = JsonConvert.SerializeObject(_Entities);

            File.WriteAllLines(_FullPath, new string[] {EncriptValidator(jsonOutFile)});

            return _Entity;
        }

        public T Update (T Entity, string index)
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            string entityJson = JsonConvert.SerializeObject(Entity);

            JObject entityJsonObject = JObject.Parse(entityJson);

            JArray updatedJsonArray = new JArray();

            foreach (var item in jObjectArray)
            {
                if (item[index].ToString() == entityJsonObject[index].ToString())
                {
                    updatedJsonArray.Add(entityJsonObject);
                }
                else
                {
                    updatedJsonArray.Add(item);
                }
            }

            _Entities = updatedJsonArray.ToObject<List<T>>();

            _Entity = Entity;

            string jsonOutFile = JsonConvert.SerializeObject(_Entities);

            File.WriteAllLines(_FullPath, new string[] {EncriptValidator(jsonOutFile)});

            return _Entity;

        }

        public bool Delete( T Entity, string index)
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            string entityJson = JsonConvert.SerializeObject(Entity);

            JObject entityJsonObject = JObject.Parse(entityJson);

            JArray updatedJsonArray = new JArray();

            foreach (var item in jObjectArray)
            {
                if (item[index].ToString() != entityJsonObject[index].ToString())
                {
                    updatedJsonArray.Add(item);
                }
            }

            _Entities = updatedJsonArray.ToObject<List<T>>();

            _Entity = Entity;

            string jsonOutFile = JsonConvert.SerializeObject(_Entities);

            File.WriteAllLines(_FullPath, new string[] {EncriptValidator(jsonOutFile)});

            return true;
        }

        public T FindFirst( Dictionary<string, string> parameters)
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            T result = default(T);

            foreach (var item in jObjectArray)
            {
                List<bool> ExactMatch = new List<bool>();

                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (item[parameter.Key].ToString() == parameter.ToString())
                    {
                        ExactMatch.Add(true);
                    }
                    else
                    {
                        ExactMatch.Add(false);
                    }
                }

                if(ExactMatch.FindAll( delegate(bool value) {return value == true;}).Count == parameters.Count)
                {
                    result = item.ToObject<T>();
                    break;
                }
            }

            _Entity = result;

            return _Entity;
        }

        public List<T> FindMany (Dictionary<string, string> parameters)
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            List<T> result = new List<T>();

            foreach (var item in jObjectArray)
            {
                List<bool> ExactMatch = new List<bool>();

                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (item[parameter.Key].ToString() == parameter.ToString())
                    {
                        ExactMatch.Add(true);
                    }
                    else
                    {
                        ExactMatch.Add(false);
                    }
                }

                if(ExactMatch.FindAll( delegate(bool value) {return value == true;}).Count == parameters.Count)
                {
                    result.Add(item.ToObject<T>());
                }
            }

            _Entities = result;

            return _Entities;
        }

        public List<T> FindAll ()
        {
            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            return jObjectArray.ToObject<List<T>>();

        }

        public void Dispose()
        {
            _JsonFileName = default(string);
            _JsonDir = default(string);
            _FullPath = default(string);
            _Encript = default(bool);
            _EncriptionKey = default(string);
            _SALT = default(byte[]);            
            _EncriptionGenerator = null;
            _Key = null;
            _Vector = null;
            _Entities = null;
            _Entity = default(T);
        }
    }

}