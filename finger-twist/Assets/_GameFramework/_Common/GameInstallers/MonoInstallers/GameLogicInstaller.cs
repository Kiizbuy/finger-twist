using GameFramework.Events;
using GameFramework.Settings;
using Zenject;
using UnityEngine;

namespace GameFramework.Installers
{
    public class GameLogicInstaller : MonoInstaller
    {
        [SerializeField]
        private bool _useSoundSettings = false;

        public override void InstallBindings()
        {
            Container.Bind<GlobalEventsRouter>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            if (_useSoundSettings)
            {
                Container.Bind<SoundVolumeChanger>()
                    .FromNewComponentOnNewGameObject()
                    .AsSingle()
                    .NonLazy();
            }

        }
    }
}

