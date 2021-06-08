using System.ComponentModel;
using UnityEngine;

namespace GameFramework.UI
{
    public class BaseViewModelMonobehavior : MonoBehaviour, IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
