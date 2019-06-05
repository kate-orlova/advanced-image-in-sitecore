using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;

namespace AdvancedImage.Fields
{
    public class Slider : Input, IContentField
    {
        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}