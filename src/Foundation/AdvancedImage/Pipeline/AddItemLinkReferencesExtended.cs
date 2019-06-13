using Sitecore.Data.Items;
using Sitecore.Publishing.Pipelines.GetItemReferences;
using Sitecore.Publishing.Pipelines.PublishItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace AdvancedImage.Pipeline
{
    public class AddItemLinkReferencesExtended : GetItemReferencesProcessor
    {
        public bool DeepScan { get; set; }

        public AddItemLinkReferencesExtended()
        {
            DeepScan = true;
        }
        protected override List<Item> GetItemReferences(PublishItemContext context)
        {
            throw new NotImplementedException();
        }
        protected virtual List<Item> GetAdvanceImageGalleryFieldsLinkedMediaItems(Item item)
        {
            var mediaList = new List<Item>();

            var advanceImageGalleryFields = item.Fields
                .Where(x => string.Equals(x.TypeKey, "advance image gallery", StringComparison.OrdinalIgnoreCase));

            foreach (var advanceImageGalleryField in advanceImageGalleryFields)
            {
                try
                {
                    var value = advanceImageGalleryField.Value;

                    if (value == null)
                        continue;

                    var galleryXmlDocument = new XmlDocument
                    {
                        InnerXml = value
                    };

                    var imageValues = galleryXmlDocument.GetElementsByTagName("image");

                    foreach (var imageValue in imageValues)
                    {
                        if (!(imageValue is XmlNode xmlNode))
                            continue;

                        var mediaItemIdAttribute = xmlNode.Attributes?["mediaid"];

                        if (string.IsNullOrWhiteSpace(mediaItemIdAttribute?.Value))
                            continue;

                        var target = ItemManager.GetItem(new ID(mediaItemIdAttribute.Value), Language.Current, Sitecore.Data.Version.Latest, item.Database);
                        AddMediaItemAndParentsToPublishList(target, mediaList);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error publishing advance image gallery related media items", ex, typeof(AddItemLinkReferencesExtended));
                }
            }

            return mediaList;
        }
        private void AddMediaItemAndParentsToPublishList(Item target, List<Item> mediaList)
        {
            if (target == null || !target.Paths.IsMediaItem)
                return;

            var parent = target.Parent;
            while (parent != null && parent.ID != ItemIDs.MediaLibraryRoot)
            {
                mediaList.Insert(0, parent);
                parent = parent.Parent;
            }
            mediaList.Add(target);
        }
    }
}