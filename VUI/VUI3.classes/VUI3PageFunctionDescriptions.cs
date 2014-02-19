using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VUI.VUI2.classes;
using umbraco.MacroEngines;
using System.Collections.Specialized;

namespace VUI.VUI3.classes
{
    public class VUI3PageFunctionDescriptions
    {
        private int rootid;

        private NameValueCollection descriptions;

        /// <summary>
        /// Initialise a list of descriptions of the page types
        /// </summary>
        public VUI3PageFunctionDescriptions()
        {
            descriptions = new NameValueCollection();
            if (Int32.TryParse(Utility.GetConst("VUI3_PageFunctionDescriptions"), out rootid))
            {
                DynamicNode functionsRoot = new DynamicNode(rootid);
                foreach (DynamicNode functionDesc in functionsRoot.Descendants())
                {
                    descriptions.Add(functionDesc.GetProperty("pageType").Value, functionDesc.GetProperty("description").Value);
                }
            }
        }

        /// <summary>
        /// Get a Page Type Description (HTML) for the pageType specified
        /// </summary>
        /// <param name="pageType">pageType</param>
        /// <returns></returns>
        public string GetDescription(string pageType)
        {
            return descriptions[pageType];
        }

    }

}