using System.Linq;
using System.Reflection;
using UnityWeld.Binding;

namespace GameFramework.UI
{
    public static class ViewModelExtension
    {
        public static void AllPropertyChanged<T>(this T viewModelObject) where T : IViewModel
        {
            var type = viewModelObject.GetType();
            var properties = type.GetProperties();

            var props = properties.Where(info =>
                info.GetCustomAttribute<BindingAttribute>(false) != null
                && !(info.PropertyType.IsGenericType
                && info.PropertyType.GetGenericTypeDefinition() == typeof(ObservableList<>)));

            foreach (var propertyInfo in props)
                viewModelObject.OnPropertyChanged(propertyInfo.Name);
        }
    }
}
