using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VUI.classes;

namespace VUI.VUI3.classes
{
    public class VUI3BenchmarkMatrix
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3BenchmarkMatrix));


        private List<VUI3BenchmarkMatrixItem> _MatrixItems = null;
        public List<VUI3BenchmarkMatrixItem> MatrixItems 
        { 
            get
            {
                if(_MatrixItems == null)
                {
                    SetBenchmarkMatrix();
                }
                return _MatrixItems;
            }
        }
        private void SetBenchmarkMatrix()
        {
            _MatrixItems = new List<VUI3BenchmarkMatrixItem>();

            string[] deviceTypes = { "TabletAndroid", "SmartphoneAndroid", "TabletiPad", "SmartphoneiPhone", "SmartphoneWindows", "TabletWindows", "Web", "Connected TV-Samsung", "Connected TV-Sony", "Connected TV-LG", "Connected TV-Panasonic", "Games Consoles-XBox", "Games Consoles-Playstation", "STB-Roku", "STB-Now TV", "Overall" };

            string sql = @"vui_BenchmarkFeatureScores";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandType = CommandType.StoredProcedure;
                SqlDataReader sr = comm.ExecuteReader();

                
                while (sr.Read())
                {
                    foreach (string dt in deviceTypes)
                    {
                        VUI3BenchmarkMatrixItem m = new VUI3BenchmarkMatrixItem();
                        m.PlatformDevice = dt;
                        m.BenchmarkFeature = (string)sr["BenchmarkFeature"];
                        m.Count = (int)sr[ dt ];
                        m.Max = (int)sr[ dt + "Count"];
                        m.Score = (int)sr[dt + "Percent"];
                        _MatrixItems.Add(m);
                    }
                }
                conn.Close();
            }
        }

        public VUI3BenchmarkMatrix() { }

        public VUI3BenchmarkMatrixItem GetScores(string feature, string platformDevice)
        {
            if (MatrixItems.Count(m => m.BenchmarkFeature.Equals(feature) && m.PlatformDevice.Equals(platformDevice)) > 0)
            {
                return MatrixItems.First(m => m.BenchmarkFeature.Equals(feature) && m.PlatformDevice.Equals(platformDevice));
            }
            else
            {
                throw new Exception("No Score for that feature combination exists " + feature + "] [" + platformDevice + "]");
            }
        }

        /// <summary>
        /// Quickly tell whether a platform has a set of benchmarks to display
        /// </summary>
        /// <param name="PlatformDevice"></param>
        /// <returns></returns>
        public bool HasBenchmarksForPlatform(string PlatformDevice)
        {
            bool retval = false;
            try
            {
                VUI3BenchmarkMatrixItem pditem = MatrixItems.First(m => m.PlatformDevice.Equals(PlatformDevice));
                if(pditem.Count > 0)
                {
                    retval = true;
                }
            }
            catch(Exception ex){;}
            return retval;
        }

    }

    public struct VUI3BenchmarkMatrixItem
    {
        public string PlatformDevice { get; set; }
        public string BenchmarkFeature { get; set; }
        public int Count { get; set; }
        public int Max { get; set; }
        public int Score { get; set; }
    }
}