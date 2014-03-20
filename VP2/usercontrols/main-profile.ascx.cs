using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace VP2.usercontrols
{
    public partial class main_profile : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            populateForm();
        }

        private void populateForm()
        {
            umbraco.cms.businesslogic.member.Member member = umbraco.cms.businesslogic.member.Member.GetCurrentMember();

            txtEmail.Text = member.Email;
            txtTitle.Text = member.getProperty("title").Value.ToString();
            txtFirstName.Text = member.getProperty("firstName").Value.ToString();
            txtLastName.Text = member.getProperty("lastName").Value.ToString();
            chkAccept.Checked = Convert.ToBoolean(member.getProperty("receivePromoEmails").Value);
            txtJobTitle.Text = member.getProperty("jobTitle").Value.ToString();
            txtCompanyName.Text = member.getProperty("companyName").Value.ToString();
            txtCompanyAddress1.Text = member.getProperty("companyAddress1").Value.ToString();
            txtCompanyAddress2.Text = member.getProperty("companyAddress2").Value.ToString();
            txtCompanyAddress3.Text = member.getProperty("companyAddress3").Value.ToString();
            txtCompanyTown.Text = member.getProperty("companyTown").Value.ToString();
            txtCounty.Text = member.getProperty("companyState").Value.ToString();
            txtPostcode.Text = member.getProperty("companyPostcodeZip").Value.ToString();
            Country.Text = member.getProperty("companyCountry").Value.ToString();
            txtPhone.Text = member.getProperty("businessPhone").Value.ToString();
            txtMobile.Text = member.getProperty("mobile").Value.ToString();
            txtOrgType.Text = member.getProperty("organisationType").Value.ToString();
            txtEmployees.Text = member.getProperty("numberOfEmployees").Value.ToString();

        }

        protected void SaveDetails(object sender, EventArgs e)
        {
            MsgSaved.Visible = true;

            umbraco.cms.businesslogic.member.Member member = umbraco.cms.businesslogic.member.Member.GetCurrentMember();
            member.Text = txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();

            // Password
            if (!PasswordText.Text.Trim().Equals(""))
            {
                if (!PasswordConfirm.Text.Equals(PasswordText.Text))
                {
                    MsgPasswordConfirm.Text = "The passwords you've entered don't match!";
                    MsgPasswordConfirm.Visible = true;
                    return;
                }
                else
                {
                    member.Password = PasswordText.Text.Trim();
                }
            }

            umbraco.cms.businesslogic.property.Property propTitle = member.getProperty("title"); // Property alias
            umbraco.cms.businesslogic.property.Property propFirstName = member.getProperty("firstName"); // Property alias
            umbraco.cms.businesslogic.property.Property propLastName = member.getProperty("lastName"); // Property alias
            umbraco.cms.businesslogic.property.Property propFullName = member.getProperty("fullName"); // Property alias
            umbraco.cms.businesslogic.property.Property propReceiveEmails = member.getProperty("receivePromoEmails"); // Property alias
            umbraco.cms.businesslogic.property.Property propJobTitle = member.getProperty("jobTitle"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyName = member.getProperty("companyName"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress1 = member.getProperty("companyAddress1"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress2 = member.getProperty("companyAddress2"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyAddress3 = member.getProperty("companyAddress3"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyTown = member.getProperty("companyTown"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyState = member.getProperty("companyState"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyPostcode = member.getProperty("companyPostcodeZip"); // Property alias
            umbraco.cms.businesslogic.property.Property propCompanyCountry = member.getProperty("companyCountry"); // Property alias
            umbraco.cms.businesslogic.property.Property propBusinessPhone = member.getProperty("businessPhone"); // Property alias
            umbraco.cms.businesslogic.property.Property propMobile = member.getProperty("mobile"); // Property alias
            umbraco.cms.businesslogic.property.Property propOrgType = member.getProperty("organisationType"); // Property alias
            umbraco.cms.businesslogic.property.Property propOrgTypeOther = member.getProperty("organisationTypeOther"); // Property alias
            umbraco.cms.businesslogic.property.Property propEmployees = member.getProperty("numberOfEmployees"); // Property alias

            propTitle.Value = txtTitle.Text.Trim();
            propFirstName.Value = txtFirstName.Text.Trim();
            propLastName.Value = txtLastName.Text.Trim();
            propFullName.Value = txtTitle.Text.Trim() + " " + txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();
            propReceiveEmails.Value = chkAccept.Checked;
            propJobTitle.Value = txtJobTitle.Text.Trim();
            propCompanyName.Value = txtCompanyName.Text.Trim();
            propCompanyAddress1.Value = txtCompanyAddress1.Text.Trim();
            propCompanyAddress2.Value = txtCompanyAddress2.Text.Trim();
            propCompanyAddress3.Value = txtCompanyAddress3.Text.Trim();
            propCompanyTown.Value = txtCompanyTown.Text.Trim();
            propCompanyState.Value = txtCounty.Text.Trim();
            propCompanyPostcode.Value = txtPostcode.Text.Trim();
            propCompanyCountry.Value = Country.Text.Trim();
            propBusinessPhone.Value = txtPhone.Text.Trim();
            propMobile.Value = txtMobile.Text.Trim();
            propOrgType.Value = txtOrgType.Text.Trim();
            propOrgTypeOther.Value = txtOrgType.Text.Trim();
            propEmployees.Value = txtEmployees.Text.Trim();

            member.Save();
            MsgSaved.Visible = true;
        }



        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        private static bool IsEmail(string email)
        {
            string MatchEmailPattern =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
               + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
               + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
               + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }
        private bool UserExists(string username)
        {
            if (Membership.GetUser(username) != null) { return true; }

            return false;
        }


        protected void BtnLogOut(object sender, EventArgs e)
        {
            umbraco.cms.businesslogic.member.Member m = umbraco.cms.businesslogic.member.Member.GetCurrentMember();
            if (m != null)
            {
                umbraco.cms.businesslogic.member.Member.RemoveMemberFromCache(m.Id);
                umbraco.cms.businesslogic.member.Member.ClearMemberFromClient(m.Id);
            }
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies["uid"] != null)
            {
                HttpCookie myCookie = new HttpCookie("uid");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
            bool rememberUserId = false;
            if (HttpContext.Current.Request.Cookies["vrl"] != null)
            {
                if (HttpContext.Current.Request.Cookies["vrl"].Value.Equals("Y"))
                {
                    rememberUserId = true;
                }
            }
            if (!rememberUserId)
            {
                if (HttpContext.Current.Request.Cookies["vid"] != null)
                {
                    HttpCookie myCookie = new HttpCookie("vid");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    HttpContext.Current.Response.Cookies.Add(myCookie);
                }
            }
            Response.Redirect("/");
            Response.End();
        }
    }
}