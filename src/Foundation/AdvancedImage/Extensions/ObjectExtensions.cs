using Glass.Mapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace AdvancedImage.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object source, JsonSerializerSettings jsonSerializerSettings = null,
            NullValueHandling nullValueHandling = NullValueHandling.Include)
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

        public static NameValueCollection GetHtmlAttributeCollection(this object target, bool lowerCaseName = false,
            bool underscoreForHyphens = true)
        {
            var nameValueCollection = new NameValueCollection();
            if (target == null) return nameValueCollection;

            foreach (var property in Utilities.GetAllProperties(target.GetType()).OrderBy(p => p.Name == "class"))
            {
                var propertyName = lowerCaseName ? property.Name.ToLower() : property.Name;
                if (underscoreForHyphens)
                {
                    propertyName = propertyName.Replace("_", "-");
                }

                var propertyValue = property.GetValue(target);
                if (propertyValue == null)
                {
                    nameValueCollection.Add(propertyName, string.Empty);
                    continue;
                }

                var val = propertyValue.GetType().IsArray
                    ? string.Join(",", propertyValue as IEnumerable)
                    : propertyValue.ToString();

                nameValueCollection.Add(propertyName, val);
            }

            return nameValueCollection;
        }
    }
}