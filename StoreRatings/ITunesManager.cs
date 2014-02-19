using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace StoreRatings
{
    public class ITunesManager
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ITunesManager));
        private static bool bLookForOldComments = false;

        public static void GetITunesRatings(string colName, string deviceType, string IngestTag, bool skipcomments, List<string> serviceNames)
        {
            // for each item in a list, get the JSON, extract the relevant data and save to database

            string iTunesRootURL = "https://itunes.apple.com/lookup?id=";
            string iTunesCommentsURL = "http://itunes.apple.com{1}/rss/customerreviews/id={0}/json";

            if (ConfigurationManager.AppSettings["RatingsLookForMoreComments"].Equals("yes"))
            {
                bLookForOldComments = true;
            }


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                try
                {
                    log.Debug(RatingsManager.LogCount + " Starting iTunes Ingest");

                    Dictionary<int, string> services = new Dictionary<int, string>();
                    Dictionary<int, string> serviceComments = new Dictionary<int, string>();

                    string sql = @"select S.ID, [" + colName + @"]
                                   from   vui_ServiceMasters S 
                                   where  [" + colName + @"] is not null and [" + colName + @"] != '' ";

                    if (serviceNames.Count > 0)
                    {
                        sql += " AND ServiceName IN ( '" + string.Join("','", serviceNames) + "' )";
                    }

                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader sr = comm.ExecuteReader();
                    while (sr.Read())
                    {
                        int id = (Int32)sr[0];
                        string inurl = (string)sr[1];

                        Regex re1 = new Regex(@"https://itunes.apple.com/(\w+)/(.*?)id(\d+)(.*)");


                        // Process the URL to retrieve the ID and append to the https://itunes.apple.com/lookup?id=
                        // https://itunes.apple.com/us/app/cnn-app-for-ipad/id407824176?mt=8

                        //Regex re = new Regex(@"(.*?)id(\d+)(.*)");
                        Match m = re1.Match(inurl);
                        string country = m.Groups[1].Value;
                        string itunesid = m.Groups[3].Value;

                        string url = String.Empty;
                        string commenturl = String.Empty;

                        if (country.Equals("app"))
                        {
                            url = iTunesRootURL + itunesid;
                            commenturl = String.Format(iTunesCommentsURL, itunesid, "");
                        }
                        else
                        {
                            url = iTunesRootURL + itunesid + "&country=" + country;
                            commenturl = String.Format(iTunesCommentsURL, itunesid, "/" + country);
                        }

                        services.Add(id, url);
                        serviceComments.Add(id, commenturl);
                    }
                    sr.Close();

                    
                    foreach (int serviceid in services.Keys)
                    {
                        try
                        {
                            log.Debug(RatingsManager.LogCount + " iTunes Ingest for serviceid [" + serviceid + "]");

                            ITunesResults i;
                            i = DownloadHelper.DownloadSerialiseJson<ITunesResults>(services[serviceid]);

                            string v = "";

                            string insertsql = @"
                                 
                                 declare @version nvarchar(50);
                                 declare @isnew nchar(1);
                                 declare @releaseDate DateTime;
                                 
                                 set @releaseDate = '{3}';

                                 if (exists ( select top 1 version from vui_ServiceStoreRating where [ServiceId]={0} and DeviceType='{1}' order by DateRecorded desc))
                                 begin
                                    set @version = (select top 1 version from vui_ServiceStoreRating where [ServiceId]={0} and DeviceType='{1}' order by DateRecorded desc);
                                 end
                                 else
                                    set @version = '0.0';

                                 if(@version != '{2}')
                                 begin
                                    set @isnew = 'Y';
                                    set @releaseDate = GetDate();
                                 end
                                 else
                                 begin
                                    set @isnew = 'N';
                                    
                                 end

                                 INSERT INTO vui_ServiceStoreRating
                                       ([ServiceID]
                                       ,DeviceType
                                       ,[Version]
                                       ,[NewVersion]
                                       ,[ReleaseDate]
                                       ,[AverageUserRating]
                                       ,[UserRatingCount]
                                       ,AverageUserRatingForCurrentVersion
                                       ,UserRatingCountForCurrentVersion
                                       ,[DateRecorded]
                                       , IngestTag)
                                 VALUES
                                       ({0},'{1}','{2}',@isnew, @releaseDate,'{4}',{5},'{6}',{7}, GetDate(), '{8}')";



                            insertsql = String.Format(insertsql, new Object[] { serviceid, deviceType, i.results[0].version, i.results[0].releaseDate, i.results[0].averageUserRating, i.results[0].userRatingCount, i.results[0].averageUserRatingForCurrentVersion, i.results[0].userRatingCountForCurrentVersion, IngestTag });

                            comm = new SqlCommand(insertsql, conn);
                            comm.ExecuteNonQuery();


                            insertsql = @"IF EXISTS (SELECT * from vui_ServiceStoreDetail where ServiceID=@serviceid and DeviceType = @deviceType)
                                      UPDATE vui_ServiceStoreDetail 
                                      SET    [Description]=@description
                                      ,      logoURL=@logourl
                                      ,      lastUpdate=GetDate()
                                      WHERE  ServiceID = @serviceid 
                                         and DeviceType = @deviceType;
                                    ELSE
                                      INSERT INTO  vui_ServiceStoreDetail (ServiceID, DeviceType, [Description], logoURL, lastUpdate)
                                      VALUES (@serviceid, @deviceType, @description, @logourl, GetDate());";

                            comm = new SqlCommand(insertsql, conn);
                            comm.Parameters.Add(new SqlParameter("@serviceid", serviceid));
                            comm.Parameters.Add(new SqlParameter("@deviceType", deviceType));
                            comm.Parameters.Add(new SqlParameter("@logourl", i.results[0].artworkUrl512));
                            comm.Parameters.Add(new SqlParameter("@description", i.results[0].description));
                            comm.ExecuteNonQuery();



                            RatingsManager.AddITunesSuccess(serviceid + "-" + services[serviceid]);
                        }
                        catch (Exception ex)
                        {
                            int lc = RatingsManager.LogCount;
                            log.Info(lc + " !!!!! Error iTunes Ingest for serviceid [" + serviceid + "]");
                            log.Error(ex);
                            RatingsManager.AddErrorToLog(lc, "iTunes Ingest for serviceid [" + serviceid + "]", ex);
                        }
                    }
                    

                    if (!skipcomments)
                    {
                        foreach (int serviceid in serviceComments.Keys)
                        {
                            try
                            {
                                log.Debug(RatingsManager.LogCount + " iTunes Comments Ingest for serviceid [" + serviceid + "]");

                                ITunes.ITunesComments i;
                                i = DownloadHelper.DownloadSerialiseJson<ITunes.ITunesComments>(serviceComments[serviceid], "im:", "");


                                DoITunesComments(serviceid, deviceType, i, conn);


                            }
                            catch (Exception ex)
                            {
                                int lc = RatingsManager.LogCount;
                                log.Info(lc + " !!!!! Error iTunes Comments Ingest for serviceid [" + serviceid + "]");
                                log.Error(ex);
                                RatingsManager.AddErrorToLog(lc, "iTunes Comments Ingest for serviceid [" + serviceid + "]", ex);
                            }
                        }
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + " !!!!! Error iTunes Ingest");
                    log.Error(ex);
                    RatingsManager.AddErrorToLog(lc, "iTunes Ingest", ex);
                }
            }

        }

        public static void DoITunesComments(int serviceid, string deviceType, ITunes.ITunesComments i, SqlConnection conn)
        {
            foreach (ITunes.Entry e in i.feed.entry.Where(v => v.author != null).ToList())
            {
                try
                {
                    string id = e.id.label;
                    string title = e.title.label;
                    if(title.Length > 500) 
                    {
                        title = title.Substring(0, 500);
                    }
                    string comment = e.content.label;
                    if(comment.Length > 2000) 
                    {
                        comment = comment.Substring(0, 2000);
                    }
                    string version = e.version.label;
                    if (version.Length > 20)
                    {
                        version = version.Substring(0, 20);
                    }
                    string rating = e.rating.label;
                    if (rating.Length > 20)
                    {
                        rating = rating.Substring(0, 20);
                    }

                    string commentsql = @"if(NOT exists(select * from vui_StoreRatingComments where Store='iTunes' and (DeviceType is null OR DeviceType=@deviceType) and Id=@Id))
                                        insert into vui_StoreRatingComments (Id , ServiceId , Store , Title, Comment, Rating, Version, ReviewDate, DateRecorded, DeviceType)
                                                                    values(@Id, @serviceId       ,'iTunes', @title, @comment, @rating,  @version  , null     , GetDate(), @deviceType); ";


                    SqlCommand comm = new SqlCommand(commentsql, conn);
                    comm.Parameters.Add(new SqlParameter("@Id", id));
                    comm.Parameters.Add(new SqlParameter("@serviceid", serviceid));
                    comm.Parameters.Add(new SqlParameter("@title", title));
                    comm.Parameters.Add(new SqlParameter("@comment", comment));
                    comm.Parameters.Add(new SqlParameter("@rating", rating));
                    comm.Parameters.Add(new SqlParameter("@version", version));
                    comm.Parameters.Add(new SqlParameter("@deviceType", deviceType));

                    comm.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + " !!!!! Error iTunes Comments Ingest for serviceid [" + serviceid + "]");
                    log.Error(exc);
                    RatingsManager.AddErrorToLog(lc, "iTunes Comments Ingest for serviceid [" + serviceid + "]", exc);
                }

            }
            if (bLookForOldComments)
            {
                string self = i.feed.link.First(l => l.attributes.rel.Equals("self")).attributes.href.Replace("/xml","/json");
                string next = i.feed.link.First(l => l.attributes.rel.Equals("next")).attributes.href.Replace("/xml", "/json");
                string last = i.feed.link.First(l => l.attributes.rel.Equals("last")).attributes.href.Replace("/xml", "/json");

                if (!self.Equals(last))
                {
                    try
                    {
                        log.Debug(RatingsManager.LogCount + " iTunes Comments Ingest for serviceid [" + serviceid + "]");

                        ITunes.ITunesComments i2;
                        i2 = DownloadHelper.DownloadSerialiseJson<ITunes.ITunesComments>(next, "im:", "");


                        DoITunesComments(serviceid, deviceType, i2, conn);

                    }
                    catch (Exception ex)
                    {
                        int lc = RatingsManager.LogCount;
                        log.Info(lc + " !!!!! Error iTunes Comments Ingest for serviceid [" + serviceid + "]");
                        log.Error(ex);
                        RatingsManager.AddErrorToLog(lc, "iTunes Comments Ingest for serviceid [" + serviceid + "]", ex);
                    }
                }

            }
        }

    }
}
