using System.Web;
using Sitecore.Resources.Media;

namespace AdvancedImage.Requests
{
    public class CropMediaRequest : MediaRequest
    {
        private HttpRequest innerRequest;
        private MediaUrlOptions mediaQueryString;
    }
}