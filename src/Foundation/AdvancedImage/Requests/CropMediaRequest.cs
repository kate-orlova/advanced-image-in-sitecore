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