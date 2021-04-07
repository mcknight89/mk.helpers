using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace mk.helpers
{
    public class UserSettings<T> where T : class
    {
        enum storageType
        {
            InMemory,
            File
        }
        public T _store = null;
        public T _defaultStore = null;
        private SerializationType _type = SerializationType.Json;
        private storageType _storageType = storageType.InMemory;
        private string _filePath = null;

        public UserSettings(T defaultStore)
        {
            Init(defaultStore);
        }

        public UserSettings()
        {
            Init(null);
        }

        private void Init(T defaultStore)
        {
            _defaultStore = defaultStore;
        }

        public T Store
        {
            get
            {
                return _store??_defaultStore;
            }
            set
            {
                _store = value;
                Commit();
            }
        }

        public void Commit()
        {
            switch (_storageType)
            {
                case storageType.File:
                    WriteToFile(_filePath);
                    break;
            }
        }

        public void Read()
        {
            switch (_storageType)
            {
                case storageType.File:
                    ReadFromFile(_filePath);
                    break;
            }
        }

        public UserSettings<T> UseFileStorage(string path)
        {
            _storageType = storageType.File;
            _filePath = path;
            Read();
            return this;
        }

        public UserSettings<T> UseJson()
        {
            _type = SerializationType.Json;
            Read();
            return this;
        }

        public UserSettings<T> UseBson()
        {
            _type = SerializationType.Bson;
            Read();
            return this;
        }


        public void WriteToFile(string path)
        {
            var data = Serialization.Serialize<T>(_store, _type);
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("WriteToFile, failed to write to file", ex);
            }
        }

        public void ReadFromFile(string path)
        {
            try
            {
                var fileData = File.ReadAllBytes(path);
                _store = Serialization.Deserialize<T>(fileData, _type);
            }
            catch (Exception ex)
            {
                throw new Exception("ReadFromFile, failed to write to file", ex);
            }
        }

    }
}
