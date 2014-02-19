using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VUI.VUI3.classes
{
    public class VUI3ServiceRating
    {
        public DateTime DateRecorded { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public string Device { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal AverageUserRating { get; set; }
        public int UserRatingCount { get; set; }

        //Optional Fields
        public decimal AverageUserRatingForCurrentVersion { get; set; }
        public int UserRatingCountForCurrentVersion { get; set; }
        public string NumDownloads { get; set; }

    }

    public class VUI3ServiceRatingsList
    {
        public Dictionary<string, VUI3ServiceRating> Ratings { get; set; }

        public VUI3ServiceRatingsList()
        {
            Ratings = new Dictionary<string, VUI3ServiceRating>();
        }

        public VUI3ServiceRating Get(string device)
        {
            try
            {
                return Ratings[device];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Has(string device)
        {
            return Ratings.ContainsKey(device);
        }

    }
}