using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Settings
{
    public enum DifficultType
    {
        VeryEasy,
        Easy,
        Normal,
        Hard,
        VeryHard
    };

    [CreateAssetMenu(menuName = "GameSettings/Difficult", fileName = "DifficultSettings")]
    public class GameDifficultSettings : GameSettingsSOData
    {
        public DifficultType Difficult;
    }
}

