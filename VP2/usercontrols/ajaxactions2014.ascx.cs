using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VP2.businesslogic;

namespace VP2.usercontrols
{
    public partial class ajaxactions2014 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["a"];
            switch (action)
            {
                case "any": LoadStories(ArticleList.TYPE_ANY); break;
                case "features": LoadStories(ArticleList.TYPE_FEATURE); break;
                case "news": LoadStories(ArticleList.TYPE_NEWS); break;
                case "l": Login(); break;
                case "uc": UserNameCheck(); break; // Username Check
                case "sr": Search(); break;
                case "jobs": LoadJobs(); break;
            }
        }

        /// <summary>
        /// Load Stories for JSON, used by the lazy-loading feature on home and section pages.
        /// </summary>
        /// <param name="type"></param>
        private void LoadStories(string type)
        {
            try
            {
                int startnum = 0;
                int count = 20;
                int rootNodeId = -1;

                Int32.TryParse(Request["s"], out startnum);
                Int32.TryParse(Request["c"], out count);
                Int32.TryParse(Request["r"], out rootNodeId);

                ArticleList articles;

                if (type.Equals(ArticleList.TYPE_ANY))
                {
                    articles = new ArticleList(rootNodeId);
                }
                else
                {
                    articles = new ArticleList(type);
                }
                articles.LoadNext(count, startnum);
                string json = articles.AsJson();
                Response.Write(WrapJSON("valid",json));
            }
            catch(Exception ex)
            {
                Response.Write(WrapJSON("invalid",ex.Message));
            }
        }

        /// <summary>
        /// Load Jobs in JSON for continuous scroll on jobs page.
        /// </summary>
        private void LoadJobs()
        {
            try
            {
                int startnum = 0;
                int count = 20;

                Int32.TryParse(Request["s"], out startnum);
                Int32.TryParse(Request["c"], out count);

                VPJobs jobs = new VPJobs();
                jobs.PopulateJobList(startnum, count);
                string json = jobs.AsJson();
                Response.Write(WrapJSON("valid",json));
            }
            catch(Exception ex)
            {
                Response.Write(WrapJSON("invalid",ex.Message));
            }
        }


        private void Login()
        {
            string prot = Request.ServerVariables["SERVER_PROTOCOL"];
            string user = Request["user"];
            string password = Request["pass"];
            string rUser = Request["rem"];

            bool remember = (!String.IsNullOrEmpty(rUser) && rUser.Equals("Y"));
            if(String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password))
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Incorrect email / password combination. Please try again"" }");
            }
            else if (VPMember.MemberLogin(user, password, remember))
            {
                Response.Write(@"{ ""response"":""valid"",""data"":""/vui3/"" }");
            }
            else
            {
                Response.Write(@"{ ""response"":""invalid"",""data"":""Incorrect email / password combination. Please try again"" }");
            }
        }


        private void UserNameCheck()
        {
            string user = Request["user"];

            if (!String.IsNullOrEmpty(user))
            {
                if (VPMember.MemberExists(user))
                {
                    Response.Write(@"{ ""response"":""invalid"" }");
                }
                else
                {
                    Response.Write(@"{ ""response"":""valid"" }");
                }
            }
        }


        private void Search()
        {
            try
            {
                int startnum = 0;
                int count = 20;
                string term = String.Empty;

                Int32.TryParse(Request["s"], out startnum);
                Int32.TryParse(Request["c"], out count);
                term = Request["t"];

                ArticleList articles = ArticleList.ListFromSearch(term, count, startnum) ;
                string json = articles.AsJson();
                Response.Write(WrapJSON("valid", json));
            }
            catch (Exception ex)
            {
                Response.Write(WrapJSON("invalid", ex.Message));
            }
        }


        private string WrapJSON(string response, string data)
        {
            string json = @"{{ ""response"":""{0}"", ""data"": {1} }}";
            return String.Format(json, response, data);
        }
    }
}