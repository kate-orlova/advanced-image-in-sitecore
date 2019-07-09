using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AdvancedImage.Fields.Editor;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;

namespace AdvancedImage.Fields
{
    public class AdvancedImageGallery : LinkBase
    {
        private const string THUMBNAIL_FOLDER_FIELD_NAME = "ThumbnailsFolderID";
        private const string IMAGES_SOURCE_FOLDER_FIELD_NAME = "ImagesSourceFolderID";
        private const string IS_DEBUG_FIELD_NAME = "IsDebug";
        private const string ASSETS_FOLDER_ID = "xxx-yyyy-zzzz";

        public AdvancedImageGallery()
        {
            Visible = false;
            Class = "scContentControlImage";
            Change = "#";
            Activation = true;
        }

        protected XmlValue XmlValue
        {
            get
            {
                XmlValue viewStateProperty = this.GetViewStateProperty("XmlValue", null) as XmlValue;
                if (viewStateProperty == null)
                {
                    viewStateProperty = new XmlValue(string.Empty, "gallery");
                    XmlValue = viewStateProperty;
                }

                return viewStateProperty;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateProperty("XmlValue", value, null);
            }
        }

        public string LastSelectedItemId
        {
            get => StringUtil.GetString(ViewState["LastSelectedItemID"]);
            set => ViewState["LastSelectedItemID"] = value;
        }

        protected string ThumbnailsFolderID { get; private set; }
        protected string ImagesSourceFolderID { get; private set; }
        protected string IsDebug { get; private set; }

        public override string GetValue()
        {
            return XmlValue.ToString();
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            XmlValue = new XmlValue(value, "gallery");
            Value = GetMediaPath();
        }

        protected void SetValue(MediaItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            XmlValue.SetAttribute("mediaid", item.ID.ToString());
            Value = GetMediaPath();
        }

        private AdvancedImageEditorDetailsModel GetDetails()
        {
            return new AdvancedImageEditorDetailsModel
            {
                XmlValue = XmlValue,
            };
        }

        private Item GetMediaItem(string imageId)
        {
            if (imageId.Length <= 0)
            {
                return null;
            }

            Language language = Language.Parse(ItemLanguage);
            return Client.ContentDatabase.GetItem(new ID(imageId), language);
        }

        private string GetMediaPath()
        {
            return string.Empty;
        }

        private static void GetMediaItemSrc(MediaItem mediaItem, out string src)
        {
            const int minHeight = 128;
            const int maxWidth = 640;

            src = string.Empty;

            if (mediaItem == null)
            {
                return;
            }

            var thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(mediaItem);
            if (!int.TryParse(mediaItem.InnerItem["Height"], out var parsedHeight))
            {
                parsedHeight = minHeight;
            }

            thumbnailOptions.Height = Math.Min(minHeight, parsedHeight);
            thumbnailOptions.MaxWidth = maxWidth;
            thumbnailOptions.UseDefaultIcon = true;

            src = MediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
        }

        private static bool IsImageMedia(TemplateItem template)
        {
            Assert.ArgumentNotNull(template, "template");
            if (template.ID == TemplateIDs.VersionedImage || template.ID == TemplateIDs.UnversionedImage)
            {
                return true;
            }

            var baseTemplates = template.BaseTemplates;
            return baseTemplates.Any(IsImageMedia);
        }

        private AdvancedImageEditorThumbnailsModel GetThumbnails()
        {
            ParseParameters(Source);

            if (!string.IsNullOrEmpty(ThumbnailsFolderID))
            {
                var thumbnailFolderItem = Client.ContentDatabase.GetItem(new ID(ThumbnailsFolderID));
                if (thumbnailFolderItem != null && thumbnailFolderItem.HasChildren)
                {
                    return new AdvancedImageEditorThumbnailsModel
                    {
                        ControlId = ID,
                        Thumbnails = thumbnailFolderItem.Children
                    };
                }
            }

            return null;
        }

        private void ParseParameters(string source)
        {
            var parameters = new UrlString(source);
            var defaultThumbnailFolderId = Settings.GetSetting("DefaultThumbnailFolderId");
            ThumbnailsFolderID = Sitecore.Data.ID.IsID(defaultThumbnailFolderId)
                ? defaultThumbnailFolderId
                : string.Empty;

            ThumbnailsFolderID = Sitecore.Data.ID.IsID(parameters.Parameters[THUMBNAIL_FOLDER_FIELD_NAME])
                ? parameters.Parameters[THUMBNAIL_FOLDER_FIELD_NAME]
                : ThumbnailsFolderID;

            ImagesSourceFolderID = Sitecore.Data.ID.IsID(parameters.Parameters[IMAGES_SOURCE_FOLDER_FIELD_NAME])
                ? parameters.Parameters[IMAGES_SOURCE_FOLDER_FIELD_NAME]
                : null;

            if (!string.IsNullOrEmpty(parameters.Parameters[IS_DEBUG_FIELD_NAME]))
            {
                IsDebug = parameters.Parameters[IS_DEBUG_FIELD_NAME];
            }
        }

        private IEnumerable<XmlElement> GetXmlImages()
        {
            var gallery = XmlValue.Xml.DocumentElement;
            var galleryImages = new List<XmlElement>();
            if (gallery == null || !gallery.HasChildNodes)
                return galleryImages;

            foreach (XmlElement galleryChildNode in gallery.ChildNodes)
            {
                galleryImages.Add(galleryChildNode);
            }

            return galleryImages;
        }
    }
}