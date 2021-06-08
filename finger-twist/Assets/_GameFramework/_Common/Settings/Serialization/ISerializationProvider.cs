namespace GameFramework.Settings
{
    public interface ISerializationProvider
    {
        string SerializeObject(object serializableObject);
        void DeserializeObject(string fileData, object deserializableObject);
    }
}