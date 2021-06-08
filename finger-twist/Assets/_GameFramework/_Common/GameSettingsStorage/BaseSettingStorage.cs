using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework.Saving.Settings
{
    public abstract class BaseSettingStorage<T> : ISettingsStorage<T>, IDisposable where T : class, new()
    {
        public event Action<T> OnSettingsHasChanged;
        
        private ISettingsSerializer _serializer;

        private T _settings = new T();
        private readonly string _dataSaveRootPath = Application.persistentDataPath;
        private readonly string _dataSaveFileName = "PlayerData.settings";

        public ISettingsStorage<T> InjectSerializer(ISettingsSerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public T Settings => _settings;

        private string GetFullPath()
            => Path.Combine(_dataSaveRootPath, _dataSaveFileName);
        private string GetFullPath(string saveFilePath, string saveFile)
            => Path.Combine(saveFilePath, saveFile);

        private bool SaveFileExist(FileInfo saveFile)
            => saveFile.Directory != null && saveFile.Directory.Exists;

        public void Save(string savePath = null, string fileName = null)
        {
            var saveFile = new FileInfo( (savePath != null && fileName != null) ? GetFullPath(savePath, fileName) : GetFullPath());
            if (!SaveFileExist(saveFile))
                saveFile.Directory.Create();

            var saveData = _serializer.Serialize(_settings);

            File.WriteAllText(GetFullPath(), saveData);
            OnSettingsHasChanged?.Invoke(_settings);
        }

        public ISettingsStorage<T> Load(string savePath, string saveFile)
        {
            if (savePath == null && saveFile == null)
                return Load();
            var fullSaveFilePath = GetFullPath(savePath, saveFile);
            var saveFileInfo = new FileInfo(fullSaveFilePath);
            if (SaveFileExist(saveFileInfo))
            {
                var saveFileData = File.ReadAllText(fullSaveFilePath);
                _settings = _serializer.Deserialize<T>(saveFileData);
                OnSettingsHasChanged?.Invoke(_settings);
            }

            return this;
        }

        private ISettingsStorage<T> Load()
        {
            var fullSaveFilePath = GetFullPath(_dataSaveRootPath, _dataSaveFileName);
            var saveFileInfo = new FileInfo(fullSaveFilePath);
            if (SaveFileExist(saveFileInfo))
            {
                var saveFileData = File.ReadAllText(fullSaveFilePath);
                _settings = _serializer.Deserialize<T>(saveFileData);
                OnSettingsHasChanged?.Invoke(_settings);
            }

            return this;
        }

        public void Delete(string savePath, string saveFile)
        {
            throw new NotImplementedException();
        }

        private void Delete()
        {
            throw new NotImplementedException();
        }
        
        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public bool Has(string savePath, string saveFile)
        {
            return savePath != null && saveFile != null
                ? SaveFileExist(new FileInfo(GetFullPath(savePath, saveFile)))
                : SaveFileExist(new FileInfo(GetFullPath()));
        }

        public T TryGet(T defaultValue)
        {
            return _settings ?? defaultValue;
        }

        public void Dispose()
        {
            _settings = null;
        }
    }
}