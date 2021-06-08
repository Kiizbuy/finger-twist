using System;
using ModestTree.Util;
using UnityEngine.Events;

namespace GameFramework.UI
{
    public interface IScreenNavigator
    {
        event Action OnHistoryPanelsHasBeenEnded; 
        void NavigateBack();
        void NavigateTo(IScreen screen, bool saveHistory);
        void ClearHistory();
    }
}
