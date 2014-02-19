using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using umbraco.cms.businesslogic.web;
using System.Configuration;
using umbraco.BusinessLogic;
using System.Data.SqlClient;

namespace VUI.VUI2.classes
{
    public class GenerateServiceMasters
    {
        static User u = new User("websitecontentuser");
        private static int umb_vuiFolderRoot;
        private static int VUI2_ServiceMastersRoot;
        private static int VUI2_DeviceAvailabilityID;
        private static Dictionary<string, string> VUI2_DeviceAvailabilityPrevals;
        private static bool IsPreview = false;

        public static string Generate()
        {
            umb_vuiFolderRoot = Int32.Parse(ConfigurationManager.AppSettings["VUI2_rootnodeid"].ToString());
            VUI2_ServiceMastersRoot = Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString());
            VUI2_DeviceAvailabilityID = Int32.Parse(ConfigurationManager.AppSettings["VUI2_DeviceAvailabilityID"].ToString());
            VUI2_DeviceAvailabilityPrevals = new Dictionary<string,string>();

            string VUI2_ServiceMasterGen_Preview = ConfigurationManager.AppSettings["VUI2_ServiceMasterGen_Preview"].ToString();
            IsPreview = VUI2_ServiceMasterGen_Preview.ToUpper().Equals("YES");


            InitDeviceAvailabilityPreVals();

            Document smr = new Document(VUI2_ServiceMastersRoot);
            foreach (Document dd in smr.Children)
            {
                dd.delete();
            }

            StringBuilder sb = new StringBuilder();
            List<ServiceMasterMeta> serviceMasters = new List<ServiceMasterMeta>();


            Document root = new Document(umb_vuiFolderRoot);

            List<ServicesList> allServicesLists = new List<ServicesList>();


            // Loop around the platforms
            foreach (Document platform in root.Children)
            {
                if (platform.ContentType.Alias.Equals("VUI2Platform"))
                {
                    if (platform.Text.Equals("Web") && platform.HasChildren)
                    {
                        Document[] services = platform.Children.Where(z => z.ContentType.Alias.Equals("VUI2Service")).ToArray();
                        ServicesList s = new ServicesList();
                        s.Services = services;
                        s.DeviceType = platform.Text;
                        allServicesLists.Add(s);
                    }
                    else
                    {
                        foreach (Document device in platform.Children)
                        {
                            if (device.ContentType.Alias.Equals("VUI2Device"))
                            {
                                Document[] services = device.Children.Where(z => z.ContentType.Alias.Equals("VUI2Service")).ToArray();
                                ServicesList s = new ServicesList();
                                s.Services = services;
                                s.DeviceType = platform.Text + "/" + device.Text;
                                allServicesLists.Add(s);
                            }
                        }
                    }
                }
            }

            foreach (ServicesList sl in allServicesLists)
            {
                string deviceType = sl.DeviceType;

                foreach(Document s in sl.Services)
                {
                    ServiceMasterMeta smm = new ServiceMasterMeta();
                    smm.ServiceName = s.Text;
                    smm.Description = s.getProperty("description").Value;
                    smm.Availability = s.getProperty("availability").Value;
                    smm.SubscriptionType = s.getProperty("subscriptionType").Value;
                    smm.ServiceCategory = s.getProperty("serviceCategory").Value;

                    string da = GetDeviceAvailability(deviceType);
                    smm.DeviceAvailability = da;


                    try
                    {
                        ServiceMasterMeta sm = serviceMasters.Where<ServiceMasterMeta>(y => y.ServiceName.Equals(smm.ServiceName)).ToList().First();
                        string existingDA = sm.DeviceAvailability;
                        if (!String.IsNullOrEmpty(existingDA))
                        {
                            sm.DeviceAvailability = smm.DeviceAvailability + "," + existingDA; 
                        }

                        string existingDescription = sm.Description.ToString();
                        if (String.IsNullOrEmpty(existingDescription) && !String.IsNullOrEmpty(smm.Description.ToString()))
                        {
                            sm.Description = smm.Description;
                        }
                    }
                    catch (InvalidOperationException ioe)
                    {
                        serviceMasters.Add(smm);
                    }
                    catch (ArgumentNullException ane)
                    {
                        serviceMasters.Add(smm);
                    }
                    
                }
            }
            sb.AppendLine("<ul>");

            foreach (ServiceMasterMeta smm in serviceMasters.OrderBy(m => m.ServiceName).ToList())
            {
                smm.SetURLs();

                if (!IsPreview)
                {
                    DocumentType dt = DocumentType.GetByAlias("VUI2ServiceMaster");
                    // Create the document
                    Document d = Document.MakeNew(smm.ServiceName, dt, u, VUI2_ServiceMastersRoot);
                    d.Text = smm.ServiceName;
                    d.getProperty("serviceName").Value = smm.ServiceName;
                    d.getProperty("description").Value = smm.Description;
                    d.getProperty("availability").Value = smm.Availability;
                    d.getProperty("subscriptionType").Value = smm.SubscriptionType;
                    d.getProperty("serviceCategory").Value = smm.ServiceCategory;
                    d.getProperty("deviceAvailability").Value = smm.DeviceAvailability;

                    d.getProperty("twitterURL").Value = smm.twitterURL;
                    d.getProperty("facebookURL").Value = smm.facebookURL;
                    d.getProperty("youTubeURL").Value = smm.youTubeURL;
                    d.getProperty("iPhoneAppURL").Value = smm.iPhoneAppURL;
                    d.getProperty("iPadAppURL").Value = smm.iPadAppURL;
                    d.getProperty("phonePlayAppURL").Value = smm.phonePlayAppURL;
                    d.getProperty("tabletPlayAppURL").Value = smm.tabletPlayAppURL;
                    d.getProperty("phoneWindowsAppURL").Value = smm.phoneWindowsAppURL;

                    d.Save();
                }
                sb.AppendLine("<li>" + smm.ServiceName + "</li>");
            }
            sb.AppendLine("</ul>");

            // Foreach service, check to see whether it already exists in the list. If so, 
            //  it would be great to update the device availability checkboxes, but we'll come back to that.
            
            return sb.ToString();
        }

        public static void InitDeviceAvailabilityPreVals()
        {
            VUI2_DeviceAvailabilityPrevals.Clear();

            string sql = String.Format(@"select value, id from [dbo].[cmsDataTypePreValues] where datatypeNodeId = {0}", VUI2_DeviceAvailabilityID);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    VUI2_DeviceAvailabilityPrevals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }
        }

        public static string GetDeviceAvailability(string val)
        {
            if (VUI2_DeviceAvailabilityPrevals.Keys.Contains(val))
            {
                return VUI2_DeviceAvailabilityPrevals[val];
            }
            else
            {
                return String.Empty;
            }
        }


    }

    public class ServicesList
    {
        public Document[] Services { get; set; }
        public string DeviceType { get; set; }
    }

    public class ServiceMasterMeta
    {
        public string ServiceName { get; set; }
        public Object Description { get; set; }
        public Object Availability { get; set; }
        public Object SubscriptionType { get; set; }
        public Object ServiceCategory { get; set; }
        public string DeviceAvailability { get; set; }
        public string twitterURL { get; set; }
        public string facebookURL { get; set; }
        public string youTubeURL { get; set; }
        public string iPhoneAppURL { get; set; }
        public string iPadAppURL { get; set; }
        public string phonePlayAppURL { get; set; }
        public string tabletPlayAppURL { get; set; }
        public string phoneWindowsAppURL { get; set; }

        public ServiceMasterMeta()
        {
            twitterURL = String.Empty;
            facebookURL = String.Empty;
            youTubeURL = String.Empty;
            iPhoneAppURL = String.Empty;
            iPadAppURL = String.Empty;
            phonePlayAppURL = String.Empty;
            tabletPlayAppURL = String.Empty;
            phoneWindowsAppURL = String.Empty;            
        }

        public void SetURLs()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();

                string sql = @"select DeviceType, URL from vui_ServiceURL where ServiceName = '" + ServiceName + "'";

                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    string devicetype = sr[0].ToString();
                    string url = sr[1].ToString();

                    switch(devicetype)
                    {
                        case "Tablet/iPad":
                        {
                            iPadAppURL = url;
                            break;
                        }
                        case "Tablet/Android":
                        {
                            tabletPlayAppURL = url;
                            break;
                        }
                        case "Smartphone/iPhone":
                        {
                            iPhoneAppURL = url;
                            break;
                        }
                        case "Smartphone/Android":
                        {
                            phonePlayAppURL = url;
                            break;
                        }
                        case "Smartphone/Windows":
                        {
                            phoneWindowsAppURL = url;
                            break;
                        }
                        case "Twitter":
                        {
                            twitterURL = url;
                            break;
                        }
                        case "Facebook":
                        {
                            facebookURL = url;
                            break;
                        }
                        case "Youtube":
                        {
                            youTubeURL = url;
                            break;
                        }
                    }

                }
                conn.Close();
            }
        }
    }
}