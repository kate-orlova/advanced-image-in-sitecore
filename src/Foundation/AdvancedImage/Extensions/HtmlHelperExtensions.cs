using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdvancedImage.Controllers;

namespace AdvancedImage.Extensions
{
    public static class HtmlHelperExtensions
    {
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
    }
}