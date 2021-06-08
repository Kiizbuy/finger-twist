using GameFramework.Events;
using UnityEngine;
using Zenject;
using GameFramework.Settings;

namespace GameFramework.Debugging
{
    public class ZenjectBindTest : MonoBehaviour
    {
        [SerializeField, Inject]
        private SoundsVolumeSettings _soundsVolumeSettings;
        [SerializeField, Inject]
        private GameDifficultSettings _difficultSettings;
    }
}

