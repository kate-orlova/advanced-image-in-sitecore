using Sitecore.Data.Items;
using Sitecore.Publishing.Pipelines.GetItemReferences;
using Sitecore.Publishing.Pipelines.PublishItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Comparers;
using Sitecore.Data.Fields;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.Publish;

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
            Assert.ArgumentNotNull(context, nameof(context));
            var objList = new List<Item>();
            if (context.PublishOptions.Mode != PublishMode.SingleItem)
                return objList;
            switch (context.Action)
            {
                case PublishAction.PublishSharedFields:
                    var sourceItem = context.PublishHelper.GetSourceItem(context.ItemId);
                    if (sourceItem == null)
                        return objList;
                    objList.AddRange(this.GetReferences(sourceItem, true, new HashSet<ID>()));
                    break;
                case PublishAction.PublishVersion:
                    var versionToPublish = context.VersionToPublish;
                    if (versionToPublish == null)
                        return objList;
                    objList.AddRange(this.GetReferences(versionToPublish, false, new HashSet<ID>()));
                    break;
                default:
                    return objList;
            }
            return objList;
        }

        private IEnumerable<Item> GetReferences(Item item, bool sharedOnly, HashSet<ID> processedItems)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            processedItems.Add(item.ID);
            var source = new List<Item>();
            var array = item.Links.GetValidLinks().Where(link =>
                item.Database.Name.Equals(link.TargetDatabaseName, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (sharedOnly)
                array = array.Where(link =>
                {
                    var sourceItem = link.GetSourceItem();
                    if (sourceItem == null)
                        return false;
                    if (!ID.IsNullOrEmpty(link.SourceFieldID))
                        return sourceItem.Fields[link.SourceFieldID].Shared;
                    return true;
                }).ToArray();
            foreach (var obj in array.Select(link => link.GetTargetItem()).Where(relatedItem => relatedItem != null)
                .ToList())
            {
                if (DeepScan && !processedItems.Contains(obj.ID))
                    source.AddRange(GetReferences(obj, sharedOnly, processedItems));
                source.AddRange(PublishQueue.GetParents(obj));
                source.Add(obj);
                source.AddRange(GetAdvancedImageFieldsLinkedMediaItems(obj));
                source.AddRange(GetAdvancedImageGalleryFieldsLinkedMediaItems(obj));
            }

            return source.Distinct(new ItemIdComparer());
        }

        protected virtual List<Item> GetAdvancedImageFieldsLinkedMediaItems(Item item)
        {
            var mediaList = new List<Item>();

            var advancedImageFields = item.Fields
                .Where(x => string.Equals(x.TypeKey, "advanced image", StringComparison.OrdinalIgnoreCase))
                .Select(x => new ImageField(x));

            foreach (var advancedImageField in advancedImageFields)
            {
                try
                {
                    var target = advancedImageField.MediaItem;
                    AddMediaItemAndParentsToPublishList(target, mediaList);
                }
                catch (Exception ex)
                {
                    Log.Error("Error publishing advanced image related media items", ex,
                        typeof(AddItemLinkReferencesExtended));
                }
            }

            return mediaList;
        }

        protected virtual List<Item> GetAdvancedImageGalleryFieldsLinkedMediaItems(Item item)
        {
            var mediaList = new List<Item>();

            var advancedImageGalleryFields = item.Fields
                .Where(x => string.Equals(x.TypeKey, "advanced image gallery", StringComparison.OrdinalIgnoreCase));

            foreach (var advancedImageGalleryField in advancedImageGalleryFields)
            {
                try
                {
                    var value = advancedImageGalleryField.Value;

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

                        var target = ItemManager.GetItem(new ID(mediaItemIdAttribute.Value), Language.Current,
                            Sitecore.Data.Version.Latest, item.Database);
                        AddMediaItemAndParentsToPublishList(target, mediaList);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error publishing advanced image gallery related media items", ex,
                        typeof(AddItemLinkReferencesExtended));
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