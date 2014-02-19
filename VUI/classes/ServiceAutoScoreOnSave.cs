using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using System.Configuration;
using VUI.VUI3.classes;
using umbraco.MacroEngines;

namespace VUI.classes
{
    public class ServiceAutoScoreOnSave : ApplicationBase
    {
        private Dictionary<string, int> scores = null;
        private string VUI_pagetype_scores = String.Empty;

        public ServiceAutoScoreOnSave()
        {
            Document.AfterSave += new Document.SaveEventHandler(Document_AfterSave);
         //   Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);

            VUI_pagetype_scores = ConfigurationManager.AppSettings["VUI_pagetype_scores"].ToString();
            InitScores();
        }

        /*void Document_AfterPublish(Document sender, umbraco.cms.businesslogic.PublishEventArgs e)
        {
            if (sender.ContentType.Alias.Equals("VUI2Analysis"))
            {
                string s = sender.getProperty("createNewsItemOnPublish").Value.ToString();
                if (sender.getProperty("createNewsItemOnPublish").Value.Equals("true"))
                {
                    int parentid = sender.ParentId;
                    
                    int serviceId;
                    string serviceName;
                    string platform = "";
                    string device = "";
                    try
                    {
                        DynamicNode s1 = new DynamicNode(parentid);

                        serviceName = s1.Name;

                        VUI3ServiceMaster sm = new VUI3ServiceMaster(serviceName);
                        serviceId = sm.Id.Value;

                        DynamicNode p1 = s1.Parent;
                        if (p1.NodeTypeAlias.Equals("VUI2Device"))
                        {
                            device = p1.Name;
                            DynamicNode p2 = p1.Parent;
                            platform = p2.Name;
                        }
                        else if (p1.NodeTypeAlias.Equals("VUI2Platform"))
                        {
                            device = String.Empty;
                            platform = p1.Name;
                        }
                        VUI3News.AddNews(newsType: VUI3News.NEWSTYPE_BENCHMARK, relatedDevice: device, relatedPlatform: platform, relatedService: serviceName, relatedServiceId: serviceId);

                    }
                    catch(Exception ex)
                    {

                    }

                    
                }
            }

        }
        */

        void Document_AfterSave(Document sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            // Before saving, set the service score
            // So, only if it's a VUI_Folder document type
            if (sender.ContentType.Alias.Equals("VUI_Folder"))
            {
                if (sender.getProperty("folderLevel") != null)
                {
                    if (sender.getProperty("folderLevel").Value.Equals("service"))
                    {
                        if (sender.getProperty("serviceCapabilities") != null)
                        {
                            int totalscore = 0;
                            
                            if (!String.IsNullOrEmpty(sender.getProperty("serviceCapabilities").Value.ToString()))
                            {
                                string[] capabilities = sender.getProperty("serviceCapabilities").Value.ToString().Split(',');
                                totalscore = capabilities.Length;
                            }
                            sender.getProperty("vuiScore").Value = totalscore;
                        }
                        else
                        {
                            sender.getProperty("vuiScore").Value = 0;
                        }
                    }
                }

            }

            if (sender.ContentType.Alias.Equals("VUI2ServiceMaster"))
            {
                string deviceAvailability = sender.getProperty("deviceAvailability").Value.ToString();
                string[] ids = deviceAvailability.Split(',');

                List<VUI3Preval> devices = VUI3Utility.GetPrevals(Int32.Parse(ConfigurationManager.AppSettings["VUI2_DeviceAvailabilityID"].ToString()));

                foreach (string sid in ids)
                {
                    int id;
                    if(Int32.TryParse(sid, out id))
                    {
                        string pd;
                        if(VUI3Utility.TryGetPrevalValue(devices, id, out pd))
                        {
                            
                            string platform = pd.Split('/')[0];

                            Document root = new Document(Int32.Parse(ConfigurationManager.AppSettings["VUI2_rootnodeid"].ToString()));
                            Document PlatformDevice = root.Children.ToList().First<Document>(n => n.Text.Equals(platform) && n.ContentType.Alias.Equals(ConfigurationManager.AppSettings["VUI2_platformtype"].ToString()));
                                
                            if (pd.Contains("/"))
                            {
                                string device = pd.Split('/')[1];
                                PlatformDevice = PlatformDevice.Children.ToList().First<Document>(n => n.Text.Equals(device) && n.ContentType.Alias.Equals(ConfigurationManager.AppSettings["VUI2_devicetype"].ToString()));
                            }

                            bool hasService = (PlatformDevice.Children.ToList().Where<Document>(n => n.Text.Equals(sender.Text) && n.ContentType.Alias.Equals(ConfigurationManager.AppSettings["VUI2_servicetype"].ToString())).Count() > 0);
                            if (!hasService)
                            {
                                User u = new User("websitecontentuser");
                                DocumentType dt = DocumentType.GetByAlias(ConfigurationManager.AppSettings["VUI2_servicetype"].ToString());
                                Document.MakeNew(sender.Text, dt, u, PlatformDevice.Id);
                            }
                        }
                    }
                }
            }
        }

        void InitScores()
        {
            scores = new Dictionary<string, int>();
            string[] scoreitems = VUI_pagetype_scores.Split(';');
            foreach (string scoreitem in scoreitems)
            {
                string[] pagetypescore = scoreitem.Split(',');
                scores.Add(pagetypescore[0], Int32.Parse(pagetypescore[1]));
            }
            scores.Add("", 0);
        }

    }
}