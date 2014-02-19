using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;

namespace VUI.VUI3.classes
{
    public class VUI3StoreComment
    {
        public string Id { get; set; }
        public string Store { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Rating { get; set; }
        public string Version { get; set; }
        public DateTime DateRecorded { get; set; }
        public string DateRecordedString
        {
            get
            {
                return DateRecorded.ToString("dd MMM yyyy");
                
            }
        }
        public DateTime ReviewDate { get; set; }
        public string ReviewDateString
        {
            get
            {
                if (ReviewDate.Year == 1)
                {
                    return "Not known";
                }
                else
                {
                    return ReviewDate.ToString("dd MMM yyyy");
                }
            }
        }
        public decimal NormalisedRating
        {
            get
            {
                Decimal d = 0.0M; 
                if (Store.Equals("iTunes"))
                {
                    Decimal.TryParse(Rating, out d);
                }
                if (Store.Equals("Google Play"))
                {
                    if (Decimal.TryParse(Rating, out d))
                    {
                        d = (d / 100M) * 5.0M; // Move from Percentile to 5-stars.
                    }
                }
                return d;
            }
        }

    }


    public class VUI3StoreCommentsMetaList
    {
        public List<VUI3StoreCommentsMeta> CommentsMeta { get; set; }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3StoreCommentsMetaList>(this);
        }

        public VUI3StoreCommentsMetaList(int serviceMasterId)
        {
            List<VUI3StoreCommentsMeta> metalist = new List<VUI3StoreCommentsMeta>();
            List<VUI3StoreCommentsMeta> tmplist  = new List<VUI3StoreCommentsMeta>();

            string sql = @" select C.ServiceId, Store, 'Tablet/iPad' as DeviceType, count(C.ServiceId) as CommentCount
                            from   vui_StoreRatingComments C inner join [dbo].[vui_ServiceMasters] SM on C.ServiceId = SM.ID
                            where  C.ServiceId = @serviceid
                            and    SM.iPadAppURL is not null and SM.iPadAppURL != ''
                            and    Store = 'iTunes' and (DeviceType = 'Tablet/iPad' or DeviceType is null)
                            group  by Store, C.ServiceId
                            UNION
                            select C.ServiceId, Store, 'Smartphone/iPhone' as DeviceType, count(C.ServiceId) as CommentCount
                            from   vui_StoreRatingComments C inner join [dbo].[vui_ServiceMasters] SM on C.ServiceId = SM.ID
                            where  C.ServiceId = @serviceid
                            and    SM.iPadAppURL is not null and SM.iPhoneAppURL != ''
                            and    Store = 'iTunes' and (DeviceType = 'Smartphone/iPhone' or DeviceType is null)
                            group  by Store, C.ServiceId
                            UNION
                            select C.ServiceId, Store, 'Tablet/Android' as DeviceType, count(C.ServiceId) as CommentCount
                            from   vui_StoreRatingComments C inner join [dbo].[vui_ServiceMasters] SM on C.ServiceId = SM.ID
                            where  C.ServiceId = @serviceid
                            and    SM.iPadAppURL is not null and SM.iPadAppURL != ''
                            and    Store = 'Google Play' and (DeviceType = 'Tablet/Android' or DeviceType is null)
                            group  by Store, C.ServiceId
                            UNION
                            select C.ServiceId, Store, 'Smartphone/Android' as DeviceType, count(C.ServiceId) as CommentCount
                            from   vui_StoreRatingComments C inner join [dbo].[vui_ServiceMasters] SM on C.ServiceId = SM.ID
                            where  C.ServiceId = @serviceid
                            and    SM.iPadAppURL is not null and SM.iPhoneAppURL != ''
                            and    Store = 'Google Play' and (DeviceType = 'Smartphone/Android' or DeviceType is null)
                            group  by Store, C.ServiceId";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                SqlCommand comm = new SqlCommand(sql);
                comm.Parameters.Add(new SqlParameter("@serviceid", serviceMasterId));
                comm.Connection = conn;
                conn.Open();
                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    VUI3StoreCommentsMeta m = new VUI3StoreCommentsMeta();
                    m.Store = (string)sr["Store"];
                    if (sr["DeviceType"].GetType() != typeof(DBNull))
                    {
                        m.DeviceType = (string)sr["DeviceType"];
                    }
                    else
                    {
                        m.DeviceType = "Any";
                    }
                    m.Count = (int)sr["CommentCount"];
                    tmplist.Add(m);
                }
                conn.Close();
            }

            VUI3StoreCommentsMeta m1;
            // iPad
            if (tmplist.Count(m => m.DeviceType.Equals("Tablet/iPad")) > 0)
            {
                m1 = tmplist.First(m => m.DeviceType.Equals("Tablet/iPad"));
            }            
            else
            {
                m1 = new VUI3StoreCommentsMeta("iTunes","Tablet/iPad",0);
            }
            metalist.Add(m1);

            // Droid Tab
            if (tmplist.Count(m => m.DeviceType.Equals("Tablet/Android")) > 0)
            {
                m1 = tmplist.First(m => m.DeviceType.Equals("Tablet/Android"));
                
            }
            else
            {
                m1 = new VUI3StoreCommentsMeta("Google Play", "Tablet/Android", 0);
            }
            metalist.Add(m1);

            // iPhone
            if (tmplist.Count(m => m.DeviceType.Equals("Smartphone/iPhone")) > 0)
            {
                m1 = tmplist.First(m => m.DeviceType.Equals("Smartphone/iPhone"));
            }
            else
            {
                m1 = new VUI3StoreCommentsMeta("iTunes", "Smartphone/iPhone", 0);
            }
            metalist.Add(m1);

            // Droid Phone
            if (tmplist.Count(m => m.DeviceType.Equals("Smartphone/Android")) > 0)
            {
                m1 = tmplist.First(m => m.DeviceType.Equals("Smartphone/Android"));
            }
            else
            {
                m1 = new VUI3StoreCommentsMeta("Google Play", "Smartphone/Android", 0);
            }
            metalist.Add(m1);

            CommentsMeta = metalist;
        }
    }

    public class VUI3StoreCommentsMeta
    {
        public string Store { get; set; }
        public string DeviceType { get; set; }
        public int Count { get; set; }

        public VUI3StoreCommentsMeta() { ;}

        public VUI3StoreCommentsMeta(string store, string deviceType, int count)
        {
            Store = store;
            DeviceType = deviceType;
            Count = count;
        }
    }


    /// <summary>
    /// Class that gets a 
    /// </summary>
    public class VUI3StoreCommentList
    {
        public List<VUI3StoreComment> Comments { get; set; }

        public string AsJson()
        {
            return VUI3Utility.SerialiseJson<VUI3StoreCommentList>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceMasterId"></param>
        /// <param name="store">iTunes | Google Play</param>
        /// <param name="deviceType">Tablet/iPad | Tablet/Android | Smartphone/iPhone | Smartphone/Android</param>
        /// <param name="startpos">Selection picks > than startpos. Start at 0</param>
        /// <param name="numrows"></param>
        public VUI3StoreCommentList(int serviceMasterId, string store, string deviceType, int startpos, int numrows)
        {
            int endpos = startpos + numrows;

            List<VUI3StoreComment> comments = new List<VUI3StoreComment>();

            string sql = @" select COMMENTS.* FROM ( 
                                select  id, title, comment, rating, version, reviewdate, daterecorded, 
                                        ROW_NUMBER() OVER (order by ReviewDate Desc, DateRecorded Desc) as ROWNUM
                                from    vui_StoreRatingComments 
                                where   ServiceId = @serviceid
                                and     Store = @store
                                and     (DeviceType is null OR DeviceType = @deviceType) 
                            ) as COMMENTS
                            where COMMENTS.ROWNUM > @startpos AND COMMENTS.ROWNUM <= @endpos";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                SqlCommand comm = new SqlCommand(sql);
                comm.Parameters.Add(new SqlParameter("@serviceId", serviceMasterId));
                comm.Parameters.Add(new SqlParameter("@store", store));
                comm.Parameters.Add(new SqlParameter("@deviceType", deviceType));
                comm.Parameters.Add(new SqlParameter("@startpos", startpos));
                comm.Parameters.Add(new SqlParameter("@endpos", endpos));
                comm.Connection = conn;
                conn.Open();
                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    VUI3StoreComment c = new VUI3StoreComment();
                    c.Store = store;
                    if (sr["id"].GetType() != typeof(DBNull))
                    {
                        c.Id = (string)sr["id"];
                    }
                    if (sr["title"].GetType() != typeof(DBNull))
                    {
                        c.Title = (string)sr["title"];
                    }
                    if (sr["comment"].GetType() != typeof(DBNull))
                    {
                        c.Comment = (string)sr["comment"];
                    }
                    if (sr["rating"].GetType() != typeof(DBNull))
                    {
                        c.Rating = (string)sr["rating"];
                    }
                    if (sr["version"].GetType() != typeof(DBNull))
                    {
                        c.Version = (string)sr["version"];
                    }
                    if (sr["reviewdate"].GetType() != typeof(DBNull))
                    {
                        c.ReviewDate = (DateTime)sr["reviewdate"];
                    }
                    if (sr["daterecorded"].GetType() != typeof(DBNull))
                    {
                        c.DateRecorded = (DateTime)sr["daterecorded"];
                    }
                    comments.Add(c);
                }

                conn.Close();
            }
            Comments = comments;
        }
        
    }
}