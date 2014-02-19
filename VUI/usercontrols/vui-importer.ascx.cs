using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using System.Xml.XPath;
using VUI.classes;
using VUI.VUI2.classes;
using VUI.VUI3.classes;
using System.Text;

namespace VUI.usercontrols
{
    public partial class vui_importer : System.Web.UI.UserControl
    {
        int umb_vuiFolderRoot = 0;
        string VUI_importfolder = String.Empty;
        string VUI_mediafolder = String.Empty;        
        string VUI_importtempfolder = String.Empty;
        int VUI_pagetypelist = -1;
        int VUI_ScoringPageTypes = -1;
        int VUI_BenchmarkDevicesList = -1;

        int VUI_maxwidth_lg = 0;
        int VUI_maxwidth_th = 0;
        int VUI_maxwidth_md = 0;
        int VUI_maxwidth_full = 0;
        
        const string VUI_FOLDERTYPE = "VUI_Folder";
        const string VUI_IMAGETYPE = "VUI_Image";
        const string VUI_FULL_FOLDER = @"full";
        const string VUI_LG_FOLDER = @"lg";
        const string VUI_MD_FOLDER = @"md";
        const string VUI_TH_FOLDER = @"th";


        DirectoryInfo fol_full;
        DirectoryInfo fol_lg;
        DirectoryInfo fol_md;
        DirectoryInfo fol_th;


        User u = new User("websitecontentuser");

        protected void Page_Load(object sender, EventArgs e)
        {
            umb_vuiFolderRoot = Int32.Parse(ConfigurationManager.AppSettings["umb_vuiFolderRoot"].ToString());
            VUI_importfolder = ConfigurationManager.AppSettings["VUI_importfolder"].ToString();
            VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString();
            VUI_importtempfolder = ConfigurationManager.AppSettings["VUI_importtempfolder"].ToString();
            VUI_maxwidth_full = Int32.Parse(ConfigurationManager.AppSettings["VUI_maxwidth_full"].ToString());
            VUI_maxwidth_lg = Int32.Parse(ConfigurationManager.AppSettings["VUI_maxwidth_lg"].ToString());
            VUI_maxwidth_md = Int32.Parse(ConfigurationManager.AppSettings["VUI_maxwidth_md"].ToString());
            VUI_maxwidth_th = Int32.Parse(ConfigurationManager.AppSettings["VUI_maxwidth_th"].ToString());
            VUI_pagetypelist = Int32.Parse(ConfigurationManager.AppSettings["VUI_pagetypelist"].ToString());
            VUI_ScoringPageTypes = Int32.Parse(ConfigurationManager.AppSettings["VUI_ScoringPageTypes"].ToString());
            VUI_BenchmarkDevicesList = Int32.Parse(ConfigurationManager.AppSettings["VUI_BenchmarkDevicesList"].ToString());

            if (!IsPostBack)
            {
                XPathNodeIterator iterator = umbraco.library.GetPreValues(VUI_pagetypelist);
                iterator.MoveNext(); //move to first
                XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

                while (preValues.MoveNext())
                {
                    string value = preValues.Current.Value;
                    string id = preValues.Current.GetAttribute("id", "");
                    litPageTypes.Text += @"<li data-id=""" + id + @""" class=""droppable ui-pagetype"">" + value + @" (<span class=""count"">0</span>)<span class=""img-data"" data-id=""" + id + @"""></span></li>";
                }

                PopulateServiceDD();
                
                /// TODO 
                /// Populate DD, and write even thandler
                iterator = umbraco.library.GetPreValues(VUI_BenchmarkDevicesList);
                iterator.MoveNext(); //move to first
                preValues = iterator.Current.SelectChildren("preValue", "");

                StringDictionary vals = new StringDictionary();

                while (preValues.MoveNext())
                {
                    ddDevice.Items.Add(new ListItem(preValues.Current.Value, preValues.Current.GetAttribute("id", "")));
                }
            }
        }


        protected void ClearImportDuplicateImages(object sender, EventArgs e)
        {
            string wpi = ImportTag.Text;

            DocumentType dt = DocumentType.GetByAlias("VUI_Image");

            List<Document> images = Document.GetDocumentsOfDocumentType(dt.Id).ToList().Where(d => d.getProperty("importTag").Value.ToString().Equals(wpi)).ToList();

            foreach (Document c in images)
            {
                if (c.Text.EndsWith(")"))
                {
                    c.UnPublish();
                    umbraco.library.UnPublishSingleNode(c.Id);
                    c.delete(true);
                }
            }

            // Tell the published XML content that the document has been unpublished.
            umbraco.library.RefreshContent();

        }

        protected void ClearWorkPackageImages(object sender, EventArgs e)
        {
            string wpi = txtWorkpackageid.Text;

            DocumentType dt = DocumentType.GetByAlias("VUI2Analysis");

            List<Document> analyses = Document.GetDocumentsOfDocumentType(dt.Id).ToList().Where(d => d.getProperty("workPackageID").Value.ToString().Equals(wpi)).ToList();

            foreach(Document a in analyses)
            {
                if(a.HasChildren)
                {
                    foreach(Document c in a.Children)
                    {
                        c.delete(true);
                    }
                }
            }
        }


        protected void ImportAnalysesFromDB(object sender, EventArgs e)
        {
            Importer.InitFeatureMaps();

            string wpi = txtWorkpackageid.Text;
            List<AnalysisImport> analyses = AnalysisImporter.GetAnalysisRecords(wpi);
            foreach (AnalysisImport a in analyses)
            {
                string features = a.FeatureList;
                string[] featurelist = features.Split(',');
                List<int> featureids = new List<int>();
                foreach (string f in featurelist)
                {
                    if (!String.IsNullOrEmpty(f))
                    {
                        string mappedFeature = String.Empty;
                        mappedFeature = Importer.featureFeatureMap[f.Trim()];

                        int id = GetPreValID(2850, mappedFeature);
                        if (id != -1)
                        {
                            featureids.Add(id);
                        }
                    }
                }

                string hotfeatures = a.HotList;
                string[] hotfeaturelist = hotfeatures.Split(',');
                List<int> hotfeatureids = new List<int>();
                foreach (string f in hotfeaturelist)
                {
                    if (!String.IsNullOrEmpty(f))
                    {
                        string mappedFeature = String.Empty;
                        mappedFeature = Importer.featureFeatureMap[f.Trim()];
                        int id = GetPreValID(10097, mappedFeature);
                        if (id != -1)
                        {
                            hotfeatureids.Add(id);
                        }
                    }
                }


                Dictionary<string, object> nodeProps = new Dictionary<string, object>();
                nodeProps.Add("analysisDate", a.Timestamp);
                nodeProps.Add("benchmarkDate", a.Timestamp);
                nodeProps.Add("benchmarkDevice", GetPreValID(7489, a.TestingDevice));
                nodeProps.Add("serviceScore", featureids.Count);
                nodeProps.Add("serviceCapabilities", String.Join(",", featureids.ToArray()));
                nodeProps.Add("hotFeatures", String.Join(",", hotfeatureids.ToArray()));
                nodeProps.Add("workPackageID", wpi);

                int sid = a.GetServiceDocumentId();

                Document svc = new Document(sid);
                Document[] ansarr = svc.Children;

                if (ansarr.Length > 0)
                {
                    try
                    {
                        Document ans = svc.Children.ToList().First(an => an.Text.Equals(a.ImportDateTag));
                        if (ans != null)
                        {
                            UpdateVUINodePublish(ans.Id, nodeProps, ans.Published);
                            continue;
                        }
                    } catch { ; }
                }
                
                
                if (a.ServiceIsPublic)
                {
                    CreateVUINode(a.ImportDateTag, sid, "VUI2Analysis", nodeProps);
                }
                else
                {
                    CreateVUINodeNoPublish(a.ImportDateTag, sid, "VUI2Analysis", nodeProps);
                }

                try
                {
                    Document sm = VUI3Utility.FindServiceMasterDocumentByName(a.ServiceName);
                    int smid = sm.Id;
                    string pd = a.Platform;
                    if (!String.IsNullOrEmpty(a.Device))
                    {
                        pd = pd + " / " + a.Device;
                    }
                    string description = String.Format(@"New benchmark score for {0} on {1}", new object[] { a.ServiceName, pd });
                    VUI3News.AddNews(VUI3News.NEWSTYPE_BENCHMARK, relatedServiceId: smid, relatedService: a.ServiceName, relatedPlatform: a.Platform, relatedDevice: a.Device, description: description);
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }

            }
        }

        protected int GetPreValID(int dataTypeId, string checkString)
        {
            // This is horrible - get the PreVal for "Paid by PayPal"
            int preValId = -1;
            string status = String.Empty;
            XPathNodeIterator statusRoot = umbraco.library.GetPreValues(dataTypeId);
            statusRoot.MoveNext(); //move to first
            XPathNodeIterator preValues = statusRoot.Current.SelectChildren("preValue", "");
            while (preValues.MoveNext())
            {
                if (preValues.Current.Value.ToLower().Equals(checkString.ToLower()))
                {
                    status = preValues.Current.GetAttribute("id", "");
                    break;
                }
            }
            if(Int32.TryParse(status, out preValId))
            {
                return preValId;
            }
            return preValId;
        }


        protected void SetAllHideHotFeatures(object sender, EventArgs e)
        {
            DynamicNode node = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));
            List<DynamicNode> analyses = node.Descendants(Utility.GetConst("VUI2_analysistype")).Items.ToList();
            Response.Write("Analyses: " + analyses.Count());
            foreach (DynamicNode analysis in analyses)
            {
                Document n = new Document(analysis.Id);
                n.getProperty("hideHotFeatures").Value = 1;
                n.Save();
                n.Publish(u);
                umbraco.library.UpdateDocumentCache(n.Id);
            }
        }


        protected void ClearAllPageTypes(object sender, EventArgs e)
        {
            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            List<DynamicNode> images = node.Descendants(VUI_IMAGETYPE).Items;
            foreach (DynamicNode image in images)
            {
                Document n = new Document(image.Id);
                n.getProperty("pageType").Value = "";
                n.Save();
                n.Publish(u);
                umbraco.library.UpdateDocumentCache(n.Id);
            }

        }


        protected void PopulateServiceDD()
        {
            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            List<DynamicNode> services = node.Descendants(VUI_FOLDERTYPE).Items
                                            .Where(n => n.GetProperty("folderLevel").Value.Equals(VUIfunctions.VUI_SERVICE))
                                            .OrderBy(n => n.Name)
                                            .ToList();
            foreach (DynamicNode service in services)
            {
                ServiceList.Items.Add(new ListItem(service.Name + "(" + service.Parent.Name + ")", service.Id.ToString()));
            }
        }

        protected void UpdateBenchmarkDetail(object sender, EventArgs e)
        {
            int device = -1;
            DateTime dt = calDate.SelectedDate;
            int rootId = -1;
            
            if(Int32.TryParse(txtServiceParent.Text, out rootId) &&  Int32.TryParse(ddDevice.SelectedValue, out device))
            {
                VUIfunctions.UpdateServiceBenchmarkDateDevice(rootId, dt, device);
                Response.Write("Updating services");
            }

        }


        protected void RegenerateVUIMetaData(object sender, EventArgs e)
        {
            lblMeta.Visible = false;
            lblMetaError.Visible = false;

            try
            {
                MetaData.RegenerateMetaData();
                SearchImagesSingleton.Instance();
                lblMeta.Visible = true;
            }
            catch (Exception ex)
            {
                lblMetaError.Text = ex.Message + ex.StackTrace;
                lblMetaError.Visible = true;
            }
        }


        protected void MigrateToVui2(object sender, EventArgs e)
        {
            Migrator mig = new Migrator();
            mig.Migrate();
            mig.MigratePart2();
        }


        protected void GenerateServiceMasterItems(object sender, EventArgs e)
        {
            lblMetaError.Text = GenerateServiceMasters.Generate();
            lblMetaError.Visible = true;
        }


        protected void GetImagesWithoutPageType()
        {
            DynamicNode node = new DynamicNode(umb_vuiFolderRoot);
            List<DynamicNode> images;
            if (!String.IsNullOrEmpty(ServiceList.SelectedValue))
            {
                int serviceid = 0;
                Int32.TryParse(ServiceList.SelectedValue, out serviceid);

                images = node.Descendants(VUI_IMAGETYPE).Items
                                                    .Where(n => n.Parent.Id == serviceid && (n.GetProperty("pageType") == null || String.IsNullOrEmpty(n.GetProperty("pageType").Value)))
                                                    .OrderBy(n => n.GetProperty("service").Value)
                                                    .OrderBy(n => n.GetProperty("device").Value)
                                                    .OrderBy(n => n.GetProperty("platform").Value)
                                                    .ToList();
            }
            else
            {
                images = node.Descendants(VUI_IMAGETYPE).Items
                                                    .Where(n => n.GetProperty("pageType") == null || String.IsNullOrEmpty(n.GetProperty("pageType").Value))
                                                    .OrderBy(n => n.GetProperty("service").Value)
                                                    .OrderBy(n => n.GetProperty("device").Value)
                                                    .OrderBy(n => n.GetProperty("platform").Value)
                                                    .ToList();
            }
            rptImageList.DataSource = images;
            rptImageList.DataBind();
        }


        private int prevParent = -1;

        protected void ImageList_Bound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                StringBuilder s = new StringBuilder(@"");
                DynamicNode imageNode = (DynamicNode)e.Item.DataItem;
                int parent = imageNode.Parent.Id;
                if (parent != prevParent)
                {
                    s.Append(@"<li class=""images-title"">" + imageNode.GetProperty("service").Value + " / " + imageNode.GetProperty("device").Value + " / " + imageNode.GetProperty("platform").Value + "</li>");
                
                    // Output checkbox list
                    s.Append(@"<ul class=""ui-service-capability"" data-serviceid=""" + parent + @""">");


                    Document serviceNode = new Document(parent);
                    string caps = "";
                    if (serviceNode.getProperty("serviceCapabilities") != null)
                    {
                        caps = serviceNode.getProperty("serviceCapabilities").Value.ToString();
                    }


                    XPathNodeIterator iterator = umbraco.library.GetPreValues(VUI_ScoringPageTypes);
                    iterator.MoveNext(); //move to first
                    XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

                    while (preValues.MoveNext())
                    {
                        string value = preValues.Current.Value;
                        string id = preValues.Current.GetAttribute("id", "");
                        s.Append(@"<li>");
                        s.Append(@"<input id=""sc-" + parent + @"-" + id + @""" type=""checkbox"" value=""" + id + @"""");
                        
                        if(caps.Contains(id.ToString()))
                        {
                            s.Append(@" checked=""checked"" ");
                        }
                        s.Append(@"/>");
                        s.Append(@"<label for=""sc-" + parent + @"-" + id + @""">" + value + @"</label></li>");
                    }
                    s.Append(@"</ul>");
                }
                s.Append(@"<li><img src=""" + ResolveUrl(VUI_mediafolder) + @"load.png"" data-original=""" + ResolveUrl(VUI_mediafolder) + @"md/" + imageNode.GetProperty("imageFile").Value + @"""  data-id=""" + imageNode.Id + @""" class=""img-draggable lazy"" title=""PageType: " + imageNode.GetProperty("pageType").Value + @""" /></li>");
                Literal img = (Literal)e.Item.FindControl("litImage");
                img.Text = s.ToString();
                prevParent = parent;
            }
        }


        protected void btnShowNoTypePages_Click(object sender, EventArgs e)
        {
            GetImagesWithoutPageType();

            pnlOrgItems.Visible = true;
        }

        protected void btnRegnerateServiceScores_Click(object sender, EventArgs e)
        {
            VUIfunctions.UpdateAllServicePageTypesScore();
        }


        protected void UpdateData(object sender, EventArgs e)
        {

            // Update PageTypes
            string d = PageTypes.Text;
            string[] d1 = d.Split('/');

            foreach (string p in d1)
            {
                if (p.Contains(";"))
                {
                    string[] u = p.Split(';');

                    string pageType = u[0];
                    for (int i = 1; i < u.Length; i++)
                    {
                        int nodeId = Int16.Parse(u[i]);
                        Dictionary<string, string> imageProps = new Dictionary<string, string>();
                        imageProps.Add("pageType", pageType);
                        UpdateVUINode(nodeId, imageProps);
                    }
                }
            }

            // Upublish Images
            d = Bin.Text;
            d1 = d.Split('/');
            foreach (string p in d1)
            {
                if (p.Contains(";"))
                {
                    string[] u = p.Split(';');

                    string command = u[0];
                    for (int i = 1; i < u.Length; i++)
                    {
                        int nodeId = Int16.Parse(u[i]);
                        RemoveNode(nodeId);
                    }
                }
            }

            string[] capabilities = Capabilities.Text.Split(';');
            foreach (string servicecap in capabilities)
            {
                if (servicecap.Contains(":"))
                {
                    string[] c = servicecap.Split(':');
                    int serviceid = -1;
                    if(Int32.TryParse(c[0], out serviceid))
                    {
                        Dictionary<string, string> serviceProps = new Dictionary<string, string>();
                        serviceProps.Add("serviceCapabilities", c[1]);
                        UpdateVUINode(serviceid, serviceProps);
                    }
                }
            }


            umbraco.library.RefreshContent();

            GetImagesWithoutPageType();
            pnlOrgItems.Visible = true;

//            Dictionary<string, string> imageProps = new Dictionary<string, string>();
//            imageProps.Add("pageType", pageType);
//            UpdateVUINode(nodeId, imageProps);
        }

        private void RemoveNode(int nodeId)
        {
            // Unpublish 
            // Response.Write(@"Unpublishing: " + nodeId.ToString() + "");

            Document d = new Document(nodeId);
            d.UnPublish();
            umbraco.library.UnPublishSingleNode(nodeId);
            //umbraco.library.UpdateDocumentCache(nodeId);
        }

        /// <summary>
        /// Lithium Build Importer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Import(object sender, EventArgs e)
        {
            Importer.Import(ImportTag.Text);


            txtImportMessages.Text = "Imported:" + Environment.NewLine;

            foreach (string m in Importer.imagesImported)
            {
                txtImportMessages.Text += " - " + m + Environment.NewLine;
            } 
            
            txtImportMessages.Text += Environment.NewLine + Environment.NewLine + "Errors:" + Environment.NewLine;

            foreach (string m in Importer.errorImages)
            {
                txtImportMessages.Text += " - " + m + Environment.NewLine;
            }
            txtImportMessages.Visible = true;
        }

        protected void Import_Deprecated(object sender, EventArgs e)
        {
            /* Find the import folder root
             *  Root: \vui-import
             *                   \import-tag
             *                              \date
             *                                   \platform
             *                                            \device
             *                                                   \service
             *                             
             * Work through the folder structure, check/create the equivalent VUI_folder structure
             */
            try
            {
                Directory.Delete(Path.Combine(VUI_importfolder,VUI_importtempfolder,VUI_FULL_FOLDER), true);
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



            string importTag = ImportTag.Text;

            DirectoryInfo importroot = new DirectoryInfo(Path.Combine(VUI_importfolder,importTag));

            foreach (DirectoryInfo root in importroot.GetDirectories())
            {

                // Check the Children of the VUI root
                var vuiRoot = new Node(umb_vuiFolderRoot);
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
                        if (n.NodeTypeAlias.Equals(VUI_FOLDERTYPE) && n.Name.Trim().Equals(platformName))
                        {
                            platformExists = true;
                            platformNodeId = n.Id;
                        }
                    }
                    // Create new platform
                    if (!platformExists)
                    {
                        Lit1.Text += @" - Creating Platform: " + platformName + "<br/>";
                        Dictionary<string, object> folderProps = new Dictionary<string, object>();
                        folderProps.Add("folderLevel", "platform");
                        platformNodeId = CreateVUINode(platformName, umb_vuiFolderRoot, VUI_FOLDERTYPE, folderProps);
                    }

                    //Loop through Device folders
                    var platformRoot = new Node(platformNodeId);
                    Nodes deviceNodes = platformRoot.Children;
                    foreach (DirectoryInfo device in platform.GetDirectories())
                    {
                        bool deviceExists = false;
                        int deviceNodeId = 0;
                        string deviceName = device.Name.Trim();
                        foreach (Node n in deviceNodes)
                        {
                            if (n.NodeTypeAlias.Equals(VUI_FOLDERTYPE) && n.Name.Trim().Equals(deviceName))
                            {
                                deviceExists = true;
                                deviceNodeId = n.Id;
                            }
                        }
                        // Create new Device
                        if (!deviceExists)
                        {
                            Lit1.Text += @" -- Creating Device: " + deviceName + "<br/>";
                            Dictionary<string, object> folderProps = new Dictionary<string, object>();

                            int dirCount = device.GetDirectories().Count();

                            if(dirCount > 0)
                                folderProps.Add("folderLevel", "device");
                            else
                                folderProps.Add("folderLevel", "service");
                            deviceNodeId = CreateVUINode(deviceName, platformNodeId, VUI_FOLDERTYPE, folderProps);
                        }

                        ProcessImagesForFolder(device, deviceNodeId, importTag, dateVal, platformName, deviceName, String.Empty);

                        //Loop through Service folders
                        //var deviceRoot = new Node(deviceNodeId);
                        Document deviceDoc = new Document(deviceNodeId);
                        List<Document> serviceDocs = deviceDoc.Children.ToList<Document>();
                        // Nodes serviceNodes = deviceRoot.Children;
                        foreach (DirectoryInfo service in device.GetDirectories())
                        {
                            bool serviceExists = false;
                            int serviceNodeId = 0;
                            string serviceName = service.Name.Trim();
                            //foreach (Node n in serviceNodes)
                            foreach(Document n in serviceDocs)
                            {
                                if (n.ContentType.Text.Equals(VUI_FOLDERTYPE) && n.Text.Trim().Equals(serviceName))
                                {
                                    serviceExists = true;
                                    serviceNodeId = n.Id;
                                }
                            }
                            // Create new Service
                            if (!serviceExists)
                            {
                                Lit1.Text += @" --- Creating Service: " + serviceName + "<br/>";
                                Dictionary<string, object> folderProps = new Dictionary<string, object>();
                                folderProps.Add("folderLevel", "service");
                                serviceNodeId = CreateVUINode(serviceName, deviceNodeId, VUI_FOLDERTYPE, folderProps);
                            }

                            // Finally, process any images
                            ProcessImagesForFolder(service, serviceNodeId, importTag, dateVal, platformName, deviceName, serviceName);
                        }
                    }
                }
            }
            // Copy Generated Images into media folder
            foreach (FileInfo f in fol_full.GetFiles())
            {
 //             Response.Write(f.FullName + " -> " + Path.Combine(Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER) + f.Name + " (" + Path.Combine(Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER, f.Name) + ")<Br/>");
                File.Copy(f.FullName, Path.Combine(Server.MapPath(VUI_mediafolder), VUI_FULL_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_lg.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(Server.MapPath(VUI_mediafolder), VUI_LG_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_md.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(Server.MapPath(VUI_mediafolder), VUI_MD_FOLDER, f.Name), true);
            }
            foreach (FileInfo f in fol_th.GetFiles())
            {
                File.Copy(f.FullName, Path.Combine(Server.MapPath(VUI_mediafolder), VUI_TH_FOLDER, f.Name), true);
            }
        }



        protected void ProcessImagesForFolder(DirectoryInfo folder, int nodeId, string importTag, string dateVal, string name1, string name2, string name3)
        {
            FileInfo[] images = folder.GetFiles();

            foreach (FileInfo image in images)
            {
                string imageName = String.Empty;
                if (!String.IsNullOrEmpty("name3"))
                {
                    imageName = (dateVal + "-" + name1 + "-" + name2 + "-" + name3 + "-" + image.Name).Replace(' ', '-');
                }
                else
                {
                    imageName = (dateVal + "-" + name1 + "-" + name2 + "-" + image.Name).Replace(' ', '-');
                }

                if (!image.Extension.Contains("jpg"))
                {
                    Regex re = new Regex(@"\.[A-z]+$");
                    imageName = re.Replace(imageName, ".jpg");
                }
                imageName.Replace(@"&", @"and");

                // File.Copy(image.FullName, Path.Combine(fol_full.FullName, imageName), true);

                Resize(image.FullName, Path.Combine(fol_full.FullName, imageName), VUI_maxwidth_full);
                //File.Copy(image.FullName, Path.Combine(fol_full.FullName, imageName), true);
                Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_lg.FullName, imageName), VUI_maxwidth_lg);
                Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_md.FullName, imageName), VUI_maxwidth_md);
                Resize(Path.Combine(fol_full.FullName, imageName), Path.Combine(fol_th.FullName, imageName), VUI_maxwidth_th);



                var serviceRoot = new Document(nodeId);
                bool serviceIsPublished = serviceRoot.Published;

                List<Document> imageNodes = serviceRoot.Children.ToList<Document>();
                bool imageExists = false;

                foreach (Document  n in imageNodes)
                {
                    if (n.ContentType.Text.Equals(VUI_IMAGETYPE) && n.Text.Trim().Equals(imageName))
                    {
                        imageExists = true;
                    }
                }

                if (!imageExists)
                {
                    Dictionary<string, object> imageProps = new Dictionary<string, object>();
                    imageProps.Add("platform", name1);
                    if (String.IsNullOrEmpty(name3))
                    {
                        imageProps.Add("service", name2);
                    }
                    else
                    {
                        imageProps.Add("device", name2);
                        imageProps.Add("service", name3);
                    }
                    imageProps.Add("vuidate", dateVal);
                    imageProps.Add("imageFile", imageName);
                    imageProps.Add("lgFile", imageName);
                    imageProps.Add("thFile", imageName);
                    imageProps.Add("importTag", importTag);

                    int imageId = CreateVUINodePublish(imageName, nodeId, VUI_IMAGETYPE, imageProps, serviceIsPublished);
                }
            }
        }



        private int CreateVUINode(string nodeName, int parentNode, string documentType)
        {
            return CreateVUINode(nodeName, parentNode, documentType, null);
        }

        private int CreateVUINode(string nodeName, int parentNode, string documentType, Dictionary<string, object> nodeProps)
        {
            return CreateVUINodePublish(nodeName, parentNode, documentType, nodeProps, true);
        }
        private int CreateVUINodeNoPublish(string nodeName, int parentNode, string documentType, Dictionary<string, object> nodeProps)
        {
            return CreateVUINodePublish(nodeName, parentNode, documentType, nodeProps, false);
        }

        private void UpdateVUINodePublish(int id, Dictionary<string, object> nodeProps, bool publish)
        {
            Document d = new Document(id);
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
                d.Publish(u);
                umbraco.library.UpdateDocumentCache(d.Id);
            }
        }

        private int CreateVUINodePublish(string nodeName, int parentNode, string documentType , Dictionary<string, object> nodeProps, bool publish)
        {
            // The documenttype that should be used, replace 10 with the id of your documenttype
            DocumentType dt = DocumentType.GetByAlias(documentType);

            // The umbraco user that should create the document,
            // 0 is the umbraco system user, and always exists
            

            // Create the document
            Document d = Document.MakeNew(nodeName, dt, u, parentNode);
            if (nodeProps != null)
            {
                foreach(string k in nodeProps.Keys)
                {
                    d.getProperty(k).Value = nodeProps[k];
                }
            }
            d.Save();
            if (publish)
            {
                d.Publish(u);
                umbraco.library.UpdateDocumentCache(d.Id);
            }
            return d.Id;
        }


        private void UpdateVUINode(int nodeId, Dictionary<string, string> nodeProps)
        {
            User u = new User("websitecontentuser");
            Document d = new Document(nodeId);
            if (nodeProps != null)
            {
                foreach (string k in nodeProps.Keys)
                {
                    d.getProperty(k).Value = nodeProps[k];
                }
            }
            d.Save();
            d.Publish(u);
            umbraco.library.UpdateDocumentCache(d.Id);
        }


        public void Resize(string imageFile, string outputFile, int maxWidth)
        {
            using (var srcImage = System.Drawing.Image.FromFile(imageFile))
            {
                double scaleFactor = 0;

                if(srcImage.Width > maxWidth)
                {
                    scaleFactor = (double)maxWidth / (double)srcImage.Width;
                }
                else {
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

        /// <summary>
        /// Resize and get orientation
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>

        private ImageCodecInfo GetEncoder(ImageFormat format)
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


        protected void UpdateServiceAppStoreURLs(object sender, EventArgs e)
        {
            Dictionary<int, string> services = VUIDataFunctions.GetServiceAppStoreURLs();
            User u = new User("websitecontentuser");
            foreach (int serviceid in services.Keys)
            {
                Document s = new Document(serviceid);
                s.getProperty("appStoreURL").Value = services[serviceid];
                s.Save();
                if (s.Published)
                {
                    s.Publish(u);
                    umbraco.library.UpdateDocumentCache(serviceid);
                }

            }
        }

        protected void UpdateServiceSocialURLs(object sender, EventArgs e)
        {
            Dictionary<int, string> twitterDic = new Dictionary<int, string>();
            twitterDic.Add(16935, @"https://twitter.com/4oD");
            twitterDic.Add(16937, @"https://twitter.com/ABCNetwork");
            twitterDic.Add(16938, @"https://twitter.com/ABCFamily");
            twitterDic.Add(16940, @"https://twitter.com/adultswim");
            twitterDic.Add(16941, @"https://twitter.com/AJEnglish");
            twitterDic.Add(16942, @"https://twitter.com/AmazonVideo");
            twitterDic.Add(16944, @"https://twitter.com/iTunesMusic");
            twitterDic.Add(16945, @"https://twitter.com/ARTEfr");
            twitterDic.Add(16947, @"https://twitter.com/BBCiPlayer");
            twitterDic.Add(16950, @"https://twitter.com/BBCSport");
            twitterDic.Add(16953, @"https://twitter.com/bfmtv");
            twitterDic.Add(16955, @"https://twitter.com/blinkbox");
            twitterDic.Add(16956, @"https://twitter.com/blip");
            twitterDic.Add(16957, @"https://twitter.com/BloombergTV");
            twitterDic.Add(16959, @"https://twitter.com/Bravotv");
            twitterDic.Add(16960, @"https://twitter.com/canalplus");
            twitterDic.Add(16961, @"https://twitter.com/cartoonnetwork");
            twitterDic.Add(16962, @"https://twitter.com/CBSTweet");
            twitterDic.Add(16963, @"https://twitter.com/CBSSports");
            twitterDic.Add(16966, @"https://twitter.com/CNN");
            twitterDic.Add(16967, @"https://twitter.com/cnni");
            twitterDic.Add(16969, @"https://twitter.com/ComedyCentral");
            twitterDic.Add(16970, @"https://twitter.com/crackle");
            twitterDic.Add(16972, @"https://twitter.com/DailymotionUSA");
            twitterDic.Add(16974, @"https://twitter.com/channel5_tv");
            twitterDic.Add(16979, @"https://twitter.com/Discovery");
            twitterDic.Add(16981, @"https://twitter.com/disney");
            twitterDic.Add(16982, @"https://twitter.com/DisneyPictures");
            twitterDic.Add(16983, @"https://twitter.com/eonline");
            twitterDic.Add(16984, @"https://twitter.com/espn");
            twitterDic.Add(16986, @"https://twitter.com/Eurosport");
            twitterDic.Add(16991, @"https://twitter.com/FoxNews");
            twitterDic.Add(16992, @"https://twitter.com/OnFrequency");
            twitterDic.Add(16993, @"https://twitter.com/Gulli_fr");
            twitterDic.Add(16995, @"https://twitter.com/HBOGO");
            twitterDic.Add(16996, @"https://twitter.com/HISTORY");
            twitterDic.Add(16999, @"https://twitter.com/hulu");
            twitterDic.Add(17001, @"https://twitter.com/itele");
            twitterDic.Add(17003, @"https://twitter.com/itvplayer");
            twitterDic.Add(17006, @"https://twitter.com/LOVEFiLM");
            twitterDic.Add(17008, @"https://twitter.com/M6lachaine");
            twitterDic.Add(17009, @"https://twitter.com/Cinemax");
            twitterDic.Add(17013, @"https://twitter.com/MTVUK");
            twitterDic.Add(17014, @"https://twitter.com/MTV");
            twitterDic.Add(17018, @"https://twitter.com/nbc");
            twitterDic.Add(17022, @"https://twitter.com/NetflixUK");
            twitterDic.Add(17023, @"https://twitter.com/netflix");
            twitterDic.Add(17024, @"https://twitter.com/NOWTV");
            twitterDic.Add(17026, @"https://twitter.com/PBS");
            twitterDic.Add(17033, @"https://twitter.com/RTEplayer");
            twitterDic.Add(17035, @"https://twitter.com/SHO_Network");
            twitterDic.Add(17036, @"https://twitter.com/showyouapp");
            twitterDic.Add(17037, @"https://twitter.com/SkyHD");
            twitterDic.Add(17042, @"https://twitter.com/smithsonian");
            twitterDic.Add(17045, @"https://twitter.com/syfy");
            twitterDic.Add(17046, @"https://twitter.com/tbsveryfunny");
            twitterDic.Add(17047, @"https://twitter.com/tedtalks");
            twitterDic.Add(17048, @"https://twitter.com/CW_network");
            twitterDic.Add(17052, @"https://twitter.com/tntweknowdrama");
            twitterDic.Add(17056, @"https://twitter.com/VEVO");
            twitterDic.Add(17058, @"https://twitter.com/Videojug");
            twitterDic.Add(17059, @"https://twitter.com/viewster");
            twitterDic.Add(17060, @"https://twitter.com/Viki");
            twitterDic.Add(17061, @"https://twitter.com/Vimeo");
            twitterDic.Add(17063, @"https://twitter.com/vudufans");
            twitterDic.Add(17064, @"https://twitter.com/W9lachaine");
            twitterDic.Add(17066, @"https://twitter.com/XFINITY_TV");
            twitterDic.Add(17067, @"https://twitter.com/YouTube");

            foreach (int id in twitterDic.Keys)
            {
                Document d = new Document(id);

                bool publishDoc = d.Published;

                string tmp = d.getProperty("twitterURL").Value.ToString();

                litSocialURLs.Text += String.Format(@"<li>{0} (Twitter) Existing [{1}] New [{2}]", new object[] { d.Text, tmp, twitterDic[id] }); ;

                if (!tmp.Equals(twitterDic[id]))
                {
                    litSocialURLs.Text += " !UPDATING!";
                    d.getProperty("twitterURL").Value = twitterDic[id];

                    d.Save();

                    if (publishDoc)
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
                }
                litSocialURLs.Text += "</li>";
            }


            Dictionary<int, string> fb = new Dictionary<int, string>();
            fb.Add(16938, @"https://www.facebook.com/abcfamily");
            fb.Add(16937, @"https://www.facebook.com/ABCNetwork?ref=ts");
            fb.Add(16940, @"https://www.facebook.com/adultswim");
            fb.Add(16941, @"https://www.facebook.com/aljazeera");
            fb.Add(16942, @"https://www.facebook.com/AmazonInstantVideo");
            fb.Add(16945, @"https://www.facebook.com/artetv");
            fb.Add(16950, @"https://www.facebook.com/BBCSport");
            fb.Add(16953, @"https://www.facebook.com/BFMTV");
            fb.Add(16955, @"https://www.facebook.com/blinkbox");
            fb.Add(16956, @"https://www.facebook.com/blip");
            fb.Add(16957, @"https://www.facebook.com/BloombergTelevision");
            fb.Add(16959, @"https://www.facebook.com/BRAVO");
            fb.Add(16960, @"https://www.facebook.com/canalplus");
            fb.Add(16961, @"https://www.facebook.com/CartoonNetwork");
            fb.Add(16962, @"https://www.facebook.com/CBS");
            fb.Add(16963, @"https://www.facebook.com/CBSSports");
            fb.Add(16935, @"https://www.facebook.com/Channel4");
            fb.Add(16974, @"https://www.facebook.com/channel5uk");
            fb.Add(17009, @"https://www.facebook.com/cinemax");
            fb.Add(16966, @"https://www.facebook.com/cnn");
            fb.Add(16967, @"https://www.facebook.com/cnninternational");
            fb.Add(16969, @"https://www.facebook.com/ComedyCentral");
            fb.Add(16970, @"https://www.facebook.com/crackle");
            fb.Add(16972, @"https://www.facebook.com/dailymotion");
            fb.Add(16979, @"https://www.facebook.com/DiscoveryChannel");
            fb.Add(16981, @"https://www.facebook.com/Disney");
            fb.Add(16983, @"https://www.facebook.com/eonline");
            fb.Add(16984, @"https://www.facebook.com/ESPN");
            fb.Add(16986, @"https://www.facebook.com/Eurosport");
            fb.Add(16991, @"https://www.facebook.com/FoxNews");
            fb.Add(16992, @"https://www.facebook.com/frequency");
            fb.Add(16993, @"https://www.facebook.com/GulliOfficiel");
            fb.Add(16995, @"https://www.facebook.com/HBO?fref=ts");
            fb.Add(16996, @"https://www.facebook.com/History");
            fb.Add(16999, @"https://www.facebook.com/hulu");
            fb.Add(17001, @"https://www.facebook.com/itele.fr");
            fb.Add(16944, @"https://www.facebook.com/iTunes");
            fb.Add(17003, @"https://www.facebook.com/itv");
            fb.Add(17006, @"https://www.facebook.com/LOVEFILM");
            fb.Add(17008, @"https://www.facebook.com/M6?fref=ts&rf=115111625170922");
            fb.Add(17014, @"https://www.facebook.com/MTV");
            fb.Add(17013, @"https://www.facebook.com/mtvuk");
            fb.Add(17018, @"https://www.facebook.com/nbc");
            fb.Add(17023, @"https://www.facebook.com/netflix");
            fb.Add(17022, @"https://www.facebook.com/NetflixUK");
            fb.Add(17024, @"https://www.facebook.com/nowtv");
            fb.Add(17044, @"https://www.facebook.com/pages/Stvplayer/184229054932961");
            fb.Add(17026, @"https://www.facebook.com/pbs");
            fb.Add(17033, @"https://www.facebook.com/rteplayer");
            fb.Add(17035, @"https://www.facebook.com/showtime");
            fb.Add(17036, @"https://www.facebook.com/showyouapp");
            fb.Add(17037, @"https://www.facebook.com/sky");
            fb.Add(17042, @"https://www.facebook.com/Smithsonian");
            fb.Add(17045, @"https://www.facebook.com/Syfy");
            fb.Add(17046, @"https://www.facebook.com/TBSveryfunny");
            fb.Add(17047, @"https://www.facebook.com/TED");
            fb.Add(17048, @"https://www.facebook.com/TheCW");
            fb.Add(17052, @"https://www.facebook.com/TNTweknowdrama");
            fb.Add(17056, @"https://www.facebook.com/VEVO");
            fb.Add(17058, @"https://www.facebook.com/videojug");
            fb.Add(17059, @"https://www.facebook.com/Viewster");
            fb.Add(17060, @"https://www.facebook.com/viki");
            fb.Add(17061, @"https://www.facebook.com/Vimeo");
            fb.Add(17063, @"https://www.facebook.com/vudufans?ref=ts");
            fb.Add(17064, @"https://www.facebook.com/W9lachaine");
            fb.Add(16982, @"https://www.facebook.com/WaltDisneyStudios");
            fb.Add(17066, @"https://www.facebook.com/xfinity");
            fb.Add(17067, @"https://www.facebook.com/youtube");

            foreach (int id in fb.Keys)
            {
                Document d = new Document(id);

                bool publishDoc = d.Published;

                string tmp = d.getProperty("facebookURL").Value.ToString();

                litSocialURLs.Text += String.Format(@"<li>{0} (Facebook) Existing [{1}] New [{2}]", new object[] { d.Text, tmp, fb[id] }); ;

                if (!tmp.Equals(fb[id]))
                {
                    litSocialURLs.Text += " !UPDATING!";
                    d.getProperty("facebookURL").Value = fb[id];

                    d.Save();

                    if (publishDoc)
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
                }
                litSocialURLs.Text += "</li>";
            }

            Dictionary<int, string>  yt = new Dictionary<int, string>();
            yt.Add(16935, @"http://www.youtube.com/user/4oD");
            yt.Add(16938, @"http://www.youtube.com/user/abcfamily");
            yt.Add(16937, @"http://www.youtube.com/user/ABCNetwork");
            yt.Add(16940, @"http://www.youtube.com/user/adultswim");
            yt.Add(16941, @"http://www.youtube.com/user/AlJazeeraEnglish");
            yt.Add(16947, @"http://www.youtube.com/user/BBC");
            yt.Add(16950, @"http://www.youtube.com/user/BBCSportLive");
            yt.Add(16953, @"http://www.youtube.com/user/BFMTV");
            yt.Add(16955, @"http://www.youtube.com/user/blinkboxlive");
            yt.Add(16957, @"http://www.youtube.com/user/Bloomberg");
            yt.Add(16960, @"http://www.youtube.com/user/canalplusfrance");
            yt.Add(16961, @"http://www.youtube.com/user/cartoonnetwork");
            yt.Add(16962, @"http://www.youtube.com/user/CBS");
            yt.Add(16963, @"http://www.youtube.com/user/CBSSports");
            yt.Add(17009, @"http://www.youtube.com/user/Cinemax");
            yt.Add(16967, @"http://www.youtube.com/user/CNNInternational");
            yt.Add(16969, @"http://www.youtube.com/user/comedycentral");
            yt.Add(16970, @"http://www.youtube.com/user/CrackleUK");
            yt.Add(17048, @"http://www.youtube.com/user/CWtelevision");
            yt.Add(16974, @"http://www.youtube.com/user/demandfive");
            yt.Add(16979, @"http://www.youtube.com/user/DiscoveryNetworks");
            yt.Add(16981, @"http://www.youtube.com/user/Disney");
            yt.Add(16984, @"http://www.youtube.com/user/ESPN");
            yt.Add(16991, @"http://www.youtube.com/user/FoxNewsChannel");
            yt.Add(16993, @"http://www.youtube.com/user/gulli");
            yt.Add(16995, @"http://www.youtube.com/user/HBO?feature=playlist");
            yt.Add(16996, @"http://www.youtube.com/user/historychannel");
            yt.Add(16999, @"http://www.youtube.com/user/huluDotCom");
            yt.Add(17003, @"http://www.youtube.com/user/ITV1");
            yt.Add(17006, @"http://www.youtube.com/user/lovefilm");
            yt.Add(17014, @"http://www.youtube.com/user/MTV");
            yt.Add(17013, @"http://www.youtube.com/user/MTVUKofficial");
            yt.Add(17018, @"http://www.youtube.com/user/NBC");
            yt.Add(17022, @"http://www.youtube.com/user/NetflixNowPlaying");
            yt.Add(17023, @"http://www.youtube.com/user/NetflixNowPlaying");
            yt.Add(17024, @"http://www.youtube.com/user/nowtvofficial");
            yt.Add(17026, @"http://www.youtube.com/user/PBS");
            yt.Add(17033, @"http://www.youtube.com/user/rte");
            yt.Add(17037, @"http://www.youtube.com/user/SKYOfficial");
            yt.Add(17042, @"http://www.youtube.com/user/SmithsonianVideos");
            yt.Add(17045, @"http://www.youtube.com/user/syfy");
            yt.Add(17046, @"http://www.youtube.com/user/TBS");
            yt.Add(17047, @"http://www.youtube.com/user/TEDtalksDirector");
            yt.Add(17056, @"http://www.youtube.com/user/VEVO");
            yt.Add(16959, @"http://www.youtube.com/user/VideoByBravo");
            yt.Add(17058, @"http://www.youtube.com/user/VideoJug");
            yt.Add(17060, @"http://www.youtube.com/user/VIKI");
            yt.Add(17066, @"http://www.youtube.com/user/xfinity");
            yt.Add(17067, @"http://www.youtube.com/user/YouTube");
            yt.Add(17059, @"http://www.youtube.com/viewstertv");
            yt.Add(16966, @"http://www.youtube.com/user/CNN");

            foreach (int id in yt.Keys)
            {
                Document d = new Document(id);

                bool publishDoc = d.Published;

                string tmp = d.getProperty("youTubeURL").Value.ToString();

                litSocialURLs.Text += String.Format(@"<li>{0} (YouTube) Existing [{1}] New [{2}]", new object[] { d.Text, tmp, yt[id] }); ;

                if (!tmp.Equals(yt[id]))
                {
                    litSocialURLs.Text += " !UPDATING!";
                    d.getProperty("youTubeURL").Value = yt[id];

                    d.Save();

                    if (publishDoc)
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
                }
                litSocialURLs.Text += "</li>";
            }
        }
    }
}