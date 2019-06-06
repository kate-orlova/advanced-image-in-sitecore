using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;

namespace AdvancedImage.Fields
{
    public class Slider : Input, IContentField
    {
        public string Source { get; set; }
        protected bool IsDebug { get; private set; }

        public Slider()
        {
            Class = "scContentControl";
            Activation = true;
        }
        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            Value = value;
        }
    }
}