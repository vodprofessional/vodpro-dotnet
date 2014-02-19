using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using Newtonsoft.Json;
using System.Net;
using System.Data.SqlClient;
using System.Web.Security;
using VUI.classes;
using umbraco.cms.businesslogic.member;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Net.Mail;
using System.IO;
using VUI.VUI2.classes;
using System.Web.UI;
using VUI.VUI3.classes.Web;
using umbraco.BusinessLogic;

namespace VUI.VUI3.classes
{
    public static class VUI3Utility
    {

        /// <summary>
        /// Used for quick replacement of dynamic figures. e.g. Number of Services / Number of Screenshots
        /// </summary>
        /// <param name="inText"></param>
        /// <returns></returns>
        public static string ReplaceDynamicField(string inText)
        {
            return inText;
        }


        public static string SetPageTitle(string titleText)
        {
            return @"<script type=""text/javascript"">document.title = '" + titleText.Replace("'","\'") + @"';</script>";
        }

        public static string PageBody(string classname)
        {
            return String.Format(@"<body class=""{0}"">",classname);
        }


        public static string GetScreenshotURL(string imagePath)
        {
            string handlerUrl = @"/handlers/vui-image.ashx?";
            return handlerUrl + URLEncodeUtility.EncryptStringAES(imagePath);
        }

        public static int NumScreenshots()
        {
            int retval=0;
            string sql = @" select count(*) from vui_Image";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                sr.Read();
                retval = (int)sr[0];
                conn.Close();
            }
            return retval;
        }

        public static VUI3SimpleServiceMasterCollection AllServicesSimple()
        {
            string sql = @" select ID, ServiceName from vui_ServiceMasters order by ServiceName ASC";

            VUI3SimpleServiceMasterCollection sms = new VUI3SimpleServiceMasterCollection();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    int id = (int)sr["ID"];
                    string serviceName = (string)sr["ServiceName"];
                    string url = umbraco.library.NiceUrl(id);
                    sms.ServiceMasters.Add(new VUI3SimpleServiceMaster(id, serviceName, url));
                }

                conn.Close();
            }
            return sms;
        }

        /// <summary>
        /// Delete image and remove from MetaData
        /// </summary>
        /// <param name="imageid"></param>
        /// <returns></returns>
        public static bool DeleteImage(int imageid)
        {
            try
            {
                Document d = new Document(imageid);
                if (d.Published)
                {
                    d.UnPublish();
                    umbraco.library.UnPublishSingleNode(imageid);
                }
                d.delete(true);
            }
            catch (Exception e)
            {
                try
                {
                    umbraco.library.UnPublishSingleNode(imageid);
                }
                catch (Exception ex)
                {

                }
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.CommandText = "vui_DeleteImageMeta";
                comm.Parameters.Add(new SqlParameter("@ImageId", imageid));
                comm.ExecuteNonQuery();
                conn.Close();
            }
            return true;
        }

        public static int NumberOfVUIProducts()
        {
            int count = 0;
            
            string sql =@"select count(*) 
		        from vui_Service S 
		        left outer join vui_Platform P on P.ID=S.PlatformID 
		        where P.Name in ('Tablet','Web','Smartphone')
		        and BenchmarkScore > 0";


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    count = (Int32)sr[0];
                }
                conn.Close();
            }
            return count;
        }

        public static int NumberOfVUIBenchmarks()
        {
            int count = 0;

            string sql = @"select count(*) 
		        from vui_Analysis A 
		        left outer join vui_Platform P on P.ID=A.PlatformID 
		        where P.Name in ('Tablet','Web','Smartphone')
		        and BenchmarkScore > 0";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    count = (Int32)sr[0];
                }
                conn.Close();
            }
            return count;
        }

        /* Service Page is built from ServiceMaster Document Type */

        /// <summary>
        /// Utility to pull back the service master for a name e.g. "4oD"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Document FindServiceMasterDocumentByName(string name)
        {
            Document node = new Document(Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString()));
            List<Document> nodes = node.Children.ToList<Document>().Where(n => n.Text.Equals(name)).ToList();
            if (nodes.Count < 1)
            {
                throw new Exception("No ServiceMaster with that name");
            }
            else
            {
                return nodes[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GetBenchmarkMatrix() 
        {
            ;
        }


        /// <summary>
        /// Return a list of News Items to appear on the LoggedIn Page
        /// </summary>
        public static void RecentNews() { ; }

        /// <summary>
        /// Return Name, Image, Job Title and Linked In URL for user
        /// </summary>
        public static VUIUser GetUser() 
        {
            return new VUIUser();
        }

        /// <summary>
        /// Upload an image file
        /// </summary>
        public static void UploadImage() { ; }


        /// <summary>
        /// List of all favourite screenshots
        /// </summary>
        public static ImageFavourites FavouriteScreenshotList(int userId) 
        {

            ImageFavourites favList = new ImageFavourites();

            string sql = @" select 
                                CollectionName, ServiceName, DateAdded, ImageId, ImageURL_full, ImageURL_lg, ImageURL_md, ImageURL_th, PageType, DateCreated , DeviceName, DateTag
                            from vui_vUserFavouriteImages 
                            where UserId = @userId order by CollectionName, DateAdded Desc";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@userId", userId));

                SqlDataReader sr = comm.ExecuteReader();

                string collectionName;

                while (sr.Read())
                {
                    ImageFavouriteCollection coll;

                    collectionName = (string)sr["CollectionName"];
                    if (favList.collections.Count<ImageFavouriteCollection>(c => c.collectionName.Equals(collectionName)) == 0)
                    {
                        coll = new ImageFavouriteCollection(collectionName);
                        favList.collections.Add(coll);
                    }
                    else
                    {
                        coll = favList.collections.First<ImageFavouriteCollection>(c => c.collectionName.Equals(collectionName));
                    }

                    VUI3Screenshot s = new VUI3Screenshot();
                    s.Id = (Int32)sr["ImageId"];
                    s.ServiceName = (string)sr["ServiceName"];
                    s.IsFavourite = true;
                    s.Device = (string)sr["DeviceName"];
                    s.ImportTag = (string)sr["DateTag"];
                    s.ImageURL_full = (string)sr["ImageURL_full"];
                    s.ImageURL_lg = (string)sr["ImageURL_lg"];
                    s.ImageURL_md = (string)sr["ImageURL_md"];
                    s.ImageURL_th = (string)sr["ImageURL_th"];
                    s.PageType = (string)sr["PageType"];
                    s.DateAdded = (DateTime)sr["DateAdded"];
                    s.DateCreated = (DateTime)sr["DateCreated"];

                    coll.imageFavourites.Add(s);
                }

                conn.Close();
            }
            return favList;
        }

        /// <summary>
        /// Returns the screenshot associated to the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static VUI3Screenshot GetScreenshot(int id)
        {
            VUI3Screenshot s = null;
            string sql = @"select 
                                top 1 I.Id, S.ServiceName, I.ImageURL_full, I.ImageURL_lg, I.ImageURL_md, I.ImageURL_th, I.PageType, I.DateCreated 
                            from vui_Image I inner join vui_Service S on I.ServiceId = S.ID where I.Id = @id";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add(new SqlParameter("@id", id));

                SqlDataReader sr = comm.ExecuteReader();

                while (sr.Read())
                {
                    s = new VUI3Screenshot();
                    s.Id = (Int32)sr["Id"];
                    s.ImageURL_full = (string)sr["ImageURL_full"];
                    s.ImageURL_lg = (string)sr["ImageURL_lg"];
                    s.ImageURL_md = (string)sr["ImageURL_md"];
                    s.ImageURL_th = (string)sr["ImageURL_th"];
                    s.PageType = (string)sr["PageType"];
                    s.DateCreated = (DateTime)sr["DateCreated"];
                }
                conn.Close();
            }
            return s;
        }

        /// <summary>
        /// SAving all favourites
        /// </summary>
        /// <param name="favs"></param>
        /// <param name="userId"></param>
        public static void SaveFavouriteScreenshots(ImageFavourites favs, int userId)
        {
            // Loop around the collections
            string sql = @"if(not exists( select top 1 * from vui_UserFavouriteImages where UserId=@userId and CollectionName=@collectionName and ImageId=@imageId)) 
                                insert into vui_UserFavouriteImages(UserId, CollectionName, ImageId, DateAdded)
                                values (@userId, @collectionName, @imageId, @dateAdded);
                            ";
            foreach (ImageFavouriteCollection col in favs.collections)
            {
                List<string> idlist = new List<string>();
                foreach(VUI3Screenshot s in col.imageFavourites)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(sql, conn);
                        comm.Parameters.Add(new SqlParameter("@userId", userId));
                        comm.Parameters.Add(new SqlParameter("@collectionName", col.collectionName));
                        comm.Parameters.Add(new SqlParameter("@imageId", s.Id));
                        comm.Parameters.Add(new SqlParameter("@dateAdded", s.DateAdded));
                        comm.ExecuteNonQuery();
                        conn.Close();

                        idlist.Add(s.Id.ToString());
                    }
                }
                string deletesql = @"delete from vui_UserFavouriteImages where UserId=@userId and CollectionName=@collectionName";
                if (idlist.Count > 0)
                {
                    deletesql = deletesql+ @" and ImageId not in ({0})";
                    deletesql = String.Format(deletesql, String.Join(",", idlist.ToArray()));
                }
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(deletesql, conn);
                    comm.Parameters.Add(new SqlParameter("@userId", userId));
                    comm.Parameters.Add(new SqlParameter("@collectionName", col.collectionName));
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Get a list of Page Types for Search purposes
        /// </summary>
        public static List<VUI3Preval> PageTypeCategoryList() 
        { 
            List<VUI3Preval> pagetypes = GetPrevals(Int32.Parse(VUI2.classes.Utility.GetConst("VUI_function_list")));
            return pagetypes.OrderBy(p => p.Val).ToList();
        }

        /// <summary>
        /// The screenshot matrix for screenshot homepage
        /// </summary>
        public static VUI3ServiceMasterMatrix ScreenshotMatrix() { return new VUI3ServiceMasterMatrix(); }


        
        public static void AddSavedScreenshotSearch() { ; }
        public static void DeleteSavedScreenshotSearch() { ; }
        public static void SavedScreenshotSearches() { ; }


        static User u = new User("websitecontentuser");

        /// <summary>
        /// Admin function used by the categorisation lightbox
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool CategoriseScreenshot(int id, string category)
        {
            if (id > 0)
            {
                Document scrn = new Document(id);
                scrn.getProperty("pageType").Value = category;
                scrn.Save();
                if (scrn.Published)
                {
                    scrn.Publish(u);
                    umbraco.library.UpdateDocumentCache(id);
                }
                VUI3MetaData.PublishImageMetadata(id);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Search screenshots based on service / page type and return results
        /// </summary>
        public static VUI3ScreenshotResults SearchScreenshots(string service, string device, string pagetype, int startfrom, int maxcount) 
        {
            VUI3ScreenshotResults results = new VUI3ScreenshotResults();
            VUI3ScreenshotList screenlist = new VUI3ScreenshotList();
            List<VUI3Screenshot> screenshots = new List<VUI3Screenshot>();

            string sql = @" 
                            select count(*) as ResultCount 
                            from [dbo].[vui_Image] I 
                            inner join vui_Service S on I.ServiceId = S.ID 
                            inner join vui_Platform P on S.PlatformID = P.ID 
                            left outer join vui_Device D on S.DeviceID = D.ID 
                            where 1=1 
                            {0} {1} {2};

                            select S.Name as ServiceName, 
                            P.Name + CASE WHEN(D.Name IS NULL) THEN '' ELSE '/' + D.Name END as DeviceName, 
                            I.* , F.CollectionName, F.DateAdded, A.DateTag
                            from [dbo].[vui_Image] I
							inner join vui_Service S on I.ServiceId = S.ID
							inner join vui_Platform P on S.PlatformID = P.ID
                            inner join vui_Analysis A on I.AnalysisId = A.Id
							left outer join vui_Device D on S.DeviceID = D.ID
                            left outer join (select * from [vui_UserFavouriteImages] where UserId = -1) F on I.Id = F.ImageId
                            where 1=1 
                            {0} {1} {2} order by S.Name, I.PageType, I.DateCreated Desc
                            ";

            string servicecrit = "";
            string devicecrit = "";
            string functioncrit = "";
            // Service
            if (!String.IsNullOrEmpty(service))
            {
                servicecrit = @" and S.Name = @service ";
            }
            // Device
            if (!String.IsNullOrEmpty(device))
            {
                devicecrit = @" and P.Name + CASE WHEN(D.Name IS NULL) THEN '' ELSE '/' + D.Name END = @device ";
            }
            // Page Type
            if (!String.IsNullOrEmpty(pagetype))
            {
                // This is for re-use in the admin tool, will never be called from the user front-end
                if (pagetype.Equals("NONE"))
                {
                    functioncrit = @" and PageType = '' ";
                }
                else
                {
                    functioncrit = @" and PageType = @pagetype ";
                }
            }
            sql = String.Format(sql, new object[] { servicecrit, devicecrit, functioncrit });

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
                comm.Parameters.Add(new SqlParameter("@service", service));
                comm.Parameters.Add(new SqlParameter("@device", device));
                comm.Parameters.Add(new SqlParameter("@pagetype", pagetype));
                SqlDataReader sr = comm.ExecuteReader();

                sr.Read();
                results.totalResults = (int)sr["ResultCount"];
                results.resultStart = startfrom;

                sr.NextResult();

                int counter = 0;
                while (sr.Read() && counter < startfrom+maxcount)
                {
                    counter++;
                    if (counter < startfrom) { continue; }

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
            results.ScreenshotsList = screenlist;
            return results;
        }


        /*  Refer to VUIDataFunctions for Benchmarking Info */

        public static string GetMemberNameFromCookies()
        {
            string userName = String.Empty;
            bool rememberUserId = false;
            if (HttpContext.Current.Request.Cookies["vrl"] != null)
            {
                if (HttpContext.Current.Request.Cookies["vrl"].Value.Equals("Y"))
                {
                    rememberUserId = true;
                }
            }
            if (rememberUserId)
            {
                string id;
                int uid = 0;
                if (HttpContext.Current.Request.Cookies["vid"] != null)
                {
                    if(!String.IsNullOrEmpty(HttpContext.Current.Request.Cookies["vid"].Value))
                    {
                        id = HttpContext.Current.Request.Cookies["vid"].Value;
                        if(Int32.TryParse(id, out uid))
                        {
                            try
                            {
                                Member m = new Member(uid);
                                userName = m.LoginName;
                            }
                            catch (Exception ex)
                            {
                                log.Error("Error finding save username", ex);
                            }
                        }
                    }
                }
            }
            return userName;
        }

        public static bool MemberLogin(string username, string password, bool rememberMyUsername)
        {
            if (rememberMyUsername)
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("vrl", "Y"));
            }
            else
            {
                if (HttpContext.Current.Request.Cookies["vrl"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("vrl");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
            return MemberLogin(username, password);
        }

        public static bool MemberLogin(string username, string password)
        {
            MembershipUser currentUser = Membership.GetUser();
            if (currentUser == null)
            {
                log.Debug("Logging in: NO CURRENT USER");


                if (Membership.ValidateUser(username, password))
                {
                    log.Debug("Logging in: Valid username or password - " + username);

                    currentUser = Membership.GetUser(username);
                    string[] roles = Roles.GetRolesForUser(username);
                    HttpContext.Current.Session["userroles"] = roles;
                    FormsAuthentication.SetAuthCookie(username, true);
                    Member m = new Member((int)currentUser.ProviderUserKey);

                    DateTime loginDate = DateTime.Now;

                    // Current Login Date will be moved to Last Login Date
                    if (m.getProperty("vuiCurrentLoginDate").Value != null)
                    {
                        m.getProperty("vuiLastLogin").Value = m.getProperty("vuiCurrentLoginDate").Value;
                    }
                    m.getProperty("vuiCurrentLoginDate").Value = loginDate;
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("uid", VUIfunctions.MemberVUIStatus(m)));
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("vid", m.Id.ToString()));
                    return true;
                }
                else if (Membership.FindUsersByName(username).Count > 0)
                {
                    log.Debug("Logging in: Multiple Users with username - " + username);

                    return false;
                    throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
                }
                else
                {
                    log.Debug("Logging in: Invalid username or password - " + username);

                    return false;
                    throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
                }
            }
            else
            {
                log.Debug("Logging in: CURRENT USER? " + currentUser.Email);
            }

            return false;
            throw new Exception(VUIfunctions.VUI_USER_OR_PWD);
        }




        /// <summary>
        /// Sign the current member out
        /// </summary>
        public static void MemberLogout()
        {
            Member m = VUIfunctions.CurrentUser();
            if (m != null)
            {
                Member.RemoveMemberFromCache(m.Id);
                Member.ClearMemberFromClient(m.Id);
            }
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies["uid"] != null)
            {
                HttpCookie myCookie = new HttpCookie("uid");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            bool rememberUserId = false;
            if (HttpContext.Current.Request.Cookies["vrl"] != null)
            {
                if (HttpContext.Current.Request.Cookies["vrl"].Value.Equals("Y"))
                {
                    rememberUserId = true;
                }
            }
            if (!rememberUserId)
            {
                if (HttpContext.Current.Request.Cookies["vid"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("vid");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
        }


        /// <summary>
        /// Highest Benchmarking scores by platform
        /// </summary>
        /// <param name="platformid"></param>
        /// <returns></returns>
        public static List<VUI3ServiceMasterMatrixItem> GetBestServicesForPlatform(int platformid)
        {
            List<VUI3ServiceMasterMatrixItem> services = new List<VUI3ServiceMasterMatrixItem>();

            string sql = String.Format(@"select SM.Id, SM.ServiceName, S.BenchmarkScore, S.IsPreviewable, SM.IconURL
		                                from vui_Service S
		                                inner join vui_ServiceMasters SM
		                                on S.Name = SM.ServiceName		
		                                where PlatformId = {0} and S.BenchmarkScore > 0
		                                order by BenchmarkScore desc;", platformid);

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);

                SqlDataReader sr = comm.ExecuteReader();

                // Work through the results set
                // For each result, get the default image from the 
                while (sr.Read())
                {
                    VUI3ServiceMasterMatrixItem s = new VUI3ServiceMasterMatrixItem();
                    s.Id = (int)sr["Id"];
                    s.ServiceName = (string)sr["ServiceName"];
                    s.BestBenchmarkScore = (int)sr["BenchmarkScore"];
                    s.IconURL = (string)sr["IconURL"];
                    services.Add(s);
                }
                conn.Close();
            }
            return services;
        }


        /// <summary>
        /// Highest Benchmarking scores by device (because done on ID, Platform implied)
        /// </summary>
        /// <param name="platformid"></param>
        /// <returns></returns>
        public static List<VUI3ServiceMasterMatrixItem> GetBestServicesForDevice(int deviceid)
        {
            List<VUI3ServiceMasterMatrixItem> services = new List<VUI3ServiceMasterMatrixItem>();

            string sql = String.Format(@"select SM.Id, SM.ServiceName, S.BenchmarkScore, S.IsPreviewable, SM.IconURL
		                                from vui_Service S
		                                inner join vui_ServiceMasters SM
		                                on S.Name = SM.ServiceName		
		                                where DeviceId = {0} and S.BenchmarkScore > 0
		                                order by BenchmarkScore desc;", deviceid);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);

                SqlDataReader sr = comm.ExecuteReader();

                // Work through the results set
                // For each result, get the default image from the 
                while (sr.Read())
                {
                    VUI3ServiceMasterMatrixItem s = new VUI3ServiceMasterMatrixItem();
                    s.Id = (int)sr["Id"];
                    s.ServiceName = (string)sr["ServiceName"];
                    s.BestBenchmarkScore = (int)sr["BenchmarkScore"];
                    s.IconURL = (string)sr["IconURL"];
                    services.Add(s);
                }
                conn.Close();
            }
            return services;
        }



        public static bool IsVUIUser(VUIUser u)
        {
            string status = VUIfunctions.MemberVUIStatus(u.Member);
            if (status.Equals(VUIfunctions.VUI_USERTYPE_ADMIN) || status.Equals(VUIfunctions.VUI_USERTYPE_USER))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static T DeserialiseJson<T>(string json_data) where T : new()
        {
            // if string with JSON data is not empty, deserialize it to class and return its instance 
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            
        }

        public static T DownloadDeserialiseJson<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return DeserialiseJson<T>(json_data);
            }
        }

        public static string SerialiseJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static bool PasswordOk(string pwd)
        {
            return Regex.IsMatch(pwd, @"^.{6,}(?<=\d.*)(?<=[^a-zA-Z0-9].*)$");
        }


        public static bool SendContactEmail(string contactname, string company, string emailaddress, string phone)
        {
            string emailBody = GetEmailTemplate(HttpContext.Current.Server.MapPath(Utility.GetConst("VUI3_ContactEmailTemplate")));
            emailBody = String.Format(emailBody, new object[] { contactname, company, emailaddress, phone });
            log.Debug("Emailing: " + emailBody);

            SmtpClient smtp = new SmtpClient();
            MailMessage msg = new MailMessage();
            msg.To.Add(@"vui@vodprofessional.com");
            msg.Subject = "VUI Library Contact Request";
            msg.From = new MailAddress("admin@vodprofessional.com", "VOD Professional Admin");
            msg.Body = emailBody;
            smtp.Send(msg);
            return true;
        }


        private static string GetEmailTemplate(string templateFullPath)
        {
            string template;

            if (!File.Exists(templateFullPath))
                throw new ArgumentException("Template file does not exist: " + templateFullPath);

            using (StreamReader reader = new StreamReader(templateFullPath))
            {
                template = reader.ReadToEnd();
                reader.Close();
            }

            return template;
        }

        /// <summary>
        /// Return a list of Prevals for a given datatype ID
        /// </summary>
        /// <param name="dataTypeid"></param>
        /// <returns></returns>
        public static List<VUI3Preval> GetPrevals(int dataTypeid)
        {
            XPathNodeIterator iterator = umbraco.library.GetPreValues(dataTypeid);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

            List<VUI3Preval> prevals = new List<VUI3Preval>();
            int sort = 0;

            while (preValues.MoveNext())
            {
                int id;
                Int32.TryParse(preValues.Current.GetAttribute("id", ""), out id);
                prevals.Add(new VUI3Preval(sort++, id , preValues.Current.Value));
            }
            return prevals;
        }

        /// <summary>
        /// Get the string value for a Preval from a list of Prevals.
        /// </summary>
        /// <param name="prevals"></param>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool TryGetPrevalValue(List<VUI3Preval> prevals, int id, out string val)
        {
            if (prevals.Where(p => p.Id == id).Count() > 0)
            {
                val = prevals.First(p => p.Id == id).Val;
                return true;
            }
            else
            {
                val = String.Empty;
                return false;
            }
        }

        public static bool ServiceIsPreviewable(string ServiceName)
        {
            return (ServiceName.Equals(ConfigurationManager.AppSettings["VUI3PreviewService"].ToString()));
        }



        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUI3Utility));

    }

    public class VUI3Preval
    {
        public int Order { get; set; }
        public int Id { get; set; }
        public string Val { get; set; }
        public VUI3Preval(int order, int id, string val) { Order = order; Id = id;  Val = val; }
    }
}

/*
 * TODO - Last Login on Member
 * TODO - Profile image and linked in on Member
 * TODO - News Generator
 * TODO - Service Favourites db/doc structure
 * TODO - VUI3 Umbraco Template structure (Master / ServiceMaster etc)
 * 
 * 
 * 
*/