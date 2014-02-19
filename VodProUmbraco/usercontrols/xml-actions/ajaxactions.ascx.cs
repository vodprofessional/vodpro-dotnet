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

namespace VODPro.usercontrols
{
    public partial class ajaxactions : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = Request.QueryString["action"];
			Response.Write("<action-response>");
            if (_action.Equals("check"))
            {
                MemberLogin();
            }
			Response.Write("</action-response>");
        }

        private void MemberLogin()
        {
            string username = Request["username"];
            string pwd = Request["password"];
            string message = String.Empty;
			
            if (Membership.ValidateUser(username, pwd))
            {
                Response.Write("<loggedinok>Y</loggedinok>");
            }
            else
            {
                if (Membership.FindUsersByName(username).Count > 0)
                {
                    message = "password";
                }
                else
                {
                    message = "user";
                }
                Response.Write("<loggedinok>N</loggedinok>");
                Response.Write("<error>" + message + "</error>");
            }
        }
    }
}