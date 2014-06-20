using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VP2.businesslogic;
using VPCommon;

namespace VP2.usercontrols
{
    public partial class forbidden_access : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            plcVUIRegistrationComplete.Visible = false; 
            plcConfirmation.Visible = false;
            plcRegisterLogin.Visible = true;


            VPMember currentUser = VPMember.GetLoggedInUser();
            
            // Why is this logged-in user seeing this page? Because they haven't completed the registration process.
            if (currentUser != null)
            {
                string[] roles = currentUser.UserRoles;
                if (roles != null)
                {
                    
                    // If roles contains "active" that means this person shouldn't be on this page - bump them to the home page
                    if (roles.Contains("active"))
                    {
                        Response.Redirect("/", true);
                    }
                    // If it's a non-active registrant, then show them the resend confirmation message.
                    else if (roles.Contains("registrant"))
                    {
                        litEmail.Text = currentUser.User.Email;
                        plcConfirmation.Visible = true;
                        plcRegisterLogin.Visible = false;
                    }
                    // Else it's a VUI-only user. Should these be automatically set to be Registrant / Active?
                    else if (roles.Contains("vui_user") || roles.Contains("vui_Administrator"))
                    {
                        plcVUIRegistrationComplete.Visible = true;
                    }

                }

            }
        }

        protected void ResendConfirmationEmail(object sender, EventArgs e)
        {
            // Send Email...
            VPMember currentUser = VPMember.GetLoggedInUser();
            if (currentUser != null)
            {
                Emailer em = new Emailer("EMAIL_CONFIRM_SIGNUP");
                em.ReplaceMemberElements(currentUser.User);
                em.Send(currentUser.User);
                plcEmailSent.Visible = true;
            }
        }
    }
}