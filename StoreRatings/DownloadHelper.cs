using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace StoreRatings
{
    public class DownloadHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DownloadHelper));

        public static T DownloadSerialiseJson<T>(string url) where T : new()
        {
            log.Debug("JSON Get of [" + url + "]");
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message);
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }


        public static T DownloadSerialiseJson<T>(string url, string replace, string replacement) where T : new()
        {
            log.Debug("JSON Get of [" + url + "]");
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    json_data = json_data.Replace(replace, replacement);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }


        public static WindowsMarketInfo DownloadWindowsMarketPage(string url)
        {
            
            log.Debug("Get of [" + url + "]");
            WindowsMarketInfo p = null;

            using (var w = new WebClient())
            {
                string page = String.Empty;
                try
                {
                    //page = w.DownloadString(url);
                    //page = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>";
                    
                    byte[] pg = w.DownloadData(url);
                    

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(new MemoryStream(pg),Encoding.UTF8);

                    p = new WindowsMarketInfo();

                    p.description = doc.DocumentNode.SelectSingleNode(@"//pre[@itemprop='description']").InnerText;
                    p.logoURL = doc.DocumentNode.SelectSingleNode(@"//img[@itemprop='image']").GetAttributeValue("src", "");
                    string rating = doc.DocumentNode.SelectSingleNode(@"//meta[@itemprop='ratingValue']").GetAttributeValue("content", "0");
                    // rating gives us a percentage.
                    rating = Regex.Replace(rating.Replace("width:", "").Replace("%", ""), @"\s", "");
                    p.rating = rating.Substring(0, 6);
                    string numberOfRatings = doc.DocumentNode.SelectSingleNode(@"//meta[@itemprop='ratingCount']").GetAttributeValue("content", "0");
                    int num = 0;
                    numberOfRatings = Regex.Replace(numberOfRatings, @"[\(\)\s,]", "");
                    Int32.TryParse(numberOfRatings, out num);
                    p.numberOfRatings = num;
                    p.lastUpdated = doc.DocumentNode.SelectSingleNode(@"//meta[@itemprop='datePublished']").GetAttributeValue("content", "0");
                    p.currentVersion = doc.DocumentNode.SelectSingleNode(@"//span[@itemprop='softwareVersion']").InnerText;
                }
                catch (Exception e)
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + "Error getting Play Store detail from [" + url + "]");
                    log.Info(e.Message);
                    RatingsManager.AddErrorToLog(lc, "Error getting Play Store detail from [" + url + "]", e);
                }
            }
            return p;
        }


        public static PlayStoreInfo DownloadPlayStorePage(string url)
        {

            Uri u = new Uri(url);
            string id = System.Web.HttpUtility.ParseQueryString(u.Query).Get("id");

            log.Debug("Get of [" + url + "]");
            PlayStoreInfo p = null;

            using (var w = new WebClient())
            {
                string page = String.Empty;

                try
                {
                    //page = w.DownloadString(url);
                    //page = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>";
                    
                    byte[] pg = w.DownloadData(url);
                    

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(new MemoryStream(pg),Encoding.UTF8);
                    // doc.Load(new MemoryStream(pg));

                    p = new PlayStoreInfo();
                    p.rating = "0";
                    p.numberOfRatings = 0;

                    p.description = doc.DocumentNode.SelectSingleNode(@"//div[@itemprop='description']/div").InnerHtml;
                    p.logoURL = doc.DocumentNode.SelectSingleNode(@"//img[@itemprop='image']").GetAttributeValue("src", "");
                    string rating = doc.DocumentNode.SelectSingleNode(@"//div[@class='current-rating']").GetAttributeValue("style", "width: 0");
                    // rating gives us a percentage.
                    rating = Regex.Replace(rating.Replace("width:","").Replace(";","").Replace("%",""),@"\s","");
                    if(rating.Length >= 6)
                        p.rating = rating.Substring(0,6);
                    else
                        p.rating = rating.Substring(0, rating.Length);
                    string numberOfRatings = doc.DocumentNode.SelectSingleNode(@"//div[@class='stars-count']").InnerText;
                    int num = 0;
                    numberOfRatings = Regex.Replace(numberOfRatings, @"[\(\)\s,]", "");
                    Int32.TryParse(numberOfRatings, out num);
                    p.numberOfRatings = num;
                    p.lastUpdated = doc.DocumentNode.SelectSingleNode(@"//div[@itemprop='datePublished']").InnerText;

                    DateTime pd; 
                    if(!DateTime.TryParse(p.lastUpdated, out pd)){ pd = DateTime.Now; }
                    p.lastUpdated = pd.ToString("yyyy-MM-dd");

                    p.currentVersion = doc.DocumentNode.SelectSingleNode(@"//div[@itemprop='softwareVersion']").InnerText;
                    p.numDownloads = doc.DocumentNode.SelectSingleNode(@"//div[@itemprop='numDownloads']").InnerText;


                    for (int pageNum = 0; pageNum < 5; pageNum++)
                    {
                        Thread.Sleep(5000);
                        string comments = PlayStoreComments.GetComments(id, pageNum);

                        HtmlDocument cDoc = new HtmlDocument();
                        cDoc.LoadHtml(comments);

                        HtmlNodeCollection reviews = cDoc.DocumentNode.SelectNodes(@"//div[@class='single-review']");

                        for (int i = 0; i < reviews.Count; i++)
                        {

                            log.Debug("Getting comments");
                            try
                            {
                                HtmlNode review = reviews[i].CloneNode(true);
                                HtmlDocument rdoc = new HtmlDocument();
                                rdoc.LoadHtml(review.InnerHtml);
                                PlayStoreComment c = new PlayStoreComment();

                                c.Id = rdoc.DocumentNode.SelectSingleNode(@"div[@class='review-header']").GetAttributeValue("data-reviewid", String.Empty);

                                string reviewBody = rdoc.DocumentNode.SelectSingleNode(@"div[@class='review-body']").InnerHtml;
                                if (!String.IsNullOrEmpty(reviewBody))
                                {
                                    Regex re = new Regex(@"<span(.*?)>(.*?)</span>(.*?)$");
                                    Match m = re.Match(reviewBody);
                                    c.Title = m.Groups[2].Value;
                                    c.Comment = m.Groups[3].Value;
                                }

                                c.ReviewDate = rdoc.DocumentNode.SelectSingleNode(@"//span[@class='review-date']").InnerText;
                                string commentRating = rdoc.DocumentNode.SelectSingleNode(@"//div[@class='current-rating']").GetAttributeValue("style", "width: 0");
                                // rating gives us a percentage.
                                if (!String.IsNullOrEmpty(commentRating))
                                {
                                    commentRating = Regex.Replace(commentRating.Replace("width:", "").Replace("%", ""), @"\s", "").Replace(";", "");
                                    if (commentRating.Length > 6)
                                        commentRating = commentRating.Substring(0, 6);
                                    c.Rating = commentRating;
                                }
                                else
                                {
                                    c.Rating = "";
                                }

                                p.Comments.Add(c);
                            }
                            catch (Exception ex)
                            {
                                int lc = RatingsManager.LogCount;
                                log.Info(lc + "Error getting Play comments from [" + url + "]");
                                log.Info(ex.Message);
                                RatingsManager.AddErrorToLog(lc, "Error getting Play Store comments from [" + url + "]", ex);
                            }

                        }
                    }


                }
                catch(Exception e) 
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + "Error getting Play Store detail from [" + url + "]");
                    log.Info(e.Message);
                    RatingsManager.AddErrorToLog(lc, "Error getting Play Store detail from [" + url + "]",e);
                }
            }
            return p;
        }
    }
}
