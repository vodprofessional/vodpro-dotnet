using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using umbraco.MacroEngines;

namespace VUI.VUI2.classes
{
    public static class VUIDataFunctions
    {

        public static List<string> SearchServices()
        {
            List<string> ss = new List<string>();

            string sql = @"select ServiceName from vui_ScreenshotMatrix where Total>0 order by ServiceName ASC";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    ss.Add((string)sr[0]);
                }
                conn.Close();
            }
            return ss;
        }


        public static List<SearchImage> GetSearchImages()
        {
            List<SearchImage> images = new List<SearchImage>();

            string sql = @"
                            SELECT [Id]
                                  ,[ImageURL_th]
                                  ,[ImageURL_md]
                                  ,[ImageURL_lg]
                                  ,[ImageURL_full]
                                  ,[ServiceName]
                                  ,[Name]
                                  ,[PlatformName]
                                  ,[DeviceName]
                                  ,[URL]
                                  ,PageType
                              FROM [dbo].[vui_SearchImage];";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    SearchImage si = new SearchImage();
                    si.Id = (int)sr[0];
                    si.ImageURL_th = (string)sr[1];
                    si.ImageURL_md = (string)sr[2];
                    si.ImageURL_lg = (string)sr[3];
                    si.ImageURL_full = (string)sr[4];
                    si.ServiceName = (string)sr[6];
                    si.Platform = (string)sr[7];
                    si.Device = (string)sr[8];
                    si.ScreenshotsURL = (string)sr[9];
                    si.PageType = (string)sr[10];
                    images.Add(si);
                }
                conn.Close();
            }

            return images;
        }


        public static List<MatrixService> ScreenshotMatrix()
        {
            //Dictionary<string, int[]> matrix = new Dictionary<string, int[]>();
            
            List<MatrixService> matrix = new List<MatrixService>();

            string sql = @"select 
                              TabletAndroid
                            , SmartphoneAndroid
                            , TabletiPad
                            , SmartphoneiPhone
                            , SmartphoneWindows
                            , Web
                            , Total 
                            , M.ServiceName
                            , S.IsPreviewable
                            , S.DefaultScreenshotID
                            from   vui_ScreenshotMatrix M
                            inner join [dbo].[vui_ServiceScreenshotsMeta] S on M.ServiceName = S.ServiceName and S.Context='All' order by M.ServiceName";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    int[] counts = new int[7];
                    for (int i = 0; i < 7; i++)
                    {
                        counts[i] = (Int32)sr[i];
                    }
                    MatrixService s = new MatrixService();
                    s.ServiceName = (string)sr[7];
                    s.IsPreviewable = (bool)sr[8];
                    s.DefaultScreenshotID = sr[9].GetType() == typeof(DBNull) ? -1 : (int)sr[9];
                    s.Counts = counts;
                    matrix.Add(s);
                }
                conn.Close();
            }
            return matrix;
        }

        public static List<Service> FindServiceOnAllPlatforms(string serviceName)
        {
            List<Service> services = new List<Service>();
            string sql = String.Format(@"select S.Id from vui_Service S where S.ServiceName='{0}'", serviceName);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    int Id = (Int32)sr[0];
                    services.Add(new Service(Id));
                }
                conn.Close();
            }
            return services;
        }


        public static List<Service> FindServiceOnPlatform(int platformId, string serviceName)
        {
            List<Service> services = new List<Service>();
            string sql = String.Format(@"select S.Id from vui_Service S where S.PlatformID = {0} and S.ServiceName='{1}'", platformId, serviceName);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    int Id = (Int32)sr[0];
                    services.Add(new Service(Id));
                }
                conn.Close();
            }
            return services;
        }

        public static int GetNumberOfBenchmarksByDevice(int deviceid)
        {
            int count = 0;
            string sql = String.Format(@"select count(*) from vui_Service where deviceid={0} and BenchmarkScore > 0", deviceid);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    count = (Int32)sr[0];
                }
                conn.Close();
            }
            return count;
        }

        public static Dictionary<string, int> GetBenchmarkScoresByDevice(int deviceid)
        {
            string sql = String.Format(@"select feature, SUM(score) from vui_BenchmarkScores where deviceid={0} group by feature ", deviceid);
            Dictionary<string, int> scores = new Dictionary<string, int>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    scores.Add((string)sr[0], (Int32)sr[1]);
                }
                conn.Close();
            }
            return scores;
        }

        public static int GetNumberOfBenchmarksByPlatform(int platformid)
        {
            int count = 0;
            string sql = String.Format(@"select count(*) from vui_Service where platformid={0} and BenchmarkScore > 0 ", platformid);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    count = (Int32)sr[0];
                }
                conn.Close();
            }
            return count;
        }

        public static Dictionary<string, int> GetBenchmarkScoresByPlatform(int platformid)
        {
            string sql = String.Format(@"select feature, SUM(score) from vui_BenchmarkScores where platformid={0} group by feature", platformid);
            Dictionary<string, int> scores = new Dictionary<string, int>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    scores.Add((string)sr[0], (Int32)sr[1]);
                }
                conn.Close();
            }
            return scores;
        }

        public static int GetNumberOfBenchmarks()
        {
            int count = 0;
            string sql =@"select count(*) from vui_Service where BenchmarkScore > 0";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    count = (Int32)sr[0];
                }
                conn.Close();
            }
            return count;
        }

        public static Dictionary<string, int> GetAllBenchmarkScores()
        {
            string sql = @"select feature, SUM(score) from vui_BenchmarkScores group by feature";
            Dictionary<string, int> scores = new Dictionary<string, int>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    scores.Add((string)sr[0], (Int32)sr[1]);
                }
                conn.Close();
            }
            return scores;
        }



        // 
        public static List<ServiceMetadata> GetScreenshotServiceList(string context)
        {
            List<ServiceMetadata> meta = new List<ServiceMetadata>();
            string sql;
            if (context.ToLower().Equals("all"))
            {
                sql = String.Format(@"select S.Name, S.Id, S.DefaultScreenshotId, S.IsPreviewable, S.PlatformId, S.DeviceID, D.Name, S.ScreenshotCount, S.ServiceName from vui_service S inner join vui_Platform P on S.PlatformID = P.ID left outer join vui_Device D on S.DeviceID = D.ID where S.ScreenshotCount > 0 and DefaultScreenshotId > 0 order by S.Name asc");
            }
            else
            {
                sql = String.Format(@"select S.Name, S.Id, S.DefaultScreenshotId, S.IsPreviewable, S.PlatformId, S.DeviceID, D.Name, S.ScreenshotCount, S.ServiceName from vui_service S inner join vui_Platform P on S.PlatformID = P.ID left outer join vui_Device D on S.DeviceID = D.ID where S.ScreenshotCount > 0 and LOWER(P.Name) = '{0}' and DefaultScreenshotId > 0 order by S.Name asc, DefaultScreenshotId DESC", context.ToLower());
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                // Work through the results set
                // For each result, get the default image from the 
                ServiceMetadata previousMeta = null;

                while (sr.Read())
                {
                    string name = (String)sr[0];
                    int Id = (Int32)sr[1];
                    int DefaultScreenshotId = (Int32)sr[2];
                    bool IsPreviewable = sr[3].GetType() == typeof(DBNull) ? false : (bool)sr[3];
                    int PlatformId = (Int32)sr[4];
                    int DeviceId = (Int32)sr[5];
                    string DeviceName = sr[6].GetType() == typeof(DBNull) ? null : (string)sr[6];
                    int ScreenshotCount = (Int32)sr[7];
                    string servicename = (String)sr[8];
                    
                    ServiceMetadata sm = new ServiceMetadata();
                    sm.Context = context;
                    sm.Name = name;
                    sm.ServiceName = servicename;
                    sm.IsPreviewable = IsPreviewable;
                    sm.DefaultServiceId = Id;
                    sm.DefaultScreenshotId = DefaultScreenshotId;
                    sm.ParentPlatform = PlatformId;
                    sm.ParentDevice = DeviceId;
                    sm.ParentDeviceName = DeviceName;
                    sm.ScreenshotCount = ScreenshotCount;

                    // If the previous had the same name
                    if (previousMeta != null && previousMeta.ServiceName.Equals(sm.ServiceName))
                    {
                        // If the previousMeta had an ID of -1 but this one doesn't
                        int index = meta.FindLastIndex(m => m.ServiceName.Equals(sm.ServiceName));
                        ServiceMetadata sm2 = meta[index];
                        if (sm2.DefaultScreenshotId == -1 && sm.DefaultScreenshotId != -1)
                        {
                            sm2.DefaultScreenshotId = sm.DefaultScreenshotId;
                        }
                        sm2.NumInstances ++;
                        sm2.ScreenshotCount += sm.ScreenshotCount;
                        meta[index] = sm2;
                    }
                    else
                    {
                        meta.Add(sm);
                    }
                    previousMeta = sm;

                }
                conn.Close();
            }
            return meta;
        }


        /// <summary>
        /// Highest Benchmarking scores by platform
        /// </summary>
        /// <param name="platformid"></param>
        /// <returns></returns>
        public static List<Service> GetBestServicesForPlatform(int platformid)
        {
            List<Service> services = new List<Service>();

            string sql = String.Format(@"select * from vui_Service where PlatformId = {0} order by BenchmarkScore desc;", platformid);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);

                SqlDataReader sr = comm.ExecuteReader();

                // Work through the results set
                // For each result, get the default image from the 
                while (sr.Read())
                {
                    int serviceId = (Int32)sr["ID"];
                    Service s = new Service(serviceId);
                    services.Add(s);
                }
                conn.Close();
            }
            return services;
        }


        /// <summary>
        /// Highest Benchmarking scores by device (because done on ID, Platform implied)
        /// </summary>
        /// <param name="platformid"></param>
        /// <returns></returns>
        public static List<Service> GetBestServicesForDevice(int deviceid)
        {
            List<Service> services = new List<Service>();

            string sql = String.Format(@"select * from vui_Service where DeviceId = {0} order by BenchmarkScore desc;", deviceid);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);

                SqlDataReader sr = comm.ExecuteReader();

                // Work through the results set
                // For each result, get the default image from the 
                while (sr.Read())
                {
                    int serviceId = (Int32)sr["ID"];
                    Service s = new Service(serviceId);
                    services.Add(s);
                }
                conn.Close();
            }
            return services;
        }

        public static void ExecQuery(string sql)
        {
            // Get all the services with the number of platforms/devices available
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandTimeout = 600;
                int rows = comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void UpdateServiceScreenshotsMetaData(int Id)
        {
            Service service = new Service(Id);
            // First, simply add the Service to vui_Service table
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.CommandText = @"vui_UpdateServiceScreenshotsMetaData";
                    comm.Parameters.Add(new SqlParameter("@ServiceId", Id));
                    comm.Parameters.Add(new SqlParameter("@ServiceName", service.Name));
                    if (service.Platform != null)
                    {
                        comm.Parameters.Add(new SqlParameter("@PlatformId", service.Platform.NodeId));
                    }
                    else
                    {
                        comm.Parameters.Add(new SqlParameter("@PlatformId", null));
                    }
                    if (service.Device != null)
                    {
                        comm.Parameters.Add(new SqlParameter("@DeviceId", service.Device.NodeId));
                    }
                    else
                    {
                        comm.Parameters.Add(new SqlParameter("@DeviceId", null));
                    }
                    int rows = comm.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static Dictionary<int, string> GetServiceAppStoreURLs()
        {
            Dictionary<int, string> services;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {

                services = new Dictionary<int, string>();

                string sql = @"select ServiceID, URL from vui_ServiceStoreURLs S where ServiceID is not null and URL is not null";

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
            }
            return services;
        }

    }


    public struct SearchImage
    {
        public int Id { get; set; }
        public string ServiceName {get; set;}
        public bool IsPreviewable { get; set; }
        public string ImageURL_th {get; set;}
        public string ImageURL_md {get; set; }
        public string ImageURL_lg {get; set; }
        public string ImageURL_full {get; set;}
        public string Platform { get; set;}
        public string Device { get; set;}
        public string ScreenshotsURL { get; set;}
        public string PageType { get; set;}
    }

    public struct MatrixService
    {
        public string ServiceName { get; set; }
        public bool IsPreviewable { get; set;}
        public int DefaultScreenshotID { get; set; }
        public int[] Counts { get; set; }

        public static Dictionary<string,string> URLfromContext = new Dictionary<string, string>() {
            {"all",@"all"},
            {"TabletAndroid",@"tablet/android"},
            {"SmartphoneAndroid",@"smartphone/android"}, 
            {"TabletiPad",@"tablet/ipad"}, 
            {"SmartphoneiPhone",@"smartphone/iphone"}, 
            {"SmartphoneWindows",@"smartphone/windows"},
            {"Web","web"}
        };

        public static Dictionary<string, int> MatrixPositions = new Dictionary<string, int>() {
            {"TabletAndroid",0},
            {"SmartphoneAndroid",1}, 
            {"TabletiPad",2}, 
            {"SmartphoneiPhone",3}, 
            {"SmartphoneWindows",4},
            {"Web",5},
            {"Total",6}
        };

    }
}