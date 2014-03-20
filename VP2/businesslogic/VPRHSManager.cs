using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace VP2.businesslogic
{
    public class VPRHSManager
    {
        public Dictionary<string, string[]> RHSMap
        {  
            get
            {
                if(_map == null)
                {
                    PopulateRHSMap();
                }
                return _map;
            }
        }

        public string[] SectionsToShow
        {
            get
            {
                string[] result;
                if (!_map.TryGetValue(_template, out result))
                {
                    _map.TryGetValue(DEFAULT_KEY, out result);
                }
                return result;
            }
        }
        private Dictionary<string, string[]> _map;
        private string _template;

        public VPRHSManager(string templateName) 
        {
            _template = templateName;
            PopulateRHSMap();
        }

        private void PopulateRHSMap()
        {
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            string rootstr = "VP2014_RHS_TEMPLATE_";
            string[] keys = ConfigurationManager.AppSettings[String.Concat(rootstr,"KEYS")].Split(';');

            foreach (string key in keys)
            {
                string[] sections = ConfigurationManager.AppSettings[String.Concat(rootstr, key)].Split(';');
                result.Add(key, sections);
            }
            _map = result;
        }


        public const string DEFAULT_KEY = "default";
    }
}