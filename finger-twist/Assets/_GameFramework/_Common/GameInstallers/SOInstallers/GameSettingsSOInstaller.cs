using UnityEngine;
using Zenject;
using GameFramework.Settings;

namespace GameFramework.Installers
{
    [CreateAssetMenu(fileName = "GameSettingsSOInstaller", menuName = "Installers/GameSettingsSOInstaller")]
    public class GameSettingsSOInstaller : ScriptableObjectInstaller<GameSettingsSOInstaller>
    {
        public override void InstallBindings()
        {
            Container.Install<GameSettingsInstaller>();

            Container.Bind<ISerializationProvider>()
                .To<UnityJsonSerialization>()
                .AsSingle();
        }
    }
}