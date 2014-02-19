using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VUI.VUI3.classes
{
    public class VUI3ServiceSnapshot
    {
        public int id { get; set; }
        public string serviceName { get; set; }
        public DateTime snapshotDate { get; set; }
        public int numScreenshots { get; set; }
        public int numScreenshotDevices { get; set; }
        public int benchmarkAverage { get; set; }
        public int benchmarkDevices { get; set; }
        public decimal ratingAverage { get; set; }
        public int twitterFollowers { get; set; }
        public int facebookLikes { get; set; }
        public string niceDate 
        { 
            get {
                string snapshotTaken = String.Empty;
                if (snapshotDate.ToString("yyyyMMdd").Equals(DateTime.Today.ToString("yyyyMMdd")))
                {
                    snapshotTaken = "today";
                }
                else if (snapshotDate.ToString("yyyyMMdd").Equals(DateTime.Today.AddDays(-1).ToString("yyyyMMdd")))
                {
                    snapshotTaken = "yesterday";
                }
                else
                {
                    snapshotTaken = snapshotDate.ToString("dd MMM yyyy");
                }

                return snapshotTaken;
            }
        }

        /*
        public bool hasPrevious { get; set; }
        public DateTime? previousDate { get; set; }
        public decimal previousRatingAverage { get; set; }
        public int previousTwitterFollowers { get; set; }
        public int previousFacebookLikes { get; set; }
        */

        [JsonIgnore]
        public int ytVideoCount { get; set; }
        public int ytSubscriberCount { get; set; }

        public string Url { get { return umbraco.library.NiceUrl(id).Replace("/vui/", "/vui3/"); } }

        public bool HasScreenshots()
        {
            return (numScreenshots > 0);
        }
        public bool HasBenchmarks()
        {
            return (benchmarkDevices > 0);
        }
        public bool HasRatings()
        {
            return (ratingAverage > 0);
        }
        public bool HasFacebook()
        {
            return (facebookLikes > 0);
        }
        public bool HasTwitter()
        {
            return (twitterFollowers > 0);
        }
        public bool HasYoutube()
        {
            return (ytSubscriberCount > 0);
        }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ServiceSnapshot>(this);
        }
    }

    public class VUI3ServiceSnapshotList
    {
        public int resultCount { get { return results.Count; } }
        public bool isRetrospective { get; set; }
        public DateTime retrospectiveDate { get; set; }
        public List<VUI3ServiceSnapshot> results { get; set; }

        public VUI3ServiceSnapshotList() 
        { 
            results = new List<VUI3ServiceSnapshot>(); 
            isRetrospective = false; 
        }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ServiceSnapshotList>(this);
        }

    }
}