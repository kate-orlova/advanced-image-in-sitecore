using Sitecore.Data.Items;
using Sitecore.Publishing.Pipelines.GetItemReferences;
using Sitecore.Publishing.Pipelines.PublishItem;
using System;
using System.Collections.Generic;
using Sitecore;

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