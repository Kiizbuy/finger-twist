using UnityEngine;

namespace GameFramework.Settings
{
    [CreateAssetMenu(menuName = "GameSettings/SoundsVolume", fileName = "SoundsVolumeSettings")]
    public class SoundsVolumeSettings : GameSettingsSOData
    {
        [Range(0f, 1f)] public float MusicVolume = 1f;
        [Range(0f, 1f)] public float SFXVolume = 1f;
        [Range(0f, 1f)] public float VoicesVolume = 1f;
    }
}

