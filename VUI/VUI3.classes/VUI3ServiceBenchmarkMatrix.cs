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
    public class VUI3ServiceBenchmarkMatrix
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3BenchmarkMatrix));


        private List<VUI3ServiceMasterMatrixItem> _MatrixItems = null;
        public List<VUI3ServiceMasterMatrixItem> MatrixItems 
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
            _MatrixItems = new List<VUI3ServiceMasterMatrixItem>();

            string sql = @" select M.*, SM.Id, ISNULL(SM.IconURL,'') as IconURL 
                            from vui_BenchmarkMatrix M 
                            inner join vui_ServiceMasters SM on M.ServiceName = SM.ServiceName
                            where Total > 0";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.CommandType = CommandType.Text;
                SqlDataReader sr = comm.ExecuteReader();

                
                while (sr.Read())
                {
                    while (sr.Read())
                    {
                        VUI3ServiceMasterMatrixItem i = new VUI3ServiceMasterMatrixItem();

                        i.Id = (int)sr["Id"];
                        i.ServiceName = (string)sr["ServiceName"];

                        i.TabletAndroidCount = (int)sr["TabletAndroid"];
                        i.SmartphoneAndroidCount = (int)sr["SmartphoneAndroid"];
                        i.SmartphoneiPhoneCount = (int)sr["SmartphoneiPhone"];
                        i.TabletiPadCount = (int)sr["TabletiPad"];
                        i.SmartphoneWindowsCount = (int)sr["SmartphoneWindows"];
                        i.WebCount = (int)sr["Web"];
                        i.ConnectedTVSamsungCount = (int)sr["Connected TV-Samsung"];
                        i.ConnectedTVSonyCount = (int)sr["Connected TV-Sony"];
                        i.ConnectedTVLGCount = (int)sr["Connected TV-LG"];
                        i.ConnectedTVPanasonicCount = (int)sr["Connected TV-Panasonic"];
                        i.GamesConsolesXBoxCount = (int)sr["Games Consoles-XBox"];
                        i.GamesConsolesPlaystationCount = (int)sr["Games Consoles-Playstation"];

                        i.STBAmazonFireTVCount = (int)sr["STB-Amazon Fire TV"];
                        i.STBAppleTVCount = (int)sr["STB-Apple TV"];
                        i.STBFreesatCount = (int)sr["STB-Freesat"];
                        i.STBRokuCount = (int)sr["STB-Roku"];
                        i.STBNowTVCount = (int)sr["STB-Now TV"];
                        i.STBSkyCount = (int)sr["STB-Sky"];
                        i.STBVirginCount = (int)sr["STB-Virgin"];
                        i.STBYouViewCount = (int)sr["STB-YouView"];
                        i.STBATTUVerseCount = (int)sr["STB-AT&T U-Verse"];
                        i.STBCoxCount = (int)sr["STB-Cox"];
                        i.STBCharterCount = (int)sr["STB-Charter"];
                        i.STBDirectTVCount = (int)sr["STB-Direct TV"];
                        i.STBDishCount = (int)sr["STB-Dish"];
                        i.STBOptimumCount = (int)sr["STB-Optimum"];
                        i.STBSuddenLinkCount = (int)sr["STB-Sudden Link"];
                        i.STBVerizonFiOSCount = (int)sr["STB-Verizon FiOS"];
                        i.STBXfinityCount = (int)sr["STB-Xfinity"];
                        i.STBTiVoCount = (int)sr["STB-TiVo"];

                        i.TotalCount = (int)sr["Total"];
                        i.IconURL = (string)sr["IconURL"];

                        _MatrixItems.Add(i);
                    }
                }
                conn.Close();
            }
        }

        public VUI3ServiceBenchmarkMatrix() { }

    }

    /*
    public struct VUI3ServiceBenchmarkMatrixItem
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string PlatformDevice { get; set; }
        public int Score { get; set; }
        public string Url { get { return umbraco.library.NiceUrl(Id); } }
        public string IconURL { get; set; }
        public bool IsPreviewable
        {
            get
            {
                return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
            }
        }
        public string ToString()
        {
            return String.Format(@"Service[{0}], PlatformDevice [{1}], Score [{2}]", new object[] { ServiceName, PlatformDevice, Score });
        }
    }
    */
}