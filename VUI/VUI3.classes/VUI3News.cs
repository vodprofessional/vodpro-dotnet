using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI3.classes
{
    public class VUI3News
    {
        public static List<NewsItem> GetNewsByStatus(int numItems, string status)
        {
            List<NewsItem> news = new List<NewsItem>();
            string sql;
            if (status.Equals("all"))
            {
                sql = String.Format(@"SELECT TOP {0} 
                            [ID],[DateCreated],[NewsType],[RelatedServiceId],[RelatedService],[RelatedPlatform],[RelatedDevice],[ScreenshotCount],[VersionNumber],[AppStore],[Description],IsLive, IsTweeted 
                            FROM [dbo].[vui_News] order by datecreated desc", numItems, status);
            }
            else
            {
                sql = String.Format(@"SELECT TOP {0} 
                            [ID],[DateCreated],[NewsType],[RelatedServiceId],[RelatedService],[RelatedPlatform],[RelatedDevice],[ScreenshotCount],[VersionNumber],[AppStore],[Description],IsLive, IsTweeted 
                            FROM [dbo].[vui_News] where IsLIve='{1}' order by datecreated desc", numItems, status);
            }
            
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    NewsItem n = new NewsItem();
                    n.ID = (Int32)(sr[0]);
                    n.DateCreated = sr.IsDBNull(1) ? DateTime.Now : (DateTime)(sr[1]);
                    n.NewsType = sr.IsDBNull(2) ? String.Empty : (string)(sr[2]);
                    n.RelatedServiceId = sr.IsDBNull(3) ? -1 : (Int32)(sr[3]);
                    n.RelatedService = sr.IsDBNull(4) ? String.Empty : (string)(sr[4]);
                    n.RelatedPlatform = sr.IsDBNull(5) ? String.Empty : (string)(sr[5]);
                    n.RelatedDevice = sr.IsDBNull(6) ? String.Empty : (string)(sr[6]);
                    n.ScreenshotCount = sr.IsDBNull(7) ? -1 : (Int32)(sr[7]);
                    n.VersionNumber = sr.IsDBNull(8) ? String.Empty : (string)(sr[8]);
                    n.AppStore = sr.IsDBNull(9) ? String.Empty : (string)(sr[9]);
                    n.Description = sr.IsDBNull(10) ? String.Empty : (string)(sr[10]);
                    n.IsLive = sr.IsDBNull(11) ? false : ((string)sr["IsLive"] == "Y" ? true : false);
                    n.IsTweeted = sr.IsDBNull(12) ? false : ((string)sr["IsTweeted"] == "Y" ? true : false);
                    news.Add(n);
                }
                conn.Close();
            }
            return news;
        }


        /// <summary>
        /// Return a list of news items
        /// </summary>
        /// <param name="numItems"></param>
        /// <returns></returns>
        public static List<NewsItem> GetNews(int numItems)
        {
            return GetNewsByStatus(numItems, "Y");
        }

        /// <summary>
        /// Return a JSON object of NewsItems
        /// </summary>
        /// <param name="numItems"></param>
        /// <returns></returns>
        public static string GetNewsAsJson(int numItems)
        {
            return JsonConvert.SerializeObject(GetNews(numItems));
        }


        /// <summary>
        /// Tweet News Item
        /// </summary>
        /// <param name="newsType"></param>
        /// <param name="relatedServiceId"></param>
        /// <param name="relatedService"></param>
        /// <param name="relatedPlatform"></param>
        /// <param name="relatedDevice"></param>
        /// <param name="ScreenshotCount"></param>
        /// <param name="version"></param>
        /// <param name="appStore"></param>
        /// <param name="description"></param>
        public static void TweetNews(string status)
        {
            TwitterHelper.Tweet(status);
        }

        public static void TweetNews(int id)
        {
            string sql = String.Format(@" update vui_News set IsTweeted='Y' where id={0}; select TwitterStatus from vui_news where id={0};", id);
            string twitterDesc;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                sr.Read();
                twitterDesc = (string)sr["TwitterStatus"];
                conn.Close();
            }
            TwitterHelper.Tweet(twitterDesc);
        }


        public static void AddNews(string newsType, int relatedServiceId = -1, string relatedService = "", string relatedPlatform = "", string relatedDevice = "", int ScreenshotCount = -1, string version = "", string appStore = "", string description = "", bool directToLive = true, bool tweetNews = true)
        {
            string sql = @" insert into vui_News (
                                [DateCreated], [NewsType] ,[RelatedServiceId] ,[RelatedService] ,[RelatedPlatform] ,[RelatedDevice] ,[ScreenshotCount] ,[VersionNumber] ,[AppStore]  ,[Description]
                                , TwitterStatus, [IsLive], [IsTweeted]
                            )
                            values ( GetDate(), '{0}'     , {1}               , '{2}'           , '{3}'            ,'{4}'           ,'{5}'             ,'{6}'           ,'{7}'       ,'{8}', '{9}', '{10}', '{11}');
                            select convert(int, SCOPE_IDENTITY());
                            ";
            
            string twitterDescription = description;
            /* Not sure if we need this yet */
            if (String.IsNullOrEmpty(description))
            {
                string pd = relatedPlatform;
                if (!String.IsNullOrEmpty(relatedDevice))
                {
                    pd += " / " + relatedDevice;
                }

                if(newsType.Equals(NEWSTYPE_SYSTEM))
                {
                    description = "";
                    twitterDescription = description;
                }
                else if (newsType.Equals(NEWSTYPE_VERSION))
                {
                    description = String.Format(@"{0} released version {1} for {2}", relatedService, version, pd);
                    twitterDescription = description + " [subscriber content] http://www.vodprofessional.com/vui";
                }
                else if (newsType.Equals(NEWSTYPE_BENCHMARK))
                {
                    description = String.Format("New benchmark score for {0} on {1}", relatedService, pd);
                    twitterDescription = description + " [subscriber content] http://www.vodprofessional.com/vui";
                }
                else if (newsType.Equals(NEWSTYPE_SCREENSHOT))
                {
                    description = String.Format("{0} new screenshots of {1} on {2}", ScreenshotCount, relatedService, pd);
                    twitterDescription = description + " [subscriber content] http://www.vodprofessional.com/vui";
                }
                else if (newsType.Equals(NEWSTYPE_NEW))
                {
                    description = String.Format("New service {0} added to the library", relatedService);
                    twitterDescription = description + " [subscriber content] http://www.vodprofessional.com/vui";
                }
            }
            string isLive = "N", isTweeted = "N";
            if (directToLive)
            {
                isLive = "Y";
            }
            

            sql = String.Format(sql, new object[] { newsType, relatedServiceId, relatedService, relatedPlatform,  relatedDevice ,  ScreenshotCount,  version,  appStore,  description, twitterDescription, isLive, isTweeted });

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                int id = (int)comm.ExecuteScalar();
                conn.Close();

                if (tweetNews)
                {
                    VUI3News.TweetNews(id);
                }
            }
        }

        /// <summary>
        /// Change to Live or pull from site
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">Y or N</param>
        public static void ChangeStatus(int id, string status)
        {
            string sql = String.Format(@" update vui_News set IsLive='{0}' where id={1}", status, id);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Completely remove News Item
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteNews(int id)
        {
            string sql = String.Format(@" delete vui_News  where id={0}", id);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }





        public const string NEWSTYPE_SYSTEM = "system";
        public const string NEWSTYPE_VERSION = "version";
        public const string NEWSTYPE_BENCHMARK = "bench";
        public const string NEWSTYPE_SCREENSHOT = "screen";
        public const string NEWSTYPE_NEW = "new";
        
        public const string NEWSSTORE_GOOGLE = "google";
        public const string NEWSSTORE_APPLE = "apple";
        public const string NEWSSTORE_WINDOWS = "windows";

        public Dictionary<string,string> NewsStoreMapping = new Dictionary<string,string>()  {
            {"google","Google Play"},
            {"apple","Apple AppStore"},
            {"windows","Windows App Market"}
        };
    }

    public class NewsItem
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string NewsType { get; set; }
        public int RelatedServiceId { get; set; }
        public string RelatedService { get; set; }
        public string RelatedPlatform { get; set; }
        public string RelatedDevice { get; set; }
        public int ScreenshotCount { get; set; }
        public string VersionNumber { get; set; }
        public string AppStore { get; set; }
        public string Description { get; set; }
        public string TwitterStatus { get; set; }
        public bool IsTweeted { get; set; }
        public bool IsLive { get; set; }
        public string Url
        {
            get
            {
                if (RelatedServiceId > 0)
                {
                    string l = umbraco.library.NiceUrl(RelatedServiceId).Replace("/vui/", "/vui3/");
                    if(NewsType.Equals(VUI3News.NEWSTYPE_VERSION))
                    {
                        l += "#slide-ratings";
                    }
                    if(NewsType.Equals(VUI3News.NEWSTYPE_SCREENSHOT))
                    {
                        l += "#slide-screenshots";
                    }
                    if(NewsType.Equals(VUI3News.NEWSTYPE_BENCHMARK))
                    {
                        l += "#slide-benchmarking";
                    }
                    return l;
                }
                else
                    return String.Empty;
            }
        }
    }
}