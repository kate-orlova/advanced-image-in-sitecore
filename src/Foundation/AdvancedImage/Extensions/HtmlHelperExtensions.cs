using AdvancedImage.Controllers;
using AdvancedImage.GlassMapper.Fields;
using Glass.Mapper;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Collections;
using Sitecore.Data.Managers;
using Sitecore.Resources.Media;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdvancedImage.Extensions
{
    public static class HtmlHelperExtensions
    {
        private static readonly Regex SitecoreImgSrcRegex = new Regex(@"(src\=\""([^""]*)\"")", RegexOptions.Compiled);

        public static string GetRazorViewAsString(string viewPath, object model)
        {
            var st = new StringWriter();
            var httpContext = HttpContext.Current ??
                              new HttpContext(new HttpRequest(String.Empty, "http://dummy.com", String.Empty),
                                  new HttpResponse(new StringWriter()));
            var context = new HttpContextWrapper(httpContext);
            var routeData = new RouteData();
            routeData.Values["Controller"] = nameof(FakeController);
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var razor = new RazorView(controllerContext, viewPath, null, false, null);
            razor.Render(
                new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st),
                st);
            return st.ToString();
        }

        public static HtmlString GetSitecoreIconUrl(this HtmlHelper helper, string iconPath)
        {
            return helper.GetSitecoreIconUrl(iconPath, 16);
        }

        public static HtmlString GetSitecoreIconUrl(this HtmlHelper helper, string iconPath, int size)
        {
            var imageTag = ThemeManager.GetImage(iconPath, size, size);
            var match = SitecoreImgSrcRegex.Match(imageTag);
            if (match.Success)
            {
                return new HtmlString(match.Groups[2].Value);
            }

            return new HtmlString(string.Empty);
        }

        public static HtmlString RenderChecked(this HtmlHelper helper, bool value)
        {
            return new HtmlString(value ? "checked" : string.Empty);
        }

        private static HtmlString BuildImageTag(SafeDictionary<string> attributes, Image image)
        {
            var builder = new TagBuilder("img");
            foreach (var keyValuePair in attributes)
            {
                builder.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
            }

            var tagResult = new HtmlString(builder.ToString(TagRenderMode.SelfClosing) +
                                           image.ToSchemaOrgJsonString<Image,
                                               MXTires.Microdata.CreativeWorks.ImageObject>());

            return tagResult;
        }

        private static HtmlString BuildEditableImageTag<TModel>(
            GlassHtmlMvc<TModel> glassView,
            object item,
            bool isAdvancedImage,
            AdvancedImageField advancedImageField,
            bool useAdvancedImage,
            object parameters = null,
            bool outputHeightWidth = false,
            float cropFactor = 0)
        {
            if (!isAdvancedImage)
                return glassView.RenderImage(item, itemField => itemField, parameters, true, outputHeightWidth);

            var attributes = new SafeDictionary<string>();
            var src = advancedImageField.Src;
            if (useAdvancedImage)
            {
                src += $"?{advancedImageField.GetFocalPointParameters(advancedImageField.Width, cropFactor)}";
            }

            var protectedUrl = HashingUtils.ProtectAssetUrl(src);
            attributes.Add("src", protectedUrl);
            return BuildImageTag(attributes, advancedImageField);
        }

        public static string GetResizedMediaUrl(this HtmlHelper helper, string url, int maxWidth,
            string focalPointSettings = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var separator = url.Contains("?") ? "&" : "?";
            var mediaUrl = focalPointSettings.HasValue()
                ? $"{url}{separator}{focalPointSettings}"
                : $"{url}{separator}mw={maxWidth}";
            var finalUrl = HashingUtils.ProtectAssetUrl(mediaUrl);
            return finalUrl;
        }
    }
}