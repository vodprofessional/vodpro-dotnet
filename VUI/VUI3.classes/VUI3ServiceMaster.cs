using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using umbraco.MacroEngines;


namespace VUI.VUI3.classes
{
    public class VUI3ServiceMaster
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3ServiceMaster));

        private bool _isInitialised = false;
        private DynamicNode _node;

        public VUI3ServiceMaster()
        {
            ;
        }
        public VUI3ServiceMaster(string serviceName)
        {
            Initialise(serviceName);
        }
        public VUI3ServiceMaster(int nodeid)
        {
            Initialise(nodeid);
        }

        /// <summary>
        /// Initialise the ServiceMaster if you have used the no-param constructor
        /// </summary>
        /// <param name="serviceName"></param>
        public void Initialise(string serviceName)
        {
            if(!_isInitialised)
            {
                DynamicNode node = new DynamicNode(Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString()));
                List<DynamicNode> nodes = node.Descendants().Items.ToList().Where(n => n.GetProperty("serviceName").ToString().Equals(serviceName)).ToList();
                if (nodes.Count < 1)
                {
                    throw new Exception("No ServiceMaster with that name");
                }
                else
                {
                    Node = nodes.First();
                    _isInitialised = true;
                }
            }
        }

        /// <summary>
        /// Initialise the ServiceMaster if you have used the no-param constructor
        /// </summary>
        /// <param name="id"></param>
        public void Initialise(int id)
        {
            if(!_isInitialised)
            {
                DynamicNode node = new DynamicNode(id);
                Node = node;
                _isInitialised = true;
            }
        }

        /// <summary>
        /// Returns the most recent for a Service
        /// </summary>
        /// <returns></returns>
        public VUI3ServiceSnapshot GetSnapshot()
        {
            log.Debug("Getting Service Snapshot for: " + ServiceName);
            if (!String.IsNullOrEmpty(ServiceName))
            {
                return GetSnapshot(ServiceName);
            }
            else
            {
                return null;
            }
        }


        public string IconURL
        {
            get
            {
                string sql = @" select top 1 IconURL from vui_ServiceMasters where ServiceName='{0}'";
                sql = String.Format(sql, this.ServiceName);

                string _icon = String.Empty;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader sr = comm.ExecuteReader();

                    if (sr.HasRows)
                    {
                        sr.Read();
                        _icon = (String)sr["IconURL"];
                    }
                    conn.Close();
                }
                return _icon;
            }
        }

        /// <summary>
        /// Returns a single snapshot for a Service based on the snapshot immediately before the requested date
        /// If there isn't one, the bottom snapshot off the list is taken.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public VUI3ServiceSnapshot GetSnapshotByDate(DateTime dt)
        {
            if (!String.IsNullOrEmpty(ServiceName))
            {
                return GetSnapshotByDate(ServiceName, dt);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the most recent for a Service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static VUI3ServiceSnapshot GetSnapshot(string serviceName)
        {
            string sql = @" select top 1 ServiceMasterId, SnapshotDate, NumScreenshots, NumScreenshotDevices, BenchmarkAverage, BenchmarkDevices, RatingAverage, TwitterFollowers, FacebookLikes, YTVideoCount, YTSubscriberCount 
                            from vui_ServiceMasterSnapshot where ServiceName='{0}' order by SnapshotDate desc";

            sql = String.Format(sql, serviceName);

            log.Debug(sql);

            VUI3ServiceSnapshot ss = null;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();

                

                while (sr.Read())
                {
                    ss = new VUI3ServiceSnapshot();
                    ss.id = (int)sr["ServiceMasterId"];
                    ss.serviceName = serviceName;
                    ss.snapshotDate = (DateTime)sr["SnapshotDate"];
                    ss.numScreenshots = sr["NumScreenshots"].GetType() == typeof(DBNull) ? 0 : (int)sr["NumScreenshots"];
                    ss.numScreenshotDevices = sr["NumScreenshotDevices"].GetType() == typeof(DBNull) ? 0 : (int)sr["NumScreenshotDevices"];
                    ss.benchmarkAverage = sr["BenchmarkAverage"].GetType() == typeof(DBNull) ? 0 : (int)sr["BenchmarkAverage"];
                    ss.benchmarkDevices = sr["BenchmarkDevices"].GetType() == typeof(DBNull) ? 0 : (int)sr["BenchmarkDevices"];
                    ss.ratingAverage = sr["RatingAverage"].GetType() == typeof(DBNull) ? 0 : (decimal)sr["RatingAverage"];
                    ss.twitterFollowers = sr["TwitterFollowers"].GetType() == typeof(DBNull) ? 0 : (int)sr["TwitterFollowers"];
                    ss.facebookLikes = sr["FacebookLikes"].GetType() == typeof(DBNull) ? 0 : (int)sr["FacebookLikes"];
                    ss.ytVideoCount = sr["YTVideoCount"].GetType() == typeof(DBNull) ? 0 : (int)sr["YTVideoCount"];
                    ss.ytSubscriberCount = sr["YTSubscriberCount"].GetType() == typeof(DBNull) ? 0 : (int)sr["YTSubscriberCount"];

                    // Really only want to do one row!
                    break;
                }
                conn.Close();

            }
            return ss;
        }

        /// <summary>
        /// Returns a single snapshot for a Service based on the snapshot immediately before the requested date
        /// If there isn't one, the bottom snapshot off the list is taken.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static VUI3ServiceSnapshot GetSnapshotByDate(string serviceName, DateTime dt)
        {
            VUI3ServiceSnapshot ss = null;

            string sql = @" select top 1 ServiceMasterId, SnapshotDate, NumScreenshots, NumScreenshotDevices, BenchmarkAverage, BenchmarkDevices, RatingAverage, TwitterFollowers, FacebookLikes, YTVideoCount, YTSubscriberCount 
                            from   vui_ServiceMasterSnapshot 
                            where ServiceName='{0}' and SnapshotDate < '{1}'
                            order by SnapshotDate desc";

            sql = String.Format(sql, serviceName, dt.ToString("yyyy-MM-dd HH:mm:ss.0"));

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                
                if (!sr.HasRows)
                {
                    sr.Close();
                    sql = @" select top 1 ServiceMasterId, SnapshotDate, NumScreenshots, NumScreenshotDevices, BenchmarkAverage, BenchmarkDevices, RatingAverage, TwitterFollowers, FacebookLikes, YTVideoCount , YTSubscriberCount
                            from   vui_ServiceMasterSnapshot where ServiceName='{0}' 
                            order by SnapshotDate asc";

                    sql = String.Format(sql, serviceName);
                    comm = new SqlCommand(sql, conn);
                    sr = comm.ExecuteReader();
                }

                while (sr.Read())
                {
                    ss = new VUI3ServiceSnapshot();
                    ss.id = (int)sr["ServiceMasterId"];
                    ss.serviceName = serviceName;
                    ss.snapshotDate = (DateTime)sr["SnapshotDate"];
                    ss.numScreenshots = sr["NumScreenshots"].GetType() == typeof(DBNull) ? 0 : (int)sr["NumScreenshots"];
                    ss.numScreenshotDevices = sr["NumScreenshotDevices"].GetType() == typeof(DBNull) ? 0 : (int)sr["NumScreenshotDevices"];
                    ss.benchmarkAverage = sr["BenchmarkAverage"].GetType() == typeof(DBNull) ? 0 : (int)sr["BenchmarkAverage"];
                    ss.benchmarkDevices = sr["BenchmarkDevices"].GetType() == typeof(DBNull) ? 0 : (int)sr["BenchmarkDevices"];
                    ss.ratingAverage = sr["RatingAverage"].GetType() == typeof(DBNull) ? 0 : (decimal)sr["RatingAverage"];
                    ss.twitterFollowers = sr["TwitterFollowers"].GetType() == typeof(DBNull) ? 0 : (int)sr["TwitterFollowers"];
                    ss.facebookLikes = sr["FacebookLikes"].GetType() == typeof(DBNull) ? 0 : (int)sr["FacebookLikes"];
                    ss.ytVideoCount = sr["YTVideoCount"].GetType() == typeof(DBNull) ? 0 : (int)sr["YTVideoCount"];
                    ss.ytSubscriberCount = sr["YTSubscriberCount"].GetType() == typeof(DBNull) ? 0 : (int)sr["YTSubscriberCount"];


                    // Really only want to do one row!
                    break;
                }
                conn.Close();
            }
            return ss;
        }

        
        /// <summary>
        /// List of the devices with screenshots, built from screenshot meta-data for speed
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static VUI3ScreenshotAnalysisList GetScreenshotPackages(string serviceName)
        {
            VUI3ScreenshotAnalysisList dl = new VUI3ScreenshotAnalysisList();

            string sql = @"select A.Id, A.DateTag , A.PlatformId, A.DeviceId, P.Name as PlatformName, D.Name as DeviceName, A.AnalysisDate, A.HasImages, A.ImageCount, A.DefaultImageId, I.ImageURL_th, I.ImageURL_md, I.ImageURL_lg, I.ImageURL_full
                            from   vui_Analysis A
                            left outer join vui_Image I on A.DefaultImageId = I.Id
                            inner join vui_Platform P on P.ID = A.PlatformId
                            left outer join vui_Device D on A.DeviceId = D.Id
                            where A.ServiceName = @ServiceName";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@ServiceName", serviceName));
                SqlDataReader sr = comm.ExecuteReader();

                if (sr.HasRows)
                {
                    while (sr.Read())
                    {
                        VUI3ScreenshotAnalysis a = new VUI3ScreenshotAnalysis();
                        a.Id = (Int32)sr["Id"];
                        a.DateTag = (string)sr["DateTag"];
                        a.PlatformId = (int)sr["PlatformId"];
                        a.PlatformName = (string)sr["PlatformName"];
                        a.DeviceId = (int)sr["DeviceId"];
                        a.DeviceName = sr["DeviceName"].GetType() == typeof(DBNull) ? String.Empty : (string)sr["DeviceName"];
                        a.AnalysisDate = (DateTime)sr["AnalysisDate"];
                        a.DefaultImageId = (Int32)sr["DefaultImageId"];
                        a.ImageCount = (int)sr["ImageCount"];
                        if (a.DefaultImageId > -1 && a.ImageCount > 0) 
                        {
                            a.DefaultImageURL_th = (string)sr["ImageURL_th"];
                            a.DefaultImageURL_md = (string)sr["ImageURL_md"];
                            a.DefaultImageURL_lg = (string)sr["ImageURL_lg"];
                            a.DefaultImageURL_full = (string)sr["ImageURL_full"];
                        }
                        
                        dl.analyses.Add(a);
                    }
                }
                conn.Close();
            }
            return dl;
        }



        private VUI3ScreenshotAnalysisList _screenshotPackages = null;
        public VUI3ScreenshotAnalysisList ScreenshotPackages
        {
            get
            {
                if (_screenshotPackages == null)
                {
                    _screenshotPackages = GetScreenshotPackages(this.Node.Name);
                }
                return _screenshotPackages;
            }
        }

        public VUI3ScreenshotAnalysis GetScreenshotPackageWithScreenshots(int id)
        {
            if (this.ScreenshotPackages.analyses.Where(a => a.Id == id).Count() > 0)
            {
                VUI3ScreenshotAnalysis analysis = this.ScreenshotPackages.analyses.First(a => a.Id == id);
                analysis.WithScreenshots = true;
                return analysis;
            }
            else
            {
                return null;
            }
        }




        /// <summary>
        /// AppStore Ratings. Most recent
        /// </summary>
        /// <returns></returns>
        public VUI3ServiceRatingsList GetRatings()
        {
            if (Ratings == null)
            {
                Ratings = new VUI3ServiceRatingsList();

                foreach (string device in RatingDeviceTypes)
                {
                    /*
                     * string sql = @" select top 1 Version, ReleaseDate, AverageUserRating, UserRatingCount, DateRecorded, AverageUserRatingForCurrentVersion, UserRatingCountForCurrentVersion, NumDownloads
                                FROM vui_ServiceStoreRating
                                where ServiceId = {0} and DeviceType='{1}' order by DateRecorded Desc";
                     */
                    string sql = @" 
                        select Version, ReleaseDate, AverageUserRating, UserRatingCount, DateRecorded, AverageUserRatingForCurrentVersion, UserRatingCountForCurrentVersion, NumDownloads, ISNULL(NewVersionDate,ReleaseDate) as NewVersionDate   from 
                        (
	                        select top 1 ServiceId, Version, ReleaseDate, AverageUserRating, UserRatingCount, DateRecorded, AverageUserRatingForCurrentVersion, UserRatingCountForCurrentVersion, NumDownloads
	                        FROM vui_ServiceStoreRating
	                        where ServiceId = {0} and DeviceType='{1}' 
	                        order by DateRecorded Desc
                        ) DET
                        LEFT OUTER JOIN
                        (
	                        select top 1 ServiceId, DateRecorded as NewVersionDate
	                        FROM vui_ServiceStoreRating
	                        where ServiceId = {0} and DeviceType='{1}' and NewVersion = 'Y'
	                        order by DateRecorded Desc
                        ) 
                        VERS on DET.ServiceId = VERS.ServiceId;";
                    

                    sql = String.Format(sql, Id.Value, device);

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        SqlDataReader sr = comm.ExecuteReader();

                        if (sr.HasRows)
                        {
                            sr.Read();
                            VUI3ServiceRating r = new VUI3ServiceRating();
                            r.ServiceId = Id.Value;
                            r.Device = device;

                            try
                            {
                                r.DateRecorded = (DateTime)sr["DateRecorded"];
                                r.Version = (string)sr["Version"];
                                r.ReleaseDate = (DateTime)sr["NewVersionDate"]; // ReleaseDate
                                r.AverageUserRating = (sr["AverageUserRating"]).GetType() == typeof(DBNull) ? 0 : decimal.Parse((string)sr["AverageUserRating"]);
                                r.UserRatingCount = (int)sr["UserRatingCount"];
                                r.AverageUserRatingForCurrentVersion = (sr["AverageUserRatingForCurrentVersion"]).GetType() == typeof(DBNull) ? 0 : decimal.Parse((string)sr["AverageUserRatingForCurrentVersion"]);
                                r.UserRatingCountForCurrentVersion = (sr["UserRatingCountForCurrentVersion"]).GetType() == typeof(DBNull) ? 0 : (int)sr["UserRatingCountForCurrentVersion"];
                                r.NumDownloads = (sr["NumDownloads"]).GetType() == typeof(DBNull) ? String.Empty : (string)sr["NumDownloads"];
                            }
                            catch(Exception ex)
                            {
                                log.Error(ex);
                            }
                            Ratings.Ratings.Add(device, r);
                        }
                        conn.Close();
                    }
                }
            }
            return Ratings;
        }
        private VUI3ServiceRatingsList Ratings = null;



        /// <summary>
        /// Handy List of the Device Types to show the ratings for.  May increase to Include Windows Devices soon.
        /// </summary>
        public string[] RatingDeviceTypes = ConfigurationManager.AppSettings["VUI2_RatingDeviceTypes"].ToString().Split(';');

        public DynamicNode Node
        {
            get
            {
                if (_isInitialised)
                {
                    return _node;
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                _node = value;
            }
        }
        public string ServiceName 
        { 
            get 
            {
                if (_isInitialised)
                {
                    return Node.GetProperty("serviceName").ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public int? Id
        {
            get
            {
                if (_isInitialised)
                {
                    return Node.Id;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool IsPreviewable
        {
            get {
                return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
            }
        }
    }

    public class VUI3SimpleServiceMaster
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string URL { get; set; }
        public bool IsPreviewable
        {
            get {
                return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
            }
        }

        public VUI3SimpleServiceMaster() { }
        public VUI3SimpleServiceMaster(int id, string serviceName, string url)
        {
            Id = id;
            ServiceName = serviceName;
            URL = url;
        }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3SimpleServiceMaster>(this);
        }
    }

    public class VUI3SimpleServiceMasterCollection
    {
        public List<VUI3SimpleServiceMaster> ServiceMasters { get; set; }

        public VUI3SimpleServiceMasterCollection()
        {
            ServiceMasters = new List<VUI3SimpleServiceMaster>();
        }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3SimpleServiceMasterCollection>(this);
        }
    }


    public class VUI3AppVersionList
    {
        public int ServiceMasterId { get; set; }
        public string DeviceType { get; set; }
        public List<VUI3AppVersion> Versions { get; set; }

        public VUI3AppVersionList()
        {
            Versions = new List<VUI3AppVersion>();
        }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3AppVersionList>(this);
        }


        public static VUI3AppVersionList GetVersionHistory(int serviceMasterId, string deviceType)
        {

            VUI3AppVersionList versions = new VUI3AppVersionList();
            versions.ServiceMasterId = serviceMasterId;
            versions.DeviceType = deviceType;

            string sql = @" select Version, DateRecorded
                            from  vui_ServiceStoreRating
                            where NewVersion = 'Y'
                            and ServiceID = @serviceid
                            and   DeviceType = @deviceType
                            order by ServiceID, DeviceType, DateRecorded desc";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                SqlCommand comm = new SqlCommand(sql);
                comm.Parameters.Add(new SqlParameter("@serviceid", serviceMasterId));
                comm.Parameters.Add(new SqlParameter("@deviceType", deviceType));
                comm.Connection = conn;
                conn.Open();

                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    VUI3AppVersion v = new VUI3AppVersion();
                    v.Version = (string)sr["Version"];
                    v.DateRecorded = (DateTime)sr["DateRecorded"];
                    versions.Versions.Add(v);
                }

                conn.Close();
            }
            return versions;
        }
    }

    public class VUI3AppVersion
    {
        public string Version { get; set; }
        public DateTime DateRecorded { get; set; }
        public string DateRecordedString
        {
            get
            {
                return DateRecorded.ToString("dd MMM yyyy");
            }
        }
        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3AppVersion>(this);
        }
    }
}