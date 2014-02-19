using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;


namespace VUI.VUI2.classes
{
    public static class Utility
    {
        private static NameValueCollection _ConfigConstants;

        static Utility()
        {
            _ConfigConstants = new NameValueCollection();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                _ConfigConstants.Add(key, ConfigurationManager.AppSettings[key].ToString());
            }
        }

        public static string GetConst(string key)
        {
            return _ConfigConstants[key];
        }

        /// <summary>
        /// Returns the last part of the URL
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string NiceUrlName(int nodeid)
        {
            string niceurl = umbraco.library.NiceUrl(nodeid);
            // Tidy leading and trailing '/'
            if (niceurl.IndexOf('/') == 0)
            {
                niceurl = niceurl.Substring(1);
            }
            if (niceurl.LastIndexOf('/') == niceurl.Length - 1)
            {
                niceurl = niceurl.Substring(0, niceurl.Length - 1);
            }
            string[] urlParts = niceurl.Split('/');
            return urlParts[urlParts.Length - 1];
        }

        public static string NiceUrlName(string servicename)
        {
            return Regex.Replace(Regex.Replace(servicename,@"\s","-"), @"[^\w^\-]", "").ToLower();
        }
    }
}