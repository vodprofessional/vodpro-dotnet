using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VP2.businesslogic;
using System.IO;
using System.Configuration;

namespace VP2.usercontrols
{
    public partial class login : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            plcErrLogin.Visible = false;
            plcForgotError.Visible = false;
            plcForgotProblem.Visible = false;
            plcPwdSent.Visible = false;

            if (!IsPostBack)
            {
                if (Request["page"] != null)
                {
                    ReturnPage.Value = Request["page"].ToString();
                }

                if (!String.IsNullOrEmpty(Request.QueryString["forgot"]))
                {
                    plcFormForgot.Visible = true;
                    plcFormLogin.Visible = false;
                }
                else
                {
                    plcFormForgot.Visible = false;
                }
            }

            txtLoginEmail.Attributes["type"] = "email";
            txtLoginEmail.Attributes.Add("autofocus", "autofocus");
            txtLoginEmail.Attributes.Add("placeholder", "Email address");

            txtEmailForgotten.Attributes["type"] = "email";
            txtEmailForgotten.Attributes.Add("placeholder", "Email address");


            txtRegEmail.Attributes["type"] = "email";
            txtRegEmail.Attributes.Add("placeholder", "Email address");
            txtPassword.Attributes.Add("placeholder", "Password");
        }

        protected void lnkForgotPwd_Click(object sender, EventArgs e)
        {
            plcFormForgot.Visible = true;
            plcFormLogin.Visible = false;
        }

        protected void lnkBackToLogin_Click(object sender, EventArgs e)
        {
            plcFormForgot.Visible = false;
            plcFormLogin.Visible = true;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                bool completedLogin = VPMember.MemberLogin(txtLoginEmail.Text, txtPassword.Text);
                if (completedLogin)
                {
                    Response.Redirect(ConfigurationManager.AppSettings["LoggedInRedirect"], true);
                }
                else
                {
                    plcErrLogin.Visible = true;
                }
            }
            catch (Exception ex)
            {
                VPMember currentUser = VPMember.GetLoggedInUser();
                // In the case below, it means that the username and password were valid, but that the user has some other defect.
                if (currentUser != null)
                {

                }
                plcErrLogin.Visible = true;
            }
        }

        protected void btnForgotPwd_Click(object sender, EventArgs e)
        {
            string email = txtEmailForgotten.Text;

            if (!String.IsNullOrEmpty(email))
            {
                if (VPMember.RecoverPassword(email))
                {
                    plcPwdSent.Visible = true;

                    if (!String.IsNullOrEmpty(ReturnPage.Value))
                    {
                        lnkBackToArticle.NavigateUrl = ReturnPage.Value;
                        plcBackToPage.Visible = true;
                    }
                }
                else
                {
                    plcForgotProblem.Visible = true;
                }
            }
            else
            {
                plcForgotError.Visible = true;
            }
        }

        protected void btnReg_Click(object sender, EventArgs e)
        {
            string email = txtRegEmail.Text;
            Response.Redirect("/register?email=" + Server.UrlEncode(email) + "&page=" + ReturnPage.Value, true);
        }
    }
}