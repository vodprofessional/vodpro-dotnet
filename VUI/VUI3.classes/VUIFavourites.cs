using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VUI.VUI3.classes
{
    public class VUIFavourites
    {
    }

    public class ServiceFavourite
    {
        public string id { get; set; }
        public string serviceName { get; set; }

        public ServiceFavourite() { ; }
        public ServiceFavourite(VUI3ServiceMaster sm)
        {
            this.id = sm.Id.ToString();
            this.serviceName = sm.ServiceName;
        }
        
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<ServiceFavourite>(this);
        }

    }

    public class ServiceFavourites
    {
        public List<ServiceFavourite> serviceFavourites { get; set; }

        public ServiceFavourites() { serviceFavourites = new List<ServiceFavourite>(); }
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<ServiceFavourites>(this);
        }
    }


    public class ImageFavourites
    {
        public List<ImageFavouriteCollection> collections { get; set; }

        public ImageFavourites() { collections = new List<ImageFavouriteCollection>(); }

        public List<VUI3Screenshot> AllFavouriteImages()
        {
            List<VUI3Screenshot> screens = new List<VUI3Screenshot>();
            foreach(ImageFavouriteCollection c in collections)
            {
                screens.AddRange(c.imageFavourites);
            }
            return screens;
        }
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<ImageFavourites>(this);
        }
    }

    public class ImageFavouriteCollection
    {
        public string collectionName { get; set; }
        public List<VUI3Screenshot> imageFavourites { get; set; }

        public ImageFavouriteCollection() { imageFavourites = new List<VUI3Screenshot>(); }
        public ImageFavouriteCollection(string collectionName) { this.collectionName = collectionName; imageFavourites = new List<VUI3Screenshot>(); }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<ImageFavouriteCollection>(this);
        }

        [JsonIgnore]
        public const string DEFAULT_COLLECTION = "Default";
    }


}