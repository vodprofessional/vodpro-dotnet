using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Text;
using System.Xml.XPath;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using System.Text.RegularExpressions;
using System.Globalization;

namespace VUI.VUI2.classes
{
    public static class MetaData
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MetaData));
        private static List<int> _analysisIDs;

        public static void RegenerateMetaData()
        {
            
            StringBuilder sbSQL = new StringBuilder();

            
            sbSQL.Clear();


            sbSQL.AppendLine(@"truncate table vui_Platform; ");
            sbSQL.AppendLine(@"truncate table vui_Device; ");
            sbSQL.AppendLine(@"truncate table vui_Service; ");
            sbSQL.AppendLine(@"truncate table vui_Image; ");
            sbSQL.AppendLine(@"truncate table vui_Analysis; ");
            sbSQL.AppendLine(@"truncate table vui_BenchmarkScores; ");

            // Start at root node
            DynamicNode root = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));

            // Find Platform Children
            List<DynamicNode> platforms = root.Descendants(Utility.GetConst("VUi2_platformtype")).Items.ToList();
            Dictionary<string, int> scores;

            log.Debug("**********************************");
            log.Debug("Regenerating Metadata");
            log.Debug("**********************************");
            
            foreach (DynamicNode platform in platforms)
            {
                log.Debug(" - Platform: " + platform.Id + " [" + platform.Name + "]");
                sbSQL.AppendLine(String.Format(@"insert into vui_Platform (ID,Name) values ({0}, '{1}'); ", new Object[]{ platform.Id, platform.Name }));
                
                // Find Devices, Or Services, Children
                List<DynamicNode> devicesOrServices = platform.GetChildrenAsList.Items.ToList();

                _analysisIDs = new List<int>();
                foreach (DynamicNode deviceOrService in devicesOrServices)
                {
                    log.Debug(" -- " + deviceOrService.NodeTypeAlias + " - : " + deviceOrService.Id + " [" + deviceOrService.Name + "]");
                
                    if (deviceOrService.NodeTypeAlias.Equals(Utility.GetConst("VUi2_devicetype")))
                    {
                        sbSQL.AppendLine(String.Format(@"insert into vui_Device (ID,Name,PlatformId) values ({0}, '{1}', {2}); ", new Object[] { deviceOrService.Id, deviceOrService.Name, platform.Id }));

                        List<DynamicNode> services = deviceOrService.GetChildrenAsList.Items.ToList();

                        _analysisIDs = new List<int>();
                        foreach (DynamicNode service in services)
                        {
                            sbSQL.AppendLine(CreateServiceMetaSQL(service, platform.Id, deviceOrService.Id));
                        }
                        scores = ScoresForAnalysis();
                        foreach (string key in scores.Keys)
                        {
                            sbSQL.AppendLine(String.Format(@"insert into vui_BenchmarkScores (PlatformID, DeviceId, feature, score) values ({0},{1},'{2}',{3}); ", new Object[] { platform.Id, deviceOrService.Id, key, scores[key] }));
                        }
                        _analysisIDs = new List<int>();
                    }
                    else if (deviceOrService.NodeTypeAlias.Equals(Utility.GetConst("VUi2_servicetype")))
                    {
                        sbSQL.AppendLine(CreateServiceMetaSQL(deviceOrService, platform.Id, -1));
                    }
                    scores = ScoresForAnalysis();
                    foreach (string key in scores.Keys)
                    {
                        sbSQL.AppendLine(String.Format(@"insert into vui_BenchmarkScores (PlatformID, feature, score) values ({0},'{1}',{2}); ", new Object[] { platform.Id, key, scores[key] }));
                    }
                    _analysisIDs = new List<int>();
                }
            }

            // This stored Proc generates the Screenshot MetaData that is used by the top-level screenshot library pages.
            sbSQL.AppendLine(@"exec vui_RefreshServiceScreenshotsMetaData; ");
            VUIDataFunctions.ExecQuery(sbSQL.ToString());

            sbSQL.Clear();

            int VUI2_ServiceMastersRoot = Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString());

            DynamicNode smRoot = new DynamicNode(VUI2_ServiceMastersRoot);
            List<DynamicNode> serviceMasters = smRoot.Descendants().Items;

            sbSQL.AppendLine("truncate table vui_ServiceMasters; ");

            foreach (DynamicNode serviceMaster in serviceMasters)
            {

                string iconpath = String.Empty;
                if (serviceMaster.GetProperty("serviceIcon") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("serviceIcon").Value))
                {
                    iconpath = serviceMaster.GetProperty("serviceIcon").Value.ToString();
                }

                string sql = @"insert into vui_ServiceMasters 
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
                string sql2 = @"IF NOT EXISTS (SELECT * FROM BRAND WHERE ID = " + serviceMaster.Id + @") 
                                  INSERT INTO BRAND(ID, BRANDNAME) VALUES (" + serviceMaster.Id + @" ,'" + serviceMaster.Name + @"'); ";

                if(serviceMaster.GetProperty("twitterURL") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("twitterURL").Value))
                {
                    // Get the handle from the Twitter URL
                    Regex re = new Regex(@"http(\w?)://(.*?)/(\w+)$");
                    Match m = re.Match(serviceMaster.GetProperty("twitterURL").Value);
                    string handle = m.Groups[3].Value;
                    if(!String.IsNullOrEmpty(handle))
                    {
                        sql2 = sql2 + @"IF EXISTS (SELECT * FROM BrandTwitter WHERE BRAND_ID=" + serviceMaster.Id + @")
                                            UPDATE BrandTwitter SET Handle = '" + handle + @"' WHERE BRAND_ID=" + serviceMaster.Id + @"
                                        ELSE
                                            INSERT INTO BrandTwitter (BRAND_ID, Handle) VALUES (" + serviceMaster.Id + @",'" + handle + @"');";
                    }
                }
                if(serviceMaster.GetProperty("facebookURL") != null && !String.IsNullOrEmpty(serviceMaster.GetProperty("facebookURL").Value))
                {
                    // Get the handle from the Facebook URL
                    Regex re = new Regex(@"http(\w?)://(.*?)/(\w+)$");
                    Match m = re.Match(serviceMaster.GetProperty("facebookURL").Value);
                    string handle = m.Groups[3].Value;
                    if(!String.IsNullOrEmpty(handle))
                    {
                        sql2 = sql2 + @"IF EXISTS (SELECT * FROM BrandFacebook WHERE BRAND_ID=" + serviceMaster.Id + @")
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
                    if(!String.IsNullOrEmpty(handle))
                    {
                        sql2 = sql2 + @"IF EXISTS (SELECT * FROM BrandChannel WHERE BRAND_ID=" + serviceMaster.Id + @")
                                            UPDATE BrandChannel SET ChannelName = '" + handle + @"' WHERE BRAND_ID=" + serviceMaster.Id + @"
                                        ELSE
                                            INSERT INTO BrandChannel (BRAND_ID, ChannelName) VALUES (" + serviceMaster.Id + @",'" + handle + @"');";
                    }
                }
                
                sbSQL.AppendLine(sql);
                sbSQL.AppendLine(sql2);
            }
            
            sbSQL.AppendLine("exec vui_CreateAllServiceMasterSnapshots");
            sbSQL.AppendLine("exec sp_SetServiceLogos");
            VUIDataFunctions.ExecQuery(sbSQL.ToString());
            

        }

        private static string CreateServiceMetaSQL(DynamicNode service, int platformId, int deviceId)
        {
            log.Debug(" --- Service:" + service.Id + " [" + service.Name + "]");

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
            DateTime mostRecentBenchmark = new DateTime(1990,1,1);
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

            

            foreach(DynamicNode analysis in analyses)
            {
                int defaultImageId = -1;
                bool hasImages = false;
                int imageCount = 0;
                int benchmarkScore = 0;

                Int32.TryParse(analysis.GetProperty("serviceScore").Value, out benchmarkScore);

                if(analysis.Descendants().Items.Count()>0)
                {
                    hasImages = true;
                    imageCount = analysis.Descendants().Items.Count;
                    defaultImageId = analysis.Descendants(ConfigurationManager.AppSettings["VUI2_screenshottype"].ToString()).Items.First().Id;
                }
                string asql = @"insert into vui_Analysis (id, ServiceName, PlatformId, DeviceId, DateTag, HasImages, AnalysisDate, DefaultImageID, ImageCount, BenchmarkScore) values ({0},'{1}', {2}, {3}, '{4}', '{5}', '{6}', {7}, {8}, {9}); ";


                log.Debug(" ---- Analysis:" + analysis.Id + " [" + analysis.Name + "]");
                log.Debug(" ----- Date:  [" + analysis.GetProperty("analysisDate").Value + "]");

                asql = String.Format(asql, new object[] { analysis.Id, service.Name, platformId, deviceId, analysis.Name, hasImages ? 'Y':'N', DateTime.Parse(analysis.GetProperty("analysisDate").Value).ToString("yyyy-MM-dd HH:mm:ss"), defaultImageId, imageCount, benchmarkScore });
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
                            }
                        }
                        else
                        {
                            log.Debug(" ----- No Date");
                        }
                    }
                    else
                    {
                        log.Debug(" ----- No Score");
                    }
                }
                else
                {
                    log.Debug(" ----- No Score");
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
                        log.Error(" ----- Error: ",e);
                    }
                }
            }



            log.Debug(" ----- Score: " + serviceScore + ";" + mostRecentBenchmark);

            if (analysisToScoreId != -1)
            {
                _analysisIDs.Add(analysisToScoreId);
            }

            string sql = String.Format(@"insert into vui_Service (ID,Name,ServiceName,PlatformId,DeviceId,BenchmarkScore,DefaultScreenshotID,IsPreviewable,ScreenshotCount,AppStoreURL) values ({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8}, '{9}'); ", new Object[] { service.Id, service.Name, Utility.NiceUrlName(service.Id), platformId, deviceId, serviceScore, defaultScreenshotID, isPreviewable, screenshotCount, appStoreURL });

            string VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");

            // Image metadata
            List<DynamicNode> images = service.Descendants("VUI_Image").Items;
            foreach (DynamicNode image in images)
            {
                int imageid = image.Id;

                int analysisId = image.Parent.Id;

                DateTime createDate = image.CreateDate;

                string pagetype = image.GetProperty("pageType").Value;

                string ImageURL_th =  VUI_mediafolder + @"th/" + image.GetProperty("thFile").Value.Replace("&", "%26");
                string ImageURL_md =  VUI_mediafolder + @"md/" + image.GetProperty("thFile").Value.Replace("&", "%26");
                string ImageURL_lg =  VUI_mediafolder + @"lg/" + image.GetProperty("lgFile").Value.Replace("&", "%26");
                string ImageURL_full = VUI_mediafolder + @"full/" + image.GetProperty("imageFile").Value.Replace("&", "%26");

                // This is for the New Handling URLs which decrypt on opening.  Note that the Preview Service is not encrypted for simplicity.
                if (EncryptImageURLs && isPreviewable==0)
                {
                    ImageURL_th = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_th);
                    ImageURL_md = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_md);
                    ImageURL_lg = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_lg);
                    ImageURL_full = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_full);
                }

                string tmp = String.Format(@"insert into vui_Image (Id, ServiceId, PageType, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, DateCreated, AnalysisId) values ({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}', {8}); ", new Object[] { imageid, service.Id, pagetype, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, createDate.ToString("yyyy-MM-dd HH:mm:ss.0"), analysisId });
                sql += tmp;
            }

            sql += ansql;

            log.Debug(sql);
            return sql;

        }


        /// <summary>
        /// Builds u
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, int> ScoresForAnalysis()
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();

            if (_analysisIDs.Count > 0)
            {
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
                foreach (int id in _analysisIDs)
                {
                    Analysis a = new Analysis(id);
                    a.SetBenchmark();

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
                }
            }
            return scores;
        }

    }
}