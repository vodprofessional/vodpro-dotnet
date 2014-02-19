using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.web;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using umbraco.BusinessLogic;
using System.Data.SqlClient;
using System.Configuration;
using VUI.VUI3.classes;
using log4net;

/*
 * Notes for import.
 * 
 * Folder structure:
 * 
 *  /importtag
 *           /DateStamp
 *                    /Platform
 *                             /[Device]
 *                                      /Service
 *  New ->                                      /Feature
 * 
 * Assume all platforms / Devices and services exist
 * 
 */
namespace VUI.VUI2.classes
{
    public class Importer
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Importer));

        const string VUI_FOLDERTYPE = "VUI_Folder";
        const string VUI_IMAGETYPE = "VUI_Image";
        const string VUI_FULL_FOLDER = @"full";
        const string VUI_LG_FOLDER = @"lg";
        const string VUI_MD_FOLDER = @"md";
        const string VUI_TH_FOLDER = @"th";


        static int VUI_maxwidth_full;
        static int VUI_maxwidth_lg;
        static int VUI_maxwidth_md;
        static int VUI_maxwidth_th;

        static DirectoryInfo fol_full;
        static DirectoryInfo fol_lg;
        static DirectoryInfo fol_md;
        static DirectoryInfo fol_th;

        static User u = new User("websitecontentuser");
        public static Dictionary<string, string> featureDirectoryMap = new Dictionary<string, string>();
        public static Dictionary<string, string> featureFeatureMap = new Dictionary<string, string>();
        public static Dictionary<string, string> pageTypeVals = new Dictionary<string, string>();

        public static List<string> imagesImported;
        public static List<string> errorImages;


        public static void Import(string importTag)
        {

            log.Debug("Starting image import [" + importTag + "]");

            imagesImported = new List<string>();
            errorImages = new List<string>();


            InitFeatureMaps();
            InitPageTypePreVals();
            int VUI2_rootnodeid = Int32.Parse(Utility.GetConst("VUI2_rootnodeid"));
            string VUI_importfolder = Utility.GetConst("VUI_importfolder");
            string VUI_mediafolder = Utility.GetConst("VUI_mediafolder");
            string VUI_importtempfolder = Utility.GetConst("VUI_importtempfolder");
             VUI_maxwidth_full = Int32.Parse(Utility.GetConst("VUI_maxwidth_full"));
             VUI_maxwidth_lg = Int32.Parse(Utility.GetConst("VUI_maxwidth_lg"));
             VUI_maxwidth_md = Int32.Parse(Utility.GetConst("VUI_maxwidth_md"));
             VUI_maxwidth_th = Int32.Parse(Utility.GetConst("VUI_maxwidth_th"));
            //int VUI_pagetypelist = Int32.Parse(Utility.GetConst("VUI_pagetypelist"));
            //int VUI_ScoringPageTypes = Int32.Parse(Utility.GetConst("VUI_ScoringPageTypes"));
            //int VUI_BenchmarkDevicesList = Int32.Parse(Utility.GetConst("VUI_BenchmarkDevicesList"));

            try
            {
                Directory.Delete(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_FULL_FOLDER), true);
                Directory.Delete(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_LG_FOLDER), true);
                Directory.Delete(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_MD_FOLDER), true);
                Directory.Delete(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_TH_FOLDER), true);
            }
            catch (Exception ex)
            {
                ;
            }
            fol_full = Directory.CreateDirectory(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_FULL_FOLDER));
            fol_lg = Directory.CreateDirectory(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_LG_FOLDER));
            fol_md = Directory.CreateDirectory(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_MD_FOLDER));
            fol_th = Directory.CreateDirectory(Path.Combine(VUI_importfolder, VUI_importtempfolder, VUI_TH_FOLDER));


            DirectoryInfo importroot = new DirectoryInfo(Path.Combine(VUI_importfolder, importTag));

            // Each "root" folder corresponds to a Datestamp folder
            foreach (DirectoryInfo root in importroot.GetDirectories())
            {

                // Check the Children of the VUI root
                var vuiRoot = new Node(VUI2_rootnodeid);
                string dateVal = root.Name;

                Nodes platformNodes = vuiRoot.Children;

                // Loop around platform directories in root
                foreach (DirectoryInfo platform in root.GetDirectories())
                {
                    string platformName = platform.Name.Trim();


                    bool platformExists = false;
                    int platformNodeId = 0;

                    // Check platform Nodes
                    foreach (Node n in platformNodes)
                    {
                        if (n.NodeTypeAlias.Equals(Utility.GetConst("VUI2_platformtype")) && n.Name.Trim().Equals(platformName))
                        {
                            platformExists = true;
                            platformNodeId = n.Id;
                        }
                    }
                    /* // Create new platform
                    if (!platformExists)
                    {
                        Lit1.Text += @" - Creating Platform: " + platformName + "<br/>";
                        Dictionary<string, string> folderProps = new Dictionary<string, string>();
                        folderProps.Add("folderLevel", "platform");
                        platformNodeId = CreateVUINode(platformName, umb_vuiFolderRoot, VUI_FOLDERTYPE, folderProps);
                    }
                    */

                    //Loop through Device folders
                    Document platformRoot = new Document(platformNodeId);
                    List<Document> deviceOrServiceNodes = platformRoot.Children.ToList<Document>();
                    foreach (DirectoryInfo deviceOrServiceFolder in platform.GetDirectories())
                    {
                        bool deviceExists = false;
                        int deviceNodeId = 0;
                        string deviceOrServiceName = deviceOrServiceFolder.Name.Trim();
                        foreach (Document n in deviceOrServiceNodes)
                        {
                            if (n.ContentType.Alias.Equals(Utility.GetConst("VUI2_servicetype")) && n.Text.Trim().Equals(deviceOrServiceName))
                            {
                                ProcessImagesForFolder(deviceOrServiceFolder, n.Id, importTag, dateVal, platformName, String.Empty, deviceOrServiceName);
                            }
                            if (n.ContentType.Alias.Equals(Utility.GetConst("VUI2_devicetype")) && n.Text.Trim().Equals(deviceOrServiceName))
                            {

                                List<Document> serviceNodes = n.Children.ToList<Document>(); 
                                foreach (DirectoryInfo serviceFolder in deviceOrServiceFolder.GetDirectories())
                                {
                                    foreach (Document serviceNode in serviceNodes)
                                    {
                                        if (serviceNode.ContentType.Alias.Equals(Utility.GetConst("VUI2_servicetype")) && serviceNode.Text.Trim().Equals(serviceFolder.Name.Trim()))
                                        {
                                            ProcessImagesForFolder(serviceFolder, serviceNode.Id, importTag, dateVal, platformName, deviceOrServiceName, serviceFolder.Name.Trim());
                                        }
                                    }
                                }
                            }
                        }
                        /* // Create new Device
                        if (!deviceExists)
                        {
                            Lit1.Text += @" -- Creating Device: " + deviceName + "<br/>";
                            Dictionary<string, string> folderProps = new Dictionary<string, string>();

                            int dirCount = device.GetDirectories().Count();

                            if (dirCount > 0)
                                folderProps.Add("folderLevel", "device");
                            else
                                folderProps.Add("folderLevel", "service");
                            deviceNodeId = CreateVUINode(deviceName, platformNodeId, VUI_FOLDERTYPE, folderProps);
                        }
                        */

                        
                        /*
                        //Loop through Service folders
                        var deviceRoot = new Node(deviceNodeId);
                        Nodes serviceNodes = deviceRoot.Children;
                        foreach (DirectoryInfo service in deviceOrServiceFolder.GetDirectories())
                        {
                            bool serviceExists = false;
                            int serviceNodeId = 0;
                            string serviceName = service.Name.Trim();
                            foreach (Node n in serviceNodes)
                            {
                                if (n.NodeTypeAlias.Equals(VUI_FOLDERTYPE) && n.Name.Trim().Equals(serviceName))
                                {
                                    serviceExists = true;
                                    serviceNodeId = n.Id;
                                }
                            }
                            // Create new Service
                            if (!serviceExists)
                            {
                                Lit1.Text += @" --- Creating Service: " + serviceName + "<br/>";
                                Dictionary<string, string> folderProps = new Dictionary<string, string>();
                                folderProps.Add("folderLevel", "service");
                                serviceNodeId = CreateVUINode(serviceName, deviceNodeId, VUI_FOLDERTYPE, folderProps);
                            }

                            // Finally, process any images
                            ProcessImagesForFolder(service, serviceNodeId, importTag, dateVal, platformName, deviceOrServiceName, serviceName);
                        }
                         * */
                    }
                }
            }
            // Copy Generated Images into media folder
            foreach (FileInfo f in fol_full.GetFiles())
            {
                //             Response.Write(f.FullName + " -> " + Path.Combine(Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER) + f.Name + " (" + Path.Combine(Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER, f.Name) + ")<Br/>");
                File.Copy(f.FullName, Path.Combine(System.Web.HttpContext.Current.Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_lg.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(System.Web.HttpContext.Current.Server.MapPath(VUI_mediafolder), VUI_LG_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_md.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(System.Web.HttpContext.Current.Server.MapPath(VUI_mediafolder), VUI_MD_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_th.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(System.Web.HttpContext.Current.Server.MapPath(VUI_mediafolder), VUI_TH_FOLDER, f.Name), true);
            }
        }


        // ProcessImagesForFolder(serviceFolder, serviceNode.Id, importTag, dateVal, platformName, deviceOrServiceName, serviceFolder.Name.Trim());
        protected static void ProcessImagesForFolder(DirectoryInfo folder, int serviceNodeId, string importTag, string dateVal, string platformName, string deviceName, string serviceName)
        {
            Document serviceRoot = new Document(serviceNodeId);
            bool serviceIsPublished = serviceRoot.Published;
            bool benchmarkIsPublished = false;

            List<Document> benchmarkNodes = serviceRoot.Children.ToList<Document>();
            int benchmarkNodeId = -1;
            bool benchmarkExists = false;
            foreach (Document b in benchmarkNodes)
            {
                if(b.ContentType.Alias.Equals(Utility.GetConst("VUI2_analysistype")) && b.Text.ToLower().Equals(dateVal.ToLower()))
                {
                    benchmarkNodeId = b.Id;
                    benchmarkExists = true;
                    benchmarkIsPublished = b.Published;
                    break;
                }
            }
            if (!benchmarkExists)
            {
                Dictionary<string, object> folderProps = new Dictionary<string, object>();
                string analysisDate = DateTime.Now.ToString("yyyy-MM-dd");
                folderProps.Add("analysisDate", analysisDate);
                benchmarkNodeId = CreateVUINodePublish(dateVal, serviceNodeId, Utility.GetConst("VUI2_analysistype"), folderProps, serviceIsPublished);
                benchmarkIsPublished = serviceIsPublished;
            }


            /*
             * At this stage, need to introduce logic to read the page type folders.
             */

            // Keep track - we are going to count the images coming in for the News!
            int imageCount = 0;

            // First get non-categorised images
            FileInfo[] images = folder.GetFiles();
            if (images.Length > 0)
            {
                DoImages(images, serviceRoot, benchmarkNodeId, importTag, dateVal, platformName, deviceName, serviceName, String.Empty, benchmarkIsPublished);
                imageCount += images.Length;
            }
            // Then trawl through the sub-folders
            foreach (DirectoryInfo featureFolder in folder.GetDirectories())
            {
                string pageTypeName = String.Empty;
                
                try {
                    pageTypeName = featureDirectoryMap[featureFolder.Name];
                    images = featureFolder.GetFiles();
                    if (images.Length > 0)
                    {
                        DoImages(images, serviceRoot, benchmarkNodeId, importTag, dateVal, platformName, deviceName, serviceName, pageTypeName, benchmarkIsPublished);
                        imageCount += images.Length;
                    }
                }
                catch (Exception e) {
                    log.Error("Error Getting images from folder [" + featureFolder.FullName + "]", e); 
                }
            }

            // Now we have an idea of how many images we've captured, record to the News
            // First, get the ServiceMaster
            try
            {
                Document sm = VUI3Utility.FindServiceMasterDocumentByName(serviceRoot.Text);
                int smid = sm.Id;
                string pd = platformName;
                if(!String.IsNullOrEmpty(deviceName)) 
                {
                    pd = pd + " / " + deviceName;
                }
                
                VUI3News.AddNews(VUI3News.NEWSTYPE_SCREENSHOT, relatedServiceId:smid, relatedService: serviceRoot.Text, relatedPlatform: platformName, relatedDevice: deviceName, ScreenshotCount: imageCount, directToLive: false, tweetNews: false );
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
        }


        private static void DoImages(FileInfo[] images, Document serviceRoot, int benchmarkNodeId, string importTag, string dateVal, string platformName, string deviceName, string serviceName, string pageTypeName,  bool publish)
        {
            Document benchmarkRoot = new Document(benchmarkNodeId);
            try
            {

                foreach (FileInfo image in images)
                {
                    log.Debug(" - Processing Image {" + image.FullName + "}");
                    string imageName = String.Empty;
                    if (!String.IsNullOrEmpty(deviceName))
                    {
                        imageName = (dateVal + "-" + platformName + "-" + deviceName + "-" + serviceName + "-" + pageTypeName + "-" + image.Name).Replace("/", "-").Replace(' ', '-');
                    }
                    else
                    {
                        imageName = (dateVal + "-" + platformName + "-" + serviceName + "-" + pageTypeName + "-" + image.Name).Replace("/", "-").Replace(' ', '-');
                    }

                    if (!image.Extension.Contains("jpg"))
                    {
                        Regex re = new Regex(@"\.[A-z]+$");
                        imageName = re.Replace(imageName, ".jpg");
                    }
                    imageName.Replace(@"&", @"and");

                    // File.Copy(image.FullName, Path.Combine(fol_full.FullName, imageName), true);


                    try
                    {

                        List<Document> imageNodes = benchmarkRoot.Children.ToList<Document>();
                        bool imageExists = false;

                        foreach (Document n in imageNodes)
                        {
                            if (n.ContentType.Alias.Equals(VUI_IMAGETYPE) && n.Text.Trim().Equals(imageName))
                            {
                                imageExists = true;
                                log.Debug(" -- The Image already exists");
                                break;
                            }
                        }

                        if (!imageExists)
                        {
                            Resize(image.FullName, Path.Combine(fol_full.FullName, imageName), VUI_maxwidth_full);
                            //File.Copy(image.FullName, Path.Combine(fol_full.FullName, imageName), true);
                            Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_lg.FullName, imageName), VUI_maxwidth_lg);
                            Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_md.FullName, imageName), VUI_maxwidth_md);
                            Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_th.FullName, imageName), VUI_maxwidth_th);

                            Dictionary<string, object> imageProps = new Dictionary<string, object>();
                            imageProps.Add("platform", platformName);
                            if (String.IsNullOrEmpty(serviceName))
                            {
                                imageProps.Add("service", deviceName);
                            }
                            else
                            {
                                imageProps.Add("device", deviceName);
                                imageProps.Add("service", serviceName);
                            }
                            imageProps.Add("vuidate", dateVal);
                            imageProps.Add("imageFile", imageName);
                            imageProps.Add("lgFile", imageName);
                            imageProps.Add("thFile", imageName);
                            imageProps.Add("importTag", importTag);
                            // ADD page type here
                            imageProps.Add("pageType", GetPageType(pageTypeName));

                            int imageId = CreateVUINodePublish(imageName, benchmarkNodeId, VUI_IMAGETYPE, imageProps, publish);

                            imagesImported.Add(image.FullName + " - " + imageName);
                        }
                        else
                        {
                            errorImages.Add("Duplicate image [" + image.FullName + " - " + imageName + "]");
                        }

                    }
                    catch (OutOfMemoryException oom)
                    {
                        log.Error("The image has an invalid format or GDI+ does not support the pixel format", oom);
                        errorImages.Add("Image Format Issue [" + image.FullName + "]");
                    }
                    catch (Exception ex)
                    {
                        log.Error("A different error occurred", ex);
                        errorImages.Add("Error importing [" + image.FullName + "]");
                    }
                }
            }
            catch (Exception fileException)
            {
                log.Error("Error getting file Info", fileException);
            }
        }



/*        private static int CreateVUINode(string nodeName, int parentNode, string documentType)
        {
            return CreateVUINodePublish(nodeName, parentNode, documentType, null, true);
        }
        */
        private static int CreateVUINode(string nodeName, int parentNode, string documentType, Dictionary<string, object> nodeProps)
        {
            return CreateVUINodePublish(nodeName, parentNode, documentType, nodeProps, true);
        }
        private static int CreateVUINodeNoPublish(string nodeName, int parentNode, string documentType, Dictionary<string, object> nodeProps)
        {
            return CreateVUINodePublish(nodeName, parentNode, documentType, nodeProps, false);
        }

        private static int CreateVUINodePublish(string nodeName, int parentNode, string documentType, Dictionary<string, object> nodeProps, bool publish)
        {
            // The documenttype that should be used, replace 10 with the id of your documenttype
            DocumentType dt = DocumentType.GetByAlias(documentType);

            // The umbraco user that should create the document,
            // 0 is the umbraco system user, and always exists


            // Create the document
            Document d = Document.MakeNew(nodeName, dt, u, parentNode);
            if (nodeProps != null)
            {
                foreach (string k in nodeProps.Keys)
                {
                    d.getProperty(k).Value = nodeProps[k];
                }
            }
            d.Save();
            if (publish)
            {
                try
                {
                    d.Publish(u);
                    umbraco.library.UpdateDocumentCache(d.Id);
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            return d.Id;
        }


        public static void Resize(string imageFile, string outputFile, int maxWidth)
        {
            log.Debug(" -- Resizing [" + imageFile + "]");

            using (var srcImage = System.Drawing.Image.FromFile(imageFile))
            {
                double scaleFactor = 0;

                if (srcImage.Width > maxWidth)
                {
                    scaleFactor = (double)maxWidth / (double)srcImage.Width;
                }
                else
                {
                    maxWidth = srcImage.Width;
                    scaleFactor = 1;
                }
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 60L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                var newWidth = maxWidth;
                var newHeight = (int)(srcImage.Height * scaleFactor);
                using (var newImage = new Bitmap(newWidth, newHeight))
                {
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.PixelOffsetMode = PixelOffsetMode.Default;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(outputFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
        }

        private static  ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        
        public static void InitPageTypePreVals()
        {
            pageTypeVals.Clear();

            string sql = String.Format(@"select value, id from [dbo].[cmsDataTypePreValues] where datatypeNodeId = {0}", 2866);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    pageTypeVals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }
        }

        public static string GetPageType(string val)
        {
            if(pageTypeVals.Keys.Contains(val))
            {
                return pageTypeVals[val];
            }
            else
            {
                return String.Empty;
            }
        }

        public static void InitFeatureMaps()
        {
            featureDirectoryMap.Clear();
            featureFeatureMap.Clear();

            featureDirectoryMap.Add("00 - Splash Page", "Splash Page");
            featureDirectoryMap.Add("01 - Featured Content", "Featured content");
            featureDirectoryMap.Add("02 - Categorisation", "Categorisation");
            featureDirectoryMap.Add("03 - Category Homepage", "Category homepage");
            featureDirectoryMap.Add("04 - Most Popular", "Most popular");
            featureDirectoryMap.Add("05 - Recently Added", "Recently added");
            featureDirectoryMap.Add("06 - More Episodes", "More episodes");
            featureDirectoryMap.Add("07 - Content Recommendation", "Content recommendation");
            featureDirectoryMap.Add("08 - EPG", "EPG");
            featureDirectoryMap.Add("09 - Search", "Search");
            featureDirectoryMap.Add("10 - Predictive Search", "Predictive search");
            featureDirectoryMap.Add("11 - A-Z Page", "A-Z page");
            featureDirectoryMap.Add("12 - Social Sharing Outbound", "Social sharing (out)");
            featureDirectoryMap.Add("13 - Social Sharing In Service", "Social sharing (in-service)");
            featureDirectoryMap.Add("14 - Customised Video Player", "Customised video player");
            featureDirectoryMap.Add("15 - Video Description", "Video description");
            featureDirectoryMap.Add("16 - Favourites", "Favourites");
            featureDirectoryMap.Add("17 - Playlists", "Playlists");
            featureDirectoryMap.Add("18 - Sign-in Register", "Sign in / Register");
            featureDirectoryMap.Add("19 - Social Sign-on", "Social sign-on");
            featureDirectoryMap.Add("20 - Parental Controls", "Parental controls");
            featureDirectoryMap.Add("21 - Help", "Help");
            featureDirectoryMap.Add("22 - About", "About");
            featureDirectoryMap.Add("23 - Contact", "Contact");
            featureDirectoryMap.Add("24 - Accessibility", "Accessibility");
            featureDirectoryMap.Add("25 - Watch on other Platforms", "Watch on other platforms");
            featureDirectoryMap.Add("26 - Adaptive Bitrate Streaming", "Adaptive bitrate streaming");
            featureDirectoryMap.Add("27 - Audio-described Shows", "Audio-described Shows");
            featureDirectoryMap.Add("28 - Device Synchronisation", "Device synchronisation");
            featureDirectoryMap.Add("29 - Download to Device", "Download to device");
            featureDirectoryMap.Add("30 - Extended Archive", "Extended Archive");
            featureDirectoryMap.Add("31 - Last Viewed", "Last Viewed");
            featureDirectoryMap.Add("32 - Live Restart", "Live Restart");
            featureDirectoryMap.Add("33 - Live Viewing", "Live Viewing");
            featureDirectoryMap.Add("34 - Resume after Stopping", "Resume after stopping");
            featureDirectoryMap.Add("35 - Social Recommendation", "Social recommendation");

            featureFeatureMap.Add("Splash Page", "Splash page");
            featureFeatureMap.Add("Featured content", "Featured content");
            featureFeatureMap.Add("Categorisation", "Categorisation");
            featureFeatureMap.Add("Individual Category Pages", "Category homepage");
            featureFeatureMap.Add("Most popular", "Most popular");
            featureFeatureMap.Add("Recently added", "Recently added");
            featureFeatureMap.Add("More episodes", "More episodes");
            featureFeatureMap.Add("Content recommendation", "Content recommendation");
            featureFeatureMap.Add("EPG (Electronic Programming Guide)", "EPG");
            featureFeatureMap.Add("Search", "Search");
            featureFeatureMap.Add("Predictive search", "Predictive search");
            featureFeatureMap.Add("A-Z page", "A-Z page");
            featureFeatureMap.Add("Social sharing (out)", "Social sharing (out)");
            featureFeatureMap.Add("Social sharing (in-service)", "Social sharing (in-service)");
            featureFeatureMap.Add("Customised video player", "Customised video player");
            featureFeatureMap.Add("Video description", "Video description");
            featureFeatureMap.Add("Favourites", "Favourites");
            featureFeatureMap.Add("Playlists", "Playlists");
            featureFeatureMap.Add("Sign in / Register", "Sign in / Register");
            featureFeatureMap.Add("Social sign-on", "Social sign-on");
            featureFeatureMap.Add("Parental controls", "Parental controls");
            featureFeatureMap.Add("Help", "Help");
            featureFeatureMap.Add("About", "About");
            featureFeatureMap.Add("Contact", "Contact");
            featureFeatureMap.Add("Accessibility", "Accessibility");
            featureFeatureMap.Add("Watch on other platforms", "Watch on other platforms");
            featureFeatureMap.Add("Adaptive bitrate streaming", "Adaptive bitrate streaming");
            featureFeatureMap.Add("Audio-described Shows", "Audio-described Shows");
            featureFeatureMap.Add("Device synchronisation", "Device synchronisation");
            featureFeatureMap.Add("Download to device", "Download to device");
            featureFeatureMap.Add("Extended Archive", "Extended Archive");
            featureFeatureMap.Add("Last Viewed / History", "Last Viewed");
            featureFeatureMap.Add("Live Restart", "Live Restart");
            featureFeatureMap.Add("Live Viewing", "Live Viewing");
            featureFeatureMap.Add("Resume after stopping", "Resume after stopping");
            featureFeatureMap.Add("Social content recommendation", "Social recommendation");

        }


    }
}