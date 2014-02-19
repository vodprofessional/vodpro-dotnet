using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;


namespace VUI.VUI2.classes
{
    public class Migrator
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Migrator));

        private int vui1rootnodeid;
        private int vui2rootnodeid;
        private User u;

        public Migrator()
        {
            vui1rootnodeid = Int32.Parse(Utility.GetConst("umb_vuiFolderRoot"));
            vui2rootnodeid = Int32.Parse(Utility.GetConst("VUI2_rootnodeid"));
            u = new User("websitecontentuser");
        }

        public void Migrate()
        {
            log.Debug(" *************************************** ");
            log.Debug(" * BEGINNING MIGRATION TO VUI2         * ");
            log.Debug(" *************************************** ");
            // THis will re-create the VUI document tree, leaving everything unpublished.
            Document vui1root = new Document(vui1rootnodeid);
            Document vui2root = new Document(vui2rootnodeid);
            string folderLevel = String.Empty;


            // Start with the platforms.
            // Cycle through the vui1 root children, picking out the vuiServiceFolder items
            Document[] l1children = vui1root.Children;
            
            // Top level should all be 
            foreach (Document l1child in l1children)
            {
                if (l1child.ContentType.Alias.Equals("VUI_Folder"))
                {
                    log.Debug(" L1 FOLDER: " + l1child.Id + ";" + l1child.Text);

                    folderLevel = l1child.getProperty("folderLevel").Value.ToString();
                    if (folderLevel.Equals("platform"))
                    {
                        string platformName = l1child.Text;

                        DocumentType dtPlatform = DocumentType.GetByAlias(Utility.GetConst("VUI2_platformtype"));
                        Document dPlatform = Document.MakeNew(platformName, dtPlatform, u, vui2rootnodeid);

                        dPlatform.getProperty("folderImage").Value = l1child.getProperty("folderImage").Value;
                        dPlatform.Save();
                        int vui2platformid = dPlatform.Id;

                        // now get the child nodes
                        // These could be device or service level nodes
                        Document[] l2children = l1child.Children;
                        foreach (Document l2child in l2children)
                        {
                            if (l2child.ContentType.Alias.Equals("VUI_Folder"))
                            {
                                folderLevel = l2child.getProperty("folderLevel").Value.ToString();
                                if (folderLevel.Equals("device"))
                                {
                                    string deviceName = l2child.Text;

                                    DocumentType dt = DocumentType.GetByAlias(Utility.GetConst("VUI2_devicetype"));
                                    Document d = Document.MakeNew(deviceName, dt, u, vui2platformid);

                                    d.getProperty("folderImage").Value = l2child.getProperty("folderImage").Value;
                                    d.Save();

                                    int vui2deviceid = d.Id;

                                    // now get the child nodes
                                    // These should be service level nodes
                                    Document[] l3children = l2child.Children;
                                    foreach (Document l3child in l3children)
                                    {
                                        if (l3child.ContentType.Alias.Equals("VUI_Folder"))
                                        {
                                            folderLevel = l3child.getProperty("folderLevel").Value.ToString();
                                            if (folderLevel.Equals("service"))
                                            {
                                                CreateService(l3child, vui2deviceid);
                                            }
                                        }
                                    }
                                }
                                else if (folderLevel.Equals("service"))
                                {
                                    CreateService(l2child, vui2platformid);
                                }
                            }
                        }
                    }
                }
            }
        }


        public void MigratePart2()
        {

            log.Debug(" *************************************** ");
            log.Debug(" * BEGINNING MIGRATION TO VUI2 Pt 2    * ");
            log.Debug(" *************************************** ");
            log.Debug(" Cycling through all descndandts of VUI2 root ");

            Document vui2root = new Document(vui2rootnodeid);

            foreach (Document d in vui2root.GetDescendants())
            {
                if (d.ContentType.Alias.Equals(Utility.GetConst("VUI2_servicetype")))
                {
                    MigrateService(d);
                }
            }

        }


        // Create a Service Stub Item.
        // We will come back and update these in round 2.
        private void CreateService(Document sourceService, int parentId)
        {
            DocumentType dt = DocumentType.GetByAlias(Utility.GetConst("VUI2_servicetype"));
            Document d = Document.MakeNew(sourceService.Text, dt, u, parentId);

            d.getProperty("migrationV1NodeId").Value = sourceService.Id;
            d.getProperty("migrationProcessed").Value = false;

            d.Save();
        }

        // This is used to migrate the values from the VUI_Folder item to the various parts of
        // the new VUI2.Service item, VUI2.Analysis and the screenshots
        private void MigrateService(Document service)
        {
            log.Debug(" - Found " + service.Id + "-" + service.Text);

            int sourceid;
            if(Int32.TryParse(service.getProperty("migrationV1NodeId").Value.ToString(), out sourceid))
            {
                Document sourceService = new Document(sourceid);
                log.Debug(" -- Ready to migrate from " + sourceid + " at " + sourceService.Text );

                if(sourceService.getProperty("description") != null)
                    service.getProperty("description").Value = sourceService.getProperty("description").Value;

                if (sourceService.getProperty("regionAvailability").Value != null)
                    service.getProperty("availability").Value = sourceService.getProperty("regionAvailability").Value;
                if (sourceService.getProperty("subscriptionType").Value != null)
                    service.getProperty("subscriptionType").Value = sourceService.getProperty("subscriptionType").Value;
                if (sourceService.getProperty("serviceCategory").Value != null)
                    service.getProperty("serviceCategory").Value = sourceService.getProperty("serviceCategory").Value;
                if (sourceService.getProperty("isComingSoon").Value != null)
                    service.getProperty("isComingSoon").Value = sourceService.getProperty("isComingSoon").Value;
                if (sourceService.getProperty("ratings").Value != null)
                    service.getProperty("ratings").Value = sourceService.getProperty("ratings").Value;

                service.Save();
                log.Debug(" -- Saved base properties");

                // Process the Benchmarking and Screenshots
                if (service.getProperty("migrationProcessed").Value.Equals(0))
                {
                    int serviceScore = -1;
                    Int32.TryParse(sourceService.getProperty("vuiScore").Value.ToString(), out serviceScore);

                    // If there are screenshot Children or a score > 0 then put in the analysis object
                    if (sourceService.HasChildren || serviceScore > 0)
                    {

                        log.Debug(" -- Has Childern: " + sourceService.HasChildren.ToString() +  "; score: " + serviceScore);


                        DocumentType dt = DocumentType.GetByAlias(Utility.GetConst("VUI2_analysistype"));
                        Document analysis = Document.MakeNew("August 2012", dt, u, service.Id);
                        analysis.getProperty("analysisDate").Value = new DateTime(2012,8,31);
                        analysis.Save();

                        if (sourceService.HasChildren)
                        {
                            log.Debug(" -- Moving " + sourceService.Children.Count().ToString() + " screenshot children ...");
                            // move all the screenshots over
                            Document[] screenshots = sourceService.Children;
                            foreach (Document screenshot in screenshots)
                            {
                                screenshot.Move(analysis.Id);
                            }
                            log.Debug(" -- ... finished moving screenshots");

                        }

                        if (serviceScore > 0)
                        {
                            log.Debug(" -- Benchmarked with score of " + serviceScore);
                            analysis.getProperty("serviceScore").Value = serviceScore;

                            if (sourceService.getProperty("serviceCapabilities").Value != null)
                                analysis.getProperty("serviceCapabilities").Value = sourceService.getProperty("serviceCapabilities").Value;
                            if (sourceService.getProperty("benchmarkDate").Value != null)
                                analysis.getProperty("benchmarkDate").Value = sourceService.getProperty("benchmarkDate").Value; ;
                            if (sourceService.getProperty("benchmarkDevice").Value != null)
                                analysis.getProperty("benchmarkDevice").Value = sourceService.getProperty("benchmarkDevice").Value; ;
                            analysis.Save();
                            log.Debug(" -- Saved benchmarking");
                        }
                    }
                }
                service.getProperty("migrationProcessed").Value = true;
                service.Save();
                log.Debug(" - COmpleted processing " + service.Id + "-" + service.Text);
            }
            log.Debug(" - Completed " + service.Path);
        }
    }
}