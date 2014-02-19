using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI3.classes
{
    public class VUI3ServiceMasterMatrix
    {

        public List<VUI3ServiceMasterMatrixItem> Services;

        public VUI3ServiceMasterMatrix()
        {
            Services = new List<VUI3ServiceMasterMatrixItem>();

            string sql = @"    SELECT M.ServiceName, M.TabletAndroid  ,M.SmartphoneAndroid ,M.SmartphoneiPhone  ,M.TabletiPad  ,M.SmartphoneWindows   , M.Web, 
                                [Connected TV-Samsung], [Connected TV-Sony],[Connected TV-LG], [Connected TV-Panasonic],[Games Consoles-XBox],[Games Consoles-Playstation],[STB-Roku], [STB-Now TV]
                                ,M.Total ,SM.Id, ISNULL(SM.IconURL,'') as IconURL
                                FROM vui_ScreenshotMatrix M
                                inner join vui_ServiceMasters SM on M.ServiceName = SM.ServiceName 
                                where Total > 0
                                order by M.ServiceName ASC";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();

                if (sr.HasRows)
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
                        i.STBRokuCount = (int)sr["STB-Roku"];
                        i.STBNowTVCount = (int)sr["STB-Now TV"];

                        i.TotalCount = (int)sr["Total"];
                        i.IconURL = (string)sr["IconURL"];


                        Services.Add(i);
                    }
                }
            }
        }

        private Dictionary<string, string> _letterLinks = null;

        public Dictionary<string, string> LetterLinks
        {
            get
            {
                if (_letterLinks == null)
                {
                    _letterLinks = SetFirstLetters();
                }
                return _letterLinks;
            }
        }

        private Dictionary<string, string> SetFirstLetters()
        {
            string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Z" };
            Dictionary<string, string> letterLinks = new Dictionary<string,string>();

            string prevLetter = "0";
            foreach (string letter in letters)
            {
                if (Services.Where(s => s.ServiceName.ToUpper().StartsWith(letter)).Count() > 0)
                {
                    letterLinks.Add(letter, letter);
                }
                else
                {
                    letterLinks.Add(letter, prevLetter);
                }
                prevLetter = letter;
            }
            return letterLinks;
        }
    }


    public class VUI3ServiceMasterMatrixItem
    {

        public int Id { get; set; }
        public string ServiceName { get; set; }
        public int TabletAndroidCount { get; set; }
        public int SmartphoneAndroidCount { get; set; }
        public int SmartphoneiPhoneCount { get; set; }
        public int TabletiPadCount { get; set; }
        public int SmartphoneWindowsCount {get; set; }
        public int WebCount { get; set; }
        public int ConnectedTVSamsungCount { get; set; }
        public int ConnectedTVSonyCount { get; set; }
        public int ConnectedTVLGCount { get; set; }
        public int ConnectedTVPanasonicCount { get; set; }
        public int GamesConsolesXBoxCount { get; set; }
        public int GamesConsolesPlaystationCount{ get; set; }
        public int STBRokuCount { get; set; }
        public int STBNowTVCount{ get; set; }
        public int TotalCount { get; set; }
        public string IconURL { get; set; }
        public string URL { 
            get { return umbraco.library.NiceUrl(Id); } }
        public int BestBenchmarkScore { get; set; }
        public bool IsPreviewable
        {
            get
            {
                return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
            }
        }

        public int GetCount(string context)
        {
            switch(context)
            {
                case "TabletAndroid":
                {
                    return TabletAndroidCount;
                }
                case "SmartphoneAndroid":
                {
                    return SmartphoneAndroidCount;
                }
                case "TabletiPad":
                {
                    return TabletiPadCount;
                }
                case "SmartphoneiPhone":
                {
                    return SmartphoneiPhoneCount;
                }
                case "Web":
                {
                    return WebCount;
                }
                case "SmartphoneWindows":
                {
                    return SmartphoneWindowsCount;
                }
                case "Connected TV-Samsung":
                {
                    return ConnectedTVSamsungCount;
                }
                case "Connected TV-Sony":
                {
                    return ConnectedTVSonyCount;
                }
                case "Connected TV-LG":
                {
                    return ConnectedTVLGCount;
                }
                case "Connected TV-Panasonic":
                {
                    return ConnectedTVPanasonicCount;
                }
                case "Games Consoles-XBox":
                { 
                    return GamesConsolesXBoxCount;
                }
                case "Games Consoles-Playstation":
                { 
                    return GamesConsolesPlaystationCount;
                }
                case "STB-Roku":
                { 
                    return STBRokuCount;
                }
                case "STB-Now TV":
                { 
                    return STBNowTVCount;
                }
                default:
                {
                    return 0;
                }
            }
        }

    }

}