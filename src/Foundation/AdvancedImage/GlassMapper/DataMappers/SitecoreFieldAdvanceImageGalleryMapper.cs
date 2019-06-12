using System.Collections.Generic;
using System.Xml;
using AdvancedImage.GlassMapper.Fields;
using AdvancedImage.Helpers;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.DataMappers;
using Sitecore.Data.Fields;
using Sitecore.StringExtensions;

namespace AdvancedImage.GlassMapper.DataMappers
{
    public class SitecoreFieldAdvanceImageGalleryMapper : AbstractSitecoreFieldMapper
    {
        public SitecoreFieldAdvanceImageGalleryMapper()
            : base(typeof(AdvanceImageGalleryField))
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
            var sitecoreGalleryField = new LinkField(field);
            var advancedGallery = new AdvanceImageGalleryField();

            var stringValue = sitecoreGalleryField.Value.IsNullOrEmpty() ? "<gallery />" : sitecoreGalleryField.Value;

            var xml = new XmlDocument();
            xml.LoadXml(stringValue);
            var gallery = xml.DocumentElement;
            var galleryImages = new List<AdvanceImageField>();
            if (gallery != null && gallery.HasChildNodes)
            {
                foreach (XmlElement galleryChildNode in gallery.ChildNodes)
                {
                    var img = AdvancedImageHelper.ConvertMediaItemToField(galleryChildNode, field.Database);
                    if (img == null) continue;

                    galleryImages.Add(img);
                }
            }

            advancedGallery.GalleryItems = galleryImages;
            return advancedGallery;
        }
    }
}