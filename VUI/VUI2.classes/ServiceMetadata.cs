using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using VUI.classes;

namespace VUI.VUI2.classes
{
    public class ServiceMetadata
    {
        private bool _urlIsSet = false;
        private string _url = String.Empty;

        public ServiceMetadata()
        {
            NumInstances = 1;
        }

        public int ParentPlatform { get; set; }
        public int ParentDevice { get; set; }
        public string ParentPlatformName { get; set; }
        public string ParentDeviceName { get; set; }
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public string Context { get; set; }
        public int NumInstances { get; set; }
        public int DefaultScreenshotId { get; set; }
        public string DefaultScreenshotURL
        {
            get
            {
                string defaultScreenshot = String.Empty;
                if (DefaultScreenshotId > 0)
                {
                    DynamicNode img = new DynamicNode(DefaultScreenshotId);
                    string thumbimage = img.GetProperty("thFile").Value;
                    defaultScreenshot = thumbimage.Replace("&", "%26");
                }
                if (!String.IsNullOrEmpty(defaultScreenshot))
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~","") + @"md/" + defaultScreenshot;
                }
                else
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~", "") + @"md/holding-screenshot.png";
                }
            }
        }
        public int DefaultServiceId { get; set; }
        public int ScreenshotCount { get; set; }
        public bool HasMultipleServices { get { return NumInstances > 1; } }
        public bool IsPreviewable { get; set; }
        public string ScreenshotURL
        {
            get
            {
                if (!IsPreviewable)
                {
                    return "#";
                }
                else
                {
                    if (!_urlIsSet) SetURL("screenshots");
                    return _url;
                }
            }
        }

        public List<Service> GetServices()
        {
            return null;
        }

        private void SetURL(string suffix)
        {
            if (HasMultipleServices)
            {
                // Then the url will look like
                // - /vui/all/[service]/screenshots
                // - Or /vui/Tablet/[service]/screenshots
                // - i.e. /vui/[context]/[service]/screenshots
                _url = String.Format(@"/vui/{0}/{1}/{2}", Context.ToLower(), ServiceName.ToLower(), suffix);
            }
            else
            {
                // There is only one URL, so dive straight in to service URL
                if (DefaultServiceId != null)
                {
                    DynamicNode node = new DynamicNode(DefaultServiceId);
                    _url = node.NiceUrl + "/" + suffix;
                }
                else
                {
                    _url = "#";
                }
            }
        }
    }
}