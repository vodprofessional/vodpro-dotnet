using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace StoreRatings
{
    public class PlayStoreManager
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlayStoreManager));

        public static void GetPlayStoreRatings(string colName, string deviceType, string IngestTag, bool skipcomments, List<string> serviceNames)
        {
            // for each item in a list, get the JSON, extract the relevant data and save to database

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                try
                {
                    log.Debug(RatingsManager.LogCount + " Starting Google Play Ingest");
                    Dictionary<int, string> services = new Dictionary<int, string>();

                    string sql = @"select S.ID, [" + colName + @"]
                                   from   vui_ServiceMasters S 
                                   where  [" + colName + @"] is not null and [" + colName + @"] != ''";

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
                        string url = (string)sr[1];
                        services.Add(id, url);
                    }
                    sr.Close();


                    foreach (int serviceid in services.Keys)
                    {
                        try
                        {
                            log.Debug(RatingsManager.LogCount + " Google Play Ingest for serviceid [" + serviceid + "]");

                            PlayStoreInfo p;
                            p = DownloadHelper.DownloadPlayStorePage(services[serviceid]);

                            if (p != null)
                            {

                                string insertsql = @"
                                 declare @version nvarchar(50);
                                 declare @isnew nchar(1);

                                 if (exists ( select top 1 version from vui_ServiceStoreRating where [ServiceId]={0} and DeviceType='{1}' order by DateRecorded desc))
                                 begin
                                    set @version = (select top 1 version from vui_ServiceStoreRating where [ServiceId]={0} and DeviceType='{1}' order by DateRecorded desc);
                                 end
                                 else
                                    set @version = '0.0';

                                 if(@version != '{2}')
                                 begin
                                    set @isnew = 'Y';
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
                                       ,[NumDownloads]
                                       ,[DateRecorded]
                                       ,IngestTag)
                                 VALUES
                                 
                                       ({0},'{1}','{2}', @isNew,'{3}','{4}',{5},'{6}', GetDate(), '{7}')";

                                insertsql = String.Format(insertsql, new Object[] { serviceid, deviceType, p.currentVersion, p.lastUpdated, p.rating, p.numberOfRatings, p.numDownloads, IngestTag });
                                comm = new SqlCommand(insertsql, conn);
                                comm.ExecuteNonQuery();


                                if (!skipcomments)
                                {
                                    foreach (PlayStoreComment c in p.Comments)
                                    {

                                        string id = c.Id;
                                        string title = c.Title;
                                        if(title.Length > 500) 
                                        {
                                            title = title.Substring(0, 500);
                                        }
                                        string comment = c.Comment;
                                        if (comment.Length > 2000)
                                        {
                                            comment = comment.Substring(0, 2000);
                                        }
                                        string rating = c.Rating;
                                        if (rating.Length > 20)
                                        {
                                            rating = rating.Substring(0, 20);
                                        }

                                        string commentsql = @"if(NOT exists(select * from vui_StoreRatingComments where Store='Google Play' and (DeviceType is null OR DeviceType=@deviceType) and Id=@Id))
                                                        insert into vui_StoreRatingComments (Id , ServiceId , Store , Title, Comment, Rating, Version, ReviewDate, DateRecorded, DeviceType)
                                                                                    values(@Id, @serviceId       ,'Google Play', @title, @comment, @rating,  null  , @reviewDate     , GetDate(), @deviceType); ";


                                        comm = new SqlCommand(commentsql, conn);
                                        comm.Parameters.Add(new SqlParameter("@Id", id));
                                        comm.Parameters.Add(new SqlParameter("@serviceid", serviceid));
                                        comm.Parameters.Add(new SqlParameter("@title", title));
                                        comm.Parameters.Add(new SqlParameter("@comment", comment));
                                        comm.Parameters.Add(new SqlParameter("@rating", rating));
                                        comm.Parameters.Add(new SqlParameter("@reviewDate", c.ReviewDate));
                                        comm.Parameters.Add(new SqlParameter("@deviceType", deviceType));
                                        try
                                        {
                                            comm.ExecuteNonQuery();
                                        }
                                        catch (Exception exc)
                                        {
                                            int lc = RatingsManager.LogCount;
                                            log.Info(lc + " !!!!! Error Play Comments Ingest for serviceid [" + serviceid + "]");
                                            log.Error(exc);
                                            RatingsManager.AddErrorToLog(lc, "Error Play Comments Ingest for serviceid [" + serviceid + "]", exc);
                                        }
                                    }
                                }

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
                                comm.Parameters.Add(new SqlParameter("@logourl", p.logoURL));
                                comm.Parameters.Add(new SqlParameter("@description", p.description));
                                comm.ExecuteNonQuery();


                                RatingsManager.AddPlaySuccess(serviceid + "-" + services[serviceid]);

                            }
                        }

                        catch (Exception ex)
                        {
                            int lc = RatingsManager.LogCount;
                            log.Info(lc + " !!!!! Error Google Play Ingest for serviceid [" + serviceid + "]");
                            log.Error(ex);
                            RatingsManager.AddErrorToLog(lc, "Google Play Ingest for serviceid [" + serviceid + "]", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + " !!!!! Error Google Play Ingest");
                    log.Error(ex);
                    RatingsManager.AddErrorToLog(lc, "Google Play Ingest", ex);
                }
            }
        }
    }
}
