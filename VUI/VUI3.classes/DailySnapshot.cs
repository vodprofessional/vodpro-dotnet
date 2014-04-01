using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;

namespace VUI.VUI3.classes
{
    public class DailySnapshot
    {
        public DailySnapshot()
        {
            
        }

        public DailySnapshot(string serviceName)
        {
            ServiceName = serviceName;
            ServiceMaster = new VUI3ServiceMaster(ServiceName);
            Save();
        }

        public static DailySnapshot GenerateSnapshot()
        {
            DailySnapshot ds = new DailySnapshot();
            ds.Pop();
            ds.ServiceMaster = new VUI3ServiceMaster(ds.ServiceName);
            ds.Save();
            return ds;
        }

        private VUI3ServiceMaster ServiceMaster { get; set; }
        private int SnapshotId { get; set; }
        public string ServiceName { get; set; }
        public string IconURL { get { return ServiceMaster.IconURL; } }
        public string Availability { get { return ServiceMaster.Node.GetProperty("availability").Value; } }
        public string PayModel { get { return ServiceMaster.Node.GetProperty("subscriptionType").Value; } }
        public string Category { get { return ServiceMaster.Node.GetProperty("serviceCategory").Value; } }
        public VUI3ServiceSnapshot Snapshot { get { return ServiceMaster.GetSnapshot(); } }
        public VUI3ScreenshotList Screenshots
        {
            get
            {
                try
                {
                    VUI3ScreenshotAnalysis an = ServiceMaster.ScreenshotPackages.analyses.OrderByDescending(p => p.AnalysisDate).First(q => q.ImageCount > 0);
                    an.WithScreenshots = true;
                    if (an.ScreenshotsList.screenshots.Count > 10)
                        an.ScreenshotsList.screenshots = an.ScreenshotsList.screenshots.GetRange(0, 10).ToList<VUI3Screenshot>();
                    return an.ScreenshotsList;
                }
                catch (Exception ex)
                {
                    return new VUI3ScreenshotList();
                }
            }
        }


        private void Save()
        {
            string JSON = this.AsJson();
            string sql = @" if (exists(select top 1 JSON FROM vui_DailySnapshot Where Id = @Id))
                                update vui_DailySnapshot set JSON = @JSON where Id = @Id;
                            else
                                insert into vui_DailySnapshot(Id, ServiceName, JSON) values (@Id, @ServiceName, @JSON)";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add("@Id", SnapshotId);
                comm.Parameters.Add("@ServiceName", ServiceName);
                comm.Parameters.Add("@JSON", JSON);
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Get the next ServiceName off the stack, and clear it out to the record database 
        /// </summary>
        private void Pop()
        {
            string sql = @"vui_DailySnapshotPop";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandText = sql;
                comm.Connection = conn;

                SqlDataReader sr = comm.ExecuteReader();
                sr.Read();
                SnapshotId = (int)sr[0];
                ServiceName = (string)sr[1];
                conn.Close();
            }
        }

        /// <summary>
        /// Get the most recent DailySnapshot off the DailySnapshot table.
        /// </summary>
        /// <returns></returns>
        public static string GetDailySnapshot()
        {
            string JSON = String.Empty;

            string sql = @"select top 1 JSON, DateDisplayed, ServiceName FROM vui_DailySnapshot Where CAST(DateDisplayed as DATE) = @Date ";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add("@Date", DateTime.Today.ToString("yyyy-MM-dd"));
                SqlDataReader sr = comm.ExecuteReader();
                if (sr.HasRows)
                {
                    sr.Read();
                    JSON = (string)sr[0];
                    DateTime lastUpdate = (DateTime)sr[1];
                    
                    // Needs regenerating
                    if (lastUpdate.Hour < 9)
                    {
                        string serviceName = (string)sr[2];
                        DailySnapshot ds = new DailySnapshot(serviceName);
                        JSON = ds.AsJson();
                    }
                }
                else
                {
                    DailySnapshot ds = DailySnapshot.GenerateSnapshot();
                    JSON = ds.AsJson();
                }
                conn.Close();
            }
            return JSON;
        }


        
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<DailySnapshot>(this);
        }
    }
}