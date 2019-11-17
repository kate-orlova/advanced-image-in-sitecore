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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdvancedImage.Extensions
{
    public static class HtmlHelperExtensions
    {
        private static readonly Regex SitecoreImgSrcRegex = new Regex(@"(src\=\""([^""]*)\"")", RegexOptions.Compiled);
        private static List<int> defaultSrcSetSizes = new List<int> {320, 360, 640, 720, 960, 1280, 1440};

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

        public static MvcHtmlString GetResizedSrcSet(
            this HtmlHelper helper,
            Image image,
            int[] sizes = null,
            bool useAspectRatio = true,
            Func<int, string> focalPointFunc = null,
            bool useFocalPointFunc = false)
        {
            if (image == null) return new MvcHtmlString(string.Empty);

            if (image.Width == 0 || image.Height == 0) useAspectRatio = false;

            var srcSetFormat = useAspectRatio ? "{0} {1}w {2}h" : "{0} {1}w";

            var sizesList = new List<int>(defaultSrcSetSizes);
            if (sizes != null) sizesList.AddRange(sizes);

            double aspectRatio = 0;
            if (useAspectRatio)
            {
                aspectRatio = (double) image.Width / image.Height;
            }

            focalPointFunc = useFocalPointFunc ? focalPointFunc : null;

            var srcSetList = new List<string>();
            foreach (var size in sizesList.OrderBy(x => x))
            {
                var resizedMediaUrl = GetResizedMediaUrl(helper, image.Src, size,
                    focalPointFunc != null ? focalPointFunc(size) : string.Empty);
                srcSetList.Add(
                    useAspectRatio
                        ? string.Format(srcSetFormat, resizedMediaUrl, size, (int) (size / aspectRatio))
                        : string.Format(srcSetFormat, resizedMediaUrl, size));
            }

            return new MvcHtmlString(string.Join(",", srcSetList));
        }
        private static HtmlString BuildNotEditableImageTag(
            HtmlHelper htmlHelper,
            Image imageField,
            object parameters = null,
            bool useAspectRatio = true,
            Func<int, string> focalPointFunc = null,
            bool useAdvancedImage = false)
        {
            var attributes = parameters.GetHtmlAttributeCollection(true).ToSafeDictionary();

            int[] sizes = null;
            var sizesStr = attributes["data-srcset"].OrDefault(attributes["sizes"]);
            if (sizesStr.HasValue())
            {
                sizes = sizesStr.ParseIntArray().ToArray();
            }

            attributes["class"] = attributes["class"].Append("lazyload", " ");
            attributes["data-srcset"] = htmlHelper.GetResizedSrcSet(imageField, sizes, useAspectRatio, focalPointFunc, useAdvancedImage).ToHtmlString();
            attributes["data-sizes"] = "auto";
            attributes["alt"] = attributes["alt"].HasValue() || imageField == null ? attributes["alt"] : imageField.Alt;

            return BuildImageTag(attributes, imageField);
        }
    }
}