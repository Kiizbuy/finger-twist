using NaughtyAttributes;
using System.IO;
using UnityEngine;
using Zenject;

namespace GameFramework.Settings
{
    public class GameSettingsSOData : ScriptableObject
    {
        [SerializeField, Header("Base Data Settings")]
        private string _fileNameExtension = ".json";
        [SerializeField, ReadOnly]
        private string _defaultSettings = _noneSaveInfo;
        [SerializeField, ReadOnly]
        private string _saveFileDataSettings = _noneSaveInfo;
      
        private bool _defaultSettingsHasInitialized = false;

        /// <summary>
        /// I Don't know, how to inject interface implementation into scriptable object
        /// </summary>
        [Inject]
        protected ISerializationProvider _serializationProvider = new UnityJsonSerialization();

        protected const string _noneSaveInfo = "(None)";

        public void InitDefaultSettings()
        {
            if (_defaultSettingsHasInitialized)
                return;

            _defaultSettings = _serializationProvider.SerializeObject(this);
            _defaultSettingsHasInitialized = true;

            TryLoad();
            OnInitializationHasComplete();
        }

        protected virtual string GetSavePath
            => Application.persistentDataPath;
        protected string GetFullPath(string filename = "")
            => Path.Combine(GetSavePath, GetSavePath);
        protected string GetFullFileName()
            => GetType().Name.ToString() + _fileNameExtension;
        protected bool FileDoesntExsist(FileInfo saveFile)
            => saveFile.Directory != null && !saveFile.Directory.Exists;
        protected bool CanLoadData()
            => _saveFileDataSettings != string.Empty || _saveFileDataSettings != _noneSaveInfo;

        public virtual void OnInitializationHasComplete() { }

        public virtual void Save()
        {
            var saveFile = new FileInfo(GetFullPath(GetFullFileName()));

            _saveFileDataSettings = _serializationProvider.SerializeObject(this);

            if (FileDoesntExsist(saveFile))
                saveFile.Directory.Create();

            File.WriteAllText(GetFullPath(GetFullFileName()), _saveFileDataSettings);
        }


        public virtual void TryLoad()
        {
            if (CanLoadData())
            {
                _serializationProvider.DeserializeObject(_saveFileDataSettings, this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            else
            {
                var saveData = new FileInfo(GetFullPath(GetFullFileName()));

                if (FileDoesntExsist(saveData))
                    return;

                _serializationProvider.DeserializeObject(File.ReadAllText(GetFullPath(GetFullFileName())), this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public virtual void ResetSettings()
        {
            if (_defaultSettingsHasInitialized)
            {
                _serializationProvider.DeserializeObject(_defaultSettings, this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

    }
}

