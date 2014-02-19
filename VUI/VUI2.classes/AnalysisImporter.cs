using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;

namespace VUI.VUI2.classes
{
    public static class AnalysisImporter
    {
        public static List<AnalysisImport> GetAnalysisRecords(string workPackageId)
        {
            List<AnalysisImport> a = new List<AnalysisImport>();
            string sql = String.Format(@"select [TIMESTAMP], [COUNTRY], TESTINGDEVICE, FEATURELIST, HOTLIST, [PLATFORM], DEVICE, SERVICENAME, WORKPACKAGEID, IMPORTDATETAG, PLATFORMID, DEVICEID  from vui_WorkPackageImport where WORKPACKAGEID = '{0}'", workPackageId);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    AnalysisImport ai = new AnalysisImport();
                    ai.Timestamp = (DateTime)sr[0];
                    ai.Country = sr[1].ToString();
                    ai.TestingDevice = sr[2].ToString();
                    ai.FeatureList = sr[3].ToString();
                    ai.HotList = sr[4].ToString();
                    ai.Platform = sr[5].ToString();
                    ai.Device = sr[6].ToString();
                    ai.ServiceName = sr[7].ToString();
                    ai.WorkPackageId = sr[8].ToString();
                    ai.ImportDateTag = sr[9].ToString();
                    ai.PlatformId = (Int32)sr[10];
                    if (!sr.IsDBNull(11))
                    {
                        ai.DeviceId = (Int32)sr[11];
                    }
                    a.Add(ai);
                }
                conn.Close();
            }
            return a;
        }
    }

    public class AnalysisImport
    {
        public DateTime Timestamp { get; set; }
        public string Country { get; set; }
        public string TestingDevice { get; set; }
        public string FeatureList { get; set; }
        public string HotList { get; set; }
        public string Platform { get; set; }
        public string Device { get; set; }
        public string ServiceName { get; set; }
        public string WorkPackageId { get; set; }
        public string ImportDateTag { get; set; }
        public int PlatformId { get; set; }
        public int? DeviceId { get; set; }
        public bool ServiceIsPublic { get; set; }

        public AnalysisImport()
        {
            ServiceIsPublic = false;
        }
        public int GetParentNodeId()
        {
            if (DeviceId.HasValue)
            {
                return (int)DeviceId;
            }
            else
            {
                return PlatformId;
            }
        }
        public int GetServiceDocumentId()
        {
            Document parent = new Document(DeviceId.GetValueOrDefault(PlatformId));
            Document service = null;
            try
            {
                service = parent.Children.First(n => n.Text.ToLower().Equals(ServiceName.ToLower()));
                ServiceIsPublic = service.Published;
            }
            catch (Exception e) { ; }
            if (service == null)
            {
                DocumentType dt = DocumentType.GetByAlias("VUI2Service"); 
                User u = User.GetAllByLoginName("websitecontentuser", false).First(); 
                service = Document.MakeNew(ServiceName, dt, u, parent.Id);
                ServiceIsPublic = false;
            }
            return service.Id;
        }
    }
}