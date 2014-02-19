using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace VUI.VUI3.classes
{
    public class VUI3ServiceMasterRatingMatrix
    {
        public List<VUI3ServiceMasterRatingMatrixItem> Ratings;

        public VUI3ServiceMasterRatingMatrix()
        {
            Ratings = new List<VUI3ServiceMasterRatingMatrixItem>();

            string sql = @"    SELECT M.ServiceName, M.TabletAndroid  ,M.SmartphoneAndroid ,M.SmartphoneiPhone  ,M.TabletiPad  ,M.SmartphoneWindows ,SM.Id, ISNULL(SM.IconURL,'') as IconURL
                                FROM vui_ServiceRatingScoreMatrix M
                                inner join vui_ServiceMasters SM on M.ServiceName = SM.ServiceName 
                                where M.TabletAndroid + M.SmartphoneAndroid + M.SmartphoneiPhone + M.TabletiPad + M.SmartphoneWindows > 0
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
                        VUI3ServiceMasterRatingMatrixItem i = new VUI3ServiceMasterRatingMatrixItem();
                        i.Id = (int)sr["Id"];
                        i.ServiceName = (string)sr["ServiceName"];
                        i.TabletAndroidScore = (decimal)sr["TabletAndroid"];
                        i.SmartphoneAndroidScore = (decimal)sr["SmartphoneAndroid"];
                        i.SmartphoneiPhoneScore = (decimal)sr["SmartphoneiPhone"];
                        i.TabletiPadScore = (decimal)sr["TabletiPad"];
                        i.SmartphoneWindowsScore = (decimal)sr["SmartphoneWindows"];
                        i.IconURL = (string)sr["IconURL"];

                        Ratings.Add(i);
                    }
                }
                conn.Close();
            }
        }
    }

    public class VUI3ServiceMasterRatingMatrixItem
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public decimal TabletAndroidScore { get; set; }
        public decimal SmartphoneAndroidScore { get; set; }
        public decimal SmartphoneiPhoneScore { get; set; }
        public decimal TabletiPadScore { get; set; }
        public decimal SmartphoneWindowsScore { get; set; }
        public string Url { get { return umbraco.library.NiceUrl(Id); } }
        public string IconURL { get; set; }
        public bool IsPreviewable
        {
            get
            {
                return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
            }
        }

        public decimal GetScore(string context)
        {
            switch (context)
            {
                case "TabletAndroid":
                    {
                        return TabletAndroidScore;
                    }
                case "SmartphoneAndroid":
                    {
                        return SmartphoneAndroidScore;
                    }
                case "TabletiPad":
                    {
                        return TabletiPadScore;
                    }
                case "SmartphoneiPhone":
                    {
                        return SmartphoneiPhoneScore;
                    }
                case "SmartphoneWindows":
                    {
                        return SmartphoneWindowsScore;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
    }
}