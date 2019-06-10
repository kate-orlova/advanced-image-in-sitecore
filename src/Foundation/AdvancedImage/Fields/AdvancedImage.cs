using System;
using System.Web;
using AdvancedImage.Fields.Editor;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Resources.Media;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp;

namespace AdvancedImage.Fields
{
    public class AdvancedImage : LinkBase
    {
        private const string THUMBNAIL_FOLDER_FIELD_NAME = "ThumbnailsFolderID";
        private const string IMAGES_SOURCE_FOLDER_FIELD_NAME = "ImagesSourceFolderID";
        private const string IS_DEBUG_FIELD_NAME = "IsDebug";
        private const string ASSETS_FOLDER_ID = "xxx-yyyy-zzzz";
        public string ItemVersion
        {
            get
            {
                return GetViewStateString("Version");
            }
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
                    viewStateProperty = new XmlValue(string.Empty, "image");
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
        protected string ThumbnailsFolderID { get; private set; }
        protected string ImageSourceFolderID { get; private set; }
        protected string IsDebug { get; private set; }
        protected string ImagesSourceFolderID { get; private set; }
        public AdvancedImage()
        {
            Visible = false;
            Class = "scContentControlImage";
            Change = "#";
            Activation = true;
        }
        protected void Browse()
        {
            if (Disabled)
            {
                return;
            }
            Sitecore.Context.ClientPage.Start(this, "BrowseImage");
        }
        public override string GetValue()
        {
            return XmlValue.ToString();
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            XmlValue = new XmlValue(value, "image");
            Value = GetMediaPath();
        }
        protected void SetValue(MediaItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            XmlValue.SetAttribute("mediaid", item.ID.ToString());
            Value = GetMediaPath();
        }

        private Item GetMediaItem()
        {
            string attribute = this.XmlValue.GetAttribute("mediaid");
            if (attribute.Length <= 0)
            {
                return null;
            }
            Language language = Language.Parse(this.ItemLanguage);
            return Client.ContentDatabase.GetItem(attribute, language);
        }

        private string GetMediaPath()
        {
            MediaItem mediaItem = this.GetMediaItem();
            if (mediaItem == null)
            {
                return string.Empty;
            }
            return mediaItem.MediaPath;
        }
        protected void LoadImage()
        {
            string attribute = this.XmlValue.GetAttribute("mediaid");
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
                return;
            }
            if (!UserOptions.View.ShowEntireTree)
            {
                Item item = Client.CoreDatabase.GetItem("/sitecore/content/Applications/Content Editor/Applications/MediaLibraryForm");
                if (item != null)
                {
                    Item item1 = Client.ContentDatabase.GetItem(attribute);
                    if (item1 != null)
                    {
                        UrlString urlString = new UrlString(item["Source"]);
                        urlString["pa"] = "1";
                        urlString["pa0"] = WebUtil.GetQueryString("pa0", string.Empty);
                        urlString["la"] = WebUtil.GetQueryString("la", string.Empty);
                        urlString["pa1"] = HttpUtility.UrlEncode(item1.Uri.ToString());
                        SheerResponse.SetLocation(urlString.ToString());
                        return;
                    }
                }
            }
            Language language = Language.Parse(this.ItemLanguage);
            ClientPage clientPage = Sitecore.Context.ClientPage;
            string[] name = { "item:load(id=", attribute, ",language=", language.Name, ")" };
            clientPage.SendMessage(this, string.Concat(name));
        }
        protected void ShowProperties(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (Disabled)
            {
                return;
            }
            string attribute = XmlValue.GetAttribute("mediaid");
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
                return;
            }
            if (!args.IsPostBack)
            {
                string str = FileUtil.MakePath("/sitecore/shell", ControlManager.GetControlUrl(new ControlName("Sitecore.Shell.Applications.Media.ImageProperties")));
                UrlString urlString = new UrlString(str);
                Item item = Client.ContentDatabase.GetItem(attribute, Language.Parse(ItemLanguage));
                if (item == null)
                {
                    SheerResponse.Alert("Select an image from the Media Library first.");
                    return;
                }
                item.Uri.AddToUrlString(urlString);
                UrlHandle urlHandle = new UrlHandle();
                urlHandle["xmlvalue"] = XmlValue.ToString();
                urlHandle.Add(urlString);
                SheerResponse.ShowModalDialog(urlString.ToString(), true);
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            {
                XmlValue = new XmlValue(args.Result, "image");
                Value = this.GetMediaPath();
                SetModified();
                Update();
            }
        }
        protected void Update(bool showCropper = true)
        {

        }
        private void GetSrc(out string src)
        {
            int num;
            src = string.Empty;
            MediaItem mediaItem = this.GetMediaItem();
            if (mediaItem == null)
            {
                return;
            }
            MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(mediaItem);
            if (!int.TryParse(mediaItem.InnerItem["Height"], out num))
            {
                num = 128;
            }
            thumbnailOptions.Height = Math.Min(128, num);
            thumbnailOptions.MaxWidth = 640;
            thumbnailOptions.UseDefaultIcon = true;
            src = MediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
        }
        private AdvancedImageEditorDetailsModel GetDetails()
        {
            MediaItem mediaItem = this.GetMediaItem();
            return new AdvancedImageEditorDetailsModel
            {
                MediaItem = mediaItem,
                XmlValue = this.XmlValue
            };
        }
        private void ParseParameters(string source)
        {
            var parameters = new UrlString(source);

            // SET THUMBNAIL FOLDER ID BY DEFAULT FROM CONSTANTS OR TAKE IT FROM FIELD SOURCE IF DEFINED SO
            ThumbnailsFolderID = Sitecore.Data.ID.IsID(ASSETS_FOLDER_ID) ? ASSETS_FOLDER_ID : string.Empty;

            ThumbnailsFolderID = Sitecore.Data.ID.IsID(parameters.Parameters[THUMBNAIL_FOLDER_FIELD_NAME])
                ? parameters.Parameters[THUMBNAIL_FOLDER_FIELD_NAME]
                : this.ThumbnailsFolderID;

            ImagesSourceFolderID = Sitecore.Data.ID.IsID(parameters.Parameters[IMAGES_SOURCE_FOLDER_FIELD_NAME])
                ? parameters.Parameters[IMAGES_SOURCE_FOLDER_FIELD_NAME]
                : null;

            // WHETHER TO SHOW RAW VALUES
            if (!string.IsNullOrEmpty(parameters.Parameters[IS_DEBUG_FIELD_NAME]))
            {
                IsDebug = parameters.Parameters[IS_DEBUG_FIELD_NAME];
            }
        }
        private AdvancedImageEditorThumbnailsModel GetThumbnails()
        {
            var src = string.Empty;

            GetSrc(out src);
            ParseParameters(Source);

            if (!string.IsNullOrEmpty(ThumbnailsFolderID) && !string.IsNullOrEmpty(src))
            {
                var thumbnailFolderItem = Client.ContentDatabase.GetItem(new ID(ThumbnailsFolderID));
                if (thumbnailFolderItem != null && thumbnailFolderItem.HasChildren)
                {
                    return new AdvancedImageEditorThumbnailsModel
                    {
                        ControlId = ID,
                        Thumbnails = thumbnailFolderItem.Children,
                        ImageSrc = src
                    };
                }
            }

            return null;
        }
        private void ClearImage()
        {
            if (Disabled)
            {
                return;
            }
            if (Value.Length > 0)
            {
                SetModified();
            }
            XmlValue = new XmlValue(string.Empty, "image");
            Value = string.Empty;
            Update();
        }
        private bool IsImageMedia(TemplateItem template)
        {
            Assert.ArgumentNotNull(template, "template");
            if (template.ID == TemplateIDs.VersionedImage || template.ID == TemplateIDs.UnversionedImage)
            {
                return true;
            }
            TemplateItem[] baseTemplates = template.BaseTemplates;
            for (int i = 0; i < baseTemplates.Length; i++)
            {
                if (IsImageMedia(baseTemplates[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}