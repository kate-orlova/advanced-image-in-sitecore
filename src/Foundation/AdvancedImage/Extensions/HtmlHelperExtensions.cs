using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdvancedImage.Controllers;
using Sitecore.Data.Managers;

namespace AdvancedImage.Extensions
{
    public static class HtmlHelperExtensions
    {
        private static readonly Regex SitecoreImgSrcRegex = new Regex(@"(src\=\""([^""]*)\"")", RegexOptions.Compiled);

        public static string GetRazorViewAsString(string viewPath, object model)
        {
            var st = new StringWriter();
            var httpContext = HttpContext.Current ?? new HttpContext(new HttpRequest(String.Empty, "http://dummy.com", String.Empty), new HttpResponse(new StringWriter()));
            var context = new HttpContextWrapper(httpContext);
            var routeData = new RouteData();
            routeData.Values["Controller"] = nameof(FakeController);
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var razor = new RazorView(controllerContext, viewPath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
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
    }
}