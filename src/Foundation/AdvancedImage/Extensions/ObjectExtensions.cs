using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvancedImage.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object source, JsonSerializerSettings jsonSerializerSettings = null, NullValueHandling nullValueHandling = NullValueHandling.Include)
        {
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = nullValueHandling
                };
            }

            return JsonConvert.SerializeObject(source, jsonSerializerSettings);
        }
    }
}