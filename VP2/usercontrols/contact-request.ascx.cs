using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VP2.businesslogic;

namespace VP2.usercontrols
{
    public partial class contact_request : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitContactForm(object sender, EventArgs e)
        {
            string body = txtBody.Text + "\n\nReason: " + ddWhy.Text;

            ContactRequest.SendRequest("kauserkanji@vodprofessional.com", txtSubject.Text, body, txtEmail.Text, txtName.Text);

            Response.Redirect(umbraco.library.NiceUrl(1187));
        }
    }
}