using Newtonsoft.Json;

namespace STTNote.Extensions
{
    public static class ObjectExtension
    {
        public static T? Clone<T>(this T source)
        {
            if (source == null) return default(T);

            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}