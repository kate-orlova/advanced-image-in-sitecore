using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace AdvancedImage.Requests
{
    public class CropMediaRequest : MediaRequest
    {
        private HttpRequest innerRequest;
        private MediaUrlOptions mediaQueryString;
        private MediaUri mediaUri;
        private MediaOptions options;

        protected override MediaOptions GetOptions()
        {
            var queryString = this.InnerRequest.QueryString;
            if (queryString != null && !string.IsNullOrEmpty(queryString.Get("cx")))
            {
                options = new MediaOptions();
                ProcessCustomParameters(options);
            }
            else
            {
                MediaUrlOptions mediaQueryString = GetMediaQueryString();
                options = new MediaOptions
                {
                    AllowStretch = mediaQueryString.AllowStretch,
                    BackgroundColor = mediaQueryString.BackgroundColor,
                    IgnoreAspectRatio = mediaQueryString.IgnoreAspectRatio,
                    Scale = mediaQueryString.Scale,
                    Width = mediaQueryString.Width,
                    Height = mediaQueryString.Height,
                    MaxWidth = mediaQueryString.MaxWidth,
                    MaxHeight = mediaQueryString.MaxHeight,
                    Thumbnail = mediaQueryString.Thumbnail
                };
            }

            return options;
        }

        public override MediaRequest Clone()
        {
            Assert.IsTrue((bool)(base.GetType() == typeof(CropMediaRequest)), "The Clone() method must be overridden to support prototyping.");
            return new CropMediaRequest
            {
                innerRequest = this.innerRequest,
                mediaUri = this.mediaUri,
                options = this.options,
                mediaQueryString = this.mediaQueryString
            };
        }
    }
}