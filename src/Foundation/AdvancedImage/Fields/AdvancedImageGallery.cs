using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using AdvancedImage.Extensions;
using AdvancedImage.Fields.Editor;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Shell.Applications.Dialogs.MediaBrowser;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

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

        public string ItemVersion
        {
            get { return GetViewStateString("Version"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Version", value);
            }
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

        private List<AdvancedImageEditorItem> GetImageEditors()
        {
            var imageCollection = GetXmlImages().ToArray();
            var imageEditorsList = new List<AdvancedImageEditorItem>();
            if (imageCollection.Any())
            {
                foreach (var image in imageCollection)
                {
                    var imageId = image.GetAttribute("mediaid");
                    var mediaItem = GetMediaItem(imageId);
                    if (mediaItem != null)
                    {
                        GetMediaItemSrc(mediaItem, out var src);
                        var newImageEditor = new AdvancedImageEditorItem
                        {
                            ControlId = ID,
                            ImageId = imageId,
                            ImageAlt = HttpUtility.HtmlEncode(mediaItem["Alt"]),
                            CropFocus =
                                $"{image.GetAttribute("cropx")},{image.GetAttribute("cropy")},{image.GetAttribute("focusx")},{image.GetAttribute("focusy")},{image.GetAttribute("showFull")}",
                            ImageSrc = src,
                            ShowFull = image.GetAttribute("showFull").ParseOrDefault(false)
                        };
                        imageEditorsList.Add(newImageEditor);
                    }
                }
            }

            return imageEditorsList;
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            base.DoRender(output);
            var editorModel = new AdvancedImageGalleryEditorModel
            {
                ControlId = ID,
                Thumbnails = GetThumbnails(),
                IsDebug = "0",
                Details = GetDetails(),
                Images = GetImageEditors()
            };
            var testViewRender =
                HtmlHelperExtensions.GetRazorViewAsString("~/Views/Shared/Fields/AdvancedImageGallery.cshtml",
                    editorModel);
            output.Write(new MvcHtmlString(testViewRender));
        }

        protected void Update(bool showCropper = true)
        {
            var updateModel = new AdvancedImageEditorUpdateModel
            {
                Details = this.GetDetails(),
                Thumbnails = this.GetThumbnails()
            };
            var updateView =
                HtmlHelperExtensions.GetRazorViewAsString("~/Views/Shared/Fields/AdvancedImageUpdate.cshtml",
                    updateModel);

            SheerResponse.SetInnerHtml(string.Concat(this.ID, "_details"), updateView);
            SheerResponse.Eval("scContent.startValidators()");
        }

        public void ClearCollection()
        {
            if (Disabled)
            {
                return;
            }

            XmlValue = new XmlValue(string.Empty, "gallery");
        }

        private void UpdateImageGalleryUI(string contextImageId)
        {
            var data = new
            {
                detail = new
                {
                    imageId = contextImageId,
                    html = HttpUtility.HtmlEncode(HtmlHelperExtensions.GetRazorViewAsString(
                        "~/Views/Shared/Fields/AdvancedImageGalleryItems.cshtml", GetImageEditors()))
                }
            };

            SheerResponse.Eval(
                $"document.querySelector('#{ID}_pane').dispatchEvent(new CustomEvent('updateGallery', {data.ToJson()}));");
        }

        private void ItemRemove(string imageId)
        {
            var xGallery = XmlValue.Xml.ToXDocument();
            var targetImage = xGallery.Descendants().FirstOrDefault(e => imageId == e.Attribute("mediaid")?.Value);
            targetImage?.Remove();
            XmlValue = new XmlValue(xGallery.ToString(), "gallery");
            SetModified();

            UpdateImageGalleryUI(null);
        }

        private void ItemMove(string imageId, bool left)
        {
            var xGallery = this.XmlValue.Xml.ToXDocument();
            var targetImage = xGallery.Descendants().FirstOrDefault(e => imageId == e.Attribute("mediaid")?.Value);
            var neighborImage = left ? targetImage?.PreviousNode : targetImage?.NextNode;
            if (neighborImage != null)
            {
                targetImage.Remove();
                if (left)
                {
                    neighborImage.AddBeforeSelf(targetImage);
                }
                else
                {
                    neighborImage.AddAfterSelf(targetImage);
                }
            }

            XmlValue = new XmlValue(xGallery.ToString(), "gallery");
            SetModified();

            UpdateImageGalleryUI(imageId);
        }

        private void UpdateCropParameters(Message message)
        {
            var imageId = message["imgId"];
            var xGallery = XmlValue.Xml.ToXDocument();
            var targetImage = xGallery.Descendants().FirstOrDefault(e => imageId == e.Attribute("mediaid")?.Value);

            if (targetImage != null)
            {
                targetImage.SetAttributeValue("cropx", message["cx"]);
                targetImage.SetAttributeValue("cropy", message["cy"]);
                targetImage.SetAttributeValue("focusx", message["fx"]);
                targetImage.SetAttributeValue("focusy", message["fy"]);
                targetImage.SetAttributeValue("showFull", message["sf"]);

                XmlValue = new XmlValue(xGallery.ToString(), "gallery");
                SetModified();
            }
        }

        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);

            if (message["id"] == this.ID)
            {
                if (message.Name.StartsWith("advancedimagegallery") && this.Disabled)
                {
                    return;
                }

                if (message.Name == "advancedimagegallery:additem")
                {
                    Sitecore.Context.ClientPage.Start(this, "AddItem");
                    return;
                }

                if (message.Name == "advancedimagegallery:clear")
                {
                    ClearCollection();
                    return;
                }

                if (message.Name == "advancedimagegallery:moveLeft")
                {
                    ItemMove(message["imgId"], true);
                    return;
                }

                if (message.Name == "advancedimagegallery:moveRight")
                {
                    ItemMove(message["imgId"], false);
                    return;
                }

                if (message.Name == "advancedimagegallery:remove")
                {
                    ItemRemove(message["imgId"]);
                    return;
                }

                if (message.Name == "advancedimagegallery:crop")
                {
                    UpdateCropParameters(message);
                }
            }
        }

        private void AddItem(ClientPipelineArgs args)
        {
            ParseParameters(this.Source);
            if (args.IsPostBack)
            {
                if (args.HasResult)
                {
                    LastSelectedItemId = args.Result;

                    var xGallery = XmlValue.Xml.ToXDocument();
                    if (xGallery == null)
                    {
                        return;
                    }

                    var existingImage = xGallery.Descendants()
                        .FirstOrDefault(e => LastSelectedItemId == e.Attribute("mediaid")?.Value);
                    if (existingImage != null)
                    {
                        SheerResponse.Eval("alert('Such media already included to gallery')");
                        return;
                    }

                    //<image mediaid="{xxx}" cropx="0.71" cropy="0.22" focusx="0.421875" focusy="0.5625" showFull="false" />
                    var newMediaNode = new XElement("image",
                        new XAttribute("mediaid", this.LastSelectedItemId),
                        new XAttribute("cropx", 0),
                        new XAttribute("cropy", 0),
                        new XAttribute("focusx", 0),
                        new XAttribute("focusy", 0),
                        new XAttribute("showFull", false)
                    );

                    xGallery.Root.Add(newMediaNode);

                    XmlValue = new XmlValue(xGallery.ToString(), "gallery");
                    Value = xGallery.ToString();
                    SetModified();
                    UpdateImageGalleryUI(this.LastSelectedItemId);
                }
            }
            else
            {
                string imagesSourceFolderPath = null;

                if (ImagesSourceFolderID != null)
                {
                    imagesSourceFolderPath = Client.GetItemNotNull(new ID(ImagesSourceFolderID)).Paths.Path;
                }

                var source = new UrlString(StringUtil.GetString(imagesSourceFolderPath, "/sitecore/media library"))
                    .Path;

                Item lastSelectedItem = null;
                if (!string.IsNullOrEmpty(this.LastSelectedItemId))
                {
                    lastSelectedItem = Client.ContentDatabase.GetItem(this.LastSelectedItemId);
                }

                if (source.Contains("/sitecore/media library"))
                {
                    var options = new MediaBrowserOptions();

                    if (source.StartsWith("~"))
                    {
                        options.Root = Client.GetItemNotNull(ItemIDs.MediaLibraryRoot);
                        options.SelectedItem = Client.GetItemNotNull(source.Substring(1));
                    }
                    else
                    {
                        options.Root = Client.GetItemNotNull(source);
                    }

                    if (lastSelectedItem != null && lastSelectedItem.Parent.Paths.IsDescendantOf(options.Root))
                    {
                        options.SelectedItem = lastSelectedItem.Parent;
                    }

                    SheerResponse.ShowModalDialog(options.ToUrlString().ToString(), true);
                }
                else
                {
                    var options = new SelectItemOptions
                    {
                        Title = "Please select an item",
                        Text = "Select an item to add",
                        Icon = "Applications/32x32/star_green.png"
                    };
                    if (source.StartsWith("~"))
                    {
                        options.Root = Client.GetItemNotNull(ItemIDs.ContentRoot);
                        options.SelectedItem = Client.GetItemNotNull(source.Substring(1));
                    }
                    else
                    {
                        options.Root = Client.GetItemNotNull(source);
                    }

                    if (lastSelectedItem != null && lastSelectedItem.Paths.IsDescendantOf(options.Root))
                    {
                        options.SelectedItem = lastSelectedItem;
                    }

                    SheerResponse.ShowModalDialog(options.ToUrlString().ToString(), true);
                }

                args.WaitForPostBack();
            }
        }
    }
}