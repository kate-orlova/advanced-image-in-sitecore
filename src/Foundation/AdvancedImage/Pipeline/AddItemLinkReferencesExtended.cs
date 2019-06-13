using Sitecore.Data.Items;
using Sitecore.Publishing.Pipelines.GetItemReferences;
using Sitecore.Publishing.Pipelines.PublishItem;
using System;
using System.Collections.Generic;

namespace AdvancedImage.Pipeline
{
    public class AddItemLinkReferencesExtended : GetItemReferencesProcessor
    {
        public bool DeepScan { get; set; }

        protected override List<Item> GetItemReferences(PublishItemContext context)
        {
            throw new NotImplementedException();
        }
    }
}