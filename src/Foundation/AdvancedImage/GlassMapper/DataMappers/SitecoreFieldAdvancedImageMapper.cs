using System.Xml;
using AdvancedImage.GlassMapper.Fields;
using AdvancedImage.Helpers;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.DataMappers;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Extensions;

namespace AdvancedImage.GlassMapper.DataMappers
{
    public class SitecoreFieldAdvancedImageMapper : AbstractSitecoreFieldMapper
    {
        public SitecoreFieldAdvancedImageMapper() : base(typeof(AdvancedImageField))
        {
        }

        public override string SetFieldValue(object value, SitecoreFieldConfiguration config,
            SitecoreDataMappingContext context)
        {
            throw new System.NotImplementedException();
        }

        public override object GetFieldValue(string fieldValue, SitecoreFieldConfiguration config,
            SitecoreDataMappingContext context)
        {
            throw new System.NotImplementedException();
        }
        public override object GetField(Field field, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            var sitecoreImage = new ImageField(field);
            var defaultImage = new AdvancedImageField();

            if (sitecoreImage.Value.IsEmptyOrNull())
            {
                return defaultImage;
            }

            var xml = new XmlDocument();
            xml.LoadXml(sitecoreImage.Value);

            var resultImage = AdvancedImageHelper.ConvertMediaItemToField(xml.DocumentElement, field.Database);
            return resultImage ?? defaultImage;
        }
    }
}