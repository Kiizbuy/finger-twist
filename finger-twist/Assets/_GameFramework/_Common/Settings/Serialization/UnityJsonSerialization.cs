using UnityEngine;

namespace GameFramework.Settings
{
    public class UnityJsonSerialization : ISerializationProvider
    {
        public void DeserializeObject(string fileData, object deserializableObject) 
            => JsonUtility.FromJsonOverwrite(fileData, deserializableObject);

        public string SerializeObject(object serializableObject)
            => JsonUtility.ToJson(serializableObject);
    }
}
