using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace StoreRatings
{
    public class WindowsMarketManager
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(WindowsMarketManager));


        public static void GetWindowsRatings(string colName, string deviceType, string IngestTag)
        {
            // for each item in a list, get the JSON, extract the relevant data and save to database

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                try
                {
                    log.Debug(RatingsManager.LogCount + " Starting Windows Market Ingest");
                    Dictionary<int, string> services = new Dictionary<int, string>();

                    string sql = @"select S.ID, [" + colName + @"]
                                   from   vui_ServiceMasters S 
                                   where  [" + colName + @"] is not null and [" + colName + @"] != ''";

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
                            log.Debug(RatingsManager.LogCount + " Windows Market Ingest for serviceid [" + serviceid + "]");

                            WindowsMarketInfo p;
                            p = DownloadHelper.DownloadWindowsMarketPage(services[serviceid]);

                            if (p != null)
                            {

                                string insertsql = @"INSERT INTO vui_ServiceStoreRating
                                       ([ServiceID]
                                       ,DeviceType
                                       ,[Version]
                                       ,[ReleaseDate]
                                       ,[AverageUserRating]
                                       ,[UserRatingCount]
                                       ,[DateRecorded],IngestTag)
                                 VALUES
                                       ({0},'{1}','{2}','{3}','{4}','{5}', GetDate(), '{6}')";

                                insertsql = String.Format(insertsql, new Object[] { serviceid, deviceType, p.currentVersion, p.lastUpdated, p.rating, p.numberOfRatings, IngestTag });
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
                                comm.Parameters.Add(new SqlParameter("@logourl", p.logoURL));
                                comm.Parameters.Add(new SqlParameter("@description", p.description));
                                comm.ExecuteNonQuery();

                                RatingsManager.AddWindowsSuccess(serviceid + "-" + services[serviceid]);

                            }
                        }

                        catch (Exception ex)
                        {
                            int lc = RatingsManager.LogCount;
                            log.Info(lc + " !!!!! Error Windows Market Ingest for serviceid [" + serviceid + "]");
                            log.Error(ex);
                            RatingsManager.AddErrorToLog(lc, "Windows Market Ingest for serviceid [" + serviceid + "]", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    int lc = RatingsManager.LogCount;
                    log.Info(lc + " !!!!! Error Windows Market Ingest");
                    log.Error(ex);
                    RatingsManager.AddErrorToLog(lc, "Windows Market Ingest", ex);
                }
            }
        }
    }
}
