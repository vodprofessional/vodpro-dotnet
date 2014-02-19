using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using umbraco.cms.businesslogic.member;
using umbraco.MacroEngines;
using VUI.classes;
using umbraco.cms.businesslogic.web;

namespace VUI.usercontrols
{
    public partial class vui_public_user_admin : System.Web.UI.UserControl
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(vui_public_user_admin));

        Member m;
        protected void Page_Load(object sender, EventArgs e)
        {
            m = VUIfunctions.CurrentUser();
            if (m == null)
            {
                log.Debug("Admin - No user. Bye!");
                Response.Redirect(VUIfunctions.VUI_subscribe_page);
                Response.End();
            }
            else if (VUIfunctions.MemberVUIStatus(m).Equals(VUIfunctions.VUI_USERTYPE_NONE) || VUIfunctions.MemberVUIStatus(m).Equals(VUIfunctions.VUI_USERTYPE_REGISTRANT))
            {
                log.Debug("Admin - Not a VUI user. Bye!");
                Response.Redirect(VUIfunctions.VUI_subscribe_page);
                Response.End();
            }


            log.Debug("Admin - Making things visible");

            divDetailsMessage.Visible = false;
            divPwdMessage.Visible = false;
            plcAdministratorDetails.Visible = false;
            plcInvoices.Visible = false;
            plcInstallments.Visible = false;

            if (!IsPostBack)
            {

                log.Debug("Admin - Is Post back");

                PopulateUserDetails();
                GetVUIAdminDetails();

                if (VUIfunctions.CurrentMemberIsVUIAdmin())
                {

                    log.Debug("Admin - admin user. Get child users");
                    GetVUIUsers();
                    plcUsers.Visible = true;


                    log.Debug("Admin - Invoices now");
                    PopulateInvoices();
                    plcInvoices.Visible = true;
                }
                else
                {
                    plcAdministratorDetails.Visible = true;
                }

                if (Session["UserShouldChangePassword"] != null)
                {
                    if ((bool)Session["UserShouldChangePassword"])
                    {
                        litDetailsMessage.Text = "You should change your password to something more memorable";
                        Session.Remove("UserShouldChangePassword");
                        divDetailsMessage.Visible = true;
                    }
                }

            }
            
        }

        protected void PopulateUserDetails()
        {

            log.Debug("Admin - Populating User details");
            Member m = VUIfunctions.CurrentUser();
            txtFirstName.Text = m.getProperty("firstName").Value.ToString();
            txtLastName.Text = m.getProperty("lastName").Value.ToString();
            txtEmail.Text = m.LoginName;
            
        }

        protected void ClearFormAndPopulateList()
        {
            txtVuiUserFirstname.Text = String.Empty;
            txtVuiUserLastname.Text = String.Empty;
            txtVuiEmail.Text = String.Empty;
            GetVUIUsers();
        }

        protected void GetVUIUsers()
        {

            log.Debug("Admin - Getting list of users");
            List<Member> vuiusers = VUIfunctions.GetVUIUsersForAdmin(VUIfunctions.CurrentUser());

            log.Debug("Admin - found" + vuiusers.Count);
            rptVUIusers.DataSource = vuiusers;
            rptVUIusers.DataBind();


            log.Debug("Admin - How many users allowed?");
            string maxUsers = VUIfunctions.CurrentUser().getProperty("vUINumberOfUsersAllowed").Value.ToString();


            log.Debug("Admin - " + maxUsers);
            int max = 0;
            if (Int32.TryParse(maxUsers, out max))
            {
                if (max < 0)
                {

                    log.Debug("Admin - Unlimited");
                    litMaxUsers.Text = @" an umlimited number of ";
                }
                else
                {
                    log.Debug("Admin - up to " + maxUsers);
                    litMaxUsers.Text = @" up to " + maxUsers + " ";
                }
            }
            else
            {
                log.Debug("Admin - Wasn't an integer. Now what?");
                litMaxUsers.Text = VUIfunctions.CurrentUser().getProperty("vUINumberOfUsersAllowed").Value.ToString();
            }
            if (max > 0 && vuiusers.Count >= max)
            {
                log.Debug("Admin - How many users allowed?");
                btnAddVUIUser.ToolTip = "You can't add more users";
                btnAddVUIUser.Enabled = false;
            }
        }

        protected void PopulateInvoices()
        {

            log.Debug("Admin - check the invoice log");
            if (m.getProperty("userInvoiceLog") != null && m.getProperty("userInvoiceLog").Value != null)
            {

                log.Debug("Admin - How many users allowed?");
                string userInvoices = m.getProperty("userInvoiceLog").Value.ToString().Replace("{", "").Replace("}", "");

                log.Debug("Admin - " + userInvoices);

                List<Invoice> invoices = new List<Invoice>();

                if (!String.IsNullOrEmpty(userInvoices))
                {

                    log.Debug("Getting the XML");

                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(userInvoices);

                    XmlNodeList transList;
                    transList = xml.SelectSingleNode("items").SelectNodes("item");

                    foreach (XmlNode item in transList)
                    {
                        Invoice i = new Invoice();
                        i.Number = item.SelectSingleNode("invoiceNumber").InnerText;
                        i.DocId = Int32.Parse(item.SelectSingleNode("invoiceItem").InnerText);
                        i.Date = DateTime.Parse(item.SelectSingleNode("invoiceDate").InnerText);
                        invoices.Add(i);
                    }
                }
                if (invoices.Count > 0)
                {
                    log.Debug("Admin - There are some invoices");
                    rptInvoice.DataSource = invoices.OrderBy(i => i.Number).ToList<Invoice>();
                    rptInvoice.DataBind();
                }
                else
                {
                    log.Debug("Admin - There are no invoices");
                    plcInvoices.Visible = false;
                }
            }
            else
            {

                log.Debug("Admin - How many users allowed?");
                plcInvoices.Visible = false;
            }

        }


        protected void GetVUIAdminDetails()
        {

            log.Debug("Admin - Getting Admin detials");
            Member m;
            if (VUIfunctions.CurrentMemberIsVUIAdmin())
            {
                log.Debug("Admin - This user is the admin");
                m = VUIfunctions.CurrentUser();
            }
            else
            {
                log.Debug("Admin - A different user is the admin");
                m = VUIfunctions.GetVUIAdminForCurrentUser();
            }


            log.Debug("Admin - Filling in the link to administrator details");
            linkAdminName.Text = m.getProperty("fullName").Value.ToString();
            linkAdminName.NavigateUrl = @"mailto:" + m.Email;


            log.Debug("Admin - Filling account details");
            txtAccountType.Text = m.getProperty("vuiSubscriptionPackage").Value.ToString();
            txtStartDate.Text = m.getProperty("vuiJoinDate").Value.ToString();

            string nextInstallment = m.getProperty("nextInstallmentProduct").Value.ToString();
            if (!String.IsNullOrEmpty(nextInstallment))
            {
                log.Debug("Found an outstanding installment: [" + nextInstallment + "]");
                plcInstallments.Visible = true;
                int nextInstallmentId;
                if (Int32.TryParse(nextInstallment, out nextInstallmentId))
                {
                    Document d = new Document(nextInstallmentId);
                    string productCode = d.getProperty("productCode").Value.ToString();

                    txtInstallment.Text = productCode + " - " + d.getProperty("productCode").Value.ToString();
                    log.Debug("Outstanding installment product code: [" + productCode + "]");
                    hidNextInstallment.Value = productCode;
                }
            }
        }


        protected void BuyInstallment(object sender, EventArgs e)
        {
            Response.Redirect("/vui/subscribe/process/BUY/" + hidNextInstallment.Value);
        }

        protected void VUIUser_Bound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                log.Debug("Admin - binding a User");
                Member m = (Member)e.Item.DataItem;
                Literal u = e.Item.FindControl("litUserDetails") as Literal;
                ((Button)(e.Item.FindControl("btnDeleteUser"))).CommandArgument = m.LoginName;
                u.Text = m.Email;
                log.Debug("Admin - finsihed binding");
            }
        }


        protected void Invoice_Bound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                log.Debug("Admin - Binding an invoice");
                Invoice i = (Invoice)(e.Item.DataItem);
                HyperLink lnk = e.Item.FindControl("lnkInvoice") as HyperLink;
                Literal lit = e.Item.FindControl("litInvoice") as Literal;
                DynamicNode invoice = new DynamicNode(i.DocId);
                string status = invoice.GetProperty("invoiceStatus").Value.ToString();

                lnk.NavigateUrl = umbraco.library.NiceUrl(i.DocId);
                lnk.Text = "INV-" + i.Number;
                lit.Text = " - " + i.Date.ToString("dd MMM yyyy") + " - <strong>" + status + "</strong>";
                log.Debug("Admin - finsihed binding");
            }
        }

        private bool PasswordOk(string pwd)
        {
            return Regex.IsMatch(pwd, @"^.{6,}(?<=\d.*)(?<=[^a-zA-Z0-9].*)$");
        }

        protected void ChangePassword(object sender, EventArgs e)
        {
            litPwdErr.Text = "";
            litDetailsMessage.Text = "";

            List<string> errsPwd = new List<string>();
            

            if (String.IsNullOrEmpty(txtPassword1.Text) || String.IsNullOrWhiteSpace(txtPassword1.Text))
            {
                errsPwd.Add("You cannot have an empty password");
            }
            else if (txtPassword1.Text.Length < 6)
            {
                errsPwd.Add("That password is too short, enter something at least 6 characters long");
            }
            if (txtPassword1.Text.Length > 20)
            {
                errsPwd.Add("A password can have a maximum of 20 characters, you entered " + txtPassword1.Text.Length.ToString());
            }
            if (!PasswordOk(txtPassword1.Text))
            {
                errsPwd.Add("Your password should contain a mixture of letters and numbers and at least one funny character");
            }
            if (txtPassword1.Text != txtPassword2.Text)
            {
                errsPwd.Add("Darn it, your passwords didn't match!");
            }
            if (errsPwd.Count > 0)
            {
                litPwdErr.Text = String.Join("<br/>", errsPwd);
                divPwdMessage.Visible = true;
            }
            else
            {
                Member m = VUIfunctions.CurrentUser();
                m.Password = txtPassword1.Text;
                m.Save();
                litDetailsMessage.Text = "Password changed";
                divDetailsMessage.Visible = true;

            }
        }

        protected void UpdateDetails(object sender, EventArgs e)
        {
            List<string> msgs = new List<string>();

            if (String.IsNullOrEmpty(txtFirstName.Text) || String.IsNullOrWhiteSpace(txtFirstName.Text) ||
                String.IsNullOrEmpty(txtLastName.Text) || String.IsNullOrWhiteSpace(txtLastName.Text))
            {
                msgs.Add("Please enter your first and last names");
            }
            if (msgs.Count > 0)
            {
                litDetailsMessage.Text = String.Join("<br/>", msgs);
                divDetailsMessage.Visible = true;
            }
            else
            {
                Member m = VUIfunctions.CurrentUser();
                m.getProperty("firstName").Value = txtFirstName.Text;
                m.getProperty("lastName").Value = txtLastName.Text;
                m.getProperty("fullName").Value = txtFirstName.Text + " " + txtLastName.Text;
                m.Save();
                litDetailsMessage.Text = "Details changed";
                divDetailsMessage.Visible = true;
            }
        }


        // For Admins - Create vui_user
        protected void CreateVUIUser(object sender, EventArgs e) 
        {
            string firstname = txtVuiUserFirstname.Text;
            string lastname = txtVuiUserLastname.Text;
            string email = txtVuiEmail.Text;

            bool isValid = true;

            if (!VUIfunctions.IsEmail(email))
            {
                errEmail.Text = "Enter a valid email address";
                errEmail.Visible = true;
                isValid = false;
            }
            if (String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname))
            {
                errName.Text = "Enter the user's first and last name";
                errName.Visible = true;
                isValid = false;
            }
            if (isValid)
            {
                string retval = VUIfunctions.CreateVUIuser(firstname, lastname, email);
                if (retval.Equals(VUIfunctions.VUI_USERADMIN_STATUS_SUCCESS) || retval.Equals(VUIfunctions.VUI_USERADMIN_STATUS_EXISTS))
                {
                    ClearFormAndPopulateList();
                }
                else
                {
                    errGeneral.Text = retval;
                    errGeneral.Visible = true;
                }
            }
        }



        protected void VUIUsers_Command(object sender, RepeaterCommandEventArgs e) 
        { 
            if(e.CommandName.Equals("DeleteVUIUser"))
            try
            {
                Membership.DeleteUser(e.CommandArgument.ToString());
                ClearFormAndPopulateList();
            }
            catch (Exception ex)
            {
                errGeneral.Text = "Error deleting user";
                errGeneral.Visible = true;
            }
        }

        protected void EditVUIUser(object sender, EventArgs e) { }



    }

    class Invoice
    {
        public Invoice()
        {
            ;
        }
        public string Number { get; set; }
        public int DocId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }

}