﻿using System.ComponentModel;

namespace GameFramework.UI
{
    public class BaseViewModel : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
