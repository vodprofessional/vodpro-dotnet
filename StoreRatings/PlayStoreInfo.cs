using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoreRatings
{
    public class PlayStoreInfo
    {
        public PlayStoreInfo() { Comments = new List<PlayStoreComment>(); }
        public string rating { get; set; }
        public string logoURL { get; set; }
        public int numberOfRatings { get; set; }
        public string lastUpdated { get; set; }
        public string currentVersion { get; set; }
        public string description { get; set; }
        public string numDownloads { get; set; }
        public List<PlayStoreComment> Comments { get; set; }
    }


    public class PlayStoreComment
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ReviewDate { get; set; }
        public string Comment { get; set; }
        public string Version { get; set; }
        public string Rating { get; set; }

    }

}
