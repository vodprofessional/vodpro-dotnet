using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI3.classes
{
    public class VUI3ScreenshotAnalysis
    {
        public int Id { get; set; }
        public string DateTag { get; set; }
        public string DeviceName { get; set; }
        public string PlatformName { get; set; }
        public int DeviceId { get; set; }
        public int PlatformId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public int DefaultImageId { get; set; }
        public string DefaultImageURL_th { get; set; }
        public string DefaultImageURL_md { get; set; }
        public string DefaultImageURL_lg { get; set; }
        public string DefaultImageURL_full { get; set; }
        public int ImageCount { get; set; }

        public bool WithScreenshots { get; set; }

        private VUI3ScreenshotList _screenshots = null;
        public VUI3ScreenshotList ScreenshotsList
        {
            get
            {
                if (WithScreenshots && _screenshots == null)
                {
                    _screenshots = SetScreenshots(this.Id);
                }
                return _screenshots;
            }
        }

        public VUI3ScreenshotAnalysis() { WithScreenshots = false; }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ScreenshotAnalysis>(this);
        }

        private VUI3ScreenshotList SetScreenshots()
        {
            return SetScreenshots(Id);
        }

        public static VUI3ScreenshotList SetScreenshots(int analysisId)
        {
            VUI3ScreenshotList screenlist = new VUI3ScreenshotList();
            List<VUI3Screenshot> screenshots = new List<VUI3Screenshot>();

            string sql = @"select S.Name as ServiceName, 
                            P.Name + CASE WHEN(D.Name IS NULL) THEN '' ELSE '/' + D.Name END as DeviceName, 
                            I.* , F.CollectionName, F.DateAdded, A.DateTag
                            from [dbo].[vui_Image] I
							inner join vui_Service S on I.ServiceId = S.ID
							inner join vui_Platform P on S.PlatformID = P.ID
							inner join vui_Analysis A on I.AnalysisId = A.Id
							left outer join vui_Device D on S.DeviceID = D.ID
                            left outer join (select * from [vui_UserFavouriteImages] where UserId = @UserId) F on I.Id = F.ImageId
                            where I.AnalysisId = @AnalysisId
                            ";

            int UserId = -1;
            VUIUser u = new VUIUser();
            if (u.Member != null)
            {
                UserId = u.Member.Id;
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@UserId", UserId));
                comm.Parameters.Add(new SqlParameter("@AnalysisId", analysisId));
                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    VUI3Screenshot s = new VUI3Screenshot();

                    s.Id = (int)sr["Id"];
                    s.ServiceName = (string)sr["ServiceName"];
                    s.Device = (string)sr["DeviceName"];
                    s.PageType = (string)sr["PageType"];
                    s.ImageURL_full = (string)sr["ImageURL_full"];
                    s.ImageURL_lg = (string)sr["ImageURL_lg"];
                    s.ImageURL_md = (string)sr["ImageURL_md"];
                    s.ImageURL_th = (string)sr["ImageURL_th"];
                    s.DateCreated = (DateTime)sr["DateCreated"];
                    s.IsFavourite = (sr["CollectionName"].GetType() != typeof(DBNull));
                    if (sr["DateAdded"].GetType() != typeof(DBNull)) s.DateAdded = (DateTime)sr["DateAdded"]; 
                    s.ImportTag = (string)sr["DateTag"];
                    screenlist.screenshots.Add(s);
                }
                conn.Close();
            }

            return screenlist;
        }



        private VUI.VUI2.classes.Analysis _benchmark = null;
        public VUI.VUI2.classes.Analysis Benchmark
        {
            get
            {
                if (_benchmark == null)
                {
                    _benchmark = GetBenchmark(Id);
                }
                return _benchmark;
            }
        }

        public static VUI.VUI2.classes.Analysis GetBenchmark(int id)
        {
            VUI.VUI2.classes.Analysis benchmark = new VUI2.classes.Analysis(id);
            benchmark.SetBenchmark();
            return benchmark;
        }


    }

    public class VUI3ScreenshotAnalysisList
    {
        public int resultsCount { get { return analyses.Count; } }
        public List<VUI3ScreenshotAnalysis> analyses { get; set; }

        public VUI3ScreenshotAnalysisList() { analyses = new List<VUI3ScreenshotAnalysis>(); }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3ScreenshotAnalysisList>(this);
        }

        public bool HasScreenshotsForPlatform(string platformName, string deviceName)
        {
            if (analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)).Count() > 0)
            {
                foreach(VUI3ScreenshotAnalysis an in analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)))
                {
                    if (an.ImageCount > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool HasBenchmarksForPlatform(string platformName, string deviceName)
        {
            if (analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)).Count() > 0)
            {
                bool retval = false;
                List<VUI3ScreenshotAnalysis> anaList = analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)).ToList();
                foreach (VUI3ScreenshotAnalysis analysis in anaList)
                {
                    retval = (analysis.Benchmark.BenchmarkScore > 0) ? true : retval;
                }
                return retval;
            }
            else
            {
                return false;
            }
        }

        public List<VUI3ScreenshotAnalysis> GetScreenshotsForPlatform(string platformName, string deviceName)
        {
            if (analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)).Count() > 0)
            {
                return analyses.Where(a => a.PlatformName.Equals(platformName) && a.DeviceName.Equals(deviceName)).ToList();
            }
            else
            {
                return new List<VUI3ScreenshotAnalysis>();
            }
        }



    }
}