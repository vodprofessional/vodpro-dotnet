using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using System.Configuration;
using umbraco.MacroEngines;
using VUI.VUI3.classes;

namespace VUI.VUI2.classes
{
    public class VUI2_MetaDataHandler : ApplicationBase
    {

        public VUI2_MetaDataHandler()
        {
            Document.AfterSave += new Document.SaveEventHandler(Document_AfterSave);
            Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);
            Document.AfterDelete += new Document.DeleteEventHandler(Document_AfterDelete);
            Document.AfterUnPublish += new Document.UnPublishEventHandler(Document_AfterUnPublish);
        }

        void Document_AfterSave(Document sender, umbraco.cms.businesslogic.SaveEventArgs e)
        {
            if (sender.ContentType.Alias.Equals(Utility.GetConst("VUI2_analysistype")))
            {
                if (sender.getProperty("serviceCapabilities") != null)
                {
                    int totalscore = 0;

                    if (!String.IsNullOrEmpty(sender.getProperty("serviceCapabilities").Value.ToString()))
                    {
                        string[] capabilities = sender.getProperty("serviceCapabilities").Value.ToString().Split(',');
                        totalscore = capabilities.Length;
                    }
                    sender.getProperty("serviceScore").Value = totalscore;
                }
                else
                {
                    sender.getProperty("serviceScore").Value = 0;
                }
            }

            if (sender.ContentType.Alias.Equals(Utility.GetConst("VUI2_servicetype")))
            {

            }
        }

        // After publishing Services, or Analyses, we want to update the metadata
        void Document_AfterPublish(Document sender, umbraco.cms.businesslogic.PublishEventArgs e)
        {
            // If the item is a Service
            if (sender.ContentType.Alias.Equals(Utility.GetConst("VUI2_servicetype")))
            {
                // If the Service has not been added to the vui_Service metadata table, add it
                // Utility.AddOrUpdateServiceMeta(sender.Id);
                // If the Service has screenshots, then the vui_ServiceScreenshotsMeta table needs to be reevaluated.

            }

            if (sender.ContentType.Alias.Equals(Utility.GetConst("VUI2_analysistype")))
            {
                if (sender.getProperty("createNewsItemOnPublish") != null)
                {
                    if (sender.getProperty("createNewsItemOnPublish").Value.ToString().Equals("1"))
                    {
                        try
                        {
                            string platform = String.Empty;
                            string device = String.Empty;

                            Document parent = new Document(sender.ParentId); // The Service
                            Document parent1 = new Document(parent.ParentId);

                            if (parent1.ContentType.Alias.Equals(Utility.GetConst("VUI2_devicetype")))
                            {
                                device = parent1.Text;
                                Document parent2 = new Document(parent1.ParentId);
                                platform = parent2.Text;
                            }
                            else
                            {
                                platform = parent1.Text;
                            }

                            Document sm = VUI3Utility.FindServiceMasterDocumentByName(sender.Parent.Text);
                            int smid = sm.Id;
                            string pd = platform;
                            if (!String.IsNullOrEmpty(device))
                            {
                                pd = pd + " / " + device;
                            }
                            // This is not required.
                            // string description = String.Format(@"New benchmark score for {0} on {1}", new object[] { sm.Text, pd });
                            VUI3News.AddNews(VUI3News.NEWSTYPE_BENCHMARK, relatedServiceId: smid, relatedService: sm.Text, relatedPlatform: platform, relatedDevice: device);

                            sender.getProperty("createNewsItemOnPublish").Value = 0;
                        }
                        catch (Exception ex)
                        {
                            string m = ex.Message;
                        }
                    }
                }

            }

        }

        void Document_AfterDelete(Document sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        {

        }

        void Document_AfterUnPublish(Document sender, umbraco.cms.businesslogic.UnPublishEventArgs e)
        {
            if (sender.ContentType.Alias.Equals("VUI2ServiceMaster"))
            {
                string serviceName = sender.Text;

                DynamicNode root = new DynamicNode(Int32.Parse(ConfigurationManager.AppSettings["VUI2_rootnodeid"].ToString()));

                List<DynamicNode> serviceNodesToUnpublish = root.Descendants(ConfigurationManager.AppSettings["VUI2_servicetype"].ToString()).Items.Where(n => n.Name.Equals(serviceName)).ToList();

                foreach (DynamicNode s in serviceNodesToUnpublish)
                {
                    Document sd = new Document(s.Id);
                    sd.UnPublish();
                    umbraco.library.UpdateDocumentCache(sd.Id);
                }
                
            }
        }


    }
}