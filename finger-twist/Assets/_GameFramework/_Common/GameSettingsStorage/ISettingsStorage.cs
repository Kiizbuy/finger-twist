using System;
using UnityEngine;

namespace GameFramework.Saving.Settings
{
    public interface ISettingsStorage<TSettings> where TSettings : class, new()
    {
        event Action<TSettings> OnSettingsHasChanged;

        ISettingsStorage<TSettings> InjectSerializer(ISettingsSerializer serializer);
        
        TSettings Settings { get; }
        void Save(string savePath, string fileName);
        ISettingsStorage<TSettings> Load(string savePath, string fileName);
        void Delete(string savePath, string fileName);
        void DeleteAll();
        bool Has(string savePath, string fileName);
    
        TSettings TryGet(TSettings defaultValue);
    }

    public interface ISettingsSerializer
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string filePath);
    }

    public class UnityJsonSerializer : ISettingsSerializer
    {
        public string Serialize<T>(T value)
        {
            return value != null ? JsonUtility.ToJson(value) : null;
        }

        public T Deserialize<T>(string filePath)
        {
            return JsonUtility.FromJson<T>(filePath);
        }
    }
}
