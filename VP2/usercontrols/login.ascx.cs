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
            plcPwdSent.Visible = false;

            if (!IsPostBack)
            {
                plcFormForgot.Visible = false;
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
                if (VPMember.MemberLogin(txtLoginEmail.Text, txtPassword.Text))
                {
                    Response.Redirect("/members/profile", true);
                }
                else
                {
                    plcErrLogin.Visible = true;
                }
            }
            catch (Exception ex)
            {
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
            Response.Redirect("/register?email=" + Server.UrlEncode(email), true);
        }
    }
}