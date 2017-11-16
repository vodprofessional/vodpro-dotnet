using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;

namespace VUI.VUI3.classes
{
    public class VUIVideo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string YouTubeShortCode { get; set; }
        public DateTime PublishDate { get; set; }
        public string[] Tags { get; set; }
        public int[] ServiceIDs { get; set; }
        public string Url { get; set; }

        public VUIVideo()
        {

        }



        public static List<VUIVideo> AllVideos()
        {
            List<VUIVideo> all = new List<VUIVideo>();

            DynamicNode videoRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI_VideoRootFolderID"]);

            foreach (DynamicNode n in videoRoot.Descendants("VUI3Video"))
            {
                string tags = n.GetProperty("tags").Value;
                string services = n.GetProperty("services").Value;

                VUIVideo v = new VUIVideo()
                {
                    Id = n.Id,
                    Title = n.GetProperty("title").Value,
                    Description = n.GetProperty("description").Value,
                    PublishDate = DateTime.Parse(n.GetProperty("publishDate").Value),
                    YouTubeShortCode = n.GetProperty("youTubeShortCode").Value,
                    Url = n.Url
                };
                v.Tags = string.IsNullOrEmpty(tags) ? null : tags.Split(',');
                v.ServiceIDs = string.IsNullOrEmpty(services) ? null : services.Split(',').Select(s => int.Parse(s)).ToArray();
                all.Add(v);
            }

            return all;
        }

        public static List<VUIVideo> AllVideosForService(int serviceId)
        {
            List<VUIVideo> videos = new List<VUIVideo>();

            if (AllVideos().Count(v => v.ServiceIDs != null && v.ServiceIDs.Length > 0 && v.ServiceIDs.Contains(serviceId)) > 0)
            {
                videos = AllVideos().Where(v => v.ServiceIDs != null && v.ServiceIDs.Length > 0 && v.ServiceIDs.Contains(serviceId)).OrderByDescending(v => v.PublishDate).ToList();
            }
            return videos;
        }
    }

}