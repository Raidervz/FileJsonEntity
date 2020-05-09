using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileEntity.Core
{

    public class FileEntity<T> : IDisposable
    {
        private  string _JsonFileName;
        private  string _JsonDir;
        private  string _FullPath;
        private  bool _Encript;
        private  string _EncriptionKey;

        public FileEntity(string FileName, string Dir, bool Encript = false, string EncrptionKey = default(string))
        {
            _JsonFileName = FileName;
            _JsonDir = Dir;
            _FullPath = _JsonDir + _JsonFileName;
            _Encript = Encript;
            _EncriptionKey = EncrptionKey;
        }

        private bool CheckJsonFileExist(string FullPath)
        {
            return File.Exists(FullPath);
        }

        private void CreateJsonFile(string FullPath)
        {
            File.WriteAllText(FullPath, EncriptValidator("[ ]"));
        }

        private string EncriptValidator(string text)
        {
            return _Encript ? Encription.Encript(text, _EncriptionKey) : text;
        }

        private string DecriptValidator(string text)
        {
            return _Encript ? Encription.Decript(text, _EncriptionKey) : text;
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
            if(Entity ==null) throw new ArgumentNullException(nameof(Entity));

            ValidateJsonFile(_FullPath);

            var jsonEncripted = File.ReadAllText(_FullPath);

            var jsonFile = DecriptValidator(jsonEncripted);

            JArray jObjectArray = JArray.Parse(jsonFile);

            var _Entities = jObjectArray.ToObject<List<T>>();

            _Entities.Add(Entity);

            string jsonOutFile = JsonConvert.SerializeObject(_Entities);

            File.WriteAllText(_FullPath, EncriptValidator(jsonOutFile));

            return Entity;
        }

        public T Update (T Entity, string index)
        {
            if(Entity ==null) throw new ArgumentNullException(nameof(Entity));
            if(string.IsNullOrWhiteSpace(index)) throw new ArgumentNullException(nameof(index));
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

            var _Entities = updatedJsonArray.ToObject<List<T>>();

            string jsonOutFile = JsonConvert.SerializeObject(_Entities);

            File.WriteAllText(_FullPath, EncriptValidator(jsonOutFile));

            return Entity;

        }

        public bool Delete( T Entity, string index)
        {
            if(Entity ==null) throw new ArgumentNullException(nameof(Entity));
            if(string.IsNullOrWhiteSpace(index)) throw new ArgumentNullException(nameof(index));            
                        
            ValidateJsonFile(_FullPath);

            try 
            {
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

                var _Entities = updatedJsonArray.ToObject<List<T>>();

                string jsonOutFile = JsonConvert.SerializeObject(_Entities);

                File.WriteAllText(_FullPath, EncriptValidator(jsonOutFile));

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public T FindFirst( Dictionary<string, string> parameters)
        {
            if(parameters.Count ==0) throw new ArgumentNullException(nameof(parameters));
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
                    if (item[parameter.Key].ToString() == parameter.Value)
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

            return result;
        }

        public List<T> FindMany (Dictionary<string, string> parameters)
        {
            if(parameters.Count ==0) throw new ArgumentNullException(nameof(parameters));
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
                    if (item[parameter.Key].ToString() == parameter.Value)
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

            return result;
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
        }
    }

}