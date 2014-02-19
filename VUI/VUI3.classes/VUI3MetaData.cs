using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Configuration;
using VUI.VUI2.classes;
using System.Data.SqlClient;

namespace VUI.VUI3.classes
{
    public class VUI3MetaData
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3MetaData));


        private static string VUI_mediafolder;
        private static bool EncryptImageURLs;
        

        /// <summary>
        /// Static Constructor
        /// </summary>
        static VUI3MetaData()
        {
            VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");
            EncryptImageURLs = (Utility.GetConst("VUI_EncryptImageURLs").Equals("YES"));
        }


        /// <summary>
        /// Publish the agglomerated screenshot metadata for a specific service
        /// </summary>
        /// <param name="serviceId"></param>
        public static void PublishScreenshotMetaData(int serviceId)
        {

        }

        public static void PublishScreenshotMetaData(string serviceName)
        {
        //    sbSQL.AppendLine(@"exec vui_RefreshSingleServiceScreenshotsMetaData @ServiceName ");
        //    VUIDataFunctions.ExecQuery(sbSQL.ToString());
        }


        /// <summary>
        /// Publish the metadata for a single image
        /// </summary>
        /// <param name="id"></param>
        public static void PublishImageMetadata(int id)
        {
            DynamicNode image = new DynamicNode(id);
            int analysisId = image.Parent.Id;
            int serviceId = image.Parent.Parent.Id;
            int isPreviewable = 0;

            DynamicNode service = new DynamicNode(serviceId);
            if (service.Name.Equals(Utility.GetConst("VUI_previewservice")))
            {
                isPreviewable = 1;
            }

            DateTime createDate = image.CreateDate;
            string pagetype = image.GetProperty("pageType").Value;
            string ImageURL_th = VUI_mediafolder + @"th/" + image.GetProperty("thFile").Value.Replace("&", "%26");
            string ImageURL_md = VUI_mediafolder + @"md/" + image.GetProperty("thFile").Value.Replace("&", "%26");
            string ImageURL_lg = VUI_mediafolder + @"lg/" + image.GetProperty("lgFile").Value.Replace("&", "%26");
            string ImageURL_full = VUI_mediafolder + @"full/" + image.GetProperty("imageFile").Value.Replace("&", "%26");

            // This is for the New Handling URLs which decrypt on opening.  Note that the Preview Service is not encrypted for simplicity.
            if (EncryptImageURLs && isPreviewable == 0)
            {
                ImageURL_th = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_th);
                ImageURL_md = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_md);
                ImageURL_lg = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_lg);
                ImageURL_full = VUI3.classes.URLEncodeUtility.EncryptStringAES(ImageURL_full);
            }

            string sql = @"
                IF ( EXISTS (select Id from vui_Image where Id=@Id))
                begin
                    update vui_Image set
                        ServiceId = @ServiceId
                    ,   AnalysisId = @AnalysisId
                    ,   PageType = @PageType
                    ,   ImageURL_th = @ImageURL_th
                    ,   ImageURL_md = @ImageURL_md
                    ,   ImageURL_lg = @ImageURL_lg
                    ,   ImageURL_full = @ImageURL_full
                    ,   DateCreated = @DateCreated
                    where Id = @Id
                    ;
                end
                ELSE
                begin
                    insert into vui_Image (Id, ServiceId, AnalysisId, PageType, ImageURL_th, ImageURL_md, ImageURL_lg, ImageURL_full, DateCreated)
                    values (@Id, @ServiceId, @AnalysisId, @PageType, @ImageURL_th, @ImageURL_md, @ImageURL_lg, @ImageURL_full, @DateCreated)
                    ;
                end
                ";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@Id", id));
                comm.Parameters.Add(new SqlParameter("@ServiceId", serviceId));
                comm.Parameters.Add(new SqlParameter("@AnalysisId", analysisId));
                comm.Parameters.Add(new SqlParameter("@PageType", pagetype));
                comm.Parameters.Add(new SqlParameter("@ImageURL_th", ImageURL_th));
                comm.Parameters.Add(new SqlParameter("@ImageURL_md", ImageURL_md));
                comm.Parameters.Add(new SqlParameter("@ImageURL_lg", ImageURL_lg));
                comm.Parameters.Add(new SqlParameter("@ImageURL_full", ImageURL_full));
                comm.Parameters.Add(new SqlParameter("@DateCreated", createDate));
                comm.ExecuteNonQuery();
                conn.Close();
            }
            
        }

        public static void PublishAnalysisMetadata(int id, bool recurse)
        {

        }

        public static void PublishServiceMetadata(int id, bool recurse)
        {

        }
    }
}