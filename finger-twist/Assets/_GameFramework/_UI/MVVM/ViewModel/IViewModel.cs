using System.ComponentModel;

namespace GameFramework.UI
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }
}
