using UnityWeld.Binding;
using UnityWeld.Binding.Adapters;

namespace GameFramework.UI
{
    [Adapter(typeof(bool), typeof(string), typeof(BoolToStringAdapterOptions))]
    public class InverseBoolToStringAdapter : IAdapter
    {
        public object Convert(object valueIn, AdapterOptions options)
        {
            var adapterOptions = (BoolToStringAdapterOptions)options;

            return (bool)valueIn ? adapterOptions.FalseValueString : adapterOptions.TrueValueString;
        }
    }
}
