using UnityEngine;
using UnityEngine.Audio;

namespace GameFramework.Settings
{
    public class SoundVolumeChanger : MonoBehaviour
    {
        private AudioMixer _mainMixer;

        private readonly string _mainMixerName = "GameMasterSound";
        private readonly string _musicVolumeMixerName = "Music";
        private readonly string _sfxVolumeMixerName = "SFX";
        private readonly string _voicesVolumeMixerName = "Voices";
        private readonly float _volumeMultiplier = 20f;

        private void Start()
        {
            _mainMixer = Resources.Load<AudioMixer>(_mainMixerName);

            if (_mainMixer == null)
                Debug.LogError("Audio mixer with name 'GameMasterSound' is not Found on Resources Folder");
        }

        public void ChangeMusicVolume(float normalizedSoundVolumeValue = 1f)
            => SetVolume(_musicVolumeMixerName, normalizedSoundVolumeValue);

        public void ChangeSFXVolume(float normalizedSoundVolumeValue = 1f)
            => SetVolume(_sfxVolumeMixerName);

        public void ChangeVoicesVolume(float normalizedSoundVolumeValue = 1f)
            => SetVolume(_voicesVolumeMixerName, normalizedSoundVolumeValue);

        public void SetVolume(string mixerChanelName, float normalizedSoundVolumeValue = 1f)
        {
            normalizedSoundVolumeValue = Mathf.Clamp01(normalizedSoundVolumeValue);

            _mainMixer.SetFloat(mixerChanelName, Mathf.Log10(normalizedSoundVolumeValue) * _volumeMultiplier);
        }
    }
}

