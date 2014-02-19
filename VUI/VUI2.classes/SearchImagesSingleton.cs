using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VUI.VUI2.classes
{
    public class SearchImagesSingleton
    {
        private static SearchImagesSingleton m_sis = null;

        public static SearchImagesSingleton Instance() 
        {
            if (m_sis == null) { m_sis = new SearchImagesSingleton(); Regenerate(); }
            return m_sis;
        }

        public static void Regenerate()
        {
            m_sis.images = VUIDataFunctions.GetSearchImages();
        }

        private SearchImagesSingleton() { }
        private List<SearchImage> images;


        public List<SearchImage> FindImages(string service, string pagetype)
        {
            List<SearchImage> tmp = new List<SearchImage>();
            if (!String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(pagetype))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            if (!String.IsNullOrEmpty(service))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).ToList();
            }
            if (!String.IsNullOrEmpty(pagetype))
            {
               tmp =  images.Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            return tmp;
        }

        public List<SearchImage> FindImages(string service, string device, string pagetype)
        {
            List<SearchImage> tmp = new List<SearchImage>();

            // Service + Device + PageType
            if (!String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(device) && !String.IsNullOrEmpty(pagetype))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).Where(i => i.Device.Equals(device)).Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            // Service + PageType
            if (!String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(pagetype))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            // Service + Device
            if (!String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(device))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).Where(i => i.Device.Equals(device)).ToList();
            }
            //Device + PageType
            if (!String.IsNullOrEmpty(device) && !String.IsNullOrEmpty(pagetype))
            {
                tmp = images.Where(i => i.Device.Equals(device)).Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            // Service
            if (!String.IsNullOrEmpty(service))
            {
                tmp = images.Where(i => i.ServiceName.Equals(service)).ToList();
            }
            // Device
            if (!String.IsNullOrEmpty(device))
            {
                tmp = images.Where(i => i.Device.Equals(device)).ToList();
            }
            // Page Type
            if (!String.IsNullOrEmpty(pagetype))
            {
                tmp = images.Where(i => i.PageType.Equals(pagetype)).ToList();
            }
            return tmp;
        }

        public string FindImagesJSON(string service, string pagetype)
        {
            // THis should maybe be in VUIFunctions to include islogged in  stuff...
            List<SearchImage> tmp = FindImages( service,  pagetype);
            return JsonConvert.SerializeObject(tmp);
        }

    }
}