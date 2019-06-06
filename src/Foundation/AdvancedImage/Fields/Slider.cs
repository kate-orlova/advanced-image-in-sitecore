﻿using System;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace AdvancedImage.Fields
{
    public class Slider : Input, IContentField
    {
        private const string IS_DEBUG_FIELD_NAME = "IsDebug";
        private const string FROM_FIELD_NAME = "From";
        private const string TO_FIELD_NAME = "To";
        private const string ASSETS_FOLDER_NAME = "Fields\\Slider\\";
        public string Source { get; set; }
        protected bool IsDebug { get; private set; }
        protected int From { get; private set; }
        protected int To { get; private set; }

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
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ServerProperties["Value"] = base.ServerProperties["Value"];
        }
        protected override void SetModified()
        {
            base.SetModified();
            if (base.TrackModified)
            {
                Sitecore.Context.ClientPage.Modified = true;
            }
        }
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            base.HandleMessage(message);
        }
    }
}