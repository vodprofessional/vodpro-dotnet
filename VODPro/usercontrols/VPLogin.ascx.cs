using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;
using System.Web.Security;


namespace VODPro.usercontrols
{
    public partial class VPLogin : System.Web.UI.UserControl
    {

        Object currentUser = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = Membership.GetUser();
            plcErrorMessage.Visible = false;
            if (IsPostBack)
            {  
                
            }
        }

        protected void Login(object sender, CommandEventArgs e)
        {
            if (currentUser == null)
            {
                if (Membership.ValidateUser(VPUserName.Text, VPPassword.Text))
                {
                    FormsAuthentication.SetAuthCookie(VPUserName.Text, true);
                    
                }
                else
                {
                    if (Membership.FindUsersByName(VPUserName.Text).Count > 0)
                    {
                        // Show error message
                        Err.Text = "Oops, that's the wrong password for this email address";
                    }
                    else
                    {
                        Err.Text = "That username doesn't exist. Have you registered?";
                    }
                    plcErrorMessage.Visible = true;
                }
            }
        }
    }
}