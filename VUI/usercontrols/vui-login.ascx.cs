using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VUI.classes;
using umbraco.cms.businesslogic.member;
using System.Web.Security;
using VUI.classes;


namespace VUI.usercontrols
{
    public partial class vui_login : System.Web.UI.UserControl
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(vui_login));
        public const string MODE_FORM = "FORM";
        public const string MODE_INFO = "INFO";

        public string Mode { get; set; }
        public string NextPage { get; set; }
        string user_status = VUIfunctions.VUI_USERTYPE_NONE;

        protected void Page_Load(object sender, EventArgs e)
        {
            Member m = VUIfunctions.CurrentUser();
            plcVUIResetPassword.Visible = false;
            plcVUIResetPasswordMessage.Visible = false;
            divPwdRecoveryError.Visible = false;
            litError.Visible = false;


            // THis handles the one-use email link
            // Once clicked, the user is logged in and is prompted to change their password
            if (Request.QueryString != null)
            {
                string qs = Request.QueryString.ToString();

                if (VUIfunctions.IsEmail(Server.UrlDecode(qs)))
                {
                    txtEmail.Text = Server.UrlDecode(qs);
                    ViewState.Add("UserShouldChangePassword", true);
                }
                else if (Session["loginnext"] != null && !String.IsNullOrEmpty(Session["loginnext"].ToString()))
                {
                    NextPage = Session["loginnext"].ToString();
                }
            }

            if (m != null)
            {
                user_status = VUIfunctions.MemberVUIStatus(m);
            }

            if (Mode.Equals(MODE_FORM))
            {
                plcViewDetails.Visible = false;
                // 1. If User is logged in, don't show

                if (user_status.Equals(VUIfunctions.VUI_USERTYPE_NONE))
                {
                    plcVUILogin.Visible = true;
                }
            }
            if(Mode.Equals(MODE_INFO))
            {
                plcViewDetails.Visible = true;
                if (user_status.Equals(VUIfunctions.VUI_USERTYPE_NONE))
                {
                    plcLoggedIn.Visible = false;
                    plcLoggedOut.Visible = true;
                }
                else if (user_status.Equals(VUIfunctions.VUI_USERTYPE_REGISTRANT))
                {
                    PopulateLoggedInInfo(m);
                    plcLoggedIn.Visible = true;
                    lnkLogout.Visible = true;
                    lnkSubscribe.Visible = true;
                    plcLoggedOut.Visible = false;
                }
                else
                {
                    PopulateLoggedInInfo(m);
                    plcLoggedIn.Visible = true;
                    plcLoggedOut.Visible = false;
                }
            }
        }

        protected void PopulateLoggedInInfo(Member m)
        {
            litUser.Text = m.LoginName;
        }

        protected void Subscribe_Click(object sender, EventArgs e)
        {
            Response.Redirect(VUIfunctions.VUI_subscribe_page);
            Response.End();
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Member m = VUIfunctions.CurrentUser();
            if (m != null)
            {
                Member.RemoveMemberFromCache(m.Id);
                Member.ClearMemberFromClient(m.Id);
            }
            Session.RemoveAll();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies["uid"] != null)
            {
                HttpCookie myCookie = new HttpCookie("uid");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }

            Response.Redirect(umbraco.library.NiceUrl(VUIfunctions.VUIMediaRootNode));

        }

        protected void Login_Click(object sender, EventArgs e)
        {
            log.Debug("Will redirect to: " + Request.Url.PathAndQuery);

            if (Request.Url.PathAndQuery.Contains("?umbPageID"))
            {
                string leftOfQuestion = Request.Url.PathAndQuery.Substring(0,Request.Url.PathAndQuery.IndexOf("?"));
                Session["loginnext"] = leftOfQuestion;
            }
            else
            {
                Session["loginnext"] = Request.Url.PathAndQuery;
            }
            Response.Redirect(VUIfunctions.VUI_login_page);
        }


        public void btnForgotPwd_Click(object sender, EventArgs e)
        {
            plcVUIResetPassword.Visible = true;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
            {
                litError.Text = "Please enter the email you use to login to VUI";
                litError.Visible = true;
                return;
            }
            if (String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
            {
                litError.Text = "Please enter your password";
                litError.Visible = true;
                plcVUIResetPassword.Visible = true;
                return;
            }

            try
            {
                bool LoggedIn = VUIfunctions.MemberLogin(email, password);

                if (LoggedIn)
                {
                    if (ViewState["UserShouldChangePassword"] != null)
                    {
                        if ((bool)(ViewState["UserShouldChangePassword"]))
                        {
                            Session.Add("UserShouldChangePassword", true);
                            Response.Redirect(VUIfunctions.VUI_admin_page);
                            Response.End();
                        }
                    }

                    if (!String.IsNullOrEmpty(NextPage))
                    {
                        if(NextPage.Contains("/subscribe") && Session["VUI_PRODUCT_CODE"] != null)
                        {
                            Response.Redirect(NextPage + Session["VUI_PRODUCT_CODE"]);
                        }
                        else
                        {
                            Session.Remove("VUI_PRODUCT_CODE");
                            Response.Redirect(NextPage);
                        }
                        Response.End();
                    }

                    Response.Redirect(umbraco.library.NiceUrl(VUIfunctions.VUIMediaRootNode));
                    Response.End();
                }
                else
                {
                    litError.Text = "You entered an incorrect email / password combination";
                    litError.Visible = true;
                    plcVUIResetPassword.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string result = ex.Message;
                litError.Text = result;
            }
        }

        protected void btnRecoverPassword_Click(object sender, EventArgs e)
        {
            string email = txtEmailRecover.Text;
            if (!String.IsNullOrEmpty(email))
            {
                VUIfunctions.SendPwdResetEmail(email);
                plcVUIResetPasswordMessage.Visible = true;
            }
            else
            {
                plcVUIResetPassword.Visible = true;
                divPwdRecoveryError.Visible = true;
            }
        }
    }
}