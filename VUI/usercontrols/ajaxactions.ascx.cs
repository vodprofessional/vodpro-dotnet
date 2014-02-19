using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms;
using umbraco.presentation;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;
using System.Web.Security;
using VUI.classes;
using VUI.VUI2.classes;
using VUI.VUI3.classes;

namespace VUI.usercontrols
{
    public partial class ajaxactions : System.Web.UI.UserControl
    {
        private VUIUser u;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = Request.QueryString["a"];

            if (_action.Equals(ACTION_CATEGORISE_IMAGE))
            {
                CategoriseImage();
            }
            if (_action.Equals(ACTION_GET_UNCATEGORISED_IMAGES))
            {
                GetUncategorisedImages();
            }


            if (_action.Equals(ACTION_SEARCH_IMAGES))
            {
                SearchImages();
            }
            if (_action.Equals(ACTION_LIGHTBOX_IMAGE_SEARCH))
            {
                LightboxImageSearch();
            }
            if (_action.Equals(ACTION_GET_LATEST_NEWS))
            {
                GetLatestNews();
            }
            if (_action.Equals(ACTION_SAVE_DETAIL))
            {
                SaveUserDetail();
            }
            if (_action.Equals(ACTION_SAVE_IMAGE))
            {
                SaveUserImage();
            }

            if (_action.Equals(ACTION_LOGIN))
            {
                Login();
            }
            if (_action.Equals(ACTION_LOGOUT))
            {
                Logout();
            }
            if (_action.Equals(ACTION_RECOVER_PASSWORD))
            {
                RecoverPassword();
            }
            if (_action.Equals(ACTION_CHANGE_PASWORD))
            {
                ChangePassword();
            }
            if (_action.Equals(ACTION_GET_IMAGES_FOR_SERVICE))
            {
                GetImagesForService();
            }

            if (_action.Equals(ACTION_GET_IMAGES_FOR_ANALYSIS))
            {
                GetImagesForPackage();
            }
            if (_action.Equals(ACTION_RATE_SERVICE))
            {
                RateService();
            }
            if (_action.Equals(ACTION_FAVOURITE_IMAGE))
            {
                AddImageFavourite();
            }
            if (_action.Equals(ACTION_UNFAVOURITE_IMAGE))
            {
                DeleteImageFavourite();
            }

            if (_action.Equals(ACTION_FAVOURITE_SERVICE))
            {
                AddServiceFavourite();
            }
            if (_action.Equals(ACTION_UNFAVOURITE_SERVICE))
            {
                DeleteServiceFavourite();
            }

            if (_action.Equals(ACTION_SERVICE_IMAGE_PACKAGES))
            {
                GetImagePackages();
            }

            if (_action.Equals(ACTION_GET_FAVOURITE_SCREENSHOTS)) 
            {
                GetFavouriteImages();
            }

            if (_action.Equals(ACTION_CONTACT_REQUEST))
            {
                SaveContactRequest();
            }

            if (_action.Equals(ACTION_GET_ALL_SERVICE_LIST))
            {
                GetAllServiceList();
            }
            if (_action.Equals(ACTION_GET_SERVICE_LIST_BY_NAME))
            {
                GetServiceListByName();
            }
            if (_action.Equals(ACTION_GET_SERVICE_LIST_BY_NAME_AND_PLATFORM))
            {
                GetServiceListByServiceNameAndPlatform();
            }
            if (_action.Equals(ACTION_GET_SERVICE_LIST_BY_PLATFORM))
            {
                GetServiceListByPlatform();
            }

            if (_action.Equals(ACTION_GET_VUI_USERS))
            {
                GetVUIUsers();
            }
            if (_action.Equals(ACTION_DELETE_VUI_USER))
            {
                DeleteVUIUser();
            } 
            if (_action.Equals(ACTION_ADD_VUI_USER))
            {
                AddVUIUser();
            }

            if (_action.Equals(ACTION_GET_STORE_COMMENTS))
            {
                GetStoreComments();
            }
            if (_action.Equals(ACTION_GET_STORE_COMMENTS_META))
            {
                GetStoreCommentsMeta();
            }
            if (_action.Equals(ACTION_GET_APP_VERSIONS))
            {
                GetAppVersions();
            }
            if (_action.Equals(ACTION_DELETE_IMAGE))
            {
                DeleteImage();
            }
        }


        private void DeleteImage()
        {
            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                int imageid = 0;
                string imgid = Request["imageid"];
                if (Int32.TryParse(imgid, out imageid))
                {
                    VUI3Utility.DeleteImage(imageid);
                    Response.Write(@"{ ""response"":""valid"",""data"":""success"" }");
                }
            }
        }

        private void GetAppVersions()
        {
            int servicemasterid = 0;
            string deviceType = "";

            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                deviceType = Request["deviceType"];
                string tmp;

                tmp = Request["servicemasterid"];
                Int32.TryParse(tmp, out servicemasterid);


                VUI3AppVersionList versions = VUI3AppVersionList.GetVersionHistory(servicemasterid, deviceType);
                Response.Write(@"{ ""response"":""valid"",""data"":" + versions.AsJson() + " }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not logged in"" }");
            }
        }

        private void GetStoreCommentsMeta()
        {
            int servicemasterid = 0;

            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                string tmp = Request["servicemasterid"];
                Int32.TryParse(tmp, out servicemasterid);
                VUI3StoreCommentsMetaList comments = new VUI3StoreCommentsMetaList(servicemasterid);
                Response.Write(@"{ ""response"":""valid"",""data"":" + comments.AsJson() + " }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not logged in"" }");
            }
        }
        
        /// <summary>
        /// ADMIN ONLY Function
        /// </summary>
        private void GetStoreComments()
        {
            int servicemasterid = 0;
            string store = "";
            string deviceType = "";
            int startpos = 0;
            int numrows = 20;

            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                deviceType = Request["deviceType"];
                store = Request["store"];

                string tmp;

                tmp = Request["servicemasterid"];
                Int32.TryParse(tmp, out servicemasterid);

                tmp = Request["startpos"];
                Int32.TryParse(tmp, out startpos);

                tmp = Request["numrows"];
                Int32.TryParse(tmp, out numrows);

                VUI3StoreCommentList comments = new VUI3StoreCommentList(servicemasterid, store, deviceType, startpos, numrows);
                Response.Write(@"{ ""response"":""valid"",""data"":" + comments.AsJson() + " }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not logged in"" }");
            }
        }


        /// <summary>
        /// ADMIN ONLY Function
        /// </summary>
        private void GetUncategorisedImages()
        {
            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                // WRite a correctly formatted JSON string to the response stream
                string service = String.Empty;
                string device = String.Empty;
                string pageType = String.Empty;
                int startnum = 0;
                int maxcount = 100;

                if (!String.IsNullOrEmpty(Request["startfrom"]))
                {
                    Int32.TryParse(Request["startfrom"], out startnum);
                }

                if (!String.IsNullOrEmpty(Request["service"]))
                {
                    service = Request["service"];
                }
                if (!String.IsNullOrEmpty(Request["device"]))
                {
                    device = Request["device"];
                }
                if (!String.IsNullOrEmpty(Request["pagetype"]))
                {
                    pageType = "NONE"; // Request["pagetype"];
                }
                Response.Write(@"{ ""response"":""valid"",""data"":" + VUI3Utility.SearchScreenshots(service, device, pageType, startnum, maxcount).AsJson() + " }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not logged in"" }");
            }
        }

        private void CategoriseImage()
        {
            if (umbraco.BasePages.UmbracoEnsuredPage.CurrentUser != null)
            {
                // WRite a correctly formatted JSON string to the response stream
                int imageid = 0;
                string category = String.Empty;

                if (!String.IsNullOrEmpty(Request["imageid"]))
                {
                    Int32.TryParse(Request["imageid"], out imageid);
                }
                if (!String.IsNullOrEmpty(Request["category"]))
                {
                    category = Request["category"];
                }

                if (VUI3Utility.CategoriseScreenshot(imageid, category))
                {
                    Response.Write(@"{ ""response"":""valid"",""data"":""Categorised"" } ");
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Problem"" } ");
                }
            }
        }


        private bool IsVUIUser()
        {
            u = VUI3Utility.GetUser();
            return VUI3Utility.IsVUIUser(u);
        }

        private void GetVUIUsers()
        {
            if (IsVUIUser())
            {
                Response.Write(u.GetVUIUsersAsJSON());
            }
        }
        private void DeleteVUIUser()
        {
            if (IsVUIUser())
            {

                int userId = 0;

                if (!String.IsNullOrEmpty(Request["id"].ToString()))
                {
                    try
                    {
                        Membership.DeleteUser(Request["id"].ToString());
                        Response.Write(@"{ ""response"":""valid"",""data"":""Deleted"" }");
                    }
                    catch (Exception ex)
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Error"" }");
                    }
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""No data"" }");
                }
            }
        }
        private void AddVUIUser()
        {
            if (IsVUIUser() && u.UserStatus.Equals(VUIfunctions.VUI_USERTYPE_ADMIN))
            {
                if (!String.IsNullOrEmpty(Request["first"].ToString()) && !String.IsNullOrEmpty(Request["last"].ToString()) && !String.IsNullOrEmpty(Request["email"].ToString()))
                {
                    try
                    {
                        string firstname = Request["first"].ToString();
                        string lastname = Request["last"].ToString();
                        string email = Request["email"].ToString();

                        bool isValid = true;

                        if (!VUIfunctions.IsEmail(email))
                        {
                            Response.Write(@"{ ""response"":""invalid"",""data"":""Deleted"" }");
                            isValid = false;
                        }
                        else if (String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname))
                        {
                            Response.Write(@"{ ""response"":""invalid"",""data"":""Deleted"" }");
                            isValid = false;
                        }
                        if (isValid)
                        {
                            int max = u.MaxUsers();
                            if (max > 0)
                            {
                                if (max <= u.GetVUIUsers().Count())
                                {
                                    Response.Write(@"{ ""response"":""invalid"",""data"":""You have reached your user limit"" }");
                                    return;
                                }
                            }

                            string retval = VUIfunctions.CreateVUIuser(firstname, lastname, email);
                            if (retval.Equals(VUIfunctions.VUI_USERADMIN_STATUS_SUCCESS) || retval.Equals(VUIfunctions.VUI_USERADMIN_STATUS_EXISTS))
                            {
                                Response.Write(@"{ ""response"":""valid"",""data"":""User added"" }");
                            }
                            else
                            {
                                Response.Write(@"{ ""response"":""valid"",""data"":""" + retval + @""" }");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""There was an error saving the user"" }");
                    }
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Please complete all the fields"" }");
                }
            }
        }



        private void SaveUserDetail()
        {
            if (IsVUIUser())
            {
                string field = Request["field"];
                string data = Request["data"];

                if (field.Equals("linkedin"))
                {
                    u.Member.getProperty("linkedIn").Value = data;
                }
                if (field.Equals("jobtitle"))
                {
                    u.Member.getProperty("jobTitle").Value = data;
                }
                Response.Write(@"{ ""response"":""valid"",""data"":""Updated"" }");
            }
        }

        private void SaveUserImage()
        {
            if (IsVUIUser())
            {
                string filename = @"/vui-media/members/img-" + u.Member.Id;

                HttpFileCollection files = Request.Files;
                if (files.Count > 0)
                {
                    HttpPostedFile f = files[0];

                    string[] contenttypes = { "image/gif","image/jpeg","image/pjpeg","image/png" };
                    if (f.ContentLength > 102400)
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""size""}");
                    }
                    else if(!contenttypes.Contains(f.ContentType))
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""type""}");
                    }
                    else
                    {
                        string extension = f.FileName.Substring(f.FileName.LastIndexOf('.'));

                        filename = filename + extension;

                        try
                        {
                            string savepath = Server.MapPath(filename);
                            f.SaveAs(savepath);

                            u.Member.getProperty("vuiUserImage").Value = filename;

                            Response.Write(@"{ ""response"":""valid"",""data"":""uploaded"", ""files"" : [ { ""name"":""" + filename + @""" } ] }");
                        }
                        catch (Exception e)
                        {
                            Response.Write(@"{ ""response"":""invalid"",""data"":""error""}");
                        }
                    }
                }
            }
        }

        private void GetLatestNews()
        {
            int numitems;
            string _numitems = Request["numitems"];
            Int32.TryParse(_numitems, out numitems);
            if (numitems == null || numitems == 0)
                numitems = 10;
            string json = VUI3News.GetNewsAsJson(numitems);
            Response.Write(json);
        }

        private void Login()
        {
            string prot = Request.ServerVariables["SERVER_PROTOCOL"];
            string user = Request["user"];
            string password = Request["pass"];
            string rUser = Request["rem"];
            
            bool remember = (!String.IsNullOrEmpty(rUser) && rUser.Equals("Y"));
            if (VUI3Utility.MemberLogin(user, password, remember))
            {
                Response.Write(@"{ ""response"":""valid"",""data"":""/vui3/"" }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Incorrect username / password combination. Please try again"" }");
            }
        }

        private void Logout()
        {
            VUI3Utility.MemberLogout();
            Response.Write(@"{ ""response"":""valid"",""data"":""/vui3/"" }");
        }

        private void RecoverPassword()
        {
            string email = Request["email"];

            VUIfunctions.SendPwdResetEmail(email);

            Response.Write(@"{ ""response"":""valid"",""data"":""Your password has been reset and sent to the email address provided"" }");
        }

        private void ChangePassword()
        {
            if (IsVUIUser())
            {
                string message;
                string valid = u.ChangePassword(Request["pwd"].ToString(), out message) ? "valid" : "invalid";

                Response.Write(@"{ ""response"":""" + valid + @""",""data"":""" + message + @""" }");
            }
        }

        private void SearchImages()
        {
            if (IsVUIUser())
            {
                string service = Request["service"];
                string pagetype = Request["pagetype"];
                Response.Write(SearchImagesSingleton.Instance().FindImagesJSON(service, pagetype));
            }
        }

        private void GetImagesForService()
        {
            // Make sure user is loggedin!!!
            if (IsVUIUser())
            {
                int serviceid;
                if (Int32.TryParse(Request["service"], out serviceid))
                {
                    Response.Write(VUIfunctions.ServiceWithImagesToJson(serviceid));
                    // Response.Write(VUIfunctions.ImageListJson(serviceid));
                }
                else
                {
                    Response.Write(VUIfunctions.ImageListJson(Request["service"]));
                }
            }
        }


        private void LightboxImageSearch()
        {
            if (IsVUIUser())
            {
                string service = String.Empty;
                string device = String.Empty;
                string pageType = String.Empty;
                int startnum = 0;
                int maxcount = 100;

                if (!String.IsNullOrEmpty(Request["startfrom"]))
                {
                    Int32.TryParse(Request["startfrom"], out startnum);
                }

                if (!String.IsNullOrEmpty(Request["service"]))
                {
                    service = Request["service"];
                }
                if (!String.IsNullOrEmpty(Request["device"]))
                {
                    device = Request["device"];
                }
                if (!String.IsNullOrEmpty(Request["pagetype"]))
                {
                    pageType = Request["pagetype"];
                }
                Response.Write(@"{ ""response"":""valid"",""data"":" + VUI3Utility.SearchScreenshots(service, device, pageType, startnum, maxcount).AsJson() + " }");
            }
        }


        private void GetFavouriteImages()
        {
            if (IsVUIUser())
            {
                VUI3ScreenshotResults results = new VUI3ScreenshotResults();
                u = VUI3Utility.GetUser();
                List<VUI3Screenshot> screens = new List<VUI3Screenshot>();
                foreach (ImageFavouriteCollection c in u.FavouriteImages.collections)
                {
                    screens.AddRange(c.imageFavourites);
                }
                results.resultStart = 0;
                results.ScreenshotsList = new VUI3ScreenshotList();
                results.ScreenshotsList.screenshots = screens;
                results.totalResults = screens.Count;
                Response.Write(@"{ ""response"":""valid"",""data"":" + results.AsJson() + " }");
            }
        }

        private void GetImagesForPackage()
        {
            int aid;
            int sid;

            if (Int32.TryParse(Request["analysis"], out aid))
            {
                if (Int32.TryParse(Request["service"], out sid))
                {
                    VUI3ServiceMaster sm = new VUI3ServiceMaster(sid);
                    if (IsVUIUser() || sm.IsPreviewable)
                    {
                        Response.Write(@"{ ""response"":""valid"",""data"": " + sm.GetScreenshotPackageWithScreenshots(aid).AsJson() + " }");
                    }
                    else
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Not logged in"" }");
                    }
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid request params"" }");
                }
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid request params"" }");
            }
        }

        private void AddServiceFavourite()
        {
            if (IsVUIUser())
            {
                if (Request["id"] != null && !String.IsNullOrEmpty(Request["id"].ToString()))
                {
                    int id;
                    if (Int32.TryParse(Request["id"].ToString(), out id))
                    {
                        u.AddFavouriteService(id);
                        VUI3ServiceMaster sm = new VUI3ServiceMaster(id);
                        VUI3ServiceSnapshot snap = sm.GetSnapshot();
                        Response.Write(@"{ ""response"":""valid"",""data"": " + snap.AsJson() + " }");
                    }
                    else
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid service"" }");
                    }
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid service"" }");
                }
            }
        }


        private void PopulateContactDetails()
        {
            //if(IsVUIUser())
        }

        private void SaveContactRequest()
        {
            string contactname;
            string company;
            string emailaddress;
            string phone;
            try
            {
                contactname = Request["contactname"].ToString();
                company = Request["company"].ToString();
                emailaddress = Request["emailaddress"].ToString();
                phone = Request["phone"].ToString();

                VUI3Utility.SendContactEmail(contactname, company, emailaddress, phone);

                Response.Write(@"{ ""response"":""valid"",""data"":""Contact form submitted"" }");
            }
            catch (Exception ex)
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid service"" }");
            }
        }

        private void DeleteServiceFavourite()
        {
            if (IsVUIUser())
            {
                if (Request["id"] != null && !String.IsNullOrEmpty(Request["id"].ToString()))
                {
                    int id;
                    if (Int32.TryParse(Request["id"].ToString(), out id))
                    {
                        u.DeleteFavouriteService(id);
                        Response.Write(@"{ ""response"":""valid"",""data"":""Service Deleted"" }");
                    }
                    else
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid service"" }");
                    }
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid service"" }");
                }
            }
        }



        private void DeleteImageFavourite()
        {
            bool ret = false;
            if (IsVUIUser())
            {
                int imageId = 0;
                try
                {
                    if (Int32.TryParse(Request["id"].ToString(), out imageId))
                    {
                        if (Request["collection"] != null && !String.IsNullOrEmpty(Request["collection"].ToString()))
                        {
                            ret = u.DeleteFavouriteImage(imageId, Request["collection"].ToString());
                        }
                        else
                        {
                            ret = u.DeleteFavouriteImage(imageId);
                        }

                        if (ret)
                        {
                            int cnt = u.FavouriteImages.AllFavouriteImages().Count();
                            Response.Write(@"{ ""response"":""valid"",""data"":""Image Deleted"", ""count"":" + cnt + @"}");
                        }
                        else
                        {
                            Response.Write(@"{ ""response"":""invalid"",""data"":""Could not delete image"" }");
                        }
                    }
                    else
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Invlaid image"" }");
                    }
                }
                catch (Exception e)
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid image"" }");
                }
            }
        }


        private void AddImageFavourite()
        {
            if (IsVUIUser())
            {
                int imageId = 0;

                try
                {
                    if (Int32.TryParse(Request["id"].ToString(), out imageId))
                    {
                        if (Request["collection"] != null && !String.IsNullOrEmpty(Request["collection"].ToString()))
                        {
                            u.AddFavouriteImage(imageId, Request["collection"].ToString());
                        }
                        else
                        {
                            u.AddFavouriteImage(imageId);
                        }

                        int cnt = u.FavouriteImages.AllFavouriteImages().Count();
                        VUI3Screenshot s = VUI3Utility.GetScreenshot(imageId);
                        Response.Write(@"{ ""response"":""valid"",""data"":" + s.AsJson() + @" , ""count"":" + cnt + @" }");
                    }
                    else
                    {
                        Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid Image"" }");
                    }
                }
                catch (Exception e) 
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid Image"" }");
                }
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not Logged In"" }");
            }
            // u.DeleteFavouriteImage(2881);
        }

        private void GetImageFavourites()
        {
            if (IsVUIUser())
            {
                Response.Write(@"{ ""response"":""valid"",""data"":" + u.FavouriteImages.AsJson() + @" }");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetImagePackages()
        {
            if (IsVUIUser())
            {
                if (Request["serviceName"] != null && !String.IsNullOrEmpty(Request["serviceName"].ToString()))
                {
                    VUI3ServiceMaster.GetScreenshotPackages(Request["serviceName"].ToString());
                }
                else
                {
                    Response.Write(@"{ ""response"":""invalid"",""data"":""Invalid Service"" }");
                }
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Not Logged In"" }");
            }
        }

        

        /// <summary>
        /// Deprecated
        /// </summary>
        private void FavouriteImage()
        {
            int imageid = -1;

            if (Int32.TryParse(Request["imageid"], out imageid))
            {
                Response.Write(VUIfunctions.FavouriteImage(imageid, Request["action"]));
            }
        }

        private void RateService()
        {
            int rating = -1;
            int serviceid = -1;

            if (Int32.TryParse(Request["rating"], out rating) && Int32.TryParse(Request["serviceid"], out serviceid))
            {
                Response.Write(VUIfunctions.RateService(serviceid, rating));
            }
        }


        private void GetServiceListByName()
        {
            string serviceName = Request["service"];
            if (!String.IsNullOrEmpty(serviceName))
            {
                Response.Write(VUIfunctions.ServiceListByNameToJson(serviceName));
            }
        }

        private void GetAllServiceList()
        {
            Response.Write(VUI3Utility.AllServicesSimple().AsJson());
        }

        private void Deprecated_GetAllServiceList()
        {
            Response.Write(VUIfunctions.ServiceListByAllPlatformsToJson());
        }

        private void GetServiceListByPlatform()
        {
            int platformId = -1;
            if (Int32.TryParse(Request["platformid"], out platformId))
            {
                Response.Write(VUIfunctions.ServiceListByPlatformToJson(platformId));
            }
        }

        private void GetServiceListByServiceNameAndPlatform()
        {
            int platformId = -1;
            string serviceName = Request["service"];
            if (!String.IsNullOrEmpty(serviceName) && Int32.TryParse(Request["platformid"], out platformId))
            {
                Response.Write(VUIfunctions.ServiceListByNameAndPlatformToJson(serviceName, platformId));
            }
        }
        
        const string ACTION_SEARCH_IMAGES = "ss";
        const string ACTION_GET_IMAGES_FOR_SERVICE = "si";
        const string ACTION_GET_SERVICE_LIST_BY_NAME = "sn";
        const string ACTION_GET_SERVICE_LIST_BY_PLATFORM = "sp";
        const string ACTION_GET_SERVICE_LIST_BY_NAME_AND_PLATFORM = "snp";
        const string ACTION_GET_ALL_SERVICE_LIST = "sa";
        const string ACTION_RATE_SERVICE = "rs";
        const string ACTION_FAVOURITE_IMAGE = "fi";
        const string ACTION_UNFAVOURITE_IMAGE = "fu";
        const string ACTION_GET_LATEST_NEWS = "ln";
        const string ACTION_LOGIN = "lo";
        const string ACTION_LOGOUT = "lu";
        const string ACTION_RECOVER_PASSWORD = "pr";
        const string ACTION_GET_FAVOURITE_SERVICES = "fs";
        const string ACTION_GET_FAVOURITE_SCREENSHOTS = "fc";
        const string ACTION_CHANGE_PASWORD = "cp";
        const string ACTION_FAVOURITE_SERVICE = "fsv";
        const string ACTION_UNFAVOURITE_SERVICE = "fuv";
        const string ACTION_SERVICE_IMAGE_PACKAGES = "sip";
        const string ACTION_GET_IMAGES_FOR_ANALYSIS = "sii";
        const string ACTION_LIGHTBOX_IMAGE_SEARCH = "lis";
        const string ACTION_CONTACT_REQUEST = "cr";

        const string ACTION_SAVE_DETAIL = "sud";
        const string ACTION_SAVE_IMAGE = "sui";
        const string ACTION_GET_VUI_USERS = "vus";
        const string ACTION_DELETE_VUI_USER = "vud";
        const string ACTION_ADD_VUI_USER = "vua";

        const string ACTION_CATEGORISE_IMAGE = "cvi";
        const string ACTION_GET_UNCATEGORISED_IMAGES = "uci";
        const string ACTION_GET_STORE_COMMENTS = "scom";
        const string ACTION_GET_STORE_COMMENTS_META = "scmt";
        const string ACTION_GET_APP_VERSIONS = "vers";
        const string ACTION_DELETE_IMAGE = "deli";
    }
}