using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VUI.VUI3.classes
{
    public class VUI3Screenshot
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Device { get; set; }
        public string PageType { get; set; }
        public string ImageURL_full { get; set; }
        public string ImageURL_lg { get; set; }
        public string ImageURL_md { get; set; }
        public string ImageURL_th { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateCreated { get; set; }
        public string DateCreatedStr { get { return DateCreated.ToString("MMM yyyy"); } }
        public bool IsFavourite { get; set; }
        public string ImportTag { get; set; }
        

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3Screenshot>(this);
        }
    }


    public class VUI3ScreenshotList
    {
        public int resultCount { get { return screenshots.Count(); } }
        public List<VUI3Screenshot> screenshots { get; set; }

        public VUI3ScreenshotList() { screenshots = new List<VUI3Screenshot>(); }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ScreenshotList>(this);
        }
    }

    public class VUI3ScreenshotResults
    {
        public VUI3ScreenshotList ScreenshotsList { get; set; }
        public int totalResults { get; set; }
        public int resultStart { get; set; }
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ScreenshotResults>(this);
        }
    }
}