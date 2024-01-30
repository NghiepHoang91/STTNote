using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace STTNote.Extensions
{
    public static class CollectionExtension
    {
        public static TValue? GetValue<TKey, TValue>(this IDictionary<TKey, TValue> keyValues, TKey key)
        {
            if (keyValues == null)
                return default;
            return keyValues.TryGetValue(key, out TValue? value) ? value : default;
        }

        public static void Refresh<T>(this ObservableCollection<T> value)
        {
            CollectionViewSource.GetDefaultView(value).Refresh();
        }
    }
}