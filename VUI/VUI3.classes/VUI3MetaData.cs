using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Configuration;
using VUI.VUI2.classes;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;

namespace VUI.VUI3.classes
{
    public class VUI3MetaData
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3MetaData));


        private static string VUI_mediafolder;
        private static bool EncryptImageURLs;
        private static Dictionary<string, int> _analyses;
        private static string _currentService = String.Empty;

        public static string CurrentService
        {
            get
            {
                if (String.IsNullOrEmpty(_currentService))
                {
                    return "Idle";
                }
                else
                {
                    return "Processing " + _currentService;
                }
            }
        }

        /// <summary>
        /// Static Constructor
        /// </summary>
        static VUI3MetaData()
        {
            VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");
            EncryptImageURLs = (Utility.GetConst("VUI_EncryptImageURLs").Equals("YES"));
        }

        /// <summary>
        /// Add an entry to the Queue!
        /// </summary>
        /// <param name="servicename"></param>
        public static void AddToQueue(string servicename)
        {
            string sql = @"insert into vui_MetaDataQueue (ServiceMasterName, Status) values (@ServiceMasterName,'N');";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.Text;
                comm.CommandText = sql;
                comm.Parameters.AddWithValue("@ServiceMasterName", servicename);
                comm.Connection = conn;
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }


        /// <summary>
        /// NOT YET USED
        /// Publish the agglomerated screenshot metadata for a specific service
        /// </summary>
        /// <param name="serviceId"></param>
        public static void PublishScreenshotMetaData(int serviceId)
        {

        }

        /// <summary>
        /// NOT YET USED
        /// </summary>
        /// <param name="serviceName"></param>
        public static void PublishScreenshotMetaData(string serviceName)
        {
        //    sbSQL.AppendLine(@"exec vui_RefreshSingleServiceScreenshotsMetaData @ServiceName ");
        //    VUIDataFunctions.ExecQuery(sbSQL.ToString());
        }


        /// <summary>
        /// Publish the metadata for a single image (USED IN CATEGORISING IMAGES APP)
        /// </summary>
        /// <param name="id"></param>
        public static void PublishImageMetadata(int id)
        {
            DynamicNode image = new DynamicNode(id);
            int analysisId = image.Parent.Id;
            int serviceId = image.Parent.Parent.Id;
            int isPreviewable = 0;

            DynamicNode service = new DynamicNode(serviceId);
            if (service.Name.Equals(Utility.GetConst("VUI_previewservice")))
            {
                isPreviewable = 1;
            }

            DateTime createDate = image.CreateDate;
            string pagetype = image.GetProperty("pageType").Value;
            string ImageURL_th = VUI_mediafolder + @"th/" + image.GetProperty("thFile").Value.Replace("&", "%26");
            string ImageURL_md = VUI_mediafolder + @"md/" + image.GetProperty("thFile").Value.Replace("&", "%26");
            string ImageURL_lg = VUI_mediafolder + @"lg/" + image.GetProperty("lgFile").Value.Replace("&", "%26");
            string ImageURL_full = VUI_mediafolder + @"full/" + image.GetProperty("imageFile").Value.Replace("&", "%26");

            // This is for the New Handling URLs which decrypt on opening.  Note that the Preview Service is not encrypted for simplicity.
            if (EncryptImageURLs && isPreviewable == 0)
            {
                ImageURL_th = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_th);
                ImageURL_md = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_md);
                ImageURL_lg = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_lg);
                ImageURL_full = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_full);
            }

            string sql = @"
                IF ( EXISTS (select Id from vui_Image where Id=@Id))
                begin
                    update vui_Image set
                        ServiceId = @ServiceId
                    ,   AnalysisId = @AnalysisId
                    ,   PageType = @PageType
                    ,   ImageURL_th = @ImageURL_th
                    ,   ImageURL_md = @ImageURL_md
                    ,   ImageURL_lg = @ImageURL_lg
                    ,   ImageURL_full = @ImageURL_full
                    ,   DateCreated = @DateCreated
                    where Id = @Id
                    ;
                end
                ELSE
                begin
                    insert into vui_Image (Id, ServiceId, AnalysisId, PageType, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, DateCreated)
                    values (@Id, @ServiceId, @AnalysisId, @PageType, @ImageURL_th, @ImageURL_md, @ImageURL_lg, @ImageURL_full, @DateCreated)
                    ;
                end
                ";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@Id", id));
                comm.Parameters.Add(new SqlParameter("@ServiceId", serviceId));
                comm.Parameters.Add(new SqlParameter("@AnalysisId", analysisId));
                comm.Parameters.Add(new SqlParameter("@PageType", pagetype));
                comm.Parameters.Add(new SqlParameter("@ImageURL_th", ImageURL_th));
                comm.Parameters.Add(new SqlParameter("@ImageURL_md", ImageURL_md));
                comm.Parameters.Add(new SqlParameter("@ImageURL_lg", ImageURL_lg));
                comm.Parameters.Add(new SqlParameter("@ImageURL_full", ImageURL_full));
                comm.Parameters.Add(new SqlParameter("@DateCreated", createDate));
                comm.ExecuteNonQuery();
                conn.Close();
            }
            
        }


        /// <summary>
        /// This is used to trigger the Stored Proc
        /// </summary>
        /// <param name="id"></param>
        /// <param name="recurse"></param>
        public static void PublishAllAnalysisMetadata()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandText = "vui_GenerateBenchmarkScores";
                comm.Connection = conn;
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Call to clear out ALL metadata. May cause system instability.
        /// </summary>
        public static void ClearAllMetadata()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandText = "vui_ClearAllMetaData";
                comm.Connection = conn;
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }


        /// <summary>
        /// Not to be used lightly! This will regenerate all the Service Metadata
        /// </summary>
        public static void PublishAllServiceMetadata()
        {
            _analyses = new Dictionary<string, int>();

            DynamicNode smRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);

            List<DynamicNode> serviceMasters = smRoot.Descendants("VUI2ServiceMaster").Items.OrderBy(n => n.Name).ToList();

            foreach (DynamicNode serviceMaster in serviceMasters)
            {
                PublishServiceMetadata(serviceMaster.Name, false);
            }

            foreach (string k in _analyses.Keys)
            {
                log.Debug("****** Analysis: " + k + " [" + _analyses[k] + "] ");
            }

            PublishAllAnalysisMetadata();
        }

        /// <summary>
        /// CALLED BY QUEUE
        /// </summary>
        /// <param name="ServiceMasterName"></param>
        /// <param name="recurse"></param>
        public static void PublishServiceMetadata(string ServiceMasterName, bool triggerAnalysisMetadataPublish)
        {
            _currentService = ServiceMasterName;

            DynamicNode root = new DynamicNode(ConfigurationManager.AppSettings["VUI2_rootnodeid"]);
            log.Debug("****** Got dynamic root node");
            // First regenerate the Device and Platform Metadata
            

            DynamicNode smRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
            log.Debug("****** Got ServiceMaster root node");

            #region Platform/Device MetaData

            List<DynamicNode> platforms = root.Descendants(Utility.GetConst("VUi2_platformtype")).Items.ToList();
            log.Debug("****** Got Platforms root node");

            List<DynamicNode> services = new List<DynamicNode>(); 

            StringBuilder sbSQL = new StringBuilder();


            foreach (DynamicNode platform in platforms)
            {
                log.Debug("***** - Found Platform: " + platform.Id + " [" + platform.Name + "]");
                sbSQL.AppendLine(
                    String.Format(
                        @"IF ( EXISTS (select Id from vui_Platform where ID={0}))
                    begin
                    update vui_Platform
                    set    Name = '{1}'
                    where  ID = {0};
                    end
                    else
                    insert into vui_Platform (ID,Name) values ({0}, '{1}'); ", new Object[] { platform.Id, platform.Name }));

                // Find Devices, Or Services, Children
                List<DynamicNode> devices = platform.Descendants(Utility.GetConst("VUi2_devicetype")).Items.ToList();

                if (devices.Count > 0)
                {
                    foreach (DynamicNode device in devices)
                    {
                        log.Debug("***** -- Found Device: " + device.Id + " [" + device.Name + "]");
                        sbSQL.AppendLine(
                            String.Format(
                                @"IF ( EXISTS (select Id from vui_Device where ID={0}))
                                    begin
                                    update vui_Device
                                    set    Name = '{1}'
                                    ,      PlatformId = {2}
                                    where  ID = {0};
                                    end
                                    else
                                    insert into vui_Device (ID,Name,PlatformId) values ({0}, '{1}', {2}); ", new Object[] { device.Id, device.Name, platform.Id }));

                        services.AddRange(device.Descendants(Utility.GetConst("VUi2_servicetype")).Items.ToList().Where(n => n.Name.Equals(ServiceMasterName)).ToList());
                            
                    }
                }
                else
                {
                    services.AddRange(platform.Descendants(Utility.GetConst("VUi2_servicetype")).Items.ToList().Where(n => n.Name.Equals(ServiceMasterName)).ToList());
                }
            }
            
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.Text;
                comm.CommandText = sbSQL.ToString();
                comm.Connection = conn;
                comm.Prepare();
                comm.ExecuteNonQuery();
                conn.Close();
            }
            #endregion

            #region Service MetaData

            log.Debug("***** -------------------------------------------- ");
            log.Debug("***** - Generating Service MetaData for " + ServiceMasterName);
            log.Debug("***** -------------------------------------------- ");

            // Tidy Up First

            string tidysql = @"delete from vui_ServiceBenchmarkFeatures where ServiceName='{0}'; ";
            tidysql = String.Format(tidysql, ServiceMasterName);

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.Text;
                comm.CommandText = tidysql;
                comm.Connection = conn;
                comm.Prepare();
                comm.ExecuteNonQuery();
                conn.Close();
            }

            foreach (DynamicNode service in services)
            {
                int deviceId;
                int platformId;
                string platformName;
                if (service.Parent.NodeTypeAlias.Equals("VUI2Device"))
                {
                    deviceId = service.Parent.Id;
                    platformId = service.Parent.Parent.Id;
                    platformName = service.Parent.Parent.Name + "/" + service.Parent.Name;
                }
                else
                {
                    deviceId = -1;
                    platformId = service.Parent.Id;
                    platformName = service.Parent.Name;
                }

                log.Debug("***** -- Found on: " + platformName);
                string sql = GetServiceMetaSQL(service, platformId, deviceId);

                // Run SQL
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = sql;
                    comm.Connection = conn;
                    comm.Prepare();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            #endregion


            #region SM MetaData
            try
            {
                DynamicNode serviceMaster = smRoot.Descendants("VUI2ServiceMaster").Items.First(n => n.Name.Equals(ServiceMasterName));


                string iconpath = String.Empty;
                if (serviceMaster.GetProperty("serviceIcon") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("serviceIcon").Value))
                {
                    iconpath = serviceMaster.GetProperty("serviceIcon").Value.ToString();
                }

                string sql = @"IF(EXISTS(select ID from vui_ServiceMasters where ID = {0}))
                               begin
                                 update vui_ServiceMasters
                                 set ServiceName = '{1}'
                                 ,   twitterURL = '{2}'
                                 ,   facebookURL = '{3}'
                                 ,   youTubeURL = '{4}'
                                 ,   iPhoneAppURL = '{5}'
                                 ,   iPadAppURL = '{6}'
                                 ,   phonePlayAppURL = '{7}'
                                 ,   tabletPlayAppURL = '{8}'
                                 ,   phoneWindowsAppURL = '{9}'
                                 ,   iconURL =  {10}
                                 where ID = {0};
                               end
                               ELSE
                                 insert into vui_ServiceMasters 
                                  (ID, ServiceName, twitterURL, facebookURL, youTubeURL, iPhoneAppURL, iPadAppURL, phonePlayAppURL, tabletPlayAppURL, phoneWindowsAppURL, IconURL)
                                  values ( {0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10} ); ";

                sql = String.Format(sql, new Object[] {   serviceMaster.Id
                                                        , serviceMaster.Name
                                                        , serviceMaster.GetProperty("twitterURL").Value
                                                        , serviceMaster.GetProperty("facebookURL").Value
                                                        , serviceMaster.GetProperty("youTubeURL").Value
                                                        , serviceMaster.GetProperty("iPhoneAppURL").Value
                                                        , serviceMaster.GetProperty("iPadAppURL").Value
                                                        , serviceMaster.GetProperty("phonePlayAppURL").Value
                                                        , serviceMaster.GetProperty("tabletPlayAppURL").Value
                                                        , serviceMaster.GetProperty("phoneWindowsAppURL").Value
                                                        , (String.IsNullOrEmpty(iconpath) ? "NULL" : @"'"+iconpath+@"'")
                                                      });


                // Insert / Update into the Brand Table
                sql = sql + @"IF NOT EXISTS (SELECT * FROM BRAND WHERE ID = " + serviceMaster.Id + @") 
                                  INSERT INTO BRAND(ID, BRANDNAME) VALUES (" + serviceMaster.Id + @" ,'" + serviceMaster.Name + @"'); ";

                if (serviceMaster.GetProperty("twitterURL") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("twitterURL").Value))
                {
                    // Get the handle from the Twitter URL
                    Regex re = new Regex(@"http(\w?)://(.*?)/(\w+)$");
                    Match m = re.Match(serviceMaster.GetProperty("twitterURL").Value);
                    string handle = m.Groups[3].Value;
                    if (!String.IsNullOrEmpty(handle))
                    {
                        sql = sql + @"IF EXISTS (SELECT * FROM BrandTwitter WHERE BRAND_ID=" + serviceMaster.Id + @")
                                            UPDATE BrandTwitter SET Handle = '" + handle + @"' WHERE BRAND_ID=" + serviceMaster.Id + @"
                                        ELSE
                                            INSERT INTO BrandTwitter (BRAND_ID, Handle) VALUES (" + serviceMaster.Id + @",'" + handle + @"');";
                    }
                }
                if (serviceMaster.GetProperty("facebookURL") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("facebookURL").Value))
                {
                    // Get the handle from the Facebook URL
                    Regex re = new Regex(@"http(\w?)://(.*?)/(\w+)$");
                    Match m = re.Match(serviceMaster.GetProperty("facebookURL").Value);
                    string handle = m.Groups[3].Value;
                    if (!String.IsNullOrEmpty(handle))
                    {
                        sql = sql + @"IF EXISTS (SELECT * FROM BrandFacebook WHERE BRAND_ID=" + serviceMaster.Id + @")
                                            UPDATE BrandFacebook SET Handle = '" + handle + @"' WHERE BRAND_ID=" + serviceMaster.Id + @"
                                        ELSE
                                            INSERT INTO BrandFacebook (BRAND_ID, Handle) VALUES (" + serviceMaster.Id + @",'" + handle + @"');";
                    }
                }
                if (serviceMaster.GetProperty("youTubeURL") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("youTubeURL").Value))
                {
                    // Get the handle from the Youtube URL
                    Regex re = new Regex(@"http(\w?)://(.*?)/(\w+)$");
                    Match m = re.Match(serviceMaster.GetProperty("youTubeURL").Value);
                    string handle = m.Groups[3].Value;
                    if (!String.IsNullOrEmpty(handle))
                    {
                        sql = sql + @"IF EXISTS (SELECT * FROM BrandChannel WHERE BRAND_ID=" + serviceMaster.Id + @")
                                            UPDATE BrandChannel SET ChannelName = '" + handle + @"' WHERE BRAND_ID=" + serviceMaster.Id + @"
                                        ELSE
                                            INSERT INTO BrandChannel (BRAND_ID, ChannelName) VALUES (" + serviceMaster.Id + @",'" + handle + @"');";
                    }
                }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = sql;
                    comm.Connection = conn;
                    comm.Prepare();
                    comm.ExecuteNonQuery();

                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.CommandText = "vui_CreateServiceMasterSnapshot";
                    comm.Parameters.Add(new SqlParameter("@ServiceName", System.Data.SqlDbType.NVarChar));
                    comm.Parameters["@ServiceName"].Size = 80;
                    comm.Parameters["@ServiceName"].Value = ServiceMasterName;
                    comm.Prepare();
                    comm.ExecuteNonQuery();


                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.CommandText = "sp_SetServiceLogo";
                    comm.Parameters["@ServiceName"].Value = ServiceMasterName;
                    comm.Prepare();
                    comm.ExecuteNonQuery();

                    conn.Close();
                }

                if (triggerAnalysisMetadataPublish)
                {
                    PublishAllAnalysisMetadata();
                }

            }
            catch (Exception ex)
            {
                log.Error("***** - Error Generating Metadata for : " + ServiceMasterName, ex);
            }
            #endregion

            _currentService = String.Empty;
        }



        public static void PublishServiceMetadata(int id, bool recurse)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="platformId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private static string GetServiceMetaSQL(DynamicNode service, int platformId, int deviceId)
        {
            string serviceScore = "0";
            string tempServiceScore = "0";
            string appStoreURL = "";
            int isPreviewable = 0;
            int screenshotCount = 0;

            if (service.Name.Equals(Utility.GetConst("VUI_previewservice")))
            {
                isPreviewable = 1;
            }

            bool EncryptImageURLs = (Utility.GetConst("VUI_EncryptImageURLs").Equals("YES"));

            // Get Analyses
            List<DynamicNode> analyses = service.Descendants(Utility.GetConst("VUi2_analysistype")).Items.ToList();

            DateTime benchmarkDate;
            DateTime mostRecentBenchmark = new DateTime(1990, 1, 1);
            int analysisToScoreId = -1;
            int tempAnalysisId = -1;

            string defaultScreenshotID = "-1";
            if (service.HasProperty("defaultScreenshot") && !String.IsNullOrEmpty(service.GetProperty("defaultScreenshot").Value))
            {
                defaultScreenshotID = service.GetProperty("defaultScreenshot").Value;
            }
            if (service.HasProperty("appStoreURL"))
            {
                appStoreURL = service.GetProperty("appStoreURL").Value;
            }

            string ansql = "";


            // For new Benchmark DataPoints
            string[] benchmarkFeatures = { "" };
            string[] hotFeatures = { "" };

            string benchmarkDataSql = "";

            foreach (DynamicNode analysis in analyses)
            {
                int defaultImageId = -1;
                bool hasImages = false;
                int imageCount = 0;
                int benchmarkScore = 0;


                Int32.TryParse(analysis.GetProperty("serviceScore").Value, out benchmarkScore);

                if (analysis.Descendants().Items.Count() > 0)
                {
                    hasImages = true;
                    imageCount = analysis.Descendants().Items.Count;
                    defaultImageId = analysis.Descendants(ConfigurationManager.AppSettings["VUI2_screenshottype"].ToString()).Items.First().Id;
                }
                string asql = @"IF(EXISTS( select ServiceName from vui_Analysis where id = {0}))
                                begin
                                  update vui_Analysis
                                  set    ServiceName = '{1}'
                                  ,      PlatformId = {2}
                                  ,      DeviceId = {3}
                                  ,      DateTag = '{4}'
                                  ,      HasImages = '{5}'
                                  ,      AnalysisDate = '{6}'
                                  ,      DefaultImageID = {7}
                                  ,      ImageCount = {8}
                                  ,      BenchmarkScore = {9}
                                  where id = {0}
                                end
                                ELSE
                                  insert into vui_Analysis (id, ServiceName, PlatformId, DeviceId, DateTag, HasImages, AnalysisDate, DefaultImageID, ImageCount, BenchmarkScore) values ({0},'{1}', {2}, {3}, '{4}', '{5}', '{6}', {7}, {8}, {9}); ";


                log.Debug("***** ---- Analysis:" + analysis.Id + " [" + analysis.Name + "]");
                log.Debug("***** ----- Date:  [" + analysis.GetProperty("analysisDate").Value + "]");

                asql = String.Format(asql, new object[] { analysis.Id, service.Name, platformId, deviceId, analysis.Name, hasImages ? 'Y' : 'N', DateTime.Parse(analysis.GetProperty("analysisDate").Value).ToString("yyyy-MM-dd HH:mm:ss"), defaultImageId, imageCount, benchmarkScore });
                ansql += asql;

                if (analysis.HasProperty("serviceScore"))
                {
                    if (!String.IsNullOrEmpty(analysis.GetProperty("serviceScore").Value))
                    {
                        tempServiceScore = analysis.GetProperty("serviceScore").Value;
                        tempAnalysisId = analysis.Id;

                        if (analysis.HasProperty("benchmarkDate") && !String.IsNullOrEmpty(analysis.GetProperty("benchmarkDate").Value))
                        {
                            benchmarkDate = DateTime.Parse(analysis.GetProperty("benchmarkDate").Value);

                            // If Compare > 0 then benchmarkDate is more recent
                            if (benchmarkDate.CompareTo(mostRecentBenchmark) >= 0)
                            {
                                mostRecentBenchmark = benchmarkDate;
                                serviceScore = tempServiceScore;
                                analysisToScoreId = tempAnalysisId;
                                benchmarkFeatures = analysis.GetProperty("serviceCapabilities").Value.Split(',');
                                hotFeatures = analysis.GetProperty("hotFeatures").Value.Split(',');
                            }
                        }
                        else
                        {
                            log.Debug("***** ----- No Benchmark Date");
                        }
                    }
                    else
                    {
                        log.Debug("***** ----- No Benchmark Score");
                    }
                }
                else
                {
                    log.Debug("***** ----- No Benchmark Score");
                }
                screenshotCount += analysis.Descendants(Utility.GetConst("VUI2_screenshottype")).Items.Count;
                if (defaultScreenshotID.Equals("-1"))
                {
                    try
                    {
                        DynamicNode firstScreenshot = analysis.Descendants(Utility.GetConst("VUI2_screenshottype")).Items.First();
                        if (firstScreenshot != null)
                        {
                            defaultScreenshotID = firstScreenshot.Id.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        log.Debug("***** ----- No screenshots in this analysis");
                    }
                }
            }



            log.Debug("***** ----- Score: " + serviceScore + ";" + mostRecentBenchmark);

            
            if (analysisToScoreId != -1)
            {
                benchmarkDataSql = GetAnalysisScoreSQL(analysisToScoreId, platformId, deviceId);
                benchmarkDataSql = benchmarkDataSql + GetBenchmarkDataSQL(benchmarkFeatures, service.Id, service.Name, platformId, deviceId);
                benchmarkDataSql = benchmarkDataSql + GetBenchmarkDataSQL(hotFeatures, service.Id, service.Name, platformId, deviceId);
            }
             

            string sql = String.Format(@"IF(EXISTS(select Name from vui_Service where ID = {0}))
                                        begin
                                          update vui_Service
                                          set   Name = '{1}'
                                          ,     ServiceName = '{2}'
                                          ,     PlatformId = {3}
                                          ,     DeviceId = {4}
                                          ,     BenchmarkScore = {5}
                                          ,     DefaultScreenshotID = {6}
                                          ,     IsPreviewable = {7}
                                          ,     ScreenshotCount = {8}
                                          ,     AppStoreURL = '{9}'
                                          where ID = {0};
                                        end
                                        else
                                          insert into vui_Service (ID,Name,ServiceName,PlatformId,DeviceId,BenchmarkScore,DefaultScreenshotID,IsPreviewable,ScreenshotCount,AppStoreURL) values ({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8}, '{9}'); "
                        , new Object[] { service.Id, service.Name, Utility.NiceUrlName(service.Id), platformId, deviceId, serviceScore, defaultScreenshotID, isPreviewable, screenshotCount, appStoreURL });

            string VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");

            // Image metadata
            List<DynamicNode> images = service.Descendants("VUI_Image").Items;
            foreach (DynamicNode image in images)
            {
                int imageid = image.Id;

                int analysisId = image.Parent.Id;

                DateTime createDate = image.CreateDate;

                string pagetype = image.GetProperty("pageType").Value;

                string ImageURL_th = VUI_mediafolder + @"th/" + image.GetProperty("thFile").Value.Replace("&", "%26");
                string ImageURL_md = VUI_mediafolder + @"md/" + image.GetProperty("thFile").Value.Replace("&", "%26");
                string ImageURL_lg = VUI_mediafolder + @"lg/" + image.GetProperty("lgFile").Value.Replace("&", "%26");
                string ImageURL_full = VUI_mediafolder + @"full/" + image.GetProperty("imageFile").Value.Replace("&", "%26");

                // This is for the New Handling URLs which decrypt on opening.  Note that the Preview Service is not encrypted for simplicity.
                if (EncryptImageURLs && isPreviewable == 0)
                {
                    ImageURL_th = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_th);
                    ImageURL_md = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_md);
                    ImageURL_lg = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_lg);
                    ImageURL_full = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_full);
                }

                string tmp = String.Format(@"IF(EXISTS(select AnalysisId from vui_Image where Id = {0}))
                                            begin
                                              update vui_Image 
                                              set   ServiceId = {1}
                                              ,     PageType = '{2}'
                                              ,     ImageURL_th = '{3}'
                                              ,     ImageURL_md = '{4}'
                                              ,     ImageURL_lg = '{5}'
                                              ,     ImageURL_full = '{6}'
                                              ,     DateCreated = '{7}'
                                              ,     AnalysisId = {8}
                                              where Id = {0}; 
                                            end 
                                            ELSE
                                              insert into vui_Image (Id, ServiceId, PageType, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, DateCreated, AnalysisId) values ({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}', {8}); "
                            , new Object[] { imageid, service.Id, pagetype, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, createDate.ToString("yyyy-MM-dd HH:mm:ss.0"), analysisId });

                sql += tmp;
            }

            sql += ansql;
            sql += benchmarkDataSql;

            // log.Debug(sql);
            return sql;

        }


        private static string GetBenchmarkDataSQL(string[] benchmarkFeatures, int serviceId, string serviceName, int platformId, int deviceId)
        {
            StringBuilder sql = new StringBuilder("");
            foreach (string feature in benchmarkFeatures)
            {
                sql.Append(String.Format(@"insert into vui_ServiceBenchmarkFeatures (ServiceId, ServiceName, PlatformId, DeviceId, Feature) values ({0}, '{1}', {2}, {3}, '{4}'); ", new object[] { serviceId, serviceName, platformId, deviceId, feature }));
            }
            return sql.ToString();
        }


        /// <summary>
        /// Gets the Score SQL for an analysis. This populates an interim table that is used to build up vui_BenchmarkScores
        /// </summary>
        /// <param name="analysisId"></param>
        /// <param name="platformId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private static string GetAnalysisScoreSQL(int analysisId, int platformId, int deviceId)
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();

            // Set up the Scores Dictionary
            XPathNodeIterator iterator = umbraco.library.GetPreValues(Int32.Parse(Utility.GetConst("VUI_function_list")));
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");
            while (preValues.MoveNext())
            {
                scores.Add(preValues.Current.Value.Trim(), 0);
            }
            iterator = umbraco.library.GetPreValues(Int32.Parse(Utility.GetConst("VUI_hotfeatures_list")));
            iterator.MoveNext(); //move to first
            preValues = iterator.Current.SelectChildren("preValue", "");
            while (preValues.MoveNext())
            {
                scores.Add(preValues.Current.Value.Trim(), 0);
            }

            Analysis a = new Analysis(analysisId);
            a.SetBenchmark();

            log.Debug("***** ----- Analysis [" + analysisId + "] ");

            try
            {
                _analyses.Add(a.Node.Parent.Name + "/" + a.Node.Name, analysisId);
            }
            catch (Exception ex) { ; }

            if (a.Capabilities != null)
            {
                foreach (string c in a.Capabilities)
                {
                    try
                    {
                        scores[c] += 1;
                    }
                    catch (Exception ex1)
                    { ;}
                }
            }
            if (a.HotFeatures != null)
            {
                foreach (string c in a.HotFeatures)
                {
                    try
                    {
                        scores[c] += 1;
                    }
                    catch (Exception ex1)
                    { ;}
                }
            }

            StringBuilder sq = new StringBuilder();
            foreach (string feature in scores.Keys)
            {
                string sql = @" IF(EXISTS(select score from vui_AnalysisBenchmarkScore where AnalysisId={0} and feature='{1}'))
                                begin
                                  update vui_AnalysisBenchmarkScore 
                                  set    PlatformID = {2}
                                  ,      DeviceID   = {3}
                                  ,      score      = {4}
                                  where  AnalysisId={0} and feature='{1}'; 
                                end
                                ELSE
                                  insert into vui_AnalysisBenchmarkScore (AnalysisId, feature, PlatformID, DeviceID, score)
                                  values ({0},'{1}',{2},{3},{4});
                                ";
                sql = String.Format(sql, new object[] { analysisId, feature, platformId, deviceId==-1 ? "NULL" : deviceId.ToString(), scores[feature] });
                sq.AppendLine(sql);
            }
            return sq.ToString();
        }


        public const string ALL_METADATA = "ALL";
        public const string CLEAR_METADATA = "CLEAR";

    }
}