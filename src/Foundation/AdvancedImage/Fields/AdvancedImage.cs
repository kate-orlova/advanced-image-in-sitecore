using System.Web;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
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
        }
        protected void Update(bool showCropper = true)
        {
        }
    }
}