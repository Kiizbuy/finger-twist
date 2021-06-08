using GameFramework.Extension;
using GameFramework.Settings;
using System.Linq;
using UnityEngine;
using Zenject;

namespace GameFramework.Installers
{ 
    public class GameSettingsInstaller : Installer
    {
        public override void InstallBindings()
        {
            BindGameSettings();
        }

        private void BindGameSettings()
        {
            var allGameSettings = Resources.LoadAll<GameSettingsSOData>(string.Empty);

            if (allGameSettings.Length == 0)
            {
                Debug.LogError("Can't inject all game settings data");
                return;
            }

            foreach (var currentGameSettings in allGameSettings)
            {
                var gameSettingsInheritanceTree = currentGameSettings.GetType()
                                                    .GetInheritanceHierarchy()
                                                    .Where(x => x.IsSubclassOf(typeof(GameSettingsSOData)) || x == typeof(GameSettingsSOData));

                Container.Bind(gameSettingsInheritanceTree)
                    .FromInstance(currentGameSettings)
                    .AsCached()
                    .NonLazy();

                currentGameSettings.InitDefaultSettings();
            }
        }
    }
}

